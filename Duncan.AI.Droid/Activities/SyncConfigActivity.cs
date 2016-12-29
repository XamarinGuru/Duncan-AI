using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Duncan.AI.Droid.Managers;
using XMLConfig;

namespace Duncan.AI.Droid
{
    [Activity(
                Label = "",
                Icon = "@drawable/ic_app_title_logo_only",
                ScreenOrientation = ScreenOrientation.Portrait,
               Theme = "@android:style/Theme.Holo.Light",
               WindowSoftInputMode = SoftInput.StateHidden
               )]
    public class SyncConfigActivity : Activity
    {
        private readonly LoginManager _loginImpl;

        private PropertiesDAO _propertiesDAO;


        //private static Button _issuanceInfoBackButton;

        private TextView _unitSerialNumberText;
        private TextView _deviceIdText;

        
        private TextView _clientNameText;
        private TextView _serverNameText;

        private TextView _lastResultText;

        public SyncConfigActivity()
        {
            _loginImpl = new LoginManager();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Set the content view for the layout
            SetContentView(Resource.Layout.SyncConfig);

            LoadAppProperties();



            //Get the Back button, and define a click handler
            //_issuanceInfoBackButton = FindViewById<Button>(Resource.Id.btnDone);
            //_issuanceInfoBackButton.Click += refIssuanceInfoBackHandleClick;



            _clientNameText = FindViewById<TextView>(Resource.Id.clientName);
            _clientNameText.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);
            _clientNameText.Text = _propertiesDAO.name;
            
            _clientNameText.Enabled = false;  // TODO - pwd protect


            _serverNameText = FindViewById<TextView>(Resource.Id.serverName);
            _serverNameText.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);
            _serverNameText.Text = _propertiesDAO.url;
            _serverNameText.Enabled = false; // TODO pwd protext


            _unitSerialNumberText = FindViewById<TextView>(Resource.Id.unitSerialNumber);
            _unitSerialNumberText.Enabled = false;
            _unitSerialNumberText.SetBackgroundResource(Resource.Drawable.EditTextDisabled);
            _unitSerialNumberText.Text = Helper.GetDeviceUniqueSerialNumber();

            _deviceIdText = FindViewById<TextView>(Resource.Id.deviceId);
            _deviceIdText.Enabled = false;
            _deviceIdText.SetBackgroundResource(Resource.Drawable.EditTextDisabled);
            _deviceIdText.Text = Constants.deviceModelBuildInfoFromOS;


            _lastResultText = FindViewById<TextView>(Resource.Id.syncResult);
            _lastResultText.Enabled = false;
            _lastResultText.SetBackgroundResource(Resource.Drawable.EditTextDisabled);
            _lastResultText.Text = DroidContext.SyncLastResultText;




            ActionBar actionBar = this.ActionBar;
            if (actionBar != null)
            {
                actionBar.SetDisplayHomeAsUpEnabled(true);
                actionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_black_24dp);

                actionBar.Title = "Synchronization Config";
            }


            //load the data on the screen

            //if hte xml hasnt been loaded yet, we cant do this
            if (DroidContext.XmlHasBeenLoaded && DroidContext.XmlCfg != null)
                LoadData();
            else
                AddView("No Data - Perform Sync Process.");
        }


        //Handle Issuance Info back button 
        private void refIssuanceInfoBackHandleClick(object sender, EventArgs e)
        {
            //StartActivity(typeof(LoginActivity));

            // just call Finish() ??
            SaveAppProperties();
            Finish();

        }

        //Load the data on the interface
        private void LoadData()
        {
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
            //use those nodes to create TextView objects to add to the layout
            //adn get the record count from the appropriate tables
            bool validData = false;
            foreach (IssStruct issStruct in structs)
            {
                switch (issStruct.Type.ToLower())
                {
                    case "tcitestruct":
                    case "tnotestruct":
                    case "tactivitylogstruct":
                    case "tvoidstruct":
                    case "treissuestruct":
                        string viewText = issStruct.Name + "       ";
                        var count = dataAccess.GetRecordCountByWsStatus(issStruct.Name, Constants.WS_STATUS_SUCCESS);
                        if (count > -1)
                        {
                            validData = true;
                            viewText = viewText + count.ToString();
                            AddView(viewText);
                        }
                        break;
                }
            }

            if (!validData)
            {
                AddView("No Data - Perform Sync Process.");
            }
        }

        private LinearLayout _layout { get; set; }
        protected LinearLayout IssuanceLayout
        {
            get { return _layout ?? (_layout = (LinearLayout) FindViewById(Resource.Id.iiDetailsLinearLayout)); }
        }

        private void AddView(string viewText)
        {
            return;



            //Get the LinearLayout object to add the TextView objects to
            var tView = new TextView(this);
            tView.SetText(viewText, TextView.BufferType.Normal);
            tView.SetBackgroundColor(Android.Graphics.Color.White);
            tView.SetPadding(10, 5, 10, 5);
            tView.SetTextColor(Android.Graphics.Color.Black);
            var parms = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            parms.SetMargins(40, 0, 0, 0);
            IssuanceLayout.AddView(tView, parms);
        }

        public   void LoadAppProperties()
        {
            _propertiesDAO = _loginImpl.RetrieveAppProperties();


            // await! control returns to the caller and the task continues to run on another thread
            //if (propertiesDAO != null)
            //{
            //    var stringArray = Resources.GetStringArray(Resource.Array.ClientsList);
            //    _cityName = stringArray[propertiesDAO.clientId];
            //}
        }

        public void SaveAppProperties()
        {
            _propertiesDAO.name = _clientNameText.Text;
            _propertiesDAO.url = _serverNameText.Text;


            long response = _loginImpl.SaveAppProperties(_propertiesDAO);

            //// await! control returns to the caller and the task continues to run on another thread
            //if (response)
            //{
            //    Finish();
            //    DroidContext.ApplicationContext = this.ApplicationContext;
            //    StartActivity(typeof(LoginActivity));
            //}
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Handle action buttons
            switch (item.ItemId)
            {

                case Android.Resource.Id.Home:
                    {
                        SaveAppProperties();
                        Finish();
                        return true;
                    }

                default:
                    {
                        break;
                    }
            }

            return base.OnOptionsItemSelected(item);
        }



    }
}