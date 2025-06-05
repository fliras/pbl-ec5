using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Weathuino.APIs.Fiware.Payloads
{
    // Model que monta o JSON enviado via POST na requisição do Fiware para cadastrar dispositivos
    public class PayloadNovoIoT
    {
        public string DeviceID { get; set; }
        public int EntityNameID { get; set; }

        // Monta um objeto JSON com base nos dados passados e retorna o JSON serializado
        public string Monta()
        {
            JObject payload = new JObject
            {
                ["devices"] = new JArray
                {
                    new JObject
                    {
                        ["device_id"] = DeviceID,
                        ["entity_name"] = $"urn:ngsi-ld:Sensor:{EntityNameID}",
                        ["entity_type"] = "Sensor",
                        ["protocol"] = "PDI-IoTA-UltraLight",
                        ["transport"] = "MQTT",
                        ["commands"] = new JArray
                        {
                            new JObject { ["name"] = "on", ["type"] = "command" },
                            new JObject { ["name"] = "off", ["type"] = "command" }
                        },
                        ["attributes"] = new JArray
                        {
                            new JObject { ["object_id"] = "t", ["name"] = "temperatura", ["type"] = "Float" },
                        }
                    }
                }
            };

            return JsonConvert.SerializeObject(payload);
        }
    }
}
