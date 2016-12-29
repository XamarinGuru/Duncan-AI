using System;
using System.Data;
using System.Text;
using Android.Content;
using Android.Util;
using Duncan.AI.Droid.Utils.DataAccess;
using Duncan.AI.Droid.Utils.HelperManagers;
using Duncan.AI.Droid.Utils;
using Reino.ClientConfig;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using Android.Preferences;
using XMLConfig;
using System.Linq.Expressions;
using System.Linq;

namespace Duncan.AI.Droid
{
	public class CommonADO
	{
		public CommonADO ()
		{
		}

        ////Match XML field types with SQLite database types
        //public static string ConvertDataType(TTableFldDef.TEditDataType tEditDataType)
        //{
        //    if (tEditDataType == TTableFldDef.TEditDataType.tftInteger || tEditDataType == TTableFldDef.TEditDataType.tftReal) {
        //        return "INTEGER";
        //    }else if (tEditDataType == TTableFldDef.TEditDataType.tftString || tEditDataType == TTableFldDef.TEditDataType.tftTime) {
        //        return "ntext";
        //    }else if (tEditDataType == TTableFldDef.TEditDataType.tftDate) {
        //        return "datetime";
        //    }
        //    return "ntext"; // default to string
        //}


#if _original
		//Build and execute create query for given issue_ap xml table
		public static string CreateTable(XMLConfig.TableDef tableDef)
		{
			string isGps = "false";
			try{
				var dropTblStrBldr = new StringBuilder();
				dropTblStrBldr.Append ("DROP TABLE IF EXISTS ");
				dropTblStrBldr.Append (tableDef.Name);
				dropTblStrBldr.Append(" ;");

				var crtTblStrBldr = new StringBuilder();
				//crtTblStrBldr.Append ("CREATE TABLE IF NOT EXISTS ");
				crtTblStrBldr.Append ("CREATE TABLE ");
				crtTblStrBldr.Append (tableDef.Name);
				crtTblStrBldr.Append (" ( ");
				crtTblStrBldr.Append (Constants.ID_COLUMN);
				crtTblStrBldr.Append (" INTEGER PRIMARY KEY AUTOINCREMENT");
				
				List<XMLConfig.TableDefField> tableDefFieldList = tableDef.TableDefFields;
			    foreach (XMLConfig.TableDefField tableDefField in tableDefFieldList)
			    {
			        crtTblStrBldr.Append(", ");
			        crtTblStrBldr.Append(tableDefField.Name);
			        crtTblStrBldr.Append(" ");
                    crtTblStrBldr.Append(" ntext");
                                      
                    if (Constants.STRUCT_NAME_ACTIVITYLOG == tableDef.Name && Constants.LATITUDE_ACTIVITYLOG == tableDefField.Name)
			            isGps = "true";
			    }

			    crtTblStrBldr.Append (", ");
				crtTblStrBldr.Append (Constants.STATUS_COLUMN);
				crtTblStrBldr.Append (" ");
				crtTblStrBldr.Append ("ntext");
                    crtTblStrBldr.Append(", ");
                    crtTblStrBldr.Append(Constants.WS_STATUS_COLUMN);
                    crtTblStrBldr.Append(" ");
                    crtTblStrBldr.Append("ntext");

				if (Constants.PARKNOTE_TABLE == tableDef.Name
                    || Constants.PARKVOID_TABLE == tableDef.Name
                    || Constants.PARKREISSUE_TABLE == tableDef.Name) {
					crtTblStrBldr.Append (", ");
					crtTblStrBldr.Append (Constants.SEQUENCE_ID);
					crtTblStrBldr.Append (" ");
					crtTblStrBldr.Append ("ntext");
				}

				crtTblStrBldr.Append (" );");

				bool isDropSucc = ExecuteQuery (dropTblStrBldr.ToString ());
				bool isCrtSucc = ExecuteQuery (crtTblStrBldr.ToString ());
			}catch(Exception e){
                LoggingManager.LogApplicationError(e, null, "CreateTable");
				Console.WriteLine("Exception source:", e.Source);
			}
			return isGps;
		}
#endif

        //Build and execute create query for given issue_ap xml table
        public static string CreateTable(XMLConfig.TableDef tableDef)
        {
            string isGps = "false";
            try
            {
                var dropTblStrBldr = new StringBuilder();
                dropTblStrBldr.Append("DROP TABLE IF EXISTS ");
                dropTblStrBldr.Append(tableDef.Name);
                dropTblStrBldr.Append(" ;");

                var crtTblStrBldr = new StringBuilder();
                crtTblStrBldr.Append("CREATE TABLE ");
                crtTblStrBldr.Append(tableDef.Name);
                crtTblStrBldr.Append(" ( ");

                int loColumnCounter = 0;
                foreach (TTableFldDef oneFldDef in tableDef.fTableDefRev.Fields)
                {
                    if (loColumnCounter > 0)
                    {
                        crtTblStrBldr.Append(", ");
                    }
                    loColumnCounter++;


                    crtTblStrBldr.Append(oneFldDef.Name);

                    if (oneFldDef.Name.Equals(Constants.ID_COLUMN) == true)
                    {
                        crtTblStrBldr.Append(" INTEGER PRIMARY KEY AUTOINCREMENT");
                    }
                    else
                    {
                        // except for the keys/foreign keys, all other fields are strings
                        crtTblStrBldr.Append(" ntext");
                    }


                    // AJW TOD - clean up this kludge
                    if (Constants.STRUCT_NAME_ACTIVITYLOG == tableDef.Name && Constants.LATITUDE_ACTIVITYLOG == oneFldDef.Name)
                        isGps = "true";

                }


                // check 
                //tableDef.fTableDefRev.Fields == tableDef.TableDefFields

                crtTblStrBldr.Append(" );");

                bool isDropSucc = ExecuteQuery(dropTblStrBldr.ToString());
                bool isCrtSucc = ExecuteQuery(crtTblStrBldr.ToString());
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, null, "CreateTable");
                Console.WriteLine("Exception source:", e.Source);
            }
            return isGps;
        }
    


