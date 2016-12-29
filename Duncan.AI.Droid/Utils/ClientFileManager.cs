using System;
using System.Text;
using Android.Util;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android.Content;
using Duncan.AI.Droid;
using Duncan.AI.Droid.Utils.DataAccess;
using Duncan.AI.Droid.Utils.HelperManagers;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using SQLite;

namespace Duncan.AI.Droid.Utils.DataAccess
{


    public class ClientFileManager
    {
        public ClientFileManager()
        {
        }


        // TODO - move these helper methods into seperate public class for usage


        /// <summary>
        /// Safe method to retrieve value from datarow
        /// </summary>
        /// <param name="iColumnName"></param>
        /// <returns></returns>
        private static DateTime GetSafeColumnDateTimeValueFromDataRow(DataRow iSourceRow, string iColumnName)
        {
            try
            {
                if (iSourceRow.Table.Columns.IndexOf(iColumnName) != -1)
                {
                    if (iSourceRow[iColumnName] != null)
                    {
                        DateTime loTemp = (DateTime)iSourceRow[iColumnName];
                        return loTemp;
                    }
                }
            }
            catch (Exception exp)
            {
                //string loErrMsg = "Error: GetValueFromDataRow " + "tableName: " + iSourceRow.Table.TableName;
                //LoggingManager.LogApplicationError(exp, "tableName: " + iSourceRow.Table.TableName, "GetValueFromDataRow");
                Console.WriteLine("Exception source: {0}", exp.Source);
            }

            // exiting here is a failure
            return new DateTime(1900, 1, 1);
        }



        /// <summary>
        /// Safe method to retrieve value from datarow
        /// </summary>
        /// <param name="iColumnName"></param>
        /// <returns></returns>
        private static long GetSafeColumnLongValueFromDataRow(DataRow iSourceRow, string iColumnName)
        {
            long loResult = 0;
            try
            {
                if (iSourceRow.Table.Columns.IndexOf(iColumnName) != -1)
                {
                    if (iSourceRow[iColumnName] != null)
                    {
                        string loTemp = iSourceRow[iColumnName].ToString();
                        Int64.TryParse( loTemp, out loResult );
                    }
                }
            }
            catch (Exception exp)
            {
                //string loErrMsg = "Error: GetValueFromDataRow " + "tableName: " + iSourceRow.Table.TableName;
                //LoggingManager.LogApplicationError(exp, "tableName: " + iSourceRow.Table.TableName, "GetValueFromDataRow");
                Console.WriteLine("Exception source: {0}", exp.Source);
            }

            return loResult;
        }



        /// <summary>
        /// Safe method to retrieve value from datarow
        /// </summary>
        /// <param name="iColumnName"></param>
        /// <returns></returns>
        private static string GetSafeColumnStringValueFromDataRow(DataRow iSourceRow, string iColumnName)
        {
            string loResultStr = string.Empty;
            try
            {
                if (iSourceRow.Table.Columns.IndexOf(iColumnName) != -1)
                {
                    if (iSourceRow[iColumnName] != null)
                    {
                        loResultStr = iSourceRow[iColumnName].ToString();
                    }
                }
            }
            catch (Exception exp)
            {
                //string loErrMsg = "Error: GetValueFromDataRow " + "tableName: " + iSourceRow.Table.TableName;
                //LoggingManager.LogApplicationError(exp, "tableName: " + iSourceRow.Table.TableName, "GetValueFromDataRow");
                Console.WriteLine("Exception source: {0}", exp.Source);
            }

            return loResultStr;
        }


        /// <summary>
        /// Return a "directory" list of filedata
        /// </summary>
        public static FileSet GetFileSystemListing()
        {

            FileSet loFileSet = new FileSet();

            try
            {
                // get all of the file info (but not the file data!)
                var query = "SELECT _id, FILENAME, FILESIZE, FILECREATIONDATE, FILEMODIFIEDDATE FROM " + AutoISSUE.DBConstants.sqlAutoISSUEFileSystemTableName;


                //public string FILENAME { get; set; }
                //public long FILESIZE { get; set; }
                //public DateTime FILECREATIONDATE { get; set; }
                //public DateTime FILEMODIFIEDDATE { get; set; }


                var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(query);

                foreach (DataRow row in result.Rows)
                {
                    FileInformation oneFile = new FileInformation();
                    oneFile.FileName = GetSafeColumnStringValueFromDataRow( row, "FILENAME" );
                    oneFile.Length = GetSafeColumnLongValueFromDataRow( row, "FILESIZE" );
                    oneFile.FileDate = GetSafeColumnDateTimeValueFromDataRow( row, "FILECREATIONDATE" );


                    loFileSet.FileList.Add(oneFile);
                }

            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception in GetFileSystemListing: {0}", exp.Message);
            }

            return loFileSet;
        }

