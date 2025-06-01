namespace DrawPT.Common.Models
{
    /// <summary>
    /// Represents a player's answer to a game question
    /// </summary>
    public class GameAnswer
    {
        /// <summary>
        /// Unique identifier for the answer
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The player's connection ID
        /// </summary>
        public string PlayerConnectionId { get; set; } = string.Empty;

        /// <summary>
        /// The player's guess/answer
        /// </summary>
        public string Guess { get; set; } = string.Empty;

        /// <summary>
        /// The score awarded for this answer
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Any bonus points awarded
        /// </summary>
        public int BonusPoints { get; set; }

        /// <summary>
        /// Explanation for the score
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Whether this answer was a gambling attempt
        /// </summary>
        public bool IsGambling { get; set; }

        /// <summary>
        /// When the answer was submitted
        /// </summary>
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}