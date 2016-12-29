
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;


namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    public class TER_ForceLiteral : EditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ForceLiteral()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnCorrection = false;
            ActiveOnReissue = false;
            ActiveOnContinuance = false;
            ActiveOnIssueMore = false;
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            EditEnumerations.ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;

            //// substitue an empty string for NULL parameter
            //string value = string.Empty;
            //if (CharParam != null)
            //{
            //    value = CharParam;
            //}

            //if (Parent.ControlType == EditEnumerations.CustomEditControlType.Spinner)
            //    Parent.SetListDBValueAndPaint(loCharParam);
            //else
            //    Parent.SetEditBufferAndPaint(loCharParam, false);


            UpdateParentControlWithNewValue(CharParam, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.bfSetHasBeenFocusedFalse);


            return false;


        }
        #endregion
    }
}