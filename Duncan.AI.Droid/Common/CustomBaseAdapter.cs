using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Views;
using Android.Widget;
using Duncan.AI.Droid.Utils.HelperManagers;
using Reino.ClientConfig;

namespace Duncan.AI.Droid
{
    public class CustomIssueStructRecordLookupAdapter : BaseAdapter<CommonDTO>
    {
        List<CommonDTO> items;
        Activity context;
        string tableName;
        public CustomIssueStructRecordLookupAdapter(Activity context, List<CommonDTO> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override CommonDTO this[int position]
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
            View view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.listview_layout, null);


            // set alternating colors to increase visibility
            //view.SetBackgroundColor(position % 2 == 1 ? Android.Graphics.Color.AntiqueWhite : Android.Graphics.Color.Azure);

            view.SetBackgroundColor(position % 2 == 1 ? context.Resources.GetColor(Resource.Color.civicsmart_gray) : Android.Graphics.Color.AntiqueWhite );


            // set the icon to indicate status
            //if (Constants.STATUS_READY.Equals(item.parkingStatus))
            //    view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_ready);
            //else if (Constants.STATUS_REISSUE.Equals(item.parkingStatus))
            //    view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_reissue);
            //else if (Constants.STATUS_ISSUED.Equals(item.parkingStatus))
            //    view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_issued);
            //else if (Constants.STATUS_VOIDED.Equals(item.parkingStatus))
            //    view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ticket_void);


            bool loCiteIsVoid = (item.RawDetailRowVoid != null);
            bool loCiteIsReIssued = (item.RawDetailRowReissue != null);
            bool loCiteIsContinuance = (item.RawDetailRowContinuance != null);





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





            //view.FindViewById<TextView>(Resource.Id.txt1).Text = item.VEHICLE_DISPLAY;
            //view.FindViewById<TextView>(Resource.Id.txt2).Text = item.ISSUENO_DISPLAY + " " + item.sqlIssueDateStr + " " + item.sqlIssueTimeStr;


            TextView loTextLine1 = view.FindViewById<TextView>(Resource.Id.txt1);
            loTextLine1.Text = item.VEHICLE_DISPLAY;

            TextView loTextLine2 = view.FindViewById<TextView>(Resource.Id.txt2);
            TextView loTextLine3 = view.FindViewById<TextView>(Resource.Id.txt3);






            // init to default
            string loLine2InfoString = item.ISSUENO_DISPLAY + " " + item.sqlIssueDateStr + " " + item.sqlIssueTimeStr;

            XMLConfig.IssStruct _struct = null;


            if (string.IsNullOrEmpty(item.structName) == false)
            {
                _struct = DroidContext.XmlCfg.GetStructByName(item.structName);

                if (_struct != null)
                {

                    if (_struct.fDisplayFormattingInfo != null)
                    {
                        // build it up in pieces
                        StringBuilder oneFormattedInfo = new StringBuilder();

                        // first is ticket issue number
                        oneFormattedInfo.Append(item.ISSUENO_DISPLAY + "  ");

                        // then issue date
                        if (string.IsNullOrEmpty(_struct.fDisplayFormattingInfo.fStructDateMask) == false)
                        {
                            // would prefer to use the masks
                            oneFormattedInfo.Append(CultureDisplayFormatLogic.ConvertDateStringToDisplayFormatLocalTime(
                                item.sqlIssueDateStr,
                                Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK,
                                _struct.fDisplayFormattingInfo.fStructDateMask) + " ");
                        }
                        else
                        {
                            // default back to original
                            oneFormattedInfo.Append(item.sqlIssueDateStr + "  ");
                        }


                        // issue time
                        if (string.IsNullOrEmpty(_struct.fDisplayFormattingInfo.fStructTimeMask) == false)
                        {
                            // would prefer to use the masks
                            oneFormattedInfo.Append(CultureDisplayFormatLogic.ConvertDateStringToDisplayFormatLocalTime(
                                item.sqlIssueTimeStr,
                                Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK,
                                _struct.fDisplayFormattingInfo.fStructTimeMask) + " ");
                        }
                        else
                        {
                            // default back to original
                            oneFormattedInfo.Append(item.sqlIssueTimeStr + "  ");
                        }


                        // final aggregation of formatted data 
                        loLine2InfoString = oneFormattedInfo.ToString();
                    }

                }

            }


