namespace DrawPT.Api.Models
{
    public class GameCompletedEvent
    {
        public required string RoomCode { get; init; }
        public required int Correct { get; init; }
    }
}
