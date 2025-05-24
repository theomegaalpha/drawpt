namespace DrawPT.Data.Models
{
    public class GameQuestion
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string OriginalPrompt { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
