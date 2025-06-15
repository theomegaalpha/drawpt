using DrawPT.Common.Interfaces;
using DrawPT.Common.Models.Game;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Images;
using System.ClientModel;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace DrawPT.Common.Services.AI
{
    public class AIService : IAIService
    {
        private readonly ChatClient _chatClient;
        private readonly ImageClient _imageClient;
        private readonly FreepikFastService _freepikImageService;

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

        public AIService(OpenAIClient aiClient, FreepikFastService freepikImageService, IConfiguration configuration)
        {
            _chatClient = aiClient.GetChatClient(configuration.GetValue<string>("Azure:OpenAI:ChatModel"));
            _imageClient = aiClient.GetImageClient(configuration.GetValue<string>("Azure:OpenAI:ImageModel"));
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
                        answers.ForEach(a => {
                            a.Reason = "Problem assing scores.";
                            a.Score = 20;
                        });
                    }
                    var jsonResponse = completion.Content[0].Text.ToString() ?? "{}";
                    var playerAnswers = JsonSerializer.Deserialize<List<PlayerAnswer>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (playerAnswers == null)
                    {
                        answers.ForEach(a => {
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
            answers.ForEach(a => {
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
                Theme = theme,
                ImageUrl = imageUrl ?? string.Empty
            };
        }

        public async Task<GameQuestion> GenerateFakeGameQuestionAsync(string _)
        {
            await Task.Delay(3000);
            string[] prompts = [
                "black and white digital illustration inspired by Dora Art, featuring a tattooed female figure with a sense of reflective calmness.",
                "Dragon Ball Fighter, presented in the distinctive style of Thiago Valdi. Featuring a sleek combination of light black and silver tones.",
                "A single girl in a vibrant, shiny dress standing gracefully in a neon-infused rainstorm. The cyberpunk-inspired scene should emphasize her captivating beauty.",
                "An enigmatic Call of Duty - Blacklist Games character, ironically captivating Mosscore adventure amidst ethereal gossamer fabrics.",
                "An evil, cyborg Terminator robot with hyper-realistic attention to detail.",
                "A nightmarish man enveloped in a blend of pink and black hues. His backlit form casts a mysterious silhouette, while intricate details amplify the horror.",
                "An ultra-detailed and ultra-realistic depiction of Iron-Man in a venomized form, ensuring an unparalleled level of intricacy, and realism.",
                "Nier Automata's 2B portrayed in a watercolor masterpiece by Greg Rutkowski, capturing sharp focus in a stunning studio photograph."
                ];
            string[] imageUrls = [
                "https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f112667271fdad06396cdb_QDhk9GJWfYfchRCbp8kTMay1FxyeMGxzHkB7IMd3Cfo.webp",
                "https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f1168c6d00b9a61edd8a3a_CxCAdRB8SeRB6KI1hcODSrz2rZg34Zpcc2_KGyUm-lg.webp",
                "https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f113ae7271fdad063aa84e_OV0Ei7Ncv6fgW7BlI_gt-QhF7qwtwCjVNGMwBNEFcM8.webp",
                "https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f201291226b683660aeaca_xLH48FdbWdRElTt--1X3hiA54tITt5XJNfA4v5bpG0w.webp",
                "https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f1169fa1e7c6f077ef00b2_R5p9PVQZa_svZ8TG6TLQp8SKtwNrTlVxrh12oJ9y4ms.webp",
                "https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f112f4e551d643fe3acdb9_2Qo1RFXaSiDysKaWsEGNmMn5lYPhoSV9gdrAm-51_1s.webp",
                "https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f1ff2c15d25940b0da7133_5hme_hy0Pf5zbZ1PahcnWIqNAsHj95232tQrFEkEdfk.webp",
                "https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f115b0b802ee4960a3da9f_4PrnPJDEMZkZm9djp21r1YPHRyIN-bRf2ncVUjssRDI.webp"
                ];
            var random = new Random();
            var index = random.Next(prompts.Length);
            return new GameQuestion()
            {
                OriginalPrompt = prompts[index],
                ImageUrl = imageUrls[index]
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

            ClientResult<GeneratedImage> imageResult = await _imageClient.GenerateImageAsync(prompt, new()
            {
                Quality = GeneratedImageQuality.Standard,
                Size = GeneratedImageSize.W1024xH1024,
                Style = GeneratedImageStyle.Vivid,
                ResponseFormat = GeneratedImageFormat.Uri
            });

            GeneratedImage image = imageResult.Value;
            Console.WriteLine($"Image URL: {image.ImageUri}");
            return image.ImageUri.ToString() ?? string.Empty;
        }

        private async Task<string?> GenerateImageRestfullyAsync(string prompt)
        {
            return await _freepikImageService.GenerateAndSaveImageAsync(prompt);
        }
    }
}
