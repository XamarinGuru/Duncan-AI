using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Reino.ClientConfig;
using Android.Net;
using Duncan.AI.Droid.Utils.HelperManagers;


namespace Duncan.AI.Droid.Utils
{
    [Activity(Label = "Error Handling")]
    class ErrorHandling
    {
        public enum ErrorCode
        {
            Printer_PMNotReady,
            Printer_CoverClosed,
            Printer_PlatenOpen,
            Printer_OutOfPaper,
            Printer_OverTemperature,
            Printer_HardWareFault,
            PrinterManager,
            SyncService,
            GPSService,
            Exception,
            Error_Unknown,
            Error_NumberOfErrors
        };


        public enum ErrorSeverityLevel
        {
            Warning,
            Minor,
            Major,
            Critical
        };

        public struct ErrorStruct
        {
            public ErrorCode errorCode;
            public ErrorSeverityLevel errorLevel;
            public string errorString;
            public bool attachLogFile;

            public ErrorStruct(ErrorCode errCode, ErrorSeverityLevel errLevel, string errStr, bool attachLog)
            {
                errorCode = errCode;
                errorLevel = errLevel;
                errorString = errStr;
                attachLogFile = attachLog;
            }
        };

        static readonly ErrorStruct[] AIErrors = new ErrorStruct[(int)ErrorCode.Error_NumberOfErrors] {
                        new ErrorStruct(ErrorCode.Printer_PMNotReady, ErrorSeverityLevel.Warning, "N5 Print Service is Disconnected. Please wait and try again!", false),
                        new ErrorStruct(ErrorCode.Printer_CoverClosed, ErrorSeverityLevel.Warning, "Printer Cover is Closed", false),
                        new ErrorStruct(ErrorCode.Printer_PlatenOpen, ErrorSeverityLevel.Warning, "Printer Platen is Opened", false),
                        new ErrorStruct(ErrorCode.Printer_OutOfPaper, ErrorSeverityLevel.Warning, "Printer Out of Paper", false),
                        new ErrorStruct(ErrorCode.Printer_OverTemperature, ErrorSeverityLevel.Major, "Printer Temperature is too high", false),
                        new ErrorStruct(ErrorCode.Printer_HardWareFault, ErrorSeverityLevel.Critical, "Printer Hardware Fault", false),
                        new ErrorStruct(ErrorCode.PrinterManager, ErrorSeverityLevel.Critical, "N5 Printer Manager is missing, please install it first befroe printing a ticket.", false),
                        new ErrorStruct(ErrorCode.SyncService, ErrorSeverityLevel.Major, "SyncService Error", false),
                        new ErrorStruct(ErrorCode.GPSService, ErrorSeverityLevel.Major, "Failed to init GPSService", false),
                        new ErrorStruct(ErrorCode.Exception, ErrorSeverityLevel.Critical, "Global Exception", true),
                        new ErrorStruct(ErrorCode.Error_Unknown, ErrorSeverityLevel.Warning, "Unknown Error", false)};

        private static ErrorSeverityLevel fCurrentLevel;  //The level where we will start showing "Send Email" button 
        private static string fEmailAddress = string.Empty;
        private static bool fEnableSendingError = false;
        private static bool fErrorIsHandling = false;
        private static Context fContext;
        //private static AlertDialog.Builder fBuilder = null;

        public ErrorHandling()
        {
        }

        public static void InitErrorHandling(Context context)
        {
            fContext = context;
            GetRegistryValues();
        }

        //public ErrorHandling(Context context, ErrorCode iCode)
        public static void ThrowError(Context context, ErrorCode iCode, string errorInfo)
        {
            if (fErrorIsHandling) return;
            fErrorIsHandling = true;

            ErrorStruct loErrorRecord = GetErrorRecord(iCode);
            if (!string.IsNullOrEmpty(errorInfo)) loErrorRecord.errorString = loErrorRecord.errorString + " = " + errorInfo;

            ErrorDialog loErrDlg = new ErrorDialog(loErrorRecord);
            loErrDlg.Show(((MainActivity)fContext).FragmentManager, "dialog");            
        }


        private static ErrorStruct GetErrorRecord(ErrorCode iCode)
        {
            if ((int)iCode >= (int)ErrorCode.Error_NumberOfErrors)
            {
                return AIErrors[(int)ErrorCode.Error_Unknown];
            }
            return AIErrors[(int)iCode];
        }

