using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

using Reino.ClientConfig;


namespace XMLConfig.Common
{
    #region PrintPicGenerator
    public class PrintPicGenerator
    {
        #region Private Members
        private PrintPicFormattingInfo FormattingTable = null;
        private HandheldPaperInfo PaperInfo = null;
        private List<PrintPicFormattingInfo> fFormattingTables = new List<PrintPicFormattingInfo>();
        #endregion

        #region Public Methods
        public PrintPicGenerator()
        {
        }

        public PrintPicGenerator(TIssStruct iSourceStruct, string iDataTableName,
            int iTableRevNum, string iFormRevName, int iFormRevNum)
        {
            GetFormattingTable(iSourceStruct, iDataTableName, iTableRevNum, iFormRevName, iFormRevNum);
        }

        /// <summary>
        /// Returns a 2-color bitmap representing the ticket image. This makes a smaller file than a .JPG format
        /// </summary>
        /// <param name="iSourceDataSet"></param>
        /// <param name="iIssStruct"></param>
        /// <returns></returns>
        public Bitmap Render2ColorBitmapForRecord(DataSet iSourceDataSet, TIssStruct iIssStruct, Int64 iUniqueKey)
        {
            // Do main render            
            RenderCitationPrintPicture(iSourceDataSet, iIssStruct, iUniqueKey);

            // Convert to 2-color bitmap, then cleanup original bitmap
            Bitmap MonochromeBitmap = CopyToBpp(TWinPrnBase.OffscreenBitmapWin32, 1);
            if (TWinPrnBase.OffscreenBitmapWin32 != null)
            {
                TWinPrnBase.OffscreenBitmapWin32.Dispose();
                TWinPrnBase.OffscreenBitmapWin32 = null;
            }

            // Return the monochrome bitmap
            return MonochromeBitmap;
        }

        public Bitmap Render2ColorSideBySideBitmapForRecord(DataSet iSourceDataSet, TIssStruct iIssStruct, Int64 iUniqueKey)
        {
            // Do main render            
            RenderCitationPrintPicture(iSourceDataSet, iIssStruct, iUniqueKey);

            // calculate the number of pages
            int fCitationPageCountTotal = 1;
            int fPageSepWidth = 30;
            if (FormattingTable.CurrentRecordPrintPicRev.Height > PaperInfo.AvailDotHeight)
            {
                // how many pages do we need to generate?
                fCitationPageCountTotal = 1 + (FormattingTable.CurrentRecordPrintPicRev.Height / PaperInfo.AvailDotHeight);
            }

            // Calculate width of final bitmap (with pages side-by-side)
            int loFullWidth = PaperInfo.AvailDotWidth * fCitationPageCountTotal;
            if (fCitationPageCountTotal > 1)
                loFullWidth = (PaperInfo.AvailDotWidth * fCitationPageCountTotal) + (fPageSepWidth * (fCitationPageCountTotal - 1));

            // Create new bitmap for final width, and start with white background
            Bitmap FullBitmap = new Bitmap(loFullWidth,
                PaperInfo.AvailDotHeight, TWinPrnBase.OffscreenBitmapWin32.PixelFormat);
            Graphics gfx = Graphics.FromImage(FullBitmap);
            gfx.SmoothingMode = SmoothingMode.Default;
            SolidBrush FillBrush = new SolidBrush(Color.White);
            gfx.FillRectangle(FillBrush, new Rectangle(0, 0, FullBitmap.Width, FullBitmap.Height));
            FillBrush.Dispose();

            // Render each page into final image
            int CitationPageNumberCurrent = 1;
            for (CitationPageNumberCurrent = 1; CitationPageNumberCurrent <= fCitationPageCountTotal; CitationPageNumberCurrent++)
            {
                Bitmap NextPage = GetRenderedCurrentPageData(CitationPageNumberCurrent, fCitationPageCountTotal);
                if (CitationPageNumberCurrent == 1)
                {
                    // Draw page at normal origin
                    gfx.DrawImage(NextPage, new Point(0, 0));
                    // If multi-page, we'll frame it with dotted line to indicate page region
                    if (fCitationPageCountTotal > 1)
                    {
                        Pen borderpen = new Pen(Color.Black);
                        borderpen.DashStyle = DashStyle.Dot;
                        gfx.DrawRectangle(borderpen, new Rectangle(0, 0, NextPage.Width - 2, NextPage.Height - 2));
                        borderpen.Dispose();
                    }
                }
                else
                {
                    // Draw page offset horizontally
                    gfx.DrawImage(NextPage, new Point((PaperInfo.AvailDotWidth * (CitationPageNumberCurrent - 1)) +
                        (fPageSepWidth * (CitationPageNumberCurrent - 1)), 0));
                    // If multi-page, we'll frame it with dotted line to indicate page region
                    if (fCitationPageCountTotal > 1)
                    {
                        Pen borderpen = new Pen(Color.Black);
                        borderpen.DashStyle = DashStyle.Dot;
                        gfx.DrawRectangle(borderpen, new Rectangle((PaperInfo.AvailDotWidth * (CitationPageNumberCurrent - 1)) +
                            (fPageSepWidth * (CitationPageNumberCurrent - 1)), 0, NextPage.Width - 2, NextPage.Height - 2));
                        borderpen.Dispose();
                    }
                }
                // JIRA AUTOCITE-61: Dispose of NextPage image
                NextPage.Dispose();
                NextPage = null;
            }
            // Finished with the graphics object
            gfx.Dispose();

            // Convert to 2-color bitmap, then cleanup original bitmap
            Bitmap MonochromeBitmap = CopyToBpp(FullBitmap, 1);
            if (TWinPrnBase.OffscreenBitmapWin32 != null)
            {
                TWinPrnBase.OffscreenBitmapWin32.Dispose();
                TWinPrnBase.OffscreenBitmapWin32 = null;
            }
            if (FullBitmap != null)
            {
                FullBitmap.Dispose();
                FullBitmap = null;
            }

            // Return the monochrome bitmap
            return MonochromeBitmap;
        }

        public Bitmap RenderFullColorBitmapForRecord(DataSet iSourceDataSet, TIssStruct iIssStruct, Int64 iUniqueKey)
        {
            // Do main render            
            RenderCitationPrintPicture(iSourceDataSet, iIssStruct, iUniqueKey);

            /*
            // Return a copy resultant bitmap
            Bitmap Result = new Bitmap(TWinPrnBase.OffscreenBitmapWin32);
            return Result;
            */

            // Convert to 8-bit color depth so we get smaller file
            Bitmap Result = CopyToBpp(TWinPrnBase.OffscreenBitmapWin32, 8);
            TWinPrnBase.OffscreenBitmapWin32.Dispose();
            TWinPrnBase.OffscreenBitmapWin32 = null;

            // Return resultant bitmap
            return Result;
        }

