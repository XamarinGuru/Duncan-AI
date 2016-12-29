using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Android.Graphics;
using Android.Renderscripts;
using Android.Views;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Reino.ClientConfig;
using ReinoControls;
using XMLConfig;
using EditRestrictionConsts = Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts;

namespace Duncan.AI.Droid.Utils.EditControlManagement
{
    public class EditControlBehavior : IDisposable
    {
        #region Private Members
        #region General
        private int _CursorCharPos;
        private int _DependentNotificationDepth;
        private EditEnumerations.EditFieldType _FieldType;
        private int _MaxLength = 80; // Default max length to match legacy code
        private int _ProcessRestrictionsDepth;
        private string _Text = "";
        private int _TextSelectionEnd;
        private int _TextSelectionStart;
        #endregion

        #region Controls
        internal CustomTTEditControl _CfgCtrl = null;
        #endregion
        #region Edit Masks / Buffer
        private string _EditBuffer = "";
        private string _EditMask = "";
        private int _EditMaskLen;
        private int _EditStateAttrs;
        private bool _ExecutingSetEditBufferInternal;
        private string _SavedEditBuffer = "";
        #endregion
        #endregion

        #region Public Properties / Members
        public string CustomId = "";
        public string StructName { get; set; }
        public View PromptLabel = null;

        #region Properties (Visual State)

        public static View InvalidatedWindowAfterEditRestrictions = null;
        public Font FocusedFont = null;
        public Font NormalFont = null;

        #endregion

        #region Properties (Edit State)

        [XmlIgnore] // We don't want the following public property/member serialized in XML
        public bool SkipNextValidation = false;

        [XmlIgnore] // We don't want the following public property/member serialized in XML
        public bool ValidationDisabled = false;

        #endregion

        #region Properties (Associations)

        public List<EditControlBehavior> BehaviorCollection = null;

        public List<EditControlBehavior> Dependents = new List<EditControlBehavior>();
        public List<EditCondition> EditConditions = new List<EditCondition>();
        public List<EditRestriction> EditRestrictions = new List<EditRestriction>();

        /// <summary>
        ///     Association (if any) to an edit control in client configuration
        /// </summary>
        public CustomTTEditControl CfgCtrl
        {
            get { return _CfgCtrl; }
            set { _CfgCtrl = value; }
        }

        private View _editCtrl;

        /// <summary>
        ///     Edit control associated with this Behavior object.
        /// </summary>
        public View EditCtrl
        {
            get { return _editCtrl; }
            set { _editCtrl = value; }
        }

        public EditEnumerations.CustomEditControlType ControlType { get; set; }


        #endregion


        #region Properties (Data / Text)

        public string EditBuffer
        {
            get { return _EditBuffer; }
            set { _EditBuffer = value; }
        }

        public int MaxLength
        {
            get { return _MaxLength; }
            set
            {
                if (value <= 0) return; // can't do that!
                _MaxLength = value;
            }
        }

        public int ListItemIndex { get; set; }
        //public string ListItemText { get; set; }
        public string ListItemValue { get; set; }
        public string ListTableName { get; set; }
        public string[] ListTableColumn { get; set; }
        private HashSet<ListFilter> _filters;

        public HashSet<ListFilter> Filters {
            get { return _filters ?? (_filters = new HashSet<ListFilter>()); }
            set { _filters = value; }
        }



        public PanelField PanelField { get; set; }

        #endregion

        #endregion

        #region Delegates

        public delegate void CtrlGotFocus(object sender);

        public delegate void CustomizeValidationErrorText(ref string oErrMsg, EditControlBehavior behavior);

        public delegate CustomTTControl FindNextFormControl(TTControl AfterCtrl, bool MustBeEnabled);

        public delegate View GetFocusedControl();

        public delegate int GetFormEditAttrs();

        public delegate int GetFormEditMode();

        public delegate void RegularKeyPress(View.KeyEventArgs e);

        public delegate void SetFormEditAttr(int iAttribute, bool iSetAttr);

        public delegate bool TabBackward();

        public delegate bool TabForward();

        public delegate View WhichControlIsFirst(View Ctrl1, View Ctrl2);

        #endregion

        #region Events

        public event TabForward OnTabForward = null;
        public event TabBackward OnTabBackward = null;
        public event FindNextFormControl OnFindNextFormControl = null;
        public event WhichControlIsFirst OnWhichControlIsFirst = null;
        public event GetFocusedControl OnGetFocusedControl = null;
        public event GetFormEditMode OnGetFormEditMode = null;
        public event GetFormEditAttrs OnGetFormEditAttrs = null;
        public event SetFormEditAttr OnSetFormEditAttr = null;
        public event RegularKeyPress OnRegularKeyPress = null;
        public event CtrlGotFocus OnCtrlGotFocus = null;
        public event EventHandler TextChanged = null;
        public event EventHandler NotifiedDependentsParentDataChanged = null;
        public event CustomizeValidationErrorText OnCustomizeValidationErrorText = null;



        public Boolean ListContentsChangedByRestriction = false;

        #endregion

        #region Constructors

        public EditControlBehavior(CustomEditText Ctrl)
        {
            ListItemIndex = -1;
            AssociateWithCustomEditText(Ctrl);
        }

        public EditControlBehavior()
        {
            ListItemIndex = -1;
        }


        /// <summary>
        ///     Disposes of the object by dettaching the event handlers from their corresponding virtual
        ///     methods of the Behavior class and setting the edit control to null.
        /// </summary>
        public virtual void Dispose()
        {
            _editCtrl = null;
        }

