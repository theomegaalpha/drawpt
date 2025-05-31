using System.Text.Json;
using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Models;
using StackExchange.Redis;

namespace DrawPT.GameEngine.Infrastructure;

public class RedisGameStateManager : IGameStateManager, IDisposable
{
    private readonly IConnectionMultiplexer _redis;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Dictionary<string, Action<string, object>> _eventHandlers = new();

    public RedisGameStateManager(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<GameState> GetGameStateAsync(string roomCode)
    {
        var db = _redis.GetDatabase();
        var stateJson = await db.StringGetAsync($"game:{roomCode}");
        
        if (stateJson.IsNull)
        {
            return new GameState { RoomCode = roomCode, Status = GameStatus.WaitingForPlayers };
        }

        return JsonSerializer.Deserialize<GameState>(stateJson!, _jsonOptions) ?? 
            new GameState { RoomCode = roomCode, Status = GameStatus.WaitingForPlayers };
    }

    public async Task SaveGameStateAsync(string roomCode, GameState state)
    {
        var db = _redis.GetDatabase();
        state.LastUpdated = DateTime.UtcNow;
        var stateJson = JsonSerializer.Serialize(state, _jsonOptions);
        await db.StringSetAsync($"game:{roomCode}", stateJson);
    }

    public async Task<bool> AddPlayerAsync(string roomCode, Player player)
    {
        var state = await GetGameStateAsync(roomCode);
        
        if (state.Players.Count >= 8) // Max players
            return false;

        state.Players[player.ConnectionId] = new PlayerState
        {
            ConnectionId = player.ConnectionId,
            Player = player,
            LastActive = DateTime.UtcNow
        };

        await SaveGameStateAsync(roomCode, state);
        await PublishGameEventAsync(roomCode, "PlayerJoined", player);
        return true;
    }

    public async Task<bool> RemovePlayerAsync(string roomCode, string connectionId)
    {
        var state = await GetGameStateAsync(roomCode);
        
        if (state.Players.Remove(connectionId))
        {
            await SaveGameStateAsync(roomCode, state);
            await PublishGameEventAsync(roomCode, "PlayerLeft", connectionId);
            return true;
        }

        return false;
    }

    public async Task<bool> UpdatePlayerStateAsync(string roomCode, string connectionId, PlayerState newState)
    {
        var state = await GetGameStateAsync(roomCode);
        
        if (state.Players.ContainsKey(connectionId))
        {
            state.Players[connectionId] = newState;
            await SaveGameStateAsync(roomCode, state);
            return true;
        }

        return false;
    }

    public async Task<bool> AddGameRoundAsync(string roomCode, GameRound round)
    {
        var state = await GetGameStateAsync(roomCode);
        state.GameRounds.Add(round);
        state.CurrentRound = round.RoundNumber;
        await SaveGameStateAsync(roomCode, state);
        await PublishGameEventAsync(roomCode, "RoundAdded", round);
        return true;
    }

    public async Task<bool> UpdateGameStatusAsync(string roomCode, GameStatus status)
    {
        var state = await GetGameStateAsync(roomCode);
        state.Status = status;
        await SaveGameStateAsync(roomCode, state);
        await PublishGameEventAsync(roomCode, "StatusChanged", status);
        return true;
    }

    public async Task<bool> LockGameAsync(string roomCode)
    {
        var db = _redis.GetDatabase();
        return await db.StringSetAsync($"lock:{roomCode}", "1", TimeSpan.FromMinutes(5), When.NotExists);
    }

    public async Task<bool> UnlockGameAsync(string roomCode)
    {
        var db = _redis.GetDatabase();
        return await db.KeyDeleteAsync($"lock:{roomCode}");
    }

    public async Task<bool> IsGameLockedAsync(string roomCode)
    {
        var db = _redis.GetDatabase();
        return await db.KeyExistsAsync($"lock:{roomCode}");
    }

    public async Task PublishGameEventAsync(string roomCode, string eventType, object data)
    {
        var pub = _redis.GetSubscriber();
        var eventData = new
        {
            Type = eventType,
            Data = data,
            Timestamp = DateTime.UtcNow
        };
        
        await pub.PublishAsync($"game:{roomCode}", JsonSerializer.Serialize(eventData, _jsonOptions));
    }

    public async Task SubscribeToGameEventsAsync(string roomCode, Func<string, object, Task> handler)
    {
        var sub = _redis.GetSubscriber();
        var channel = $"game:{roomCode}";
        
        _eventHandlers[channel] = async (_, message) =>
        {
            var eventData = JsonSerializer.Deserialize<dynamic>(message, _jsonOptions);
            if (eventData != null)
            {
                await handler(eventData.Type.ToString(), eventData.Data);
            }
        };

        await sub.SubscribeAsync(channel, (_, message) => _eventHandlers[channel](_, message));
    }

    public void Dispose()
    {
        foreach (var channel in _eventHandlers.Keys)
        {
            _redis.GetSubscriber().Unsubscribe(channel);
        }
    }
} 