using System;
using Android.Widget;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils.HelperManagers;

namespace Duncan.AI.Droid.Utils.EditControlManagement.Entities
{
    public class EditCondition : EditDependency
    {
        #region Properties and Members

        public EditEnumerations.EditConditionType ConditionType { get; set; }

        #endregion

        #region Implementation code

        /*
		 * TEditCondition::DateFieldValueInRange()
		 *
		 * Evaluates the FieldValueInRange condition for a date field.
		 * 
		 * Ranges are relative to the current date.
		 * - fIntParam: Number of days from current date to be greater than
		 * - fIntParam2: Number of days from current date to be less than
		 * Example 1: Date Must be in Future
		 * - fIntParam = 1, fIntParam2 = 0 (ignore it)
		 * Example 2: Date Must be in Past
		 * - fIntParam = 0 (ingnore it), fIntParam2 = -1 
		 */

        private bool DateFieldValueInRange(EditControlBehavior iControlEdit)
        {
            DateTime loFieldDate = DateTime.Today;
            DateTimeManager.DateStringToOSDate(iControlEdit.PanelField.EditMask, iControlEdit.GetValue(), ref loFieldDate);

            // difference between entered date & current date
            TimeSpan loDateDif = loFieldDate - DateTime.Today;

            // 1st check: have a low && number < low;
            if ((IntParam != 0) && (loDateDif.Days <= IntParam))
                return false; // number is less than lower limit! return 0

            // 2nd check: have a high && number > high;
            if ((IntParam2 != 0) && (loDateDif.Days >= IntParam2))
                return false; // number is greater than upper limit! return 0

            // passed both checks, return 1.
            return true;
        }

        /*
         * TEditCondition::EvaluateCondition()
         * 
         * Evaluates conditions based on the condition type.  This should have been implemented as separate classes.  
         * Instead we are faced with a switch statement that will grow as new evaluations are added.
         */

