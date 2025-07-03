using Microsoft.Extensions.Options;
using System.Text.Json;
using xampl.Services.ConfigOptionsService;

namespace xampl.Services.GeminiService
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeminiService> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public GeminiService(
            IHttpClientFactory httpClientFactory,
            ILogger<GeminiService> logger
        )
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
        }

        private async Task<GeminiApiResponse?> PerformGeminiRequest(
            string apiKey,
            string apiUrl,
            string prompt
        )
        {
            GeminiApiResponse? geminiResponse = null;
            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Headers.Add("X-goog-api-key", apiKey);
                var jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    geminiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(responseString, _jsonSerializerOptions)!;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error from Gemini API: Status {status_code}, Reason: {responsePhrase}, Content: {errorContent}", (int)response.StatusCode, response.ReasonPhrase, errorContent);
                    throw new HttpRequestException($"Error from Gemini API: {response.ReasonPhrase} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred: {ex_message}", ex.Message);
            }

            return geminiResponse;
        }

        public async Task<(string Quote, string Author)> GetInspirationQuoteAsync(
            string apiKey,
            string apiUrl,
            string prompt
        )
        {
            try
            {
                var geminiResponse = await PerformGeminiRequest(apiKey, apiUrl, prompt);
                var quoteText = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                if (!string.IsNullOrEmpty(quoteText))
                {
                    quoteText = quoteText.Trim().Replace(" ", " ");

                    string quote = quoteText;
                    string author = "Unknown";

                    var parts = quoteText.Split([" - "], StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        quote = parts[0].Trim().Replace("\"", "");
                        author = parts[1].Trim();
                    }
                    else if (parts.Length == 1)
                    {
                        quote = parts[0].Trim().Replace("\"", "");
                    }

                    return (quote, author);
                }
                else
                {
                    _logger.LogError("Gemini response was valid JSON but no quote text extracted.");
                    throw new InvalidOperationException("Could not extract quote from Gemini response.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred: {ex_message}", ex.Message);
                throw;
            }
        }
    }
}
