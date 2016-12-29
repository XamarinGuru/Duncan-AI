

using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.Preferences;
using Reino.ClientConfig;
using Duncan.AI.Droid.Common;
using Duncan.AI.Droid.Utils.HelperManagers;
using System.Data;



namespace Duncan.AI.Droid
{
	public class SearchMatchFragment : DialogFragment
	{

        string _tagName;
        string _structName;


        View _dialogView;
        AlertDialog _thisDialog = null; 

        TextView _txtSummaryHeaderText;
        
        ListView _listView;


        TSearchMatchForm _SearchMatchResultsForm;
        SearchParameterPacket _SearchResult;



        //public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        //{
        //    // AJW - always call the superclass
        //    base.OnCreateView(inflater, container, savedInstanceState);

        //    _structName = Arguments.GetString("structName", null);

        //    _tagName = Helper.BuildSearchMatchFragmentTag(_structName);


        //    View view = inflater.Inflate(Resource.Layout.SearchMatchFragment, null);



        //    _txtSummaryHeaderText = view.FindViewById<TextView>(Resource.Id.txtSummaryHeaderText);
        //    _txtSummaryHeaderText.Text = _SearchMatchResultDTOList.Count.ToString() + " Matching " + _SearchMatchResultsForm.Parent.Name + " Record";



        //    _listView = view.FindViewById<ListView>(Resource.Id.list);

        //    if (_listView != null)
        //    {
        //        if (_SearchMatchResultDTOList != null)
        //        {

        //            _listView.Adapter = new CustomSearchMatchAdapter(Activity, _SearchMatchResultDTOList);
        //        }
        //    }


        //    //SetListItemClick();

        //    _btnBack = view.FindViewById<Button>(Resource.Id.btnBack);
        //    _btnBack.Click += BtnBackClick;


        //    _btnDone = view.FindViewById<Button>(Resource.Id.btnDone);
        //    _btnDone.Click += BtnDoneClick;



        //    return view;
        //}

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {

            _structName = Arguments.GetString("structName", null);

            _tagName = Helper.BuildSearchMatchFragmentTag(_structName);


            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);


            // Inflate and set the layout for the dialog -  Pass null as the parent view because its going in the dialog layout
            _dialogView = Activity.LayoutInflater.Inflate(Resource.Layout.SearchMatchFragment, null);

            // Pass null as the parent view because its going in the dialog layout
            builder.SetView(_dialogView);

            // if we set these, we'll get blank lines
            //builder.SetTitle("Search Results");
            //builder.SetMessage("");

            string loRecordSuffixStr;
            if (_SearchResult.fSearchResultDTOList.Count > 1)
            {
                loRecordSuffixStr = "s";
            }
            else
            {
                loRecordSuffixStr = "";
            }


            _txtSummaryHeaderText = _dialogView.FindViewById<TextView>(Resource.Id.txtSummaryHeaderText);
            _txtSummaryHeaderText.Gravity = (GravityFlags.CenterVertical | GravityFlags.CenterHorizontal);

            // initialize our typeface 
            Helper.SetTypefaceForTextView(_txtSummaryHeaderText, FontManager.cnCardViewDialogHeaderTextTypeface, FontManager.cnCardViewDialogHeaderTextTypefaceSizeSp);




            _txtSummaryHeaderText.Text = _SearchResult.fSearchResultDTOList.Count.ToString() + " Matching " + _SearchMatchResultsForm.Parent.Name + " Record" + loRecordSuffixStr;


            _listView = _dialogView.FindViewById<ListView>(Resource.Id.list);

            if (_listView != null)
            {
                _listView.DividerHeight = 0;

                if (_SearchResult.fSearchResultDTOList != null)
                {
                    _listView.Adapter = new CustomSearchMatchAdapter(Activity, _SearchResult);
                }

                SetListItemClick();
            }


            //gotta call back the frag that called you!
            //if (string.IsNullOrEmpty(gCallingFragmentTagName) == true)
            //{
            //    gCallingFragmentTagName = "PARKING";
            //}



            builder.SetPositiveButton("EXIT", delegate
            {
                // EXIT just closes the dialog
                // If they want to write a ticket they use th button inside the list view
                this.Dismiss();


                // launched from the listview

                //if (_SearchResult.fSearchStruct.StructLogicObj != null)
                //{
                //    if (_SearchResult.fSearchStruct.StructLogicObj is SearchStructLogicAndroid)
                //    {
                //        // if they didn't select one, just return the first
                //        if (_SearchResult.SearchResultSelectedRow == null)
                //        {
                //            _SearchResult.SearchResultSelectedRow = _SearchResult.fSearchResultDTOList[0].rawRow;
                //        }

                //        ((SearchStructLogicAndroid)(_SearchResult.fSearchStruct.StructLogicObj)).HandleSearchResult_DialogCallback(_SearchResult );
                //    }
                //}


            });


