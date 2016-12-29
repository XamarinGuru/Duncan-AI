using System;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Duncan.AI.Droid.Utils.HelperManagers;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_Increment.
    /// </summary>
    public class TER_Increment :  EditRestriction
    {
        #region Properties and Members
        private int _CachedFieldIndex = -1;
        private Reino.ClientConfig.TTTable _CachedListSourceTable = null;
        #endregion

        public TER_Increment()
            : base()
        {
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            try
            {
                EditEnumerations.ETrueFalseIgnore loEnforce;
                double loIncValue = 0;
                double loCurrValue = 0;
                DateTime loCurrValueDateTime = DateTime.Now;
                string loCurrValueStr = "";

                if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) == EditEnumerations.ETrueFalseIgnore.tfiIgnore)
                    return false;

                if (loEnforce == EditEnumerations.ETrueFalseIgnore.tfiTrue)
                {
                    // 1) The delta value for incrementing is initialized to zero,
                    //and then based on a numerical entry in a list associated with an optional secondary parent field. 
                    //This parent field is designated by the ControlEdit2 property, 
                    //and the list value should come from the column declared by the CharaParam property for the list 
                    //entry that has the same record index as the secondary parent field’s value. 

                    //Note: The list value doesn’t necessarily equal the value of the parent field, but shares a common record index within the list table
                    //Note: The source list isn’t declared in this edit restriction’s properties; 
                    //rather it must be inferred by inspecting the current state of the parent field. 
                    //(For example, the parent field might be associated with its “normal” list, or it might currently be 
                    //associated with a different list due to active edit restrictions, such as a TER_ChildList)
                    if (ControlEdit2 != "" && GetControlEdit2() != null && GetControlEdit2().ListItemIndex > 0 && !string.IsNullOrEmpty(CharParam))
                    {
                        var controlToUse = GetControlEdit2();
                        //lets do some sanity checks to make sure this behavior has list indormation (table name, etc.)
                        if (controlToUse.PanelField.OptionsList != null &&
                            !string.IsNullOrEmpty(controlToUse.PanelField.OptionsList.ListName))
                        {
                            var result = GetControlEdit2().GetAssociatedRowData(CharParam);
                            var value = result;
                            //if somethign came back , try to convert it to loCurrValue
                            if (!string.IsNullOrEmpty(value))
                                double.TryParse(value, out loCurrValue);
                        }
                    }

                    //2)	The delta from step one is then incremented by the current numeric value of another optional parent field’s data. 
                    //This parent field is designated by the ControlEdit1 property, and its value should contain a value that can be interpreted numerically
                    // is there a value in another field
                    if (ControlEdit1 != "")
                    {
                        NumericManager.MaskStrToDouble(GetControlEdit1().GetValue(), GetControlEdit1().GetEditMask(), ref loIncValue);
                    }

                    //3)	Finally, the delta from step two is then incremented by the value declared in the IntParam2 property
                    // get the total amt, include constant value in fIntParam2
                    loIncValue += loCurrValue + IntParam2;
                }

                if (loIncValue == 0) // adding/subtracting by 0 does nothing
                    return false;



                //After the delta to be used by the increment function has been calculated, 
                //it is either added to or subtracted from the current field’s value depending on the data type of the field, 
                //and the arithmetic operation indicated by the IntParam property. If IntParam is “0”, then addition is used. 
                //Subtraction will be used if the IntParam contains “1”.
                //•	If the field is a Date, then the delta value is added/subtracted as a number of days.
                //•	If the field is a Time, then the delta value is added/subtracted as a number of seconds.
                //•	If the field is not either Date or Time, then the delta value is added/subtracted to the field’s current value 
                //which is assumed to currently represent either an integer or floating point numerical value
                // Get current field value
                switch (Parent.GetFieldType())
                {
                    case EditEnumerations.EditFieldType.efDate:
                        {
                            DateTimeManager.DateStringToOSDate(Parent.GetEditMask(), Parent.GetValue(), ref loCurrValueDateTime);
                            if (IntParam > 0)
                                loCurrValueDateTime = loCurrValueDateTime.AddDays(loIncValue * -1);
                            else
                                loCurrValueDateTime = loCurrValueDateTime.AddDays(loIncValue);
                            break;
                        }
                    case EditEnumerations.EditFieldType.efTime:
                        {
                            DateTimeManager.TimeStringToOSTime(Parent.GetEditMask(), Parent.GetValue(), ref loCurrValueDateTime);
                            if (IntParam > 0)
                                loCurrValueDateTime = loCurrValueDateTime.AddSeconds(loIncValue * -1);
                            else
                                loCurrValueDateTime = loCurrValueDateTime.AddSeconds(loIncValue);
                            break;
                        }
                    default:
                        NumericManager.MaskStrToDouble(Parent.GetValue(), Parent.GetEditMask(), ref loCurrValue);
                        if (IntParam > 0)
                            loCurrValue -= loIncValue;
                        else
                            loCurrValue += loIncValue;
                        break;
                }

                // Get current field value
                switch (Parent.GetFieldType())
                {
                    case EditEnumerations.EditFieldType.efDate:
                        {
                            DateTimeManager.OsDateToDateString(loCurrValueDateTime, Parent.GetEditMask(), ref loCurrValueStr);
                            break;
                        }
                    case EditEnumerations.EditFieldType.efTime:
                        {
                            DateTimeManager.OsTimeToTimeString(loCurrValueDateTime, Parent.GetEditMask(), ref loCurrValueStr);
                            break;
                        }


                    // TODO - AJW in legacy code, the SetEditBufferAndPaint calls MergeEditBufferWMask to set the field values using the edit mask
                    //        if this is fleshed out the same as legacy code, then this case for Numeric should be removed from here
                    case EditEnumerations.EditFieldType.efNumeric:
                        {
                            string loCurentNumericValueAsStr = loCurrValue.ToString();
                            string loParentEditMask = Parent.GetEditMask();

                            // KLUDGE until the MergeEditBuffer is properly implemented
                            // if the edit mask contains a decimal point but the string value doesn't have one then add it so the formatting function will work
                            if (loParentEditMask.Contains("."))
                            {
                                if (loCurentNumericValueAsStr.Contains(".") == false)
                                {
                                    loCurentNumericValueAsStr = loCurentNumericValueAsStr + ".";
                                }
                            }

                            NumericManager.FormatNumberStr(loCurentNumericValueAsStr, loParentEditMask, ref loCurrValueStr);
                            break;
                        }


                    default:
                        loCurrValueStr = string.Format("{0:N}", loCurrValue);
                        break;
                }
                Parent.SetEditBufferAndPaint(loCurrValueStr, false);
                return false;
            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, exp.Message, "EnforceRestriction.TER_Increment: " + Parent.CustomId );
                Console.WriteLine("Exception caught in process: {0} {1}", exp.Message, "EnforceRestriction.TER_Increment: " + Parent.CustomId );
            }

            return false;
        }
        #endregion
    }
}