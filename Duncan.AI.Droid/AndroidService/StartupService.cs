// define this guy to write downloaded files to filesystem for debugging
//#define WRITE_DOWNLOADED_FILES_TO_DEBUG_FOLDER


using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using System.IO;
using Android.Preferences;
using Android.Net;
using Duncan.AI.Droid.Managers;
using Duncan.AI.Utils;
using Duncan.AI.Droid;
using Duncan.AI.Droid.Utils.HelperManagers;
using XMLConfig;

using Duncan.AI.Droid.Utils.DataAccess;

//Android intent service to run in the background to retrieve users login 
//info from the AI host webservice and save the data to the SQLite DB
namespace Duncan.AI.Droid
{
    [Service]
    [IntentFilter(new String[] { "Duncan.AI.Droid.StartupService" })]
    class StartupService : IntentService
    {
        public StartupService()
            : base("StartupService")
        {
            _syncStartupImpl = new SyncStartupImpl();
        }

        bool _isError = false;
        string _encryptedSessionKey = "";
        string _compFiles = "";
        string _hhplatformfiles = "";

        string _uploadFileList = "";
        string _deleteFiles = "";



        long _assignedSubConfigKey = 0;
        string _confFiles = "";
        string _customerConfigKey = "";
        string _sequenceNames = "";
        private string _serialNumber;
        private readonly SyncStartupImpl _syncStartupImpl = new SyncStartupImpl();




        /// <summary>
        /// Broadcast intent callback to update progressbar on screen
        /// </summary>
        /// <param name="iProgress"></param>
        private void SendBroadcastProgressUpdate(int iProgress, string iProgressDesc)
        {
            var intentProgressUpdate = new Intent(Constants.ACTIVITY_INTENT_DISPLAY_SYNC_PROGRESS_NAME);

            intentProgressUpdate.PutExtra(Constants.ActivityIntentExtraInt_ProgressValue, iProgress);
            intentProgressUpdate.PutExtra(Constants.ActivityIntentExtraInt_ProgressDesc, iProgressDesc);

            SendBroadcast(intentProgressUpdate);
        }

        private void UpdateSyncProgressMessages(Duncan.AI.Constants.SyncStepEnummeration iSyncStep, string iSyncStepDescription)
        {

            string loStatusText = "Process " + ((int)iSyncStep).ToString() + " of " + ((int)Duncan.AI.Constants.SyncStepEnummeration.SyncStep_Complete).ToString();

            var intentSyncStepUpdate = new Intent("Duncan.AI.Droid.ListInsertedBroadcastRec");
            intentSyncStepUpdate.PutExtra("Status", loStatusText);
            SendBroadcast(intentSyncStepUpdate);

            SendBroadcastProgressUpdate((int)iSyncStep, iSyncStepDescription);
        }





        //onHanleIntent method runs on a new thread. 
        protected override void OnHandleIntent(Intent intent)
        {
            string errorMgs = "";
            try
            {
                var cm = (ConnectivityManager)GetSystemService(ConnectivityService);
                var activeNetworkInfo = cm.ActiveNetworkInfo;

                if (activeNetworkInfo != null && activeNetworkInfo.IsConnected)
                {
                    Console.WriteLine("Starting the process");

                    // reset
                    _isError = false;

                    //Sync Authenticate
                    SyncAuthenticate();

                    //Sync Begin
                    SyncBegin();


                    // 1st priority is to upload files to the host server.
                    if (!UploadAndImportFiles())
                    {
                        //// failed. Log out and exit. _SyncProgress has already been updated.
                        //SafeCloseOutCurrentUserFromDatabase(_HostEncryptedSessionKey, _HostSessionKey, ref _HostErrMsg);
                        //return;
                    }

                    // the files have been safely imported, ok to delete them locally.  
                    if (!DeleteLocalFiles())
                    {
                        //// failed. Log out and exit. _SyncProgress has already been updated.
                        //SafeCloseOutCurrentUserFromDatabase(_HostEncryptedSessionKey, _HostSessionKey, ref _HostErrMsg);
                        //return;
                    }




                    // download platform specific files - registry, APK
                    ProcessHHPlatformFiles();


                    //Download issue_ap.xml and other config files
                    ProcessConfigFiles();

                    //Download List files
                    ProcessListFiles();

                    //Download Seq files
                    ProcessSequenceFiles();

                    //Get users, decrpt the file and insert into the db
                    ProcessUserStructFile();

                    if (_isError == false)
                    {
                        ParkingSequenceADO.ResetAllPendingTickets();
                    }



                    if (!_isError)
                    {
                        Duncan.AI.Droid.DroidContext.SyncLastResultText = "Synchronization Successful.";

                        var i1E = new Intent("Duncan.AI.Droid.RefreshDB");
                        i1E.PutExtra("Status", "Success");
                        SendBroadcast(i1E);
                    }
                }
                else
                {
                    Duncan.AI.Droid.DroidContext.SyncLastResultText = "No Internet Connection";
                    var i1E = new Intent("Duncan.AI.Droid.RefreshDB");
                    i1E.PutExtra("Status", "No Internet Connection");
                    SendBroadcast(i1E);
                }
            }
            catch (Exception e)
            {
                _isError = true;
                if (string.IsNullOrEmpty(errorMgs))
                {
                    errorMgs = e.Message;
                }
                else
                {
                    errorMgs = errorMgs + "\n" + e.Message;
                }
                Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;

                var i1E = new Intent("Duncan.AI.Droid.RefreshDB");
                i1E.PutExtra("Status", "Exception in step 1");
                SendBroadcast(i1E);
                LoggingManager.LogApplicationError(e, "errorMgs: " + errorMgs, "StartupService.OnHandleIntent");
            }

        }



