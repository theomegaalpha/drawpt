using DrawPT.Data.Repositories;
using DrawPT.Data.Repositories.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DrawPT.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly ILogger<FeedbackController> _logger;
        private readonly MiscRepository _miscRepo;

        private static readonly string[] FeedbackTypes = new[]
        {
            "Bug report",
            "Feature request",
            "General comment"
        };

        public FeedbackController(ILogger<FeedbackController> logger, MiscRepository miscRepo)
        {
            _logger = logger;
            _miscRepo = miscRepo;
        }

        // GET: /Feedback/types
        [HttpGet("types")]
        public ActionResult<IEnumerable<string>> GetTypes()
        {
            return Ok(FeedbackTypes);
        }

        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<FeedbackEntity>> GetFeedback(int page = 1)
        {
            _logger.LogInformation("Fetching all feedback entries.");
            var feedbacks = _miscRepo.GetAllFeedback().Skip((page - 1) * 50).Take(50).ToList();
            return Ok(feedbacks);
        }

        // POST: /Feedback
        [HttpPost]
        public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackEntity entity)
        {
            if (entity == null || string.IsNullOrWhiteSpace(entity.Type) || string.IsNullOrWhiteSpace(entity.Message))
                return BadRequest("Type and message are required.");

            _logger.LogInformation($"Feedback received: [{entity.Type}] {entity.Message}");

            entity.Id = Guid.NewGuid();
            await _miscRepo.SaveFeedbackAsync(entity);

            return Ok(new { success = true });
        }
    }
}
