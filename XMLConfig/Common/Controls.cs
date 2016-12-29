// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 3/19/08 11:20a $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Common/ClientConfig/Controls.cs $
//              Revision: $Revision: 13 $

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Reino.ClientConfig
{
	/// <summary>
	/// Base class for objects that will be visible (either on-screen or printed).
	/// </summary>
	
	public class TWinBase : Reino.ClientConfig.TObjBase
	{
        public enum TFont
        {
            Font8x8 = 0,
            Font12x12,
            Font12x12Bold,
            Font16x16,
            Font20x20,
            Font24x24,
            FontBC128,
            FontOCR,
            FontBC39,
            Font36x36,
			FontBC3of9,
			Font16x16Bold
        }

        public enum THandheldColor
        {
            LCDColor_BLACK,
            LCDColor_BKGDGRAY,
            LCDColor_BKGLGRAY,
            LCDColor_WHITE
        }

		#region Properties and Members
		protected TFont _Font = TFont.Font8x8;
        // JLA 19-MAR-2008: Don't set serialization default because some descendants have different defaults
        //   which can result in bad configs. For example, if a TWinPrnPrompt is set to Font8x8, it wouldn't
        //   get written since Font8x8 is default as this level, but TWinPrnPrompt default is Font16x16.
        /*
        [System.ComponentModel.DefaultValue(TFont.Font8x8)] // This prevents serialization of default values
        */
        public TFont Font
		{
			get { return _Font; }
			set { _Font = value; }
		}

		protected int _Top = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int Top
		{
			get { return _Top; }
			set { _Top = value; }
		}

		protected int _Left = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int Left
		{
			get { return _Left; }
			set { _Left = value; }
		}

		protected int _Width = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int Width
		{
			get { return _Width; }
			set { _Width = value; }
		}

		protected int _Height = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int Height
		{
			get { return _Height; }
			set { _Height = value; }
		}

        protected THandheldColor _TextColor = THandheldColor.LCDColor_BLACK;
        [System.ComponentModel.DefaultValue(THandheldColor.LCDColor_BLACK)] // This prevents serialization of default values
        public THandheldColor TextColor
		{
			get { return _TextColor; }
			set { _TextColor = value; }
		}

        protected THandheldColor _BackgroundColor = THandheldColor.LCDColor_BKGDGRAY;
        [System.ComponentModel.DefaultValue(THandheldColor.LCDColor_BKGDGRAY)] // This prevents serialization of default values
        public THandheldColor BackgroundColor
		{
			get { return _BackgroundColor; }
			set { _BackgroundColor = value; }
		}

		protected int _FrameThickness = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int FrameThickness
		{
			get { return _FrameThickness; }
			set { _FrameThickness = value; }
		}

		protected string _TextBuf = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string TextBuf
		{
			get { return _TextBuf; }
			set { _TextBuf = value; }
		}

		protected int _FontHorzMult = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int FontHorzMult
		{
			get { return _FontHorzMult; }
			set { _FontHorzMult = value; }
		}

		protected int _FontVertMult = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int FontVertMult
		{
			get { return _FontVertMult; }
			set { _FontVertMult = value; }
		}
		#endregion

		public TWinBase()
			: base()
		{
		}
	}

	/// <summary>
	/// Base class for on-screen objects, most of which are controls.
	/// </summary>
	
	/* The XmlInclude attribute is used on a base type to indicate that when serializing 
	 * instances of that type, they might really be instances of one or more subtypes. 
	 * This allows the serialization engine to emit a schema that reflects the possibility 
	 * of really getting a Derived when the type signature is Base. For example, we keep
	 * field definitions in a generic collection of TWinClass. If an array element is 
	 * TTControl, the XML serializer gets mad because it was only expecting TWinClass. 
	 */
	[XmlInclude(typeof(TTControl)), XmlInclude(typeof(TTButton)),
	 XmlInclude(typeof(TTBitBtn)), XmlInclude(typeof(TTPanel)), 
	 XmlInclude(typeof(TSheet)), XmlInclude(typeof(TTTabSheet)), 
     XmlInclude(typeof(TEditPrompt)),
	 XmlInclude(typeof(TTEdit)), XmlInclude(typeof(TDrawField)),
	 XmlInclude(typeof(TEditField)), XmlInclude(typeof(TEditListBox)),
	 XmlInclude(typeof(TPwdEditField)), XmlInclude(typeof(TNumEdit)),
	 XmlInclude(typeof(TIssEdit)), XmlInclude(typeof(TTMemo)),
	 XmlInclude(typeof(TBaseListBox)), XmlInclude(typeof(TStringListBox)),
	 XmlInclude(typeof(TTDBListBox)), XmlInclude(typeof(TTBitmap))]
	public class TWinClass : Reino.ClientConfig.TWinBase
	{
        #region Properties and Members
        #endregion

		public TWinClass()
			: base()
		{
		}
	}

    /// <summary>
    /// Enumerations for field categories needed by the issuance app. This is used to enhance
    /// performance by reducing the number of name comparisons needed while running application.
    /// </summary>
    public enum FieldCategories
    {
        None,
        IssueNoElement,
        OfficerNameOrID,
        IssueDateOrTime,
        NoteDateOrTime,
        RecCreationDateOrTime,
        PlateOrState,
        VioCodeOrFine,
        VoidOrReinstate,
        SigReqOrMisdemeanor,
        SuspBlockOrStreet
    }

    /// <summary>
    /// Enumerations for specific known fields needed by the issuance app. This is used to enhance
    /// performance by reducing the number of name comparisons needed while running application.
    /// </summary>
    public enum KnownFields
    {
        None,
        DetailRecNo,
        IssueNoDisplay,
        IsWarning,
        MatchRecs,
        HotDispo,
        BtnDone,
        SearchButton,
        BtnReadReino,
        LocMeter
    }

	/// <summary>
	/// Base class for controls, which are objects that react to navigation or data input
	/// </summary>
	public class TTControl : Reino.ClientConfig.TWinClass
    {
        #region Properties and Members
        // Association with a Windows control (usually a ReinoTextBox or a button)
        /*
        private System.Windows.Forms.Control _WinCtrl = null;
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public System.Windows.Forms.Control WinCtrl
        {
            get { return _WinCtrl; }
            set { _WinCtrl = value; }
        }
        */
        // Association with a TextBoxBehavior
        private ReinoControls.TextBoxBehavior _Behavior = null;
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public ReinoControls.TextBoxBehavior Behavior
        {
            get { return _Behavior; }
            set { _Behavior = value; }
        }

        public Type TypeOfWinCtrl()
        {
        /* KLC
            // Virtual List Box?
            if (this is TEditListBox)
                return typeof(ReinoControls.ReinoVirtualListBox);
            // DrawBox?
            if (this is TDrawField)
                return typeof(ReinoControls.ReinoDrawBox);
            // ReinoTextBox?
            if (this is TTEdit)
                return typeof(ReinoControls.ReinoTextBox);
            // Button?
            if (this is TTButton)
                return typeof(ReinoControls.ReinoNavButton);
            // Bitmap?
            if (this is TTBitmap)
                return typeof(System.Windows.Forms.PictureBox);
            // Don't care about any other types
         */ 
            return typeof(System.DBNull);
        }

        /// <summary>
        /// FieldCategory is only used by the handheld issuance application
        /// </summary>
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public FieldCategories FieldCategory = FieldCategories.None;

        /// <summary>
        /// KnownField is only used by the handheld issuance application
        /// </summary>
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public KnownFields KnownField = KnownFields.None;

        private bool _IsHidden = false;
        private bool _IsProtected = false;
        private bool _IsEnabled = true;
        private bool _IsEnabledLocked = false;

        /// <summary>
        /// IsHidden will be set when an edit restriction hides the field
        /// </summary>
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public bool IsHidden
        {
            get { return _IsHidden; }
            set { _IsHidden = value; }
        }

        /// <summary>
        /// IsProtected will be set when an edit restriction disables the field
        /// </summary>
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public bool IsProtected
        {
            get { return _IsProtected; }
            set { _IsProtected = value; }
        }

        /// <summary>
        /// IsEnabled will be set to false when an field is disabled in general (ie. because it was printed/saved, etc)
        /// </summary>
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                // Don't allow the property change if we're "locked"
                if (_IsEnabledLocked == false)
                    _IsEnabled = value;
            }
        }
        #endregion

        public void SetEnabledLocked(bool iEnabledLocked)
        {
            _IsEnabledLocked = iEnabledLocked;
        }

		public TTControl()
			: base()
		{
		}
    }

	/// <summary>
	/// A button control
	/// </summary>
	
	public class TTButton : Reino.ClientConfig.TTControl
	{
		#region Properties and Members
		protected char _HotKey;
		public char HotKey
		{
			get { return _HotKey; }
			set { _HotKey = value; }
		}

		protected TWinClass _PromptWin = new TWinClass();
		public TWinClass PromptWin
		{
			get { return _PromptWin; }
			set { _PromptWin = value; }
		}

        protected bool _Enabled = true;
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; }
        }

        protected bool _Visible = true;
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public bool Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public int SharesRowWithIdx = -1;
		#endregion

		public TTButton()
			: base()
		{
		}
	}

	/// <summary>
	/// A button control capable of displaying a bitmap
	/// </summary>
	
	public class TTBitBtn : Reino.ClientConfig.TTButton
	{
		#region Properties and Members
		#endregion

		public TTBitBtn()
			: base()
		{
		}
	}

    /// <summary>
    /// Summary description for TTBitmap.
    /// </summary>
    public class TTBitmap : Reino.ClientConfig.TTControl
    {
        #region Properties and Members
        #endregion

        public TTBitmap()
            : base()
        {
            // Set our defaults that differ from our parent.
            FrameThickness = 2;
            Height = 90;
            Width = 120;
        }
    }

	/// <summary>
	/// A panel is a screen area that can house mutliple other visible objects
	/// </summary>
	
	public class TTPanel : Reino.ClientConfig.TTControl
	{
		#region Properties and Members
		protected List<TWinClass> _Controls = new List<TWinClass>();
		/// <summary>
		/// A collection of TWinClass objects
		/// </summary>
		public List<TWinClass> Controls
		{
			get { return _Controls; }
			set { _Controls = value; }
		}
		#endregion

		public TTPanel()
			: base()
		{
            // Set our defaults that differ from our parent.
            FrameThickness = 2;
            Height = 50;
            Width = 100;
        }
	}

	/// <summary>
	/// Summary description for TSheet.
	/// </summary>
	
	public class TSheet : Reino.ClientConfig.TTPanel
	{
		#region Properties and Members
		protected char _HotKey;
		public char HotKey
		{
			get { return _HotKey; }
			set { _HotKey = value; }
		}
		#endregion

		public TSheet()
			: base()
		{
		}
	}

	/// <summary>
	/// Summary description for TTTabSheet.
	/// </summary>
	
	public class TTTabSheet : Reino.ClientConfig.TTPanel
	{
		#region Properties and Members
        protected List<TSheet> _Sheets = new List<TSheet>();
        public List<TSheet> Sheets
        {
            get { return _Sheets; }
            set { _Sheets = value; }
        }

		#endregion

		public TTTabSheet()
			: base()
		{
		}
	}


  
}
