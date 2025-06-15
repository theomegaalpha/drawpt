using DrawPT.Common.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DrawPT.Common.Services.AI
{

    public class FreepikMysticService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly string apiEndpoint;
        private readonly string apiKey;
        private readonly IStorageService _storageService;
        private readonly ILogger<FreepikMysticService> _logger;

        public FreepikMysticService(IConfiguration configuration, IStorageService storageService, ILogger<FreepikMysticService> logger)
        {
            apiEndpoint = configuration.GetValue<string>("FreepikUrl") ?? throw new InvalidOperationException("Freepik API endpoint not configured.");
            apiEndpoint += "/mystic";
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

            var refImage = _storageService.GetImageAsync("freepik/references/1.png").Result;
            string? refImageString = null;
            if (refImage != null)
                refImageString = Convert.ToBase64String(refImage);


            var requestPayload = new FreepikImageRequestPayload
            {
                Prompt = prompt,
                StyleReference = refImageString, // image style reference, replace with actual value
                Adherence = 100,
                Resolution = "2k",
                AspectRatio = "traditional_3_4",
                FixedGeneration = false,
                FilterNsfw = true,
                Styling = new StylingPayload
                {
                    Colors = new List<ColorDetailPayload>
                    {
                        new ColorDetailPayload { Color = "#7E9DB2", Weight = 0.5 }
                    }
                }
            };
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var payload = JsonSerializer.Serialize(requestPayload, options);

            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("x-freepik-api-key", apiKey);
                HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

                HttpResponseMessage initialResponse = await httpClient.PostAsync(apiEndpoint, httpContent);
                string initialResponseBody = await initialResponse.Content.ReadAsStringAsync();

                if (!initialResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Initial API Error: {initialResponse.StatusCode}. Response: {initialResponseBody}");
                    return null;
                }

                var initialFreepikResponse = JsonSerializer.Deserialize<FreepikImageResponse>(initialResponseBody);

                if (initialFreepikResponse?.Data == null || string.IsNullOrEmpty(initialFreepikResponse.Data.TaskId))
                {
                    _logger.LogError("Failed to deserialize initial Freepik API response, data is null, or TaskId is missing.");
                    _logger.LogDebug($"Full initial response for debugging: {initialResponseBody}");
                    return null;
                }

                string taskId = initialFreepikResponse.Data.TaskId;
                _logger.LogInformation($"Freepik API Task ID: {taskId}, Initial Status: {initialFreepikResponse.Data.Status}");

                string? imageUrlToDownload = null;
                string currentStatus = initialFreepikResponse.Data.Status;
                int pollingAttempts = 0;
                int maxPollingAttempts = 7; // 7 min
                int pollingIntervalSeconds = 60;

                while (currentStatus != "COMPLETED" && currentStatus != "FAILED" && pollingAttempts < maxPollingAttempts)
                {
                    pollingAttempts++;
                    await Task.Delay(TimeSpan.FromSeconds(pollingIntervalSeconds));

                    string statusCheckUrl = $"{apiEndpoint}/{taskId}";
                    _logger.LogInformation($"Polling attempt {pollingAttempts}/{maxPollingAttempts}. Checking status at: {statusCheckUrl}");

                    HttpResponseMessage statusResponse = await httpClient.GetAsync(statusCheckUrl);
                    string statusResponseBody = await statusResponse.Content.ReadAsStringAsync();

                    if (!statusResponse.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Status Check API Error: {statusResponse.StatusCode}. Response: {statusResponseBody}");
                        continue;
                    }

                    var statusFreepikResponse = JsonSerializer.Deserialize<FreepikImageResponse>(statusResponseBody);

                    if (statusFreepikResponse?.Data == null)
                    {
                        _logger.LogError("Failed to deserialize status Freepik API response or data is null.");
                        _logger.LogDebug($"Full status response for debugging: {statusResponseBody}");
                        continue;
                    }

                    currentStatus = statusFreepikResponse.Data.Status;
                    _logger.LogInformation($"Freepik API Task ID: {taskId}, Current Status: {currentStatus}");

                    if (currentStatus == "COMPLETED")
                    {
                        if (statusFreepikResponse.Data.Generated != null && statusFreepikResponse.Data.Generated.Any())
                        {
                            imageUrlToDownload = statusFreepikResponse.Data.Generated.FirstOrDefault();
                            var imageUrl = await _storageService.DownloadImageAsync(imageUrlToDownload!, $"dailies/{DateTime.UtcNow.AddDays(1):yyyyMMdd}.png");
                            return imageUrl;
                        }
                        else
                        {
                            _logger.LogError("Task completed but no image URL found in 'generated' array.");
                            _logger.LogDebug($"Full status response for debugging: {statusResponseBody}");
                            return null;
                        }
                    }
                    else if (currentStatus == "FAILED")
                    {
                        _logger.LogError($"Image generation failed for Task ID: {taskId}.");
                        _logger.LogDebug($"Full status response for debugging: {statusResponseBody}");
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An unexpected error occurred: {e.Message}");
                return null;
            }
            return null;
        }

        private class FreepikImageResponse
        {
            [JsonPropertyName("data")]
            public FreepikDataPayload? Data { get; set; }
        }

        private class FreepikDataPayload
        {
            [JsonPropertyName("generated")]
            public List<string> Generated { get; set; } = new List<string>();

            [JsonPropertyName("task_id")]
            public string TaskId { get; set; } = string.Empty;

            [JsonPropertyName("status")]
            public string Status { get; set; } = string.Empty;
        }

        private class FreepikImageRequestPayload
        {
            [JsonPropertyName("prompt")]
            public string Prompt { get; set; } = string.Empty;

            [JsonPropertyName("webhook_url")]
            public string? WebhookUrl { get; set; }

            [JsonPropertyName("structure_reference")]
            public string? StructureReference { get; set; }

            [JsonPropertyName("structure_strength")]
            public int? StructureStrength { get; set; }

            [JsonPropertyName("style_reference")]
            public string? StyleReference { get; set; }

            [JsonPropertyName("adherence")]
            public int? Adherence { get; set; }

            [JsonPropertyName("hdr")]
            public int? Hdr { get; set; }

            [JsonPropertyName("resolution")]
            public string? Resolution { get; set; }

            [JsonPropertyName("aspect_ratio")]
            public string? AspectRatio { get; set; }

            [JsonPropertyName("model")]
            public string? Model { get; set; }

            [JsonPropertyName("creative_detailing")]
            public int? CreativeDetailing { get; set; }

            [JsonPropertyName("engine")]
            public string? Engine { get; set; }

            [JsonPropertyName("fixed_generation")]
            public bool? FixedGeneration { get; set; }

            [JsonPropertyName("filter_nsfw")]
            public bool FilterNsfw { get; set; }

            [JsonPropertyName("styling")]
            public StylingPayload? Styling { get; set; }
        }

        private class StylingPayload
        {
            [JsonPropertyName("styles")]
            public List<StyleDetail>? Styles { get; set; }

            [JsonPropertyName("characters")]
            public List<CharacterDetail>? Characters { get; set; }

            [JsonPropertyName("colors")]
            public List<ColorDetailPayload>? Colors { get; set; }
        }

        private class StyleDetail
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("strength")]
            public int Strength { get; set; }
        }

        private class CharacterDetail
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("strength")]
            public int Strength { get; set; }
        }

        private class ColorDetailPayload
        {
            [JsonPropertyName("color")]
            public string Color { get; set; } = string.Empty;

            [JsonPropertyName("weight")]
            public double Weight { get; set; }
        }
    }
}
