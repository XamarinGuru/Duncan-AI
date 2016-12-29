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
using Duncan.AI.Droid.ExternalEnforcementWebService;
using Duncan.AI.Droid.Utils.HelperManagers;

using Android.App;
using Android.Media;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Reino.ClientConfig;

namespace Duncan.AI.Droid.Common
{
    class ExternalEnforcementConfirmationServiceInterface
    {
        //Server URL and port are now defined in the registry 
        //string requestHost = @"http://localhost:3000/receipts";
        //private string ServerURL = @"http://106.51.253.214";
        //private int ServerPort = 8000;
        //private string ServerURL = @"http://69.197.149.66";
        //private string ServerURL = @"http://64.132.70.3";
        //private int ServerPort = 8000;

        private RestRequestAsyncHandle _asyncHandle = null;
        private bool _ResultsAvailable = false;
        private string _LastStatusText = string.Empty;

        private static JsonValue _ANPRServerResponse;


        private static string cnLPRCandidateSavedFileName = "LPR_CANDIDIATE.jpg";


        //public Activity _context;
        //public FragmentManager _fragmentManager;


        ///** Instantiate the interface and set the context */
        //public ExternalEnforcementConfirmationServiceInterface(Activity context, FragmentManager fragmentManager)
        //{
        //    this._context = context;
        //    _fragmentManager = fragmentManager;
        //}



        public class Coordinate
        {
            public int x { get; set; }
            public int y { get; set; }
        }

        public class Candidate
        {
            public string plate { get; set; }
            public double confidence { get; set; }
            public int matches_template { get; set; }
        }

        public class Result
        {
            public string plate { get; set; }
            public double confidence { get; set; }
            public int matches_template { get; set; }
            public int plate_index { get; set; }
            public string region { get; set; }
            public int region_confidence { get; set; }
            public double processing_time_ms { get; set; }
            public int requested_topn { get; set; }
            public List<Coordinate> coordinates { get; set; }
            public List<Candidate> candidates { get; set; }
        }

        public class RootObject
        {
            public int version { get; set; }
            public string data_type { get; set; }
            public long epoch_time { get; set; }
            public int img_width { get; set; }
            public int img_height { get; set; }
            public double processing_time_ms { get; set; }
            public List<object> regions_of_interest { get; set; }
            public List<Duncan.AI.Droid.Common.ExternalEnforcementConfirmationServiceInterface.Result> results { get; set; }
        }




        string topPlateResult = string.Empty;
        double topPlateConfidence = 100;


        public string GetTopPlateResult()
        {
            return topPlateResult;
        }

        public double GetTopPlateConfidence()
        {
            return topPlateConfidence;
        }

