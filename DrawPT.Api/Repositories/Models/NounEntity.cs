using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawPT.Api.Repositories.Models
{
    [Keyless]
    [Table("Nouns", Schema = "ref")]
    public class NounEntity
    {
        public required string Noun { get; set; }
    }
}
