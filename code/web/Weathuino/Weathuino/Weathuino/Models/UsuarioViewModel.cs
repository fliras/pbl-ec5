namespace Weathuino.Models
{
    public class UsuarioViewModel : PadraoViewModel
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public PerfilAcessoViewModel PerfilAcesso { get; set; }
    }
}
