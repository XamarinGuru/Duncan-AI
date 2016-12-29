using System;
using System.IO;
using System.Drawing;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Duncan.AI.Droid;
using Duncan.AI.Droid.Common;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Java.Lang;
using SignaturePad;
using XMLConfig;
using String = System.String;


namespace Duncan.AI.Droid.Utils.PickerDialogs
{
    public class CustomSignaturePadView : SignaturePadView
    {
        private bool _AllowDrawing = true;
        public bool AllowDrawing
        {
            get { return _AllowDrawing; }
            set { _AllowDrawing = value; }
        }

        public CustomSignaturePadView(Context context):base(context)
        {
        }
 
        public override bool OnTouchEvent(MotionEvent e)
        {
            if (!_AllowDrawing) return false;
            return (base.OnTouchEvent(e));
        }
    }

    public class SignaturePickerDialog : DialogFragment 
    {
        private const int SIGNPAD_WITDTH = 1100;
        private const int SIGNPAD_HIEGHT = 600;
        private const int BUTTON_SPACE = 20;  //space between buttons

        private CustomSignaturePadView _signPad;
		private PanelField _panelField;
		private XMLConfig.IssStruct _callerStruct;
        private FormPanel _callBackClass;
        private AlertDialog.Builder _dialog;
        private Button _btnDone;
        private static string _prevImageFile = String.Empty;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            return _dialog.Create();
        }

        public SignaturePickerDialog(Context context, string strTitle, string strMessage, string fieldName, FormPanel callBack)
        {
            if (callBack == null) return;
            _callBackClass = callBack;
            _callerStruct = _callBackClass.thisStruct;
            int loPanelFieldIdx = 0;
            bool loPreviousSignFlag = false;
            int loFieldsCount = _callBackClass.thisStruct.Panels[_callBackClass.thisStruct.PanelPageCurrentDisplayedIndexSavedForResume].PanelFields.Count;
            for (loPanelFieldIdx = 0; loPanelFieldIdx < loFieldsCount; loPanelFieldIdx++)
            {
                _panelField = _callBackClass.thisStruct.Panels[_callBackClass.thisStruct.PanelPageCurrentDisplayedIndexSavedForResume].PanelFields[loPanelFieldIdx];
                if (_panelField.Name == fieldName)
                {
                    break;
                }
            }
            if (loPanelFieldIdx >= loFieldsCount) return; //we can't find the panel field
            _dialog = new AlertDialog.Builder(context);
            _dialog.SetTitle(strTitle);

            if (strMessage != null)
            {
                _dialog.SetMessage(strMessage); //"I swear to the accuracy of this notice and am authorized to issue it:");
            }
            //Create signature pad component
            CreateSignaturePad(context, _panelField.Name);
            
            var layoutVert = new LinearLayout(context) { Orientation = Orientation.Vertical };
            layoutVert.AddView(_signPad);

            //Check if we already have a stored sign or not
            Bitmap loSignBitmap = null;
            if (!String.IsNullOrEmpty(_panelField.Value))
            {
                loSignBitmap = Helper.BuildCustomSignatureBitmap(_callBackClass.thisStruct.sequenceId);
            }else if(_prevImageFile != ""){
                BitmapFactory.Options loOptions = new BitmapFactory.Options();
                loOptions.InJustDecodeBounds = false;
                loSignBitmap = BitmapFactory.DecodeFile(_prevImageFile, loOptions);
            }

            if(loSignBitmap != null)
            {
                _signPad.BackgroundImageView.SetImageBitmap(loSignBitmap);
                _signPad.BackgroundImageView.SetAdjustViewBounds(true);
                var layout = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.FillParent, RelativeLayout.LayoutParams.FillParent);
                layout.AddRule(LayoutRules.CenterVertical);
                layout.SetMargins(20, 20, 20, 20);
                _signPad.BackgroundImageView.LayoutParameters = layout;
                _signPad.AllowDrawing = false;
                
                //We have a previous signature, disable done button until user cleared the current
                loPreviousSignFlag = true;
            }
            _dialog.SetView(layoutVert);

            var layoutHorz = new LinearLayout(context) { Orientation = Orientation.Horizontal };

            //Calc the 3 buttons width and sapces
            int loBtnWidth = (SIGNPAD_WITDTH - (4 * BUTTON_SPACE)) /3;

