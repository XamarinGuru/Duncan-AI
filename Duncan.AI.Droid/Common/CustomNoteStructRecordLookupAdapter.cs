using System.Collections.Generic;
using System.Text;
using System;
using Android.App;
using Android.Graphics;
using Android.Content;
using Android.Provider;
using Android.Views;
using Android.Widget;
using AutoISSUE;
using Duncan.AI.Droid.Utils.HelperManagers;
using Reino.ClientConfig;

namespace Duncan.AI.Droid
{

    public class CustomNoteStructRecordLookupAdapter : BaseAdapter<ParkNoteDTO>
    {
        List<ParkNoteDTO> items;
        Activity context;
        string tableName;

        Java.IO.File _photoFile;
        Java.IO.File _photoDir;

        


        public CustomNoteStructRecordLookupAdapter(Activity context, List<ParkNoteDTO> items)
            : base()
        {
            this.context = context;
            this.items = items;

          _photoDir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), Constants.MULTIMEDIA_FOLDERNAME);
        }

        /// <summary>
        /// Clean up cached bitmaps
        /// </summary>
        ~CustomNoteStructRecordLookupAdapter()
        {
            try
            {
                if (_myThumbnailColection != null)
                {
                    foreach (ThumbnailCache oneThumbnailCache in _myThumbnailColection)
                    {
                        if (oneThumbnailCache.thumbnailBitmap != null)
                        {
                            oneThumbnailCache.thumbnailBitmap.Recycle();
                            oneThumbnailCache.thumbnailBitmap.Dispose();
                            oneThumbnailCache.thumbnailBitmap = null;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "CustomNoteStructRecordLookupAdapter.FindThumbnailCache", ex.TargetSite.Name);
                System.Console.WriteLine("CustomNoteStructRecordLookupAdapter::FindThumbnailCache Exception source {0}: {1}", ex.Source, ex.ToString());
            }

        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override ParkNoteDTO this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }




        public class ThumbnailCache
        {
            public string originalFileName;
            public Bitmap thumbnailBitmap;
        }

        public List<ThumbnailCache> _myThumbnailColection = new List<ThumbnailCache>();


        public Bitmap FindThumbnailCache(string iOriginalFilename)
        {
            try
            {
                foreach (ThumbnailCache oneThumbnailCache in _myThumbnailColection)
                {
                    if (oneThumbnailCache.originalFileName.Equals(iOriginalFilename) == true)
                    {
                        return oneThumbnailCache.thumbnailBitmap;
                    }
                }

                // not found, create and add it for next time
                ThumbnailCache newThumbnailCache = new ThumbnailCache();
                newThumbnailCache.originalFileName = iOriginalFilename;
                _myThumbnailColection.Add(newThumbnailCache);

                _photoFile = new Java.IO.File(_photoDir, iOriginalFilename);

                if (_photoFile.AbsolutePath.Contains(Constants.VIDEO_FILE_SUFFIX))
                {
                    newThumbnailCache.thumbnailBitmap = Android.Media.ThumbnailUtils.CreateVideoThumbnail(_photoFile.AbsolutePath, ThumbnailKind.MiniKind);
                    return newThumbnailCache.thumbnailBitmap;
                }
                else if (_photoFile.AbsolutePath.Contains(Constants.PHOTO_FILE_SUFFIX))
                {

                    // resize the bitmap to fit the display, Loading the full sized image will consume too much memory 
                    //int height = reproductionImage.Height;
                    //int width = Resources.DisplayMetrics.WidthPixels;
                    //bitmap = _photoFile.Path.LoadAndResizeBitmap(width, height);

                    int height = 384; // (int)Math.Round(Resources.DisplayMetrics.HeightPixels * 0.05);
                    int width = 384; // (int)Math.Round(Resources.DisplayMetrics.WidthPixels * 0.05);

                    //bitmap = BitmapHelpers.decodeSampledBitmapFromFile(_photoFile.Path, width, height);

                    newThumbnailCache.thumbnailBitmap = BitmapHelpers.LoadAndResizeBitmap(_photoFile.Path, width, height);
                    return newThumbnailCache.thumbnailBitmap;

                    //                var rotatedBitmap = BitmapHelpers.RotateBitmap(bitmap, GetRotationAngle());
                }

                return null;
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "CustomNoteStructRecordLookupAdapter.FindThumbnailCache", ex.TargetSite.Name);
                System.Console.WriteLine("CustomNoteStructRecordLookupAdapter::FindThumbnailCache Exception source {0}: {1}", ex.Source, ex.ToString());
            }

            return null;
        }


        
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];

            //find view to re-use or create new one
            View view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.noteslist_layout, null);


            // set alternating colors to increase visibility
            //view.SetBackgroundColor(position % 2 == 1 ? Android.Graphics.Color.AntiqueWhite : Android.Graphics.Color.Azure);

            view.SetBackgroundColor(position % 2 == 1 ? context.Resources.GetColor( Resource.Color.civicsmart_gray) : Android.Graphics.Color.AntiqueWhite  );


            // show thumbnail indication
            AutoISSUE.DBConstants.TMultimediaType loAttachmentType = AutoISSUE.DBConstants.GetMultimediaTypeForDisplayName(item.MultiMediaNoteDataType);

            Bitmap loThumbnailBitmap = null;
            if ( string.IsNullOrEmpty( item.MultiMediaNoteFileName ) == false )
            {
                loThumbnailBitmap = FindThumbnailCache(item.MultiMediaNoteFileName ); 
            }


            ImageView thumbnailView = view.FindViewById<ImageView>(Resource.Id.imgViewLogo);

            switch (loAttachmentType)
            {
                case DBConstants.TMultimediaType.mmPicture:
                    {
                        if (loThumbnailBitmap != null)
                        {
                            thumbnailView.SetImageBitmap(loThumbnailBitmap);
                        }
                        else
                        {
                            thumbnailView.SetImageResource(Resource.Drawable.ic_photo_camera_black_36dp);
                        }
                        break;
                    }
                case DBConstants.TMultimediaType.mmDiagram:
                    {
                        view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ic_view_list_black_36dp);
                        break;
                    }
                case DBConstants.TMultimediaType.mmWaveAudio:
                    {
                        view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ic_view_list_black_36dp);
                        break;
                    }
                default:
                    {
                        view.FindViewById<ImageView>(Resource.Id.imgViewLogo).SetImageResource(Resource.Drawable.ic_view_list_black_36dp);
                        break;
                    }
            }




            //view.FindViewById<TextView>(Resource.Id.txt1).Text = item.VEHICLE_DISPLAY;
            //view.FindViewById<TextView>(Resource.Id.txt2).Text = item.ISSUENO_DISPLAY + " " + item.sqlIssueDateStr + " " + item.sqlIssueTimeStr;


            TextView loTextLine1 = view.FindViewById<TextView>(Resource.Id.txt1);
            loTextLine1.Text = item.NotesMemo; // TODO elipisies?

            TextView loTextLine2 = view.FindViewById<TextView>(Resource.Id.txt2);
            TextView loTextLine3 = view.FindViewById<TextView>(Resource.Id.txt3);






            // init to default
            string loLine2InfoString = item.NoteDate + " " + item.NoteTime;

            XMLConfig.IssStruct _struct = null;

            string loStructName = "PARKING"; // kludge!

            if (string.IsNullOrEmpty(loStructName) == false)
            {
                _struct = DroidContext.XmlCfg.GetStructByName(loStructName);

                if (_struct != null)
                {

                    if (_struct.fDisplayFormattingInfo != null)
                    {
                        // build it up in pieces
                        StringBuilder oneFormattedInfo = new StringBuilder();

                        // first is ticket issue number
                        //oneFormattedInfo.Append(item.ISSUENO_DISPLAY + "  ");

                        // then issue date
                        if (string.IsNullOrEmpty(_struct.fDisplayFormattingInfo.fStructDateMask) == false)
                        {
                            // would prefer to use the masks
                            oneFormattedInfo.Append(CultureDisplayFormatLogic.ConvertDateStringToDisplayFormatLocalTime(
                                item.NoteDate,
                                Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK,
                                _struct.fDisplayFormattingInfo.fStructDateMask) + " ");
                        }
                        else
                        {
                            // default back to original
                            oneFormattedInfo.Append(item.NoteDate + "  ");
                        }


                        // issue time
                        if (string.IsNullOrEmpty(_struct.fDisplayFormattingInfo.fStructTimeMask) == false)
                        {
                            // would prefer to use the masks
                            oneFormattedInfo.Append(CultureDisplayFormatLogic.ConvertDateStringToDisplayFormatLocalTime(
                                item.NoteTime,
                                Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK,
                                _struct.fDisplayFormattingInfo.fStructTimeMask) + " ");
                        }
                        else
                        {
                            // default back to original
                            oneFormattedInfo.Append(item.NoteTime + "  ");
                        }


                        // final aggregation of formatted data 
                        loLine2InfoString = oneFormattedInfo.ToString();
                    }

                }

            }


            // your final answer for this one
            loTextLine2.Text = "";


            // put the time stamp in the small print
            loTextLine3.Text = loLine2InfoString;


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

