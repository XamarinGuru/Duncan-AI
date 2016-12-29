using System;
using Android.Util;
namespace Duncan.AI.Droid.Utils.HelperManagers
{
    public class LoggingManager
    {
        public static void LogApplicationError(Exception ex, string additionalInformation, string callingMethod)
        {
            if (ex != null)
            {
                Log.Error("Error:", "An Exception has occurred at : " + DateTime.Now.ToString(Constants.DT_YYYY_MM_DDT));
                Log.Error("Error Stack:", ex.StackTrace);
                Log.Error("Error Message:", ex.Message);
                Log.Error("Error Source:", ex.Source);
            }
            else
            {
                Log.Error("Error:", "Non-Exception has occurred at : " + DateTime.Now.ToString(Constants.DT_YYYY_MM_DDT));
            }

            if (!string.IsNullOrEmpty(additionalInformation))
            {
                Log.Error("Additional Info: ", additionalInformation);
            }

            if (!string.IsNullOrEmpty(callingMethod))
            {
                Log.Error("Calling Method: ", callingMethod);
            }
        }
    }
}