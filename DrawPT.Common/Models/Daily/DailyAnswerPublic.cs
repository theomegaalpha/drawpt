namespace DrawPT.Common.Models.Daily
{
    public class DailyAnswerPublic
    {
        public Guid PlayerId { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
        public required string Guess { get; set; }
        public required string Reason { get; set; }
        public int Score { get; set; }
    }
}
