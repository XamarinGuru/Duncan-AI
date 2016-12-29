using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
   public  class TER_ForceSequence : EditRestriction
    {
       #region Properties and Members
       #endregion

       public TER_ForceSequence()
           : base()
       {
       }

        public event SetIssueNoFields OnSetIssueNoFields = null;

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;

            // Do the OnSetIssueNoFields event if its assigned 
            if (OnSetIssueNoFields != null)
                OnSetIssueNoFields();
            return false;
        }
    }
}