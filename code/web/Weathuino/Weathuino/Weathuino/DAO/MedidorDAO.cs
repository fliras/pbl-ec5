using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Weathuino.Models;

namespace Weathuino.DAO
{
    public class MedidorDAO : PadraoDAO<MedidorViewModel>
    {
        public bool VerificaSeMedidorEstaEmUso(int id)
        {
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("id", id) };
            DataTable dados = HelperDAO.ExecutaProcSelect("spVerificaUsoMedidor", parametros);
            if (dados.Rows.Count == 0) return false;
            bool estaEmUso = Convert.ToBoolean(dados.Rows[0]["estaEmUso"]);
            return estaEmUso;
        }

        protected override SqlParameter[] CriaParametros(MedidorViewModel medidor)
        {
            return new SqlParameter[]
            {
                new SqlParameter("id", medidor.Id),
                new SqlParameter("nome", medidor.Nome),
                new SqlParameter("deviceIdFiware", medidor.DeviceIdFiware)
            };
        }

        protected override MedidorViewModel MontaModel(DataRow row)
        {
            MedidorViewModel medidor = new MedidorViewModel()
            {
                Id = Convert.ToInt32(row["idMedidor"]),
                Nome = row["nomeMedidor"].ToString(),
                DeviceIdFiware = row["deviceIdFiware"].ToString()
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
