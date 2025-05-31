namespace DrawPT.GameEngine.Models
{
    /// <summary>
    /// Represents a player's final results in a game
    /// </summary>
    public class PlayerResult
    {
        /// <summary>
        /// Player's unique identifier
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Player's connection ID
        /// </summary>
        public string ConnectionId { get; set; } = string.Empty;

        /// <summary>
        /// Player's username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Player's final score
        /// </summary>
        public int FinalScore { get; set; }
    }
}