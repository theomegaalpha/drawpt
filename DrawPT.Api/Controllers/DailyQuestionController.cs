using DrawPT.Data.Repositories;
using DrawPT.Data.Repositories.Game;
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
        public ActionResult<DailyQuestionEntity> GetDailyQuestionPrompt()
        {
            var todaysQuestion = _dailiesRepository.GetDailyQuestion(DateTime.Now.Date);
            if (todaysQuestion != null)
            {
                return Ok(todaysQuestion);
            }

            return NotFound("No daily question found for today.");
        }
    }
}
