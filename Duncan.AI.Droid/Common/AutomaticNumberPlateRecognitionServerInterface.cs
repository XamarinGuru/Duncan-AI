
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
using Reino.ClientConfig;

namespace Duncan.AI.Droid.Common
{
    class AutomaticNumberPlateRecognitionServerInterface
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
        //public AutomaticNumberPlateRecognitionServerInterface(Activity context, FragmentManager fragmentManager)
        //{
        //    this._context = context;
        //    _fragmentManager = fragmentManager;
        //}



        public class ANPRCoordinate
        {
            public int x { get; set; }
            public int y { get; set; }
        }

        public class ANPRCandidate
        {
            public string plate { get; set; }
            public double confidence { get; set; }
            public int matches_template { get; set; }
        }

        public class ANPRServiceResult
        {
            public string plate { get; set; }
            public double confidence { get; set; }
            public int matches_template { get; set; }
            public int plate_index { get; set; }
            public string region { get; set; }
            public int region_confidence { get; set; }
            public double processing_time_ms { get; set; }
            public int requested_topn { get; set; }
            public List<ANPRCoordinate> coordinates { get; set; }
            public List<ANPRCandidate> candidates { get; set; }
        }

        public class ANPRResultRootObject
        {
            public int version { get; set; }
            public string data_type { get; set; }
            public long epoch_time { get; set; }
            public int img_width { get; set; }
            public int img_height { get; set; }
            public double processing_time_ms { get; set; }
            public List<object> regions_of_interest { get; set; }
            public List<Duncan.AI.Droid.Common.AutomaticNumberPlateRecognitionServerInterface.ANPRServiceResult> results { get; set; }
        }

        public ANPRResultRootObject myLPRResults = null;

        //protected int byteSizeOf(Bitmap data)
        //{
        //    if (Build.VERSION.SDK_INT < Build.VERSION_CODES.HoneycombMr1)
        //    {
        //        return data.GetRowBytesgetRowBytes() * data.getHeight();
        //    }
        //    else if (Build.VERSION.SDK_INT < Build.VERSION_CODES.KITKAT)
        //    {
        //        return data.GetBitmapInfo.getByteCount();
        //    }
        //    else
        //    {
        //        return data.getAllocationByteCount();
        //    }
        //}


        // https:/ /developer.xamarin.com/recipes/android/resources/general/load_large_bitmaps_efficiently/


        public async Task<BitmapFactory.Options> GetBitmapOptionsOfImageAsync(string path)
        {
            BitmapFactory.Options options = new BitmapFactory.Options
                                            {
                                                InJustDecodeBounds = true
                                            };

            try
            {
                // The result will be null because InJustDecodeBounds == true.
                //Bitmap result = await BitmapFactory.DecodeResourceAsync(Resources, Resource.Drawable.samoyed, options);
                Bitmap result = await BitmapFactory.DecodeFileAsync(path, options);

                //int imageHeight = options.OutHeight;
                //int imageWidth = options.OutWidth;

                //_originalDimensions.Text = string.Format("Original Size= {0}x{1}", imageWidth, imageHeight);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception in GetBitmapOptionsOfImageAsync: {0}", exp.Message);
            }

            return options;
        }

        public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            float height = options.OutHeight;
            float width = options.OutWidth;
            double inSampleSize = 1D;

            if (height > reqHeight || width > reqWidth)
            {
                int halfHeight = (int)(height / 2);
                int halfWidth = (int)(width / 2);

                // Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
                while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return (int)inSampleSize;
        }

        public async Task<Bitmap> LoadScaledDownBitmapForDisplayAsync(string path, BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Calculate inSampleSize
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;

            //return await BitmapFactory.DecodeResourceAsync(res, Resource.Drawable.samoyed, options);

            Bitmap result = null;


            try
            {
                //return BitmapFactory.DecodeFile(path, options);
                result = await BitmapFactory.DecodeFileAsync(path, options);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception in LoadScaledDownBitmapForDisplayAsync: {0}", exp.Message);
            }

            return result;
        }

