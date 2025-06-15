using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawPT.Data.Repositories.Game
{
    [Table("FreepikImageStyles", Schema = "game")]
    public class FreepikImageStyleEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string? Style { get; set; }
        public string? NegativePrompt { get; set; }
    }
}
