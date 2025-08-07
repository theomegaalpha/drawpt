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
        private const string _imageEnhancePrompt = @"  Use a painterly style image with visible brushstrokes and a slightly textured surface, utilizing a muted pastel color palette of soft blues, light grays, and pale yellow with soft, diffused lighting.";
        private const string _assessmentPrompt = @"You are an AI game moderator responsible for evaluating contestant guesses against a given original phrase in a gameshow setting. Your primary task is to provide consistent similarity assessments, awarding scores between 0 and 100 based on linguistic, semantic, and contextual resemblance.

🎯 Evaluation Criteria:

(100 points) Exact Match:  The guessed phrase is identical to the original—includes all elements, words, and structure precisely. Rare but possible.
(90–99 points) Very Strong Match:  The guess mirrors the original in tone and core meaning, with nearly all key elements retained and high lexical overlap. Minor omissions only.
(75–89 points) Strong Match:  The guess expresses the same emotional tone and central ideas, but may rephrase or omit one minor component. Essential concepts still present.
(60–74 points) Moderate Match:  The guess preserves general themes and partial sentiment, but lacks multiple key elements or introduces mild distortion.
(40–59 points) Partial Overlap:  The guess reflects some relevant ideas but includes significant omissions or semantic drift. Emotional tone may diverge.
(25–39 points) Weak Connection:  The guess includes loosely related concepts with noticeable element loss and inconsistent tone or meaning.
(1–24 points) Very Weak Connection:  The guess contains minimal resemblance—perhaps one surviving concept or emotion—but lacks completeness or cohesion.
(0 points) Unrelated:  No discernible connection in wording, sentiment, or concept. Completely off-target.

🧾 Strict Response Format
Return results as a JSON array where each contestant’s data follows this structure:

json
[
    {
        'Id': '<unique identifier>',
        'PlayerId': '<unique identifier of original Player ID>',
        'ConnectionId': '<unique identifier of original Connection ID>',
        'Username': '<contestant\'s username>',
        'Avatar': '<contestant\'s avatar URL>',
        'Guess': '<contestant\'s guessed phrase>',
        'Score': <integer from 0 to 100>,
        'Reason': '<explanation for the given score>',
        'IsGambling': <boolean>,
        'BonusPoints': <integer from 0 to 25 of original scoring>,
        'SubmittedAt': '<string>'
    }
]
The response must always be a JSON array, even when evaluating a single contestant.Ensure explanations(Reason) are concise but clearly justify the score using linguistic and semantic reasoning.";

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
                            a.Reason = "Problem assessing scores.";
                            a.Score = 100;
                        });
                    }
                    var jsonResponse = completion.Content[0].Text.ToString() ?? "{}";
                    var playerAnswers = JsonSerializer.Deserialize<List<PlayerAnswer>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (playerAnswers == null)
                    {
                        answers.ForEach(a =>
                        {
                            a.Reason = "Problem assessing scores.";
                            a.Score = 100;
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
                a.Reason = "Problem assessing scores.";
                a.Score = 100;
            });
            return answers;
        }

        public async Task<GameQuestion> GenerateGameQuestionAsync(string theme)
        {
            var prompt = await GenerateImagePromptAsync(theme);
            var imageUrl = string.IsNullOrEmpty(prompt) ? string.Empty : await GenerateImageAsync(prompt, theme);
            return new GameQuestion()
            {
                OriginalPrompt = prompt.Split(']')[^1],
                Theme = theme,
                ImageUrl = imageUrl ?? string.Empty
            };
        }

        public async Task<GameQuestion> GenerateGameQuestionFromPromptAsync(string prompt)
        {
            var imageUrl = string.IsNullOrEmpty(prompt.Trim()) ? string.Empty : await GenerateImageAsync(prompt, "Player Generated");
            return new GameQuestion()
            {
                OriginalPrompt = prompt,
                Theme = "Player Generated",
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
                    return completion?.Content[0].Text.ToString() ?? string.Empty;
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
            return string.Empty;
        }

        // create a GenerateImageAsync method here
        private async Task<string?> GenerateImageAsync(string prompt, string theme)
        {
            return await GenerateImageRestfullyAsync(prompt, theme);

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

        private async Task<string?> GenerateImageRestfullyAsync(string prompt, string theme)
        {
            return await _freepikImageService.GenerateAndSaveImageAsync(prompt, theme);
        }
    }
}
