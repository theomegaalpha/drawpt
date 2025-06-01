using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IRoundOrchestrator
{
    Task<Theme> SelectThemeAsync(PlayerState selector);
    Task<GameQuestion> GenerateQuestionAsync(Theme theme);
    Task<List<PlayerAnswer>> CollectAnswersAsync(GameQuestion question);
    Task<List<PlayerAnswer>> AssessAnswersAsync(GameQuestion question, List<PlayerAnswer> answers);
    Task<bool> IsRoundCompleteAsync(GameRound round);
} 