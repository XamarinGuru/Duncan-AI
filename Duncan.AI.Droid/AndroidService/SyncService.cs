// define this guy to write upload files to filesystem for debugging
//#define WRITE_UPLOAD_FILES_TO_DEBUG_FOLDER



using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Preferences;
using Android.Util;
using Android.Runtime;
using Duncan.AI.Droid.Utils.HelperManagers;
using Duncan.AI.Droid.Utils;

namespace Duncan.AI.Droid
{
    [Service]
    [IntentFilter(new String[] { "Duncan.AI.Droid.SyncService" })]
    class SyncService : IntentService
	{
        public SyncService()
            : base("SyncService")
		{
		}

        private void HandleSyncServiceException(Exception e)
        {
            LoggingManager.LogApplicationError(e, "SyncService Exception", e.TargetSite.Name);
            ErrorHandling.ReportExceptionWithConfirmationDlg(e.Message);
        }


        string _DebugDATDir = string.Empty;


        // AJW TODO - this is extremely ineffecient to check every struct every time. 
        // instead, pass target struct name as a parameter and only target that one
        // ALSO - we're calling this on form init, which doesn't make much sense, unless it was a kludge to upload previously posted data?



		//onHanleIntent method runs on a new thread. 
		protected override void OnHandleIntent (Intent intent)
		{
            Log.Debug("SyncService", "OnHandleIntent");
		    try
		    {
                var importRecordImpl = new IImportRecordImpl();                
		        ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
		        string officerId = prefs.GetString(Constants.OFFICER_ID, null);
		        string officerName = prefs.GetString(Constants.OFFICER_NAME, null);
		        string deviceId = prefs.GetString(Constants.DEVICEID, null);
                string whereClause = " WHERE " + Constants.WS_STATUS_COLUMN + " = '" + Constants.WS_STATUS_READY + "'";
                
		        List<XMLConfig.IssStruct> structs = DroidContext.XmlCfg.IssStructs;
                foreach (var structI in structs)
                {       
                    var datFiles = CommonADO.GenerateDATFile(structI.Name, whereClause, structI.Type);
                    if (datFiles.Count > 0)
                    {
                        string revision = prefs.GetString(structI.Name + Constants.LABEL_REVISION, null);
                        int iStructCfgRev = 0;
                        if (revision != null)
                        {
                            iStructCfgRev = Convert.ToInt32(revision);
                        }
                        Log.Debug("SyncService::::struct Name::", structI.Name);
                        foreach (CommonDTO commonDTO in datFiles)
                        {
                            try
                            {
                                Log.Debug("SyncService::Row ID::", commonDTO.rowId);
                                //set this as empty string and NOT a null string. Doesnt work with the public web services.
                                string errorMsg = string.Empty;
                                var mkValues = string.Format(Constants.MasterKeyValueTemplate, commonDTO.seqId, commonDTO.sqlIssueDateStr, commonDTO.sqlIssueTimeStr);
                                string parentStruct;
                                if (structI.ParentStruct == null)
                                {
                                    parentStruct = "";
                                }
                                else
                                {
                                    parentStruct = structI.ParentStruct;
                                }



#if WRITE_UPLOAD_FILES_TO_DEBUG_FOLDER
                                // debug - let's see what's in the file we are uploading
                                string loDATDebugFileName = string.Empty;
                                try
                                {
                                    if (commonDTO.datFile.Length > 0)
                                    {
                                        if (ValidateOrCreateDirectoryForAIWebProxyDebugFiles() == true)
                                        {

                                            loDATDebugFileName = _DebugDATDir + "/" +
                                                                 structI.Name + " "  +
                                                                 Constants.SERIAL_NUMBER + " " +
                                                                 DateTimeOffset.Now.ToString(Constants.DEBUG_FILENAME_TIMESTAMP_FORMAT) + ".DAT";


                                            var loFile = new FileStream(loDATDebugFileName, FileMode.Create, FileAccess.Write);
                                            loFile.Write(commonDTO.datFile, 0, commonDTO.datFile.Length);
                                            loFile.Flush();
                                            loFile.Close();
                                            loFile = null;
                                        }
                                    }                            

                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Error writing debug DAT file: {0} {0}", loDATDebugFileName, e.Message);
                                }
#else
                                // don't want these, make sure there isn't anything leftover
                                RemoveDirectoryForAIWebProxyDebugFiles();
#endif




                                // 7.24.03 should be fixed now, we'll leave this trap in place for validation
                                //Some times the Client URL is missed and the syncService crashes, do the check before we continue
                                if (string.IsNullOrEmpty(DuncanWebServicesClient.WebserviceConstants.CLIENT_URL))
                                {
                                    errorMsg = "Missing Client URL";
                                }
                                else
                                {
                                    importRecordImpl.ImportRecord(iStructCfgRev, mkValues, commonDTO.datFile, commonDTO.attachment,
                                      officerName, officerId, ref errorMsg, structI.Name, parentStruct);
                                }



                                if (errorMsg.Equals(Constants.AIHostKnownDetailRecordError) == true)
                                {
                                    // sort of OK for detail recs, indicates they are being sent 2x? TODO - need to research this 
                                    if (string.IsNullOrEmpty(structI.ParentStruct) == false)
                                    {
                                        // this is some kind of connectivity error, should log this but not force this to be in the user's face repeatedly
                                        LoggingManager.LogApplicationError(null, "SyncService WebException. " + structI.Name + " ", errorMsg);
                                        errorMsg = string.Empty;
                                    }

                                }



                                if (errorMsg == "" || errorMsg == Constants.AIHostKnownAutoProcError)
                                {

                                    CommonADO.UpdateRowStatus( commonDTO.rowId, structI.Name, Constants.STATUS_ISSUED, Constants.WS_STATUS_SUCCESS );
                                }
                                else
                                {
                                    //There is an error, inform the user
                                    ErrorHandling.ThrowError(DroidContext.mainActivity, ErrorHandling.ErrorCode.SyncService, errorMsg);
                                }
                            }
                            catch (System.Net.WebException webEx)
                            {
                                // this is some kind of connectivity error, should log this but not force this to be in the user's face repeatedly
                                LoggingManager.LogApplicationError(webEx, "SyncService WebException ", webEx.TargetSite.Name);

                                // we're done for now, come back later and see if we can do this successfully
                                // by having the ACTIVITYLOG log active every N minutes, we will be initiating another try soon... in N minutes
                                // as long as this option is active in registry, we don't need another timer to re-try the upload
                                return;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception source: {0}", e.Source);
                                HandleSyncServiceException(e);
                            }
                        }
                    }                                      
                }	      
		    }
		    catch (Exception e)
		    {
		        Console.WriteLine("Exception source: {0}", e.Source);
                HandleSyncServiceException(e);
		    }
		}

#if _old_way_saved_for_reference_
        