            Button btnClear = new Button(context);
            btnClear.SetWidth(loBtnWidth);
            btnClear.Text = "Clear";
            btnClear.Click += btnSignatureClearClick;
            layoutHorz.AddView(btnClear);
            Button btnCancel = new Button(context);
            btnCancel.SetWidth(loBtnWidth);
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnSignatureCancelClick;
            layoutHorz.AddView(btnCancel);
            _btnDone = new Button(context);
            _btnDone.SetWidth(loBtnWidth);
            _btnDone.Text = "Done";
            _btnDone.Click += btnSingatureDoneClick;
            if (loPreviousSignFlag)
            {
                _btnDone.Clickable = false;
                _btnDone.SetTextColor(Android.Graphics.Color.Gray);
            }
            layoutHorz.AddView(_btnDone);
            layoutVert.AddView(layoutHorz);
            _dialog.SetView(layoutVert);
        }

        /*
        public void GetCurrentSignatureImage(CustomSignatureImageView signImage)
        {
            if (signImage == null) return;
            if (!String.IsNullOrEmpty(_panelField.Value))
            {
                Helper.BuildCustomSignatureImageView(signImage, _callerStruct.sequenceId);
            }
        }
		*/

        public bool IsSignaturePadBlank()
        {
            return _signPad.IsBlank;
        }

        public Bitmap GetSignatureBitmap()
        {
            return _signPad.GetImage();
        }

        public System.Drawing.PointF[] GetSignatureArrayOfPoints()
        {
            return _signPad.Points;
        }

        private void CreateSignaturePad(Context context, string Tag)
        {
            _signPad = new CustomSignaturePadView(context) { StrokeWidth = 10, Tag = Tag };
            var layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent)
            {
                //Width = SIGNPAD_WITDTH,
                Height = SIGNPAD_HIEGHT
            };
            _signPad.LayoutParameters = layoutParams;
        }

        
        private void btnSignatureClearClick(object sender, EventArgs e)
        {
            _signPad.Visibility = ViewStates.Visible;
            _signPad.Clear();
            _signPad.BackgroundImageView.SetImageBitmap(null);
            _signPad.BackgroundImageView.Activated = true;
            _signPad.AllowDrawing = true;
            //Enable done button
            _btnDone.Clickable = true;
            _btnDone.SetTextColor(Android.Graphics.Color.Black);
        }
        
        private void btnSignatureCancelClick(object sender, EventArgs e)
        {
            this.DismissAllowingStateLoss();
        }

        private void btnSingatureDoneClick(object sender, EventArgs e)
        {
            SaveCurrentSignature();
            _callBackClass.UpdateCurrentSignatureImage(_signPad.GetImage(Android.Graphics.Color.Black, true));
            this.DismissAllowingStateLoss();
        }

        private void SaveCurrentSignature()
        {
            var photoDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures) + "/" + Constants.MULTIMEDIA_FOLDERNAME;Java.IO.File MediaDir =
                                    new Java.IO.File(
                                                    Android.OS.Environment.GetExternalStoragePublicDirectory(
                                                    Android.OS.Environment.DirectoryPictures), Constants.MULTIMEDIA_FOLDERNAME);
            Java.IO.File sigFile = new Java.IO.File(MediaDir,
                                                    _callBackClass.thisStruct.sequenceId +
                                                    Constants.SIG_FILE_SUFFIX);
            // get the bmp image from the sig pad
            Bitmap loSignBmp = GetSignatureBitmap();

            // save the image to the device
            using (FileStream fileStream = System.IO.File.OpenWrite(sigFile.ToString()))
            {
                loSignBmp.Compress(Bitmap.CompressFormat.Png, 0, fileStream);
            }

            // store the filename for later retrieval
            _prevImageFile = photoDirectory + "/" + sigFile.Name;


            System.Drawing.PointF[] points = GetSignatureArrayOfPoints(); //sigPad.Points;
            
            // convert to an array of int Points (instead of floats, to get fewer pixel coordinates
            var pointsArray =
                points.ToList()
                      .Select(
                          x =>
                          new System.Drawing.Point(Convert.ToInt32(x.X / 3), Convert.ToInt32(x.Y / 5)))
                      .ToArray();

            String pointsAsHex = "";

            // scale the down image to fit what duncan expects
            for (int k = 0; k < pointsArray.Count(); k++)
            {
                // don't add this point if it is the same as the last one
                if (k > 0 && pointsArray[k].X == pointsArray[k - 1].X &&
                    pointsArray[k].Y == pointsArray[k - 1].Y)
                    continue;

                // don't add this point if it is negative
                if (pointsArray[k].X < 0 || pointsArray[k].Y < 0)
                    continue;

                if (pointsArray[k].IsEmpty || (pointsArray[k].X == 0 && pointsArray[k].Y == 0))
                {
                    // duncan needs a line break to start a new drawing line
                    pointsAsHex += "\n";
                }
                else
                {
                    // add a comma when needed
                    if (!System.String.IsNullOrEmpty(pointsAsHex) && k > 0 &&
                        pointsAsHex.Substring(pointsAsHex.Length - 1) != "\n")
                        pointsAsHex += ",";

                    // get the hex of each integer
                    pointsAsHex += pointsArray[k].X.ToString("X") + " " +
                                   pointsArray[k].Y.ToString("X");
                }
            }

            _panelField.Value = pointsAsHex;
        }

    }
}