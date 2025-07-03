using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using xampl.Services.ConfigOptionsService;
using xampl.Services.GeminiService;

namespace xampl.Controllers
{
    public class AdminController(
        IOptions<ConfigOptions> configOptions,
        GeminiService geminiService
    ) : Controller
    {
        private readonly ConfigOptions _config = configOptions.Value;
        private readonly GeminiService _gemini = geminiService;

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetInspirationQuote()
        {
            var prompt = "Provide a short, inspirational quote from a historical figure or famous author. " +
                "The quote should be no more than 20 words. Provide only the quote and the author, nothing else. " +
                "Format it as: \"Quote text\" - Author name";

            (var quote, var author) = await _gemini.GetInspirationQuoteAsync(
                _config.GoogleGeminiApiKey,
                _config.GoogleGeminiApiUrl,
                prompt
            );
            return Ok(new { quote, author });
        }
    }
}