        public void CancelLPRProcessingRequest()
        {
            if (_asyncHandle != null)
            {
                _asyncHandle.Abort();
                _asyncHandle = null;
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



        private async Task<bool> GetEnforcementConfirmation(string oneEnforcementKey)
        {
            try
            {
                _ResultsAvailable = false;

                _LastStatusText = "Preparing...";


                string loServerURL = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP,
                                                                            TTRegistry.regENFORCEMENT_SERVICE_CONFIRMATION_URL,
                                                                            TTRegistry.regENFORCEMENT_SERVICE_CONFIRMATION_URL_DEFAULT);

                string url = loServerURL;



                _LastStatusText = "Sending Request...";

                try
                {
                    ExternalEnforcementWebService.citationsmiamiService myEnforceCheckService = new citationsmiamiService();


                    var loJsonResult = myEnforceCheckService.GetCitationData(oneEnforcementKey);

                    if (loJsonResult != null)
                    {
                        try
                        {
                            loJsonResult = null;  // JsonValue.Parse("");
                        }
                        catch
                        {
                        }
                    }


#if _hammer_
            //fail checks first
            if (string.IsNullOrEmpty(loJsonResult))
            {
                //Toast.MakeText(_context, "No meter data supplied.", ToastLength.Long).Show();
                return false;
            }

            try
            {
                var jsonMeter = JsonValue.Parse(meterInfo);

                ////ALAN: they are stryingifying twice when sending the data over to us, so parse it again: Android.LoadMeterInformation(JSON.stringify(JSON.stringify(meterInfo))); remove once they fix this to only stringify once
                ////UPDATE(12/28/2015): It looks like they updated this to only do it once:  Android.LoadMeterInformation(JSON.stringify(meterInfo));
                ////BUT there were no enforcable meters at this time, so could not test. We shouldnt need the below try/catch and should be safe to remove.
                //try
                //{
                //    if (jsonMeter != null) jsonMeter = JsonValue.Parse(jsonMeter);
                //}
                //catch (Exception ex)
                //{
                //    //couldnt parse it again, hopefully means they fixed the stringify to only occur once. can parse it the same amount of times they stringify it, any more and will throw an exception.
                //    LoggingManager.LogApplicationError(ex, "PayBySpaceMapFragment: Double Parse", "LoadMeterInformation");
                //}

                if (jsonMeter == null)
                {
                    Toast.MakeText(_context, "Invalid format: meter data.", ToastLength.Long).Show();
                    LoggingManager.LogApplicationError(null, "PayBySpaceMapFragment: Invalid format: meter data", "LoadMeterInformation");
                    return;
                }

                //store the data in the context. not saving to the db anymore
                //DroidContext.XmlCfg.SetJsonValueObjectPaySpaceMap(jsonMeter);
                ExternalEnforcementInterfaces.SetWirelessEnforcementMode(ExternalEnforcementInterfaces.TWirelessEnforcementMode.wefPayByPlateMap, jsonMeter);

                // a little kludgy... should be consistenltly passing known value
                bool loNoEnforcementDataPassed = (( meterInfo ==  null ) || (meterInfo.Length < 7));
#endif

                }
                catch (Exception exp)
                {
                    _LastStatusText = "Error: Unable to Confirm Status"; // +exp.Message;

                    LoggingManager.LogApplicationError(exp, "GetEnforcementConfirmation", exp.Message);
                    System.Console.WriteLine("Exception caught in process: {0} {1}", "GetEnforcementConfirmation", exp.Message);

                    _ResultsAvailable = true;

                    return false;
                }


                bool loDesrializedOK = true;


                if (loDesrializedOK == true)
                {
                    _LastStatusText = "Response OK.";

                    try
                    {


                        loDesrializedOK = true;

                        _ResultsAvailable = true;
                    }
                    catch (Exception exp)
                    {
                        _LastStatusText = "Unable to deserialize response";
                        Console.WriteLine("Unable to deserialize response: {0}", "");
                        return false;
                    }

                }
                else
                {
                    //_LastStatusText = "Error: " +  response.StatusCode + response.StatusDescription;

                    _LastStatusText = "Error: Unspecified Failure";
                }



                if (loDesrializedOK == false)
                {
                    return false;
                }


                _LastStatusText = "Response Recieved.";

                return true;
            }
            catch (Exception exp)
            {

                LoggingManager.LogApplicationError(exp, "GetEnforcementConfirmation", exp.Message);
                System.Console.WriteLine("Exception caught in process: {0} {1}", "GetEnforcementConfirmation", exp.Message);

            }


            return false;
        }





        public async Task<bool> CallExternalEnforcementService(string iExternalEnforcementKey)
        {


            // fetch the information asynchronously, 
            // parse the results, then update the screen:
            JsonValue json = await GetEnforcementConfirmation(iExternalEnforcementKey);

            return true;
        }



        //    public void doBeginCall()
        //{
        //  Toast.makeText(this, "Call started", Toast.LENGTH_SHORT).show();
        //  new CallTask().execute(null);  
        //}

        //public void onCallComplete(String result)
        //{
        //    Toast.makeText(this, "Call complete", Toast.LENGTH_SHORT).show();
        //    ((EditText)findViewById(R.id.Textbox)).setText(result);
        //    invalidate();
        //}

        //class CallTask extends AsyncTask<String, String, String> 
        //{

        //    protected void onPostExecute(String result) 
        //    {
        //        onCallComplete(result);
        //    }

        //    @Override
        //    protected TaskResult doInBackground(String... params) 
        //    {           
        //        return call();
        //    }
        //}

    }
}

