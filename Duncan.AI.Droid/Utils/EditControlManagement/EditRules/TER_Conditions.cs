using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_Conditions.
    /// </summary>
    public class TER_Conditions :  EditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_Conditions()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnValidate = true;
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            return RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) == EditEnumerations.ETrueFalseIgnore.tfiTrue;
        }
        #endregion
    }
}