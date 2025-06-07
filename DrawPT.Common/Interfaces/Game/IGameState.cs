namespace DrawPT.Common.Interfaces.Game
{
    public interface IGameState
    {
        string RoomCode { get; set; }
        int CurrentRound { get; set; }
        IGameConfiguration GameConfiguration { get; set; }
        int TotalRounds { get; set; }
        Guid HostPlayerId { get; set; }
    }
}
