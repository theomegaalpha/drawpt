using DrawPT.Data.Repositories.Game;
using Microsoft.EntityFrameworkCore;

namespace DrawPT.Data.Repositories
{
    public class DailiesDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<DailyQuestionEntity> DailyQuestions => Set<DailyQuestionEntity>();
        public DbSet<DailyThemeEntity> DailyThemes => Set<DailyThemeEntity>();
    }

    public class DailiesRepository
    {
        private readonly DailiesDbContext _context;

        public DailiesRepository(DailiesDbContext context)
        {
            _context = context;
        }

        public List<DailyThemeEntity> GetDailyThemes()
        {
            return _context.DailyThemes.ToList();
        }

        public List<DailyQuestionEntity> GetDailyQuestions(int limit = 5)
        {
            return _context.DailyQuestions.OrderByDescending(dq => dq.Date).Take(limit).ToList();
        }

        public DailyQuestionEntity? GetDailyQuestion(DateTime dateTime)
        {
            return _context.DailyQuestions.Where(dq => dq.Date == dateTime.Date).FirstOrDefault();
        }

        public async Task AddDailyQuestion(DailyQuestionEntity image)
        {
            _context.DailyQuestions.Add(image);
            await _context.SaveChangesAsync();
        }
    }
}
