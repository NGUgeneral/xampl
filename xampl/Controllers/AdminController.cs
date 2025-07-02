using Microsoft.AspNetCore.Mvc;

namespace xampl.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
