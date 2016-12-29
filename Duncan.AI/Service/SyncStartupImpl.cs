using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Duncan.AI.Utils;
using DuncanWebServicesClient;

namespace Duncan.AI
{
	class SyncStartupImpl : ISyncStartup
	{
        private readonly SyncStartupService _syncStartupService;
		//No arg constructor
		public SyncStartupImpl()
		{
            _syncStartupService = new SyncStartupService();
		}

		//Authenticate - setup keys, etc.
		public string SyncAuthenticate (string username, string password, string serialNumber, ref string errorMgs,
			out string sessionKey, out long assignedSubConfigKey,
			out string customerConfigKey, out string serialNumberStatus,
			out string availableSerialNumbers)
		{
			sessionKey = "";
			assignedSubConfigKey = 0;
			customerConfigKey = "";
			serialNumberStatus = "";
			availableSerialNumbers = "";
			string encryptedSessionKey = "";
			try
			{
				encryptedSessionKey = _syncStartupService.SyncAuthenticate(username, password, serialNumber, ref errorMgs,
					out sessionKey, out assignedSubConfigKey,
					out customerConfigKey, out serialNumberStatus,
					out availableSerialNumbers);
			}
			catch(Exception e)
			{
                errorMgs = errorMgs + "\n" + "SyncAuthenticate Exception: " + e.Message;

				Console.WriteLine("SyncAuthenticate Exception source: {0}", e.Source);
			}
			return encryptedSessionKey;
		}

		//Begin the sync - gets file lists and names
		public void SyncBegin(string encryptedSessionKey, string serialNumber, string serializedClientList,
			ref string  errorMgs, out string oSessionName, out string  platformFiles, out string  compFiles,
			out string  confFiles, out string  uploadFileList, out string deleteFiles, out long assignedSubConfigKey,
			out string customerConfigKey, out string serialNumberStatus, out string sequenceNames)
		{
			oSessionName = "";
			platformFiles = "";
			compFiles = "";
			confFiles = "";
			uploadFileList = "";
			deleteFiles = "";
			assignedSubConfigKey = 0;
			customerConfigKey = "";
			serialNumberStatus = "";
			sequenceNames = "";

			try
			{
				_syncStartupService.SyncBegin(encryptedSessionKey, serialNumber, serializedClientList,
					ref errorMgs, out  oSessionName, out platformFiles, out compFiles,
					out confFiles, out uploadFileList, out deleteFiles, out assignedSubConfigKey,
					out customerConfigKey, out serialNumberStatus, out sequenceNames);
			}
			catch(Exception e)
			{
                string loMsg = e.Source + " " + e.Message;
				Console.WriteLine("SyncBegin Exception source: {0}" );

                errorMgs = errorMgs + "\n" + "SyncBegin Exception: " + e.Message;
			}
		}

		//DownloadListFiles
		public string DownloadListFiles(string serialNumber, string compFiles, int assignedSubConfigKey,
			string encryptedSessionKey, ref string errorMgs)
		{
			string compositeFiles = "";
			try
			{
				compositeFiles = _syncStartupService.DownloadListFiles(serialNumber, compFiles, (int) assignedSubConfigKey,
					encryptedSessionKey, ref errorMgs);
			}
			catch(Exception e)
			{
				Console.WriteLine("DownloadListFiles Exception source: {0}", e.Source);
                errorMgs = errorMgs + "\n" + "DownloadListFiles Exception: " + e.Message;
			}

			return compositeFiles;
		}

		//DownloadListFiles .DAT files
		public byte[] DownloadCongfigFiles(string serialNumber, string customerConfigKey, string fileName,
			string encryptedSessionKey, ref string errorMgs)
		{
			byte[] loFileContents = null;
			try
			{
				loFileContents = _syncStartupService.GetHHConfigFile(serialNumber, customerConfigKey,
					fileName, encryptedSessionKey, ref errorMgs);
			}
			catch(Exception e)
			{
				Console.WriteLine("DownloadCongfigFiles Exception source: {0}", e.Source);
                errorMgs = errorMgs + "\n" + "DownloadCongfigFiles Exception: " + e.Message;
			}

			return loFileContents;
		}

        // download hhplatform files
        public byte[] DownloadHHPlatformFile( string serialNumber, string customerConfigKey, string fileName,
                                              string encryptedSessionKey, ref string errorMgs)
        {
            byte[] loFileContents = null;
            try
            {
                loFileContents = _syncStartupService.GetHHPlatformFile(serialNumber, customerConfigKey,
                    fileName, encryptedSessionKey, ref errorMgs);
            }
            catch (Exception e)
            {
                Console.WriteLine("DownloadHHPlatformFile Exception source: {0}", e.Source);
                errorMgs = errorMgs + "\n" + "DownloadHHPlatformFile Exception: " + e.Message;
            }

            return loFileContents;
        }



