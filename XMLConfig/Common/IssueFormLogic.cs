// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 12/04/13 8:15a $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Handheld/IssueFormLogic.cs $
//              Revision: $Revision: 70 $

#define DEBUG
#undef PRINTER_LOOP_TEST_FOR_STEVE_B // Define this to do an endless printer loop (Toggle with #undef or #define)
#undef DAILY_USAGE_SIMULATION // Define this to do automated volume testing (Toggle with #undef or #define)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
// KLC using System.Data;
using System.Diagnostics;
using System.Drawing;
// KLC using System.Drawing.Drawing2D;
using System.Text;
using System.Threading;
using System.IO;
// KLC using System.Windows.Forms;
using System.Text.RegularExpressions;
using Reino.ClientConfig;
using Reino.CommonLogic;
using System.Runtime.InteropServices;
using ReinoControls;

#if !WindowsCE && !__ANDROID__   
using System.Drawing.Printing; // Not available on Compact Framework
#endif

namespace Reino.CommonLogic
{
    #region IssFormSection
    public class IssFormSection
    {
        public TTControl FirstCfgCtrl = null;
        public int FirstCfgCtrlIdx = -1;
        public TTControl LastCfgCtrl = null;
        public int LastCfgCtrlIdx = -1;
    }
    #endregion

    #region DisplayPage
    public class DisplayPage
    {
        public List<TTControl> DisplayedFields = new List<TTControl>();
        public int FieldsRowCount = 0;
        public int ButtonsCount = 0;
        /// <summary>
        /// VioOccur has meaning only for violation-related fields
        /// </summary>
        public int VioOccur = 0;
        /// IsVioPage has meaning only for violation-related fields
        public bool IsVioPage = false;
    }
    #endregion

    #region FormEditResults
    public static class FormEditResults
    {
        // Base Form edit results. User can define these at will...
        public const int FormEditResult_Failed = -2;
        public const int FormEditResult_Cancelled = -1;
        public const int FormEditResult_OK = 0;
        public const int FormEditResult_BtnTwo = 1;
        public const int FormEditResult_BtnThree = 2;
        public const int FormEditResult_BtnFour = 3;
        public const int FormEditResult_Reswipe = 30;
    }
    #endregion

    #region Enumerations (FormLogicStyle, ListSearchType)
    internal enum FormLogicStyle { flsIssue, flsSelect };

    //internal enum ListSearchType { NewIfAgencyOr1stData, Get1st, Get2nd, NewIfAgencyOr2ndData };
    public enum ListSearchType { NewIfAgencyOr1stData, Get1st, Get2nd, NewIfAgencyOr2ndData };
    #endregion

    public partial class IssFormBuilder
    {
        #region Public/Private Members
        // A special const to indicate that no message should be displayed
        private const string cnPreventExitNoMessage = "!NOMSG";

        // Configuration references
        public TIssStruct IssueStruct = null;
        public Reino.ClientConfig.TBaseIssForm CfgForm = null;
        public Reino.ClientConfig.TTControl CurrentCfgCtrl = null;
//        public Reino.ClientConfig.TIssPrnFormRev PrintPicture = null;
        public Reino.ClientConfig.TIssStruct fSourceDataStruct = null;
        public Reino.ClientConfig.TIssForm CfgIssForm
        {
            get
            {
                if ((CfgForm != null) && (CfgForm is TIssForm))
                    return CfgForm as TIssForm;
                else
                    return null;
            }
        }
        public int CurrentCfgCtrlIdx = -1;

        // State attributes
        protected int fFormEditMode = Reino.ClientConfig.EditRestrictionConsts.femNewEntry;
        protected int fFormEditAttrs = 0;
        protected short fFormEditResult = 0;

        // Controls
        /*
        public IssueForm/*Form* / WinForm = null;
        public System.Windows.Forms.Panel EntryPanel;
        public System.Windows.Forms.Panel NavigationPanel;
        public System.Windows.Forms.Panel TitleBar;
        public System.Windows.Forms.Label TitleBarCaption;
        public string TitleBarRawCaption = "";
        public ReinoGlyphButton BtnPrevField;
        public ReinoGlyphButton BtnNextField;
        private System.Windows.Forms.Timer MainTimer;
        public ReinoControls.KeyEventHelper FieldNavHelper = null;
        public ReinoControls.ReinoVirtualListBox vlbFields = null;
        protected ReinoNavButton NavigateEscBtn;
        protected ReinoNavButton NavigateOKBtn;
        public ReinoNavButton EntryFieldsBtn;
        protected ReinoRadioList Popular = null;
        public System.Windows.Forms.Panel StartPanel = null;
        public TTControl StartField = null;
        public ReinoGlyphButton BtnNavAntenna;
        public ReinoGlyphButton BtnEntryAntenna;
        */
        // Control references
        internal Reino.ClientConfig.TTControl IssueDateCtrl = null;
        internal Reino.ClientConfig.TTControl IssueTimeCtrl = null;
        internal Reino.ClientConfig.TTControl IssueNoCtrl = null;
        internal Reino.ClientConfig.TTControl IssueNoDisplayCtrl = null;
        internal Reino.ClientConfig.TTControl IssueNoPfxCtrl = null;
        internal Reino.ClientConfig.TTControl IssueNoSfxCtrl = null;
        internal Reino.ClientConfig.TTControl ControlNoCtrl = null;
        internal Reino.ClientConfig.TTControl RecCreationDateCtrl = null;
        internal Reino.ClientConfig.TTControl RecCreationTimeCtrl = null;
        internal Reino.ClientConfig.TTControl EditPendingAttachments = null;
        internal Reino.ClientConfig.TTControl fDrawSuspSignature = null;
        internal Reino.ClientConfig.TTControl fDrawOfficerSignature = null;
        internal Reino.ClientConfig.TTControl VoidDateTimeCtrl = null;
        internal Reino.ClientConfig.TTControl ReIssuedByCtrl = null;
        /*
        internal ReinoControls.ReinoNavButton _LastBuiltSectionButton = null;
        internal ReinoControls.ReinoNavButton _ActiveSectionButton = null;
        internal ReinoControls.ReinoNavButton BtnDone = null;
        internal ReinoControls.ReinoNavButton BtnPrint = null;
        internal ReinoControls.ReinoNavButton BtnNotes = null;
        internal ReinoControls.ReinoNavButton BtnCorrection = null;
        internal ReinoControls.ReinoNavButton BtnIssueMore = null;
        internal ReinoControls.ReinoNavButton BtnIssueMultiple = null;
        internal ReinoControls.ReinoNavButton BtnEndIssueMultiple = null;
        internal ReinoControls.ReinoNavButton BtnCancel = null;
        internal ReinoControls.ReinoNavButton BtnVoid = null;
        internal ReinoControls.ReinoNavButton BtnReissue = null;
        internal ReinoControls.ReinoNavButton BtnSuspSignature = null;
        internal ReinoControls.ReinoNavButton BtnOfficerSignature = null;
        internal ReinoControls.ReinoNavButton BtnContinuance = null;
        internal ReinoControls.ReinoNavButton BtnNew = null;
        internal ReinoControls.ReinoNavButton BtnIssueChild = null;
        internal ReinoControls.ReinoNavButton BtnReadReino = null;
        */
        // Lists
        //public List<Reino.ClientConfig.TTControl> EntryOrder = new List<Reino.ClientConfig.TTControl>();
        //public List<ReinoNavButton> SectionButtons = null;
        //public List<DisplayPage> DisplayPages = new List<DisplayPage>();
        //public int CurrentDisplayPageIdx = -1;
        //private VScrollBar _SectionButtonsVScroll = null;
        //private int _PrevSectionButtonsVScrollValue = 0;
        //private bool _SuppressSectionButtonsVScrollValueChangedEvent = false;

        //public List<ReinoControls.TextBoxBehavior> BehaviorCollection = new List<TextBoxBehavior>();
        //public List<ReinoControls.ReinoRadioList> PopularLists = new List<ReinoRadioList>();
        //public List<Control> EntryPanelControls = new List<Control>();

        //public List<ReinoControls.BasicButton> DisplayedButtons = new List<BasicButton>();
        //public List<ReinoControls.ReinoTextBox> ReusableTextBoxes = new List<ReinoTextBox>();
        //public List<ReinoTextBox.StaticListBox> ReusableStaticListBoxes = new List<ReinoTextBox.StaticListBox>();
        //public List<ReinoRadioList> ReusableRadioListBoxes = new List<ReinoRadioList>();
        //public List<ReinoVirtualListBox> ReusableEditListBoxes = new List<ReinoVirtualListBox>();
        //public List<ReinoDrawBox> ReusableDrawBoxes = new List<ReinoDrawBox>();
        //public List<Label> ReusableLabels = new List<Label>();

        // General form-building variables
        //private int NextCtrlTop = 2;
        //private int NextBtnLeft = 4;
        //private int NextBtnTop = 28;
        //private int SectionBtnCnt = 0;
        //private int SectionBtnHeight = 20;
        //private int SectionBtnColumns = 3; //4; //3;
        //private int SectionBtnRows = 0;
        private int _SectionsInCfg = 0;

        // Misc variables
        internal FormLogicStyle LogicStyle = FormLogicStyle.flsIssue;
//        private bool PopularListHasPriority = false;
        // KLC private MultipleEntryMgr fMultipleEntryMgr = null;
        internal int fReissuedKey = 0;
        public bool fMagStripeUsed = false;
        public bool fUseSourceMagSwipe = false;

        public bool UsePSTRadComData = false;

        protected short fExitForm = 0;
        protected IssFormBuilder fSavedFocusForm = null;
//        private string fSavedCaption = "";
        internal bool fDidAutoAttachProcedure = false;
        internal bool fInsideAutoAttachProcedure = false;
        internal bool fSuppressInitMessageBox = false;
        public bool MustExecuteSetActiveCfgCtrl = false;
        //private bool _InsidePrepareForEdit = false;
        //private bool _MustRebuildDisplayAfterFormInit = false;
        //private bool _AlreadyDisplayedForm = false;

        // Windows Message constants
        private const int WM_SETREDRAW = 0x000B;
        private const int WM_USER = 0x400;
        private const int EM_GETEVENTMASK = (WM_USER + 59);
        private const int EM_SETEVENTMASK = (WM_USER + 69);

        // Fonts
        /*
        private System.Drawing.Font _Font10pt = null;
        private System.Drawing.Font _Font10ptBold = null;
        private System.Drawing.Font _Font8_25pt = null;
        private System.Drawing.Font _Font8_25ptBold = null;
        private System.Drawing.Font _Font14pt = null;
        private System.Drawing.Font _Font14ptBold = null;

        private System.Drawing.Font _Font10ptUnScaled = null;
        private System.Drawing.Font _Font10ptBoldUnScaled = null;
        private System.Drawing.Font _Font8_25ptUnScaled = null;
        private System.Drawing.Font _Font8_25ptBoldUnScaled = null;
        private System.Drawing.Font _Font14ptUnScaled = null;
        private System.Drawing.Font _Font14ptBoldUnScaled = null;
        */

        // Button Name constants
        protected const string BtnDoneName = "BTNDONE";
        protected const string BtnPrintName = "BTNPRINT";
        protected const string BtnNotesName = "BTNNOTES";
        protected const string BtnCorrectionName = "BTNCORRECTION";
        protected const string BtnIssueMoreName = "BTNISSUEMORE";
        protected const string BtnIssueMultipleName = "BTNISSUEMULTIPLE";
        protected const string BtnEndIssueMultipleName = "BTNENDISSUEMULTIPLE";
        protected const string BtnCancelName = "BTNCANCEL";
        protected const string BtnVoidName = "BTNVOID";
        protected const string BtnReissueName = "BTNREISSUE";
        protected const string BtnSuspSignatureName = "BTNSUSPSIGNATURE";
        protected const string BtnOfficerSignatureName = "BTNOFFICERSIGNATURE";
        protected const string BtnContinuanceName = "BTNCONTINUANCE";
        protected const string BtnNewName = "BTNNEW";
        protected const string BtnIssueChildName = "BTNISSUECHILD";
        protected const string BtnReadReinoName = "BTNREADREINO";
        protected const string BtnDiagram = "BtnDiagram";
        protected const string BitBtnDiagram = "BITBTNDIAGRAM";
        protected const string BtnVoiceNote = "BITBTNVOICENOTE";
        protected const string BtnPictureNote = "BITBTNPICTURENOTE";
        protected const string BtnVoiceNotePlayPause = "BITBTNVOICENOTEPLAYPAUSE";
        protected const string BtnVoiceNoteRecord = "BITBTNVOICENOTERECORD";
        protected const string BtnVoiceNoteStop = "BITBTNVOICENOTESTOP";
        protected const string BtnVoiceNoteFastForward = "BITBTNVOICENOTEFASTFORWARD";
        protected const string BtnVoiceNoteRewind = "BITBTNVOICENOTEREWIND";
        protected const string BtnVoiceNoteVolume = "BITBTNVOICENOTEVOLUME";
        protected const string BitBtnAttach = "BITBTNATTACH";
        protected const string BmpThumbnailPreview = "BMPTHUMBNAILPREVIEW";
        #endregion

        #region Public Properties
        #endregion

        #region Entry Form Creation

//        public void CreateLayoutFromConfig(IssueForm/*Form*/ ParentForm, ref Reino.ClientConfig.TClientDef pClientDef,
//                TIssStruct issueStruct)
//        {
//            // Keep reference to the passed objects
//            this.WinForm = ParentForm;
//            this.IssueStruct = issueStruct;

//            this.WinForm.SuspendLayout();

//            // Create set of fonts that we will use
//            _Font10pt = FontCache.CreateFontManaged("Tahoma", (10F * ReinoControls.BasicButton.ScaleFactorAsInt), System.Drawing.FontStyle.Regular);
//            _Font10ptBold = FontCache.CreateFontManaged("Tahoma", (10F * ReinoControls.BasicButton.ScaleFactorAsInt), System.Drawing.FontStyle.Bold);
//            _Font8_25pt = FontCache.CreateFontManaged("Tahoma", (8.25F * ReinoControls.BasicButton.ScaleFactorAsInt), System.Drawing.FontStyle.Regular);
//            _Font8_25ptBold = FontCache.CreateFontManaged("Tahoma", (8.25F * ReinoControls.BasicButton.ScaleFactorAsInt), System.Drawing.FontStyle.Bold);
//            _Font14pt = FontCache.CreateFontManaged("Tahoma", (14F * ReinoControls.BasicButton.ScaleFactorAsInt), System.Drawing.FontStyle.Regular);
//            _Font14ptBold = FontCache.CreateFontManaged("Tahoma", (14F * ReinoControls.BasicButton.ScaleFactorAsInt), System.Drawing.FontStyle.Bold);

//            _Font10ptUnScaled = FontCache.CreateFontManaged("Tahoma", (10F), System.Drawing.FontStyle.Regular);
//            _Font10ptBoldUnScaled = FontCache.CreateFontManaged("Tahoma", (10F), System.Drawing.FontStyle.Bold);
//            _Font8_25ptUnScaled = FontCache.CreateFontManaged("Tahoma", (8.25F), System.Drawing.FontStyle.Regular);
//            _Font8_25ptBoldUnScaled = FontCache.CreateFontManaged("Tahoma", (8.25F), System.Drawing.FontStyle.Bold);
//            _Font14ptUnScaled = FontCache.CreateFontManaged("Tahoma", (14F), System.Drawing.FontStyle.Regular);
//            _Font14ptBoldUnScaled = FontCache.CreateFontManaged("Tahoma", (14F), System.Drawing.FontStyle.Bold);

//            // Create TitleBar panel
//            this.TitleBar = new System.Windows.Forms.Panel();
//            this.TitleBar.Name = "pnlTitleBar";
//#if !WindowsCE && !__ANDROID__   
//            this.TitleBar.BorderStyle = System.Windows.Forms.BorderStyle.None;
//            this.TitleBar.Location = new System.Drawing.Point(0, 0); //(12, 77);
//#else
//            this.TitleBar.Location = new System.Drawing.Point(0, 0);
//#endif
//            this.TitleBar.Size = new System.Drawing.Size(240, 13);
//            this.TitleBar.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlTitleBar_Paint);

//            // Create caption label for title bar
//            TitleBarCaption = new Label();
//            TitleBarCaption.Parent = this.TitleBar;
//            TitleBarCaption.Font = _Font10ptBold;
//            TitleBarCaption.Text = "AutoISSUE";
//            TitleBarRawCaption = "AutoISSUE";
//            TitleBarCaption.Bounds = new Rectangle(4, -2, this.TitleBar.Width - 8, this.TitleBar.Height);
//            /*
//            TitleBarCaption.Location = new System.Drawing.Point(4, -2);
//            TitleBarCaption.Size = new System.Drawing.Size(this.TitleBar.Width - 8, this.TitleBar.Height);
//            */
//            TitleBarCaption.ForeColor = Color.White;
//            TitleBarCaption.BackColor = SystemColors.ControlDarkDark;

//            // Create EntryPanel
//            this.EntryPanel = new System.Windows.Forms.Panel();
//#if !WindowsCE && !__ANDROID__   
//            this.EntryPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
//            this.EntryPanel.Location = new System.Drawing.Point(0, 0 + TitleBar.Height); //(12, 77 + TitleBar.Height);
//#else
//            this.EntryPanel.Location = new System.Drawing.Point(0, 0 + TitleBar.Height);
//#endif
//            this.EntryPanel.Name = "pnlEntry";
//            //this.EntryPanel.Size = new System.Drawing.Size(240, 240 - TitleBar.Height);
//            this.EntryPanel.Size = new System.Drawing.Size(ParentForm.Width, ParentForm.Height - TitleBar.Height);
//            this.EntryPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlEntry_Paint);

//            // Create NavigationPanel
//            this.NavigationPanel = new System.Windows.Forms.Panel();
//#if !WindowsCE && !__ANDROID__   
//            this.NavigationPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
//            this.NavigationPanel.Location = new System.Drawing.Point(0, 0 + TitleBar.Height); //(12, 77 + TitleBar.Height);
//#else
//            this.NavigationPanel.Location = new System.Drawing.Point(0, 0 + TitleBar.Height);
//#endif
//            this.NavigationPanel.Name = "NavigationPanel";
//            this.NavigationPanel.Size = new System.Drawing.Size(this.WinForm.Width, this.WinForm.Height - TitleBar.Height);
//            this.NavigationPanel.Size = new System.Drawing.Size(ParentForm.Width, ParentForm.Height - TitleBar.Height);
//            this.NavigationPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.NavigationPanel_Paint);

//            // Create PREV field button
//            BtnPrevField = new ReinoGlyphButton();
//            BtnPrevField.ResizeSuspended = true;
//            BtnPrevField.Bounds = new Rectangle(123 /*+ 30*/, 3, 69, 20);
//            /*
//            BtnPrevField.Location = new System.Drawing.Point(123 + 30, 3);
//            BtnPrevField.Size = new System.Drawing.Size(69, 20);
//            */
//            BtnPrevField.GlyphType = ReinoButtonGlyphs.bgLeft;
//            BtnPrevField.Text = "PREV";
//            BtnPrevField.Font = _Font10ptBold;
//#if !WindowsCE && !__ANDROID__   
//            BtnPrevField.CausesValidation = false;
//#endif
//            BtnPrevField.MouseDown += new MouseEventHandler(BtnPrevField_Click); // Used to be click event

//            // Create NEXT field button
//            BtnNextField = new ReinoGlyphButton();
//            BtnNextField.ResizeSuspended = true;
//            BtnNextField.Bounds = new Rectangle(BtnPrevField.Left + BtnPrevField.Width, 3, 69, 20);
//            /*
//            BtnNextField.Location = new System.Drawing.Point(BtnPrevField.Left + BtnPrevField.Width, 3);
//            BtnNextField.Size = new System.Drawing.Size(69, 20);
//            */
//            BtnNextField.GlyphType = ReinoButtonGlyphs.bgRight;
//            BtnNextField.GlyphSide = ReinoGlyphSide.gsRight;
//            BtnNextField.Text = "NEXT";
//            BtnNextField.Font = _Font10ptBold;
//            BtnNextField.MouseDown += new MouseEventHandler(BtnNextField_Click); // Used to be click event

//            // Create Antenna button for entry panel
//            BtnEntryAntenna = new ReinoGlyphButton();
//            BtnEntryAntenna.ResizeSuspended = true;
//            BtnEntryAntenna.Bounds = new Rectangle(240 - 24, 3, 18, 20);
//            /*
//            BtnEntryAntenna.Location = new System.Drawing.Point(240 - 24, 3);
//            BtnEntryAntenna.Size = new System.Drawing.Size(18, 20);
//            */
//            BtnEntryAntenna.Visible = false;
//            BtnEntryAntenna.GlyphType = ReinoButtonGlyphs.bgAntenna;
//            BtnEntryAntenna.GlyphSide = ReinoGlyphSide.gsLeft;
//            BtnEntryAntenna.GlyphYOffset = 3;
//            BtnEntryAntenna.Text = "";
//            BtnEntryAntenna.MouseDown += new MouseEventHandler(BtnAntenna_Click); // Used to be click event

//            // Create Antenna button for navigation panel
//            BtnNavAntenna = new ReinoGlyphButton();
//            BtnNavAntenna.ResizeSuspended = true;
//            BtnNavAntenna.Bounds = new Rectangle(240 - 24, 3, 18, 20);
//            /*
//            BtnNavAntenna.Location = new System.Drawing.Point(240 - 24, 3);
//            BtnNavAntenna.Size = new System.Drawing.Size(18, 20);
//            */
//            BtnNavAntenna.Visible = false;
//            BtnNavAntenna.GlyphType = ReinoButtonGlyphs.bgAntenna;
//            BtnNavAntenna.GlyphSide = ReinoGlyphSide.gsLeft;
//            BtnNavAntenna.GlyphYOffset = 3;
//            BtnNavAntenna.Text = "";
//            BtnNavAntenna.MouseDown += new MouseEventHandler(BtnAntenna_Click); // Used to be click event

//            // Create MainTimer
//            this.MainTimer = new System.Windows.Forms.Timer();
//            this.MainTimer.Enabled = false; // true;
//            this.MainTimer.Interval = 1000; // 500;
//            this.MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);

//            // Add controls to the form
//            this.EntryPanel.Parent = this.WinForm;
//            this.NavigationPanel.Parent = this.WinForm;
//            this.TitleBar.Parent = this.WinForm;

//            // Add buttons to the entry panel
//            BtnPrevField.Parent = EntryPanel;
//            /*this.EntryPanelControls.Add(BtnPrevField);*/
//            // Don't want non-config items in the list
//            BtnNextField.Parent = EntryPanel;
//            /*this.EntryPanelControls.Add(BtnNextField);*/
//            // Don't want non-config items in the list

//            // Add antenna button to the entrypanel
//            BtnEntryAntenna.Parent = EntryPanel;
//            /*this.EntryPanelControls.Add(BtnEntryAntenna);*/
//            // Don't want non-config items in the list
//            BtnEntryAntenna.BringToFront();

//            // Create a control that will be sized so small that its not visible.
//            // This will be used to handle keyboard navigation when the current field 
//            // is not capable of being focused (ie. its protected/disabled/hidden, etc.)
//            FieldNavHelper = new ReinoControls.KeyEventHelper();
//            FieldNavHelper.Size = new System.Drawing.Size(0, 0);
//            FieldNavHelper.OnNavigationKeyPress += new ReinoControls.KeyEventHelper.NavigationKeyPress(FieldNavHelper_OnNavKeyPress);
//            FieldNavHelper.KeyPress += new KeyPressEventHandler(FieldNavHelper_KeyPress);
//            FieldNavHelper.KeyDown += new KeyEventHandler(FieldNavHelper_KeyDown);
//            FieldNavHelper.Parent = this.WinForm;
//            FieldNavHelper.Enabled = true;

//            // Build edit controls based on the issuance form's configuration
//            BuildControlsFromCfgContainer(this.CfgForm.Controls);

//            // After all of the form's controls are built, we need to resolve object references
//            ResolveObjReferences(this.CfgForm);

//            // By default, the entry panel will be the only visible panel
//            this.HideNavigationPanel();
//            EntryPanel.Visible = true;
//            EntryPanel.BringToFront();

//            // Of course the title bar will be visible too
//            TitleBar.Visible = true;

//            // Create "Escape" button for Entry panel
//            EntryFieldsBtn = new ReinoNavButton();
//            EntryFieldsBtn.ResizeSuspended = true;
//            EntryFieldsBtn.Parent = EntryPanel;
//            /*this.EntryPanelControls.Add(EntryFieldsBtn);*/
//            // Don't want non-config items in the list
//            EntryFieldsBtn.Text = "Review"; // "Navigate"; // "Menu"; 
//            EntryFieldsBtn.Bounds = new Rectangle(4, 4, 104, SectionBtnHeight - 3);
//            /*
//            EntryFieldsBtn.Size = new System.Drawing.Size(104, SectionBtnHeight - 3);
//            EntryFieldsBtn.Location = new System.Drawing.Point(4, 4);
//            */
//            EntryFieldsBtn.Font = _Font8_25pt;
//            EntryFieldsBtn.BackColor = Color.Red;
//            EntryFieldsBtn.ForeColor = Color.White;
//            if (FontCache.UseGdiFonts == false)
//            {
//                EntryFieldsBtn.TextXOffset = 1;
//                EntryFieldsBtn.TextYOffset = 1;
//            }
//            EntryFieldsBtn.MouseDown += new MouseEventHandler(EntryFieldsBtn_Click);
//            EntryFieldsBtn.KeyDown += new KeyEventHandler(EntryFieldsBtn_KeyDown);
//            EntryFieldsBtn.KeyPress += new KeyPressEventHandler(EntryFieldsBtn_KeyPress);
//#if !WindowsCE && !__ANDROID__   
//            EntryFieldsBtn.OnIsInputKey += new ReinoNavButton.IsInputKeyEvent(EntryFieldsBtn_OnIsInputKey);
//            EntryFieldsBtn.CausesValidation = false;
//#endif
//            // Create "Escape" button for Navigation panel
//            NavigateEscBtn = new ReinoNavButton();
//            NavigateEscBtn.ResizeSuspended = true;
//            NavigateEscBtn.Parent = this.NavigationPanel;
//            NavigateEscBtn.Text = "Exit";
//            NavigateEscBtn.Bounds = new Rectangle(4, 4, 50, SectionBtnHeight - 3);
//            /*
//            NavigateEscBtn.Size = new System.Drawing.Size(50, SectionBtnHeight - 3);
//            NavigateEscBtn.Location = new System.Drawing.Point(4, 4);
//            */
//            NavigateEscBtn.Font = _Font8_25pt;
//            NavigateEscBtn.BackColor = Color.Red;
//            NavigateEscBtn.ForeColor = Color.White;
//            if (FontCache.UseGdiFonts == false)
//            {
//                NavigateEscBtn.TextXOffset = 1;
//                NavigateEscBtn.TextYOffset = 1;
//            }
//            NavigateEscBtn.MouseDown += new MouseEventHandler(NavigateEscBtn_Click);
//            NavigateEscBtn.KeyDown += new KeyEventHandler(NavigateEscBtn_KeyDown);
//#if !WindowsCE && !__ANDROID__   
//            NavigateEscBtn.OnIsInputKey += new ReinoNavButton.IsInputKeyEvent(NavigateEscBtn_OnIsInputKey);
//#endif
//            // Create "OK" button
//            NavigateOKBtn = new ReinoNavButton();
//            NavigateOKBtn.ResizeSuspended = true;
//            NavigateOKBtn.Parent = this.NavigationPanel;
//            NavigateOKBtn.Text = "Edit"; // "OK";
//            NavigateOKBtn.Bounds = new Rectangle(NavigateEscBtn.Left + NavigateEscBtn.Width + 4, 4, 50, SectionBtnHeight - 3);
//            /*
//            NavigateOKBtn.Size = new System.Drawing.Size(50, SectionBtnHeight - 3);
//            NavigateOKBtn.Location = new System.Drawing.Point(NavigateEscBtn.Left + NavigateEscBtn.Width + 4, 4);
//            */
//            NavigateOKBtn.Font = _Font8_25pt;
//            if (FontCache.UseGdiFonts == false)
//            {
//                NavigateOKBtn.TextXOffset = 2;
//                NavigateOKBtn.TextYOffset = 1;
//            }
//            NavigateOKBtn.MouseDown += new MouseEventHandler(NavigateOKBtn_Click);
//            NavigateOKBtn.KeyDown += new KeyEventHandler(NavigateOKBtn_KeyDown);

//            Label NavHeader = new Label();
//            NavHeader.Parent = this.NavigationPanel;
//            NavHeader.Font = _Font10ptBold;
//            NavHeader.Text = "Select Field to Edit/View";
//            NavHeader.Size = new System.Drawing.Size(this.NavigationPanel.Width - NavHeader.Left - 4, SectionBtnHeight - 4);
//            NavHeader.Location = new System.Drawing.Point(NavigateOKBtn.Left + NavigateOKBtn.Width + 8, 4);
//            NavHeader.ForeColor = Color.White;
//            NavHeader.BackColor = SystemColors.ControlDark;

//            // Prepare for building navigation buttons for each section of the ticket
//            NextBtnLeft = 4;
//            NextBtnTop = 28;
//            SectionBtnCnt = 0;
//            SectionButtons = new List<ReinoNavButton>();

//            // Determine how many section buttons will need to be built.
//            _SectionsInCfg = 0;
//            this.CountSectionsInCfg(CfgForm.Controls);
//            // 10 or more sections: use 5 columns per row
//            // 7 or more sections: use 4 columns per row
//            // otherwise we can get away with just 3 columns per row
//            if (_SectionsInCfg >= 10)
//                this.SectionBtnColumns = 4; /*5*/ // 4 Columns at most since we might need a Vertical ScrollBar
//            else if (_SectionsInCfg >= 7)
//                this.SectionBtnColumns = 4;
//            else
//                this.SectionBtnColumns = 3;

//            this.SectionBtnColumns = 3;

//            // Count how many rows of section buttons we will end up with
//            int loRowCounter = _SectionsInCfg;
//            this.SectionBtnRows = 0;
//            while (loRowCounter > 0)
//            {
//                this.SectionBtnRows++;
//                loRowCounter -= this.SectionBtnColumns;
//            }

//            // Do we need a vertical scrollbar? (More than 3 rows will use scrollbar)
//            const int _ScrollbarRowThreshold = 2;
//            if (this.SectionBtnRows > _ScrollbarRowThreshold) 
//            {
//                const int _VSrollExtraHeight = 2;
//                // Create a ScrollBar instance
//                _SectionButtonsVScroll = new VScrollBar();
//                _SectionButtonsVScroll.ValueChanged += new EventHandler(SectionButtonsVScroll_ValueChanged);
//                _SectionButtonsVScroll.Value = 0;
//                _SectionButtonsVScroll.Parent = this.NavigationPanel;
//                _SectionButtonsVScroll.LargeChange = 1;
//                _SectionButtonsVScroll.SmallChange = 1;
//                _SectionButtonsVScroll.Minimum = 0;
//                _SectionButtonsVScroll.Maximum = this.SectionBtnRows - 1;
//                _SectionButtonsVScroll.Height = (SectionBtnHeight * _ScrollbarRowThreshold) + 1 + _VSrollExtraHeight;
//                _SectionButtonsVScroll.Top = NextBtnTop - _VSrollExtraHeight;
//#if WindowsCE || __ANDROID__
//                _SectionButtonsVScroll.Left = (this.NavigationPanel.Width - 4) - _SectionButtonsVScroll.Width;
//#else
//                _SectionButtonsVScroll.Left = (this.NavigationPanel.Width - 4) - _SectionButtonsVScroll.Width - 2;
//#endif
//                _SectionButtonsVScroll.Visible = true;
//                this.NavigationPanel.Controls.Add(_SectionButtonsVScroll);
//                _PrevSectionButtonsVScrollValue = 0;
//                _SuppressSectionButtonsVScrollValueChangedEvent = false;
//            }

//            // Build a navigation button for each section of the ticket
//            AddSectionButtons(CfgForm.Controls);

//            // Now lets make sure that no buttons that start a new section are
//            // designated to share row with something from a different section.
//            // Also do similar for last button in sections.
//            foreach (ReinoNavButton NextBtn in SectionButtons)
//            {
//                Reino.CommonLogic.IssFormSection section;
//                section = NextBtn.Tag as Reino.CommonLogic.IssFormSection;
//                if ((section.FirstCfgCtrl != null) && (section.FirstCfgCtrl is TTButton))
//                {
//                    if ((section.FirstCfgCtrl as TTButton).SharesRowWithIdx < section.FirstCfgCtrlIdx)
//                        (section.FirstCfgCtrl as TTButton).SharesRowWithIdx = -1;
//                }
//                if ((section.LastCfgCtrl != null) && (section.LastCfgCtrl is TTButton))
//                {
//                    if ((section.LastCfgCtrl as TTButton).SharesRowWithIdx > section.LastCfgCtrlIdx)
//                        (section.LastCfgCtrl as TTButton).SharesRowWithIdx = -1;
//                }

//                // Initially start out with the buttons active but not physically showing. This is accomplished
//                // by setting "visible" = true, but resizing to a zero height.
//                NextBtn.Visible = true;
//                NextBtn.Height = 0;
//                NextBtn.PrepForPaint(true);
//                // Offset the initial verticle position
//                NextBtn.Top = 28 + (SectionBtnHeight * 2 /* 3 */) + (1 * 3);
//            }

//            // Adjust next component position if we created section buttons
//            /*
//            if (SectionButtons.Count > 0)
//                NextBtnTop += SectionBtnHeight + 1;
//            */

//            // Create a listbox of all fields in the ticket
//            vlbFields = new ReinoControls.ReinoVirtualListBox();
//            vlbFields.BeginUpdate();
//            vlbFields.Bounds = new Rectangle(4, NextBtnTop, NavigationPanel.Width - 8,
//                NavigationPanel.Height - NextBtnTop /*- 4 - 24*/ - (IssueAppImp.glMainToolBar.Height / ReinoControls.BasicButton.ScaleFactorAsInt));
//            //vlbFields.Size = new System.Drawing.Size(NavigationPanel.Width - 8, NavigationPanel.Height - NextBtnTop - 4 /*8*/ - 24);
//            //vlbFields.Location = new System.Drawing.Point(4, NextBtnTop);
//            vlbFields.Parent = this.NavigationPanel;
//            vlbFields.BringToFront();
//            vlbFields.Font = FontCache.CreateFontManaged("Tahoma", (8.25F * ReinoControls.BasicButton.ScaleFactorAsInt), System.Drawing.FontStyle.Regular);
//            vlbFields.GetVirtualTextForIndex += new ReinoControls.GetVirtualTextEventHandlder(FieldListBox_GetVirtualTextForIndex);
//            vlbFields.DrawItem += new ReinoControls.DrawItemEventHandler(FieldListBox_DrawItem);
//            vlbFields.PaintBackground += new PaintBackgroundEventHandler(FieldListBox_PaintBackground);
//            vlbFields.DoubleClick += new EventHandler(FieldListBox_DoubleClick);
//            vlbFields.TrapAllNavigationKeys = true; // We want to know about TAB, ESC, etc...
//            vlbFields.OnNavigationKeyPress += new ReinoVirtualListBox.NavigationKeyPress(FieldListBox_OnNavigationKeyPress);
//            vlbFields.KeyDown += new KeyEventHandler(FieldListBox_KeyDown);
//            vlbFields.KeyPress += new KeyPressEventHandler(FieldListBox_KeyPress);
//            vlbFields.GotFocus += new EventHandler(FieldListBox_GotFocus);
//            vlbFields.LostFocus += new EventHandler(FieldListBox_LostFocus);
//            vlbFields.Count = EntryOrder.Count;
//            vlbFields.SelectedIndexChanged += new EventHandler(FieldListBox_SelectedIndexChanged);
//            vlbFields.OnMeasureItem += new MeasureItemEventHandler(FieldListBox_OnMeasureItem);
//            vlbFields.OnSharesRowWithIndex += new SharesRowWithIndexEventHandler(FieldListBox_OnSharesRowWithIndex);
//            vlbFields.OnItemMouseDown += new MouseEventHandler(FieldListBox_OnItemMouseDown);

//            // Add antenna button to the navigation panel
//            BtnNavAntenna.Parent = NavigationPanel;
//            BtnNavAntenna.BringToFront();

//            // Set form edit mode for new entry
//            this.fFormEditMode = EditRestrictionConsts.femNewEntry;

//            // Verify the integrity of the sequence file and also create a recovery file as needed
//            if (this.IssueStruct is TCiteStruct)
//            {
//                // Make sure our sequence's LastIssuedNumber agrees with what is stored locally
//                AssocCiteStructLogic.ValidateSequenceLastIssued();
//                AssocCiteStructLogic.InitTempRecoveryFile();
//            }

//            // Set the starting edit record number
//            AssocIssueStructLogic.SetEditRecNo(IssueStruct.MainTable.GetRecCount());

//            // Set appropriate panel to show when the form is shown
//            StartPanel = this.NavigationPanel;
//            if (this.IssueStruct is TVoidStruct)
//                StartPanel = this.EntryPanel;
//            StartField = null;

//            this.WinForm.ResumeLayout();

//            this.WinForm.Scale(ReinoControls.BasicButton.ScaleFactorSize);
//            SectionBtnHeight = (20 * ReinoControls.BasicButton.ScaleFactorAsInt);

//            NavHeader.Font = ParentForm.Font;
//            TitleBarCaption.Font = ParentForm.Font;
//            //NavHeader.Font = _Font10ptBold;
//            //TitleBarCaption.Font = _Font10ptBold;
//        }

