using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrawPT.Data.Repositories.Misc;
using Microsoft.EntityFrameworkCore;

namespace DrawPT.Data.Repositories
{
    public class MiscDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<FeedbackEntity> Feedback => Set<FeedbackEntity>();
    }

    public class MiscRepository
    {
        private readonly MiscDbContext _context;

        public MiscRepository(MiscDbContext context)
        {
            _context = context;
        }

        public List<FeedbackEntity> GetAllFeedback()
        {
            return _context.Feedback
                .AsNoTracking()
                .ToList();
        }

        public FeedbackEntity? GetFeedback(Guid id)
        {
            return _context.Feedback.Find(id);
        }

        public async Task SaveFeedbackAsync(FeedbackEntity entity)
        {
            var existing = await _context.Feedback.FindAsync(entity.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(entity);
            }
            else
            {
                _context.Feedback.Add(entity);
            }
            await _context.SaveChangesAsync();
        }
    }
}
