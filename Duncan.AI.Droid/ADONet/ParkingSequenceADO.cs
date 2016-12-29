using System;
using System.Text;
using Android.Util;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android.Content;
using Duncan.AI.Droid.Utils.DataAccess;
using Duncan.AI.Droid.Utils.HelperManagers;
using System.Data;

namespace Duncan.AI.Droid
{
    public class ParkingSequenceADO
    {
        public ParkingSequenceADO()
        {
        }

        //Check if rows exists
        public static bool CheckRows(String seqName)
        {
            bool hasRows = false;
            try
            {
                var queryStringBuilder = new StringBuilder();
                queryStringBuilder.Append(" SELECT * FROM ");
                queryStringBuilder.Append(seqName + Constants.LABEL_SEQUENCE);
                hasRows = (new DatabaseManager()).CheckForRows(queryStringBuilder.ToString());
                return hasRows;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception source: {0}", e.Source);
            }
            return hasRows;
        }

        //Insert Row into the database 
        public static void CreateSequenceTable(NewDataSet newDataSet, String seqName)
        {
            var crtTblStrBldr = new StringBuilder();
            crtTblStrBldr.Append("CREATE TABLE if not exists ");
            crtTblStrBldr.Append(seqName + Constants.LABEL_SEQUENCE);
            crtTblStrBldr.Append(" ( ");
            crtTblStrBldr.Append(Constants.ID_COLUMN);
            crtTblStrBldr.Append(" INTEGER PRIMARY KEY AUTOINCREMENT");
            crtTblStrBldr.Append(", ");
            crtTblStrBldr.Append(Constants.SEQUENCE_ID);
            crtTblStrBldr.Append(" ntext");
            crtTblStrBldr.Append(", ");
            crtTblStrBldr.Append(Constants.SRCISSUENOPFX_COLUMN);
            crtTblStrBldr.Append(" ntext");
            crtTblStrBldr.Append(", ");
            crtTblStrBldr.Append(Constants.SRCISSUENOSFX_COLUMN);
            crtTblStrBldr.Append(" ntext");
            crtTblStrBldr.Append(", ");
            crtTblStrBldr.Append(Constants.BOOKNUMBER_COLUMN);
            crtTblStrBldr.Append(" ntext");
            crtTblStrBldr.Append(", ");
            crtTblStrBldr.Append(Constants.SRCINUSE_FLAG);
            crtTblStrBldr.Append(" ntext");
            crtTblStrBldr.Append(" );");
            CommonADO.ExecuteQuery(crtTblStrBldr.ToString());
            InsertSequenceRow(newDataSet, seqName);
        }
        
        //Insert Row into the database 
        private static void InsertSequenceRow(NewDataSet newDataSet, String seqName)
        {
            if (newDataSet != null)
            {
                string insertStartTemplate = "INSERT INTO " + seqName + Constants.LABEL_SEQUENCE + " ( "
                                                   + Constants.SEQUENCE_ID + ", "
                                                   + Constants.SRCISSUENOPFX_COLUMN + ", "
                                                   + Constants.SRCISSUENOSFX_COLUMN + ", "
                                                   + Constants.BOOKNUMBER_COLUMN + ", "
                                                   + Constants.SRCINUSE_FLAG + " ) VALUES ( '";
                const string seperator = "', '";
                const string end = "');";
                foreach (NewDataSetTable dataSetObject in newDataSet.Items)
                {
                    var instRowStrBldr = new StringBuilder();
                    instRowStrBldr.Append(insertStartTemplate);
                    instRowStrBldr.Append(dataSetObject.ALLOCATIONNO);
                    instRowStrBldr.Append(seperator);
                    instRowStrBldr.Append(dataSetObject.PREFIX);
                    instRowStrBldr.Append(seperator);
                    instRowStrBldr.Append(dataSetObject.SUFFIX);
                    instRowStrBldr.Append(seperator);
                    instRowStrBldr.Append(dataSetObject.BOOKNUMBER);
                    instRowStrBldr.Append(seperator);
                    instRowStrBldr.Append(Constants.SRCINUSE_FLAG_NOTUSED); 
                    instRowStrBldr.Append(end);
                    var builderString = instRowStrBldr.ToString();
                    CommonADO.ExecuteQuery(builderString);
                }
            }
        }

