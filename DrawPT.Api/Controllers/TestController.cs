using DrawPT.Common.Services.AI;
using DrawPT.Common.Util;

using Microsoft.AspNetCore.Mvc;

namespace DrawPT.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : Controller
    {
        private readonly DailyAIService _dailyAIService;

        public TestController(DailyAIService dailyAIService)
        {
            _dailyAIService = dailyAIService;
        }


        // GET: api/<RoomController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(TimezoneHelper.Now());
        }
    }
}
