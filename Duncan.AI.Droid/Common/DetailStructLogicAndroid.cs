
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
using System.Threading.Tasks;
using ReinoControls;

namespace Duncan.AI.Droid.Common
{
    public class DetailStructLogicAndroid : IssueStructLogicAndroid
    {



        public DataRow GetFirstRecordForMasterKey(int iMasterKey)
        {
            // default to not found
            DataRow loRecordRow = null;

            // Convert the master key to a string
            string loMasterKeyStr = iMasterKey.ToString();

            // Perform a search, returning the primarykey of the found record.
            //return IssueStruct.MainTable.HighTableRevision.Tables[0].FilterSearch(FieldNames.MasterKeyFieldName, loMasterKeyStr, "", false);

            CommonADO oneCommonADO = new CommonADO();

            DataTable loMatchingDetailRowsTable = oneCommonADO.GetRowsWithMasterKey(loMasterKeyStr, IssueStruct.MainTable.Name);
            if (loMatchingDetailRowsTable != null)
            {
                if (loMatchingDetailRowsTable.Rows.Count > 0)
                {
                    loRecordRow = loMatchingDetailRowsTable.Rows[0];
                }
            }

            // final answer
            return loRecordRow;
        }


        public int FindRecordForMasterKey(int iMasterKey)
        {
            // default to not found
            int loRecordId = -1;

            DataRow loResultRow = GetFirstRecordForMasterKey(iMasterKey);
            if (loResultRow != null)
            {
                string loRowId = CommonADO.GetSafeColumnStringValueFromDataRow(loResultRow, AutoISSUE.DBConstants.sqlMasterKeyStr);
                if (string.IsNullOrEmpty(loRowId) == false)
                {
                    Int32.TryParse(loRowId, out loRecordId);
                }
            }

            // final answer
            return loRecordId;
        }

        //public int FindRecordForMasterKey(int iMasterKey)
        //{
        //    // default to not found
        //    int loRecordId = -1;

        //    // Convert the master key to a string
        //    string loMasterKeyStr = iMasterKey.ToString();

        //    // Perform a search, returning the primarykey of the found record.
        //    return IssueStruct.MainTable.HighTableRevision.Tables[0].FilterSearch(FieldNames.MasterKeyFieldName, loMasterKeyStr, "", false);
        //}

        public int CopySourceValuesFromTable(TTTable iSrcTable)
        {
            int loFldNdx;
            TTableFldDef loDestFld;

            // Get reference to data table we will be using
            TTTable fTable = this.IssueStruct.MainTable.HighTableRevision.Tables[0];
            for (loFldNdx = 0; loFldNdx < this.IssueStruct.MainTable.GetFieldCnt(); loFldNdx++)
            {
                loDestFld = this.IssueStruct.MainTable.GetField(loFldNdx);
                fTable.SetFormattedFieldData(loFldNdx, "", iSrcTable.GetFormattedFieldData(loDestFld.Name, ""));
            }
            return 0;
        }
    }
}
