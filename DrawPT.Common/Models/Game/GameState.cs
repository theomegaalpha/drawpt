using DrawPT.Common.Interfaces.Game;
using DrawPT.Common.Constants;

namespace DrawPT.Common.Models.Game
{
    /// <summary>
    /// Represents the state of a game session.
    /// </summary>
    public class GameState : IGameState
    {
        /// <summary>
        /// Gets or sets the room code for the game session.
        /// </summary>
        public string RoomCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current round number.
        /// </summary>
        public int CurrentRound { get; set; } = 0;

        /// <summary>
        /// Gets or sets the game configuration.
        /// </summary>
        public IGameConfiguration GameConfiguration { get; set; } = new GameConfiguration();

        /// <summary>
        /// Gets or sets the unique identifier of the host player.
        /// </summary>
        public Guid HostPlayerId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the player who is currently active (e.g., drawing or answering).
        /// Guid.Empty indicates action is on all players; null indicates no active player.
        /// </summary>
        public Guid? ActivePlayerId { get; set; }

        /// <summary>
        /// Gets or sets the current status of the game.
        /// </summary>
        public GameStatus CurrentStatus { get; set; }

        /// <summary>
        /// Gets or sets the list of players in the game.
        /// </summary>
        public List<Player> Players { get; set; } = new();

        /// <summary>
        /// Gets or sets the results for each player.
        /// </summary>
        public List<PlayerResults> PlayerResults { get; set; } = new();
    }
}
