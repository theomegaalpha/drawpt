using DrawPT.Api.Services;
using DrawPT.Common.Models;
using DrawPT.Common.Interfaces;
using DrawPT.Common.Models.Supabase;
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
        private readonly RandomService _randomService;
        private readonly ICacheService _cacheService;
        private readonly ProfileService _profileService;
        private readonly IConfiguration _configuration;

        public PlayerController(ICacheService cacheService,
            RandomService randomService, ProfileService profileService,
            IConfiguration configuration)
        {
            _randomService = randomService;
            _cacheService = cacheService;
            _profileService = profileService;
            _configuration = configuration;
        }

        // GET: api/<PlayerController>
        [HttpGet]
        public async Task<ActionResult<Player>> Get()
        {
            var player = await _cacheService.CreatePlayerAsync();
            player.Username = _randomService.GenerateRandomUsername();

            var userId = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            
            if (userId == null)
            {
                return BadRequest("User ID claim is not present.");
            }

            var profile = await _profileService.GetProfileAsync(new Guid(userId));

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
            var userId = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;

            if (userId == null)
            {
                return BadRequest("User ID claim is not present.");
            }

            var updatedPlayer = await _cacheService.UpdatePlayerAsync(player);
            await _profileService.UpdateUsernameAsync(new Guid(userId), player.Username);
            return Ok(updatedPlayer);
        }
    }
}
