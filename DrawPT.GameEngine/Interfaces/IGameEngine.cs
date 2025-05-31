using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameEngine
{
    Task<bool> InitializeGameAsync(string roomCode, GameConfiguration configuration);
    Task<bool> AddPlayerAsync(string roomCode, Player player);
    Task<bool> RemovePlayerAsync(string roomCode, string connectionId);
    Task<GameState> GetGameStateAsync(string roomCode);
    Task<GameQuestion> GenerateQuestionAsync(string roomCode, string theme);
    Task<GameAnswer> SubmitAnswerAsync(string roomCode, string connectionId, string guess, bool isGambling);
    Task<List<GameAnswer>> AssessAnswersAsync(string roomCode, int roundNumber);
    Task<GameResults> GetGameResultsAsync(string roomCode);
    Task<bool> StartGameAsync(string roomCode);
    Task<bool> EndGameAsync(string roomCode);
    Task<bool> IsGameCompleteAsync(string roomCode);
    Task<bool> IsPlayerTurnAsync(string roomCode, string connectionId);
    Task<string> GetCurrentThemeAsync(string roomCode);
    Task<bool> SetThemeAsync(string roomCode, string theme);
} 