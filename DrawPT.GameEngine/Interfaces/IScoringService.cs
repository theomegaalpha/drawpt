using DrawPT.Common.Models.Game;

namespace DrawPT.GameEngine.Interfaces;

public interface IScoringService
{
    Task<int> CalculateRoundScoreAsync(PlayerAnswer answer, PlayerAnswer assessment);
    Task<int> CalculateBonusPointsAsync(PlayerAnswer answer);
    Task<int> CalculateFinalResultsAsync();
}