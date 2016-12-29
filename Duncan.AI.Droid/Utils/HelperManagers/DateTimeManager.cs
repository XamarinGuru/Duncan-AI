using System;

namespace Duncan.AI.Droid.Utils.HelperManagers
{
    public static class DateTimeManager
    {
        /// <summary>
        /// Describes the data element to spin for nextlistitem and prevlistitem fields
        /// </summary>
        public enum TDateTimeCommonSpinElement
        {
            seSpinYear = 0,
            seSpinMonth,
            seSpinDay,
            seSpinHour,
            seSpinMinute
        };


    

        /// <summary>
        ///     Converts a OSDate to a formated string. Accepted mask tokens are:
        ///     WWW - 3 character day of week abreviation.
        ///     WWWW - day of week spelled out.
        ///     MM           - month number left padded with 0 to 2 digits
        ///     mm           - month number trimmed to length
        ///     MON          - 3 character month abbreviation
        ///     MONTH        - month spelled out.
        ///     D or d       - day number trimmed to length.
        ///     DD           - day number left padded with 0 to 2 digits
        ///     dd           - day number left padded with space to 2 digits
        ///     DDD or ddd   - day number within a given year (Used for Julian date formats, ie. "yyyyddd")
        ///     YY or yy     - 2 digit year
        ///     YYYY or yyyy - 4 digit year
        /// </summary>
        public static int OsDateToDateString(DateTime iOSDate, string iPictureMask, ref string oDateString)
        {
            int loYear = 0;
            int loMonth = 0;
            int loDayNo = 0;
            int loDayOfWeek = 0;
            int loDayOfYear = 0;
            var dayFullNames = new string[7]{"SUNDAY", "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY"};
            var dayAbrevNames = new string[7] {"SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"};
            // These arrays have an extra blank value at index 0, so "January" = 1 (instead of zero)
            var monthFullNames = new string[13]
                {
                    "", "JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY", "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER",
                    "NOVEMBER", "DECEMBER"
                };
            var monthAbrevNames = new string[13]{"", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"};

            // convert the date into d/m/y and day of week
            loYear = iOSDate.Year;
            loMonth = iOSDate.Month;
            loDayNo = iOSDate.Day;
            loDayOfWeek = (int) iOSDate.DayOfWeek;
            loDayOfYear = iOSDate.DayOfYear;

            if (iPictureMask == "")
                iPictureMask = "YYYYMMDD";

            oDateString = "";
            for (int loIdx = 0; loIdx < iPictureMask.Length;)
            {
                // Get the next mask substring
                string loMaskSubStr = iPictureMask.Substring(loIdx);

                if ((loMaskSubStr.StartsWith("MM")) ||
                    (loMaskSubStr.StartsWith("mm")))
                {
                    // Month number  
                    oDateString += loMonth.ToString().PadLeft(2, '0');
                    loIdx += 2;
                    continue;
                }

                if ((loMaskSubStr.StartsWith("DDD")) ||
                    (loMaskSubStr.StartsWith("ddd")))
                {
                    // This is the day of the year number (so Feb 2
                    // would be day number 33).
                    oDateString += loDayOfYear.ToString().PadLeft(3, '0');
                    loIdx += 3;
                    continue;
                }

                if ((loMaskSubStr.StartsWith("DD")) ||
                    (loMaskSubStr.StartsWith("dd")))
                {
                    // Day number  
                    oDateString += loDayNo.ToString().PadLeft(2, '0');
                    loIdx += 2;
                    continue;
                }

                if ((loMaskSubStr.StartsWith("YYYY")) ||
                    (loMaskSubStr.StartsWith("yyyy")))
                {
                    // 4 digit year 
                    oDateString += loYear.ToString().PadLeft(4, '0');
                    loIdx += 4;
                    continue;
                }

                if ((loMaskSubStr.StartsWith("YY")) ||
                    (loMaskSubStr.StartsWith("yy")))
                {
                    // 2 digit year 
                    oDateString += loYear.ToString().PadLeft(4, '0').Substring(2, 2);
                    loIdx += 2;
                    continue;
                }

                if (loMaskSubStr.StartsWith("WWWW"))
                {
                    // day of week spelled out 
                    oDateString += dayFullNames[loDayOfWeek];
                    loIdx += 4;
                    continue;
                }

                if (loMaskSubStr.StartsWith("WWW"))
                {
                    // day of week abreviated 
                    oDateString += dayAbrevNames[loDayOfWeek];
                    loIdx += 3;
                    continue;
                }

                if (loMaskSubStr.StartsWith("MONTH"))
                {
                    // full Month name 
                    oDateString += monthFullNames[loMonth];
                    loIdx += 5;
                    continue;
                }

                if (loMaskSubStr.StartsWith("MON"))
                {
                    // abreviated Month name 
                    oDateString += monthAbrevNames[loMonth];
                    loIdx += 3;
                    continue;
                }

                // wasn't a token, so add as a literal
                oDateString += iPictureMask[loIdx];
                loIdx++;
            }
            return 0;
        }

        /// <summary>
        ///     Converts a OSTime to a formated string. Accepted mask tokens are:
        ///     MM,mm        - minute left padded with 0 to 2 digits
        ///     HH,hh        - hour left padded with 0 to 2 digits. (12 hr in presence of TT/tt, 24 hr in absence)
        ///     SS,ss        - seconds left padded with 0 to 2 digits
        ///     TT           - Uppercase AM/PM
        ///     tt           - Lowercase am/pm
        /// </summary>
        public static int OsTimeToTimeString(DateTime iOSTime, string iPictureMask, ref string oTimeString)
        {
            bool lo12HrTime = false;

            // convert the time into h/m/s 
            int loSecond = iOSTime.Second;
            int loMinute = iOSTime.Minute;
            int loHour = iOSTime.Hour;

            if (iPictureMask == "")
                return -1;

            // will we be using 24hr or 12 hour time? 
            lo12HrTime = (iPictureMask.IndexOf("T") >= 0) || (iPictureMask.IndexOf("t") >= 0);

            oTimeString = "";
            for (int loIdx = 0; loIdx < iPictureMask.Length;)
            {
                // Get the next mask substring
                string loMaskSubStr = iPictureMask.Substring(loIdx);

                if ((loMaskSubStr.StartsWith("HH")) ||
                    (loMaskSubStr.StartsWith("hh")) ||
                    (loMaskSubStr.ToUpper().StartsWith("HH")))
                {
                    // hour number  
                    // 12 hour time? 
                    string loNumStr = "";
                    if (lo12HrTime && (loHour > 12))
                        loNumStr = Convert.ToString(loHour - 12);
                    else if (lo12HrTime && (loHour == 0))
                        loNumStr = Convert.ToString(12);
                    else
                        loNumStr = Convert.ToString(loHour);
                    oTimeString += loNumStr.PadLeft(2, '0');
                    loIdx += 2;
                    continue;
                }

                if ((loMaskSubStr.StartsWith("MM")) ||
                    (loMaskSubStr.StartsWith("mm")) ||
                    (loMaskSubStr.ToUpper().StartsWith("MM")))
                {
                    // minute number  
                    oTimeString += loMinute.ToString().PadLeft(2, '0');
                    loIdx += 2;
                    continue;
                }

                if ((loMaskSubStr.StartsWith("SS")) ||
                    (loMaskSubStr.StartsWith("ss")) ||
                    (loMaskSubStr.ToUpper().StartsWith("SS")))
                {
                    // seconds number  
                    oTimeString += loSecond.ToString().PadLeft(2, '0');
                    loIdx += 2;
                    continue;
                }

                if ((loMaskSubStr.StartsWith("TT")) ||
                    (loMaskSubStr.StartsWith("tt")) ||
                    (loMaskSubStr.ToUpper().StartsWith("TT")))
                {
                    // AM/PM 
                    if (loMaskSubStr.StartsWith("T")) // if upper case 
                    {
                        if (loHour >= 12)
                            oTimeString += "PM";
                        else
                            oTimeString += "AM";
                    }
                    else // else lower case 
                    {
                        if (loHour >= 12)
                            oTimeString += "pm";
                        else
                            oTimeString += "am";
                    }
                    loIdx += 2;
                    continue;
                }

                    // special circumstance they just want the A or the P
                if ((loMaskSubStr.StartsWith("T")) ||
                    (loMaskSubStr.StartsWith("t")))
                {
                    // AM/PM 
                    if (loMaskSubStr.StartsWith("T")) // if upper case 
                    {
                        if (loHour >= 12)
                            oTimeString += "P";
                        else
                            oTimeString += "A";
                    }
                    else // else lower case 
                    {
                        if (loHour >= 12)
                            oTimeString += "p";
                        else
                            oTimeString += "a";
                    }
                    loIdx += 1;
                    continue;
                }

                // wasn't a token, so add as a literal
                oTimeString += iPictureMask[loIdx];
                loIdx++;
            }
            return 0;
        }


        /// <summary>
        /// Returns the logical spin element for the DATE data type
        /// </summary>
        /// <param name="iEditMask"></param>
        /// <returns></returns>
        public static TDateTimeCommonSpinElement GetDateTypeSpinElementForEditMask(string iEditMask)
        {

            if (string.IsNullOrEmpty(iEditMask) == false)
            {

                // convert to upper case for simplified comparison
                string loEditMaskUpper = iEditMask.Trim().ToUpper();

                // look for matches in decesnding logic order
                if (
                    (loEditMaskUpper.Contains("DD") == true) ||
                    (loEditMaskUpper.Contains("WWW") == true)
                    )
                {
                    return TDateTimeCommonSpinElement.seSpinDay;
                }

                if (
                    (loEditMaskUpper.Contains("MM") == true) ||
                    (loEditMaskUpper.Contains("MON") == true)
                    )
                {
                    return TDateTimeCommonSpinElement.seSpinMonth;
                }


                if (loEditMaskUpper.Contains("YY") == true)
                {
                    return TDateTimeCommonSpinElement.seSpinYear;
                }
            }


            // default
            return TDateTimeCommonSpinElement.seSpinDay;
        }


        /// <summary>
        /// Returns the logical spin element for the TIME data type
        /// </summary>
        /// <param name="iEditMask"></param>
        /// <returns></returns>
        public static TDateTimeCommonSpinElement GetTimeTypeSpinElementForEditMask(string iEditMask)
        {

            if (string.IsNullOrEmpty(iEditMask) == false)
            {

                // convert to upper case for simplified comparison
                string loEditMaskUpper = iEditMask.Trim().ToUpper();

                // look for matches in decesnding logic order
                if (
                    (loEditMaskUpper.Contains("HH") == true)
                    )
                {
                    return TDateTimeCommonSpinElement.seSpinHour;
                }

                if (
                    (loEditMaskUpper.Contains("MM") == true)
                    )
                {
                    return TDateTimeCommonSpinElement.seSpinMinute;
                }

            }

            // default
            return TDateTimeCommonSpinElement.seSpinMinute;
        }



        /// <summary>
        ///     Returns a substring starting at iStart position and upto iMaxLength characters.
        ///     If iStart is invalid, and empty string is returned. If there are not enough characters
        ///     to satisfy iMaxLength, then the returned substring will contain as many characters as
        ///     possible.
        /// </summary>
        public static string SafeSubString(string iString, int iStart, int iMaxLength)
        {
            return iStart > iString.Length - 1 ? "" : iString.Substring(iStart, Math.Min(iMaxLength, (iString.Length - iStart)));
        }

        /// <summary>
        ///     Converts a formated datestring to an OSDate. Accepted mask tokens are:
        ///     WWW - 3 character day of week abreviation.
        ///     WWWW - day of week spelled out.
        ///     MM           - month number left padded with 0 to 2 digits
        ///     mm           - month number trimmed to length
        ///     MON          - 3 character month abbreviation
        ///     MONTH        - month spelled out.
        ///     D or d       - day number trimmed to length.
        ///     DD           - day number left padded with 0 to 2 digits
        ///     dd           - day number left padded with space to 2 digits
        ///     YY or yy     - 2 digit year
        ///     YYYY or yyyy - 4 digit year
        /// </summary>
        public static int DateStringToDMY(string iPictureMask, string iDateString, ref int oDayNo, ref int oMonth,
                                          ref int oYear)
        {
            const int invalidMonth = -2;
            const int invalidDay = -1;
            const int invalidYear = -3;
            int dayOfYear = 0;

            // These arrays have an extra blank value at index 0, so "January" = 1 (instead of zero)
            var monthFullNames = new string[13]
                {
                    "", "JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY", "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER",
                    "NOVEMBER", "DECEMBER"
                };
            var monthAbrevNames = new string[13]
                {"", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"};

            oYear = 2000;
            oMonth = 1;
            oDayNo = 1;

            if (iPictureMask == "")
                return -1;

            int loSrcIdx = 0;
            for (int loIdx = 0; loIdx < iPictureMask.Length;)
            {
                string loNumStr = "";
                if (iPictureMask.Substring(loIdx).StartsWith("MM"))
                {
                    // Month number fixed at 2 chars 
                    loNumStr = SafeSubString(iDateString, loSrcIdx, 2);
                    if ((loNumStr.Equals(string.Empty)) || (loNumStr.Equals(" ")))
                        return invalidMonth;
                    try
                    {
                        oMonth = Convert.ToInt32(loNumStr);
                    }
                    catch
                    {
                        return invalidMonth;
                    }
                    loIdx += 2;
                    loSrcIdx += 2;
                    continue;
                }

                if (iPictureMask.Substring(loIdx).StartsWith("mm"))
                {
                    // Month number , 1 or 2 digits 
                    loNumStr = SafeSubString(iDateString, loSrcIdx, 2);
                    if ((loNumStr[1] > '9') || (loNumStr[1] < '0'))
                    {
                        // only one digit of month 
                        loNumStr = loNumStr.Remove(1, 1);
                        loSrcIdx++;
                    }
                    else
                        loSrcIdx += 2;

                    if ((loNumStr.Equals(string.Empty)) || (loNumStr.Equals(" ")))
                        return invalidMonth;
                    try
                    {
                        oMonth = Convert.ToInt32(loNumStr);
                    }
                    catch
                    {
                        return invalidMonth;
                    }
                    loIdx += 2; // on to next token 
                    continue;
                }

                if (iPictureMask.Substring(loIdx).StartsWith("MONTH"))
                {
                    // full Month name.  
                    for (int loMonthArrayIdx = 1; loMonthArrayIdx <= 12; loMonthArrayIdx++)
                    {
                        if (iDateString.Substring(loSrcIdx).StartsWith(monthFullNames[loMonthArrayIdx]))
                        {
                            oMonth = loMonthArrayIdx;
                            break;
                        }
                    }

                    // did we find a valid month? 
                    if (oMonth > 12)
                        return invalidMonth; // invalid month

                    loSrcIdx += monthFullNames[oMonth].Length;
                    loIdx += 5;
                    continue;
                }

                if (iPictureMask.Substring(loIdx).StartsWith("MON"))
                {
                    // abreviated Month name 
                    for (int loMonthArrayIdx = 1; loMonthArrayIdx <= 12; loMonthArrayIdx++)
                    {
                        if (iDateString.Substring(loSrcIdx).StartsWith(monthAbrevNames[loMonthArrayIdx]))
                        {
                            oMonth = loMonthArrayIdx;
                            break;
                        }
                    }

                    // did we find a valid month? 
                    if (oMonth > 12)
                        return invalidMonth;

                    loSrcIdx += monthAbrevNames[oMonth].Length;
                    loIdx += 3;
                    continue;
                }

                if ((iPictureMask.Substring(loIdx).StartsWith("ddd")) ||
                    (iPictureMask.Substring(loIdx).StartsWith("DDD")))
                {
                    // This is a julian day, which means its the number of days in the year (so Jan 10 would = 10 but
                    // Feb 10 would = 41).
                    loNumStr = SafeSubString(iDateString, loSrcIdx, 3);
                    try
                    {
                        dayOfYear = Convert.ToInt32(loNumStr.Trim());
                    }
                    catch
                    {
                        return invalidDay;
                    }
                    loIdx += 3;
                    loSrcIdx += 3;
                    continue;
                }

                if (iPictureMask.Substring(loIdx).StartsWith("DD"))
                {
                    // Day number fixed at 2 chars 
                    loNumStr = SafeSubString(iDateString, loSrcIdx, 2);
                    try
                    {
                        oDayNo = Convert.ToInt32(loNumStr);
                    }
                    catch
                    {
                        return invalidDay;
                    }
                    loIdx += 2;
                    loSrcIdx += 2;
                    continue;
                }

                if (iPictureMask.Substring(loIdx).StartsWith("dd"))
                {
                    // day number , 1 or 2 digits 
                    loNumStr = SafeSubString(iDateString, loSrcIdx, 2);
                    if ((loNumStr[1] > '9') || (loNumStr[1] < '0'))
                    {
                        // only one digit of month 
                        loNumStr = loNumStr.Remove(1, 1);
                        loSrcIdx++;
                    }
                    else
                        loSrcIdx += 2;

                    try
                    {
                        oDayNo = Convert.ToInt32(loNumStr);
                    }
                    catch
                    {
                        return invalidDay;
                    }
                    loIdx += 2;
                    continue;
                }

                if ((iPictureMask.Substring(loIdx).StartsWith("YYYY")) ||
                    (iPictureMask.Substring(loIdx).StartsWith("yyyy")))
                {
                    // 4 digit year 
                    try
                    {
                        loNumStr = SafeSubString(iDateString, loSrcIdx, 4);
                        oYear = Convert.ToInt32(loNumStr);
                    }
                    catch
                    {
                        return invalidYear;
                    }
                    // add a century to the year (if necessary) 
                    if (oYear < 100)
                    {
                        if (oYear < 30)
                            oYear += 2000;
                        else
                            oYear += 1900;
                    }

                    // don't allow dates before 1900 & after 2099
                    if ((oYear < 1900) || (oYear > 2099))
                        return invalidYear;

                    loIdx += 4;
                    loSrcIdx += 4;
                    continue;
                }

                if ((iPictureMask.Substring(loIdx).StartsWith("YY")) ||
                    (iPictureMask.Substring(loIdx).StartsWith("yy")))
                {
                    // 2 digit year 
                    try
                    {
                        loNumStr = SafeSubString(iDateString, loSrcIdx, 2);
                        oYear = Convert.ToInt32(loNumStr);
                    }
                    catch
                    {
                        return invalidYear;
                    }
                    // add a century to the year 
                    if (oYear < 30)
                        oYear += 2000;
                    else
                        oYear += 1900;
                    loIdx += 2;
                    loSrcIdx += 2;
                    continue;
                }
                // wasn't a token, so skip it 
                loSrcIdx++;
                loIdx++;
            }

            // If this was a julian type date (day of year instead of day and month) then will have
            // to convert the day of year into a month and day.
            if ((oMonth == 1) && (oDayNo == 1) && (dayOfYear > 0))
            {
                // Probably a better way to get this but dont know of one yet. For now, we will
                // get a close estimate of what month it would be and then keep adding days 
                // until we get the exact day of year
                /*
                DateTime findDate = new DateTime(oYear, Convert.ToInt32(dayOfYear / 29), 1);
                while (findDate.DayOfYear != dayOfYear)
                {
                    // If we ever go past the actual year then something went wrong and we'll just
                    // return invalid day error.
                    if (findDate.Year > oYear) { return InvalidDay; }
                    findDate = findDate.AddDays(1);
                }
                */
                // Start with Jan 1st of given year, then simply add dayOfYear minus 1 day
                var findDate = new DateTime(oYear, 1, 1);
                findDate = findDate.AddDays(dayOfYear - 1);

                // Ok, got exact date, get the month and day from it.
                oMonth = findDate.Month;
                oDayNo = findDate.Day;
            }

            // make sure we have a valid date 
            if ((oMonth < 1) || (oMonth > 12))
                return invalidMonth;
            if (oDayNo <= 0)
                return invalidDay;
            if (oYear < 0)
                return invalidYear;
            if (oDayNo > DateTime.DaysInMonth(oYear, oMonth))
                return invalidDay;
            return 0;
        }

        public static int DateStringToOSDate(string iPictureMask, string iDateString, ref DateTime oOSDate)
        {
            int loDayNo = 0;
            int loMonth = 0;
            int loYear = 0;
            int loResult = 0;

            if (iDateString == "" || iPictureMask == "")
                return -1;
            // 1st convert to DMY 
            if ((loResult = DateStringToDMY(iPictureMask, iDateString, ref loDayNo, ref loMonth, ref loYear)) < 0)
                return loResult; // failed! 
            // now from DMY to OSDate 
            oOSDate = new DateTime(loYear, loMonth, loDayNo);
            return 0;
        }

        public static int TimeStringToOSTime(string iPictureMask, string iTimeString, ref DateTime oOSTime)
        {
            int loHour = 0;
            int loSecond = 0;
            int loMinute = 0;
            int loResult = 0;

            if (iTimeString == "" || iPictureMask == "")
                return -1;

            if ((loResult = TimeStringToHMS(iPictureMask, iTimeString, ref loHour, ref loMinute, ref loSecond)) < 0)
                return loResult; // time conversion failed 
            oOSTime = new DateTime(2000, 1, 1, loHour, loMinute, loSecond);
            return 0;
        }

        /// <summary>
        ///     Converts a formated time string to an OSTime. Accepted mask tokens are:
        ///     MM,mm        - minute left padded with 0 to 2 digits
        ///     HH,hh        - hour left padded with 0 to 2 digits. (12 hr in presence of TT/tt, 24 hr in absence)
        ///     SS,ss        - seconds left padded with 0 to 2 digits
        ///     TT           - Uppercase AM/PM
        ///     tt           - Lowercase am/pm
        /// </summary>
        public static int TimeStringToHMS(string iPictureMask, string iTimeString, ref int oHour,
                                          ref int oMinute, ref int oSecond)
        {
            const int InvalidHour = -4;
            const int InvalidMinute = -5;
            const int InvalidSecond = -6;
            int loAMPMTime = 0;

            string loNumStr = "";

            oHour = 0;
            oMinute = 0;
            oSecond = 0;

            if (iPictureMask == "")
                return -1;

            int loSrcIdx = 0;
            for (int loIdx = 0; loIdx < iPictureMask.Length;)
            {
                if ((iPictureMask.Substring(loIdx).StartsWith("HH")) ||
                    (iPictureMask.Substring(loIdx).StartsWith("hh")) ||
                    (iPictureMask.Substring(loIdx).ToUpper().StartsWith("HH")))
                {
                    // hour fixed at 2 chars 
                    loNumStr = SafeSubString(iTimeString, loSrcIdx, 2);
                    if (loNumStr.Equals(string.Empty))
                        return InvalidHour;
                    try
                    {
                        oHour = Convert.ToInt32(loNumStr);
                    }
                    catch
                    {
                        return InvalidHour;
                    }
                    loIdx += 2;
                    loSrcIdx += 2;
                    continue;
                }

                if ((iPictureMask.Substring(loIdx).StartsWith("MM")) ||
                    (iPictureMask.Substring(loIdx).StartsWith("mm")) ||
                    (iPictureMask.Substring(loIdx).ToUpper().StartsWith("MM")))
                {
                    // minute fixed at 2 chars 
                    loNumStr = SafeSubString(iTimeString, loSrcIdx, 2);
                    try
                    {
                        oMinute = Convert.ToInt32(loNumStr);
                    }
                    catch
                    {
                        return InvalidMinute;
                    }
                    loIdx += 2;
                    loSrcIdx += 2;
                    continue;
                }

                if ((iPictureMask.Substring(loIdx).StartsWith("SS")) ||
                    (iPictureMask.Substring(loIdx).StartsWith("ss")) ||
                    (iPictureMask.Substring(loIdx).ToUpper().StartsWith("SS")))
                {
                    // seconds fixed at 2 chars 
                    loNumStr = SafeSubString(iTimeString, loSrcIdx, 2);
                    try
                    {
                        oSecond = Convert.ToInt32(loNumStr);
                    }
                    catch
                    {
                        return InvalidSecond;
                    }
                    loIdx += 2;
                    loSrcIdx += 2;
                    continue;
                }

                if ((iPictureMask.Substring(loIdx).StartsWith("TT")) ||
                    (iPictureMask.Substring(loIdx).StartsWith("tt")) ||
                    (iPictureMask.Substring(loIdx).ToUpper().StartsWith("TT")))
                {
                    // AM/PM indicator 
                    // Had better be AM or PM or am or pm 
                    loAMPMTime = 1;
                    if ((SafeSubString(iTimeString, loSrcIdx, 1) == "P") ||
                        (SafeSubString(iTimeString, loSrcIdx, 1) == "p"))
                        loAMPMTime++;
                    loIdx += 2;
                    loSrcIdx += 2;
                    continue;
                }

                // wasn't a token, so skip it 
                loSrcIdx++;
                loIdx++;
            }
            // add 12 to hour if it was PM 
            if (loAMPMTime > 0)
            {
                if ((oHour <= 0) || (oHour > 12))
                    return InvalidHour;

                // if PM, add 12 to hour (unless it is 12 PM) 
                // if AM set hour to 0 if it is 12. 
                if ((loAMPMTime == 2) && (oHour != 12))
                    oHour += 12;
                else if ((loAMPMTime == 1) && (oHour == 12))
                    oHour = 0; // JIRA: AUTOCITE-375
            } // if AM/PM time 

            // make sure we have a valid time 
            if ((oHour < 0) || (oHour >= 24))
                return InvalidHour;

            if ((oMinute < 0) || (oMinute >= 60))
                return InvalidMinute;

            if ((oSecond < 0) || (oSecond >= 60))
                return InvalidSecond;

            return 0;
        }

        //Get System date in string format with Time set to 00
        public static string GetDate(string date, string dateFormat, string returnedDtFmt)
        {
            try
            {
                DateTime myDate = date != null ? DateTime.ParseExact(date, dateFormat, null) : DateTime.Now;
                return myDate.ToString(returnedDtFmt);
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "time: " + date + " timeformat:" + dateFormat, "GetDate");
            }
            return date;
        }

        //Get System time in string format with Date set to 1899-12-30
        public static string GetTime(string time, string timeFormat, string returnedTimeFmt)
        {
            //todo - this is breaking
            try
            {
                 DateTime myDate = time != null ? DateTime.ParseExact(time, timeFormat, null) : DateTime.Now;
                return myDate.ToString(returnedTimeFmt);
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "time: " + time + " timeformat:" + timeFormat, "GetTime");
            }
            return time;
        }
      
    }
}