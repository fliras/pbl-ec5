using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Weathuino.Models;
using Weathuino.Enums;
using System.Collections.Generic;

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
                new SqlParameter("senha", CriaHashBcrypt(usuario.Senha)),
                new SqlParameter("idPerfil", usuario.PerfilAcesso)
            };
        }

        private string CriaHashBcrypt(string texto)
        {
            return BCrypt.Net.BCrypt.HashPassword(texto);
        }

        private bool ValidaHashBcrypt(string senha, string hashSenha)
        {
            return BCrypt.Net.BCrypt.Verify(senha, hashSenha);
        }

        public SessaoViewModel RealizaLogin(string email, string senha)
        {
            FiltrosUsuarioViewModel filtroEmail = new FiltrosUsuarioViewModel { Email = email };
            List<UsuarioViewModel> usuariosFiltrados = ConsultaComFiltros(filtroEmail);
            if (usuariosFiltrados.Count == 0)
                return null;

            UsuarioViewModel usuarioDoEmail = usuariosFiltrados[0];
            bool senhaValida = ValidaHashBcrypt(senha, usuarioDoEmail.Senha);
            if (!senhaValida)
                return null;

            return new SessaoViewModel
            {
                IdUsuario = usuarioDoEmail.Id,
                PerfilAcesso = usuarioDoEmail.PerfilAcesso
            };
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
