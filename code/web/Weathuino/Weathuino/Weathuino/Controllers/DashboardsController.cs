using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using Weathuino.Enums;
using Weathuino.Models;
using System.Threading;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Weathuino.APIs.Fiware;
using Weathuino.APIs.Fiware.Models;
using Weathuino.DAO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Weathuino.Controllers
{
    public class DashboardsController : PadraoController
    {
        public DashboardsController()
        {
            AcessoExigido = PerfisAcesso.COMUM;
        }

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
            ViewBag.Estufas = listaEstufas;
            ViewBag.JsonEstufas = JsonConvert.SerializeObject(estufas);
        }

        public JsonResult ProxyToExternal()
        {
            try
            {
                int entityNameID = int.Parse(Request.Query["entityNameID"]);
                DateTime dataInicio = Convert.ToDateTime(Request.Query["dataInicio"]);
                DateTime dataFim = Convert.ToDateTime(Request.Query["dataFim"]);
                FiwareClient fClient = new FiwareClient();
                FiwareOutput dados;

                if (dataInicio != DateTime.MinValue && dataFim != DateTime.MinValue)
                {
                    dados = fClient.ObtemDadosDeDispositivoEmUmPeriodo(entityNameID, dataInicio, dataFim);
                }
                else
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

        public JsonResult DadosAtuaisDispositivo()
        {
            try
            {
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
