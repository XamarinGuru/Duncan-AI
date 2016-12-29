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
    public class CustomSignatureImageView : ImageView
    {
        #region Constructors

        public CustomSignatureImageView(Context context)
            : base(context)
        {
            Ctx = context;
            Visible = true;

            // initialize our typeface 
            Typeface loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnSignatureImageViewTypeface);
            if (loCustomTypeface != null)
            {
                //this.Typeface = loCustomTypeface;
            }

            //SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnSignatureImageViewTypefaceSizeSp);
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

                //    this.BeforeTextChanged += HandleBeforeTextChanged;
                //    this.TextChanged += HandleTextChanged;
                //    this.AfterTextChanged += HandleAfterTextChanged;

                this.FocusChange += HandleFocusChange;
            }           
        }

        public void DetachEvents()
        {
            if (EventsAttached)
            {
                // this.KeyPress +=HandleKeyPress;
                // this.BeforeTextChanged -= HandleBeforeTextChanged;
                // this.TextChanged -= HandleTextChanged;
                // this.AfterTextChanged -= HandleAfterTextChanged;

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
            if (sender is CustomSignatureImageView == false)
                return;

            var loEditCtrl = ((CustomSignatureImageView)(sender));
            EditControlBehavior loBehavior = loEditCtrl.BehaviorAndroid;
         
            // AJW - hook here until method changed to pass sender along
            if (e.HasFocus == true)
            {
                loEditCtrl.SetBackgroundResource(Resource.Drawable.EditTextFocused);

                loEditCtrl.ParentPanel.FocusedViewCurrent = sender;
                
                // no keyboard for sig capture Helper.ShowKeyboard(loEditCtrl); // show the keyboard 

                // make sure we're at the top of the screen
 //               loEditCtrl.ParentLayout.ScrollTo(0, loEditCtrl.Top);
            }
            else
            {
             
                //loEditCtrl.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);

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


        public bool IgnoreEvents { get; set; }



        #endregion

        public void AssociateControl(EditControlBehavior behavior)
        {
            _behavior = behavior;
            _behavior.AssociateWithCustomSignatureImageView(this);
        }
    }
}