namespace Weathuino.Models
{
    /// <summary>
    /// Utilizada para transportar parâmetros de filtro para buscar registros no BD por meio das DAOs
    /// </summary>
    public class FiltrosViewModel
    {
        public int? Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string DeviceID { get; set; }
    }
}
