using DrawPT.Common.Interfaces;
using DrawPT.Common.Models.Game;

namespace DrawPT.GameEngine.Services
{
    /// <summary>
    /// Interface for AI-related services
    /// </summary>
    public class AIService : IAIService
    {
        /// <summary>
        /// Generates a new game question
        /// </summary>
        public Task<GameQuestion> GenerateGameQuestionAsync()
        {
            return Task.FromResult(new GameQuestion());
        }

        /// <summary>
        /// Assesses player answers for a round
        /// </summary>
        public Task<List<PlayerAnswer>> AssessAnswersAsync(string originalPrompt, List<PlayerAnswer> answers)
        {
            return Task.FromResult(new List<PlayerAnswer>());
        }
    }
}