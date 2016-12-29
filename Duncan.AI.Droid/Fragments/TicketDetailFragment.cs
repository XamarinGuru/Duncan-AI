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
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;


//// DEVELOPER NOTE
////
//// This was the original version of the citation review. It has been replaced with the ticketimage display, but this 
//// code is retained for reference, possibluy for use in building review screens for structures that don't have a printed form (i.e. MARKMODE )
////



namespace Duncan.AI.Droid
{
    public class TicketDetailFragment2 : Fragment
    {
        View _view;

        TextView _issuePfxTextView;
        TextView _issueNoTextView;
        TextView _issueDtTimeTextView;
        TextView _statusTextView;

        TextView _vehVinTextView;
        TextView _vehMakeTextView;
        TextView _vehNoTextView;
        TextView _vehStateTextView;
        TextView _vehTypeTextView;
        TextView _vehDateTextView;

        TextView _officerIdTextView;
        TextView _officerNameTextView;

        TextView _locMeterTextView;
        TextView _locBlcokTextView;
        TextView _locDireTextView;
        TextView _locStreetTextView;

        TextView _vioSelectTextView;
        TextView _vioCodeTextView;
        TextView _vioDescTextView;
        TextView _vioFeeTextView;
        //TextView vioFineTextView;

        Button _submitBtn;
        Button _printBtn;
        Button _localPrintBtn;
        Button _viewTicketBtn;
        Button _notesBtn;
        Button _backBtn;
        Button _issueAnotherBtn;

        string _ticketId;       
        ParkingDTO _parkingDTO;
        ProgressDialog _progressDialog;
        string _structName;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // AJW - always call the superclass
            base.OnCreateView(inflater, container, savedInstanceState);


            _view = inflater.Inflate(Resource.Layout.Ticket_detail, null);

            _issuePfxTextView = _view.FindViewById<TextView>(Resource.Id.IssuePfxText);
            _issueNoTextView = _view.FindViewById<TextView>(Resource.Id.IssueNoText);
            _issueDtTimeTextView = _view.FindViewById<TextView>(Resource.Id.IssueDateText);
            _statusTextView = _view.FindViewById<TextView>(Resource.Id.IssueStatusText);

            _vehVinTextView = _view.FindViewById<TextView>(Resource.Id.VehVinText);
            _vehMakeTextView = _view.FindViewById<TextView>(Resource.Id.VehMakeText);
            _vehNoTextView = _view.FindViewById<TextView>(Resource.Id.VecNoText);
            _vehStateTextView = _view.FindViewById<TextView>(Resource.Id.VecStateText);
            _vehTypeTextView = _view.FindViewById<TextView>(Resource.Id.VehTypeText);
            _vehDateTextView = _view.FindViewById<TextView>(Resource.Id.VehExpDateText);

            _officerIdTextView = _view.FindViewById<TextView>(Resource.Id.OfficerIdText);
            _officerNameTextView = _view.FindViewById<TextView>(Resource.Id.OfficerNameText);

            _locMeterTextView = _view.FindViewById<TextView>(Resource.Id.LocMeterText);
            _locBlcokTextView = _view.FindViewById<TextView>(Resource.Id.LocBlockText);
            _locDireTextView = _view.FindViewById<TextView>(Resource.Id.LocDirectionText);
            _locStreetTextView = _view.FindViewById<TextView>(Resource.Id.LocStreetText);

            _vioSelectTextView = _view.FindViewById<TextView>(Resource.Id.VioSelectText);
            _vioCodeTextView = _view.FindViewById<TextView>(Resource.Id.VioCodeText);
            _vioDescTextView = _view.FindViewById<TextView>(Resource.Id.VioDescText);
            _vioFeeTextView = _view.FindViewById<TextView>(Resource.Id.VioFeeText);
            //vioFineTextView = view.FindViewById<TextView>(Resource.Id.VioFeeText);

            _viewTicketBtn = _view.FindViewById<Button>(Resource.Id.ViewTicketButton);
            _submitBtn = _view.FindViewById<Button>(Resource.Id.button1);
            _printBtn = _view.FindViewById<Button>(Resource.Id.PrintButton);
            _localPrintBtn = _view.FindViewById<Button>(Resource.Id.LocalPrintButton);
            _notesBtn = _view.FindViewById<Button>(Resource.Id.NotesButton);
            _backBtn = _view.FindViewById<Button>(Resource.Id.BackButton);
            _issueAnotherBtn = _view.FindViewById<Button>(Resource.Id.IssueAnotherButton);