        public void BuildControlsFromCfgContainer(System.Collections.Generic.IList<TWinClass> Container)
        {
            // Increment the list capacity for the number of objects we know about
           // this.BehaviorCollection.Capacity = this.BehaviorCollection.Capacity + Container.Count;
            //this.EntryOrder.Capacity = this.EntryOrder.Capacity + Container.Count;

            foreach (Reino.ClientConfig.TWinClass NextCtrl in Container)
            {
                // Is it a "TabSheet" container?
                if (NextCtrl is Reino.ClientConfig.TTTabSheet)
                    BuildControlsFromCfgTabSheet((NextCtrl as Reino.ClientConfig.TTTabSheet).Sheets); // Recursive

                // Is it a TTPanel or descendant? (TTTabSheet, TPanel, TSheet)
                if (NextCtrl is Reino.ClientConfig.TTPanel)
                    BuildControlsFromCfgContainer((NextCtrl as Reino.ClientConfig.TTPanel).Controls); // Recursive

                // Is it a TTEdit or descendant? (TNumEdit, TIssEdit, TTMemo, TEditListBox)
                if (NextCtrl is Reino.ClientConfig.TTEdit)
                    BuildEditCtrlFromCfg(NextCtrl as Reino.ClientConfig.TTEdit); // Specific
                    /*
                // Is it a TTButton?
                if (NextCtrl is Reino.ClientConfig.TTButton)
                    BuildActionBtnFromCfg(NextCtrl as Reino.ClientConfig.TTButton); // Specific

                // Is it a TTBitmap?
                if (NextCtrl is Reino.ClientConfig.TTBitmap)
                    BuildBitmapFromCfg(NextCtrl as Reino.ClientConfig.TTBitmap); //Specific
                     */ 
            }
        }

        private void BuildControlsFromCfgTabSheet(System.Collections.Generic.IList<TSheet> Container)
        {
            // Increment the list capacity for the number of objects we know about
            //this.BehaviorCollection.Capacity = this.BehaviorCollection.Capacity + Container.Count;
            //this.EntryOrder.Capacity = this.EntryOrder.Capacity + Container.Count;

            foreach (Reino.ClientConfig.TSheet NextCtrl in Container)
            {
                // Is it a TTPanel or descendant? (It certainly should be since TSheet inherits from TTPanel)
                if (NextCtrl is Reino.ClientConfig.TTPanel)
                    BuildControlsFromCfgContainer((NextCtrl as Reino.ClientConfig.TTPanel).Controls); // recursive
            }
        }

        private void BuildEditCtrlFromCfg(Reino.ClientConfig.TTEdit pCtrl)
        {
            // Add this configuration control to the EntryOrder list for easier navigation
//            EntryOrder.Add(pCtrl);

            // If its a TEditListBox then we're building a listbox instead of a textbox
            if (pCtrl is TEditListBox)
            {
                //BuildListBoxFromCfg(pCtrl as TEditListBox);
                return;
            }

            // If its a TDrawField then we're building a ReinoDrawBox instead of a textbox
            if (pCtrl is TDrawField)
            {
  //              BuildDrawBoxFromCfg(pCtrl as TDrawField);
                return;
            }

            // First, lets see if its something we want to flag as a special fieldname or category
    //        SetKnownFieldCategory(pCtrl);

            // Create TextBoxBehavior object and set default attributes
            ReinoControls.TextBoxBehavior Behavior = new TextBoxBehavior();
            Behavior.ListStyle = ReinoControls.ListBoxStyle.lbsStatic;
      //      Behavior.NormalFont = _Font10ptUnScaled;
        //    Behavior.FocusedFont = _Font10ptBoldUnScaled;
            pCtrl.IsEnabled = true;

            // Set cross-ref links between new behavior and configuration object.
            // Set link so behavior can find other behaviors for this form.
            // Add behavior to list of behaviors for this form.
            //Behavior.CfgCtrl = pCtrl;
            //pCtrl.Behavior = Behavior;
            //Behavior.BehaviorCollection = this.BehaviorCollection;
            //BehaviorCollection.Add(Behavior);

            //// We need to add special events to the ReinoTextBox's behavior so the ReinoTextBox 
            //// code is more portable and doesn't rely on application specific stuff...
            //Behavior.OnFindNextFormControl += this.FindNextFormControl;
            //Behavior.OnWhichControlIsFirst += this.WhichControlIsFirst;
            //Behavior.OnGetFocusedControl += this.GetFocusedControl;
            //Behavior.OnGetFormEditMode += this.GetFormEditMode;
            //Behavior.OnGetFormEditAttrs += this.GetFormEditAttrs;
            //Behavior.OnSetFormEditAttr += this.SetFormEditAttr;
            //Behavior.OnTabForward += this.TabForward;
            //Behavior.OnTabBackward += this.TabBackward;
            //Behavior.OnNavigationKeyPress += new TextBoxBehavior.NavigationKeyPress(Field_OnNavigationKeyPress);
            //Behavior.OnRegularKeyPress += new TextBoxBehavior.RegularKeyPress(Field_OnRegularKeyPress);
            //Behavior.TextChanged += new EventHandler(Behavior_TextChanged);
            //Behavior.NotifiedDependentsParentDataChanged += new EventHandler(Behavior_NotifiedDependentsParentDataChanged);
            //Behavior.OnCreatedPopupListBox += new TextBoxBehavior.CreatedPopupListBox(Behavior_OnCreatedPopupListBox);
            //Behavior.OnCreatedRegularListBox += new TextBoxBehavior.CreatedRegularListBox(Behavior_OnCreatedRegularListBox);
            //Behavior.OnGetNewRegularListBox += new TextBoxBehavior.GetNewRegularListBox(Behavior_OnGetNewRegularListBox);
            //Behavior.OnCtrlGotFocus += new TextBoxBehavior.CtrlGotFocus(Behavior_OnCtrlGotFocus);
            //Behavior.OnGetRingBellVolume += this.GetRingBellVolume;

            //// For both safety and speed, lets get an uppercase version of cfg object's name for string comparison
            //string loCfgObjName = pCtrl.Name.ToUpper();

            //// Is this the magical IssueDate or IssueTime field?
            //if (loCfgObjName.Equals(FieldNames.IssueDateFieldName))
            //    IssueDateCtrl = pCtrl;
            //else if (loCfgObjName.Equals(FieldNames.IssueTimeFieldName))
            //    IssueTimeCtrl = pCtrl;
            //else if (loCfgObjName.Equals(FieldNames.IssueNoFieldName))
            //    IssueNoCtrl = pCtrl;
            //else if (loCfgObjName.Equals(FieldNames.IssueNoDisplayFieldName))
            //    IssueNoDisplayCtrl = pCtrl;
            //else if (loCfgObjName.Equals(FieldNames.ControlNoFieldName))
            //    ControlNoCtrl = pCtrl;
            //else if (loCfgObjName.Equals(FieldNames.IssueNoPfxFieldName))
            //    IssueNoPfxCtrl = pCtrl;
            //else if (loCfgObjName.Equals(FieldNames.IssueNoSfxFieldName))
            //    IssueNoSfxCtrl = pCtrl;
            //else if (loCfgObjName.Equals(FieldNames.RecCreationDateFieldName))
            //    RecCreationDateCtrl = pCtrl;
            //else if (loCfgObjName.Equals(FieldNames.RecCreationTimeFieldName))
            //    RecCreationTimeCtrl = pCtrl;
            //else if (loCfgObjName.Equals(FieldNames.PendingAttachmentsFieldName))
            //    EditPendingAttachments = pCtrl;
            //else if (loCfgObjName.Equals(FieldNames.SuspSignatureFieldName))
            //    fDrawSuspSignature = pCtrl;
            //else if (loCfgObjName.Equals(FieldNames.OfficerSignatureFieldName))
            //    fDrawOfficerSignature = pCtrl;
            //else if (loCfgObjName.Equals("VOIDDATETIME"))
            //    VoidDateTimeCtrl = pCtrl;
            //else if (loCfgObjName.Equals("REISSUED_BY"))
            //    ReIssuedByCtrl = pCtrl;

            //// We want to eliminate the PromptWin for certain fields
            //if ((loCfgObjName.StartsWith(FieldNames.VioDesc1FieldName)) ||
            //    (loCfgObjName.StartsWith(FieldNames.VioCodeFieldName)) ||
            //    (loCfgObjName.StartsWith(FieldNames.VioFineFieldName)))
            //{
            //    pCtrl.PreventPrompt = true;
            //}

            //// Copy applicable properties from the configuration object to the ReinoTextBox's Behavior object.

            //// A TNumEdit should always be efNumeric
            //if (pCtrl is Reino.ClientConfig.TNumEdit)
            //    Behavior.SetFieldType(ReinoControls.TEditFieldType.efNumeric);
            //else
            //{
            //    // We must convert the ClientConfig field type to the ReinoTextBox field type
            //    if (pCtrl is Reino.ClientConfig.TEditField)
            //    {
            //        if ((pCtrl as TEditField).FieldType == TEditField.TEditFieldType.efDate)
            //            Behavior.SetFieldType(ReinoControls.TEditFieldType.efDate);
            //        else if ((pCtrl as TEditField).FieldType == TEditField.TEditFieldType.efTime)
            //            Behavior.SetFieldType(ReinoControls.TEditFieldType.efTime);
            //        else if ((pCtrl as TEditField).FieldType == TEditField.TEditFieldType.efString)
            //            Behavior.SetFieldType(ReinoControls.TEditFieldType.efString);
            //    }
            //}

            //// Copy the edit mask and maxlength properties
            //if ((pCtrl is Reino.ClientConfig.TEditField) && ((pCtrl as TEditField).EditMask != ""))
            //    Behavior.SetEditMask((pCtrl as TEditField).EditMask);
            //Behavior.MaxLength = pCtrl.MaxLength;

            //// If a ListName is specified, we have a little bit of work to do...
            //if (pCtrl is Reino.ClientConfig.TEditField)
            //{
            //    if ((pCtrl as TEditField).ListName != "")
            //    {
            //        // Try to split the ListName property into two elements that together
            //        // specify a Table and Column for the list's source.
            //        string[] ListNameElements = (pCtrl as TEditField).ListName.Split(' ');
            //        if (ListNameElements.Length == 2)
            //        {
            //            // Set the behavior's list source properties (both natural and active list references)
            //            if (this is IssueSelectFormLogic)
            //                Behavior.NaturalListSourceTable = GetListTableByName(ListNameElements[0], ListSearchType.NewIfAgencyOr2ndData);
            //            else
            //                Behavior.NaturalListSourceTable = GetListTableByName(ListNameElements[0], ListSearchType.NewIfAgencyOr1stData);
            //            Behavior.NaturalListSourceFieldName = ListNameElements[1];
            //            Behavior.ListSourceTable = Behavior.NaturalListSourceTable;
            //            Behavior.ListSourceFieldName = Behavior.NaturalListSourceFieldName;
                        
            //            // If we're an issue select form that doesn't know about its main data table yet, set it now.
            //            if (this is IssueSelectFormLogic)
            //            {
            //                if ((this as IssueSelectFormLogic).MainDataTable == null)
            //                    (this as IssueSelectFormLogic).MainDataTable = Behavior.NaturalListSourceTable;
            //            }
            //        }
            //    }
            //}

            //// Build edit conditions if the configuration object has some
            //if (pCtrl.Conditions.Count > 0)
            //    BuildCtrlConditions(ref Behavior, pCtrl);

            //// Build edit restrictions if the configuration object has some
            //if (pCtrl.Restrictions.Count > 0)
            //    BuildCtrlRestrictions(ref Behavior, pCtrl);
        }

        //private void BuildListBoxFromCfg(TEditListBox pCtrl)
        //{
        //    // First, lets see if its something we want to flag as a special fieldname or category
        //    SetKnownFieldCategory(pCtrl);

        //    // Create TextBoxBehavior object and set default attributes
        //    ReinoControls.TextBoxBehavior Behavior = new TextBoxBehavior();
        //    Behavior.ListStyle = ReinoControls.ListBoxStyle.lbsStatic;
        //    pCtrl.IsEnabled = true;

        //    // Set cross-ref links between new behavior and configuration object.
        //    // Set link so behavior can find other behaviors for this form.
        //    // Add behavior to list of behaviors for this form.
        //    Behavior.CfgCtrl = pCtrl;
        //    pCtrl.Behavior = Behavior;
        //    Behavior.BehaviorCollection = BehaviorCollection;
        //    BehaviorCollection.Add(Behavior);

        //    // We need to add special events to the ReinoTextBox's behavior so the ReinoTextBox 
        //    // code is more portable and doesn't rely on application specific stuff...
        //    Behavior.OnFindNextFormControl += this.FindNextFormControl;
        //    Behavior.OnWhichControlIsFirst += this.WhichControlIsFirst;
        //    Behavior.OnGetFocusedControl += this.GetFocusedControl;
        //    Behavior.OnGetFormEditMode += this.GetFormEditMode;
        //    Behavior.OnGetFormEditAttrs += this.GetFormEditAttrs;
        //    Behavior.OnSetFormEditAttr += this.SetFormEditAttr;
        //    Behavior.OnTabForward += this.TabForward;
        //    Behavior.OnTabBackward += this.TabBackward;
        //    Behavior.OnNavigationKeyPress += new TextBoxBehavior.NavigationKeyPress(Field_OnNavigationKeyPress);
        //    Behavior.OnRegularKeyPress += new TextBoxBehavior.RegularKeyPress(Field_OnRegularKeyPress);

        //    // JLA (03/15/07) More events that weren't being set before:
        //    Behavior.TextChanged += new EventHandler(Behavior_TextChanged);
        //    Behavior.NotifiedDependentsParentDataChanged += new EventHandler(Behavior_NotifiedDependentsParentDataChanged);
        //    Behavior.OnCreatedPopupListBox += new TextBoxBehavior.CreatedPopupListBox(Behavior_OnCreatedPopupListBox);
        //    Behavior.OnCreatedRegularListBox += new TextBoxBehavior.CreatedRegularListBox(Behavior_OnCreatedRegularListBox);
        //    Behavior.OnGetNewRegularListBox += new TextBoxBehavior.GetNewRegularListBox(Behavior_OnGetNewRegularListBox);
        //    Behavior.OnCtrlGotFocus += new TextBoxBehavior.CtrlGotFocus(Behavior_OnCtrlGotFocus);
        //    Behavior.OnGetRingBellVolume += this.GetRingBellVolume;

        //    // Copy applicable properties from the configuration object to the ReinoTextBox's Behavior object.

        //    // We must convert the ClientConfig field type to the ReinoTextBox field type
        //    if (pCtrl is Reino.ClientConfig.TEditField)
        //    {
        //        if ((pCtrl as TEditField).FieldType == TEditField.TEditFieldType.efDate)
        //            Behavior.SetFieldType(ReinoControls.TEditFieldType.efDate);
        //        else if ((pCtrl as TEditField).FieldType == TEditField.TEditFieldType.efTime)
        //            Behavior.SetFieldType(ReinoControls.TEditFieldType.efTime);
        //        else if ((pCtrl as TEditField).FieldType == TEditField.TEditFieldType.efString)
        //            Behavior.SetFieldType(ReinoControls.TEditFieldType.efString);
        //    }

        //    // Copy the edit mask and maxlength properties
        //    if (pCtrl is Reino.ClientConfig.TEditField)
        //        Behavior.SetEditMask((pCtrl as TEditField).EditMask);
        //    Behavior.MaxLength = pCtrl.MaxLength;

        //    // If a ListName is specified, we have a little bit of work to do...
        //    if (pCtrl is Reino.ClientConfig.TEditField)
        //    {
        //        if ((pCtrl as TEditField).ListName != "")
        //        {
        //            // Try to split the ListName property into two elements that together
        //            // specify a Table and Column for the list's source.
        //            string[] ListNameElements = (pCtrl as TEditField).ListName.Split(' ');
        //            if (ListNameElements.Length == 2)
        //            {
        //                // Set the behavior's list source properties (both natural and active list references)
        //                if (this is IssueSelectFormLogic)
        //                    Behavior.NaturalListSourceTable = GetListTableByName(ListNameElements[0], ListSearchType.NewIfAgencyOr2ndData);
        //                else
        //                    Behavior.NaturalListSourceTable = GetListTableByName(ListNameElements[0], ListSearchType.NewIfAgencyOr1stData/*Get1st*/);
        //                Behavior.NaturalListSourceFieldName = ListNameElements[1];
        //                Behavior.ListSourceTable = Behavior.NaturalListSourceTable;
        //                Behavior.ListSourceFieldName = Behavior.NaturalListSourceFieldName;

        //                // If we're an issue select form that doesn't know about its main data table yet, set it now.
        //                if (this is IssueSelectFormLogic)
        //                {
        //                    if ((this as IssueSelectFormLogic).MainDataTable == null)
        //                        (this as IssueSelectFormLogic).MainDataTable = Behavior.NaturalListSourceTable;
        //                }
        //            }
        //        }
        //    }

        //    // Build edit conditions if the configuration object has some
        //    if (pCtrl.Conditions.Count > 0)
        //        BuildCtrlConditions(ref Behavior, pCtrl);

        //    // Build edit restrictions if the configuration object has some
        //    if (pCtrl.Restrictions.Count > 0)
        //        BuildCtrlRestrictions(ref Behavior, pCtrl);
        //}

//        private void BuildDrawBoxFromCfg(TDrawField pCtrl)
//        {
//            // First, lets see if its something we want to flag as a special fieldname or category
//            SetKnownFieldCategory(pCtrl);

//            // Create TextBoxBehavior object and set default attributes
//            ReinoControls.TextBoxBehavior Behavior = new TextBoxBehavior();
//            Behavior.ListStyle = ReinoControls.ListBoxStyle.lbsNone;
//            pCtrl.IsEnabled = true;

//            // Set cross-ref links between new behavior and configuration object.
//            // Set link so behavior can find other behaviors for this form.
//            // Add behavior to list of behaviors for this form.
//            Behavior.CfgCtrl = pCtrl;
//            pCtrl.Behavior = Behavior;
//            Behavior.BehaviorCollection = BehaviorCollection;
//            BehaviorCollection.Add(Behavior);

//            // We need to add special events to the ReinoTextBox's behavior so the ReinoTextBox 
//            // code is more portable and doesn't rely on application specific stuff...
//            Behavior.OnFindNextFormControl += this.FindNextFormControl;
//            Behavior.OnWhichControlIsFirst += this.WhichControlIsFirst;
//            Behavior.OnGetFocusedControl += this.GetFocusedControl;
//            Behavior.OnGetFormEditMode += this.GetFormEditMode;
//            Behavior.OnGetFormEditAttrs += this.GetFormEditAttrs;
//            Behavior.OnSetFormEditAttr += this.SetFormEditAttr;
//            Behavior.OnTabForward += this.TabForward;
//            Behavior.OnTabBackward += this.TabBackward;
        //            Behavior.OnNavigGetTextSelectedLengthationKeyPress += new TextBoxBehavior.NavigationKeyPress(Field_OnNavigationKeyPress);
//            Behavior.OnRegularKeyPress += new TextBoxBehavior.RegularKeyPress(Field_OnRegularKeyPress);

//            // JLA (03/15/07) More events that weren't being set before:
//            Behavior.TextChanged += new EventHandler(Behavior_TextChanged);
//            Behavior.NotifiedDependentsParentDataChanged += new EventHandler(Behavior_NotifiedDependentsParentDataChanged);
//            Behavior.OnCreatedPopupListBox += new TextBoxBehavior.CreatedPopupListBox(Behavior_OnCreatedPopupListBox);
//            Behavior.OnCreatedRegularListBox += new TextBoxBehavior.CreatedRegularListBox(Behavior_OnCreatedRegularListBox);
//            Behavior.OnGetNewRegularListBox += new TextBoxBehavior.GetNewRegularListBox(Behavior_OnGetNewRegularListBox);
//            Behavior.OnCtrlGotFocus += new TextBoxBehavior.CtrlGotFocus(Behavior_OnCtrlGotFocus);
//            Behavior.OnGetRingBellVolume += this.GetRingBellVolume;

//            // Copy applicable properties from the configuration object to the ReinoTextBox's Behavior object.

//            // We must convert the ClientConfig field type to the ReinoTextBox field type
//            Behavior.SetFieldType(ReinoControls.TEditFieldType.efString);

//            // Copy the maxlength properties (edit mask is not applicable to TDrawField?)
//            Behavior.MaxLength = pCtrl.MaxLength;

//            // For both safety and speed, lets get an uppercase version of cfg object's name for string comparison
//            string loCfgObjName = pCtrl.Name.ToUpper();
//            if (loCfgObjName.Equals(FieldNames.SuspSignatureFieldName))
//                fDrawSuspSignature = pCtrl;
//            else if (loCfgObjName.Equals(FieldNames.OfficerSignatureFieldName))
//                fDrawOfficerSignature = pCtrl;

//            // TDrawFields shouldn't be associated with lists, so don't bother checking for ListName...

//            // Build edit conditions if the configuration object has some
//            if (pCtrl.Conditions.Count > 0)
//                BuildCtrlConditions(ref Behavior, pCtrl);

//            // Build edit restrictions if the configuration object has some
//            if (pCtrl.Restrictions.Count > 0)
//                BuildCtrlRestrictions(ref Behavior, pCtrl);
//        }

//        private void BuildBitmapFromCfg(Reino.ClientConfig.TTBitmap pCtrl)
//        {
//            // First, lets see if its something we want to flag as a special fieldname or category
//            SetKnownFieldCategory(pCtrl);

//            // Add this configuration control to the EntryOrder list for easier navigation
//            EntryOrder.Add(pCtrl);

//            // Create an image with the same name as the passed configuration object
//            PictureBox newImage = new PictureBox();
//            newImage.Name = pCtrl.Name;
//            newImage.Left = 2;
//            newImage.Top = NextCtrlTop;
//            newImage.Height = pCtrl.Height;
//            newImage.Width = pCtrl.Width;
//            newImage.SizeMode = PictureBoxSizeMode.StretchImage; // Zoom?
//#if !WindowsCE && !__ANDROID__ 
//            newImage.BorderStyle = BorderStyle.Fixed3D;
//#endif
//            newImage.MouseDown += new MouseEventHandler(Bitmap_Click);

//            // Start with Window control invisible but enabled
//            newImage.Visible = false;
//            newImage.Enabled = true;

//            // Set cross-ref links between new edit control and configuration object
//            pCtrl.WinCtrl = newImage;

//            // Increment the NextCtrlTop so the next control is added below us
//            NextCtrlTop += newImage.Height + 4;
//        }

//        private void BuildActionBtnFromCfg(Reino.ClientConfig.TTButton pCtrl)
//        {
//            // First, lets see if its something we want to flag as a special fieldname or category
//            SetKnownFieldCategory(pCtrl);

//            // Find out if previous item in list is a button that could share the row with us.
//            int SharedRowIdx = -1;
//            if ((EntryOrder.Count > 0) && (EntryOrder[EntryOrder.Count - 1] is TTButton))
//            {
//                // If previous item was a button that is not already marked as shared, 
//                // then set it as shared with the index we're about to insert. Also, we 
//                // will set flag so we know to share current button after we create it.
//                if (((TTButton)(EntryOrder[EntryOrder.Count - 1])).SharesRowWithIdx == -1)
//                {
//                    ((TTButton)(EntryOrder[EntryOrder.Count - 1])).SharesRowWithIdx =
//                        EntryOrder.Count;
//                    SharedRowIdx = EntryOrder.Count - 1;
//                }
//            }

//            // Add this configuration control to the EntryOrder list for easier navigation
//            EntryOrder.Add(pCtrl);

//            // Remove HotKey from config if present (1st and 2nd character)
//            if (pCtrl.TextBuf.IndexOf("=") == 1)
//                pCtrl.TextBuf = pCtrl.TextBuf.Remove(0, 2);

//            // Create a button with the same name as the passed configuration object
//            ReinoControls.ReinoNavButton newBtn = new ReinoControls.ReinoNavButton();
//            newBtn.ResizeSuspended = true;
//            newBtn.Name = pCtrl.Name;
//            newBtn.AssocCfgBtn = pCtrl;
//            // If its a TTBitBtn, then set the glyph type as "custom"
//            if (pCtrl is TTBitBtn)
//            {
//                newBtn.GlyphType = ReinoButtonGlyphs.bgCustom;
//                newBtn.GlyphXOffset = 3;
//                newBtn.GlyphYOffset = 2;
//            }
//            newBtn.Text = pCtrl.TextBuf;
//            newBtn.Left = 2;
//            newBtn.Top = NextCtrlTop;
//            if (pCtrl.Width != 0)
//                newBtn.Width = pCtrl.Width;
//            else
//                newBtn.Width = 100;
//            newBtn.FocusFont = _Font10ptBold;
//            newBtn.NormalFont = _Font10pt;
//            newBtn.Height = SectionBtnHeight;
//            newBtn.GotFocus += new EventHandler(ActionBtn_GotFocus);
//            newBtn.LostFocus += new EventHandler(ActionBtn_LostFocus);
//            newBtn.MouseDown += new MouseEventHandler(ActionBtn_Click);
//            newBtn.KeyPress += new KeyPressEventHandler(ActionBtn_KeyPress);
//            newBtn.KeyDown += new KeyEventHandler(ActionBtn_KeyDown);
//#if !WindowsCE && !__ANDROID__  
//            newBtn.OnIsInputKey += new ReinoNavButton.IsInputKeyEvent(ActionBtn_OnIsInputKey);
//#endif

//            // Set the shared row index (-1 means its not shared with anything else)
//            pCtrl.SharesRowWithIdx = SharedRowIdx;

//            // Start with Window control invisible but enabled
//            newBtn.Visible = false;
//            newBtn.Enabled = true;
//            // Set visible and enabled in configuration control to true
//            // (They act as state-holders since the configuration control is not visual)
//            pCtrl.Enabled = true;
//            pCtrl.Visible = true;

//            // For both safety and speed, lets get an uppercase version of cfg object's name for string comparison
//            string loCfgObjName = pCtrl.Name.ToUpper();

//            // Set reference to static object based on name
//            if (loCfgObjName.Equals(BtnDoneName))
//                BtnDone = newBtn;
//            else if (loCfgObjName.Equals(BtnPrintName))
//                BtnPrint = newBtn;
//            else if (loCfgObjName.Equals(BtnNotesName))
//                BtnNotes = newBtn;
//            else if (loCfgObjName.Equals(BtnCorrectionName))
//                BtnCorrection = newBtn;
//            else if (loCfgObjName.Equals(BtnIssueMoreName))
//                BtnIssueMore = newBtn;
//            else if (loCfgObjName.Equals(BtnIssueMultipleName))
//                BtnIssueMultiple = newBtn;
//            else if (loCfgObjName.Equals(BtnEndIssueMultipleName))
//                BtnEndIssueMultiple = newBtn;
//            else if (loCfgObjName.Equals(BtnCancelName))
//                BtnCancel = newBtn;
//            else if (loCfgObjName.Equals(BtnVoidName))
//                BtnVoid = newBtn;
//            else if (loCfgObjName.Equals(BtnReissueName))
//                BtnReissue = newBtn;
//            else if (loCfgObjName.Equals(BtnSuspSignatureName))
//                BtnSuspSignature = newBtn;
//            else if (loCfgObjName.Equals(BtnOfficerSignatureName))
//                BtnOfficerSignature = newBtn;
//            else if (loCfgObjName.Equals(BtnContinuanceName))
//                BtnContinuance = newBtn;
//            else if (loCfgObjName.Equals(BtnNewName))
//                BtnNew = newBtn;
//            else if (loCfgObjName.Equals(BtnIssueChildName))
//                BtnIssueChild = newBtn;
//            else if (loCfgObjName.Equals(BtnReadReinoName))
//                BtnReadReino = newBtn;

//            // Set cross-ref links between new edit control and configuration object
//            pCtrl.WinCtrl = newBtn;

//            // Increment the NextCtrlTop so the next control is added below us
//            NextCtrlTop += newBtn.Height + 4;
//        }

//        void ActionBtn_LostFocus(object sender, EventArgs e)
//        {
//            if (!(sender is ReinoNavButton))
//                return;

//            if ((sender as ReinoNavButton).NormalFont == null)
//                return;

//            ReinoNavButton Btn = (sender as ReinoNavButton);
//            Btn.Font = Btn.NormalFont;
//            Btn.Invalidate();
//        }

//        void ActionBtn_GotFocus(object sender, EventArgs e)
//        {
//            if (!(sender is ReinoNavButton))
//                return;

//            if ((sender as ReinoNavButton).FocusFont == null)
//                return;

//            ReinoNavButton Btn = (sender as ReinoNavButton);
//            Btn.Font = Btn.FocusFont;
//            Btn.Invalidate();
//        }

//        private void BuildCtrlRestrictions(ref ReinoControls.TextBoxBehavior pBehavior, Reino.ClientConfig.TTEdit pCtrl)
//        {
//            // Make sure we have the event OnRestrictionForcesRebuildDisplay event for the passed behavior object
//            pBehavior.OnRestrictionForcesDisplayRebuild += this.OnRestrictionForcesDisplayRebuild;

//            foreach (Reino.ClientConfig.TEditRestriction ConfigRestriction in pCtrl.Restrictions)
//            {
//                // EditRestictions didn't get their registry hooks resolved yet, so do it now
//                ConfigRestriction.ResolveRegistryItems(TTRegistry.glRegistry);

//                // Certain edit restriction classes need application-defined delegate methods assigned
//                if (ConfigRestriction is Reino.ClientConfig.TER_ForceGlobalCurrentValue)
//                {
//                    // We need to add an event to TER_ForceGlobalCurrentValueImp so it 
//                    // can call logic in the form 
//                    ((Reino.ClientConfig.TER_ForceGlobalCurrentValue)(ConfigRestriction)).OnGetCurrentGlobalFieldValue +=
//                        Reino.CommonLogic.IssueAppImp.GlobalIssueApp.GetCurrentGlobalFieldValue;
//                }
//                else if (ConfigRestriction is Reino.ClientConfig.TER_SetGlobalCurrentValue)
//                {
//                    // We need to add an event to TER_SetGlobalCurrentValueImp so it 
//                    // can call logic in the form 
//                    ((Reino.ClientConfig.TER_SetGlobalCurrentValue)(ConfigRestriction)).OnSetCurrentGlobalFieldValue +=
//                        Reino.CommonLogic.IssueAppImp.GlobalIssueApp.SetCurrentGlobalFieldValue;
//                }
//                else if (ConfigRestriction is Reino.ClientConfig.TER_ForceSequence)
//                {
//                    // We need to add an event to TER_ForceSequence so it can call logic
//                    // in the form to control setting the next issue number
//                    ((Reino.ClientConfig.TER_ForceSequence)(ConfigRestriction)).OnSetIssueNoFields +=
//                        this.SetIssueNoFields;
//                }
//                else if (ConfigRestriction is Reino.ClientConfig.TER_CurrentUser)
//                {
//                    // This edit restriction needs to know where to find datasets to get current user info
//                    ((Reino.ClientConfig.TER_CurrentUser)(ConfigRestriction)).clientDef =
//                        TClientDef.GlobalClientDef;
//                }
//                else if (ConfigRestriction is Reino.ClientConfig.TER_SearchHotSheet)
//                {
//                    ((Reino.ClientConfig.TER_SearchHotSheet)(ConfigRestriction)).OnDoSearch +=
//                        DoHotSheetSearch;
//                }
//                else if (ConfigRestriction is Reino.ClientConfig.TER_HotsheetFilter)
//                {
//                    ((Reino.ClientConfig.TER_HotsheetFilter)(ConfigRestriction)).OnDoHotSheetFilter +=
//                        DoHotSheetFilter;
//                }
//                else if (ConfigRestriction is Reino.ClientConfig.TER_Protected)
//                {
//                    ((Reino.ClientConfig.TER_Protected)(ConfigRestriction)).OnRestrictionForcesDisplayRebuild +=
//                        OnRestrictionForcesDisplayRebuild;
//                }
//                else if (ConfigRestriction is Reino.ClientConfig.TER_Hidden)
//                {
//                    ((Reino.ClientConfig.TER_Hidden)(ConfigRestriction)).OnRestrictionForcesDisplayRebuild +=
//                        OnRestrictionForcesDisplayRebuild;
//                }
//                else if (ConfigRestriction is Reino.ClientConfig.TER_ListFilter)
//                {
//                    ((Reino.ClientConfig.TER_ListFilter)(ConfigRestriction)).OnListContentsChangedByRestriction +=
//                        OnListContentsChangedByRestriction;
//                }

//                // The edit restiction must have a parent property that points to the TextBoxBehavior
//                // that it is associated with
//                ConfigRestriction.SetParent(pBehavior);
//                // Add the new edit restriction to the EditRestrictions list of the TextBoxBehavior
//                pBehavior.EditRestrictions.Add(ConfigRestriction);
//            }
//        }

//        private void BuildCtrlConditions(ref ReinoControls.TextBoxBehavior pBehavior, Reino.ClientConfig.TTEdit pCtrl)
//        {
//            foreach (Reino.ClientConfig.TEditCondition ConfigCondition in pCtrl.Conditions)
//            {
//                // The edit condition must have a parent property that points to the TextBoxBehavior
//                // that it is associated with
//                ConfigCondition.SetParent(pBehavior);
//                // Add the new edit condition to the EditConditions list of the TextBoxBehavior
//                pBehavior.EditConditions.Add(ConfigCondition);
//            }
//        }

