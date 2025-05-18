using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Weathuino.Models;

namespace Weathuino.DAO
{
    public class MedidorDAO : PadraoDAO<MedidorViewModel>
    {
        public void DeletaPorID(int id)
        {
            HelperDAO.ExecutaProc("spDelete", CriaParametrosDelete("medidores", id));
        }

        public bool VerificaSeMedidorEstaEmUso(int id)
        {
            DataTable dados = HelperDAO.ExecutaProcSelect("spVerificaUsoMedidor", CriaParametrosConsulta(id));
            if (dados.Rows.Count == 0) return false;
            bool estaEmUso = Convert.ToBoolean(dados.Rows[0]["estaEmUso"]);
            return estaEmUso;
        }

        protected override SqlParameter[] CriaParametros(MedidorViewModel medidor)
        {
            return new SqlParameter[]
            {
                new SqlParameter("id", medidor.Id),
                new SqlParameter("nome", medidor.Nome)
            };
        }


        private SqlParameter[] CriaParametrosDelete(string nomeTabela, int idRegistro)
        {
            return new SqlParameter[]
            {
                new SqlParameter("tabela", nomeTabela),
                new SqlParameter("id", idRegistro)
            };
        }

        private SqlParameter[] CriaParametrosConsulta(int id = 0)
        {
            if (id == 0)
                return new SqlParameter[] { new SqlParameter("id", DBNull.Value) };
            else
                return new SqlParameter[] { new SqlParameter("id", id) };
        }

        protected override MedidorViewModel MontaModel(DataRow row)
        {
            MedidorViewModel medidor = new MedidorViewModel()
            {
                Id = Convert.ToInt32(row["idMedidor"]),
                Nome = row["nomeMedidor"].ToString(),
            };
            
            if (row["ultimoRegistroMedidor"] != DBNull.Value)
                medidor.DataUltimoRegistro = Convert.ToDateTime(row["ultimoRegistroMedidor"]);
            return medidor;
        }
        protected override void SetTabela()
        {
            Tabela = "medidores";
        }
    }
}
