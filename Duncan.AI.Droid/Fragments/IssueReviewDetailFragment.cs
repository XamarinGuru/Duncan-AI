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
    public class IssueReviewDetailFragment : Fragment
    {
        View _view;

        string _tagName;
        string _structName;

        XMLConfig.IssStruct _struct;

        List<ParkNoteDTO> _parkNoteDTOs;

        Java.IO.File _photoFile;


        LinearLayout linearLayoutMain;

        TextView _RecordInfoPrimary;
        TextView _RecordInfoSecondary;

        TextView _RecordInfoVehicleInfo;
        TextView _RecordInfoVoidStatus;
        TextView _RecordInfoReissueStatus;


        TextView _RecordInfoDetails;
        TextView _statusTextView;




        LinearLayout _RecordInfoContainer;

        LinearLayout _AutoISSUE_Toolbar_StatusAction;

        ImageView reproductionImage;
        ScrollView _ticketscrollviewMain;

        LinearLayout _listViewNotesAndAttachmentsLayout;
        ListView _listViewNotesAndAttachments;


        List<string> _RecordActionOptions = new List<string>();
        List<string> _NotesActionOptions = new List<string>();

        DataRow _RawDetailRowVoid = null;
        DataRow _RawDetailRowReissue = null;
        DataRow _RawDetailRowContinuance = null;



        Button _notesBtn;
        Button _btnAction;
        Button _printBtn;
        Button _doneBtn;

        string _ticketId;
        ParkingDTO _parkingDTO;
        ProgressDialog _progressDialog;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // AJW - always call the superclass
            base.OnCreateView(inflater, container, savedInstanceState);


            _structName = Arguments.GetString("structName", null);
            _struct = DroidContext.XmlCfg.GetStructByName(_structName);


            _tagName = Helper.BuildIssueReviewFragmentTag(_structName);


            _view = inflater.Inflate(Resource.Layout.IssueReviewDetailFragment, null);

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




            // _RecordInfoDetails = _view.FindViewById<TextView>(Resource.Id.recordinfodetails);

            _RecordInfoContainer = _view.FindViewById<LinearLayout>(Resource.Id.RecordInfoContainer);


            _listViewNotesAndAttachmentsLayout = _view.FindViewById<LinearLayout>(Resource.Id.linearLayoutNotesList);

            _listViewNotesAndAttachments = _view.FindViewById<ListView>(Resource.Id.noteslist);
            if (_listViewNotesAndAttachments != null)
            {
                SetListItemClick();
            }



            linearLayoutMain = _view.FindViewById<LinearLayout>(Resource.Id.LinearLayoutMain);
            reproductionImage = _view.FindViewById<ImageView>(Resource.Id.imageReproduction);
            _ticketscrollviewMain = _view.FindViewById<ScrollView>(Resource.Id.ticketdetailscrollviewMain);



            // action toolbar 
            _AutoISSUE_Toolbar_StatusAction = _view.FindViewById<LinearLayout>(Resource.Id.toolbar_bottom_review_layout);
            if (_AutoISSUE_Toolbar_StatusAction != null)
            {
                _AutoISSUE_Toolbar_StatusAction.SetBackgroundResource(Resource.Drawable.autoissue_toolbar_finalizing_background);
            }




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





            _doneBtn = _view.FindViewById<Button>(Resource.Id.BackButton2);
            if (_doneBtn != null)
            {
                Helper.SetTypefaceForButton(_doneBtn, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);
                _doneBtn.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_black));
                _doneBtn.Click += BtnDoneClick;
            }





            return _view;
        }

        //Async Task to retrieve Notes by a Ticket Id from DB
        private async void GetNotesByTicket(string iIssueNo)
        {
            //ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            //String sequenceId = prefs.GetString(Constants.ISSUENO_COLUMN, null);
            string sequenceId = iIssueNo;



            // hide by default
            _listViewNotesAndAttachments.Visibility = ViewStates.Gone;
            _listViewNotesAndAttachmentsLayout.Visibility = ViewStates.Gone;

            var parkingSequenceADO = new ParkingSequenceADO();
            Task<List<ParkNoteDTO>> result = parkingSequenceADO.GetParkNotesBySequenceId(sequenceId);

            // await! control returns to the caller and the task continues to run on another thread
            _parkNoteDTOs = await result;

            //_listViewNotesAndAttachments.Adapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, _parkNoteDTOs.Select(x => x.NotesMemo).ToArray());
            _listViewNotesAndAttachments.Adapter = new CustomNoteStructRecordLookupAdapter(Activity, _parkNoteDTOs);

            if (_parkNoteDTOs.Count > 0)
            {
                _listViewNotesAndAttachmentsLayout.Visibility = ViewStates.Visible;
                _listViewNotesAndAttachments.Visibility = ViewStates.Visible;
            }


            _listViewNotesAndAttachmentsLayout.Invalidate();
            _listViewNotesAndAttachmentsLayout.RequestLayout();

            _listViewNotesAndAttachments.Invalidate();
            _listViewNotesAndAttachments.RequestLayout();
        }

        void SetListItemClick()
        {
            _listViewNotesAndAttachments.ItemClick += (sender, e) =>
            {

#if _not_anymore_to_be_replaced_
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
                ISharedPreferencesEditor editor = prefs.Edit();

                if (_parkNoteDTOs != null)
                {
                    ParkNoteDTO parkNoteDTO = _parkNoteDTOs[e.Position];

                    editor.PutString(Constants.PARKNOTE_ROWID, parkNoteDTO.DBRowId);
                    editor.Apply();

                    LoadNoteDetails();
                }
#endif
            };
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


        public async Task<ParkingDTO> RefreshDisplayedRecord()
        {

            // on record refresh, iterate through void form and determine which buttons should be available on this sceen

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity);
            //ISharedPreferencesEditor editor = prefs.Edit();

            // _structName = prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);  lets not change our structure identity!!!

            _ticketId = prefs.GetString(Constants.TICKETID, null);
            //string loRowId = prefs.GetString(Constants.ID_COLUMN, null);

            //editor.Remove (Constants.ISSUENO_COLUMN);
            //editor.Apply();

            var commonADO = new CommonADO();
            Log.Debug("IssueReviewDetailFragment _ticketId", _ticketId);
            Log.Debug("IssueReviewFragment _structName", _structName);

            //Task<ParkingDTO> result = commonADO.GetRowInfoByRowId(loRowId, _structName);
            Task<ParkingDTO> result = commonADO.GetRowInfoBySequenceId(_ticketId, _structName);
            _parkingDTO = await result;

            // now load the notes details while the screen is being painted
            //GetNotesByTicket(_parkingDTO.sqlIssueNumberStr);  // AJW - TODO this needs to reference _id 


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






            // check the legacy layout definition to determine action buttons
            bool loLegacyButtonVoidDefined = false;
            bool loLegacyButtonPrintDefined = false;
            bool loLegacyButtonIssueMoreDefined = false;
            bool loLegacyButtonReIssueDefined = false;


            // cancel, re-issue, etc 
            if (_struct != null)
            {
                if (_struct._TIssStruct != null)
                {
                    if (_struct._TIssStruct is TCiteStruct)
                    {
                        if (_struct._TIssStruct.StructLogicObj is IssueStructLogicAndroid)
                        {
                            IssueStructLogicAndroid loIssueStructLogic = (_struct._TIssStruct.StructLogicObj as IssueStructLogicAndroid);

                            loLegacyButtonPrintDefined = loIssueStructLogic.IssueStructActionButtons.Contains(AutoISSUE.DBConstants.BtnPrintName);
                            loLegacyButtonVoidDefined = loIssueStructLogic.IssueStructActionButtons.Contains(AutoISSUE.DBConstants.BtnVoidName);
                            loLegacyButtonReIssueDefined = loIssueStructLogic.IssueStructActionButtons.Contains(AutoISSUE.DBConstants.BtnReissueName);
                            loLegacyButtonIssueMoreDefined = loIssueStructLogic.IssueStructActionButtons.Contains(AutoISSUE.DBConstants.BtnIssueMoreName);

                        }
                    }
                }
            }




            // determine if this type of struct can be voided
            bool loVoidSupported = false;
            if (_struct != null)
            {
                if (_struct._TIssStruct != null)
                {
                    if (_struct._TIssStruct is TCiteStruct)
                    {
                        loVoidSupported = (((TCiteStruct)_struct._TIssStruct).VoidStruct != null);
                    }
                }
            }


            // determine if this type of struct can be re-issued
            bool loReIssueSupported = false;
            if (_struct != null)
            {
                if (_struct._TIssStruct != null)
                {
                    if (_struct._TIssStruct is TCiteStruct)
                    {
                        loReIssueSupported = (((TCiteStruct)_struct._TIssStruct).ReissueStruct != null);
                    }
                }
            }



            // now to decide - are there cricumstances to override?

            // where did we come from? just peek, don't want to consume it now
            string prevFragName = DroidContext.MyFragManager.PeekInternalBackstack();


            // is prevFrag empty?
            if (string.IsNullOrEmpty(prevFragName) == true)
            {
                // no backstack, that means we got here from the end of ticket issuance, and when done we want to back to the next new ticket of this type
                // no logic override = if voids are supported at end of ticket, so they are
            }
            else
            {
                // KLUDGY but effective. there is something in the backstack, this not the end of ticket issuance, no voiding. TODO - this needs registry overrides
                loVoidSupported = false;
            }





            // figure out how to handle the void/reinstate options
            VoidorReIssueEligibility loVoidOrReIssueMode = GetVoidOrReIssueEligibility(loVoidSupported, loReIssueSupported, ioCiteIsVoid, ioCiteIsReIssued);

            // reset each evaluation
            _RecordActionOptions.Clear();

            switch (loVoidOrReIssueMode)
            {
                case VoidorReIssueEligibility.vrVoidOnly:
                    {
                        _btnAction.Visibility = ViewStates.Visible;
                        _btnAction.Text = "VOID";
                        _RecordActionOptions.Add(Constants.cnStatusUpdateActionMenuTextVoid);
                        break;
                    }

                case VoidorReIssueEligibility.vrReIssueOnly:
                    {
                        _btnAction.Visibility = ViewStates.Visible;
                        _btnAction.Text = "REISSUE";
                        _RecordActionOptions.Add(Constants.cnStatusUpdateActionMenuTextReIssue);
                        break;
                    }

                case VoidorReIssueEligibility.vrVoidOrReIssue:
                    {
                        _btnAction.Visibility = ViewStates.Visible;
                        _btnAction.Text = "ACTION";
                        // multiple options will be displayed from the VOID button action
                        _RecordActionOptions.Add(Constants.cnStatusUpdateActionMenuTextVoid);
                        _RecordActionOptions.Add(Constants.cnStatusUpdateActionMenuTextReIssue);
                        break;
                    }

                default:
                    // incl  case VoidorReIssueEligibility.vrNotElgible:
                    {
                        _btnAction.Visibility = ViewStates.Gone;
                        break;
                    }
            }

            // 
            //_btnVoidOrReIssue.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_gray_light));
            //_btnVoidOrReIssue.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_black));

            // display issue more?
            if (loLegacyButtonIssueMoreDefined == true)
            {
                _RecordActionOptions.Add(Constants.cnStatusUpdateActionMenuTextIssueMore);
            }


            // ok, so now that we have action options, decide format
            if (_RecordActionOptions.Count == 0)
            {
                // no actions
                _btnAction.Visibility = ViewStates.Gone;
            }
            else if (_RecordActionOptions.Count == 1)
            {
                
                // TODO - get button names from config?


                // many actions, popup menu - always - even if one menu
                _btnAction.Text = "ACTION";
                _btnAction.Visibility = ViewStates.Visible;
/*

                // one action, no popup
                _btnAction.Visibility = ViewStates.Visible;
                switch (_RecordActionOptions[0])
                {
                    case Constants.cnStatusUpdateActionMenuTextVoid:
                        {
                            _btnAction.Text = "VOID";
                            break;
                        }
                    case Constants.cnStatusUpdateActionMenuTextReIssue:
                        {
                            _btnAction.Text = "REISSUE";
                            break;
                        }

                    case Constants.cnStatusUpdateActionMenuTextIssueMore:
                        {
                            _btnAction.Text = "MORE";
                            break;
                        }

                    //case Constants.cnStatusUpdateActionMenuTextIssueMultiple = "Issue Multiple";
                    //case Constants.cnStatusUpdateActionMenuTextCancel = "Cancel Citation";

                    default:
                        {
                            _btnAction.Text = "ACTION";
                            break;
                        }
                }
 * */
 

            }
            else
            {
                // many actions, popup menu
                _btnAction.Text = "ACTION";
                _btnAction.Visibility = ViewStates.Visible;
            }



            // reset each evaluation
            _NotesActionOptions.Clear();
            _NotesActionOptions.Add(Constants.cnNotesActionMenuTextAddNotation);
            _NotesActionOptions.Add(Constants.cnNotesActionMenuTextAddPhoto);
            _NotesActionOptions.Add(Constants.cnNotesActionMenuTextReviewNotes);






            // display print?
            if (loLegacyButtonPrintDefined == true)
            {
                _printBtn.Visibility = ViewStates.Visible;
            }
            else
            {
                _printBtn.Visibility = ViewStates.Gone;
            }








            // file extension is added by reader
            string loTicketImageName = Helper.GetTIssueFormBitmapImageFileNameOnly(_structName, _parkingDTO.sqlIssueNumberStr, DateTime.Today); // TODOD -needs datetime in proper formats


            //// for debug only, show alternate views
            //var sigImg2 = _view.FindViewById<ImageView>(Resource.Id.sigImg);
            //sigImg2 = Helper.GetTIssueFormBitmapImageFromStorage(sigImg2, loTicketImageName);



            reproductionImage = Helper.GetTIssueFormBitmapImageFromStorage(reproductionImage, loTicketImageName);


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
                _ticketscrollviewMain.SmoothScrollTo(0, 0);


                // put the keyboard away
                Context _context = this.Activity;
                if (_context != null)
                {
                    Helper.HideKeyboardFromFragment(_context);
                }
            });




            return _parkingDTO;
        }




        private enum VoidorReIssueEligibility
        {
            vrNotElgible = 0,
            vrVoidOnly,
            vrReIssueOnly,
            vrVoidOrReIssue
        };


        /// <summary>
        /// Walk through the available options from the layout and the current status
        /// </summary>
        /// <param name="iVoidSupported"></param>
        /// <param name="iReIssueSupported"></param>
        /// <param name="iCiteIsVoided"></param>
        /// <param name="iCiteIsReIssued"></param>
        /// 
        /// 
        ///   Window Mobile Legacy code reference - TODO - have to implement wireless enforcement pieces
        /// 
        ///     // void enabled if not void and in view mode or form has printed.
        //    if (fBtnVoid)
        //    {
        //        if ( fPreventManualFieldVoids == true )
        //        {
        //            // when registry item is set to prevent manually initiated field voids, 
        //            // the void button is always hidden so the user can't click it, but
        //            // it can be programatically invoked when needed, as for wireless meter enforcement
        //            fBtnVoid->SetVisible( 0 );  
        //        }
        //        else
        //        {
        //            fBtnVoid->SetVisible( 1 );
        //            // mcb 4.23.2008 - PAM - Allow reissue before print if ExternPreventEsc is true.
        //            if (fParentStruct->IsAClassMember(TCiteStructNameStr))
        //                fBtnVoid->SetEnabled( (!((TCiteStruct *)fParentStruct)->CiteIsVoid( fEditRecNo)) && (GetFormPrinted() || GetExternalPreventESC()) && !InMultiEntryAddMode() );
        //        }
        //    }

        //    // Reissue under same constraints as void. Not any more. No reissue when issuing multi.
        //    if (fBtnReissue)
        //    {
        //        if (fParentStruct->IsAClassMember(TCiteStructNameStr))
        //        {
        //        fBtnReissue->SetVisible( 1 );
        //        // mcb 4.23.2008 - PAM - Allow reissue before print if ExternPreventEsc is true.

        //        bool loReissueEnabled = 
        //            (!((TCiteStruct *)fParentStruct)->CiteIsReissued( fEditRecNo ) ) && 
        //        			
        //            (GetFormPrinted() || GetExternalPreventESC()) && 
        // 			
        //            !(fFormEditMode & femIssueMultipleAttr);
        //		
        //        // do we also have a requirement to prevent re-issue of MSM based records?
        //        if ((loReissueEnabled == true) && ( glReinoMeter->GetPreventReissueOfMSMBasedEnforcement() == true ))
        //        {
        //            // can only allow re-issue if the cite wasn't issued from wireless MSM data
        //            loReissueEnabled = (((TCiteStruct *)fParentStruct)->CiteIsMSMEnforcementBased( fEditRecNo )  == false);
        //        }
        //        		
        //        fBtnReissue->SetEnabled( loReissueEnabled );
        //    }
        ///
        /// <returns></returns>
        private VoidorReIssueEligibility GetVoidOrReIssueEligibility(bool iVoidSupported, bool iReIssueSupported, bool iCiteIsVoided, bool iCiteIsReIssued)
        {
            // is void supported?
            if (iVoidSupported == true)
            {
                if (iCiteIsVoided == true)
                {
                    // already voided... can it be re-issued?
                    if (iReIssueSupported == true)
                    {
                        // haven't done that already, right?
                        if (iCiteIsReIssued == false)
                        {
                            return VoidorReIssueEligibility.vrReIssueOnly;
                        }
                        else
                        {
                            // already re-issued
                            return VoidorReIssueEligibility.vrNotElgible;
                        }
                    }
                    else
                    {
                        // already voided, no more action allowed
                        return VoidorReIssueEligibility.vrNotElgible;
                    }
                }


                // its not voided yet
                if (iReIssueSupported == true)
                {
                    // they can do either
                    return VoidorReIssueEligibility.vrVoidOrReIssue;
                }
                else
                {
                    // void only
                    return VoidorReIssueEligibility.vrVoidOnly;
                }
            }


            // TODO - evaluate when void is not supported BUT re-issue is?

            return VoidorReIssueEligibility.vrNotElgible;
        }




        private void btnNotesActionClickEvaluation(object sender, EventArgs e)
        {

            // is there a coice to be made?
            if (_NotesActionOptions.Count < 2)
            {
                if (_NotesActionOptions.Count > 0)
                {
                    // no choice, just default action
                    PopUpSingleChoiceCallback(_NotesActionOptions[0]);
                }
            }
            else
            {
                try
                {
                    Android.App.FragmentManager loFm = this.FragmentManager;
                    PopUpSingleChoiceDialog loModeSelectionDlg = new PopUpSingleChoiceDialog(this.Activity, this, null, _NotesActionOptions);
                    loModeSelectionDlg.Show(loFm, "");
                }
                catch (Exception ex)
                {
                    LoggingManager.LogApplicationError(ex, "IssueReviewDetailFragment.btnNotesActionClickEvaluation", ex.TargetSite.Name);
                    System.Console.WriteLine("IssueReviewDetailFragment.btnNotesActionClickEvaluation Exception source {0}: {1}", ex.Source, ex.ToString());
                }
            }

        }



        private void btnActionClickEvaluation(object sender, EventArgs e)
        {
/*
 * 
 *  for consistency, always show sub menu
 * 
            // is there a coice to be made?
            if (_RecordActionOptions.Count < 2)
            {
                if (_RecordActionOptions.Count > 0)
                {
                    // no choice, just default action
                    PopUpSingleChoiceCallback(_RecordActionOptions[0]);
                }
            }
            else
 */
            {
                try
                {
                    Android.App.FragmentManager loFm = this.FragmentManager;
                    PopUpSingleChoiceDialog loModeSelectionDlg = new PopUpSingleChoiceDialog(this.Activity, this, null,_RecordActionOptions);
                    loModeSelectionDlg.Show(loFm, "");
                }
                catch (Exception ex)
                {
                    LoggingManager.LogApplicationError(ex, "IssueReviewDetailFragment.btnActionClickEvaluation", ex.TargetSite.Name);
                    System.Console.WriteLine("IssueReviewDetailFragment.btnActionClickEvaluation Exception source {0}: {1}", ex.Source, ex.ToString());
                }
            }

        }


        public void UserCommentPopUp_GetVoidReason()
        {

            string loDefaultVoidReasonTableName = Constants.PARKVOIDREASON;
            string loDefaultVoidReasonColumName = Constants.PARKVOIDREASON;

            // extract the void reasons from the parent struct if we can
            if (_struct._TIssStruct is TCiteStruct)
            {
                TCiteStruct loParentStruct = (TCiteStruct)_struct._TIssStruct;

                //If the Void Reason List is blank try populate it here
                if (string.IsNullOrEmpty(loParentStruct.VoidReasonList) == false)
                {
                    loDefaultVoidReasonTableName = loParentStruct.VoidReasonList;
                }
            }

            var listSupport = new ListSupport();
            string[] response = listSupport.GetListDataByTableColumnName(loDefaultVoidReasonTableName, loDefaultVoidReasonColumName);

            // debug - supplement list for demo
            if ((response == null) || (response.GetLength(0) == 0))
            {
                string[] loSampleResponse = new string[2] { "DRIVER PRODUCED VALID PERMIT", "OPERATOR ERROR" };
                response = loSampleResponse;
            }


            var ft = this.FragmentManager.BeginTransaction();

            // always new dialog created
            UserCommentPopUpDialogFragment oneUserCommentPopUpDialogFragment = new UserCommentPopUpDialogFragment { Arguments = new Bundle() };

            // TODO - get this text label from layout
            oneUserCommentPopUpDialogFragment.SetUserCommentDialogItems(Constants.cnUserCommentPopupVoidReason_TitleText, "Reason:", response);

            oneUserCommentPopUpDialogFragment.SetCallbackActivity(this, Constants.ISSUE_VOID_FRAGMENT_TAG_PREFIX);
            oneUserCommentPopUpDialogFragment.Show(this.FragmentManager, Constants.ISSUE_VOID_FRAGMENT_TAG_PREFIX);

            ft.Commit();

            // we need to see this dialog immediately
            this.FragmentManager.ExecutePendingTransactions();
        }

        public void UserCommentPopUp_GetReissueReason()
        {

            string loDefaultVoidReasonTableName = Constants.PARKVOIDREASON;
            string loDefaultVoidReasonColumName = Constants.PARKVOIDREASON;

            // extract the void reasons from the parent struct if we can
            if (_struct._TIssStruct is TCiteStruct)
            {
                TCiteStruct loParentStruct = (TCiteStruct)_struct._TIssStruct;

                //If the Void Reason List is blank try populate it here
                if (string.IsNullOrEmpty(loParentStruct.VoidReasonList) == false)
                {
                    loDefaultVoidReasonTableName = loParentStruct.VoidReasonList;
                }
            }

            var listSupport = new ListSupport();
            string[] response = listSupport.GetListDataByTableColumnName(loDefaultVoidReasonTableName, loDefaultVoidReasonColumName);

            // debug - supplement list for demo
            if ((response == null) || (response.GetLength(0) == 0))
            {
                string[] loSampleResponse = new string[2] { "DRIVER PRODUCED VALID PERMIT", "OPERATOR ERROR" };
                response = loSampleResponse;
            }


            var ft = this.FragmentManager.BeginTransaction();

            // always new dialog created
            UserCommentPopUpDialogFragment oneUserCommentPopUpDialogFragment = new UserCommentPopUpDialogFragment { Arguments = new Bundle() };

            // TODO - get this text label from layout
            oneUserCommentPopUpDialogFragment.SetUserCommentDialogItems(Constants.cnUserCommentPopupReissueReason_TitleText, "Reason:", response);

            oneUserCommentPopUpDialogFragment.SetCallbackActivity(this, Constants.ISSUE_REISSUE_FRAGMENT_TAG_PREFIX);
            oneUserCommentPopUpDialogFragment.Show(this.FragmentManager, Constants.ISSUE_REISSUE_FRAGMENT_TAG_PREFIX);

            ft.Commit();

            // we need to see this dialog immediately
            this.FragmentManager.ExecutePendingTransactions();
        }


        public void GotoNotesReviewSelectFragment()
        {
            // switch to the notes review list fragment
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            ISharedPreferencesEditor editor = prefs.Edit();

            // save where we came from
            editor.PutString(Constants.PREVIOUS_FRAGMENT, _tagName);

            editor.PutString(Constants.TICKETID, _ticketId);

            editor.Apply();

            DroidContext.MyFragManager.AddToInternalBackstack(_tagName);


            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            Fragment issueSelectFragment = FragmentManager.FindFragmentByTag(_tagName);
            if (issueSelectFragment != null)
            {
                // needed because this is a top level fragment that is managed by hide/view
                if (issueSelectFragment.View != null)
                {
                    issueSelectFragment.View.Visibility = ViewStates.Gone;
                }

                fragmentTransaction.Hide(issueSelectFragment);
            }




            // what data type did they select? this is the data type we need
            string loTargetFragmentTag = Helper.BuildNotesReviewSelectFragmentTag(_structName);

            var dtlFragment = (NotesReviewSelectFragment)FragmentManager.FindFragmentByTag(loTargetFragmentTag);
            if (dtlFragment != null)
            {
                fragmentTransaction.Show(dtlFragment);
                dtlFragment.RefreshDisplayedRecord();
            }
            else
            {
                var fragment = new NotesReviewSelectFragment { Arguments = new Bundle() };
                fragment.Arguments.PutString("structName", _structName);

                //MainActivity.RegisterFragment(fragment, loTag, _structName, "NoMenu " + loTag, FragmentClassificationType.fctSecondaryActivity, -1, -1);
                fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, loTargetFragmentTag);

                //fragmentTransaction.Replace(Resource.Id.frameLayout1, new IssueReviewDetailFragment(), Constants.ISSUE_REVIEW_DETAIL_FRAGMENT_TAG);

                fragmentTransaction.Show(fragment);

                fragment.RefreshDisplayedRecord(); // ok here?

            }

            fragmentTransaction.Commit();
        }
    

        

        public void UserCommentPopUp_GetNotationText()
        {

            //string loDefaultNotationTextTableName = Constants.PARKVOIDREASON;
            //string loDefaultNotationTextColumName = Constants.PARKVOIDREASON;

            //// extract the void reasons from the parent struct if we can
            //if (_struct._TIssStruct is TCiteStruct)
            //{
            //    TCiteStruct loParentStruct = (TCiteStruct)_struct._TIssStruct;

            //    //If the Void Reason List is blank try populate it here
            //    if (string.IsNullOrEmpty(loParentStruct.VoidReasonList) == false)
            //    {
            //        loDefaultNotationTextTableName = loParentStruct.VoidReasonList;
            //    }
            //}

            //var listSupport = new ListSupport();
            //string[] response = listSupport.GetListDataByTableColumnName(loDefaultNotationTextTableName, loDefaultNotationTextColumName);

            //// debug - supplement list for demo
            //if ((response == null) || (response.GetLength(0) == 0))
            //{
            //    string[] loSampleResponse = new string[2] { "DRIVER PRODUCED VALID PERMIT", "OPERATOR ERROR" };
            //    response = loSampleResponse;
            //}

            string[] response = null;


            var ft = this.FragmentManager.BeginTransaction();

            // always new dialog created
            UserCommentPopUpDialogFragment oneUserCommentPopUpDialogFragment = new UserCommentPopUpDialogFragment { Arguments = new Bundle() };

            // TODO - get this text label from layout
            oneUserCommentPopUpDialogFragment.SetUserCommentDialogItems(Constants.cnUserCommentPopupNotationText_TitleText, "Notes:", response);

            oneUserCommentPopUpDialogFragment.SetCallbackActivity(this, Constants.ISSUE_NOTES_FRAGMENT_TAG_PREFIX);
            oneUserCommentPopUpDialogFragment.Show(this.FragmentManager, Constants.ISSUE_NOTES_FRAGMENT_TAG_PREFIX);

            ft.Commit();

            // we need to see this dialog immediately
            this.FragmentManager.ExecutePendingTransactions();
        }



        public void UserCommentPopUp_UserCommentCallback(string iTag, string iUserCommentText)
        {
            string _Voidreason = iUserCommentText;

            switch (iTag)
            {

                case Constants.ISSUE_VOID_FRAGMENT_TAG_PREFIX:
                    {
                        VoidRecordPrim(iUserCommentText);
                        break;
                    }

                case Constants.ISSUE_REISSUE_FRAGMENT_TAG_PREFIX:
                    {
                        ReissueRecordPrim(iUserCommentText);
                        break;
                    }

                case Constants.ISSUE_NOTES_FRAGMENT_TAG_PREFIX:
                    {
                        AddNotePrim(iUserCommentText);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }


        }


        //
        //
        //

        private void btnVoidClick(object sender, EventArgs e)
        {
            UserCommentPopUp_GetVoidReason();
        }

        private void btnReIssueClick(object sender, EventArgs e)
        {
            UserCommentPopUp_GetReissueReason();
        }

        private void btnAddNotationClick(object sender, EventArgs e)
        {
            UserCommentPopUp_GetNotationText();
        }


        private void btnAddPhotoClick(object sender, EventArgs e)
        {
            ActivateCamera(MediaStore.ActionImageCapture, Constants.PHOTO_FILE_SUFFIX);
            //ActivateCamera(MediaStore.ActionVideoCapture, Constants.VIDEO_FILE_SUFFIX);
        }

        //
        //
        //


        private async void AddNotePrim(string iUserCommentText)
        {


            try
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);

                string officerId = prefs.GetString(Constants.OFFICER_ID, null);
                string officerName = prefs.GetString(Constants.OFFICER_NAME, null);


                string sequenceId = _parkingDTO.sqlIssueNumberStr;
                string masterKey = _parkingDTO.DBRowId;


                ParkNoteDTO newParkNoteDTO = new ParkNoteDTO();


                newParkNoteDTO.SeqId = sequenceId;
                newParkNoteDTO.MasterKey = masterKey;
                newParkNoteDTO.NotesMemo = iUserCommentText;
                newParkNoteDTO.DBRowId = "";  // reset for new note

                newParkNoteDTO.NoteDate = DateTimeOffset.Now.ToString(Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK);
                newParkNoteDTO.NoteTime = DateTimeOffset.Now.ToString(Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK);
                newParkNoteDTO.OfficerId = officerId;
                newParkNoteDTO.OfficerName = officerName;

                if (_photoFile != null)
                {
                    newParkNoteDTO.MultiMediaNoteFileName = _photoFile.Name;
                    //_parkNoteDTO.MultiMediaNoteDataType = "2"; // TODO : a number code for  photo, video etc.
                    newParkNoteDTO.MultiMediaNoteDataType = AutoISSUE.DBConstants.TMultimediaType.mmPicture.ToString();


                    // this file LastModified will return milliseconds since 1/1/1970
                    var dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(_photoFile.LastModified());
                    newParkNoteDTO.MultiMediaNoteDateStamp = dt.ToString(Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK);
                    newParkNoteDTO.MultiMediaNoteTimeStamp = dt.ToString(Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK);
                }

                var parkingSequenceADO = new ParkingSequenceADO();


                try
                {
                    // if it is a new note, then insert
                    if (string.IsNullOrEmpty(newParkNoteDTO.DBRowId) == true)
                    {
                        await parkingSequenceADO.InsertRowParkNote(newParkNoteDTO);
                    }
                    else
                    {
                        await parkingSequenceADO.UpdateParkNote(newParkNoteDTO);
                    }

                }
                catch (Exception exp)
                {
                    if (string.IsNullOrEmpty(newParkNoteDTO.DBRowId) == true)
                    {
                        Log.Debug("ERROR Exception in AutoAttachMultimediaFiles InsertRowParkNote: ", exp.Message);
                        System.Console.WriteLine("ERROR Exception in AutoAttachMultimediaFiles InsertRowParkNote: ", exp.Message);
                    }
                    else
                    {
                        Log.Debug("ERROR Exception in AutoAttachMultimediaFiles UpdateParkNote: ", exp.Message);
                        System.Console.WriteLine("ERROR Exception in AutoAttachMultimediaFiles UpdateParkNote: ", exp.Message);
                    }
                }


                Toast.MakeText(Activity, "Note Saved", ToastLength.Long).Show();
        
                // any kind of record update, we'll look to upload it ASAP
                Activity.StartService(new Intent(Activity, typeof(SyncService)));

                RefreshDisplayedRecord();
        
        
            }
            catch (Exception exp)
            {
                Log.Debug("ERROR Exception in AutoAttachMultimediaFiles AddParkNote: ", exp.Message);
                System.Console.WriteLine("ERROR Exception in AutoAttachMultimediaFiles AddParkNote: ", exp.Message);
            }
            finally
            {
                // we're done, clear this out 
                _photoFile = null;
            }


        }


        /// <summary>
        /// Do the actual voiding of the selected record
        /// </summary>
        private void VoidRecordPrim(string iUserCommentText)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);

            string officerId = prefs.GetString(Constants.OFFICER_ID, null);
            string officerName = prefs.GetString(Constants.OFFICER_NAME, null);


            string sequenceId = _parkingDTO.sqlIssueNumberStr;
            string masterKey = _parkingDTO.DBRowId;

            string voidStructName = null;


            var parkingSequenceADO = new ParkingSequenceADO();

            // get the void struct reference
            if (_struct._TIssStruct is TCiteStruct)
            {
                if (((TCiteStruct)_struct._TIssStruct).VoidStruct != null)
                {
                    voidStructName = ((TCiteStruct)_struct._TIssStruct).VoidStruct.Name;
                }
            }



            if (string.IsNullOrEmpty(voidStructName) == false)
            {

                parkingSequenceADO.InsertRowParkingVoid(masterKey, sequenceId, officerId, officerName, iUserCommentText, Activity, voidStructName);
                Toast.MakeText(Activity, "Record Voided", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(Activity, "Void Failed. Not Supported for Record Type", ToastLength.Long).Show();
            }


            RefreshDisplayedRecord();
        }



       void SetIssueSourceRowAndFormEditMode(Android.App.Fragment iCommonFragment, DataRow iSourceRow, int iFormEditMode  )
        {
            if (iCommonFragment != null)
            {
                if (iCommonFragment is CommonFragment)
                {
                    ((CommonFragment)iCommonFragment).fSourceDataRawRow = iSourceRow;
                    ((CommonFragment)iCommonFragment).fSourceDataRowFormEditMode = iFormEditMode;
                }
            }

        }

        /// <summary>
        /// Do the actual re-issue of the selected record
        /// </summary>
        private void ReissueRecordPrim(string iUserCommentText)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);

            string officerId = prefs.GetString(Constants.OFFICER_ID, null);
            string officerName = prefs.GetString(Constants.OFFICER_NAME, null);


            string sequenceId = _parkingDTO.sqlIssueNumberStr;
            string masterKey = _parkingDTO.DBRowId;


            string voidStructName = null;
            string reIssueStructName = null;


            var parkingSequenceADO = new ParkingSequenceADO();

            // get the reissue struct reference
            if (_struct._TIssStruct is TCiteStruct)
            {
                if (((TCiteStruct)_struct._TIssStruct).ReissueStruct != null)
                {
                    reIssueStructName = ((TCiteStruct)_struct._TIssStruct).ReissueStruct.Name;
                }
            }

            // get the void struct reference
            if (_struct._TIssStruct is TCiteStruct)
            {
                if (((TCiteStruct)_struct._TIssStruct).VoidStruct != null)
                {
                    voidStructName = ((TCiteStruct)_struct._TIssStruct).VoidStruct.Name;
                }
            }


            // void it first. assume the worst
            string loVoidResultText = "";
            if (string.IsNullOrEmpty(voidStructName) == false)
            {

                parkingSequenceADO.InsertRowParkingVoid(masterKey, sequenceId, officerId, officerName, iUserCommentText, Activity, voidStructName);
            }
            else
            {
                loVoidResultText = "Void Failed. Not Supported for Record Type. Record Reissued.";
            }



            // now reissue
            parkingSequenceADO.InsertRowParkingReIssue(masterKey,
                                                        _parkingDTO.sqlIssueNumberPrefixStr, _parkingDTO.sqlIssueNumberStr, _parkingDTO.sqlIssueNumberSuffixStr,
                                                        officerId, officerName, Activity, reIssueStructName);


            // an error status to show?
            if (string.IsNullOrEmpty(loVoidResultText) == false)
            {
                Toast.MakeText(Activity, loVoidResultText, ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(Activity, "Record Voided for Reissue", ToastLength.Long).Show();
            }




            // Get the citation structure by name
            TIssStruct CiteStruct = _struct._TIssStruct;
            if (CiteStruct == null)
            {
                return;// false; // can't issue a ticket if we don't have a ticket struct 
            }


            // Now make sure we have an issuance form and form logic to work with
            IssueStructLogicAndroid CiteLogic = CiteStruct.StructLogicObj as IssueStructLogicAndroid;
            if (CiteLogic.IssueFormLogic == null)  // || (CiteLogic.IssueFormLogic.CfgForm == null))
            {
                return;// false;
            }


            // get the target fragment tag
            string loTargetFragmentTag = CiteLogic.IssueForm1LogicFragmentTag;
            //string loTargetFragmentTag = Helper.BuildIssueNewFragmentTag(CiteStruct.Name);


            CommonADO commonADO = new CommonADO();
            DataRow loSourceIssueRow = commonADO.GetRawDataRowByRowId(masterKey, _structName);

            SetIssueSourceRowAndFormEditMode(CiteLogic.IssueFormLogic, loSourceIssueRow, Reino.ClientConfig.EditRestrictionConsts.femReissue);
            //SetIssueSource(CiteLogic.IssueFormLogic, iSearchResult );
                        

            CiteLogic.IssueRecord(Reino.ClientConfig.EditRestrictionConsts.femReissue, _struct._TIssForm.IssueMoreFirstFocus, "", loTargetFragmentTag );


            // this is now done in formpanel when they complete the record
            //UndoIssueSource(CiteLogic.IssueFormLogic);

            //// release the mutex
            //SearchStructLogicAndroid.unSearchEvaluateInProcess = false; //  true; // DEBUG -- This is true in C++ source?

        }