            _viewTicketBtn.Click += BtnViewTicketClick;
            _submitBtn.Click += BtnSubmitClick;
            _printBtn.Click += BtnPrintClick;
            _localPrintBtn.Click += BtnLocalPrintClick;
            _notesBtn.Click += BtnNotesClick;
            _backBtn.Click += BtnBackClick;
            _issueAnotherBtn.Click += BtnIssueAnotherClick;

            CreateLayout();
            return _view;
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

            _issuePfxTextView.Text = _parkingDTO.sqlIssueNumberPrefixStr;
            _issueNoTextView.Text = _parkingDTO.sqlIssueNumberStr;
            _statusTextView.Text = _parkingDTO.Status;
            _issueDtTimeTextView.Text = _parkingDTO.sqlIssueDateStr + " " + _parkingDTO.sqlIssueTimeStr;

            _vehVinTextView.Text = _parkingDTO.sqlVehVINStr;
            _vehMakeTextView.Text = _parkingDTO.sqlVehMakeStr;
            _vehNoTextView.Text = _parkingDTO.sqlVehLicNoStr;
            _vehStateTextView.Text = _parkingDTO.sqlVehLicStateStr;
            _vehTypeTextView.Text = _parkingDTO.sqlVehPlateTypeStr;
            _vehDateTextView.Text = _parkingDTO.sqlVehLicExpDateStr + " " + _parkingDTO.sqlVehYearDateStr;

            _officerIdTextView.Text = _parkingDTO.sqlIssueOfficerIDStr;
            _officerNameTextView.Text = _parkingDTO.sqlIssueOfficerNameStr;

            _locMeterTextView.Text = _parkingDTO.LocMeter;
            _locBlcokTextView.Text = _parkingDTO.sqlLocBlockStr;
            _locDireTextView.Text = _parkingDTO.sqlLocDirectionStr;
            _locStreetTextView.Text = _parkingDTO.sqlLocStreetStr;

            _vioSelectTextView.Text = _parkingDTO.VioSelect;
            _vioCodeTextView.Text = _parkingDTO.VioCode;
            _vioDescTextView.Text = _parkingDTO.VioDesc;
            _vioFeeTextView.Text = _parkingDTO.VioFee;
            //vioFineTextView.Text = parkingDTO.vioFine;        

            if (Constants.MARKMODE_TABLE.Equals(_structName))
            {
                _submitBtn.Visibility = ViewStates.Gone;
                _printBtn.Visibility = ViewStates.Gone;
                _localPrintBtn.Visibility = ViewStates.Gone;
                _viewTicketBtn.Visibility = ViewStates.Gone;
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
                    _localPrintBtn.Visibility = ViewStates.Gone;

                }
                else if (Constants.STATUS_ISSUED.Equals(_parkingDTO.Status) || Constants.STATUS_REISSUE.Equals(_parkingDTO.Status))
                {
                    _submitBtn.Visibility = ViewStates.Visible;
                    _printBtn.Visibility = ViewStates.Visible;

                    // AJW - redundant now  _localPrintBtn.Visibility = ViewStates.Visible;
                    _localPrintBtn.Visibility = ViewStates.Gone;


                    _submitBtn.Text = this.Activity.Resources.GetString(Resource.String.VoidButton);
                }
                else if (Constants.STATUS_VOIDED.Equals(_parkingDTO.Status))
                {
                    _submitBtn.Visibility = ViewStates.Visible;
                    _printBtn.Visibility = ViewStates.Visible;

                    // AJW redundant now _localPrintBtn.Visibility = ViewStates.Visible;
                    _localPrintBtn.Visibility = ViewStates.Gone;

                    _submitBtn.Text = this.Activity.Resources.GetString(Resource.String.ReIssueButton);
                }
                else
                {
                    _submitBtn.Visibility = ViewStates.Gone;
                    _printBtn.Visibility = ViewStates.Gone;
                    _localPrintBtn.Visibility = ViewStates.Gone;
                }

                if (!Constants.STATUS_INPROCESS.Equals(_parkingDTO.Status))
                    _issueAnotherBtn.Visibility = ViewStates.Visible;
                else
                    _issueAnotherBtn.Visibility = ViewStates.Gone;

                CustomSignatureImageView sigImg = _view.FindViewById<CustomSignatureImageView>(Resource.Id.sigImg);
                sigImg = Helper.BuildCustomSignatureImageView(sigImg, _parkingDTO.sqlIssueNumberStr);
                var linearLayoutMain = _view.FindViewById<LinearLayout>(Resource.Id.LinearLayoutMain);
            }

