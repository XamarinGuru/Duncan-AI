using System;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Duncan.AI.Droid.Utils;
using Duncan.AI.Droid.Utils.HelperManagers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Java.IO;
using Reino.ClientConfig;


namespace Duncan.AI.Droid
{
    [Service]
    [IntentFilter(new String[] { "Duncan.AI.Droid.FileCleaningUpService" })]
    class FileCleaningUpService : IntentService
    {

        private long fOldTime_msec = 0;
        private long fRefTime = 0;

        public FileCleaningUpService()
            : base("FileCleaningUpService")
        {
        }

        private void HandleExceptions(Exception e)
        {
            LoggingManager.LogApplicationError(e, "FileCleaningUpService Exception", e.TargetSite.Name);
            ErrorHandling.ReportExceptionWithConfirmationDlg(e.Message);
        }



        private void RemoveOldRecordsAlreadyUploaded()
        {
            // go through the database structs and remove all records that are aged and have been successfully uploaded
            try
            {
                List<XMLConfig.IssStruct> structs = DroidContext.XmlCfg.IssStructs;
                foreach (var structI in structs)
                {
                    CommonADO.DeleteAgedTableRows(structI.Name, structI.Type, fRefTime, fOldTime_msec );
                }
            }
            catch (Exception e)
            {
                Log.Error("FileCleaningUpService Exception", e.Source);
                HandleExceptions(e);
            }

        }


        //onHanleIntent method runs on a new thread. 
        protected override void OnHandleIntent(Intent intent)
        {
            //If the globale registry object is not exist, then return now
            if (TTRegistry.glRegistry == null) return;
 
            try
            {
                //Get the file old days to compare with
                //fOldTime_msec = (long)TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                //                                                                    TTRegistry.regCLEANINGUP_SERVICE_FILE_OLD_DAYS,
                //                                                                    TTRegistry.regCLEANINGUP_SERVICE_FILE_OLD_DAYS_DEFAULT);
                //fOldTime_msec = fOldTime_msec * 24 * 60 * 60 * 1000;  //msec


                //Get the file age in hours to compare with
                fOldTime_msec = (long)TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                    TTRegistry.regCLEANINGUP_SERVICE_FILE_OLD_HOURS,
                                                                                    TTRegistry.regCLEANINGUP_SERVICE_FILE_OLD_HOURS_DEFAULT);
                fOldTime_msec = fOldTime_msec * 60 * 60 * 1000;  //msec



                var loRefDt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                fRefTime = ((long)(DateTime.UtcNow - loRefDt).TotalMilliseconds);


                // first we will cleanse records from the database
                // TODO - this should generate the list of attachments to delete
                // right now, we risk deleting attachments before they are uploaded, if they get to old
                RemoveOldRecordsAlreadyUploaded();



                //Get list of file types we will clean up
                string loFileTypes = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP,
                                                                            TTRegistry.regCLEANINGUP_SERVICE_FILETYPES,
                                                                            TTRegistry.regCLEANINGUP_SERVICE_FILETYPES_DEFAULT);
                if (string.IsNullOrEmpty(loFileTypes))
                {
                    return; //No files to remove, return now.
                }

                string[] loTypesArray = loFileTypes.Split(';');
                if (loTypesArray.Length <= 0) return; //nothing to do

                //Get list of directories will be cleaned up
                string loDirs = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP,
                                                                       TTRegistry.regCLEANINGUP_SERVICE_DIRECTORIES,
                                                                       TTRegistry.regCLEANINGUP_SERVICE_DIRECTORIES_DEFAULT);
                if (string.IsNullOrEmpty(loDirs))
                {
                    return; //No dirs to be cleaned up, return now.
                }


                string[] loDirsArray = loDirs.Split(';');
                if (loDirsArray.Length <= 0) return; //nothing to do

