// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 4/19/07 4:38p $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Common/ClientConfig/Forms.cs $
//              Revision: $Revision: 8 $

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Reino.ClientConfig
{
	/// <summary>
	/// Summary description for TTForm.
	/// </summary>
	
	/* The XmlInclude attribute is used on a base type to indicate that when serializing 
	 * instances of that type, they might really be instances of one or more subtypes. 
	 * This allows the serialization engine to emit a schema that reflects the possibility 
	 * of really getting a Derived when the type signature is Base. For example, we keep
	 * field definitions in a generic collection of TTForm. If an array element is 
	 * TIssForm, the XML serializer gets mad because it was only expecting TTForm. 
	 */
	[XmlInclude(typeof(TDrawForm)), XmlInclude(typeof(TSigCaptureForm)),
	 XmlInclude(typeof(TBaseIssForm)), XmlInclude(typeof(TIssueSelectForm)),
	 XmlInclude(typeof(TIssForm)), XmlInclude(typeof(TCancelForm)),
	 XmlInclude(typeof(TActivityLogForm)), XmlInclude(typeof(TPublicContactForm)),
	 XmlInclude(typeof(TBaseDetailForm)), XmlInclude(typeof(TBaseSearchForm)),
	 XmlInclude(typeof(TSearchMatchForm)), XmlInclude(typeof(TSearchForm)),
 	 XmlInclude(typeof(TNotesForm)), XmlInclude(typeof(TMarkModeForm)),
     XmlInclude(typeof(THotSheetForm))]
	public class TTForm : Reino.ClientConfig.TTPanel
	{
		#region Properties and Members
		protected string _Caption = "";
		public string Caption
		{
			get { return _Caption; }
			set { _Caption = value; }
		}
		#endregion

		public TTForm()
			: base()
		{
            // Set our defaults that differ from our parent.
            Height = 220;
            Width = 320;
        }
	}

	/// <summary>
	/// Summary description for TDrawForm.
	/// </summary>
	
	public class TDrawForm : Reino.ClientConfig.TTForm
	{
		#region Properties and Members
		#endregion

		public TDrawForm()
			: base()
		{
		}
	}

	/// <summary>
	/// Summary description for TSigCaptureForm.
	/// </summary>
	
	public class TSigCaptureForm : Reino.ClientConfig.TDrawForm
	{
		#region Properties and Members
		#endregion

		public TSigCaptureForm()
			: base()
		{
		}
	}

	/// <summary>
	/// Summary description for TBaseIssForm.
	/// </summary>
	
	public class TBaseIssForm : Reino.ClientConfig.TTForm
	{
		#region Properties and Members
		protected bool _PreventEsc = false;
		public bool PreventEsc
		{
			get { return _PreventEsc; }
			set { _PreventEsc = value; }
		}

        [XmlIgnoreAttribute]
        public object StructLogicObj = null;
		#endregion

		public TBaseIssForm()
			: base()
		{
		}
	}

	/// <summary>
	/// Summary description for TIssueSelectForm.
	/// </summary>
	
	public class TIssueSelectForm : Reino.ClientConfig.TBaseIssForm
	{
		#region Properties and Members
		#endregion

		public TIssueSelectForm()
			: base()
		{
		}
	}

	/// <summary>
	/// Summary description for TIssForm.
	/// </summary>
	
	public class TIssForm : Reino.ClientConfig.TBaseIssForm
	{

        #region Properties and Members
        protected List<TIssPrnForm> _PrintPictureList = new List<TIssPrnForm>();
        public List<TIssPrnForm> PrintPictureList
        {
            get { return _PrintPictureList; }
            set { _PrintPictureList = value; }
        }

        protected string _IssueMoreFirstFocus = "";
        public string IssueMoreFirstFocus
        {
            get { return _IssueMoreFirstFocus; }
            set { _IssueMoreFirstFocus = value; }
        }

        protected string _AutoIssueMore = "";
        public string AutoIssueMore
        {
            get { return _AutoIssueMore; }
            set { _AutoIssueMore = value; }
        }

        protected int _PrintCopyCnt = 0;
        public int PrintCopyCnt
        {
            get { return _PrintCopyCnt; }
            set { _PrintCopyCnt = value; }
        }

        protected bool _PreventPostPrintSignatures = false;
        public bool PreventPostPrintSignatures
        {
            get { return _PreventPostPrintSignatures; }
            set { _PreventPostPrintSignatures = value; }
        }

        protected string _SuspSignatureCaption = "";
        public string SuspSignatureCaption
        {
            get { return _SuspSignatureCaption; }
            set { _SuspSignatureCaption = value; }
        }

        protected string _OfficerSignatureCaption = "";
        public string OfficerSignatureCaption
        {
            get { return _OfficerSignatureCaption; }
            set { _OfficerSignatureCaption = value; }
        }

        protected bool _PomMeterEnabled = false;
        public bool PomMeterEnabled
        {
            get { return _PomMeterEnabled; }
            set { _PomMeterEnabled = value; }
        }

        protected bool _PrintNotMandatory = false;
        public bool PrintNotMandatory
        {
            get { return _PrintNotMandatory; }
            set { _PrintNotMandatory = value; }
        }

        protected string _SigCaptureDisplayFieldName = "";
        public string SigCaptureDisplayFieldName
        {
            get { return _SigCaptureDisplayFieldName; }
            set { _SigCaptureDisplayFieldName = value; }
        }

        protected string _SigCaptureDisplayFieldPrompt = "";
        public string SigCaptureDisplayFieldPrompt
        {
            get { return _SigCaptureDisplayFieldPrompt; }
            set { _SigCaptureDisplayFieldPrompt = value; }
        }
        #endregion

        public TIssForm()
            : base()
        {
        }
    }

	/// <summary>
	/// Summary description for TCancelForm.
	/// </summary>
	
	public class TCancelForm : Reino.ClientConfig.TIssForm
	{
		#region Properties and Members
		#endregion

		public TCancelForm()
			: base()
		{
		}
	}

	/// <summary>
	/// Summary description for TActivityLogForm.
	/// </summary>
	
	public class TActivityLogForm : Reino.ClientConfig.TIssForm
	{
		#region Properties and Members
		#endregion

		public TActivityLogForm()
			: base()
		{
		}
	}

	/// <summary>
	/// Summary description for TPublicContactForm.
	/// </summary>
	
	public class TPublicContactForm : Reino.ClientConfig.TIssForm
	{
		#region Properties and Members
		#endregion

		public TPublicContactForm()
			: base()
		{
		}
	}

	/// <summary>
	/// Summary description for TBaseDetailForm.
	/// </summary>
	
	public class TBaseDetailForm : Reino.ClientConfig.TIssForm
	{
		#region Properties and Members
		#endregion

		public TBaseDetailForm()
			: base()
		{
		}
	}

	/// <summary>
	/// Summary description for TBaseSearchForm.
	/// </summary>
	
	public class TBaseSearchForm : Reino.ClientConfig.TIssForm
	{
		#region Properties and Members
		#endregion

		public TBaseSearchForm()
			: base()
		{
		}
	}

	/// <summary>
	/// Summary description for TSearchMatchForm.
	/// </summary>
	
	public class TSearchMatchForm : Reino.ClientConfig.TBaseSearchForm
	{
		#region Properties and Members
		#endregion

		public TSearchMatchForm()
			: base()
		{
		}
	}

	/// <summary>
	/// Summary description for TSearchForm.
	/// </summary>
	
	public class TSearchForm : Reino.ClientConfig.TBaseSearchForm
	{
		#region Properties and Members
		#endregion

		public TSearchForm()
			: base()
		{
		}
	}

    /// <summary>
    /// Summary description for TMarkModeForm.
    /// </summary>

    public class TMarkModeForm : Reino.ClientConfig.TSearchForm
    {
        #region Properties and Members
        #endregion

        public TMarkModeForm()
            : base()
        {
        }
    }

    /// <summary>
    /// Summary description for THotSheetForm.
    /// </summary>

    public class THotSheetForm : Reino.ClientConfig.TSearchForm
    {
        #region Properties and Members
        protected string _MatchFieldsName = "";
        public string MatchFieldsName
        {
            get { return _MatchFieldsName; }
            set { _MatchFieldsName = value; }
        }

        #endregion

        public THotSheetForm()
            : base()
        {
        }
    }

    /// <summary>
	/// Summary description for TNotesForm.
	/// </summary>
	
	public class TNotesForm : Reino.ClientConfig.TBaseDetailForm
	{
		#region Properties and Members
		#endregion

		public TNotesForm()
			: base()
		{
		}
	}
}
