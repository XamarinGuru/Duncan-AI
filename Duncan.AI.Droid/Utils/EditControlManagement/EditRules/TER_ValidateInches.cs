using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_ValidateInches.
    /// </summary>
    public class TER_ValidateInches :  EditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ValidateInches()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnValidate = true;
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            int loNdx;
            int loInchesStartPos = -1;
            bool lo2InchesChars = false;
            string loDataStr = "";

            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;

            if (Parent.FieldIsBlank())
                return false;

            loDataStr = Parent.GetValue();

            // find the inches portion. It is either the 1st two digits from the end OR the first digit before a literal mask character.
            for (loNdx = loDataStr.Length - 1; loNdx >= 0; loNdx--)
            {
                if ((loDataStr[loNdx] < '0') || (loDataStr[loNdx] > '9'))
                    break; // found a delimiter

                // have a digit. Is this the 1st or 2nd?
                if (loInchesStartPos == -1) // 1st inches digit we've encountered
                {
                    loInchesStartPos = loNdx;
                }
                else
                { // 2nd inches character
                    loInchesStartPos = loNdx;
                    lo2InchesChars = true;
                    break;
                }
            }

            // did we encounter any inches?
            if (loInchesStartPos == -1)
                return true; // no inches, return FAILURE

            // make sure there are feet as well
            for (loNdx = loInchesStartPos - 1; loNdx >= 0; loNdx--)
            {
                if ((loDataStr[loNdx] >= '0') && (loDataStr[loNdx] <= '9'))
                    break; // found a DIGIT for feet
            }

            if (loNdx < 0)
                return true; // no feet, so return FAILURE!

            if (!lo2InchesChars)
            { // only 1 inch digit, so merely insert a 0 and we're done.
                loDataStr = loDataStr.Insert(loInchesStartPos, "0");
                Parent.SetEditBufferAndPaint(loDataStr, false);
                return false;
            }

            // there are two inches digits. Make sure they don't exceed 11.
            if ((loDataStr[loInchesStartPos] > '1') ||((loDataStr[loInchesStartPos] == '1') && (loDataStr[loInchesStartPos + 1] > '1')))
                return true;

            return false;
        }
        #endregion
    }
}