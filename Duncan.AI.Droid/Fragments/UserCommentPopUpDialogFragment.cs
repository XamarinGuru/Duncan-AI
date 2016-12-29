

using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Text;
using Android.Graphics;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.Preferences;
using Reino.ClientConfig;
using Duncan.AI.Droid.Common;
using System.Data;
using Duncan.AI.Droid.Utils.InputFilters;
using Duncan.AI.Droid.Utils.HelperManagers;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;

/// <summary>
/// Menu Popup Dialog to accept user input to another fragment/activiy
/// </summary>

namespace Duncan.AI.Droid
{
	public class UserCommentPopUpDialogFragment : DialogFragment
	{




        string _tagName;
        string _structName;

        private AlertDialog _thisDialog = null;  


        View _dialogView;

        TextView _menuTitle;
        ScrollView _menuScrollView;
        LinearLayout _menuScrollViewLinearLayout;


        TextView _userCommentLabel;
        AutoCompleteTextView _userComment;



        string gMenuTitleText = string.Empty;
        List<MainActivity.TMenuItemParent> gParentItems = new List<MainActivity.TMenuItemParent>();
        List<MainActivity.TMenuItemChild> gChildItems = new List<MainActivity.TMenuItemChild>();


        string _CommentLabelText = string.Empty;
        string[] _SuggestedItems = null;




        // margins
        int cnMarginTitleLeft = 50;
        int cnMarginTitleTop = 15;
        int cnMarginTitleRight = 50;
        int cnMarginTitleBottom = 15;


        int cnMarginParentLeft = 50;
        int cnMarginParentTop = 5;
        int cnMarginParentRight = 200;
        int cnMarginParentBottom = 5;

        int cnMarginChildLeft = 50;
        int cnMarginChildTop = 1;
        int cnMarginChildRight = 50;
        int cnMarginChildBottom = 1;


        int cnMarginExitLeft = 50;
        int cnMarginExitTop = 25;
        int cnMarginExitRight = 50;
        int cnMarginExitBottom = 25;


        int cnMarginCommentLabelLeft = 20;
        int cnMarginCommentLabelTop = 5;
        int cnMarginCommentLabelRight = 20;
        int cnMarginCommentLabelBottom = 0;

        int cnMarginCommentLeft = 20;
        int cnMarginCommentTop = 5;
        int cnMarginCommentRight = 20;
        int cnMarginCommentBottom = 100;


        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {

            _structName = Arguments.GetString("structName", null);

            _tagName = "menu_popup"; // get from parameters if really needed // Helper.BuildUserCommentPopUpDialogFragmentTag(_structName);


            Typeface loCustomTypeface;


            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);



            // Inflate and set the layout for the dialog -  Pass null as the parent view because its going in the dialog layout
            _dialogView = Activity.LayoutInflater.Inflate(Resource.Layout.UserCommentPopUpDialogFragment, null);


            // Pass null as the parent view because its going in the dialog layout
            builder.SetView(_dialogView);


            // if we set these, we'll get blank lines
            //builder.SetTitle("");
            //builder.SetMessage("");



            _menuTitle = _dialogView.FindViewById<TextView>(Resource.Id.title);
            if (_menuTitle != null)
            {
                _menuTitle.Text = gMenuTitleText;
                _menuTitle.Gravity = (GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);

                // initialize our typeface 
                loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnMenuPopupTitleTypeface);
                if (loCustomTypeface != null)
                {
                    _menuTitle.Typeface = loCustomTypeface;
                }
                _menuTitle.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnMenuPopupTitleTypefaceSizeSp);

