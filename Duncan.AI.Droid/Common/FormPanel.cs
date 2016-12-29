
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using Android.App;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Provider;
using Android.Views.Animations;
using Android.Widget;

using Duncan.AI.Droid.Utils.EditControlManagement;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Duncan.AI.Droid.Utils.EditControlManagement.EditRules;
using Duncan.AI.Droid.Utils.HelperManagers;
using Duncan.AI.Droid.Utils.PickerDialogs;
using Duncan.AI.Droid.Utils.BarCode;
using Duncan.AI.Droid.Common;
using Duncan.AI.Droid.Utils;
using SignaturePad;
using System.Data;
using System.Linq;
using System.Collections.Generic;


using XMLConfig;
using Reino.ClientConfig;
using Reino.CommonLogic;
//using TTControl = Reino.ClientConfig.TTControl;

using Boolean = System.Boolean;
using Math = System.Math;
using String = System.String;
using AutoISSUE;

#if !_integrate_n5_support_
using ZXing;
using ZXing.Mobile;
#endif




namespace Duncan.AI.Droid.Common
{

    public class FormPanel : Activity, INotifyPropertyChanged
    {
        public XMLConfig.IssStruct thisStruct;
        public LinearLayout PanelRootPageLinearLayout;

        public ScrollView PanelRootPageScrollView;
     
        private Int32 _PanelPageCurrentDisplayedIndex;
        public Int32 PanelPageCurrentDisplayedIndex
        {
            get
            {
                return _PanelPageCurrentDisplayedIndex;
            }
            set
            {
                _PanelPageCurrentDisplayedIndex = value;
            }
        }
               



        DisplayMetrics metrics;
        Context context;

        public CommonFragment ParentFragment;



        public Int32 PanelPageDelta;
        public bool SkipMode = false;

        SignaturePickerDialog signaturePickerDialog = null;
        CustomSignatureImageView sigImg;

        //ListSupport listSupport = new ListSupport();

        const string cnButtonPlateOCRTag =  "btnPlateOCR";
        const string cnButtonVINScanTag = "btnVINScan";
        const string cnButtonGeoCodeTag = "btnGeoCode";


        // keep a reference of last geo code address confirmed for use
        //Android.Locations.Address gLastGeoCodeAddress = null;


        Boolean _isNew;

        // AJW - added for OCR
        Java.IO.File _photoFile;
        Java.IO.File _photoDir;

        //ISharedPreferences _prefs;


        private Boolean isPanelBuilt;

        //public FormPanel() 
        public FormPanel() : base()
        {
            _isNew = true;

        }


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // AJW - make sure the current edit view isn't covered
            Window.SetSoftInputMode(SoftInput.AdjustPan);

            //_prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            //_prefs = PreferenceManager.GetDefaultSharedPreferences(this);
        }


        public Boolean IsPanelBuilt
        {
            get { return isPanelBuilt; }
            set
            {
                if (value != IsPanelBuilt)
                {
                    isPanelBuilt = value;
                    // Whenever property value is changed, the PropertyChanged event is triggered
                    OnPropertyChanged("IsPanelBuilt");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // method to raise event
        protected void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }


        /// <summary>
        /// //////////
        /// </summary>
        /// <param name="e"></param>
        /*
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.Click += (sender, evt) =>
                {
                    new Handler().Post(delegate
                    {
                        var imm = (InputMethodManager)Control.Context.GetSystemService(Android.Content.Context.InputMethodService);
                        var result = imm.HideSoftInputFromWindow(Control.WindowToken, 0);

                        Console.WriteLine(result);
                    });
                };

                Control.FocusChange += (sender, evt) =>
                {
                    new Handler().Post(delegate
                    {
                        var imm = (InputMethodManager)Control.Context.GetSystemService(Android.Content.Context.InputMethodService);
                        var result = imm.HideSoftInputFromWindow(Control.WindowToken, 0);

                        Console.WriteLine(result);
                    });
                };
            }
        }
        */

        /// <summary>
        /// /////////
        /// </summary>
        /// <param name="SlideDir"></param>


        // animation duration constants
        const int cnAnimationDurationScale = 300;
        const int cnAnimationDurationFadeIn = 700;

        const int cnAnimationDurationSlide = 500;
        const int cnAnimationDuractionFadeOut = 500;


        public void CreateOrShowDisplayPage(Int32 SlideDir)
        {
            IsPanelBuilt = false;

            // only animate the swipes, not the first load
            if (SlideDir != 0)
            {
                Animation slideAnim = new TranslateAnimation(0, -metrics.WidthPixels * Math.Sign(SlideDir), 0, 0);
                slideAnim.Duration = cnAnimationDurationSlide;

                Animation fadeOut = new AlphaAnimation(1, 0);
                fadeOut.Duration = cnAnimationDuractionFadeOut;

                slideAnim.AnimationEnd += delegate
                    {
                        BuildCurrentDisplayPage();
                    };

                var animSet = new AnimationSet(false);
                animSet.AddAnimation(slideAnim);
                animSet.AddAnimation(fadeOut);

                PanelRootPageLinearLayout.StartAnimation(animSet);
            }
            else
            {
                BuildCurrentDisplayPage();
            }
        }


        public void BuildCurrentDisplayPage()
        {
            var scaleAnim = new ScaleAnimation(0.3f, 1.0f, 0.3f, 1.0f, 600.0f, 600.0f) { Duration = cnAnimationDurationScale };
            Animation fadeIn = new AlphaAnimation(0, 1);
            fadeIn.Duration = cnAnimationDurationFadeIn;
            var animSet = new AnimationSet(false);
            animSet.AddAnimation(scaleAnim);
            animSet.AddAnimation(fadeIn);
            PanelRootPageLinearLayout.StartAnimation(animSet);

            //if (_isNew == true)
            {
                PanelRootPageLinearLayout.RemoveAllViews();
                BuildCurrentDisplayPageLayoutFields();
            }

            BuildNavigationToolbar();

            PanelRootPageLinearLayout.Invalidate();
            PanelRootPageLinearLayout.RequestLayout();


            SetFirstFocusFieldOnCurrentPage();

            IsPanelBuilt = true;
        }


        private void BuildFormNavigationToolbar()
        {

            // the navigation bar is at the top of each form
            bool loFirstPage = (PanelPageCurrentDisplayedIndex == 0);
            bool loLastPage = (PanelPageCurrentDisplayedIndex >= (thisStruct.Panels.Count-1));

            //  are we adding something?
            if ( loFirstPage == true )
            {

                var loToolbarLinearLayout = new LinearLayout(context);

                //LinearLayout layout_horiz_btns = view.FindViewById<LinearLayout>(Resource.Id.NavButtons);  
                //layout_horiz_btns.Orientation = Orientation.Horizontal;

                //var layoutParamsButton = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                var layoutParamsButton = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,  ViewGroup.LayoutParams.WrapContent);
                layoutParamsButton.SetMargins(10, 25, 10, 25);




                if (loFirstPage == true)
                {
                        var btnIssueFormNavigation = new Button(context);
                        btnIssueFormNavigation.SetBackgroundResource(Resource.Drawable.button_issueform_navigation);
                        btnIssueFormNavigation.SetTextColor(Android.Graphics.Color.Black);
                    

                        // initialize our typeface 
                        Typeface loCustomTypefaceParent = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnIssueFormNavigationTypeface);
                        if (loCustomTypefaceParent != null)
                        {
                            btnIssueFormNavigation.Typeface = loCustomTypefaceParent;
                        }
                        btnIssueFormNavigation.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnIssueFormNavigationTypefaceSizeSp);


                        string loNavigationTitleText = ParentFragment.GetFragmentMenuItemName();
                        btnIssueFormNavigation.Text = loNavigationTitleText;
                        btnIssueFormNavigation.Gravity = (GravityFlags.Left | GravityFlags.CenterVertical);

                        btnIssueFormNavigation.Tag = "btnIssueFormNavigation";
                        btnIssueFormNavigation.Click += btnIssueFormNavigationClick;
                        loToolbarLinearLayout.AddView(btnIssueFormNavigation, layoutParamsButton);
                }



                loToolbarLinearLayout.SetGravity(GravityFlags.Center);

                if (loToolbarLinearLayout.ChildCount > 0)
                {
                    PanelRootPageLinearLayout.AddView(loToolbarLinearLayout); //, lp);
                }

            }
        }






        private void BuildNavigationToolbar()
        {

#if _use_buttons_for_toolbar_

                these buttons have been replaced with a toolbar above the keyboard

            // all buttons appear every time, just obviously disabled when unavailable
            bool loFirstPage = (PanelPageCurrentDisplayedIndex == 0);
            bool loLastPage = (PanelPageCurrentDisplayedIndex >= (thisStruct.Panels.Count-1));

            //  if we found fields to display, or if this is the last page
            if (PanelRootPageLinearLayout.ChildCount > 0 || PanelPageCurrentDisplayedIndex == thisStruct.Panels.Count - 1)
            {


                var loToolbarLinearLayout = new LinearLayout(context);

                //LinearLayout layout_horiz_btns = view.FindViewById<LinearLayout>(Resource.Id.NavButtons);  
                //layout_horiz_btns.Orientation = Orientation.Horizontal;

                var layoutParamsButton = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                                       ViewGroup.LayoutParams.WrapContent);
                layoutParamsButton.SetMargins(10, 20, 10, 10);




                // AJW = todo - allow order flip for preferences
                bool loRightHanded = true;

                if (loRightHanded == true)
                {
                    // alight buttons for right handed user
                }
                else
                {
                    // alight buttons for left handed user
                }



                if (loLastPage == false)
                {
                    // LIST ITEM PREVIOUS
                    /* AJW will hook these functions to custom keyboard at a later date */
                    //if (CurrentPanelIdx < thisStruct.Panels.Count - 1)
                    // list item prev next enabled/disabled by focus field
                    {
                        var btnPrevListItem = new Button(context);
                        btnPrevListItem.SetBackgroundDrawable(context.Resources.GetDrawable(Resource.Drawable.button_toolbar));
                        btnPrevListItem.SetTextColor(Android.Graphics.Color.White);
                        btnPrevListItem.SetBackgroundColor(Color.Rgb(0xff, 0x6e, 0x2b)); // "#ff6e2b");


                        //btnPrevListItem.Text = "PREV";
                        btnPrevListItem.Text = "";
                        //btnPrevListItem.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_list_news_prev_32_rev2, 0, 0, 0);
                        btnPrevListItem.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_list_news_prev_32_rev3, 0, 0, 0);


                        btnPrevListItem.Tag = "btnPrevListItem";
                        btnPrevListItem.Click += btnPreviousListItemClick;
                        loToolbarLinearLayout.AddView(btnPrevListItem, layoutParamsButton);
                    }
                }


                if (loLastPage == false)
                {
                    // LIST ITEM NEXT
                    //if (CurrentPanelIdx < thisStruct.Panels.Count - 1)
                    // list item prev next enabled/disabled by focus field

                    {
                        var btnNextListItem = new Button(context);
                        btnNextListItem.SetBackgroundDrawable(context.Resources.GetDrawable(Resource.Drawable.button_toolbar));
                        btnNextListItem.SetTextColor(Android.Graphics.Color.White);
                        btnNextListItem.SetBackgroundColor(Color.Rgb(0xff, 0x6e, 0x2b)); // "#ff6e2b");

                        //btnNextListItem.Text = "NEXT";
                        btnNextListItem.Text = "";
                        //btnNextListItem.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_list_news_next_32_rev2, 0, 0, 0);
                        btnNextListItem.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_list_news_next_32_rev3, 0, 0, 0);



                        btnNextListItem.Tag = "btnNextListItem";
                        btnNextListItem.Click += btnNextListItemClick;
                        loToolbarLinearLayout.AddView(btnNextListItem, layoutParamsButton);
                    }
                }


                if (loLastPage == false)
                {
                    // LIST POP UP
                    //if (CurrentPanelIdx < thisStruct.Panels.Count - 1)
                    // list item prev next enabled/disabled by focus field

                    {
                        var btnListItemDisplayPopUp = new Button(context);

                        btnListItemDisplayPopUp.SetBackgroundDrawable(context.Resources.GetDrawable(Resource.Drawable.button_toolbar));

                        btnListItemDisplayPopUp.SetTextColor(Android.Graphics.Color.White);

                        btnListItemDisplayPopUp.SetBackgroundColor(Color.Rgb(0xff, 0x6e, 0x2b)); // "#ff6e2b");



                        //btnListItemDisplayPopUp.Text = "LIST";
                        btnListItemDisplayPopUp.Text = "";
                        btnListItemDisplayPopUp.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_list_news_32_rev3, 0, 0, 0);



                        btnListItemDisplayPopUp.Tag = "btnListItemDisplayPopUp";
                        btnListItemDisplayPopUp.Click += btnListItemDisplayPopUpClick;
                        loToolbarLinearLayout.AddView(btnListItemDisplayPopUp, layoutParamsButton);
                    }
                }



                if (loLastPage == false)
                {
                    // TAB BACKWARD
                    //if (CurrentPanelIdx != 0)
                    bool loBtnPrevEnabled = (PanelPageCurrentDisplayedIndex != 0);
                    {
                        var btnPrevious = new Button(context);
                        btnPrevious.SetBackgroundDrawable(context.Resources.GetDrawable(Resource.Drawable.button_toolbar));
                        btnPrevious.SetTextColor(Android.Graphics.Color.White);

                        btnPrevious.SetBackgroundColor(Color.Rgb(0xff, 0x6e, 0x2b)); // "#ff6e2b");


                        //btnPrevious.Text = "< PREVIOUS";
                        //btnPrevious.Text = "< PREV";
                        //btnPrevious.Text = "PREV";
                        btnPrevious.Text = "";
                        //btnPrevious.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_button_left, 0, 0, 0);
                        btnPrevious.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_button_left_32, 0, 0, 0);

                        btnPrevious.Tag = "btnPrevious";
                        btnPrevious.Click += btnPreviousClick;
                        loToolbarLinearLayout.AddView(btnPrevious, layoutParamsButton);
                    }
                }

                if (loLastPage == false)
                {
                    // TAB FORWARD
                    //if (CurrentPanelIdx < thisStruct.Panels.Count - 1)
                    bool loBtnNextEnabled = (PanelPageCurrentDisplayedIndex < thisStruct.Panels.Count - 1);
                    {
                        var btnNext = new Button(context);
                        btnNext.SetBackgroundDrawable(context.Resources.GetDrawable(Resource.Drawable.button_toolbar));
                        btnNext.SetTextColor(Android.Graphics.Color.White);

                        btnNext.SetBackgroundColor(Color.Rgb(0xff, 0x6e, 0x2b)); // "#ff6e2b");



                        //btnNext.Text = "NEXT >";
                        //btnNext.Text = "NEXT";
                        btnNext.Text = "";
                        //btnNext.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.ic_button_right, 0);
                        //btnNext.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.ic_button_right_64, 0);
                        btnNext.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.ic_button_right_32, 0);
                        btnNext.Tag = "btnNext";
                        btnNext.Click += btnNextClick;
                        loToolbarLinearLayout.AddView(btnNext, layoutParamsButton);
                    }
                }


                loToolbarLinearLayout.SetGravity(GravityFlags.Center);
                PanelRootPageLinearLayout.AddView(loToolbarLinearLayout); //, lp);

            }
#endif
        }




        private void UpdateEditControlBehaviorDelagateHooks(EditControlBehavior iBehavior)
        {
            // AJW - take care in updating existing delegates - 
            // if we just add the same delegate again to the existing collection, we'll end up 
            // with a multicast delegate scenario where our delegate is called repeatedly,
            // once for each time another was added to the collection

            iBehavior.OnGetFormEditMode -= this.GetFormEditMode;
            iBehavior.OnGetFormEditMode += this.GetFormEditMode;

            iBehavior.OnGetFormEditAttrs -= this.GetFormEditAttrs; 
            iBehavior.OnGetFormEditAttrs += this.GetFormEditAttrs;


            iBehavior.OnSetFormEditAttr -= this.SetFormEditAttr;
            iBehavior.OnSetFormEditAttr += this.SetFormEditAttr;


            // Make sure we have the event OnRestrictionForcesRebuildDisplay event for the passed behavior object
           iBehavior.OnRestrictionForcesDisplayRebuild -= this.OnRestrictionForcesDisplayRebuild;
           iBehavior.OnRestrictionForcesDisplayRebuild += this.OnRestrictionForcesDisplayRebuild;
           

            // Certain edit restriction classes need application-defined delegate methods assigned
            foreach (EditRestriction loEditRestrict in iBehavior.EditRestrictions)
            {

                if (loEditRestrict is Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_ForceGlobalCurrentValue)
                {
                    // We need to add an event to TER_ForceGlobalCurrentValueImp so it 
                    // can call logic in the form 
                    //((TER_ForceGlobalCurrentValue)(loEditRestrict)).OnGetCurrentGlobalFieldValue -= Reino.CommonLogic.IssueAppImp.GlobalIssueApp.GetCurrentGlobalFieldValue;
                    //((TER_ForceGlobalCurrentValue)(loEditRestrict)).OnGetCurrentGlobalFieldValue += Reino.CommonLogic.IssueAppImp.GlobalIssueApp.GetCurrentGlobalFieldValue;
                }
                else if (loEditRestrict is Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_SetGlobalCurrentValue)
                {
                    // We need to add an event to TER_SetGlobalCurrentValueImp so it 
                    // can call logic in the form 
                    //((TER_SetGlobalCurrentValue)(loEditRestrict)).OnSetCurrentGlobalFieldValue -= Reino.CommonLogic.IssueAppImp.GlobalIssueApp.SetCurrentGlobalFieldValue;
                    //((TER_SetGlobalCurrentValue)(loEditRestrict)).OnSetCurrentGlobalFieldValue += Reino.CommonLogic.IssueAppImp.GlobalIssueApp.SetCurrentGlobalFieldValue;
                }
                else if (loEditRestrict is Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_ForceSequence)
                {
                    // We need to add an event to TER_ForceSequence so it can call logic
                    // in the form to control setting the next issue number
                    ((Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_ForceSequence)(loEditRestrict)).OnSetIssueNoFields -= this.SetIssueNoFields;
                    ((Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_ForceSequence)(loEditRestrict)).OnSetIssueNoFields += this.SetIssueNoFields;
                }
                else if (loEditRestrict is Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_CurrentUser)
                {
                    // This edit restriction needs to know where to find datasets to get current user info
                    //((TER_CurrentUser)(loEditRestrict)).clientDef = TClientDef.GlobalClientDef;
                }
                else if (loEditRestrict is Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_SearchHotSheet)
                {
                    ((Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_SearchHotSheet)(loEditRestrict)).OnDoSearch -= this.DoHotSheetSearch;
                    ((Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_SearchHotSheet)(loEditRestrict)).OnDoSearch += this.DoHotSheetSearch;
                }
                else if (loEditRestrict is Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_HotsheetFilter)
                {
                    ((Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_HotsheetFilter)(loEditRestrict)).OnDoHotSheetFilter -= this.DoHotSheetFilter;
                    ((Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_HotsheetFilter)(loEditRestrict)).OnDoHotSheetFilter += this.DoHotSheetFilter;
                }
                else if (loEditRestrict is Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_Protected)
                {
                    ((Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_Protected)(loEditRestrict)).OnRestrictionForcesDisplayRebuild -= OnRestrictionForcesDisplayRebuild;
                    ((Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_Protected)(loEditRestrict)).OnRestrictionForcesDisplayRebuild += OnRestrictionForcesDisplayRebuild;
                }
                else if (loEditRestrict is Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_Hidden)
                {
                    ((Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_Hidden)(loEditRestrict)).OnRestrictionForcesDisplayRebuild -= OnRestrictionForcesDisplayRebuild;
                    ((Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_Hidden)(loEditRestrict)).OnRestrictionForcesDisplayRebuild += OnRestrictionForcesDisplayRebuild;
                }
                else if (loEditRestrict is Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_ListFilter)
                {
                    ((Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_ListFilter)(loEditRestrict)).OnListContentsChangedByRestriction -= OnListContentsChangedByRestriction;
                    ((Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_ListFilter)(loEditRestrict)).OnListContentsChangedByRestriction += OnListContentsChangedByRestriction;
                }


            }

        }

        public async Task<int> BuildCurrentDisplayPageLayoutFields()
        {
            if (_isNew)
            {
               // not here?   DroidContext.ResetControlStatusByStructName(thisStruct.Name);
                _isNew = false;
            }


            // start with navigation view on the top of page 0
            BuildFormNavigationToolbar();

            Single widths = 0;
            var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            // a horizontal layout to have multiple combos on one line
            var layoutHoriz = new LinearLayout(context);
            bool loAtLeastOneLabelAddedToCurrentHorizontalLayoutView = false;

            // make labels and boxes
            if (thisStruct.Panels != null && thisStruct.Panels.Count > PanelPageCurrentDisplayedIndex &&
                thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields != null)
            {

                // AJW - except we do nothing with it here
                //ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
                //ISharedPreferencesEditor loSharedPreferences = prefs.Edit();
                // only created this once, not re-build for every field that needs
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);


                // reset - only one per form
                bool loGeoCodeButtonAdded = false;


                // track the row positions from the XML screen layout
                // in the layout the screens are laid out in multiple panels,
                // but in Android we put them all in one scrolling form
                // so the XML screen layout positions will reset on each panel
                int loPreviousPanelField_LegacyPositionTop = -1;

                // track the dividers added
                int loDividerCount = 0;

                // use index loop to help look ahead when needed
                //foreach (PanelField panelField in thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields)
                for (int loPanelFieldIdx = 0; loPanelFieldIdx < thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields.Count; loPanelFieldIdx++)
                {
                    PanelField panelField = thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields[ loPanelFieldIdx ];



                    if (
                         (panelField.Name.Contains(AutoISSUE.DBConstants.sqlPendingAttachmentsStr)) ||
                         (panelField.Name.Contains(AutoISSUE.DBConstants.sqlTireStemsFrontTimeStr)) ||
                         (panelField.Name.Contains(AutoISSUE.DBConstants.sqlTireStemsRearTimeStr))
                        )
                        // TODO - create/display these as needed
                        continue;





                    ///////////////////
                    // Some logic to help determine row placement
                    ///////////////////

                    // assume so
                    bool loLegacyPositionIndicatesNewRow = false;


                    // we're only considering the info from this field in placement calculation if the field is visible
                    if (panelField.IsHidden == false)
                    {
                        // 
                        int loCurentPanelField_LegacyPositionTop = -1;
                        if (panelField.fEditFieldDef != null)
                        {
                            loCurentPanelField_LegacyPositionTop = panelField.fEditFieldDef.Top;
                        }
                        
                        // do we have enough info to make a judgement about row placement?
                        if ((loPreviousPanelField_LegacyPositionTop != -1) && (loCurentPanelField_LegacyPositionTop != -1))
                        {
                            // how close are they in the legacy layout ?
                            int loTopDifference = Math.Abs(loCurentPanelField_LegacyPositionTop - loPreviousPanelField_LegacyPositionTop);

                            // in consideration of occasional sloppiness, anything within 5 pixels considered to be the same row
                            loLegacyPositionIndicatesNewRow = (loTopDifference > 5);
                        }


                        // set for next
                        loPreviousPanelField_LegacyPositionTop = loCurentPanelField_LegacyPositionTop;
                    }

                    ////////////////////




                    ////////////////////

                    // when we had full screen width spinners we wanted to do this sometimes
                    bool loForceSingleItemOnRow = (panelField.OptionsList.Columns != null && panelField.OptionsList.Columns.Length > 0 && panelField.OptionsList.IsListOnly);

                    // but for now let them fall on the defined rows
                    loForceSingleItemOnRow = false;



                    /////////////////////

                    // is this a grouping panel?
                    if (panelField.FieldType.Equals("efDivider") == true)
                    {

                        // look ahead - is the next object also a divider?
                        if (loPanelFieldIdx < thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields.Count - 1)
                        {
                            PanelField nextPanelField = thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields[loPanelFieldIdx + 1];
                            if (nextPanelField.FieldType.Equals("efDivider") == true)
                            {
                                // two dividers in a row means a an empty page, skip this one
                                continue;
                            }
                        }


                         // any pending horizontal views needing to committed before we add the divider?
                        if (layoutHoriz.ChildCount > 0)
                        {
                            var parent = (LinearLayout)layoutHoriz.Parent;

                            if (parent != null)
                            {
                                // AJW - TODO is thie strategy out of use now that we have a single scrolling panel?
                                parent.RemoveView(layoutHoriz);
                            }

                            // commit this previous view
                            PanelRootPageLinearLayout.AddView(layoutHoriz);

                            // start the next one
                            layoutHoriz = new LinearLayout(context) { Orientation = Android.Widget.Orientation.Horizontal };
                            // widths always back to 0 for start of new section
                            widths = 0;
                            loAtLeastOneLabelAddedToCurrentHorizontalLayoutView = false;
                        }



                        // construct a divider to help in navigation
                        TextView oneTextViewPanelDivider = new TextView(context);
                        oneTextViewPanelDivider.Text = panelField.Label;

                        oneTextViewPanelDivider.SetTextColor(DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.civicsmart_white));
                        oneTextViewPanelDivider.SetBackgroundColor(DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.neutral_orange));

                        oneTextViewPanelDivider.Gravity = (GravityFlags.Left | GravityFlags.CenterVertical);

                        // initialize our typeface 
                        Typeface loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnDividerLabelTypeface);
                        if (loCustomTypeface != null)
                        {
                            oneTextViewPanelDivider.Typeface = loCustomTypeface;
                        }
                        oneTextViewPanelDivider.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnDividerLabelTypefaceSizeSp);



                        // setup our margins according to our placement
                        var lpPanelDivider = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                        if (loDividerCount == 0)
                        {
                            // nothing at the top
                            lpPanelDivider.SetMargins(0, 0, 0, 50);   // TODO - put this in scalable units so it looks OK on larger screens too
                        }
                        else
                        {
                            // give a little space above and below
                            lpPanelDivider.SetMargins(0, 50, 0, 50);
                        }

                        // keep count and add our divider
                        loDividerCount++;
                        PanelRootPageLinearLayout.AddView(oneTextViewPanelDivider, lpPanelDivider);


                        // move on to next component 
                        continue;
                    }


                    /////////////////////
                    bool loIsAGeoCodeCandidate = 
                        (
                          (panelField.Name.ToUpper().Equals(AutoISSUE.DBConstants.sqlLocMeterNumberStr) == true) ||
                          (panelField.Name.ToUpper().Equals(AutoISSUE.DBConstants.sqlLocBlockStr) == true) ||
                          (panelField.Name.ToUpper().Equals(AutoISSUE.DBConstants.sqlLocDirectionStr) == true) 
                       );

                    if (loIsAGeoCodeCandidate == true)
                    {
                        // but only once per form
                        if (loGeoCodeButtonAdded == true)
                        {
                            // already have one, no longer considered
                            loIsAGeoCodeCandidate = false;
                        }
                    }


                    bool loSpecialFunctionRequiringNewRow = 
                      (
                        (panelField.Name.ToUpper().Equals(AutoISSUE.DBConstants.sqlVehLicNoStr) == true) ||
                        (panelField.Name.ToUpper().Equals(AutoISSUE.DBConstants.sqlVehVINStr) == true) ||
                        (panelField.Name.ToUpper().Equals(Constants.OFFICER_SIGNATURE) == true) 
                      );



                    // based on our evaluations, do we need to force a new row for this field?
                    if (
                        ( loLegacyPositionIndicatesNewRow == true ) ||
                        ( loIsAGeoCodeCandidate == true ) ||
                        ( loSpecialFunctionRequiringNewRow == true )
                        )
                    {

                        // any pending horizontal views needing to committed before start the new row?
                        if (layoutHoriz.ChildCount > 0)
                        {
                            var parent = (LinearLayout)layoutHoriz.Parent;

                            if (parent != null)
                            {
                                // AJW - TODO is thie strategy out of use now that we have a single scrolling panel?
                                parent.RemoveView(layoutHoriz);
                            }

                            // commit this previous view - this will force our plate to be on a horizonatl view by itself
                            PanelRootPageLinearLayout.AddView(layoutHoriz);

                            // start the next one
                            layoutHoriz = new LinearLayout(context) { Orientation = Android.Widget.Orientation.Horizontal };
                            // widths always back to 0 for start of new section
                            widths = 0;
                            loAtLeastOneLabelAddedToCurrentHorizontalLayoutView = false;
                        }
                    }


                    // KLUDGY - make sure the VIN has enough room for the scanner 
                    if (
                        (panelField.Name.ToUpper().Equals(AutoISSUE.DBConstants.sqlVehVINStr) == true)
                        )
                    {
                        if (panelField.Width == 1)
                        {
                            panelField.Width = 0.8f;  // not 100% width, scale to leave room for scan button
                        }
                    }


                    /////////////////////



                    //  a vertical layout for the label/textbox combo
                    var layoutVert = new LinearLayout(context) { Orientation = Orientation.Vertical };
                    lp.SetMargins(10, 20, 0, 0);
                    layoutVert.LayoutParameters = lp;

                    widths += panelField.Width;
                    if (widths > 1 || (loForceSingleItemOnRow == true) )
                    {
                        var parent = (LinearLayout)layoutHoriz.Parent;
                        if (parent != null)
                        {
                            parent.RemoveView(layoutHoriz);
                        }

                        //hook up events here since we have the panel and layout we need.
                        //then add or re-add
                        PanelRootPageLinearLayout.AddView(layoutHoriz);
                        layoutHoriz = new LinearLayout(context) { Orientation = Android.Widget.Orientation.Horizontal };

                        if (loForceSingleItemOnRow == true)
                        //if (panelField.OptionsList.Columns != null && panelField.OptionsList.Columns.Length > 0 && panelField.OptionsList.IsListOnly)
                        {
                            widths = 2;
                        }
                        else
                        {
                            widths = panelField.Width;

                        }
                        loAtLeastOneLabelAddedToCurrentHorizontalLayoutView = false;
                    }





