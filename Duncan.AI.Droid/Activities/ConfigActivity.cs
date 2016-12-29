using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Duncan.AI.Droid.Managers;
using Duncan.AI.Droid.Utils;
using Duncan.AI.Droid.Utils.HelperManagers;
using DuncanWebServicesClient;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Java.IO;
using File = Java.IO.File;

namespace Duncan.AI.Droid
{
    [Activity(
                Label = "@string/ApplicationName",
                Icon = "@drawable/ic_app_title", 
                ScreenOrientation = ScreenOrientation.Portrait, 
                Theme = "@android:style/Theme.NoTitleBar"
                )]	
	public class ConfigActivity : Activity
	{
	    readonly LoginManager _loginImpl;
		Button _nextButton;
		EditText _name;
        Spinner _spinner; //648058


		public ConfigActivity()
		{
            _loginImpl = new LoginManager();
		
		}
	   

	    protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);


            // TODO - this would probably not be required if the xmlcfg was restructured not to auto-load on read/get
            if (DroidContext.ApplicationContext == null)
            {
                DroidContext.ApplicationContext = this.ApplicationContext;
            }


			GetAppProperties();
		}

        private string GetConfigFileName()
        {
            string loConfigFileName = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads) + 
                                      "/" + 
                                      Constants.CONFIG_FILE_NAME;

            return loConfigFileName;
        }



        public void GetAppProperties()
        {
            ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();


            //Create table if table does not exist
            _loginImpl.ValidateAppPropertiesTable();


            // look for legacy local config file exists. 
            var fileName = GetConfigFileName();
            if (System.IO.File.Exists(fileName) == true)
            {
                bool loConifgReadResult = ConfigurationLocalOptionsHelper.ReadConfigData();

                if (loConifgReadResult == true)
                {
                    var id = 0;

                    // the converted file may have a full URI - pick out the pieces we need
                    string loLegacyURL = WebserviceConstants.CLIENT_URL;
                    if (loLegacyURL.ToUpper().Contains("REINOWEBSERVICES") == true)
                    {
                        Uri loURLParts = new Uri(loLegacyURL);
                        string loReconstrucedURL = loURLParts.Scheme + "://" + loURLParts.Authority;
                        WebserviceConstants.CLIENT_URL = loReconstrucedURL;

                        string[] loURLPieces = loLegacyURL.Split('/');
                        foreach (string oneURLPiec in loURLPieces)
                        {
                            // extract the client name
                            if (oneURLPiec.ToUpper().EndsWith("AUTOISSUEPUBLIC") == true)
                            {
                                int loIdx = oneURLPiec.ToUpper().IndexOf("AUTOISSUEPUBLIC");
                                if (loIdx > 0)
                                {
                                    string loClientName = oneURLPiec.Substring(0, loIdx);
                                    if (loClientName.Contains("%20") == true)
                                    {
                                        loClientName = Uri.UnescapeDataString(loClientName);
                                   }

                                    Constants.CLIENT_NAME = loClientName;
                                    break;

                                }
                            }
                        }
                    }



                    var propertiesDAONewToDB = new PropertiesDAO
                    {
                        Id = 1,
                        name = Constants.CLIENT_NAME,
                        url = WebserviceConstants.CLIENT_URL,
                        private_url = WebserviceConstants.PRIVATE_CLIENT_URL,
                        clientId = id,
                        PrinterType = Constants.PRINTER_TYPE
                    };
                    SaveAppProperties(propertiesDAONewToDB);
                }

                // read it in once and then delete it
                System.IO.File.Delete(fileName);
            }


            var propertiesDAO = _loginImpl.RetrieveAppProperties();


            if (propertiesDAO == null)
            {

                var id = 0;

                // build a default version
                propertiesDAO = new PropertiesDAO
                {
                    Id = 1,
                    name = Constants.CLIENT_NAME,
                    url = WebserviceConstants.CLIENT_URL,
                    private_url = WebserviceConstants.PRIVATE_CLIENT_URL,
                    clientId = id,
                    PrinterType = Constants.PRINTER_TYPE
                };
                SaveAppProperties(propertiesDAO);
            }


            // edit your options on the sync config 
            Finish();

            WebserviceConstants.CLIENT_URL = propertiesDAO.url;
            WebserviceConstants.PRIVATE_CLIENT_URL = propertiesDAO.private_url;

            Constants.CLIENT_NAME = propertiesDAO.name;
            Constants.SERIAL_NUMBER = Helper.GetDeviceUniqueSerialNumber();

            Constants.PRINTER_TYPE = propertiesDAO.PrinterType;
            StartActivity(typeof(LoginActivity));
        }


        //void NextBtnHandleClick(object sender, EventArgs ev)
        //{
        //    if(!string.IsNullOrEmpty (_name.Text))
        //    {
        //        var propertiesDAO = new PropertiesDAO
        //            {
        //                name = _name.Text,
        //                url = _clientUrls[_spinner.SelectedItemPosition],
        //                private_url = _clientPrivateUrls[_spinner.SelectedItemPosition],
        //                clientId = _spinner.SelectedItemPosition
        //            };


        //        //Constants.SERIAL_NUMBER = _name.Text;
        //        Constants.SERIAL_NUMBER = Helper.GetDeviceUniqueSerialNumber();

        //        Constants.PRINTER_TYPE = Helper.GetSelectedPrinterType();

        //        WebserviceConstants.CLIENT_URL = _clientUrls[_spinner.SelectedItemPosition];
        //        WebserviceConstants.PRIVATE_CLIENT_URL = _clientPrivateUrls[_spinner.SelectedItemPosition];

        //        ConfigurationLocalOptionsHelper.WriteConfigData();

        //        SaveAppProperties (propertiesDAO);                
                
        //    }
        //    else
        //    {
        //        Toast.MakeText (this, "Please Enter Serial#", ToastLength.Short).Show ();                
        //    }
        //}

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            /*
            if (e.Position == 2)
                _name.Text = "Y6-e9f1b";           
            else if (e.Position == 0)
                _name.Text = "N4-22000";
            else if (e.Position == 1)
                _name.Text = "N4-90001";
             * */
        }

		public void SaveAppProperties(PropertiesDAO propertiesDAO)
		{
            long response = _loginImpl.SaveAppProperties(propertiesDAO);

            //// await! control returns to the caller and the task continues to run on another thread
            //if (response != -1) {
            //    Finish();
            //    DroidContext.ApplicationContext = this.ApplicationContext;
            //    StartActivity(typeof(LoginActivity));
            //}
		}

    //    public bool WriteConfigData()
    //    {
    //        bool retValue = false;
    //        FileWriter loFileWriter = null;

    //        try
    //        {
    //            var id = 0;  // AJW TODO - this is fixed value in current code, do we really need it? YES, there is reference against list item - this needs to be cleaned up
    //            var fileName = GetConfigFileName();

    //            List<string> loConfigFileLines = new List<string>();

    //            /*
    //           // URL@http://aidev.duncan-usa.com/ReinoWebServices/Android%20DemoAutoISSUEPublic/AutoISSUEpublicService.asmx
    //           // PRIVATEURL@http://aidev.duncan-usa.com/ReinoWebServices/Android%20DemoAutoISSUEHost/AutoISSUEHostService.asmx
    //            // Serial@N4-22000
    //             * */



    //            loConfigFileLines.Add(Constants.CONFIG_FILE_PARAMETER_PUBLIC_URL +
    //                                   Constants.CONFIG_FILE_PARAMETER_VALUE_DELIMETER +
    //                                   WebserviceConstants.CLIENT_URL);

    //            loConfigFileLines.Add(Constants.CONFIG_FILE_PARAMETER_PRIVATE_URL +
    //                                   Constants.CONFIG_FILE_PARAMETER_VALUE_DELIMETER +
    //                                   WebserviceConstants.PRIVATE_CLIENT_URL);

    //            loConfigFileLines.Add(Constants.CONFIG_FILE_PARAMETER_UNIT_SERIAL_NUMBER +
    //                                   Constants.CONFIG_FILE_PARAMETER_VALUE_DELIMETER +
    //                                   Helper.GetDeviceUniqueSerialNumber() );
    //                                   // TODO - this is not updated yet.... is this a lurking issue with "public const" decl?   Constants.SERIAL_NUMBER);

    //            // AJW this value doesn't exist - does it need to be?
    //            //loConfigFileLines.Add( Constants.CONFIG_FILE_PARAMETER_CLIENT_ID + 
    //            //                       Constants.CONFIG_FILE_PARAMETER_VALUE_DELIMETER + 
    //            //                       Constants.ID  );


    //            loConfigFileLines.Add(Constants.CONFIG_FILE_PARAMETER_PRINTER_TYPE +
    //                                   Constants.CONFIG_FILE_PARAMETER_VALUE_DELIMETER +
    //                                   Helper.GetSelectedPrinterType());



    //            loFileWriter = new FileWriter(fileName);
    //            var loWriter = new BufferedWriter(loFileWriter);

    //            foreach (string oneLine in loConfigFileLines)
    //            {
    //                loWriter.Write(oneLine);
    //                loWriter.NewLine();
    //            }
    //            loWriter.Flush();
    //            loFileWriter.Flush();
    //            loFileWriter.Close();
    //            loFileWriter.Dispose();
    //            loFileWriter = null;

    //            retValue = true;

    //        }
    //        catch (Exception e)
    //        {
    //            LoggingManager.LogApplicationError(e, null, "WriteConfigData");
    //        }
    //        finally
    //        {
    //            if (loFileWriter != null)
    //            {
    //                loFileWriter.Dispose();
    //                loFileWriter = null;
    //            }
    //        }

    //        return (retValue);
    //    }
	


    //    public bool ReadConfigData()
    //    {
    //        bool retValue = false;
    //        try
    //        {
    //            var id = 0;
    //            var fileName = GetConfigFileName();

    //            //var fileName =
    //            //           Android.OS.Environment.GetExternalStoragePublicDirectory
    //            //           (Android.OS.Environment.DirectoryDownloads) + "/" + Constants.CONFIG_FILE_NAME;


    //            //var file = new File(fileName);
    //            var fileReader = new FileReader(fileName);
    //            //if (file.Exists())
    //            if (fileReader != null)
    //            {
                 
    //                var reader = new BufferedReader(fileReader);
    //                string line = "";
    //                while ((line = reader.ReadLine()) != null)
    //                {
    //                    var linedata = line.Split('@');
    //                    if (linedata.Length >= 2)
    //                    {
    //                        retValue = true;


    //                        string loOneParameterName = linedata[0].ToUpper();

    //                        switch (loOneParameterName)
    //                        {
    //                            case Constants.CONFIG_FILE_PARAMETER_PUBLIC_URL:
    //                                {
    //                                    WebserviceConstants.CLIENT_URL = linedata[1];
    //                                    break;
    //                                }

    //                            case Constants.CONFIG_FILE_PARAMETER_PRIVATE_URL:
    //                                {
    //                                    WebserviceConstants.PRIVATE_CLIENT_URL = linedata[1];
    //                                    break;
    //                                }

    //                            case Constants.CONFIG_FILE_PARAMETER_UNIT_SERIAL_NUMBER:
    //                                {
    //                                    Constants.SERIAL_NUMBER = linedata[1];
    //                                    break;
    //                                }

    //                            case Constants.CONFIG_FILE_PARAMETER_CLIENT_ID:
    //                                {
    //                                    try
    //                                    {
    //                                        id = Convert.ToInt32(linedata[1]);
    //                                    }
    //                                    catch (Exception e)
    //                                    {
    //                                        id = 0;
    //                                    }
    //                                    break;
    //                                }


    //                            case Constants.CONFIG_FILE_PARAMETER_PRINTER_TYPE:
    //                                {
    //                                    try
    //                                    {
    //                                        Constants.PRINTER_TYPE = linedata[1];
    //                                    }
    //                                    catch (Exception e)
    //                                    {
    //                                        Constants.PRINTER_TYPE = Constants.PRINTER_TYPE_NAME_ZEBRA_MZ320;
    //                                    }
    //                                    break;
    //                                }

    //                            default:
    //                                {
    //                                    // unknown parameter - do nothing
    //                                    break;
    //                                }
    //                        }


    //                        /*
    //                        if (linedata[0].Equals("URL"))
    //                        {
    //                            WebserviceConstants.CLIENT_URL = linedata[1];
    //                        }
    //                        if (linedata[0].Equals("PRIVATEURL"))
    //                        {
    //                            WebserviceConstants.PRIVATE_CLIENT_URL = linedata[1];
    //                        }
    //                        if (linedata[0].Equals("Serial"))
    //                        {
    //                            Constants.SERIAL_NUMBER = linedata[1];
    //                        }
    //                        if (linedata[0].Equals("Client"))
    //                        {
    //                            id = Convert.ToInt32(linedata[1]);
    //                        }
    //                        */

    //                    }                        
    //                }
    //                fileReader.Close();
    //                fileReader.Dispose();
    //            }

    //            var propertiesDAO = new PropertiesDAO
    //            {
    //                name = Constants.SERIAL_NUMBER,
    //                url = WebserviceConstants.CLIENT_URL,
    //                private_url = WebserviceConstants.PRIVATE_CLIENT_URL,
    //                clientId = id,
    //                PrinterType = Constants.PRINTER_TYPE
    //            };
    //            SaveAppProperties(propertiesDAO); 
    //        }
    //        catch (Exception e)
    //        {
    //            LoggingManager.LogApplicationError(e, null, "ReadConfigData");
    //        }

    //        return (retValue);
    //    }



    }

    public class TrustAllCertificatePolicy :  ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint sp,
         X509Certificate cert, WebRequest req, int problem)
        {
            return true;
        }
    }
}

