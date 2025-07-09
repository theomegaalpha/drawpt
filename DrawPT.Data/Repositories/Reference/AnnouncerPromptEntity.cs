using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DrawPT.Data.Repositories.Reference
{
    [Keyless]
    [Table("AnnouncerPrompts", Schema = "ref")]
    internal class AnnouncerPromptEntity
    {
        public required string Name { get; set; }
        public required string Prompt { get; set; }
    }
}
