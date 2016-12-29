using System;
using System.Json;
using System.Text;
using Android.App;
using Android.Graphics;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Webkit;
using Android.Widget;

using Reino.ClientConfig;
using Duncan.AI.Droid.Common;
using Duncan.AI.Droid.Utils.HelperManagers;


using System.Collections.Generic;
using System.Threading.Tasks;

using Java.Interop;


namespace Duncan.AI.Droid.Fragments
{
    public class AICustomWebViewClient : WebViewClient
    {


        //As user clicks 'Enforce' : new JSON
        //{"state":"LA","lpn":"WNK572","zId":29294,"zText":"Poydras 1300","txId":39780,"custId ":"4176","expt":111719590,"internalTranId":"4"}
        //new field added : ,"internalTranId":"4"
        //PbpRefresh : 
        //URL : http://106.51.253.214/PaybyPlate/Pbp/pbpRefresh/<<internalTranId>>
        //Response JSON :
        //{"InternalTranId":1,"customerId":4176,"ZoneId":2901,"ProviderTranId":0,"LPN":"adsfds","State":"AL","Location":"Aline 1300","Expt":null,"NoPaymentPlate":true,"DateTime":"\/Date(1459920649240)\/","Status":2}
        //public enum Status : int
        //{ RESPONSE_NO_DATA_FOUND_DB = 0, RESPONSE_NO_DATA_FOUND_API = 1, RESPONSE_OK = 2, RESPONSE_NO_PAYMENT_FOUND = 3, RESPONSE_ERROR= 4, }
        //For NO-Payment_Found still i am sending Status: RESPONSE_OK = 2,



        private Context _context;

        private ProgressBar _ProgressBar;
        private View _ErrorViewParent;
        private Button _ReloadButton;
        private Button _ExitToAppButton;
        private TextView _ErrorMsg;
        private TextView _ErrorMsgDetail;
        private TextView _ErrorMsgFinePrint;
        private View _ErrorView;


        private const string _WebTitleToTrap = "WEBPAGE NOT AVAILABLE";


        private bool _ErrorHasOccurred = false;
        public bool ErrorOccurredOnLastLoadURLResult()
        {
            return _ErrorHasOccurred;
        }

        private ClientError _LastURLResultCode = ClientError.Unknown;
        public ClientError GetLastLoadURLResulCode()
        {
            return _LastURLResultCode;
        }

        public AICustomWebViewClient(Context iContext, View iErrorViewParent, ProgressBar iProgressBar, Button iReloadButton, Button iExitToAppButton)
            : base()
        {
            _context = iContext;

            _ErrorHasOccurred = false;
            _LastURLResultCode = ClientError.Unknown;

            _ErrorViewParent = iErrorViewParent;

            _ReloadButton = iReloadButton;
            _ExitToAppButton = iExitToAppButton;
            _ProgressBar = iProgressBar;

            _ErrorMsg = _ErrorViewParent.FindViewById<TextView>(Resource.Id.ErrorMessageHeader);
            _ErrorMsgDetail = _ErrorViewParent.FindViewById<TextView>(Resource.Id.ErrorMessageDetail);
            _ErrorMsgFinePrint = _ErrorViewParent.FindViewById<TextView>(Resource.Id.ErrorMessageFinePrint);



            // initialize our typefaces
            Typeface loCustomTypeface;

            if (_ErrorMsg != null)
            {
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnWebViewErrorMessageTypeface);
                if (loCustomTypeface != null)
                {
                    _ErrorMsg.Typeface = loCustomTypeface;
                }
                _ErrorMsg.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnWebViewErrorMessageSizeSp);
            }


