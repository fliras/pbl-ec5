using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Weathuino.DAO;
using Weathuino.Models;

namespace Weathuino.Controllers
{
    public class AutenticacaoController : Controller
    {
        private UsuarioDAO usuarioDAO = new UsuarioDAO();

        public IActionResult Index()
        {
            return View("Index");
        }

        public IActionResult Login(string email, string senha)
        {
            if (email.IsNullOrEmpty() || senha.IsNullOrEmpty())
            {
                ViewBag.ErroLogin = "Informe email e senha!";
                return View("Index");
            }

            SessaoViewModel sessao = usuarioDAO.RealizaLogin(email, senha);
            if (sessao == null)
            {
                ViewBag.ErroLogin = "Usuário ou senha inválidos!";
                return View("Index");
            }

            RegistraSessao(sessao);
            return RedirectToAction("index", "Home");
        }

        private void RegistraSessao(SessaoViewModel dadosSessao)
        {
            string dadosSessaoEmJSON = JsonConvert.SerializeObject(dadosSessao);
            HttpContext.Session.SetString("DadosSessao", dadosSessaoEmJSON);
            HttpContext.Session.SetString("Logado", "true");
        }

        public IActionResult Logoff()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
