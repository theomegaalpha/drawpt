using DrawPT.Common.Models;
using DrawPT.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DrawPT.Common.Services;

namespace DrawPT.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly PlayerService _profileService;

        public PlayerController(ICacheService cacheService, PlayerService profileService)
        {
            _cacheService = cacheService;
            _profileService = profileService;
        }

        // GET: api/<PlayerController>
        [HttpGet]
        public async Task<ActionResult<Player>> Get()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            if (userId == null)
                return BadRequest("User ID claim is not present.");

            return Ok(await _profileService.GetPlayerAsync(Guid.Parse(userId)));
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
        [HttpPost]
        public async Task<ActionResult<Player>> Post([FromBody] Player player)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            if (userId == null)
                return BadRequest("User ID claim is not present.");

            if (userId != player.Id.ToString())
                return Unauthorized("You can only edit your own profile.");

            await _profileService.UpdatePlayerAsync(player);
            await _cacheService.UpdatePlayerAsync(player);
            return Ok(player);
        }
    }
}
