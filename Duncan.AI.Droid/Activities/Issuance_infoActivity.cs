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
                Icon = "@drawable/ic_app_title",
                ScreenOrientation = ScreenOrientation.Portrait, 
                Theme = "@android:style/Theme.NoTitleBar")]
    public class Issuance_infoActivity : Activity
    {
        //create some variables to hold references to controls,
        //and to hold the city name
        private readonly LoginManager _loginImpl;
        private static Button _issuanceInfoBackButton;
        private TextView _cityNameText;
        private string _cityName;


        public Issuance_infoActivity()
        {
            _loginImpl = new LoginManager();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Set the content view for the layout
            SetContentView(Resource.Layout.Issuance_info);

            //Get the Back button, and define a click handler
            _issuanceInfoBackButton = FindViewById<Button>(Resource.Id.refIssuanceInfoBackButton);
            _issuanceInfoBackButton.Click += refIssuanceInfoBackHandleClick;

            //Get the city name TextView object, and get the selected city
            //and set the text
            _cityNameText = FindViewById<TextView>(Resource.Id.textViewCity);
            GetAppCityName();
            _cityNameText.Text = _cityName;

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
            StartActivity(typeof(LoginActivity));
            //SetContentView(Resource.Layout.Login);
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

        public   void GetAppCityName()
        {
            PropertiesDAO propertiesDAO = _loginImpl.RetrieveAppProperties();
            // await! control returns to the caller and the task continues to run on another thread
            if (propertiesDAO != null)
            {
                var stringArray = Resources.GetStringArray(Resource.Array.ClientsList);
                _cityName = stringArray[propertiesDAO.clientId];
            }
        }


    }
}