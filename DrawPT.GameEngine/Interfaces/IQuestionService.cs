using DrawPT.Common.Models.Game;

namespace DrawPT.GameEngine.Interfaces;

public interface IQuestionService
{
    Task<GameQuestion> GenerateQuestionAsync(string theme);
}