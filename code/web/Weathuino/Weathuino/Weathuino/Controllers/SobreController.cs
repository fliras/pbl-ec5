using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Weathuino.Models;

namespace Weathuino.Controllers
{
    public class SobreController : Controller
    {
        private List<IntegranteViewModel> _integrantes = new List<IntegranteViewModel>()
        {
            new IntegranteViewModel { Nome = "Felipe Lira", RM = "081230006" },
            new IntegranteViewModel { Nome = "Gustavo Trindade", RM = "081230033" },
            new IntegranteViewModel { Nome = "Lucas Barboza", RM = "081230009" },
            new IntegranteViewModel { Nome = "Matheus Nunes", RM = "081230030" },
            new IntegranteViewModel { Nome = "Vitor Malvão", RM = "081230020" },
        };

        public IActionResult Index()
        {
            
            return View(_integrantes);
        }
    }
}
