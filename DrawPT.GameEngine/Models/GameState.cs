using System.Collections.Concurrent;
using DrawPT.Common.Models.Game;

namespace DrawPT.GameEngine.Models;

public class GameState
{
    public string RoomCode { get; init; } = string.Empty;
    public GameStatus Status { get; set; }
    public List<GameRound> Rounds { get; } = new();
    public ConcurrentDictionary<string, PlayerState> Players { get; } = new();
}

public enum GameStatus
{
    WaitingForPlayers,
    InProgress,
    Completed,
    Abandoned
}

public class PlayerState
{
    public required string ConnectionId { get; init; }
    public required Player Player { get; init; }
    public int Score { get; set; }
    public int BonusPoints { get; set; }
}

public class Player
{
    public required string Id { get; init; }
    public required string Username { get; init; }
} 