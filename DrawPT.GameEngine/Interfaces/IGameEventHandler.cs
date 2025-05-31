using DrawPT.GameEngine.Events;

namespace DrawPT.GameEngine.Interfaces
{
    /// <summary>
    /// Interface for handling game events
    /// </summary>
    public interface IGameEventHandler
    {
        /// <summary>
        /// Gets the type of event this handler can process
        /// </summary>
        GameEventType EventType { get; }

        /// <summary>
        /// Handles a game event
        /// </summary>
        /// <param name="eventJson">The event data as JSON</param>
        void HandleEvent(string eventJson);
    }
} 