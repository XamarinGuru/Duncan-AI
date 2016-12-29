using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Duncan.AI.Droid.Utils.HelperManagers;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_ForceCalcJuvenile.
    /// </summary>
    public class TER_ForceCalcJuvenile :  EditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ForceCalcJuvenile()
            : base()
        {
            // Set our defaults that differ from parent.
            CharParam = "Y";
            IntParam = 17;
            ControlEdit1 = "IssueDate";
            ControlEdit2 = "DLBirthDate";
        }

        #region Implementation code
        protected int CalcAge()
        {
            int loIssueYear = 0;
            int loIssueMo = 0;
            int loIssueDay = 0;
            int loDOBYear = 0;
            int loDOBMo = 0;
            int loDOBDay = 0;
            int loAge = 0;

            if (CheckForControlEdit1() != 0) return 0;
            if (CheckForControlEdit2() != 0) return 0;

            // get the issue date in fControlEdit1
            if (DateTimeManager.DateStringToDMY(GetControlEdit1().GetEditMask(),
                GetControlEdit1().GetValue(), ref loIssueDay, ref loIssueMo, ref loIssueYear) < 0)
                return -1;

            // get the birth date in fControlEdit2
            if (DateTimeManager.DateStringToDMY(this.GetControlEdit2().GetEditMask(),
                this.GetControlEdit2().GetValue(), ref loDOBDay, ref loDOBMo, ref loDOBYear) < 0)
                return -1;

            // calculate the age in years
            loAge = loIssueYear - loDOBYear;
            if ((loDOBMo > loIssueMo) || ((loDOBMo == loIssueMo) && (loDOBDay > loIssueDay)))
                loAge--;

            return loAge;
        }

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            int loAge;

            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;

            if (ControlEdit1 == "" || ControlEdit2 == "")
                return false;

            // is DOB blank? blank out juvenile
            if (this.GetControlEdit2().FieldIsBlank())
            {
                Parent.SetEditBufferAndPaint("", false);
                return false;
            }

            // save the previous value of the field so we can determine if we changed it
            string loPrevValue = Parent.GetValue();

            if ((loAge = CalcAge()) < 0)
                return false;

            // is it less than the limit?
            if (loAge < IntParam)
            {
                // We have a juvenile, is the value changing?
                if (loPrevValue.CompareTo("Y") != 0)
                {
                    string loLine1 = string.Format("{0:d} yrs old (Juv is under {1:d}).", loAge, IntParam);
                    Parent.SetEditBufferAndPaint("Y", false);
                    // should we alert the user?
                    if (CharParam == "Y")
                    {
                        if (EditControlBehavior.OnStandardMessageBox != null)
                            EditControlBehavior.OnStandardMessageBox(loLine1, "Juvenile Offender!");
                        //else
                        //    MessageBox.Show(loLine1, "Juvenile Offender!");
                    }
                }
            }
            else
            {
                Parent.SetEditBufferAndPaint("N", false);
            }
            return false;
        }
        #endregion
    }
}