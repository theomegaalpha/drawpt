using DrawPT.Common.Constants;
using DrawPT.Common.Models.Game;

namespace DrawPT.Common.Interfaces.Game
{
    public interface IGameState
    {
        string RoomCode { get; set; }
        int CurrentRound { get; set; }
        IGameConfiguration GameConfiguration { get; set; }
        int TotalRounds { get; set; }
        Guid HostPlayerId { get; set; }
        GameStatus CurrentStatus { get; set; }
        List<string> Themes { get; set; }
        List<PlayerResults> PlayerResults { get; set; }
    }
}
