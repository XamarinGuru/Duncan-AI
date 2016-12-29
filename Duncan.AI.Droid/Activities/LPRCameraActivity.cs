#define _USE_NEW_FOCUS_
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Hardware;
using Android.Content.PM;
using Android.Graphics;
using Android.Provider;
using Java.IO;
using Reino.ClientConfig;
using System.Net;
using Android.Net;
using Android.Util;
using Android.Media;
using Duncan.AI.Droid.Utils.HelperManagers;

namespace Duncan.AI.Droid.Activities
{
    //LPRCameraActivity
    [Activity(Label = "Take Plate's Photo")]
    public class LPRCameraActivity : Activity,
        ISurfaceHolderCallback,
        Android.Hardware.Camera.IShutterCallback,
        Android.Hardware.Camera.IPictureCallback,
        Android.Hardware.Camera.IPreviewCallback,
		Android.Hardware.Camera.IAutoFocusCallback
    {

        #region Var
        //private const string _uploadPath = "http://69.197.149.66:8000/";        
        private int _compressRatio = 100;
        private static Bitmap _bitmap;
        private static float _scaleFactor;
		private static float _scaleFactorW;
        private static String _fileName = String.Empty;
        private static Java.IO.File _file;
        private Android.Hardware.Camera _camera;
        private SurfaceView _surfaceView;
        private ISurfaceHolder _surfaceHolder;
        private bool _previewing = false;
        private LayoutInflater _controlInflater = null;
        private View _cameraControlView;
        private Android.Widget.RelativeLayout.LayoutParams _layoutParamsControl;
        private Android.Hardware.Camera.Size _PreviewSize;
        private IList<Android.Hardware.Camera.Size> _SupportedPreviewSizes;
        
        #if _USE_NEW_FOCUS_
		private Handler mAutoFocusHandler;
		private bool _safeToTakePicture = false;
        #endif

        public bool IsConnectedToNetwork
        {
            get
            {
                ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
                NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
                bool isOnline = (activeConnection != null) && activeConnection.IsConnected;
                return (isOnline);
            }
        }

        #endregion

        #region Overrides

        private void HandleLPRException(Exception e)
        {
            LoggingManager.LogApplicationError(e, "LPRCameraActivity Exception", e.TargetSite.Name);
            Duncan.AI.Droid.Utils.ErrorHandling.ReportExceptionWithConfirmationDlg(e.Message);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			#if _USE_NEW_FOCUS_
			mAutoFocusHandler = new Handler ();
            #endif
            if (!IsThereAnAppToTakePictures())
            {
                Toast.MakeText(this, "No app found to take a photo", ToastLength.Long).Show();
                return;
            }
            //Also make sure that there is network connection
            if (IsConnectedToNetwork)
            {
                //Make sure that the HHTP connection is ok
                try
                {
                    string url = "https://encrypted.google.com/";
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    var response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        //We are not ok to continue, return
                        response.Close();
                        return;
                    }
                    response.Close();
                }
                catch (Exception exp)
                {
                    System.Console.WriteLine("HTTPS test failed: " + exp.Message);
                    HandleLPRException(exp);
                }
            }

            try
            {
                _fileName = Intent.GetStringExtra("LPRCameraActivity.PhotFileName");
                _file = new File(_fileName);
                ActionBar.Hide();
                //  RequestedOrientation = ScreenOrientation.Portrait;

                SetContentView(Resource.Layout.Camera);

                Window.SetFormat(Android.Graphics.Format.Jpeg);

                _surfaceView = (SurfaceView)FindViewById(Resource.Id.camerapreview);
                _surfaceHolder = _surfaceView.Holder;
                _surfaceHolder.AddCallback(this);
                _surfaceHolder.SetType(SurfaceType.PushBuffers);

                _controlInflater = LayoutInflater.From(BaseContext);
                _cameraControlView = _controlInflater.Inflate(Resource.Layout.CameraControl, null);
            }
            catch (Exception exp)
            {
                System.Console.WriteLine("LPRCameraActivity failed: " + exp.Message);
                HandleLPRException(exp);
            }
            
        }

        #endregion

        #region Events

