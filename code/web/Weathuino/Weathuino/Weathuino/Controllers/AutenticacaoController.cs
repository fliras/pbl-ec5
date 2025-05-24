using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Weathuino.DAO;
using Weathuino.Models;
using Weathuino.Utils;

namespace Weathuino.Controllers
{
    public class AutenticacaoController : PadraoController
    {
        private readonly UsuarioDAO _usuarioDAO = new UsuarioDAO();

        public IActionResult Login(string email, string senha)
        {
            try
            {
                if (email.IsNullOrEmpty() || senha.IsNullOrEmpty())
                {
                    ViewBag.ErroLogin = "Informe email e senha!";
                    return View("Index");
                }

                SessaoViewModel sessao = _usuarioDAO.RealizaLogin(email, senha);
                if (sessao == null)
                {
                    ViewBag.ErroLogin = "Usuário ou senha inválidos!";
                    return View("Index");
                }

                RegistraSessao(sessao);
                return RedirectToAction("index", "Home");
            }
            catch (Exception error)
            {
                return View("Error", new ErrorViewModel(error.ToString()));
            }
        }

        private void RegistraSessao(SessaoViewModel dadosSessao)
        {
            string dadosSessaoEmJSON = JSONUtils.ConverteObjetoParaStringJSON(dadosSessao);
            HttpContext.Session.SetString("DadosSessao", dadosSessaoEmJSON);
            HttpContext.Session.SetString("Logado", "true");
        }

        public IActionResult Logoff()
        {
            try
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }
            catch (Exception error)
            {
                return View("Error", new ErrorViewModel(error.ToString()));
            }
        }
    }
}