        public static byte[] bitmapToByteArray(Bitmap bm)
        {
            byte[] result = null;

            try
            {
                AndroidBitmapInfo loBMI = bm.GetBitmapInfo();

                // Create the buffer with the correct size
                uint iBytes = loBMI.Width * loBMI.Height * 4;
                int iBytesAsInt = (int)iBytes;

                //ByteBuffer buffer = ByteBuffer.Allocate(iBytesAsInt);

                //// Copy to buffer and then into byte array
                //bm.CopyPixelsToBuffer(buffer);

                //// Log.e("DBG", buffer.remaining()+""); -- Returns 0
                //result = buffer.ToArray<byte>();


                ByteBuffer buffer = ByteBuffer.Allocate(iBytesAsInt);

                // Copy to buffer and then into byte array
                bm.CopyPixelsToBuffer(buffer);
                buffer.Rewind();

                result = new byte[iBytes];

                buffer.Get(result);

            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception in bitmapToByteArray: {0}", exp.Message);
            }

            return result;
        }


        /*
         public static final int ORIENTATION_FLIP_HORIZONTAL
         Constant Value: 2 (0x00000002)
          
        public static final int ORIENTATION_FLIP_VERTICAL
        Constant Value: 4 (0x00000004)
          
       public static final int ORIENTATION_NORMAL          // this is what we need for LPR server to work
       Constant Value: 1 (0x00000001)
          
           
       public static final int ORIENTATION_ROTATE_180
       Constant Value: 3 (0x00000003)
          
       public static final int ORIENTATION_ROTATE_270
       Constant Value: 8 (0x00000008)
        
       public static final int ORIENTATION_ROTATE_90      // this is what the image is when the camera is oriented up and down, lpr cant read it without rotating 
       Constant Value: 6 (0x00000006)
           
        public static final int ORIENTATION_TRANSPOSE
        Constant Value: 5 (0x00000005)
           
       public static final int ORIENTATION_TRANSVERSE
        Constant Value: 7 (0x00000007)
          
       public static final int ORIENTATION_UNDEFINED
        Constant Value: 0 (0x00000000) 
         * 
         * 
         * 
         * 
          
    * Constants for {@link TAG_ORIENTATION}. They can be interpreted as
    * follows:
    * <ul>
    * <li>TOP_LEFT is the normal orientation.</li>
    * <li>TOP_RIGHT is a left-right mirror.</li>
    * <li>BOTTOM_LEFT is a 180 degree rotation.</li>
    * <li>BOTTOM_RIGHT is a top-bottom mirror.</li>
    * <li>LEFT_TOP is mirrored about the top-left<->bottom-right axis.</li>
    * <li>RIGHT_TOP is a 90 degree clockwise rotation.</li>
    * <li>LEFT_BOTTOM is mirrored about the top-right<->bottom-left axis.</li>
    * <li>RIGHT_BOTTOM is a 270 degree clockwise rotation.</li>
    * </ul>
     
   public static interface Orientation {
       public static final short TOP_LEFT = 1;
       public static final short TOP_RIGHT = 2;
       public static final short BOTTOM_LEFT = 3;
       public static final short BOTTOM_RIGHT = 4;
       public static final short LEFT_TOP = 5;
       public static final short RIGHT_TOP = 6;
       public static final short LEFT_BOTTOM = 7;
       public static final short RIGHT_BOTTOM = 8;
          }
         */

        public static byte[] ImageToByteArrayForANPRResaved(string iOriginalImageFileName, int reqWidth, int reqHeight)
        {
            string ResizedImageFileName = System.IO.Path.GetDirectoryName(iOriginalImageFileName) + "/" + cnLPRCandidateSavedFileName;

            byte[] result = null;

            try
            {
                // get the Exif metadata from the original image
                ExifInterface OriginalImageExifMetedata = new ExifInterface(iOriginalImageFileName);
                string originalOrientation = OriginalImageExifMetedata.GetAttribute(ExifInterface.TagOrientation);

                // resize  to make the send faster
               
                Bitmap bitmap = BitmapHelpers.LoadAndResizeBitmap(iOriginalImageFileName, reqWidth, reqHeight);

                using (var os = new FileStream(ResizedImageFileName, FileMode.Create))
                {
                    bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, os);
                    os.Close();
                }

                bitmap.Recycle();
                // added in 7.25.06 - does it actually help? need a mem profile before/after
                bitmap.Dispose();
                bitmap = null;
                GC.Collect();

                // update the Exif metadata, this is lost when saving the bitmap?
                ExifInterface ResizedImageExitMetaData = new ExifInterface(ResizedImageFileName);

                //// force it back to normal
                //originalOrientation = "1";
                //// remove it altogether
                //originalOrientation = "";

                ResizedImageExitMetaData.SetAttribute(ExifInterface.TagOrientation, originalOrientation);

                ResizedImageExitMetaData.SaveAttributes();

            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception in ImageToByteArrayForANPRResaved: {0}", exp.Message);
                return null;
            }

