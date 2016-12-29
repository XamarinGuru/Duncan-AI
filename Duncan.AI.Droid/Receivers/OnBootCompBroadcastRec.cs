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
using Android.Content.PM;
using Android.Views.InputMethods;
using System.Threading.Tasks;
using Android.Net;
using Android.Preferences;

//Android broadcast reciever register to event phone boot(device switch on) completed and internet connected
namespace Duncan.AI.Droid
{
	[BroadcastReceiver]
//	[IntentFilter (new[] {Intent.ActionBootCompleted})]
//	[IntentFilter(new string[]{ "android.net.conn.CONNECTIVITY_CHANGE" })]
	public class OnBootCompBroadcastRec : BroadcastReceiver
	{
		//onReceive mthod runs on main thread, inkokes intent service 
		//runs in a sperate thread.
		public override void OnReceive (Context context, Intent intent)
		{
			//Connectivity manager to get internet information
			var cm = (ConnectivityManager) context.GetSystemService (
				Context.ConnectivityService);
			var activeNetworkInfo = cm.ActiveNetworkInfo;

			//Boot completed and internet connected event handler code
			if (intent.Action == Intent.ActionBootCompleted) {
				Toast.MakeText (context, "Received OnBootCompBroadcastRec!", ToastLength.Long).Show ();

				//IsharedPreferences object like session object in web application
				ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context); 
				ISharedPreferencesEditor editor = prefs.Edit();
				editor.PutString(Constants.INVOKE_LOGIN, "YES");
				editor.Apply();
			} else if (intent.Action == ConnectivityManager.ConnectivityAction) {
				if(activeNetworkInfo.IsConnected){
					Toast.MakeText (context, "Internet Connected", ToastLength.Long).Show ();

					ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context); 
					ISharedPreferencesEditor editor = prefs.Edit();
					string ss = prefs.GetString(Constants.INVOKE_LOGIN, "NO");

					//Invoke users login webservice only one time when device booted not everytime 
					//when internet connects and disconnects
					if (ss.Equals ("YES", StringComparison.Ordinal)) {
						Toast.MakeText (context, "Invoking Web Service", ToastLength.Long).Show ();
						context.StartService (new Intent (context, typeof(StartupService)));
						//Application.Context.UnregisterReceiver (this);
						editor.PutString(Constants.INVOKE_LOGIN, "NO");
						editor.Apply();
					}
				}
			}
		}
	}
}

