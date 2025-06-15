using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawPT.Data.Repositories.Game
{
    [Table("DailyAnswers", Schema = "game")]
    public class DailyAnswerEntity
    {
        [Key]
        public Guid Id { get; set; }
        public required string Guess { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
