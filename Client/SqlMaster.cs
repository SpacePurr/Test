using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Client
{
    class SqlMaster
    {
        private readonly string connectionString;
        public SqlMaster(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public object ExecuteScalarCommand(string cmdText)
        {
            if(connectionString == null)
                return "Отсутсвует подключение";

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
