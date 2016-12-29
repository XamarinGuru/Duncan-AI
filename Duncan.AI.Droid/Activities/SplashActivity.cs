
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Preferences;
using Android.OS;
using Android.Support.V7.App;
using Duncan.AI.Droid.Managers;
using Duncan.AI.Droid.Utils;
using Duncan.AI.Droid.Utils.HelperManagers;
using Android.Util;
using DuncanWebServicesClient;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Java.IO;
using File = Java.IO.File;

namespace Duncan.AI.Droid
{
    [Activity(
                Theme = "@style/MyTheme.Splash",
                MainLauncher = true,
                Icon = "@mipmap/ic_launcher",
                NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;

        readonly LoginManager _loginImpl;

        public SplashActivity() : base()
		{
            _loginImpl = new LoginManager();
		}



        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            // we don't want to save the state
            //  we want a new splash screen displayed each time we launch this activity base.OnCreate(savedInstanceState, persistentState);

            Log.Debug(TAG, "SplashActivity.OnCreate");
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Init our global DroidContext class
            DroidContext.SetGlobalApplicationContext(this.ApplicationContext);


            Task startupWork = new Task(() =>
                                        {
                                            Log.Debug(TAG, "Performing some startup work that takes a bit of time.");


                                           // Task.Delay(5000); // Simulate a bit of startup work.
                                            


//                                            Log.Debug(TAG, "Working in the background - important stuff.");
                                        });

            startupWork.ContinueWith(t =>
                                     {
                                         Log.Debug(TAG, "Work is finished - start MainActivity.");


                                         ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                                         
                                         //string loBootMode = prefs.GetString( Constants.appsettings_property_name_bootmode, "" );
                                         //if (loBootMode.Equals(Constants.appsettings_bootmode_value_resync) == true)
                                         //{
                                         //    this.Window.SetBackgroundDrawableResource(Resource.Drawable.splashscreen_refresh_background);
                                         //}

                                         //  // always clear on boot
                                         //ISharedPreferencesEditor editor = prefs.Edit();
                                         //editor.PutString(Constants.appsettings_property_name_bootmode, "");
                                         //editor.Apply();



                                         // always load basic stuff
                                         GetAppProperties();

                                         string username = prefs.GetString(Constants.username, null);

                                         bool loAlreadyLoggedIn = (string.IsNullOrEmpty(username) == false);

                                         // mot loaded yet?
                                         if (string.IsNullOrEmpty(DuncanWebServicesClient.WebserviceConstants.CLIENT_URL))
                                         {
                                             System.Console.WriteLine("Missing Client URL");
                                         }


                                         // where to?
                                         if (loAlreadyLoggedIn == false)
                                         {
                                             StartActivity(typeof(LoginActivity));
                                         }
                                         else
                                         {

                                             StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                                         }

                                         // clear the history so back doesn't return here
                                         //Intent mainActivity = new Intent(Application.Context, typeof(MainActivity));
                                         //mainActivity.AddFlags(ActivityFlags.ClearTop);
                                         //mainActivity.AddFlags(ActivityFlags.NoHistory);
                                         //StartActivity(mainActivity);

                                         // we're done
                                         this.Finish();


                                     }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }

        private string GetConfigFileName()
        {
            string loConfigFileName = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads) +
                                      "/" +
                                      Constants.CONFIG_FILE_NAME;

            return loConfigFileName;
        }



        private void SaveAppProperties(PropertiesDAO propertiesDAO)
        {
            long response = _loginImpl.SaveAppProperties(propertiesDAO);
        }

        private void GetAppProperties()
        {
            Constants.SERIAL_NUMBER = Helper.GetDeviceUniqueSerialNumber();


            // ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();  deprecated....
            // call the following before you make service call but be careful to remove this code on the production when you have real certs
            SSLValidator.OverrideValidation();  


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


            // set some global values
            if
                (
                  (string.IsNullOrEmpty(propertiesDAO.url) == false) &&
                  (string.IsNullOrEmpty(propertiesDAO.name) == false)
                )
            {
                WebserviceConstants.CLIENT_URL = Helper.BuildAutoISSUEPublicServiceHostURL(propertiesDAO.url, propertiesDAO.name);
                WebserviceConstants.PRIVATE_CLIENT_URL = propertiesDAO.private_url;
            }
            else
            {
                // this is an error condition
                WebserviceConstants.CLIENT_URL = "";
                WebserviceConstants.PRIVATE_CLIENT_URL = "";
            }


            Constants.CLIENT_NAME = propertiesDAO.name;
            Constants.PRINTER_TYPE = propertiesDAO.PrinterType;
        }
    }


    public static class SSLValidator
    {
        private static bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain,
                                                  SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public static void OverrideValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                OnValidateCertificate;
            ServicePointManager.Expect100Continue = true;
        }
    }
}