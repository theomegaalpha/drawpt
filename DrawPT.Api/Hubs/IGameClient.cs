using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;

namespace DrawPT.Api.Hubs
{
    public interface IGameClient
    {
        Task PlayerJoined(Player player);
        Task PlayerLeft(Player player);
        Task PlayerScoreUpdated(Guid playerId, int newScore);
        Task GameStarted(GameConfiguration configuration);
        Task GameEnded(GameResults results);
        Task RoundStarted(GameRound round);
        Task RoundEnded(GameRound round);
        Task SuccessfullyJoined(string connectionId);
        Task WriteMessage(string message);



        Task ThemeSelection(List<string> themes);
        Task ThemeSelected(string theme);


        Task<string> AskTheme(List<string> themes, CancellationToken ct);
        Task<PlayerAnswerBase> AskQuestion(GameQuestion question, CancellationToken ct);
    }
}
