namespace DrawPT.GameEngine.Interfaces
{
    /// <summary>
    /// Manages game events and their distribution
    /// </summary>
    public interface IGameEventBus
    {
        /// <summary>
        /// Publishes an event to all subscribers
        /// </summary>
        Task PublishAsync<T>(T gameEvent) where T : IGameEvent;

        /// <summary>
        /// Subscribes a handler to a specific event type
        /// </summary>
        void Subscribe<T>(IGameEventHandler<T> handler) where T : IGameEvent;

        /// <summary>
        /// Unsubscribes a handler from a specific event type
        /// </summary>
        void Unsubscribe<T>(IGameEventHandler<T> handler) where T : IGameEvent;
    }
}