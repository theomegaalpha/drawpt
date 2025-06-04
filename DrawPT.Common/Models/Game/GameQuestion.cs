namespace DrawPT.Common.Models.Game
{
    /// <summary>
    /// Represents a question in a game round
    /// </summary>
    public class GameQuestion
    {
        /// <summary>
        /// Unique identifier for the question
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        public int RoundNumber { get; set; } = 0;


        /// <summary>
        /// The theme of this question
        /// </summary>
        public string Theme { get; set; } = string.Empty;

        /// <summary>
        /// The original prompt used to generate the question
        /// </summary>
        public string OriginalPrompt { get; set; } = string.Empty;

        /// <summary>
        /// URL to the image associated with the question
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// When this question was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}