namespace DrawPT.Common.Models.Game
{
    /// <summary>
    /// Represents a gamble in a game round
    /// </summary>
    public class GameGambleBase
    {
        /// <summary>
        /// Player Id of the gambler
        /// </summary>
        public Guid GamblerId { get; set; } = Guid.Empty;

        /// <summary>
        /// Player Id of the player who's being gambled on
        /// </summary>
        public Guid PlayerId { get; set; } = Guid.Empty;

        /// <summary>
        /// Gamble type high or low
        /// </summary>
        public bool IsHigh { get; set; } = true;

        /// <summary>
        /// When this gamble was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Represents a gamble in a game round
    /// </summary>
    public class GameGamble : GameGambleBase
    {
        /// <summary>
        /// The score awarded for the gamble
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Any bonus points awarded
        /// </summary>
        public int BonusPoints { get; set; }
    }
}
