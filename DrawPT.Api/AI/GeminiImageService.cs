using DrawPT.Api.Services;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DrawPT.Api.AI
{
    public class GeminiRequestPayload
    {
        [JsonPropertyName("contents")]
        public List<ContentPart> Contents { get; set; } = new();

        [JsonPropertyName("generationConfig")]
        public GenerationConfiguration GenerationConfig { get; set; } = new();
    }

    public class ContentPart
    {
        [JsonPropertyName("parts")]
        public List<TextPart> Parts { get; set; } = new();
    }

    public class TextPart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    public class GenerationConfiguration
    {
        [JsonPropertyName("responseModalities")]
        public List<string> ResponseModalities { get; set; } = new();
    }

    // Define response body DTOs (simplified, based on common Gemini API structure for images)
    public class GeminiImageResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; } = new();
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public ContentData? Content { get; set; } // Marked as nullable
    }

    public class ContentData
    {
        [JsonPropertyName("parts")]
        public List<PartData> Parts { get; set; } = new();
    }

    public class PartData
    {
        [JsonPropertyName("inlineData")]
        public InlineData? InlineData { get; set; } // Marked as nullable

        [JsonPropertyName("text")]
        public string? Text { get; set; } // For any text part that might also be returned, marked as nullable
    }

    public class InlineData
    {
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public string Data { get; set; } = string.Empty; // This is the base64 image string
    }

    public class GeminiImageGenerator
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly string apiEndpoint;
        private readonly StorageService _storageService;

        public GeminiImageGenerator(IConfiguration configuration, StorageService storageService)
        {
            apiEndpoint = configuration.GetValue<string>("ConnectionStrings:gemini") ?? throw new InvalidOperationException("Gemini API endpoint not configured.");
            _storageService = storageService;
        }

        public async Task<string?> GenerateAndSaveImageAsync(string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                throw new ArgumentNullException(nameof(prompt), "Prompt cannot be null or empty.");
            }

            var requestPayload = new GeminiRequestPayload
            {
                Contents = new List<ContentPart>
                {
                    new ContentPart
                    {
                        Parts = new List<TextPart>
                        {
                            new TextPart { Text = prompt }
                        }
                    }
                },
                GenerationConfig = new GenerationConfiguration
                {
                    ResponseModalities = new List<string> { "TEXT", "IMAGE" }
                }
            };

            var jsonRequest = JsonSerializer.Serialize(requestPayload);
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(apiEndpoint, httpContent);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    Console.WriteLine($"Response: {responseBody}");
                    response.EnsureSuccessStatusCode();
                }

                var geminiResponse = JsonSerializer.Deserialize<GeminiImageResponse>(responseBody);

                string? base64Image = null; // Changed to nullable string
                if (geminiResponse?.Candidates != null)
                {
                    foreach (var candidate in geminiResponse.Candidates)
                    {
                        if (candidate.Content?.Parts != null)
                        {
                            foreach (var part in candidate.Content.Parts)
                            {
                                if (part.InlineData != null && !string.IsNullOrEmpty(part.InlineData.Data))
                                {
                                    base64Image = part.InlineData.Data;
                                    Console.WriteLine($"Found image data with MIME type: {part.InlineData.MimeType}");
                                    break;
                                }
                            }
                        }
                        if (base64Image != null) break;
                    }
                }

                if (string.IsNullOrEmpty(base64Image))
                {
                    Console.WriteLine("Could not find image data in the API response.");
                    Console.WriteLine($"Full response for debugging: {responseBody}");
                    return null;
                }

                byte[] imageBytes = Convert.FromBase64String(base64Image);
                string blobName = $"gemini-image-{Guid.NewGuid()}.png";
                string? imageUrl = await _storageService.UploadImageAsync(imageBytes, blobName);

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
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON parsing error: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An unexpected error occurred: {e.Message}");
                return null;
            }
        }
    }
}