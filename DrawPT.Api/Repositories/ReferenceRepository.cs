using DrawPT.Api.Models;
using DrawPT.Api.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace DrawPT.Api.Repositories
{
    public class ReferenceDbContext : DbContext
    {
        public ReferenceDbContext(DbContextOptions<ReferenceDbContext> options) : base(options) { }

        public DbSet<ThemeEntity> Themes { get; set; }
        public DbSet<AdjectiveEntity> Adjectives { get; set; }
        public DbSet<NounEntity> Nouns { get; set; }
        public DbSet<ItemTypeEntity> ItemTypes { get; set; }
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

        public List<ItemType> GetAllItemTypes()
        {
            return [.. _context.ItemTypes.Select(ConvertToModelItemType)];
        }

        public static ItemType ConvertToModelItemType(ItemTypeEntity entity)
        {
            return new ItemType
            {
                // Assuming both models have the same properties
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Cost = entity.Cost,
                IsTargetable = entity.IsTargetable
            };
        }
    }
}