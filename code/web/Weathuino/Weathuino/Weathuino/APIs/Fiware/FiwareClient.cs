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
        private Dictionary<string, string> _mensagensDeErroDasURLs = new Dictionary<string, string>()
        {
            {Constantes.URL_NOVO_IOT, "Não foi possível registrar o dispositivo"},
        };

        public FiwareOutput CriaDispositivo(string deviceID, int entityNameID)
        {
            var payloadNovoIoT = new PayloadNovoIoT
            {
                DeviceID = deviceID,
                EntityNameID = entityNameID
            };
            string payload = payloadNovoIoT.Monta();
            var request = MontaPostRequest(Constantes.URL_NOVO_IOT, payload);

            using var httpClient = new HttpClient(new HttpClientHandler());
            using var response = httpClient.SendAsync(request).Result;

            return MontaOutput(response);
        }

        public FiwareOutput RegistraComandosDeDispositivo(int entityNameID)
        {
            var registroComandos = new PayloadRegistroComandos { EntityNameID = entityNameID };
            string payload = registroComandos.Monta();
            var request = MontaPostRequest(Constantes.URL_REGISTRO_COMANDOS, payload);

            using var httpClient = new HttpClient(new HttpClientHandler());
            using var response = httpClient.SendAsync(request).Result;

            return MontaOutput(response);
        }

        public FiwareOutput RegistraAtributosDeDispositivo(int entityNameID)
        {
            var registroAtributos = new PayloadRegistroAtributos { EntityNameID = entityNameID };
            string payload = registroAtributos.Monta();
            var request = MontaPostRequest(Constantes.URL_REGISTRO_ATRIBUTOS, payload);

            using var httpClient = new HttpClient(new HttpClientHandler());
            using var response = httpClient.SendAsync(request).Result;

            return MontaOutput(response);
        }

        private HttpRequestMessage MontaPostRequest(string url, string payload)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(payload, Encoding.UTF8, "appplication/json");
            ConfiguraHeaders(request);
            return request;
        }

        public FiwareOutput DeletaDispositivoNoAgentMQTT(string deviceID)
        {
            string url = $"{Constantes.URL_EXCLUSAO_DISPOSITIVO_AGENT_MQTT}/{deviceID}";
            var request = MontaDeleteRequest(url);
            using var httpClient = new HttpClient(new HttpClientHandler());
            using var response = httpClient.SendAsync(request).Result;
            return MontaOutput(response);
        }

        public FiwareOutput DeletaDispositivoNoOrion(string entityName)
        {
            string url = $"{Constantes.URL_EXCLUSAO_DISPOSITIVO_AGENT_MQTT}/{entityName}";
            var request = MontaDeleteRequest(url);
            using var httpClient = new HttpClient(new HttpClientHandler());
            using var response = httpClient.SendAsync(request).Result;
            return MontaOutput(response);
        }

        private HttpRequestMessage MontaDeleteRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            ConfiguraHeaders(request);
            return request;
        }

        private static void ConfiguraHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("fiware-service", "smart");
            request.Headers.Add("fiware-servicepath", "/");
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
                MensagemDeErro = $"Erro na comunicação com o Fiware ao acessar: {response.RequestMessage.RequestUri}"
            };
        }
    }
}
