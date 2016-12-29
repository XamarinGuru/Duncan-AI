using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.Preferences;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Duncan.AI.Droid
{
    public class TicketsFragment : Fragment
    {
        ListView _listView;
        List<CommonDTO> _orgCommonDTOList;
        List<CommonDTO> _commonDTOList;
        List<string> _tablesNames;
        List<string> _spinnerValues;
        Spinner _spinner;
        EditText _vehNoSearch;
        EditText _issueNoSearch;
        EditText _locStreetSearch;
        Button _searchButton;
        Boolean _spinnerSecondCall;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // AJW - always call the superclass
            base.OnCreateView(inflater, container, savedInstanceState);


            View view = inflater.Inflate(Resource.Layout.Tickets_fragment, null);

            //List view to show list of User locations by user
            _listView = view.FindViewById<ListView>(Resource.Id.list);
            _vehNoSearch = view.FindViewById<EditText>(Resource.Id.vehNoSearch);
            _issueNoSearch = view.FindViewById<EditText>(Resource.Id.issueNoSearch);
            _locStreetSearch = view.FindViewById<EditText>(Resource.Id.locStreetSearch);
            _searchButton = view.FindViewById<Button>(Resource.Id.searchButton);
            _searchButton.Click += SearchBtnHandleClick;
            
            List<XMLConfig.IssStruct> structs = DroidContext.XmlCfg.IssStructs;
            _tablesNames = new List<string>();
            _spinnerValues = new List<string>();
            _spinnerValues.Add("All");

            foreach (var structI in structs)
            {
                if (
                     structI.Type.Equals(Constants.STRUCT_TYPE_CITE) ||
                     structI.Type.Equals(Constants.STRUCT_TYPE_CHALKING) ||
                     structI.Type.Equals(Constants.STRUCT_TYPE_GENERIC_ISSUE)
                    )
                {
                    _tablesNames.Add(structI.Name);
                    _spinnerValues.Add(structI.Name);
                }
            }

            _spinner = view.FindViewById<Spinner>(Resource.Id.ticketTypeSpinner);
            _spinner.ItemSelected += spinner_ItemSelected;
            var adapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleSpinnerItem, _spinnerValues);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spinner.Adapter = adapter;
                        
            SetListItemClick();

            // hide the keyboard if it was open
            Helper.HideKeyboard(view);

            return view;
        }



        void SearchBtnHandleClick(object sender, EventArgs ev)
        {
            if (!string.IsNullOrEmpty(_vehNoSearch.Text)
                || !string.IsNullOrEmpty(_issueNoSearch.Text)
                || !string.IsNullOrEmpty(_locStreetSearch.Text))
            {
                GetTickets(_spinner.SelectedItemPosition);              
            }
            else
            {
                GetTickets(0);              
            }
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (!_spinnerSecondCall)
            {
                GetTickets(e.Position);
            }
            else
            {
                _spinnerSecondCall = false;
            }
            
        }

        //Async Task to retrieve Tickets by the officer name from DB
        public async Task<List<CommonDTO>> GetTickets(int position)
        {
            _commonDTOList = new List<CommonDTO>();
            _spinnerSecondCall = true;
            _spinner.SetSelection(position, false);
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
                result = commonADO.GetTableRows(_tablesNames, queryStringBuilder.ToString());
            }
            else
            {
                List<string> _tabNames = new List<string>();
                _tabNames.Add(_spinnerValues[position]);
                result = commonADO.GetTableRows(_tabNames, queryStringBuilder.ToString());
            }      

            // await! control returns to the caller and the task continues to run on another thread
            _orgCommonDTOList = await result;

            if (_orgCommonDTOList != null)
            {
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
                _listView.SetAdapter(new CustomBaseAdapter(Activity, _commonDTOList));
            }
                
            return _orgCommonDTOList;
        }


        public void MyFakeOCR()
        {

            //CustomBaseAdapter myANPRWrapper = new CustomBaseAdapter();

            //myANPRWrapper.SetImageFilePath( string iImageToProcess );

            //myANPRWrapper.SetOtherParameters( ... );

            //myANPRWrapper.ProcessImageForANPR();

            //myANPRWrapper.GetResultingStatusInformation( statistics, confidence level, errors, issues  );

            //myANPRWrapper.GetResutingANPRText( string oLicensePlateCharacters );

            //myANPRWrapper.Dispose();

            //myANPRWrapper = null;


        }


        public void SetListItemClick()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            ISharedPreferencesEditor editor = prefs.Edit();

            _listView.ItemClick += (sender, e) =>
            {
                FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
                Fragment ticketsFragment = FragmentManager.FindFragmentByTag(Constants.TICKETS_FRAGMENT_TAG);
                if (ticketsFragment != null)
                {
                    fragmentTransaction.Hide(ticketsFragment);
                }
                CommonDTO commonDTO = _commonDTOList[e.Position];

                editor.PutString(Constants.STRUCT_NAME_TKT_DTL, commonDTO.structName);
                if (Constants.MARKMODE_TABLE.Equals(commonDTO.structName))
                {
                    editor.PutString(Constants.TICKETID, commonDTO.rowId);
                }
                else
                {
                    editor.PutString(Constants.TICKETID, commonDTO.seqId);
                }
               
                editor.Apply();

                var dtlFragment = (TicketDetailFragment)FragmentManager.FindFragmentByTag(Constants.DETAIL_FRAGMENT_TAG);
                if (dtlFragment != null)
                {
                    fragmentTransaction.Show(dtlFragment);
                    dtlFragment.CreateLayout();
                }
                else
                    fragmentTransaction.Replace(Resource.Id.frameLayout1, new TicketDetailFragment(), Constants.DETAIL_FRAGMENT_TAG);
                
                fragmentTransaction.Commit();
            };
        }
    }
}

