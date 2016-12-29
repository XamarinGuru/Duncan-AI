using System;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    public class TER_ForceCincinnatiMod7CheckDigit : EditRestriction
    {
        public TER_ForceCincinnatiMod7CheckDigit()
            : base()
        {
            // Set our defaults that differ from parent.
            ControlEdit1 = "IssueNo";
        }


        protected Int64 CalcMod7CheckDigit(Int64 iSourceNumber)
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


        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {

            int loNdx = 0;
            int loSum = 0;
            string loResult;
            string loSource;

            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;

            // make sure we have a source field
            if (CheckForControlEdit1() != 0)
                return false;

            if (GetControlEdit1().GetValue() == "")
                return false; // source is blank.

            // start with the first
            loSource = GetControlEdit1().GetValue();


            // look for a second source
            if ((ControlEdit2 != "") && (ControlEdit2Obj != null))
            {
                if (GetControlEdit2().GetValue() != "")
                {
                    // append it to the first
                    loSource = loSource + GetControlEdit2().GetValue();
                }
            }

            // while we're at it, look for a third
            if ((ControlEdit3 != "") && (ControlEdit3Obj != null))
            {
                if (GetControlEdit3().GetValue() != "")
                {
                    // append it to the first
                    loSource = loSource + GetControlEdit3().GetValue();
                }
            }

            // keep only the numeric digits
            string loNumericOnly = "";
            foreach (char oneChar in loSource)
            {
                if (( oneChar >= '0') && (oneChar <= '9' ))
                {
                    loNumericOnly += oneChar;
                }
            }


            // convert to numeric
            Int64 loHugeInt;
            loHugeInt = Convert.ToInt64(loNumericOnly);

            // bump it by ten to make a place holder for the check digit
            loHugeInt = (loHugeInt * 10);

            // calculate the check digit
            Int64 loValueWithCheckDigit = CalcMod7CheckDigit(loHugeInt);

            // isolate the check digit
            Int64 loCheckDigitOnly = loValueWithCheckDigit - ((loValueWithCheckDigit / 10) * 10);

            // Cincinnati special - add 1
            loCheckDigitOnly++;

            loResult = Convert.ToString(loCheckDigitOnly);

            // force the parent field to this value.
            _Parent.SetEditBufferAndPaint(loResult, false);
            //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            return false;
        }
    }
}