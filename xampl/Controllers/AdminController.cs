using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using xampl.Services.ConfigOptionsService;
using xampl.Services.GeminiService;

namespace xampl.Controllers
{
    public class AdminController(
        IOptions<ConfigOptions> configOptions,
        GeminiService geminiService,
        ILogger<AdminController> logger
    ) : Controller
    {
        private readonly ConfigOptions _config = configOptions.Value;
        private readonly GeminiService _gemini = geminiService;
        private readonly ILogger<AdminController> _logger = logger;

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetInspirationQuote()
        {
            try
            {
                var prompt = "Provide a short, inspirational quote from a historical figure or famous author. " +
                    "The quote should be no more than 20 words. Provide only the quote and the author, nothing else. " +
                    "Format it as: \"Quote text\" - Author name";

                var geminiResponse = await _gemini.PerformGeminiRequest(
                    _config.GoogleGeminiApiKey,
                    _config.GoogleGeminiApiUrl,
                    prompt
                 );
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

                    return Ok(new { quote, author });
                }
                else
                {
                    _logger.LogError("Gemini response was valid JSON but no quote text extracted.");
                    throw new InvalidOperationException("Could not extract quote from Gemini response.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }
    }
}
