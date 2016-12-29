using System;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Duncan.AI.Droid.Utils;
using Duncan.AI.Droid.Utils.HelperManagers;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Java.IO;


namespace Duncan.AI.Droid
{
    [Service]
    [IntentFilter(new String[] { "Duncan.AI.Droid.GPSService" })]
    class GPSService : IntentService
    {
        private static bool _recordStarted = false;
        private static ActivityLogDTO _activityLogDTO;

        public GPSService()
            : base("GPSService")
        {
        }

        private void HandleExceptions(Exception e)
        {
            LoggingManager.LogApplicationError(e, "GPSService Exception", e.TargetSite.Name);
            ErrorHandling.ReportExceptionWithConfirmationDlg(e.Message);
        }


        protected void InitActivityFields()
        {
            try
            {

                //We will start new record, first get the current location
                Location loLocation = GetCurrentLocation();
                if (loLocation == null) return; //nothing to fill in the record if no location info

                //We should be ok to init the record            
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                _activityLogDTO.startDate = DateTimeManager.GetDate(null, null, Constants.DT_YYYYMMDD);
                _activityLogDTO.startTime = DateTimeManager.GetTime(null, null, Constants.TIME_HHMMSS);
                _activityLogDTO.officerId = prefs.GetString(Constants.OFFICER_ID, null);
                _activityLogDTO.officername = prefs.GetString(Constants.OFFICER_NAME, null);
                _activityLogDTO.startLatitude = loLocation.Latitude.ToString();
                _activityLogDTO.startLongitude = loLocation.Longitude.ToString();
                _activityLogDTO.primaryActivityName = Constants.LOCATION_UPDATE_ACTIVITY_NAME;
                _activityLogDTO.primaryActivityCount = "0";
                _activityLogDTO.secondaryActivityName = "";
                _activityLogDTO.secondaryActivityCount = "0";
                _recordStarted = true;
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
        }

        protected void EndActivityRecord()
        {
            try
            {
                //We need to complete the record, get the current location
                Location loLocation = GetCurrentLocation();
                if (loLocation != null)
                {
                    _activityLogDTO.endLatitude = loLocation.Latitude.ToString();
                    _activityLogDTO.endLongitude = loLocation.Longitude.ToString();
                }
                else
                {
                    //We failed to get the current location, we still can continue by using the start location
                    _activityLogDTO.endLatitude = _activityLogDTO.startLatitude;
                    _activityLogDTO.endLongitude = _activityLogDTO.startLongitude;
                }

                _activityLogDTO.endDate = DateTimeManager.GetDate(null, null, Constants.DT_YYYYMMDD);
                _activityLogDTO.endTime = DateTimeManager.GetTime(null, null, Constants.TIME_HHMMSS);
                ActivityLogADO.InsertGPSRecord(_activityLogDTO);
                //Now submit the record
                this.StartService(new Intent(this, typeof(SyncService)));
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }


        //onHanleIntent method runs on a new thread. 
        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                if (!_recordStarted)
                {
                    InitActivityFields();
                }
                else
                {
                    //Fill the end fields and submit the record
                    EndActivityRecord();
                    //Init the fields again for next record
                    InitActivityFields();
                }
            }
            catch (Exception e)
            {

                Log.Error("GPSService Exception", e.Source);
                HandleExceptions(e);
            }
        }


        protected Location GetCurrentLocation()
        {
            //If the registered listener got an updated location, return it now
            Location loLocation = LocationUpdateListener.GetLastUpdatedLocation();

            if (loLocation != null) return loLocation;

            //Not yet a vaild location, try to get last known one by system location manager            
            try
            {
                LocationManager loLocationManager = (LocationManager)GetSystemService(LocationService);

                bool isGPSEnabled = loLocationManager.IsProviderEnabled(LocationManager.GpsProvider);
                bool isNetworkEnabled = loLocationManager.IsProviderEnabled(LocationManager.NetworkProvider);
                Location location = null;

                if (isGPSEnabled)
                {
                    location = loLocationManager.GetLastKnownLocation(LocationManager.GpsProvider);
                }
                else if (isNetworkEnabled)
                {
                    location = loLocationManager.GetLastKnownLocation(LocationManager.NetworkProvider);
                }
                return location;
            }
            catch (Exception e)
            {
                Log.Error("GPSService Exception", e.Source);
                HandleExceptions(e);
                return null;
            }

        }


        public override void OnStart(Intent intent, int startId)
        {
            base.OnStart(intent, startId);
        }

    }
}

