namespace DrawPT.Api.Models
{
    public class Player
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? ConnectionId { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }

    public class PlayerResult : Player
    {
        public int FinalScore { get; set; }
    }
}
