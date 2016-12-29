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
using Duncan.AI.Droid.Common;

using Duncan.AI.Droid.Utils.HelperManagers;
using Java.IO;
using Java.Lang;
using Object = Java.Lang.Object;

using Duncan.AI.Droid.Common;

namespace Duncan.AI.Droid
{
    public class ANPRConfirmResultFragment : DialogFragment
    {

        public string gCallingFragmentTagName = string.Empty;
        public string gFilenameForLPRProcessing = string.Empty;


        public interface ANPRConfirmResultDialogListener
        {
            void OnFinishANPRDialog(string myResult);
        }

        private AutomaticNumberPlateRecognitionServerInterface myANPRWebService = null;

        private AlertDialog _thisDialog = null;  

        private CustomANPRAlertDialogTimer myTimer = null;

        private View ANPRDialogView;
        private TextView _titleText;
        private TextView _statusText;
        private ListView _listView;
        private Button _btnConfirm;


        File _photoFile;
        //File _photoDir;


        ImageView _imageView;


        List<ANPRMatchDTO> _ANPRResultsList = null;

        //public TextView LPRResultsTextView;
        //public TextView LPRConfidenceTextView;


        public void SetStatusText(string iStatusText)
        {
            if (_statusText != null)
            {
                _statusText.Text = iStatusText;
            }
        }


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }
        public void SetUserSelectionAndReturnToCallingFragnment( string iSelectedPlate )
        {
            try
            {
                //This photo is used for LPR, we need to attach it to the ticket as well
                // Add it to list of photos also add watermark string
                DroidContext.AddPhotoToPendingAttachmentList(gFilenameForLPRProcessing, 0);


                Fragment fm = FragmentManager.FindFragmentByTag(gCallingFragmentTagName);
                if (fm != null)
                {
                    Intent resultIntent = new Intent();
                    resultIntent.PutExtra(AutoISSUE.DBConstants.sqlVehLicNoStr, iSelectedPlate);
                    fm.OnActivityResult(Constants.ACTIVITY_REQUEST_CODE_LPR_PROCESSING, Result.Ok, resultIntent);
                }

                this.Dismiss();
            }
            catch (System.Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "ANPRConfirmResultFragment.SetUserSelectionAndReturnToCallingFragnment", ex.TargetSite.Name);
                System.Console.WriteLine("ANPRConfirmResultFragment::SetUserSelectionAndReturnToCallingFragnment Exception source {0}: {1}", ex.Source, ex.ToString());
            }


        }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);


            // Inflate and set the layout for the dialog -  Pass null as the parent view because its going in the dialog layout
            ANPRDialogView = Activity.LayoutInflater.Inflate(Resource.Layout.ANPRConfirmSelectionListView, null);


            // Pass null as the parent view because its going in the dialog layout
            builder.SetView(ANPRDialogView);


            _titleText = ANPRDialogView.FindViewById<TextView>(Resource.Id.lprTitle);
            if (_titleText != null)
            {
                _titleText.Gravity = (GravityFlags.CenterVertical | GravityFlags.CenterHorizontal);

                // initialize our typeface 
                Helper.SetTypefaceForTextView(_titleText, FontManager.cnLPRDialogHeaderTextTypeface, FontManager.cnLPRDialogHeaderTextTypefaceSizeSp);

                _titleText.Text = "LPR Image Processing";
            }

            _statusText = ANPRDialogView.FindViewById<TextView>(Resource.Id.lprStatus);
            if (_statusText != null)
            {
                _statusText.Gravity = (GravityFlags.CenterVertical | GravityFlags.CenterHorizontal);

                // initialize our typeface 
                Helper.SetTypefaceForTextView(_statusText, FontManager.cnCardViewDialogHeaderTextTypeface, FontManager.cnCardViewDialogHeaderTextTypefaceSizeSp);

                _statusText.Text = "";
            }

            _imageView = ANPRDialogView.FindViewById<ImageView>(Resource.Id.platephoto);
            _listView = ANPRDialogView.FindViewById<ListView>(Resource.Id.candidateslist);

            //gotta call back the frag that called you!
            if (string.IsNullOrEmpty(gCallingFragmentTagName) == true)
            {
                gCallingFragmentTagName = "PARKING";
            }




            builder.SetPositiveButton("CONFIRM", delegate
            {
                // TODO - 

                // EXIT just closes the dialog
                // If they want to use a result, they use the button inside the list view



                if (myANPRWebService != null)
                {
                    if (myANPRWebService.ResultsAreAvailable() == true)
                    {

                        string iPlateResult = myANPRWebService.GetTopPlateResult();
                        double iTopPlateConfidence = myANPRWebService.GetTopPlateConfidence();

                        // set it and git
                        SetUserSelectionAndReturnToCallingFragnment(iPlateResult);
                        return;

                        //Fragment fm = FragmentManager.FindFragmentByTag(gCallingFragmentTagName);
                        //if (fm != null)
                        //{
                        //    Intent resultIntent = new Intent();
                        //    resultIntent.PutExtra(AutoISSUE.DBConstants.sqlVehLicNoStr, iPlateResult);
                        //    fm.OnActivityResult(Constants.ACTIVITY_REQUEST_CODE_LPR_PROCESSING, Result.Ok, resultIntent);
                        //}

                        ////This photo is used for LPR, we need to attach it to the ticket as well
                        //// Add it to list of photos also add watermark string
                        //DroidContext.AddPhotoToPendingAttachmentList(gFilenameForLPRProcessing, 0);                        
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
                }

                this.Dismiss();
            });

            builder.SetNegativeButton("CANCEL", delegate
            {
                // just close and quit
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
            });

            _thisDialog = builder.Create();

            // call this now to initialize vars for customization
            _thisDialog.Show();


            // customize buttons - has to execute after .Show()
            CustomizeANPRDialog();

            
            return _thisDialog;
        }


        private int GetCalculatedDialogHeight()
        {
            var metric = Resources.DisplayMetrics;
            int loDialogHeight = (int)(metric.HeightPixels * .90);

            return loDialogHeight;
        }


        private int GetCalculatedDialogWidth()
        {
            var metric = Resources.DisplayMetrics;
            int loDialogWidth = (int)(metric.WidthPixels * .95);

            return loDialogWidth;
        }

        public void ResizeDialogForResults()
        {
            // lets set the buttons sizes dynamically by screen size
            int loDialogWidth = GetCalculatedDialogWidth();
            int loDialogHeight = GetCalculatedDialogHeight();


            Window window = _thisDialog.Window;

            window.SetLayout( loDialogWidth, loDialogHeight );
        }


        private void CustomizeANPRDialog()
        {
            int cnMarginExitLeft = 25;
            int cnMarginExitTop = 25;
            int cnMarginExitRight = 50;
            int cnMarginExitBottom = 25;

            int cnMarginConfirmLeft = 25;
            int cnMarginConfirmTop = 25;
            int cnMarginConfirmRight = 50;
            int cnMarginConfirmBottom = 25;

            _btnConfirm = _thisDialog.GetButton((int)Android.Content.DialogButtonType.Positive);
            if (_btnConfirm != null)
            {

                //Disable it until we get the result
                _btnConfirm.Clickable = false;
                _btnConfirm.SetTextColor(Android.Graphics.Color.Gray);
                
                //_btnConfirm.Text = "CONFIRM";// Constants.cnMenuPopup_ExitText;
                _btnConfirm.Text = "SELECT TOP";// Constants.cnMenuPopup_ExitText;
                Helper.SetTypefaceForButton(_btnConfirm, FontManager.cnLPRDialogButtonTypeface, FontManager.cnMLPRDialogButtonTypefaceSizeSp);
                
#if _more_

                // lets set the buttons sizes dynamically by screen size
                var metric = Resources.DisplayMetrics;
                int loButtonWidth = (int)(metric.WidthPixels * .33);

                // KLDUGE- should be done dynamically - what is the width of the AlertDialog?
                int loDialogWidth = GetCalculatedDialogWidth();


                // calculate margin to center in dialog
                //int loButtonMarginLeft = (int)((metric.WidthPixels - loButtonWidth) / 2);
                int loButtonMarginLeft = (int)((loDialogWidth - loButtonWidth) / 2);



                btnConfirm.SetBackgroundResource(Resource.Drawable.button_menu_popup_exit);
                btnConfirm.SetTextColor(Resources.GetColor(Resource.Color.civicsmart_black));


                btnConfirm.Text = "CONFIRM";// Constants.cnMenuPopup_ExitText;
                btnConfirm.Gravity = (GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);

                btnConfirm.Tag = Constants.cnMenuPopup_ExitText;

                // initialize our typeface 
                Helper.SetTypefaceForButton(btnConfirm, FontManager.cnLPRDialogButtonTypeface, FontManager.cnMLPRDialogButtonTypefaceSizeSp);


                LinearLayout.LayoutParams layoutParamsButtonConfirm = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);

                // calculated placement
                //layoutParamsButtonConfirm.SetMargins(cnMarginConfirmLeft, cnMarginConfirmTop, cnMarginConfirmRight, cnMarginConfirmBottom);
                //layoutParamsButtonConfirm.SetMargins(loButtonMarginLeft, cnMarginConfirmTop, 0, cnMarginConfirmBottom);

                layoutParamsButtonConfirm.Gravity = GravityFlags.NoGravity;


                // gravity doesnt work... because parent is not horizontal layout?
                //;ayoutParamsButtonExit.Gravity = GravityFlags.Right;
                //layoutParamsButtonExit.Gravity = GravityFlags.CenterVertical | GravityFlags.CenterHorizontal;


                btnConfirm.LayoutParameters = layoutParamsButtonConfirm;
                btnConfirm.RequestLayout();
#endif
            }


            Button btnExit = _thisDialog.GetButton((int)Android.Content.DialogButtonType.Negative);
            if (btnExit != null)
            {

                btnExit.Text = "CANCEL"; // Constants.cnMenuPopup_ExitText;
                // initialize our typeface 
                Helper.SetTypefaceForButton(btnExit, FontManager.cnLPRDialogButtonTypeface, FontManager.cnMLPRDialogButtonTypefaceSizeSp);


#if _more_

                // lets set the buttons sizes dynamically by screen size
                var metric = Resources.DisplayMetrics;
                int loButtonWidth = (int)(metric.WidthPixels * .33);

                // KLDUGE- should be done dynamically - what is the width of the AlertDialog?
                int loDialogWidth = GetCalculatedDialogWidth();

                
                // calculate margin to center in dialog
                //int loButtonMarginLeft = (int)((metric.WidthPixels - loButtonWidth) / 2);
                int loButtonMarginLeft = (int)((loDialogWidth - loButtonWidth) / 2);



                btnExit.SetBackgroundResource(Resource.Drawable.button_menu_popup_exit);
                btnExit.SetTextColor(Resources.GetColor(Resource.Color.civicsmart_black));


                btnExit.Text = "CANCEL"; // Constants.cnMenuPopup_ExitText;
                btnExit.Gravity = (GravityFlags.Left | GravityFlags.CenterVertical);

                btnExit.Tag = Constants.cnMenuPopup_ExitText;

                // initialize our typeface 
                Helper.SetTypefaceForButton(btnExit, FontManager.cnLPRDialogButtonTypeface, FontManager.cnMLPRDialogButtonTypefaceSizeSp);


                LinearLayout.LayoutParams layoutParamsButtonExit = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);

                // calculated placement
                //layoutParamsButtonExit.SetMargins(cnMarginExitLeft, cnMarginExitTop, cnMarginExitRight, cnMarginExitBottom);
                //layoutParamsButtonExit.SetMargins(loButtonMarginLeft, cnMarginExitTop, 0, cnMarginExitBottom);

                layoutParamsButtonExit.Gravity = GravityFlags.NoGravity;


                // gravity doesnt work... because parent is not horizontal layout?
                //;ayoutParamsButtonExit.Gravity = GravityFlags.Right;
                //layoutParamsButtonExit.Gravity = GravityFlags.CenterVertical | GravityFlags.CenterHorizontal;


                btnExit.LayoutParameters = layoutParamsButtonExit;
                btnExit.RequestLayout();
