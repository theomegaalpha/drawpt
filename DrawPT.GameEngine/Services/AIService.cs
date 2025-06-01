using DrawPT.Common.Interfaces;
using DrawPT.Common.Models;

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
        public Task<List<GameAnswer>> AssessAnswersAsync(string originalPrompt, List<GameAnswer> answers)
        {
            return Task.FromResult(new List<GameAnswer>());
        }
    }
}