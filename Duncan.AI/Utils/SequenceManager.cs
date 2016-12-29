using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Reino.ClientConfig;

using System.Text;


namespace Duncan.AI.Utils
{
   
    // Interface for some objects that can read themselves from file
    public interface IObjReader
    {
        IObjReader ReadInObj(IObjReader primaryObj, StreamReader sr, ReinoObjectReader objReader);
        IObjReader ReadChildObjFromFile(string iObjClass, string iObjName, TextReader tr);
        void SetPropertyByName(string token1, string token2);
    }

    public class SequenceBookImp : TObjBase, IObjReader
    {
        private string _fPrefix = "";
        private string _fSuffix = "";
        private Int64 _fRangeHi = 0;
        private Int64 _fRangeLo = 0;
        private Int64 _fLastUsed = 0;
        public List<Int64> LoggedNumbers = new List<long>();

        public SequenceImp ParentSequence = null;
        public Int64 BookNumber;

        public string GetPrefix() { return _fPrefix; }
        public string GetSuffix() { return _fSuffix; }
        public Int64 GetRangeLo() { return _fRangeLo; }
        public Int64 GetRangeHi() { return _fRangeHi; }
        public Int64 GetLastUsed() { return _fLastUsed; }
        public Int64 GetCurrentBookNumber() { return BookNumber; }

        public IObjReader ReadChildObjFromFile(string iObjClass, string iObjName, System.IO.TextReader tr)
        {
            // There are no child objects available for a SequenceBookImp
            return null;
        }

        public IObjReader ReadInObj(IObjReader primaryObj, System.IO.StreamReader sr, ReinoObjectReader objReader)
        {
            return objReader.ReadInObj(this, sr, objReader);
        }

        public void SetPropertyByName(string token1, string token2)
        {
            if (String.Compare(token1, "fRangeLo", true) == 0)
            {
                if (token2 != "")
                    _fRangeLo = Convert.ToInt64(token2);
            }
            else if (String.Compare(token1, "fRangeHi", true) == 0)
            {
                if (token2 != "")
                    _fRangeHi = Convert.ToInt64(token2);
            }
            else if (String.Compare(token1, "fLastUsed", true) == 0)
            {
                if (token2 != "")
                    _fLastUsed = Convert.ToInt64(token2);
            }
            else if (String.Compare(token1, "fPrefix", true) == 0)
                _fPrefix = token2;
            else if (String.Compare(token1, "fSuffix", true) == 0)
                _fSuffix = token2;
        }

        public void SetLastUsed(Int64 iLastUsed)
        {
            Int64 loLastUsed = iLastUsed;
            Int64 loRangeHi = _fRangeHi;
            Int64 loRangeLo = _fRangeLo;
            Int64 loLoggedLastUsed = _fLastUsed;

            // eliminate check digit from comparisons
            if (ParentSequence.CheckDigitCalc != TSequenceStruct.CheckDigitType.cdNone)
            {
                loLastUsed /= 10;
                loRangeHi /= 10;
                loRangeLo /= 10;
                loLoggedLastUsed /= 10;
            }
            if ((loLastUsed > loLoggedLastUsed) && (loLastUsed >= loRangeLo) && (loLastUsed <= loRangeHi))
                _fLastUsed = iLastUsed;
        }

        public Int64 GetRemaining()
        {
            Int64 loRangeHi;
            Int64 loLastUsed;

            if (_fLastUsed > 0)
            {
                if (ParentSequence.CheckDigitCalc != TSequenceStruct.CheckDigitType.cdNone)
                {
                    loRangeHi = _fRangeHi / 10;
                    loLastUsed = _fLastUsed / 10;
                }
                else
                {
                    loRangeHi = _fRangeHi;
                    loLastUsed = _fLastUsed;
                }
            }
            else
            {
                if (ParentSequence.CheckDigitCalc != TSequenceStruct.CheckDigitType.cdNone)
                {
                    loRangeHi = _fRangeHi / 10;
                    loLastUsed = (_fRangeLo / 10) - 1;
                }
                else
                {
                    loRangeHi = _fRangeHi;
                    loLastUsed = _fRangeLo - 1;
                }
            }
            // return the calculated high and low
            return loRangeHi - loLastUsed;
        }

    }

    public class SequenceImp : TSequenceStruct, IObjReader
    {
        // mcb - patrolcar AIR support a different root folder.

        public string SeqFilename { get; set; }
        //{
        //    get
        //    {
        //        //  return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads) + "/" + this.Name + ".SEQ";
        //        return Name;
        //    }
        //}
        public string SequenceName { get; set; }

        public List<SequenceBookImp> Books = new List<SequenceBookImp>();

