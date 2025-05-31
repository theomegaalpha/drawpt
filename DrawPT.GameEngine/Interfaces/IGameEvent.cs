using DrawPT.GameEngine.Events;

namespace DrawPT.GameEngine.Interfaces
{
    /// <summary>
    /// Interface for game events
    /// </summary>
    public interface IGameEvent
    {
        /// <summary>
        /// Gets the type of the event
        /// </summary>
        GameEventType EventType { get; }

        /// <summary>
        /// Gets game ID
        /// </summary>
        Guid GameId { get; }
    }
} 