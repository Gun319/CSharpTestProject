using Npgsql;
using System;

namespace DatabaseConnectionTestTool.Connections
{
    /// <summary>
    /// NpgSql 连接
    /// </summary>
    public class PostgresqlConnection
    {
        public static string Connection(string connectionStr)
        {
            string result;
            using NpgsqlConnection npgsqlConnection = new(connectionStr);
            try
            {
                npgsqlConnection.Open();
                result = "连接成功";
            }
            catch (Exception ex)
            {
                npgsqlConnection.Close();
                result = ex.Message;
            }
            return result;
        }
    }
}
