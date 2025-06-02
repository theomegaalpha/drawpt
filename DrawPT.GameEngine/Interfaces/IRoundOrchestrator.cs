using DrawPT.Common.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IRoundOrchestrator
{
    Task<GameTheme> SelectThemeAsync(string playerConnectionId);
    Task<GameQuestion> GenerateQuestionAsync(GameTheme theme);
    Task<List<PlayerAnswer>> CollectAnswersAsync(GameQuestion question);
    Task<List<PlayerAnswer>> AssessAnswersAsync(GameQuestion question, List<PlayerAnswer> answers);
    Task<bool> IsRoundCompleteAsync(GameRound round);
} 