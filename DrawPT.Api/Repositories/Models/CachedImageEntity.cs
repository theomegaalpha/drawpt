using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawPT.Api.Repositories.Models
{
    [Table("CachedImages", Schema = "game")]
    public class CachedImageEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ThemeId { get; set; }
        public required string OriginalPrompt { get; set; }
    }
}
