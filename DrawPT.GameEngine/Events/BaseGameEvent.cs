using DrawPT.GameEngine.Interfaces;

namespace DrawPT.GameEngine.Events
{
    /// <summary>
    /// Base class for all game events
    /// </summary>
    public abstract class BaseGameEvent : IGameEvent
    {
        /// <summary>
        /// Gets the type of the event
        /// </summary>
        public abstract GameEventType EventType { get; }

        /// <summary>
        /// When the event occurred
        /// </summary>
        public DateTime Timestamp { get; } = DateTime.UtcNow;

        /// <summary>
        /// The game ID this event belongs to
        /// </summary>
        public Guid GameId { get; set; } = Guid.NewGuid();
    }
} 