

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Reino.ClientConfig;

namespace Duncan.AI.Droid.Utils.PrinterSupport
{




    public class PrinterSupport_BaseClass
    {
        protected Context _ApplicationContext;

        // updated internally by descendant class instances
        protected static string toastMsg = string.Empty;


        public static string GetLastPrintJobResultMessage()
        {
            return toastMsg;
        }

        public PrinterSupport_BaseClass(Context iApplicationContext)
        {
            _ApplicationContext = iApplicationContext;
        }


        public Bitmap toGrayscale(Bitmap bmpOriginal)
        {
            int width, height;
            height = bmpOriginal.Height;
            width = bmpOriginal.Width;

            //Bitmap bmpGrayscale = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888);
            Bitmap bmpGrayscale = Bitmap.CreateBitmap(width, height, bmpOriginal.GetConfig());


            Canvas c = new Canvas(bmpGrayscale);
            Paint paint = new Paint();
            ColorMatrix cm = new ColorMatrix();
            cm.SetSaturation(0);
            ColorMatrixColorFilter f = new ColorMatrixColorFilter(cm);
            paint.SetColorFilter(f);
            c.DrawBitmap(bmpOriginal, 0, 0, paint);
            return bmpGrayscale;
        }





        //public boolean printImage(final Bitmap bitmap, final String fileName, int x, int y, int width, int height, PrinterLanguage language,   
        //  PrintListener listener) { //in this case PrinterLanguage.ZPL was passed into the method  
        //  if (connection != null) {  
        //  if (connection.isConnected()) {  
        //  try {  
        //       com.zebra.sdk.printer.ZebraPrinter printer = ZebraPrinterFactory.getInstance(language, connection);  //used to be ZebraPrinterFactory.getInstance(connection) for RW420  
        //       printer.printImage(new ZebraImageAndroid(bitmap), x, y, width, height, false);  
        //       listener.onPrintSuccess();  
        //       return true;  

        //  } catch (ConnectionException e) {  
        //       listener.onPrintFail();  
        //       e.printStackTrace();  
        //       try {  
        //            connection.close();  
        //       } catch (ConnectionException e1) {  
        //       e1.printStackTrace();  
        //       }  
        //  }  finally {  
        //       if (bitmap != null && !bitmap.isRecycled()) {  
        //       bitmap.recycle();  
        //       System.gc();  
        //       }  
        //  }  
        //          } else {  
        //  listener.onPrintFail();  
        //          }  
        //  }  
        //  return false;  
        //}  




        //public static byte[] ImageToByte(Image img)
        //{
        //    ImageConverter converter = new ImageConverter();
        //    return (byte[])converter.ConvertTo(img, typeof(byte[]));
        //}


        //public static byte[] ImageToByte2(Image img)
        //{
        //    byte[] byteArray = new byte[0];
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        //        stream.Close();

        //        byteArray = stream.ToArray();
        //    }
        //    return byteArray;
        //}

        //        public static Bitmap createBlackAndWhite(Bitmap src) 
        //        {
        //    int width = src.Width;
        //    int height = src.Height;

        //    Bitmap bmOut = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);

        //    float factor = 255f;
        //    float redBri = 0.2126f;
        //    float greenBri = 0.2126f;
        //    float blueBri = 0.0722f;

        //    int length = width * height;
        //    int[] inpixels = new int[length];
        //    int[] oupixels = new int[length];

        //    src.GetPixels(inpixels, 0, width, 0, 0, width, height);

        //    int point = 0;
        //            long loPixelLen = inpixels.GetLongLength(0);
        //            for (int pix = 0; pix < loPixelLen; loPixelLen++)
        //            //for(int pix: inpixels)
        //            {
        //                int R = (pix >> 16) & 0xFF;
        //                int G = (pix >> 8) & 0xFF;
        //                int B = pix & 0xFF;

        //                float lum = (redBri * R / factor) + (greenBri * G / factor) + (blueBri * B / factor);

