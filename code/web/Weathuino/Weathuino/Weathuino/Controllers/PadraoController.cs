using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Weathuino.Enums;
using Weathuino.Models;

namespace Weathuino.Controllers
{
    public class PadraoController : Controller
    {
        protected PerfisAcesso? AcessoExigido { get; set; }

        public virtual IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (AcessoExigido == null)
            {
                base.OnActionExecuting(context);
            }
            else
            {
                SessaoViewModel dadosSessao = HelpersControllers.ObtemDadosDaSessao(HttpContext.Session);
                bool usuarioTemPermissao = dadosSessao != null && dadosSessao.PerfilAcesso >= AcessoExigido;

                if (usuarioTemPermissao)
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
}