        private async void ProcessUserStructFile()
        {
            string errorMgs = "";
            if (!_isError)
            {
                try
                {
                    var i7 = new Intent("Duncan.AI.Droid.ListInsertedBroadcastRec");
                    i7.PutExtra("Status", "Process Step 7");
                    SendBroadcast(i7);

                    SendBroadcastProgressUpdate((int)Constants.SyncStepEnummeration.SyncStep_ProcessUserStruct, "Validating Security...");

                    string userStructureFilename = "";
                    byte[] loFileContents = _syncStartupImpl.GetUserStructFile(_serialNumber, _encryptedSessionKey,
                                                                           _encryptedSessionKey, ref errorMgs, ref userStructureFilename);
                    if (String.IsNullOrEmpty(errorMgs) == false)
                    {
                        Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;
                        throw new Exception(errorMgs);
                    }



                    if (!string.IsNullOrEmpty(userStructureFilename))
                    {


                        var structFileNameLocal =
                            Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads) +
                            "/" + userStructureFilename;
                        Console.WriteLine("struct FileName: " + structFileNameLocal);

                        if (File.Exists(structFileNameLocal))
                        {
                            File.Delete(structFileNameLocal);
                        }

#if WRITE_DOWNLOADED_FILES_TO_DEBUG_FOLDER
                        if (loFileContents != null && loFileContents.Length > 0)
                        {
                            var loFile = new FileStream(structFileNameLocal, FileMode.Create, FileAccess.Write);
                            loFile.Write(loFileContents, 0, loFileContents.Length);
                            loFile.Close();
                        }
#endif


                        byte[] unscrambledUserBytes = null;
                        if (loFileContents != null && loFileContents.Length > 0)
                        {
                            //we have the struct file, do stuff with it - decrypt it
                            unscrambledUserBytes = DecryptScruct(loFileContents, 0);
                            string result = Encoding.UTF8.GetString(unscrambledUserBytes);

                            // only for debugging
                            //Console.Write(result);  
                        }


                        if (loFileContents != null && loFileContents.Length > 0)
                        {
                            Console.WriteLine("Updating AI Filesystem FileName: " + userStructureFilename);

                            ClientFileManager.InsertOrUpdateFileData(userStructureFilename, unscrambledUserBytes.Length, DateTime.Now, DateTime.Now, unscrambledUserBytes);
                        }


                        if (loFileContents != null && loFileContents.Length > 0)
                        {
                            //now that we have it correctly, lets save it.
                            var usersFileName =
                                Android.OS.Environment.GetExternalStoragePublicDirectory(
                                    Android.OS.Environment.DirectoryDownloads) + "/" + "USERS.DAT";
                            if (File.Exists(usersFileName))
                            {
                                File.Delete(usersFileName);
                            }

#if WRITE_DOWNLOADED_FILES_TO_DEBUG_FOLDER
                            var userFile = new FileStream(usersFileName, FileMode.CreateNew, FileAccess.Write);
                            userFile.Write(unscrambledUserBytes, 0, unscrambledUserBytes.Length);
                            userFile.Close();
#endif

                            //var userDAOs = _syncStartupImpl.GetUsers(usersFileName);

                            var userDAOs = _syncStartupImpl.LoadUserDataFromRawFileData(unscrambledUserBytes);
                            var loginImpl = new LoginManager();
                            loginImpl.PopulateUserNames(userDAOs);
                        }




                    }

                    Console.WriteLine("ErrorMgs: {0}", errorMgs);
                }
                catch (Exception e)
                {
                    _isError = true;
                    if (string.IsNullOrEmpty(errorMgs))
                    {
                        errorMgs = e.Message;
                    }
                    else
                    {
                        errorMgs = errorMgs + "\n" + e.Message;
                    }
                    Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;

                    var i1E = new Intent("Duncan.AI.Droid.RefreshDB");
                    i1E.PutExtra("Status", "Error in ProcessUserStructFile");
                    SendBroadcast(i1E);
                    LoggingManager.LogApplicationError(e, "errorMgs: " + errorMgs, "ProcessUserStructFile");
                }
            }
        }

        private void ProcessSequenceFiles()
        {
            string errorMgs = "";
            if (!_isError)
            {
                try
                {
                    var i6 = new Intent("Duncan.AI.Droid.ListInsertedBroadcastRec");
                    i6.PutExtra("Status", "Step 6");
                    SendBroadcast(i6);

                    SendBroadcastProgressUpdate((int)Constants.SyncStepEnummeration.SyncStep_ProcessSequenceFiles, "Allocating Sequence Files....");


                    //_sequenceNames = "PARKING WARNING GENERALWARN";
                    string sequenceNamesTmp = _sequenceNames;
                    if (sequenceNamesTmp == null || sequenceNamesTmp.Trim().Equals(""))
                    {
                        List<IssStruct> _structs = Duncan.AI.Droid.DroidContext.XmlCfg.IssStructs;
                        foreach (var structI in _structs)
                        {
                            if (Constants.STRUCT_TYPE_CITE.Equals(structI.Type))
                            {
                                _sequenceNames = _sequenceNames + " " + structI.SequenceName;
                            }
                        }
                    }

                    string[] loSequenceNameList = _sequenceNames.Split(' ');
                    foreach (string loSequenceName in loSequenceNameList)
                    {
                        // beware of empty strings.
                        if (loSequenceName == "")
                            continue;


                        // AJW TODO this needs help to support multiple structures, more than just "Parking"


                        if (!string.IsNullOrEmpty(loSequenceName))
                        {
                            byte[] loFileContents = _syncStartupImpl.GetSequenceFile(_serialNumber, loSequenceName,
                                                                                         _encryptedSessionKey,
                                                                                         ref errorMgs);

                            if (String.IsNullOrEmpty(errorMgs) == false)
                            {
                                Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;
                                throw new Exception(errorMgs);
                            }


                            var sequenceFileNameLocal =
                                Android.OS.Environment.GetExternalStoragePublicDirectory(
                                    Android.OS.Environment.DirectoryDownloads) + "/" + loSequenceName + ".SEQ";
                            Console.WriteLine("Sequence FileName: " + sequenceFileNameLocal);


                            if (File.Exists(sequenceFileNameLocal))
                            {
                                File.Delete(sequenceFileNameLocal);
                            }

#if WRITE_DOWNLOADED_FILES_TO_DEBUG_FOLDER

                            if (loFileContents != null && loFileContents.Length > 0)
                            {
                                var loFile = new FileStream(sequenceFileNameLocal, FileMode.Create, FileAccess.Write);
                                loFile.Write(loFileContents, 0, loFileContents.Length);
                                loFile.Close();
                                //we have the sequence file, do stuff with it.
                            }
#endif

                            if (loFileContents != null && loFileContents.Length > 0)
                            {
                                string sequenceTableName = loSequenceName + ".SEQ";
                                Console.WriteLine("Updating AI Filesystem FileName: " + sequenceTableName);

                                ClientFileManager.InsertOrUpdateFileData(sequenceTableName, loFileContents.Length, DateTime.Now, DateTime.Now, loFileContents);
                            }


                            if (loFileContents != null && loFileContents.Length > 0)
                            {

                                // this needs to include clientname from XML

                                //Insert sequence numbers
                                //check to see if any rows exist. if they dont, create and populate the table.
                                bool rowsExists = ParkingSequenceADO.CheckRows(loSequenceName);
                                if (!rowsExists)
                                {
                                    var sequenceData = _syncStartupImpl.GetSequenceData(sequenceFileNameLocal, loSequenceName, loFileContents);
                                    // TicketImpl TicketImpl = new TicketImpl();
                                    // NewDataSet newDataSet = TicketImpl.GetParkingSequence("", ref errorMsg, modDeviceId);
                                    ParkingSequenceADO.CreateSequenceTable(sequenceData, loSequenceName);
                                }
                            }

                            Console.WriteLine("ErrorMgs: {0}", errorMgs);
                        }
                    }
                }
                catch (Exception e)
                {
                    _isError = true;
                    if (string.IsNullOrEmpty(errorMgs))
                    {
                        errorMgs = e.Message;
                    }
                    else
                    {
                        errorMgs = errorMgs + "\n" + e.Message;
                    }
                    Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;

                    var i1E = new Intent("Duncan.AI.Droid.RefreshDB");
                    i1E.PutExtra("Status", "Error in ProcessSequenceFiles");
                    SendBroadcast(i1E);
                    LoggingManager.LogApplicationError(e, "errorMgs: " + errorMgs, "ProcessSequenceFiles");
                }
            }
        }

