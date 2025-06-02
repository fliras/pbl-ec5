using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Weathuino.Models;
using Weathuino.Enums;
using System.Collections.Generic;
using Weathuino.Utils;

namespace Weathuino.DAO
{
    public class UsuarioDAO : PadraoDAO<UsuarioViewModel>
    {
        protected override SqlParameter[] CriaParametros(UsuarioViewModel usuario)
        {
            return new SqlParameter[]
            {
                new SqlParameter("id", usuario.Id),
                new SqlParameter("nome", usuario.Nome),
                new SqlParameter("email", usuario.Email),
                new SqlParameter("senha", BCryptUtils.CriaHashBcrypt(usuario.Senha)),
                new SqlParameter("perfilUsuario", usuario.PerfilAcesso)
            };
        }

        public SessaoViewModel RealizaLogin(string email, string senha)
        {
            FiltrosViewModel filtroEmail = new FiltrosViewModel { Email = email };
            List<UsuarioViewModel> usuariosFiltrados = ConsultaComFiltros(filtroEmail);
            if (usuariosFiltrados.Count == 0)
                return null;

            UsuarioViewModel usuarioDoEmail = ObtemPorEmail(email);
            if (usuarioDoEmail == null)
                return null;

            bool senhaValida = BCryptUtils.ValidaHashBcrypt(senha, usuarioDoEmail.Senha);
            if (!senhaValida)
                return null;

            return new SessaoViewModel
            {
                IdUsuario = usuarioDoEmail.Id,
                PerfilAcesso = usuarioDoEmail.PerfilAcesso
            };
        }

        public UsuarioViewModel ObtemPorEmail(string email)
        {
            FiltrosViewModel filtroEmail = new FiltrosViewModel { Email = email };
            List<UsuarioViewModel> usuariosFiltrados = ConsultaComFiltros(filtroEmail);
            if (usuariosFiltrados.Count == 0)
                return null;
            return usuariosFiltrados[0];
        }

        protected override UsuarioViewModel MontaModel(DataRow row)
        {
            return new UsuarioViewModel()
            {
                Id = Convert.ToInt32(row["idUsuario"]),
                Nome = row["nomeUsuario"].ToString(),
                Email = row["emailUsuario"].ToString(),
                Senha = row["senhaUsuario"].ToString(),
                PerfilAcesso = (PerfisAcesso)Convert.ToInt32(row["idPerfilAcesso"]),
            };
        }
        protected override void SetTabela()
        {
            Tabela = "usuarios";
        }
    }
}