#if _old_
        private void btnReIssueClick(object sender, EventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);

            string officerId = prefs.GetString(Constants.OFFICER_ID, null);
            string officerName = prefs.GetString(Constants.OFFICER_NAME, null);
            string structName = prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);

            string reIssueStructName = null;
            var parkingSequenceADO = new ParkingSequenceADO();
            List<XMLConfig.IssStruct> structs = DroidContext.XmlCfg.IssStructs;
            foreach (var structI in structs)
            {
                if (structI.ParentStruct != null)
                {
                    if (structI.ParentStruct.Equals(structName) && Constants.STRUCT_TYPE_REISSUE.Equals(structI.Type))
                    {
                        reIssueStructName = structI.Name;
                    }
                }
            }
            parkingSequenceADO.InsertRowParkingReIssue(_parkingDTO.sqlIssueNumberStr, _parkingDTO.sqlIssueNumberPrefixStr, officerId, officerName, Activity, reIssueStructName);

            Toast.MakeText(Activity, "Ticket ReIssued", ToastLength.Long).Show();

            IssueAnotherCommon();
        }
#endif

        public void PopUpSingleChoiceCallback(string iSelectedItemText)
        {
            switch (iSelectedItemText)
            {
                case Constants.cnStatusUpdateActionMenuTextVoid:
                    {
                        btnVoidClick(null, null);
                        break;
                    }
                case Constants.cnStatusUpdateActionMenuTextReIssue:
                    {
                        btnReIssueClick(null, null);
                        break;
                    }
                case Constants.cnStatusUpdateActionMenuTextIssueMore:
                    {
                        btnIssueMoreClick(null, null);
                        break;
                    }
                case Constants.cnNotesActionMenuTextAddNotation:
                    {
                        btnAddNotationClick(null, null);
                        break;
                    }
                case Constants.cnNotesActionMenuTextAddPhoto:
                    {
                        btnAddPhotoClick(null, null);
                        break;
                    }
                case Constants.cnNotesActionMenuTextReviewNotes:
                    {
                        GotoNotesReviewSelectFragment();
                        break;
                    }

                //case Constants.cnNotesActionMenuTextAddAudioNote:
                //case Constants.cnNotesActionMenuTextAddVideo:
                // etc


                default:
                    {
                        break;
                    }
            }

        }


        private DateTime GetIssueDateWithMask()
        {
            DateTime loFieldDate = DateTime.Today;
            var editMask = Constants.DT_MM_DD_YYYY;
            var issueDateField = DroidContext.XmlCfg.BehaviorCollection.FirstOrDefault(
                    x => x.PanelField.Name == Constants.ISSUEDATE_COLUMN);
            if (issueDateField != null)
                editMask = issueDateField.PanelField.EditMask;
            DateTimeManager.DateStringToOSDate(editMask, _parkingDTO.sqlIssueDateStr, ref loFieldDate);
            return loFieldDate;
        }

        private void BtnPrintClick(object sender, EventArgs e)
        {
            _progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Printing Ticket...", true);
            ThreadPool.QueueUserWorkItem(o => SendPrint(_parkingDTO.sqlIssueNumberStr, GetIssueDateWithMask(), false));
        }


        //private void BtnLocalPrintClick(object sender, EventArgs e)
        //{
        //    _progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Printing Ticket...", true);
        //    ThreadPool.QueueUserWorkItem(o => SendPrint(_parkingDTO.sqlIssueNumberStr, GetIssueDateWithMask(), true));
        //}





        public void SendPrint(String issueNum, DateTime issueDate, bool localFlag)
        {
            string toastMsg = "";
            try
            {
                string structName = "PARKING";

                string loTicketImageFileName = Helper.GetTIssueFormBitmapImageFileNameOnly(structName, issueNum, issueDate);

                string loTicketPCLTextFileName = Helper.GetTIssueFormPCLPrintJobFileNameOnly(structName, issueNum, issueDate);

                PrinterSupport_BaseClass.ReprintTicketToCurrentlySelectedPrinter(loTicketImageFileName, loTicketPCLTextFileName, ref toastMsg);


                //// AJW TODO - this is kludgy, need a better filter
                //if ( toastMsg.ToUpper().Contains("ERROR") == true )
                //{
                //    throw new Exception( toastMsg );
                //}

            }
            catch (Exception e)
            {
                toastMsg = "Error printing ticket";
            }



            // clean up the progress dialog
            try
            {
                Activity.RunOnUiThread(() => _progressDialog.Hide());
                Activity.RunOnUiThread(() => Toast.MakeText(this.Activity, toastMsg, ToastLength.Long).Show());
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Exception source: {0}", e.Source);
            }

        }



