using System.Data.SqlClient;

namespace ScoutAI.DAL
{
    public static class DbConnectionFactory
    {
        // SQL Server bağlantı adresin (Server adın farklıysa buradaki Data Source kısmını güncelleyebilirsin)
        private const string ConnectionString = @"Server=DESKTOP-25CJ3UL;Database=ScoutAIDB;Trusted_Connection=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}