using System;
using Microsoft.AspNetCore.Mvc;
using Weathuino.DAO;
using Weathuino.Models;

namespace Weathuino.Controllers
{
    public class MedidoresController : PadraoController<MedidorViewModel>
    {
        public MedidoresController()
        {
            DAO = new MedidorDAO();
            GeraProximoId = true;
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

        protected override bool ValidaDados(MedidorViewModel medidor, string operacao)
        {
            bool validacaoBaseOK = base.ValidaDados(medidor, operacao);
            if (!validacaoBaseOK) return false;

            if (string.IsNullOrEmpty(medidor.Nome))
                ModelState.AddModelError("Nome", "Informe um nome!");

            return ModelState.IsValid;
        }
    }
}
