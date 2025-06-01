namespace DrawPT.Common.Interfaces
{
    public interface IGameState
    {
        string RoomCode { get; set; }
        int CurrentRound { get; set; }
        int MaxRounds { get; set; }
        Guid HostPlayer { get; set; }
        List<Guid> Players { get; set; }

    }
}
