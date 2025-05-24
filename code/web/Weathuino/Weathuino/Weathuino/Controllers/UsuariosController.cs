using Weathuino.DAO;
using Weathuino.Models;
using Weathuino.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Weathuino.Controllers
{
    public class UsuariosController : CRUDController<UsuarioViewModel>
    {
        public UsuariosController()
        {
            DAO = new UsuarioDAO();
            AcessoExigido = PerfisAcesso.ADMIN;
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

            if (usuario.PerfilAcesso == 0)
                ModelState.AddModelError("PerfilAcesso", "Escolha um perfil de acesso!");

            return ModelState.IsValid;
        }

        protected override void PreencheDadosParaView(string Operacao, UsuarioViewModel model)
        {
            base.PreencheDadosParaView(Operacao, model);
        }
    }
}
