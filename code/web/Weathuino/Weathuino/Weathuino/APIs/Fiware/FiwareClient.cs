using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Weathuino.APIs.Fiware.Models;
using Weathuino.APIs.Fiware.Payloads;

namespace Weathuino.APIs.Fiware
{
    public class FiwareClient
    {
        public FiwareOutput CriaDispositivo(string deviceID, int entityNameID)
        {
            var payloadNovoIoT = new PayloadNovoIoT
            {
                DeviceID = deviceID,
                EntityNameID = entityNameID
            };
            
            string payload = payloadNovoIoT.Monta();
            var request = MontaPostRequest(Constantes.URL_NOVO_IOT, payload);
            var response = ExecutaRequisicao(request);

            return MontaOutput(response);
        }

        public FiwareOutput RegistraAtributosDeDispositivo(int entityNameID)
        {
            var registroAtributos = new PayloadRegistroAtributos { EntityNameID = entityNameID };
            string payload = registroAtributos.Monta();
            var request = MontaPostRequest(Constantes.URL_REGISTRO_ATRIBUTOS, payload);
            var response = ExecutaRequisicao(request);
            return MontaOutput(response);
        }

        private HttpRequestMessage MontaPostRequest(string url, string payload)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            ConfiguraHeaders(request);
            return request;
        }

        public FiwareOutput DeletaDispositivoNoAgentMQTT(string deviceID)
        {
            string url = $"{Constantes.URL_EXCLUSAO_DISPOSITIVO_AGENT_MQTT}/{deviceID}";
            var request = MontaDeleteRequest(url);
            var response = ExecutaRequisicao(request);
            return MontaOutput(response);
        }

        private HttpRequestMessage MontaDeleteRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            ConfiguraHeaders(request);
            return request;
        }

        public FiwareOutput ObtemUltimosDadosDeDispositivo(int entityNameID)
        {
            string url = Constantes.URL_DADOS_DISPOSITIVO(entityNameID);
            var request = MontaGetRequest(url);
            var response = ExecutaRequisicao(request);
            return MontaOutput(response);
        }

        public FiwareOutput ObtemDadosDeDispositivoEmUmPeriodo(int entityNameID, DateTime dataInicio, DateTime dataFim)
        {
            string url = Constantes.URL_DADOS_DISPOSITIVO_POR_PERIODO(entityNameID, dataInicio, dataFim);
            var request = MontaGetRequest(url);
            var response = ExecutaRequisicao(request);
            return MontaOutput(response);
        }

        private HttpRequestMessage MontaGetRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            ConfiguraHeaders(request);
            return request;
        }

        private static void ConfiguraHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("fiware-service", "smart");
            request.Headers.Add("fiware-servicepath", "/");
        }

        private static HttpResponseMessage ExecutaRequisicao(HttpRequestMessage request)
        {
            using var httpClient = new HttpClient(new HttpClientHandler());
            using var response = httpClient.SendAsync(request).Result;
            return response;
        }

        private FiwareOutput MontaOutput(HttpResponseMessage response)
        {
            List<HttpStatusCode> statusAceitos = new List<HttpStatusCode> {
                HttpStatusCode.OK,
                HttpStatusCode.NoContent,
                HttpStatusCode.Created
            };

            if (statusAceitos.Contains(response.StatusCode))
            {
                return new FiwareOutput
                {
                    Sucesso = true,
                    Dados = response.Content.ReadAsStringAsync().Result,
                };
            }

            return new FiwareOutput
            {
                Sucesso = false,
                MensagemDeErro = $"Erro na comunicação com o Fiware ao acessar: {response.RequestMessage.RequestUri}"
            };
        }
    }
}