                // since this textview is already built into the dialogfragment, we have to grab and update the layout margins
                ViewGroup.MarginLayoutParams lp = (ViewGroup.MarginLayoutParams)_menuTitle.LayoutParameters;
                lp.SetMargins(cnMarginTitleLeft, cnMarginTitleTop, cnMarginTitleRight, cnMarginTitleBottom);
            }

            _menuScrollView = _dialogView.FindViewById<ScrollView>(Resource.Id.menuscrollview_root);
            _menuScrollViewLinearLayout = _dialogView.FindViewById<LinearLayout>(Resource.Id.menuscrollview_linearlayout);


            // set the max lenth of the input
            int loMaxUserCommentLen = 80;   // TODO - would be nice if this could come from XML when available



            // create a layout to hold the user comment and label combo
            LinearLayout layoutUserComment = new LinearLayout(Activity) { Orientation = Android.Widget.Orientation.Vertical };


            // label comment
            _userCommentLabel = new TextView(Activity);
            loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnEditTextLabelTypeface);
            if (loCustomTypeface != null)
            {
                _userCommentLabel.Typeface = loCustomTypeface;
            }
            _userCommentLabel.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnEditTextLabelTypefaceSizeSp);
            _userCommentLabel.Text = _CommentLabelText;


            _userComment = new AutoCompleteTextView(Activity);
            _userComment.SetBackgroundResource(Resource.Drawable.EditTextLoginFocusLost);
            //_userName.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(loMaxUserNameLen), new InputFilterAllCaps() });

            // initialize our typeface 
            loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnEditTextViewTypeface);
            if (loCustomTypeface != null)
            {
                _userComment.Typeface = loCustomTypeface;
            }
            _userComment.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnEditTextViewTypefaceSizeSp);

            Int32 loMinTextLines = 3; //Convert.ToInt32(panelField.fEditFieldDef.Height / 17);
            _userComment.SetMinLines(loMinTextLines);
            _userComment.SetLines(loMinTextLines);

            _userComment.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;

            _userComment.SetHorizontallyScrolling(false);
            _userComment.SetMaxLines(int.MaxValue);

            // make sure we can get to it
            //_userComment.Focusable = true;
            //_userComment.FocusableInTouchMode = true;