        //private ReinoTextBox CreateNewTextBox()
        //{
        //    ReinoControls.ReinoTextBox newTextBox = new ReinoControls.ReinoTextBox(null);
        //    newTextBox.Text = "";
        //    newTextBox.Visible = false;
        //    return newTextBox;
        //}

        //private Label CreateNewLabel()
        //{
        //    Label newLabel = new Label();
        //    newLabel.Text = "";
        //    newLabel.Visible = false;
        //    newLabel.Scale(ReinoControls.BasicButton.ScaleFactorSize);
        //    return newLabel;
        //}

        //private void PrepTextBoxForCfgCtrl(ReinoTextBox pTextBox, Reino.ClientConfig.TTControl pCtrl)
        //{
        //    pTextBox.Visible = false;
        //    pTextBox.BaseText = "";
        //    pTextBox.WordWrap = false;

        //    // Set association with the configuration object
        //    pTextBox.Behavior = pCtrl.Behavior;
        //    pTextBox.Behavior.CfgCtrl.WinCtrl = pTextBox;
        //    pTextBox.Behavior.AssociateWithReinoTextBox(pTextBox);

        //    // If configuration specifies taller than 20, then we'll use the config's height.
        //    // Also. if its a memo, then we want wordwrap
        //    if (pCtrl.Height > 20)
        //    {
        //        pTextBox.Height = pCtrl.Height * ReinoControls.BasicButton.ScaleFactorAsInt;
        //        if (pCtrl is TTMemo)
        //            pTextBox.WordWrap = true;
        //        //pTextBox.Behavior.NormalFont
        //    }
        //    else
        //    {
        //        pTextBox.Height = 20 * ReinoControls.BasicButton.ScaleFactorAsInt;
        //    }
        //    pTextBox.Width = pCtrl.Width * ReinoControls.BasicButton.ScaleFactorAsInt;

        //    // Set the control's text without triggering change events
        //    pTextBox.BaseText = pCtrl.Behavior.GetText();

        //    // Set control's availability
        //    if ((pCtrl.IsProtected == false) && (pCtrl.IsEnabled == true))
        //        pTextBox.Enabled = true;
        //    else
        //        pTextBox.Enabled = false;
        //    if (pCtrl.IsHidden == false)
        //        pTextBox.Visible = true;
        //    else
        //        pTextBox.Visible = false;

        //    // If we have less than 8 list items, lets use radio box instead of static listbox
        //    // (We won't change anything if its a popup-sytle though)
        //    if ((pTextBox.Behavior.ListSourceTable != null) &&
        //        (pTextBox.Behavior.ListStyle != ListBoxStyle.lbsPopup))
        //    {
        //        if (pTextBox.Behavior.ListItemCount <= 8)
        //            pTextBox.Behavior.ListStyle = ListBoxStyle.lbsRadio;
        //        else
        //            pTextBox.Behavior.ListStyle = ListBoxStyle.lbsStatic;
        //    }
        //}

        //private ReinoControls.ReinoVirtualListBox CreateNewListBox()
        //{
        //    // Create a virtual list box that does NOT create its own cache for list items.
        //    ReinoControls.ReinoVirtualListBox newListBox = new ReinoControls.ReinoVirtualListBox(false);
        //    newListBox.Text = "";
        //    SetListBoxVisible(newListBox, false);
        //    newListBox.TrapAllNavigationKeys = true;
        //    newListBox.OnNavigationKeyPress += new ReinoVirtualListBox.NavigationKeyPress(DataGrid_OnNavigationKeyPress);
        //    newListBox.GetVirtualTextForIndex += new GetVirtualTextEventHandlder(DataGrid_GetVirtualTextForIndex);
        //    newListBox.DrawItem += new ReinoControls.DrawItemEventHandler(DataGrid_DrawItem);
        //    newListBox.OnMeasureHeader += new MeasureItemEventHandler(DataGrid_OnMeasureHeader);
        //    newListBox.DrawHeader += new ReinoControls.DrawItemEventHandler(DataGrid_DrawHeader);
        //    return newListBox;
        //}

        //private void PrepEditListBoxForCfgCtrl(ReinoControls.ReinoVirtualListBox pListBox, Reino.ClientConfig.TTControl pCtrl)
        //{
        //    SetListBoxVisible(pListBox, false);

        //    // Set association with the configuration object
        //    pListBox.Behavior = pCtrl.Behavior;
        //    pListBox.Behavior.CfgCtrl.WinCtrl = pListBox;
        //    pListBox.Behavior.AssociateWithListBox(pListBox);

        //    // If configuration specifies taller than 20, then we'll use the config's height.
        //    if (pCtrl.Height > 20)
        //        pListBox.Height = pCtrl.Height * ReinoControls.BasicButton.ScaleFactorAsInt;
        //    else
        //        pListBox.Height = 20 * ReinoControls.BasicButton.ScaleFactorAsInt;
        //    pListBox.Width = pCtrl.Width * ReinoControls.BasicButton.ScaleFactorAsInt;

        //    if (pListBox.Width >= 230 * ReinoControls.BasicButton.ScaleFactorAsInt)
        //    {
        //        System.Diagnostics.Debug.WriteLine(pCtrl.Name + ".Width = " + pCtrl.Width.ToString());
        //    }

        //    // Set control's availability
        //    if ((pCtrl.IsProtected == false) && (pCtrl.IsEnabled == true))
        //        pListBox.Enabled = true;
        //    else
        //        pListBox.Enabled = false;
        //    if (pCtrl.IsHidden == false)
        //        SetListBoxVisible(pListBox, true);
        //    else
        //        SetListBoxVisible(pListBox, false);

        //    // Attach listbox to cache associated with behavior
        //    pListBox.Cache = pCtrl.Behavior.ListItemCache;
        //    pListBox.DisplayCache = pCtrl.Behavior.GridDisplayCache;
        //    pListBox.Count = pCtrl.Behavior.ListItemCount;
        //    pListBox.RefreshItems(false);

        //    // Set the control's text without triggering change events
        //    pListBox.SelectedIndexWithoutChangeEvent = pCtrl.Behavior.GetListNdx();
        //}

        //private ReinoControls.ReinoDrawBox CreateNewDrawBox()
        //{
        //    ReinoControls.ReinoDrawBox newDrawBox = new ReinoControls.ReinoDrawBox();
        //    newDrawBox.Text = "";
        //    newDrawBox.Visible = false;
        //    newDrawBox.TrapAllNavigationKeys = true;
        //    newDrawBox.OnNavigationKeyPress += new ReinoDrawBox.NavigationKeyPress(DrawBox_OnNavigationKeyPress);
        //    return newDrawBox;
        //}

        //private void PrepDrawBoxForCfgCtrl(ReinoControls.ReinoDrawBox pDrawBox, Reino.ClientConfig.TTControl pCtrl)
        //{
        //    pDrawBox.Visible = false;

        //    // Set association with the configuration object
        //    pDrawBox.Behavior = pCtrl.Behavior;
        //    pDrawBox.Behavior.CfgCtrl.WinCtrl = pDrawBox;
        //    pDrawBox.Behavior.AssociateWithListBox(pDrawBox);

        //    // If configuration specifies taller than 20, then we'll use the config's height.
        //    if (pCtrl.Height > 20)
        //        pDrawBox.Height = pCtrl.Height * ReinoControls.BasicButton.ScaleFactorAsInt;
        //    else
        //        pDrawBox.Height = 20 * ReinoControls.BasicButton.ScaleFactorAsInt;
        //    pDrawBox.Width = pCtrl.Width * ReinoControls.BasicButton.ScaleFactorAsInt;

        //    // Set control's availability
        //    if ((pCtrl.IsProtected == false) && (pCtrl.IsEnabled == true))
        //        pDrawBox.Enabled = true;
        //    else
        //        pDrawBox.Enabled = false;
        //    if (pCtrl.IsHidden == false)
        //        pDrawBox.Visible = true;
        //    else
        //        pDrawBox.Visible = false;

        //    // Set the control's text which will also update the lines
        //    pDrawBox.Text = pCtrl.Behavior.GetText();
        //}

        //protected virtual void Bitmap_Click(object sender, EventArgs e)
        //{
        //    // Ensure that we don't retain mouse capture
        //    if (sender is PictureBox)
        //        (sender as PictureBox).Capture = false;
        //    return;
        //}

        //private void AddSectionButtons(System.Collections.Generic.IList<TWinClass> Container)
        //{
        //    foreach (Reino.ClientConfig.TWinClass NextCtrl in Container)
        //    {
        //        // Is it a "TabSheet" container?
        //        if (NextCtrl is Reino.ClientConfig.TTTabSheet)
        //            AddSectionButtons(((Reino.ClientConfig.TTTabSheet)(NextCtrl)).Sheets); // recursive

        //        // Is it a TTPanel or descendant? (TTTabSheet, TPanel, TSheet)
        //        if (NextCtrl is Reino.ClientConfig.TTPanel)
        //            AddSectionButtons(((Reino.ClientConfig.TTPanel)(NextCtrl)).Controls); // recursive

        //        if (((NextCtrl is Reino.ClientConfig.TTEdit) || (NextCtrl is Reino.ClientConfig.TTButton))
        //            && (_LastBuiltSectionButton != null))
        //        {
        //            Reino.CommonLogic.IssFormSection section = null;
        //            // Is this the first field of the section?
        //            // If so, we need to create a section, associate it with the section button,
        //            // and set the link for the first and last fields.
        //            if (_LastBuiltSectionButton.Tag == null)
        //            {
        //                section = new IssFormSection();
        //                section.FirstCfgCtrl = ((Reino.ClientConfig.TTControl)(NextCtrl));
        //                section.LastCfgCtrl = ((Reino.ClientConfig.TTControl)(NextCtrl));
        //                _LastBuiltSectionButton.Tag = section;

        //                // Now find the field index
        //                int loLoopMax = EntryOrder.Count;
        //                for (int loIdx = 0; loIdx < loLoopMax; loIdx++)
        //                {
        //                    TTControl NextEdit = EntryOrder[loIdx];
        //                    if (NextEdit == section.FirstCfgCtrl)
        //                    {
        //                        section.FirstCfgCtrlIdx = loIdx;
        //                        section.LastCfgCtrlIdx = loIdx;
        //                        break;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // Section already exists, so get reference to it and update the last field info
        //                section = _LastBuiltSectionButton.Tag as Reino.CommonLogic.IssFormSection;
        //                section.LastCfgCtrl = ((Reino.ClientConfig.TTControl)(NextCtrl));

        //                // Now find the field index
        //                int loLoopMax = EntryOrder.Count;
        //                for (int loIdx = 0; loIdx < loLoopMax; loIdx++)
        //                {
        //                    TTControl NextEdit = EntryOrder[loIdx];
        //                    if (NextEdit == section.LastCfgCtrl)
        //                    {
        //                        section.LastCfgCtrlIdx = loIdx;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

//        private void AddSectionButtons(System.Collections.Generic.IList<TSheet> Container)
//        {
//            foreach (Reino.ClientConfig.TSheet NextCtrl in Container)
//            {
//                // If we should be starting a new line, update the next top and left position
//                if ((SectionBtnCnt > 0) && ((SectionBtnCnt % SectionBtnColumns) == 0))
//                {
//                    NextBtnLeft = 4;
//                    // Only increment the next btn top if were working with the first 2 rows
//                    if ((_SectionButtonsVScroll == null) || (SectionBtnCnt < 8))
//                        NextBtnTop += SectionBtnHeight;
//                }

//                ReinoNavButton NewSectionButton = new ReinoNavButton();
//                // It would be nice to use ResizeSuspended, but for some reason we can't get it to 
//                // work correctly on the handheld...
//                NewSectionButton.ResizeSuspended = false; // true; 
//                SectionButtons.Add(NewSectionButton);

//                NewSectionButton.Text = NextCtrl.Name;
//                if (_SectionButtonsVScroll == null)
//                    NewSectionButton.Width = ((int)((this.NavigationPanel.Width - 8) / SectionBtnColumns)); // 100
//                else
//                    NewSectionButton.Width = ((int)(((this.NavigationPanel.Width - 8) - _SectionButtonsVScroll.Width) / SectionBtnColumns)); // 100
                
//                // Use normal height if button is in the first two rows. After that, set to height = 0, as
//                // it will be brought into view and resized by "scrolling into view"
//                if ((_SectionButtonsVScroll == null) || (SectionBtnCnt < 8))
//                    NewSectionButton.Height = SectionBtnHeight;
//                else
//                    NewSectionButton.Height = SectionBtnHeight; // 0;

//                NewSectionButton.Left = NextBtnLeft;
//                if ((_SectionButtonsVScroll == null) || (SectionBtnCnt < 8))
//                    NewSectionButton.Top = NextBtnTop;
//                else
//                    NewSectionButton.Top = 28 + (SectionBtnHeight * 2) + (1 * 2);
//                NewSectionButton.BackColor = SystemColors.Control; //Color.FromArgb(175, 206, 251); 
//                NewSectionButton.Font = FontCache.CreateFontManaged("Tahoma", (10F * ReinoControls.BasicButton.ScaleFactor), System.Drawing.FontStyle.Regular);
//                SectionBtnCnt += 1;
//                NextBtnLeft += NewSectionButton.Width;
//                _LastBuiltSectionButton = NewSectionButton;
//                NewSectionButton.KeyDown += new KeyEventHandler(NewSectionButton_KeyDown);
//                NewSectionButton.KeyPress += new KeyPressEventHandler(NewSectionButton_KeyPress);
//#if !WindowsCE && !__ANDROID__    
//                NewSectionButton.OnIsInputKey += new ReinoNavButton.IsInputKeyEvent(NewSectionButton_OnIsInputKey);
//#endif
//                NewSectionButton.MouseDown += new MouseEventHandler(NewSectionButton_Click);
//                NewSectionButton.Parent = this.NavigationPanel;

//                // Is it a TTPanel or descendant? (It certainly should be since TSheet inherits from TTPanel)
//                if (NextCtrl is Reino.ClientConfig.TTPanel)
//                    AddSectionButtons(((Reino.ClientConfig.TTPanel)(NextCtrl)).Controls); // recursive
//            }
//        }

        private void CountSectionsInCfg(System.Collections.Generic.IList<TWinClass> Container)
        {
            foreach (Reino.ClientConfig.TWinClass NextCtrl in Container)
            {
                // Is it a "TabSheet" container?
                if (NextCtrl is Reino.ClientConfig.TTTabSheet)
                    CountSectionsInCfg((NextCtrl as Reino.ClientConfig.TTTabSheet).Sheets); // recursive

                // Is it a TTPanel or descendant? (TTTabSheet, TPanel, TSheet)
                if (NextCtrl is Reino.ClientConfig.TTPanel)
                    CountSectionsInCfg((NextCtrl as Reino.ClientConfig.TTPanel).Controls); // recursive
            }
        }

        private void CountSectionsInCfg(System.Collections.Generic.IList<TSheet> Container)
        {
            foreach (Reino.ClientConfig.TSheet NextCtrl in Container)
            {
                // Increment the number of sections that we find
                _SectionsInCfg++;

                // Is it a TTPanel or descendant? (It certainly should be since TSheet inherits from TTPanel)
                if (NextCtrl is Reino.ClientConfig.TTPanel)
                    CountSectionsInCfg(((Reino.ClientConfig.TTPanel)(NextCtrl)).Controls); // recursive
            }
        }

        //private void ResolveObjReferences(Reino.ClientConfig.TBaseIssForm loIssForm)
        //{
        //    // Loop through behaviors in the parent container 
        //    foreach (TextBoxBehavior NextBehavior in this.BehaviorCollection)
        //    {
        //        // Resolved oject references for each edit restriction and edit dependency
        //        foreach (Reino.ClientConfig.TEditRestriction loEditRestrict in NextBehavior.EditRestrictions)
        //            loEditRestrict.ResolveObjectReferences();
        //        foreach (Reino.ClientConfig.TEditCondition loEditCondition in NextBehavior.EditConditions)
        //            loEditCondition.ResolveObjectReferences();

        //        // For both safety and speed, lets get an uppercase version of cfg object's name for string comparison
        //        string loCfgObjName = NextBehavior.CfgCtrl.Name.ToUpper();

        //        // We need to set flags for certain types of fields so we don't spend too much time
        //        // doing fieldname comparisons during application useage...
        //        if (loCfgObjName.StartsWith(FieldNames.VioDesc1FieldName))
        //            NextBehavior.CfgCtrl.IsVioField = true;
        //        if (loCfgObjName.StartsWith(FieldNames.VioCodeFieldName))
        //            NextBehavior.CfgCtrl.IsVioField = true;
        //        if (loCfgObjName.StartsWith(FieldNames.VioFineFieldName))
        //            NextBehavior.CfgCtrl.IsVioField = true;
        //        if (loCfgObjName.StartsWith(FieldNames.VioSelectFieldName))
        //            NextBehavior.CfgCtrl.IsVioField = true;

        //        if (loCfgObjName.StartsWith(FieldNames.NotesMemoFieldName))
        //            NextBehavior.CfgCtrl.IsNotesMemo = true;

        //        if ((loCfgObjName.Equals(FieldNames.VehColor1FieldName)) ||
        //            (loCfgObjName.Equals(FieldNames.VehColor2FieldName)) ||
        //            (loCfgObjName.Equals(FieldNames.SuspEyeColorFieldName)) ||
        //            (loCfgObjName.Equals(FieldNames.SuspHairColorFieldName)))
        //        {
        //            NextBehavior.CfgCtrl.IsColorField = true;
        //            // Lets try to have uniform width for color items
        //            NextBehavior.CfgCtrl.Width = 115;
        //        }

        //        // Some fields on TIssueSelectForm will be resized
        //        if (this.CfgForm is TIssueSelectForm)
        //        {
        //            if (loCfgObjName.StartsWith(FieldNames.IssueNoDisplayFieldName))
        //                NextBehavior.CfgCtrl.Width = 100;
        //            if (loCfgObjName.StartsWith(FieldNames.IssueDateFieldName))
        //                NextBehavior.CfgCtrl.Width = 100;
        //            if (loCfgObjName.StartsWith(FieldNames.IssueTimeFieldName))
        //                NextBehavior.CfgCtrl.Width = 75;
        //            if (loCfgObjName.StartsWith(FieldNames.VehLicNoFieldName))
        //                NextBehavior.CfgCtrl.Width = 100;
        //            if (loCfgObjName.StartsWith(FieldNames.VehLicStateFieldName))
        //                NextBehavior.CfgCtrl.Width = 100;
        //            if (loCfgObjName.StartsWith(FieldNames.IsWarningFieldName))
        //                NextBehavior.CfgCtrl.Width = 75;
        //            if (loCfgObjName.StartsWith(FieldNames.VoidDateTimeFieldName))
        //                NextBehavior.CfgCtrl.Width = 150;
        //            if (loCfgObjName.StartsWith(FieldNames.ReissuedByFieldName))
        //                NextBehavior.CfgCtrl.Width = 150;
        //        }

        //        // Some fields on TNotesForm will be resized
        //        if (this.CfgForm is TNotesForm)
        //        {
        //            if (loCfgObjName.StartsWith(FieldNames.NotesMemoFieldName))
        //            {
        //                NextBehavior.CfgCtrl.Width = 180;
        //                NextBehavior.CfgCtrl.Height = 90;
        //            }
        //            if (loCfgObjName.StartsWith(BtnNewName, StringComparison.CurrentCultureIgnoreCase))
        //                NextBehavior.CfgCtrl.Width = 70;
        //            if (loCfgObjName.StartsWith(BtnDoneName, StringComparison.CurrentCultureIgnoreCase))
        //                NextBehavior.CfgCtrl.Width = 70;
        //        }

        //        // IssueNo_Display on the TIssueSelectForm will also be popup instead of static list
        //        if ((this.CfgForm is TIssueSelectForm) &&
        //            (
        //            (loCfgObjName.StartsWith(FieldNames.IssueNoDisplayFieldName)) ||
        //            (loCfgObjName.StartsWith(FieldNames.ControlNoFieldName))
        //            ))
        //        {
        //            NextBehavior.ListStyle = ReinoControls.ListBoxStyle.lbsPopup;
        //        }

        //        // DetailRecNoFieldName will always be popup style
        //        if (loCfgObjName.Equals(FieldNames.DetailRecNoFieldName.ToUpper()))
        //        {
        //            NextBehavior.ListStyle = ReinoControls.ListBoxStyle.lbsPopup;
        //        }

        //        // Vehicle state (or anything with LICSTATE substring) will always be popup style
        //        if ((loCfgObjName.Equals(FieldNames.VehLicStateFieldName.ToUpper())) ||
        //            (loCfgObjName.IndexOf("LICSTATE") != -1))
        //        {
        //            NextBehavior.ListStyle = ReinoControls.ListBoxStyle.lbsPopup;
        //        }

        //        // PlateType will always be popup style
        //        if (loCfgObjName.Equals("VEHLICTYPE"))
        //        {
        //            NextBehavior.ListStyle = ReinoControls.ListBoxStyle.lbsPopup;
        //        }

        //        // LocDirection will always be popup style
        //        if (loCfgObjName.Equals("LOCDIRECTION"))
        //        {
        //            NextBehavior.ListStyle = ReinoControls.ListBoxStyle.lbsPopup;
        //        }

        //        // CancelReason will always be popup style
        //        if (loCfgObjName.Equals("CANCELREASON"))
        //        {
        //            NextBehavior.ListStyle = ReinoControls.ListBoxStyle.lbsPopup;
        //        }

        //        // VIN will always be popup style
        //        if ((loCfgObjName.Equals(FieldNames.VehVINFieldName)) &&
        //            (this.IssueStruct is TCiteStruct))
        //        {
        //            NextBehavior.ListStyle = ReinoControls.ListBoxStyle.lbsPopup;
        //        }
        //    }

        //    // 2nd pass will attach child list after the object references have been created
        //    foreach (TextBoxBehavior NextBehavior in this.BehaviorCollection)
        //    {
        //        // Look through restrictions of current edit control
        //        foreach (Reino.ClientConfig.TEditRestriction loEditRestrict in NextBehavior.EditRestrictions)
        //        {
        //            // If this is a ChildList restriction, we need to tie the appropriate list to the ReinoTextBox
        //            if (loEditRestrict is Reino.ClientConfig.TER_ChildList)
        //            {
        //                ReinoControls.TextBoxBehavior Controller = loEditRestrict.GetControlEdit1Obj();
        //                if (Controller != null)
        //                {
        //                    Reino.ClientConfig.TER_ChildList loChildListRestrict = loEditRestrict as Reino.ClientConfig.TER_ChildList;
        //                    // Set the list source referenced by the child restriction
        //                    loChildListRestrict.ListSourceTable = Controller.NaturalListSourceTable;
        //                    loChildListRestrict.ListSourceFieldName = loEditRestrict.CharParam;
        //                }
        //            }
        //        }
        //    }

        //    // Now we need to set parents for the printpicture
        //    if ((loIssForm is TIssForm) && ((loIssForm as TIssForm).PrintPictureList.Count > 0))
        //    {
        //        if ((loIssForm as TIssForm).PrintPictureList[0] != null)
        //        {
        //            foreach (Reino.ClientConfig.TIssPrnFormRev loPrintPicRev in ((loIssForm as TIssForm).PrintPictureList[0].Revisions))
        //            {
        //                loPrintPicRev.ResolveObjectReferences(null, loPrintPicRev);
        //                loPrintPicRev.AddDataElementsToList(loPrintPicRev.AllPrnDataElements);
        //                this.PrintPicture = loPrintPicRev;
        //            }
        //        }
        //    }
        //}

        //private void SetKnownFieldCategory(TTControl iControl)
        //{
        //    if ((iControl.Name.Equals(FieldNames.IssueNoPfxFieldName)) ||
        //        (iControl.Name.Equals(FieldNames.IssueNoFieldName)) ||
        //        (iControl.Name.Equals(FieldNames.IssueNoSfxFieldName)))
        //    {
        //        iControl.FieldCategory = FieldCategories.IssueNoElement;
        //    }

        //    if ((iControl.Name.Equals(FieldNames.OfficerNameFieldName)) ||
        //        (iControl.Name.Equals(FieldNames.OfficerIDFieldName)))
        //    {
        //        iControl.FieldCategory = FieldCategories.OfficerNameOrID;
        //    }

        //    if ((iControl.Name.Equals(FieldNames.IssueDateFieldName)) ||
        //        (iControl.Name.Equals(FieldNames.IssueTimeFieldName)))
        //    {
        //        iControl.FieldCategory = FieldCategories.IssueDateOrTime;
        //    }

        //    if ((iControl.Name.Equals(FieldNames.NoteDateFieldName)) ||
        //        (iControl.Name.Equals(FieldNames.NoteTimeFieldName)))
        //    {
        //        iControl.FieldCategory = FieldCategories.NoteDateOrTime;
        //    }

        //    if ((iControl.Name.Equals(FieldNames.RecCreationDateFieldName)) ||
        //        (iControl.Name.Equals(FieldNames.RecCreationTimeFieldName)))
        //    {
        //        iControl.FieldCategory = FieldCategories.RecCreationDateOrTime;
        //    }

        //    // This one needs to be case-insensitive?
        //    if (string.Compare(iControl.Name, FieldNames.DetailRecNoFieldName, true) == 0)
        //        iControl.KnownField = KnownFields.DetailRecNo;

        //    if (string.Compare(iControl.Name, FieldNames.LocMeterFieldName, true) == 0)
        //        iControl.KnownField = KnownFields.LocMeter;

        //    if (string.Compare(iControl.Name, FieldNames.BtnReadReinoFieldName, true) == 0)
        //        iControl.KnownField = KnownFields.BtnReadReino;

        //    if ((iControl.Name.Equals(FieldNames.VehLicNoFieldName)) ||
        //        (iControl.Name.Equals(FieldNames.VehLicStateFieldName)))
        //    {
        //        iControl.FieldCategory = FieldCategories.PlateOrState;
        //    }

        //    if ((iControl.Name.Equals(FieldNames.SuspAddrStreetNoFieldName)) ||
        //        (iControl.Name.Equals(FieldNames.SuspAddrStreetFieldName)) ||
        //        (iControl.Name.Equals(FieldNames.ROAddrStreetNoFieldName)) ||
        //        (iControl.Name.Equals(FieldNames.ROAddrStreetFieldName)))
        //    {
        //        iControl.FieldCategory = FieldCategories.SuspBlockOrStreet;
        //    }

        //    if ((iControl.Name.Equals(FieldNames.SigRequiredNoFieldName)) ||
        //        (iControl.Name.Equals(FieldNames.MisdemeanorFieldName)))
        //    {
        //        iControl.FieldCategory = FieldCategories.SigReqOrMisdemeanor;
        //    }

        //    // Field is plate or state, OR its Done button of THotSheetForm
        //    if ((this.CfgForm is THotSheetForm) && (iControl.Name.Equals(BtnDoneName)))
        //        iControl.KnownField = KnownFields.SearchButton;

        //    if ((iControl.Name.StartsWith(FieldNames.VioCodeFieldName) == true) ||
        //        (iControl.Name.StartsWith(FieldNames.VioFineFieldName) == true))
        //    {
        //        iControl.FieldCategory = FieldCategories.VioCodeOrFine;
        //    }

        //    if (iControl.Name.StartsWith(FieldNames.IssueNoDisplayFieldName) == true)
        //        iControl.KnownField = KnownFields.IssueNoDisplay;

        //    if ((iControl.Name.StartsWith(FieldNames.VoidDateTimeFieldName) == true) ||
        //        (iControl.Name.StartsWith(FieldNames.ReissuedByFieldName) == true))
        //    {
        //        iControl.FieldCategory = FieldCategories.VoidOrReinstate;
        //    }

        //    if (iControl.Name.StartsWith(FieldNames.IsWarningFieldName) == true)
        //        iControl.KnownField = KnownFields.IsWarning;

        //    if (iControl.Name.StartsWith(FieldNames.MatchRecsFieldName) == true)
        //        iControl.KnownField = KnownFields.MatchRecs;

        //    if (iControl.Name.Equals(FieldNames.HotDispoFieldName))
        //        iControl.KnownField = KnownFields.HotDispo;

        //    if (iControl.Name.Equals(BtnDoneName))
        //        iControl.KnownField = KnownFields.BtnDone;
        //}
        #endregion

        #region General Issuance Support

        //private CiteStructLogic AssocCiteStructLogic
        //{
        //    get
        //    {
        //        // If the StructLogicObj is of class CiteStructLogic, then return it as such
        //        if (IssueStruct is TCiteStruct)
        //        {
        //            TCiteStruct CiteStruct = IssueStruct as TCiteStruct;
        //            if ((CiteStruct.StructLogicObj != null) && (CiteStruct.StructLogicObj is CiteStructLogic))
        //                return CiteStruct.StructLogicObj as CiteStructLogic;
        //        }
        //        // StructLogicObj is not the desired class type, so return null
        //        return null;
        //    }
        //}

        //private ReissueStructLogic AssocReissueStructLogic
        //{
        //    get
        //    {
        //        // If the StructLogicObj is of class ReissueStructLogic, then return it as such
        //        if (IssueStruct is TReissueStruct)
        //        {
        //            TReissueStruct ReissueStruct = IssueStruct as TReissueStruct;
        //            if ((ReissueStruct.StructLogicObj != null) && (ReissueStruct.StructLogicObj is ReissueStructLogic))
        //                return ReissueStruct.StructLogicObj as ReissueStructLogic;
        //        }
        //        // StructLogicObj is not the desired class type, so return null
        //        return null;
        //    }
        //}

        //private IssueStructLogic AssocIssueStructLogic
        //{
        //    get
        //    {
        //        // If the StructLogicObj is of class IssueStructLogic (or descendant), then return it as such
        //        if ((IssueStruct.StructLogicObj != null) && (IssueStruct.StructLogicObj is IssueStructLogic))
        //            return IssueStruct.StructLogicObj as IssueStructLogic;
        //        // StructLogicObj is not the desired class type, so return null
        //        return null;
        //    }
        //}

        //private SearchStructLogic AssocSearchStructLogic
        //{
        //    get
        //    {
        //        // If the StructLogicObj is of class SearchStructLogic (or descendant), then return it as such
        //        if ((IssueStruct.StructLogicObj != null) && (IssueStruct.StructLogicObj is SearchStructLogic))
        //            return IssueStruct.StructLogicObj as SearchStructLogic;
        //        // StructLogicObj is not the desired class type, so return null
        //        return null;
        //    }
        //}

        //private PublicContactStructLogic AssocPublicContactStructLogic
        //{
        //    get
        //    {
        //        // If the StructLogicObj is of class PublicContactStructLogic (or descendant), then return it as such
        //        if ((IssueStruct.StructLogicObj != null) && (IssueStruct.StructLogicObj is PublicContactStructLogic))
        //            return IssueStruct.StructLogicObj as PublicContactStructLogic;
        //        // StructLogicObj is not the desired class type, so return null
        //        return null;
        //    }
        //}

        //private void MainTimer_Tick(object sender, EventArgs e)
        //{
        //    // If not entering a record, don't change the time
        //    if ((this.fFormEditMode == EditRestrictionConsts.femView) ||
        //        (this.CfgForm is TIssueSelectForm))
        //        return;

        //    // If the record has already printed or been saved, don't change the time
        //    if (GetFormPrinted() || GetFormSaved() ||
        //        (((fFormEditMode & EditRestrictionConsts.femReissueAttr) > 0) &&
        //        TClientDef.GlobalClientDef.FreezeDateInReissueMode))
        //        return;

        //    int CfgCtrlIndex = -1;

        //    // Do we have an IssueDate control that needs to be updated?
        //    if (IssueDateCtrl != null)
        //    {
        //        string CurrentDateStr = "";
        //        ReinoControls.TextBoxBehavior.OSDateToDateString(DateTime.Today, IssueDateCtrl.Behavior.GetEditMask(), ref CurrentDateStr);
        //        if ((IssueDateCtrl.Behavior.EditCtrl != null) && (IssueDateCtrl.Behavior.EditCtrl.Visible == true))
        //        {
        //            IssueDateCtrl.Behavior.EditCtrl.Text = CurrentDateStr;
        //        }
        //        else
        //        {
        //            // Set the edit buffer with the passed text. This also merges with the edit mask,
        //            // so the final edit buffer may differ.
        //            IssueDateCtrl.Behavior.SetEditBuffer(CurrentDateStr, true);
        //            // Now set the real text to match the edit buffer
        //            IssueDateCtrl.Behavior.SetText(IssueDateCtrl.Behavior.EditBuffer, ReinoControls.WinControl_SetText_Options.sto_ReplaceText, 0, true);
        //        }

        //        // If the Navigation panel is visible, repaint the IssueDate if its displayed
        //        if (this.NavigationPanel.Visible == true)
        //        {
        //            CfgCtrlIndex = GetCfgCtrlIndex(IssueDateCtrl.Behavior.CfgCtrl);
        //            vlbFields.RepaintItemIfShown(CfgCtrlIndex);
        //        }
        //    }

