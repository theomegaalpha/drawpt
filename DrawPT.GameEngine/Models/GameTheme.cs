namespace DrawPT.GameEngine.Models
{
    /// <summary>
    /// Represents a theme that can be used in game rounds
    /// </summary>
    public class GameTheme
    {
        /// <summary>
        /// Unique identifier for the theme
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Name of the theme
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the theme
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Whether this theme is currently active
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}