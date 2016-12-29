
using System.Collections.Generic;
using System.Xml.Serialization;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Reino.ClientConfig;

namespace Duncan.AI.Droid.Utils.EditControlManagement.Controls
{
    public class CustomTTEditControl : CustomTTControl
    {

        public CustomTTEditControl()
			: base()
		{
		}
      
         #region Properties and Members
		protected TEditPrompt _PromptWin = new TEditPrompt();
		public TEditPrompt PromptWin
		{
			get { return _PromptWin; }
			set { _PromptWin = value; }
		}

        protected List<EditCondition> _Conditions = new List<EditCondition>();
        /// <summary>
        /// A collection of TEditCondition objects
        /// </summary>
        public List<EditCondition> Conditions
        {
            get { return _Conditions; }
            set { _Conditions = value; }
        }

        protected List<EditRestriction> _Restrictions = new List<EditRestriction>();
        /// <summary>
        /// A collection of TEditRestriction objects
        /// </summary>
        public List<EditRestriction> Restrictions
        {
            get { return _Restrictions; }
            set { _Restrictions = value; }
        }

        protected short _MaxLength = 80;
        [System.ComponentModel.DefaultValue(80)] // This prevents serialization of default values
        public short MaxLength
        {
            get { return _MaxLength; }
            set { _MaxLength = value; }
        }

        protected bool _DontDisableWhenCopiedFromForm = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool DontDisableWhenCopiedFromForm
        {
            get { return _DontDisableWhenCopiedFromForm; }
            set { _DontDisableWhenCopiedFromForm = value; }
        }

        protected bool _PreventPrompt = false;
        [XmlIgnore] // We don't want the following public property/member serialized in XML
        public bool PreventPrompt
        {
            get { return _PreventPrompt; }
            set { _PreventPrompt = value; }
        }

        protected bool _IsVioField = false;
        [XmlIgnore] // We don't want the following public property/member serialized in XML
        public bool IsVioField
        {
            get { return _IsVioField; }
            set { _IsVioField = value; }
        }

        protected bool _IsNotesMemo = false;
        [XmlIgnore] // We don't want the following public property/member serialized in XML
        public bool IsNotesMemo
        {
            get { return _IsNotesMemo; }
            set { _IsNotesMemo = value; }
        }

        protected bool _IsColorField = false;
        [XmlIgnore] // We don't want the following public property/member serialized in XML
        public bool IsColorField
        {
            get { return _IsColorField; }
            set { _IsColorField = value; }
        }
        #endregion

    }
}