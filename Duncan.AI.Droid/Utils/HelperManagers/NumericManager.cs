using System;
namespace Duncan.AI.Droid.Utils.HelperManagers
{
   public static class NumericManager
    {
        /// <summary>
        /// Numeric mask options:
        /// '-' : negative numbers allowed, signified by minus sign in 1st position
        /// '$' : currency symbol forced after negative sign.
        /// ',' : comma placed if preceding (more significant) digit exists.
        /// '.' : fixed location of decimal point.  
        /// '#' : Any digit or a decimal point.  Incompatible w/ '.'
        /// '9' : Any digit.
        /// '0' : Any digit.  string will be padded out from the decimal point w/ 0's to
        ///       this position. (left padded if before decimal point, right padded if after).
        /// '8' : Any digit.  Implied decimal point in 3rd position from right. 125.67 -> 12467, 125.6 -> 12560
        /// </summary>
        public static int FormatNumberStr(string iSrcStr, string iMask, ref string oResult)
        {
            int loSrcDecPos = -1;
            int loNumberIsZero = 1;
            int loNumberIsBlank = 1;
            int loMaxIntDigCnt = 0;
            int loMinIntDigCnt = -1;
            int loMaxFracDigCnt = 0;
            int loMinFracDigCnt = 0;
            int loFixedPnt = 0;
            int loFloatPnt = 0;
            int loNegative = 0;
            int loCurrency = 0;
            int loCommas = 0;
            int loImpliedDecimalPt = 0;

            int loResLen = 0;
            int loMaskNdx;
            int loFirstNumIdx = 0;
            string DestMask = "";
            if (iMask != null)
                DestMask = iMask;

            // Sometimes the mask might be in a string style. Under this case, we'll just
            // substitute with a character that has a meaning to us.
            if (DestMask.IndexOf("!") > -1)
            {
                DestMask = DestMask.Replace("!", "#");
            }

            // We can't do anything if there is no source string (or empty source string).
            if ((iSrcStr == null) || (iSrcStr.Length == 0) || (DestMask == null))
                return -1;

            // analyze the mask

            // if the mask is blank, leave it wide open.
            if (DestMask.Length == 0)
            {
                loFloatPnt = 1;
                loNegative = 1;
                loMaxIntDigCnt = 30;
            }

            for (loMaskNdx = 0; loMaskNdx < DestMask.Length; loMaskNdx++)
            {
                if (DestMask[loMaskNdx] == '$')
                {
                    loCurrency = 1;
                    continue;
                }

                if (DestMask[loMaskNdx] == ',')
                {
                    loCommas = 1;
                    continue;
                }

                if (DestMask[loMaskNdx] == '.')
                {
                    loFixedPnt = 1;
                    loFloatPnt = 0;
                    continue;
                }

                if (DestMask[loMaskNdx] == '8')
                {
                    loFixedPnt = 1;
                    loFloatPnt = 0;
                    loImpliedDecimalPt = 1;
                    loMaxFracDigCnt = 2;
                    loMinFracDigCnt = 2;
                    loMaxIntDigCnt = DestMask.Length - 2;
                    loMinIntDigCnt = 0; // this gets set to loMaxIntDigCnt below
                    break;
                }

                if (DestMask[loMaskNdx] == '-')
                {
                    loNegative = 1;
                    continue;
                }

                char MaskChar = DestMask[loMaskNdx];
                if (((MaskChar == '9') || (MaskChar == '#') || (MaskChar == '0') || (MaskChar == '8')) == false)
                    return -1; // invalid mask character 

                if (loFixedPnt == 0)
                {
                    // haven't encountered a decimal point yet. So this digit counts towards all the digits
                    // in a floating point number or towards the integer digits of a fixed point number 
                    if ((DestMask[loMaskNdx] == '#'))
                        loFloatPnt = 1; // '#' indicates a floating point number (decimal point can be anywhere) 

                    loMaxIntDigCnt++;
                    // a '0' indicates a "pad w/ 0". Mark the 1st 0 we encounter. 
                    if ((DestMask[loMaskNdx] == '0') && (loMinIntDigCnt == -1))
                    {
                        loMinIntDigCnt = loMaxIntDigCnt - 1;
                    }
                }
                else // a fixed point number 
                {
                    loMaxFracDigCnt++;
                    // a '0' indicates a "pad w/ 0", increment minimum number of digits 
                    if ((DestMask[loMaskNdx] == '0'))
                        loMinFracDigCnt = loMaxFracDigCnt;
                }
            } // for loop to analyze the mask 


            // loMinIntDigCnt was recorded as the 1st pad to char 'cuz didn't 
            // know how many digits would follow.  Now we know, so adjust accordingly
            if (loMinIntDigCnt == -1)
                loMinIntDigCnt = 0;
            else
                loMinIntDigCnt = loMaxIntDigCnt - loMinIntDigCnt;

            // done analyzing the mask.  Lets strip the source string down to 
            // nothing but prominent digits, a decimal point, and a negative sign

            // advance past leading white space
            int iSrcStrIdx = 0;
            while (iSrcStr[iSrcStrIdx] == ' ')
                iSrcStrIdx++;

            // 1st char can be a negative sign.
            if (iSrcStr[iSrcStrIdx] == '-')
            {
                if (loNegative == 0)
                    return -4; // mask doesn't allow negatives
                // add negative sign to output
                oResult = oResult + iSrcStr[(iSrcStrIdx++)];
                loResLen++;
                loFirstNumIdx++;
            }

            // next char can be a dollar sign 
            if (iSrcStr.Length > iSrcStrIdx)
            {
                if ((iSrcStr[iSrcStrIdx] == '$') || (loCurrency != 0))
                {
                    if (iSrcStr[iSrcStrIdx] == '$')
                        iSrcStrIdx++;
                    oResult = oResult + '$';
                    loResLen++;
                    loFirstNumIdx++;
                }
            }

            // eliminate leading 0's and commas 
            while ((iSrcStrIdx < iSrcStr.Length) && ((iSrcStr[iSrcStrIdx] == ',') || (iSrcStr[iSrcStrIdx] == '0')))
            {
                // If we found a zero, then we know the number is not a null value
                if (iSrcStr[iSrcStrIdx] == '0')
                    loNumberIsBlank = 0;
                iSrcStrIdx++;
            }

            // copy remaining digits, eliminating commas on the way.
            for (; iSrcStrIdx < iSrcStr.Length; iSrcStrIdx++)
            {
                // copy any digits 
                char SourceChar = iSrcStr[iSrcStrIdx];
                if ((SourceChar >= '0') && (SourceChar <= '9'))
                {
                    // We now know entire numeric value isn't zero
                    loNumberIsZero = 0;
                    oResult = oResult + iSrcStr[iSrcStrIdx];
                    loResLen++;
                    continue;
                }

                // if this is a decimal point (but not the second), save its position 
                if (iSrcStr[iSrcStrIdx] == '.')
                {
                    // no decimal unless a fixed or floating point number 
                    if (loFixedPnt == 0 && loFloatPnt == 0)
                        break; // just truncate it here.
                    if (loSrcDecPos != -1)
                        return -3; // can't have two decimal points either
                    loSrcDecPos = loResLen;
                    oResult = oResult + iSrcStr[iSrcStrIdx];
                    loResLen++;
                    continue;
                }

                // if this is white space, stop
                if (iSrcStr[iSrcStrIdx] == ' ')
                    break;
                //if this is a comma, swallow it. 
                if (iSrcStr[iSrcStrIdx] == ',')
                    continue;
                // anything else, we are hosed 
                return -2;
            }

            int loPrevSrcDecPos = loSrcDecPos;
            // eliminate trailing 0's if after a decimal point 
            if (loSrcDecPos >= 0)
            {
                while (oResult[loResLen - 1] == '0')
                    loResLen--;
            }

            // don't let number end w/ a decimal point 
            if ((loResLen > 0) && (loSrcDecPos == (loResLen - 1)))
                loResLen--;

            // truncate to new length if necessary
            if (loResLen < oResult.Length)
                oResult = oResult.Remove(loResLen, oResult.Length - loResLen);

            // oResult holds the source string w/ commas & leading 0's stripped out.  
            // Now merge it w/ the mask. loResLen holds the length of oResult

            // if we are dealing w/ an empty number, return null string 
            if (loNumberIsZero != 0 && loNumberIsBlank != 0)
            {
                oResult = "";
                return 0;
            }

            // if the number is 0, need to stuff it in.  The trim 0's would have removed all 0's 
            if (loNumberIsZero != 0)
            {
                oResult = oResult + "0";
                loResLen++;
            }

            loSrcDecPos = oResult.IndexOf('.');

            if ((loSrcDecPos == -1) && (loMinFracDigCnt > 0))
            {
                // fixed point number needs a decimal point
                loSrcDecPos = Math.Max(loFirstNumIdx, loPrevSrcDecPos);
                oResult = oResult.Insert(loSrcDecPos, ".");
                loResLen++;

                // left pad w/ 0's fractional portion to minimum size
                while ((loResLen - 1 - loSrcDecPos) < loMinFracDigCnt)
                {
                    oResult = oResult.Insert(loSrcDecPos + 1, "0");
                    loResLen++;
                }
            }

            if (loSrcDecPos == -1)
                loSrcDecPos = loResLen;

            // remove excess digits 
            while (loSrcDecPos - loFirstNumIdx > loMaxIntDigCnt)
            {
                oResult = oResult.Remove(loFirstNumIdx, 1);
                loResLen--;
                loSrcDecPos--;
            }

            // remove unecessary leading 0's 
            while ((loSrcDecPos - loFirstNumIdx > loMinIntDigCnt) && (oResult[loFirstNumIdx] == '0'))
            {
                // Don't remove leading zeros is the number is zero and the length doesn't exceed 1
                if ((loNumberIsZero != 0) && (oResult.Length <= 1))
                    break;
                // This is from the original ported code, but does it make any sense?
                if (loNumberIsZero != 0 && (oResult[loFirstNumIdx + 1] != '0'))
                    break;
                oResult = oResult.Remove(loFirstNumIdx, 1);
                loResLen--;
                loSrcDecPos--;
            }

            // left pad w/ 0's integer portion to minimum size
            while (loSrcDecPos - loFirstNumIdx < loMinIntDigCnt)
            {
                oResult = oResult.Insert(loFirstNumIdx, "0");
                loResLen++;
                loSrcDecPos++;
            }

            // remove excess fractional portion
            if (loFixedPnt != 0)
            {
                while ((loResLen - 1 - loSrcDecPos) > loMaxFracDigCnt)
                {
                    oResult = oResult.Remove(oResult.Length - 1, 1);
                    loResLen--;
                }

                // right pad w/ 0's fractional portion to minimum size
                if (loMinFracDigCnt > 0)
                {
                    while ((loResLen - 1 - loSrcDecPos) < loMinFracDigCnt)
                    {
                        oResult = oResult + "0";
                        loResLen++;
                    }
                }

                if (loImpliedDecimalPt != 0)
                {
                    // remove the decimal point
                    oResult = oResult.Remove(loSrcDecPos, 1);
                    loResLen--;
                }
            }

            // finally, insert comma separaters
            if (loCommas != 0)
            {
                while (loSrcDecPos - loFirstNumIdx > 3)
                {
                    loSrcDecPos -= 3;
                    oResult = oResult.Insert(loSrcDecPos, ",");
                }
            }

            // all done
            return 0;
        }