#if _old_
        /// <summary>
        /// when they touch a note entry, open the note detail fragment
        /// </summary>
        void LoadNoteDetails()
        {
            // save where we came from
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity);
            ISharedPreferencesEditor editor = prefs.Edit();

            //editor.PutString(Constants.PREVIOUS_FRAGMENT, Constants.ISSUE_REVIEW_DETAIL_FRAGMENT_TAG);
            editor.PutString(Constants.PREVIOUS_FRAGMENT, _tagName);


            // come back when you're done there
            DroidContext.MyFragManager.AddToInternalBackstack(_tagName);


            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();

            // hide ourselves
            Fragment notesFragment = FragmentManager.FindFragmentByTag(_tagName);
            //Fragment notesFragment = FragmentManager.FindFragmentByTag(Constants.NOTES_FRAGMENT_TAG);
            if (notesFragment != null)
            {
                fragmentTransaction.Hide(notesFragment);
            }


            // this is the data type we need
            string loTargetFragmentTag = Helper.BuildIssueNoteDetailFragmentTag(_structName);

            var noteDetail = (NoteDetailFragment)FragmentManager.FindFragmentByTag(loTargetFragmentTag);
            //var noteDetail = (NoteDetailFragment)FragmentManager.FindFragmentByTag(Constants.NOTE_DETAIL_FRAGMENT_TAG);

            if (noteDetail != null)
            {
                fragmentTransaction.Show(noteDetail);
                noteDetail.CreateLayout();
            }
            else
            {
                //fragmentTransaction.Replace(Resource.Id.frameLayout1, new NoteDetailFragment(), Constants.NOTE_DETAIL_FRAGMENT_TAG);

                var fragment = new NoteDetailFragment { Arguments = new Bundle() };
                fragment.Arguments.PutString("structName", _structName);

                //MainActivity.RegisterFragment(fragment, loTag, _structName, "NoMenu " + loTag, FragmentClassificationType.fctSecondaryActivity, -1, -1);
                fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, loTargetFragmentTag);

                fragment.CreateLayout();  // needed here?

            }

            fragmentTransaction.Commit();
        }
