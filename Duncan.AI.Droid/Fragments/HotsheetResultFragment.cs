using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Provider;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Object = Java.Lang.Object;

using Duncan.AI.Droid.Common;

namespace Duncan.AI.Droid
{
    public class HotsheetResultFragment : DialogFragment
    {

        public string gCallingFragmentTagName = string.Empty;
        public string gFilenameForLPRProcessing = string.Empty;


        public interface HotsheetSearchConfirmResultDialogListener
        {
            void OnFinishANPRDialog(string myResult);
        }

        private SearchStructServerInterface mySearchStructWebService = null;

        private AlertDialog _thisDialog = null;  // there must be a way to reference this

        private CustomAlertDialogTimer_HotsheetResultFragment myTimer = null;

        private View ANPRDialogView;
        private View StatusText;


        File _photoFile;
        //File _photoDir;


        ImageView _imageView;
        
        public TextView LPRResultsTextView;
        public TextView LPRConfidenceTextView;

        

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        //public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        //{
        //    // Use this to return your custom view for this Fragment
        //    // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

        //    //return base.OnCreateView(inflater, container, savedInstanceState);

        //    // Inflate the custom dialog view
        //    var view = inflater.Inflate(Resource.Layout.ANPRConfirmSelection, container, false);

        //    // Remove the title bar from the dialog
        //   //this.Dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);

        //    // remove the fading background
        //   //this.Dialog.Window.ClearFlags(WindowManagerFlags.DimBehind);
        //   this.Dialog.Window.SetFlags(WindowManagerFlags.BlurBehind, WindowManagerFlags.BlurBehind);


        //    // give the dialog a custom in/out transition animation
        //    //this.Dialog.Window.SetWindowAnimations(Resource.Style.ForgotPasswordDialogAnimation);


        //    return view;

        //}

        //public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        //{
        //    Dialog dialog = base.OnCreateDialog(savedInstanceState);

        //    dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);

        //    //dialog.Window.ClearFlags(WindowManagerFlags.DimBehind);

        //    dialog.Window.SetFlags(WindowManagerFlags.BlurBehind);

        //    return dialog;
        //}

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);


            // Inflate and set the layout for the dialog -  Pass null as the parent view because its going in the dialog layout
            ANPRDialogView = Activity.LayoutInflater.Inflate(Resource.Layout.ANPRConfirmSelection, null);

            // Pass null as the parent view because its going in the dialog layout
            builder.SetView( ANPRDialogView );

            builder.SetTitle("LPR Processing");
            builder.SetMessage("");


            _imageView =  ANPRDialogView.FindViewById<ImageView>(Resource.Id.platephoto);

            LPRResultsTextView = ANPRDialogView.FindViewById<TextView>(Resource.Id.lblLPRResults);
            LPRConfidenceTextView = ANPRDialogView.FindViewById<TextView>(Resource.Id.lblLPRConfidence);



            //gotta call back the frag that called you!
            if (string.IsNullOrEmpty(gCallingFragmentTagName) == true)
            {
                gCallingFragmentTagName = "PARKING";
            }
           



            builder.SetPositiveButton("CONFIRM", delegate
            {
                // save the user choice
                //ANPRConfirmResultDialogListener activity = (ANPRConfirmResultDialogListener)GetActivity();
                //ANPRConfirmResultDialogListener activity = (ANPRConfirmResultDialogListener)Activity;
                //activity.OnFinishANPRDialog("ABC123");

                if (mySearchStructWebService != null)
                {
                    if (mySearchStructWebService.ResultsAreAvailable() == true)
                    {

                        string iPlateResult = mySearchStructWebService.GetTopPlateResult();
                        double iTopPlateConfidence = mySearchStructWebService.GetTopPlateConfidence();

                        Fragment fm = FragmentManager.FindFragmentByTag(gCallingFragmentTagName);
                        if (fm != null)
                        {
                            Intent resultIntent = new Intent();
                            resultIntent.PutExtra(AutoISSUE.DBConstants.sqlVehLicNoStr, iPlateResult);
                            fm.OnActivityResult(Constants.ACTIVITY_REQUEST_CODE_LPR_PROCESSING, Result.Ok, resultIntent);
                        }
                    }
                    else
                    {
                        // treat it like a cancellation - send back an empty plate
                        if (mySearchStructWebService != null)
                        {
                            // send back empty plate 
                            Fragment fm = FragmentManager.FindFragmentByTag(gCallingFragmentTagName);
                            if (fm != null)
                            {
                                Intent resultIntent = new Intent();
                                resultIntent.PutExtra(AutoISSUE.DBConstants.sqlVehLicNoStr, "");
                                fm.OnActivityResult(Constants.ACTIVITY_REQUEST_CODE_LPR_PROCESSING, Result.Ok, resultIntent);
                            }

                            mySearchStructWebService.CancelLPRProcessingRequest();
                        }

                    }
                }

                this.Dismiss();
            });