		//onHanleIntent method runs on a new thread. 
		protected override void OnHandleIntent (Intent intent)
		{
            Log.Debug("SyncService", "OnHandleIntent");
		    try
		    {
                var importRecordImpl = new IImportRecordImpl();                
		        ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
		        string officerId = prefs.GetString(Constants.OFFICER_ID, null);
		        string officerName = prefs.GetString(Constants.OFFICER_NAME, null);
		        string deviceId = prefs.GetString(Constants.DEVICEID, null);
                string whereClause = " WHERE " + Constants.WS_STATUS_COLUMN + " = '" + Constants.WS_STATUS_READY + "'";
                
		        List<XMLConfig.IssStruct> structs = DroidContext.XmlCfg.IssStructs;
                foreach (var structI in structs)
                {       
                    var datFiles = CommonADO.GenerateDATFile(structI.Name, whereClause, structI.Type);
                    if (datFiles.Count > 0)
                    {
                        string revision = prefs.GetString(structI.Name + Constants.LABEL_REVISION, null);
                        int iStructCfgRev = 0;
                        if (revision != null)
                        {
                            iStructCfgRev = Convert.ToInt32(revision);
                        }
                        Log.Debug("SyncService::::struct Name::", structI.Name);
                        foreach (CommonDTO commonDTO in datFiles)
                        {
                            try
                            {
                                Log.Debug("SyncService::Row ID::", commonDTO.rowId);
                                //set this as empty string and NOT a null string. Doesnt work with the public web services.
                                string errorMsg = "";
                                var mkValues = string.Format(Constants.MasterKeyValueTemplate, commonDTO.seqId, commonDTO.sqlIssueDateStr, commonDTO.sqlIssueTimeStr);
                                string parentStruct;
                                if (structI.ParentStruct == null)
                                {
                                    parentStruct = "";
                                }
                                else
                                {
                                    parentStruct = structI.ParentStruct;
                                }



//# if WRITE_UPLOAD_FILES_TO_DEBUG_FOLDER
                                // debug - let's see what's in the file we are uploading
                                string loDATDebugFileName = string.Empty;
                                try
                                {
                                    if (commonDTO.datFile.Length > 0)
                                    {
                                        if (ValidateOrCreateDirectoryForAIWebProxyDebugFiles() == true)
                                        {

                                            loDATDebugFileName = _DebugDATDir + "/" +
                                                                 structI.Name + " "  +
                                                                 Constants.SERIAL_NUMBER + " " +
                                                                 DateTimeOffset.Now.ToString(Constants.DEBUG_FILENAME_TIMESTAMP_FORMAT) + ".DAT";


                                            var loFile = new FileStream(loDATDebugFileName, FileMode.Create, FileAccess.Write);
                                            loFile.Write(commonDTO.datFile, 0, commonDTO.datFile.Length);
                                            loFile.Flush();
                                            loFile.Close();
                                            loFile = null;
                                        }
                                    }                            

                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Error writing debug DAT file: {0} {0}", loDATDebugFileName, e.Message);
                                }
//# else
                                // don't want these, make sure there isn't anything leftover
                                RemoveDirectoryForAIWebProxyDebugFiles();
//# endif




                                // 7.24.03 should be fixed now, we'll leave this trap in place for validation
                                //Some times the Client URL is missed and the syncService crashes, do the check before we continue
                                if (string.IsNullOrEmpty(DuncanWebServicesClient.WebserviceConstants.CLIENT_URL))
                                {
                                    errorMsg = "Missing Client URL";
                                }
                                else
                                {
                                    importRecordImpl.ImportRecord(iStructCfgRev, mkValues, commonDTO.datFile, commonDTO.attachment,
                                      officerName, officerId, ref errorMsg, structI.Name, parentStruct);
                                }

                                if (errorMsg == "" || errorMsg == Constants.AIHostKnownAutoProcError)
                                {

                                    CommonADO.UpdateRowStatus( commonDTO.rowId, structI.Name, Constants.STATUS_ISSUED, Constants.WS_STATUS_SUCCESS );



                                    //
                                    // -  these should be deleted when the master record is
                                    //
                                    //if (structI.Type.Equals(Constants.STRUCT_TYPE_VOID))
                                    //{
                                    //    CommonADO.DeleteRow(commonDTO.rowId, structI.Name);
                                    //}
                                    //else if (structI.Type.Equals(Constants.STRUCT_TYPE_REISSUE))
                                    //{
                                    //    CommonADO.DeleteRow(commonDTO.rowId, structI.Name);
                                    //}


                                    /*

                                    if (structI.Type.Equals(Constants.STRUCT_TYPE_CITE))
                                    {
                                        CommonADO.UpdateRowStatus(commonDTO.seqId, structI.Name, Constants.STATUS_ISSUED, Constants.WS_STATUS_SUCCESS, structI.Type);
                                    }
                                    else if (
                                                structI.Type.Equals(Constants.STRUCT_TYPE_NOTES) ||
                                                structI.Type.Equals(Constants.STRUCT_TYPE_CHALKING) ||
                                                structI.Type.Equals(Constants.STRUCT_TYPE_ACTIVITYLOG) ||
                                                structI.Type.Equals(Constants.STRUCT_TYPE_GENERIC_ISSUE)
                                        )
                                    {
                                        CommonADO.UpdateRowStatus(commonDTO.rowId, structI.Name, Constants.STATUS_ISSUED, Constants.WS_STATUS_SUCCESS, structI.Type);
                                    }
                                    else if (structI.Type.Equals(Constants.STRUCT_TYPE_VOID))
                                    {
                                        CommonADO.DeleteRow(commonDTO.rowId, structI.Name);
                                        CommonADO.UpdateRowStatus(commonDTO.seqId, structI.ParentStruct, Constants.STATUS_VOIDED, Constants.WS_STATUS_SUCCESS, null);
                                    }
                                    else if (structI.Type.Equals(Constants.STRUCT_TYPE_REISSUE))
                                    {
                                        CommonADO.DeleteRow(commonDTO.rowId, structI.Name);
                                        CommonADO.UpdateRowStatus(commonDTO.seqId, structI.ParentStruct, Constants.STATUS_REISSUE, Constants.WS_STATUS_SUCCESS, null);
                                    }
                                     
                                     */
                                }
                                else
                                {
                                    //There is an error, inform the user
                                    ErrorHandling.ThrowError(DroidContext.mainActivity, ErrorHandling.ErrorCode.SyncService, errorMsg);
                                }
                            }
                            catch (System.Net.WebException webEx)
                            {
                                // this is some kind of connectivity error, should log this but not force this to be in the user's face repeatedly
                                LoggingManager.LogApplicationError(webEx, "SyncService WebException ", webEx.TargetSite.Name);

                                // we're done for now, come back later and see if we can do this successfully
                                // by having the ACTIVITYLOG log active every N minutes, we will be initiating another try soon... in N minutes
                                // as long as this option is active in registry, we don't need another timer to re-try the upload
                                return;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception source: {0}", e.Source);
                                HandleSyncServiceException(e);
                            }
                        }
                    }                                      
                }	      
		    }
		    catch (Exception e)
		    {
		        Console.WriteLine("Exception source: {0}", e.Source);
                HandleSyncServiceException(e);
		    }
		}

#endif

