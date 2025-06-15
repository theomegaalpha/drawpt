namespace DrawPT.Common.Models.Daily
{
    public class DailyQuestionPublic
    {
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
        public string? Style { get; set; }
        public string Theme { get; set; } = string.Empty;
        public required string ImageUrl { get; set; }
    }
}
