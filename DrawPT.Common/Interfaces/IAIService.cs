using DrawPT.Common.Models.Game;

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
        Task<List<PlayerAnswer>> AssessAnswersAsync(string originalPrompt, List<PlayerAnswer> answers);
    }
} 