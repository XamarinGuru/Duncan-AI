
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



    public class PrinterSupport_ZebraMZ320 : PrinterSupport_BaseClass
    {


            bool IsInitialized = false;

            public string LastStatusText = string.Empty;


            public PrinterSupport_ZebraMZ320(Context iApplicationContext) : base( iApplicationContext )
            {
                // base class has been called. do something original here
            }


            private void SendCommandStringToPrinter(BluetoothPrinterConnection iConnection, string iCommandString)
            {
                byte[] zplCommandBytes = System.Text.Encoding.UTF8.GetBytes(iCommandString);  // or Encoding.ASCII.GetBytes
                iConnection.Write(zplCommandBytes);
            }




        /// <summary>
        /// This is a duplicated method from TicketDetailFragment2 - this needs to moved into a universal single helper function
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


                int width = 576;
                int height = 880;

                //if (iTicketImageBitmap != null)
                //{

                //    // ZQ image needs to be flipped first
                //    width = iTicketImageBitmap.Width;
                //    height = iTicketImageBitmap.Height;


                //    // create a matrix for the manipulation
                //    Matrix matrix = new Matrix();

                //    // rotate the bitmap
                //    matrix.PostRotate(180);



                //    // create new bitmap
                //    Bitmap loTicketBitmap = Bitmap.CreateBitmap(iTicketImageBitmap, 0, 0, width, height, matrix, false);
                //}

                Bitmap loTicketBitmap = iTicketImageBitmap;




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


                            // debug - let's print to 1ts paired bluetooth printer
                            if (device.BluetoothClass.MajorDeviceClass != MajorDeviceClass.Imaging /*|| !device.Name.Contains(Constants.BIXOLON_PRINTER_BT_NAME)*/ )
                                continue;



                            // 
                            bool loIsZebra = true;
                            loFoundPairedPrinter = true;


                            BluetoothPrinterConnection connection = new BluetoothPrinterConnection(device.Address);
                            connection.Open();

                            try
                            {
                                if (loIsZebra)
                                {

                                    //reset margin
                                    //ref https://km.zebra.com/kb/index?page=forums&topic=021407fb4efb3012e55595f77007e8a
                                    //connection.write("! U1 JOURNAL\r\n! U1 SETFF 100 2\r\n".getBytes());



#if _use_zpl_

                                    // IZebraPrinter printer = ZebraPrinterFactory.GetInstance(connection);

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
                                        Console.WriteLine("Zebra GetInstance Error", "Exception " + ex.Message);
                                    }


                                    //zebraPrinter.FormatUtil.

                                    // make sure the printer understands us! - must be lower case
                                    // ZPL must be in lower case in order to be accepted by the printer.
                                    // The printer ZQ510 by default is set up in "line_print".
                                    zebraPrinter.ToolsUtil.SendCommand("! U1 setvar \"device.languages\" \"zpl\"");

                                    //zebraPrinter.ToolsUtil.SendCommand("! U1 setvar \"device.languages\" \"hybrid_xml_zpl\"");



                                    //If you are using black bar labels, you need to send this SGD commands to the printer:
                                    //zebraPrinter.ToolsUtil.SendCommand("! U1 setvar \"ezpl.media_type\" \"mark\"");


                                    // AJW - TODO - get these and other print configuration commands from downloaded REGISTRY
                                    zebraPrinter.ToolsUtil.SendCommand("! U1 SPEED 3");
                                    zebraPrinter.ToolsUtil.SendCommand("! U1 setvar \"print.tone\" \"150\"");


                                    //string loTone = SGD.GET("print.tone", connection);
                                    //if (loTone.Length > 1)
                                    //{
                                    //    SGD.SET("print.tone", 175, connection);

                                    //    string loTone2 = SGD.GET("print.tone", connection);
                                    //    if (loTone2.Length > 1)
                                    //    {
                                    //        zebraPrinter.ToolsUtil.SendCommand("! U1 setvar \"print.tone\" \"150\"");
                                    //    }

                                    //}


                                    // if these are included, we get nothing out of the printer
                                    //zebraPrinter.ToolsUtil.SendCommand("! U1 setvar \"media.draft_mode\" \"off\"");
                                    //zebraPrinter.ToolsUtil.SendCommand("! U1 setvar \"media.draft_mode\" \"disabled\"");


                                    //zebraPrinter.ToolsUtil.SendCommand("! U1 SPEED 1");


                                    // this is a CPCL command
                                    //zebraPrinter.ToolsUtil.SendCommand("! U1 setvar \"print.tone\" \"100\"");
                                    //zebraPrinter.ToolsUtil.SendCommand("! U1 setvar \"print.tone\" \"150\"");
#endif


#if _use_zpl_


                                    // so if we're in ZPL, we need to use the ZPL language
                                    //zebraPrinter.ToolsUtil.SendCommand("^MD0");  // MediaDarkness ^MDa , is added to SDs -30 to 30 in .1 steps
                                    //zebraPrinter.ToolsUtil.SendCommand("~SD29.2");  // SetDarkness  ~SD## , MD is added to this 0-30 in .1 steps

                                    //zebraPrinter.ToolsUtil.SendCommand("^PR5,5,5");  // PrintRate print speed (1-14), slew speed (2-14), backfeed speed (2-14)

                                    // MZ-320 just echos this...

#endif


#if _use_cpcl_direct_
                                    // specify how long the label is, with a fudge factor to account for the DPI difference of 200 vs 203.1
                                    int loMaxLabelHeightInt = (int)((float)height * (float)1.05);





                                    string loLabelCommandStr = "! 0 200 200 " + loMaxLabelHeightInt.ToString() + " 1\r\n";
                                    SendCommandStringToPrinter(connection, loLabelCommandStr);


                                    SendCommandStringToPrinter(connection, "SETFF 1200 35\r\n");
                                    SendCommandStringToPrinter(connection, "BAR-SENSE 70\r\n");
                                    SendCommandStringToPrinter(connection, "PREFEED 0\r\n");
                                    SendCommandStringToPrinter(connection, "TONE 75\r\n");
                                    SendCommandStringToPrinter(connection, "JOURNAL\r\n");
                                    SendCommandStringToPrinter(connection, "SPEED 4\r\n");


                                    SendCommandStringToPrinter(connection, "PCX 0 0");
                                    connection.Write(iTicketImageAsByteArray);


                                    SendCommandStringToPrinter(connection, "FORM\r\n");
                                    SendCommandStringToPrinter(connection, "POSTFEED 5\r\n");
                                    SendCommandStringToPrinter(connection, "PRINT\r\n");



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

#if _use_zebra_pcl_
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



#if _use_sgd_                                    
                                    zebraPrinter.ToolsUtil.SendCommand("! UTILITIES\r\nIN-MILLIMETERS\r\nSETFF 10 2\r\nPRINT\r\n");

                                    string modelName = SGD.GET("device.product_name", connection);
                                    if (modelName.Length > 400)
                                    {
                                        // dummy
                                        zebraPrinter.GraphicsUtil.PrintImage(loTicketBitmap, 0, 0, -1, -1, false);
                                    }

#endif

#if _use_zpl_

                                    // use -1 -1 to width and height parameter to mantain the original width and height of the Bitmap
                                    zebraPrinter.GraphicsUtil.PrintImage(loTicketBitmap, 0, 0, -1, -1, false);

                                    //zebraPrinter.GraphicsUtil.PrintImage(loTicketBitmap, 0, 0, loTicketBitmap.Width, loTicketBitmap.Height, false);

                                    //zebraPrinter.GraphicsUtil.PrintImage( new Z ZebraImageAndroid( loTicketBitmap), 0, 0, loTicketBitmap.Width, loTicketBitmap.Height, false);
#endif


#if _use_zebra_pcl_
                                    zebraPrinter.ToolsUtil.SendCommand("FORM\r\n");
                                    zebraPrinter.ToolsUtil.SendCommand("POSTFEED 5\r\n");
                                    zebraPrinter.ToolsUtil.SendCommand("PRINT\r\n");  
#endif




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
                                    //byte[] printdata = null; // GetDocumentForBixolon(issueNum, issueDate, localFlag);
                                    //if (printdata != null)
                                    //{
                                    //    connection.Write(printdata);
                                    //}
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