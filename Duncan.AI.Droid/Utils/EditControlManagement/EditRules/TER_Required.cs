using Android.Views;
using Android.Widget;
using Android.Graphics;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;


namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_Required.
    /// </summary>
    public class TER_Required :  EditRestriction
    {
        #region Properties and Members
        public string StructName { get; set; }
        #endregion

        public TER_Required()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnValidate = true;
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            EditEnumerations.ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) == EditEnumerations.ETrueFalseIgnore.tfiIgnore)
                return false;


            // lets parcel this out for easier debugging

            /*
            //determine if this is required or not (blank field or default text)
            bool retValue = (loEnforce == EditEnumerations.ETrueFalseIgnore.tfiTrue) &&
                            ((Parent.FieldIsBlank()) || (Parent.GetValue().Length < IntParam) ||
                             (Constants.SPINNER_DEFAULT.Equals(Parent.GetValue())));
            */

            bool retValue = false;
            bool loShouldBeEnforced = (loEnforce == EditEnumerations.ETrueFalseIgnore.tfiTrue);
            string loParentValueAsUpper = Parent.GetValue().ToUpper();
            bool loParentFieldIsBlank = Parent.FieldIsBlank();
            int loParentLength = loParentValueAsUpper.Length;

            if (loShouldBeEnforced == true)
            {
                //retValue = (
                //             (loParentFieldIsBlank == true) ||
                //             (loParentLength < IntParam) ||
                //             (loParentValueAsUpper.Equals(Constants.SPINNER_DEFAULT_AS_UPPER) == true)
                //           );
                retValue = (
                             (loParentFieldIsBlank == true) ||
                             (loParentLength < IntParam)
                           );

            }


            //go update the label to reflect this.
            UpdateLabelToShowRequiredStatus(retValue);
            return retValue;
        }


        #endregion
    }
}