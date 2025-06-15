using DrawPT.Data.Repositories.Game;
using Microsoft.EntityFrameworkCore;

namespace DrawPT.Data.Repositories
{
    public class DailiesDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<DailyQuestionEntity> DailyQuestions => Set<DailyQuestionEntity>();
    }

    public class DailiesRepository
    {
        private readonly DailiesDbContext _context;

        public DailiesRepository(DailiesDbContext context)
        {
            _context = context;
        }

        public List<DailyQuestionEntity> GetDailyQuestions(DateTime dateTime)
        {
            return _context.DailyQuestions.Where(dq => dq.Date == dateTime.Date).ToList();
        }

        public async Task AddDailyQuestion(DailyQuestionEntity image)
        {
            _context.DailyQuestions.Add(image);
            await _context.SaveChangesAsync();
        }
    }
}