        // get the rowId for a pre-allocated record row
        public static string GetRowIdForInUseSequenceId( string iTableName, string iIssueNumberStr )
        {
            string loResultId = "";

            try
            {
                var queryStringBuilder = new StringBuilder();
                queryStringBuilder.Append(" SELECT * FROM " + iTableName );
                queryStringBuilder.Append(" WHERE " + AutoISSUE.DBConstants.sqlIssueNumberStr + " = " + iIssueNumberStr);

                var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());

                foreach (DataRow row in result.Rows)
                {
                    loResultId = row[Constants.ID_COLUMN].ToString();
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception source: {0}", e.Source);
            }

            return loResultId;
        }


        //Get In use sequence id
        public static CommonDTO GetInUseSequenceId(string seqName)
        {
            var commonDTO = new CommonDTO {seqId = null};
            try
            {
                var queryStringBuilder = new StringBuilder();
                queryStringBuilder.Append(" SELECT * FROM ");
                queryStringBuilder.Append(seqName + Constants.LABEL_SEQUENCE);
                queryStringBuilder.Append(" WHERE INUSEFLAG = ");
                queryStringBuilder.Append(Constants.SRCINUSE_FLAG_USED);

                // TODO - can this be more effecient by just fetching the 1 row we want? Need to check about ordering
                //queryStringBuilder.Append(" DESC LIMIT 1");



                var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());

                foreach (DataRow row in result.Rows)
                {
                    commonDTO.seqId = row[Constants.SEQUENCE_ID].ToString();
                    commonDTO.rowId = row[Constants.ID_COLUMN].ToString();
                    commonDTO.sqlIssueNumberPrefixStr = row[Constants.SRCISSUENOPFX_COLUMN].ToString();
                    commonDTO.sqlIssueNumberSuffixStr = row[Constants.SRCISSUENOSFX_COLUMN].ToString();
                    commonDTO.bookNumber = row[Constants.BOOKNUMBER_COLUMN].ToString();
                    commonDTO.inUseFlag = row[Constants.SRCINUSE_FLAG].ToString();
                    break;
                }                
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception source: {0}", e.Source);
            }

            return commonDTO;
        }

        //Update in use sequence id
        public static void UpdateInUseSequenceId(string seqid, string data, String seqName)
        {

            var updateStrBldr = new StringBuilder();
            updateStrBldr.Append("UPDATE ");
            updateStrBldr.Append(seqName + Constants.LABEL_SEQUENCE);
            updateStrBldr.Append(" SET ");
            updateStrBldr.Append(Constants.SRCINUSE_FLAG);
            updateStrBldr.Append(" = ");
            updateStrBldr.Append(data);
            updateStrBldr.Append(" WHERE ");
            updateStrBldr.Append(Constants.SEQUENCE_ID);
            updateStrBldr.Append(" = ");
            updateStrBldr.Append(seqid);
            
            CommonADO.ExecuteQuery(updateStrBldr.ToString());
        }

        //Update in use sequence id
        public static void ResetAllPendingTickets()
        {
            List<XMLConfig.IssStruct> structs = DroidContext.XmlCfg.IssStructs;
            foreach (var structI in structs)
            {
                if (structI.Type.Equals(Constants.STRUCT_TYPE_CITE))
                {
                    // not all structures have sequences
                    if (string.IsNullOrEmpty(structI.SequenceName) == true)
                    {
                        continue;
                    }


                    var updateStrBldr = new StringBuilder();
                    updateStrBldr.Append("UPDATE ");
                    updateStrBldr.Append(structI.SequenceName + Constants.LABEL_SEQUENCE);
                    updateStrBldr.Append(" SET ");
                    updateStrBldr.Append(Constants.SRCINUSE_FLAG);
                    updateStrBldr.Append(" = ");
                    updateStrBldr.Append(Constants.SRCINUSE_FLAG_NOTUSED);
                    updateStrBldr.Append(" WHERE ");
                    updateStrBldr.Append(Constants.SRCINUSE_FLAG);
                    updateStrBldr.Append(" = ");
                    updateStrBldr.Append(Constants.SRCINUSE_FLAG_USED);

                    CommonADO.ExecuteQuery(updateStrBldr.ToString());
                }
            }            
        }

