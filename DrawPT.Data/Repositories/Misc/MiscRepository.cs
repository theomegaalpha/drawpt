using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DrawPT.Data.Repositories.Misc
{
    public class MiscDbContext : DbContext
    {
        public MiscDbContext(DbContextOptions<MiscDbContext> options)
            : base(options)
        {
        }

        public DbSet<FeedbackEntity> Feedback { get; set; }
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
