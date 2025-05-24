using DrawPT.Api.Cache;
using DrawPT.Data.Repositories.Reference;
namespace DrawPT.Api.Services
{
    public class RandomService
    {
        private readonly ReferenceCache _referenceCache;
        public RandomService(ReferenceCache referenceCache)
        {
            _referenceCache = referenceCache;
        }

        public string GenerateRoomCode()
        {
            Random random = new();
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string GenerateRandomUsername()
        {
            return $"{GetRandomAdjective()} {GetRandomNoun()}";
        }

        public string GetRandomAdjective()
        {
            Random random = new();
            return _referenceCache.Adjectives[random.Next(_referenceCache.Adjectives.Count)];
        }

        public string GetRandomNoun()
        {
            Random random = new();
            return _referenceCache.Nouns[random.Next(_referenceCache.Nouns.Count)];
        }

        public List<ThemeEntity> GetRandomThemes(int count = 5)
        {
            List<ThemeEntity> themes = new();
            Random random = new();
            var themeKeys = _referenceCache.Themes.Keys.ToList();
            for (int i = 0; i < count; i++)
            {
                int index = random.Next(themeKeys.Count);
                Guid key = themeKeys[index];
                ThemeEntity theme = new() {
                    Id = key,
                    Name = _referenceCache.Themes[key]
                };
                themes.Add(theme);
                themeKeys.RemoveAt(index);
            }

            return themes;
        }
    }
}
