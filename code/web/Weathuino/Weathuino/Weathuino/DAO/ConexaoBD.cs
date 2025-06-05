using Microsoft.Data.SqlClient;

namespace Weathuino.DAO
{
    /// <summary>
    /// Classe auxiliar para gerenciar conexões com o BD.
    /// </summary>
    public static class ConexaoBD
    {
        public static SqlConnection GetConexao()
        {
            // Alterar string de conexão conforme o uso !
            string strCon = "Data Source=LOCALHOST;Initial Catalog=weathuinodb;user id=sa; password=Cod.6219; TrustServerCertificate=True";
            SqlConnection conexao = new SqlConnection(strCon);
            conexao.Open();
            return conexao;
        }
    }
}
