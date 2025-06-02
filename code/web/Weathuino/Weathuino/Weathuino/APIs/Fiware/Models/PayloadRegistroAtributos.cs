using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Weathuino.APIs.Fiware.Payloads
{
    public class PayloadRegistroAtributos
    {
        public int EntityNameID { get; set; }

        public string Monta()
        {
            var payload = new JObject
            {
                ["description"] = "Notify STH-Comet of all Motion Sensor count changes",
                ["subject"] = new JObject
                {
                    ["entities"] = new JArray
                    {
                        new JObject
                        {
                            ["id"] = $"urn:ngsi-ld:Sensor:{EntityNameID}",
                            ["type"] = "Sensor"
                        }
                    },
                    ["condition"] = new JObject
                    {
                        ["attrs"] = new JArray
                        {
                            "temperatura"
                        }
                    }
                },
                ["notification"] = new JObject
                {
                    ["http"] = new JObject
                    {
                        ["url"] = $"http://{Constantes.IP_SERVIDOR}:8666/notify"
                    },
                    ["attrs"] = new JArray
                    {
                        "temperatura"
                    },
                    ["attrsFormat"] = "legacy"
                }
            };

            return JsonConvert.SerializeObject(payload);
        }
    }
}
