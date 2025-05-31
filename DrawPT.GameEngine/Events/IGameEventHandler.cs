namespace DrawPT.GameEngine.Events
{
    /// <summary>
    /// Handler for game events
    /// </summary>
    /// <typeparam name="T">The type of event to handle</typeparam>
    public interface IGameEventHandler<in T> where T : IGameEvent
    {
        /// <summary>
        /// Handles a game event
        /// </summary>
        /// <param name="gameEvent">The event to handle</param>
        Task HandleAsync(T gameEvent);
    }
} 