using DrawPT.Common.Interfaces.Game;
using DrawPT.Common.Constants;

namespace DrawPT.Common.Models.Game
{
    public class GameState : IGameState
    {
        public string RoomCode { get; set; } = string.Empty;
        public int CurrentRound { get; set; } = 0;
        public int TotalRounds { get; set; } = 8;
        public IGameConfiguration GameConfiguration { get; set; } = new GameConfiguration();
        public Guid HostPlayerId { get; set; }
        public GameStatus CurrentStatus { get; set; }
        public List<Player> Players { get; set; } = new();
        public List<PlayerResults> PlayerResults { get; set; } = new();
    }
}
