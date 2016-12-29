using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Java.IO;

using Android.App;
using Android.Provider;
using Android.Bluetooth;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Zebra.Android.Printer;
using Com.Zebra.Android.Comm;
using Duncan.AI.Droid.Common;
using Duncan.AI.Droid.Utils.HelperManagers;
using Duncan.AI.Droid.Utils.PrinterSupport;
using Duncan.AI.Droid.Utils.PickerDialogs;

using Reino.ClientConfig;

using Duncan.AI.Droid.Utils;

namespace Duncan.AI.Droid
{
    public class NotesReviewDetailFragment : Fragment
    {
        View _view;

        string _tagName;
        string _structName;

        XMLConfig.IssStruct _struct;

        List<ParkNoteDTO> _parkNoteDTOs;

        Java.IO.File _photoFile;
        Java.IO.File _photoDir;


        LinearLayout linearLayoutMain;

        TextView _RecordInfoPrimary;
        TextView _RecordInfoSecondary;

        TextView _RecordInfoVehicleInfo;
        TextView _RecordInfoVoidStatus;
        TextView _RecordInfoReissueStatus;
        TextView _NotesReviewTitle;


        TextView _RecordInfoDetails;
        TextView _statusTextView;




        LinearLayout _RecordInfoContainer;

        LinearLayout _AutoISSUE_Toolbar_StatusAction;

        ImageView reproductionImage;
        //ScrollView _ticketscrollviewMain;

        //LinearLayout _listViewNotesAndAttachmentsLayout;
       // ListView _listViewNotesAndAttachments;


        List<string> _RecordActionOptions = new List<string>();
        List<string> _NotesActionOptions = new List<string>();

        DataRow _RawDetailRowVoid = null;
        DataRow _RawDetailRowReissue = null;
        DataRow _RawDetailRowContinuance = null;



        //Button _notesBtn;
       // Button _btnAction;
       // Button _printBtn;
        Button _doneBtn;

        string _ticketId;
        string _parkNoteRowId;

        ParkingDTO _parkingDTO;
        ParkNoteDTO _parkNoteDTO;

        ProgressDialog _progressDialog;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // AJW - always call the superclass
            base.OnCreateView(inflater, container, savedInstanceState);


            _structName = Arguments.GetString("structName", null);
            _struct = DroidContext.XmlCfg.GetStructByName(_structName);


            _tagName = Helper.BuildNotesReviewDetailFragmentTag(_structName);


            _view = inflater.Inflate(Resource.Layout.NotesReviewDetailFragment, null);

            _RecordInfoPrimary = _view.FindViewById<TextView>(Resource.Id.recordinfoprimary);
            Helper.SetTypefaceForTextView(_RecordInfoPrimary, FontManager.cnTextMessageLargeTypeface, FontManager.cnTextMessageLargeSizeSp);
            _RecordInfoPrimary.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_black));


            _RecordInfoSecondary = _view.FindViewById<TextView>(Resource.Id.recordinfosecondary);
            Helper.SetTypefaceForTextView(_RecordInfoSecondary, FontManager.cnTextMessageMediumTypeface, FontManager.cnTextMessageMedimSizeSp);
            _RecordInfoSecondary.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_black));



            _RecordInfoVehicleInfo = _view.FindViewById<TextView>(Resource.Id.licenseplate);
            Helper.SetTypefaceForTextView(_RecordInfoVehicleInfo, FontManager.cnTextMessageMediumTypeface, FontManager.cnTextMessageMedimSizeSp);
            _RecordInfoVehicleInfo.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_black));


            _RecordInfoVoidStatus = _view.FindViewById<TextView>(Resource.Id.voidstatus);
            Helper.SetTypefaceForTextView(_RecordInfoVoidStatus, FontManager.cnTextMessageMediumTypeface, FontManager.cnTextMessageMedimSizeSp);
            _RecordInfoVoidStatus.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_black));

            _RecordInfoReissueStatus = _view.FindViewById<TextView>(Resource.Id.reissuestatus);
            Helper.SetTypefaceForTextView(_RecordInfoReissueStatus, FontManager.cnTextMessageMediumTypeface, FontManager.cnTextMessageMedimSizeSp);
            _RecordInfoReissueStatus.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_black));


            _NotesReviewTitle = _view.FindViewById<TextView>(Resource.Id.notesreviewtitle);
            Helper.SetTypefaceForTextView(_NotesReviewTitle, FontManager.cnTextMessageMediumTypeface, FontManager.cnTextMessageMedimSizeSp);
            _NotesReviewTitle.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_black));


            // _RecordInfoDetails = _view.FindViewById<TextView>(Resource.Id.recordinfodetails);

            _RecordInfoContainer = _view.FindViewById<LinearLayout>(Resource.Id.RecordInfoContainer);


