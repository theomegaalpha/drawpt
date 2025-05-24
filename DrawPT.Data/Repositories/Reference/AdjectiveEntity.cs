using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawPT.Data.Repositories.Reference
{
    [Keyless]
    [Table("Adjectives", Schema = "ref")]
    public class AdjectiveEntity
    {
        public required string Adjective { get; set; }
    }
}
