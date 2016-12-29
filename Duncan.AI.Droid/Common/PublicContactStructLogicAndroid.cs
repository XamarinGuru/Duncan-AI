
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
    public class PublicContactStructLogic : SearchStructLogicAndroid
    {
        /// <summary>
        /// Performs whatever action was selected. Main job is to find the form that is to 
        /// be entered, prepare it, then launch the issuance.
        /// Returns the Edit result of the form.
        /// </summary>
        public int PerformActionTaken(IssFormBuilder iIssFormBuilder)
        {

#if _original_
            TIssStruct loIssStruct;
            IssueStructLogicAndroid loIssStuctLogic;
            TBaseIssForm loIssForm;
            string loIssFormName = "";
            int loStructIssueResult;
            TTEdit loPubEdit;
            TTTable DataTable = this.IssueStruct.MainTable.HighTableRevision.Tables[0];

            // Make sure SrcIssueNo isn't stuck w/ any residual values
            if ((loPubEdit = FindCfgEditByName(iIssFormBuilder.CfgForm, FieldNames.SrcIssueNoFieldName)) != null)
                loPubEdit.Behavior.SetEditBufferAndPaint("");
            if ((loPubEdit = FindCfgEditByName(iIssFormBuilder.CfgForm, FieldNames.SrcIssueNoPfxFieldName)) != null)
                loPubEdit.Behavior.SetEditBufferAndPaint("");
            if ((loPubEdit = FindCfgEditByName(iIssFormBuilder.CfgForm, FieldNames.SrcIssueNoSfxFieldName)) != null)
                loPubEdit.Behavior.SetEditBufferAndPaint("");

            // Copy the form data into the structure
            this.IssueFormLogic.ReadFieldValuesFromForm(iIssFormBuilder.CfgForm, DataTable);

            // Get the value of the issuance struct
            loIssStruct = GetIssueStructByName(DataTable.GetFormattedFieldData(FieldNames.PubContActionTakenStructFieldName, ""));
            if (loIssStruct == null) // no EditActionStruct field, so can't launch a structure
                return FormEditResults.FormEditResult_OK; // OK to save the data 'cuz form issuance wasn't cancelled
            loIssStuctLogic = ((IssueStructLogicAndroid)(loIssStruct.StructLogicObj));

            // Get the name of the issuance form
            loIssFormName = DataTable.GetFormattedFieldData(FieldNames.PubContActionTakenFormFieldName, "");

            if ((loIssFormName == null) || (loIssFormName == ""))
                loIssForm = loIssStuctLogic.GetCfgIssueForm();
            else
                loIssForm = loIssStuctLogic.GetCfgFormByPartialName(loIssFormName);

            if (loIssForm == null)
            {
                AppMessageBox.ShowMessageWithBell(loIssStruct.Name + ": Form does not exist to issue", "", "");
                return 0;
            }

            // Make sure form grabs whatever data it can from this structure
            SetIssueSource(loIssStuctLogic.IssueFormLogic, DataTable);

            // Force the issuance form to use mag data if we used it.
            ((IssueStructLogicAndroid)(loIssStruct.StructLogicObj)).IssueFormLogic.fUseSourceMagSwipe =
                iIssFormBuilder.fMagStripeUsed;

            if ((loStructIssueResult = loIssStuctLogic.IssueRecord(EditRestrictionConsts.femSingleEntry | 
                EditRestrictionConsts.femNewEntry, ((TPublicContactStruct)(IssueStruct)).IssueStructFirstFocus, loIssForm.Name)) == 
                FormEditResults.FormEditResult_OK)
            {
                // A record was issued, capture its issue number
                TTEdit loIssEdit;

                // Start w/ IssueNo
                if (((loIssEdit = FindCfgEditByName(loIssForm, FieldNames.IssueNoFieldName)) != null) &&
                     ((loPubEdit = FindCfgEditByName(iIssFormBuilder.CfgForm, FieldNames.SrcIssueNoFieldName)) != null))
                {
                    loPubEdit.Behavior.SetEditBufferAndPaint(loIssEdit.Behavior.GetText());
                }

                // then w/ IssueNoPfx
                if (((loIssEdit = FindCfgEditByName(loIssForm, FieldNames.IssueNoPfxFieldName)) != null) &&
                     ((loPubEdit = FindCfgEditByName(iIssFormBuilder.CfgForm, FieldNames.SrcIssueNoPfxFieldName)) != null))
                {
                    loPubEdit.Behavior.SetEditBufferAndPaint(loIssEdit.Behavior.GetText());
                }
                // then w/ IssueNoSfx
                if (((loIssEdit = FindCfgEditByName(loIssForm, FieldNames.IssueNoSfxFieldName)) != null) &&
                     ((loPubEdit = FindCfgEditByName(iIssFormBuilder.CfgForm, FieldNames.SrcIssueNoSfxFieldName)) != null))
                {
                    loPubEdit.Behavior.SetEditBufferAndPaint(loIssEdit.Behavior.GetText());
                }
            }

            UndoIssueSource(loIssStuctLogic.IssueFormLogic);

            return loStructIssueResult;
#else
            return 0;
#endif

        }
    }
}
