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
        public int[] ClosenessArray { get; set; } = new int[10]; // Array of 10 integers from 0 to 9 representing semantic accuracy
        public required string Reason { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