            _thisDialog = builder.Create();

            // call this now to initialize vars for customization
            _thisDialog.Show();
           

            // customize buttons - has to execute after .Show()
            CustomizeSearchMatchDialog();


            if (_SearchResult != null)
            {
                // set this so we can dismiss
                _SearchResult._parentSelectionDialog = _thisDialog;
            }


            return _thisDialog;
        }



        public void CustomizeSearchMatchDialog()
        {
            int cnMarginExitLeft = 100;
            int cnMarginExitTop = 25;
            int cnMarginExitRight = 50;
            int cnMarginExitBottom = 25;

            Button btnExitMenu = _thisDialog.GetButton((int)Android.Content.DialogButtonType.Positive);
            if (btnExitMenu != null)
            {
                // lets set the buttons sizes dynamically by screen size
                var metric = Resources.DisplayMetrics;
                int loButtonWidth = (int)(metric.WidthPixels * .33);

                // KLDUGE- should be done dynamically - what is the width of the AlertDialog?
                int loDialogWidth = metric.WidthPixels - 150;


                // get the actual width from the view - this doesn't work....
                View v = _thisDialog.CurrentFocus;
                if (v != null)
                {
                    v.Measure(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.WrapContent);
                    loDialogWidth = v.MeasuredWidth;
                }

                // calculate margin to center in dialog
                //int loButtonMarginLeft = (int)((metric.WidthPixels - loButtonWidth) / 2);
                int loButtonMarginLeft = (int)((loDialogWidth - loButtonWidth) / 2);



                btnExitMenu.SetBackgroundResource(Resource.Drawable.button_menu_popup_exit);
                btnExitMenu.SetTextColor(Resources.GetColor(Resource.Color.ok_green));


                btnExitMenu.Text = Constants.cnMenuPopup_ExitText;
                btnExitMenu.Gravity = (GravityFlags.Left | GravityFlags.CenterVertical);

                btnExitMenu.Tag = Constants.cnMenuPopup_ExitText;

                // initialize our typeface 
                Helper.SetTypefaceForButton(btnExitMenu, FontManager.cnMenuPopupButtonExitTypeface, FontManager.cnMenuPopupButtonExitTypefaceSizeSp);


                LinearLayout.LayoutParams layoutParamsButtonExit = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);

                // calculated placement
                //layoutParamsButtonExit.SetMargins(cnMarginExitLeft, cnMarginExitTop, cnMarginExitRight, cnMarginExitBottom);
                layoutParamsButtonExit.SetMargins(loButtonMarginLeft, cnMarginExitTop, 0, cnMarginExitBottom);

                layoutParamsButtonExit.Gravity = GravityFlags.NoGravity;


                // gravity doesnt work... because parent is not horizontal layout?
                //;ayoutParamsButtonExit.Gravity = GravityFlags.Right;
                //layoutParamsButtonExit.Gravity = GravityFlags.CenterVertical | GravityFlags.CenterHorizontal;


                btnExitMenu.LayoutParameters = layoutParamsButtonExit;
                btnExitMenu.RequestLayout();

            }
        }


        public void SetSearchMatchResults(TSearchMatchForm iSearchMatchResultsForm, SearchParameterPacket iSearchResult)
	    {
            _SearchMatchResultsForm = iSearchMatchResultsForm;
            _SearchResult = iSearchResult;
            //_listView.Adapter = new CustomSearchMatchAdapter(Activity, _SearchMatchResultDTOList);
	    }

     


	    void SetListItemClick()
        {
           
            _listView.ItemClick += (sender, e) =>
            {
                // TODO - need a way to HIGHLIGHT the selected record?
                _SearchResult.SearchResultSelectedRow = _SearchResult.fSearchResultDTOList[e.Position].rawRow;
                _SearchResult.SearchResultSelectedRowIndex = e.Position;


                //ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
                //ISharedPreferencesEditor editor = prefs.Edit();

                //ParkNoteDTO parkNoteDTO = _SearchMatchResultDTOList[e.Position];

                ////editor.PutString(Constants.PARKNOTE_ROWID, parkNoteDTO.DBRowId);
                ////editor.Apply();

                ////LoadNoteDetails();
			};
        }


    }  // end class SearchMatchFragment


}

