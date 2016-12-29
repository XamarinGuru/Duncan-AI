using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Duncan.AI.Droid.Utils.HelperManagers;
using Duncan.AI.Droid.Utils;

namespace Duncan.AI.Droid
{

    //
    //Important Note: If you implement a Java interface, you have to inherit from Java.Lang.Object. Do not implement Handle and Dispose() on your own.
    //
    //http://stackoverflow.com/documentation/xamarin.android/771/bindings/18058/implementing-java-interfaces#t=201612150629119746015
    //
    // http://stackoverflow.com/questions/38706366/interface-implement-differences-in-c-sharp-vs-java/38707255#38707255
    //

    public class GPSLocationUpdate : Java.Lang.Object, IJavaObject, IDisposable
    //public class GPSLocationUpdate :  IJavaObject, IDisposable
    {
        private LocationManager _locationManager;
        private Context _context;
        private static Address _lastKnownAddress = null; 

        public GPSLocationUpdate()
        {
        }

        public GPSLocationUpdate(Context context)
        {
            _context = context;
        }
               
        private void HandleExceptions(Exception e)
        {
            LoggingManager.LogApplicationError(e, "GPSLocationUpdate Exception", e.TargetSite.Name);
            ErrorHandling.ReportExceptionWithConfirmationDlg(e.Message);
        }

        //Get last known location
        public bool GetCurrentLocation(ref double latitude, ref double longitude)
        {
            try
            {

                Location location = null;
                //First we will try to get our own last updated current location received by our own GPSService
                location = LocationUpdateListener.GetLastUpdatedLocation();
                if (location == null) //not valid location, go ahead and get the last known one
                {
                    _locationManager = ((Activity)_context).GetSystemService(Context.LocationService) as LocationManager;

                    bool isGPSEnabled = _locationManager.IsProviderEnabled(LocationManager.GpsProvider);
                    bool isNetworkEnabled = _locationManager.IsProviderEnabled(LocationManager.NetworkProvider);

                    if (isGPSEnabled)
                    {
                        location = _locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
                    }
                    else if (isNetworkEnabled)
                    {
                        location = _locationManager.GetLastKnownLocation(LocationManager.NetworkProvider);
                    }
                }

                if (location == null)
                {
                    latitude = 0;
                    longitude = 0;
                    return false;
                }

                latitude = location.Latitude;
                longitude = location.Longitude;
                return true;

            }
            catch (Exception e)
            {
                Log.Error("GPSLocationUpdate Exception", e.Source);
                HandleExceptions(e);
                return false;
            }
        }


        //Get detailed address from last known good Location Latitude & Longitude
        public Address GetCurrentLocationAddress()
        {
            double loLatitude = 0;
            double loLongitude = 0;
            Address loAddress = null;
            if (GetCurrentLocation(ref loLatitude, ref loLongitude))
            {
                loAddress = GetLocationAddress(loLatitude, loLongitude);
            }
            if (loAddress != null)
            {
                _lastKnownAddress = loAddress;
                return loAddress;
            }
            //We failed to get te reverse GeoCode address, then return the last one we found
            return _lastKnownAddress;
        }

        //Get detailed address from passed Location Latitude & Longitude
        public Address GetLocationAddress(double iLatitude, double iLongitude)
        {
            Address loAddress = null;
            try
            {
                Geocoder loGeocoder = new Geocoder(_context);
                IList<Address> loAddresses = loGeocoder.GetFromLocation(iLatitude, iLongitude, 1);
                if (loAddresses != null && loAddresses.Count > 0)
                {
                    loAddress = loAddresses[0];
                }
                return loAddress;
            }

            catch (Java.IO.IOException e)
            {
                //Just log the error and return. No need to pop up error dlg.
                Log.Error("GPSLocationUpdate Exception", e.Source);
                return null;
            }

            catch (Exception e)
            {                
                Log.Error("GPSLocationUpdate Exception", e.Source);
                HandleExceptions(e);
                return null;
            }
        }

        //Get detailed address from last known good Location Latitude & Longitude
        public static Address GetLastUpdatedAddress()
        {
            return _lastKnownAddress;
        }

        public async Task<bool> GetCurrentLocationAddressAsync()
        {
            double loLatitude = 0;
            double loLongitude = 0;
            Address loAddress = null;
            if (GetCurrentLocation(ref loLatitude, ref loLongitude))
            {
                loAddress = await GetLocationAddressAsync(loLatitude, loLongitude);
            }
            if (loAddress != null)
            {
                _lastKnownAddress = loAddress;
                return true;
            }
            //We failed to get te reverse GeoCode address, then return the last one we found
            return true;            
        }

        //Get detailed address from passed Location Latitude & Longitude
        public async Task<Address> GetLocationAddressAsync(double iLatitude, double iLongitude)
        {
            Address loAddress = null;
            try
            {
                Geocoder loGeocoder = new Geocoder(_context);
                IList<Address> loAddresses = await loGeocoder.GetFromLocationAsync(iLatitude, iLongitude, 1);
                if (loAddresses != null && loAddresses.Count > 0)
                {
                    loAddress = loAddresses[0];
                }
                return loAddress;
            }

            catch (Java.IO.IOException e)
            {
                //Just log the error and return. No need to pop up error dlg.
                Log.Error("GPSLocationUpdate Exception", e.Source);
                return null;
            }

            catch (Exception e)
            {
                Log.Error("GPSLocationUpdate Exception", e.Source);
                HandleExceptions(e);
                return null;
            }
        }

        //Important Note: If you implement a Java interface, you have to inherit from Java.Lang.Object. Do not implement Handle and Dispose() on your own.
        //public void Dispose()
        //{
        //}

        //public IntPtr Handle { get; set; }
        
    }

