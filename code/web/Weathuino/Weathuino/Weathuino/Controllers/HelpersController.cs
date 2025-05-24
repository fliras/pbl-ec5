using Microsoft.AspNetCore.Http;

namespace Weathuino.Controllers
{
    public class HelpersControllers
    {
        public static bool VerificaUserLogado(ISession session)
        {
            string logado = session.GetString("Logado");
            if (logado == null)
                return false;
            else
                return true;
        }
    }
}