#endif


#if _old_
        private void btnAddNotesClick(object sender, EventArgs e)
        {
        }

        private void BtnNotesClick(object sender, EventArgs e)
        {
            // save where we came from
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity);
            ISharedPreferencesEditor editor = prefs.Edit();

            editor.PutString(Constants.PREVIOUS_FRAGMENT, _tagName);
            //editor.PutString(Constants.PREVIOUS_FRAGMENT, Constants.ISSUE_REVIEW_DETAIL_FRAGMENT_TAG);

            // and what ticket we're looking at
            editor.PutString(Constants.ID_COLUMN, _parkingDTO.DBRowId);
            editor.PutString(Constants.ISSUENO_COLUMN, _parkingDTO.sqlIssueNumberStr); // AJW - TODO needs to go away from issueno to db generated keys



            editor.Apply();

            // come back when you're done there
            DroidContext.MyFragManager.AddToInternalBackstack(_tagName);

            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();

            // hide ourselves
            Fragment detailFragment = FragmentManager.FindFragmentByTag(_tagName);
            if (detailFragment != null)
            {
                fragmentTransaction.Hide(detailFragment);
            }


            // this is the data type we need
            string loTargetFragmentTag = Helper.BuildIssueNotesFragmentTag(_structName);

            var notesFragment = (NotesFragment)FragmentManager.FindFragmentByTag(loTargetFragmentTag);
            //var notesFragment = (NotesFragment)FragmentManager.FindFragmentByTag(Constants.NOTES_FRAGMENT_TAG);
            if (notesFragment != null)
            {
                fragmentTransaction.Show(notesFragment);
                notesFragment.GetNotesByTicket();
            }
            else
            {

                //fragmentTransaction.Replace(Resource.Id.frameLayout1, new NotesFragment(), Constants.NOTES_FRAGMENT_TAG);

                var fragment = new NotesFragment { Arguments = new Bundle() };
                fragment.Arguments.PutString("structName", _structName);

                //MainActivity.RegisterFragment(fragment, loTag, _structName, "NoMenu " + loTag, FragmentClassificationType.fctSecondaryActivity, -1, -1);
                fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, loTargetFragmentTag);

                //fragment.GetNotesByTicket(); cant here
            }


            //// add this to the backstack so we can come back here when done
            //// TODO - need to account for skipping around with drawer menus
            //fragmentTransaction.AddToBackStack(null);

            fragmentTransaction.Commit();
        }
