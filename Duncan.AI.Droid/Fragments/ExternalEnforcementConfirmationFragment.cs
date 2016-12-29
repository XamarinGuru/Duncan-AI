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
    public class ExternalEnforcementConfirmationFragment : DialogFragment
    {
        public string gCallingFragmentTagName = string.Empty;


        private ExternalEnforcementConfirmationServiceInterface myExternalEnfCheckWebService = null;
        private ExternalEnfCheckAlertDialogTimer fExternalEnforcementCheckTimer = null;

       // public GPSLocationUpdate gLocUpdate = null;

        private View fExternalEnfCheckFrag = null;
        private AlertDialog fAlertDialog;
        AlertDialog.Builder fBuilder;

        private Address fAddress = null;
        private string fFullAddress = string.Empty;

        private TextView fTitleText = null;
        private TextView fExternalEnfCheckStatusText = null;
        private Button fBtnConfirm = null;


        private bool _ExternalEnfCheckResult = false;
        private string _ExternalEnforcementKey = null;


        private string _PaymentQueryPendingStr = "Space Expiration Query Pending...";
        //private string _


        public void SetStatusText(string iStatusText)
        {
            if (fExternalEnfCheckStatusText != null)
            {
                fExternalEnfCheckStatusText.Text = iStatusText;
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {

            fBuilder = new AlertDialog.Builder(Activity);
            fExternalEnfCheckFrag = Activity.LayoutInflater.Inflate(Resource.Layout.ExternalEnforcementConfirmationCheck, null);
            fBuilder.SetView(fExternalEnfCheckFrag);

            //gotta call back the frag that called you!
            if (string.IsNullOrEmpty(gCallingFragmentTagName) == true)
            {
                gCallingFragmentTagName = "PARKING";
            }

            // initialize the view typeface etc
            fTitleText = fExternalEnfCheckFrag.FindViewById<TextView>(Resource.Id.enfCheckDialogTitle);
            if (fTitleText != null)
            {
                fTitleText.Gravity = (GravityFlags.CenterVertical | GravityFlags.CenterHorizontal);
                
                // initialize our typeface 
                Helper.SetTypefaceForTextView(fTitleText, FontManager.cnLPRDialogHeaderTextTypeface, FontManager.cnLPRDialogHeaderTextTypefaceSizeSp);
                fTitleText.Text = "Confirming Payment Status...";
            }


            fExternalEnfCheckStatusText = fExternalEnfCheckFrag.FindViewById<TextView>(Resource.Id.enfCheckStatusText);
            if (fExternalEnfCheckStatusText != null)
            {
                fExternalEnfCheckStatusText.Visibility = ViewStates.Visible;
                fExternalEnfCheckStatusText.Gravity = (GravityFlags.CenterVertical | GravityFlags.CenterHorizontal);

                // initialize our typeface 
                //Helper.SetTypefaceForTextView(fExternalEnfCheckStatusText, FontManager.cnCardViewDialogHeaderTextTypeface, FontManager.cnLPRDialogHeaderTextTypefaceSizeSp);
                Helper.SetTypefaceForTextView(fExternalEnfCheckStatusText, FontManager.cnCardViewHeaderTextMediumTypeface, FontManager.cnCardViewHeaderTextMediumTypefaceSizeSp);
            }

            fBuilder.SetPositiveButton("CONFIRM", delegate
            {
                try
                {

#if _anpr_

                if (myANPRWebService != null)
                {
                    if (myANPRWebService.ResultsAreAvailable() == true)
                    {

                        string iPlateResult = myANPRWebService.GetTopPlateResult();
                        double iTopPlateConfidence = myANPRWebService.GetTopPlateConfidence();

                        Fragment fm = FragmentManager.FindFragmentByTag(gCallingFragmentTagName);
                        if (fm != null)
                        {
                            Intent resultIntent = new Intent();
                            resultIntent.PutExtra(AutoISSUE.DBConstants.sqlVehLicNoStr, iPlateResult);
                            fm.OnActivityResult(Constants.ACTIVITY_REQUEST_CODE_LPR_PROCESSING, Result.Ok, resultIntent);
                        }

                        //This photo is used for LPR, we need to attach it to the ticket as well
                        // Add it to list of photos also add watermark string
                        DroidContext.AddPhotoToPendingAttachmentList(gFilenameForLPRProcessing, 0);                        
                    }
                    else
                    {
                        // treat it like a cancellation - send back an empty plate
                        if (myANPRWebService != null)
                        {
                            // send back empty plate 
                            Fragment fm = FragmentManager.FindFragmentByTag(gCallingFragmentTagName);
                            if (fm != null)
                            {
                                Intent resultIntent = new Intent();
                                resultIntent.PutExtra(AutoISSUE.DBConstants.sqlVehLicNoStr, "");
                                fm.OnActivityResult(Constants.ACTIVITY_REQUEST_CODE_LPR_PROCESSING, Result.Ok, resultIntent);
                            }

                            myANPRWebService.CancelLPRProcessingRequest();
                        }

                    }


#endif
                    Fragment fm = FragmentManager.FindFragmentByTag(gCallingFragmentTagName);
                    if (fm != null)
                    {
                        Intent resultIntent = new Intent();
                        //resultIntent.PutExtra(AutoISSUE.DBConstants.sqlLocStreetStr, fFullAddress);
                        fm.OnActivityResult(Constants.ACTIVITY_REQUEST_CODE_EXTERNAL_ENFORCMENT_CONFIRMATION, Result.Ok, resultIntent);
                    }
                }
                catch (System.Exception exp)
                {
                    System.Console.WriteLine("Failed to confirm ExternalEnforcementConfirmationFragment payment:" + exp.Message);
                }

                this.Dismiss();
            });
            
            fBuilder.SetNegativeButton("CANCEL", delegate
            {

                try
                {
                    // just close and quit
                    if (myExternalEnfCheckWebService != null)
                    {
                        myExternalEnfCheckWebService.CancelLPRProcessingRequest();
                    }


                    Fragment fm = FragmentManager.FindFragmentByTag(gCallingFragmentTagName);
                    if (fm != null)
                    {
                        Intent resultIntent = new Intent();
                        //resultIntent.PutExtra(AutoISSUE.DBConstants.sqlLocStreetStr, fFullAddress);
                        fm.OnActivityResult(Constants.ACTIVITY_REQUEST_CODE_EXTERNAL_ENFORCMENT_CONFIRMATION, Result.Canceled, resultIntent);
                    }
                }
                catch (System.Exception exp)
                {
                    System.Console.WriteLine("Failed to confirm ExternalEnforcementConfirmationFragment payment:" + exp.Message);
                }


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
                fExternalEnforcementCheckTimer.UnregisterFromRuntime();
                fExternalEnforcementCheckTimer.Dispose();

                // put the address data into a single line so we can parse it
                System.Text.StringBuilder loFullAddressBuilder = new System.Text.StringBuilder();
                for (int loAddressLineIdx = 0; loAddressLineIdx < iAddress.MaxAddressLineIndex; loAddressLineIdx++)
                {
                    string loAddressLine = iAddress.GetAddressLine(loAddressLineIdx);
                    loFullAddressBuilder.Append(loAddressLine + " ");
                }

                // try to break into meaning
                fFullAddress = loFullAddressBuilder.ToString().Trim();
               
                if (fExternalEnfCheckStatusText != null)
                {
                    fExternalEnfCheckStatusText.Text = iAddress.GetAddressLine(0);                    
                    fExternalEnfCheckStatusText.Visibility = ViewStates.Visible;
                }

                fTitleText.Text = "Confirming Payment Status..."; 

                // have a location to confirm, enable the confirm
                if (fBtnConfirm != null)
                {
                    fBtnConfirm.Clickable = true;
                    fBtnConfirm.SetTextColor(Android.Graphics.Color.Black);
                }                               
            }
            else
            {
                if (fExternalEnfCheckStatusText != null)
                {
                    fExternalEnfCheckStatusText.Text = "Payment Status Update Not Currently Available.";
                    fExternalEnfCheckStatusText.Visibility = ViewStates.Visible;
                }                   
            }
            fExternalEnfCheckFrag.RequestLayout();
            fExternalEnfCheckFrag.Invalidate();

        }

        public ExternalEnforcementConfirmationFragment( string iExternalEnforcementKey )
        {
            _ExternalEnforcementKey = iExternalEnforcementKey;
        }

        public ExternalEnforcementConfirmationFragment(Context context)
        {
            
        }



        public override void OnStart()
        {
            base.OnStart();



            myExternalEnfCheckWebService = new ExternalEnforcementConfirmationServiceInterface();

            fExternalEnforcementCheckTimer = new ExternalEnfCheckAlertDialogTimer(this);
            fExternalEnforcementCheckTimer.Run();

            // go web go
            myExternalEnfCheckWebService.CallExternalEnforcementService(_ExternalEnforcementKey);
                       
        }


        public void UpdateInfoFromWebService()
        {
            // check the async result
            if (myExternalEnfCheckWebService != null)
            {
                string loText;

                if (myExternalEnfCheckWebService.ResultsAreAvailable() == true)
                {
                    fBtnConfirm.Clickable = true;
                    fBtnConfirm.SetTextColor(Android.Graphics.Color.Black);
                    fBtnConfirm.Invalidate();

                    fTitleText.Text = "Status Check Complete";
                }


                // always keep updating the status as we go
                loText = myExternalEnfCheckWebService.GetLastStatusTextMessage();
                SetStatusText(loText);
            }
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

    class ExternalEnfCheckAlertDialogTimer : Java.Lang.Object, IRunnable
    {

        private readonly  ExternalEnforcementConfirmationFragment _Owner;
        private readonly Handler _Handler = new Handler();

        public ExternalEnfCheckAlertDialogTimer(ExternalEnforcementConfirmationFragment iOwnerDialog)
        {
            _Owner = iOwnerDialog;                       
        }
        public void Run()
        {
            if (_Owner != null)
            {
                _Owner.UpdateInfoFromWebService();
            }

            _Handler.PostDelayed(UpdateTimer, 750);
        }

        private async void UpdateTimer()
        {
            if (_Owner != null)
            {
                _Owner.UpdateInfoFromWebService();

                _Handler.PostDelayed(UpdateTimer, 750);
            }
        }
    }

}