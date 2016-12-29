using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;
using Java.Lang;
using Duncan.AI.Droid.Utils.HelperManagers;

namespace Duncan.AI.Droid.Common
{
    public class GeoCodeLocationAddressFragment : DialogFragment
    {
        public string gCallingFragmentTagName = string.Empty;
        public GPSLocationUpdate gLocUpdate = null;

        private View fGeoCodeFrag = null;
        private AlertDialog fAlertDialog;
        AlertDialog.Builder fBuilder;
        private Address fAddress = null;
        private string fFullAddress = string.Empty;
        private CustomAlertDialogTimer fGeoCodeTimer = null;
        private TextView fTitleText = null;
        private TextView fAddressText = null;
        private Button fBtnConfirm = null;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {

            fBuilder = new AlertDialog.Builder(Activity);
            fGeoCodeFrag = Activity.LayoutInflater.Inflate(Resource.Layout.GeoCodeAddressConfirm, null);
            fBuilder.SetView(fGeoCodeFrag);

            //gotta call back the frag that called you!
            if (string.IsNullOrEmpty(gCallingFragmentTagName) == true)
            {
                gCallingFragmentTagName = "PARKING";
            }

            // initialize the view typeface etc
            fTitleText = fGeoCodeFrag.FindViewById<TextView>(Resource.Id.geocodeDlgTitle);
            if (fTitleText != null)
            {
                fTitleText.Gravity = (GravityFlags.CenterVertical | GravityFlags.CenterHorizontal);
                
                // initialize our typeface 
                Helper.SetTypefaceForTextView(fTitleText, FontManager.cnLPRDialogHeaderTextTypeface, FontManager.cnLPRDialogHeaderTextTypefaceSizeSp);
                fTitleText.Text = "Reverse GeoCoding...";
            }

            fAddressText = fGeoCodeFrag.FindViewById<TextView>(Resource.Id.geocodeAddress);
            if (fAddressText != null)
            {
                fAddressText.Visibility = ViewStates.Invisible;
                fAddressText.Gravity = (GravityFlags.CenterVertical | GravityFlags.CenterHorizontal);

                // initialize our typeface 
                Helper.SetTypefaceForTextView(fAddressText, FontManager.cnCardViewDialogHeaderTextTypeface, FontManager.cnLPRDialogHeaderTextTypefaceSizeSp);
            }

            fBuilder.SetPositiveButton("CONFIRM", delegate
            {
                try
                {
                    if (!string.IsNullOrEmpty(fFullAddress))
                    {
                        Fragment fm = FragmentManager.FindFragmentByTag(gCallingFragmentTagName);
                        if (fm != null)
                        {
                            Intent resultIntent = new Intent();
                            resultIntent.PutExtra(AutoISSUE.DBConstants.sqlLocStreetStr, fFullAddress);
                            fm.OnActivityResult(Constants.ACTIVITY_REQUEST_CODE_GEOCODE_ADDRESS, Result.Ok, resultIntent);
                        }
                    }
                }
                catch (System.Exception exp)
                {
                    System.Console.WriteLine("Failed to confirm Geocode address:" + exp.Message);
                }

                this.Dismiss();
            });
            
            fBuilder.SetNegativeButton("CANCEL", delegate
            {
                this.Dismiss();
            });
                                                
            fAlertDialog = fBuilder.Create();
            fAlertDialog.Show();

            //Customize the dlg to meet our UX spec
            fBtnConfirm = fAlertDialog.GetButton((int)Android.Content.DialogButtonType.Positive);
            if (fBtnConfirm != null)
            {
                //Disable it until we get the result
                fBtnConfirm.Clickable = false;
                fBtnConfirm.SetTextColor(Android.Graphics.Color.Gray);

                Helper.SetTypefaceForButton(fBtnConfirm, FontManager.cnLPRDialogButtonTypeface, FontManager.cnMLPRDialogButtonTypefaceSizeSp);
            }

            Button loBtnExit = fAlertDialog.GetButton((int)Android.Content.DialogButtonType.Negative);
            if (loBtnExit != null)
            {
                // initialize our typeface 
                Helper.SetTypefaceForButton(loBtnExit, FontManager.cnLPRDialogButtonTypeface, FontManager.cnMLPRDialogButtonTypefaceSizeSp);
            }
            
            return fAlertDialog;
        }

        public void UpdateGeoCodeAddress(Address iAddress)
        {            
            if (iAddress != null)
            {
                //We got the result, stop the timer
                fGeoCodeTimer.UnregisterFromRuntime();
                fGeoCodeTimer.Dispose();

                // put the address data into a single line so we can parse it
                System.Text.StringBuilder loFullAddressBuilder = new System.Text.StringBuilder();
                for (int loAddressLineIdx = 0; loAddressLineIdx < iAddress.MaxAddressLineIndex; loAddressLineIdx++)
                {
                    string loAddressLine = iAddress.GetAddressLine(loAddressLineIdx);
                    loFullAddressBuilder.Append(loAddressLine + " ");
                }

                // try to break into meaning
                fFullAddress = loFullAddressBuilder.ToString().Trim();
               
                if (fAddressText != null)
                {
                    fAddressText.Text = iAddress.GetAddressLine(0);                    
                    fAddressText.Visibility = ViewStates.Visible;
                }

                fTitleText.Text = "Reverse Geocode Address"; //"Reverse Geocode Location to Address";

                // have a location to confirm, enable the confirm
                if (fBtnConfirm != null)
                {
                    fBtnConfirm.Clickable = true;
                    fBtnConfirm.SetTextColor(Android.Graphics.Color.Black);
                }                               
            }
            else
            {
                if (fAddressText != null)
                {
                    fAddressText.Text = "GPS Location Not Currently Available.";
                    fAddressText.Visibility = ViewStates.Visible;
                }                   
            }
            fGeoCodeFrag.RequestLayout();
            fGeoCodeFrag.Invalidate();

        }

        public GeoCodeLocationAddressFragment()
        {
        }

        public GeoCodeLocationAddressFragment(Context context)
        {
            
        }

        public void InitGeoCodeLocationAddressFragment( Address iAddress )
        {
            fAddress = iAddress;
        }


        public override void OnStart()
        {
            base.OnStart();

            fAddress = null;

            gLocUpdate = new GPSLocationUpdate(Activity);
                       
            fGeoCodeTimer = new CustomAlertDialogTimer(this);
            fGeoCodeTimer.Run();
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnStop()
        {
            base.OnStop();
        }

        public override void OnDestroy()
        {
            fBuilder.Dispose();
            base.OnDestroy();
        }

    }

    public class CustomAlertDialogTimer : Java.Lang.Object, IRunnable
    {

        private readonly  GeoCodeLocationAddressFragment _Owner;
        private readonly Handler _Handler = new Handler();

        public CustomAlertDialogTimer(GeoCodeLocationAddressFragment iOwnerDialog)
        {
            _Owner = iOwnerDialog;                       
        }
        public void Run()
        {
            _Handler.PostDelayed(UpdateTimer, 750);
        }

        private async void UpdateTimer()
        {
            if (_Owner != null)
            {
                bool loRes = await _Owner.gLocUpdate.GetCurrentLocationAddressAsync();  
                if(loRes == true)
                {
                    Address loAddress = GPSLocationUpdate.GetLastUpdatedAddress();
                    if (loAddress != null)
                    {
                        _Owner.UpdateGeoCodeAddress(loAddress);
                    }
                }
                else
                {
                    _Handler.PostDelayed(UpdateTimer, 750);
                }
                
            }
        }
    }

}