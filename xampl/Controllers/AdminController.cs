using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Net.Http;
using System.Text.Json;
using xampl.Services.ConfigOptionsService;

namespace xampl.Controllers
{
    public class AdminController(
        IHttpClientFactory httpClientFactory,
        IOptions<ConfigOptions> configOptions
    ) : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly ConfigOptions _config = configOptions.Value;

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetInspirationQuote()
        {
            var googleGeminiApiKey = _config.GoogleGeminiApiKey;
            if (string.IsNullOrEmpty(googleGeminiApiKey))
            {
                return StatusCode(500, "Gemini API Key is not configured.");
            }

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("X-goog-api-key", googleGeminiApiKey);

            var apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { 
                                text = "Provide a short, inspirational quote from a historical figure or famous author. " +
                                "The quote should be no more than 20 words. Provide only the quote and the author, nothing else. " +
                                "Format it as: \"Quote text\" - Author name" 
                            }
                        }
                    }
            }
            };

            try
            {
                var jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Raw Gemini Response: {responseString}"); // Log the raw response for debugging

                    // Deserialize with options for robustness
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true, // Important for matching 'candidates' to 'Candidates'
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip
                    };

                    GeminiApiResponse geminiResponse = null;
                    try
                    {
                        geminiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(responseString, options);
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
                        Console.WriteLine($"JSON Content that failed: {responseString}");
                        return StatusCode(500, $"Error parsing Gemini API response JSON: {ex.Message}");
                    }


                    var quoteText = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                    if (!string.IsNullOrEmpty(quoteText))
                    {
                        // Clean up the quote text: remove trailing newlines and non-breaking spaces if any
                        quoteText = quoteText.Trim().Replace(" ", " "); // Replace non-breaking space with regular space

                        string quote = quoteText;
                        string author = "Unknown";

                        // Split by " - " to separate quote and author
                        var parts = quoteText.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            quote = parts[0].Trim().Replace("\"", ""); // Remove surrounding quotes
                            author = parts[1].Trim();
                        }
                        else if (parts.Length == 1)
                        {
                            // If only one part, assume it's the quote itself, and try to remove internal quotes
                            quote = parts[0].Trim().Replace("\"", "");
                        }

                        return Ok(new { quote = quote, author = author });
                    }
                    else
                    {
                        Console.WriteLine($"Gemini response was valid JSON but no quote text extracted. Full response: {responseString}");
                        return StatusCode(500, "Could not extract quote from Gemini response. Check console for full response.");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Error from Gemini API: {response.ReasonPhrase} - {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Network error or problem connecting to Gemini API: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return StatusCode(500, $"Error parsing Gemini API response: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }

    public class GeminiApiResponse
    {
        public Candidate[] Candidates { get; set; }
        public UsageMetadata? UsageMetadata { get; set; } // Nullable, as it might not always be there
        public string? ModelVersion { get; set; }
        public string? ResponseId { get; set; }
    }

    public class Candidate
    {
        public Content Content { get; set; }
        public string? FinishReason { get; set; } // For 'STOP' etc.
        public double? AvgLogprobs { get; set; } // Nullable double
    }

    public class Content
    {
        public Part[] Parts { get; set; }
        public string? Role { get; set; } // For 'model' or 'user'
    }

    public class Part
    {
        public string Text { get; set; }
    }

    public class UsageMetadata
    {
        public int PromptTokenCount { get; set; }
        public int CandidatesTokenCount { get; set; }
        public int TotalTokenCount { get; set; }
        public PromptTokensDetail[] PromptTokensDetails { get; set; }
        public CandidatesTokensDetail[] CandidatesTokensDetails { get; set; }
    }

    public class PromptTokensDetail
    {
        public string Modality { get; set; }
        public int TokenCount { get; set; }
    }

    public class CandidatesTokensDetail
    {
        public string Modality { get; set; }
        public int TokenCount { get; set; }
    }
}
