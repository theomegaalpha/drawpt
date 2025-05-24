using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawPT.Api.Repositories.Models
{
    [Table("Themes", Schema = "ref")]
    public class ThemeEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
