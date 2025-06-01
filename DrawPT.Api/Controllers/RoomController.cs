using DrawPT.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace DrawPT.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly IModel _channel;

        public RoomController(ICacheService cacheService, IConnection rabbitMQConnection)
        {
            _cacheService = cacheService;
            _channel = rabbitMQConnection.CreateModel();
            _channel.ExchangeDeclare("matchmaking_events", ExchangeType.Topic, false);
        }

        // GET: api/<RoomController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var room = await _cacheService.CreateRoomAsync();

            _channel.BasicPublish(exchange: "matchmaking_events",
                routingKey: "room.created",
                basicProperties: null,
                body: System.Text.Encoding.UTF8.GetBytes(room.Code));

            return Ok(room.Code);
        }

        // GET: api/<RoomController>/roomCode
        [HttpGet("{roomCode}")]
        public async Task<IActionResult> Get(string roomCode)
        {
            var roomExists = await _cacheService.GetRoomAsync(roomCode);
            return Ok(roomExists != null);
        }
    }
}