            // your final answer for this one
            loTextLine2.Text = loLine2InfoString;




            string loVoidStatusDetailStr = "VALID";
            if (loCiteIsVoid)
            {
                string loVoidDateAsStr = Helper.GetSafeColumnStringValueFromDataRow( item.RawDetailRowVoid, Constants.RECCREATIONDATE_COLUMN);
                string loVoidTimeAsStr = Helper.GetSafeColumnStringValueFromDataRow( item.RawDetailRowVoid, Constants.RECCREATIONTIME_COLUMN);

                loVoidStatusDetailStr = "Voided " + FormatDateTimeStringInStructMask(_struct, loVoidDateAsStr, loVoidTimeAsStr);
            }

            string loReIssueStatusDetailStr = "";
            if (loCiteIsReIssued)
            {
                string loReIssueDateAsStr = Helper.GetSafeColumnStringValueFromDataRow( item.RawDetailRowReissue, Constants.RECCREATIONDATE_COLUMN);
                string loReIssueTimeAsStr = Helper.GetSafeColumnStringValueFromDataRow( item.RawDetailRowReissue, Constants.RECCREATIONTIME_COLUMN);

                loReIssueStatusDetailStr = "Reissued " + FormatDateTimeStringInStructMask(_struct, loReIssueDateAsStr, loReIssueTimeAsStr);
            }


            string loStatusStr = loVoidStatusDetailStr;
            if (string.IsNullOrEmpty(loReIssueStatusDetailStr) == false)
            {
                loStatusStr = loStatusStr + "\n" + loReIssueStatusDetailStr;
            }

            loTextLine3.Text = loStatusStr;


            return view;
        }



        private string FormatDateTimeStringInStructMask(XMLConfig.IssStruct iIssStruct, string iDateStringInFixedDBFormat, string iTimeStringInFixedDBFormat)
        {
            // default formatting
            string loFormattedResultText = iDateStringInFixedDBFormat + "  " + iTimeStringInFixedDBFormat;

            // let's see if we can format this in the defined format of the structure
            if (iIssStruct != null)
            {
                if (iIssStruct.fDisplayFormattingInfo != null)
                {
                    // build it up in pieces
                    StringBuilder oneFormattedInfo = new StringBuilder();

                    // issue date
                    if (
                         (string.IsNullOrEmpty(iIssStruct.fDisplayFormattingInfo.fStructDateMask) == false) &&
                         (string.IsNullOrEmpty(iDateStringInFixedDBFormat) == false)
                        )
                    {
                        // would prefer to use the masks
                        oneFormattedInfo.Append(CultureDisplayFormatLogic.ConvertDateStringToDisplayFormatLocalTime(
                            iDateStringInFixedDBFormat,
                            Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK,
                            iIssStruct.fDisplayFormattingInfo.fStructDateMask) + " ");
                    }
                    else
                    {
                        // default back to original
                        oneFormattedInfo.Append(iDateStringInFixedDBFormat + "  ");
                    }


                    // issue time
                    if (
                        (string.IsNullOrEmpty(iIssStruct.fDisplayFormattingInfo.fStructTimeMask) == false) &&
                        (string.IsNullOrEmpty(iTimeStringInFixedDBFormat) == false)
                       )
                    {
                        // would prefer to use the masks
                        oneFormattedInfo.Append(CultureDisplayFormatLogic.ConvertDateStringToDisplayFormatLocalTime(
                            iTimeStringInFixedDBFormat,
                            Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK,
                            iIssStruct.fDisplayFormattingInfo.fStructTimeMask) + " ");
                    }
                    else
                    {
                        // default back to original
                        oneFormattedInfo.Append(iTimeStringInFixedDBFormat + "  ");
                    }


                    // final aggregation
                    loFormattedResultText = oneFormattedInfo.ToString();
                }


            }

            return loFormattedResultText;
        }


    }


}

