using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Interfaces
{
    /// <summary>
    /// Interface for theme repository operations
    /// </summary>
    public interface IThemeRepository
    {
        /// <summary>
        /// Gets all active themes
        /// </summary>
        Task<IEnumerable<GameTheme>> GetActiveThemesAsync();

        /// <summary>
        /// Gets a theme by its ID
        /// </summary>
        Task<GameTheme?> GetThemeByIdAsync(string themeId);

        /// <summary>
        /// Adds a new theme
        /// </summary>
        Task<GameTheme> AddThemeAsync(GameTheme theme);

        /// <summary>
        /// Updates an existing theme
        /// </summary>
        Task<GameTheme> UpdateThemeAsync(GameTheme theme);

        /// <summary>
        /// Deactivates a theme
        /// </summary>
        Task DeactivateThemeAsync(string themeId);
    }
} 