        //    // Do we have an IssueTime control that needs to be updated?
        //    if (IssueTimeCtrl != null)
        //    {
        //        string CurrentTimeStr = "";
        //        ReinoControls.TextBoxBehavior.OSTimeToTimeString(DateTime.Now, IssueTimeCtrl.Behavior.GetEditMask(), ref CurrentTimeStr);
        //        if ((IssueTimeCtrl.Behavior.EditCtrl != null) && (IssueTimeCtrl.Behavior.EditCtrl.Visible == true))
        //        {
        //            IssueTimeCtrl.Behavior.EditCtrl.Text = CurrentTimeStr;
        //        }
        //        else
        //        {
        //            // Set the edit buffer with the passed text. This also merges with the edit mask,
        //            // so the final edit buffer may differ.
        //            IssueTimeCtrl.Behavior.SetEditBuffer(CurrentTimeStr, true);
        //            // Now set the real text to match the edit buffer
        //            IssueTimeCtrl.Behavior.SetText(IssueTimeCtrl.Behavior.EditBuffer, ReinoControls.WinControl_SetText_Options.sto_ReplaceText, 0, true);
        //        }

        //        // If the Navigation panel is visible, repaint the IssueTime if its displayed
        //        if (this.NavigationPanel.Visible == true)
        //        {
        //            CfgCtrlIndex = GetCfgCtrlIndex(IssueTimeCtrl.Behavior.CfgCtrl);
        //            vlbFields.RepaintItemIfShown(CfgCtrlIndex);
        //        }
        //    }

        //    // Do we have a RecCreationDate control that needs to be updated?
        //    if (RecCreationDateCtrl != null)
        //    {
        //        string CurrentDateStr = "";
        //        ReinoControls.TextBoxBehavior.OSDateToDateString(DateTime.Today, RecCreationDateCtrl.Behavior.GetEditMask(), ref CurrentDateStr);
        //        if ((RecCreationDateCtrl.Behavior.EditCtrl != null) && (RecCreationDateCtrl.Behavior.EditCtrl.Visible == true))
        //        {
        //            RecCreationDateCtrl.Behavior.EditCtrl.Text = CurrentDateStr;
        //        }
        //        else
        //        {
        //            // Set the edit buffer with the passed text. This also merges with the edit mask,
        //            // so the final edit buffer may differ.
        //            RecCreationDateCtrl.Behavior.SetEditBuffer(CurrentDateStr, true);
        //            // Now set the real text to match the edit buffer
        //            RecCreationDateCtrl.Behavior.SetText(RecCreationDateCtrl.Behavior.EditBuffer, ReinoControls.WinControl_SetText_Options.sto_ReplaceText, 0, true);
        //        }

        //        // If the Navigation panel is visible, repaint the RecCreationDate if its displayed
        //        if (this.NavigationPanel.Visible == true)
        //        {
        //            CfgCtrlIndex = GetCfgCtrlIndex(RecCreationDateCtrl.Behavior.CfgCtrl);
        //            vlbFields.RepaintItemIfShown(CfgCtrlIndex);
        //        }
        //    }

        //    // Do we have a RecCreationTime control that needs to be updated?
        //    if (RecCreationTimeCtrl != null)
        //    {
        //        string CurrentTimeStr = "";
        //        ReinoControls.TextBoxBehavior.OSTimeToTimeString(DateTime.Now, RecCreationTimeCtrl.Behavior.GetEditMask(), ref CurrentTimeStr);
        //        if ((RecCreationDateCtrl.Behavior.EditCtrl != null) && (RecCreationDateCtrl.Behavior.EditCtrl.Visible == true))
        //        {
        //            RecCreationTimeCtrl.Behavior.EditCtrl.Text = CurrentTimeStr;
        //        }
        //        else
        //        {
        //            // Set the edit buffer with the passed text. This also merges with the edit mask,
        //            // so the final edit buffer may differ.
        //            RecCreationTimeCtrl.Behavior.SetEditBuffer(CurrentTimeStr, true);
        //            // Now set the real text to match the edit buffer
        //            RecCreationTimeCtrl.Behavior.SetText(RecCreationTimeCtrl.Behavior.EditBuffer, ReinoControls.WinControl_SetText_Options.sto_ReplaceText, 0, true);
        //        }

        //        // If the Navigation panel is visible, repaint the RecCreationTime if its displayed
        //        if (this.NavigationPanel.Visible == true)
        //        {
        //            CfgCtrlIndex = GetCfgCtrlIndex(RecCreationTimeCtrl.Behavior.CfgCtrl);
        //            vlbFields.RepaintItemIfShown(CfgCtrlIndex);
        //        }
        //    }

        //    // Update pending attachments if the field is visible in either entry panel or navigation panel
        //    if ((this.EditPendingAttachments != null) && (this.AssocCiteStructLogic != null))
        //    {
        //        // First we need to determine if this field is currently displayed 
        //        bool loNeedToUpdate = false;
        //        if ((EditPendingAttachments.Behavior.EditCtrl != null) && (EditPendingAttachments.Behavior.EditCtrl.Visible == true))
        //            loNeedToUpdate = true;
        //        if (this.NavigationPanel.Visible == true)
        //        {
        //            CfgCtrlIndex = GetCfgCtrlIndex(EditPendingAttachments.Behavior.CfgCtrl);
        //            if (this.vlbFields.IsItemShowing(CfgCtrlIndex) == true)
        //                loNeedToUpdate = true;
        //        }

        //        // Now get updated info if the field is displayed
        //        if (loNeedToUpdate == true)
        //        {
        //            // UpdatePendingAttachments will return 1 if the text value is different.
        //            // We can use this info to prevent unwanted repaints
        //            if (this.AssocCiteStructLogic.UpdatePendingAttachments() == 1)
        //            {
        //                // Edit control was updated by above, so we just need to  update the vlbFields item 
        //                if (this.NavigationPanel.Visible == true)
        //                    vlbFields.RepaintItemIfShown(CfgCtrlIndex);
        //            }
        //        }
        //    }
        //}

        //private void HandleCursorBlinkTimerEvent()
        //{
        //    // Simply call the main timer event. 
        //    // (This takes care of things such as updating date and time fields)
        //    MainTimer_Tick(this.WinForm, new EventArgs());
        //}

        /// <summary>
        /// Global instance of "Initializing..." dialog. We'll keep this one created to speed things 
        /// up since its a commonly displayed window.
        /// </summary>
       // private static AppMessageBox glInitializingDlg = null;

//        public virtual short PrepareForEdit()
//        {
//            // DEBUG -- Do we see a performance gain by bumping up the thread priority temporarily?
//            Thread.CurrentThread.Priority = ThreadPriority.Highest;

//            // Show a temporary dialog to let user know we're busy
//            if (glInitializingDlg == null)
//            {
//                glInitializingDlg = AppMessageBox.ShowMessageNotModal("", "Initializing...", null);
//            }
//            else
//            {
//                glInitializingDlg.Visible = true;
//                glInitializingDlg.Show();
//                glInitializingDlg.Refresh();
//                IssueAppImp.ApplicationDoEvents();
//            }

//            // Make sure items are removed from entry panel and force rebuild next time
//            // entry panel is displayed (This is to prevent button from appearing on wrong page)
//            MustExecuteSetActiveCfgCtrl = true;
//            for (int loIdx = this.EntryPanelControls.Count - 1; loIdx >= 0; loIdx--)
//            {
//                if (this.EntryPanelControls[loIdx] != null)
//                    this.EntryPanelControls[loIdx].Visible = false;
//                this.EntryPanelControls.RemoveAt(loIdx);
//            }

//            // Do some one time visual stuff
//            if (_AlreadyDisplayedForm == false)
//            {
//                // Make sure field list is initialized for its items
//                vlbFields.ResizeSuspended = false;
//                vlbFields.EndUpdate();

//                // Buttons may need to be prepared before painting to reduce flicker.
//                // (ie. button region and offscreen buffer will be created if necessary)
//                BtnPrevField.PrepForPaint();
//                BtnNextField.PrepForPaint();
//                NavigateEscBtn.PrepForPaint();
//                NavigateOKBtn.PrepForPaint();
//                EntryFieldsBtn.PrepForPaint();
//                foreach (ReinoNavButton loNavBtn in SectionButtons)
//                {
//                    loNavBtn.PrepForPaint();
//                }

//                // Now set flag so we know this has already been done
//                _AlreadyDisplayedForm = true;
//            }

//            // Set flag indicating we can't skip the SetActiveCfgCtrl routine
//            this.MustExecuteSetActiveCfgCtrl = true;

//            short loStatus;
//            bool loMagDataStuffed = false;

//            ClearFormEditAttrs();
//            fMagStripeUsed = false;

//            // Default the Next/Prev buttons to enabled state
//            BtnNextField.Enabled = true;
//            BtnPrevField.Enabled = true;

//            // If its a search match form, set the form caption
//            if (this.CfgForm is TSearchMatchForm)
//            {
//                string loFormCaption = string.Format("{0:d} Matching {1:s} Records",
//                    this.AssocSearchStructLogic.MatchRecs.Behavior.ListSourceTable.GetRecCount(),
//                    this.AssocIssueStructLogic.IssueStruct.Name);
//                SetCaption(loFormCaption);
//            }

//            // Reset flags used for keeping track of the Auto-Attached multimedia notes status.
//            // However we can't reset them if currently inside the AutoAttach procedure!
//            // Also, these flags only apply to IssueForms with the Notes button
//            if ((fInsideAutoAttachProcedure == false) &&
//                 ((this.CfgForm is TNotesForm) == false) &&
//                 (BtnNotes != null) &&
//                 (AssocCiteStructLogic != null))
//            {
//                fDidAutoAttachProcedure = false;
//                fInsideAutoAttachProcedure = false;
//            }

//            if (SetIssueNoFields() == false)
//            {
//                glInitializingDlg.Close();
//                /*
//                if (loMsgForm != null)
//                    loMsgForm.Dispose();
//                */

//                // DEBUG -- Reset thread priority back to normal
//                Thread.CurrentThread.Priority = ThreadPriority.Normal;

//                return -1;
//            }

//            if ((loStatus = (short)IssueStruct.MainTable.MakeFileSpaceForRecord()) < 0)
//            {
//                string loLine0 = string.Format("No room in table {0:s}", IssueStruct.Name);
//                AppMessageBox.ShowMessageWithBell(loLine0, AssocIssueStructLogic.TranslateFileErrorCode2(loStatus), "");

//                // DEBUG -- Reset thread priority back to normal
//                Thread.CurrentThread.Priority = ThreadPriority.Normal;

//                return loStatus;
//            }

//            // This will initialize IssueDate and Time fields
//            HandleCursorBlinkTimerEvent();

//            // Set flag so other routines know they are working in context of "FormInit"
//            _InsidePrepareForEdit = true;
//            _MustRebuildDisplayAfterFormInit = false;

//            foreach (TTControl NextCfgCtrl in EntryOrder)
//            {
//                if (NextCfgCtrl.TypeOfWinCtrl() == typeof(ReinoTextBox))
//                {
//                    NextCfgCtrl.Behavior.InitToDefaultState();
//                    if ((fFormEditMode & Reino.ClientConfig.EditRestrictionConsts.femView) > 0)
//                        NextCfgCtrl.IsProtected = true;
//                }
//                else if (NextCfgCtrl.TypeOfWinCtrl() == typeof(ReinoControls.ReinoVirtualListBox))
//                {
//                    NextCfgCtrl.Behavior.InitToDefaultState();
//                    if ((fFormEditMode & Reino.ClientConfig.EditRestrictionConsts.femView) > 0)
//                        NextCfgCtrl.IsProtected = true;
//                }
//                else if (NextCfgCtrl.TypeOfWinCtrl() == typeof(ReinoControls.ReinoDrawBox))
//                {
//                    NextCfgCtrl.Behavior.InitToDefaultState();
//                    if ((fFormEditMode & Reino.ClientConfig.EditRestrictionConsts.femView) > 0)
//                        NextCfgCtrl.IsProtected = true;
//                }
//            }

//            // Clear flag now so OnRestrictionForcesDisplayRebuild() executes normally
//            _InsidePrepareForEdit = false;
//            if (_MustRebuildDisplayAfterFormInit == true)
//            {
//                // We explicity rebuild display pages further down in this routine, so
//                // lets only do it once for best speed
//                /*OnRestrictionForcesDisplayRebuild();*/
//                _MustRebuildDisplayAfterFormInit = false;
//            }

//            // initialize
//            loMagDataStuffed = false;

//            if ((fInsideAutoAttachProcedure == false) &&
//                 ((this.CfgForm is TNotesForm) == false) &&
//                 (BtnNotes != null) &&
//                 (AssocCiteStructLogic != null))
//            {
//                AssocCiteStructLogic.RefreshInitialMultimediaFilesList();
//            }

//#if WindowsCE || __ANDROID__
//#if PSTEnabled
//            // Are we supposed to use RadCom data from PST?
//            if (UsePSTRadComData == true)
//            {
//                // Stuff data from PST / RadCom
//                IssueAppImp.GlobalIssueApp.StuffPSTData(this);
//                // Reset flag so this doesn't happen again the next time
//                UsePSTRadComData = false;
//            }
//#endif //PSTEnabled
//#endif //WindowsCE

//            if (fSourceDataStruct != null)
//            {
//                // if there is a source struct, we want to get the swiped data FIRST....
//                // but only if we haven't already stuffed it
//                if ((!loMagDataStuffed) && (fUseSourceMagSwipe))
//                {
//                    // Process the data read from the MagCard
//                    loMagDataStuffed = (DriverLicenseDataUI.QueryUserChooseMagStripeDataDestination(this, true) == 1);
//                }
//                fUseSourceMagSwipe = false; // reset this

//                // and then take the parent form data SECOND...
//                // because anything we changed on the parent form, we want carried over
//                // into the issuance form- and this will override any changes
//                // in a) the infocop data (if used) and b) the swiped data
//                // resulting in the most recent edits from the parent struct being retained
//                if ((fSourceDataStruct.StructLogicObj != null) && (fSourceDataStruct.StructLogicObj is IssueStructLogic))
//                    ((IssueStructLogic)(fSourceDataStruct.StructLogicObj)).InitFormData(this);
//            }

//            // Enable the buttons as appropriate for current edit conditions.
//            SetButtonStates();

//            // Make sure virtual lists know how many items will be displayed
//            UpdateVirtualLists();

//            // Rebuild the information about which fields will be displayed together
//            RebuildDisplayPages();

//            // Close the "Initializing" window
//            glInitializingDlg.Close();
//            /*
//            if (loMsgForm != null)
//                loMsgForm.Dispose();
//            */

//            // DEBUG -- Reset thread priority back to normal
//            Thread.CurrentThread.Priority = ThreadPriority.Normal;

//            // JLA (3/9/07) Single display pages forms don't need "review/navigation" mode enabled
//            if (DisplayPages.Count == 1)
//            {
//                BtnNextField.Visible = false;
//                BtnPrevField.Visible = false;
//                EntryFieldsBtn.Text = "Exit";
//                EntryFieldsBtn.Width = 50;
//            }
//            else
//            {
//                BtnNextField.Visible = true;
//                BtnPrevField.Visible = true;
//                EntryFieldsBtn.Text = "Review"; // "Navigate";
//                EntryFieldsBtn.Width = 104;
//            }

//            return 0;
//        }

//        public bool SetIssueNoFields()
//        {
//            // This is only applicable for cite structures
//            if (!(IssueStruct is TCiteStruct))
//                return true;

//            // If this is a new entry and there is an issue number field, need to get next issue number 
//            if (((fFormEditMode & (EditRestrictionConsts.femReissue | EditRestrictionConsts.femContinuance |
//                EditRestrictionConsts.femNewEntry | EditRestrictionConsts.femIssueMoreAttr)) > 0) &&
//                 (IssueNoCtrl != null))
//            {
//                // Find the associated sequence object
//                TCiteStruct CiteStruct = IssueStruct as TCiteStruct;
//                TObjBasePredicate predicate = new TObjBasePredicate(CiteStruct.Sequence);
//                SequenceImp SeqObj =
//                    Reino.CommonLogic.SequenceManager.GlobalSequenceMgr.Sequences.Find(predicate.CompareByName_CaseInsensitive);

//                // Return false if there is no sequence to work with
//                if (SeqObj == null)
//                {
//                    AppMessageBox.ShowMessageWithBell("Could not find", "issue number sequence!", "");
//                    return false;
//                }

//                // Do we have an IssueNo field?
//                if (IssueNoCtrl != null)
//                {
//                    Int64 loIssueNo = SeqObj.GetNextNumber();
//                    // Did GetNextNumber fail?
//                    if (loIssueNo <= 0)
//                    {
//                        AppMessageBox.ShowMessageWithBell("No numbers available in", "issue number sequence!", "");
//                        return false;
//                    }
//                    IssueNoCtrl.Behavior.SetEditBufferAndPaint(Convert.ToString(loIssueNo));
//                }

//                // Do we have an IssueNoPfx field?
//                if (IssueNoPfxCtrl != null)
//                    IssueNoPfxCtrl.Behavior.SetEditBufferAndPaint(SeqObj.GetNextNumberPfx());

//                // Do we have an IssueNoSfx field?
//                if (IssueNoSfxCtrl != null)
//                    IssueNoSfxCtrl.Behavior.SetEditBufferAndPaint(SeqObj.GetNextNumberSfx());
//            }

//            return true;
//        }

//        public TTControl FindCfgControlByName(string SearchName)
//        {
//            // Use predicate to find the desired control
//            TObjBasePredicate predicate = new TObjBasePredicate(SearchName);
//            TTControl Result = EntryOrder.Find(predicate.CompareByName_CaseInsensitive);
//            return Result;
//        }

//        public int ReadFieldValuesFromForm(TBaseIssForm iForm, TTTable iDestTable)
//        {
//            TTControl loNextCtrl;
//            /*ReinoTextBox loEditCtrl = null;*/
//            TTableFldDef loField;
//            int loNdx;

//            iDestTable.ClearFieldValues();
//            int loLoopMax = iDestTable.fTableDef.GetFieldCnt();
//            for (loNdx = 0; loNdx < loLoopMax; loNdx++)
//            {
//                // Get next field and try to find an associated edit control
//                loField = iDestTable.fTableDef.GetField(loNdx);
//                loNextCtrl = FindCfgControlByName(loField.Name);
//                TextBoxBehavior loBehavior = null;
//                if ((loNextCtrl != null) && (loNextCtrl.Behavior != null))
//                    loBehavior = loNextCtrl.Behavior;

//                // If there's a non-blank edit control, grab its value
//                if ((loBehavior != null) && (!loBehavior.FieldIsBlank()))
//                {
//                    iDestTable.SetFormattedFieldData(loNdx, loBehavior.GetEditMask(), loBehavior.EditBuffer);
//                }
//                // Is it the Form Revision field?
//                else if ((loField.Name.Equals(FieldNames.FORMREVFieldName)) && (iForm is TIssForm))
//                {
//                    if (this.PrintPicture != null)
//                    {
//                        string loRevisionStr = this.PrintPicture.Revision.ToString();
//                        iDestTable.SetFormattedFieldData(loNdx, "999", loRevisionStr);
//                    }
//                    else
//                        iDestTable.SetFormattedFieldData(loNdx, "999", "0");
//                }
//                // Is it the Serial Number field?
//                else if ((loField.Name.Equals(FieldNames.HHSerialNoFieldName)))
//                {
//                    iDestTable.SetFormattedFieldData(loNdx, "", IssueAppImp.GlobalIssueApp.GetHHSerialNumber());
//                }
//                // Is it the Form Name field?
//                else if (loField.Name.Equals(FieldNames.FORMNAMEFieldName))
//                {
//                    if (iForm is TIssForm)
//                    {
//                        if (this.PrintPicture != null)
//                            iDestTable.SetFormattedFieldData(loNdx, "", this.PrintPicture.Name);
//                        else
//                            iDestTable.SetFormattedFieldData(loNdx, "", iForm.Name);
//                    }
//                    else
//                        iDestTable.SetFormattedFieldData(loNdx, "", iForm.Name);
//                }
//                else
//                    iDestTable.SetFormattedFieldData(loNdx, "", "");
//            }
//            return 0;
//        }

        public int GetFormEditMode()
        {
            return fFormEditMode;
        }

        protected virtual int GetRingBellVolume()
        {
            // For most forms, we'll just return -1 which indicates no specific volume needs to be set
            return -1;
        }

        public void SetFormEditMode(int iFormEditMode)
        {
            fFormEditMode = iFormEditMode;
        }

        public int GetFormEditAttrs()
        {
            return fFormEditAttrs;
        }

        public void SetFormEditAttr(int iAttribute, bool iSetAttr)
        {
            if (iSetAttr)
                fFormEditAttrs |= iAttribute;
            else
                fFormEditAttrs &= ~iAttribute;
        }

        public void ClearFormEditAttrs()
        {
            fFormEditAttrs = 0;
        }

        public short GetFormEditResult()
        {
            return fFormEditResult;
        }

        public bool GetFormPrinted()
        {
            return (fFormEditAttrs & EditRestrictionConsts.feaPrinted) != 0;
        }

        public void SetFormPrinted(bool iSetAttr)
        {
            SetFormEditAttr(EditRestrictionConsts.feaPrinted, iSetAttr);
        }

        public bool GetFormSaved()
        {
            return (fFormEditAttrs & EditRestrictionConsts.feaSaved) != 0;
        }

        public void SetFormSaved(bool iSetAttr)
        {
            SetFormEditAttr(EditRestrictionConsts.feaSaved, iSetAttr);
        }

        public bool GetFormIssueNoLogged()
        {
            return (fFormEditAttrs & EditRestrictionConsts.feaIssueNoLogged) != 0;
        }

        public void SetFormIssueNoLogged(bool iSetAttr)
        {
            SetFormEditAttr(EditRestrictionConsts.feaIssueNoLogged, iSetAttr);
        }

        public bool GetFormTempFileSaved()
        {
            return (fFormEditAttrs & EditRestrictionConsts.feaTempFileSaved) != 0;
        }

        public void SetFormTempFileSaved(bool iSetAttr)
        {
            SetFormEditAttr(EditRestrictionConsts.feaTempFileSaved, iSetAttr);
        }

        //public bool InMultiEntryAddMode()
        //{
        //    return (((fFormEditMode & EditRestrictionConsts.femIssueMultipleAttr) > 0) && (fMultipleEntryMgr.fMultiEntryMode == EMultiEntryMode.memAdding));
        //}

        //bool MultipleIssuanceOkToFinish()
        //{
        //    if ((fFormEditMode & EditRestrictionConsts.femIssueMultipleAttr) > 0)
        //    {
        //        if (fMultipleEntryMgr.fMultiEntryMode != EMultiEntryMode.memFinishing)
        //        {
        //            AppMessageBox.ShowMultiLineMessageWithBell("Press the End Multiple\r\n" +
        //                "button to finish this\r\n" + "group of multiple tickets", "");
        //            if (BtnEndIssueMultiple != null)
        //                BtnEndIssueMultiple.Focus();
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        private void GatherAssociatedFieldNames(ref List<string> iFldNamesForCurrPrnData, TTableFldDef iFldDef)
        {
            // If its not a virtual field def, we just add name of passed field to the list and exit.
            // If it is a virtual field, then we have to recurse the fields it contains.
            if (!(iFldDef is TTableVirtualFldDef))
            {
                iFldNamesForCurrPrnData.Add(iFldDef.Name);
                return;
            }
            else
            {
                // If the name of the virtual field matches the name of a true field, 
                // then we need to add the fieldname to the list
                int loFieldNo = this.IssueStruct.MainTable.GetFldNo(iFldDef.Name);
                if (loFieldNo >= 0)
                    iFldNamesForCurrPrnData.Add(iFldDef.Name);

                // Its a virtual field, so recurse all fields it is comprised of
                foreach (TTableFldDef loFld in ((TTableVirtualFldDef)(iFldDef)).Fields)
                {
                    if (loFld is TTableVirtualFldDef)
                        GatherAssociatedFieldNames(ref iFldNamesForCurrPrnData, loFld);
                    else
                        iFldNamesForCurrPrnData.Add(loFld.Name);
                }
            }
            // If we get this far, the passed field is fully processed, so exit
            return;
        }

        static internal Reino.ClientConfig.TTTable GetListTableByName(string TableName, ListSearchType iSearchType)
        {
            // Find the associated table definition. We'll look through agency lists fist, then 
            // through data structures.
            TObjBasePredicate predicate = new TObjBasePredicate(TableName);
            Reino.ClientConfig.TTableDef tableDef = null;

            foreach (Reino.ClientConfig.TAgList agencyList in TClientDef.GlobalClientDef.ListMgr.AgLists)
            {
                tableDef = agencyList.TableDefs.Find(predicate.CompareByName_CaseInsensitive);
                if (tableDef != null)
                {
                    if ((iSearchType == ListSearchType.NewIfAgencyOr1stData) ||
                        (iSearchType == ListSearchType.NewIfAgencyOr2ndData))
                    {
                        // Create a new instance of TTTable and add it to the TTableDef object
                        TTTable newTable = new TTTable();
                        newTable.SetTableName(TableName);
                        //tableDef.HighTableRevision.Tables.Add(newTable); // SetTableName adds it to the list
                        return newTable;
                    }
                    else if (iSearchType == ListSearchType.Get1st)
                    {
                        // Make sure we have enough tables, then return the 1st
                        while (tableDef.HighTableRevision.Tables.Count < 1)
                        {
                            // Create a new instance of TTTable and add it to the TTableDef object
                            TTTable newTable = new TTTable();
                            newTable.SetTableName(TableName); // This adds it to the list
                        }
                        return tableDef.HighTableRevision.Tables[0];
                    }
                    else if (iSearchType == ListSearchType.Get2nd)
                    {
                        // Make sure we have enough tables, then return the 2nd
                        while (tableDef.HighTableRevision.Tables.Count < 2)
                        {
                            // Create a new instance of TTTable and add it to the TTableDef object
                            TTTable newTable = new TTTable();
                            newTable.SetTableName(TableName); // This adds it to the list
                        }
                        return tableDef.HighTableRevision.Tables[1];
                    }
                    else
                    {
                        return tableDef.HighTableRevision.Tables[0];
                    }
                }
            }

            // Didn't find it in Agency Lists, so lets look at data structures now...
            // We won't add additional TTTable instances for writeable data structures
            foreach (Reino.ClientConfig.TIssStruct IssueStruct in TClientDef.GlobalClientDef.IssStructMgr.IssStructs)
            {
                tableDef = IssueStruct.TableDefs.Find(predicate.CompareByName_CaseInsensitive);
                if (tableDef != null)
                {
                    // We're not an agency list, so switch to Get2nd style if necessary
                    if (iSearchType == ListSearchType.NewIfAgencyOr2ndData)
                        iSearchType = ListSearchType.Get2nd;

                    if ((iSearchType == ListSearchType.NewIfAgencyOr1stData) ||
                        (iSearchType == ListSearchType.Get1st))
                    {
                        // Make sure we have enough tables, then return the 1st
                        while (tableDef.HighTableRevision.Tables.Count < 1)
                        {
                            // Create a new instance of TTTable and add it to the TTableDef object
                            TTTable newTable = new TTTable();
                            newTable.SetTableName(TableName); // This adds it to the list
                        }
                        return tableDef.HighTableRevision.Tables[0];
                    }
                    else if (iSearchType == ListSearchType.Get2nd)
                    {
                        // Make sure we have enough tables, then return the 2nd
                        while (tableDef.HighTableRevision.Tables.Count < 2)
                        {
                            // Create a new instance of TTTable and add it to the TTableDef object
                            TTTable newTable = new TTTable();
                            newTable.SetTableName(TableName); // This adds it to the list
                        }
                        return tableDef.HighTableRevision.Tables[1];
                    }
                    else
                    {
                        return tableDef.HighTableRevision.Tables[0];
                    }
                }
            }

            // Couldn't find it, so return null
            return null;
        }

        //private bool DoHotSheetSearch(TER_SearchHotSheet EditRestrict)
        //{
        //    THotSheetStruct loHotSheetStruct = GetHotSheetStruct(EditRestrict.CharParam);
        //    if (loHotSheetStruct != null)
        //    {
        //        return ((SearchStructLogic)(loHotSheetStruct.StructLogicObj)).PerformSearchAndIssue(
        //            this.CfgForm, true, 1, EditRestrict.MatchFieldsName, EditRestrict, true);
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //private void DoHotSheetFilter(TER_HotsheetFilter EditRestrict)
        //{
        //    TTTable loTable = null;
        //    TTTable loParentTable = null;
        //    bool loWirelessHostAvailable = false;

        //    // Because of the way wireless & local searches interact, it is possible
        //    // for a wireless result to be processed while the local search is processed.
        //    // The wireless result will call FinishEnforceRestriction prior to returnning from
        //    // PerformSearch.  If this happens, we need to ignore the PerformSearch results
        //    // because the wireless results are to be kept and used. 
        //    EditRestrict.fPreemptedByWireless = false;

        //    // Reset the table to the local (not wireless version)
        //    EditRestrict.GetParent().ListSourceTable = EditRestrict.GetParent().LocalListSourceTable;
        //    if (EditRestrict.GetParent() != null)
        //    {
        //        EditRestrict.GetParent().ListItemCount = EditRestrict.GetParent().ListSourceTable.GetRecCount();
        //        EditRestrict.GetParent().ListItemCache.Clear();
        //        if (EditRestrict.GetParent().GridDisplayCache != null)
        //            EditRestrict.GetParent().GridDisplayCache.Clear();
        //        if (EditRestrict.GetParent().ListBox != null)
        //            EditRestrict.GetParent().ListBox.RefreshItems(false);
        //    }

        //    // Make sure we have an associated table and it is the same as the search table
        //    loParentTable = EditRestrict.GetParent().ListSourceTable;
        //    if (loParentTable == null)
        //        return;

        //    // Find the hotsheet struct
        //    THotSheetStruct loHotSheetStruct = GetHotSheetStruct(loParentTable.fTableDef.Name);
        //    if (loHotSheetStruct == null)
        //    {
        //        AppMessageBox.ShowMessageWithBell("TER_HotsheetFilter: No hotsheet named " + loParentTable.fTableDef.Name, "", "");
        //        return;
        //    }

        //    loTable = ((SearchStructLogic)(loHotSheetStruct.StructLogicObj)).PerformSearch(
        //        this.CfgForm, true, 1, EditRestrict.MatchFieldsName, false,
        //        EditRestrict, ref loWirelessHostAvailable, true);

        //    // It is possible the wireles results have now been processed. 
        //    // If so, do not allow the local results to erase them.
        //    if (EditRestrict.fPreemptedByWireless)
        //        return;

        //    if (loTable == null)
        //    {
        //        // This needs to set a filter the excludes all permits?
        //        loParentTable.RemoveAllFilters();
        //        loParentTable.ActivateFilters();
        //        return;
        //    }

        //    // Have a result, paste it to parent edit
        //    loParentTable.CopyFilters(loTable.GetFilter());
        //    if (EditRestrict.GetParent() != null)
        //    {
        //        EditRestrict.GetParent().ListItemCount = loParentTable.GetRecCount();
        //        EditRestrict.GetParent().ListItemCache.Clear();
        //        if (EditRestrict.GetParent().GridDisplayCache != null)
        //            EditRestrict.GetParent().GridDisplayCache.Clear();
        //        if (EditRestrict.GetParent().ListBox != null)
        //            EditRestrict.GetParent().ListBox.RefreshItems(false);
        //    }
        //    EditRestrict.GetParent().SetEditBufferAndPaint(ReinoControls.TextBoxBehavior.GetFieldDataFromTable(
        //        loParentTable, EditRestrict.GetParent().CfgCtrl.Name,
        //        loTable.GetCurRecNo(), EditRestrict.GetParent().GetEditMask()));
        //    return;
        //}

        private THotSheetStruct GetHotSheetStruct(string StructName)
        {
            foreach (TIssStruct NextStruct in TClientDef.GlobalClientDef.IssStructMgr.IssStructs)
            {
                if ((NextStruct is THotSheetStruct) && (string.Compare(NextStruct.Name, StructName, true) == 0))
                    return NextStruct as THotSheetStruct;
            }
            return null;
        }

        /// <summary>
        /// Calls each printable TTEdit's ValidateSelf routine. If a field fails validation, 
        /// will place the focus on it and display the error message.
        /// </summary>
        //int ValidatePrintedFields()
        //{
        //    int loResult;
        //    int loFieldNo = -1;
        //    string loErrMsg = "";
        //    List<string> FldNamesForCurrPrnData = new List<string>();

        //    // Loop through all print elements in the print picture
        //    int loLoopMax = this.PrintPicture.AllPrnDataElements.Count;
        //    for (int loNdx = 0; loNdx < loLoopMax; loNdx++)
        //    {
        //        // Get next print element
        //        TWinBasePrnData loPrnData = this.PrintPicture.AllPrnDataElements[loNdx];
        //        if (loPrnData == null)
        //            continue;

        //        // Find main field associated with current PrnData element
        //        if ((loFieldNo = this.IssueStruct.MainTable.GetFldNo(loPrnData.Name)) < 0)
        //            continue;
        //        TTableFldDef loDBFld = this.IssueStruct.MainTable.HighTableRevision.Fields[loFieldNo];

        //        // Get list of all field names associated with current PrnData element
        //        // (There can be more than 1 associated field if we're dealing with a virtual field)
        //        FldNamesForCurrPrnData.Clear();
        //        GatherAssociatedFieldNames(ref FldNamesForCurrPrnData, loDBFld);

        //        // Now loop for every fieldname we have in the list (Normally there will only be 1)
        //        foreach (string loFieldName in FldNamesForCurrPrnData)
        //        {
        //            // Try to find an associated edit field in the configuration
        //            TObjBasePredicate predicate = new TObjBasePredicate(loFieldName/*loPrnData.Name*/);
        //            TTControl MatchObj = EntryOrder.Find(predicate.CompareByName_CaseInsensitive);
        //            if ((MatchObj != null) && (MatchObj is TTEdit))
        //            {
        //                TTEdit CfgEdit = MatchObj as TTEdit;
        //                // don't validate DrawFields here
        //                if (CfgEdit.IsAClassMember(typeof(TDrawField)))
        //                    continue;
        //                if (CfgEdit.TypeOfWinCtrl() == typeof(ReinoTextBox))
        //                {
        //                    // Get reference to the associated behavior object
        //                    ReinoControls.TextBoxBehavior behavior = CfgEdit.Behavior;
        //                    // If it passes validation, we can move ahead
        //                    if ((loResult = behavior.ValidateSelf(ref loErrMsg)) == 0)
        //                        continue;
        //                    // Failed validation, so make this field the current one on
        //                    // the entry panel, then display the error message in a balloon.
        //                    // First we need to make sure the entry panel is visible.
        //                    this.HideNavigationPanel(); // NavigationPanel.Visible = false;
        //                    EntryPanel.Visible = true;
        //                    EntryPanel.BringToFront();
        //                    this.SetActiveCfgCtrl(CfgEdit);
        //                    behavior.DisplayBalloonMsg(loErrMsg);
        //                    return -1;
        //                }
        //            }
        //        }
        //    }

        //    if ((loResult = ValidateDrawFields()) != 0)
        //        return loResult;

