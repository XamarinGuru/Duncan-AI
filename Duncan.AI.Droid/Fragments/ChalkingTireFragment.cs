using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Preferences;

namespace Duncan.AI.Droid
{
	public class ChalkingTireFragment : Fragment
	{
		RadioGroup radioGroupFront;
		RadioGroup radioGroupRear;
		RelativeLayout r1;
		RelativeLayout r2;
		View view;
		Button submitBtn;
		Spinner spinner;
		int position;
		XMLConfig.IssStruct ChalkingStruct;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
            // AJW - always call the superclass
            base.OnCreateView(inflater, container, savedInstanceState);


			view = inflater.Inflate(Resource.Layout.ChalkingTire, null);
			r1 = view.FindViewById<RelativeLayout>(Resource.Id.r1);
			r2 = view.FindViewById<RelativeLayout>(Resource.Id.r2);
			radioGroupFront = view.FindViewById<RadioGroup>(Resource.Id.radioGrpFront);
			radioGroupRear = view.FindViewById<RadioGroup>(Resource.Id.radioGrpRear);
			submitBtn = view.FindViewById<Button>(Resource.Id.submitBtn);
			submitBtn.Click += btnSubmitClick;
			spinner = view.FindViewById<Spinner>(Resource.Id.spinner1);
			string[] response = new string[]{ "Front", "Rear"};
			spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_ItemSelected);
			ArrayAdapter<String> adr = new ArrayAdapter<String>(this.Activity, Android.Resource.Layout.SimpleSpinnerItem, response);
			spinner.Adapter = adr;

			Button btnBack = view.FindViewById<Button>(Resource.Id.btnBack);
			btnBack.Click += btnBackClick;


            // AJW - review - what is this about - unfinished work?
            //ChalkingStruct = null; // DroidContext.XmlCfg.GetStruct(Constants.STRUCT_TYPE_CHALKING, Constants.STRUCT_NAME_CHALKING);


            ChalkingStruct = DroidContext.XmlCfg.GetStruct(Constants.STRUCT_TYPE_CHALKING, Constants.STRUCT_NAME_CHALKING);

			setTireStem ();

