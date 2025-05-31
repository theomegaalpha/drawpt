using System.Collections.Concurrent;

namespace DrawPT.GameEngine.Models;

public class GameState
{
    public string RoomCode { get; set; } = string.Empty;
    public bool IsGamblingUnlocked { get; set; }
    public List<GameRound> GameRounds { get; set; } = [];
    public Dictionary<string, PlayerState> Players { get; set; } = new();
    public int CurrentRound { get; set; }
    public GameStatus Status { get; set; }
    public DateTime LastUpdated { get; set; }
}

public enum GameStatus
{
    WaitingForPlayers,
    InProgress,
    Completed,
    Error
}

public class PlayerState
{
    public required string ConnectionId { get; init; }
    public required Player Player { get; init; }
    public int Correct { get; set; }
    public int Score { get; set; }
    public DateTime LastActive { get; set; }
}

public class Player
{
    public required string Id { get; init; }
    public required string Username { get; init; }
    public required string ConnectionId { get; init; }
}

public class GameRound
{
    public int RoundNumber { get; set; }
    public required GameQuestion Question { get; set; }
    public List<GameAnswer> Answers { get; set; } = [];
}

public class GameQuestion
{
    public required string Id { get; init; }
    public required string ImageUrl { get; init; }
    public required string OriginalPrompt { get; set; }
}

public class GameAnswer
{
    public required string PlayerConnectionId { get; init; }
    public required string Guess { get; init; }
    public int Score { get; set; }
    public int BonusPoints { get; set; }
    public required string Reason { get; init; }
    public bool IsGambling { get; set; }
} 