		//Insert Row into the database 
        //
        //  AJW - TODO - rewrite this with stricter proper type handling and object ennumeration/flow 
        //
        public Task<bool> InsertRow(XMLConfig.IssStruct structType, Context ctx, string tableName, string iMasterKey )
		{
			return Task.Factory.StartNew (() => { 
				try
				{
					var colsStrBldr = new StringBuilder();
					var valStrBldr = new StringBuilder();
                    List<XMLConfig.TableDef> tableDefList = DroidContext.XmlCfg.TableDefs;
                    TableDef tableDef = tableDefList.Find(x => x != null && x.Name == structType.Name);


                    // we'll already need to read
                    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(DroidContext.ApplicationContext);
                    // open the global datastore for updates
                    ISharedPreferencesEditor editor = prefs.Edit();

                    // is this a final commit?
                    bool loUpdateGlobalDatastore = false; //  (status.Equals(Constants.STATUS_READY) == true);
                    if (loUpdateGlobalDatastore == true)
                    {
                    }


					int noOfColms = 0;
					for(int i = 0; i < structType.Panels.Count; i++)
					{
						foreach (XMLConfig.PanelField panelField in structType.Panels[i].PanelFields) 
                        {
							/* if (panelField.IsHidden)
								continue; */

                            // if this is non-null after evaluation, it will be used in place of panelfield.value
                            string loDBFormattedOverrideString = null;


							if (Constants.ISSUENO_COLUMN == panelField.Name) 
                            {
								panelField.Value = structType.sequenceId;
							}

                            if (panelField.Name.Equals(Constants.ISSUEDATE_COLUMN))
                            {
                                string loTmpBuf = "";
                                DateTime dtNow = DateTime.Today;
                                DateTimeManager.OsDateToDateString(dtNow, panelField.EditMask, ref loTmpBuf);
                                panelField.Value = loTmpBuf;
                            }

                            if (panelField.Name.Equals(Constants.ISSUETIME_COLUMN))
                            {
                                string loTmpBuf = "";
                                DateTime dtNow = DateTime.Now;
                                DateTimeManager.OsTimeToTimeString(dtNow, panelField.EditMask, ref loTmpBuf);
                                panelField.Value = loTmpBuf;
                            }


                            if (Constants.ISSUENOPFX_COLUMN == panelField.Name)
                            {
                                string prefix = prefs.GetString(Constants.SRCISSUENOPFX_COLUMN, null);
                                panelField.Value = prefix;
                            }

                            if (Constants.ISSUENOSFX_COLUMN == panelField.Name)
                            {
                                string suffix = prefs.GetString(Constants.SRCISSUENOSFX_COLUMN, null);
                                panelField.Value = suffix;
                            }



                            // format the date/time types
                            switch (panelField.FieldType)
                            {
                                case "efTime":          // this is really lame type evaluation :-(
                                    {
                                        if ((panelField.Value != null) && (panelField.EditMask != null))
                                        {
                                            if ((panelField.Value.Length > 0) && (panelField.EditMask.Length > 0))
                                            {

                                                // define the value in fixed format for DB
                                                loDBFormattedOverrideString = DateTimeHelper.ConvertTimeColumnValueToFixedDBString(panelField.Value, panelField.EditMask);
                                            }
                                        }
                                        break;
                                    }

                                case "efDate":
                                    {
                                        if ((panelField.Value != null) && (panelField.EditMask != null))
                                        {
                                            if ((panelField.Value.Length > 0) && (panelField.EditMask.Length > 0))
                                            {
                                                // define the value in fixed format for DB
                                                loDBFormattedOverrideString = DateTimeHelper.ConvertDateColumnValueToFixedDBString(panelField.Value, panelField.EditMask);
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }




                            // default to using the panel value
                            string loValueToStoreInDB = panelField.Value;

                            // allow override to take
                            if (loDBFormattedOverrideString != null)
                            {
                                loValueToStoreInDB = loDBFormattedOverrideString;
                            }


                            if (string.IsNullOrEmpty(loValueToStoreInDB) == false)
                            {
                                // remove abbrev descriptions when present
                                int loPosSep = loValueToStoreInDB.IndexOf(Constants.LIST_ITEM_DESCRIPTION_SEPARATOR);
                                // has to be beyond the first char
                                if (loPosSep > 0)
                                {
                                    // keep everything up to the space preceeding the seperator char
                                    loValueToStoreInDB = loValueToStoreInDB.Substring(0, loPosSep - 1);
                                }
                            }




                            if (loValueToStoreInDB != null
                                && tableDef.TableDefFields.Any(x => x.Name == panelField.Name))
                            {
                                if (noOfColms != 0) 
                                {
									colsStrBldr.Append (", ");
									valStrBldr.Append (", ");
								}
								noOfColms++;

								colsStrBldr.Append (panelField.Name);

								valStrBldr.Append ("'");
								//Cases when text has apostrophes 
                                valStrBldr.Append(loValueToStoreInDB.Replace('\'', ' '));
								valStrBldr.Append ("'");
							}


                            if (loUpdateGlobalDatastore == true)
                            {

                                // update the global datastore
                                editor.PutString(Helper.BuildGlobalPreferenceKeyName(structType.Name, panelField.Name), loValueToStoreInDB);
                            }
                           
						}
					}


                    if (Constants.STRUCT_TYPE_ACTIVITYLOG == structType.Type)
					{
						colsStrBldr.Append (", ");
						colsStrBldr.Append (Constants.STATUS_COLUMN);
						valStrBldr.Append (", '");
						valStrBldr.Append (Constants.STATUS_READY);
						valStrBldr.Append ("'");
						colsStrBldr.Append (", ");
						colsStrBldr.Append (Constants.WS_STATUS_COLUMN);
						valStrBldr.Append (", '");
                        valStrBldr.Append(Constants.WS_STATUS_READY);
						valStrBldr.Append ("'");						
					}

                    if ( structType._TIssStruct is TCiteDetailStruct )
                    //if (Constants.PARKVOID_TABLE == tableName
                    //    || Constants.PARKREISSUE_TABLE == tableName)
                    {
                        colsStrBldr.Append(", ");
                        colsStrBldr.Append(Constants.SEQUENCE_ID);
                        valStrBldr.Append(", '");
                        //valStrBldr.Append(commonDTO.seqId);
                        valStrBldr.Append(iMasterKey);
                        valStrBldr.Append("'");
                    }


                    if (
                         Constants.STRUCT_TYPE_CITE == structType.Type ||
                         Constants.STRUCT_TYPE_CHALKING == structType.Type ||
                         Constants.STRUCT_TYPE_GENERIC_ISSUE == structType.Type
                        )
                    {
						colsStrBldr.Append (", ");
						colsStrBldr.Append (Constants.STATUS_COLUMN);
						valStrBldr.Append (", '");
						valStrBldr.Append (Constants.STATUS_INPROCESS);
						valStrBldr.Append ("'");
						colsStrBldr.Append (", ");
						colsStrBldr.Append (Constants.WS_STATUS_COLUMN);
						valStrBldr.Append (", '");
						valStrBldr.Append (Constants.WS_STATUS_EMPTY);
						valStrBldr.Append ("'");
					}

                    if (tableDef.TableDefFields.Any(x => x.Name == Constants.FORMREV)
                        && structType.PrintPicture != null)
                    {
                        colsStrBldr.Append(", ");
                        colsStrBldr.Append(Constants.FORMREV);
                        valStrBldr.Append(", '");
                        valStrBldr.Append(structType.PrintPicture.Revision);
                        valStrBldr.Append("'");
                    }

                    if (tableDef.TableDefFields.Any(x => x.Name == Constants.FORMNAME)
                        && structType.PrintPicture != null)
                    {
                        colsStrBldr.Append(", ");
                        colsStrBldr.Append(Constants.FORMNAME);
                        valStrBldr.Append(", '");
                        valStrBldr.Append(structType.PrintPicture.Name);
                        valStrBldr.Append("'");
                    }



                    if (loUpdateGlobalDatastore == true)
                    {
                        // commit the global datastore
                        editor.Apply();
                    }



					var instRowStrBldr = new StringBuilder();
					instRowStrBldr.Append ("INSERT INTO ");
					instRowStrBldr.Append (tableName);
					instRowStrBldr.Append (" ( ");
					instRowStrBldr.Append (colsStrBldr.ToString());
					instRowStrBldr.Append (" ) VALUES ( ");
					instRowStrBldr.Append (valStrBldr.ToString());
					instRowStrBldr.Append (" );");

					ExecuteQuery (instRowStrBldr.ToString ());

                    if (Constants.STRUCT_TYPE_ACTIVITYLOG == structType.Type)
                    {
                        ctx.StartService(new Intent(ctx, typeof(SyncService)));
                    }

					return true;
				}
				catch(Exception e)
				{
                    LoggingManager.LogApplicationError(e, null, "InsertRow");
					Console.WriteLine("Exception source: {0}", e.Source);
				}
				return false;
			});
		}

		//Delete Row into the database 
		public static string DeleteRow(string rowId, string tableName)
		{
			var queryStringBuilder = new StringBuilder ();
			queryStringBuilder.Append("DELETE FROM ");
			queryStringBuilder.Append(tableName);
			queryStringBuilder.Append(" WHERE ");
			queryStringBuilder.Append(Constants.ID_COLUMN);
			queryStringBuilder.Append(" = '");
			queryStringBuilder.Append(rowId);
			queryStringBuilder.Append("'");
			ExecuteQuery (queryStringBuilder.ToString());
			return queryStringBuilder.ToString ();
		}

        //Load the issue ticket details from one row
        public void LoadStructFromDataRow(XMLConfig.IssStruct structType, DataRow oneRow)
        {

            for (int loPanelIdx = 0; loPanelIdx < structType.Panels.Count; loPanelIdx++)
            {
                foreach (XMLConfig.PanelField panelField in structType.Panels[loPanelIdx].PanelFields)
                {
                    // AJW - defend against misaligned configurations - fields on form are not always defined in the table
                    if (oneRow.Table.Columns.IndexOf(panelField.Name) != -1)
                    {
                        // AJW - review - we only read from DB if Value is defined? When would it NOT be instantiated?
                        if ((panelField.Value != null) && (oneRow[panelField.Name] != null))
                        {
                            // if this is non-null after evaluation, it will be used in place of db raw string value
                            string loConvertedFromDBFormatOverrideString = null;

                            // safe extraction from datarow
                            string loOneValueAsString = GetSafeColumnStringValueFromDataRow(oneRow, panelField.Name);

                            // format the date/time types
                            switch (panelField.FieldType)
                            {
                                case "efTime":          // this is really lame type evaluation :-(
                                    {
                                        if ((loOneValueAsString != null) && (panelField.EditMask != null))
                                        {
                                            if ((loOneValueAsString.Length > 0) && (panelField.EditMask.Length > 0))
                                            {
                                                // convert from fixed DB format to panel format
                                                loConvertedFromDBFormatOverrideString = DateTimeHelper.ConvertDBTimeColumnValueToString(loOneValueAsString, panelField.EditMask);
                                            }
                                        }
                                        break;
                                    }

                                case "efDate":
                                    {
                                        if ((loOneValueAsString != null) && (panelField.EditMask != null))
                                        {
                                            if ((loOneValueAsString.Length > 0) && (panelField.EditMask.Length > 0))
                                            {
                                                // convert from fixed DB format to panel format
                                                loConvertedFromDBFormatOverrideString = DateTimeHelper.ConvertDBDateColumnValueToString(loOneValueAsString, panelField.EditMask);
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }

                            // use the override if available
                            if (loConvertedFromDBFormatOverrideString != null)
                            {
                                panelField.Value = loConvertedFromDBFormatOverrideString;
                            }
                            else
                            {
                                panelField.Value = loOneValueAsString;
                            }



                        }

                    }
                    else
                    {
                        // warn only if it not a extra field type added on the fly
                        if (panelField.Name.Contains(AutoISSUE.DBConstants.gPanelDividerNameSuffix) == false)
                        {
                            string loErrMsg = "WARNING: XML CONFIG panel field " + panelField.Name + " not defined in table " + oneRow.Table.TableName;
                            string loMethodName = "(Method Name: LoadFromDB)";
                            LoggingManager.LogApplicationError(null, loErrMsg, loMethodName);
                            Console.WriteLine(loErrMsg + " " + loMethodName);
                        }
                    }
                }
            }
        }


        //Load the issue ticket details from DB
        public void LoadFromDB(XMLConfig.IssStruct structType, string tableName)
        {

            var queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append(" SELECT * ");
            queryStringBuilder.Append(" FROM ");
            queryStringBuilder.Append(tableName);
            queryStringBuilder.Append(" WHERE  ");
            queryStringBuilder.Append(Constants.ISSUENO_COLUMN);
            queryStringBuilder.Append(" =  ");
            queryStringBuilder.Append(structType.sequenceId);

            var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());
            foreach (DataRow oneRow in result.Rows)
            {
                LoadStructFromDataRow(structType, oneRow);
            }
        }


        //Load the issue ticket details from DB by row ID
        public void LoadStructureRecordFromDBByRowId(XMLConfig.IssStruct structType )
        {
            var queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append(" SELECT * ");
            queryStringBuilder.Append(" FROM ");
            queryStringBuilder.Append(structType._TIssStruct.MainTable.Name);
            queryStringBuilder.Append(" WHERE  ");
            queryStringBuilder.Append(Constants.ID_COLUMN);
            queryStringBuilder.Append(" =  ");
            queryStringBuilder.Append(structType._rowID);

            var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());
            foreach (DataRow oneRow in result.Rows)
            {
                LoadStructFromDataRow(structType, oneRow);
            }
        }


#if _original_

        //Load the issue ticket details from DB
        public void LoadFromDB(XMLConfig.IssStruct structType, string tableName)
        {

            var queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append(" SELECT * ");
            queryStringBuilder.Append(" FROM ");
            queryStringBuilder.Append(tableName);
            queryStringBuilder.Append(" WHERE  ");
            queryStringBuilder.Append(Constants.ISSUENO_COLUMN);
            queryStringBuilder.Append(" =  ");
            queryStringBuilder.Append(structType.sequenceId);

            var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());
            foreach (DataRow oneRow in result.Rows)
            {
                for (int loPanelIdx = 0; loPanelIdx < structType.Panels.Count; loPanelIdx++)
                {
                    foreach (XMLConfig.PanelField panelField in structType.Panels[loPanelIdx].PanelFields)
                    {
                        // AJW - defend against misaligned configurations - fields on form are not always defined in the table
                        if (oneRow.Table.Columns.IndexOf(panelField.Name) != -1)
                        {
                            // AJW - review - we only read from DB if Value is defined? When would it NOT be instantiated?
                            if ((panelField.Value != null) && (oneRow[panelField.Name] != null))
                            {
                                // if this is non-null after evaluation, it will be used in place of db raw string value
                                string loConvertedFromDBFormatOverrideString = null;

                                // safe extraction from datarow
                                string loOneValueAsString = GetSafeColumnStringValueFromDataRow(oneRow, panelField.Name);

                                // format the date/time types
                                switch (panelField.FieldType)
                                {
                                    case "efTime":          // this is really lame type evaluation :-(
                                        {
                                            if ((loOneValueAsString != null) && (panelField.EditMask != null))
                                            {
                                                if ((loOneValueAsString.Length > 0) && (panelField.EditMask.Length > 0))
                                                {
                                                    // convert from fixed DB format to panel format
                                                    loConvertedFromDBFormatOverrideString = ConvertDBTimeColumnValueToString(loOneValueAsString, panelField.EditMask);
                                                }
                                            }
                                            break;
                                        }

                                    case "efDate":
                                        {
                                            if ((loOneValueAsString != null) && (panelField.EditMask != null))
                                            {
                                                if ((loOneValueAsString.Length > 0) && (panelField.EditMask.Length > 0))
                                                {
                                                    // convert from fixed DB format to panel format
                                                    loConvertedFromDBFormatOverrideString = ConvertDBDateColumnValueToString(loOneValueAsString, panelField.EditMask);
                                                }
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            break;
                                        }
                                }

                                // use the override if available
                                if (loConvertedFromDBFormatOverrideString != null)
                                {
                                    panelField.Value = loConvertedFromDBFormatOverrideString;
                                }
                                else
                                {
                                    panelField.Value = loOneValueAsString;
                                }



                            }

                        }
                        else
                        {
                            string loErrMsg = "WARNING: XML CONFIG panel field " + panelField.Name + " not defined in table " + oneRow.Table.TableName;
                            string loMethodName = "(Method Name: LoadFromDB)";
                            LoggingManager.LogApplicationError(null, loErrMsg, loMethodName);
                            Console.WriteLine(loErrMsg + " " + loMethodName);
                        }
                    }
                }
            }
        }

#endif



		/// <summary>
		/// Update row in table using the identify column
		/// </summary>
		/// <param name="iRowId"></param>
		/// <param name="tableName"></param>
		/// <param name="status"></param>
		/// <param name="wsStatus"></param>
		/// <returns></returns>
		public static void UpdateRowStatus(string iRowId, string tableName, string status, string wsStatus )
		{
			var updateStrBldr = new StringBuilder();
			updateStrBldr.Append ("UPDATE ");
			updateStrBldr.Append (tableName);
			updateStrBldr.Append (" SET ");
            updateStrBldr.Append(Constants.WS_STATUS_COLUMN);
            updateStrBldr.Append(" = '");
            updateStrBldr.Append(wsStatus);
            updateStrBldr.Append("' ,");
			updateStrBldr.Append (Constants.STATUS_COLUMN);
			updateStrBldr.Append (" = '");
			updateStrBldr.Append (status);
			updateStrBldr.Append ("' WHERE ");
            updateStrBldr.Append(Constants.ID_COLUMN);
		    updateStrBldr.Append (" = '");
			updateStrBldr.Append (iRowId);
			updateStrBldr.Append ("'");

			ExecuteQuery (updateStrBldr.ToString());
		}

		//Execute Query
		public static bool ExecuteQuery(string query)
		{
			bool isSucc = false;
            try
            {
                Log.Debug("Query****", query);
                isSucc = (new DatabaseManager()).ExecuteQuery(query);
                Log.Debug("Result**** ", isSucc.ToString());
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "Query: " + query, "ExecuteQuery");
            }
		    return isSucc;
		}






        private static bool IsRowOldEnoughToDelete(DataRow oneDataRow, string structType, long iRefTime, long iOldTime_msec)
        {
            bool loResult = false;


            // TODO - easy, once reccreationdate is established
            try
            {


                // every row has an identity
                string rowId = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.ID_COLUMN);


                // first choice for date
                string loRecCreationDateStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.RECCREATIONDATE_COLUMN);
                // second choices
                if (string.IsNullOrEmpty(loRecCreationDateStr) == true)
                {
                    loRecCreationDateStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.ISSUEDATE_COLUMN);
                }
                if (string.IsNullOrEmpty(loRecCreationDateStr) == true)
                {
                    loRecCreationDateStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.NOTEDATE_COLUMN);
                }
                if (string.IsNullOrEmpty(loRecCreationDateStr) == true)
                {
                    loRecCreationDateStr = GetSafeColumnStringValueFromDataRow(oneDataRow, "STARTDATE");
                }



                // first choice for time
                string loRecCreationTimeStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.RECCREATIONTIME_COLUMN);
                // second choices
                if (string.IsNullOrEmpty(loRecCreationTimeStr) == true)
                {
                    loRecCreationTimeStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.ISSUETIME_COLUMN);
                }
                if (string.IsNullOrEmpty(loRecCreationTimeStr) == true)
                {
                    loRecCreationTimeStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.NOTETIME_COLUMN);
                }
                if (string.IsNullOrEmpty(loRecCreationTimeStr) == true)
                {
                    loRecCreationTimeStr = GetSafeColumnStringValueFromDataRow(oneDataRow, "STARTTIME");
                }


                // we have to have a date to proceed
                if (string.IsNullOrEmpty(loRecCreationDateStr) == true)
                {
                    // not enough info.
                    return false;
                }

                // start with just the date
                DateTime loRowDTStamp = DateTime.ParseExact(loRecCreationDateStr, Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK, CultureInfo.InvariantCulture);

                DateTime loRowDTTimePart;
                if (string.IsNullOrEmpty(loRecCreationTimeStr) == true)
                {
                    // no info, set to midnight
                    loRowDTTimePart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                }
                else
                {
                    try
                    {
                        loRowDTTimePart = DateTime.ParseExact(loRecCreationTimeStr, Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK, CultureInfo.InvariantCulture);
                        //DateTime loSourceDTLocal = DateTime.SpecifyKind(loSourceDT, System.DateTimeKind.Local);
                        //loFormattedString = loSourceDTLocal.ToString(iDateStringDestinationFormat);

                        // redo to combine date + time
                        loRowDTStamp = new DateTime(loRowDTStamp.Year, loRowDTStamp.Month, loRowDTStamp.Day,
                                                      loRowDTTimePart.Hour, loRowDTTimePart.Minute, loRowDTTimePart.Second,
                                                      DateTimeKind.Utc);
                    }
                    catch (Exception exp)
                    {
                        // no time part, leave it
                    }
                }


                var loRowTimeAsMillisecondsSince1970 = loRowDTStamp.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

                ////Get the file date/time                      
                //long loFileTime = loFile.LastModified();
                //long loFileOldTime = fRefTime - loFileTime;
                //if (loFileOldTime >= fOldTime_msec) return true;

                long loFileTime = (long)loRowTimeAsMillisecondsSince1970;
                long loFileOldTime = iRefTime - loFileTime;
                if (loFileOldTime >= iOldTime_msec)
                {
                    loResult = true;
                }

                return loResult;
            }
            catch (Exception e)
            {
                // can't do it, 
                LoggingManager.LogApplicationError(e, "TableName: " + structType, "DeleteAgedTableRows.IsRowOldEnoughToDelete");
                return false;
            }
        }

        public static void DeleteAgedTableRows(string tableName, string structType,  long iRefTime, long iOldTime_msec  )
        {

            var rows = new List<CommonDTO>();
            try
            {
                var selectStringBuilder = new StringBuilder();

                XMLConfig.TableDef loCleaningTableDef = null;

                List<XMLConfig.TableDef> tableDefList = DroidContext.XmlCfg.TableDefs;
                if (tableDefList != null)
                {
                    foreach (XMLConfig.TableDef tableDef in tableDefList)
                    {
                        if (tableName == tableDef.Name)
                        {
                            // we have our target table
                            loCleaningTableDef = tableDef;
                            break;
                        }
                    }
                }


                // excluded tables
                switch (tableName)
                {
                    case "USERSTRUCT":
                        {
                            // don't want to age your data, move along
                            return;
                        }
                    default:
                        {
                            // carry on 
                            break;
                        }
                }


                // query looking for candidate
                var queryStringBuilder = new StringBuilder();
                queryStringBuilder.Append(" SELECT ");
                queryStringBuilder.Append(" * ");
                queryStringBuilder.Append(" FROM ");
                queryStringBuilder.Append(tableName);

                string whereClause = " WHERE " + Constants.WS_STATUS_COLUMN + " = '" + Constants.WS_STATUS_SUCCESS + "'";
                queryStringBuilder.Append(whereClause);


                // delete statement
                var deleteStringBuilder = new StringBuilder();
                deleteStringBuilder.Append(" DELETE FROM ");
                deleteStringBuilder.Append(tableName);
                deleteStringBuilder.Append(" WHERE " + Constants.ID_COLUMN + " IN ( ");


                //get all the rows that meet the -- TOOD only select the needed columns
                var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());
                int loRowsToDeleteCount = 0;
                foreach (DataRow oneDataRow in result.Rows)
                {
                    // old enough to go?
                    if (IsRowOldEnoughToDelete(oneDataRow, structType, iRefTime, iOldTime_msec) == true)
                    {
                        // every row has an identity - this the delete key
                        string rowId = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.ID_COLUMN);
                        if ( string.IsNullOrEmpty( rowId ) == false )
                        {
                            if (loRowsToDeleteCount > 0)
                            {
                                deleteStringBuilder.Append(", ");
                            }
                            deleteStringBuilder.Append(rowId);
                            loRowsToDeleteCount++;
                        }
                    }
                }


                // get any?
                if (loRowsToDeleteCount > 0)
                {
                    // execute
                    deleteStringBuilder.Append(" );");

                    ExecuteQuery(deleteStringBuilder.ToString());

                }


            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, "TableName: " + tableName,  "DeleteAgedTableRows");
            }
        }

    


        //
        // AJW TODO this is really ineffecient how it regenerate the query select every single time
        //
        /* AJW TODO - egads! */

		//Generate DAT file date and return List CommonDTO

        public static List<CommonDTO> GenerateDATFile(string tableName, string whereClause, string structType)
        {
            var rows = new List<CommonDTO>();
            try
            {
                var selectStringBuilder = new StringBuilder();

                XMLConfig.TableDef loUploadTableDef = null;

                List<XMLConfig.TableDef> tableDefList = DroidContext.XmlCfg.TableDefs;
                if (tableDefList != null)
                {
                    foreach (XMLConfig.TableDef tableDef in tableDefList)
                    {
                        if (tableName == tableDef.Name)
                        {
                            // we have our target table, lets send it
                            loUploadTableDef = tableDef;
                            break;
                        }
                    }
                }

                var queryStringBuilder = new StringBuilder();
                queryStringBuilder.Append(" SELECT ");
                queryStringBuilder.Append(" * " );
                queryStringBuilder.Append(" FROM ");
                queryStringBuilder.Append(tableName);

                if (whereClause != null)
                {
                    queryStringBuilder.Append(whereClause);
                }

                var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());
                foreach (DataRow oneDataRow in result.Rows)
                {
                    //Each row
                    var commonDTO = new CommonDTO();
                    var stringBuilder = new StringBuilder();

                    // every row has an identity
                    commonDTO.rowId = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.ID_COLUMN);



                    if (
                        Constants.STRUCT_TYPE_CITE.Equals(structType) ||
                        Constants.STRUCT_TYPE_GENERIC_ISSUE.Equals(structType)
                        )
                    {
                        // DEFENSE - not all issue structs have issue numbers
                        commonDTO.seqId = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.ISSUENO_COLUMN);
                        commonDTO.sqlIssueDateStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.ISSUEDATE_COLUMN);
                    }


                    if (Constants.STRUCT_TYPE_VOID.Equals(structType))
                    {
                        commonDTO.sqlIssueDateStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.RECCREATIONDATE_COLUMN);
                        commonDTO.sqlIssueTimeStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.RECCREATIONTIME_COLUMN);
                        commonDTO.seqId = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.SEQUENCE_ID);
                    }


                    if (Constants.STRUCT_TYPE_REISSUE.Equals(structType))
                    {
                        commonDTO.seqId = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.SRCISSUENO_COLUMN);
                    }


                    if (Constants.STRUCT_TYPE_NOTES.Equals(structType))
                    {
                        commonDTO.sqlIssueDateStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.NOTEDATE_COLUMN);
                        commonDTO.sqlIssueTimeStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.NOTETIME_COLUMN);
                        commonDTO.seqId = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.SEQUENCE_ID);

                        //todo - not working, need to compress the file to 64k
                        ////we need to truncate the imate to 65536 bytes of data, as that is the max the web service allows us to send.
                        ////"Import Record failed:\nLength of LOB data (1107901) to be replicated exceeds configured maximum 65536.\nThe statement has been terminated."
                        //var imageAsBytes =  Helper.FileToByteArray(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures) + "/" + Constants.MULTIMEDIA_FOLDERNAME + "/" + reader[Constants.MULTIMEDIANOTEFILENAME_COLUMN].ToString());
                        //var attachmentFileName =  Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures) + "/" + Constants.MULTIMEDIA_FOLDERNAME + "/" + reader[Constants.MULTIMEDIANOTEFILENAME_COLUMN];
                        //BitmapHelpers.LoadAndResizeBitmap(attachmentFileName,)
                        //Bitmap b = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
                        //b.Compress(Bitmap.CompressFormat.Jpeg,1,b.)
                        //var scaledPhoto = Bitmap.(b, 120, 120, false);


                        //first, lets check ot make sure the note has an attachment
                        if (!string.IsNullOrEmpty(oneDataRow[Constants.MULTIMEDIANOTEFILENAME_COLUMN].ToString()))
                        {
                            string fileName = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures) +
                                              "/" + Constants.MULTIMEDIA_FOLDERNAME + "/" +
                                              oneDataRow[Constants.MULTIMEDIANOTEFILENAME_COLUMN].ToString();

                            if (fileName.Contains(Constants.PHOTO_FILE_SUFFIX))
                            {
                                commonDTO.attachment = Helper.ImageToByteArray(fileName, 240, 240);
                            }
                            else
                            {
                                commonDTO.attachment = Helper.FileToByteArray(fileName);
                            }
                        }
                    }



                    ////////////////

                    // build a tab delimited record according to the table def

                    int loColCounter = 0;
                    foreach (XMLConfig.TableDefField tableDefField in loUploadTableDef.TableDefFields)
                    {
                        // AJW- don't upload virtual link table fields
                        string loFieldTypeNameUpper = tableDefField.LayoutFileFieldTypeName;


                        // skip client side added fields
                        if (loUploadTableDef.fIssueStructTableOwner.fAndroidClientOnlyStandardFields.Any(x => x.FieldName == tableDefField.Name))
                        {
                            // AJW - with one kludgy exception - masterkey is being added as needed but should be uploaded if defined
                            if (tableDefField.Name.Equals(AutoISSUE.DBConstants.sqlMasterKeyStr) == true)
                            {
                                // keep going with this field
                            }
                            else
                            {
                                // skip this field and loop back
                                continue;
                            }
                        }


                        // skip virtual fields
                        if (loFieldTypeNameUpper.Contains("VIRTUAL"))
                        {
                            continue;
                        }

                        // AJW TODO - we certainly don't want Virtual Link Fields, these are host side defnitions
                        if (loFieldTypeNameUpper.Contains("VTABLELINK"))
                        {
                            continue;
                        }

                        if (loFieldTypeNameUpper.Contains("VLINKED"))
                        {
                            continue;
                        }


                        String oneColumnName = tableDefField.Name;
                        String oneColumnTypeString = tableDefField.EditDataType;
                        string oneColumnValue = null;


                        try
                        {
                            if (oneColumnTypeString.Contains("tftTime"))
                            {
                                // extract from fixed DB format and convert to fixed DAT format
                                oneColumnValue = DateTimeHelper.ConvertDBTimeColumnValueToString(GetSafeColumnStringValueFromDataRow(oneDataRow, oneColumnName), Constants.TIME_HHMMSS);
                            }
                            else if (oneColumnTypeString.Contains("tftDate"))
                            {
                                // extract from fixed DB format and convert to fixed DAT format
                                oneColumnValue = DateTimeHelper.ConvertDBDateColumnValueToString(GetSafeColumnStringValueFromDataRow(oneDataRow, oneColumnName), Constants.DT_YYYYMMDD);
                            }
                            else
                            {
                                oneColumnValue = GetSafeColumnStringValueFromDataRow(oneDataRow, oneColumnName);
                            }
                        }
                        catch (Exception e)
                        {
                            LoggingManager.LogApplicationError(e, "TableName: " + tableName + " ColumnName: " + oneColumnName, "GenerateDATFile");
                            oneColumnValue = string.Empty;
                        }


                        // AJW - TODO - follow up on this kludginess
                        if (Constants.STRUCT_TYPE_NOTES.Equals(structType)
                            && oneColumnName.Equals(Constants.DETAILRECNO_COLUMN))
                        {
                            oneColumnValue = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.ID_COLUMN);
                        }


                        if (loColCounter > 0)
                        {
                            stringBuilder.Append("\t");
                        }
                        loColCounter++;

                        stringBuilder.Append(oneColumnValue);
                    }


                    string dataStr = stringBuilder.ToString();
                    commonDTO.datFile = Encoding.ASCII.GetBytes(dataStr);
                    rows.Add(commonDTO);
                }
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, "TableName: " + tableName + " WhereClause: " + whereClause,
                                                    "GenerateDatFile");
            }
            return rows;
        }


