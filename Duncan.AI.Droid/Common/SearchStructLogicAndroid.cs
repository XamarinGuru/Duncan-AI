

using Android.App;
using Android.OS;
using Android.Preferences;
using Android.Content;
using Duncan.AI.Droid.Utils.DataAccess;
using Duncan.AI.Droid.Utils.EditControlManagement;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Duncan.AI.Droid.Utils.HelperManagers;
//using System.Windows.Forms;
using Reino.ClientConfig;
using Reino.CommonLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
//using System.Data;
//using System.Drawing.Drawing2D;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using XMLConfig;

using Java.Lang;


namespace Duncan.AI.Droid.Common
{

    public class SearchStructLogicAndroid : IssueStructLogicAndroid    
    {
        #region InheritedFromWindowsMobileIssueStructLogic



        static internal Reino.ClientConfig.TTTable GetListTableByName(string TableName, ListSearchType iSearchType)
        {
            // Find the associated table definition. We'll look through agency lists fist, then 
            // through data structures.
            TObjBasePredicate predicate = new TObjBasePredicate(TableName);
            Reino.ClientConfig.TTableDef tableDef = null;

            foreach (Reino.ClientConfig.TAgList agencyList in DroidContext.XmlCfg.clientDef.ListMgr.AgLists)
            //foreach (Reino.ClientConfig.TAgList agencyList in TClientDef.GlobalClientDef.ListMgr.AgLists)
            {
                tableDef = agencyList.TableDefs.Find(predicate.CompareByName_CaseInsensitive);
                if (tableDef != null)
                {
                    if ((iSearchType == ListSearchType.NewIfAgencyOr1stData) ||
                        (iSearchType == ListSearchType.NewIfAgencyOr2ndData))
                    {
                        // Create a new instance of TTTable and add it to the TTableDef object
                        TTTable newTable = new TTTable();
                        newTable.SetTableName(TableName);
                        //tableDef.HighTableRevision.Tables.Add(newTable); // SetTableName adds it to the list
                        return newTable;
                    }
                    else if (iSearchType == ListSearchType.Get1st)
                    {
                        // Make sure we have enough tables, then return the 1st
                        while (tableDef.HighTableRevision.Tables.Count < 1)
                        {
                            // Create a new instance of TTTable and add it to the TTableDef object
                            TTTable newTable = new TTTable();
                            newTable.SetTableName(TableName); // This adds it to the list
                        }
                        return tableDef.HighTableRevision.Tables[0];
                    }
                    else if (iSearchType == ListSearchType.Get2nd)
                    {
                        // Make sure we have enough tables, then return the 2nd
                        while (tableDef.HighTableRevision.Tables.Count < 2)
                        {
                            // Create a new instance of TTTable and add it to the TTableDef object
                            TTTable newTable = new TTTable();
                            newTable.SetTableName(TableName); // This adds it to the list
                        }
                        return tableDef.HighTableRevision.Tables[1];
                    }
                    else
                    {
                        return tableDef.HighTableRevision.Tables[0];
                    }
                }
            }

            // Didn't find it in Agency Lists, so lets look at data structures now...
            // We won't add additional TTTable instances for writeable data structures
            foreach (Reino.ClientConfig.TIssStruct IssueStruct in DroidContext.XmlCfg.clientDef.IssStructMgr.IssStructs)
           //foreach (Reino.ClientConfig.TIssStruct IssueStruct in TClientDef.GlobalClientDef.IssStructMgr.IssStructs)
           {
                tableDef = IssueStruct.TableDefs.Find(predicate.CompareByName_CaseInsensitive);
                if (tableDef != null)
                {
                    // We're not an agency list, so switch to Get2nd style if necessary
                    if (iSearchType == ListSearchType.NewIfAgencyOr2ndData)
                        iSearchType = ListSearchType.Get2nd;

                    if ((iSearchType == ListSearchType.NewIfAgencyOr1stData) ||
                        (iSearchType == ListSearchType.Get1st))
                    {
                        // Make sure we have enough tables, then return the 1st
                        while (tableDef.HighTableRevision.Tables.Count < 1)
                        {
                            // Create a new instance of TTTable and add it to the TTableDef object
                            TTTable newTable = new TTTable();
                            newTable.SetTableName(TableName); // This adds it to the list

                            // TODO- ENDLESS LOOP - this doesnt add to the list, different references?

                            // we don't need to worry about fixing this because we will access the SQLite DB
                            // but we do need to worry about other places this type of code may exist



                        }
                        return tableDef.HighTableRevision.Tables[0];
                    }
                    else if (iSearchType == ListSearchType.Get2nd)
                    {
                        // Make sure we have enough tables, then return the 2nd
                        while (tableDef.HighTableRevision.Tables.Count < 2)
                        {
                            // Create a new instance of TTTable and add it to the TTableDef object
                            TTTable newTable = new TTTable();
                            newTable.SetTableName(TableName); // This adds it to the list
                        }
                        return tableDef.HighTableRevision.Tables[1];
                    }
                    else
                    {
                        return tableDef.HighTableRevision.Tables[0];
                    }
                }
            }

            // Couldn't find it, so return null
            return null;
        }

        #endregion


        /// <summary>
        /// Formatting table information 
        /// </summary>
        public class TSearchResultFormattingTableInfo : object
        {
            public string fFormatRecordFingerprint = "";

            //public TDataSetStructInfo fDataSetStructInfo;
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



        #region Members
        public static bool unSearchEvaluateInProcess = false;
        internal bool fCopyDataFromResult = false;

        internal TSearchResultFormattingTableInfo fFormatInfo;


        private TTTable fWirelessSearchResultTable = null;




        SearchResultEvaluationTimer myTimer = null;
        SearchStructServerInterface mySearchService = null;

        DataTable myDummyResultTable = null;

        byte[] _LastResultDataset = null;
        string _LastStatusText = string.Empty;
        string _LastResultErrMsg = string.Empty;
        bool _CalledFromSearchAndIssue = false;
        Reino.ClientConfig.TSearchStruct _CallingSearchStruct = null;


        short _MinMatchCount = 1;
        Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditDependency _InitiatingEditRestriction = null;



        IAsyncResult _LastAsyncResult = null;



        #endregion





        /// <summary>
        /// Resolve internal formatting table references based on the FORMREV for passed row
        /// The passed row should be from the MAIN TABLE of the record of interest
        /// To optimize for speed, we don't actually fill the formatting table with
        /// any data until/unless the click on the cite print picture tab
        /// </summary>
        private void ConstructFormattingTableInfo()
        //private TSearchResultFormattingTableInfo ConstructFormattingTableInfo(string iDataTableName, int iTableRevNum, string iFormRevName, int iFormRevNum)
        {
            // the formatting info is the table, form, and print picture that match the data revision
            TSearchResultFormattingTableInfo loFormatInfo = new TSearchResultFormattingTableInfo();
            //loFormatInfo.fDataSetStructInfo = dsDataSetStructInfo;
            //loFormatInfo.fFormRevName = iFormRevName;
            //loFormatInfo.fFormRevNum = iFormRevNum;
            //loFormatInfo.fTableRevNum = iTableRevNum;
            
            // handheld has one revision only - its always the highest table rev
            TTableDefRev loDefRev = IssueStruct.MainTable.HighTableRevision;

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


            // set the global reference 
            fFormatInfo = loFormatInfo;
        }



