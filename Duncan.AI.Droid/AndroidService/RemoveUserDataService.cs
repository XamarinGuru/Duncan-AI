using System;
using System.Collections;
using System.Text;
using Android.App;
using Android.Content;
using Android.Util;
using System.Collections.Generic;
using Duncan.AI.Droid.Utils.HelperManagers;
using Duncan.AI.Droid.Utils;

namespace Duncan.AI.Droid
{
	[Service]
	[IntentFilter(new String[]{"Duncan.AI.Droid.RemoveUserDataService"})]
	class RemoveUserDataService : IntentService
	{
		public RemoveUserDataService () : base("RemoveUserDataService")
		{
		}

        private void HandleExceptions(Exception e)
        {
            LoggingManager.LogApplicationError(e, "RemoveUserDataService Exception", e.TargetSite.Name);
            ErrorHandling.ReportExceptionWithConfirmationDlg(e.Message);
        }

		//onHanleIntent method runs on a new thread. 
		protected override void OnHandleIntent (Intent intent)
		{
			Log.Debug ("RemoveUserDataService::::", "OnHandleIntent");
			try{
				var queryStringList = new ArrayList ();
				
                List<XMLConfig.IssStruct> structs = DroidContext.XmlCfg.IssStructs;
                foreach (var structI in structs)
                {
                    var deleteRowsStrBldr = new StringBuilder();
                    deleteRowsStrBldr.Append("DELETE FROM ");
                    deleteRowsStrBldr.Append(structI.Name);
                    deleteRowsStrBldr.Append(" ;");
                    queryStringList.Add(deleteRowsStrBldr.ToString());                    
                }			

				foreach (string command in queryStringList) {
					bool isSucc = CommonADO.ExecuteQuery (command);
					Log.Info ("Query Result::::", isSucc.ToString());
				}

			}catch(Exception e){
				Console.WriteLine("Exception source: {0}", e.Source);
                HandleExceptions(e);
			}
		}
	}
}

