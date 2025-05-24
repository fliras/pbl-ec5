using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System;
using Weathuino.Models;
using Newtonsoft.Json.Linq;

namespace Weathuino.DAO
{
    public abstract class PadraoDAO<T> where T : PadraoViewModel
    {
        public PadraoDAO()
        {
            SetTabela();
        }

        protected string Tabela { get; set; }
        protected abstract SqlParameter[] CriaParametros(T model);
        protected abstract T MontaModel(DataRow registro);
        protected abstract void SetTabela();

        public virtual void Insert(T model)
        {
            HelperDAO.ExecutaProc("spInsere_" + Tabela, CriaParametros(model));
        }
        public virtual void Update(T model)
        {
            HelperDAO.ExecutaProc("spAltera_" + Tabela, CriaParametros(model));
        }
        public virtual void Delete(int id)
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("id", id),
                new SqlParameter("tabela", Tabela)
            };
            HelperDAO.ExecutaProc("spDelete", p);
        }
        public virtual T ObtemPorID(int id)
        {
            FiltrosViewModel filtroId = new FiltrosViewModel { Id = id };
            List<T> registrosFiltrados = ConsultaComFiltros(filtroId);
            
            if (registrosFiltrados.Count == 0)
                return null;

            return registrosFiltrados[0];
        }

        public List<T> ConsultaComFiltros(FiltrosViewModel filtros)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            JObject jsonDeFiltros = JObject.FromObject(filtros);
            foreach (var filtro in jsonDeFiltros.Properties())
            {
                object valorFiltro = filtro.Value.ToObject<object>();
                if (valorFiltro != null)
                    parametros.Add(new SqlParameter(filtro.Name, valorFiltro));
            }

            var tabela = HelperDAO.ExecutaProcSelect($"spConsulta_{Tabela}", parametros.ToArray());
            List<T> registros = new List<T>();

            if (tabela.Rows.Count > 0)
            {
                foreach (DataRow row in tabela.Rows)
                {
                    registros.Add(MontaModel(row));
                }
            }

            return registros;
        }

        public virtual List<T> ObtemTodos()
        {
            var tabela = HelperDAO.ExecutaProcSelect($"spConsulta_{Tabela}");
            List<T> lista = new List<T>();
            foreach (DataRow registro in tabela.Rows)
                lista.Add(MontaModel(registro));

            return lista;
        }

        public virtual int GeraProximoID()
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("tabela", Tabela)
            };
            var tabela = HelperDAO.ExecutaProcSelect("spProximoId", p);
            return Convert.ToInt32(tabela.Rows[0][0]);
        }
    }
}

