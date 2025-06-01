namespace DrawPT.Common.Models
{
    /// <summary>
    /// Represents a player in the game
    /// </summary>
    public class PlayerResult
    {
        /// <summary>
        /// Unique identifier for the player
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Player's display name
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Player's current score
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Player's connection ID for real-time communication
        /// </summary>
        public string ConnectionId { get; set; } = string.Empty;

        /// <summary>
        /// Whether the player is currently active in the game
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
} 