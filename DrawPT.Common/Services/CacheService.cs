using DrawPT.Common.Interfaces;
using DrawPT.Common.Interfaces.Game;
using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DrawPT.Common.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _options;
        private readonly PlayerService _profileService;

        public CacheService(IDistributedCache cache, PlayerService profileService)
        {
            _cache = cache;
            _options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(2));
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService), "PlayerService cannot be null.");
        }

        public async Task<Room?> GetRoomAsync(string code)
        {
            var room = await _cache.GetStringAsync("room:" + code.ToString());
            if (room is null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<Room>(room);
        }

        public async Task<Room> CreateRoomAsync()
        {
            var room = new Room();
            while (RoomExistsAsync(room.Code).Result)
            {
                room.RegenerateCode();
            }

            var roomJson = JsonSerializer.Serialize(room);
            await _cache.SetStringAsync($"room:{room.Code}", roomJson, _options);
            return room;
        }

        public async Task<bool> UpdateRoomAsync(Room room)
        {
            if (!RoomExistsAsync(room.Code).Result)
                return false;

            var roomJson = JsonSerializer.Serialize(room);
            await _cache.SetStringAsync($"room:{room.Code}", roomJson, _options);
            return true;
        }

        public void CloseRoom(string roomCode)
        {
            if (!RoomExistsAsync(roomCode).Result)
                return;

            _cache.Remove($"room:{roomCode}");
        }

        public Task<IGameState?> GetGameState(string roomCode)
        {
            var gameStateJson = _cache.GetString($"gameState:{roomCode}");
            if (gameStateJson is null)
            {
                return Task.FromResult<IGameState?>(null);
            }
            IGameState? gameState;
            try
            {
                gameState = JsonSerializer.Deserialize<GameState>(gameStateJson);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Failed to deserialize game state.", ex);
            }
            return Task.FromResult(gameState);
        }

        public async Task SetGameState(IGameState gameState)
        {
            var gameStateJson = JsonSerializer.Serialize(gameState);
            await _cache.SetStringAsync($"gameState:{gameState.RoomCode}", gameStateJson, _options);
        }

        public async Task<bool> RoomExistsAsync(string code)
        {
            var value = await _cache.GetStringAsync("room:" + code.ToString());
            return value != null;
        }

        public async Task<Player?> GetPlayerAsync(Guid id)
        {
            var playerJson = await _cache.GetStringAsync("player:" + id.ToString());
            if (playerJson is null)
                return null;

            var player = JsonSerializer.Deserialize<Player>(playerJson);
            return player;
        }

        public async Task<Player> CreatePlayerAsync(Guid id)
        {
            var player = await _profileService.GetPlayerAsync(id);
            if (player != null)
            {
                return player;
            }

            player = new Player()
            {
                Id = id
            };
            var playerJson = JsonSerializer.Serialize(player);
            await _cache.SetStringAsync("player:" + player.Id.ToString(), playerJson, _options);
            return player;
        }

        public async Task<Player> UpdatePlayerAsync(Player player)
        {
            var playerJson = JsonSerializer.Serialize(player);
            await _cache.SetStringAsync("player:" + player.Id.ToString(), playerJson, _options);
            return player;
        }

        public async Task<List<Player>> GetRoomPlayersAsync(string roomCode)
        {
            var playerIds = await _cache.GetStringAsync($"room:{roomCode}:players");
            if (string.IsNullOrEmpty(playerIds))
            {
                return new List<Player>();
            }
            var parsedIds = JsonSerializer.Deserialize<List<Guid>>(playerIds) ?? [];
            var roomPlayers = new List<Player>();
            foreach (var playerId in parsedIds)
            {
                var player = await GetPlayerAsync(playerId);
                if (player != null)
                    roomPlayers.Add(player);
            }
            return roomPlayers;
        }

        public async Task AddPlayerToRoom(string roomCode, Player player)
        {
            var playerIds = await _cache.GetStringAsync($"room:{roomCode}:players");
            List<Guid> players = string.IsNullOrEmpty(playerIds) ? new () : JsonSerializer.Deserialize<List<Guid>>(playerIds) ?? new ();
            if (players.Any(p => p == player.Id))
                return;

            players.Add(player.Id);
            await UpdatePlayerAsync(player);
            await _cache.SetStringAsync($"room:{roomCode}:players", JsonSerializer.Serialize(players), _options);
        }

        public async Task RemovePlayerFromRoom(string roomCode, Player player)
        {
            var playerIds = await _cache.GetStringAsync($"room:{roomCode}:players");
            List<Guid> parsedIds = string.IsNullOrEmpty(playerIds) ? new() : JsonSerializer.Deserialize<List<Guid>>(playerIds) ?? new();
            if (!parsedIds.Any(pid => pid == player.Id))
                return; // Player already doesn't exist in the room

            parsedIds.RemoveAll(pid => pid == player.Id);

            if (parsedIds.Count == 0)
            {
                // If no players left, remove the room
                await _cache.RemoveAsync($"room:{roomCode}:players");
                await _cache.RemoveAsync($"room:{roomCode}");
                return;
            }
            await _cache.SetStringAsync($"room:{roomCode}:players", JsonSerializer.Serialize(parsedIds), _options);
        }
    }
}
