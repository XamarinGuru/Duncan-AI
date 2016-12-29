using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_Mod10CheckDigit.
    /// </summary>
    public class TER_Mod10CheckDigit :  EditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_Mod10CheckDigit()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnCorrection = false;
            ActiveOnReissue = false;
            ActiveOnContinuance = false;
            ActiveOnFormInit = true;
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;
            return false;
        }
        #endregion
    }
}