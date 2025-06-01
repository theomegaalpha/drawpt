namespace DrawPT.GameEngine.Models;

public class GameRound
{
    public int RoundNumber { get; init; }
    public Theme? SelectedTheme { get; set; }
    public GameQuestion? Question { get; set; }
    public List<PlayerAnswer> Answers { get; } = new();
    public bool IsComplete { get; set; }
}

public class Theme
{
    public required string Id { get; init; }
    public required string Name { get; init; }
}

public class GameQuestion
{
    public required string Id { get; init; }
    public required string ImageUrl { get; init; }
    public required string OriginalPrompt { get; init; }
}

public class PlayerAnswer
{
    public required string PlayerConnectionId { get; init; }
    public required string Guess { get; init; }
    public int Score { get; set; }
    public int BonusPoints { get; set; }
    public string? Reason { get; set; }
    public bool IsGambling { get; set; }
} 