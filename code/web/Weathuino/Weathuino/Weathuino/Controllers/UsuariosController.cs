using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Weathuino.DAO;
using Weathuino.Models;

namespace Weathuino.Controllers
{
    public class UsuariosController : Controller
    {
        public IActionResult Index()
        {
            UsuarioDAO dao = new UsuarioDAO();
            return View(dao.Listagem());
        }

        public IActionResult Create()
        {
            try
            {
                ViewBag.Operacao = "I";
                UsuarioDAO dao = new UsuarioDAO();
                int idLivre = dao.ProximoId();
                PreparaComboPerfisAcesso();
                return View("Form", new UsuarioViewModel() { Id = idLivre });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        public IActionResult Edit(int id)
        {
            try
            {
                ViewBag.Operacao = "A";
                UsuarioDAO dao = new UsuarioDAO();
                UsuarioViewModel usuario = dao.Consulta(id);

                if (usuario == null)
                    return View("Index");

                PreparaComboPerfisAcesso();
                return View("Form", usuario);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                UsuarioDAO dao = new UsuarioDAO();
                dao.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        public IActionResult Salvar(UsuarioViewModel usuario, string operacao)
        {
            try
            {
                if (!ValidaDados(usuario, operacao))
                {
                    ViewBag.Operacao = operacao;
                    PreparaComboPerfisAcesso();
                    return View("Form", usuario);
                }

                UsuarioDAO dao = new UsuarioDAO();

                if (operacao == "I")
                    dao.Insert(usuario);
                else
                    dao.Update(usuario);

                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public bool ValidaDados(UsuarioViewModel usuario, string operacao)
        {
            ModelState.Clear();
            UsuarioDAO dao = new UsuarioDAO();

            if (usuario.Id <= 0)
                ModelState.AddModelError("Id", "Informe um ID válido!");
            else if (operacao == "I" && dao.Consulta(usuario.Id) != null)
                ModelState.AddModelError("Id", "Este ID já está em uso!");
            else if (operacao == "A" && dao.Consulta(usuario.Id) == null)
                ModelState.AddModelError("Id", "usuário não encontrada!");

            if (string.IsNullOrEmpty(usuario.Nome))
                ModelState.AddModelError("Nome", "Informe um nome!");

            if (string.IsNullOrEmpty(usuario.Email))
                ModelState.AddModelError("Email", "Informe um E-mail!");

            if (string.IsNullOrEmpty(usuario.Senha))
                ModelState.AddModelError("Senha", "Informe uma senha!");

            if (usuario.PerfilAcesso.Id == 0)
                ModelState.AddModelError("PerfilAcesso.Id", "Escolha um perfil de acesso!");

            return ModelState.IsValid;
        }

        private void PreparaComboPerfisAcesso()
        {
            PerfilAcessoDAO dao = new PerfilAcessoDAO();
            List<PerfilAcessoViewModel> perfis = dao.Listagem();
            List<SelectListItem> listPerfis = new List<SelectListItem>();

            listPerfis.Add(new SelectListItem("Selecione um perfil...", "0"));

            foreach (PerfilAcessoViewModel perfil in perfis)
            {
                SelectListItem item = new SelectListItem(perfil.Nome, perfil.Id.ToString());
                listPerfis.Add(item);
            }
            ViewBag.PerfisAcesso = listPerfis;
        }
    }
}
