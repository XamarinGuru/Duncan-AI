

using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
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
using Duncan.AI.Droid.Utils.HelperManagers;


/// <summary>
/// Menu Popup Dialog to navigate to another fragment/activiy
/// </summary>

namespace Duncan.AI.Droid
{
	public class MenuPopUpDialogFragment : DialogFragment
	{

        string _tagName;
        string _structName;

        private AlertDialog _thisDialog = null;  // there must be a way to reference this

        View _dialogView;
        TextView _menuTitle;
        ScrollView _menuScrollView;
        LinearLayout _menuScrollViewLinearLayout;


        string gMenuTitleText = string.Empty;
        List<MainActivity.TMenuItemParent> gParentItems = new List<MainActivity.TMenuItemParent>();
        List<MainActivity.TMenuItemChild> gChildItems = new List<MainActivity.TMenuItemChild>();



        // margins
        int cnMarginTitleLeft = 50;
        int cnMarginTitleTop = 15;
        int cnMarginTitleRight = 200;
        int cnMarginTitleBottom = 15;


        int cnMarginParentLeft = 50;
        int cnMarginParentTop = 5;
        int cnMarginParentRight = 200;
        int cnMarginParentBottom = 5;

        int cnMarginChildLeft = 50;
        int cnMarginChildTop = 1;
        int cnMarginChildRight = 50;
        int cnMarginChildBottom = 1;

        int cnMarginExitLeft = 0;
        int cnMarginExitTop = 25;
        int cnMarginExitRight = 50;
        int cnMarginExitBottom = 25;


        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {

            _structName = Arguments.GetString("structName", null);

            _tagName = "menu_popup"; // get from parameters if really needed // Helper.BuildMenuPopUpDialogFragmentTag(_structName);




            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);

           

            // Inflate and set the layout for the dialog -  Pass null as the parent view because its going in the dialog layout
            _dialogView = Activity.LayoutInflater.Inflate(Resource.Layout.MenuPopUpDialogFragment, null);


            // Pass null as the parent view because its going in the dialog layout
            builder.SetView(_dialogView);
            

            // if we set these, we'll get blank lines
            //builder.SetTitle("");
            //builder.SetMessage("");


            _menuTitle = _dialogView.FindViewById<TextView>(Resource.Id.title);
            if (_menuTitle != null)
            {
                _menuTitle.Text = gMenuTitleText;
                _menuTitle.Gravity = (GravityFlags.Left | GravityFlags.CenterVertical);

                // initialize our typeface 
                Typeface loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnMenuPopupTitleTypeface);
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

            if (_menuScrollView != null)
            {
                if ((gParentItems != null) && (gChildItems != null))
                {
                    BuildMenuItems();
                }


                // create a layout to hold the exit button
                LinearLayout layoutHoriz = new LinearLayout(Activity) { Orientation = Android.Widget.Orientation.Horizontal };

                // lets set the buttons sizes dynamically by screen size
                var metric = Resources.DisplayMetrics;
                int loButtonWidth = (int)(metric.WidthPixels * .33);

                // add EXIT button on bottom of pop-up menu
                Button btnExitMenu = new Button(Activity);

                btnExitMenu.SetBackgroundResource(Resource.Drawable.button_menu_popup_exit);
                btnExitMenu.SetTextColor(Resources.GetColor(Resource.Color.ok_green));


                btnExitMenu.Text = Constants.cnMenuPopup_ExitText;
                btnExitMenu.Gravity = (GravityFlags.Left | GravityFlags.CenterVertical);

                btnExitMenu.Tag = Constants.cnMenuPopup_ExitText;

                // initialize our typeface 
                Helper.SetTypefaceForButton(btnExitMenu, FontManager.cnMenuPopupButtonExitTypeface, FontManager.cnMenuPopupButtonExitTypefaceSizeSp);


                LinearLayout.LayoutParams layoutParamsButtonExit = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
                layoutParamsButtonExit.SetMargins(cnMarginExitLeft, cnMarginExitTop, cnMarginExitRight, cnMarginExitBottom);
                layoutParamsButtonExit.Gravity = GravityFlags.Right;
                btnExitMenu.LayoutParameters = layoutParamsButtonExit;

               // btnExitMenu.SetWidth(loButtonWidth);


               // var layoutParamsButtonExit = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
                //layoutParamsButtonExit.SetMargins(cnMarginExitLeft, cnMarginExitTop, cnMarginExitRight, cnMarginExitBottom);
               //layoutParamsButtonExit.AddRule(LayoutRules.AlignParentRight);

                //var layoutParamsButtonExit = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                //layoutParamsButtonExit.SetMargins(cnMarginExitLeft, cnMarginExitTop, cnMarginExitRight, cnMarginExitBottom);
                //layoutParamsButtonExit.AddRule(LayoutRules.AlignParentRight);


                //btnExitMenu.SetWidth(loButtonWidth);


                btnExitMenu.Click += btnActionButtonClick;

                
                _menuScrollViewLinearLayout.AddView(btnExitMenu, layoutParamsButtonExit);

                // add button to container view
                //layoutHoriz.AddView(btnExitMenu, layoutParamsButtonExit);

                // add container view to layour
                //_menuScrollViewLinearLayout.AddView(layoutHoriz);


            }


            // dont want dialog buttons at bottom
            //builder.SetPositiveButton("DONE", delegate
            //{
            //    this.Dismiss();
            //});


            //builder.SetNegativeButton("CANCEL", delegate
            //{
            //});


            _thisDialog = builder.Create();

            return _thisDialog;
        }



        public interface MenuPopupDialogListener
        {
            void OnFinishMenuPopUpDialog(string iSelectedItemTag);
        }

        private MenuPopupDialogListener resultListener;

        async void btnActionButtonClick(object sender, EventArgs e)
        {
            if ((sender is Button) == false)
            {
                return;
            }

            Button oneButtonClicked = (Button)sender;
            string loButtonClickedTag = (string)oneButtonClicked.Tag;
            
            //Intent resultIntent = new Intent();
            //resultIntent.PutExtra(AutoISSUE.DBConstants.sqlVehLicNoStr, loButtonClickedTag);
            //this.TargetFragment.OnActivityResult(Constants.ACTIVITY_REQUEST_CODE_MENU_POPUP_NAVIGATION_RESULT, Result.Ok, resultIntent);

            _CallbackActivity.MenuPopUp_ItemSelectCallback(loButtonClickedTag);


            this.Dismiss();
        }

        private MainActivity _CallbackActivity;
        public void SetCallbackActivity( MainActivity iCallbackActivity )
        {
            _CallbackActivity = iCallbackActivity;
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

                        btnChildMenuItem.Click += btnActionButtonClick;
                        _menuScrollViewLinearLayout.AddView(btnChildMenuItem, layoutParamsButtonChild);
                    }
                }


            }

        }



        public void SetMenuItems( string iMenuTitleText, List<MainActivity.TMenuItemParent> iParentItems, List<MainActivity.TMenuItemChild> iChildItems )
        {
            gMenuTitleText = iMenuTitleText;
            gParentItems = iParentItems;
            gChildItems = iChildItems;
	    }

     




    }  // end class MenuPopUpDialogFragment


}

