namespace Weathuino.Utils
{
    /// <summary>
    /// Métodos utiliarios para lidar com hashs bcrypts
    /// </summary>
    public class BCryptUtils
    {
        /// <summary>
        /// Cria um hash BCrypt para um texto
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static string CriaHashBcrypt(string texto)
        {
            return BCrypt.Net.BCrypt.HashPassword(texto);
        }

        /// <summary>
        /// Valida se um texto qualquer é valido em relação a uma hash bcrypt
        /// </summary>
        /// <param name="textoLimpo"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool ValidaHashBcrypt(string textoLimpo, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(textoLimpo, hash);
        }
    }
}