        public Bitmap RenderFullColorSideBySideBitmapForRecord(DataSet iSourceDataSet, TIssStruct iIssStruct, Int64 iUniqueKey)
        {
            // Do main render            
            RenderCitationPrintPicture(iSourceDataSet, iIssStruct, iUniqueKey);

            // calculate the number of pages
            int fCitationPageCountTotal = 1;
            int fPageSepWidth = 30;
            if (FormattingTable.CurrentRecordPrintPicRev.Height > PaperInfo.AvailDotHeight)
            {
                // how many pages do we need to generate?
                fCitationPageCountTotal = 1 + (FormattingTable.CurrentRecordPrintPicRev.Height / PaperInfo.AvailDotHeight);
            }

            // Calculate width of final bitmap (with pages side-by-side)
            int loFullWidth = PaperInfo.AvailDotWidth * fCitationPageCountTotal;
            if (fCitationPageCountTotal > 1)
                loFullWidth = (PaperInfo.AvailDotWidth * fCitationPageCountTotal) + (fPageSepWidth * (fCitationPageCountTotal - 1));

            // Create new bitmap for final width, and start with white background
            // (Use Format16bppRgb565 which makes the image a little smaller than 24-bit)
            Bitmap FullBitmap = new Bitmap(loFullWidth,
                PaperInfo.AvailDotHeight, TWinPrnBase.OffscreenBitmapWin32.PixelFormat);
            Graphics gfx = Graphics.FromImage(FullBitmap);
            gfx.SmoothingMode = SmoothingMode.Default;
            SolidBrush FillBrush = new SolidBrush(Color.White);
            gfx.FillRectangle(FillBrush, new Rectangle(0, 0, FullBitmap.Width, FullBitmap.Height));
            FillBrush.Dispose();

            // Render each page into final image
            int CitationPageNumberCurrent = 1;
            for (CitationPageNumberCurrent = 1; CitationPageNumberCurrent <= fCitationPageCountTotal; CitationPageNumberCurrent++)
            {
                Bitmap NextPage = GetRenderedCurrentPageData(CitationPageNumberCurrent, fCitationPageCountTotal);
                if (CitationPageNumberCurrent == 1)
                {
                    // Draw page at normal origin
                    gfx.DrawImage(NextPage, new Point(0, 0));
                    // If multi-page, we'll frame it with dotted line to indicate page region
                    if (fCitationPageCountTotal > 1)
                    {
                        Pen borderpen = new Pen(Color.Black);
                        borderpen.DashStyle = DashStyle.Dot;
                        gfx.DrawRectangle(borderpen, new Rectangle(0, 0, NextPage.Width - 2, NextPage.Height - 2));
                        borderpen.Dispose();
                    }
                }
                else
                {
                    // Draw page offset horizontally
                    gfx.DrawImage(NextPage, new Point((PaperInfo.AvailDotWidth * (CitationPageNumberCurrent - 1)) +
                        (fPageSepWidth * (CitationPageNumberCurrent - 1)), 0));
                    // If multi-page, we'll frame it with dotted line to indicate page region
                    if (fCitationPageCountTotal > 1)
                    {
                        Pen borderpen = new Pen(Color.Black);
                        borderpen.DashStyle = DashStyle.Dot;
                        gfx.DrawRectangle(borderpen, new Rectangle((PaperInfo.AvailDotWidth * (CitationPageNumberCurrent - 1)) +
                            (fPageSepWidth * (CitationPageNumberCurrent - 1)), 0, NextPage.Width - 2, NextPage.Height - 2));
                        borderpen.Dispose();
                    }
                }

                // JIRA AUTOCITE-61: Dispose of NextPage image
                NextPage.Dispose();
                NextPage = null;
            }
            // Finished with the graphics object
            gfx.Dispose();

            // Don't need original image anymore
            if (TWinPrnBase.OffscreenBitmapWin32 != null)
            {
                TWinPrnBase.OffscreenBitmapWin32.Dispose();
                TWinPrnBase.OffscreenBitmapWin32 = null;
            }

            // Convert to 8-bit color depth so we get smaller file
            Bitmap SmallerBitmap = CopyToBpp(FullBitmap, 8);
            FullBitmap.Dispose();
            FullBitmap = null;

            // Return resultant bitmap
            return SmallerBitmap;
        }
        #endregion

        #region Private Methods
        private string GenerateFormattingFingerprint(string iDataTableName, int iTableRevNum, string iFormRevName, int iFormRevNum)
        {
            return iDataTableName.Trim() + "-" + iTableRevNum.ToString() + "-" + iFormRevName.Trim() + iFormRevNum.ToString();
        }

        private TIssForm GetIssueFormForStruct(TIssStruct iIssStruct)
        {
            // Look through the forms defined in the passed TCiteStruct
            foreach (TTForm loForm in iIssStruct.Forms)
            {
                // Is this the issuance form?
                if (loForm is TIssForm)
                {
                    return loForm as TIssForm;
                }
            }
            // If we get this far, we couldn't find the target 
            return null;
        }

