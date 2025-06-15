using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawPT.Data.Repositories.Game
{
    [Table("DailyAnswers", Schema = "game")]
    public class DailyAnswerEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public Guid PlayerId { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
        public required string Guess { get; set; }
        public required string Reason { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
