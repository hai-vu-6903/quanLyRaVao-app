using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace qlrv.Services
{
    public class DatabaseService
    {
        private static string connectionString = "Server=.\\SQLEXPRESS;" + "Database=QuanLyRaVao;" + "Trusted_Connection=True;" + "TrustServerCertificate=True;";
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