        /// <summary>
        /// Fills the formatting table with the current record from the dataset
        /// so that the data can be pulled out using the config defined formats
        /// </summary>
        internal void FillFormattingTableWithCurrentRecord(DataRow iCurrentRecordDataRow)
        {

            string loFieldValueStr;
            string loSourceFieldNameStr;
            DateTime loFieldValueDT;
            //DataRowView dvSourceRowView = null;



            // most fields will be located in the main table, but some will be found in subtables
            bool loFieldLocated = false;


            // loop through the fields in the table revision, and populate the table
            fFormatInfo.fCurrentRecordFormattingTable.ClearFieldValues();
            foreach (TTableFldDef loFld in fFormatInfo.fCurrentRecordFormattingTableDefRev.Fields)
            {

                // assume we'll find the target field in the main table
                loFieldLocated = false;

                // start with the actual name
                loSourceFieldNameStr = loFld.Name;

                // in the main table?
                if ((iCurrentRecordDataRow.Table.Columns.IndexOf(loSourceFieldNameStr)) != -1)
                {
                    // defense against funky databases - make sure this column wasn't created
                    // in error and the data is actually moved to a detail table
                    if (loFld.IsRedefinedInDetailTable == false)
                    {
                        // get the current datarow from the main table's currency manager
                        //iCurrentRecordDataRow = (DataRowView)fCurrentDataSetRecordInfo.fDataSetStructInfo.fMainTableCurrencyManager.Current;
                        loFieldLocated = true;
                    }

                }

                // find it in the main table?
                if (loFieldLocated == false)
                {
                    // it maybe in a detail table. Since detail tables can have multiple records,
                    // we're prepare for that. Start by assuming its the default row;
                    int loDetailSubTablePosition = 0;

                    // is this a "special" field?
                    if (loFld.TableNdx > 0)
                    {
                        // extract the occurence number from the end of the field name
                        int loOccurNoPos = loFld.Name.LastIndexOf('_');
                        if (loOccurNoPos != -1)
                        {
                            string loOccurNoStr = loFld.Name.Substring(loOccurNoPos + 1);
                            try
                            {
                                // the target is the "root" field name, without the occur no suffix
                                loSourceFieldNameStr = loFld.Name.Substring(0, loOccurNoPos);
                                // and the occurno suffix is its position in the list
                                loDetailSubTablePosition = Convert.ToInt32(loOccurNoStr);
                            }
                            catch
                            {
                                // somthing wasn't right - force a bad name so we'll get no data
                                loSourceFieldNameStr = loFld.Name + " is not defined.";
                            }
                        }


                        /*
                        // wasn't in the main table. look through the sub tables
                        foreach (TSubTableInfo loCheckTableInfo in fCurrentDataSetRecordInfo.fDataSetStructInfo.fSubTables)
                        {
                            // now we've stripped off any suffix... is it in this subtable?
                            if ((loCheckTableInfo.fDataTable.Columns.IndexOf(loSourceFieldNameStr)) != -1)
                            {
                                // if a sub table is the source, then we need to 
                                // use ITS currency manager for the child relation row, if we can...
                                if ((loCheckTableInfo.fSubTableCurrencyManager.Count > 0) && (loDetailSubTablePosition < loCheckTableInfo.fSubTableCurrencyManager.Count))
                                {
                                    // reference the row
                                    dvSourceRowView = (DataRowView)loCheckTableInfo.fSubTableCurrencyManager.List[loDetailSubTablePosition];
                                    // we got it
                                    loFieldLocated = true;
                                    break;
                                }

                            }
                        }
                         * */
                    }

                }

                // find one?
                if (loFieldLocated == false)
                {
                    // should always be able to locate the field, unless the db and the config aren't matched
                    continue;
                }



                try
                {

                    // default to 
                    string loSQLiteColumnReadDataMask = loFld.Mask;

                    //TPrnDataElementFindPredicate loFindPred = new TPrnDataElementFindPredicate(loFld.Name);
                    //TWinBasePrnData loPrnData = fFormatInfo.fCurrentRecordPrintPicRev.AllPrnDataElements.Find(loFindPred.CompareByElementName);
                    TWinBasePrnData loPrnData = null;
                    if (loPrnData == null)
                    {
                        loPrnData = new TWinBasePrnData();
                        loPrnData.Name = loFld.Name;
                    }


                    // must locate the fieldno in the table revision; if we ask via the tabledef we always get the highest
                    int loFieldNo = fFormatInfo.fCurrentRecordFormattingTable.fTableDef.GetFldNo(loPrnData.Name);

                    //loFieldNo = dsStructInfo.fCurrentRecordFormattingTableDefRev.Fields.FindIndex(new TObjBasePredicate(loPrnData.Name).CompareByName);

                    if (loFieldNo != -1)
                    {
                        TTableFldDef loOneField = fFormatInfo.fCurrentRecordFormattingTableDefRev.Fields[loFieldNo];
                        //string loDataMask;
                        // translate it back to AutoISSUE style for accurate display
                        // Always use the MaskForHH whenever possible as it will handle date and time masks
                        // properly. (The regular Mask property is not reliable because it has been subjected
                        // to various translations)
                        switch (loFld.EditDataType)
                        {
                            case TTableFldDef.TEditDataType.tftDate:
                                {
                                    /*
                                    if (loPrnData.MaskForHH != "")
                                        loSQLiteColumnReadDataMask = loPrnData.MaskForHH;
                                    else
                                        loSQLiteColumnReadDataMask = AutoISSUE.DBConstants.GetAutoISSUEMaskForDotNetMask_Date(loPrnData.Mask);

                                    loSQLiteColumnReadDataMask = AutoISSUE.DBConstants.GetDotNetMaskForAutoISSUEMask_Date(loSQLiteColumnReadDataMask);
                                     */

                                    // this is how it SHOULD be, we have to fix all the writes to use this too
                                    loSQLiteColumnReadDataMask = Constants.DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK;
                                    break;
                                }

                            case TTableFldDef.TEditDataType.tftTime:
                                {
                                    /*
                                    if (loPrnData.MaskForHH != "")
                                        loSQLiteColumnReadDataMask = loPrnData.MaskForHH;
                                    else
                                        loSQLiteColumnReadDataMask = AutoISSUE.DBConstants.GetAutoISSUEMaskForDotNetMask_Time(loPrnData.Mask);

                                    loSQLiteColumnReadDataMask = AutoISSUE.DBConstants.GetDotNetMaskForAutoISSUEMask_Time(loSQLiteColumnReadDataMask);
                                     */

                                    // this is how it SHOULD be, we have to fix all the writes to use this too
                                    loSQLiteColumnReadDataMask = Constants.DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK;
                                    break;
                                }
                            default:
                                {

                                    // we dont want to override?? 
                                    /*
                                    if (loPrnData.MaskForHH != "")
                                        loDataMask = loPrnData.MaskForHH;
                                    else
                                        loDataMask = loPrnData.Mask;
                                     */
                                    break;

                                }
                        }
                    }


                    // get the value from the dataset in the appropriate format
                    switch (loFld.EditDataType)
                    {
                        /* AJW - these are all string types in SQLite.... 
                         * */

                        case TTableFldDef.TEditDataType.tftDate:
                            {
                                if (iCurrentRecordDataRow.IsNull(loSourceFieldNameStr) == false)
                                {
                                    // get the value from the datarow
                                    //loFieldValueDT = (DateTime)iCurrentRecordDataRow[loSourceFieldNameStr];

                                    // get the string value from the datarow
                                    string loDateTimeTypeAsString = iCurrentRecordDataRow[loSourceFieldNameStr].ToString().Trim();

                                    if (loDateTimeTypeAsString.Length > 0)
                                    {
                                        try
                                        {
                                            // convert from SQLite formatted date string to a DateTime obj
                                            loFieldValueDT = DateTime.ParseExact(loDateTimeTypeAsString, loSQLiteColumnReadDataMask, CultureInfo.CurrentCulture);

                                            // and now convert it into the format the formatting table is expecting
                                            loFieldValueStr = loFieldValueDT.ToString(ReinoTablesConst.DATE_TYPE_DATAMASK);
                                        }
                                        catch (System.Exception exp)
                                        {
                                            Console.WriteLine("An error occurred during date extraction. " + exp.Message);
                                            loFieldValueStr = "";
                                        }
                                    }
                                    else
                                    {
                                        loFieldValueStr = ""; // unparsable value
                                    }


                                    // the "(select)" should't be stored or printed
                                    //if (loFieldValueStr == Constants.SPINNER_DEFAULT)
                                    //    loFieldValueStr = "";    


                                    // set it in the field
                                    fFormatInfo.fCurrentRecordFormattingTable.SetFormattedFieldData(loFld.Name, ReinoTablesConst.DATE_TYPE_DATAMASK, loFieldValueStr);
                                }
                                break;
                            }

                        case TTableFldDef.TEditDataType.tftTime:
                            {
                                if (iCurrentRecordDataRow.IsNull(loSourceFieldNameStr) == false)
                                {
                                    // get the value from the datarow
                                    //loFieldValueDT = (DateTime)iCurrentRecordDataRow[loSourceFieldNameStr];

                                    // get the string value from the datarow
                                    string loDateTimeTypeAsString = iCurrentRecordDataRow[loSourceFieldNameStr].ToString().Trim();


                                    if (loDateTimeTypeAsString.Length > 0)
                                    {
                                        try
                                        {
                                            // convert from SQLite formatted date string to a DateTime obj
                                            loFieldValueDT = DateTime.ParseExact(loDateTimeTypeAsString, loSQLiteColumnReadDataMask, CultureInfo.CurrentCulture);

                                            // and now convert it into the format the formatting table is expecting
                                            loFieldValueStr = loFieldValueDT.ToString(ReinoTablesConst.TIME_TYPE_DATAMASK);
                                        }
                                        catch (System.Exception exp)
                                        {
                                            Console.WriteLine("An error occurred during date extraction. " + exp.Message);
                                            loFieldValueStr = "";
                                        }
                                    }
                                    else
                                    {
                                        loFieldValueStr = ""; // unparsable value
                                    }


                                    //// the "(select)" should't be stored or printed
                                    //if (loFieldValueStr == Constants.SPINNER_DEFAULT)
                                    //    loFieldValueStr = "";    

                                    // set it in the field
                                    fFormatInfo.fCurrentRecordFormattingTable.SetFormattedFieldData(loFld.Name, ReinoTablesConst.TIME_TYPE_DATAMASK, loFieldValueStr);
                                }
                                break;
                            }
                        case TTableFldDef.TEditDataType.tftInteger:
                            {
                                // get the value from the datarow
                                loFieldValueStr = iCurrentRecordDataRow[loSourceFieldNameStr].ToString();

                                //// the "(select)" should't be stored or printed
                                //if (loFieldValueStr == Constants.SPINNER_DEFAULT)
                                //    loFieldValueStr = "";    

                                // set it in the field
                                fFormatInfo.fCurrentRecordFormattingTable.SetFormattedFieldData(loFld.Name, loSQLiteColumnReadDataMask, loFieldValueStr);
                                break;
                            }
                        case TTableFldDef.TEditDataType.tftReal:
                            {
                                // get the value from the datarow
                                loFieldValueStr = iCurrentRecordDataRow[loSourceFieldNameStr].ToString();

                                // the "(select)" should't be stored or printed
                                //if (loFieldValueStr == Constants.SPINNER_DEFAULT)
                                //    loFieldValueStr = "";    


                                // NOTE: this is a KLUGE fix - 
                                // the field mask usually isn't set for real fields,
                                // and when its not, the SetForattedFieldData won't format it
                                // This usually isn't a problem because the value is coming from an edit box which
                                // already has formatted it. For us, we're getting the value
                                // from a dataset, raw and unformatted. So we need to force
                                // a format if there isn't one defined so that we don't
                                // end up storing "35" and read back "0.35" when what we wanted was "35.00"
                                if (loFld.Mask.Length == 0)
                                {
                                    // no mask defined, we'll make an assumption and give it one that forces decimal places
                                    if (iCurrentRecordDataRow[loSourceFieldNameStr] != System.DBNull.Value)
                                    {
                                        loFieldValueStr += "00";
                                    }

                                    fFormatInfo.fCurrentRecordFormattingTable.SetFormattedFieldData(loFld.Name, "8", loFieldValueStr);
                                }
                                else
                                {
                                    // set it in the field, using the defined mask
                                    fFormatInfo.fCurrentRecordFormattingTable.SetFormattedFieldData(loFld.Name, loSQLiteColumnReadDataMask, loFieldValueStr);
                                }
                                break;
                            }


                        // default case is also the string
                        default: //TTableFldDef.TEditDataType.tftString :
                            {
                                // get the value from the datarow
                                loFieldValueStr = iCurrentRecordDataRow[loSourceFieldNameStr].ToString();

                                // the "(select)" should't be stored or printed
                                //if (loFieldValueStr == Constants.SPINNER_DEFAULT)
                                //    loFieldValueStr = "";    


                                // set it in the field
                                fFormatInfo.fCurrentRecordFormattingTable.SetFormattedFieldData(loFld.Name, loSQLiteColumnReadDataMask, loFieldValueStr);
                                break;
                            }
                    }

                }
                catch (System.Exception exp)
                {
                    // if its not in the dataset... ??
                    Console.WriteLine("An error occurred during data formatting. " + exp.Message);
                }

            }


        }
        




