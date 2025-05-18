using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Weathuino.DAO;
using Weathuino.Models;

namespace Weathuino.Controllers
{
    public class MedidoresController : Controller
    {
        public IActionResult Index()
        {
            MedidorDAO dao = new MedidorDAO();
            return View(dao.Listagem());
        }

        public IActionResult Create()
        {
            try
            {
                ViewBag.Operacao = "I";
                MedidorDAO dao = new MedidorDAO();
                int idLivre = dao.ProximoId();
                return View("Form", new MedidorViewModel() { Id = idLivre });
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
                MedidorDAO dao = new MedidorDAO();
                MedidorViewModel medidor = dao.Consulta(id);

                if (medidor == null)
                    return View();

                return View("Form", medidor);

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
                MedidorDAO dao = new MedidorDAO();
                
                bool medidorEmUso = dao.VerificaSeMedidorEstaEmUso(id);
                if (medidorEmUso)
                {
                    ViewBag.AlertaErro = "Este medidor não pode ser excluído pois já está em uso!";
                    return View("Index", dao.Listagem());
                }

                dao.DeletaPorID(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        public IActionResult Salvar(MedidorViewModel medidor, string operacao)
        {
            try
            {
                if (!ValidaDados(medidor, operacao))
                {
                    ViewBag.Operacao = operacao;
                    return View("Form", medidor);
                }

                MedidorDAO dao = new MedidorDAO();

                if (operacao == "I")
                    dao.Insert(medidor);
                else
                    dao.Update(medidor);

                return RedirectToAction("Index");
                
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public bool ValidaDados(MedidorViewModel medidor, string operacao)
        {
            ModelState.Clear();
            MedidorDAO dao = new MedidorDAO();

            if (medidor.Id <= 0)
                ModelState.AddModelError("Id", "Informe um ID válido!");
            else if (operacao == "I" && dao.Consulta(medidor.Id) != null)
                ModelState.AddModelError("Id", "Este ID já está em uso!");
            else if (operacao == "A" && dao.Consulta(medidor.Id) == null)
                ModelState.AddModelError("Id", "medidor não encontrado!");

            if (string.IsNullOrEmpty(medidor.Nome))
                ModelState.AddModelError("Nome", "Informe um nome!");

            return ModelState.IsValid;
        }
    }
}
