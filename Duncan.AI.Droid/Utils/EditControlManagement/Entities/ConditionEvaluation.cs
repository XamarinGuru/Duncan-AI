namespace Duncan.AI.Droid.Utils.EditControlManagement.Entities
{
    public class ConditionEvaluation
    {
        protected bool _evalutation = false;
        public bool Evaluation
        {
            get { return _evalutation; }
            set { _evalutation = value; }
        }

        protected string _ConditionName = "";
        public string ConditionName
        {
            get { return _ConditionName; }
            set { _ConditionName = value.ToUpper(); } // Must be uppercase to match the name of an TEditCondition
        }
    }
}