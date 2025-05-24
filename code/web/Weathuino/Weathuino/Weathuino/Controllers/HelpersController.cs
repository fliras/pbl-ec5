using Microsoft.AspNetCore.Http;

namespace Weathuino.Controllers
{
    public class HelpersControllers
    {
        public static bool VerificaSeUsuarioEstaLogado(ISession session)
        {
            return session.GetString("Logado") != null;
        }
    }
}
