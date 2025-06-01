using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameFlowController
{
    Task StartGameAsync(GameConfiguration config);
    Task<GameRound> StartNewRoundAsync(int roundNumber);
    Task EndRoundAsync(GameRound round);
    Task EndGameAsync();
    Task<bool> IsGameCompleteAsync();
    Task<GameState> GetCurrentStateAsync();
} 