        //                if (lum > 0.4)
        //                {
        //                    oupixels[point] = 0xFFFFFFFF;
        //                }
        //                else
        //                {
        //                    oupixels[point] = 0xFF000000;
        //                }
        //                point++;
        //            }
        //    bmOut.SetPixels(oupixels, 0, width, 0, 0, width, height);
        //    return bmOut;
        //}
        public static List<string> GetListOfSupportedPrinters()
        {
            List<string> oListOfSupportedPrinters = new List<string>();
            oListOfSupportedPrinters.Add(Constants.PRINTER_TYPE_NAME_ZEBRA_MZ320);
            oListOfSupportedPrinters.Add(Constants.PRINTER_TYPE_NAME_ZEBRA_iMZ320);
            oListOfSupportedPrinters.Add(Constants.PRINTER_TYPE_NAME_ZEBRA_ZQ510);
            oListOfSupportedPrinters.Add(Constants.PRINTER_TYPE_NAME_BIXOLON_SPP200);
            oListOfSupportedPrinters.Add(Constants.PRINTER_TYPE_NAME_N5CLASS_PRINTER_TEXT);
            oListOfSupportedPrinters.Add(Constants.PRINTER_TYPE_NAME_N5CLASS_PRINTER_GRAPHIC);

            return oListOfSupportedPrinters;

        }

        public static PrintersSupported GetPrinterEnumForPrinterNameString(string iPrinterNameString)
        {


            if (string.IsNullOrEmpty(iPrinterNameString) == true)
            {
                return PrintersSupported.Printer_ZebraMZ320;
            }

            switch (iPrinterNameString)
            {

                case Constants.PRINTER_TYPE_NAME_N5CLASS_PRINTER_TEXT:                
                    {
                        return PrintersSupported.Printer_TwoTechnologiesN5Class_Text;
                        break;
                    }                
                case Constants.PRINTER_TYPE_NAME_N5CLASS_PRINTER_GRAPHIC:
                    {
                        return PrintersSupported.Printer_TwoTechnologiesN5Class_Graphic;
                        break;
                    }
                case Constants.PRINTER_TYPE_NAME_ZEBRA_MZ320:
                    {
                        return PrintersSupported.Printer_ZebraMZ320;
                        break;
                    }

                case Constants.PRINTER_TYPE_NAME_ZEBRA_iMZ320:
                    {
                        return PrintersSupported.Printer_ZebraiMZ30;
                        break;
                    }

                case Constants.PRINTER_TYPE_NAME_ZEBRA_ZQ510:
                    {
                        return PrintersSupported.Printer_ZebraZQ510;
                        break;
                    }

                case Constants.PRINTER_TYPE_NAME_BIXOLON_SPP200:
                    {
                        return PrintersSupported.Printer_BixolonSPP200;
                        break;
                    }
                default:
                    {
                        return PrintersSupported.Printer_ZebraMZ320;
                    }
            }

        }