        private void ProcessListFiles()
        {
            string errorMgs = "";
            if (!_isError)
            {
                try
                {
                    var i5 = new Intent("Duncan.AI.Droid.ListInsertedBroadcastRec");
                    i5.PutExtra("Status", "Step 5" );
                    SendBroadcast(i5);

                    SendBroadcastProgressUpdate((int)Constants.SyncStepEnummeration.SyncStep_ProcessListFiles, "Refreshing List Files....");


                    bool loAtLeastOneFileUpdated = false;

                    string compositeFiles = _syncStartupImpl.DownloadListFiles(_serialNumber, _compFiles,
                                                                               (int) _assignedSubConfigKey, _encryptedSessionKey,
                                                                               ref errorMgs);
                    if (String.IsNullOrEmpty(errorMgs) == false)
                    {
                        Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;
                        throw new Exception(errorMgs);
                    }


                    if (!string.IsNullOrEmpty(compositeFiles))
                    {
                        var loCompositesWData = (FileSet)Helper.DeSerializeObject(compositeFiles, typeof(FileSet));
                        foreach (FileInformation loFileInfo in loCompositesWData.FileList)
                        {
                            Console.WriteLine("List FileName: " + loFileInfo.FileName);
                            var localListFileName =
                                    Android.OS.Environment.GetExternalStoragePublicDirectory(
                                        Android.OS.Environment.DirectoryDownloads) +
                                "/" + loFileInfo.FileName;

                            if (File.Exists(localListFileName))
                            {
                                File.Delete(localListFileName);
                            }


#if WRITE_DOWNLOADED_FILES_TO_DEBUG_FOLDER
                            if (Helper.SafeGetFileInfoDataSize(loFileInfo) > 0)
                            {
                                Console.WriteLine("List FileName Data: " + loFileInfo.FileName);
                                var loFile = new FileStream(localListFileName, FileMode.Create, FileAccess.Write);
                                loFile.Write(loFileInfo.Data, 0, loFileInfo.Data.Length);
                                loFile.Close();
                                loFile = null;
                                //File.SetLastWriteTime(filePath, loFileInfo.FileDate.ToUniversalTime());

                                loAtLeastOneFileUpdated = true;
                            }
#endif

                            if (Helper.SafeGetFileInfoDataSize(loFileInfo) > 0)
                            {
                                Console.WriteLine("Updating AI Filesystem FileName: " + loFileInfo.HHFileName);

                                ClientFileManager.InsertOrUpdateFileData(loFileInfo.HHFileName, loFileInfo.Data.Length, loFileInfo.FileDate, loFileInfo.FileDate, loFileInfo.Data);

                                loAtLeastOneFileUpdated = true;
                            }


                        }




                        //// test - read them all back in
                        //foreach (FileInformation loFileInfo in loCompositesWData.FileList)
                        //{
                        //    if (Helper.SafeGetFileInfoDataSize(loFileInfo) > 0)
                        //    {
                        //        Console.WriteLine("Confirming AI Filesystem FileName: " + loFileInfo.HHFileName);

                        //        AIFileSystemDAO oneFile = ClientFileManager.GetDataForFileSystemFile( loFileInfo.HHFileName );
                        //        if (oneFile != null)
                        //        {
                        //            Console.WriteLine("Confirming AI Filesystem Check: " + oneFile.FILENAME );
                        //            Console.WriteLine("Confirming AI Filesystem Size: " + oneFile.FILESIZE.ToString() );
                        //            if (oneFile.FILEDATA != null)
                        //            {
                        //                Console.WriteLine("Confirming AI Filesystem Length: " + oneFile.FILEDATA.Length);
                        //            }
                        //            else
                        //            {
                        //                Console.WriteLine("Confirming AI Filesystem Check ERROR: byte[] is NULL");
                        //            }
                        //        }



                        //    }
                        //}
                    }


                    Console.WriteLine("ErrorMgs: {0}", errorMgs);

                    var listSupport = new ListSupport();
                    listSupport.InsertListData();

                    // if windows explorer is connected, this will force it to refresh
                    if (loAtLeastOneFileUpdated == true)
                    {
                        //MediaScannerConnection.scanFile(this, new String[] { file.toString() }, null, null);
                    }

                }
                catch (Exception e)
                {
                    _isError = true;
                    if (string.IsNullOrEmpty(errorMgs))
                    {
                        errorMgs = e.Message;
                    }
                    else
                    {
                        errorMgs = errorMgs + "\n" + e.Message;
                    }
                    Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;

                    var i1e = new Intent("Duncan.AI.Droid.RefreshDB");
                    i1e.PutExtra("Status", "Error in step 5");
                    SendBroadcast(i1e);
                    LoggingManager.LogApplicationError(e, "errorMgs: " + errorMgs, "ProcessListFiles");
                }
            }
        }