/*
 * TODO - java -> c# to support drondown on touch
 * 
 * 
            _userComment.OnFocusChangeListener = new onFocusChange( 

 _userComment.setOnFocusChangeListener(new OnFocusChangeListener() {

        @Override
        public void onFocusChange(View v, boolean hasFocus) {
            if (hasFocus)
                _userComment.ShowDropDown();

        }
    });

    _userComment.setOnTouchListener(new OnTouchListener() {

        @Override
        public boolean onTouch(View v, MotionEvent event) {
            _userComment.ShowDropDown();
            return false;
        }
    });


*/




            //now set the input filters, text watchers, etc based on the edit mask.
            //_userComment.InputType = 

            List<IInputFilter> _inputFilters;
            InputTypes _inputType;

            _inputFilters = new List<IInputFilter> { new InputFilterLengthFilter(loMaxUserCommentLen) };

            // all characters upper case only
            _inputFilters.Add(new UpperCaseInputFilter());
            _inputType = InputTypes.TextFlagCapCharacters;

            _userComment.SetFilters(_inputFilters.ToArray());
            _userComment.InputType = _inputType;



            //_userComment.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);

            if (_SuggestedItems != null)
            {
                // custom adapter
                var autoCompleteAdapter = new CustomAutoCompleteTextViewAdapter2(Activity);
                autoCompleteAdapter.OriginalItems = _SuggestedItems;

                // standard adapter
                //var autoCompleteAdapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleSpinnerItem, _SuggestedItems);
                //var autoCompleteAdapter = new ArrayAdapter(Activity, Resource.Layout.autocompletetextview_dropdown_item, _SuggestedItems);

                _userComment.Adapter = autoCompleteAdapter;

                // show the drop down 
               _userComment.SetBackgroundResource(Resource.Drawable.EditTextDropDownListFocusedGray);

                // drop down on touch1

               //now hook up the events for this edit text
               _userComment.Click += _userComment_Click;

            }
            else
            {
                // no list here
               // let multiline  _userComment.SetBackgroundResource(Resource.Drawable.EditTextFocusedGray);
            }



            LinearLayout.LayoutParams layoutParamsUserCommentLabel = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            layoutParamsUserCommentLabel.SetMargins(cnMarginCommentLabelLeft, cnMarginCommentLabelTop, cnMarginCommentLabelRight, cnMarginCommentLabelBottom);
            //layoutParamsUserComment.Gravity = GravityFlags.Right;
            _userCommentLabel.LayoutParameters = layoutParamsUserCommentLabel;

            LinearLayout.LayoutParams layoutParamsUserComment = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            layoutParamsUserComment.SetMargins(cnMarginCommentLeft, cnMarginCommentTop, cnMarginCommentRight, cnMarginCommentBottom);
            //layoutParamsUserComment.Gravity = GravityFlags.Right;
            _userComment.LayoutParameters = layoutParamsUserComment;

            layoutUserComment.AddView(_userCommentLabel);
            layoutUserComment.AddView(_userComment);


            _menuScrollViewLinearLayout.AddView(layoutUserComment);




            ////////
            //
            //  action buttons
            //
            ////////



            builder.SetPositiveButton("OK", btnOKClick);
            builder.SetNegativeButton("CANCEL", btnCancel_Click);



            _thisDialog = builder.Create();


            // call this now to initialize vars for customization
            _thisDialog.Show();


            // customize buttons - has to execute after .Show()
            CustomizeUserCommentDialogButtons();


            // start here
            _userComment.RequestFocusFromTouch();
            Helper.ShowKeyboard(_userComment);


            return _thisDialog;
        }



        void btnCancel_Click(object sender, EventArgs e)
        {
            Helper.HideKeyboard(_userComment);
            // we're just going home
            this.DismissAllowingStateLoss();
        }


        void _userComment_Click(object sender, EventArgs e)
        {
            if (sender is AutoCompleteTextView)
            {
                AutoCompleteTextView oneView = (sender as AutoCompleteTextView);
                {
                    if (oneView.IsPopupShowing == false)
                    {
                        oneView.ShowDropDown();
                    }
                }
            }
        }



        private void CustomizeUserCommentDialogButtons()
        {
            int cnMarginExitLeft = 25;
            int cnMarginExitTop = 25;
            int cnMarginExitRight = 50;
            int cnMarginExitBottom = 25;

            int cnMarginConfirmLeft = 25;
            int cnMarginConfirmTop = 25;
            int cnMarginConfirmRight = 50;
            int cnMarginConfirmBottom = 25;

            Button btnConfirm = _thisDialog.GetButton((int)Android.Content.DialogButtonType.Positive);
            if (btnConfirm != null)
            {

                btnConfirm.Text = "CONFIRM";// Constants.cnMenuPopup_ExitText;
                Helper.SetTypefaceForButton(btnConfirm, FontManager.cnLPRDialogButtonTypeface, FontManager.cnMLPRDialogButtonTypefaceSizeSp);
            }


            Button btnExit = _thisDialog.GetButton((int)Android.Content.DialogButtonType.Negative);
            if (btnExit != null)
            {

                btnExit.Text = "CANCEL"; // Constants.cnMenuPopup_ExitText;
                // initialize our typeface 
                Helper.SetTypefaceForButton(btnExit, FontManager.cnLPRDialogButtonTypeface, FontManager.cnMLPRDialogButtonTypefaceSizeSp);

          }

        }



        public interface MenuPopupDialogListener
        {
            void OnFinishMenuPopUpDialog(string iSelectedItemTag);
        }

        private MenuPopupDialogListener resultListener;



        async void btnOKClick(object sender, EventArgs e)
        {

            string loUserComment = _userComment.Text;

            if (string.IsNullOrEmpty(loUserComment) == true)
            {
                Toast.MakeText(Activity, _userCommentLabel.Text + " : REQUIRED", ToastLength.Long).Show();

                _userComment.RequestFocusFromTouch();
                return;
            }


            if (_CallbackFragment != null)
            {
                _CallbackFragment.UserCommentPopUp_UserCommentCallback(_CallbackTag, loUserComment);
            }
            else if (_CallbackFragmentB != null)
            {
                _CallbackFragmentB.UserCommentPopUp_UserCommentCallback(_CallbackTag, loUserComment);
            }

            Helper.HideKeyboard(_userComment);

            //this.Dismiss();
            this.DismissAllowingStateLoss();
        }

        private IssueReviewDetailFragment _CallbackFragment;
        private string _CallbackTag;
        public void SetCallbackActivity(IssueReviewDetailFragment iCallbackFragment, string iCallbackTag )
        {
            _CallbackTag = iCallbackTag;
            _CallbackFragment = iCallbackFragment;
        }

        private NotesReviewSelectFragment _CallbackFragmentB;
        public void SetCallbackActivity(NotesReviewSelectFragment iCallbackFragment, string iCallbackTag)
        {
            _CallbackTag = iCallbackTag;
            _CallbackFragmentB = iCallbackFragment;
        }




        private void BuildMenuItems()
        {
            // define layout margins for use on each instance
            var layoutParamsButtonParent = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            layoutParamsButtonParent.SetMargins(cnMarginParentLeft, cnMarginParentTop, cnMarginParentRight, cnMarginParentBottom);

            var layoutParamsButtonChild = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            layoutParamsButtonChild.SetMargins(cnMarginChildLeft, cnMarginChildTop, cnMarginChildRight, cnMarginChildBottom);


            for (int i = 0; i < gParentItems.Count; i++)
            {
                MainActivity.TMenuItemParent loParentItem = gParentItems[i];

                Button btnParentMenuItem = new Button(Activity);

                btnParentMenuItem.SetBackgroundResource(Resource.Drawable.button_menu_popup_parent);
                btnParentMenuItem.SetTextColor(Color.White);

                // force parent items to be all upper case 
                btnParentMenuItem.Text = loParentItem.MenuItemText.ToUpper();
                btnParentMenuItem.Gravity = ( GravityFlags.Left | GravityFlags.CenterVertical );

                // tag is the reference to the fragment to navigate to
                btnParentMenuItem.Tag = loParentItem.ItemTag;



                // initialize our typeface 
                Typeface loCustomTypefaceParent = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnMenuPopupButtonParentItemTypeface);
                if (loCustomTypefaceParent != null)
                {
                    btnParentMenuItem.Typeface = loCustomTypefaceParent;
                }
                btnParentMenuItem.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnMenuPopupButtonParentItemTypefaceSizeSp);


                // parent items don't have a click event, they are just layout headings
                _menuScrollViewLinearLayout.AddView(btnParentMenuItem, layoutParamsButtonParent);


                for (int j = 0; j < gChildItems.Count; j++)
                {
                    MainActivity.TMenuItemChild loChildItem = gChildItems[j];

                    // is this our parent?
                    if (loChildItem.ParentMenu == loParentItem)
                    {

                        Button btnChildMenuItem = new Button(Activity);

                        btnChildMenuItem.SetBackgroundResource(Resource.Drawable.button_menu_popup_child);
                        btnChildMenuItem.SetTextColor(Color.Black);

                        btnChildMenuItem.Text = loChildItem.MenuItemText;
                        btnChildMenuItem.Gravity = (GravityFlags.Left | GravityFlags.CenterVertical);

                        // tag is the reference to the fragment to navigate to
                        btnChildMenuItem.Tag = loChildItem.ItemTag;


                        // initialize our typeface 
                        Typeface loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnMenuPopupButtonChildActionTypeface);
                        if (loCustomTypeface != null)
                        {
                            btnChildMenuItem.Typeface = loCustomTypeface;
                        }
                        btnChildMenuItem.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnMenuPopupButtonChildActionTypefaceSizeSp);

                        btnChildMenuItem.Click += btnOKClick;
                        _menuScrollViewLinearLayout.AddView(btnChildMenuItem, layoutParamsButtonChild);
                    }
                }


            }

        }



        public void SetUserCommentDialogItems( string iMenuTitleText, string iCommentLabelText, string[] iSuggestedItems )
        {
            gMenuTitleText = iMenuTitleText;
            _CommentLabelText = iCommentLabelText;
            _SuggestedItems = iSuggestedItems;
	    }

     




    }  // end class UserCommentPopUpDialogFragment


}

