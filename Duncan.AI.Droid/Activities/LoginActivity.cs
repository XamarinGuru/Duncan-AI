using System;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Text;
using System.Threading.Tasks;
using Android.Preferences;
using Android.Util;
using Android.Locations;
using System.Threading;
using Duncan.AI.Droid.Managers;
using Duncan.AI.Droid.Utils;
using DuncanWebServicesClient;
using XMLConfig;
using Duncan.AI.Droid.Utils.HelperManagers;

//Login Screen 

namespace Duncan.AI.Droid
{
    [Activity(
                Label = "@string/ApplicationName",
                Icon = "@drawable/ic_app_title",
                ScreenOrientation = ScreenOrientation.Portrait, 
                Theme = "@android:style/Theme.NoTitleBar"
                )]
    public class LoginActivity : Activity
    {
        private readonly LoginManager _loginImpl;
        private static EditText _password, _userName;
        private static Button _btnLogin;

        private static TextView _cityNameText;


        private PropertiesDAO _propertiesDAO = null;



        //private static Button _issuanceInfoButton;

        private static Activity _SyncScreenActivity;
        private static ProgressBar _progressbar1;


        private static Button _btnConfig;



        private UserDAO _userDAOParent;
        private static Button _btnSynchronize;
        private static TextView _loadingTextView;
        private static TextView _loadingTextDetailsView;


        private static TextView _SerialNumberText;
        private static TextView _ApplicationRevisionText;


        private RefreshDBReceiver _refreshDBReceiver;
        private DisplaySyncProgressReceiver _SyncProgressReceiver;
        private string _cityName;

        public LoginActivity()
        {
            _loginImpl = new LoginManager();

            LoadAppProperties();
            _loginImpl.GetIssueAPXMLRevisionInfo();
        }


        private void HandleAndroidException_LoginActivity(object sender, RaiseThrowableEventArgs e)
        {
            System.Console.WriteLine("LoginActivity::Global Exception source {0}: {1}", e.Exception.TargetSite.Name, e.Exception.Source);
            LoggingManager.LogApplicationError(e.Exception, "Global Exception", e.Exception.TargetSite.Name);
            ErrorHandling.ReportException(e.Exception.Message);
        }


        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);


                // Init our global error handling class
                ErrorHandling.InitErrorHandling(this);

                // this needs to ne hooked here for login/sync, and then this hook replaced with MainActivity when it is loaded
                AndroidEnvironment.UnhandledExceptionRaiser -= HandleAndroidException_LoginActivity; // actually needed to prevent multiple callbacks?
                AndroidEnvironment.UnhandledExceptionRaiser += HandleAndroidException_LoginActivity;
        






                Typeface loCustomTypeface;

                _SyncScreenActivity = this;

                SetContentView(Resource.Layout.Login);

