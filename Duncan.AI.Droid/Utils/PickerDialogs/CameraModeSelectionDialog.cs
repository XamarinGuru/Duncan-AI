using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Duncan.AI.Droid.Utils.HelperManagers;

namespace Duncan.AI.Droid.Utils.PickerDialogs
{
    class CameraModeSelectionDialog : DialogFragment
    {
        private static AlertDialog.Builder builder = null;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }
        
        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            if(builder != null)
                return builder.Create();


            return null;
        }
         
 
        public CameraModeSelectionDialog()
        {
        }

        public CameraModeSelectionDialog(Context context)
        {
            builder = new AlertDialog.Builder(context);
            builder.SetIconAttribute(Android.Resource.Attribute.Action);
            //builder.SetTitle("Select Camera Mode");
            //builder.SetIcon(Resource.Drawable.ic_printer_black_48);
            var layoutVert = new LinearLayout(context) { Orientation = Orientation.Vertical };
                        
            Button loButtonLPR = new Button(context);
            loButtonLPR.Text = "Capture Plate (LPR)"; // "LPR";
            Helper.SetTypefaceForButton(loButtonLPR, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);
            //loButtonLPR.SetTextColor(Resources.GetColor(Resource.Color.civicsmart_black));
            loButtonLPR.SetBackgroundColor(Android.Graphics.Color.White);
            var loLayout = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,ViewGroup.LayoutParams.WrapContent);
            loLayout.AddRule(LayoutRules.AlignLeft);
            loButtonLPR.LayoutParameters = loLayout;
            loButtonLPR.Click += delegate
            {
                try
                {                   
                    //First check if there is a valid fragment or not
                    LaunchLPRActivity((MainActivity)((Activity)context));                    
                    this.DismissAllowingStateLoss();
                }
                catch (Exception exp)
                {
                    //exp.PrintStackTrace();
                }
            };

            Button loButtonPhoto = new Button(context);
            loButtonPhoto.Text = "Take Photo";
            Helper.SetTypefaceForButton(loButtonPhoto, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);
            //loButtonPhoto.SetTextColor(Resources.GetColor(Resource.Color.civicsmart_black));
            loButtonPhoto.SetBackgroundColor(Android.Graphics.Color.White);
            loButtonPhoto.LayoutParameters = loLayout;
            loButtonPhoto.Click += delegate
            {
                try
                {
                    DroidContext.mainActivity.LaunchCameraForPendingAttachments();
                    this.DismissAllowingStateLoss();
                }
                catch (Exception exp)
                {
                    //exp.PrintStackTrace();
                }
            };


            Button loButtonVideo = new Button(context);
            loButtonVideo.Text = "Record Video";
            Helper.SetTypefaceForButton(loButtonVideo, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);
            loButtonVideo.SetTextColor(Android.Graphics.Color.DimGray);
            loButtonVideo.SetBackgroundColor(Android.Graphics.Color.White);
            loButtonVideo.LayoutParameters = loLayout;
            loButtonVideo.Click += delegate
            {                
            };

            Button loButtonAudio = new Button(context);
            loButtonAudio.Text = "Record Audio";
            Helper.SetTypefaceForButton(loButtonAudio, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);
            loButtonAudio.SetTextColor(Android.Graphics.Color.DimGray);
            loButtonAudio.SetBackgroundColor(Android.Graphics.Color.White);
            loButtonAudio.LayoutParameters = loLayout;
            loButtonAudio.Click += delegate
            {
            };

            Button loButtonScan = new Button(context);
            loButtonScan.Text = "Scan Barcode";
            Helper.SetTypefaceForButton(loButtonScan, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);
            //loButtonScan.SetTextColor(Android.Graphics.Color.DimGray);
            loButtonScan.SetBackgroundColor(Android.Graphics.Color.White);
            loButtonScan.LayoutParameters = loLayout;
            loButtonScan.Click += delegate
            {
               
                try
                {                   
                    LaunchScannerActivity((MainActivity)((Activity)context));                    
                    this.DismissAllowingStateLoss();
                }
                catch (Exception exp)
                {
                    //exp.PrintStackTrace();
                }

            };


// TODO - activate video/audio modes

            layoutVert.AddView(loButtonLPR);
            layoutVert.AddView(loButtonPhoto);
#if _enable_audio_video_
            layoutVert.AddView(loButtonVideo);
            layoutVert.AddView(loButtonAudio);
#endif
#if _enable_scanner_
            layoutVert.AddView(loButtonScan);
#endif

            layoutVert.SetBackgroundColor(Android.Graphics.Color.White);            
            builder.SetView(layoutVert);            
        }

        void LaunchLPRActivity(MainActivity iMainActivity)
        {

            // if we have a valid destination, we can redirect
            if (iMainActivity.gFragmentLastShown != null)
            {
                {
                    if (iMainActivity.gFragmentLastShown is CommonFragment)
                    {
                        var commonFragment = (CommonFragment)iMainActivity.gFragmentLastShown;
                        if (commonFragment._formPanel != null)
                        {
                            commonFragment._formPanel.btnPlateOCRClick(null, null);
                            return;
                        }
                    }
                }
            }

            // if we got here, we couldn't launch - display some kind of message?            
            var loBuilder = new AlertDialog.Builder(iMainActivity);
            loBuilder.SetTitle("LPR");
            loBuilder.SetMessage("Please select a form before capturing.");
            loBuilder.SetPositiveButton("OK", delegate
            {
                this.DismissAllowingStateLoss();
            });
            loBuilder.Show();
        }


        void LaunchScannerActivity(MainActivity iMainActivity)
        {

            // if we have a valid destination, we can redirect
            if (iMainActivity.gFragmentLastShown != null)
            {
                {
                    if (iMainActivity.gFragmentLastShown is CommonFragment)
                    {
                        var commonFragment = (CommonFragment)iMainActivity.gFragmentLastShown;
                        if (commonFragment._formPanel != null)
                        {
                            commonFragment._formPanel.btnScanClick(null, null);
                            return;
                        }
                    }
                }
            }

            // if we got here, we couldn't launch - display some kind of message?            
            var loBuilder = new AlertDialog.Builder(iMainActivity);
            loBuilder.SetTitle("Scanner");
            loBuilder.SetMessage("Please select a form before capturing.");
            loBuilder.SetPositiveButton("OK", delegate
            {
                this.DismissAllowingStateLoss();
            });
            loBuilder.Show();
        }


    }
}