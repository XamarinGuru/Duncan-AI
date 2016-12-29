using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace DuncanWebServicesClient
{
	public class UserDetailsService
	{
		public UserDetailsService ()
		{
		}

        //todo - move this to where it needs to go
        //test getting image, will need to move
        public byte[] GetTicketBytes(String IssueNum, DateTime IssueDate)
        {
            var aiwebservice = new DuncanDemoWebservices.AutoISSUEHostService(WebserviceConstants.PRIVATE_CLIENT_URL);
            string errorMgs = "";

            //todo - make this date string a constant format
            var datestring = IssueDate.ToString("yyyy-MM-ddT00:00:00");
            System.Data.DataSet dsParking = aiwebservice.PerformQuery("select * from parking where ISSUENO = '" + IssueNum + "' and ISSUEDATE = '" + datestring + "'", new List<object>().ToArray(), WebserviceConstants.AutoIssueSvcInternalSessionKey, ref errorMgs);
            Int32 loUniqueKey = Convert.ToInt32(dsParking.Tables[0].Rows[0]["UNIQUEKEY"]);
            //todo - error checking, make sure the tables[0] isnt null, then rows[0] isnt null, then "Uniquekey column isnt null
            //todo - just incase the issue number and date doesnt bring anything back correctly.

            var imageBytes = aiwebservice.ApiGetCitationImage("Parking", loUniqueKey, WebserviceConstants.AutoIssueSvcInternalSessionKey);
            return imageBytes;
        }
	}
}