                // find the view so it can be updated when db loads
                _cityNameText = FindViewById<TextView>(Resource.Id.textViewCity);
                // initialize our typeface 
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnTextMessageLargeTypeface);
                if (loCustomTypeface != null)
                {
                    _cityNameText.Typeface = loCustomTypeface;
                }
                _cityNameText.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnTextMessageLargeSizeSp);





                //LoadAppProperties();




                TextView _loginPrompt = FindViewById<TextView>(Resource.Id.textLoginPrompt);
                // initialize our typeface 
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnTextMessageLargeTypeface);
                if (loCustomTypeface != null)
                {
                    _loginPrompt.Typeface = loCustomTypeface;
                }
                _loginPrompt.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnTextMessageLargeSizeSp);







                // set the max lenth of the input
                int loMaxUserNameLen = 25;   // TODO - would be nice if this could come from XML when available
                _userName = FindViewById<EditText>(Resource.Id.username);
                _userName.SetBackgroundResource(Resource.Drawable.EditTextLoginFocusLost);
                _userName.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(loMaxUserNameLen), new InputFilterAllCaps() });

                // initialize our typeface 
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnEditTextViewTypeface);
                if (loCustomTypeface != null)
                {
                    _userName.Typeface = loCustomTypeface;
                }
                _userName.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnEditTextViewTypefaceSizeSp);


                TextView _userNameLabel = FindViewById<TextView>(Resource.Id.usernameLabel);
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnEditTextLabelTypeface);
                if (loCustomTypeface != null)
                {
                    _userNameLabel.Typeface = loCustomTypeface;
                }
                _userNameLabel.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnEditTextLabelTypefaceSizeSp);





                int loMaxUserPasswordLen = 25;   // TODO - would be nice if this could come from XML when available
                _password = FindViewById<EditText>(Resource.Id.password);
                _password.SetBackgroundResource(Resource.Drawable.EditTextLoginFocusLost);
                _password.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(loMaxUserPasswordLen), new InputFilterAllCaps() });  // password is case INsensitive

                // initialize our typeface 
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnEditTextViewTypeface);
                if (loCustomTypeface != null)
                {
                    _password.Typeface = loCustomTypeface;
                }
                _password.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnEditTextViewTypefaceSizeSp);

                TextView _passwordLabel = FindViewById<TextView>(Resource.Id.passwordLabel);
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnEditTextLabelTypeface);
                if (loCustomTypeface != null)
                {
                    _passwordLabel.Typeface = loCustomTypeface;
                }
                _passwordLabel.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnEditTextLabelTypefaceSizeSp);






                _btnLogin = FindViewById<Button>(Resource.Id.loginButton);
                // initialize our typeface 
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnButtonTypeface);
                if (loCustomTypeface != null)
                {
                    _btnLogin.Typeface = loCustomTypeface;
                }
                _btnLogin.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnButtonTypefaceSizeSp);
                _btnLogin.SetTextColor(Resources.GetColor(Resource.Color.civicsmart_black));



                _btnSynchronize = FindViewById<Button>(Resource.Id.refDBButton);
                // initialize our typeface 
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnButtonTypeface);
                if (loCustomTypeface != null)
                {
                    _btnSynchronize.Typeface = loCustomTypeface;
                }
                _btnSynchronize.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnButtonTypefaceSizeSp);
                _btnSynchronize.SetTextColor(Resources.GetColor(Resource.Color.civicsmart_black));
                // soon.... _btnSynchronize.Visibility = ViewStates.Gone;



                _loadingTextView = FindViewById<TextView>(Resource.Id.loading);
                // initialize our typeface 
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnWebViewErrorMessageDetailTypeface);
                if (loCustomTypeface != null)
                {
                    _loadingTextView.Typeface = loCustomTypeface;
                }
                _loadingTextView.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnWebViewErrorMessageDetailSizeSp);
                _loadingTextView.SetTextColor(Resources.GetColor(Resource.Color.civicsmart_black));


                _loadingTextDetailsView = FindViewById<TextView>(Resource.Id.loadingdetails);
                // initialize our typeface 
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnWebViewErrorMessageFinePrintTypeface);
                if (loCustomTypeface != null)
                {
                    _loadingTextDetailsView.Typeface = loCustomTypeface;
                }
                _loadingTextDetailsView.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnWebViewErrorMessageFinePringtSizeSp);
                _loadingTextDetailsView.SetTextColor(Resources.GetColor(Resource.Color.civicsmart_black));



                _SerialNumberText = FindViewById<TextView>(Resource.Id.textViewSerialNumber);
                // initialize our typeface 
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnTextMessageMediumTypeface);
                if (loCustomTypeface != null)
                {
                    _SerialNumberText.Typeface = loCustomTypeface;
                }
                _SerialNumberText.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnTextMessageMedimSizeSp);
                _SerialNumberText.SetTextColor(Resources.GetColor(Resource.Color.civicsmart_black));

                Constants.SERIAL_NUMBER = Helper.GetDeviceUniqueSerialNumber();
                _SerialNumberText.Text = "Unit Serial Number " + Constants.SERIAL_NUMBER;


                _ApplicationRevisionText = FindViewById<TextView>(Resource.Id.textViewVersionNumber);
                // initialize our typeface 
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnWebViewErrorMessageFinePrintTypeface);
                if (loCustomTypeface != null)
                {
                    _ApplicationRevisionText.Typeface = loCustomTypeface;
                }
                _ApplicationRevisionText.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnWebViewErrorMessageFinePringtSizeSp);
                _ApplicationRevisionText.SetTextColor(Resources.GetColor(Resource.Color.civicsmart_black));



                //_ApplicationRevisionText.Click += delegate { ThreadPool.QueueUserWorkItem(o => LogCollector.WriteLogData(this, ApplicationContext)); };





                _btnConfig = FindViewById<Button>(Resource.Id.btnConfig);
                _btnConfig.Click += btnConfigClick;



                _progressbar1 = FindViewById<ProgressBar>(Resource.Id.progressbar1);
                _progressbar1.Visibility = ViewStates.Gone;
                _progressbar1.Indeterminate = false;
                _progressbar1.Max = 7;
                _progressbar1.Progress = 0;




                _btnLogin.Click += btnLoginClick;
                _btnSynchronize.Click += btnSynchronizeClick;

                ////Get the Issuance Info button, and Issuance Info back button, and set the Click handler for them
                ////Jim Crosby 2/3/2015
                //_issuanceInfoButton = FindViewById<Button>(Resource.Id.refissuanceInfoButton);
                //if (_issuanceInfoButton != null)
                //{
                //    _issuanceInfoButton.Click += refIssuanceInfoHandleClick;

                //    // AJW - hide this function until it is fleshed out and presents better
                //    _issuanceInfoButton.Visibility = ViewStates.Gone;
                //}



                //			ActivityManager activityManager = (ActivityManager) GetSystemService(ActivityService);
                //			int memClass = activityManager.MemoryClass;
                //			Log.Debug("****************************", memClass.ToString());
                //			Java.Lang.Runtime rr = Java.Lang.Runtime.GetRuntime ();
                //			long maxMemory = rr.MaxMemory();
                //			Log.Debug("****************************", maxMemory.ToString());

                //_userName.Text = "BEI";
                //_password.Text = "AC";

                _userName.Text = string.Empty;
                _password.Text = string.Empty;


                //			StartService (new Intent (this, typeof(StartupService)));
                _refreshDBReceiver = new RefreshDBReceiver();
                _SyncProgressReceiver = new DisplaySyncProgressReceiver();
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "LoginActivity.OnCreate", ex.TargetSite.Name);
                System.Console.WriteLine("LoginActivity::OnCreate Exception source {0}: {1}", ex.Source, ex.ToString());
            }


        }

        //OnResume life cycle method
        protected override void OnResume()
        {
            base.OnResume(); // Always call the superclass first.
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            string username = prefs.GetString(Constants.username, null);

            // reload the properties, they may have been updated on the config screen
            LoadAppProperties();

            if (username != null)
            {
                StartActivity(typeof(SplashActivity));
                //StartActivity(typeof(MainActivity));
            }


            RegisterReceiver(_refreshDBReceiver, new IntentFilter("Duncan.AI.Droid.RefreshDB"));
            RegisterReceiver(_SyncProgressReceiver, new IntentFilter("Duncan.AI.Droid.SyncProgress"));
        }

        protected override void OnPause()
        {
            base.OnPause();

            UnregisterReceiver(_refreshDBReceiver);
            UnregisterReceiver(_SyncProgressReceiver);

            Log.Debug("Browse", "Receivers unregistered");
        }

        protected override void OnDestroy()
        {
            // clean up
            AndroidEnvironment.UnhandledExceptionRaiser -= HandleAndroidException_LoginActivity;

            base.OnDestroy();

            _SyncScreenActivity = null;
        }

        public void SetProgressBar(int iProgress)
        {
            if (_progressbar1 != null)
            {
                if (iProgress > _progressbar1.Max)
                {
                    iProgress = _progressbar1.Max;
                }

                _progressbar1.Progress = iProgress;
            }

        }



        private bool HaveSufficientInformationForLoginAttempt( )
        {
            // clear any previous errors before we try again
            _userName.Error = null;
            _password.Error = null;



            // some defense first to make sure we're ready to sync
            if (_propertiesDAO == null)
            {
                _userName.SetError("Please configure first.", Resources.GetDrawable(Resource.Drawable.Icon));
                _userName.RequestFocusFromTouch();
                return false;
            }

            if
                (
                  (string.IsNullOrEmpty(_propertiesDAO.url) == true) ||
                  (string.IsNullOrEmpty(_propertiesDAO.name) == true)
                )
            {
                _userName.SetError("Please configure first.", Resources.GetDrawable(Resource.Drawable.Icon));
                _userName.RequestFocusFromTouch();
                return false;

            }

            if (string.IsNullOrEmpty(_userName.Text) == true)
            {
                _userName.SetError("Please enter User Name.", Resources.GetDrawable(Resource.Drawable.Icon));
                _userName.RequestFocusFromTouch();
                return false;
            }


            if (string.IsNullOrEmpty(_password.Text) == true)
            {
                _password.SetError("Please enter password.", Resources.GetDrawable(Resource.Drawable.Icon));
                _password.RequestFocusFromTouch();
                return false;
            }


            // looks like we have enough to try
            return true;
        }


        private void btnLoginClick(object sender, EventArgs e)
        {
            // ok to try?
            if (HaveSufficientInformationForLoginAttempt() == false)
            {
                return;
            }

            // go sync first
            // soon   btnSynchronizeClick(sender, e);



            UserDAO userDAO = ValidateLogin();
            if (userDAO != null)
            {
                _userDAOParent = userDAO;
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                string isGps = prefs.GetString(Constants.IS_GPS, null);

                if (DroidContext.gLocationUpdateDuration > 0)
                {
                    var locationManager = (LocationManager)GetSystemService(LocationService);
                    bool isGPSEnabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
                    if (!isGPSEnabled)
                    {
                        var builder = new AlertDialog.Builder(this);
                        builder.SetTitle("GPS Settings");
                        builder.SetMessage("GPS is not enabled. Do you want to go to settings menu?");
                        builder.SetPositiveButton("Settings", delegate
                        {
                            var intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                            StartActivity(intent);
                        });

                        builder.SetNegativeButton("Cancel", delegate
                        {
                            StartMainActivity();
                        });
                        builder.Show();
                    }
                    else
                    {
                        StartMainActivity();
                    }
                }
                else
                {
                    StartMainActivity();
                }
            }
            else
            {
                _userName.SetError(" Username is incorrect.", Resources.GetDrawable(Resource.Drawable.Icon));
                _password.SetError(" Password is incorrect.", Resources.GetDrawable(Resource.Drawable.Icon));
            }
        }




        //Start Main Activity
        public void StartMainActivity()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(Constants.username, _userName.Text);

            string deviceId = Build.Serial;
            //Log.Debug("Before Build.Serial", deviceId);
            string modDeviceId = Helper.GetSerialNumber(deviceId);
            //Log.Debug("After Build.Serial", modDeviceId);
            editor.PutString(Constants.DEVICEID, modDeviceId);

            if (_userDAOParent != null)
            {
                editor.PutString(Constants.OFFICER_ID, _userDAOParent.officerId);
                editor.PutString(Constants.OFFICER_NAME, _userDAOParent.officerName);
                editor.PutString(Constants.AGENCY, _userDAOParent.agency);
            }
            editor.Apply();


            // let's have a fresh start from boot
            MemoryWatcher.RestartApp(Constants.appsettings_bootmode_value_logon, this, this, 2);

            //StartActivity(typeof(SplashActivity));
        }

        //Validate user login
        public UserDAO ValidateLogin()
        {
            var result = _loginImpl.ValidateLogin(_userName.Text, _password.Text, CancellationToken.None);
            // await! control returns to the caller and the task continues to run on another thread
            UserDAO userDAO =  result;
            return userDAO;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            string deviceId = Build.Serial;
            string modDeviceId = Helper.GetSerialNumber(deviceId);
            menu.Add(0, 0, 0, "Serial Number::" + modDeviceId);
            return true;
        }


        private void btnSynchronizeClick(object sender, EventArgs e)
        {

            // ok to try?
            if (HaveSufficientInformationForLoginAttempt() == false)
            {
                return;
            }




            // isn't this already on the UI thread? Do we need to sleep to make it paint before proceeding?
            //RunOnUiThread(() =>
            //{

            // hide buttons immediately
            _btnLogin.Visibility = ViewStates.Gone;
            _btnSynchronize.Visibility = ViewStates.Gone;
            _btnConfig.Visibility = ViewStates.Gone;

            _SerialNumberText.Visibility = ViewStates.Gone;
            _ApplicationRevisionText.Visibility = ViewStates.Gone;



            // disable inputs
            _userName.Enabled = false;
            _password.Enabled = false;


            _loadingTextView.Text = "Preparing to Synchronize...";
            _loadingTextDetailsView.Text = "";
            _loadingTextView.Visibility = ViewStates.Visible;
            _loadingTextDetailsView.Visibility = ViewStates.Visible;
            //});



            //determine if we can sync by making a few checks
            var continueWithSynch = true;

            ////do we have an xml file on the device?
            //var usersFileName =Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads) + "/" + "ISSUE_AP.XML";
            //if (File.Exists(usersFileName))
            //{
            //    //we have an xml file, lets roll thorugh the structs and see if there are any unsubmitted items 

            //    //if hte xml hasnt been loaded yet, we cant do this
            //    if (DroidContext.XmlCfg != null)
            //    {
            //        if (GetUnSubmittedDataCount() > 0)
            //        {
            //            continueWithSynch = false;
            //        }
            //    }
            //}


            // AJW TODO - should not be possible to wipe out existing data!!

            //hide the sync button if you have any items that have not beed sumbitted yet.
            if (!continueWithSynch)
            {
                _loadingTextView.Visibility = ViewStates.Gone;
                _loadingTextDetailsView.Visibility = ViewStates.Gone;

                // make these available again
                _btnLogin.Visibility = ViewStates.Visible;
                _btnSynchronize.Visibility = ViewStates.Visible;
                _btnConfig.Visibility = ViewStates.Visible;

                _SerialNumberText.Visibility = ViewStates.Visible;
                _ApplicationRevisionText.Visibility = ViewStates.Visible;


                _userName.Enabled = true;
                _password.Enabled = true;




                ////  Toast.MakeText(this, "Synch Unavailable, Pending tickets.", ToastLength.Long).Show();
                //var builder = new AlertDialog.Builder(this);
                //builder.SetTitle("NOTE");
                //builder.SetMessage("Synch process unavailable: Pending tickets. Do you want to clear current user data?");

                //builder.SetPositiveButton("YES", delegate
                //{
                //    DroidContext.XmlCfg = null;
                //    StartService(new Intent(this, typeof(StartupService)));
                //});

                //builder.SetNegativeButton("CANCEL", delegate
                //{
                //    //jsut a cancel button

                //});


                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("NOTE");
                builder.SetMessage("Synch process unavailable: Pending tickets. You must log in and allow pending tickets to upload.");

                builder.SetPositiveButton("OK", delegate
                    {
                    });

                //builder.SetNegativeButton("CANCEL", delegate
                //{
                //    //usut a cancel button

                //});



                builder.Show();
            }
            else
            {
                _btnLogin.Visibility = ViewStates.Gone;
                _btnSynchronize.Visibility = ViewStates.Gone;
                _btnConfig.Visibility = ViewStates.Gone;

                _SerialNumberText.Visibility = ViewStates.Gone;
                _ApplicationRevisionText.Visibility = ViewStates.Gone;


                // disable inputs
                _userName.Enabled = false;
                _password.Enabled = false;


                //if (_issuanceInfoButton != null)
                //{
                //    _issuanceInfoButton.Visibility = ViewStates.Gone;
                //}

                _loadingTextView.Visibility = ViewStates.Visible;
                _loadingTextDetailsView.Visibility = ViewStates.Visible;

                //_progressbar1.Visibility = ViewStates.Visible;
                _progressbar1.Visibility = ViewStates.Gone;


                DroidContext.XmlCfg = null;
                StartService(new Intent(this, typeof(StartupService)));
            }
        }

        private int GetUnSubmittedDataCount()
        {
            int unsubmittedCount = 0;
            //Get the Structs from the XML file
            var structs = DroidContext.XmlCfg.IssStructs;

            //Create the data access object
            var dataAccess = new CommonADO();
            //Loop through the structs. for each of them see what type it is.
            //If it is either:
            //TCiteStruct
            //TNoteStruct
            //TVoidStruct
            //TREissueStruct
            //tactivitylogstruct
          //get the count of unsibmitted tickets. 
            foreach (IssStruct issStruct in structs)
            {
                switch (issStruct.Type.ToLower())
                {
                    case "tcitestruct":
                    case "tnotestruct":
                    // KLUDGE - orphaned ones are getting in the way    
                    //case "tactivitylogstruct":
                    case "tvoidstruct":
                    case "treissuestruct":
                        string viewText = issStruct.Name + "       ";
                        var count = dataAccess.GetRecordCountByWsStatus(issStruct.Name, Constants.WS_STATUS_READY);
                        if (count > 0)
                        {
                            unsubmittedCount += count;
                        }
                        break;
                }
            }

            return unsubmittedCount;
        }

        //Handle Issuance Info button
        //Jim Crosby 2/3/2015
        private void refIssuanceInfoHandleClick(object sender, EventArgs e)
        {
            StartActivity(typeof(Issuance_infoActivity));
            //SetContentView(Resource.Layout.Issuance_info);
        }


        private void btnConfigClick(object sender, EventArgs e)
        {
            StartActivity(typeof(SyncConfigActivity));
        }






        public void LoadAppProperties()
        {
            _propertiesDAO = _loginImpl.RetrieveAppProperties();

            if (_propertiesDAO != null)
            {
                if
                    (
                      (string.IsNullOrEmpty(_propertiesDAO.url) == false) &&
                      (string.IsNullOrEmpty(_propertiesDAO.name) == false)
                    )
                {
                    WebserviceConstants.CLIENT_URL = Helper.BuildAutoISSUEPublicServiceHostURL(_propertiesDAO.url, _propertiesDAO.name);
                    Constants.CLIENT_NAME = _propertiesDAO.name;

                }


                // seperate check so city name changes alone will still reflect right away
                if (string.IsNullOrEmpty(_propertiesDAO.name) == false)
                {
                    if (_cityNameText != null)
                    {
                        // default to web service client name
                        string loDisplayedClientName = Helper.FormatTitleText(_propertiesDAO.name);

                        // KLDUGE! TODO - if we can get it from registry, do it
                        if (loDisplayedClientName.ToUpper().Contains("MIAMI DADE") == true)
                        {
                            loDisplayedClientName = "Miami Dade Clerk's Office";
                        }




                        _cityNameText.Text = loDisplayedClientName;
                    }
                }




                if (string.IsNullOrEmpty(_propertiesDAO.private_url) == false)
                {
                    WebserviceConstants.PRIVATE_CLIENT_URL = _propertiesDAO.private_url;
                }

                Constants.SERIAL_NUMBER = Helper.GetDeviceUniqueSerialNumber();

                if (string.IsNullOrEmpty(_propertiesDAO.PrinterType) == false)
                {
                    Constants.PRINTER_TYPE = _propertiesDAO.PrinterType;
                }

            }

        }


        [BroadcastReceiver]
        [IntentFilter(new String[] { "Duncan.AI.Droid.RefreshDB" })]
        public class RefreshDBReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {

               
                string msg = intent.GetStringExtra("Status");
                if ("Success".Equals(msg))
                {
                    // login disabled until sync is successful?
                    _btnLogin.Visibility = ViewStates.Visible;


                    // AJW - hide this function until it is fleshed out and presents better
                    //_issuanceInfoButton.Visibility = ViewStates.Gone;
                    //_issuanceInfoButton.Visibility = ViewStates.Visible;

                }
                else
                {
                    Toast.MakeText(context, msg, ToastLength.Short).Show();
                }


                _progressbar1.Visibility = ViewStates.Gone;

                _btnSynchronize.Visibility = ViewStates.Visible;
                _btnConfig.Visibility = ViewStates.Visible;


                _loadingTextView.Visibility = ViewStates.Gone;
                _loadingTextDetailsView.Visibility = ViewStates.Gone;

                _SerialNumberText.Visibility = ViewStates.Visible;
                _ApplicationRevisionText.Visibility = ViewStates.Visible;

                // re-enable inputs
                _userName.Enabled = true;
                _password.Enabled = true;



                // AJW - hide this function until it is fleshed out and presents better
                //_issuanceInfoButton.Visibility = ViewStates.Gone;

                //_issuanceInfoButton.Visibility = ViewStates.Visible;


            }
        }

        [BroadcastReceiver]
        [IntentFilter(new String[] { Constants.ACTIVITY_INTENT_DISPLAY_SYNC_PROGRESS_NAME })]
        public class DisplaySyncProgressReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {

                int loProgress = intent.GetIntExtra(Constants.ActivityIntentExtraInt_ProgressValue, 0);
                string loProgressDesc = intent.GetStringExtra(Constants.ActivityIntentExtraInt_ProgressDesc);

                if (_SyncScreenActivity != null)
                {
                    _SyncScreenActivity.RunOnUiThread(() =>
                        {
                            //_progressbar1.Progress = loProgress;

                            //_progressbar1.IncrementProgressBy(1);

                            int loSyncSteps = (int)Constants.SyncStepEnummeration.SyncStep_Complete;

                            string loSyncProgressPercentageText = "";
                            if (loSyncSteps > 0)
                            {
                                // percentage not meaningful when there are only a handful of steps
                                //float percentage = ( (float)loProgress / (float)loSyncSteps );
                                //loSyncProgressPercentageText = String.Format("Synchronization {0:0%} complete.", percentage);

                                loSyncProgressPercentageText = "Synchronization Step " + loProgress.ToString() + " of " + loSyncSteps.ToString();

                            }


                            _loadingTextView.Text = loSyncProgressPercentageText;

                            _loadingTextDetailsView.Text = loProgressDesc;

                        });
                }

            }
        }

    }
}

