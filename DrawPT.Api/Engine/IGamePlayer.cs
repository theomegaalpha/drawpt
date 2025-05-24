using DrawPT.Data.Models;

namespace DrawPT.Api.Engine
{
    public interface IGamePlayer
    {
        Task SuccessfullyJoined(string connectionId);
        Task PlayerJoined(Player player);
        Task PlayerLeft(Player player);

        #region player interactions
        Task<string> AskTheme(List<string> themes, CancellationToken cancellationToken);
        Task<ItemType> AskItemSelection(List<ItemType> itemTypes, CancellationToken cancellationToken);
        Task<GameAnswerBase> AskQuestion(int roundNumber, string question, CancellationToken cancellationToken);
        #endregion

        #region game notifications
        Task GameStarted(GameConfiguration gameConfiguration);
        Task ThemeSelection(List<string> themes);
        Task ThemeSelected(string theme);
        Task AwardBonusPoints(int bonusPoints);
        Task BroadcastRoundResults(GameRound gameRound);
        Task BroadcastFinalResults(GameResults results);
        Task GameCompleted(GameCompletedEvent @event);
        #endregion

        Task WriteMessage(string message);
    }

}
