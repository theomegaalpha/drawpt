using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameCommunicationService
{
    Task<string> AskPlayerTheme(Player player, int timeoutInSeconds);
    Task<PlayerAnswer> AskPlayerQuestion(Player player, GameQuestion question, int timeoutInSeconds);
    Task BroadcastGameMessage(string roomCode, string message);
    //Task BroadcastGameEvent(GameEvent gameEvent);
}