            // ticket image name YYYY MM DD iSSUE_NO_DISPLAY
            //string loTicketImageName = _parkingDTO.IssuePfx.Trim() + _parkingDTO.IssueNo.Trim(); // + " " + _parkingDTO.IssueDt.Tr date format needs to be constant fromt/to db


            //string loTicketImageName = "2016_01_07_00_00_27"; 

            // file extension is added by reader
            string loTicketImageName = Helper.GetTIssueFormBitmapImageFileNameOnly(_structName, _parkingDTO.sqlIssueNumberStr, DateTime.Today); // TODOD -needs datetime in proper formats


            // for debug only, show alternate views
            var sigImg2 = _view.FindViewById<ImageView>(Resource.Id.sigImg);
            sigImg2 = Helper.GetTIssueFormBitmapImageFromStorage(sigImg2, loTicketImageName);



            //var reproductionImage = _view.FindViewById<ImageView>(Resource.Id.reproductionImage);
            var reproductionImage = _view.FindViewById<ImageView>(Resource.Id.ticketImg);
            reproductionImage = Helper.GetTIssueFormBitmapImageFromStorage(reproductionImage, loTicketImageName);
            //var linearLayoutMain = _view.FindViewById<LinearLayout>(Resource.Id.LinearLayoutMain);



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

        private void BtnLocalPrintClick(object sender, EventArgs e)
        {
            _progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Printing Ticket...", true);
            ThreadPool.QueueUserWorkItem(o => SendPrint(_parkingDTO.sqlIssueNumberStr, GetIssueDateWithMask(), true));
        }


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


        /*
        private byte[] GetDocumentForZebra(String issueNum, DateTime issueDate, bool localFlag)
        {
            // causes ticket image file to be created 
            BitmapData data = GetBitmapData(issueNum, issueDate, localFlag);


            String storagePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

            String filePath = storagePath + "/APPNAME2.png";

            return ConvertToZebraPNGImageData( filePath);
        }
*/

        /*
         * 
         *  AJW - implemented correctly in PrintManager - remove this code 
         *  
        private byte[] GetDocumentForZebra(String issueNum, DateTime issueDate, bool localFlag)
        {
            // causes ticket image file to be created 
            BitmapData data = GetBitmapData(issueNum, issueDate, localFlag);

            if (data == null)
            {
                return null;
            }

            int iDotHeight = data.Height;

            int loBytesToPrint = (104 * iDotHeight); // 104 bytes * 8 bits = 832 pixels wide

            // Allocate memory for a copy of bitmap bits
            byte[] RealBits = new byte[104 * iDotHeight]; // 832 bits wide divided by 8 bits/byte = 104 bytes

            // And grab bits from DIBSection data
            //Marshal.Copy(TWinPrnBase.glDrawBitmapDataPtr, RealBits, 0, 104 * iDotHeight);


            / *
            byte[] fullBitmapFile = Printer_Base.GetFullBitmapByteArray(RealBits, 832, iDotHeight);
            using (FileStream OutFile = File.Open("Ticket1BPP.bmp", FileMode.Create))
            {
                OutFile.Write(fullBitmapFile, 0, fullBitmapFile.Length);
            }
             * * /


            // Convert our 1BPP bitmap to PCX image format
            byte[] PCXData = ConvertToPCX(RealBits, / *832 * /576, iDotHeight);


            String storagePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

            String filePath = storagePath + "/ZEBRATIK.PCX";

            ///*
            using (FileStream OutFile = File.Open(filePath, FileMode.Create))
            {
                OutFile.Write(PCXData, 0, PCXData.Length);
            }


            return PCXData;
        }
*/


