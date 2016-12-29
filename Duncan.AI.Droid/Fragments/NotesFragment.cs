using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.Preferences;

namespace Duncan.AI.Droid
{
	public class NotesFragment : Fragment, ITapLock
	{

        string _tagName;
        string _structName;

        LinearLayout _Notes_List_Review_Toolbar;


        public TapLockVars TapLockVars
        { get; set; }

        ListView _listView;
        TextView _noNotes;
        Button _btnNew;
        List<ParkNoteDTO> _parkNoteDTOList;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
            // AJW - always call the superclass
            base.OnCreateView(inflater, container, savedInstanceState);


            _structName = Arguments.GetString("structName", null);
            //_struct = DroidContext.XmlCfg.GetStructByName(structName);

            _tagName = Helper.BuildIssueNotesFragmentTag(_structName);



			View view = inflater.Inflate(Resource.Layout.Notes_fragment, null);


            // action toolbar 
            //_Notes_List_Review_Toolbar = view.FindViewById<LinearLayout>(Resource.Id.toolbar_bottom_review_layout);
            //if (_Notes_List_Review_Toolbar != null)
            //{
            //    _Notes_List_Review_Toolbar.SetBackgroundResource(Resource.Drawable.autoissue_toolbar_finalizing_background);
            //}



            _btnNew = view.FindViewById<Button>(Resource.Id.btnNew);
            if (_btnNew != null)
            {
                _btnNew.Click += BtnNewClick;
            }




            _noNotes = view.FindViewById<TextView>(Resource.Id.NoNotes);

			_listView = view.FindViewById<ListView>(Resource.Id.list);
			GetNotesByTicket();
            SetListItemClick();

            var btnBack = view.FindViewById<Button>(Resource.Id.btnBack);
            btnBack.Click += BtnBackClick;
            
			return view;
		}

        //Async Task to retrieve Notes by a Ticket Id from DB
	    public async void GetNotesByTicket()
	    {
	        ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
	        String sequenceId = prefs.GetString(Constants.ISSUENO_COLUMN, null);

            // TODO - convert to master-detail key values
            string masterKey = prefs.GetString(Constants.ID_COLUMN, "");

	        var parkingSequenceADO = new ParkingSequenceADO();
	        Task<List<ParkNoteDTO>> result = parkingSequenceADO.GetParkNotesBySequenceId(sequenceId);

	        // await! control returns to the caller and the task continues to run on another thread
	        _parkNoteDTOList = await result;
            if (_parkNoteDTOList != null)
            {
                if(_listView != null)
                {
                    //_listView.Adapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, _parkNoteDTOList.Select(x => x.NotesMemo).ToArray());
                    string[] loObjects = _parkNoteDTOList.Select(x => x.NotesMemo).ToArray();
                    if (loObjects != null && loObjects.Count() > 0)
                    {
                        _listView.Adapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, loObjects);
                    }
                }
            }

            if (_noNotes != null)
            {
                _noNotes.Visibility = _parkNoteDTOList.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
            }

            if (_btnNew != null)
            {
                _btnNew.Visibility = ViewStates.Visible;
            }


