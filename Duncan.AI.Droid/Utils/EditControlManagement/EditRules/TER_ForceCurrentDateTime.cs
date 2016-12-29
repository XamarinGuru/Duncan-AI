using System;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Duncan.AI.Droid.Utils.HelperManagers;
namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_ForceCurrentDateTime.
    /// </summary>
    public class TER_ForceCurrentDateTime :  EditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ForceCurrentDateTime()
            : base()
        {
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            string loTmpBuf = "";
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;

            if (Parent.GetFieldType() == EditEnumerations.EditFieldType.efDate)
               DateTimeManager.OsDateToDateString(DateTime.Today.AddDays(IntParam), Parent.GetEditMask(), ref loTmpBuf);

            if (Parent.GetFieldType() == EditEnumerations.EditFieldType.efTime)
                DateTimeManager.OsTimeToTimeString(DateTime.Now.AddSeconds(IntParam), Parent.GetEditMask(), ref loTmpBuf);

            Parent.SetEditBufferAndPaint(loTmpBuf, false);
            return false;
        }
        #endregion
    }
}