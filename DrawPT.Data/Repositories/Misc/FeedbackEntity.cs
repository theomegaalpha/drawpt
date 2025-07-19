using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawPT.Data.Repositories.Misc
{
    [Table("Feedback", Schema = "misc")]
    public class FeedbackEntity
    {
        [Key]
        public Guid Id { get; set; }
        public required string Type { get; set; }
        public required string Message { get; set; }
        public bool IsResolved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
