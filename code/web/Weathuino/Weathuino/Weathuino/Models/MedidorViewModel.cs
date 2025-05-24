using System;

namespace Weathuino.Models
{
    public class MedidorViewModel : PadraoViewModel
    {
        public string Nome { get; set; }
        public DateTime? DataUltimoRegistro { get; set; }
    }
}
