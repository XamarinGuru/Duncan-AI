
using System;
using System.Collections.Generic;
using System.Data;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Reino.ClientConfig;
using Duncan.AI.Droid.Utils.HelperManagers;
using Duncan.AI.Droid.Common;

namespace Duncan.AI.Droid
{
    public class CustomSearchMatchAdapter : BaseAdapter<SearchMatchDTO>
    {
        SearchParameterPacket _SearchResult;
        List<SearchMatchDTO> items;
        Activity _context;
        string tableName;


        public CustomSearchMatchAdapter(Activity context, SearchParameterPacket iSearchResult)
            : base()
        {
            this._context = context;
            this._SearchResult = iSearchResult;

            // duplicate, TODO remove extra ref
            this.items = iSearchResult.fSearchResultDTOList;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override SearchMatchDTO this[int position]
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
            View view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.CustomSearchMatchListViewCard, null);


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




            ////////
            ///
            //    Get some display formatting info from the layout
            //  
            //
            ///////




            ///////////
            ///
            ///  Update the content
            ///  
            //////////          

            if (item.fSearchStructLogic == null)
            {
                // bad code
            }
            else if (item.fSearchStructLogic is MarkModeStructLogicAndroid)
            {
                // List<TGridColumnInfo> loMatchGridColumns = ExtractDBListBoxDisplayFormatInfo( item );


                #region _CardView_MarkMode

                string loFormattedData = string.Empty;

                // prepare the formatting objects
                //((MarkModeStructLogicAndroid)item.fSearchStructLogic).InitSourceFormattingInfo(item.rawRow);
                item.fSearchStructLogic.InitSourceFormattingInfo(item.rawRow);


                // header layout view

                // how much time for this record?
                int loElapsedMinutes = ((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetElapsedTimeAsMinutes();

                // evaluate backwards
                Color loCardStatusColor;
                Color loElapsedClockStatusColor;
                if (loElapsedMinutes > 59)
                {
                    loCardStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_red);
                    loElapsedClockStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_red);

                    //locard_band.SetBackgroundResource(Resource.Drawable.CardViewBorder_Red);
                    locard_band.SetBackgroundResource(Resource.Drawable.CardViewBorder_Gray);
                }
                else if (loElapsedMinutes > 29)
                {
                    loCardStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_yellow);
                    loElapsedClockStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.civicsmart_black);

