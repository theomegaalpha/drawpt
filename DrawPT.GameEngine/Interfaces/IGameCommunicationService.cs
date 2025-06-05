using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;
using System.Diagnostics.CodeAnalysis;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameCommunicationService
{
    Task<string> AskPlayerTheme(Player player, int timeoutInSeconds);
    Task<PlayerAnswer> AskPlayerQuestion(Player player, GameQuestion question, int timeoutInSeconds);
    void BroadcastGameEvent(string roomCode, string gameAction, object? message = null);
}