        #region Public Methods
        public void PostConstruction()
        {
            // create a table to hold the wireless search results 
            // It will be populated later.  The table itself relies on the TTableDef to 
            // already exist. Table defs are stored in a global list so
            // we don't have to keep track of them.
            string loWirelessSearchResultFormattingTableName = this.IssueStruct.MainTable.HighTableRevision.Name +
                "_WIRELESS" + ReinoTablesConst.WRITE_TABLE_TO_RAM_IDENT;

            // create the table def for it - pass the name without the extension to the constructor
            TTableDef loTableDef = new TTableDef();
            loTableDef.Name = loWirelessSearchResultFormattingTableName;
            loTableDef.fOpenEdit = true; // open in read/write mode
            loTableDef.fTblNameExt = ".DAT";

            if (loTableDef.Revisions.Count == 0)
            {
                TTableDefRev DefRev = new TTableDefRev();
                DefRev.Name = loWirelessSearchResultFormattingTableName;
                loTableDef.Revisions.Add(DefRev);
            }

            // this isn't read from the CFG file, but we have to make sure the call is made to get things initialized
            loTableDef.PostDeserialize(Reino.ClientConfig.TTableListMgr.glTableMgr);
            // we're just a duplicate of the "real" table
            TTableDef loSrcTblDef = this.IssueStruct.MainTable;
            loTableDef.CopyTableStructure(ref loSrcTblDef, false);

            // create this table instance
            fWirelessSearchResultTable = new Reino.ClientConfig.TTTable();
            fWirelessSearchResultTable.Name = loWirelessSearchResultFormattingTableName;
            fWirelessSearchResultTable.SetTableName(loWirelessSearchResultFormattingTableName);


            ConstructFormattingTableInfo();

            fHasDefinedSearchResultAction = ValidateSearchResultActionSetup();
        }

        /// <summary>
        /// Searches the data table for the records that match the current ISSUE form's
        /// fMatchFields values. If more than iMinMatchCount matching record 
        /// (the current record will always match), the SearchMatch screen is displayed.
        /// </summary>
        public bool PerformSearchAndIssue(FormPanel iFromForm, bool iOnlyOnNewFilters, short iMinMatchCount,
            string iMatchFieldsName, Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditDependency iActiveEditRestriction, bool DisablePaintingSearchScreenWhenClosing)
        {
            SearchParameterPacket loSearchResult;
            bool loWirelessHostAvailable = false;
            bool loResult;

            CallingFormPanel = iFromForm;
            

            // do the search and select
            loSearchResult = privPerformSearch( iFromForm, iOnlyOnNewFilters, iMinMatchCount, iMatchFieldsName,
                                                                    true, iActiveEditRestriction, ref loWirelessHostAvailable, DisablePaintingSearchScreenWhenClosing);
            if ( loSearchResult.fSearchResultDTOList.Count == 0 )
            {
            //if ((loSearchResult = privPerformSearch(iFromForm, iOnlyOnNewFilters, iMinMatchCount, iMatchFieldsName,
            //    true, iActiveEditRestriction, ref loWirelessHostAvailable, DisablePaintingSearchScreenWhenClosing)) == null)
            //{
                // We can return right away when the table is null (empty), UNLESS the EditRestriction
                // has IntParam2 = 1, which means reversed logic where an empty table IS the desireable
                // return that should be evaluated.
                if ((iActiveEditRestriction == null) || (iActiveEditRestriction.IntParam2 == 0))
                    return false;
            }


            // decide how to handle the results - is this a wireless search?
            bool loWirelessSearchEnabled = ((this.IssueStruct as TSearchStruct).WirelessSearchEnabled == TSearchStruct.TWirelessEnabledType.wlWhenAvailable);

            // this is Android, we are always wireless?
            //loWirelessSearchEnabled = true;



            // decide how to handle the results - do we look for them now or later?
            if ((loWirelessSearchEnabled == true ) && (loWirelessHostAvailable == true))
            //if (((this.IssueStruct as TSearchStruct).WirelessSearchEnabled == TSearchStruct.TWirelessEnabledType.wlWhenAvailable) &&
            //        (loWirelessHostAvailable == true))
                {
                // this was a wireless search, we'll come back when its finished
                return false;
            }
            else
            {

                //// this is a local file search, evaluate the results now
                //// grab the mutex
                //SearchStructLogicAndroid.unSearchEvaluateInProcess = true;
                //loResult = EvaluateSearchAndIssueResult(loSearchResult);
                //// release the mutex
                //SearchStructLogicAndroid.unSearchEvaluateInProcess = false;
                //EvaluateWirelessSearchResult();
                //return loResult;

                // this is a local file search, done until the dialog call back is complete
                EvaluateWirelessSearchResult();
                return false;
            }
        }

        /// <summary>
        /// Shell around privPerformSearch that then calls EvaluateWirelessSearchResult when it is done.
        /// Because a locally invoked version of performSearch will acquire the mutex,
        /// EvaluateWirelessSearchResult does nothing, just returns.  All wireless search results are
        /// left pending. By calling that routine upon completion, the wireless results are handled.
        /// </summary>
        public SearchParameterPacket PerformSearch(FormPanel iFromForm, bool iOnlyOnNewFilters, short iMinMatchCount,
            string iMatchFieldsName, bool iCalledFromSearchAndIssue, Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditDependency iActiveEditRestriction,
            ref bool ioWirelessHostAvailable, bool DisablePaintingSearchScreenWhenClosing)
        {
            CallingFormPanel = iFromForm;

            SearchParameterPacket loSearchResult;

            // Do the work.
            loSearchResult = privPerformSearch(iFromForm, iOnlyOnNewFilters, iMinMatchCount, iMatchFieldsName,
                iCalledFromSearchAndIssue, iActiveEditRestriction, ref ioWirelessHostAvailable, DisablePaintingSearchScreenWhenClosing);

            // Catch-up with any wireless results that may have arrived while mutex was locked.
            EvaluateWirelessSearchResult();
            return loSearchResult;
        }

        public void HandleWirelessSearchResult( SearchParameterPacket iWirelessSearchResult )
        {

            // every search does the basic result evaluation
            EvaluateSearchResult(iWirelessSearchResult, true);

            //// is this was a search AND issue? 
            //if (iWirelessSearchResult.fCalledFromSearchAndIssue)
            //{
            //    // follow through with the whole search/issue mode as defined
            //    EvaluateSearchAndIssueResult(iWirelessSearchResult);
            //}

            //// give the initiating editrestriction and opportunity to process the results.
            //if ( (iWirelessSearchResult.fInitiatingEditRestriction != null) &&
            //     (iWirelessSearchResult.fInitiatingEditRestriction is Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_HotsheetFilter))
            //{
            //    (iWirelessSearchResult.fInitiatingEditRestriction as Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_HotsheetFilter).FinishEnforceRestriction(iWirelessSearchResult.fSearchMatchDTOList);

            //}
        }


        /// <summary>
        /// Safe method to retrieve value from datarow
        /// </summary>
        /// <param name="iColumnName"></param>
        /// <returns></returns>
        private string GetSafeColumnStringValueFromDataRow(DataRow iSourceRow, string iColumnName)
        {
            string loResultStr = string.Empty;
            try
            {
                if (iSourceRow.Table.Columns.IndexOf(iColumnName) != -1)
                {
                    if (iSourceRow[iColumnName] != null)
                    {
                        loResultStr = iSourceRow[iColumnName].ToString();
                    }
                }
            }
            catch (System.Exception exp)
            {
                //string loErrMsg = "Error: GetValueFromDataRow " + "tableName: " + iSourceRow.Table.TableName;
                LoggingManager.LogApplicationError(exp, "tableName: " + iSourceRow.Table.TableName, "GetValueFromDataRow-HotSheetFilter");
                Console.WriteLine("Exception source: {0}", exp.Source);
            }

            return loResultStr;
        }


        private string[] GetHotSheetFilteredListFromSearchResults(SearchParameterPacket iSearchResult)
        {
            List<string> loFilteredList = new List<string>();


            Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_HotsheetFilter loEditRestiction = (Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_HotsheetFilter)iSearchResult.fInitiatingEditRestriction;
            THotSheetStruct loHotSheetStruct = (THotSheetStruct)iSearchResult.fSearchStruct;
            SearchStructLogicAndroid loSearchStructLogic = (SearchStructLogicAndroid)loHotSheetStruct.StructLogicObj;

            string loFilteredColumnName = null;;
            if (iSearchResult.fInitiatingEditRestriction.Parent.ListTableColumn.Length > 0)
            {
                loFilteredColumnName = iSearchResult.fInitiatingEditRestriction.Parent.ListTableColumn[0];
            }

            if (loFilteredColumnName != null)
            {
                foreach (SearchMatchDTO oneDTO in iSearchResult.fSearchResultDTOList)
                {
                    if (oneDTO.rawRow != null)
                    {
                        loFilteredList.Add(GetSafeColumnStringValueFromDataRow(oneDTO.rawRow, loFilteredColumnName));
                    }
                }
            }


            // if filter has no items match, we must return one empty string
            if (loFilteredList.Count == 0)
            {
                loFilteredList.Add("");
            }
            return loFilteredList.ToArray();
        }

