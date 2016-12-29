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
	[IntentFilter(new String[]{"Duncan.AI.Droid.ListInsertedBroadcastRec"})]
	public class ListInsertedBroadcastRec : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			string msg = intent.GetStringExtra ("Status");

			// AJW - deferred in favor of static progress test...  Toast.MakeText (context, msg, ToastLength.Short).Show ();

            // AJW TODO these are touch points to update progress bar just before/after this call in calling activity code
            
		}
	}
}