		public override bool OnTouchEvent (MotionEvent e)
		{
			Android.Hardware.Camera.Parameters parms = _camera.GetParameters ();
			var action = e.Action;


			if (e.PointerCount > 1) {
				//			// handle multi-touch events
				if (action == MotionEventActions.PointerDown) {
					//mDist = getFingerSpacing (event);
				} else if (action == MotionEventActions.Move && parms.IsZoomSupported) {
					_camera.CancelAutoFocus ();
					handleZoom (e, parms);
				}
			} else {
				// handle single touch events
				if (action == MotionEventActions.Up) {
					handleFocus (e, parms);
				}
			}
			return true;
		}
		float mDist;

		private void handleZoom (MotionEvent e, Android.Hardware.Camera.Parameters parms)
		{
			int maxZoom = parms.MaxZoom;
			int zoom = parms.Zoom;
			float newDist = GetFingerSpacing (e);
			if (newDist > mDist) {
				//zoom in
				if (zoom < maxZoom)
					zoom++;
			} else if (newDist < mDist) {
				//zoom out
				if (zoom > 0)
					zoom--;
			}
			mDist = newDist;
			parms.Zoom = zoom;
			_camera.SetParameters(parms);
		}

		#if !_USE_NEW_FOCUS_
		public void handleFocus (MotionEvent e, Android.Hardware.Camera.Parameters parms)
		{
			List<String> supportedFocusModes = parms.SupportedFocusModes.ToList ();
			if (supportedFocusModes != null && supportedFocusModes.Contains (Android.Hardware.Camera.Parameters.FocusModeAuto)) {
				_camera.AutoFocus (this);
			}
		}
		#else
		//New handleFocus from Nino
		public void handleFocus(MotionEvent e, Android.Hardware.Camera.Parameters parms)
		{
			List<string> supportedFocusModes = parms.SupportedFocusModes.ToList();
			if (supportedFocusModes != null
				&& supportedFocusModes.Contains(Android.Hardware.Camera.Parameters.FocusModeAuto)) {
				SafeAutoFocus();
			}
		}
	   	
		
		private void ScheduleAutoFocus()
		{
			mAutoFocusHandler.PostDelayed (DoAutoFocus, 1000);
		}

		public void SafeAutoFocus()
		{
            try
            {
                _camera.AutoFocus(this);
            }
            catch (Java.Lang.RuntimeException re)
            {
                ScheduleAutoFocus(); // wait 1 sec and then do check again
            }
            catch (Exception exp)
            {
                HandleLPRException(exp);
            }
		}
		private void DoAutoFocus ()
		{
            try
            {
                RunOnUiThread(() =>
                {
                    List<string> supportedFocusModes = _camera.GetParameters().SupportedFocusModes.ToList();
                    if (supportedFocusModes != null && _previewing
                        && supportedFocusModes.Contains(Android.Hardware.Camera.Parameters.FocusModeAuto))
                    {
                        SafeAutoFocus();
                    }
                });
            }
            catch (Exception exp)
            {
                HandleLPRException(exp);
            }
		}
		#endif //_USE_NEW_FOCUS_

		//private Rect calculateTapArea (float x, float y, float coefficient)
		//{
		//	int areaSize = Float.ValueOf (focusAreaSize * coefficient).intValue ();

		//	int left = clamp ((int)x - areaSize / 2, 0, GetSurfaceView ().getWidth () - areaSize);
		//	int top = clamp ((int)y - areaSize / 2, 0, getSurfaceView ().getHeight () - areaSize);

		//	RectF rectF = new RectF (left, top, left + areaSize, top + areaSize);
		//	matrix.mapRect (rectF);

		//	return new Rect (Math.round (rectF.left), Math.round (rectF.top), Math.round (rectF.right), Math.round (rectF.bottom));
		//}

		private int clamp (int x, int min, int max)
		{
			if (x > max) {
				return max;
			}
			if (x < min) {
				return min;
			}
			return x;
		}

		private float GetFingerSpacing (MotionEvent e)
		{
			float x = e.GetX (0) - e.GetX (1);
			float y = e.GetY (0) - e.GetY (1);
			return FloatMath.Sqrt (x * x + y * y);
		}