         public void AssociateWithCustomEditText(CustomEditText ctrl)
        {
            // Somebody's doing something wrong if no control was passed
            if (ctrl == null)
                throw new ArgumentNullException("ctrl");

            // Set the associated edit control
            _editCtrl = ctrl;
        }

        public void AssociateWithCustomAutoTextView(CustomAutoTextView ctrl)
        {
            // Somebody's doing something wrong if no control was passed
            if (ctrl == null)
                throw new ArgumentNullException("ctrl");

            // Set the associated edit control
            _editCtrl = ctrl;
        }

        public void AssociateWithCustomSpinner(CustomSpinner ctrl)
        {
            // Somebody's doing something wrong if no control was passed
            if (ctrl == null)
                throw new ArgumentNullException("ctrl");
            // Set the associated edit control
            _editCtrl = ctrl;
        }

        public void AssociateWithCustomSignatureImageView(CustomSignatureImageView ctrl)
        {
            // Somebody's doing something wrong if no control was passed
            if (ctrl == null)
                throw new ArgumentNullException("ctrl");

            // Set the associated edit control
            _editCtrl = ctrl;
        }


        #endregion


        #region Keyboard Functions

        internal int HandleTextChange()
        {
            // save a copy of the field 's edit buffer and text buffer
            string loSavedEditBuf = _EditBuffer;
            string loSavedText = _Text;

            // the only way fListNdx would have changed is if the user scrolled the list, 
            // in which case we don't need to anticipate

            if ((ProcessRestrictions(EditRestrictionConsts.dneDataChanged, null)) != null)
            {
                // whoops! Don't allow this. Restore to the original value. 
                _EditBuffer = loSavedEditBuf;
                // make sure the window text matches the edit buffer
                SetText(_EditBuffer, WinControl_SetText_Options.sto_ReplaceText, 0, false);
                // Trigger the TextChanged event if the text value changed
                if (loSavedText != _Text)
                    OnTextChanged(EventArgs.Empty);
                return 0;
            }

            NotifyDependents(EditRestrictionConsts.dneParentDataChanged);
            //might have to take into consideration resursively changed values.
            _EditBuffer = loSavedText;
            SetText(_EditBuffer, WinControl_SetText_Options.sto_ReplaceText, 0, false);
            PaintEditCtrl(_EditBuffer);
            return 0;
        }


        internal int HandleAncestorFieldExit()
        {
            NotifyDependents(EditRestrictionConsts.dneAncestorFieldExit);
            //might have to take into consideration resursively changed values.
            SetText(_EditBuffer, WinControl_SetText_Options.sto_ReplaceText, 0, false);
            PaintEditCtrl(_EditBuffer);
            return 0;
        }


        internal int HandleBeforeTextChange()
        {
            //otherwise they jsut left focus, so fire that event off
            // save a copy of the field 's edit buffer and text buffer
            string loSavedEditBuf = _EditBuffer;
            // the only way fListNdx would have changed is if the user scrolled the list, 
            // in which case we don't need to anticipate
            if ((ProcessRestrictions(EditRestrictionConsts.dneBeforeEdit, null)) != null)
            {
                // whoops! Don't allow this. Restore to the original value. 
                _EditBuffer = loSavedEditBuf;
                // make sure the window text matches the edit buffer
                SetText(_EditBuffer, WinControl_SetText_Options.sto_ReplaceText, 0, false);
                // Trigger the TextChanged event if the text value changed
            }
            return 0;
        }

        internal int HandleFocusChange(bool hasfocus)
        {
            //otherwise they jsut left focus, so fire that event off
            // save a copy of the field 's edit buffer and text buffer
            string loSavedEditBuf = _EditBuffer;
            string loSavedText = _Text;

            // the only way fListNdx would have changed is if the user scrolled the list, 
            // in which case we don't need to anticipate
            if (hasfocus)
            {
                //// Skip first edit focus notification if we have already edited the 1st field
                //if this is the first time this field is being focused
                if (!HasBeenFocused())
                {
                    if ((ProcessRestrictions(EditRestrictionConsts.dneFirstEditFocus, null)) != null)
                    {
                        // whoops! Don't allow this. Restore to the original value. 
                        _EditBuffer = loSavedEditBuf;
                        // make sure the window text matches the edit buffer
                        SetText(_EditBuffer, WinControl_SetText_Options.sto_ReplaceText, 0, false);
                        // Trigger the TextChanged event if the text value changed
                    }
                    else
                        //flag this behavoir as being focused. - only if validation passed?
                        SetFocusedState(true);
                    if (loSavedText != _Text)
                        OnTextChanged(EventArgs.Empty);
                    return 0;
                }
                else
                {
                    //if we got here, this control has already been focused, so just fire off the before edit event
                    if ((ProcessRestrictions(EditRestrictionConsts.dneBeforeEdit, null)) != null)
                    {
                        // whoops! Don't allow this. Restore to the original value. 
                        _EditBuffer = loSavedEditBuf;
                        // make sure the window text matches the edit buffer
                        SetText(_EditBuffer, WinControl_SetText_Options.sto_ReplaceText, 0, false);
                        // Trigger the TextChanged event if the text value changed
                    }

                    //now we have to let those dependents know that 
                    if (loSavedText != _Text)
                        OnTextChanged(EventArgs.Empty);
                    return 0;
                }
            }

            // dont need this anymore because it is called from OkToExit
            //if this control does not have focus, just notify the dependents that the parent field just lost focus
            //NotifyDependents(EditRestrictionConsts.dneParentFieldExit);
            return 0;
        }

