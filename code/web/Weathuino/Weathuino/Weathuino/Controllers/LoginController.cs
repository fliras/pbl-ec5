using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Weathuino.DAO;

namespace Weathuino.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }

        public IActionResult FazLogin(string email, string senha)
        {
            if (email.IsNullOrEmpty() || senha.IsNullOrEmpty())
            {
                ViewBag.ErroLogin = "Informe email e senha!";
                return View("Index");
            }

            UsuarioDAO dao = new UsuarioDAO();
            if (dao.RealizaLogin(email, senha))
            {
                HttpContext.Session.SetString("Logado", "true");
                return RedirectToAction("index", "Home");
            }

            ViewBag.ErroLogin = "Usuário ou senha inválidos!";
            return View("Index");
        }

        public IActionResult LogOff()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
