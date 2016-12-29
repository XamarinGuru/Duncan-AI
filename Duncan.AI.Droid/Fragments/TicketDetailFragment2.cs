using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
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
using Duncan.AI.Droid.Utils.HelperManagers;
using Duncan.AI.Droid.Utils.PrinterSupport;


namespace Duncan.AI.Droid
{
    public class TicketDetailFragment : Fragment
    {
        View _view;



        List<ParkNoteDTO> _parkNoteDTOs;

        ISharedPreferences _prefs;

        //File _photoFile;
        //File _photoDir;


        TextView _RecordInfoPrimary;
        TextView _RecordInfoSecondary;
        TextView _RecordInfoDetails;
        TextView _statusTextView;

        LinearLayout _RecordInfoContainer;

        ListView _listView;


        //TextView _vehVinTextView;
        //TextView _vehMakeTextView;
        //TextView _vehNoTextView;
        //TextView _vehStateTextView;
        //TextView _vehTypeTextView;
        //TextView _vehDateTextView;

        //TextView _officerIdTextView;
        //TextView _officerNameTextView;

        //TextView _locMeterTextView;
        //TextView _locBlcokTextView;
        //TextView _locDireTextView;
        //TextView _locStreetTextView;

        //TextView _vioSelectTextView;
        //TextView _vioCodeTextView;
        //TextView _vioDescTextView;
        //TextView _vioFeeTextView;
        ////TextView vioFineTextView;

        Button _submitBtn;
        //Button _viewTicketBtn;


        Button _printBtn;
        //Button _localPrintBtn;


        Button _notesBtn;
        Button _backBtn;
        Button _issueAnotherBtn;

        string _ticketId;       
        ParkingDTO _parkingDTO;
        ProgressDialog _progressDialog;
        string _structName;




        ////OnResume life cycle method
        //public override void OnResume()
        //{
        //    // Always call the superclass first.
        //    base.OnResume();

        //    //if (viewPager.getCurrentItem() == 8)
        //    //{
        //    //    foo();
        //    //    //Your code here. Executed when fragment is seen by user.
        //           //}
            
        //    var _ticketscrollviewMain = _view.FindViewById<ScrollView>(Resource.Id.ticketdetailscrollviewMain);

        //    if (_ticketscrollviewMain != null )
        //    {
        //        // start at the top
        //        _ticketscrollviewMain.ScrollTo(0, 0);
        //    }
        //}

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // AJW - always call the superclass
            base.OnCreateView(inflater, container, savedInstanceState);


            _view = inflater.Inflate(Resource.Layout.TicketDetailV2, null);

            _RecordInfoPrimary = _view.FindViewById<TextView>(Resource.Id.recordinfoprimary);
            _RecordInfoSecondary = _view.FindViewById<TextView>(Resource.Id.recordinfosecondary);
           // _RecordInfoDetails = _view.FindViewById<TextView>(Resource.Id.recordinfodetails);


            _RecordInfoContainer = _view.FindViewById<LinearLayout>(Resource.Id.RecordInfoContainer);

            //_vehVinTextView = _view.FindViewById<TextView>(Resource.Id.VehVinText);
            //_vehMakeTextView = _view.FindViewById<TextView>(Resource.Id.VehMakeText);
            //_vehNoTextView = _view.FindViewById<TextView>(Resource.Id.VecNoText);
            //_vehStateTextView = _view.FindViewById<TextView>(Resource.Id.VecStateText);
            //_vehTypeTextView = _view.FindViewById<TextView>(Resource.Id.VehTypeText);
            //_vehDateTextView = _view.FindViewById<TextView>(Resource.Id.VehExpDateText);

            //_officerIdTextView = _view.FindViewById<TextView>(Resource.Id.OfficerIdText);
            //_officerNameTextView = _view.FindViewById<TextView>(Resource.Id.OfficerNameText);

            //_locMeterTextView = _view.FindViewById<TextView>(Resource.Id.LocMeterText);
            //_locBlcokTextView = _view.FindViewById<TextView>(Resource.Id.LocBlockText);
            //_locDireTextView = _view.FindViewById<TextView>(Resource.Id.LocDirectionText);
            //_locStreetTextView = _view.FindViewById<TextView>(Resource.Id.LocStreetText);

            //_vioSelectTextView = _view.FindViewById<TextView>(Resource.Id.VioSelectText);
            //_vioCodeTextView = _view.FindViewById<TextView>(Resource.Id.VioCodeText);
            //_vioDescTextView = _view.FindViewById<TextView>(Resource.Id.VioDescText);
            //_vioFeeTextView = _view.FindViewById<TextView>(Resource.Id.VioFeeText);
            ////vioFineTextView = view.FindViewById<TextView>(Resource.Id.VioFeeText);

            _listView = _view.FindViewById<ListView>(Resource.Id.noteslist);

            //_viewTicketBtn = _view.FindViewById<Button>(Resource.Id.ViewTicketButton2);

            _submitBtn = _view.FindViewById<Button>(Resource.Id.button12);



            _printBtn = _view.FindViewById<Button>(Resource.Id.PrintButton2);
            //_localPrintBtn = _view.FindViewById<Button>(Resource.Id.LocalPrintButton2);
            _notesBtn = _view.FindViewById<Button>(Resource.Id.NotesButton2);
            _backBtn = _view.FindViewById<Button>(Resource.Id.BackButton2);
            _issueAnotherBtn = _view.FindViewById<Button>(Resource.Id.IssueAnotherButton2);

