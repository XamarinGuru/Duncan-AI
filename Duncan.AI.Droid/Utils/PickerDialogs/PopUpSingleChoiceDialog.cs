using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Duncan.AI.Droid.Utils.HelperManagers;

namespace Duncan.AI.Droid.Utils.PickerDialogs
{
    class PopUpSingleChoiceDialog : DialogFragment
    {
        private static AlertDialog.Builder builder = null;


        IssueReviewDetailFragment fCallingFragment = null;
        NotesReviewSelectFragment fCallingFragmentB = null;
        List<string> fSelectionItems;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }
        
        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            if(builder != null)
                return builder.Create();


            return null;
        }


        //public void SetPopUpSelectionChoices(IssueReviewDetailFragment iCallingFragment, List<string> iSelectionItems)
        //{
        //    fCallingFragment = iCallingFragment;
        //    fSelectionItems = iSelectionItems;
        //}

        public PopUpSingleChoiceDialog(Context context, IssueReviewDetailFragment iCallingFragment, NotesReviewSelectFragment iCallingFragmentB, List<string> iSelectionItems)
        {
            fCallingFragment = iCallingFragment;
            fCallingFragmentB = iCallingFragmentB;
            fSelectionItems = iSelectionItems;

            builder = new AlertDialog.Builder(context);
            builder.SetIconAttribute(Android.Resource.Attribute.Action);
            //builder.SetTitle("");
            var layoutVert = new LinearLayout(context) { Orientation = Orientation.Vertical };


            for (int loButtonIdx = 0; loButtonIdx < fSelectionItems.Count; loButtonIdx++)
            {


                Button loButtonChoice1 = new Button(context);
                loButtonChoice1.Text = fSelectionItems[loButtonIdx];
                Helper.SetTypefaceForButton(loButtonChoice1, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);
                //loButtonLPR.SetTextColor(Resources.GetColor(Resource.Color.civicsmart_black));
                loButtonChoice1.SetBackgroundColor(Android.Graphics.Color.White);
                var loLayout = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                loLayout.AddRule(LayoutRules.AlignLeft);
                loButtonChoice1.LayoutParameters = loLayout;
                loButtonChoice1.Click += delegate
                {
                    try
                    {
                        ExecutePopUpSingleChoiceCallback(loButtonChoice1.Text);
                        this.DismissAllowingStateLoss();
                    }
                    catch (Exception exp)
                    {
                        //exp.PrintStackTrace();
                    }
                };


                layoutVert.AddView(loButtonChoice1);
            }



            layoutVert.SetBackgroundColor(Android.Graphics.Color.White);            
            builder.SetView(layoutVert);            
        }




        void ExecutePopUpSingleChoiceCallback(string iSelectedChoiceText )
        {
            // if we have a valid destination, we can redirect
            if ( fCallingFragment != null )
            {
                fCallingFragment.PopUpSingleChoiceCallback(iSelectedChoiceText);
            }
            else if (fCallingFragmentB != null)
            {
                fCallingFragmentB.PopUpSingleChoiceCallback(iSelectedChoiceText);
            }
        }


    }
}