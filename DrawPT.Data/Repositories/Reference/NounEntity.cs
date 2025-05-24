using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawPT.Data.Repositories.Reference
{
    [Keyless]
    [Table("Nouns", Schema = "ref")]
    public class NounEntity
    {
        public required string Noun { get; set; }
    }
}