#endif





        private void BtnDoneClick(object sender, EventArgs e)
        {
            // kludgy but effective
            // see if its time to reboot and reclaim leaked resrouces (egads!)
            //
            // this is a good spot to do this because it at the end of the citation process.
            //
            // putting here on DONE only checks when they are done, and not when they want to re-issue or take other action
            //
            if (MemoryWatcher.RestartRequestFlagIsRaised() == true)
            {
                // restart and reclaim resrouce
                MemoryWatcher.RestartApp(Constants.appsettings_bootmode_value_resync, DroidContext.mainActivity, DroidContext.ApplicationContext, 2);
            }






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

            // assume not
            bool loReturnToTopIssueMenu = false;


            // is prevFrag empty?
            if (string.IsNullOrEmpty(prevFragName) == true)
            {
                // no backstack, that means we got here from the end of ticket issuance, and when done we want to back to the next new ticket of this type
                loReturnToTopIssueMenu = true;
            }


            // find and hide ourselves
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();

            Fragment reviewDetailFragment = FragmentManager.FindFragmentByTag(_tagName);
            //Fragment reviewDetailFragment = FragmentManager.FindFragmentByTag(Constants.ISSUE_REVIEW_DETAIL_FRAGMENT_TAG);

            if (reviewDetailFragment != null)
            {
                fragmentTransaction.Hide(reviewDetailFragment);
            }


            // are we going back to one of the the review/list pages?
            if (prevFragName.StartsWith(Constants.ISSUE_SELECT_FRAGMENT_TAG_PREFIX) == true)
            {

                // back to the selection screen
                IssueSelectFragment issSelectFragment = (IssueSelectFragment)FragmentManager.FindFragmentByTag(prevFragName);
                if (issSelectFragment != null)
                {
                    fragmentTransaction.Show(issSelectFragment);
                    issSelectFragment.DisplayMatchingRecordsForDefaultStruct();

                    //RunOnUiThread(() =>
                    // {
                    //     // show me the records created so far
                    //     ((IssueSelectFragment)fragment).DisplayMatchingRecordsForDefaultStruct();
                    // });

                }
                else
                {
                    // lost it, have to rebuild??
                    Android.App.Fragment fragment = new IssueSelectFragment { Arguments = new Bundle() };
                    fragment.Arguments.PutString("structName", structName);

                    // this is the data type we need
                    string loTargetFragmentTag = Helper.BuildIssueSelectFragmentTag(_structName);

                    //fragmentTransaction.Replace(Resource.Id.frameLayout1, fragment, prevFragName);
                    //fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, prevFragName);
                    fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, loTargetFragmentTag);
                }

            }
            else
            {
                // for now, only other place to go
                loReturnToTopIssueMenu = true;
            }

            // commit the pending transactions
            fragmentTransaction.Commit();


            // TODO - this needs to go to any issue structure


            // do we need to go back through th top?
            if (loReturnToTopIssueMenu == true)
            {
                DroidContext.mainActivity.RunOnUiThread(() =>
                {
                    // TODO - genericize this to any form type
                    //now send the user to the parking form
                    const string formNameToUse = "PARKING";

                    DroidContext.ResetControlStatusByStructName(formNameToUse);

                    //CommonFragment dtlFragment = (CommonFragment)((MainActivity)_context).FindRegisteredFragment(formNameToUse);

                    // here comes the new ticket
                    //dtlFragment.StartTicketLayout();

                    // set the selected navigation back to the parking form. we're on the UI thread for it to work.
                    // is it bad form to cast the context like this?
                    DroidContext.mainActivity.ChangeToParkingFragment();

                });

            }

        }



        //Create duplicate issue
        public async void btnIssueMoreClick(object sender, EventArgs e)
        {
            //IssueAnotherCommon();
            IssueMorePrim("");
        }


