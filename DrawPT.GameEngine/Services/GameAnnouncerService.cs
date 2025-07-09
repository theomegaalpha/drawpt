using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;
using DrawPT.Data.Repositories;
using DrawPT.Data.Repositories.Reference;
using DrawPT.GameEngine.Interfaces;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;

namespace DrawPT.GameEngine.Services
{
    public class GameAnnouncerService : IGameAnnouncerService
    {
        private readonly ChatClient _chatClient;
        private readonly ILogger<GameAnnouncerService> _logger;
        private readonly ReferenceRepository _referenceRepository;


        public GameAnnouncerService(OpenAIClient client, IConfiguration configuration,
            ReferenceRepository referenceRepository, ILogger<GameAnnouncerService> logger)
        {
            _chatClient = client.GetChatClient(configuration.GetValue<string>("Azure:OpenAI:ChatModel"));
            _referenceRepository = referenceRepository;
            _logger = logger;
        }

        public async Task<string?> GenerateGreetingAnnouncement(List<Player> players)
        {
            var systemPrompt = players.Count == 1
                ? _referenceRepository.GetAnnouncerPrompt(AnnouncerPromptKeys.GreetingSolo)
                : players.Count == 2
                    ? _referenceRepository.GetAnnouncerPrompt(AnnouncerPromptKeys.GreetingTwoPlayers)
                    : _referenceRepository.GetAnnouncerPrompt(AnnouncerPromptKeys.GreetingGroup);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage($"players: {JsonConvert.SerializeObject(players.Select(a => new { a.Username }))}")
            };

            try
            {
                var options = new ChatCompletionOptions
                {
                    Temperature = 1,
                    MaxOutputTokenCount = 800,

                    TopP = 1,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0
                };

                ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);

                // Print the response
                if (completion != null)
                {
                    if (completion.Content.Count == 0)
                    {
                        _logger.LogWarning($"No announcer message was produced for {players}.");
                        return null;
                    }
                    return completion?.Content[0].Text.ToString() ?? "";
                }
                else
                {
                    _logger.LogWarning($"No response received from the announcer for {players}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
            }
            return null;
        }

        public async Task<string?> GenerateRoundResultAnnouncement(string originalPrompt, RoundResults roundResults)
        {
            var systemPrompt = roundResults.Answers.Count == 1
                ? _referenceRepository.GetAnnouncerPrompt(AnnouncerPromptKeys.RoundResultSolo)
                : roundResults.Answers.Count == 2
                    ? _referenceRepository.GetAnnouncerPrompt(AnnouncerPromptKeys.RoundResultTwoPlayers)
                    : _referenceRepository.GetAnnouncerPrompt(AnnouncerPromptKeys.RoundResultGroup);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage($"original prompt: {originalPrompt}\nresults: {JsonConvert.SerializeObject(roundResults.Answers.Select(a => new { a.Username, a.Guess, Points = a.Score + a.BonusPoints }))}")
            };

            try
            {
                var options = new ChatCompletionOptions
                {
                    Temperature = 1,
                    MaxOutputTokenCount = 800,

                    TopP = 1,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0
                };

                ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);

                // Print the response
                if (completion != null)
                {
                    if (completion.Content.Count == 0)
                    {
                        _logger.LogWarning($"No announcer message was produced for {roundResults.Answers}.");
                        return null;
                    }
                    return completion?.Content[0].Text.ToString() ?? "";
                }
                else
                {
                    _logger.LogWarning($"No response received from the announcer for {roundResults.Answers}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
            }
            return null;
        }

        public string GenerateGameResultAnnouncement()
        {
            return "blegh";
        }
    }
}
