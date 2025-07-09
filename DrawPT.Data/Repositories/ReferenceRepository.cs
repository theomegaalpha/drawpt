using DrawPT.Data.Repositories.Game;
using DrawPT.Data.Repositories.Reference;
using Microsoft.EntityFrameworkCore;

namespace DrawPT.Data.Repositories
{
    public class ReferenceDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<ThemeEntity> Themes => Set<ThemeEntity>();
        public DbSet<AdjectiveEntity> Adjectives => Set<AdjectiveEntity>();
        public DbSet<NounEntity> Nouns => Set<NounEntity>();
        public DbSet<AnnouncerPromptEntity> AnnouncerPrompts => Set<AnnouncerPromptEntity>();
    }

    public class ReferenceRepository
    {
        private readonly ReferenceDbContext _context;

        // Add dictionary for caching announcer prompts
        private readonly Dictionary<string, string> _announcerPrompts;

        public ReferenceRepository(ReferenceDbContext context)
        {
            _context = context;
            _announcerPrompts = _context.AnnouncerPrompts
                .AsNoTracking()
                .ToDictionary(p => p.Name, p => p.Prompt);
        }

        public List<ThemeEntity> GetAllThemes()
        {
            return [.. _context.Themes];
        }

        public List<string> GetAllAdjectives()
        {
            return [.. _context.Adjectives.Select(a => a.Adjective)];
        }

        public List<string> GetAllNouns()
        {
            return [.. _context.Nouns.Select(a => a.Noun)];
        }

        /// <summary>
        /// Retrieves a seeded announcer prompt by its key.
        /// Throws if the key is not found.
        /// </summary>
        public string GetAnnouncerPrompt(string key)
        {
            if (_announcerPrompts.TryGetValue(key, out var prompt))
                return prompt;
            throw new KeyNotFoundException($"No announcer prompt found for key '{key}'.");
        }
    }
}