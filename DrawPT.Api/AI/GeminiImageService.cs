using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DrawPT.Api.AI
{
    public class GeminiRequestPayload
    {
        [JsonPropertyName("contents")]
        public List<ContentPart> Contents { get; set; }

        [JsonPropertyName("generationConfig")]
        public GenerationConfiguration GenerationConfig { get; set; }
    }

    public class ContentPart
    {
        [JsonPropertyName("parts")]
        public List<TextPart> Parts { get; set; }
    }

    public class TextPart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class GenerationConfiguration
    {
        [JsonPropertyName("responseModalities")]
        public List<string> ResponseModalities { get; set; }
    }

    // Define response body DTOs (simplified, based on common Gemini API structure for images)
    public class GeminiImageResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public ContentData Content { get; set; }
    }

    public class ContentData
    {
        [JsonPropertyName("parts")]
        public List<PartData> Parts { get; set; }
    }

    public class PartData
    {
        [JsonPropertyName("inlineData")]
        public InlineData InlineData { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } // For any text part that might also be returned
    }

    public class InlineData
    {
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; } // This is the base64 image string
    }

    public class GeminiImageGenerator
    {
        // It's recommended to manage HttpClient instances carefully.
        // For simplicity, a static instance is used here, but in a larger application (like ASP.NET Core),
        // you would typically use IHttpClientFactory to create and manage HttpClient instances.
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly string apiEndpoint;

        public GeminiImageGenerator(IConfiguration configuration)
        {
            apiEndpoint = configuration.GetValue<string>("ConnectionStrings:gemini") ?? "";
        }

        public async Task GenerateAndSaveImageAsync(string prompt)
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
                    ResponseModalities = [ "TEXT", "IMAGE" ]
                }
            };

            var jsonRequest = JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions { }); // Add options if needed
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(apiEndpoint, httpContent);

                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Log or handle the error response
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    Console.WriteLine($"Response: {responseBody}");
                    response.EnsureSuccessStatusCode(); // This will throw an exception
                }

                var geminiResponse = JsonSerializer.Deserialize<GeminiImageResponse>(responseBody);

                string base64Image = null;
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
                    return;
                }

                byte[] imageBytes = Convert.FromBase64String(base64Image);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                // Further error details if needed, e.g., e.StatusCode
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON parsing error: {e.Message}");
                // Potentially log the responseBody here if it caused the JSON error
            }
            catch (Exception e)
            {
                Console.WriteLine($"An unexpected error occurred: {e.Message}");
            }
        }
    }
}