	    //DownloadSeqFiles .DAT files
        public byte[] GetSequenceFile(string serialNumber, string loSequenceName, string encryptedSessionKey,
			ref string errorMgs)
		{
			byte[] loFileContents = null;
			try
			{
                loFileContents = _syncStartupService.GetSequenceFile(serialNumber, loSequenceName, encryptedSessionKey,
					ref errorMgs);
			}
			catch(Exception e)
			{
                Console.WriteLine("GetSequenceFile Exception source: {0}", e.Source);
                errorMgs = errorMgs + "\n" + "GetSequenceFile Exception: " + e.Message;
            }

			return loFileContents;
		}

        public NewDataSet GetSequenceData(string losequenceFilePath,string loSequenceName, byte[] iSeqFileData )
	    {
	      //  var sequenceNumbers = new List<SequenceDTO>();
	        var sequenceDataSet = new NewDataSet();
	        var datasetTable = new List<NewDataSetTable>();

	        //load all the sequences intot he global manager
	        if ((loSequenceName != ""))
	        {
                var seqObj = new  SequenceImp { SeqFilename = losequenceFilePath, SequenceName = loSequenceName, Name = loSequenceName};

	            //seqObj.ReadInFromFile();
                seqObj.ReadInFromByteArray(iSeqFileData);

                 SequenceManager.GlobalSequenceMgr.Sequences.Add(seqObj);
	        }

	        //build a list of items here and return
            var seq = SequenceManager.GlobalSequenceMgr.Sequences.Find(x => x.SequenceName == loSequenceName);
	        long seqNumber = 0;

	        int bookNum = 0;
	        do
	        {
	            seqNumber = seq.GetNextNumber();
                if (seqNumber > 0)
                {

                    bookNum ++;
                    var dsTableItem = new NewDataSetTable
                        {
                            SEQUENCENAME = loSequenceName,
                            PREFIX = seq.GetNextNumberPfx(),
                            SUFFIX = seq.GetNextNumberSfx(),
                            BOOKNUMBER = bookNum,
                            ALLOCATIONNO = seqNumber
                        };

                    datasetTable.Add(dsTableItem);
                    seq.LogUsedNumber(seqNumber);
                }

	        } while (seqNumber > 0);

            sequenceDataSet.Items = datasetTable.ToArray();
            return sequenceDataSet;
	    }

	    public byte[] GetUserStructFile(string serialNumber, string encryptedSessionKey, string s, ref string errorMgs, ref string userStructureFilename)
	    {
            byte[] loFileContents = null;
            try
            {
                loFileContents = _syncStartupService.GetUserStructFile(serialNumber, encryptedSessionKey, ref errorMgs, ref userStructureFilename);
            }
            catch (Exception e)
            {
                Console.WriteLine("GetUserStructFile Exception source: {0}", e.Source);
                errorMgs = errorMgs + "\n" + "GetUserStructFile Exception: " + e.Message;
            }

            return loFileContents;
	    }


        public List<UserDAO> GetUsers(string filePath)
        {
            var users = new List<UserDAO>();


            // AJW TODO - the user struct table is NOT static in definition - must check TTableDef for columns defs and order


            //load the file
            using (var sr = new StreamReader(filePath))
            {
                string line;
                // Read and display lines from the file until the end of  
                // the file is reached. 
                while ((line = sr.ReadLine()) != null)
                {
                    //split on tabs, format is
                    //Username  Password    OfficerName OfficerId   CompanyName
                    var columns = line.Split('\t');
                    var user = new UserDAO();

                     if (columns.Length > 0)
                         user.username = columns[0];
                     if (columns.Length > 1)
                         user.password = columns[1];
                     if (columns.Length > 2)
                         user.officerName = columns[2];
                     if (columns.Length > 3)
                         user.officerId = columns[3];
                     if (columns.Length > 4)
                         user.agency = columns[4];                            
                     
                    users.Add(user);                    
                }
            }


            //read each line
           

            //create a user dao out of it

           //add to list 

            //return list


            return users;
        }



        public List<UserDAO> LoadUserDataFromRawFileData(byte[] iFileData)
        {
            var users = new List<UserDAO>();


            // not in there? 
            if (iFileData == null)
            {
                // nothing to do
                return null;
            }


            // AJW TODO - the user struct table is NOT static in definition - must check TTableDef for columns defs and order



            // todo - may need to specify encoding....
            using (var sr = new StreamReader(new MemoryStream(iFileData), Encoding.Default))
            {
                string line;
                // Read and display lines from the file until the end of  
                // the file is reached. 
                while ((line = sr.ReadLine()) != null)
                {
                    //split on tabs, format is
                    //Username  Password    OfficerName OfficerId   CompanyName
                    var columns = line.Split('\t');
                    var user = new UserDAO();

                     if (columns.Length > 0)
                         user.username = columns[0];
                     if (columns.Length > 1)
                         user.password = columns[1];
                     if (columns.Length > 2)
                         user.officerName = columns[2];
                     if (columns.Length > 3)
                         user.officerId = columns[3];
                     if (columns.Length > 4)
                         user.agency = columns[4];                            
                     
                    users.Add(user);                    
                }
            }


            //read each line
           

            //create a user dao out of it

           //add to list 

            //return list


            return users;
        }

	}

}