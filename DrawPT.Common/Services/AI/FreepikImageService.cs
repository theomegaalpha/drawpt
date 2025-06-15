using DrawPT.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DrawPT.Common.Services.AI
{

    public class FreepikImageService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly string apiEndpoint;
        private readonly string apiKey;
        private readonly IStorageService _storageService;
        private readonly ILogger<FreepikImageService> _logger;

        public FreepikImageService(IConfiguration configuration, IStorageService storageService, ILogger<FreepikImageService> logger)
        {
            apiEndpoint = configuration.GetValue<string>("FreepikUrl") ?? throw new InvalidOperationException("Freepik API endpoint not configured.");
            apiKey = configuration.GetValue<string>("FreepikApiKey") ?? throw new InvalidOperationException("Freepik API key not configured.");
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<string?> GenerateAndSaveImageAsync(string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                throw new ArgumentNullException(nameof(prompt), "Prompt cannot be null or empty.");
            }

            var requestPayload = new FreepikImageRequestPayload
            {
                Prompt = prompt,
                NegativePrompt = "low quality, worst quality, normal quality, jpeg artifacts, ugly, duplicate, morbid, mutilated, extra fingers, fewer fingers, long neck, long body",
                GuidanceScale = 2,
                NumImages = 1,
                Image = new FreepikImageRequestPayload.ImageDetails
                {
                    Size = "traditional_3_4"
                },
                Styling = new FreepikImageRequestPayload.StylingDetails
                {
                    //Style = "anime",
                    Effects = new FreepikImageRequestPayload.EffectsDetails
                    {
                        Lightning = "cold"
                    },
                    Colors = new List<FreepikImageRequestPayload.ColorDetails>
                    {
                        new FreepikImageRequestPayload.ColorDetails { Color = "#91AEC2", Weight = 1 }
                    }
                },
                FilterNsfw = true
            };
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var payload = JsonSerializer.Serialize(requestPayload, options);


            try
            {
                // Remove Content-Type from default headers
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("x-freepik-api-key", apiKey);
                HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(apiEndpoint, httpContent);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    Console.WriteLine($"Response: {responseBody}");
                    return null;
                }

                var freepikResponse = JsonSerializer.Deserialize<FreepikImageResponse>(responseBody);

                string? base64Image = null; // Declare base64Image at the beginning of the method

                if (freepikResponse?.Data != null && freepikResponse.Data.Any())
                {
                    base64Image = freepikResponse.Data.FirstOrDefault()?.Base64;
                }

                if (string.IsNullOrEmpty(base64Image))
                {
                    Console.WriteLine("Could not find image data in the API response.");
                    Console.WriteLine($"Full response for debugging: {responseBody}");
                    return null;
                }

                byte[] imageBytes = Convert.FromBase64String(base64Image);
                string blobName = $"freepik-image-{Guid.NewGuid()}.png";
                string? imageUrl = await _storageService.SaveImageAsync(imageBytes, blobName);

                if (imageUrl != null)
                {
                    Console.WriteLine($"Image successfully generated and uploaded to: {imageUrl}");
                    return imageUrl;
                }
                else
                {
                    Console.WriteLine("Failed to upload image to blob storage.");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An unexpected error occurred: {e.Message}");
                return null;
            }
        }

        private class FreepikImageResponse
        {
            [JsonPropertyName("data")]
            public List<ImageData> Data { get; set; } = new List<ImageData>();

            [JsonPropertyName("meta")]
            public MetaData Meta { get; set; } = new MetaData();

            public class ImageData
            {
                [JsonPropertyName("base64")]
                public string Base64 { get; set; } = string.Empty;

                [JsonPropertyName("has_nsfw")]
                public bool HasNsfw { get; set; }
            }

            public class MetaData
            {
                [JsonPropertyName("image")]
                public ImageMeta Image { get; set; } = new ImageMeta();

                [JsonPropertyName("seed")]
                public int Seed { get; set; }

                [JsonPropertyName("guidance_scale")]
                public int GuidanceScale { get; set; }

                [JsonPropertyName("prompt")]
                public string Prompt { get; set; } = string.Empty;

                [JsonPropertyName("num_inference_steps")]
                public int NumInferenceSteps { get; set; }
            }

            public class ImageMeta
            {
                [JsonPropertyName("size")]
                public string Size { get; set; } = string.Empty;

                [JsonPropertyName("width")]
                public int Width { get; set; }

                [JsonPropertyName("height")]
                public int Height { get; set; }
            }
        }

        private class FreepikImageRequestPayload
        {
            [JsonPropertyName("prompt")]
            public string Prompt { get; set; } = string.Empty;

            [JsonPropertyName("negative_prompt")]
            public string? NegativePrompt { get; set; }

            [JsonPropertyName("guidance_scale")]
            public double GuidanceScale { get; set; }

            [JsonPropertyName("seed")]
            public int? Seed { get; set; }

            [JsonPropertyName("num_images")]
            public int NumImages { get; set; }

            [JsonPropertyName("image")]
            public ImageDetails Image { get; set; } = new ImageDetails();

            [JsonPropertyName("styling")]
            public StylingDetails Styling { get; set; } = new StylingDetails();

            [JsonPropertyName("filter_nsfw")]
            public bool FilterNsfw { get; set; }

            public class ImageDetails
            {
                [JsonPropertyName("size")]
                public string? Size { get; set; }
            }

            public class StylingDetails
            {
                [JsonPropertyName("style")]
                public string? Style { get; set; }

                [JsonPropertyName("effects")]
                public EffectsDetails Effects { get; set; } = new EffectsDetails();

                [JsonPropertyName("colors")]
                public List<ColorDetails> Colors { get; set; } = new List<ColorDetails>();
            }

            public class EffectsDetails
            {
                [JsonPropertyName("color")]
                public string? Color { get; set; }

                [JsonPropertyName("lightning")]
                public string? Lightning { get; set; }

                [JsonPropertyName("framing")]
                public string? Framing { get; set; }
            }

            public class ColorDetails
            {
                [JsonPropertyName("color")]
                public string? Color { get; set; }

                [JsonPropertyName("weight")]
                public int Weight { get; set; }
            }
        }
    }
}