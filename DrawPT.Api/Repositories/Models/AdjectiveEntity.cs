using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawPT.Api.Repositories.Models
{
    [Keyless]
    [Table("Adjectives", Schema = "ref")]
    public class AdjectiveEntity
    {
        public required string Adjective { get; set; }
    }
}
