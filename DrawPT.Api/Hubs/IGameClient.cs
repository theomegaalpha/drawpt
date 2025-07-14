using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;
using DrawPT.Common.Interfaces.Game;

namespace DrawPT.Api.Hubs
{
    public interface IGameClient
    {
        /*
         *  Room lifecycle events
         */
        Task RoomIsFull();
        Task AlreadyInRoom();
        Task NavigateToRoom();
        Task ErrorJoiningRoom(string error);
        Task InitRoomPlayers(List<Player> players);
        Task SuccessfullyJoined(string connectionId, IGameState gameState);

        Task PlayerJoined(Player player);
        Task PlayerLeft(Player player);

        /*
         *  Game lifecycle events
         */
        Task GameStarted(IGameState? state);
        Task PlayerScoreUpdated(Guid playerId, int newScore);
        Task PlayerAnswered(PlayerAnswer playerAnswer);

        Task ThemeSelection(List<string> themes);
        Task ThemeSelected(string theme);

        Task RoundStarted(int roundNumber);
        Task RoundResults(RoundResults round);
        Task BroadcastFinalResults(GameResults results);


        Task WriteMessage(string message);
        Task WriteError(string message);


        /*
         *  Game interaction events
         */
        Task<string> AskTheme(List<string> themes, CancellationToken ct);
        Task<PlayerAnswerBase> AskQuestion(GameQuestion question, CancellationToken ct);

        /*
         *  Streaming audio chunks
         */
        Task ReceiveAudio(byte[] audioChunk);
        Task AudioStreamCompleted();
    }
}
