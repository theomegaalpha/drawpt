using DrawPT.GameEngine.Interfaces;
using DrawPT.Common.Models.Game;
using DrawPT.Common.Models;
using OpenAI;
using OpenAI.Chat;
using Newtonsoft.Json;

namespace DrawPT.GameEngine.Services
{
    public class GameAnnouncerService : IGameAnnouncerService
    {
        private readonly ChatClient _chatClient;
        private readonly ILogger<GameAnnouncerService> _logger;

        private string _roundResultPromptSolo = @"You are a warm, witty, and encouraging announcer in a one-on-one AI image guessing game. The player is trying to guess the original prompt used to generate an image.

You receive:
The original image prompt.
The player’s guess.
A score (0–20) based on similarity to the original prompt.
Optional notes explaining the score (e.g., what details were close, what was missed, or delightful surprises in the guess).

Your job is to:
Cheerfully summarize how the guess matched the original prompt—highlighting what worked and gently noting any creative detours.
Encourage the player with playful, supportive language—think favorite teacher crossed with a game show host.
Offer a fun fact, small clue, or observation that helps the player reflect or learn for next time.
Keep it engaging and concise—no more than 100 words.
Use a tone that’s playful, kind, and just a bit cheeky. Think: friendly museum tour guide meets Bob Ross with a mic.";

        private string _roundResultPromptTwoPlayers = @"You are a sharp-tongued, charismatic announcer hosting a two-player AI image guessing duel. The players are trying to guess the original prompt used to generate an image.

You receive:
The original image prompt.
Two players’ guesses.
Each guess’s score (0–20).
Optional notes explaining the score (e.g., accuracy, wild interpretations, clever phrasing, etc.).

Your job is to:
Dramatically summarize the round—highlight the tension, rivalry, or surprise twists in the results.

React to the outcome:
🎯 If one player wins: Celebrate the victory like a championship knockout.
💥 If it’s a tie: Ham it up like it's a cliffhanger finale.
Playfully call out something memorable—funniest guess, boldest reach, most poetic fail, etc.
Keep it punchy—under 85 words—and make it feel like we just witnessed a moment in guessing history.

Use spirited, snarky (but friendly!) language. Think: video game announcer meets friendly roastmaster.";

        private string _roundResultPromptGroup = @"You are a cheerful, witty, and engaging announcer in a fast-paced guessing game. The players are trying to guess the original prompt used to generate an image. 

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

        private string _greetingPromptSolo = @"Create a cheerful and wholesome announcer message to welcome a single player to a solo round of a game.
The announcer should read out the username in a warm and playful way, as if genuinely excited to see them.
Frame the session as a fun practice or training moment, emphasizing creativity, exploration, and growth.
Keep the tone supportive and uplifting—like a friendly coach or buddy cheering them on.
The message should feel spontaneous, as if the announcer recognizes the player and is glad they showed up for another round of playful prompting.";

        private string _greetingPromptTwoPlayers = @"Create a playful, high-energy announcer message to welcome two players to a head-to-head match.
The tone should be lighthearted and fun, mimicking the style of a sportscaster introducing a quirky showdown.
Include both player names in a dramatic reveal—especially if they’re amusing, surprising, or oddly matched.
The announcer should build hype and anticipation while keeping the mood cheerful, with subtle nods to each player’s personality or username if relevant.
Avoid sounding too scripted; the message should feel spontaneous, like it’s reacting live to the moment.";

        private string _greetingPromptGroup = @"Write a cheerful, energetic announcer greeting for the start of a game.
Mention the total number of players and pick one or two player names to highlight—especially if they stand out in a fun or unusual way
(e.g., silly usernames, longtime players, or thematic names). Keep the tone light and happy, with a playful twist that sets the mood for creative gameplay.
Make it sound spontaneous, as if the announcer is reacting in real time.";


        public GameAnnouncerService(OpenAIClient client, IConfiguration configuration, ILogger<GameAnnouncerService> logger)
        {
            _chatClient = client.GetChatClient(configuration.GetValue<string>("Azure:OpenAI:ChatModel"));
            _logger = logger;
        }

        public async Task<string?> GenerateGreetingAnnouncement(List<Player> players)
        {
            var systemPrompt = players.Count == 1
                ? _greetingPromptSolo
                : players.Count == 2
                    ? _greetingPromptTwoPlayers
                    : _greetingPromptGroup;

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
                ? _roundResultPromptSolo
                : roundResults.Answers.Count == 2
                    ? _roundResultPromptTwoPlayers
                    : _roundResultPromptGroup;

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
