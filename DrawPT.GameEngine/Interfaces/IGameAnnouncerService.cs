using DrawPT.Common.Models.Game;
using DrawPT.Common.Models;

namespace DrawPT.GameEngine.Interfaces
{
    public interface IGameAnnouncerService
    {
        Task<string?> GenerateGreetingAnnouncement(List<Player> players);
        Task<string?> GenerateRoundResultAnnouncement(string originalPrompt, RoundResults roundResults);
        string GenerateGameResultAnnouncement();
    }
}
