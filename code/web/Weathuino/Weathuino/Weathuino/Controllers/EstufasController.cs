using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Weathuino.DAO;
using Weathuino.Models;
using Weathuino.Enums;

namespace Weathuino.Controllers
{
    public class EstufasController : CRUDController<EstufaViewModel>
    {
        public EstufasController()
        {
            DAO = new EstufaDAO();
            AcessoExigido = PerfisAcesso.COMUM;
        }

        protected override bool ValidaDados(EstufaViewModel estufa, string operacao)
        {
            bool validacaoBaseOK = base.ValidaDados(estufa, operacao);
            if (!validacaoBaseOK) return false;

            if (string.IsNullOrEmpty(estufa.Nome))
                ModelState.AddModelError("Nome", "Informe um nome!");

            if (string.IsNullOrEmpty(estufa.Descricao))
                ModelState.AddModelError("Descricao", "Dê uma descrição!");

            if (estufa.Medidor.Id == 0)
                ModelState.AddModelError("Medidor.Id", "Escolha um medidor!");

            return ModelState.IsValid;
        }

        protected override void PreencheDadosParaView(string Operacao, EstufaViewModel model)
        {
            base.PreencheDadosParaView(Operacao, model);
            PreparaComboMedidores();
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
