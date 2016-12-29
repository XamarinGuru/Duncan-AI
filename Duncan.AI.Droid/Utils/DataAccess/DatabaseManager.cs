
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using Android.Content;
using Android.Util;
using Duncan.AI.Droid.Utils.HelperManagers;
using Mono.Data.Sqlite;
using SQLite;
using Exception = System.Exception;

namespace Duncan.AI.Droid.Utils.DataAccess
{
    public class DatabaseManager
    {

        #region Connections

        private static string DBPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Constants.DBName);
            }
        }

        private static SqliteConnection _connection { get; set; }
        protected static SqliteConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    bool exists = File.Exists(DBPath);
                    if (!exists)
                    {
                        SqliteConnection.CreateFile(DBPath);
                        Syscall.chmod(DBPath, Syscall.S_IRWXU | Syscall.S_IRWXG | Syscall.S_IRWXO);
                    }
                    _connection = new SqliteConnection("Data Source=" + DBPath);
                }

                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                return _connection;
            }
        }

        private static SQLiteConnection _sqlconnection { get; set; }
        protected static SQLiteConnection SqlConnection
            {
            get {
            if (_sqlconnection == null)
                {
                bool exists = File.Exists(DBPath);
                if (!exists)
                    {
                    SqliteConnection.CreateFile(DBPath);
                    Syscall.chmod(DBPath, Syscall.S_IRWXU | Syscall.S_IRWXG | Syscall.S_IRWXO);
                    }
                _sqlconnection = new SQLiteConnection(DBPath);

                }
            return _sqlconnection;}
        }

        private static Mutex ConnectionMutex = new Mutex();

        #endregion

        #region Generic methods

        public  int ExecuteNonQuery(string command)
        {
            command = CleanUpQuery(command);
            int count = 0;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex");
            else
            {
                try
                {
                    using (var c = Connection.CreateCommand())
                    {
                        c.CommandText = command;
                        count = c.ExecuteNonQuery();
            }
                }
                catch (Exception e)
            {
                    LoggingManager.LogApplicationError(e, "command: " + command, "ExecuteNonQuery");
            }
            finally
            {
                    ConnectionMutex.ReleaseMutex();
            }
            }

            return count;
        }

        #endregion

        public int GetRecordCount(string command)
        {
            command = CleanUpQuery(command);

            int count = 0;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex");
            else
            {
                try
                {
                    using (var cs = Connection.CreateCommand())
                    {
                        cs.CommandText = command;
                        using (var reader = cs.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //Each row
                                string value = reader[0].ToString();
                                int.TryParse(value, out count);
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "query: " + command, "GetRecordCount");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }

            return count;
        }

        #region Login

        public bool PopulateUserNames(List<UserDAO> users)
        {
            bool retVal = true;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex");
            else
            {
                try
                {
                    //Create table if table does not exist
                    var created = SqlConnection.CreateTable<UserDAO>();
                    if (created > -1)
                    {
                        //Delete existing records
                        var userDAOs = SqlConnection.Table<UserDAO>();
                        if (userDAOs != null)
                            foreach (var userDAO in userDAOs)
                                SqlConnection.Delete<UserDAO>(userDAO.Id);

                        //Insert new records
                        foreach (UserDAO user in users)
                            SqlConnection.Insert(user);
                    }
                }
                catch (Exception e)
                {
                    retVal = false;
                    LoggingManager.LogApplicationError(e, null, "PopulateUserNames");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return retVal;
        }

        //Asyn task to authenticate and authorize user by username and password 
        public UserDAO ValidateLogin(string username, string password, CancellationToken cancellationToken)
        {
            UserDAO retVal = null;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex - ");
            else
            {

                try
                {
                    var users = SqlConnection.Query<UserDAO>("SELECT * FROM User WHERE username = ?", username);
                    foreach (UserDAO userDAO in users.Where(userDAO => userDAO.password == password))
                    {
                        retVal = userDAO;
                        break;
                    }
                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "Username: " + username + " Pw: " + password,
                                                       "ValidateLogin");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }

            return retVal;
        }



        public bool CreateAppPropertiesTable()
        {
            bool retVal = true;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex");
            else
            {
                try
                {
                    //Create table if table does not exist
                    var created = SqlConnection.CreateTable<PropertiesDAO>();
                }
                catch (Exception e)
                {
                    retVal = false;
                    LoggingManager.LogApplicationError(e, null, "CreateAppPropertiesTable");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return retVal;
        }

        public PropertiesDAO RetrieveAppProperties()
        {

            PropertiesDAO retVal = null;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex - ");
            else
            {
                try
                {
                    var properties = SqlConnection.Query<PropertiesDAO>("SELECT * FROM Properties WHERE _id = 1");
                    if (properties.Any())
                    {
                        retVal = properties.FirstOrDefault();
                    }
                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, null, "RetrieveAppProperties");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return retVal;

        }

        public bool SaveAppProperties_old(PropertiesDAO propertiesDAO)
        {

            bool retVal = true;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex - ");
            else
            {
                try
                {
                    //Create table if table does not exist
                    var created = SqlConnection.CreateTable<PropertiesDAO>();
                    if (created > -1)
                    {
                        SqlConnection.Insert(propertiesDAO);
                    }
                    else
                    {
                        SqlConnection.Update(propertiesDAO);
                    }
                }
                catch (Exception e)
                {
                    retVal = false;
                    LoggingManager.LogApplicationError(e, null, "SaveAppProperties");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return retVal;

        }
        public long SaveAppProperties(PropertiesDAO iAppProperties)
        {
            long loReturnDefault = -1;
            if (!ConnectionMutex.WaitOne())
            {
                Log.Error("Mutex:", "Waiting on ConnectionMutex - SaveAppProperties");
            }
            else
            {
                try
                {
                    // does this one already exist?
                    PropertiesDAO loExistingRecord = SqlConnection.Table<PropertiesDAO>().Where(x => x.Id == iAppProperties.Id).FirstOrDefault();
                    if (loExistingRecord != null)
                    {
                        // found it - lets use the same
                        iAppProperties.Id = loExistingRecord.Id;
                        // this has to have a primary key
                        if (SqlConnection.Update(iAppProperties, typeof(PropertiesDAO)) > 0)
                        {
                            // return the row id
                            loReturnDefault = loExistingRecord.Id;
                        }
                    }
                    else
                    {
                        // new record - insert;
                        if (SqlConnection.Insert(iAppProperties) > -1)
                        {
                            loReturnDefault = iAppProperties.Id;
                        }
                    }

                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "sqlCommand: UPDATE or INSERT", "SaveAppProperties");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return loReturnDefault;

        }


        #endregion


        #region Reporting
        //Report user activity to DB
        public bool ReportActivity(string username, string desc, CancellationToken cancellationToken)
        {
            bool retVal = true;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex - ");
            else
            {
                try
                {
                    //Create table if table does not exist
                    SqlConnection.CreateTable<ReportingDAO>();
                    var reporting = new ReportingDAO {desc = desc, username = username, date = DateTime.Now};
                    SqlConnection.Insert(reporting);
                }
                catch (Exception e)
                {
                    retVal = false;
                    LoggingManager.LogApplicationError(e, "Username: " + username + " Description: " + desc,
                                                       "ReportActivity");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
                return retVal;
           
        }
            return retVal;

        }

        //Get all pending activities submitted by user from DB
        public string[] GetAllActivityLogsByUser(string username, CancellationToken cancellationToken)
        {

            var retVal = new List<string>();
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex - ");
            else
            {
                try
                {
                    //Create table if table does not exist
                    var reportingList = SqlConnection.Query<ReportingDAO>("SELECT * FROM Reporting WHERE username = ?", username);
                    retVal.AddRange(reportingList.Select(reporting => reporting.desc + "---" + reporting.date));
                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "Username: " + username, "GetAllActivityLogsByUser");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
                return retVal.ToArray();
        }
            return retVal.ToArray();
        }

        #endregion

        #region GPS
        //Save user current location to DB
        public bool SaveUserCurrentLocation(string username, string latitude, string longitude)
        {
            var retVal = true;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex - ");
            else
            {
                try
                {
                    //Create table if table does not exist
                    SqlConnection.CreateTable<GpsDAO>();
                    var gpsDAO = new GpsDAO
                        {
                            latitude = latitude,
                            longitude = longitude,
                            username = username,
                            date = DateTime.Now
                        };
                    SqlConnection.Insert(gpsDAO);
                }
                catch (Exception e)
                {
                    retVal = false;
                    LoggingManager.LogApplicationError(e,
                                                       "Username: " + username + " latitude: " + latitude +
                                                       " Longitude: " + longitude, "SaveUserCurrentLocation");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return retVal;
                }

        public string[] GetAllUserLocation(string username, CancellationToken cancellationToken)
        {
            var retVal = new List<string>();
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex - ");
            else
            {
                try
                {
                    //Create table if table does not exist
                    var userLocationList = SqlConnection.Query<GpsDAO>("SELECT * FROM Gps WHERE username = ?", username);
                    retVal.AddRange(
                        userLocationList.Select(
                            location => location.latitude + "---" + location.longitude + "---" + location.date));
                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "Username: " + username, "GetAllUserLocation");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return retVal.ToArray();
        }

        #endregion

        #region Sync process

        public string[] GetListData(string sqlCommand)
        {
            sqlCommand = CleanUpQuery(sqlCommand);

            var retVal = new List<string>();
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex - ");
            else
            {
                try
                {
                    using (var cs = Connection.CreateCommand())
                    {
                        cs.CommandText = sqlCommand;
                        using (var reader = cs.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //Each row
                                string value = reader[0].ToString();

                                //Remove the seperator from the the value if data is null
                                if (value.Contains(Constants.LIST_ITEM_DESCRIPTION_SEPARATOR))
                                {
                                    string[] colData = value.Split(Constants.LIST_ITEM_DESCRIPTION_SEPARATOR[0]);
                                    string colmnOne = colData[1].Trim();
                                    if (colmnOne.Length < 1)
                                    {
                                        //value = value.Replace(':', ' ');
                                        value = value.Replace(Constants.LIST_ITEM_DESCRIPTION_SEPARATOR[0], ' ');

                                    }
                                }

                                retVal.Add(value);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "sqlCommand: " + sqlCommand, "GetListData");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return retVal.ToArray();

        }

        public string GetDataFromTableWithColumnValue(string query, string columnData)
        {
            query = CleanUpQuery(query);
            columnData = CleanUpQuery(columnData);
            var retVal = string.Empty;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex - ");
            else
            {
                try
                {
                    using (var cs = Connection.CreateCommand())
                    {
                        cs.CommandText = query;
                        using (var reader = cs.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                retVal = reader[columnData].ToString();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "query: " + query + " columnData: " + columnData,
                                                       "GetDataFromTableWithColumnValue");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
        }
            return retVal;
        }

        public DataTable GetAllFromTableWithColumnValue(string query)
        {
            query = CleanUpQuery(query);
            var retVal = new DataTable();
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex - ");
            else
            {
                try
                {
                    using (var cs = Connection.CreateCommand())
                    {
                        cs.CommandText = query;
                        using (var reader = cs.ExecuteReader())
                        {
                            retVal.Load(reader);
                        }
                    }
                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "query: " + query, "GetDataFromTableWithColumnValue");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
                }
                return retVal;
        }



        #endregion

        #region Creating ticket

        public bool ExecuteQuery(String str)
        {
            str = CleanUpQuery(str);
            var retVal = false;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex - ");
            else
            {
                try
                {
                    ExecuteNonQuery(str);
                    retVal = true;
                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "query: " + str, "ExecuteQuery");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return retVal;

        }

        public bool CheckForRows(string command)
        {
            command = CleanUpQuery(command);
            bool retVal = false;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex - ");
            else
            {
                try
                {
                    using (var cs = Connection.CreateCommand())
                    {
                        cs.CommandText = command;
                        var reader = cs.ExecuteReader();
                        retVal = reader.HasRows;
                        reader.Close();
                    }
                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "query: " + command, "GetDataFromTableWithColumnValue");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return retVal;
        }
        #endregion


        #region Helpers
        private string CleanUpQuery(string query)
        {
            query = query.Replace("[[", "[");
            query = query.Replace("]]", "]");
            return query;
        }
        #endregion


        #region PseudoFileSystemFunctions

        public bool CreateAutoISSUEFileSystemTable()
        {
            bool retVal = true;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex");
            else
            {
                try
                {
                    //Create table if table does not exist
                    var created = SqlConnection.CreateTable<AIFileSystemDAO>();
                }
                catch (Exception e)
                {
                    retVal = false;
                    LoggingManager.LogApplicationError(e, null, "CreateAutoISSUEFileSystemTable");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return retVal;
        }

        public bool EmptyAutoISSUEFileSystemTable()
        {
            // test update func
            return true;


            bool retVal = true;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex");
            else
            {

                // TOOO - easier/faster  to just to DROP and then re-create the table?


                try
                {
                    //Delete existing records
                    var FileSystemDAOs = SqlConnection.Table<AIFileSystemDAO>();
                    if (FileSystemDAOs != null)
                    {
                        foreach (var oneFileSystemDAO in FileSystemDAOs)
                        {
                            SqlConnection.Delete<AIFileSystemDAO>(oneFileSystemDAO.Id);
                        }
                    }
                }
                catch (Exception e)
                {
                    retVal = false;
                    LoggingManager.LogApplicationError(e, null, "EmptyAutoISSUEFileSystemTable");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return retVal;
        }


        /// <summary>
        /// Returns the filedata for iFileName
        /// 
        /// Returns NULL if not found
        /// </summary>
        /// <param name="iFileName"></param>
        /// <returns></returns>
        public AIFileSystemDAO GetAIFileSystemData(string iFileName)
        {
            AIFileSystemDAO retVal = null;
            if (!ConnectionMutex.WaitOne())
                Log.Error("Mutex:", "Waiting on ConnectionMutex - ");
            else
            {
                try
                {
                    // if the file isn't there, will return null
                    retVal = SqlConnection.Table<AIFileSystemDAO>().Where(x => x.FILENAME == iFileName).FirstOrDefault();
                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "filename: " + iFileName, "GetAIFileSystemData");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return retVal;
        }



        public long InsertAutoISSUEFileSystemRow(AIFileSystemDAO iFileSystemRecord)
        {
            long loReturnDefault = -1;
            if (!ConnectionMutex.WaitOne())
            {
                Log.Error("Mutex:", "Waiting on ConnectionMutex - InsertAutoISSUEFileSystemRow");
            }
            else
            {
                try
                {
                    if (SqlConnection.Insert(iFileSystemRecord) > -1)
                    {
                        loReturnDefault = iFileSystemRecord.Id;
                    }

                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "sqlCommand: INSERT", "InsertAutoISSUEFileSystemRow");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return loReturnDefault;

        }

        public long InsertOrUpdateAutoISSUEFileSystemRow(AIFileSystemDAO iFileSystemRecord)
        {
            long loReturnDefault = -1;
            if (!ConnectionMutex.WaitOne())
            {
                Log.Error("Mutex:", "Waiting on ConnectionMutex - UpdateOrInsertAutoISSUEFileSystemRow");
            }
            else
            {
                try
                {
                    // does this one already exist?
                    AIFileSystemDAO loExistingRecord = SqlConnection.Table<AIFileSystemDAO>().Where(x => x.FILENAME == iFileSystemRecord.FILENAME).FirstOrDefault();
                    if (loExistingRecord != null)
                    {
                        // found it - lets use the same
                        iFileSystemRecord.Id = loExistingRecord.Id;
                        // this has to have a primary key
                        if (SqlConnection.Update(iFileSystemRecord, typeof(AIFileSystemDAO)) > 0)
                        {
                            // return the row id
                            loReturnDefault = loExistingRecord.Id;
                        }
                    }
                    else
                    {
                        // new record - insert;
                        if (SqlConnection.Insert(iFileSystemRecord) > -1)
                        {
                            loReturnDefault = iFileSystemRecord.Id;
                        }
                    }

                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "sqlCommand: UPDATE or INSERT", "UpdateOrInsertAutoISSUEFileSystemRow");
                }
                finally
                {
                    ConnectionMutex.ReleaseMutex();
                }
            }
            return loReturnDefault;

        }

        #endregion

    }
}
