using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Animation;

using Android.Support.V7.View.Menu;

using System.Data;
using System.IO;
using Java.IO;

using Duncan.AI.Droid.Common;
using Duncan.AI.Droid.Utils;
using Android.Util;
using Android.Preferences;
using System.Threading.Tasks;
using Duncan.AI.Droid.Utils.HelperManagers;

// AJW added for immediate printing - temp until revamped
using Android.Bluetooth;
using Com.Zebra.Android.Printer;
using Com.Zebra.Android.Comm;
using System.Threading;
using System.Threading.Tasks;

// AJW added for N5Print
using Reino.ClientConfig;
using Duncan.AI.Droid.Utils.PrinterSupport;


namespace Duncan.AI.Droid
{
    public class CommonFragment : Android.App.Fragment, View.IOnTouchListener
    {
        XMLConfig.IssStruct _struct;


        public Reino.ClientConfig.TIssStruct fSourceDataStruct = null;
        public DataRow fSourceDataRawRow = null;
        public int fSourceDataRowFormEditMode = Reino.ClientConfig.EditRestrictionConsts.femNewEntry;
        public IssueStructLogicAndroid fSourceIssueStructLogic = null;


        string _tagName;
        string _structName;
        string _menuItem;
        int _IssueFormIdx;

        public string GetFragmentTagName()
        {
            return _tagName;
        }

        public string GetFragmentMenuItemName()
        {
            return _menuItem;
        }


        LinearLayout _layout;
        ScrollView _scrollview_root;

        Toolbar _AutoISSUE_Toolbar_Bottom;
        LinearLayout _AutoISSUE_Toolbar_Confirmation;



        TextView oneTextViewTicketPreviewDivider = null;
        //EditText oneFocusableField = null;
        Bitmap bmpTicketPreview = null;
        ImageView imageViewTicketPreview = null;


        DisplayMetrics _metrics;
        View _view;

        Touch _touch = new Touch();
        public FormPanel _formPanel;
        ISharedPreferences _prefs;

        ProgressDialog _progressDialog;

      

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //string structName = Arguments.GetString("structName", null);

