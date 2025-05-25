using Microsoft.AspNetCore.Http;
using System.IO;

namespace Weathuino.Utils
{
    public class FileUtils
    {
        public static byte[] ConverteArquivoEmArrayDeBytes(IFormFile arquivo)
        {
            if (arquivo != null)
                using (var ms = new MemoryStream())
                {
                    arquivo.CopyTo(ms);
                    return ms.ToArray();
                }
            else
                return null;
        }
    }
}
