using DrawPT.Data.Repositories.Game;
using Microsoft.EntityFrameworkCore;

namespace DrawPT.Data.Repositories
{
    public class GameEntitiesDbContext : DbContext
    {
        public GameEntitiesDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ArchivedQuestionEntity> ArchivedQuestions => Set<ArchivedQuestionEntity>();
    }

    public class GameEntitiesRepository
    {
        private readonly GameEntitiesDbContext _context;

        public GameEntitiesRepository(GameEntitiesDbContext context)
        {
            _context = context;
        }

        public ArchivedQuestionEntity? GetRandomArchivedQuestion(string theme)
        {
            return _context.ArchivedQuestions.FirstOrDefault(q => q.Theme == theme) ?? _context.ArchivedQuestions.FirstOrDefault();
        }

        public List<ArchivedQuestionEntity> GetArchivedQuestions()
        {
            return _context.ArchivedQuestions.ToList();
        }

        public ArchivedQuestionEntity? GetArchivedQuestion(Guid id)
        {
            return _context.ArchivedQuestions.Find(id);
        }

        public async Task SaveArchivedQuestion(ArchivedQuestionEntity question)
        {
            var existing = await _context.ArchivedQuestions.FindAsync(question.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(question);
            }
            else
            {
                _context.ArchivedQuestions.Add(question);
            }
            await _context.SaveChangesAsync();
        }
    }
}
