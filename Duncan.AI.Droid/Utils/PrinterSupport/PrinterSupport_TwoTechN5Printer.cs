

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
using Android.Content.PM;


using Android.Graphics;
using Android.Views.Animations;
using Android.Animation;
using System.IO;
using Duncan.AI.Droid.Common;
using Android.Util;
using Android.Preferences;
using System.Threading.Tasks;
using Duncan.AI.Droid.Utils.HelperManagers;
using System.Drawing;

using System.Threading;
using System.Duncan.Drawing;
using Reino.ClientConfig;




namespace Duncan.AI.Droid.Utils.PrinterSupport
{



    public class PrinterSupport_TwoTechN5Printer : PrinterSupport_BaseClass
    {

        private int fPrintMode = Constants.TWOTECH_N5PRINTER_MODE_T5_TEXT;  //Print ticket using PCL commands vs. image. For now the defualt is using PCL commands


        public void SetN5PrintingMode_PCL()
        {
            SetN5PrintingMode(Constants.TWOTECH_N5PRINTER_MODE_T5_TEXT);
        }

        public void SetN5PrintingMode_Graphic()
        {
            SetN5PrintingMode(Constants.TWOTECH_N5PRINTER_MODE_T5_GRAPHIC);
        }

        public void SetN5PrintingMode(int iPrintMode)
        {
            fPrintMode = iPrintMode;
        }

        public int IsPCLCurrentPrintingMode()
        {
            return fPrintMode;
        }

        public PrinterSupport_TwoTechN5Printer(Context iApplicationContext, int iPrintMode)
            : base(iApplicationContext)
        {
            // base class has been called. do something original here
            fPrintMode = iPrintMode;
        }

