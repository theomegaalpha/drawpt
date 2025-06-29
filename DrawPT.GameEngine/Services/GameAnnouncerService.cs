using DrawPT.GameEngine.Interfaces;
using DrawPT.Common.Models.Game;
using OpenAI;
using OpenAI.Chat;

namespace DrawPT.GameEngine.Services
{
    public class GameAnnouncerService : IGameAnnouncerService
    {
        private readonly ChatClient _chatClient;
        private readonly ILogger<GameAnnouncerService> _logger;

        private string _roundResultPrompt = @"You are a cheerful, witty, and engaging announcer in a fast-paced guessing game. The players are trying to guess the original prompt used to generate an image. 

You receive:
- The original image prompt.
- A list of players' guesses.
- Each guess’s score (0–20) based on similarity to the original prompt.
- Optional notes with reasoning for the scores (e.g., partial accuracy, creative deviation, etc.).

Your job is to:
1. Playfully summarize the round—mention how close or wild the guesses were overall.
2. Highlight and react to any ONE of the following (if present):
   - 🎯 Highest score: Celebrate the win with fun, over-the-top enthusiasm.
   - ❄️ Lowest score: Lightly tease the lowest scorer without sounding mean-spirited.
   - 🤣 Funniest guess: Shine a spotlight on any absurd or laugh-out-loud submission.
3. Keep it punchy—your announcement should be under 75 words and feel like a TV game show host or sports commentator.

Use friendly, humorous language with lots of personality. Be energetic but concise. Think: “Whose Line is it Anyway?” meets “Mario Party” announcer.
";

        public GameAnnouncerService(OpenAIClient client, IConfiguration configuration, ILogger<GameAnnouncerService> logger)
        {
            _chatClient = client.GetChatClient(configuration.GetValue<string>("Azure:OpenAI:ChatModel"));
            _logger = logger;
        }

        public async Task<string?> GenerateRoundResultAnnouncement(string originalPrompt, RoundResults roundResults)
        {

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(_roundResultPrompt),
                new UserChatMessage($"original prompt: {originalPrompt}\nresults: {roundResults.Answers.Select(a => a.Guess).ToString()}")
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
