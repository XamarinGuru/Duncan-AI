
using System;
using System.Collections.Generic;
using System.Data;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Duncan.AI.Droid.Utils.HelperManagers;
using Duncan.AI.Droid.Common;

namespace Duncan.AI.Droid
{
    public class CustomANPRMatchAdapter : BaseAdapter<ANPRMatchDTO>
    {
        ANPRConfirmResultFragment _parentSelectionDialog = null;


        List<ANPRMatchDTO> items;
        Activity context;
        string tableName;


        public CustomANPRMatchAdapter(Activity context, List<ANPRMatchDTO> iItems, ANPRConfirmResultFragment iParentSelectionDialog)
            : base()
        {
            this.context = context;
            this.items = iItems;
            this._parentSelectionDialog = iParentSelectionDialog;
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override ANPRMatchDTO this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];

            //find view to re-use or create new one
            View view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.CustomSearchMatchListViewCard, null);


            // set alternating colors to increase visibility
            //view.SetBackgroundColor(position%2 == 1 ? Android.Graphics.Color.AntiqueWhite : Android.Graphics.Color.Azure);            	        

            // first we will do all the common setup work, before we populate the content 


            // whole card view
            var locard_band_summary = view.FindViewById<LinearLayout>(Resource.Id.card_band_summary);


            // header layout view
            var locard_band = view.FindViewById<LinearLayout>(Resource.Id.card_band);


            // header layout view
            var locard_band_header = view.FindViewById<LinearLayout>(Resource.Id.card_band_header);

            // header band
            var locard_band_header_text1 = view.FindViewById<TextView>(Resource.Id.card_band_header_text1);
            if (locard_band_header_text1 != null)
            {
                // initialize our typeface and set the text
                Helper.SetTypefaceForTextView(locard_band_header_text1, FontManager.cnCardViewHeaderTextLargeTypeface, FontManager.cnCardViewHeaderTextLargeTypefaceSizeSp);
            }


            var locard_band_header_text2 = view.FindViewById<TextView>(Resource.Id.card_band_header_text2);
            if (locard_band_header_text2 != null)
            {
                // initialize our typeface and set the text
                Helper.SetTypefaceForTextView(locard_band_header_text2, FontManager.cnCardViewHeaderTextSmallestTypeface, FontManager.cnCardViewHeaderTextSmallestTypefaceSizeSp);
            }




            // clock band
            var locard_back_clock1_layout = view.FindViewById<RelativeLayout>(Resource.Id.card_band_clock1_layout);

            var locard_band_clock1_label = view.FindViewById<TextView>(Resource.Id.card_band_clock1_label);
            if (locard_band_clock1_label != null)
            {
                // initialize our typeface and set the text
                Helper.SetTypefaceForTextView(locard_band_clock1_label, FontManager.cnCardViewHeaderTextSmallTypeface, FontManager.cnCardViewHeaderTextSmallTypefaceSizeSp);
            }

            var locard_band_clock1 = view.FindViewById<TextView>(Resource.Id.card_band_clock1);
            if (locard_band_clock1 != null)
            {
                // initialize our typeface and set the text
                Helper.SetTypefaceForTextView(locard_band_clock1, FontManager.cnCardViewHeaderTextClockTypeface, FontManager.cnCardViewHeaderTextClockTypefaceSizeSp);
            }






            // 
            var locard_band_clock2_label = view.FindViewById<TextView>(Resource.Id.card_band_clock2_label);
            if (locard_band_clock2_label != null)
            {
                // initialize our typeface and set the text
                Helper.SetTypefaceForTextView(locard_band_clock2_label, FontManager.cnCardViewHeaderTextSmallTypeface, FontManager.cnCardViewHeaderTextSmallTypefaceSizeSp);
            }


            var locard_band_clock2 = view.FindViewById<TextView>(Resource.Id.card_band_clock2);
            if (locard_band_clock2 != null)
            {
                // initialize our typeface and set the text
                Helper.SetTypefaceForTextView(locard_band_clock2, FontManager.cnCardViewHeaderTextClockTypeface, FontManager.cnCardViewHeaderTextClockTypefaceSizeSp);
            }





