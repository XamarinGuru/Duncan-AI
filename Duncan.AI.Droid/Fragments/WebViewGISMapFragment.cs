using System;
using System.Json;
using System.Text;
using Android.App;
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
    public class WebViewGISMapFragment : Fragment
    {

        private WebView _webView = null;
        private GISPayByPlateMapAppInterface _JavascriptAppInterface = null;

        private AICustomWebViewClient _AICustomWebClient = null;

        private View _ErrorView;
        private Button _ReloadButton;
        private Button _ExitToAppButton;
        private ProgressBar _ProgressBar;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //string tag = Constants.GIS_FRAGMENT_TAG;

            //((MainActivity)Activity).RegisterFragment(this, tag);
        }


        public void btnReloadClick(object sender, EventArgs e)
        {
            try
            {
                // only one click per attempt
                _ReloadButton.Visibility = ViewStates.Gone;
                _ExitToAppButton.Visibility = ViewStates.Gone;


                // clear the messages
                _AICustomWebClient.ResetLoadingMessage();


                if (_AICustomWebClient.ErrorOccurredOnLastLoadURLResult() == false)
                {
                    // this only works if the page was loaded successfully in the first place...
                    _webView.LoadUrl("javascript:window.location.reload( true )");
                }
                else
                {
                    // reload from scratch
                    _webView.LoadUrl(_JavascriptAppInterface.GetURLForWebViewPage());
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "WebViewGISMapFragment", "ReloadClick");
                Toast.MakeText(DroidContext.ApplicationContext, "Error in WebView reload", ToastLength.Long).Show();
            }

        }

        void _ExitToAppButton_Click(object sender, EventArgs e)
        {
            DroidContext.mainActivity.RunOnUiThread(() =>
            {
                try
                {
                    // this is just bad form
                    DroidContext.mainActivity.MenuPopUp_IssueFormSelection();
                }
                catch (Exception ex)
                {
                    LoggingManager.LogApplicationError(ex, "WebViewMapFragment", "_ExitToAppButton_Click");
                    Toast.MakeText(DroidContext.ApplicationContext, "Error in WebViewMapFragment", ToastLength.Long).Show();
                }
            });

        }



        public override void OnStart()
        {
            base.OnStart();

            // (re)load the web page
            //_webView.LoadUrl(_JavascriptAppInterface.GetURLForWebViewPage());

            //Helper.HideKeyboardFromFragment(this.Activity);

            this.Activity.RunOnUiThread(() =>
            {
                //Helper.HideKeyboardFromActivity(this);

                Helper.HideKeyboardFromFragment(this.Activity);
            });



        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // AJW - always call the superclass
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.WebViewGISMapFragment, null);
            _webView = view.FindViewById<WebView>(Resource.Id.WebViewGISMap);

            // the java application interface class
            _JavascriptAppInterface = new GISPayByPlateMapAppInterface(Activity, FragmentManager);
            _JavascriptAppInterface.InitURLForWebViewPage();


            // find the parent container of the error display
            _ErrorView = view.FindViewById(Resource.Id.ErrorDisplayPayBySpaceMap);
            _ReloadButton = view.FindViewById<Button>(Resource.Id.ButtonReloadPayBySpaceMap);
            _ExitToAppButton = view.FindViewById<Button>(Resource.Id.ButtonExitToApp);
            _ProgressBar = view.FindViewById<ProgressBar>(Resource.Id.WebViewGISMapProgressBar);


            Helper.SetTypefaceForButton(_ReloadButton, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);

            //  set up the reload button. This is only displayed in the event of a failure when loading the page initially
            // once the page is loaded it has its own reload button and the parent view of this will be hidden
            _ReloadButton.Click += btnReloadClick;

            _ExitToAppButton.Click += _ExitToAppButton_Click;



            // DPANDROID-79
            // to avoid having the browser take over the URL and launch instead of the web view,
            // just implement the web client and set it before loadUrl. The simplest way is:
            //_webView.SetWebViewClient(new WebViewClient() );

            _AICustomWebClient = new AICustomWebViewClient(this.Activity, _ErrorView, _ProgressBar, _ReloadButton, _ExitToAppButton);
            _webView.SetWebViewClient(_AICustomWebClient);

            if (DroidContext.UseChromeWebViewClient() == true)
            {
                //supplying the default WebChromeClient() is enough to get JavaScript alert() to work.
                //You don't need to override the onJsAlert() function.
                //You need to supply a WebChromeClient(), the default one will do, i.e.
                // with this we get debug messages ! 
                _webView.SetWebChromeClient(new WebChromeClient());
            }


            //We were able to use a Google Map with markers in a WebView without crashes by falling back to software rendering for now:
            //But this will likely hurt your WebView's performance.
            //_webView.SetLayerType(LayerType.Software, null);



            // javascript required for our pages
            _webView.Settings.JavaScriptEnabled = true;

            //"Android" is the name of the class called from the web view in Javascript (I.E. Android.LoadPayByPlateInformation(data);)
            _webView.AddJavascriptInterface(_JavascriptAppInterface, "Android");

            // clear any old cached web data
            //_webView.ClearCache(true /* clear disk files*/ );
            _AICustomWebClient.ClearApplicationWebViewCachesOnceAtBoot(this.Activity, _webView);



            try
            {
                // TODO - make sure the internet is available before attempting to load, and let them come back later to try again
                // now we can safely load the URL
                _webView.LoadUrl(_JavascriptAppInterface.GetURLForWebViewPage());

            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "WebViewGISMapFragment", "OnCreateView");
                Toast.MakeText(DroidContext.ApplicationContext, "Error in WebView create", ToastLength.Long).Show();
            }


            return view;
        }

    }


    //create a class for the webview interface for the GIS map. 
    public class GISPayByPlateMapAppInterface : GISWebViewBaseAppInterface
    {

        /** Instantiate the interface and set the context */
        public GISPayByPlateMapAppInterface(Activity context, FragmentManager fragmentManager)
            : base(context, fragmentManager)
        {
        }


        /// <summary>
        /// Attempt to get webview URL from registry
        /// </summary>
        /// <returns></returns>
        public override bool InitURLForWebViewPage()
        {


            // gets specific URL info from registry
            string loBaseURL = TTRegistry.glRegistry.GetRegistryValue(
                                  TTRegistry.regSECTION_ISSUE_AP,
                                  TTRegistry.regPAYBYSPACE_WEBVIEW_MAP_URL_BASE,
                                  TTRegistry.regPAYBYSPACE_WEBVIEW_MAP_URL_BASE_DEFAULT);



            string loCustomerIDAsString = TTRegistry.glRegistry.GetRegistryValue(
                                                        TTRegistry.regSECTION_ISSUE_AP,
                                                        TTRegistry.regPAYBYSPACE_WEBVIEW_MAP_CUSTID,
                                                        TTRegistry.regPAYBYSPACE_WEBVIEW_MAP_CUSTID_DEFAULT);


            string loCityCodeParameter = "?cityid=";

            // if there is any ID at all, it needs to be formatted
            if (loCustomerIDAsString.Length > 0)
            {
                loCustomerIDAsString = loCityCodeParameter + loCustomerIDAsString;
            }


            _WebViewURL = Helper.UrlCombine(loBaseURL, loCustomerIDAsString);


            // return false is not found in registry ?

            return true;
        }


        private static int _testIdx = 0;

        [Export] // !!! do not work without Export
        [JavascriptInterface] // This is also needed in API 17+
        // to become consistent with Java/JS interop convention, the argument cannot be System.String. 
        public void LoadMeterInformation(string meterInfo)
        {
            //// test
            //NavigateToIssueFormSelectionMenu(meterInfo);
            //return;

            //fail checks first
            if (string.IsNullOrEmpty(meterInfo))
            {
                Toast.MakeText(_context, "No meter data supplied.", ToastLength.Long).Show();
                return;
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


                if (loNoEnforcementDataPassed == true)
                {
                    // no data - show em menu
                   ShowMenuPopUp_IssueFormSelection(Constants.GIS_MAP_FRAGMENT_TAG);

                    //if ((_testIdx & 1) > 0)
                    //{
                    //   // NavigateToPayByPlateListWebView(string.Empty);
                    //    NavigateToGISMapWebView(string.Empty);
                    //}
                    //else
                    //{
                    //    NavigateToPayBySpaceListWebView(string.Empty);
                    //}
                    //_testIdx++;

                }
                else
                {
                    // they passed data - do the enforcement
                    SwitchToParkingFormForEnforcement(Constants.GIS_MAP_FRAGMENT_TAG);
                }




                //_context.RunOnUiThread(() => this.SwitchToParkingFormForEnforcement();

            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "PayBySpaceMapFragment", "LoadMeterInformation");
                Toast.MakeText(_context, "Error loading ticket data.", ToastLength.Long).Show();
            }

        }

        [Export] // !!! do not work without Export
        [JavascriptInterface] // This is also needed in API 17+
        // to become consistent with Java/JS interop convention, the argument cannot be System.String. 
        public void NavigateToParkingIssueForm(string iJSONWebViewParameters)
        {
            try
            {
                // did they pass something to enforce with?
                if (string.IsNullOrEmpty(iJSONWebViewParameters) == false)
                {
                    // if they passed meter data, use it to enforce 
                    LoadMeterInformation(iJSONWebViewParameters);
                    return;
                }

                // no meter selected, just want to write a ticket without web data
                ExternalEnforcementInterfaces.ClearGISMeterJsonValueObject();
                SwitchToParkingFormForEnforcement(Constants.GIS_MAP_FRAGMENT_TAG);
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "WebViewMapFragment", "NavigateToParkingIssueForm");
                Toast.MakeText(_context, "Error in WebViewMapFragment", ToastLength.Long).Show();
            }
        }


    }

}