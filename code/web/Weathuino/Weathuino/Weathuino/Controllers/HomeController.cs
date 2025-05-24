using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Weathuino.Models;

namespace Weathuino.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            SessaoViewModel dadosSessao = HelpersControllers.ObtemDadosDaSessao(HttpContext.Session);

            if (dadosSessao != null)
            {
                ViewBag.Logado = true;
                ViewBag.DadosSessao = dadosSessao;
                base.OnActionExecuting(context);
            }
            else
            {
                context.Result = RedirectToAction("Index", "Autenticacao");
            }
        }
    }
}
