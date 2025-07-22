namespace DrawPT.Common.Models.Daily
{
    public class DailyAnswerPublic
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public required string Username { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
        public required string Guess { get; set; }
        public required string Reason { get; set; }
        public int[] ClosenessArray { get; set; } = new int[10];
        public int Score { get; set; }
    }
}
