using DrawPT.Common.Models.Supabase;
using Supabase;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DrawPT.Common.Services
{
    public class ProfileService
    {
        private readonly Client _supabase;

        public ProfileService(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<Profile?> GetProfileAsync(Guid userId)
        {
            var allProfiles = await _supabase.From<Profile>()
                .Get();

            return await _supabase.From<Profile>()
                .Where(p => p.Id == userId)
                .Single();
        }

        public async Task UpdateUsernameAsync(Guid userId, string username)
        {
            await _supabase.From<Profile>()
              .Where(x => x.Id == userId)
              .Set(x => x.Username, username)
              .Update();
        }
    }
}