        private bool ValidateOrCreateDirectoryForAIWebProxyDebugFiles()
        {
            try
            {
                if (_DebugDATDir.Length == 0)
                {
                    _DebugDATDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads) + "/" + Constants.AIWEBPROXY_DEBUG_FOLDERNAME;
                }

                if (!Directory.Exists(_DebugDATDir))
                {
                    Directory.CreateDirectory(_DebugDATDir);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error creating debug folder: {0}", exp.Message);
                _DebugDATDir = string.Empty;
                HandleSyncServiceException(exp);
                return false;
            }

            return true;
        }



        private static bool _AIWebProxyFolderDeleteAttempted = false; // only once, not repeated tries
        private bool RemoveDirectoryForAIWebProxyDebugFiles()
        {
            try
            {
                if (_AIWebProxyFolderDeleteAttempted == false)
                {
                    _AIWebProxyFolderDeleteAttempted = true;
                    if (_DebugDATDir.Length == 0)
                    {
                        _DebugDATDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads) + "/" + Constants.AIWEBPROXY_DEBUG_FOLDERNAME;

                        if (Directory.Exists(_DebugDATDir) == true)
                        {
                            Directory.Delete(_DebugDATDir, true);
                        }

                        _DebugDATDir = string.Empty;
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error removing debug folder: {0}", exp.Message);
                _DebugDATDir = string.Empty;
                HandleSyncServiceException(exp);
                return false;
            }

            return true;
        }


	}
}
