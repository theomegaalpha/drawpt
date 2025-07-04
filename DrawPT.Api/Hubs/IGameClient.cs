using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;
using DrawPT.Common.Interfaces.Game;

namespace DrawPT.Api.Hubs
{
    public interface IGameClient
    {
        Task RoomIsFull();
        Task AlreadyInRoom();
        Task NavigateToRoom();
        Task ErrorJoiningRoom(string error);
        Task InitRoomPlayers(List<Player> players);

        Task PlayerJoined(Player player);
        Task PlayerLeft(Player player);
        Task PlayerScoreUpdated(Guid playerId, int newScore);
        Task GameStarted(IGameState? state);
        Task BroadcastFinalResults(GameResults results);
        Task RoundStarted(int roundNumber);
        Task RoundResults(RoundResults round);
        Task SuccessfullyJoined(string connectionId);
        Task WriteMessage(string message);
        Task WriteError(string message);


        Task AnnouncerSpeaks();

        Task ThemeSelection(List<string> themes);
        Task ThemeSelected(string theme);


        Task<string> AskTheme(List<string> themes, CancellationToken ct);
        Task<PlayerAnswerBase> AskQuestion(GameQuestion question, CancellationToken ct);

        // Streaming audio chunks
        Task ReceiveAudio(string audioChunk);
        Task AudioStreamCompleted();
    }
}