#if _old_
            _listViewNotesAndAttachmentsLayout = _view.FindViewById<LinearLayout>(Resource.Id.linearLayoutNotesList);

            _listViewNotesAndAttachments = _view.FindViewById<ListView>(Resource.Id.noteslist);
            if (_listViewNotesAndAttachments != null)
            {
                SetListItemClick();
            }
#endif


            linearLayoutMain = _view.FindViewById<LinearLayout>(Resource.Id.LinearLayoutMain);
            reproductionImage = _view.FindViewById<ImageView>(Resource.Id.imageReproduction);
            //_ticketscrollviewMain = _view.FindViewById<ScrollView>(Resource.Id.ticketdetailscrollviewMain);



            // action toolbar 
            _AutoISSUE_Toolbar_StatusAction = _view.FindViewById<LinearLayout>(Resource.Id.toolbar_bottom_review_layout);
            if (_AutoISSUE_Toolbar_StatusAction != null)
            {
                _AutoISSUE_Toolbar_StatusAction.SetBackgroundResource(Resource.Drawable.autoissue_toolbar_finalizing_background);
            }




#if _old_

            //action -> popop single choice "Void", "Re-Issue" "Issue More", "Issue Multiple", etc.
            _btnAction = _view.FindViewById<Button>(Resource.Id.btnAction);
            if (_btnAction != null)
            {
                Helper.SetTypefaceForButton(_btnAction, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);
                _btnAction.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_black));
                _btnAction.Click += btnActionClickEvaluation;
            }



            _printBtn = _view.FindViewById<Button>(Resource.Id.btnReviewPrint);
            if (_printBtn != null)
            {
                Helper.SetTypefaceForButton(_printBtn, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);
                _printBtn.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_black));
                _printBtn.Click += BtnPrintClick;
            }



            _notesBtn = _view.FindViewById<Button>(Resource.Id.btnAddNotes);
            if (_notesBtn != null)
            {
                Helper.SetTypefaceForButton(_notesBtn, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);
                _notesBtn.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_black));
                _notesBtn.Click += btnNotesActionClickEvaluation;
            }