        //This function will set the 2T N5 device as default printer and launch PrintManager app
        public static void SetTwoTechN5PrinterAsDefaultPrinter(Context context)
        {
            try
            {
                int loN5PrintMode = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                TTRegistry.regPRINTER_GRAPHIC_MODE_T5,
                                                                                TTRegistry.regPRINTER_GRAPHIC_MODE_T5_DEFAULT);
                //Based on the configured mode in the registry we should sned only one file.
                if (loN5PrintMode == Constants.TWOTECH_N5PRINTER_MODE_T5_TEXT)
                {
                    //Set the default selected printer to 2T if the current device is N5Class
                    Constants.PRINTER_TYPE = Constants.PRINTER_TYPE_NAME_N5CLASS_PRINTER_TEXT;
                }else{
                    //Set the default selected printer to 2T if the current device is N5Class
                    Constants.PRINTER_TYPE = Constants.PRINTER_TYPE_NAME_N5CLASS_PRINTER_GRAPHIC;                    
                }
                //Also launch the PrintManager external app            
                bool loResult = false;
                Intent mainIntent = new Intent(Intent.ActionMain, null);
                mainIntent.AddCategory(Intent.CategoryLauncher);
                IList<ResolveInfo> pkgAppsList = context.PackageManager.QueryIntentActivities(mainIntent, 0);
                int loCount = pkgAppsList.Count;
                if (pkgAppsList != null && pkgAppsList.Count > 0)
                {
                    foreach (ResolveInfo loActInfo in pkgAppsList)
                    {
                        //Look for Print Manager "com.twotechnologies.n5simpleprint"                        
                        String loProcessName = loActInfo.ActivityInfo.ApplicationInfo.ProcessName;
                        if (String.IsNullOrEmpty(loProcessName)) continue;

                        if (loProcessName.Contains(Constants.TWOTECH_N5PRINTER_PM_APP_NAME_TOKEN))
                        {
                            //Found it, then launch it
                            PackageManager pm = context.PackageManager;
                            Intent loIntent = pm.GetLaunchIntentForPackage(loProcessName);
                            if (null != loIntent)
                            {
                                context.StartActivity(loIntent);
                            }
                            //Now tell the PM to move to background
                            Intent intent = new Intent(Constants.TWOTECH_N5PRINTER_SEND_PM_TO_BACKGROUND_INTENT);
                            context.SendBroadcast(intent);
                            loResult = true;
                            break;
                        }
                    }
                }
                if (!loResult)
                {
                    //PrintManager is not installed, pop up error message
                    ErrorHandling.ThrowError(context, ErrorHandling.ErrorCode.PrinterManager, "");
                }
            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "Exception", "SetTwoTechN5PrinterAsDefaultPrinter");
                Console.WriteLine("SetTwoTechN5PrinterAsDefaultPrinter Exception: {0}", exp);
            }
        }

        public void LaunchN5PrintServiceForAI(string iTicketFileName)
        {
            try
            {
                // add paths to the passed filenames if they are not empty
                string loTicketFileNameAbsolutePath = string.Empty;                
                 
                //Based on the configured mode in the registry we should sned only one file.
                if (iTicketFileName.Length > 0)
                {
                    //Graphic mode
                    loTicketFileNameAbsolutePath = System.IO.Path.Combine(Helper.GetMultimediaFolder(), iTicketFileName);
                }
                else
                {
                    //Nothig to print, exit now
                    return;
                }
                
                //Get printing parameters based on the current mode
                int loN5ContrastLevel = 0;
                int loN5SetFFMaxFeed = 0;
                int loN5SetFFSkipLength = 0;
                int loN5ExtraDotLinesPreFeed = 0;
                int loN5ExtraDotLinesPostFeed = 0;



                if (fPrintMode == Constants.TWOTECH_N5PRINTER_MODE_T5_TEXT)

                {
                    //Text (PCL) mode parameters
                    loN5SetFFMaxFeed = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                   TTRegistry.regPRINTER_SETFF_MAXFEED_T5,
                                                                                   TTRegistry.regPRINTER_SETFF_MAXFEED_T5_DEFAULT);

                    loN5SetFFSkipLength = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                      TTRegistry.regPRINTER_SETFF_SKIPLENGTH_T5,
                                                                                      TTRegistry.regPRINTER_SETFF_SKIPLENGTH_T5_DEFAULT);

                    loN5ExtraDotLinesPreFeed = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                           TTRegistry.regEXTRA_DOT_LINES_T5_PREFEED,
                                                                                           TTRegistry.regEXTRA_DOT_LINES_T5_PREFEED_DEFAULT);

                    loN5ExtraDotLinesPostFeed = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                            TTRegistry.regEXTRA_DOT_LINES_T5_POSTFEED,
                                                                                            TTRegistry.regEXTRA_DOT_LINES_T5_POSTFEED_DEFAULT);
                    loN5ContrastLevel = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                    TTRegistry.regPRINTER_CONTRAST_LEVEL_TEXT_T5,
                                                                                    TTRegistry.regPRINTER_CONTRAST_LEVEL_TEXT_T5_DEFAULT);
                }
                else
                {
                    //Graphic mode parameters
                    loN5SetFFMaxFeed = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                   TTRegistry.regPRINTER_SETFF_MAXFEED_GRAPHIC_T5,
                                                                                   TTRegistry.regPRINTER_SETFF_MAXFEED_GRAPHIC_T5_DEFAULT);

                    loN5SetFFSkipLength = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                      TTRegistry.regPRINTER_SETFF_SKIPLENGTH_GRAPHIC_T5,
                                                                                      TTRegistry.regPRINTER_SETFF_SKIPLENGTH_GRAPHIC_T5_DEFAULT);

                    loN5ExtraDotLinesPreFeed = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                           TTRegistry.regEXTRA_DOT_LINES_GRAPHIC_T5_PREFEED,
                                                                                           TTRegistry.regEXTRA_DOT_LINES_GRAPHIC_T5_PREFEED_DEFAULT);

                    loN5ExtraDotLinesPostFeed = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                            TTRegistry.regEXTRA_DOT_LINES_GRAPHIC_T5_POSTFEED,
                                                                                            TTRegistry.regEXTRA_DOT_LINES_GRAPHIC_T5_POSTFEED_DEFAULT);
                    loN5ContrastLevel = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                    TTRegistry.regPRINTER_CONTRAST_LEVEL_GRAPHIC_T5,
                                                                                    TTRegistry.regPRINTER_CONTRAST_LEVEL_GRAPHIC_T5_DEFAULT);
                }
                //Init the intent
                Intent intent = new Intent(Constants.TWOTECH_N5PRINTER_PRINTJOB_INTENT);

                //Sned ticket file name.
                intent.PutExtra("MyN5PrintServiceForAI.TicketFileName", loTicketFileNameAbsolutePath);
                
                //Send the reg values to the N5 print manager
                int[] loRegParameters = new int[6];
                loRegParameters[0] = fPrintMode;
                loRegParameters[1] = loN5SetFFMaxFeed;
                loRegParameters[2] = loN5SetFFSkipLength;
                loRegParameters[3] = loN5ExtraDotLinesPreFeed;
                loRegParameters[4] = loN5ExtraDotLinesPostFeed;
                loRegParameters[5] = loN5ContrastLevel;

                intent.PutExtra("MyN5PrintServiceForAI.RegParameters", loRegParameters);

                
                _ApplicationContext.SendBroadcast(intent);
            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "FileName: " + iTicketFileName, "LaunchN5PrintServiceForAI");
                Console.WriteLine("LaunchN5PrintServiceForAI Exception: {0}", exp);
            }
        }



        public void SendPrintDirectPCL(List<PCLPrintingClass.PCLStringRow> ticketPCLCommands, bool useLooper, Context iContext)
        {
            string loNewPrintQueueFileNameOnly = string.Empty;
            try
            {

                if (ticketPCLCommands == null) return; //nothing to print, return now.

                loNewPrintQueueFileNameOnly = "N5-XXXXX_PRINTQUEUE_001" + Constants.PCLCMD_FILE_SUFFIX;

                Helper.SaveTIssueFormPrintJobPCLCommandFile(loNewPrintQueueFileNameOnly, ticketPCLCommands);

                LaunchN5PrintServiceForAI(loNewPrintQueueFileNameOnly);

                toastMsg = "Print Job Submitted";

            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "FileName: " + loNewPrintQueueFileNameOnly, "SendPrintDirectPCL");
                Console.WriteLine("SendPrintDirectPCL Exception: {0}", exp);
            }

        }

        /// <summary>
        /// T
        /// </summary>
        /// <param name="issueNum"></param>
        /// <param name="issueDate"></param>
        /// <param name="localFlag"></param>
        public void SendPrintDirect(Android.Graphics.Bitmap iTicketImageBitmap, bool useLooper, Context iContext)
        {
            string loNewPrintQueueFileNameOnly = string.Empty;

            try
            {
                // save the bitmap as printqued file
                loNewPrintQueueFileNameOnly = Constants.SERIAL_NUMBER + "_PRINTQUEUE_001" + Constants.PHOTO_FILE_SUFFIX;

                string loNewPrintQueueFileNameAbsolutePath = System.IO.Path.Combine(Helper.GetMultimediaFolder(), loNewPrintQueueFileNameOnly);

                using (var os = new FileStream(loNewPrintQueueFileNameAbsolutePath, FileMode.Create))  // will overwrite every time, or do we need to delete old first?
                {
                    iTicketImageBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, os);
                    os.Close();
                }

                iTicketImageBitmap.Recycle();  // ?? it not of our origin , should we really?


                LaunchN5PrintServiceForAI(loNewPrintQueueFileNameOnly);

                toastMsg = "Print Job Submitted";

            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "FileName: " + loNewPrintQueueFileNameOnly, "SendPrintDirectPCL");
                Console.WriteLine("SendPrintDirectPCL Exception: {0}", exp);
            }

        }


    }
}
