using DrawPT.GameEngine.Interfaces;
using DrawPT.Common.Models;

namespace DrawPT.GameEngine.Services;

public class ScoringService : IScoringService
{
    public Task<int> CalculateRoundScoreAsync(PlayerAnswer answer, PlayerAnswer assessment)
    {
        return Task.FromResult(1);
    }

    public Task<int> CalculateBonusPointsAsync(PlayerAnswer answer)
    {
        return Task.FromResult(1);
    }

    public Task<int> CalculateFinalResultsAsync()
    {
        return Task.FromResult(1);
    }
} 