using DrawPT.Api.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace DrawPT.Api.Repositories
{
    public class ImageDbContext : DbContext
    {
        public ImageDbContext(DbContextOptions<ImageDbContext> options) : base(options) { }

        public DbSet<CachedImageEntity> CachedImages { get; set; }
    }

    public class ImageRepository
    {
        private readonly ImageDbContext _context;

        public ImageRepository(ImageDbContext context)
        {
            _context = context;
        }

        public List<CachedImageEntity> GetCachedImagesByTheme(Guid themeId)
        {
            return _context.CachedImages.Where(ci => ci.ThemeId == themeId).ToList();
        }

        public async Task AddCachedImage(CachedImageEntity image)
        {
            _context.CachedImages.Add(image);
            await _context.SaveChangesAsync();
        }
    }
}