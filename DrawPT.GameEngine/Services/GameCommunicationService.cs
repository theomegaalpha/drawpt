using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;
using DrawPT.GameEngine.Interfaces;

namespace DrawPT.GameEngine.Services;

public class GameCommunicationService : IGameCommunicationService
{
    public Task<GameTheme> AskPlayerTheme(Player player, int timeoutMilliseconds)
    {
        return Task.FromResult(new GameTheme());
    }
    public Task<PlayerAnswer> AskPlayerQuestion(Player player, GameQuestion question, int timeoutMilliseconds)
    {
        return Task.FromResult(new PlayerAnswer());
    }
    public Task BroadcastGameMessage(string roomCode, string message)
    {

        return Task.CompletedTask;
    }
}