        #endregion

        #region EditState Attribute Functions

        public void ClearEditStateAttrs()
        {
            _EditStateAttrs = 0;
        }

        public bool GetEditStatePreInitialized()
        {
            return (_EditStateAttrs & EditRestrictionConsts.esaPreInitialized) != 0;
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



        #region FieldType / DataType

        public EditEnumerations.EditFieldType GetFieldType()
        {
            return _FieldType;
        }

        public void SetFieldType(EditEnumerations.EditFieldType iFieldType)
        {
            _FieldType = iFieldType;
        }

        #endregion

        #region Cursor Position Functions



        #endregion

        #region Selected Text Functions

        public int GetTextSelectedLength()
        {
            if (_TextSelectionEnd == 0) // don't return false values
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



        ///// <summary>
        /////     initialize the start and end of the selected text
        ///// </summary>
        //public void SetTextSelection(int pStart, int pStop)
        //{
        //    // mark offsets Start..Stop as selected
        //    if (pStart <= pStop)
        //    {
        //        int loLen = _Text.Length;
        //        if (pStart > loLen)
        //            pStart = loLen;
        //        if (pStop > loLen)
        //            pStop = loLen;
        //        _TextSelectionStart = pStart;
        //        _TextSelectionEnd = pStop;
        //        if (_editCtrl is CustomEditText)
        //            ((CustomEditText) (EditCtrl)).MaxLength = _TextSelectionEnd - _TextSelectionStart;
        //    }
        //}

        #endregion

        #region Edit Buffer Functions

        private void ReplaceEditBuffCharAtIndex(int index, char character)
        {
            // create string builder object based on existing edit buffer
            var buffer = new StringBuilder(_EditBuffer);
            // if valid index, remove existing character
            if (buffer.Length >= index + 1)
                buffer.Remove(index, 1);
            // insert new character unless its a null
            if (character != (char) 0)
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
            var buffer = new StringBuilder(_EditBuffer);
            // insert new character unless its a null
            if (character != (char) 0)
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
        ///     Replaces the text in the edit buffer with iFieldText then repaints itself
        /// </summary>
        private int SetEditBufferInternal(string iFieldText, bool iNotifyDependents)
        {
            // Can't do anything if there is no max length
            if (_MaxLength <= 0) return 0;

            //make sure we dont have a null string (for invisible fields)
            if (string.IsNullOrEmpty(iFieldText))
                iFieldText = string.Empty;

            // save a copy for later comparisons if we are going to notify of any changes
            if (iNotifyDependents)
                _SavedEditBuffer = _EditBuffer;

            // Set buffer contents (truncated at max length)
            _EditBuffer = iFieldText;


            // Android - lets see the full items + descriptions for all fields on entry form
            ////for now we are ignoring max length for spinners, since max length and spinnres dont mesh well.
            //if (ControlType != EditEnumerations.CustomEditControlType.Spinner)
            //{
            //    if (_EditBuffer.Length > _MaxLength)
            //    {
            //        _EditBuffer = _EditBuffer.Remove(_MaxLength, _EditBuffer.Length - _MaxLength);
            //    }
            //}

            // We're finished unless we need to notify dependents about our changes
            if (!iNotifyDependents)
                return 0;

            // Exit if nothing changed
            if (_EditBuffer.Equals(_SavedEditBuffer))
                return 0;

            // We need to use this flag to avoid infinite recursion caused by ProcessRestrictions
            if (_ExecutingSetEditBufferInternal)
                return 0;

            // Now set flag to avoid infinite recursion caused by ProcessRestrictions
            _ExecutingSetEditBufferInternal = true;

            if (ProcessRestrictions(EditRestrictionConsts.dneDataChanged, null) != null)
            {
                // something failed, restore the data
                _EditBuffer = _SavedEditBuffer;
                _ExecutingSetEditBufferInternal = false;
                return 0;
            }

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
        ///     Replaces the text in the edit buffer with iFieldText, making sure it is valid for the
        ///     field mask. TEditField will also find the associated list item if one exists.
        /// </summary>
        public int SetEditBuffer(string iFieldText)
        {
            return SetEditBuffer(iFieldText, false);
        }

        public int SetEditBuffer(string iFieldText, bool iNotifyDependents)
        {
            SetEditBufferInternal(iFieldText, iNotifyDependents);
            //SetTextSelection(0, 0);
            return 0;
        }

        public void SetEditBufferAndPaint(string newText)
        {
            SetEditBufferAndPaint(newText, true);
        }

        public void SetEditBufferAndPaint(string newText, bool raiseChangeEvent)
        {
            // Set the edit buffer with the passed text. 
            _EditBuffer = newText;
            SetText(_EditBuffer, WinControl_SetText_Options.sto_ReplaceText, 0, raiseChangeEvent);
            // Finally, update the associated control's text
            PaintEditCtrl(_EditBuffer);
        }


        /// <summary>
        ///     Returns true if a field is blank (i.e. only contains mask literals and spaces), false if not.
        /// </summary>
        public bool FieldIsBlank()
        {
            // If the edit buffer is completely empty, then of course the field is blank
            //also check to see if it is the spinner default. 
            //if (string.IsNullOrEmpty(_EditBuffer) || _EditBuffer == Constants.SPINNER_DEFAULT)
            //    return true;
            if (string.IsNullOrEmpty(_EditBuffer))
            {


                // AJW - TODO - this is looking ahead to the View's buffer that hasn't been yet added to our copy.... this is not ideal!
                if (this._editCtrl != null)
                {
                    if (this._editCtrl is CustomEditText)
                    {
                        if ( string.IsNullOrEmpty(((CustomEditText)this._editCtrl).Text) == false)
                        {
                            return false;
                        }
                    }

                    if (this._editCtrl is CustomAutoTextView)
                    {
                        if (string.IsNullOrEmpty(((CustomAutoTextView)this._editCtrl).Text) == false)
                        {
                            return false;
                        }
                    }
                }



                return true;
            }



            // Is it a numeric field which is treated differently?
            if (_FieldType == EditEnumerations.EditFieldType.efNumeric)
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
        public string GetValue()
        {
            if (ControlType == EditEnumerations.CustomEditControlType.Spinner)
            {
                var crt = ((CustomSpinner) (_editCtrl));
               var value =  (crt).GetValue(crt.OptionsList);
                return value;
            }
           
            return GetText();
        }
        public void SetText(string value)
        {
            //ListItemText = value;
            _Text = value;
        }

        // Raises the TextChanged event.  
        protected virtual void OnTextChanged(EventArgs e)
        {
            if (TextChanged != null)
                TextChanged(this, e);
        }

        public void RaiseTextChangedEvent()
        {
            OnTextChanged(new EventArgs());
        }

        /// <summary>
        ///     Routine to update the displayed text of a WinControl.  If the existing TextBuf
        ///     is large enough, the new text is placed in it.  Otherwise, a new TextBuf is
        ///     allocated, and the old one is FREEd.
        ///     Expects:
        ///     - iText : New text to add.
        ///     - iOptions: one of...
        ///     - sto_InsertTextAtPos: iText is inserted into the string at iPos.
        ///     - sto_ReplaceTextAtPos: iText is inserted at iPos, and overwrites any existing text at
        ///     that position.
        ///     - sto_InsertTextAfterPos: iText is inserted into the string after iPos.
        ///     - sto_ReplaceTextAfterPos: iText is inserted after iPos, and overwrites any existing text
        ///     that position.
        ///     - sto_ReplaceText: iText replaces existing text in its entirety. iPos is not used.
        ///     - iPos: Position to place text into fTextBuf for all options except sto_ReplaceText
        /// </summary>
        public int SetText(string iText, WinControl_SetText_Options iOptions, int iPos, bool RaiseChangedEvent)
        {
         
            if (string.IsNullOrEmpty(iText))
                iText = string.Empty;

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
                    PanelField.Value = iText;
                }
                else
                {
                    // place the new string in the TextBuf 
                    var buffer = new StringBuilder(_Text);
                    buffer.Insert(iPos, iText);
                    _Text = buffer.ToString();
                    PanelField.Value = iText;
                }
            }

            // if the current cursor char position is beyond the end, set it to end. 
            if (_CursorCharPos > loNewTextLen)
                _CursorCharPos = loNewTextLen;

            // Trigger the TextChanged event if the text value changed and we're flagged to raise the event
            if (RaiseChangedEvent && (PrevText != _Text))
                OnTextChanged(EventArgs.Empty);

            // all done
            return 0;
        }

        #endregion

        #region Edit Restrictions / Dependant Notifications

        public void ProcessRestrictionsFormInit(int iNotifyEvent)
        {
            if (ProcessRestrictions(iNotifyEvent, null) != null)
            {
                // something failed, restore the data
                _EditBuffer = _SavedEditBuffer;
                _ExecutingSetEditBufferInternal = false;
            }
        }

        public EditRestriction ProcessRestrictions(int iNotifyEvent, EditControlBehavior iParentBehavior)
        {
            // defend against circular references. Don't allow a nested invocation for the same object instance.
            if (_ProcessRestrictionsDepth > 0)
                return null;
            _ProcessRestrictionsDepth++;

            int loNdx;
            EditRestriction loFailedRestrict = null;

            // Use the OnGetFormEditMode event if we have one, 
            // otherwise just assume the default "femNewEntry" mode
            //int loFormEditMode = OnGetFormEditMode != null ? OnGetFormEditMode() : EditRestrictionConsts.dneFormInit;
            int loFormEditMode = OnGetFormEditMode != null ? OnGetFormEditMode() : EditRestrictionConsts.femNewEntry;

            // Loop through each edit restriction
            int loLoopMax = EditRestrictions.Count;
            for (loNdx = 0; loNdx < loLoopMax; loNdx++)
            {
                EditRestriction loRestrict = EditRestrictions[loNdx];
                if (loRestrict.EnforceRestriction(iNotifyEvent, loFormEditMode, ref iParentBehavior) == true)
                {
                    _ProcessRestrictionsDepth--;
                    // If this is the first failed one, retain it for a return value
                    if (loFailedRestrict == null)
                        loFailedRestrict = loRestrict; // This one failed
                }
            }
            _ProcessRestrictionsDepth--;

            // If one failed, return it
            return loFailedRestrict;
        }

        public void NotifyDependents(int iNotifyEvent)
        {
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
        }

        public void DependentNotification(int iNotifyEvent, EditControlBehavior iParentBehavior)
        {
            // defend against circular references. Don't allow a nested invocation for the same object instance.
            if (_DependentNotificationDepth > 0)
                return;
            _DependentNotificationDepth++;
            if (string.IsNullOrEmpty(_EditBuffer))
                _EditBuffer = string.Empty;
            // retain current field value
            string loSavedData = _EditBuffer;

            // process our edit restrictions
            ProcessRestrictions(iNotifyEvent, iParentBehavior);

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

        public delegate bool OnQueryUserEvent(string iLine1, string iLine2);

        public delegate void OnStandardMessageBoxEvent(string iText, string iCaption);

        public static bool SkipRebuildDisplay = false;
        public static bool RebuildDisplayWasSkipped = false;

        public static OnQueryUserEvent OnQueryUser = null;

        public static OnStandardMessageBoxEvent OnStandardMessageBox = null;
        public bool SkipNextOkToExit = false;

        public string LastValidationErrorMessage = string.Empty;

        public string GetEditMask()
        {
            return _EditMask;
        }

        public void SetEditMask(string iEditMask)
        {
            _EditMask = iEditMask;
            _EditMaskLen = iEditMask.Length;
            _MaxLength = _EditMaskLen;
        }

        public int ValidateSelf(ref string oErrMsg)
        {
            int loResult = 0;
            EditRestriction loRestrict;
            oErrMsg = "";

            // Process our restrictions and see if we get one that failed
            if ((loRestrict = ProcessRestrictions(EditRestrictionConsts.dneValidate, null)) != null)
            {
                // loRestrict points to restriction that failed.
                // Lets beautify the name by replacing underscores with spaces
                string loRestrictionDisplayName = loRestrict.Name.Replace("_", " ");

                // AJW TODO - we need a lookup/translation from restrictions names -> user friendly text
                if (loRestrictionDisplayName.Equals("REQUIRED") == true)
                {
                    oErrMsg = loRestrictionDisplayName;  // adding FAILED! is confusing to the user
                }
                else
                {
                    oErrMsg = loRestrictionDisplayName + " FAILED!";
                }

                // Can the failure be overridden?
                if (loRestrict.Overrideable)
                {
                    //if (QueryUser(oErrMsg, "Do you wish to correct this?"))
                    //{
                    //    loRestrict.SetDisabledIfNoChange(false);
                    //    loResult = -1;
                    //}
                    // the user has chosen to ignore this error. 
                    // Make sure this check isn't enforced until the data changes 
                    loRestrict.SetDisabledIfNoChange(true);
                }
                else
                {
                    // RingBell(this);
                    loResult = -1;
                }
            }

            if (loResult != 0)
                return loResult;
            return 0;
        }

        public event RestrictionForcesDisplayRebuild OnRestrictionForcesDisplayRebuild = null;

        public bool OkToExit(bool iExitForward)
        {
            // If flagged to skip this execution, reset flag, then exit
            if (SkipNextOkToExit)
            {
                SkipNextOkToExit = false;
                return true;
            }
            // Reset SkipNextOkToExit for next time
            SkipNextOkToExit = false;


            LastValidationErrorMessage = string.Empty;
            if ((!iExitForward) || (ValidateSelf(ref LastValidationErrorMessage) == 0))
            //string loValidateErrMsg = "";
            //if ((!iExitForward) || (ValidateSelf(ref loValidateErrMsg) == 0))

            {
                // We're valid, so close any previous popup balloon messages
                // ClosePopupBalloons();
                // Let our children do stuff they care about. Edit Restictions might want to
                // rebuild the display, but let's make it only happen once
                if (OnRestrictionForcesDisplayRebuild != null)
                {
                    SkipRebuildDisplay = true;
                    RebuildDisplayWasSkipped = false;
                }
                else
                {
                    SkipRebuildDisplay = false;
                    RebuildDisplayWasSkipped = false;
                }

                // Notify dependents that our focus has left
                NotifyDependents(EditRestrictionConsts.dneParentFieldExit);

                if (RebuildDisplayWasSkipped && (OnRestrictionForcesDisplayRebuild != null))
                {
                    // Turn off flags, then force rebuild
                    SkipRebuildDisplay = false;
                    RebuildDisplayWasSkipped = false;
                    OnRestrictionForcesDisplayRebuild();
                }
                else
                {
                    // Turn off flags for Rebuild procedure
                    SkipRebuildDisplay = false;
                    RebuildDisplayWasSkipped = false;
                }
                return true;
            }
            return false; // swallow the key so that we don't leave it.
        }

        #endregion

        #region General / Helper Functions

        //public static int GetFieldIndexForTable( TTTable pListTable, string pListField)
        //{
        //    // Find field index
        //    var predicate = new TObjBasePredicate(pListField);
        //    return pListTable.fTableDef.HighTableRevision.Fields.FindIndex(predicate.CompareByName_CaseInsensitive);
        //}

        //public static string GetFieldDataFromTable(TTTable pListTable, int pListFieldIdx, int pRowIdx, string DestMask)
        //{
        //    // Just return empty string if table, fieldname or row index is bad
        //    if (pListTable == null) return "";

        //    if (pListFieldIdx == -1) return "";
        //    if ((pRowIdx < 0) || (pRowIdx >= pListTable.GetRecCount())) return "";

        //    // Get field value from the table. First we have to read the desired record
        //    pListTable.ReadRecord(pRowIdx);

        //    string FieldData = null;
        //    if (DestMask == null)
        //        FieldData = pListTable.GetFormattedFieldData(pListFieldIdx, pListTable.fTableDef.GetField(pListFieldIdx).MaskForHH);
        //    else
        //        FieldData = pListTable.GetFormattedFieldData(pListFieldIdx, DestMask);

        //    return FieldData;
        //}


        public EditControlBehavior GetEditControlBehaviorByName(string controlName)
        {
            // Loop through all behaviors in our collection,
            // and return the matching EditControlBehavior when found.
            int loLoopMax = BehaviorCollection.Count;
            for (int loIdx = 0; loIdx < loLoopMax; loIdx++)
            {
                if (BehaviorCollection[loIdx].CfgCtrl.Name.Equals(controlName) && BehaviorCollection[loIdx].StructName == StructName)
                    return BehaviorCollection[loIdx];
            }
            // Couldn't find a EditControlBehavior with the passed name
            return null;
        }

        private void PaintEditCtrl(string text)
        {
            // AJW TODO - review and update


            //only valid for certain controls. Check the control type and go
            if (ControlType == EditEnumerations.CustomEditControlType.EditText)
                ((CustomEditText) (_editCtrl)).Text = text;
            if (ControlType == EditEnumerations.CustomEditControlType.AutoCompleteText)
                ((CustomAutoTextView) (_editCtrl)).Text = text;
            if (ControlType == EditEnumerations.CustomEditControlType.Spinner)
                ((CustomSpinner) (_editCtrl)).Text = text;
        }

        private bool HasBeenFocused()
        {
            if (ControlType == EditEnumerations.CustomEditControlType.EditText)
                return ((CustomEditText) (_editCtrl)).HasBeenFocused;
            if (ControlType == EditEnumerations.CustomEditControlType.AutoCompleteText)
                return ((CustomAutoTextView) (_editCtrl)).HasBeenFocused;
            if (ControlType == EditEnumerations.CustomEditControlType.Spinner)
                return ((CustomSpinner) (_editCtrl)).HasBeenFocused;
            return false;
        }

        private void SetFocusedState(bool focused)
        {
            if (ControlType == EditEnumerations.CustomEditControlType.EditText)
                ((CustomEditText) (_editCtrl)).HasBeenFocused = focused;
            if (ControlType == EditEnumerations.CustomEditControlType.AutoCompleteText)
                ((CustomAutoTextView) (_editCtrl)).HasBeenFocused = focused;
            if (ControlType == EditEnumerations.CustomEditControlType.Spinner)
                ((CustomSpinner) (_editCtrl)).HasBeenFocused = focused;
        }

        #endregion

        #region List Items

        public int SetListIndexAndPaint(int iListNdx)
        {
            if (iListNdx < 0)
                iListNdx = 0;
            //try to get the spinner for this edit control
            if (this.ControlType == EditEnumerations.CustomEditControlType.Spinner)
                //Position + 1 Thats because spinner defualt  "select" at 0 index
                ((CustomSpinner) (_editCtrl)).SetListIndex(iListNdx + 1);
            if (this.ControlType == EditEnumerations.CustomEditControlType.AutoCompleteText)
                ((CustomAutoTextView)(_editCtrl)).SetListIndex(iListNdx);
            return 0;
        }

        public int SetListDisplayValueAndPaint(string value)
        {
            //if (string.IsNullOrEmpty(value))
            //    value = Constants.SPINNER_DEFAULT;

            //try to get the spinner for this edit control
            if (this.ControlType == EditEnumerations.CustomEditControlType.Spinner)
                ((CustomSpinner)(_editCtrl)).Text = value;
            return 0;
        }

        /// <summary>
        /// Sets the list to the saved db value instead of the displayed value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int SetListDBValueAndPaint(string value)
        {
            //if (string.IsNullOrEmpty(value))
            //    value = Constants.SPINNER_DEFAULT;
            if (string.IsNullOrEmpty(value))
            {
                value = "";
            }


            //try to get the spinner for this edit control
            if (this.ControlType == EditEnumerations.CustomEditControlType.Spinner)
                ((CustomSpinner)(_editCtrl)).SetListItemDataByValue(value);
            return 0;
        }

        #endregion

        public void InitToDefaultStateAndroid(EditControlBehavior nextBehavior)
        {


            // we must re-build the list each time we init so the old type-ahead filters will be reset
            bool loListRefreshNeeded = true;

            //// before we re-set the filters, did we have any previous?
            //bool loListRefreshNeeded = false;

            //if (nextBehavior.Filters != null)
            //{
            //    if (nextBehavior.Filters.Count > 1)
            //    {
            //        // only for this type which we will cast again below
            //        if (nextBehavior.PanelField.uiComponent is CustomAutoTextView)
            //        {
            //            loListRefreshNeeded = true;
            //        }
            //    }
            //}



            // reset the filters
            nextBehavior.Filters = new HashSet<ListFilter>();

            // need to redo the list?
            if (loListRefreshNeeded == true)
            {
                //List<string> loRefreshedList = nextBehavior.GetFilteredListItems();
                //Helper.UpdateControlWithNewListPrim((nextBehavior.PanelField.uiComponent as CustomAutoTextView), loRefreshedList.ToArray(), EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.bfSetHasBeenFocusedFalse);

                if (nextBehavior.PanelField.uiComponent is CustomAutoTextView)
                {
                    if (string.IsNullOrEmpty(nextBehavior.PanelField.OptionsList.ListName) == false)
                    {
                        string[] loRefreshedList = (new ListSupport()).GetListDataByTableColumnName(nextBehavior.PanelField.OptionsList.ListName, Helper.ConcatColumns(nextBehavior.PanelField.OptionsList.Columns));
                        Helper.UpdateControlWithNewListPrim((nextBehavior.PanelField.uiComponent as CustomAutoTextView), loRefreshedList, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.bfSetHasBeenFocusedFalse);
                    }
                }
            }



            nextBehavior.InitToDefaultState();


            if (nextBehavior.ControlType == EditEnumerations.CustomEditControlType.EditText)
            {
                var item = (CustomEditText) nextBehavior.EditCtrl;
                if (item != null)
                {
                    item.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);
                    //item.SetBackgroundColor(Color.White);
                    item.SetTextColor(Color.Black);
                    item.FormStatus = null;
                    item.HasBeenFocused = false;
                }
            }

            if (nextBehavior.ControlType == EditEnumerations.CustomEditControlType.AutoCompleteText)
            {
                var item = (CustomAutoTextView) nextBehavior.EditCtrl;
                if (item != null)
                {
                    item.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);
                    //item.SetBackgroundColor(Color.White);
                    item.SetTextColor(Color.Black);
                    item.FormStatus = null;
                    item.HasBeenFocused = false;
                }
            }

            if (nextBehavior.ControlType == EditEnumerations.CustomEditControlType.Spinner)
            {
                var item = (CustomSpinner) nextBehavior.EditCtrl;
                if (item != null)
                {
                    item.SetBackgroundColor(Color.White);
                    item.FormStatus = null;
                    item.HasBeenFocused = false;
                }
            }
        }




        // AJW - for removal, this was never called
        //public void InvokeFormInitEventRestriction(View oneCustomEditViewControl)
        //{
            
        //    if (oneCustomEditViewControl is CustomEditText)
        //    {
        //        var item = (CustomEditText)oneCustomEditViewControl;
        //        if (item != null)
        //        {
        //            item.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
        //            item.FormStatus = "Processed";


        //        }
        //    }
        //    else if (oneCustomEditViewControl is CustomAutoTextView)
        //    {
        //        var item = (CustomAutoTextView)oneCustomEditViewControl;
        //        if (item != null)
        //        {
        //            item.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
        //            item.FormStatus = "Processed";
        //        }
        //    }
        //    else if (oneCustomEditViewControl is CustomSpinner)
        //    {
        //        var item = (CustomSpinner)oneCustomEditViewControl;
        //        if (item != null)
        //        {
        //            item.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
        //            item.FormStatus = "Processed";
        //        }
        //    }

        //}


        #region List Related Functions

        /// <summary>
        /// retrieves the value of a specific column based on the currently selected value of the behavior.
        /// </summary>
        /// <param name="columnToReturn"></param>
        /// <returns></returns>
        public   string  GetAssociatedRowData( string columnToReturn)
        {
            if (PanelField.OptionsList != null && !string.IsNullOrEmpty(PanelField.OptionsList.saveColumn))
            {
                //get the parent controls value 
                string parentValue = GetValue();

                // remove abbrev descriptions when present - these are often displayed on entry screen
                int loPosSep = parentValue.IndexOf(Constants.LIST_ITEM_DESCRIPTION_SEPARATOR);
                // has to be beyond the first char
                if (loPosSep > 0)
                {
                    // keep everything up to the space preceeding the seperator char
                    parentValue = parentValue.Substring(0, loPosSep - 1);
                }

                //also, set some parent control table name and column name stuff here
                var parentTableName = PanelField.OptionsList.ListName;
                var parentColumnName = PanelField.OptionsList.saveColumn;

                //update the list item text and value here if needed
                // go get the value from the DB
                var value = (new ListSupport()).GetDataFromTableWithColumnValue(columnToReturn, parentTableName, parentColumnName, parentValue);
                return   value;
            }
            return string.Empty;
        }


        #region List Filters
        public bool AddFilter(ListFilter filter)
        {
          var index =  RemoveFilter(filter.Column);
             Filters.Add(filter);
            return index > 0 ;
        }

        public int RemoveFilter(string columnName)
        {
            return Filters.RemoveWhere(x => x.Column == columnName);
        }



        private bool FiltersAreEqual( HashSet<ListFilter> iFilterA, HashSet<ListFilter> iFilterB)
        {
            if (( iFilterA == null ) && ( iFilterB == null ))
            {
                return true;
            }

            if ((iFilterA == null) && (iFilterB != null))
            {
                return false;
            }

            if ((iFilterA != null) && (iFilterB == null))
            {
                return false;
            }

            // through here we now have two non-null filters

            if ((iFilterA.Count == 0) && (iFilterB.Count == 0))
            {
                return true;
            }

            if (iFilterA.Count != iFilterB.Count)
            {
                return false;
            }

            // through here we have two filters with the same number of columns

            foreach (ListFilter oneFilterFromA in iFilterA)
            {
                ListFilter compareFilterFromB = null;
                foreach (ListFilter oneFilterFromB in iFilterB)
                {
                    if (oneFilterFromA.Column.Equals(oneFilterFromB.Column) == true)
                    {
                        compareFilterFromB = oneFilterFromB;
                        break;
                    }
                }

                if (compareFilterFromB == null)
                {
                    // filterB doesn't have the same columns
                    return false;
                }

                if (oneFilterFromA.Value.Equals(compareFilterFromB.Value) == false)
                {
                    // filterB has different value for same column
                    return false;
                }

                //int loIndexFromB = iFilterB.IndexOf(oneFilterFromA);  // TODO is this a valid comparison? does it need to be done by column name?
                //if (loIndexFromB == -1)
                //{
                //    // filterB doesn't have the same columns
                //    return false;
                //}

                //if (oneFilterFromA.Value.Equals(iFilterB[loIndexFromB].Value) == false)
                //{
                //    // filterB has different value for same column
                //    return false;
                //}
            }

            // here we have compared all columns and values - these filters are the same
            return true;

        }

        /// <summary>
        // this method will get a list of items for this control with respect to the filters applied to it.
        /// </summary>
        /// <returns></returns>
        private List<string> _FilteredListItems = null;
        private List<ListFilter> _FiltersUsedList = null;
        private HashSet<ListFilter> _FiltersUsedHashSet = null;
        private int _FiltersListItemsListReusedCount = 0;          // debug / scope check
        private int _FiltersListItemsListReusedCountMax = 0;


        public List<string> GetFilteredListItems()
        {
            //string[] response =  (new ListSupport()).GetFilteredListData(PanelField.OptionsList.ListName, Helper.ConcatColumns(PanelField.OptionsList.Columns), new List<ListFilter>(Filters));
            //return new List<String>(response);

            // AJW - lets speed things up by not pulling the same list over and over.  TODO ? save/reference last N versions by filters

            //// are the filters still the same ones used to initialize the list?
            if ((_FiltersUsedList == null) || (FiltersAreEqual(_FiltersUsedHashSet, Filters) == false))
            {
                // redo the list with new filters
                _FilteredListItems = null;
                _FiltersUsedList = new List<ListFilter>(Filters);
                _FiltersUsedHashSet = new HashSet<ListFilter>(Filters, Filters.Comparer);

                // debug breakpoint to determine how often filters are changing
                if (_FiltersListItemsListReusedCount > 0)
                {
                    _FiltersListItemsListReusedCount++;
                }

                _FiltersListItemsListReusedCount = 0;

            }
            else
            {
                _FiltersListItemsListReusedCount++;
                if (_FiltersListItemsListReusedCount > _FiltersListItemsListReusedCountMax)
                {
                    _FiltersListItemsListReusedCountMax = _FiltersListItemsListReusedCount;
                }
            }

            // do we have list for the current filters?
            if (_FilteredListItems == null)
            {
                // is a list defined?
                if ((String.IsNullOrEmpty(PanelField.OptionsList.ListName) == false) && ( PanelField.OptionsList.Columns.Length > 0 ))
                {
                    // go get it 
                    string[] response = (new ListSupport()).GetFilteredListData(PanelField.OptionsList.ListName, Helper.ConcatColumns(PanelField.OptionsList.Columns), _FiltersUsedList);
                    _FilteredListItems = new List<String>(response);
                }
                else
                {
                    // no list defined, return an emoty one
                    _FilteredListItems = new List<String>();
                }
            }

            return _FilteredListItems;
        }



        //this method will get a list of items for this control with respect to hte filters applied to it.
        private List<string> _FilteredListItemsBySaveColumn = null;
        private List<ListFilter> _FiltersBySaveColumn = null;
        private HashSet<ListFilter> _FiltersBySaveColumnHasSet = null;
        
        private int _FiltersListItemsByColumnListReusedCount = 0;       // debug / scope check
        private int _FiltersListItemsByColumnListReusedCountMax = 0;


        public List<string>  GetFilteredListItemsBySaveColumn()
        {
            //string[] response = (new ListSupport()).GetFilteredListData(PanelField.OptionsList.ListName,  PanelField.OptionsList.saveColumn  , new List<ListFilter>(Filters));
            //return new List<String>(response);


            // AJW - lets speed things up by not pulling the same list over and over.  TODO ? save/reference last N versions by filters

            //// are the filters still the same ones used to initialize the list?
            if (_FiltersBySaveColumn == null)
            {
                _FiltersBySaveColumn = new List<ListFilter>(Filters);
            }

            //// are the filters still the same ones used to initialize the list?
            if ((_FiltersBySaveColumn == null) || (FiltersAreEqual(_FiltersBySaveColumnHasSet, Filters) == false))
            {
                // redo the list with new filters
                _FilteredListItemsBySaveColumn = null;
                _FiltersBySaveColumn = new List<ListFilter>(Filters);
                _FiltersBySaveColumnHasSet = new HashSet<ListFilter>(Filters, Filters.Comparer);

                // debug breakpoint to determine how often filters are changing
                if (_FiltersListItemsByColumnListReusedCount > 0)
                {
                    _FiltersListItemsByColumnListReusedCount++;
                }

                _FiltersListItemsByColumnListReusedCount = 0;
            }
            else
            {
                _FiltersListItemsByColumnListReusedCount++;
                if (_FiltersListItemsByColumnListReusedCount > _FiltersListItemsByColumnListReusedCountMax)
                {
                    _FiltersListItemsByColumnListReusedCountMax = _FiltersListItemsByColumnListReusedCount;
                }
            }


            if (_FilteredListItemsBySaveColumn == null)
            {

                string[] response = (new ListSupport()).GetFilteredListData(PanelField.OptionsList.ListName, PanelField.OptionsList.saveColumn, _FiltersBySaveColumn);
                _FilteredListItemsBySaveColumn = new List<string>(response);
            }

            return _FilteredListItemsBySaveColumn;
        }

        public void  RefreshListItems()
        {
            //this only applies to spinners
            if (ControlType == EditEnumerations.CustomEditControlType.Spinner)
            {
                var spinner = ((CustomSpinner) (_editCtrl));
                spinner.SetDataSource(spinner.Ctx);
            }
        }

        #endregion
        #endregion
    }
}
