using System;
using System.Data;
using DuncanWebServicesClient.DuncanPublicWebservices;

namespace DuncanWebServicesClient
{
	public class ImportRecordService
	{
		public ImportRecordService ()
		{
		}		

		//public web service call - all calls will use this. everything else will be removed eventually
        public int ImportRecord(string serialNumber, string officerName, string officerID, string masterStructName, string masterKeyValues, string structName, int structCfgRev,byte[] structBytes, byte[] attachmentBytes, ref string errorMessage)
        {
            //we have to make sure the error message is a "" string and not a null string - otherwise we will recieve an error.
            errorMessage = "";

            // defense - this can't be null
            if (attachmentBytes == null)
            {
                attachmentBytes = new byte[0];
            }


            var client = new AutoISSUEPublicService(WebserviceConstants.CLIENT_URL);
            var resultID = client.ImportRecord(serialNumber, officerName, officerID, masterStructName, masterKeyValues, structName, structCfgRev, structBytes, attachmentBytes, ref errorMessage);
            return resultID;
        }
	}
}

