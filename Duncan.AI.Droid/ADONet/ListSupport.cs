using System;
using System.Data;
using Duncan.AI.Droid.Utils.DataAccess;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Duncan.AI.Droid.Utils.HelperManagers;
using Mono.Data.Sqlite;
using System.IO;
using Reino.ClientConfig;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using Android.App;
using Android.Content;

namespace Duncan.AI.Droid
{
    public class ListSupport
    {

        public ListSupport( )
        {
        }

        /// <summary>
        /// Broadcast intent callback to update progressbar on screen
        /// </summary>
        /// <param name="iProgress"></param>
        private void SendBroadcastProgressUpdate(int iProgress, string iProgressDescriton )
        {

            var intentProgressUpdate = new Intent(Constants.ACTIVITY_INTENT_DISPLAY_SYNC_PROGRESS_NAME);

            intentProgressUpdate.PutExtra(Constants.ActivityIntentExtraInt_ProgressValue, iProgress);
            intentProgressUpdate.PutExtra(Constants.ActivityIntentExtraInt_ProgressDesc, iProgressDescriton);

            if ( DroidContext.ApplicationContext != null )
            {
                 DroidContext.ApplicationContext.SendBroadcast(intentProgressUpdate);
            }

        }

#if _new_

        //Retrive List data from the web service and insert into phone Sqlite DB
        public string InsertListData()
        {
            try
            {
                BuildTablesAndImportListItemDATFilesIntoDatabase();
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, null, "InsertListData");
            }

            return "Success";
        }

        private void BuildTablesAndImportListItemDATFilesIntoDatabase()
        {
            var queryStringList = new ArrayList();

            //Only for Logging purpose total count
            int totalRowCount = 0;

            string tableName = string.Empty;

            foreach (Reino.ClientConfig.TAgList tAgList in DroidContext.XmlCfg.clientDef.ListMgr.AgLists)
            {
                var tableDefs = tAgList.TableDefs;
                foreach (TTableDef tTableDef in tableDefs)
                {

                    try
                    {

                        tableName = tTableDef.Name;

                        SendBroadcastProgressUpdate((int)Constants.SyncStepEnummeration.SyncStep_ProcessListFiles, "Processing List " + tableName + "...");

                        //Build string create table query based on table and column names
                        var crtTblStrBldr = new StringBuilder();
                        //Drop Table if Exists
                        var drpTblStrBldr = new StringBuilder();
                        //Build string for column names
                        var colNamesStrBldr = new StringBuilder();

                        drpTblStrBldr.Append("DROP TABLE IF EXISTS ");
                        drpTblStrBldr.Append(tableName);
                        drpTblStrBldr.Append(" ;");
                        queryStringList.Add(drpTblStrBldr.ToString());

                        crtTblStrBldr.Append("CREATE TABLE ");
                        crtTblStrBldr.Append(tableName);
                        crtTblStrBldr.Append(" (_id INTEGER PRIMARY KEY AUTOINCREMENT");
                        //Fields
                        ListObjBase<TTableFldDef> tTableFldDefList = tTableDef.HighTableRevision.Fields;
                        int noOfColms = 0;
                        foreach (TTableFldDef tTableFldDef in tTableFldDefList)
                        {
                            crtTblStrBldr.Append(", ");
                            crtTblStrBldr.Append("[" + tTableFldDef.Name + "]");

                            if (noOfColms != 0)
                                colNamesStrBldr.Append(", ");
                            noOfColms++;
                            colNamesStrBldr.Append("[" + tTableFldDef.Name + "]");
                            //string dataType = convertDataType (tTableFldDef.EditDataType);
                            crtTblStrBldr.Append(" ");
                            crtTblStrBldr.Append(" ntext");
                        }
                        crtTblStrBldr.Append(");");
                        queryStringList.Add(crtTblStrBldr.ToString());

                        //Write date to the file and store it in the downloads folder with file name Temp
                        String tempDatFilePath1 =
                            Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads) + "/" + tTableDef.HighTableRevision.DATFileName;

                        Console.WriteLine("DAT FILE NAME-------------------------: " + tTableDef.HighTableRevision.DATFileName);

                        //Loop through Temp file and build string INSERT QUERIES
                        queryStringList.Clear();
                        BuildInsertQueries(tempDatFilePath1, tTableDef, colNamesStrBldr, noOfColms, queryStringList);

                        // TODO - we need to figure how to do direct DB load from the file. faster and more mem efficient


                        //Loop and execuete queries
                        var dbManager = new DatabaseManager();
                        foreach (string command in queryStringList)
                        {
                            try
                            {
                                totalRowCount = totalRowCount + dbManager.ExecuteNonQuery(command);
                            }
                            catch (Exception e)
                            {
                                LoggingManager.LogApplicationError(e, "query: " + command, "BuildListItemTablesInDatabase");
                            }
                        }


                    }
                    catch (Exception e)
                    {
                        LoggingManager.LogApplicationError(e, "ListName: " + tableName, "BuildListItemTablesInDatabase");
                    }

                }

            } //Tag List End



        }

