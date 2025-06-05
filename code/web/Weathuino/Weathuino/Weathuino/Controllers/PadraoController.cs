using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using Weathuino.Enums;
using Weathuino.Models;
using Weathuino.Utils;

namespace Weathuino.Controllers
{
    /// <summary>
    /// Base para criação de controllers no sistema
    /// </summary>
    public class PadraoController : Controller
    {
        protected PerfisAcesso? AcessoExigido { get; set; } // Perfil de acesso mínimo de cada controller no sistema

        /// <summary>
        /// Carregamento base da página principal da controller
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Método para executar regras logo antes do arranque de cada controller
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (AcessoExigido == null)
            {
                base.OnActionExecuting(context);
            }
            else
            {
                SessaoViewModel dadosSessao = ObtemDadosDaSessao(HttpContext.Session);
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

        /// <summary>
        /// Carrega os dados da sessão do usuário logado no sistema
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private static SessaoViewModel ObtemDadosDaSessao(ISession session)
        {
            string dadosEmJSON = session.GetString("DadosSessao");
            if (dadosEmJSON.IsNullOrEmpty())
                return null;
            return JSONUtils.ConverteStringJSONParaObjeto<SessaoViewModel>(dadosEmJSON);
        }
    }
}
