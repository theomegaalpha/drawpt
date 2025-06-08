using DrawPT.Common.Models.Game;
namespace DrawPT.Common.Interfaces
{
    public interface IAssessmentService
    {
        Task<List<PlayerAnswer>> AssessAnswersAsync(string originalPrompt, List<PlayerAnswer> answers);
    }
}
