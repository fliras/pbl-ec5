namespace Weathuino.Models
{
    public class EstufaViewModel: BaseViewModel
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public float? TemperaturaMinima { get; set; }
        public float? TemperaturaMaxima { get; set; }
        public MedidorViewModel Medidor { get; set; }
    }
}