#endif




            _doneBtn = _view.FindViewById<Button>(Resource.Id.BackButton2);
            if (_doneBtn != null)
            {
                Helper.SetTypefaceForButton(_doneBtn, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);
                _doneBtn.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_black));
                _doneBtn.Click += BtnDoneClick;
            }



            _photoDir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), Constants.MULTIMEDIA_FOLDERNAME);


            return _view;
        }



        void GetRecordStatusDetails(ParkingDTO oneItem, out bool ioCiteIsVoid, out bool ioCiteIsReIssued, out bool ioCiteIsContinuance)
        {
            // default all 
            ioCiteIsVoid = false;
            ioCiteIsReIssued = false;
            ioCiteIsContinuance = false;

            _RawDetailRowVoid = null;
            _RawDetailRowReissue = null;
            _RawDetailRowContinuance = null;



            try
            {
                if (_struct != null)
                {
                    if (_struct._TIssStruct.StructLogicObj != null)
                    {

                        if (string.IsNullOrEmpty(oneItem.DBRowId.Trim()) == true)
                        {
                            // empty row ID - nothing to look for
                            return;
                        }



                        if (_struct._TIssStruct.StructLogicObj is CiteStructLogicAndroid)
                        {
                            Int32 loRowIDAsInt = -1;
                            Int32.TryParse(oneItem.DBRowId, out loRowIDAsInt);
                            if (loRowIDAsInt >= 0)
                            {
                                // is it void? null if not
                                _RawDetailRowVoid = ((CiteStructLogicAndroid)_struct._TIssStruct.StructLogicObj).GetFirstVoidRecordForMasterKey(loRowIDAsInt);
                                ioCiteIsVoid = (_RawDetailRowVoid != null);

                                // is it reissued? null if not
                                _RawDetailRowReissue = ((CiteStructLogicAndroid)_struct._TIssStruct.StructLogicObj).GetFirstReissueRecordForMasterKey(loRowIDAsInt);
                                ioCiteIsReIssued = (_RawDetailRowReissue != null);

                                // is it a continuance? null if not
                                _RawDetailRowContinuance = ((CiteStructLogicAndroid)_struct._TIssStruct.StructLogicObj).GetFirstContinuanceRecordForMasterKey(loRowIDAsInt);
                                ioCiteIsContinuance = (_RawDetailRowContinuance != null);

                            }

                        }

                    }
                }
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, e.Source, "IssueReviewDetail: GetCiteDetailInfo");
                System.Console.WriteLine("Exception source: {0}", e.Source);
            }

        }


        private string FormatDateTimeStringInStructMask(string iDateStringInFixedDBFormat, string iTimeStringInFixedDBFormat)
        {
            // default formatting
            string loFormattedResultText = iDateStringInFixedDBFormat + "  " + iTimeStringInFixedDBFormat;

            // let's see if we can format this in the defined format of the structure
            if (_struct != null)
            {
                if (_struct.fDisplayFormattingInfo != null)
                {
                    // build it up in pieces
                    StringBuilder oneFormattedInfo = new StringBuilder();

                    // issue date
                    if (
                         (string.IsNullOrEmpty(_struct.fDisplayFormattingInfo.fStructDateMask) == false) &&
                         (string.IsNullOrEmpty(iDateStringInFixedDBFormat) == false)
                        )
                    {
                        // would prefer to use the masks
                        oneFormattedInfo.Append(CultureDisplayFormatLogic.ConvertDateStringToDisplayFormatLocalTime(
                            iDateStringInFixedDBFormat,
                            Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK,
                            _struct.fDisplayFormattingInfo.fStructDateMask) + " ");
                    }
                    else
                    {
                        // default back to original
                        oneFormattedInfo.Append(iDateStringInFixedDBFormat + "  ");
                    }


                    // issue time
                    if (
                        (string.IsNullOrEmpty(_struct.fDisplayFormattingInfo.fStructTimeMask) == false) &&
                        (string.IsNullOrEmpty(iTimeStringInFixedDBFormat) == false)
                       )
                    {
                        // would prefer to use the masks
                        oneFormattedInfo.Append(CultureDisplayFormatLogic.ConvertDateStringToDisplayFormatLocalTime(
                            iTimeStringInFixedDBFormat,
                            Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK,
                            _struct.fDisplayFormattingInfo.fStructTimeMask) + " ");
                    }
                    else
                    {
                        // default back to original
                        oneFormattedInfo.Append(iTimeStringInFixedDBFormat + "  ");
                    }


                    // final aggregation
                    loFormattedResultText = oneFormattedInfo.ToString();
                }


            }

            return loFormattedResultText;
        }



        private int GetRotationAngle()
        {
            //Display display = this.WindowManager.DefaultDisplay;
            Display display =  DroidContext.mainActivity.WindowManager.DefaultDisplay;

            int rotation = 0;
            switch (display.Rotation)
            {
                case SurfaceOrientation.Rotation0: // This is display orientation
                    rotation = 90;
                    break;
                case SurfaceOrientation.Rotation90:
                    rotation = 0;
                    break;
                case SurfaceOrientation.Rotation180:
                    rotation = 270;
                    break;
                case SurfaceOrientation.Rotation270:
                    rotation = 180;
                    break;
            }

            return rotation;
        }



        private void ShowImage()
        {
            Bitmap bitmap = null;
            if (_photoFile.AbsolutePath.Contains(Constants.VIDEO_FILE_SUFFIX))
            {
                bitmap = Android.Media.ThumbnailUtils.CreateVideoThumbnail(_photoFile.AbsolutePath, ThumbnailKind.MiniKind);

                //reproductionImage.SetScaleType(ImageView.ScaleType.FitCenter); 
                reproductionImage.SetImageBitmap(bitmap);
            }
            else if (_photoFile.AbsolutePath.Contains(Constants.PHOTO_FILE_SUFFIX))
            {
                // resize the bitmap to fit the display, Loading the full sized image will consume too much memory 
                //int height = reproductionImage.Height;
                //int width = Resources.DisplayMetrics.WidthPixels;
                //bitmap = _photoFile.Path.LoadAndResizeBitmap(width, height);


                int height = (int)Math.Round(Resources.DisplayMetrics.HeightPixels * 0.8);
                int width = (int)Math.Round(Resources.DisplayMetrics.WidthPixels * 0.8);

                bitmap = BitmapHelpers.decodeSampledBitmapFromFile(_photoFile.Path, width, height);


                var rotatedBitmap = BitmapHelpers.RotateBitmap(bitmap, GetRotationAngle());


                //reproductionImage.SetScaleType(ImageView.ScaleType.FitCenter); 
                reproductionImage.SetImageBitmap(rotatedBitmap);

                // clean up
                bitmap.Recycle();
                bitmap.Dispose();
                bitmap = null;

            }

        }


        private async void LoadMultimediaNoteForDisplay()
        {
            // if it is an existing note
            if (_parkNoteRowId != null)
            {
                var parkingSequenceADO = new ParkingSequenceADO();
                Log.Debug("NoteDetailFragment NoteId", _parkNoteRowId);
                Task<ParkNoteDTO> result = parkingSequenceADO.GetParkNoteByNoteId(_parkNoteRowId);
                _parkNoteDTO = await result;

                //_txtNoteId.Text = _parkNoteDTO.DBRowId;
                //_txtNoteMemo.Text = _parkNoteDTO.NotesMemo;

                if (_NotesReviewTitle != null)
                {
                    if (_parkNoteDTO != null)
                    {
                        _NotesReviewTitle.Text = "Photo Added " + FormatDateTimeStringInStructMask(_parkNoteDTO.NoteDate, _parkNoteDTO.NoteTime);
                    }
                }


                _photoFile = new Java.IO.File(_photoDir, _parkNoteDTO.MultiMediaNoteFileName);

                ShowImage();
            }
            else
            {
                if (_NotesReviewTitle != null)
                {
                    _NotesReviewTitle.Text = "Multimedia Not Found";
                }

                _parkNoteDTO = null;
                //_txtNoteId.Text = String.Empty;
                //_txtNoteMemo.Text = String.Empty;
                _photoFile = null;

                reproductionImage.SetImageBitmap(null);
            }
        }

        public async Task<ParkingDTO> RefreshDisplayedRecord()
        {

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity);
            //ISharedPreferencesEditor editor = prefs.Edit();

            _ticketId = prefs.GetString(Constants.TICKETID, null);
            _parkNoteRowId = prefs.GetString(Constants.PARKNOTE_ROWID, null);



            var commonADO = new CommonADO();
            Log.Debug("NotesReviewDetailFragment _ticketId", _ticketId);
            Log.Debug("IssueReviewFragment _structName", _structName);

            //Task<ParkingDTO> result = commonADO.GetRowInfoByRowId(loRowId, _structName);
            Task<ParkingDTO> result = commonADO.GetRowInfoBySequenceId(_ticketId, _structName);
            _parkingDTO = await result;

            // now load the notes details while the screen is being painted
            //GetNotesByTicket(_parkingDTO.sqlIssueNumberStr);  // AJW - TODO this needs to reference _id 
            LoadMultimediaNoteForDisplay();

            // _RecordInfoPrimary.Text = "Citation Review";
            _RecordInfoPrimary.Text = _parkingDTO.ISSUENO_DISPLAY;


            // default formatting
            _RecordInfoSecondary.Text = "Issued " + FormatDateTimeStringInStructMask(_parkingDTO.sqlIssueDateStr, _parkingDTO.sqlIssueTimeStr);


            _RecordInfoVehicleInfo.Text = _parkingDTO.sqlVehLicNoStr + " " + _parkingDTO.sqlVehLicStateStr;




            //_RecordInfoDetails.Text = _parkingDTO.LOCATION_DISPLAY;

            // hide this for now, maybe only show for mark mode or records without print picture?
            //_RecordInfoContainer.Visibility = ViewStates.Gone;



            bool ioCiteIsVoid = false;
            bool ioCiteIsReIssued = false;
            bool ioCiteIsContinuance = false;
            GetRecordStatusDetails(_parkingDTO, out ioCiteIsVoid, out ioCiteIsReIssued, out ioCiteIsContinuance);


            if (ioCiteIsVoid)
            {
                string loVoidDateAsStr = Helper.GetSafeColumnStringValueFromDataRow(_RawDetailRowVoid, Constants.RECCREATIONDATE_COLUMN);
                string loVoidTimeAsStr = Helper.GetSafeColumnStringValueFromDataRow(_RawDetailRowVoid, Constants.RECCREATIONTIME_COLUMN);

                _RecordInfoVoidStatus.Text = "Voided " + FormatDateTimeStringInStructMask(loVoidDateAsStr, loVoidTimeAsStr);
                _RecordInfoVoidStatus.Visibility = ViewStates.Visible;
            }
            else
            {
                _RecordInfoVoidStatus.Visibility = ViewStates.Gone;
            }



            if (ioCiteIsReIssued)
            {
                string loReIssueDateAsStr = Helper.GetSafeColumnStringValueFromDataRow(_RawDetailRowReissue, Constants.RECCREATIONDATE_COLUMN);
                string loReIssueTimeAsStr = Helper.GetSafeColumnStringValueFromDataRow(_RawDetailRowReissue, Constants.RECCREATIONTIME_COLUMN);

                _RecordInfoReissueStatus.Text = "Reissued " + FormatDateTimeStringInStructMask(loReIssueDateAsStr, loReIssueTimeAsStr);
                _RecordInfoReissueStatus.Visibility = ViewStates.Visible;
            }
            else
            {
                _RecordInfoReissueStatus.Visibility = ViewStates.Gone;
            }






            // file extension is added by reader
            //string loTicketImageName = Helper.GetTIssueFormBitmapImageFileNameOnly(_structName, _parkingDTO.sqlIssueNumberStr, DateTime.Today); // TODOD -needs datetime in proper formats


            //// for debug only, show alternate views
            //var sigImg2 = _view.FindViewById<ImageView>(Resource.Id.sigImg);
            //sigImg2 = Helper.GetTIssueFormBitmapImageFromStorage(sigImg2, loTicketImageName);



            //reproductionImage = Helper.GetTIssueFormBitmapImageFromStorage(reproductionImage, loTicketImageName);


            // start at the top
            //_ticketscrollviewMain.ScrollTo(0, 0);

            //// put the keyboard away
            //Context _context = this.Activity;
            //if (_context != null)
            //{
            //    Helper.HideKeyboardFromFragment(_context);
            //}


            // _formPanel.PanelRootPageScrollView.SmoothScrollTo(0, 0);


            Activity.RunOnUiThread(() =>
            {
                // start at the top
                //_ticketscrollviewMain.SmoothScrollTo(0, 0);


                // put the keyboard away
                Context _context = this.Activity;
                if (_context != null)
                {
                    Helper.HideKeyboardFromFragment(_context);
                }
            });





            return _parkingDTO;
        }













        private void BtnDoneClick(object sender, EventArgs e)
        {



            // go back to where we came from
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            String prevFragName = prefs.GetString(Constants.PREVIOUS_FRAGMENT, string.Empty);
            string structName = prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, string.Empty);


            // we are leaving, don't come back here
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(Constants.PREVIOUS_FRAGMENT, string.Empty);
            editor.Apply();


            // move back in the direction we came from 
            prevFragName = DroidContext.MyFragManager.PopInternalBackstack();



            // find and hide ourselves
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();

            Fragment reviewDetailFragment = FragmentManager.FindFragmentByTag(_tagName);
            //Fragment reviewDetailFragment = FragmentManager.FindFragmentByTag(Constants.ISSUE_REVIEW_DETAIL_FRAGMENT_TAG);

            if (reviewDetailFragment != null)
            {
                fragmentTransaction.Hide(reviewDetailFragment);
            }



            if (string.IsNullOrEmpty(prevFragName) == true)
            {
                // not sure where to go.... this is the data type we need
                prevFragName = Helper.BuildNotesReviewSelectFragmentTag(_structName);
            }

            // back to the ticket detail
            var dtlFragment = FragmentManager.FindFragmentByTag(prevFragName);
            if ((dtlFragment != null) && (dtlFragment is NotesReviewSelectFragment))
            {
                fragmentTransaction.Show(dtlFragment);

                // since can't add notes or otherwise change, the previous dataset should be OK?
                //((NotesReviewSelectFragment)dtlFragment).RefreshDisplayedRecord();
            }
            else if (dtlFragment != null)
            {
                // don't know what kind of fragment it is
                fragmentTransaction.Show(dtlFragment);
            }
            else
            {
                var fragment = new NotesReviewSelectFragment { Arguments = new Bundle() };
                fragment.Arguments.PutString("structName", _structName);

                fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, Helper.BuildIssueReviewFragmentTag(_structName));

                fragment.RefreshDisplayedRecord();

            }


            fragmentTransaction.Commit();
        }



    }
}