            builder.SetNegativeButton("CANCEL", delegate
            {
                // just close and quit
                if (mySearchStructWebService != null)
                {
                    // send back empty plate 
                    Fragment fm = FragmentManager.FindFragmentByTag(gCallingFragmentTagName);
                    if (fm != null)
                    {
                        Intent resultIntent = new Intent();
                        resultIntent.PutExtra(AutoISSUE.DBConstants.sqlVehLicNoStr, "" );
                        fm.OnActivityResult(Constants.ACTIVITY_REQUEST_CODE_LPR_PROCESSING, Result.Ok, resultIntent);
                    }

                    mySearchStructWebService.CancelLPRProcessingRequest();
                }
            });

            //return builder.Create();

            _thisDialog = builder.Create();

            return _thisDialog;
        }



        private void ActivateCamera(System.String action, System.String fileSuffix)
        {

            try
            {

                string loFilename = Helper.GetLPRImageFilenameFixed(fileSuffix);


                Intent intent = new Intent(action);

                _photoFile = new Java.IO.File(loFilename);  // the path is already included
                intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_photoFile));

                StartActivityForResult(intent, Constants.ACTIVITY_REQUEST_CODE_CAPTURE_PHOTO_FOR_LPR);

            }
            catch (System.Exception exp)
            {
                System.Console.WriteLine("Failed to launch Camera for ANPR:" + exp.Message);
            }


        }



        public override void OnStart()
        {
            base.OnStart();

            //ActivateCamera(MediaStore.ActionImageCapture, Constants.PHOTO_FILE_SUFFIX);


            string loFilenameForLPRProcessing = gFilenameForLPRProcessing;

            // override for test
            //loFilenameForLPRProcessing = @"/storage/emulated/0/Pictures/civicsmart/20160202_163307.jpg"; // bad image test
            //loFilenameForLPRProcessing = @"/storage/emulated/0/Pictures/civicsmart/20160202_163313_top_left_good.jpg"; // good image test 

            if (gFilenameForLPRProcessing.Length > 0)
            {
                _photoFile = new File(gFilenameForLPRProcessing);
                ShowImage();
            }

           

            mySearchStructWebService = new SearchStructServerInterface();


            myTimer = new CustomAlertDialogTimer_HotsheetResultFragment(this);
            myTimer.Run();

            // go web go
            //mySearchStructWebService.CallHotSheetSearchService(loFilenameForLPRProcessing);


            //myTimer = new CountDownTimer(

        }


        public void UpdateInfoFromWebService()
        {
            // check the async result
            if (mySearchStructWebService != null)
            {
                string loText;
                string loConfidenceText;

                if (mySearchStructWebService.ResultsAreAvailable() == true)
                {
                    string iPlateResult = mySearchStructWebService.GetTopPlateResult();
                    double iTopPlateConfidence = mySearchStructWebService.GetTopPlateConfidence();

                    if (iPlateResult.Length > 0)
                    {
                        loText = "Plate: " + iPlateResult;
                        loConfidenceText = "Confidence: " + string.Format("{0:0.00}", iTopPlateConfidence) + "%";
                    }
                    else
                    {
                        loText = "No Plate Found";
                        loConfidenceText = "";
                    }

                    //_thisDialog.SetMessage(loText);

                    LPRResultsTextView.SetText(loText, TextView.BufferType.Normal);
                    LPRConfidenceTextView.SetText(loConfidenceText, TextView.BufferType.Normal);

                }

                loText = mySearchStructWebService.GetLastStatusTextMessage();
                _thisDialog.SetMessage(loText);

            }
        }


        private void ShowImage()
        {
  //          try
            {
                Bitmap bitmap = null;
                if (_photoFile != null)
                {
                    if (_photoFile.AbsolutePath.Contains(Constants.PHOTO_FILE_SUFFIX))
                    {
                        // resize the bitmap to fit the display, Loading the full sized image will consume too much memory 

                        //int height = _imageView.Height;
                        //int width = Resources.DisplayMetrics.WidthPixels;

                        int height = _imageView.Height;
                        int width = _imageView.Width;


                        if (( height == 0 ) || ( width == 0 ))
                        {
                            height = 320;
                            width = 240;
                        }


                        bitmap = _photoFile.Path.LoadAndResizeBitmap(width, height);
                    }

                    _imageView.SetImageBitmap(bitmap);
                }
            }
            //catch (System.IO.Exception exp)
            //{
            //    //
            //}
        }

    }


    public class CustomAlertDialogTimer_HotsheetResultFragment : Object, IRunnable
    {

        private readonly HotsheetResultFragment _Owner;
        private readonly Handler mHandler = new Handler();

        public CustomAlertDialogTimer_HotsheetResultFragment(HotsheetResultFragment iOwnerDialog)
        {
            _Owner = iOwnerDialog;
        }
        public void Run()
        {
            mHandler.PostDelayed(UpdateTimer, 1000);
        }
        private void UpdateTimer()
        {
            if (_Owner != null)
            {
                _Owner.UpdateInfoFromWebService();

                 mHandler.PostDelayed(UpdateTimer, 1000);
            }
        }
    }

}