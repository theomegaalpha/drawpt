using DrawPT.Data.Repositories;
using DrawPT.Common.Models.Daily;
using Microsoft.AspNetCore.Mvc;

namespace DrawPT.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DailyQuestionController : ControllerBase
    {
        private readonly DailiesRepository _dailiesRepository;

        public DailyQuestionController(DailiesRepository dailiesRepository)
        {
            _dailiesRepository = dailiesRepository ?? throw new ArgumentNullException(nameof(dailiesRepository), "DailiesRepository cannot be null.");
        }

        /// <summary>
        /// Gets the daily prompt for the current date.
        /// </summary>
        [HttpGet]
        public ActionResult<DailyQuestionPublic> GetDailyQuestionPrompt()
        {
            var todaysQuestion = _dailiesRepository.GetDailyQuestion(DateTime.Now.Date);
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
    }
}
