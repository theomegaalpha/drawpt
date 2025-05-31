namespace DrawPT.GameEngine.Events
{
    /// <summary>
    /// Base interface for all game events
    /// </summary>
    public interface IGameEvent
    {
        /// <summary>
        /// Type of the event
        /// </summary>
        string EventType { get; }

        /// <summary>
        /// When the event occurred
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The game ID this event belongs to
        /// </summary>
        string GameId { get; }
    }
} 