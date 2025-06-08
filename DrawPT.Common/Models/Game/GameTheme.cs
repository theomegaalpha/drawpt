namespace DrawPT.Common.Models.Game
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
        /// Gets or sets the active status of this theme
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}