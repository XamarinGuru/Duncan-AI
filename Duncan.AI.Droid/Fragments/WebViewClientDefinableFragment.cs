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
    public class WebViewClientDefinableFragment : Fragment
    {

        private WebView _webView = null;
        private WebViewClientDefinableFragmentAppInterface _JavascriptAppInterface = null;

        private AICustomWebViewClient _AICustomWebClient = null;


        private string _tagName;

        private int _WebView_Client_Definable_Index_Number = 0;  // 1 and up are valid instances

        private View _ErrorView;
        private Button _ReloadButton;
        private Button _ExitToAppButton;
        private ProgressBar _ProgressBar;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _WebView_Client_Definable_Index_Number = Arguments.GetInt(Constants.WebView_Client_Definable_Index_Parameter_Name, 0);

            _tagName = Helper.BuildWebViewClientDefinableTag(_WebView_Client_Definable_Index_Number);
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
                LoggingManager.LogApplicationError(ex, "WebClientDefinableFragment", "btnReloadClick");
                Toast.MakeText(DroidContext.ApplicationContext, "Error in WebView Reload", ToastLength.Long).Show();
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

            View view = inflater.Inflate(Resource.Layout.WebViewClientDefinableFragment1, null);
            _webView = view.FindViewById<WebView>(Resource.Id.WebViewGISMap);

            // the java application interface class
            _JavascriptAppInterface = new WebViewClientDefinableFragmentAppInterface(Activity, FragmentManager, _WebView_Client_Definable_Index_Number);
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
                LoggingManager.LogApplicationError(ex, "WebClientDefinableFragment", "OnCreateView");
                Toast.MakeText(DroidContext.ApplicationContext, "Error in WebView create", ToastLength.Long).Show();
            }


            return view;
        }

    }


    //create a class for the webview interface for the GIS map. 
    public class WebViewClientDefinableFragmentAppInterface : GISWebViewBaseAppInterface
    {
        private int _WebViewClientDefinedIndex = 0;


        /** Instantiate the interface and set the context */
        public WebViewClientDefinableFragmentAppInterface(Activity context, FragmentManager fragmentManager, int iWebViewClientDefinedIndex )
            : base(context, fragmentManager)
        {
            _WebViewClientDefinedIndex =  iWebViewClientDefinedIndex;
        }


        /// <summary>
        /// Attempt to get webview URL from registry
        /// </summary>
        /// <returns></returns>
        public override bool InitURLForWebViewPage()
        {


            string loIdxStr = _WebViewClientDefinedIndex.ToString();



            // gets specific URL info from registry
            string loBaseURL = TTRegistry.glRegistry.GetRegistryValue(
                                  TTRegistry.regSECTION_ISSUE_AP,
                                  TTRegistry.regWEBVIEW_MENU_ITEM_N_PREFIX + loIdxStr +
                                  TTRegistry.regWEBVIEW_MENU_ITEM_N_URL_BASE_SUFFIX,
                                  TTRegistry.regWEBVIEW_MENU_ITEM_N_URL_BASE_DEFAULT);



            // get parameters
            string loParameterName1AsString = TTRegistry.glRegistry.GetRegistryValue(
                                                        TTRegistry.regSECTION_ISSUE_AP,
                                                        TTRegistry.regWEBVIEW_MENU_ITEM_N_PREFIX + loIdxStr +
                                                        TTRegistry.regWEBVIEW_MENU_ITEM_N_PARAM_NAME_1_SUFFIX,
                                                        TTRegistry.regWEBVIEW_MENU_ITEM_N_PARAM_NAME_1_DEFAULT);


            string loParameterValue1AsString = TTRegistry.glRegistry.GetRegistryValue(
                                                        TTRegistry.regSECTION_ISSUE_AP,
                                                        TTRegistry.regWEBVIEW_MENU_ITEM_N_PREFIX + loIdxStr +
                                                        TTRegistry.regWEBVIEW_MENU_ITEM_N_PARAM_VALUE_1_SUFFIX,
                                                        TTRegistry.regWEBVIEW_MENU_ITEM_N_PARAM_VALUE_1_DEFAULT);


            string loParameterName2AsString = TTRegistry.glRegistry.GetRegistryValue(
                                                        TTRegistry.regSECTION_ISSUE_AP,
                                                         TTRegistry.regWEBVIEW_MENU_ITEM_N_PREFIX + loIdxStr +
                                                        TTRegistry.regWEBVIEW_MENU_ITEM_N_PARAM_NAME_2_SUFFIX,
                                                        TTRegistry.regWEBVIEW_MENU_ITEM_N_PARAM_NAME_2_DEFAULT);


            string loParameterValue2AsString = TTRegistry.glRegistry.GetRegistryValue(
                                                        TTRegistry.regSECTION_ISSUE_AP,
                                                        TTRegistry.regWEBVIEW_MENU_ITEM_N_PREFIX + loIdxStr +
                                                        TTRegistry.regWEBVIEW_MENU_ITEM_N_PARAM_VALUE_2_SUFFIX,
                                                        TTRegistry.regWEBVIEW_MENU_ITEM_N_PARAM_VALUE_2_DEFAULT);


            string loParameterPrefix = "?";
            StringBuilder loParameters = new StringBuilder();


            if (string.IsNullOrEmpty(loParameterName1AsString) == false)
            {
                loParameters.Append(loParameterPrefix + loParameterName1AsString + "=" + EvaluateWebViewParameter(loParameterValue1AsString));
                loParameterPrefix = "&";
            }


            if (string.IsNullOrEmpty(loParameterName2AsString) == false)
            {
                loParameters.Append(loParameterPrefix + loParameterName2AsString + "=" + EvaluateWebViewParameter(loParameterValue2AsString));
                loParameterPrefix = "&";
            }


            _WebViewURL = Helper.UrlCombine(loBaseURL, loParameters.ToString());

            return true;
        }

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
                //    LoggingManager.LogApplicationError(ex, "WebViewClientDefinableFragment: Double Parse", "LoadMeterInformation");
                //}

                if (jsonMeter == null)
                {
                    Toast.MakeText(_context, "Invalid format: meter data.", ToastLength.Long).Show();
                    LoggingManager.LogApplicationError(null, "WebViewClientDefinableFragment: Invalid format: meter data", "LoadMeterInformation");

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
                LoggingManager.LogApplicationError(ex, "WebViewClientDefinableFragment", "LoadMeterInformation");
                Toast.MakeText(_context, "Error loading enforcement data.", ToastLength.Long).Show();
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