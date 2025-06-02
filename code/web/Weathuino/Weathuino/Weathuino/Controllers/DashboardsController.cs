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

namespace Weathuino.Controllers
{
    public class DashboardsController : PadraoController
    {
        public DashboardsController()
        {
            AcessoExigido = PerfisAcesso.COMUM;
        }

        public IActionResult Dashboard1()
        {
            return View();
        }

        public IActionResult Dashboard2()
        {
            return View();
        }

        public JsonResult GetTemperatureData()
        {
            var rng = new Random();
            var now = DateTimeOffset.UtcNow;

            var data = new List<DadosTemperatura>();
            for (float i = 10; i >= 0; i -= 0.1f)
            { // −2x^2 + 4x + 1
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var temp = Math.Round(20 + rng.NextDouble() * 10, 2);
                data.Add(new DadosTemperatura { Timestamp = timestamp, Temperature = temp });
                Thread.Sleep(100);
            }

            Console.WriteLine($"tamanho da bitola: {data.Count}");

            return Json("{\"contextResponses\":[{\"contextElement\":{\"attributes\":[{\"name\":\"luminosidade\",\"values\":[{\"_id\":\"682fb32a141d41000788a1e5\",\"recvTime\":\"2025-05-22T23:28:42.747Z\",\"attrName\":\"luminosidade\",\"attrType\":\"Integer\",\"attrValue\":43},{\"_id\":\"682fb32b141d41000788a1e9\",\"recvTime\":\"2025-05-22T23:28:43.814Z\",\"attrName\":\"luminosidade\",\"attrType\":\"Integer\",\"attrValue\":43}]}],\"id\":\"urn:ngsi-ld:Sensor:1\",\"isPattern\":false,\"type\":\"Sensor\"},\"statusCode\":{\"code\":\"200\",\"reasonPhrase\":\"OK\"}}]}");
        }

        public JsonResult ProxyToExternal()
        {
            FiwareClient fClient = new FiwareClient();
            FiwareOutput dados = fClient.ObtemUltimosDadosDeDispositivo(5);
            return Json(dados.Dados);
        }
    }
}