        public bool EvaluateCondition()
        {
            Int64 loTmpNum = 0;
            EditControlBehavior loControlEdit;

            switch (ConditionType)
            {
                case EditEnumerations.EditConditionType.rcIfFieldMatchesValue:
                    if (CheckForControlEdit1() != 0)
                        return false;
                    if (string.IsNullOrEmpty(CharParam) && GetControlEdit1() != null)
                        return GetControlEdit1().FieldIsBlank();
                    return CharParam.Equals(GetControlEdit1().GetValue());

                case EditEnumerations.EditConditionType.rcIfFieldContainsValue:
                    if (ControlEdit1 == "" || CharParam == "")
                        return false;

                    if (GetControlEdit1() == null)
                        return false;
                    if (string.IsNullOrEmpty(GetControlEdit1().GetValue()))
                        return false;
                    return GetControlEdit1().GetValue().IndexOf(CharParam ?? string.Empty, StringComparison.Ordinal) >= 0;

                case EditEnumerations.EditConditionType.rcIfFieldContainsFieldName:
                    if (ControlEdit1 == "" || Parent == null)
                        return false;
                    if (GetControlEdit1() == null)
                        return false;

                    if (string.IsNullOrEmpty(GetControlEdit1().GetValue()))
                        return false;
                    // Lets do a case-insensitive comparison for this one
                    return GetControlEdit1().GetValue().ToUpper().IndexOf(Parent.CfgCtrl.Name, StringComparison.Ordinal) >= 0;

                case EditEnumerations.EditConditionType.rcIfFieldIsListItem:
                    if (ControlEdit1 == "")
                        return false;

                    loControlEdit = GetControlEdit1();
                    if (loControlEdit != null && loControlEdit.EditCtrl is CustomSpinner)
                    {
                        var spinner = ((CustomSpinner) (loControlEdit.EditCtrl));
                        if (spinner.SelectedItemPosition > 0)
                            return true;
                    }

                    // AJW - must also evaluate autotextview for listvalues
                    // is it an autotextview?
                    if (loControlEdit.EditCtrl is CustomAutoTextView)
                    {
                        CustomAutoTextView loAutoTextView = ((CustomAutoTextView)(loControlEdit.EditCtrl));
                        if (loAutoTextView.BehaviorAndroid != null)
                        {
                            var items = loAutoTextView.BehaviorAndroid.GetFilteredListItems();
                            if (items != null)
                            {
                                if (items.IndexOf(loAutoTextView.Text) > 0)
                                {
                                    // there is a list, and the current value in the field is in the list
                                    return true;
                                }

                            }

                        }
                    }

                    return false;

                case EditEnumerations.EditConditionType.rcIfListIsPopulated:
                    loControlEdit = ControlEdit1 == "" ? Parent : GetControlEdit1();

                    // is it a spinner?
                    if (loControlEdit.EditCtrl is CustomSpinner)
                    {
                        // If there is no list, then its not populated
                        var spinner = ((CustomSpinner)(loControlEdit.EditCtrl));
                        var customAdapter = (ArrayAdapter<String>)spinner.Adapter;

                        if (customAdapter != null && string.IsNullOrEmpty( spinner.GetText().Trim()) == false)
                        //if (customAdapter != null && spinner.GetText() != Constants.SPINNER_DEFAULT)
                        {
                            //if it has values that isnt the default value
                            return true;
                        }
                    }

                    // AJW - must also evaluate autotextview for listvalues
                    // is it an autotextview?
                    if (loControlEdit.EditCtrl is CustomAutoTextView)
                    {
                        var loAutoTextView = ((CustomAutoTextView)(loControlEdit.EditCtrl));
                        if (loAutoTextView.BehaviorAndroid != null)
                        {
                            var items = loAutoTextView.BehaviorAndroid.GetFilteredListItems();
                            if (items != null)
                            {
                                if (items.Count > 0)
                                {
                                    // there is a list, and its not empty
                                    return true;
                                }

                            }

                        }
                    }


                    return false;




                case EditEnumerations.EditConditionType.rcFieldValueInRange:
                    loControlEdit = ControlEdit1 == "" ? Parent : GetControlEdit1();

                    if (loControlEdit.GetFieldType() == EditEnumerations.EditFieldType.efDate)
                        return DateFieldValueInRange(loControlEdit);
                    NumericManager.StrTollInt(loControlEdit.GetValue(), ref loTmpNum);
                    // 1st check: have a low && number < low;
                    if ((IntParam > 0) && (loTmpNum < IntParam))
                        return false; // number is less than lower limit! return 0
                    // 2nd check: have a high && number > high;
                    if ((IntParam2 > 0) && (loTmpNum > IntParam2))
                        return false; // number is greater than upper limit! return 0
                    // passed both checks, return 1.
                    return true;


                case EditEnumerations.EditConditionType.rcFieldValuesMatch:
                    {
                        // safe wrappers around access
                        string loControlEdit1Value = string.Empty;
                        if (string.IsNullOrEmpty(ControlEdit1) == false)
                        //if (ControlEdit1 != "")
                        {
                            EditControlBehavior loControl1 = GetControlEdit1();
                            if (loControl1 != null)
                            {
                                loControlEdit1Value = loControl1.GetValue();
                            }
                        }

                        string loControlEdit2Value = string.Empty;
                        if (string.IsNullOrEmpty(ControlEdit2) == false)
                        //if (ControlEdit2 != "")
                        {
                            EditControlBehavior loControl2 = GetControlEdit2();
                            if (loControl2 != null)
                            {
                                loControlEdit2Value = loControl2.GetValue();
                            }
                        }


                        // two fields' values match. Compare controlEdit1 & 2. If one is blank, use parent edit.
                        if (ControlEdit1 != "" && ControlEdit2 != "") // both control1 & 2 exist, so compare them.
                        {
                            return loControlEdit1Value.Equals(loControlEdit2Value);
                        }

                        if (ControlEdit1 != "") // only control 1 exists (2 doesn't), so compare to parent
                        {
                            return loControlEdit1Value.Equals(Parent.GetValue());
                        }

                        if (ControlEdit2 != "") // only control 2 exiss (1 doesn't), so compare to parent
                        {
                            return loControlEdit2Value.Equals(Parent.GetValue());
                        }

                        // control 1 & 2 don't exist, so compare parent to itself
                        return true;
                    }

                case EditEnumerations.EditConditionType.rcFieldIsProtected:
                    // two fields' values match. Compare controlEdit1 & 2. If one is blank, use parent edit.
                    if (ControlEdit1 != "")
                        return GetControlEdit1().CfgCtrl.IsProtected; // !GetControlEdit1().Enabled
                        return Parent.CfgCtrl.IsProtected;

                default:
                    return true;
            }
        }

        #endregion
    }
}