        public void HandleSearchResult_DialogCallback(SearchParameterPacket iSearchResult )
        {

            // is this was a search AND issue? 
            if (iSearchResult.fCalledFromSearchAndIssue)
            {
                // follow through with the whole search/issue mode as defined
                EvaluateSearchAndIssueResult(iSearchResult);
            }

            // give the initiating editrestriction and opportunity to process the results.
            if ((iSearchResult.fInitiatingEditRestriction != null) &&
                 (iSearchResult.fInitiatingEditRestriction is Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_HotsheetFilter))
            {


                // It is possible the wireles results have now been processed. 
                // If so, do not allow the local results to erase them.
                //if (fPreemptedByWireless == false)
                {

                    // cast into local vars to simplify readability and maintenance
                    Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_HotsheetFilter loEditRestiction = (Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_HotsheetFilter)iSearchResult.fInitiatingEditRestriction;
                    THotSheetStruct loHotSheetStruct = (THotSheetStruct)iSearchResult.fSearchStruct;
                    SearchStructLogicAndroid loSearchStructLogic = (SearchStructLogicAndroid)loHotSheetStruct.StructLogicObj;

                    if (loEditRestiction.Parent != null)
                    {
                        // set the same filters constructed by the search - these will filter the drop down list until the next form is started
                        loEditRestiction.Parent.Filters = iSearchResult.FiltersForInitiatingRestriction;

                        // replace the Adapter as well so the drop down will contain the filtered results as well
                        // AJW TODO - not needed when CustomSearchMatchAdapter in place and working
                        string[] loFilteredList = GetHotSheetFilteredListFromSearchResults(iSearchResult);

                        if (loEditRestiction.Parent.PanelField.uiComponent is CustomAutoTextView)
                        {
                            Helper.UpdateControlWithNewListPrim((CustomAutoTextView)loEditRestiction.Parent.PanelField.uiComponent, loFilteredList, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.bfSetHasBeenFocusedFalse);
                        }
                    }



#if _original_
            if (loTable == null)
            {
                // This needs to set a filter the excludes all permits?
                loParentTable.RemoveAllFilters();
                loParentTable.ActivateFilters();
                return;
            }

            // Have a result, paste it to parent edit
            loParentTable.CopyFilters(loTable.GetFilter());

            if (EditRestrict.GetParent() != null)
            {
                EditRestrict.GetParent().ListItemCount = loParentTable.GetRecCount();
                EditRestrict.GetParent().ListItemCache.Clear();
                if (EditRestrict.GetParent().GridDisplayCache != null)
                    EditRestrict.GetParent().GridDisplayCache.Clear();
                if (EditRestrict.GetParent().ListBox != null)
                    EditRestrict.GetParent().ListBox.RefreshItems(false);
            }
#endif




#if _original_
            EditRestrict.GetParent().SetEditBufferAndPaint(ReinoControls.TextBoxBehavior.GetFieldDataFromTable(
                loParentTable, EditRestrict.GetParent().CfgCtrl.Name,
                loTable.GetCurRecNo(), EditRestrict.GetParent().GetEditMask()));
#endif


                    // update the found value
                    string loNewFieldValue = string.Empty;
                    if (loSearchStructLogic.GetFieldValueFromIssueSourceTable( iSearchResult.SearchResultSelectedRow,
                                                                               iSearchResult.fInitiatingEditRestriction.GetParent().CfgCtrl.Name,
                                                                               ref loNewFieldValue) == true)
                    {
                        loEditRestiction.UpdateParentControlWithNewValue( loNewFieldValue, 
                                                                          EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, 
                                                                          EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue);
                    }
                }



                (iSearchResult.fInitiatingEditRestriction as Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_HotsheetFilter).FinishEnforceRestriction( iSearchResult );

            }
        }



#if _original_
        public void HandleWirelessSearchResult( SearchStructResult iWirelessSearchResult )
        {

            string loTempTableCompleteFilename; //[ MAX_PATH ];
            TTTable loTable;
            int loStatus;

            // ------ populate the result table -------
            // start by closing the file; (it was opened on startup)
            fWirelessSearchResultTable.fTableDef.CloseTable();

            // construct the complete pathname as it exists in Windows CE
            loTempTableCompleteFilename = @"\Temp\" + fWirelessSearchResultTable.Name + ".DAT";

#if !WindowsCE
            string loBaseFolder = Path.Combine(Reino.ClientConfig.ReinoTablesConst.cnstClientConfigFolder, "Temp");
            try { Directory.CreateDirectory(loBaseFolder); }
            catch { }
            if (loBaseFolder.EndsWith("\\") == false)
                loBaseFolder = loBaseFolder + "\\";
            loTempTableCompleteFilename = loBaseFolder + fWirelessSearchResultTable.Name + ".DAT";
#endif

            // delete any previous file to dump any previous temp records
            try
            {
                if (File.Exists(loTempTableCompleteFilename) == true)
                    File.Delete(loTempTableCompleteFilename);
            }
            catch
            {
            }

            // now open the table - the file will be re-created and empty
            fWirelessSearchResultTable.fTableDef.OpenEditTable(10);

            // populate the record buffer
            /* mcb 8/5/04: The returned data uses carrots (^) as record delimiters. We will
               parse the result set one record at a time. */
            int loRecLen = 0;
            for (; ; )
            {
                // find the end of this record. 
                // in this web services era, rows are delimited w/ CRLF. The last row may not have one.
                loRecLen = iResultRows.IndexOf("\r\n");
                if ((loRecLen < 0) && (iResultRows.Length == 0))
                    break; // no more delimiters means no more records.                
                if (loRecLen < 0)
                    loRecLen = iResultRows.Length;
                string loTempRec = iResultRows.Substring(0, loRecLen);
                fWirelessSearchResultTable.fTableDef.SetRecBuffer(loTempRec);
                // copy the record buffer to the fields
                fWirelessSearchResultTable.CopyRecBufToFieldValues();
                if ((loStatus = fWirelessSearchResultTable.WriteRecord()) < 0)
                {
                    // write error!
                    //AppMessageBox.ShowMessageWithBell("Failed writing record.", TranslateFileErrorCode2(loStatus), "");
                    break; // don't continue to fail
                }

                // Remove processed row data from source string
                iResultRows = iResultRows.Remove(0, loRecLen);
                // Also remove the CRLF that normally will be left behind
                if (iResultRows.StartsWith("\r\n"))
                    iResultRows = iResultRows.Remove(0, 2);
            }

            // close the table
            fWirelessSearchResultTable.fTableDef.CloseTable();

            // and re-open for reading
            fWirelessSearchResultTable.fTableDef.OpenReadTable(10);

            // there should be no filters, this table ONLY contains the matching records
            fWirelessSearchResultTable.RemoveAllFilters();
            // and make sure the status is set to suspend them so the record counts are accurate
            fWirelessSearchResultTable.SuspendFilters();


            // every search does the basic result evaluation
            loTable = EvaluateSearchResult(fWirelessSearchResultTable, iMinMatchRecCount, true);

            // is this was a search AND issue? 
            if (iCalledFromSearchAndIssue)
            {
                // follow through with the whole search/issue mode as defined
                EvaluateSearchAndIssueResult(loTable, iInitiatingEditRestriction);
            }

            // give the initiating editrestriction and opportunity to process the results.
            if ((iInitiatingEditRestriction != null) && (iInitiatingEditRestriction is TER_HotsheetFilter))
            {
                (iInitiatingEditRestriction as TER_HotsheetFilter).FinishEnforceRestriction(loTable);

            }

        }
#endif




        public Reino.ClientConfig.TSearchMatchForm SearchMatchResultsForm
        {
            get
            {
                // Find the first TSearchMatchForm in the structure's form list
                TObjBasePredicate predicate = new TObjBasePredicate(typeof(Reino.ClientConfig.TSearchMatchForm));
                return IssueStruct.Forms.Find(predicate.CompareByClassType) as TSearchMatchForm;
            }
        }

        public TTTable GetMatchRecsTable()
        {

            // Get the match form 
            TSearchMatchForm loMatchForm;
            string loTableFieldName = "";
            if ((loMatchForm = SearchMatchResultsForm) == null) return null;

            TTTable loTable = null;
            if (this.MatchRecs.ListName != "")
            {
                // Try to split the ListName property into two elements that together
                // specify a Table and Column for the list's source.
                string[] ListNameElements = this.MatchRecs.ListName.Split(' ');
                if (ListNameElements.Length == 2)
                {
                    //loTable = IssFormBuilder.GetListTableByName(ListNameElements[0], ListSearchType.Get1st);
                    loTable = SearchStructLogicAndroid.GetListTableByName(ListNameElements[0], ListSearchType.Get1st);
                    loTableFieldName = ListNameElements[1];
                }
            }
            // Return the resultant table
            return loTable;
        }

        public TEditListBox MatchRecs
        {
            get
            {
                return FindMatchRecsObj(SearchMatchResultsForm);
            }
        }

        public Reino.ClientConfig.TIssForm GetIssueForm()
        {
            // Find the first TIssForm in the structure's form list
            TObjBasePredicate predicate = new TObjBasePredicate(typeof(Reino.ClientConfig.TIssForm));
            return IssueStruct.Forms.Find(predicate.CompareByClassType) as TIssForm;
        }


        /// <summary>
        /// Returns TRUE if the config is completely setup for an optional action based on search results
        /// </summary>
        /// <returns></returns>
        private bool fHasDefinedSearchResultAction = false;
        public bool HasDefinedSearhResultAction()
        {
            return fHasDefinedSearchResultAction;
        }

        private bool ValidateSearchResultActionSetup()
        {
            // The name of the associated citation structure is in the "IssueStruct" property
            if (((TSearchStruct)(this.IssueStruct)).IssueStruct == "")
                return false; // can't issue a ticket if we don't have a ticket struct 

            // Get the citation structure by name
            TIssStruct CiteStruct = GetIssueStructByName(((TSearchStruct)(this.IssueStruct)).IssueStruct);
            if (CiteStruct == null)
                return false; // can't issue a ticket if we don't have a ticket struct 

            // Now make sure we have an issuance form and form logic to work with
            IssueStructLogicAndroid CiteLogic = CiteStruct.StructLogicObj as IssueStructLogicAndroid;
            if (CiteLogic.IssueFormLogic == null)  // || (CiteLogic.IssueFormLogic.CfgForm == null))
            {
                return false;
            }


            // so we have passed all the evaluations 
            return true;

        }

        #endregion

        #region Protected Methods
        //protected TTEdit FindCfgEditByName(TTControl SearchObj, string FieldName)
        //{
        //    // If names match then return object
        //    if (string.Compare(SearchObj.Name, FieldName, true) == 0)
        //        return SearchObj as TTEdit;

        //    TTEdit loResult = null;

        //    // If its a "TabSheet" container, then recurse items contained in the "Sheets" collection
        //    if (SearchObj is TTTabSheet)
        //    {
        //        foreach (TTControl NextCtrl in ((TTTabSheet)(SearchObj)).Sheets)
        //        {
        //            loResult = FindCfgEditByName(NextCtrl, FieldName);
        //            if (loResult != null)
        //                return loResult;
        //        }
        //    }

        //    // If its a "Panel" container, then recurse items contained in the "Controls" collection
        //    if (SearchObj is TTPanel)
        //    {
        //        foreach (TTControl NextCtrl in ((TTPanel)(SearchObj)).Controls)
        //        {
        //            loResult = FindCfgEditByName(NextCtrl, FieldName);
        //            if (loResult != null)
        //                return loResult;
        //        }
        //    }
        //    return null;
        //}
        #endregion

        #region Private Methods


