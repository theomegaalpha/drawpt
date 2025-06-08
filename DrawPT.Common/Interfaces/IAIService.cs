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
        Task<GameQuestion> GenerateGameQuestionAsync(string theme);
        Task<GameQuestion> GenerateFakeGameQuestionAsync(string theme);

        /// <summary>
        /// Assesses player answers for a round
        /// </summary>
        Task<List<PlayerAnswer>> AssessAnswerAsync(string originalPrompt, List<PlayerAnswer> answers);
        
    }
} 