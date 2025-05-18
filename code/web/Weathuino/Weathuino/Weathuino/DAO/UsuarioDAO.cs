using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Weathuino.Models;

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
                new SqlParameter("idPerfil", usuario.PerfilAcesso.Id)
            };
        }

        private string CriaHashBcrypt(string texto)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(texto, 13);
        }

        protected override UsuarioViewModel MontaModel(DataRow row)
        {
            return new UsuarioViewModel()
            {
                Id = Convert.ToInt32(row["idUsuario"]),
                Nome = row["nomeUsuario"].ToString(),
                Email = row["emailUsuario"].ToString(),
                Senha = row["senhaUsuario"].ToString(),
                PerfilAcesso = new PerfilAcessoViewModel()
                {
                    Id = Convert.ToInt32(row["idPerfilAcesso"]),
                    Nome = row["nomePerfilAcesso"].ToString()
                }
            };
        }
        protected override void SetTabela()
        {
            Tabela = "usuarios";
        }
    }
}
