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
using Android.Preferences;

namespace Duncan.AI.Droid
{
	[BroadcastReceiver]
	[IntentFilter(new String[]{"Duncan.AI.Droid.AlarmBroadcastRec"})]
	public class AlarmBroadcastRec : BroadcastReceiver
	{
		long REPEAT_TIME_SYNC = 1000 * 10; //10 Seconds

		long REPEAT_TIME_GPS = 1000 * 15; //15 Seconds

		public override void OnReceive (Context context, Intent intent)
		{
//			AlarmManager alarmSync = (AlarmManager)context.GetSystemService (Context.AlarmService);
//			Intent intentSyncBroad = new Intent (context, typeof (SyncDataBroadcastRec));
//			PendingIntent pendingServiceIntentSync = PendingIntent.GetBroadcast (context, 0, intentSyncBroad, PendingIntentFlags.CancelCurrent);
//			alarmSync.SetRepeating (AlarmType.Rtc, REPEAT_TIME_SYNC, REPEAT_TIME_SYNC, pendingServiceIntentSync);

//			Toast.MakeText (context, "User Location Tracking Invoked!", ToastLength.Short).Show ();
//			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context); 
//			ISharedPreferencesEditor editor = prefs.Edit();
//			string isGps = prefs.GetString(Constants.IS_GPS, Constants.invalid);
//			if ("true".Equals(isGps)) {
//				AlarmManager alarmGPS = (AlarmManager)context.GetSystemService (Context.AlarmService);
//				Intent intentGPSBroad = new Intent (context, typeof (GPSTrackingBroadcastRec));
//				PendingIntent pendingServiceIntentGPS = PendingIntent.GetBroadcast (context, 0, intentGPSBroad, PendingIntentFlags.CancelCurrent);
//				alarmGPS.SetRepeating (AlarmType.Rtc, 1000 * 60, 1000 * 60 * 20, pendingServiceIntentGPS);
//			}
		}
	}
}

