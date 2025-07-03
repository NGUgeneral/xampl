using Microsoft.AspNetCore.Mvc;
using xampl.Services.GeminiService;

namespace xampl.Controllers
{
    public class AdminController(
        GeminiService geminiService
    ) : Controller
    {
        private readonly GeminiService _gemini = geminiService;

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetInspirationQuote()
        {
            var promptText = "Provide a short, inspirational quote from a historical figure or famous author. " +
                "The quote should be no more than 20 words. Provide only the quote and the author, nothing else. " +
                "Format it as: \"Quote text\" - Author name";
            (var quote, var author) = await _gemini.GetInspirationQuoteAsync(promptText);
            return Ok(new { quote, author });
        }
    }
}
