using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_ForceCleared.
    /// </summary>
    public class TER_ForceCleared : TER_ForceLiteral /*Reino.ClientConfig.TEditRestriction*/
    {
        #region Properties and Members
        #endregion

        public TER_ForceCleared()
            : base()
        {
            // Put default settings back in place
            ActiveOnCorrection = true;
            ActiveOnReissue = true;
            ActiveOnContinuance = true;
            ActiveOnIssueMore = true;
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            EditEnumerations.ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;

            //if it s a spinner, and the char param is null, then jsut clear it
            if (Parent.ControlType == EditEnumerations.CustomEditControlType.Spinner)
            {
                if (CharParam == null)
                {
                    //Parent.SetListDisplayValueAndPaint(Constants.SPINNER_DEFAULT);
                    Parent.SetListDisplayValueAndPaint("");
                }
                else
                {
                    //if the char param is not null, set based on db value instead of display value
                    Parent.SetListDBValueAndPaint(CharParam);
                }
            }
            else 
                Parent.SetEditBufferAndPaint(CharParam  ?? string.Empty, false);
            return false;
        }
        #endregion
    }
}