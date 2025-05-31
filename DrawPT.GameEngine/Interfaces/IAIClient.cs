using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IAIClient
{
    Task<GameQuestion> GenerateGameQuestionAsync(string theme);
    Task<string> GenerateAssessmentAsync(string originalPrompt, List<GameAnswer> answers);
} 