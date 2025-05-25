using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Weathuino.Models;
using Weathuino.Enums;

namespace Weathuino.Controllers
{
    public class SobreController : PadraoController
    {
        public SobreController()
        {
            AcessoExigido = PerfisAcesso.COMUM;
        }

        private List<IntegranteViewModel> _integrantes = new List<IntegranteViewModel>()
        {
            new IntegranteViewModel { Nome = "Felipe Lira", RM = "081230006" },
            new IntegranteViewModel { Nome = "Gustavo Trindade", RM = "081230033" },
            new IntegranteViewModel { Nome = "Lucas Barboza", RM = "081230009" },
            new IntegranteViewModel { Nome = "Matheus Nunes", RM = "081230030" },
            new IntegranteViewModel { Nome = "Vitor Malvão", RM = "081230020" },
        };

        public override IActionResult Index()
        {
            try
            {
                return View(_integrantes);
            }
            catch (Exception error)
            {
                return View("Error", new ErrorViewModel(error.ToString()));
            }
        }
    }
}
