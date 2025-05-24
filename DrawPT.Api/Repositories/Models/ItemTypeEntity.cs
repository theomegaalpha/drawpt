using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrawPT.Api.Repositories.Models
{
    [Table("ItemTypes", Schema = "ref")]
    public class ItemTypeEntity
    {
        [Key]
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Cost { get; set; }
        public bool IsTargetable { get; set; }
    }
}
