namespace Duncan.AI.Droid.Utils.EditControlManagement.Entities
{
    // ReSharper disable InconsistentNaming
    public class EditEnumerations
    {
        public enum EditConditionType
        {
            rcIfFieldMatchesValue,
            rcIfFieldContainsValue,
            rcIfFieldContainsFieldName,
            rcIfFieldIsListItem,
            rcIfListIsPopulated,
            rcFieldValueInRange,
            rcFieldValuesMatch,
            rcFieldIsProtected
        }


        public enum ETrueFalseIgnore { tfiTrue, tfiFalse, tfiIgnore };

        public enum EditFieldType
        {
            efString = 0,
            efDate,
            efTime,
            efNumeric //Integer, Floating Point and Currency
        }
        public enum ListBoxStyle
        {
            lbsPopup,
            lbsStatic,
            lbsNone,
            lbsRadio
        }

        public enum CustomEditControlType
        {
            EditText,
            AutoCompleteText,
            Spinner
        }


        public enum IgnoreEventsType
        {
            ieIgnoreEventsFalse,
            ieIgnoreEventsTrue
        }

        public enum SetHasBeenFocusedType
        {
            bfSetHasBeenFocusedFalse,
            btSetHasBeenFocusedTrue
        }


    }
    // ReSharper restore InconsistentNaming
}