using Microsoft.AspNetCore.Http;
using System;

namespace Weathuino.Models
{
    public class EstufaViewModel: PadraoViewModel
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public float? TemperaturaMinima { get; set; }
        public float? TemperaturaMaxima { get; set; }
        public MedidorViewModel Medidor { get; set; }
        public IFormFile Imagem { get; set; }
        public byte[] ImagemEmByte { get; set; }
        public string ImagemEmBase64
        {
            get
            {
                if (ImagemEmByte != null)
                    return Convert.ToBase64String(ImagemEmByte);
                else
                    return string.Empty;
            }
        }
    }
}
