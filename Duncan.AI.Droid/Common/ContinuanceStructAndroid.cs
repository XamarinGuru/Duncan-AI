
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
    public class ContinuanceStructLogicAndroid : DetailStructLogicAndroid
    {
        public int SaveContinuanceStructRec(string iSrcIssueNo, string iSrcIssueNoPfx,
            string iSrcIssueNoSfx, TTTable iContinuancedTable)
        {

#if _original_
            // Get references to data tables we will be using
            TTTable fTable = this.IssueStruct.MainTable.HighTableRevision.Tables[0];
            TTTable UserTable = IssueAppImp.GlobalUserStruct.MainTable.HighTableRevision.Tables[0];

            // copy all the fields in common from the source record.
            CopySourceValuesFromTable(iContinuancedTable);

            // set the master key that links the original ticket to the reissue record
            string loContinuancedPrimaryKeyStr = (iContinuancedTable.GetRecCount() - 1).ToString();
            fTable.SetFormattedFieldData(FieldNames.MasterKeyFieldName, "-999999999", loContinuancedPrimaryKeyStr);

            // set the SrcIssueNo, Date & Time stamp
            fTable.SetFormattedFieldData(FieldNames.SrcIssueNoFieldName, "", iSrcIssueNo);
            fTable.SetFormattedFieldData(FieldNames.SrcIssueNoPfxFieldName, "", iSrcIssueNoPfx);
            fTable.SetFormattedFieldData(FieldNames.SrcIssueNoSfxFieldName, "", iSrcIssueNoSfx);

            fTable.SetFormattedFieldData(FieldNames.OfficerNameFieldName, "", UserTable.GetFormattedFieldData(FieldNames.OfficerNameFieldName, ""));
            fTable.SetFormattedFieldData(FieldNames.OfficerIDFieldName, "", UserTable.GetFormattedFieldData(FieldNames.OfficerIDFieldName, ""));

            ReinoControls.TextBoxBehavior.OSDateToDateString(DateTime.Today, "YYYYMMDD", ref loContinuancedPrimaryKeyStr);
            fTable.SetFormattedFieldData(FieldNames.RecCreationDateFieldName, "YYYYMMDD", loContinuancedPrimaryKeyStr);
            ReinoControls.TextBoxBehavior.OSTimeToTimeString(DateTime.Now, "HHMMSS", ref loContinuancedPrimaryKeyStr);
            fTable.SetFormattedFieldData(FieldNames.RecCreationTimeFieldName, "HHMMSS", loContinuancedPrimaryKeyStr);

            return fTable.WriteRecord();
#else
             return 0;
#endif
        }
    }
}
