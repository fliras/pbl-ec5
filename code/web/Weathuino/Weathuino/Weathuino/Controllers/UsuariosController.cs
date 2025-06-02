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

        protected override bool ValidaDados(UsuarioViewModel usuario, ModosOperacao operacao)
        {
            bool validacaoBaseOK = base.ValidaDados(usuario, operacao);
            if (!validacaoBaseOK) return false;

            if (string.IsNullOrEmpty(usuario.Nome))
                ModelState.AddModelError("Nome", "Informe um nome!");

            if (string.IsNullOrEmpty(usuario.Email))
                ModelState.AddModelError("Email", "Informe um E-mail!");
            else
            {
                UsuarioViewModel usuarioDoEmail = ((UsuarioDAO)DAO).ObtemPorEmail(usuario.Email);
                bool emailEmUsoPorOutroUsuario = usuarioDoEmail != null && usuarioDoEmail.Id != usuario.Id;
                if (emailEmUsoPorOutroUsuario)
                    ModelState.AddModelError("Email", "Este E-mail já está em uso!");
            }

            if (string.IsNullOrEmpty(usuario.Senha))
                ModelState.AddModelError("Senha", "Informe uma senha!");

            if (usuario.PerfilAcesso == 0)
                ModelState.AddModelError("PerfilAcesso", "Escolha um perfil de acesso!");

            return ModelState.IsValid;
        }

        protected override void PreencheDadosParaView(ModosOperacao Operacao, UsuarioViewModel model)
        {
            base.PreencheDadosParaView(Operacao, model);
        }
    }
}
