using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace MathMaster.Views
{
    class DB_Connection
    {
        // Database connection details
        private const string Server = "10.0.2.2"; // Emulator: 10.0.2.2
        private const string Database = "mathmaster";
        private const string UserID = "root";
        private const string Password = "";
        private const string SslMode = "None";

        // Connection string
        private static string ConnectionString =>
            $"Server={Server};Database={Database};User ID={UserID};Password={Password};SslMode={SslMode};";

        // Method to get a MySQL connection instance
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        // Method to test if the database connection works
        public static async Task<bool> TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}