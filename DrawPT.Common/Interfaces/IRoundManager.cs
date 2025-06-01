using DrawPT.Common.Models;

namespace DrawPT.Common.Interfaces
{
    /// <summary>
    /// Interface for managing game rounds
    /// </summary>
    public interface IRoundManager : IDisposable
    {
        /// <summary>
        /// Gets the current round
        /// </summary>
        GameRound? CurrentRound { get; }

        /// <summary>
        /// Gets the current round number
        /// </summary>
        int CurrentRoundNumber { get; }

        /// <summary>
        /// Gets all rounds in the game
        /// </summary>
        IReadOnlyList<GameRound> Rounds { get; }

        /// <summary>
        /// Starts a new round in the game
        /// </summary>
        Task<GameRound> StartNewRoundAsync();

        /// <summary>
        /// Submits an answer for the current round
        /// </summary>
        Task SubmitAnswerAsync(string playerId, string answer, bool isGambling = false);

        /// <summary>
        /// Ends the current round and assesses all answers
        /// </summary>
        Task EndRoundAsync();
    }
}