using System;
using System.Text;
using Android.Util;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android.Content;
using Android.Preferences;
using Duncan.AI.Droid.Utils.HelperManagers;

namespace Duncan.AI.Droid
{
    class CiteADO
    {
        public CiteADO()
        {
        }

        //Update Row into the database 
        public static string UpdateCiteTable(string rowId, string tableName)
        {
            var updateStrBldr = new StringBuilder();
            updateStrBldr.Append("UPDATE ");
            updateStrBldr.Append(tableName);
            updateStrBldr.Append(" SET ");
            updateStrBldr.Append(Constants.WS_STATUS_COLUMN);
            updateStrBldr.Append(" = '");
            updateStrBldr.Append(Constants.WS_STATUS_SUCCESS);
            updateStrBldr.Append("' ,");
            updateStrBldr.Append(Constants.STATUS_COLUMN);
            updateStrBldr.Append(" = '");
            updateStrBldr.Append(Constants.STATUS_ISSUED);
            updateStrBldr.Append("' WHERE ");
            updateStrBldr.Append(Constants.ID_COLUMN);
            updateStrBldr.Append(" = '");
            updateStrBldr.Append(rowId);
            updateStrBldr.Append("'");

            CommonADO.ExecuteQuery(updateStrBldr.ToString());
            return updateStrBldr.ToString();
        }
    }
}