        //Get Next Sequence ID
        public static CommonDTO GetSequenceId(String seqName)
        {
            var commonDTO = new CommonDTO();
            try
            {
                var queryStringBuilder = new StringBuilder();
                queryStringBuilder.Append(" SELECT * FROM ");
                queryStringBuilder.Append(seqName + Constants.LABEL_SEQUENCE);
                queryStringBuilder.Append(" WHERE INUSEFLAG = ");
                queryStringBuilder.Append(Constants.SRCINUSE_FLAG_NOTUSED);
                queryStringBuilder.Append(" ORDER BY ");
                queryStringBuilder.Append(Constants.SEQUENCE_ID);
                queryStringBuilder.Append(" ASC LIMIT 1 ");

                var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());

                foreach (DataRow row in result.Rows)
                {
                    commonDTO.seqId = row[Constants.SEQUENCE_ID].ToString();
                    commonDTO.rowId = row[Constants.ID_COLUMN].ToString();
                    commonDTO.sqlIssueNumberPrefixStr = row[Constants.SRCISSUENOPFX_COLUMN].ToString();
                    commonDTO.sqlIssueNumberSuffixStr = row[Constants.SRCISSUENOSFX_COLUMN].ToString();
                    commonDTO.bookNumber = row[Constants.BOOKNUMBER_COLUMN].ToString();
                    commonDTO.inUseFlag = row[Constants.SRCINUSE_FLAG].ToString();
                    break;
                }
                Log.Debug("GetSequenceId::", commonDTO.seqId);

                UpdateInUseSequenceId(commonDTO.seqId, Constants.SRCINUSE_FLAG_USED, seqName);

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception source: {0}", e.Source);
            }
            return commonDTO;
        }

        //Insert Void ticket in ParkVoid table
        public Task<bool> InsertRowParkingVoid( string iMasterKey, 
                                                string sequenceId, string officerId, 
                                                string officerName, string cancelReason, Context ctx, string structName)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var insertStrBldr = new StringBuilder();
                    insertStrBldr.Append("INSERT INTO ");
                    insertStrBldr.Append(structName);
                    insertStrBldr.Append(" ( ");
                    insertStrBldr.Append(Constants.MASTERKEY_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.OFFICER_NAME);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.OFFICER_ID);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.STATUS_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.WS_STATUS_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.RECCREATIONDATE_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.RECCREATIONTIME_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.CANCELREASON_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.SEQUENCE_ID);
                    insertStrBldr.Append(" ) VALUES ( '");
                    insertStrBldr.Append(iMasterKey);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(officerName);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(officerId);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(Constants.STATUS_READY);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(Constants.WS_STATUS_READY);
                    insertStrBldr.Append("', '");
                    //insertStrBldr.Append(DateTimeManager.GetDate(null, null, Constants.DT_YYYYMMDD));
                    insertStrBldr.Append(DateTimeManager.GetDate(null, null, Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK));
                    insertStrBldr.Append("', '");
                    //insertStrBldr.Append(DateTimeManager.GetDate(null, null, Constants.TIME_HHMMSS));
                    insertStrBldr.Append(DateTimeManager.GetDate(null, null, Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK));
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(cancelReason);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(sequenceId);
                    insertStrBldr.Append("');");

                    bool result = CommonADO.ExecuteQuery(insertStrBldr.ToString());
                    ctx.StartService(new Intent(ctx, typeof(SyncService)));
                    return result;
                }
                catch (Exception e)
                {
                    //TODO Need to come up with better exception handling, something like saving to SQLite or Toast msg
                    Console.WriteLine("Exception source: {0}", e.Source);
                }
                return false;
            });
        }



        //Insert re issue ticket in park re issue table
        public Task<bool> InsertRowParkingReIssue( string iMasterKey,
                                                   string srcIssueNumberPrefix, string srcIssueNumber,  string srcIssueNumberSuffix,
                                                   string officerId, string officerName, Context ctx, string structName)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var insertStrBldr = new StringBuilder();
                    insertStrBldr.Append("INSERT INTO ");
                    insertStrBldr.Append(structName);
                    insertStrBldr.Append(" ( ");
                    insertStrBldr.Append(Constants.MASTERKEY_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.SRCISSUENOPFX_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.SRCISSUENO_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.SRCISSUENOSFX_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.OFFICER_NAME);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.OFFICER_ID);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.STATUS_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.WS_STATUS_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.RECCREATIONDATE_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.RECCREATIONTIME_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.SEQUENCE_ID);
                    insertStrBldr.Append(" ) VALUES ( '");
                    insertStrBldr.Append(iMasterKey);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(srcIssueNumberPrefix);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(srcIssueNumber);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(srcIssueNumberSuffix);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(officerName);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(officerId);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(Constants.STATUS_READY);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(Constants.WS_STATUS_READY);
                    insertStrBldr.Append("', '");
                    //insertStrBldr.Append(DateTimeManager.GetDate(null, null, Constants.DT_YYYYMMDD));
                    insertStrBldr.Append(DateTimeManager.GetDate(null, null, Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK));
                    insertStrBldr.Append("', '");
                    //insertStrBldr.Append(DateTimeManager.GetDate(null, null, Constants.TIME_HHMMSS));
                    insertStrBldr.Append(DateTimeManager.GetDate(null, null, Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK));
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(srcIssueNumber);
                    insertStrBldr.Append("');");

                    bool result = CommonADO.ExecuteQuery(insertStrBldr.ToString());
                    ctx.StartService(new Intent(ctx, typeof(SyncService)));
                    return result;
                }
                catch (Exception e)
                {
                    //TODO Need to come up with better exception handling, something like saving to SQLite or Toast msg
                    Console.WriteLine("Exception source: {0}", e.Source);
                }
                return false;
            });
        }