        //    return 0;
        //}

        /// <summary>
        /// Calls each TTEdit's ValidateSelf routine.  If a field fails validation, 
        /// will place the focus on it and display the error message.
        /// </summary>
        //int ValidateFormFields()
        //{
        //    int loResult;
        //    string loErrMsg = "";

        //    foreach (TTControl MatchObj in EntryOrder)
        //    {
        //        // don't validate DrawFields here
        //        if (MatchObj is TDrawField)
        //            continue;

        //        if (MatchObj is TTEdit)
        //        {
        //            TTEdit CfgEdit = MatchObj as TTEdit;
        //            if (CfgEdit.TypeOfWinCtrl() == typeof(ReinoTextBox))
        //            {
        //                // Get reference to the associated behavior object
        //                ReinoControls.TextBoxBehavior behavior = CfgEdit.Behavior;
        //                // If it passes validation, we can move ahead
        //                if ((loResult = behavior.ValidateSelf(ref loErrMsg)) == 0)
        //                    continue;
        //                // Failed validation, so make this field the current one on
        //                // the entry panel, then display the error message in a balloon.
        //                // First we need to make sure the entry panel is visible.
        //                this.HideNavigationPanel(); // NavigationPanel.Visible = false;
        //                EntryPanel.Visible = true;
        //                EntryPanel.BringToFront();
        //                this.SetActiveCfgCtrl(CfgEdit);
        //                behavior.DisplayBalloonMsg(loErrMsg);
        //                return -1;
        //            }
        //        }
        //    }

        //    if ((loResult = ValidateDrawFields()) != 0)
        //        return loResult;
        //    return 0;
        //}

        /// <summary>
        /// OfficerSignature is a draw field, and cannot receive focus directly.  Therefore, it cannot be validated by ValidateFormFields
        /// (which sets the focus to any field that has an error).
        /// </summary>
        //int ValidateDrawFields()
        //{
        //    int loResult;
        //    string loErrMsg = "";

        //    // only two supported draw fields, fDrawSuspSignature and fDrawOfficerSignature;
        //    if ((fDrawSuspSignature != null) && ((loResult = fDrawSuspSignature.Behavior.ValidateSelf(ref loErrMsg)) < 0))
        //    {
        //        // whoops! Display error message, then set focus to the SuspSignature button
        //        // Failed validation, so make this field the current one on
        //        // the entry panel, then display the error message in a balloon.
        //        // First we need to make sure the entry panel is visible.
        //        this.HideNavigationPanel(); // NavigationPanel.Visible = false;
        //        EntryPanel.Visible = true;
        //        EntryPanel.BringToFront();
        //        if (BtnSuspSignature != null)
        //        {
        //            this.SetActiveCfgCtrl(BtnSuspSignature.AssocCfgBtn);
        //            fDrawSuspSignature.Behavior.DisplayBalloonMsg("Error with Signature:\r\n" + loErrMsg);
        //            fDrawSuspSignature.Behavior.RepositionPopupBalloon(BtnSuspSignature);
        //            try { BtnSuspSignature.Focus(); }
        //            catch { }
        //        }
        //        else
        //        {
        //            this.SetActiveCfgCtrl(fDrawSuspSignature);
        //            fDrawSuspSignature.Behavior.DisplayBalloonMsg("Error with Signature:\r\n" + loErrMsg);
        //        }
        //        return -1;
        //    }

        //    if ((fDrawOfficerSignature != null) && ((loResult = fDrawOfficerSignature.Behavior.ValidateSelf(ref loErrMsg)) < 0))
        //    {
        //        // whoops! Display error message, then set focus to the SuspSignature button
        //        // Failed validation, so make this field the current one on
        //        // the entry panel, then display the error message in a balloon.
        //        // First we need to make sure the entry panel is visible.
        //        this.HideNavigationPanel(); // NavigationPanel.Visible = false;
        //        EntryPanel.Visible = true;
        //        EntryPanel.BringToFront();
        //        if (BtnOfficerSignature != null)
        //        {
        //            this.SetActiveCfgCtrl(BtnOfficerSignature.AssocCfgBtn);
        //            fDrawOfficerSignature.Behavior.DisplayBalloonMsg("Error with Signature:\r\n" + loErrMsg);
        //            fDrawOfficerSignature.Behavior.RepositionPopupBalloon(BtnOfficerSignature);
        //            try { BtnOfficerSignature.Focus(); }
        //            catch { }
        //        }
        //        else
        //        {
        //            this.SetActiveCfgCtrl(fDrawOfficerSignature);
        //            fDrawOfficerSignature.Behavior.DisplayBalloonMsg("Error with Signature:\r\n" + loErrMsg);
        //        }
        //        return -1;
        //    }
        //    return 0;
        //}

        /// <summary>
        /// Called after a form has been printed.
        /// For  all the fields in the printed form that exist in the entry form:
        /// Sets the printed attribute for the field, its dependents, and dependencies.
        /// (disables those fields)
        /// </summary>
//        private short MarkPrintedFields(bool iMarkVal)
//        {
//            int loFieldNo = -1;
//            List<string> FldNamesForCurrPrnData = new List<string>();

//            // Loop through all print elements in the print picture
//            int loLoopMax = this.PrintPicture.AllPrnDataElements.Count;
//            for (int loNdx = 0; loNdx < loLoopMax; loNdx++)
//            {
//                // Get next print element
//                TWinBasePrnData loPrnData = this.PrintPicture.AllPrnDataElements[loNdx];
//                if (loPrnData == null)
//                    continue;

//                // Find main field associated with current PrnData element
//                if ((loFieldNo = this.IssueStruct.MainTable.GetFldNo(loPrnData.Name)) < 0)
//                    continue;
//                TTableFldDef loDBFld = this.IssueStruct.MainTable.HighTableRevision.Fields[loFieldNo];

//                // Get list of all field names associated with current PrnData element
//                // (There can be more than 1 associated field if we're dealing with a virtual field)
//                FldNamesForCurrPrnData.Clear();
//                GatherAssociatedFieldNames(ref FldNamesForCurrPrnData, loDBFld);

//                // Now loop for every fieldname we have in the list (Normally there will only be 1)
//                foreach (string loFieldName in FldNamesForCurrPrnData)
//                {
//                    // Try to find an associated edit field in the configuration
//                    TObjBasePredicate predicate = new TObjBasePredicate(loFieldName/*loPrnData.Name*/);
//                    TTControl MatchObj = EntryOrder.Find(predicate.CompareByName_CaseInsensitive);
//                    if ((MatchObj != null) && (MatchObj is TTEdit))
//                    {
//                        TTEdit CfgEdit = MatchObj as TTEdit;
//                        if (CfgEdit.TypeOfWinCtrl() == typeof(ReinoTextBox))
//                        {
//                            // Mark the field as printed
//                            CfgEdit.Behavior.SetEditStatePrinted(iMarkVal);
//                            CfgEdit.IsEnabled = iMarkVal;
//                        }
//                    }
//                }
//            }
//            return 0;
//        }

//        int WriteRecordToFile(TBaseIssForm iForm)
//        {
//            int loStatus;
//            // Don't mess with activity log because it already filled the data before we got here
//            if ((iForm != null) && ((iForm is TActivityLogForm) == false))
//                ReadFieldValuesFromForm(iForm, IssueStruct.MainTable.HighTableRevision.Tables[0]);

//            if ((loStatus = IssueStruct.MainTable.HighTableRevision.Tables[0].WriteRecord()) < 0)
//                AppMessageBox.ShowMessageWithBell("Failed writing record.", AssocIssueStructLogic.TranslateFileErrorCode2(loStatus), "");

//            // Don't do wireless stuff for emulator anymore
//            bool DontWantWireless = false;
//#if !WindowsCE && !__ANDROID__ 
//            DontWantWireless = true;
//#endif

//            // if the save was good, and we're "wireless", we'll send the upload this record
//            if ((DontWantWireless == false) &&
//                ((this.IssueStruct.WirelessUploadEnabled == TIssStruct.TWirelessUploadType.wuWhenAvailable) &&
//                (loStatus >= 0)))
//            {
//                TAPDIImportRecordCommandRec loAPDIClientCommandRec;
//                // is this a detail record? Need to some key info from the parent so that we can be
//                // joined on the host side.
//                if (!(this.IssueStruct is TCiteDetailStruct))
//                {
//                    // Get copy of record buffer and make sure it ends with a CRLF
//                    string loTempRecBuf = this.IssueStruct.MainTable.fRecBuffer.ToString();
//                    if (loTempRecBuf.EndsWith("\r\n") == false)
//                        loTempRecBuf = loTempRecBuf + "\r\n";
//                    // Convert record buffer to byte array
//                    ASCIIEncoding encoder = new ASCIIEncoding();
//                    Byte[] RecBufferBytes = encoder.GetBytes(loTempRecBuf);
//                    loAPDIClientCommandRec =
//                        new TAPDIImportRecordCommandRec("", 0, "", this.IssueStruct.Name,
//                        this.IssueStruct.MainTable.HighTableRevision.Revision,
//                        RecBufferBytes, RecBufferBytes.Length, "");
//                }
//                else
//                {
//                    string loMasterKeyStr = "";
//                    string loParentKeyStr = "";
//                    string loAttachmentFileName = "";
//                    int loMasterKeyInt;
//                    int loSavedRecordNo;

//                    // get the master key from the form
//                    loMasterKeyInt = GetMasterKey();
//                    loMasterKeyStr = loMasterKeyInt.ToString();

//                    // save the previous record no
//                    loSavedRecordNo = (this.IssueStruct as TCiteDetailStruct).ParentStructObj.MainTable.GetCurRecNo();

//                    // get the master record from the parent table
//                    (this.IssueStruct as TCiteDetailStruct).ParentStructObj.MainTable.HighTableRevision.Tables[0].ReadRecord(loMasterKeyInt);

//                    // and extract the key string from it
//                    ((this.IssueStruct as TCiteDetailStruct).ParentStructObj.StructLogicObj as IssueStructLogic).BuildKeyString(ref loParentKeyStr);
//                    // restore the previous record in the table buffer, if there was one...
//                    if (loSavedRecordNo >= 0)
//                    {
//                        (this.IssueStruct as TCiteDetailStruct).ParentStructObj.MainTable.HighTableRevision.Tables[0].ReadRecord(loSavedRecordNo);
//                    }

//                    // Get the attachment file name (will be blank if the field doesn't exist)
//                    loAttachmentFileName = "";
//                    IssueStruct.MainTable.HighTableRevision.Tables[0].GetFormattedFieldData(
//                        FieldNames.MultimediaNoteFilenameFieldName, "", ref loAttachmentFileName);

//                    // Get copy of record buffer and make sure it ends with a CRLF
//                    string loTempRecBuf = this.IssueStruct.MainTable.fRecBuffer.ToString();
//                    if (loTempRecBuf.EndsWith("\r\n") == false)
//                        loTempRecBuf = loTempRecBuf + "\r\n";
//                    // Convert record buffer to byte array
//                    ASCIIEncoding encoder = new ASCIIEncoding();
//                    Byte[] RecBufferBytes = encoder.GetBytes(loTempRecBuf);
//                    loAPDIClientCommandRec =
//                        new TAPDIImportRecordCommandRec((this.IssueStruct as TCiteDetailStruct).ParentStructObj.Name,
//                        (this.IssueStruct as TCiteDetailStruct).ParentStructObj.MainTable.HighTableRevision.Revision,
//                        loParentKeyStr, this.IssueStruct.Name,
//                        this.IssueStruct.MainTable.HighTableRevision.Revision,
//                        RecBufferBytes, RecBufferBytes.Length, loAttachmentFileName);
//                }
//                // put the actual command to transmit in the out que
//                IssueAppImp.glWirelessQueue.CE_APDI_PutCommandInSendQue(loAPDIClientCommandRec);
//            }

//            return loStatus;
//        }

//        short SaveFormRecord()
//        {
//            if (ValidateFormFields() != 0)
//                return -1;

//            if (CleanUpPendingWirelessSearches(this.CfgForm, null, false) < 0)
//                return -1; // user wants to wait for pending searches to complete.

//            if ((fFormEditMode == EditRestrictionConsts.femView) || GetFormSaved())
//                return 0; // no need to save, already been saved. 

//            // Make sure IssueDate & Time are updated before saving
//            MainTimer_Tick(this.WinForm, new EventArgs());

//            WriteRecordToFile(this.CfgForm);
//            SetFormSaved(true);

//            // Now that the real file has been written, clean up the temp one
//            if (this.IssueStruct is TCiteStruct)
//                AssocCiteStructLogic.CleanUpRecoveryFile();
//            SetFormTempFileSaved(false);

//            // If this is a new entry and there is an issue number field, need to log this number as used 
//            // but we only need to log once, and it possible it was logged during printing, so 
//            // we must also check the status flag  09/30/04 ajw 
//            if ((IssueNoCtrl != null) && (!GetFormIssueNoLogged()))
//            {
//                Int64 loIssueNo = 0;
//                ReinoControls.TextBoxBehavior.StrTollInt(IssueNoCtrl.Behavior.EditBuffer, ref loIssueNo);
//                if (IssueStruct is TCiteStruct)
//                    this.AssocCiteStructLogic.GetSequence().LogUsedNumber(loIssueNo);
//                SetFormIssueNoLogged(true); // don't need to log again for 2nd copies
//            }

//            // If we were in reissue mode, save the reissued record
//            if ((fFormEditMode & EditRestrictionConsts.femReissueAttr) > 0)
//            {
//                if (AssocCiteStructLogic != null)
//                {
//                    AssocCiteStructLogic.SaveReissuedRec(fReissuedKey);
//                    fReissuedKey = 0;
//                }
//            }
//            return 0;
//        }

//        public short SetMasterKey(int iMasterKeyInt)
//        {
//            // Try to find the MASTERKEY edit field in the configuration
//            TObjBasePredicate predicate = new TObjBasePredicate(FieldNames.MasterKeyFieldName);
//            TTControl MatchObj = EntryOrder.Find(predicate.CompareByName_CaseInsensitive);
//            if ((MatchObj != null) && (MatchObj is TTEdit))
//            {
//                // set the masterkey field value to the passed value. 
//                TTEdit CfgEdit = MatchObj as TTEdit;
//                string loIntBuf = iMasterKeyInt.ToString();
//                if (CfgEdit.TypeOfWinCtrl() == typeof(ReinoTextBox))
//                    CfgEdit.Behavior.SetEditBufferAndPaint(loIntBuf);
//                else
//                    CfgEdit.WinCtrl.Text = loIntBuf;

//                // tell everybody fMasterKey has changed 
//                CfgEdit.Behavior.NotifyDependents(EditRestrictionConsts.dneParentDataChanged);
//            }
//            return 0;
//        }

//        public int GetMasterKey()
//        {
//            // Try to find the MASTERKEY edit field in the configuration
//            try
//            {
//                TObjBasePredicate predicate = new TObjBasePredicate(FieldNames.MasterKeyFieldName);
//                TTControl MatchObj = EntryOrder.Find(predicate.CompareByName_CaseInsensitive);
//                if ((MatchObj != null) && (MatchObj is TTEdit))
//                {
//                    // get the masterkey field value 
//                    TTEdit CfgEdit = MatchObj as TTEdit;
//                    if (CfgEdit.TypeOfWinCtrl() == typeof(ReinoTextBox))
//                        return Convert.ToInt32(CfgEdit.Behavior.EditBuffer);
//                    else
//                        return Convert.ToInt32(CfgEdit.WinCtrl.Text);
//                }
//                return -1;
//            }
//            catch
//            {
//                return -1;
//            }
//        }

        /// <summary>
        /// Routine to modify the value of the protected fExitForm property.  Setting to 1
        /// indicates that the Form should be exited.
        /// </summary>
        public void SetExitForm(short iExitForm)
        {
            fExitForm = iExitForm;
        }

        /// <summary>
        /// Routine to read the current value of the ExitForm property.
        /// </summary>
        public short GetExitForm()
        {
            return fExitForm;
        }

        /// <summary>
        /// Returns true if state info changed
        /// </summary>
        //protected bool SetButtonVisibility(ReinoNavButton iBtn, bool iVisible)
        //{
        //    bool loResult = false;
        //    if ((iBtn.Visible != iVisible) ||
        //        ((iBtn.AssocCfgBtn != null) && (iBtn.AssocCfgBtn.Visible != iVisible)))
        //        loResult = true;
        //    // Set button visibility, but only if its in the "DisplayedButtons" list
        //    if (this.DisplayedButtons.IndexOf(iBtn) >= 0)
        //    {
        //        if (iBtn.Visible != iVisible)
        //            iBtn.Visible = iVisible;
        //    }
        //    // Also set state-holder in associated configuration object
        //    if (iBtn.AssocCfgBtn != null)
        //        iBtn.AssocCfgBtn.Visible = iVisible;
        //    return loResult;
        //}

        ///// <summary>
        ///// Returns true if state info changed
        ///// </summary>
        //protected bool SetButtonEnabled(ReinoNavButton iBtn, bool iEnabled)
        //{
        //    bool loResult = false;
        //    bool loAltResult = false;
        //    if ((iBtn.Enabled != iEnabled) ||
        //        ((iBtn.AssocCfgBtn != null) && (iBtn.AssocCfgBtn.Enabled != iEnabled)))
        //        loResult = true;
        //    // Set button enabled state
        //    if (iBtn.Enabled != iEnabled)
        //        iBtn.Enabled = iEnabled;
        //    // Also set state-holder in associated configuration object
        //    if (iBtn.AssocCfgBtn != null)
        //        iBtn.AssocCfgBtn.Enabled = iEnabled;

        //    // If we're enabled, then we must be visible too
        //    if ((iEnabled == true) && (iBtn.Visible == false))
        //        loAltResult = SetButtonVisibility(iBtn, true);

        //    if ((loResult == true) || (loAltResult == true))
        //        return true;
        //    else
        //        return false;
        //}

        /// <summary>
        /// Sets the enabled property of the buttons to an appropriate value for the current
        /// form conditions.
        /// </summary>
        //internal virtual void SetButtonStates()
        //{
        //    // All post-entry functions are now available from the select screen.  In order to
        //    //  simplify the interface, all buttons will be hidden while in view mode.  That
        //    //  way there will not be two paths to each of the functions. 
        //    if (fFormEditMode == EditRestrictionConsts.femView)
        //    {
        //        if (BtnPrint != null) SetButtonVisibility(BtnPrint, false);
        //        if (BtnIssueMore != null) SetButtonVisibility(BtnIssueMore, false);
        //        if (BtnIssueMultiple != null) SetButtonVisibility(BtnIssueMultiple, false);
        //        if (BtnEndIssueMultiple != null) SetButtonVisibility(BtnEndIssueMultiple, false);
        //        if (BtnCorrection != null) SetButtonVisibility(BtnCorrection, false);
        //        if (BtnCancel != null) SetButtonVisibility(BtnCancel, false);
        //        if (BtnVoid != null) SetButtonVisibility(BtnVoid, false);
        //        if (BtnReissue != null) SetButtonVisibility(BtnReissue, false);
        //        if (BtnDone != null) SetButtonVisibility(BtnDone, false);
        //        if (BtnNotes != null) SetButtonVisibility(BtnNotes, false);
        //        if (BtnIssueChild != null) SetButtonVisibility(BtnIssueChild, false);
        //        if (BtnSuspSignature != null) SetButtonVisibility(BtnSuspSignature, false);
        //        if (BtnOfficerSignature != null) SetButtonVisibility(BtnOfficerSignature, false);
        //        if (BtnReadReino != null) SetButtonVisibility(BtnReadReino, false);
        //        return;
        //    }

        //    // print button is disabled only if there is no print picture.
        //    if (BtnPrint != null)
        //    {
        //        SetButtonVisibility(BtnPrint, true);
        //        SetButtonEnabled(BtnPrint, ((PrintPicture != null) && (PrintPicture.Height > 0) && (!InMultiEntryAddMode())));
        //    }

        //    // done and notes buttons always enabled..
        //    if (BtnDone != null) SetButtonEnabled(BtnDone, true);
        //    // ..not any more
        //    if (BtnNotes != null) SetButtonEnabled(BtnNotes, !InMultiEntryAddMode());

        //    if (BtnIssueChild != null) SetButtonEnabled(BtnIssueChild, true);
        //    if (BtnReadReino != null) SetButtonEnabled(BtnReadReino, true);

        //    // absolutely no IssueMore while IssuingMultiple
        //    if (BtnIssueMore != null) SetButtonEnabled(BtnIssueMore, !((fFormEditMode & EditRestrictionConsts.femIssueMultipleAttr) > 0));

        //    // EndIssueMultiple enabled when in add mode in multi entry
        //    if (BtnEndIssueMultiple != null) SetButtonEnabled(BtnEndIssueMultiple, InMultiEntryAddMode());
        //    // IssueMultiple enabled
        //    if (BtnIssueMultiple != null) SetButtonEnabled(BtnIssueMultiple, !((fFormEditMode & EditRestrictionConsts.femIssueMultipleAttr) > 0) || InMultiEntryAddMode());

        //    // IssueMultiple not allowed after record has been saved.
        //    if ((BtnIssueMultiple != null) && (GetFormSaved())) SetButtonEnabled(BtnIssueMultiple, false);

        //    // correction enabled if printed but not saved.
        //    if (BtnCorrection != null)
        //    {
        //        SetButtonVisibility(BtnCorrection, true);
        //        SetButtonEnabled(BtnCorrection, GetFormPrinted() && !GetFormSaved());
        //    }

        //    // cancel enabled if not saved
        //    if (BtnCancel != null)
        //    {
        //        SetButtonVisibility(BtnCancel, true);
        //        SetButtonEnabled(BtnCancel, !GetFormSaved());
        //    }

        //    // void enabled if not void and in view mode or form has printed.
        //    if (BtnVoid != null)
        //    {
        //        SetButtonVisibility(BtnVoid, true);
        //        SetButtonEnabled(BtnVoid, (!(AssocCiteStructLogic.CiteIsVoid(AssocIssueStructLogic.GetEditRecNo())) && GetFormPrinted() && !InMultiEntryAddMode()));
        //    }
        //    // Reissue under same constraints as void. Not any more. No reissue when issuing multi.
        //    if (BtnReissue != null)
        //    {
        //        SetButtonVisibility(BtnReissue, true);
        //        SetButtonEnabled(BtnReissue, (!(AssocCiteStructLogic.CiteIsReissued(AssocIssueStructLogic.GetEditRecNo())) && GetFormPrinted() && !((fFormEditMode & EditRestrictionConsts.femIssueMultipleAttr) > 0)));
        //    }

        //    // Suspect Signature disabled if form is printed and post print signatures are disabled
        //    if (BtnSuspSignature != null)
        //    {
        //        SetButtonVisibility(BtnSuspSignature, true);
        //        SetButtonEnabled(BtnSuspSignature, (!this.CfgIssForm.PreventPostPrintSignatures || !GetFormPrinted()) && !GetFormSaved());
        //    }
        //    // Officer Signature disabled if form is printed
        //    if (BtnOfficerSignature != null)
        //    {
        //        SetButtonVisibility(BtnOfficerSignature, true);
        //        SetButtonEnabled(BtnOfficerSignature, !GetFormPrinted() && !GetFormSaved());
        //    }
        //}

        //// Loads the current record from fMultipleEntryMgr into the form and prepares to edit it.
        //int EditMultipleEntry(int iRecNo)
        //{
        //    if ((fFormEditMode & EditRestrictionConsts.femIssueMultiple) == 0)
        //        return 0; // IssueMultiple | IssueMore

        //    int loMoveForwardBy = iRecNo - fMultipleEntryMgr.CurrentRecNo();

        //    // transition from "adding" to "finishing" mode
        //    fMultipleEntryMgr.fMultiEntryMode = EMultiEntryMode.memFinishing;

        //    // load up the record
        //    if (fMultipleEntryMgr.MoveToRecord(iRecNo) != 0)
        //    {
        //        // we are out of multiple entry mode
        //        fMultipleEntryMgr.ClearAll();
        //        fFormEditMode &= ~EditRestrictionConsts.femIssueMultiple;
        //        SetCaption(fSavedCaption);
        //        AppMessageBox.ShowMessageWithBell("All tickets in multiple issuance", "mode have been completed.", "");
        //        return 0;
        //    }

        //    // load up the 1st record in the queue
        //    AssocIssueStructLogic.SetEditRecNo(AssocIssueStructLogic.GetEditRecNo() + loMoveForwardBy);

        //    string loCaption = string.Format("Multi Cites: Finish no. {0:d} of {1:d}", iRecNo + 1, fMultipleEntryMgr.RecordCount());
        //    SetCaption(loCaption);

        //    // if this is the first one, display message indicating that they are now finishing the tickets.
        //    if (iRecNo == 0)
        //    {
        //        loCaption = string.Format("the {0:d} tickets entered.", fMultipleEntryMgr.RecordCount());
        //        AppMessageBox.ShowMultiLineMessageWithBell("Finish Multi-Issued Tickets\r\n" +
        //            "You now need to complete\r\n" + loCaption, "");
        //    }
        //    else
        //    {
        //        // inform user that we are switching to a different record
        //        loCaption = string.Format("issuance record no. {0:d} of {1:d}.", iRecNo + 1, fMultipleEntryMgr.RecordCount());
        //        AppMessageBox.ShowMultiLineMessageWithBell("Returning to complete multiple\r\n" + loCaption, "");
        //    }

        //    fFormEditMode = EditRestrictionConsts.femNewEntry;
        //    // rather than exit, we will enter a new record. 
        //    fFormEditMode |= EditRestrictionConsts.femIssueMultiple; // IssueMultiple | IssueMore
        //    // mask out all but SingleEntry and IssueMore 
        //    fFormEditMode &= (EditRestrictionConsts.femIssueMultiple | EditRestrictionConsts.femSingleRecordAttr);

        //    PrepareForEdit();
        //    // suppress the first edit focus event
        //    fFormEditAttrs |= EditRestrictionConsts.feaEditedFirstField;

        //    // copy data to form from multi entry manager
        //    fMultipleEntryMgr.CopyRecordToForm(this);

        //    // protect printed fields
        //    MarkPrintedFields(true);

        //    // set focus to next control AFTER Issue Multiple button
        //    if (BtnEndIssueMultiple != null)
        //    {
        //        int FldIdx = EntryOrder.IndexOf(BtnEndIssueMultiple.AssocCfgBtn);
        //        if ((FldIdx >= 0) && (FldIdx < EntryOrder.Count - 1))
        //        {
        //            FldIdx++;
        //            this.SetActiveCfgCtrl(EntryOrder[FldIdx]);
        //        }
        //    }
        //    SetButtonStates();
        //    return 1;
        //}

        //private bool GetCancelReason()
        //{
        //    if (!(this.IssueStruct is TCiteStruct))
        //        return false;
        //    return (this.AssocCiteStructLogic.CancelRecord(this));
        //}

        //protected bool OkToExitForm(ref string oErrMsg, ref string oErrMsg2)
        //{
        //    if (fFormEditMode == EditRestrictionConsts.femView)
        //        return true; // can always escape when just viewing

        //    // If BtnIssueChild exists, they have to click Done so that they get routed through IssueChild code
        //    if ((BtnIssueChild != null) && (GetFormSaved()))
        //    {
        //        if (oErrMsg != null)
        //            oErrMsg = "Click Done to Exit.";
        //        return false;
        //    }

        //    // In IssueMultiple mode, Escaping behaves as follows:
        //    //  - It is only allowed while Adding records. Once in "Finishing" mode, they
        //    //    have to void the record.
        //    //  - Escaping will cancel the current record, and return them to the first record, placing them into "FinishingMode".
        //    if ((fFormEditMode & EditRestrictionConsts.femIssueMultipleAttr) > 0)
        //    {
        //        if (fMultipleEntryMgr.fMultiEntryMode != EMultiEntryMode.memAdding)
        //        {
        //            // not adding records, so fail
        //            if (oErrMsg != null)
        //                oErrMsg = "All records must be completed.";
        //            return false;
        //        }

        //        // Start the 2nd phase. Because this record wasn't saved, we need to manually set the record number back by one.
        //        AssocIssueStructLogic.SetEditRecNo(AssocIssueStructLogic.GetEditRecNo() - 1);
        //        EditMultipleEntry(0); // this sets the focus to btnSuspSignature, which is where we want to be
        //        // Fall through and capture the signature as normal
        //        if (oErrMsg != null)
        //            oErrMsg = "Returning to previous records.";
        //        return false;
        //    }

        //    if (!GetFormPrinted() || GetFormSaved())
        //    {
        //        // If the form has been saved, Esc=Done
        //        if (GetFormSaved())
        //            fFormEditResult = FormEditResults.FormEditResult_OK;

        //        if (this.CfgForm.PreventEsc == true)
        //        {
        //            if (oErrMsg != null)
        //            {
        //                oErrMsg = "Form cannot be exited this way.";
        //                oErrMsg2 = "(Entry must be completed and saved)";
        //            }
        //            return false;
        //        }

        //        // now we can see if we have any pending wireless searches	   
        //        if (CleanUpPendingWirelessSearches(this.CfgForm, null, false) == -1)
        //        {
        //            // they decided to wait...
        //            if (oErrMsg != null)
        //                oErrMsg = cnPreventExitNoMessage;
        //            return false;
        //        }
        //        else
        //        {
        //            // nothing to wait for, or they decided not to
        //            return true;
        //        }
        //    }
        //    if (oErrMsg != null)
        //    {
        //        oErrMsg = "Can't exit yet. Form has printed.";
        //        oErrMsg2 = "(Entry must be completed and saved)";
        //    }
        //    return false;
        //}

