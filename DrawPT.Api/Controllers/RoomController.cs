using DrawPT.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrawPT.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly CacheService _cacheService;

        public RoomController(CacheService cacheService)
        {
            _cacheService = cacheService;
        }

        // GET: api/<RoomController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var room = await _cacheService.CreateRoomAsync();
            return Ok(room.Code);
        }

        // GET: api/<RoomController>/roomCode
        [HttpGet("{roomCode}")]
        public async Task<IActionResult> Get(string roomCode)
        {
            var roomExists = await _cacheService.GetRoomAsync(roomCode);
            return Ok(roomExists);
        }
    }
}
