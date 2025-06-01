using DrawPT.Common.Models;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DrawPT.Api.Hubs
{
    public interface IGameClient
    {
        Task PlayerJoined(Player player);
        Task PlayerLeft(Player player);
        Task PlayerScoreUpdated(string playerId, int newScore);
        Task GameStarted(GameConfiguration configuration);
        Task GameEnded(GameResults results);
        Task RoundStarted(GameRound round);
        Task RoundEnded(GameRound round);
        Task SuccessfullyJoined(string connectionId);
        Task WriteMessage(string message);
    }

    public class GameHub : Hub<IGameClient>
    {
        private readonly ILogger<GameHub> _logger;
        private readonly IModel _channel;
        private readonly IHubContext<GameHub, IGameClient> _hubContext;

        public GameHub(
            ILogger<GameHub> logger,
            IConnection rabbitMqConnection,
            IHubContext<GameHub, IGameClient> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;

            // Set up RabbitMQ channel
            _channel = rabbitMqConnection.CreateModel();
            _channel.ExchangeDeclare("game_events", ExchangeType.Topic, true);

            // Set up consumer
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                try
                {
                    // Extract room code from routing key (format: game.{roomCode}.event)
                    var parts = routingKey.Split('.');
                    if (parts.Length >= 2)
                    {
                        var roomCode = parts[1];
                        await HandleGameEvent(message, roomCode);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling game event: {Message}", message);
                }
            };

            // Start consuming from all game events
            _channel.QueueDeclare("game_events", true, false, false);
            _channel.QueueBind("game_events", "game_events", "game.#");
            _channel.BasicConsume(queue: "game_events",
                                autoAck: true,
                                consumer: consumer);
        }

        private async Task HandleGameEvent(string message, string roomCode)
        {
            var eventData = JsonSerializer.Deserialize<JsonElement>(message);
            var eventType = eventData.GetProperty("EventType").GetString();

            switch (eventType)
            {
                case "PlayerJoined":
                    var player = JsonSerializer.Deserialize<Player>(eventData.GetProperty("Player").GetRawText());
                    await _hubContext.Clients.Group(roomCode).PlayerJoined(player!);
                    break;

                case "PlayerLeft":
                    var leftPlayer = JsonSerializer.Deserialize<Player>(eventData.GetProperty("Player").GetRawText());
                    await _hubContext.Clients.Group(roomCode).PlayerLeft(leftPlayer!);
                    break;

                case "PlayerScoreUpdated":
                    var playerId = eventData.GetProperty("PlayerId").GetString();
                    var newScore = eventData.GetProperty("NewScore").GetInt32();
                    await _hubContext.Clients.Group(roomCode).PlayerScoreUpdated(playerId!, newScore);
                    break;

                case "GameStarted":
                    var config = JsonSerializer.Deserialize<GameConfiguration>(eventData.GetProperty("Configuration").GetRawText());
                    await _hubContext.Clients.Group(roomCode).GameStarted(config!);
                    break;

                case "GameEnded":
                    var results = JsonSerializer.Deserialize<GameResults>(eventData.GetProperty("Results").GetRawText());
                    await _hubContext.Clients.Group(roomCode).GameEnded(results!);
                    break;

                case "RoundStarted":
                    var round = JsonSerializer.Deserialize<GameRound>(eventData.GetProperty("Round").GetRawText());
                    await _hubContext.Clients.Group(roomCode).RoundStarted(round!);
                    break;

                case "RoundEnded":
                    var endedRound = JsonSerializer.Deserialize<GameRound>(eventData.GetProperty("Round").GetRawText());
                    await _hubContext.Clients.Group(roomCode).RoundEnded(endedRound!);
                    break;
            }
        }

        public async Task JoinGame(string roomCode, Guid playerId)
        {
            // Add player to SignalR group
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            // Publish join request to RabbitMQ
            var player = new Player()
            {
                ConnectionId = Context.ConnectionId,
                Id = playerId.ToString()
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(player));
            _channel.BasicPublish(
                exchange: "game_events",
                routingKey: $"game.{roomCode}.player.join_request",
                basicProperties: null,
                body: body);

            await Clients.Caller.SuccessfullyJoined(Context.ConnectionId);
        }

        public async Task LeaveGame(string roomCode)
        {
            // Remove player from SignalR group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);

            // Publish leave request to RabbitMQ
            var message = new
            {
                EventType = "PlayerLeaveRequest",
                RoomCode = roomCode,
                ConnectionId = Context.ConnectionId
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublish(
                exchange: "game_events",
                routingKey: $"game.{roomCode}.player.leave_request",
                basicProperties: null,
                body: body);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Note: You'll need to track which room the player is in to properly handle disconnection
            await base.OnDisconnectedAsync(exception);
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}