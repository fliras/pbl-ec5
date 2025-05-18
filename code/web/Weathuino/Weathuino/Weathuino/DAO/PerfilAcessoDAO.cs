using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Weathuino.Models;

namespace Weathuino.DAO
{
    public class PerfilAcessoDAO : PadraoDAO<PerfilAcessoViewModel>
    {
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
            Tabela = "perfisAcesso";
        }
    }
}
