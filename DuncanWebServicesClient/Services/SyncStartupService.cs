using System;
using System.Data;

namespace DuncanWebServicesClient
{
    public class SyncStartupService
    {
        public SyncStartupService()
        {
        }

        public string SyncAuthenticate(string username, string password, string serialNumber, ref string errorMgs,out string sessionKey, out long assignedSubConfigKey,
                                       out string customerConfigKey, out string serialNumberStatus,out string availableSerialNumbers)
        {
            string loURL = WebserviceConstants.CLIENT_URL;
            var aiwebservice = new DuncanPublicWebservices.AutoISSUEPublicService(loURL);

            //var aiwebservice = new DuncanPublicWebservices.AutoISSUEPublicService(WebserviceConstants.CLIENT_URL);
            Console.WriteLine("SyncAuthenticate Username SerialNumber {0}  {1}", username, serialNumber);
            string encryptedSessionKey = aiwebservice.SyncSession_Authenticate(username, password, serialNumber,ref errorMgs,out sessionKey, out assignedSubConfigKey,out customerConfigKey,out serialNumberStatus,out availableSerialNumbers);
            Console.WriteLine("SyncAuthenticate return ErrorMgs {0} ", errorMgs);
            return encryptedSessionKey;
        }

        public void SyncBegin(string encryptedSessionKey, string serialNumber, string serializedClientList,ref string errorMgs, out string oSessionName, out string platformFiles,out string compFiles,
                              out string confFiles, out string uploadFileList, out string deleteFiles,out long assignedSubConfigKey,
                              out string customerConfigKey, out string serialNumberStatus, out string sequenceNames)
        {
            string loURL = WebserviceConstants.CLIENT_URL;
            var aiwebservice = new DuncanPublicWebservices.AutoISSUEPublicService(loURL);

            //var aiwebservice = new DuncanPublicWebservices.AutoISSUEPublicService(WebserviceConstants.CLIENT_URL);
            Console.WriteLine("SyncBegin encryptedSessionKey SerialNumber {0}  {1}", encryptedSessionKey, serialNumber);
            aiwebservice.SyncSession_Begin(encryptedSessionKey, encryptedSessionKey, serialNumber, serializedClientList,ref errorMgs, out oSessionName, out platformFiles, out compFiles,
                                           out confFiles, out uploadFileList, out deleteFiles, out assignedSubConfigKey,out customerConfigKey, out serialNumberStatus, out sequenceNames);
            Console.WriteLine("SyncBegin return ErrorMgs {0} ", errorMgs);
        }

        public string DownloadListFiles(string serialNumber, string compFiles, int assignedSubConfigKey,string encryptedSessionKey, ref string errorMgs)
        {
            string loURL = WebserviceConstants.CLIENT_URL;
            var aiwebservice = new DuncanPublicWebservices.AutoISSUEPublicService(loURL);

            //var aiwebservice = new DuncanPublicWebservices.AutoISSUEPublicService(WebserviceConstants.CLIENT_URL);
            Console.WriteLine("DownloadListFiles encryptedSessionKey SerialNumber {0}  {1}", encryptedSessionKey,serialNumber);
            string compositeFiles = aiwebservice.RefreshCompositeFiles2(serialNumber, compFiles,(int) assignedSubConfigKey,encryptedSessionKey, ref errorMgs);
            Console.WriteLine("DownloadListFiles return ErrorMgs {0} ", errorMgs);
            return compositeFiles;
        }


        // AJW - TODO - this and related code must be ported from CommCmdIP to correctly handle LARGE list files


        ///// <summary>
        ///// Helper routine for DoSynchronization().  
        ///// Downloads all the composite files in _DownloadCompositeFileList from the host.  
        ///// Updates _SyncProgress stats as files are downloaded.
        ///// In the event of an error, _SyncProgress members CurrentSyncStep, ErrorCode, and ErrorText are all updated.  Caller need only
        ///// end the current sync session.
        ///// Returns true on success, false on failure.
        ///// 
        ///// Note: While this routine is similar in structure to DownloadConfigFiles() and DownloadPlatformFiles(), 
        ///// enough differences exist to warrant separate routines and aid code clarity.
        ///// </summary>
        ///// <returns></returns>
        //protected bool DownloadCompositeFiles()
        //{
        //    _SyncProgress.CurrentSyncStep = ESyncSteps.SyncStep_DownloadFiles;


        //    // if the host didn't give us a list, there won't be anything to do here
        //    if (_DownloadCompositeFileList == null)
        //    {
        //        return true;
        //    }


        //    // RefreshCompositeFiles() returns all the composite files at once.  We'll break this up into chunks of 100K or so.
        //    FileSet loThisDownloadSet = new FileSet();
        //    _SyncProgress.CurrentFileName = "";
        //    _SyncProgress.CurrentFileSize = 0;
        //    StringBuilder loFileNames = new StringBuilder();