            // summary band
            var locard_band_summary_layout = view.FindViewById<LinearLayout>(Resource.Id.card_band_summary);
            if (locard_band_summary_layout != null)
            {
                // hide it until we have something useful to put here
                locard_band_summary_layout.Visibility = ViewStates.Gone;
            }


            var locard_band_summary_text1 = view.FindViewById<TextView>(Resource.Id.card_band_summary_text1);
            if (locard_band_summary_text1 != null)
            {
                // initialize our typeface and set the text
                Helper.SetTypefaceForTextView(locard_band_summary_text1, FontManager.cnCardViewSummaryTextLargeTypeface, FontManager.cnCardViewSummaryTextLargeTypefaceSizeSp);
            }

            var locard_band_summary_text2 = view.FindViewById<TextView>(Resource.Id.card_band_summary_text2);
            if (locard_band_summary_text2 != null)
            {
                // initialize our typeface and set the text
                Helper.SetTypefaceForTextView(locard_band_summary_text2, FontManager.cnCardViewSummaryTextSmallTypeface, FontManager.cnCardViewSummaryTextSmallTypefaceSizeSp);
            }



            // counter
            var locard_band_counter_text = view.FindViewById<TextView>(Resource.Id.card_band_counter_text);
            if (locard_band_counter_text != null)
            {
                // initialize our typeface and set the text
                Helper.SetTypefaceForTextView(locard_band_counter_text, FontManager.cnCardViewCounterTextSmallTypeface, FontManager.cnCardViewCounterTextSmallTypefaceSizeSp);
            }


            // action bar - hookup the buttons
            Button btnCamera = view.FindViewById<Button>(Resource.Id.btnCardViewDisplayImage);
            Button btnEnforce = view.FindViewById<Button>(Resource.Id.btnCardViewExitToApp);





            ///////////
            ///
            ///  Update the content
            ///  
            //////////          


            #region _CardView_LPR_Result_

            // generic search results
            // TODO - look to the source match form for column info


            string loFormattedData = string.Empty;

            // no plate?
            bool loNoPlateFound = string.IsNullOrEmpty(item.sqlVehLicNoStr);


            // header layout view

            Color loCardStatusColor;
            Color loElapsedClockStatusColor;

