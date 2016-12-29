// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 12/04/13 8:15a $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Handheld/ReinoTextBoxBehavior.cs $
//              Revision: $Revision: 73 $

#define DEBUG

using System;
using System.IO;
// KLC using System.Data;
using System.Diagnostics;
using System.Text;
using System.Drawing;
//using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using Reino.ClientConfig;
using System.Xml.Serialization;
//using System.Web.UI;
// KLC using System.Web.UI.WebControls;

/*  WARNING: This is a shared file for Desktop and Handheld, so code needs to be compatible with 
 *           the .NET Compact Framework! If you see code in here that doesn't look optimal, you 
 *           need to realize that the way you would do it may not be supported by the 
 *           .NET Compact Framework for Windows CE.
 */

/* ****************************************
 * *** IMPORTANT WINDOWS CE INFORMATION ***
 * ****************************************
 * In order to use the ReinoTextBox in a Windows CE application, the applications message loop must
 * be handled by ReinoApplicationEx instead of Application. Here is an example of the Main() method
 * in "Program.cs".
 * 
 * Here is an example of usage for Windows CE:
 * 
 *		static void Main()
 *		{
 *			// Since we are using ReinoTextBox in this application, we need message filtering which isn't
 *			// provided by default in the Compact Framework. So we need to use the ReinoApplicationEx object 
 *			// instead which introduces message filtering.
 *			ReinoApplicationEx.Run(new Form1());
 *		}
 * 
 * In addition, showing forms modally requires using the ReinoApplicationEx.ShowDialog 
 * method instead of [Form].ShowDialog. (Otherwise message filtering won't be available
 * in the modal form because it will be in a different thread)
 * For example, use:
 *		ReinoApplicationEx.ShowDialog(loPwd_Logon)
 * instead of:
 *		loPwd_Logon.ShowDialog()
 * 
 * Along the same lines, the following Application functions should be executed via 
 * ReinoApplicationEx instead of Application:
 *		DoEvents()
 *		Exit()
 *		Run()
 *		ShowDialog()
 */

/* *************************************************
 * *** IMPORTANT FULL .NET FRAMEWORK INFORMATION ***
 * *************************************************
 * In order for the PopupListBox of a ReinoTextBox to closeup when the user switches to another application,
 * the main form's WndProc needs to be overridden to close popups under this circumstance. 
 * Below is an example:
 * 
 * protected override void WndProc(ref System.Windows.Forms.Message m)
 * {
 *		base.WndProc(ref m);
 *		const int WM_ACTIVATEAPP = 0x01C;
 *		if ((m.Msg == WM_ACTIVATEAPP) && ((int)m.WParam == 0))
 *			ReinoControls.ReinoMessageFilters.PopupListBoxFilter.ClosePopups();
 * }
 */

namespace ReinoControls
{
    #region Windows API Interface
    // IPlatformDependent is an interface for OS dependant routines. 
    // An example woud be Windows API calls that are in different DLLs on WinCE and Win32 platforms.
    // Any changes to this interface must be reflected in "WinCEAPI" and "Win32API" which BOTH appear
    // in "WinCEMessageFilter.cs" and "Win32MessageFilter.cs"!
    public interface IPlatformDependent
    {
        bool PeekMessage(out ReinoControls.WinMessage msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);
        void WaitMessage();
        int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        int ShowWindow(IntPtr hWnd, int nCmdShow);
        System.Drawing.Rectangle GetWorkingArea();
        bool RenderWithVisualStyles();
        //KLC void AddDoubleClickEvent(EventHandler pEventHandler, Control pControl);
        //void AddMouseClickEvent(MouseEventHandler pEventHandler, Control pControl);
        //void AddMouseMoveEvent(MouseEventHandler pEventHandler, Control pControl);
        //KLC void RemoveDoubleClickEvent(EventHandler pEventHandler, Control pControl);
        //void RemoveMouseClickEvent(MouseEventHandler pEventHandler, Control pControl);
        //void RemoveMouseMoveEvent(MouseEventHandler pEventHandler, Control pControl);
        void MessageBeep();
        int PlaySound(string szSound, IntPtr hMod, int flags);
        int waveOutSetVolume(IntPtr hwo, int dwVolume);
        int waveOutGetVolume(IntPtr hwo, ref int pdwVolume);
        //DialogResult ReinoShowDialog(Form form, System.EventHandler ShownEventForCE, System.EventHandler ShownEventAfterVisibleForCE);
        //DialogResult ReinoShowDialog(Form form, Form prevForm, bool disposeForm, System.EventHandler ShownEventForCE, System.EventHandler ShownEventAfterVisibleForCE);
        IntPtr GetCapture();
    }
    #endregion

    #region Handheld Keyboard Characters
    // Special Characters recognized by ProcessKeyStroke. (non-character keys need to be translated to these)
    static public class ReinoKeys
    {
        public const char chDEL = ((char)0x80);
        public const char chUPARROW = ((char)0x81);
        public const char chDNARROW = ((char)0x82);
        public const char chLCDUPARROW = chUPARROW;
        public const char chLCDDNARROW = chDNARROW;
        public const char chPGUP = ((char)0x83);
        public const char chPGDN = ((char)0x84);
        public const char chLFARROW = ((char)0x85);
        public const char chRTARROW = ((char)0x86);
        public const char chLCDLFARROW = chLFARROW;
        public const char chLCDRTARROW = chRTARROW;
        public const char chPREV = ((char)0x87);
        public const char chNEXT = ((char)0x88);
        public const char chCLEAR = ((char)0x89);
        public const char chESC = ((char)0x1B);
        public const char chSYS = ((char)0x8B);
        public const char chHELP = ((char)0x8C);
        public const char chBKLT = ((char)0x8D);
        public const char chFFLF = ((char)0x8E);
        public const char chACC = ((char)0x8F);
        public const char chCHECKMARK = ((char)0xFB);
        public const char chHOME = ((char)0x90);
        public const char chEND = ((char)0x91);
        public const char chTESTMENU = ((char)0x92);
        public const char chLCDBIASUP = ((char)0x93);
        public const char chLCDBIASDN = ((char)0x94);
        public const char chTAB = ((char)0x9);
        public const char chBS = ((char)0x8);
        public const char chLF = ((char)0xA);
        public const char chCR = ((char)0xD);
    }
    #endregion

    #region Message Filters
    static public class ReinoMessageFilters
    {
        // Create the Message Filters that will be used.
        // Note: It's important that the PopupListBoxFilter is added to the application's
        //       message loop before TextBoxMouseEventFilter, so it MUST be created first!
        /* KLC
        public static PopupListBoxMessageFilter PopupListBoxFilter = new PopupListBoxMessageFilter();
        public static StaticListBoxMessageFilter StaticListBoxFilter = new StaticListBoxMessageFilter();
        public static ReinoTextBoxMessageFilter TextBoxMouseEventFilter = new ReinoTextBoxMessageFilter();
        public static PopupBalloonMsgFilter PopupBalloonMsgEventFilter = new PopupBalloonMsgFilter();
         */ 
#if WindowsCE || __ANDROID__
        /* KLC
        // ReinoRadioList only needs message filtering under WindowsCE
        public static ReinoRadioListMessageFilter RadioListEventFilter = new ReinoRadioListMessageFilter();
        public static BasicButtonMessageFilter BasicButtonEventFilter = new BasicButtonMessageFilter();
        public static VirtualListBoxMessageFilter VirtualListBoxEventFilter = new VirtualListBoxMessageFilter();
         */
#endif

        public static void Init()
        {
            /* KLC
                if (PopupListBoxFilter == null)
                    PopupListBoxFilter = new PopupListBoxMessageFilter();
                if (StaticListBoxFilter == null)
                    StaticListBoxFilter = new StaticListBoxMessageFilter();
                if (PopupBalloonMsgEventFilter == null)
                    PopupBalloonMsgEventFilter = new PopupBalloonMsgFilter();
                if (TextBoxMouseEventFilter == null)
                    TextBoxMouseEventFilter = new ReinoTextBoxMessageFilter();
    #if WindowsCE || __ANDROID__
                if (RadioListEventFilter == null)
                    RadioListEventFilter = new ReinoRadioListMessageFilter();
                if (BasicButtonEventFilter == null)
                    BasicButtonEventFilter = new BasicButtonMessageFilter();
                if (VirtualListBoxEventFilter == null)
                    VirtualListBoxEventFilter = new VirtualListBoxMessageFilter();
    #endif
             */
        }

        // Helper function to extract the Hi word from an integer
        public static int HiWord(int number)
        {
            if ((number & 0x80000000) == 0x80000000)
                return (number >> 16);
            else
                return (number >> 16) & 0xffff;
        }

        // Helper function to extract the Lo word from an integer
        public static int LoWord(int number)
        {
            return number & 0xffff;
        }

        // Helper function to create a long integer from two integers
        public static int MakeLong(int LoWord, int HiWord)
        {
            return (HiWord << 16) | (LoWord & 0xffff);
        }

