using DrawPT.Common.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IAIClient
{
    Task<GameQuestion> GenerateGameQuestionAsync(string theme);
    Task<List<PlayerAnswer>> AssessAnswersAsync(string originalPrompt, List<PlayerAnswer> answers);
} 