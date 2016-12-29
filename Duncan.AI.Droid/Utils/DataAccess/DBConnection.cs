using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using SQLite;
using Environment = System.Environment;

namespace Duncan.AI.Droid.Utils.DataAccess
{
    public static class DbConnection
    {
        //todo - make this private
        public static string GetDBPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Constants.DBName);
        }

        private static SqliteConnection _connection;
        public static SqliteConnection GetConnection()
        {
            //todo - might have to move this somewhere else.
            if (_connection == null)
            {
                string dbPath = GetDBPath();
                bool exists = File.Exists(dbPath);
                if (!exists)
                    SqliteConnection.CreateFile(dbPath);
                Syscall.chmod(dbPath, Syscall.S_IRWXU | Syscall.S_IRWXG | Syscall.S_IRWXO);
                _connection = new SqliteConnection("Data Source=" + dbPath);
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
            }

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            return _connection;
        }

        public static void CloseConnection()
        {
            if (_connection != null && (_connection != null || _connection.State != ConnectionState.Closed))
            {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
            }
        }

        private static SQLiteConnection _sqlconnection;
        public static SQLiteConnection GetSqlConnection()
        {
            string dbPath = GetDBPath();
            if (_sqlconnection == null)
            {
                bool exists = File.Exists(dbPath);
                if (!exists)
                    SqliteConnection.CreateFile(dbPath);
                Syscall.chmod(dbPath, Syscall.S_IRWXU | Syscall.S_IRWXG | Syscall.S_IRWXO);
                _sqlconnection = new SQLiteConnection(dbPath);
           
            }
            return _sqlconnection;
        }

        public static void CloseSqlConnection()
        {
            if (_sqlconnection != null)
            {
                _sqlconnection.Close();
                _sqlconnection.Dispose();
                _sqlconnection = null;
            }
        }
    }
}