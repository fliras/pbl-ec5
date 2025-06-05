using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Weathuino.DAO;
using Weathuino.Models;
using Weathuino.Enums;
using Weathuino.Utils;

namespace Weathuino.Controllers
{
    /// <summary>
    /// Gerencia o CRUD de estufas
    /// </summary>
    public class EstufasController : CRUDController<EstufaViewModel>
    {
        public EstufasController()
        {
            DAO = new EstufaDAO();
            AcessoExigido = PerfisAcesso.COMUM; // ao menos usuários logados devem acessar
        }

        /// <summary>
        /// Validação dos dados de estufa
        /// </summary>
        /// <param name="estufa"></param>
        /// <param name="operacao"></param>
        /// <returns></returns>
        protected override bool ValidaDados(EstufaViewModel estufa, ModosOperacao operacao)
        {
            bool validacaoBaseOK = base.ValidaDados(estufa, operacao);
            if (!validacaoBaseOK) return false;

            if (string.IsNullOrEmpty(estufa.Nome))
                ModelState.AddModelError("Nome", "Informe um nome!");

            if (string.IsNullOrEmpty(estufa.Descricao))
                ModelState.AddModelError("Descricao", "Dê uma descrição!");

            if (estufa.Medidor.Id == 0)
                ModelState.AddModelError("Medidor.Id", "Escolha um medidor!");

            if (estufa.Imagem == null && operacao == ModosOperacao.INCLUSAO)
                ModelState.AddModelError("Imagem", "Escolha uma imagem.");

            bool imagemDeTamanhoExcessivo = estufa.Imagem != null && estufa.Imagem.Length / 1024 / 1024 >= 2;
            if (imagemDeTamanhoExcessivo)
                ModelState.AddModelError("Imagem", "Imagem limitada a 2 mb.");

            if (ModelState.IsValid)
            {
                bool imagemNaoMudouNaAlteracao = operacao == ModosOperacao.ALTERACAO && estufa.Imagem == null;
                if (imagemNaoMudouNaAlteracao)
                {
                    estufa.ImagemEmByte = DAO.ObtemPorID(estufa.Id).ImagemEmByte;
                }
                else
                {
                    estufa.ImagemEmByte = FileUtils.ConverteArquivoEmArrayDeBytes(estufa.Imagem);
                }
            }

            return ModelState.IsValid;
        }

        protected override void PreencheDadosParaView(ModosOperacao Operacao, EstufaViewModel model)
        {
            base.PreencheDadosParaView(Operacao, model);
            PreparaComboMedidores();
        }

        /// <summary>
        /// Carrega o combobox de medidores que podem ser associados as estufas
        /// </summary>
        private void PreparaComboMedidores()
        {
            MedidorDAO dao = new MedidorDAO();
            List<MedidorViewModel> medidores = dao.ObtemTodos();
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
