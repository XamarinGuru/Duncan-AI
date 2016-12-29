using Android.Content;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    public class TER_ChildList :  EditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ChildList()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnParentDataChanged = true;
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            //have to have a parent control for this to work
            if (this.ControlEdit1Obj == null)
                return false;

            //is this restriction active
            EditEnumerations.ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;

            // get the current panelfield for the control
            // get the parent control
            // update the lsit item text and value here if needed
            // go get the value from the DB

            var result = GetControlEdit1().GetAssociatedRowData(CharParam);

            UpdateParentControlWithNewValue(result, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue);



            //var value = result;
            //if (string.IsNullOrEmpty(value))
            //{
            //    //set this to empty string since once this cahnges, we need to clear the controls that might have already had values.
            //    value = string.Empty;
            //}

            //// set the text of the box
            //if (Parent.ControlType == EditEnumerations.CustomEditControlType.EditText)
            //{
            //    var customControl = (CustomEditText)Parent.EditCtrl;
            //    if (customControl != null)
            //    {
            //        customControl.IgnoreEvents = true;
            //        customControl.Text = value;


            //        // TODO - there should be a InitForEntry to take care of this kind of stuff
            //        customControl.HasBeenFocused = true;
            //        //customControl.FormStatus = "Processed";

            //        customControl.IgnoreEvents = false;
            //    }
            //}


            //if (Parent.ControlType == EditEnumerations.CustomEditControlType.AutoCompleteText)
            //{
            //    var customControl = (CustomAutoTextView)Parent.EditCtrl;
            //    if (customControl != null)
            //    {
            //        customControl.IgnoreEvents = true;
            //        customControl.SetListItemDataByValue(value);

            //        customControl.HasBeenFocused = true;
            //        //customControl.FormStatus = "Processed";

            //        customControl.IgnoreEvents = false;
            //    }
            //}


            //if (Parent.ControlType == EditEnumerations.CustomEditControlType.Spinner)
            //{
            //    var customControl = (CustomSpinner)Parent.EditCtrl;
            //    if (customControl != null)
            //    {
            //        customControl.IgnoreEvents = true;

            //        if (string.IsNullOrEmpty(value))
            //        {
            //            customControl.SetListIndex(-1);
            //            //customControl.SetListIndex(0);
            //        }
            //        else
            //        {
            //            customControl.SetListItemDataByValue(value);
            //        }

            //        customControl.HasBeenFocused = true;
            //        //customControl.FormStatus = "Processed";

            //        customControl.IgnoreEvents = false;
            //    }
            //}
            return false;
        }
        #endregion
    }
}