    //Location update listener is the only place where we will get the updated location from the LocationManager. 
    class LocationUpdateListener : Java.Lang.Object, ILocationListener
    {
        private static LocationManager _locationManager = null;        
        private static Location _currentLocation = null;
        private static Context _context = null;
        private static bool _listenerStarted = false;

        public LocationUpdateListener()            
        {
        }

        public LocationUpdateListener(Context context)
        {
            _context = context;    
        }

        public void Start()
        {
            if (!InitLocationUpdateRequest())
            {
                //Throw an error
                ErrorHandling.ThrowError(_context, ErrorHandling.ErrorCode.GPSService, "");
            }
            _listenerStarted = true;
        }

        public void Stop()
        {

            if (_locationManager != null)
            {
                _locationManager.RemoveUpdates(this);
                _listenerStarted = false;
            }
        }

        private void HandleExceptions(Exception e)
        {
            LoggingManager.LogApplicationError(e, "LocationUpdateListener Exception", e.TargetSite.Name);
            ErrorHandling.ReportExceptionWithConfirmationDlg(e.Message);
        }

        public static Location GetLastUpdatedLocation()
        {
            return _currentLocation;
        }
        
        protected Location GetCurrentLocation()
        {
            //If the listener not started, then do and start it now
            if (!_listenerStarted) Start();
            //If the registered listener got an updated location, return it now
            if (_currentLocation != null) return _currentLocation;

            //Not yet a vaild location, try to get last known one            
            try
            {
                if (_locationManager == null)
                {
                    _locationManager = (LocationManager)_context.GetSystemService(Context.LocationService);
                }
                bool isGPSEnabled = _locationManager.IsProviderEnabled(LocationManager.GpsProvider);
                bool isNetworkEnabled = _locationManager.IsProviderEnabled(LocationManager.NetworkProvider);
                Location location = null;

                if (isGPSEnabled)
                {
                    location = _locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
                }
                else if (isNetworkEnabled)
                {
                    location = _locationManager.GetLastKnownLocation(LocationManager.NetworkProvider);
                }
                return location;
            }
            catch (Exception e)
            {
                Log.Error("LocationUpdateListener Exception", e.Source);
                HandleExceptions(e);
                return null;
            }

        }

        protected bool InitLocationUpdateRequest()
        {
            try
            {
                _locationManager = (LocationManager)_context.GetSystemService(Context.LocationService);
                bool isGPSEnabled = _locationManager.IsProviderEnabled(LocationManager.GpsProvider);
                bool isNetworkEnabled = _locationManager.IsProviderEnabled(LocationManager.NetworkProvider);

                if (isGPSEnabled)
                {
                    //If we set minTime to 0, OnLocationChanged will be called once when it first receives a location update,
                    //then it won't be called until we change our position in minDistance meters.
                    _locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 0, 1, this);
                    //Also get the last known location to start with until we get updated one
                    _currentLocation = _locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
                }
                else if (isNetworkEnabled)
                {
                    _locationManager.RequestLocationUpdates(LocationManager.NetworkProvider, 0, 1, this);
                    //Also get the last known location to start with until we get updated one
                    _currentLocation = _locationManager.GetLastKnownLocation(LocationManager.NetworkProvider);
                }
                else
                {
                    return false;
                }
                return true;

            }
            catch (Exception e)
            {
                if (_locationManager != null) _locationManager.RemoveUpdates(this);
                Log.Error("LocationUpdateListener Exception", e.Source);
                HandleExceptions(e);
                return false;
            }
        }
       

        public void OnLocationChanged(Location location)
        {
            if (location != null)
            {
                _currentLocation = location;
            }
        }

        public void OnProviderDisabled(string provider)
        {
            if (_locationManager != null)
            {
                _locationManager.RemoveUpdates(this);
            }
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
        }

        //Important Note: If you implement a Java interface, you have to inherit from Java.Lang.Object. Do not implement Handle and Dispose() on your own.
        //public void Dispose()
        //{
        //    if (_locationManager != null)
        //    {
        //        _locationManager.RemoveUpdates(this);
        //    }
        //}

    }
}

