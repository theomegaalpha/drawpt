using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameCommunicationService
{
    Task<string> AskPlayerThemeAsync(Player player, int timeoutInSeconds);
    Task<PlayerAnswer> AskPlayerQuestionAsync(Player player, GameQuestion question, int timeoutInSeconds);
    void BroadcastGameEvent(string roomCode, string gameAction, object? message = null);
}
