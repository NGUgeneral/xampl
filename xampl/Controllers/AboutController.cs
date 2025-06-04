using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using xampl.Models;
using xampl.Models.Documents;
using xampl.Utils;

namespace xampl.Controllers
{
    public class AboutController : Controller
    {
        private readonly ILogger<AboutController> _logger;

        public AboutController(ILogger<AboutController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                ToastUtils.SetData(TempData, $"Welcome {User.FindFirstValue(ClaimTypes.GivenName)}");
            }
            ToastUtils.BindData(ViewBag, TempData);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