#if _old_way_saved_for_reference_

		public static List<CommonDTO> GenerateDATFile(string tableName, string whereClause, string structType)
		{
			var rows = new List<CommonDTO>();
            try
            {
                var selectStringBuilder = new StringBuilder();
                selectStringBuilder.Append(Constants.ID_COLUMN);
                //for park notes, we need to add field sequence id
                if (Constants.PARKNOTE_TABLE == tableName
                    || Constants.PARKVOID_TABLE == tableName
                    || Constants.PARKREISSUE_TABLE == tableName)
                {
                    if (selectStringBuilder.Length > 0)
                        selectStringBuilder.Append(", ");
                    selectStringBuilder.Append(Constants.SEQUENCE_ID);
                }

                var columns = new List<String>();
                var columnsType = new List<String>();

                List<XMLConfig.TableDef> tableDefList = DroidContext.XmlCfg.TableDefs;
                if (tableDefList != null)
                {
                    foreach (XMLConfig.TableDef tableDef in tableDefList)
                    {
                        if (tableName == tableDef.Name)
                        {
                            foreach (XMLConfig.TableDefField tableDefField in tableDef.TableDefFields)
                            {
                                //if (tableDefField.Name.Contains("FORMREV") ||
                                //    tableDefField.Name.Contains("UNITSERIAL"))
                                //{
                                //    continue;
                                //}


                                // AJW- don't upload virtual link table fields
                                string loFieldTypeNameUpper = tableDefField.LayoutFileFieldTypeName;



                                // skip client side added fields
                                if (tableDef.fIssueStructTableOwner.fAndroidClientOnlyStandardFields.Any(x => x.FieldName == loFieldTypeNameUpper))
                                {
                                    continue;
                                }




                                // skip virtual fields
                                if (loFieldTypeNameUpper.Contains("VIRTUAL"))
                                {
                                    continue;
                                }

                                // AJW TODO - we certainly don't want Virtual Link Fields, these are host side defnitions
                                if (loFieldTypeNameUpper.Contains("VTABLELINK"))
                                {
                                    continue;
                                }

                                if (loFieldTypeNameUpper.Contains("VLINKED"))
                                {
                                    continue;
                                }


                                columnsType.Add(tableDefField.EditDataType);
                                columns.Add(tableDefField.Name);
                                if (selectStringBuilder.Length > 0)
                                    selectStringBuilder.Append(", ");
                                selectStringBuilder.Append(tableDefField.Name);
                            }
                            break;
                        }
                    }
                }

                var queryStringBuilder = new StringBuilder();
                queryStringBuilder.Append(" SELECT ");
                queryStringBuilder.Append(selectStringBuilder.ToString());
                queryStringBuilder.Append(" FROM ");
                queryStringBuilder.Append(tableName);

                if (whereClause != null)
                {
                    queryStringBuilder.Append(whereClause);
                }

                var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());
                foreach (DataRow oneDataRow in result.Rows)
                {
                    //Each row
                    var commonDTO = new CommonDTO();
                    var stringBuilder = new StringBuilder();

                    // every row has an identity
                    commonDTO.rowId = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.ID_COLUMN);



                    if (
                        Constants.STRUCT_TYPE_CITE.Equals(structType) ||
                        Constants.STRUCT_TYPE_GENERIC_ISSUE.Equals(structType)
                        )
                    {
                        // DEFENSE - not all issue structs have issue numbers
                        commonDTO.seqId = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.ISSUENO_COLUMN);
                        commonDTO.sqlIssueDateStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.ISSUEDATE_COLUMN);
                    }


                    if (Constants.STRUCT_TYPE_VOID.Equals(structType))
                    {
                        commonDTO.sqlIssueDateStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.RECCREATIONDATE_COLUMN);
                        commonDTO.sqlIssueTimeStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.RECCREATIONTIME_COLUMN);
                        commonDTO.seqId = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.SEQUENCE_ID);
                    }


                    if (Constants.STRUCT_TYPE_REISSUE.Equals(structType))
                    {
                        commonDTO.seqId = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.SRCISSUENO_COLUMN);
                    }


                    if (Constants.STRUCT_TYPE_NOTES.Equals(structType))
                    {
                        commonDTO.sqlIssueDateStr = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.NOTEDATE_COLUMN);
                        commonDTO.sqlIssueTimeStr = GetSafeColumnStringValueFromDataRow( oneDataRow,Constants.NOTETIME_COLUMN);
                        commonDTO.seqId = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.SEQUENCE_ID);

                        //todo - not working, need to compress the file to 64k
                        ////we need to truncate the imate to 65536 bytes of data, as that is the max the web service allows us to send.
                        ////"Import Record failed:\nLength of LOB data (1107901) to be replicated exceeds configured maximum 65536.\nThe statement has been terminated."
                        //var imageAsBytes =  Helper.FileToByteArray(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures) + "/" + Constants.MULTIMEDIA_FOLDERNAME + "/" + reader[Constants.MULTIMEDIANOTEFILENAME_COLUMN].ToString());
                        //var attachmentFileName =  Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures) + "/" + Constants.MULTIMEDIA_FOLDERNAME + "/" + reader[Constants.MULTIMEDIANOTEFILENAME_COLUMN];
                        //BitmapHelpers.LoadAndResizeBitmap(attachmentFileName,)
                        //Bitmap b = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
                        //b.Compress(Bitmap.CompressFormat.Jpeg,1,b.)
                        //var scaledPhoto = Bitmap.(b, 120, 120, false);


                        //first, lets check ot make sure the note has an attachment
                        if (!string.IsNullOrEmpty(oneDataRow[Constants.MULTIMEDIANOTEFILENAME_COLUMN].ToString()))
                        {
                            string fileName =
                           Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures) +
                           "/" + Constants.MULTIMEDIA_FOLDERNAME + "/" +
                           oneDataRow[Constants.MULTIMEDIANOTEFILENAME_COLUMN].ToString();
                            if (fileName.Contains(Constants.PHOTO_FILE_SUFFIX))
                            {
                                commonDTO.attachment = Helper.ImageToByteArray(fileName, 240, 240);
                            }
                            else
                            {
                                commonDTO.attachment = Helper.FileToByteArray(fileName);
                            }
                        }
                    }

                    for (int j = 0; j < columns.Count; j++)
                    {
                        if (j > 0)
                        {
                            stringBuilder.Append("\t");
                        }
                        String column = columns[j];
                        String columnType = columnsType[j];
                        string colValue = null;


/* AJW TODO - egads! 
 * 
                        //todo - ignore seq for park notes
                        if (
                              Constants.STRUCT_TYPE_CITE.Equals(structType) ||
                              Constants.STRUCT_TYPE_CHALKING.Equals(structType) ||
                              Constants.STRUCT_TYPE_GENERIC_ISSUE.Equals(structType)
                            )
                        {
                            try
                            {
                                if (columnType.Contains("tftTime"))
                                {
                                    foreach (XMLConfig.EditMaskMapEntry entry in XMLConfig.ConfigData.EditMaskingData)
                                    {
                                        if (entry.name.Equals(column))
                                        {
                                            colValue = oneDataRow[column].ToString();
                                            if (colValue.Length > 1)
                                            {
                                                //determine if it is 24 hours. when we dsave the data, we might save 04:25 PM, but this date gets converted to 04:25
                                                entry.editmask = NumericManager.FixTimeEditMask(entry.editmask);
                                                colValue = DateTimeManager.GetTime(oneDataRow[column].ToString(),
                                                        entry.editmask, Constants.TIME_HHMMSS);
                                            }
                                            break;
                                        }
                                    }


                                }
                                else if (columnType.Contains("tftDate"))
                                {
                                    foreach (XMLConfig.EditMaskMapEntry entry in XMLConfig.ConfigData.EditMaskingData)
                                    {
                                        if (entry.name.Equals(column))
                                        {
                                            colValue = oneDataRow[column].ToString();
                                            if (colValue.Length > 1)
                                            {
                                                entry.editmask = NumericManager.FixDateEditMask(entry.editmask);
                                                colValue = DateTimeManager.GetDate(oneDataRow[column].ToString(),
                                                        entry.editmask, Constants.DT_YYYYMMDD);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error("Error:::", e.ToString());
                            }
                        }
*/

                        try
                        {
                            if (columnType.Contains("tftTime"))
                            {
                                // extract from fixed DB format and convert to fixed DAT format
                                colValue = DateTimeHelper.ConvertDBTimeColumnValueToString(GetSafeColumnStringValueFromDataRow(oneDataRow, column), Constants.TIME_HHMMSS);
                            }
                            else if (columnType.Contains("tftDate"))
                            {
                                // extract from fixed DB format and convert to fixed DAT format
                                colValue = DateTimeHelper.ConvertDBDateColumnValueToString(GetSafeColumnStringValueFromDataRow(oneDataRow, column), Constants.DT_YYYYMMDD);
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error("Error:::", e.ToString());
                        }



                        if (Constants.STRUCT_TYPE_NOTES.Equals(structType)
                            && column.Equals(Constants.DETAILRECNO_COLUMN))
                        {
                            colValue = GetSafeColumnStringValueFromDataRow(oneDataRow, Constants.ID_COLUMN);
                        }

                        //if ((oneDataRow[column] != null) && (oneDataRow[column].Equals(Constants.SPINNER_DEFAULT)))
                        //{
                        //    colValue = "";
                        //}


                        stringBuilder.Append(colValue ?? Helper.GetPrefix( GetSafeColumnStringValueFromDataRow( oneDataRow, column )));
                    }

                    string dataStr = stringBuilder.ToString();
                    commonDTO.datFile = Encoding.ASCII.GetBytes(dataStr);
                    rows.Add(commonDTO);

                }
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, "TableName: " + tableName + " WhereClause: " + whereClause,
                                                    "GenerateDatFile");
            }
			return rows;						
		}


