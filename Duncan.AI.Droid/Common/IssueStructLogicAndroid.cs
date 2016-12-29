


using Android.OS;

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

    public static class FieldNames
    {
        // AJW TODO - multiple defn of column names floating around, need to consolidate into single source
        public const string IssueDateFieldName = "ISSUEDATE";
        public const string IssueTimeFieldName = "ISSUETIME";
        public const string SrcIssueDateFieldName = "SRCISSUEDATE";
        public const string SrcIssueTimeFieldName = "SRCISSUETIME";
        public const string RecCreationDateFieldName = "RECCREATIONDATE";
        public const string RecCreationTimeFieldName = "RECCREATIONTIME";
        public const string NoteDateFieldName = "NOTEDATE";
        public const string NoteTimeFieldName = "NOTETIME";

        public const string IssueNoFieldName = "ISSUENO";
        public const string IssueNoPfxFieldName = "ISSUENOPFX";
        public const string IssueNoSfxFieldName = "ISSUENOSFX";
        public const string SrcIssueNoFieldName = "SRCISSUENO";
        public const string SrcIssueNoPfxFieldName = "SRCISSUENOPFX";
        public const string SrcIssueNoSfxFieldName = "SRCISSUENOSFX";
        public const string IssueNoDisplayFieldName = "ISSUENO_DISPLAY";
        public const string ControlNoFieldName = "CONTROLNO";
        public const string OrderTypeFieldName = "ORDERTYPE";

        public const string MasterKeyFieldName = "MasterKey";
        public const string DetailRecNoFieldName = "DetailRecNo";

        public const string FORMREVFieldName = "FORMREV";
        public const string FORMNAMEFieldName = "FORMNAME";
        public const string HHSerialNoFieldName = "UNITSERIAL"; //used to be "HHSERIALNO", but no configs ever used it

        public const string LOGStartDateFieldName = "STARTDATE";
        public const string LOGStartTimeFieldName = "STARTTIME";
        public const string LOGEndDateFieldName = "ENDDATE";
        public const string LOGEndTimeFieldName = "ENDTIME";
        public const string LOGPrimaryActivityNameFieldName = "PRIMARYACTIVITYNAME";
        public const string LOGPrimaryActivityCountFieldName = "PRIMARYACTIVITYCOUNT";
        public const string LOGSecondaryActivityNameFieldName = "SECONDARYACTIVITYNAME";
        public const string LOGSecondaryActivityCountFieldName = "SECONDARYACTIVITYCOUNT";

        public const string OfficerNameFieldName = "OFFICERNAME";
        public const string OfficerIDFieldName = "OFFICERID";

        public const string CancelReasonFieldName = "CANCELREASON";

        public const string VehLicNoFieldName = "VEHLICNO";
        public const string VehLicStateFieldName = "VEHLICSTATE";
        public const string VehVINFieldName = "VEHVIN";
        public const string VehLicExpFieldName = "VEHLICEXPDATE";
        public const string VehYearFieldName = "VEHYEARDATE";
        public const string VehMakeFieldName = "VEHMAKE";
        public const string VehModelFieldName = "VEHMODEL";
        public const string VehTypeFieldName = "VEHTYPE";

        public const string VehColor1FieldName = "VEHCOLOR1";
        public const string VehColor2FieldName = "VEHCOLOR2";
        public const string SuspHairColorFieldName = "SUSPHAIRCOLOR";
        public const string SuspEyeColorFieldName = "SUSPEYECOLOR";

        public const string ChalkTimeFieldName = "CHALKTIME";
        public const string ChalkDateFieldName = "CHALKDATE";

        public const string Remark1FieldName = "REMARK1";
        public const string Remark2FieldName = "REMARK2";
        public const string ElapsedTimeFieldName = "TIME_ELAPSE";
        public const string LocationFieldName = "LOCATION_DISPLAY";

        public const string LocMeterFieldName = "LOCMETER";
        public const string LocMeterBayNoFieldName = "METERBAYNO";
        public const string BtnReadReinoFieldName = "BTNREADREINO";

        public const string PubContActionTakenFieldName = "ACTIONTAKEN";
        public const string PubContActionTakenStructFieldName = "ACTIONTAKENSTRUCT";
        public const string PubContActionTakenFormFieldName = "ACTIONTAKENFORM";

        public const string VioSelectFieldName = "VIOSELECT";
        public const string VioCodeFieldName = "VIOCODE";
        public const string VioDesc1FieldName = "VIODESCRIPTION1";
        public const string VioFineFieldName = "VIOFINE";

        public const string IsWarningFieldName = "ISWARNING";
        public const string VoidDateTimeFieldName = "VOIDDATETIME";
        public const string ReissuedByFieldName = "REISSUED_BY";

        public const string NotesMemoFieldName = "NOTESMEMO";

        public const string MatchRecsFieldName = "MATCHRECS";
        public const string HotDispoFieldName = "HOTDISPO";

        public const string MultimediaNoteFilenameFieldName = "MULTIMEDIANOTEFILENAME";
        public const string MultimediaNoteDataTypeFieldName = "MULTIMEDIANOTEDATATYPE";
        public const string MultimediaNoteFileDateStampFieldName = "MULTIMEDIANOTEFILEDATESTAMP";
        public const string MultimediaNoteFileTimeStampFieldName = "MULTIMEDIANOTEFILETIMESTAMP";
        public const string PendingAttachmentsFieldName = "PENDINGATTACHMENTS";

        public const string SuspLastNameFieldName = "SUSPLASTNAME";
        public const string SuspFirstNameFieldName = "SUSPFIRSTNAME";
        public const string SuspMidNameFieldName = "SUSPMIDNAME";
        public const string SuspSfxNameFieldName = "SUSPSFXNAME";
        public const string SuspAddrStreetNoFieldName = "SUSPADDRSTREETNO";
        public const string SuspAddrStreetFieldName = "SUSPADDRSTREET";
        public const string SuspAddrAptNoFieldName = "SUSPADDRAPTNO";
        public const string SuspAddrCityFieldName = "SUSPADDRCITY";
        public const string SuspAddrStateFieldName = "SUSPADDRSTATE";
        public const string SuspAddrZipFieldName = "SUSPADDRZIP";
        public const string SuspGenderFieldName = "SUSPGENDER";
        public const string SuspRaceFieldName = "SUSPRACE";
        public const string SuspHeightFieldName = "SUSPHEIGHT";
        public const string SuspWeightFieldName = "SUSPWEIGHT";
        public const string SuspOtherIDFieldName = "SUSPOTHERID";
        public const string SuspSignatureFieldName = "SUSPSIGNATURE";
        public const string OfficerSignatureFieldName = "OFFICERSIGNATURE";
        public const string SuspSSN = "SUSPSSN";

        public const string DLNumFieldName = "DLNUM";
        public const string DLStateFieldName = "DLSTATE";
        public const string DLEXPDATEFieldName = "DLEXPDATE";
        public const string DLClassFieldName = "DLCLASS";
        public const string DLDATEOFBIRTHFieldName = "DLBIRTHDATE";
        public const string DLRestrictionsFieldName = "DLRESTRICTIONS";
        public const string DLEndorsementsFieldName = "DLENDORSEMENTS";

        public const string ROLastNameFieldName = "ROLASTNAME";
        public const string ROFirstNameFieldName = "ROFIRSTNAME";
        public const string ROMidNameFieldName = "ROMIDNAME";
        public const string ROSfxNameFieldName = "ROSFXNAME";
        public const string ROAddrStreetNoFieldName = "ROADDRSTREETNO";
        public const string ROAddrStreetFieldName = "ROADDRSTREET";
        public const string ROAddrAptNoFieldName = "ROADDRAPTNO";
        public const string ROAddrCityFieldName = "ROADDRCITY";
        public const string ROAddrStateFieldName = "ROADDRSTATE";
        public const string ROAddrZipFieldName = "ROADDRZIP";

        public const string SigRequiredNoFieldName = "SIGNATUREREQD";
        public const string MisdemeanorFieldName = "MISDEMEANOR";
    }


    public static class FileErrors
    {
        public const int FILE_ALREADY_EXISTS = -1;
        public const int FILE_IN_USE = -2;
        public const int FILE_NOT_FOUND = -3;
        public const int FILE_TOO_MANY_OPEN = -4;
        public const int FILE_CORRUPTED = -5;
        public const int FILE_FAILED_FAT_UPDATE = -6;
        public const int FILE_CROSSLINKED_FRAGS = -7;
        public const int FILE_TOOMANY_FREE_BLOCKS = -8;
        public const int FILE_DISKFULL = -9;
        public const int FILE_WRITE_FAILED = -10;
        public const int FILE_SYSTEM_NOT_INITED = -11;
        public const int FILE_CANNOT_EXTEND_FILE = -12;
        public const int FILE_INVALID_FILETYPE = -13;
        public const int FILE_RUL_FULL = -14;
        public const int FILE_RUL_INVALID_HANDLE = -15;
        public const int FILE_READONLY = -16;
        public const int FILE_CORRUPT_CHUNK_HDR = -17;
        public const int FILE_ILLEGAL_OPENMODE = -18;
    }

    public class IssueStructLogicAndroid
    {
        #region Public members
        public TIssStruct IssueStruct = null;
        public int ActiveIssueFormIdx = 1;

        public Android.App.Fragment SelectFormLogic = null;
        public string SelectFormLogicFragmentTag = string.Empty;

        public SearchParameterPacket fSearchAndIssueResult = null;


#if _original_
        public Form CallingForm = null;
#else
        public FormPanel CallingFormPanel = null;
#endif


        public Android.App.Fragment IssueFormLogic
        {
            get
            {
                if (this.ActiveIssueFormIdx == 1)
                    return this.IssueForm1Logic;
                else if (this.ActiveIssueFormIdx == 2)
                    return this.IssueForm2Logic;
                else
                    return null;
            }
        }

        public bool HasNthIssueForm(int NthIndex)
        {
            if ((NthIndex == 1) && (this.IssueForm1Logic != null))
                return true;
            
            if ((NthIndex == 2) && (this.IssueForm2Logic != null))
                return true;

            return false;
        }

        public Android.App.Fragment IssueForm1Logic = null;
        public Android.App.Fragment IssueForm2Logic = null;
        public Android.App.Fragment IssueReviewDetailLogic = null;
        public Android.App.Fragment NotesFormLogic = null;
        public Android.App.Fragment NotesDetailFormLogic = null;

        public string IssueForm1LogicFragmentTag = string.Empty;
        public string IssueForm2LogicFragmentTag = string.Empty;
        public string IssueReviewDetailLogicFragmentTag = string.Empty;
        public string NotesFormLogicFragmentTag = string.Empty;
        public string NotesDetailFormLogicFragmentTag = string.Empty;


        #endregion

        #region Private members
        private int fEditRecNo = 0;
        #endregion

#if WindowsCE //|| __ANDROID__
        [DllImport("coredll")]
        private static extern bool GetDiskFreeSpaceEx(string directoryName, ref long freeBytesAvailable, ref long totalBytes, ref long totalFreeBytes);

        [DllImport("coredll.dll", EntryPoint = "CreateFile", SetLastError = true)]
        private static extern int CreateFileCE(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            int lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            int hTemplateFile);

        [DllImport("coredll.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
        private static extern int DeviceIoControlCE(
            IntPtr hDevice,
            uint dwIoControlCode,
            byte[] lpInBuffer,
            int nInBufferSize,
            byte[] lpOutBuffer,
            int nOutBufferSize,
            ref int lpBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);
#endif

        /// <summary>
        /// Global instance of "Checking Available Flash" dialog. We'll keep this one created 
        /// to speed things up since its a commonly displayed window.
        /// </summary>
#if _original_
        private static AppMessageBox glCheckingFlashDlg = null;
#endif

        private long GetTotalDiskFree(ref long oDiskFree, ref int oDiskDeleted)
        {
#if WindowsCE // || __ANDROID__
            // DEBUG -- should be moved to IssueApLogic?
            long loFreeBytesAvailableToCaller = 0;
            long loTotalNumberOfBytes = 0;
            long loTotalNumberOfFreeBytes = 0;

            // This can take a long time on the X3, so put up a screen so user knows we're doing something
            if (glCheckingFlashDlg == null)
            {
                glCheckingFlashDlg = AppMessageBox.ShowCenteredMessageNotModal("Checking Available Flash", "Please Wait...", null);
            }
            else
            {
                glCheckingFlashDlg.Visible = true;
                glCheckingFlashDlg.Show();
                glCheckingFlashDlg.Refresh();
                IssueAppImp.ApplicationDoEvents();
            }

            if (GetDiskFreeSpaceEx("AC Flash", ref loFreeBytesAvailableToCaller,
                ref loTotalNumberOfBytes, ref loTotalNumberOfFreeBytes) == false)
            {
                glCheckingFlashDlg.Close();
                /*
                if (loMsgForm != null)
                    loMsgForm.Dispose();
                */
                return Marshal.GetLastWin32Error();
            }
            glCheckingFlashDlg.Close();
            /*
            if (loMsgForm != null)
                loMsgForm.Dispose();
            */

            // zero out diskdeleted if present
            if (oDiskDeleted > 0) 
                oDiskDeleted = 0;

            oDiskFree = loFreeBytesAvailableToCaller;
#endif
            // return free space
            return oDiskFree;
        }

        /// <summary>
        /// Checks to see if iMinimumFlashFree bytes of flash remain available. If not, queries
        /// user to perform a reclaim.  Returns to total flash free (after the Reclaim, if performed)
        /// </summary>
        public long QueryUserReclaimFlash(int iMinimumFlashFree)
        {
#if _original_
            // DEBUG -- should be moved to IssueApLogic?
            long loFlashFree = 0;
            int loFlashDeleted = 0;

#if !WindowsCE
            // There is no Flash in PatrolCar / Emulator, so for now we'll just make it pass the test
            loFlashFree = iMinimumFlashFree + 1;
#endif

            // get free & deleted
            for (; ; )
            {
                GetTotalDiskFree(ref loFlashFree, ref loFlashDeleted);
                // if enough free, just exit
                if (loFlashFree > iMinimumFlashFree) 
                    return loFlashFree;

                // On X3, GetTotalDiskFree won't return deleted bytes, so we will 
                // always try a reclaim when using the X3

                // they really don't have a choice, do they?
                AppMessageBox loMsgForm = AppMessageBox.ShowCenteredMessageNotModal("Reclaiming Flash", "Please Wait...", null);
                /*DelayOSTicks(30 * OSTICKS_mSEC_50);  // wait about 1.5 seconds... the reclaim message is also posted for each block*/

                ReclaimFlash2();

                // EmptyKey buffer so that keys don't stack up...
                /*EmptyMsgQueue();*/

                loMsgForm.Close();
                if (loMsgForm != null)
                    loMsgForm.Dispose();

                // If using an X3, we need to check free space again after reclaim
                GetTotalDiskFree(ref loFlashFree, ref loFlashDeleted);
                if ((loFlashDeleted + loFlashFree) < iMinimumFlashFree)
                    AppMessageBox.ShowMessageWithBell("Flash is Full!", "Unload handheld to free Flash.", "Error");
                // Always exit the loop on X3 platform. We only need to reclaim once
                return loFlashFree;
            }
            return loFlashFree;
#else
            return 0xffff;
#endif
        }

        private static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return ((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method);
        }

        public int ReclaimFlash2()
        {
            // DEBUG -- should be moved to IssueApLogic?
#if WindowsCE
            const uint GENERIC_READ = 0x80000000;
            const uint OPEN_EXISTING = 3;
            const uint FILE_ATTRIBUTE_NORMAL = 0;

            IntPtr loReclaimHandle = (IntPtr)CreateFileCE("\\AC Flash\\RECLAIM.RES", 
                (uint)(GENERIC_READ), (uint)0, 0, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, 0);
            int loBytesReturned = 0;
            uint IOCTL = CTL_CODE(32768, 2049, 0, 0); //IOCTL_RECLAIMFLASH
            DeviceIoControlCE(loReclaimHandle, IOCTL, null, 0,
              null, 0, ref loBytesReturned, IntPtr.Zero);
            CloseHandle(loReclaimHandle);
#endif
            return 0;
        }

        // "Standard" error codes defined in the Series3/4 OS
        public string TranslateFileErrorCode2(int iErrorCode)
        {
            // DEBUG -- should be moved to IssueApLogic?
            if (iErrorCode >= 0)
                return "No Error";

            switch (iErrorCode)
            {
                case FileErrors.FILE_ALREADY_EXISTS: return "File already exists";
                case FileErrors.FILE_IN_USE: return "File in use";
                case FileErrors.FILE_NOT_FOUND: return "File not found";
                case FileErrors.FILE_TOO_MANY_OPEN: return "Too many open files";
                case FileErrors.FILE_CORRUPTED: return "File Corrupted";
                case FileErrors.FILE_FAILED_FAT_UPDATE: return "Failed FAT update";
                case FileErrors.FILE_CROSSLINKED_FRAGS: return "Cross-linked fragments";
                case FileErrors.FILE_TOOMANY_FREE_BLOCKS: return "Too many free blocks";
                case FileErrors.FILE_DISKFULL: return "Disk full";
                case FileErrors.FILE_WRITE_FAILED: return "FLASH write failed";
                case FileErrors.FILE_SYSTEM_NOT_INITED: return "File system not initialized";
                case FileErrors.FILE_CANNOT_EXTEND_FILE: return "Cannot extend file";
                case FileErrors.FILE_INVALID_FILETYPE: return "Invalid File Type";
                case FileErrors.FILE_RUL_FULL: return "RUL Full";
                case FileErrors.FILE_RUL_INVALID_HANDLE: return "Invalid Handle";
                case FileErrors.FILE_READONLY: return "Read Only";
                case FileErrors.FILE_CORRUPT_CHUNK_HDR: return "Corrupt chunk header";
                case FileErrors.FILE_ILLEGAL_OPENMODE: return "Illegal open mode";
                default: return "Unknown error code";
            }
        }

        public int GetEditRecNo()
        {
            return fEditRecNo;
        }

        public void SetEditRecNo(int iEditRecNo)
        {
            fEditRecNo = iEditRecNo;
        }

        public bool GetFieldValueFromIssueSourceTable(DataRow iSourceRow, string iFieldName, ref string ioIssueSourceValue)
        {
            if (iSourceRow != null)
            {
                int loColumnIdx = iSourceRow.Table.Columns.IndexOf(iFieldName);
                if (loColumnIdx != -1)
                {
                    try
                    {
                        ioIssueSourceValue = iSourceRow[loColumnIdx].ToString();
                        return true;
                    }
                    catch (System.Exception exp)
                    {
                        ioIssueSourceValue = string.Empty;
                        Console.WriteLine("Error in GetFieldValueFromIssueSourceTable: {0}", exp.Source);
                    }
                }

            }

            return false;
        }


        public void BuildIssueReviewDetailLogic()
        {
            // Exit if we already have an logic object for IssueReviewDetail form
            if (IssueReviewDetailLogic != null)
                return;

            // there needs to be at least one ISSUE form in configuration. If none exist, then
            // don't bother creating review logic for it.
            TBaseIssForm loCfgForm1 = this.GetCfgIssueForm();
            TBaseIssForm loCfgForm2 = this.GetCfgIssueForm2();
            if ((loCfgForm1 == null) && ( loCfgForm2 == null ))
            {
                return;
            }

            // interrogate the forms to discover which action buttons are available
            AddToIssueStructActionButtonList(loCfgForm1);
            AddToIssueStructActionButtonList(loCfgForm2);

            IssueReviewDetailLogicFragmentTag = Helper.BuildIssueReviewFragmentTag(this.IssueStruct.Name);

            IssueReviewDetailLogic = new IssueReviewDetailFragment { Arguments = new Bundle() };
            IssueReviewDetailLogic.Arguments.PutString("structName", this.IssueStruct.Name);
        }


        public void BuildIssueForm1Logic()
        {

            // Exit if we already have an logic object for ISSUE form
            if (IssueForm1Logic != null)
                return;

            // Find ISSUE form in configuration. If it doesn't exist, then
            // don't bother creating logic for it.
            TBaseIssForm loCfgForm = this.GetCfgIssueForm();
            if (loCfgForm == null)
                return;


            // TODO - may want to do more difference handling here

            if (loCfgForm is TNotesForm)
            {
                if (this.IssueStruct is TCiteDetailStruct)
                {
                    TCiteDetailStruct loNotesStruct = (this.IssueStruct as TCiteDetailStruct);

#if _old_
                     // the fragment name needs to reference the parent
                    NotesFormLogicFragmentTag = Helper.BuildIssueNotesFragmentTag(loNotesStruct.ParentStruct);
                    //NotesFormLogicFragmentTag = Helper.BuildIssueNotesFragmentTag(this.IssueStruct.Name);


                    NotesFormLogic = new NotesFragment { Arguments = new Bundle() };
                    NotesFormLogic.Arguments.PutString("structName", loNotesStruct.ParentStruct);
                    //NotesFormLogic.Arguments.PutString("structName", this.IssueStruct.Name);
#endif

                    // the fragment name needs to reference the parent
                    NotesFormLogicFragmentTag = Helper.BuildNotesReviewSelectFragmentTag(loNotesStruct.ParentStruct);


                    NotesFormLogic = new NotesReviewSelectFragment { Arguments = new Bundle() };
                    NotesFormLogic.Arguments.PutString("structName", loNotesStruct.ParentStruct);
                    //NotesFormLogic.Arguments.PutString("structName", this.IssueStruct.Name);



#if _old_
                    NotesDetailFormLogicFragmentTag = Helper.BuildIssueNoteDetailFragmentTag(loNotesStruct.ParentStruct);
                    //NotesDetailFormLogicFragmentTag = Helper.BuildIssueNoteDetailFragmentTag(this.IssueStruct.Name);

                    NotesDetailFormLogic = new NoteDetailFragment { Arguments = new Bundle() };
                    NotesDetailFormLogic.Arguments.PutString("structName", loNotesStruct.ParentStruct);
                    //NotesDetailFormLogic.Arguments.PutString("structName", this.IssueStruct.Name);
#endif


                    NotesDetailFormLogicFragmentTag = Helper.BuildNotesReviewDetailFragmentTag(loNotesStruct.ParentStruct);

                    NotesDetailFormLogic = new NotesReviewDetailFragment { Arguments = new Bundle() };
                    NotesDetailFormLogic.Arguments.PutString("structName", loNotesStruct.ParentStruct);
                    //NotesDetailFormLogic.Arguments.PutString("structName", this.IssueStruct.Name);

                }




            }
            else
            {
                IssueForm1LogicFragmentTag = Helper.BuildIssueNewFragmentTag(this.IssueStruct.Name);


                // TODO - this is dupplicated when it is created from main menu

                IssueForm1Logic = new CommonFragment { Arguments = new Bundle() };
                IssueForm1Logic.Arguments.PutString("structName", this.IssueStruct.Name);
            }






#if _original_

            // Exit if we already have an logic object for ISSUE form
            if (IssueForm1Logic != null)
                return;

            // Find ISSUE form in configuration. If it doesn't exist, then
            // don't bother creating logic for it.
            TBaseIssForm loCfgForm = this.GetCfgIssueForm();
            if (loCfgForm == null)
                return;

            Reino.CommonLogic.IssFormBuilder loIssFormBuilder;
            // Create IssueForm logic object and assign it as our IssueForm1Logic
            // (Some structures have special requirements, so build proper descendant type)
            if (loCfgForm is TNotesForm)
                loIssFormBuilder = new NotesFormLogic();
            else
                loIssFormBuilder = new IssFormBuilder();
            IssueForm1Logic = loIssFormBuilder;

            // Set the logic style and set reference to the configuration's form object
            loIssFormBuilder.LogicStyle = FormLogicStyle.flsIssue;
            loIssFormBuilder.CfgForm = loCfgForm;
            loIssFormBuilder.CfgForm.StructLogicObj = this;

            // Create new Windows form and tie it to the logic object
            IssueForm loIssueForm = new IssueForm();
            loIssueForm.FormBuilder = loIssFormBuilder;

            // Dynamically build controls on form based on client's configuration
            loIssFormBuilder.CreateLayoutFromConfig(loIssueForm, ref TClientDef.GlobalClientDef,
                IssueStruct);
#endif
        }

        public void BuildIssueForm2Logic()
        {

            // Exit if we already have an logic object for ISSUE form
            if (IssueForm2Logic != null)
                return;

            // Find ISSUE form in configuration. If it doesn't exist, then
            // don't bother creating logic for it.
            TBaseIssForm loCfgForm = this.GetCfgIssueForm2();
            if (loCfgForm == null)
                return;


            IssueForm2LogicFragmentTag = Helper.BuildIssueNew2FragmentTag(this.IssueStruct.Name);

            IssueForm2Logic = new CommonFragment { Arguments = new Bundle() };
            IssueForm2Logic.Arguments.PutString("structName", this.IssueStruct.Name);

#if _original_
            // Exit if we already have an logic object for ISSUE form
            if (IssueForm2Logic != null)
                return;

            // Find ISSUE form in configuration. If it doesn't exist, then
            // don't bother creating logic for it.
            TBaseIssForm loCfgForm = this.GetCfgIssueForm2();
            if (loCfgForm == null)
                return;

            Reino.CommonLogic.IssFormBuilder loIssFormBuilder;
            // Create IssueForm logic object and assign it as our IssueForm1Logic
            // (Some structures have special requirements, so build proper descendant type)
            if (loCfgForm is TNotesForm)
                loIssFormBuilder = new NotesFormLogic();
            else
                loIssFormBuilder = new IssFormBuilder();
            IssueForm2Logic = loIssFormBuilder;

            // Set the logic style and set reference to the configuration's form object
            loIssFormBuilder.LogicStyle = FormLogicStyle.flsIssue;
            loIssFormBuilder.CfgForm = loCfgForm;
            loIssFormBuilder.CfgForm.StructLogicObj = this;

            // Create new Windows form and tie it to the logic object
            IssueForm loIssueForm = new IssueForm();
            loIssueForm.FormBuilder = loIssFormBuilder;

            // Dynamically build controls on form based on client's configuration
            loIssFormBuilder.CreateLayoutFromConfig(loIssueForm, ref TClientDef.GlobalClientDef,
                IssueStruct);
#endif
        }



        public List<string> IssueStructActionButtons = new List<string>();

        private void ExtractActionButtonsFromCfgTabSheet(System.Collections.Generic.IList<TSheet> Container)
        {

            foreach (Reino.ClientConfig.TSheet NextCtrl in Container)
            {
                // Is it a TTPanel or descendant? (It certainly should be since TSheet inherits from TTPanel)
                if (NextCtrl is Reino.ClientConfig.TTPanel)
                    ExtractActionButtonsFromCfgContainer((NextCtrl as Reino.ClientConfig.TTPanel).Controls); // recursive
            }
        }


        private void ExtractActionButtonsFromCfgContainer(System.Collections.Generic.IList<TWinClass> Container)
        {

            foreach (Reino.ClientConfig.TWinClass NextCtrl in Container)
            {
                // Is it a "TabSheet" container?
                if (NextCtrl is Reino.ClientConfig.TTTabSheet)
                    ExtractActionButtonsFromCfgTabSheet((NextCtrl as Reino.ClientConfig.TTTabSheet).Sheets); // Recursive

                // Is it a TTPanel or descendant? (TTTabSheet, TPanel, TSheet)
                if (NextCtrl is Reino.ClientConfig.TTPanel)
                    ExtractActionButtonsFromCfgContainer((NextCtrl as Reino.ClientConfig.TTPanel).Controls); // Recursive

                //// Is it a TTEdit or descendant? (TNumEdit, TIssEdit, TTMemo, TEditListBox)
                //if (NextCtrl is Reino.ClientConfig.TTEdit)
                //    BuildEditCtrlFromCfg(NextCtrl as Reino.ClientConfig.TTEdit); // Specific

                // Is it a TTButton?
                if (NextCtrl is Reino.ClientConfig.TTButton)
                {
                    string loActionButtonName = NextCtrl.Name.ToUpper();
                    if (IssueStructActionButtons.Contains(loActionButtonName) == false)
                    {
                        // winner
                        IssueStructActionButtons.Add(NextCtrl.Name.ToUpper());
                    }
                }


                //// Is it a TTBitmap?
                //if (NextCtrl is Reino.ClientConfig.TTBitmap)
                //    BuildBitmapFromCfg(NextCtrl as Reino.ClientConfig.TTBitmap); //Specific
            }
        }


        private void AddToIssueStructActionButtonList(TBaseIssForm iSelectForm)
        {

            // TODO  - maybe we'll keep seperate lists per form?

            if (iSelectForm != null)
            {
                // we will interrogate the select form to discover which action buttons are available
                ExtractActionButtonsFromCfgContainer(iSelectForm.Controls);
            }
        }





        public void BuildSelectFormLogic()
        {
            // Exit if we already have an logic object for SELECT form
            if (SelectFormLogic != null)
                return;

            // Find ISSUE form in configuration. If it doesn't exist, then
            // don't bother creating logic for it.
            TBaseIssForm loCfgForm = this.GetCfgSelectForm();
            if (loCfgForm == null)
                return;



            // we will interrogate the select form to discover which action buttons are available
            AddToIssueStructActionButtonList(loCfgForm);



            SelectFormLogicFragmentTag = Helper.BuildIssueSelectFragmentTag(this.IssueStruct.Name);

            SelectFormLogic = new IssueSelectFragment{ Arguments = new Bundle() };
            SelectFormLogic.Arguments.PutString("structName", this.IssueStruct.Name);


#if _original_
            // Exit if we already have an logic object for SELECT form
            if (SelectFormLogic != null)
                return;

            // Find SELECT form in configuration. If it doesn't exist, then
            // don't bother creating logic for it.
            TBaseIssForm loCfgForm = this.GetCfgSelectForm();
            if (loCfgForm == null)
                return;

            Reino.CommonLogic.IssFormBuilder loIssFormBuilder;
            // Create IssueForm logic object and assign it as our SelectFormLogic
            // (Some structures have special requirements, so build proper descendant type)
            if (loCfgForm is TIssueSelectForm)
                loIssFormBuilder = new IssueSelectFormLogic();
            else
                loIssFormBuilder = new IssFormBuilder();
            SelectFormLogic = loIssFormBuilder;

            // Set the logic style and set reference to the configuration's form object
            loIssFormBuilder.LogicStyle = FormLogicStyle.flsSelect;
            loIssFormBuilder.CfgForm = loCfgForm;
            loIssFormBuilder.CfgForm.StructLogicObj = this;

            // Create new Windows form and tie it to the logic object
            IssueForm loIssueForm = new IssueForm();
            loIssueForm.FormBuilder = loIssFormBuilder;

            // Dynamically build controls on form based on client's configuration
            loIssFormBuilder.CreateLayoutFromConfig(loIssueForm, ref TClientDef.GlobalClientDef,
                IssueStruct);
#endif
        }

        public Reino.ClientConfig.TBaseIssForm GetCfgIssueForm()
        {
            TBaseIssForm loResult = null;
            TObjBasePredicate predicate = null;

            // Find the first form in the structure with the desired name
            predicate = new TObjBasePredicate("ISSUE");
            loResult = this.IssueStruct.Forms.Find(predicate.CompareByName_CaseInsensitive) as TBaseIssForm;
            if (loResult != null)
                return loResult;

            // If we didn't find it yet, lets try again with a different name because the
            // layout tool converter switched names on us
            foreach (TTForm NextForm in this.IssueStruct.Forms)
            {
                if (NextForm.Name.StartsWith("ISSUE_", StringComparison.CurrentCultureIgnoreCase))
                    return NextForm as TBaseIssForm;
            }
            return null;
        }

        public Reino.ClientConfig.TBaseIssForm GetCfgIssueForm2()
        {
            TBaseIssForm loResult = null;
            TObjBasePredicate predicate = null;

            // Find the first form in the structure with the desired name
            predicate = new TObjBasePredicate("ISSUE2");
            loResult = this.IssueStruct.Forms.Find(predicate.CompareByName_CaseInsensitive) as TBaseIssForm;
            if (loResult != null)
                return loResult;

            // If we didn't find it yet, lets try again with a different name because the
            // layout tool converter switched names on us
            foreach (TTForm NextForm in this.IssueStruct.Forms)
            {
                if (NextForm.Name.StartsWith("ISSUE2_", StringComparison.CurrentCultureIgnoreCase))
                    return NextForm as TBaseIssForm;
            }
            return null;
        }

        public Reino.ClientConfig.TBaseIssForm GetCfgSelectForm()
        {
            TBaseIssForm loResult = null;
            TObjBasePredicate predicate = null;

            // Find the first form in the structure with the desired name
            predicate = new TObjBasePredicate("SELECT");
            loResult = this.IssueStruct.Forms.Find(predicate.CompareByName_CaseInsensitive) as TBaseIssForm;
            if (loResult != null)
                return loResult;

            // If we didn't find it yet, lets try again with a different name because the
            // layout tool converter switched names on us
            foreach (TTForm NextForm in this.IssueStruct.Forms)
            {
                if (NextForm.Name.StartsWith("SELECT_", StringComparison.CurrentCultureIgnoreCase))
                    return NextForm as TBaseIssForm;
            }
            return null;
        }

        public Reino.ClientConfig.TBaseIssForm GetCfgFormByPartialName(string SearchName)
        {
            // First, look for exact name match
            for (int loIdx = 0; loIdx < this.IssueStruct.Forms.Count; loIdx++)
            {
                // Do the names match? (Case-Insensitive)
                if ((string.Compare(this.IssueStruct.Forms[loIdx].Name, SearchName, true) == 0) &&
                    (this.IssueStruct.Forms[loIdx] is TBaseIssForm))
                    return this.IssueStruct.Forms[loIdx] as TBaseIssForm;
            }

            // Didn't find yet, so lets try finding something that is prefixed with desired name
            for (int loIdx = 0; loIdx < this.IssueStruct.Forms.Count; loIdx++)
            {
                string CompareName = this.IssueStruct.Forms[loIdx].Name;
                if (CompareName.Length > SearchName.Length)
                {
                    CompareName = CompareName.Substring(0, SearchName.Length);
                    if ((string.Compare(CompareName, SearchName, true) == 0) &&
                        (this.IssueStruct.Forms[loIdx] is TBaseIssForm))
                        return this.IssueStruct.Forms[loIdx] as TBaseIssForm;
                }
            }
            return null;
        }

        internal Reino.ClientConfig.TIssForm GetIssueForm(TIssStruct iIssueStruct)
        {
            // Find the first TIssForm in the structure's form list
            TObjBasePredicate predicate = new TObjBasePredicate(typeof(Reino.ClientConfig.TIssForm));
            return iIssueStruct.Forms.Find(predicate.CompareByClassType) as TIssForm;
        }

        /// <summary>
        /// Returns the first form whose print picture name matches iPictureName. 
        /// If iPictureName is blank, returns the first form named ISSUE.
        /// </summary>
        protected TIssForm FormByPictureName(string iPictureName)
        {
            for (int loNdx = 0; loNdx < this.IssueStruct.Forms.Count; loNdx++)
            {
                if (iPictureName == "")
                { // look for ISSUE form
                    if (IssueStruct.Forms[loNdx].Name.ToUpper().StartsWith("ISSUE"))
                        return IssueStruct.Forms[loNdx] as TIssForm;
                    continue;
                }

                // Get next object. Ignore it if its not a TIssForm 
                if (!(IssueStruct.Forms[loNdx] is TIssForm))
                    continue;

                // names must match
                if (string.Compare(((TIssForm)(IssueStruct.Forms[loNdx])).PrintPictureList[0].HighFormRevision.Name, iPictureName, true) == 0)
                    return IssueStruct.Forms[loNdx] as TIssForm;
            }
            return null; // found nothing
        }

            

        public TIssStruct GetIssueStructByName(string StructName)
        {
            foreach (TIssStruct NextStruct in DroidContext.XmlCfg.clientDef.IssStructMgr.IssStructs)
            //foreach (TIssStruct NextStruct in TClientDef.GlobalClientDef.IssStructMgr.IssStructs)
            {
                if (string.Compare(NextStruct.Name, StructName, true) == 0)
                    return NextStruct;
            }
            return null;
        }

        /// <summary>
        /// Routine called by a ticket ISSUE form when data elements on form need to default to
        /// values from this structure.  More specifically, this routine is called during
        /// citation issue form initialization when a ticket is being issued as a result of a mark
        /// mode match.
        /// Our job will be to initialize various fields w/ values mark mode.  Our intialization will
        /// be as follows:
        /// For each field in the form, the routine InitFormDataField will be called.  The default behavior of
        /// this routine will be to 
        /// 1. Find its occurrence in the structure.
        /// 2. If it is found, set its contents to the value in the structure.
        /// 3. Notify dependents if data changed & w/ parent FieldExit
        /// 4. Disable the field.
        /// 5. Set EnabledLocked so nothing can enable it.
        /// </summary>
        public int InitFormData(Android.App.Fragment iFormBuilder)
        {
#if _original_
            TTableFldDef loStructFld;
            short loStructFldNdx;

            if (fCopyFromTable == null)
            {
                AppMessageBox.ShowMessageWithBell("InitFormData Error:", "fCopyFromData is NULL", "");
                return -1;
            }

            // Loop through all the fields in the structure, copying values to iss form as necessary 
            for (loStructFldNdx = 0; loStructFldNdx < fCopyFromTable.fTableDef.GetFieldCnt(); loStructFldNdx++)
            {
                loStructFld = fCopyFromTable.fTableDef.GetField(loStructFldNdx);
                InitFormDataField(iFormBuilder, loStructFld, loStructFld.Name, "");
            }

            // Allow descendant to do any extra fields
            InitExtraFormDataFields(iFormBuilder);
#endif
            return 0;
        }

        public virtual int InitExtraFormDataFields(Android.App.Fragment iFormBuilder)
        {
            return 0;
        }

        public virtual bool InitExtraFormDataFieldsAndroid(string iFieldName, ref string ioIssueSourceValue)
        {
            return false;
        }

        public virtual void InitSourceFormattingInfo(DataRow iSourceDataRow)
        {
            return;
        }


        public virtual bool InitFormDataField(string iFieldName, ref string ioIssueSourceValue)
        {
            if (fSearchAndIssueResult != null)
            {
                return GetFieldValueFromIssueSourceTable(fSearchAndIssueResult.SearchResultSelectedRow, iFieldName, ref ioIssueSourceValue);
            }
            else
            {
                return false;
            }

#if _original_
            // Find the field associated with the passed fieldname 
            TTControl loCfgCtrl = iFormBuilder.FindCfgControlByName(iToFormFieldName);
            // We can't do anything with it unless its a TTEdit
            if ((loCfgCtrl == null) || (!(loCfgCtrl is Reino.ClientConfig.TTEdit)))
                return 0;
            // Make local references to the configuration object and the associated Windows edit control
            TTEdit loEdit = loCfgCtrl as TTEdit;

            // make a copy of the field value before we stuff it
            string loSavedData = loEdit.Behavior.EditBuffer;

            // copy the field values over 
            if (iToFormFieldValue != "")
                loEdit.Behavior.SetEditBufferAndPaint(iToFormFieldValue);
            else
                loEdit.Behavior.SetEditBufferAndPaint(fCopyFromTable.GetFormattedFieldData(iStructFld.Name,
                    loEdit.Behavior.GetEditMask()));
            // and disable it...
            //mcb 6/12/03: some fields are optional in the initial form, but might be populated later in the secondary form.
            //  For example, in Markmode, both plate and VIN can be entered.   However, the operator might only enter Plate
            //  in the mark mode form, but want to enter plate and VIN in the issuance form. If we disable all fields that
            //  were entered in MarkMode, they will not be able to enter a VIN if it wasn't populated in MarkMode. To solve this
            //  delimna, certain fields will be marked as "supplemental".  If these fields are blank, they will not be protected.
            //  This technique can't be applied to all fields. In certain situations, a blank field needs to remain blank.
            //  Specifically, the location fields.  We don't want any variation in location between the mark and the ticket, so the
            //  absense of a 2nd cross street in the mark doesn't mean the operator can add it in the ticket. 
            if ((loEdit.Behavior.EditBuffer != "") ||
                !loEdit.DontDisableWhenCopiedFromForm)
                loEdit.IsEnabled = false;
            // and suppress the "FirstFocus" event.. 
            loEdit.Behavior.SetEditStatePreInitialized(true);

            if ((loEdit.Behavior.EditBuffer != "") || (!loEdit.DontDisableWhenCopiedFromForm))
                loEdit.SetEnabledLocked(true);

            int loNotifyEvent = EditRestrictionConsts.dneParentFieldExit;
            // did the data change as a result the stuff?

            if (string.Compare(loSavedData, loEdit.Behavior.EditBuffer) != 0)
            {
                // note that the data itself has changed
                loEdit.Behavior.ProcessRestrictions(EditRestrictionConsts.dneDataChanged, null);
                loNotifyEvent |= EditRestrictionConsts.dneParentDataChanged;
            }

            // let the field do whatever initialization it needs to
            loEdit.Behavior.NotifyDependents(loNotifyEvent);

            // and suppress the "FirstFocus" event..
            loEdit.Behavior.SetEditStatePreInitialized(true);
#endif
            return false;
        }

        public virtual int InitFormDataField(Android.App.Fragment iFormBuilder, TTableFldDef iStructFld, string iToFormFieldName, string iToFormFieldValue)
        {
#if _original_
            // Find the field associated with the passed fieldname 
            TTControl loCfgCtrl = iFormBuilder.FindCfgControlByName(iToFormFieldName);
            // We can't do anything with it unless its a TTEdit
            if ((loCfgCtrl == null) || (!(loCfgCtrl is Reino.ClientConfig.TTEdit)))
                return 0;
            // Make local references to the configuration object and the associated Windows edit control
            TTEdit loEdit = loCfgCtrl as TTEdit;

            // make a copy of the field value before we stuff it
            string loSavedData = loEdit.Behavior.EditBuffer;

            // copy the field values over 
            if (iToFormFieldValue != "")
                loEdit.Behavior.SetEditBufferAndPaint(iToFormFieldValue);
            else
                loEdit.Behavior.SetEditBufferAndPaint(fCopyFromTable.GetFormattedFieldData(iStructFld.Name,
                    loEdit.Behavior.GetEditMask()));
            // and disable it...
            //mcb 6/12/03: some fields are optional in the initial form, but might be populated later in the secondary form.
            //  For example, in Markmode, both plate and VIN can be entered.   However, the operator might only enter Plate
            //  in the mark mode form, but want to enter plate and VIN in the issuance form. If we disable all fields that
            //  were entered in MarkMode, they will not be able to enter a VIN if it wasn't populated in MarkMode. To solve this
            //  delimna, certain fields will be marked as "supplemental".  If these fields are blank, they will not be protected.
            //  This technique can't be applied to all fields. In certain situations, a blank field needs to remain blank.
            //  Specifically, the location fields.  We don't want any variation in location between the mark and the ticket, so the
            //  absense of a 2nd cross street in the mark doesn't mean the operator can add it in the ticket. 
            if ((loEdit.Behavior.EditBuffer != "") ||
                !loEdit.DontDisableWhenCopiedFromForm)
                loEdit.IsEnabled = false;
            // and suppress the "FirstFocus" event.. 
            loEdit.Behavior.SetEditStatePreInitialized(true);

            if ((loEdit.Behavior.EditBuffer != "") || (!loEdit.DontDisableWhenCopiedFromForm))
                loEdit.SetEnabledLocked(true);

            int loNotifyEvent = EditRestrictionConsts.dneParentFieldExit;
            // did the data change as a result the stuff?

            if (string.Compare(loSavedData, loEdit.Behavior.EditBuffer) != 0)
            {
                // note that the data itself has changed
                loEdit.Behavior.ProcessRestrictions(EditRestrictionConsts.dneDataChanged, null);
                loNotifyEvent |= EditRestrictionConsts.dneParentDataChanged;
            }

            // let the field do whatever initialization it needs to
            loEdit.Behavior.NotifyDependents(loNotifyEvent);

            // and suppress the "FirstFocus" event..
            loEdit.Behavior.SetEditStatePreInitialized(true);
#endif
            return 1;
        }

        internal void SetIssueSource(Android.App.Fragment iCommonFragment,  SearchParameterPacket iSearchResult  )
        {
            if (iCommonFragment != null)
            {
                if (iCommonFragment is CommonFragment)
                {
                    ((CommonFragment)iCommonFragment).fSourceDataStruct = this.IssueStruct;
                    ((CommonFragment)iCommonFragment).fSourceIssueStructLogic = this;
                }
            }

            // this is set when the results are about to be displayed fSearchAndIssueResult = iSearchResult;
        }

        internal void UndoIssueSource(Android.App.Fragment iCommonFragment)
        {
            CommonFragment iFormBuilder = null;
            if (iCommonFragment != null)
            {
                if (iCommonFragment is CommonFragment)
                {
                    iFormBuilder = iCommonFragment as CommonFragment;
                }
            }

            if (iFormBuilder == null)
            {
                return;
            }


#if _original_
            // Undo all the "Enabled" locks set by InitFormData
            TTableFldDef loStructFld;
            short loStructFldNdx;

            // loop through all the fields in the structure, unlocking the protected property as necessary 
            for (loStructFldNdx = 0; loStructFldNdx < fCopyFromTable.fTableDef.GetFieldCnt(); loStructFldNdx++)
            {
                loStructFld = fCopyFromTable.fTableDef.GetField(loStructFldNdx);

                // Does this field exist in the issue form? 
                // Find the field associated with the passed fieldname 
                TTControl loCfgCtrl = iFormBuilder.FindCfgControlByName(loStructFld.Name);
                // We can't do anything with it unless its a TTEdit
                if ((loCfgCtrl == null) || (!(loCfgCtrl is Reino.ClientConfig.TTEdit)))
                    continue;
                // Make local references to the configuration object and the associated Windows edit control
                TTEdit loEdit = loCfgCtrl as TTEdit;
                loEdit.SetEnabledLocked(false);
            }
#endif

            UndoIssueSourceExtraFields(iFormBuilder);

            // Remove ourselves from CiteIssForm so that future issuances are not tied to us. 
            iFormBuilder.fSourceDataStruct = null;
            iFormBuilder.fSourceIssueStructLogic = null;
            iFormBuilder.fSourceDataRawRow = null;
            iFormBuilder.fSourceDataRowFormEditMode = Reino.ClientConfig.EditRestrictionConsts.femNewEntry;
            fSearchAndIssueResult = null;
        }


        internal virtual int UndoIssueSourceExtraFields(Android.App.Fragment iFormBuilder)
        {
            return 0;
        }

        protected short WriteFieldValuesToForm(TTForm iForm)
        {
#if _original_
            // On something big like Traffic, this might actually take awhile,
            // so lets do a "busy" cursor so the user gets a visual that something is happening
            // instead of letting them wonder.
            Cursor.Current = Cursors.WaitCursor;
            System.Threading.Thread.Sleep(0);

            TTControl loEdit;
            TTableFldDef loField;
            short loNdx;
            TTTable DataTable = this.IssueStruct.MainTable.HighTableRevision.Tables[0];
            short loSubNdx = 0;

            for (loNdx = 0; loNdx < this.IssueStruct.MainTable.GetFieldCnt(); loNdx++)
            {
                loField = this.IssueStruct.MainTable.GetField(loNdx);

                // Lets try to speed this up by first looking at control at current subindex.
                // If its a match, we can use it right-away. If its not the same field,
                // then we'll do a predicate search.
                loEdit = null;
                try
                {
                    if ((loSubNdx < this.IssueFormLogic.EntryOrder.Count) &&
                        (this.IssueFormLogic.EntryOrder[loSubNdx].Name.Equals(loField.Name)))
                    {
                        // Retain this control and bump up our subindex for next iteration
                        loEdit = this.IssueFormLogic.EntryOrder[loSubNdx];
                        loSubNdx++;
                    }
                }
                catch
                {
                    // Don't care if it failed for any reason, because we'll just do a predicate search instead
                }
                // If loEdit is still null, then we need to do predicate search
                if (loEdit == null)
                {
                    // Find control with predicate search
                    loEdit = this.IssueFormLogic.FindCfgControlByName(loField.Name);
                    // If we found a control, then lets get its index + 1 to
                    // help speed up the search on the next iteration
                    if (loEdit != null)
                    {
                        int loCtrlIdx = this.IssueFormLogic.GetCfgCtrlIndex(loEdit);
                        if (loCtrlIdx != -1)
                            loSubNdx = (short)(loCtrlIdx + 1);
                    }
                }
                if ((loEdit != null) && (loEdit.TypeOfWinCtrl() == typeof(ReinoTextBox)))
                {
                    // Set the text without processing edit restrictions
                    loEdit.Behavior.SetEditBufferAndPaint(DataTable.GetFormattedFieldData(loNdx,
                        loEdit.Behavior.GetEditMask()), false);
                }
            }

            // Reset back to normal cursor
            Cursor.Current = Cursors.Default;
            System.Threading.Thread.Sleep(0);
#endif
            return 0;
        }

        private int GetChildRecordKey(int iMasterRecordKey)
        {
#if _original_
            // Get the child structure by name
            TIssStruct ChildStruct = this.GetIssueStructByName(this.IssueStruct.IssueStruct);
            // we only work w/ TCiteDetailStructs
            if ((ChildStruct == null) || (!(ChildStruct is TCiteDetailStruct)))
                return -1;
            return ((DetailStructLogic)(ChildStruct.StructLogicObj)).FindRecordForMasterKey(iMasterRecordKey);
#else
            return 0;
#endif
        }

        internal int ViewRecord(int iRecordNumber)
        {
#if _original_
            TTForm loForm;
            string loFormCaption = "";
            if (iRecordNumber < 0)
            {
                AppMessageBox.ShowMessageWithBell("No record selected.", "", "");
                return 0;
            }

            TTTable DataTable = this.IssueStruct.MainTable.HighTableRevision.Tables[0];
            DataTable.ReadRecord(iRecordNumber);

            // if the form name is captured in the data, get it so we can pull that form
            loForm = FormByPictureName(DataTable.GetFormattedFieldData(FieldNames.FORMNAMEFieldName, ""));

            if (loForm == null)
            {
                AppMessageBox.ShowMessageWithBell("No ISSUE form!", "", "");
                return 0;
            }

            SetEditRecNo(iRecordNumber);
            WriteFieldValuesToForm(loForm);

            loFormCaption = "View " + this.IssueStruct.Name + " Record";
            this.IssueFormLogic.SetCaption(loFormCaption);

            this.IssueFormLogic.FormEdit(EditRestrictionConsts.femView, "", null);
#endif
            return 1;
        }

        private TIssMenuItem GetMenuItemByName( string iName ) 
        { 
            TObjBasePredicate predicate = new TObjBasePredicate(iName);
            return this.IssueStruct.IssMenuItems.Find(predicate.CompareByName_CaseInsensitive);
        }

        internal int IssueRecord(int iFormEditMode, string iInitialFocusControl, string iIssueFormName, string iTargetFragmentTag)
        {


            TBaseIssForm loForm;
            FragmentRegistation loTargetFragment;





            //// Get the form w/ name iIssueFormName
            //if (iIssueFormName == "")
            //    loTargetFragment = DroidContext.mainActivity.FindFragmentRegistration(iIssueFormName);
            //else
            //    loForm = GetCfgFormByPartialName(iIssueFormName);

            //if (loForm == null)
            //{
            //    AppMessageBox.ShowMessageWithBell("Missing form named " + iIssueFormName, "", "");
            //    return -1;
            //}


            loTargetFragment = DroidContext.mainActivity.FindFragmentRegistration(iTargetFragmentTag);

            if (loTargetFragment == null)
            {
                //AppMessageBox.ShowMessageWithBell("Missing form named " + iIssueFormName, "", "");
                return -1;
            }




            //// Make sure we have enough flash
            //if (QueryUserReclaimFlash(50000) < 50000)
            //    return -1;

            // IssueStructLogicAndroid loStructLogic = loForm.StructLogicObj as IssueStructLogicAndroid;

            //// Put an appropriate caption on the form.  Use the menu item text if available
            //TIssMenuItem loMenuItem = GetMenuItemByName(iIssueFormName);
            ///*
            //if (loMenuItem != null)
            //    loStructLogic.IssueFormLogic.SetCaption(loMenuItem.MenuText);
            //else
            //*/
            //{ // no menu item available.  Use generic "Enter <struct name> Record"
            //    string loFormCaption = "Enter " + this.IssueStruct.Name + " Record";
            //    loStructLogic.IssueFormLogic.SetCaption(loFormCaption);
            //}

            //loStructLogic.SetEditRecNo(this.IssueStruct.MainTable.HighTableRevision.Tables[0].GetRecCount());
            //return loStructLogic.IssueFormLogic.FormEdit(iFormEditMode, iInitialFocusControl, null);


            DroidContext.mainActivity.ShowFragmentForIssueanceFromAnotherIssueForm(loTargetFragment);


            return 0;


#if _original_
            TBaseIssForm loForm;

            // Get the form w/ name iIssueFormName
            if (iIssueFormName == "")
                loForm = this.GetCfgIssueForm();
            else
                loForm = GetCfgFormByPartialName(iIssueFormName);

            if (loForm == null)
            {
                AppMessageBox.ShowMessageWithBell("Missing form named " + iIssueFormName, "", "");
                return -1;
            }

            // Make sure we have enough flash
            if (QueryUserReclaimFlash(50000) < 50000)
                return -1;

            IssueStructLogicAndroid loStructLogic = loForm.StructLogicObj as IssueStructLogicAndroid;

            // Put an appropriate caption on the form.  Use the menu item text if available
            TIssMenuItem loMenuItem = GetMenuItemByName(iIssueFormName);
            /*
            if (loMenuItem != null)
                loStructLogic.IssueFormLogic.SetCaption(loMenuItem.MenuText);
            else
            */
            { // no menu item available.  Use generic "Enter <struct name> Record"
                string loFormCaption = "Enter " + this.IssueStruct.Name + " Record";
                loStructLogic.IssueFormLogic.SetCaption(loFormCaption);
            }

            loStructLogic.SetEditRecNo(this.IssueStruct.MainTable.HighTableRevision.Tables[0].GetRecCount());
            return loStructLogic.IssueFormLogic.FormEdit(iFormEditMode, iInitialFocusControl, null);
#else
              return 0;
#endif
        }

        internal int IssueChildRecord(short iFormEditMode, string iInitialFocusControl, int iMasterRecordNo)
        {
#if _original_
            TIssForm loCiteIssForm;

            // Get the child structure by name
            TIssStruct ChildStruct = this.GetIssueStructByName(this.IssueStruct.IssueStruct);
            if (ChildStruct == null)
            {
                AppMessageBox.ShowMessageWithBell("No Issue Struct", "", "");
                return 0;
            }

            if ((loCiteIssForm = GetIssueForm(ChildStruct)) == null)
            {
                AppMessageBox.ShowMessageWithBell("Issue Struct", "Has no Issue Form", "");
                return 0;
            }

            // Hard-coded maximum of 1 child record per parent...
            if (GetChildRecordKey(iMasterRecordNo) >= 0)
                return 0;

            // Issue the ChildSturct
            SetIssueSource(((IssueStructLogicAndroid)(ChildStruct.StructLogicObj)).IssueFormLogic, this.IssueStruct.MainTable.HighTableRevision.Tables[0]);
            if (loCiteIssForm is TBaseDetailForm)
                ((IssueStructLogicAndroid)(ChildStruct.StructLogicObj)).IssueFormLogic.SetMasterKey(iMasterRecordNo);

            ((IssueStructLogicAndroid)(ChildStruct.StructLogicObj)).IssueRecord(EditRestrictionConsts.femSingleEntry, iInitialFocusControl, "");

            UndoIssueSource(((IssueStructLogicAndroid)(ChildStruct.StructLogicObj)).IssueFormLogic);
#endif
            return 0;
        }

        static protected int ReadPersistentMemoryLocationFromRegistry(ref string iRegValue)
        {
#if _original_
            // Series3CE always stores flash in "AC Flash"
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
                iRegValue = "\\AC Flash\\";
            else
                iRegValue = ReinoTablesConst.cnstClientConfigFolder;
#endif
            return 0;
        }

#if _original_
        static internal void GetMultimediaFolder(ref string ioMultimediaFolder, TTMultimedia iMultimediaType, bool iAutoISSUEFileStore)
        {
            // Get the location of the flash mem folder
            string loFlashFileLocation = "";
            ReadPersistentMemoryLocationFromRegistry(ref loFlashFileLocation);

            // Select the path
            switch (iMultimediaType)
            {
                case TTMultimedia.mmAudio:
                    {
                        // Do we want the AutoISSUE file store, or the original path?
                        if (iAutoISSUEFileStore)
                            ioMultimediaFolder = loFlashFileLocation;
                        else
                            ioMultimediaFolder = CiteStructLogic.CN_AUDIO_RECORD_PATH + "\\";
                        break;
                    }
                case TTMultimedia.mmPicture:
                    {
                        // Do we want the AutoISSUE file store, or the original path?
                        if (iAutoISSUEFileStore)
                            ioMultimediaFolder = loFlashFileLocation;
                        else
                            ioMultimediaFolder = CiteStructLogic.CN_PICTURE_RECORD_PATH + "\\";
                        break;
                    }
                case TTMultimedia.mmVideo:
                    {
                        // Do we want the AutoISSUE file store, or the original path?
                        if (iAutoISSUEFileStore)
                            ioMultimediaFolder = loFlashFileLocation;
                        else
                            ioMultimediaFolder = CiteStructLogic.CN_MOVIE_RECORD_PATH + "\\";
                        break;
                    }
            }
        }
#endif

        public virtual void EvaluateWirelessSearchResult()
        {

            SearchParameterPacket loSearchResult = null;

            /* mcb 7/27/04
               Wireless and local searches are performed in the same thread. It is possible for a wireless
                        search result to occur while a local or another wireless search is being processed.  It is
                        not possible for a local search result to return while a wireless search result is being
                        processed.
                        Since search results are processed in the same thread, we cannot simply stall here while
                        waiting for a previous search result to play-out because that other search result will not
                        ever execute (its the same thread). So, our sol'n is as follows:
                        1) check/set a mutex before executing anything else. 
                           If the mutex is already set, it means either a local or wireless (or both) search is
                                    being processed. If so we will exit.
                        2) Once the mutex is acquired, we will loop until all wireless search results have
                           been processed.
                        3) Upon completion, the local search result will call this routine so that any wireless
                           results that have arrived in the interim are processed.
                                    */
            if (SearchStructLogicAndroid.unSearchEvaluateInProcess)
            {
                return; // can't acquire mutex
            }
            SearchStructLogicAndroid.unSearchEvaluateInProcess = true;

            for (; ; )
            {
                // free the ClientCommandRec from the previous time through
                if (loSearchResult != null)
                {
                    loSearchResult = null;
                }

                // CE_APDI_GetReplyFromReceiveQue will find the associated command packet in the send queue.
                // it will also remove the item from both queues.  Our only responsibility is to deallocate
                // any additional memory, which to my knowledge is limited to loAPDIClientCommandRec
                if (!SearchStructWirelessQueueAndroid.CE_APDI_GetReplyFromReceiveQue(ref loSearchResult))
                {
                    // couldn't get anything from the que... It's empty
                    SearchStructLogicAndroid.unSearchEvaluateInProcess = false;
                    return;
                }

                // didn't find a matching message? 
                if (loSearchResult == null)
                {
                    // the original inquiry was discarded when they left the form
                    // they don't want to see it anymore
                    continue;
                }

                // if the user has left the form before the reponse came back
                // and doesn't want to know about this result,then we're outta here
                if (loSearchResult.fCancelledByUser)
                {
                    // its been handled, delete the last piece of dynamic memory
                    continue; // any more?
                }


                // TODO - this queue should be genericized for any type


                // let each one hand its own result
                if (loSearchResult.fSearchStruct != null)
                {
                    if (loSearchResult.fSearchStruct.StructLogicObj is SearchStructLogicAndroid)
                    {
                        //(loAPDIClientCommandRec.fSearchStruct.StructLogicObj as SearchStructLogicAndroid).HandleWirelessSearchResult(
                        //    loAPDIClientCommandRec.fSearchMatchDTOList,
                        //    loAPDIClientCommandRec.fMinMatchCount,
                        //    loAPDIClientCommandRec.fCalledFromSearchAndIssue,
                        //    loAPDIClientCommandRec.fInitiatingEditRestriction);

                        (loSearchResult.fSearchStruct.StructLogicObj as SearchStructLogicAndroid).HandleWirelessSearchResult( loSearchResult );

                    }
                }


                // tell the corresponding search struct to handle it!
            } // for loop; loops until receive queue is empty


#if _original_
            TAPDIClientCommandRec loAPDIClientCommandRec = null;
            string loResponseMessageIDStr; //[ MAX_PATH ];
            string loResponseColumnHeadersStr; //[ MAX_APDI_PAYLOAD_CHARS ];
            string loResponseColumnValuesStr; //[ MAX_APDI_PAYLOAD_CHARS ];

            bool loValidates;

            /* mcb 7/27/04
               Wireless and local searches are performed in the same thread. It is possible for a wireless
                        search result to occur while a local or another wireless search is being processed.  It is
                        not possible for a local search result to return while a wireless search result is being
                        processed.
                        Since search results are processed in the same thread, we cannot simply stall here while
                        waiting for a previous search result to play-out because that other search result will not
                        ever execute (its the same thread). So, our sol'n is as follows:
                        1) check/set a mutex before executing anything else. 
                           If the mutex is already set, it means either a local or wireless (or both) search is
                                    being processed. If so we will exit.
                        2) Once the mutex is acquired, we will loop until all wireless search results have
                           been processed.
                        3) Upon completion, the local search result will call this routine so that any wireless
                           results that have arrived in the interim are processed.
                                    */
            if (SearchStructLogic.unSearchEvaluateInProcess)
            {
                return; // can't acquire mutex
            }
            SearchStructLogic.unSearchEvaluateInProcess = true;

            for (; ; )
            {
                // free the ClientCommandRec from the previous time through
                if (loAPDIClientCommandRec != null)
                {
                    loAPDIClientCommandRec = null;
                }
                // mcb 7/23/04:
                // CE_APDI_GetReplyFromReceiveQue will find the associated command packet in the send queue.
                // it will also remove the item from both queues.  Our only responsibility is to deallocate
                // any additional memory, which to my knowledge is limited to loAPDIClientCommandRec
                if (!IssueAppImp.glWirelessQueue.CE_APDI_GetReplyFromReceiveQue(ref loAPDIClientCommandRec))
                {
                    // couldn't get anything from the que... It's empty
                    SearchStructLogic.unSearchEvaluateInProcess = false;
                    return;
                }

                // didn't find a matching message? 
                if (loAPDIClientCommandRec == null)
                {
                    // the original inquiry was discarded when they left the form
                    // they don't want to see it anymore
                    continue;
                }

                // if the user has left the form before the reponse came back
                // and doesn't want to know about this result,then we're outta here
                if (loAPDIClientCommandRec.fCancelledByUser)
                {
                    // its been handled, delete the last piece of dynamic memory
                    continue; // any more?
                }

                // mcb 4/15/2006: Now that these are objects, we use polynomialism to let each one hand
                // its own result.
                loAPDIClientCommandRec.ProcessResponse();

                // tell the corresponding search struct to handle it!
            } // for loop; loops until receive queue is empty
#endif
        }

        private void BuildOneFieldKeyString(string iFieldName, ref string oOutString)
        {
#if _original_
            string loTmpFldValue = "";

            int loFldNdx;
            if ((loFldNdx = this.IssueStruct.MainTable.GetFldNo(iFieldName)) < 0)
                return; // adds nothing.

            loTmpFldValue = this.IssueStruct.MainTable.HighTableRevision.Tables[0].GetFormattedFieldData(loFldNdx, "");
            oOutString = oOutString + iFieldName + "=" + loTmpFldValue + "\t";
#endif
        }

        public void BuildKeyString(ref string oKeyString)
        {
            oKeyString = "";

#if _original_

            // start off with issue date and time
            BuildOneFieldKeyString(FieldNames.IssueDateFieldName, ref oKeyString);
            BuildOneFieldKeyString(FieldNames.IssueTimeFieldName, ref oKeyString);

            // throw in IssueNo
            BuildOneFieldKeyString(FieldNames.IssueNoFieldName, ref oKeyString);

            // Officer ID
            BuildOneFieldKeyString(FieldNames.OfficerIDFieldName, ref oKeyString);

            // LicPlate and/or VIN
            BuildOneFieldKeyString(FieldNames.VehLicNoFieldName, ref oKeyString);
            BuildOneFieldKeyString(FieldNames.VehVINFieldName, ref oKeyString);
#endif
        }
    }
}