            //((MainActivity)Activity).RegisterFragment(this, structName);

          
        }


        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }


        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
        }




        //public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        //{
        //    return base.OnCreateView(inflater, container, savedInstanceState);
        //}

        public enum AIToolbarMode
        {
            tbmNoneShown = 0,
            tbmEditing,
            tbmFinalizing
        };



        private void SetToolbarMode(AIToolbarMode iToolbarMode)
        {
            switch (iToolbarMode)
            {
                case AIToolbarMode.tbmNoneShown:
                    {
                        _AutoISSUE_Toolbar_Bottom.Visibility = ViewStates.Gone;
                        _AutoISSUE_Toolbar_Confirmation.Visibility = ViewStates.Gone;
                        break;
                    }

                case (AIToolbarMode.tbmFinalizing):
                    {
                        _AutoISSUE_Toolbar_Bottom.Visibility = ViewStates.Gone;
                        _AutoISSUE_Toolbar_Confirmation.Visibility = ViewStates.Visible;
                        break;
                    }

                default:  // incl  AIToolbarMode.tbmEditing
                    {
                        _AutoISSUE_Toolbar_Confirmation.Visibility = ViewStates.Gone;
                        _AutoISSUE_Toolbar_Bottom.Visibility = ViewStates.Visible;
                        break;
                    }
            }
        }





        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            // AJW - always call the superclass
            base.OnCreateView(inflater, container, savedInstanceState);


            try
            {
                _metrics = Resources.DisplayMetrics;


                _structName = Arguments.GetString("structName", null);
                _IssueFormIdx = Arguments.GetInt("issueFormNdx", 1);
                _menuItem = Arguments.GetString("menuItem", "");


                if (_IssueFormIdx < 2)
                {
                    // standard single form struct
                    _struct = DroidContext.XmlCfg.GetStructByName(_structName);
                    _tagName = Helper.BuildIssueNewFragmentTag(_structName);
                }
                else
                {
                    // we'll need to reference the copy created for issue2
                    _struct = DroidContext.XmlCfg.GetStructByNameHavingIssue2(_structName);

                    // and tag to match
                    _tagName = Helper.BuildIssueNew2FragmentTag(_structName);
                }



                _formPanel = new FormPanel();
                _formPanel.ParentFragment = this;


                _prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);




                // subscribe to know when the new panel is done building
                _formPanel.PropertyChanged += LayoutReady;
                _view = inflater.Inflate(Resource.Layout.Common_fragment, null);





                _layout = _view.FindViewById<LinearLayout>(Resource.Id.linearLayout1);
                _layout.SetOnTouchListener(this);
                //_layout.SetPadding(20, 0, 20, 20);
                _layout.SetPadding(20, 0, 20, 800);
                _layout.Orientation = Orientation.Vertical;

                _scrollview_root = _view.FindViewById<ScrollView>(Resource.Id.scrollview_root);


                ////

                // the editing toobar is used for quick access
                _AutoISSUE_Toolbar_Bottom = _view.FindViewById<Toolbar>(Resource.Id.toolbar_bottom);
                _AutoISSUE_Toolbar_Bottom.Title = "";
                _AutoISSUE_Toolbar_Bottom.InflateMenu(Resource.Menu.autoissue_toolbar);

                _AutoISSUE_Toolbar_Bottom.SetBackgroundResource(Resource.Drawable.autoissue_toolbar_background);

                ////////////

                DoToolbarLayout(_AutoISSUE_Toolbar_Bottom);



                //////////

                _AutoISSUE_Toolbar_Bottom.MenuItemClick += toolbarBottom_MenuItemClick;


                ////

                // the submit toolbar give is shown at the end of the entry process
                _AutoISSUE_Toolbar_Confirmation = _view.FindViewById<LinearLayout>(Resource.Id.toolbar_bottom_confirm);
                _AutoISSUE_Toolbar_Confirmation.SetBackgroundResource(Resource.Drawable.autoissue_toolbar_finalizing_background);


                // hook in the click events to actions
                Button loSubmit = _view.FindViewById<Button>(Resource.Id.btnConfirm);

                // initialize our typeface 
                Helper.SetTypefaceForButton(loSubmit, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);
                loSubmit.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_black));
                loSubmit.Text = "PRINT";  // 
                loSubmit.Click += BtnSaveParkingClick;

                Button loBackToEdit = _view.FindViewById<Button>(Resource.Id.btnBackToEdit);
                Helper.SetTypefaceForButton(loBackToEdit, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);
                loBackToEdit.SetTextColor(Activity.Resources.GetColor(Resource.Color.civicsmart_black));
                loBackToEdit.Click += btnReturnToEditClick;

                ////



                // show or hide the appropriate toolbars
                SetToolbarMode(AIToolbarMode.tbmEditing);

                StartTicketLayout();

                return _view;
            }
            catch
            {
                return null;
            }
        }


        void toolbarBottom_MenuItemClick(object sender, Toolbar.MenuItemClickEventArgs e)
        {
            // redirect to the implementation, once the panel has been initialized
            if (_formPanel != null)
            {

                if (e.Item != null)
                {
                    // hook the activity level functions first
                    switch (e.Item.ItemId)
                    {

                        case Resource.Id.ai_toolbar_back_to_edit:
                            {
                                btnReturnToEditClick( sender, e );
                                break;
                            }

                        case Resource.Id.ai_toolbar_confirm_and_submit:
                            {
                                BtnSaveParkingClick(sender, e);
                                break;
                            }


                        default:
                            {
                                // call the form editing 
                                _formPanel.OnAutoISSUEToolbarClick(sender, e);
                                break;
                            }
                    }


                }
            }

        }



        
        // remember (int) pixels != dp  - a conversion function would look like this 

        //public int dp2px(int dp)
        //{
        //    return (int)TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, dp, context.getResources().getDisplayMetrics());
        //}


        private void DoToolbarLayout(Toolbar iToolbar)
        {

            // Add 10 spacing on either side of the toolbar
            //iToolbar.SetContentInsetsAbsolute(10, 10);
            iToolbar.SetContentInsetsAbsolute(0, 0);
            iToolbar.SetContentInsetsRelative(0, 0);

            ////iToolbar.SetClipToPadding(false);


            //int loPaddingLeft = iToolbar.PaddingLeft;
            //int loPaddingRight = iToolbar.PaddingRight;

            //loPaddingRight = iToolbar.MinimumWidth;


            //iToolbar.SetPadding(loPaddingLeft, iToolbar.PaddingTop, loPaddingRight, iToolbar.PaddingBottom);

            //iToolbar.SetMinimumWidth = 0;


            // Get the ChildCount of your Toolbar, this should only be 1
            int childCount = iToolbar.ChildCount;
            // Get the Screen Width in pixels
            int screenWidth = _metrics.WidthPixels;

            // Create the Toolbar Params based on the screenWidth
            //Toolbar.LayoutParams toolbarParams = new Toolbar.LayoutParams(screenWidth, Toolbar.LayoutParams.WrapContent);

            // Loop through the child Items
            for (int i = 0; i < childCount; i++)
            {
                // Get the item at the current index
                View childView = iToolbar.GetChildAt(i);
                // If its a ViewGroup
                if (childView is ViewGroup)
                {
                    ViewGroup.LayoutParams loExistinglp = childView.LayoutParameters;

                    // Set its layout params
                    //childView.SetLayoutParams(toolbarParams);

                    loExistinglp.Width = screenWidth;
                    loExistinglp.Height = ViewGroup.LayoutParams.WrapContent;
                    childView.RequestLayout();


                    // Get the child count of this view group, and compute the item widths based on this count & screen size
                    int innerChildCount = ((ViewGroup)childView).ChildCount;
                    int itemWidth = (screenWidth / innerChildCount);

                    if (screenWidth <= 1080)
                    {
                       // customize for smaller screen?
                    }


                    // Create layout params for the ActionMenuView
                    //ActionMenuView.LayoutParams loParams = new ActionMenuView.LayoutParams(itemWidth, ActionMenuView.LayoutParams.WrapContent);

                    // Loop through the children
                    for (int j = 0; j < innerChildCount; j++)
                    {
                        View grandChild = ((ViewGroup)childView).GetChildAt(j);

                        //if (grandChild is ActionMenuItemView)

                        //if (grandChild is com.android.internal.view.menu.ActionMenuItemView)
                        //if (grandChild is Android.Views.int com.android.internal.view.menu.ActionMenuItemView)

                        //if (grandChild != null)
                        if (grandChild is Android.Widget.TextView)
                            {
                            // set the layout parameters on each View
                            //grandChild.setLayoutParams(loParams);
                            ViewGroup.LayoutParams lolp = grandChild.LayoutParameters;
                            lolp.Width = itemWidth;
                            lolp.Height = ViewGroup.LayoutParams.WrapContent;

                            //grandChild.SetPadding(0, 0, 0, 0);
                            grandChild.RequestLayout();

                        }
                    }
                }
            }




        }

        public async void StartTicketLayout()
        {
            try
            {
                // AJW TODO - some structures don't have visible panels... but do we want to re-direct earlier in the process?
                if (_struct.Panels.Count > 0)
                {
                    // go back to the start
                    _struct.PanelPageCurrentDisplayedIndexSavedForResume = 0;

                    /* AJW - we are starting a NEW form, but we MUST save to refresh/init sequence numbers */
                    var success = await _formPanel.SaveCurrentDisplayPage(_struct, _layout, _scrollview_root, 0, Activity, _metrics);

                    if (success)
                    {
                        _formPanel.CreateOrShowDisplayPage(_struct.PanelPageCurrentDisplayedIndexSavedForResume);
                        _formPanel.PrepareForEditAndroid();
                    }
                }
                else
                {
                    // and a debug point
                    System.Console.WriteLine("INFO: Struct " + _struct.Name + " has 0 panels" );
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("Exception in StartTicketLayout---", e.Message);
                System.Console.WriteLine("Exception StartTicketLayout: {0}", e.Source);
                System.Console.WriteLine("e.Message: {0}", e.Message);
            }            
        }

        private View RenderIssueStructPrintPreview()
        {
            
            var commonADO = new CommonADO();
            // AJW - TODO - these are getting wiped in UpdateRow, but we need them
            string loSavedSequenceId = _struct.sequenceId;
            string loSavedStructName = _struct.Name;


            // the DB is updated every panel change, dont need another

           // ParkingSequenceADO.UpdateInUseSequenceId(_struct.sequenceId, Constants.SRCINUSE_FLAG_SUBMITTED, _struct.SequenceName);
           // bool loUpdateRowResult = await commonADO.UpdateRow(_struct, Constants.STATUS_READY, Constants.WS_STATUS_READY, Activity, _struct.Name);

            // AJW - review/todo do for universal ticket types


            if (bmpTicketPreview != null)
            {
                // clean up the last one before creating the next
                bmpTicketPreview.Recycle();
                // added in 7.25.06 - does it actually help? need a mem profile before/after
                bmpTicketPreview.Dispose();
                bmpTicketPreview = null;
            }



            if (_struct.PrintPicture != null )
            //if (_struct.Name.Equals("PARKING"))
            {

                // AJW - this should be using value from TIssStruct/Panels
                var myPrintMgr = new PrintManager();
                bmpTicketPreview = myPrintMgr.PrintPicture(loSavedStructName, loSavedSequenceId, DateTime.Now);
            }


            // file extension is added by reader
            //string loTicketImageName = Helper.GetTIssueFormBitmapImageFileName(_structName, _parkingDTO.sqlIssueNumberStr, DateTime.Today); // TODOD -needs datetime in proper formats
            //string loTicketImageName = Helper.GetTIssueFormBitmapImageFileName(loSavedStructName, loSavedSequenceId, DateTime.Today); // TODOD -needs datetime in proper formats


            // remove any previous rendering
            if (oneTextViewTicketPreviewDivider != null)
            {
                _formPanel.PanelRootPageLinearLayout.RemoveView(oneTextViewTicketPreviewDivider);
                oneTextViewTicketPreviewDivider = null;
            }

            if (imageViewTicketPreview != null)
            {
                _formPanel.PanelRootPageLinearLayout.RemoveView(imageViewTicketPreview);
                imageViewTicketPreview = null;
            }

            //if (oneFocusableField != null)
            //{
            //    _formPanel.PanelRootPageLinearLayout.RemoveView(oneFocusableField);
            //    oneFocusableField = null;
            //}

            
            // construct a divider to help in navigation
            oneTextViewTicketPreviewDivider = new TextView(Activity);
            oneTextViewTicketPreviewDivider.Text = "Final Review and Confirmation";
            oneTextViewTicketPreviewDivider.TextAlignment = TextAlignment.Center | TextAlignment.Gravity;

            oneTextViewTicketPreviewDivider.SetTextColor(Color.White);
            oneTextViewTicketPreviewDivider.SetBackgroundColor(Color.DarkSlateGray);

            // setup our margins according to our placement
            var lpPanelDivider = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            lpPanelDivider.SetMargins(0, 0, 0, 50);

            // add our divider
            _formPanel.PanelRootPageLinearLayout.AddView(oneTextViewTicketPreviewDivider, lpPanelDivider);

            //// test another
            //oneFocusableField = new EditText(Activity);
            //_formPanel.PanelRootPageLinearLayout.AddView(oneFocusableField, lpPanelDivider);

            ////






            var imageViewLayoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
                                                                   LinearLayout.LayoutParams.WrapContent);
            imageViewLayoutParams.SetMargins(0, 5, 0, 5);

            // TODO - determine white space on pre-printed forms and ajdust image accordinlgly
            //        <ImageView
            //            android:layout_width="match_parent"
            //            android:layout_height="wrap_content"
            //            android:layout_marginTop="5dp"

            //            android:id="@+id/imageReproduction"
            //            android:scaleType="fitCenter"
            //            android:adjustViewBounds="true"

            //            android:layout_marginBottom="5dp"
            //            android:background="@drawable/parking_ticket_placeholder" />
            //    </LinearLayout>


            imageViewTicketPreview = new ImageView(Activity);


            // AJW - TODO why are we setting the dummy image here?

           // imageViewTicketPreview.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.parking_ticket_placeholder));
            imageViewTicketPreview.SetBackgroundResource(Resource.Drawable.parking_ticket_placeholder);



            //// for debug only, show alternate views
            //var sigImg2 = _view.FindViewById<ImageView>(Resource.Id.sigImg);
            //sigImg2 = Helper.GetTIssueFormBitmapImageFromStorage(sigImg2, loTicketImageName);

            //var reproductionImage = _view.FindViewById<ImageView>(Resource.Id.reproductionImage);
            //var reproductionImage = _view.FindViewById<ImageView>(Resource.Id.imageReproduction2);
            //imageViewTicketPreview = Helper.GetTIssueFormBitmapImageFromStorage(imageViewTicketPreview, loTicketImageName);



            imageViewTicketPreview.SetImageBitmap(bmpTicketPreview);

            imageViewTicketPreview.SetScaleType(ImageView.ScaleType.FitCenter);
            imageViewTicketPreview.SetAdjustViewBounds(true);


            //imageViewTicketPreview.SetTextColor(Android.Graphics.Color.White);
            //imageViewTicketPreview.Text = "SUBMIT TICKET";
            //imageViewTicketPreview.Click += BtnSaveParkingClick;
            _formPanel.PanelRootPageLinearLayout.AddView(imageViewTicketPreview, imageViewLayoutParams);


            // this will pull the screen focus down
            imageViewTicketPreview.RequestFocusFromTouch();

            // this is the last page, make sure the keyboard is down
            Helper.HideKeyboard(imageViewTicketPreview);

            // give the page a focus
            //_AutoISSUE_Toolbar_Confirmation.RequestFocusFromTouch();


            // then give NOTHING the focus so we don't have odd selections
            View loCurrentFocus = _formPanel.CurrentFocus;
            if (loCurrentFocus != null)
            {
                loCurrentFocus.ClearFocus();
            }

            return oneTextViewTicketPreviewDivider;
            //return oneFocusableField;
        }


        public void btnReturnToEditClick( object sender, EventArgs e)
        {
            _formPanel.btnPreviousClick(null, e);
        }

        private void LayoutReady(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_formPanel.IsPanelBuilt)
            {


                DoToolbarLayout(_AutoISSUE_Toolbar_Bottom);




                _struct.PanelPageCurrentDisplayedIndexSavedForResume = _formPanel.PanelPageCurrentDisplayedIndex;  // Save it out here becuase the formPanel may go away

                // does this if we are going forward, atuo go until the next required field
                if (_formPanel.PanelRootPageLinearLayout.ChildCount == 0 
                    //todo - ptu this back in when we want to auto-skip the pages that the user doestn care about. 
                    //not doing this now since all fo the force clear items arent in place and working.
                   // ||(_formPanel.PanelDelta == 1 && (_struct.CurrentPanel < _struct.Panels.Count - 1))

                    )
                {

                    // make sure we aren't at the edges before we try to skip
                    if (
                        (_formPanel.PanelPageDelta == 1) &&
                         (_formPanel.PanelPageCurrentDisplayedIndex < _formPanel.thisStruct.Panels.Count - 1)
                        )
                    {
                        _formPanel.SkipMode = true;
                        _formPanel.ChangeCurrentDisplayPage(_formPanel.PanelPageDelta); // keep going in the same direction
                        return;
                    }

                    if (
                        (_formPanel.PanelPageDelta == -1) &&
                         (_formPanel.PanelPageCurrentDisplayedIndex > 0)
                        )
                    {
                        _formPanel.SkipMode = true;
                        _formPanel.ChangeCurrentDisplayPage(_formPanel.PanelPageDelta); // keep going in the same direction
                        return;
                    }


                }








                //else if (_formPanel.PanelPageCurrentDisplayedIndex == _formPanel.thisStruct.Panels.Count - 1) //  if this is the last panel
                if (_formPanel.PanelPageCurrentDisplayedIndex == _formPanel.thisStruct.Panels.Count - 1) //  if this is the last panel
                {


                    var layoutParamsButton = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
                                                                           LinearLayout.LayoutParams.WrapContent);
                    layoutParamsButton.SetMargins(30, 30, 30, 30);


       // no specialty buttons anymore - using the common toolbar instead
                    //if (_struct.Type == Constants.STRUCT_TYPE_ACTIVITYLOG)
                    //{
                    //    var submitBtn = new Button(Activity);
                    //    submitBtn.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.button));
                    //    submitBtn.SetTextColor(Android.Graphics.Color.White);
                    //    submitBtn.Text = "SUBMIT";
                    //    submitBtn.Click += BtnSubmitClick;
                    //    _formPanel.PanelRootPageLinearLayout.AddView(submitBtn, layoutParamsButton);
                    //}



                    if (_struct.PrintPicture != null)
                    //if (_struct.Type == Constants.STRUCT_TYPE_CITE)
                    {

                        // hide the toolbar
                        SetToolbarMode(AIToolbarMode.tbmNoneShown);

                        // this is the last page, make sure the keyboard is down
                        //Helper.HideKeyboard(_AutoISSUE_Toolbar_Confirmation);

                        // this will pull the screen focus down
                        //btnSave.RequestFocusFromTouch();

                        View loFocusableItem = RenderIssueStructPrintPreview();

                        // show the toolbar for
                        SetToolbarMode(AIToolbarMode.tbmFinalizing);

                        // screen scroll back to the top
                        // Defer the scroll code until the form is rendered - this delay is needed because of the transition animation
                        _formPanel.PanelRootPageScrollView.PostDelayed(new RunnableAnonymousScrollViewClassHelper(_formPanel.PanelRootPageScrollView, 0, 0), 450);



                        /////



                        // AJW - for better presentation, no notes until after ticket is completed
                        //var btnNotes = new Button(Activity);
                        //btnNotes.Visibility = ViewStates.Gone;
                        //btnNotes.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.button_secondary));
                        //btnNotes.SetTextColor(Android.Graphics.Color.White);
                        //btnNotes.Text = "NOTES (with Media)";
                        //btnNotes.Click += BtnNotesClick;
                        //_formPanel.PanelRootPageLinearLayout.AddView(btnNotes, layoutParamsButton);
                    }

                    if (_struct.Name == Constants.STRUCT_NAME_CHALKING)
                    {

#if _old_markmode_

                        var btnSave = new Button(this.Activity);
                        btnSave.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.button));
                        btnSave.SetTextColor(Android.Graphics.Color.White);
                        btnSave.Text = "SAVE";
                        btnSave.Click += BtnSaveMarkmodeClick;
                        _formPanel.PanelRootPageLinearLayout.AddView(btnSave, layoutParamsButton);

                        // this will pull the screen focus down
                        btnSave.RequestFocusFromTouch();
                        // this is the last page, make sure the keyboard is down
                        Helper.HideKeyboard(btnSave);


                        for (int i = 0; i < _struct.Panels.Count; i++)
                        {
                            foreach (XMLConfig.PanelField panelField in _struct.Panels[i].PanelFields)
                            {
                                if (panelField.Name == Constants.TIRESTEMSFRONTTIME_COLUMN
                                    || panelField.Name == Constants.TIRESTEMSREARTIME_COLUMN)
                                {
                                    var btnStem = new Button(Activity);
                                    btnStem.SetBackgroundDrawable(
                                        Resources.GetDrawable(Resource.Drawable.button_secondary));
                                    btnStem.SetTextColor(Android.Graphics.Color.White);
                                    btnStem.Text = "Stem Picker";
                                    btnStem.Click += BtnStemClick;
                                    _formPanel.PanelRootPageLinearLayout.AddView(btnStem, layoutParamsButton);
                                    break;
                                }
                            }
                        }