        /// <summary>
        /// 
        /// Async Task to retrieve records matching the search criteria using SQL
        /// 
        /// </summary>
        /// <returns></returns>
        public void privPerformSearchPrim(SearchParameterPacket ioSearchResult, ref bool ioWirelessHostAvailable)
        {
            var queryStringBuilder = new System.Text.StringBuilder();
            queryStringBuilder.Append(" SELECT * FROM " + _SearchTableName);

            queryStringBuilder.Append(" WHERE ");

            // build up both SQL and web search filters
            _SearchFilterWeb.Clear();

            int loParameterCount = 0;
            foreach (string oneFilter in _SearchFilter.Keys)
            {
                if (loParameterCount > 0)
                {
                    queryStringBuilder.Append(" AND ");
                }


                string loOneFilterValue = GetSearchFilterFieldValue(oneFilter);
                if (string.IsNullOrEmpty(loOneFilterValue) == false)
                {
                    // non empty, do standard match
                    queryStringBuilder.Append(oneFilter);
                    queryStringBuilder.Append(" = '");
                    queryStringBuilder.Append(loOneFilterValue);
                    queryStringBuilder.Append("'");
                }
                else
                {
                    // empty, so check against '' or NULL 
                    queryStringBuilder.Append( "((" );

                    queryStringBuilder.Append(oneFilter);
                    queryStringBuilder.Append(" = '");
                    queryStringBuilder.Append(loOneFilterValue);
                    queryStringBuilder.Append("'");


                    queryStringBuilder.Append(") OR (" + oneFilter + " IS NULL)" );


                    queryStringBuilder.Append(")");
                }





                // add TAB between parameter pair
                if (loParameterCount > 0)
                {
                    _SearchFilterWeb.Append("\t");
                }

                _SearchFilterWeb.Append(oneFilter);
                _SearchFilterWeb.Append("=");
                _SearchFilterWeb.Append(GetSearchFilterFieldValue(oneFilter));


                loParameterCount++;
            }



            // AJW - for review - newest data displayed on top? what is ORDER by on search results?
            //queryStringBuilder.Append(" ASC ");
            //queryStringBuilder.Append(" DESC ");


            //List<string> _tablesNames = new List<string>();
            //_tablesNames.Add(_SearchTableName);
            //result = commonADO.GetTableRows(_tablesNames, queryStringBuilder.ToString());



            // is this a wireless search?
            ioWirelessHostAvailable = ((this.IssueStruct as TSearchStruct).WirelessSearchEnabled == TSearchStruct.TWirelessEnabledType.wlWhenAvailable);

            //// this is Android, we are always wireless?
            //ioWirelessHostAvailable = true;

            ioSearchResult.fSearchResultDTOList.Clear();


            if (ioWirelessHostAvailable == false)
            {
                try
                {

                    var loResultRows = (new DatabaseManager()).GetAllFromTableWithColumnValue(queryStringBuilder.ToString());
                    foreach (DataRow row in loResultRows.Rows)
                    {
                        var oneSearchMatchDTO = new SearchMatchDTO();
                        oneSearchMatchDTO.structName = this.IssueStruct.Name;

                        oneSearchMatchDTO.rawRow = row;
                        oneSearchMatchDTO.fSearchStructLogic = this;

                        //oneSearchMatchDTO.parkingStatus = Helper.GetSafeColumnStringValueFromDataRow(row, Constants.STATUS_COLUMN);

                        // populated - add to the list
                        ioSearchResult.fSearchResultDTOList.Add(oneSearchMatchDTO);
                    }


                    //return commonDTOList;
                }
                catch (System.Exception e)
                {
                    //LoggingManager.LogApplicationError(e, "tableName: " + tableName, "GetTableRows");
                    Console.WriteLine("Exception source: {0}", e.Source);
                }
            }
            else
            {
                // launch a wireless search
                try
                {

                    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(DroidContext.ApplicationContext);

                    string iOfficerName = prefs.GetString(Constants.OFFICER_NAME, null);
                    string iOfficerID = prefs.GetString(Constants.OFFICER_ID, null);

                    string iSerialNo = Constants.SERIAL_NUMBER;
                    string iSearchStructName = IssueStruct.Name;
                    string iSearchParams = _SearchFilterWeb.ToString();
                    string oErrMsg = string.Empty;

                    // this is were we will go if any hits show up
                    string loSearchMatchFragmentTagName = Helper.BuildSearchMatchFragmentTag(iSearchStructName);



                    // for the wireless search results, we need a DataTable of the matching the search structure
                    // that we can populate with the results from the web service
                    // the easiest way to get one of the correct format is to execute a dummy search
                    var loDummyQueryString = " SELECT * FROM " + _SearchTableName + " ORDER BY ROWID ASC LIMIT 1";
                    myDummyResultTable = (new DatabaseManager()).GetAllFromTableWithColumnValue(loDummyQueryString);
                    


                    mySearchService = new SearchStructServerInterface();

                    myTimer = new SearchResultEvaluationTimer(this);
                    myTimer.Run();

                    // go web go!
                    mySearchService.CallHotSheetSearchWebService( iSerialNo, iOfficerName, iOfficerID, 
                                                                  _CallingSearchStruct, iSearchStructName, iSearchParams, loSearchMatchFragmentTagName,
                                                                  _MinMatchCount, _CalledFromSearchAndIssue, _InitiatingEditRestriction );

                }
                catch (System.Exception e)
                {
                    //LoggingManager.LogApplicationError(e, "tableName: " + tableName, "GetTableRows");
                    Console.WriteLine("Exception source: {0}", e.Source);
                }
            }

      
        }
    





        private void GetMatchFieldList(string iMatchFieldsName, ref List<string> MatchList)
        {
            string[] FieldNames = null;
            if (iMatchFieldsName == "")
            {
                FieldNames = ((TSearchStruct)(IssueStruct)).MatchFields.Split(' ');
                foreach (string FieldName in FieldNames)
                {
                    //MatchList.Add(FieldName);
                    MatchList.Add(FieldName.ToUpper());
                }
                return;
            }

            for (int loIdx = 0; loIdx < ((TSearchStruct)(IssueStruct)).AddlMatchFields.Count; loIdx++)
            {
                FieldNames = ((TSearchStruct)(IssueStruct)).AddlMatchFields[loIdx].Split(' ');
                foreach (string FieldName in FieldNames)
                {
                    //MatchList.Add(FieldName);
                    MatchList.Add(FieldName.ToUpper()); // ANDROID - not sure this is needed
                }

            }
            return;
        }

        private TEditListBox FindMatchRecsObj(TTControl SearchObj)
        {
            if (SearchObj is TEditListBox)
                return SearchObj as TEditListBox;

            if (SearchObj is TTPanel)
            {
                TEditListBox loResult = null;
                foreach (TTControl NextCtrl in ((TTPanel)(SearchObj)).Controls)
                {
                    loResult = FindMatchRecsObj(NextCtrl);
                    if (loResult != null)
                        return loResult;
                }
            }
            return null;
        }

        private bool FilterAreTheSame(Hashtable FilterList1, Hashtable FilterList2)
        {
            // Take care of simple comparisons first
            if ((FilterList1 == null) && (FilterList2 == null)) return true;
            if ((FilterList1 != null) && (FilterList2 == null)) return false;
            if ((FilterList1 == null) && (FilterList2 != null)) return false;
            if (FilterList1.Count != FilterList2.Count) return false;

            // If we get here, then both objects exist and have the same number of filters,
            // so now we can compare each one
            foreach (DictionaryEntry oneFilter in FilterList1)
            {
                if (FilterList2.Contains(oneFilter.Key) == true)
                {
                    string loFilter1 = FilterList1[oneFilter.Key].ToString();
                    string loFilter2 = FilterList2[oneFilter.Key].ToString();
                    if (loFilter1.Equals(loFilter2) == false)
                    {
                        return false;
                    }
                }
            }


            // At this point the filters are the same
            return true;

            //// Take care of simple comparisons first
            //if ((FilterList1 == null) && (FilterList2 == null)) return true;
            //if ((FilterList1 != null) && (FilterList2 == null)) return false;
            //if ((FilterList1 == null) && (FilterList2 != null)) return false;
            //if (FilterList1.Filters.Count != FilterList2.Filters.Count) return false;

            //// If we get here, then both objects exist and have the same number of filters,
            //// so now we can compare each one
            //TFilter Filter1 = null;
            //TFilter Filter2 = null;
            //for (int loIdx = 0; loIdx < FilterList1.Filters.Count; loIdx++)
            //{
            //    Filter1 = FilterList1.Filters[loIdx];
            //    Filter2 = FilterList2.Filters[loIdx];
            //    if (Filter1.fFieldNo != Filter2.fFieldNo)
            //        return false;
            //    if (Filter1.fFilterValue != Filter2.fFilterValue)
            //        return false;
            //}
            //// At this point the filters are the same
            //return true;
        }



        // used for the table search
        private string _SearchTableName = string.Empty;
        private Hashtable _SearchFilter = new Hashtable();
        private System.Text.StringBuilder _SearchFilterWeb = new System.Text.StringBuilder();

        private string GetSearchFilterFieldValue(string iFieldName)
        {
            // return if no current global value for this field
            if (_SearchFilter.ContainsKey(iFieldName) == false)
                return "";
            else
                return _SearchFilter[iFieldName].ToString();
        }

        private void SetSearchFilterFieldValue(string iFieldName, string iFieldValue)
        {
            // Add or update the value for the passed field
            if (_SearchFilter.ContainsKey(iFieldName) == false)
                _SearchFilter.Add(iFieldName, iFieldValue);
            else
                _SearchFilter[iFieldName] = iFieldValue;
        }

        private void ClearSearchFilter()
        {
            _SearchFilter.Clear();
        }


