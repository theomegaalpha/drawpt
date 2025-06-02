using DrawPT.GameEngine.Models;
using DrawPT.Common.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IScoringService
{
    Task<int> CalculateRoundScoreAsync(PlayerAnswer answer, PlayerAnswer assessment);
    Task<int> CalculateBonusPointsAsync(PlayerAnswer answer);
    Task<int> CalculateFinalResultsAsync();
}

public class GameResults
{
    public required List<PlayerResult> PlayerResults { get; init; }
}

public class PlayerResult
{
    public required string Id { get; init; }
    public required string ConnectionId { get; init; }
    public required string Username { get; init; }
    public int FinalScore { get; set; }
} 