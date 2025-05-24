namespace DrawPT.Data.Models
{
    public class GameItem
    {
        public Guid User { get; set; }
        public Guid? Target { get; set; }
        public ItemType Type { get; set; }
    }

    public class ItemType
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Cost { get; set; }
        public bool IsTargetable { get; set; }
    }
}
