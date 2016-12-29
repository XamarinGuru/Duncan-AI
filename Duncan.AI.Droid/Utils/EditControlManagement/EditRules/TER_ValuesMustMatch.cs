using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_ValuesMustMatch.
    /// </summary>
    public class TER_ValuesMustMatch :  EditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ValuesMustMatch()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnValidate = true;
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            if (((RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                 || ControlEdit1 == "")
                return false;
            if (CheckForControlEdit1() != 0)
                return false;
            return Parent.GetValue().CompareTo(GetControlEdit1().GetValue()) != 0;
        }
        #endregion
    }

}