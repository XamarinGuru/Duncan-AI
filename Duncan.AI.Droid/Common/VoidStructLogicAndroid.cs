
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Drawing.Drawing2D;
using System.Text;
using System.IO;
//using System.Windows.Forms;
using System.Text.RegularExpressions;
using Reino.ClientConfig;
using Reino.CommonLogic;
using System.Runtime.InteropServices;
using ReinoControls;

namespace Duncan.AI.Droid.Common
{
    public class VoidStructLogicAndroid : DetailStructLogicAndroid
    {
        public int SaveVoidStructRec(int iVoidedPrimaryKey, TTTable iVoidedTable, string iVoidReason)
        {
#if _original_
            // Get references to data tables we will be using
            TTTable fTable = this.IssueStruct.MainTable.HighTableRevision.Tables[0];
            TTTable UserTable = IssueAppImp.GlobalUserStruct.MainTable.HighTableRevision.Tables[0];

            // Copy all the fields in common from the source record.
            CopySourceValuesFromTable(iVoidedTable);

            // Set the master key that links the original ticket to the reissue record
            string loPrimaryKeyStr = iVoidedPrimaryKey.ToString();
            fTable.SetFormattedFieldData(FieldNames.MasterKeyFieldName, "-999999999", loPrimaryKeyStr);

            // Set the Date & Time stamp
            fTable.SetFormattedFieldData(FieldNames.OfficerNameFieldName, "", UserTable.GetFormattedFieldData(FieldNames.OfficerNameFieldName, ""));
            fTable.SetFormattedFieldData(FieldNames.OfficerIDFieldName, "", UserTable.GetFormattedFieldData(FieldNames.OfficerIDFieldName, ""));

            ReinoControls.TextBoxBehavior.OSDateToDateString(DateTime.Today, "YYYYMMDD", ref loPrimaryKeyStr);
            fTable.SetFormattedFieldData(FieldNames.RecCreationDateFieldName, "YYYYMMDD", loPrimaryKeyStr);
            ReinoControls.TextBoxBehavior.OSTimeToTimeString(DateTime.Now, "HHMMSS", ref loPrimaryKeyStr);
            fTable.SetFormattedFieldData(FieldNames.RecCreationTimeFieldName, "HHMMSS", loPrimaryKeyStr);
            fTable.SetFormattedFieldData(FieldNames.CancelReasonFieldName, "", iVoidReason);

            return fTable.WriteRecord();
#else
            return 0;
#endif
        }
    }
}
