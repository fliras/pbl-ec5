using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Weathuino.Models;
using Weathuino.Enums;
using System.Collections.Generic;
using Weathuino.Utils;

namespace Weathuino.DAO
{
    /// <summary>
    /// Gerencia as operações com usuários no BD
    /// </summary>
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

        /// <summary>
        /// Realiza valida os dados de login do usuário e retorna seus dados de sessão.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        public SessaoViewModel RealizaLogin(string email, string senha)
        {
            UsuarioViewModel usuarioDoEmail = ObtemPorEmail(email);
            if (usuarioDoEmail == null)
                return null;

            // valida a senha comparando-a com o hash armazenado no BD
            bool senhaValida = BCryptUtils.ValidaHashBcrypt(senha, usuarioDoEmail.Senha);
            if (!senhaValida)
                return null;

            return new SessaoViewModel
            {
                IdUsuario = usuarioDoEmail.Id,
                PerfilAcesso = usuarioDoEmail.PerfilAcesso
            };
        }

        /// <summary>
        /// Obtem um usuário com base no seu e-mail
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
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
