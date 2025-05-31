namespace Weathuino.APIs.Fiware
{
    public class Constantes
    {
        public static readonly string IP_SERVIDOR = "";
        //public static readonly string URL_NOVO_IOT = $"http://{IP_SERVIDOR}:4041/iot/devices";
        public static readonly string URL_NOVO_IOT = $"https://testefiware.free.beeceptor.com";

        //public static readonly string URL_REGISTRO_COMANDOS = $"http://{IP_SERVIDOR}:1026/v2/registrations";
        public static readonly string URL_REGISTRO_COMANDOS = $"https://testefiware.free.beeceptor.com";

        //public static readonly string URL_REGISTRO_ATRIBUTOS = $"http://{IP_SERVIDOR}:1026/v2/subscriptions";
        public static readonly string URL_REGISTRO_ATRIBUTOS = $"https://testefiware.free.beeceptor.com";

        //public static readonly string URL_EXCLUSAO_DISPOSITIVO_AGENT_MQTT = $"http://{IP_SERVIDOR}:4041/iot/devices";
        public static readonly string URL_EXCLUSAO_DISPOSITIVO_AGENT_MQTT = $"https://testefiware.free.beeceptor.com";

        //public static readonly string URL_EXCLUSAO_DISPOSITIVO_ORION = $"http://{IP_SERVIDOR}:1026/v2/entities";
        public static readonly string URL_EXCLUSAO_DISPOSITIVO_ORION = $"https://testefiware.free.beeceptor.com";

        public static readonly string BASE_ENTITY_NAME = "urn:ngsi-ld:Sensor:";
    }
}
