using DrawPT.Common.Interfaces;
using DrawPT.Common.Models.Game;
using OpenAI;
using OpenAI.Chat;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace DrawPT.Common.Services.AI
{
    public class DailyAIService : IAIService
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
        private const string _assessmentPrompt = @"You are an AI game moderator responsible for evaluating contestant guesses against a given original phrase in a gameshow setting. Your primary task is to provide consistent similarity assessments, awarding scores between 0 and 20 based on linguistic, semantic, and contextual resemblance.
Evaluation Criteria:
Exact Match (20 Points): The guessed phrase matches the original exactly—rare but possible.
Very Strong Conceptual & Sentiment Match (15-19 Points): The guess conveys the same emotional tone and core meaning, matching on some words.
Strong Conceptual & Sentiment Match (11-14 Points): The guess conveys the same emotional tone and core meaning, even if the wording is different.
Moderate Conceptual Overlap (8-10 Points): The guess captures important aspects of the phrase—such as key themes, related emotions, or partial word matches—but deviates in structure.
Weak Connection (4-7 Points): The guess contains elements that vaguely relate to the original phrase but alters meaning significantly.
Very Weak Connection (1-3 Points): The guess contains trace elements that can arguably relate to the original phrase.
Unrelated (0 Points): The guess has no meaningful connection in words, sentiment, or concept.

Strict Response Format:
Return results as a JSON array where each contestant's data follows this structure:

json
[
    {
        'Id': '<unique identifier>',
        'PlayerId': '<unique identifier of original Player ID>',
        'ConnectionId': '<unique identifier of original Connection ID>',
        'Username': '<contestant's username>',
        'Guess': '<contestant's guessed phrase>',
        'Score': '<integer from 0 to 20>',
        'Reason': '<explanation for the given score>',
        'IsGambling': <boolean>,
        'BonusPoints': <integer from 0 to 5 of original scoring>,
        'SubmittedAt': <string>
    }
]
The response must always be a JSON array, even when evaluating a single contestant.
Ensure explanations ('Reason') are concise yet sufficiently justify the assigned score.
Original Phrase:
Now, the original phrase is:";

        public DailyAIService(OpenAIClient aiClient, FreepikMysticService freepikImageService, IConfiguration configuration)
        {
            _chatClient = aiClient.GetChatClient(configuration.GetValue<string>("Azure:OpenAI:ChatModel"));
            _freepikImageService = freepikImageService;
        }

        public async Task<List<PlayerAnswer>> AssessAnswerAsync(string originalPrompt, List<PlayerAnswer> answers)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(_assessmentPrompt + originalPrompt),
                new UserChatMessage(JsonSerializer.Serialize(answers))
            };

            if (answers.Count == 0)
                return answers;

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
                        answers.ForEach(a =>
                        {
                            a.Reason = "Problem assing scores.";
                            a.Score = 20;
                        });
                    }
                    var jsonResponse = completion.Content[0].Text.ToString() ?? "{}";
                    var playerAnswers = JsonSerializer.Deserialize<List<PlayerAnswer>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (playerAnswers == null)
                    {
                        answers.ForEach(a =>
                        {
                            a.Reason = "Problem assing scores.";
                            a.Score = 20;
                        });
                        return answers;
                    }
                    return playerAnswers;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            answers.ForEach(a =>
            {
                a.Reason = "Problem assing scores.";
                a.Score = 20;
            });
            return answers;
        }

        public async Task<GameQuestion> GenerateGameQuestionAsync(string theme)
        {
            var prompt = await GenerateImagePromptAsync(theme);
            var imageUrl = await GenerateImageAsync(prompt);
            return new GameQuestion()
            {
                OriginalPrompt = prompt.Split(']')[^1],
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
        private async Task<string?> GenerateImageAsync(string prompt)
        {
            return await GenerateImageRestfullyAsync(prompt);
        }

        private async Task<string?> GenerateImageRestfullyAsync(string prompt)
        {
            return await _freepikImageService.GenerateAndSaveImageAsync(prompt);
        }
    }
}