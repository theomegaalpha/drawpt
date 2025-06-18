using DrawPT.Common.Interfaces;
using DrawPT.Common.Models;
using DrawPT.Common.Models.Supabase;

using Supabase;

namespace DrawPT.Common.Services
{
    public class PlayerService
    {
        private readonly Client _supabase;
        private readonly ICacheService _cacheService;

        public PlayerService(Client supabase, ICacheService cacheService)
        {
            _supabase = supabase;
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService), "CacheService cannot be null.");
        }

        public async Task<Player?> GetPlayerAsync(Guid userId)
        {
            var profile = await _supabase.From<Profile>()
                .Where(p => p.Id == userId)
                .Single();

            if (profile == null)
                return null;

            var player = new Player() { Id = userId, Username = profile.Username, Avatar = profile.Avatar };
            await _cacheService.UpdatePlayerAsync(player);
            return player;
        }

        public async Task UpdatePlayerAsync(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player), "Player cannot be null.");

            await _supabase.From<Profile>()
              .Where(x => x.Id == player.Id)
              .Set(x => x.Username, player.Username)
              .Set(x => x.Avatar, player.Avatar)
              .Update();

            await _cacheService.UpdatePlayerAsync(player);
        }
    }
}
