using DrawPT.Data.Repositories.Game;

using Microsoft.EntityFrameworkCore;

namespace DrawPT.Data.Repositories
{
    public class DailiesDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<DailyAnswerEntity> DailyAnswers => Set<DailyAnswerEntity>();
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

        public DailyQuestionEntity? GetDailyQuestion(DateTime date)
        {
            return _context.DailyQuestions.Where(dq => dq.Date == date.Date).FirstOrDefault();
        }

        public List<DailyAnswerEntity> GetDailyAnswers(DateTime date)
        {
            return _context.DailyAnswers.Where(da => da.Date == date.Date).ToList();
        }

        public List<DailyAnswerEntity> GetDailyAnswersByPlayerId(Guid id, DateTime date)
        {
            return _context.DailyAnswers.Where(da => da.Id == id && da.Date == date.Date).ToList();
        }

        public async Task AddDailyQuestion(DailyQuestionEntity image)
        {
            _context.DailyQuestions.Add(image);
            await _context.SaveChangesAsync();
        }

        public async Task SaveDailyAnswer(DailyAnswerEntity answer)
        {
            var existingAnswer = await _context.DailyAnswers
                .FirstOrDefaultAsync(da => da.PlayerId == answer.PlayerId && da.QuestionId == answer.QuestionId);

            if (existingAnswer != null)
            {
                // Update existing answer
                existingAnswer.Guess = answer.Guess;
                existingAnswer.Reason = answer.Reason;
                existingAnswer.Score = answer.Score;
                _context.DailyAnswers.Update(existingAnswer);
            }
            else
            {
                // Add new answer
                _context.DailyAnswers.Add(answer);
            }
            await _context.SaveChangesAsync();
        }
    }
}
