using DrawPT.Common.Configuration;
using DrawPT.Common.Interfaces;
using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;
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

            var interactionConsumer = new EventingBasicConsumer(_channel);
            interactionConsumer.Received += async (model, ea) =>
            {
                var corrId = ea.BasicProperties.CorrelationId;
                var replyTo = ea.BasicProperties.ReplyTo;
                string response = string.Empty;

                byte[] body = ea.Body.ToArray();
                var routingKey = ea.RoutingKey;
                var connectionId = routingKey.Split('.')[1];
                var action = routingKey.Split('.')[2];
                IBasicProperties props = ea.BasicProperties;
                props.CorrelationId = corrId;

                var message = Encoding.UTF8.GetString(body);

                if (action == ClientInteractionMQ.Theme)
                {
                    CancellationTokenSource themeTimoutTokenSource = new();
                    themeTimoutTokenSource.CancelAfter(TimeSpan.FromSeconds(30 + 5));
                    _logger.LogInformation($"Asking theme for connection {connectionId} with message: {message}");
                    var client = _hubContext.Clients.Client(connectionId);
                    if (client == null)
                    {
                        _logger.LogError($"Client with connection ID {connectionId} not found in {action}!");
                        return;
                    }
                    try
                    {
                        var themes = JsonSerializer.Deserialize<List<string>>(message);
                        response = await client.AskTheme(themes ?? [], themeTimoutTokenSource.Token);
                    }
                    catch
                    {
                        _logger.LogError($"Error while asking theme for connection {connectionId} with message: {message}");
                        return;
                    }
                    var body2 = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                    _channel.BasicPublish(GameResponseMQ.ExchangeName, replyTo, basicProperties: props, body: body2);
                }
                else if (action == ClientInteractionMQ.Question)
                {
                    CancellationTokenSource questionTimeoutSource = new();
                    questionTimeoutSource.CancelAfter(TimeSpan.FromSeconds(30 + 5));
                    var client = _hubContext.Clients.Client(connectionId);
                    if (client == null)
                    {
                        _logger.LogError($"Client with connection ID {connectionId} not found in {action}!");
                        return;
                    }
                    try
                    {
                        response = await client.AskQuestion(new GameQuestion(), questionTimeoutSource.Token);
                    }
                    catch
                    {
                        _logger.LogError($"Error while asking question for connection {connectionId} with message: {message}");
                        return;
                    }
                    var body2 = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                    _channel.BasicPublish(GameResponseMQ.ExchangeName, replyTo, basicProperties: props, body: body2);
                }
                else
                {
                    _logger.LogWarning($"Unknown action '{action}' for connection {connectionId}");
                    return;
                }
            };

            _channel.QueueDeclare(ClientInteractionMQ.QueueName);
            // Bind to catch ALL client interaction messages with any number of segments
            _channel.QueueBind(ClientInteractionMQ.QueueName,
                ClientInteractionMQ.ExchangeName, ClientInteractionMQ.RoutingKey);
            _channel.BasicConsume(queue: ClientInteractionMQ.QueueName,
                                autoAck: true,
                                consumer: interactionConsumer);
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
                    var playerResults = JsonSerializer.Deserialize<PlayerResults>(message);
                    if (playerResults == null)
                    {
                        _logger.LogError($"PlayerResults deserialization failed for message: {message}");
                        return;
                    }
                    await _hubContext.Clients.Group(roomCode).PlayerScoreUpdated(playerResults.PlayerId, playerResults.Score);
                    break;

                case ClientBroadcastMQ.StartedGame:
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
            _logger.LogInformation($"Player {playerId} joined room {roomCode} with connection {Context.ConnectionId}");

            var player = await _cache.GetPlayerAsync(playerId);
            if (player == null)
                return;
            player.RoomCode = roomCode;
            player.ConnectionId = Context.ConnectionId;
            await _cache.SetPlayerSessionAsync(Context.ConnectionId, player);
            await _cache.AddPlayerToRoom(roomCode, player);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(player));

            _channel.BasicPublish(
                exchange: ClientBroadcastMQ.ExchangeName,
                routingKey: ClientBroadcastMQ.RoutingKeys.PlayerJoined(roomCode),
                basicProperties: null,
                body: body);

            await Clients.Caller.SuccessfullyJoined(Context.ConnectionId);
        }

        public async Task<bool> StartGame()
        {
            var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            if (userId == null)
            {
                _logger.LogWarning("user_id not found in claim while starting a game!");
                return false;
            }

            var player = await _cache.GetPlayerSessionAsync(Context.ConnectionId);
            if (player == null)
            {
                _logger.LogWarning("The user that started the game can not be found in cache!");
                return false;
            }
            _logger.LogInformation($"Player {userId} joined room {player.RoomCode} with connection {Context.ConnectionId}");

            var message = JsonSerializer.Serialize(new GameConfiguration());
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: GameMQ.ExchangeName,
                                routingKey: GameMQ.RoutingKeys.GameStarted(player.RoomCode),
                                body: body);

            _channel.BasicPublish(
                exchange: ClientBroadcastMQ.ExchangeName,
                routingKey: ClientBroadcastMQ.RoutingKeys.GameStarted(player.RoomCode),
                body: body);

            return true;
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