                    //locard_band.SetBackgroundResource(Resource.Drawable.CardViewBorder_Yellow);
                    locard_band.SetBackgroundResource(Resource.Drawable.CardViewBorder_Gray);
                }
                else
                {
                    // leave it green/black
                    loCardStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_green);
                    loElapsedClockStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.civicsmart_black);

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
                    loFormattedData =

                        //"Plate " +  // TODO flex with result type

                        GetColumnValueFromDataRow(item.rawRow, AutoISSUE.DBConstants.sqlVehLicStateStr) +
                        " " +
                        GetColumnValueFromDataRow(item.rawRow, AutoISSUE.DBConstants.sqlVehLicNoStr);

                    locard_band_header_text1.Text = loFormattedData;
                }


                if (locard_band_header_text2 != null)
                {
                    ((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhLocationText, ref loFormattedData, item.rawRow, position);
                    locard_band_header_text2.Text = loFormattedData;

                    //locard_band_header_text2.Visibility = ViewStates.Gone;
                }




                // clock band

                if (locard_band_clock1_label != null)
                {
                    // TODO: this will vary by result type
                    ((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhMarkXofYPhrase, ref loFormattedData, item.rawRow, position);

                    locard_band_clock1_label.Text = loFormattedData + " at";
                }

                if (locard_band_clock1 != null)
                {
                    // original - issue time
                    ((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhMarkOriginalTime, ref loFormattedData, item.rawRow, position);

                    locard_band_clock1.Text = loFormattedData;

                    locard_band_clock1.SetTextColor(DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.civicsmart_black));
                }






                // 
                if (locard_band_clock2_label != null)
                {
                    // TODO: this will vary by result type
                    loFormattedData = "Elapsed since";

                    locard_band_clock2_label.Text = loFormattedData;
                }

                if (locard_band_clock2 != null)
                {
                    ((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhMarkElapsedTime, ref loFormattedData, item.rawRow, position);
                    locard_band_clock2.Text = loFormattedData;

                    locard_band_clock2.SetTextColor(loElapsedClockStatusColor);

                    // TODO - hook the clock for realtime elapsed time updates... 
                }





                // summary band




                // TODO - put HOT CODE down here

                if (locard_band_summary_text1 != null)
                {
                    //((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhMarkedAtPhrase, ref loFormattedData, item.rawRow, position);

                    ((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhLocationText, ref loFormattedData, item.rawRow, position);

                    locard_band_summary_text1.Text = loFormattedData;
                }

                if (locard_band_summary_text2 != null)
                {
                    // ((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhMarkedAtPhrase, ref loFormattedData, item.rawRow, position);
                    loFormattedData = "";
                    locard_band_summary_text2.Text = loFormattedData;

                    locard_band_summary_text2.Visibility = ViewStates.Gone;
                }



                // counter
                if (locard_band_counter_text != null)
                {
                    ((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhMarkXofYPhrase, ref loFormattedData, item.rawRow, position);
                    locard_band_counter_text.Text = loFormattedData;

                    locard_band_counter_text.Visibility = ViewStates.Gone;

                }



                // action bar - hookup the buttons
                if (btnCamera != null)
                {
                    // save the list position in the tag for reference when we are clicked
                    btnCamera.Tag = position;

                    // but mark mode photos not yet...
                    //btnCamera.Visibility = ViewStates.Gone;

                }

                if (btnEnforce != null)
                {
                    // default to not visible
                    ViewStates lobtnEnforceVisibility = ViewStates.Gone;

                    if (_SearchResult.fSearchStruct.StructLogicObj != null)
                    {
                        if (_SearchResult.fSearchStruct.StructLogicObj is SearchStructLogicAndroid)
                        {
                            if (((SearchStructLogicAndroid)_SearchResult.fSearchStruct.StructLogicObj).HasDefinedSearhResultAction() == true)
                            {
                                // save the list position in the tag for reference when we are clicked
                                btnEnforce.Tag = position;
                                // must remove before re-adding, else we'll get cascading execution of clicks from each view paint
                                btnEnforce.Click -= btnEnforcementActionClick;
                                btnEnforce.Click += btnEnforcementActionClick;

                                // this wil be seen
                                lobtnEnforceVisibility = ViewStates.Visible;
                            }
                        }
                    }

                    // set the final outcome
                    btnEnforce.Visibility = lobtnEnforceVisibility;
                }


                #endregion




            }
            else //if (item.fSearchStructLogic is SearchStructLogicAndroid)
            {


                #region _CardView_GenericSearch


                // generic search results
                // TODO - look to the source match form for column info


                string loFormattedData = string.Empty;

                // prepare the formatting objects
                //((SearchStructLogicAndroid)item.fSearchStructLogic).InitSourceFormattingInfo(item.rawRow);
                item.fSearchStructLogic.InitSourceFormattingInfo(item.rawRow);








                // some kludgy formatting - should all come from tabldef
                bool loIsPermitXref = false;
                bool loIsVINPresent = false;

                string loPermitNoCheckStr = string.Empty;
                if (GetColumnValueFromDataRowIfPresent(item.rawRow, AutoISSUE.DBConstants.sqlVehPermitNumberStr, ref loPermitNoCheckStr) == false)
                {
                    if (GetColumnValueFromDataRowIfPresent(item.rawRow, "PERMIT", ref loPermitNoCheckStr) == false)
                    {
                    }
                }

                if (string.IsNullOrEmpty(loPermitNoCheckStr) == false)
                {
                    loIsPermitXref = true;
                }



                string loVINCheckStr = string.Empty;
                if (GetColumnValueFromDataRowIfPresent(item.rawRow, AutoISSUE.DBConstants.sqlVehVINStr, ref loVINCheckStr) == true)
                {
                    loIsVINPresent = true;
                }




                // header layout view

                Color loCardStatusColor;
                Color loElapsedClockStatusColor;

                if (loIsPermitXref == true)
                {
                    // cross ref make green
                    loCardStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_green);
                    loElapsedClockStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_green);

                    locard_band.SetBackgroundResource(Resource.Drawable.CardViewBorder_Green);
                }
                else
                {
                    // hot search hits default to red
                    // TODO - make them color according to hot code
                    loCardStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_red);
                    loElapsedClockStatusColor = DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.cardview_red);

                    locard_band.SetBackgroundResource(Resource.Drawable.CardViewBorder_Red);
                }


                // whole card view
                locard_band_summary.SetBackgroundColor(loCardStatusColor);

                // header layout view
                locard_band_header.SetBackgroundColor(loCardStatusColor);

                // header band
                if (locard_band_header_text1 != null)
                {

                    if (loIsVINPresent == true)
                    {
                        loFormattedData =

                            "VIN: " +  // TODO flex with result type

                            loVINCheckStr;
                    }
                    else
                    {
                        loFormattedData =

                            //"Plate " +  // TODO flex with result type

                            GetColumnValueFromDataRow(item.rawRow, AutoISSUE.DBConstants.sqlVehLicStateStr) +
                            " " +
                            GetColumnValueFromDataRow(item.rawRow, AutoISSUE.DBConstants.sqlVehLicNoStr);
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

                if (locard_back_clock1_layout != null)
                {
                    // get rid of any pre-packed stuff
                    locard_back_clock1_layout.RemoveAllViews();

                    // construct a dynamic views based on the dbcolumn grid 
                    MakeDBGridRowView(locard_back_clock1_layout, item, position);
                }


#if _old_kludgy_direct_
                if (locard_band_clock1_label != null)
                {
                    // TODO: this will vary by result type
                    //((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhMarkXofYPhrase, ref loFormattedData, item.rawRow, position);
                    loFormattedData = "Code";

                    locard_band_clock1_label.Text = loFormattedData;
                }

                if (locard_band_clock1 != null)
                {
                    // original - issue time
                    //((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhMarkOriginalTime, ref loFormattedData, item.rawRow, position);

                    loFormattedData = GetColumnValueFromDataRow(item.rawRow, AutoISSUE.DBConstants.sqlHotCodeStr);
                    locard_band_clock1.Text = loFormattedData;


                    locard_band_clock1.SetTextColor(DroidContext.ApplicationContext.Resources.GetColor(Resource.Color.civicsmart_black));
                }



                // 
                if (locard_band_clock2_label != null)
                {
                    // TODO: this will vary by result type
                    //loFormattedData = "Elapsed since";
                    loFormattedData = "";

                    locard_band_clock2_label.Text = loFormattedData;
                }

                if (locard_band_clock2 != null)
                {
                    loFormattedData = "";
                    //((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(MarkModeStructLogicAndroid.TMarkModeHitData.mmhMarkElapsedTime, ref loFormattedData, item.rawRow, position);
                    locard_band_clock2.Text = loFormattedData;

                    locard_band_clock2.SetTextColor(loElapsedClockStatusColor);

                    // TODO - hook the clock for realtime elapsed time updates... 
                }
#endif




                // summary band






                if (loIsPermitXref == true)
                {

                    if (locard_band_summary_text1 != null)
                    {

                        loFormattedData = "";
                        loFormattedData = "Permit No: " + loPermitNoCheckStr;
                        locard_band_summary_text1.Text = loFormattedData;
                    }

                    if (locard_band_summary_text2 != null)
                    {
                        locard_band_summary_text2.Visibility = ViewStates.Gone;
                    }

                }
                else
                {
                    if (locard_band_summary_text2 != null)
                    {
                        locard_band_summary_text2.Visibility = ViewStates.Gone;
                    }

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

                    // but hotsheet no photos not yet...
                    btnCamera.Visibility = ViewStates.Gone;

                }

                if (btnEnforce != null)
                {
                    // default to not visible
                    ViewStates lobtnEnforceVisibility = ViewStates.Gone;

                    if (_SearchResult.fSearchStruct.StructLogicObj != null)
                    {
                        if (_SearchResult.fSearchStruct.StructLogicObj is SearchStructLogicAndroid)
                        {
                            if (((SearchStructLogicAndroid)_SearchResult.fSearchStruct.StructLogicObj).HasDefinedSearhResultAction() == true)
                            {
                                // save the list position in the tag for reference when we are clicked
                                btnEnforce.Tag = position;
                                // must remove before re-adding, else we'll get cascading execution of clicks from each view paint
                                btnEnforce.Click -= btnEnforcementActionClick;
                                btnEnforce.Click += btnEnforcementActionClick;

                                // this wil be seen
                                lobtnEnforceVisibility = ViewStates.Visible;
                            }
                        }
                    }

                    // set the final outcome
                    btnEnforce.Visibility = lobtnEnforceVisibility;
                }

                #endregion


            }
            //else
            //{
            //    // new type needs work
            //}


            // put search match type

            // icon to represent hit type: valid permit, previous mark, stolen, boot. tow. define hot codes to icons defs in registy

            // markmode: when previous marks found, time elapsed graduated green to yellow to red - OK, expiring in 5 mins, in violation

            return view;
        }



        async void btnEnforcementActionClick(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button loButton = sender as Button;

                int loListpos = (int)loButton.Tag;

                _SearchResult.SearchResultSelectedRow = _SearchResult.fSearchResultDTOList[loListpos].rawRow;
                _SearchResult.SearchResultSelectedRowIndex = loListpos;

                // dismiss the parent dialog
                if (_SearchResult._parentSelectionDialog != null)
                {
                    _SearchResult._parentSelectionDialog.Dismiss();
                }




                if (_SearchResult.fSearchStruct.StructLogicObj != null)
                {
                    if (_SearchResult.fSearchStruct.StructLogicObj is SearchStructLogicAndroid)
                    {
                        // if they didn't select one, just return the first
                        if (_SearchResult.SearchResultSelectedRow == null)
                        {
                            _SearchResult.SearchResultSelectedRow = _SearchResult.fSearchResultDTOList[0].rawRow;
                        }

                        ((SearchStructLogicAndroid)(_SearchResult.fSearchStruct.StructLogicObj)).HandleSearchResult_DialogCallback(_SearchResult);
                    }
                }
            }
        }





