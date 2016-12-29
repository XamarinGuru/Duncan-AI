using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    public delegate bool DoSearch(TER_SearchHotSheet EditRestrict);

    public  class TER_SearchHotSheet : TER_BaseHotSheet
    {

        #region Properties and Members
        public event DoSearch OnDoSearch = null;
        #endregion

        public TER_SearchHotSheet()
            : base()
        {
        }

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;

            if (OnDoSearch != null)
            {
                return OnDoSearch(this);
            }
            return false;
        }
    }
}