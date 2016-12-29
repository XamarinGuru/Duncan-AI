#define _suppress_datatype_spinner_

using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.Preferences;
using Android.Text;
using Duncan.AI.Droid.Common;
using Duncan.AI.Droid.Utils;
using Duncan.AI.Droid.Utils.HelperManagers;
using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace Duncan.AI.Droid
{

    /// <summary>
    /// IssueSelect form (lookup / search / review) for specific issue struct
    /// </summary>
    public class IssueSelectFragment : Fragment
    {

        XMLConfig.IssStruct _struct;

        string _structName = string.Empty;
        string _tagName = string.Empty;


        View _issueSelectFragmentRootView;

        ListView _listView;
        List<CommonDTO> _orgCommonDTOList;
        List<CommonDTO> _commonDTOList;
        List<string> _tablesNames;


        List<string> _spinnerValues;
        Spinner _spinner;
        TextView _spinnerLabel;

        // set to the struct name index
        int gDefaultSpinnerPosition = -1;

        
        EditText _vehNoSearch;
        EditText _issueNoSearch;
        EditText _locStreetSearch;

        Button _searchButton;
        Boolean _spinnerSecondCall;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // AJW - always call the superclass
            base.OnCreateView(inflater, container, savedInstanceState);


            _structName = Arguments.GetString("structName", null);
            _struct = DroidContext.XmlCfg.GetStructByName(_structName);

            _tagName = Helper.BuildIssueSelectFragmentTag(_structName);

            _issueSelectFragmentRootView = inflater.Inflate(Resource.Layout.IssueSelectFragment, null);

            _listView = _issueSelectFragmentRootView.FindViewById<ListView>(Resource.Id.list);
            _listView.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);




            ////////


            var loToolbarLinearLayout = new LinearLayout(Activity);
            loToolbarLinearLayout.SetGravity(GravityFlags.Center);

                //LinearLayout layout_horiz_btns = view.FindViewById<LinearLayout>(Resource.Id.NavButtons);  
                //layout_horiz_btns.Orientation = Orientation.Horizontal;

                //var layoutParamsButton = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                var layoutParamsButton = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,  ViewGroup.LayoutParams.WrapContent);
                layoutParamsButton.SetMargins(10, 25, 10, 25);


                var btnIssueFormNavigation = _issueSelectFragmentRootView.FindViewById<Button>(Resource.Id.btnIssueFormNavigation);
            btnIssueFormNavigation.SetBackgroundResource(Resource.Drawable.button_issueform_navigation);

            //brnIssueFormNavigation.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.button_issueform_navigation));

            btnIssueFormNavigation.SetTextColor(Android.Graphics.Color.Black);
            //brnIssueFormNavigation.SetBackgroundColor(Color.Rgb(0xff, 0x6e, 0x2b)); // "#ff6e2b");


            // initialize our typeface 
            Typeface loCustomTypefaceParent = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnIssueFormNavigationTypeface);
            if (loCustomTypefaceParent != null)
            {
                btnIssueFormNavigation.Typeface = loCustomTypefaceParent;
            }
            btnIssueFormNavigation.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnIssueFormNavigationTypefaceSizeSp);


            string loNavigationTitleText = "Parking Look Up"; //  ParentFragment.GetFragmentMenuItemName();
            btnIssueFormNavigation.Text = loNavigationTitleText;
            btnIssueFormNavigation.Gravity = (GravityFlags.Left | GravityFlags.CenterVertical);


            //btnPrevListItem.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_list_news_prev_32_rev2, 0, 0, 0);
            //brnIssueFormNavigation.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_list_news_prev_32_rev3, 0, 0, 0);


            btnIssueFormNavigation.Tag = "btnIssueFormNavigation";
            btnIssueFormNavigation.Click += btnIssueFormNavigationClick;
            btnIssueFormNavigation.LayoutParameters = layoutParamsButton;





            ///////////////


            // construct a divider to help in navigation
            TextView oneTextViewPanelDivider = _issueSelectFragmentRootView.FindViewById<TextView>(Resource.Id.efDividerFilter);
            oneTextViewPanelDivider.Text = " Filters";

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



            int loDividerCount = 0;
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

            oneTextViewPanelDivider.LayoutParameters = lpPanelDivider;




            //////////////////

            int loMaxSearchFilterLen = 25;   // TODO - would be nice if this could come from XML when available

            _vehNoSearch = _issueSelectFragmentRootView.FindViewById<EditText>(Resource.Id.vehNoSearch);
            _vehNoSearch.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);
            _vehNoSearch.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(loMaxSearchFilterLen), new InputFilterAllCaps() });


            _issueNoSearch = _issueSelectFragmentRootView.FindViewById<EditText>(Resource.Id.issueNoSearch);
            _issueNoSearch.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);
            _issueNoSearch.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(loMaxSearchFilterLen), new InputFilterAllCaps() });

            _locStreetSearch = _issueSelectFragmentRootView.FindViewById<EditText>(Resource.Id.locStreetSearch);
            _locStreetSearch.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);
            _locStreetSearch.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(loMaxSearchFilterLen), new InputFilterAllCaps() });




            _searchButton = _issueSelectFragmentRootView.FindViewById<Button>(Resource.Id.searchButton);
            _searchButton.Click += SearchBtnHandleClick;
            
            List<XMLConfig.IssStruct> structs = DroidContext.XmlCfg.IssStructs;
            _tablesNames = new List<string>();


            _spinnerValues = new List<string>();
            _spinnerValues.Add("ALL");



            foreach (var structI in structs)
            {
                // TODO - filter out everything but cite with print images, until we have display formats for markmode and other non-printed records
                if (structI._TIssForm != null)
                {
                    if (structI.PrintPicture != null) 
                    {

                        //// TODO  AJW review - PrintPicture lists is created by form type... but it only has a print print picture if elements are defined for it
                        //if (loPrintPicRev.AllPrnDataElements.Count > 0)
                        //{
                        //}

                        //_tablesNames.Add(structI.Name);
                        //_spinnerValues.Add(structI.Name);

                        _tablesNames.Add(structI._TIssStruct.MainTable.Name);
                        _spinnerValues.Add(structI.Name);
                    }
                }


                //if (
                //     structI.Type.Equals(Constants.STRUCT_TYPE_CITE) ||
                //     structI.Type.Equals(Constants.STRUCT_TYPE_CHALKING) ||
                //     structI.Type.Equals(Constants.STRUCT_TYPE_GENERIC_ISSUE)
                //    )
                //{
                //    _tablesNames.Add(structI.Name);
                //    _spinnerValues.Add(structI.Name);
                //}


            }

            _spinner = _issueSelectFragmentRootView.FindViewById<Spinner>(Resource.Id.ticketTypeSpinner);
            _spinner.ItemSelected += spinner_ItemSelected;


            _spinnerLabel = _issueSelectFragmentRootView.FindViewById<TextView>(Resource.Id.ticketTypeSpinnerLabel);


            var adapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleSpinnerItem, _spinnerValues);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spinner.Adapter = adapter;

            
            // start struct on our data type
            gDefaultSpinnerPosition = _spinnerValues.IndexOf(_struct.Name);
            if (gDefaultSpinnerPosition != -1)
            {
                _spinner.SetSelection(gDefaultSpinnerPosition, false);
            }


