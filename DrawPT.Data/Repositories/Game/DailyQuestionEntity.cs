using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawPT.Data.Repositories.Game
{
    [Table("DailyQuestions", Schema = "game")]
    public class DailyQuestionEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
        public string? Style { get; set; }
        public string Theme { get; set; } = string.Empty;
        public required string OriginalPrompt { get; set; }
        public required string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
