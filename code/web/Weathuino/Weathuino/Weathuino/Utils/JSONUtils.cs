using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Weathuino.Utils
{
    /// <summary>
    /// Métodos utilitários para operações com JSON
    /// </summary>
    public class JSONUtils
    {
        public static string ConverteObjetoParaStringJSON(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static JObject ConverteObjetoParaJObject(object obj)
        {
            return JObject.FromObject(obj);
        }

        public static T ConverteStringJSONParaObjeto<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}
