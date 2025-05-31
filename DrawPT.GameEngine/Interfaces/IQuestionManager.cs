using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Interfaces
{
    /// <summary>
    /// Manages game questions and their lifecycle
    /// </summary>
    public interface IQuestionManager
    {
        /// <summary>
        /// Generates a new question for a given theme
        /// </summary>
        Task<GameQuestion> GenerateQuestionAsync(string theme);

        /// <summary>
        /// Processes a player's answer to a question
        /// </summary>
        Task<GameAnswer> ProcessAnswerAsync(string playerId, string answer);

        /// <summary>
        /// Assesses all answers for a question
        /// </summary>
        Task<GameAssessment> AssessAnswersAsync(GameQuestion question, IEnumerable<GameAnswer> answers);

        /// <summary>
        /// Gets a question by its ID
        /// </summary>
        Task<GameQuestion?> GetQuestionAsync(string questionId);

        /// <summary>
        /// Saves a question to the database
        /// </summary>
        Task SaveQuestionAsync(GameQuestion question);
    }
}