        //        private delegate void Close_BackThreadDelegate(object state);
        //Reino.CommonLogic.IssFormBuilder.FormEditCompleted
        //private Control FocusControlBeforeFormEdit = null;
        public delegate short FormEditCompleted(object state);
        FormEditCompleted FormEditOnCompletedCallback = null;
//        public virtual short FormEdit(int iFormEditMode, string iInitialFocusControl, FormEditCompleted completedDelegate)
//        {
//            FormEditOnCompletedCallback = completedDelegate;

//            // For better performance during bootup, lets "overclock" the CPU
//            IssueAppImp.ScaleCPUFrequency(true);

//#if WindowsCE || __ANDROID__
//            // If this is a citation structure, then set flag that we're issuing a ticket
//            if ((this.IssueStruct is Reino.ClientConfig.TCiteStruct) ||
//                (this.IssueStruct is Reino.ClientConfig.TPublicContactStruct))
//            {
//                IssueAppImp.GlobalIssueApp.SetInsideTicketMutex();
//            }
//#endif

//            // First of all, we need to update the item count for virtual listboxes
//            // associated with TEditListBox objects in the configuration
//            UpdateDataGrids();

//            // Auto-select 1st index if possible
//            if (this.CfgForm is TSearchMatchForm)
//            {
//                try { this.AssocSearchStructLogic.MatchRecs.Behavior.SetListNdxAndPaint(0); }
//                catch { }
//            }

//            // Get platform dependant object that we will use for showing the form.
//            // This is because on WindowsCE, we need to show the form via the ReinoApplicationEx 
//            // instead of normal method (so our message loop handling works for it)
//            ReinoControls.IPlatformDependent WinAPI;
//            if (Environment.OSVersion.Platform == PlatformID.WinCE)
//                WinAPI = new ReinoControls.WinCEAPI();
//            else
//                WinAPI = new ReinoControls.Win32API();

//            short loStatus;
//            TTControl loNewFocusField = null;

//            // Attempt to find the currently focused control in the previous "Fake" form
//            Control loPrevFocusCtrl = null;
//            IntPtr loPrevFocusHandle = IntPtr.Zero;
//            if (IssueAppImp.glFakeFormStack.Count > 0)
//            {
//                loPrevFocusCtrl = IssueAppImp.FindFocusedCtrl(IssueAppImp.glFakeFormStack[IssueAppImp.glFakeFormStack.Count - 1].Controls);
//            }
//            if ((loPrevFocusCtrl == null) && (IssueAppImp.glFocusForm != null))
//            {
//                if ((IssueAppImp.glFocusForm.CurrentCfgCtrl != null) &&
//                    (IssueAppImp.glFocusForm.CurrentCfgCtrl.WinCtrl != null))
//                    loPrevFocusCtrl = IssueAppImp.glFocusForm.CurrentCfgCtrl.WinCtrl;
//            }
//            FocusControlBeforeFormEdit = loPrevFocusCtrl;
//            if (loPrevFocusCtrl == null)
//            {
//                loPrevFocusHandle = IssueAppImp.GetFocus();
//            }

//            // Retain current focus form if its not ourself
//            if (IssueAppImp.glFocusForm != this)
//                fSavedFocusForm = IssueAppImp.glFocusForm;
//            // We are the new glFocusForm 
//            IssueAppImp.glFocusForm = this;

//            // Set fFormEditMode immediately; it is required by PrepareForEdit. 
//            fFormEditMode = iFormEditMode;

//            // Reset fExitForm 
//            fExitForm = 0;

//            // Reset FormEditResult
//            fFormEditResult = FormEditResults.FormEditResult_Cancelled;

//            // Peform any descendant specific initialization 
//            if ((loStatus = PrepareForEdit()) != 0)
//            {
//                // Restore previous focus form 
//                IssueAppImp.glFocusForm = fSavedFocusForm;
//                if (IssueAppImp.glFocusForm != null)
//                {
//                    IssueAppImp.glFocusForm.WinForm.Invalidate();
//                    IssueAppImp.ApplicationDoEvents();
//                }

//                // If this is a citation structure, then set flag that we're not issuing a ticket
//#if WindowsCE || __ANDROID__
//                if ((this.IssueStruct is Reino.ClientConfig.TCiteStruct) ||
//                    (this.IssueStruct is Reino.ClientConfig.TPublicContactStruct))
//                {
//                    IssueAppImp.GlobalIssueApp.ClearInsideTicketMutex();
//                }
//#endif

//                // Return the CPU to its normal operating speed
//                IssueAppImp.ScaleCPUFrequency(false);

//                return FormEditResults.FormEditResult_Failed;
//            }

//            // Find the 1st available focus control 
//            if (iInitialFocusControl != "")
//                loNewFocusField = this.FindCfgControlByName(iInitialFocusControl);
//            this.StartField = loNewFocusField;

//            // Before showing, we must remove the tag that is used by ReinoApplicationEx for knowing when to 
//            // hide the form (instead of a real form closure)
//            if ((this.WinForm.Tag != null) && (this.WinForm.Tag.Equals("FAKECLOSE")))
//                this.WinForm.Tag = null;

//            this.WinForm.Parent = IssueAppImp.glMainMenuForm;
//            this.WinForm.BringToFront();

//            // Since we're using "Fake" forms, we don't have a true modal dialog, so we have to fake that too.
//            // We'll start by making the last item on the stack be disabled, show the new "form", then
//            // restore the previous form when done.
//            if (IssueAppImp.glFakeFormStack.Count > 0)
//            {
//                // Disable the previous "Fake Form". We will use an API-level call to disable it since it
//                // will do so without repainting the form in a disabled mode (which is too slow)
//                GenericForm.EnableWindow(IssueAppImp.glFakeFormStack[IssueAppImp.glFakeFormStack.Count - 1].Handle, false);
//            }
//            IssueAppImp.glFakeFormStack.Add(this.WinForm);

//            // Show form modally WITHOUT destroying it when it closes
//            // Now show the dialog. Under CE, this will be controlled by our our application loop via ReinoApplicationEx
//            this.FormIsClosed = false;
//            this.WinForm.Show();

//            // JLA (3/9/07) Enable the timer
//            this.MainTimer.Enabled = true;

//            // If PSTHandlerThread exists, we need to bring it up to normal priority?
//#if WindowsCE || __ANDROID__
//#if PSTEnabled
//            if (IssueAppImp.GlobalIssueApp.PSTHandlerThread != null)
//            {
//                IssueAppImp.GlobalIssueApp.PSTHandlerThread.Priority = ThreadPriority.Normal;
//            }
//#endif //PSTEnabled
//#endif //WindowsCE

//            // Return the CPU to its normal operating speed
//            IssueAppImp.ScaleCPUFrequency(false);

//            // This is the simple loop that emulates a "modal" form
//            /*
//            while (this.FormIsClosed == false)
//            {
//                // Wait until a message arrives. This is the least CPU intensive technique. (Better than Sleep())
//                WinAPI.WaitMessage();
//                IssueAppImp.ApplicationDoEvents();
                
//                // Allow other threads to run too. If we don't do this, then the backlight might not work
//                // JLA (3/9/07) Sleep longer so we don't hammer the CPU as much
//                ////Thread.Sleep(20); //0);

//                // Allow other threads to run too. If we don't do this, then the backlight might not work
//                Thread.Sleep(1);
//            }
//            */

//            /*
//            // JLA (3/9/07) Disable the timer
//            this.MainTimer.Enabled = false;

//            // Now we need to remove ourself from the stack and restore the previous item
//            if (IssueAppImp.glFakeFormStack.IndexOf(this.WinForm) != -1)
//                IssueAppImp.glFakeFormStack.Remove(this.WinForm);
//            if (IssueAppImp.glFakeFormStack.Count > 0)
//            {
//                // Enable the previous window
//                GenericForm.EnableWindow(IssueAppImp.glFakeFormStack[IssueAppImp.glFakeFormStack.Count - 1].Handle, true);
//                IssueAppImp.glFakeFormStack[IssueAppImp.glFakeFormStack.Count - 1].Invalidate();
//                IssueAppImp.ApplicationDoEvents();
//            }

//            // If we know the previous focus control, try to reset focus back to it
//            if (loPrevFocusCtrl != null)
//            {
//                try { loPrevFocusCtrl.Focus(); }
//                catch { }
//            }
//            if ((loPrevFocusCtrl == null) && (loPrevFocusHandle != IntPtr.Zero))
//            {
//                IssueAppImp.SetFocus(loPrevFocusHandle);
//            }

//            // DEBUG -- Need to implement?
//            ////FormExit(); // FormExit event

//            // Restore previous focus form 
//            IssueAppImp.glFocusForm = fSavedFocusForm;
//            if (IssueAppImp.glFocusForm != null)
//            {
//                ActivateForm(IssueAppImp.glFocusForm.WinForm);
//                IssueAppImp.glFocusForm.WinForm.Invalidate();
//            }

//#if WindowsCE || __ANDROID__
//            // If this is a citation structure, then set flag that we're not issuing a ticket
//            if ((this.IssueStruct is Reino.ClientConfig.TCiteStruct) ||
//                (this.IssueStruct is Reino.ClientConfig.TPublicContactStruct))
//            {
//                IssueAppImp.GlobalIssueApp.ClearInsideTicketMutex();
//            }

//#if PSTEnabled
//            // If PSTHandlerThread exists, we need to bring it up to normal priority?
//            if (IssueAppImp.GlobalIssueApp.PSTHandlerThread != null)
//            {
//                IssueAppImp.GlobalIssueApp.PSTHandlerThread.Priority = ThreadPriority.Normal;
//            }
//#endif //PSTEnabled
//#endif //WindowsCE

//            // Clear flag to indicate FormCancel wasn't invoked yet
//            AlreadyInvokedFormCancel = false;

//            // Launch completion delegate
//            if (FormEditOnCompletedCallback != null)
//            {
//                FormEditOnCompletedCallback(null);
//            }
//            */

//            return fFormEditResult;
//        }

//        private void DoAfterFormIsClosed()
//        {
//            // JLA (3/9/07) Disable the timer
//            this.MainTimer.Enabled = false;

//            // Now we need to remove ourself from the stack and restore the previous item
//            if (IssueAppImp.glFakeFormStack.IndexOf(this.WinForm) != -1)
//                IssueAppImp.glFakeFormStack.Remove(this.WinForm);
//            if (IssueAppImp.glFakeFormStack.Count > 0)
//            {
//                // Enable the previous window
//                GenericForm.EnableWindow(IssueAppImp.glFakeFormStack[IssueAppImp.glFakeFormStack.Count - 1].Handle, true);
//                IssueAppImp.glFakeFormStack[IssueAppImp.glFakeFormStack.Count - 1].Invalidate();
//                IssueAppImp.ApplicationDoEvents();
//            }

//            // If we know the previous focus control, try to reset focus back to it
//            if (/*loPrevFocusCtrl*/FocusControlBeforeFormEdit != null)
//            {
//                try { /*loPrevFocusCtrl*/FocusControlBeforeFormEdit.Focus(); }
//                catch { }
//            }
//            /*
//            if ((loPrevFocusCtrl == null) && (loPrevFocusHandle != IntPtr.Zero))
//            {
//                IssueAppImp.SetFocus(loPrevFocusHandle);
//            }
//            */

//            // DEBUG -- Need to implement?
//            /*
//            FormExit(); // FormExit event
//            */

//            // Restore previous focus form 
//            IssueAppImp.glFocusForm = fSavedFocusForm;
//            if (IssueAppImp.glFocusForm != null)
//            {
//                ActivateForm(IssueAppImp.glFocusForm.WinForm);
//                IssueAppImp.glFocusForm.WinForm.Invalidate();
//            }

//#if WindowsCE
//            // If this is a citation structure, then set flag that we're not issuing a ticket
//            if ((this.IssueStruct is Reino.ClientConfig.TCiteStruct) ||
//                (this.IssueStruct is Reino.ClientConfig.TPublicContactStruct))
//            {
//                IssueAppImp.GlobalIssueApp.ClearInsideTicketMutex();
//            }

//#if PSTEnabled
//            // If PSTHandlerThread exists, we need to bring it up to normal priority?
//            if (IssueAppImp.GlobalIssueApp.PSTHandlerThread != null)
//            {
//                IssueAppImp.GlobalIssueApp.PSTHandlerThread.Priority = ThreadPriority.Normal;
//            }
//#endif //PSTEnabled
//#endif //WindowsCE

//            // Clear flag to indicate FormCancel wasn't invoked yet
//            AlreadyInvokedFormCancel = false;

//            // Launch completion delegate
//            if (FormEditOnCompletedCallback != null)
//            {
//                FormEditOnCompletedCallback(null);
//            }
//        }

//        private void UpdateDataGrids()
//        {
//            // The virtual list boxes associated with TEditListBox configuration objects 
//            // need to have their item count updated because active table filters can 
//            // dynamically change the number of reported items.
//            foreach (TTControl NextCtrl in EntryOrder)
//            {
//                if (!(NextCtrl is TEditListBox))
//                    continue;

//                // Update the virtual item count, and refresh the control so it's scrollbar gets updated
//                NextCtrl.Behavior.ListItemCount = NextCtrl.Behavior.ListSourceTable.GetRecCount();
//                if (NextCtrl.Behavior.ListBox != null)
//                    NextCtrl.Behavior.ListBox.RefreshItems(true);
//            }
//        }

//        private void UpdateVirtualLists()
//        {
//            foreach (TTControl NextCtrl in EntryOrder)
//            {
//                if (!(NextCtrl is TTEdit))
//                    continue;

//                if (NextCtrl.TypeOfWinCtrl() == typeof(ReinoTextBox))
//                {
//                    if (NextCtrl.Behavior.ListSourceTable != null)
//                    {
//                        NextCtrl.Behavior.ListItemCount = NextCtrl.Behavior.ListSourceTable.GetRecCount();
//                        if (NextCtrl.Behavior.ListBox != null)
//                            NextCtrl.Behavior.ListBox.RefreshItems(true);
//                    }
//                }
//            }
//        }

//        protected void PerformExtraDoneBtnTasks()
//        {
//            // THotSheetForm needs to do a search            
//            if (this.CfgForm is THotSheetForm)
//            {
//                this.AssocSearchStructLogic.PerformSearchAndIssue(this.CfgForm, false, 1,
//                    ((THotSheetForm)(this.CfgForm)).MatchFieldsName, null, false);
//            }

//            // TMarkModeForm needs to do a search            
//            if (this.CfgForm is TMarkModeForm)
//            {
//                this.AssocSearchStructLogic.PerformSearchAndIssue(this.CfgForm, false, 2, "", null, false);
//            }
//        }

//        public virtual void DoBtnPrintClick(object sender)
//        {
//            // SearchMatchForm has different needs
//            if (this.CfgForm is TSearchMatchForm)
//            {
//                SearchStructLogic SearchLogic = this.AssocSearchStructLogic;
//                TTTable MatchRecsTable = SearchLogic.GetMatchRecsTable();
//                TTTable DataTable = SearchLogic.IssueStruct.MainTable.HighTableRevision.Tables[0];

//                // Get the structure table to read in the current record
//                DataTable.ReadRecord(MatchRecsTable.GetPrimaryKey());
//                // Print the form, but extract data from structure, not form
//                PrintForm(false);
//                // Make the form think it hasn't printed so that ESC works and if we 
//                // change records it will do print from scratch.
//                SetFormPrinted(false);
//                return;
//            }

//            // Generic SearchForm also is treated differntly
//            if (this.CfgForm is TSearchForm)
//            {
//                // Print the form
//                PrintForm(true);
//                // Make the form think it hasn't printed so that ESC works and if we 
//                // change records it will do print from scratch.
//                SetFormPrinted(false);
//                return;
//            }

//            // Don't allow printing when in Adding mode in MultiEntry
//            if (!MultipleIssuanceOkToFinish())
//                return;

//            if (ValidatePrintedFields() != 0)
//                return;

//            if (IssueStruct is TCiteStruct)
//            {
//                if (this.AssocCiteStructLogic.CiteIsVoid(AssocIssueStructLogic.GetEditRecNo()))
//                {
//                    AppMessageBox.ShowMessageWithBell("Record is Void", "and cannot be printed.", "");
//                    return;
//                }
//            }

//            PrintForm(true);

//            // Enable the buttons as appropriate for current edit conditions.
//            SetButtonStates();
//        }

        //struct to retrive memory status
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUS
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public uint dwTotalPhys;
            public uint dwAvailPhys;
            public uint dwTotalPageFile;
            public uint dwAvailPageFile;
            public uint dwTotalVirtual;
            public uint dwAvailVirtual;
        }

#if WindowsCE
        //To get Memory status
		[DllImport("coredll", EntryPoint="GlobalMemoryStatus", SetLastError=false)]
		static extern void GlobalMemoryStatus(ref MEMORYSTATUS buf);
#elif __ANDROID__
        //To get Memory status
        [DllImport("coredll", EntryPoint = "GlobalMemoryStatus", SetLastError = false)]
        static extern void GlobalMemoryStatus(ref MEMORYSTATUS buf);
#else
        //To get Memory status
        [DllImport("kernel32")]
        static extern void GlobalMemoryStatus(ref MEMORYSTATUS buf);
#endif

//        public virtual short DoBtnDoneClick(object sender)
//        {
//            short loStatus;

//            // Is this the activity log form?
//            if (this.CfgForm is TActivityLogForm)
//            {
//                if (ValidateFormFields() != 0)
//                    return -1;
//                TTControl loLogCtrl = FindCfgControlByName(FieldNames.LOGPrimaryActivityNameFieldName);
//                if (loLogCtrl == null)
//                    return -1;
//                TTTable ActivityLogTable = IssueStruct.MainTable.HighTableRevision.Tables[0];
//                ReadFieldValuesFromForm(this.CfgForm, ActivityLogTable);
//                // ReadFieldValuesFromForm cleared out some core fields, reset them.
//                IssueAppImp.GlobalIssueApp.InitActivityFields(loLogCtrl.Behavior.EditBuffer);
//                IssueAppImp.GlobalIssueApp.LogActivity(loLogCtrl.Behavior.EditBuffer);
//                string loTmpStr = "";
//                // Populate the end date and time
//                ReinoControls.TextBoxBehavior.OSDateToDateString(DateTime.Today, "yyyymmdd", ref loTmpStr);
//                ActivityLogTable.SetFormattedFieldData(FieldNames.LOGEndDateFieldName, "yyyymmdd", loTmpStr);
//                ReinoControls.TextBoxBehavior.OSTimeToTimeString(DateTime.Now, "hhmmss", ref loTmpStr);
//                ActivityLogTable.SetFormattedFieldData(FieldNames.LOGEndTimeFieldName, "hhmmss", loTmpStr);
//            }

//            // Is it a PublicContactForm?
//            if (this.CfgForm is TPublicContactForm)
//            {
//                // This is where the magic of public contact occurs.
//                // In this routine, the "ExtraAction" fields are inspected to see if an issuance of another structure should occur.
//                // If one should, the structure is launched in "single entry" mode.  If the issuance results in a saved record,
//                // the inherited DoneButtonClick is called, which will save the public contact record.

//                // Validate the fields
//                if (ValidateFormFields() != 0)
//                    return -1;

//                if (this.AssocPublicContactStructLogic.PerformActionTaken(this) != FormEditResults.FormEditResult_OK)
//                    return -1;
//            }

//            // If we're dealing with a TSearchMatchForm, then we can skip save/print, etc.
//            if (this.CfgForm is TSearchMatchForm)
//            {
//                SetExitForm(1);
//                fFormEditResult = FormEditResults.FormEditResult_OK;
//                CloseForm(this.WinForm); //this.WinForm.Close();
//                return 0;
//            }

//            // If its a THotSheetForm, set flag that we're already saved
//            if (this.CfgForm is THotSheetForm /*TSearchForm*/)
//            {
//                SetFormSaved(true);
//            }

//            // DEBUG -- Need to implement?
//            /*
//            if (fFormEditMode == femView) return
//                TBaseIssForm.DoneButtonClick(this.BtnDone);
//            */

//            // If we are issuing multiple, make sure the user hasn't by-passed "capture signature"
//            if (!MultipleIssuanceOkToFinish())
//                return -1;

//#if DAILY_USAGE_SIMULATION
//            int loLoopMax = 3; // 75;
//            for (int loTicketCount = 1; loTicketCount <= loLoopMax; loTicketCount++)
//            {
//                if (loTicketCount < loLoopMax)
//                {
//                    // This code is only for automated volume testing!

//#if WindowsCE || __ANDROID__
//                    // Take a picture with the camera (except on the first loop index)
//                    if (loTicketCount > 1)
//                    {
//                        /*
//                        // Launch the camera application
//                        System.Diagnostics.Process.Start("\\Windows\\X3Camera.exe", "/SHOW");
//                        Thread.Sleep(3000);
//                        // Simulate the [ENTER] key being pressed
//                        Reino.CommonLogic.SendKeys.Send("~");
//                        */
//                        Thread.Sleep(3000);
//                    }
//#endif
//                    if ((loStatus = SaveFormRecord()) < 0)
//                        return loStatus;
//                    PrintForm(true);
//                    if (this.AssocCiteStructLogic != null)
//                        this.AssocCiteStructLogic.AutoAttachMMNotes(this.BtnNotes);
//                    IssueAppImp.GlobalIssueApp.LogActivity(IssueStruct.Name);
//                    if (BtnIssueChild != null)
//                        AssocIssueStructLogic.IssueChildRecord(EditRestrictionConsts.femSingleEntry, "", AssocIssueStructLogic.GetEditRecNo());
//                    PerformExtraDoneBtnTasks();

//                    // Make new ticket just by incrementing the IssueNo and clearing the Printed & Saved flags
//                    SetFormPrinted(false);
//                    SetFormSaved(false);
//                    SetFormIssueNoLogged(false);
//                    SetFormEditMode(EditRestrictionConsts.femNewEntry);
//                    SetIssueNoFields();
//                    fDidAutoAttachProcedure = false;
//                    fInsideAutoAttachProcedure = false;
//                    AssocIssueStructLogic.SetEditRecNo(AssocIssueStructLogic.GetEditRecNo() + 1);

//                    // Generate a random string and fill the license plate with it
//                    Random RandX = new Random(DateTime.Now.Millisecond);
//                    StringBuilder builder = new StringBuilder();
//                    char ch;
//                    for (int i = 0; i < 11; i++)
//                    {
//                        ch = (char)(RandX.Next(65, 90));
//                        builder.Append(ch);
//                    }
//                    string loRandStr = builder.ToString();
//                    TTControl loFieldToStuff = this.FindCfgControlByName(FieldNames.VehLicNoFieldName);
//                    if (loFieldToStuff != null)
//                        loFieldToStuff.Behavior.SetEditBufferAndPaint(loRandStr);

//                    // Fill the "PublicComment" field with Memory Info
//                    MEMORYSTATUS mst = new MEMORYSTATUS();
//                    GlobalMemoryStatus(ref mst);
//                    string loCommentStr = "RAM: Phys=" + (mst.dwTotalPhys / 1024).ToString() +
//                        " Avail=" + (mst.dwAvailPhys / 1024).ToString() +
//                        " Load=" + mst.dwMemoryLoad.ToString() + "%";
//                    loFieldToStuff = this.FindCfgControlByName("PUBLICCOMMENT");
//                    if (loFieldToStuff != null)
//                        loFieldToStuff.Behavior.SetEditBufferAndPaint(loCommentStr);
//                    loFieldToStuff = this.FindCfgControlByName("REMARK1");
//                    if (loFieldToStuff != null)
//                        loFieldToStuff.Behavior.SetEditBufferAndPaint(loCommentStr);
//                }
//            }
//#endif

//            if (!GetFormSaved())
//            {
//                if ((loStatus = SaveFormRecord()) < 0)
//                    return loStatus;
//            }
//            // print the form if necessary 
//            if ((!this.CfgIssForm.PrintNotMandatory) && (!GetFormPrinted()))
//            {
//                PrintForm(true);
//                if (!GetFormPrinted())
//                    return -1;
//            }

//            // Attach pictures/sound that entered system during the ticket entry 
//            if (this.AssocCiteStructLogic != null)
//                this.AssocCiteStructLogic.AutoAttachMMNotes(this.BtnNotes);

//            IssueAppImp.GlobalIssueApp.LogActivity(IssueStruct.Name);

//            // If its activity log, then we wrote what we needed already, so just reinit the table
//            if (this.CfgForm is TActivityLogForm)
//            {
//                TTTable ActivityLogTable = IssueStruct.MainTable.HighTableRevision.Tables[0];
//                ActivityLogTable.ClearFieldValues();
//            }

//            if (BtnIssueChild != null)
//                AssocIssueStructLogic.IssueChildRecord(EditRestrictionConsts.femSingleEntry, "", AssocIssueStructLogic.GetEditRecNo());

//            PerformExtraDoneBtnTasks();

//            // if we are in multiple entry mode, move to the next record in the queue 
//            if (((fFormEditMode & EditRestrictionConsts.femIssueMultipleAttr) > 0) &&
//                (EditMultipleEntry(fMultipleEntryMgr.CurrentRecNo() + 1) > 0))
//                return 0;


//            if ((fFormEditMode & EditRestrictionConsts.femSingleRecordAttr) == 0)
//            {
//                // rather than exit, we will enter a new record.

//                // make sure we have enough flash
//                if (AssocIssueStructLogic.QueryUserReclaimFlash(50000) < 50000)
//                {
//                    SetExitForm(1);
//                    CloseForm(this.WinForm); //this.WinForm.Close();
//                    return 0;
//                }

//                AssocIssueStructLogic.SetEditRecNo(AssocIssueStructLogic.GetEditRecNo() + 1);
//                fFormEditMode = EditRestrictionConsts.femNewEntry;
//                if (PrepareForEdit() < 0)
//                {
//                    // get outta here!
//                    SetExitForm(1);
//                    fFormEditResult = FormEditResults.FormEditResult_OK;
//                    CloseForm(this.WinForm); //this.WinForm.Close();
//                }

//                // Determine if we should go to entry mode or navigation mode
//                bool loGotoNavScreen = false;
//                if (this.AssocIssueStructLogic.IssueStruct.Name.Equals("METERSTATUS"))
//                    loGotoNavScreen = true;
//                if (this.AssocIssueStructLogic.IssueStruct is TCiteStruct)
//                    loGotoNavScreen = true;

//                // Are we going to navigation mode?
//                if (loGotoNavScreen == true)
//                {
//                    // Find the first control we want to activate, then set it as current
//                    Reino.ClientConfig.TTControl loNewFocusCtrl = GetNextCfgCtrl(null, true);
//                    if (loNewFocusCtrl != null)
//                    {
//                        CurrentCfgCtrl = loNewFocusCtrl;
//                        int loCfgCtrlIndex = GetCfgCtrlIndex(CurrentCfgCtrl);
//                        if (loCfgCtrlIndex != -1)
//                            CurrentCfgCtrlIdx = loCfgCtrlIndex;
//                    }
//                    else
//                    {
//                        CurrentCfgCtrl = null;
//                        CurrentCfgCtrlIdx = -1;
//                    }
//                    // Now show the nav screen and perform the normal focus events
//                    EntryFieldsBtn_Click(EntryFieldsBtn, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
//                }
//                else
//                {
//                    // First we need to make sure the entry panel is visible,
//                    // then get the first editable control and set focus on it
//                    this.HideNavigationPanel(); // NavigationPanel.Visible = false;
//                    EntryPanel.Visible = true;
//                    EntryPanel.BringToFront();
//                    SetFocus(null);
//                }
//                return 0;
//            }

//            SetExitForm(1);
//            fFormEditResult = FormEditResults.FormEditResult_OK;
//            CloseForm(this.WinForm); //this.WinForm.Close();
//            return 0;
//        }

        /// <summary>
        /// IssueMore button performs the actions of done (validate record, print it, save it),
        /// Then begins issuance of a new record under following conditions:
        /// - sets fFormEditMode is femIssueMore.
        /// - Calls PrepareForEdit.
        /// - Sets feaEditedFirstField so that first focus field and all following fields are
        ///   not cleared.
        /// - Sets focus to fIssueMoreFirstFocus.
        /// The net result is that only the issue number field is initialized.
        /// </summary>
        //public virtual short DoBtnIssueMoreClick(object sender)
        //{
        //    if ((fFormEditMode & Reino.ClientConfig.EditRestrictionConsts.femView) != 0)
        //        return 0;

        //    // Under no circumstances is IssueMore allowed while IssuingMultiple
        //    if ((fFormEditMode & Reino.ClientConfig.EditRestrictionConsts.femIssueMultipleAttr) > 0)
        //    {
        //        AppMessageBox.ShowMessageWithBell("Issue More is not allowed", "while issuing multiple", "");
        //        return -1;
        //    }

        //    // If its a THotSheetForm, set flag that we're already saved
        //    if (this.CfgForm is THotSheetForm/*TSearchForm*/)
        //        SetFormSaved(true);

        //    if (!GetFormSaved())
        //    {
        //        short loStatus;
        //        if ((loStatus = SaveFormRecord()) < 0)
        //            return loStatus;
        //    }

        //    // Print the form if necessary 
        //    if ((!this.CfgIssForm.PrintNotMandatory) && (!GetFormPrinted()))
        //        PrintForm(true);

        //    // Attach pictures/sound that entered system during the ticket entry 
        //    if (this.AssocCiteStructLogic != null)
        //        this.AssocCiteStructLogic.AutoAttachMMNotes(this.BtnNotes);

        //    // log it 
        //    IssueAppImp.GlobalIssueApp.LogActivity(IssueStruct.Name);

        //    // If BtnIssueChild exists, the child MUST be issued in conjunction with the parent
        //    if (BtnIssueChild != null)
        //        AssocIssueStructLogic.IssueChildRecord(EditRestrictionConsts.femSingleEntry, "", AssocIssueStructLogic.GetEditRecNo());

        //    PerformExtraDoneBtnTasks();

        //    // Make sure we have enough flash
        //    if (AssocIssueStructLogic.QueryUserReclaimFlash(50000) < 50000)
        //    {
        //        SetExitForm(1);
        //        return 0;
        //    }

        //    AssocIssueStructLogic.SetEditRecNo(AssocIssueStructLogic.GetEditRecNo() + 1);

        //    // Rather than exit, we will enter a new record. 
        //    fFormEditMode |= Reino.ClientConfig.EditRestrictionConsts.femIssueMoreAttr;
        //    // Mask out all but SingleEntry and IssueMore 
        //    fFormEditMode &= (Reino.ClientConfig.EditRestrictionConsts.femIssueMoreAttr | Reino.ClientConfig.EditRestrictionConsts.femSingleRecordAttr);
        //    PrepareForEdit();

        //    // If there is a "IssueMoreFirstFocus" field specified, try to resolve it,
        //    // then set focus to the either the resolved field, or the next editable field
        //    TTControl NewFocusCtrl = null;
        //    if (this.CfgIssForm.IssueMoreFirstFocus != "")
        //        NewFocusCtrl = FindCfgControlByName(this.CfgIssForm.IssueMoreFirstFocus);
        //    SetFocus(NewFocusCtrl);

        //    return 0;
        //}

        /// <summary>
        /// IssueMultiple button performs a print level validation, then saves the record to a temporary buffer so a new record can
        /// be entered.  Ultimately, this record will be updated with a signature, then printed.  The goal is to allow several tickets
        /// to be entered without requiring the signature to be captured on one before the next can be started.
        /// The steps are:
        ///  - Validate printed fields.
        ///  - Protect printed fields.
        ///  - Save record to temp buffer.
        ///  - sets fFormEditMode is femIssueMultiple.
        ///  - Calls PrepareForEdit.
        ///  - Sets feaEditedFirstField so that first focus field and all following fields are
        ///    not cleared.
        ///  - Sets focus to fIssueMoreFirstFocus.
        ///  The net result is that only the issue number field is initialized.
        /// </summary>
        //public virtual short DoBtnIssueMultipleClick(object sender)
        //{
        //    if ((fFormEditMode & EditRestrictionConsts.femView) != 0)
        //        return 0;

        //    // In Multiple entry mode, this sends us back to the first record in the queue
        //    if (sender == BtnEndIssueMultiple)
        //    {
        //        if (((fFormEditMode & EditRestrictionConsts.femIssueMultipleAttr) > 0) && (fMultipleEntryMgr.fMultiEntryMode == EMultiEntryMode.memAdding))
        //        {  // this is now the "End Multiple Issuance Button"
        //            // save this record
        //            fMultipleEntryMgr.SaveRecord(this, fFormEditMode);
        //            EditMultipleEntry(0); // this sets the focus to the control immediately after this button
        //            return 0;
        //        }
        //        else
        //        {
        //            return -1; // shouldn't happen.
        //        }
        //    }

        //    // assume sender is fBtnIssueMultiple

        //    // This button should not be pressed if in the "IssueMultiple" flow we are in the 2nd pass (i.e. have returned to capture signatures, print, and save)
        //    if ((fMultipleEntryMgr != null) && (fMultipleEntryMgr.fMultiEntryMode != EMultiEntryMode.memAdding))
        //    {
        //        AppMessageBox.ShowMessageWithBell("All tickets must be completed", "before issuing multiple again.", "");
        //        return -1;
        //    }

        //    // print level validation
        //    if (ValidatePrintedFields() != 0)
        //        return -1;

        //    // make sure we have enough flash for the next record
        //    if (AssocIssueStructLogic.QueryUserReclaimFlash(50000) < 50000)
        //    {
        //        SetExitForm(1);
        //        return -1;
        //    }

        //    // save to RAM
        //    if (fMultipleEntryMgr == null)
        //        fMultipleEntryMgr = new MultipleEntryMgr();
        //    // rather than exit, we will enter a new record. 
        //    fFormEditMode |= EditRestrictionConsts.femIssueMultiple; // IssueMultiple | IssueMore
        //    fMultipleEntryMgr.SaveRecord(this, fFormEditMode);

        //    if ((IssueNoCtrl != null) && (!GetFormIssueNoLogged()))
        //    {
        //        Int64 loIssueNo = 0;
        //        ReinoControls.TextBoxBehavior.StrTollInt(IssueNoCtrl.Behavior.EditBuffer, ref loIssueNo);
        //        if (IssueStruct is TCiteStruct)
        //            this.AssocCiteStructLogic.GetSequence().LogUsedNumber(loIssueNo);
        //        SetFormIssueNoLogged(true); // don't need to log again for 2nd copies
        //    }

        //    AssocIssueStructLogic.SetEditRecNo(AssocIssueStructLogic.GetEditRecNo() + 1);

        //    // mask out all but SingleEntry and IssueMore 
        //    fFormEditMode &= (EditRestrictionConsts.femIssueMultiple | EditRestrictionConsts.femSingleRecordAttr);

        //    if (PrepareForEdit() < 0)
        //    { // get outta here!
        //        SetExitForm(1);
        //        fFormEditResult = FormEditResults.FormEditResult_OK;
        //    }

        //    // If there is a "IssueMoreFirstFocus" field specified, try to resolve it,
        //    // then set focus to the either the resolved field, or the next editable field
        //    TTControl NewFocusCtrl = null;
        //    if (this.CfgIssForm.IssueMoreFirstFocus != "")
        //        NewFocusCtrl = FindCfgControlByName(this.CfgIssForm.IssueMoreFirstFocus);
        //    SetFocus(NewFocusCtrl);

        //    SetButtonStates();
        //    // update form caption to reflect multi issuance mode
        //    // save the old caption if overwriting for 1st time
        //    if (fMultipleEntryMgr.RecordCount() == 1)
        //        fSavedCaption = this.TitleBarCaption.Text;
        //    string loCaption = string.Format("Multi issuance: Adding no. {0:d}", fMultipleEntryMgr.RecordCount() + 1);
        //    SetCaption(loCaption);

        //    // show a message if this is the first one
        //    if (fMultipleEntryMgr.RecordCount() == 1)
        //        AppMessageBox.ShowMultiLineMessageWithBell("Multiple Issuance Mode\r\n" +
        //                        "Press End Multiple after the\r\n" + "last ticket has been entered\r\n" +
        //                        "to sign and print each one.", "");
        //    return 0;
        //}

        //public virtual short DoBtnVoidClick(object sender)
        //{
        //    // Save the ticket record if necessary
        //    // if we are issuing multiple, make sure the user hasn't by-passed "capture signature". Capture Signature 
        //    if (!MultipleIssuanceOkToFinish())
        //        return -1;

        //    if (!GetFormPrinted() && !GetFormSaved())
        //    {
        //        AppMessageBox.ShowMessageWithBell("Form can still be edited", "Voiding is unnecessary", "");
        //        return 0;
        //    }

        //    if (SaveFormRecord() != 0)
        //        return 0;

        //    // Let user enter a void reason.  User can choose not to void at this time.
        //    if (this.IssueStruct is TCiteStruct)
        //        AssocCiteStructLogic.VoidRecord(AssocCiteStructLogic.GetEditRecNo());

        //    // Attach pictures/sound that entered system during the ticket entry 
        //    if (this.AssocCiteStructLogic != null)
        //        this.AssocCiteStructLogic.AutoAttachMMNotes(this.BtnNotes);

        //    SetButtonStates();

        //    // SetButtonStates may have disabled the Void button. 
        //    // If so, we need to find a new focus control 
        //    if (this.CurrentCfgCtrl is TTButton)
        //    {
        //        if (((TTButton)(this.CurrentCfgCtrl)).Enabled == true)
        //            return 0;
        //    }
        //    if (this.CurrentCfgCtrl is TTEdit)
        //    {
        //        if (this.CurrentCfgCtrl.IsEnabled == true)
        //            return 0;
        //    }

        //    // First we need to make sure the entry panel is visible,
        //    // then get set focus on the closest enabled control
        //    if (this.EntryPanel.Visible)
        //    {
        //        this.HideNavigationPanel(); // NavigationPanel.Visible = false;
        //        EntryPanel.Visible = true;
        //        EntryPanel.BringToFront();
        //    }
        //    Reino.ClientConfig.TTControl NextCfgCtrl = GetNextCfgCtrl(this.CurrentCfgCtrl, true);
        //    if (NextCfgCtrl != null)
        //        SetActiveCfgCtrl(NextCfgCtrl);
        //    else
        //    {
        //        NextCfgCtrl = GetPreviousCfgCtrl(this.CurrentCfgCtrl, true);
        //        if (NextCfgCtrl != null)
        //            SetActiveCfgCtrl(NextCfgCtrl);
        //    }
        //    if (this.NavigationPanel.Visible)
        //    {
        //        EntryFieldsBtn_Click(EntryFieldsBtn, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
        //    }
        //    return 0;
        //}

        //public virtual short DoBtnReissueClick(object sender)
        //{
        //    // Perform a reissue
        //    if (!(this.IssueStruct is TCiteStruct))
        //    {
        //        AppMessageBox.ShowMessageWithBell(this.IssueStruct.ObjDisplayName + " cannot be reissued", "", "");
        //        return 0;
        //    }

        //    // If the record has not been printed nor saved, no need to reissue.
        //    if (!GetFormPrinted() && !GetFormSaved())
        //    {
        //        AppMessageBox.ShowMessageWithBell("Form can still be edited", "Reissuing is unnecessary", "");
        //        return 0;
        //    }

        //    if (SaveFormRecord() != 0)
        //        return 0;

        //    // Let user enter a void reason.  User can choose not to void at this time.
        //    if ((this.IssueStruct is TCiteStruct))
        //    {
        //        if ((!this.AssocCiteStructLogic.CiteIsVoid(AssocIssueStructLogic.GetEditRecNo())) &&
        //            (!this.AssocCiteStructLogic.VoidRecord(AssocIssueStructLogic.GetEditRecNo())))
        //        {
        //            return 0;
        //        }
        //    }

        //    // Attach pictures/sound that entered system during the ticket entry 
        //    if (this.AssocCiteStructLogic != null)
        //        this.AssocCiteStructLogic.AutoAttachMMNotes(this.BtnNotes);

        //    // Log it 
        //    IssueAppImp.GlobalIssueApp.LogActivity(IssueStruct.Name);

