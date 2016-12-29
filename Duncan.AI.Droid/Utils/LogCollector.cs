using Android.App;
using Android.Content;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Util;
using Android.OS;

namespace Duncan.AI.Droid.Utils
{
    public class LogCollector
    {
        private static String filePath = String.Empty;

        public static void WriteLogData(Activity a, Context c)
        {
            try
            {
                LogCollector log = new LogCollector();
                string logdata = log.CollectLogs(c);

                String toastMsg = "Written the log file";
                a.RunOnUiThread(() => { Android.Widget.Toast.MakeText(c, toastMsg, ToastLength.Long).Show(); });
            }
            catch (Exception e)
            {
            }
        }

        
        public static String SaveLogData(Context context)
        {
            String loFilePath;
            LogCollector log = new LogCollector();
            string logdata = log.CollectLogs(context);
            loFilePath = LogCollector.filePath;
            return loFilePath;
        }
                
        public string GetAndroidVersionNumber()
        {
            var codeName = string.Empty;
            var sdkName = string.Empty;
            var sdkNum = string.Empty;

            try { codeName = Android.OS.Build.VERSION.Codename; }
            catch { }

            try { sdkName = Android.OS.Build.VERSION.Sdk; }
            catch { }

            try { sdkNum = Android.OS.Build.VERSION.SdkInt.ToString(); }
            catch { }

            return string.Format("{0} ({1}) [{2}]", sdkName, sdkNum, codeName);
        }

        public string GetVersionNumber(Context context)
        {
            var version = "?";

            try
            {
                var pkgInfo = context.PackageManager.GetPackageInfo(context.PackageName, (Android.Content.PM.PackageInfoFlags)0);
                version = pkgInfo.VersionName + " (" + pkgInfo.VersionCode + ")";
            }
            catch { }

            return version;
        }

        public string CollectLogs(Context context)
        {
            var log = new StringBuilder();
            //File name should be: 
            //YYYYMMDD_THHMMSSZ_XX-NNNNN_AIClient_ClientName.log
            //Where:
            //     "YYYYYMMDD_THHMMSSZ" is GMT date/timestamp
            //     "XX-NNNNN" is unit serial number
            //     "ClientName" is the Agency Designator
            //Example:
            //20160413_T000016Z Y4-00300_AIClient_Spokane.log
            DateTime loDateTime = DateTime.UtcNow;
            string loFileName = "/" + loDateTime.ToString("yyyyMMdd") + "_T" + loDateTime.ToString("hhmmss") + "Z_" + Helper.GetSerialNumber(Build.Serial) + "_" + DroidContext.XmlClientName + "_" + DroidContext.XmlAgencyDesignator+ ".log";
            char[] loTemp = loFileName.ToCharArray();
            loFileName = "";
            for (int i = 0; i < loTemp.Length; i++)
            {
                if (loTemp[i] ==' ' || loTemp[i] == 0x20) loTemp[i] = '-'; 
                loFileName += loTemp[i].ToString();
            }

            filePath = Android.OS.Environment.GetExternalStoragePublicDirectory
                        (Android.OS.Environment.DirectoryDownloads) + loFileName; // "/AI_ErrorLogs.log"; 
            //Delete the older log file if there is one
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            string loFilePathCmd = " logcat -d -f " + filePath;
            var p = Java.Lang.Runtime.GetRuntime().Exec(loFilePathCmd);

            var reader = new BufferedReader(new InputStreamReader(p.InputStream));

            string line = "";

            while ((line = reader.ReadLine()) != null)
            {
                log.AppendLine(line.Trim());
            }
            p.InputStream.Close();
            p.OutputStream.Close();
            return log.ToString();
        }


        string GetBody(Context context)
        {
            var body = new StringBuilder();
            body.AppendLine("Log Report");
            body.AppendLine("App Version:     " + GetVersionNumber(context));
            body.AppendLine("Android Version: " + GetAndroidVersionNumber());
            
            body.AppendLine();
            body.AppendLine(CollectLogs(context));

            return body.ToString();
        }

        public void SendEmailIntent(Context context, string emailTo, string introMsg)
        {
            var body = new StringBuilder();

            body.AppendLine(introMsg);
            body.AppendLine();
            body.AppendLine("Log Report:");
            body.AppendLine("=====================================");
            body.AppendLine(GetBody(context));

            var emailIntent = new Intent(Android.Content.Intent.ActionSend);

            emailIntent.PutExtra(Android.Content.Intent.ExtraEmail, emailTo);
            emailIntent.PutExtra(Android.Content.Intent.ExtraSubject, "HyperLocal Log Report");
            emailIntent.SetType("plain/text");
            emailIntent.PutExtra(Android.Content.Intent.ExtraText, body.ToString());

            context.StartActivity(emailIntent);
        }


    }
}