#endif
            }

        }




        public override void OnStart()
        {
            base.OnStart();


            string loFilenameForLPRProcessing = gFilenameForLPRProcessing;

            // override for test
            //loFilenameForLPRProcessing = @"/storage/emulated/0/Pictures/civicsmart/20160202_163307.jpg"; // bad image test
            //loFilenameForLPRProcessing = @"/storage/emulated/0/Pictures/civicsmart/20160202_163313_top_left_good.jpg"; // good image test 

            if (gFilenameForLPRProcessing.Length > 0)
            {
                _photoFile = new File(gFilenameForLPRProcessing);
                ShowImage();
            }

           

            myANPRWebService = new AutomaticNumberPlateRecognitionServerInterface();


            myTimer = new CustomANPRAlertDialogTimer(this);
            myTimer.Run();

            // go web go
            myANPRWebService.CallAutomaticNumberPlateRecognitionService(loFilenameForLPRProcessing);

        }


        public void UpdateInfoFromWebService()
        {
            try
            {
                // check the async result
                if (myANPRWebService != null)
                {
                    string loText;
                    string loConfidenceText;

                    if (myANPRWebService.ResultsAreAvailable() == true)
                    {
                        // presented them yet?
                        if (_ANPRResultsList == null)
                        {
                            _ANPRResultsList = new List<ANPRMatchDTO>();

                            bool loAtLeastOneCandidateFound = false;


                            // get  the top result as the default choice
                            string iPlateResult = myANPRWebService.GetTopPlateResult();
                            double iTopPlateConfidence = myANPRWebService.GetTopPlateConfidence();

                            if (iPlateResult.Length > 0)
                            {
                                loText = "Plate: " + iPlateResult;
                                //loConfidenceText = "Confidence: " + string.Format("{0:0.00}", iTopPlateConfidence) + "%";
                                loConfidenceText = string.Format("{0:0.00}", iTopPlateConfidence) + "%";
                                //We have good result, Enable the confirm button                            
                                _btnConfirm.Clickable = true;
                                _btnConfirm.SetTextColor(Android.Graphics.Color.Black);
                                loAtLeastOneCandidateFound = true;
                            }
                            else
                            {
                                loText = "No Plate Found";
                                loConfidenceText = "";
                            }


                            // iterate and present all of the choices
                            foreach (AutomaticNumberPlateRecognitionServerInterface.ANPRServiceResult oneResult in myANPRWebService.myLPRResults.results)
                            {
                                foreach (AutomaticNumberPlateRecognitionServerInterface.ANPRCandidate oneCandidate in oneResult.candidates)
                                {
                                    ANPRMatchDTO loOneANPRMatch = new ANPRMatchDTO();

                                    if (oneCandidate.plate.Length > 0)
                                    {
                                        loOneANPRMatch.sqlVehLicNoStr = oneCandidate.plate;
                                        loOneANPRMatch.sqlConfidenceStr = string.Format("{0:0.00}", oneCandidate.confidence) + "%";

                                        loOneANPRMatch.sqlVehLicStateStr = oneResult.region;

                                        // potentially useful 
                                        loOneANPRMatch.confidenceAsDouble = oneCandidate.confidence;

                                        loOneANPRMatch.matches_template = oneCandidate.matches_template;
                                        loOneANPRMatch.plate_index = oneResult.plate_index;

                                        loOneANPRMatch.region = oneResult.region;
                                        loOneANPRMatch.region_confidence = oneResult.region_confidence;

                                        loOneANPRMatch.processing_time_ms = oneResult.processing_time_ms;
                                        loOneANPRMatch.requested_topn = oneResult.requested_topn;
                                    }
                                    else
                                    {
                                        loOneANPRMatch.sqlVehLicNoStr = "No Plate Found";
                                        loOneANPRMatch.sqlConfidenceStr = "";
                                    }

                                    _ANPRResultsList.Add(loOneANPRMatch);

                                    // watch out for runaway result lists
                                    if (_ANPRResultsList.Count > 9)
                                    {
                                        break;
                                    }
                                }


                                // watch out for runaway result lists
                                if (_ANPRResultsList.Count > 9)
                                {
                                    break;
                                }
                            }

                            if (_listView != null)
                            {
                                _listView.DividerHeight = 0;
                                _listView.Adapter = new CustomANPRMatchAdapter(Activity, _ANPRResultsList, this);
                            }


                            // summarize
                            switch (_ANPRResultsList.Count)
                            {
                                case 0:
                                    {
                                        loText = "No Candidates Found";
                                        break;
                                    }
                                case 1:
                                    {
                                        loText = "1 Candidate Found";
                                        break;
                                    }
                                default:
                                    {
                                        loText = _ANPRResultsList.Count.ToString() + " Candidates Found";
                                        break;
                                    }
                            }
                            SetStatusText(loText);



                            // need to resize 
                            // ResizeDialogForResults();
                            ANPRDialogView.Invalidate();

                            _imageView.Invalidate();

                            // force to repaint correctly on Android 5? not when the image is too tall
                            if (gFilenameForLPRProcessing.Length > 0)
                            {
                                ShowImage();
                            }



                            // turning on the list, need to redraw
                            ANPRDialogView.RequestLayout();

                        }
                    }
                    else
                    {

                        // always keep updating the status as we go
                        loText = myANPRWebService.GetLastStatusTextMessage();
                        SetStatusText(loText);
                    }

                }
            }
            catch (System.Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "ANPRConfirmResultFragment.UpdateInfoFromWebService", ex.TargetSite.Name);
                System.Console.WriteLine("ANPRConfirmResultFragment::UpdateInfoFromWebService Exception source {0}: {1}", ex.Source, ex.ToString());
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

                        //int height = _imageView.Height;
                        //int width = _imageView.Width;


                        //if (( height == 0 ) || ( width == 0 ))
                        //{
                        //    height = 320;
                        //    width = 240;
                        //}


                        //int height = 384; // (int)Math.Round(Resources.DisplayMetrics.HeightPixels * 0.05);
                        //int width = 384; // (int)Math.Round(Resources.DisplayMetrics.WidthPixels * 0.05);
                        int height = (int)System.Math.Round(Resources.DisplayMetrics.HeightPixels * 0.15);
                        int width =  (int)System.Math.Round(Resources.DisplayMetrics.WidthPixels * 0.15);


                        bitmap = _photoFile.Path.LoadAndResizeBitmap(width, height);                       



                    }

                    _imageView.SetScaleType(ImageView.ScaleType.FitCenter);
                    //_imageView.SetScaleType(ImageView.ScaleType.CenterInside);

                    _imageView.SetImageBitmap(bitmap);
                                        
                }
            }
            //catch (System.IO.Exception exp)
            //{
            //    //
            //}
        }

    }


    class CustomANPRAlertDialogTimer : Object, IRunnable
    {

        private readonly ANPRConfirmResultFragment _Owner;
        private readonly Handler mHandler = new Handler();

        public CustomANPRAlertDialogTimer(ANPRConfirmResultFragment iOwnerDialog)
        {
            _Owner = iOwnerDialog;
        }
        public void Run()
        {
            mHandler.PostDelayed(UpdateTimer, 750);
        }
        private void UpdateTimer()
        {
            if (_Owner != null)
            {
                _Owner.UpdateInfoFromWebService();

                 mHandler.PostDelayed(UpdateTimer, 750);
            }
        }
    }

}