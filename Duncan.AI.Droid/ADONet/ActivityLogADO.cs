using System;
using System.Text;
using Duncan.AI.Droid.Utils.HelperManagers;
using AutoISSUE;

namespace Duncan.AI.Droid
{
	public class ActivityLogADO
	{
		public ActivityLogADO ()
		{
		}

		//Get All Activity log start and end date is not null
		public static string InsertGPSRecord(ActivityLogDTO activityLogDTO)
		{
			try
			{
				var instRowStrBldr = new StringBuilder();
				instRowStrBldr.Append ("INSERT INTO ");
                instRowStrBldr.Append(Constants.STRUCT_NAME_ACTIVITYLOG);
				instRowStrBldr.Append (" ( ");
				instRowStrBldr.Append (DBConstants.sqlStartDateStr);
				instRowStrBldr.Append (" , ");
				instRowStrBldr.Append (DBConstants.sqlStartTimeStr);
				instRowStrBldr.Append (" , ");
				instRowStrBldr.Append (DBConstants.sqlEndDateStr);
				instRowStrBldr.Append (" , ");
				instRowStrBldr.Append (DBConstants.sqlEndTimeStr);
				instRowStrBldr.Append (" , ");
				instRowStrBldr.Append (DBConstants.sqlIssueOfficerIDStr);
				instRowStrBldr.Append (" , ");
				instRowStrBldr.Append (DBConstants.sqlIssueOfficerNameStr);
				instRowStrBldr.Append (" , ");
				instRowStrBldr.Append (DBConstants.sqlStartLatitudeStr);
				instRowStrBldr.Append (" , ");
				instRowStrBldr.Append (DBConstants.sqlStartLongituteStr);
				instRowStrBldr.Append (" , ");
				instRowStrBldr.Append (DBConstants.sqlEndLatitudeStr);
				instRowStrBldr.Append (" , ");
				instRowStrBldr.Append (DBConstants.sqlEndLongitudeStr);
				instRowStrBldr.Append (" , ");
				instRowStrBldr.Append (DBConstants.sqlPrimaryActivityNameStr);
                instRowStrBldr.Append(" , ");
                instRowStrBldr.Append(DBConstants.sqlPrimaryActivityCountStr);
                instRowStrBldr.Append(" , ");
                instRowStrBldr.Append(DBConstants.sqlSecondaryActivityNameStr); 
                instRowStrBldr.Append(" , ");
                instRowStrBldr.Append(DBConstants.sqlSecondaryActivityCountStr);
                instRowStrBldr.Append(" , ");
                instRowStrBldr.Append(Constants.WS_STATUS_COLUMN);
				instRowStrBldr.Append (" ) VALUES ( '");
				instRowStrBldr.Append (activityLogDTO.startDate);
				instRowStrBldr.Append ("', '");
				instRowStrBldr.Append (activityLogDTO.startTime);
				instRowStrBldr.Append ("', '");
				instRowStrBldr.Append (activityLogDTO.startDate);
				instRowStrBldr.Append ("', '");
				instRowStrBldr.Append (activityLogDTO.startTime);
				instRowStrBldr.Append ("', '");
				instRowStrBldr.Append (activityLogDTO.officerId);
				instRowStrBldr.Append ("', '");
				instRowStrBldr.Append (activityLogDTO.officername);
				instRowStrBldr.Append ("', '");
				instRowStrBldr.Append (activityLogDTO.startLatitude);
				instRowStrBldr.Append ("', '");
				instRowStrBldr.Append (activityLogDTO.startLongitude);
				instRowStrBldr.Append ("', '");
				instRowStrBldr.Append (activityLogDTO.endLatitude);
				instRowStrBldr.Append ("', '");
				instRowStrBldr.Append (activityLogDTO.endLongitude);
				instRowStrBldr.Append ("', '");
                instRowStrBldr.Append(activityLogDTO.primaryActivityName);
                instRowStrBldr.Append("', '");
                instRowStrBldr.Append(activityLogDTO.primaryActivityCount);
                instRowStrBldr.Append("', '");
                instRowStrBldr.Append(activityLogDTO.secondaryActivityName);
                instRowStrBldr.Append("', '");
                instRowStrBldr.Append(activityLogDTO.secondaryActivityCount);
                instRowStrBldr.Append("', '");
                instRowStrBldr.Append (Constants.STATUS_READY);
                instRowStrBldr.Append("');");

				bool isSucc = CommonADO.ExecuteQuery (instRowStrBldr.ToString ());
			}catch (Exception e){
                LoggingManager.LogApplicationError(e, null, "InsertGPSRecord");
				Console.WriteLine("Exception source: {0}", e.Source);
			}

			return null;
		}
	}
}

