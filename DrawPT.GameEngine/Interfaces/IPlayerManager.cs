using DrawPT.Common.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IPlayerManager
{
    Task<bool> AddPlayerAsync(string connectionId, Player player);
    Task RemovePlayerAsync(string connectionId);
    Task<bool> IsRoomFullAsync();
    Task BroadcastPlayerUpdateAsync(Player player, PlayerUpdateType updateType);
    Task<IEnumerable<Player>> GetPlayersAsync();
}

public enum PlayerUpdateType
{
    Joined,
    Left,
    ScoreUpdated
} 