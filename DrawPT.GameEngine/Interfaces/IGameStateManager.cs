using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameStateManager
{
    Task<GameState> GetGameStateAsync(string roomCode);
    Task SaveGameStateAsync(string roomCode, GameState state);
    Task<bool> AddPlayerAsync(string roomCode, Player player);
    Task<bool> RemovePlayerAsync(string roomCode, string connectionId);
    Task<bool> UpdatePlayerStateAsync(string roomCode, string connectionId, PlayerState state);
    Task<bool> AddGameRoundAsync(string roomCode, GameRound round);
    Task<bool> UpdateGameStatusAsync(string roomCode, GameStatus status);
    Task<bool> LockGameAsync(string roomCode);
    Task<bool> UnlockGameAsync(string roomCode);
    Task<bool> IsGameLockedAsync(string roomCode);
    Task PublishGameEventAsync(string roomCode, string eventType, object data);
    Task SubscribeToGameEventsAsync(string roomCode, Func<string, object, Task> handler);
} 