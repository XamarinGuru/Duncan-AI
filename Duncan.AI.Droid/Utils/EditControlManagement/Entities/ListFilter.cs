namespace Duncan.AI.Droid.Utils.EditControlManagement.Entities
{
    public class ListFilter
    {
        public string Value { get; set; }
        public int Index { get; set; }
        public string Column { get; set; }
        public bool FilterByIndex { get; set; }
        public EditControlBehavior ParentBehavior { get; set; }
    }
}