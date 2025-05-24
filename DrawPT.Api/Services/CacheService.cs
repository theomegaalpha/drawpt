using DrawPT.Data.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DrawPT.Api.Services
{
    public class CacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<bool> GetRoomAsync(string code)
        {
            var roomExists = await _cache.GetStringAsync("room:" + code.ToString());
            if (roomExists is null)
            {
                return false;
            }

            return JsonSerializer.Deserialize<bool>(roomExists);
        }

        public async Task<Room> CreateRoomAsync()
        {
            var room = new Room();
            while (RoomExistsAsync(room.Code).Result)
            {
                room.RegenerateCode();
            }

            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));
            await _cache.SetStringAsync($"room:{room.Code}", "true", options);
            return room;
        }

        public async Task<bool> UpdateRoomAsync(Room room)
        {
            if (!RoomExistsAsync(room.Code).Result)
                return false;

            var roomJson = JsonSerializer.Serialize(room);
            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));
            await _cache.SetStringAsync($"room:{room.Code}", roomJson, options);
            return true;
        }

        public void CloseRoom(string roomCode)
        {
            if (!RoomExistsAsync(roomCode).Result)
                return;

            _cache.Remove($"room:{roomCode}");
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

        public async Task<Player> CreatePlayerAsync()
        {
            var player = new Player();
            var playerJson = JsonSerializer.Serialize(player);
            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));
            await _cache.SetStringAsync("player:" + player.Id.ToString(), playerJson, options);
            return player;
        }

        public async Task<Player> UpdatePlayerAsync(Player player)
        {
            var playerJson = JsonSerializer.Serialize(player);
            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));
            await _cache.SetStringAsync("player:" + player.Id.ToString(), playerJson, options);
            return player;
        }
    }
}
