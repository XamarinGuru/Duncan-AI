
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
    public enum TTMultimedia
    {
        mmNone = 0,
        mmAudio,
        mmPicture,
        mmVideo
    }

    public class CiteStructLogicAndroid : IssueStructLogicAndroid
    {
        #region Public members
        public TCiteStruct CiteStruct
        {
            get { return this.IssueStruct as TCiteStruct; }
        }

        // Multimedia file locations differ based on hardware platform
#if WindowsCE
        internal const string CN_PICTURE_FILE_EXT = ".JPG";
        internal const string CN_PICTURE_RECORD_PATH = "\\My Documents\\My Pictures";
        internal const string CN_PICTURE_STORAGE_PATH = "pictures\\";

        internal const string CN_MOVIE_FILE_EXT = ".MPG";
        internal const string CN_MOVIE_RECORD_PATH = "\\My Documents\\My Movies";
        internal const string CN_MOVIE_STORAGE_PATH = "Movies\\";

        internal const string CN_AUDIO_RECORD_PATH = "\\My Documents";
        internal const string CN_AUDIO_STORAGE_PATH = "sounds\\";
        internal const string CN_AUDIO_FILE_EXT = ".WAV";
#else
        internal const string CN_PICTURE_FILE_EXT = ".JPG";
        internal const string CN_PICTURE_RECORD_PATH = "C:\\Documents and Settings\\All Users\\Documents\\My Pictures\\Sample Pictures";
        internal const string CN_PICTURE_STORAGE_PATH = "pictures\\";

        internal const string CN_MOVIE_FILE_EXT = ".MPG";
        internal const string CN_MOVIE_RECORD_PATH = "C:\\Documents and Settings\\All Users\\Documents\\My Pictures\\Sample Pictures";
        internal const string CN_MOVIE_STORAGE_PATH = "Movies\\";

        internal const string CN_AUDIO_RECORD_PATH = "C:\\Documents and Settings\\All Users\\Documents\\My Music\\Sample Music";
        internal const string CN_AUDIO_STORAGE_PATH = "sounds\\";
        internal const string CN_AUDIO_FILE_EXT = ".WAV";
#endif
#endregion

        #region Private members
        private TTTable TempDataRecoveryTable = null;
        private List<string> InitialFilesList = null;
        #endregion

#if _original_
        public SequenceImp GetSequence()
        {
            TObjBasePredicate predicate = new TObjBasePredicate(CiteStruct.Sequence);
            SequenceImp SeqObj = Reino.CommonLogic.SequenceManager.GlobalSequenceMgr.Sequences.Find(predicate.CompareByName_CaseInsensitive);
            return SeqObj;
        }
#endif

        #region RecoveryFile routines
        /// <summary>
        /// Determines if a recovery file is appropriate for this data type and 
        /// creates one if needed. If an already existing file is found, it copies 
        /// the data into the main table and deletes the temporary file.
        /// </summary>
        public void InitTempRecoveryFile()
        {

#if _original_

            // Avoid doing this more than once per structure
            if (TempDataRecoveryTable != null)
                return;

            int loStatus = 0;
            bool loRecoveredData = false;

            // for some datatypes, we'll define a recovery file
            // if its important enough to have a sequence file, its important enough to backup
            SequenceImp SeqObj = GetSequence();
            if (SeqObj != null)
            {
                // Build filename for temp file  
                string loTempTableName = "";
                CiteStruct.MainTable.GetTblName(ref loTempTableName);
                loTempTableName = "SAVE_" + Path.ChangeExtension(loTempTableName, null);

                // Build FULL path and filename for temp file
                string loFullPathAndFileName = "";
                CiteStruct.MainTable.GetTblPathName(ref loFullPathAndFileName);
                loFullPathAndFileName += Path.ChangeExtension(loTempTableName, ".DAT");

                // create a table to hold the temporary saved data
                TTableDef loTableDef = new TTableDef();
                loTableDef.Name = loTempTableName;
                loTableDef.fOpenEdit = false; // not in edit mode, read only

                if (loTableDef.Revisions.Count == 0)
                {
                    TTableDefRev DefRev = new TTableDefRev();
                    DefRev.Name = loTempTableName;
                    loTableDef.Revisions.Add(DefRev);
                }

                // this isn't read from the CFG file, but we have to make sure the call is made to get things initialized
                loTableDef.PostDeserialize(Reino.ClientConfig.TTableListMgr.glTableMgr);
                // we're just a duplicate of the "real" table
                TTableDef SrcTableDef = CiteStruct.MainTable;
                loTableDef.CopyTableStructure(ref SrcTableDef, true); // exclude virtual fields

                TempDataRecoveryTable = new TTTable();
                TempDataRecoveryTable.Name = loTempTableName;
                TempDataRecoveryTable.SetTableName(loTempTableName);

                // if there is any data in the file now, it must be from
                // a crash in the field - the unit was restarted and hasn't communicated
                // with the host - so we must extract the data and put it in the primary table

                // see if the file has anything in it
                int loRecCount = TempDataRecoveryTable.GetRecCount();

                // there are records in there, lets move em to the main table
                if (loRecCount > 0)
                {
                    // close and reopen the main table
                    CiteStruct.MainTable.CloseTable();

                    // make sure we have enough flash
                    if (QueryUserReclaimFlash(50000) < 50000)
                    {
                        // uh oh... not enough room to copy the records
                        AppMessageBox.ShowMessageWithBell("Insufficient Flash. Unable to write data", "Unload handheld at host.", "");
                        // should this be a fatal application error?
                        return;
                    }

                    loStatus = CiteStruct.MainTable.OpenEditTable(10);
                    if (loStatus < 0)
                    {
                        // uh oh... something is screwed up
                        AppMessageBox.ShowMessageWithBell("Failed opening recovery table.", TranslateFileErrorCode2(loStatus), "");
                        // should this be a fatal application error?
                        return;
                    }

                    for (int loReadIdx = 0; loReadIdx < loRecCount; loReadIdx++)
                    {
                        // read the record
                        if ((loStatus = TempDataRecoveryTable.fTableDef.ReadRecordToBuf(loReadIdx)) < 0)
                        {
                            AppMessageBox.ShowMessageWithBell("Failed reading record.", TranslateFileErrorCode2(loStatus), "");
                        }

                        // copy the record buffer
                        CiteStruct.MainTable.SetRecBuffer(TempDataRecoveryTable.fTableDef.fRecBuffer.ToString());

                        // copy the record buffer to the fields
                        CiteStruct.MainTable.HighTableRevision.Tables[0].CopyRecBufToFieldValues();

                        // write it to the main table
                        if ((loStatus = CiteStruct.MainTable.HighTableRevision.Tables[0].WriteRecord()) < 0)
                        {
                            AppMessageBox.ShowMessageWithBell("Failed writing record.", TranslateFileErrorCode2(loStatus), "");
                        }
                        else
                        {
                            // we saved this one
                            loRecoveredData = (loRecoveredData || true);
                        }
                    }
                }

                // close the recovery table
                TempDataRecoveryTable.fTableDef.CloseTable();

                // and now we dump the recovery table since we've moved the data over
                try
                {
                    if (File.Exists(loFullPathAndFileName))
                        File.Delete(loFullPathAndFileName);
                }
                catch { }

                // notify the user that we've saved something
                if (loRecoveredData == true)
                {
                    AppMessageBox.ShowMultiLineMessageWithBell(CiteStruct.ObjDisplayName + " data has been recovered.\r\n\r\n" +
                        "Use \"" + CiteStruct.ObjDisplayName + " Look Up\"\r\n" +
                        "to review the recovered data.", "Error");
                }
            }
#endif
        }

        public void CleanUpRecoveryFile()
        {
#if _original_
            // only need to bother if we have a recovery file defined
            if (TempDataRecoveryTable != null)
            {
                // if it exists then delete it - it doesn't get created if the form wasn't
                // explicity (via the print button) printed before saving

                string loFullPathAndFileName = "";
                TempDataRecoveryTable.fTableDef.GetTblCompletePathAndFileName(ref loFullPathAndFileName);
                try { File.Delete(loFullPathAndFileName); }
                catch { }
            }
#endif
        }

        /// <summary>
        /// Writes the current structure field values to a TEMPORARY file.  
        /// One that we can recover if the unit crashes before the main file is written
        /// </summary>
        public int WriteRecordToRecoveryFile(TBaseIssForm iForm)
        {
            #if _original_
            int loStatus = 0;

            // no need to bother if we don't have a recovery file defined
            if (TempDataRecoveryTable == null)
                return 1;

            // make sure we have enough flash
            if (QueryUserReclaimFlash(50000) < 50000)
            {
                // uh oh... not enough room to write the records
                AppMessageBox.ShowMessageWithBell("Insufficient Flash. Unable to write data", "Unload handheld at host.", "");
                // should this be a fatal application error?
                return 0;
            }

            // now open the table - the file will be re-created and empty
            loStatus = TempDataRecoveryTable.fTableDef.OpenEditTable(10);

            // openwrite failed. If out of disk space, do a reclaim then try again
            // even with the check about, the FAT could be full and return an error when opening
            if (loStatus == FileErrors.FILE_DISKFULL)
            {
                ReclaimFlash2();
                loStatus = TempDataRecoveryTable.fTableDef.OpenEditTable(10);
                if (loStatus < 0)
                {
                    // didn't work even after a reclaim.. not enough room to write the records
                    AppMessageBox.ShowMessageWithBell("Insufficient Flash. Unable to write data", "Unload handheld at host.", "");
                    // should this be a fatal application error?
                    return 0;
                }
            }

            // get the values from the form
            // use the primary routine that allows us to specify an alternate table
            if (iForm != null)
                IssueFormLogic.ReadFieldValuesFromForm(iForm, TempDataRecoveryTable);

            // write the temp record
            if ((loStatus = TempDataRecoveryTable.WriteRecord()) < 0)
            {
                // write record failed. If out of disk space, do a reclaim then try again
                if (loStatus == FileErrors.FILE_DISKFULL)
                {
                    // free seom space
                    ReclaimFlash2();
                    // try and write again
                    loStatus = TempDataRecoveryTable.WriteRecord();
                }

                // do we still have an error (even after Reclaim, if room was the problem)
                if (loStatus < 0)
                {
                    // didn't work even after a reclaim.. not enough room to write the records
                    AppMessageBox.ShowMessageWithBell("Failed writing record.", TranslateFileErrorCode2(loStatus), "");
                }
            }

            // close the table
            TempDataRecoveryTable.fTableDef.CloseTable();
            return loStatus;
#else
            return 0;
#endif
        }
        #endregion

        /// <summary>
        /// Called at startup, reads in each stored record, and asks the sequence manager to 
        /// confirm that the issue number has been logged.  A defensive measure in case the 
        /// sequence file last issued doesn't reflect what is stored locally.
        /// This problem occurs when the host transmits an out-of-date sequence file.
        /// </summary>
        public void ValidateSequenceLastIssued()
        {
#if _original_
            SequenceImp SeqObj = GetSequence();
            if (SeqObj == null)
                return; // nothing to do.

            int loRecNdx;
            int loResult;
            Int64 loIssueNo = 0;
            string loIssuePfx = "";
            string loIssueSfx = "";
            string loIssueNoStr = "";

            TTTable DataTable = this.CiteStruct.MainTable.HighTableRevision.Tables[0];
            int loLoopMax = DataTable.GetRecCount();
            for (loRecNdx = 0; loRecNdx < loLoopMax; loRecNdx++)
            {
                // read in this record
                if ((loResult = DataTable.ReadRecord(loRecNdx)) != loRecNdx)
                {
                    string loMsg1 = "";
                    loMsg1 = string.Format("Table: {0:s}, Record: {1:d}, Err: {2:d}", this.CiteStruct.Name, loRecNdx, loResult);
                    AppMessageBox.ShowMessageWithBell("ValidateSequenceLastIssued Failed!", loMsg1, "");
                    return;
                }

                // extract the issue number, prefix, and suffix
                DataTable.GetFormattedFieldData(FieldNames.IssueNoPfxFieldName, "", ref loIssuePfx);
                DataTable.GetFormattedFieldData(FieldNames.IssueNoFieldName, "", ref loIssueNoStr);
                DataTable.GetFormattedFieldData(FieldNames.IssueNoSfxFieldName, "", ref loIssueSfx);
                // convert issueno to integer
                TextBoxBehavior.StrTollInt(loIssueNoStr, ref loIssueNo);
                SeqObj.VerifyUsedNumberIsLogged(loIssuePfx, loIssueSfx, loIssueNo);
            }
#endif
        }

        public bool CiteIsReissued(int iPrimaryKey)
        {
            return ((CiteStruct.ReissueStruct != null) &&
                (((ReissueStructLogicAndroid)(CiteStruct.ReissueStruct.StructLogicObj)).FindRecordForMasterKey(iPrimaryKey) >= 0));
        }

        public DataRow GetFirstReissueRecordForMasterKey(int iPrimaryKey)
        {
            if (CiteStruct.ReissueStruct != null)
            {
                return ((ReissueStructLogicAndroid)(CiteStruct.ReissueStruct.StructLogicObj)).GetFirstRecordForMasterKey(iPrimaryKey);
            }
            else
            {
                return null;
            }
        }


        public bool CiteIsAContinuance(int iPrimaryKey)
        {
            return ((CiteStruct.ContinuanceStruct != null) &&
                (((ContinuanceStructLogicAndroid)(CiteStruct.ContinuanceStruct.StructLogicObj)).FindRecordForMasterKey(iPrimaryKey) >= 0));
        }

        public DataRow GetFirstContinuanceRecordForMasterKey(int iPrimaryKey)
        {
            if (CiteStruct.ContinuanceStruct != null)
            {
                return ((ContinuanceStructLogicAndroid)(CiteStruct.ContinuanceStruct.StructLogicObj)).GetFirstRecordForMasterKey(iPrimaryKey);
            }
            else
            {
                return null;
            }
        }



        public bool CiteIsVoid(int iPrimaryKey)
        {
            return ((CiteStruct.VoidStruct != null) &&
                (((VoidStructLogicAndroid)(CiteStruct.VoidStruct.StructLogicObj)).FindRecordForMasterKey(iPrimaryKey) >= 0));
        }

        public DataRow GetFirstVoidRecordForMasterKey(int iPrimaryKey)
        {
            if ( CiteStruct.VoidStruct != null )
            {
                return ((VoidStructLogicAndroid)(CiteStruct.VoidStruct.StructLogicObj)).GetFirstRecordForMasterKey(iPrimaryKey);
            }
            else
            {
                return null;
            }
        }



        public bool VoidRecord(int iEditRecNo)
        {
#if _original_
            TBaseIssForm loForm;

            if (iEditRecNo < 0)
            {
                AppMessageBox.ShowMessageWithBell("No record Selected", "", "");
                return false;
            }

            if (CiteStruct.VoidStruct == null)
            {
                AppMessageBox.ShowMessageWithBell(CiteStruct.ObjDisplayName + " Has No Void Struct", "", "");
                return false;
            }

            loForm = ((IssueStructLogicAndroid)(CiteStruct.VoidStruct.StructLogicObj)).IssueFormLogic.CfgForm;
            if (loForm == null)
            {
                AppMessageBox.ShowMessageWithBell(CiteStruct.ObjDisplayName + " Has No Void Form", "", "");
                return false;
            }

            if (CiteIsVoid(iEditRecNo))
            {
                AppMessageBox.ShowMessageWithBell("Record is already Void!", "", "");
                return false;
            }

            ((IssueStructLogicAndroid)(CiteStruct.VoidStruct.StructLogicObj)).IssueFormLogic.SetMasterKey(iEditRecNo);
            ((IssueStructLogicAndroid)(CiteStruct.VoidStruct.StructLogicObj)).IssueFormLogic.SetCaption("Enter Void Reason:");

            int loResult = ((IssueStructLogicAndroid)(CiteStruct.VoidStruct.StructLogicObj)).IssueFormLogic.FormEdit(EditRestrictionConsts.femSingleEntry, "", null);
            return loResult == FormEditResults.FormEditResult_OK;
#else
            return false;
#endif
        }

        public bool CancelRecord(IssFormBuilder iFromFormBuilder)
        {
#if _original_
            TBaseIssForm loForm;

            if (CiteStruct.CancelStruct == null)
            {
                AppMessageBox.ShowMessageWithBell(CiteStruct.ObjDisplayName + " Has No Cancel Struct", "", "");
                return false;
            }

            loForm = ((IssueStructLogicAndroid)(CiteStruct.CancelStruct.StructLogicObj)).IssueFormLogic.CfgForm;
            if (loForm == null)
            {
                AppMessageBox.ShowMessageWithBell(CiteStruct.ObjDisplayName + " Has No Cancel Form", "", "");
                return false;
            }

            int loResult = 0;
            IssueFormLogic.ReadFieldValuesFromForm(iFromFormBuilder.CfgForm, CiteStruct.MainTable.HighTableRevision.Tables[0]);
            SetIssueSource(((IssueStructLogicAndroid)(CiteStruct.CancelStruct.StructLogicObj)).IssueFormLogic, CiteStruct.MainTable.HighTableRevision.Tables[0]);
            ((IssueStructLogicAndroid)(CiteStruct.CancelStruct.StructLogicObj)).IssueFormLogic.SetCaption("Enter Cancel Reason:");

            loResult = ((IssueStructLogicAndroid)(CiteStruct.CancelStruct.StructLogicObj)).IssueFormLogic.FormEdit(EditRestrictionConsts.femSingleEntry, "", null);

            UndoIssueSource(((IssueStructLogicAndroid)(CiteStruct.CancelStruct.StructLogicObj)).IssueFormLogic);
            ((IssueStructLogicAndroid)(CiteStruct.CancelStruct.StructLogicObj)).IssueFormLogic.fSourceDataStruct = null;
            return loResult == FormEditResults.FormEditResult_OK;
#else
            return false;
#endif
        }

        public int PrintRecord(int iRecordNumber)
        {
#if _original_
            TTForm loForm;

            if (iRecordNumber < 0)
            {
                AppMessageBox.ShowMessageWithBell("No record selected.", "", "");
                return 0;
            }
            TTTable DataTable = this.CiteStruct.MainTable.HighTableRevision.Tables[0];
            // read in the record to print
            DataTable.ReadRecord(iRecordNumber);

            // if the form name is captured in the data, get it so we can pull that form
            loForm = FormByPictureName(DataTable.GetFormattedFieldData(FieldNames.FORMNAMEFieldName, ""));

            if (loForm == null)
            {
                AppMessageBox.ShowMessageWithBell("No ISSUE form!", "", "");
                return 0;
            }

            this.IssueFormLogic.PrintForm(false);
            return 1;
#else
            return 0;
#endif
        }

        public short AddNotes(int iEditRecNo)
        {
#if _original_
            TBaseIssForm loForm;

            if (CiteStruct.NotesStruct == null)
            {
                AppMessageBox.ShowMessageWithBell(CiteStruct.ObjDisplayName + " Has No notes struct", "", "");
                return 0;
            }

            // copy current issue number to notes struct...
            loForm = ((IssueStructLogicAndroid)(CiteStruct.NotesStruct.StructLogicObj)).IssueFormLogic.CfgForm;
            if (loForm != null)
            {
                // let the notes form know which cite is having its notes edited. 
                ((IssueStructLogicAndroid)(CiteStruct.NotesStruct.StructLogicObj)).IssueFormLogic.SetMasterKey(iEditRecNo);
                ((IssueStructLogicAndroid)(CiteStruct.NotesStruct.StructLogicObj)).IssueFormLogic.StartPanel =
                    ((IssueStructLogicAndroid)(CiteStruct.NotesStruct.StructLogicObj)).IssueFormLogic.EntryPanel;
                ((IssueStructLogicAndroid)(CiteStruct.NotesStruct.StructLogicObj)).IssueFormLogic.StartField = null;
                ((IssueStructLogicAndroid)(CiteStruct.NotesStruct.StructLogicObj)).IssueFormLogic.FormEdit(EditRestrictionConsts.femSingleEntry, "", null);

                return 0;
            }
            else
            {
                AppMessageBox.ShowMessageWithBell(CiteStruct.ObjDisplayName + " Has No notes form.", "", "");
            }
            return 0;
#else
            return 0;
#endif
        }

        public int SaveReissuedRec(int iRecordNumber)
        {
            // save the supplementary reissue record
            return ((ReissueStructLogicAndroid)(CiteStruct.ReissueStruct.StructLogicObj)).SaveReissueStructRec(iRecordNumber,
                this.IssueStruct.MainTable.HighTableRevision.Tables[0]);
        }

        public bool ReissueRecord(int iRecordNumber)
        {
#if _original_
            TTForm loForm;

            if (iRecordNumber < 0)
            {
                AppMessageBox.ShowMessageWithBell("No record selected.", "", "");
                return false;
            }

            if (CiteIsReissued(iRecordNumber))
            {
                AppMessageBox.ShowMessageWithBell("Record already reissued!", "", "");
                return false;
            }
            TTTable DataTable = this.CiteStruct.MainTable.HighTableRevision.Tables[0];
            // make them void it if it isn't already (void would read in the record, so do it if voiding has already been done
            DataTable.ReadRecord(iRecordNumber);

            // if the form name is captured in the data, get it so we can pull that form
            loForm = FormByPictureName(DataTable.GetFormattedFieldData(FieldNames.FORMNAMEFieldName, ""));

            if (loForm == null)
            {
                AppMessageBox.ShowMessageWithBell("No ISSUE form!", "", "");
                return false;
            }

            if (!CiteIsVoid(iRecordNumber) && (!VoidRecord(iRecordNumber)))
                return false;

            WriteFieldValuesToForm(loForm);

            // tell the issue form who is being reissued
            this.IssueFormLogic.fReissuedKey = iRecordNumber;
            if (IssueRecord(EditRestrictionConsts.femReissue, "", loForm.Name) != FormEditResults.FormEditResult_OK)
                return false;

            return true;
#else
            return false;
#endif
        }

        public bool IssueContinuance(int iRecordNumber)
        {
#if _original_
            TTForm loForm = null;
            string loSrcIssueNo = "";
            string loSrcIssueNoPfx = "";
            string loSrcIssueNoSfx = "";

            if (iRecordNumber < 0)
            {
                AppMessageBox.ShowMessageWithBell("No record selected.", "", "");
                return false;
            }

            if (CiteIsVoid(iRecordNumber))
            {
                AppMessageBox.ShowMessageWithBell("Record is Void!", "", "");
                return false;
            }

            if (CiteIsAContinuance(iRecordNumber))
            {
                AppMessageBox.ShowMessageWithBell("Record is a continuance!", "", "");
                return
                    false;
            }

            // if this record has been reissued, make the user select another.
            if (CiteIsReissued(iRecordNumber))
            {
                AppMessageBox.ShowMessageWithBell("Record has been reissued!", "", "");
                return false;
            }

            TTTable DataTable = this.CiteStruct.MainTable.HighTableRevision.Tables[0];
            DataTable.ReadRecord(iRecordNumber);
            // if the form name is captured in the data, get it so we can pull that form
            loForm = FormByPictureName(DataTable.GetFormattedFieldData(FieldNames.FORMNAMEFieldName, ""));
            if (loForm == null)
            {
                AppMessageBox.ShowMessageWithBell("No ISSUE form!", "", "");
                return false;
            }

            // extract the record to continue's issue number... 
            DataTable.GetFormattedFieldData(FieldNames.IssueNoFieldName, "", ref loSrcIssueNo);
            DataTable.GetFormattedFieldData(FieldNames.IssueNoPfxFieldName, "", ref loSrcIssueNoPfx);
            DataTable.GetFormattedFieldData(FieldNames.IssueNoSfxFieldName, "", ref loSrcIssueNoSfx);

            WriteFieldValuesToForm(loForm);

            string loFirstFocusField = "";
            if (IssueFormLogic.CfgIssForm != null)
                loFirstFocusField = IssueFormLogic.CfgIssForm.IssueMoreFirstFocus.ToUpper();
            if (IssueRecord(EditRestrictionConsts.femContinuance, loFirstFocusField, "") == FormEditResults.FormEditResult_OK)
            {
                // save the supplementary continuance record
                ((ContinuanceStructLogic)(CiteStruct.ContinuanceStruct.StructLogicObj)).SaveContinuanceStructRec(loSrcIssueNo, loSrcIssueNoPfx, loSrcIssueNoSfx, DataTable);
            }
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// Routine called by a ticket ISSUE form when data elements on form need to default to
        /// values from this structure.  More specifically, this routine is called during
        /// citation issue form initialization when a ticket is being issued as a result of a mark
        /// mode match.
        /// Our job will be to initialize various fields w/ values mark mode.  Our intialization will
        /// be as follows:
        /// Ignore ISSUEDATE and ISSUETIME
        /// Copy all remaining fields in common verbatim, and protect them.
        /// A future enhancement would be to place an "SearchVio" filter on the violations table
        /// to force the user to select only appropriate violations.
        /// </summary>
        public override int InitFormDataField(Android.App.Fragment iFormBuilder, TTableFldDef iStructFld, string iToFormFieldName, string iToFormFieldValue)
        {
#if _original_
            // IssueNo gets mapped to "SrcIssueNo" 
            if (string.Compare(iStructFld.Name, FieldNames.IssueNoFieldName) == 0)
                return base.InitFormDataField(iFormBuilder, iStructFld, FieldNames.SrcIssueNoFieldName, "");
            else if (string.Compare(iStructFld.Name, FieldNames.IssueNoPfxFieldName) == 0)
                return base.InitFormDataField(iFormBuilder, iStructFld, FieldNames.SrcIssueNoPfxFieldName, "");
            else if (string.Compare(iStructFld.Name, FieldNames.IssueNoSfxFieldName) == 0)
                return base.InitFormDataField(iFormBuilder, iStructFld, FieldNames.SrcIssueNoSfxFieldName, "");
            else if (string.Compare(iStructFld.Name, FieldNames.IssueTimeFieldName) == 0)
                return base.InitFormDataField(iFormBuilder, iStructFld, FieldNames.SrcIssueTimeFieldName, "");
            else if (string.Compare(iStructFld.Name, FieldNames.IssueDateFieldName) == 0)
                return base.InitFormDataField(iFormBuilder, iStructFld, FieldNames.SrcIssueDateFieldName, "");

            return base.InitFormDataField(iFormBuilder, iStructFld, iStructFld.Name, "");
#else
            return 0;
#endif
        }

        private void FillListWithMuliMediaFiles(List<string> oFileList)
        {
#if _original_

            // Figure out where the source picture files are located
            TTMultimedia fMultimediaTargetType = TTMultimedia.mmPicture;
            string loSourceFolderPath = "";
            string loSourceFileExt = CN_PICTURE_FILE_EXT;
            GetMultimediaFolder(ref loSourceFolderPath, fMultimediaTargetType, false);
            // Add list of current picture files
            OSGetFolderListing(loSourceFolderPath, loSourceFileExt, oFileList, false);

            // Figure out where the source audio files are located
            fMultimediaTargetType = TTMultimedia.mmAudio;
            loSourceFolderPath = "";
            loSourceFileExt = CN_AUDIO_FILE_EXT;
            GetMultimediaFolder(ref loSourceFolderPath, fMultimediaTargetType, false);
            // Append list of current audio files
            OSGetFolderListing(loSourceFolderPath, loSourceFileExt, oFileList, true);
#endif
        }

        public void RefreshInitialMultimediaFilesList()
        {
#if _original_
            // Create stringlist to hold list of multimedia files existing at start of ticket
            if (InitialFilesList == null)
                InitialFilesList = new List<string>();
            InitialFilesList.Clear();
            FillListWithMuliMediaFiles(InitialFilesList);
#endif
        }

        static public int OSGetFolderListing(string iPathName, string iExt, List<string> oFileList, bool AppendFilesToList)
        {
#if _original_
            // Clear out all the previous files unless we're appending to existing list
            if (AppendFilesToList == false)
                oFileList.Clear();

            DirectoryInfo dir = new DirectoryInfo(iPathName);
            if (dir.Exists)
            {
                FileInfo[] MatchFiles = dir.GetFiles("*" + iExt);
                foreach (FileInfo MatchFile in MatchFiles)
                    oFileList.Add(MatchFile.Name);
            }
            return 0;
#else
            return 0;
#endif
        }

        public short AutoAttachMMNotes(object iSender)
        //public short AutoAttachMMNotes(ReinoNavButton iSender)
        {
#if _original_
            // We don't belong in here if its the NotesForm or there is no Notes button
            if (((this.IssueFormLogic.CfgForm is TNotesForm) == true) || (this.IssueFormLogic.BtnNotes == null))
            {
                this.IssueFormLogic.fDidAutoAttachProcedure = true;
                return 0;
            }

            // Exit if we've already been through here once or if currently inside it
            if ((this.IssueFormLogic.fDidAutoAttachProcedure == true) ||
                (this.IssueFormLogic.fInsideAutoAttachProcedure == true))
                return 0;

            // Get the associated Notes structure and Notes entry form
            TNotesStruct NotesStruct;
            TNotesForm loNotesForm;
            NotesStruct = this.CiteStruct.NotesStruct;
            loNotesForm = (NotesStruct.StructLogicObj as IssueStructLogicAndroid).GetIssueForm(NotesStruct)
                as TNotesForm;
            NotesFormLogic loNotesFormLogic = (NotesStruct.StructLogicObj as IssueStructLogicAndroid).IssueFormLogic as NotesFormLogic;
            // Exit if there is no Notes structure or Notes entry form to work with
            if ((NotesStruct == null) || (loNotesForm == null))
                return 0;

            //  Declare local variables
            List<string> CurrentFilesList;
            List<string> NewFilesList;
            TTMultimedia fMultimediaTargetType;
            int loNdx;
            string loFoundFileName = "";
            short loDlgResult = FormEditResults.FormEditResult_OK;
            TNewMMFilesForm loNewMMFilesForm;
            DateTime loSystemTime = DateTime.Now;
            string loMMTypeStr = "";
            TIssEdit loMultimediaNoteFileDateStamp = null;
            TIssEdit loMultimediaNoteFileTimeStamp = null;
            string loBuff1 = "";
            string loBuff2 = "";
            string loBuffer = "";
            string loExt = "";
            int loAutoAttachFileCountRegVal = 0;
            AppMessageBox loWaitMsgForm = null;

            // Set flag so we won't enter this routine again
            this.IssueFormLogic.fDidAutoAttachProcedure = true;
            this.IssueFormLogic.fInsideAutoAttachProcedure = true;

            // Create stringlists needed for determining which files are new
            CurrentFilesList = new List<string>();
            NewFilesList = new List<string>();

            // Get list of current files
            FillListWithMuliMediaFiles(CurrentFilesList);

            // Compare current list to initial list to determine which ones are new 
            for (loNdx = 0; loNdx < CurrentFilesList.Count; loNdx++)
            {
                loFoundFileName = CurrentFilesList[loNdx];
                // If filename wasn't in the initial list, add it to the list of new files
                if (InitialFilesList.IndexOf(loFoundFileName) == -1)
                    NewFilesList.Add(loFoundFileName);
            }
            // Clear the current file list since we're done with it now
            CurrentFilesList.Clear();

            // We don't want the "Initializing" box popping up all the time 
            loNotesFormLogic.fSuppressInitMessageBox = true;
            // But we do want our own message box that persists until done with all loop iterations
            loWaitMsgForm = AppMessageBox.ShowMessageNotModal("", "", null/*this.IssueFormLogic.WinForm*/);
            loWaitMsgForm.SetMsgAndPaint("", "Attaching Multimedia Notes", "Please Wait...", "", "");

            // Read from Registry to see if we should auto-attach files without prompting
            loAutoAttachFileCountRegVal = TTRegistry.glRegistry.GetRegistryValueAsInt("ISSUE_AP", "AUTOATTACHFILECOUNT", 0);

            // Loop through each item in the new files list
            for (loNdx = 0; loNdx < NewFilesList.Count; loNdx++)
            {
                // Repaint the "Please Wait" dialog 
                loBuffer = "";
                loBuff1 = Convert.ToString(loNdx + 1);
                loBuff2 = Convert.ToString(NewFilesList.Count);
                loBuffer = "(" + loBuff1 + " of " + loBuff2 + ")";
                if (NewFilesList.Count <= loAutoAttachFileCountRegVal)
                    loWaitMsgForm.SetMsgAndPaint("", "Attaching Multimedia Notes", "Please Wait...", "", loBuffer);
                else
                    loWaitMsgForm.Invalidate();

                // Get next filename and extension from the list
                loFoundFileName = NewFilesList[loNdx];
                loExt = Path.GetExtension(loFoundFileName);

                // If the file extension is ".WAV", we know its audio.
                // If should be fine to assume other extensions are pictures, especially
                // since images commonly can be ".JPG" or ".JPEG"
                if (string.Compare(loExt, CN_AUDIO_FILE_EXT, true) == 0)
                {
                    loNewMMFilesForm = new TNewMMFilesForm();
                    loNewMMFilesForm.Line2.Text = "A new audio file was found.";
                    loNewMMFilesForm.Line3.Text = "Add this recording to a new note?";
                    loNewMMFilesForm.Line4.Text = NewFilesList[loNdx];
                    loNewMMFilesForm.SetCaption("New Recording Found");
                    loNewMMFilesForm.fMultimediaNoteDataType = TTMultimedia.mmAudio;
                    fMultimediaTargetType = TTMultimedia.mmAudio;
                    loNewMMFilesForm.fMMFilename = loFoundFileName;
                }
                else
                {
                    loNewMMFilesForm = new TNewMMFilesForm();
                    loNewMMFilesForm.Line2.Text = "A new image file was found.";
                    loNewMMFilesForm.Line3.Text = "Add this image to a new note?";
                    loNewMMFilesForm.Line4.Text = NewFilesList[loNdx];
                    loNewMMFilesForm.SetCaption("New Picture Found");
                    loNewMMFilesForm.fMultimediaNoteDataType = TTMultimedia.mmPicture;
                    fMultimediaTargetType = TTMultimedia.mmPicture;
                    loNewMMFilesForm.fMMFilename = loFoundFileName;
                }

                // Are we supposed to prompt the user or do it automatically? 
                if (NewFilesList.Count <= loAutoAttachFileCountRegVal)
                {
                    loDlgResult = FormEditResults.FormEditResult_OK;
                }
                else
                {
                    loNewMMFilesForm.UpdatePreviewThumbnail();
                    loDlgResult = loNewMMFilesForm.FormEdit();
                }
                if (loNewMMFilesForm != null)
                    loNewMMFilesForm.Close();
                if (loNewMMFilesForm != null)
                    loNewMMFilesForm.Dispose();

                // Does the user want to add this file as a new note?
                if (loDlgResult == FormEditResults.FormEditResult_OK)
                {
                    // let the notes form know which cite is having its notes edited
                    loNotesFormLogic.SetMasterKey(this.GetEditRecNo());

                    // Set flag indicating multimedia file should be attached automatically
                    loNotesFormLogic.FileWasAutoAttached = true;

                    if (NewFilesList.Count <= loAutoAttachFileCountRegVal)
                        loNotesFormLogic.AutoSelectDoneButton = true;
                    else
                        loNotesFormLogic.AutoSelectDoneButton = false;

                    // Init strings
                    loNotesFormLogic.AutoAttachMMNoteFilename = "";
                    loNotesFormLogic.AutoAttachMMNoteDataType = "";
                    loNotesFormLogic.AutoAttachMMNoteFileDateStamp = "";
                    loNotesFormLogic.AutoAttachMMNoteFileTimeStamp = "";

                    // This routine will move the multimedia file from RAM to Flash and rename it
                    loNotesFormLogic.AutoAttachMMNoteFilename = NewFilesList[loNdx];
                    MultimediaBrowse.ce_AttachOrRemoveMultimediaFile(ref loNotesFormLogic.AutoAttachMMNoteFilename,
                        fMultimediaTargetType, ref loSystemTime, false, loNdx,
                        (loNotesFormLogic.AutoSelectDoneButton == false), null/*this.IssueFormLogic.WinForm*/);

                    // Set the attachment type on the Notes form
                    if (fMultimediaTargetType == TTMultimedia.mmPicture)
                    {
                        loMMTypeStr = TTMultimedia.mmPicture.ToString();
                        loNotesFormLogic.AutoAttachMMNoteDataType = loMMTypeStr;
                    }
                    else
                    {
                        loMMTypeStr = TTMultimedia.mmAudio.ToString();
                        loNotesFormLogic.AutoAttachMMNoteDataType = loMMTypeStr;
                    }

                    // Find the edit fields for the datestamp and timestamp
                    TIssEdit loCfgEdit = loNotesFormLogic.FindCfgControlByName(FieldNames.MultimediaNoteFileDateStampFieldName) as TIssEdit;
                    if (loCfgEdit != null)
                        loMultimediaNoteFileDateStamp = loCfgEdit;
                    loCfgEdit = loNotesFormLogic.FindCfgControlByName(FieldNames.MultimediaNoteFileTimeStampFieldName) as TIssEdit;
                    if (loCfgEdit != null)
                        loMultimediaNoteFileTimeStamp = loCfgEdit;

                    // Convert the filetime to a local filetime
                    string loPathName = "";
                    IssueStructLogicAndroid.GetMultimediaFolder(ref loPathName, TTMultimedia.mmPicture, true);
                    string loFileName = System.IO.Path.Combine(loPathName, loNotesFormLogic.AutoAttachMMNoteFilename);
                    FileInfo fi = new FileInfo(loFileName);
                    loSystemTime = fi.CreationTime;

                    // Set the datestamp for the attachment
                    if (loMultimediaNoteFileDateStamp != null)
                    {
                        loBuffer = "";
                        TextBoxBehavior.OSDateToDateString(loSystemTime, 
                            loMultimediaNoteFileDateStamp.Behavior.GetEditMask(), ref loBuffer);
                        loNotesFormLogic.AutoAttachMMNoteFileDateStamp = loBuffer;
                    }

                    // Set the timestamp for the attachment
                    if (loMultimediaNoteFileTimeStamp != null)
                    {
                        loBuffer = "";
                        TextBoxBehavior.OSTimeToTimeString(loSystemTime, 
                            loMultimediaNoteFileTimeStamp.Behavior.GetEditMask(), ref loBuffer);
                        loNotesFormLogic.AutoAttachMMNoteFileTimeStamp = loBuffer;
                    }

                    // Now call the Note form's FormEdit method
                    if (loNotesFormLogic.AutoSelectDoneButton == true)
                        loNotesFormLogic.WinForm.Visible = false;
                    loNotesFormLogic.StartPanel = loNotesFormLogic.EntryPanel;
                    loNotesFormLogic.StartField = null;
                    loNotesFormLogic.FormEdit(EditRestrictionConsts.femSingleEntry, "", null);
                }
            }
            // Finished with all iterations, so we can free the list of new files
            NewFilesList.Clear();

            // Clear the flag so we know we're not in the loop iterations anymore
            this.IssueFormLogic.fInsideAutoAttachProcedure = false;

            // Clear flag so files aren't auto-attached when user enters notes the next time
            loNotesFormLogic.FileWasAutoAttached = false;
            loNotesFormLogic.AutoSelectDoneButton = false;

            // Now reset the SuppressInitMessageBox back to normal
            loNotesFormLogic.fSuppressInitMessageBox = false;
            loNotesFormLogic.WinForm.Visible = false;

            // Delete our "Please Wait" message box
            if (loWaitMsgForm != null)
                loWaitMsgForm.Close();
            if (loWaitMsgForm != null)
                loWaitMsgForm.Dispose();

            // Init strings
            loNotesFormLogic.AutoAttachMMNoteFilename = "";
            loNotesFormLogic.AutoAttachMMNoteDataType = "";
            loNotesFormLogic.AutoAttachMMNoteFileDateStamp = "";
            loNotesFormLogic.AutoAttachMMNoteFileTimeStamp = "";

            // Refresh the Initial Files list so the "PendingAttachments" field gets current info
            RefreshInitialMultimediaFilesList();

            if (this.IssueFormLogic.EditPendingAttachments != null)
                UpdatePendingAttachments();

            // Enable the buttons as appropriate for current edit conditions.
            this.IssueFormLogic.SetButtonStates();
            iSender.Focus();
            return 0;
#else
            return 0;
#endif
        }

        public short UpdatePendingAttachments()
        {
#if _original_

            // Exit if there is no list of initial files
            if (InitialFilesList == null)
                return 0;
            // Exit if there is not Pending Attachments control
            if (this.IssueFormLogic.EditPendingAttachments == null)
                return 0;
            // We don't belong in here if its the NotesForm or there is no Notes button
            if ((this.IssueFormLogic.CfgForm is TNotesForm) || (this.IssueFormLogic.BtnNotes == null))
                return 0;

            // This has already been checked by our caller
            /*
            // No need to waste processor time if the field isn't showing...
            if ((this.IssueFormLogic.EditPendingAttachments.Behavior.EditCtrl == null) ||
                (!this.IssueFormLogic.EditPendingAttachments.Behavior.EditCtrl.Visible))
                return 0;
            */

            // Get the associated Notes structure and Notes entry form
            TNotesStruct NotesStruct = null;
            TIssForm loNotesForm = null;
            NotesStruct = this.CiteStruct.NotesStruct;
            loNotesForm = ((IssueStructLogicAndroid)(NotesStruct.StructLogicObj)).GetIssueForm(NotesStruct);
            // Exit if there is no Notes structure or Notes entry form to work with
            if ((NotesStruct == null) || (loNotesForm == null))
                return 0;

            //  Declare local variables
            List<string> CurrentFilesList;
            List<string> NewFilesList;
            int loNdx;
            string loFoundFileName = "";
            string loExt = "";
            string loTempBuffer = "";
            string loBuffer = "";
            int loNewJpgCount = 0;
            int loNewWavCount = 0;

            // Create stringlists needed for determining which files are new
            CurrentFilesList = new List<string>();
            NewFilesList = new List<string>();

            // Get list of current files
            FillListWithMuliMediaFiles(CurrentFilesList);

            // Compare current list to initial list to determine which ones are new 
            for (loNdx = 0; loNdx < CurrentFilesList.Count; loNdx++)
            {
                loFoundFileName = CurrentFilesList[loNdx];
                // If filename wasn't in the initial list, add it to the list of new files
                if (InitialFilesList.IndexOf(loFoundFileName) == -1)
                    NewFilesList.Add(loFoundFileName);
            }
            // Free the list of current files since we're done with it now
            CurrentFilesList.Clear();

            // Loop through each item in the new files list
            for (loNdx = 0; loNdx < NewFilesList.Count; loNdx++)
            {
                // Get next filename from the list
                loFoundFileName = NewFilesList[loNdx];

                // Get the file extension so we can determine what type of file it is
                loExt = Path.GetExtension(loFoundFileName);

                // If the file extension is ".WAV", we know its audio.
                if (string.Compare(loExt, CiteStructLogicAndroid.CN_AUDIO_FILE_EXT, true) == 0)
                    loNewWavCount = loNewWavCount + 1;
                else
                    loNewJpgCount = loNewJpgCount + 1;
            }
            // Finished with all iterations, so we can free the list of new files
            NewFilesList.Clear();

            // Build display string for the "PendingAttachments" field
            loBuffer = "";
            loTempBuffer = loNewJpgCount.ToString();
            loBuffer += loTempBuffer + " Picture, ";
            loTempBuffer = loNewWavCount.ToString();
            loBuffer += loTempBuffer + " Audio";

            // We can skip the update if the text hasn't changed
            if (loBuffer.Equals(this.IssueFormLogic.EditPendingAttachments.Behavior.EditBuffer))
                return 0;

            this.IssueFormLogic.EditPendingAttachments.Behavior.SetEditBufferAndPaint(loBuffer);

            // Update the edit control if its present
            if (this.IssueFormLogic.EditPendingAttachments.Behavior.EditCtrl != null)
            {
                this.IssueFormLogic.EditPendingAttachments.Behavior.EditCtrl.Invalidate();
            }
            return 1;
#else
            return 0;
#endif
        }
    }
}
