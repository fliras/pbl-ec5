using Weathuino.Enums;

namespace Weathuino.Models
{
    public class UsuarioViewModel : PadraoViewModel
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public PerfisAcesso PerfilAcesso {  get; set; }
    }
}