#if _moved_to_prepare_for_edit_delete_when_testing_satisfied_

                    // AJW - lets do this outside the loop, not for every field
                    //ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
                    //ISharedPreferencesEditor editor = prefs.Edit();

                    if (panelField.Name.Equals(Constants.ISSUENO_COLUMN))
                    {
                        panelField.Value = thisStruct.sequenceId;
                    }


                    if (panelField.Name.Equals(Constants.ISSUENOPFX_COLUMN))
                    {
                        panelField.Value = thisStruct.prefix;
                    }


                    if (panelField.Name.Equals(Constants.ISSUETIME_COLUMN))
                    {
                        string loTmpBuf = "";
                        DateTimeManager.OsTimeToTimeString(DateTime.Now, panelField.EditMask, ref loTmpBuf);
                        panelField.Value = loTmpBuf;
                    }

                    if (panelField.Name.Equals(Constants.ISSUEDATE_COLUMN))
                    {
                        string loTmpBuf = "";
                        DateTimeManager.OsDateToDateString(DateTime.Today, panelField.EditMask, ref loTmpBuf);
                        panelField.Value = loTmpBuf;
                    }
#endif



                    /////////////////////

                    // we only need to create labels for non-blank values, unless we already have one one in the same veiw
                    if (
                        (string.IsNullOrEmpty(panelField.Label) == false) ||
                        ( loAtLeastOneLabelAddedToCurrentHorizontalLayoutView == true )
                        )
                    {
                        View loNewLabel = Helper.MakeLabel(panelField, context, thisStruct);

                        // AJW - when re-adding, we need to initialize view state since form init EditRestrictions won't be processed 
                        if (_isNew == false)
                        {
                            if (panelField.IsHidden == true)
                            {
                                lp.SetMargins(0, 0, 0, 0);
                                loNewLabel.Visibility = ViewStates.Gone;
                            }
                        }

                        layoutVert.AddView(loNewLabel);
                        loAtLeastOneLabelAddedToCurrentHorizontalLayoutView = true;
                    }

                    if (panelField.Name == Constants.OFFICER_SIGNATURE)
                    {

                        if (sigImg == null)
                        {
                            sigImg = Helper.MakeCustomSignatureImageView(panelField, context, metrics, layoutVert, thisStruct.Panels[PanelPageCurrentDisplayedIndex], thisStruct);
                            sigImg.Click += btnSignatureViewClick;
                        }



                        // if we have a signature, show the image of it and not the sigcap
                        if (!String.IsNullOrEmpty(panelField.Value))
                        {
                            Helper.LoadBitmapToCustomSignatureImageView(sigImg, thisStruct.sequenceId);
                            // we're adding this below, not needed 2x layoutVert.AddView(sigImg);
                        }
                        //else
                        //{
                        //    if (sigImg == null)
                        //    {
                        //        //sigImg = new CustomSignatureImageView(context);
                        //        sigImg = Helper.MakeCustomSignatureImageView(panelField, context, metrics, layoutVert, thisStruct.Panels[PanelPageCurrentDisplayedIndex], thisStruct);
                        //    }
                        //}

                        
                        // create a display view that uses up the half the width of the screen but preserves 
                        // the width/height ratio so that signature won't appear skewed from resizing
                        int loSignatureBitmapViewWidth = 700;
                        int loSignatureBitmapViewHeight = 270;

                        if (metrics != null)
                        {
                            loSignatureBitmapViewWidth = (int)((metrics.WidthPixels - (20 * 2)) / 2); // half screen, plus left/right padding
                            loSignatureBitmapViewHeight = (int)((double)loSignatureBitmapViewWidth / 2.67);
                        }


						var loSignatureBitmapViewLayoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
                        {
                            LeftMargin = 0,
                            TopMargin = 0,
                            Width = loSignatureBitmapViewWidth,
                            Height = loSignatureBitmapViewHeight
                        };

                        sigImg.LayoutParameters = loSignatureBitmapViewLayoutParams;
                        //sigImg.Visibility = ViewStates.Visible;

                        try
                        {
                            var parent = (LinearLayout)sigImg.Parent;

                            if (parent != null)
                            {
                                // AJW - TODO is thie strategy out of use now that we have a single scrolling panel?
                                parent.RemoveView(sigImg);
                            }



                            layoutVert.AddView(sigImg);
                            layoutHoriz.AddView(layoutVert);
                        }
                        catch (Exception e)
                        {
                            LoggingManager.LogApplicationError(e, "FormPanel", e.TargetSite.Name);
                        }

                        // commit this previous view
                        PanelRootPageLinearLayout.AddView(layoutHoriz);

                        // start the next one
                        layoutHoriz = new LinearLayout(context) { Orientation = Android.Widget.Orientation.Horizontal };
                        // widths always back to 0 for start of new section
                        widths = 0;
                        loAtLeastOneLabelAddedToCurrentHorizontalLayoutView = false;

                        continue;
                    }

                    if (panelField.OptionsList.Columns != null && panelField.OptionsList.Columns.Length > 0)
                    {

#if _allow_spinners_
                            if (panelField.OptionsList.IsListOnly)
                            {
                                var customSpinner = Helper.MakeSpinnerSelector(panelField.Name, panelField, context, metrics, layoutVert, thisStruct.Panels[PanelPageCurrentDisplayedIndex], thisStruct);
                                if (customSpinner != null)
                                {

                                    customSpinner.Behavior.OnGetFormEditMode += this.GetFormEditMode;
                                    customSpinner.Behavior.OnGetFormEditAttrs += this.GetFormEditAttrs;
                                    customSpinner.Behavior.OnSetFormEditAttr += this.SetFormEditAttr;

                                    var parent = (LinearLayout)customSpinner.Parent;
                                    if (parent != null)
                                    {
                                        parent.RemoveView(customSpinner);
                                    }
                                    //then add or re-add
                                    layoutVert.AddView(customSpinner);



                                     // AJW - to do, we REALLY need an implementation of PrepareForEdit for new citations
                                    if (_isNew == false)
                                    {
                                        if (customSpinner.Behavior != null)
                                        {
                                           // customSpinner.Behavior.
                                        }
                                    }


                                }
                            }
                            else
#endif
                        {
                            CustomAutoTextView customAutoEditText = Helper.MakeAutoTextView(panelField, context, metrics, layoutVert, thisStruct.Panels[PanelPageCurrentDisplayedIndex], thisStruct);
                            if (customAutoEditText != null)
                            {
                                //customAutoEditText.Behavior.OnGetFormEditMode += this.GetFormEditMode;
                                //customAutoEditText.Behavior.OnGetFormEditAttrs += this.GetFormEditAttrs;
                                //customAutoEditText.Behavior.OnSetFormEditAttr += this.SetFormEditAttr;
                                UpdateEditControlBehaviorDelagateHooks(customAutoEditText.BehaviorAndroid);


                                var parent = (LinearLayout)customAutoEditText.Parent;
                                if (parent != null)
                                {
                                    parent.RemoveView(customAutoEditText);
                                }
                                //then add or re-add
                                layoutVert.AddView(customAutoEditText);
                            }

                        }
                    }
                    else
                    {
                        CustomEditText customEditText = Helper.MakeEditTextBox(panelField, context, metrics, layoutVert, thisStruct.Panels[PanelPageCurrentDisplayedIndex], thisStruct);
                        if (customEditText != null)
                        {
                            //customEditText.Behavior.OnGetFormEditMode += this.GetFormEditMode;
                            //customEditText.Behavior.OnGetFormEditAttrs += this.GetFormEditAttrs;
                            //customEditText.Behavior.OnSetFormEditAttr += this.SetFormEditAttr;
                            UpdateEditControlBehaviorDelagateHooks(customEditText.BehaviorAndroid);


                            var parent = (LinearLayout)customEditText.Parent;
                            if (parent != null)
                            {
                                parent.RemoveView(customEditText);
                            }
                            //then add or re-add
                            layoutVert.AddView(customEditText);
                        }
                    }
                    


                    // add the label+field combo
                    layoutHoriz.AddView(layoutVert);



                    // vehicle license plate gets the LPR button
                    if (panelField.Name.ToUpper().Equals(AutoISSUE.DBConstants.sqlVehLicNoStr) == true)
                    {

                        // the photo OCR button - add after the lic plate field in same layout
                        AddLPRRButtonToLinearLayout(layoutHoriz);

                        // commit this previous view
                        PanelRootPageLinearLayout.AddView(layoutHoriz);

                        // start the next one
                        layoutHoriz = new LinearLayout(context) { Orientation = Android.Widget.Orientation.Horizontal };
                        // widths always back to 0 for start of new section
                        widths = 0;
                        loAtLeastOneLabelAddedToCurrentHorizontalLayoutView = false;


                        continue;
                    }


                    // VIN button the scan button
                    if (panelField.Name.ToUpper().Equals(AutoISSUE.DBConstants.sqlVehVINStr) == true)
                    {

                        // add after the field in same layout
                        AddVINBarcodeScanButtonToLinearLayout(layoutHoriz);

                        // commit this previous view
                        PanelRootPageLinearLayout.AddView(layoutHoriz);

                        // start the next one
                        layoutHoriz = new LinearLayout(context) { Orientation = Android.Widget.Orientation.Horizontal };
                        // widths always back to 0 for start of new section
                        widths = 0;
                        loAtLeastOneLabelAddedToCurrentHorizontalLayoutView = false;


                        continue;
                    }


                    // first location gets the geocode button 
                    if (

                          (panelField.Name.ToUpper().Equals(AutoISSUE.DBConstants.sqlLocMeterNumberStr) == true) ||
                          (panelField.Name.ToUpper().Equals(AutoISSUE.DBConstants.sqlLocBlockStr) == true) ||
                          (panelField.Name.ToUpper().Equals(AutoISSUE.DBConstants.sqlLocDirectionStr) == true) 

                       )
                    {
                        // but only once per form
                        if (loGeoCodeButtonAdded == false)
                        {
                            // the geocode button 
                            AddGeoCodeButtonToLinearLayout(layoutHoriz);




                            // added
                            loGeoCodeButtonAdded = true;

                            // commit this previous view
                            PanelRootPageLinearLayout.AddView(layoutHoriz);

                            // start the next one
                            layoutHoriz = new LinearLayout(context) { Orientation = Android.Widget.Orientation.Horizontal };
                            // widths always back to 0 for start of new section
                            widths = 0;
                            loAtLeastOneLabelAddedToCurrentHorizontalLayoutView = false;


                            continue;
                        }
                    }




                }
            }


            // add any stragglers
            if (layoutHoriz.ChildCount > 0)
            {
                PanelRootPageLinearLayout.AddView(layoutHoriz);
            }
            else
            {
                if (PanelRootPageLinearLayout.ChildCount >= 1)
                {
                    PanelRootPageLinearLayout.AddView(layoutHoriz);
                }
            }

            //if (PanelRootPageLinearLayout.ChildCount >= 1)
            //{
            //    PanelRootPageLinearLayout.AddView(layoutHoriz);
            //}


            return PanelRootPageLinearLayout.ChildCount;
        }


        public void AddLPRRButtonToLinearLayout(LinearLayout iLinearLayout)
        {
            int cnImageButtonMarginLeft = 25;
            int cnImageButtonMarginTop = 55;
            int cnImageButtonMarginRight = 25;
            int cnImageButtonMarginBottom = 20;

            // the photo OCR button - add after the lic plate field in same layout
            var newOCRButton = new ImageButton(context);
            newOCRButton.Tag = cnButtonPlateOCRTag;
            newOCRButton.Click += btnPlateOCRClick;

            //newOCRButton.SetBackgroundResource(Resource.Drawable.ic_photo_camera_black_36dp);
            newOCRButton.SetBackgroundResource(Resource.Drawable.Button_in_form_action_border);
            newOCRButton.SetImageDrawable(DroidContext.ApplicationContext.Resources.GetDrawable(Resource.Drawable.ic_photo_camera_black_36dp));

            newOCRButton.SetAdjustViewBounds(true);
            newOCRButton.SetScaleType(ImageView.ScaleType.FitCenter);

            // initialize our typeface 
            // no text, no need Helper.SetTypefaceForButton(newOCRButton, FontManager.cnMenuPopupButtonExitTypeface, FontManager.cnMenuPopupButtonExitTypefaceSizeSp);

            LinearLayout.LayoutParams layoutParamsButton = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            layoutParamsButton.SetMargins(cnImageButtonMarginLeft, cnImageButtonMarginTop, cnImageButtonMarginRight, cnImageButtonMarginBottom);
            layoutParamsButton.Gravity = GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
            newOCRButton.LayoutParameters = layoutParamsButton;

            iLinearLayout.AddView(newOCRButton, layoutParamsButton);
        }


        public void AddVINBarcodeScanButtonToLinearLayout(LinearLayout iLinearLayout)
        {

            // we are disabling this for now - later should be a REGISTRY setting
            return;


            int cnImageButtonMarginLeft = 25;
            int cnImageButtonMarginTop = 55;
            int cnImageButtonMarginRight = 25;
            int cnImageButtonMarginBottom = 20;

            var newVINScanButton = new ImageButton(context);
            newVINScanButton.Tag = cnButtonVINScanTag;
            newVINScanButton.Click += btnScanClick;

            //newVINScanButton.SetBackgroundResource(Resource.Drawable.ic_barcode_scan_36dp);
            newVINScanButton.SetBackgroundResource(Resource.Drawable.Button_in_form_action_border);
            newVINScanButton.SetImageDrawable(DroidContext.ApplicationContext.Resources.GetDrawable(Resource.Drawable.ic_barcode_scan_36dp));

            newVINScanButton.SetAdjustViewBounds(true);
            newVINScanButton.SetScaleType(ImageView.ScaleType.FitCenter);

            // initialize our typeface 
            // no text, no need Helper.SetTypefaceForButton(newOCRButton, FontManager.cnMenuPopupButtonExitTypeface, FontManager.cnMenuPopupButtonExitTypefaceSizeSp);

            LinearLayout.LayoutParams layoutParamsButton = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            layoutParamsButton.SetMargins(cnImageButtonMarginLeft, cnImageButtonMarginTop, cnImageButtonMarginRight, cnImageButtonMarginBottom);
            layoutParamsButton.Gravity = GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
            newVINScanButton.LayoutParameters = layoutParamsButton;

            iLinearLayout.AddView(newVINScanButton, layoutParamsButton);
        }

        public void AddGeoCodeButtonToLinearLayout(LinearLayout iLinearLayout)
        {
            int cnImageButtonMarginLeft = 25;
            int cnImageButtonMarginTop = 55;
            int cnImageButtonMarginRight = 25;
            int cnImageButtonMarginBottom = 20;


            var newGeoCodeButton = new ImageButton(context);
            newGeoCodeButton.Tag = cnButtonGeoCodeTag;
            newGeoCodeButton.Click += btnGeoCodeClick;
            
            newGeoCodeButton.SetBackgroundResource(Resource.Drawable.Button_in_form_action_border);
            newGeoCodeButton.SetImageDrawable(DroidContext.ApplicationContext.Resources.GetDrawable(Resource.Drawable.ic_near_me_black_36dp));

            newGeoCodeButton.SetAdjustViewBounds(true);
            newGeoCodeButton.SetScaleType(ImageView.ScaleType.FitCenter);


            // initialize our typeface 
            // no text, no need Helper.SetTypefaceForButton(newOCRButton, FontManager.cnMenuPopupButtonExitTypeface, FontManager.cnMenuPopupButtonExitTypefaceSizeSp);

            LinearLayout.LayoutParams layoutParamsButton = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            layoutParamsButton.SetMargins(cnImageButtonMarginLeft, cnImageButtonMarginTop, cnImageButtonMarginRight, cnImageButtonMarginBottom);
            layoutParamsButton.Gravity = GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
            newGeoCodeButton.LayoutParameters = layoutParamsButton;

            iLinearLayout.AddView(newGeoCodeButton, layoutParamsButton);
        }


		public void UpdateCurrentSignatureImage(Bitmap iSignImage)
        {
            if (iSignImage == null) return;
            sigImg.SetImageBitmap(iSignImage);
            sigImg.SetScaleType(ImageView.ScaleType.FitCenter);
        }
        
        
        public void CreateSignaturePad()
        {
            FragmentManager loFm = ((Activity)context).FragmentManager;
            FragmentTransaction loFt = loFm.BeginTransaction();
            Fragment loPrev = loFm.FindFragmentByTag(Constants.OFFICER_SIGNATURE);
            if (loPrev != null)
            {
                loFt.Remove(loPrev);
            }


            // TDOD - should this be used? Do we need to pop if they use the DONE button instead of BACK ?
            loFt.AddToBackStack(null);

            signaturePickerDialog = new SignaturePickerDialog(context, "Officer Signature",
                                                               "I swear to the accuracy of this notice and am authorized to issue it:",
                                                                Constants.OFFICER_SIGNATURE, this);

            signaturePickerDialog.Show(loFm, Constants.OFFICER_SIGNATURE);
            
        }

		public void btnSignatureViewClick(object sender, EventArgs e)
        {
            CreateSignaturePad();       
        }


        private void ActivateCamera(String action, String fileSuffix)
        {

            try
            {

                string loFilename = Helper.GetLPRImageFilenameFixed(fileSuffix);


                Intent intent = new Intent(action);



                _photoFile = new Java.IO.File(loFilename);  // the path is already included
                intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_photoFile));

                intent.AddFlags( 
                                 ActivityFlags.ClearWhenTaskReset | 
                                 ActivityFlags.GrantWriteUriPermission | 
                                 ActivityFlags.GrantReadUriPermission | 
                                 ActivityFlags.NoHistory | 
                                 ActivityFlags.ReceiverRegisteredOnly | 
                                 ActivityFlags.ResetTaskIfNeeded
                                 );

                //StartActivityForResult(intent, 0);

                //ApplicationContext.StartActivityForResult(intent, 0);

                //MainActivity.this.StartActivityForResult(intent, 0);

                //((MainActivity)Activity).StartActivityForResult(intent, 0);

                //context.StartActivity(intent);  // that works

                // ((Activity)context).StartActivityForResult(intent, 0);  works and sends result back to main activity


                ((Activity)context).StartActivityForResult(intent, Constants.ACTIVITY_REQUEST_CODE_CAPTURE_PHOTO_FOR_LPR);


                //context.StartActivity(intent);


                //StartActivityForResult(intent, Constants.ACTIVITY_REQUEST_CODE_CAPTURE_PHOTO_FOR_LPR);

            }
            catch (Exception exp)
            {
                Console.WriteLine("Failed to launch Camera for ANPR:" + exp.Message);
            }


        }
       
       

        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);



            //// why is this NEVER called?
            switch (requestCode)
            {

                case Constants.ACTIVITY_REQUEST_CODE_CAPTURE_PHOTO_FOR_LPR:
                    {
                        // second time, rename the file and process it
                        string loFilenameForLPRProcessing = Helper.GetLPRImageFilenameWithTimeStamp(Constants.PHOTO_FILE_SUFFIX);

                        try
                        {
                            string loFilename = Helper.GetLPRImageFilenameFixed(Constants.PHOTO_FILE_SUFFIX);

                            if (System.IO.File.Exists(loFilename) == false)
                            {
                                // no file found, they backed out
                                Console.WriteLine("ANPR file not found. LPR halted.");
                                // still have to get us back to a new focus field
                                OnFinishANPRDialog("");
                                return;
                            }


                            System.IO.File.Move(loFilename, loFilenameForLPRProcessing);
                        }
                        catch (Exception exp)
                        {
                            Toast.MakeText(context, "Error occurred renaming LPR file", ToastLength.Long).Show();
                            Console.WriteLine("Failed to rename file  ANPR:" + exp.Message);

                        }

                        try
                        {
                            ShowANPRConfirmationFragmentFormPanel(loFilenameForLPRProcessing);
                        }
                        catch (Exception exp)
                        {
                            ///
                        }
                        break;
                    }


                default:
                    {
                        break;
                    }
            }





            ////// make it available in the gallery
            ////var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            ////Android.Net.Uri contentUri = Android.Net.Uri.FromFile(_photoFile);

            ////mediaScanIntent.SetData(contentUri);
            //////            SendBroadcast(mediaScanIntent);

            //////ShowImage();


            //AutomaticNumberPlateRecognitionServerInterface myANPRWebService = new AutomaticNumberPlateRecognitionServerInterface(this, FragmentManager);
            ////AutomaticNumberPlateRecognitionServerInterface myANPRWebService = new AutomaticNumberPlateRecognitionServerInterface(Activity, FragmentManager);

            //string loImageFile = "";


            //loImageFile = _photoFile.AbsolutePath;


            ////loImageFile = @"/storage/emulated/0/Pictures/civicsmart/S5-197AE_2016_02_06_11_30_31.jpg";

            ////loImageFile = @"/storage/emulated/0/Pictures/civicsmart/20160202_163307.jpg";


            //myANPRWebService.CallAutomaticNumberPlateRecognitionService(loImageFile);
            //{
            //    //we havre read a plate
            //}

        }


        public void ShowANPRConfirmationFragmentFormPanel(string iFilenameForLPRProcessing)
        {

            FragmentManager fm = ((Activity)context).FragmentManager;

            //FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            FragmentTransaction fragmentTransaction = fm.BeginTransaction();

            if (Helper.UseBuiltInCameraWithLPR())
            {
                //Images from built-in camera should be rotated 
                var loRotatedBitmap = BitmapHelpers.RotateBitmapFromFile(iFilenameForLPRProcessing, 90);
                Java.IO.FileOutputStream loOutStream;
                Java.IO.File loImageFile = new Java.IO.File(iFilenameForLPRProcessing);
                using (loOutStream = new Java.IO.FileOutputStream(loImageFile))
                {
                    loOutStream.Write(BitmapHelpers.GetBitmapData(loRotatedBitmap));
                    loOutStream.Close();
                    loOutStream.Flush();
                }
                loRotatedBitmap.Recycle();
                loRotatedBitmap.Dispose();
                loOutStream.Dispose();
            }


            ANPRConfirmResultFragment ANPRDialogConfirm = new ANPRConfirmResultFragment( );
            
            ANPRDialogConfirm.gFilenameForLPRProcessing = iFilenameForLPRProcessing;

            ANPRDialogConfirm.gCallingFragmentTagName = ParentFragment.GetFragmentTagName();
            //ANPRDialogConfirm.SetTargetFragment(this, 99);

            bool mIsLargeLayout = true;

            if (mIsLargeLayout)
            {
                // The device is using a large layout, so show the fragment as a dialog
                ANPRDialogConfirm.Show(fm, Constants.ANPR_CONFIRMATION_FRAGMENT_TAG);
            }
            else
            {
                //// The device is smaller, so show the fragment fullscreen
                //FragmentTransaction transaction = fragmentManager.beginTransaction();
                //// For a little polish, specify a transition animation
                //transaction.setTransition(FragmentTransaction.TRANSIT_FRAGMENT_OPEN);
                //// To make it fullscreen, use the 'content' root view as the container
                //// for the fragment, which is always the root view for the activity
                //transaction.add(android.R.id.content, newFragment)
                //           .addToBackStack(null).commit();

            }

            fragmentTransaction.Commit();
        }

        /// <summary>
        /// Process ActivityResult redirected from MainActivity
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="resultCode"></param>
        /// <param name="data"></param>
        public void OnActivityResultForCommonFragment(int requestCode, Android.App.Result resultCode, Intent data)
        {

            switch (requestCode)
            {

                case Constants.ACTIVITY_REQUEST_CODE_CAPTURE_PHOTO_FOR_LPR:
                    {                       
                        // second time, rename the file and process it
                        string loFilenameForLPRProcessing = Helper.GetLPRImageFilenameWithTimeStamp(Constants.PHOTO_FILE_SUFFIX);

                        try
                        {
                            string loFilename = Helper.GetLPRImageFilenameFixed(Constants.PHOTO_FILE_SUFFIX);

                            if (System.IO.File.Exists(loFilename) == false)
                            {
                                // no file found, they backed out
                                Console.WriteLine("ANPR file not found. LPR halted." );
                                // still have to get us back to a new focus field
                                OnFinishANPRDialog("");
                                return;
                            }


                            System.IO.File.Move(loFilename, loFilenameForLPRProcessing);
                        }
                        catch (Exception exp)
                        {
                            Toast.MakeText(context, "Error occurred renaming LPR file", ToastLength.Long).Show();
                            Console.WriteLine("Failed to rename file  ANPR:" + exp.Message);

                        }

                        try
                        {
                            //Make sure the keyboard is hidden
                            HideKeyBoardFromCurrentView();
                            ShowANPRConfirmationFragmentFormPanel(loFilenameForLPRProcessing);
                        }
                        catch (Exception exp)
                        {
                            ///
                        }
                        break;
                    }


                default:
                    {
                        break;
                    }
            }
        }


        // can sometimes take few secs... ignore double clicks
        static bool gReverseGeoCodeInProcess  = false;

        /// <summary>
        ///  Call location service, reverse geo-code GPS to block street, etc, show dialog to preview, on confirm stuff the locations fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnGeoCodeClick(object sender, EventArgs e)
        {
            
            try
            {
                // already busy?
                if (gReverseGeoCodeInProcess == true)
                {
                    return;
                }

                gReverseGeoCodeInProcess = true;


                //// TODO - put up an interactive dialog with cancel etc. paralell style to LPR service 
                //RunOnUiThread(() =>
                //{
                //    try
                //    {
                //             
                //        // show them that their button click was heard
                //        Toast.MakeText(context, "Reverse GeoCoding...", ToastLength.Long).Show();
                //    }
                //    catch
                //    {
                //    }
                //});



                //Address struct fields:
                //AdminArea = Ontario
                //CountryCode = CA
                //CountryName = Canada
                //FeatureName = 5      (house number)
                //Locality = Richmond Hill    (city)
                //PostalCode = L4E 0M3
                //SubThoroughfare = 5  (looks like house number)
                //Thoroughfare = Military Court     (street name)              

                FragmentManager fm = ((Activity)context).FragmentManager;
                FragmentTransaction fragmentTransaction = fm.BeginTransaction();
                GeoCodeLocationAddressFragment loGeoCodeAddressFrag = new GeoCodeLocationAddressFragment();
                loGeoCodeAddressFrag.gCallingFragmentTagName = ParentFragment.GetFragmentTagName();
                loGeoCodeAddressFrag.Show(fm, Constants.GEOCODE_LOCATION_CONFIRMATION_FRAGMENT_TAG);
            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "btnGeoCodeClick", "btnGeoCodeClick");
                Console.WriteLine("Exception caught in process: {0} {1}", exp, "btnGeoCodeClick");
            }
            finally
            {
                gReverseGeoCodeInProcess = false;
            }

        }




        //public void btnPlateOCRClick(object sender, EventArgs e)
        public async void btnPlateOCRClick(object sender, EventArgs e)
        {

            

            string loFilename = Helper.GetLPRImageFilenameFixed(Constants.PHOTO_FILE_SUFFIX);
            //Make sure that the keyboard is hiden
            HideKeyBoardFromCurrentView();


            try
            {
                // defense against crashes - if we don't make it back, at least the current partial record gets saved and will be availale when they restart
                var success = await SaveCurrentDisplayPageNoValidation();
            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "btnPlateOCRClick", "SaveCurrentDisplayPageNoValidation");
                Console.WriteLine("Exception caught in process: {0} {1}", exp, "btnPlateOCRClick calling SaveCurrentDisplayPageNoValidation");
            }




            if (Helper.UseBuiltInCameraWithLPR())
            {
                // file already there?
                //if (System.IO.File.Exists(loFilename) == false)
                //{
                if (ParentFragment != null)
                {
                    DroidContext.MyFragManager.AddToInternalBackstack(ParentFragment.GetFragmentTagName());
                }

                // first time through, go get the image
                ActivateCamera(MediaStore.ActionImageCapture, Constants.PHOTO_FILE_SUFFIX);

                // come back later
                return;
                //}
            }
            else
            {

                if (ParentFragment != null)
                {
                    DroidContext.MyFragManager.AddToInternalBackstack(ParentFragment.GetFragmentTagName());
                }
                Intent intent = new Intent(context, typeof(Duncan.AI.Droid.Activities.LPRCameraActivity));
                _photoFile = new Java.IO.File(loFilename);  // the path is already included
                intent.PutExtra("LPRCameraActivity.PhotFileName", loFilename);

                ((Activity)context).StartActivityForResult(intent, Constants.ACTIVITY_REQUEST_CODE_CAPTURE_PHOTO_FOR_LPR);

            }

            /*
            else
            {
                // second time, rename the file and process it
                string loFilenameForLPRProcessing = Helper.GetLPRImageFilenameWithTimeStamp(Constants.PHOTO_FILE_SUFFIX);

                try
                {
                    System.IO.File.Move(loFilename, loFilenameForLPRProcessing);
                }
                catch (Exception exp)
                {
                    Toast.MakeText(context, "Error occurred renaming LPR file", ToastLength.Long).Show();
                    Console.WriteLine("Failed to rename file  ANPR:" + exp.Message);

                }

#if _indirect_                                
                AutomaticNumberPlateRecognitionServerInterface myANPRWebService = new AutomaticNumberPlateRecognitionServerInterface(this, FragmentManager);

                // override for test
                //loFilenameForLPRProcessing = @"/storage/emulated/0/Pictures/civicsmart/20160202_163307.jpg"; // bad image test
                loFilenameForLPRProcessing = @"/storage/emulated/0/Pictures/civicsmart/20160202_163313_top_left_good.jpg"; // good image test 

                myANPRWebService.CallAutomaticNumberPlateRecognitionService(loFilenameForLPRProcessing);
                {
                    //we havre read a plate
                }
#endif


                ShowANPRConfirmationFragmentFormPanel(loFilenameForLPRProcessing);
            }
             */ 
 
        }


        // this is being called from OnActivityResult in CommonFragment
        public void OnFinishGeoCodeAddressDialog(string iGeoCodeAddress)
        {
            
            // did they confirm it?
            //bool iGeoCodeAddressConfirmed = (string.IsNullOrEmpty(iGeoCodeAddress) == false);

            // did they keep it?
            //if (iGeoCodeAddressConfirmed == false)
            //{
            //    // nope. dump it and we're done
            //    gLastGeoCodeAddress = null;
            //    return;
            //}


            try
            {
                // put the address data into a single line so we can parse it
                //StringBuilder loFullAddressBuilder = new StringBuilder();
                //for (int loAddressLineIdx = 0; loAddressLineIdx < gLastGeoCodeAddress.MaxAddressLineIndex; loAddressLineIdx++)
                //{
                //    string loAddressLine = gLastGeoCodeAddress.GetAddressLine(loAddressLineIdx);
                //    loFullAddressBuilder.Append(loAddressLine + " ");
                //}

                // try to break into meaning
                string loFullAddress = iGeoCodeAddress; // loFullAddressBuilder.ToString().Trim();
                AddressParser addressParser = new AddressParser();
                AddressParseResult parsed_address = addressParser.ParseAddress(loFullAddress);



                // stuff fields that are familar to us
                EditControlBehavior oneEditControlBehavior;

                // BLOCK.....   
                oneEditControlBehavior = GetEditControlBehaviorByPanelFieldName(AutoISSUE.DBConstants.sqlLocBlockStr);
                // did we get something?
                if (oneEditControlBehavior != null)
                {
                    Helper.UpdateControlWithNewValuePrim(oneEditControlBehavior, parsed_address.Number, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.bfSetHasBeenFocusedFalse);
                }



                // DIRECTION
                string loStreetDirection;

                // direction could be one or the other, OR both
                // TODO - deal with BOTH present
                if (string.IsNullOrEmpty(parsed_address.Predirectional) == false)
                {
                    loStreetDirection = parsed_address.Predirectional;
                }
                else
                {
                    loStreetDirection = parsed_address.Postdirectional;
                }

                oneEditControlBehavior = GetEditControlBehaviorByPanelFieldName(AutoISSUE.DBConstants.sqlLocDirectionStr);
                // did we get something?
                if (oneEditControlBehavior != null)
                {
                    Helper.UpdateControlWithNewValuePrim(oneEditControlBehavior, loStreetDirection, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue);
                }



                // street name
                oneEditControlBehavior = GetEditControlBehaviorByPanelFieldName(AutoISSUE.DBConstants.sqlLocStreetStr);
                // did we get something?
                if (oneEditControlBehavior != null)
                {
                    Helper.UpdateControlWithNewValuePrim(oneEditControlBehavior, parsed_address.Street, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue);
                }


                // street descriptor
                oneEditControlBehavior = GetEditControlBehaviorByPanelFieldName(AutoISSUE.DBConstants.sqlLocDescriptorStr);
                // did we get something?
                if (oneEditControlBehavior != null)
                {
                    Helper.UpdateControlWithNewValuePrim(oneEditControlBehavior, parsed_address.Suffix, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue);
                }


                // city name
                oneEditControlBehavior = GetEditControlBehaviorByPanelFieldName(AutoISSUE.DBConstants.sqlLocCityStr);
                // did we get something?
                if (oneEditControlBehavior != null)
                {
                    Helper.UpdateControlWithNewValuePrim(oneEditControlBehavior, parsed_address.City, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue);
                }


                // state
                oneEditControlBehavior = GetEditControlBehaviorByPanelFieldName(AutoISSUE.DBConstants.sqlLocStateStr);
                // did we get something?
                if (oneEditControlBehavior != null)
                {
                    Helper.UpdateControlWithNewValuePrim(oneEditControlBehavior, parsed_address.Street, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue);
                }


                //postal code
                oneEditControlBehavior = GetEditControlBehaviorByPanelFieldName(AutoISSUE.DBConstants.sqlLocZipStr);
                // did we get something?
                if (oneEditControlBehavior != null)
                {
                    Helper.UpdateControlWithNewValuePrim(oneEditControlBehavior, parsed_address.Zip, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue);
                }








                /////////////////


                // fields have been stuffed, where to set focus?

                // working backwards based on tradition....

                // find the "last" field of the location section
                
                PanelField NextCfgCtrl = GetCfgCtrlByName(AutoISSUE.DBConstants.sqlLocZipStr);
                if (NextCfgCtrl == null)
                {
                    NextCfgCtrl = GetCfgCtrlByName(AutoISSUE.DBConstants.sqlLocCityStr);
                }
                if (NextCfgCtrl == null)
                {
                    NextCfgCtrl = GetCfgCtrlByName(AutoISSUE.DBConstants.sqlLocCountyStr);
                }
                if (NextCfgCtrl == null)
                {
                    NextCfgCtrl = GetCfgCtrlByName(AutoISSUE.DBConstants.sqlLocCrossStreet2Str);
                }
                if (NextCfgCtrl == null)
                {
                    NextCfgCtrl = GetCfgCtrlByName(AutoISSUE.DBConstants.sqlLocCrossStreet1Str);
                }
                if (NextCfgCtrl == null)
                {
                    NextCfgCtrl = GetCfgCtrlByName(AutoISSUE.DBConstants.sqlLocDescriptorStr);
                }
                if (NextCfgCtrl == null)
                {
                    NextCfgCtrl = GetCfgCtrlByName(AutoISSUE.DBConstants.sqlLocStreetStr);
                }


                // did we find someplace to go next?
                if (NextCfgCtrl != null)
                {
                    // get next editable control in entry order
                    NextCfgCtrl = GetNextCfgCtrl(NextCfgCtrl.fEditFieldDef, true);

                    // have a skip to field?
                    if (NextCfgCtrl != null)
                    {
                        View uiComponentNext = null;
                        try
                        {
                            // resolve to the associate view component -TODO shouldn't this be a property of the panelfield for instant access? Or is it disposable and may not be static?
                            uiComponentNext = PanelRootPageLinearLayout.FindViewWithTag(NextCfgCtrl.Name);
                        }
                        catch (Exception exp)
                        {
                            uiComponentNext = null;
                            Console.WriteLine("Failed to find field in FindViewWithTag:" + exp.Message);
                        }


                        if (uiComponentNext != null)
                        {
                            ScrollIntoViewAndSetFocus(uiComponentNext, NextCfgCtrl.fEditFieldDef, false, false);
                        }
                    }
                }

            }
            catch (System.Exception exp)
            {
                System.Console.WriteLine("Failed to populate GeoCode Address:" + exp.Message);
            }

        }


        // this is being called from OnActivityResult in CommonFragment
        public void OnFinishANPRDialog(string iConfirmedPlateValue)  // later add state
        {
            // trim esxess of trailing spacess
            iConfirmedPlateValue = iConfirmedPlateValue.Trim();


            // if plate not passed, we will leave the focus on the plate field so they can key it in
            bool loPlateGetsUIFocus = (iConfirmedPlateValue.Length == 0);

            // do we have anything to used?
            bool loPlateHasConfirmedValue = (iConfirmedPlateValue.Length > 0);

            // TODO - this should fire the exit events, for TER_HotSheetFilter etc
            // for now, go back to the field so they do fire when they leave
            loPlateGetsUIFocus = true;


            // if the plate is not passed, we will not overwrite what might already be there
            if (iConfirmedPlateValue.Length > 0)
            {

                // find the plate and update it
                EditControlBehavior oneEditControlBehavior = GetEditControlBehaviorByPanelFieldName(AutoISSUE.DBConstants.sqlVehLicNoStr);
                // did we get something?
                if (oneEditControlBehavior != null)
                {
                    Helper.UpdateControlWithNewValuePrim(oneEditControlBehavior, iConfirmedPlateValue, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue);
                }


                //// go find the plate
                //string loPanelFieldName = AutoISSUE.DBConstants.sqlVehLicNoStr;

                //View uiComponent = PanelRootPageLinearLayout.FindViewWithTag(loPanelFieldName);
                //if (uiComponent != null)
                //{
                //    if (uiComponent is CustomEditText)
                //    {
                //        CustomEditText customView = (CustomEditText)PanelRootPageLinearLayout.FindViewWithTag(loPanelFieldName);
                //        if (customView != null)
                //        {


                //            //customView.ProcessRestrictions(EditRestrictionConsts.dneFormInit);

                //            // TODO - there should be a PrepareForEdit to take care of this kind of stuff
                //            customView.HasBeenFocused = true;
                //            //customView.FormStatus = "Processed";

                //            customView.Text = iConfirmedPlateValue;

                //            //customView.SetText(iConfirmedPlateValue, TextView.BufferType.Normal);


                //            //if (customView.Behavior != null)
                //            //{
                //            //    customView.Behavior.SetEditBuffer(iConfirmedPlateValue);
                //            //}

                //        }
                //    }
                //}
            }



            // find the plate
            PanelField NextCfgCtrl = GetCfgCtrlByName(AutoISSUE.DBConstants.sqlVehLicNoStr);

            // shoud we go to plate or the field after?
            if (loPlateGetsUIFocus == false)
            {
                if (NextCfgCtrl != null)
                {
                    // get next editable control in entry order
                    NextCfgCtrl = GetNextCfgCtrl(NextCfgCtrl.fEditFieldDef, true);
                }
            }

            // have a skip to field?
            if (NextCfgCtrl != null)
            {
                View uiComponentNext = null;
                try
                {
                    // resolve to the associate view component -TODO shouldn't this be a property of the panelfield for instant access? Or is it disposable and may not be static?
                    uiComponentNext = PanelRootPageLinearLayout.FindViewWithTag(NextCfgCtrl.Name);
                }
                catch (Exception exp)
                {
                    uiComponentNext = null;
                    Console.WriteLine("Failed to find field in FindViewWithTag:" + exp.Message);
                }


                if (uiComponentNext != null)
                {
                    //ScrollIntoViewAndSetFocus(uiComponentNext, NextCfgCtrl.fEditFieldDef, false, true);
                    ScrollIntoViewAndSetFocus(uiComponentNext, NextCfgCtrl.fEditFieldDef, false, false);

                    // and we had a plate value?
                    if (loPlateHasConfirmedValue == true)
                    {
                        if (uiComponentNext is CustomEditText)
                        {
                            CustomEditText oneCustomEditText = (CustomEditText)uiComponentNext;
                            if (oneCustomEditText.BehaviorAndroid != null)
                            {
                                oneCustomEditText.BehaviorAndroid.OkToExit(true);
                            }
                        }

                        // now tab out of it to fire events, searches etc
                        //btnNextClick(null, null);

                    }
                }
            }


            return;




            // Windows CE Mobile code for reference
#if _plateOCR_
       


	if (iSender == fBtnReadReino)
	{
 

        //#define MODE_OCR_3M_STR		    TEXT("OCR_3M")
        //#define MODE_OCR_ALPR_STR 	    TEXT("OCR_ALPR")
        //#define OCR_PLATE_NUMBER_KEY	TEXT("INTERNAL_REG\\INTERNAL")
        //#define OCR_PLATE_NUMBER_VAL	TEXT("PlateNumber")

		//Check if the button for the plate OCR or not? 
		if (strstr(fBtnReadReino->GetCaption(), "Plate OCR"))
		{
			//First we should make sure that the LPR lib is defined
			char loLPRLib[32] = {0};
			strcpy(loLPRLib, GetRegistryValue( SECTION_ISSUE_AP, LPR_IMAGING_LIBRARY, LPR_IMAGING_LIBRARY_DEFAULT));
			//See if a Lib is defined or not
			if(strcmp(loLPRLib, LPR_IMAGING_LIBRARY_NONE) == 0)
			{
				//LPR lib is not defined, exit now
				return 0;
			}

			
			TTStringList* loImagesList;
			loImagesList = new TTStringList();
			
			if(LaunchCamera_PlateOCR(loImagesList) > 0)  //Do we have at least one image?
			{
				//Yes we have
				//ShowMessage("Plate OCR",loImagesList->GetString(loImagesList->fItemCnt-1));
						
				//Our watermark tool uses wide char
				wchar_t loWCFileName[MAX_PATH] = {0};
				char *loFileName = loImagesList->GetString(loImagesList->fItemCnt-1);
				mbstowcs(loWCFileName, loFileName, strlen(loFileName) + 1);
				//Now we need to create the command line argument
				wchar_t loCmdLineStr[MAX_PATH]= {0};
				//Build the command line for calling the watermark app
				if(strcmp(loLPRLib, LPR_IMAGING_LIBRARY_3M) == 0)
				{
					swprintf(loCmdLineStr, TEXT("\"%s\" %s %s"),loWCFileName, TEXT("--"), MODE_OCR_3M_STR); // File path command line params should be enclosed in double quotes 
				}else if(strcmp(loLPRLib, LPR_IMAGING_LIBRARY_ALPR) == 0){
					swprintf(loCmdLineStr, TEXT("\"%s\" %s %s"),loWCFileName, TEXT("--"), MODE_OCR_ALPR_STR); // File path command line params should be enclosed in double quotes 
				}else{
					//Somwthing we don't support, just use 3M lib for now
					swprintf(loCmdLineStr, TEXT("\"%s\" %s %s"),loWCFileName, TEXT("--"), MODE_OCR_3M_STR); // File path command line params should be enclosed in double quotes 
				}
				//Construct the path of the watermark application
				wchar_t loWaterMarkApp[MAX_PATH] = {0};
    			swprintf(loWaterMarkApp, TEXT("%sWatermark.exe"), TEXT("\\Windows\\"));  // For best performance, launch from Windows instead of flash area
				//MessageBox(glCETopWindow, loWaterMarkApp, TEXT("WaterMark App path"),0);
				//We are now ready to init the watermark tool
				PROCESS_INFORMATION loProcessInfo;
				memset( &loProcessInfo, 0, sizeof( loProcessInfo ));
				//MessageBox( glCETopWindow,loCmdLineStr, TEXT("Cmd Line Str"), 0);
				//Now we are ready to launch our external tool to add the watermark
				if(CreateProcess( loWaterMarkApp, loCmdLineStr, NULL, NULL, NULL, CREATE_NEW_CONSOLE, NULL, NULL, NULL, &loProcessInfo ))
				{		
					//Wait for the process to complete
					DWORD loStatus = STILL_ACTIVE;
					while (loStatus == STILL_ACTIVE)
					{
						Sleep(100); //give it some time to finish
						GetExitCodeProcess(loProcessInfo.hProcess, &loStatus);
					}
					//Make sure the process is terminated
					TerminateProcess(loProcessInfo.hProcess, 1);
					// Close thread and process handles returned
					CloseHandle(loProcessInfo.hThread);
					CloseHandle(loProcessInfo.hProcess);
		
					//ShowMessage("Watermark is added", "AddWatermarkToPicture");
					SetForegroundWindow(glCETopWindow);		
				}
			}
			//Clean up before exit
			loImagesList->Clear();
			delete loImagesList;
			//Now we should have the LIC plate number in the windows registry
			HKEY	loKey = NULL;
  			DWORD loStatus, loType, loSize;
			wchar_t loPlateNumberW[20] = {0};
  			loStatus = RegOpenKeyEx( HKEY_LOCAL_MACHINE, OCR_PLATE_NUMBER_KEY, 0, 0, &loKey);
  			if(loStatus == ERROR_SUCCESS) 
  			{
				loType = REG_SZ;  					
			    loSize = sizeof(loPlateNumberW);
				loStatus = RegQueryValueEx( loKey, OCR_PLATE_NUMBER_VAL, NULL, &loType, (LPBYTE)loPlateNumberW, &loSize);
				RegCloseKey( loKey ); //No need to keep it.
  				if(loStatus == ERROR_SUCCESS)
				{
					//MessageBox( glCETopWindow,loPlateNumberW, TEXT("LIC Plate Number"), 0);
					//We got the number, do the action
					if(wcslen(loPlateNumberW) > 0)
					{
						char loPlateNumber[20] = {0};
						wcstombs( loPlateNumber, loPlateNumberW, wcslen(loPlateNumberW)+1);
						//ShowMessage("Lic Plate Number:",loPlateNumber);
						
						//Now we are ready to execute the required action, we will get the action from the registry
						char loAction[64] = {0};
						strcpy(loAction, GetRegistryValue( SECTION_ISSUE_AP, LPR_IMAGING_ACTION, LPR_IMAGING_ACTION_DEFAULT));

						if(strcmp(loAction, LPR_IMAGING_ACTION_NONE) == 0)
						{
							//nothing to do, just exit now.
							//ShowMessage("Action: None", loPlateNumber);
							return 0;
						}else if(strcmp(loAction, LPR_IMAGING_ACTION_LICPLATE_STUFF) == 0){							
							UpdateVehLicNoField(loPlateNumber);
							return 0; //done
						}else if(strcmp(loAction, LPR_IMAGING_ACTION_DIALOG_COMPARE) == 0){
							//First get the current Lic plate number from the edit field
							char loCurrentLicNumber[32] = {0};
							TTEdit *loEdit;
							if ( (loEdit = (TTEdit *)this->FindControlByName( VehLicNo ) ) != 0 )
							{								
								loEdit->GetEditBuffer(loCurrentLicNumber);
							}
							//See if there is vaild Lic plate number in the field or not
							if(strlen(loCurrentLicNumber) > 0)
							{
								//See if the two lic numbers are different or not
								if (strcmp(loPlateNumber, loCurrentLicNumber) != 0) //If the 2 numbers are different then tell the user to pick one
								{
									TLPRActionDialogCompare *loDialogCompare = new TLPRActionDialogCompare(this, loPlateNumber, loCurrentLicNumber);
									loDialogCompare->FormEdit( femNewEntry, "" );
									delete loDialogCompare;
								}else{ //The 2 numbers are same, tell the user
									if(GetRegistryValueAsInt( SECTION_ISSUE_AP, LPR_IMAGING_ACTION_DIALOG_COMPARE_CONFIRM_ALWAYS, LPR_IMAGING_ACTION_DIALOG_COMPARE_CONFIRM_ALWAYS_DEFAULT))
									{

        								TMsgForm *loMsgForm;
        								loMsgForm = new TMsgForm( 0, 0, "OK", 'O', "", 0 );
        								loMsgForm->SetCaption( "License Plate Number" );
        								loMsgForm->SetMsgAndPaint( "OCR Plate Matches",  loPlateNumber);
        								loMsgForm->FormShow( 1 );
        								delete loMsgForm;
									}
								}
							}else{
								//No Lic plate number in the field, just copy the one we just got
								UpdateVehLicNoField(loPlateNumber);
							}
							return 0; //done
						} else {
							return 0; //do nothing
						}
					}

				}  				
			}  
			return 0;
		}
#endif
        }


        public void OnAutoISSUEToolbarClick(object sender, Toolbar.MenuItemClickEventArgs e)
        {
            if (e.Item != null)
            {

                switch ( e.Item.ItemId )
                {
                    case Resource.Id.ai_toolbar_listnext :
                        {
                            btnNextListItemClick(sender, null);
                            break;
                        }

                    //case Resource.Id.ai_toolbar_listpopup:
                    //    {
                    //        btnListItemDisplayPopUpClick(sender, null);
                    //        break;
                    //    }

                    case Resource.Id.ai_toolbar_listprev:
                        {
                            btnPreviousListItemClick(sender, null);
                            break;
                        }

                    case Resource.Id.ai_toolbar_tabnext:
                        {
                            btnNextClick(sender, null);
                            break;
                        }

                    case  Resource.Id.ai_toolbar_tabprev:
                        {
                            btnPreviousClick(sender, null);
                            break;
                        }


                    //case Resource.Id.ai_toolbar_pageprev:
                    //    {
                    //        btnPrevPageClick(sender, null);
                    //        break;
                    //    }

                    case Resource.Id.ai_toolbar_pagenext:
                        {
                            btnNextPageClick(sender, null);
                            break;
                        }

                    case Resource.Id.ai_toolbar_clear:
                        {
                            // clear the field contents
                            btnClearFieldClick(sender, null);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }


            }
        }


        public void btnIssueFormNavigationClick(object sender, EventArgs e)
        {
            DroidContext.mainActivity.MenuPopUp_IssueFormSelection();
        }



        /// <summary>
        /// Common handler for NEXT FIELD and NEXT PAGE actions
        /// </summary>
        /// <param name="iNextPageClicked"></param>
        private void btnNextActionPrim(object sender, EventArgs e, bool iNextPageClicked)
        {
            Boolean loPassedValidation = false;

            // use the tab forward to call the form validation
            bool loNextFieldFound = TabForward(thisStruct, PanelRootPageLinearLayout, 0, context, metrics, ref loPassedValidation);

            // next field clicked?
            if (iNextPageClicked == false)
            {
                // found someplace to go?
                if (loNextFieldFound == true)
                {
                    // go there, we're done 
                    return;
                }
            }


            // so we are at the last field... did the form pass validation?
            if (loPassedValidation == false)
            {
                // failed validation, we're done
                return;
            }


            // standard entry forms have a single panel
            int loLastEntryPanelIndex = thisStruct.Panels.Count - 1;

            // at the last panel before preview?
            if (PanelPageCurrentDisplayedIndex >= loLastEntryPanelIndex)
            {
                // at the end with no more pages to go, re-firect to submit.save button
                if (ParentFragment != null)
                {
                    ParentFragment.btnDoneClick(sender, e);
                }

                // done here
                return;
            }

            // try to move the next page
            ChangeCurrentDisplayPage(1);
        }


        public void btnNextClick(object sender, EventArgs e)
        {
            btnNextActionPrim(sender, e, false);
        }

        public void btnNextPageClick(object sender, EventArgs e)
        {
            btnNextActionPrim(sender, e, true);
        }


        public void btnPrevPageClick(object sender, EventArgs e)
        {
            // the toolbar buttons should be disabled at the first/last panels
            if (PanelPageCurrentDisplayedIndex <= 0)
            {
                // no more to go
                return;
            }

            // try to move the previous page
            ChangeCurrentDisplayPage(-1);
        }


        public void btnPreviousClick(object sender, EventArgs e)
        {
            // try to move the next field on current panel
            if (TabBackward(thisStruct, PanelRootPageLinearLayout, 0, context, metrics) == false)
            {
                // the toolbar buttons should be disabled at the first/last panels
                if (PanelPageCurrentDisplayedIndex <= 0)
                {
                    // no more to go
                    return;
                }
                
                // nothing on current panel, move to the previous panel
              ChangeCurrentDisplayPage(-1);
            }


        }


        public void btnPreviousListItemClick(object sender, EventArgs e)
        {
            //disable the buttons to prevent the double clicking issue from happening.
            //var button = (Button)sender;



            if (
                 (thisStruct.Panels != null) &&
                 (thisStruct.Panels.Count > PanelPageCurrentDisplayedIndex) &&
                 (thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields != null)
            )
            {

                Panel loCurrentPanel = thisStruct.Panels[PanelPageCurrentDisplayedIndex];
                object uiComponent = loCurrentPanel.FocusedViewCurrent;

                if (uiComponent != null)
                {
                    if (uiComponent is CustomEditText)
                    {
                        var customEditText = (CustomEditText)uiComponent;
                        if (customEditText != null)
                        {

                            if (customEditText.BehaviorAndroid != null)
                            {
                                if (customEditText.BehaviorAndroid.GetFieldType() == EditEnumerations.EditFieldType.efDate)
                                {
                                    customEditText.SpinnerEditTextViewDateSpin(false);
                                }
                            }

                        }
                    }
                    else if (uiComponent is CustomAutoTextView)
                    {

                        var customView = (CustomAutoTextView)uiComponent;
                        if (customView != null)
                        {
                            var behavior = customView.BehaviorAndroid;

                            customView.SpinnerAutoTextViewSpin(false); // backward

                            //var valid = ValidateBehavior(Struct.Name, delta, panelField, behavior, customView, customView.Text);
                            //if (!valid)
                            //    return false;
                        }

                    }
                    else if (uiComponent is CustomSpinner)
                    {

                        //var customView = (CustomSpinner)layout.FindViewWithTag(loPanelFieldName);
                        var customView = (CustomSpinner)uiComponent;
                        if (customView != null)
                        {
                            var behavior = customView.BehaviorAndroid;


                            customView.SpinnerSpin( false ); // backward
                            
                            //var valid = ValidateBehavior(Struct.Name, delta, panelField, behavior, customView, customView.GetValue(panelField.OptionsList));
                            //if (!valid)
                            //    return false;
                        }
                    }
                }

            }

        }


        public void btnNextListItemClick(object sender, EventArgs e)
        {
            //disable the buttons to prevent the double clicking issue from happening.
            //var button = (Button)sender;



            if (
                 (thisStruct.Panels != null) &&
                 (thisStruct.Panels.Count > PanelPageCurrentDisplayedIndex) &&
                 (thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields != null)
            )
            {
                Panel loCurrentPanel = thisStruct.Panels[PanelPageCurrentDisplayedIndex];
                object uiComponent = loCurrentPanel.FocusedViewCurrent;


                if (uiComponent != null)
                {
                    if (uiComponent is CustomEditText)
                    {
                        var customEditText = (CustomEditText)uiComponent;
                        if (customEditText != null)
                        {

                            if (customEditText.BehaviorAndroid != null)
                            {
                                if (customEditText.BehaviorAndroid.GetFieldType() == EditEnumerations.EditFieldType.efDate)
                                {
                                    customEditText.SpinnerEditTextViewDateSpin(true);
                                }
                            }

                        }
                    }
                    else if (uiComponent is CustomAutoTextView)
                    {

                        //var customView = (CustomAutoTextView)layout.FindViewWithTag(loPanelFieldName);
                        var customView = (CustomAutoTextView)uiComponent;
                        if (customView != null)
                        {
                            var behavior = customView.BehaviorAndroid;


                            customView.SpinnerAutoTextViewSpin(true); // forward

                            //var valid = ValidateBehavior(Struct.Name, delta, panelField, behavior, customView, customView.Text);
                            //if (!valid)
                            //    return false;
                        }

                    }
                    else if (uiComponent is CustomSpinner)
                    {

                        //var customView = (CustomSpinner)layout.FindViewWithTag(loPanelFieldName);
                        var customView = (CustomSpinner)uiComponent;
                        if (customView != null)
                        {
                            var behavior = customView.BehaviorAndroid;


                            customView.SpinnerSpin(true); // forward

                            //var valid = ValidateBehavior(Struct.Name, delta, panelField, behavior, customView, customView.GetValue(panelField.OptionsList));
                            //if (!valid)
                            //    return false;
                        }
                    }
                }






            }

            //button.Enabled = false;
            //ChangeLayout(-1);
        }


        public void btnClearFieldClick(object sender, EventArgs e)
        {

            if (
                 (thisStruct.Panels != null) &&
                 (thisStruct.Panels.Count > PanelPageCurrentDisplayedIndex) &&
                 (thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields != null)
            )
            {
                Panel loCurrentPanel = thisStruct.Panels[PanelPageCurrentDisplayedIndex];
                object uiComponent = loCurrentPanel.FocusedViewCurrent;


                if (uiComponent != null)
                {
                    if (uiComponent is CustomEditText)
                    {
                        CustomEditText customEditText = (CustomEditText)uiComponent;
                        if (customEditText.BehaviorAndroid != null)
                        {
                            Helper.UpdateControlWithNewValuePrim(customEditText.BehaviorAndroid, "", EditEnumerations.IgnoreEventsType.ieIgnoreEventsFalse, EditEnumerations.SetHasBeenFocusedType.bfSetHasBeenFocusedFalse);
                        }
                    }
                    else if (uiComponent is CustomAutoTextView)
                    {
                        CustomAutoTextView customView = (CustomAutoTextView)uiComponent;
                        if (customView.BehaviorAndroid != null)
                        {
                            // set new list first
                            string[] loRefreshedList = (new ListSupport()).GetListDataByTableColumnName(customView.BehaviorAndroid.PanelField.OptionsList.ListName, Helper.ConcatColumns(customView.BehaviorAndroid.PanelField.OptionsList.Columns));
                            Helper.UpdateControlWithNewListPrim((customView.BehaviorAndroid.PanelField.uiComponent as CustomAutoTextView), loRefreshedList, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.bfSetHasBeenFocusedFalse);
                            //customView.ClearField();

                            // now clear the field
                            Helper.UpdateControlWithNewValuePrim(customView.BehaviorAndroid, "", EditEnumerations.IgnoreEventsType.ieIgnoreEventsFalse, EditEnumerations.SetHasBeenFocusedType.bfSetHasBeenFocusedFalse);
                        }
                    }




                    return;
                }



                if (uiComponent != null)
                {
                    if (uiComponent is CustomEditText)
                    {
                        CustomEditText customEditText = (CustomEditText)uiComponent;
                        if (customEditText != null)
                        {
                            // clear out the field completely
                            customEditText.SetText("", TextView.BufferType.Normal);

                            if (customEditText.BehaviorAndroid != null)
                            {
                                customEditText.BehaviorAndroid.SetEditBuffer("");
                            }

                        }
                    }
                    else if (uiComponent is CustomAutoTextView)
                    {
                        CustomAutoTextView customView = (CustomAutoTextView)uiComponent;
                        if (customView != null)
                        {
                            // clear out the field completely
                            customView.SetListItemDataByText("");
                            customView.SetText("", false);

                            if (customView.BehaviorAndroid != null)
                            {
                                customView.BehaviorAndroid.SetEditBuffer("");
                                customView.BehaviorAndroid.ListItemIndex = -1;
                            }

                        }

                    }
                    else if (uiComponent is CustomSpinner)
                    {

                        // can't clear spinners... thats why we dont use them anymore
                        //var customView = (CustomSpinner)uiComponent;
                        //if (customView != null)
                        //{
                        //    var behavior = customView.Behavior;

                        //}
                    }
                }






            }

        }


        public void btnListItemDisplayPopUpClick(object sender, EventArgs e)
        {
            //disable the buttons to prevent the double clicking issue from happening.
            //var button = (Button)sender;



            if (
                 (thisStruct.Panels != null) &&
                 (thisStruct.Panels.Count > PanelPageCurrentDisplayedIndex) &&
                 (thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields != null)
            )
            {
                Panel loCurrentPanel = thisStruct.Panels[PanelPageCurrentDisplayedIndex];
                object uiComponent = loCurrentPanel.FocusedViewCurrent;


                if (uiComponent != null)
                {
                    if (uiComponent is CustomEditText)
                    {
                        var customEditText = (CustomEditText)uiComponent;
                        if (customEditText != null)
                        {

                            if (customEditText.BehaviorAndroid != null)
                            {
                                EditEnumerations.EditFieldType loFieldType = customEditText.BehaviorAndroid.GetFieldType();

                                switch (loFieldType)
                                {
                                    case EditEnumerations.EditFieldType.efDate:
                                    case EditEnumerations.EditFieldType.efTime:
                                        {
                                            customEditText.PerformClick();
                                            break;
                                        }
                                    default:
                                        {
                                            // nothing to do for other field types
                                            break;
                                        }
                                }
                            }

                        }
                    }
                    else if (uiComponent is CustomAutoTextView)
                    {
                        //var customView = (CustomAutoTextView)layout.FindViewWithTag(loPanelFieldName);
                        CustomAutoTextView customView = (CustomAutoTextView)uiComponent;
                        if (customView != null)
                        {
                            var behavior = customView.BehaviorAndroid;

                            //// clear the existing text so you will see the whole list
                            //customView.SetText("", false);
                            //// make the list drop down
                            //customView.ShowDropDown();  


                            customView.ShowDropDownAutoTextViewCustom();

                            //var valid = ValidateBehavior(Struct.Name, delta, panelField, behavior, customView, customView.Text);
                            //if (!valid)
                            //    return false;
                        }

                    }
                    else if (uiComponent is CustomSpinner)
                    {

                        //var customView = (CustomSpinner)layout.FindViewWithTag(loPanelFieldName);
                        var customView = (CustomSpinner)uiComponent;
                        if (customView != null)
                        {
                            var behavior = customView.BehaviorAndroid;


                            customView.PerformClick(); // make the list pop up


                            //customView.SpinnerSpin(true); // forward

                            //var valid = ValidateBehavior(Struct.Name, delta, panelField, behavior, customView, customView.GetValue(panelField.OptionsList));
                            //if (!valid)
                            //    return false;
                        }
                    }
                }






            }

            //button.Enabled = false;
            //ChangeLayout(-1);
        }


        public async void ChangeCurrentDisplayPage(Int32 delta)
        {

            try
            {

                //save the current layout fields to the db
                var success = await SaveCurrentDisplayPage(thisStruct, PanelRootPageLinearLayout, PanelRootPageScrollView, delta, context, metrics);
                if (success)
                {

                    // hide the keyboard if it was open
                    object uiComponent = thisStruct.Panels[PanelPageCurrentDisplayedIndex].FocusedViewCurrent;
                    if (uiComponent != null)
                    {
                        if (uiComponent is View)
                        {
                            Helper.HideKeyboard((View)uiComponent);
                        }
                    }

                    CreateOrShowDisplayPage(delta);
                }
                else
                {
                    //find the buttons and re-enable them.
                    var btnPrevious = PanelRootPageLinearLayout.FindViewWithTag("btnPrevious");

                    if (btnPrevious != null)
                    {
                        (btnPrevious).Enabled = true;
                    }

                    var btnNext = PanelRootPageLinearLayout.FindViewWithTag("btnNext");
                    if (btnNext != null)
                    {
                        btnNext.Enabled = true;
                    }
                }

                PanelPageDelta = delta;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception ChangeCurrentDisplayPage: {0}", e.Source);
                Console.WriteLine("e.Message: {0}", e.Message);
            }

        }


#if _original_
        //Common to be invoked in both previous and next buttons
		public async Task<bool> UpdateStruct()
        {
			string id = null;
			CommonDTO commonDTO = null;
            string tableName = thisStruct.Name;

			if (thisStruct.Type.Equals (Constants.STRUCT_TYPE_CITE)) 
            {				
                commonDTO = ParkingSequenceADO.GetInUseSequenceId(thisStruct.SequenceName);
            }
            else if (thisStruct.Type.Equals(Constants.STRUCT_TYPE_CHALKING))
            {
                id = thisStruct.chalkRowId;
            }

		    //todo - review this,i put the null check in, but need ot understand why this was working previously.
            //the logic changed for this when Murthy checked in his sprint fixes (see comments on checkin). 
            // This is currently not working they way it used to and breaks the current tire chalking screens
            if (thisStruct.Type.Equals(Constants.STRUCT_TYPE_ACTIVITYLOG))
            {
                //TODO: 
            } 
                //todo - caleb. its not getting the in use seq id, so its re-creating it and using the same one (insert row.)
                //need ot do a check to see if there s already a parking ticket with the sequence id and if not
            else if (commonDTO != null && commonDTO.seqId == null)
            {
				ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
				ISharedPreferencesEditor editor = prefs.Edit();
				
				if (thisStruct.Type.Equals (Constants.STRUCT_TYPE_CITE)) 
                {
                    commonDTO = ParkingSequenceADO.GetSequenceId(thisStruct.SequenceName);
					thisStruct.sequenceId = commonDTO.seqId;
					editor.PutString(Constants.ISSUENO_COLUMN, commonDTO.seqId);
                    //editor.PutString(Constants.SRCISSUENOPFX_COLUMN, commonDTO.prefix);
                    //editor.PutString(Constants.SRCISSUENOSFX_COLUMN, commonDTO.suffix);
					editor.Apply();
					//thisStruct.prefix = commonDTO.prefix;
				}
				var commonADO = new CommonADO();
				 //Task<bool> result  = commonADO.InsertRow(thisStruct, context, tableName, commonDTO);
                 Task<bool> result = commonADO.InsertRow(thisStruct, context, tableName, commonDTO._masterKey);

                bool resultCont = await result;
              
                if (thisStruct.Name.Equals (Constants.STRUCT_NAME_CHALKING)) 
                {
					thisStruct.chalkRowId = commonADO.GetRowId (Constants.MARKMODE_TABLE);
				}
                return resultCont;
			}
            else
            {
				var commonADO = new CommonADO();
                //todo - added a null ref check to prevent it from breaking when the chalking fragment is instantiated. Still need to determine better loginc flow for this portion of the app.
               
                //only od this if we have a commonAdo
                if (thisStruct.sequenceId == null && commonDTO != null)
                {
                    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
                    ISharedPreferencesEditor editor = prefs.Edit();
                    thisStruct.sequenceId = commonDTO.seqId;
                    editor.PutString(Constants.ISSUENO_COLUMN, commonDTO.seqId);
                    editor.Apply();
                    //thisStruct.prefix = commonDTO.prefix;
                    commonADO.LoadFromDB(thisStruct, tableName);
                }

                Task<bool> result = commonADO.UpdateRow(thisStruct, Constants.STATUS_INPROCESS, Constants.WS_STATUS_EMPTY, context, tableName);
                bool resultCont = await result;
			    return resultCont;

			}
		    return true;
        }

#endif

//#if _newonme_    
        //Common to be invoked in both previous and next buttons
        public async Task<bool> UpdateStruct()
        {

            // first time through when fragment is created
            //
            // then it is called when panel pages changes or entry ends
            //
            // first time through, the citation row exists, but isn't used yet - its only been pre-allocated
            // next 
            //
            // soo first time through, we must find the row (for structs with sequence) or create the row (for structs witout sequence)
            // 
            // subsequent times though, we just update the row with the row ID we have


            // move up to add location info to any kind of form that includes the lat/long fields
            // TODO this only needs to be on the final save...
            UpdateLocationInfo();



            CommonDTO commonDTO = null;
            CommonADO commonADO = new CommonADO();


            // first - if we have a rowId, update it and we're done
            if (string.IsNullOrEmpty(thisStruct._rowID) == false)
            {
                Task<bool> result = commonADO.UpdateRow(thisStruct, Constants.STATUS_INPROCESS, Constants.WS_STATUS_EMPTY, context, thisStruct._TIssStruct.MainTable.Name);
                bool resultCont = await result;
                return resultCont;
            }


            // so from here, we don't already have a rowID to update
            // so we either need to
            // a) for sequence structs, we have to find the row pre-allocated with sequence number
            // or
            // b) for non-sequence structs, we may have to insert the row and retain the rowId for future updates



            // sequence structs are simplest - handle them first
            if (string.IsNullOrEmpty(thisStruct._TIssStruct.Sequence) == false)
            {

                // first - is there ALREADY a row in the DB from an unfinished record?
                commonDTO = ParkingSequenceADO.GetInUseSequenceId(thisStruct.SequenceName);
                if (String.IsNullOrEmpty( commonDTO.seqId ) == false)
                {
                    // try to get that row
                    thisStruct._rowID = ParkingSequenceADO.GetRowIdForInUseSequenceId(thisStruct._TIssStruct.MainTable.Name, commonDTO.seqId);

                    thisStruct.sequenceId = commonDTO.seqId;
                    thisStruct.prefix = commonDTO.sqlIssueNumberPrefixStr;
                    thisStruct.suffix = commonDTO.sqlIssueNumberSuffixStr;

                    if (String.IsNullOrEmpty(thisStruct._rowID) == false)
                    {
                        // get that row - when do we load from existing/restored
                        commonADO.LoadStructureRecordFromDBByRowId(thisStruct);

                        // we're done
                        return true;
                    }
                }




                // get the the next sequence info for this structure
                commonDTO = ParkingSequenceADO.GetSequenceId(thisStruct._TIssStruct.Sequence);
                if (commonDTO == null)
                {
                    // fail! no more sequence numbers? bad allocation?
                    return false;
                }

                // update the sequence info in the fragment
                thisStruct.sequenceId = commonDTO.seqId;
                thisStruct.prefix = commonDTO.sqlIssueNumberPrefixStr;
                thisStruct.suffix = commonDTO.sqlIssueNumberSuffixStr;


                // update the global store
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
                ISharedPreferencesEditor editor = prefs.Edit();

                editor.PutString(Constants.ISSUENO_COLUMN, commonDTO.seqId);
                editor.PutString(Constants.SRCISSUENOPFX_COLUMN, commonDTO.sqlIssueNumberPrefixStr);
                editor.PutString(Constants.SRCISSUENOSFX_COLUMN, commonDTO.sqlIssueNumberSuffixStr);
                editor.Apply();


                // insert a new row
                Task<bool> resultInsertRow = commonADO.InsertRow(thisStruct, context, thisStruct._TIssStruct.MainTable.Name, null);
                bool resultInsertRowCont = await resultInsertRow;
               
                // get the row new id
                thisStruct._rowID = commonADO.GetRowId(thisStruct._TIssStruct.MainTable.Name);
                commonDTO.rowId = thisStruct._rowID;

                // update the global store again
                editor.PutString(Constants.ID_COLUMN, thisStruct._rowID);



                // mark this row as in process 
                Task<bool> resultUpdateRow = commonADO.UpdateRow(thisStruct, Constants.STATUS_INPROCESS, Constants.WS_STATUS_EMPTY, context, thisStruct._TIssStruct.MainTable.Name);
                bool resultUpdateRowCont = await resultUpdateRow;

                return (resultInsertRowCont && resultUpdateRowCont);


            }

            // default case - record isn't saved, no record... or do we want every transaction recorded?
            return true;
        }

// #endif


        public Task<bool> SaveCurrentDisplayPageNoValidation()
        {
            return SaveCurrentDisplayPage(thisStruct, PanelRootPageLinearLayout, PanelRootPageScrollView, 0, context, metrics);
        }
          
        public Task<bool> SaveCurrentLayout()
        {
            return SaveCurrentDisplayPage(thisStruct, PanelRootPageLinearLayout, PanelRootPageScrollView, 1, context, metrics);
        }

        public async Task<Boolean> SaveCurrentDisplayPage(XMLConfig.IssStruct Struct, LinearLayout linearLayout, ScrollView scrollView, Int32 delta, Context Context, DisplayMetrics Metrics)
        {

            // setting these object globals for this run
            thisStruct = Struct;
            context = Context;
            PanelRootPageLinearLayout = linearLayout;
            PanelRootPageScrollView = scrollView;
            metrics = Metrics;



            if (delta == 0)
            {
                PanelPageCurrentDisplayedIndex = 0;
            }
            else
            {
                var panelFields = Struct.Panels[PanelPageCurrentDisplayedIndex].PanelFields;

                foreach (PanelField panelField in panelFields)
                {
                    View uiComponent = PanelRootPageLinearLayout.FindViewWithTag(panelField.Name);

                    if (uiComponent != null)
                    {
                        // don't validate going backward, but validate going forward (1) only
                        if (delta != -1)
                        {

                            if (uiComponent is CustomEditText)
                            {
                                var customView = (CustomEditText)uiComponent;
                                if (customView != null)
                                {
                                    var behavior = customView.BehaviorAndroid;
                                    var valid = ValidateBehavior(Struct.Name, delta, panelField, behavior, customView, customView.Text);
                                    if (!valid)
                                    {
                                        return false;
                                    }
                                }
                            }
                            else if (uiComponent is CustomAutoTextView)
                            {
                                var customView = (CustomAutoTextView)uiComponent;
                                if (customView != null)
                                {
                                    var behavior = customView.BehaviorAndroid;
                                    var valid = ValidateBehavior(Struct.Name, delta, panelField, behavior, customView, customView.Text);
                                    if (!valid)
                                        return false;
                                }

                            }
                            else if (uiComponent is CustomSpinner)
                            {
                                var customView = (CustomSpinner)uiComponent;
                                if (customView != null)
                                {
                                    var behavior = customView.BehaviorAndroid;
                                    var valid = ValidateBehavior(Struct.Name, delta, panelField, behavior, customView, customView.GetValue(panelField.OptionsList));
                                    if (!valid)
                                        return false;
                                }
                            }
                            else if (uiComponent is CustomSignatureImageView)
                            {
                                if (sigImg != null)
                                {
                                    //panelField.Value is nul when backing up?
                                    if (panelField.Value == null)
                                    {
                                        // AJW kludge - find and fix at source of error
                                        panelField.Value = "";
                                    }

                                    if ((panelField.Value.Length == 0) && (delta == 1))
                                    //if (!sigImg.IsDirty && delta == 1)
                                    {
                                        Toast.MakeText(context,
                                                       panelField.Label + " : This Required Field is Missing a Selection.",
                                                       ToastLength.Long).Show();
                                        return false;
                                    }

                                }
                            }

                        } // ( delta != 1 )
                    }  // uiComponent != null
                }  // end foreach

                PanelPageCurrentDisplayedIndex += delta;
            }

            Task<bool> result = UpdateStruct();
            bool resultCont = await result;

            // AJW - TOD - we need to address the failure
            if (resultCont == false)
            {
                Log.Debug("UpdateStruct", "UpdateStruct() failed.");
            }



            return true;
        }

        private void UpdateLocationInfo()
        {
            //if (DroidContext.gLocationUpdateDuration <= 0)
            //{
            //    return;  //Location update is disabled, do nothing.
            //}

            //Get last known location
            double loLatitude = 0;
            double loLongitude = 0;
            GPSLocationUpdate loGPSLocationUpdate = new GPSLocationUpdate(context);
            loGPSLocationUpdate.GetCurrentLocation(ref loLatitude, ref loLongitude);

            // try to use the current page
            int loPageIdx = PanelPageCurrentDisplayedIndex;
            // watch out for the end of stack references
            if (loPageIdx > thisStruct.Panels.Count - 1)
            {
                // fall  back to the root bage
                loPageIdx = 0;
            }

            var panelFields = thisStruct.Panels[loPageIdx].PanelFields;
            foreach (PanelField panelField in panelFields)
            {
                if (panelField.Name.Equals(DBConstants.sqlLatitudeDegreesStr))
                {
                    panelField.Value = loLatitude.ToString();
                }
                else if (panelField.Name.Equals(DBConstants.sqlLongitudeDegreesStr))
                {
                    panelField.Value = loLongitude.ToString();
                }
            }
        }

        private bool ValidateBehavior( string structName, int delta, PanelField panelField, 
                                       EditControlBehavior behavior, View txt, string txtValue,
                                       bool iQuietMode = false )
        {
            //call the validate method here for each control.
            // If it passes validation, we can move ahead
            string loErrMsg = "";
            //if ((behavior.ValidateSelf(ref loErrMsg)) == 0 || delta == -1)
            //{
            //    panelField.Value = txtValue;
            //    return true;
            //}


            if (delta == -1)
            {
                return true;
            }

            bool loMovingForward = true; // always from this method
            if (behavior.OkToExit(loMovingForward) == true)
            {
                return true;
            }

            loErrMsg = behavior.LastValidationErrorMessage;

            // quiet mode?
            if (iQuietMode == true)
            {
                // we're done validating, iQuietMode says don't paint fields or show messsages
                return false;
            }



            // Failed validation - only do this if we are NOT auto skipping
            //then set skip mode to false, since we have to stop on this panel for some reason (failed validation)
            if (!SkipMode)
            {
                //var labelTag = structName + (panelField.Label ?? panelField.Name) + "_LABEL";
                var labelTag = Helper.GetLabelFieldTag(panelField);

                var label = (TextView) PanelRootPageLinearLayout.FindViewWithTag(labelTag);
                if (label != null)
                {
                    label.SetTextColor(Color.Red);
                }


                //txt.RequestFocus();
                //txt.RequestFocusFromTouch(); // ??
                ScrollIntoViewAndSetFocus(txt, null, false, false);



                Toast.MakeText(context, panelField.Label + " : " + loErrMsg, ToastLength.Long).Show();
            }

            SkipMode = false;
            return false;
        }

#region sigPadSupportToBeMoved_2
        public string DecToHexString(Decimal dec )
        {
            var sb = new System.Text.StringBuilder();
            while( dec > 1 ) {
                var r = dec % 16;
                dec /= 16;
                sb.Insert( 0, ((int)r).ToString( "X" ) );
            }
            return sb.ToString();
        }    
#endregion




        #region IssueFormLogic


        private bool _InsidePrepareForEdit = false;
        private bool _MustRebuildDisplayAfterFormInit = false;

        // State attributes
        protected int fFormEditMode = Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts.femNewEntry;
        protected int fFormEditAttrs = 0;
        protected short fFormEditResult = 0;


        public int GetFormEditMode()
        {
            return fFormEditMode;
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
            return (fFormEditAttrs & Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts.feaPrinted) != 0;
        }

        public void SetFormPrinted(bool iSetAttr)
        {
            SetFormEditAttr(Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts.feaPrinted, iSetAttr);
        }

        public bool GetFormSaved()
        {
            return (fFormEditAttrs & Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts.feaSaved) != 0;
        }

        public void SetFormSaved(bool iSetAttr)
        {
            SetFormEditAttr(Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts.feaSaved, iSetAttr);
        }

        public bool GetFormIssueNoLogged()
        {
            return (fFormEditAttrs & Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts.feaIssueNoLogged) != 0;
        }

        public void SetFormIssueNoLogged(bool iSetAttr)
        {
            SetFormEditAttr(Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts.feaIssueNoLogged, iSetAttr);
        }

        public bool GetFormTempFileSaved()
        {
            return (fFormEditAttrs & Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts.feaTempFileSaved) != 0;
        }

        public void SetFormTempFileSaved(bool iSetAttr)
        {
            SetFormEditAttr(Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts.feaTempFileSaved, iSetAttr);
        }



        private bool GetFieldValueFromIssueSource(View uiComponent, PanelField iPanelField, ref string ioIssueSourceValue)
        {
            bool loValueFound = false;
            bool loExtraValueFound = false;

            ioIssueSourceValue = string.Empty;


            //// Loop through all the fields in the structure, copying values to iss form as necessary 
            //for (loStructFldNdx = 0; loStructFldNdx < fCopyFromTable.fTableDef.GetFieldCnt(); loStructFldNdx++)
            //{
            //    loStructFld = fCopyFromTable.fTableDef.GetField(loStructFldNdx);
            //    InitFormDataField(iFormBuilder, loStructFld, loStructFld.Name, "");
            //}

            //// Allow descendant to do any extra fields
            //InitExtraFormDataFields(iFormBuilder);
            


            if (ParentFragment.fSourceIssueStructLogic != null)
            {
                // we have to check for both base form values (straight copies) 
                loValueFound = ParentFragment.fSourceIssueStructLogic.InitFormDataField(iPanelField.Name, ref ioIssueSourceValue);

                // and extra specialty fields
                loExtraValueFound = ParentFragment.fSourceIssueStructLogic.InitExtraFormDataFieldsAndroid(iPanelField.Name, ref ioIssueSourceValue);
            }

            return ( loValueFound || loExtraValueFound );
        }





        /// <summary>
        /// The Android version of PrepareForEdit, to initialize all fields on entry form
        /// </summary>
        public void PrepareForEditAndroid()
        {
            if (thisStruct._TIssForm.PreventEsc == true)
            {
                //DroidContext.mainActivity.SetPreventESC(true, "This is a test of the emergency broadcast system");

                // cant set this here, if set at boot the pages dont get hidden
                // this needs to be set
            }


            ClearFormEditAttrs();


            // kludge!
            if (ParentFragment.fSourceDataRawRow != null)
            {
                SetFormEditMode(ParentFragment.fSourceDataRowFormEditMode);
            }
            else
            {
                SetFormEditMode(Reino.ClientConfig.EditRestrictionConsts.femNewEntry);
            }



            // Set flag so other routines know they are working in context of "FormInit"
            _InsidePrepareForEdit = true;
            _MustRebuildDisplayAfterFormInit = false;


            // kicked off from another source?
            if (ParentFragment.fSourceIssueStructLogic != null)
            {
                // get ready to pull data
                ParentFragment.fSourceIssueStructLogic.InitSourceFormattingInfo(ParentFragment.fSourceIssueStructLogic.fSearchAndIssueResult.SearchResultSelectedRow);
            }


            /////
            ///// Default value sources - first look to retain data from previous forms
            /////


            ISharedPreferences loGlobalDataStore = PreferenceManager.GetDefaultSharedPreferences(DroidContext.ApplicationContext);


            // initialize the fields in this struct
            foreach (EditControlBehavior nextBehavior in DroidContext.XmlCfg.BehaviorCollection.Where(x => x.StructName == thisStruct.Name))
            {

                // before we do any overriding of default values, we must populate the edit controls with the values from the previous form entry
                // since the Android version the edit controls may not persist we have to re-populate them at the start of each form entry
                bool loGlobalDataAvailableForField = false;
                string loGlobalDataForField = string.Empty;
                string loGlobalStoreKeyName = string.Empty;


                if (ParentFragment.fSourceIssueStructLogic != null)
                {
                    if ((nextBehavior.PanelField.Name.Equals("VEHLICNO") == true) && (thisStruct.Name.Equals("PARKING") == true))
                    {
                        // debug breakpoint
                        _MustRebuildDisplayAfterFormInit = !_MustRebuildDisplayAfterFormInit;
                        _MustRebuildDisplayAfterFormInit = !_MustRebuildDisplayAfterFormInit;
                    }
                }


                // first look for global data from last time through
                if (nextBehavior.PanelField != null)
                {
                    //
                    loGlobalStoreKeyName = Helper.BuildGlobalPreferenceKeyName(thisStruct.Name, nextBehavior.PanelField.Name);

                    loGlobalDataForField = loGlobalDataStore.GetString(loGlobalStoreKeyName, null);


                    if (ParentFragment.fSourceIssueStructLogic != null)
                    {
                        if ((nextBehavior.PanelField.Name.Equals("VEHLICNO") == true) && (thisStruct.Name.Equals("PARKING") == true))
                        {
                            // debug breakpoint
                            _MustRebuildDisplayAfterFormInit = !_MustRebuildDisplayAfterFormInit;
                            _MustRebuildDisplayAfterFormInit = !_MustRebuildDisplayAfterFormInit;
                        }
                    }



                    // in re-issue mode? or issue more or etc
                    //if (( GetFormEditMode() & Reino.ClientConfig.EditRestrictionConsts.femReissueAttr) > 0) 
                    {
                        // source table to copy from?
                        if (ParentFragment.fSourceDataRawRow != null)
                        {
                            loGlobalDataForField = Helper.GetSafeColumnStringValueFromDataRow(ParentFragment.fSourceDataRawRow, nextBehavior.PanelField.Name);
                        }
                       
                    }



                    // see if its really saved
                    loGlobalDataAvailableForField = (loGlobalDataForField != null);

                    // look for certain fields to exclude from updating with previous data
                    switch (nextBehavior.PanelField.Name)
                    {
                        // there may be data available, but we aren't copying it for certain fields
                        case AutoISSUE.DBConstants.sqlUniqueKeyStr:
                        case AutoISSUE.DBConstants.sqlSourceIssueMasterKeyStr :
                        case AutoISSUE.DBConstants.sqlMasterKeyStr:
                        case AutoISSUE.DBConstants.sqlOccurenceNumberStr:
                        case AutoISSUE.DBConstants.ID_COLUMN:
                        case AutoISSUE.DBConstants.STATUS_COLUMN:
                        case AutoISSUE.DBConstants.SEQUENCE_ID:
                        case AutoISSUE.DBConstants.WS_STATUS_COLUMN:
                        case AutoISSUE.DBConstants.SRCINUSE_FLAG:
                            {
                                loGlobalDataAvailableForField = false;
                                break;
                            }


                        
                            // handle some other special cases
                        case AutoISSUE.DBConstants.sqlIssueNumberPrefixStr:
                            {
                                loGlobalDataForField = thisStruct.prefix;
                                loGlobalDataAvailableForField = true;
                                break;
                            }

                        case AutoISSUE.DBConstants.sqlIssueNumberStr:
                            {
                                loGlobalDataForField = thisStruct.sequenceId;
                                loGlobalDataAvailableForField = true;
                                break;
                            }

                        case AutoISSUE.DBConstants.sqlIssueNumberSuffixStr:
                            {
                                loGlobalDataForField = thisStruct.suffix;
                                loGlobalDataAvailableForField = true;
                                break;
                            }

                        case AutoISSUE.DBConstants.sqlIssueDateStr:
                            {
                                DateTimeManager.OsDateToDateString(DateTime.Today, Duncan.AI.Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK, ref loGlobalDataForField);
                                loGlobalDataAvailableForField = true;
                                break;
                            }

                        case AutoISSUE.DBConstants.sqlIssueTimeStr:
                            {
                                DateTimeManager.OsTimeToTimeString(DateTime.Now, Duncan.AI.Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK, ref loGlobalDataForField);
                                loGlobalDataAvailableForField = true;
                                break;
                            }


                        default:
                            {
                                // do nothing
                                break;
                            }
                    }
                }



                if (ParentFragment.fSourceIssueStructLogic != null)
                {
                    if ((nextBehavior.PanelField.Name.Equals("VEHLICNO") == true) && (thisStruct.Name.Equals("PARKING") == true))
                    {
                        // debug breakpoint
                        _MustRebuildDisplayAfterFormInit = !_MustRebuildDisplayAfterFormInit;
                        _MustRebuildDisplayAfterFormInit = !_MustRebuildDisplayAfterFormInit;
                    }
                }



                // did we get something?
                if (loGlobalDataAvailableForField == true)
                {
                    Helper.UpdateControlWithNewValuePrim(nextBehavior, loGlobalDataForField, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.bfSetHasBeenFocusedFalse);
                }



                /////
                ///// Default value sources - Now set initial defaults
                /////

                // this may wipe out previous data if the attributes are set to do so
                nextBehavior.InitToDefaultStateAndroid(nextBehavior);



                if (ParentFragment.fSourceIssueStructLogic != null)
                {
                    if ((nextBehavior.PanelField.Name.Equals("VEHLICNO") == true) && (thisStruct.Name.Equals("PARKING") == true))
                    {
                        // debug breakpoint
                        _MustRebuildDisplayAfterFormInit = !_MustRebuildDisplayAfterFormInit;
                        _MustRebuildDisplayAfterFormInit = !_MustRebuildDisplayAfterFormInit;
                    }
                }



                // see if we have data from 3rd party enforcement interfaces
                if (nextBehavior.ControlType == EditEnumerations.CustomEditControlType.EditText)
                {
                    CustomEditText editText = (CustomEditText)nextBehavior.EditCtrl;
                    if (editText != null)
                    {

                        if ((fFormEditMode & Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts.femView) > 0)
                        {
                            //nextBehavior.IsProtected = true;
                        }

                        try
                        {
                            // AJW - get GIS info now
                            string loGISValue = string.Empty;


                            if (ExternalEnforcementInterfaces.GetGISEnforcementValue(thisStruct, editText, editText.BehaviorAndroid.PanelField, ref loGISValue) == true)
                            {
                                // 
                                if (editText.FormStatus == null)
                                {
                                    //editText.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
                                    editText.FormStatus = "Processed";
                                }

                                //editText.Behavior.PanelField.Value = loGISValue;

                                // 
                                editText.Text = loGISValue;
                                editText.HasBeenFocused = true;
                            }
                        }
                        catch (Exception exp)
                        {
                            LoggingManager.LogApplicationError(exp, "FieldName: " + editText.BehaviorAndroid.PanelField.Name, "PrepareForEditAndroid-GISValue");
                            Console.WriteLine("Exception caught in process: {0} {1}", exp, editText.BehaviorAndroid.PanelField.Name);
                        }




                        try
                        {

                            if (ParentFragment.fSourceIssueStructLogic != null)
                            {
                                // AJW - get source struct info now
                                string loIssueSourceValue = string.Empty;

                                if (GetFieldValueFromIssueSource(editText, editText.BehaviorAndroid.PanelField, ref loIssueSourceValue) == true)
                                {
                                    // 
                                    if (editText.FormStatus == null)
                                    {
                                        //editText.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
                                        editText.FormStatus = "Processed";
                                    }

                                    //editText.Behavior.PanelField.Value = loGISValue;

                                    // 
                                    editText.Text = loIssueSourceValue;
                                    editText.HasBeenFocused = true;
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            LoggingManager.LogApplicationError(exp, "FieldName: " + editText.BehaviorAndroid.PanelField.Name, "PrepareForEditAndroid-IssueSource");
                            Console.WriteLine("Exception caught in process: {0} {1}", exp, editText.BehaviorAndroid.PanelField.Name);
                        }




                    }
                }

                if (nextBehavior.ControlType == EditEnumerations.CustomEditControlType.AutoCompleteText)
                {
                    CustomAutoTextView autoTextView = (CustomAutoTextView)nextBehavior.EditCtrl;
                    if (autoTextView != null)
                    {

                        if ((fFormEditMode & Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts.femView) > 0)
                        {
                            //nextBehavior.IsProtected = true;
                        }



                        try
                        {
                            //  get issue source info now
                            string loGISValue = string.Empty;

                            if (ExternalEnforcementInterfaces.GetGISEnforcementValue(thisStruct, autoTextView, autoTextView.BehaviorAndroid.PanelField, ref loGISValue) == true)
                            {
                                //
                                if (autoTextView.FormStatus == null)
                                {
                                    //autoTextView.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
                                    autoTextView.FormStatus = "Processed";
                                }

                                //autoTextView.Behavior.PanelField.Value = loGISValue;


                                // TODO these PULL FROM should be implemented as an edit restriction ala TER_ChildList or TER_RegsitryValue
                                bool loSavedIgnoreEventsState = autoTextView.IgnoreEvents;
                                try
                                {
                                    autoTextView.IgnoreEvents = true;
                                    autoTextView.SetListItemDataByValue(loGISValue);
                                }
                                catch (System.Exception exp)
                                {
                                    LoggingManager.LogApplicationError(exp, "FieldName: " + autoTextView.BehaviorAndroid.PanelField.Name, "PrepareForEditAndroid-GISValue");
                                    Console.WriteLine("Exception caught in process: {0} {1}", exp, autoTextView.BehaviorAndroid.PanelField.Name);
                                }
                                finally
                                {
                                    autoTextView.IgnoreEvents = loSavedIgnoreEventsState;
                                }


                                autoTextView.HasBeenFocused = true;
                            }
                        }
                        catch (Exception exp)
                        {
                            LoggingManager.LogApplicationError(exp, "FieldName: " + autoTextView.BehaviorAndroid.PanelField.Name, "PrepareForEditAndroid-GISValue");
                            Console.WriteLine("Exception caught in process: {0} {1}", exp, autoTextView.BehaviorAndroid.PanelField.Name);
                        }

                        ////////////////

                        try
                        {

                            if (ParentFragment.fSourceIssueStructLogic != null)
                            {
                                // AJW - get source struct info now
                                string loIssueSourceValue = string.Empty;

                                if (GetFieldValueFromIssueSource(autoTextView, autoTextView.BehaviorAndroid.PanelField, ref loIssueSourceValue) == true)
                                {
                                    //
                                    if (autoTextView.FormStatus == null)
                                    {
                                        //autoTextView.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
                                        autoTextView.FormStatus = "Processed";
                                    }

                                    //autoTextView.Behavior.PanelField.Value = loGISValue;

                                    // TODO these PULL FROM should be implemented as an edit restriction ala TER_ChildList or TER_RegsitryValue
                                    bool loSavedIgnoreEventsState = autoTextView.IgnoreEvents;
                                    try
                                    {
                                        autoTextView.IgnoreEvents = true;
                                        autoTextView.SetListItemDataByValue(loIssueSourceValue);
                                    }
                                    catch (System.Exception exp)
                                    {
                                        LoggingManager.LogApplicationError(exp, "FieldName: " + autoTextView.BehaviorAndroid.PanelField.Name, "PrepareForEditAndroid-IssueSource");
                                        Console.WriteLine("Exception caught in process: {0} {1}", exp,  autoTextView.BehaviorAndroid.PanelField.Name);
                                    }
                                    finally
                                    {
                                        autoTextView.IgnoreEvents = loSavedIgnoreEventsState;
                                    }

                                    autoTextView.HasBeenFocused = true;
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            LoggingManager.LogApplicationError(exp, "FieldName: " + autoTextView.BehaviorAndroid.PanelField.Name, "PrepareForEditAndroid-IssueSource");
                            Console.WriteLine("Exception caught in process: {0} {1}", exp, autoTextView.BehaviorAndroid.PanelField.Name);
                        }




                    }
                }

                if (nextBehavior.ControlType == EditEnumerations.CustomEditControlType.Spinner)
                {
                    CustomSpinner spinner = (CustomSpinner)nextBehavior.EditCtrl;
                    if (spinner != null)
                    {
                        //nextBehavior.InitToDefaultStateAndroid(nextBehavior);

                        spinner.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);

                        spinner.FormStatus = null;
                        spinner.HasBeenFocused = false;

                        if ((fFormEditMode & Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts.femView) > 0)
                        {
                           // nextBehavior.IsProtected = true;
                        }


                        try
                        {
                            // AJW - get external enforcement info info now
                            string loGISValue = string.Empty;

                            if (ExternalEnforcementInterfaces.GetGISEnforcementValue(thisStruct, spinner, spinner.BehaviorAndroid.PanelField, ref loGISValue) == true)
                            {
                                // TODO there should be a PrepareForEdit to do this once and not when values are being pulled from DB
                                if (spinner.FormStatus == null)
                                {
                                    //spinner.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
                                    spinner.FormStatus = "Processed";
                                }

                                spinner.BehaviorAndroid.PanelField.Value = loGISValue;

                                // TODO these should be implementaed as an edit restriction ala TER_ChildList
                                if (string.IsNullOrEmpty(loGISValue))
                                {
                                    spinner.SetListIndex(-1);
                                }
                                else
                                {
                                    spinner.SetListItemDataByValue(loGISValue);
                                }

                                spinner.HasBeenFocused = true;
                            }

                        }
                        catch (Exception exp)
                        {
                            LoggingManager.LogApplicationError(exp, "FieldName: " + spinner.BehaviorAndroid.PanelField.Name, "PrepareForEditAndroid-GISValue");
                            Console.WriteLine("Exception caught in process: {0} {1}", exp, spinner.BehaviorAndroid.PanelField.Name);
                        }


                    }
                }
            }



            // Clear flag now so OnRestrictionForcesDisplayRebuild() executes normally
            _InsidePrepareForEdit = false;
            if (_MustRebuildDisplayAfterFormInit == true)
            {
                // We explicity rebuild display pages further down in this routine, so
                // lets only do it once for best speed
                /*OnRestrictionForcesDisplayRebuild();*/
                _MustRebuildDisplayAfterFormInit = false;
            }


            // let the page switch functions decide when to set the field focus
            //// call this again now that all fields have been initialized
            //// we need to clear focus first so we get the first time events again
            //View loCurrentFocus = this.CurrentFocus;
            //if (loCurrentFocus != null)
            //{
            //    loCurrentFocus.ClearFocus();
            //}
            //SetFirstFocusFieldOnCurrentPage();
        }

        #endregion


        ////todo - finish this for form edit modes, first edit focus, etc.
        //protected int fFormEditMode = EditRestrictionConsts.femNewEntry;
        //protected int fFormEditAttrs = 0;
        //protected short fFormEditResult = 0;
        //public int GetFormEditAttrs()
        //{
        //    return fFormEditAttrs;
        //}

        //public void SetFormEditAttr(int iAttribute, bool iSetAttr)
        //{
        //    if (iSetAttr)
        //        fFormEditAttrs |= iAttribute;
        //    else
        //        fFormEditAttrs &= ~iAttribute;
        //}

        /// <summary>
        /// Searches for and returns the panel field by name
        /// returns NULL if not found
        /// </summary>
        /// <param name="iPanelFieldName"></param>
        /// <returns></returns>
        public XMLConfig.PanelField FindPanelControlByName(string iPanelFieldName)
        {
            if (thisStruct == null)
            {
                return null;
            }

            for (int loPanelIdx = 0; loPanelIdx < thisStruct.Panels.Count; loPanelIdx++)
            {
                foreach (XMLConfig.PanelField onePanelField in thisStruct.Panels[loPanelIdx].PanelFields)
                {
                    if (onePanelField.Name.Equals(iPanelFieldName) == true)
                    {
                        return onePanelField;
                    }

                }
            }

            return null;
        }

        public EditControlBehavior GetEditControlBehaviorByPanelFieldName(string iPanelFieldName)
        {
            PanelField onePanelField = FindPanelControlByName( iPanelFieldName );
            if (onePanelField != null)
            {
                return Helper.GetSafeEditControlBehaviorForCustomView(onePanelField.uiComponent);
            }
            else
            {
                return null;
            }
        }



        public void setCurrentIssueDateAndTime()
        {

            // AJW - review the fitness of purpose

            foreach (XMLConfig.PanelField panelField in thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields)
            {
                if (Constants.ISSUETIME_COLUMN.Equals(panelField.Name))
                {
                    string loTmpBuf = "";
                    DateTimeManager.OsTimeToTimeString(DateTime.Now, panelField.EditMask, ref loTmpBuf);
                    panelField.Value = loTmpBuf;                    
                } 

                if (Constants.ISSUEDATE_COLUMN.Equals(panelField.Name))
                {
                    string loTmpBuf = "";
                    DateTimeManager.OsDateToDateString(DateTime.Today, panelField.EditMask, ref loTmpBuf);
                    panelField.Value = loTmpBuf;                    
                }

                if (Constants.ISSUEDATE_COLUMN.Equals(panelField.Name)
                    || Constants.ISSUETIME_COLUMN.Equals(panelField.Name))
                {
                    var customEditText = Helper.MakeEditTextBox(panelField, context, metrics, PanelRootPageLinearLayout, thisStruct.Panels[PanelPageCurrentDisplayedIndex], thisStruct);
                    if (customEditText != null)
                    {
                        var parent = (LinearLayout)customEditText.Parent;
                        if (parent != null)
                        {
                            parent.RemoveView(customEditText);
                        }
                        //then add or re-add
                        parent.AddView(customEditText);
                    }
                }                
            }            
        }



        public PanelField GetCfgCtrlByName(string iTargetName)
        {
            if (string.IsNullOrEmpty(iTargetName) == true)
            {
                return null;
            }

            //normalize
            iTargetName = iTargetName.ToUpper();


            // sometimes we call this after the last page
            int loPanelPageIdx = PanelPageCurrentDisplayedIndex;
              // the toolbar buttons should be disabled at the first/last panels
            if (loPanelPageIdx >= thisStruct.Panels.Count - 1)
            {
                // defense
                loPanelPageIdx = thisStruct.Panels.Count - 1;
            }


            foreach (PanelField panelField in thisStruct.Panels[loPanelPageIdx].PanelFields)
            {
                if (panelField.Name.Equals(iTargetName) == true)
                {
                    return panelField;
                }
            }

            return null;
        }


        public int GetCfgCtrlIndex(Reino.ClientConfig.TTControl CfgCtrl)
        {
            if (CfgCtrl == null)
            {
                return -1;
            }
            else
            {
                //return EntryOrder.IndexOf(CfgCtrl);
                //return thisStruct.Panels[CurrentPanelIdx].PanelFields.IndexOf(CfgCtrl.Name);


                //string loTargetName = thisStruct.Name + CfgCtrl.Name;
                string loTargetName = CfgCtrl.Name;

                // sometimes we call this after the last page
                int loPanelPageIdx = PanelPageCurrentDisplayedIndex;
                // the toolbar buttons should be disabled at the first/last panels
                if (loPanelPageIdx >= thisStruct.Panels.Count - 1)
                {
                    // defense
                    loPanelPageIdx = thisStruct.Panels.Count - 1;
                }

                
                int loEntryIdx = 0;
                foreach (PanelField panelField in thisStruct.Panels[loPanelPageIdx].PanelFields)
                {
                    if (panelField.Name.Equals(loTargetName) == true)
                    {
                        return loEntryIdx;
                    }
                    loEntryIdx++;
                }

                return -1;
            }
        }



        public PanelField GetNextCfgCtrl(Reino.ClientConfig.TTControl AfterCfgEdit, bool MustBeEnabled)
        //public TTControl GetNextCfgCtrl( TTControl AfterCfgEdit, bool MustBeEnabled)
        {

            // defend against the edges
            if ((PanelPageCurrentDisplayedIndex < 0) || (PanelPageCurrentDisplayedIndex >= thisStruct.Panels.Count))
            {
                return null;
            }


            // Get entry order count in local variable to speed up for loops
            //int loEntryOrderCount = EntryOrder.Count;
            int loEntryOrderCount = thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields.Count;

            if (loEntryOrderCount == 0)
            {
                return null;
            }


            // default no starting object
            int loStartIdx = 0;

            // use next field in list if index is acceptable
            if (AfterCfgEdit != null)
            {
                // Get index of starting object
                loStartIdx = GetCfgCtrlIndex(AfterCfgEdit);

                // is this the last control on the page?
                if (loStartIdx == loEntryOrderCount - 1)
                {
                    // no more controls moving forward
                    return null;
                }


                // make sure its valid
                if ((loStartIdx >= 0) && (loStartIdx < loEntryOrderCount - 1))
                {
                    // the first control index after current to check for validity
                    loStartIdx++;
                }

                // if not found, go back to start
                if (loStartIdx == -1)
                {
                    loStartIdx = 0;
                }

               

            }


            int loDebugCounter = 0;


            // we have a starting point, start looking
            if (loEntryOrderCount > 0)
            {
                if (MustBeEnabled == false)
                {
                    return thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields[loStartIdx];
                    //return EntryOrder[0];
                }



                // Loop to find the next one, but never return when its a bitmap control
                // (because we don't treat it as a focusable control)
                for (int loIdx = loStartIdx; loIdx < loEntryOrderCount; loIdx++)
                {
                    // get the reference to the panelfield
                    PanelField onePanelFieldInEntryOrder = thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields[loIdx];

                                        
                    //// AJW - DEMO KLUDGE
                    //if (onePanelFieldInEntryOrder.Name.ToUpper().Equals("METERNO") == true)
                    //{
                    //    continue;
                    //}

                    View uiComponent = null;
                    try
                    {
                        // resolve to the associate view component -TODO shouldn't this be a property of the panelfield for instant access? Or is it disposable and may not be static?
                        uiComponent = PanelRootPageLinearLayout.FindViewWithTag(onePanelFieldInEntryOrder.Name);
                    }
                    catch (Exception exp)
                    {
                        uiComponent = null;
                        Console.WriteLine("Failed to find field in FindViewWithTag:" + exp.Message);
                    }


                    // find it?
                    if (uiComponent == null)
                    {
                        // some fields are suppressed for some configurations, so if we couldn't find the view component, move on
                        continue;
                    }



                    EditControlBehavior uiComponentBehavior = null;

                    // extract and deference the behavior component - TODO roll this into a function, or move the Behaviour reference up to a base class
                    if (uiComponent != null)
                    {
                        if (uiComponent is CustomEditText)
                        {
                            uiComponentBehavior = ((CustomEditText)uiComponent).BehaviorAndroid;
                        }
                        else if (uiComponent is CustomAutoTextView)
                        {
                            uiComponentBehavior = ((CustomAutoTextView)uiComponent).BehaviorAndroid;
                        }
                        else if (uiComponent is CustomSpinner)
                        {
                            uiComponentBehavior = ((CustomSpinner)uiComponent).BehaviorAndroid;
                        }
                        else if (uiComponent is CustomSignatureImageView)
                        {
                            uiComponentBehavior = ((CustomSignatureImageView)uiComponent).BehaviorAndroid;
                        }
                    }


                    if (uiComponentBehavior != null)
                    {
                        // is this a signature?
                        if (uiComponent is CustomSignatureImageView)
                        {
                            // signature fields come from the layout as PROTECTED, which works for CE but not for us
                            // for us, we need to override that and  we will allow focus if it is required and empty

                            // do we have signature yet?
                            if (String.IsNullOrEmpty(uiComponentBehavior.PanelField.Value))
                            {
                                // it it a required field?
                                if (uiComponentBehavior.PanelField.IsRequired == true)
                                {
                                    // we need a signature, stop here
                                    return onePanelFieldInEntryOrder;
                                }
                            }
                        }

                        
                        // normal evaluation for non-signatiure fields
                        if (uiComponentBehavior.CfgCtrl != null)
                        {
                            if (
                                (uiComponentBehavior.CfgCtrl.IsHidden == false) &&
                                (uiComponentBehavior.CfgCtrl.IsProtected == false) &&
                                (uiComponentBehavior.CfgCtrl.IsEnabled == true)
                                //  && (!(onePanelFieldInEntryOrder.fEditFieldDef is TTBitmap))
                                )
                            {
                                return onePanelFieldInEntryOrder;
                            }
                        }
                    }
                    else
                    {
                        //if ((EntryOrder[loIdx].WinCtrl != null) && (EntryOrder[loIdx].WinCtrl.Enabled == true) &&
                        //    (!(EntryOrder[loIdx] is TTBitmap)))
                        //    return EntryOrder[loIdx];
                        //if(!(onePanelFieldInEntryOrder.fEditFieldDef is TTBitmap)))
                        if (
                            (onePanelFieldInEntryOrder.IsHidden == false)                             // AJW - TODO - eliminate duplicated property values/settings
                            )
                        {
                            return onePanelFieldInEntryOrder;
                        }


                    }
                    
                }
            }


            if (loDebugCounter > 0)
            {
                return null;
            }

            // If we get this far, then there isn't a next field
            return null;
        }




        public PanelField GetPreviousCfgCtrl(Reino.ClientConfig.TTControl BeforeCfgEdit, bool MustBeEnabled)
        {
            // Get entry order count in local variable to speed up for loops
            //int loEntryOrderCount = EntryOrder.Count;
            int loEntryOrderCount = thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields.Count;

            if (loEntryOrderCount == 0)
            {
                return null;
            }


            // default no starting object
            int loStartIdx = 0;

            // use next field in list if index is acceptable
            if (BeforeCfgEdit != null)
            {
                // Get index of starting object
                loStartIdx = GetCfgCtrlIndex(BeforeCfgEdit);


                // is this the first control on the page?
                if (loStartIdx == 0)
                {
                    // no more controls moving backward
                    return null;
                }


                // make sure its valid
                if (loStartIdx > 0) 
                {
                    // the first control index after current to check for validity
                    loStartIdx--;
                }

                // if not found, go to the end
                if (loStartIdx == -1)
                {
                    loStartIdx = loEntryOrderCount - 1;
                }

            }

            // we have a starting point, start looking
            if (loEntryOrderCount > 0)
            {
                if (MustBeEnabled == false)
                {
                    return thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields[loStartIdx];
                    //return EntryOrder[0];
                }

                // Loop to find the next one, but never return when its a bitmap control
                // (because we don't treat it as a focusable control)
                for (int loIdx = loStartIdx; loIdx >= 0; loIdx--)
                    {
                    // get the reference to the panelfield
                    PanelField onePanelFieldInEntryOrder = thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields[loIdx];

                    //// AJW - DEMO KLUDGE
                    //if (onePanelFieldInEntryOrder.Name.ToUpper().Equals("METERNO") == true)
                    //{
                    //    continue;
                    //}



                    View uiComponent = null;
                    try
                    {
                        // resolve to the associate view component -TODO shouldn't this be a property of the panelfield for instant access? Or is it disposable and may not be static?
                        uiComponent = PanelRootPageLinearLayout.FindViewWithTag(onePanelFieldInEntryOrder.Name);
                    }
                    catch (Exception exp)
                    {
                        uiComponent = null;
                        Console.WriteLine("Failed to find field in FindViewWithTag:" + exp.Message);
                    }


                    // find it?
                    if (uiComponent == null)
                    {
                        // some fields are suppressed for some configurations, so if we couldn't find the view component, move on
                        continue;
                    }


                    EditControlBehavior uiComponentBehavior = null;

                    // extract and deference the behavior component - TODO roll this into a function, or move the Behaviour reference up to a base class
                    if (uiComponent != null)
                    {
                        if (uiComponent is CustomEditText)
                        {
                            uiComponentBehavior = ((CustomEditText)uiComponent).BehaviorAndroid;
                        }
                        else if (uiComponent is CustomAutoTextView)
                        {
                            uiComponentBehavior = ((CustomAutoTextView)uiComponent).BehaviorAndroid;
                        }
                        else if (uiComponent is CustomSpinner)
                        {
                            uiComponentBehavior = ((CustomSpinner)uiComponent).BehaviorAndroid;
                        }
                        else if (uiComponent is CustomSignatureImageView)
                        {
                            uiComponentBehavior = ((CustomSignatureImageView)uiComponent).BehaviorAndroid;
                        }
                    }


                    if (uiComponentBehavior != null)
                    {
                        if (uiComponentBehavior.CfgCtrl != null)
                        {
                            if (
                                (uiComponentBehavior.CfgCtrl.IsHidden == false) &&
                                (uiComponentBehavior.CfgCtrl.IsProtected == false) &&
                                (uiComponentBehavior.CfgCtrl.IsEnabled == true)
                                //  && (!(onePanelFieldInEntryOrder.fEditFieldDef is TTBitmap))
                                )
                            {
                                return onePanelFieldInEntryOrder;
                            }
                        }
                    }
                    else
                    {
                        //if ((EntryOrder[loIdx].WinCtrl != null) && (EntryOrder[loIdx].WinCtrl.Enabled == true) &&
                        //    (!(EntryOrder[loIdx] is TTBitmap)))
                        //    return EntryOrder[loIdx];
                        //if(!(onePanelFieldInEntryOrder.fEditFieldDef is TTBitmap)))
                        if (
                             (onePanelFieldInEntryOrder.IsHidden == false)                                   // AJW - TODO - eliminate duplicated property values/settings
                            )
                        {
                            return onePanelFieldInEntryOrder;
                        }
                    }
                    
                }
            }


            // If we get this far, then there isn't a next field
            return null;
        }


        private bool IsViewVisible(View view)
        {
            Rect scrollBounds = new Rect();
            PanelRootPageScrollView.GetDrawingRect(scrollBounds);

            float top = view.GetY();
            float bottom = top + view.Height;


            // TODO - needs to account for the toolbar?


            if (scrollBounds.Top < top && scrollBounds.Bottom > bottom)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ScrollIntoViewAndSetFocus(View uiComponentNext, Reino.ClientConfig.TTControl iCurrentCfgCtrl, Boolean iMovingBackwards, Boolean iScrollFromTopOfForm )
        {
            if (uiComponentNext == null)
            {
                return;
            }



           


            // isn't this already on the UI thread?
            RunOnUiThread(() =>
            {

                try
                {

                    // Determine where to set the scroll-to to by measuring the distance from the top of the scroll view
                    // to the control to focus on by summing the "top" position of each view in the hierarchy.
                    int yDistanceToControlsView = 0;
                    //View parentView = (View)uiComponentNext.Parent;
                    var parentView = Android.Runtime.Extensions.JavaCast<View>(uiComponentNext.Parent);
                    while (true)
                    {
                        if (parentView.Equals(PanelRootPageScrollView))
                        {
                            break;
                        }
                        yDistanceToControlsView += parentView.Top;
                        //parentView = (View)parentView.Parent;
                        parentView = Android.Runtime.Extensions.JavaCast<View>(parentView.Parent);
                    }

                    // Compute the final position value for the top and bottom of the control in the scroll view.
                    int topInScrollView = yDistanceToControlsView + uiComponentNext.Top;
                    int bottomInScrollView = yDistanceToControlsView + uiComponentNext.Bottom;


                    View uiComponentCurrent = null;
                    if (iMovingBackwards)
                    {
                        if (iCurrentCfgCtrl != null)
                        {
                            uiComponentCurrent = PanelRootPageLinearLayout.FindViewWithTag(iCurrentCfgCtrl.Name);
                        }


                        if ((uiComponentCurrent != null) && (uiComponentCurrent is CustomEditText))
                        {
                            ((CustomEditText)uiComponentCurrent).BehaviorAndroid.ValidationDisabled = true;
                        }

                    }

                    // take the height of the parent view, which also contains the label
                    //int height = ((View)uiComponentNext.Parent).Height;
                    int height = Android.Runtime.Extensions.JavaCast<View>(uiComponentNext.Parent).Height;



                    // AJW - kludgy workaround for fields with no labels - this needs to be dynamically calculated
                    //if (height < 185)
                    //{
                    //    height = 185;
                    //}


                    // ** this doesn't work for us because the visibility test fails to account for the toolbar so it can leave the field covered **
                    // test and don't scroll if we're already onscreen
                    //Rect scrollBounds = new Rect();
                    //PanelRootPageScrollView.GetHitRect(scrollBounds);


                    //if (uiComponentNext.GetLocalVisibleRect(scrollBounds) == false)
                    {
                        // this works, but the dropdown list pop-up will often scroll the screen to make room for the dropdown. Need to make it drop "up"
                        bool loSrcollFromTheBottom = (DroidContext.gFormEntryFocusFieldPlaceAtBottom > 0);

                        int loScrollToPosition = 0;


                        if (loSrcollFromTheBottom == true)
                        {
                            Rect loDrawingRect = new Rect();
                            PanelRootPageScrollView.GetDrawingRect(loDrawingRect);

                            //loScrollToPosition = (bottomInScrollView - (height*2));

                            loScrollToPosition = (((topInScrollView + bottomInScrollView) / 2) - height);

                            loScrollToPosition -= (loDrawingRect.Bottom - loDrawingRect.Top);

                            //int labelHeight = 78;  // AJW - TODO this should be dynamically determined

                            int labelHeight = 92;  // AJW - TODO this should be dynamically determined


                            if (uiComponentNext.Parent is LinearLayout)
                            {
                                LinearLayout loParentLinearLayout = (LinearLayout)uiComponentNext.Parent;
                                if (loParentLinearLayout.ChildCount < 2)
                                {
                                    labelHeight = (labelHeight * 2);
                                }
                            }



                            loScrollToPosition += (height * 2) + (labelHeight);


                            // defend against offsecreen placement - use the top of view placement instead
                            if (loScrollToPosition < 0)
                            {

                                loScrollToPosition = (((topInScrollView + bottomInScrollView) / 2) - height);
                            }
                        }
                        else
                        {
                            // this is the best position - puts the field in the middle of the scrollview so 
                            // you can (usually) see the previous field data and the next field
                            bool loScrollToMidddle = true;


                            if (loScrollToMidddle == true)
                            {
                                // scroll to top middle

                                loScrollToPosition = (((topInScrollView + bottomInScrollView) / 2) - (height * 2));
                            }
                            else
                            {         // scroll to the top 

                                loScrollToPosition = (((topInScrollView + bottomInScrollView) / 2) - height);
                            }
                        }



                        // keep scrolling smooth
                        int cnMinDurationMS = 250;
                        int cnMaxDurationMS = 1500;



                        if (iScrollFromTopOfForm == true)
                        {

                            // go to top an wait a bit
                            PanelRootPageScrollView.ScrollTo(0, 0);
                            //Thread.Sleep(500);

                            // now scroll to focus field - set a ms step per distance
                            //int durationMS = 1500;
                            int durationMS = 100 * (loScrollToPosition / 100);

                            // keep in bounds bounds
                            if (durationMS < cnMinDurationMS)
                            {
                                durationMS = cnMinDurationMS;
                            }
                            else if (durationMS > cnMaxDurationMS)
                            {
                                durationMS = cnMaxDurationMS;
                            }


                            ObjectAnimator.OfInt(PanelRootPageScrollView, "scrollY", loScrollToPosition).SetDuration(durationMS).Start();
                        }
                        else
                        {

                            // Defer the scroll code until the form is rendered
                            //PanelRootPageScrollView.PostDelayed(new RunnableAnonymousScrollViewClassHelper(PanelRootPageScrollView, 0, loScrollToPosition), 25);

                            //PanelRootPageScrollView.SmoothScrollTo(0, loScrollToPosition);

                            // scrolling from current position, keep it smooth
                            int durationMS = cnMinDurationMS;
                            ObjectAnimator.OfInt(PanelRootPageScrollView, "scrollY", loScrollToPosition).SetDuration(durationMS).Start();
                        }
                    }



                    // what is invoked on touch of signature image bitmap?
                    // TODO - ajw

                    //uiComponentNext.RequestFocus();
                    uiComponentNext.ClearFocus();
                    uiComponentNext.RequestFocusFromTouch();



                    if (iMovingBackwards)
                    {
                        // Now reenable validations on the field that was active
                        if (uiComponentCurrent != null)
                        {
                            if (uiComponentCurrent is CustomEditText)
                            {
                                ((CustomEditText)uiComponentCurrent).BehaviorAndroid.ValidationDisabled = false;
                            }
                            else if (uiComponentCurrent is CustomAutoTextView)
                            {
                                ((CustomAutoTextView)uiComponentCurrent).BehaviorAndroid.ValidationDisabled = false;
                            }
                            else if (uiComponentCurrent is CustomSpinner)
                            {
                                ((CustomSpinner)uiComponentCurrent).BehaviorAndroid.ValidationDisabled = false;
                            }

                        }
                    }

                }
                catch (Exception e)
                {
                    LoggingManager.LogApplicationError(e, "ScrollIntoViewAndSetFocus", e.TargetSite.Name);
                }

            });

        }


        /// <summary>
        /// User at start of edit - try to start at the target field, 
        /// but validating each field before it starting from the top, 
        /// stopping on the first field that fails validation
        /// or ending up at the target if all fields pass
        /// </summary>
        /// <param name="Struct"></param>
        /// <param name="linearLayout"></param>
        /// <param name="delta"></param>
        /// <param name="Context"></param>
        /// <param name="Metrics"></param>
        /// <param name="ioPassedValidation"></param>
        private void SetFocusOnTargetOrFirstInvalidField(string iIdealTargetFieldName)
        {
            try
            {
                //thisStruct = Struct;
                //context = Context;
                //PanelRootPageLinearLayout = linearLayout;
                //metrics = Metrics;


                // delta always forward
                int delta = 1;

                // assume success
                //ioPassedValidation = true;

                // View-Only mode?
                //if (this.GetFormEditMode() == EditRestrictionConsts.femView)
                {
                    // First we need to see if there is something enabled on the same page that
                    // will take precedence over moving to a new page.
                }


                View uiComponent = null;
                PanelField loCandidatePanelField = null;
                EditControlBehavior loCandidatePanelFieldBehavior = null;



                int loResult;
                string loErrMsg = "";

                var panelFields = thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields;

                // work through the fields until we hit the target, or we hit a field that fails validation
                foreach (PanelField panelField in panelFields)
                {
                    // find it
                    uiComponent = PanelRootPageLinearLayout.FindViewWithTag(panelField.Name);


                    if (uiComponent != null)
                    {

                        // is this the target field?
                        if (uiComponent.Tag.Equals(iIdealTargetFieldName) == true)
                        {
                            // no further, stopping here
                            break;
                        }


                        if (uiComponent is CustomEditText)
                        {
                            CustomEditText customView = (CustomEditText)uiComponent;
                            loCandidatePanelFieldBehavior = customView.BehaviorAndroid;
                            var valid = ValidateBehavior(thisStruct.Name, delta, loCandidatePanelFieldBehavior.PanelField, loCandidatePanelFieldBehavior, customView, customView.Text, true);
                            if (!valid)
                            {
                                // no further, stopping here
                                break;
                            }

                            loCandidatePanelField = loCandidatePanelFieldBehavior.PanelField;
                        }
                        else if (uiComponent is CustomAutoTextView)
                        {
                            CustomAutoTextView customView = (CustomAutoTextView)uiComponent;
                            loCandidatePanelFieldBehavior = customView.BehaviorAndroid;
                            var valid = ValidateBehavior(thisStruct.Name, delta, loCandidatePanelFieldBehavior.PanelField, loCandidatePanelFieldBehavior, customView, customView.Text, true);
                            if (!valid)
                            {
                                // no further, stopping here
                                break;
                            }

                            loCandidatePanelField = loCandidatePanelFieldBehavior.PanelField;
                        }
                        else if (uiComponent is CustomSpinner)
                        {

                            CustomSpinner customView = (CustomSpinner)uiComponent;
                            loCandidatePanelFieldBehavior = customView.BehaviorAndroid;
                            var valid = ValidateBehavior(thisStruct.Name, delta, loCandidatePanelFieldBehavior.PanelField, loCandidatePanelFieldBehavior, customView, customView.GetValue(loCandidatePanelFieldBehavior.PanelField.OptionsList), true);
                            if (!valid)
                            {
                                // no further, stopping here
                                break;
                            }

                            loCandidatePanelField = loCandidatePanelFieldBehavior.PanelField;
                        }
                        else if (uiComponent is CustomSignatureImageView)
                        {
                            if (sigImg.Visibility == ViewStates.Visible)
                            {
                                CustomSignatureImageView customView = (CustomSignatureImageView)uiComponent;
                                loCandidatePanelFieldBehavior = customView.BehaviorAndroid;

                                if ((loCandidatePanelFieldBehavior.PanelField.Value.Length == 0) && (delta == 1))
                                {
                                    // no further, stopping here
                                    break;
                                }
                            }
                        } // end else if
                    }
                }  // end foreach


                if (uiComponent != null)
                {
                    ScrollIntoViewAndSetFocus(uiComponent, null, false, true);
                }
            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "", "SetFocusOnTargetOrFirstInvalidField");
                Console.WriteLine("Exception caught in process: {0} {1}", exp, "SetFocusOnTargetOrFirstInvalidField");
            }


        }


        private void SetFirstFocusFieldOnCurrentPage()
        {
            // do this on the UI thread so it happens after the page has been fully rendered 
            RunOnUiThread(() =>
            {

                PanelField NextCfgCtrl = null;

                // does this form have a firstfocus field defined?
                if (thisStruct != null)
                {
                    if (thisStruct._TIssForm != null)
                    {
                        if (thisStruct._TIssStruct != null)
                        {
                            // assume standard staring positions
                            string loFirstFocusFieldName = string.Empty;


                            if (thisStruct._TIssStruct is TMarkModeStruct)
                            {
                                loFirstFocusFieldName = DroidContext.gFormEntryFirstFocusMarkMode;
                            }
                            else if (thisStruct._TIssStruct is TIssStruct)
                            {
                                loFirstFocusFieldName = DroidContext.gFormEntryFirstFocusParking;
                            }
                            


                            // are we in a specialty mode?
                            if ((GetFormEditMode() & (Reino.ClientConfig.EditRestrictionConsts.femIssueMoreAttr)) > 0)
                            {
                                if (thisStruct._TIssForm.IssueMoreFirstFocus != null)
                                {
                                    if (thisStruct._TIssForm.IssueMoreFirstFocus != null)
                                    {
                                        loFirstFocusFieldName = thisStruct._TIssForm.IssueMoreFirstFocus;
                                    }
                                }
                            }


                            //if ((GetFormEditMode() & (Reino.ClientConfig.EditRestrictionConsts.femIssueMoreAttr)) > 0)
                            //{
                            //    if (thisStruct._TIssForm.AutoIssueMore != null)
                            //    {
                            //        if (thisStruct._TIssForm.AutoIssueMore != null)
                            //        {
                            //            loFirstFocusFieldName = thisStruct._TIssForm.AutoIssueMore;
                            //        }
                            //    }
                            //}





                            // get a name to look for?
                            if (string.IsNullOrEmpty(loFirstFocusFieldName) == false)
                            {
                                NextCfgCtrl = GetCfgCtrlByName(loFirstFocusFieldName);
                            }


                        }
                    }
                }


                // have a skip to field?
                if (NextCfgCtrl == null)
                {
                    // get next editable control in entry order, starting from the top
                    NextCfgCtrl = GetNextCfgCtrl(null, true);
                }

                if (NextCfgCtrl != null)
                {
                    //go to it
                    SetFocusOnTargetOrFirstInvalidField(NextCfgCtrl.Name);
                }

            });

        }


        public bool TabForward(XMLConfig.IssStruct Struct, LinearLayout linearLayout, Int32 delta, Context Context, DisplayMetrics Metrics, ref Boolean ioPassedValidation)
        {
            thisStruct = Struct;
            context = Context;
            PanelRootPageLinearLayout = linearLayout;
            metrics = Metrics;

            // assume success
            ioPassedValidation = true;

            // View-Only mode?
            //if (this.GetFormEditMode() == EditRestrictionConsts.femView)
            {
                // First we need to see if there is something enabled on the same page that
                // will take precedence over moving to a new page.
            }


            PanelField loExitingPanelField = null;
            EditControlBehavior loExitingPanelFieldBehavior = null;



            //if (delta == 0)
            //    CurrentPanelIdx = 0;
            //else
            {
                int loResult;
                string loErrMsg = "";

                var panelFields = Struct.Panels[PanelPageCurrentDisplayedIndex].PanelFields;

                //foreach (PanelField panelField in panelFields)
                {
                    //View uiComponent = layout.FindViewWithTag(panelField.Name);

                    // if moving forward, we need to validate the current field
                    // depending on what the user pressed to get here (tab vs keyboard NEXT) the field may still have focus
                    object uiComponent = Struct.Panels[PanelPageCurrentDisplayedIndex].FocusedViewCurrent;
                    if (uiComponent == null)
                    {
                        uiComponent = Struct.Panels[PanelPageCurrentDisplayedIndex].FocusedViewPrevious;
                    }




                    if (uiComponent != null )
                    {

                        if (uiComponent is CustomEditText)
                        {
                            CustomEditText customView = (CustomEditText)uiComponent;
                            loExitingPanelFieldBehavior = customView.BehaviorAndroid;
                            var valid = ValidateBehavior(Struct.Name, delta, loExitingPanelFieldBehavior.PanelField, loExitingPanelFieldBehavior, customView, customView.Text);
                            if (!valid)
                            {
                                ioPassedValidation = false;
                                return false;
                            }

                            loExitingPanelField = loExitingPanelFieldBehavior.PanelField;
                        }
                        else if (uiComponent is CustomAutoTextView)
                        {
                            CustomAutoTextView customView = (CustomAutoTextView)uiComponent;
                            loExitingPanelFieldBehavior = customView.BehaviorAndroid;
                            var valid = ValidateBehavior(Struct.Name, delta, loExitingPanelFieldBehavior.PanelField, loExitingPanelFieldBehavior, customView, customView.Text);
                            if (!valid)
                            {
                                ioPassedValidation = false;
                                return false;
                            }

                            loExitingPanelField = loExitingPanelFieldBehavior.PanelField;
                        }
                        else if (uiComponent is CustomSpinner)
                        {

                            CustomSpinner customView = (CustomSpinner)uiComponent;
                            loExitingPanelFieldBehavior = customView.BehaviorAndroid;
                            var valid = ValidateBehavior(Struct.Name, delta, loExitingPanelFieldBehavior.PanelField, loExitingPanelFieldBehavior, customView, customView.GetValue(loExitingPanelFieldBehavior.PanelField.OptionsList));
                            if (!valid)
                            {
                                ioPassedValidation = false;
                                return false;
                            }

                            loExitingPanelField = loExitingPanelFieldBehavior.PanelField;
                        }
                        else if (uiComponent is CustomSignatureImageView)
                        {
                            if (sigImg.Visibility == ViewStates.Visible)
                            {
                                CustomSignatureImageView customView = (CustomSignatureImageView)uiComponent;
                                loExitingPanelFieldBehavior = customView.BehaviorAndroid;

                                if ((loExitingPanelFieldBehavior.PanelField.Value.Length == 0) && (delta == 1))
                                // if (!sigImg.IsDirty && delta == 1)
                                {
                                    Toast.MakeText(context,
                                                   //panelField.Label + " : This Required Field is Missing a Selection.",
                                                   "This Required Field is Missing a Selection.",
                                                   ToastLength.Long).Show();
                                    ioPassedValidation = false;
                                    return false;
                                }
                            }
                        } // end else if
                    }
                }  // end foreach

            }
            


            // current field passed validation.... 
            TTControl CurrentCfgCtrl = null;
            if ( loExitingPanelField != null )
            {
               CurrentCfgCtrl = loExitingPanelField.fEditFieldDef;
            }


            // current field has passed validation... although we haven't lost focus yet, we need to notify dependents that we're going to
            // so that they can update their protected status as needed and allow us to tab into them (or not)
            if (loExitingPanelFieldBehavior != null)
            {
                // dont need this anymore because it is called from OkToExit
                //loExitingPanelFieldBehavior.NotifyDependents(Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts.dneParentFieldExit);
            }

            
            
            // get next editable control in entry order
            PanelField NextCfgCtrl = GetNextCfgCtrl(CurrentCfgCtrl, true);

            // Don't change focus if there is no next control
            if (NextCfgCtrl == null)
            {
                //ReturnFocusToActiveCtrl();
                return false;
            }

            // Move current control off entry panel and move next control onto entry panel, making it current
            View uiComponentNext = PanelRootPageLinearLayout.FindViewWithTag(NextCfgCtrl.Name);

            if (uiComponentNext != null)
            {
                //uiComponentNext.RequestFocus();
                //uiComponentNext.RequestFocusFromTouch();

                ScrollIntoViewAndSetFocus(uiComponentNext, CurrentCfgCtrl, false, false);

            }


            return true;
        }


        public bool TabBackward(XMLConfig.IssStruct Struct, LinearLayout linearLayout, Int32 delta, Context Context, DisplayMetrics Metrics )
        {
            /*
            TTControl NextCfgCtrl = null;

            // Retain current field, then disable its validations until we are finished with the focus changes
            TTControl loActiveCfgCtrl = CurrentCfgCtrl;
            if ((loActiveCfgCtrl != null) && (loActiveCfgCtrl.TypeOfWinCtrl() == typeof(ReinoTextBox)))
            {
                loActiveCfgCtrl.Behavior.ValidationDisabled = true;
            }

            // Get previous field
            NextCfgCtrl = GetPreviousCfgCtrl(CurrentCfgCtrl, true);
            // Don't change focus if there is no next control
            if (NextCfgCtrl == null)
            {
                ReturnFocusToActiveCtrl();
                return false;
            }

            // Move current field off entry panel and move next field onto entry panel, making it current
            SetActiveCfgCtrl(NextCfgCtrl);

            // Now reenable validations on the field that was active
            if ((loActiveCfgCtrl != null) &&
                (loActiveCfgCtrl.TypeOfWinCtrl() == typeof(ReinoTextBox)))
            {
                loActiveCfgCtrl.Behavior.ValidationDisabled = false;
            }
            */



            thisStruct = Struct;
            context = Context;
            PanelRootPageLinearLayout = linearLayout;
            metrics = Metrics;


            // View-Only mode?
            //if (this.GetFormEditMode() == EditRestrictionConsts.femView)
            {
                // First we need to see if there is something enabled on the same page that
                // will take precedence over moving to a new page.
            }


            PanelField loExitingPanelField = null;
            EditControlBehavior loExitingPanelFieldBehavior = null;



            //if (delta == 0)
            //    CurrentPanelIdx = 0;
            //else
            {
                int loResult;
                string loErrMsg = "";

                var panelFields = Struct.Panels[PanelPageCurrentDisplayedIndex].PanelFields;

                //foreach (PanelField panelField in panelFields)
                {
                    //View uiComponent = layout.FindViewWithTag(panelField.Name);

                    // if moving forward, we need to validate the current field
                    // depending on what the user pressed to get here (tab vs keyboard NEXT) the field may still have focus
                    object uiComponent = Struct.Panels[PanelPageCurrentDisplayedIndex].FocusedViewCurrent;
                    if (uiComponent == null)
                    {
                        uiComponent = Struct.Panels[PanelPageCurrentDisplayedIndex].FocusedViewPrevious;
                    }



                    // AJW - make sure the UNLESS this is a different copy of the view??

                    if (uiComponent != null)
                    {

                        if (uiComponent is CustomEditText)
                        {
                            CustomEditText customView = (CustomEditText)uiComponent;
                            loExitingPanelFieldBehavior = customView.BehaviorAndroid;
                            //var valid = ValidateBehavior(Struct.Name, delta, behavior.PanelField, behavior, customView, customView.Text);
                            //if (!valid)
                            //{
                            //    return false;
                            //}

                            loExitingPanelField = loExitingPanelFieldBehavior.PanelField;
                        }
                        else if (uiComponent is CustomAutoTextView)
                        {
                            CustomAutoTextView customView = (CustomAutoTextView)uiComponent;
                            loExitingPanelFieldBehavior = customView.BehaviorAndroid;
                            //var valid = ValidateBehavior(Struct.Name, delta, behavior.PanelField, behavior, customView, customView.Text);
                            //if (!valid)
                            //{
                            //    return false;
                            //}

                            loExitingPanelField = loExitingPanelFieldBehavior.PanelField;
                        }
                        else if (uiComponent is CustomSpinner)
                        {

                            CustomSpinner customView = (CustomSpinner)uiComponent;
                            loExitingPanelFieldBehavior = customView.BehaviorAndroid;
                            //var valid = ValidateBehavior(Struct.Name, delta, behavior.PanelField, behavior, customView, customView.GetValue(behavior.PanelField.OptionsList));
                            //if (!valid)
                            //{
                            //    return false;
                            //}

                            loExitingPanelField = loExitingPanelFieldBehavior.PanelField;
                        }
                        else if (uiComponent is CustomSignatureImageView)
                        {
                            CustomSignatureImageView customSignatureImageView = (CustomSignatureImageView)uiComponent;
                            loExitingPanelFieldBehavior = customSignatureImageView.BehaviorAndroid;

                            // don't validate going backward
                            //if (sigImg.Visibility == ViewStates.Visible)
                            //{
                            //    if (!sigImg.IsDirty && delta == 1)
                            //    {
                            //        Toast.MakeText(context,
                            //                       //panelField.Label + " : This Required Field is Missing a Selection.",
                            //                       "This Required Field is Missing a Selection.",
                            //                       ToastLength.Long).Show();
                            //        ioPassedValidation = false;
                            //        return false;
                            //    }
                            //}

                            loExitingPanelField = loExitingPanelFieldBehavior.PanelField;

                        } // end else if
                    }
                }  // end foreach

            }



            // current field passed validation.... 
            TTControl CurrentCfgCtrl = null;
            if (loExitingPanelField != null)
            {
                CurrentCfgCtrl = loExitingPanelField.fEditFieldDef;
            }

            // current field has passed validation... although we haven't lost focus yet, we need to notify dependents that we're going to
            // so that they can update their protected status as needed and allow us to tab into them (or not)
            if (loExitingPanelFieldBehavior != null)
            {
                // dont need this anymore because it is called from OkToExit
                //loExitingPanelFieldBehavior.NotifyDependents(Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts.dneParentFieldExit);

            }


            // get previous editable control in entry order
            PanelField loCnfgControlToRecieveFocus = GetPreviousCfgCtrl(CurrentCfgCtrl, true);

            // Don't change focus if there is no next control
            if (loCnfgControlToRecieveFocus == null)
            {
                //ReturnFocusToActiveCtrl();
                return false;
            }

            // Move current control off entry panel and move next control onto entry panel, making it current
            View uiComponentNext = PanelRootPageLinearLayout.FindViewWithTag(loCnfgControlToRecieveFocus.Name);



            if (uiComponentNext != null)
            {

                ScrollIntoViewAndSetFocus(uiComponentNext, CurrentCfgCtrl, true, false);
            }


            return true;

        }



        #region IssueFormLogic_MoreOfIt


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
        //        if ((thisStruct._TIssForm.StructLogicObj != null) && (thisStruct._TIssForm.StructLogicObj is IssueStructLogic))
        //            return thisStruct._TIssForm.StructLogicObj as IssueStructLogic;

        //        // StructLogicObj is not the desired class type, so return null
        //        return null;
        //    }
        //}

        private SearchStructLogicAndroid AssocSearchStructLogic
        {
            get
            {
                // If the StructLogicObj is of class SearchStructLogic (or descendant), then return it as such
                if ((thisStruct._TIssForm.StructLogicObj != null) && (thisStruct._TIssForm.StructLogicObj is SearchStructLogicAndroid))
                {
                    return thisStruct._TIssForm.StructLogicObj as SearchStructLogicAndroid;
                }

                // StructLogicObj is not the desired class type, so return null
                return null;
            }
        }

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





        private bool DoHotSheetSearch(Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_SearchHotSheet EditRestrict)
        {
            THotSheetStruct loHotSheetStruct = GetHotSheetStruct(EditRestrict.CharParam);
            if (loHotSheetStruct != null)
            {
                SearchStructLogicAndroid loSearchLogic = (SearchStructLogicAndroid)(loHotSheetStruct.StructLogicObj);

                bool loSearchResult = loSearchLogic.PerformSearchAndIssue(this, true, 1, EditRestrict.MatchFieldsName, EditRestrict, true);

                return loSearchResult;
            }
            else
            {
                return false;
            }
        }


        private void DoHotSheetFilter(Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_HotsheetFilter EditRestrict)
        {

            SearchParameterPacket loTable = null;
            TTTable loParentTable = null;
            bool loWirelessHostAvailable = false;

            // Because of the way wireless & local searches interact, it is possible
            // for a wireless result to be processed while the local search is processed.
            // The wireless result will call FinishEnforceRestriction prior to returnning from
            // PerformSearch.  If this happens, we need to ignore the PerformSearch results
            // because the wireless results are to be kept and used. 
            EditRestrict.fPreemptedByWireless = false;

#if _original_
            // Reset the table to the local (not wireless version)
            EditRestrict.GetParent().ListSourceTable = EditRestrict.GetParent().LocalListSourceTable;
            if (EditRestrict.GetParent() != null)
            {
                EditRestrict.GetParent().ListItemCount = EditRestrict.GetParent().ListSourceTable.GetRecCount();
                EditRestrict.GetParent().ListItemCache.Clear();
                if (EditRestrict.GetParent().GridDisplayCache != null)
                    EditRestrict.GetParent().GridDisplayCache.Clear();
                if (EditRestrict.GetParent().ListBox != null)
                    EditRestrict.GetParent().ListBox.RefreshItems(false);
            }

            // Make sure we have an associated table and it is the same as the search table
            loParentTable = EditRestrict.GetParent().ListSourceTable;
            if (loParentTable == null)
                return;

            // Find the hotsheet struct
            THotSheetStruct loHotSheetStruct = GetHotSheetStruct(loParentTable.fTableDef.Name);
            if (loHotSheetStruct == null)
            {
                //AppMessageBox.ShowMessageWithBell("TER_HotsheetFilter: No hotsheet named " + loParentTable.fTableDef.Name, "", "");
                return;
            }
#else
            string loParentTableName = EditRestrict.GetParent().ListTableName;

            if (String.IsNullOrEmpty(loParentTableName) == true)
            {
                //AppMessageBox.ShowMessageWithBell("TER_HotsheetFilter: No hotsheet named " + loParentTableName, "", "");
                return;
            }


            // Find the hotsheet struct
            THotSheetStruct loHotSheetStruct = GetHotSheetStruct(loParentTableName);


            if (loHotSheetStruct == null)
            {
                //AppMessageBox.ShowMessageWithBell("TER_HotsheetFilter: No hotsheet named " + loParentTable.fTableDef.Name, "", "");
                return;
            }

#endif



            //loTable = ((SearchStructLogicAndroid)(loHotSheetStruct.StructLogicObj)).PerformSearch(
            //    this.CfgForm, true, 1, EditRestrict.MatchFieldsName, false,
            //    EditRestrict, ref loWirelessHostAvailable, true);

            loTable = ((SearchStructLogicAndroid)(loHotSheetStruct.StructLogicObj)).PerformSearch(
                this, true, 1, EditRestrict.MatchFieldsName, false,
                EditRestrict, ref loWirelessHostAvailable, true);



            // in Android, we're done here for now - the rest of the processing happens 
            // after the dialog results are shown and the user interaction is finished
            return;


            // It is possible the wireles results have now been processed. 
            // If so, do not allow the local results to erase them.
            if (EditRestrict.fPreemptedByWireless)
                return;



#if _original_
            if (loTable == null)
            {
                // This needs to set a filter the excludes all permits?
                loParentTable.RemoveAllFilters();
                loParentTable.ActivateFilters();
                return;
            }

            // Have a result, paste it to parent edit
            loParentTable.CopyFilters(loTable.GetFilter());

            if (EditRestrict.GetParent() != null)
            {
                EditRestrict.GetParent().ListItemCount = loParentTable.GetRecCount();
                EditRestrict.GetParent().ListItemCache.Clear();
                if (EditRestrict.GetParent().GridDisplayCache != null)
                    EditRestrict.GetParent().GridDisplayCache.Clear();
                if (EditRestrict.GetParent().ListBox != null)
                    EditRestrict.GetParent().ListBox.RefreshItems(false);
            }
#endif

           
            

#if _original_
            EditRestrict.GetParent().SetEditBufferAndPaint(ReinoControls.TextBoxBehavior.GetFieldDataFromTable(
                loParentTable, EditRestrict.GetParent().CfgCtrl.Name,
                loTable.GetCurRecNo(), EditRestrict.GetParent().GetEditMask()));
#endif
            return;
        }

        private THotSheetStruct GetHotSheetStruct(string StructName)
        {
            foreach (TIssStruct NextIssStruct in DroidContext.XmlCfg.clientDef.IssStructMgr.IssStructs)
            {
                if ((NextIssStruct is THotSheetStruct) && (string.Compare(NextIssStruct.Name, StructName, true) == 0))
                {
                    return NextIssStruct as THotSheetStruct;
                }
            }
            return null;
        }





        public void LoadSearchMatchFragment(TSearchMatchForm iSearchMatchForm, SearchParameterPacket iSearchResults)
        //List<SearchMatchDTO> iResultsList)
        {
            //FragmentManager fm = ((Activity)context).FragmentManager;


            // AJW TODO - ParentFragment.Activity will be NULL after the app is minimized and restored - need to know how to deal with this scenario
            if ((ParentFragment == null) || (ParentFragment.Activity == null))
            {

                // to reproduce, log in, do some searches, log out, then log in again, do search
                // the 2nd time login the fragment is still running but now the original activity is null

                // 7.24.05 fixed by forcing the reload of the XML each time MainActity is restarted. Without it old objects were still being referenced

                LoggingManager.LogApplicationError(null, "LoadSearchMatchFragment", "ParentFragment.Activity is null");
                Console.WriteLine("Exception caught in process: {0} {1}", "LoadSearchMatchFragment", "ParentFragment.Activity is null");


                // we need to shut down and restart
                return;
            }


            try
            {

                FragmentManager fm = ParentFragment.Activity.FragmentManager;
                FragmentTransaction fragmentTransaction = fm.BeginTransaction();

                // this is the data type we need
                string loTargetFragmentTag = Helper.BuildSearchMatchFragmentTag(thisStruct.Name);


                // always new dialog created
                SearchMatchFragment oneSearchMatchFragment = new SearchMatchFragment { Arguments = new Bundle() };
                oneSearchMatchFragment.Arguments.PutString("structName", thisStruct.Name);

                oneSearchMatchFragment.SetSearchMatchResults(iSearchMatchForm, iSearchResults);

                oneSearchMatchFragment.Show(fm, loTargetFragmentTag);

                //oneSearchMatchFragment.CustomizeSearchMatchDialog();

                fragmentTransaction.Commit();

                // we need to see this dialog immediately
                fm.ExecutePendingTransactions();
            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "", "LoadSearchMatchFragment");
                Console.WriteLine("Exception caught in process: {0} {1}", exp, "LoadSearchMatchFragment");
            }


        }



        //public void LoadSearchMatchFragment(TSearchMatchForm iSearchMatchForm, List<SearchMatchDTO> iResultsList)
        //{
        //    //FragmentManager fm = ((Activity)context).FragmentManager;


        //    // AJW TODO - ParentFragment.Activity will be NULL after the app is minimized and restored - need to know how to deal with this scenario


        //    FragmentManager fm = ParentFragment.Activity.FragmentManager;


        //    FragmentTransaction fragmentTransaction = fm.BeginTransaction();

        //    Fragment oneCommonFragment = fm.FindFragmentByTag(ParentFragment.GetFragmentTagName());
        //    if (oneCommonFragment != null)
        //    {
        //        fragmentTransaction.Hide(oneCommonFragment);
        //    }

        //    // come back when you're done there
        //    DroidContext.MyFragManager.AddToInternalBackstack( ParentFragment.GetFragmentTagName() );


        //    // this is the data type we need
        //    string loTargetFragmentTag = Helper.BuildSearchMatchFragmentTag( thisStruct.Name );

        //    var oneSearchMatchFragment = (SearchMatchFragment)FragmentManager.FindFragmentByTag(loTargetFragmentTag);
        //    if (oneSearchMatchFragment != null)
        //    {
        //        fragmentTransaction.Show(oneSearchMatchFragment);
        //        oneSearchMatchFragment.SetSearchMatchResults(iSearchMatchForm, iResultsList);
        //        fragmentTransaction.Commit();
        //    }
        //    else
        //    {
        //        // first time through
        //        oneSearchMatchFragment = new SearchMatchFragment { Arguments = new Bundle() };
        //        oneSearchMatchFragment.Arguments.PutString("structName", thisStruct.Name);

        //        fragmentTransaction.Add(Resource.Id.frameLayout1, oneSearchMatchFragment, loTargetFragmentTag);
        //        fragmentTransaction.Show(oneSearchMatchFragment);
        //        fragmentTransaction.Commit();

        //        // register it - as secondary activity (not on a direct menu)
        //        ((MainActivity)ParentFragment.Activity).RegisterFragment(oneSearchMatchFragment, loTargetFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

        //        oneSearchMatchFragment.SetSearchMatchResults(iSearchMatchForm, iResultsList);
        //    }

        //    //fragmentTransaction.Commit();
        //}
           


        private void OnRestrictionForcesDisplayRebuild()
        {
            // If we are working within the context of "FormInit" mode, then don't do this stuff
            // yet. We'll just set a flag so we know it still needs to be done later...
            //do stuff here
            var hello = "Hello!";
        }


        private void OnListContentsChangedByRestriction(EditControlBehavior iBehavior)
        {
            iBehavior.ListContentsChangedByRestriction = true;
        }


        private bool SetIssueNoFields()
        {
#if _implementation_
            // This is only applicable for cite structures
            if (!(IssueStruct is TCiteStruct))
                return true;

            // If this is a new entry and there is an issue number field, need to get next issue number 
            if (((fFormEditMode & (EditRestrictionConsts.femReissue | EditRestrictionConsts.femContinuance |
                EditRestrictionConsts.femNewEntry | EditRestrictionConsts.femIssueMoreAttr)) > 0) &&
                 (IssueNoCtrl != null))
            {
                // Find the associated sequence object
                TCiteStruct CiteStruct = IssueStruct as TCiteStruct;
                TObjBasePredicate predicate = new TObjBasePredicate(CiteStruct.Sequence);
                SequenceImp SeqObj =
                    Reino.CommonLogic.SequenceManager.GlobalSequenceMgr.Sequences.Find(predicate.CompareByName_CaseInsensitive);

                // Return false if there is no sequence to work with
                if (SeqObj == null)
                {
                    AppMessageBox.ShowMessageWithBell("Could not find", "issue number sequence!", "");
                    return false;
                }

                // Do we have an IssueNo field?
                if (IssueNoCtrl != null)
                {
                    Int64 loIssueNo = SeqObj.GetNextNumber();
                    // Did GetNextNumber fail?
                    if (loIssueNo <= 0)
                    {
                        AppMessageBox.ShowMessageWithBell("No numbers available in", "issue number sequence!", "");
                        return false;
                    }
                    IssueNoCtrl.Behavior.SetEditBufferAndPaint(Convert.ToString(loIssueNo));
                }

                // Do we have an IssueNoPfx field?
                if (IssueNoPfxCtrl != null)
                    IssueNoPfxCtrl.Behavior.SetEditBufferAndPaint(SeqObj.GetNextNumberPfx());

                // Do we have an IssueNoSfx field?
                if (IssueNoSfxCtrl != null)
                    IssueNoSfxCtrl.Behavior.SetEditBufferAndPaint(SeqObj.GetNextNumberSfx());
            }
#endif
            return true;
        }


        #endregion





        // Barcode scan/parser debug - uses barcode sample raw data
        /*
        public void btnScanClick(object sender, EventArgs e)
        {
            String loTestStr = "";
            TBarCodeParser loBarCodeParser;
            try
            {
                loBarCodeParser = new TBarCodeParser();
                loTestStr = loBarCodeParser.GetSampleData();
                BarCodeCommons.TBarCodeType loBarCodeType = loBarCodeParser.QueryUserChooseBarCodeDataDestination(loTestStr);
                if (loBarCodeType == BarCodeCommons.TBarCodeType.Bar_2D)
                {
                    //2D barcode should be populated here
                    BarCodeConfirmResultFragment.gPanelRootPageLinearLayout = this.PanelRootPageLinearLayout;
                    BarCodeCommons.TBarCodeResultData loBarCodeResultData = loBarCodeParser.GetBarCodeResultDataFields();
                    var loIntent = new Intent((Activity)context,typeof(BarCodeConfirmResultFragment));
                    List<String> loElementList = loBarCodeParser.GetBarCodePreviewList()();
                    loIntent.PutStringArrayListExtra(Constants.ACTIVITY_INTENT_CONFIRM_BARCODE_FIELDSNAMES_KEY, loBarCodeResultData.FieldsNamesList);
                    loIntent.PutStringArrayListExtra(Constants.ACTIVITY_INTENT_CONFIRM_BARCODE_FIELDSVALUES_KEY, loBarCodeResultData.FieldsValuesList);
                    loIntent.PutStringArrayListExtra(Constants.ACTIVITY_INTENT_CONFIRM_BARCODE_PREVIEWLIST_KEY, loElementList);
                    ((Activity)context).StartActivity(loIntent);
                     
                    return;
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Failed to scan barcode:" + exp.Message);
            }
        }
        */

        public void HideKeyBoardFromCurrentView()
        {
            // hide the keyboard if it was open
            object uiCurrentComponent = thisStruct.Panels[PanelPageCurrentDisplayedIndex].FocusedViewCurrent;
            if (uiCurrentComponent != null)
            {
                if (uiCurrentComponent is View)
                {
                    Helper.HideKeyboard((View)uiCurrentComponent);
                }
            }
        }

        public async void btnScanClick(object sender, EventArgs e)
        {

#if _enable_ZXing_

            String loRawData = "";
            //Make sure that the keyboard is hidden before we start the camera
            HideKeyBoardFromCurrentView();            
            try
            {

//#if !_integrate_n5_support_
                var loScanner = new ZXing.Mobile.MobileBarcodeScanner();



#if _autofocus_
                // limit the supported formats for faster scanning
                var scanOptions = new ZXing.Mobile.MobileBarcodeScanningOptions();
                scanOptions.PossibleFormats = new List<BarcodeFormat>()
                { 
                    ZXing.BarcodeFormat.CODE_128,
                    ZXing.BarcodeFormat.CODE_39,
                    ZXing.BarcodeFormat.PDF_417
                };

                //scanOptions.CameraResolutionSelector = new MobileBarcodeScanningOptions.CameraResolutionSelectorDelegate();

                loScanner.BottomText = "Touch screen to re-focus";
                loScanner.AutoFocus();

                
                var loResult = await loScanner.Scan( scanOptions );
#else
                loScanner.BottomText = "Touch screen to re-focus";
                var loResult = await loScanner.Scan();
#endif
                if (loResult == null)
                {
                    return;
                }
                loRawData = loResult.Text;
//N5 Scan support is disabled as it is broken. For now we will use the camaera imager to capture barcodes.
// The H/W BARCODE button in N5 is also directed to here to use the camera imager
//#else  
//			    //N5 scan code, for now we will use sample data
//                //loRawData = loBarCodeParser.GetSampleData();
                
//                BarCodeScanFragment_N5.gPanelRootPageLinearLayout = this.PanelRootPageLinearLayout;
//                var loN5Intent = new Intent((Activity)context, typeof(BarCodeScanFragment_N5));           
//                ((Activity)context).StartActivity(loN5Intent);     
//                return;
//#endif
                //Now parse the barcode and populate the fields
                ParseScannedBarCode(loRawData);

            }
            catch (Exception exp)
            {
                Console.WriteLine("Failed to scan barcode:" + exp.Message);
            }
#endif                
        }
       
        //Common method to parse scanned barcode
        public void ParseScannedBarCode(String iRawData)
        {
            
            if(String.IsNullOrEmpty(iRawData))
                return;
            TBarCodeParser loBarCodeParser;

            Console.WriteLine("Scanned Barcode: " + iRawData);


            loBarCodeParser = new TBarCodeParser();
            try
            {
                BarCodeCommons.TBarCodeType loBarCodeType = loBarCodeParser.QueryUserChooseBarCodeDataDestination(iRawData);

                if (loBarCodeType == BarCodeCommons.TBarCodeType.Bar_None)
                {
                    return;
                }

                if (loBarCodeType == BarCodeCommons.TBarCodeType.Bar_2D)
                {
                    //2D barcode should be populated here
                    BarCodeConfirmResultFragment.gPanelRootPageLinearLayout = this.PanelRootPageLinearLayout;
                    BarCodeCommons.TBarCodeResultData loBarCodeResultData = loBarCodeParser.GetBarCodeResultDataFields();
                    var loIntent = new Intent((Activity)context, typeof(BarCodeConfirmResultFragment));
                    List<String> loElementList = loBarCodeParser.GetBarCodePreviewList();
                    loIntent.PutStringArrayListExtra(Constants.ACTIVITY_INTENT_CONFIRM_BARCODE_FIELDSNAMES_KEY, loBarCodeResultData.FieldsNamesList);
                    loIntent.PutStringArrayListExtra(Constants.ACTIVITY_INTENT_CONFIRM_BARCODE_FIELDSVALUES_KEY, loBarCodeResultData.FieldsValuesList);
                    loIntent.PutStringArrayListExtra(Constants.ACTIVITY_INTENT_CONFIRM_BARCODE_PREVIEWLIST_KEY, loElementList);
                    ((Activity)context).StartActivity(loIntent);
                    return;
                }

                //It should be 1D barcode, got ahead and update the focused panel field
                if (
                    (thisStruct.Panels != null) &&
                    (thisStruct.Panels.Count > PanelPageCurrentDisplayedIndex) &&
                    (thisStruct.Panels[PanelPageCurrentDisplayedIndex].PanelFields != null)
                    )
                {
                    Panel loCurrentPanel = thisStruct.Panels[PanelPageCurrentDisplayedIndex];
                    object uiComponent = loCurrentPanel.FocusedViewCurrent;


                    if (uiComponent != null)
                    {
                        if (uiComponent is CustomEditText)
                        {
                            CustomEditText customEditText = (CustomEditText)uiComponent;
                            if (customEditText != null)
                            {
                                // insert the scanned string
                                customEditText.SetText(iRawData, TextView.BufferType.Normal);
                            }
                        }
                        else if (uiComponent is CustomAutoTextView)
                        {
                            CustomAutoTextView customView = (CustomAutoTextView)uiComponent;
                            if (customView != null)
                            {
                                // Insert the scanned string in the field
                                customView.SetListItemDataByText(iRawData);
                                customView.SetText(iRawData, false);
                            }

                        }
                    }

                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Failed to parse barcode:" + exp.Message);
            }

        }
    }   // end class FormPanel
}