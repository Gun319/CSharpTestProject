using Microsoft.Data.Sqlite;

namespace DatabaseConnectionTestTool.Connections
{
    /// <summary>
    /// Sqlite 连接
    /// </summary>
    public class SQLiteConnection
    {
        public static string Connection(string connectionStr)
        {
            string result;
            using SqliteConnection sqliteConnection = new(connectionStr);
            try
            {
                sqliteConnection.Open();
                result = "连接成功";
            }
            catch (System.Exception ex)
            {
                sqliteConnection.Close();
                result = ex.Message;
            }
            return result;
        }
    }
}
