using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DrawPT.Common.Models.Supabase
{
    [Table("profiles")]
    public class Profile : BaseModel
    {
        [PrimaryKey("id", false)]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [Column("username")]
        [JsonProperty("username")]
        public string Username { get; set; } = string.Empty;

        [Column("updated_at")]
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("created_at")]
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
