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
using Duncan.AI.Droid.Utils.HelperManagers;

namespace Duncan.AI.Droid.Utils
{
    public class DateTimeHelper
    {
        public DateTimeHelper()
        {
        }

    
        /// <summary>
        /// Convert the DBDate column value from the fixed database format into the destination format
        /// </summary>
        /// <param name="iPanelField"></param>
        /// <returns></returns>
        public static string ConvertDBDateColumnValueToString(string iSourceDBDateAsString, string iDestDateMask)
        {
            string loResultAsString = "";

            if (iSourceDBDateAsString.Length == 0)
            {
                return loResultAsString;
            }

            try
            {
                DateTime loOSDate = DateTime.Now;

                // first convert it to a DateTime object
                if (DateTimeManager.DateStringToOSDate(Duncan.AI.Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK, iSourceDBDateAsString, ref loOSDate) != 0)
                {
                    string loMethodName = "ConvertDBDateColumnValueToString";
                    string loErrMsg = "Error converting DateStringToOSDate( " +
                                      Duncan.AI.Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK + " " +
                                      iSourceDBDateAsString +
                                      ", ref DateTime )";
                    LoggingManager.LogApplicationError(null, loErrMsg, loMethodName);
                    Console.WriteLine(" Error in " + loMethodName + ": " + loErrMsg);
                }


                // now convert it into target mask
                DateTimeManager.OsDateToDateString(loOSDate, iDestDateMask, ref loResultAsString);
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, null, "ConvertDBDateColumnValueToString");
                Console.WriteLine("ConvertDBDateColumnValueToString Exception:", e.Message);
            }

            return loResultAsString;
        }


        /// <summary>
        /// Convert the DBDate column value from the source format into the fixed database format 
        /// </summary>
        /// <param name="iPanelField"></param>
        /// <returns></returns>
        public static string ConvertDateColumnValueToFixedDBString(string iSourceDateValueAsString, string iSourceDateMask)
        {
            string loResultAsString = "";

            if (iSourceDateValueAsString.Length == 0)
            {
                return loResultAsString;
            }

            try
            {
                DateTime loOSDate = DateTime.Now;

                // first convert it to a DateTime object
                if (DateTimeManager.DateStringToOSDate(iSourceDateMask, iSourceDateValueAsString, ref loOSDate) != 0)
                {
                    string loMethodName = "ConvertDateColumnValueToFixedDBString";
                    string loErrMsg = "Error converting DateStringToOSDate( " +
                                      iSourceDateMask + " " + 
                                      iSourceDateValueAsString +
                                      ", ref DateTime )";
                    LoggingManager.LogApplicationError(null, loErrMsg, loMethodName);
                    Console.WriteLine(" Error in " + loMethodName + ": " + loErrMsg);
                }


                // now convert it into target mask
                DateTimeManager.OsDateToDateString(loOSDate, Duncan.AI.Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK, ref loResultAsString);
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, null, "ConvertDateColumnValueToFixedDBString");
                Console.WriteLine("ConvertDateColumnValueToFixedDBString Exception:", e.Message);
            }

            return loResultAsString;
        }




        /// <summary>
        /// Convert the DBTime column value from the fixed database format into the destination format
        /// </summary>
        /// <param name="iPanelField"></param>
        /// <returns></returns>
        public static string ConvertDBTimeColumnValueToString(string iSourceDBTimeAsString, string iDestTimeMask)
        {
            string loResultAsString = "";

            if (iSourceDBTimeAsString.Length == 0)
            {
                return loResultAsString;
            }

            try
            {
                DateTime loOSTime = DateTime.Now;

                // first convert it to a DateTime object
                if (DateTimeManager.TimeStringToOSTime(Duncan.AI.Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK, iSourceDBTimeAsString, ref loOSTime) != 0)
                {
                    string loMethodName = "ConvertDBTimeColumnValueToString";
                    string loErrMsg = "Error converting TimeStringToOSTime( " +
                                      Duncan.AI.Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK + " " +
                                      iSourceDBTimeAsString +
                                      ", ref DateTime )";
                    LoggingManager.LogApplicationError(null, loErrMsg, loMethodName);
                    Console.WriteLine(" Error in " + loMethodName + ": " + loErrMsg);
                }


                // now convert it into target mask
                DateTimeManager.OsTimeToTimeString(loOSTime, iDestTimeMask, ref loResultAsString);
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, null, "ConvertDBTimeColumnValueToString");
                Console.WriteLine("ConvertDBTimeColumnValueToString Exception:", e.Message);
            }

            return loResultAsString;
        }




        /// <summary>
        /// Convert the DBTime column value from the source format into the fixed database format 
        /// </summary>
        /// <param name="iPanelField"></param>
        /// <returns></returns>
        public static string ConvertTimeColumnValueToFixedDBString(string iSourceTimeValueAsString, string iSourceTimeMask)
        {
            string loResultAsString = "";

            if (iSourceTimeValueAsString.Length == 0)
            {
                return loResultAsString;
            }

            try
            {
                DateTime loOSTime = DateTime.Now;

                // first convert it to a DateTime object
                if (DateTimeManager.TimeStringToOSTime(iSourceTimeMask, iSourceTimeValueAsString, ref loOSTime) != 0)
                {
                    string loMethodName = "ConvertTimeColumnValueToFixedDBString";
                    string loErrMsg = "Error converting TimeStringToOSTime( " +
                                      iSourceTimeMask + " " + 
                                      iSourceTimeValueAsString +
                                      ", ref DateTime )";
                    LoggingManager.LogApplicationError(null, loErrMsg, loMethodName);
                    Console.WriteLine(" Error in " + loMethodName + ": " + loErrMsg);
                }


                // now convert it into target mask
                DateTimeManager.OsTimeToTimeString(loOSTime, Duncan.AI.Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK, ref loResultAsString);
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, null, "ConvertTimeColumnValueToFixedDBString");
                Console.WriteLine("ConvertTimeColumnValueToFixedDBString Exception:", e.Message);
            }

            return loResultAsString;
        }

    }
}