			return view;
		}

		public void setTireStem()
		{
			for (int i = 0; i < ChalkingStruct.Panels.Count; i++) {
				foreach (XMLConfig.PanelField panelField in ChalkingStruct.Panels[i].PanelFields) {
					if (panelField.Name == Constants.TIRESTEMSFRONTTIME_COLUMN) {
						if (panelField.Value != null) {
							if (panelField.Value.Equals ("12")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.Twele);
								radioButton.Checked = true;
							} else if (panelField.Value.Equals ("11")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.eleven);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("10")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.ten);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("9")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.nine);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("8")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.eight);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("7")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.seven);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("6")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.six);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("5")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.five);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("4")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.four);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("3")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.three);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("2")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.two);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("1")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.one);
								radioButton.Checked = true;
							}
						} else {
							radioGroupFront.ClearCheck();
						}
					}else if (panelField.Name == Constants.TIRESTEMSREARTIME_COLUMN) {
						if (panelField.Value != null) {
							if (panelField.Value.Equals ("12")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.TweleRear);
								radioButton.Checked = true;
							} else if (panelField.Value.Equals ("11")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.elevenRear);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("10")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.tenRear);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("9")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.nineRear);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("8")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.eightRear);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("7")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.sevenRear);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("6")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.sixRear);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("5")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.fiveRear);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("4")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.fourRear);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("3")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.threeRear);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("2")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.twoRear);
								radioButton.Checked = true;
							}else if (panelField.Value.Equals ("1")) {
								RadioButton radioButton = (RadioButton) view.FindViewById(Resource.Id.oneRear);
								radioButton.Checked = true;
							}
						} else {
							radioGroupRear.ClearCheck();
						}
					}
				}
			}
		}

		private void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;
			position = e.Position;
			string ss = spinner.GetItemAtPosition (e.Position).ToString();

			if("Front".Equals(spinner.GetItemAtPosition (e.Position).ToString())){
				r1.Visibility = ViewStates.Visible;
				r2.Visibility = ViewStates.Gone;
			}else if("Rear".Equals(spinner.GetItemAtPosition (e.Position).ToString())){
				r2.Visibility = ViewStates.Visible;
				r1.Visibility = ViewStates.Gone;
			}
		}

		private void btnSubmitClick(object sender, EventArgs e)
		{
			int frontId = radioGroupFront.CheckedRadioButtonId;
			int rearId = radioGroupRear.CheckedRadioButtonId;

			int frontTime = 0;
			int rearTime = 0;

			if (frontId > 0 && "Front".Equals(spinner.GetItemAtPosition (position).ToString())) {
				RadioButton radioButtonFrt = (RadioButton) view.FindViewById(frontId);
				switch(radioButtonFrt.Id){
				case Resource.Id.Twele:
					frontTime = 12;
					break;

				case Resource.Id.one:
					frontTime = 1;
					break;

				case Resource.Id.two:
					frontTime = 2;
					break;

				case Resource.Id.three:
					frontTime = 3;
					break;

				case Resource.Id.four:
					frontTime = 4;
					break;

				case Resource.Id.five:
					frontTime = 5;
					break;

				case Resource.Id.six:
					frontTime = 6;
					break;

				case Resource.Id.seven:
					frontTime = 7;
					break;

				case Resource.Id.eight:
					frontTime = 8;
					break;

				case Resource.Id.nine:
					frontTime = 9;
					break;

				case Resource.Id.ten:
					frontTime = 10;
					break;

				case Resource.Id.eleven:
					frontTime = 11;
					break;

				}

				Toast.MakeText(this.Activity, "Front Stem Submitted", ToastLength.Long).Show();
			}

			if (rearId > 0 && "Rear".Equals(spinner.GetItemAtPosition (position).ToString())) {
				RadioButton radioButtonRear = (RadioButton) view.FindViewById(rearId);
				switch(radioButtonRear.Id){
				case Resource.Id.TweleRear:
					rearTime = 12;
					break;

				case Resource.Id.oneRear:
					rearTime = 1;
					break;

				case Resource.Id.twoRear:
					rearTime = 2;
					break;

				case Resource.Id.threeRear:
					rearTime = 3;
					break;

				case Resource.Id.fourRear:
					rearTime = 4;
					break;

				case Resource.Id.fiveRear:
					rearTime = 5;
					break;

				case Resource.Id.sixRear:
					rearTime = 6;
					break;

				case Resource.Id.sevenRear:
					rearTime = 7;
					break;

				case Resource.Id.eightRear:
					rearTime = 8;
					break;

				case Resource.Id.nineRear:
					rearTime = 9;
					break;

				case Resource.Id.tenRear:
					rearTime = 10;
					break;

				case Resource.Id.elevenRear:
					rearTime = 11;
					break;
				}
				Toast.MakeText(this.Activity, "Rear Stem Submitted", ToastLength.Long).Show();
			}



			for (int i = 0; i < ChalkingStruct.Panels.Count; i++) {
				foreach (XMLConfig.PanelField panelField in ChalkingStruct.Panels[i].PanelFields) {
					if (panelField.Name == Constants.TIRESTEMSFRONTTIME_COLUMN
					    || panelField.Name == Constants.TIRESTEMSREARTIME_COLUMN) {
						if(panelField.Name == Constants.TIRESTEMSFRONTTIME_COLUMN && "Front".Equals(spinner.GetItemAtPosition (position).ToString()))
						{
							if (frontTime > 0) {
								panelField.Value = frontTime.ToString() + "AM";
							}
						}

						if(panelField.Name == Constants.TIRESTEMSREARTIME_COLUMN && "Rear".Equals(spinner.GetItemAtPosition (position).ToString()))
						{
							if (rearTime > 0) {
								panelField.Value = rearTime.ToString() + "AM";
							}
						}
					}
				}
			}
		}

		void btnBackClick(object sender, EventArgs e)
		{
			// go back to where we came from
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity);
            string structName = prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);

			FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();

			Fragment chalkTireFragment = FragmentManager.FindFragmentByTag(Constants.CHALK_TIRE_FRAGMENT_TAG);
			if (chalkTireFragment != null)
			{
				fragmentTransaction.Hide(chalkTireFragment);
			}

            CommonFragment chalkFragment = (CommonFragment)FragmentManager.FindFragmentByTag(structName);  // AJW - this should NEVER fail, but sometimes it does... why?
			if (chalkFragment != null)
			{
				fragmentTransaction.Show(chalkFragment);
			}
			else
			{
                //fragmentTransaction.Replace(Resource.Id.frameLayout1, new CommonFragment(), structName);

                Fragment fragment = new CommonFragment { Arguments = new Bundle() };
                fragment.Arguments.PutString("structName", structName);
                fragmentTransaction.Replace(Resource.Id.frameLayout1, fragment, structName);

			}
			fragmentTransaction.Commit();
		}
	}
}

