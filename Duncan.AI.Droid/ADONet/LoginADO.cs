using System;
using System.Data;
using System.Text;
using Android.Content;
using Android.Util;
using Duncan.AI.Droid.Utils.HelperManagers;
using Mono.Data.Sqlite;
using Reino.ClientConfig;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace Duncan.AI.Droid
{
    public class LoginADO
    {
        public LoginADO()
		{
		}

        public static bool PopulateUserNames(string filePath, string colNamesStrBldr, int noOfColms)
        {
            string dbPath = Constants.getSqliteDBPath();
            bool exists = File.Exists(dbPath);
            if (!exists)
                SqliteConnection.CreateFile(dbPath);
            SqliteConnection connection = new SqliteConnection("Data Source=" + dbPath);
            connection.Open();
            SqliteTransaction txn = connection.BeginTransaction();

            //Loop through Temp file and build string INSERT QUERIES
            using (var sr = new StreamReader(filePath))
            {
                while (sr.Peek() >= 0)
                {
                    string content = sr.ReadLine();
                    string[] data = content.Split('\t');
                    var instRowStrBldr = new StringBuilder();
                    instRowStrBldr.Append("INSERT INTO ");
                    instRowStrBldr.Append(Constants.USERSTRUCT_TABLE);
                    instRowStrBldr.Append(" ( ");
                    instRowStrBldr.Append(colNamesStrBldr);
                    instRowStrBldr.Append(" ) ");
                    instRowStrBldr.Append("VALUES ( ");
                    int j = 0;
                    foreach (string colVal in data)
                    {
                        //Below condition to make sure the no of values columns equal to columns in table fields
                        if (j < noOfColms)
                        {
                            if (j != 0)
                                instRowStrBldr.Append(", ");
                            j++;
                            instRowStrBldr.Append("'");
                            //Cases when text has apostrophes 
                            instRowStrBldr.Append(colVal.Replace('\'', ' '));
                            instRowStrBldr.Append("'");
                        }
                    }

                    //Below condition to make sure the no of table fields columns equal to values columns
                    if (j < noOfColms)
                    {
                        int diffNoOfColms = noOfColms - j;
                        for (int k = 0; k < diffNoOfColms; k++)
                        {
                            instRowStrBldr.Append(", ");
                            instRowStrBldr.Append("''");
                        }
                    }
                    instRowStrBldr.Append(" );");
                    using (var c = connection.CreateCommand())
                    {
                        c.CommandText = instRowStrBldr.ToString();
                        c.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

       
    }
}