#if _old_
        //Insert re issue ticket in park re issue table
        public Task<bool> InsertRowParkingReIssue(string seqId, string issuenoPfx, string officerId, string officerName, Context ctx, string structName)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var insertStrBldr = new StringBuilder();
                    insertStrBldr.Append("INSERT INTO ");
                    insertStrBldr.Append(structName);
                    insertStrBldr.Append(" ( ");
                    insertStrBldr.Append(Constants.MASTERKEY_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.SRCISSUENO_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.SRCISSUENOPFX_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.OFFICER_NAME);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.OFFICER_ID);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.STATUS_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.WS_STATUS_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.RECCREATIONDATE_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.RECCREATIONTIME_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.SEQUENCE_ID);
                    insertStrBldr.Append(" ) VALUES ( '");
                    insertStrBldr.Append("0");//master key id has to be 0 for re-issuing and voiding tickets
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(seqId);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(issuenoPfx);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(officerName);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(officerId);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(Constants.STATUS_READY);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(Constants.WS_STATUS_READY);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(DateTimeManager.GetDate(null, null, Constants.DT_YYYYMMDD));
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(DateTimeManager.GetDate(null, null, Constants.TIME_HHMMSS));
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(seqId);
                    insertStrBldr.Append("');");

                    bool result = CommonADO.ExecuteQuery(insertStrBldr.ToString());
                    ctx.StartService(new Intent(ctx, typeof(SyncService)));
                    return result;
                }
                catch (Exception e)
                {
                    //TODO Need to come up with better exception handling, something like saving to SQLite or Toast msg
                    Console.WriteLine("Exception source: {0}", e.Source);
                }
                return false;
            });
        }