        public static byte[] SerializeComplexType(ComplexType c)
        {
            try
            {
                byte[] arrayData;

                using (MemoryStream stream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, c);
                    arrayData = stream.ToArray();
                    stream.Close();
                }

                return arrayData;
            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception in SerializeComplexType: {0}", exp.Message);
            }

            return null;
        }



        public static ComplexType DeserializeComplexType(byte[] arrayData)
        {
            ComplexType c = ComplexType.MantissaMask;

            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(arrayData, 0, arrayData.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    BinaryFormatter formatter = new BinaryFormatter();
                    c = (ComplexType)formatter.Deserialize(stream);
                }

                return c;
            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception in DeserializeComplexType: {0}", exp.Message);
            }

            return c;
        }



        /// <summary>
        /// Insert a file into the file system
        /// </summary>
        /// <param name="newDataSet"></param>
        /// <param name="seqName"></param>
        public static long InsertFileData(string iFileName, long iFileByteSize, DateTime iFileCreationTimeStamp, DateTime iFileModificationTimeStamp, byte[] iFileData)
        {
            long loNewRowId = -1;

            try
            {
                AIFileSystemDAO loInitialValues = new AIFileSystemDAO();
                loInitialValues.FILENAME = iFileName;
                loInitialValues.FILECREATIONDATE = iFileCreationTimeStamp;
                loInitialValues.FILEMODIFIEDDATE = iFileModificationTimeStamp;
                loInitialValues.FILESIZE = iFileByteSize;
                loInitialValues.FILEDATA = iFileData;

                loNewRowId = (new DatabaseManager()).InsertAutoISSUEFileSystemRow(loInitialValues);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception InsertFileData source: {0}", e.Source);
            }

            return loNewRowId;
        }



        /// <summary>
        /// Return the DataRow for filename filesystem
        /// </summary>
        /// <param name="iTableName"></param>
        /// <param name="iIssueNumberStr"></param>
        /// <returns></returns>
        public static AIFileSystemDAO GetDataForFileSystemFile(string iFileSystemFileName)
        {
            AIFileSystemDAO retVal = null;

            try
            {
                retVal = (new DatabaseManager()).GetAIFileSystemData( iFileSystemFileName );
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception source: {0}", e.Source);
            }

            return retVal;
        }




        /// <summary>
        /// Return the DataRow for filename filesystem
        /// </summary>
        /// <param name="iTableName"></param>
        /// <param name="iIssueNumberStr"></param>
        /// <returns></returns>
        public static DataRow GetDataRowForFileSystemFile(string iFileSystemFileName)
        {
            try
            {
                var queryStringBuilder = new StringBuilder();
                queryStringBuilder.Append(" SELECT * FROM " + AutoISSUE.DBConstants.sqlAutoISSUEFileSystemTableName);
                queryStringBuilder.Append(" WHERE " + AutoISSUE.DBConstants.sqlAutoISSUEFileSystemFileName + " = " + iFileSystemFileName);

                var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());

                foreach (DataRow oneRow in result.Rows)
                {
                    return oneRow;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception source: {0}", e.Source);
            }

            return null;
        }



        public static long InsertOrUpdateFileData(string iFileName, long iFileByteSize, DateTime iFileCreationTimeStamp, DateTime iFileModificationTimeStamp, byte[] iFileData)
        {
            long loNewRowId = -1;

            try
            {
                AIFileSystemDAO loInitialValues = new AIFileSystemDAO();
                loInitialValues.Id = -1;
                loInitialValues.FILENAME = iFileName;
                loInitialValues.FILECREATIONDATE = iFileCreationTimeStamp;
                loInitialValues.FILEMODIFIEDDATE = iFileModificationTimeStamp;
                loInitialValues.FILESIZE = iFileByteSize;
                loInitialValues.FILEDATA = iFileData;

                loNewRowId = (new DatabaseManager()).InsertOrUpdateAutoISSUEFileSystemRow(loInitialValues);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception InsertOrUpdateFileData source: {0}", e.Source);
            }

            return loNewRowId;
        }


    }
}