#if _old_reference_
        public async void IssueAnotherCommon()
        {
            try
            {
                /* TODO - fix

                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity);
                //ISharedPreferencesEditor editor = prefs.Edit();
                string structName = prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);
                //TODO - FIX THIS
                string tabPosString = prefs.GetString(structName + Constants.LABEL_TAB_POSITION, null);
                int tabPosition = 0;
                if (tabPosString != null)
                {
                    int.TryParse(tabPosString, out tabPosition);
                }

                var commonADO = new CommonADO();
                Task<bool> result = commonADO.CreateDuplicateRowInfoById(_parkingDTO.sqlIssueNumberStr, structName);
                bool response = await result;
                FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
                //			Fragment detailFragment = FragmentManager.FindFragmentByTag(Constants.ISSUE_REVIEW_DETAIL_FRAGMENT_TAG);
                //			if (detailFragment != null){
                //				fragmentTransaction.Hide(detailFragment);
                //			}
                //
                var issFrag = (CommonFragment)FragmentManager.FindFragmentByTag(structName);
                if (issFrag != null)
                    issFrag.StartTicketLayout();
                fragmentTransaction.Commit();
                this.Activity.ActionBar.SetSelectedNavigationItem(tabPosition);
                 * 
                 */
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, null, "BtnIssueAnotherClick");
            }

        }
