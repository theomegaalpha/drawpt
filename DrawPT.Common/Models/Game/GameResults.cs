namespace DrawPT.Common.Models.Game
{
    /// <summary>
    /// Represents the final results of a game
    /// </summary>
    public class GameResults
    {
        /// <summary>
        /// List of player results
        /// </summary>
        public List<PlayerResults> PlayerResults { get; set; } = new();

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