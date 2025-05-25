using System;
using Microsoft.AspNetCore.Mvc;
using Weathuino.DAO;
using Weathuino.Models;
using Weathuino.Enums;

namespace Weathuino.Controllers
{
    public class MedidoresController : CRUDController<MedidorViewModel>
    {
        public MedidoresController()
        {
            DAO = new MedidorDAO();
            AcessoExigido = PerfisAcesso.COMUM;
        }

        public override IActionResult Delete(int id)
        {
            try
            {
                MedidorDAO dao = new MedidorDAO();
                bool medidorEmUso = dao.VerificaSeMedidorEstaEmUso(id);
                if (medidorEmUso)
                {
                    ViewBag.AlertaErro = "Este medidor não pode ser excluído pois já está em uso!";
                    return View("Index", dao.ObtemTodos());
                }

                dao.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        protected override bool ValidaDados(MedidorViewModel medidor, ModosOperacao operacao)
        {
            bool validacaoBaseOK = base.ValidaDados(medidor, operacao);
            if (!validacaoBaseOK) return false;

            if (string.IsNullOrEmpty(medidor.Nome))
                ModelState.AddModelError("Nome", "Informe um nome!");

            return ModelState.IsValid;
        }
    }
}
