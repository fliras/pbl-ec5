using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Weathuino.DAO;
using Weathuino.Models;
using Weathuino.Utils;

namespace Weathuino.Controllers
{
    /// <summary>
    /// Gerenciamento da autenticação do sistema
    /// </summary>
    public class AutenticacaoController : PadraoController
    {
        private readonly UsuarioDAO _usuarioDAO = new UsuarioDAO();

        /// <summary>
        /// Realiza a autenticação do usuário por meio de seu login e senha
        /// </summary>
        /// <param name="email"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        public IActionResult Login(string email, string senha)
        {
            try
            {
                if (email.IsNullOrEmpty() || senha.IsNullOrEmpty())
                {
                    ViewBag.ErroLogin = "Informe email e senha!";
                    return View("Index");
                }

                // realiza o login e caso não consiga os dados de sessão, os dados de login estão incorretos
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

        /// <summary>
        /// Registra os dados de sessão do usuário após o login
        /// </summary>
        /// <param name="dadosSessao"></param>
        private void RegistraSessao(SessaoViewModel dadosSessao)
        {
            string dadosSessaoEmJSON = JSONUtils.ConverteObjetoParaStringJSON(dadosSessao);
            HttpContext.Session.SetString("DadosSessao", dadosSessaoEmJSON);
            HttpContext.Session.SetString("Logado", "true");
        }

        /// <summary>
        /// Finaliza a sessão do usuário e volta para a tela de login
        /// </summary>
        /// <returns></returns>
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
