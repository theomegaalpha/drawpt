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
        Player AddPlayer(string connectionId, Player player);

        /// <summary>
        /// Removes a player from the game
        /// </summary>
        void RemovePlayer(string connectionId);

        /// <summary>
        /// Gets all current players in the game
        /// </summary>
        IEnumerable<Player> GetPlayers();

        /// <summary>
        /// Updates a player's score
        /// </summary>
        void UpdatePlayerScore(string connectionId, int score);

        /// <summary>
        /// Gets a specific player by their connection ID
        /// </summary>
        Player? GetPlayer(string connectionId);

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