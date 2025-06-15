
using DrawPT.Data.Repositories;
using DrawPT.Data.Repositories.Game;
using DrawPT.Common.Models.Game;
using DrawPT.Common.Services.AI;
using Microsoft.AspNetCore.Mvc;


namespace DrawPT.Api.Controllers
{
    [Route("api/[controller]")]
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
        public ActionResult<DailyQuestionEntity> GetDailyPrompt()
        {
            var todaysQuestion = _dailiesRepository.GetDailyQuestion(DateTime.UtcNow.Date);
            if (todaysQuestion != null)
            {
                return Ok(todaysQuestion);
            }

            return NotFound("No daily question found for today.");
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
                if (userId != null)
                    saveToDb = true;

                var playerAnswer = new PlayerAnswer
                {
                    PlayerId = userId != null ? Guid.Parse(userId) : Guid.NewGuid(),
                    Guess = answer,
                    SubmittedAt = DateTime.UtcNow
                };

                var assessment = await _dailyAIService.AssessAnswerAsync(todaysQuestion.OriginalPrompt, [playerAnswer]);

                if (assessment == null || assessment.Count == 0)
                    return BadRequest("Failed to assess the answer.");

                return Ok(assessment.First());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
