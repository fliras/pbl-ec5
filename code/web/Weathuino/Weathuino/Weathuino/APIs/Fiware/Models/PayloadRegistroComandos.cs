using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Weathuino.APIs.Fiware.Payloads
{
    public class PayloadRegistroComandos
    {
        public int EntityNameID { get; set; }

        public string Monta()
        {
            JObject payload = new JObject
            {
                ["description"] = "Sensor Commands",
                ["dataProvided"] = new JObject
                {
                    ["entities"] = new JArray
                    {
                        new JObject
                        {
                            ["id"] = $"urn:ngsi-ld:Sensor:{EntityNameID}",
                            ["type"] = "Sensor"
                        }
                    },
                    ["attrs"] = new JArray { "on", "off" }
                },
                ["provider"] = new JObject
                {
                    ["http"] = new JObject
                    {
                        ["url"] = $"http://{Constantes.IP_SERVIDOR}:4041"
                    },
                    ["legacyForwarding"] = true
                }
            };

            return JsonConvert.SerializeObject(payload);
        }
    }
}