        void ButtonTakePicture_Click(object sender, EventArgs e)
        {
            try
            {
                Android.Hardware.Camera.Parameters p = _camera.GetParameters();
                p.PictureFormat = Android.Graphics.ImageFormatType.Jpeg;
                _camera.SetParameters(p);
#if !_USE_NEW_FOCUS_
            _camera.TakePicture(this, this, this);
#else
                if (_safeToTakePicture)
                {
                    _camera.TakePicture(this, this, this);
                    _safeToTakePicture = false;
                }
#endif
            }
            catch (Exception exp)
            {
                HandleLPRException(exp);
            }
        }

        #endregion

        #region Interface implementation

        public void OnPreviewFrame(byte[] data, Android.Hardware.Camera camera)
        {

        }

        public void OnPictureTaken(byte[] data, Android.Hardware.Camera camera)
        {
            if (data != null)
            {
                try
                {
                    Java.IO.FileOutputStream outStream = null;
                    outStream = new Java.IO.FileOutputStream(_file);
                    outStream.Write(data);
                    outStream.Close();
                    //Resize the file and save
                    // Getting image width in pixels
                    //int height = Resources.DisplayMetrics.HeightPixels; //_surfaceView.Height;
                    //int width = Resources.DisplayMetrics.WidthPixels; //_surfaceView.Width;;

                    // Setting exif information to undefined
                    var exif = new Android.Media.ExifInterface(_file.Path);
                    //var orientation = exif.GetAttribute (Android.Media.ExifInterface.TagOrientation);
                    //_bitmap = _file.Path.LoadAndResizeBitmap(width, height);
                    exif.SetAttribute(Android.Media.ExifInterface.TagOrientation, "0");
                    exif.SaveAttributes();
                    exif.Dispose();

                    ImageView loRectImageView = (ImageView)FindViewById (Resource.Id.rectimage);

					Bitmap bitmap = BitmapFactory.DecodeFile (_file.AbsolutePath);
					var rotatedBitmap = BitmapHelpers.RotateBitmap (bitmap, GetRotationAngle ());

					bitmap = scaleCenterCrop (rotatedBitmap, loRectImageView.LayoutParameters.Width,
											   loRectImageView.LayoutParameters.Height);

					using (outStream = new Java.IO.FileOutputStream (_file)) {
						outStream.Write (GetBitmapData (bitmap));
						outStream.Close ();
						outStream.Flush ();
					}
					rotatedBitmap.Recycle ();
                    rotatedBitmap.Dispose();
					rotatedBitmap = null;

					bitmap.Recycle ();
                    bitmap.Dispose();
					bitmap = null;


					GC.Collect ();

					this.SetResult (Result.Ok);
					this.Finish ();

					//StopCameraPreview ();	//Ayman This causes crash
                }
                catch (Exception exp)
                {
                    System.Console.WriteLine("LPRCameraActivity failed: " + exp.ToString());
                    HandleLPRException(exp);
					//StopCameraPreview ();
                }
            }
        }

        public Bitmap scaleCenterCrop (Bitmap source, int newWidth, int newHeight)
		{
            try
            {
                int sourceWidth = source.Width;
				int sourceHeight = source.Height;

				if (sourceWidth > newWidth) {

					int screenHeight = Resources.DisplayMetrics.HeightPixels;
					int screeWidth = Resources.DisplayMetrics.WidthPixels;

					var width = newWidth * sourceWidth / screeWidth;
					var height = newHeight * sourceHeight / screenHeight;

					var x = (sourceWidth - width) / 2;
					var y = (sourceHeight - height) / 2;
					var result = Bitmap.CreateBitmap (source, x, y, width, height);

					return result;
				} else {

					// Compute the scaling factors to fit the new height and width, respectively.
					// To cover the final image, the final scaling will be the bigger 
					// of these two.
					float xScale = (float)newWidth / sourceWidth;
					float yScale = (float)newHeight / sourceHeight;
					float scale = Math.Max (xScale, yScale);

					// Now get the size of the source bitmap when scaled
					float scaledWidth = scale * sourceWidth;
					float scaledHeight = scale * sourceHeight;

					// Let's find out the upper left coordinates if the scaled bitmap
					// should be centered in the new size give by the parameters
					float left = (newWidth - scaledWidth) / 2;
					float top = (newHeight - scaledHeight) / 2;

					// The target rectangle for the new, scaled version of the source bitmap will now
					// be
					RectF targetRect = new RectF (left, top, left + scaledWidth, top + scaledHeight);

					// Finally, we create a new bitmap of the specified size and draw our new,
					// scaled bitmap onto it.
					Bitmap dest = Bitmap.CreateBitmap (newWidth, newHeight, source.GetConfig ());
					Canvas canvas = new Canvas (dest);
					canvas.DrawBitmap (source, null, targetRect, null);

					return dest;
            	}
			}
            catch (Exception exp)
            {
                HandleLPRException(exp);
                return null;
            }
		}

