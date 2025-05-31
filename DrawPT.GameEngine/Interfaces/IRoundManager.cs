using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Interfaces
{
    /// <summary>
    /// Manages game rounds and their lifecycle
    /// </summary>
    public interface IRoundManager
    {
        /// <summary>
        /// Starts a new round
        /// </summary>
        Task<GameRound> StartRoundAsync(int roundNumber);

        /// <summary>
        /// Ends the current round
        /// </summary>
        Task EndRoundAsync(int roundNumber);

        /// <summary>
        /// Processes all answers for the current round
        /// </summary>
        Task ProcessAnswersAsync(GameRound round);

        /// <summary>
        /// Selects a theme for the current round
        /// </summary>
        Task<GameTheme> SelectThemeAsync(Player player);

        /// <summary>
        /// Gets the current round number
        /// </summary>
        int CurrentRoundNumber { get; }

        /// <summary>
        /// Gets all rounds in the game
        /// </summary>
        IReadOnlyList<GameRound> Rounds { get; }

        /// <summary>
        /// Gets the current active round
        /// </summary>
        GameRound? CurrentRound { get; }
    }
}