using System;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_Multiply.
    /// </summary>
    public class TER_Multiply :  EditRestriction
    {
        #region Properties and Members
        private int _CachedFieldIndex = -1;
        private Reino.ClientConfig.TTTable _CachedListSourceTable = null;
        #endregion

        public TER_Multiply()
            : base()
        {
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            EditEnumerations.ETrueFalseIgnore loEnforce;
            Int64 loMulValue = 1;
            Int64 loCurrValue = 0;
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
                if (GetControlEdit2() != null  && ControlEdit2 != ""  && GetControlEdit2().ListItemIndex > 0 && !string.IsNullOrEmpty(CharParam))
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
                            Int64.TryParse(value, out loCurrValue);
                    }
                }

                //2)	The delta from step one is then incremented by the current numeric value of another optional parent field’s data. 
                //This parent field is designated by the ControlEdit1 property, and its value should contain a value that can be interpreted numerically
                // is there a value in another field
                if (GetControlEdit1() != null && ControlEdit1 != "")
                    long.TryParse(GetControlEdit1().GetValue(), out loMulValue);
                // get the total amt, include constant value in fIntParam2
                loMulValue = loCurrValue + IntParam2;
            }

            if (loMulValue == 1) // multplying/dividing by 1 does nothing
                return false;
            // Get current field value. If string contains a decimal, then we first have to treat it as a double/floating value

            var parentValue = Parent.GetValue();
            if (!string.IsNullOrEmpty(parentValue))
            {
                if (parentValue.IndexOf('.') >= 0)
                {
                    double dbVal;
                    if (Double.TryParse(parentValue, out dbVal))
                        long.TryParse(dbVal.ToString(), out loCurrValue);
                }
                else
                    loCurrValue = Convert.ToInt64(parentValue);
            }
           
            if (IntParam > 0)
                loCurrValue /= loMulValue;
            else
                loCurrValue *= loMulValue;

            loCurrValueStr = loCurrValue.ToString();
            Parent.SetEditBufferAndPaint(loCurrValueStr, false);
            return false;
        }
        #endregion
    }
}