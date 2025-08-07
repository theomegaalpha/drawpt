using System.Text.Json;
using Azure.Messaging.ServiceBus;
using DrawPT.Common.Interfaces;
using DrawPT.Common.Interfaces.Game;
using DrawPT.Common.Models.Game;
using DrawPT.Common.ServiceBus;
using DrawPT.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DrawPT.Api.Hubs
{
    [Authorize]
    public partial class GameHub : Hub<IGameClient>
    {
        protected readonly ILogger<GameHub> _logger;
        protected readonly IHubContext<GameHub, IGameClient> _hubContext;
        protected readonly ICacheService _cache;
        protected readonly ServiceBusClient _serviceBusClient;
        protected readonly TtsService _ttsService;

        public GameHub(
            ILogger<GameHub> logger,
            ICacheService cacheService,
            ServiceBusClient serviceBusClient,
            TtsService ttsService,
            IHubContext<GameHub, IGameClient> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
            _cache = cacheService;
            _serviceBusClient = serviceBusClient;
            _ttsService = ttsService;
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

            var parsedId = Guid.Parse(userId);

            // Check if player already exists in the room
            var playerInRoom = players.FirstOrDefault(p => p.Id == parsedId);
            if (playerInRoom != null)
            {
                _logger.LogInformation($"Player {parsedId} already exists in room {roomCode}!");
                await Clients.Caller.AlreadyInRoom();
                return;
            }

            var player = await _cache.GetPlayerAsync(parsedId);
            if (player == null)
                player = await _cache.CreatePlayerAsync(parsedId);
            player.RoomCode = roomCode;
            player.ConnectionId = Context.ConnectionId;
            await _cache.UpdatePlayerAsync(player);

            await _cache.SetGameState(gameState);
            await _cache.AddPlayerToRoom(roomCode, player);

            // Notify the client that they can join the room
            await Clients.Caller.NavigateToRoom();
        }

        public async Task TestAudio(string text)
        {
            await _ttsService.GenerateAudio(text, Clients.Caller);
        }

        public async Task JoinGame(string roomCode, string username)
        {
            // check if user is allowed to be in the room
            var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            if (userId == null)
            {
                _logger.LogError("user_id not found in claim while joining a room!");
                await Clients.Caller.ErrorJoiningRoom("Failed to join room.  Please refresh and try again.");
                return;
            }

            var playerId = Guid.Parse(userId);
            var player = await _cache.GetPlayerAsync(playerId);
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
            player.ConnectionId = Context.ConnectionId;
            await _cache.UpdatePlayerAsync(player);
            await _cache.AddPlayerToRoom(roomCode, player);

            var gameState = await _cache.GetGameState(roomCode);
            if (gameState == null)
            {
                _logger.LogError($"Game state for room {roomCode} not found!");
                await Clients.Caller.ErrorJoiningRoom("Failed to join room.  Please refresh and try again.");
                return;
            }
            gameState.Players = players;
            await _cache.SetGameState(gameState);

            await Clients.Caller.InitRoomPlayers(players);
            await Clients.Caller.SuccessfullyJoined(Context.ConnectionId, gameState);
            await Clients.GroupExcept(player.RoomCode, Context.ConnectionId).PlayerJoined(player);
        }

        public async Task<bool> StartGame(IGameConfiguration gameConfiguration)
        {
            var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            if (userId == null)
            {
                _logger.LogWarning("user_id not found in claim while starting a game!");
                return false;
            }

            var player = await _cache.GetPlayerAsync(Guid.Parse(userId));
            if (player == null)
            {
                _logger.LogWarning("The user that started the game can not be found in cache!");
                return false;
            }
            _logger.LogInformation($"Player {userId} joined room {player.RoomCode} with connection {Context.ConnectionId}");


            var gameState = await _cache.GetGameState(player.RoomCode);
            if (gameState == null)
                return false;

            gameState.GameConfiguration = gameConfiguration;
            await _cache.SetGameState(gameState);
            var message = JsonSerializer.Serialize(gameState);

            // Send start game message to Azure Service Bus queue 'apiGlobal'
            var serviceBusSender = _serviceBusClient.CreateSender(ApiGlobalQueue.Name);
            var serviceBusMessage = new ServiceBusMessage(message);
            await serviceBusSender.SendMessageAsync(serviceBusMessage);

            return true;
        }

        public async Task TriggerNavigateBackToLobby()
        {
            var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            if (userId == null)
            {
                _logger.LogWarning("user_id not found in claim while starting a game!");
                return;
            }

            var player = await _cache.GetPlayerAsync(Guid.Parse(userId));
            if (player == null)
            {
                _logger.LogWarning("The user that started the game can not be found in cache!");
                return;
            }
            _logger.LogInformation($"Player {userId} joined room {player.RoomCode} with connection {Context.ConnectionId}");


            await _hubContext.Clients.Group(player.RoomCode).NavigateBackToLobby();
        }

        public async Task GameConfigurationChanged(IGameConfiguration configuration)
        {
            var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            if (userId == null)
            {
                _logger.LogWarning("user_id not found in claim while changing game configuration!");
                return;
            }
            var player = await _cache.GetPlayerAsync(Guid.Parse(userId));
            if (player == null)
            {
                _logger.LogWarning("The user that changed the game configuration can not be found in cache!");
                return;
            }
            _logger.LogInformation($"Player {userId} changed game configuration in room {player.RoomCode} with connection {Context.ConnectionId}");
            var gameState = await _cache.GetGameState(player.RoomCode);
            if (gameState == null)
            {
                _logger.LogWarning($"Game state for room {player.RoomCode} not found!");
                return;
            }
            gameState.GameConfiguration = configuration;
            await _cache.SetGameState(gameState);
            await _hubContext.Clients.Group(player.RoomCode).BroadcastGameConfigurationChange(configuration);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;

            if (userId == null)
            {
                _logger.LogWarning("user_id not found in claim while disconnecting!");
                return;
            }

            var player = await _cache.GetPlayerAsync(Guid.Parse(userId));

            if (player == null)
            {
                _logger.LogWarning($"Player session not found for connection {Context.ConnectionId}");
                await base.OnDisconnectedAsync(exception);
                return;
            }

            // Handle dupe connections  (e.g. if a player reconnects with a different connection ID)
            if (player.ConnectionId != Context.ConnectionId)
            {
                _logger.LogInformation($"Ignoring disconnect for stale connection {Context.ConnectionId} (current stored: {player.ConnectionId})");
                await base.OnDisconnectedAsync(exception);
                return;
            }

            await _hubContext.Clients.GroupExcept(player.RoomCode, player.ConnectionId).PlayerLeft(player);
            await _cache.RemovePlayerFromRoom(player.RoomCode, player);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
