using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    public class TER_ForceStLouisCheckDigit : EditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ForceStLouisCheckDigit()
            : base()
        {
            // Set our defaults that differ from parent.
            ControlEdit1 = "IssueNo";
        }

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {

            int loNdx = 0;
            int loSourceStrLen = 0;
            int loSum = 0;
            string loResult;

            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;

            // make sure we have a source field
            if (CheckForControlEdit1() != 0)
                return false;

            if (GetControlEdit1().GetValue() == "")
                return false; // source is blank.

            loSourceStrLen = GetControlEdit1().GetValue().Length;

            for (loNdx = loSourceStrLen - 1; loNdx >= 0; loNdx--)
            {
                // make sure the character is a digit.
                if ((GetControlEdit1().GetValue()[loNdx] < '0') ||
                    (GetControlEdit1().GetValue()[loNdx] > '9'))
                    continue;

                // least significant digit is at right-most position in string.
                loSum += (GetControlEdit1().GetValue()[loNdx] - '0') * (loSourceStrLen - loNdx + 1);
            }

            // calculate the sum Mod 11
            loSum = 11 - (loSum % 11);
            // convert to a character.
            if (loSum < 10)
                loResult = ((char)(loSum + (int)'0')).ToString();
            else
                loResult = ((char)(loSum + (int)'A' - 10)).ToString();
            // force the parent field to this value.
            _Parent.SetEditBufferAndPaint(loResult, false);
            //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            return false;
        }
    }
}