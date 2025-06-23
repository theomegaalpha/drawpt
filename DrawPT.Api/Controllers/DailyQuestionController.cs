using DrawPT.Common.Models.Daily;
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
    public class DailyQuestionController : ControllerBase
    {
        private readonly DailiesRepository _dailiesRepository;
        private readonly DailyAIService _dailyAIService;

        public DailyQuestionController(DailiesRepository dailiesRepository, DailyAIService aiService)
        {
            _dailiesRepository = dailiesRepository ?? throw new ArgumentNullException(nameof(dailiesRepository), "DailiesRepository cannot be null.");
            _dailyAIService = aiService ?? throw new ArgumentNullException(nameof(aiService), "DailyAIService cannot be null.");
        }

        /// <summary>
        /// Gets the daily prompt for the current date.
        /// </summary>
        [HttpGet]
        public ActionResult<DailyQuestionPublic> GetDailyQuestion()
        {
            var todaysQuestion = _dailiesRepository.GetDailyQuestion(TimezoneHelper.Now());
            if (todaysQuestion != null)
            {
                var dailyQuestionPublic = new DailyQuestionPublic
                {
                    Date = todaysQuestion.Date,
                    Style = todaysQuestion.Style,
                    Theme = todaysQuestion.Theme,
                    ImageUrl = todaysQuestion.ImageUrl,
                };
                return Ok(dailyQuestionPublic);
            }

            return NotFound("No daily question found for today.");
        }


        [HttpGet("list")]
        public ActionResult<DailyQuestionPublic> GetDailyQuestions()
        {
            var dailyQuestions = _dailiesRepository.GetDailyQuestions().Where(dq => dq.Date <= TimezoneHelper.Now()).ToList();

            if (dailyQuestions.Count == 0)
            {
                return Ok(new List<DailyQuestionPublic>());
            }

            var dailyQuestionsPublic = dailyQuestions.Select(dq => new DailyQuestionPublic
            {
                Date = dq.Date,
                Style = dq.Style,
                Theme = dq.Theme,
                ImageUrl = dq.ImageUrl,
            }).ToList();
            return Ok(dailyQuestionsPublic);
        }


        [HttpGet("future")]
        [Authorize(Policy = "AdminOnly")]
        public ActionResult<DailyQuestionEntity> GetFutureDailyQuestions()
        {
            var dailyQuestions = _dailiesRepository.GetDailyQuestions().Where(dq => dq.Date >= TimezoneHelper.Now().Date).ToList();

            if (dailyQuestions.Count == 0)
            {
                return Ok(new List<DailyQuestionEntity>());
            }

            return Ok(dailyQuestions);
        }


        [HttpPost]
        public async Task<ActionResult<DailyQuestionPublic>> CreateDailyQuestion([FromBody] DateTime date)
        {

            var randomTheme = _dailiesRepository.GetDailyThemes().OrderBy(_ => Guid.NewGuid()).First();
            date = date.Date;

            var question = await _dailyAIService.GenerateGameQuestionAsync(randomTheme.Theme, date);
            var daily = new DailyQuestionEntity
            {
                Date = date,
                Style = randomTheme.Style,
                Theme = randomTheme.Theme,
                ImageUrl = question.ImageUrl,
                OriginalPrompt = question.OriginalPrompt
            };
            await _dailiesRepository.SaveDailyQuestion(daily);
            return Ok(daily);
        }
    }
}