            if (_ErrorMsgDetail != null)
            {
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnWebViewErrorMessageDetailTypeface);
                if (loCustomTypeface != null)
                {
                    _ErrorMsgDetail.Typeface = loCustomTypeface;
                }
                _ErrorMsgDetail.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnWebViewErrorMessageDetailSizeSp);
            }


            if (_ErrorMsgFinePrint != null)
            {
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnWebViewErrorMessageFinePrintTypeface);
                if (loCustomTypeface != null)
                {
                    _ErrorMsgFinePrint.Typeface = loCustomTypeface;
                }
                _ErrorMsgFinePrint.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnWebViewErrorMessageFinePringtSizeSp);
            }




            _ReloadButton.Visibility = ViewStates.Gone;
            _ReloadButton.Enabled = false;

            _ExitToAppButton.Visibility = ViewStates.Gone;
            _ExitToAppButton.Enabled = false;

        }


        /// <summary>
        /// reset the loading message for a reload retry
        /// </summary>
        public void ResetLoadingMessage()
        {
            // not needed for javascript reloads
            if (_ErrorViewParent.Visibility == ViewStates.Visible)
            {
                _ErrorMsg.Text = "Loading...";
                _ErrorMsgDetail.Text = "";
                _ErrorMsgFinePrint.Text = "";
            }
        }


        /// <summary>
        /// Clears the application's web view caches
        /// 
        /// So titled because the web cache data is shared per application. so we don't want to do this for every web view created, 
        /// so only the first call executes the 
        /// 
        /// 
        /// </summary>
        public void ClearApplicationWebViewCachesOnceAtBoot(Context iContext, WebView iWebView)
        {
            try
            {
                lock (DroidContext.gWebViewCacheAccessLock)
                {
                    if (DroidContext.gWebViewCacheHasBeenCleared == false)
                    {
                        // some of these are probably not needed, but lets go ahead for thoroughness
                        iWebView.ClearHistory();
                        iWebView.ClearSslPreferences();
                        iWebView.ClearFormData();
                        iWebView.ClearMatches();


                        // clear any old cached web data - this is at an application level, 
                        // so we only want to do the once
                        iWebView.ClearCache(true /* clear disk files*/ );


                        // remove the db files
                        try
                        {
                            iContext.DeleteDatabase("webview.db");
                        }
                        catch (Exception exp)
                        {
                        }

                        try
                        {
                            iContext.DeleteDatabase("webviewCache.db");
                        }
                        catch (Exception exp)
                        {
                        }

                        // crumble up the cookies
                        //CookieSyncManager.CreateInstance(DroidContext.ApplicationContext);
                        //CookieManager cookieManager = CookieManager.Instance;
                        //cookieManager.RemoveAllCookies(null);


                        if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
                        {
                            CookieManager.Instance.RemoveAllCookies(null);
                            CookieManager.Instance.Flush();
                        }
                        else
                        {
                            CookieSyncManager cookieSyncMngr = CookieSyncManager.CreateInstance(DroidContext.ApplicationContext);
                            cookieSyncMngr.StartSync();
                            CookieManager cookieManager = CookieManager.Instance;
                            cookieManager.RemoveAllCookie();
                            cookieManager.RemoveSessionCookie();
                            cookieSyncMngr.StopSync();
                            cookieSyncMngr.Sync();
                        }



                        // all done
                        DroidContext.gWebViewCacheHasBeenCleared = true;
                    }
                }
            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "ClearApplicationWebViewCachesOnceAtBoot Exception", exp.TargetSite.Name);
            }



        }




        //public bool isOnline()
        //{
        //    ConnectivityManager cm = (ConnectivityManager)getBaseContext()
        //            .getSystemService(Context.ConnectivityService);

        //    NetworkInfo i = cm.getActiveNetworkInfo();
        //    if ((i == null) || (!i.isConnected()))
        //    {
        //        Toast toast = Toast.MakeText(GetBaseContext(),
        //                "Error: No connection to Internet", Toast.LENGTH_SHORT);
        //        toast.SetGravity(Gravity.TOP | Gravity.CENTER, 0, 0);
        //        toast.Show();
        //        return false;
        //    }
        //    return true;
        //}

        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            //  only if we want to redirect..... view.LoadUrl(url);
            return true;
        }


        public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
        {
            // reset for each try
            _ErrorHasOccurred = false;
            _LastURLResultCode = ClientError.Unknown;

            if (_ProgressBar != null)
            {
                _ProgressBar.Visibility = ViewStates.Visible;
            }

            base.OnPageStarted(view, url, favicon);
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);


            // is this an error page? this catches server level errors in a kludgy way.... until API23 / 6.0 can be used
            if (
                (view.Title.Trim().Length == 0) ||
                (view.Title.ToUpper().Equals(_WebTitleToTrap) == true))
            {
                // if this wasn't caught already
                if (_ErrorHasOccurred == false)
                {

                    ClientError loErrorCode = ClientError.HostLookup;
                    string loDescription = "Webpage not available.";
                    string loFailingURL = url;

                    // call common error display
                    DisplayErrorMessagesAndReloadOption(view, loErrorCode, loDescription, loFailingURL);
                }
            }


            if (_ProgressBar != null)
            {
                _ProgressBar.Visibility = ViewStates.Gone;
            }

            if (_ErrorHasOccurred == false)
            {
                // hide the error view
                if (_ErrorViewParent != null)
                {
                    _ErrorViewParent.Visibility = ViewStates.Gone;
                }

                // show the webview
                view.Visibility = ViewStates.Visible;
            }


            // put the keyboard away
            if (_context != null)
            {
                Helper.HideKeyboardFromFragment(_context);
            }

        }

        /// <summary>
        /// Common method to present error messages and display the reload button
        /// </summary>
        private void DisplayErrorMessagesAndReloadOption(WebView view, ClientError errorCode, string description, string failingUrl)
        {
            // hide progress bar
            _ProgressBar.Visibility = ViewStates.Gone;

            // warning: This error handling can be called from the javascipt code, 
            // we need to to evaluate what the error is, because it might be related to the enforcement code and not just the connection

            _LastURLResultCode = errorCode;


            if ((_ErrorViewParent != null) && (_LastURLResultCode != ClientError.Unknown))
            {
                _ErrorHasOccurred = true;

                // hide the webview
                view.Visibility = ViewStates.Gone;

                //TextView _ErrorMsg = _ErrorViewParent.FindViewById<TextView>(Resource.Id.ErrorMessageHeader);
                //TextView _ErrorMsgDetail = _ErrorViewParent.FindViewById<TextView>(Resource.Id.ErrorMessageDetail);
                //TextView _ErrorMsgFinePrint = _ErrorViewParent.FindViewById<TextView>(Resource.Id.ErrorMessageFinePrint);

                _ErrorMsg.Text = "An error has occurred";
                _ErrorMsgDetail.Text = errorCode.ToString() + " " + description;
                _ErrorMsgFinePrint.Text = "Please check the internet connection and try again.";

                // the reload button is invisible on very first execution, make visible 
                _ReloadButton.Visibility = ViewStates.Visible;
                _ExitToAppButton.Visibility = ViewStates.Visible;

                // enabled until they press it again
                _ReloadButton.Enabled = true;
                _ExitToAppButton.Enabled = true;


                // display the error message
                _ErrorViewParent.Visibility = ViewStates.Visible;


            }

        }

        public override void OnReceivedSslError(WebView view, SslErrorHandler handler, Android.Net.Http.SslError error)
        {
            base.OnReceivedSslError(view, handler, error);


            ClientError loErrorCode = ClientError.FailedSslHandshake;
            string loDescription = error.ToString();
            string loFailingURL = error.Url;

            // call common error display
            DisplayErrorMessagesAndReloadOption(view, loErrorCode, loDescription, loFailingURL);

        }
        //        // we cant use this until API 23 - Android 6
        //        public void onReceivedHttpError(WebView view, WebResourceRequest request, WebResourceResponse errorResponse)
        //        {
        //        }
        ////  Ref: https://developer.android.com/intl/es/reference/android/webkit/WebViewClient.html#onReceivedHttpError%28android.webkit.WebView,%20android.webkit.WebResourceRequest,%20android.webkit.WebResourceResponse%29





        public override void OnReceivedError(WebView view, ClientError errorCode, string description, string failingUrl)
        {
            base.OnReceivedError(view, errorCode, description, failingUrl);

            // call common error display
            DisplayErrorMessagesAndReloadOption(view, errorCode, description, failingUrl);
        }

    }


    public class GISWebViewBaseAppInterface : Java.Lang.Object
    {
        protected readonly Activity _context;
        protected readonly FragmentManager _fragmentManager;


        /// <summary>
        /// Instantiate the interface and set the context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fragmentManager"></param>
        public GISWebViewBaseAppInterface(Activity context, FragmentManager fragmentManager)
        {
            this._context = context;
            _fragmentManager = fragmentManager;
        }


        /// <summary>
        /// WebView URL. Default set in descendant classes; populated at runtime from registry
        /// </summary>
        protected string _WebViewURL = "";

        /// <summary>
        /// Attempt to get webview URL from registry, return false if value not found in config
        /// </summary>
        /// <returns></returns>
        public virtual bool InitURLForWebViewPage()
        {
            // base class reset only - descendant implementation gets specific URL from registry
            _WebViewURL = "";
            return true;
        }


        // helper method to convert parameters into current system values (as needed)
        public string EvaluateWebViewParameter(string iWebViewParameterSource)
        {
            // look for the prefix
            if (iWebViewParameterSource.StartsWith(TTRegistry.cnWebViewParameterSystemPrefix) == false)
            {
                // no specialty handling, just return original value
                return iWebViewParameterSource;
            }



            switch (iWebViewParameterSource)
            {
                case TTRegistry.cnWebViewParameterCityId:
                    {

                        // for now this is defined in PEMS, not in XML, so use reg value
                        return iWebViewParameterSource;
                    }


                case TTRegistry.cnWebViewParameterOfficerId:
                    {
                        ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
                        string officerId = prefs.GetString(Constants.OFFICER_ID, null);
                        return officerId;
                    }

                default:
                    {
                        return iWebViewParameterSource;
                    }
            }


        }




        private int _loadCounter = 0;
        public string GetURLForWebViewPage()
        {

#if _unit_test_force_error
            // for debug - force error on first few trys
            _loadCounter++;
            if ( _loadCounter < 5 )
            {
                return "http://this.isgointobeanerrorforsure" + _loadCounter.ToString() + ".com";
            }
#endif


            return _WebViewURL;
        }

        // AJW - need run this on UI thread and wait for it to finish
        private async Task StartTicketLayoutOnUIThread(CommonFragment oneFragment)
        {
            int counter = 0;
            counter++;


            // - is this really waiting for task to be done, or is it just coming back immediately because StartTicketLauout is async?
            await Task.Run(() =>
            {
                //_context.RunOnUiThread(() => oneFragment.StartTicketLayout());

                Task.Run(() => _context.RunOnUiThread(() => oneFragment.StartTicketLayout())).ContinueWith(result => counter++);


                //    Task.Run(() => _context.RunOnUiThread() ) => oneFragment.StartTicketLayout()).ContinueWith(result => RunOnUiThread(() => oneFragment.StartTicketLayout()));

            });


            if (counter > 0)
            {
                switch (counter)
                {
                    case 0:
                        {
                            counter++;
                            break;
                        }
                    default:
                        {
                            counter--;
                            break;
                        }
                }
            }
        }



        private async Task SwitchToParkingEnforcementOnUIThread(string iSwitchingFromFragmentTag)
        {

            try
            {

                _context.RunOnUiThread(() =>
                {
                    //now send the user to the parking form
                    const string formNameToUse = "PARKING";

                    // do this after each ticket is completed and saved... if we do it here, we'll be skipping issue numbers
                    //not here...  DroidContext.ResetControlStatusByStructName(formNameToUse);

                    // but we do need to make these are cleared since we are starting a issue.... TODO - consolidate all of this logic
                    ((MainActivity)_context).ResetIssueSourceReferences(((MainActivity)_context).gParkingItemFragmentTag);


                    //CommonFragment dtlFragment = (CommonFragment)((MainActivity)_context).FindRegisteredFragment(formNameToUse);

                    // here comes the new ticket
                    //dtlFragment.StartTicketLayout();

                    // set the selected navigation back to the parking form. we're on the UI thread for it to work.
                    // is it bad form to cast the context like this?
                    ((MainActivity)_context).ChangeToParkingFragment();

                });

            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "WebView Enforcement", "SwitchToParkingFormForEnfocement: " + ex.Message);
                Toast.MakeText(_context, "Error switching to enforcement.", ToastLength.Long).Show();
            }

        }



        public void SwitchToParkingFormForEnforcement(string iSwitchingFromFragmentTag)
        {
            try
            {

                SwitchToParkingEnforcementOnUIThread(iSwitchingFromFragmentTag);
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "GISMapFragment", "SwitchToParkingFormForEnfocement");
                Toast.MakeText(_context, "Error switching to enforcement.", ToastLength.Long).Show();
            }

        }



        private async Task ShowMenuPopUp_IssueFormSelectionOnUIThread(string iSwitchingFromFragmentTag)
        {
            try
            {
                _context.RunOnUiThread(() =>
                {

                    // bad form to cast the context like this?
                    ((MainActivity)_context).MenuPopUp_IssueFormSelection();

                });

            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "WebView Enforcement", "ShowMenuPopUp_IssueFormSelection: " + ex.Message);
                Toast.MakeText(_context, "Error switching to menu.", ToastLength.Long).Show();
            }

        }



        public void ShowMenuPopUp_IssueFormSelection(string iSwitchingFromFragmentTag)
        {
            try
            {

                ShowMenuPopUp_IssueFormSelectionOnUIThread(iSwitchingFromFragmentTag);
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "GISMapFragment", "ShowMenuPopUp_IssueFormSelection");
                Toast.MakeText(_context, "Error switching to menu.", ToastLength.Long).Show();
            }

        }


        [Export] // !!! do not work without Export
        [JavascriptInterface] // This is also needed in API 17+
        // to become consistent with Java/JS interop convention, the argument cannot be System.String. 
        public void NavigateToIssueFormSelectionMenu(string iJSONWebViewParameters)
        {

            DroidContext.mainActivity.RunOnUiThread(() =>
            {
                try
                {
                    //// did they pass something to enforce with?
                    //if (string.IsNullOrEmpty(iJSONWebViewParameters) == false)
                    //{

                    //}

                    // there was nothing selected, clear
                    ExternalEnforcementInterfaces.ClearGISMeterJsonValueObject();

                    // let them choose a destination
                    DroidContext.mainActivity.MenuPopUp_IssueFormSelection();
                }
                catch (Exception ex)
                {
                    LoggingManager.LogApplicationError(ex, "WebViewMapFragment", "NavigateToIssueFormSelectionMenu");
                    Toast.MakeText(_context, "Error in WebViewMapFragment", ToastLength.Long).Show();
                }
            });
        }




        [Export] // !!! do not work without Export
        [JavascriptInterface] // This is also needed in API 17+
        // to become consistent with Java/JS interop convention, the argument cannot be System.String. 
        public void NavigateToGISMapWebView(string iJSONWebViewParameters)
        {
            try
            {
                DroidContext.mainActivity.ChangeToTargetFragmentTag(Constants.GIS_MAP_FRAGMENT_TAG);
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "WebViewMapFragment", "NavigateToPayBySpaceListWebView");
                Toast.MakeText(_context, "Error in WebViewMapFragment", ToastLength.Long).Show();
            }
        }



        [Export] // !!! do not work without Export
        [JavascriptInterface] // This is also needed in API 17+
        // to become consistent with Java/JS interop convention, the argument cannot be System.String. 
        public void NavigateToPayBySpaceListWebView(string iJSONWebViewParameters)
        {
            try
            {
                DroidContext.mainActivity.ChangeToTargetFragmentTag(Constants.GIS_PAYBYSPACE_LIST_FRAGMENT_TAG);
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "WebViewMapFragment", "NavigateToPayBySpaceListWebView");
                Toast.MakeText(_context, "Error in WebViewMapFragment", ToastLength.Long).Show();
            }
        }


        [Export] // !!! do not work without Export
        [JavascriptInterface] // This is also needed in API 17+
        // to become consistent with Java/JS interop convention, the argument cannot be System.String. 
        public void NavigateToPayByPlateListWebView(string iJSONWebViewParameters)
        {
            try
            {
                DroidContext.mainActivity.ChangeToTargetFragmentTag(Constants.GIS_PAYBYPLATE_LIST_FRAGMENT_TAG);
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "WebViewMapFragment", "NavigateToPayByPlateListWebView");
                Toast.MakeText(_context, "Error in WebViewMapFragment", ToastLength.Long).Show();
            }
        }

    


        [Export] // !!! do not work without Export
        [JavascriptInterface] // This is also needed in API 17+
        // to become consistent with Java/JS interop convention, the argument cannot be System.String. 
        public void NavigateToCameraAction(string iJSONWebViewParameters)
        {


            DroidContext.mainActivity.RunOnUiThread(() =>
            {
                try
                {
                    // let them choose a destination
                    DroidContext.mainActivity.MenuPopUp_LaunchCamera();
                }
                catch (Exception ex)
                {
                    LoggingManager.LogApplicationError(ex, "WebViewMapFragment", "NavigateToCameraAction");
                    Toast.MakeText(_context, "Error in WebViewMapFragment.NavigateToCameraAction", ToastLength.Long).Show();
                }
            });
        }



        [Export] // !!! do not work without Export
        [JavascriptInterface] // This is also needed in API 17+
        // to become consistent with Java/JS interop convention, the argument cannot be System.String. 
        //public JsonObject GetDeviceCurrentLocation(string iJSONWebViewParameters)
        public string GetDeviceCurrentLocation(string iJSONWebViewParameters)
        {
           // DroidContext.mainActivity.RunOnUiThread(() =>
            {
                try
                {
                    JsonObject loLocationResult = new JsonObject();
                    Android.Locations.Location oneCurrentLocation = LocationUpdateListener.GetLastUpdatedLocation();

                    if (oneCurrentLocation != null)
                    {
                        loLocationResult.Add("LAT", oneCurrentLocation.Latitude.ToString());
                        loLocationResult.Add("LONG", oneCurrentLocation.Longitude.ToString());
                    }

                    return loLocationResult.ToString();
                }
                catch (Exception ex)
                {
                    LoggingManager.LogApplicationError(ex, "WebViewMapFragment", "GetDeviceCurrentLocation");
                    Toast.MakeText(_context, "Error in WebViewMapFragment.GetDeviceCurrentLocation", ToastLength.Long).Show();
                    return "GetDeviceCurrentLocation Error: " + ex.Message;
                }
            }  //);
        }

    }

}

