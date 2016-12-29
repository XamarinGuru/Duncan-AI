using System;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Duncan.AI.Droid.Utils.HelperManagers;
using Java.Lang;
using XMLConfig;

namespace Duncan.AI.Droid.Utils.EditControlManagement.Controls
{
    public class CustomEditText : EditText
    {
        #region Constructors

        public CustomEditText(Context context)
            : base(context)
        {
            Ctx = context;
            // Init
            SetText("", BufferType.Normal);
            Visible = true;

            // initialize our typeface 
            Typeface loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnEditTextViewTypeface);
            if (loCustomTypeface != null)
            {
                this.Typeface = loCustomTypeface;
            }

            SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnEditTextViewTypefaceSizeSp);

           
        }


     

        #endregion

        #region Members
        protected EditControlBehavior _behavior = null;

        #endregion

        #region Properties
        public Context Ctx { get; set; }
        public EditControlBehavior BehaviorAndroid
        {
            get { return _behavior; }
            set { _behavior = value; }
        }

        public EditEnumerations.EditFieldType FieldType
        {
            get { return _behavior.GetFieldType(); }
            set { _behavior.SetFieldType(value); }
        }

        public bool Visible { get; set; }

        
            public new string Text
        {
            get { return base.Text; }
            set { SetText(value); }
        }

        public int MaxLength
        {
            get { return _behavior.MaxLength; }
            set
            {
                _behavior.MaxLength = value;
            }
        }
        public bool EventsAttached { get; set; }
        public string CustomId { get; set; }
        public string FormStatus { get; set; }
        public bool HasBeenFocused { get; set; }

        public LinearLayout ParentLayout;
        public Panel ParentPanel;
        #endregion

        #region Methods
        public void HookupEvents(LinearLayout layout, Panel panel)
        {
            ParentLayout = layout;
            ParentPanel = panel;

            if (!EventsAttached)
            {
                EventsAttached = true;
             
                this.BeforeTextChanged += HandleBeforeTextChanged;
                this.TextChanged += HandleTextChanged;
                this.AfterTextChanged += HandleAfterTextChanged;
                this.FocusChange += HandleFocusChange;

                //if this is a issue date column, fire off the thread that keeps the time in sync.
                if (BehaviorAndroid.PanelField.Name.Equals(Constants.ISSUETIME_COLUMN))
                {
                    var myRunnableThread = new CustomTimer(layout, BehaviorAndroid.PanelField.EditMask, this);
                    myRunnableThread.Run();
                }
            }
           
        }

        public void DetachEvents()
        {
            if (EventsAttached)
            {
                //   this.KeyPress +=HandleKeyPress;
                this.BeforeTextChanged -= HandleBeforeTextChanged;
                this.TextChanged -= HandleTextChanged;
                this.AfterTextChanged -= HandleAfterTextChanged;
                this.FocusChange -= HandleFocusChange;
                EventsAttached = false;
            }
        }

        public void ProcessRestrictions(int iNotifyEvent)
        {
            _behavior.ProcessRestrictionsFormInit(iNotifyEvent);
        }

        void HandleFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (sender is CustomEditText == false)
                return;
            var loEditCtrl = ((CustomEditText)(sender));
            EditControlBehavior loBehavior = loEditCtrl.BehaviorAndroid;
         
            if (e.HasFocus == true)
            {
                // edit text from the end when focused
                loEditCtrl.SetSelection(loEditCtrl.Length());

                loEditCtrl.SetBackgroundResource(Resource.Drawable.EditTextFocused);

                loEditCtrl.ParentPanel.FocusedViewCurrent = sender;
                
                Helper.ShowKeyboard(loEditCtrl); // show the keyboard 

            }
            else
            {     // e.HasFocus == false

                // display text from start when not focused
                loEditCtrl.SetSelection(0, 0);

                if (loEditCtrl.Enabled == true)
                {
                    loEditCtrl.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);
                }
                else
                {
                    loEditCtrl.SetBackgroundResource(Resource.Drawable.EditTextDisabled);
                }


                // lets not hide every time, let the next field decide if it should be available or not Helper.HideKeyboard(loEditCtrl); // hide the keyboard 

                loEditCtrl.ParentPanel.FocusedViewPrevious = sender;
                if (loEditCtrl.ParentPanel.FocusedViewCurrent == sender)
                {
                    loEditCtrl.ParentPanel.FocusedViewCurrent = null;
                }
            }


            loBehavior.HandleFocusChange(e.HasFocus);
        }

        private void HandleAfterTextChanged(object sender, AfterTextChangedEventArgs e)
        {
            if (!IgnoreEvents)
            {
                // IMPORTANT! Make sure we use the "sender" object instead of "this"
                // because we might be called via the message loop inside a different behavior object!!!
                // We can't do anything if the sender is not a textbox!
                if (sender is CustomEditText == false)
                    return;
                var loEditCtrl = ((CustomEditText) (sender));
                EditControlBehavior loBehavior = loEditCtrl.BehaviorAndroid;

                // Exit if validation is currently disabled
                if (loBehavior.ValidationDisabled)
                    return;

                // Are we flagged to skip this validation attempt?
                if (loBehavior.SkipNextValidation)
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
                loBehavior.SetText(loEditCtrl.Text);
                loBehavior.HandleTextChange();
            }

        }

        public bool IgnoreEvents { get; set; }

        void HandleBeforeTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IgnoreEvents)
            {
                //set the edit buffer here so the text change event can fall back if needed.
                if (sender is CustomEditText == false)
                    return;
                var loEditCtrl = ((CustomEditText) (sender));
                EditControlBehavior loBehavior = loEditCtrl.BehaviorAndroid;
                loBehavior.EditBuffer = loEditCtrl.Text;
                //fire off the Before Edit restriction events
                loBehavior.HandleBeforeTextChange();
            }
        }

        public void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IgnoreEvents)
            {
                // need to keep the visual and abstract components in sync
                if (sender is CustomEditText == false)
                    return;
                var loEditCtrl = ((CustomEditText)(sender));
                EditControlBehavior loBehavior = loEditCtrl.BehaviorAndroid;
                loBehavior.EditBuffer = loEditCtrl.Text;
            }
            else
            {
                // debug breakpoint - we are ignoring the changed value
                if (sender is CustomEditText == false)
                {
                    return;
                }
                else
                {
                    return;
                }

            }

        }

        private void SetText(string newText)
        {
            if (string.IsNullOrEmpty(newText))
                newText = string.Empty;

            // Don't bother if the text isn't changing
            //if (this.Text == newText)
            //    return;

            // don't bother for same data, but make sure text and editbuffer both agree
            if (this.Text.Equals(newText) == true)
            {
                bool loUpdateNeeded = false;
                if (_behavior != null)
                {
                    // if we have a behavior, make sure its editbuffer is considered 
                    loUpdateNeeded = (_behavior.EditBuffer.Equals(newText) == false);
                }

                // we've checked what we can, are we leaving or staying?
                if (loUpdateNeeded == false)
                {
                    return;
                }
            }


            // Can only set base text if there is no behavior object
            if (_behavior == null)
            {
                base.SetText(newText, BufferType.Normal);
                SetSelection(Text == null ? 0 : Text.Length);
                return;
            }

            // Set the edit buffer with the passed text. 
            // so the final edit buffer may differ.
            _behavior.SetEditBuffer(newText, true);

            // Now set the real text to match the edit buffer
            _behavior.SetText(_behavior.EditBuffer, ReinoControls.WinControl_SetText_Options.sto_ReplaceText, 0, true);
            _behavior.PanelField.Value = _behavior.EditBuffer;


            bool loSavedIgnoreEventsState = IgnoreEvents;

            try
            {

                IgnoreEvents = true;

                base.SetText(_behavior.EditBuffer, BufferType.Normal);
                SetSelection(Text == null ? 0 : Text.Length);
            }
            catch (System.Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "FieldName: " + this._behavior.PanelField.Name, "CustomEditText-SetText");
                Console.WriteLine("Exception caught in process: {0} {1}", exp, this._behavior.PanelField.Name);
            }
            finally
            {
                IgnoreEvents = loSavedIgnoreEventsState;
            }

        }


        public void SpinnerEditTextViewDateSpin(bool iSpinToNextPosition)
        {
            if (_behavior == null)
            {
                return;
            }


            if (_behavior.GetFieldType() == EditEnumerations.EditFieldType.efDate)
            {
                string loCurrentValueAsString = _behavior.GetValue();
                string loEditMask = _behavior.GetEditMask();

                // empty?
                if (string.IsNullOrEmpty(loCurrentValueAsString) == true)
                {
                    // start with today;
                    DateTimeManager.OsDateToDateString(DateTime.Today, loEditMask, ref loCurrentValueAsString);
                }

                DateTime loCurrentValueAsDateTime = DateTime.Today;
                var loConversionResult = DateTimeManager.DateStringToOSDate(loEditMask, loCurrentValueAsString, ref loCurrentValueAsDateTime);

                if (loConversionResult >= 0)
                {
                    // select plus or minus delta
                    int loSpinDelta = ((iSpinToNextPosition == true) ? 1 : -1);

                    string loNewValueAsString = "";

                    // select the element to spin
                    switch (DateTimeManager.GetDateTypeSpinElementForEditMask(loEditMask))
                    {
                        case DateTimeManager.TDateTimeCommonSpinElement.seSpinYear:
                            {
                                DateTimeManager.OsDateToDateString(loCurrentValueAsDateTime.AddYears(loSpinDelta), loEditMask, ref loNewValueAsString);
                                break;
                            }

                        case DateTimeManager.TDateTimeCommonSpinElement.seSpinMonth:
                            {
                                DateTimeManager.OsDateToDateString(loCurrentValueAsDateTime.AddMonths(loSpinDelta), loEditMask, ref loNewValueAsString);
                                break;
                            }

                        default:  // including DAY
                            {
                                DateTimeManager.OsDateToDateString(loCurrentValueAsDateTime.AddDays(loSpinDelta), loEditMask, ref loNewValueAsString);
                                break;
                            }

                    }



                    // set the new date
                    this.SetText(loNewValueAsString, TextView.BufferType.Normal);
                    this.BehaviorAndroid.SetEditBuffer(loNewValueAsString);
                }


            }
        }
        #endregion

        public void AssociateControl(EditControlBehavior behavior)
        {
            _behavior = behavior;
            _behavior.AssociateWithCustomEditText(this);
        }
    }
}