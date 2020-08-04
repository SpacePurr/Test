using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Client
{
    class SqlMaster
    {
        private readonly string connectionString;

        public bool IsConnected { get; set; }

        public SqlMaster(string connectionString)
        {
            this.connectionString = connectionString;

            IsConnected = connectionString != null && ExecuteScalarCommand("select 1").ToString() == "1";
        }

        public object ExecuteScalarCommand(string cmdText)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                connection.Open();
                return command.ExecuteScalar();
            }
        }

        public void ExecuteNonQueryCommand(string cmdText)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
