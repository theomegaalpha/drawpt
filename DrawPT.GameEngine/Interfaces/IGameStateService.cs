using DrawPT.Common.Interfaces.Game;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameStateService
{
    Task<IGameState> StartGameAsync(string roomCode);
    Task<IGameState> AskThemeAsync(string roomCode);
    Task<IGameState> ChooseThemeAsync(string roomCode);
    Task<IGameState> AskQuestionAsync(string roomCode);

    Task<IGameState> StartRoundAsync(string roomCode, int roundNumber);
    Task<IGameState> EndGameAsync(string roomCode);
}