		private Bitmap GetRotatedBitmap (Bitmap bitmap)
		{
            try
            {
                Matrix rotateMatrix = new Matrix();
                rotateMatrix.SetRotate(GetRotationAngle(), bitmap.Width / 2, bitmap.Height / 2);
                return Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height,
                        rotateMatrix, true);
            }
            catch (Exception exp)
            {
                HandleLPRException(exp);
                return null;
            }
		}
		
		private RectF CalcImageRect (RectF rect, Matrix matrix)
		{
			RectF applied = new RectF ();
			matrix.MapRect (applied, rect);
			return applied;
		}


		private int GetRotationAngle ()
		{
			Display display = this.WindowManager.DefaultDisplay;

			int rotation = 0;
			switch (display.Rotation) {
			case SurfaceOrientation.Rotation0: // This is display orientation
				rotation = 90;
				break;
			case SurfaceOrientation.Rotation90:
				rotation = 0;
				break;
			case SurfaceOrientation.Rotation180:
				rotation = 270;
				break;
			case SurfaceOrientation.Rotation270:
				rotation = 180;
				break;
			}

			return rotation;
		}

		public void OnShutter ()
		{

		}

		#if !_USE_NEW_FOCUS_
       	public void SurfaceChanged (ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
        {
            if (_previewing) {

                _camera.StopPreview ();
                _previewing = false;
            }

            if (_camera != null) {

                try {

                    FixDisplayOrientation (width, height);

                    _camera.SetPreviewDisplay (_surfaceHolder);
                    _camera.StartPreview ();
                    _previewing = true;


                } catch (Exception ex) {

                    System.Console.WriteLine ("Exception:->>>>>" + ex.ToString ());
                }
            }
        }
		#else
		public void SurfaceChanged (ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
		{
            try
            {

                if (_previewing)
                {

                    StopCameraPreview();

                }
            }
            catch (Exception exp)
            {
                HandleLPRException(exp);
            }

			if (_camera != null) {

				try {

					FixDisplayOrientation (width, height);
					_safeToTakePicture = true;

					_camera.SetPreviewDisplay (_surfaceHolder);
					_camera.StartPreview ();
					_previewing = true;

				} catch (Exception exp) {
					System.Console.WriteLine ("Exception:->>>>>" + exp.ToString ());
                    HandleLPRException(exp);
				}
			}
		}
				
		#endif
		
		public void FixDisplayOrientation (int width, int height)
		{
            try
            {
                var parameters = _camera.GetParameters();
                Display display = this.WindowManager.DefaultDisplay;

                if (display.Rotation == SurfaceOrientation.Rotation0)
                {
                    parameters.SetPreviewSize(height, width);
                    _camera.SetDisplayOrientation(90);
                }

                if (display.Rotation == SurfaceOrientation.Rotation270)
                {
                    parameters.SetPreviewSize(width, height);
                    _camera.SetDisplayOrientation(180);
                }
            }
            catch (Exception exp)
            {
                HandleLPRException(exp);
                return;
            }

		}


        public void SurfaceCreated(ISurfaceHolder holder)
        {
            try
            {
                _camera = Android.Hardware.Camera.Open();


                Android.Hardware.Camera.Parameters p = _camera.GetParameters();

                p.PictureFormat = Android.Graphics.ImageFormatType.Jpeg;
                
                //Calculate the preview size
                _SupportedPreviewSizes = p.SupportedPreviewSizes;
                if (_SupportedPreviewSizes != null)
                {
                    _PreviewSize = GetOptimalPreviewSize(_SupportedPreviewSizes, 640, 480); 
                    p.SetPreviewSize(_PreviewSize.Width, _PreviewSize.Height);
                }
                else
                {
                    p.SetPreviewSize(640, 480);
                }
            
                _layoutParamsControl
                = new Android.Widget.RelativeLayout.LayoutParams(_surfaceView.LayoutParameters.Width,
                    _surfaceView.LayoutParameters.Height);

                if (_cameraControlView.Parent != null)
                {
                    ((ViewGroup)_cameraControlView.Parent).RemoveView(_cameraControlView);
                }
                this.AddContentView(_cameraControlView, _layoutParamsControl);

                Button buttonTakePicture = (Button)FindViewById(Resource.Id.takepicture);
                Helper.SetTypefaceForButton(buttonTakePicture, FontManager.cnButtonTypeface, FontManager.cnButtonTypefaceSizeSp);
                buttonTakePicture.SetTextColor(Resources.GetColor(Resource.Color.civicsmart_black));
                buttonTakePicture.Click += ButtonTakePicture_Click;

                ImageView loRectImageView = (ImageView)FindViewById(Resource.Id.rectimage);
                if (loRectImageView != null)
                {                    
                    _scaleFactor = (float)loRectImageView.LayoutParameters.Height / (float)_surfaceView.Height;
                    _scaleFactorW = (float)loRectImageView.LayoutParameters.Width / (float)_surfaceView.Width;                
                }
                else
                {
                    _scaleFactor = (float)ConvertDpToPixel (120) / (float)_surfaceView.Height;
					_scaleFactorW = (float)ConvertDpToPixel (200) / (float)_surfaceView.Height;
                }

                _camera.SetParameters(p);
            }
            catch (Exception exp)
            {
                System.Console.WriteLine("Exception source {0}: {1}", exp.Source, exp.ToString());
                HandleLPRException(exp);
            }


        }

        #if !_USE_NEW_FOCUS_
        public void SurfaceDestroyed(ISurfaceHolder holder)
        {            
            _camera.StopPreview();
            _camera.Release();
            _camera = null;
            _previewing = false;
        }
		#else
		public void SurfaceDestroyed (ISurfaceHolder holder)
		{
			StopCameraPreview ();
		}

		public void StopCameraPreview ()
		{
			if (_camera != null) {
				try {
					_previewing = false;
					_camera.CancelAutoFocus();
					_camera.StopPreview();
					_camera.Release();
                    _camera.Dispose();
				} catch (Exception exp) {
                    HandleLPRException(exp);
				}
			}
		}
		#endif

		
        #endregion

        #region Methods

        public byte[] GetBitmapData(Bitmap bitmap)
        {
            try
            {
                byte[] bitmapData;
                using (var stream = new System.IO.MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, _compressRatio, stream);
                    bitmapData = stream.ToArray();
                }
                return bitmapData;
            }
            catch (Exception exp)
            {
                HandleLPRException(exp);
                return null;
            }
        }

        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }

        private int ConvertDpToPixel(int dp)
        {
			var pixels = (int)(dp * Resources.DisplayMetrics.Density);
            return pixels;
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
    

        private Android.Hardware.Camera.Size GetOptimalPreviewSize(IList<Android.Hardware.Camera.Size> sizes, int w, int h)
        {
            const double ASPECT_TOLERANCE = 0.1;
            double targetRatio = (double)w / h;

            if (sizes == null)
                return null;

            Android.Hardware.Camera.Size optimalSize = null;
            double minDiff = Double.MaxValue;

            int targetHeight = h;

            // Try to find an size match aspect ratio and size
            foreach (Android.Hardware.Camera.Size size in sizes)
            {
                double ratio = (double)size.Width / size.Height;

                if (Math.Abs(ratio - targetRatio) > ASPECT_TOLERANCE)
                    continue;

                if (Math.Abs(size.Height - targetHeight) < minDiff)
                {
                    optimalSize = size;
                    minDiff = Math.Abs(size.Height - targetHeight);
                }
            }

            // Cannot find the one match the aspect ratio, ignore the requirement
            if (optimalSize == null)
            {
                minDiff = Double.MaxValue;
                foreach (Android.Hardware.Camera.Size size in sizes)
                {
                    if (Math.Abs(size.Height - targetHeight) < minDiff)
                    {
                        optimalSize = size;
                        minDiff = Math.Abs(size.Height - targetHeight);
                    }
                }
            }

            return optimalSize;
        }

		#if !_USE_NEW_FOCUS_
		public void OnAutoFocus(bool success, Android.Hardware.Camera camera)
		{
			_camera.CancelAutoFocus ();
			var parms = _camera.GetParameters ();
			if (parms.FocusMode != Android.Hardware.Camera.Parameters.FocusModeContinuousPicture) {
				parms.FocusMode = Android.Hardware.Camera.Parameters.FocusModeContinuousPicture;
				camera.SetParameters (parms);
			}
		}
		#else
		public void OnAutoFocus (bool success, Android.Hardware.Camera camera)
		{
            try
            {
                var parameters = camera.GetParameters();
                parameters.FocusMode = Android.Hardware.Camera.Parameters.FocusModeContinuousPicture;
                if (parameters.MaxNumFocusAreas > 0)
                {
                    parameters.FocusAreas = null;
                }
                camera.SetParameters(parameters);
            }
            catch (Exception exp)
            {
                HandleLPRException(exp);
            }
		}
		#endif

#if _LocalWebService        
        public string PostImageToApiRESTSHARP(Bitmap bitmap)
        {

            byte[] bitmapData = GetBitmapData(bitmap);

            RestSharp.RestClient client = new RestSharp.RestClient(_uploadPath);

            RestSharp.RestRequest request = new RestSharp.RestRequest(_uploadPath, RestSharp.Method.POST);



            request.AlwaysMultipartFormData = true;


            //request.AddFileBytes (App._file.Name, System.IO.File.ReadAllBytes (App._file.Path), App._file.Name, "application/x-www-form-urlencoded");

            //request.AddFileBytes ("data", bitmapData, App._file.Name, "multipart/form-data");
            request.AddFile("photo", bitmapData, "photo.jpg", "image/jpeg");
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Image-type", "jpg");
            request.AddHeader("Accept", "*/*");
        
            request.ReadWriteTimeout = int.MaxValue;
            client.Timeout = int.MaxValue;
            var response = client.Execute(request);
            return response.Content.ToString();

            //var fileContent = new System.Net.Http.ByteArrayContent (bitmapData);

            //			System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient ();		
            //
            //			System.Net.Http.MultipartFormDataContent multipartContent = new System.Net.Http.MultipartFormDataContent ();
            //			multipartContent.Add (fileContent);
            //
            //			multipartContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse ("multipart/form-data");
            //			multipartContent.Headers.Add ("Image-type", "jpg");
            //
            //			fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue ("form-data") {
            //				Name = "file",
            //				FileName = App._file.Name
            //			};
            //
            //			var responseMessage = httpClient.PostAsync (_uploadPath, multipartContent).Result;
            //			var stringResult = responseMessage.Content.ReadAsStringAsync ().Result;
            //			return stringResult; 
        }

        public string PostImageToApi(Bitmap bitmap)
        {

            byte[] bitmapData = GetBitmapData(bitmap);

            var fileContent = new System.Net.Http.ByteArrayContent(bitmapData);

            System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();

            System.Net.Http.ByteArrayContent byteArrayContent = new System.Net.Http.ByteArrayContent(bitmapData);
            byteArrayContent.Headers.Add("Image-type", "jpg");

            var responseMessage = httpClient.PostAsync(_uploadPath, byteArrayContent).Result;
            var stringResult = responseMessage.Content.ReadAsStringAsync().Result;
            return stringResult;
            //request.Content = 
        }
        
#endif

        #endregion

    }

}
