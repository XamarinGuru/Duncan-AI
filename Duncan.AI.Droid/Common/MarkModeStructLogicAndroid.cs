
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
    public class MarkModeStructLogicAndroid : SearchStructLogicAndroid
    {
        private void GetChalkTime(string iMask, ref string oChalkTime)
        {

            TSearchMatchForm loMatchForm;
            TTTable loTable = null;

            // Init result string 
            oChalkTime = "";

            //// get the match form 
            //if ((loMatchForm = SearchMatchResultsForm) == null) return;

            //// Get the table for easier access 
            //loTable = GetMatchRecsTable();
            //// We can't get the field if there is no table to look at
            //if (loTable == null)
            //    return;

            //loTable.GetFormattedFieldData(FieldNames.IssueTimeFieldName, iMask, ref oChalkTime);

            fFormatInfo.fCurrentRecordFormattingTable.GetFormattedFieldData(FieldNames.IssueTimeFieldName, iMask, ref oChalkTime);
        }

        private void GetChalkDate(string iMask, ref string oChalkDate)
        {
            TSearchMatchForm loMatchForm;
            TTTable loTable = null;

            // Init result string 
            oChalkDate = "";

            //// get the match form 
            //if ((loMatchForm = SearchMatchResultsForm) == null) return;

            //// get the table for easier access 
            //loTable = GetMatchRecsTable();
            //// We can't get the field if there is no table to look at
            //if (loTable == null)
            //    return;

            //loTable.GetFormattedFieldData(FieldNames.IssueDateFieldName, iMask, ref oChalkDate);

            fFormatInfo.fCurrentRecordFormattingTable.GetFormattedFieldData(FieldNames.IssueDateFieldName, iMask, ref oChalkDate);
        }

        public int GetElapsedTimeAsMinutes()
        {
            int loElapsedTimeAsMinutes = 0;
            int loJustHours = 0;
            int loJustMinutes = 0;


            string loElapsedTimeHHMM = fFormatInfo.fCurrentRecordFormattingTable.GetFormattedFieldData(FieldNames.ElapsedTimeFieldName, "hh:mm" );
            string[] loParts = loElapsedTimeHHMM.Split(':');
            if (loParts.Length > 0)
            {
                Int32.TryParse(loParts[0], out loJustHours);
            }

            if ( loParts.Length > 1)
            {
                Int32.TryParse(loParts[1], out loJustMinutes);
            }

            loElapsedTimeAsMinutes = (loJustHours*60) + loJustMinutes;

            return loElapsedTimeAsMinutes;
        }





		public enum TMarkModeHitData
		{
			mmhPlate = 0,
			mmhState,
			mmhMarkedAtPhrase,
            mmhLocationText,
            mmhMarkOriginalTime,
            mmhMarkXofYPhrase,
            mmhMarkXofYInversePhrase,
            mmhMarkElapsedTime

		}

        public void GetMarkModeTextForSearchMatchListDisplay(TMarkModeHitData iMarkModeTextType, ref string oMarkModeLine, DataRow iSearchResultRow, int iSearchResultIndex)
        {

            string loTmpStr = "";
            // Init result string 
            oMarkModeLine = "";

            StringBuilder RemarkSB = new StringBuilder();

            switch (iMarkModeTextType)
            {
                case TMarkModeHitData.mmhMarkedAtPhrase:
                    {
                        RemarkSB.Append("Marked ");

                        //iSearchResultIndex++; // zero based
                        //RemarkSB.Append(iSearchResultIndex.ToString());
                        ////RemarkSB.Append(fSearchAndIssueResult.SearchResultSelectedRowIndex);

                        //RemarkSB.Append(" of ");
                        //RemarkSB.Append(fSearchAndIssueResult.fSearchResultDTOList.Count.ToString());
                        //RemarkSB.Append(" ");
                        RemarkSB.Append(fFormatInfo.fCurrentRecordFormattingTable.GetFormattedFieldData(FieldNames.IssueDateFieldName, "mm/dd/yyyy"));
                        RemarkSB.Append(" at ");
                        RemarkSB.Append(fFormatInfo.fCurrentRecordFormattingTable.GetFormattedFieldData(FieldNames.IssueTimeFieldName, "hh:mm"));

                        //RemarkSB.Append("  elapsed: ");
                        //RemarkSB.Append(fFormatInfo.fCurrentRecordFormattingTable.GetFormattedFieldData(FieldNames.ElapsedTimeFieldName, "dd hh:mm"));

                        // Now stuff StringBuilder value into result string in one shot
                        oMarkModeLine = RemarkSB.ToString();
                        return;
                    }

                case TMarkModeHitData.mmhLocationText :
                    {
                        // Build result string with StringBuilder 
                        //RemarkSB.Append("Loc: ");
                        RemarkSB.Append(fFormatInfo.fCurrentRecordFormattingTable.GetFormattedFieldData(FieldNames.LocationFieldName, ""));

                        // test code - make it long enough to test ellipsize
                       // RemarkSB.Append("fFormatInfo fCurrentRecordFormattingTable GetFormattedFieldData ( FieldNames LocationFieldName" );


                        // Now stuff StringBuilder value into result string in one shot
                        oMarkModeLine = RemarkSB.ToString();
                        return;
                    }

                case TMarkModeHitData.mmhMarkXofYPhrase:
                    {
                        // Build result string with StringBuilder 
                        RemarkSB.Append("Marked ");

                        iSearchResultIndex++; // zero based
                        RemarkSB.Append(iSearchResultIndex.ToString());
                        //RemarkSB.Append(fSearchAndIssueResult.SearchResultSelectedRowIndex);

                        RemarkSB.Append(" of ");
                        RemarkSB.Append(fSearchAndIssueResult.fSearchResultDTOList.Count.ToString());

                        // Now stuff StringBuilder value into result string in one shot
                        oMarkModeLine = RemarkSB.ToString();
                        return;
                    }


                case TMarkModeHitData.mmhMarkXofYInversePhrase:
                    {
                        // Build result string with StringBuilder 
                        RemarkSB.Append("Marked ");

                        // the inversed counter
                        iSearchResultIndex = ( fSearchAndIssueResult.fSearchResultDTOList.Count - iSearchResultIndex );


                        RemarkSB.Append(iSearchResultIndex.ToString());
                        //RemarkSB.Append(fSearchAndIssueResult.SearchResultSelectedRowIndex);

                        RemarkSB.Append(" of ");
                        RemarkSB.Append(fSearchAndIssueResult.fSearchResultDTOList.Count.ToString());

                        // Now stuff StringBuilder value into result string in one shot
                        oMarkModeLine = RemarkSB.ToString();
                        return;
                    }
                    

                case TMarkModeHitData.mmhMarkOriginalTime:
                    {
                        RemarkSB.Append(fFormatInfo.fCurrentRecordFormattingTable.GetFormattedFieldData(FieldNames.IssueTimeFieldName, "hh:mm tt"));

                        // Now stuff StringBuilder value into result string in one shot
                        oMarkModeLine = RemarkSB.ToString();
                        return;
                    }

                case TMarkModeHitData.mmhMarkElapsedTime:
                    {
                        RemarkSB.Append(fFormatInfo.fCurrentRecordFormattingTable.GetFormattedFieldData(FieldNames.ElapsedTimeFieldName, "dd hh:mm"));

                        // Now stuff StringBuilder value into result string in one shot
                        oMarkModeLine = RemarkSB.ToString();
                        return;
                    }

            }


#if _original_
            TSearchMatchForm loMatchForm;
            TTTable loTable = null;
            string loTmpStr = "";
            // Init result string 
            oMMRemark = "";

            // get the match form 
            if ((loMatchForm = MatchForm) == null) return;

            // get the table for easier access 
            loTable = GetMatchRecsTable();
            // We can't get the field if there is no table to look at
            if (loTable == null)
                return;

            TextBoxBehavior MatchRecsBehavior = ((ReinoVirtualListBox)(MatchRecs.WinCtrl)).Behavior;
            StringBuilder RemarkSB = new StringBuilder();

            switch (iRemarkNo)
            {
                case 1:
                    // "Marked 1 of 5 at 03:00, elapsed 2:20" 
                    // Build result string with StringBuilder 
                    loTmpStr = Convert.ToString(MatchRecsBehavior.GetListNdx() + 1);

                    RemarkSB.Append("Marked ");
                    RemarkSB.Append(loTmpStr);
                    loTmpStr = Convert.ToString(loTable.GetRecCount());
                    RemarkSB.Append(" of ");
                    RemarkSB.Append(loTmpStr);
                    RemarkSB.Append(" at ");
                    RemarkSB.Append(loTable.GetFormattedFieldData(FieldNames.IssueTimeFieldName, "hh:mm"));
                    RemarkSB.Append(", elapsed: ");
                    RemarkSB.Append(loTable.GetFormattedFieldData(FieldNames.ElapsedTimeFieldName, "dd hh:mm"));
                    // Now stuff StringBuilder value into result string in one shot
                    oMMRemark = RemarkSB.ToString();
                    return;
                case 2:
                    // Build result string with StringBuilder 
                    RemarkSB.Append("Marked Loc: ");
                    RemarkSB.Append(loTable.GetFormattedFieldData(FieldNames.LocationFieldName, ""));
                    // Now stuff StringBuilder value into result string in one shot
                    oMMRemark = RemarkSB.ToString();
                    return;
            }
#endif
        }


        public void GetMMRemark(short iRemarkNo, ref string oMMRemark, DataRow iSearchResultRow, int iSearchResultIndex )
        {

            string loTmpStr = "";
            // Init result string 
            oMMRemark = "";

            StringBuilder RemarkSB = new StringBuilder();

            switch (iRemarkNo)
            {
                case 1:
                    // "Marked 1 of 5 at 03:00, elapsed 2:20" 
                    // Build result string with StringBuilder 

                    //loTmpStr = Convert.ToString(MatchRecsBehavior.GetListNdx() + 1);

                    //RemarkSB.Append("Marked X of Y" );

                    //if (GetFieldValueFromIssueSourceTable(iSearchResultRow, AutoISSUE.DBConstants.sqlIssueTimeStr, ref loTmpStr) == true)
                    //{
                    //    RemarkSB.Append(" at " + loTmpStr + " elapsed dd hh MM" );
                    //}

                    RemarkSB.Append("Marked ");

                    iSearchResultIndex++; // zero based
                    RemarkSB.Append(iSearchResultIndex.ToString());
                    //RemarkSB.Append(fSearchAndIssueResult.SearchResultSelectedRowIndex);

                    RemarkSB.Append(" of ");
                    RemarkSB.Append(fSearchAndIssueResult.fSearchResultDTOList.Count.ToString());
                    RemarkSB.Append(" at ");
                    RemarkSB.Append( fFormatInfo.fCurrentRecordFormattingTable.GetFormattedFieldData(FieldNames.IssueTimeFieldName, "hh:mm"));
                    // AutoCITE-231 - commas cause issues in CSV export. We have simply replaced the "," with " " so any column based parsing routines will be unaffected
                    //RemarkSB.Append(", elapsed: ");
                    RemarkSB.Append("  elapsed: ");
                    RemarkSB.Append(fFormatInfo.fCurrentRecordFormattingTable.GetFormattedFieldData(FieldNames.ElapsedTimeFieldName, "dd hh:mm"));

                    // Now stuff StringBuilder value into result string in one shot
                    oMMRemark = RemarkSB.ToString();
                    return;

                case 2:
                    // Build result string with StringBuilder 
                    RemarkSB.Append("Marked Loc: ");
                    RemarkSB.Append(fFormatInfo.fCurrentRecordFormattingTable.GetFormattedFieldData(FieldNames.LocationFieldName, ""));

                    // Now stuff StringBuilder value into result string in one shot
                    oMMRemark = RemarkSB.ToString();
                    return;
            }


#if _original_
            TSearchMatchForm loMatchForm;
            TTTable loTable = null;
            string loTmpStr = "";
            // Init result string 
            oMMRemark = "";

            // get the match form 
            if ((loMatchForm = MatchForm) == null) return;

            // get the table for easier access 
            loTable = GetMatchRecsTable();
            // We can't get the field if there is no table to look at
            if (loTable == null)
                return;

            TextBoxBehavior MatchRecsBehavior = ((ReinoVirtualListBox)(MatchRecs.WinCtrl)).Behavior;
            StringBuilder RemarkSB = new StringBuilder();

            switch (iRemarkNo)
            {
                case 1:
                    // "Marked 1 of 5 at 03:00, elapsed 2:20" 
                    // Build result string with StringBuilder 
                    loTmpStr = Convert.ToString(MatchRecsBehavior.GetListNdx() + 1);

                    RemarkSB.Append("Marked ");
                    RemarkSB.Append(loTmpStr);
                    loTmpStr = Convert.ToString(loTable.GetRecCount());
                    RemarkSB.Append(" of ");
                    RemarkSB.Append(loTmpStr);
                    RemarkSB.Append(" at ");
                    RemarkSB.Append(loTable.GetFormattedFieldData(FieldNames.IssueTimeFieldName, "hh:mm"));
                    RemarkSB.Append(", elapsed: ");
                    RemarkSB.Append(loTable.GetFormattedFieldData(FieldNames.ElapsedTimeFieldName, "dd hh:mm"));
                    // Now stuff StringBuilder value into result string in one shot
                    oMMRemark = RemarkSB.ToString();
                    return;
                case 2:
                    // Build result string with StringBuilder 
                    RemarkSB.Append("Marked Loc: ");
                    RemarkSB.Append(loTable.GetFormattedFieldData(FieldNames.LocationFieldName, ""));
                    // Now stuff StringBuilder value into result string in one shot
                    oMMRemark = RemarkSB.ToString();
                    return;
            }
#endif
        }

        /// <summary>
        /// This routine is called when a ticket is being issued from a mark mode record.  The ticket
        /// form's fSourceDataStruct was set to this struct, so part of the TIssForm::PrepareForEdit 
        /// is to call this routine.
        /// The default implementation copies all the fields in common from this structure to the
        /// form, and disables those fields.
        /// Additionally, we want to stuff the remarks w/ mark mode specific info:
        /// Remark1: "Marked 1/5 @10:30"
        /// Remark2: "[original mark location]"
        /// </summary>
        /// 
        public override int InitExtraFormDataFields(Android.App.Fragment iFormBuilder)
        {

#if _original_

            TTableFldDef loStructFld;
            string loMMRemark = "";
            TTTable SrcDataTable = ((IssueStructLogicAndroid)(iFormBuilder.CfgForm.StructLogicObj)).IssueStruct.MainTable.HighTableRevision.Tables[0];

            // Any extra fields in here need to be undone in "UndoIssueSourceExtraFields"
            if ((loStructFld = SrcDataTable.fTableDef.GetField(FieldNames.Remark1FieldName)) != null)
            {
                GetMMRemark(1, ref loMMRemark);
                base.InitFormDataField(iFormBuilder, loStructFld, FieldNames.Remark1FieldName, loMMRemark);
            }

            if ((loStructFld = SrcDataTable.fTableDef.GetField(FieldNames.Remark2FieldName)) != null)
            {
                GetMMRemark(2, ref loMMRemark);
                base.InitFormDataField(iFormBuilder, loStructFld, FieldNames.Remark2FieldName, loMMRemark);
            }
#endif
            return base.InitExtraFormDataFields(iFormBuilder);
        }


        public override bool InitExtraFormDataFieldsAndroid( string iFieldName, ref string ioIssueSourceValue)
        {
            switch (iFieldName)
            {

                case AutoISSUE.DBConstants.sqlRemark1Str:
                    {
                        GetMMRemark(1, ref ioIssueSourceValue, fSearchAndIssueResult.SearchResultSelectedRow, fSearchAndIssueResult.SearchResultSelectedRowIndex);
                        return true;
                    }

                case AutoISSUE.DBConstants.sqlRemark2Str:
                    {
                        GetMMRemark(2, ref ioIssueSourceValue, fSearchAndIssueResult.SearchResultSelectedRow, fSearchAndIssueResult.SearchResultSelectedRowIndex);
                        return true;
                    }

                default:
                    {
                        return base.InitExtraFormDataFieldsAndroid(iFieldName, ref ioIssueSourceValue);
                    }
            }

        }


        public override void InitSourceFormattingInfo( DataRow iSourceDataRow )
        {
            FillFormattingTableWithCurrentRecord(iSourceDataRow);
            //FillFormattingTableWithCurrentRecord(fSearchAndIssueResult.SearchResultSelectedRow);
        }


        internal override int UndoIssueSourceExtraFields(Android.App.Fragment iFormBuilder)
        {

#if _original_
            TTEdit loEdit;
            // does this field exist in the issue form? 
            if ((loEdit = FindCfgEditByName(iFormBuilder.CfgForm, FieldNames.Remark1FieldName)) != null)
            {
                // don't let mm stuffed remarks hang-around
                loEdit.Behavior.SetEditBufferAndPaint("");
                loEdit.SetEnabledLocked(false);
            }
            if ((loEdit = FindCfgEditByName(iFormBuilder.CfgForm, FieldNames.Remark2FieldName)) != null)
            {
                // don't let mm stuffed remarks hang-around
                loEdit.Behavior.SetEditBufferAndPaint("");
                loEdit.SetEnabledLocked(false);
            }

#endif
            return base.UndoIssueSourceExtraFields(iFormBuilder);
        }

        public override int InitFormDataField(Android.App.Fragment iFormBuilder, TTableFldDef iStructFld, string iToFormFieldName, string iToFormFieldValue)
        {

#if _original_
            if (string.Compare(iStructFld.Name, FieldNames.IssueTimeFieldName, true) == 0)
            {
                string loChalkTime = "";
                TTEdit loChalkTimeFld = FindCfgEditByName(iFormBuilder.CfgForm, FieldNames.ChalkTimeFieldName);
                if (loChalkTimeFld == null) return 0;
                GetChalkTime(loChalkTimeFld.Behavior.GetEditMask(), ref loChalkTime);
                return base.InitFormDataField(iFormBuilder, iStructFld, FieldNames.ChalkTimeFieldName, loChalkTime);
            }

            if (string.Compare(iStructFld.Name, FieldNames.IssueDateFieldName, true) == 0)
            {
                string loChalkDate = "";
                TTEdit loChalkDateFld = FindCfgEditByName(iFormBuilder.CfgForm, FieldNames.ChalkDateFieldName);
                if (loChalkDateFld == null) return 0;
                GetChalkDate(loChalkDateFld.Behavior.GetEditMask(), ref loChalkDate);
                return base.InitFormDataField(iFormBuilder, iStructFld, FieldNames.ChalkDateFieldName, loChalkDate);
            }
#endif
            return base.InitFormDataField(iFormBuilder, iStructFld, iStructFld.Name, "");
        }
    }
}
