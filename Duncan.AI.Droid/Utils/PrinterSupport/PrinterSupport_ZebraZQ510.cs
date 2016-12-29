
#define _use_zpl_    

//#define _use_cpcl_direct_


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


using Android.Graphics;
using Android.Views.Animations;
using Android.Animation;
using System.IO;
//using Duncan.AI.Droid.Common;
using Android.Util;
using Android.Preferences;
using System.Threading.Tasks;
using Duncan.AI.Droid.Utils.HelperManagers;

using Reino.ClientConfig;

// AJW added for immediate printing - temp until revamped
using Android.Bluetooth;
using Com.Zebra.Android.Printer;
using Com.Zebra.Android.Comm;

using Com.Zebra.Android.Graphics;
using Com.Zebra.Android.Util;

using Com.Zebra.Android.Sgd;



using System.Threading;





namespace Duncan.AI.Droid.Utils.PrinterSupport
{



    public class PrinterSupport_ZebraZQ510 : PrinterSupport_BaseClass
    {


        bool IsInitialized = false;

        public string LastStatusText = string.Empty;


        public PrinterSupport_ZebraZQ510(Context iApplicationContext)
            : base(iApplicationContext)
        {
            // base class has been called. do something original here
        }


        private void SendCommandStringToPrinter(BluetoothPrinterConnection iConnection, string iCommandString)
        {
            // this is ContextThemeWrapper correct encoding - but every command MUST be terminated with \n or else it is buffered
            // and prepended to the next command until a \n is recieved, and then they are all misunderstood and ignored by printer
            byte[] zplCommandBytes = System.Text.Encoding.ASCII.GetBytes(iCommandString);
            iConnection.Write(zplCommandBytes);
        }




        /// <summary>
        //
        /// </summary>
        /// <param name="issueNum"></param>
        /// <param name="issueDate"></param>
        /// <param name="localFlag"></param>
        public void SendPrintDirect(Bitmap iTicketImageBitmap, byte[] iTicketImageAsByteArray, bool useLooper, Context iContext)
        {



            // need to have our own
            //Looper.Prepare();
            //ProgressDialog loProgressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Printing Ticket...", true);

            //String toastMsg = "Unable to find the printer";
            toastMsg = "Printing Complete";
            try
            {

                if ((iTicketImageBitmap == null) && (iTicketImageAsByteArray == null))
                {
                    toastMsg = "Ticket image not available";
                    throw new Exception(toastMsg);
                }



                #region Bitmap Prep

                int width = 576;
                int height = 880;
                Bitmap loTicketBitmap = null;

                if (iTicketImageBitmap != null)
                {

                    // ZQ image needs to be flipped first
                    width = iTicketImageBitmap.Width;
                    height = iTicketImageBitmap.Height;


                    // create a matrix for the manipulation
                    Matrix matrix = new Matrix();

                    // rotate the bitmap
                    matrix.PostRotate(180);

                    // create new bitmap
                    loTicketBitmap = Bitmap.CreateBitmap(iTicketImageBitmap, 0, 0, width, height, matrix, false);

                    // recycle the old one
                    iTicketImageBitmap.Recycle();
                    iTicketImageBitmap.Dispose();
                    iTicketImageBitmap = null;

                }

                #endregion



                #region Get Registry Printer Control Commands

                // extra info about prefeed and density
                int loExtraLinesPreFeedInt = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_SYSTEM, TTRegistry.regEXTRA_DOT_LINES_ZQ510_PREFEED, TTRegistry.regEXTRA_DOT_LINES_ZQ510_PREFEED_DEFAULT);
                int loExtraLinesPostFeedInt = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_SYSTEM, TTRegistry.regEXTRA_DOT_LINES_ZQ510_POSTFEED, TTRegistry.regEXTRA_DOT_LINES_ZQ510_POSTFEED_DEFAULT);
                int loPrintToneInt = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_SYSTEM, TTRegistry.regPRINT_TONE_ZQ510, TTRegistry.regPRINT_TONE_ZQ510_DEFAULT);

