using DrawPT.Common.Interfaces;
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
        private readonly ICacheService _cache;

        public GameHub(
            ILogger<GameHub> logger,
            ICacheService cacheService,
            IConnection rabbitMqConnection,
            IHubContext<GameHub, IGameClient> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
            _cache = cacheService;

            // Set up RabbitMQ channel
            _channel = rabbitMqConnection.CreateModel();
            _channel.ExchangeDeclare("client_broadcast", ExchangeType.Topic, true);

            // Set up consumer
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                _logger.LogInformation("Received message with routing key: {RoutingKey}", routingKey);

                try
                {
                    // Extract room code from routing key (format: game.{roomCode}.{type}.{action})
                    var parts = routingKey.Split('.');
                    var roomCode = parts[1];
                    var type = parts[2];
                    var action = parts[3];
                    await HandleGameEvent(roomCode, action, message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling game event: {Message}", message);
                }
            };

            // Start consuming from all game events
            _channel.QueueDeclare("client_broadcast", true, false, false);
            // Bind to catch ALL game events with any number of segments
            _channel.QueueBind("client_broadcast", "client_broadcast", "game.#");
            _channel.BasicConsume(queue: "client_broadcast",
                                autoAck: true,
                                consumer: consumer);

            _logger.LogInformation("Started consuming from client_broadcast queue");
        }

        private async Task HandleGameEvent(string roomCode, string action, string message)
        {
            switch (action)
            {
                case "join_request":
                    var player = JsonSerializer.Deserialize<Player>(message);
                    await _hubContext.Clients.Group(roomCode).PlayerJoined(player!);
                    break;

                case "leave_request":
                    var leftPlayer = JsonSerializer.Deserialize<Player>(message);
                    await _hubContext.Clients.Group(roomCode).PlayerLeft(leftPlayer!);
                    break;

                case "PlayerScoreUpdated":
                    var playerResult = JsonSerializer.Deserialize<PlayerResult>(message);
                    await _hubContext.Clients.Group(roomCode).PlayerScoreUpdated(playerResult.Id, playerResult.Score);
                    break;

                case "GameStarted":
                    var config = JsonSerializer.Deserialize<GameConfiguration>(message);
                    await _hubContext.Clients.Group(roomCode).GameStarted(config!);
                    break;

                case "GameEnded":
                    var results = JsonSerializer.Deserialize<GameResults>(message);
                    await _hubContext.Clients.Group(roomCode).GameEnded(results!);
                    break;

                case "RoundStarted":
                    var round = JsonSerializer.Deserialize<GameRound>(message);
                    await _hubContext.Clients.Group(roomCode).RoundStarted(round!);
                    break;

                case "RoundEnded":
                    var endedRound = JsonSerializer.Deserialize<GameRound>(message);
                    await _hubContext.Clients.Group(roomCode).RoundEnded(endedRound!);
                    break;
            }
        }

        public async Task JoinGame(string roomCode, Guid playerId)
        {
            // Add player to SignalR group
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            var player = await _cache.GetPlayerAsync(playerId);
            if (player == null)
                return;
            player.RoomCode = roomCode;
            await _cache.SetPlayerSessionAsync(Context.ConnectionId, player);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(player));
            var routingKey = $"game.{roomCode}.player.join_request";
            _logger.LogInformation("Publishing message with routing key: {RoutingKey}", routingKey);

            _channel.BasicPublish(
                exchange: "client_broadcast",
                routingKey: routingKey,
                basicProperties: null,
                body: body);

            await Clients.Caller.SuccessfullyJoined(Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var player = await _cache.GetPlayerSessionAsync(Context.ConnectionId);

            if (player == null)
            {
                _logger.LogWarning("Player session not found for connection {ConnectionId}", Context.ConnectionId);
                return;
            }

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(player));
            _channel.BasicPublish(
                exchange: "client_broadcast",
                routingKey: $"game.{player.RoomCode}.player.leave_request",
                basicProperties: null,
                body: body);

            await base.OnDisconnectedAsync(exception);
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}