        /// <summary>
        /// Helper routine for DoSynchronization().  
        /// Uploads all the file in _UploadFileList to the host.
        /// Updates _SyncProgress stats as files are uploaded.
        /// In the event of an error, _SyncProgress members CurrentSyncStep, ErrorCode, and ErrorText are all updated.  Caller need only
        /// end the current sync session.
        /// Returns true on success, false on failure.
        /// </summary>
        /// <returns></returns>
        protected bool UploadAndImportFiles()
        {

            // TODO - flesh out and activate
            return true;


            string errorMgs = "";
            if (!_isError)
            {
                try
                {


                    UpdateSyncProgressMessages(Constants.SyncStepEnummeration.SyncStep_UploadFiles, "Uploading and Importing....");

                    var loUploadFileList = (FileSet)Helper.DeSerializeObject(_uploadFileList, typeof(FileSet));

#if _implemented_
                    foreach (FileInformation loFileInfo in loUploadFileList.FileList)
                    {
                        // don't waste time w/ empty files
                        if (loFileInfo.Length > 0)
                        {
                            //_SyncProgress.CurrentFileSize = (int)loFileInfo.Length;
                            //_SyncProgress.CurrentFileName = Path.GetFileName(loFileInfo.FileName);
                            //WriteSyncProgressToLocalLog();

                            byte[] loFileData;
                            try
                            {
                                FileStream loFile = new FileStream(loFileInfo.FileName, FileMode.Open);
                                loFileData = new byte[loFile.Length];
                                loFile.Read(loFileData, 0, loFileData.Length);
                                loFile.Close();
                            }
                            catch (Exception E)
                            {
                                //_SyncProgress.CurrentSyncStep = ESyncSteps.SyncStep_Failed;
                                //_SyncProgress.ErrorCode = BaseSyncSupport.SYNCSUPPORT_ERROR_LOCAL_FILE_READ_FAILED;
                                //_SyncProgress.ErrorText = "Failed reading " + loFileInfo.FileName + ": " + E.Message;
                                //WriteSyncProgressToLocalLog();
                                return false;
                            }


                            // read it in, now send it up.
                            try
                            {
                                _CreateFileInCommSessionDelegate(_SessionName, Path.GetFileName(loFileInfo.FileName), loFileInfo.FileDate, loFileData, _HostEncryptedSessionKey, ref _HostErrMsg);
                                // Write to COMMLOG to record file download
                                StringBuilder loLogMsg = new StringBuilder("Uploaded file to server");
                                loLogMsg.Append(" | Records: 0");
                                loLogMsg.Append(" | Size: " + loFileInfo.Length);
                                loLogMsg.Append(" | Touch: " + loFileInfo.FileDate.ToString("yyyyddMM HHmmss"));
                                loLogMsg.Append(" | Attrs: 0");
                                LogTransmittedFile(loFileInfo, true);
                            }
                            catch (Exception E)
                            {
                                _SyncProgress.CurrentSyncStep = ESyncSteps.SyncStep_Failed;
                                _SyncProgress.ErrorCode = BaseSyncSupport.SYNCSUPPORT_ERROR_WEB_SERVICE_ERROR;
                                _SyncProgress.ErrorText = "Exception invoking CreateFileInCommSession with file " + loFileInfo.FileName + ": " + E.Message;
                                WriteSyncProgressToLocalLog();
                                return false;
                            }


                            if (_HostErrMsg != "")
                            {
                                _SyncProgress.CurrentSyncStep = ESyncSteps.SyncStep_Failed;
                                _SyncProgress.ErrorCode = BaseSyncSupport.SYNCSUPPORT_ERROR_WEB_SERVICE_ERROR;
                                _SyncProgress.ErrorText = "CreateFileInCommSession with file " + loFileInfo.FileName + " returned error: " + _HostErrMsg;
                                WriteSyncProgressToLocalLog();
                                return false;
                            }
                        }

                        //_SyncProgress.TotalBytesUploadedSoFar += (int)loFileInfo.Length;
                        //_SyncProgress.TotalFilesUploadedSoFar++;
                        //if ((_SyncProgress.TotalUploadByteCount + _SyncProgress.TotalDownloadByteCount) > 0)
                        //    _SyncProgress.PercentageComplete = (100 * SyncProgress.TotalBytesUploadedSoFar) / (_SyncProgress.TotalUploadByteCount + _SyncProgress.TotalDownloadByteCount);
                    }

                    // done w/ the last file
                    _SyncProgress.CurrentFileSize = 0;
                    _SyncProgress.CurrentFileName = "";

                    // all the files have been uploaded, instruct the host to import them.
                    // now import the uploaded files
                    _SyncProgress.CurrentSyncStep = ESyncSteps.SyncStep_ProcessUploadedFiles;
                    WriteSyncProgressToLocalLog();
                    try
                    {
                        _ImportCommSessionDataDelegate(_SessionName, _SerialNo, _HostEncryptedSessionKey, ref _HostErrMsg);
                    }
                    catch (Exception E)
                    {
                        _SyncProgress.CurrentSyncStep = ESyncSteps.SyncStep_Failed;
                        _SyncProgress.ErrorCode = BaseSyncSupport.SYNCSUPPORT_ERROR_WEB_SERVICE_ERROR;
                        _SyncProgress.ErrorText = "Exception invoking ImportCommSessionData:" + E.Message;
                        WriteSyncProgressToLocalLog();
                        return false;
                    }

                    if (_HostErrMsg != "")
                    {
                        _SyncProgress.CurrentSyncStep = ESyncSteps.SyncStep_Failed;
                        _SyncProgress.ErrorCode = BaseSyncSupport.SYNCSUPPORT_ERROR_WEB_SERVICE_ERROR;
                        _SyncProgress.ErrorText = "ImportCommSessionData returned error:" + _HostErrMsg;
                        WriteSyncProgressToLocalLog();
                        return false;
                    }
#endif

                }
                catch (Exception e)
                {
                    _isError = true;
                    if (string.IsNullOrEmpty(errorMgs))
                    {
                        errorMgs = e.Message;
                    }
                    else
                    {
                        errorMgs = errorMgs + "\n" + e.Message;
                    }
                    Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;


                    var i2_5e = new Intent("Duncan.AI.Droid.RefreshDB");
                    i2_5e.PutExtra("Status", "Error in step 2.1");
                    SendBroadcast(i2_5e);
                    LoggingManager.LogApplicationError(e, "errorMgs: " + errorMgs, "UploadAndImportFiles");
                }
            }


            return true; // made it through w/ no errors.
        }


