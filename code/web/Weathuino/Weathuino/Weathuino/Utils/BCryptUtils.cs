namespace Weathuino.Utils
{
    public class BCryptUtils
    {
        public static string CriaHashBcrypt(string texto)
        {
            return BCrypt.Net.BCrypt.HashPassword(texto);
        }

        public static bool ValidaHashBcrypt(string senha, string hashSenha)
        {
            return BCrypt.Net.BCrypt.Verify(senha, hashSenha);
        }
    }
}
