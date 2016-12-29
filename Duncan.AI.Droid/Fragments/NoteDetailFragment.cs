using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;

using Duncan.AI.Droid.Common;


namespace Duncan.AI.Droid
{
	public class NoteDetailFragment : Fragment
	{
        string _tagName;
        string _structName;


        TextView _txtNoteId;
        TextView _txtNoteMemo;
        Button _btnSave;
        Button _btnPhoto;
        //Button _btnVideo;
        Button _btnBack;
        String _noteId;
        ParkNoteDTO _parkNoteDTO;

         File _photoFile;
        File _photoDir;
        ImageView _imageView;

        ISharedPreferences _prefs;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
            // AJW - always call the superclass
            base.OnCreateView(inflater, container, savedInstanceState);

            _structName = Arguments.GetString("structName", null);
            //_struct = DroidContext.XmlCfg.GetStructByName(structName);

            _tagName = Helper.BuildIssueNoteDetailFragmentTag(_structName);


			View view = inflater.Inflate (Resource.Layout.Note_detail, null);
            _prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);

            _txtNoteId = view.FindViewById<TextView>(Resource.Id.txtNoteID);
            _txtNoteMemo = view.FindViewById<TextView>(Resource.Id.txtNoteMemo);
            _btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
            _btnPhoto = view.FindViewById<Button>(Resource.Id.btnPhoto);
           // _btnVideo = view.FindViewById<Button>(Resource.Id.btnVideo);
            _btnBack = view.FindViewById<Button>(Resource.Id.btnBack);
            
