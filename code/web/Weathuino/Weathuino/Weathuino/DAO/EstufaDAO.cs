using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Weathuino.Models;

namespace Weathuino.DAO
{
    public class EstufaDAO: PadraoDAO<EstufaViewModel>
    {

        protected override SqlParameter[] CriaParametros(EstufaViewModel estufa)
        {
            SqlParameter[] parametros = new SqlParameter[6];
            parametros[0] = new SqlParameter("id", estufa.Id);
            parametros[1] = new SqlParameter("nome", estufa.Nome);
            parametros[2] = new SqlParameter("descricao", estufa.Descricao);

            if (estufa.TemperaturaMinima != null)
                parametros[3] = new SqlParameter("temperaturaMin", estufa.TemperaturaMinima);
            else
                parametros[3] = new SqlParameter("temperaturaMin", DBNull.Value);

            if (estufa.TemperaturaMaxima != null)
                parametros[4] = new SqlParameter("temperaturaMax", estufa.TemperaturaMaxima);
            else
                parametros[4] = new SqlParameter("temperaturaMax", DBNull.Value);

            parametros[5] = new SqlParameter("idMedidor", estufa.Medidor.Id);

            return parametros;
        }

        private SqlParameter[] CriaParametrosConsulta(int id = 0)
        {
            if (id == 0)
                return new SqlParameter[] { new SqlParameter("id", DBNull.Value) };
            else
                return new SqlParameter[] { new SqlParameter("id", id) };
        }

        private SqlParameter[] CriaParametrosDelete(string nomeTabela, int idRegistro)
        {
            return new SqlParameter[]
            {
                new SqlParameter("tabela", nomeTabela),
                new SqlParameter("id", idRegistro)
            };
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
