
using DrawPT.Common.Models.Daily;
using DrawPT.Common.Services.AI;
using DrawPT.Data.Repositories;
using DrawPT.Data.Repositories.Game;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DrawPT.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DailyPromptController : ControllerBase
    {
        private readonly DailiesRepository _dailiesRepository;
        private readonly DailyAIService _dailyAIService;

        public DailyPromptController(DailiesRepository dailiesRepository, DailyAIService dailyAIService)
        {
            _dailiesRepository = dailiesRepository ?? throw new ArgumentNullException(nameof(dailiesRepository), "DailiesRepository cannot be null.");
            _dailyAIService = dailyAIService ?? throw new ArgumentNullException(nameof(dailyAIService), "DailyAIService cannot be null.");
        }

        /// <summary>
        /// Gets the daily prompt for the current date.
        /// </summary>
        [HttpGet]
        public ActionResult<DailyQuestionEntity> GetDailyQuestionPrompt()
        {
            var todaysQuestion = _dailiesRepository.GetDailyQuestion(DateTime.Now.Date);
            if (todaysQuestion != null)
            {
                return Ok(todaysQuestion);
            }

            return NotFound("No daily question found for today.");
        }

        /// <summary>
        /// Gets the daily prompt for the current date.
        /// </summary>
        [Authorize]
        [HttpGet("myanswer")]
        public ActionResult<DailyQuestionEntity> GetDailyAnswerPrompt()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in claims.");
            }

            var todaysAnswers = _dailiesRepository.GetDailyAnswersByPlayerId(Guid.Parse(userId), DateTime.Now.Date);
            if (todaysAnswers != null && todaysAnswers.Any())
            {
                return Ok(todaysAnswers.FirstOrDefault());
            }

            return NotFound("No daily answer found for today.");
        }


        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> AnswerTodaysDaily([FromBody] string answer)
        {
            if (string.IsNullOrEmpty(answer))
            {
                return BadRequest("Answer cannot be null.");
            }
            try
            {
                var todaysQuestion = _dailiesRepository.GetDailyQuestion(DateTime.UtcNow.Date);

                var saveToDb = false;
                var userId = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
                Guid parsedUserId = Guid.Empty;
                if (userId != null && Guid.TryParse(userId, out parsedUserId))
                    saveToDb = true;

                var playerAnswer = new DailyAnswerPublic
                {
                    PlayerId = Guid.NewGuid(),
                    Guess = answer,
                    Date = todaysQuestion.Date,
                    Reason = ""
                };

                var assessment = await _dailyAIService.AssessAnswerAsync(todaysQuestion.OriginalPrompt, playerAnswer);

                if (assessment == null)
                    return BadRequest("Failed to assess the answer.");


                if (saveToDb)
                {
                    var answerToSave = new DailyAnswerEntity
                    {
                        Date = todaysQuestion.Date,
                        PlayerId = parsedUserId,
                        QuestionId = todaysQuestion.Id,
                        Guess = playerAnswer.Guess,
                        Score = assessment.Score,
                        ClosenessArray = assessment.ClosenessArray,
                        Reason = assessment.Reason,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _dailiesRepository.SaveDailyAnswer(answerToSave);
                }

                var answerPublic = new DailyAnswerPublic()
                {
                    Date = todaysQuestion.Date,
                    PlayerId = playerAnswer.PlayerId,
                    Guess = playerAnswer.Guess,
                    Score = assessment.Score,
                    Reason = assessment.Reason,
                    ClosenessArray = assessment.ClosenessArray,
                };
                return Ok(answerPublic);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
