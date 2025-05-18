using Microsoft.AspNetCore.Mvc;

namespace Weathuino.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
