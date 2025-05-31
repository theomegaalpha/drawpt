using DrawPT.GameEngine.Interfaces;

namespace DrawPT.GameEngine.Models
{
    public class GameState : IGameState
    {
        public string RoomCode { get; set; }
        public int CurrentRound { get; set; }
        public int MaxRounds { get; set; }
        public Guid HostPlayer { get; set; }
        public List<Guid> Players { get; set; }
        
        public GameState()
        {
            Players = new List<Guid>();
        }

    }
}
