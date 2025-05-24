using Weathuino.Enums;

namespace Weathuino.Controllers
{
    public class HomeController : PadraoController
    {
        public HomeController()
        {
            AcessoExigido = PerfisAcesso.COMUM;
        }
    }
}
