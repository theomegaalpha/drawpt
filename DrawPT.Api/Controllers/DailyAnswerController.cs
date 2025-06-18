using DrawPT.Common.Models.Daily;
using DrawPT.Common.Services;
using DrawPT.Common.Services.AI;
using DrawPT.Common.Util;
using DrawPT.Data.Repositories;
using DrawPT.Data.Repositories.Game;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrawPT.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DailyAnswerController : ControllerBase
    {
        private readonly DailiesRepository _dailiesRepository;
        private readonly DailyAIService _dailyAIService;
        private readonly PlayerService _profileService; // Added ProfileService

        public DailyAnswerController(DailiesRepository dailiesRepository, DailyAIService dailyAIService, PlayerService profileService) // Added profileService parameter
        {
            _dailiesRepository = dailiesRepository ?? throw new ArgumentNullException(nameof(dailiesRepository), "DailiesRepository cannot be null.");
            _dailyAIService = dailyAIService ?? throw new ArgumentNullException(nameof(dailyAIService), "DailyAIService cannot be null.");
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService), "ProfileService cannot be null."); // Initialize ProfileService
        }


        /// <summary>
        /// Gets the daily answer for today.
        /// </summary>
        [Authorize]
        [HttpGet]
        public ActionResult<DailyAnswerPublic> GetMyDailyAnswer()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in claims.");
            }

            var todaysAnswers = _dailiesRepository.GetDailyAnswersByPlayerId(Guid.Parse(userId), TimezoneHelper.Now());
            if (todaysAnswers != null && todaysAnswers.Any())
            {
                return Ok(todaysAnswers.FirstOrDefault());
            }

            return NotFound("No daily answer found for today.");
        }

        /// <summary>
        /// Gets the daily answer for today.
        /// </summary>
        [HttpGet("top20")]
        public async Task<ActionResult<IEnumerable<DailyAnswerPublic>>> GetTop20DailyAnswers() // Changed to async Task
        {
            var todaysAnswers = _dailiesRepository.GetDailyAnswers(TimezoneHelper.Now());

            if (todaysAnswers != null && todaysAnswers.Any())
            {
                var top20Answers = todaysAnswers
                    .OrderByDescending(a => a.Score)
                    .Take(20)
                    .ToList(); // Convert to List to iterate multiple times if needed or to modify

                var result = new List<DailyAnswerPublic>();
                foreach (var answer in top20Answers)
                {
                    var profile = await _profileService.GetPlayerAsync(answer.PlayerId);
                    result.Add(new DailyAnswerPublic
                    {
                        PlayerId = answer.PlayerId,
                        Username = profile?.Username ?? "Unknown", // Use profile username, default to "Unknown"
                        Avatar = profile?.Avatar,
                        Date = answer.Date,
                        Guess = answer.Guess,
                        Reason = answer.Reason,
                        ClosenessArray = answer.ClosenessArray,
                        Score = answer.Score
                    });
                }

                return Ok(result);
            }


            if (todaysAnswers != null)
                return Ok(todaysAnswers);

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
                var todaysQuestion = _dailiesRepository.GetDailyQuestion(TimezoneHelper.Now());

                var saveToDb = false;
                var userId = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
                Guid parsedUserId = Guid.Empty;
                if (userId != null && Guid.TryParse(userId, out parsedUserId))
                    saveToDb = true;

                var playerAnswer = new DailyAnswerPublic
                {
                    PlayerId = Guid.NewGuid(),
                    Username = User.Identity?.Name ?? "Unknown", // Added Username
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
                    Username = User.Identity?.Name ?? "Unknown",
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