        private int _fCurrentBookNdx = 0;
        public long _currentBookNumber;
        public void ReadInFromFile()
        {
            // Safety check to make sure file exists
            if (File.Exists(SeqFilename) == false)
            {
                System.Diagnostics.Debug.WriteLine("No sequence file present: " + SeqFilename);
                Console.WriteLine("No sequence file present: " + SeqFilename);
                return;
            }

            try
            {
                var sr = new StreamReader(SeqFilename);
                var loSeqReader = new ReinoObjectReader();
                ReadInObj(this, sr, loSeqReader);
                sr.Close();

                // loop through the books until we find one with numbers available 
                for (int loBookNdx = 0; loBookNdx < Books.Count; loBookNdx++)
                {
                    // If no numbers left, continue to the next book. Otherwise,
                    // set current book index and break out of loop
                    if (Books[loBookNdx].GetRemaining() <= 0)
                        continue;
                    _fCurrentBookNdx = loBookNdx;
                    //also set the book number 
                    _currentBookNumber = Books[loBookNdx].GetCurrentBookNumber();
                    break;
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error inside SequenceImp.ReadInFromFile(): " + Ex.Message);
            }
        }

        public void ReadInFromByteArray( byte[] iSeqFileData )
        {

            // not in there? 
            if (iSeqFileData == null)
            {
                System.Diagnostics.Debug.WriteLine("No sequence file present: " + SeqFilename);
                Console.WriteLine("No sequence file present: " + SeqFilename);
                return;
            }


            try
            {
                // todo - may need to specify encoding....
                var sr = new StreamReader(new MemoryStream(iSeqFileData), Encoding.Default);
                var loSeqReader = new ReinoObjectReader();
                ReadInObj(this, sr, loSeqReader);
                sr.Close();

                // loop through the books until we find one with numbers available 
                for (int loBookNdx = 0; loBookNdx < Books.Count; loBookNdx++)
                {
                    // If no numbers left, continue to the next book. Otherwise,
                    // set current book index and break out of loop
                    if (Books[loBookNdx].GetRemaining() <= 0)
                        continue;
                    _fCurrentBookNdx = loBookNdx;
                    //also set the book number 
                    _currentBookNumber = Books[loBookNdx].GetCurrentBookNumber();
                    break;
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error inside SequenceImp.ReadInFromByteArray(): " + Ex.Message);
            }
        }

        public IObjReader ReadInObj(IObjReader primaryObj, StreamReader sr, ReinoObjectReader objReader)
        {
            return objReader.ReadInObj(this, sr, objReader);
        }

        public IObjReader ReadChildObjFromFile(string iObjClass, string iObjName, TextReader tr)
        {
            // The only object we need to look for is a TIssueNoBook
            if (String.Compare(iObjClass, "TIssueNoBook", true) == 0)
            {
                // Create new book object, set name and parent sequence, then add to list of books
                var loBook = new SequenceBookImp();
                loBook.Name = iObjName;
                loBook.ParentSequence = this;
                this.Books.Add(loBook);
                return loBook;
            }
            // If we get here, its some object we know nothing about
            return null;
        }

        public void SetPropertyByName(string token1, string token2)
        {
            // Is this one of the possible aliases for the CheckDigitCalc?
            if ((String.Compare(token1, "fCheckDigitCalc", true) == 0) ||
                (String.Compare(token1, "CheckDigitCalc", true) == 0))
            {
                // Set the CheckDigitCalc according to the name
                if (String.Compare(token2, "cdNone", true) == 0)
                    CheckDigitCalc = CheckDigitType.cdNone;
                else if (String.Compare(token2, "cdMod7", true) == 0)
                    CheckDigitCalc = CheckDigitType.cdMod7;
                else if (String.Compare(token2, "cdMod10", true) == 0)
                    CheckDigitCalc = CheckDigitType.cdMod10;
                else if (String.Compare(token2, "cdMod10OddRJ", true) == 0)
                    CheckDigitCalc = CheckDigitType.cdMod10OddRJ;
                else if (String.Compare(token2, "cdMod10CDW2", true) == 0)
                    CheckDigitCalc = CheckDigitType.cdMod10CDW2;
                else if (String.Compare(token2, "cdMod10Hobart", true) == 0)
                    CheckDigitCalc = CheckDigitType.cdMod10Hobart;
                else if (String.Compare(token2, "cdMod10Mississauga", true) == 0)
                    CheckDigitCalc = CheckDigitType.cdMod10Mississauga;
                else if (String.Compare(token2, "cdMod11", true) == 0)
                    CheckDigitCalc = CheckDigitType.cdMod11;
                else if (String.Compare(token2, "cdMod11Dade", true) == 0)
                    CheckDigitCalc = CheckDigitType.cdMod11Dade;
                else if (String.Compare(token2, "cdHennepinCnty", true) == 0)
                    CheckDigitCalc = CheckDigitType.cdHennepinCnty;
                else if (String.Compare(token2, "cdPGeorgeMod10", true) == 0)
                    CheckDigitCalc = CheckDigitType.cdPGeorgeMod10;
                else if (String.Compare(token2, "cdSaltLakeCityMod10", true) == 0)
                    CheckDigitCalc = CheckDigitType.cdSaltLakeCityMod10;
                // property is finished, so exit
                return;
            }

            // Is this the "LOG" property?
            if (String.Compare(token1, "LOG", true) == 0)
            {
                // Nothing special we need to do for this property, so just exit
                return;
            }

            // If we get this far, we should be dealing with a BookName and a used citation number.
            // Try to find the Book object by name (case-insensitive)
            var predicate = new TObjBasePredicate(token1);
            SequenceBookImp loBook = this.Books.Find(predicate.CompareByName_CaseInsensitive);
            // If we got a book, set the last used number
            if (loBook != null)
                loBook.SetLastUsed(Convert.ToInt64(token2));

            // No other properties recognized at this level, so just exit
        }


        /// <summary>
        /// Logs the use of an issue number by making an entry in the issue number sequence file.
        /// The entry is in the format "bookname" "Used Number".  Only the number portion is written;
        /// the prefix and suffix are not written.
        /// </summary>
        public int LogUsedNumber(Int64 iUsedNumber)
        {
            // Try to get the current book
            SequenceBookImp loBook = this.Books[_fCurrentBookNdx];
            if (loBook == null)
                return -1;

            // Mark passed number as used
            loBook.SetLastUsed(iUsedNumber);

            // Write to file if hasn't already been done
            if (loBook.LoggedNumbers.IndexOf(iUsedNumber) == -1)
            {
                // Create stream writer in APPEND mode
                var sw = new StreamWriter(SeqFilename, true);
                // Output used number in format: Uppercase book name + 2 spaces + used number
                sw.WriteLine(loBook.Name.ToUpper() + "  " + iUsedNumber.ToString());
                // Properly close the file stream
                sw.Flush();
                sw.Close();
                //sw.Dispose(); // Not supported by Compact Framework

                // Add the number to internal list of logged numbers
                loBook.LoggedNumbers.Add(iUsedNumber);
            }

            // If we used up the current book, move on to the next. 
            if (loBook.GetRemaining() <= 0)
                _fCurrentBookNdx++;
            return 0;
        }

        // Finds the next available issue number.
        public Int64 GetNextNumber()
        {
            // Return if the current book index is invalid
            if ((_fCurrentBookNdx < 0) || (_fCurrentBookNdx >= Books.Count))
                return 0;

            Int64 loNextNumber;
            SequenceBookImp loBook = this.Books[_fCurrentBookNdx];

            // If any used in this book, get next number
            if (loBook.GetLastUsed() > 0)
            {
                loNextNumber = loBook.GetLastUsed();
                if (CheckDigitCalc == CheckDigitType.cdNone)
                    loNextNumber++;
                else
                    loNextNumber += 10;
            }
            else // start at beginning of this book
                loNextNumber = loBook.GetRangeLo();

            // do we need to tack on a check digit?
            if (CheckDigitCalc == CheckDigitType.cdNone)
                return loNextNumber;

            // need to calc check digit. Check digit routines require
            switch (CheckDigitCalc)
            {
                case CheckDigitType.cdNone: return loNextNumber;
                case CheckDigitType.cdMod10: return CalcMod10CheckDigit(loNextNumber, false);
                case CheckDigitType.cdMod10OddRJ: return CalcMod10OddRJCheckDigit(loNextNumber);
                case CheckDigitType.cdMod7: return CalcMod7CheckDigit(loNextNumber);
                case CheckDigitType.cdHennepinCnty: return CalcHennepinCntyCheckDigit(loNextNumber);
                case CheckDigitType.cdMod11Dade: return CalcMod11DadeCheckDigit(loNextNumber);
                case CheckDigitType.cdMod10CDW2: return CalcMod10CheckDigit(loNextNumber, true);
                case CheckDigitType.cdMod10Hobart: return CalcMod10HobartCheckDigit(loNextNumber);
                case CheckDigitType.cdMod10Mississauga: return CalcMod10MississaugaCheckDigit(loNextNumber);
                case CheckDigitType.cdMod11: return CalcMod11CheckDigit(loNextNumber);
                case CheckDigitType.cdPGeorgeMod10: return CalcPGeorgeMod10(loNextNumber);
                case CheckDigitType.cdSaltLakeCityMod10: return CalcSaltLakeCityMod10(loNextNumber);
                default: return -1;
            }
        }

        // Finds the next available issue number.
        public string GetNextNumberPfx()
        {
            // Return if the current book index is invalid
            if ((_fCurrentBookNdx < 0) || (_fCurrentBookNdx >= Books.Count))
                return "";

            SequenceBookImp loBook = this.Books[_fCurrentBookNdx];
            return loBook.GetPrefix();
        }

        // Finds the next available issue number.
        public string GetNextNumberSfx()
        {
            // Return if the current book index is invalid
            if ((_fCurrentBookNdx < 0) || (_fCurrentBookNdx >= Books.Count))
                return "";

            SequenceBookImp loBook = this.Books[_fCurrentBookNdx];
            return loBook.GetSuffix();
        }

        /// <summary>
        /// Makes sure that fLastIssued of the book that contains the passed number is at or 
        /// beyond the passed number. This is used as a redundant check on the sequence file.  
        /// The sequence file has been known to get out of phase with the data in the handheld, 
        /// causing numbers to be reused.
        /// </summary>
        public void VerifyUsedNumberIsLogged(string iPfx, string iSfx, Int64 iNumber)
        {
            int loBookNdx;
            // Find which book contains this number
            for (loBookNdx = 0; loBookNdx < Books.Count; loBookNdx++)
            {
                SequenceBookImp loBook = Books[loBookNdx];
                if ((iNumber < loBook.GetRangeLo()) || (iNumber > loBook.GetRangeHi())) continue;
                if (string.Compare(iSfx, loBook.GetSuffix()) != 0) continue;
                if (string.Compare(iPfx, loBook.GetPrefix()) != 0) continue;

                // It's in this book. Has it been logged?
                if (iNumber <= loBook.GetLastUsed()) return; // It's been logged, nothing to do.

                // In this book and hasn't been logged. Log it!
                _fCurrentBookNdx = loBookNdx; // Needed by LogUsedNumber
                LogUsedNumber(iNumber);
                return;
            }
        }

        /*
         * Result is such that sum of all digits multiplied by respective weight is evenly divisible by
         * 10.  Weights are assigned as follows: check digit has weight 1.  Digits in iNumberString
         * are assigned (from least to most significant, or right to left) alternating weights of 2 and 1,
         * starting with 2.
         */
        Int64 CalcMod10HobartCheckDigit(Int64 iSourceNumber)
        {
            const int HOBART_WEIGHT_COUNT = 8;
            int[] loWeights = new int[HOBART_WEIGHT_COUNT] { 1, 9, 8, 7, 4, 3, 2, 1 };
            Int64 loQuot;
            Int64 loRem;
            Int64 loSum = 0;
            int loNdx;

            // chop off the last digit
            iSourceNumber /= 10;

            // work from least significant to most
            // Don't use div function because it is not supported on all platforms for __int64
            for (loQuot = iSourceNumber, loNdx = HOBART_WEIGHT_COUNT - 2;
                (loQuot != 0) && (loNdx >= 0); loNdx--)
            {
                loRem = loQuot % 10;
                loQuot /= 10;
                loSum += loRem * loWeights[loNdx]; // add the digit-weight product to the sum
            }

            // mod intermediate sum by 10
            loSum %= 10;

            // check digit is 10 - remainder, unless remainder is 0
            if (loSum > 0) loSum = 10 - loSum;

            // loSum is now the check digit. Append it to the result
            iSourceNumber *= 10; // add a blank digit
            iSourceNumber += loSum; // and add the check digit.
            return iSourceNumber;
        }

        private void CalcMod10OddRJCheckDigitStr(ref string ioSrcStr)
        {
            int loLen;
            int loSum = 0;
            int loNdx;
            int loWeightIsTwo = 1;
            int quot = 0;
            int rem = 0;
            int loCheckDigit = 0;

            loLen = ioSrcStr.Length;

            for (loNdx = loLen - 2; loNdx >= 0; loNdx--)
            {
                // ignore spaces
                if ((ioSrcStr[loNdx] > '9') || (ioSrcStr[loNdx] < '0')) continue;

                quot = ((ioSrcStr[loNdx] - 0x30) * (1 + loWeightIsTwo)) / 10;
                rem = ((ioSrcStr[loNdx] - 0x30) * (1 + loWeightIsTwo)) % 10;
                loSum += quot + rem;
                if (loWeightIsTwo == 1)
                    loWeightIsTwo = 0;
                else
                    loWeightIsTwo = 1;
            }

            // Only need to work with least significant digit now (right-most digit)
            while (loSum >= 10)
            {
                loSum = loSum - 10;
            }

            // if the remaining value isn't zero, "inverse" it
            if (loSum > 0)
            {
                loSum = 10 - loSum;
            }

            loCheckDigit = loSum;
            ioSrcStr = ioSrcStr.Substring(0, ioSrcStr.Length - 1) + loCheckDigit.ToString();
        }

        private Int64 CalcMod10OddRJCheckDigit(Int64 iSourceNumber)
        {
            string loSrcNumberStr = Convert.ToString(iSourceNumber);
            CalcMod10OddRJCheckDigitStr(ref loSrcNumberStr);
            iSourceNumber = Convert.ToInt64(loSrcNumberStr);
            return iSourceNumber;
        }


        /*
         * Fixed v1.07, 11/6/03.
         * Prior version did not sum the digits of the intermediate weighted sums. 
         * For example, digit "7" with weight "2" should be 5 (2 * 7 = 14, 1 + 4 =5), not 14 (just 2 * 7)
         */
        void CalcMod10CheckDigitStr(ref string ioSrcStr, bool iCDW2)
        {
            int loLen;
            int loSum = 0;
            int loNdx;
            int loWeightIsTwo;
            int quot = 0;
            int rem = 0;
            int loCheckDigit = 0;

            loLen = ioSrcStr.Length;
            if (iCDW2 == true)
                loWeightIsTwo = 0;
            else
                loWeightIsTwo = 1;

            for (loNdx = loLen - 2; loNdx >= 0; loNdx--)
            {
                // ignore spaces
                if (ioSrcStr[loNdx] == ' ')
                    continue;
                quot = ((ioSrcStr[loNdx] - 0x30) * (1 + loWeightIsTwo)) / 10;
                rem = ((ioSrcStr[loNdx] - 0x30) * (1 + loWeightIsTwo)) % 10;
                loSum += quot + rem;
                if (loWeightIsTwo == 1)
                    loWeightIsTwo = 0;
                else
                    loWeightIsTwo = 1;
            }

            loCheckDigit = 10 - (loSum % 10);
            if (loCheckDigit == 10) loCheckDigit = 0;
            ioSrcStr = ioSrcStr.Substring(0, ioSrcStr.Length - 1) + loCheckDigit.ToString();
        }

        /*
         * Result is such that sum of all digits multiplied by respective weight is evenly divisible by
         * 10.  Weights are assigned as follows: check digit has weight 1.  Digits in iNumberString
         * are assigned (from least to most significant, or right to left) alternating weights of 2 and 1,
         * starting with 2.
         */
        Int64 CalcMod10CheckDigit(Int64 iSourceNumber, bool iCDW2)
        {
            string loSrcNumberStr = Convert.ToString(iSourceNumber);
            CalcMod10CheckDigitStr(ref loSrcNumberStr, iCDW2);
            iSourceNumber = Convert.ToInt64(loSrcNumberStr);
            return iSourceNumber;
        }

        /*
         * Result is such that number before the check digit + the check digit - 7 mod 7 == 0
         */
        Int64 CalcMod7CheckDigit(Int64 iSourceNumber)
        {
            Int64 loCheckDigit = 0;

            // chop off the last digit
            iSourceNumber /= 10;

            // check digit number mod 7
            loCheckDigit = (iSourceNumber % 7);
            // add a digit back to source.
            iSourceNumber *= 10;
            iSourceNumber += loCheckDigit;
            return iSourceNumber;
        }

        /* 
         * Nice Algorithm:
         * 1. Number length must be 10.
         * 2. Sum all the even digits (excluding the last)
         * 3. For the Odd digits, sum depending on digit value as follows:
         *    0: add self (do nothing)
         *    1-4: add self.
         *    5: add 1
         *    6: add 3
         *    7: add 5
         *    8: add 7
         *    9: add 9
         *    (Turns out this is "digit - 5" times 2, then add one.)
         *
         * 4. Take the last digit of the running sum. If it is 0, the check digit is also 0, otherwise the check digit is
         *    this digit subtracted from 10.
         */
        Int64 CalcHennepinCntyCheckDigit(Int64 iSourceNumber)
        {
            int loDigitNo;
            Int64 loSum = 0;
            Int64 loSrcNo;
            Int64 loDigit;

            // chop the last digit off iSourceNumber, will replace it w/ calculated check digit.

            iSourceNumber /= 10;

            for (loSrcNo = iSourceNumber, loDigitNo = 9; (loSrcNo != 0) && (loDigitNo != 0); loSrcNo /= 10, loDigitNo--)
            {
                loDigit = loSrcNo % 10;

                // digits in even positions are merely added to the running sum.
                if (((loDigitNo & 1) == 0))
                {
                    loSum += loDigit;
                    continue;
                }

                // digits in odd positions <= 4 are doubled then added to the running sum.
                if (loDigit <= 4)
                    loSum += loDigit * 2;
                else // remaining digits (in odd positions, > 4) have 5 subtracted, are doubled, incremented by 1, then added to running sum
                    loSum += ((loDigit - 5) * 2) + 1;
            }

            // the check digit the complement of the last digit in the sum mod 10.
            if ((loDigit = loSum % 10) != 0) loDigit = 10 - loDigit;

            // make room for the check digit on the number...
            iSourceNumber *= 10;
            // ...and add the check digit
            iSourceNumber += loDigit;

            return iSourceNumber;
        }

        /*
         * Algorithm:
         * 1. Multiply each digit by its respective multiplier, then add to a running total.
         *    NOTE: the pascal code declared the multipliers in order of least to most significant digit. It converted the number
         *          to a string, then started at the end of it and worked backwards to the beginning.  The weights were
         *          traversed in ascending order.
         * 2. Sum all the even digits (excluding the last)
         * 3. Mod the sum by 11.  If the result is less than 2, the check digit is 0, otherwise it is 11 - result.
         */
        Int64 CalcMod11DadeCheckDigit(Int64 iSourceNumber)
        {
            string loSrcNumberStr;
            const int Mod11DadeWeightCnt = 10;
            int[] loWeights = new int[Mod11DadeWeightCnt] { 2, 3, 4, 5, 6, 7, 2, 3, 4, 5 };
            int loDigitNdx;
            int loWeightNdx;
            int loLen;
            int loSum = 0;

            // chop the last digit off iSourceNumber, will replace it w/ calculated check digit.
            iSourceNumber /= 10;

            loSrcNumberStr = Convert.ToString(iSourceNumber);
            loLen = loSrcNumberStr.Length;

            // sum the weighted digits. Note that the weights are ordered from least significant to most, hence the need
            // to traverse the string and the weights in opposite order
            for (loDigitNdx = loLen - 1, loWeightNdx = 0; (loDigitNdx >= 0) && (loWeightNdx < Mod11DadeWeightCnt); loDigitNdx--, loWeightNdx++)
            {
                loSum += (loSrcNumberStr[loDigitNdx] - 0x30) * loWeights[loWeightNdx];
            }

            loSum %= 11; // do the mod 11

            // complement it if result is > 1
            if (loSum < 2) loSum = 0;
            else loSum = 11 - loSum;

            // append it
            //loSrcNumberStr = loSrcNumberStr + loSum + 0x30;  wrong translation from c++
            loSrcNumberStr = loSrcNumberStr + loSum.ToString();

            // back to an integer
            iSourceNumber = Convert.ToInt64(loSrcNumberStr);
            return iSourceNumber;
        }

        /* CalcMod10MississaugaCheckDigit     02.16.2005 ajw
         *
         * Algorithm:
         * 1. Starting with the right-most digit and working toward left, form a number
         *    from *every other* digit:
         *
         *       47268
         *       \ | /
         *        \|/
         *        428
         *
         * 2. Multiply this number times 2
         *
         *       2 x 428 = 856
         *
         * 3. Sum the digits of this new number and the digits in the
         *    original number *not* multiplied by 2:
         *
         *       856  47268
         *       |\ \  | |
         *       | \ \ | |
         *       8+5+6+7+6 = 32
         *
         * 4. Subract this number from the next higher number ending in zero
         *    to form the check digit
         *
         *       40 - 32 = 8
         *
         *    If the sum had ended in 0, say 30, the difference would be 10, in which
         *    case 0 is used for the check digit
         *
         * 5. Append the check digit to the original number to form the self checking number:
         *
         *       472688
         *
         // test samples, these already include the correct check digit
         Int64 loTestResult = CalcMod10MississaugaCheckDigit( 472688 );
         loTestResult = CalcMod10MississaugaCheckDigit( 12345674 );
         loTestResult = CalcMod10MississaugaCheckDigit( 12345682 );
         loTestResult = CalcMod10MississaugaCheckDigit( 12345690 );
         loTestResult = CalcMod10MississaugaCheckDigit( 12345708 );
         loTestResult = CalcMod10MississaugaCheckDigit( 12345716 );
         */
        Int64 CalcMod10MississaugaCheckDigit(Int64 iSourceNumber)
        {
            const int MaxMississaugaDigits = 25;
            string loSrcNumberStr = "";
            string loWeightDblStr = "";
            string loWeightRawStr = "";
            int loDigitNdx;
            int loWeightNdx;
            int loLen;
            int loSum = 0;
            Int64 loWeightNum;
            //DEBUG -- Should the default value for loAddToDblStr be "true" or "false"? Let's test this one to be sure...
            bool loAddToDblStr = true;

            // chop the last digit off iSourceNumber, will replace it w/ calculated check digit.
            iSourceNumber /= 10;

            loSrcNumberStr = Convert.ToString(iSourceNumber);
            loLen = loSrcNumberStr.Length;

            // Step 1. Starting with the right-most digit and working toward left,
            // form a number from *every other* digit; at the same time, build a
            // number from those digits not included in the first
            for (loDigitNdx = loLen - 1, loWeightNdx = 0, loAddToDblStr = true; (loDigitNdx >= 0) && (loWeightNdx < MaxMississaugaDigits); loDigitNdx--)
            {
                // determine which string to add this digit to
                if (loAddToDblStr == true)
                    loWeightDblStr = loSrcNumberStr[loDigitNdx] + loWeightDblStr;
                else
                    loWeightRawStr = loSrcNumberStr[loDigitNdx] + loWeightRawStr;

                // toggle the dest
                loAddToDblStr = !loAddToDblStr;
            }

            // Step 2. convert to an integer and multiply by 2
            loWeightNum = Convert.ToInt64(loWeightDblStr);
            loWeightNum = loWeightNum * 2;

            // Step 3. sum the digits of this new number and the digits of the
            // original number *not* multiplied by 2
            loWeightDblStr = Convert.ToString(loWeightNum);
            loLen = loWeightDblStr.Length;
            loSum = 0;
            for (loDigitNdx = 0; (loDigitNdx < loLen); loDigitNdx++)
                loSum += (loWeightDblStr[loDigitNdx] - 0x30);
            loLen = loWeightRawStr.Length;
            for (loDigitNdx = 0; (loDigitNdx < loLen); loDigitNdx++)
                loSum += (loWeightRawStr[loDigitNdx] - 0x30);

            // Step 4. subtract this number from the next highest number ending in 0
            loSum %= 10; // we'll accomplish it by first doing a mod 10
            if (loSum > 0)
                loSum = (10 - loSum); // and then inverting it if its not already 0

            // append it to the original string (the placeholder has already been removed)
            loLen = loSrcNumberStr.Length;

            //loSrcNumberStr = loSrcNumberStr + loSum + 0x30;  wrong translation from c++
            loSrcNumberStr = loSrcNumberStr + loSum.ToString();


            // back to an integer
            iSourceNumber = Convert.ToInt64(loSrcNumberStr);
            return iSourceNumber;
        }

        /*
         *  CalcMod11 02.18.2005 ajw
         *
         * APPENDIX E -	MODULUS 11 CHECK DIGIT ROUTINE
         *
         * Example
         * 1. Take the 9 digit infringement number and divide it by 11	
            *      172000253 divided by 11 = 15636386.63
            *              4                          .73      8
                           5                          .82      9 
                           6                          .91     10
                           7                           0       0
                           8                          .09      1
                           9                          .18      2 
                          60                          .27      3
                          61                          .36      4
                          62                          .45      5  
                          63                          .55      6
         * 2. Round the remainder up to the nearest whole number	
            *      the remainder of .63 is rounded to 7
            * 
         * 3. Subtract the remainder from 11	
            *      11 minus 7
            * 
         * 4. The result is the check digit	4
         *
         * NB: if the remainder >= 10 or is 0 then check digit = 0

           // test samples, these already include the correct check digit
         int loTestResult = CalcMod11CheckDigit( 7606743531 );
            loTestResult = CalcMod11CheckDigit( 7606743504 );
            loTestResult = CalcMod11CheckDigit( 7606743513 );
            loTestResult = CalcMod11CheckDigit(	7606743522 );
            loTestResult = CalcMod11CheckDigit(	7606743550 );
            loTestResult = CalcMod11CheckDigit(	7606743540 );
            loTestResult = CalcMod11CheckDigit(	1720002534 );
        */
        Int64 CalcMod11CheckDigit(Int64 iSourceNumber)
        {
            // chop the last digit off iSourceNumber, will replace it w/ calculated check digit.
            iSourceNumber /= 10;

            // Step 1 - take the infingment number and divide by 11, keeping the remainder only
            Int64 loRemainder = (iSourceNumber % 11);

            // Step 2. Round the remainder up to the nearest whole number	
            //  this is implied in the modulus step %

            // Step 3. Subtract the remainder from 11	unless its 0, 1 or > 10 already
            //         remember: they wanted us to round partial numbers up, but 
            //         modulus % took that in to account already 
            if (loRemainder < 2)
                loRemainder = 0;
            else
                loRemainder = (11 - loRemainder);

            // add a digit back to source.
            iSourceNumber *= 10;
            iSourceNumber += loRemainder;

            // return the result
            return iSourceNumber;
        }

        /* 
         * Mod 10 check digit as defined by Prince George:
         *        Each number is multiplied by a weight number. The order of the weight  
         *        numbers is important. The right-most ticket digit is multiplied by 1.  
         *        the 2nd right-most is multiplied by 7. 3rd right-most by 3. And this   
         *        pattern then repeats. So, for ticket number 123456 would match up like 
         *        1    2    3    4    5    6                                             
         *        3    7    1    3    7    1                                             
         *        --------------------------                                             
         *        3    14   3    12   35   6                                             
         *        And for ticket number 1234567 would match up like                      
         *        1    2    3    4    5    6    7                                        
         *        1    3    7    1    3    7    1                                        
         *        -------------------------------                                        
         *        1    6    21   4    15   42   7                                        
         */
        Int64 CalcPGeorgeMod10(Int64 iSourceNumber)
        {
            int[] Weights = new int[] { 1, 7, 3 };
            string IssueNumStr = Convert.ToString(iSourceNumber);
            int RunningTotal = 0;
            int WeightIdx = 0;
            int DigitMultiplier = 0;
            int DigitIdx = 0;

            // Loop backward for each character in the source string
            for (DigitIdx = IssueNumStr.Length - 1; DigitIdx >= 0; DigitIdx--)
            {
                // Get the value to use as a weighted multiplier for the current character position
                DigitMultiplier = Weights[WeightIdx];
                // Increment weight index, but reset index if it goes past max
                WeightIdx++;
                if (WeightIdx >= Weights.Length)
                    WeightIdx = 0;
                // Ok, keep track of sum of all multiplied numbers. So, in first example 
                // the running total would end up being 73 (3+14+3+12+35+6). In the 2nd  
                // example it would be 96 (1+6+21+4+15+42+7).                            
                RunningTotal = RunningTotal + (Convert.ToInt32(IssueNumStr[DigitIdx].ToString()) * DigitMultiplier);
            }

            // Use MOD 10 to get the last digit of running total. So, first example, would just 
            // have 3, second example would just have 6.                               
            RunningTotal = (RunningTotal % 10);
            // If the remaining value isn't zero, "inverse" it. So, in first example, the  
            // final check digit will be 7. In the second example, final check digit would 
            // be 4. And the final ticket numbers, with check digits, would end up being   
            // 1234567 and 12345674.                                                       
            if (RunningTotal > 0)
                RunningTotal = (10 - RunningTotal);

            // Append checkdigit to the end of the source string, then convert to numeric return value
            IssueNumStr = IssueNumStr + RunningTotal.ToString();
            return Convert.ToInt64(IssueNumStr);
        }


        Int64 CalcSaltLakeCityMod10(Int64 iSourceNumber)
        {
            int loLen;
            int loSum = 0;
            int loNdx;
            bool loWeightIsTwo = true;
            int loOneDigitWeight;
            int loBonusWeightForOddDigitsGreaterThanFour = 0;
            int loCheckDigit = 0;
            string loSourceStr = iSourceNumber.ToString();

            loLen = loSourceStr.Length;

            // we'll work right to left to keep the weighting consistent even when they change the issueno len
            for (loNdx = loLen - 2; loNdx >= 0; loNdx--)
            {
                // ignore spaces
                if ((loSourceStr[loNdx] > '9') || (loSourceStr[loNdx] < '0')) continue;


                if (loWeightIsTwo == true)
                {
                    loOneDigitWeight = 2;
                }
                else
                {
                    loOneDigitWeight = 1;
                }

                // salt lake city just adds the weighted values, not the individual digits as some alorithms do
                loSum += Convert.ToInt32(loSourceStr[loNdx].ToString()) * (loOneDigitWeight);


                // toggle
                loWeightIsTwo = !loWeightIsTwo;

                // salt lake city special twist #1: 
                // is this and odd numbered slot 1 / 3 / 5 or 7?
                if (loNdx % 2 == 0)   // we test against EVEN indexes because char arrays are zero-based
                {
                    // is this slot value greater than 4?
                    if ((loSourceStr[loNdx] - 0x30) > 4)
                    {
                        // we add one for each odd digit greater than 4
                        loBonusWeightForOddDigitsGreaterThanFour++;
                    }

                }
            }


            // SLC special twist #2: add 7 to the total
            loSum += 7;

            // Only need to work with least significant digit now (right-most digit)
            while (loSum >= 10)
            {
                loSum = loSum - 10;
            }

            // now we add the bonus weighting from SLC twist #1
            loSum += loBonusWeightForOddDigitsGreaterThanFour;


            // again, keep only the least significant digit now (right-most digit)
            while (loSum >= 10)
            {
                loSum = loSum - 10;
            }


            // if the remaining value isn't zero, "inverse" it
            if (loSum > 0)
            {
                loSum = 10 - loSum;
            }

            // now we have it
            loCheckDigit = loSum;
            return loCheckDigit;
        }
    }

    public class ReinoObjectReader
    {
        #region Regular Expression Patterns
        /*
		 *  These regular expression pattern strings are prefixed by the @ symbol which
		 *  makes these "Verbatim" string literals, which means the \ characters are not 
		 *  treated as escape sequences. However if a double quote character needs to be in
		 *  the pattern, then it needs to be escaped with a second double quote character.
		 */

        /*  [ 1st Token ]
         *  This Regexp pattern will match the 1st token in the searchable string.
         *  This pattern can handle optional leading whitespace.  
         *  Here is a break-down of the pattern meaning:
         * 		(?<=	Zero-width positive lookbehind capture. Stuff in this group is used for
         * 				matching but won't be included in the result.
         * 		^[^/]	String does not begin with /
         * 		\s*		Zero or more sequential whitespace characters (Optional leading space)
         * 		)		Ends the Zero-width positive lookbehind capture.
         * 		\S+		Matches 1 or more non-whitespace characters. (1st token)
        */
        const string pattern1stToken = @"(?<=^\s*)\S+";

        /*  [ 2nd Token ]
         *  This Regexp pattern will match the 2nd token in the searchable string.
         *  Here is a break-down of the pattern meaning:
         * 		(?<=	Zero-width positive lookbehind capture. Stuff in this group is used for
         * 				matching but won't be included in the result.
         *		^\s*	String begins with zero or more whitespace characters (Optional leading space)
         *		\S+		1 or more non-whitespace characters (1st token)
         *		\s+		1 or more whitespace characters (Seperates 1st and 2nd tokens)
         * 		)		Ends the Zero-width positive lookbehind capture.
         *		\S+		Matches 1 or more non-whitespace characters. (2nd Token)
        */
        const string pattern2ndToken = @"(?<=^\s*\S+\s+)\S+";

        /*  [ 3rd Token ]
         *  This Regexp pattern will match the 3rd token in the searchable string.
         *  Here is a break-down of the pattern meaning:
         * 		(?<=	Zero-width positive lookbehind capture. Stuff in this group is used for
         * 				matching but won't be included in the result.
         *		^\s*	String begins with zero or more whitespace characters (Optional leading space)
         *		\S+		1 or more non-whitespace characters (1st token)
         *		\s+		1 or more whitespace characters (Seperates 1st and 2nd tokens)
         *		\S+		1 or more non-whitespace characters (2nd token)
         *		\s+		1 or more whitespace characters (Seperates 2nd and 3rd tokens)
         * 		)		Ends the Zero-width positive lookbehind capture.
         *		\S+		Matches 1 or more non-whitespace characters. (3rd Token)
        */
        const string pattern3rdToken = @"(?<=^\s*\S+\s+\S+\s+)\S+";

        /*  [ 2nd Token to End ]
         *  This Regexp pattern will match the right-side of the search string starting at 
         *  the 2nd token. However it will also remove double quotes that commonly surround
         *  a property value. Note that property values can consist of multiple words, so
         *  we are not necessarily isolating just the 2nd token.
         *  Here is a break-down of the pattern meaning:
         * 		(?<=	Zero-width positive lookbehind capture. Stuff in this group is used for
         * 				matching but won't be included in the result.
         *		\s*		Zero or more whitespace characters (Optional leading space)
         *		\S+		1 or more non-whitespace characters (1st token)
         *		\s+		1 or more whitespace characters (Seperates 1st and 2nd tokens)
         *		"*		Zero or more double quote characters
         * 		)		Ends the Zero-width positive lookbehind capture.
         *		[a-zA-Z0-9]+	1 or more Alpha or Numeric digit
         *		[^\n^\r]*		Zero or more characters other than carriage-return or line-feed
         *		(?=["])			Zero-width positive lookahead for a double-quote
         *		|				OR condition (Matches expression on left or right side)
         * 		(?<=	Zero-width positive lookbehind capture. Stuff in this group is used for
         * 				matching but won't be included in the result.
         *		\s*		Zero or more whitespace characters (Optional leading space)
         *		\S+		1 or more non-whitespace characters (1st token)
         *		\s+		1 or more whitespace characters (Seperates 1st and 2nd tokens)
         *		"*		Zero or more double quote characters
         * 		)		Ends the Zero-width positive lookbehind capture.
         *		[a-zA-Z0-9]+	1 or more Alpha or Numeric digit
         *		[^\n^\r]*		Zero or more characters other than carriage-return or line-feed
        */
        const string pattern2ndTokenToEnd = @"(?<=\s*\S+\s+""*)[a-zA-Z0-9]+[^\n^\r]*(?=[""])|(?<=\s*\S+\s+""*)[a-zA-Z0-9]+[^\n^\r]*";
        #endregion

        public IObjReader ReadInObj(IObjReader PrimaryObj, System.IO.StreamReader sr, ReinoObjectReader ObjReader)
        {
            Match TokenMatch;
            string Token1 = null;
            string Token2 = null;
            string Token3 = null;
            string RecBuffer = null;
            IObjReader loResult = null;

            // loop until end-of-file or end of this object
            while (null != (RecBuffer = sr.ReadLine()))
            {
                // Start by extracting the 1st word which normally is a property name.
                // We will use a regular expression that will return the 1st word, but its
                // also smart enough not to return anything if the line from the CFG file
                // was commented out, which is a line beginning with "//".
                TokenMatch = Regex.Match(RecBuffer, pattern1stToken);

                // Ignore this record if there is no useable token (Probably a blank or commented-out line)
                if ((TokenMatch.Success == false) || (TokenMatch.Length == 0))
                    continue;

                // Initialize the tokens
                Token1 = null;
                Token2 = null;
                Token3 = null;

                // Match was successful, so lets keep the 1st token
                Token1 = TokenMatch.Value;

                // Break out of loop if reached end of object
                if (String.Compare(Token1, "end", true) == 0)
                    break;

                // Check for start of a child object
                if (String.Compare(Token1, "object", true) == 0)
                {
                    // An object's class is the 2nd token. We will use a regular expression
                    // to extract the 2nd complete token (whitespace is not allowed in class name)
                    TokenMatch = Regex.Match(RecBuffer, pattern2ndToken);
                    if (TokenMatch.Success == true)
                        Token2 = TokenMatch.Value;

                    // An object's name is the 3rd token. We will use a regular expression
                    // to extract the 3rd complete token (whitespace is not allowed in object name)
                    TokenMatch = Regex.Match(RecBuffer, pattern3rdToken);
                    if (TokenMatch.Success == true)
                        Token3 = TokenMatch.Value;

                    loResult = PrimaryObj.ReadChildObjFromFile(Token2, Token3, sr);
                    if (loResult == null)
                        return loResult;
                    loResult.ReadInObj(loResult, sr, ObjReader);
                    continue;
                }

                // At this point its safe to assume we are working with a property.
                // The property name is 1st token and value begins with the 2nd token.
                // A property value CAN consist of multiple words, and OPTIONALLY is 
                // surrounded by double quotes. We will use a regular expression to return
                // the 2nd token through the end of the line but without the surrounding 
                // quotes. 

                // TO-DO: If the property value starts wth *REG* then we will need to
                //        get the runtime value from the registry instead...

                // We already used a regular expression above to extract the 1st token,
                // which we will now treat as the property name. Now we need to use a 
                // regular expression to extract the complete property value.
                TokenMatch = Regex.Match(RecBuffer, pattern2ndTokenToEnd);
                if (TokenMatch.Success == true)
                    Token2 = TokenMatch.Value;

                // Let's watch out for NULL strings
                if (Token1 == null)
                    Token1 = "";
                if (Token2 == null)
                    Token2 = "";

                // Now set the property by name and value
                PrimaryObj.SetPropertyByName(Token1, Token2);
            }
            return PrimaryObj;
        }
    }

    public class SequenceManager
    {
        public static SequenceManager GlobalSequenceMgr = new SequenceManager();

        public List<SequenceImp> Sequences = new List<SequenceImp>();
    }
}
