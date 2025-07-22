using DrawPT.Common.Models.Daily;
using DrawPT.Common.Services;
using DrawPT.Common.Services.AI;
using DrawPT.Common.Util;
using DrawPT.Data.Repositories;
using DrawPT.Data.Repositories.Game;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using DrawPT.Api.Hubs;

namespace DrawPT.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DailyAnswerController : ControllerBase
    {
        private readonly DailiesRepository _dailiesRepository;
        private readonly DailyAIService _dailyAIService;
        private readonly PlayerService _profileService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public DailyAnswerController(
            DailiesRepository dailiesRepository,
            DailyAIService dailyAIService,
            PlayerService profileService,
            IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _dailiesRepository = dailiesRepository ?? throw new ArgumentNullException(nameof(dailiesRepository), "DailiesRepository cannot be null.");
            _dailyAIService = dailyAIService ?? throw new ArgumentNullException(nameof(dailyAIService), "DailyAIService cannot be null.");
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService), "ProfileService cannot be null.");
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext), "HubContext cannot be null.");
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
        [Authorize]
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<DailyAnswerPublic>>> GetMyDailyAnswers()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in claims.");
            }

            var answerHistory = _dailiesRepository.GetDailyAnswersByPlayerId(Guid.Parse(userId));
            if (answerHistory == null)
                return NotFound("No daily answer found for today.");


            var profile = await _profileService.GetPlayerAsync(Guid.Parse(userId));
            var result = new List<DailyAnswerPublic>();
            foreach (var answer in answerHistory)
            {
                result.Add(new DailyAnswerPublic
                {
                    Id = answer.Id,
                    PlayerId = answer.PlayerId,
                    Username = profile?.Username ?? "Username not found.",
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
                    .ToList();

                var result = new List<DailyAnswerPublic>();
                foreach (var answer in top20Answers)
                {
                    var profile = await _profileService.GetPlayerAsync(answer.PlayerId);
                    result.Add(new DailyAnswerPublic
                    {
                        Id = answer.Id,
                        PlayerId = answer.PlayerId,
                        Username = profile?.Username ?? "Guest User",
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
                if (todaysQuestion == null)
                {
                    return NotFound("No daily question found for today.");
                }

                var anonymousUser = true;
                var userId = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
                Guid.TryParse(userId, out Guid parsedUserId);

                // no need to check anonymousUser,  just work off missing profile
                var profile = await _profileService.GetPlayerAsync(parsedUserId);
                var playerAnswer = new DailyAnswerPublic
                {
                    PlayerId = Guid.NewGuid(),
                    Username = profile?.Username ?? "Guest User",
                    Avatar = profile?.Avatar,
                    Guess = answer,
                    Date = todaysQuestion.Date,
                    Reason = ""
                };

                var assessment = await _dailyAIService.AssessAnswerAsync(todaysQuestion.OriginalPrompt, playerAnswer);

                if (assessment == null)
                    return BadRequest("Failed to assess the answer.");

                var answerToSave = new DailyAnswerEntity
                {
                    Id = Guid.NewGuid(),
                    Date = todaysQuestion.Date,
                    PlayerId = anonymousUser ? new Guid() : parsedUserId,
                    QuestionId = todaysQuestion.Id,
                    Guess = playerAnswer.Guess,
                    Score = assessment.Score,
                    ClosenessArray = assessment.ClosenessArray,
                    Reason = assessment.Reason,
                    CreatedAt = DateTime.UtcNow
                };
                var savedAnswer = await _dailiesRepository.SaveDailyAnswerAsync(answerToSave);

                var answerPublic = new DailyAnswerPublic()
                {
                    Id = savedAnswer.Id,
                    Date = todaysQuestion.Date,
                    Username = profile?.Username ?? "Guest User",
                    Avatar = profile?.Avatar,
                    Guess = playerAnswer.Guess,
                    Score = assessment.Score,
                    Reason = assessment.Reason,
                    ClosenessArray = assessment.ClosenessArray,
                };
                // Broadcast to all connected clients via SignalR
                await _hubContext.Clients.All.NewDailyAnswer(answerPublic);
                return Ok(answerPublic);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateDailyAnswer(Guid id, [FromBody] DailyAnswerPublic answer)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            if (answer == null || answer.Id == Guid.Empty)
            {
                return BadRequest("Invalid answer data.");
            }
            try
            {
                var existingAnswer = _dailiesRepository.GetDailyAnswers(answer.Date).FirstOrDefault(da => da.Id == answer.Id);
                if (existingAnswer == null)
                    return NotFound("No existing daily answer found for the player on this date.");

                if (existingAnswer.PlayerId != new Guid())
                    return BadRequest("You cannot update an existing answer that belongs to a player.");

                Guid.TryParse(userId, out Guid parsedUserId);

                if (parsedUserId == Guid.Empty)
                    return Unauthorized("User ID not found in claims.");

                existingAnswer.PlayerId = parsedUserId;
                var savedAnswer = await _dailiesRepository.UpdateDailyAnswerAsync(existingAnswer);

                var profile = await _profileService.GetPlayerAsync(parsedUserId);
                answer.Id = savedAnswer.Id;
                answer.Username = profile?.Username ?? "Guest User";
                answer.Avatar = profile?.Avatar;

                // Broadcast updated answer to all clients
                await _hubContext.Clients.All.NewDailyAnswer(answer);
                return Ok(answer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
