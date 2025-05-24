using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Weathuino.Models;

namespace Weathuino.DAO
{
    public class EstufaDAO : PadraoDAO<EstufaViewModel>
    {
        protected override SqlParameter[] CriaParametros(EstufaViewModel estufa)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("id", estufa.Id),
                new SqlParameter("nome", estufa.Nome),
                new SqlParameter("descricao", estufa.Descricao),
                new SqlParameter("idMedidor", estufa.Medidor.Id)
            };

            if (estufa.TemperaturaMinima != null)
                parametros.Add(new SqlParameter("temperaturaMin", estufa.TemperaturaMinima));

            if (estufa.TemperaturaMaxima != null)
                parametros.Add(new SqlParameter("temperaturaMax", estufa.TemperaturaMaxima));

            return parametros.ToArray();
        }

        protected override EstufaViewModel MontaModel(DataRow row)
        {
            EstufaViewModel estufa = new EstufaViewModel()
            {
                Id = Convert.ToInt32(row["idEstufa"]),
                Nome = row["nomeEstufa"].ToString(),
                Descricao = row["descricaoEstufa"].ToString(),
                Medidor = new MedidorViewModel()
                {
                    Id = Convert.ToInt32(row["idMedidor"]),
                    Nome = row["nomeMedidor"].ToString()
                }
            };

            if (row["tempMinEstufa"] != DBNull.Value)
                estufa.TemperaturaMinima = Convert.ToSingle(row["tempMinEstufa"]);
            if (row["tempMaxEstufa"] != DBNull.Value)
                estufa.TemperaturaMaxima = Convert.ToSingle(row["tempMaxEstufa"]);

            return estufa;
        }
        protected override void SetTabela()
        {
            Tabela = "estufas";
        }

    }
}
