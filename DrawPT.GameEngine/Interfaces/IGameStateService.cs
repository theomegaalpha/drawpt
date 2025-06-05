using DrawPT.Common.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameStateService
{
    Task StartGameAsync(string roomCode);
    Task StartRoundAsync(string roomCode, int roundNumber);
    Task EndGameAsync(string roomCode);
}