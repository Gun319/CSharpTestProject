using Microsoft.Data.SqlClient;

namespace DatabaseConnectionTestTool.Connections
{
    /// <summary>
    /// SQLServer 连接
    /// </summary>
    public class SQLServerConnection
    {
        public static string Connection(string connectionStr)
        {
            string result;
            using SqlConnection sqlConnection = new(connectionStr);
            try
            {
                sqlConnection.Open();
                result = "连接成功";
            }
            catch (System.Exception ex)
            {
                sqlConnection.Close();
                result = ex.Message;
            }
            return result;
        }
    }
}
