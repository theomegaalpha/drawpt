using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DrawPT.Data.Repositories.Game
{
    [Table("ArchivedQuestions", Schema = "game")]
    public class ArchivedQuestionEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Theme { get; set; } = string.Empty;
        public required string OriginalPrompt { get; set; }
        public required string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
