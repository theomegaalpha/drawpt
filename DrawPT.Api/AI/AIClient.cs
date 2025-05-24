using System.Text;
using System.Text.Json;
using DrawPT.Api.Models;

namespace DrawPT.Api.AI
{
    public class AIClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _openAITextEndpoint;
        private readonly string _openAIImageEndpoint;
        private readonly string _openApiKey;

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

        public AIClient(IConfiguration configuration)
        {
            _openAITextEndpoint = configuration["Azure:OpenAI:TextEndpoint"];
            _openAIImageEndpoint = configuration["Azure:OpenAI:ImageEndpoint"];
            _openApiKey = configuration["Azure:OpenAI:ApiKey"];
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openApiKey}");
        }

        public async Task<string> GenerateAssessmentAsync(string originalPrompt, List<GameAnswer> answers)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var stringAnswers = JsonSerializer.Serialize(answers);
            var requestData = new
            {
                model = "gpt-4o",
                messages = new Message[]
                {
                    new Message{ Role = "system", Content = _assessmentPrompt + originalPrompt },
                    new Message{ Role = "user", Content = stringAnswers }
                },
                max_tokens = 1000
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData, options), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_openAITextEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OpenAICompletionResponse>(responseContent, options);
                return result.Choices[0].Message.Content;
            }
            else
            {
                throw new Exception($"Failed to generate text completion. Status code: {response.StatusCode}");
            }
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
            var requestData = new
            {
                model = "gpt-4o",
                messages = new[]
                {
                    new { role = "system", content = _imagePrompt },
                    new { role = "user", content = theme }
                },
                max_tokens = 100
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_openAITextEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var result = JsonSerializer.Deserialize<OpenAICompletionResponse>(responseContent, options);
                return result.Choices[0].Message.Content;
            }
            else
            {
                throw new Exception($"Failed to generate text completion. Status code: {response.StatusCode}");
            }
        }

        // create a GenerateImageAsync method here
        private async Task<string> GenerateImageAsync(string prompt)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var requestData = new OpenAIImageRequest()
            {
                Model = "dall-e-3",
                Prompt = prompt,
                N = 1,
                Size = "1024x1024"
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData, options), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_openAIImageEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OpenAIImageResponse>(responseContent, options);
                return result.Data[0].Url;
            }
            else
            {
                throw new Exception($"Failed to generate image. Status code: {response.StatusCode}");
            }
        }
    }
}