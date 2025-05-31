namespace DrawPT.GameEngine.Models
{
    /// <summary>
    /// Represents the current state of a game instance
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// Game is waiting for players to join
        /// </summary>
        WaitingForPlayers,

        /// <summary>
        /// Game is in progress
        /// </summary>
        InProgress,

        /// <summary>
        /// Game is paused
        /// </summary>
        Paused,

        /// <summary>
        /// Game has ended
        /// </summary>
        Ended,

        /// <summary>
        /// Game is in an error state
        /// </summary>
        Error
    }
} 