#endif



                    }
                }
                else
                {
                    // not the last page
                    SetToolbarMode(AIToolbarMode.tbmEditing);
                }
            }
        }





/* deprecated
        void BtnNotesClick(object sender, EventArgs e)
        {
            // save where we came from
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            ISharedPreferencesEditor editor = prefs.Edit();

            editor.PutString(Constants.PREVIOUS_FRAGMENT, _struct.Name);

            
            editor.PutString(Constants.ISSUENO_COLUMN, _struct.sequenceId);
            editor.PutString(Constants.STRUCT_NAME_TKT_DTL, _struct.Name);
            editor.Apply();


            DroidContext.MyFragManager.AddToInternalBackstack(_struct.Name);


            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();

            Fragment issueFragment = FragmentManager.FindFragmentByTag(_struct.Name);
            if (issueFragment != null)
            {
                fragmentTransaction.Hide(issueFragment);
            }

            var notesFragment = (NotesFragment)FragmentManager.FindFragmentByTag(Constants.NOTES_FRAGMENT_TAG);
            if (notesFragment != null)
            {
                fragmentTransaction.Show(notesFragment);
                notesFragment.GetNotesByTicket();
            }
            else
            {
                fragmentTransaction.Replace(Resource.Id.frameLayout1, new NotesFragment(), Constants.NOTES_FRAGMENT_TAG);
            }


            //// add this to the backstack so we can come back here when done
            //// TODO - need to account for skipping around with drawer menus
            //fragmentTransaction.AddToBackStack(null);


            fragmentTransaction.Commit();
        }

 */
 
        //async void BtnSaveParkingClick(object sender, EventArgs e)
        //{
        //    Log.Debug("saveButton", "Start");

        //    var builder = new AlertDialog.Builder(Activity);
        //    builder.SetTitle("Issue Ticket");
        //    builder.SetMessage("Confirm ticket issuance?");
        //    builder.SetPositiveButton("YES", delegate
        //    {
        //        // save the ticket data
        //        var commonADO = new CommonADO();
        //        ParkingSequenceADO.UpdateInUseSequenceId(_struct.sequenceId, Constants.SRCINUSE_FLAG_SUBMITTED, _struct.SequenceName);
        //        bool loUpdateRowResult  = await commonADO.UpdateRow(_struct, Constants.STATUS_READY, Constants.WS_STATUS_READY, Activity, _struct.Name);

        //        // we have to wait until the save is complete before we can try to print 
        //        bool loResultCont = await loUpdateRowResult.Result;
        //        //todo - figure out how to await a delegate
        //        //bool resultCont = await result;

                
        //        // AJW - review/todo do for universal ticket types

        //        if (_struct.Name.Equals("PARKING"))
        //        {

        //            // AJW - this should be using value from TIssStruct/Panels
        //            var myPrintMgr = new PrintManager();
        //            Bitmap loTicketBitmap = myPrintMgr.PrintPicture(_struct.Name, _struct.sequenceId, DateTime.Now);

        //            // AJW - this should be using value from TIssStruct
        //            //Bitmap loTicketBitmap = null; // SaveTicketImageToDeviceStorage(_struct.sequenceId, DateTime.Now.ToString("YYYY MM DD")); //  _struct. issueNum, issueDate);
        //            if (loTicketBitmap != null)
        //            {
        //                Helper.SaveTIssueFormBitmapImageToStorage(loTicketBitmap, _struct.Name, _struct.sequenceId, DateTime.Now);
        //                //toastMsg = "Ticket image not available";
        //                //throw new Exception(toastMsg);
        //            }
                    
        //        }


        //        DroidContext.ResetControlStatusByStructName(_struct.Name);
        //        Toast.MakeText(Activity, "Submission Complete", ToastLength.Long).Show();





        //        String tabPosStr = null;
        //        ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
        //        ISharedPreferencesEditor editor = prefs.Edit();
        //        if (Constants.STRUCT_TYPE_CITE.Equals(_struct.Type))
        //        {
        //            tabPosStr = prefs.GetString(Constants.LABEL_TICKETS_TAB + Constants.LABEL_TAB_POSITION, null);                    
        //        }
        //        else if (Constants.STRUCT_TYPE_CHALKING.Equals(_struct.Type))
        //        {
        //            tabPosStr = prefs.GetString(Constants.LABEL_CHALKS_TAB + Constants.LABEL_TAB_POSITION, null);   
        //        }
        //        else
        //        {
        //            tabPosStr = prefs.GetString(_struct.Name + Constants.LABEL_TAB_POSITION, null); 
        //        }
        //        int tabPosition = 0;
        //        if (tabPosStr != null)
        //        {
        //            int.TryParse(tabPosStr, out tabPosition);
        //        }
        //        Activity.ActionBar.SetSelectedNavigationItem(tabPosition);
        //        _formPanel.ChangeLayout(0);
        //    });

        //    builder.SetNegativeButton("NO", delegate
        //    {
        //    });
        //    builder.Show();
        //}




        

        /// <summary>
        /// This is a duplicated method from TicketDetailFragment2 - this needs to moved into a universal single helper function
        /// </summary>
        /// <param name="issueNum"></param>
        /// <param name="issueDate"></param>
        /// <param name="localFlag"></param>
        public void SendPrintDirect(Bitmap iTicketImageBitmap, List<PCLPrintingClass.PCLStringRow> iAllStringsInCurrentTicket)
        {
            string loResultMsg = "";

            PrinterSupport_BaseClass.SendImageToCurrentlySelectedPrinter(iTicketImageBitmap, iAllStringsInCurrentTicket, ref loResultMsg);

           // display a result
            try
            {
                Activity.RunOnUiThread(() => Toast.MakeText(this.Activity, loResultMsg, ToastLength.Long).Show());
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Exception source: {0}", e.Source);
            }


        }


        /// <summary>
        /// Attach as notes any multimedia files that were created during the issuance
        /// </summary>
        private async void AutoAttachMultimediaFiles( string structName, string iParentSeqId, string iOfficerName, string iOfficerId )
        {
            // any multimedia files to attach?
            if (DroidContext.gPhotosTakenForPendingAttatchment.Count == 0)
            {
                // nothing to do here
                return;
            }

            try
            {
                Java.IO.File loPhotoDir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), Constants.MULTIMEDIA_FOLDERNAME);

                var parkingSequenceADO = new ParkingSequenceADO();


                  // for now, we know the ticket has been submitted
                  bool submitNoteInfo = true;
                

                foreach (string onePhotoFileToAttach in DroidContext.gPhotosTakenForPendingAttatchment)
                {

                    Java.IO.File loPhotoFile = new Java.IO.File(loPhotoDir, onePhotoFileToAttach);



                    ParkNoteDTO loParkNoteDTO = new ParkNoteDTO();
                    loParkNoteDTO.SeqId = iParentSeqId; //Make sure to attach the multimedia to same ticket //_prefs.GetString(Constants.ISSUENO_COLUMN, null);                    loParkNoteDTO.SeqId = _prefs.GetString(Constants.ISSUENO_COLUMN, null);
                    loParkNoteDTO.NotesMemo = "MULTIMEDIA ATTACHED";
                    loParkNoteDTO.DBRowId = null;  // these are all new notes

                    loParkNoteDTO.NoteDate = DateTimeOffset.Now.ToString(Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK);
                    loParkNoteDTO.NoteTime = DateTimeOffset.Now.ToString(Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK);


                    loParkNoteDTO.OfficerId = iOfficerId;
                    loParkNoteDTO.OfficerName = iOfficerName;

                    if (loPhotoFile != null)
                    {
                        loParkNoteDTO.MultiMediaNoteFileName = loPhotoFile.Name;
                        //loParkNoteDTO.MultiMediaNoteDataType = "2"; // TODO : a number code for  photo, video etc.
                        loParkNoteDTO.MultiMediaNoteDataType = AutoISSUE.DBConstants.TMultimediaType.mmPicture.ToString();

                        // this file LastModified will return milliseconds since 1/1/1970
                        var dt = new DateTime(1970, 1, 1);
                        dt = dt.AddMilliseconds(loPhotoFile.LastModified());

                        //loParkNoteDTO.MultiMediaNoteDateStamp = dt.ToString(Constants.DT_YYYYMMDD);
                        //loParkNoteDTO.MultiMediaNoteTimeStamp = dt.ToString(Constants.TIME_HHMMSS);
                        loParkNoteDTO.MultiMediaNoteDateStamp = dt.ToString(Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK);
                        loParkNoteDTO.MultiMediaNoteTimeStamp = dt.ToString(Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK);


                    }


                    try
                    {
                        // if it is a new note, then insert
                        if (string.IsNullOrEmpty(loParkNoteDTO.DBRowId) == true)
                        {
                            await parkingSequenceADO.InsertRowParkNote(loParkNoteDTO);
                        }
                        else
                        {
                            await parkingSequenceADO.UpdateParkNote(loParkNoteDTO);
                        }
                    }
                    catch (Exception exp)
                    {
                        if (string.IsNullOrEmpty(loParkNoteDTO.DBRowId) == true)
                        {
                            Log.Debug("ERROR Exception in AutoAttachMultimediaFiles InsertRowParkNote: ", exp.Message);
                            System.Console.WriteLine("ERROR Exception in AutoAttachMultimediaFiles InsertRowParkNote: ", exp.Message);
                        }
                        else
                        {
                            Log.Debug("ERROR Exception in AutoAttachMultimediaFiles UpdateParkNote: ", exp.Message);
                            System.Console.WriteLine("ERROR Exception in AutoAttachMultimediaFiles UpdateParkNote: ", exp.Message);
                        }
                    }
                }


                // these have been attached successfully
                // TODO - should we delete these files? not here, they should be deleted if not attached?
                DroidContext.gPhotosTakenForPendingAttatchment.Clear();


                // all the notes have been been added to DB, do we need to upload now?
                if (submitNoteInfo)
                {
                    Activity.StartService(new Intent(Activity, typeof(SyncService)));
                }

                //Toast.MakeText(Activity, "Multmedia Note Attached", ToastLength.Long).Show();
            }
            catch (Exception exp)
            {
                Log.Debug("ERROR Exception in AutoAttachMultimediaFiles: ", exp.Message);
                System.Console.WriteLine("ERROR Exception in AutoAttachMultimediaFiles: ", exp.Message);

            }
        }




        protected void PerformExtraDoneBtnTasks()
        {

            if (this.fSourceDataStruct != null)
            {
                if (this.fSourceIssueStructLogic != null)
                {
                    // clean up
                    this.fSourceIssueStructLogic.UndoIssueSource(this);

                    //// release the mutex
                    SearchStructLogicAndroid.unSearchEvaluateInProcess = false; //  true; // DEBUG -- This is true in C++ source?
                }
            }

            // clear out for future
            fSourceDataRawRow = null;
            fSourceDataRowFormEditMode = Reino.ClientConfig.EditRestrictionConsts.femNewEntry;



            // TMarkModeStruct needs to do a search            
            // Evaluate this first because TMarkModeStruct is a descendant of TSearchStruct
            if (this._struct._TIssStruct is TMarkModeStruct)
            {
                MarkModeStructLogicAndroid loMarkModeSearchLogic = (MarkModeStructLogicAndroid)(this._struct._TIssStruct.StructLogicObj);
                if (loMarkModeSearchLogic != null)
                {
                    loMarkModeSearchLogic.PerformSearchAndIssue(this._formPanel, false, 2, "", null, false);
                    // this.AssocSearchStructLogic.PerformSearchAndIssue(this.CfgForm, false, 2, "", null, false);
                }

                // don't let TMarkModeStruct get evaluated as TSearchStruct 
                return;
            }

            // THotSheetStruct needs to do a search         
            if ( this._struct._TIssStruct is TSearchStruct)
            {
                SearchStructLogicAndroid loSearchLogic = (SearchStructLogicAndroid)(this._struct._TIssStruct.StructLogicObj);
                if ( loSearchLogic != null )
                {

                    string loMatchFieldNames = string.Empty;
                    if ( this._struct._TIssStruct is THotSheetStruct )
                    {
                        // these matchfield names need to come from the FORM, not the structure - in support of ISSUE2 searches etc.
                        //loMatchFieldNames = (this._struct._TIssStruct as THotSheetStruct).MatchFieldsName;
                        loMatchFieldNames = (this._struct._TIssForm as THotSheetForm).MatchFieldsName;
                    }

                    loSearchLogic.PerformSearchAndIssue(this._formPanel, false, 1, loMatchFieldNames, null, false);
                    // this.AssocSearchStructLogic.PerformSearchAndIssue(this.CfgForm, false, 1, ((THotSheetForm)(this.CfgForm)).MatchFieldsName, null, false);
                }


                // for completeness....
                // don't let TSearchStruct get evaluated as something else we implement later.... 
                return;
            }


        }


        /*
        * TIssForm DoneButtonClick
        *
        * Every IssForm must hava a save button. When the save button is pressed, this routine
        * is executed (but not directly.)
        */
        private short TBaseIssueForm_DoneButtonClick()
        {
            /* in all other modes will will exit the form entirely */
            //SetExitForm(1);
            //fFormEditResult = FormEditResult_OK;
            return 0;
        }


        private short TSearchMatchForm_DoneButtonClick()
        {
            // skip TIssForm's version which tries to save the record, print if necessary, etc.

            #if _implemented_
            // check to see if there is a pending wireless search before we go.
            if (PendingWirelessSearch != NULL)
            {
                // there is a pending search, ask the user if they should wait.
                bool loWantToWait = QueryUser( "Wireless Search is Pending!", "Wait for a reply?" );

                // the wireless result could have arrived while asking the question, so check again.
                // sure enough, it completed while we were waiting. Need to process it now since the message was
                // handled by the QueryUser form, not us.
                if (ProcessPendingSearch())
                {
                    return -1; // a successful search arrived. ProcessPendingSearch has set fExitForm=TRUE.
                }
     

                if (loWantToWait && (PendingWirelessSearch!=NULL))
                return -1; // User want's to wait and there is still something to wait for.
            }
            #endif

            return TBaseIssueForm_DoneButtonClick();
        }



        private short TIssForm_DoneButtonClick()
        {
            short loStatus;

            #if _implemented_
	
	        if (fFormEditMode == femView) return TBaseIssForm::DoneButtonClick( iSender );
	
	// if we are issuing multiple, make sure the user hasn't by-passed "capture signature". Capture Signature 
	if (!MultipleIssuanceOkToFinish()) return -1;
	
	
	if (!GetFormSaved())
	{
		// PAM additions here
		if (!HandlePAMConfirmation( PAMConfirmationPoint_BeforeSave) )
			return -1;
		
		if ((loStatus = SaveFormRecord()) < 0)
			return loStatus;
	}
	
	
	// TEMP: for oakland demo, we must attach notes before we print, since we are printing images from the notes
	// in production solution, we will store the image reference in the main form
	//if ( _stricmp( glIssueAp->fClientName, "OAKLAND DEMO" ) == 0 )
	//if ( uHALr_strpos( glIssueAp->fClientName, "DEMO" ) != -1)
   if ( GetRegistryValueAsInt( SECTION_ISSUE_AP, AUTOATTACH_IMAGE, AUTOATTACH_IMAGE_DEFAULT ) != 0 )
	{
		// Attach pictures/sound that entered system during the ticket entry
		AutoAttachMMNotes(iSender);
   }

 
    
	
	
	
	/* print the form if necessary */
	if ((!fPrintNotMandatory) && (!GetFormPrinted()))
	{
		PrintForm( 1 );
		if (!GetFormPrinted())  
			return -1;
		Sleep(10);
	}
	
    if ((!fPrintNotMandatory) && (!GetFormPrinted())) PrintForm( 1 );
	
	
	// TEMP - patch code for Oakland Demo, dont do attachment after
	//if ( _stricmp( glIssueAp->fClientName, "OAKLAND DEMO" ) != 0 )
	//if ( uHALr_strpos( glIssueAp->fClientName, "DEMO" ) < 0)
    if ( GetRegistryValueAsInt( SECTION_ISSUE_AP, AUTOATTACH_IMAGE, AUTOATTACH_IMAGE_DEFAULT ) == 0 )
	{
		// Attach pictures/sound that entered system during the ticket entry
		AutoAttachMMNotes(iSender);
    }
    
	/* log it */
	glIssueAp->fIssStructMgr->LogActivity( fParentStruct->GetName() );
	
	
	/* no, don't
	if (fBtnIssueChild)
    fParentStruct->IssueChildRecord( femSingleEntry, "", fEditRecNo );
	*/
	
	TIssForm_PerformExtraDoneBtnTasks();
	

			/* if we are in multiple entry mode, move to the next record in the queue */
	if ( (fFormEditMode & femIssueMultipleAttr) &&  (EditMultipleEntry(fMultipleEntryMgr->CurrentRecNo() + 1)) ) return 0;
	
	
	if ( !(fFormEditMode & femSingleRecordAttr))
	{
		/* rather than exit, we will enter a new record. */
		
		// make sure we have enough flash
		Sleep(10);
		if (QueryUserReclaimFlash( 50000 ) < 50000)
		{
			SetExitForm( 1 );
			return 0;
		}
		


		SetObscured( 1 );
		SetEditRecNo( GetEditRecNo() + 1 );
		fFormEditMode = femNewEntry;
		if (PrepareForEdit() < 0)
		{ // get outta here!
			SetExitForm( 1 );
			fFormEditResult = FormEditResult_OK;
		}
		

		SetFocus( 0, sfmFirstControl, 0, 0 );
		SetObscured( 0 );
		PaintDescendants();
		Sleep(10);
		return 0;
	}

    #endif

            return TBaseIssueForm_DoneButtonClick();
        }



        private void ExternalEnforcementInterfaceConfirmation_Callback(Result resultCode)
        {

            // debug - stay here to retry/test
            //return;


            // for now always continue to save
            // confirmed, resume the final save
            BtnSaveParkingPrim();


#if _later_

            switch (resultCode)
            {
                case (Result.Ok):
                    {
                        // confirmed, resume the final save
                        BtnSaveParkingPrim();
                        break;
                    }
                case (Result.Canceled):
                    {
                        break;
                    }

                default:
                    {
                        break;
                        // confirmed, resume the final save
                        BtnSaveParkingPrim();
                    }
            }
#endif

        }

    

        // can sometimes take few secs... ignore double clicks
        static bool gExternalEnfConfirmationCheckInProgress  = false;
        /// <summary>
        ///  Call enforcement confirmation check web service as needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchExternalEnforcementInterfaceConfirmationCheck()
        {

            //if (!GetFormSaved())
            //{
            //    // PAM additions here
            //    if (!HandlePAMConfirmation(PAMConfirmationPoint_BeforeSave))
            //        return -1;

            //    if ((loStatus = SaveFormRecord()) < 0)
            //        return loStatus;
            //}



            try
            {
                // already busy?
                if (gExternalEnfConfirmationCheckInProgress == true) // this scheme doesnt work here
                {
                    return;
                }

                gExternalEnfConfirmationCheckInProgress = true;



                string loEnforcmentKey = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(ExternalEnforcementInterfaces.cnGISPropertyNameEnforcementKey);

                //string loEnfValue = GetChildListItemForParentIndex(uiComponent, iPanelField);



                FragmentManager fm = this.FragmentManager;
                FragmentTransaction fragmentTransaction = fm.BeginTransaction();
                ExternalEnforcementConfirmationFragment loExternalEnfConfirmationFrag = new ExternalEnforcementConfirmationFragment(loEnforcmentKey);
                loExternalEnfConfirmationFrag.gCallingFragmentTagName = this.GetFragmentTagName();
                loExternalEnfConfirmationFrag.Show(fm, Constants.EXTERNAL_ENFORCEMENT_CONFIRMATION_FRAGMENT_TAG);
            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "ExternalEnforcementInterfaceConfirmationOK", "ExternalEnforcementInterfaceConfirmationOK");
                System.Console.WriteLine("Exception caught in process: {0} {1}", exp, "ExternalEnforcementInterfaceConfirmationOK");
            }
            finally
            {
                gExternalEnfConfirmationCheckInProgress = false;
            }
            
        }
    


        async void BtnSaveParkingClick(object sender, EventArgs e)
        {
            Log.Debug("saveButton", "Start");


            // until ready for prime time
            BtnSaveParkingPrim();
            return;



            // did they start with external data?
            if (ExternalEnforcementInterfaces.gWirelessEnforcementMode != ExternalEnforcementInterfaces.TWirelessEnforcementMode.wefNone)
            {
                // re-direct to "PAM" check first
                LaunchExternalEnforcementInterfaceConfirmationCheck();
            }
            else
            {
                // no wireless info, just save the ticket
                BtnSaveParkingPrim();
            }
        
        }



        async void BtnSaveParkingPrim()
        {
            Log.Debug("BtnSaveParkingPrim", "Start");


            // AJW - review save the ticket data -             we have to wait until the save is complete before we can try to print 
            // this is a problem to try to do within the button click delagate... this code need to be restructred



            var commonADO = new CommonADO();


            // AJW - TODO - these are getting wiped in UpdateRow, but we need them
            string loSavedSequenceId = _struct.sequenceId;
            string loSavedStructName = _struct.Name;

            ParkingSequenceADO.UpdateInUseSequenceId(_struct.sequenceId, Constants.SRCINUSE_FLAG_SUBMITTED, _struct.SequenceName);
            bool loUpdateRowResult = await commonADO.UpdateRow(_struct, Constants.STATUS_READY, Constants.WS_STATUS_READY, Activity, _struct.Name);

            // AJW - review/todo do for universal ticket types

             
            //check for defined printpicture to determine procedure
            bool loHasPrintPicture = false;
            if (this._struct._TIssForm != null)
            {
                //loHasPrintPicture = ( this._struct._TIssForm.PrintPictureList.Count > 0 );

                loHasPrintPicture = (this._struct.PrintPicture != null); 
            }


            if ( loHasPrintPicture == true )
            //if (_struct.Name.Equals("PARKING"))
            {

                // AJW - this should be using value from TIssStruct/Panels
                var myPrintMgr = new PrintManager();
                Bitmap loTicketBitmap = myPrintMgr.PrintPicture(loSavedStructName, loSavedSequenceId, DateTime.Now);

                // AJW - this should be using value from TIssStruct
                //Bitmap loTicketBitmap = null; // SaveTicketImageToDeviceStorage(_struct.sequenceId, DateTime.Now.ToString("YYYY MM DD")); //  _struct. issueNum, issueDate);
                if (loTicketBitmap != null)
                {
                    // save for later re-printing
                    Helper.SaveTIssueFormPrintJobDataStorage(loTicketBitmap, myPrintMgr.GetPCLPrintRowsList(), loSavedStructName, loSavedSequenceId, DateTime.Now);
                    //toastMsg = "Ticket image not available";
                    //throw new Exception(toastMsg);

                    //_progressDialog = ProgressDialog.Show(this.Activity, "Please wait...", "Printing Ticket...", true);
                    ThreadPool.QueueUserWorkItem(o => SendPrintDirect(loTicketBitmap, myPrintMgr.GetPCLPrintRowsList()));
                    //SendPrintDirect(loTicketBitmap);

                }

            }



            if ( loHasPrintPicture == true )
            //if (_struct.Name.Equals("PARKING"))
            {
                ISharedPreferences loMMprefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
                ISharedPreferencesEditor loMMeditor = loMMprefs.Edit();

                // get the current ticket info
                //string loStructName = loMMprefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);
                //string loParentSequenceId = loMMprefs.GetString(Constants.ISSUENO_COLUMN, null);

                string loStructName = loSavedStructName;
                string loParentSequenceId = loSavedSequenceId;


                string loOfficerId = loMMprefs.GetString(Constants.OFFICER_ID, null);
                string loOfficerName = loMMprefs.GetString(Constants.OFFICER_NAME, null);

                AutoAttachMultimediaFiles(loStructName, loParentSequenceId, loOfficerName, loOfficerId);
            }




            PerformExtraDoneBtnTasks();


            // KLUDGY! 
            if (_struct.Name.Equals("PARKING") == true)
            {
                DroidContext.CitationsSinceRestart++;
            }

            DroidContext.ResetControlStatusByStructName(_struct.Name);
            //Toast.MakeText(Activity, "Submission Complete", ToastLength.Long).Show();


            // go back to the start for next time
           // not now, later when we start another form _formPanel.ChangeCurrentDisplayPage(0);




            // save where we came from - but when done we want to go to main menu
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            ISharedPreferencesEditor editor = prefs.Edit();

            //editor.PutString(Constants.PREVIOUS_FRAGMENT, _struct.Name);
            editor.PutString(Constants.PREVIOUS_FRAGMENT, _tagName);

            editor.PutString(Constants.ID_COLUMN, _struct._rowID);
            editor.PutString(Constants.ISSUENO_COLUMN, _struct.sequenceId);
            editor.PutString(Constants.STRUCT_NAME_TKT_DTL, _struct.Name);
            editor.Apply();



            // AJW - KLUDGE - work around fragment management issue
            _formPanel.PanelRootPageLinearLayout.RemoveAllViews();



            /////////////////////////




            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();

            Fragment issueFragment = FragmentManager.FindFragmentByTag(_tagName);
            //Fragment issueFragment = FragmentManager.FindFragmentByTag(_struct.Name);
            if (issueFragment != null)
            {
                fragmentTransaction.Hide(issueFragment);
            }
            else
            {
                // debug breakpoint - how can this not be found? or be found but not hidden? why does it fail to be hidden on 2nd/3rd ticket?
                issueFragment = null;
            }




            editor.PutString(Constants.STRUCT_NAME_TKT_DTL, loSavedStructName);
            if (Constants.MARKMODE_TABLE.Equals(loSavedStructName))
            {
                editor.PutString(Constants.TICKETID, loSavedSequenceId);
            }
            else
            {
                editor.PutString(Constants.TICKETID, loSavedSequenceId);
            }

            editor.Apply();


            // this is a one way road - not backing up to ticket issuance
            DroidContext.MyFragManager.ClearInternalBackstack();

/*
            // decide where to go
            if (Constants.MARKMODE_TABLE.Equals(loSavedStructName))
            {

                // what data type did they select? this is the data type we need
                string loTargetFragmentTag = Helper.BuildIssueReviewFragmentTag(loSavedStructName);


                var dtlFragment = (IssueReviewDetailFragment)FragmentManager.FindFragmentByTag(Constants.ISSUE_REVIEW_DETAIL_FRAGMENT_TAG);
                if (dtlFragment != null)
                {
                    fragmentTransaction.Show(dtlFragment);
                    dtlFragment.CreateLayout();
                }
                else
                {
                    fragmentTransaction.Replace(Resource.Id.frameLayout1, new IssueReviewDetailFragment(), Constants.ISSUE_REVIEW_DETAIL_FRAGMENT_TAG);
                }
            }
            else
 */ 
            {
                // what data type did they select? this is the data type we need
                string loTargetFragmentTag = Helper.BuildIssueReviewFragmentTag(loSavedStructName);


                var dtlFragment = (IssueReviewDetailFragment)FragmentManager.FindFragmentByTag(loTargetFragmentTag);
                //var dtlFragment = (TicketIssueSummaryFragment)FragmentManager.FindFragmentByTag(loTargetFragmentTag);
                //var dtlFragment = (TicketIssueSummaryFragment)FragmentManager.FindFragmentByTag(Constants.ISSUE_REVIEW_SUMMARY_FRAGMENT_TAG);
                if (dtlFragment != null)
                {
                    fragmentTransaction.Show(dtlFragment);
                    dtlFragment.RefreshDisplayedRecord();
                }
                else
                {
                    var fragment = new IssueReviewDetailFragment { Arguments = new Bundle() };
                    fragment.Arguments.PutString("structName", _structName);

                    //MainActivity.RegisterFragment(fragment, loTag, _structName, "NoMenu " + loTag, FragmentClassificationType.fctSecondaryActivity, -1, -1);
                    fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, loTargetFragmentTag);

                    // fragmentTransaction.Replace(Resource.Id.frameLayout1, new TicketIssueSummaryFragment(), Constants.ISSUE_REVIEW_SUMMARY_FRAGMENT_TAG);

                    fragment.RefreshDisplayedRecord(); // ok here?
                }
            }



            fragmentTransaction.Commit();

       }


        //async void BtnSaveMarkmodeClick(object sender, EventArgs e)
        //{
        //    Log.Debug("saveButton", "Start");
        //    bool reqMissing = false;
        //    for (int i = 0; i < _struct.Panels.Count; i++)
        //    {
        //        foreach (XMLConfig.PanelField panelField in _struct.Panels[i].PanelFields)
        //        {
        //            if (panelField.Name == Constants.TIRESTEMSFRONTTIME_COLUMN
        //                || panelField.Name == Constants.TIRESTEMSREARTIME_COLUMN)
        //            {
        //                if (panelField.IsRequired && panelField.Value == null)
        //                {
        //                    reqMissing = true;
        //                    Toast.MakeText(this.Activity, "Please select both front and rear stem", ToastLength.Long).Show();
        //                }
        //            }
        //        }
        //    }

        //    if (!reqMissing)
        //    {
        //        var commonADO = new CommonADO();
        //        Task<bool> result = commonADO.UpdateRow(_struct, Constants.STATUS_READY, Constants.WS_STATUS_READY, this.Activity, Constants.MARKMODE_TABLE);
        //        bool resultCont = await result;

        //        Toast.MakeText(this.Activity, _struct.Name + " Saved", ToastLength.Long).Show();
        //        //Toast.MakeText(Activity, "Chalking Submitted", ToastLength.Long).Show();



        //        PerformExtraDoneBtnTasks();




        //        DroidContext.ResetControlStatusByStructName(_struct.Name);

        //        // re-init for the next form to be issued.
        //        //_formPanel.ChangeCurrentDisplayPage(0);
        //        StartTicketLayout();

        //    /*
        //        string tabPosString = _prefs.GetString("TabPosition_" + _struct.Name, null);
        //        int tabPosition = 0;
        //        if (tabPosString != null)
        //        {
        //            int.TryParse(tabPosString, out tabPosition);
        //        }

        //        // not anymore, should go back to start of markmode for new record Activity.ActionBar.SetSelectedNavigationItem(tabPosition);
        //     */
             
        //    }
        //}



        ////Submit Activity Log to the Database
        //private async void BtnSubmitClick(object sender, EventArgs e)
        //{
        //    XMLConfig.IssStruct activityStruct = DroidContext.XmlCfg.GetStruct(Constants.STRUCT_TYPE_ACTIVITYLOG, _struct.Name);
        //    bool success = await _formPanel.SaveCurrentLayout();

        //    if (success)
        //    {
        //        var commonADO = new CommonADO();
        //        Task<bool> result = commonADO.InsertRow(activityStruct, Activity, _struct.Name, null);
        //        await result;
        //        Toast.MakeText(this.Activity, "Activity Submitted", ToastLength.Long).Show();
        //        DroidContext.ResetControlStatusByStructName(_struct.Name);
        //        _formPanel.ChangeCurrentDisplayPage(0);
        //    }
        //    //TODO - add error message here.
        //}


        /// <summary>
        /// Submit common TIssStruct without a print picture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSubmitCommonClick(object sender, EventArgs e)
        {

            /// TIssForm_DoneButtonClick

            bool success = await _formPanel.SaveCurrentLayout();

            if (success)
            {
              
                // assume we'll want to save this 
                bool loSaveFormData = true;

                // pure search structures aren't saved
                // is there a cleaner way to discriminate this?
                if (_struct._TIssStruct is THotSheetStruct)
                {
                    loSaveFormData = false;
                }



                if (loSaveFormData == true)
                {
                    var commonADO = new CommonADO();

                    // does this structure have a sequence?
                    if (String.IsNullOrEmpty(_struct._TIssStruct.Sequence) == false)
                    {
                        ParkingSequenceADO.UpdateInUseSequenceId(_struct.sequenceId, Constants.SRCINUSE_FLAG_SUBMITTED, _struct.SequenceName);
                    }

                    // already working on an existing row?
                    if (String.IsNullOrEmpty(_struct._rowID) == false)
                    {
                        Task<bool> resultUpdate = commonADO.UpdateRow(_struct, Constants.STATUS_READY, Constants.WS_STATUS_READY, Activity, _struct._TIssStruct.MainTable.Name);
                        await resultUpdate;
                    }
                    else
                    {
                        // insert new row
                        Task<bool> resultInsert = commonADO.InsertRow(_struct, Activity, _struct._TIssStruct.MainTable.Name, null);
                        await resultInsert;

                        // get the row id
                        _struct._rowID = commonADO.GetRowId(_struct._TIssStruct.MainTable.Name);

                        // mark  the row as complete and ready for upload  - TODO - build a version of InsertRow that does this in the same step
                        Task<bool> resultUpdate = commonADO.UpdateRow(_struct, Constants.STATUS_READY, Constants.WS_STATUS_READY, Activity, _struct._TIssStruct.MainTable.Name);
                        await resultUpdate;

                    }

                    Toast.MakeText(this.Activity, _struct.Name + " Saved", ToastLength.Long).Show();
                }


                PerformExtraDoneBtnTasks();


                DroidContext.ResetControlStatusByStructName(_struct.Name);

                // re-init for the next form to be issued.
                //_formPanel.ChangeCurrentDisplayPage(0);
                StartTicketLayout();

            }
            //TODO - add error message here.
        }


        public void btnDoneClick(object sender, EventArgs e)
        {

            // only forms with no print picture would come through here, 
            // those print picture go though print preview first


            // temp kludge
            string loStructName = _struct.Name.ToUpper();

            switch (loStructName)
            {
                //case Constants.STRUCT_NAME_ACTIVITYLOG:
                //    {
                //        BtnSubmitClick(sender, e);
                //        break;
                //    }

                //case Constants.STRUCT_NAME_CHALKING:
                //    {
                //        BtnSaveMarkmodeClick(sender, e);
                //        break;
                //    }

                default:
                    {
                        btnSubmitCommonClick(sender, e);
                        break;
                    }
            }

        }


        void BtnStemClick(object sender, EventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(Constants.STRUCT_NAME_TKT_DTL, _struct.Name);
            editor.Apply();

            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            Fragment chalkFragment = FragmentManager.FindFragmentByTag(_struct.Name);
            if (chalkFragment != null)
                fragmentTransaction.Hide(chalkFragment);

            var chalkTireFragment = (ChalkingTireFragment)FragmentManager.FindFragmentByTag(Constants.CHALK_TIRE_FRAGMENT_TAG);
            if (chalkTireFragment != null)
            {
                chalkTireFragment.setTireStem();
                fragmentTransaction.Show(chalkTireFragment);
            }
            else
            {
                //fragmentTransaction.Replace(Resource.Id.frameLayout1, new ChalkingTireFragment(), Constants.CHALK_TIRE_FRAGMENT_TAG);

                Fragment fragment = new ChalkingTireFragment { Arguments = new Bundle() };
                fragment.Arguments.PutString("structName", Constants.CHALK_TIRE_FRAGMENT_TAG);
                fragmentTransaction.Replace(Resource.Id.frameLayout1, fragment, Constants.CHALK_TIRE_FRAGMENT_TAG);

            }

            fragmentTransaction.Commit();
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            _touch.ProcessTouch(e);
            DisplayMetrics metrics = Resources.DisplayMetrics;

            // horizontal swipe detection
            if (Math.Abs(_touch.deltaX) > metrics.WidthPixels / 4)  //  a swipe 1/4 the width of the screen
            {
                if (_touch.deltaX < 0)
                {  // Swipe Left to Right

                    if (_formPanel.PanelPageCurrentDisplayedIndex > 0)
                        _formPanel.ChangeCurrentDisplayPage(-1);

                    return false;
                }
                if (_touch.deltaX > 0)
                {  // Swipe Right to Left

                    if (_formPanel.PanelPageCurrentDisplayedIndex < _struct.Panels.Count - 1)
                        _formPanel.ChangeCurrentDisplayPage(1);

                    return false;
                }
                /*
                if (deltaY < 0)   // Swipe Top to Bottom
                    return false;

                if (deltaY > 0)   // Swipe Bottom to Top
                    return false;
                */
            }

            return true;
        }


        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            switch (requestCode)
            {
                case Constants.ACTIVITY_REQUEST_CODE_LPR_PROCESSING:
                    {
                        if (data != null)
                        {
                            string iVehPlate = data.GetStringExtra(AutoISSUE.DBConstants.sqlVehLicNoStr);

                            _formPanel.OnFinishANPRDialog(iVehPlate);
                        }
                        break;
                    }

                case Constants.ACTIVITY_REQUEST_CODE_HOTSHEET_RESULT:
                    {
                        _formPanel.OnFinishANPRDialog("");
                        break;
                    }

                case Constants.ACTIVITY_REQUEST_CODE_GEOCODE_ADDRESS:
                    {
                        string loGeoCodeAddress = data.GetStringExtra(AutoISSUE.DBConstants.sqlLocStreetStr);
                        _formPanel.OnFinishGeoCodeAddressDialog(loGeoCodeAddress);
                        break;
                    }

                case Constants.ACTIVITY_REQUEST_CODE_EXTERNAL_ENFORCMENT_CONFIRMATION:
                    {
                        ExternalEnforcementInterfaceConfirmation_Callback(resultCode);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }
    }

   
}