            try
            {
                result = File.ReadAllBytes(ResizedImageFileName);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception in ImageToByteArrayForANPR: {0}", exp.Message);
            }

            return result;
        }

        public static byte[] ImageToByteArrayForANPRResaved(string iOriginalImageFileName)
        {
            string ResizedImageFileName = iOriginalImageFileName;
            byte[] result = null;

            try
            {
                // get the Exif metadata from the original image
                ExifInterface OriginalImageExifMetedata = new ExifInterface(iOriginalImageFileName);
                string originalOrientation = OriginalImageExifMetedata.GetAttribute(ExifInterface.TagOrientation);

                // update the Exif metadata, this is lost when saving the bitmap?
                ExifInterface ResizedImageExitMetaData = new ExifInterface(ResizedImageFileName);

                ResizedImageExitMetaData.SetAttribute(ExifInterface.TagOrientation, originalOrientation);

                ResizedImageExitMetaData.SaveAttributes();

            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception in ImageToByteArrayForANPRResaved: {0}", exp.Message);
                return null;
            }

            try
            {
                result = File.ReadAllBytes(ResizedImageFileName);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception in ImageToByteArrayForANPR: {0}", exp.Message);
            }

            return result;
        }

        public static byte[] ImageToByteArrayForANPR(string fileName, int reqWidth, int reqHeight)
        {
            try
            {
                Bitmap bitmap = BitmapHelpers.LoadAndResizeBitmap(fileName, reqWidth, reqHeight);

                using (var ms = new MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                    //  WriteBitmapData(bitmap);
                    byte[] bitmapData = ms.ToArray();
                    bitmap.Recycle();
                    // added in 7.25.06 - does it actually help? need a mem profile before/after
                    bitmap.Dispose();
                    bitmap = null;

                    return bitmapData;
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception in ImageToByteArrayForANPR: {0}", exp.Message);
            }

            return null;
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



        private async Task<bool> FetchANPRUsingRestSharp(string path)
        {

            _ResultsAvailable = false;

            _LastStatusText = "Preparing...";
            string loServerURL;
            int loServerPort;

            if (TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                           TTRegistry.regLPR_TEST_ALTERNATE_URL,
                                                           TTRegistry.regLPR_TEST_ALTERNATE_URL_DEFAULT) > 0)
            {
                loServerURL = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP,
                                                                     TTRegistry.regLPR_TEST_SERVER_URL,
                                                                     TTRegistry.regLPR_TEST_SERVER_URL_DEFAULT);
                loServerPort = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                           TTRegistry.regLPR_TEST_SERVER_PORT,
                                                                           TTRegistry.regLPR_TEST_SERVER_PORT_DEFAULT);
            }
            else
            {
                loServerURL = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP,
                                                                     TTRegistry.regLPR_SERVER_URL,
                                                                     TTRegistry.regLPR_SERVER_URL_DEFAULT);
                loServerPort = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                           TTRegistry.regLPR_SERVER_PORT,
                                                                           TTRegistry.regLPR_SERVER_PORT_DEFAULT);
            }
            
            string url = loServerURL + ":" + loServerPort.ToString();

            //localhost settings

            //string requestHost = @"http://localhost:3000/receipts";
            //string tagnr = "p94tt7w";
            //string machinenr = "2803433";
            //string safe_token = "123";

            // Do it with RestSharp
            //templateRequest req = new templateRequest();
            //req.receipt = new templateRequest.Receipt(tagnr);
            //req.machine = new templateRequest.Machine(machinenr, safe_token);

            //var request = new RestRequest("/receipts", Method.POST);
            var request = new RestRequest("/", Method.POST);

            //request.AddParameter("receipt[tag_number]", tagnr);
            //request.AddParameter("receipt[ispaperduplicate]", 0);
            //request.AddParameter("machine[serial_number]", machinenr);
            //request.AddParameter("machine[safe_token]", safe_token);
            //request.AddFile("receipt[receipt_file]", File.ReadAllBytes(path), Path.GetFileName(path), "application/octet-stream");


            // add file bytes, method 1

            ////request.AddFile("data[car.jpg]", File.ReadAllBytes(path), Path.GetFileName(path), "application/octet-stream");
            ////request.AddFile("", File.ReadAllBytes(path), Path.GetFileName(path), "application/octet-stream");
            //request.AddFile("", File.ReadAllBytes(path), Path.GetFileName(path) );

            //// Add HTTP Headers
            ////request.AddHeader("Content-type", "application/json");
            ////request.AddHeader("Accept", "application/json");
            //request.AddHeader("image-type", "jpg");



            // add file bytes, method 2
            //string documentId = "";
            //string fullFileName = path;

            //byte[] documentBytes = File.ReadAllBytes(path);

            //request.AddHeader("Content-Disposition",
            //        string.Format("file; filename=\"{0}\"; documentid={1}; fileExtension=\"{2}\"",
            //        Path.GetFileNameWithoutExtension(path), documentId, Path.GetExtension(fullFileName)));

            //request.AddParameter("application/pdf", documentBytes, ParameterType.RequestBody);

            //request.AddHeader("image-type", "jpg");

            // // add file bytes, method 3, with resizing
            // string documentId = "";
            // string fullFileName = path;

            // //byte[] documentBytes = File.ReadAllBytes(path);

            
            ////BitmapFactory.Options options = await GetBitmapOptionsOfImageAsync(path);
            ////Bitmap resized = await LoadScaledDownBitmapForDisplayAsync(path, options, newWidth, newHeight);
            // //byte[] documentBytes = ImageToByteArrayForANPR(path, newWidth, newHeight);

            //byte[] documentBytes = File.ReadAllBytes(candidatename);

            // substiture real camera image with test one
            //path = System.IO.Path.GetDirectoryName(path) + "/" + "20160202_163313_top_left_good.jpg"; // retest original

            /// method 4 - test resized file alone
            // add file bytes, method 3, with resizing
            string documentId = "";
            string fullFileName = path;


            // the resized one will be saved with a fixed name
                        
            byte[] documentBytes = null;
            //Check if resizing is enable or not
             int loRegValue = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                         TTRegistry.regLPR_TEST_RESIZE_IMAGE,
                                                                         TTRegistry.regLPR_TEST_RESIZE_IMAGE_DEFAULT);
             if (loRegValue > 0)
             {
                 string candidatename = System.IO.Path.GetDirectoryName(path) + "/" + cnLPRCandidateSavedFileName;
                 int newWidth = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                            TTRegistry.regLPR_TEST_RESIZE_IMAGE_WIDTH,
                                                                            TTRegistry.regLPR_TEST_RESIZE_IMAGE_WIDTH_DEFAULT);
                 int newHeight = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                            TTRegistry.regLPR_TEST_RESIZE_IMAGE_HEIGHT,
                                                                            TTRegistry.regLPR_TEST_RESIZE_IMAGE_HEIGHT_DEFAULT);
                 documentBytes = ImageToByteArrayForANPRResaved(path, newWidth, newHeight);
             }
             else
             {
                 //No resizing
                 documentBytes = ImageToByteArrayForANPRResaved(path);
             }
            //string candidatename = System.IO.Path.GetDirectoryName(path) + "/" + "alan_p640.jpg"; // retest original
            //string candidatename = System.IO.Path.GetDirectoryName(path) + "/" + "20160202_163307_right_top_bad.jpg"; // retest original
            //string candidatename = System.IO.Path.GetDirectoryName(path) + "/" + "20160202_163313_top_left_good.jpg"; // retest original
            //byte[] documentBytes = File.ReadAllBytes(candidatename);

            bool loPayloadOK = false;


            if (documentBytes != null)
            {
                loPayloadOK = true;

                request.AddHeader("Content-Disposition",
                        string.Format("file; filename=\"{0}\"; documentid={1}; fileExtension=\"{2}\"",
                        System.IO.Path.GetFileNameWithoutExtension(path), documentId, System.IO.Path.GetExtension(fullFileName)));

                request.AddParameter("application/pdf", documentBytes, ParameterType.RequestBody);

                request.AddHeader("image-type", "jpg");

            }


            if (loPayloadOK == false)
            {
                _LastStatusText = "No payload available ";
                Console.WriteLine("No payload available for FetchANPRUsingRestSharp: {0}", "");
                return false;
            }

            request.RequestFormat = DataFormat.Json;
            //set request Body
            //request.AddBody(req); 


            // execute the request
            //calling server with restClient
            //RestClient restClient = new RestClient("http://localhost:3000");
            RestClient restClient = new RestClient(url);


            string plateToDisplay;
            double confidenceToDisplay;

            topPlateResult = "";

            bool loDesrializedOK = false;

            _LastStatusText = "Sending Request...";

            //restClient.ExecuteAsync(request, (response) =>
            _asyncHandle = restClient.ExecuteAsync<Duncan.AI.Droid.Common.AutomaticNumberPlateRecognitionServerInterface.ANPRResultRootObject>(request, (response) =>
            {
  
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _LastStatusText = "Response OK.";

                    try
                    {
                        RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                        myLPRResults = deserial.Deserialize<ANPRResultRootObject>(response);


                        foreach (ANPRServiceResult oneResult in myLPRResults.results)
                        {
                            topPlateResult = oneResult.plate;
                            topPlateConfidence = oneResult.confidence;


                            foreach (ANPRCandidate oneCandidate in oneResult.candidates)
                            {
                                plateToDisplay = oneCandidate.plate;
                                confidenceToDisplay = oneCandidate.confidence;
                            }


                            // we have what we need here, will iterate through rest of results during confirmation dialog
                            break;
                        }


                        loDesrializedOK = true;

                        _ResultsAvailable = true;
                    }
                    catch ( Exception exp )
                    {
                        _LastStatusText = "Unable to deserialize response";
                        Console.WriteLine("Unable to deserialize response: {0}", "");
                        return;
                    }



                    //var showLPR = new AlertDialog.Builder(_context);
                    //showLPR.SetPositiveButton("Yes", (sender, args) =>
                    //    {
                    //        // User pressed yes
                    //    });
                    //showLPR.SetNegativeButton("No", (sender, args) =>
                    //{
                    //    // User pressed no 
                    //});
                    //showLPR.SetMessage("Top Candidate" + topPlateResult);
                    //showLPR.SetTitle("LPR Results");
                    //showLPR.Show();

                    //upload successfull
                    //MessageBox.Show("Upload completed succesfully...\n" + response.Content);
                }
                else
                {
                    //_LastStatusText = "Error: " +  response.StatusCode + response.StatusDescription;

                    if (string.IsNullOrEmpty(response.ErrorMessage) == true)
                    {
                        _LastStatusText = "Error: Unspecified Failure";
                    }
                    else
                    {
                        _LastStatusText = response.ErrorMessage;
                    }

                    //error ocured during upload
                    // MessageBox.Show(response.StatusCode + "\n" + response.StatusDescription);
                }

            });


            if (loDesrializedOK == false)
            {
                return false;
            }


            _LastStatusText = "Response Recieved.";

            return true;




            //var Client = new RestClient(url);

            //var request = new RestRequest("resource/{id}", Method.POST);
            //request.AddParameter("name", "value"); // adds to POST or URL querystring based on Method
            //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

            //// easily add HTTP Headers
            //request.AddHeader("header", "value");

            //// add files to upload (works with compatible verbs)
            //request.AddFile(path);

            //// execute the request
            //RestResponse response = client.Execute(request);
            //var content = response.Content; // raw content as string

            //// or automatically deserialize result
            //// return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            //RestResponse<Person> response2 = client.Execute<Person>(request);
            //var name = response2.Data.Name;

            //// easy async support
            //client.ExecuteAsync(request, response =>
            //{
            //    Console.WriteLine(response.Content);
            //});

            //// async with deserialization
            //var asyncHandle = client.ExecuteAsync<Person>(request, response =>
            //{
            //    Console.WriteLine(response.Data.Name);
            //});

            //// abort the request on demand
            //asyncHandle.Abort();

        }





        public async Task<bool> CallAutomaticNumberPlateRecognitionService(string iPhotoFile)
        {


            // Fetch the weather information asynchronously, 
            // parse the results, then update the screen:
            JsonValue json = await FetchANPRUsingRestSharp(iPhotoFile);

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

