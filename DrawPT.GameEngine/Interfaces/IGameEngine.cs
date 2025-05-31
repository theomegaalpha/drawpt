using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Interfaces
{
    /// <summary>
    /// Interface for the game engine
    /// </summary>
    public interface IGameEngine : IDisposable
    {
        /// <summary>
        /// Gets the unique identifier for this game instance
        /// </summary>
        string GameId { get; }

        /// <summary>
        /// Gets the game configuration
        /// </summary>
        GameConfiguration Configuration { get; }

        /// <summary>
        /// Gets the player manager for this game
        /// </summary>
        IPlayerManager PlayerManager { get; }

        /// <summary>
        /// Gets the round manager for this game
        /// </summary>
        IRoundManager RoundManager { get; }

        /// <summary>
        /// Gets the question manager for this game
        /// </summary>
        IQuestionManager QuestionManager { get; }

        /// <summary>
        /// Starts the game
        /// </summary>
        Task StartGameAsync();

        /// <summary>
        /// Ends the game
        /// </summary>
        Task EndGameAsync();
    }
}