        public static int StrToDouble(string iStr, ref double oDouble)
        {
            bool loIsNegative = false;
            int loDecimalPos = -1;
            int loDigitCnt = 0;
            int loNdx = 0;

            if (iStr == "")
                return 0;

            oDouble = 0;

            // Skip past leading spaces and other non-numerics 
            for (loNdx = 0; loNdx < iStr.Length; loNdx++)
            {
                if (iStr[loNdx] < '0' || iStr[loNdx] > '9')
                {
                    // deal w/ negative numbers 
                    if (iStr[loNdx] == '-')
                        loIsNegative = true;
                }
                else
                    break;
            }

            for (; loNdx < iStr.Length; loNdx++)
            {
                if (iStr[loNdx] == ',')
                    continue; // disregard commas

                if ((iStr[loNdx] == '.') && (loDecimalPos == -1))
                {
                    loDecimalPos = loDigitCnt;  // record the position of the decimal point
                    continue;
                }
                if ((iStr[loNdx] > '9') || (iStr[loNdx] < '0'))
                    break; // invalid char 
                loDigitCnt++;
                oDouble *= 10; // multiply existing value by 10... 
                oDouble += iStr[loNdx] - 0x30; // and add next digit
            }
            if (loIsNegative)
                oDouble *= -1;

            // now divide by 10 for each digit beyond the decimal point
            for (; (loDecimalPos >= 0) && (loDecimalPos < loDigitCnt); loDecimalPos++)
                oDouble /= 10;
            return 0;
        }

