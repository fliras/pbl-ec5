using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System;
using Weathuino.Models;
using Newtonsoft.Json.Linq;
using Weathuino.Utils;

namespace Weathuino.DAO
{
    /// <summary>
    /// Classe base para DAOs utilizadas pelos controllers de CRUD
    /// </summary>
    /// <typeparam name="T">Model associada a cada DAO especifica</typeparam>
    public abstract class PadraoDAO<T> where T : PadraoViewModel
    {
        public PadraoDAO()
        {
            SetTabela(); // especifica a tabela no BD gerenciada pela DAO
        }

        protected string Tabela { get; set; } // tabela associada a DAO
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
        
        /// <summary>
        /// Obtém registros por meio de seu ID no BD
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T ObtemPorID(int id)
        {
            FiltrosViewModel filtroId = new FiltrosViewModel { Id = id }; // monta uma model para pesquisar pelo campo ID do registro
            List<T> registrosFiltrados = ConsultaComFiltros(filtroId);
            
            if (registrosFiltrados.Count == 0)
                return null;

            return registrosFiltrados[0];
        }

        /// <summary>
        /// Método para pesquisar registros no BD com base em uma série de filtros (opcionais)
        /// </summary>
        /// <param name="filtros">Model com os filtros com os quais os registros podem ser filtrados</param>
        /// <returns></returns>
        public List<T> ConsultaComFiltros(FiltrosViewModel filtros)
        {
            // cria um objeto JSON com base no Model de filtros, para iterá-lo campo a campo e identificar
            // o que deve ser tratado como parâmetro de pesquisa na procedure de consulta do BD.
            List<SqlParameter> parametros = new List<SqlParameter>();
            JObject jsonDeFiltros = JSONUtils.ConverteObjetoParaJObject(filtros);
            foreach (var filtro in jsonDeFiltros.Properties())
            {
                object valorFiltro = filtro.Value.ToObject<object>();
                if (valorFiltro != null)
                    parametros.Add(new SqlParameter(filtro.Name, valorFiltro));
            }

            // Executa a requisição e obtém os registros
            var tabela = HelperDAO.ExecutaProcSelect($"spConsulta_{Tabela}", parametros.ToArray());
            List<T> registros = new List<T>();

            // Havendo dados, os dados são mapeados no Model respectivo
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

