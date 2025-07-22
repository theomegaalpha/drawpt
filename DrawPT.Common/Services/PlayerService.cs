using DrawPT.Common.Models;
using DrawPT.Common.Models.Supabase;
using Supabase;

namespace DrawPT.Common.Services
{
    public class PlayerService
    {
        private readonly Client _supabase;

        public PlayerService(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<Player?> GetPlayerAsync(Guid userId)
        {
            var profile = await _supabase.From<Profile>()
                .Where(p => p.Id == userId)
                .Single();

            if (profile == null)
                return null;

            var player = new Player() { Id = userId, Username = profile.Username, Avatar = profile.Avatar };
            return player;
        }

        public async Task UpdatePlayerAsync(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player), "Player cannot be null.");

            var existingProfiles = await _supabase.From<Profile>()
                .Where(x => x.Id != player.Id && x.Username == player.Username)
                .Get();

            // only do a prelim check for one conflict
            // TODO: handle nested conflicts if needed
            var count = existingProfiles.Models.Count;
            player.Username = count > 0 ? $"{player.Username} {count}" : player.Username;

            await _supabase.From<Profile>()
              .Where(x => x.Id == player.Id)
              .Set(x => x.Username, player.Username)
              .Set(x => x.Avatar, player.Avatar)
              .Update();
        }
    }
}
