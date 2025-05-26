using DrawPT.Data.Models;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Images;
using System.ClientModel;
using System.Text.Json;

namespace DrawPT.Api.AI
{
    public class AIClient
    {
        private readonly ChatClient _chatClient;
        private readonly ImageClient _imageClient;
        private readonly GeminiImageGenerator _geminiImageGenerator;

        string _imagePrompt = @"You are a very creative player of a game that's trying to use AI to generate a digital art picture.
            Given a theme, please generate a short and unique prompt about anything you want that would produce a cool picture.
            Response format excluding curcly braces:  '[{Theme goes here}] {Response goes here}'
            Keep your response short while keeping the creativity and aligned to the theme provided.";
        private const string _assessmentPrompt = @"You are the AI that a gameshow host uses to assess similarities between two phrases.
            The gameshow host has a phrase that contestants have to guess based on a picture.
            You have to score the contestant's phrase based on how similar it is to the host's phrase and give a score between 0 and 20.
            Response format should be in a JSON array with each contestant's
            {PlayerConnectionId: PlayerConnectionId, Score: Score, BonusPoints: bonusPoints, Guess: phrase, Reason: reason}
            and reason for the score.
            Ensure the returned result is an array of JSON objects even if only one answer is provided.
            Now, the original phrase is: ";

        public AIClient(OpenAIClient aiClient, GeminiImageGenerator geminiImageGenerator, IConfiguration configuration)
        {
            _chatClient = aiClient.GetChatClient(configuration.GetValue<string>("Azure:OpenAI:ChatModel"));
            _imageClient = aiClient.GetImageClient(configuration.GetValue<string>("Azure:OpenAI:ImageModel"));
            _geminiImageGenerator = geminiImageGenerator;
        }

        public async Task<string> GenerateAssessmentAsync(string originalPrompt, List<GameAnswer> answers)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(_assessmentPrompt + originalPrompt),
                new UserChatMessage(JsonSerializer.Serialize(answers))
            };

            try
            {
                var options = new ChatCompletionOptions
                {
                    Temperature = (float)1,
                    MaxOutputTokenCount = 800,

                    TopP = (float)1,
                    FrequencyPenalty = (float)0,
                    PresencePenalty = (float)0
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

        public async Task<GameQuestion> GenerateGameQuestionAsync(string theme)
        {
            var prompt = await GenerateImagePromptAsync(theme);
            var imageUrl = await GenerateImageAsync(prompt);
            return new GameQuestion()
            {
                OriginalPrompt = prompt.Split(']')[1],
                ImageUrl = imageUrl
            };
        }

        public async Task<GameQuestion> GenerateFakeGameQuestionAsync(string _)
        {
            await Task.Delay(8000);
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
                new UserChatMessage(theme)
            };

            try
            {
                var options = new ChatCompletionOptions
                {
                    Temperature = (float)1,
                    MaxOutputTokenCount = 800,

                    TopP = (float)1,
                    FrequencyPenalty = (float)0,
                    PresencePenalty = (float)0
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
        private async Task<string> GenerateImageAsync(string prompt)
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

        private async Task<string> GenerateImageRestfullyAsync(string prompt)
        {
            await _geminiImageGenerator.GenerateAndSaveImageAsync(prompt);
            return "https://assets-global.website-files.com/632ac1a36830f75c7e5b16f0/64f112667271fdad06396cdb_QDhk9GJWfYfchRCbp8kTMay1FxyeMGxzHkB7IMd3Cfo.webp";
        }
    }
}