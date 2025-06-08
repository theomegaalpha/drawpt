using DrawPT.Common.Interfaces;
using DrawPT.Common.Models.Game;
using DrawPT.GameEngine.Interfaces;

namespace DrawPT.GameEngine.Services
{
    public class QuestionService : IQuestionService
    {
        IAIService _aiService;

        public QuestionService(IAIService aiService)
        {
            _aiService = aiService;
        }

        public async Task<GameQuestion> GenerateQuestionAsync(string theme)
        {
            // Simulate question generation logic
            await Task.Delay(1000);
            return await _aiService.GenerateGameQuestionAsync(theme);
        }
    }
}
