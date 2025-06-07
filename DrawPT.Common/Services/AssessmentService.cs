using DrawPT.Common.Interfaces;
using DrawPT.Common.Models.Game;

namespace DrawPT.Common.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly IAIService _aiService;
        public AssessmentService(IAIService aiService)
        {
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        }

        /// <summary>
        /// Assesses the player's answer against the original prompt.
        /// </summary>
        /// <param name="answer">The player's answer to assess.</param>
        /// <returns>A task that represents the asynchronous operation, containing the assessed player answer.</returns>
        public async Task<PlayerAnswer> AssessAnswerAsync(string originalPrompt, PlayerAnswer answer)
        {
            if (answer == null)
            {
                throw new ArgumentNullException(nameof(answer));
            }
            // Call the AI service to assess the answer
            return await _aiService.AssessAnswerAsync(originalPrompt, answer);
        }
    }
}
