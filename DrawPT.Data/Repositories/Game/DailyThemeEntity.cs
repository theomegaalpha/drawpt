using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawPT.Data.Repositories.Game
{
    [Table("DailyThemes", Schema = "game")]
    public class DailyThemeEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string? Style { get; set; }
        public required string Theme { get; set; }
    }
}
