using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Weathuino.Models;

namespace Weathuino.Controllers
{
    public class HelpersControllers
    {
        public static SessaoViewModel ObtemDadosDaSessao(ISession session)
        {
            string dadosEmJSON = session.GetString("DadosSessao");
            if (dadosEmJSON.IsNullOrEmpty())
                return null;
            return JsonConvert.DeserializeObject<SessaoViewModel>(dadosEmJSON);
        }
    }
}
