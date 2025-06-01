using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DrawPT.Common.Models.Supabase
{
    [Table("profiles")]
    public class Profile : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
