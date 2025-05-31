namespace DrawPT.GameEngine.Events
{
    /// <summary>
    /// Base class for all game events
    /// </summary>
    public abstract class BaseGameEvent : IGameEvent
    {
        /// <summary>
        /// Type of the event
        /// </summary>
        public abstract string EventType { get; }

        /// <summary>
        /// When the event occurred
        /// </summary>
        public DateTime Timestamp { get; } = DateTime.UtcNow;

        /// <summary>
        /// The game ID this event belongs to
        /// </summary>
        public string GameId { get; set; } = string.Empty;
    }
} 