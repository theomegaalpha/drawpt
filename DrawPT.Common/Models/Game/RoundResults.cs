namespace DrawPT.Common.Models.Game
{
    /// <summary>
    /// Represents a single round in the game
    /// </summary>
    public class RoundResults
    {
        /// <summary>
        /// Unique identifier for the round
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The round number in the game sequence
        /// </summary>
        public int RoundNumber { get; set; }

        /// <summary>
        /// The theme for this round
        /// </summary>
        public string Theme { get; set; } = string.Empty;

        /// <summary>
        /// The question for this round
        /// </summary>
        public GameQuestion Question { get; set; } = null!;

        /// <summary>
        /// All answers submitted for this round
        /// </summary>
        public List<PlayerAnswer> Answers { get; set; } = new();
    }
}