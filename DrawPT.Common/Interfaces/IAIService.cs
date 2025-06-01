using DrawPT.Common.Models;

namespace DrawPT.Common.Interfaces
{
    /// <summary>
    /// Interface for AI-related services
    /// </summary>
    public interface IAIService
    {
        /// <summary>
        /// Generates a new game question
        /// </summary>
        Task<GameQuestion> GenerateGameQuestionAsync();

        /// <summary>
        /// Assesses player answers for a round
        /// </summary>
        Task<List<GameAnswer>> AssessAnswersAsync(string originalPrompt, List<GameAnswer> answers);
    }
} 