            //_viewTicketBtn.Click += BtnViewTicketClick;
            _submitBtn.Click += BtnSubmitClick;


            _printBtn.Click += BtnPrintClick;
            //_localPrintBtn.Click += BtnLocalPrintClick;
            _notesBtn.Click += BtnNotesClick;
            _backBtn.Click += BtnBackClick;
            _issueAnotherBtn.Click += BtnIssueAnotherClick;

            CreateLayout();
            return _view;
        }

        //Async Task to retrieve Notes by a Ticket Id from DB
        private async void GetNotesByTicket( string iIssueNo )
        {
            //ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            //String sequenceId = prefs.GetString(Constants.ISSUENO_COLUMN, null);
            string sequenceId = iIssueNo;


            var parkingSequenceADO = new ParkingSequenceADO();
            Task<List<ParkNoteDTO>> result = parkingSequenceADO.GetParkNotesBySequenceId(sequenceId);

            // await! control returns to the caller and the task continues to run on another thread
            _parkNoteDTOs = await result;

            _listView.Adapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, _parkNoteDTOs.Select(x => x.NotesMemo).ToArray());
            //_noNotes.Visibility = _parkNoteDTOList.Count == 0 ? ViewStates.Visible : ViewStates.Gone;

            //_btnNew.Visibility = ViewStates.Visible;
            //_btnNew.Click += BtnNewClick;
        }

        void SetListItemClick()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            ISharedPreferencesEditor editor = prefs.Edit();

            _listView.ItemClick += (sender, e) =>
            {
                ParkNoteDTO parkNoteDTO = _parkNoteDTOs[e.Position];

                editor.PutString(Constants.PARKNOTE_ROWID, parkNoteDTO.DBRowId);
                editor.Apply();

                LoadNoteDetails();
            };
        }

        void LoadNoteDetails()
        {
            // save where we came from
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(Constants.PREVIOUS_FRAGMENT, Constants.DETAIL_FRAGMENT_TAG);
            
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            Fragment notesFragment = FragmentManager.FindFragmentByTag(Constants.NOTES_FRAGMENT_TAG);
            if (notesFragment != null)
                fragmentTransaction.Hide(notesFragment);

            var noteDetail = (NoteDetailFragment)FragmentManager.FindFragmentByTag(Constants.NOTE_DETAIL_FRAGMENT_TAG);
            if (noteDetail != null)
            {
                fragmentTransaction.Show(noteDetail);
                noteDetail.CreateLayout();
            }
            else
                fragmentTransaction.Replace(Resource.Id.frameLayout1, new NoteDetailFragment(), Constants.NOTE_DETAIL_FRAGMENT_TAG);

            fragmentTransaction.Commit();
        }



        public async Task<ParkingDTO> CreateLayout()
        {

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity);
            ISharedPreferencesEditor editor = prefs.Edit();
             _structName = prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);
             _ticketId = prefs.GetString(Constants.TICKETID, null);
            //editor.Remove (Constants.ISSUENO_COLUMN);
            //editor.Apply();

            var commonADO = new CommonADO();
            Log.Debug("TicketDetailFragment _ticketId", _ticketId);
            Log.Debug("TicketDetailFragment _structName", _structName);

            Task<ParkingDTO> result = commonADO.GetRowInfoById(_ticketId, _structName);
            _parkingDTO = await result;

            // now load the notes details while the screen is being painted
            GetNotesByTicket( _parkingDTO.sqlIssueNumberStr );  // AJW - TODO this shoud be a universal generated key system..


            //_RecordInfoPrimary.Text = "Review and Confirm";
            _RecordInfoPrimary.Text = "Citation Review";
            //_RecordInfoPrimary.Text = _parkingDTO.VEHICLE_DISPLAY;

            _RecordInfoSecondary.Text = _parkingDTO.ISSUENO_DISPLAY + "  " + _parkingDTO.sqlIssueDateStr + "  " + _parkingDTO.sqlIssueTimeStr;

            //_RecordInfoDetails.Text = _parkingDTO.LOCATION_DISPLAY;

            // hide this for now, maybe only show for mark mode or records without print picture?
            //_RecordInfoContainer.Visibility = ViewStates.Gone;




            //_RecordInfoPrimary.Text = _parkingDTO.sqlIssueNumberPrefixStr;
            //_RecordInfoSecondary.Text = _parkingDTO.sqlIssueNumberStr;
            //_statusTextView.Text = _parkingDTO.Status;
            //_RecordInfoDetails.Text = _parkingDTO.sqlIssueDateStr + " " + _parkingDTO.sqlIssueTimeStr;

            //_vehVinTextView.Text = _parkingDTO.sqlVehVINStr;
            //_vehMakeTextView.Text = _parkingDTO.sqlVehMakeStr;
            //_vehNoTextView.Text = _parkingDTO.sqlVehLicNoStr;
            //_vehStateTextView.Text = _parkingDTO.sqlVehLicStateStr;
            //_vehTypeTextView.Text = _parkingDTO.sqlVehPlateTypeStr;
            //_vehDateTextView.Text = _parkingDTO.sqlVehLicExpDateStr + " " + _parkingDTO.sqlVehYearDateStr;

            //_officerIdTextView.Text = _parkingDTO.sqlIssueOfficerIDStr;
            //_officerNameTextView.Text = _parkingDTO.sqlIssueOfficerNameStr;

            //_locMeterTextView.Text = _parkingDTO.LocMeter;
            //_locBlcokTextView.Text = _parkingDTO.sqlLocBlockStr;
            //_locDireTextView.Text = _parkingDTO.sqlLocDirectionStr;
            //_locStreetTextView.Text = _parkingDTO.sqlLocStreetStr;

            //_vioSelectTextView.Text = _parkingDTO.VioSelect;
            //_vioCodeTextView.Text = _parkingDTO.VioCode;
            //_vioDescTextView.Text = _parkingDTO.VioDesc;
            //_vioFeeTextView.Text = _parkingDTO.VioFee;
            ////vioFineTextView.Text = parkingDTO.vioFine;        

            if (Constants.MARKMODE_TABLE.Equals(_structName))
            {
                _submitBtn.Visibility = ViewStates.Gone;


                _printBtn.Visibility = ViewStates.Gone;


                //_localPrintBtn.Visibility = ViewStates.Gone;

                //_viewTicketBtn.Visibility = ViewStates.Gone;

                _notesBtn.Visibility = ViewStates.Gone;
                _issueAnotherBtn.Visibility = ViewStates.Gone;               
            }
            else
            {
                if (Constants.STATUS_INPROCESS.Equals(_parkingDTO.Status)
                || Constants.STATUS_READY.Equals(_parkingDTO.Status))
                {
                    _submitBtn.Visibility = ViewStates.Gone;

                    _printBtn.Visibility = ViewStates.Visible;
                    // redundant now.... _localPrintBtn.Visibility = ViewStates.Visible;
                    //_localPrintBtn.Visibility = ViewStates.Gone;

                }
                else if (Constants.STATUS_ISSUED.Equals(_parkingDTO.Status) || Constants.STATUS_REISSUE.Equals(_parkingDTO.Status))
                {
                    _submitBtn.Visibility = ViewStates.Visible;


                    _printBtn.Visibility = ViewStates.Visible;

                    // AJW - redundant now  _localPrintBtn.Visibility = ViewStates.Visible;
                    //_localPrintBtn.Visibility = ViewStates.Gone;


                    _submitBtn.Text = this.Activity.Resources.GetString(Resource.String.VoidButton);
                }
                else if (Constants.STATUS_VOIDED.Equals(_parkingDTO.Status))
                {
                    _submitBtn.Visibility = ViewStates.Visible;
                    _printBtn.Visibility = ViewStates.Visible;

                    // AJW redundant now _localPrintBtn.Visibility = ViewStates.Visible;
                    //_localPrintBtn.Visibility = ViewStates.Gone;

                    _submitBtn.Text = this.Activity.Resources.GetString(Resource.String.ReIssueButton);
                }
                else
                {
                    _submitBtn.Visibility = ViewStates.Gone;
                    _printBtn.Visibility = ViewStates.Gone;
                    //_localPrintBtn.Visibility = ViewStates.Gone;
                }

                if (!Constants.STATUS_INPROCESS.Equals(_parkingDTO.Status))
                    _issueAnotherBtn.Visibility = ViewStates.Visible;
                else
                    _issueAnotherBtn.Visibility = ViewStates.Gone;

                //var sigImg = _view.FindViewById<ImageView>(Resource.Id.sigImg);
                //sigImg = Helper.BuildImageViewWithSig(sigImg, _parkingDTO.sqlIssueNumberStr);


                var linearLayoutMain = _view.FindViewById<LinearLayout>(Resource.Id.LinearLayoutMain);
            }


            // AJW - redundant now  _localPrintBtn.Visibility = ViewStates.Visible;
            //_localPrintBtn.Visibility = ViewStates.Gone;


            // AJW - hide until this functionality is more robust
            _issueAnotherBtn.Visibility = ViewStates.Gone;               


            // file extension is added by reader
            string loTicketImageName = Helper.GetTIssueFormBitmapImageFileNameOnly(_structName, _parkingDTO.sqlIssueNumberStr, DateTime.Today); // TODOD -needs datetime in proper formats


            //// for debug only, show alternate views
            //var sigImg2 = _view.FindViewById<ImageView>(Resource.Id.sigImg);
            //sigImg2 = Helper.GetTIssueFormBitmapImageFromStorage(sigImg2, loTicketImageName);



            //var reproductionImage = _view.FindViewById<ImageView>(Resource.Id.reproductionImage);
            var reproductionImage = _view.FindViewById<ImageView>(Resource.Id.imageReproduction);
            reproductionImage = Helper.GetTIssueFormBitmapImageFromStorage(reproductionImage, loTicketImageName);



            var _ticketscrollviewMain = _view.FindViewById<ScrollView>(Resource.Id.ticketdetailscrollviewMain);

            // start at the top
            _ticketscrollviewMain.ScrollTo(0, 0);



            return _parkingDTO;
        }

        private void BtnViewTicketClick(object sender, EventArgs e)
        {
             Activity.ActionBar.SetSelectedNavigationItem(1);
        }

        private void BtnSubmitClick(object sender, EventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            ISharedPreferencesEditor editor = prefs.Edit();
            if (_submitBtn.Text.Equals(this.Activity.Resources.GetString(Resource.String.VoidButton)))
            {
                //TODO we need to put the sequence id here so we can get the master key values out of the prefs when voiding a ticket.
                editor.PutString(Constants.VOID_AITICKETID, _parkingDTO.sqlIssueNumberStr);
                editor.Apply();

                FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
                Fragment dtlFragment = FragmentManager.FindFragmentByTag(Constants.DETAIL_FRAGMENT_TAG);
                if (dtlFragment != null)
                    fragmentTransaction.Hide(dtlFragment);

                Fragment voidFragment = FragmentManager.FindFragmentByTag(Constants.VOID_FRAGMENT_TAG);
                if (voidFragment != null)
                    fragmentTransaction.Show(voidFragment);
                else
                {
                    voidFragment = new VoidFragment();
                    fragmentTransaction.Replace(Resource.Id.frameLayout1, voidFragment, Constants.VOID_FRAGMENT_TAG);
                }
                fragmentTransaction.Commit();
            }
            else
            {
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

                //FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();

                //Fragment detailFragment = FragmentManager.FindFragmentByTag(Constants.DETAIL_FRAGMENT_TAG);
                //if (detailFragment != null)
                //    fragmentTransaction.Hide(detailFragment);

                //Fragment tksFragment = FragmentManager.FindFragmentByTag(Constants.TICKETS_FRAGMENT_TAG);
                //if (tksFragment != null)
                //    fragmentTransaction.Show(tksFragment);
                //else
                //    fragmentTransaction.Replace(Resource.Id.frameLayout1, new TicketsFragment(), Constants.TICKETS_FRAGMENT_TAG);

                //fragmentTransaction.Commit();
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


        #region Deprecated_Code_Saved_For_Reference
        /*
         * 
         *  AJW - implemented correctly in PrintManager - remove this code 
         *  

        public Bitmap GetPrintBitmap(String issueNum, DateTime issueDate)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            ISharedPreferencesEditor editor = prefs.Edit();
            string structName = prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);
            return (DroidContext.XmlCfg.PrintPicture(Constants.STRUCT_TYPE_CITE, structName));
        }
                 */



                /*
         * 
         *  AJW - implemented correctly in PrintManager - remove this code 
         *  

        public async void PrintDummyTicket(BinaryWriter bw, String issueNum, DateTime issueDate)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            ISharedPreferencesEditor editor = prefs.Edit();
            string structName = prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);
            var commonADO = new CommonADO();
            Task<ParkingDTO> result = commonADO.GetRowInfoById(issueNum, structName);
            var _parkingDTO = await result;
            String printext = "********************************" + "\x0d\x0a";
            if (_parkingDTO.IssueNo == issueNum)
            {
                printext += "Issue No.: " + issueNum + "\x0d\x0a";
                printext += "Date: " + issueDate + "\x0d\x0a";
                printext += "Date: " + _parkingDTO.LocBlcok + "\x0d\x0a";
            }

            byte[] printdata = Encoding.ASCII.GetBytes(printext);
            bw.Write(printdata);

        }
                 */



                /*
         * 
         *  AJW - implemented correctly in PrintManager - remove this code 
         *  

        private BitmapData GetBitmapData(String issueNum, DateTime issueDate, bool localFlag)
        {
            // get the byte array from the web service
            Bitmap imgBitMap = null;
            if (localFlag == false)
            {
                var userService = new DuncanWebServicesClient.UserDetailsService();
                byte[] imageBytes;
                try
                {
                    imageBytes = userService.GetTicketBytes(issueNum, issueDate);
                }
                catch (Exception e)
                {
                    Log.Debug("TicketDetailsFragment", "Unable to get the bitmap data from server");
                    return null;
                }
                imgBitMap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
            }
            else
                imgBitMap = GetPrintBitmap(issueNum, issueDate);

            using (var bitmap = imgBitMap)
            {
                const int threshold = 127;
                var index = 0;
                var dimensions = bitmap.Width * bitmap.Height;
                var dots = new BitArray(dimensions);

                for (var y = 0; y < bitmap.Height; y++)
                {
                    for (var x = 0; x < bitmap.Width; x++)
                    {
                        int c = bitmap.GetPixel(x, y);
                        var color = new Color(c);
                        var luminance = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                        dots[index] = (luminance < threshold);
                        index++;
                    }
                }

                return new BitmapData()
                {
                    Dots = dots,
                    Height = bitmap.Height,
                    Width = bitmap.Width
                };
            }
        }
                 */


        /*
         * 
         *  AJW - implemented correctly in PrintManager - remove this code 
         *  

        private void RenderPrintImage(BinaryWriter bw, String issueNum, DateTime issueDate, bool localFlag)
        {
            BitmapData data = GetBitmapData(issueNum, issueDate, localFlag);
            if (data == null)
            {
                PrintDummyTicket(bw, issueNum, issueDate);
                return;
            }
            var dots = data.Dots;
            var width = BitConverter.GetBytes(data.Width);

            bw.Write(AsciiControlChars.Escape);
            bw.Write('*');         // bit-image mode
            bw.Write((byte)0);     // 8-dot single-density
            bw.Write((byte)5);     // width low byte
            bw.Write((byte)0);     // width high byte
            bw.Write((byte)128);
            bw.Write((byte)64);
            bw.Write((byte)32);
            bw.Write((byte)16);
            bw.Write((byte)8);


            bw.Write(AsciiControlChars.Escape);
            bw.Write('*');         // bit-image mode
            bw.Write((byte)0);     // 8-dot single-density
            bw.Write((byte)5);     // width low byte
            bw.Write((byte)0);     // width high byte
            bw.Write((byte)1);
            bw.Write((byte)2);
            bw.Write((byte)4);
            bw.Write((byte)8);
            bw.Write((byte)16);

            bw.Write(AsciiControlChars.Newline);

            // So we have our bitmap data sitting in a bit array called "dots."
            // This is one long array of 1s (black) and 0s (white) pixels arranged
            // as if we had scanned the bitmap from top to bottom, left to right.
            // The printer wants to see these arranged in bytes stacked three high.
            // So, essentially, we need to read 24 bits for x = 0, generate those
            // bytes, and send them to the printer, then keep increasing x. If our
            // image is more than 24 dots high, we have to send a second bit image
            // command.

            // Set the line spacing to 24 dots, the height of each "stripe" of the
            // image that we're drawing.
            bw.Write(AsciiControlChars.Escape);
            bw.Write('3');
            bw.Write((byte)24);

            // OK. So, starting from x = 0, read 24 bits down and send that data
            // to the printer.
            int offset = 0;

            while (offset < data.Height)
            {
                bw.Write(AsciiControlChars.Escape);
                bw.Write('*');         // bit-image mode
                bw.Write((byte)33);    // 24-dot double-density
                bw.Write(width[0]);  // width low byte
                bw.Write(width[1]);  // width high byte

                for (int x = 0; x < data.Width; ++x)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        byte slice = 0;

                        for (int b = 0; b < 8; ++b)
                        {
                            int y = (((offset / 8) + k) * 8) + b;

                            // Calculate the location of the pixel we want in the bit array.
                            // It'll be at (y * width) + x.
                            int i = (y * data.Width) + x;

                            // If the image is shorter than 24 dots, pad with zero.
                            bool v = false;
                            if (i < dots.Length)
                            {
                                v = dots[i];
                            }

                            slice |= (byte)((v ? 1 : 0) << (7 - b));
                        }

                        bw.Write(slice);
                    }
                }

                offset += 24;
                bw.Write(AsciiControlChars.Newline);
            }

            // Restore the line spacing to the default of 30 dots.
            bw.Write(AsciiControlChars.Escape);
            bw.Write('3');
            bw.Write((byte)30);

        }
*/
        #endregion


        #region PCX Image Support
        /*
        private static void WriteWord(Stream stream, int data)
        {
            stream.WriteByte((byte)(data & 0xFF));
            stream.WriteByte((byte)((data >> 8) & 0xFF));
        }

        public static byte[] ConvertToPCX(byte[] bmp, int width, int height)
        {
            int SubStride = 72;
            if (width == 576)
                SubStride = 72;
            else if (width == 832)
                SubStride = 104;

            int Stride = 104; // 832 / 8
            MemoryStream pcxStream = new MemoryStream();
            try
            {
                //header
                pcxStream.WriteByte(10); //    char manufacturer;
                pcxStream.WriteByte(5); //char version;
                pcxStream.WriteByte(1); //char encoding;
                pcxStream.WriteByte(1); // char bpp;
                pcxStream.WriteByte(0);
                pcxStream.WriteByte(0); //char xmin[2];
                pcxStream.WriteByte(0);
                pcxStream.WriteByte(0); //char ymin[2];

                WriteWord(pcxStream, width - 1); // char xmax[2];
                WriteWord(pcxStream, height - 1); //char ymax[2];

                WriteWord(pcxStream, 72); //word(pcx->hdpi, 72);
                WriteWord(pcxStream, 72); // word(pcx->vdpi, 72);
                for (int i = 0; i < 16 * 3; i++) //4bpp palette
                {
                    pcxStream.WriteByte(0);
                }
                pcxStream.WriteByte(0); // pcx->res = 0;
                pcxStream.WriteByte(1); // pcx->nplanes = 1;

                WriteWord(pcxStream, SubStride /*Stride*/ /*data.Stride* /); // word(pcx->bytesperline, width / 2);

                WriteWord(pcxStream, 0); //word(pcx->palletteinfo, 0);
                WriteWord(pcxStream, 0); //word(pcx->hscrn, 0);
                WriteWord(pcxStream, 0); //word(pcx->vscrn, 0);

                for (int i = 0; i < 54; i++) //memset(pcx->filler, 0, 54);
                {
                    pcxStream.WriteByte(0);
                }

                // End of header

                //read all bytes to an array
                byte[] baseLine = bmp; // data.Scan0;
                // Declare an array to hold the bytes of the bitmap.
                int byteLength = SubStride /*Stride* / * height; //data.Stride * data.Height;
                byte[] bytes = new byte[byteLength];

                // Copy the RGB values into the array.
                for (int y = 0; y < height; y++)
                {
                    int lineOffset = y * Stride;
                    //Debug.WriteLine(string.Format("Y={0}, Offset={1}", y, lineOffset));
                    for (int x = 0; x < SubStride /*Stride * /; x++)
                    {
                        bytes[y * SubStride /*Stride* / + x] = baseLine[lineOffset + x];// Marshal.ReadByte(baseLine, lineOffset + x);
                    }
                }

                int baseIdx = 0;
                int end = byteLength;
                int run = 0;
                int ldata = -1;
                byte ld;

                while (baseIdx < end)
                {
                    //if it matches, increase the run by 1 up to max of 63
                    if ((bytes[baseIdx] == ldata) && (run < 63)) run++;
                    else
                    {
                        //write data
                        if (run != 0) //not first run
                        {
                            ld = (byte)ldata;
                            if ((run > 1) || (ld >= 0xC0))
                                pcxStream.WriteByte((byte)(0xC0 | run));
                            pcxStream.WriteByte(ld);
                        }
                        run = 1;
                    }
                    ldata = bytes[baseIdx];
                    baseIdx++;
                }
                ld = (byte)((ldata >> 4) | (ldata << 4));
                if ((run > 1) || (ld >= 0xC0))
                    pcxStream.WriteByte((byte)(0xC0 | run));
                pcxStream.WriteByte(ld);
            }
            finally
            {
                /*bmp.UnlockBits(data);* /
            }

            pcxStream.Flush();
            pcxStream.Position = 0;
            pcxStream.Capacity = Convert.ToInt32(pcxStream.Length);

            byte[] result = new byte[Convert.ToInt32(pcxStream.Length)];
            pcxStream.Read(result, 0, Convert.ToInt32(pcxStream.Length));

            return result;
        }
*/
        #endregion


        #region BMP (1BPP) Image Support
/*
        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public BITMAPINFOHEADER(ushort bpp, int height, int width)
            {
                biBitCount = bpp;
                biWidth = width;
                biHeight = height;

                biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                biPlanes = 1; // must be 1
                biCompression = 0; // no compression
                biSizeImage = 0; // no compression, so can be 0
                biXPelsPerMeter = 0;
                biYPelsPerMeter = 0;
                biClrUsed = 0;
                biClrImportant = 0;
            }

            public void Store(BinaryWriter bw)
            {
                Store(bw, null);
            }

            public void Store(BinaryWriter bw, uint[] colorPalette)
            {
                // Must maintain order for file writing
                bw.Write(biSize);
                bw.Write(biWidth);
                bw.Write(biHeight);
                bw.Write(biPlanes);
                bw.Write(biBitCount);
                bw.Write(biCompression);
                bw.Write(biSizeImage);
                bw.Write(biXPelsPerMeter);
                bw.Write(biYPelsPerMeter);
                bw.Write(biClrUsed);
                bw.Write(biClrImportant);

                // write color palette if 8 bpp or less
                if (biBitCount <= 8)
                {
                    if (colorPalette == null)
                        throw new ArgumentNullException("bpp is 8 or less, color palette is required");

                    uint paletteCount = BITMAPFILEHEADER.CalcPaletteSize(biBitCount) / 4;
                    if (colorPalette.Length < paletteCount)
                        throw new ArgumentException(string.Format("bpp is 8 or less, color palette must contain {0} colors", paletteCount));

                    foreach (uint color in colorPalette)
                        bw.Write(color);
                }
            }

            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPFILEHEADER
        {
            public BITMAPFILEHEADER(BITMAPINFOHEADER info, out uint sizeOfImageData)
            {
                bfType = 0x4D42;  // Microsoft supplied value to indicate Bitmap 'BM'
                bfReserved1 = 0;
                bfReserved2 = 0;

                // calculate amount of space needed for color palette
                uint paletteSize = CalcPaletteSize(info.biBitCount);

                bfOffBits = 54 + paletteSize; // default value + paletteSize

                // calculate size of image
                sizeOfImageData = (uint)(CalcRowSize(info.biWidth * info.biBitCount) * info.biHeight);
                bfSize = sizeOfImageData + bfOffBits;
            }

            private static int CalcRowSize(int bits)
            {
                return ((((bits) + 31) / 32) * 4);
            }

            public static uint CalcPaletteSize(int bpp)
            {
                // 8 bpp or less, needs an uint per color
                if (bpp <= 8)
                    return 4 * (uint)Math.Pow(2, bpp);

                // no palette needed for 16bpp or higher
                return 0;
            }

            public void Store(BinaryWriter bw)
            {
                // Must maintain order for file writing
                bw.Write(bfType);
                bw.Write(bfSize);
                bw.Write(bfReserved1);
                bw.Write(bfReserved2);
                bw.Write(bfOffBits);
            }

            public ushort bfType;
            public uint bfSize;
            public short bfReserved1;
            public short bfReserved2;
            public uint bfOffBits;
        }

        // Deprecated
        /*
        /// <summary>
        /// Converts a "normal" bitmap to a 1BPP bitmap (2-color)?
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] GetByteArray(Bitmap image)
        {
            IntPtr hbmOld;
            IntPtr hBitmap;
            IntPtr hDC;

            // create infoheader
            BITMAPINFOHEADER bih = new BITMAPINFOHEADER(1, image.Height, image.Width);
            // set black and white for 1 bit color palette

            // create fileheader and get data size
            uint sizeOfImageData;
            BITMAPFILEHEADER bfh = new BITMAPFILEHEADER(bih, out sizeOfImageData);

            IPlatformDependentGDI WinGDI;
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
                WinGDI = new WinCE_GDI();
            else
                WinGDI = new Win32_GDI();

            // create device context in memory
            hDC = WinGDI.CreateCompatibleDC(IntPtr.Zero);

            // Not entirely sure why, but we have to allocate heap memory and 
            // copy our BITMAPINFO structure there before calling CreateDIBSection
            IntPtr pBI = WinGDI.LocalAlloc(0x40, Marshal.SizeOf(bih)); // LMEM_ZEROINIT = 0x40
            Marshal.StructureToPtr(bih, pBI, false);


            // create a 1 bpp DIB
            IntPtr pBits = IntPtr.Zero;
            //hBitmap = WinGDI.CreateDIBSection(hDC, ref bih, 1, ref pBits, IntPtr.Zero, 0);
            hBitmap = WinGDI.CreateDIBSection(hDC, pBI, 1, ref pBits, IntPtr.Zero, 0);

            // selet DIB into device context
            hbmOld = WinGDI.SelectObject(hDC, hBitmap);

            using (Graphics g = Graphics.FromHdc(hDC))
            {
                g.DrawImage(image, 0, 0);
            }

            byte[] imageData = new byte[sizeOfImageData];
            byte[] fileData;

            using (MemoryStream ms = new MemoryStream((int)bfh.bfSize))
            {
                using (BinaryWriter w = new BinaryWriter(ms))
                {
                    bfh.Store(w);
                    // store bitmapinfoheader with 1 bpp color palette for black and white
                    bih.Store(w, new uint[] { (uint)0x0, (uint)0xffffff });

                    // copy image data into imageData buffer
                    Marshal.Copy(pBits, imageData, 0, imageData.Length);

                    // write imageData to stream
                    w.Write(imageData);

                    w.Close();
                }

                fileData = ms.GetBuffer();
                ms.Close();
            }

            // select old object
            if (hbmOld != IntPtr.Zero)
                WinGDI.SelectObject(hDC, hbmOld);

            // delete memory bitmap
            if (hBitmap != IntPtr.Zero)
                WinGDI.DeleteObject(hBitmap);

            // delete memory device context
            if (hDC != IntPtr.Zero)
                WinGDI.DeleteDC(hDC);

            return fileData;
        }
        * /

        /// <summary>
        /// Returns a byte array of an entire BMP file based on just the 1BPP
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static byte[] GetFullBitmapByteArray(byte[] bmp, int Width, int Height)
        {
            IntPtr hbmOld;
            IntPtr hBitmap;
            IntPtr hDC;

            // create infoheader
            BITMAPINFOHEADER bih = new BITMAPINFOHEADER(1, Height, Width);
            // set black and white for 1 bit color palette

            // create fileheader and get data size
            uint sizeOfImageData;
            BITMAPFILEHEADER bfh = new BITMAPFILEHEADER(bih, out sizeOfImageData);

            IPlatformDependentGDI WinGDI;
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
                WinGDI = new WinCE_GDI();
            else
                WinGDI = new Win32_GDI();

            // create device context in memory
            hDC = WinGDI.CreateCompatibleDC(IntPtr.Zero);

            // Not entirely sure why, but we have to allocate heap memory and 
            // copy our BITMAPINFO structure there before calling CreateDIBSection
            IntPtr pBI = WinGDI.LocalAlloc(0x40, Marshal.SizeOf(bih)); // LMEM_ZEROINIT = 0x40
            Marshal.StructureToPtr(bih, pBI, false);

            // create a 1 bpp DIB
            IntPtr pBits = IntPtr.Zero;
            hBitmap = WinGDI.CreateDIBSection(hDC, pBI / *ref bih* /, 1, ref pBits, IntPtr.Zero, 0);

            // selet DIB into device context
            hbmOld = WinGDI.SelectObject(hDC, hBitmap);

            / *
            using (Graphics g = Graphics.FromHdc(hDC))
            {
                g.DrawImage(image, 0, 0);
            }
            * /

            byte[] imageData = bmp; // new byte[sizeOfImageData];
            byte[] fileData;

            using (MemoryStream ms = new MemoryStream((int)bfh.bfSize))
            {
                using (BinaryWriter w = new BinaryWriter(ms))
                {
                    bfh.Store(w);
                    // store bitmapinfoheader with 1 bpp color palette for black and white
                    bih.Store(w, new uint[] { (uint)0x0, (uint)0xffffff });

                    // copy image data into imageData buffer
                    //Marshal.Copy(pBits, imageData, 0, imageData.Length);

                    // write imageData to stream
                    w.Write(imageData);

                    w.Close();
                }

                fileData = ms.GetBuffer();
                ms.Close();
            }

            // select old object
            if (hbmOld != IntPtr.Zero)
                WinGDI.SelectObject(hDC, hbmOld);

            // delete memory bitmap
            if (hBitmap != IntPtr.Zero)
                WinGDI.DeleteObject(hBitmap);

            // delete memory device context
            if (hDC != IntPtr.Zero)
                WinGDI.DeleteDC(hDC);

            return fileData;
        }
*/
        #endregion


        private byte[] ConvertToZebraPNGImageData(string iFileName)
        {
            // needs StringBuilder 
            string zplImageData = string.Empty;
            //Make sure no transparency exists. I had some trouble with this. This PNG has a white background
            //string filePath = @"C:\Users\Path\To\Logo.png";
            byte[] binaryData = System.IO.File.ReadAllBytes(iFileName);
            foreach (Byte b in binaryData)
            {
                string hexRep = String.Format("{0:X}", b);
                if (hexRep.Length == 1)
                    hexRep = "0" + hexRep;
                zplImageData += hexRep;
            }
            string zplToSend = "^XA" + "^MNN" + "^LL500" + "~DYE:LOGO,P,P," + binaryData.Length + ",," + zplImageData + "^XZ";

            return System.Text.Encoding.UTF8.GetBytes(zplToSend);  // or Encoding.ASCII.GetBytes
        }


     
     

        private Bitmap SaveTicketImageToDeviceStorage(String issueNum, DateTime issueDate)
        {
            PrintManager printMgr = new PrintManager();
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            ISharedPreferencesEditor editor = prefs.Edit();
            string structName = prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);
            Bitmap imgBitMap = printMgr.PrintPicture(structName, issueNum, issueDate);

            var photoDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures) + "/" + Constants.MULTIMEDIA_FOLDERNAME;

            Java.IO.File MediaDir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), Constants.MULTIMEDIA_FOLDERNAME);


            // AJW - TODO format date as constant mask
            Java.IO.File ticketImageFile = new Java.IO.File(MediaDir, issueNum.Trim() + ".bmp" );

            //Java.IO.File sigFile = new Java.IO.File(MediaDir, System.DateTimeOffset.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".bmp");
            //Java.IO.File sigFile = new Java.IO.File(MediaDir, thisStruct.sequenceId + Constants.SIG_FILE_SUFFIX);



            if (!Directory.Exists(photoDirectory))
                Directory.CreateDirectory(photoDirectory);

            // get the bmp image from the sig pad
            //Bitmap sigBmp = sigPad.GetImage();

            // save the image to the device
            using (FileStream fileStream = System.IO.File.OpenWrite(ticketImageFile.ToString()))
            {
                imgBitMap.Compress(Bitmap.CompressFormat.Png, 0, fileStream);
            }

            //return "Ticket saved as a bmp image.";
            return imgBitMap;
        }


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
                Console.WriteLine("Exception source: {0}", e.Source);
            }

        }


        private void BtnNotesClick(object sender, EventArgs e)
        {
            // save where we came from
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(Constants.PREVIOUS_FRAGMENT, Constants.DETAIL_FRAGMENT_TAG);

            // and what ticket we're looking at
            editor.PutString(Constants.ISSUENO_COLUMN, _parkingDTO.sqlIssueNumberStr); // AJW - TODO needs to go away from issueno to db generated keys
            
            editor.Apply();

            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();

            Fragment detailFragment = FragmentManager.FindFragmentByTag(Constants.DETAIL_FRAGMENT_TAG);
            if (detailFragment != null)
            {
                fragmentTransaction.Hide(detailFragment);
            }

            var notesFragment = (NotesFragment)FragmentManager.FindFragmentByTag(Constants.NOTES_FRAGMENT_TAG);
            if (notesFragment != null)
            {
                fragmentTransaction.Show(notesFragment);
                notesFragment.GetNotesByTicket();
            }
            else
                fragmentTransaction.Replace(Resource.Id.frameLayout1, new NotesFragment(), Constants.NOTES_FRAGMENT_TAG);

            fragmentTransaction.Commit();
        }

        //Back Button take to tickets list screen
        private void BtnBackClick(object sender, EventArgs e)
        {


            // figure out where to go back to
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            String prevFragName = prefs.GetString(Constants.PREVIOUS_FRAGMENT, null);
            string structName = prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);

            // where we coming from the ticket lookup?
            //if (prevFragName.Equals(Constants.DETAIL_FRAGMENT_TAG) == true)
            {

                FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
                Fragment detailFragment = FragmentManager.FindFragmentByTag(Constants.DETAIL_FRAGMENT_TAG);
                if (detailFragment != null)
                    fragmentTransaction.Hide(detailFragment);

                Fragment tksFragment = FragmentManager.FindFragmentByTag(Constants.TICKETS_FRAGMENT_TAG);
                if (tksFragment != null)
                    fragmentTransaction.Show(tksFragment);
                else
                    fragmentTransaction.Replace(Resource.Id.frameLayout1, new TicketsFragment(), Constants.TICKETS_FRAGMENT_TAG);

                fragmentTransaction.Commit();
            }
            //else
           // {
                /*
                // go to the first page of the submitted ticket
                String tabPosStr = null;
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
                ISharedPreferencesEditor editor = prefs.Edit();

                if (Constants.STRUCT_TYPE_CITE.Equals(_struct.Type))
                {
                    tabPosStr = prefs.GetString(Constants.LABEL_TICKETS_TAB + Constants.LABEL_TAB_POSITION, null);
                }
                else if (Constants.STRUCT_TYPE_CHALKING.Equals(_struct.Type))
                {
                    tabPosStr = prefs.GetString(Constants.LABEL_CHALKS_TAB + Constants.LABEL_TAB_POSITION, null);
                }
                else
                {
                    tabPosStr = prefs.GetString(_struct.Name + Constants.LABEL_TAB_POSITION, null);
                }
                int tabPosition = 0;
                if (tabPosStr != null)
                {
                    int.TryParse(tabPosStr, out tabPosition);
                }
                 * */

                //int tabPosition = 0;
                //Activity.RunOnUiThread(() => Activity.ActionBar.SetSelectedNavigationItem(tabPosition));
                //Activity.RunOnUiThread(() => Activity._formPanel.ChangeLayout(0));

            //}
        }

        //Create duplicate issue
        public async void BtnIssueAnotherClick(object sender, EventArgs e)
        {
            IssueAnotherCommon();            
        }

        public async void IssueAnotherCommon()
        {
            try
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity);
                ISharedPreferencesEditor editor = prefs.Edit();
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
                //			Fragment detailFragment = FragmentManager.FindFragmentByTag(Constants.DETAIL_FRAGMENT_TAG);
                //			if (detailFragment != null){
                //				fragmentTransaction.Hide(detailFragment);
                //			}
                //
                var issFrag = (CommonFragment)FragmentManager.FindFragmentByTag(structName);
                if (issFrag != null)
                    issFrag.StartTicketLayout();
                fragmentTransaction.Commit();
                this.Activity.ActionBar.SetSelectedNavigationItem(tabPosition);
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, null, "BtnIssueAnotherClick");
            }

        }

        private class BitmapData
        {
            public BitArray Dots
            {
                get;
                set;
            }

            public int Height
            {
                get;
                set;
            }

            public int Width
            {
                get;
                set;
            }
        }
    }
}

