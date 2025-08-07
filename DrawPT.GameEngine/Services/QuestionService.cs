using DrawPT.Common.Interfaces;
using DrawPT.Common.Models.Game;
using DrawPT.Data.Repositories;
using DrawPT.Data.Repositories.Game;
using DrawPT.GameEngine.Interfaces;

namespace DrawPT.GameEngine.Services
{
    public class QuestionService : IQuestionService
    {
        IAIService _aiService;
        GameEntitiesRepository _gameEntitiesRepository;

        public QuestionService(IAIService aiService, GameEntitiesRepository gameEntitiesRepository)
        {
            _aiService = aiService;
            _gameEntitiesRepository = gameEntitiesRepository;
        }

        public async Task<GameQuestion> GenerateQuestionAsync(string theme)
        {
            var question = await _aiService.GenerateGameQuestionAsync(theme);

            if (!string.IsNullOrEmpty(question.ImageUrl))
            {
                await _gameEntitiesRepository.SaveArchivedQuestion(new ArchivedQuestionEntity
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = question.ImageUrl,
                    OriginalPrompt = question.OriginalPrompt,
                    Theme = question.Theme
                });
                return question;
            }

            var archivedQuestion = _gameEntitiesRepository.GetRandomArchivedQuestion(theme);

            question.ImageUrl = archivedQuestion?.ImageUrl;
            question.OriginalPrompt = archivedQuestion?.OriginalPrompt;
            question.Theme = archivedQuestion?.Theme;
            return question;
        }

        public async Task<GameQuestion> GenerateQuestionFromPromptAsync(PlayerPrompt prompt)
        {
            var question = await _aiService.GenerateGameQuestionFromPromptAsync(prompt.Prompt);
            question.PlayerId = prompt.PlayerId;

            if (!string.IsNullOrEmpty(question.ImageUrl))
            {
                question.PlayerGenerated = true;
                return question;
            }

            var archivedQuestion = _gameEntitiesRepository.GetRandomArchivedQuestion(prompt.Prompt);

            question.ImageUrl = archivedQuestion?.ImageUrl;
            question.OriginalPrompt = archivedQuestion?.OriginalPrompt;
            question.Theme = archivedQuestion?.Theme;
            return question;
        }
    }
}
