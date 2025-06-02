using DrawPT.Common.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameFlowController
{
    Task PlayGameAsync();
    Task StartGameAsync();
    Task<GameRound> StartNewRoundAsync(int roundNumber);
    Task EndRoundAsync(GameRound round);
    Task EndGameAsync();
    Task<bool> IsGameCompleteAsync();
} 