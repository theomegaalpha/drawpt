using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Interfaces
{
    /// <summary>
    /// Manages player-related operations within the game
    /// </summary>
    public interface IPlayerManager
    {
        /// <summary>
        /// Adds a new player to the game
        /// </summary>
        Task<Player> AddPlayerAsync(string connectionId, Player player);

        /// <summary>
        /// Removes a player from the game
        /// </summary>
        Task RemovePlayerAsync(string connectionId);

        /// <summary>
        /// Gets all current players in the game
        /// </summary>
        Task<IEnumerable<Player>> GetPlayersAsync();

        /// <summary>
        /// Updates a player's score
        /// </summary>
        Task UpdatePlayerScoreAsync(string connectionId, int score);

        /// <summary>
        /// Gets a specific player by their connection ID
        /// </summary>
        Task<Player?> GetPlayerAsync(string connectionId);

        /// <summary>
        /// Checks if the game has reached maximum player capacity
        /// </summary>
        bool IsGameFull();

        /// <summary>
        /// Gets the current number of players in the game
        /// </summary>
        int PlayerCount { get; }
    }
}