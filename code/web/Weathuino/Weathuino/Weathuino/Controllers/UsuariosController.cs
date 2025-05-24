using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Weathuino.DAO;
using Weathuino.Models;

namespace Weathuino.Controllers
{
    public class UsuariosController : PadraoController<UsuarioViewModel>
    {
        public UsuariosController()
        {
            DAO = new UsuarioDAO();
            GeraProximoId = true;
        }

        protected override bool ValidaDados(UsuarioViewModel usuario, string operacao)
        {
            bool validacaoBaseOK = base.ValidaDados(usuario, operacao);
            if (!validacaoBaseOK) return false;

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

        protected override void PreencheDadosParaView(string Operacao, UsuarioViewModel model)
        {
            base.PreencheDadosParaView(Operacao, model);
            PreparaComboPerfisAcesso();
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