                //Go through all file types and delete all from the registered folders
                foreach (string loDir in loDirsArray)
                {
                    string loAndroidDir = GetAndroidStandardDirectory(loDir);                    
                    if (string.IsNullOrEmpty(loAndroidDir)) continue;

                    //As special case, the picture dir needs to be extended to our own dir we create
                    Java.IO.File loDirFullPath;
                    if (loAndroidDir.Contains("Pictures"))
                    {
                        loDirFullPath = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(loAndroidDir), Constants.MULTIMEDIA_FOLDERNAME);
                    }
                    else
                    {
                        loDirFullPath = Android.OS.Environment.GetExternalStoragePublicDirectory(loAndroidDir);
                    }
                    foreach (string loFileType in loTypesArray)
                    {
                       
                        string[] loFiles = System.IO.Directory.GetFiles(loDirFullPath.ToString(), "*." + loFileType);
                        if (loFiles == null || loFiles.Length <= 0) continue;
                        foreach (string loFile in loFiles)
                        {
                            //Check if we should check the file old or not
                            if (fOldTime_msec > 0)
                            {
                                //Delete the file if it is old enough
                                if (IsFileOld(loFile)) System.IO.File.Delete(loFile);
                            }
                            else
                            {
                                System.IO.File.Delete(loFile);
                            }
                        }
                    }
                }                                                   
            }
            catch (Exception e)
            {
                Log.Error("FileCleaningUpService Exception", e.Source);
                HandleExceptions(e);
            }
        }

        protected string GetAndroidStandardDirectory(string iDirName)
        {
            string loAndroidDir = string.Empty;
            
            loAndroidDir = Android.OS.Environment.DataDirectory.ToString();
            if (loAndroidDir.Contains(iDirName)) return loAndroidDir;

            loAndroidDir = Android.OS.Environment.DirectoryAlarms.ToString();
            if (loAndroidDir.Contains(iDirName)) return loAndroidDir;

            loAndroidDir = Android.OS.Environment.DirectoryDcim.ToString();
            if (loAndroidDir.Contains(iDirName)) return loAndroidDir;

            loAndroidDir = Android.OS.Environment.DirectoryDocuments.ToString();
            if (loAndroidDir.Contains(iDirName)) return loAndroidDir;

            loAndroidDir = Android.OS.Environment.DirectoryDownloads.ToString();
            if (loAndroidDir.Contains(iDirName)) return loAndroidDir;

            loAndroidDir = Android.OS.Environment.DirectoryMovies.ToString();
            if (loAndroidDir.Contains(iDirName)) return loAndroidDir;

            loAndroidDir = Android.OS.Environment.DirectoryMusic.ToString();
            if (loAndroidDir.Contains(iDirName)) return loAndroidDir;

            loAndroidDir = Android.OS.Environment.DirectoryNotifications.ToString();
            if (loAndroidDir.Contains(iDirName)) return loAndroidDir;

            loAndroidDir = Android.OS.Environment.DirectoryPictures.ToString();
            if (loAndroidDir.Contains(iDirName)) return loAndroidDir;

            loAndroidDir = Android.OS.Environment.DirectoryPodcasts.ToString();
            if (loAndroidDir.Contains(iDirName)) return loAndroidDir;

            loAndroidDir = Android.OS.Environment.DirectoryRingtones.ToString();
            if (loAndroidDir.Contains(iDirName)) return loAndroidDir;

            loAndroidDir = Android.OS.Environment.DownloadCacheDirectory.ToString();
            if (loAndroidDir.Contains(iDirName)) return loAndroidDir;

            return string.Empty;
        }

        private bool IsFileOld(string iFileName)
        {
            if (string.IsNullOrEmpty(iFileName)) return false; 
            
            Java.IO.File loFile = new Java.IO.File(iFileName);
            if (loFile == null) return false;
                              
            //Get the file date/time                      
            long loFileTime = loFile.LastModified();
            long loFileOldTime = fRefTime - loFileTime;            
            if (loFileOldTime >= fOldTime_msec) return true;

            return false;
        }
                    
    }
}

