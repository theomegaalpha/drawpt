namespace DrawPT.GameEngine.Models
{
    /// <summary>
    /// Represents a game theme
    /// </summary>
    public class GameTheme
    {
        /// <summary>
        /// Gets or sets the unique identifier for the theme
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the theme
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the theme
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether this theme is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the difficulty level of the theme (1-5)
        /// </summary>
        public int DifficultyLevel { get; set; } = 1;
    }
}