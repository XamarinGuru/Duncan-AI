// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 7/21/09 7:22a $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Common/ClientConfig/Edits.cs $
//              Revision: $Revision: 11 $

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Reino.ClientConfig
{
	/// <summary>
	/// TEditPrompt is a textual label usually displayed next to an edit control.
	/// </summary>
	
    public class TEditPrompt : Reino.ClientConfig.TWinClass
    {
        public enum TPromptAlignment
        {
            TopCenter = 0,
            TopLeft,
            TopRight,
            Left,
            Right,
            AbsCoord
        }

        #region Properties and Members
		protected TPromptAlignment _PromptAlignment = TPromptAlignment.TopLeft;
        [System.ComponentModel.DefaultValue(TPromptAlignment.TopLeft)] // This prevents serialization of default values
        public TPromptAlignment PromptAlignment
        {
            get { return _PromptAlignment; }
            set { _PromptAlignment = value; }
        }

        protected string _ComponentName = "";
        /// <summary>
        /// For the benefit of AI.NET manual entry.  The layout is written with a ComponentName
        /// that refers to the name of another control that will be used for the label's text.
        /// </summary>
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string ComponentName
        {
            get { return _ComponentName; }
            set { _ComponentName = value; }
        }
        #endregion

		public TEditPrompt()
			: base()
		{
		}
    }

	/// <summary>
	/// Base class for edit controls which accept keyboard input.
	/// </summary>
	
    public class TTEdit : Reino.ClientConfig.TTControl
    {
        #region Properties and Members
		protected TEditPrompt _PromptWin = new TEditPrompt();
		public TEditPrompt PromptWin
		{
			get { return _PromptWin; }
			set { _PromptWin = value; }
		}

        protected List<TEditCondition> _Conditions = new List<TEditCondition>();
        /// <summary>
        /// A collection of TEditCondition objects
        /// </summary>
        public List<TEditCondition> Conditions
        {
            get { return _Conditions; }
            set { _Conditions = value; }
        }

        protected List<TEditRestriction> _Restrictions = new List<TEditRestriction>();
        /// <summary>
        /// A collection of TEditRestriction objects
        /// </summary>
        public List<TEditRestriction> Restrictions
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
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public bool PreventPrompt
        {
            get { return _PreventPrompt; }
            set { _PreventPrompt = value; }
        }

        protected bool _IsVioField = false;
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public bool IsVioField
        {
            get { return _IsVioField; }
            set { _IsVioField = value; }
        }

        protected bool _IsNotesMemo = false;
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public bool IsNotesMemo
        {
            get { return _IsNotesMemo; }
            set { _IsNotesMemo = value; }
        }

        protected bool _IsColorField = false;
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public bool IsColorField
        {
            get { return _IsColorField; }
            set { _IsColorField = value; }
        }
        #endregion

		public TTEdit()
			: base()
		{
		}
    }

	/// <summary>
	/// TDrawField is used for free-form stylus drawing, such as for signatures and diagrams.
	/// </summary>
	
	public class TDrawField : Reino.ClientConfig.TTEdit
	{
		#region Properties and Members
		#endregion

		public TDrawField()
			: base()
		{
		}
	}

	/// <summary>
	/// Summary description for TEditField.
	/// </summary>
	
	public class TEditField : Reino.ClientConfig.TTEdit
	{
		public enum TEditFieldType
		{
			efString = 0,
			efDate,
			efTime,

            efDivider


		}

		#region Properties and Members
        protected TTDBListBox _DBListGrid = new TTDBListBox();
        public TTDBListBox DBListGrid
		{
			get { return _DBListGrid; }
			set { _DBListGrid = value; }
		}

		protected string _ListName = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string ListName
		{
			get { return _ListName; }
			set { _ListName = value; }
		}

		protected string _EditMask = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string EditMask
		{
			get { return _EditMask; }
			set { _EditMask = value; }
		}

		protected TEditFieldType _FieldType = TEditFieldType.efString;
        [System.ComponentModel.DefaultValue(TEditFieldType.efString)] // This prevents serialization of default values
        public TEditFieldType FieldType
		{
			get { return _FieldType; }
			set { _FieldType = value; }
		}

		#endregion

		public TEditField()
			: base()
		{
		}
    }

	/// <summary>
	/// Summary description for TEditListBox.
	/// </summary>
	
	public class TEditListBox : Reino.ClientConfig.TEditField
	{
		#region Properties and Members
		#endregion

		public TEditListBox()
			: base()
		{
		}
	}

	/// <summary>
	/// Edit control for password fields (visually obfuscated input).
	/// </summary>
	
	public class TPwdEditField : Reino.ClientConfig.TEditField
	{
		#region Properties and Members
		#endregion

		public TPwdEditField()
			: base()
		{
		}
	}

	/// <summary>
	/// Edit control for numeric fields.
	/// </summary>
	
	public class TNumEdit : Reino.ClientConfig.TEditField
	{
		#region Properties and Members
		#endregion

		public TNumEdit()
			: base()
		{
		}
	}

	/// <summary>
	/// Summary description for TIssEdit.
	/// </summary>
	
	public class TIssEdit : Reino.ClientConfig.TEditField
	{
		#region Properties and Members
		#endregion

		public TIssEdit()
			: base()
		{
		}
	}

	/// <summary>
	/// Edit control for Multi-line text input.
	/// </summary>
	
	public class TTMemo : Reino.ClientConfig.TTEdit
	{
		#region Properties and Members
		#endregion

		public TTMemo()
			: base()
		{
		}
	}

	/// <summary>
	/// Column object used for list boxes/grids
	/// </summary>
	
	public class TGridColumnInfo : Reino.ClientConfig.TObjBase
	{
		#region Properties and Members
		protected int _Width = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int Width
		{
			get { return _Width; }
			set { _Width = value; }
		}

		protected string _ColTitle = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string ColTitle
		{
			get { return _ColTitle; }
			set { _ColTitle = value; }
		}

		protected string _Mask = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string Mask
		{
			get { return _Mask; }
			set { _Mask = value; }
		}
		#endregion

		public TGridColumnInfo()
			: base()
		{
		}
	}

	/// <summary>
	/// Base class for list boxes
	/// </summary>
	
	public class TBaseListBox : Reino.ClientConfig.TWinClass
	{
		#region Properties and Members
		protected bool _DisplayColTitles = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool DisplayColTitles
		{
			get { return _DisplayColTitles; }
			set { _DisplayColTitles = value; }
		}

        protected List<TGridColumnInfo> _Columns;
		/// <summary>
		/// Collection of TGridColumnInfo objects
		/// </summary>
        public List<TGridColumnInfo> Columns
		{
			get { return _Columns; }
			set { _Columns = value; }
		}
		#endregion

		public TBaseListBox()
			: base()
		{
            _Columns = new List<TGridColumnInfo>();
		}
	}

	/// <summary>
	/// Summary description for TDBListBox.
	/// </summary>
	
	public class TTDBListBox : Reino.ClientConfig.TBaseListBox
	{
		#region Properties and Members
		#endregion

		public TTDBListBox()
			: base()
		{
		}
	}

	/// <summary>
	/// Summary description for TStringListBox.
	/// </summary>
	
	public class TStringListBox : Reino.ClientConfig.TBaseListBox
	{
		#region Properties and Members
		#endregion

		public TStringListBox()
			: base()
		{
		}
	}
}
