using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using Weathuino.Enums;
using Weathuino.Models;
using Newtonsoft.Json;
using Weathuino.APIs.Fiware;
using Weathuino.APIs.Fiware.Models;
using Weathuino.DAO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Weathuino.Controllers
{
    /// <summary>
    /// Controller que gerenciar os dashboards do sistema
    /// </summary>
    public class DashboardsController : PadraoController
    {
        public DashboardsController()
        {
            AcessoExigido = PerfisAcesso.COMUM; // acessível no mínimo por usuários logados
        }

        /// <summary>
        /// Carrega o dashboard que busca os dados do Fiware em tempo real
        /// </summary>
        /// <returns></returns>
        public IActionResult DashboardTempoReal()
        {
            try
            {
                PreparaComboEstufas();
                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        /// <summary>
        /// Carrega o dashboard para busca de dados históricos
        /// </summary>
        /// <returns></returns>
        public IActionResult DashboardHistorico()
        {
            try
            {
                PreparaComboEstufas();
                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel(ex.ToString()));
            }
        }

        /// <summary>
        /// Prepara o ComboBox para se selecionar a Estufa que será analisada no Dashboard
        /// </summary>
        private void PreparaComboEstufas()
        {
            EstufaDAO dao = new EstufaDAO();
            List<EstufaViewModel> estufas = dao.ObtemTodos();
            List<SelectListItem> listaEstufas = new List<SelectListItem>();

            listaEstufas.Add(new SelectListItem("Selecione uma estufa...", "0"));

            foreach (EstufaViewModel estufa in estufas)
            {
                SelectListItem item = new SelectListItem(estufa.Nome, estufa.Id.ToString());
                listaEstufas.Add(item);
            }

            // Carrega-se as estufas em JSON na ViewBag para que sejam manipuladas mais facilmente no JavaScript
            ViewBag.Estufas = listaEstufas;
            ViewBag.JsonEstufas = JsonConvert.SerializeObject(estufas);
        }

        public JsonResult DadosHistoricosDispositivo()
        {
            try
            {
                //Carrega os dados do dispositivo passados via QueryParams
                int entityNameID = int.Parse(Request.Query["entityNameID"]);
                DateTime dataInicio = Convert.ToDateTime(Request.Query["dataInicio"]);
                DateTime dataFim = Convert.ToDateTime(Request.Query["dataFim"]);
                FiwareClient fClient = new FiwareClient();
                FiwareOutput dados;

                // se são informados data de início e fim da busca, carrega-se os dados nesse período
                if (dataInicio != DateTime.MinValue && dataFim != DateTime.MinValue)
                {
                    dados = fClient.ObtemDadosDeDispositivoEmUmPeriodo(entityNameID, dataInicio, dataFim);
                }
                else // se não se carrega os últimos registros associados ao dispositivo
                {
                    dados = fClient.ObtemUltimosDadosDeDispositivo(entityNameID);
                }

                return Json(dados.Dados);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception ao consultar Fiware: {ex.StackTrace}");
                return Json("[]");
            }
        }

        /// <summary>
        /// Carrega os dados instantâneos do dispotivo para exibição no dashboard
        /// </summary>
        /// <returns></returns>
        public JsonResult DadosAtuaisDispositivo()
        {
            try
            {
                // carrega dados via QueryParams
                int entityNameID = int.Parse(Request.Query["entityNameID"]);

                FiwareClient fClient = new FiwareClient();
                FiwareOutput dados = fClient.ObtemDadosAtuaisDeDispositivo(entityNameID);
                return Json(dados.Dados);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception ao consultar Fiware: {ex.StackTrace}");
                return Json("[]");
            }
        }
    }
}
