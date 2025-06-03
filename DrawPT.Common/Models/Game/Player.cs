namespace DrawPT.Common.Models
{
    /// <summary>
    /// Represents a player in the game
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Unique identifier for the player
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Player's display name
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Player's connection ID for real-time communication
        /// </summary>
        public string ConnectionId { get; set; } = string.Empty;

        /// <summary>
        /// Whether the player is currently active in the game
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Room code for the game this player is in
        /// </summary>
        public string RoomCode { get; set; } = string.Empty;
    }
} 