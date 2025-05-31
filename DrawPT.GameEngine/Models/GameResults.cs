namespace DrawPT.GameEngine.Models;

public class GameResults
{
    public List<PlayerResult> PlayerResults { get; set; } = [];
}

public class PlayerResult
{
    public required string Id { get; init; }
    public required string ConnectionId { get; init; }
    public required string Username { get; init; }
    public int FinalScore { get; set; }
} 