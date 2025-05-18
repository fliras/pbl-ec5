using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Weathuino.DAO;
using Weathuino.Models;

namespace Weathuino.Controllers
{
    public class EstufasController : Controller
    {
        public IActionResult Index()
        {
            EstufaDAO dao = new EstufaDAO();
            return View(dao.Listagem());
        }

        public IActionResult Create()
        {
            try
            {
                ViewBag.Operacao = "I";
                EstufaDAO dao = new EstufaDAO();
                int idLivre = dao.ProximoId();
                PreparaComboMedidores();
                return View("Form", new EstufaViewModel() { Id = idLivre });
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
                EstufaDAO dao = new EstufaDAO();
                EstufaViewModel estufa = dao.Consulta(id);

                if (estufa == null)
                    return View("Index");

                PreparaComboMedidores();
                return View("Form", estufa);

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
                EstufaDAO dao = new EstufaDAO();
                dao.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        public IActionResult Salvar(EstufaViewModel estufa, string operacao)
        {
            try
            {
                if (!ValidaDados(estufa, operacao))
                {
                    ViewBag.Operacao = operacao;
                    PreparaComboMedidores();
                    return View("Form", estufa);
                }

                EstufaDAO dao = new EstufaDAO();

                if (operacao == "I")
                    dao.Insert(estufa);
                else
                    dao.Update(estufa);

                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public bool ValidaDados(EstufaViewModel estufa, string operacao)
        {
            ModelState.Clear();
            EstufaDAO dao = new EstufaDAO();

            if (estufa.Id <= 0)
                ModelState.AddModelError("Id", "Informe um ID válido!");
            else if (operacao == "I" && dao.Consulta(estufa.Id) != null)
                ModelState.AddModelError("Id", "Este ID já está em uso!");
            else if (operacao == "A" && dao.Consulta(estufa.Id) == null)
                ModelState.AddModelError("Id", "estufa não encontrada!");

            if (string.IsNullOrEmpty(estufa.Nome))
                ModelState.AddModelError("Nome", "Informe um nome!");

            if (string.IsNullOrEmpty(estufa.Descricao))
                ModelState.AddModelError("Descricao", "Dê uma descrição!");

            if (estufa.Medidor.Id == 0)
                ModelState.AddModelError("Medidor.Id", "Escolha um medidor!");

            return ModelState.IsValid;
        }

        private void PreparaComboMedidores()
        {
            MedidorDAO dao = new MedidorDAO();
            List<MedidorViewModel> medidores = dao.Listagem();
            List<SelectListItem> listMedidores = new List<SelectListItem>();

            listMedidores.Add(new SelectListItem("Selecione um medidor...", "0"));

            foreach (MedidorViewModel medidor in medidores)
            {
                SelectListItem item = new SelectListItem(medidor.Nome, medidor.Id.ToString());
                listMedidores.Add(item);
            }
            ViewBag.Medidores = listMedidores;
        }
    }
}
