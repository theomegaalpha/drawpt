using DrawPT.GameEngine.Models;
using DrawPT.GameEngine.Events;

namespace DrawPT.GameEngine.Interfaces
{
    /// <summary>
    /// Core interface for the game engine that manages the game lifecycle and state
    /// </summary>
    public interface IGameEngine
    {
        /// <summary>
        /// Unique identifier for the game instance
        /// </summary>
        string GameId { get; }

        /// <summary>
        /// Current state of the game
        /// </summary>
        GameState CurrentState { get; }

        /// <summary>
        /// Starts a new game instance
        /// </summary>
        Task StartGameAsync(GameConfiguration configuration);

        /// <summary>
        /// Ends the current game instance
        /// </summary>
        Task EndGameAsync();

        /// <summary>
        /// Adds a player to the game
        /// </summary>
        Task<bool> AddPlayerAsync(string connectionId, Player player);

        /// <summary>
        /// Removes a player from the game
        /// </summary>
        Task RemovePlayerAsync(string connectionId);

        /// <summary>
        /// Processes a single round of the game
        /// </summary>
        Task ProcessRoundAsync(int roundNumber);

        /// <summary>
        /// Gets the current game configuration
        /// </summary>
        GameConfiguration GetConfiguration();
    }
}