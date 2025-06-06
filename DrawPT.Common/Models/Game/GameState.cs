using DrawPT.Common.Interfaces.Game;

namespace DrawPT.Common.Models.Game
{
    public class GameState : IGameState
    {
        public string RoomCode { get; set; } = string.Empty;
        public int CurrentRound { get; set; } = 0;
        public int TotalRounds { get; set; } = 8;
        public IGameConfiguration GameConfiguration { get; set; }
        public Guid HostPlayerId { get; set; }
        public List<Guid> Players { get; set; } = new ();

        public GameState(GameConfiguration gameConfiguration)
        {
            GameConfiguration = gameConfiguration;
        }

        public GameState() : this(new GameConfiguration()) { }
    }

    public enum GameStatus
    {
        WaitingForPlayers,
        InProgress,
        Completed,
        Abandoned
    }
}
