using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
   public class TER_VicRoadsCheckDigit : EditRestriction
    {
       #region Properties and Members
       #endregion

       public TER_VicRoadsCheckDigit()
           : base()
       {
           // Set our defaults that differ from parent.
           ActiveOnNewEntry = false;
           ActiveOnCorrection = false;
           ActiveOnReissue = false;
           ActiveOnContinuance = false;
           ActiveOnValidate = true;
           Overrideable = true;
           ControlEdit1 = "VehLicNo";
       }

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;

            if (ControlEdit1 == "")
                return false;

            /* Lifted from the C88 code.  The algorithm is as follows:
               - make sure the plate is <= 6 characters.  Return an error if not. If the plate is less than
                 6, left pad w/ spaces to 6 characters.
               - Convert each digit to a numeric value; '0'-'9' = 0-9; 'A'-'Z' are 10 + position in 
                 alphabet.  'A' is 11, 'B' is 12.. 'Z' is 37.
               - Sum the product of each converted value and its corresponding position's weight.
               - Take the remainder of the sum divided by 37.
               - The check digit is the character found in the remainder position in the string
                 '+123456789.ABCDEFGHIJKLMN-PQRSTUVWXYZ'; */

            const int FixedVicRoadsLength = 6;
            const int VicRoadsCheckDigitCnt = 37;
            const string CheckDigits = "+123456789.ABCDEFGHIJKLMN-PQRSTUVWXYZ";
            // the weight multipliers 
            int[] DigitWeights = new int[FixedVicRoadsLength] { 1, 3, 7, 1, 3, 7 };
            // the digit lookup table 
            string loPlateStr = "";
            int loCharNdx = 0;
            int loRunningTotal = 0;
            int loDigitVal = 0;
            char loCalcCheckDigit;

            loPlateStr = GetControlEdit1().GetValue();
            // make sure plate isn't too long or blank
            if (loPlateStr == "" || (loPlateStr.Length > FixedVicRoadsLength))
                return true;

            // left pad it to 6
            loPlateStr = loPlateStr.PadLeft(FixedVicRoadsLength, ' ');

            for (loCharNdx = 0; loCharNdx < FixedVicRoadsLength; loCharNdx++)
            {
                // assign a numeric value to the character between 0 and 36
                if ((loPlateStr[loCharNdx] >= '0') && (loPlateStr[loCharNdx] <= '9'))
                    loDigitVal = loPlateStr[loCharNdx] - '0';
                else if (loPlateStr[loCharNdx] == ' ')
                    loDigitVal = 10;
                else if ((loPlateStr[loCharNdx] >= 'A') && (loPlateStr[loCharNdx] <= 'Z'))
                    loDigitVal = loPlateStr[loCharNdx] - 'A' + 11;
                else
                    return true; // illegal character

                // add product of number and weight to running total
                loRunningTotal += loDigitVal * DigitWeights[loCharNdx];
            }

            // mod Running total w/ 37
            loCalcCheckDigit = CheckDigits[loRunningTotal % VicRoadsCheckDigitCnt];

            // compare the calced check digit w/ the entered one
            if (_Parent.GetValue() == loCalcCheckDigit.ToString())
                return false;

            // under no circumstances will the check digit be left wrong.  Clear is preferred.
            _Parent.SetEditBufferAndPaint("", false);
            //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();

            return true;
        }
    }
}