        /*
         * 
         *  AJW - implemented correctly in PrintManager - remove this code 
         *  

        private byte[] GetDocumentForBixolon(String issueNum, DateTime issueDate, bool localFlag)
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                // Reset the printer bws (NV images are not cleared)
                bw.Write(AsciiControlChars.Escape);
                bw.Write('@');

                RenderPrintImage(bw, issueNum, issueDate, localFlag);
                // Feed 3 vertical motion units and cut the paper with a 1 point cut
                bw.Write(AsciiControlChars.GroupSeparator);
                bw.Write('V');
                bw.Write((byte)66);
                bw.Write((byte)3);

                bw.Flush();

                return ms.ToArray();
            }
        }
         */

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
            String toastMsg = "Unable to find the printer";
            try
            {


                // AJW - TODO this should be reading the image from storage, not generating it
                // ToDo:  comment out when not needed
                //toastMsg = SaveTicketToDevice(issueNum, issueDate);
                Bitmap loTicketBitmap = SaveTicketImageToDeviceStorage(issueNum, issueDate);
                if (loTicketBitmap == null)
                {
                    toastMsg = "Ticket image not available";
                    throw new Exception(toastMsg);
                }

                BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

                if (mBluetoothAdapter == null)
                {
                    toastMsg = "No Bluetooth Adapter Available";
                }

                if (!mBluetoothAdapter.IsEnabled)
                {
                    toastMsg = "Bluetooth is not enabled.";
                }

                if (mBluetoothAdapter != null && mBluetoothAdapter.IsEnabled)
                {
                    ICollection<BluetoothDevice> pairedDevices = mBluetoothAdapter.BondedDevices;
                    if (pairedDevices.Count > 0)
                    {
                        foreach (BluetoothDevice device in pairedDevices)
                        {

                            //if (device.BluetoothClass.MajorDeviceClass != MajorDeviceClass.Imaging || !device.Name.Contains(Constants.BIXOLON_PRINTER_BT_NAME))
                            //    continue;


                            // debug - let's print to 1ts paired bluetooth printer
                            if (device.BluetoothClass.MajorDeviceClass != MajorDeviceClass.Imaging /*|| !device.Name.Contains(Constants.BIXOLON_PRINTER_BT_NAME)*/ )
                                continue;



                            // TDOD - this need to let user designate the printer type
                            bool loIsZebra = true;


                            var connection = new BluetoothPrinterConnection(device.Address);
                            connection.Open();

                            try
                            {
                                if (loIsZebra)
                                {

                                    //reset margin
                                    //ref https://km.zebra.com/kb/index?page=forums&topic=021407fb4efb3012e55595f77007e8a
                                    //connection.write("! U1 JOURNAL\r\n! U1 SETFF 100 2\r\n".getBytes());


                                    //this works perfect for me:

                                    //Connection connection = getZebraPrinterConn();
                                    //connection.open();
                                    //ZebraPrinter printer = ZebraPrinterFactory.getInstance(connection);
                                    //// this is very important which sets automatic heigth setting for label
                                    //connection.write("! U1 JOURNAL\r\n! U1 SETFF 50 2\r\n".getBytes());
                                    //printer.printImage(new ZebraImageAndroid(bitmap), 0, 0,800, 1200, false);
                                    //connection.close();
                                    //This wont waste paper and it will print upto the availability of text/data


                                    // IZebraPrinter printer = ZebraPrinterFactory.GetInstance(connection);

                                    IZebraPrinter zebraPrinter = null;

                                    try
                                    {
                                        zebraPrinter = ZebraPrinterFactory.GetInstance(connection);
                                        PrinterLanguage pl = zebraPrinter.PrinterControlLanguage;
                                        Console.WriteLine("Zebra Printer Name: " + device.Name);
                                        Console.WriteLine("Zebra Print Language: ", pl.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Zebra GetInstance Error", "Exception " + ex.Message);
                                    }


                                    //zebraTU.SendCommand("! U1 setvar \"device.languages\" \"line_print\"" + NL);
                                    //zebraTU.SendCommand("! U1 setvar \"media.tof\" \"0\"");
                                    //zebraTU.SendCommand("! U1 PAGE-WIDTH " + pagewidth + NL);
                                    //ZebraPrintText(0, 2, 18, "After this line there is a white space");
                                    //zebraTU.SendCommand("! UTILITIES" + NL + "IN-MILLIMETERS" + NL + "SETFF 0 0" + NL + "PRINT" + NL);

                                    //ZebraPrintImage("file.bmp", 0, 0, pagewidth, -1);





                                    // make sure the printer understands us!
                                    zebraPrinter.ToolsUtil.SendCommand("! U1 setvar \"device.languages\" \"zpl\"");

                                    //zebraPrinter.ToolsUtil.SendCommand("TONE 125");

                                    // AJW - TODO - get these and other print configuration commands from downloaded REGISTRY
                                    zebraPrinter.ToolsUtil.SendCommand("! U1 SPEED 3");
                                    zebraPrinter.ToolsUtil.SendCommand("! U1 setvar \"print.tone\" \"75\"");


                                    // PRE FEED COMMANDS


                                    //// PRINTER_SETFF_MAXFEED_xxxxx and PRINTER_SETFF_SKIPLENGTH_xxxxx
                                    //if ((loSetFF_MaxFeedParamInt != -1) && (loSetFF_SkipLengthParamInt != -1))
                                    //{
                                    //    PreFeedByteArrayAppendString("SETFF ");
                                    //    PreFeedByteArrayAppendString(loSetFF_MaxFeedParamStr);
                                    //    PreFeedByteArrayAppendString(" ");
                                    //    PreFeedByteArrayAppendString(loSetFF_SkipLengthParamStr);
                                    //    PreFeedByteArrayAppendString(loCRLF);
                                    //}


                                    //// PRINTER_BAR_SENSE_SENSITIVITY_xxxxx
                                    //PreFeedByteArrayAppendString("BAR-SENSE");
                                    //if (loBarSenseParamInt != -1)
                                    //{
                                    //    PreFeedByteArrayAppendString(" ");
                                    //    PreFeedByteArrayAppendString(loBarSenseParamStr);
                                    //}
                                    //PreFeedByteArrayAppendString(loCRLF);


                                    //PreFeedByteArrayAppendString("PREFEED ");
                                    //PreFeedByteArrayAppendString(loExtraLinesPreFeedStr);
                                    //PreFeedByteArrayAppendString(loCRLF);

                                    //PreFeedByteArrayAppendString("TONE ");
                                    //PreFeedByteArrayAppendString(loPrintToneStr);
                                    //PreFeedByteArrayAppendString(loCRLF);


                                    //// PRINTER_JOURNAL_MODE_xxxxx
                                    //if (loJournalModeParamInt == 1)
                                    //{
                                    //    PreFeedByteArrayAppendString("JOURNAL");
                                    //    PreFeedByteArrayAppendString(loCRLF);
                                    //}  // TODO - what is the opposite of JOURNAL mode?



                                    //// PRINTER_SPEED_xxxxx
                                    //if (loPrintSpeedParamInt != -1)
                                    //{
                                    //    PreFeedByteArrayAppendString("SPEED ");
                                    //    PreFeedByteArrayAppendString(loPrintSpeedParamStr);
                                    //    PreFeedByteArrayAppendString(loCRLF);
                                    //}

                                    // ! U1 setvar "media.speed" "2"
                                    // CPCL says value from 2 - 12 (?)


                                    // POST FEED COMMANDS

                                    //// should we try to feed to the sensor mark?
                                    //if (loUseSensorMark == true)
                                    //{
                                    //    PostFeedByteArrayAppendString("FORM");
                                    //    PostFeedByteArrayAppendString(loCRLF);
                                    //}

                                    ////PostFeedByteArrayAppendString( "POSTFEED 30" );
                                    //PostFeedByteArrayAppendString("POSTFEED ");
                                    //PostFeedByteArrayAppendString(loExtraLinesPostFeedStr);
                                    //PostFeedByteArrayAppendString(loCRLF);

                                    //PostFeedByteArrayAppendString("PRINT");
                                    //PostFeedByteArrayAppendString(loCRLF);





                                    //! U1 SPEED 3
                                    //! U1 setvar "print.tone" "0"


                                    // zebraPrinter.CurrentStatus.

                                    // use -1 -1 to width and height parameter to mantain the original width and height of the Bitmap
                                    zebraPrinter.GraphicsUtil.PrintImage(loTicketBitmap, 0, 0, -1, -1, false);
                                    //printer.GraphicsUtil.PrintImage(loTicketBitmap, 0, 0, loTicketBitmap.Width, loTicketBitmap.Height, false);



                                    /*
                                    //ZebraPrinterConnectionA

                                    byte[] printdata = GetDocumentForZebra(issueNum, issueDate, localFlag);

                                    if (printdata != null)
                                    {

                                        string zplCommand = "^XA^FO115,50^IME:LOGO.PNG^FS^XZ";
                                        byte[] zplCommandBytes = System.Text.Encoding.UTF8.GetBytes(zplCommand);  // or Encoding.ASCII.GetBytes


                                        connection.Write(zplCommandBytes);
                                        // how to flush?


                                        connection.Write(printdata);
                                        // how to flush?
                                    }
                                     */

                                }
                                else
                                {
                                    byte[] printdata = null; // GetDocumentForBixolon(issueNum, issueDate, localFlag);
                                    if (printdata != null)
                                    {
                                        connection.Write(printdata);
                                    }
                                }


                            }
                            catch (Exception e)
                            {
                                connection.Close();
                                toastMsg = "Error in sending the print data";
                                break;

                            }
                            connection.Close();
                            toastMsg = "Ticket printed.";

                            break;
                        }  // end foreach
                    }  // end if
                }
            }
            catch (Exception e)
            {
                toastMsg = "Error printing ticket";
            }

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

