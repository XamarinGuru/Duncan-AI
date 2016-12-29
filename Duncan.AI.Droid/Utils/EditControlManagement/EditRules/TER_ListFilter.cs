using System;
using System.Linq;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Duncan.AI.Droid.Utils.HelperManagers;
using Reino.ClientConfig;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_ListFilter.
    /// </summary>
    public class TER_ListFilter :  EditRestriction
    {
        #region Properties and Members
        public event ListContentsChangedByRestriction OnListContentsChangedByRestriction = null;
        #endregion

        public TER_ListFilter()
            : base()
        {
        }

        #region Implementation code
        // TER_ListFilter
        //Places a filter on a fields list based on the value of a parent field.
        //Default Edit Modes: NewIssue, Correction, Reissue, Continuance, IssueMore
        //Default Events: None
        //Default Overidable: False
        //ControlEdit1: Parent field to acquire filter value from.
        //CharParam: Column in field’s list table to apply filter to.  The special case MasterKey uses the parent’s list item number as the filter.
        //IntParam: If 1, the child field will be cleared if the parent value changes, and the current child value is now filtered out.
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            //this is only valid for list items (custom spinners for now)
            if (Parent != null && Parent.ControlType != EditEnumerations.CustomEditControlType.Spinner)
                return false;

            //is this restriction valid
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;

            //make sure we have a control edit 1 (parent)
            if (CheckForControlEdit1() != 0)
                return false;

            //build the filter
            //first we have to go find the parent value (controledit1)
            //then get the value of the parent


            var parentValue = ControlEdit1Obj.GetValue();

            //if parent control is a date, then format it and remove the /'s, since the dat files we will be comparing to have the dates in teh following format:
            //20150214
            //20150215
            //20150216
            //20150217
            //20150218
            if (ControlEdit1Obj.GetFieldType() == EditEnumerations.EditFieldType.efDate && !string.IsNullOrEmpty(parentValue))
            {
                //try to covert the value to a date, then convert that dat into the standard format expected by the server
                DateTime loFieldDate = DateTime.Today;
              var retValue =  DateTimeManager.DateStringToOSDate(ControlEdit1Obj.GetEditMask(), parentValue, ref loFieldDate);
                if (retValue >= 0)
                    //not we need to take this date and to string it on the specific server format.
                    parentValue = loFieldDate.ToString(Constants.DT_YYYYMMDD);
            }

            var parentIndex = ControlEdit1Obj.ListItemIndex;
            //then get the charparam property of the parent, this is the column we are filtering on.
            var columnName = CharParam;
            var filter = new ListFilter
            {
                Column = columnName,
                Value = parentValue,
                Index = parentIndex,
                FilterByIndex = false,
                ParentBehavior = ControlEdit1Obj
            };

            //// Assume the list filtering makes some sort of change to the list
            bool listFilterMadeChanges = true;

            //if the field is blank and the intparam is not enforced, remove the filter
            if (GetControlEdit1().FieldIsBlank() && IntParam == 0)
            {
                // if the field is blank and the filter is not strictly enforced, clear out the filter
                Parent.RemoveFilter(CharParam);
            }
            else
            {
                int filterResult = 0;
                if (CharParam == "MasterKey")
                    filter.FilterByIndex = true;

                //in this case we need to filter by index
                // If AddFilter returns false, then we know nothing has actually changed
                if (!Parent.AddFilter(filter))
                    listFilterMadeChanges = false;
            }

            //// if the current field value doesn't pass the filter, clear List Item no. 
            //// if the filter is strictly enforced, clear the field 
            if ((IntParam > 0) && (Parent.ListItemIndex < 0))
                Parent.SetEditBufferAndPaint("", false);
                 
            else
            {
                //call set datasource again if something changed and we didnt just clear it out.
                if (listFilterMadeChanges)
                    Parent.RefreshListItems();
            }

            // Do callback function so application knows to respond accordingly
            //if ((OnListContentsChangedByRestriction != null) && (listFilterMadeChanges == true))
              //  OnListContentsChangedByRestriction(Parent);
            return false;
        }
        #endregion
    }
}