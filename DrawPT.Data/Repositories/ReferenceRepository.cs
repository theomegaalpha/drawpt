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
    }

    public class ReferenceRepository
    {
        private readonly ReferenceDbContext _context;

        public ReferenceRepository(ReferenceDbContext context)
        {
            _context = context;
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
    }
}