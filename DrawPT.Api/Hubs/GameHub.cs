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
        protected readonly EventingBasicConsumer _consumer;
        protected readonly EventingBasicConsumer _interactionConsumer;

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
            _channel.ExchangeDeclare(GameEngineBroadcastMQ.ExchangeName, ExchangeType.Topic);
            _channel.ExchangeDeclare(GameEngineRequestMQ.ExchangeName, ExchangeType.Topic);
            _channel.ExchangeDeclare(ApiMasterMQ.ExchangeName, ExchangeType.Topic);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            // Set up consumer
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += async (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
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
            #endregion

            #region interaction setup

            _interactionConsumer = new EventingBasicConsumer(_channel);
            _interactionConsumer.Received += async (model, ea) =>
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

                if (action == GameEngineRequestMQ.Theme)
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
                        var player = await _cache.GetPlayerSessionAsync(connectionId);
                        if (player == null)
                        {
                            _logger.LogError($"Player with connection ID {connectionId} not found in cache!");
                            return;
                        }
                        await _hubContext.Clients.GroupExcept(player.RoomCode, connectionId).ThemeSelection(themes ?? []);
                        response = await client.AskTheme(themes ?? [], themeTimoutTokenSource.Token);
                    }
                    catch
                    {
                        _logger.LogError($"Error while asking theme for connection {connectionId} with message: {message}");
                        return;
                    }
                    var body2 = Encoding.UTF8.GetBytes(response);
                    _channel.BasicPublish(ApiResponseMQ.ExchangeName, replyTo, basicProperties: props, body: body2);
                }
                else if (action == GameEngineRequestMQ.Question)
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
                        var question = JsonSerializer.Deserialize<GameQuestion>(message);
                        if (question == null)
                            question = new GameQuestion(); //TODO:  default question
                        var playerAnswer = await client.AskQuestion(question, questionTimeoutSource.Token);
                        response = JsonSerializer.Serialize(playerAnswer);
                    }
                    catch
                    {
                        _logger.LogError($"Error while asking question for connection {connectionId} with message: {message}");
                        return;
                    }
                    var body2 = Encoding.UTF8.GetBytes(response);
                    _channel.BasicPublish(ApiResponseMQ.ExchangeName, replyTo, basicProperties: props, body: body2);
                }
                else
                {
                    _logger.LogWarning($"Unknown action '{action}' for connection {connectionId}");
                    return;
                }
            };
            #endregion

            _logger.LogInformation("Started consuming from client_broadcast queue");
        }

        private async Task HandleClientBroadcast(string roomCode, string action, string message)
        {
            switch (action)
            {
                case GameEngineBroadcastMQ.PlayerJoinedAction:
                    var player = JsonSerializer.Deserialize<Player>(message);
                    await _hubContext.Clients.Group(roomCode).PlayerJoined(player!);
                    break;

                case GameEngineBroadcastMQ.PlayerLeftAction:
                    var leftPlayer = JsonSerializer.Deserialize<Player>(message);
                    await _hubContext.Clients.Group(roomCode).PlayerLeft(leftPlayer!);
                    break;

                case GameEngineBroadcastMQ.PlayerScoreUpdateAction:
                    var playerResults = JsonSerializer.Deserialize<PlayerResults>(message);
                    if (playerResults == null)
                    {
                        _logger.LogError($"PlayerResults deserialization failed for message: {message}");
                        return;
                    }
                    await _hubContext.Clients.Group(roomCode).PlayerScoreUpdated(playerResults.PlayerId, playerResults.Score);
                    break;

                case GameEngineBroadcastMQ.PlayerThemeSelectedAction:
                    var theme = JsonSerializer.Deserialize<string>(message);
                    await _hubContext.Clients.Group(roomCode).ThemeSelected(theme ?? "");
                    break;

                case GameEngineBroadcastMQ.GameStartedAction:
                    var config = JsonSerializer.Deserialize<GameConfiguration>(message);
                    await _hubContext.Clients.Group(roomCode).GameStarted(config!);
                    break;

                case GameEngineBroadcastMQ.RoundStartedAction:
                    var round = JsonSerializer.Deserialize<RoundResults>(message);
                    await _hubContext.Clients.Group(roomCode).RoundStarted(round?.RoundNumber ?? 0);
                    break;

                case GameEngineBroadcastMQ.AssessingAnswersAction:
                    await _hubContext.Clients.Group(roomCode).WriteMessage("Assessing player scores...");
                    break;

                case GameEngineBroadcastMQ.RoundResultsAction:
                    var endedRound = JsonSerializer.Deserialize<RoundResults>(message);
                    await _hubContext.Clients.Group(roomCode).RoundResults(endedRound!);
                    break;

                case GameEngineBroadcastMQ.GameResultsAction:
                    var results = JsonSerializer.Deserialize<GameResults>(message);
                    await _hubContext.Clients.Group(roomCode).BroadcastFinalResults(results!);
                    break;
            }
        }

        public async Task RequestToJoinGame(string roomCode)
        {
            var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            if (userId == null)
            {
                _logger.LogWarning("user_id not found in claim while requesting to join a room!");
                await Clients.Caller.ErrorJoiningRoom("Failed to join room.  Please refresh and try again.");
                return;
            }

            var gameState = await _cache.GetGameState(roomCode);
            if (gameState == null)
            {
                gameState = new GameState
                {
                    RoomCode = roomCode,
                    HostPlayerId = new Guid(userId)
                };
            }

            var players = await _cache.GetRoomPlayersAsync(roomCode);
            if (players.Count >= gameState.GameConfiguration.MaxPlayers)
            {
                _logger.LogInformation($"Player {userId} is requesting to join room {roomCode}");
                await Clients.Caller.RoomIsFull();
                return;
            }

            var player = await _cache.GetPlayerAsync(Guid.Parse(userId));
            if (player == null)
            {
                player = await _cache.CreatePlayerAsync();
                player.Id = Guid.Parse(userId);
            }
            player.RoomCode = roomCode;
            player.ConnectionId = Context.ConnectionId;
            await _cache.SetPlayerSessionAsync(Context.ConnectionId, player);
            await _cache.UpdatePlayerAsync(player);

            // Check if player already exists in the room
            var playerInRoom = players.FirstOrDefault(p => p.Id == player.Id);
            if (playerInRoom != null)
            {
                _logger.LogInformation($"Player {player.Id} already exists in room {roomCode}!");
                await Clients.Caller.AlreadyInRoom();
                return;
            }

            await _cache.SetGameState(gameState);
            await _cache.AddPlayerToRoom(roomCode, player);

            // Notify the client that they can join the room
            await Clients.Caller.NavigateToRoom();
        }


        public async Task JoinGame(string roomCode, string username)
        {
            // check if user is allowed to be in the room
            var playerIdClaim = Context.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            if (playerIdClaim == null)
            {
                _logger.LogError("user_id not found in claim while joining a room!");
                await Clients.Caller.ErrorJoiningRoom("Failed to join room.  Please refresh and try again.");
                return;
            }

            var playerId = Guid.Parse(playerIdClaim);
            var player = await _cache.GetPlayerSessionAsync(Context.ConnectionId);
            if (player == null)
                return;

            var players = await _cache.GetRoomPlayersAsync(roomCode);
            if (players.FirstOrDefault(p => p.Id == playerId) == null)
                return;

            // Add player to SignalR group
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            _logger.LogInformation($"Player {playerId} joined room {roomCode} with connection {Context.ConnectionId}");
            
            player.RoomCode = roomCode;
            player.Username = username;
            await _cache.UpdatePlayerAsync(player);

            var gameState = await _cache.GetGameState(roomCode);
            if (gameState == null)
            {
                _logger.LogError($"Game state for room {roomCode} not found!");
                await Clients.Caller.ErrorJoiningRoom("Failed to join room.  Please refresh and try again.");
                return;
            }
            await _cache.SetGameState(gameState);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(player));

            // Start consuming from all client broadcast messages
            if (gameState.HostPlayerId == playerId)
            {
                var broadcastQueueName = _channel.QueueDeclare(queue: "", exclusive: true, autoDelete: true, durable: false).QueueName; // Server-generated name
                _channel.QueueBind(
                    queue: broadcastQueueName,
                    exchange: GameEngineBroadcastMQ.ExchangeName,
                    routingKey: GameEngineBroadcastMQ.RoutingKey(roomCode)
                );
                _channel.BasicConsume(queue: broadcastQueueName,
                                    autoAck: true,
                                    consumer: _consumer);
            }
            // Bind to catch ALL client interaction messages with any number of segments
            _channel.QueueDeclare(GameEngineRequestMQ.QueueName(Context.ConnectionId));
            _channel.QueueBind(GameEngineRequestMQ.QueueName(Context.ConnectionId),
                GameEngineRequestMQ.ExchangeName, GameEngineRequestMQ.RoutingKey(Context.ConnectionId));
            _channel.BasicConsume(queue: GameEngineRequestMQ.QueueName(Context.ConnectionId),
                                autoAck: true,
                                consumer: _interactionConsumer);

            await HandleClientBroadcast(roomCode, GameEngineBroadcastMQ.PlayerJoinedAction, JsonSerializer.Serialize(player));
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
            _channel.BasicPublish(exchange: ApiMasterMQ.ExchangeName,
                                routingKey: ApiMasterMQ.RoutingKeys.GameStarted(player.RoomCode),
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

            await _hubContext.Clients.GroupExcept(player.RoomCode, player.ConnectionId).PlayerLeft(player);
            await _cache.RemovePlayerFromRoom(player.RoomCode, player);
            await _cache.ClearPlayerSessionAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}