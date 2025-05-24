using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Weathuino.Utils
{
    public class JSONUtils
    {
        public static string ConverteObjetoParaStringJSON(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static JObject ConverteObjetoParaJSON(object obj)
        {
            return JObject.FromObject(obj);
        }

        public static T ConverteSringJSONParaObjeto<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}