#if _old_view

	    public override View GetView(int position, View convertView, ViewGroup parent)
	    {
	        var item = items[position];

	        //find view to re-use or create new one
	        View view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.SearchMatchListview_layout, null);


            // set alternating colors to increase visibility
	        view.SetBackgroundColor(position%2 == 1 ? Android.Graphics.Color.AntiqueWhite : Android.Graphics.Color.Azure);            	        

            // set the icon to indicate status
            //if (Constants.STATUS_READY.Equals(item.parkingStatus))
            //    view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_ready);
            //else if (Constants.STATUS_REISSUE.Equals(item.parkingStatus))
            //    view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_reissue);
            //else if (Constants.STATUS_ISSUED.Equals(item.parkingStatus))
            //    view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_issued);
            //else if (Constants.STATUS_VOIDED.Equals(item.parkingStatus))
            //    view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_void);

            switch (item.parkingStatus)
            {
                case Constants.STATUS_READY:
                    {
                        //view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_ready);
                        view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_ready_rev2);
                        break;
                    }
                case Constants.STATUS_REISSUE:
                    {
                        view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_reissue);
                        break;
                    }
                case Constants.STATUS_ISSUED:
                    {
                        //view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_issued);
                        view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_issued_rev2);
                        break;
                    }
                case Constants.STATUS_VOIDED:
                    {
                        //view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_void);
                        view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_voided_rev2);
                        break;
                    }
                default:
                    {
                        // TODO unknown status?
                        break;
                    }
            }




            // AJW - this needs to use the field prompt names instead of fixed text
            //view.FindViewById<TextView>(Resource.Id.txt1).Text = "Reg No: " + item.licNo;
            //view.FindViewById<TextView>(Resource.Id.txt2).Text = "Iss No: " + item.seqId + " Street Off: " + item.locStreet;

            //view.FindViewById<TextView>(Resource.Id.txt1).Text = item.VEHICLE_DISPLAY;
            //view.FindViewById<TextView>(Resource.Id.txt2).Text = item.ISSUENO_DISPLAY + " " + item.sqlIssueDateStr + " " + item.sqlIssueTimeStr;

            //view.FindViewById<TextView>(Resource.Id.txt1).Text = GetColumnValueFromDataRow(item.rawRow, AutoISSUE.DBConstants.sqlVehLicNoStr);
            //view.FindViewById<TextView>(Resource.Id.txt2).Text = GetColumnValueFromDataRow(item.rawRow, AutoISSUE.DBConstants.sqlVehLicStateStr);

            if (
                (item.fSearchStructLogic != null) &&
                (item.fSearchStructLogic is MarkModeStructLogicAndroid)
                )
            {
                string loFormattedData = string.Empty;
                ((MarkModeStructLogicAndroid)item.fSearchStructLogic).InitSourceFormattingInfo(item.rawRow);

                view.FindViewById<TextView>(Resource.Id.txt1).Text = 
                    GetColumnValueFromDataRow(item.rawRow, AutoISSUE.DBConstants.sqlVehLicStateStr) +
                    " " +
                    GetColumnValueFromDataRow(item.rawRow, AutoISSUE.DBConstants.sqlVehLicNoStr);

                ((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(1, ref loFormattedData, item.rawRow, position);
                view.FindViewById<TextView>(Resource.Id.txt2).Text = loFormattedData;

                ((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(2, ref loFormattedData, item.rawRow, position);
                view.FindViewById<TextView>(Resource.Id.txt3).Text = loFormattedData;

                ((MarkModeStructLogicAndroid)item.fSearchStructLogic).GetMarkModeTextForSearchMatchListDisplay(3, ref loFormattedData, item.rawRow, position);
                view.FindViewById<TextView>(Resource.Id.txt4).Text = loFormattedData;

            }
            else
            {
                // generic search results
                // TODO - look to the source match form for column info
                string loFormattedData = string.Empty;
                ((SearchStructLogicAndroid)item.fSearchStructLogic).InitSourceFormattingInfo(item.rawRow);

                view.FindViewById<TextView>(Resource.Id.txt1).Text =
                    GetColumnValueFromDataRow(item.rawRow, AutoISSUE.DBConstants.sqlVehLicStateStr) +
                    " " +
                    GetColumnValueFromDataRow(item.rawRow, AutoISSUE.DBConstants.sqlVehLicNoStr);

                // look for some common elements and populate

                string loHotCodeStr = GetColumnValueFromDataRow(item.rawRow, AutoISSUE.DBConstants.sqlHotCodeStr);
                if (string.IsNullOrEmpty(loHotCodeStr) == false)
                {

                    // TODOD - fix right justify
                    view.FindViewById<TextView>(Resource.Id.txt4).Text = "   " + loHotCodeStr;
                    //view.FindViewById<TextView>(Resource.Id.txt4).Text = loHotCodeStr;
                }
                else
                {
                    view.FindViewById<TextView>(Resource.Id.txt4).Visibility = ViewStates.Invisible;
                }


                string loPermitNoStr = GetColumnValueFromDataRow(item.rawRow, AutoISSUE.DBConstants.sqlVehPermitNumberStr);
                if (string.IsNullOrEmpty(loPermitNoStr) == false)
                {
                    view.FindViewById<TextView>(Resource.Id.txt2).Text = loPermitNoStr;
                }
                else
                {
                    view.FindViewById<TextView>(Resource.Id.txt2).Visibility = ViewStates.Invisible;
                }


                view.FindViewById<TextView>(Resource.Id.txt3).Visibility = ViewStates.Gone;
            }




         
            // put search match type

            // icon to represent hit type: valid permit, previous mark, stolen, boot. tow. define hot codes to icons defs in registy

            // markmode: when previous marks found, time elapsed graduated green to yellow to red - OK, expiring in 5 mins, in violation

	        return view;
	    }

#endif

        private TTDBListBox DrillDownToDBListGridRecursive(List<TWinClass> iControls)
        {
            foreach (TWinClass oneControl in iControls)
            {
                // is this it?
                if (oneControl is TTDBListBox)
                {
                    if (((TTDBListBox)oneControl).Columns.Count > 0)
                    {
                        return (oneControl as TTDBListBox);
                    }
                }


                // hiding in one of these?
                if (oneControl is TEditField)
                {
                    if (((TEditField)oneControl).DBListGrid != null)
                    {
                        if (((TEditField)oneControl).DBListGrid.Columns.Count > 0)
                        {
                            object loDBGrid = ((TEditField)oneControl).DBListGrid;
                            return (loDBGrid as TTDBListBox);
                        }
                    }

                }

                // is this another container?
                if (oneControl is TTPanel)
                {
                    // drill down
                    return DrillDownToDBListGridRecursive( ((TTPanel)oneControl).Controls );
                }
            }

            return null;
        }

        private List<TGridColumnInfo> ExtractDBListBoxDisplayFormatInfo( SearchMatchDTO iMatchItem )
        {
            List<TGridColumnInfo> loMatchGridColumnInfo = new List<TGridColumnInfo>();

            // drill down and come back with the 1st DBListGid
            TTDBListBox oneDBListGrid = DrillDownToDBListGridRecursive(iMatchItem.fSearchStructLogic.SearchMatchResultsForm.Controls);
            
            if ( oneDBListGrid != null )
            {
                if ( oneDBListGrid.Columns.Count > 0 )
                {
                    loMatchGridColumnInfo.AddRange( oneDBListGrid.Columns );
                }
            }

            return loMatchGridColumnInfo;
        }


        private View MakeColumnTitle(TGridColumnInfo iColumnInfo )
        {
            //    <TextView
            //        android:textAppearance="?android:attr/textAppearanceMedium"
            //        android:layout_height="wrap_content"
            //        android:text="Expired Since"
            //        android:gravity="center"
            //        android:layout_width="wrap_content"
            //        android:id="@+id/card_band_clock1_label"
            //        android:textColor="@color/civicsmart_black" />
            


            string labelText = string.Empty;
            if (string.IsNullOrEmpty(iColumnInfo.ColTitle) == false)
            {
                labelText = iColumnInfo.ColTitle;
            }


            var textView = new TextView(_context)
            {
                Text = labelText,
                Tag = ""
            };


            LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            textView.LayoutParameters = lp;


            // initialize our typeface 
            Typeface loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnCardViewHeaderTextSmallestTypeface);
            if (loCustomTypeface != null)
            {
                textView.Typeface = loCustomTypeface;
            }
            textView.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnCardViewHeaderTextSmallestTypefaceSizeSp);


            return textView;
        }


        private View MakeColumnData(TGridColumnInfo iColumnInfo, string iText )
        {
            //    <TextView
            //        android:textAppearance="?android:attr/textAppearanceLarge"
            //        android:background="@drawable/cardviewclock"
            //        android:layout_height="wrap_content"
            //        android:text="17:32"
            //        android:gravity="center"
            //        android:layout_gravity="center_vertical|left"
            //        android:textColor="@color/civicsmart_black"
            //        android:layout_width="100dp"
            //        android:id="@+id/card_band_clock1" />

            string labelText = string.Empty;
            if (string.IsNullOrEmpty(iText) == false)
            {
                labelText = iText;
            }


            var textView = new TextView(_context)
            {
                Text = labelText,
                Tag = ""
            };


            textView.SetBackgroundResource(Resource.Drawable.CardViewClock);


            // scale our box according to the ratio in the iColumm info

            int loBaseLayoutScaleWidth = 320;

            int loScaledWidthWeight = ( iColumnInfo.Width / loBaseLayoutScaleWidth );

            int loCalculatedWidth = ViewGroup.LayoutParams.WrapContent;

            int loLeft = 5;
            int loRight = 5;
            int loTop = 0;
            int loBottom = 0;


            LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams( loCalculatedWidth , ViewGroup.LayoutParams.WrapContent);
            lp.SetMargins(loLeft, loTop, loRight, loBottom);
            textView.LayoutParameters = lp;

            // initialize our typeface 
            Typeface loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnCardViewHeaderTextClockTypeface);
            if (loCustomTypeface != null)
            {
                textView.Typeface = loCustomTypeface;
            }
            textView.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnCardViewHeaderTextClockTypefaceSizeSp);


            return textView;
        }


        private LinearLayout BuildOneColumnLinearLayout(TGridColumnInfo iColumnInfo, SearchMatchDTO item, int position)
        {
            // <LinearLayout
            //    android:id="@+id/card_band_clock"
            //    android:layout_width="wrap_content"
            //    android:layout_height="wrap_content"
            //    android:orientation="vertical"
            //    android:visibility="visible"
            //    android:layout_alignParentLeft="true"
            //    android:gravity="center">
            //    <TextView
            //        android:id="@+id/card_band_clock1_label"
            //    <TextView
            //        android:id="@+id/card_band_clock1" />
            //</LinearLayout>


            LinearLayout layoutOneColumn = new LinearLayout(_context);
            layoutOneColumn.Orientation = Orientation.Vertical;
            layoutOneColumn.SetGravity(GravityFlags.Center);

            LinearLayout.LayoutParams layoutOneParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            layoutOneColumn.LayoutParameters = layoutOneParams;

            layoutOneColumn.AddView( MakeColumnTitle( iColumnInfo ));

            layoutOneColumn.AddView( MakeColumnData( iColumnInfo, GetColumnValueFromDataRow( item.rawRow, iColumnInfo.Name )));


            return layoutOneColumn;
        }




        private void MakeDBGridRowView(RelativeLayout iParentLayout, SearchMatchDTO item, int position)
        {

            List<TGridColumnInfo> loMatchGridColumns = ExtractDBListBoxDisplayFormatInfo(item);


            int loCounter = 0;
            LinearLayout loLastColumnLayout = null;

            foreach (TGridColumnInfo oneColumn in loMatchGridColumns)
            {
                LinearLayout oneColumnLinearLayout = BuildOneColumnLinearLayout(oneColumn, item, position);

                // give it an id so we can reference it
                oneColumnLinearLayout.Id = View.GenerateViewId();

                RelativeLayout.LayoutParams oneColumnRelLayoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

                // setup each to the right of the previous column
                if (loLastColumnLayout != null)
                {
                    oneColumnRelLayoutParams.AddRule(LayoutRules.RightOf, loLastColumnLayout.Id);
                }

                oneColumnLinearLayout.LayoutParameters = oneColumnRelLayoutParams;
                


                iParentLayout.AddView(oneColumnLinearLayout);

                loLastColumnLayout = oneColumnLinearLayout;

                //loCounter++;
                //if (loCounter > 1)
                //{
                //    break;
                //}
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


