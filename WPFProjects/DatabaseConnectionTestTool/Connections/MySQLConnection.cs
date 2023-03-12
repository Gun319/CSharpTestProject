using MySqlConnector;

namespace DatabaseConnectionTestTool.Connections
{
    /// <summary>
    /// MySQL 连接
    /// </summary>
    public class MySQLConnection
    {
        public static string Connection(string connectionStr)
        {
            string result;
            using MySqlConnection mySqlConnection = new(connectionStr);
            try
            {
                mySqlConnection.Open();
                result = "连接成功";
            }
            catch (System.Exception ex)
            {
                mySqlConnection.Clone();
                result = ex.Message;
            }
            return result;
        }

    }
}