        public static int StrTollInt(string iStr, ref Int64 oInt)
        {
            bool loIsNegative = false;
            int loNdx = 0;

            if (iStr == "")
                return 0;

            oInt = 0;

            // Skip past leading spaces and other non-numerics 
            for (loNdx = 0; loNdx < iStr.Length; loNdx++)
            {
                if (iStr[loNdx] < '0' || iStr[loNdx] > '9')
                {
                    // deal w/ negative numbers 
                    if (iStr[loNdx] == '-')
                        loIsNegative = true;
                }
                else
                    break;
            }

            for (; loNdx < iStr.Length; loNdx++)
            {
                if (iStr[loNdx] == ',')
                    continue; // disregard commas

                if ((iStr[loNdx] > '9') || (iStr[loNdx] < '0'))
                    break; // invalid char 
                oInt *= 10; // multiply existing value by 10... 
                oInt += iStr[loNdx] - 0x30; // and add next digit
            }
            if (loIsNegative)
                oInt *= -1;

            return 0;
        }

        public static int MaskStrToDouble(string iStr, string iMask, ref double oDouble)
        {
            int loResult = StrToDouble(iStr, ref oDouble);
            if (loResult < 0)
                return loResult;

            // only mask we are worried about is "888...", in which case the value needs to be divided by 100.
            if (iMask.IndexOf("8") >= 0)
                oDouble /= 100;
            return loResult;
        }

        public static string FixTimeEditMask(string mask)
        {
            mask = mask.Replace("TT", "tt");
            mask = mask.Replace("YYYY", "yyyy");
            mask = mask.Replace("YY", "yy");
            mask = mask.Replace("M", "m");
            return mask;
        }

        public static string FixDateEditMask(string mask)
        {
            mask = mask.Replace("WWWW", "dddd"); // day of week, full
            mask = mask.Replace("wwww", "dddd");
            mask = mask.Replace("WWW", "ddd");  // day of week, abbreviated
            mask = mask.Replace("www", "ddd");
            mask = mask.Replace("MONTH", "MMMM"); // month name, full
            mask = mask.Replace("month", "MMMM");
            mask = mask.Replace("MON", "MMM"); // month name, abbreviated
            mask = mask.Replace("mon", "MMM");
            //all lowercase m have to be uppercased
            mask = mask.Replace("m", "M");
            mask = mask.Replace("DD", "dd");
            // Next, make sure all DAY number designators are lowercase
            mask = mask.Replace('D', 'd');
            mask = mask.Replace("YYYY", "yyyy");
            mask = mask.Replace("YY", "yy");
            mask = mask.Replace('Y', 'y');

            return mask;
        }
    }
}