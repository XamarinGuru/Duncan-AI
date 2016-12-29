using System;
using System.Collections.Generic;
using System.Linq;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.Util;

    using System.Diagnostics;
    using System.Web.Services.Protocols;

using Java.Nio;

using System.Collections;

using System.Json;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using RestSharp;

using Android.App;
using Android.Media;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using DuncanWebServicesClient;
using DuncanWebServicesClient.DuncanPublicWebservices;


namespace Duncan.AI.Droid.Common
{
    class SearchStructServerInterface
    {

        private AutoISSUEPublicService client = null;

        public string gCallingFragmentTagName = string.Empty;

        //private string _tagName = string.Empty;
        
        private AsyncCallback _asyncCallback = null;
        private IAsyncResult _asyncResult = null;


        private bool _ResultsAvailable = false;
        private string _LastStatusText = string.Empty;

        private string _LastResultErrMsg = string.Empty;
        private byte[] _LastResultDataset = null;


        protected Reino.ClientConfig.TSearchStruct fSearchStruct;    // the structure that initiated the search 
        protected bool fCalledFromSearchAndIssue; // differentiates between two types of evaluations
        protected short fMinMatchCount;		      // some of the search criteria
        protected Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditDependency fInitiatingEditRestriction; // editrestriction that initiated the wireless search.




        public SearchStructServerInterface()
        {
            client = new AutoISSUEPublicService(WebserviceConstants.CLIENT_URL);

            //_tagName = Helper.BuildIssueSelectFragmentTag("SEARCHRESULT");

        }


        public void CancelHotsheetSearchRequest()
        {
            if (_asyncCallback != null)
            {
                _asyncCallback.EndInvoke( null );
                _asyncCallback = null;
            }
        }

        public bool ResultsAreAvailable()
        {
            return _ResultsAvailable;
        }

        public string GetLastStatusTextMessage()
        {
            return _LastStatusText;
        }

        public byte[] GetLastResultDataSet()
        {
            return _LastResultDataset;
        }

        public string GetLastResultErrorMessage()
        {
            return _LastResultErrMsg;
        }


        public Reino.ClientConfig.TSearchStruct GetSearchStruct()
        {
            return fSearchStruct;
        }


        public short GetMinMatchCount()
        {
            return fMinMatchCount;
        }

        public bool GetWasCalledFromSearchAndIssue()
        {
            return fCalledFromSearchAndIssue;
        }

        public Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditDependency GetInitiatingEditRestriction()
        {
            return fInitiatingEditRestriction;
        }



        public IAsyncResult GetLastAsycnResult()
        {
            return _asyncResult;
        }


        void WirelessHotSheetSearchFinished(IAsyncResult iResult)
        {
            try
            {
                _LastResultDataset = client.EndSearchHotSheet(iResult, out _LastResultErrMsg);
                _asyncResult = iResult;
                _ResultsAvailable = true;
            }
            catch (SystemException e)
            {
                Console.WriteLine("WirelessHotSheetSearchFinished Error: {0}", e.Message);
                return;
            }
        }

        private async Task<bool> CallHotSheetSearchWebServicePrim( string iSerialNo, string iOfficerName, string iOfficerID, 
                                                                   Reino.ClientConfig.TSearchStruct iSearchStruct, string iSearchStructName, string iSearchParams, 
                                                                   string iCallingFragmentTagName, 
                                                                   short iMinMatchCount, bool iCalledFromSearchAndIssue,
                                                                   Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditDependency iInitiatingEditRestriction)
        {

            _ResultsAvailable = false;
            _LastStatusText = "Preparing...";

            gCallingFragmentTagName = iCallingFragmentTagName;

            _LastStatusText = "Sending Request...";


            try
            {
                _LastResultErrMsg = "";
                fMinMatchCount = iMinMatchCount;
                fCalledFromSearchAndIssue = iCalledFromSearchAndIssue;
                fInitiatingEditRestriction = iInitiatingEditRestriction;
                fSearchStruct = iSearchStruct;



                client = new AutoISSUEPublicService(WebserviceConstants.CLIENT_URL);
                _asyncCallback = new AsyncCallback(WirelessHotSheetSearchFinished);
                client.BeginSearchHotSheet(iSerialNo, iOfficerName, iOfficerID, iSearchStructName, iSearchParams, _LastResultErrMsg, _asyncCallback, null);
                return true;
            }


            catch (SystemException e)
            {
                Console.WriteLine("CallHotSheetSearchWebService Error: {0}", e.Message);
                return false;
            }


        }





        public async Task<bool> CallHotSheetSearchWebService( string iSerialNo, string iOfficerName, string iOfficerID, 
                                                              Reino.ClientConfig.TSearchStruct iSearchStruct,  string iSearchStructName, string iSearchParams, 
                                                              string iCallingFragmentTagName,
                                                              short iMinMatchCount, bool iCalledFromSearchAndIssue,
                                                              Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditDependency iInitiatingEditRestriction)

        {


            //await .... no, dont wait
            CallHotSheetSearchWebServicePrim( iSerialNo, iOfficerName, iOfficerID, 
                                              iSearchStruct, iSearchStructName, iSearchParams, iCallingFragmentTagName,
                                              iMinMatchCount, iCalledFromSearchAndIssue, iInitiatingEditRestriction);

            return true;
        }


    }
}

