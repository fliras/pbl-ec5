using System;

namespace Weathuino.APIs.Fiware
{
    // Constantes utilizadas no contexto do Fiware
    public class Constantes
    {
        // IP do servidor onde o Fiware está instalado
        public static readonly string IP_SERVIDOR = "52.1.120.205";

        // Endpoints do Fiware
        public static readonly string URL_NOVO_IOT = $"http://{IP_SERVIDOR}:4041/iot/devices";
        public static readonly string URL_REGISTRO_ATRIBUTOS = $"http://{IP_SERVIDOR}:1026/v2/subscriptions";
        public static readonly string URL_EXCLUSAO_DISPOSITIVO_AGENT_MQTT = $"http://{IP_SERVIDOR}:4041/iot/devices";
        public static readonly string URL_EXCLUSAO_DISPOSITIVO_ORION = $"http://{IP_SERVIDOR}:1026/v2/entities";

        // Método auxiliar para montar o parâmetro entiyName a partir de um ID numérico
        public static string ENTITY_NAME(int entityNameID)
        {
            return $"urn:ngsi-ld:Sensor:{entityNameID}";
        }

        // Método auxiliar para montar uma URL do Fiware
        public static string URL_DADOS_DISPOSITIVO_POR_PERIODO(int entityNameID, DateTime inicio, DateTime fim)
        {
            string dInicio = inicio.ToString("yyyy-MM-dd");
            string dFim = $"{fim.ToString("yyyy-MM-dd")}T23:59:59";
            return $"{URL_DADOS_DISPOSITIVO(entityNameID)}&dateFrom={dInicio}&dateTo={dFim}";
        }

        // Método auxiliar para montar uma URL do Fiware
        public static string URL_DADOS_DISPOSITIVO(int entityNameID)
        {
            string entityName = ENTITY_NAME(entityNameID);
            return $"http://{IP_SERVIDOR}:8666/STH/v1/contextEntities/type/Sensor/id/{entityName}/attributes/temperatura?lastN=100";
        }

        // Método auxiliar para montar uma URL do Fiware
        public static string URL_DADOS_ATUAIS_DISPOSITIVO(int entityNameID)
        {
            string entityName = ENTITY_NAME(entityNameID);
            return $"http://{IP_SERVIDOR}:1026/v2/entities/{entityName}";
        }
    }
}