#endif
        //Get all notes by sequence id from DB
        public Task<List<ParkNoteDTO>> GetParkNotesBySequenceId(string sequenceId)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var parkNoteDTOList = new List<ParkNoteDTO>();

                    var queryStringBuilder = new StringBuilder();
                    queryStringBuilder.Append(" SELECT * FROM ");
                    queryStringBuilder.Append(Constants.PARKNOTE_TABLE);
                    queryStringBuilder.Append(" WHERE ");
                    queryStringBuilder.Append(Constants.SEQUENCE_ID);
                    queryStringBuilder.Append(" = '");
                    queryStringBuilder.Append(sequenceId);
                    queryStringBuilder.Append("'");

                    var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());

                    foreach (DataRow row in result.Rows)
                    {
                        //String listItemText = reader[Constants.DETAILRECNO_COLUMN].ToString() + ".  " + reader[Constants.NOTESMEMO_COLUMN].ToString().PadRight(40,'.').Substring(0, 40) + "...";
                        var parkNoteDTO = new ParkNoteDTO();

                        //parkNoteDTO.DBRowId = row[Constants.ID_COLUMN].ToString();
                        //parkNoteDTO.SeqId = row[Constants.SEQUENCE_ID].ToString();
                        //parkNoteDTO.NotesMemo = row[Constants.NOTESMEMO_COLUMN].ToString().PadRight(40, '.').Substring(0, 40) + "...";

                        GetParkNoteDataIntoDTO(row, parkNoteDTO);

                        parkNoteDTOList.Add(parkNoteDTO);
                    }

                    return parkNoteDTOList;
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
        /// Get all Park Note data columns into DTO
        /// </summary>
        /// <param name="iRow"></param>
        /// <param name="oneParkNoteDTO"></param>
        private void GetParkNoteDataIntoDTO(DataRow iRow, ParkNoteDTO oneParkNoteDTO)
        {
            oneParkNoteDTO.DBRowId = Helper.GetSafeColumnStringValueFromDataRow(iRow, Constants.ID_COLUMN);
            oneParkNoteDTO.SeqId = Helper.GetSafeColumnStringValueFromDataRow(iRow, Constants.SEQUENCE_ID);
            oneParkNoteDTO.MasterKey = Helper.GetSafeColumnStringValueFromDataRow(iRow, AutoISSUE.DBConstants.sqlMasterKeyStr);

            oneParkNoteDTO.NoteDate = Helper.GetSafeColumnStringValueFromDataRow(iRow, Constants.NOTEDATE_COLUMN);
            oneParkNoteDTO.NoteTime = Helper.GetSafeColumnStringValueFromDataRow(iRow, Constants.NOTETIME_COLUMN);
            oneParkNoteDTO.NotesMemo = Helper.GetSafeColumnStringValueFromDataRow(iRow, Constants.NOTESMEMO_COLUMN);

            oneParkNoteDTO.Diagram = Helper.GetSafeColumnStringValueFromDataRow(iRow, Constants.DIAGRAM_COLUMN);

            oneParkNoteDTO.MultiMediaNoteDataType = Helper.GetSafeColumnStringValueFromDataRow(iRow, Constants.MULTIMEDIANOTEDATATYPE_COLUMN);
            oneParkNoteDTO.MultiMediaNoteFileName = Helper.GetSafeColumnStringValueFromDataRow(iRow, Constants.MULTIMEDIANOTEFILENAME_COLUMN);
            oneParkNoteDTO.MultiMediaNoteDateStamp = Helper.GetSafeColumnStringValueFromDataRow(iRow, Constants.MULTIMEDIANOTEFILEDATESTAMP_COLUMN);
            oneParkNoteDTO.MultiMediaNoteTimeStamp = Helper.GetSafeColumnStringValueFromDataRow(iRow, Constants.MULTIMEDIANOTEFILETIMESTAMP_COLUMN);
        }

        //Get one note by note id from DB
        public Task<ParkNoteDTO> GetParkNoteByNoteId(string noteID)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var parkNoteDTO = new ParkNoteDTO();

                    var queryStringBuilder = new StringBuilder();
                    queryStringBuilder.Append(" SELECT * FROM ");
                    queryStringBuilder.Append(Constants.PARKNOTE_TABLE);
                    queryStringBuilder.Append(" WHERE ");
                    queryStringBuilder.Append(Constants.ID_COLUMN);
                    queryStringBuilder.Append(" = '");
                    queryStringBuilder.Append(noteID);
                    queryStringBuilder.Append("'");

                    var result = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());

                    foreach (DataRow row in result.Rows)
                    {
                        //parkNoteDTO.DBRowId = row[Constants.ID_COLUMN].ToString();
                        //parkNoteDTO.SeqId = row[Constants.SEQUENCE_ID].ToString();
                        //parkNoteDTO.NoteDate = row[Constants.NOTEDATE_COLUMN].ToString();
                        //parkNoteDTO.NoteTime = row[Constants.NOTETIME_COLUMN].ToString();
                        //parkNoteDTO.NotesMemo = row[Constants.NOTESMEMO_COLUMN].ToString();
                        //parkNoteDTO.Diagram = row[Constants.DIAGRAM_COLUMN].ToString();
                        //parkNoteDTO.MultiMediaNoteDataType = row[Constants.MULTIMEDIANOTEDATATYPE_COLUMN].ToString();
                        //parkNoteDTO.MultiMediaNoteFileName = row[Constants.MULTIMEDIANOTEFILENAME_COLUMN].ToString();
                        //parkNoteDTO.MultiMediaNoteDateStamp =
                        //    row[Constants.MULTIMEDIANOTEFILEDATESTAMP_COLUMN].ToString();
                        //parkNoteDTO.MultiMediaNoteTimeStamp =
                        //    row[Constants.MULTIMEDIANOTEFILETIMESTAMP_COLUMN].ToString();

                        GetParkNoteDataIntoDTO(row, parkNoteDTO);

                        break;
                    }
                    return parkNoteDTO;
                }
                catch (Exception e)
                {
                    //TODO Need to come up with better exception handling, something like saving to SQLite or Toast msg
                    Console.WriteLine("Exception source: {0}", e.Source);
                }
                return null;
            });
        }

        //Insert Void ticket in ParkVoid table
        public Task<bool> InsertRowParkNote(ParkNoteDTO parkNoteDTO)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var insertStrBldr = new StringBuilder();
                    insertStrBldr.Append("INSERT INTO ");
                    insertStrBldr.Append(Constants.PARKNOTE_TABLE);
                    insertStrBldr.Append(" ( ");
                    insertStrBldr.Append(Constants.MASTERKEY_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.SEQUENCE_ID);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.NOTEDATE_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.NOTETIME_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.NOTESMEMO_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.DIAGRAM_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.MULTIMEDIANOTEDATATYPE_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.MULTIMEDIANOTEFILENAME_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.MULTIMEDIANOTEFILEDATESTAMP_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.MULTIMEDIANOTEFILETIMESTAMP_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.STATUS_COLUMN);
                    insertStrBldr.Append(", ");
                    insertStrBldr.Append(Constants.WS_STATUS_COLUMN);
                    insertStrBldr.Append(" ) VALUES ( '");
                    insertStrBldr.Append("0"); //dont need master key column, can be 0
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(parkNoteDTO.SeqId);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(parkNoteDTO.NoteDate);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(parkNoteDTO.NoteTime);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(parkNoteDTO.NotesMemo);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(parkNoteDTO.Diagram);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(parkNoteDTO.MultiMediaNoteDataType);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(parkNoteDTO.MultiMediaNoteFileName);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(parkNoteDTO.MultiMediaNoteDateStamp);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(parkNoteDTO.MultiMediaNoteTimeStamp);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(Constants.STATUS_INPROCESS);
                    insertStrBldr.Append("', '");
                    insertStrBldr.Append(Constants.WS_STATUS_READY);
                    insertStrBldr.Append("');");

                    bool result = CommonADO.ExecuteQuery(insertStrBldr.ToString());
                    return result;
                }
                catch (Exception e)
                {
                    //TODO Need to come up with better exception handling, something like saving to SQLite or Toast msg
                    Console.WriteLine("Exception source: {0}", e.Source);
                }
                return false;
            });
        }

        public Task<bool> UpdateParkNote(ParkNoteDTO parkNoteDTO)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var updateStrBldr = new StringBuilder();
                    updateStrBldr.Append("UPDATE ");
                    updateStrBldr.Append(Constants.PARKNOTE_TABLE);
                    updateStrBldr.Append(" SET ");
                    updateStrBldr.Append(Constants.NOTESMEMO_COLUMN);
                    updateStrBldr.Append(" = '");
                    updateStrBldr.Append(parkNoteDTO.NotesMemo.Replace("'", "''"));
                    updateStrBldr.Append("'");
                    updateStrBldr.Append(" WHERE ");
                    updateStrBldr.Append(Constants.ID_COLUMN);
                    updateStrBldr.Append(" = '");
                    updateStrBldr.Append(parkNoteDTO.DBRowId);
                    updateStrBldr.Append("'");

                    CommonADO.ExecuteQuery(updateStrBldr.ToString());
                    Log.Debug("ParkingSequenceADO::updateParkNote", parkNoteDTO.DBRowId);
                    return true;
                }
                catch (Exception e)
                {
                    //TODO Need to come up with better exception handling, something like saving to SQLite or Toast msg
                    Console.WriteLine("Exception source: {0}", e.Source);
                }
                return false;
            });
        }
    }
}

