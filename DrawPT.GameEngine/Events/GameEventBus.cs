using DrawPT.GameEngine.Interfaces;

namespace DrawPT.GameEngine.Events
{
    /// <summary>
    /// Implementation of the game event bus
    /// </summary>
    public class GameEventBus : IGameEventBus
    {
        private readonly ILogger<GameEventBus> _logger;
        private readonly Dictionary<Type, List<object>> _handlers = new();

        public GameEventBus(ILogger<GameEventBus> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Publishes an event to all subscribers
        /// </summary>
        public async Task PublishAsync<T>(T gameEvent) where T : IGameEvent
        {
            var eventType = typeof(T);
            if (_handlers.TryGetValue(eventType, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    try
                    {
                        await ((IGameEventHandler<T>)handler).HandleAsync(gameEvent);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error handling event {EventType} in game {GameId}", 
                            gameEvent.EventType, gameEvent.GameId);
                    }
                }
            }
        }

        /// <summary>
        /// Subscribes a handler to a specific event type
        /// </summary>
        public void Subscribe<T>(IGameEventHandler<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);
            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = new List<object>();
            }
            _handlers[eventType].Add(handler);
        }

        /// <summary>
        /// Unsubscribes a handler from a specific event type
        /// </summary>
        public void Unsubscribe<T>(IGameEventHandler<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);
            if (_handlers.TryGetValue(eventType, out var handlers))
            {
                handlers.Remove(handler);
            }
        }
    }
} 