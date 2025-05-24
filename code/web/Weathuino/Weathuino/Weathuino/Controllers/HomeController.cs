using Microsoft.AspNetCore.Mvc;

namespace Weathuino.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