        /// <summary>
        /// Searches the table for the records that match the current ISSUE form's
        /// fMatchFields values.
        /// </summary>
        private SearchParameterPacket privPerformSearch(FormPanel iFromForm, bool iOnlyOnNewFilters, short iMinMatchCount,
                                                        string iMatchFieldsName, bool iCalledFromSearchAndIssue, 
                                                        Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditDependency iActiveEditRestriction,
                                                        ref bool ioWirelessHostAvailable, bool DisablePaintingSearchScreenWhenClosing)
        {
            // init empty result
            SearchParameterPacket loSearchParams = new SearchParameterPacket();
            loSearchParams.fCalledFromSearchAndIssue = iCalledFromSearchAndIssue;
            loSearchParams.fInitiatingEditRestriction = iActiveEditRestriction;
            loSearchParams.fMinMatchCount = iMinMatchCount;
            
            short loMatchFldNdx;
            TSearchMatchForm loMatchForm;
            PanelField loIssEdit;
            //TTTable loTable = null;
            //string loTableFieldName = "";
            //TFilterList loSavedFilters = null;
            //AppMessageBox loMsgForm = null;

            _MinMatchCount = iMinMatchCount;
            
            if ( IssueStruct is TSearchStruct )
            {
              _CallingSearchStruct = (TSearchStruct)IssueStruct;
              loSearchParams.fSearchStruct = _CallingSearchStruct;
            }

            _CalledFromSearchAndIssue = iCalledFromSearchAndIssue;
            _InitiatingEditRestriction = iActiveEditRestriction;


            List<string> loMatchFieldList = new List<string>();

            // Get the match form 
            loMatchForm = SearchMatchResultsForm;
            if (loMatchForm == null)
            {
                //AppMessageBox.ShowMessageWithBell("Error: " + IssueStruct.Name + " missing SELECT form", "", "");
                //return null;
                return loSearchParams;
            }

            // Match form had better have fMatchRecs 
            GetMatchFieldList(iMatchFieldsName, ref loMatchFieldList);
            if (loMatchFieldList.Count == 0)
            {
                //AppMessageBox.ShowMessageWithBell("Missing Match Field List: " + iMatchFieldsName, "", "");
                //return null;
                return loSearchParams;
            }

            if (this.MatchRecs == null)
            {
                //AppMessageBox.ShowMessageWithBell("Error: " + IssueStruct.Name + " SELECT missing fMatchRecs", "", "");
                //return null;
                return loSearchParams;
            }

            // This code is not re-entrant, so have to defend against wireless search results from
            //   trampling our state.

            // However, wireless search results are processed in our thread (via the message handler loop),
            //   so we can be assured that it won't execute at any random moment, only when we process
            //   messages.

            SearchStructLogicAndroid.unSearchEvaluateInProcess = true;
            _SearchTableName = string.Empty;

            // get the table for easier access 
            if (this.MatchRecs.ListName != "")
            {
                // Try to split the ListName property into two elements that together
                // specify a Table and Column for the list's source.
                string[] ListNameElements = this.MatchRecs.ListName.ToUpper().Split(' ');
                if (ListNameElements.Length == 2)
                {
                    //loTable = IssFormBuilder.GetListTableByName(ListNameElements[0], ListSearchType.Get1st);
                    //loTable = null; // endless loop  SearchStructLogicAndroid.GetListTableByName(ListNameElements[0], ListSearchType.Get1st);
                    //loTableFieldName = ListNameElements[1];
                }

                _SearchTableName = ListNameElements[0];
            }



            // We can't do a search if no table could be found
            //if (loTable == null)
            if (string.IsNullOrEmpty(_SearchTableName) == true)
            {
                SearchStructLogicAndroid.unSearchEvaluateInProcess = false;
                //return null;
                return loSearchParams;
            }

            // Make copy of existing filters 
            Hashtable loSavedFilters = null;
            if (iOnlyOnNewFilters)
            {
                loSavedFilters = new Hashtable(_SearchFilter);
                

                //// Get list of existing filters
                //TFilterList ExistingFilterList = loTable.GetFilter();
                //if (ExistingFilterList != null)
                //{
                //    // Create a copy of the existing filters
                //    if (loSavedFilters == null)
                //        loSavedFilters = new TFilterList();
                //    loSavedFilters.Filters.Clear();
                //    foreach (TFilter NextFilter in ExistingFilterList.Filters)
                //    {
                //        TFilter ClonedFilter = new TFilter();
                //        ClonedFilter.fFieldNo = NextFilter.fFieldNo;
                //        ClonedFilter.fFilterValue = NextFilter.fFilterValue;
                //        loSavedFilters.Filters.Add(ClonedFilter);
                //    }
                //}
            }


            //// We need to get the comparison field values now because RemoveAllFilters will wipe out the field values    
            //if (loTable.fComparisonFieldValues == null)
            //    loTable.fComparisonFieldValues = new List<string>();
            //loTable.fComparisonFieldValues.Clear();
            //foreach (string loFldValue in IssueStruct.MainTable.HighTableRevision.Tables[0].fFieldValues)
            //    loTable.fComparisonFieldValues.Add(loFldValue);

            //// Clear out pre-existing filter 
            //loTable.RemoveAllFilters();

            //// Suspend the filter so that each AddFilter doesn't perform a new search 
            //loTable.SuspendFilters();


            ClearSearchFilter();
            loSearchParams.FiltersForInitiatingRestriction.Clear();


            // Acquire the filter field values from the issue form 
            for (loMatchFldNdx = 0; loMatchFldNdx < loMatchFieldList.Count; loMatchFldNdx++)
            {

                //loIssEdit = FindCfgEditByName(iFromForm, loMatchFieldList[loMatchFldNdx]);
                //if (loIssEdit == null)
                //    continue;  // match field doesn't exist, don't use it as a filter 

                //// Add this field and value as a filter 
                //if (loIssEdit.Behavior.FieldIsBlank())
                //    loTable.AddFilter(loIssEdit.Name, "", loIssEdit.Behavior.GetEditMask());
                //else
                //    loTable.AddFilter(loIssEdit.Name, loIssEdit.Behavior.EditBuffer, loIssEdit.Behavior.GetEditMask());


                loIssEdit = iFromForm.GetCfgCtrlByName(loMatchFieldList[loMatchFldNdx]);
                if (loIssEdit == null)
                {
                    continue;  // match field doesn't exist, don't use it as a filter 
                }


                EditControlBehavior uiComponentBehavior = Helper.GetBehaviorAndroidFromUIComponent(loIssEdit.uiComponent);

                if (uiComponentBehavior != null)
                {

                    string iFieldFilterValue = string.Empty;

                    if (uiComponentBehavior.FieldIsBlank() == false )
                    {
                        iFieldFilterValue = uiComponentBehavior.EditBuffer;
                    }

                    // remove abbrev descriptions when present - these are often displayed on entry screen
                    int loPosSep = iFieldFilterValue.IndexOf(Constants.LIST_ITEM_DESCRIPTION_SEPARATOR);
                    // has to be beyond the first char
                    if (loPosSep > 0)
                    {
                        // keep everything up to the space preceeding the seperator char
                        iFieldFilterValue = iFieldFilterValue.Substring(0, loPosSep - 1);
                    }


                    // Add this field and value as a filter 
                    SetSearchFilterFieldValue(loIssEdit.Name, iFieldFilterValue);


                    // update the filter list used by the edit restriction
                    var oneListFilterForInitiatingRestriction = new ListFilter
                    {
                        Column = loIssEdit.Name,
                        Value = iFieldFilterValue,
                        Index = -1,
                        FilterByIndex = false,
                        ParentBehavior = uiComponentBehavior
                    };

                    // TODO - make sure this isn't a dup column ref??
                    loSearchParams.FiltersForInitiatingRestriction.Add(oneListFilterForInitiatingRestriction);


                }

            }


            // If the new filters are the same as the old, and we only want new stuff, then we're done here
            if (iOnlyOnNewFilters == true)
            {
                if (FilterAreTheSame(loSavedFilters, _SearchFilter) == true)
                {
                    SearchStructLogicAndroid.unSearchEvaluateInProcess = false;
                    return loSearchParams;
                }
            }


             privPerformSearchPrim(loSearchParams, ref ioWirelessHostAvailable);



            if (ioWirelessHostAvailable == true)
            {
                // the wireless search is proceeding, exit now
                ioWirelessHostAvailable = true;
                SearchStructLogicAndroid.unSearchEvaluateInProcess = false;
                //return null;
                return loSearchParams;
            }


#if _original_code_base_

#if _original_
            // If the new filters are the same as the old, restore the old 
            if ((iOnlyOnNewFilters) && (loSavedFilters != null) && (FilterAreTheSame(loSavedFilters, loTable.GetFilter())))
            {
                //loTable.CopyFilters(loSavedFilters); // DEBUG --- this isn't necessary is it?
                loSavedFilters.Filters.Clear();
                loSavedFilters = null;
                SearchStructLogicAndroid.unSearchEvaluateInProcess = false;
                return null;
            }
#endif

            // Don't do wireless stuff for emulator anymore
            bool DontWantWireless = false;
            //#if !WindowsCE
            //            DontWantWireless = true;
            //#endif

            // is this a table we want to do a realtime search via winsock?
            if ((DontWantWireless == false) &&
                ((this.IssueStruct as TSearchStruct).WirelessSearchEnabled == TSearchStruct.TWirelessEnabledType.wlWhenAvailable))
            {
                string iMatchFldName = "";
                string iMatchValue = "";
                string iMatchMask = "";
                bool iPartialMatch = false;
                int loMatchBufLen = 0;
                int loWirelessMatchFldNdx = 0;
                bool loMatchFldIsKey = false;
                TTTable.TSearchType loSearchType;

                StringBuilder loMatchBuf = new StringBuilder();
                StringBuilder loSearchParamFieldNameList = new StringBuilder();

                // Since we are doing a new search, clear out any pending wireless searches
                // that are on this structure. Those results are no longer relevant.
                CleanUpPendingWirelessSearches(iFromForm, ((TSearchStruct)(this.IssueStruct)), true);

                // 1) lock the send & receive queues so nothing new gets added.
                // 2) Search the send queue for any searches on this structure. Remove them.
                // 3) Search the receive queue for any search results on this structure. Remove them.
                // 4) unlock the send & receive queues.

                // ask the filter code to build us a match buffer
                loSearchType = loTable.BuildSearchParams(
                    "", "", "", false, ref loMatchBuf,
                    ref loMatchFldIsKey, ref loWirelessMatchFldNdx,
                    ref loSearchParamFieldNameList);

                // Before trying to send the command, find out if we're connected
                bool loRFIsConnected = true;
#if WindowsCE
                loRFIsConnected = IssueAppImp.glWirelessQueue.ModemMgr.CommMgrIsConnected();
#endif

                // it should have built a match buffer- if it didn't, don't bother
                if (loMatchBuf != null)
                {
#if _original_
                    // create and populate a search record to pass to the wireless que
                    TAPDIHotSheetSearchCommandRec loAPDIClientCommandRec =
                      new TAPDIHotSheetSearchCommandRec((TSearchStruct)(this.IssueStruct),
                      iCalledFromSearchAndIssue, iMinMatchCount, iActiveEditRestriction,
                      iFromForm, loMatchBuf.ToString(), loSearchParamFieldNameList.ToString());

                    // make sure we have all the values we need from the "real"
                    fWirelessSearchResultTable.fComparisonFieldValues = this.IssueStruct.MainTable.HighTableRevision.Tables[0].GetFieldValuesPtr();

                    // send the request via Webservices
                    IssueAppImp.glWirelessQueue.CE_APDI_PutCommandInSendQue(loAPDIClientCommandRec);
#endif

                    // clean up the temp match buffer
                    loMatchBuf.Length = 0;
                    loMatchBuf = null;
                }

                // is the RF connection working right now?
                /*
                if (IssueAppImp.glWirelessQueue.ModemMgr.CommMgrIsConnected() == true)
                */
                if (loRFIsConnected == true)
                {
                    // the wireless search is proceeding, exit now
                    ioWirelessHostAvailable = true;
                    SearchStructLogicAndroid.unSearchEvaluateInProcess = false;
                    return null;
                }

#if _original_
                // the RF isn't available, fall through to a local search
                loMsgForm = AppMessageBox.ShowCenteredMessageNotModal("RF Host Not Available", "Doing Local Search..", null);
#endif
            }
            else
            {
#if _original_
                // activating the filters will cause table stats to reflect the records passing the filter 
                // JLA: Debug -- experiment for flickering caused by HotSheet search
                loMsgForm = AppMessageBox.ShowCenteredMessageNotModal("Searching...", "", null);
#endif

                // JLA: This could alternatively be done to show more info and refresh itself
                /*
                // Explicitly allow drawing of main window to ensure this message box will be shown
                IPlatformDependent WinAPI = null;
                if (Environment.OSVersion.Platform == PlatformID.WinCE)
                    WinAPI = new WinCEAPI();
                else
                    WinAPI = new Win32API();
                WinAPI.SendMessage(IssueAppImp.glMainMenuForm.Handle, 0x000B, 1, 0); 
                loMsgForm = AppMessageBox.ShowCenteredMessageNotModal("Searching " + IssueStruct.Name + "...", "", null);
                loMsgForm.Refresh();
                */
            }
#if !WindowsCE
            // For full framework, we'll put in a pause so user has a chance to see whats happening
            Thread.Sleep(10);
#endif
#if _original_
            loTable.ActivateFilters();
#endif

            // JLA: Attempt to reduce flicker caused by HotSheet searches
            if (DisablePaintingSearchScreenWhenClosing == true)
            {

#if _original_
                IPlatformDependent WinAPI = null;
                if (Environment.OSVersion.Platform == PlatformID.WinCE)
                    WinAPI = new WinCEAPI();
                else
                    WinAPI = new Win32API();

                // Lets disable painting for the message box to help avoid painting it and parent
                // when we close it.
                WinAPI.SendMessage(loMsgForm.Handle, 0x000B /*WM_SETREDRAW*/, 0, 0);

                // Prevent painting of main window. It will be re-enabled by OkToExit() in ReinoTextBoxBehavior.cs.
                // First we need to set static TextBoxBehavior.InvalidatedWindowAfterEditRestrictions equal to our 
                // main window since this window is application specific and TextBoxBehavior is in a seperate DLL.
                TextBoxBehavior.InvalidatedWindowAfterEditRestrictions = IssueAppImp.glMainMenuForm;
                WinAPI.SendMessage(IssueAppImp.glMainMenuForm.Handle, 0x000B /*WM_SETREDRAW*/, 0, 0); 
#endif
            }



#if _original_
            IssueAppImp.glMainMenuForm.SuspendLayout();
            if (loMsgForm != null)
                loMsgForm.Close();
            if (loMsgForm != null)
                loMsgForm.Dispose();
            IssueAppImp.glMainMenuForm.ResumeLayout();
#endif
            // the wireless host isn't available. Depending on the
            // hardware platform AND the table configuration, an inquiry
            // may have been added to the que. If we've gotten to this
            // point in the code, there is no wireless host, because
            //  a) the RF isn't connecting  -OR-
            //  b) this table isn't configured to do a wireless search -OR-
            //  c) the hardware platform doesn't support it
            ioWirelessHostAvailable = false;


#endif

            // this is a local file search, evaluate the results now
            EvaluateSearchResult(loSearchParams, false);


            // mcb 7/27/04: clear our mutex so wireless searches can return their results.
            SearchStructLogicAndroid.unSearchEvaluateInProcess = false;
            return loSearchParams;
        }


