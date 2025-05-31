namespace DrawPT.GameEngine.Models
{
    /// <summary>
    /// Represents a cached image in the system
    /// </summary>
    public class CachedImage
    {
        /// <summary>
        /// Gets or sets the unique identifier for the image
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the original prompt used to generate the image
        /// </summary>
        public string OriginalPrompt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the theme ID associated with the image
        /// </summary>
        public string ThemeId { get; set; } = string.Empty;
    }
} 