#endif


        private void BuildTablesAndImportHotSheetDATFilesIntoDatabase()
        {
            var queryStringList = new ArrayList();

            //Only for Logging purpose total count
            int totalRowCount = 0;

            string tableName = string.Empty;

            Reino.ClientConfig.TIssStructMgr myIssStructMgr = DroidContext.XmlCfg.clientDef.IssStructMgr;
            foreach (TIssStruct oneStruct in myIssStructMgr.IssStructs)
            {
                // only looking to process hotsheet structs
                if (oneStruct is THotSheetStruct)
                {
                    foreach (TTableDef tTableDef in oneStruct.TableDefs)
                    {

                        try
                        {

                            tableName = tTableDef.Name;

                            SendBroadcastProgressUpdate((int)Constants.SyncStepEnummeration.SyncStep_ProcessListFiles, "Processing Hot List " + tableName + "...");

                            //Build string create table query based on table and column names
                            var crtTblStrBldr = new StringBuilder();
                            //Drop Table if Exists
                            var drpTblStrBldr = new StringBuilder();
                            //Build string for column names
                            var colNamesStrBldr = new StringBuilder();

                            drpTblStrBldr.Append("DROP TABLE IF EXISTS ");
                            drpTblStrBldr.Append(tableName);
                            drpTblStrBldr.Append(" ;");
                            queryStringList.Add(drpTblStrBldr.ToString());

                            crtTblStrBldr.Append("CREATE TABLE ");
                            crtTblStrBldr.Append(tableName);
                            crtTblStrBldr.Append(" ( ");


                            //Fields
                            ListObjBase<TTableFldDef> tTableFldDefList = tTableDef.HighTableRevision.Fields;
                            int loColumnCreateCounter = 0;
                            int loColumnInsertCounter = 0;

                            foreach (TTableFldDef oneFldDef in tTableFldDefList)
                            {
                                // creating one list for the CREATE TABLE
                                if (loColumnCreateCounter > 0)
                                {
                                    crtTblStrBldr.Append(", ");
                                }
                                loColumnCreateCounter++;


                                crtTblStrBldr.Append("[" + oneFldDef.Name + "]");
                                if (oneFldDef.Name.Equals(Constants.ID_COLUMN) == true)
                                {
                                    crtTblStrBldr.Append(" INTEGER PRIMARY KEY AUTOINCREMENT");
                                }
                                else
                                {
                                    // except for the keys/foreign keys, all other fields are strings
                                    //string dataType = convertDataType (tTableFldDef.EditDataType);
                                    crtTblStrBldr.Append(" ntext");
                                }

                                // for the INSERT loads, we aren't reading in host only columns
                                if (oneFldDef.IsHostSideDefinitionOnly == false)
                                {
                                    // and another column list for the INSERT
                                    if (loColumnInsertCounter > 0)
                                    {
                                        colNamesStrBldr.Append(", ");
                                    }
                                    loColumnInsertCounter++;

                                    colNamesStrBldr.Append("[" + oneFldDef.Name + "]");
                                }

                            }
                            crtTblStrBldr.Append(");");
                            queryStringList.Add(crtTblStrBldr.ToString());

                        
                            //String tempDatFilePath1 =
                            //    Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads) + "/" + tTableDef.HighTableRevision.DATFileName;

                            Console.WriteLine("DAT FILE NAME-------------------------: " + tTableDef.HighTableRevision.DATFileName);



                            // AJW - TODO - we need to implement optimized inserts prepared statements, or direct load from the file. faster and more mem efficient

                            // progress while we go read the file in
                            SendBroadcastProgressUpdate((int)Constants.SyncStepEnummeration.SyncStep_ProcessListFiles, "Processing Hot List " + tableName + "...");

                            //Loop through Temp file and build string INSERT QUERIES
                            queryStringList.Clear();

                            // if the file exists, we can load it
                            //if (File.Exists(tempDatFilePath1) == true)
                            //{
                            //    BuildInsertQueries(tempDatFilePath1, tTableDef, colNamesStrBldr, loColumnInsertCounter, queryStringList);
                            //}
                            //else
                            //{
                            //    Console.WriteLine("DAT FILE NAME (FILE NOT FOUND) -------: " + tTableDef.HighTableRevision.DATFileName);
                            //}

                            BuildInsertQueriesFromAIFileSystem(tTableDef.HighTableRevision.DATFileName, tTableDef, colNamesStrBldr, loColumnInsertCounter, queryStringList);



                            int loTotalCountInt = queryStringList.Count;
                            string loTotalCountStr = loTotalCountInt.ToString();

                            Console.WriteLine("DAT FILE NAME-------------------------: " + tTableDef.HighTableRevision.DATFileName );
                            Console.WriteLine("IMPORTING RECORDS (NOT OPTIMIZED)-----: " + loTotalCountStr );



                            //Loop and execuete queries
                            int loCounter = 0;
                            var dbManager = new DatabaseManager();
                            foreach (string command in queryStringList)
                            {
                                try
                                {
                                    totalRowCount = totalRowCount + dbManager.ExecuteNonQuery(command);

                                    // track
                                    loCounter++;

                                    // update UI every so often
                                    if ((loCounter % 100) == 0)
                                    {
                                        int percentComplete = (int)Math.Round((double)(100 * loCounter) / loTotalCountInt);
                                        string loPctComplete = percentComplete.ToString() + "%";

                                        SendBroadcastProgressUpdate((int)Constants.SyncStepEnummeration.SyncStep_ProcessListFiles, "Processing Hot List " + tableName + " " + loPctComplete);
                                    }
                                }
                                catch (Exception e)
                                {
                                    LoggingManager.LogApplicationError(e, "query: " + command, "BuildTablesAndImportHotSheetmDATFilesIntoDatabase");
                                }
                            }


                        }
                        catch (Exception e)
                        {
                            LoggingManager.LogApplicationError(e, "ListName: " + tableName, "BuildTablesAndImportHotSheetmDATFilesIntoDatabase");
                        }

                    }

                } //Tag List End

            }
        }


        //#if _based_on_listnames_only_
        //Retrive List data from the web service and insert into phone Sqlite DB
        public string InsertListData()
        {

            try
            {
                //List of calls to be made to retrieve list data, each agency is one table
                List<string> loListTableNames = DroidContext.XmlCfg.ListTableNames;

                //List of tables and fields defined for each agency(table)
                List<TAgList> tagLists = DroidContext.XmlCfg.AgencyListTableDefsManager;

                foreach (string oneAgencyList in loListTableNames)
                {

                    SendBroadcastProgressUpdate( (int)Constants.SyncStepEnummeration.SyncStep_ProcessListFiles, "Processing List " + oneAgencyList + "..." );

                    BuildListItemTablesInDatabase(oneAgencyList, tagLists);
                }


                // now do the hotsheets
                BuildTablesAndImportHotSheetDATFilesIntoDatabase();
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, null, "InsertListData");
            }

            return "Success";
        }



        private void BuildListItemTablesInDatabase(string iListItemTableName, IEnumerable<TAgList> tagLists)
        {
            var queryStringList = new ArrayList();

            try
            {
                //Get file data for each table/agency response is DAT file

                //Find out table name, no of columns and there names for each agency
                foreach (TAgList tAgList in tagLists)
                {
                    var tableDefs = tAgList.TableDefs;
                    foreach (TTableDef tTableDef in tableDefs)
                    {
                        string tableName = tTableDef.Name;
                        if (tableName == iListItemTableName)
                        {
                            //Build string create table query based on table and column names
                            var crtTblStrBldr = new StringBuilder();
                            //Drop Table if Exists
                            var drpTblStrBldr = new StringBuilder();
                            //Build string for column names
                            var colNamesStrBldr = new StringBuilder();

                            drpTblStrBldr.Append("DROP TABLE IF EXISTS ");
                            drpTblStrBldr.Append(tableName);
                            drpTblStrBldr.Append(" ;");
                            queryStringList.Add(drpTblStrBldr.ToString());


                            crtTblStrBldr.Append("CREATE TABLE ");
                            crtTblStrBldr.Append(tableName);
                            crtTblStrBldr.Append(" ( ");


                            //Fields
                            ListObjBase<TTableFldDef> tTableFldDefList = tTableDef.HighTableRevision.Fields;
                            int loColumnCreateCounter = 0;
                            int loColumnInsertCounter = 0;


                            //// AJW - would like to have this in the TTableDef eventually
                            //// since the agency list tables doesn't inclue an SQL identiny column, 
                            //// add it manually to the CREATE (but not to the INSERT)
                            ////if ( tTableDef.HighTableRevision.Fields.IndexOf( 
                            //crtTblStrBldr.Append("[" + Constants.ID_COLUMN + "] INTEGER PRIMARY KEY AUTOINCREMENT");
                            //loColumnCreateCounter++;


                            foreach (TTableFldDef oneFldDef in tTableFldDefList)
                            {
                                // creating one list for the CREATE TABLE
                                if (loColumnCreateCounter > 0)
                                {
                                    crtTblStrBldr.Append(", ");
                                }
                                loColumnCreateCounter++;


                                crtTblStrBldr.Append("[" + oneFldDef.Name + "]");
                                if (oneFldDef.Name.Equals(Constants.ID_COLUMN) == true)
                                {
                                    crtTblStrBldr.Append(" INTEGER PRIMARY KEY AUTOINCREMENT");
                                }
                                else
                                {
                                    // except for the keys/foreign keys, all other fields are strings
                                    //string dataType = convertDataType (tTableFldDef.EditDataType);
                                    crtTblStrBldr.Append(" ntext");
                                }

                                // for the INSERT loads, we aren't reading in host only columns
                                if (oneFldDef.IsHostSideDefinitionOnly == false)
                                {
                                    // and another column list for the INSERT
                                    if (loColumnInsertCounter > 0)
                                    {
                                        colNamesStrBldr.Append(", ");
                                    }
                                    loColumnInsertCounter++;

                                    colNamesStrBldr.Append("[" + oneFldDef.Name + "]");
                                }

                            }
                            crtTblStrBldr.Append(");");
                            queryStringList.Add(crtTblStrBldr.ToString());


#if DEBUG
                            //Write date to the file and store it in the downloads folder with file name Temp
                            String tempDatFilePath1 =
                                Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads) + "/" + tTableDef.HighTableRevision.DATFileName;
#endif


                            //// if the file exists, we can load it
                            //if (File.Exists(tempDatFilePath1) == true)
                            //{
                            //    //Loop through Temp file and build string INSERT QUERIES
                            //    Console.WriteLine("DAT FILE NAME-------------------------: " + tTableDef.HighTableRevision.DATFileName);
                            //    BuildInsertQueries(tempDatFilePath1, tTableDef, colNamesStrBldr, loColumnInsertCounter, queryStringList);
                            //}
                            //else
                            //{
                            //    Console.WriteLine("DAT FILE NAME (FILE NOT FOUND) -------: " + tTableDef.HighTableRevision.DATFileName);
                            //}


                            // convert from raw file bytes to db tables
                            Console.WriteLine("DAT FILE NAME-------------------------: " + tTableDef.HighTableRevision.DATFileName);
                            BuildInsertQueriesFromAIFileSystem(tTableDef.HighTableRevision.DATFileName, tTableDef, colNamesStrBldr, loColumnInsertCounter, queryStringList);


                        }
                    }
                } //Tag List End

                //Only for Logging purpose total count
                int totalRowCount = 0;
                int loTotalCountInt = queryStringList.Count;

                //Loop and execuete queries
                int loCounter = 0;
                var dbManager = new DatabaseManager();
                foreach (string command in queryStringList)
                {
                    totalRowCount = totalRowCount + dbManager.ExecuteNonQuery(command);

                    // track
                    loCounter++;

                    // update UI every so often
                    if ((loCounter % 100) == 0)
                    {
                        int percentComplete = (int)Math.Round((double)(100 * loCounter) / loTotalCountInt);
                        string loPctComplete = percentComplete.ToString() + "%";

                        SendBroadcastProgressUpdate((int)Constants.SyncStepEnummeration.SyncStep_ProcessListFiles, "Processing List " + iListItemTableName  + " " + loPctComplete);
                    }

                }
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, "ListName: " + iListItemTableName, "BuildListItemTablesInDatabase");
            }
        }


        private static void BuildInsertQueriesFromAIFileSystem(string iAIFileSystemFileName,
                                                TTableDef iTableDef,
                                                StringBuilder colNamesStrBldr,
                                                int noOfColms,
                                                ArrayList queryStringList)
        {
            string loTableName = iTableDef.Name;



            // kludge! temporary wireless support 
            if (string.IsNullOrEmpty(loTableName) == false)
            {
                switch (loTableName)
                {
                    case "HANDICAP":
                    case "PLATELIST":
                    case "VINLIST":
                        {
                            // KLUDGE - don't load force these to be wireless search. TODO: Later need to fix host to respect the System HotSheet OPtions and not send these files
                            return;
                        }
                    default:
                        {
                            // 
                            break;
                        }
                }
            }




            AIFileSystemDAO loFileSystemData = ClientFileManager.GetDataForFileSystemFile(iAIFileSystemFileName);

            // not in there? 
            if (loFileSystemData == null)
            {
                // nothing to do
                return;
            }


            // todo - may need to specify encoding....
            using (var sr = new StreamReader( new MemoryStream(loFileSystemData.FILEDATA), Encoding.Default))
            {

                /* spinners gone, don't want to add blank entries anymore

                // insert row of blank string entries as first item in every list - for our spinners to have an empty value to select from
                var emptyRowStringBuilder = new StringBuilder();
                emptyRowStringBuilder.Append("INSERT INTO ");
                emptyRowStringBuilder.Append(loTableName);
                emptyRowStringBuilder.Append(" ( ");
                emptyRowStringBuilder.Append(colNamesStrBldr);
                emptyRowStringBuilder.Append(" ) ");
                emptyRowStringBuilder.Append("VALUES ( ");

                // always have at least one column
                int loEmptyColIdx = 1;
                emptyRowStringBuilder.Append("' '");  // set our empty row as string constructed as single space character

                // add more as needed
                if (loEmptyColIdx < noOfColms)
                {
                    int loColumnsNeeded = noOfColms - loEmptyColIdx;
                    for (int loColAdded = 0; loColAdded < loColumnsNeeded; loColAdded++)
                    {
                        emptyRowStringBuilder.Append(", ");
                        emptyRowStringBuilder.Append("' '");  // set our empty row as string constructed as single space character
                    }
                }
                emptyRowStringBuilder.Append(" );");
                queryStringList.Add(emptyRowStringBuilder.ToString());
                */


                while (sr.Peek() >= 0)
                {
                    string content = sr.ReadLine();
                    string[] data = content.Split('\t');
                    var instRowStrBldr = new StringBuilder();
                    instRowStrBldr.Append("INSERT INTO ");
                    instRowStrBldr.Append(loTableName);
                    instRowStrBldr.Append(" ( ");
                    instRowStrBldr.Append(colNamesStrBldr);
                    instRowStrBldr.Append(" ) ");
                    instRowStrBldr.Append("VALUES ( ");
                    int loColIdx = 0;
                    foreach (string colVal in data)
                    {
                        var value = colVal;
                        //Below condition to make sure the no of values columns equal to columns in table fields
                        if (loColIdx < noOfColms)
                        {
                            if (loColIdx != 0)
                            {
                                instRowStrBldr.Append(", ");
                            }
                            //loColIdx++;  still need the current value, inc at the end of loop
                            instRowStrBldr.Append("'");


                            //lets strip down the colval to only have the data we need

                            // cases when text has apostrophes 
                            value = value.Replace('\'', ' ');

                            // AJW - we only want to strip leading zeros when we are dealing with a numeric - leading zeros in a string field were intended to be retained
                            /*
                            //get rid of any leading zeros as well 
                            //(their int values come in as 00001, etc and when we filter on this, we only use 1, so 0's have to go)

                            //make sure we dont strip all zeros, so set a temp value, and afterwards check to see if anythig is in it.
                            //this iwll prevent us from removing valid 0 items
                            var tempvalue = value.TrimStart('0');
                            if (!string.IsNullOrEmpty(tempvalue))
                                value = tempvalue;
                             * */

                            // only numeric fields should be stripped
                            var loFieldDef = iTableDef.GetField(loColIdx);
                            //if (typeof(loFieldDef) == typeof(TTableIntFldDef))
                            if ((loFieldDef is TTableIntFldDef) || (loFieldDef is TTableRealFldDef))
                            {
                                var tempvalue = value.TrimStart('0');
                                if (!string.IsNullOrEmpty(tempvalue))
                                    value = tempvalue;
                            }

                            instRowStrBldr.Append(value);
                            instRowStrBldr.Append("'");

                            // ready for next
                            loColIdx++;
                        }
                    }

                    //Below condition to make sure the no of table fields columns equal to values columns
                    if (loColIdx < noOfColms)
                    {
                        int diffNoOfColms = noOfColms - loColIdx;
                        for (int k = 0; k < diffNoOfColms; k++)
                        {
                            instRowStrBldr.Append(", ");
                            instRowStrBldr.Append("''");
                        }
                    }
                    instRowStrBldr.Append(" );");
                    queryStringList.Add(instRowStrBldr.ToString());
                }
            }
        }



        private static void BuildInsertQueries( string tempDatFilePath1,
                                                TTableDef iTableDef,
                                                StringBuilder colNamesStrBldr,
                                                int noOfColms, 
                                                ArrayList queryStringList)
        {
            string loTableName = iTableDef.Name;


            using (var sr = new StreamReader(tempDatFilePath1))
            {

                /* spinners gone, don't want to add blank entries anymore

                // insert row of blank string entries as first item in every list - for our spinners to have an empty value to select from
                var emptyRowStringBuilder = new StringBuilder();
                emptyRowStringBuilder.Append("INSERT INTO ");
                emptyRowStringBuilder.Append(loTableName);
                emptyRowStringBuilder.Append(" ( ");
                emptyRowStringBuilder.Append(colNamesStrBldr);
                emptyRowStringBuilder.Append(" ) ");
                emptyRowStringBuilder.Append("VALUES ( ");

                // always have at least one column
                int loEmptyColIdx = 1;
                emptyRowStringBuilder.Append("' '");  // set our empty row as string constructed as single space character

                // add more as needed
                if (loEmptyColIdx < noOfColms)
                {
                    int loColumnsNeeded = noOfColms - loEmptyColIdx;
                    for (int loColAdded = 0; loColAdded < loColumnsNeeded; loColAdded++)
                    {
                        emptyRowStringBuilder.Append(", ");
                        emptyRowStringBuilder.Append("' '");  // set our empty row as string constructed as single space character
                    }
                }
                emptyRowStringBuilder.Append(" );");
                queryStringList.Add(emptyRowStringBuilder.ToString());
                */


                while (sr.Peek() >= 0)
                {
                    string content = sr.ReadLine();
                    string[] data = content.Split('\t');
                    var instRowStrBldr = new StringBuilder();
                    instRowStrBldr.Append("INSERT INTO ");
                    instRowStrBldr.Append(loTableName);
                    instRowStrBldr.Append(" ( ");
                    instRowStrBldr.Append(colNamesStrBldr);
                    instRowStrBldr.Append(" ) ");
                    instRowStrBldr.Append("VALUES ( ");
                    int loColIdx = 0;
                    foreach (string colVal in data)
                    {
                        var value = colVal;
                        //Below condition to make sure the no of values columns equal to columns in table fields
                        if (loColIdx < noOfColms)
                        {
                            if (loColIdx != 0)
                            {
                                instRowStrBldr.Append(", ");
                            }
                            //loColIdx++;  still need the current value, inc at the end of loop
                            instRowStrBldr.Append("'");


                            //lets strip down the colval to only have the data we need

                            // cases when text has apostrophes 
                            value = value.Replace('\'', ' ');

                            // AJW - we only want to strip leading zeros when we are dealing with a numeric - leading zeros in a string field were intended to be retained
                            /*
                            //get rid of any leading zeros as well 
                            //(their int values come in as 00001, etc and when we filter on this, we only use 1, so 0's have to go)

                            //make sure we dont strip all zeros, so set a temp value, and afterwards check to see if anythig is in it.
                            //this iwll prevent us from removing valid 0 items
                            var tempvalue = value.TrimStart('0');
                            if (!string.IsNullOrEmpty(tempvalue))
                                value = tempvalue;
                             * */

                            // only numeric fields should be stripped
                            var loFieldDef = iTableDef.GetField(loColIdx);
                            //if (typeof(loFieldDef) == typeof(TTableIntFldDef))
                            if ((loFieldDef is TTableIntFldDef) || ( loFieldDef is TTableRealFldDef))
                            {
                                var tempvalue = value.TrimStart('0');
                                if (!string.IsNullOrEmpty(tempvalue))
                                    value = tempvalue;
                            }
                            
                            instRowStrBldr.Append(value);
                            instRowStrBldr.Append("'");

                            // ready for next
                            loColIdx++;
                        }
                    }

                    //Below condition to make sure the no of table fields columns equal to values columns
                    if (loColIdx < noOfColms)
                    {
                        int diffNoOfColms = noOfColms - loColIdx;
                        for (int k = 0; k < diffNoOfColms; k++)
                        {
                            instRowStrBldr.Append(", ");
                            instRowStrBldr.Append("''");
                        }
                    }
                    instRowStrBldr.Append(" );");
                    queryStringList.Add(instRowStrBldr.ToString());
                }
            }
        }


        //Get dropdown list by table and column name
        public  string[]  GetListDataByTableColumnName(string tableName, string columnName)
        {
            return GetListData("SELECT " + columnName + " FROM " + tableName);
        }

        /// <summary>
        /// Gets the drop down data source filtered but hte filters passed in.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public string[] GetFilteredListData(string tableName, string columnName, List<ListFilter> filters)
        {
            //if this is the case, use the value at the parents index instead of the selected value.
            string whereClause = "";
            //foreach filter, build a where clause
            if (filters != null)
            {
                if (filters.Count > 0)
                {
                    whereClause = BuildWhereClause(filters);
                }
                else
                {
                    // debug breakpoint
                    whereClause = string.Empty;
                }
            }

            return GetListData("SELECT " + columnName + " FROM " + tableName + " " + whereClause);
        }

        /// <summary>
        /// Builds a dynamic where clause based on the filters passed in.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        private string BuildWhereClause(IEnumerable<ListFilter> filters)
        {
            StringBuilder whereClause = new StringBuilder();

            foreach (var filter in filters)
            {
                if (whereClause.Length == 0)
                {
                    whereClause.Append(" WHERE ");
                }
                else
                {
                    whereClause.Append(" AND ");
                }


                //if the filter has use index, then we have to go get the value associated with the selected index of the parent instead of the text selected.
                if (filter.FilterByIndex)
                {
                    //  //get the items for this behaviors list
                    //whereClause += string.IsNullOrEmpty(whereClause)? " Where [" + filter.Column + "] = " + filter.Index 
                    //    : " AND [" + filter.Column + "] = " + filter.Index;

                    whereClause.Append(" [" + filter.Column + "] = " + filter.Index.ToString());
                }
                else
                {
                    //if not using index, use the filter value. 
                    // this may have been modified in the restriction (dates and formats) 
                    // so make sure to use the value in the filter an not the value of the parent.
                    //get the items value
                    var parentValueToUse = filter.Value;
                    if (string.IsNullOrEmpty(parentValueToUse) == true)
                    {
                        parentValueToUse = string.Empty;
                    }

                    // //only add to the clause if we found a value
                    //if (!string.IsNullOrEmpty(parentValueToUse) )
                    //{
                    //    //also filter out any spinner defaults that may have been set.
                    //    //if (parentValueToUse != Constants.SPINNER_DEFAULT)
                    //    {
                    //        //use that value int he where statement
                    //        whereClause += string.IsNullOrEmpty(whereClause) ? " Where [" + filter.Column + "] = " + parentValueToUse
                    //                          : " AND [" + filter.Column + "] = " + parentValueToUse;
                    //    }
                    //}


                    whereClause.Append(" [" + filter.Column + "] = '" + parentValueToUse + "'");


                }
            }
            return whereClause.ToString();
        }

        private string[] GetListData(string sqlCommand)
        {
            var result = (new DatabaseManager()).GetListData(sqlCommand);
            return result;
        }


        public  string  GetDataFromTableWithColumnValue(string columnData, string tableName, string columnName, string columnValue)
        {
            var query = "SELECT * FROM " + tableName + " WHERE [" + columnName + "] = '" + columnValue + "'";
            var result =  (new DatabaseManager()).GetDataFromTableWithColumnValue(query, columnData);
            return result;
        }

        public  DataTable GetAllFromTableWithColumnValue(string tableName, string columnName, string columnValue)
        {
            var query = "SELECT * FROM " + tableName + " WHERE [" + columnName + "] = '" + columnValue + "'";
            var result =  (new DatabaseManager()).GetAllFromTableWithColumnValue(query);
            return result;
        }
    }
}

