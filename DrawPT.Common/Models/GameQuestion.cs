namespace DrawPT.Common.Models
{
    /// <summary>
    /// Represents a question in a game round
    /// </summary>
    public class GameQuestion
    {
        /// <summary>
        /// Unique identifier for the question
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The original prompt used to generate the question
        /// </summary>
        public string OriginalPrompt { get; set; } = string.Empty;

        /// <summary>
        /// URL to the image associated with the question
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// The theme this question belongs to
        /// </summary>
        public string ThemeId { get; set; } = string.Empty;

        /// <summary>
        /// When this question was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}