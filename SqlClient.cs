using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DurableFunctionHook
{
    internal class SqlClient
    {
        private static string GetConnectionString()
        {
            return "Server=US1228789W1\\APP01SQL19;Database=TableStorageWarehouse;Trusted_Connection=True;TrustServerCertificate=True";
        }

        public static async Task<List<string>> GetInstanceIdsToTerminate()
        {
            List<string> instanceIds = new();
            using SqlConnection sqlConnection = new SqlConnection(GetConnectionString());
            await sqlConnection.OpenAsync();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "GetInstanceIdsToTerminate";
            var reader = await sqlCommand.ExecuteReaderAsync();
            while (reader.Read())
            {
                instanceIds.Add(reader[0] as string);
            }
            return instanceIds;
        }
    }
}