                // more custom settings 
                int loSetFF_MaxFeedParamInt = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_SYSTEM, TTRegistry.regPRINTER_SETFF_MAXFEED_ZQ510, TTRegistry.regPRINTER_SETFF_MAXFEED_ZQ510_DEFAULT);
                int loSetFF_SkipLengthParamInt = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_SYSTEM, TTRegistry.regPRINTER_SETFF_SKIPLENGTH_ZQ510, TTRegistry.regPRINTER_SETFF_SKIPLENGTH_ZQ510_DEFAULT);
                int loBarSenseParamInt = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_SYSTEM, TTRegistry.regPRINTER_BAR_SENSE_SENSITIVITY_ZQ510, TTRegistry.regPRINTER_BAR_SENSE_SENSITIVITY_ZQ510_DEFAULT);
                int loJournalModeParamInt = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_SYSTEM, TTRegistry.regPRINTER_JOURNAL_MODE_ZQ510, TTRegistry.regPRINTER_JOURNAL_MODE_ZQ510_DEFAULT);
                int loPrintSpeedParamInt = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_SYSTEM, TTRegistry.regPRINTER_SPEED_ZQ510, TTRegistry.regPRINTER_SPEED_ZQ510_DEFAULT);

                int loIgnorePaperSenseRegValue = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_SYSTEM, TTRegistry.regPRINTER_BAR_SENSE_IGNORE_ZQ510, TTRegistry.regPRINTER_BAR_SENSE_IGNORE_ZQ510_DEFAULT);
                // we'll use the mark unless this is set to 1
                bool loUseSensorMark = !(loIgnorePaperSenseRegValue == 1);

                #endregion




                BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

                if (mBluetoothAdapter == null)
                {
                    toastMsg = "No Bluetooth Adapter Available";
                }

                if (!mBluetoothAdapter.IsEnabled)
                {
                    toastMsg = "Bluetooth is not enabled.";
                }

                bool loFoundPairedPrinter = false;

                if (mBluetoothAdapter != null && mBluetoothAdapter.IsEnabled)
                {
                    ICollection<BluetoothDevice> pairedDevices = mBluetoothAdapter.BondedDevices;
                    if (pairedDevices.Count > 0)
                    {
                        foreach (BluetoothDevice device in pairedDevices)
                        {

                            //if (device.BluetoothClass.MajorDeviceClass != MajorDeviceClass.Imaging || !device.Name.Contains(Constants.BIXOLON_PRINTER_BT_NAME))
                            //    continue;


                            // debug - let's print to 1ts paired bluetooth printer - how do we find the signature of our ZQ=510?
                            if (device.BluetoothClass.MajorDeviceClass != MajorDeviceClass.Imaging /*|| !device.Name.Contains(Constants.BIXOLON_PRINTER_BT_NAME)*/ )
                                continue;

                            // we got one!
                            loFoundPairedPrinter = true;



                            BluetoothPrinterConnection connection = new BluetoothPrinterConnection(device.Address);
                            connection.Open();

                            try
                            {

                                IZebraPrinter zebraPrinter = null;

                                try
                                {
                                    zebraPrinter = ZebraPrinterFactory.GetInstance(connection);
                                    //PrinterLanguage pl = zebraPrinter.PrinterControlLanguage;
                                    //Console.WriteLine("Zebra Printer Name: " + device.Name);
                                    //Console.WriteLine("Zebra Print Language: ", pl.ToString());
                                }
                                catch (Exception ex)
                                {
                                    toastMsg = "Zebra SDK Error: " + ex.Message;
                                    Console.WriteLine(toastMsg);
                                    zebraPrinter = null;
                                }



                                if (zebraPrinter != null)
                                {

                                    /// PRE_IMAGE commands to set up before printing  ////


                                    // make sure the printer understands us! - must be lower case
                                    // ZPL must be in lower case in order to be accepted by the printer.
                                    // The printer ZQ510 by default is set up in "line_print".
                                    zebraPrinter.ToolsUtil.SendCommand("! U1 setvar \"device.languages\" \"zpl\"" + "\r\n");

                                    zebraPrinter.ToolsUtil.SendCommand("! U1 setvar \"media.tof\" \"0\"" + "\r\n");


                                    int loMaxLabelHeightInt = (int)((float)height * (float)1.05);
                                    //string loLabelHeightCommandStr = "! U1 setvar \"zpl.label_length\" \"153\"" + "\n";
                                    string loLabelHeightCommandStr = "! U1 setvar \"zpl.label_length\" " + "\"" + loMaxLabelHeightInt.ToString() + "\"" + "\n";
                                    // set label length to calculated height
                                    zebraPrinter.ToolsUtil.SendCommand(loLabelHeightCommandStr);



                                    //If you are using black bar labels, you need to send this SGD commands to the printer:
                                    zebraPrinter.ToolsUtil.SendCommand("! U1 setvar \"ezpl.media_type\" \"mark\"" + "\n");


                                    //The root of the problem is that the PrintImage() method appends a FORM PRINT command to 
                                    // the image being printed. It isn�t possible, given the options available in the SDK, to easily stop this command being added. 
                                    //However, if before your print command you add the CPCL command to set the form feed length then you can mitigate the problem.
                                    // For instance:   
                                    //zebraPrinter.ToolsUtil.SendCommand("! U1 JOURNAL\r\n! U1 SETFF 50 2\r\n");
                                    zebraPrinter.ToolsUtil.SendCommand("! U1 JOURNAL\r\n! U1 SETFF 10 1\r\n");

                                    // feed some before printing
                                    //zebraPrinter.ToolsUtil.SendCommand("! U! PREFEED 250\r\n");

                                    // TODO - translate the TONE commands from 0-100 into 1-30 for new PCL
                                    zebraPrinter.ToolsUtil.SendCommand("^MD0");  // MediaDarkness ^MDa , is added to SDs -30 to 30 in .1 steps
                                    //zebraPrinter.ToolsUtil.SendCommand("~SD19.2");  // SetDarkness  ~SD## , MD is added to this 0-30 in .1 steps
                                    zebraPrinter.ToolsUtil.SendCommand("~SD28.1");  // SetDarkness  ~SD## , MD is added to this 0-30 in .1 steps

                                    zebraPrinter.ToolsUtil.SendCommand("^PR5,5,5");  // PrintRate print speed (1-14), slew speed (2-14), backfeed speed (2-14)



                                    // setup commands sent prior to the image
                                    List<string> loPreFeedCommands = new List<string>();
                                    if (( loSetFF_MaxFeedParamInt != -1 ) && ( loSetFF_SkipLengthParamInt != -1))
                                    {
                                        loPreFeedCommands.Add("! U1 SETFF " + loSetFF_MaxFeedParamInt.ToString() + " " + loSetFF_SkipLengthParamInt.ToString() + "\r\n");
                                    }

                                    if ( loBarSenseParamInt != -1 )
                                    {
                                       loPreFeedCommands.Add( "! U1 BAR-SENSE " + loBarSenseParamInt.ToString());
                                    }
                                    else
                                    {
                                      loPreFeedCommands.Add( "! U1 BAR-SENSE" + "\r\n");
                                    }


                                    loPreFeedCommands.Add("! U1 PREFEED " + loExtraLinesPreFeedInt.ToString() + " \r\n" );
                                    loPreFeedCommands.Add("! U1 TONE  " + loPrintToneInt.ToString() + "\r\n");

                                    if (loPrintSpeedParamInt != -1)
                                    {
                                        loPreFeedCommands.Add("! U1 SPEED  " + loPrintSpeedParamInt.ToString() + "\r\n");
                                    }






                                    // final commands, sent after the image
                                    List<string> loPostFeedCommands = new List<string>();

                                    //// back to label mode, then feed to sensor
                                    loPostFeedCommands.Add("! U1 LABEL" + "\r\n");
                                    loPostFeedCommands.Add("! U1 BAR-SENSE" + "\r\n");

                                    if ((loSetFF_MaxFeedParamInt != -1) && (loSetFF_SkipLengthParamInt != -1))
                                    {
                                        loPostFeedCommands.Add("! U1 SETFF " + loSetFF_MaxFeedParamInt.ToString() + " " + loSetFF_SkipLengthParamInt.ToString() + "\r\n");
                                    }
                                    else
                                    {
                                        // default = no more than 1200 until barsense, and advance 35 past it
                                        loPostFeedCommands.Add("! U1 SETFF 1200 35" + "\r\n");
                                    }

                                    if (loUseSensorMark == true)
                                    {
                                        loPostFeedCommands.Add("! U1 FORM" + "\r\n");
                                    }


                                    if (loExtraLinesPostFeedInt != -1)
                                    {
                                        loPostFeedCommands.Add("! U1 POSTFEED " + loExtraLinesPostFeedInt.ToString() + "\r\n");
                                    }

                                    loPostFeedCommands.Add("! U1 PRINT" + "\r\n");



                                    // PRE-IMAGE SETUP COMMANDS 


                                    // TODO - test if optimizing speed by sending them all one command set


                                    // extra feed after top of form?
                                    if (loExtraLinesPreFeedInt > 0)
                                    {
                                        // use the form feed command to advance the paper specified amount before printing the image
                                        //  no more than N until barsense, and advance X past it
                                        //zebraPrinter.ToolsUtil.SendCommand("! U1 SETFF 200 1\r\n");
                                        zebraPrinter.ToolsUtil.SendCommand("! U1 SETFF " + loExtraLinesPreFeedInt.ToString() + " 1" + "\r\n");
                                        zebraPrinter.ToolsUtil.SendCommand("! U1 FORM\r\n");
                                        zebraPrinter.ToolsUtil.SendCommand("! U1 PRINT\r\n");
                                    }
                                    

                                    // send them all together....  maybe too long someday?
                                    //zebraPrinter.ToolsUtil.SendCommand(loPreFeedCommands.ToString());    

                                    /////   now to send the ticket image ///////
                                    // use -1 -1 to width and height parameter to mantain the original width and height of the Bitmap
                                    zebraPrinter.GraphicsUtil.PrintImage(loTicketBitmap, 0, 0, -1, -1, false);

                                    //// POST IMAGE commands to wrap it up
                                    // send them all together....  maybe too long someday?
                                    //zebraPrinter.ToolsUtil.SendCommand(loPostFeedCommands.ToString());



                                    //// back to label mode, then feed to sensor
                                    zebraPrinter.ToolsUtil.SendCommand("! U1 LABEL\r\n");
                                    zebraPrinter.ToolsUtil.SendCommand("! U1 BAR-SENSE\r\n");


                                    // extra feed after top of form?
                                    if (loExtraLinesPostFeedInt > 0)
                                    {
                                        // use the form feed command to advance the paper specified amount after printing the image
                                        //  no more than N until barsense, and advance X past it
                                        zebraPrinter.ToolsUtil.SendCommand("! U1 SETFF 1200 " + loExtraLinesPostFeedInt.ToString() + "\r\n");
                                    }
                                    else
                                    {
                                        // no more than 1200 until barsense, and advance 35 past it
                                        //zebraPrinter.ToolsUtil.SendCommand("! U1 SETFF 1200 35\r\n");
                                        zebraPrinter.ToolsUtil.SendCommand("! U1 SETFF 1200 1\r\n");
                                    }






                                    zebraPrinter.ToolsUtil.SendCommand("! U1 FORM\r\n");

                                    zebraPrinter.ToolsUtil.SendCommand("! U1 PRINT\r\n");




                                    // extra feed after top of form?
                                    //if (loExtraLinesPostFeedInt > 0)
                                    //{
                                    //    // use the form feed command to advance the paper specified amount after printing the image
                                    //    //  no more than N until barsense, and advance X past it
                                    //    //zebraPrinter.ToolsUtil.SendCommand("! U1 SETFF 200 1\r\n");
                                    //    zebraPrinter.ToolsUtil.SendCommand("! U1 SETFF " + loExtraLinesPostFeedInt.ToString() + " 1" + "\r\n");
                                    //    zebraPrinter.ToolsUtil.SendCommand("! U1 FORM\r\n");
                                    //    zebraPrinter.ToolsUtil.SendCommand("! U1 PRINT\r\n");
                                    //}




#if _windows_mobile_reference_
		                            // specify how long the label is, with a fudge factor to account for the DPI difference of 200 vs 203.1
		                            int loMaxLabelHeightInt = (int)( (float)loTicketBitmap.Height * (float)1.05 );

                                  

                                    string loLabelCommandStr = "! 0 200 200 " + loMaxLabelHeightInt.ToString() + " 1\r\n";
                                    zebraPrinter.ToolsUtil.SendCommand(loLabelCommandStr);


                                    zebraPrinter.ToolsUtil.SendCommand("SETFF 1200 35\r\n");
                                    zebraPrinter.ToolsUtil.SendCommand("BAR-SENSE 70\r\n");
                                    zebraPrinter.ToolsUtil.SendCommand("PREFEED 0\r\n");
                                    zebraPrinter.ToolsUtil.SendCommand("TONE 75\r\n");
                                    zebraPrinter.ToolsUtil.SendCommand("JOURNAL\r\n");
                                    zebraPrinter.ToolsUtil.SendCommand("SPEED 4\r\n");
                                    //zebraPrinter.ToolsUtil.SendCommand("PCX 0 0");



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

                                    //zebraPrinter.ToolsUtil.SendCommand("! U1 JOURNAL\r\n! U1 SETFF 100 2\r\n");
#endif
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


                    if (loFoundPairedPrinter == false)
                    {
                        toastMsg = "Paired Printer not found.";
                    }

                }
            }
            catch (Exception e)
            {
                toastMsg = "Error printing ticket";
            }

            try
            {
                //Activity.RunOnUiThread(() => _progressDialog.Hide());
                //Activity.RunOnUiThread(() => Toast.MakeText(this.Activity, toastMsg, ToastLength.Long).Show());
                //loProgressDialog.Hide(); // already on UI thread?
                //Toast.MakeText(this.Activity, toastMsg, ToastLength.Long).Show();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception source: {0}", e.Source);
            }

        }


    }
}