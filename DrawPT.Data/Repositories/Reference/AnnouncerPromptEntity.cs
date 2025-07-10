using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DrawPT.Data.Repositories.Reference
{
    [Table("AnnouncerPrompts", Schema = "ref")]
    public class AnnouncerPromptEntity
    {
        [Key]
        public required string Name { get; set; }
        public required string Prompt { get; set; }
    }
}
