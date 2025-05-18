using Microsoft.Data.SqlClient;

namespace Weathuino.DAO
{
    public static class ConexaoBD
    {
        public static SqlConnection GetConexao()
        {
            string strCon = "Data Source=LOCALHOST;Initial Catalog=weathuinodb;user id=sa; password=Cod.6219; TrustServerCertificate=True";
            SqlConnection conexao = new SqlConnection(strCon);
            conexao.Open();
            return conexao;
        }
    }
}
