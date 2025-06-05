namespace Weathuino.Models
{
    /// <summary>
    /// Model utilizada para representar os dados de temperatura exibido nos dashboards
    /// </summary>
    public class DadosTemperatura
    {
        public long Timestamp { get; set; } // Unix timestamp em milissegundos
        public double Temperature { get; set; } // Temperatura em °C
    }
}