#endif


        public async void IssueAnotherCommon()
        {
            try
            {
                //var commonADO = new CommonADO();
                //Task<bool> result = commonADO.CreateDuplicateRowInfoById(_parkingDTO.sqlIssueNumberStr, _structName);
                //bool response = await result;


                // get the target fragment tag
                string loTargetFragmentTag = Helper.BuildIssueNewFragmentTag(_structName);


                FragmentRegistation loTargetFragment;
                loTargetFragment = DroidContext.mainActivity.FindFragmentRegistration(loTargetFragmentTag);

                if (loTargetFragment == null)
                {
                    //AppMessageBox.ShowMessageWithBell("Missing form named " + iIssueFormName, "", "");
                    return;
                }


                // we are leaving, don't come back here
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutString(Constants.PREVIOUS_FRAGMENT, string.Empty);
                editor.Apply();


                // move back in the direction we came from 
                DroidContext.MyFragManager.ClearInternalBackstack();

                // find and hide ourselves
                FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();

                Fragment reviewDetailFragment = FragmentManager.FindFragmentByTag(_tagName);
                if (reviewDetailFragment != null)
                {
                    fragmentTransaction.Hide(reviewDetailFragment);
                }

                // commit the pending transactions
                fragmentTransaction.Commit();


                DroidContext.mainActivity.ShowFragmentForIssueanceFromAnotherIssueForm(loTargetFragment);
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, null, "BtnIssueAnotherClick");
            }

        }
        /// <summary>
        /// Do the actual issue more of the selected record
        /// </summary>
        private void IssueMorePrim(string iUserCommentText)
        {

#if _mode_
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);

            string officerId = prefs.GetString(Constants.OFFICER_ID, null);
            string officerName = prefs.GetString(Constants.OFFICER_NAME, null);


            string sequenceId = _parkingDTO.sqlIssueNumberStr;
            string masterKey = _parkingDTO.DBRowId;


            string voidStructName = null;
            string reIssueStructName = null;


            var parkingSequenceADO = new ParkingSequenceADO();

            // get the reissue struct reference
            if (_struct._TIssStruct is TCiteStruct)
            {
                if (((TCiteStruct)_struct._TIssStruct).ReissueStruct != null)
                {
                    reIssueStructName = ((TCiteStruct)_struct._TIssStruct).ReissueStruct.Name;
                }
            }

            // get the void struct reference
            if (_struct._TIssStruct is TCiteStruct)
            {
                if (((TCiteStruct)_struct._TIssStruct).VoidStruct != null)
                {
                    voidStructName = ((TCiteStruct)_struct._TIssStruct).VoidStruct.Name;
                }
            }


            // void it first. assume the worst
            string loVoidResultText = "";
            if (string.IsNullOrEmpty(voidStructName) == false)
            {

                parkingSequenceADO.InsertRowParkingVoid(masterKey, sequenceId, officerId, officerName, iUserCommentText, Activity, voidStructName);
            }
            else
            {
                loVoidResultText = "Void Failed. Not Supported for Record Type. Record Reissued.";
            }



            // now reissue
            parkingSequenceADO.InsertRowParkingReIssue(masterKey,
                                                        _parkingDTO.sqlIssueNumberPrefixStr, _parkingDTO.sqlIssueNumberStr, _parkingDTO.sqlIssueNumberSuffixStr,
                                                        officerId, officerName, Activity, reIssueStructName);


            // an error status to show?
            if (string.IsNullOrEmpty(loVoidResultText) == false)
            {
                Toast.MakeText(Activity, loVoidResultText, ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(Activity, "Record Voided for Reissue", ToastLength.Long).Show();
            }


#endif

            string sequenceId = _parkingDTO.sqlIssueNumberStr;
            string masterKey = _parkingDTO.DBRowId;


            // Get the citation structure by name
            TIssStruct CiteStruct = _struct._TIssStruct;
            if (CiteStruct == null)
            {
                return;// false; // can't issue a ticket if we don't have a ticket struct 
            }


            // Now make sure we have an issuance form and form logic to work with
            IssueStructLogicAndroid CiteLogic = CiteStruct.StructLogicObj as IssueStructLogicAndroid;
            if (CiteLogic.IssueFormLogic == null)  // || (CiteLogic.IssueFormLogic.CfgForm == null))
            {
                return;// false;
            }


            // get the target fragment tag
            string loTargetFragmentTag = CiteLogic.IssueForm1LogicFragmentTag;
            //string loTargetFragmentTag = Helper.BuildIssueNewFragmentTag(CiteStruct.Name);




            //// Rather than exit, we will enter a new record. 
            //fFormEditMode |= Reino.ClientConfig.EditRestrictionConsts.femIssueMoreAttr;
            //// Mask out all but SingleEntry and IssueMore 
            //fFormEditMode &= (Reino.ClientConfig.EditRestrictionConsts.femIssueMoreAttr | Reino.ClientConfig.EditRestrictionConsts.femSingleRecordAttr);
            //PrepareForEdit();


            // Rather than exit, we will enter a new record. 
            int loFormEditMode = Reino.ClientConfig.EditRestrictionConsts.femIssueMoreAttr;
            // get ready to enter a new record - mask out all but SingleEntry and IssueMore
            loFormEditMode &= (Reino.ClientConfig.EditRestrictionConsts.femIssueMoreAttr | Reino.ClientConfig.EditRestrictionConsts.femSingleRecordAttr);


            CommonADO commonADO = new CommonADO();
            DataRow loSourceIssueRow = commonADO.GetRawDataRowByRowId(masterKey, _structName);


            SetIssueSourceRowAndFormEditMode(CiteLogic.IssueFormLogic, loSourceIssueRow, Reino.ClientConfig.EditRestrictionConsts.femIssueMore);
            //SetIssueSourceRowAndFormEditMode(CiteLogic.IssueFormLogic, loSourceIssueRow, Reino.ClientConfig.EditRestrictionConsts.femIssueMore);
            //SetIssueSource(CiteLogic.IssueFormLogic, iSearchResult );




            CiteLogic.IssueRecord(loFormEditMode, _struct._TIssForm.IssueMoreFirstFocus, "", loTargetFragmentTag);


            // this is now done in formpanel when they complete the record
            //UndoIssueSource(CiteLogic.IssueFormLogic);

            //// release the mutex
            //SearchStructLogicAndroid.unSearchEvaluateInProcess = false; //  true; // DEBUG -- This is true in C++ source?

        }



        private void ActivateCamera(String action, String fileSuffix)
        {
            try
            {
                //String fileNamePrefix = _prefs.GetString(Constants.DEVICEID, null) + "_";
                String fileNamePrefix = Helper.GetDeviceUniqueSerialNumber() + "_";

                Java.IO.File loPhotoDir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), Constants.MULTIMEDIA_FOLDERNAME);

                var intent = new Intent(action);
                _photoFile = new Java.IO.File(loPhotoDir, fileNamePrefix + DateTimeOffset.Now.ToString("yyyy_MM_dd_HH_mm_ss") + fileSuffix);
                intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_photoFile));
                StartActivityForResult(intent, 0);
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, null, "ActivateCamera");
            }
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            try
            {
                if (resultCode == Result.Ok)
                {
                    // make it available in the gallery
                    var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                    Android.Net.Uri contentUri = Android.Net.Uri.FromFile(_photoFile);

                    mediaScanIntent.SetData(contentUri);
                    //            SendBroadcast(mediaScanIntent);

                    //ShowImage();

                    AddNotePrim("MULTIMEDIA ATTACHED");
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, null, "IssueReviewDetailFragment.OnActivityResult");
            }
        }


        private void ShowImage()
        {
            //Bitmap bitmap = null;
            //if (_photoFile.AbsolutePath.Contains(Constants.VIDEO_FILE_SUFFIX))
            //    bitmap = Android.Media.ThumbnailUtils.CreateVideoThumbnail(_photoFile.AbsolutePath, ThumbnailKind.MiniKind);
            //else if (_photoFile.AbsolutePath.Contains(Constants.PHOTO_FILE_SUFFIX))
            //{
            //    // resize the bitmap to fit the display, Loading the full sized image will consume too much memory 
            //    int height = _imageView.Height;
            //    int width = Resources.DisplayMetrics.WidthPixels;
            //    bitmap = _photoFile.Path.LoadAndResizeBitmap(width, height);
            //}

            //_imageView.SetImageBitmap(bitmap);
        }
    }
}



