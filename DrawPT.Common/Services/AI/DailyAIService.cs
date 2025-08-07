using System.Text.Json;
using DrawPT.Common.Models.Daily;
using DrawPT.Common.Models.Game;
using DrawPT.Common.Util;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace DrawPT.Common.Services.AI
{
    public class DailyAIService
    {
        private readonly ChatClient _chatClient;
        private readonly FreepikMysticService _freepikImageService;

        string _imagePrompt = @"You are a highly imaginative, visually oriented AI assistant designed to generate striking, beautiful image prompts for an AI-powered digital art game.
Your task is to craft concise, highly descriptive prompts that drive the generation of visually stunning yet recognizable images, tailored to the given theme.
Focus on vivid visual elements, textures, compositions, and lighting that enhance the appeal of the artwork.
Ensure clear imagery, avoiding overly abstract or ambiguous concepts that may be difficult to depict accurately.
Maintain engagement and variety in each response while aligning to the provided theme.

Response Format:
(Use this format without curly braces) [{Theme}] {Creative image prompt}";
        private const string _assessmentPrompt = @"You are an AI game moderator responsible for evaluating contestant guesses against a given original phrase in a gameshow setting. Your primary task is to provide consistent and precise similarity assessments, awarding scores between 0 and 100 based on linguistic, semantic, and contextual resemblance.
Additionally, you must generate a closeness array, a 10-integer heatmap ranging from 0 to 9, representing how well different portions of the guessed phrase match the original prompt.

Scoring Guidelines
This scoring system heavily penalizes incomplete guesses, ensuring accuracy and fairness in rating attempts.
100 (Exact Match) – Word-for-word identical phrase.
90–99 (Near Exact Match) – Slight rewording but captures ALL core details.
80–89 (High Similarity) – Strong rephrasing but retains the full meaning and details.
70–79 (Moderate Similarity) – All major components present but phrasing is altered.
50–69 (Loose Similarity) – Recognizable but missing multiple key elements.
25–49 (Weak Similarity) – Guess touches on a single aspect while ignoring full context.
0–24 (Unrelated) – Guess is too vague or completely incorrect.
Penalty for Missing Key Details
The more context lost, the greater the deduction.
Weight prioritization: Subject + Scene + Action > Single Noun Identification

Closeness Array Rules
Generate an array of 10 integers (0–9) indicating word-by-word similarity.
The highest scores should align with words that strongly match, even if rephrased (e.g., synonyms like ""cat"" vs. ""feline"").
Lower scores indicate words with minimal relevance or contradictory meaning.
The array reflects sentence position, ensuring early and late-word variations are captured separately.
Example:
Original Prompt: ""A radiant idol singer on a dazzling stage..."" Guess: ""Singer"" Expected Score: 30 (captures only the profession, missing pose, costume, lighting, audience, and scene) Expected Array: [9,0,0,0,0,0,0,0,0,0] (only the first word matches, rest are completely absent)

Strict Response Format
Return results as a JSON object with the following structure:
json
{
    ""Id"": ""<unique identifier>"",
    ""QuestionId"": ""<unique identifier of original Question ID>"",
    ""PlayerId"": ""<unique identifier of original Player ID>"",
    ""Username"": ""<contestant's username>"",
    ""Date"": ""<ISO 8601 string>"",
    ""Guess"": ""<contestant's guessed phrase>"",
    ""OriginalPrompt"": ""<the original image prompt>"",
    ""Reason"": ""<justification for assigned score>"",
    ""Score"": <integer from 0 to 100>,
    ""ClosenessArray"": [<10 integers from 0 to 9 representing semantic accuracy>],
    ""CreatedAt"": ""<ISO 8601 string>""
}
Guidelines for Consistency
""Reason"" must concisely justify the score using specific linguistic and semantic comparisons without revealing the original prompt.
""Reason"" should include details on how the guess aligns with the original prompt.
""Reason"" should only very briefly mention where guess deviates from original prompt.
""Reason"" should not mention any aspects of the original prompt not captured in the guess.
The closeness array must align with sentence structure, capturing semantic similarity at each word position.
Ensure the response always follows the structured JSON format.
Penalty for Missing Key Details
The more context lost, the greater the deduction.
Weight prioritization: Subject + Scene + Action > Single Noun Identification";

        public DailyAIService(OpenAIClient aiClient, FreepikMysticService freepikImageService, IConfiguration configuration)
        {
            _chatClient = aiClient.GetChatClient(configuration.GetValue<string>("Azure:OpenAI:ChatModel"));
            _freepikImageService = freepikImageService;
        }

        public async Task<DailyAnswerPublic?> AssessAnswerAsync(string originalPrompt, DailyAnswerPublic answer)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(_assessmentPrompt + originalPrompt),
                new UserChatMessage(JsonSerializer.Serialize(answer))
            };

            try
            {
                var options = new ChatCompletionOptions
                {
                    Temperature = 1,

                    TopP = 1,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0
                };

                ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);

                // Print the response
                if (completion != null)
                {
                    if (completion.Content.Count == 0)
                        return null;

                    var jsonResponse = completion.Content[0].Text.ToString() ?? "{}";
                    return JsonSerializer.Deserialize<DailyAnswerPublic>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return null;
        }

        public async Task<GameQuestion> GenerateGameQuestionAsync(string theme, DateTime? date = null)
        {
            var prompt = await GenerateImagePromptAsync(theme);
            var imageUrl = await GenerateImageAsync(prompt, date ?? TimezoneHelper.Now());
            return new GameQuestion()
            {
                OriginalPrompt = prompt.Split(']')[^1],
                Theme = theme,
                ImageUrl = imageUrl ?? string.Empty
            };
        }

        private async Task<string> GenerateImagePromptAsync(string theme)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(_imagePrompt),
                new UserChatMessage($"[{theme}]")
            };

            try
            {
                var options = new ChatCompletionOptions
                {
                    Temperature = 1,

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
                        Console.WriteLine("No content in response.");
                        return string.Empty;
                    }
                    return completion?.Content[0].Text.ToString() ?? "";
                }
                else
                {
                    Console.WriteLine("No response received.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return "false";
        }

        // create a GenerateImageAsync method here
        private async Task<string?> GenerateImageAsync(string prompt, DateTime date)
        {
            return await GenerateImageRestfullyAsync(prompt, date);
        }

        private async Task<string?> GenerateImageRestfullyAsync(string prompt, DateTime date)
        {
            return await _freepikImageService.GenerateAndSaveImageAsync(prompt, date);
        }
    }
}
