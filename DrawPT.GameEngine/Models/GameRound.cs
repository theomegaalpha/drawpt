namespace DrawPT.GameEngine.Models
{
    /// <summary>
    /// Represents a single round in the game
    /// </summary>
    public class GameRound
    {
        /// <summary>
        /// Unique identifier for the round
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The round number in the game sequence
        /// </summary>
        public int RoundNumber { get; set; }

        /// <summary>
        /// The theme for this round
        /// </summary>
        public GameTheme Theme { get; set; } = null!;

        /// <summary>
        /// The question for this round
        /// </summary>
        public GameQuestion Question { get; set; } = null!;

        /// <summary>
        /// All answers submitted for this round
        /// </summary>
        public List<GameAnswer> Answers { get; set; } = new();

        /// <summary>
        /// Whether the round is currently active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// When the round started
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// When the round ended
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}