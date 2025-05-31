using DrawPT.Common.Interfaces;

namespace DrawPT.Common.Models
{
    public class GameLobby : IGameLobby
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string RoomCode { get; set; } = "ABCD";
    }
}
