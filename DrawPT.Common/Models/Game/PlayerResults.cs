namespace DrawPT.Common.Models.Game
{
    /// <summary>
    /// Represents a player in the game
    /// </summary>
    public class PlayerResults
    {
        /// <summary>
        /// Unique identifier for the player
        /// </summary>
        public Guid PlayerId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Player's display name
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Player's current score
        /// </summary>
        public int Score { get; set; }
    }
} 