            if (loNoPlateFound == true)
            {

                // maybe go yellow here?
                loCardStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_red);
                loElapsedClockStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_red);

                //locard_band.SetBackgroundResource(Resource.Drawable.CardViewBorder_Green);
                locard_band.SetBackgroundResource(Resource.Drawable.CardViewBorder_Gray);
            }
            else
            {

                if (item.confidenceAsDouble >= 90.0f)
                {
                    // 
                    loCardStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_green);
                    loElapsedClockStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_green);
                }
                else if (item.confidenceAsDouble >= 80.0f)
                {
                    loCardStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_yellow);
                    loElapsedClockStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_yellow);
                }
                else
                {
                    loCardStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_red);
                    loElapsedClockStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_red);
                }

                //locard_band.SetBackgroundResource(Resource.Drawable.CardViewBorder_Green);
                locard_band.SetBackgroundResource(Resource.Drawable.CardViewBorder_Gray);
            }


            // whole card view
            locard_band_summary.SetBackgroundColor(loCardStatusColor);

            // header layout view
            locard_band_header.SetBackgroundColor(loCardStatusColor);

            // header band
            if (locard_band_header_text1 != null)
            {


                loFormattedData = "";

                if (loNoPlateFound == true)
                {
                    loFormattedData = "No Plate Found";
                }
                else
                {
                    loFormattedData = "Processing Result";
                }

                locard_band_header_text1.Text = loFormattedData;
            }


            if (locard_band_header_text2 != null)
            {
                //((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhLocationText, ref loFormattedData, item.rawRow, position);
                // locard_band_header_text2.Text = loFormattedData;

                locard_band_header_text2.Visibility = ViewStates.Gone;
            }




            // clock band

            if (locard_band_clock1_label != null)
            {
                // TODO: this will vary by result type
                //((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhMarkXofYPhrase, ref loFormattedData, item.rawRow, position);
                loFormattedData = "Confidence";

                locard_band_clock1_label.Text = loFormattedData;
            }

            if (locard_band_clock1 != null)
            {
                // original - issue time
                //((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhMarkOriginalTime, ref loFormattedData, item.rawRow, position);

                loFormattedData = item.sqlConfidenceStr;
                locard_band_clock1.Text = loFormattedData;

                locard_band_clock1.SetTextColor(DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.civicsmart_black));
            }




            // 
            if (locard_band_clock2_label != null)
            {
                // TODO: get from MatchRec DBListGrid
                loFormattedData = "Plate";

                locard_band_clock2_label.Text = loFormattedData;
            }

            if (locard_band_clock2 != null)
            {
                loFormattedData = item.sqlVehLicNoStr;

                locard_band_clock2.Text = loFormattedData;
                //locard_band_clock2.SetTextColor(loElapsedClockStatusColor);
                locard_band_clock2.SetTextColor(DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.civicsmart_black));
            }





            // summary band






            if (locard_band_summary_text1 != null)
            {

                loFormattedData = "";
                locard_band_summary_text1.Text = loFormattedData;
                locard_band_summary_text1.Visibility = ViewStates.Gone;
            }

            if (locard_band_summary_text2 != null)
            {
                locard_band_summary_text2.Visibility = ViewStates.Gone;
            }




            // counter
            if (locard_band_counter_text != null)
            {
                //((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhMarkXofYPhrase, ref loFormattedData, item.rawRow, position);
                //locard_band_counter_text.Text = loFormattedData;

                locard_band_counter_text.Visibility = ViewStates.Gone;

            }



            // action bar - hookup the buttons
            if (btnCamera != null)
            {
                // save the list position in the tag for reference when we are clicked
                btnCamera.Tag = position;

                // but LPR no photos
                btnCamera.Visibility = ViewStates.Gone;

            }

            if (btnEnforce != null)
            {
                // save the list position in the tag for reference when we are clicked
                btnEnforce.Tag = position;
                // must remove before re-adding, else we'll get cascading execution of clicks from each view paint
                btnEnforce.Click -= btnEnforcementActionClick;
                btnEnforce.Click += btnEnforcementActionClick;


                // can't select this one
                if (loNoPlateFound == true)
                {
                    btnEnforce.Visibility = ViewStates.Gone;
                }


            }

            #endregion



            return view;
        }


        async void btnEnforcementActionClick(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button loButton = sender as Button;

                int loListpos = (int)loButton.Tag;

                if ((loListpos > -1) && (loListpos < items.Count))
                {
                    var loSelectedItem = items[loListpos];

                    string loSelectedPlate = loSelectedItem.sqlVehLicNoStr;

                    if (_parentSelectionDialog != null)
                    {
                        _parentSelectionDialog.SetUserSelectionAndReturnToCallingFragnment(loSelectedPlate);
                    }

                }
            }
        }



        private string GetColumnValueFromDataRow(DataRow iDataRow, string iColumnName)
        {
            string loResult = string.Empty;

            if (iDataRow != null)
            {
                foreach (DataColumn oneColumn in iDataRow.Table.Columns)
                {
                    if (iColumnName.Equals(oneColumn.ColumnName) == true)
                    {
                        try
                        {
                            loResult = iDataRow[oneColumn].ToString();
                            break;
                        }
                        catch (Exception exp)
                        {
                            loResult = string.Empty;
                            Console.WriteLine("Error in SearchMatchAdapter: {0}", exp.Source);
                        }
                    }
                }
            }

            return loResult;
        }


        private bool GetColumnValueFromDataRowIfPresent(DataRow iDataRow, string iColumnName, ref string ioResultStr )
        {
            string loResult = string.Empty;
            bool loColumnIsPresent = false;

            if (iDataRow != null)
            {
                foreach (DataColumn oneColumn in iDataRow.Table.Columns)
                {
                    if (iColumnName.Equals(oneColumn.ColumnName) == true)
                    {
                        try
                        {
                            loColumnIsPresent = true;
                            loResult = iDataRow[oneColumn].ToString();
                            break;
                        }
                        catch (Exception exp)
                        {
                            loResult = string.Empty;
                            loColumnIsPresent = false;
                            Console.WriteLine("Error in SearchMatchAdapter: {0}", exp.Source);
                        }
                    }
                }
            }

            loResult = ioResultStr;
            return loColumnIsPresent;
        }


    }
}


