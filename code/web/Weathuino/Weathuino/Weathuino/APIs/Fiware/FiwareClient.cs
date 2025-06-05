using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Weathuino.APIs.Fiware.Models;
using Weathuino.APIs.Fiware.Payloads;

namespace Weathuino.APIs.Fiware
{
    // Classe para lidar com as requisições feitas ao Fiware
    public class FiwareClient
    {
        /// <summary>
        /// Registra um novo dispositivo no Iot Agent do Fiware
        /// </summary>
        /// <param name="deviceID">ID do dispositivo no fiware</param>
        /// <param name="entityNameID">ID numérico que compõe o parâmetro entityName do Fiware</param>
        /// <returns>FiwareOutput (status da operação e os dados/erros de retorno</returns>
        public FiwareOutput CriaDispositivo(string deviceID, int entityNameID)
        {
            var payloadNovoIoT = new PayloadNovoIoT
            {
                DeviceID = deviceID,
                EntityNameID = entityNameID
            };
            
            string payload = payloadNovoIoT.Monta();
            var request = MontaPostRequest(Constantes.URL_NOVO_IOT, payload);
            using var response = ExecutaRequisicao(request);

            return MontaOutput(response);
        }

        /// <summary>
        /// Registra os atributos a serem monitorados no dispositivo do Fiware
        /// </summary>
        /// <param name="entityNameID">id numérico que compõem o parâmetro entityName do Fiware</param>
        /// <returns>FiwareOutput (status da operação e os dados/erros de retorno</returns>
        public FiwareOutput RegistraAtributosDeDispositivo(int entityNameID)
        {
            var registroAtributos = new PayloadRegistroAtributos { EntityNameID = entityNameID };
            string payload = registroAtributos.Monta();
            var request = MontaPostRequest(Constantes.URL_REGISTRO_ATRIBUTOS, payload);
            using var response = ExecutaRequisicao(request);
            return MontaOutput(response);
        }

        /// <summary>
        /// Monta uma requisição do tipo POST utilizada nos métodos principais
        /// </summary>
        /// <param name="url">url requisitada</param>
        /// <param name="payload">dados a serem enviados</param>
        /// <returns>Objeto da requisição montada</returns>
        private HttpRequestMessage MontaPostRequest(string url, string payload)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            ConfiguraHeaders(request);
            return request;
        }

        /// <summary>
        /// Deleta um dispositivo no Iot Agent do Fiware
        /// </summary>
        /// <param name="deviceID">ID do dispositivo no Fiware</param>
        /// <returns>FiwareOutput (status da operação e os dados/erros de retorno</returns>
        public FiwareOutput DeletaDispositivoNoAgentMQTT(string deviceID)
        {
            string url = $"{Constantes.URL_EXCLUSAO_DISPOSITIVO_AGENT_MQTT}/{deviceID}";
            var request = MontaDeleteRequest(url);
            using var response = ExecutaRequisicao(request);
            return MontaOutput(response);
        }

        /// <summary>
        /// Monta uma requisição do tipo DELETE utilizada nos métodos principais
        /// </summary>
        /// <param name="url">url requisitada</param>
        /// <returns>Objeto da requisição montada</returns>
        private HttpRequestMessage MontaDeleteRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            ConfiguraHeaders(request);
            return request;
        }

        /// <summary>
        /// Obtém os últimos registros enviados por um dispositivo no Fiware
        /// </summary>
        /// <param name="entityNameID">id numérico que compõe o entityName de um dispositivo</param>
        /// <returns>FiwareOutput (status da operação e os dados/erros de retorno</returns>
        public FiwareOutput ObtemUltimosDadosDeDispositivo(int entityNameID)
        {
            string url = Constantes.URL_DADOS_DISPOSITIVO(entityNameID);
            var request = MontaGetRequest(url);
            using var response = ExecutaRequisicao(request);
            return MontaOutput(response);
        }

        /// <summary>
        /// Obtém os dados registrados por um dispositivo em um dado períod
        /// </summary>
        /// <param name="entityNameID">id numérico que compõe o entityname de um dispositivo</param>
        /// <param name="dataInicio">data de início</param>
        /// <param name="dataFim">data de fim</param>
        /// <returns>FiwareOutput (status da operação e os dados/erros de retorno</returns>
        public FiwareOutput ObtemDadosDeDispositivoEmUmPeriodo(int entityNameID, DateTime dataInicio, DateTime dataFim)
        {
            string url = Constantes.URL_DADOS_DISPOSITIVO_POR_PERIODO(entityNameID, dataInicio, dataFim);
            var request = MontaGetRequest(url);
            using var response = ExecutaRequisicao(request);
            return MontaOutput(response);
        }

        /// <summary>
        /// Obtém os dados instantâneos de um dispositivo no Fiware
        /// </summary>
        /// <param name="entityNameID">id numérico que compõe o entityname de um dispositivo</param>
        /// <returns>FiwareOutput (status da operação e os dados/erros de retorno</returns>
        public FiwareOutput ObtemDadosAtuaisDeDispositivo(int entityNameID)
        {
            string url = Constantes.URL_DADOS_ATUAIS_DISPOSITIVO(entityNameID);
            var request = MontaGetRequest(url);
            using var response = ExecutaRequisicao(request);
            return MontaOutput(response);
        }

        /// <summary>
        /// Monta uma requisição GET utilizada nos métodos principais
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private HttpRequestMessage MontaGetRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            ConfiguraHeaders(request);
            return request;
        }

        /// <summary>
        /// Configura os Headers padrão para requisições do Fiware
        /// </summary>
        /// <param name="request"></param>
        private static void ConfiguraHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("fiware-service", "smart");
            request.Headers.Add("fiware-servicepath", "/");
        }

        /// <summary>
        /// Executa as requisições HTTP criadas
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Objeto de retorno da requisição</returns>
        private static HttpResponseMessage ExecutaRequisicao(HttpRequestMessage request)
        {
            using var httpClient = new HttpClient(new HttpClientHandler());
            return httpClient.SendAsync(request).Result;
        }

        /// <summary>
        /// Monta o objeto de retorno do FiwareClient, indicando sucesso ou erro em uma requisição
        /// </summary>
        /// <param name="response">objeto de retorno da requisição HTTP associada</param>
        /// <returns>Objeto indicando o status da requisição e seus dados</returns>
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
