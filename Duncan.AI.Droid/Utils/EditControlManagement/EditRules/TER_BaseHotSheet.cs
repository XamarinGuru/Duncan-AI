using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
   public  class TER_BaseHotSheet : EditRestriction
    {
        #region Properties and Members
        protected string _MatchFieldsName = "";
        public string MatchFieldsName
        {
            get { return _MatchFieldsName; }
            set { _MatchFieldsName = value; }
        }

        #endregion

        public TER_BaseHotSheet()
            : base()
        {
        }
    }
}