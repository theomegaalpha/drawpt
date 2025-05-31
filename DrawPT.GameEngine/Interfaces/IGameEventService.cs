using DrawPT.GameEngine.Events;

namespace DrawPT.GameEngine.Interfaces
{
    /// <summary>
    /// Service for handling game events
    /// </summary>
    public interface IGameEventService
    {
        /// <summary>
        /// Publishes a game event
        /// </summary>
        void PublishEvent(IGameEvent gameEvent);

        /// <summary>
        /// Subscribes to events for a specific game
        /// </summary>
        void SubscribeToGame(string gameId);

        /// <summary>
        /// Subscribes to events for a specific player
        /// </summary>
        void SubscribeToPlayer(string gameId, string playerId);

        /// <summary>
        /// Subscribes to events for a specific round
        /// </summary>
        void SubscribeToRound(string gameId, int roundNumber);

        /// <summary>
        /// Subscribes to events for a specific question
        /// </summary>
        void SubscribeToQuestion(string gameId, string questionId);
    }
} 