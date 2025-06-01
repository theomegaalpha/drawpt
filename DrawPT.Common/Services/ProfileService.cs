using DrawPT.Common.Models.Supabase;
using Supabase;

namespace DrawPT.Common.Services
{
    public class ProfileService
    {
        private readonly Client _supabase;

        public ProfileService(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<Profile?> GetProfile(Guid userId)
        {
            return await _supabase.From<Profile>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Single();
        }
    }
}
