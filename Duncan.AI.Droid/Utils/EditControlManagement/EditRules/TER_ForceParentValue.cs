using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Duncan.AI.Droid.Utils.HelperManagers;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_ForceParentValue.
    /// </summary>
    public class TER_ForceParentValue : EditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ForceParentValue()
            : base()
        {
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            EditEnumerations.ETrueFalseIgnore loEnforce;

            if (ControlEdit1 == "")
                return false;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) == EditEnumerations.ETrueFalseIgnore.tfiIgnore)
                return false;

            if (loEnforce == EditEnumerations.ETrueFalseIgnore.tfiTrue)
            {
                // try to handle number format differences
                if (Parent.GetFieldType() == EditEnumerations.EditFieldType.efNumeric)
                {
                    string loSrcStr = "";
                    string loDestStr = "";
                    // is source a numeric field?
                    var ctrlEdit1 = GetControlEdit1();
                   if (ctrlEdit1.GetFieldType() == EditEnumerations.EditFieldType.efNumeric)
                       NumericManager.FormatNumberStr(ctrlEdit1.GetValue(), ctrlEdit1.GetEditMask(), ref loSrcStr);
                    else
                       loSrcStr = ctrlEdit1.GetValue();

                   NumericManager.FormatNumberStr(loSrcStr, Parent.GetEditMask(), ref loDestStr);
                    Parent.SetEditBufferAndPaint(loDestStr, false);
                }
                else
                    Parent.SetEditBufferAndPaint(GetControlEdit1().GetValue(), false);
            }
            return false;
        }
        #endregion
    }
}