        /// <summary>
        /// The second half of the search and issue routines, this method
        /// calls the issue form if defined and required
        /// This was split off from PerformSearchAndIssue such that it could be executed
        /// either immediately after a local file search or later when a 
        /// wireless search result is returned after a delay
        /// </summary>
        private bool EvaluateSearchAndIssueResult(SearchParameterPacket iSearchResult )
        {

            // Assume we are doing normal logic. If IntParam2 of the editrestriction is set, then
            // we will use reverse logic (Currently hard-coded for GeoCode hotsheet, i.e. Seattle demo)
            bool ShowResultOnlyOnNoMatch = false;
            if ((iSearchResult.fInitiatingEditRestriction != null) && (iSearchResult.fInitiatingEditRestriction.IntParam2 == 1))
            {
                ShowResultOnlyOnNoMatch = true;
                // If result table is empty, we want a message balloon shown to user indicating failure.
                // To do this, we simply need to return true so the initiating EditRestricion also
                // returns true for its EnforceRestriction call.
                if (iSearchResult.fSearchResultDTOList.Count == 0)
                //if (iSearchResult.fSearchMatchDTOList == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // no result table? that means nothing matched
            if (iSearchResult.fSearchResultDTOList == null) 
            {
                return false;
            }

            // empty list? that means nothing matched
            if (iSearchResult.fSearchResultDTOList.Count == 0)
            {
                return false;
            }


            // where are we coming from?
            //string loCurrentFragmentTag = ((TSearchStruct)(this.IssueStruct)).IssueStruct


            // The name of the associated citation structure is in the "IssueStruct" property
            if (((TSearchStruct)(this.IssueStruct)).IssueStruct == "")
                return false; // can't issue a ticket if we don't have a ticket struct 

            // Get the citation structure by name
            TIssStruct CiteStruct = GetIssueStructByName(((TSearchStruct)(this.IssueStruct)).IssueStruct);
            if (CiteStruct == null)
                return false; // can't issue a ticket if we don't have a ticket struct 

            // Now make sure we have an issuance form and form logic to work with
            IssueStructLogicAndroid CiteLogic = CiteStruct.StructLogicObj as IssueStructLogicAndroid;
            if (CiteLogic.IssueFormLogic == null)  // || (CiteLogic.IssueFormLogic.CfgForm == null))
            {
                return false;
            }


            // get the target fragment tag
            string loTargetFragmentTag = CiteLogic.IssueForm1LogicFragmentTag;
            //string loTargetFragmentTag = Helper.BuildIssueNewFragmentTag(CiteStruct.Name);


            // grab the mutex.
            SearchStructLogicAndroid.unSearchEvaluateInProcess = true;

            SetIssueSource(CiteLogic.IssueFormLogic, iSearchResult );

            // HotDispo will go to entry screen as exception. Others will go to navigation screen
            if (CiteLogic.IssueStruct is THotDispoStruct)
            {
                //CiteLogic.IssueFormLogic.StartPanel = CiteLogic.IssueFormLogic.EntryPanel;
            }
            else
            {
                //CiteLogic.IssueFormLogic.StartPanel = CiteLogic.IssueFormLogic.NavigationPanel;
            }

                        

            CiteLogic.IssueRecord(Reino.ClientConfig.EditRestrictionConsts.femSingleEntry, ((TSearchStruct)(IssueStruct)).IssueStructFirstFocus, "", loTargetFragmentTag );


            // this is now done in formpanel when they complete the record
            //UndoIssueSource(CiteLogic.IssueFormLogic);

            //// release the mutex
            //SearchStructLogicAndroid.unSearchEvaluateInProcess = false; //  true; // DEBUG -- This is true in C++ source?

            return false;
        }



        private void EvaluateSearchResult(SearchParameterPacket ioSearchResult, bool iWirelessResult)
        {
            TSearchMatchForm loSearchMatchResultsForm;
            TTTable loSavedTable = null;
            TTTable loResultTable = null;
            int loFormEditResult;

            // Get the match form 
            loSearchMatchResultsForm = SearchMatchResultsForm;
            if (loSearchMatchResultsForm == null)
            {
                //AppMessageBox.ShowMessageWithBell("Error: " + IssueStruct.Name + " missing SELECT form", "", "");
                //return null;
                return;
            }

            if (this.MatchRecs == null)
            {
                //AppMessageBox.ShowMessageWithBell("Error: " + IssueStruct.Name + " SELECT missing fMatchRecs", "", "");
                //return null;
                return;
            }

            // Get enough matches to display a result?
            if (ioSearchResult.fSearchResultDTOList.Count < ioSearchResult.fMinMatchCount)
            {
                //return null;
                return;
            }

            // DEBUG -- Need to Implement
            /*
            // if the match form has a place for it, we'll display local vs. wireless results
            if (loMatchForm.fLocalOrWirelessCaption)
            {
                if (iWirelessResult)
                {
                    loMatchForm.fLocalOrWirelessCaption.SetText("Wireless Search Result", sto_ReplaceText, 0);
                }
                else
                {
                    string loDisplayStr = "";
                    string loTimeStr = "";
                    string loDateStr = "";
                    ReinoControls.TextBoxBehavior.OSDateToDateString(iResultTable.fTableDef.fDBCreationDate, "dd-MON-yyyy", ref loDateStr);
                    ReinoControls.TextBoxBehavior.OSTimeToTimeString(iResultTable.fTableDef.fDBCreationTime, "hh:mmtt", ref loTimeStr);
                    loDisplayStr = string.Format("Searched local file dated: {0:s}", loDateStr);
                    loMatchForm.fLocalOrWirelessCaption.SetText(loDisplayStr, sto_ReplaceText, 0);
                }

                loMatchForm.fLocalOrWirelessCaption.SetVisible(1);
                loMatchForm.fLocalOrWirelessCaption.SetFont(Font12x12);
                loMatchForm.fLocalOrWirelessCaption.SetFont(Font12x12);
                loMatchForm.fLocalOrWirelessCaption.SetColors(AI_COLOR_ACTIVETEXT, AI_COLOR_WINDOWBACKGROUND);
            }
            */


#if _original_
            // Is the passed result table different than the one in the match form?
            loSavedTable = null;
            if (iResultList != MatchRecs.Behavior.ListSourceTable)
            {
                // It could be if this is a wireless search result passing a temp table
                loSavedTable = MatchRecs.Behavior.ListSourceTable;
            }
            // Now update the virtual list box
            MatchRecs.Behavior.ListSourceTable = iResultList;
            MatchRecs.Behavior.ListItemCount = iResultList.GetRecCount();
            if (MatchRecs.WinCtrl != null)
            {
                ((ReinoVirtualListBox)(MatchRecs.WinCtrl)).RefreshItems(false);
                if (((ReinoVirtualListBox)(MatchRecs.WinCtrl)).Count > 0)
                    ((ReinoVirtualListBox)(MatchRecs.WinCtrl)).SelectedIndex = 0;
            }
#endif


            // Found something, ring bell twice 
            //TextBoxBehavior.RingBell(null);
            //Thread.Sleep(300);
            //TextBoxBehavior.RingBell(null);


            // When showing a match, we want to start the user at a certain spot
#if _original_
            this.SelectFormLogic.StartPanel = this.SelectFormLogic.EntryPanel;
            this.SelectFormLogic.StartField = MatchRecs;
            loFormEditResult = this.SelectFormLogic.FormEdit(EditRestrictionConsts.femNewEntry, null, null);
#endif

            fSearchAndIssueResult = ioSearchResult;
            CallingFormPanel.LoadSearchMatchFragment(loSearchMatchResultsForm, ioSearchResult);


#if _no_more_

            // Did they just back out?
            if (loFormEditResult != FormEditResults.FormEditResult_OK)
                loResultTable = null;
            else
            {
                // DEBUG -- Set the table to match the selected index
                if ((MatchRecs.WinCtrl != null) && (MatchRecs.WinCtrl is ReinoVirtualListBox))
                {
                    MatchRecs.Behavior.SetListNdxAndPaint((MatchRecs.WinCtrl as ReinoVirtualListBox).SelectedIndex);
                }

                if (fCopyDataFromResult)
                {
                    iResultList.ReadRecord(MatchRecs.Behavior.GetListNdx());
                    loResultTable = iResultList;
                }
                else
                {
                    loResultTable = ((TSearchStruct)(IssueStruct)).MainTable.HighTableRevision.Tables[0];
                }
            }

            // Restore the original table ptr if it was swapped
            if (loSavedTable != null)
            {
                MatchRecs.Behavior.ListSourceTable = loSavedTable;
                MatchRecs.Behavior.ListItemCount = loSavedTable.GetRecCount();
                if (MatchRecs.WinCtrl != null)
                    ((ReinoVirtualListBox)(MatchRecs.WinCtrl)).RefreshItems(false);
            }

            // Return the result
            return loResultTable;
#else
            return;
#endif

        }

        /// <summary>
        /// Called when user is attempting exit a form
        /// If wireless searches are still pending, the user
        /// is given a choice of abandoning them or continuing to wait
        /// If they choose to wait, the result is -1 and the user 
        /// should be prevented from exiting
        /// </summary>
        private int CleanUpPendingWirelessSearches(FormPanel pSender, TSearchStruct iSearchStruct, bool iQuietly)
        {
#if _original_
            TAPDIClientCommandRec loAPDIClientCommandRec;
            TAPDIHotSheetSearchCommandRec loCmdRec;
            // determine if we have any pending inquiries that need to be tossed
            // have to be careful here. What if we toss it as the result arrives? We will
            // need critical sections...
            bool loWirelessSearchesPending = false;

            // mcb 7/23/04: make this thread safe
            lock (IssueAppImp.glWirelessQueue)
            {
                int loWhichQueueNdx;
                // loop through twice, 1st w/ SendQue, then with receive queue

                // MCB 4/15/2006: deleting the items here is not thread safe.  The WinsockThread does
                // not keep the critical sections while it is invoking a web method.  Therefore,
                // rather than delete items, we will set the "CancelledByUser" flag so that
                // they will either not be sent or their result will be ignored.
                for (loWhichQueueNdx = 1; loWhichQueueNdx >= 0; loWhichQueueNdx--)
                {
                    bool loIsSendQue;
                    if (loWhichQueueNdx == 1)
                        loIsSendQue = true;
                    else
                        loIsSendQue = false;
                    for (int loSendQueNdx = 0; loSendQueNdx < IssueAppImp.glWirelessQueue.GetAPDIQueItemCnt(loIsSendQue); loSendQueNdx++)
                    {
                        // derefernce the ptr
                        loAPDIClientCommandRec = IssueAppImp.glWirelessQueue.GetAPDIQueClientCommandRec(loIsSendQue, loSendQueNdx);
                        if (loAPDIClientCommandRec == null)
                            continue;

                        // only want hotsheetsearch packets. Use our bad RTTI technique.
                        if (loAPDIClientCommandRec.GetCommandNo() != TAPDIClientCommandRec.APDICommand_SearchHotsheet)
                            continue;

                        loCmdRec = (loAPDIClientCommandRec as TAPDIHotSheetSearchCommandRec);

                        // if they supplied a form & the form doesn't match, skip it.
                        if ((pSender != null) && (loCmdRec.GetFromForm() != pSender))
                            continue;

                        // if they supplied a search struct & they don't match, skip it
                        if ((iSearchStruct != null) && (loCmdRec.GetSearchStruct() != iSearchStruct))
                            continue;

                        // OK. This one qualifies. Do we need to get user confirmation?
                        if (!iQuietly && !loWirelessSearchesPending &&
                            AppMessageBox.QueryUser("Wireless Searches Pending!", "Wait for a reply?"))
                        {
                            return -1; // user wants to wait.
                        }
                        // found at least one pending inquiry. If our flag is true, 
                        // then user has already decided to cancel searches.
                        loWirelessSearchesPending = true;
                        loCmdRec.fCancelledByUser = true;
                    }
                }
            }
#endif
            return 0;
        }
        #endregion



        public void UpdateInfoFromWebService()
        {
            // check the async result
            if (mySearchService != null)
            {
                string loText;
                string loConfidenceText;

                if (mySearchService.ResultsAreAvailable() == true)
                {
                    // copy the results
                    _LastResultDataset = mySearchService.GetLastResultDataSet();
                    _LastStatusText = mySearchService.GetLastStatusTextMessage();
                    _LastResultErrMsg = mySearchService.GetLastResultErrorMessage();
                    _LastAsyncResult = mySearchService.GetLastAsycnResult();

                    _CalledFromSearchAndIssue = mySearchService.GetWasCalledFromSearchAndIssue();
                    _MinMatchCount = mySearchService.GetMinMatchCount();
                    _InitiatingEditRestriction = mySearchService.GetInitiatingEditRestriction();
                    _CallingSearchStruct = mySearchService.GetSearchStruct();



                    // thanks, done with you now
                    mySearchService = null;


                    // convert the web service data into DataTable
                    List<SearchMatchDTO> searchMatchDTOList = new List<SearchMatchDTO>();

                    if (_LastResultDataset != null)
                    {
                        if (_LastResultDataset.GetLongLength(0) > 0)
                        {
                            string loLastResultDatasetDeserialized = System.Text.Encoding.ASCII.GetString(_LastResultDataset);
                            Console.WriteLine("Hotsheet Search Result: {0}", loLastResultDatasetDeserialized);



                            // populate the record buffer
                            int loRecLen = 0;
                            for (; ; )
                            {
                                // find the end of this record. 
                                // in this web services era, rows are delimited w/ CRLF. The last row may not have one.
                                loRecLen = loLastResultDatasetDeserialized.IndexOf("\r\n");

                                if ((loRecLen < 0) && (loLastResultDatasetDeserialized.Length == 0))
                                {
                                    break; // no more delimiters means no more records.                
                                }

                                if (loRecLen < 0)
                                {
                                    loRecLen = loLastResultDatasetDeserialized.Length;
                                }

                                string loTempRec = loLastResultDatasetDeserialized.Substring(0, loRecLen);

                                DataRow loOneRow = myDummyResultTable.NewRow();
                                int loColumnIdx = 0;
                                int loColumnsInTableDef = fWirelessSearchResultTable.fTableDef.HighTableRevision.Fields.Count;


                                string[] loOneRowOfColumns = loTempRec.Split( (char)0x09 );
                                foreach (string oneColumnAsString in loOneRowOfColumns)
                                {
                                    bool loColumnFound = false;

                                    while ((loColumnIdx < loColumnsInTableDef) && (loColumnFound == false))
                                    {


                                        string loColumnName = fWirelessSearchResultTable.fTableDef.HighTableRevision.Fields[loColumnIdx].Name;

                                        switch (loColumnName)
                                        {
                                            // there may be data available, but we aren't copying it for certain fields
                                            case AutoISSUE.DBConstants.sqlUniqueKeyStr:
                                            case AutoISSUE.DBConstants.sqlSourceIssueMasterKeyStr:
                                            case AutoISSUE.DBConstants.sqlMasterKeyStr:
                                            case AutoISSUE.DBConstants.sqlOccurenceNumberStr:
                                            case AutoISSUE.DBConstants.ID_COLUMN:
                                            case AutoISSUE.DBConstants.STATUS_COLUMN:
                                            case AutoISSUE.DBConstants.SEQUENCE_ID:
                                            case AutoISSUE.DBConstants.WS_STATUS_COLUMN:
                                            case AutoISSUE.DBConstants.SRCINUSE_FLAG:
                                                {
                                                    // skip it
                                                    loColumnIdx++;
                                                    break;
                                                }
                                            default:
                                                {
                                                    int loDataTableIdx = loOneRow.Table.Columns.IndexOf(loColumnName);
                                                    if (loDataTableIdx != -1)
                                                    {
                                                        loOneRow[loDataTableIdx] = oneColumnAsString;
                                                    }

                                                    // done
                                                    loColumnFound = true;
                                                    loColumnIdx++;
                                                    break;
                                                }
                                        }
                                    }


                                }


                                // populate and add to the list
                                var oneSearchMatchDTO = new SearchMatchDTO();
                                oneSearchMatchDTO.structName = this.IssueStruct.Name;
                                oneSearchMatchDTO.fSearchStructLogic = this;
                                oneSearchMatchDTO.rawRow = loOneRow;

                                searchMatchDTOList.Add(oneSearchMatchDTO);



                                // Remove processed row data from source string
                                loLastResultDatasetDeserialized = loLastResultDatasetDeserialized.Remove(0, loRecLen);
                                // Also remove the CRLF that normally will be left behind
                                if (loLastResultDatasetDeserialized.StartsWith("\r\n"))
                                {
                                    loLastResultDatasetDeserialized = loLastResultDatasetDeserialized.Remove(0, 2);
                                }
                            }


                        }
                    }



                    //EvaluateSearchResult(searchMatchDTOList, 1, true);

                    //HandleWirelessSearchResult(searchMatchDTOList, _MinMatchCount, _CalledFromSearchAndIssue, _InitiatingEditRestriction);
                    
                    SearchParameterPacket loWirelessSearchResult = new SearchParameterPacket();
                    loWirelessSearchResult.fSearchStruct = _CallingSearchStruct;
                    loWirelessSearchResult.fSearchResultDTOList = searchMatchDTOList;
                    loWirelessSearchResult.fCalledFromSearchAndIssue = _CalledFromSearchAndIssue;
                    loWirelessSearchResult.fMinMatchCount = _MinMatchCount;
                    loWirelessSearchResult.fInitiatingEditRestriction = _InitiatingEditRestriction;

                    SearchStructWirelessQueueAndroid.CE_APDI_AddReplyToReceiveQue( loWirelessSearchResult );


                    // let them know the results are ready - TODO - is using this global static a good idea? :~/
                    DroidContext.mainActivity.RunOnUiThread(() =>
                    {
                        EvaluateWirelessSearchResult();
                    });



                    //    if ((IssueAppImp.glFocusForm != null) && (IssueAppImp.glFocusForm.IssueStruct.StructLogicObj != null))
                    //        (IssueAppImp.glFocusForm.IssueStruct.StructLogicObj as IssueStructLogic).EvaluateWirelessSearchResult();



                    //// let the calling fragment know the results are ready
                    //Fragment fm = FragmentManager.FindFragmentByTag(gCallingFragmentTagName);
                    //if (fm != null)
                    //{
                    //    Intent resultIntent = new Intent();
                    //    //resultIntent.PutExtra(AutoISSUE.DBConstants.sqlVehLicNoStr, "");
                    //    fm.OnActivityResult(Constants.ACTIVITY_REQUEST_CODE_HOTSHEET_RESULT, Result.Ok, resultIntent);
                    //}


                }

            }
        }

    }


    public class SearchResultEvaluationTimer : Java.Lang.Object, IRunnable
    {

        int _TimerIntervalMS = 500;

        private readonly SearchStructLogicAndroid _Owner;
        private readonly Handler mHandler = new Handler();

        public SearchResultEvaluationTimer(SearchStructLogicAndroid iOwnerDialog)
        {
            _Owner = iOwnerDialog;
        }
        public void Run()
        {
            mHandler.PostDelayed(UpdateTimer, _TimerIntervalMS);
        }
        private void UpdateTimer()
        {
            if (_Owner != null)
            {
                _Owner.UpdateInfoFromWebService();

                mHandler.PostDelayed(UpdateTimer, _TimerIntervalMS);
            }
        }
    }

}
