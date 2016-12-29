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

namespace Duncan.AI.Droid
{
	[BroadcastReceiver]
	public class GPSTrackingBroadcastRec : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			Toast.MakeText (context, "User Location Tracking Begin", ToastLength.Short).Show ();
//			context.StartService (new Intent (context, typeof(GPSService)));
		}
	}
}

