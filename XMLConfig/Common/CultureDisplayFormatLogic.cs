using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System;
//using System.Data;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using Reino.ClientConfig;

using XMLConfig;
//using Duncan.AI.Droid.Utils;
//using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
//using Duncan.AI.Droid.Utils.PrinterSupport;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Reino.ClientConfig
{

    /// <summary>
    /// Formatting table information 
    /// </summary>
    public class CultureDisplayFormattingTableInfo : object
    {
        public string fFormatRecordFingerprint = "";

        //public TDataSetStructInfo fDataSetStructInfo;
        public TIssStruct fIssStruct;



        public int fFormRevNum;
        public int fTableRevNum;
        public string fFormRevName = "";
        public TIssPrnFormRev fCurrentRecordPrintPicRev;
        public TTTable fCurrentRecordFormattingTable;
        public TTableDef fCurrentRecordFormattingTableDef;
        public TTableDefRev fCurrentRecordFormattingTableDefRev;
        public TIssForm fIssueForm;

        public string fStructDateMask;
        public string fStructTimeMask;


        /// <summary>
        /// Collection of all displayable data fields
        /// </summary>
        //public List<TClientColumnInfoClass> fDisplayableDataFields = new List<TClientColumnInfoClass>();

        /// <summary>
        /// The list of fields that are actually displayed on the primary record viewing tab
        /// Used to exclude fields from the non-printed fields list
        /// </summary>
        //public List<string> fDisplayedDataFields = new List<string>();


        /// <summary>
        /// Info about the handheld paper for this record, if a print picture is defined
        /// This object is not available until a citation image has been rendered 
        /// </summary>
        //public HandheldPaperInfo fPaperInfo;
    }

    public class CultureDisplayFormatLogic
    {

        public static string ConvertDateStringToDisplayFormatLocalTime(string iDateStringSource, string iDateStringSourceFormat, string iDateStringDestinationFormat)
        {
            string loFormattedString = iDateStringSource;

            try
            {
                DateTime loSourceDT = DateTime.ParseExact(iDateStringSource, iDateStringSourceFormat, CultureInfo.InvariantCulture);
                DateTime loSourceDTLocal = DateTime.SpecifyKind(loSourceDT, System.DateTimeKind.Local);
                loFormattedString = loSourceDTLocal.ToString(iDateStringDestinationFormat);
            }
            catch (Exception exp)
            {
                // default to return the original string, in case of conversion issues
                loFormattedString = iDateStringSource;
            }

            return loFormattedString;
        }



        /// <summary>
        /// Specialized call to handle recursive drill for TSheets
        /// </summary>
        /// <param name="iTargetName"></param>
        /// <param name="Container"></param>
        /// <returns></returns>
        private TEditField GetEditClassFromTabSheet(string iTargetName, System.Collections.Generic.IList<TSheet> Container)
        {
            TEditField loResult;

            foreach (Reino.ClientConfig.TSheet NextCtrl in Container)
            {
                // Is it a TTPanel or descendant? (It certainly should be since TSheet inherits from TTPanel)
                if (NextCtrl is Reino.ClientConfig.TTPanel)
                {
                    // check each
                    loResult = GetEditFieldInContainer_Recursive(iTargetName, ((Reino.ClientConfig.TTPanel)(NextCtrl)).Controls); // recursive
                    // got something?
                    if (loResult != null)
                    {
                        // first match wins!
                        return loResult;
                    }

                }
            }

            return null;
        }


        /// <summary>
        /// Drills recursively down to locate first occurence of TEditField with targetname
        /// </summary>
        /// <param name="iTargetField"></param>
        /// <param name="Container"></param>
        /// <returns></returns>
        private TEditField GetEditFieldInContainer_Recursive(string iTargetField, System.Collections.Generic.IList<TWinClass> Container)
        {
            foreach (Reino.ClientConfig.TWinClass NextCtrl in Container)
            {
                // Is it a TTEdit or descendant? (TNumEdit, TIssEdit, TTMemo, TEditListBox)
                //if (NextCtrl is Reino.ClientConfig.TTEdit)
                // we're only interested in TEditFields
                if (NextCtrl is Reino.ClientConfig.TEditField)
                {
                    if (((TEditField)NextCtrl).Name.Equals(iTargetField))
                    {
                        // got it
                        return ((TEditField)NextCtrl);
                    }
                }

                // Is it a "TabSheet" container?
                if (NextCtrl is Reino.ClientConfig.TTTabSheet)
                {
                    return GetEditClassFromTabSheet(iTargetField, ((Reino.ClientConfig.TTTabSheet)(NextCtrl)).Sheets); // Recursive
                }

                // Is it a TTPanel or descendant? (TTTabSheet, TPanel, TSheet)
                if (NextCtrl is Reino.ClientConfig.TTPanel)
                {
                    return GetEditFieldInContainer_Recursive(iTargetField, ((Reino.ClientConfig.TTPanel)(NextCtrl)).Controls); // Recursive
                }
            }

            // not in this container
            return null;
        }



        /// <summary>
        /// returns the picture mask from the print picture if defined, or the struture when the field isn't printed
        /// Pass -1 as the iFormatRevisionNumber to get the highest available
        /// </summary>
        /// <returns></returns>
        private string GetDotNetDisplayMaskForField(string iSqlFieldNameStr, ReinoControls.TEditFieldType iFieldType, CultureDisplayFormattingTableInfo iFormatInfo, int iFormatRevisionNumber)
        {
            // determine what our  masks will be
            string loDotNetDisplayMask = "";
            bool loMaskDefined = false;


            // if we have a print picture? we'll try to find them there first
            if ((iFormatInfo.fIssueForm != null) && (iFormatInfo.fIssueForm.PrintPictureList.Count > 0))
            {
                TIssPrnForm loPrintPicture = null;

                // we'd like to just locate it by name
                if (iFormatInfo.fFormRevName.Length > 0)
                {
                    TObjBasePredicate loFindFormPredicate = new TObjBasePredicate(iFormatInfo.fFormRevName);
                    loPrintPicture = iFormatInfo.fIssueForm.PrintPictureList.Find(loFindFormPredicate.CompareByName_CaseInsensitive);
                }

                // find the print picture? if not, employ a safety fallback,
                // in case printpicture name is not populated in the data
                if (loPrintPicture == null)
                {
                    if (iFormatInfo.fIssueForm.PrintPictureList.Count > 0)
                    {
                        // default to the only print picture
                        loPrintPicture = iFormatInfo.fIssueForm.PrintPictureList[0];
                    }
                }


                // did we get something?
                if (loPrintPicture != null)
                {
                    // do they want the latest?
                    if (iFormatRevisionNumber == -1)
                    {
                        {
                            TIssPrnFormRev loHighFormRev = loPrintPicture.HighFormRevision;
                            if (loHighFormRev != null)
                            {
                                // substitute the high form revision number
                                iFormatRevisionNumber = loHighFormRev.Revision;
                            }
                        }
                    }

                    // now we have a target form revision - find it in the collection
                    TIssPrnFormRev loPrintPicRev = null;
                    foreach (TIssPrnFormRev loCheckFormRev in loPrintPicture.Revisions)
                    {
                        if (loCheckFormRev.Revision == iFormatRevisionNumber)
                        {
                            loPrintPicRev = loCheckFormRev;
                            break;
                        }
                    }

                    // if we've got a print picture revision, we'll try and find it in there
                    if (loPrintPicRev != null)
                    {
                        TPrnDataElementFindPredicate loFindFieldPred = new TPrnDataElementFindPredicate(iSqlFieldNameStr);
                        TWinBasePrnData loPrnField = loPrintPicRev.AllPrnDataElements.Find(loFindFieldPred.CompareByElementName);
                        if (loPrnField != null)
                        {
                            loDotNetDisplayMask = loPrnField.Mask;
                            loMaskDefined = true;
                        }
                    }
                }
            }


            // got one yet?
            if (loMaskDefined == false)
            {
                if (iFormatInfo.fIssueForm != null)
                {
                    // not in the print picture, lets try to find it in the issue form
                    TEditField loEditField = GetEditFieldInContainer_Recursive(iSqlFieldNameStr, iFormatInfo.fIssueForm.Controls);

                    // got it?
                    if (loEditField != null)
                    {
                        loDotNetDisplayMask = loEditField.EditMask;
                        loMaskDefined = true;
                    }
                }

            }


            // anything yet?
            if (loMaskDefined == false)
            {
                // not in print pic or issue form, so we'll try to find them in the storage struct
                //TTableFldDef loFldDef = iFormatInfo.fDataSetStructInfo.fIssStruct.MainTable.GetField(iSqlFieldNameStr);
                TTableFldDef loFldDef = iFormatInfo.fIssStruct.MainTable.GetField(iSqlFieldNameStr);
                if (loFldDef != null)
                {
                    loDotNetDisplayMask = loFldDef.Mask;
                    loMaskDefined = true;
                }
            }



            // return what we found - translated it into .NET compatible
            switch (iFieldType)
            {
                case ReinoControls.TEditFieldType.efDate:
                    {
                        return AutoISSUE.DBConstants.GetDotNetMaskForAutoISSUEMask_Date(loDotNetDisplayMask);
                    }

                //case TBindFieldType.bftDateMonthYearOnly:
                //    {
                //        return AutoISSUE.DBConstants.GetDotNetMaskForAutoISSUEMask_Date(loDotNetDisplayMask);
                //    }

                //case TBindFieldType.bftDateYearOnly:
                //    {
                //        return AutoISSUE.DBConstants.GetDotNetMaskForAutoISSUEMask_Date(loDotNetDisplayMask);
                //    }

                case ReinoControls.TEditFieldType.efTime:
                    {
                        return AutoISSUE.DBConstants.GetDotNetMaskForAutoISSUEMask_Time(loDotNetDisplayMask);
                    }

                //case TBindFieldType.bftCurrency:
                //    {
                //        return "c";
                //    }


                default: // also bftGeneric
                    {
                        return loDotNetDisplayMask;
                    }
            }
        }


        public CultureDisplayFormattingTableInfo ConstructFormattingTableInfoForIssueStruct(TIssStruct iIssueStruct, TIssForm iIssueForm)
        {
            // the formatting info is the table, form, and print picture that match the data revision
            CultureDisplayFormattingTableInfo loFormatInfo = new CultureDisplayFormattingTableInfo();


            loFormatInfo.fIssStruct = iIssueStruct;
            loFormatInfo.fIssueForm = iIssueForm;

            //loFormatInfo.fDataSetStructInfo = dsDataSetStructInfo;
            //loFormatInfo.fFormRevName = iFormRevName;
            //loFormatInfo.fFormRevNum = iFormRevNum;
            //loFormatInfo.fTableRevNum = iTableRevNum;

            // handheld has one revision only - its always the highest table rev
            TTableDefRev loDefRev = iIssueStruct.MainTable.HighTableRevision;

            // we must make our own copy because it must have exactly one revision
            // this way we will have only a single revision in our private def because
            // the virtual field code embed references to the highest table revision, which we are not
            // plus we'll have it here for thread safe access/usage
            loFormatInfo.fCurrentRecordFormattingTableDef = new TTableDef();
            loFormatInfo.fCurrentRecordFormattingTableDefRev = new TTableDefRev();
            foreach (TTableFldDef oneField in loDefRev.Fields)
            {
                loFormatInfo.fCurrentRecordFormattingTableDefRev.Fields.Add(oneField);
            }
            loFormatInfo.fCurrentRecordFormattingTableDef.Revisions.Add(loFormatInfo.fCurrentRecordFormattingTableDefRev);

            // now the table
            loFormatInfo.fCurrentRecordFormattingTable = new TTTable();
            loFormatInfo.fCurrentRecordFormattingTable.fTableDef = loFormatInfo.fCurrentRecordFormattingTableDef;


            // get the display masks
            loFormatInfo.fStructDateMask = GetDotNetDisplayMaskForField(AutoISSUE.DBConstants.sqlIssueDateStr, ReinoControls.TEditFieldType.efDate, loFormatInfo, -1);
            loFormatInfo.fStructTimeMask = GetDotNetDisplayMaskForField(AutoISSUE.DBConstants.sqlIssueTimeStr, ReinoControls.TEditFieldType.efTime, loFormatInfo, -1);


            // return result
            return loFormatInfo;
        }



    }
}