        //    // Make sure we have enough flash
        //    if (AssocIssueStructLogic.QueryUserReclaimFlash(50000) < 50000)
        //    {
        //        SetExitForm(1);
        //        return 0;
        //    }

        //    fReissuedKey = AssocIssueStructLogic.GetEditRecNo();

        //    AssocIssueStructLogic.SetEditRecNo(AssocIssueStructLogic.GetEditRecNo() + 1);

        //    // rather than exit, we will enter a new record. 
        //    fFormEditMode &= (EditRestrictionConsts.femSingleRecordAttr);
        //    fFormEditMode |= EditRestrictionConsts.femReissueAttr;

        //    if (PrepareForEdit() < 0)
        //    { // get outta here!
        //        SetExitForm(1);
        //        fFormEditResult = FormEditResults.FormEditResult_OK;
        //    }

        //    /*SetFocus(null);*/
        //    // Find the first control we want to activate, then set it as current
        //    Reino.ClientConfig.TTControl loNewFocusCtrl = GetNextCfgCtrl(null, true);
        //    if (loNewFocusCtrl != null)
        //    {
        //        CurrentCfgCtrl = loNewFocusCtrl;
        //        int loCfgCtrlIndex = GetCfgCtrlIndex(CurrentCfgCtrl);
        //        if (loCfgCtrlIndex != -1)
        //            CurrentCfgCtrlIdx = loCfgCtrlIndex;
        //    }
        //    else
        //    {
        //        CurrentCfgCtrl = null;
        //        CurrentCfgCtrlIdx = -1;
        //    }
        //    // Now show the nav screen and perform the normal focus events
        //    EntryFieldsBtn_Click(EntryFieldsBtn, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));

        //    return 0;
        //}

        /// <summary>
        /// Enters "Correction" mode.  In correction mode, the same ticket number is used to 
        /// issue a new ticket.  All field values are retained; the user merely changes those fields
        /// that require changing.
        /// Correction mode is only applicable after the form has printed.
        /// </summary>
        //public virtual short DoBtnCorrectionClick(object sender)
        //{
        //    // only enter correction mode if form has already printed but not saved.
        //    if (!GetFormPrinted() || GetFormSaved()) return 0;

        //    if (fFormEditMode == EditRestrictionConsts.femView) return 0;

        //    // require the user to provide a reason for correcting the ticket, and log it.
        //    if (!GetCancelReason()) return 0;

        //    // make sure we have enough flash
        //    if (AssocIssueStructLogic.QueryUserReclaimFlash(50000) < 50000)
        //    {
        //        SetExitForm(1);
        //        return 0;
        //    }

        //    // set mode to "Correction"
        //    fFormEditMode &= EditRestrictionConsts.femSingleRecordAttr;
        //    fFormEditMode |= EditRestrictionConsts.femCorrectionAttr;

        //    if (PrepareForEdit() < 0)
        //    { // get outta here!
        //        SetExitForm(1);
        //        fFormEditResult = FormEditResults.FormEditResult_OK;
        //    }

        //    SetFocus(null);
        //    return 0;
        //}

        //public virtual short DoBtnFormCancelClick()
        //{
        //    string loErrMsg = "";
        //    string loErrMsg2 = "";
        //    if (OkToExitForm(ref loErrMsg, ref loErrMsg2))
        //    {
        //        SetExitForm(1);
        //        CloseForm(this.WinForm); //this.WinForm.Close();
        //        return 0;
        //    }

        //    // only show an error message if one was set- the "special" message
        //    // indicates that an ancestor function has already shown a message
        //    // and we don't need to double up here
        //    if (string.Compare(loErrMsg, cnPreventExitNoMessage) != 0)
        //    {
        //        AppMessageBox.ShowMessageWithBell(loErrMsg, loErrMsg2, "");
        //    }
        //    return 0;
        //}

        //private delegate void InvokeFormCancelDelegate(string FailMsgPrefix1, string FailMsgPrefix2);
        //private bool AlreadyInvokedFormCancel = false;

        //public void InvokeFormCancel(string FailMsgPrefix1, string FailMsgPrefix2)
        //{
        //    // Don't do this again
        //    if (AlreadyInvokedFormCancel == true)
        //        return;
        //    else
        //        AlreadyInvokedFormCancel = true;

        //    if (IssueAppImp.glMainMenuForm.InvokeRequired == true)
        //    {
        //        InvokeFormCancelDelegate loDelegate = new InvokeFormCancelDelegate(InvokeFormCancel);
        //        IssueAppImp.glMainMenuForm.Invoke(loDelegate, new object[] { FailMsgPrefix1, FailMsgPrefix2 });
        //    }
        //    else
        //    {
        //        string loErrMsg = "";
        //        string loErrMsg2 = "";
        //        if (OkToExitForm(ref loErrMsg, ref loErrMsg2))
        //        {
        //            // Set flag to let field know it doesn't need to validate right now
        //            if ((this.CurrentCfgCtrl != null) &&
        //                (this.CurrentCfgCtrl.TypeOfWinCtrl() == typeof(ReinoTextBox)))
        //            {
        //                this.CurrentCfgCtrl.Behavior.SkipNextValidation = true;
        //            }

        //            // Now close the form
        //            SetExitForm(1);
        //            CloseForm(this.WinForm); //this.WinForm.Close();
        //        }

        //        // only show an error message if one was set- the "special" message
        //        // indicates that an ancestor function has already shown a message
        //        // and we don't need to double up here
        //        if ((string.Compare(loErrMsg, cnPreventExitNoMessage) != 0) && (loErrMsg != ""))
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            if (FailMsgPrefix1 != "")
        //                sb.Append(FailMsgPrefix1 + "\r\n");
        //            if (FailMsgPrefix2 != "")
        //                sb.Append(FailMsgPrefix2 + "\r\n\r\n");
        //            sb.Append(loErrMsg + "\r\n" + loErrMsg2);
        //            AppMessageBox.ShowMultiLineMessageWithBell(sb.ToString(), "");
        //        }
        //    }
        //}

        //public virtual short DoBtnCancelClick(object sender)
        //{
        //    // Can't cancel if already saved
        //    if (GetFormSaved()) return 0;

        //    // If we've already printed, prompt for a cancel reason
        //    if (GetFormPrinted())
        //    {
        //        // Require the user to provide a reason for correcting the ticket, and log it.
        //        if (!GetCancelReason()) return 0;
        //    }

        //    // Make sure we have enough flash
        //    if (AssocIssueStructLogic.QueryUserReclaimFlash(50000) < 50000)
        //    {
        //        SetExitForm(1);
        //        return 0;
        //    }

        //    fFormEditMode &= (EditRestrictionConsts.femSingleRecordAttr);
        //    fFormEditMode |= EditRestrictionConsts.femCancelAttr;

        //    if (PrepareForEdit() < 0)
        //    { // Get outta here!
        //        SetExitForm(1);
        //        fFormEditResult = FormEditResults.FormEditResult_OK;
        //    }

        //    SetFocus(null);
        //    return 0;
        //}

        //public virtual short DoBtnNotesClick(object sender)
        //{
        //    if (!(this.IssueStruct is TCiteStruct))
        //        return 0; // nothing to do. 

        //    // save the form if necessary 
        //    if (fFormEditMode != EditRestrictionConsts.femView)
        //    {
        //        // if we are issuing multiple, make sure the user hasn't 
        //        // by-passed "capture signature". Capture Signature 
        //        if (!MultipleIssuanceOkToFinish())
        //            return -1;

        //        if (!GetFormSaved() && (SaveFormRecord() < 0))
        //            return -1;

        //        // print the form if necessary 
        //        if ((this.CfgIssForm != null) && (!this.CfgIssForm.PrintNotMandatory) && (!GetFormPrinted()))
        //            PrintForm(true);

        //        // Attach pictures/sound that entered system during the ticket entry 
        //        if (this.AssocCiteStructLogic != null)
        //            this.AssocCiteStructLogic.AutoAttachMMNotes(this.BtnNotes);
        //    }

        //    this.AssocCiteStructLogic.AddNotes(AssocIssueStructLogic.GetEditRecNo());
        //    // Enable the buttons as appropriate for current edit conditions.
        //    SetButtonStates();

        //    // Set focus to either the sender or the next enabled field
        //    Reino.ClientConfig.TTControl FocusCtrl = null;
        //    if (sender is ReinoNavButton)
        //        FocusCtrl = ((ReinoNavButton)(sender)).AssocCfgBtn;
        //    SetFocus(FocusCtrl);
        //    return 1;
        //}

        //public virtual short DoBtnSuspSignatureClick(object sender)
        //{
        //    if (fDrawSuspSignature == null)
        //    {
        //        AppMessageBox.ShowMessageWithBell("No Signature Field!", "", "");
        //        return 0;
        //    }

        //    // Make sure we close any popup balloons that might be displayed
        //    if (fDrawSuspSignature.Behavior != null)
        //        fDrawSuspSignature.Behavior.ClosePopupBalloons();

        //    StringBuilder loSigCaption = new StringBuilder();
        //    TTControl loVioDescField = null;

        //    // Get the header
        //    if ((this.CfgForm as TIssForm).SuspSignatureCaption != "")
        //        loSigCaption.Append((this.CfgForm as TIssForm).SuspSignatureCaption);
        //    else
        //        loSigCaption.Append("By signing I acknowledge receipt of this citation. This is not an admission of guilt.");

        //    // Get the complete issue number
        //    loSigCaption.Append("\nIssue No. ");
        //    if (IssueNoPfxCtrl != null)
        //        loSigCaption.Append(IssueNoPfxCtrl.Behavior.EditBuffer);
        //    if (IssueNoCtrl != null)
        //        loSigCaption.Append(IssueNoCtrl.Behavior.EditBuffer);
        //    if (IssueNoSfxCtrl != null)
        //        loSigCaption.Append(IssueNoSfxCtrl.Behavior.EditBuffer);

        //    // Get the violations
        //    if ((this.CfgForm as TIssForm).SigCaptureDisplayFieldName != "")
        //    {
        //        if ((this.CfgForm as TIssForm).SigCaptureDisplayFieldPrompt != "")
        //            loSigCaption.Append("\n" + (this.CfgForm as TIssForm).SigCaptureDisplayFieldPrompt + "\n");
        //        else
        //            loSigCaption.Append("\n");
        //        string loVioDescFieldName = (this.CfgForm as TIssForm).SigCaptureDisplayFieldName;
        //        for (int loNdx = 1; ; loNdx++)
        //        {
        //            // attempt to find all the violation descriptions
        //            loVioDescField = this.FindCfgControlByName(loVioDescFieldName) as TTControl; // TEditField;
        //            if ((loVioDescField == null) || (loVioDescField.Behavior.FieldIsBlank()))
        //                break;
        //            loSigCaption.Append(loNdx.ToString() + ") " + loVioDescField.Behavior.EditBuffer + "\n");
        //            // prepare field name for next time through
        //            loVioDescFieldName = (this.CfgForm as TIssForm).SigCaptureDisplayFieldName + "_" + loNdx;
        //        }
        //    }

        //    // Now we're ready to do the capture signature form (Flip the display for violator's signature)
        //    string loTempBuffer = "";
        //    TDrawField loDrawField = (fDrawSuspSignature as TDrawField);
        //    Reino.CommonLogic.TSigCaptureForm.CaptureSignature(loSigCaption.ToString(), ref loTempBuffer, 0, ref loDrawField, true);

        //    // Take a moment to ensure mouse/keyboard events finish before trying to advance focus
        //    IssueAppImp.ApplicationDoEvents();
        //    System.Threading.Thread.Sleep(10);
        //    IssueAppImp.ApplicationDoEvents();

        //    // Automatically advance user past this field now that they finished it
        //    if (TabForward() == false)
        //        ReturnFocusToActiveCtrl();

        //    // If navigation panel was active, return to it
        //    if (EntryPanel.Visible == false)
        //    {
        //        EntryFieldsBtn_Click(EntryFieldsBtn, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
        //        vlbFields.Focus();
        //    }
        //    return 0;
        //}

        //public virtual short DoBtnOfficerSignatureClick(object sender)
        //{
        //    if (fDrawOfficerSignature == null)
        //    {
        //        AppMessageBox.ShowMessageWithBell("No Signature Field!", "", "");
        //        return 0;
        //    }

        //    // Make sure we close any popup balloons that might be displayed
        //    if (fDrawOfficerSignature.Behavior != null)
        //        fDrawOfficerSignature.Behavior.ClosePopupBalloons();

        //    // Get the header
        //    StringBuilder loSigCaption = new StringBuilder();
        //    TEditField loVioDescField = null;
        //    if ((this.CfgForm as TIssForm).OfficerSignatureCaption != "")
        //        loSigCaption.Append((this.CfgForm as TIssForm).OfficerSignatureCaption);
        //    else
        //        loSigCaption.Append("I swear to the accuracy of this notice and am authorized to issue it:");

        //    // Now we're ready to do the capture signature form (Don't flip the display)
        //    string loTempBuffer = "";
        //    TDrawField loDrawField = (fDrawOfficerSignature as TDrawField);
        //    Reino.CommonLogic.TSigCaptureForm.CaptureSignature(loSigCaption.ToString(), ref loTempBuffer, 0, ref loDrawField, false);

        //    // Take a moment to ensure mouse/keyboard events finish before trying to advance focus
        //    IssueAppImp.ApplicationDoEvents();

        //    // Automatically advance user past this field now that they finished it
        //    if (TabForward() == false)
        //        ReturnFocusToActiveCtrl();

        //    // If navigation panel was active, return to it
        //    if (EntryPanel.Visible == false)
        //    {
        //        EntryFieldsBtn_Click(EntryFieldsBtn, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
        //        vlbFields.Focus();
        //    }
        //    return 0;
        //}

        //public virtual short DoBtnReadReinoClick(object sender)
        //{
        //    TTControl loNextFocusControl = FindNextFormControl(this.BtnReadReino.AssocCfgBtn, true);
        //    ReadReinoMeter();
        //    if (loNextFocusControl != null)
        //        SetFocus(loNextFocusControl);
        //    return 0;
        //}

        //public void SetCaption(string NewCaption)
        //{
        //    this.TitleBarCaption.Text = NewCaption;
        //    this.TitleBarRawCaption = NewCaption;
        //}

        //public void HideNavigationPanel()
        //{
        //    // Hide the panel. We need to also explicitly hide the listbox and section buttons so
        //    // they get unregistered from the application's message filters
        //    this.NavigationPanel.Visible = false;
        //    if (this.vlbFields != null)
        //        this.vlbFields.Visible = false;
        //    if (this.SectionButtons != null)
        //    {
        //        foreach (ReinoNavButton NextNavBtn in this.SectionButtons)
        //            NextNavBtn.Visible = false;
        //    }
        //}

        //public void ShowNavigationPanel()
        //{
        //    // Make sure items are removed from entry panel and force rebuild next time
        //    // entry panel is displayed (This is to prevent button from appearing on wrong page)
        //    if (CurrentCfgCtrl != null)
        //    {
        //        DisplayPage loCurrentDisplayPage = GetDisplayPageForCfgCtrl(CurrentCfgCtrl);
        //        MoveDisplayedItemsToHoldingTank(loCurrentDisplayPage);
        //        MustExecuteSetActiveCfgCtrl = true;
        //    }

        //    // Show the panel. We need to also explicitly show the listbox and section buttons so
        //    // they get registered with the application's message filters
        //    if (this.vlbFields != null)
        //        this.vlbFields.Visible = true;
        //    if (this.SectionButtons != null)
        //    {
        //        foreach (ReinoNavButton NextNavBtn in this.SectionButtons)
        //        {
        //            // Lets force the offscreen bitmap and graphics to get recreated
        //            NextNavBtn.DestroyGfxCache();
        //            NextNavBtn.PrepForPaint(true);
                    
        //            NextNavBtn.Visible = true;
        //        }
        //    }
        //    this.TitleBarCaption.Text = this.TitleBarRawCaption;
        //    this.NavigationPanel.Visible = true;
        //}
        //#endregion

        //#region Printer Support

        //public int WriteFieldValuesToPrintPicture()
        //{
        //    TWinBasePrnData loPrnData;
        //    short loNdx;
        //    int loFieldNo;

        //    int loLoopMax = this.PrintPicture.AllPrnDataElements.Count;
        //    for (loNdx = 0; loNdx < loLoopMax; loNdx++)
        //    {
        //        loPrnData = this.PrintPicture.AllPrnDataElements[loNdx];
        //        if (loPrnData == null)
        //            continue;
        //        if ((loFieldNo = this.IssueStruct.MainTable.GetFldNo(loPrnData.Name)) < 0)
        //            continue;
        //        loPrnData.TextBuf = this.IssueStruct.MainTable.HighTableRevision.Tables[0].GetFormattedFieldData(loFieldNo, loPrnData.MaskForHH);
        //    }
        //    return 0;
        //}

        private int QueryUserLoadPaper()
        {
            // This routine is only applicable to X3 handhelds
#if WindowsCE && !__ANDROID__
            // ask the X3 monitor to load the paper - it will set an event for us when its done
            TWaitForPaperForm loWaitForm = new TWaitForPaperForm();
            loWaitForm.Line3.Text = "Use printer applet to load paper.";
            loWaitForm.Line4.Text = "(Applet will popup momentarily)";

            short loUserChoice = loWaitForm.FormEdit();
            if (loWaitForm != null)
                loWaitForm.Close();
            if (loUserChoice == FormEditResults.FormEditResult_OK) //FormEditResult_PaperLoaded)
                return 0;
            else
                return -1;
#else
            return 0;
#endif  // End of WindowsCE
        }

//        public int PrintForm(bool iFromForm)
//        {
//            // Make sure IssueDate & Time are updated before printing. This does nothing if form already printed or saved.
//            MainTimer_Tick(this.WinForm, new EventArgs());

//            // Copied from form save to make sure we log the issueno as soon as its printed 09/30/04 ajw 
//            // If this is a new entry and there is an issue number field, need to log this number as used
//            if ((iFromForm) && (IssueNoCtrl != null) && (!GetFormIssueNoLogged()))
//            {
//                Int64 loIssueNo = 0;
//                ReinoControls.TextBoxBehavior.StrTollInt(IssueNoCtrl.Behavior.EditBuffer, ref loIssueNo);
//                if (IssueStruct is TCiteStruct)
//                    this.AssocCiteStructLogic.GetSequence().LogUsedNumber(loIssueNo);
//                SetFormIssueNoLogged(true); // don't need to log again for 2nd copies 
//            }

//            // before we print, we'll save what we have into a temp file 
//            // this gives us a chance to recover if the unit crashes after printing 
//            // but prior to saving the record in the main table   09/30/04 ajw  
//            // and of course, if the've already saved, we don't need to 
//            if ((iFromForm) && (!GetFormTempFileSaved()) && (!GetFormSaved()))
//            {
//                if (this.IssueStruct is TCiteStruct)
//                    AssocCiteStructLogic.WriteRecordToRecoveryFile(this.CfgForm);
//                SetFormTempFileSaved(true); // we'll need to delete once the "real" table is updated
//            }

//            TIssPrnFormRev loPrintPic = null;
//            if (PrintPicture != null)
//            {
//                // Get the latest Print Picture revision
//                if (this.CfgIssForm.PrintPictureList[0].Revisions.Count > 0)
//                    loPrintPic = this.CfgIssForm.PrintPictureList[0].Revisions[this.CfgIssForm.PrintPictureList[0].Revisions.Count - 1];
//            }

//            if ((loPrintPic == null) || (loPrintPic.Height < 1) || loPrintPic.Children.Count == 0)
//            {
//                SetFormPrinted(true);
//                return 0;
//            }

//            AppMessageBox loMsgForm = new AppMessageBox();
//            loMsgForm.SetMsg("", "Printing...");
//            // Semi-Centered for better screen placement
//            /*
//            loMsgForm.Left = this.WinForm.Left + ((this.EntryPanel.Width - loMsgForm.Width) / 2) + this.EntryPanel.Left; // this.WinForm.Left + 5 + this.EntryPanel.Left;
//            loMsgForm.Top = this.WinForm.Top + ((this.EntryPanel.Height - loMsgForm.Height) / 2) + this.EntryPanel.Top; // this.WinForm.Top + 20 + this.EntryPanel.Top;
//            */
//            loMsgForm.Left = (((240 * 1/*ReinoControls.BasicButton.ScaleFactorAsInt*/) - loMsgForm.Width) / 2);
//            loMsgForm.Top = ((((240 - 25) * 1 /*ReinoControls.BasicButton.ScaleFactorAsInt*/) - loMsgForm.Height) / 2);

//            loMsgForm.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
//            loMsgForm.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
//            loMsgForm.Scale(ReinoControls.BasicButton.ScaleFactorSize);

//            loMsgForm.Visible = true;
//            loMsgForm.Show();
//#if !WindowsCE && !__ANDROID__   
//            // For full framework, we'll put in a pause so we can see whats happening
//            this.WinForm.Refresh();
//            loMsgForm.Refresh();
//            IssueAppImp.ApplicationDoEvents();
//            Thread.Sleep(250);
//#else
//            this.WinForm.Refresh();
//            loMsgForm.Refresh();
//            IssueAppImp.ApplicationDoEvents();
//#endif  // End of !WindowsCE

//            if (!iFromForm || !GetFormPrinted())
//            {
//                loMsgForm.SetMsgAndPaint("", "Printing:", "Preparing Data...", "", "");
//#if !WindowsCE && !__ANDROID__   
//                // For full framework, we'll put in a pause so we can see whats happening
//                Thread.Sleep(250);
//#endif // End of !WindowsCE
//                if (iFromForm)
//                    ReadFieldValuesFromForm(this.CfgForm, IssueStruct.MainTable.HighTableRevision.Tables[0]);
//                WriteFieldValuesToPrintPicture();


//                /*************************************************/
//                // DEBUG -- THIS IS HERE JUST FOR TESTING ONLY!!!!
//                /*
//                // Save the ticket to end of associated .DAT file
//                this.citeStruct.TableDefs[0].Revisions[0].Tables[0].WriteRecord();
//                // Log the used citation number
//                TObjBasePredicate predicate = new TObjBasePredicate(citeStruct.Sequence);
//                SequenceImp SeqObj =
//                    Reino.CommonLogic.SequenceManager.GlobalSequenceMgr.Sequences.Find(predicate.CompareByName_CaseInsensitive);
//                if (SeqObj != null)
//                {
//                    Int64 loIssueNo = SeqObj.GetNextNumber();
//                    SeqObj.LogUsedNumber(loIssueNo);
//                }
//                // Next ticket we write should use new number
//                this.SetIssueNoFields();
//                */
//                /*************************************************/


//                loPrintPic.Series3CE_ClearPrintCanvas(loPrintPic.Height, loPrintPic.Width, this.EntryPanel);
//                loPrintPic.PrepareForPrint();
//                loMsgForm.SetMsgAndPaint("", "Printing...", "", "", "");
//#if !WindowsCE && !__ANDROID__   
//                // For full framework, we'll put in a pause so we can see whats happening
//                Thread.Sleep(250);
//#endif  // End of !WindowsCE
//            } // if form hasn't printed yet, need to paint canvas

//            // print to physical paper
//            int loCopyCnt = 0;
//            for (; ; )
//            {
//                loPrintPic.Series3CE_ClearPrintCanvas(loPrintPic.Height, loPrintPic.Width, this.EntryPanel);
//                loPrintPic.PaintDescendants();

//#if WindowsCE

//#if PRINTER_LOOP_TEST_FOR_STEVE_B
//                int loTotalSleep = 0;
//                int loLoops = 0;     
//                int loSeconds = 45;  
//                for (; ; )           
//                {                    
//                    loLoops++;       
//                    loMsgForm.SetMsgAndPaint("", "Printing...", "Pages so far = " + loLoops.ToString(), "", "");  
//#endif // End of PRINTER_LOOP_TEST_FOR_STEVE_B

//                // DEBUG: Testing
//                Printer_QL320_PCX zebraPrinter = new Printer_QL320_PCX();
//                zebraPrinter.WriteToPrinter(loPrintPic.Height);

//                int loPrinterResult = 1; // WriteToSeries3CEPrinter(loPrintPic.Height);

//                // Delete the bitmap now that we're completely done with it
//                IPlatformDependentGDI WinGDI;
//                if (Environment.OSVersion.Platform == PlatformID.WinCE)
//                    WinGDI = new Reino.ClientConfig.WinCE_GDI();
//                else
//                    WinGDI = new Reino.ClientConfig.Win32_GDI();
//                WinGDI.DeleteObject(TWinPrnBase.glDrawBitmap);
//                WinGDI.DeleteDC(TWinPrnBase.glDrawBitmapHDC);

//                // A failure is indicated by -1 returned by WriteToSeries3CEPrinter function
//                if (loPrinterResult == -1)
//                {
//                    // print failed 'cuz out of paper, give user opportunity to load paper now.
//                    if ((loPrinterResult = QueryUserLoadPaper()) != 0)
//                    {
//                        loMsgForm.Close();
//                        if (loMsgForm != null)
//                            loMsgForm.Dispose();
//                        return 1;
//                    }
//                    continue;
//                }

//#if PRINTER_LOOP_TEST_FOR_STEVE_B
//                    loTotalSleep = 0;
//                    loSeconds = 45;  
//                    loMsgForm.SetMsgAndPaint("", "Printing...", "Pages so far = " + loLoops.ToString(), "Next in " + loSeconds.ToString() + " seconds", "");
//                    while (loTotalSleep < 45000)
//                    {                           
//                        Thread.Sleep(5000);     
//                        loTotalSleep += 5000;   
//                        loSeconds -= 5;         
//                        loMsgForm.SetMsgAndPaint("", "Printing...", "Pages so far = " + loLoops.ToString(), "Next in " + loSeconds.ToString() + " seconds", "");
//                        // Simulate some keystrokes to keep the unit from going to sleep 
//                        string loStringToSend = "  keep awake";          
//                        Reino.CommonLogic.SendKeys.Send(loStringToSend); 
//                    }
//                }
//#endif // End of PRINTER_LOOP_TEST_FOR_STEVE_B

//#endif // End of WindowsCE

//#if !WindowsCE && !__ANDROID__   
//                    // DEBUG -- Display Print Picture to user
//                PrintDocument printDoc = new PrintDocument();
//                PageSettings pgSettings = new PageSettings();
//                PrinterSettings prtSettings = new PrinterSettings();
//                printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);

//                AutoISSUE.PrintPreviewForm dlg = new AutoISSUE.PrintPreviewForm();
//                printDoc.DefaultPageSettings = pgSettings;
//                printDoc.PrinterSettings = prtSettings;
//                dlg.Document = printDoc;
//                dlg.Size = new Size(800, 600);
//                dlg.StartPosition = FormStartPosition.CenterScreen;
//                dlg.WindowState = FormWindowState.Maximized;
//                dlg.PrintPreviewControl.UseAntiAlias = true;
//                if (TWinPrnBase.OffscreenBitmapWin32.Height > 880)
//                    dlg.Zoom150();
//                // Can't do a real show modal because it'll interfere with our application's message loop
//                /*dlg.ShowDialog();*/
//                // Disable the application's main form until user is done with print preview
//                IssueAppImp.glMainMenuForm.Enabled = false;
//                dlg.Show();
//                while ((dlg != null) && (dlg.Visible))
//                {
//                    dlg.BringToFront();
//                    System.Threading.Thread.Sleep(80);
//                    IssueAppImp.ApplicationDoEvents();
//                }
//                if (dlg != null)
//                {
//                    try { dlg.Close(); }
//                    catch { }
//                }
//                if (printDoc != null)
//                {
//                    printDoc.Dispose();
//                    printDoc = null;
//                }

//                // Re-enable the applications main form
//                IssueAppImp.glMainMenuForm.Enabled = true;
//                IssueAppImp.glMainMenuForm.Refresh();

//                // Old print preview that sucked
//                /*
//                Form loPicView = new Form();
//                loPicView.WindowState = FormWindowState.Maximized;
//                Panel loPnl = new Panel();
//                loPnl.Parent = loPicView;
//                loPnl.Left = 400 - (int)(loPrintPic.Width / 2);
//                loPnl.Top = 0;
//                loPnl.Width = loPrintPic.Width + 32;
//                loPnl.Height = 700 + 32;
//                loPnl.Visible = true;
//                loPnl.AutoScroll = true;
//                PictureBox pb = new PictureBox();
//                pb.Parent = loPnl;
//                pb.Left = 0;
//                pb.Top = 0;
//                pb.Width = loPrintPic.Width;
//                pb.Height = loPrintPic.Height;
//                pb.Visible = true;
//                pb.Image = TWinPrnBase.OffscreenBitmapWin32;

//                // If an AppMessageBox shows up, we get stuck if this form is modal, so we gotta fake it a bit
//                // Show the form and just wait until it closes
//                loPicView.Show();
//                while ((loPicView != null) && (loPicView.Visible))
//                {
//                    System.Threading.Thread.Sleep(80);
//                    IssueAppImp.ApplicationDoEvents();
//                }
//                if (loPicView != null)
//                {
//                    try { loPicView.Close(); }
//                    catch { }
//                }
//                */

//                if (this.EntryPanel.Visible)
//                    ReturnFocusToActiveCtrl();
//                else
//                {
//                    try { vlbFields.Focus(); }
//                    catch { }
//                }

//                // Delete the bitmap now that we're completely done with it
//                TWinPrnBase.OffscreenBitmapWin32.Dispose();
//                TWinPrnBase.OffscreenBitmapWin32 = null;
//                // END DEBUG -- Display Print Picture to user
//#endif // End of !WindowsCE

//                // mcb v1.06, 10/1/03: accommodate form feeds in the print picture
//                int loPageNdx = 0;
//                int loPageTopLine = 0;
//                int loPrintResult = -1;

//                string loLine1 = "";
//                string loLine2 = "";

//                for (; ; )
//                { // print all pages in form

//                    // print this page
//                    // DEBUG -- Need to implement
//                    /*
//                    uHALr_FormatStr("Printing copy %d of %d", loLine1, 40, loCopyCnt + 1, fPrintCopyCnt + 1);
//                    uHALr_FormatStr("Page %d of %d", loLine2, 40, loPageNdx + 1, fPrintPicture->PageBreakCnt());
//                    loMsgForm.SetMsgAndPaint(loLine1, loLine2);
//#if !WindowsCE && !__ANDROID__   
//                    // For full framework, we'll put in a pause so we can see whats happening
//                    Thread.Sleep(250);
//#endif  // End of !WindowsCE

//                    loPrintResult = PrintCanvas(&fPrintCanvas[loPageTopLine * LTP_PrintLineBufSize], fPrintPicture->GetPageBreak(loPageNdx) - loPageTopLine);

//                    if (loPrintResult == 0)  // different drivers returning different codes...
//                    {
//                        // print failed 'cuz out of paper, give user opportunity to load paper now.
//                        TextBoxBehavior.RingBell();

//                        if (QueryUserLoadPaper() != 0)
//                        {
//                            delete loMsgForm;
//                            //PaintDescendants();
//                            return 1;
//                        }
//                        continue;
//                    }
//                    // end of prev page is top of next page, and on to next page...
//                    loPageTopLine = fPrintPicture->GetPageBreak(loPageNdx++);

//                    // on all but last page, give user the opportunity to tear the sheet off
//                    if (loPageNdx == fPrintPicture->PageBreakCnt()) 
//                    */
//                    break;
//                    // DEBUG -- Need to implement
//                    /*
//                   AppMessageBox.ShowMultiLineMessageWithBell("Tear sheet at perforation\r\nthen press OK to continue.", "");
//                    */
//                } // for... print all pages in form

//                if (loPrintResult != 0)
//                    break; // print failed, so bail

//                // DEBUG -- Need to implement
//                /*
//                loCopyCnt++;
//                if (loCopyCnt > fPrintCopyCnt) 
//                */
//                break;

//                // on all but last copy, give user the opportunity to tear the sheet off
//                // DEBUG -- Need to implement
//                //MessageBox.Show("Tear sheet at perforation", "then press OK to continue.");
//            } // for loop to print all the copies

//            // mark all printed fields as printed and disabled them. 
//            loMsgForm.SetMsgAndPaint("", "Printing: Mark", "Printed Fields...", "", "");
//#if !WindowsCE && !__ANDROID__   
//            // For full framework, we'll put in a pause so we can see whats happening
//            Thread.Sleep(250);
//#endif  // End of !WindowsCE

//            if (iFromForm)
//            {
//                MarkPrintedFields(true);
//                SetFormPrinted(true);
//                // suppress the "FirstEditFocus" event
//                fFormEditAttrs |= EditRestrictionConsts.feaEditedFirstField;
//            }

//            loMsgForm.Close();
//            if (loMsgForm != null)
//                loMsgForm.Dispose();
//            return 0;
//        }

#if !WindowsCE && !__ANDROID__   
        //private void printDoc_PrintPage(Object sender, PrintPageEventArgs e)
        //{
        //    float leftMargin = e.MarginBounds.Left;
        //    float topMargin = e.MarginBounds.Top;
        //    int loWidth = TWinPrnBase.OffscreenBitmapWin32.Width;
        //    int loHeight = TWinPrnBase.OffscreenBitmapWin32.Height;
        //    if (loHeight > 880)
        //    {
        //        loHeight = (int)(loHeight / 2);
        //        loWidth = (int)(loWidth / 2);
        //        topMargin = (int)(topMargin / 2);
        //        leftMargin = leftMargin * 2;
        //    }
        //    Rectangle destRect = new Rectangle(Convert.ToInt32(leftMargin), Convert.ToInt32(topMargin),
        //        loWidth, loHeight);
        //    GraphicsUnit units = GraphicsUnit.Pixel;
        //    e.Graphics.DrawImage(TWinPrnBase.OffscreenBitmapWin32, destRect, 0, 0,
        //        TWinPrnBase.OffscreenBitmapWin32.Width, TWinPrnBase.OffscreenBitmapWin32.Height, units);
        //    e.HasMorePages = false;
        //}
#endif  // End of !WindowsCE

#if WindowsCE  // this is all WindowsCE specific, no ANDROID code within
        [DllImport("coredll.dll", EntryPoint = "CreateFile", SetLastError = true)]
        internal static extern int CreateFileCE(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            int lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            int hTemplateFile);

