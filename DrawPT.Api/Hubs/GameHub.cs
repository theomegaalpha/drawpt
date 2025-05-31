using DrawPT.GameEngine.Models;
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
        private readonly string _gameId = (new Guid()).ToString();

        public GameHub(
            ILogger<GameHub> logger,
            IConnection rabbitMqConnection,
            string gameId)
        {
            _logger = logger;
            _gameId = gameId;

            // Set up RabbitMQ channel
            _channel = rabbitMqConnection.CreateModel();
            _channel.ExchangeDeclare("game_events", ExchangeType.Topic, true);

            // Declare queue for this game
            _channel.QueueDeclare($"game_events_{gameId}", true, false, false);
            _channel.QueueBind($"game_events_{gameId}", "game_events", $"game.{gameId}.*");

            // Set up consumer
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                try
                {
                    await HandleGameEvent(message, routingKey);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling game event: {Message}", message);
                }
            };

            _channel.BasicConsume(queue: $"game_events_{gameId}",
                                autoAck: true,
                                consumer: consumer);
        }

        private async Task HandleGameEvent(string message, string routingKey)
        {
            var eventData = JsonSerializer.Deserialize<JsonElement>(message);
            var eventType = eventData.GetProperty("EventType").GetString();

            switch (eventType)
            {
                case "PlayerJoined":
                    var player = JsonSerializer.Deserialize<Player>(eventData.GetProperty("Player").GetRawText());
                    await Clients.Group(_gameId).PlayerJoined(player!);
                    break;

                case "PlayerLeft":
                    var leftPlayer = JsonSerializer.Deserialize<Player>(eventData.GetProperty("Player").GetRawText());
                    await Clients.Group(_gameId).PlayerLeft(leftPlayer!);
                    break;

                case "PlayerScoreUpdated":
                    var playerId = eventData.GetProperty("PlayerId").GetString();
                    var newScore = eventData.GetProperty("NewScore").GetInt32();
                    await Clients.Group(_gameId).PlayerScoreUpdated(playerId!, newScore);
                    break;

                case "GameStarted":
                    var config = JsonSerializer.Deserialize<GameConfiguration>(eventData.GetProperty("Configuration").GetRawText());
                    await Clients.Group(_gameId).GameStarted(config!);
                    break;

                case "GameEnded":
                    var results = JsonSerializer.Deserialize<GameResults>(eventData.GetProperty("Results").GetRawText());
                    await Clients.Group(_gameId).GameEnded(results!);
                    break;

                case "RoundStarted":
                    var round = JsonSerializer.Deserialize<GameRound>(eventData.GetProperty("Round").GetRawText());
                    await Clients.Group(_gameId).RoundStarted(round!);
                    break;

                case "RoundEnded":
                    var endedRound = JsonSerializer.Deserialize<GameRound>(eventData.GetProperty("Round").GetRawText());
                    await Clients.Group(_gameId).RoundEnded(endedRound!);
                    break;
            }
        }

        public async Task JoinGame(Player player)
        {
            // Add player to SignalR group
            await Groups.AddToGroupAsync(Context.ConnectionId, _gameId);

            // Publish join request to RabbitMQ
            var message = new
            {
                EventType = "PlayerJoinRequest",
                GameId = _gameId,
                ConnectionId = Context.ConnectionId,
                Player = player
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublish(
                exchange: "game_events",
                routingKey: $"game.{_gameId}.player.join_request",
                basicProperties: null,
                body: body);

            await Clients.Caller.SuccessfullyJoined(Context.ConnectionId);
        }

        public async Task LeaveGame()
        {
            // Remove player from SignalR group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, _gameId);

            // Publish leave request to RabbitMQ
            var message = new
            {
                EventType = "PlayerLeaveRequest",
                GameId = _gameId,
                ConnectionId = Context.ConnectionId
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublish(
                exchange: "game_events",
                routingKey: $"game.{_gameId}.player.leave_request",
                basicProperties: null,
                body: body);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await LeaveGame();
            await base.OnDisconnectedAsync(exception);
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}