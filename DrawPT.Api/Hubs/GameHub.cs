using DrawPT.Common.Configuration;
using DrawPT.Common.Interfaces;
using DrawPT.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DrawPT.Api.Hubs
{
    [Authorize]
    public partial class GameHub : Hub<IGameClient>
    {
        protected readonly ILogger<GameHub> _logger;
        protected readonly IModel _channel;
        protected readonly IHubContext<GameHub, IGameClient> _hubContext;
        protected readonly ICacheService _cache;

        public GameHub(
            ILogger<GameHub> logger,
            ICacheService cacheService,
            IConnection rabbitMqConnection,
            IHubContext<GameHub, IGameClient> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
            _cache = cacheService;

            #region broadcast setup
            // Set up RabbitMQ channel
            _channel = rabbitMqConnection.CreateModel();
            _channel.ExchangeDeclare(ClientBroadcastMQ.ExchangeName, ExchangeType.Topic);

            // Set up consumer
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                _logger.LogInformation($"Received message with routing key: {routingKey}");

                try
                {
                    // Extract room code from routing key (format: client_broadcast.{roomCode}.{action})
                    var parts = routingKey.Split('.');
                    var roomCode = parts[1];
                    var action = parts[2];
                    await HandleClientBroadcast(roomCode, action, message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error handling client broadcast: {message}");
                }
            };

            // Start consuming from all client broadcast messages
            _channel.QueueDeclare(ClientBroadcastMQ.QueueName);
            // Bind to catch ALL client broadcast messages with any number of segments
            _channel.QueueBind(ClientBroadcastMQ.QueueName,
                ClientBroadcastMQ.ExchangeName, ClientBroadcastMQ.RoutingKey);
            _channel.BasicConsume(queue: ClientBroadcastMQ.QueueName,
                                autoAck: true,
                                consumer: consumer);
            #endregion

            #region interaction setup
            _channel.ExchangeDeclare(ClientInteractionMQ.ExchangeName, ExchangeType.Topic);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var interactionConsumer = new AsyncEventingBasicConsumer(_channel);
            interactionConsumer.Received += async (object sender, BasicDeliverEventArgs ea) =>
            {
                var cons = (AsyncEventingBasicConsumer)sender;
                var ch = cons.Model;
                var corrId = ea.BasicProperties.CorrelationId;
                string response = string.Empty;

                byte[] body = ea.Body.ToArray();
                var routingKey = ea.RoutingKey;
                var connectionId = routingKey.Split('.')[1];
                var action = routingKey.Split('.')[2];
                IBasicProperties props = ea.BasicProperties;

                var message = Encoding.UTF8.GetString(body);
                int n = int.Parse(message);

                if (action == ClientInteractionMQ.AskTheme)
                {
                    response = (await _hubContext.Clients.Client(connectionId).AskTheme([""], CancellationToken.None)).ToString();
                    var body2 = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                    _channel.BasicPublish(GameResponseMQ.ExchangeName, GameResponseMQ.RoutingKeys.AnswerTheme, body: body2);
                }
                else if (action == ClientInteractionMQ.AskQuestion)
                {
                    response = (await _hubContext.Clients.Client(connectionId).AskQuestion(new GameQuestion(), CancellationToken.None)).ToString();
                    var body2 = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                    _channel.BasicPublish(GameResponseMQ.ExchangeName, GameResponseMQ.RoutingKeys.AnswerQuestion, body: body2);
                }
                else
                {
                    _logger.LogWarning($"Unknown action '{action}' for connection {connectionId}");
                    return;
                }
            };

            _channel.QueueDeclare(ClientInteractionMQ.QueueName);
            // Bind to catch ALL client broadcast messages with any number of segments
            _channel.QueueBind(ClientInteractionMQ.QueueName,
                ClientInteractionMQ.ExchangeName, ClientInteractionMQ.RoutingKey);
            _channel.BasicConsume(ClientInteractionMQ.QueueName, true, interactionConsumer);
            #endregion

            _logger.LogInformation("Started consuming from client_broadcast queue");
        }

        public void SendResponse(string responsePayload, string correlationId, string replyQueue)
        {
            var properties = _channel.CreateBasicProperties();
            properties.CorrelationId = correlationId; // Match the correlation ID

            var responseBytes = Encoding.UTF8.GetBytes(responsePayload);
            _channel.BasicPublish(exchange: "",
                                  routingKey: replyQueue,
                                  basicProperties: properties,
                                  body: responseBytes);
        }

        private async Task HandleClientBroadcast(string roomCode, string action, string message)
        {
            switch (action)
            {
                case ClientBroadcastMQ.PlayerJoinedAction:
                    Player player = JsonSerializer.Deserialize<Player>(message);
                    await _hubContext.Clients.Group(roomCode).PlayerJoined(player!);
                    break;

                case ClientBroadcastMQ.PlayerLeftAction:
                    var leftPlayer = JsonSerializer.Deserialize<Player>(message);
                    await _hubContext.Clients.Group(roomCode).PlayerLeft(leftPlayer!);
                    break;

                case "PlayerScoreUpdated":
                    var playerResult = JsonSerializer.Deserialize<PlayerResult>(message);
                    await _hubContext.Clients.Group(roomCode).PlayerScoreUpdated(playerResult.Id, playerResult.Score);
                    break;

                case ClientBroadcastMQ.GameStarted:
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
            var routingKey = ClientBroadcastMQ.RoutingKeys.PlayerJoined(roomCode);
            _logger.LogInformation($"Publishing message with routing key: {routingKey}");

            _channel.BasicPublish(
                exchange: ClientBroadcastMQ.ExchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body);

            await Clients.Caller.SuccessfullyJoined(Context.ConnectionId);
        }

        public async Task StartGame()
        {
            var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            if (userId == null)
            {
                _logger.LogWarning("user_id not found in claim while starting a game!");
                return;
            }

            var player = await _cache.GetPlayerSessionAsync(Context.ConnectionId);
            if (player == null)
            {
                _logger.LogWarning("The user that started the game can not be found in cache!");
                return;
            }

            var message = JsonSerializer.Serialize(new { RoomCode = player.RoomCode });
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: GameMQ.ExchangeName,
                                routingKey: GameMQ.RoutingKeys.GameStarted(player.RoomCode),
                                body: body);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var player = await _cache.GetPlayerSessionAsync(Context.ConnectionId);

            if (player == null)
            {
                _logger.LogWarning($"Player session not found for connection {Context.ConnectionId}");
                return;
            }

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(player));
            _channel.BasicPublish(
                exchange: ClientBroadcastMQ.ExchangeName,
                routingKey: ClientBroadcastMQ.RoutingKeys.PlayerLeft(player.RoomCode),
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