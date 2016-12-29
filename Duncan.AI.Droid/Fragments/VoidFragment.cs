using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Reino.ClientConfig;
using System.Collections.Generic;

namespace Duncan.AI.Droid
{
    public class VoidFragment : Fragment
    {
        string _tagName;
        string _parent_structName;

        XMLConfig.IssStruct _parent_struct;

        Button _submitBtn;
        Spinner _spinner;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // AJW - always call the superclass
            base.OnCreateView(inflater, container, savedInstanceState);

            // AJW - this was the only fragment that had superclass call here
            base.OnCreate(savedInstanceState);


            _parent_structName = Arguments.GetString("structName", null);
            _parent_struct = DroidContext.XmlCfg.GetStructByName(_parent_structName);

            _tagName = Helper.BuildIssueVoidFragmentTag(_parent_structName);


            var view = inflater.Inflate(Resource.Layout.Void_fragment, container, false);

            _submitBtn = view.FindViewById<Button>(Resource.Id.button1);
            _submitBtn.Click += BtnSubmitClick;
            _spinner = view.FindViewById<Spinner>(Resource.Id.spinner1);


            string loDefaultVoidReasonTableName = Constants.PARKVOIDREASON;
            string loDefaultVoidReasonColumName = Constants.PARKVOIDREASON;

            // extract the void reasons from the parent struct if we can
            if (_parent_struct._TIssStruct is TCiteStruct)
            {
                TCiteStruct loParentStruct = (TCiteStruct)_parent_struct._TIssStruct;

                //If the Void Reason List is blank try populate it here
                if (string.IsNullOrEmpty(loParentStruct.VoidReasonList) == false)
                {
                    loDefaultVoidReasonTableName = loParentStruct.VoidReasonList;
                }
            }

            var listSupport = new ListSupport();
            string[] response = listSupport.GetListDataByTableColumnName(loDefaultVoidReasonTableName, loDefaultVoidReasonColumName);

            // debug - supplement list for demo
            if ((response == null) || (response.GetLength(0) == 0))
            {
                string[] loSampleResponse = new string[2] { "DRIVER PRODUCED VALID PERMIT", "OPERATOR ERROR" };
                response = loSampleResponse;
            }

            var adr = new ArrayAdapter<String>(Activity, Android.Resource.Layout.SimpleSpinnerItem, response);
            _spinner.Adapter = adr;

            return view;
        }

        //Submit Button
        private void BtnSubmitClick(object sender, EventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            ISharedPreferencesEditor editor = prefs.Edit();
            string sequenceId = prefs.GetString(Constants.VOID_AITICKETID, null);
            string masterKey = prefs.GetString(Constants.ID_COLUMN, null);
            string structName = prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);
            string officerId = prefs.GetString(Constants.OFFICER_ID, null);
            string officerName = prefs.GetString(Constants.OFFICER_NAME, null);
            editor.Remove(Constants.VOID_AITICKETID);
            editor.Apply();

            string voidStructName = null;

            var parkingSequenceADO = new ParkingSequenceADO();


            // get the void struct reference
            if (_parent_struct._TIssStruct is TCiteStruct)
            {
                if (((TCiteStruct)_parent_struct._TIssStruct).VoidStruct != null)
                {
                    voidStructName = ((TCiteStruct)_parent_struct._TIssStruct).VoidStruct.Name;
                }
            }

            //List<XMLConfig.IssStruct> structs = DroidContext.XmlCfg.IssStructs;
            //foreach (var structI in structs)
            //{
            //    if (structI.ParentStruct != null)
            //    {
            //        if (structI.ParentStruct.Equals(structName) && Constants.STRUCT_TYPE_VOID.Equals(structI.Type))
            //        {
            //            voidStructName = structI.Name;
            //        }
            //    }
            //}





            // AJW - defend against mis-configured systems
            string loVoidReason = string.Empty;
            if (_spinner.SelectedItem != null)
            {
                loVoidReason = _spinner.SelectedItem.ToString();
            }


            parkingSequenceADO.InsertRowParkingVoid(masterKey, sequenceId, officerId, officerName, loVoidReason, Activity, voidStructName);
            Toast.MakeText(Activity, "Ticket Voided", ToastLength.Long).Show();



            // go back to where we came from
            //ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            String prevFragName = prefs.GetString(Constants.PREVIOUS_FRAGMENT, null);
            //string structName = prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);

            // 
            prevFragName = DroidContext.MyFragManager.PopInternalBackstack();


            // hide ourselves
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();


            Fragment voidFragment = FragmentManager.FindFragmentByTag(_tagName);
            if (voidFragment != null)
            {
                fragmentTransaction.Hide(voidFragment);
            }


            if (string.IsNullOrEmpty(prevFragName) == true)
            {
                // not sure where to go.... this is the data type we need
                prevFragName = Helper.BuildIssueReviewFragmentTag(_parent_structName);
            }

            // back to the ticket detail
            var dtlFragment = FragmentManager.FindFragmentByTag(prevFragName);
            if ((dtlFragment != null) && (dtlFragment is IssueReviewDetailFragment))
            {
                fragmentTransaction.Show(dtlFragment);
                ((IssueReviewDetailFragment)dtlFragment).RefreshDisplayedRecord();
            }
            else if (dtlFragment != null) 
            {
                // don't know what kind of fragment it is
                fragmentTransaction.Show(dtlFragment);
            }
            else
            {
                var fragment = new IssueReviewDetailFragment { Arguments = new Bundle() };
                fragment.Arguments.PutString("structName", _parent_structName);

                fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, Helper.BuildIssueReviewFragmentTag(_parent_structName));

                fragment.RefreshDisplayedRecord();

            }


            fragmentTransaction.Commit();

        }
    }
}