            _btnSave.Click += BtnSaveClick;
            _btnSave.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.button));

            _btnPhoto.Click += BtnPhotoClick;
            //btnPhoto.SetWidth(Convert.ToInt32(Resources.DisplayMetrics.WidthPixels * .5));
            _btnPhoto.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.button_secondary));

            //_btnVideo.Click += BtnVideoClick;
            //btnVideo.SetWidth(Convert.ToInt32(Resources.DisplayMetrics.WidthPixels * .5));
           // _btnVideo.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.button_secondary));

            _btnBack.Click += BtnBackClick;
            _btnBack.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.button_back));

            _imageView = view.FindViewById<ImageView>(Resource.Id.imageView1);

            if (IsThereAnAppToTakePictures())
                CreateDirectoryForPictures();
            else
            {
                _btnPhoto.Visibility = ViewStates.Invisible;
               // _btnVideo.Visibility = ViewStates.Invisible;
            }

			CreateLayout();
			return view;
		}

		public async Task<ParkNoteDTO> CreateLayout(){

            _noteId = _prefs.GetString(Constants.PARKNOTE_ROWID, null);

            // if it is an existing note
            if (_noteId != null)
            {
			    var parkingSequenceADO = new ParkingSequenceADO ();
                Log.Debug("NoteDetailFragment NoteId", _noteId);
			    Task<ParkNoteDTO> result = parkingSequenceADO.GetParkNoteByNoteId(_noteId);
			    _parkNoteDTO = await result;

                _txtNoteId.Text = _parkNoteDTO.DBRowId;
                _txtNoteMemo.Text = _parkNoteDTO.NotesMemo;

                _photoFile = new File(_photoDir, _parkNoteDTO.MultiMediaNoteFileName);

                ShowImage();
            }
            else
            {
                _parkNoteDTO = null;
                _txtNoteId.Text = String.Empty;
                _txtNoteMemo.Text = String.Empty;
                _photoFile = null;
                _imageView.SetImageBitmap(null);
            }
            /*
			Task<bool> status = ParkingSequenceADO.IsTicketEditable(this.Activity);
			bool awaitStatus = await status;

			if (awaitStatus)
            */

            if (_parkNoteDTO == null)
            {
                _btnSave.Visibility = ViewStates.Visible;
                _btnPhoto.Visibility = ViewStates.Visible;
               // _btnVideo.Visibility = ViewStates.Visible;
                _txtNoteMemo.InputType = Android.Text.InputTypes.TextFlagMultiLine;
            }
            else
            {
                _btnSave.Visibility = ViewStates.Gone;
                _btnPhoto.Visibility = ViewStates.Gone;
               // _btnVideo.Visibility = ViewStates.Gone;
                _txtNoteMemo.InputType = 0;  // prevents input
            }

            return _parkNoteDTO;
		}

	    private async void BtnSaveClick(object sender, EventArgs e)
	    {
            // AJW TODO - why two prefs declarations?

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            //ISharedPreferencesEditor editor = prefs.Edit();
            string structName = prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);

            // same right?
            string structName2 = _prefs.GetString(Constants.STRUCT_NAME_TKT_DTL, null);

            if (structName2.Equals(structName) == true)
            {
                if (_parkNoteDTO == null)  // debug breakpoint
                {
                    _parkNoteDTO = new ParkNoteDTO();
                }
            }


	        if (_parkNoteDTO == null)
	            _parkNoteDTO = new ParkNoteDTO();

	        _parkNoteDTO.SeqId = _prefs.GetString(Constants.ISSUENO_COLUMN, null);
            _parkNoteDTO.MasterKey = _prefs.GetString(Constants.ID_COLUMN, string.Empty);
	        _parkNoteDTO.NotesMemo = _txtNoteMemo.Text;
	        _parkNoteDTO.DBRowId = _noteId;

	        _parkNoteDTO.NoteDate = DateTimeOffset.Now.ToString(Constants.DT_YYYYMMDD);
	        _parkNoteDTO.NoteTime = DateTimeOffset.Now.ToString(Constants.TIME_HHMMSS);
	        _parkNoteDTO.OfficerId = _prefs.GetString(Constants.OFFICER_ID, null);
	        _parkNoteDTO.OfficerName = _prefs.GetString(Constants.OFFICER_NAME, null);

	        if (_photoFile != null)
	        {
	            _parkNoteDTO.MultiMediaNoteFileName = _photoFile.Name;
	            //_parkNoteDTO.MultiMediaNoteDataType = "2"; // TODO : a number code for  photo, video etc.
                _parkNoteDTO.MultiMediaNoteDataType = AutoISSUE.DBConstants.TMultimediaType.mmPicture.ToString();

	            // this file LastModified will return milliseconds since 1/1/1970
	            var dt = new DateTime(1970, 1, 1);
	            dt = dt.AddMilliseconds(_photoFile.LastModified());
	            _parkNoteDTO.MultiMediaNoteDateStamp = dt.ToString(Constants.DT_YYYYMMDD);
	            _parkNoteDTO.MultiMediaNoteTimeStamp = dt.ToString(Constants.TIME_HHMMSS);
	        }

	        var parkingSequenceADO = new ParkingSequenceADO();


            ////go get the parking tickets status
            //var commonADO = new CommonADO();
            //var result = commonADO.GetRowInfoBySequenceId(_parkNoteDTO.SeqId, structName);
            //var parkingDTO = await result;

            ////if the ticket has been submitted, 
            //if (Constants.STATUS_ISSUED.Equals(parkingDTO.Status) || Constants.STATUS_REISSUE.Equals(parkingDTO.Status))
            //    submitNoteInfo = true;


	        // if it is a new note, then insert
            if (_parkNoteDTO.DBRowId == null)
            {
                await parkingSequenceADO.InsertRowParkNote(_parkNoteDTO);
            }
            else
            {
                await parkingSequenceADO.UpdateParkNote(_parkNoteDTO);
            }


            // any kind of record update, we'll look to upload it ASAP
            Activity.StartService(new Intent(Activity, typeof(SyncService)));

	        Toast.MakeText(Activity, "Note Saved", ToastLength.Long).Show();

	        ReturnToNotesFragment();
	    }

	    private void BtnBackClick(object sender, EventArgs e)
		{
            ReturnToNotesFragment();
		}



        private void ReturnToNotesFragment()
        {
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();


            // hide this fragment
            //var noteDetail = (NoteDetailFragment)FragmentManager.FindFragmentByTag(Constants.NOTE_DETAIL_FRAGMENT_TAG);
            var noteDetail = (NoteDetailFragment)FragmentManager.FindFragmentByTag(_tagName);
            if (noteDetail != null)
            {
                fragmentTransaction.Hide(noteDetail);
            }

            string prevFragmentTag = DroidContext.MyFragManager.PopInternalBackstack();

            if (string.IsNullOrEmpty(prevFragmentTag) == true)
            {
                // we are lost?... this is the data type we need. shouldn't occur
                prevFragmentTag = Helper.BuildIssueNotesFragmentTag(_structName);
            }


            var notesFragment = (NotesFragment)FragmentManager.FindFragmentByTag(prevFragmentTag);
            //var notesFragment = (NotesFragment)FragmentManager.FindFragmentByTag(Constants.NOTES_FRAGMENT_TAG);
            if (notesFragment != null)
            {
                fragmentTransaction.Show(notesFragment);
                notesFragment.GetNotesByTicket();
            }
            else
            {
                // here for completeness and reference - this should not ever happen, we're expecting to go back to our parent fragment

                // lost it, have to rebuild??
                Android.App.Fragment fragment = new NotesFragment { Arguments = new Bundle() };
                fragment.Arguments.PutString("structName", _structName);

                //fragmentTransaction.Replace(Resource.Id.frameLayout1, fragment, prevFragName);
                //fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, prevFragName);
                //fragmentTransaction.Replace(Resource.Id.frameLayout1, new NotesFragment(), Constants.NOTES_FRAGMENT_TAG);
                fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, prevFragmentTag);
            }

            fragmentTransaction.Commit();
        }


        private void BtnPhotoClick(object sender, EventArgs e)
        {
            ActivateCamera(MediaStore.ActionImageCapture, Constants.PHOTO_FILE_SUFFIX);
        }

        private void BtnVideoClick(object sender, EventArgs e)
        {
            ActivateCamera(MediaStore.ActionVideoCapture, Constants.VIDEO_FILE_SUFFIX);
        }

        private void ActivateCamera(String action, String fileSuffix)
        {
            String fileNamePrefix = _prefs.GetString(Constants.DEVICEID, null) + "_";
            var intent = new Intent(action);
            _photoFile = new File(_photoDir, fileNamePrefix + DateTimeOffset.Now.ToString("yyyy_MM_dd_HH_mm_ss") + fileSuffix);
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_photoFile));
            StartActivityForResult(intent, 0);
        }

        private bool IsThereAnAppToTakePictures()
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            //IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            //return availableActivities != null && availableActivities.Count > 0;
            return true;
        }

        private void CreateDirectoryForPictures()
        {
            _photoDir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), Constants.MULTIMEDIA_FOLDERNAME);
            if (!_photoDir.Exists())
            {
                _photoDir.Mkdirs();
            }
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // make it available in the gallery
            var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Android.Net.Uri contentUri = Android.Net.Uri.FromFile(_photoFile);

            mediaScanIntent.SetData(contentUri);