#endif

#if _enable_this_
        /// <summary>
        /// Update the target row in DB with the specific values in the passed list
        /// </summary>
        /// <param name="oneStruct"></param>
        /// <param name="status"></param>
        /// <param name="wsStatus"></param>
        /// <param name="ctx"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRowWithColumnValues_NotReadyForDateTimeStandardFormat(XMLConfig.IssStruct oneStruct, string status, string wsStatus, Context ctx, string tableName, List<string> iColumnNameValuesPairList)
        {
            try
            {
                var colValStr = new StringBuilder();
                List<XMLConfig.TableDef> tableDefList = DroidContext.XmlCfg.TableDefs;
                TableDef tableDef = tableDefList.Find(x => x != null && x.Name == oneStruct.Name);


                foreach (string oneColumnValuePair in iColumnNameValuesPairList)
                {
                    string[] loColumnValuePairSplit = oneColumnValuePair.Split('=');
                    if (loColumnValuePairSplit.Length != 2)
                    {
                        Console.WriteLine(tableName + " row update: Invalid Column-Value Pair *" + oneColumnValuePair + "*");
                        continue;
                    }

                    string loColumnName = loColumnValuePairSplit[0];
                    string loColumnValue = loColumnValuePairSplit[1];

                    if (colValStr.Length > 0)
                        colValStr.Append(", ");

                    colValStr.Append(loColumnName);
                    colValStr.Append(" = '");


/*

                    switch (panelField.FieldType)
                    {
                        case "efTime":
                            {
                                if ((panelField.Value != null) && (panelField.EditMask != null))
                                {
                                    if ((panelField.Value.Length > 0) && (panelField.EditMask.Length > 0))
                                    {

                                        // define the value in fixed format for DB
                                        loDBFormattedOverrideString = ConvertTimeColumnValueToFixedDBString(panelField.Value, panelField.EditMask);
                                    }
                                }
                                break;
                            }

                        case "efDate":
                            {
                                if ((panelField.Value != null) && (panelField.EditMask != null))
                                {
                                    if ((panelField.Value.Length > 0) && (panelField.EditMask.Length > 0))
                                    {
                                        // define the value in fixed format for DB
                                        loDBFormattedOverrideString = ConvertDateColumnValueToFixedDBString(panelField.Value, panelField.EditMask);
                                    }
                                }
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
*/


                    // AJW - for review - this is updating the datetime to NOW, this should only happen at specific points,
                    //    should not be insider row update!!


                    //Cases when text has apostrophes 
                    if (loColumnName.Equals(Constants.ISSUEDATE_COLUMN))
                    {
                        //string loTmpBuf = "";
                        //DateTimeManager.OsDateToDateString(DateTime.Today, onePanelField.EditMask, ref loTmpBuf);
                        //onePanelField.Value = loTmpBuf;
                    }


                    if (loColumnName.Equals(Constants.ISSUETIME_COLUMN))
                    {
                        //string loTmpBuf = "";
                        //DateTimeManager.OsTimeToTimeString(DateTime.Now, onePanelField.EditMask, ref loTmpBuf);
                        //onePanelField.Value = loTmpBuf;
                    }

                    colValStr.Append(loColumnValue.Replace('\'', ' '));
                    colValStr.Append("'");

                    //if (Constants.WS_STATUS_READY.Equals(wsStatus))
                    //    onePanelField.Value = null;
                }



                var updateStrBldr = new StringBuilder();
                //WHERE ENDDATE IS NULL ORDER BY _id desc limit 1
                updateStrBldr.Append("UPDATE ");
                updateStrBldr.Append(tableName);
                updateStrBldr.Append(" SET ");
                updateStrBldr.Append(colValStr.ToString());

                //updateStrBldr.Append(", ");
                //updateStrBldr.Append(Constants.STATUS_COLUMN);
                //updateStrBldr.Append(" = '");
                //updateStrBldr.Append(status);
                //updateStrBldr.Append("' , ");
                //updateStrBldr.Append(Constants.WS_STATUS_COLUMN);
                //                updateStrBldr.Append(" = '");
                //updateStrBldr.Append(wsStatus);
                //updateStrBldr.Append("' ");



                updateStrBldr.Append(" WHERE ");

                if (Constants.STRUCT_TYPE_CITE.Equals(oneStruct.Type))
                {
                    updateStrBldr.Append(Constants.ISSUENO_COLUMN);
                    updateStrBldr.Append(" = '");
                    updateStrBldr.Append(oneStruct.sequenceId);
                    updateStrBldr.Append("'");

                }
                else if (Constants.MARKMODE_TABLE.Equals(tableName))
                {
                    updateStrBldr.Append(Constants.ID_COLUMN);
                    updateStrBldr.Append(" = '");
                    updateStrBldr.Append(oneStruct.chalkRowId);
                    updateStrBldr.Append("'");
                }

                ExecuteQuery(updateStrBldr.ToString());

                Log.Debug("ParkingSequenceADO::", "updatestruct");
                if (Constants.WS_STATUS_READY.Equals(wsStatus))
                {
                    oneStruct.sequenceId = null;
                    oneStruct.chalkRowId = null;
                    oneStruct.prefix = null;
                    ctx.StartService(new Intent(ctx, typeof(SyncService)));
                }

                return true;
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, "tableName: " + tableName, "UpdateRowWithColumnValues");
                Console.WriteLine("Exception source: {0}", e.Source);
            }
            return false;
        }
