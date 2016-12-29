using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_ForceListItem.
    /// </summary>
    public class TER_ForceListItem : EditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ForceListItem()
            : base()
        {
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            EditEnumerations.ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) == EditEnumerations.ETrueFalseIgnore.tfiIgnore)
                return false;
           
            if (loEnforce == EditEnumerations.ETrueFalseIgnore.tfiTrue)
                Parent.SetListIndexAndPaint(IntParam);
            return false;
        }
        #endregion
    }
}