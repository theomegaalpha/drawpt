namespace DrawPT.Common.Models.Game
{
    /// <summary>
    /// Represents a player prompt
    /// </summary>
    public class PlayerPrompt
    {
        /// <summary>
        /// Unique identifier for the player
        /// </summary>
        public Guid PlayerId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Player username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Player's avatar URL
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// Player's image prompt
        /// </summary>
        public string Prompt { get; set; }
    }
}
