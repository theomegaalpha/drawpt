using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameCommunicationService
{
    Task<GameTheme> AskPlayerTheme(Player player, int timeoutMilliseconds);
    Task<PlayerAnswer> AskPlayerQuestion(Player player, GameQuestion question, int timeoutMilliseconds);
    Task BroadcastGameMessage(string roomCode, string message);
    //Task BroadcastGameEvent(GameEvent gameEvent);
}