using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Weathuino.APIs.Fiware.Models;
using Weathuino.APIs.Fiware.Payloads;

namespace Weathuino.APIs.Fiware
{
    public class FiwareClient
    {
        private readonly HttpClientHandler _httpClientHandler;
        private PayloadNovoIoT _montaPayloadNovoIoT;
        private PayloadRegistroComandos _registroComandos;
        private PayloadRegistroAtributos _registroAtributos;

        public FiwareClient()
        {
            _httpClientHandler = new HttpClientHandler
            {
                //Proxy = new WebProxy
                //{
                //    Address = new Uri("http://proxycefsa.cefsa.corp.local:8080"),
                //    BypassProxyOnLocal = true,
                //    UseDefaultCredentials = true,
                //}
            };
        }

        public FiwareOutput CriaDispositivo(string deviceID, int entityNameID)
        {
            _montaPayloadNovoIoT = new PayloadNovoIoT
            {
                DeviceID = deviceID,
                EntityNameID = entityNameID
            };
            string payload = _montaPayloadNovoIoT.Monta();
            var request = MontaRequest(Constantes.URL_NOVO_IOT, payload);

            using var httpClient = new HttpClient(_httpClientHandler);
            using var response = httpClient.SendAsync(request).Result;

            return MontaOutput(response);
        }

        public FiwareOutput RegistraComandosDeDispositivo(int entityNameID)
        {
            _registroComandos = new PayloadRegistroComandos { EntityNameID = entityNameID };
            string payload = _registroComandos.Monta();
            var request = MontaRequest(Constantes.URL_REGISTRO_COMANDOS, payload);

            using var httpClient = new HttpClient(_httpClientHandler);
            using var response = httpClient.SendAsync(request).Result;

            return MontaOutput(response);
        }

        public FiwareOutput RegistraAtributosDeDispositivo(int entityNameID)
        {
            _registroAtributos = new PayloadRegistroAtributos { EntityNameID = entityNameID };
            string payload = _registroAtributos.Monta();
            var request = MontaRequest(Constantes.URL_REGISTRO_ATRIBUTOS, payload);

            using var httpClient = new HttpClient(_httpClientHandler);
            using var response = httpClient.SendAsync(request).Result;

            return MontaOutput(response);
        }

        private HttpRequestMessage MontaRequest(string url, string payload)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(payload, Encoding.UTF8, "appplication/json");
            request.Headers.Add("fiware-service", "smart");
            request.Headers.Add("fiware-servicepath", "/");
            return request;
        }

        private FiwareOutput MontaOutput(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
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
                MensagemDeErro = $"Erro ao enviar dados. Code: {response.StatusCode}"
            };
        }
    }
}
