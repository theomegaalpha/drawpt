using DrawPT.Api.Services;
using DrawPT.Common.Models;
using DrawPT.Common.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DrawPT.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly RandomService _randomService;
        private readonly CacheService _cacheService;

        public PlayerController(CacheService cacheService, RandomService randomService)
        {
            _randomService = randomService;
            _cacheService = cacheService;
        }

        // GET: api/<PlayerController>
        [HttpGet]
        public async Task<ActionResult<Player>> Get()
        {
            var player = await _cacheService.CreatePlayerAsync();
            player.Username = _randomService.GenerateRandomUsername();
            return Ok(player);
        }

        // GET: api/<PlayerController>/00000000-0000-0000-0000-000000000000
        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> Get(Guid id)
        {
            var player = await _cacheService.GetPlayerAsync(id);
            if (player is null)
            {
                return NotFound();
            }

            return Ok(player);
        }


        // POST: api/<PlayerController>
        [HttpPost()]
        public async Task<ActionResult<Player>> Post([FromBody] Player player)
        {
            var updatedPlayer = await _cacheService.UpdatePlayerAsync(player);
            return Ok(updatedPlayer);
        }
    }
}
