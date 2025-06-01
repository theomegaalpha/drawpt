using DrawPT.Common.Models;

namespace DrawPT.Common.Interfaces
{
    /// <summary>
    /// Interface for managing game questions and themes
    /// </summary>
    public interface IQuestionManager : IDisposable
    {
        /// <summary>
        /// Gets the current theme
        /// </summary>
        GameTheme? CurrentTheme { get; }

        /// <summary>
        /// Gets all available themes
        /// </summary>
        Task<IEnumerable<GameTheme>> GetAvailableThemesAsync();

        /// <summary>
        /// Selects a theme for the current round
        /// </summary>
        Task<GameTheme> SelectThemeAsync(string playerId);

        /// <summary>
        /// Generates a new question based on the current theme
        /// </summary>
        Task<GameQuestion> GenerateQuestionAsync();

        /// <summary>
        /// Gets a random theme from the available themes
        /// </summary>
        Task<GameTheme> GetRandomThemeAsync();
    }
}