        private static void GetRegistryValues()
        {
            try
            {
                if (TTRegistry.glRegistry != null)
                {

                    if (TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                   TTRegistry.regERROR_HANDLING_ENABLE,
                                                                   TTRegistry.regERROR_HANDLING_ENABLE_DEFAULT) > 0)
                    {
                        fEnableSendingError = true;
                    }
                    else
                    {
                        fEnableSendingError = false;
                    }

                    if (fEnableSendingError)
                    {
                        fEmailAddress = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP,
                                                                               TTRegistry.regERROR_HANDLING_EMAIL,
                                                                               TTRegistry.regERROR_HANDLING_EMAIL_DEFAULT);

                        try
                        {
                            fCurrentLevel = (ErrorSeverityLevel)TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                   TTRegistry.regERROR_HANDLING_LEVEL,
                                                                                   TTRegistry.regERROR_HANDLING_LEVEL_DEFAULT);
                        }
                        catch (System.Exception loLevelException)
                        {
                            // defend against casting against invalid values that could be miskeyed into registry
                            fCurrentLevel = ErrorSeverityLevel.Warning;

                            LoggingManager.LogApplicationError(loLevelException, "ErrorHandling.GetRegistryValues ErrorHandlingLevel", loLevelException.TargetSite.Name);
                            System.Console.WriteLine("ErrorHandling.GetRegistryValues ErrorHandlingLevel Exception source {0}: {1}", loLevelException.Source, loLevelException.ToString());
                        }
                    }
                }
                else
                {
                    // registry is not available, set defaults
                    fEnableSendingError = (TTRegistry.regERROR_HANDLING_ENABLE_DEFAULT == 1);
                    fEmailAddress = TTRegistry.regERROR_HANDLING_EMAIL_DEFAULT;
                    fCurrentLevel = (ErrorSeverityLevel)TTRegistry.regERROR_HANDLING_LEVEL_DEFAULT;
                }
            }
            catch (System.Exception ex)
            {
                // an error in the error handling initialization... looks like we're in some trouble here!
                LoggingManager.LogApplicationError(ex, "ErrorHandling.GetRegistryValues", ex.TargetSite.Name);
                System.Console.WriteLine("ErrorHandling.GetRegistryValues Exception source {0}: {1}", ex.Source, ex.ToString());
            }


        }


        public static void ReportException(string errorMsg)
        {
            ErrorStruct loErr = AIErrors[(int)ErrorCode.Exception];
            loErr.errorString = loErr.errorString + " - " + errorMsg;
            SendErrorReport(loErr);
        }

        public static void ReportExceptionWithConfirmationDlg(string errorMsg)
        {
            ErrorStruct loErr = AIErrors[(int)ErrorCode.Exception];
            loErr.errorString = loErr.errorString + " - " + errorMsg;

            if (fContext != null)
            {
                ErrorDialog loErrDlg = new ErrorDialog(loErr);
                loErrDlg.Show(((MainActivity)fContext).FragmentManager, "dialog");
            }
            else
            {
                //main activity is destroied, just allow the error report only
                SendErrorReport(loErr);
            }
        }



        public static void SendErrorReport(ErrorStruct errRecord)
        {
            var body = new System.Text.StringBuilder();
            string deviceId = Build.Serial;
            string modDeviceId = Helper.GetSerialNumber(deviceId);

            body.AppendLine(DateTime.Now.ToShortDateString());
            body.AppendLine(DateTime.Now.ToLongTimeString());
            body.AppendLine();
            body.AppendLine("Software Revision: " + DroidContext.ApplicationContext.Resources.GetString(Resource.String.Version));
            body.AppendLine("Device Serial Number: " + modDeviceId);
            body.AppendLine("Client Name: " + DroidContext.XmlClientName);
            body.AppendLine("Agency Designator: " + DroidContext.XmlAgencyDesignator);
            body.AppendLine("Layout Revision: " + DroidContext.XmlLayoutRevision);            
            body.AppendLine();
            body.AppendLine("Error Report:");
            body.AppendLine("===============================");
            body.AppendLine("Error Code:         " + errRecord.errorCode.ToString());
            body.AppendLine("Error Severity:     " + errRecord.errorLevel.ToString());
            body.AppendLine("Error Description: " + errRecord.errorString);
            body.AppendLine("===============================");

            var emailIntent = new Intent(Android.Content.Intent.ActionSend);
            string[] loEmailAddress = fEmailAddress.Split(';');
            if (loEmailAddress.Length <= 0) return;
            emailIntent.PutExtra(Android.Content.Intent.ExtraEmail, loEmailAddress);
            emailIntent.PutExtra(Android.Content.Intent.ExtraSubject, modDeviceId + " - Error Report - " + errRecord.errorLevel.ToString());
            emailIntent.SetType("plain/text");
            emailIntent.PutExtra(Android.Content.Intent.ExtraText, body.ToString());
            //Do we need to attach log file
            if (errRecord.attachLogFile)
            {
                System.String loLogFilePath = LogCollector.SaveLogData(fContext);
                if (!System.String.IsNullOrEmpty(loLogFilePath))
                {
                    Java.IO.File loLogFileName = new Java.IO.File(loLogFilePath);
                    emailIntent.PutExtra(Intent.ExtraStream, Android.Net.Uri.FromFile(loLogFileName));
                }
            }

            ((Activity)(fContext)).StartActivity(emailIntent);
        }

        //Class to display error/excpetion dialog 
        public class ErrorDialog : DialogFragment
        {
            private static ErrorHandling.ErrorStruct fError;

            public ErrorDialog()
            {
            }

            public ErrorDialog(ErrorHandling.ErrorStruct err)
            {
                fError = err;
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                var alert = new AlertDialog.Builder(Activity);
                alert.SetIconAttribute(Android.Resource.Attribute.Action);

                switch (fError.errorLevel)
                {
                    case ErrorSeverityLevel.Minor:
                    case ErrorSeverityLevel.Major:
                    case ErrorSeverityLevel.Critical:
                        alert.SetTitle("Error");
                        break;

                    case ErrorSeverityLevel.Warning:
                    default:
                        alert.SetTitle("Warning");
                        break;
                }

                alert.SetMessage(fError.errorString);

                if (fEnableSendingError)
                {
                    if (((int)fError.errorLevel >= (int)fCurrentLevel) && fEmailAddress != string.Empty)
                    {
                        //Send email is needed
                        alert.SetPositiveButton("Send Report", delegate
                        {

                            ErrorHandling.SendErrorReport(fError);
                            ErrorHandling.fErrorIsHandling = false;
                            this.DismissAllowingStateLoss();
                        });
                    }
                }

                alert.SetNegativeButton("Ok", delegate
                {
                    ErrorHandling.fErrorIsHandling = false;
                });
                return alert.Show();
            }
        }
    }
}