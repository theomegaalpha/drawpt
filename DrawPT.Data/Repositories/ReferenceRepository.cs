using DrawPT.Data.Repositories.Reference;
using Microsoft.EntityFrameworkCore;

namespace DrawPT.Data.Repositories
{
    public class ReferenceDbContext : DbContext
    {
        public ReferenceDbContext(DbContextOptions<ReferenceDbContext> options) : base(options) { }

        public DbSet<ThemeEntity> Themes { get; set; }
        public DbSet<AdjectiveEntity> Adjectives { get; set; }
        public DbSet<NounEntity> Nouns { get; set; }
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