#endif


        //Update Row in DB 
        public async Task<bool> UpdateRow(XMLConfig.IssStruct structType, string status, string wsStatus, Context ctx, string tableName)
        {
            try
            {
                var colValStr = new StringBuilder();
                List<XMLConfig.TableDef> tableDefList = DroidContext.XmlCfg.TableDefs;
                TableDef tableDef = tableDefList.Find(x => x != null && x.Name == structType.Name);


                // decide if is this a final commit
                bool loUpdateGlobalDatastore = (status.Equals(Constants.STATUS_READY) == true);

                // open the global datastore for updates
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(DroidContext.ApplicationContext);
                ISharedPreferencesEditor editor = prefs.Edit();

                for (int loPanelIdx = 0; loPanelIdx < structType.Panels.Count; loPanelIdx++)
                {
                    foreach (XMLConfig.PanelField panelField in structType.Panels[loPanelIdx].PanelFields)
                    {
                        /*    if (panelField.IsHidden)
                                continue; */

                        if (panelField.Value != null
                            && tableDef.TableDefFields.Any(x => x.Name == panelField.Name))
                        {
                            if (colValStr.Length > 0)
                            {
                                colValStr.Append(", ");
                            }

                            colValStr.Append(panelField.Name);
                            colValStr.Append(" = '");

                            // AJW - for review - this is updating the datetime to NOW, this should only happen at specific points,
                            //    should not be inside row update!!
                            //
                            // needs form initalization for these



                            // if this is non-null after evaluation, it will be used in place of panelfield.value
                            string loDBFormattedOverrideString = null;


                            if (panelField.Name.Equals(Constants.ISSUEDATE_COLUMN))
                            {
                                string loTmpBuf = "";
                                DateTime dtNow = DateTime.Today;
                                DateTimeManager.OsDateToDateString(dtNow, panelField.EditMask, ref loTmpBuf);
                                panelField.Value = loTmpBuf;
                            }

                            if (panelField.Name.Equals(Constants.ISSUETIME_COLUMN))
                            {
                                string loTmpBuf = "";
                                DateTime dtNow = DateTime.Now;
                                DateTimeManager.OsTimeToTimeString(dtNow, panelField.EditMask, ref loTmpBuf);
                                panelField.Value = loTmpBuf;
                            }



                            // format the date/time types
                            switch (panelField.FieldType)
                            {
                                case "efTime":
                                    {
                                        if ((panelField.Value != null) && (panelField.EditMask != null))
                                        {
                                            if ((panelField.Value.Length > 0) && (panelField.EditMask.Length > 0))
                                            {
                                                // define the value in fixed format for DB
                                                loDBFormattedOverrideString = DateTimeHelper.ConvertTimeColumnValueToFixedDBString(panelField.Value, panelField.EditMask);
                                            }
                                        }
                                        break;
                                    }

                                case "efDate":
                                    {
                                        if ((panelField.Value != null) && (panelField.EditMask != null))
                                        {
                                            if ((panelField.Value.Length > 0) && (panelField.EditMask.Length > 0))
                                            {
                                                // define the value in fixed format for DB
                                                loDBFormattedOverrideString = DateTimeHelper.ConvertDateColumnValueToFixedDBString(panelField.Value, panelField.EditMask);
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }



                            // default to using the panel value
                            string loValueToStoreInDB = panelField.Value;

                            if (string.IsNullOrEmpty(loValueToStoreInDB) == false)
                            {
                                // remove abbrev descriptions when present
                                int loPosSep = loValueToStoreInDB.IndexOf(Constants.LIST_ITEM_DESCRIPTION_SEPARATOR);
                                // has to be beyond the first char
                                if (loPosSep > 0)
                                {
                                    // keep everything up to the space preceeding the seperator char
                                    loValueToStoreInDB = loValueToStoreInDB.Substring(0, loPosSep - 1);
                                }
                            }




                            // allow override to take
                            if (loDBFormattedOverrideString != null)
                            {
                                loValueToStoreInDB = loDBFormattedOverrideString;
                            }


                            colValStr.Append(loValueToStoreInDB.Replace('\'', ' '));
                            colValStr.Append("'");

                            if (Constants.WS_STATUS_READY.Equals(wsStatus))
                            {
                                panelField.Value = null;
                            }


                            if (loUpdateGlobalDatastore == true)
                            {
                                // update the global datastore
                                editor.PutString(Helper.BuildGlobalPreferenceKeyName(structType.Name, panelField.Name), loValueToStoreInDB);
                            }
                        }
                    }
                }


                if (loUpdateGlobalDatastore == true)
                {
                    // commit the global datastore
                    editor.Apply();
                }
                else
                {
                    // rollback?
                }


                var updateStrBldr = new StringBuilder();
                updateStrBldr.Append("UPDATE ");
                updateStrBldr.Append(tableName);
                updateStrBldr.Append(" SET ");
                updateStrBldr.Append(colValStr.ToString());
                updateStrBldr.Append(", ");
                updateStrBldr.Append(Constants.STATUS_COLUMN);
                updateStrBldr.Append(" = '");
                updateStrBldr.Append(status);
                updateStrBldr.Append("' , ");
                updateStrBldr.Append(Constants.WS_STATUS_COLUMN);
                updateStrBldr.Append(" = '");
                updateStrBldr.Append(wsStatus);
                updateStrBldr.Append("' ");
                updateStrBldr.Append(" WHERE ");

                // ALL updates by rowID
                updateStrBldr.Append(Constants.ID_COLUMN);
                updateStrBldr.Append(" = '");
                updateStrBldr.Append(structType._rowID);
                updateStrBldr.Append("'");

                //if (Constants.STRUCT_TYPE_CITE.Equals(structType.Type))
                //{
                //    updateStrBldr.Append(Constants.ISSUENO_COLUMN);
                //    updateStrBldr.Append(" = '");
                //    updateStrBldr.Append(structType.sequenceId);
                //    updateStrBldr.Append("'");

                //}
                //else if (Constants.MARKMODE_TABLE.Equals(tableName))
                //{
                //    updateStrBldr.Append(Constants.ID_COLUMN);
                //    updateStrBldr.Append(" = '");
                //    updateStrBldr.Append(structType.chalkRowId);
                //    updateStrBldr.Append("'");
                //}


                ExecuteQuery(updateStrBldr.ToString());

                Log.Debug("ParkingSequenceADO::", "updatestruct");
                if (Constants.WS_STATUS_READY.Equals(wsStatus))
                {
                    // AJW TODO - why are these being wiped out? so new row will be created on next FormPanel.UpdateStruct
                    // let ResetControlStatusByStructName do this 
                    //structType.sequenceId = null;
                    //structType.chalkRowId = null;
                    //structType.prefix = null;

                    ctx.StartService(new Intent(ctx, typeof(SyncService)));
                }

                return true;
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, "tableName: " + tableName, "UpdateRow");
                Console.WriteLine("Exception source: {0}", e.Source);
            }
            return false;
        }

		//Get tickets by officer name from DB
        public Task<List<CommonDTO>> GetTableRows(List<string> tableNames, string whereString)
	    {
	        return Task.Factory.StartNew(() =>
	            {
	                try
	                {
	                    var commonDTOList = new List<CommonDTO>();
                        foreach (var tableName in tableNames)
                        {
                            var tableDTOList = GetTableRowsByTableName(tableName,
                                                 whereString);
                            commonDTOList.AddRange(tableDTOList);
                        }

                        return commonDTOList;
	                }
	                catch (Exception e)
	                {
	                    //TODO Need to come up with better exception handling, something like saving to SQLite or Toast msg
	                    Console.WriteLine("Exception source: {0}", e.Source);
	                }
	                return null;
	            });
	    }


        /// <summary>
        /// Safe method to retrieve value from datarow
        /// </summary>
        /// <param name="iColumnName"></param>
        /// <returns></returns>
        public static string GetSafeColumnStringValueFromDataRow(DataRow iSourceRow, string iColumnName)
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
                LoggingManager.LogApplicationError(exp, "tableName: " + iSourceRow.Table.TableName, "GetValueFromDataRow");
                Console.WriteLine("Exception source: {0}", exp.Source);
            }

            // suppress spinner values - ultimately these shoud NOT be written to the DB
            //if (loResultStr.Equals(Constants.SPINNER_DEFAULT) == true)
            //{
            //    loResultStr = "";
            //}


            return loResultStr;
        }

        /// <summary>
        /// Append next word to sentence handling spaces and empty words
        /// </summary>
        /// <param name="iSentence"></param>
        /// <param name="iNextWord"></param>
        private void AppendNonNullToSentenceStringBuilder(StringBuilder iSentence, string iNextWord)
        {
            if (iNextWord.Length > 0)
            {
                iSentence.Append(" " + iNextWord);
            }
        }

        //Get tickets by officer name from DB
        public List<CommonDTO> GetTableRowsByTableName(string tableName, string whereString)
        {
            try
            {
                var commonDTOList = new List<CommonDTO>();

                var queryStringBuilder = new StringBuilder();
                queryStringBuilder.Append(" SELECT * FROM ");
                queryStringBuilder.Append(tableName);
                queryStringBuilder.Append(whereString);
                queryStringBuilder.Append(" ORDER BY ");

                queryStringBuilder.Append(Constants.ID_COLUMN);
                //if (Constants.MARKMODE_TABLE.Equals(tableName))
                //    queryStringBuilder.Append(Constants.ID_COLUMN);
                //else
                //    queryStringBuilder.Append(Constants.ISSUENO_COLUMN);


                // AJW - for review - newest data displayed on top?
                //queryStringBuilder.Append(" ASC ");
                queryStringBuilder.Append(" DESC ");

                var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());
                foreach (DataRow row in result.Rows)
                {
                    var commonDTO = new CommonDTO();
                    commonDTO.structName = tableName;

                    //if (Constants.MARKMODE_TABLE.Equals(tableName))
                    //    commonDTO.rowId = row[Constants.ID_COLUMN].ToString();
                    //else
                    //    commonDTO.seqId = row[Constants.ISSUENO_COLUMN].ToString();

                    //commonDTO.ISSUEDATE = row[Constants.ISSUEDATE_COLUMN].ToString();
                    //commonDTO.ISSUETIME = row[Constants.ISSUETIME_COLUMN].ToString();
                    //commonDTO.LOCSTREET = row[Constants.LOCSTREET_COLUMN].ToString();
                    //commonDTO.parkingStatus = row[Constants.STATUS_COLUMN].ToString();

                    //if (row[Constants.VEHLICNO_COLUMN] != null)
                    //    commonDTO.VEHLICNO = row[Constants.VEHLICNO_COLUMN].ToString();


                    commonDTO.parkingStatus = Helper.GetSafeColumnStringValueFromDataRow(row, Constants.STATUS_COLUMN);


                    // AJW - TODOD clean up all these kludge logic references and streamline to single identity column across all data type
                    //if (Constants.MARKMODE_TABLE.Equals(tableName))
                    //{
                    //    commonDTO.rowId = GetSafeColumnStringValueFromDataRow(row, Constants.ID_COLUMN);
                    //}
                    //else
                    //{
                    //    commonDTO.seqId = GetSafeColumnStringValueFromDataRow(row, Constants.ISSUENO_COLUMN);
                    //}

                    commonDTO.rowId = GetSafeColumnStringValueFromDataRow(row, Constants.ID_COLUMN);
                    commonDTO.seqId = GetSafeColumnStringValueFromDataRow(row, Constants.ISSUENO_COLUMN);
                    commonDTO.masterKey = GetSafeColumnStringValueFromDataRow(row, Constants.MASTERKEY_COLUMN);



                    commonDTO.sqlIssueDateStr = GetSafeColumnStringValueFromDataRow(row, Constants.ISSUEDATE_COLUMN);
                    commonDTO.sqlIssueTimeStr = GetSafeColumnStringValueFromDataRow(row, Constants.ISSUETIME_COLUMN);

                    commonDTO.sqlIssueNumberPrefixStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlIssueNumberPrefixStr);
                    commonDTO.sqlIssueNumberStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlIssueNumberStr);
                    commonDTO.sqlIssueNumberSuffixStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlIssueNumberSuffixStr);

                    // AJW - for review - this should really come from the formatted print pciture
                    commonDTO.ISSUENO_DISPLAY = commonDTO.sqlIssueNumberPrefixStr.Trim() + commonDTO.sqlIssueNumberStr.Trim() + commonDTO.sqlIssueNumberSuffixStr.Trim();


                    // location info
                    commonDTO.sqlLocLotStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocLotStr);
                    commonDTO.sqlLocBlockStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocBlockStr);
                    commonDTO.sqlLocDirectionStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocDirectionStr);
                    commonDTO.sqlLocStreetStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocStreetStr);
                    commonDTO.sqlLocDescriptorStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocDescriptorStr);
                    if (commonDTO.sqlLocLotStr.Length > 0)
                    {
                        commonDTO.LOCATION_DISPLAY = commonDTO.sqlLocLotStr;
                    }
                    else
                    {
                        StringBuilder loLocationDisplay = new StringBuilder();
                        AppendNonNullToSentenceStringBuilder(loLocationDisplay, commonDTO.sqlLocBlockStr);
                        AppendNonNullToSentenceStringBuilder(loLocationDisplay, commonDTO.sqlLocDirectionStr);
                        AppendNonNullToSentenceStringBuilder(loLocationDisplay, commonDTO.sqlLocStreetStr);
                        AppendNonNullToSentenceStringBuilder(loLocationDisplay, commonDTO.sqlLocDescriptorStr);

                        commonDTO.LOCATION_DISPLAY = loLocationDisplay.ToString();
                    }


                    // vehicle info
                    commonDTO.sqlVehLicNoStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlVehLicNoStr);
                    commonDTO.sqlVehLicStateStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlVehLicStateStr);
                    commonDTO.sqlVehVINStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlVehVINStr);
                    if (commonDTO.sqlVehLicNoStr.Length > 0)
                    {
                        StringBuilder loVehicleDisplay = new StringBuilder();
                        AppendNonNullToSentenceStringBuilder(loVehicleDisplay, commonDTO.sqlVehLicStateStr);
                        AppendNonNullToSentenceStringBuilder(loVehicleDisplay, commonDTO.sqlVehLicNoStr);

                        commonDTO.VEHICLE_DISPLAY = loVehicleDisplay.ToString();
                    }
                    else
                    {
                        commonDTO.VEHICLE_DISPLAY = "VIN: " + commonDTO.sqlVehVINStr;
                    }
                        

                    // populated - add to the list
                    commonDTOList.Add(commonDTO);
                }


                return commonDTOList;
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, "tableName: " + tableName, "GetTableRows");
                Console.WriteLine("Exception source: {0}", e.Source);
            }
	                return null;
	    }

	    public string GetRowId(string tableName)
	    {
	        String rowId = null;

	        var queryStringBuilder = new StringBuilder();
	        queryStringBuilder.Append("SELECT * FROM ");
	        queryStringBuilder.Append(tableName);
	        queryStringBuilder.Append(" ORDER BY ");
	        queryStringBuilder.Append(Constants.ID_COLUMN);
	        queryStringBuilder.Append(" DESC LIMIT 1");

            var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());

            foreach (DataRow row in result.Rows)
	        {
                rowId = row[Constants.ID_COLUMN].ToString();
	        }
	        return rowId;
	    }

        /// <summary>
        ///  Get all detail rows matching passed masterkey
        /// </summary>
        /// <param name="iMasterKeyID"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable GetRowsWithMasterKey(string iMasterKeyID, string tableName)
        {
            DataTable loResult = null;

            try
            {
                var queryStringBuilder = new StringBuilder();
                queryStringBuilder.Append(" SELECT * FROM ");
                queryStringBuilder.Append(tableName);
                queryStringBuilder.Append(" WHERE ");
                queryStringBuilder.Append(AutoISSUE.DBConstants.sqlMasterKeyStr);

                queryStringBuilder.Append(" = '");
                queryStringBuilder.Append(iMasterKeyID);
                queryStringBuilder.Append("'");

                loResult = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());

                return loResult;
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, "tableName: " + tableName + " id: " + iMasterKeyID, "GetRowsWithMasterKey");
                Console.WriteLine("Exception source: {0}", e.Source);
            }
            return null;

        }


        public DataRow GetRawDataRowByRowId(string iRowId, string tableName)
        //public Task<DataRow> GetRawDataRowByRowId(string iRowId, string tableName)
        {
            try
            {
                var queryStringBuilder = new StringBuilder();
                queryStringBuilder.Append(" SELECT * FROM ");
                queryStringBuilder.Append(tableName);
                queryStringBuilder.Append(" WHERE ");
                queryStringBuilder.Append(Constants.ID_COLUMN);

                queryStringBuilder.Append(" = '");
                queryStringBuilder.Append(iRowId);
                queryStringBuilder.Append("'");

                var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());

                // there should only be one!
                if (result != null)
                {
                    if (result.Rows != null)
                    {
                        if (result.Rows.Count > 0)
                        {
                            return result.Rows[0];
                        }
                    }
                }

                // nothing?
                return null;
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, "tableName: " + tableName + " id: " + iRowId, "GetRawDataRowByRowId");
                Console.WriteLine("Exception source: {0}", e.Source);
            }
            return null;

        }


        
        
        
        //Get all tickets
		public Task<ParkingDTO> GetRowInfoByRowId(string iRowId, string tableName)
		{
			return Task.Factory.StartNew (() => { 
				try
				{
					var parkingDTO = new ParkingDTO();

				    var queryStringBuilder = new StringBuilder();
					queryStringBuilder.Append(" SELECT * FROM ");
					queryStringBuilder.Append(tableName);
					queryStringBuilder.Append(" WHERE ");
                    queryStringBuilder.Append(Constants.ID_COLUMN);

					
                    //if (Constants.MARKMODE_TABLE.Equals(tableName))
                    //    queryStringBuilder.Append(Constants.ID_COLUMN);
                    //else
                    //    queryStringBuilder.Append(Constants.ISSUENO_COLUMN);
				   
                   queryStringBuilder.Append(" = '");
					queryStringBuilder.Append(iRowId);
					queryStringBuilder.Append("'");

                    var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());


                    // AJW - TODO why are there COMMON and PARKING DTO objects that do overalpping things?
                    foreach (DataRow row in result.Rows)			        
				    {
				        parkingDTO.DBRowId = row[Constants.ID_COLUMN].ToString();
                        if (!Constants.MARKMODE_TABLE.Equals(tableName))
				        {

                            //parkingDTO.sqlIssueNumberPrefixStr = GetSafeValue(row, Constants.ISSUENOPFX_COLUMN);
                            //parkingDTO.sqlIssueNumberStr = GetSafeValue(row, Constants.ISSUENO_COLUMN);
                            parkingDTO.sqlIssueNumberPrefixStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlIssueNumberPrefixStr);
                            parkingDTO.sqlIssueNumberStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlIssueNumberStr);
                            parkingDTO.sqlIssueNumberSuffixStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlIssueNumberSuffixStr);

                            // AJW - for review - this should really come from the formatted print pciture
                            parkingDTO.ISSUENO_DISPLAY = parkingDTO.sqlIssueNumberPrefixStr.Trim() + parkingDTO.sqlIssueNumberStr.Trim() + parkingDTO.sqlIssueNumberSuffixStr.Trim();

                      
                           

                            parkingDTO.VioSelect = GetSafeValue(row, Constants.VIOSELECT_COLUMN);
                            parkingDTO.VioCode = GetSafeValue(row, Constants.VIOCODE_COLUMN);
                            parkingDTO.VioDesc = GetSafeValue(row, Constants.VIODESCRIPTION1_COLUMN);
                            parkingDTO.VioFee = GetSafeValue(row, Constants.VIOFINE_COLUMN);
				        }

				        parkingDTO.sqlIssueDateStr = GetSafeValue(row, Constants.ISSUEDATE_COLUMN);
				        parkingDTO.sqlIssueTimeStr = GetSafeValue(row, Constants.ISSUETIME_COLUMN);

				        parkingDTO.Status = GetSafeValue(row, Constants.STATUS_COLUMN);


                        //parkingDTO.sqlVehVINStr = GetSafeValue(row, Constants.VEHVIN_COLUMN);
                        //parkingDTO.sqlVehLicNoStr = GetSafeValue(row, Constants.VEHLICNO_COLUMN);
                        //parkingDTO.sqlVehLicStateStr = GetSafeValue(row, Constants.VEHLICSTATE_COLUMN);


                        //parkingDTO.sqlVehMakeStr = GetSafeValue(row, Constants.VEHMAKE_COLUMN);
                        //parkingDTO.sqlVehPlateTypeStr = GetSafeValue(row, Constants.VEHPLATETYPE_COLUMN);
                        //parkingDTO.sqlVehLicExpDateStr = GetSafeValue(row, Constants.VEHLICEXPDATE_COLUMN);
                        ////parkingDTO.VehTime = GetSafeValue(row, Constants.VEHYEAR_COLUMN);

                        // vehicle info
                        parkingDTO.sqlVehLicNoStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlVehLicNoStr);
                        parkingDTO.sqlVehLicStateStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlVehLicStateStr);
                        parkingDTO.sqlVehYearDateStr = GetSafeValue(row, AutoISSUE.DBConstants.sqlVehYearDateStr);
                        parkingDTO.sqlVehVINStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlVehVINStr);
                        if (parkingDTO.sqlVehLicNoStr.Length > 0)
                        {
                            StringBuilder loVehicleDisplay = new StringBuilder();
                            AppendNonNullToSentenceStringBuilder(loVehicleDisplay, parkingDTO.sqlVehLicStateStr);
                            AppendNonNullToSentenceStringBuilder(loVehicleDisplay, parkingDTO.sqlVehLicNoStr);

                            parkingDTO.VEHICLE_DISPLAY = loVehicleDisplay.ToString();
                        }
                        else
                        {
                            parkingDTO.VEHICLE_DISPLAY = "VIN: " + parkingDTO.sqlVehVINStr;
                        }



				        parkingDTO.sqlIssueOfficerIDStr = GetSafeValue(row, Constants.OFFICER_ID);
				        parkingDTO.sqlIssueOfficerNameStr = GetSafeValue(row, Constants.OFFICER_NAME);


				        //parkingDTO.LocMeter = GetSafeValue(row, Constants.LOCMETER_COLUMN);
				        //parkingDTO.sqlLocBlockStr = GetSafeValue(row, Constants.LOCBLOCK_COLUMN);
				        //parkingDTO.sqlLocDirectionStr = GetSafeValue(row, Constants.LOCDIRECTION_COLUMN);
				        //parkingDTO.sqlLocStreetStr = GetSafeValue(row, Constants.LOCSTREET_COLUMN);

                        // location info
                        parkingDTO.sqlLocLotStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocLotStr);
                        parkingDTO.sqlLocBlockStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocBlockStr);
                        parkingDTO.sqlLocDirectionStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocDirectionStr);
                        parkingDTO.sqlLocStreetStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocStreetStr);
                        parkingDTO.sqlLocDescriptorStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocDescriptorStr);
                        if (parkingDTO.sqlLocLotStr.Length > 0)
                        {
                            parkingDTO.LOCATION_DISPLAY = parkingDTO.sqlLocLotStr;
                        }
                        else
                        {
                            StringBuilder loLocationDisplay = new StringBuilder();
                            AppendNonNullToSentenceStringBuilder(loLocationDisplay, parkingDTO.sqlLocBlockStr);
                            AppendNonNullToSentenceStringBuilder(loLocationDisplay, parkingDTO.sqlLocDirectionStr);
                            AppendNonNullToSentenceStringBuilder(loLocationDisplay, parkingDTO.sqlLocStreetStr);
                            AppendNonNullToSentenceStringBuilder(loLocationDisplay, parkingDTO.sqlLocDescriptorStr);

                            parkingDTO.LOCATION_DISPLAY = loLocationDisplay.ToString();
                        }


				        break;
				    }
				    return parkingDTO;
				}
				catch (Exception e)
				{
                    LoggingManager.LogApplicationError(e, "tableName: " + tableName + " id: " + iRowId, "GetRowInfoById");
					Console.WriteLine("Exception source: {0}", e.Source);
				}
				return null;
			});
		}


        //
        public Task<ParkingDTO> GetRowInfoBySequenceId(string iSequenceId, string tableName)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var parkingDTO = new ParkingDTO();

                    var queryStringBuilder = new StringBuilder();
                    queryStringBuilder.Append(" SELECT * FROM ");
                    queryStringBuilder.Append(tableName);
                    queryStringBuilder.Append(" WHERE ");

                    queryStringBuilder.Append(Constants.ISSUENO_COLUMN);


                    //if (Constants.MARKMODE_TABLE.Equals(tableName))
                    //    queryStringBuilder.Append(Constants.ID_COLUMN);
                    //else
                    //    queryStringBuilder.Append(Constants.ISSUENO_COLUMN);

                    queryStringBuilder.Append(" = '");
                    queryStringBuilder.Append(iSequenceId);
                    queryStringBuilder.Append("'");

                    var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());


                    // AJW - TODO why are there COMMON and PARKING DTO objects that do overalpping things?
                    foreach (DataRow row in result.Rows)
                    {
                        parkingDTO.DBRowId = row[Constants.ID_COLUMN].ToString();
                        if (!Constants.MARKMODE_TABLE.Equals(tableName))
                        {

                            //parkingDTO.sqlIssueNumberPrefixStr = GetSafeValue(row, Constants.ISSUENOPFX_COLUMN);
                            //parkingDTO.sqlIssueNumberStr = GetSafeValue(row, Constants.ISSUENO_COLUMN);
                            parkingDTO.sqlIssueNumberPrefixStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlIssueNumberPrefixStr);
                            parkingDTO.sqlIssueNumberStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlIssueNumberStr);
                            parkingDTO.sqlIssueNumberSuffixStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlIssueNumberSuffixStr);

                            // AJW - for review - this should really come from the formatted print pciture
                            parkingDTO.ISSUENO_DISPLAY = parkingDTO.sqlIssueNumberPrefixStr.Trim() + parkingDTO.sqlIssueNumberStr.Trim() + parkingDTO.sqlIssueNumberSuffixStr.Trim();




                            parkingDTO.VioSelect = GetSafeValue(row, Constants.VIOSELECT_COLUMN);
                            parkingDTO.VioCode = GetSafeValue(row, Constants.VIOCODE_COLUMN);
                            parkingDTO.VioDesc = GetSafeValue(row, Constants.VIODESCRIPTION1_COLUMN);
                            parkingDTO.VioFee = GetSafeValue(row, Constants.VIOFINE_COLUMN);
                        }

                        parkingDTO.sqlIssueDateStr = GetSafeValue(row, Constants.ISSUEDATE_COLUMN);
                        parkingDTO.sqlIssueTimeStr = GetSafeValue(row, Constants.ISSUETIME_COLUMN);

                        parkingDTO.Status = GetSafeValue(row, Constants.STATUS_COLUMN);


                        //parkingDTO.sqlVehVINStr = GetSafeValue(row, Constants.VEHVIN_COLUMN);
                        //parkingDTO.sqlVehLicNoStr = GetSafeValue(row, Constants.VEHLICNO_COLUMN);
                        //parkingDTO.sqlVehLicStateStr = GetSafeValue(row, Constants.VEHLICSTATE_COLUMN);


                        //parkingDTO.sqlVehMakeStr = GetSafeValue(row, Constants.VEHMAKE_COLUMN);
                        //parkingDTO.sqlVehPlateTypeStr = GetSafeValue(row, Constants.VEHPLATETYPE_COLUMN);
                        //parkingDTO.sqlVehLicExpDateStr = GetSafeValue(row, Constants.VEHLICEXPDATE_COLUMN);
                        ////parkingDTO.VehTime = GetSafeValue(row, Constants.VEHYEAR_COLUMN);

                        // vehicle info
                        parkingDTO.sqlVehLicNoStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlVehLicNoStr);
                        parkingDTO.sqlVehLicStateStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlVehLicStateStr);
                        parkingDTO.sqlVehYearDateStr = GetSafeValue(row, AutoISSUE.DBConstants.sqlVehYearDateStr);
                        parkingDTO.sqlVehVINStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlVehVINStr);
                        if (parkingDTO.sqlVehLicNoStr.Length > 0)
                        {
                            StringBuilder loVehicleDisplay = new StringBuilder();
                            AppendNonNullToSentenceStringBuilder(loVehicleDisplay, parkingDTO.sqlVehLicStateStr);
                            AppendNonNullToSentenceStringBuilder(loVehicleDisplay, parkingDTO.sqlVehLicNoStr);

                            parkingDTO.VEHICLE_DISPLAY = loVehicleDisplay.ToString();
                        }
                        else
                        {
                            parkingDTO.VEHICLE_DISPLAY = "VIN: " + parkingDTO.sqlVehVINStr;
                        }



                        parkingDTO.sqlIssueOfficerIDStr = GetSafeValue(row, Constants.OFFICER_ID);
                        parkingDTO.sqlIssueOfficerNameStr = GetSafeValue(row, Constants.OFFICER_NAME);


                        //parkingDTO.LocMeter = GetSafeValue(row, Constants.LOCMETER_COLUMN);
                        //parkingDTO.sqlLocBlockStr = GetSafeValue(row, Constants.LOCBLOCK_COLUMN);
                        //parkingDTO.sqlLocDirectionStr = GetSafeValue(row, Constants.LOCDIRECTION_COLUMN);
                        //parkingDTO.sqlLocStreetStr = GetSafeValue(row, Constants.LOCSTREET_COLUMN);

                        // location info
                        parkingDTO.sqlLocLotStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocLotStr);
                        parkingDTO.sqlLocBlockStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocBlockStr);
                        parkingDTO.sqlLocDirectionStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocDirectionStr);
                        parkingDTO.sqlLocStreetStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocStreetStr);
                        parkingDTO.sqlLocDescriptorStr = GetSafeColumnStringValueFromDataRow(row, AutoISSUE.DBConstants.sqlLocDescriptorStr);
                        if (parkingDTO.sqlLocLotStr.Length > 0)
                        {
                            parkingDTO.LOCATION_DISPLAY = parkingDTO.sqlLocLotStr;
                        }
                        else
                        {
                            StringBuilder loLocationDisplay = new StringBuilder();
                            AppendNonNullToSentenceStringBuilder(loLocationDisplay, parkingDTO.sqlLocBlockStr);
                            AppendNonNullToSentenceStringBuilder(loLocationDisplay, parkingDTO.sqlLocDirectionStr);
                            AppendNonNullToSentenceStringBuilder(loLocationDisplay, parkingDTO.sqlLocStreetStr);
                            AppendNonNullToSentenceStringBuilder(loLocationDisplay, parkingDTO.sqlLocDescriptorStr);

                            parkingDTO.LOCATION_DISPLAY = loLocationDisplay.ToString();
                        }


                        break;
                    }
                    return parkingDTO;
                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "tableName: " + tableName + " id: " + iSequenceId, "GetRowInfoById");
                    Console.WriteLine("Exception source: {0}", e.Source);
                }
                return null;
            });
        }


        private static string GetSafeValue(IDataRecord reader, string column)
        {
            try
            {
                return reader[column].ToString();
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, "column: " + column , "GetSafeValue");
                return string.Empty;
            }
        }

        private static string GetSafeValue(DataRow iSourceRow, string iColumnName)
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
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, "column: " + iColumnName, "GetSafeValue");
                loResultStr = string.Empty;
            }

            return loResultStr;
        }

		//Duplicate Row
	    public Task<bool> CreateDuplicateRowInfoById(string id, string tableName)
	    {
	        return Task.Factory.StartNew(() =>
	            {
	                try
	                {
	                    XMLConfig.IssStruct parkingStruct = DroidContext.XmlCfg.GetStruct(
	                        Constants.STRUCT_TYPE_CITE, tableName);

	                    var queryStringBuilder = new StringBuilder();
	                    queryStringBuilder.Append(" SELECT * FROM ");
	                    queryStringBuilder.Append(tableName);
	                    queryStringBuilder.Append(" WHERE ");
	                        queryStringBuilder.Append(Constants.ISSUENO_COLUMN);
	                    queryStringBuilder.Append(" = '");
	                    queryStringBuilder.Append(id);
	                    queryStringBuilder.Append("'");

                        var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());

                        foreach (DataRow row in result.Rows)
	                    {
	                        for (int i = 0; i < parkingStruct.Panels.Count; i++)
	                        {
	                            foreach (XMLConfig.PanelField panelField in parkingStruct.Panels[i].PanelFields)
	                            {
	                               /* if (panelField.IsHidden)
	                                    continue; */

	                                if (Constants.ISSUENO_COLUMN != panelField.Name 
                                        && Constants.ISSUENOPFX_COLUMN != panelField.Name
                                        && Constants.ISSUENOSFX_COLUMN != panelField.Name)
                                        panelField.Value = row[panelField.Name].ToString();
	                            }
	                        }
	                    }
	                }
	                catch (Exception e)
	                {
	                    LoggingManager.LogApplicationError(e, "tableName: " + tableName + " id: " + id,
	                                                       "CreateDuplicateRowInfoById");
	                    Console.WriteLine("Exception source: {0}", e.Source);
	                }
	                return true;
	            });
	    }


        public int GetRecordCountByWsStatus(string tableName, string wsStatus)
        {
            //build the query here
            var query = new StringBuilder();
            query.Append(" SELECT COUNT(*) FROM ");
            query.Append(tableName);
            query.Append(" WHERE " + Constants.WS_STATUS_COLUMN);
            query.Append(" = '");
            query.Append(wsStatus + "'");
            var retVal = (new DatabaseManager()).GetRecordCount(query.ToString());
            return retVal;
        }

	}
}