            // AJW - bad! bad! repeatedly adding click delegates means that onclick will be fired repeately, once for each delegate!
            //_btnNew.Click += BtnNewClick;
            //_btnNew.Click -= BtnNewClick;
            //_btnNew.Click += BtnNewClick;
            // moved to OnCreate

	    }

     

	    void SetListItemClick()
        {
           
            _listView.ItemClick += (sender, e) =>
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
                ISharedPreferencesEditor editor = prefs.Edit();

                if (_parkNoteDTOList != null)
                {
                    ParkNoteDTO parkNoteDTO = _parkNoteDTOList[e.Position];

                    editor.PutString(Constants.PARKNOTE_ROWID, parkNoteDTO.DBRowId);
                    editor.Apply();
                }

                LoadNoteDetails();
			};
        }

        void LoadNoteDetails()
        {
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            //Fragment notesFragment = FragmentManager.FindFragmentByTag(Constants.NOTES_FRAGMENT_TAG);
            Fragment notesFragment = FragmentManager.FindFragmentByTag(_tagName);
            if (notesFragment != null)
            {
                fragmentTransaction.Hide(notesFragment);
            }
            
            // come back when you're done there
            DroidContext.MyFragManager.AddToInternalBackstack(_tagName);


            // this is the data type we need
            string loTargetFragmentTag = Helper.BuildIssueNoteDetailFragmentTag(_structName);

            var noteDetail = (NoteDetailFragment)FragmentManager.FindFragmentByTag(loTargetFragmentTag);
            //var noteDetail = (NoteDetailFragment)FragmentManager.FindFragmentByTag(Constants.NOTE_DETAIL_FRAGMENT_TAG);
            if (noteDetail != null)
            {
                fragmentTransaction.Show(noteDetail);
                noteDetail.CreateLayout();
            }
            else
            {
                // first time through
                var fragment = new NoteDetailFragment { Arguments = new Bundle() };
                fragment.Arguments.PutString("structName", _structName);

                //fragmentTransaction.Replace(Resource.Id.frameLayout1, fragment, prevFragName);
                //fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, prevFragName);
                //fragmentTransaction.Replace(Resource.Id.frameLayout1, new NoteDetailFragment(), Constants.NOTE_DETAIL_FRAGMENT_TAG);
                fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, loTargetFragmentTag);

                fragment.CreateLayout();  // needed here?
            }

            fragmentTransaction.Commit();
        }


        void BtnNewClick(object sender, EventArgs e)
        {

            try
            {
                if (this.AcquireTapLock())
                {

                    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
                    ISharedPreferencesEditor editor = prefs.Edit();
                    editor.PutString(Constants.PARKNOTE_ROWID, null);  // reset for new Note
                    editor.Apply();
                    LoadNoteDetails();
                }
            }
            finally
            {
                this.ReleaseTapLock();
            }
        }
        

        async void BtnBackClick(object sender, EventArgs e)
        {
            // go back to where we came from
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            String prevFragName = prefs.GetString(Constants.PREVIOUS_FRAGMENT, null);
            string structName = prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);

            // 
            prevFragName = DroidContext.MyFragManager.PopInternalBackstack();


            // hide ourselves
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            Fragment notesFragment = FragmentManager.FindFragmentByTag(_tagName);
            //Fragment notesFragment = FragmentManager.FindFragmentByTag(Constants.NOTES_FRAGMENT_TAG);
            if (notesFragment != null)
            {
                fragmentTransaction.Hide(notesFragment);
            }


            if (string.IsNullOrEmpty(prevFragName) == true)
            {
                // not sure where to go.... this is the data type we need
                prevFragName = Helper.BuildIssueReviewFragmentTag(_structName);
            }

            // debug check
            if (prevFragName.Equals(Helper.BuildIssueReviewFragmentTag(_structName)) == false)
            {
                prevFragName = prevFragName.Trim();
            }

            //if ( prevFragName.StartsWith( Constants.ISSUE_REVIEW_FRAGMENT_TAG_PREFIX ) == true )
            {

                // go to the ticket detail 
                //var dtlFragment = (IssueReviewDetailFragment)FragmentManager.FindFragmentByTag(prevFragName);
                //
                //if (dtlFragment != null)
                //{
                //    fragmentTransaction.Show(dtlFragment);
                //    dtlFragment.RefreshDisplayedRecord();
                //}

                // go to the the ticket detail... carefully
                //
                // DPANDROID-601
                // if this error occurs elsewhere, the same technique to be applied throughout fragment switching
                //
                Fragment dtlFragment =  FragmentManager.FindFragmentByTag(prevFragName);
                if ((dtlFragment != null) && (dtlFragment is IssueReviewDetailFragment))
                {
                    fragmentTransaction.Show(dtlFragment);
                    ((IssueReviewDetailFragment)dtlFragment).RefreshDisplayedRecord();
                }
                else
                {
                    var fragment = new IssueReviewDetailFragment { Arguments = new Bundle() };
                    fragment.Arguments.PutString("structName", _structName);

                    fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, prevFragName);

                    fragment.RefreshDisplayedRecord();

                }
            }
            /*

                            else if (prevFragName.StartsWith(Constants.ISSUE_NOTES_FRAGMENT_TAG_PREFIX) == true)
                            {
                                // go to the ticket detail 
                                var dtlFragment = (IssueReviewDetailFragment)FragmentManager.FindFragmentByTag(prevFragName);
                                if (dtlFragment != null)
                                {
                                    fragmentTransaction.Show(dtlFragment);
                                    dtlFragment.CreateLayout();
                                }
                                else
                                {
                                    Android.App.Fragment fragment = new IssueReviewDetailFragment { Arguments = new Bundle() };
                                    fragment.Arguments.PutString("structName", _structName);

                                    fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, prevFragName);
                                }
                            }
            */


            /*
                            else if (Constants.ISSUE_REVIEW_SUMMARY_FRAGMENT_TAG.Equals(prevFragName))
                            {
                                // go to the ticket detail 
                                var dtlFragment = (TicketIssueSummaryFragment)FragmentManager.FindFragmentByTag(Constants.ISSUE_REVIEW_SUMMARY_FRAGMENT_TAG);
                                if (dtlFragment != null)
                                {
                                    fragmentTransaction.Show(dtlFragment);
                                    dtlFragment.CreateLayout();
                                }
                                else
                                    fragmentTransaction.Replace(Resource.Id.frameLayout1, new TicketIssueSummaryFragment(), Constants.ISSUE_REVIEW_SUMMARY_FRAGMENT_TAG);
                            }
                            else
                            {
                                // go back to the last Issue Ticket panel 
                                var tktFragment = (CommonFragment)FragmentManager.FindFragmentByTag(structName);
                                if (tktFragment != null)
                                {
                                    fragmentTransaction.Show(tktFragment);
                                }
                                else
                                {
                                    Fragment fragment = new CommonFragment { Arguments = new Bundle() };
                                    fragment.Arguments.PutString("structName", structName);
                                    fragmentTransaction.Replace(Resource.Id.frameLayout1, fragment, structName);
                                }
                            }
                        }
                        */


            fragmentTransaction.Commit();
        }

    }  // end class NotesFragment


    public interface ITapLock
    {
        TapLockVars TapLockVars { get; set; }
    }

    public struct TapLockVars
    {
        public bool Locked;
    }
    public static class TapLockExtensions
    {

        private static DateTime _lastTappedTime = DateTime.Now;
        public static bool AcquireTapLock(this ITapLock obj)
        {
            // if locked is true, return false
            // if locked is false, set to true and return true
            try
            {
                var vars = obj.TapLockVars;
                return (!vars.Locked && (vars.Locked = true) && (obj.TapLockVars = vars).Locked) ||
                       _lastTappedTime.AddSeconds(3) < DateTime.Now;
            }
            finally
            {
                _lastTappedTime = DateTime.Now;
            }


        }

        public static void ReleaseTapLock(this ITapLock obj)
        {
            var vars = obj.TapLockVars;
            vars.Locked = false;
            obj.TapLockVars = vars;
        }

    }
}