#if _suppress_datatype_spinner_
            // disable for now, only one data type per review form
            _spinnerLabel.Visibility = ViewStates.Gone;
            _spinner.Visibility = ViewStates.Gone;
#endif

            SetListItemClick();

            // hide the keyboard if it was open
            Helper.HideKeyboard(_issueSelectFragmentRootView);

            return _issueSelectFragmentRootView;
        }


        void btnIssueFormNavigationClick(object sender, EventArgs e)
        {
            DroidContext.mainActivity.MenuPopUp_IssueFormSelection();
        }



        void GetCiteDetailInfo( List<CommonDTO> iItems )
        {

            try
            {
                if (_struct != null)
                {
                    if (_struct._TIssStruct.StructLogicObj != null)
                    {
                        foreach (CommonDTO oneItem in iItems)
                        {

                            if (string.IsNullOrEmpty(oneItem.rowId.Trim()) == true)
                            {
                                // empty row ID - nothing to look for
                                continue;
                            }



                            if (_struct._TIssStruct.StructLogicObj is CiteStructLogicAndroid)
                            {
                                Int32 loRowIDAsInt = -1;
                                Int32.TryParse(oneItem.rowId, out loRowIDAsInt);
                                if (loRowIDAsInt >= 0)
                                {
                                    // is it void? null if not
                                    oneItem.RawDetailRowVoid =   ((CiteStructLogicAndroid)_struct._TIssStruct.StructLogicObj).GetFirstVoidRecordForMasterKey(loRowIDAsInt);

                                    // is it reissued? null if not
                                    oneItem.RawDetailRowReissue = ((CiteStructLogicAndroid)_struct._TIssStruct.StructLogicObj).GetFirstReissueRecordForMasterKey(loRowIDAsInt);

                                    // is it a continuance? null if not
                                    oneItem.RawDetailRowContinuance = ((CiteStructLogicAndroid)_struct._TIssStruct.StructLogicObj).GetFirstContinuanceRecordForMasterKey(loRowIDAsInt);

                                }

                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, e.Source, "GetCiteDetailInfo");
                Console.WriteLine("Exception source: {0}", e.Source);
            }

        }


        void SearchBtnHandleClick(object sender, EventArgs ev)
        {

            // hide the keyboard if it was open
            Helper.HideKeyboard(_issueSelectFragmentRootView);


            GetMatchingRecords(_spinner.SelectedItemPosition);              
/*
            if (!string.IsNullOrEmpty(_vehNoSearch.Text)
                || !string.IsNullOrEmpty(_issueNoSearch.Text)
                || !string.IsNullOrEmpty(_locStreetSearch.Text))
            {
                GetMatchingRecords(_spinner.SelectedItemPosition);              
            }
            else
            {
                GetMatchingRecords(0);              
            }
            */
        }


        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (!_spinnerSecondCall)
            {
                GetMatchingRecords(e.Position);
            }
            else
            {
                _spinnerSecondCall = false;
            }
            
        }

        public void DisplayMatchingRecordsForDefaultStruct()
        {
            if ( gDefaultSpinnerPosition != 1)
            {
                GetMatchingRecords(gDefaultSpinnerPosition);              
            }
            else
            {
                GetMatchingRecords(0);              
            }

        }


        //Async Task to retrieve Tickets by the officer name from DB
        public async Task<List<CommonDTO>> GetMatchingRecords(int position)
        {



            _commonDTOList = new List<CommonDTO>();

            _spinnerSecondCall = true;


#if _suppress_datatype_spinner_
            // override
            position = 0;
#else

            _spinner.SetSelection(position, false);
#endif


            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            string officerName = prefs.GetString(Constants.OFFICER_NAME, null);
            var commonADO = new CommonADO(); 
            Task<List<CommonDTO>> result = null;

            //Build Where Clause
            var queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append(" WHERE ");
            queryStringBuilder.Append(Constants.OFFICER_NAME);
            queryStringBuilder.Append(" = '");
            queryStringBuilder.Append(officerName);
            queryStringBuilder.Append("' and ");
            queryStringBuilder.Append(Constants.STATUS_COLUMN);
            queryStringBuilder.Append(" != '");
            queryStringBuilder.Append(Constants.STATUS_INPROCESS);
            queryStringBuilder.Append("'");
            

            if(position == 0)
            {
                // get records from all structs using query parameters
                result = commonADO.GetTableRows(_tablesNames, queryStringBuilder.ToString());
            }
            else
            {
                // only get records from target struct using query parameters
                List<string> _tabNames = new List<string>();
                _tabNames.Add(_spinnerValues[position]);
                result = commonADO.GetTableRows(_tabNames, queryStringBuilder.ToString());
            }      

            // await! control returns to the caller and the task continues to run on another thread
            _orgCommonDTOList = await result;

            if (_orgCommonDTOList != null)
            {
                
                // filter out the records according to user selections
                foreach(CommonDTO commonDTO in _orgCommonDTOList)
                {
                    if (!string.IsNullOrEmpty(_vehNoSearch.Text)
                       || !string.IsNullOrEmpty(_issueNoSearch.Text)
                       || !string.IsNullOrEmpty(_locStreetSearch.Text))
                    {
                        if (!string.IsNullOrEmpty(_vehNoSearch.Text) 
                            && !string.IsNullOrEmpty(commonDTO.sqlVehLicNoStr)
                            && Regex.IsMatch(commonDTO.sqlVehLicNoStr, _vehNoSearch.Text, RegexOptions.IgnoreCase))                           
                        {
                            _commonDTOList.Add(commonDTO);
                        }

                        if (!string.IsNullOrEmpty(_issueNoSearch.Text)
                            && !string.IsNullOrEmpty(commonDTO.seqId)
                            && Regex.IsMatch(commonDTO.seqId, _issueNoSearch.Text, RegexOptions.IgnoreCase))
                        {
                            _commonDTOList.Add(commonDTO);
                        }

                        if (!string.IsNullOrEmpty(_locStreetSearch.Text)
                            && !string.IsNullOrEmpty(commonDTO.sqlLocStreetStr)
                            && Regex.IsMatch(commonDTO.sqlLocStreetStr, _locStreetSearch.Text, RegexOptions.IgnoreCase))
                        {
                            _commonDTOList.Add(commonDTO);
                        }
                    }
                    else
                    {
                        _commonDTOList.Add(commonDTO);
                    }
                }

                // get the child record details as needed
                GetCiteDetailInfo(_commonDTOList);



                //_listView.SetAdapter(new CustomBaseAdapter(Activity, _commonDTOList));
                _listView.Adapter = new CustomIssueStructRecordLookupAdapter(Activity, _commonDTOList);
            }
              
            // return the filtered lists
            //return _orgCommonDTOList;
            return _commonDTOList;
        }


        public void SetListItemClick()
        {

            // on a selected item, switch to the detailed review of that record type
            _listView.ItemClick += (sender, e) =>
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
                ISharedPreferencesEditor editor = prefs.Edit();

                // save where we came from
                editor.PutString(Constants.PREVIOUS_FRAGMENT, _tagName);

                DroidContext.MyFragManager.AddToInternalBackstack(_tagName);


                FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
                Fragment issueSelectFragment = FragmentManager.FindFragmentByTag(_tagName);
                if (issueSelectFragment != null)
                {
                    // needed because this is a top level fragment that is managed by hide/view
                    if (issueSelectFragment.View != null)
                    {
                        issueSelectFragment.View.Visibility = ViewStates.Gone;
                    }
                    
                    fragmentTransaction.Hide(issueSelectFragment);
                }



                CommonDTO loSelectedDTO = _commonDTOList[e.Position];

                editor.PutString(Constants.STRUCT_NAME_TKT_DTL, loSelectedDTO.structName);

                //if (Constants.MARKMODE_TABLE.Equals(loSelectedDTO.structName))
                //{
                //    editor.PutString(Constants.TICKETID, loSelectedDTO.rowId);
                //}
                //else
                //{
                //    editor.PutString(Constants.TICKETID, loSelectedDTO.seqId);
                //}

                editor.PutString(Constants.ID_COLUMN, loSelectedDTO.rowId);
                editor.PutString(Constants.TICKETID, loSelectedDTO.seqId);


                editor.Apply();




                // what data type did they select? this is the data type we need
                string loTargetFragmentTag = Helper.BuildIssueReviewFragmentTag(loSelectedDTO.structName);
        
                var dtlFragment = (IssueReviewDetailFragment)FragmentManager.FindFragmentByTag(loTargetFragmentTag);
                if (dtlFragment != null)
                {
                    fragmentTransaction.Show(dtlFragment);
                    dtlFragment.RefreshDisplayedRecord();
                }
                else
                {
                    var fragment = new IssueReviewDetailFragment { Arguments = new Bundle() };
                    fragment.Arguments.PutString("structName", loSelectedDTO.structName);

                    //MainActivity.RegisterFragment(fragment, loTag, _structName, "NoMenu " + loTag, FragmentClassificationType.fctSecondaryActivity, -1, -1);
                    fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, loTargetFragmentTag);

                    //fragmentTransaction.Replace(Resource.Id.frameLayout1, new IssueReviewDetailFragment(), Constants.ISSUE_REVIEW_DETAIL_FRAGMENT_TAG);

                    fragment.RefreshDisplayedRecord(); // ok here?

                }
                
                fragmentTransaction.Commit();
            };
        }
    }
}