        private void FillFormattingTableWithCurrentRecord(DataSet iSourceDataSet, Int64 iUniqueKey)
        {
            string loFieldValueStr;
            string loSourceFieldNameStr;
            DateTime loFieldValueDT;

            // most fields will be located in the main table, but some will be found in subtables
            bool loFieldLocated = false;

            DataRow CurrentRow = null;
            DataTable MainTable = null;
            DataRow MainTableRow = null;
            if (iSourceDataSet.Tables.Count > 0)
            {
                MainTable = iSourceDataSet.Tables[0];
                // Find the main table's data row for the passed UniqueKey
                foreach (DataRow NextRow in MainTable.Rows)
                {
                    if (Convert.ToInt64(NextRow[AutoISSUE.DBConstants.sqlUniqueKeyStr]) == iUniqueKey)
                    {
                        MainTableRow = NextRow;
                        break;
                    }
                }
            }

            // loop through the fields in the table revision, and populate the table
            FormattingTable.CurrentRecordFormattingTable.ClearFieldValues();
            foreach (TTableFldDef loFld in FormattingTable.CurrentRecordFormattingTableDefRev.Fields)
            {
                // assume we'll find the target field in the main table
                loFieldLocated = false;
                CurrentRow = null;

                // start with the actual name
                loSourceFieldNameStr = loFld.Name;

                // in the main table?
                if ((MainTable != null) && (MainTableRow != null) && ((MainTable.Columns.IndexOf(loSourceFieldNameStr)) != -1))
                {
                    // defense against funky databases - make sure this column wasn't created
                    // in error and the data is actually moved to a detail table
                    if (loFld.IsRedefinedInDetailTable == false)
                    {
                        // get the current datarow from the main table
                        if (MainTable.Rows.Count > 0)
                        {
                            CurrentRow = MainTableRow;
                            loFieldLocated = true;
                        }
                    }
                }

                // didn't find it in the main table?
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

                        // wasn't in the main table. look through the sub tables
                        DataTable SubTable = null;
                        for (int loIdx = 1; loIdx < iSourceDataSet.Tables.Count; loIdx++)
                        {
                            // now we've stripped off any suffix... is it in this subtable?
                            SubTable = iSourceDataSet.Tables[loIdx];
                            if ((SubTable.Columns.IndexOf(loSourceFieldNameStr)) != -1)
                            {
                                foreach (DataRow NextDetailRow in SubTable.Rows)
                                {
                                    try
                                    {
                                        if ((Convert.ToInt32(NextDetailRow[AutoISSUE.DBConstants.sqlOccurNoStr]) == loDetailSubTablePosition) &&
                                            (Convert.ToInt64(NextDetailRow[AutoISSUE.DBConstants.sqlMasterKeyStr]) == iUniqueKey))
                                        {
                                            CurrentRow = NextDetailRow;
                                            loFieldLocated = true;
                                            break;
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
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
                    // get the value from the dataset in the appropriate format
                    switch (loFld.EditDataType)
                    {
                        case TTableFldDef.TEditDataType.tftDate:
                            {
                                if (CurrentRow.IsNull(loSourceFieldNameStr) == false)
                                {
                                    // get the value from the datarow
                                    loFieldValueDT = (DateTime)CurrentRow[loSourceFieldNameStr];

                                    // convert it into the format the table is expecting
                                    loFieldValueStr = loFieldValueDT.ToString(ReinoTablesConst.DATE_TYPE_DATAMASK);

                                    // set it in the field
                                    FormattingTable.CurrentRecordFormattingTable.SetFormattedFieldData(loFld.Name, ReinoTablesConst.DATE_TYPE_DATAMASK, loFieldValueStr);
                                }
                                break;
                            }

                        case TTableFldDef.TEditDataType.tftTime:
                            {
                                if (CurrentRow.IsNull(loSourceFieldNameStr) == false)
                                {
                                    // get the value from the datarow
                                    loFieldValueDT = (DateTime)CurrentRow[loSourceFieldNameStr];

                                    // convert it into the format the table is expecting
                                    loFieldValueStr = loFieldValueDT.ToString(ReinoTablesConst.TIME_TYPE_DATAMASK);

                                    // set it in the field
                                    FormattingTable.CurrentRecordFormattingTable.SetFormattedFieldData(loFld.Name, ReinoTablesConst.TIME_TYPE_DATAMASK, loFieldValueStr);
                                }
                                break;
                            }
                        case TTableFldDef.TEditDataType.tftInteger:
                            {
                                // get the value from the datarow
                                loFieldValueStr = CurrentRow[loSourceFieldNameStr].ToString();
                                // set it in the field
                                FormattingTable.CurrentRecordFormattingTable.SetFormattedFieldData(loFld.Name, loFld.Mask, loFieldValueStr);
                                break;
                            }
                        case TTableFldDef.TEditDataType.tftReal:
                            {
                                // get the value from the datarow
                                loFieldValueStr = CurrentRow[loSourceFieldNameStr].ToString();

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
                                    if (CurrentRow[loSourceFieldNameStr] != System.DBNull.Value)
                                    {
                                        loFieldValueStr += "00";
                                    }

                                    FormattingTable.CurrentRecordFormattingTable.SetFormattedFieldData(loFld.Name, "8", loFieldValueStr);
                                }
                                else
                                {
                                    // set it in the field, using the defined mask
                                    FormattingTable.CurrentRecordFormattingTable.SetFormattedFieldData(loFld.Name, loFld.Mask, loFieldValueStr);
                                }
                                break;
                            }

                        // default case is also the string
                        default: //TTableFldDef.TEditDataType.tftString :
                            {
                                // get the value from the datarow
                                loFieldValueStr = CurrentRow[loSourceFieldNameStr].ToString();

                                // set it in the field
                                FormattingTable.CurrentRecordFormattingTable.SetFormattedFieldData(loFld.Name, loFld.Mask, loFieldValueStr);
                                break;
                            }
                    }
                }
                catch (Exception exp)
                {
                    // if its not in the dataset... ??
                    //MessageBox.Show("An error occurred during data formatting. " + exp.Message);
                }
            }
        }

        private void GetFormattingTable(DataSet iSourceDataSet, TIssStruct iIssStruct)
        {
            // Start by trying to get the correct formatting table
            int loTableRev = 0;
            string loFormName = "";
            int loFormRev = 0;
            try
            {
                loTableRev = Convert.ToInt32(iSourceDataSet.Tables[0].Rows[0][AutoISSUE.DBConstants.sqlTableRevisionNumberStr]);
                loFormName = iSourceDataSet.Tables[0].Rows[0][AutoISSUE.DBConstants.sqlFormRevisionNameStr].ToString();
                loFormRev = Convert.ToInt32(iSourceDataSet.Tables[0].Rows[0][AutoISSUE.DBConstants.sqlFormRevisionNumberStr]);
            }
            catch { }
            GetFormattingTable(iIssStruct, iIssStruct.MainTable.Name, loTableRev, loFormName, loFormRev);
        }

        private void GetFormattingTable(TIssStruct iSourceStruct, string iDataTableName,
            int iTableRevNum, string iFormRevName, int iFormRevNum)
        {
            // Init
            FormattingTable = null;
            PaperInfo = null;

            // See if there is a cached one we can use
            string loFingerprint = GenerateFormattingFingerprint(iDataTableName, iTableRevNum, iFormRevName, iFormRevNum);
            PrintPicFormattingInfoPredicate loPredicate = new PrintPicFormattingInfoPredicate(loFingerprint);
            PrintPicFormattingInfo loFormatInfo = fFormattingTables.Find(loPredicate.CompareByFingerprint);
            if (loFormatInfo != null)
            {
                FormattingTable = loFormatInfo;
                PaperInfo = new HandheldPaperInfo(FormattingTable.CurrentRecordPrintPicRev.Name);
                return;
            }
            else
            {
                PaperInfo = new HandheldPaperInfo(string.Empty);

            }
            // the formatting info is the table, form, and print picture that match the data revision
            FormattingTable = new PrintPicFormattingInfo();
            FormattingTable.SourceStruct = iSourceStruct;
            FormattingTable.FormRevName = iFormRevName;
            FormattingTable.FormRevNum = iFormRevNum;
            FormattingTable.TableRevNum = iTableRevNum;

            // if its valid, resolve the references
            if (FormattingTable.TableRevNum <= iSourceStruct.MainTable.Revisions.Count)
            {
                // can't count on the serialization to preserve the order... so we have to find it specifically
                foreach (TTableDefRev loDefRev in iSourceStruct.MainTable.Revisions)
                {
                    //is this the revision we need?
                    if (loDefRev.Revision == FormattingTable.TableRevNum)
                    {
                        // we must make our own copy because it must have exactly one revision
                        // this way we will have only a single revision in our private def because
                        // the virtual field code embed references to the highest table revision, which we are not
                        // plus we'll have it here for thread safe access/usage
                        FormattingTable.CurrentRecordFormattingTableDef = new TTableDef();
                        FormattingTable.CurrentRecordFormattingTableDefRev = new TTableDefRev();
                        foreach (TTableFldDef oneField in loDefRev.Fields)
                        {
                            FormattingTable.CurrentRecordFormattingTableDefRev.Fields.Add(oneField);
                        }
                        FormattingTable.CurrentRecordFormattingTableDef.Revisions.Add(FormattingTable.CurrentRecordFormattingTableDefRev);

                        // now the table
                        FormattingTable.CurrentRecordFormattingTable = new TTTable();
                        FormattingTable.CurrentRecordFormattingTable.fTableDef = FormattingTable.CurrentRecordFormattingTableDef;
                        break;
                    }
                }
            }

            // Get the Issuance form for the passed structure
            FormattingTable.IssueForm = GetIssueFormForStruct(iSourceStruct);

            // if we found an issue form, we need to set parents for the printpicture
            if (FormattingTable.IssueForm != null)
            {
                if (FormattingTable.IssueForm.PrintPictureList.Count > 0)
                {
                    foreach (TIssPrnForm loPrintPic in FormattingTable.IssueForm.PrintPictureList)
                    {
                        foreach (TIssPrnFormRev loPrintPicRev in loPrintPic.Revisions)
                        {
                            // If the list is empty, maybe it hasn't been populated yet
                            if (loPrintPicRev.AllPrnDataElements.Count == 0)
                            {
                                loPrintPicRev.AllPrnDataElements.Clear();
                                loPrintPicRev.ResolveObjectReferences(null, loPrintPicRev);
                                loPrintPicRev.AddDataElementsToList(loPrintPicRev.AllPrnDataElements);
                            }
                        }
                    }
                }
            }

            // get the print picture revision for this record, if there is one
            if (FormattingTable.IssueForm != null)
            {
                TIssPrnForm loPrintPicture = null;

                // we'd like to just locate it by name
                if (FormattingTable.FormRevName.Length > 0)
                {
                    TObjBasePredicate loFindFormPredicate = new TObjBasePredicate(FormattingTable.FormRevName);
                    loPrintPicture = FormattingTable.IssueForm.PrintPictureList.Find(loFindFormPredicate.CompareByName_CaseInsensitive);
                }

                // find the print picture? if not, employ a safety fallback,
                // in case printpicture name is not populated in the data
                if (loPrintPicture == null)
                {
                    if (FormattingTable.IssueForm.PrintPictureList.Count > 0)
                    {
                        // default to the only print picture
                        loPrintPicture = FormattingTable.IssueForm.PrintPictureList[0];
                    }
                }

                // we have a print picture, now find the revision number
                if (loPrintPicture != null)
                {
                    // iterate to find the matching revision, serialization doesn't preserve collection order (?)
                    foreach (TIssPrnFormRev loPrintRev in loPrintPicture.Revisions)
                    {
                        if (loPrintRev.Revision == FormattingTable.FormRevNum)
                        {
                            FormattingTable.CurrentRecordPrintPicRev = loPrintRev;
                            PaperInfo = new HandheldPaperInfo(FormattingTable.CurrentRecordPrintPicRev.Name);
                            break;
                        }
                    }
                }

                // generate a "fingerprint" for this formatting info so we can match it up for other records
                FormattingTable.fFormatRecordFingerprint = GenerateFormattingFingerprint(iDataTableName,
                    FormattingTable.TableRevNum, FormattingTable.FormRevName, FormattingTable.FormRevNum);

                // add this one to the list so others might use it
                fFormattingTables.Add(FormattingTable);
            }
        }

        private void RenderCitationPrintPicture(DataSet iSourceDataSet, TIssStruct iIssStruct, Int64 iUniqueKey)
        {
            // Start by trying to get the correct formatting table
            GetFormattingTable(iSourceDataSet, iIssStruct);

            // some structs have no print picture... but this routine may
            // be called because of the generic nature of handling the dataset
            if ((FormattingTable == null) || (FormattingTable.CurrentRecordPrintPicRev == null))
            {
                // if we have a table but no print picture, thats a data problem
                if ((FormattingTable != null) && (FormattingTable.FormRevName.Length > 0))
                {
                    throw new Exception("An error has occured while rendering the reproduction view. A print picture was not located for form " + FormattingTable.FormRevName);
                }
                return;
            }

            // if there was any previous rendered image, clean it up
            if (TWinPrnBase.OffscreenBitmapWin32 != null)
            {
                TWinPrnBase.OffscreenBitmapWin32.Dispose();
                TWinPrnBase.OffscreenBitmapWin32 = null;
            }

            // get the data from the dataset into the formatting table
            FillFormattingTableWithCurrentRecord(iSourceDataSet, iUniqueKey);

            // get the data back out of the formatting table... formatted as required for printing
            TWinBasePrnData loPrnData;
            short loNdx;
            int loFieldNo;
            for (loNdx = 0; loNdx < FormattingTable.CurrentRecordPrintPicRev.AllPrnDataElements.Count; loNdx++)
            {
                loPrnData = FormattingTable.CurrentRecordPrintPicRev.AllPrnDataElements[loNdx];
                if (loPrnData == null)
                    continue;
                try
                {
                    // must locate the fieldno in the table revision; if we ask via the tabledef we always get the highest
                    loFieldNo = FormattingTable.CurrentRecordFormattingTable.fTableDef.GetFldNo(loPrnData.Name);
                    if (loFieldNo != -1)
                    {
                        TTableFldDef loOneField = FormattingTable.CurrentRecordFormattingTableDefRev.Fields[loFieldNo];
                        string loDataMask;

                        // translate it back to AutoISSUE style for accurate display
                        switch (loOneField.EditDataType)
                        {
                            case TTableFldDef.TEditDataType.tftDate:
                                {
                                    if (loPrnData.MaskForHH != "")
                                        loDataMask = loPrnData.MaskForHH;
                                    else
                                        loDataMask = AutoISSUE.DBConstants.GetAutoISSUEMaskForDotNetMask_Date(loPrnData.Mask);
                                    break;
                                }
                            case TTableFldDef.TEditDataType.tftTime:
                                {
                                    if (loPrnData.MaskForHH != "")
                                        loDataMask = loPrnData.MaskForHH;
                                    else
                                        loDataMask = AutoISSUE.DBConstants.GetAutoISSUEMaskForDotNetMask_Time(loPrnData.Mask);
                                    break;
                                }
                            default:
                                {
                                    if (loPrnData.MaskForHH != "")
                                        loDataMask = loPrnData.MaskForHH;
                                    else
                                        loDataMask = loPrnData.Mask;
                                    break;
                                }
                        }

                        // Get the properly formatted data and assign to the print picture field
                        loPrnData.TextBuf = FormattingTable.CurrentRecordFormattingTable.GetFormattedFieldData(loFieldNo, loDataMask);
                    }

                    // 2009.09.01 JLA - Special case is TWinPrnImage which lets us embed an image from the notes table into the ticket image
                    if (loPrnData is TWinPrnImage)
                    {
                        GetDataForEmbeddedImage((loPrnData as TWinPrnImage), iSourceDataSet, iIssStruct);
                    }
                }
                catch
                {
                    // if its not in the dataset... ??
                }
            }

            // Assume default height of 3" printer if width was not specified
            int loDotHeight = FormattingTable.CurrentRecordPrintPicRev.Height;
            if (loDotHeight == 0)
                loDotHeight = AutoISSUE.DBConstants.LTP3345_PaperAvailDotHt;

            // Assume default width of 3" printer if width was not specified
            int loDotWidth = FormattingTable.CurrentRecordPrintPicRev.Width;
            if (loDotWidth == 0)
                loDotWidth = AutoISSUE.DBConstants.LTP3345_PaperAvailDotWd;

            FormattingTable.CurrentRecordPrintPicRev.Series3CE_ClearPrintCanvas(loDotHeight, loDotWidth);
            FormattingTable.CurrentRecordPrintPicRev.PrepareForPrint();
            FormattingTable.CurrentRecordPrintPicRev.PaintDescendants();
        }

        private void GetDataForEmbeddedImage(TWinPrnImage imageField, DataSet iSourceDataSet, TIssStruct iIssStruct)
        {
            // Init the imageField data to blank value
            imageField.ImageDataAsBase64 = "";

            // Is the image sourced from a detail record?
            if (imageField.ImageSourceType == TWinPrnImage.TImageSourceType.imgSelectedDetailRecord)
            {
                // ImageSourceParameter should equate to the detail record number that the multimedia image will be extracted from
                int loDetailRecNo = 0;
                try
                {
                    if (!string.IsNullOrEmpty(imageField.ImageSourceParameter))
                        loDetailRecNo = Convert.ToInt32(imageField.ImageSourceParameter);
                }
                catch { }

                // Find the detail table that has the multimedia fields
                DataTable SubTable = null;
                for (int loIdx = 1; loIdx < iSourceDataSet.Tables.Count; loIdx++)
                {
                    if ((iSourceDataSet.Tables[loIdx].Columns.IndexOf(AutoISSUE.DBConstants.sqlMultimediaNoteDataStr)) != -1)
                    {
                        SubTable = iSourceDataSet.Tables[loIdx];
                        break;
                    }
                }

                // Can't go any further if there is no table to get data from
                if (SubTable == null)
                    return;

                // Now find the record that has the correct detail number and get the blob data.
                // The handheld now supports the ability to have the officer choose which photo to
                // put on the ticket, and this information is stored in a "PrintedImageOrder" field.
                // However, we must also be backward compatible so we can handle data that was 
                // generated prior to this new feature.

                byte[] RawData = new byte[] { };
                foreach (DataRow nextRow in SubTable.Rows)
                {
                    // Does the detail record number match what we're looking for
                    //AutoISSUE.DBConstants.sqlPrintedImageOrderNameStr

                    if (nextRow[AutoISSUE.DBConstants.sqlPrintedImageOrderNameStr] == DBNull.Value)
                    {
                        // If the "PrintedImageOrder" is null, we will tentatively retain this
                        // as a backward-compatible default fallback record if we don't already
                        // have data
                        if (RawData.Length == 0)
                        {
                            if (!nextRow[AutoISSUE.DBConstants.sqlMultimediaNoteDataStr].Equals(System.DBNull.Value))
                            {
                                RawData = (byte[])nextRow[AutoISSUE.DBConstants.sqlMultimediaNoteDataStr];
                            }
                        }
                    }
                    else
                    {
                        // Get byte array from row unless its null
                        if (!nextRow[AutoISSUE.DBConstants.sqlMultimediaNoteDataStr].Equals(System.DBNull.Value))
                        {
                            RawData = (byte[])nextRow[AutoISSUE.DBConstants.sqlMultimediaNoteDataStr];

                            // No need to look at other rows if this is the one that matches the "PrintedImageOrder"
                            try
                            {
                                if (Convert.ToInt32(nextRow[AutoISSUE.DBConstants.sqlPrintedImageOrderNameStr].ToString()) ==
                                    Convert.ToInt32(AutoISSUE.DBConstants.TPrintedImageOrder.piSelectedImage))
                                {
                                    break;
                                }
                            }
                            catch { }
                        }
                    }
                }

                // Do we have some image data to work with?
                if (RawData.Length > 0)
                {
                    // First we will load the raw data into a stream
                    // We need to be careful of bad data that can cause an exception
                    MemoryStream RawMS = null;
                    MemoryStream destMemStream = null;
                    Bitmap img = null;
                    Bitmap ditheredImg = null;
                    try
                    {
                        // Load byte array into memory stream, then create image from memory stream
                        RawMS = new MemoryStream(RawData);
                        RawMS.Position = 0;

                        // Create image from memory stream
                        Bitmap imgRawSrc = new Bitmap(RawMS);

                        // High-quality resample/resize source image to destination size BEFORE applying dither function
                        img = new Bitmap(imageField.Width, imageField.Height);
                        using (Graphics graphics = Graphics.FromImage(img))
                        {
                            // Set the resize quality modes to high quality
                            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            // Draw the image into the target bitmap
                            graphics.DrawImage(imgRawSrc, 0, 0, img.Width, img.Height);
                        }
                        // Now we can dispose of the raw source image
                        imgRawSrc.Dispose();
                        imgRawSrc = null;

                        //// Now we need to dither the image so it looks more like the image produced by mobile device and thermal printer
                        //Reino.ClientConfig.Dithering_Stucki stuckiDither = new Dithering_Stucki();
                        //ditheredImg = stuckiDither.Dither(img);

                        // Now convert dithered image into a BASE64 string and assign to the TWinPrnImage
                        destMemStream = new MemoryStream();
                        img.Save(destMemStream, ImageFormat.Bmp);
                        string base64 = Convert.ToBase64String(destMemStream.ToArray());
                        imageField.ImageDataAsBase64 = base64;
                    }
                    catch (Exception jpgEx)
                    {
                    }
                    finally
                    {
                        // Close the raw source memory stream. Note: This CANNOT be closed before we save the image!
                        if (RawMS != null)
                        {
                            RawMS.Close();
                            RawMS.Dispose();
                        }
                        if (destMemStream != null)
                        {
                            destMemStream.Close();
                            destMemStream.Dispose();
                        }
                        if (img != null)
                        {
                            img.Dispose();
                            img = null;
                        }
                        if (ditheredImg != null)
                        {
                            ditheredImg.Dispose();
                            ditheredImg = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Copies a bitmap into a 1bpp/8bpp bitmap of the same dimensions, fast
        /// </summary>
        /// <param name="b">original bitmap</param>
        /// <param name="bpp">1 or 8, target bpp</param>
        /// <returns>a 1bpp copy of the bitmap</returns>
        private static System.Drawing.Bitmap CopyToBpp(System.Drawing.Bitmap b, int bpp)
        {
            if (bpp != 1 && bpp != 8) throw new System.ArgumentException("1 or 8", "bpp");

            // Plan: built into Windows GDI is the ability to convert
            // bitmaps from one format to another. Most of the time, this
            // job is actually done by the graphics hardware accelerator card
            // and so is extremely fast. The rest of the time, the job is done by
            // very fast native code.
            // We will call into this GDI functionality from C#. Our plan:
            // (1) Convert our Bitmap into a GDI hbitmap (ie. copy unmanaged->managed)
            // (2) Create a GDI monochrome hbitmap
            // (3) Use GDI "BitBlt" function to copy from hbitmap into monochrome (as above)
            // (4) Convert the monochrone hbitmap into a Bitmap (ie. copy unmanaged->managed)

            int w = b.Width, h = b.Height;
            IntPtr hbm = b.GetHbitmap(); // this is step (1)
            //
            // Step (2): create the monochrome bitmap.
            // "BITMAPINFO" is an interop-struct which we define below.
            // In GDI terms, it's a BITMAPHEADERINFO followed by an array of two RGBQUADs
            BITMAPINFO bmi = new BITMAPINFO();
            bmi.biSize = 40;  // the size of the BITMAPHEADERINFO struct
            bmi.biWidth = w;
            bmi.biHeight = h;
            bmi.biPlanes = 1; // "planes" are confusing. We always use just 1. Read MSDN for more info.
            bmi.biBitCount = (short)bpp; // ie. 1bpp or 8bpp
            bmi.biCompression = BI_RGB; // ie. the pixels in our RGBQUAD table are stored as RGBs, not palette indexes
            bmi.biSizeImage = (uint)(((w + 7) & 0xFFFFFFF8) * h / 8);
            bmi.biXPelsPerMeter = 1000000; // not really important
            bmi.biYPelsPerMeter = 1000000; // not really important
            // Now for the colour table.
            uint ncols = (uint)1 << bpp; // 2 colours for 1bpp; 256 colours for 8bpp
            bmi.biClrUsed = ncols;
            bmi.biClrImportant = ncols;
            bmi.cols = new uint[256]; // The structure always has fixed size 256, even if we end up using fewer colours
            if (bpp == 1) { bmi.cols[0] = MAKERGB(0, 0, 0); bmi.cols[1] = MAKERGB(255, 255, 255); }
            else { for (int i = 0; i < ncols; i++) bmi.cols[i] = MAKERGB(i, i, i); }
            // For 8bpp we've created an palette with just greyscale colours.
            // You can set up any palette you want here. Here are some possibilities:
            // greyscale: for (int i=0; i<256; i++) bmi.cols[i]=MAKERGB(i,i,i);
            // rainbow: bmi.biClrUsed=216; bmi.biClrImportant=216; int[] colv=new int[6]{0,51,102,153,204,255};
            //          for (int i=0; i<216; i++) bmi.cols[i]=MAKERGB(colv[i/36],colv[(i/6)%6],colv[i%6]);
            // optimal: a difficult topic: http://en.wikipedia.org/wiki/Color_quantization
            // 
            // Now create the indexed bitmap "hbm0"
            IntPtr bits0; // not used for our purposes. It returns a pointer to the raw bits that make up the bitmap.
            IntPtr hbm0 = CreateDIBSection(IntPtr.Zero, ref bmi, DIB_RGB_COLORS, out bits0, IntPtr.Zero, 0);
            //
            // Step (3): use GDI's BitBlt function to copy from original hbitmap into monocrhome bitmap
            // GDI programming is kind of confusing... nb. The GDI equivalent of "Graphics" is called a "DC".
            IntPtr sdc = GetDC(IntPtr.Zero);       // First we obtain the DC for the screen
            // Next, create a DC for the original hbitmap
            IntPtr hdc = CreateCompatibleDC(sdc);
            SelectObject(hdc, hbm);
            // and create a DC for the monochrome hbitmap
            IntPtr hdc0 = CreateCompatibleDC(sdc);
            SelectObject(hdc0, hbm0);
            // Now we can do the BitBlt:
            BitBlt(hdc0, 0, 0, w, h, hdc, 0, 0, SRCCOPY);
            // Step (4): convert this monochrome hbitmap back into a Bitmap:
            System.Drawing.Bitmap destBitmap = System.Drawing.Bitmap.FromHbitmap(hbm0);
            //
            // Finally some cleanup.
            DeleteDC(hdc);
            DeleteDC(hdc0);
            ReleaseDC(IntPtr.Zero, sdc);
            DeleteObject(hbm);
            DeleteObject(hbm0);
            //
            return destBitmap;
        }

        private static uint MAKERGB(int r, int g, int b)
        {
            return ((uint)(b & 255)) | ((uint)((r & 255) << 8)) | ((uint)((g & 255) << 16));
        }

        // returns the bitmap for the current page, null when there are no more pages
        public Bitmap GetRenderedCurrentPageData(int CitationPageNumberCurrent, int fCitationPageCountTotal)
        {
            // have a page to return?
            if (CitationPageNumberCurrent <= fCitationPageCountTotal)
            {
                // in calculating the top of the page we have to account for the preprinted areas
                // In the handheld the printer is positioned beyond it at the start of the first
                // page (following the form feed from the previous cite or paper loading)
                // Some paper stock has a preprinted header on every sheet, while some stock
                // is designed to print two pages every time and has no header on alternate sheets

                // TODO: Get the preprinted info/graphics specified for each client so we can reproduce 
                int loCitationSourceBitmapRectTop;
                int loCitationSourceBitmapRectHeight;
                int loCitationDestBitmapRectTop;

                // on the very first page, the printer has already skipped past the pre-printed header  
                int loFirstPageRectHeight = PaperInfo.AvailDotHeight - (int)(PaperInfo.PaperPreprintedHeaderInches * PaperInfo.PaperDPIVert);

                if (CitationPageNumberCurrent == 1)
                {
                    // on the very first page, the printer has already skipped past the pre-printed header  
                    loCitationSourceBitmapRectTop = 0;
                    loCitationSourceBitmapRectHeight = loFirstPageRectHeight;
                    loCitationDestBitmapRectTop = (int)(PaperInfo.PaperPreprintedHeaderInches * PaperInfo.PaperDPIVert);
                }
                else
                {
                    // after that, the print picture skips over the pre-printed areas  
                    // we calculate ((CitationPageNumberCurrent - 2) * pageheight) because 1st page is precalculated
                    loCitationSourceBitmapRectTop = loFirstPageRectHeight + ((CitationPageNumberCurrent - 2) * (PaperInfo.AvailDotHeight));
                    loCitationSourceBitmapRectHeight = PaperInfo.AvailDotHeight;
                    loCitationDestBitmapRectTop = 0;
                }

                // specify the region to copy
                Rectangle iRect = new Rectangle(0, loCitationSourceBitmapRectTop,
                    FormattingTable.CurrentRecordPrintPicRev.Width,
                    loCitationSourceBitmapRectHeight);

                // make sure the rectangle fits with the source bitmap - often the 2nd page 
                // isn't completely full, and when its not, the source bitmap isn't rendered 
                // to the full size of the paper
                if (iRect.Bottom > TWinPrnBase.OffscreenBitmapWin32.Height)
                {
                    iRect.Height -= (iRect.Bottom - TWinPrnBase.OffscreenBitmapWin32.Height);
                }
                if (iRect.Right > TWinPrnBase.OffscreenBitmapWin32.Width)
                {
                    iRect.Width -= (iRect.Right - TWinPrnBase.OffscreenBitmapWin32.Width);
                }


                // create a bitmap that is exactly the page height -
                // this preserves the scaling by accounting for the white space that
                // isn't always used at the bottom/sides of pages
                Bitmap oneCitePage = new Bitmap(PaperInfo.AvailDotWidth, PaperInfo.AvailDotHeight, TWinPrnBase.OffscreenBitmapWin32.PixelFormat);

                // create graphics object to manipulate the bitmap
                Graphics gfx = Graphics.FromImage(oneCitePage);
                // crispy, please - if we have antialiasing on during the fill method, we end up with rough (unfilled) edges
                gfx.SmoothingMode = SmoothingMode.Default;
                // create a brush and clear out the bitmap... a clean sheet of paper
                SolidBrush FillBrush = new SolidBrush(Color.White);
                gfx.FillRectangle(FillBrush, new Rectangle(0, 0, oneCitePage.Width, oneCitePage.Height));
                // copy the image from the current page
                gfx.DrawImage(TWinPrnBase.OffscreenBitmapWin32, 0, loCitationDestBitmapRectTop, iRect, GraphicsUnit.Pixel);
                // clean up
                FillBrush.Dispose();
                gfx.Dispose();

                // give it back
                return oneCitePage;
            }
            else
            {
                // nothing for ya
                return null;
            }
        }
        #endregion

        #region Windows API Declarations for GDI functions
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern int DeleteDC(IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern int BitBlt(IntPtr hdcDst, int xDst, int yDst, int w, int h, IntPtr hdcSrc, int xSrc, int ySrc, int rop);
        private static int SRCCOPY = 0x00CC0020;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO bmi, uint Usage, out IntPtr bits, IntPtr hSection, uint dwOffset);
        private static uint BI_RGB = 0;
        private static uint DIB_RGB_COLORS = 0;

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public uint biSize;
            public int biWidth, biHeight;
            public short biPlanes, biBitCount;
            public uint biCompression, biSizeImage;
            public int biXPelsPerMeter, biYPelsPerMeter;
            public uint biClrUsed, biClrImportant;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] cols;
        }
        #endregion
    }
    #endregion

    #region PrintPicFormattingInfo
    public class PrintPicFormattingInfo : Object
    {
        public TIssStruct SourceStruct;
        public int FormRevNum;
        public int TableRevNum;
        public string FormRevName = "";
        public string fFormatRecordFingerprint = "";

        public TIssPrnFormRev CurrentRecordPrintPicRev;
        public TTTable CurrentRecordFormattingTable;
        public TTableDef CurrentRecordFormattingTableDef;
        public TTableDefRev CurrentRecordFormattingTableDefRev;
        public TIssForm IssueForm;
    }
    #endregion

    #region PrintPicFormattingInfoPredicate
    public class PrintPicFormattingInfoPredicate : object
    {
        private string fCompareFingerprint;

        public PrintPicFormattingInfoPredicate(string iCompareFingerprint)
        {
            fCompareFingerprint = iCompareFingerprint;
        }

        public bool CompareByFingerprint(PrintPicFormattingInfo iFormatTableObject)
        {
            return iFormatTableObject.fFormatRecordFingerprint.Equals(fCompareFingerprint);
        }
    }
    #endregion

    #region Handheld Paper Info Class
    /// <summary>
    /// Class to encapsulate details about the paper on the handheld printout
    /// Used to generate accurate reproductions on the host sie
    /// </summary>
    public class HandheldPaperInfo : Object
    {
        private int fAvailDotWidth;
        private int fAvailDotHeight;
        private int fHorzDotsPerPage;
        private int fVertDotsPerPage;
        private double fPaperInchWd;
        private double fPaperInchHt;
        private double fPaperDPIHorz;
        private double fPaperDPIVert;
        private double fPaperPhysicalInchWd;
        private double fPaperPhysicalInchHt;
        private double fPaperPreprintedHeaderInches;

        public int AvailDotWidth
        {
            get { return fAvailDotWidth; }
        }

        public int AvailDotHeight
        {
            get { return fAvailDotHeight; }
        }

        public int HorzDotsPerPage
        {
            get { return fHorzDotsPerPage; }
        }

        public int VertDotsPerPage
        {
            get { return fVertDotsPerPage; }
        }

        public double PaperInchWd
        {
            get { return fPaperInchWd; }
        }
        public double PaperInchHt
        {
            get { return fPaperInchHt; }
        }
        public double PaperDPIHorz
        {
            get { return fPaperDPIHorz; }
        }
        public double PaperDPIVert
        {
            get { return fPaperDPIVert; }
        }
        public double PaperPhysicalInchWd
        {
            get { return fPaperPhysicalInchWd; }
        }
        public double PaperPhysicalInchHt
        {
            get { return fPaperPhysicalInchHt; }
        }
        public double PaperPreprintedHeaderInches
        {
            get { return fPaperPreprintedHeaderInches; }
        }

        /// <summary>
        /// Paper Width in Hundredths of an inch - the way the printer graphics object wants it
        /// </summary>
        public int PaperHundredthsOfAnInchWidth
        {
            get { return (int)(fPaperInchWd * 100); }
        }

        /// <summary>
        /// Paper Height in Hundredths of an inch - the way the printer graphics object wants it
        /// </summary>
        public int PaperHundredthsOfAnInchHeight
        {
            get { return (int)(fPaperInchHt * 100); }
        }

        public HandheldPaperInfo(string iPaperClassName)
        {
            // initialize the values according to the type of paper
            if (iPaperClassName.EndsWith(AutoISSUE.DBConstants.cnPaperClassSuffixTwoInch))
            {
                // 2 in paper
                fAvailDotWidth = AutoISSUE.DBConstants.LTP_2IN_PaperAvailDotWd;
                fAvailDotHeight = AutoISSUE.DBConstants.LTP_2IN_PaperAvailDotHt;
                fHorzDotsPerPage = AutoISSUE.DBConstants.LTP_2IN_PaperAvailDotWd;
                fVertDotsPerPage = AutoISSUE.DBConstants.LTP_2IN_PaperAvailDotHt;
                fPaperInchWd = AutoISSUE.DBConstants.LTP_2IN_PaperInchWd;
                fPaperInchHt = AutoISSUE.DBConstants.LTP_2IN_PaperInchHt;
                fPaperDPIHorz = AutoISSUE.DBConstants.LTP_2IN_DPIHorz;
                fPaperDPIVert = AutoISSUE.DBConstants.LTP_2IN_DPIVert;
                fPaperPhysicalInchWd = AutoISSUE.DBConstants.LTP_2IN_PhysicalPaperInchWd;
                fPaperPhysicalInchHt = AutoISSUE.DBConstants.LTP_2IN_PhysicalPaperInchHt;
                fPaperPreprintedHeaderInches = 1.125; // this should be a configuration or ClientCfg value
            }
            else if (iPaperClassName.EndsWith(AutoISSUE.DBConstants.cnPaperClassSuffixFourInch))
            {
                // 4in paper
                fAvailDotWidth = AutoISSUE.DBConstants.LTP3445_PaperAvailDotWd;
                fAvailDotHeight = AutoISSUE.DBConstants.LTP3445_PaperAvailDotHt;
                fHorzDotsPerPage = AutoISSUE.DBConstants.LTP3445_PaperAvailDotWd;
                fVertDotsPerPage = AutoISSUE.DBConstants.LTP3445_PaperAvailDotHt;
                fPaperInchWd = AutoISSUE.DBConstants.LTP3445_PaperInchWd;
                fPaperInchHt = AutoISSUE.DBConstants.LTP3445_PaperInchHt;
                fPaperDPIHorz = AutoISSUE.DBConstants.LTP3445_DPIHorz;
                fPaperDPIVert = AutoISSUE.DBConstants.LTP3445_DPIVert;
                fPaperPhysicalInchWd = AutoISSUE.DBConstants.LTP3445_PhysicalPaperInchWd;
                fPaperPhysicalInchHt = AutoISSUE.DBConstants.LTP3445_PhysicalPaperInchHt;
                fPaperPreprintedHeaderInches = 1.125; // this should be a configuration or ClientCfg value
            }
            else
            {
                // no match, we'll assume 3in paper
                fAvailDotWidth = AutoISSUE.DBConstants.LTP3345_PaperAvailDotWd;
                fAvailDotHeight = AutoISSUE.DBConstants.LTP3345_PaperAvailDotHt;
                fHorzDotsPerPage = AutoISSUE.DBConstants.LTP3345_PaperAvailDotWd;
                fVertDotsPerPage = AutoISSUE.DBConstants.LTP3345_PaperAvailDotHt;
                fPaperInchWd = AutoISSUE.DBConstants.LTP3345_PaperInchWd;
                fPaperInchHt = AutoISSUE.DBConstants.LTP3345_PaperInchHt;
                fPaperDPIHorz = AutoISSUE.DBConstants.LTP3345_DPIHorz;
                fPaperDPIVert = AutoISSUE.DBConstants.LTP3345_DPIVert;
                fPaperPhysicalInchWd = AutoISSUE.DBConstants.LTP3345_PhysicalPaperInchWd;
                fPaperPhysicalInchHt = AutoISSUE.DBConstants.LTP3345_PhysicalPaperInchHt;
                fPaperPreprintedHeaderInches = 1.125; // this should be a configuration or ClientCfg value
            }
        }
    }
    #endregion
}
