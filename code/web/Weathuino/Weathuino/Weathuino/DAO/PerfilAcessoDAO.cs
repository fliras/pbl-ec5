using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Weathuino.Models;

namespace Weathuino.DAO
{
    public class PerfilAcessoDAO : PadraoDAO<PerfilAcessoViewModel>
    {

        private SqlParameter[] CriaParametrosConsulta(int id = 0)
        {
            if (id == 0)
                return new SqlParameter[] { new SqlParameter("id", DBNull.Value) };
            else
                return new SqlParameter[] { new SqlParameter("id", id) };
        }
        protected override SqlParameter[] CriaParametros(PerfilAcessoViewModel Perfil)
        {
            return new SqlParameter[]
            {
                new SqlParameter("id", Perfil.Id),
                new SqlParameter("nome", Perfil.Nome)
            };
        }

        protected override PerfilAcessoViewModel MontaModel(DataRow row)
        {
            return new PerfilAcessoViewModel()
            {
                Id = Convert.ToInt32(row["idPerfilAcesso"]),
                Nome = row["nomePerfilAcesso"].ToString(),
            };
        }
        protected override void SetTabela()
        {
            Tabela = "perfil";
        }
    }
}
