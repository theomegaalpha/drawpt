using DrawPT.Common.Interfaces.Game;

namespace DrawPT.Common.Models.Game
{
    public class GameState : IGameState
    {
        public string RoomCode { get; set; } = string.Empty;
        public int CurrentRound { get; set; }
        public int TotalRounds { get; set; } = 8;
        public IGameConfiguration GameConfiguration { get; set; } = new GameConfiguration();
        public Guid HostPlayer { get; set; }
        public List<Guid> Players { get; set; } = new ();
    }
}
