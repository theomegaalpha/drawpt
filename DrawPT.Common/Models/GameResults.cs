namespace DrawPT.Common.Models
{
    /// <summary>
    /// Represents the final results of a game
    /// </summary>
    public class GameResults
    {
        /// <summary>
        /// List of player results
        /// </summary>
        public List<PlayerResult> PlayerResults { get; set; } = new();

        /// <summary>
        /// When the game ended
        /// </summary>
        public DateTime EndedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Total number of rounds played
        /// </summary>
        public int TotalRounds { get; set; }

        /// <summary>
        /// Whether the game was completed successfully
        /// </summary>
        public bool WasCompleted { get; set; }
    }
}