        public static void ReprintTicketToCurrentlySelectedPrinter(string iTicketImageBitmapFileNameOnly, string iTicketPCLTextCommandFileNameOnly, ref string ioResultMsg)
        {

            bool loPrintSuccess = false;
            Bitmap loTicketBitmap = null;
            bool loTicketPCLFileExists = false;

            try
            {
                // add paths to the passed filenames if they are not empty
                string loImageFileNameAbsolutePath = string.Empty;
                if (iTicketImageBitmapFileNameOnly.Length > 0)
                {
                    loImageFileNameAbsolutePath = System.IO.Path.Combine(Helper.GetMultimediaFolder(), iTicketImageBitmapFileNameOnly);
                }

                string loPCLTextCommandFileNameAbsolutePath = string.Empty;
                if (iTicketPCLTextCommandFileNameOnly.Length > 0)
                {
                    loPCLTextCommandFileNameAbsolutePath = System.IO.Path.Combine(Helper.GetMultimediaFolder(), iTicketPCLTextCommandFileNameOnly);
                }
                


                // even if we are PCL only, we will still try to get the image
                try
                {
                    loTicketBitmap = BitmapFactory.DecodeFile(loImageFileNameAbsolutePath);
                }
                catch (Exception exp)
                {
                    //
                    loTicketBitmap = null;
                    Console.WriteLine("Error reading ticket image file: " + loImageFileNameAbsolutePath + " " + exp.Message);
                }


                loTicketPCLFileExists = File.Exists(loPCLTextCommandFileNameAbsolutePath);


                // need one or the other
                if ((loTicketBitmap == null) && (loTicketPCLFileExists == false))
                {
                    toastMsg = "Ticket image not available";
                    ioResultMsg = GetLastPrintJobResultMessage();
                    return;
                }

                switch (DroidContext.gPrinterSelected)
                {
                    case PrintersSupported.Printer_TwoTechnologiesN5Class_Text:
                        {


                            try
                            {
                                PrinterSupport_TwoTechN5Printer myN5Printer = new PrinterSupport_TwoTechN5Printer(DroidContext.ApplicationContext, Constants.TWOTECH_N5PRINTER_MODE_T5_TEXT);                       
                                // print direct text using PCL
                                myN5Printer.LaunchN5PrintServiceForAI(iTicketPCLTextCommandFileNameOnly);
                                loPrintSuccess = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception source: {0}", e.Source);
                                toastMsg = "Error Printing Ticket";
                            }
                            break;
                        }

                    case PrintersSupported.Printer_TwoTechnologiesN5Class_Graphic:
                        {
                            try
                            {
                                PrinterSupport_TwoTechN5Printer myN5Printer = new PrinterSupport_TwoTechN5Printer(DroidContext.ApplicationContext, Constants.TWOTECH_N5PRINTER_MODE_T5_GRAPHIC);

                                // print direct text using PCL
                                myN5Printer.LaunchN5PrintServiceForAI(iTicketImageBitmapFileNameOnly);
                                loPrintSuccess = true;

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception source: {0}", e.Source);
                                toastMsg = "Error Printing Ticket";
                            }
                            break;
                        }

                    case PrintersSupported.Printer_ZebraZQ510:
                        {
                            try
                            {
                                PrinterSupport_ZebraZQ510 myZQPrinter = new PrinterSupport_ZebraZQ510(DroidContext.ApplicationContext);

                                myZQPrinter.SendPrintDirect(loTicketBitmap, null, true, DroidContext.ApplicationContext);

                                loPrintSuccess = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception source: {0}", e.Source);
                            }

                            //return;
                            break;
                        }

                    case PrintersSupported.Printer_ZebraiMZ30:
                        {
                            try
                            {
                                PrinterSupport_ZebraiMZ320 myMZPrinter = new PrinterSupport_ZebraiMZ320(DroidContext.ApplicationContext);

                                myMZPrinter.SendPrintDirect(loTicketBitmap, null, true, DroidContext.ApplicationContext);

                                loPrintSuccess = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception source: {0}", e.Source);
                            }

                            //return;
                            break;

                        }

                    case PrintersSupported.Printer_ZebraMZ320:
                        {
                            try
                            {
                                PrinterSupport_ZebraMZ320 myMZPrinter = new PrinterSupport_ZebraMZ320(DroidContext.ApplicationContext);

                                myMZPrinter.SendPrintDirect(loTicketBitmap, null, true, DroidContext.ApplicationContext);

                                loPrintSuccess = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception source: {0}", e.Source);
                            }

                            //return;
                            break;

                        }

                    default:
                        {

                            toastMsg = "Unsupported printer selected.";
                            Console.WriteLine("Unsupported printer selected = {0}", DroidContext.gPrinterSelected.ToString());

                            //return;
                            break;
                        }
                }

                if (toastMsg.Length > 0)
                {
                    Console.WriteLine("Print Result: {0}", toastMsg);
                }


            }
            catch (Exception exp)
            {
                ////
            }


            try
            {
                if (loTicketBitmap != null)
                {
                    if (loTicketBitmap.IsRecycled == false)
                    {
                        loTicketBitmap.Recycle();
                        loTicketBitmap.Dispose();
                    }
                }
            }
            catch (Exception exp)
            {
                ////
            }

            ioResultMsg = GetLastPrintJobResultMessage();
        }

    


