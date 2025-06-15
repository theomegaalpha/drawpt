using DrawPT.Common.Interfaces;
using DrawPT.Common.Services.AI;

using Microsoft.AspNetCore.Mvc;

namespace DrawPT.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : Controller
    {
        private readonly IAIService _dailyAIService;

        public TestController(DailyAIService dailyAIService)
        {
            _dailyAIService = dailyAIService;
        }


        // GET: api/<RoomController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var imageBytes = await _dailyAIService.GenerateGameQuestionAsync("Nier Automata");


            return Ok();
        }
    }
}