        //    foreach (FileInformation loFileNameInfo in _DownloadCompositeFileList.FileList)
        //    {
        //        // add this file to the download list.
        //        loThisDownloadSet.FileList.Add(loFileNameInfo);
        //        _SyncProgress.CurrentFileSize += (int)loFileNameInfo.Length;
        //        if (loFileNames.Length > 0)
        //            loFileNames.Append(',');
        //        loFileNames.Append(loFileNameInfo.FileName);
        //        // does this file put us over the top?
        //        if (_SyncProgress.CurrentFileSize < 10000)
        //            continue; // nope, get the next one.

        //        _SyncProgress.CurrentFileName = loFileNames.ToString();
        //        WriteSyncProgressToLocalLog();
        //        if (!DownloadOneCompositeFileSet(loThisDownloadSet))
        //            return false; // _SyncProgress is updated.

        //        _SyncProgress.CurrentFileName = "";
        //        loThisDownloadSet.FileList.Clear(); // clear it out for next time.
        //        _SyncProgress.CurrentFileSize = 0;
        //        loFileNames.Length = 0; // clear it out.
        //    }

        //    // at the end of the loop, there may still be some left-overs.
        //    if (loThisDownloadSet.FileList.Count > 0)
        //    {
        //        _SyncProgress.CurrentFileName = loFileNames.ToString();
        //        WriteSyncProgressToLocalLog();
        //        return DownloadOneCompositeFileSet(loThisDownloadSet);
        //    }
        //    return true;
        //}


        public byte[] GetHHConfigFile(string serialNumber, string customerConfigKey, string fileName,string encryptedSessionKey, ref string errorMgs)
        {
            string loURL = WebserviceConstants.CLIENT_URL;
            var aiwebservice = new DuncanPublicWebservices.AutoISSUEPublicService(loURL);

            //var aiwebservice = new DuncanPublicWebservices.AutoISSUEPublicService(WebserviceConstants.CLIENT_URL);
            Console.WriteLine("GetHHConfigFile encryptedSessionKey SerialNumber {0}  {1}", encryptedSessionKey,serialNumber);
            byte[] loFileContents = aiwebservice.GetHHConfigFile(serialNumber, customerConfigKey,fileName, encryptedSessionKey, ref errorMgs);
            Console.WriteLine("GetHHConfigFile return ErrorMgs {0} ", errorMgs);
            return loFileContents;
        }

        public byte[] GetHHPlatformFile(string serialNumber, string customerConfigKey, string fileName, string encryptedSessionKey, ref string errorMgs)
        {
            string loURL = WebserviceConstants.CLIENT_URL;
            var aiwebservice = new DuncanPublicWebservices.AutoISSUEPublicService(loURL);

            Console.WriteLine("GetHHPlatformFile encryptedSessionKey SerialNumber {0}  {1}", encryptedSessionKey, serialNumber);

            byte[] loFileContents = aiwebservice.GetHHPlatformFile(serialNumber, fileName, encryptedSessionKey, ref errorMgs);

            Console.WriteLine("GetHHPlatformFile return ErrorMgs {0} ", errorMgs);

            return loFileContents;
        }


        public byte[] GetSequenceFile(string serialNumber, string loSequenceName, string encryptedSessionKey,ref string errorMgs)
        {
            string loURL = WebserviceConstants.CLIENT_URL;
            var aiwebservice = new DuncanPublicWebservices.AutoISSUEPublicService(loURL);

            //var aiwebservice = new DuncanPublicWebservices.AutoISSUEPublicService(WebserviceConstants.CLIENT_URL);
            Console.WriteLine("GetSequenceFile encryptedSessionKey SerialNumber {0}  {1}", encryptedSessionKey, serialNumber);
            byte[] loFileContents = aiwebservice.GetNewUnitBooks(serialNumber, loSequenceName, encryptedSessionKey,ref errorMgs);
            Console.WriteLine("GetSequenceFile return ErrorMgs {0} ", errorMgs);
            return loFileContents;
        }

        public byte[] GetUserStructFile(string serialNumber, string encryptedSessionKey, ref string errorMgs, ref string userStructFilename)
        {
            string loURL = WebserviceConstants.CLIENT_URL;
            var aiwebservice = new DuncanPublicWebservices.AutoISSUEPublicService(loURL);

            //var aiwebservice = new DuncanPublicWebservices.AutoISSUEPublicService(WebserviceConstants.CLIENT_URL);
            Console.WriteLine("GetUserStructFile encryptedSessionKey SerialNumber {0}  {1}", encryptedSessionKey, serialNumber);
            byte[] loFileContents = aiwebservice.GenerateHandheldUserStructFile_Ex(serialNumber, encryptedSessionKey, ref errorMgs, ref userStructFilename);
            Console.WriteLine("GetUserStructFile return ErrorMgs {0} ", errorMgs);
            return loFileContents;
        }
    }
}

