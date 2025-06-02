using DrawPT.Common.Models;

namespace DrawPT.Common.Interfaces
{
    public interface ICacheService
    {
        Task<Room?> GetRoomAsync(string code);
        Task<Room> CreateRoomAsync();
        Task<bool> UpdateRoomAsync(Room room);
        void CloseRoom(string roomCode);
        Task<bool> RoomExistsAsync(string code);
        Task<Player?> GetPlayerAsync(Guid id);
        Task<Player> CreatePlayerAsync();
        Task<Player> UpdatePlayerAsync(Player player);
        Task<Player?> GetPlayerSessionAsync(string connectionId);
        Task SetPlayerSessionAsync(string connectionId, Player player);

        Task<List<Player>> GetRoomPlayersAsync(string roomCode);
        Task AddPlayerToRoom(string roomCode, Player player);
        Task RemovePlayerFromRoom(string roomCode, Player player);
    }
}
