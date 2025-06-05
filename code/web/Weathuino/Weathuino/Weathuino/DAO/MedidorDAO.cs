using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Weathuino.Models;

namespace Weathuino.DAO
{
    /// <summary>
    /// Gerencia operações com medidores no BD
    /// </summary>
    public class MedidorDAO : PadraoDAO<MedidorViewModel>
    {
        /// <summary>
        /// Verifica se um medidor está associado a uma estufa
        /// </summary>
        /// <param name="id">id do medidor</param>
        /// <returns></returns>
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

        /// <summary>
        /// Método para pesquisar um medidor pelo seu ID no Fiware
        /// </summary>
        /// <param name="deviceID">Device ID cadastrado no Fiware</param>
        /// <returns></returns>
        public MedidorViewModel ObtemPorDeviceID(string deviceID)
        {
            FiltrosViewModel filtroDeviceID = new FiltrosViewModel { DeviceID = deviceID }; // Prepara o model de filtro
            List<MedidorViewModel> medidoresFiltrados = ConsultaComFiltros(filtroDeviceID);
            if (medidoresFiltrados.Count == 0)
                return null;
            return medidoresFiltrados[0];
        }

        protected override void SetTabela()
        {
            Tabela = "medidores";
        }
    }
}