        public static void SendImageToCurrentlySelectedPrinter(Bitmap iTicketImageBitmap, List<PCLPrintingClass.PCLStringRow> iAllStringsInCurrentTicket, ref string ioResultMsg)
        {

            try
            {
                bool loPrintSuccess = false;

                switch (DroidContext.gPrinterSelected)
                {
                    case PrintersSupported.Printer_TwoTechnologiesN5Class_Text:
                        {


                            try
                            {
                                PrinterSupport_TwoTechN5Printer myN5Printer = new PrinterSupport_TwoTechN5Printer(DroidContext.ApplicationContext, Constants.TWOTECH_N5PRINTER_MODE_T5_TEXT);
                                
                                if (iAllStringsInCurrentTicket != null)
                                {
                                        // print direct text using PCL
                                        myN5Printer.SendPrintDirectPCL(iAllStringsInCurrentTicket, true, DroidContext.ApplicationContext);                                    
                                }

                                loPrintSuccess = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception source: {0}", e.Source);
                            }


                            break;
                        }


                    case PrintersSupported.Printer_TwoTechnologiesN5Class_Graphic:
                        {
                            try
                            {
                                PrinterSupport_TwoTechN5Printer myN5Printer = new PrinterSupport_TwoTechN5Printer(DroidContext.ApplicationContext, Constants.TWOTECH_N5PRINTER_MODE_T5_GRAPHIC);                                                                    
                                // Print ticket image
                                myN5Printer.SendPrintDirect(iTicketImageBitmap, true, DroidContext.ApplicationContext);                                                                    
                                loPrintSuccess = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception source: {0}", e.Source);
                            }


                            break;
                        }

                    case PrintersSupported.Printer_ZebraZQ510:
                        {
                            try
                            {
                                PrinterSupport_ZebraZQ510 myZQPrinter = new PrinterSupport_ZebraZQ510(DroidContext.ApplicationContext);

                                myZQPrinter.SendPrintDirect(iTicketImageBitmap, null, true, DroidContext.ApplicationContext);

                                loPrintSuccess = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception source: {0}", e.Source);
                            }

                            //return;
                            break;
                        }

                    case PrintersSupported.Printer_ZebraiMZ30:
                        {
                            try
                            {
                                PrinterSupport_ZebraiMZ320 myMZPrinter = new PrinterSupport_ZebraiMZ320(DroidContext.ApplicationContext);

                                myMZPrinter.SendPrintDirect(iTicketImageBitmap, null, true, DroidContext.ApplicationContext);

                                loPrintSuccess = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception source: {0}", e.Source);
                            }

                            //return;
                            break;

                        }

                    case PrintersSupported.Printer_ZebraMZ320:
                        {
                            try
                            {
                                PrinterSupport_ZebraMZ320 myMZPrinter = new PrinterSupport_ZebraMZ320(DroidContext.ApplicationContext);

                                myMZPrinter.SendPrintDirect(iTicketImageBitmap, null, true, DroidContext.ApplicationContext);

                                loPrintSuccess = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception source: {0}", e.Source);
                            }

                            //return;
                            break;

                        }

                    default:
                        {

                            toastMsg = "Unsupported printer selected.";
                            Console.WriteLine("Unsupported printer selected = {0}", DroidContext.gPrinterSelected.ToString());

                            //return;
                            break;
                        }
                }

                if (toastMsg.Length > 0)
                {
                    Console.WriteLine("Print Result: {0}", toastMsg);
                }


            }
            catch (Exception exp)
            {
                ////
            }


            try
            {
                if (iTicketImageBitmap != null)
                {
                    if (iTicketImageBitmap.IsRecycled == false)
                    {
                        iTicketImageBitmap.Recycle();
                        iTicketImageBitmap.Dispose();
                        iTicketImageBitmap = null;
                    }
                }
            }
            catch (Exception exp)
            {
                ////
            }

            ioResultMsg = GetLastPrintJobResultMessage();
        }


        /*
                public static void SendStringsDataToCurrentlySelectedPrinter(List<PCLPrintingClass.PCLStringRow> ticketStrings, ref string ioResultMsg)
                {
                    if ((ticketStrings == null) || (ticketStrings.Count == 0))
                    {
                        return; //If first item is invalid, then no need to continue 
                    }

                    try
                    {
                        switch (DroidContext.gPrinterSelected)
                        {
                            case PrintersSupported.Printer_TwoTechnologiesN5Class:
                                {

        #if _integrate_n5_support_

                                    try
                                    {
                                        PrinterSupport_TwoTechN5Printer myN5Printer = new PrinterSupport_TwoTechN5Printer(DroidContext.ApplicationContext);
                                        myN5Printer.SendPrintDirectPCL(ticketStrings, true, DroidContext.ApplicationContext);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Exception source: {0}", e.Source);
                                    }
        #endif
                                    //return;
                                    break;

                                }


                            case PrintersSupported.Printer_ZebraZQ510:
                            case PrintersSupported.Printer_ZebraiMZ30:
                            case PrintersSupported.Printer_ZebraMZ320:
                            default:
                                {

                                    toastMsg = "PCL command printing is unsupported on printer.";
                                    Console.WriteLine("Unsupported printer selected = {0}", DroidContext.gPrinterSelected.ToString());

                                    //return;
                                    break;
                                }
                        }

                        if (toastMsg.Length > 0)
                        {
                            Console.WriteLine("Print Result: {0}", toastMsg);
                        }


                    }
                    catch (Exception exp)
                    {
                        ////
                    }
                    ioResultMsg = GetLastPrintJobResultMessage();
                }
            }
         * 
                */
    }
}