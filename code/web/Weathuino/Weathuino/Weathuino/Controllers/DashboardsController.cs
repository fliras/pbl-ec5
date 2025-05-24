using Microsoft.AspNetCore.Mvc;
using Weathuino.Enums;

namespace Weathuino.Controllers
{
    public class DashboardsController : PadraoController
    {
        public DashboardsController()
        {
            AcessoExigido = PerfisAcesso.COMUM;
        }

        public IActionResult Dashboard1()
        {
            return View();
        }

        public IActionResult Dashboard2()
        {
            return View();
        }
    }
}