        // Helper function to create a LParam based on two integers
        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }
    }
    #endregion

    #region Enumerations
    public enum TEditFieldType
    {
        efString = 0,
        efDate,
        efTime,
        efNumeric //Integer, Floating Point and Currency
    }

    public enum WinControl_SetText_Options
    {
        sto_InsertTextAtPos,
        sto_ReplaceTextAtPos,
        sto_InsertTextAfterPos,
        sto_ReplaceTextAfterPos,
        sto_ReplaceText
    }

    public enum ListBoxStyle
    {
        lbsPopup,
        lbsStatic,
        lbsNone,
        lbsRadio
    }
    #endregion

    #region Windows Message Structure
    /// <summary>
    /// WinMessage is the Windows MSG structure used by the PeekMessage function in the Windows API 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WinMessage
    {
        public IntPtr hWnd;
        public uint msg;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public System.Drawing.Point p;
    }
    #endregion

    /// <summary>
    /// Behavior object designed to be attached to TextBox.
    /// This handles key and mouse events to implement edit mask, anticipation, etc...
    /// </summary>
    public class TextBoxBehavior : IDisposable
    {
        #region EditMask Attributes
        // Edit mask attributes
        const int emaFloatingPoint = 0x0001;     // mask has a decimal point that is NOT in a fixed position
        const int emaFixedPoint = 0x0002;        // mask has a decimal point that IS in a fixed position
        const int emaFixedMask = 0x0004;         // mask has literals in fixed positions
        const int emaSingleCharMask = 0x0008;    // mask consists of a single character that applies to all positions
        const int emaDateMask = 0x0010;          // mask contains characters that represent a date format
        const int emaTimeMask = 0x0020;          // mask contains characters that represent a time format
        const int emaNegativeNumbers = 0x0040;   // mask is prefixed with an optional "-" symbol to represent negative numbers
        const int emaCommaSeparated = 0x0080;    // mask uses commas to seperate groups of 1000 (ie. 1,2345 instead of 12345)
        const int emaCurrencySign = 0x0100;      // mask is prefixed by a "$" symbol ("-" prefix gets first priority though)
        const int emaZeroPadFractional = 0x0200; // mask will pad the fractional digits that appear after decimal point
        #endregion

        #region Private Members
        private string _Text = "";
        private string _EditMask = "";
        private string _EditBuffer = "";
        private string _SavedEditBuffer = "";
        private string _NaturalListSourceFieldName = "";
        private string _ListSourceFieldName = "";

        private int _EditMaskLen = 0;
        private int _MaskAttrs = 0;
        private int _CursorCharPos = 0;
        private int _MaxLength = 80; // Default max length to match legacy code
        private int _TextSelectionStart = 0;
        private int _TextSelectionEnd = 0;
        private int _DependentNotificationDepth = 0;
        private int _ProcessRestrictionsDepth = 0;
        private int _EditStateAttrs = 0;
        private int _ListPrimaryKey = -1;
        private int _ListNdx = -1;
        private int _ListSourceFieldIdx = -1;
        private int _ListItemCount = 0;

        /* KLC
        private ReinoControls.ReinoVirtualListBox.FixedSizeCache _ListItemCache = null;
        private ReinoControls.ReinoVirtualListBox.FixedSizeCache _GridDisplayCache = null;
        */

        private bool _ExecutingSetEditBufferInternal = false;
        private bool _HasRightMargin = false;
        private bool _ListNdxNotTrusted = false;

        private Object _EditCtrl = null;
        internal Reino.ClientConfig.TTEdit _CfgCtrl = null;
        private Reino.ClientConfig.TTTable _LocalListSourceTable = null;
        private Reino.ClientConfig.TTTable _PopularListSourceTable = null;
        private Reino.ClientConfig.TTTable _NaturalListSourceTable = null;
        private Reino.ClientConfig.TTTable _ListSourceTable = null;

        private ListBoxStyle _ListStyle = ListBoxStyle.lbsNone;
        /* KLC
        private ReinoTextBox.PopupListBox _PopupListBox = null;
        private ReinoTextBox.StaticListBox _StaticListBox = null;
        private ReinoRadioList _RadioListBox = null;
        private ReinoRadioList _Popular = null;
        */
        private TEditFieldType _FieldType;

        #endregion
        #region Public Properties / Members
        //KLC public Label PromptLabel = null;

        #region Properties (Visual State)
        //KLC  public Font NormalFont = null;
        //KLC public Font FocusedFont = null;
        //KLC static public Control InvalidatedWindowAfterEditRestrictions = null;
        #endregion

        #region Properties (Edit State)
        // This is used to prevent Validating event from firing twice when focus changes due to keyboard event
        // (It is also public so the application can fine-tune the validations if desired)
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public bool SkipNextValidation = false;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public bool ValidationDisabled = false;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public bool AnticipationDisabled = false;
        #endregion

        #region Properties (Associations)
        /// <summary>
        /// Association (if any) to an edit control in client configuration
        /// </summary>
        
        
        public TTEdit CfgCtrl
        {
            get { return _CfgCtrl; }
            set { _CfgCtrl = value; }
        }
         

        /// <summary>
        /// Edit control associated with this Behavior object. 
        /// </summary>
        public Object EditCtrl
        {
            get { return _EditCtrl; }
            set { _EditCtrl = value; }
        }

        // KLC public ReinoControls.PopupBalloonMsg PopupBalloon = null;
        public List<ReinoControls.TextBoxBehavior> BehaviorCollection = null;

        // KLC public List<Reino.ClientConfig.TEditRestriction> EditRestrictions = new List<Reino.ClientConfig.TEditRestriction>();
        // KLC public List<Reino.ClientConfig.TEditCondition> EditConditions = new List<Reino.ClientConfig.TEditCondition>();
        // KLC public List<ReinoControls.TextBoxBehavior> Dependents = new List<TextBoxBehavior>();
        #endregion

        #region Properties (List related)
        public Reino.ClientConfig.TTTable LocalListSourceTable
        {
            get { return _LocalListSourceTable; }
            set { _LocalListSourceTable = value; }
        }

        public Reino.ClientConfig.TTTable PopularListSourceTable
        {
            get { return _PopularListSourceTable; }
            set { _PopularListSourceTable = value; }
        }

        public Reino.ClientConfig.TTTable NaturalListSourceTable
        {
            get { return _NaturalListSourceTable; }
            set { _NaturalListSourceTable = value; }
        }

        public Reino.ClientConfig.TTTable ListSourceTable
        {
            get
            {
                // If ListSourceTable isn't set for some reason but we have a "RegularListSourceTableDef",
                // then thats the one we want to use.
                if ((_ListSourceTable == null) && (NaturalListSourceTable != null))
                    _ListSourceTable = NaturalListSourceTable;

                return _ListSourceTable;
            }
            set
            {
                // Don't need to do anything unless the value is changing
                if (_ListSourceTable != value)
                {
                    // Set active list source to passed value
                    _ListSourceTable = value;

                    // Reset list item count and cache
                    ResetListItemStats();

                    // Create popular table if applicable
                    InitPopularItemsTable();

                    // If local list table isn't set yet, this is it
                    if ((_LocalListSourceTable == null) && (_ListSourceTable != null))
                        _LocalListSourceTable = _ListSourceTable;
                }
            }
        }

        public string NaturalListSourceFieldName
        {
            get { return _NaturalListSourceFieldName; }
            set { _NaturalListSourceFieldName = value; }
        }

        public string ListSourceFieldName
        {
            get
            {
                return _ListSourceFieldName;
            }
            set
            {
                // Don't need to do anything unless the value is changing
                if (_ListSourceFieldName != value)
                {
                    _ListSourceFieldName = value;
                    // Reset list item count and cache
                    ResetListItemStats();

                    // Find field index
                    if (ListSourceTable != null)
                    {
                        TObjBasePredicate predicate = new TObjBasePredicate(_ListSourceFieldName);
                        _ListSourceFieldIdx = ListSourceTable.fTableDef.HighTableRevision.Fields.FindIndex(predicate.CompareByName_CaseInsensitive);
                    }
                    else
                    {
                        _ListSourceFieldIdx = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the PopupListBox associated with this Behavior object. 
        /// </summary>
        /// 

        /* KLC
        public ReinoTextBox.PopupListBox Popup
        {
            get { return _PopupListBox; }
            set { _PopupListBox = value; }
        }

        public ReinoControls.ReinoVirtualListBox ListBox
        {
            get
            {
                if (_ListStyle == ListBoxStyle.lbsPopup)
                    return _PopupListBox;
                else if (_ListStyle == ListBoxStyle.lbsStatic)
                    return _StaticListBox;
                else if (_ListStyle == ListBoxStyle.lbsRadio)
                    return _RadioListBox;
                else
                    return null;
            }
        }
         */ 
        /* KLC
        public ReinoRadioList Popular
        {
            get { return _Popular; }
            set { _Popular = value; }
        }
        */

        /// <summary>
        /// This method should be used to change the visibility of the associated listbox.
        /// Our list boxes descend from the same base, however the "Visible" property does not
        /// support polymorphism, so the actual classtype of the the listbox must be utilized.
        /// </summary>
        public void SetListBoxVisible(bool WantVisible)
        {
        /* KLC
            if (_PopupListBox != null)
                _PopupListBox.Visible = WantVisible;

            if (_StaticListBox != null)
                _StaticListBox.Visible = WantVisible;

            if (_RadioListBox != null)
                _RadioListBox.Visible = WantVisible;
         */ 
        }

        public ListBoxStyle ListStyle
        {
            get
            {
                return _ListStyle;
            }
            set
            {
                _ListStyle = value;

                // Destroy any listboxes that are not applicable
                // Note: It is important that this be done by type, because although all 
                // listboxes are inherited from the same base, the "Visible" property does
                // not support polymorphism.
                /*
                if (_ListStyle != ListBoxStyle.lbsPopup)
                {
                    RemoveDropDownButton();
                    if (_PopupListBox != null)
                    {
                        _PopupListBox.Visible = false;
                        _PopupListBox.Dispose();
                        _PopupListBox = null;
                    }
                }

                if ((_ListStyle != ListBoxStyle.lbsStatic) && (_StaticListBox != null))
                {
                    _StaticListBox.Visible = false;
                    _StaticListBox.Dispose();
                    _StaticListBox = null;
                }

                if ((_ListStyle != ListBoxStyle.lbsRadio) && (_RadioListBox != null))
                {
                    _RadioListBox.Visible = false;
                    _RadioListBox.Dispose();
                    _RadioListBox = null;
                }
                 */
            }
        }

        //KLC public ReinoControls.ReinoDropDownButton fDropDownButton = null;
        public Boolean ListContentsChangedByRestriction = false;

        public Boolean RelaxListOnlyRestriction = false;
        #endregion

        #region Properties (Data / Text)
        public string EditBuffer
        {
            get
            {
                return _EditBuffer;
            }
        }

        public int MaxLength
        {
            get
            {
                return _MaxLength;
            }
            set
            {
                if (value <= 0) return; // can't do that!
                _MaxLength = value;
            }
        }
        /* KLC
        public int ListItemCount
        {
            get
            {
                return _ListItemCount;
            }
            set
            {
                _ListItemCount = value;
                // Also set count in associated listbox if exits
                if (this.ListBox != null)
                {
                    this.ListBox.Count = _ListItemCount;
                }
            }
        }

        public ReinoControls.ReinoVirtualListBox.FixedSizeCache ListItemCache
        {
            get
            {
                return _ListItemCache;
            }
            set
            {
                _ListItemCache = value;
                // Also set cache in associated listbox if exits
                if (this.ListBox != null)
                {
                    this.ListBox.Cache = _ListItemCache;
                }
            }
        }
        public ReinoControls.ReinoVirtualListBox.FixedSizeCache GridDisplayCache
        {
            get
            {
                return _GridDisplayCache;
            }
            set
            {
                _GridDisplayCache = value;
                // Also set cache in associated listbox if exits
                if (this.ListBox != null)
                {
                    this.ListBox.DisplayCache = _GridDisplayCache;
                }
            }
        }
         */
        #endregion
        #endregion

        #region Delegates
        public delegate bool TabForward();
        public delegate bool TabBackward();
        public delegate Reino.ClientConfig.TTControl FindNextFormControl(TTControl AfterCtrl, bool MustBeEnabled);
        //KLC public delegate Control WhichControlIsFirst(Control Ctrl1, Control Ctrl2);
        //KLC public delegate Control GetFocusedControl();
        public delegate int GetFormEditMode();
        public delegate int GetFormEditAttrs();
        public delegate void SetFormEditAttr(int iAttribute, bool iSetAttr);
        /* KLC
        public delegate void NavigationKeyPress(KeyPressEventArgs e);
        public delegate void RegularKeyPress(KeyPressEventArgs e);
        public delegate void CreatedPopupListBox(ReinoTextBox.PopupListBox NewPopup);
        public delegate void CreatedRegularListBox(ReinoVirtualListBox NewListBox);
        public delegate ReinoVirtualListBox GetNewRegularListBox(ListBoxStyle ListStyle, ref bool IsNewlyCreated);
         */ 
        public delegate void CtrlGotFocus(object sender);
        public delegate int GetRingBellVolume();
        public delegate void CustomizeValidationErrorText(ref string oErrMsg, TextBoxBehavior behavior);
        #endregion

        #region Events
        public event TabForward OnTabForward = null;
        public event TabBackward OnTabBackward = null;
        public event FindNextFormControl OnFindNextFormControl = null;
        //KLC public event WhichControlIsFirst OnWhichControlIsFirst = null;
        //KLC public event GetFocusedControl OnGetFocusedControl = null;
        public event GetFormEditMode OnGetFormEditMode = null;
        public event GetFormEditAttrs OnGetFormEditAttrs = null;
        public event SetFormEditAttr OnSetFormEditAttr = null;
        /* KLC
        public event NavigationKeyPress OnNavigationKeyPress = null;
        public event RegularKeyPress OnRegularKeyPress = null;
        public event CreatedPopupListBox OnCreatedPopupListBox = null;
        public event CreatedRegularListBox OnCreatedRegularListBox = null;
        public event GetNewRegularListBox OnGetNewRegularListBox = null;
         */ 
        public event CtrlGotFocus OnCtrlGotFocus = null;
        public event EventHandler TextChanged = null;
        public event EventHandler NotifiedDependentsParentDataChanged = null;
        public event GetRingBellVolume OnGetRingBellVolume = null;
        public event CustomizeValidationErrorText OnCustomizeValidationErrorText = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Behavior class by associating it with an edit control. 
        /// </summary>
        public TextBoxBehavior()
        {
        }

        /*
        public TextBoxBehavior(Control Ctrl)
        {
            AssociateWithReinoTextBox(Ctrl);
        }
        */
        /* KLC
        public void AssociateWithReinoTextBox(ReinoTextBox Ctrl)
        {
            // Somebody's doing something wrong if no control was passed
            if (Ctrl == null)
                throw new ArgumentNullException("Ctrl");

            // Set the associated edit control
            _EditCtrl = Ctrl;

            // By default it should be a multiline control that accepts the tab character
            Ctrl.AcceptsTab = true;
            Ctrl.Multiline = true;

            // Make sure popup list box isn't set as visible
            if (_PopupListBox != null)
            {
                _PopupListBox.Visible = false;
            }

            // Make sure the dropdown button isn't set as visible, and give it a link to the ReinoTextBox
            if (fDropDownButton != null)
            {
                fDropDownButton.Visible = false;
                // DEBUG
                /*fDropDownButton.Click += new System.EventHandler(TextBoxBehavior.HandleDropDownClick);* /
                fDropDownButton.EditCtrl = this._EditCtrl;
            }
        }
        */
        /*
        public void AssociateWithGenericCtrl(Control Ctrl)
        {
        /* KLC
            // Somebody's doing something wrong if no control was passed
            if (Ctrl == null)
                throw new ArgumentNullException("Ctrl");

            // Set the associated edit control
            _EditCtrl = Ctrl;

            // By default it should be a multiline control that accepts the tab character
            if (_EditCtrl is ReinoControls.ReinoTextBox)
            {
                ((ReinoControls.ReinoTextBox)(_EditCtrl)).AcceptsTab = true;
                ((ReinoControls.ReinoTextBox)(_EditCtrl)).Multiline = true;
            }

            // Make sure popup list box isn't set as visible
            if (_PopupListBox != null)
            {
                _PopupListBox.Visible = false;
            }

            // Make sure the dropdown button isn't set as visible, and give it a link to the ReinoTextBox
            if (fDropDownButton != null)
            {
                fDropDownButton.Visible = false;
                // DEBUG
                /*fDropDownButton.Click += new System.EventHandler(TextBoxBehavior.HandleDropDownClick);* /
                fDropDownButton.EditCtrl = this._EditCtrl;
            }
         * / 
        }
         */
        /*
        public void AssociateWithListBox(Control Ctrl)
        {
            // Somebody's doing something wrong if no control was passed
            if (Ctrl == null)
                throw new ArgumentNullException("Ctrl");

            // Set the associated edit control
            _EditCtrl = Ctrl;
        }
        */
        /// <summary>
        /// Disposes of the object by dettaching the event handlers from their corresponding virtual 
        /// methods of the Behavior class and setting the edit control to null. 
        /// </summary>
        public virtual void Dispose()
        {
            _EditCtrl = null;
        }
        #endregion

        #region EventHandlers
        /// <summary>
        /// Attaches several event handlers to the associated edit control. 
        /// </summary>
        /// 
        /* KLC
        public static void AddEventHandlers(ReinoTextBox pTextBox)
        {
            try
            {
                // Add keyboard and focus events common on both Win32 and WinCE
                pTextBox.KeyDown += new KeyEventHandler(TextBoxBehavior.HandleKeyDown);
                pTextBox.KeyPress += new KeyPressEventHandler(TextBoxBehavior.HandleKeyPress);
                pTextBox.GotFocus += new EventHandler(TextBoxBehavior.HandleGotFocus);
                pTextBox.Validating += new CancelEventHandler(TextBoxBehavior.HandleValidating);

                // Mouse events aren't available in the Compact Framework, so we have to 
                // use the message filters in ReinoApplicationEx as a work-around.
                // We also use message filtering on the desktop for the PreMouseDown event
                // which will occur prior to Windows processing the event.

                /* KLC
                // Initialize the message filters if necessary
                if ((ReinoMessageFilters.PopupListBoxFilter == null) ||
                    (ReinoMessageFilters.StaticListBoxFilter == null) ||
                    (ReinoMessageFilters.TextBoxMouseEventFilter == null) ||
                    (ReinoMessageFilters.PopupBalloonMsgEventFilter == null)
#if WindowsCE || __ANDROID__
 || (ReinoMessageFilters.RadioListEventFilter == null)
                    || (ReinoMessageFilters.BasicButtonEventFilter == null)
                    || (ReinoMessageFilters.VirtualListBoxEventFilter == null)
#endif
)
                    ReinoMessageFilters.Init();
                    * /
                // The PreMouseDown will prevent Windows from processing the mouse click if we are just getting focus.
                // It is used in both Win32 and WinCE, so we can attach event to the filter directly.
                // DEBUG -- The message filter doesn't need to loop through an invocation list, so only add the event handler once
                // (Essentially its one event handler for the entire class rather than each instance of the class)
       
                if (ReinoMessageFilters.TextBoxMouseEventFilter.PreMouseDownExists == false)
                    ReinoMessageFilters.TextBoxMouseEventFilter.PreMouseDown += new MouseEventHandler(TextBoxBehavior.HandlePreMouseDown);
               
                // Get platform dependant object we will use for calling a Windows API function
                IPlatformDependent WinAPI = GetWinAPI();
                // Now we need to add standard mouse events for the ReinoTextBox.
                // On the Compact Framework, this will add the event handler as a message filter,
                // but on the FULL .NET Framework, it will simply add the event to the control.
                WinAPI.AddMouseClickEvent(new MouseEventHandler(TextBoxBehavior.HandleMouseClick), pTextBox);
                WinAPI.AddMouseMoveEvent(new MouseEventHandler(TextBoxBehavior.HandleMouseMove), pTextBox);
                WinAPI.AddDoubleClickEvent(new EventHandler(TextBoxBehavior.HandleDoubleClick), pTextBox);
            }
            catch (Exception Ex)
            {
                Debug.WriteLine("ReinoTextBoxBehavior AddEventHandlers Error:\r\n" + Ex.Message);
                Debug.WriteLine("Type: " + Ex.GetType().ToString());
                if (Ex.InnerException != null)
                    Debug.WriteLine("Inner: " + Ex.InnerException.Message);
                Debug.WriteLine("StackTrace: " + Ex.StackTrace);
                Debug.WriteLine("");
                if (pTextBox != null)
                {
                    Debug.WriteLine("Args ctrl: Name=" + pTextBox.Name + " Handle=" + pTextBox.Handle.ToString());
                }
            }
        }
        */
        /// <summary>
        /// Dettaches several event handlers from the associated edit control.
        /// </summary>
        /// 
        /* KLC
        public static void RemoveEventHandlers(ReinoTextBox pTextBox)
        {
            if (pTextBox == null)
                return;

            // Remove keyboard and focus events common on both Win32 and WinCE
            pTextBox.KeyDown -= new KeyEventHandler(TextBoxBehavior.HandleKeyDown);
            pTextBox.KeyPress -= new KeyPressEventHandler(TextBoxBehavior.HandleKeyPress);
            pTextBox.GotFocus -= new EventHandler(TextBoxBehavior.HandleGotFocus);
            pTextBox.Validating -= new CancelEventHandler(TextBoxBehavior.HandleValidating);

            // Mouse events aren't available in the Compact Framework, so we have to 
            // use the message filters in ReinoApplicationEx as a work-around.
            // We also use message filtering on the desktop for the PreMouseDown event
            // which will occur prior to Windows processing the event.

            // The PreMouseDown will prevent Windows from processing the mouse click if we are just getting focus.
            // It is used in both Win32 and WinCE, so we can dettach event from the filter directly.
            // DEBUG: Our message filter is not using an invocation list. We have one event handler for the 
            // class as a whole, so DON'T dettach it when destroying a control!
            /*ReinoMessageFilters.TextBoxMouseEventFilter.PreMouseDown -= new MouseEventHandler(HandlePreMouseDown);* /

            // Get platform dependant object we will use for calling a Windows API function
            IPlatformDependent WinAPI = GetWinAPI();
            // Now we need to remove standard mouse events from the ReinoTextBox.
            // On the Compact Framework, this will remove the event handler from the message filter,
            // but on the FULL .NET Framework, it will simply remove the event from the control.
            WinAPI.RemoveMouseClickEvent(new MouseEventHandler(TextBoxBehavior.HandleMouseClick), pTextBox);
            WinAPI.RemoveMouseMoveEvent(new MouseEventHandler(TextBoxBehavior.HandleMouseMove), pTextBox);
            WinAPI.RemoveDoubleClickEvent(new EventHandler(TextBoxBehavior.HandleDoubleClick), pTextBox);

            // Also remove event from dropdown button
            if ((pTextBox.Behavior != null) && (pTextBox.Behavior.fDropDownButton != null))
            {
                pTextBox.Behavior.fDropDownButton.Click -= new System.EventHandler(TextBoxBehavior.HandleDropDownClick);
            }
        }
        */
        /// <summary>
        /// Handles keyboard presses of control characters inside the edit control. 
        /// </summary>
        /* KLC
        public static void HandleKeyDown(object sender, KeyEventArgs e)
        {
            // Make sure we work with the sender object
            if (sender is ReinoControls.ReinoTextBox == false)
                return;
            ReinoControls.ReinoTextBox loEditCtrl = (sender as ReinoControls.ReinoTextBox);
            TextBoxBehavior loBehavior = loEditCtrl.Behavior;

            // Is there an application-assigned event for dealing with navigation keys?
            if (loBehavior.OnNavigationKeyPress != null)
            {
                // Translate the key to known navigation keys
                char TranslatedNavKey = ReinoControls.KeyEventHelper.TranslateToReinoNavChar(e);
                // If we have a translated navigation key, call the application-assigned event
                if (TranslatedNavKey != (char)0)
                {
                    KeyPressEventArgs EventArgs = new KeyPressEventArgs(TranslatedNavKey);
                    loBehavior.OnNavigationKeyPress(EventArgs);
                    // If application assigned event handled the keystroke, 
                    // then there's nothing left for us to do.
                    if (EventArgs.Handled == true)
                    {
                        // Set event as handled so nothing else happens
                        e.Handled = true;
                        return;
                    }
                }
            }

#if !WindowsCE && !__ANDROID__  
          // Check for special functions
            if (e.Modifiers == Keys.Control)
            {
                // Support for clipboard keys (cut/copy/paste)
                if (e.KeyCode == Keys.V)
                {
                    loEditCtrl.Paste();
                    loBehavior.SetEditBufferAndPaint(loEditCtrl.Text, true);
                    /* KLC
                    // Now set the real text to match the final edit buffer after being merged with edit mask
                    loBehavior.SetText(loBehavior._EditBuffer, ReinoControls.WinControl_SetText_Options.sto_ReplaceText, 0, true);
                    if (loBehavior._EditCtrl is ReinoTextBox)
                    {
                        // Finally, update the associated control's text
                        ((ReinoTextBox)(loBehavior._EditCtrl)).BaseText = loBehavior._EditBuffer;
                    }
                    * /
                    // Set event as handled so nothing else happens
                    e.Handled = true;
                    return;
                }
                if (e.KeyCode == Keys.C)
                {
                    loEditCtrl.Copy();
                    // Set event as handled so nothing else happens
                    e.Handled = true;
                    return;
                }
                if (e.KeyCode == Keys.X)
                {
                    loEditCtrl.Cut();
                    loBehavior.SetEditBufferAndPaint(loEditCtrl.Text, true);
                    // Set event as handled so nothing else happens
                    e.Handled = true;
                    return;
                }
            }

            /*
            if (e.Shift)
            {
                // If SHIFT is used with arrows, let's allow normal control activity to happen
                // to control text selection
                if ((e.KeyCode == Keys.Right) || (e.KeyCode == Keys.Left))
                {
                    e.Handled = false;
                    return;
                }
            }
            * /

            // Check for ALT+Down
            if ((e.Modifiers == Keys.Alt) && (e.KeyCode == Keys.Down))
            {
                // Translate to an enter key since that is what we use to invoke dropdown list
                loBehavior.ProcessKeyStroke(ReinoKeys.chLF);
                // Set event as handled so nothing else happens
                e.Handled = true;
                return;
            }
#endif
            // Check for and translate non-character keys that are not captured by KeyPress event
            char TranslatedKey = (char)0;
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    TranslatedKey = ReinoKeys.chDEL;
                    break;
                case Keys.Left:
                    TranslatedKey = ReinoKeys.chLFARROW;
                    /*
                    // SHIFT+LEFT is handled elsewhere, so when we get here, cursor should move and there should be no selected text
                    loBehavior.SetTextSelection(loEditCtrl.SelectionStart, loEditCtrl.SelectionStart);
                    * /
                    break;
                case Keys.Up:
                    TranslatedKey = ReinoKeys.chUPARROW;
                    break;
                case Keys.Right:
                    TranslatedKey = ReinoKeys.chRTARROW;
                    /*
                    // SHIFT+RIGHT is handled elsewhere, so when we get here, cursor should move and there should be no selected text
                    loBehavior.SetTextSelection(loEditCtrl.SelectionStart, loEditCtrl.SelectionStart);
                    * /
                    break;
                case Keys.Down:
                    TranslatedKey = ReinoKeys.chDNARROW;
                    break;
                case Keys.Return:
                    TranslatedKey = ReinoKeys.chLF;
                    break;
                case Keys.Prior:
                    TranslatedKey = ReinoKeys.chPGUP;
                    break;
                case Keys.Next:
                    TranslatedKey = ReinoKeys.chPGDN;
                    break;
                case Keys.End:
                    TranslatedKey = ReinoKeys.chEND;
                    break;
                case Keys.Home:
                    TranslatedKey = ReinoKeys.chHOME;
                    break;
                case Keys.Tab:
                    if (e.Shift == true)
                        TranslatedKey = ReinoKeys.chPREV;
                    else
                        TranslatedKey = ReinoKeys.chNEXT;
                    break;
                case Keys.F12:
                    TranslatedKey = ReinoKeys.chCLEAR;
                    break;
                case Keys.F1:
                    TranslatedKey = ReinoKeys.chHELP;
                    break;
            }

            // All key-handling is done in ProcessKeyStroke
            if (TranslatedKey != (char)0)
            {
                loBehavior.ProcessKeyStroke(TranslatedKey);
                // Set event as handled so nothing else happens
                e.Handled = true;
            }
        }
         */

        /// <summary>
        /// Handles keyboard presses of regular characters inside the edit control. 
        /// </summary>
        /* KLC
        public static void HandleKeyPress(object sender, KeyPressEventArgs e)
        {
            // Make sure we work with the sender object
            if (sender is ReinoControls.ReinoTextBox == false)
                return;
            ReinoControls.ReinoTextBox loEditCtrl = (sender as ReinoControls.ReinoTextBox);
            TextBoxBehavior loBehavior = loEditCtrl.Behavior;

            char TranslatedKey = e.KeyChar;

            // Tab key should have already been dealt with in HandleKeyDown?
            if (TranslatedKey == '\t')
            {
                e.Handled = true;
                return;
            }

            // All key-handling is done in ProcessKeyStroke
            loBehavior.ProcessKeyStroke(TranslatedKey);
            // Set event as handled so nothing else happens
            e.Handled = true;
        }
         */ 

        /// <summary>
        /// HandlePreMouseDown is called by the application's message filtering loop is 
        /// used for setting initial focus to a ReinoTextBox and preventing the mouse click event 
        /// from being processed so the cursor is not moved.
        /// </summary>
        /* KLC
        public static void HandlePreMouseDown(object sender, MouseEventArgs e)
        {
            // IMPORTANT! Make sure we use the "sender" object instead of "this"
            // because we might be called via the message loop inside a different behavior object!!!
            // We can't do anything if the sender is not a textbox!
            if (sender is ReinoControls.ReinoTextBox == false)
                return;
            ReinoControls.ReinoTextBox loEditCtrl = ((ReinoControls.ReinoTextBox)(sender));
            TextBoxBehavior loBehavior = loEditCtrl.Behavior;

            // DEBUG -- whats this?
            /*
            // Also exit if we're not the correct Behavior object target for this operation
            if (this.EditCtrl != loEditCtrl)
                return;
            * /

            // Assume the mouse click will not be fully handled here
            ReinoMessageFilters.TextBoxMouseEventFilter.MouseClickHandled = false;

            // We only care if the left mouse button is clicked
            if (e.Button == MouseButtons.Left)
            {
                // If control is not focus, shift focus to it now, and set 
                // flag indicating the mouse click event is fully processed
                // so Windows doesn't call our HandleMouseClick event which
                // would move the cursor position.
                if (!loEditCtrl.Focused)
                {
                    loEditCtrl.Focus();
                    ReinoMessageFilters.TextBoxMouseEventFilter.MouseClickHandled = true;
                }
            }
        }
            */

        /// <summary>
        /// Handles mouse clicks to update internal cursor position. 
        /// </summary>
        /* KLC
        public static void HandleMouseClick(object sender, MouseEventArgs e)
        {
            // IMPORTANT! Make sure we use the "sender" object instead of "this"
            // because we might be called via the message loop inside a different behavior object!!!
            // We can't do anything if the sender is not a textbox!
            if (sender is ReinoControls.ReinoTextBox == false)
                return;
            ReinoControls.ReinoTextBox loEditCtrl = ((ReinoControls.ReinoTextBox)(sender));
            TextBoxBehavior loBehavior = loEditCtrl.Behavior;

            // DEBUG -- whats this?
            /*
            // Also exit if we're not the correct Behavior object target for this operation
            if (this.EditCtrl != loEditCtrl)
                return;
            * /

            // This occurs after the left mouse button was clicked
            // We need to make the internal cursor position match the true cursor position
            loEditCtrl.Invalidate();
            loBehavior._CursorCharPos = loEditCtrl.SelectionStart;
        }
         */ 

        /// <summary>
        /// Handles mouse movements to update internal text selection variables
        /// </summary>
        /* KLC
        public static void HandleMouseMove(object sender, MouseEventArgs e)
        {
            // IMPORTANT! Make sure we use the "sender" object instead of "this"
            // because we might be called via the message loop inside a different behavior object!!!
            // We can't do anything if the sender is not a textbox!
            if (sender is ReinoControls.ReinoTextBox == false)
                return;

            try
            {
                ReinoControls.ReinoTextBox loEditCtrl = ((ReinoControls.ReinoTextBox)(sender));
                TextBoxBehavior loBehavior = loEditCtrl.Behavior;

                // If the left mouse button is pressed, we need to keep track of the text selection
                if (e.Button == MouseButtons.Left)
                {
                    loBehavior._TextSelectionStart = loEditCtrl.SelectionStart;
                    if (loEditCtrl.SelectionLength == 0)
                        loBehavior._TextSelectionEnd = 0;
                    else
                        loBehavior._TextSelectionEnd = loEditCtrl.SelectionStart + loEditCtrl.SelectionLength - 1;
                }
            }
            catch { }
        }
         */ 

        /// <summary>
        /// Handles behavior when edit control receives double click from the left mouse button
        /// </summary>
        public static void HandleDoubleClick(object sender, EventArgs e)
        {
        /* KLC
            // IMPORTANT! Make sure we use the "sender" object instead of "this"
            // because we might be called via the message loop inside a different behavior object!!!
            // We can't do anything if the sender is not a textbox!
            if (sender is ReinoControls.ReinoTextBox == false)
                return;
            ReinoControls.ReinoTextBox loEditCtrl = ((ReinoControls.ReinoTextBox)(sender));
            TextBoxBehavior loBehavior = loEditCtrl.Behavior;

            if ((loBehavior._ListSourceTable != null) && (loBehavior._ListSourceTable.GetRecCount() > 0))
            {
                loBehavior.DoDropDown();
            }
         */ 
        }

        /// <summary>
        /// Handles behavior when edit control is about to lose focus
        /// </summary>
        public static void HandleValidating(object sender, CancelEventArgs e)
        {
        /* KLC
            // IMPORTANT! Make sure we use the "sender" object instead of "this"
            // because we might be called via the message loop inside a different behavior object!!!
            // We can't do anything if the sender is not a textbox!

            if (sender is ReinoControls.ReinoTextBox == false)
                return;
            ReinoControls.ReinoTextBox loEditCtrl = ((ReinoControls.ReinoTextBox)(sender));
            TextBoxBehavior loBehavior = loEditCtrl.Behavior;

            // Exit if validation is currently disabled
            if (loBehavior.ValidationDisabled == true)
                return;

            // Are we flagged to skip this validation attempt?
            if (loBehavior.SkipNextValidation == true)
            {
                // Reset the flag so next validation does occur
                loBehavior.SkipNextValidation = false;
                return;
            }
            else
            {
                // Reset the flag so next validation does occur
                loBehavior.SkipNextValidation = false;
            }

            // Assume we must be valid to exit
            bool loMustBeValid = true;

            // Is there an event that will allow us to get the focused control?
            if (loBehavior.OnGetFocusedControl != null)
            {
                // Try to get the focused control
                Control FocusedCtrl = loBehavior.OnGetFocusedControl();

                // Is there an event we can use to determine if the focused control appears prior to us?
                if ((loBehavior.OnWhichControlIsFirst != null) && (FocusedCtrl != null))
                {
                    Control FirstCtrl = loBehavior.OnWhichControlIsFirst(FocusedCtrl, loEditCtrl);
                    // If we are moving focus to a previous control, then its okay to skip validation
                    if ((FirstCtrl != null) && (FirstCtrl == FocusedCtrl))
                        loMustBeValid = false;
                }
            }

            if (loBehavior.OkToExit(loMustBeValid) == false)
            {
                e.Cancel = true;
            }
         */
        }

        /// <summary>
        /// Handles behavior when edit control receives focus either from keyboard or mouse
        /// </summary>
        public static void HandleGotFocus(object sender, EventArgs e)
        {
        /*
            // IMPORTANT! Make sure we use the "sender" object instead of "this"
            // because we might be called via the message loop inside a different behavior object!!!

            // Resolve the associated behavior object
            TextBoxBehavior loBehavior = null;
            if (sender is ReinoControls.ReinoTextBox)
                loBehavior = (sender as ReinoControls.ReinoTextBox).Behavior;
            else if (sender is ReinoControls.ReinoVirtualListBox)
                loBehavior = (sender as ReinoControls.ReinoVirtualListBox).Behavior;

            // Can't do this if we don't have a behavior object to work with
            if (loBehavior == null)
                return;

            // Is there an extra app-assigned event to perform?
            if (loBehavior.OnCtrlGotFocus != null)
                loBehavior.OnCtrlGotFocus(sender);

            // Use the OnGetFormEditMode event if we have one, 
            // otherwise just assume the default "femNewEntry" mode
            int loFormEditMode;
            if (loBehavior.OnGetFormEditMode != null)
                loFormEditMode = loBehavior.OnGetFormEditMode();
            else
                loFormEditMode = EditRestrictionConsts.femNewEntry;

            // If we are in View only mode, we don't need to do anything
            if (loFormEditMode == EditRestrictionConsts.femView)
                return;

            // Use the OnGetFormEditAttrs event if we have one, 
            // otherwise just assume the default mode of zero (No attributes set)
            int loFormEditAttrs;
            if (loBehavior.OnGetFormEditAttrs != null)
                loFormEditAttrs = loBehavior.OnGetFormEditAttrs();
            else
                loFormEditAttrs = 0;

            // Skip first edit focus notification if we have already edited the 1st field
            if ((loFormEditAttrs & EditRestrictionConsts.feaEditedFirstField) == 0)
            {
                TTControl loNextControl = null;
                loFormEditAttrs |= EditRestrictionConsts.feaEditedFirstField;

                // Need to update form with the new edit attributes
                if (loBehavior.OnSetFormEditAttr != null)
                    loBehavior.OnSetFormEditAttr(EditRestrictionConsts.feaEditedFirstField, true);

                if (loBehavior.OnFindNextFormControl != null)
                {
                    // Let our children do stuff they care about. Edit Restictions might want to
                    // rebuild the display, but let's make it only happen once
                    ReinoControls.TextBoxBehavior.SkipRebuildDisplay = true;
                    ReinoControls.TextBoxBehavior.RebuildDisplayWasSkipped = false;
                    for (loNextControl = loBehavior._CfgCtrl; loNextControl != null; loNextControl = loBehavior.OnFindNextFormControl(loNextControl, false))
                    {
                        if (loNextControl == null) continue;
                        if (loNextControl.TypeOfWinCtrl() != typeof(ReinoTextBox)) continue;
                        if (loNextControl.Behavior.GetEditStatePreInitialized()) continue;
                        loNextControl.Behavior.DependentNotification(EditRestrictionConsts.dneFirstEditFocus, null);
                    }
                    // Now if the flag was set, we need to do the rebuild
                    if (RebuildDisplayWasSkipped == true)
                    {
                        try
                        {
                            // Turn off flags, then force rebuild
                            ReinoControls.TextBoxBehavior.SkipRebuildDisplay = false;
                            ReinoControls.TextBoxBehavior.RebuildDisplayWasSkipped = false;
                            loBehavior.OnRestrictionForcesDisplayRebuild();
                        }
                        catch { }
                    }
                    else
                    {
                        // Turn off flags for skipping Rebuild procedure
                        ReinoControls.TextBoxBehavior.SkipRebuildDisplay = false;
                        ReinoControls.TextBoxBehavior.RebuildDisplayWasSkipped = false;
                    }
                }
            }

            // save contents of field.
            loBehavior._SavedEditBuffer = loBehavior._EditBuffer;
            // Is it a numeric field which is treated differently?
            if (loBehavior._FieldType == TEditFieldType.efNumeric)
            {
                // put the cursor at the end so entire field isn't automatically highlighted
                loBehavior.MoveCursor(loBehavior._EditBuffer.Length);
            }
            else
            {
                // put the cursor at the beginning so entire field isn't automatically highlighted
                int loNewCursorPos = loBehavior.GetNextAvailCursorPos(-1);
                loBehavior.MoveCursor(loNewCursorPos);
            }

            // Empty the message queue so mouse event isn't triggered and moves to a different caret position
            // (This doesn't seem to do exactly what we expect on WindowsCE, so for that platform we also
            // have the PreMouseDown event.)
            loBehavior.EmptyMsgQueue();
         */ 
        }

        public static void HandleDropDownClick(object sender, EventArgs e)
        {
        /*
            // IMPORTANT! Make sure we use the "sender" object instead of "this"
            // because we might be called via the message loop inside a different behavior object!!!
            // We can't do anything if the sender is not a textbox!
            if (sender is ReinoDropDownButton == false)
                return;
            ReinoDropDownButton loDropBtn = (sender as ReinoDropDownButton);
            ReinoControls.ReinoTextBox loEditCtrl = (loDropBtn.EditCtrl as ReinoControls.ReinoTextBox);
            TextBoxBehavior loBehavior = loEditCtrl.Behavior;

            if ((loBehavior._ListSourceTable != null) && (loBehavior._ListSourceTable.GetRecCount() > 0))
            {
                if ((loBehavior.fDropDownButton.Dropped == false) ||
                    (loBehavior._PopupListBox == null) || ((loBehavior._PopupListBox != null) &&
                    (loBehavior._PopupListBox.Visible == false)))
                {
                    // Set focus to the button first, otherwise we might see the screen "jump" 
                    // to a new location when we're done with the drop down. You would think
                    // a click event should set focus automatically, but I guess it doesn't!
                    if (sender is ReinoDropDownButton)
                    {
                        ((ReinoDropDownButton)(sender)).Focus();
                        // If focus did not move as expected, then the previous control's
                        // validation must have failed. Since this is the case, we will
                        // abort and NOT do the dropdown event
                        if (((ReinoDropDownButton)(sender)).Focused == false)
                            return;
                    }
                    loBehavior.DoDropDown();
                }
            }
         */ 
        }
        #endregion

        #region Keyboard Functions
        /// <summary>
        /// Returns 0 if the keystroke was handled, the key value if it was not handled.
        /// </summary>
        private int ProcessKeyStroke(char iKey)
        {
            /* KLC
                // First of all, any keystroke other than NEXT/PREV will cancel visible popup balloon windows,
                // so if any popupBalloon is visible, close it.
                if ((iKey != ReinoKeys.chNEXT) && (iKey != ReinoKeys.chPREV))
                    ClosePopupBalloons();

                int loNewCursorPos = _CursorCharPos;
                int loSavedListNdx = _ListNdx;
                bool loDroppedChar = false;
                int loSavedTextSelectionLen = GetTextSelectedLength();

                // save a copy of the field 's edit buffer and text buffer
                string loSavedEditBuf = _EditBuffer;
                string loSavedText = _Text;

                ReinoTextBox loReinoTextBox = (this.EditCtrl as ReinoTextBox);

                // check for any control characters. Within an edit field, 
                // we only recognize Home, End, Backspace, and left/right arrow keys 

                // transform space into right arrow for date or time field
                if ((iKey == ' ') && ((_FieldType == TEditFieldType.efDate) || (_FieldType == TEditFieldType.efTime)))
                    iKey = ReinoKeys.chRTARROW;

                // Is it a non-printable character?
                if ((iKey < 0x20) || (iKey > 0x7E))
                {
                    switch (iKey)
                    {
                        case ReinoKeys.chLF:
                            // [ENTER] key only has a meaning when there is a list
                            if ((loReinoTextBox != null) && (loReinoTextBox.ReadOnly == false))
                            {
                                if ((_ListSourceTable != null) && (_ListSourceTable.GetRecCount() > 0))
                                {
                                /* KLC
                                    // If the popup listbox exists and is showing, close it up	
                                    if ((_PopupListBox != null) && (_PopupListBox.Visible == true))
                                    {
                                        _PopupListBox.Visible = false;
                                        // Make sure the listbox isn't capturing the mouse events
                                        _PopupListBox.Capture = false;
                                        // Return keyboard focus to the edit control
                                        loReinoTextBox.Focus();
                                    }
                                    else
                                    {
                                        // Only do the drop down if there are items in the list
                                        if ((_ListSourceTable != null) && (_ListSourceTable.GetRecCount() > 0))
                                            DoDropDown();
                                    }
                                 * / 
                                }
                            }
                            return iKey;
                        //break;
                        case ReinoKeys.chBS:
                            // JLA 2009.07.21 - Manual entry support (Begin)
                            int currBufferLength = _EditBuffer.Length;
                            loNewCursorPos = GetPrevAvailCursorPos(_CursorCharPos);

                            // Is there highlighted text that should be deleted?
                            if (GetTextSelectedLength() > 0)
                            {
                                // delete the highlighted text and get the new cursor pos
                                loNewCursorPos = DeleteSelectedText();
                                // and reset the seletion
                                SetTextSelection(0, 0);
                            }
                            else
                            {
                                // If a list item is selected, allow the delete key to at least move the cursor back (similar to backspace)
                                if (((_ListNdx >= 0) || (loNewCursorPos == EditBuffer.Length)) && (loNewCursorPos > 0))
                                    loNewCursorPos--;
                                if ((loReinoTextBox != null) && (loReinoTextBox.ReadOnly == false))
                                    DeleteCharFromEditBuf(loNewCursorPos, true);
                            }

                            // JLA 2009.07.21 - Manual entry support (Begin)
    #if !WindowsCE && !__ANDROID__  
              // Specialized cursor handling for numeric fields
                            if (_FieldType == TEditFieldType.efNumeric)
                            {
                                if (currBufferLength == _EditBuffer.Length)
                                {
                                    loNewCursorPos = _CursorCharPos;
                                }
                            }
    #endif
                            break;
                        // JLA 2009.07.21 - Manual entry support (End)
                        case ReinoKeys.chDEL:
                            // Is there highlighted text that should be deleted?
                            if (GetTextSelectedLength() > 0)
                            {
                                // delete the highlighted text and get the new cursor pos
                                loNewCursorPos = DeleteSelectedText();
                                // and reset the seletion
                                SetTextSelection(0, 0);
                            }
                            else
                            {
                                // If a list item is selected, allow the delete key to at least move the cursor back (similar to backspace)
                                if (((_ListNdx >= 0) || (loNewCursorPos == EditBuffer.Length)) && (loNewCursorPos > 0))
                                    loNewCursorPos--;
                                if ((loReinoTextBox != null) && (loReinoTextBox.ReadOnly == false))
                                    DeleteCharFromEditBuf(loNewCursorPos, true);
                            }
                            break;
                        case ReinoKeys.chCLEAR:
                            // is there any text selected?
                            if ((loReinoTextBox != null) && (loReinoTextBox.ReadOnly == false))
                            {
                                if (GetTextSelectedLength() > 0)
                                {
                                    // delete the highlighted text and get the new cursor pos
                                    loNewCursorPos = DeleteSelectedText();
                                    // and reset the seletion
                                    SetTextSelection(0, 0);
                                }
                                else
                                {
                                    // nothing highlighted, so wipe it all out
                                    _ListNdx = -1;
                                    _ListNdxNotTrusted = false;
                                    // Update the listbox's selected index 
                                    UpdateListBoxIndex(_ListNdx);
                                    SetEditBuffer("");
                                    if ((loNewCursorPos = GetNextAvailCursorPos(-1)) < 0)
                                        loNewCursorPos = _CursorCharPos;
                                }
                            }
                            break;
                        case ReinoKeys.chHOME:
                            if ((loNewCursorPos = GetNextAvailCursorPos(-1)) < 0)
                                loNewCursorPos = _CursorCharPos;
                            break;
                        case ReinoKeys.chEND:
                            if ((loNewCursorPos = GetPrevAvailCursorPos(-1)) > EditBuffer.Length)
                                loNewCursorPos = _CursorCharPos;
                            break;
                        case ReinoKeys.chLFARROW:
                            loNewCursorPos = GetPrevAvailCursorPos(_CursorCharPos);
                            break;
                        case ReinoKeys.chRTARROW:
                            loNewCursorPos = GetNextAvailCursorPos(_CursorCharPos);
                            break;
                        case ReinoKeys.chDNARROW: // get next list item if there is one 
                            if ((loReinoTextBox != null) && (loReinoTextBox.ReadOnly == false))
                            {
                                if ((_ListSourceTable != null) && (_ListSourceTable.GetRecCount() > 0) &&
                                    (AnticipationDisabled == false))
                                {
                                    _ListNdx++;
                                    if (_ListNdx >= _ListSourceTable.GetRecCount())
                                    {
                                        _ListNdx = 0;
                                        _ListNdxNotTrusted = false;
                                    }
                                    _EditBuffer = GetListItem(_ListNdx);
                                    // Update the listbox's selected index 
                                    UpdateListBoxIndex(_ListNdx);
                                    break;
                                }
                                // a date or time field w/o an associated list will increment the m/d/y depending on the cursor location
                                IncrementDateTimeComponentAtCursorPos(-1);
                            }
                            break;
                        case ReinoKeys.chPGDN: // Scroll down one page worth of list items 
                            break;
                        case ReinoKeys.chUPARROW: // get prev list item if there is one 
                            if ((loReinoTextBox != null) && (loReinoTextBox.ReadOnly == false))
                            {
                                if ((_ListSourceTable != null) && (_ListSourceTable.GetRecCount() > 0) &&
                                    (AnticipationDisabled == false))
                                {
                                    _ListNdx--;
                                    if (_ListNdx < 0)
                                    {
                                        _ListNdx = _ListSourceTable.GetRecCount() - 1;
                                        _ListNdxNotTrusted = false;
                                    }
                                    _EditBuffer = GetListItem(_ListNdx);
                                    // Update the listbox's selected index 
                                    UpdateListBoxIndex(_ListNdx);
                                    break;
                                }
                                IncrementDateTimeComponentAtCursorPos(1);
                            }
                            break;
                        case ReinoKeys.chPGUP: // Scroll UP one page worth of list items 
                            break;
                        case ReinoKeys.chNEXT:
                            // First, see if there is a "OnTabForward" event which takes precedence
                            if (OnTabForward != null)
                            {
                                OnTabForward();
                                break;
                            }
                            else
                            {
                                if (loReinoTextBox != null)
                                {
                                    if (loReinoTextBox.Parent != null)
                                        loReinoTextBox.Parent.SelectNextControl(loReinoTextBox, true, true, true, true);
                                }
                            }
                            break;
                        case ReinoKeys.chPREV:
                            // First, see if there is a "OnTabBackward" event which takes precedence
                            if (OnTabBackward != null)
                            {
                                OnTabBackward();
                                break;
                            }
                            else
                            {
                                if (loReinoTextBox != null)
                                {
                                    if (loReinoTextBox.Parent != null)
                                        loReinoTextBox.Parent.SelectNextControl(loReinoTextBox, false, true, true, true);
                                }
                            }
                            break;
                        default:
                            {
                                break;
                            }
                    }
                } // if a non-printable character 
                else
                {
                    // Is there an application-assigned event for dealing with regular keys?
                    if (OnRegularKeyPress != null)
                    {
                        KeyPressEventArgs EventArgs = new KeyPressEventArgs(iKey);
                        OnRegularKeyPress(EventArgs);
                        // If application assigned event handled the keystroke, 
                        // then there's nothing left for us to do.
                        if (EventArgs.Handled == true)
                            return iKey;
                    }

                    if ((loReinoTextBox != null) && (loReinoTextBox.ReadOnly == false))
                    {
                        // printable character, insert it a current cursor position 
                        // is there any text selected?
                        loNewCursorPos = _CursorCharPos;  // make sure this is set
                        if (GetTextSelectedLength() > 0)
                        {
                            // delete the highlighted text and get the new cursor pos
                            loNewCursorPos = DeleteSelectedText();
                            // and reset the selection
                            SetTextSelection(0, 0);
                        }

                        int currBufferLength = _EditBuffer.Length;
                        loDroppedChar = _EditBuffer.Length >= _MaxLength;
                        if (AddCharToEditBuf(iKey, loNewCursorPos) == 0)
                        {
                            RingBell(this);
                            return 0;
                        }
                        // JLA 2009.07.21 - Manual entry support (Begin)
    #if !WindowsCE
                        // Specialized cursor handling for numeric fields
                        if (_FieldType == TEditFieldType.efNumeric)
                        {
                            if (currBufferLength == 0)
                                loNewCursorPos = _MaxLength;
                            else if (currBufferLength == _EditBuffer.Length)
                                loNewCursorPos = loNewCursorPos - 1;
                        }
    #endif
                        // JLA 2009.07.21 - Manual entry support (End)

                        loNewCursorPos = GetNextAvailCursorPos(loNewCursorPos);
                    }
                }

                // truncate at max length
                if (_EditBuffer.Length > _MaxLength)
                    _EditBuffer = _EditBuffer.Remove(_MaxLength, _EditBuffer.Length - _MaxLength);

                // after all those gyrations, did the data change? 
                if ((_EditBuffer.Equals(loSavedEditBuf) == false) || (loSavedListNdx != _ListNdx))
                {
                    // the only way fListNdx would have changed is if the user scrolled the list, 
                    // in which case we don't need to anticipate
                    Reino.ClientConfig.TEditRestriction loRestriction;

                    if (loSavedListNdx == _ListNdx)
                    {
                        // Do the anticipation routine (A.K.A. Autocomplete)
                        AnticipateListItem(loNewCursorPos);

                        // truncate at max length
                        if (_EditBuffer.Length > _MaxLength)
                            _EditBuffer = _EditBuffer.Remove(_MaxLength, _EditBuffer.Length - _MaxLength);
                        // if have a list item, dropped char is false
                        if (_ListNdx >= 0)
                        {
                            loDroppedChar = false;
                        }

                        // if used to have a list item, but don't any more, dropped char is false
                        if ((loSavedListNdx >= 0) && (_ListNdx == -1))
                        {
                            loDroppedChar = false;
                        }
                    }

                    // Date & time fields can overwrite existing values
                    if ((_FieldType == TEditFieldType.efDate) || (_FieldType == TEditFieldType.efTime) || (_EditMaskLen > 0 && (_EditMaskLen == _MaxLength)))
                        loDroppedChar = false;
                        /* KLC
                    if (loDroppedChar || ((loRestriction = ProcessRestrictions(EditRestrictionConsts.dneDataChanged, null)) != null))
                    {
                        // whoops! Don't allow this. Restore to the original value. 
                        _EditBuffer = loSavedEditBuf;
                        // make sure the window text matches the edit buffer
                        SetText(_EditBuffer, WinControl_SetText_Options.sto_ReplaceText, 0, false);
                        // Trigger the TextChanged event if the text value changed
                        if (loSavedText != _Text)
                            this.OnTextChanged(EventArgs.Empty);

                        _ListNdx = loSavedListNdx;
                        _ListNdxNotTrusted = false;
                        if (_ListSourceTable != null)
                        {
                            // Update the listbox's selected index 
                            UpdateListBoxIndex(_ListNdx);
                        }
                        RingBell(this);
                        return 0;
                    }
                    * /
                    NotifyDependents(EditRestrictionConsts.dneParentDataChanged);

                    if (loNewCursorPos > EditBuffer.Length)
                    {
                        loNewCursorPos = EditBuffer.Length;
                        // we are moving the cursor because list item change, force the MoveCuror method
                        // to be called to re-align the text and cursor pos. Setting this to -1 is reqd since
                        // occasionally the EditBuffer.Length and fCursorCharPos are equal, and when they
                        // are the MoveCursor method below doesn't get called and the cursor pos isn't set properly
                        _CursorCharPos = -1;
                    }
                    SetText(_EditBuffer, WinControl_SetText_Options.sto_ReplaceText, 0, false);
                    // Trigger the TextChanged event if the text value changed
                    if (loSavedText != _Text)
                        this.OnTextChanged(EventArgs.Empty);
                    PaintEditCtrl();
                }
                else if (loNewCursorPos == _CursorCharPos)
                {
                    // Data didn't change, cursor didn't move, nothing happened!
                    if ((iKey != ReinoKeys.chNEXT) && (iKey != ReinoKeys.chPREV) &&
                        (iKey != ReinoKeys.chESC) && (iKey != ReinoKeys.chLF)
                        && (iKey != ReinoKeys.chCR))
                        RingBell(this);
                }

                if (loNewCursorPos != _CursorCharPos)
                {
                    MoveCursor(loNewCursorPos);
                    if (loSavedTextSelectionLen != 0)
                    {
                        SetTextSelection(0, 0);
                        PaintEditCtrl();
                    }
                }
                else
                {
                    MoveCursor(loNewCursorPos);
                    PaintEditCtrl();
                }
                */
            return 0;
        } //ProcessKeyStroke 

        private bool KeyIsForEditting(char iKey)
        {
            return (iKey != ReinoKeys.chNEXT) && (iKey != ReinoKeys.chPREV) && (iKey != ReinoKeys.chHOME) &&
                (iKey != ReinoKeys.chEND) && (iKey != ReinoKeys.chLFARROW) && (iKey != ReinoKeys.chRTARROW);
        }
        #endregion

        #region EditState Attribute Functions
        public void ClearEditStateAttrs()
        {
            _EditStateAttrs = 0;
        }

        public bool GetEditStatePreInitialized()
        {
            return (_EditStateAttrs & Reino.ClientConfig.EditRestrictionConsts.esaPreInitialized) != 0;
        }

        public void SetEditStatePreInitialized(bool iState)
        {
            if (iState)
                _EditStateAttrs |= EditRestrictionConsts.esaPreInitialized;
            else
                _EditStateAttrs &= ~EditRestrictionConsts.esaPreInitialized;
        }

        public bool GetEditStateEdited()
        {
            return (_EditStateAttrs & EditRestrictionConsts.esaEdited) != 0;
        }

        public void SetEditStateEdited(bool iState)
        {
            if (iState) _EditStateAttrs |= EditRestrictionConsts.esaEdited;
            else _EditStateAttrs &= ~EditRestrictionConsts.esaEdited;
        }

        public bool GetEditStatePrinted()
        {
            return (_EditStateAttrs & EditRestrictionConsts.esaPrinted) != 0;
        }

        public void SetEditStatePrinted(bool iState)
        {
            if (iState)
                _EditStateAttrs |= EditRestrictionConsts.esaPrinted;
            else
                _EditStateAttrs &= ~EditRestrictionConsts.esaPrinted;
        }
        #endregion

        #region FieldType / DataType
        public TEditFieldType GetFieldType()
        {
            return _FieldType;
        }

        public void SetFieldType(TEditFieldType iFieldType)
        {
            _FieldType = iFieldType;
            UpdateEditMaskAttributes();
            MergeEditBufferWMask();
        }
        #endregion

        #region Cursor Position Functions
        /// <summary>
        /// Returns the position of the next available position of the cursor depending on the 
        /// current data and edit mask.  To find the first position, call w/ iAfterPos = -1.
        /// </summary>
        private int GetNextAvailCursorPos(int iAfterPos)
        {
            int loCursorPos = iAfterPos + 1;
            int loDataLen = EditBuffer.Length;
            char loMaskChar;

            // JLA 2009.07.21 - Manual entry support (Begin)
#if WindowsCE || __ANDROID__
            // For X3 we will retain functional compatibility with legacy eC++ product
            // Numeric fields are treated differently
            if (_FieldType == TEditFieldType.efNumeric)
            {
                // put the cursor at either 1st editable character or at the end
                // iAfterPos = 1 would be caused by pressing the [HOME] key
                if (iAfterPos == -1)
                {
                    while (loCursorPos >= 0)
                    {
                        loMaskChar = ((loCursorPos < _EditBuffer.Length && loCursorPos >= 0) ? _EditBuffer[loCursorPos] : (char)0);
                        if (((loMaskChar >= '0') && (loMaskChar <= '9')) || (loMaskChar == '-') || (loMaskChar == (char)0))
                        {
                            return loCursorPos;
                        }
                        loCursorPos++;
                    }
                    return (_EditBuffer.Length);
                }
                else
                    return (_EditBuffer.Length);
            }
#endif
            // JLA 2009.07.21 - Manual entry support (End)

            // Find next position in mask that is editable
            while ((loCursorPos <= loDataLen) && (loCursorPos <= _MaxLength))
            {
                loMaskChar = GetMaskCharForPos(loCursorPos);
                if (!MaskCharIsLiteral(loMaskChar))
                {
                    return loCursorPos;
                }
                loCursorPos++;
            }
            // If we made it far, then the cursor position isn't changing
            return iAfterPos;
        }

        /// <summary>
        /// Returns the position of the previous available position of the cursor 
        /// depending on the current data and edit mask. 
        /// </summary>
        private int GetPrevAvailCursorPos(int iBeforePos)
        {
            int loCursorPos = iBeforePos - 1;
            char loMaskChar;
            if (iBeforePos < 0)
                loCursorPos = EditBuffer.Length;

            // JLA 2009.07.21 - Manual entry support (Begin)
#if WindowsCE || __ANDROID__
            // For X3 we will retain functional compatibility with legacy eC++ product
            // Numeric fields are treated differently
            if (_FieldType == TEditFieldType.efNumeric)
            {
                // if the position is -1 (which happens when the [END] key is pressed), then
                // simply go to the end. Otherwise find the previous position in mask that is editable
                if (iBeforePos == -1)
                    return EditBuffer.Length;
                while (loCursorPos >= 0)
                {
                    loMaskChar = ((loCursorPos < _EditBuffer.Length && loCursorPos >= 0) ? _EditBuffer[loCursorPos] : (char)0);
                    if (((loMaskChar >= '0') && (loMaskChar <= '9')) || (loMaskChar == '-') || (loMaskChar == (char)0))
                    {
                        return loCursorPos;
                    }
                    loCursorPos--;
                }
                return iBeforePos;
            }
#endif
            // JLA 2009.07.21 - Manual entry support (End)

            // Find previous position in mask that is editable
            while (loCursorPos >= 0)
            {
                loMaskChar = GetMaskCharForPos(loCursorPos);
                if (!MaskCharIsLiteral(loMaskChar))
                {
                    return loCursorPos;
                }
                loCursorPos--;
            }
            // If we made it far, then the cursor position isn't changing
            return iBeforePos;
        }

        private int MoveCursor(int iNewCursorCharPos)
        {
        /* KLC
            // Only applicable to a textbox control
            if (!(this._EditCtrl is ReinoControls.ReinoTextBox))
                return 0;

            ((ReinoControls.ReinoTextBox)(this._EditCtrl)).SelectionStart = iNewCursorCharPos;
            _CursorCharPos = iNewCursorCharPos;

            // When the cursor position changes we want it visible, so
            // scroll until its visible
            ((ReinoControls.ReinoTextBox)(this._EditCtrl)).ScrollToCaret();
         */ 
            return 0;
        }
        #endregion

        #region Selected Text Functions
        public int GetTextSelectedLength()
        {
            if (_TextSelectionEnd == 0)  // don't return false values
                return 0;
            else
                return 1 + (_TextSelectionEnd - _TextSelectionStart);
        }

        public int GetTextSelectedStart()
        {
            return _TextSelectionStart;
        }

        public int GetTextSelectedEnd()
        {
            return _TextSelectionEnd;
        }

        /// <summary>
        /// deletes the selected text from the field, returning the new cursor position
        /// used for the CLEAR key, and when overtyping
        /// </summary>
        public int DeleteSelectedText()
        {
            int loNewCursorPos = _CursorCharPos;
            int loTextSelectedLength, loTextSelectedStart;

            loTextSelectedLength = GetTextSelectedLength();

            // if nothing is selected, nothing gets deleted
            if (loTextSelectedLength == 0)
            {
                // nothing to delete, just git
                return loNewCursorPos;
            }

            // loop and delete only the selected text; this method respects literal characters
            // we must delete backwards, since each delete changes the pos of chars following the del
            loTextSelectedStart = GetTextSelectedStart();
            for (int loDelIdx = loTextSelectedLength - 1; loDelIdx >= 0; loDelIdx--)
            {
                DeleteCharFromEditBuf(loTextSelectedStart + loDelIdx, false);
                // we only move the cursor if the delete ais happening "behind" the cursor
                if (_CursorCharPos > loTextSelectedStart + loDelIdx)
                {
                    loNewCursorPos = GetPrevAvailCursorPos(loNewCursorPos);
                }
            }
            // Now that all selected characters have been deleted, merge with the mask
            MergeEditBufferWMask();
            // reset selected length
            _TextSelectionEnd = 0;
            /* KLC
            if (this._EditCtrl is ReinoControls.ReinoTextBox)
                ((ReinoControls.ReinoTextBox)(this._EditCtrl)).SelectionLength = 0;
             */ 
            return loNewCursorPos;
        }

        /// <summary>
        /// initialize the start and end of the selected text
        /// </summary>
        public void SetTextSelection(int pStart, int pStop)
        {
            // mark offsets Start..Stop as selected
            int loLen;
            if (pStart <= pStop)
            {
                loLen = _Text.Length;
                if (pStart > loLen) { pStart = loLen; }
                if (pStop > loLen) { pStop = loLen; }
                _TextSelectionStart = pStart;
                _TextSelectionEnd = pStop;
                /* KLC
                if (this._EditCtrl is ReinoControls.ReinoTextBox)
                    ((ReinoControls.ReinoTextBox)(this._EditCtrl)).SelectionLength = _TextSelectionEnd - _TextSelectionStart;
                 */ 
            }
        }
        #endregion

        #region List Related Functions
        private int AnticipateListItem(int iCursorCharPos)
        {
            int loPrevListNdx = _ListNdx;
            if ((_ListSourceTable != null) && (_ListSourceTable.GetRecCount() > 0) && (AnticipationDisabled == false))
            {
                if (iCursorCharPos == 0) // can't anticipate no data 
                {
                    _EditBuffer = "";
                    _ListNdx = -1;
                    _ListNdxNotTrusted = false;
                    // Update the listbox's selected index 
                    UpdateListBoxIndex(_ListNdx);
                    return 0;
                }

                // Lets use the table's filter search instead of simple loop so we 
                // can realize performance benefit of table index
                /*
                for (_ListNdx = 0; _ListNdx < _ListSourceTable.GetRecCount(); _ListNdx++)
                {
                    if (GetListItem(_ListNdx).IndexOf(_EditBuffer.Substring(0, iCursorCharPos)) == 0)
                    {
                        _EditBuffer = GetListItem(_ListNdx);
                        // Update the listbox's selected index 
                        UpdateListBoxIndex(_ListNdx);
                        return 0;
                    }
                }
                */
                _ListNdx = _ListSourceTable.FilterSearch(_ListSourceFieldName, SafeSubString(_EditBuffer, 0, iCursorCharPos), GetEditMask(), true);
                _ListNdxNotTrusted = false;
                if (_ListNdx >= 0)
                {
                    _ListSourceTable.GetFormattedFieldData(_ListSourceFieldName, GetEditMask(), ref _EditBuffer);
                    // Update the listbox's selected index 
                    UpdateListBoxIndex(_ListNdx);
                    return 0;
                }

                _ListNdx = -1;
                _ListNdxNotTrusted = false;
                // Update the listbox's selected index 
                UpdateListBoxIndex(_ListNdx);
                // Truncate the string if going from list item to non-list item
                if ((iCursorCharPos < _EditBuffer.Length) && (loPrevListNdx > -1))
                    _EditBuffer = _EditBuffer.Remove(iCursorCharPos, _EditBuffer.Length - iCursorCharPos);
                return 0;
            }
            return 0;
        }

        public string GetListItem(int Index)
        {
            if (_ListSourceFieldIdx != -1)
                return GetFieldDataFromTable(ListSourceTable, _ListSourceFieldIdx, Index, this.GetEditMask());
            else
                return GetFieldDataFromTable(ListSourceTable, ListSourceFieldName, Index, this.GetEditMask());
        }

        public string GetDisplayListItem(int Index)
        {
            // Do we need to get data for a grid-style display?
            if ((this._CfgCtrl != null) && (this._CfgCtrl is TEditField) &&
                ((this._CfgCtrl as TEditField).DBListGrid != null) &&
                ((this._CfgCtrl as TEditField).DBListGrid.Columns.Count > 0))
            {
                TTDBListBox loGrid = (this._CfgCtrl as TEditField).DBListGrid;
                // If there is no associated grid in the configuration, we can't do anything else
                if (loGrid == null)
                    return "";

                // Bring the table to the desired index
                this.ListSourceTable.ReadRecord(Index);

                string loResult = "";
                foreach (TGridColumnInfo Column in loGrid.Columns)
                {
                    string FieldName = Column._Name;
                    int loFldIdx = this.ListSourceTable.fTableDef.GetFldNo(FieldName);
                    if ((loFldIdx >= 0) && (loFldIdx < this.ListSourceTable.fTableDef.HighTableRevision.Fields.Count))
                    {
                        if (loResult != "")
                            loResult += "\t";
                        // Get field value from the table. Note that if the field is virtual, we
                        // must get it via GetFormattedFieldData instead of from fFieldValues
                        TTableFldDef loFld = this.ListSourceTable.fTableDef.GetField(loFldIdx);
                        if (loFld is TTableVirtualFldDef)
                            loResult += this.ListSourceTable.GetFormattedFieldData(loFldIdx, Column.Mask);
                        else
                            loResult += this.ListSourceTable.GetFormattedFieldData(loFldIdx, Column.Mask);// fFieldValues[loFldIdx];
                    }
                }
                return loResult;
            }
            else
            {
                return GetListItem(Index);
            }
        }

        public static int GetFieldIndexForTable(Reino.ClientConfig.TTTable pListTable, string pListField)
        {
            // Find field index
            TObjBasePredicate predicate = new TObjBasePredicate(pListField);
            return pListTable.fTableDef.HighTableRevision.Fields.FindIndex(predicate.CompareByName_CaseInsensitive);
        }

        public static string GetFieldDataFromTable(Reino.ClientConfig.TTTable pListTable, string pListField, int pRowIdx, string DestMask)
        {
            // Just return empty string if table, fieldname or row index is bad
            if (pListTable == null) return "";

            // Find field index
            TObjBasePredicate predicate = new TObjBasePredicate(pListField);
            int loFldIdx = pListTable.fTableDef.HighTableRevision.Fields.FindIndex(predicate.CompareByName_CaseInsensitive);

            if (loFldIdx == -1) return "";
            if ((pRowIdx < 0) || (pRowIdx >= pListTable.GetRecCount())) return "";

            // Get field value from the table. First we have to read the desired record
            pListTable.ReadRecord(pRowIdx);

            string FieldData = null;
            if (DestMask == null)
                FieldData = pListTable.GetFormattedFieldData(loFldIdx, pListTable.fTableDef.GetField(loFldIdx).MaskForHH);
            else
                FieldData = pListTable.GetFormattedFieldData(loFldIdx, DestMask);

            return FieldData;
        }

        public static string GetFieldDataFromTable(Reino.ClientConfig.TTTable pListTable, int pListFieldIdx, int pRowIdx, string DestMask)
        {
            // Just return empty string if table, fieldname or row index is bad
            if (pListTable == null) return "";

            if (pListFieldIdx == -1) return "";
            if ((pRowIdx < 0) || (pRowIdx >= pListTable.GetRecCount())) return "";

            // Get field value from the table. First we have to read the desired record
            pListTable.ReadRecord(pRowIdx);

            string FieldData = null;
            if (DestMask == null)
                FieldData = pListTable.GetFormattedFieldData(pListFieldIdx, pListTable.fTableDef.GetField(pListFieldIdx).MaskForHH);
            else
                FieldData = pListTable.GetFormattedFieldData(pListFieldIdx, DestMask);

            return FieldData;
        }

        public int GetListNdx()
        {
            // If the list index is not trusted to be correct, then lets get it up-to-date first!
            if (_ListNdxNotTrusted == true)
                ResynchListNdx();
            return _ListNdx;
        }

        public int GetForcedNaturalNdx()
        {
            int loIdx = -1;
            if (_NaturalListSourceTable != null)
            {
                if (this._EditBuffer == "")
                {
                    loIdx = -1;
                    return loIdx;
                }
                loIdx = _NaturalListSourceTable.FilterSearch(NaturalListSourceFieldName, _EditBuffer, GetEditMask(), false);
                return loIdx;
            }
            return loIdx;
        }

        public int GetListPrimaryKey()
        {
            _ListSourceTable.ReadRecord(_ListNdx);
            _ListPrimaryKey = _ListSourceTable.GetPrimaryKey();
            return _ListPrimaryKey;
        }

        private void InitPopularItemsTable()
        {
            // no source table? 
            if (_ListSourceTable == null)
            {
                _PopularListSourceTable = null;
                return;
            }

            // Create filename for popular list
            string loPopularTableName = "";
            string loTableFileExt = ".POP";
            _ListSourceTable.fTableDef.GetTblName(ref loPopularTableName);
            loPopularTableName = Path.ChangeExtension(loPopularTableName, loTableFileExt);

            // Build FULL path and filename for popular list
            string loFullPathAndFileName = "";
            _ListSourceTable.fTableDef.GetTblPathName(ref loFullPathAndFileName);
            loFullPathAndFileName += loPopularTableName;

            // DEBUG -- Is it a color field? (Always make colors a popular list?)
            if ((string.Compare(this._CfgCtrl._Name, "VEHCOLOR1", true) == 0) ||
                (string.Compare(this._CfgCtrl._Name, "VEHCOLOR2", true) == 0) ||
                (string.Compare(this._CfgCtrl._Name, "SUSPEYECOLOR", true) == 0) ||
                (string.Compare(this._CfgCtrl._Name, "SUSPHAIRCOLOR", true) == 0))
            {
                if (_ListSourceTable.GetRecCount() > 0)
                {
                    if (File.Exists(loFullPathAndFileName) == false)
                    {
                        // Switch to regular .DAT file if no .POP exists
                        loPopularTableName = "";
                        loTableFileExt = ".DAT";
                        _ListSourceTable.fTableDef.GetTblName(ref loPopularTableName);
                        loPopularTableName = Path.ChangeExtension(loPopularTableName, loTableFileExt);
                        loFullPathAndFileName = "";
                        _ListSourceTable.fTableDef.GetTblPathName(ref loFullPathAndFileName);
                        loFullPathAndFileName += loPopularTableName;
                    }
                }
            }

            // ok, we have a name - is there such a file?
            if (File.Exists(loFullPathAndFileName) == true)
            {
                // see if find the associated table definition is already defined
                TTableDef loTableDef = Reino.ClientConfig.TTableListMgr.glTableMgr.GetTableDef(loPopularTableName);
                // not defined yet? 
                if (loTableDef == null)
                {
                    // create the table def for it - pass the name without the extension to the constructor
                    loTableDef = new TTableDef();
                    loTableDef.Name = loPopularTableName;
                    loTableDef.fOpenEdit = false; // open in read-only mode
                    loTableDef.fTblNameExt = loTableFileExt;

                    if (loTableDef.Revisions.Count == 0)
                    {
                        TTableDefRev DefRev = new TTableDefRev();
                        DefRev.Name = loPopularTableName;
                        loTableDef.Revisions.Add(DefRev);
                    }

                    // this isn't read from the CFG file, but we have to make sure the call is made to get things initialized
                    loTableDef.PostDeserialize(Reino.ClientConfig.TTableListMgr.glTableMgr);
                    // we're just a duplicate of the "real" table
                    loTableDef.CopyTableStructure(ref _ListSourceTable.fTableDef, false /* include virtual fields */ );
                }

                // create this table instance
                _PopularListSourceTable = new TTTable();
                _PopularListSourceTable.Name = loPopularTableName;
                _PopularListSourceTable.SetTableName(loPopularTableName);
            }
            else
            {
                _PopularListSourceTable = null;
            }
        }

        private void ResetListItemStats()
        {
        /* KLC
            // Set the record count for the number of virtual items
            if (_ListSourceTable != null)
                _ListItemCount = _ListSourceTable.GetRecCount();
            else
                _ListItemCount = 0;

            // Lets allow a few records to be cached to reduce the cost of always
            // reading records from physical storage. Setting the cache size also
            // clears existing entries.
            if (_ListItemCache == null)
                _ListItemCache = new ReinoControls.ReinoVirtualListBox.FixedSizeCache();
            _ListItemCache.Size = 25;

            // We also need a display cache if there is grid info
            if ((this._CfgCtrl != null) && (this._CfgCtrl is TEditField) &&
                ((this._CfgCtrl as TEditField).DBListGrid != null) &&
                ((this._CfgCtrl as TEditField).DBListGrid.Columns.Count > 0))
            {
                if (_GridDisplayCache == null)
                    _GridDisplayCache = new ReinoControls.ReinoVirtualListBox.FixedSizeCache();
                _GridDisplayCache.Size = 25;
            }
         */ 
        }

        public int SetListNdxAndPaint(int iListNdx)
        {
            _ListNdx = iListNdx;
            _ListNdxNotTrusted = false;
            if ((_ListSourceTable != null) && (_ListSourceTable.GetRecCount() > 0))
            {
                SetEditBufferAndPaint(GetListItem(_ListNdx));
            }
            // Update the listbox's selected index 
            UpdateListBoxIndex(_ListNdx);
            return 0;
        }

        public void RemoveDropDownButton()
        {
        /* KLC
            // Can't do it if there is no control
            if ((this.EditCtrl == null) || !(this.EditCtrl is ReinoTextBox))
                return;

            // Send message to the textbox to remove any right-hand margin that was used for our button.
            // The margin message takes a single parameter with both left and right margin values 
            // combined into a single long value. The LoWord contains the left margin and the 
            // HiWord contains the right margin.
            if (_HasRightMargin == true)
            {
                // Get platform dependant object we will use for calling a Windows API function
                IPlatformDependent WinAPI = GetWinAPI();
                const int EC_RIGHTMARGIN = 0x2;
                const int EM_SETMARGINS = 0xD3;
                int margin = 0 * 0x10000;
                WinAPI.SendMessage((IntPtr)this._EditCtrl.Handle, EM_SETMARGINS, EC_RIGHTMARGIN, margin);
                _HasRightMargin = false;
            }

            // If we have a drop down button, make it invisible, then dispose of it
            if (fDropDownButton != null)
            {
                fDropDownButton.Visible = false;
                fDropDownButton.Dispose();
                fDropDownButton = null;
            }
         */ 
        }

        private void CreateRegularListBox()
        {
        /* KLC
            // Create listbox (VIRTUAL listbox that doesn't host the data)
            if ((_ListStyle == ListBoxStyle.lbsStatic) ||
                (_ListStyle == ListBoxStyle.lbsRadio))
            {
                // If possible, get the new listbox control from the application assigned event.
                // If there is no event, then we can create our own.
                if (OnGetNewRegularListBox != null)
                {
                    // Get listbox control from application. We will use the "IsNewlyCreated" flag
                    // so GetVirtualTextForIndex is only attached once. The flag is necessary because
                    // the application might give us a reused control instead of a new control.
                    bool IsNewlyCreated = false;
                    ReinoVirtualListBox loNewListBox = OnGetNewRegularListBox(_ListStyle, ref IsNewlyCreated);
                    if (IsNewlyCreated == true)
                    {
                        loNewListBox.GetVirtualTextForIndex += new GetVirtualTextEventHandlder(ListBox_GetVirtualTextForIndex);
                        // Since the listbox can be reused for other fields, we will have to always supply
                        // an event for gathering virtual display text
                        loNewListBox.GetVirtualDisplayTextForIndex += new GetVirtualTextEventHandlder(ListBox_GetVirtualDisplayTextForIndex);
                    }
                    // Associate new control to internal variables based on the the list style
                    if (_ListStyle == ListBoxStyle.lbsStatic)
                        _StaticListBox = loNewListBox as ReinoTextBox.StaticListBox;
                    else if (_ListStyle == ListBoxStyle.lbsRadio)
                        _RadioListBox = loNewListBox as ReinoRadioList;
                }
                else
                {
                    // No app-assigned event for getting a new control, so we will create our own
                    // based on the list style
                    if (_ListStyle == ListBoxStyle.lbsStatic)
                        _StaticListBox = new ReinoTextBox.StaticListBox(false);
                    else if (_ListStyle == ListBoxStyle.lbsRadio)
                        _RadioListBox = new ReinoRadioList(false);
                    // Attach GetVirtualTextForIndex event to new listbox
                    ListBox.GetVirtualTextForIndex += new GetVirtualTextEventHandlder(ListBox_GetVirtualTextForIndex);
                    // Since the listbox can be reused for other fields, we will have to always supply
                    // an event for gathering virtual display text
                    ListBox.GetVirtualDisplayTextForIndex += new GetVirtualTextEventHandlder(ListBox_GetVirtualDisplayTextForIndex);

                    // Now notify application we created a listbox control so it can set properties
                    // and attach events as necessary
                    if (OnCreatedRegularListBox != null)
                        OnCreatedRegularListBox(ListBox);
                }

                // Associate the virtual listbox with the textbox
                if (_ListStyle == ListBoxStyle.lbsStatic)
                    _StaticListBox.EditCtrl = this.EditCtrl;
                else if (_ListStyle == ListBoxStyle.lbsRadio)
                    _RadioListBox.EditCtrl = this.EditCtrl;

                // Associate behavior with the listbox
                ListBox.Behavior = this;

                // Setup visual properties of the listbox
                if (this.EditCtrl != null)
                {
                    // JLA (3/2/07) Fixed for a problem exposed by Seattle demo
                    ListBox.Parent = this.EditCtrl.Parent;
                    this.EditCtrl.Parent.SuspendLayout();
                }
                ListBox.ResizeSuspended = true;
                int calcedListBoxWidth = ListBox.Width;
                if (this.EditCtrl != null) // JLA (3/2/07) Fixed for a problem exposed by Seattle demo
                {
                    ListBox.Location = new Point(this.EditCtrl.Left, this.EditCtrl.Top + this.EditCtrl.Height);
                    //ListBox.Width = this.EditCtrl.Width;
                    calcedListBoxWidth = this.EditCtrl.Width;
                }
                // If we ended up being about 60% or more of screen width, might as well take whole screen
                /*
                if (ListBox.Width >= 192)
                    ListBox.Width = 316; //320?
                * /
                if (ListBox.Parent != null)
                {
                    if (calcedListBoxWidth >= Convert.ToInt32(ListBox.Parent.Width * 0.60f))
                        calcedListBoxWidth = ListBox.Parent.Width - (12 * ReinoControls.BasicButton.ScaleFactorAsInt); // -4 - 13;
                }

                if (ListBox.Width != calcedListBoxWidth)
                    ListBox.Width = calcedListBoxWidth;

                // If we're displaying in grid-style, we can calc the width from the DBListGrid info
                // This is the default drawing for grid-style text
                if ((CfgCtrl is Reino.ClientConfig.TEditField) &&
                    ((CfgCtrl as TEditField).DBListGrid != null) &&
                    ((CfgCtrl as TEditField).DBListGrid.Columns.Count > 0))
                {
                    TTDBListBox loGrid = (CfgCtrl as TEditField).DBListGrid;
                    int loColIdx = 0;
                    int loWidth = 2; // for the border
                    foreach (TGridColumnInfo Column in loGrid.Columns)
                    {
                        // Adjust the width for the next column
                        loWidth += (Column.Width * ReinoControls.BasicButton.ScaleFactorAsInt);
                        // Increment column index
                        loColIdx++;
                    }
                    // Now set the width for the listbox (unless its a radio box)
                    if (_ListStyle != ListBoxStyle.lbsRadio)
                    {
                        calcedListBoxWidth = loWidth;
                    }
                    else
                    {
                        calcedListBoxWidth = Math.Max(loWidth, calcedListBoxWidth) + (16 * ReinoControls.BasicButton.ScaleFactorAsInt); // Add a little extra to account for "bullets" in radio buttons
                    }

                    // If we ended up being about 60% or more of screen width, might as well take whole screen
                    // if its a single column list
                    /*
                    if ((ListBox.Width >= 192) && (loGrid.Columns.Count == 1))
                        ListBox.Width = 316; //320?
                    * /
                    if (ListBox.Parent != null)
                    {
                        if (calcedListBoxWidth >= Convert.ToInt32(ListBox.Parent.Width * 0.60f))
                            calcedListBoxWidth = ListBox.Parent.Width - (12 * ReinoControls.BasicButton.ScaleFactorAsInt); // -4 - 13;
                    }

                    if (ListBox.Width != calcedListBoxWidth)
                        ListBox.Width = calcedListBoxWidth;
                }

                if (this.EditCtrl != null)
                {
                    this.EditCtrl.Parent.ResumeLayout();
                }

                // Lets allow a few records to be cached to reduce the cost of always
                // reading records from physical storage
                ListBox.Cache = _ListItemCache;
                ListBox.Cache.Size = 25;
                ListBox.DisplayCache = _GridDisplayCache;
                if (ListBox.DisplayCache != null)
                {
                    ListBox.DisplayCache.Size = 25;
                    //ListBox.DisplayCache.Clear();
                }
            }
         */ 
        }

        public void RefreshState()
        {
        /* KLC
            // JLA (3/2/07) Fixed for a problem exposed by Seattle demo
            /*
            // Can't do it if there is no control
            if (this.EditCtrl == null)
                return;
            * /
            // Nothing to do if there is no configuration object associated with the behavior
            if (this._CfgCtrl == null)
                return;

            // Determine if current control is editable
            bool IsEditable = ((this._CfgCtrl.IsEnabled == true) &&
                (this._CfgCtrl.IsProtected == false) && (this._CfgCtrl.IsHidden == false));

            // If its not editable, then hide the listbox if we have one.
            // Note: It is important that this be done by type, because although all 
            // listboxes are inherited from the same base, the "Visible" property does
            // not support polymorphism.
            if ((IsEditable == false) && (ListBox != null))
                SetListBoxVisible(false);

            // Make sure we don't have a dropdown button if we're not popup-style or not editable
            if ((_ListStyle != ListBoxStyle.lbsPopup) || (IsEditable == false))
            {
                RemoveDropDownButton();
            }
            // Is it eligible for a dropdown button?
            else if ((_ListStyle == ListBoxStyle.lbsPopup) &&
                (_ListSourceTable != null) && (_ListSourceTable.GetRecCount() > 0))
            {
                // Get platform dependant object we will use for calling a Windows API function
                IPlatformDependent WinAPI = GetWinAPI();
                //const int EC_LEFTMARGIN = 0x1;
                const int EC_RIGHTMARGIN = 0x2;
                const int EM_SETMARGINS = 0xD3;

                // Send message to the textbox to make a right-hand margin with enough room for our button.
                // The margin message itself takes a single parameter with both left and right margin values 
                // combined into a single long value. The LoWord contains the left margin and the 
                // HiWord contains the right margin.
                // We only need to set the right-margin if not already set
                if ((_HasRightMargin == false) && (this._EditCtrl != null))
                {
                    int margin = 16 * 0x10000;
                    WinAPI.SendMessage((IntPtr)this._EditCtrl.Handle, EM_SETMARGINS, EC_RIGHTMARGIN, margin);
                    _HasRightMargin = true;
                }

                // Do we need to create a drop down button? 
                if ((fDropDownButton == null) && (this._EditCtrl != null))
                {
                    // Create drop down button
                    fDropDownButton = new ReinoControls.ReinoDropDownButton();
                    fDropDownButton.Visible = false;
                    // Attach a click event and let it know we're its associated edit control
                    fDropDownButton.Click += new System.EventHandler(TextBoxBehavior.HandleDropDownClick);
                    fDropDownButton.EditCtrl = this._EditCtrl;
                }

                // Position dropdown button as appropriate for the operating system and rendering style
                if (this._EditCtrl != null)
                {
                    if (Environment.OSVersion.Platform == PlatformID.WinCE)
                    {
                        this._EditCtrl.Parent.SuspendLayout();
                        fDropDownButton.Parent = this._EditCtrl.Parent;
                        fDropDownButton.Bounds = new Rectangle(this._EditCtrl.Left + this._EditCtrl.Width - (16 * ReinoControls.BasicButton.ScaleFactorAsInt),
                            this._EditCtrl.Top + (1 * ReinoControls.BasicButton.ScaleFactorAsInt), (15 * ReinoControls.BasicButton.ScaleFactorAsInt),
                            this._EditCtrl.Height - (2 * ReinoControls.BasicButton.ScaleFactorAsInt));
                        fDropDownButton.Visible = true;
                        fDropDownButton.BringToFront();
                        this._EditCtrl.Parent.ResumeLayout();
                        fDropDownButton.Invalidate();
                    }
                    else
                    {
                        this._EditCtrl.Parent.SuspendLayout();
                        if (WinAPI.RenderWithVisualStyles() == true)
                        {
                            this._EditCtrl.Parent.SuspendLayout();
                            fDropDownButton.Parent = this._EditCtrl.Parent;
                            fDropDownButton.Bounds = new Rectangle(this._EditCtrl.Left + this._EditCtrl.Width - 18,
                                this._EditCtrl.Top + 1, 17, this._EditCtrl.Height - 2);
                        }
                        else
                        {
                            fDropDownButton.Parent = this._EditCtrl.Parent;
                            fDropDownButton.Bounds = new Rectangle(this._EditCtrl.Left + this._EditCtrl.Width - 19,
                                this._EditCtrl.Top + 2, 17, this._EditCtrl.Height - 3);
                        }
                        fDropDownButton.Visible = true;
                        fDropDownButton.BringToFront();
                        this._EditCtrl.Parent.ResumeLayout();
                        fDropDownButton.Invalidate();
                    }
                }
            }
            else if (_ListStyle == ListBoxStyle.lbsPopup)
            {
                // Make sure we don't display a dropdown button since there are no list items 
                RemoveDropDownButton();
            }

            // Are there any list items and the control is editable?
            if ((IsEditable == true) && (_ListSourceTable != null) && (_ListSourceTable.GetRecCount() > 0))
            {
                // Create static listbox if necessary
                if ((ListBox == null) &&
                    ((_ListStyle == ListBoxStyle.lbsStatic) || (_ListStyle == ListBoxStyle.lbsRadio)))
                {
                    // JLA (3/2/07) Fixed for a problem exposed by Seattle demo
                    // Can't do it if there is no control
                    if (this.EditCtrl != null)
                    {
                        CreateRegularListBox();
                        // Set the Visible property to true, which also prepares the message hooks
                        // Note: It is important that this be done by type, because although all 
                        // listboxes are inherited from the same base, the "Visible" property does
                        // not support polymorphism.
                        SetListBoxVisible(true);
                    }
                }
                else if ((ListBox != null) &&
                    ((_ListStyle == ListBoxStyle.lbsStatic) || (_ListStyle == ListBoxStyle.lbsRadio)))
                {
                    int calcedListBoxWidth = ListBox.Width;
                    // List box already exists, but lets size it appropriately
                    if (this.EditCtrl != null) // JLA (3/2/07) Fixed for a problem exposed by Seattle demo
                    {
                        //ListBox.Width = this.EditCtrl.Width;
                        calcedListBoxWidth = this.EditCtrl.Width;
                    }
                    // If we ended up being about 60% or more of screen width, might as well take whole screen
                    /*
                    if (ListBox.Width >= 192)
                        ListBox.Width = 316; //320?
                    * /
                    if (ListBox.Parent != null)
                    {
                        if (calcedListBoxWidth >= Convert.ToInt32(ListBox.Parent.Width * 0.60f))
                            calcedListBoxWidth = ListBox.Parent.Width - (12 * ReinoControls.BasicButton.ScaleFactorAsInt); // -4 - 13;
                    }

                    if (ListBox.Width != calcedListBoxWidth)
                        ListBox.Width = calcedListBoxWidth;
                }

                // If the listbox exists, update its contents 
                if (ListBox != null)
                {
                    ListBox.Count = _ListSourceTable.GetRecCount();
                    ListBox.RefreshItems(true);
                }
            }
         */ 
        }

        public void ResynchListNdx()
        {
            if (_ListSourceTable != null)
            {
                if (this._EditBuffer == "")
                {
                    _ListNdx = -1;
                    _ListNdxNotTrusted = false;
                    _ListPrimaryKey = -1;

                    // Update the listbox's selected index 
                    UpdateListBoxIndex(_ListNdx);
                    _ListSourceTable.ReadRecord(-1); // clear out the table cursor
                    return;
                }
                _ListNdx = _ListSourceTable.FilterSearch(ListSourceFieldName, _EditBuffer, GetEditMask(), false);
                _ListNdxNotTrusted = false;
                _ListPrimaryKey = _ListSourceTable.GetPrimaryKey();

                // Update the listbox's selected index 
                UpdateListBoxIndex(_ListNdx);
                return;
            }
            return;
        }
        /* KLC 
        private string ListBox_GetVirtualTextForIndex(object sender, GetVirtualTextEventArgs e)
        {
            // We can't do this if the sender wasn't a ReinoVirtualListBox
            if (!(sender is ReinoControls.ReinoVirtualListBox))
                return "";

            // Get the list item from the associated edit behavior
            return ((ReinoControls.ReinoVirtualListBox)(sender)).Behavior.GetListItem(e.Index);
        }

        private string ListBox_GetVirtualDisplayTextForIndex(object sender, GetVirtualTextEventArgs e)
        {
            // We can't do this if the sender wasn't a ReinoVirtualListBox
            if (!(sender is ReinoControls.ReinoVirtualListBox))
                return "";

            // Get the list item from the associated edit behavior
            return ((ReinoControls.ReinoVirtualListBox)(sender)).Behavior.GetDisplayListItem(e.Index);
        }
        */
        /// <summary>
        /// DoDropDown displays the PopupListBox in the correct location
        /// </summary>
        public void DoDropDown()
        {
        /* KLC
            // Only applicable to a textbox control
            if (!(this._EditCtrl is ReinoControls.ReinoTextBox))
                return;

            // Create listbox if necessary
            if (ListBox == null)
            {
                // Are we creating a popup listbox (simulating a combobox)?
                if (_ListStyle == ListBoxStyle.lbsPopup)
                {
                    _PopupListBox = new ReinoTextBox.PopupListBox(false);
                    // Do the OnCreatedPopupListBox event if there is one
                    if (OnCreatedPopupListBox != null)
                        OnCreatedPopupListBox(_PopupListBox);
                    // Associate the virtual ListBox with the TextBox
                    _PopupListBox.EditCtrl = this.EditCtrl;
                    _PopupListBox.Behavior = this;
                    _PopupListBox.GetVirtualTextForIndex += new GetVirtualTextEventHandlder(ListBox_GetVirtualTextForIndex);
                    // If there is DBListGrid info with 1 or more columns, then we will need to supply
                    // an event for virtual display text
                    if ((this._CfgCtrl is Reino.ClientConfig.TEditField) &&
                        ((this._CfgCtrl as TEditField).DBListGrid != null) &&
                        ((this._CfgCtrl as TEditField).DBListGrid.Columns.Count > 0))
                    {
                        _PopupListBox.GetVirtualDisplayTextForIndex += new GetVirtualTextEventHandlder(ListBox_GetVirtualDisplayTextForIndex);

                        // Let's do a header if there is more than one column?
                        if ((this._CfgCtrl as TEditField).DBListGrid.Columns.Count > 1)
                        {
                            _PopupListBox.OnMeasureHeader += new MeasureItemEventHandler(_PopupListBox_OnMeasureHeader);
                            _PopupListBox.DrawHeader += new DrawItemEventHandler(_PopupListBox_DrawHeader);
                        }
                    }
                    // Lets allow a few records to be cached to reduce the cost of always
                    // reading records from physical storage
                    _PopupListBox.Cache = _ListItemCache;
                    _PopupListBox.Cache.Size = 25;
                    _PopupListBox.DisplayCache = _GridDisplayCache;
                    if (_PopupListBox.DisplayCache != null)
                        _PopupListBox.DisplayCache.Size = 25;
                }
                // Are we creating a regular listbox (can be static or radio list)
                else if ((_ListStyle == ListBoxStyle.lbsStatic) ||
                        (_ListStyle == ListBoxStyle.lbsRadio))
                {
                    CreateRegularListBox();
                }
            }

            // Reset list item count and cache
            ResetListItemStats();

            // Update margin, dropdown button, and create listbox if necessary
            RefreshState();

            // Lets allow a few records to be cached to reduce the cost of always
            // reading records from physical storage
            ListBox.Cache = _ListItemCache;
            ListBox.Cache.Size = 25;
            ListBox.DisplayCache = _GridDisplayCache;
            if (ListBox.DisplayCache != null)
                ListBox.DisplayCache.Size = 25;

            // Initially prepare listbox for display directly below the textbox
            if (_ListStyle == ListBoxStyle.lbsPopup)
            {
                _PopupListBox.Parent = this.EditCtrl.Parent;
                _PopupListBox.ResizeSuspended = true;
                _PopupListBox.Left = this.EditCtrl.Left;
                _PopupListBox.Top = this.EditCtrl.Top + this.EditCtrl.Height;
                _PopupListBox.Width = Convert.ToInt32(this.EditCtrl.Width * _PopupListBox.ColWidthMultiplier) + (17 * ReinoControls.BasicButton.ScaleFactorAsInt); // Width multiplier, plus extra for VScrollBar width

                // If we're displaying in grid-style, we can calc the width from the DBListGrid info
                // This is the default drawing for grid-style text
                if ((CfgCtrl is Reino.ClientConfig.TEditField) &&
                    ((CfgCtrl as TEditField).DBListGrid != null) &&
                    ((CfgCtrl as TEditField).DBListGrid.Columns.Count > 0))
                {
                    TTDBListBox loGrid = (CfgCtrl as TEditField).DBListGrid;
                    int loColIdx = 0;
                    int loWidth = (2 * ReinoControls.BasicButton.ScaleFactorAsInt); // for the border
                    foreach (TGridColumnInfo Column in loGrid.Columns)
                    {
                        // Adjust the width for the next column
                        loWidth += (Column.Width * ReinoControls.BasicButton.ScaleFactorAsInt);
                        // Increment column index
                        loColIdx++;
                    }
                    // Now set the width for the listbox
                    _PopupListBox.Width = Convert.ToInt32(loWidth * _PopupListBox.ColWidthMultiplier); // Width multiplier, plus extra for VScrollBar width

                    // JLA 2008.10.06 - Set the height for upto 10 items + header
                    int itemsToDisplay = Math.Min(11, _ListSourceTable.GetRecCount() + 1);
                    _PopupListBox.Height = (itemsToDisplay * _PopupListBox.ItemHeight) + (4 * ReinoControls.BasicButton.ScaleFactorAsInt);

                    // 1st we'll try to align with right-side if goes past edge of screen
                    if (_PopupListBox.Left + loWidth > /*320* /_PopupListBox.Parent.Width)
                        _PopupListBox.Left = /*320* /_PopupListBox.Parent.Width - loWidth;
                    // Now lets make sure its not truncated on the left-side, which presumably
                    // has the most important data...
                    if (_PopupListBox.Left < 0)
                        _PopupListBox.Left = 0;
                }
                else
                {
                    // JLA 2008.10.06 - Set the height for upto 10 items
                    int itemsToDisplay = Math.Min(11, _ListSourceTable.GetRecCount());
                    _PopupListBox.Height = (itemsToDisplay * _PopupListBox.ItemHeight) + (4 * ReinoControls.BasicButton.ScaleFactorAsInt);
                }

                _PopupListBox.ResizeSuspended = false;
                _PopupListBox.RefreshItems(false); // Force it to be resized properly
                // Associate the virtual ListBox with the TextBox
                _PopupListBox.EditCtrl = this.EditCtrl;
            }

            // Update listbox index. If item text is blank, don't search entire list
            int defaultIdx = -1;
            if (this._EditBuffer != "")
            {
                // Let's use the filter search so index can be used to speed up searching
                /*defaultIdx = this.ListBox.Items.IndexOf(this._EditBuffer);* /
                defaultIdx = this._ListSourceTable.FilterSearch(this._ListSourceFieldName,
                    this._EditBuffer, this.GetEditMask(), false);
            }
            UpdateListBoxIndex(defaultIdx);

            // If its a static or radio list, make it visible
            // Note: It is important that this be done by type, because although all 
            // listboxes are inherited from the same base, the "Visible" property does
            // not support polymorphism.
            if ((ListBox != null) &&
                ((_ListStyle == ListBoxStyle.lbsStatic) ||
                (_ListStyle == ListBoxStyle.lbsRadio)))
            {
                SetListBoxVisible(true);
            }
            else if (_ListStyle == ListBoxStyle.lbsPopup)
            {
                // Set the default selected index for the popup list
                _PopupListBox.SelectedIndexWithoutChangeEvent = defaultIdx;

                // On the FULL .NET Framework, the popup listbox floats on the desktop, so
                // the position must be adjusted accordingly
                if (Environment.OSVersion.Platform != PlatformID.WinCE)
                {
                    // Convert relative coordinates to screen coordinates
                    Point P = this.EditCtrl.Parent.PointToScreen(new Point(/*this.EditCtrl* /_PopupListBox.Left, this.EditCtrl.Top));
                    int Y = P.Y + this.EditCtrl.Height - (1 * ReinoControls.BasicButton.ScaleFactorAsInt);
                    int X = P.X;
                    // Get platform dependant object we will use for calling a Windows API function
                    IPlatformDependent WinAPI = GetWinAPI();
                    // Get the working area of the desktop
                    Rectangle WorkAreaRect = WinAPI.GetWorkingArea();
                    // Adjust PopupListBox location
                    if ((Y + _PopupListBox.Height > WorkAreaRect.Bottom) && (P.Y - _PopupListBox.Height > 0))
                        Y = P.Y - _PopupListBox.Height;
                    _PopupListBox.Location = new Point(X, Y);
                }

                // Make the PopupListBox visible, then set keyboard focus
                _PopupListBox.Visible = true;

                // Retain current state, disable next validation, change focus, then restore validation state
                bool RetainedStatus = this.SkipNextValidation;
                this.SkipNextValidation = true;
                _PopupListBox.Focus();
                this.SkipNextValidation = RetainedStatus;

                _PopupListBox.EnsureVisible(_PopupListBox.SelectedIndex);
            }
         */ 
        }

        /* KLC
        void _PopupListBox_DrawHeader(object sender, DrawItemEventArgs e)
        {
            ReinoTextBox.PopupListBox loListBox = null;
            TTDBListBox loGrid = null;
            try
            {
                loListBox = sender as ReinoTextBox.PopupListBox;
                loGrid = (loListBox.Behavior._CfgCtrl as TEditField).DBListGrid;
            }
            catch { }

            // If there is no associated grid in the configuration, we can't do anything else
            if (loGrid == null)
                return;

            Rectangle rc = new Rectangle(e.Bounds.Left + (1 * ReinoControls.BasicButton.ScaleFactorAsInt), e.Bounds.Top, e.Bounds.Width, e.Bounds.Height);
            rc.Y += (1 * ReinoControls.BasicButton.ScaleFactorAsInt);
            CommonDraw.FillRectangle(e.Graphics, /*SystemColors.Control* /Color.Wheat/*Tan* /, rc);

            Color textColor = SystemColors.WindowText;
            Rectangle rcText = new Rectangle(rc.Left + (3 * ReinoControls.BasicButton.ScaleFactorAsInt), rc.Top, rc.Width - (3 * ReinoControls.BasicButton.ScaleFactorAsInt), rc.Height);
            Rectangle rcClipText = new Rectangle(rc.Left + (3 * ReinoControls.BasicButton.ScaleFactorAsInt), rc.Top, rc.Width - (3 * ReinoControls.BasicButton.ScaleFactorAsInt), rc.Height);
            int loColIdx = 0;
            foreach (TGridColumnInfo Column in loGrid.Columns)
            {
                // Draw the text for next column
                string loTitle = Column.ColTitle;
                rcClipText.Width = Convert.ToInt32((Column.Width * loListBox.ColWidthMultiplier) * ReinoControls.BasicButton.ScaleFactorAsInt);
                // Special case when no display name (ColTitle) is specified
                if (loTitle == "")
                {
                    if (Column._Name == "ISSUENO_DISPLAY")
                        loTitle = "Issue No.";
                    else if (Column._Name == "VEHLICNO")
                        loTitle = "Plate";
                    else if (Column._Name == "VEHLICSTATE")
                        loTitle = "State";
                    else
                        loTitle = Column._Name;
                }
                /* KLC
                CommonDraw.DrawText(e.Graphics, e.DoubleBuffer, loTitle, loListBox.Font, textColor, /*rcText* /rcClipText);
                CommonDraw.DrawLine(e.Graphics, Color.White, rcText.X + /*Column.Width* /rcClipText.Width - (3 * ReinoControls.BasicButton.ScaleFactorAsInt),
                    rc.Top, rcText.X + /*Column.Width* /rcClipText.Width - (3 * ReinoControls.BasicButton.ScaleFactorAsInt), rc.Top + rc.Height);
                    * /
                // Adjust text rectangle for next column to be drawn
                rcText.Width -= /*Column.Width* /rcClipText.Width;
                rcText.X += /*Column.Width* /rcClipText.Width;
                rcClipText = rcText;
                // Increment column index
                loColIdx++;
            }
        }
         */ 
         /* KLC
        void _PopupListBox_OnMeasureHeader(object sender, MeasureItemEventArgs e)
        {
         // KLC   ReinoTextBox.PopupListBox loListBox = sender as ReinoTextBox.PopupListBox;
         //KLC    e.ItemHeight = loListBox.ItemHeight;
        }
        */
        private void UpdateListBoxIndex(int Index)
        {
        /* KLC
            // Update the listbox's selected index (unless its a popup listbox)
            if (_StaticListBox != null)
                _StaticListBox.SelectedIndexWithoutChangeEvent = Index;
            if (_RadioListBox != null)
                _RadioListBox.SelectedIndexWithoutChangeEvent = Index;
         */ 
        }
        #endregion

        #region Edit Mask Functions
        public string GetEditMask()
        {
            return _EditMask;
        }

        /// <summary>
        /// Sets the fEditMask member to the value passed in iEditMask.
        /// Will allocate/FREE memory as necessary.
        /// </summary>
        public void SetEditMask(string iEditMask)
        {
            _EditMask = iEditMask;
            _EditMaskLen = iEditMask.Length;
            _MaxLength = _EditMaskLen;
            UpdateEditMaskAttributes();
            MergeEditBufferWMask();
        }

        public bool HasMaskAttr(int MaskAttr)
        {
            return (_MaskAttrs & MaskAttr) != 0;
        }

        public void ModifyMaskAttrs(int MaskAttr, bool AddMaskAttr)
        {
            if (AddMaskAttr)
                _MaskAttrs = _MaskAttrs | MaskAttr;
            else
                _MaskAttrs = _MaskAttrs & ~MaskAttr;
        }

        /// <summary>
        /// Returns the mask character at the position.  
        /// - If iPos is greater than maximum length, returns '!'.
        /// - If mask is null, returns '!' (any character).
        /// - If the mask is a single character mask (one  character applies to all positions), returns that character.
        /// - Otherwise, returns character at iPos in fEditMask.
        /// </summary>
        private char GetMaskCharForPos(int iPos)
        {
            // if position is beyond max length, return '!'
            if (iPos >= _MaxLength)
                return '!';

            // if no mask at all, default is '!' (any character).
            if (_EditMaskLen == 0)
                return '!';

            // in a single char mask, one character applies to all positions in string. 
            if (HasMaskAttr(emaSingleCharMask))
                return _EditMask[0];

            // any character beyond length of mask 
            if (iPos >= _EditMaskLen)
                return '!';

            return _EditMask[iPos];
        }

        /// <summary>
        /// Returns true if passed character is a literal character (is placed directly into field).
        /// </summary>
        private bool MaskCharIsLiteral(char iMaskChar)
        {
            return !(
                     (iMaskChar == '!') || (iMaskChar == 'A') || (iMaskChar == '9') || (iMaskChar == '&') ||
                     (iMaskChar == '0') || (iMaskChar == '@') || (iMaskChar == 'N') || (iMaskChar == 'm') ||
                     (iMaskChar == 'y') || (iMaskChar == 'd') || (iMaskChar == 'h') || (iMaskChar == '#') ||
                     (iMaskChar == 'Y') || (iMaskChar == 'D') || (iMaskChar == 'M') || (iMaskChar == 'H') ||
                     (iMaskChar == 'S') || (iMaskChar == 's') || (iMaskChar == 'X') || (iMaskChar == 't') ||
                     (iMaskChar == 'T')
                    );
        }

        private bool CharValidForMaskChar(ref char iChar, char iMaskChar)
        {
            // A "null" in either character signifies the end of string and can go anywhere 
            if ((iMaskChar.Equals((char)0)) || (iChar.Equals((char)0)))
                return true;

            // Analyze each known mask character
            switch (iMaskChar)
            {
                case 'X': // all characters are valid
                    return true;
                case '!': // all characters valid, just convert to uppercase
                    iChar = char.ToUpper(iChar);
                    return true;
                case 'A': // all characters except numerics 
                    iChar = char.ToUpper(iChar);
                    return !((iChar >= '0') && (iChar <= '9'));
                case '9': // numerics only
                case '0': // numerics only
                    return ((iChar >= '0') && (iChar <= '9'));
                case '&': // upper case alpha & numerics only
                    iChar = char.ToUpper(iChar);
                    return ((iChar >= '0') && (iChar <= '9')) || ((iChar >= 'A') && (iChar <= 'Z'));
                case '@': // upper case alpha only
                case 'N': // upper case alpha only
                    iChar = char.ToUpper(iChar);
                    return ((iChar >= 'A') && (iChar <= 'Z'));
                case 'M': // date and time masks only require numerics. Will validate total entry later. 
                case 'S':
                case 'D':
                case 'Y':
                case 'H':
                case 'm':
                case 's':
                case 'd':
                case 'y':
                case 'h':
                    return ((iChar >= '0') && (iChar <= '9'));
                case '#': // digits and decimal point only 
                    return ((iChar >= '0') && (iChar <= '9')) || (iChar == '.');
                case 't': // Time-of-Day indicator only accepts A, P, and M (case-insensitive)
                case 'T': // Time-of-Day indicator only accepts A, P, and M (case-insensitive)
                    return ((iChar == 'A') || (iChar == 'a') ||
                            (iChar == 'P') || (iChar == 'p') ||
                            (iChar == 'M') || (iChar == 'm'));
                default: // all other mask characters are interpeted as literals.  Character much match exactly. 
                    return iChar == iMaskChar;
            }
        }

        /// <summary>
        /// Virtual routine called by "SetEditBuffer".  Descendants implement this routine to
        /// verify that the data being placed in the edit buffer is valid for the field's mask.
        /// 
        /// Returns:
        ///   -1: invalid data.
        ///    0: valid data.
        /// </summary>
        private int DataValidForMask(string iData)
        {
            if (_EditMask.Length == 0 || _EditMask.Length == 0)
                return 0; // no mask, so anything goes... 

            char loDataChar;
            if (HasMaskAttr(emaSingleCharMask))
            {
                // just make sure all characters are valid for the mask. 
                for (int idx = 0; idx < iData.Length; idx++)
                {
                    loDataChar = ((idx < iData.Length && idx >= 0) ? iData[idx] : (char)0);
                    if (!CharValidForMaskChar(ref loDataChar, _EditMask[0]))
                        return -1;
                }
                return 0;
            }

            for (int idx = 0; idx < iData.Length; idx++)
            {
                if (idx >= _EditMask.Length)
                    return -1; // more data than mask characters. 

                loDataChar = ((idx < iData.Length && idx >= 0) ? iData[idx] : (char)0);
                if (!CharValidForMaskChar(ref loDataChar, _EditMask[idx]))
                    return -1;
            }
            return 0;
        }

        /// <summary>
        /// Routine that sets EditMaskAttibutes based on the field type and the edit mask.
        /// Is called whenever the edit mask or field type is changed.
        /// </summary>
        private void UpdateEditMaskAttributes()
        {
            _MaskAttrs = 0; // clear out the mask attributes
            // now let's do some analysis on this mask...
            if (_EditMaskLen == 1)
            {
                _MaskAttrs = emaSingleCharMask; // in a one char mask, the mask character applies to ALL positions.
                MergeEditBufferWMask();
                return; // that's all we need to know
            }

            // first off, is it a date or time mask? If so, then it is also a "fixed mask", meaning all
            // positions in the mask correspond to a single character in the edit field 
            if (_EditMask.IndexOf("hh") >= 0)
            {
                _MaskAttrs = emaFixedMask | emaTimeMask;
                MergeEditBufferWMask();
            }

            if (_EditMask.IndexOf("yy") >= 0)
            {
                _MaskAttrs = emaFixedMask | emaDateMask;
                MergeEditBufferWMask();
            }

            if (_EditMask.IndexOf("-") >= 0)
            {
                _MaskAttrs |= emaNegativeNumbers; // negative numbers are accepted
            }

            if (_EditMask.IndexOf(",") >= 0)
            {
                _MaskAttrs |= emaCommaSeparated;
            }

            if (_EditMask.IndexOf("$") >= 0)
            {
                _MaskAttrs |= emaCurrencySign;
            }

            int loPntPos;
            if ((loPntPos = _EditMask.IndexOf(".")) >= 0)
            {
                _MaskAttrs |= emaFixedPoint; // fixed number of digits after decimal point 
                // if mask chars are 0's, then will right pad w/ 0's after the decimal point. 
                for (loPntPos += 1; loPntPos < _EditMaskLen; loPntPos++)
                {
                    if (_EditMask[loPntPos] == '0')
                        _MaskAttrs |= emaZeroPadFractional;
                }
            }
            else if (_EditMask.IndexOf("#") >= 0)
            {
                _MaskAttrs |= emaFloatingPoint; // variable number of digits after decimal point
            }
            MergeEditBufferWMask();
        }
        #endregion

        #region Edit Buffer Functions
        private void ReplaceEditBuffCharAtIndex(int index, char character)
        {
            // create string builder object based on existing edit buffer
            StringBuilder buffer = new StringBuilder(_EditBuffer);
            // if valid index, remove existing character
            if (buffer.Length >= index + 1)
                buffer.Remove(index, 1);
            // insert new character unless its a null
            if (character != (char)0)
            {
                if (index <= buffer.Length)
                    buffer.Insert(index, character.ToString());
                else
                    buffer.Append(character.ToString());
            }
            // replace edit buffer with new string
            _EditBuffer = buffer.ToString();
        }

        private void InsertEditBuffCharAtIndex(int index, char character)
        {
            // create string builder object based on existing edit buffer
            StringBuilder buffer = new StringBuilder(_EditBuffer);
            // insert new character unless its a null
            if (character != (char)0)
            {
                if (index <= buffer.Length)
                    buffer.Insert(index, character.ToString());
                else
                    buffer.Append(character.ToString());
            }
            // replace edit buffer with new string
            _EditBuffer = buffer.ToString();
        }

        /// <summary>
        /// Deletes the character at the passed position in fEditBuffer.
        /// 
        /// Is conscientious of edit masks; if there are mask literals after the
        /// current position, only characters before the literal are shifted left to occupy the
        /// delete space.  If a shifted character is invalid for its new mask position,
        /// it and any following characters will not be shifted.
        /// 
        /// Returns: 0 - character was NOT deleted.
        ///          1 - character WAS deleted, cursor should retreat.
        /// </summary>
        private int DeleteCharFromEditBuf(int iDelPos, bool MergeWithMask)
        {
            int loDataLen = EditBuffer.Length;

            // Is it a numeric field which is treated differently?
            if (_FieldType == TEditFieldType.efNumeric)
            {
                int loDecimalPos = -1;

                // do not delete '$'
                if ((loDataLen <= 0) || (_EditBuffer[iDelPos] == '$'))
                    return 0;

                // delete character at the passed index
                ReplaceEditBuffCharAtIndex(iDelPos, (char)0);

                // shift the decimal point to the left if a fixed decimal point
                if (((_MaskAttrs & emaFixedPoint) != 0) &&
                     ((loDecimalPos = _EditBuffer.IndexOf(".")) >= 0) &&
                     (loDecimalPos < iDelPos))
                {
                    // if no digits to the left of the decimal point, have to add one. 
                    if ((loDecimalPos == 0) || !((_EditBuffer[loDecimalPos - 1] >= '0') && (_EditBuffer[loDecimalPos - 1] <= '9')))
                        InsertEditBuffCharAtIndex(loDecimalPos++, '0');
                    ReplaceEditBuffCharAtIndex(loDecimalPos, (char)0);
                    InsertEditBuffCharAtIndex(loDecimalPos - 1, '.');
                }

                // re-format the data
                if (MergeWithMask == true)
                    MergeEditBufferWMask();
                return 1;
            }

            char loMaskChar;
            char loNextMaskChar;
            char loDataChar;

            // Time to start shifting. Shift until we find a mask literal 
            // or a character that doesn't match its new location mask 
            loMaskChar = GetMaskCharForPos(iDelPos);
            while (iDelPos < loDataLen)
            {
                if (MaskCharIsLiteral(loMaskChar))
                    return 0; // done shifting when we reach a literal 

                loNextMaskChar = GetMaskCharForPos(iDelPos + 1);
                if (MaskCharIsLiteral(loNextMaskChar))
                {
                    ReplaceEditBuffCharAtIndex(iDelPos, ' ');
                    return 0;
                }

                // can the next character go here? 
                loDataChar = ((iDelPos + 1 < _EditBuffer.Length && iDelPos + 1 >= 0) ? _EditBuffer[iDelPos + 1] : (char)0);
                if ((iDelPos + 1 >= loDataLen) || (!CharValidForMaskChar(ref loDataChar, loMaskChar)))
                {
                    // Character isn't valid. If its the last character, remove it completely,
                    // otherwise replace it with a space
                    if (iDelPos + 1 >= loDataLen)
                        ReplaceEditBuffCharAtIndex(iDelPos, (char)0);
                    else
                        ReplaceEditBuffCharAtIndex(iDelPos, ' ');
                    return 0;
                }
                // perform the shift with the validated character
                ReplaceEditBuffCharAtIndex(iDelPos, loDataChar);
                // update variables
                loDataLen = EditBuffer.Length;
                loMaskChar = loNextMaskChar;
                iDelPos++;
            }
            return 1;
        }

        /// <summary>
        /// Adds the passed  character at the passed position in fEditBuffer.
        /// 
        /// Is conscientious of edit masks; if there are mask literals after the
        /// current position, only characters before the literal are shifted left to occupy the
        /// delete space.  If a shifted character is invalid for its new mask position,
        /// it and any following characters will not be shifted.
        /// 
        /// Returns: 0 - character was NOT inserted.
        ///          1 - character WAS inserted, cursor should advance.
        /// </summary>
        private int AddCharToEditBuf(char iAddChar, int iAddPos)
        {
            int loDataLen = EditBuffer.Length;

            // Is it a numeric field which is treated differently?
            if (_FieldType == TEditFieldType.efNumeric)
            {
                int loDecPos;

                // only accept "-", ".", and 0-9 
                if ((iAddChar != '-') && (iAddChar != '.') &&
                     (!((iAddChar >= '0') && (iAddChar <= '9'))))
                    return 0;

                // if user pressed '-', and we accept negative numbers, either 
                // insert or remove '-' at begining of field
                if (iAddChar == '-')
                {
                    if ((_MaskAttrs & emaNegativeNumbers) == 0)
                        return 0; // no negatives allowed. 

                    if ((loDataLen > 0) && (_EditBuffer[0] == '-'))
                        ReplaceEditBuffCharAtIndex(0, (char)0); // remove character
                    else
                        InsertEditBuffCharAtIndex(0, '-'); // insert character
                    MergeEditBufferWMask();
                    return 1;
                } // if adding '-' 

                if (iAddChar == '.')
                {
                    if (!HasMaskAttr(emaFloatingPoint) && !HasMaskAttr(emaFixedPoint))
                        return 0; // do not accept decimal points.

                    // if the decimal point already exists, don't accept another. 
                    if (_EditBuffer.IndexOf(".") >= 0)
                        return 0;
                }

                // JLA 2009.07.21 - Manual entry support (Begin)
                // at this point, whatever character there is gets appended 
#if WindowsCE || __ANDROID__
                // For X3 handheld, we want the numeric entry to work like it does in the eC++ product
                InsertEditBuffCharAtIndex(loDataLen, iAddChar); // insert character
#else
                // For desktop, we will use a more intuitive functionality
                InsertEditBuffCharAtIndex(iAddPos, iAddChar); // insert character
#endif
                // JLA 2009.07.21 - Manual entry support (End)

                // shift decimal over if a fixed point 
                if ((iAddChar != '.') && (_EditMask.IndexOf(".") >= 0) &&
                     ((loDecPos = _EditBuffer.IndexOf(".")) >= 0))
                {
                    // swap decimal & character after it. 
                    ReplaceEditBuffCharAtIndex(loDecPos, _EditBuffer[loDecPos + 1]);
                    ReplaceEditBuffCharAtIndex(loDecPos + 1, '.');
                }
                MergeEditBufferWMask();
                return 1;
            }

            char loMaskChar;
            char loShiftChar;
            int loResult = 0;

            // if add pos is beyond end of field, put it at end of field (or beginning if empty) 
            if (iAddPos >= _MaxLength) iAddPos = Math.Max(0, _MaxLength - 1);

            // Time to start shifting. Shift until we find a mask literal or a 
            // character that doesn't match its new location mask
            for (; (iAddPos < _MaxLength) && (iAddPos <= (loDataLen + 1)); iAddPos++)
            {
                loMaskChar = GetMaskCharForPos(iAddPos);
                // can this character go here? 
                if ((!CharValidForMaskChar(ref iAddChar, loMaskChar)) || MaskCharIsLiteral(loMaskChar))
                {
                    return loResult; // nope! 
                }

                // Save existing character in current position, being careful to avoid invalid index
                loShiftChar = ((iAddPos < _EditBuffer.Length && iAddPos >= 0) ? _EditBuffer[iAddPos] : (char)0);
                ReplaceEditBuffCharAtIndex(iAddPos, iAddChar); // replace it
                iAddChar = loShiftChar; // ...now it will be added next time through.
                loResult = 1; // character was inserted. 
            }

            return loResult;
        }

        private int MergeEditBufferWMask()
        {
            // Is it a numeric field which is treated differently?
            if (_FieldType == TEditFieldType.efNumeric)
            {
                string loTempStr = _EditBuffer;
                _EditBuffer = "";
                FormatNumberStr(loTempStr, _EditMask, ref _EditBuffer);
                SetText(_EditBuffer, WinControl_SetText_Options.sto_ReplaceText, 0, false);
                // We need to paint the control after setting the text
                PaintEditCtrl();
                return 0;
            }

            int loLitPos;
            int loDataLen = EditBuffer.Length;
            char loMaskChar;

            if (_EditMask.Length == 0)
            {
                SetText(_EditBuffer, WinControl_SetText_Options.sto_ReplaceText, 0, false);
                // We need to paint the control after setting the text
                PaintEditCtrl();
                return 0;
            }
            //  do a character by character comparison of data to mask characters. 
            for (loLitPos = 0; (loLitPos < _EditMask.Length) && (loLitPos < _MaxLength); loLitPos++)
            {
                loMaskChar = GetMaskCharForPos(loLitPos);
                if (MaskCharIsLiteral(loMaskChar))
                {
                    if (loDataLen <= loLitPos)
                    {
                        for (int loIdx = 0; loIdx < loLitPos - loDataLen + 1; loIdx++)
                        {
                            loDataLen = EditBuffer.Length;
                            ReplaceEditBuffCharAtIndex(loDataLen, ' ');
                        }
                    }

                    ReplaceEditBuffCharAtIndex(loLitPos, _EditMask[loLitPos]);
                    continue;
                } // if need to force a literal mask character into the data. 

                // not a literal character, so compare data to mask char (if there is data) 
                if (loLitPos >= loDataLen)
                    continue;

                char loDataChar;
                loDataChar = ((loLitPos < _EditBuffer.Length && loLitPos >= 0) ? _EditBuffer[loLitPos] : (char)0);
                if (!CharValidForMaskChar(ref loDataChar, loMaskChar))
                    ReplaceEditBuffCharAtIndex(loLitPos, ' ');
                else
                    ReplaceEditBuffCharAtIndex(loLitPos, loDataChar);
            }

            SetText(_EditBuffer, WinControl_SetText_Options.sto_ReplaceText, 0, false);
            // We need to paint the control after setting the text
            PaintEditCtrl();
            return 0;
        }

        /// <summary>
        /// Replaces the text in the edit buffer with iFieldText then repaints itself
        /// </summary>
        private int SetEditBufferInternal(string iFieldText, bool iNotifyDependents)
        {
            // Can't do anything if there is no max length
            if (_MaxLength <= 0) return 0;

            // save a copy for later comparisons if we are going to notify of any changes
            if (iNotifyDependents == true)
                _SavedEditBuffer = _EditBuffer;

            // Set buffer contents (truncated at max length)
            _EditBuffer = iFieldText;
            if (_EditBuffer.Length > _MaxLength)
                _EditBuffer = _EditBuffer.Remove(_MaxLength, _EditBuffer.Length - _MaxLength);

            MergeEditBufferWMask();

            // Keep list index up-to-date
            if ((_ListSourceTable != null) && (_ListSourceTable.GetRecCount() > 0))
            {
                // See if the existing index is still current
                bool loNeedToFindNewIndex = true;
                if ((_ListNdx >= 0) && (_ListNdx < _ListSourceTable.GetRecCount()))
                    if (_EditBuffer.Equals(GetListItem(_ListNdx)))
                        loNeedToFindNewIndex = false;

                // if the field value is being set to a list item, don't override that list item. 
                if (loNeedToFindNewIndex == true)
                {
                    // First, let's set the index to something entirely invalid
                    _ListNdx = -2;
                    // We need to set a flag indicating the index can't be trusted to be correct
                    _ListNdxNotTrusted = true;

                    // DEBUG -- Maybe we don't need to do this if there is no visible listbox and no dependents?
                    // (This is an attempt to speedup the ticket lookup screen)
                    loNeedToFindNewIndex = false;
                    /* KLC
                    if ((this.ListBox != null) && (this.ListBox.Visible == true))
                        loNeedToFindNewIndex = true;
                    if (this.Dependents.Count > 0)
                        loNeedToFindNewIndex = true;
                        */
                    // Do we still need to continue looking for new index?
                    if (loNeedToFindNewIndex == true)
                    {
                        // Look for a partial match if the source text is NOT blank
                        if (_EditBuffer != "")
                        {
                            // DEBUG: Should we use FilterSearch for increased search speed?
                            /*
                            int loLoopMax = _ListSourceTable.GetRecCount();
                            for (_ListNdx = 0; _ListNdx < loLoopMax; _ListNdx++)
                            {
                                if (GetListItem(_ListNdx).IndexOf(_EditBuffer) == 0)
                                {
                                    _ListNdxNotTrusted = false;
                                    // Update the listbox's selected index 
                                    UpdateListBoxIndex(_ListNdx);
                                    break;
                                }
                            }
                            */
                            _ListNdx = _ListSourceTable.FilterSearch(_ListSourceFieldName, _EditBuffer, GetEditMask(), false);
                        }
                    }
                    // Don't allow a bad index
                    if ((_ListNdx < -1) || (_ListNdx >= _ListSourceTable.GetRecCount()))
                    {
                        _ListNdx = -1;
                        // Update the listbox's selected index 
                        UpdateListBoxIndex(_ListNdx);
                    }
                    else
                    {
                        // Reset not trusted flag since we know its the correct index now
                        _ListNdxNotTrusted = false;
                        // Update the listbox's selected index 
                        UpdateListBoxIndex(_ListNdx);
                    }
                }
            }
            else
            {
                // No list items to use, so the index needs to be set to -1
                _ListNdx = -1;
                _ListNdxNotTrusted = false;
                // Update the listbox's selected index 
                UpdateListBoxIndex(_ListNdx);
            }

            // We're finished unless we need to notify dependents about our changes
            if (!iNotifyDependents)
                return 0;

            // Exit if nothing changed
            if (_EditBuffer.Equals(_SavedEditBuffer))
                return 0;

            // We need to use this flag to avoid infinite recursion caused by ProcessRestrictions
            if (_ExecutingSetEditBufferInternal == true)
                return 0;

            // Now set flag to avoid infinite recursion caused by ProcessRestrictions
            _ExecutingSetEditBufferInternal = true;
            /* KLC
            if (ProcessRestrictions(EditRestrictionConsts.dneDataChanged, null) != null)
            {
                // something failed, restore the data
                _EditBuffer = _SavedEditBuffer;
                _ExecutingSetEditBufferInternal = false;
                return 0;
            }
            */
            // Any changes now?
            if (_EditBuffer.Equals(_SavedEditBuffer))
            {
                _ExecutingSetEditBufferInternal = false;
                return 0; // no changes
            }
            NotifyDependents(EditRestrictionConsts.dneParentDataChanged);
            _ExecutingSetEditBufferInternal = false;
            return 0;
        }

        /// <summary>
        /// Replaces the text in the edit buffer with iFieldText, making sure it is valid for the
        /// field mask. TEditField will also find the associated list item if one exists.
        /// </summary>
        public int SetEditBuffer(string iFieldText)
        {
            return SetEditBuffer(iFieldText, false);
        }

        public int SetEditBuffer(string iFieldText, bool iNotifyDependents)
        {
            SetEditBufferInternal(iFieldText, iNotifyDependents);
            SetTextSelection(0, 0);
            return 0;
        }

        public void SetEditBufferAndPaint(string NewText)
        {
            // Set the edit buffer with the passed text. 
            // (This also merges with the edit mask, so the final edit buffer may differ)
            SetEditBuffer(NewText, true);

            // JLA 4/23/07: This is redundant with what SetEditBuffer does?
            /*
            // Now set the real text to match the final edit buffer after being merged with edit mask
            SetText(_EditBuffer, ReinoControls.WinControl_SetText_Options.sto_ReplaceText, 0, true);
            if (this._EditCtrl is ReinoTextBox)
            {
                // Finally, update the associated control's text
                ((ReinoTextBox)(this._EditCtrl)).BaseText = _EditBuffer;
            }
            */
        }

        public void SetEditBufferAndPaint(string NewText, bool RaiseChangeEvent)
        {
            // Set the edit buffer with the passed text. 
            // (This also merges with the edit mask, so the final edit buffer may differ)
            SetEditBuffer(NewText, RaiseChangeEvent);

            // JLA 4/23/07: This is redundant with what SetEditBuffer does?
            /*
            // Now set the real text to match the final edit buffer after being merged with edit mask
            SetText(_EditBuffer, ReinoControls.WinControl_SetText_Options.sto_ReplaceText, 0, RaiseChangeEvent);
            if (this._EditCtrl is ReinoTextBox)
            {
                // Finally, update the associated control's text
                ((ReinoTextBox)(this._EditCtrl)).BaseText = _EditBuffer;
            }
            */
        }

        /// <summary>
        /// Counts number of non-mask characters that the field contains.
        /// </summary>
        public int GetDataLen()
        {
            int loCharNdx;
            int loCharCnt;
            for (loCharCnt = 0, loCharNdx = 0; loCharNdx < _EditBuffer.Length; loCharNdx++)
            {
                if (MaskCharIsLiteral(GetMaskCharForPos(loCharNdx)))
                    continue; // ignore literals 
                // increment character count except for spaces
                if (_EditBuffer[loCharNdx] != ' ')
                    loCharCnt++;
            }
            return loCharCnt;
        }

        /// <summary>
        /// Returns true if a field is blank (i.e. only contains mask literals and spaces), false if not.
        /// </summary>
        public bool FieldIsBlank()
        {
            // If the edit buffer is completely empty, then of course the field is blank
            if (_EditBuffer == "")
                return true;

            // Is it a numeric field which is treated differently?
            if (_FieldType == TEditFieldType.efNumeric)
            {
                int loNdx;
                // any numerics anywhere indicate a non-blank field
                for (loNdx = 0; loNdx < _EditBuffer.Length; loNdx++)
                {
                    if ((_EditBuffer[loNdx] >= '0') && (_EditBuffer[loNdx] <= '9'))
                        return false;
                }
                return true; // fell through w/o finding anything, so field is blank.
            }

            // alphas are pretty easy 
            int loCharNdx;
            for (loCharNdx = 0; loCharNdx < _EditBuffer.Length; loCharNdx++)
            {
                if (MaskCharIsLiteral(GetMaskCharForPos(loCharNdx)))
                    continue; // ignore literals 
                if (_EditBuffer[loCharNdx] != ' ')
                    return false; //any non-literal, non-space indicates a non-blank field
            }
            return true; // fell through w/o finding anything, so field is blank.
        }
        #endregion

        #region Text Value Functions
        public string GetText()
        {
            return _Text;
        }

        // Raises the TextChanged event.  
        protected virtual void OnTextChanged(EventArgs e)
        {
            if (this.TextChanged != null)
                this.TextChanged(this, e);
        }

        public void RaiseTextChangedEvent()
        {
            OnTextChanged(new EventArgs());
        }

        /// <summary>
        /// Routine to update the displayed text of a WinControl.  If the existing TextBuf
        /// is large enough, the new text is placed in it.  Otherwise, a new TextBuf is
        /// allocated, and the old one is FREEd.
        ///  
        /// Expects:
        ///  - iText : New text to add.
        ///  - iOptions: one of...
        ///    - sto_InsertTextAtPos: iText is inserted into the string at iPos. 
        ///    - sto_ReplaceTextAtPos: iText is inserted at iPos, and overwrites any existing text at 
        ///      that position.
        ///    - sto_InsertTextAfterPos: iText is inserted into the string after iPos.
        ///    - sto_ReplaceTextAfterPos: iText is inserted after iPos, and overwrites any existing text  
        ///      that position.
        ///    - sto_ReplaceText: iText replaces existing text in its entirety. iPos is not used.
        ///  - iPos: Position to place text into fTextBuf for all options except sto_ReplaceText
        /// </summary>
        public int SetText(string iText, WinControl_SetText_Options iOptions, int iPos, bool RaiseChangedEvent)
        {
            // Keep copy of previous text
            string PrevText = _Text;

            int loNewTextLen = iText.Length;
            int loOldTextLen = _Text.Length;

            // iPos can't be longer than TextBuf. iPos = -1 means end of line 
            if ((iPos < 0) || (iPos > loOldTextLen))
                iPos = loOldTextLen;

            // determine how much space we will need in the buffer 
            switch (iOptions)
            {
                case WinControl_SetText_Options.sto_InsertTextAfterPos:
                    iPos++;
                    goto case WinControl_SetText_Options.sto_InsertTextAtPos;
                case WinControl_SetText_Options.sto_InsertTextAtPos:
                    break;
                case WinControl_SetText_Options.sto_ReplaceTextAfterPos:
                    iPos++;
                    goto case WinControl_SetText_Options.sto_ReplaceTextAtPos;
                case WinControl_SetText_Options.sto_ReplaceTextAtPos:
                    break;
                case WinControl_SetText_Options.sto_ReplaceText:
                    iPos = 0;
                    _Text = ""; // delete the existing string
                    break;
            }

            if ((iText.Length > 0))
            {
                // If simple replacement, just assign text directly, otherwise use a stringbuilder
                if (iOptions == WinControl_SetText_Options.sto_ReplaceText)
                {
                    _Text = iText;
                }
                else
                {
                    // place the new string in the TextBuf 
                    StringBuilder buffer = new StringBuilder(_Text);
                    buffer.Insert(iPos, iText);
                    _Text = buffer.ToString();
                }
            }

            // if the current cursor char position is beyond the end, set it to end. 
            if (_CursorCharPos > loNewTextLen)
                _CursorCharPos = loNewTextLen;

            // Trigger the TextChanged event if the text value changed and we're flagged to raise the event
            if ((RaiseChangedEvent == true) && (PrevText != _Text))
                this.OnTextChanged(EventArgs.Empty);

            // all done
            return 0;
        }
        #endregion

        #region Edit Restrictions / Dependant Notifications
        /*
        public Reino.ClientConfig.TEditRestriction ProcessRestrictions(int iNotifyEvent, TextBoxBehavior iParentBehavior)
        {
            // defend against circular references. Don't allow a nested invocation for the same object instance.
            if (_ProcessRestrictionsDepth > 0)
                return null;
            _ProcessRestrictionsDepth++;

            int loNdx;
            Reino.ClientConfig.TEditRestriction loRestrict;
            Reino.ClientConfig.TEditRestriction loFailedRestrict = null;

            // Use the OnGetFormEditMode event if we have one, 
            // otherwise just assume the default "femNewEntry" mode
            int loFormEditMode;
            if (OnGetFormEditMode != null)
                loFormEditMode = OnGetFormEditMode();
            else
                loFormEditMode = EditRestrictionConsts.femNewEntry;

            // Loop through each edit restriction
            int loLoopMax = EditRestrictions.Count;
            for (loNdx = 0; loNdx < loLoopMax; loNdx++)
            {
                loRestrict = EditRestrictions[loNdx];
                // EnforceRestrictions is only linked-in if the USE_DEFN_IMPLEMENTATION directive is set
#if USE_DEFN_IMPLEMENTATION
                if (loRestrict.EnforceRestriction(iNotifyEvent, loFormEditMode, ref iParentBehavior) == true)
                {
                    _ProcessRestrictionsDepth--;
                    // If this is the first failed one, retain it for a return value
                    if (loFailedRestrict == null)
                        loFailedRestrict = loRestrict; // This one failed
                    //return loRestrict; // This one failed
                }
#endif
            }
            _ProcessRestrictionsDepth--;

            // If one failed, return it
            if (loFailedRestrict != null)
                return loFailedRestrict;
            else
                return null;
        }
        */
        public void NotifyDependents(int iNotifyEvent)
        {
        /* KLC
            // Exit if there are no dependents to notify
            int loLoopMax = Dependents.Count;
            if (loLoopMax == 0)
                return;

            // Call DependentNotification for each dependent
            for (int loNdx = 0; loNdx < loLoopMax; loNdx++)
                Dependents[loNdx].DependentNotification(iNotifyEvent, this);

            // Should we raise a "NotifiedDependentsParentDataChanged" event?
            if ((iNotifyEvent & EditRestrictionConsts.dneParentDataChanged) > 0)
            {
                if (NotifiedDependentsParentDataChanged != null)
                    NotifiedDependentsParentDataChanged(this, new EventArgs());
            }
         */ 
        }

        public void DependentNotification(int iNotifyEvent, TextBoxBehavior iParentBehavior)
        {
            // defend against circular references. Don't allow a nested invocation for the same object instance.
            if (_DependentNotificationDepth > 0)
                return;
            _DependentNotificationDepth++;

            // retain current field value
            string loSavedData = _EditBuffer;

            // process our edit restrictions
            //KLC ProcessRestrictions(iNotifyEvent, iParentBehavior);

            // don't let "ParentFieldExit" propegate down 
            if ((iNotifyEvent & EditRestrictionConsts.dneParentFieldExit) > 0)
            {
                iNotifyEvent |= EditRestrictionConsts.dneAncestorFieldExit;
                iNotifyEvent &= ~EditRestrictionConsts.dneParentFieldExit;
            }

            // Set or Clear the ParentDataChanged flag if current field value was changed
            if (loSavedData.Equals(_EditBuffer) == false)
                iNotifyEvent |= EditRestrictionConsts.dneParentDataChanged;
            else
                iNotifyEvent &= ~EditRestrictionConsts.dneParentDataChanged;

            // Now notify our dependents 
            if (((iNotifyEvent > 0) && (iNotifyEvent != EditRestrictionConsts.dneFormInit) &&
                (iNotifyEvent != EditRestrictionConsts.dneFirstEditFocus)))
                NotifyDependents(iNotifyEvent);

            _DependentNotificationDepth--;
        }
        #endregion

        #region Validations
        public int ValidateSelf(ref string oErrMsg)
        {
            int loResult = 0;
            DateTime loOSDate = DateTime.Today;
            DateTime loOSTime = DateTime.Now;
            Reino.ClientConfig.TEditRestriction loRestrict;
            oErrMsg = "";
            /* KLC
            // Process our restrictions and see if we get one that failed
            if ((loRestrict = ProcessRestrictions(EditRestrictionConsts.dneValidate, null)) != null)
            {
                // loRestrict points to restriction that failed.
                // Lets beautify the name by replacing underscores with spaces
                string loRestrictionDisplayName = loRestrict.Name.Replace("_", " ");
                oErrMsg = loRestrictionDisplayName + " FAILED!";

                // Can the failure be overridden?
                if (loRestrict.Overrideable)
                {
                    RingBell(this);
                    if (QueryUser(oErrMsg, "Do you wish to correct this?"))
                    {
                      //KLC  loRestrict.SetDisabledIfNoChange(false);
                        loResult = -1;
                    }
                    // the user has chosen to ignore this error. 
                    // Make sure this check isn't enforced until the data changes 
                    //KLC  loRestrict.SetDisabledIfNoChange(true);
                }
                else
                {
                    RingBell(this);
                    loResult = -1;
                }
            }
            */
            if (loResult != 0)
                return loResult;

            // make sure date & time fields are valid 
            if ((_FieldType == TEditFieldType.efDate) && (!FieldIsBlank()))
            {
                if ((loResult = DateStringToOSDate(_EditMask, _EditBuffer, ref loOSDate)) < 0)
                {
                    if (this._CfgCtrl != null)
                        oErrMsg = "Invalid date for " + this._CfgCtrl.Name + ".\nMask: " + _EditMask + " Data: " + _EditBuffer;
                    else
                        oErrMsg = "Invalid date" + ".\nMask: " + _EditMask + " Data: " + _EditBuffer;
                    RingBell(this);
                }
                else
                {
                    string loTmpDate = "";
                    OSDateToDateString(loOSDate, _EditMask, ref loTmpDate);
                    SetEditBufferAndPaint(loTmpDate);
                }
                return loResult; // not a valid date 
            }

            if ((_FieldType == TEditFieldType.efTime) && (!FieldIsBlank()))
            {
                if ((loResult = TimeStringToOSTime(_EditMask, _EditBuffer, ref loOSTime)) < 0)
                {
                    if (this._CfgCtrl != null)
                        oErrMsg = "Invalid time for " + this._CfgCtrl.Name + ".\nMask: " + _EditMask + " Data: " + _EditBuffer;
                    else
                        oErrMsg = "Invalid time" + ".\nMask: " + _EditMask + " Data: " + _EditBuffer;
                    RingBell(this);
                }
                else
                {
                    string loTmpTime = "";
                    OSTimeToTimeString(loOSTime, _EditMask, ref loTmpTime);
                    SetEditBufferAndPaint(loTmpTime);
                }
                return loResult; // not a valid date 
            }

            return 0;
        }

        public static bool SkipRebuildDisplay = false;
        public static bool RebuildDisplayWasSkipped = false;
        // KLCpublic event RestrictionForcesDisplayRebuild OnRestrictionForcesDisplayRebuild = null;
        public bool SkipNextOkToExit = false;

        public bool OkToExit(bool iExitForward)
        {
        /* KLC
            // If flagged to skip this execution, reset flag, then exit
            if (SkipNextOkToExit == true)
            {
                SkipNextOkToExit = false;
                return true;
            }
            // Reset SkipNextOkToExit for next time
            SkipNextOkToExit = false;

            string loValidateErrMsg = "";
            if ((!iExitForward) || (ValidateSelf(ref loValidateErrMsg) == 0))
            {
                // We're valid, so close any previous popup balloon messages
                ClosePopupBalloons();
                // Let our children do stuff they care about. Edit Restictions might want to
                // rebuild the display, but let's make it only happen once
                if (OnRestrictionForcesDisplayRebuild != null)
                {
                    ReinoControls.TextBoxBehavior.SkipRebuildDisplay = true;
                    ReinoControls.TextBoxBehavior.RebuildDisplayWasSkipped = false;
                }
                else
                {
                    ReinoControls.TextBoxBehavior.SkipRebuildDisplay = false;
                    ReinoControls.TextBoxBehavior.RebuildDisplayWasSkipped = false;
                }

                // Notify dependents that our focus has left
                NotifyDependents(EditRestrictionConsts.dneParentFieldExit);

                // JLA: Attempt to reduce flicker caused by HotSheet searches.
                // If a window was specifically marked as invalidated, we will turn on 
                // drawing support for it (and all children), then force an immediate repaint
                if (TextBoxBehavior.InvalidatedWindowAfterEditRestrictions != null)
                {
                    IPlatformDependent WinAPI = null;
                    if (Environment.OSVersion.Platform == PlatformID.WinCE)
                        WinAPI = new WinCEAPI();
                    else
                        WinAPI = new Win32API();
                    WinAPI.SendMessage(TextBoxBehavior.InvalidatedWindowAfterEditRestrictions.Handle, 0x000B /*WM_SETREDRAW* /, 1, 0);
                    TextBoxBehavior.InvalidatedWindowAfterEditRestrictions.Refresh();

                    // Reset reference for invalidated window
                    TextBoxBehavior.InvalidatedWindowAfterEditRestrictions = null;
                }

                if ((RebuildDisplayWasSkipped == true) && (OnRestrictionForcesDisplayRebuild != null))
                {
                    // Turn off flags, then force rebuild
                    ReinoControls.TextBoxBehavior.SkipRebuildDisplay = false;
                    ReinoControls.TextBoxBehavior.RebuildDisplayWasSkipped = false;
                    OnRestrictionForcesDisplayRebuild();
                }
                else
                {
                    // Turn off flags for Rebuild procedure
                    ReinoControls.TextBoxBehavior.SkipRebuildDisplay = false;
                    ReinoControls.TextBoxBehavior.RebuildDisplayWasSkipped = false;
                }

                // Empty Key buffer so that keys don't stack up... (But don't empty out the mouse messages!)
                EmptyKeysFromMsgQueue();
                return true;
            }

            // Display the validation error message in a popup balloon window
            /* KLC
            if (loValidateErrMsg != "")
                DisplayBalloonMsg(loValidateErrMsg);
                */
            return false; // swallow the key so that we don't leave it.
        }

        public delegate bool OnQueryUserEvent(string iLine1, string iLine2);

        public static OnQueryUserEvent OnQueryUser = null;

        /// <summary>
        /// Displays a "Yes/No" form, returns True if user exited by pressing Yes. 
        /// </summary>
        private bool QueryUser(string iLine1, string iLine2)
        {
            // If no application-assigned event exists, we'll just use regular MessageBox
            if (OnQueryUser == null)
            {
            /*
                //DialogResult dlgRes = MessageBox.Show(iLine1 + "\n" + iLine2, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (dlgRes == DialogResult.Yes)
                    return true;
                else
             */ 
                    return false;
            }
            else
            {
                // Application-assigned event exists (probably to use a different message box)
                return OnQueryUser(iLine1, iLine2);
            }
        }

        public delegate void OnStandardMessageBoxEvent(string iText, string iCaption);

        public static OnStandardMessageBoxEvent OnStandardMessageBox = null;

        public static void RingBell(TextBoxBehavior Behavior)
        {
            // Get platform dependant object we will use for calling a Windows API function
            IPlatformDependent WinAPI = GetWinAPI();

            // Lets see if there is a specific preferred volume we should use
            int PreferredVolume = -1;
            if ((Behavior != null) && (Behavior.OnGetRingBellVolume != null))
                PreferredVolume = Behavior.OnGetRingBellVolume();

            // Now call our helper object that will playsound in a seperate thread.
            // The thread will set desired volume, play sound, wait for sound to finish, then restore volume.
            PlaySoundSupport SoundPlayer = new PlaySoundSupport();
            SoundPlayer.RingBellThreaded(PreferredVolume);
        }
        #endregion

        #region Popup Balloon Message
        /* KLC
        public ReinoControls.PopupBalloonMsg DisplayBalloonMsg(string MsgText)
        {
            // DEBUG -- Can't do this if there is no control to tie it to
            if (this._EditCtrl == null)
                return null;

            // See if the application might want to alter the default error message before we show in balloon
            if (OnCustomizeValidationErrorText != null)
                OnCustomizeValidationErrorText(ref MsgText, this);

            // Create popup balloon window
            if (PopupBalloon == null)
            {
                PopupBalloon = new ReinoControls.PopupBalloonMsg();
                this.PopupBalloon.Disposed += new EventHandler(PopupBalloonMsg_Disposed);
            }
            // Set balloon's text, size and position, then show it
            PopupBalloon.label1.Text = MsgText;
            PopupBalloon.setBalloonPosition(((Form)(this._EditCtrl.TopLevelControl)), this._EditCtrl);

            // Focus moves to the balloon when we show it, so skip the next validation event
            this.SkipNextValidation = true;
            PopupBalloon.Show();
            PopupBalloon.Visible = true;
            // Return focus to the edit control
            try
            {
                this._EditCtrl.Focus();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error inside DisplayBalloonMsg(): " + ex.Message);
            }

            this.SkipNextValidation = false;

            // Now that the PopupBalloonMsg is visible, include it in the 
            // application's Message Filtering loop if not already there.
            if (!ReinoMessageFilters.PopupBalloonMsgEventFilter.ObjectList.Contains(PopupBalloon))
                ReinoMessageFilters.PopupBalloonMsgEventFilter.ObjectList.Add(PopupBalloon);

            return PopupBalloon;
        }
        */
        /* KLC
        public void RepositionPopupBalloon(Control iSrcControl)
        {
        /* KLC
            // Can't do this if there is no balloon or source control
            if ((this.PopupBalloon == null) || (iSrcControl == null))
                return;
            // Reposition the balloon based on the source control's position
            PopupBalloon.setBalloonPosition(((Form)(iSrcControl.TopLevelControl)), iSrcControl);
         * /
        }
        */

        private void PopupBalloonMsg_Disposed(object sender, EventArgs e)
        {
            // KLC this.PopupBalloon = null;
        }

        public void ClosePopupBalloons()
        {
        /* KLC
            // Loop through all Popup Balloons registered in the application's PopupBalloonMsgEventFilter
            for (int CtrlIdx = 0; CtrlIdx < ReinoMessageFilters.PopupBalloonMsgEventFilter.ObjectList.Count; CtrlIdx++)
            {
                // Get the next object that filters messages
                Object MsgObj = ReinoMessageFilters.PopupBalloonMsgEventFilter.ObjectList[CtrlIdx];
                PopupBalloonMsg popupBalloon = null;
                if ((MsgObj != null) && (MsgObj is PopupBalloonMsg))
                    popupBalloon = (PopupBalloonMsg)(MsgObj);

                // If the popupBalloon is visible, close it
                if ((popupBalloon != null) && (popupBalloon.Visible == true))
                {
                    // Turn off visibility
                    popupBalloon.Visible = false;
                    // Remove popup balloon from list of objects we are filtering messages for
                    // application's Message Filtering loop if not already there.
                    if (ReinoMessageFilters.PopupBalloonMsgEventFilter.ObjectList.Contains(popupBalloon))
                        ReinoMessageFilters.PopupBalloonMsgEventFilter.ObjectList.Remove(popupBalloon);
                    // Now close the popup balloon form
                    popupBalloon.Close(); // Closing a form also disposes it
                }
            }
         */ 
        }
        #endregion

        #region Date/Time Functions
        /// <summary>
        /// Increments the portion of a date or time field where the cursor is.
        /// </summary>
        private void IncrementDateTimeComponentAtCursorPos(int iIncAmt)
        {
            int loComponentStartPos;
            int loComponentSize;
            StringBuilder loNumericMask = new StringBuilder();
            StringBuilder loComponentValue = new StringBuilder();
            string loNewValue = "";
            int loComponentIntValue;
            bool loTimeFieldIsAMPM;

            char loMaskCharAtCursor = GetMaskCharForPos(_CursorCharPos);
            // We can't do anything if the cursor is past the end of the mask
            if ((loMaskCharAtCursor == '!') || (_CursorCharPos >= _EditMask.Length))
                return;

            if ((_FieldType != TEditFieldType.efDate) && (_FieldType != TEditFieldType.efTime)) return;

            loTimeFieldIsAMPM = (_FieldType == TEditFieldType.efTime) &&
                              ((_EditMask.IndexOf("tt") >= 0) || (_EditMask.IndexOf("TT") >= 0));


            // cursor could be anywhere in the component. Find the begining.
            for (loComponentStartPos = _CursorCharPos;
                 (loComponentStartPos > 0) && (_EditMask[loComponentStartPos - 1] == loMaskCharAtCursor);
                 loComponentStartPos--) ;
            // find the size
            loNumericMask.Length = 0;
            loComponentValue.Length = 0;
            for (loComponentSize = 0; loComponentStartPos + loComponentSize < _EditMask.Length; loComponentSize++)
            {
                // We've gone too far if the mask character is different
                if (_EditMask[loComponentStartPos + loComponentSize] != loMaskCharAtCursor)
                {
                    break;
                }
                if (loComponentStartPos + loComponentSize < _EditBuffer.Length)
                    loComponentValue.Append(_EditBuffer[loComponentStartPos + loComponentSize]);
                else
                    loComponentValue.Append('0');
                if (loComponentSize == 0)
                    loNumericMask.Append('0');
                else
                    loNumericMask.Append('9');
            }

            // deal w/ AM/PM 1st (it is non-numeric)
            if (Char.ToUpper(loMaskCharAtCursor) == 'T')
            {
                // if current value is PM or blank then make it AM
                if (Char.ToUpper(loComponentValue[0]) != 'P')
                    loComponentIntValue = 0 + iIncAmt;
                else
                    loComponentIntValue = 1 + iIncAmt;
                if ((loComponentIntValue == 0) || (loComponentIntValue == 2))
                {
                    if (loMaskCharAtCursor == 't')
                        loNewValue = "am";
                    else
                        loNewValue = "AM";
                }
                else
                {
                    if (loMaskCharAtCursor == 't')
                        loNewValue = "pm";
                    else
                        loNewValue = "PM";
                }
            }
            else // not AM/PM, all others are numeric.
            {
                try
                {
                    loComponentIntValue = Convert.ToInt32(loComponentValue.ToString().Trim());
                }
                catch /*(Exception ex)*/
                {
                    loComponentIntValue = 0;
                }

                loComponentIntValue += iIncAmt;
                if (Char.ToUpper(loMaskCharAtCursor) == 'D')
                { // D is always day number
                    if (loComponentIntValue > 31) loComponentIntValue = 1;
                    else if (loComponentIntValue < 1) loComponentIntValue = 31;
                }
                else if ((Char.ToUpper(loMaskCharAtCursor) == 'M') || (Char.ToUpper(loMaskCharAtCursor) == 'S'))
                {
                    // M can be month or minute, depending on field type
                    if (_FieldType == TEditFieldType.efDate)
                    { // month no can't exceed 12
                        if (loComponentIntValue > 12) loComponentIntValue = 1;
                        else if (loComponentIntValue < 1) loComponentIntValue = 12;
                    }
                    else
                    { // minute or second cannot exceed 59
                        if (loComponentIntValue > 59) loComponentIntValue = 0;
                        else if (loComponentIntValue < 0) loComponentIntValue = 59;
                    }
                }
                else if (Char.ToUpper(loMaskCharAtCursor) == 'H')
                {
                    // H for hour
                    if (loTimeFieldIsAMPM)
                    { // hour between 1 & 12 in AM/PM time
                        if (loComponentIntValue > 12) loComponentIntValue = 1;
                        else if (loComponentIntValue < 1) loComponentIntValue = 12;
                    }
                    else
                    { // hour between 0 & 23 in military time
                        if (loComponentIntValue > 23) loComponentIntValue = 0;
                        else if (loComponentIntValue < 0) loComponentIntValue = 23;
                    }
                } // "H"
                else if (Char.ToUpper(loMaskCharAtCursor) == 'Y')
                { // deal w/ years
                    if (loComponentSize == 2)
                    {
                        if (loComponentIntValue > 99) loComponentIntValue = 0;
                        else if (loComponentIntValue < 0) loComponentIntValue = 99;
                    }
                    else if (loComponentSize == 4)
                    {
                        if (loComponentIntValue > 2100) loComponentIntValue = 1900;
                        else if (loComponentIntValue < 1900) loComponentIntValue = 2100;
                    }
                    else return;  // can't deal w/ year that isn't 2 or 4 digits

                }
                else return; // some bizarre mask char that we don't handle

                // have the new numeric, convert to a string
                loComponentValue.Length = 0;
                loComponentValue.Append(Convert.ToString(loComponentIntValue));

                // and format it
                FormatNumberStr(loComponentValue.ToString(), loNumericMask.ToString(), ref loNewValue);
            } // else dealing w/ a pure numeric

            // insert loNewValue into position in fEditBuf
            StringBuilder buffer = new StringBuilder(_EditBuffer);
            if (loComponentStartPos < buffer.Length)
                buffer.Remove(loComponentStartPos, Math.Min(loComponentSize, buffer.Length - loComponentStartPos));
            buffer.Insert(loComponentStartPos, loNewValue);
            _EditBuffer = buffer.ToString();
        }

        /// <summary>
        /// Converts a OSDate to a formated string. Accepted mask tokens are:
        /// WWW - 3 character day of week abreviation.
        /// WWWW - day of week spelled out.
        /// MM           - month number left padded with 0 to 2 digits
        /// mm           - month number trimmed to length
        /// MON          - 3 character month abbreviation
        /// MONTH        - month spelled out.
        /// D or d       - day number trimmed to length.
        /// DD           - day number left padded with 0 to 2 digits
        /// dd           - day number left padded with space to 2 digits
        /// DDD or ddd   - day number within a given year (Used for Julian date formats, ie. "yyyyddd")
        /// YY or yy     - 2 digit year
        /// YYYY or yyyy - 4 digit year
        /// </summary>
        public static int OSDateToDateString(DateTime iOSDate, string iPictureMask, ref string oDateString)
        {
            int loYear = 0;
            int loMonth = 0;
            int loDayNo = 0;
            int loDayOfWeek = 0;
            int loDayOfYear = 0;
            string[] DayFullNames = new string[7] { "SUNDAY", "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY" };
            string[] DayAbrevNames = new string[7] { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };
            // These arrays have an extra blank value at index 0, so "January" = 1 (instead of zero)
            string[] MonthFullNames = new string[13] { "", "JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY", "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER", "NOVEMBER", "DECEMBER" };
            string[] MonthAbrevNames = new string[13] { "", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };

            // convert the date into d/m/y and day of week
            loYear = iOSDate.Year;
            loMonth = iOSDate.Month;
            loDayNo = iOSDate.Day;
            loDayOfWeek = (int)iOSDate.DayOfWeek;
            loDayOfYear = iOSDate.DayOfYear;

            if (iPictureMask == "")
                iPictureMask = "YYYYMMDD";

            oDateString = "";
            string loMaskSubStr;
            for (int loIdx = 0; loIdx < iPictureMask.Length; )
            {
                // Get the next mask substring
                loMaskSubStr = iPictureMask.Substring(loIdx);

                if ((loMaskSubStr.StartsWith("MM")) ||
                     (loMaskSubStr.StartsWith("mm")))
                {   // Month number  
                    oDateString += loMonth.ToString().PadLeft(2, '0');
                    loIdx += 2;
                    continue;
                }

                else if ((loMaskSubStr.StartsWith("DDD")) ||
                    (loMaskSubStr.StartsWith("ddd")))
                {
                    // This is the day of the year number (so Feb 2
                    // would be day number 33).
                    oDateString += loDayOfYear.ToString().PadLeft(3, '0');
                    loIdx += 3;
                    continue;
                }

                else if ((loMaskSubStr.StartsWith("DD")) ||
                     (loMaskSubStr.StartsWith("dd")))
                {   // Day number  
                    oDateString += loDayNo.ToString().PadLeft(2, '0');
                    loIdx += 2;
                    continue;
                }

                else if ((loMaskSubStr.StartsWith("YYYY")) ||
                     (loMaskSubStr.StartsWith("yyyy")))
                {   // 4 digit year 
                    oDateString += loYear.ToString().PadLeft(4, '0');
                    loIdx += 4;
                    continue;
                }

                else if ((loMaskSubStr.StartsWith("YY")) ||
                     (loMaskSubStr.StartsWith("yy")))
                {   // 2 digit year 
                    oDateString += loYear.ToString().PadLeft(4, '0').Substring(2, 2);
                    loIdx += 2;
                    continue;
                }

                else if (loMaskSubStr.StartsWith("WWWW"))
                {   // day of week spelled out 
                    oDateString += DayFullNames[loDayOfWeek];
                    loIdx += 4;
                    continue;
                }

                else if (loMaskSubStr.StartsWith("WWW"))
                {   // day of week abreviated 
                    oDateString += DayAbrevNames[loDayOfWeek];
                    loIdx += 3;
                    continue;
                }

                else if (loMaskSubStr.StartsWith("MONTH"))
                {   // full Month name 
                    oDateString += MonthFullNames[loMonth];
                    loIdx += 5;
                    continue;
                }

                else if (loMaskSubStr.StartsWith("MON"))
                {   // abreviated Month name 
                    oDateString += MonthAbrevNames[loMonth];
                    loIdx += 3;
                    continue;
                }

                // wasn't a token, so add as a literal
                oDateString += iPictureMask[loIdx];
                loIdx++;
            }
            return 0;
        }

        /// <summary>
        /// Converts a OSTime to a formated string. Accepted mask tokens are:
        /// MM,mm        - minute left padded with 0 to 2 digits
        /// HH,hh        - hour left padded with 0 to 2 digits. (12 hr in presence of TT/tt, 24 hr in absence)
        /// SS,ss        - seconds left padded with 0 to 2 digits
        /// TT           - Uppercase AM/PM
        /// tt           - Lowercase am/pm
        /// </summary>
        public static int OSTimeToTimeString(DateTime iOSTime, string iPictureMask, ref string oTimeString)
        {
            int loSecond;
            int loMinute;
            int loHour;
            string loNumStr = "";
            bool lo12HrTime = false;

            // convert the time into h/m/s 
            loSecond = iOSTime.Second;
            loMinute = iOSTime.Minute;
            loHour = iOSTime.Hour;

            if (iPictureMask == "")
                return -1;

            // will we be using 24hr or 12 hour time? 
            lo12HrTime = (iPictureMask.IndexOf("T") >= 0) || (iPictureMask.IndexOf("t") >= 0);

            oTimeString = "";
            string loMaskSubStr;
            for (int loIdx = 0; loIdx < iPictureMask.Length; )
            {
                // Get the next mask substring
                loMaskSubStr = iPictureMask.Substring(loIdx);

                if ((loMaskSubStr.StartsWith("HH")) ||
                     (loMaskSubStr.StartsWith("hh")) ||
                     (loMaskSubStr.ToUpper().StartsWith("HH")))
                {  // hour number  
                    // 12 hour time? 
                    if (lo12HrTime && (loHour > 12))
                        loNumStr = Convert.ToString(loHour - 12);
                    else if (lo12HrTime && (loHour == 0))
                        loNumStr = Convert.ToString(12);
                    else
                        loNumStr = Convert.ToString(loHour);
                    oTimeString += loNumStr.PadLeft(2, '0');
                    loIdx += 2;
                    continue;
                }

                else if ((loMaskSubStr.StartsWith("MM")) ||
                     (loMaskSubStr.StartsWith("mm")) ||
                     (loMaskSubStr.ToUpper().StartsWith("MM")))
                {  // minute number  
                    oTimeString += loMinute.ToString().PadLeft(2, '0');
                    loIdx += 2;
                    continue;
                }

                else if ((loMaskSubStr.StartsWith("SS")) ||
                (loMaskSubStr.StartsWith("ss")) ||
                (loMaskSubStr.ToUpper().StartsWith("SS")))
                {  // seconds number  
                    oTimeString += loSecond.ToString().PadLeft(2, '0');
                    loIdx += 2;
                    continue;
                }

                else if ((loMaskSubStr.StartsWith("TT")) ||
                (loMaskSubStr.StartsWith("tt")) ||
                (loMaskSubStr.ToUpper().StartsWith("TT")))
                {  // AM/PM 
                    if (loMaskSubStr.StartsWith("T")) // if upper case 
                    {
                        if (loHour >= 12)
                            oTimeString += "PM";
                        else
                            oTimeString += "AM";
                    }
                    else   // else lower case 
                    {
                        if (loHour >= 12)
                            oTimeString += "pm";
                        else
                            oTimeString += "am";
                    }
                    loIdx += 2;
                    continue;
                }

                // special circumstance they just want the A or the P
                else if ((loMaskSubStr.StartsWith("T")) ||
                     (loMaskSubStr.StartsWith("t")))
                {  // AM/PM 
                    if (loMaskSubStr.StartsWith("T")) // if upper case 
                    {
                        if (loHour >= 12)
                            oTimeString += "P";
                        else
                            oTimeString += "A";
                    }
                    else   // else lower case 
                    {
                        if (loHour >= 12)
                            oTimeString += "p";
                        else
                            oTimeString += "a";
                    }
                    loIdx += 1;
                    continue;
                }

                // wasn't a token, so add as a literal
                oTimeString += iPictureMask[loIdx];
                loIdx++;
            }
            return 0;
        }

        /// <summary>
        /// Returns a substring starting at iStart position and upto iMaxLength characters.
        /// If iStart is invalid, and empty string is returned. If there are not enough characters
        /// to satisfy iMaxLength, then the returned substring will contain as many characters as
        /// possible.
        /// </summary>
        public static string SafeSubString(string iString, int iStart, int iMaxLength)
        {
            if (iStart > iString.Length - 1)
                return "";
            return iString.Substring(iStart, Math.Min(iMaxLength, (iString.Length - iStart)));
        }

        /// <summary>
        /// Converts a formated datestring to an OSDate. Accepted mask tokens are:
        /// WWW - 3 character day of week abreviation.
        /// WWWW - day of week spelled out.
        /// MM           - month number left padded with 0 to 2 digits
        /// mm           - month number trimmed to length
        /// MON          - 3 character month abbreviation
        /// MONTH        - month spelled out.
        /// D or d       - day number trimmed to length.
        /// DD           - day number left padded with 0 to 2 digits
        /// dd           - day number left padded with space to 2 digits
        /// YY or yy     - 2 digit year
        /// YYYY or yyyy - 4 digit year
        /// </summary>
        public static int DateStringToDMY(string iPictureMask, string iDateString, ref int oDayNo, ref int oMonth, ref int oYear)
        {
            const int InvalidMonth = -2;
            const int InvalidDay = -1;
            const int InvalidYear = -3;
            int dayOfYear = 0;

            string loNumStr = "";

            string[] DayFullNames = new string[7] { "SUNDAY", "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY" };
            string[] DayAbrevNames = new string[7] { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };
            // These arrays have an extra blank value at index 0, so "January" = 1 (instead of zero)
            string[] MonthFullNames = new string[13] { "", "JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY", "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER", "NOVEMBER", "DECEMBER" };
            string[] MonthAbrevNames = new string[13] { "", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };

            oYear = 2000;
            oMonth = 1;
            oDayNo = 1;

            if (iPictureMask == "")
                return -1;

            int loSrcIdx = 0;
            for (int loIdx = 0; loIdx < iPictureMask.Length; )
            {

                if (iPictureMask.Substring(loIdx).StartsWith("MM"))
                {  // Month number fixed at 2 chars 
                    loNumStr = SafeSubString(iDateString, loSrcIdx, 2);
                    if ((loNumStr.Equals(string.Empty)) || (loNumStr.Equals(" ")))
                        return InvalidMonth;
                    try { oMonth = Convert.ToInt32(loNumStr); }
                    catch { return InvalidMonth; }
                    loIdx += 2;
                    loSrcIdx += 2;
                    continue;
                }

                if (iPictureMask.Substring(loIdx).StartsWith("mm"))
                {  // Month number , 1 or 2 digits 
                    loNumStr = SafeSubString(iDateString, loSrcIdx, 2);
                    if ((loNumStr[1] > '9') || (loNumStr[1] < '0'))
                    { // only one digit of month 
                        loNumStr = loNumStr.Remove(1, 1);
                        loSrcIdx++;
                    }
                    else
                    {
                        loSrcIdx += 2;
                    }

                    if ((loNumStr.Equals(string.Empty)) || (loNumStr.Equals(" ")))
                        return InvalidMonth;
                    try { oMonth = Convert.ToInt32(loNumStr); }
                    catch { return InvalidMonth; }
                    loIdx += 2; // on to next token 
                    continue;
                }

                if (iPictureMask.Substring(loIdx).StartsWith("MONTH"))
                {  // full Month name.  
                    for (int loMonthArrayIdx = 1; loMonthArrayIdx <= 12; loMonthArrayIdx++)
                    {
                        if (iDateString.Substring(loSrcIdx).StartsWith(MonthFullNames[loMonthArrayIdx]))
                        {
                            oMonth = loMonthArrayIdx;
                            break;
                        }
                    }

                    // did we find a valid month? 
                    if (oMonth > 12)
                        return InvalidMonth; // invalid month

                    loSrcIdx += MonthFullNames[oMonth].Length;
                    loIdx += 5;
                    continue;
                }

                if (iPictureMask.Substring(loIdx).StartsWith("MON"))
                {  // abreviated Month name 
                    for (int loMonthArrayIdx = 1; loMonthArrayIdx <= 12; loMonthArrayIdx++)
                    {
                        if (iDateString.Substring(loSrcIdx).StartsWith(MonthAbrevNames[loMonthArrayIdx]))
                        {
                            oMonth = loMonthArrayIdx;
                            break;
                        }
                    }

                    // did we find a valid month? 
                    if (oMonth > 12)
                        return InvalidMonth;

                    loSrcIdx += MonthAbrevNames[oMonth].Length;
                    loIdx += 3;
                    continue;
                }

                if ((iPictureMask.Substring(loIdx).StartsWith("ddd")) || (iPictureMask.Substring(loIdx).StartsWith("DDD")))
                {
                    // This is a julian day, which means its the number of days in the year (so Jan 10 would = 10 but
                    // Feb 10 would = 41).
                    loNumStr = SafeSubString(iDateString, loSrcIdx, 3);
                    try { dayOfYear = Convert.ToInt32(loNumStr.Trim()); }
                    catch { return InvalidDay; }
                    loIdx += 3;
                    loSrcIdx += 3;
                    continue;
                }

                if (iPictureMask.Substring(loIdx).StartsWith("DD"))
                {  // Day number fixed at 2 chars 
                    loNumStr = SafeSubString(iDateString, loSrcIdx, 2);
                    try { oDayNo = Convert.ToInt32(loNumStr); }
                    catch { return InvalidDay; }
                    loIdx += 2;
                    loSrcIdx += 2;
                    continue;
                }

                if (iPictureMask.Substring(loIdx).StartsWith("dd"))
                {  // day number , 1 or 2 digits 
                    loNumStr = SafeSubString(iDateString, loSrcIdx, 2);
                    if ((loNumStr[1] > '9') || (loNumStr[1] < '0'))
                    { // only one digit of month 
                        loNumStr = loNumStr.Remove(1, 1);
                        loSrcIdx++;
                    }
                    else
                    {
                        loSrcIdx += 2;
                    }

                    try { oDayNo = Convert.ToInt32(loNumStr); }
                    catch { return InvalidDay; }
                    loIdx += 2;
                    continue;
                }

                if ((iPictureMask.Substring(loIdx).StartsWith("YYYY")) ||
                     (iPictureMask.Substring(loIdx).StartsWith("yyyy")))
                {  // 4 digit year 
                    try
                    {
                        loNumStr = SafeSubString(iDateString, loSrcIdx, 4);
                        oYear = Convert.ToInt32(loNumStr);
                    }
                    catch { return InvalidYear; }
                    // add a century to the year (if necessary) 
                    if (oYear < 100)
                    {
                        if (oYear < 30)
                            oYear += 2000;
                        else
                            oYear += 1900;
                    }

                    // don't allow dates before 1900 & after 2099
                    if ((oYear < 1900) || (oYear > 2099))
                        return InvalidYear;

                    loIdx += 4;
                    loSrcIdx += 4;
                    continue;
                }

                if ((iPictureMask.Substring(loIdx).StartsWith("YY")) ||
                     (iPictureMask.Substring(loIdx).StartsWith("yy")))
                {  // 2 digit year 
                    try
                    {
                        loNumStr = SafeSubString(iDateString, loSrcIdx, 2);
                        oYear = Convert.ToInt32(loNumStr);
                    }
                    catch { return InvalidYear; }
                    // add a century to the year 
                    if (oYear < 30)
                        oYear += 2000;
                    else
                        oYear += 1900;
                    loIdx += 2;
                    loSrcIdx += 2;
                    continue;
                }
                // wasn't a token, so skip it 
                loSrcIdx++;
                loIdx++;
            }

            // If this was a julian type date (day of year instead of day and month) then will have
            // to convert the day of year into a month and day.
            if ((oMonth == 1) && (oDayNo == 1) && (dayOfYear > 0))
            {
                // Probably a better way to get this but dont know of one yet. For now, we will
                // get a close estimate of what month it would be and then keep adding days 
                // until we get the exact day of year
                /*
                DateTime findDate = new DateTime(oYear, Convert.ToInt32(dayOfYear / 29), 1);
                while (findDate.DayOfYear != dayOfYear)
                {
                    // If we ever go past the actual year then something went wrong and we'll just
                    // return invalid day error.
                    if (findDate.Year > oYear) { return InvalidDay; }
                    findDate = findDate.AddDays(1);
                }
                */
                // Start with Jan 1st of given year, then simply add dayOfYear minus 1 day
                DateTime findDate = new DateTime(oYear, 1, 1);
                findDate = findDate.AddDays(dayOfYear - 1);

                // Ok, got exact date, get the month and day from it.
                oMonth = findDate.Month;
                oDayNo = findDate.Day;
            }

            // make sure we have a valid date 
            if ((oMonth < 1) || (oMonth > 12))
                return InvalidMonth;
            if (oDayNo <= 0)
                return InvalidDay;
            if (oYear < 0)
                return InvalidYear;
            if (oDayNo > DateTime.DaysInMonth(oYear, oMonth))
                return InvalidDay;
            return 0;
        }

        public static int DateStringToOSDate(string iPictureMask, string iDateString, ref DateTime oOSDate)
        {
            int loDayNo = 0;
            int loMonth = 0;
            int loYear = 0;
            int loResult = 0;

            if (iDateString == "" || iPictureMask == "")
                return -1;
            // 1st convert to DMY 
            if ((loResult = DateStringToDMY(iPictureMask, iDateString, ref loDayNo, ref loMonth, ref loYear)) < 0)
                return loResult; // failed! 
            // now from DMY to OSDate 
            oOSDate = new DateTime(loYear, loMonth, loDayNo);
            return 0;
        }

        public static int TimeStringToOSTime(string iPictureMask, string iTimeString, ref DateTime oOSTime)
        {
            int loHour = 0;
            int loSecond = 0;
            int loMinute = 0;
            int loResult = 0;

            if (iTimeString == "" || iPictureMask == "")
                return -1;

            if ((loResult = TimeStringToHMS(iPictureMask, iTimeString, ref loHour, ref loMinute, ref loSecond)) < 0)
                return loResult; // time conversion failed 
            oOSTime = new DateTime(2000, 1, 1, loHour, loMinute, loSecond);
            return 0;
        }

        /// <summary>
        /// Converts a formated time string to an OSTime. Accepted mask tokens are:
        /// MM,mm        - minute left padded with 0 to 2 digits
        /// HH,hh        - hour left padded with 0 to 2 digits. (12 hr in presence of TT/tt, 24 hr in absence)
        /// SS,ss        - seconds left padded with 0 to 2 digits
        /// TT           - Uppercase AM/PM
        /// tt           - Lowercase am/pm
        /// </summary>
        public static int TimeStringToHMS(string iPictureMask, string iTimeString, ref int oHour,
            ref int oMinute, ref int oSecond)
        {
            const int InvalidHour = -4;
            const int InvalidMinute = -5;
            const int InvalidSecond = -6;
            int loAMPMTime = 0;

            string loNumStr = "";

            oHour = 0;
            oMinute = 0;
            oSecond = 0;

            if (iPictureMask == "")
                return -1;

            int loSrcIdx = 0;
            for (int loIdx = 0; loIdx < iPictureMask.Length; )
            {

                if ((iPictureMask.Substring(loIdx).StartsWith("HH")) ||
                 (iPictureMask.Substring(loIdx).StartsWith("hh")) ||
                 (iPictureMask.Substring(loIdx).ToUpper().StartsWith("HH")))
                {  // hour fixed at 2 chars 
                    loNumStr = SafeSubString(iTimeString, loSrcIdx, 2);
                    if (loNumStr.Equals(string.Empty))
                        return InvalidHour;
                    try { oHour = Convert.ToInt32(loNumStr); }
                    catch { return InvalidHour; }
                    loIdx += 2;
                    loSrcIdx += 2;
                    continue;
                }

                if ((iPictureMask.Substring(loIdx).StartsWith("MM")) ||
                 (iPictureMask.Substring(loIdx).StartsWith("mm")) ||
                 (iPictureMask.Substring(loIdx).ToUpper().StartsWith("MM")))
                {  // minute fixed at 2 chars 
                    loNumStr = SafeSubString(iTimeString, loSrcIdx, 2);
                    try { oMinute = Convert.ToInt32(loNumStr); }
                    catch { return InvalidMinute; }
                    loIdx += 2;
                    loSrcIdx += 2;
                    continue;
                }

                if ((iPictureMask.Substring(loIdx).StartsWith("SS")) ||
                 (iPictureMask.Substring(loIdx).StartsWith("ss")) ||
                 (iPictureMask.Substring(loIdx).ToUpper().StartsWith("SS")))
                {  // seconds fixed at 2 chars 
                    loNumStr = SafeSubString(iTimeString, loSrcIdx, 2);
                    try { oSecond = Convert.ToInt32(loNumStr); }
                    catch { return InvalidSecond; }
                    loIdx += 2;
                    loSrcIdx += 2;
                    continue;
                }

                if ((iPictureMask.Substring(loIdx).StartsWith("TT")) ||
                 (iPictureMask.Substring(loIdx).StartsWith("tt")) ||
                 (iPictureMask.Substring(loIdx).ToUpper().StartsWith("TT")))
                {  // AM/PM indicator 
                    // Had better be AM or PM or am or pm 
                    loAMPMTime = 1;
                    if ((SafeSubString(iTimeString, loSrcIdx, 1) == "P") ||
                        (SafeSubString(iTimeString, loSrcIdx, 1) == "p"))
                        loAMPMTime++;
                    loIdx += 2;
                    loSrcIdx += 2;
                    continue;
                }

                // wasn't a token, so skip it 
                loSrcIdx++;
                loIdx++;
            }
            // add 12 to hour if it was PM 
            if (loAMPMTime > 0)
            {
                if ((oHour <= 0) || (oHour > 12))
                    return InvalidHour;

                // if PM, add 12 to hour (unless it is 12 PM) 
                // if AM set hour to 0 if it is 12. 
                if ((loAMPMTime == 2) && (oHour != 12))
                    oHour += 12;
                else if ((loAMPMTime == 1) && (oHour == 12))
                    oHour = 0; // JIRA: AUTOCITE-375
            } // if AM/PM time 

            // make sure we have a valid time 
            if ((oHour < 0) || (oHour >= 24))
                return InvalidHour;

            if ((oMinute < 0) || (oMinute >= 60))
                return InvalidMinute;

            if ((oSecond < 0) || (oSecond >= 60))
                return InvalidSecond;

            return 0;
        }
        #endregion

        #region Numeric Functions
        /// <summary>
        /// Numeric mask options:
        /// '-' : negative numbers allowed, signified by minus sign in 1st position
        /// '$' : currency symbol forced after negative sign.
        /// ',' : comma placed if preceding (more significant) digit exists.
        /// '.' : fixed location of decimal point.  
        /// '#' : Any digit or a decimal point.  Incompatible w/ '.'
        /// '9' : Any digit.
        /// '0' : Any digit.  string will be padded out from the decimal point w/ 0's to
        ///       this position. (left padded if before decimal point, right padded if after).
        /// '8' : Any digit.  Implied decimal point in 3rd position from right. 125.67 -> 12467, 125.6 -> 12560
        /// </summary>
        public static int FormatNumberStr(string iSrcStr, string iMask, ref string oResult)
        {
            int loSrcDecPos = -1;
            int loNumberIsZero = 1;
            int loNumberIsBlank = 1;
            int loMaxIntDigCnt = 0;
            int loMinIntDigCnt = -1;
            int loMaxFracDigCnt = 0;
            int loMinFracDigCnt = 0;
            int loFixedPnt = 0;
            int loFloatPnt = 0;
            int loNegative = 0;
            int loCurrency = 0;
            int loCommas = 0;
            int loImpliedDecimalPt = 0;

            int loResLen = 0;
            int loMaskNdx;
            int loFirstNumIdx = 0;
            string DestMask = "";
            if (iMask != null)
                DestMask = iMask;

            // Sometimes the mask might be in a string style. Under this case, we'll just
            // substitute with a character that has a meaning to us.
            if (DestMask.IndexOf("!") > -1)
            {
                DestMask = DestMask.Replace("!", "#");
            }

            // We can't do anything if there is no source string (or empty source string).
            if ((iSrcStr == null) || (iSrcStr.Length == 0) || (DestMask == null))
                return -1;

            // analyze the mask

            // if the mask is blank, leave it wide open.
            if (DestMask.Length == 0)
            {
                loFloatPnt = 1;
                loNegative = 1;
                loMaxIntDigCnt = 30;
            }

            for (loMaskNdx = 0; loMaskNdx < DestMask.Length; loMaskNdx++)
            {
                if (DestMask[loMaskNdx] == '$')
                {
                    loCurrency = 1;
                    continue;
                }

                if (DestMask[loMaskNdx] == ',')
                {
                    loCommas = 1;
                    continue;
                }

                if (DestMask[loMaskNdx] == '.')
                {
                    loFixedPnt = 1;
                    loFloatPnt = 0;
                    continue;
                }

                if (DestMask[loMaskNdx] == '8')
                {
                    loFixedPnt = 1;
                    loFloatPnt = 0;
                    loImpliedDecimalPt = 1;
                    loMaxFracDigCnt = 2;
                    loMinFracDigCnt = 2;
                    loMaxIntDigCnt = DestMask.Length - 2;
                    loMinIntDigCnt = 0; // this gets set to loMaxIntDigCnt below
                    break;
                }

                if (DestMask[loMaskNdx] == '-')
                {
                    loNegative = 1;
                    continue;
                }

                char MaskChar = DestMask[loMaskNdx];
                if (((MaskChar == '9') || (MaskChar == '#') || (MaskChar == '0') || (MaskChar == '8')) == false)
                    return -1; // invalid mask character 

                if (loFixedPnt == 0)
                {
                    // haven't encountered a decimal point yet. So this digit counts towards all the digits
                    // in a floating point number or towards the integer digits of a fixed point number 
                    if ((DestMask[loMaskNdx] == '#'))
                        loFloatPnt = 1; // '#' indicates a floating point number (decimal point can be anywhere) 

                    loMaxIntDigCnt++;
                    // a '0' indicates a "pad w/ 0". Mark the 1st 0 we encounter. 
                    if ((DestMask[loMaskNdx] == '0') && (loMinIntDigCnt == -1))
                    {
                        loMinIntDigCnt = loMaxIntDigCnt - 1;
                    }
                }
                else // a fixed point number 
                {
                    loMaxFracDigCnt++;
                    // a '0' indicates a "pad w/ 0", increment minimum number of digits 
                    if ((DestMask[loMaskNdx] == '0'))
                        loMinFracDigCnt = loMaxFracDigCnt;
                }
            } // for loop to analyze the mask 


            // loMinIntDigCnt was recorded as the 1st pad to char 'cuz didn't 
            // know how many digits would follow.  Now we know, so adjust accordingly
            if (loMinIntDigCnt == -1)
                loMinIntDigCnt = 0;
            else
                loMinIntDigCnt = loMaxIntDigCnt - loMinIntDigCnt;

            // done analyzing the mask.  Lets strip the source string down to 
            // nothing but prominent digits, a decimal point, and a negative sign

            // advance past leading white space
            int iSrcStrIdx = 0;
            while (iSrcStr[iSrcStrIdx] == ' ')
                iSrcStrIdx++;

            // 1st char can be a negative sign.
            if (iSrcStr[iSrcStrIdx] == '-')
            {
                if (loNegative == 0)
                    return -4; // mask doesn't allow negatives
                // add negative sign to output
                oResult = oResult + iSrcStr[(iSrcStrIdx++)];
                loResLen++;
                loFirstNumIdx++;
            }

            // next char can be a dollar sign 
            if (iSrcStr.Length > iSrcStrIdx)
            {
                if ((iSrcStr[iSrcStrIdx] == '$') || (loCurrency != 0))
                {
                    if (iSrcStr[iSrcStrIdx] == '$')
                        iSrcStrIdx++;
                    oResult = oResult + '$';
                    loResLen++;
                    loFirstNumIdx++;
                }
            }

            // eliminate leading 0's and commas 
            while ((iSrcStrIdx < iSrcStr.Length) && ((iSrcStr[iSrcStrIdx] == ',') || (iSrcStr[iSrcStrIdx] == '0')))
            {
                // If we found a zero, then we know the number is not a null value
                if (iSrcStr[iSrcStrIdx] == '0')
                    loNumberIsBlank = 0;
                iSrcStrIdx++;
            }

            // copy remaining digits, eliminating commas on the way.
            for (; iSrcStrIdx < iSrcStr.Length; iSrcStrIdx++)
            {
                // copy any digits 
                char SourceChar = iSrcStr[iSrcStrIdx];
                if ((SourceChar >= '0') && (SourceChar <= '9'))
                {
                    // We now know entire numeric value isn't zero
                    loNumberIsZero = 0;
                    oResult = oResult + iSrcStr[iSrcStrIdx];
                    loResLen++;
                    continue;
                }

                // if this is a decimal point (but not the second), save its position 
                if (iSrcStr[iSrcStrIdx] == '.')
                {
                    // no decimal unless a fixed or floating point number 
                    if (loFixedPnt == 0 && loFloatPnt == 0)
                        break; // just truncate it here.
                    if (loSrcDecPos != -1)
                        return -3; // can't have two decimal points either
                    loSrcDecPos = loResLen;
                    oResult = oResult + iSrcStr[iSrcStrIdx];
                    loResLen++;
                    continue;
                }

                // if this is white space, stop
                if (iSrcStr[iSrcStrIdx] == ' ')
                    break;
                //if this is a comma, swallow it. 
                if (iSrcStr[iSrcStrIdx] == ',')
                    continue;
                // anything else, we are hosed 
                return -2;
            }

            int loPrevSrcDecPos = loSrcDecPos;
            // eliminate trailing 0's if after a decimal point 
            if (loSrcDecPos >= 0)
            {
                while (oResult[loResLen - 1] == '0')
                    loResLen--;
            }

            // don't let number end w/ a decimal point 
            if ((loResLen > 0) && (loSrcDecPos == (loResLen - 1)))
                loResLen--;

            // truncate to new length if necessary
            if (loResLen < oResult.Length)
                oResult = oResult.Remove(loResLen, oResult.Length - loResLen);

            // oResult holds the source string w/ commas & leading 0's stripped out.  
            // Now merge it w/ the mask. loResLen holds the length of oResult

            // if we are dealing w/ an empty number, return null string 
            if (loNumberIsZero != 0 && loNumberIsBlank != 0)
            {
                oResult = "";
                return 0;
            }

            // if the number is 0, need to stuff it in.  The trim 0's would have removed all 0's 
            if (loNumberIsZero != 0)
            {
                oResult = oResult + "0";
                loResLen++;
            }

            loSrcDecPos = oResult.IndexOf('.');

            if ((loSrcDecPos == -1) && (loMinFracDigCnt > 0))
            {
                // fixed point number needs a decimal point
                loSrcDecPos = Math.Max(loFirstNumIdx, loPrevSrcDecPos);
                oResult = oResult.Insert(loSrcDecPos, ".");
                loResLen++;

                // left pad w/ 0's fractional portion to minimum size
                while ((loResLen - 1 - loSrcDecPos) < loMinFracDigCnt)
                {
                    oResult = oResult.Insert(loSrcDecPos + 1, "0");
                    loResLen++;
                }
            }

            if (loSrcDecPos == -1)
                loSrcDecPos = loResLen;

            // remove excess digits 
            while (loSrcDecPos - loFirstNumIdx > loMaxIntDigCnt)
            {
                oResult = oResult.Remove(loFirstNumIdx, 1);
                loResLen--;
                loSrcDecPos--;
            }

            // remove unecessary leading 0's 
            while ((loSrcDecPos - loFirstNumIdx > loMinIntDigCnt) && (oResult[loFirstNumIdx] == '0'))
            {
                // Don't remove leading zeros is the number is zero and the length doesn't exceed 1
                if ((loNumberIsZero != 0) && (oResult.Length <= 1))
                    break;
                // This is from the original ported code, but does it make any sense?
                if (loNumberIsZero != 0 && (oResult[loFirstNumIdx + 1] != '0'))
                    break;
                oResult = oResult.Remove(loFirstNumIdx, 1);
                loResLen--;
                loSrcDecPos--;
            }

            // left pad w/ 0's integer portion to minimum size
            while (loSrcDecPos - loFirstNumIdx < loMinIntDigCnt)
            {
                oResult = oResult.Insert(loFirstNumIdx, "0");
                loResLen++;
                loSrcDecPos++;
            }

            // remove excess fractional portion
            if (loFixedPnt != 0)
            {
                while ((loResLen - 1 - loSrcDecPos) > loMaxFracDigCnt)
                {
                    oResult = oResult.Remove(oResult.Length - 1, 1);
                    loResLen--;
                }

                // right pad w/ 0's fractional portion to minimum size
                if (loMinFracDigCnt > 0)
                {
                    while ((loResLen - 1 - loSrcDecPos) < loMinFracDigCnt)
                    {
                        oResult = oResult + "0";
                        loResLen++;
                    }
                }

                if (loImpliedDecimalPt != 0)
                {
                    // remove the decimal point
                    oResult = oResult.Remove(loSrcDecPos, 1);
                    loResLen--;
                }
            }

            // finally, insert comma separaters
            if (loCommas != 0)
            {
                while (loSrcDecPos - loFirstNumIdx > 3)
                {
                    loSrcDecPos -= 3;
                    oResult = oResult.Insert(loSrcDecPos, ",");
                }
            }

            // all done
            return 0;
        }

        public static int StrToDouble(string iStr, ref double oDouble)
        {
            bool loIsNegative = false;
            int loDecimalPos = -1;
            int loDigitCnt = 0;
            int loNdx = 0;

            if (iStr == "")
                return 0;

            oDouble = 0;

            // Skip past leading spaces and other non-numerics 
            for (loNdx = 0; loNdx < iStr.Length; loNdx++)
            {
                if (iStr[loNdx] < '0' || iStr[loNdx] > '9')
                {
                    // deal w/ negative numbers 
                    if (iStr[loNdx] == '-')
                        loIsNegative = true;
                }
                else
                    break;
            }

            for (; loNdx < iStr.Length; loNdx++)
            {
                if (iStr[loNdx] == ',')
                    continue; // disregard commas

                if ((iStr[loNdx] == '.') && (loDecimalPos == -1))
                {
                    loDecimalPos = loDigitCnt;  // record the position of the decimal point
                    continue;
                }
                if ((iStr[loNdx] > '9') || (iStr[loNdx] < '0'))
                    break; // invalid char 
                loDigitCnt++;
                oDouble *= 10; // multiply existing value by 10... 
                oDouble += iStr[loNdx] - 0x30; // and add next digit
            }
            if (loIsNegative)
                oDouble *= -1;

            // now divide by 10 for each digit beyond the decimal point
            for (; (loDecimalPos >= 0) && (loDecimalPos < loDigitCnt); loDecimalPos++)
                oDouble /= 10;
            return 0;
        }

        public static int StrTollInt(string iStr, ref Int64 oInt)
        {
            bool loIsNegative = false;
            int loNdx = 0;

            if (iStr == "")
                return 0;

            oInt = 0;

            // Skip past leading spaces and other non-numerics 
            for (loNdx = 0; loNdx < iStr.Length; loNdx++)
            {
                if (iStr[loNdx] < '0' || iStr[loNdx] > '9')
                {
                    // deal w/ negative numbers 
                    if (iStr[loNdx] == '-')
                        loIsNegative = true;
                }
                else
                    break;
            }

            for (; loNdx < iStr.Length; loNdx++)
            {
                if (iStr[loNdx] == ',')
                    continue; // disregard commas

                if ((iStr[loNdx] > '9') || (iStr[loNdx] < '0'))
                    break; // invalid char 
                oInt *= 10; // multiply existing value by 10... 
                oInt += iStr[loNdx] - 0x30; // and add next digit
            }
            if (loIsNegative)
                oInt *= -1;

            return 0;
        }

        public static int MaskStrToDouble(string iStr, string iMask, ref double oDouble)
        {
            int loResult = StrToDouble(iStr, ref oDouble);
            if (loResult < 0)
                return loResult;

            // only mask we are worried about is "888...", in which case the value needs to be divided by 100.
            if (iMask.IndexOf("8") >= 0)
                oDouble /= 100;
            return loResult;
        }
        #endregion

        #region General / Helper Functions
        public void InitToDefaultState()
        {
            ClearEditStateAttrs();
            /* KLC
            if (this._EditCtrl != null)
                this._EditCtrl.Enabled = true;
             */ 
            if (this._CfgCtrl != null)
            {
                this._CfgCtrl.IsEnabled = true;
                this._CfgCtrl.IsProtected = false;
            }

            DependentNotification(EditRestrictionConsts.dneFormInit, null);
        }

        public TextBoxBehavior GetTextBoxBehaviorByName(string ControlName)
        {
            // Loop through all behaviors in our collection,
            // and return the matching TextBoxBehavior when found.
            int loLoopMax = this.BehaviorCollection.Count;
            for (int loIdx = 0; loIdx < loLoopMax; loIdx++)
            {
                if (this.BehaviorCollection[loIdx]._CfgCtrl._Name.Equals(ControlName))
                    return this.BehaviorCollection[loIdx] as TextBoxBehavior;
            }
            // Couldn't find a TextBoxBehavior with the passed name
            return null;
        }

        public void EmptyMsgQueue()
        {
            WinMessage msg;

            // specialized loop that retieves and discards all keyboard and mouse messages
            // MSHelp warns against testing only for pure boolean results,
            // because the result can be -1, 0, or non-zero
            const int WM_KEYFIRST = 0x0100;
            const int WM_KEYLAST = 0x0108;
            const int WM_MOUSEFIRST = 0x0200;
            const int WM_MOUSELAST = 0x020A;
            const int PM_REMOVE = 0x1;

            // Get platform dependant object we will use for calling a Windows API function
            IPlatformDependent WinAPI = GetWinAPI();
            while (WinAPI.PeekMessage(out msg, IntPtr.Zero, WM_KEYFIRST, WM_KEYLAST, PM_REMOVE) == true)
            {
                //loop until all the keyboard messages are removed
            }

            while (WinAPI.PeekMessage(out msg, IntPtr.Zero, WM_MOUSEFIRST, WM_MOUSELAST, PM_REMOVE) == true)
            {
                //loop until all the mouse messages are removed
            }
        }

        public void EmptyKeysFromMsgQueue()
        {
            WinMessage msg;

            // specialized loop that retieves and discards all keyboard messages
            // MSHelp warns against testing only for pure boolean results,
            // because the result can be -1, 0, or non-zero
            const int WM_KEYFIRST = 0x0100;
            const int WM_KEYLAST = 0x0108;
            const int PM_REMOVE = 0x1;

            // Get platform dependant object we will use for calling a Windows API function
            IPlatformDependent WinAPI = GetWinAPI();
            while (WinAPI.PeekMessage(out msg, IntPtr.Zero, WM_KEYFIRST, WM_KEYLAST, PM_REMOVE) == true)
            {
                //loop until all the keyboard messages are removed
            }
        }

        private void PaintEditCtrl()
        {
        /* KLC
            // Only applicable to a textbox control
            if (!(this._EditCtrl is ReinoControls.ReinoTextBox))
                return;

            // Set the control's underlying text if its different
            if (((ReinoTextBox)(this._EditCtrl)).BaseText != this._Text)
                ((ReinoTextBox)(this._EditCtrl)).BaseText = this._Text;
         */ 
        }

        public void RemoveListBoxes()
        {
        /* KLC
            this._PopupListBox = null;
            this._StaticListBox = null;
            this._RadioListBox = null;
         */
        }

        public static IPlatformDependent GetWinAPI()
        {
        /* KLC
            // Get platform dependant object we will use for calling a Windows API function
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
                return new WinCEAPI();
            else
            return new ReinoControls.Win32API();
         */
            return null;

        }
        #endregion
    }

    #region PlaySoundSupport class
    public class PlaySoundSupport
    {
        public int PreferredVolume = -1;
        public string SoundFile = "";

        private const int SND_SYNC = 0x0000;  // play synchronously (default) 
        private const int SND_ASYNC = 0x0001;  // play asynchronously 
        private const int SND_NODEFAULT = 0x0002;  // silence (!default) if sound not found 
        private const int SND_MEMORY = 0x0004;  // pszSound points to a memory file
        private const int SND_LOOP = 0x0008;  // loop the sound until next sndPlaySound 
        private const int SND_NOSTOP = 0x0010;  // don't stop any currently playing sound 
        private const int SND_NOWAIT = 0x00002000; // don't wait if the driver is busy 
        private const int SND_ALIAS = 0x00010000; // name is a registry alias 
        private const int SND_ALIAS_ID = 0x00110000; // alias is a predefined ID
        private const int SND_FILENAME = 0x00020000; // name is file name 
        private const int SND_RESOURCE = 0x00040004; // name is resource name or atom 

        public void RingBellThreaded(int iPreferredVolume)
        {
            // Use passed parameters
            this.PreferredVolume = iPreferredVolume;

            // Start a seperate thread to play the sound
            Thread SoundThread = new Thread(this.RingBell);
            SoundThread.Start();
        }

        public void RingBell()
        {
            // Get platform dependant object we will use for calling a Windows API function
            IPlatformDependent WinAPI = TextBoxBehavior.GetWinAPI();
            int SavedVolume = -1;

            // We need to set the volume if preferred volume is not -1
            if (PreferredVolume != -1)
            {
                // Save current volume, then set preferred volume
                int loVolumeResult = WinAPI.waveOutGetVolume(IntPtr.Zero, ref SavedVolume);
                if (loVolumeResult == 0)
                    WinAPI.waveOutSetVolume(IntPtr.Zero, PreferredVolume);
            }

            // Now call the PlaySound API function via our MessageBeep function
            WinAPI.MessageBeep();

            // If preferred volume was not -1, then restore the volume
            if (PreferredVolume != -1)
                WinAPI.waveOutSetVolume(IntPtr.Zero, SavedVolume);
        }

        public void StopPlaying()
        {
            // Get platform dependant object we will use for calling a Windows API function
            IPlatformDependent WinAPI = TextBoxBehavior.GetWinAPI();
            WinAPI.PlaySound(null, IntPtr.Zero, (SND_ASYNC | SND_FILENAME | SND_NODEFAULT | SND_NOWAIT));
        }

        public void PlaySoundThreaded(int iPreferredVolume, string iSoundFile)
        {
            // Use passed parameters
            this.PreferredVolume = iPreferredVolume;
            this.SoundFile = iSoundFile;

            // Start a seperate thread to play the sound
            Thread SoundThread = new Thread(this.PlaySound);
            SoundThread.Start();
        }

        public void PlaySound()
        {
            // Get platform dependant object we will use for calling a Windows API function
            IPlatformDependent WinAPI = TextBoxBehavior.GetWinAPI();
            int SavedVolume = -1;

            // We need to set the volume if preferred volume is not -1
            if (PreferredVolume != -1)
            {
                // Save current volume, then set preferred volume
                int loVolumeResult = WinAPI.waveOutGetVolume(IntPtr.Zero, ref SavedVolume);
                if (loVolumeResult == 0)
                    WinAPI.waveOutSetVolume(IntPtr.Zero, PreferredVolume);
            }

            // Now play the sound sychronously
            WinAPI.PlaySound(SoundFile, IntPtr.Zero, (SND_SYNC | SND_FILENAME | SND_NODEFAULT | SND_NOWAIT));

            // If preferred volume was not -1, then restore the volume
            if (PreferredVolume != -1)
                WinAPI.waveOutSetVolume(IntPtr.Zero, SavedVolume);
        }
    }
    #endregion
}
