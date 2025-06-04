using DrawPT.Common.Models.Game;
using DrawPT.GameEngine.Interfaces;

namespace DrawPT.GameEngine.Services
{
    public class QuestionService : IQuestionService
    {
        public async Task<GameQuestion> GenerateQuestionAsync(string theme)
        {
            // Simulate question generation logic
            await Task.Delay(1000); // Simulating async operation
            
            // For now, return a dummy question
            return new GameQuestion
            {
                Id = Guid.NewGuid(),
                Theme = theme,
                OriginalPrompt = "Nier Automata's 2B portrayed in a watercolor masterpiece by Greg Rutkowski, capturing sharp focus in a stunning studio photograph.",
                ImageUrl = "https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f115b0b802ee4960a3da9f_4PrnPJDEMZkZm9djp21r1YPHRyIN-bRf2ncVUjssRDI.webp"
            };
        }
    }
}