//            SendBroadcast(mediaScanIntent);
            ShowImage();




#if _lpr_
            ///  LPR integration test

            AutomaticNumberPlateRecognitionServerInterface myANPRWebService = new AutomaticNumberPlateRecognitionServerInterface( Activity, FragmentManager);

            string loImageFile = _photoFile.AbsolutePath;


            //loImageFile = @"/storage/emulated/0/Pictures/civicsmart/S5-197AE_2016_02_06_11_30_31.jpg";

            loImageFile = @"/storage/emulated/0/Pictures/civicsmart/20160202_163307.jpg";


            myANPRWebService.CallAutomaticNumberPlateRecognitionService(loImageFile);
            {
                //we havre read a plate
            }


            ShowANPRConfirmationFragment();
            ///
#endif

        }


        private void ShowImage()
        {
            Bitmap bitmap = null;
            if (_photoFile.AbsolutePath.Contains(Constants.VIDEO_FILE_SUFFIX))
                bitmap = Android.Media.ThumbnailUtils.CreateVideoThumbnail(_photoFile.AbsolutePath, ThumbnailKind.MiniKind);
            else if (_photoFile.AbsolutePath.Contains(Constants.PHOTO_FILE_SUFFIX))
            {
                // resize the bitmap to fit the display, Loading the full sized image will consume too much memory 
                int height = _imageView.Height;
                int width = Resources.DisplayMetrics.WidthPixels;
                bitmap = _photoFile.Path.LoadAndResizeBitmap(width, height);               
            }

            _imageView.SetImageBitmap(bitmap);
        }

	}
    
}