        /// Helper routine for DoSynchronization().  
        /// Deletes all the local files named in _DeleteFileList.
        /// In the event of an error, _SyncProgress members CurrentSyncStep, ErrorCode, and ErrorText are all updated.  Caller need only
        /// end the current sync session.
        /// Returns true on success, false on failure.
        private bool DeleteLocalFiles()
        {

            // make sure the table exists
            (new DatabaseManager()).CreateAutoISSUEFileSystemTable();

            // get rid of all of the files therein
            (new DatabaseManager()).EmptyAutoISSUEFileSystemTable();





            // TODO - flesh out and activate
            return true;


            string errorMgs = "";
            if (!_isError)
            {
                try
                {
                    UpdateSyncProgressMessages(Constants.SyncStepEnummeration.SyncStep_RemoveLocalFiles, "Removing Local files....");

                    if (!string.IsNullOrEmpty(_deleteFiles))
                    {


                        var _DeleteFileList = (FileSet)Helper.DeSerializeObject(_deleteFiles, typeof(FileSet));

                        foreach (FileInformation loFileInfo in _DeleteFileList.FileList)
                        {
                            try
                            {
                                File.Delete(loFileInfo.FileName);
                            }
                            catch (Exception E)
                            {
                                throw new Exception("Delete failed. " + Path.GetFileName(loFileInfo.FileName) + " " + errorMgs);

                                //_SyncProgress.CurrentSyncStep = ESyncSteps.SyncStep_Failed;
                                //_SyncProgress.ErrorCode = BaseSyncSupport.SYNCSUPPORT_ERROR_LOCAL_FILE_DELETE_FAILED;
                                //_SyncProgress.ErrorText = "Failed deleting " + loFileInfo.FileName + ": " + E.Message;
                                //WriteSyncProgressToLocalLog();
                                return false;
                            }
                        }
                    }

                    return true;
                }
                catch (Exception e)
                {
                    _isError = true;
                    if (string.IsNullOrEmpty(errorMgs))
                    {
                        errorMgs = e.Message;
                    }
                    else
                    {
                        errorMgs = errorMgs + "\n" + e.Message;
                    }
                    Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;

                    var i2_5e = new Intent("Duncan.AI.Droid.RefreshDB");
                    i2_5e.PutExtra("Status", "Error in step 2.2");
                    SendBroadcast(i2_5e);
                    LoggingManager.LogApplicationError(e, "errorMgs: " + errorMgs, "DeleteLocalFiles");

                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Get HHPlatform specific files - REGISTRY, APK, ect
        /// </summary>
        private void ProcessHHPlatformFiles()
        {
            string errorMgs = "";
            if (!_isError)
            {
                try
                {
                    var i2_5 = new Intent("Duncan.AI.Droid.ListInsertedBroadcastRec");
                    i2_5.PutExtra("Status", "Process 2.5 of 7");
                    SendBroadcast(i2_5);

                    SendBroadcastProgressUpdate((int)Constants.SyncStepEnummeration.SyncStep_GetPlatformfiles, "Verifying Platform files....");


                    if (!string.IsNullOrEmpty(_hhplatformfiles))
                    {


                        var loHHPlatformFileset = (FileSet)Helper.DeSerializeObject(_hhplatformfiles, typeof(FileSet));
                        foreach (FileInformation loFileInfo in loHHPlatformFileset.FileList)
                        {

                            Console.WriteLine("HHPlatform FileName: " + loFileInfo.FileName + " Exists on Host:" + loFileInfo.Exists.ToString());

                            // the file may be defined but not be present - check first before asking
                            if (loFileInfo.Exists == true)
                            {

                                // lets start with the assumption that we'll download this file, but we'll check for some reasons not to
                                bool loDownloadThisFile = true;


                                // when we save to our local storage, try use the file name we are given to use
                                string loLocalFileName = loFileInfo.FileName;
                                if (string.IsNullOrEmpty(loFileInfo.HHFileName) == false)
                                {
                                    loLocalFileName = loFileInfo.HHFileName;
                                }

                                var localHHFileNameComplete = Android.OS.Environment.GetExternalStoragePublicDirectory
                                                                  (Android.OS.Environment.DirectoryDownloads) + "/" +
                                                                  loLocalFileName;


                                if (File.Exists(localHHFileNameComplete) == true)
                                {
                                    // is the time stamp older that what the server has?
                                    DateTime loLocalHHFileNameTimeStampUTC = Directory.GetCreationTimeUtc(localHHFileNameComplete);



                                    string loFileExt = Path.GetExtension(localHHFileNameComplete).ToUpper();
                                    bool loFileIsAPK = loFileExt.Equals("APK");

                                    // this isn't valid until we are setting local filetime after download
                                    loDownloadThisFile = (loLocalHHFileNameTimeStampUTC < loFileInfo.FileDate);

                                    // KLUDGE demo - skip APK files, re-donwload every thing else always 
                                    loDownloadThisFile = (loFileIsAPK == false);
                                }





                                if (loDownloadThisFile == true)
                                {
                                    // it exists, but its older - get the updated one
                                    byte[] loFileContents = _syncStartupImpl.DownloadHHPlatformFile(_serialNumber,
                                                                                                  _customerConfigKey,
                                                                                                  Path.GetFileName(
                                                                                                      loFileInfo.FileName),
                                                                                                  _encryptedSessionKey,
                                                                                                  ref errorMgs);
                                    if (String.IsNullOrEmpty(errorMgs) == false)
                                    {
                                        Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;
                                        throw new Exception(errorMgs);
                                    }


                                    // any problems?
                                    if ((loFileContents != null) && (errorMgs.Length == 0))
                                    {

                                        if (File.Exists(localHHFileNameComplete))
                                        {
                                            File.Delete(localHHFileNameComplete);
                                        }


#if WRITE_DOWNLOADED_FILES_TO_DEBUG_FOLDER
                                        if (loFileContents.Length > 0)
                                        {
                                            var loFile = new FileStream(localHHFileNameComplete, FileMode.Create, FileAccess.Write);
                                            loFile.Write(loFileContents, 0, loFileContents.Length);
                                            loFile.Close();
                                            loFile = null;
                                        }
                                        else
                                        {
                                            // shoud we erase the old version local file then?
                                        }
#endif

                                        if (loFileContents.Length > 0)
                                        {
                                            Console.WriteLine("Updating AI Filesystem FileName: " + loFileInfo.HHFileName);
                                            //ClientFileManager.InsertOrUpdateFileData(loFileInfo.HHFileName, loFileContents.Length, DateTime.Now, DateTime.Now, loFileContents);
                                            ClientFileManager.InsertOrUpdateFileData(loFileInfo.HHFileName, loFileContents.Length, loFileInfo.FileDate, loFileInfo.FileDate, loFileContents);
                                        }
                                        else
                                        {
                                            // file is empty from host,  should be empty here
                                            Console.WriteLine("Updating AI Filesystem FileName: " + loFileInfo.HHFileName + " (Empty File)");
                                            ClientFileManager.InsertOrUpdateFileData(loFileInfo.HHFileName, 0, new DateTime(1970, 1, 1), new DateTime(1970, 1, 1), new byte[] { });
                                        }


                                    }
                                    else
                                    {
                                        // download failed
                                        throw new Exception("Download failed. " + Path.GetFileName(loFileInfo.FileName) + " " + errorMgs);
                                    }

                                }
                                else
                                {
                                    LoggingManager.LogApplicationError(null, "ProcessHHPlatformFiles - Skipping based on filedate: ", localHHFileNameComplete);
                                }


                            }

                        }


                    }
                }
                catch (Exception e)
                {
                    _isError = true;
                    if (string.IsNullOrEmpty(errorMgs))
                    {
                        errorMgs = e.Message;
                    }
                    else
                    {
                        errorMgs = errorMgs + "\n" + e.Message;
                    }
                    Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;
                    var i2_5e = new Intent("Duncan.AI.Droid.RefreshDB");
                    i2_5e.PutExtra("Status", "Error in ProcessHHPlatformFiles");
                    SendBroadcast(i2_5e);
                    LoggingManager.LogApplicationError(e, "errorMgs: " + errorMgs, "ProcessHHPlatformFiles");
                }
            }

        }


        /// <summary>
        /// /
        /// </summary>

        private void ProcessConfigFiles()
        {
            string errorMgs = "";
            List<XMLConfig.TableDef> tableDefList = null;
            if (!_isError)
            {
                try
                {
                    var i3 = new Intent("Duncan.AI.Droid.ListInsertedBroadcastRec");
                    i3.PutExtra("Status", "Process 3 of 7");
                    SendBroadcast(i3);

                    SendBroadcastProgressUpdate((int)Constants.SyncStepEnummeration.SyncStep_GetConfiguration, "Preparing Configuration...");

                    bool IssueXMLAlreadyDefined = false; // TODO - we will force re-generaton XML at the sync finish, even when a new XML isn't downloaded - new lists etc could be
                    bool recievedIssueXML = false;

                    if (!string.IsNullOrEmpty(_confFiles))
                        {


                        var loconfigWData = (FileSet)Helper.DeSerializeObject(_confFiles, typeof(FileSet));
                        foreach (FileInformation loFileInfo in loconfigWData.FileList)
                        {
                            Console.WriteLine("Config FileName: " + loFileInfo.FileName);

                            byte[] loFileContents = _syncStartupImpl.DownloadCongfigFiles(_serialNumber,
                                                                                          _customerConfigKey,
                                                                                          Path.GetFileName(
                                                                                              loFileInfo.FileName),
                                                                                          _encryptedSessionKey,
                                                                                          ref errorMgs);

                            if (String.IsNullOrEmpty(errorMgs) == false)
                            {
                                Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;
                                throw new Exception(errorMgs);
                            }


                            var localConfigFileName = Android.OS.Environment.GetExternalStoragePublicDirectory
                                                              (Android.OS.Environment.DirectoryDownloads) + "/" +
                                                          loFileInfo.FileName;
                            if (File.Exists(localConfigFileName))
                            {
                                File.Delete(localConfigFileName);
                            }


#if WRITE_DOWNLOADED_FILES_TO_DEBUG_FOLDER
                            if (loFileContents.Length > 0)
                            {
                                var loFile = new FileStream(localConfigFileName, FileMode.Create, FileAccess.Write);
                                loFile.Write(loFileContents, 0, loFileContents.Length);
                                loFile.Close();
                                loFile = null;
                            }
#endif


                            // put file into the database
                            if (loFileContents.Length > 0)
                            {
                                Console.WriteLine("Updating AI Filesystem FileName: " + loFileInfo.HHFileName);
                                //ClientFileManager.InsertOrUpdateFileData(loFileInfo.HHFileName, loFileContents.Length, DateTime.Now, DateTime.Now, loFileContents);
                                ClientFileManager.InsertOrUpdateFileData(loFileInfo.HHFileName, loFileContents.Length, loFileInfo.FileDate, loFileInfo.FileDate, loFileContents);
                            }
                            else
                            {
                                // file is empty from host,  should be empty here
                                Console.WriteLine("Updating AI Filesystem FileName: " + loFileInfo.HHFileName + " (Empty File)" );
                                ClientFileManager.InsertOrUpdateFileData(loFileInfo.HHFileName, 0, new DateTime( 1970, 1, 1 ), new DateTime( 1970, 1, 1 ), new byte[]{} );
                            }

                            // did we get our layout file?
                            if (loFileInfo.FileName.ToUpper().Equals(Constants.ISSUE_AP_XML_FILENAME))
                            {
                                recievedIssueXML = true;
                            }



                        }

                    }

                    // did we get the XML?
                    // TODO - if we already have same rev, we don't need to dl again
                    if ((string.IsNullOrEmpty( errorMgs) == true) && (recievedIssueXML == false))
                    {
                        errorMgs = "No Issue_Ap.XML file from the server.";
                        var noXmlIntent = new Intent("Duncan.AI.Droid.RefreshDB");
                        noXmlIntent.PutExtra("Status", "No Issue_Ap.XML file from the server.");
                        SendBroadcast(noXmlIntent);
                        _isError = true;
                        Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;
                        return;
                    }

                    // wipe the existing XML 
                    Duncan.AI.Droid.DroidContext.XmlCfg = null;
                    // force reload with the info just downloaded
                    Duncan.AI.Droid.DroidContext.XmlCfgLoadFromInternalFilesystem();

                    // let's use it
                    tableDefList = Duncan.AI.Droid.DroidContext.XmlCfg.TableDefs;
                }
                catch (Exception e)
                {
                    _isError = true;
                    if (string.IsNullOrEmpty(errorMgs))
                    {
                        errorMgs = e.Message;
                    }
                    else
                    {
                        errorMgs = errorMgs + "\n" + e.Message;
                    }
                    Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;

                    var i1e = new Intent("Duncan.AI.Droid.RefreshDB");
                    i1e.PutExtra("Status", "Error in ProcessConfigFiles");
                    SendBroadcast(i1e);
                    LoggingManager.LogApplicationError(e, "errorMgs: " + errorMgs, "ProcessConfigFiles");
                }
            }

            //Create table and Set revision shared preferences
            if (!_isError)
            {
                try
                {
                    var i4 = new Intent("Duncan.AI.Droid.ListInsertedBroadcastRec");
                    i4.PutExtra("Status", "Process 4 of 7");
                    SendBroadcast(i4);

                    SendBroadcastProgressUpdate((int)Constants.SyncStepEnummeration.SyncStep_LoadPreferences, "Loading Preferences...");


                    //Create panel tables
                    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                    ISharedPreferencesEditor editor = prefs.Edit();

                    if (tableDefList != null)
                    {
                        foreach (TableDef tableDef in tableDefList)
                        {
                            Console.WriteLine("Table Name: {0}", tableDef.Name);
                            string isGps = CommonADO.CreateTable(tableDef);

                            //Store struct revisions
                            if (Constants.STRUCT_NAME_ACTIVITYLOG == tableDef.Name)
                            {

                                // AJW - KLUDGE DEMO - disable GPS threads to test stability
                                editor.PutString(Constants.IS_GPS, isGps);

                            }

                            editor.PutString(tableDef.Name + Constants.LABEL_REVISION, tableDef.Revision.ToString());
                           
                            //else if (Constants.USERSTRUCT_TABLE == tableDef.Name)
                            //{
                            //    var colNamesStrBldr = new StringBuilder();
                            //    int noOfColms = 0;
                            //    foreach (TableDefField tTableFldDef in tableDef.TableDefFields)
                            //    {
                            //        if (noOfColms != 0)
                            //            colNamesStrBldr.Append(", ");
                            //        noOfColms++;
                            //        colNamesStrBldr.Append(tTableFldDef.Name);
                            //    }
                            //    _userStructColumns = colNamesStrBldr.ToString();
                            //    _noOfColms = noOfColms;
                            //}
                        }
                    }
                    editor.Apply();
                }
                catch (Exception e)
                {
                    _isError = true;
                    if (string.IsNullOrEmpty(errorMgs))
                    {
                        errorMgs = e.Message;
                    }
                    else
                    {
                        errorMgs = errorMgs + "\n" + e.Message;
                    }
                    Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;

                    var i1e = new Intent("Duncan.AI.Droid.RefreshDB");
                    i1e.PutExtra("Status", "Error in ProcessConfigFiles2");
                    SendBroadcast(i1e);
                    LoggingManager.LogApplicationError(e, "errorMgs: " + errorMgs, "ProcessConfigFiles2");
                }
            }
        }

        private void SyncBegin()
        {string errorMgs = "";
            if (!_isError)
            {
                try
                {

                    var i2 = new Intent("Duncan.AI.Droid.ListInsertedBroadcastRec");
                    i2.PutExtra("Status", "Process 2 of 7");
                    SendBroadcast(i2);
                    
                    SendBroadcastProgressUpdate((int)Constants.SyncStepEnummeration.SyncStep_StartSync, "Initiating Synchronization....");


                    //TODO: Hard coding params
                    
                    string oSessionName = "";
                    string serialNumberStatus = "";


                    // get the current system file info timestamps and pass along so we only download what is new
                    FileSet loAIFileSystemListing = ClientFileManager.GetFileSystemListing();



                    // soon...
                    //_syncStartupImpl.SyncBegin(_encryptedSessionKey, _serialNumber, Helper.SerializeObject(loAIFileSystemListing),
                    //                           ref errorMgs, out oSessionName, out _hhplatformfiles, out _compFiles,
                    //                           out _confFiles, out _uploadFileList, out _deleteFiles, out _assignedSubConfigKey,
                    //                           out _customerConfigKey, out serialNumberStatus, out _sequenceNames);


                    _syncStartupImpl.SyncBegin(_encryptedSessionKey, _serialNumber, Helper.SerializeObject(new FileSet()),
                                               ref errorMgs, out oSessionName, out _hhplatformfiles, out _compFiles,
                                               out _confFiles, out _uploadFileList, out _deleteFiles, out _assignedSubConfigKey,
                                               out _customerConfigKey, out serialNumberStatus, out _sequenceNames);

                  
                    if (String.IsNullOrEmpty(errorMgs) == false)
                    {
                        Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;
                        throw new Exception(errorMgs);
                    }

                }
                catch (Exception e)
                {
                    _isError = true;
                    if (string.IsNullOrEmpty(errorMgs))
                    {
                        errorMgs = e.Message;
                    }
                    else
                    {
                        errorMgs = errorMgs + "\n" + e.Message;
                    }
                    Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;

                    var i1E = new Intent("Duncan.AI.Droid.RefreshDB");
                    i1E.PutExtra("Status", "Error in SyncBegin");
                    SendBroadcast(i1E);
                    LoggingManager.LogApplicationError(e, "errorMgs: " + errorMgs, "SyncBegin");
                }
            }
        }

        private void SyncAuthenticate()
        {
            string errorMgs = "";
            try
            {
                var i1 = new Intent("Duncan.AI.Droid.ListInsertedBroadcastRec");
                i1.PutExtra("Status", "Process 1 of 7");
                SendBroadcast(i1);

                SendBroadcastProgressUpdate( (int)Constants.SyncStepEnummeration.SyncStep_Authenticate, "Authenticating....");


                var syncStartupImpl = new SyncStartupImpl();

                _serialNumber = Helper.GetDeviceUniqueSerialNumber();

                string loOfficerName = "BEI";
                string loOfficerPwd = "AC";

                // no info available?
                if (Constants.deviceModelAsCategorized.Equals(Constants.deviceModelCategorized_UnknownDevice) == true)
                {
                    // it won't be accepted by host, but still send up the build info - this will help us with identifying new devices
                    loOfficerName = Constants.deviceModelBuildInfoFromOS;
                    loOfficerPwd = "XX";  // could send more info in here
                }


               
                string sessionKey = "";
                string serialNumberStatus = "";
                string availableSerialNumbers = "";
                _encryptedSessionKey = syncStartupImpl.SyncAuthenticate(loOfficerName, loOfficerPwd, _serialNumber, ref errorMgs, out sessionKey,
                                                                        out _assignedSubConfigKey, out _customerConfigKey,
                                                                        out serialNumberStatus, out availableSerialNumbers);


                if (errorMgs.Length > 0)
                {
                    Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;

                    throw new Exception(errorMgs);
                }

                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.Clear();
                editor.Commit();
                editor.PutString(Constants.SESSION_KEY, sessionKey);
                editor.PutString(Constants.ENCRYPTED_SESSION_KEY, _encryptedSessionKey);
                editor.Apply();
                Console.WriteLine("ErrorMgs: {0}", errorMgs);
            }
            catch (Exception e)
            {
                _isError = true;
                if (string.IsNullOrEmpty(errorMgs))
                {
                    errorMgs = e.Message;
                }
                else
                {
                    errorMgs = errorMgs + "\n" + e.Message;
                }
                Duncan.AI.Droid.DroidContext.SyncLastResultText = errorMgs;


                var i1E = new Intent("Duncan.AI.Droid.RefreshDB");
                i1E.PutExtra("Status", "Error in SyncAuthenticate");
                SendBroadcast(i1E);
                LoggingManager.LogApplicationError(e, "errorMgs: " + errorMgs, "SyncAuthenticate");

                var i2E = new Intent("Duncan.AI.Droid.RefreshDB");
                i2E.PutExtra("Status", errorMgs);
                SendBroadcast(i2E);



            }
        }

        //helper method to decrypt the struct file
        private const string CnScrambleEncryptionpassword = "ANYTHINGBOX";
        private const string CnScrambleEncryptedfileprefix = "©©©";
        public static byte[] DecryptScruct(byte[] structBytes, int pBufferStartFileOffset)
        {
            byte[] loPrefixBuffer = Encoding.ASCII.GetBytes(CnScrambleEncryptedfileprefix);
            // If this is encrypted, we're gonna see ©©©
            // This is just a prefix, not part of the data
            // so we need to remove it before we unencrypt data.
            bool loStripPrefix = false;
            if (structBytes.GetLength(0) > loPrefixBuffer.GetLength(0))
            {
                loStripPrefix =
                    (
                      (structBytes[0] == loPrefixBuffer[0]) &&
                      (structBytes[1] == loPrefixBuffer[1]) &&
                      (structBytes[2] == loPrefixBuffer[2])
                    );
            }

            var loWorkingBuffer = new byte[structBytes.GetLength(0)];
            structBytes.CopyTo(loWorkingBuffer, loStripPrefix ? 3 : 0);

            byte[] keyBuffer = Encoding.ASCII.GetBytes(CnScrambleEncryptionpassword);

            int keyLen = keyBuffer.GetLength(0);
            int bufLen = structBytes.GetLength(0);

            var resultBuffer = new byte[bufLen];
            if (((keyLen == 0) || (bufLen == 0)))
                return resultBuffer;

            // start at the first char of the buffer
            int bufPos = 0;

            // start the unscramble at the correct relative key position for the buffer offest
            int keyPos = keyLen - (pBufferStartFileOffset % keyLen);
            while (bufPos < bufLen)
            {
                if (keyPos == 0)
                    keyPos = keyLen;
                if (structBytes[bufPos] != keyBuffer[keyPos - 1])
                    resultBuffer[bufPos] = (byte)(structBytes[bufPos] ^ keyBuffer[keyPos - 1]);
                bufPos++;
                keyPos--;
            }
            return resultBuffer;
        }

    }
}

