using DrawPT.Common.Models.Game;
using DrawPT.Common.Models;

namespace DrawPT.GameEngine.Interfaces
{
    public interface IGameAnnouncerService
    {
        Task<string?> GenerateGreetingAnnouncement(List<Player> players);
        Task<string?> GenerateRoundResultAnnouncement(string originalPrompt, RoundResults roundResults);
        Task<string?> GenerateGambleResultAnnouncement(GameGamble gamble);
        Task<string?> GenerateGameResultsAnnouncement(List<PlayerResults> playerResults);
    }
}