        [DllImport("coredll.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
        internal static extern int DeviceIoControlCE(
            IntPtr hDevice,
            uint dwIoControlCode,
            byte[] lpInBuffer,
            int nInBufferSize,
            byte[] lpOutBuffer,
            int nOutBufferSize,
            ref int lpBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("coredll.dll", EntryPoint = "WriteFile", SetLastError = true)]
        internal static extern int WriteFileCE(
            IntPtr hFile,
            Byte[] lpBuffer,
            int nNumberOfBytesToWrite,
            ref int lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        [DllImport("coredll.dll", EntryPoint = "ReadFile", SetLastError = true)]
        internal static extern int ReadFileCE(
            IntPtr hFile,
            byte[] lpBuffer,
            int nNumberOfBytesToRead,
            ref int lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return ((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method);
        }

        int WriteToSeries3CEPrinter(int iDotHeight)
        {
            // Local Variables
            int loBytesWritten = 0;
            int loBytesToPrint = 0;
            const uint GENERIC_READ = 0x80000000;
            const uint GENERIC_WRITE = 0x40000000;
            const uint CREATE_NEW = 1;
            const uint CREATE_ALWAYS = 2;
            const uint OPEN_EXISTING = 3;
            const uint OPEN_ALWAYS = 4;
            const uint FILE_ATTRIBUTE_NORMAL = 0;

            // Open the printer
            IntPtr hFile = (IntPtr)CreateFileCE("S3P1:", (uint)(GENERIC_READ | GENERIC_WRITE),
                (uint)0, 0, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, 0);

            // Write the bitmap to file. When writing to file will send number of
            // bytes in data to WriteFile. When writing to printer will just send
            // number of vertical lines.
            loBytesToPrint = (104 * iDotHeight); // iDotHeight; 

            // Get platform dependant object we will use for calling a Windows API functions
            IPlatformDependentGDI WinGDI;
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
                WinGDI = new Reino.ClientConfig.WinCE_GDI();
            else
                WinGDI = new Reino.ClientConfig.Win32_GDI();

            // Write data to printer
            // Allocate memory for a copy of bitmap bits
            byte[] RealBits = new byte[104 * iDotHeight]; // 832 bits wide divided by 8 bits/byte = 104 bytes
            // And grab bits from DIBSection data
            Marshal.Copy(TWinPrnBase.glDrawBitmapDataPtr, RealBits, 0, 104 * iDotHeight);

            bool loResult = false;
            try
            {
                // If writing to file, we would use (104 * iDotHeight), but when going to printer,
                // we send number of vertical lines
                loBytesToPrint = iDotHeight; // (104 * iDotHeight);
                int loFuncRet = WriteFileCE(hFile, RealBits, loBytesToPrint, ref loBytesWritten, IntPtr.Zero);
                loResult = Convert.ToBoolean(loFuncRet);
                if ((loResult == false) || (loBytesWritten <= 0))
                {
                    loBytesWritten = -1;
                    int loLastError = Marshal.GetLastWin32Error();
                    throw new System.ComponentModel.Win32Exception(loLastError);
                }
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex.Message);
                Debug.WriteLine("Type: " + Ex.GetType().ToString());
                if (Ex.InnerException != null)
                    Debug.WriteLine("Inner: " + Ex.InnerException.Message);
                Debug.WriteLine("StackTrace: " + Ex.StackTrace);
            }

            try
            {
                // Now we need to send the printer a formfeed. (We have to get a native handle first)
                // Don't send the formfeed if we had an error above (probably due to out-of-paper)
                if ((loResult == true) && (loBytesWritten > 0))
                {
                    int loBytesReturned = 0;
                    uint IOCTL = CTL_CODE(32768, 2053, 0, 0); //IOCTL_PRINTER_FF
                    DeviceIoControlCE(hFile, IOCTL, null, 0, null, 0, ref loBytesReturned, IntPtr.Zero);
                    CloseHandle(hFile);
                }
                else
                {
                    CloseHandle(hFile);
                }
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex.Message);
                Debug.WriteLine("Type: " + Ex.GetType().ToString());
                if (Ex.InnerException != null)
                    Debug.WriteLine("Inner: " + Ex.InnerException.Message);
                Debug.WriteLine("StackTrace: " + Ex.StackTrace);
            }

            // Return number of bytes printed
            return loBytesWritten;
        }
#endif // End of WindowsCE

        #endregion

        #region List Synchronization & Data Events

        protected virtual void Behavior_NotifiedDependentsParentDataChanged(object sender, EventArgs e)
        {
            return;
        }

        //protected virtual void Behavior_TextChanged(object sender, EventArgs e)
        //{
        //    // Try to find an associated "Popular Item" radio list
        //    ReinoControls.TextBoxBehavior behavior = sender as ReinoControls.TextBoxBehavior;
        //    ReinoRadioList radioList = null;
        //    for (int loIdx = PopularLists.Count - 1; loIdx >= 0; loIdx--)
        //    {
        //        if (PopularLists[loIdx].Behavior == behavior)
        //        {
        //            radioList = PopularLists[loIdx];
        //            break;
        //        }
        //    }

        //    // Did we find an associated "Popular Item" radio list?
        //    if (radioList != null)
        //    {
        //        // Lets give the popular list priority if the current text is empty
        //        if (behavior.GetText() == "")
        //            PopularListHasPriority = true;

        //        // Try to find radiolist index that matches text of associated ReinoTextBox
        //        int loLoopMax = radioList.Count;
        //        for (int loIdx = 0; loIdx < loLoopMax; loIdx++)
        //        {
        //            // If we found a match, set the selected index and get out
        //            if (radioList.GetTextForIndex(loIdx) == behavior.GetText())
        //            {
        //                radioList.SelectedIndexWithoutChangeEvent = loIdx;
        //                return;
        //            }
        //        }
        //        // If we get this far, there is no match, so deselect all items
        //        radioList.SelectedIndexWithoutChangeEvent = -1;
        //    }

        //    // When the IssueNo or IssueNo_Display field on a IssueSelectForm changes, 
        //    // we need to update the button states.
        //    // This means we need to also repaint the virtual list box if its showing
        //    if ((this is IssueSelectFormLogic) && (behavior.CfgCtrl != null))
        //    {
        //        if ((behavior.CfgCtrl == IssueNoCtrl) || 
        //            (behavior.CfgCtrl == IssueNoDisplayCtrl) ||
        //            (behavior.CfgCtrl == ControlNoCtrl))
        //        {
        //            // Datatable needs to be explicitly moved to correct record before refreshing button states
        //            behavior.ResynchListNdx();
        //            this.SetButtonStates();
        //            if ((this.NavigationPanel.Visible == true) && (behavior.GetText() != ""))
        //                this.vlbFields.Invalidate();
        //        }
        //    }
        //}

        //void Behavior_OnCreatedPopupListBox(ReinoTextBox.PopupListBox NewPopup)
        //{
        //    NewPopup.TrapAllNavigationKeys = true;
        //    NewPopup.OnNavigationKeyPress += new ReinoVirtualListBox.NavigationKeyPress(PopupList_OnNavigationKeyPress);
        //    NewPopup.SelectedIndexChanged += new EventHandler(PopupList_SelectedIndexChanged);
        //}

        //void Behavior_OnCreatedRegularListBox(ReinoVirtualListBox NewListBox)
        //{
        //    NewListBox.TrapAllNavigationKeys = true;
        //    NewListBox.OnNavigationKeyPress += new ReinoVirtualListBox.NavigationKeyPress(StaticList_OnNavigationKeyPress);
        //    NewListBox.SelectedIndexChanged += new EventHandler(StaticList_SelectedIndexChanged);

        //    if (NewListBox is ReinoTextBox.StaticListBox)
        //    {
        //        (NewListBox as ReinoTextBox.StaticListBox).IsClaimed = true;
        //        ReusableStaticListBoxes.Add(NewListBox as ReinoTextBox.StaticListBox);
        //    }
        //    else if (NewListBox is ReinoRadioList)
        //    {
        //        (NewListBox as ReinoRadioList).IsClaimed = true;
        //        ReusableRadioListBoxes.Add(NewListBox as ReinoRadioList);
        //    }
        //}

        //ReinoVirtualListBox Behavior_OnGetNewRegularListBox(ReinoControls.ListBoxStyle ListStyle, ref bool IsNewlyCreated)
        //{
        //    if (ListStyle == ListBoxStyle.lbsStatic)
        //    {
        //        // Lets see if there is an unclaimed static listbox we can reuse
        //        foreach (ReinoTextBox.StaticListBox NextListBox in ReusableStaticListBoxes)
        //        {
        //            if (NextListBox.IsClaimed == false)
        //            {
        //                NextListBox.IsClaimed = true;
        //                // Turn off "IsNewlyCreated" flag so additonal event handlers don't get added again by the behavior object
        //                IsNewlyCreated = false;
        //                return NextListBox;
        //            }
        //        }

        //        // Nothings unclaimed at this point, so we'll create a new one
        //        ReinoTextBox.StaticListBox NewListBox = new ReinoTextBox.StaticListBox(false);
        //        ReusableStaticListBoxes.Add(NewListBox);
        //        NewListBox.IsClaimed = true;
        //        NewListBox.TrapAllNavigationKeys = true;
        //        NewListBox.OnNavigationKeyPress += new ReinoVirtualListBox.NavigationKeyPress(StaticList_OnNavigationKeyPress);
        //        NewListBox.SelectedIndexChanged += new EventHandler(StaticList_SelectedIndexChanged);
        //        // Turn on "IsNewlyCreated" flag so additional event handlers are added by the behavior object
        //        IsNewlyCreated = true;
        //        return NewListBox;
        //    }
        //    else if (ListStyle == ListBoxStyle.lbsRadio)
        //    {
        //        // Lets see if there is an unclaimed static listbox we can reuse
        //        foreach (ReinoRadioList NextListBox in ReusableRadioListBoxes)
        //        {
        //            if (NextListBox.IsClaimed == false)
        //            {
        //                NextListBox.IsClaimed = true;
        //                // Turn off "IsNewlyCreated" flag so additonal event handlers don't get added again by the behavior object
        //                IsNewlyCreated = false;
        //                return NextListBox;
        //            }
        //        }

        //        // Nothings unclaimed at this point, so we'll create a new one
        //        ReinoRadioList NewListBox = new ReinoRadioList(false);
        //        ReusableRadioListBoxes.Add(NewListBox);
        //        NewListBox.IsClaimed = true;
        //        NewListBox.TrapAllNavigationKeys = true;
        //        NewListBox.OnNavigationKeyPress += new ReinoVirtualListBox.NavigationKeyPress(StaticList_OnNavigationKeyPress);
        //        NewListBox.SelectedIndexChanged += new EventHandler(StaticList_SelectedIndexChanged);
        //        // Turn on "IsNewlyCreated" flag so additional event handlers are added by the behavior object
        //        IsNewlyCreated = true;
        //        return NewListBox;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //void Behavior_OnCtrlGotFocus(object sender)
        //{
        //    // Retain currently cfg control before possibly setting new one
        //    TTControl loPrevCfgCtrl = CurrentCfgCtrl;

        //    if (sender is ReinoTextBox)
        //    {
        //        // For better focus cue, change to bold font if not already set
        //        ReinoTextBox loTextBox = sender as ReinoTextBox;
        //        if (loTextBox != null)
        //        {
        //            if ((loTextBox.Behavior.FocusedFont != null) && (loTextBox.Font != loTextBox.Behavior.FocusedFont))
        //                loTextBox.Font = loTextBox.Behavior.FocusedFont;
        //            if ((loTextBox.Behavior.PromptLabel != null) && (loTextBox.Behavior.PromptLabel.Font != loTextBox.Behavior.FocusedFont))
        //            {
        //                loTextBox.Behavior.PromptLabel.Font = loTextBox.Behavior.FocusedFont;
        //                loTextBox.Behavior.PromptLabel.BackColor = Color.Yellow;
        //            }
        //        }

        //        // Don't need to redo if already current
        //        if (CurrentCfgCtrl == (sender as ReinoTextBox).Behavior.CfgCtrl)
        //            return;

        //        // Make the current cfg ctrl the one passed to us
        //        CurrentCfgCtrl = (sender as ReinoTextBox).Behavior.CfgCtrl;

        //        // Keep the current control index up-to-date with the passed control
        //        int CfgCtrlIndex = GetCfgCtrlIndex(CurrentCfgCtrl);
        //        if (CfgCtrlIndex != -1)
        //        {
        //            CurrentCfgCtrlIdx = CfgCtrlIndex;
        //        }

        //        // When a textbox is focused, we want its associated static or radio listbox visible too.
        //        // But we must also ensure its up-to-date!
        //        if (loTextBox != null)
        //        {
        //            TTEdit loCfgCtrl = loTextBox.Behavior.CfgCtrl;
        //            if ((loCfgCtrl != null) &&
        //                (loCfgCtrl.WinCtrl != null) &&
        //                (loCfgCtrl.WinCtrl.Enabled) && (loCfgCtrl.WinCtrl.Visible) &&
        //                (loCfgCtrl.WinCtrl is ReinoTextBox) &&
        //                (loCfgCtrl.Behavior.ListBox != null) &&
        //                (loCfgCtrl.Behavior.ListBox.Visible == false) &&
        //                ((loCfgCtrl.Behavior.ListStyle == ReinoControls.ListBoxStyle.lbsStatic) ||
        //                (loCfgCtrl.Behavior.ListStyle == ReinoControls.ListBoxStyle.lbsRadio)))
        //            {
        //                loCfgCtrl.Behavior.ListBox.BringToFront();
        //                // Don't make it visible if the edit control is disabled or invisible
        //                // Also, the NotesMemo field will never show the listbox.
        //                // Also, don't show regular list if its the same contents as a popular list.
        //                if (((loCfgCtrl is TTEdit) && ((loCfgCtrl as TTEdit).IsNotesMemo == true)) ||
        //                    ((this.Popular != null) && (this.Popular.Count == loCfgCtrl.Behavior.ListBox.Count)))
        //                {
        //                    SetListBoxVisible(loCfgCtrl.Behavior.ListBox, false);
        //                }
        //                else
        //                {
        //                    SetListBoxVisible(loCfgCtrl.Behavior.ListBox, (loTextBox.Enabled & loTextBox.Visible));
        //                    // Do the refresh items which will finalize offscreen painting buffer,
        //                    // and vertical scrollbar, etc...
        //                    loCfgCtrl.Behavior.ListBox.StopDrawing = false;
        //                    loCfgCtrl.Behavior.ListBox.ResizeSuspended = false;
        //                    loCfgCtrl.Behavior.ListBox.RefreshItems(false);
        //                }
        //                loCfgCtrl.Behavior.ListBox.Enabled = loTextBox.Enabled;
        //                // Clear flag to indicate list has not been changed by a restriction at this point
        //                loCfgCtrl.Behavior.ListContentsChangedByRestriction = false;
        //            }
        //            else if ((loCfgCtrl != null) && (loCfgCtrl.WinCtrl == null))
        //            {
        //                Debug.WriteLine("Behavior_OnCtrlGotFocus: locfgCtrl.WinCtrl is null when not expected to be");
        //            }

        //        }
        //    }
        //    else if ((sender is ReinoNavButton) && ((sender as ReinoNavButton).AssocCfgBtn != null))
        //    {
        //        // Make the current cfg ctrl the one passed to us
        //        CurrentCfgCtrl = (sender as ReinoNavButton).AssocCfgBtn;

        //        // Keep the current control index up-to-date with the passed control
        //        int CfgCtrlIndex = GetCfgCtrlIndex(CurrentCfgCtrl);
        //        if (CfgCtrlIndex != -1)
        //        {
        //            CurrentCfgCtrlIdx = CfgCtrlIndex;
        //        }
        //    }
        //    else if ((sender is ReinoVirtualListBox) && ((sender as ReinoVirtualListBox).Behavior != null))
        //    {
        //        // Make the current cfg ctrl the one passed to us
        //        CurrentCfgCtrl = (sender as ReinoVirtualListBox).Behavior.CfgCtrl;

        //        // Keep the current control index up-to-date with the passed control
        //        int CfgCtrlIndex = GetCfgCtrlIndex(CurrentCfgCtrl);
        //        if (CfgCtrlIndex != -1)
        //        {
        //            CurrentCfgCtrlIdx = CfgCtrlIndex;
        //        }
        //    }
        //    else if ((sender is ReinoDrawBox) && ((sender as ReinoDrawBox).Behavior != null))
        //    {
        //        // Make the current cfg ctrl the one passed to us
        //        CurrentCfgCtrl = (sender as ReinoDrawBox).Behavior.CfgCtrl;

        //        // Keep the current control index up-to-date with the passed control
        //        int CfgCtrlIndex = GetCfgCtrlIndex(CurrentCfgCtrl);
        //        if (CfgCtrlIndex != -1)
        //        {
        //            CurrentCfgCtrlIdx = CfgCtrlIndex;
        //        }
        //    }
        //    else
        //    {
        //        System.Diagnostics.Debug.WriteLine("Type not implemented in Behavior_OnCtrlGotFocus(): " + sender.GetType().ToString());
        //    }

        //    // Now do a lost focus event for the previous control if we have changed to a new one
        //    // (We want to do this manually so it is fired AFTER this GotFocus event!)
        //    if ((loPrevCfgCtrl != null) && (loPrevCfgCtrl != CurrentCfgCtrl) &&
        //        (loPrevCfgCtrl.TypeOfWinCtrl() == typeof(ReinoTextBox)))
        //    {
        //        Behavior_AfterLostFocus(loPrevCfgCtrl.Behavior);
        //    }
        //}

        //void Behavior_AfterLostFocus(object sender)
        //{
        //    try
        //    {
        //        ReinoTextBox loTextBox = null;
        //        if (sender is ReinoTextBox)
        //            loTextBox = sender as ReinoTextBox;
        //        else if (sender is TextBoxBehavior)
        //            loTextBox = ((sender as TextBoxBehavior).CfgCtrl.WinCtrl as ReinoTextBox);

        //        if (loTextBox != null)
        //        {
        //            // Change back to normal font if we have moved to a different field
        //            if ((loTextBox != null) && (CurrentCfgCtrl != null) && (CurrentCfgCtrl != loTextBox.Behavior.CfgCtrl))
        //            {
        //                if ((loTextBox.Behavior.NormalFont != null) && (loTextBox.Font != loTextBox.Behavior.NormalFont))
        //                    loTextBox.Font = loTextBox.Behavior.NormalFont;
        //                if ((loTextBox.Behavior.PromptLabel != null) && (loTextBox.Behavior.PromptLabel.Font != loTextBox.Behavior.NormalFont))
        //                {
        //                    // If current control doesn't have its own prompt label, then we don't need to change this one?
        //                    if ((CurrentCfgCtrl.Behavior == null) || (CurrentCfgCtrl.Behavior.PromptLabel != null))
        //                    {
        //                        loTextBox.Behavior.PromptLabel.Font = loTextBox.Behavior.NormalFont;
        //                        loTextBox.Behavior.PromptLabel.BackColor = SystemColors.Control;
        //                    }
        //                }
        //            }

        //            // When no longer focused and its not the current control, 
        //            // our static or radio list boxes should not be visible
        //            if ((loTextBox != null) && (CurrentCfgCtrl != null) && (CurrentCfgCtrl != loTextBox.Behavior.CfgCtrl))
        //            {
        //                TTEdit loCfgCtrl = loTextBox.Behavior.CfgCtrl;
        //                if ((loCfgCtrl != null) &&
        //                    (loCfgCtrl.Behavior.ListBox != null) &&
        //                    ((loCfgCtrl.Behavior.ListStyle == ReinoControls.ListBoxStyle.lbsStatic) ||
        //                    (loCfgCtrl.Behavior.ListStyle == ReinoControls.ListBoxStyle.lbsRadio)))
        //                {
        //                    SetListBoxVisible(loCfgCtrl.Behavior.ListBox, false);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        Debug.WriteLine("Error in Behavior_AfterLostFocus: " + Ex.Message);
        //    }
        //}

        //void Popular_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ReinoControls.ReinoRadioList radioList = ((ReinoControls.ReinoRadioList)(sender));

        //    // If the user made a valid selection, make the TextBox's text match the selected item
        //    if ((radioList.SelectedIndex >= 0) && (radioList.SelectedIndex < radioList.Count))
        //        radioList.Behavior.SetEditBufferAndPaint(radioList.GetTextForIndex(radioList.SelectedIndex));
        //}

        //string Popular_GetVirtualTextForIndex(object sender, GetVirtualTextEventArgs e)
        //{
        //    // We can't do this if the sender wasn't a ReinoRadioList
        //    if (!(sender is ReinoControls.ReinoRadioList))
        //        return "";

        //    ReinoRadioList RadioList = ((ReinoControls.ReinoRadioList)(sender));

        //    // Just return empty string if table, fieldname or row index is bad
        //    if (RadioList.Behavior.PopularListSourceTable == null) return "";

        //    // Find field index
        //    TObjBasePredicate predicate = new TObjBasePredicate(RadioList.Behavior.ListSourceFieldName);
        //    int loFldIdx = RadioList.Behavior.PopularListSourceTable.fTableDef.HighTableRevision.Fields.FindIndex(predicate.CompareByName_CaseInsensitive);

        //    if (loFldIdx == -1) return "";
        //    if ((e.Index < 0) || (e.Index >= RadioList.Behavior.PopularListSourceTable.GetRecCount())) return "";

        //    // Get field value from the table. Note that if the field is virtual, we
        //    // must get it via GetFormattedFieldData instead of from fFieldValues
        //    RadioList.Behavior.PopularListSourceTable.ReadRecord(e.Index);
        //    string FieldData;
        //    TTableFldDef loFld = RadioList.Behavior.PopularListSourceTable.fTableDef.GetField(loFldIdx);
        //    if (loFld is TTableVirtualFldDef)
        //        FieldData = RadioList.Behavior.PopularListSourceTable.GetFormattedFieldData(loFldIdx, "");
        //    else
        //        FieldData = RadioList.Behavior.PopularListSourceTable.fFieldValues[loFldIdx];

        //    return FieldData;
        //}

        //string Popular_GetVirtualDisplayTextForIndex(object sender, GetVirtualTextEventArgs e)
        //{
        //    // We can't do this if the sender wasn't a ReinoRadioList
        //    if (!(sender is ReinoControls.ReinoRadioList))
        //        return "";

        //    ReinoRadioList RadioList = ((ReinoControls.ReinoRadioList)(sender));

        //    // Just return empty string if table, fieldname or row index is bad
        //    if (RadioList.Behavior.PopularListSourceTable == null) return "";

        //    // Is there grid info to use?
        //    if ((RadioList.Behavior.CfgCtrl != null) && (RadioList.Behavior.CfgCtrl is TEditField) &&
        //        ((RadioList.Behavior.CfgCtrl as TEditField).DBListGrid != null) &&
        //        ((RadioList.Behavior.CfgCtrl as TEditField).DBListGrid.Columns.Count > 0))
        //    {
        //        TTDBListBox loGrid = (RadioList.Behavior.CfgCtrl as TEditField).DBListGrid;
        //        // If there is no associated grid in the configuration, we can't do anything else
        //        if (loGrid == null)
        //            return "";

        //        // Bring the table to the desired index
        //        RadioList.Behavior.ListSourceTable.ReadRecord(e.Index);

        //        string loResult = "";
        //        foreach (TGridColumnInfo Column in loGrid.Columns)
        //        {
        //            string FieldName = Column.Name;
        //            int loFldIdx = RadioList.Behavior.ListSourceTable.fTableDef.GetFldNo(FieldName);
        //            if ((loFldIdx >= 0) && (loFldIdx < RadioList.Behavior.ListSourceTable.fTableDef.HighTableRevision.Fields.Count))
        //            {
        //                if (loResult != "")
        //                    loResult += "\t";

        //                // Get field value from the table. Note that if the field is virtual, we
        //                // must get it via GetFormattedFieldData instead of from fFieldValues
        //                TTableFldDef loFld = RadioList.Behavior.ListSourceTable.fTableDef.GetField(loFldIdx);
        //                if (loFld is TTableVirtualFldDef)
        //                    loResult += RadioList.Behavior.ListSourceTable.GetFormattedFieldData(loFldIdx, Column.Mask);
        //                else
        //                    loResult += RadioList.Behavior.ListSourceTable.GetFormattedFieldData(loFldIdx, Column.Mask); // fFieldValues[loFldIdx];
        //            }
        //        }
        //        return loResult;
        //    }
        //    else
        //    {
        //        // Just use the normal method
        //        return Popular_GetVirtualTextForIndex(sender, e);
        //    }
        //}

        //string DataGrid_GetVirtualTextForIndex(object sender, GetVirtualTextEventArgs e)
        //{
        //    ReinoVirtualListBox loListBox = sender as ReinoVirtualListBox;
        //    TEditListBox loCfgListBox = loListBox.Behavior.CfgCtrl as TEditListBox;
        //    TTDBListBox loGrid = loCfgListBox.DBListGrid;
        //    // If there is no associated grid in the configuration, we can't do anything else
        //    if (loGrid == null)
        //        return "";

        //    // Bring the table to the desired index
        //    loListBox.Behavior.ListSourceTable.ReadRecord(e.Index);

        //    string loResult = "";
        //    foreach (TGridColumnInfo Column in loGrid.Columns)
        //    {
        //        string FieldName = Column.Name;
        //        int loFldIdx = loListBox.Behavior.ListSourceTable.fTableDef.GetFldNo(FieldName);
        //        if ((loFldIdx >= 0) && (loFldIdx < loListBox.Behavior.ListSourceTable.fTableDef.HighTableRevision.Fields.Count))
        //        {
        //            if (loResult != "")
        //                loResult += "\t";
        //            // Get field value from the table. Note that if the field is virtual, we
        //            // must get it via GetFormattedFieldData instead of from fFieldValues
        //            TTableFldDef loFld = loListBox.Behavior.ListSourceTable.fTableDef.GetField(loFldIdx);
        //            if (loFld is TTableVirtualFldDef)
        //                loResult += loListBox.Behavior.ListSourceTable.GetFormattedFieldData(loFldIdx, Column.Mask);
        //            else
        //                loResult += loListBox.Behavior.ListSourceTable.GetFormattedFieldData(loFldIdx, Column.Mask); // fFieldValues[loFldIdx];
        //        }
        //    }
        //    return loResult;
        //}

        //void DataGrid_OnMeasureHeader(object sender, MeasureItemEventArgs e)
        //{
        //    ReinoVirtualListBox loListBox = sender as ReinoVirtualListBox;
        //    e.ItemHeight = loListBox.ItemHeight;
        //}

        //void StaticList_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ReinoVirtualListBox listbox = ((ReinoVirtualListBox)(sender));
        //    // If the user made a valid selection, make the TextBox's text match the selected item
        //    if ((listbox.SelectedIndex >= 0) && (listbox.SelectedIndex < listbox.Count))
        //        listbox.Behavior.SetEditBufferAndPaint(listbox.GetTextForIndex(listbox.SelectedIndex));
        //}

        void PopupList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // For a popup list we don't want to automatically update the associated textbox 
            // when the index changes as we would with a static listbox.
            // (This will allow user to press [ESC] to cancel changes to field)
            /*
            ReinoVirtualListBox listbox = ((ReinoVirtualListBox)(sender));
            // If the user made a valid selection, make the TextBox's text match the selected item
            if ((listbox.SelectedIndex >= 0) && (listbox.SelectedIndex < listbox.Count))
                listbox.Behavior.SetEditBufferAndPaint(listbox.GetTextForIndex(listbox.SelectedIndex));
             */
        }

        //void FieldListBox_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ReinoNavButton ActiveSectionBtn = FindSectionBtnForFld(vlbFields.SelectedIndex);
        //    if (ActiveSectionBtn != null)
        //    {
        //        SetActiveSectionButton(ActiveSectionBtn);
        //        ScrollSectionBtnIntoView(ActiveSectionBtn);
        //    }
        //}

//#if WindowsCE || __ANDROID__
//        void FieldListBox_OnMeasureItem(object sender, ReinoControls.MeasureItemEventArgs e)
//#else
//        void FieldListBox_OnMeasureItem(object sender, MeasureItemEventArgs e)
//#endif
//        {
//            // Initially set the default item height and width
//            e.ItemHeight = (14 * ReinoControls.BasicButton.ScaleFactorAsInt);
//            e.ItemWidth = vlbFields.ItemWidth;

//            // Is there a button referenced by the passed index?
//            // If so, set the height equal to button height plus a little bit for margin space.
//            // Also, if the row is shared with another item, the width will be one half.
//            if ((e.Index >= 0) && (e.Index < EntryOrder.Count) && (EntryOrder[e.Index] is TTButton))
//            {
//                if (((TTButton)(EntryOrder[e.Index])).Visible == false)
//                {
//                    e.ItemHeight = 0;
//                    e.ItemWidth = 0;
//                }
//                else
//                {
//                    e.ItemHeight = SectionBtnHeight + (6 * ReinoControls.BasicButton.ScaleFactorAsInt);
//                    if (((TTButton)(EntryOrder[e.Index])).SharesRowWithIdx != -1)
//                        e.ItemWidth = (vlbFields.ItemWidth / 2);
//                }
//            }

//            // Finally, lets see if its a hidden field. If so, set the height/width to zero
//            if ((e.Index >= 0) && (e.Index < EntryOrder.Count) && (EntryOrder[e.Index] is TTEdit))
//            {
//                if (EntryOrder[e.Index].TypeOfWinCtrl() == typeof(ReinoTextBox))
//                {
//                    if (EntryOrder[e.Index].IsHidden == true)
//                    {
//                        e.ItemHeight = 0;
//                        e.ItemWidth = 0;
//                    }
//                }
//            }
//        }

//        int FieldListBox_OnSharesRowWithIndex(int Index)
//        {
//            // Exit if the index is invalid
//            if ((Index < 0) || (Index >= EntryOrder.Count))
//                return -1;

//            // Get config object at specified index and return the SharesRowWithIdx value
//            // if the object is a TTButton
//            TTControl Ctrl = EntryOrder[Index];
//            if (Ctrl is TTButton)
//                return ((TTButton)(Ctrl)).SharesRowWithIdx;

//            // Anything else isn't shared, so return -1
//            return -1;
//        }

        //private string FieldListBox_GetVirtualTextForIndex(object sender, ReinoControls.GetVirtualTextEventArgs e)
        //{
        //    // We can't do this if the sender wasn't a ReinoVirtualListBox
        //    if (!(sender is ReinoControls.ReinoVirtualListBox))
        //        return "";

        //    // Make sure the passed index is valid before trying to access item
        //    if ((e.Index >= 0) && (e.Index < EntryOrder.Count))
        //    {
        //        if (EntryOrder[e.Index] is TTEdit)
        //        {
        //            if ((((TTEdit)(EntryOrder[e.Index])).PromptWin != null) &&
        //                (((TTEdit)(EntryOrder[e.Index])).PromptWin.TextBuf != ""))
        //                return ((TTEdit)(EntryOrder[e.Index])).PromptWin.TextBuf;
        //            else
        //                return ((TTEdit)(EntryOrder[e.Index])).Name;
        //        }
        //        else if (EntryOrder[e.Index] is TTButton)
        //        {
        //            return ((TTButton)(EntryOrder[e.Index])).TextBuf;
        //        }
        //        else
        //        {
        //            // Unexpected Class, so return empty string
        //            return "";
        //        }
        //    }
        //    else
        //    {
        //        // Invalid index, so return empty string
        //        return "";
        //    }
        //}

//        private void OnListContentsChangedByRestriction(TextBoxBehavior iBehavior)
//        {
//            iBehavior.ListContentsChangedByRestriction = true;
//        }
//        #endregion

//        int ReadReinoMeter()
//        {
//            TReinoMeter fReinoMeter = new TReinoMeter();
//            fReinoMeter.GetEnforcementInfo(this);
//            return -1; // not implemented
//        }

//        /// <summary>
//        /// Called when user is attempting exit a form
//        /// If wireless searches are still pending, the user
//        /// is given a choice of abandoning them or continuing to wait
//        /// If they choose to wait, the result is -1 and the user 
//        /// should be prevented from exiting
//        /// </summary>
//        private int CleanUpPendingWirelessSearches(TTForm pSender, TSearchStruct iSearchStruct, bool iQuietly)
//        {
//            // Don't do wireless stuff for emulator anymore
//            bool DontWantWireless = false; 
//#if !WindowsCE && !__ANDROID__   
//            DontWantWireless = true;
//#endif  // End of !WindowsCE
//            // Just exit if we don't want to do wireless at all
//            if (DontWantWireless == true)
//            {
//                return 0;
//            }

//            TAPDIClientCommandRec loAPDIClientCommandRec;
//            TAPDIHotSheetSearchCommandRec loCmdRec;
//            // determine if we have any pending inquiries that need to be tossed
//            // have to be careful here. What if we toss it as the result arrives? We will
//            // need critical sections...
//            bool loWirelessSearchesPending = false;

//            // mcb 7/23/04: make this thread safe
//            lock (IssueAppImp.glWirelessQueue)
//            {

//                int loWhichQueueNdx;
//                // loop through twice, 1st w/ SendQue, then with receive queue

//                // MCB 4/15/2006: deleting the items here is not thread safe.  The WinsockThread does
//                // not keep the critical sections while it is invoking a web method.  Therefore,
//                // rather than delete items, we will set the "CancelledByUser" flag so that
//                // they will either not be sent or their result will be ignored.
//                for (loWhichQueueNdx = 1; loWhichQueueNdx >= 0; loWhichQueueNdx--)
//                {
//                    bool loIsSendQue;
//                    if (loWhichQueueNdx == 1)
//                        loIsSendQue = true;
//                    else
//                        loIsSendQue = false;
//                    for (int loSendQueNdx = 0; loSendQueNdx < IssueAppImp.glWirelessQueue.GetAPDIQueItemCnt(loIsSendQue); loSendQueNdx++)
//                    {
//                        // derefernce the ptr
//                        loAPDIClientCommandRec = IssueAppImp.glWirelessQueue.GetAPDIQueClientCommandRec(loIsSendQue, loSendQueNdx);
//                        if (loAPDIClientCommandRec == null)
//                            continue;

//                        // only want hotsheetsearch packets. Use our bad RTTI technique.
//                        if (loAPDIClientCommandRec.GetCommandNo() != TAPDIClientCommandRec.APDICommand_SearchHotsheet)
//                            continue;

//                        loCmdRec = (loAPDIClientCommandRec as TAPDIHotSheetSearchCommandRec);

//                        // if they supplied a form & the form doesn't match, skip it.
//                        if ((pSender != null) && (loCmdRec.GetFromForm() != pSender))
//                            continue;

//                        // if they supplied a search struct & they don't match, skip it
//                        if ((iSearchStruct != null) && (loCmdRec.GetSearchStruct() != iSearchStruct))
//                            continue;

//                        // OK. This one qualifies. Do we need to get user confirmation?
//                        if (!iQuietly && !loWirelessSearchesPending &&
//                            AppMessageBox.QueryUser("Wireless Searches Pending!", "Wait for a reply?"))
//                        {
//                            return -1; // user wants to wait.
//                        }
//                        // found at least one pending inquiry. If our flag is true, 
//                        // then user has already decided to cancel searches.
//                        loWirelessSearchesPending = true;
//                        loCmdRec.fCancelledByUser = true;
//                    }
//                }
//            }
//            return 0;
//        }

    }
}

        #endregion