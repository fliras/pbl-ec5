namespace Weathuino.Models
{
    public class DadosTemperatura
    {
        public long Timestamp { get; set; } // Unix timestamp em milissegundos
        public double Temperature { get; set; } // Temperatura em °C
    }
}
