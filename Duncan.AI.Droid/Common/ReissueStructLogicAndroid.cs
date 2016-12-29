

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
    public class ReissueStructLogicAndroid : DetailStructLogicAndroid
    {
        public int SaveReissueStructRec(int iReissuedPrimaryKey, TTTable iReissuedTable)
        {
#if _original_
            // Get references to data tables we will be using
            TTTable fTable = this.IssueStruct.MainTable.HighTableRevision.Tables[0];
            TTTable UserTable = IssueAppImp.GlobalUserStruct.MainTable.HighTableRevision.Tables[0];

            // Copy all the fields in common from the source record.
            CopySourceValuesFromTable(iReissuedTable);

            // Set the master key that links the original ticket to the reissue record
            string loReissuedPrimaryKeyStr = iReissuedPrimaryKey.ToString();
            fTable.SetFormattedFieldData(FieldNames.MasterKeyFieldName, "-999999999", loReissuedPrimaryKeyStr);

            // Set the SrcIssueNo, Date & Time stamp
            fTable.SetFormattedFieldData(FieldNames.SrcIssueNoFieldName, "", iReissuedTable.GetFormattedFieldData(FieldNames.IssueNoFieldName, ""));
            fTable.SetFormattedFieldData(FieldNames.SrcIssueNoPfxFieldName, "", iReissuedTable.GetFormattedFieldData(FieldNames.IssueNoPfxFieldName, ""));
            fTable.SetFormattedFieldData(FieldNames.SrcIssueNoSfxFieldName, "", iReissuedTable.GetFormattedFieldData(FieldNames.IssueNoSfxFieldName, ""));

            fTable.SetFormattedFieldData(FieldNames.OfficerNameFieldName, "", UserTable.GetFormattedFieldData(FieldNames.OfficerNameFieldName, ""));
            fTable.SetFormattedFieldData(FieldNames.OfficerIDFieldName, "", UserTable.GetFormattedFieldData(FieldNames.OfficerIDFieldName, ""));

            ReinoControls.TextBoxBehavior.OSDateToDateString(DateTime.Today, "YYYYMMDD", ref loReissuedPrimaryKeyStr);
            fTable.SetFormattedFieldData(FieldNames.RecCreationDateFieldName, "YYYYMMDD", loReissuedPrimaryKeyStr);
            ReinoControls.TextBoxBehavior.OSTimeToTimeString(DateTime.Now, "HHMMSS", ref loReissuedPrimaryKeyStr);
            fTable.SetFormattedFieldData(FieldNames.RecCreationTimeFieldName, "HHMMSS", loReissuedPrimaryKeyStr);

            return fTable.WriteRecord();
#else
            return 0;
#endif
        }
    }
}
