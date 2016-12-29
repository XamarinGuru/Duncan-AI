using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Duncan.AI.Droid.Utils.EditControlManagement;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Duncan.AI.Droid.Utils.EditControlManagement.EditRules;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils.DataAccess;
using Duncan.AI.Droid.Utils.HelperManagers;
using Mono.Data.Sqlite;
using System.IO;
using System.Json;
using System.Data;
using Android.Widget;
using Android.Util;
using Android.Content;
using Android.Views;
using Android.OS;
using Android.Views.InputMethods;
using System.Collections.Generic;
using System.Text;
using Android.Graphics;
using System.Text.RegularExpressions;

using XMLConfig;
using Reino.ClientConfig;

using EditRestrictionConsts = Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts;


namespace Duncan.AI.Droid
{
    public static class WirelessEnforcementOptions
    {
        public enum TMeterStatusProvider
        {
            mspDuncan = 0,
            mspDPT,
            mspParkeon,
            mspParkNOW,
            mspVerrus,  // Verrus PayByPhone support
            mspPango,
            mspDuncan_OverstayVio, // Overstay violation support
            mspMultiVendor,
            mspDuncanPayByPlate,     // Duncan PayByPlate from other providers - ParkMobile, etc.
            mspCalePayByPlate
        }


        public enum TMeterWarningLevelDetail
        {
            wldNone = 0,
            wldMeterCounts,
            wldMeterNames,
            wldMeterCountsAndNames
        }


        public static bool fPayBySpaceMapEnforcementActive = false;
        public static bool fPayBySpaceListEnforcementActive = false;
        public static bool fPayByPlateMapEnforcementActive = false;
        public static bool fPayByPlateListEnforcementActive = false;

        public static bool fWebViewClientDefinableEnabled = false;


        public static string fMeterCustomerID = string.Empty;
        public static bool fGPRSMeterDeferToPublicMode = false;
        public static bool fPreventReissueOfMSMBasedEnforcement = false;
        public static string fMeterStatusProviderName = string.Empty;
        public static TMeterStatusProvider fMeterStatusProvider = TMeterStatusProvider.mspDuncan;
        public static TMeterWarningLevelDetail fMeterWarningLevelDetail = TMeterWarningLevelDetail.wldNone;
        public static bool fGPRSMeterShowResultsWhenWarnings = false;
        public static bool fGPRSMeterShowVerboseActivity = false;


        public static int fMaxPaymentAgeMinutes = 0;
        public static int fMaxServerResponseTimeoutSeconds = 0;
        public static string fMaxPaymentAgeDisplayText = string.Empty;

        public static string fPBC_SearchZoneID_MappedField = string.Empty;
        public static string fPBC_SearchZoneDesc_MappedField = string.Empty;
        public static int fPBCRefreshIntervalSec = 0;
        public static UInt64 fPBCMaxDataAge100ns = 0;
        public static int fPBCMaxConfirmationDataAgeSec = 0;

    }

	public class ExternalEnforcementInterfaces
	{
        public ExternalEnforcementInterfaces()
		{
		}



        /// <summary>
        /// Legacy mode references
        /// </summary>
        public enum TMeterStatusProvider
        {
            mspDuncan = 0,
            mspDPT,
            mspParkeon,
            mspParkNOW,
            mspVerrus,  // Verrus PayByPhone support
            mspPango,
            mspDuncan_OverstayVio, // Overstay violation support
            mspMultiVendor,
            mspDuncanPayByPlate,     // Duncan PayByPlate from other providers - ParkMobile, etc.
            mspCalePayByPlate
        }



        #region _LegacyReferenceCode_

        private void SetEditAndChildrenPreInitialized(object iEdit) //TTEdit *iEdit )
        {
            int loNdx;

            //// defend against circular references. Don't allow a nested invocation for the same object instance.
            //if (iEdit->fEditStatePreInitializedDepth) 
            //    return;
            //iEdit->fEditStatePreInitializedDepth++;

            //iEdit->SetEditStatePreInitialized(1);
            //for (loNdx=0; loNdx < iEdit->GetDependentCnt(); loNdx++)
            //{
            //  SetEditAndChildrenPreInitialized( iEdit->GetDependent( loNdx ) );
            //}

            //iEdit->fEditStatePreInitializedDepth++;
        }



        //private bool PutEnforcementInfo(FormPanel iForm)
        //{

        //    return false;  // now in Helper. to be removed

        //    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
        //    ISharedPreferencesEditor editor = prefs.Edit();

        //    //editor.PutString(Constants.PREVIOUS_FRAGMENT, _struct.Name);
        //    //editor.PutString(Constants.ISSUENO_COLUMN, _struct.sequenceId);
        //    //editor.PutString(Constants.STRUCT_NAME_TKT_DTL, _struct.Name);

        //    editor.PutString("DID COME MAP VIEW", "TRUE");
        //    editor.PutString(AutoISSUE.DBConstants.sqlVehLicNoStr, "MYTESTVAL");

        //    editor.PutString(AutoISSUE.DBConstants.sqlIssueNumberPrefixStr, "ZZ");

        //    editor.Apply();

        //    return false;

        //    //if (_userDAOParent != null)
        //    //{
        //    //    editor.PutString(Constants.OFFICER_ID, _userDAOParent.officerId);
        //    //    editor.PutString(Constants.OFFICER_NAME, _userDAOParent.officerName);
        //    //    editor.PutString(Constants.AGENCY, _userDAOParent.agency);
        //    //}


        //    List<string> loColumnValuePairList = new List<string>();
        //    loColumnValuePairList.Add(AutoISSUE.DBConstants.sqlVehLicNoStr + "=" + "MYTESTVAL");


        //    UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlVehLicNoStr, "MYTESTVAL");

        //    var commonADO = new CommonADO();
        //    Task<bool> result = commonADO.UpdateRowWithColumnValues(iForm.thisStruct, Constants.STATUS_INPROCESS, Constants.WS_STATUS_EMPTY, _context, iForm.thisStruct.Name, loColumnValuePairList);
        //    //bool resultCont = await result;
        //    //bool resultCont = result;
        //    bool resultCont = result.Result;

        //    if (result.Result == true)
        //    {
        //        // now read it all back in
        //        Task<bool> loReadResult = iForm.UpdateStruct();
        //        //bool loFinalAnswer = await loReadResult.Result;
        //        bool loFinalAnswer = loReadResult.Result;
        //        return loFinalAnswer;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}



        //private void UpdateFieldInForm(FormPanel iForm, string iFieldName, string iFieldValue)
        //{
        //    //TTForm *loCallingForm = (TTForm *)iForm;
        //    var loCallingForm = iForm;
        //    string loSavedData = string.Empty;

        //    // copy results to calling form
        //    //TTEdit *loEdit;
        //    object loEdit = null;


        //    int loNotifyEvent;
        //    if (iForm == null) return;

        //    // find the field to update
        //    XMLConfig.PanelField onePanelfield = iForm.FindPanelControlByName(iFieldName);

        //    if (onePanelfield == null)
        //    {
        //        return;
        //    }

        //    //if ( (loEdit = (TTEdit *)loCallingForm->FindControlByName( iFieldName ) ) == 0 ) return;
        //    string loFieldType = onePanelfield.FieldType.ToUpper();



        //    //// get the current value
        //    //loSavedData = uHALr_CopyOfStr( loEdit->GetEditBufferPtr() );
        //    loSavedData = onePanelfield.Value;

        //    //  // ok now  stuff the value in as a string
        //    //loEdit->SetEditBufferAndPaint( iFieldValue );
        //    onePanelfield.Value = iFieldValue;

        //    loNotifyEvent = EditRestrictionConsts.dneParentFieldExit;
        //    //// did the data change as a result the stuff?
        //    //if ( STRCMP(loSavedData, loEdit->GetEditBufferPtr()) !=0 )
        //    //{
        //    //    // note that the data itself has changed
        //    //  loEdit->ProcessRestrictions( dneDataChanged, 0 );
        //    //  loNotifyEvent |= dneParentDataChanged;
        //    //}
        //    if (loSavedData.Equals(onePanelfield.Value) == false)
        //    {
        //        //    // note that the data itself has changed
        //        //  loEdit->ProcessRestrictions( dneDataChanged, 0 );
        //        //NotifyDependents(EditRestrictionConsts.dneParentDataChanged);

        //        loNotifyEvent |= EditRestrictionConsts.dneParentDataChanged;

        //    }


        //    // make sure date & time fields are valid 
        //    //    if ((_FieldType == TEditFieldType.efDate) && (!FieldIsBlank()))
        //    //    {
        //    //        if ((loResult = DateStringToOSDate(_EditMask, _EditBuffer, ref loOSDate)) < 0)
        //    //        {
        //    //            if (this._CfgCtrl != null)
        //    //                oErrMsg = "Invalid date for " + this._CfgCtrl.Name + ".\nMask: " + _EditMask + " Data: " + _EditBuffer;
        //    //            else
        //    //                oErrMsg = "Invalid date" + ".\nMask: " + _EditMask + " Data: " + _EditBuffer;
        //    //            RingBell(this);
        //    //        }
        //    //        else
        //    //        {
        //    //            string loTmpDate = "";
        //    //            OSDateToDateString(loOSDate, _EditMask, ref loTmpDate);
        //    //            SetEditBufferAndPaint(loTmpDate);
        //    //        }
        //    //        return loResult; // not a valid date 
        //    //    }

        //    //    if ((_FieldType == TEditFieldType.efTime) && (!FieldIsBlank()))
        //    //    {
        //    //        if ((loResult = TimeStringToOSTime(_EditMask, _EditBuffer, ref loOSTime)) < 0)
        //    //        {
        //    //            if (this._CfgCtrl != null)
        //    //                oErrMsg = "Invalid time for " + this._CfgCtrl.Name + ".\nMask: " + _EditMask + " Data: " + _EditBuffer;
        //    //            else
        //    //                oErrMsg = "Invalid time" + ".\nMask: " + _EditMask + " Data: " + _EditBuffer;
        //    //            RingBell(this);
        //    //        }
        //    //        else
        //    //        {
        //    //            string loTmpTime = "";
        //    //            OSTimeToTimeString(loOSTime, _EditMask, ref loTmpTime);
        //    //            SetEditBufferAndPaint(loTmpTime);
        //    //        }
        //    //        return loResult; // not a valid date 
        //    //    }

        //    //    return 0;
        //    //}

        //    //// release the mem
        //    //FREE( loSavedData );

        //    //    // let the field do whatever initialization it needs to
        //    //loEdit->NotifyDependents( loNotifyEvent );

        //    //// and suppress the "FirstFocus" event..
        //    //SetEditAndChildrenPreInitialized( loEdit );
        //}




        //private void SetWirelessEnforcementInfo(FormPanel iForm)
        //{
        //    TMeterStatusProvider fMeterStatusProvider = TMeterStatusProvider.mspMultiVendor;

        //    string fBayNoOrSpaceNoTextCaptialized = "SPACE";
        //    string fPBC_SearchZoneID_MappedField = "";
        //    string fPBC_SearchZoneDesc_MappedField = "";
        //    bool fExcludeStateInPayByCellVehicleFilters = false;
        //    bool fSendVehicleStatusRequests = true;
        //    bool fPBC_AnyZoneEnforcementEnabled = true;


        //    string lofReinoInfoForm_MultiVendor_GetSelectedBayExpiredMinutes = "";
        //    string lofReinoInfoForm_MultiVendor_GetSelectedBayClusterID = "";
        //    string lofReinoInfoForm_MultiVendor_GetSelectedBayLUT = "";
        //    string lofReinoInfoForm_MultiVendor_GetSelectedMeterType = "";
        //    string lofReinoInfoForm_MultiVendor_GetSelectedBayID_Internal = "";
        //    string lofReinoInfoForm_MultiVendor_GetSelectedMeterID_Internal = "";
        //    string lofReinoInfoForm_MultiVendor_GetSelectedMeterName = "1234";


        //    string loGetSelectedBayNo = "6121";
        //    string loGetSelectedBayName = "12120-128";
        //    string loGetSelectedBayEnforcementText = "EXPIRED 17 MINS";
        //    string loGetSelectedMeterName = "3-0192";
        //    string loGetMeterDescWinBufferPtr = "MTR";
        //    string loGetMeterWinDescMeterID = "";
        //    string loGetSelectedMeterType = "MSM";

        //    string lofReinoInfoForm_MultiVendor_GetSelectedBayName = "781";
        //    string lofReinoInfoForm_OverstayVio_GetSelectedMeter = "123";
        //    string lofReinoInfoForm_OverstayVio_GetSelectedBayNo = "871";

        //    string loReinoInfoForm_Verrus_GetSelectedLicPlate = "CBA323";
        //    string loReinoInfoForm_Verrus_GetSelectedLicPlateState = "CA";

        //    string lofReinoInfoForm_GetSelectedBayNo = "";
        //    string lofReinoInfoForm_GetSelectedBayScanDateTime = "";
        //    string lofReinoInfoForm_GetSelectedBayRTC = "";
        //    string lofReinoInfoForm_GetSelectedBayLUT = "";
        //    string lofReinoInfoForm_GetSelectedBayMode = "";
        //    string lofReinoInfoForm_GetSelectedBayClusterID = "";
        //    string lofReinoInfoForm_GetSelectedBayClusterList = "";
        //    string lofReinoInfoForm_GetSelectedBayExpiredMinutes = "";
        //    string lofReinoInfoForm_GetSelectedBayStatusCode = "";
        //    string lofReinoInfoForm_GetSelectedBayStateCode = "";


        //    //uHALr_FormatStr( "%s: %s", loMsg1, 80, fBayNoOrSpaceNoTextCaptialized, fReinoInfoForm->GetSelectedBayNo() );
        //    //sprintf( loMsg2, "Expired:%s", fReinoInfoForm->GetSelectedBayEnforcmentText() );



        //    //Get the GIS Meter JsonValue Object 
        //    var lastGISValue = DroidContext.XmlCfg.GetGISMeterJsonValueObject();

        //    //get individual properties from it
        //    //Get the meter ID (which is an int value):
        //    var meterId = DroidContext.XmlCfg.GetGISMeterPropertyInt("MeterId");

        //    //get meterName which is a string:
        //    var meterName = DroidContext.XmlCfg.GetGISMeterPropertyString("MeterName");



        //    string loMeterID = meterId.ToString(); // plenty big

        //    string loMsg1 = string.Empty;
        //    string loMsg2 = string.Empty;



        //    UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlVehLicNoStr, "TESTVAL");


        //    //if (( fMeterStatusProvider == mspVerrus ) || (GetMeterStatusProvider() == mspPango) || ( fMeterStatusProvider == mspDuncanPayByPlate ) || ( fMeterStatusProvider == mspCalePayByPlate ))
        //    //{
        //    //    sprintf( loMeterID, "%s", fReinoInfoForm_Verrus->GetMeterWinMeterID() );
        //    //}
        //    //else if ( fMeterStatusProvider == mspMultiVendor )
        //    //{
        //    //    sprintf( loMeterID, "%s", fReinoInfoForm_MultiVendor->GetMeterZoneSelectID() );
        //    //}
        //    //else if ( fMeterStatusProvider == mspDuncan_OverstayVio )
        //    //{
        //    //    sprintf( loMeterID, "%s", fReinoInfoForm_OverstayVio->GetSelectedMeter() );
        //    //}
        //    //else
        //    //{
        //    //    // Legacy PAM
        //    //    sprintf( loMeterID, "%d", GetMeterNo(loMeterNdx) );
        //    //}

        //    switch (fMeterStatusProvider)
        //    {
        //        case TMeterStatusProvider.mspDPT:
        //            {
        //                // stalls need to be padded with leading zeros for look-ups
        //                loMeterID = loMeterID.PadLeft(4, '0');
        //                break;
        //            }

        //        case TMeterStatusProvider.mspParkNOW:
        //            {
        //                // stalls need to be padded with leading zeros for look-ups
        //                loMeterID = loMeterID.PadLeft(7, '0');
        //                break;
        //            }

        //        default:
        //            {
        //                // do nothing
        //                break;
        //            }
        //    }



        //    // format the results
        //    switch (fMeterStatusProvider)
        //    {

        //        case TMeterStatusProvider.mspDPT:
        //            {
        //                // stall range mode, no meter number to list here
        //                //uHALr_FormatStr( "%s: %s", loMsg1, 80, fBayNoOrSpaceNoTextCaptialized, fReinoInfoForm->GetSelectedBayNo() );
        //                loMsg1 = fBayNoOrSpaceNoTextCaptialized + " " + loGetSelectedBayNo;
        //                //sprintf( loMsg2, "Expired:%s", fReinoInfoForm->GetSelectedBayEnforcmentText() );
        //                loMsg2 = "Expired: " + loGetSelectedBayEnforcementText;
        //                break;
        //            }

        //        case TMeterStatusProvider.mspParkNOW:
        //            {
        //                // stall range mode, no meter number to list here
        //                //uHALr_FormatStr( "%s: %s", loMsg1, 80, fBayNoOrSpaceNoTextCaptialized, fReinoInfoForm->GetSelectedBayNo() );
        //                loMsg1 = fBayNoOrSpaceNoTextCaptialized + " " + loGetSelectedBayNo;

        //                // parknow doesn't have expiration times, so we can't display it
        //                //strcpy( loMsg2, "" );
        //                loMsg2 = "";
        //                break;
        //            }

        //        default:
        //            {
        //                // Verrus PayByPhone support (begin)
        //                if (
        //                    (fMeterStatusProvider == TMeterStatusProvider.mspVerrus) ||
        //                    (fMeterStatusProvider == TMeterStatusProvider.mspPango) ||
        //                    (fMeterStatusProvider == TMeterStatusProvider.mspDuncanPayByPlate) ||
        //                    (fMeterStatusProvider == TMeterStatusProvider.mspCalePayByPlate)
        //                    )
        //                {
        //                    // For vehicle-based enforcement, stuffing remarks isn't normally applicable
        //                    /*
        //                    uHALr_FormatStr( "Meter No: %s, %s: %s", loMsg1, 80, fReinoInfoForm_Verrus->fMeterDescWin->GetEditBufferPtr(), fBayNoOrSpaceNoTextCaptialized, fReinoInfoForm_Verrus->GetSelectedBayNo() );
        //                    sprintf( loMsg2, "Expired:%s", fReinoInfoForm_Verrus->GetSelectedBayEnforcmentText() );
        //                    */
        //                    //sprintf( loMsg1, "");
        //                    loMsg1 = "";
        //                    //sprintf( loMsg2, "");
        //                    loMsg2 = "";
        //                    break;
        //                }
        //                else if (fMeterStatusProvider == TMeterStatusProvider.mspMultiVendor)
        //                {
        //                    //// Get the meter type so we can see if its a single space meter (where bay name/number is not important)

        //                    // Compare strings to see if its known to be a singlespace meter
        //                    if (loGetSelectedMeterType.Equals("SSM") == true)
        //                    {
        //                        //uHALr_FormatStr( "Meter: %s", loMsg1, 80, fReinoInfoForm_MultiVendor->GetSelectedMeterName());
        //                        loMsg1 = "Meter " + loGetSelectedMeterName;

        //                        //sprintf( loMsg2, "Expired" );
        //                        loMsg2 = "Expired";
        //                    }
        //                    else
        //                    {
        //                        // "uHALr_FormatStr" and "sprintf" don't seem to work the way we expected, and results in both params having same value
        //                        // This must be something strange about the pointer returned by our "GetSelectedXXXX" methods
        //                        /*
        //                        uHALr_FormatStr( "Meter No: %s, %s: %s", loMsg1, 80,
        //                            fReinoInfoForm_MultiVendor->GetSelectedMeterName(), //fMeterDescWin->GetEditBufferPtr(),
        //                            fBayNoOrSpaceNoTextCaptialized,
        //                            fReinoInfoForm_MultiVendor->GetSelectedBayName() );
        //                        */



        //                        //memset( loMsg1, 0, sizeof( loMsg1 ) );
        //                        ////strcpy( (char *)&loMsg1, "Meter No: " );
        //                        //strcpy( (char *)&loMsg1, "Meter/Zone: " );
        //                        //strcat( (char *)&loMsg1, fReinoInfoForm_MultiVendor->GetSelectedMeterName() );
        //                        //strcat( (char *)&loMsg1, ", " );
        //                        //strcat( (char *)&loMsg1, fBayNoOrSpaceNoTextCaptialized );
        //                        //strcat( (char *)&loMsg1, ": " );
        //                        //strcat( (char *)&loMsg1, fReinoInfoForm_MultiVendor->GetSelectedBayName() );
        //                        loMsg1 = "Meter/Zone: " + loGetSelectedMeterName + ", " + fBayNoOrSpaceNoTextCaptialized + ": " + loGetSelectedBayName;

        //                        //sprintf( loMsg2, "Expired: %s", fReinoInfoForm_MultiVendor->GetSelectedBayEnforcmentText() );
        //                        loMsg2 = "Expired: " + loGetSelectedBayEnforcementText;
        //                    }


        //                    break;
        //                }
        //                else if (fMeterStatusProvider == TMeterStatusProvider.mspDuncan_OverstayVio)
        //                {
        //                    //uHALr_FormatStr( "Sensor: %s, Space: %s", loMsg1, 80, fReinoInfoForm_OverstayVio->GetSelectedMeter(), fReinoInfoForm_OverstayVio->GetSelectedBayNo() );
        //                    //uHALr_FormatStr( "Stayed: %s, Limit: %s", loMsg2, 80, fReinoInfoForm_OverstayVio->GetSelectedBayEnforcmentText(), fReinoInfoForm_OverstayVio->GetSelectedRegulations() );

        //                    //sprintf( loMsg1, "Sensor: %s, Space: %s", fReinoInfoForm_OverstayVio->GetSelectedMeter(), fReinoInfoForm_OverstayVio->GetSelectedBayNo() );
        //                    //sprintf( loMsg2, "Stayed: %s, Limit: %s", fReinoInfoForm_OverstayVio->GetSelectedBayEnforcmentText(), fReinoInfoForm_OverstayVio->GetSelectedRegulations() );

        //                    // "uHALr_FormatStr" and "sprintf" don't seem to work the way we expected, and results in both params having same value
        //                    // This must be something strange about the pointer returned by our "GetSelectedXXXX" methods
        //                    //memset( loMsg1, 0, sizeof( loMsg1 ) );
        //                    //strcpy( (char *)&loMsg1, "Sensor: " );
        //                    //strcat( (char *)&loMsg1, fReinoInfoForm_OverstayVio->GetSelectedMeter() );
        //                    //strcat( (char *)&loMsg1, "  Space: " );
        //                    //strcat( (char *)&loMsg1, fReinoInfoForm_OverstayVio->GetSelectedBayNo() );
        //                    //strcat( (char *)&loMsg1, "  Arrived: " );
        //                    //strcat( (char *)&loMsg1, fReinoInfoForm_OverstayVio->GetSelectedArrival() );

        //                    //memset( loMsg2, 0, sizeof( loMsg2 ) );
        //                    //strcpy( (char *)&loMsg2, "Stayed: " );
        //                    //strcat( (char *)&loMsg2, fReinoInfoForm_OverstayVio->GetSelectedBayEnforcmentText() );
        //                    //strcat( (char *)&loMsg2, "  Limit: " );
        //                    //strcat( (char *)&loMsg2, fReinoInfoForm_OverstayVio->GetSelectedRegulations() );

        //                    break;
        //                }
        //                else
        //                {
        //                    //uHALr_FormatStr( "Meter No: %s, %s: %s", loMsg1, 80, fReinoInfoForm->fMeterDescWin->GetEditBufferPtr(), fBayNoOrSpaceNoTextCaptialized, fReinoInfoForm->GetSelectedBayNo() );
        //                    ////uHALr_FormatStr( "Meter No: %s, Bay: %s", loMsg1, 80, MeterNoDisplay, fReinoInfoForm->GetSelectedBayNo() );
        //                    loMsg1 = "Meter No:  " + loGetMeterDescWinBufferPtr + " " + fBayNoOrSpaceNoTextCaptialized + " " + loGetSelectedBayNo;


        //                    //sprintf( loMsg2, "Expired:%s", fReinoInfoForm->GetSelectedBayEnforcmentText() );
        //                    loMsg2 = "";

        //                    break;
        //                }
        //                // Verrus PayByPhone support (end)
        //            }
        //    }


        //    if (iForm == null) return; // -1; // nothing more to do




        //    switch (fMeterStatusProvider)
        //    {
        //        case TMeterStatusProvider.mspDPT:
        //            {
        //                // update location space number field in calling form
        //                //UpdateFieldInForm( iForm, LocMeterFieldName, fReinoInfoForm->GetSelectedBayNo() );
        //                UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlLocationMeterNumberStr, loGetSelectedBayNo);
        //                break;
        //            }

        //        case TMeterStatusProvider.mspParkNOW:
        //            {
        //                // update location space number field in calling form
        //                //UpdateFieldInForm( iForm, LocMeterFieldName, fReinoInfoForm->GetSelectedBayNo() );
        //                UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlLocationMeterNumberStr, loGetSelectedBayNo);
        //                break;
        //            }

        //        default:
        //            {
        //                // Verrus PayByPhone support (begin)
        //                if (
        //                    (fMeterStatusProvider == TMeterStatusProvider.mspVerrus) ||
        //                    (fMeterStatusProvider == TMeterStatusProvider.mspPango) ||
        //                    (fMeterStatusProvider == TMeterStatusProvider.mspDuncanPayByPlate) ||
        //                    (fMeterStatusProvider == TMeterStatusProvider.mspCalePayByPlate))
        //                {
        //                    // update ZoneID field in calling form (when mapping is defined)
        //                    if (fPBC_SearchZoneID_MappedField.Length > 0)
        //                    {
        //                        UpdateFieldInForm(iForm, fPBC_SearchZoneID_MappedField, loMeterID/*fReinoInfoForm_Verrus->GetMeterWinMeterID()*/ );
        //                    }

        //                    // update ZoneDesc field in calling form (when mapping is defined)
        //                    if (fPBC_SearchZoneDesc_MappedField.Length > 0)
        //                    {
        //                        //UpdateFieldInForm( iForm, fPBC_SearchZoneDesc_MappedField, fReinoInfoForm_Verrus->GetMeterWinDescMeterID() );
        //                        UpdateFieldInForm(iForm, fPBC_SearchZoneDesc_MappedField, loGetMeterWinDescMeterID);
        //                    }

        //                    break;
        //                }
        //                else if (fMeterStatusProvider == TMeterStatusProvider.mspMultiVendor)
        //                {
        //                    // update location meter field in calling form
        //                    //UpdateFieldInForm( iForm, LocMeterFieldName, fReinoInfoForm_MultiVendor->GetSelectedMeterName() );
        //                    UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlLocationMeterNumberStr, loGetSelectedMeterName);
        //                    break;
        //                }
        //                else if (fMeterStatusProvider == TMeterStatusProvider.mspDuncan_OverstayVio)
        //                {
        //                    // Stuff fields for overstay
        //                    //UpdateFieldInForm( iForm, LocMeterFieldName, fReinoInfoForm_OverstayVio->GetSelectedMeter() );
        //                    UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlLocationMeterNumberStr, lofReinoInfoForm_OverstayVio_GetSelectedMeter);
        //                    break;
        //                }
        //                else
        //                {
        //                    // update location meter field in calling form
        //                    //UpdateFieldInForm( iForm, LocMeterFieldName, fReinoInfoForm->fMeterDescWin->GetEditBufferPtr() );
        //                    UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlLocationMeterNumberStr, loGetMeterDescWinBufferPtr);
        //                    break;
        //                }
        //                // Verrus PayByPhone support (end)
        //            }
        //    }



        //    // Verrus PayByPhone support (begin)
        //    if ((fMeterStatusProvider == TMeterStatusProvider.mspVerrus) || (fMeterStatusProvider == TMeterStatusProvider.mspPango) || (fMeterStatusProvider == TMeterStatusProvider.mspDuncanPayByPlate) || (fMeterStatusProvider == TMeterStatusProvider.mspCalePayByPlate))
        //    {
        //        // Not applicable to PayByPhone
        //        /*
        //        // update BayNo field
        //        UpdateFieldInForm( iForm, LocMeterBayNoFieldName, fReinoInfoForm_Verrus->GetSelectedBayNo() );
        //        */
        //    }
        //    else if (fMeterStatusProvider == TMeterStatusProvider.mspMultiVendor)
        //    {
        //        // update BayNo field
        //        //UpdateFieldInForm( iForm, LocMeterBayNoFieldName, fReinoInfoForm_MultiVendor->GetSelectedBayName() );
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlLocMeterBayNumberStr, lofReinoInfoForm_MultiVendor_GetSelectedBayName);
        //    }
        //    else if (fMeterStatusProvider == TMeterStatusProvider.mspDuncan_OverstayVio)
        //    {
        //        // Stuff fields for overstay
        //        //UpdateFieldInForm( iForm, LocMeterBayNoFieldName, fReinoInfoForm_OverstayVio->GetSelectedBayNo() );
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlLocMeterBayNumberStr, lofReinoInfoForm_OverstayVio_GetSelectedBayNo);
        //    }
        //    else
        //    {
        //        // update BayNo field
        //        //UpdateFieldInForm( iForm, LocMeterBayNoFieldName, fReinoInfoForm->GetSelectedBayNo() );
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlLocMeterBayNumberStr, loGetSelectedBayNo);
        //    }
        //    // Verrus PayByPhone support (end)

        //    // update remark 1
        //    if (loMsg1.Length > 0)
        //    {
        //        //UpdateFieldInForm( iForm, Remark1FieldName, loMsg1 );
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlRemark1Str, loMsg1);

        //    }

        //    // update remark 2
        //    if (loMsg2.Length > 0)
        //    {
        //        //UpdateFieldInForm( iForm, Remark2FieldName, loMsg2 );
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlRemark2Str, loMsg2);
        //    }



        //    // capture some info about the meter scan for later reporting/analysis:
        //    // raw meter id, space no, time scanned, config type, (pam, paired, etc), paired meters/meters in cluster
        //    // only these fields defined in the struct will get saved

        //    //const int MAX_ENFORCEMENT_INFO_LEN = 250;                        // maxed out for meter cluster info, most wont be very long
        //    //char loMeterEnforcementInfo[ MAX_ENFORCEMENT_INFO_LEN ];	  // and the size struct will limit it TTRegistry.regardless
        //    string loMeterEnforcementInfo = string.Empty;


        //    // Verrus PayByPhone support (begin)
        //    if ((fMeterStatusProvider == TMeterStatusProvider.mspVerrus) || (fMeterStatusProvider == TMeterStatusProvider.mspPango) || (fMeterStatusProvider == TMeterStatusProvider.mspDuncanPayByPlate) || (fMeterStatusProvider == TMeterStatusProvider.mspCalePayByPlate))
        //    {
        //        //char loCurrentVehLicNo[MAX_VEHLICNO_SIZE];
        //        //char loCurrentVehLicSt[MAX_VEHLICST_SIZE];
        //        string loCurrentVehLicNo = string.Empty;
        //        string loCurrentVehLicSt = string.Empty;


        //        //sprintf(loMeterEnforcementInfo, "%s", loMeterID/*fReinoInfoForm_Verrus->GetMeterWinMeterID()*/ );
        //        loMeterEnforcementInfo = loMeterID;
        //        //UpdateFieldInForm( iForm, PBC_ENF_ZONEIDFieldName, loMeterEnforcementInfo );
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlPBC_ENF_ZONEIDFieldName, loMeterEnforcementInfo);



        //        //sprintf(loCurrentVehLicNo, "%s", fReinoInfoForm_Verrus->GetSelectedLicPlate());
        //        loCurrentVehLicNo = loReinoInfoForm_Verrus_GetSelectedLicPlate;
        //        //UpdateFieldInForm( iForm, VehLicNo, loCurrentVehLicNo );
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlVehLicNoStr, loCurrentVehLicNo);
        //        //UpdateFieldInForm( iForm, PBC_ENF_LICPLATEFieldName, loCurrentVehLicNo );
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlPBC_ENF_LICPLATEFieldName, loCurrentVehLicNo);

        //        //sprintf(loCurrentVehLicSt, "%s", fReinoInfoForm_Verrus->GetSelectedLicPlateState());
        //        loCurrentVehLicSt = loReinoInfoForm_Verrus_GetSelectedLicPlateState;
        //        //UpdateFieldInForm( iForm, VehLicSt, loCurrentVehLicSt );
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlPBC_ENF_LICSTATEFieldName, loCurrentVehLicSt);
        //        //UpdateFieldInForm( iForm, PBC_ENF_LICSTATEFieldName, loCurrentVehLicSt );
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlPBC_ENF_LICSTATEFieldName, loCurrentVehLicSt);


        //        // now we can update GPRSRefresh to include the vehicle info
        //        if ((fMeterStatusProvider == TMeterStatusProvider.mspPango) || (fMeterStatusProvider == TMeterStatusProvider.mspCalePayByPlate))
        //        {
        //            // should we keep the license state?
        //            if (fExcludeStateInPayByCellVehicleFilters == true)
        //            {
        //                // nope, wipe it out
        //                loCurrentVehLicSt = "";
        //            }

        //            /* old way was Pango exclusive, now we also allow anyzone support to trigger this, below
        //            // should we submit request with vehicle info, to trigger a GetVehicleStatus at the server side? (In addition to GetZoneStatues)
        //           if ( glReinoMeter->fReinoInfoForm_Verrus->fSendVehicleStatusRequests == true )
        //           {
        //              glReinoMeter->SubmitNewGPRSRefreshMeterID( glReinoMeter->GetCurrentClusterID(), loMeterID, loCurrentVehLicNo, loCurrentVehLicSt, 0);
        //           }
        //            */

        //        }


        //        // new way - not Pango exlusive - should we submit request with vehicle info, to trigger a GetVehicleStatus at the server side? (In addition to GetZoneStatuses)
        //        if (
        //            //( glReinoMeter->fReinoInfoForm_Verrus->fSendVehicleStatusRequests == true ) ||
        //            //( glReinoMeter->fPBC_AnyZoneEnforcementEnabled == TRUE ) 
        //            (fSendVehicleStatusRequests == true) ||
        //            (fPBC_AnyZoneEnforcementEnabled == true)
        //            )
        //        {
        //            //glReinoMeter->SubmitNewGPRSRefreshMeterID( glReinoMeter->GetCurrentClusterID(), loMeterID, loCurrentVehLicNo, loCurrentVehLicSt, 0);
        //        }


        //        //sprintf(loMeterEnforcementInfo, "%s", fReinoInfoForm_Verrus->GetSelectedBayEnforcmentText() );
        //        loMeterEnforcementInfo = loGetSelectedBayEnforcementText;

        //        //UpdateFieldInForm( iForm, PBC_ENF_REASONFieldName, loMeterEnforcementInfo );
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlPBC_ENF_REASONFieldName, loMeterEnforcementInfo);
        //    }
        //    else if (fMeterStatusProvider == TMeterStatusProvider.mspDuncan_OverstayVio)
        //    {
        //        // Since we are writing a ticket based on the OverstayVio.cmd file from the web-browser, its time to delete it
        //        // so we don't keep seeing the same data
        //        //try
        //        //{
        //        //    if ( OSFileExists("\\AIWebProxy\\OverstayVio.cmd") )
        //        //        DeleteFile(L"\\AIWebProxy\\OverstayVio.cmd");
        //        //}
        //        //catch (...)
        //        //{
        //        //}

        //        //sprintf(loMeterEnforcementInfo, "%s", fReinoInfoForm_OverstayVio->GetSelectedMeter());
        //        loMeterEnforcementInfo = lofReinoInfoForm_OverstayVio_GetSelectedMeter;
        //        //UpdateFieldInForm(iForm, EnforcedMeterIDFieldName, loMeterEnforcementInfo);
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterIDFieldName, loMeterEnforcementInfo);

        //        // We only want background refreshes to occur when user is inside the enforcement screen,
        //        // so we will now send an empty meter data so the background refresh will stop
        //        //glReinoMeter->SubmitNewGPRSRefreshMeterID( "", "", "", "", 0 );
        //    }
        //    else if (fMeterStatusProvider == TMeterStatusProvider.mspMultiVendor)
        //    {
        //        //sprintf(loMeterEnforcementInfo, "%s", fReinoInfoForm_MultiVendor->GetSelectedMeterName());
        //        //UpdateFieldInForm( iForm, EnforcedMeterIDFieldName, loMeterEnforcementInfo );
        //        loMeterEnforcementInfo = lofReinoInfoForm_MultiVendor_GetSelectedMeterName;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterIDFieldName, loMeterEnforcementInfo);

        //        //sprintf(loMeterEnforcementInfo, "%s", fReinoInfoForm_MultiVendor->GetSelectedMeterID_Internal());
        //        //UpdateFieldInForm( iForm, EnforcedMeterIDInternalFieldName, loMeterEnforcementInfo );
        //        loMeterEnforcementInfo = lofReinoInfoForm_MultiVendor_GetSelectedMeterID_Internal;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterIDInternalFieldName, loMeterEnforcementInfo);

        //        //strcpy( loMeterEnforcementInfo, fReinoInfoForm_MultiVendor->GetSelectedBayName() );
        //        //UpdateFieldInForm( iForm, EnforcedMeterBayNoFieldName, loMeterEnforcementInfo );
        //        loMeterEnforcementInfo = lofReinoInfoForm_MultiVendor_GetSelectedBayName;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterBayNoFieldName, loMeterEnforcementInfo);

        //        //strcpy( loMeterEnforcementInfo, fReinoInfoForm_MultiVendor->GetSelectedBayID_Internal() );
        //        //UpdateFieldInForm( iForm, EnforcedMeterBayNoInternalFieldName, loMeterEnforcementInfo );
        //        loMeterEnforcementInfo = lofReinoInfoForm_MultiVendor_GetSelectedBayID_Internal;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterBayNoInternalFieldName, loMeterEnforcementInfo);

        //        //strcpy( loMeterEnforcementInfo, fReinoInfoForm_MultiVendor->GetSelectedMeterType() );
        //        //UpdateFieldInForm( iForm, EnforcedMeterTypeFieldName, loMeterEnforcementInfo );
        //        loMeterEnforcementInfo = lofReinoInfoForm_MultiVendor_GetSelectedMeterType;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterTypeFieldName, loMeterEnforcementInfo);

        //        //fReinoInfoForm_MultiVendor->GetSelectedBayScanDateTime( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterScanDateTimeFieldName, loMeterEnforcementInfo );

        //        //fReinoInfoForm_MultiVendor->GetSelectedBayRTC( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterRTCFieldName, loMeterEnforcementInfo );

        //        //sprintf(loMeterEnforcementInfo, "%s", fReinoInfoForm_MultiVendor->GetSelectedBayLUT());
        //        //UpdateFieldInForm( iForm, EnforcedMeterLUTFieldName, loMeterEnforcementInfo );
        //        loMeterEnforcementInfo = lofReinoInfoForm_MultiVendor_GetSelectedBayLUT;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterLUTFieldName, loMeterEnforcementInfo);

        //        //fReinoInfoForm_MultiVendor->GetSelectedBayMode( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterModeFieldName, loMeterEnforcementInfo);

        //        //sprintf(loMeterEnforcementInfo, "%s", fReinoInfoForm_MultiVendor->GetSelectedBayClusterID());
        //        //UpdateFieldInForm( iForm, EnforcedMeterClusterIDFieldName, loMeterEnforcementInfo );
        //        loMeterEnforcementInfo = lofReinoInfoForm_MultiVendor_GetSelectedBayClusterID;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterClusterIDFieldName, loMeterEnforcementInfo);

        //        //fReinoInfoForm_MultiVendor->GetSelectedBayClusterList( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterClusterMembersFieldName, loMeterEnforcementInfo );

        //        //sprintf(loMeterEnforcementInfo, "%s", fReinoInfoForm_MultiVendor->GetSelectedBayExpiredMinutes());
        //        //UpdateFieldInForm( iForm, EnforcedMeterBayExpiredMinutesFieldName, loMeterEnforcementInfo  );
        //        loMeterEnforcementInfo = lofReinoInfoForm_MultiVendor_GetSelectedBayExpiredMinutes;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterBayExpiredMinutesFieldName, loMeterEnforcementInfo);

        //        //fReinoInfoForm_MultiVendor->GetSelectedBayStatusCode( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterBayStatusCodeFieldName, loMeterEnforcementInfo  );

        //        //fReinoInfoForm_MultiVendor->GetSelectedBayStateCode( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterBayStateCodeFieldName, loMeterEnforcementInfo  );
        //    }
        //    else
        //    {
        //        string lofReinoInfoForm_GetSelectedBayMeterID = "";

        //        //fReinoInfoForm->GetSelectedBayMeterID( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterIDFieldName, loMeterEnforcementInfo );
        //        loMeterEnforcementInfo = lofReinoInfoForm_GetSelectedBayMeterID;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterIDInternalFieldName, loMeterEnforcementInfo);


        //        //strcpy( loMeterEnforcementInfo, fReinoInfoForm->GetSelectedBayNo() );
        //        //UpdateFieldInForm( iForm, EnforcedMeterBayNoFieldName, loMeterEnforcementInfo );
        //        loMeterEnforcementInfo = lofReinoInfoForm_GetSelectedBayNo;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterBayNoFieldName, loMeterEnforcementInfo);

        //        //fReinoInfoForm->GetSelectedBayScanDateTime( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterScanDateTimeFieldName, loMeterEnforcementInfo );
        //        loMeterEnforcementInfo = lofReinoInfoForm_GetSelectedBayScanDateTime;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterScanDateTimeFieldName, loMeterEnforcementInfo);

        //        //fReinoInfoForm->GetSelectedBayRTC( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterRTCFieldName, loMeterEnforcementInfo );
        //        loMeterEnforcementInfo = lofReinoInfoForm_GetSelectedBayRTC;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterRTCFieldName, loMeterEnforcementInfo);

        //        //fReinoInfoForm->GetSelectedBayLUT( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterLUTFieldName, loMeterEnforcementInfo );
        //        loMeterEnforcementInfo = lofReinoInfoForm_GetSelectedBayLUT;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterLUTFieldName, loMeterEnforcementInfo);

        //        //fReinoInfoForm->GetSelectedBayMode( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterModeFieldName, loMeterEnforcementInfo);
        //        loMeterEnforcementInfo = lofReinoInfoForm_GetSelectedBayMode;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterModeFieldName, loMeterEnforcementInfo);

        //        //fReinoInfoForm->GetSelectedBayClusterID( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterClusterIDFieldName, loMeterEnforcementInfo );
        //        loMeterEnforcementInfo = lofReinoInfoForm_GetSelectedBayClusterID;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterClusterIDFieldName, loMeterEnforcementInfo);

        //        //fReinoInfoForm->GetSelectedBayClusterList( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterClusterMembersFieldName, loMeterEnforcementInfo );
        //        loMeterEnforcementInfo = lofReinoInfoForm_GetSelectedBayClusterList;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterClusterMembersFieldName, loMeterEnforcementInfo);

        //        //fReinoInfoForm->GetSelectedBayExpiredMinutes( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterBayExpiredMinutesFieldName, loMeterEnforcementInfo  );
        //        loMeterEnforcementInfo = lofReinoInfoForm_GetSelectedBayExpiredMinutes;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterBayExpiredMinutesFieldName, loMeterEnforcementInfo);

        //        //fReinoInfoForm->GetSelectedBayStatusCode( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterBayStatusCodeFieldName, loMeterEnforcementInfo  );
        //        loMeterEnforcementInfo = lofReinoInfoForm_GetSelectedBayStatusCode;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterBayStatusCodeFieldName, loMeterEnforcementInfo);

        //        //fReinoInfoForm->GetSelectedBayStateCode( (char *)&loMeterEnforcementInfo, sizeof( loMeterEnforcementInfo ) );
        //        //UpdateFieldInForm( iForm, EnforcedMeterBayStateCodeFieldName, loMeterEnforcementInfo  );
        //        loMeterEnforcementInfo = lofReinoInfoForm_GetSelectedBayStateCode;
        //        UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterBayStateCodeFieldName, loMeterEnforcementInfo);
        //    }
        //    // Verrus PayByPhone support (end)


        //    //strcpy( loMeterEnforcementInfo, cnHandheldMeterEnforcementMode_GPRS );
        //    //UpdateFieldInForm( iForm, EnforcedMeterHandheldModeFieldName, loMeterEnforcementInfo );

        //    loMeterEnforcementInfo = AutoISSUE.DBConstants.cnHandheldMeterEnforcementMode_GPRS;
        //    UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterHandheldModeFieldName, loMeterEnforcementInfo);


        //    //// mcb 3.27.2008 - GPRS PAM Refresh
        //    //// Need to know if a ticket is being issued from a GPRS PAM bay selection so that the expired status can
        //    //// be confirmed when the ticket is printed.
        //    //if ( ((TObjBase *)iForm)->IsAClassMember(TIssFormNameStr))
        //    //{

        //    //    // Verrus PayByPhone support (begin)
        //    //    if (( fMeterStatusProvider == TMeterStatusProvider.mspVerrus ) || ( fMeterStatusProvider == TMeterStatusProvider.mspPango ) || ( fMeterStatusProvider == TMeterStatusProvider.mspDuncanPayByPlate ) || ( fMeterStatusProvider == TMeterStatusProvider.mspCalePayByPlate ))
        //    //    {
        //    //        strcpy( ((TIssForm *)iForm)->_GPRSPAMEnforcedBayNo, fReinoInfoForm_Verrus->GetSelectedBayNo());
        //    //    }
        //    //    else if ( fMeterStatusProvider == mspMultiVendor )
        //    //    {
        //    //        strcpy( ((TIssForm *)iForm)->_GPRSPAMEnforcedBayNo, fReinoInfoForm_MultiVendor->GetSelectedBayName());
        //    //    }
        //    //    else if ( fMeterStatusProvider == mspDuncan_OverstayVio )
        //    //    {
        //    //        strcpy( ((TIssForm *)iForm)->_GPRSPAMEnforcedBayNo, fReinoInfoForm_OverstayVio->GetSelectedBayNo());
        //    //    }
        //    //    else
        //    //    {
        //    //        strcpy( ((TIssForm *)iForm)->_GPRSPAMEnforcedBayNo, fReinoInfoForm->GetSelectedBayNo());
        //    //    }
        //    //    // Verrus PayByPhone support (end)

        //    //  strcpy( ((TIssForm *)iForm)->_GPRSPAMClusterID, _CurrentClusterID );
        //    //  ((TIssForm *)iForm)->_BayStatusAcquisitionTime = _LastClusterStatusRefreshTime;
        //    //  ( (TIssForm *)iForm)->_GPRSPAMEnforcedBayConfirmed = false;
        //    //}

        //    // record the reino_meter_id which can be different from the displayed meter description.
        //    // Verrus PayByPhone support (begin)
        //    if ((fMeterStatusProvider != TMeterStatusProvider.mspVerrus) && (fMeterStatusProvider != TMeterStatusProvider.mspPango) && (fMeterStatusProvider != TMeterStatusProvider.mspDuncanPayByPlate) && (fMeterStatusProvider != TMeterStatusProvider.mspCalePayByPlate))
        //    {
        //        if (fMeterStatusProvider == TMeterStatusProvider.mspMultiVendor)
        //        {
        //            //UpdateFieldInForm( iForm, EnforcedMeterREINO_METER_ID, fReinoInfoForm_MultiVendor->GetSelectedMeterName() );
        //            UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterREINO_METER_ID, lofReinoInfoForm_MultiVendor_GetSelectedMeterName);
        //        }
        //        else
        //        {

        //            UpdateFieldInForm(iForm, AutoISSUE.DBConstants.sqlEnforcedMeterREINO_METER_ID, loMeterID);
        //        }
        //    }
        //    // Verrus PayByPhone support (end)

        //    //EvaluateWirelessSearchResult(); // in case any hotsheet searches completed while we were displayed.


        //}

#endregion


        /// <summary>
        /// Wireless enforcement modes using Json data returned from webview pages
        /// </summary>
        public enum TWirelessEnforcementMode
        {
            wefNone = 0,
            wefPayBySpaceMap,
            wefPayBySpaceList,
            wefPayByPlateMap,
            wefPayByPlateList
        }
        public static TWirelessEnforcementMode gWirelessEnforcementMode = TWirelessEnforcementMode.wefNone;

        // TODO - maybe put together a collection of Json objects by the enum types




        //{{"BayID ": 1041840010320140000, "BayName": 1, "ExpiredTime": "20161018_T221333Z", "IsOccupied": 1, "LOCBAYID_UNIQUEKEY": "", 
        //  "METERLOC_BLOCKNUMBER": "", "METERLOC_CROSSSTREET1": "", "METERLOC_CROSSSTREET2": "", "METERLOC_ENFORCEMENTHOURSDESC": "", 
        //  "METERLOC_STREETDIRECTION": "", "METERLOC_STREETNAME": "Demo", "METERLOC_STREETTYPE": "", "MererClusterList": "", 
        //  "MeterClusterID": 414768, "MeterExpiredMinutes": 81, "MeterID": 32014, "MeterLastUpdateTime": "20161018_T221304Z", 
        //  "MeterName": "LIB14", "MeterRTC ": "", "MeterScanDateTime": "", "MeterStreet": "Demo", "MeterType": "Liberty", 
        //  "SensorEventTime": "20161018_T221333Z", "SpaceMode": "", "SpaceNo": "", "SpaceStateCode": "", "SpaceStatusCode": ""}}


        // as of 11/17/2016
        //{{"EnforcementKey": "7001-12-303-CUL08-CA", "ExpiredTime": "20161117_T233725Z", "IsOccupied": 1, "LicensePlate": "CUL08", 
        //  "METERLOC_BLOCKNUMBER": "7200", "METERLOC_CROSSSTREET1": "57", "METERLOC_CROSSSTREET2": "", "METERLOC_ENFORCEMENTHOURSDESC": "MON-FRI 08:00 AM to 06:00 PM", 
        //  "METERLOC_STREETDIRECTION": "SW", "METERLOC_STREETNAME": "SW 57 Avenue", "METERLOC_STREETTYPE": "AVE", 
        //  "Make": "Hundai", "MeterExpiredMinutes": 316, "MeterID": 12, "MeterLastUpdateTime": "20161117_T113725Z", 
        //  "MeterName": "4430", "MeterRTC ": "20161117_T182132Z", "MeterStreet": "SW 57 Avenue", "Model": "Accent", 
        // "SensorEventTime": "20161117_T113725Z", "SpaceNo": 303, "State": "CA"}}


        public const string cnGISPropertyNameEnforcementKey = "EnforcementKey";

        const string cnGISPropertyNameMeterId = "MeterId";

        const string cnGISPropertyNameMeterExpiredTime = "ExpiredTime";
        const string cnGISPropertyNameMeterName = "MeterName";
        const string cnGISPropertyNameMeterSpaceNumber = "SpaceNo";

        const string cnGISPropertyNameMeterBlock = "METERLOC_BLOCKNUMBER";
        const string cnGISPropertyNameMeterStreetDirection = "METERLOC_STREETDIRECTION";
        const string cnGISPropertyNameMeterStreet = "METERLOC_STREETNAME";
        const string cnGISPropertyNameMeterStreetType = "METERLOC_STREETTYPE";
        const string cnGISPropertyNameMeterCrossStreet1 = "METERLOC_CROSSSTREET1";
        const string cnGISPropertyNameMeterCrossStreet2 = "METERLOC_CROSSSTREET2";

        const string cnGISPropertyNameMeterExpiredMinutes = "MeterExpiredMinutes";



        //"MeterType\":\"Liberty\",\"
        const string cnGISPropertyNameMeterType = "MeterType";

        //"LastUpdated \":\"1/10/2016 12:44:55 PM
        const string cnGISPropertyNameMeterLastUpdated = "LastUpdated";

        //"BayID \":1041610010320140000,\
        const string cnGISPropertyNameBayID = "BayID";

        //"BayName\":1,\"
        const string cnGISPropertyNameBayName = "BayName";

        //"ExpiredTime\":\"1/10/2016 1:47:55 PM\",\
        const string cnGISPropertyNameExpiredTime = "ExpiredTime";
        const string cnGISPropertyNameExpiredTimeFormat = "yyyyMMdd_THHmmssZ";

        //"IsOccupied\":1
        const string cnGISPropertyNameIsOccupied = "IsOccupied";

        //"SensorEventTime\":\"\"}"
        const string cnGISPropertyNameSensorEventTime = "SensorEventTime";

        // different versions over time, have to check them all for backward compatibility

        // what we want, REV0 is checked first and will be retained if present
        const string cnGISPropertyNameVehLicNoStr_Rev0 = "VEHLICNO";
        const string cnGISPropertyNameVehLicStateStr_Rev0 = "VEHLICSTATE";
        const string cnGISPropertyNameVehMakeStr_Rev0 = "VEHMAKE";
        const string cnGISPropertyNameVehModelStr_Rev0 = "VEHMODEL";


        // what we may find
        const string cnGISPropertyNameVehLicNoStr_Rev1 = "lpn";
        const string cnGISPropertyNameVehLicStateStr_Rev1 = "state";
        const string cnGISPropertyNameVehMakeStr_Rev1 = "Make";
        const string cnGISPropertyNameVehModelStr_Rev1 = "Model";

        const string cnGISPropertyNameVehLicNoStr_Rev2 = "LicensePlate";
        const string cnGISPropertyNameVehLicStateStr_Rev2 = "State";
        const string cnGISPropertyNameVehMakeStr_Rev2 = "Make";
        const string cnGISPropertyNameVehModelStr_Rev2 = "Model";


        // TODO - build a dictionary of Json fields < - > autoissue fields




        // Json Value items to hold enforcement data returned from webviews
        private static JsonValue _JsonValuePayBySpaceMap = null;
        private static JsonObject _JsonObjectPayBySpaceMap = null;
        public static void SetJsonValueObjectPaySpaceMap(JsonValue jsonValue)
        {
            // save the value
            _JsonValuePayBySpaceMap = jsonValue;

            // convert to an object
            if (jsonValue != null)
            {
                try
                {
                    _JsonObjectPayBySpaceMap = jsonValue as JsonObject;
                }
                catch (Exception exp)
                {
                    _JsonObjectPayBySpaceMap = null;
                    Console.WriteLine("Failed to cast Json Value: " + exp.Message);
                }
            }
            else
            {
                _JsonObjectPayBySpaceMap = null;
            }

        }



        private static JsonValue _JsonValuePayBySpaceList = null;
        private static JsonObject _JsonObjectPayBySpaceList = null;
        public static void SetJsonValueObjectPaySpaceList(JsonValue jsonValue)
        {
            _JsonValuePayBySpaceList = jsonValue;

            // convert to an object
            if (jsonValue != null)
            {
                try
                {
                    _JsonObjectPayBySpaceList = jsonValue as JsonObject;
                }
                catch (Exception exp)
                {
                    _JsonObjectPayBySpaceList = null;
                    Console.WriteLine("Failed to cast _JsonObjectPayBySpaceList Value: " + exp.Message);
                }
            }
            else
            {
                _JsonObjectPayBySpaceList = null;
            }

        }


        private static JsonValue _JsonValuePayByPlateMap = null;
        private static JsonObject _JsonObjectPayByPlateMap = null;
        public static void SetJsonValueObjectPayByPlateMap(JsonValue jsonValue)
        {
            _JsonValuePayByPlateMap = jsonValue;

            // convert to an object
            if (jsonValue != null)
            {
                try
                {
                    _JsonObjectPayByPlateMap = jsonValue as JsonObject;
                }
                catch (Exception exp)
                {
                    _JsonObjectPayByPlateMap = null;
                    Console.WriteLine("Failed to cast _JsonObjectPayByPlateMap Value: " + exp.Message);
                }
            }
            else
            {
                _JsonObjectPayByPlateMap = null;
            }

        }

        
        private static JsonValue _JsonValuePayByPlateList = null;
        private static JsonObject _JsonObjectPayByPlateList = null;
        public static void SetJsonValueObjectPayByPlateList(JsonValue jsonValue)
        {
            _JsonValuePayByPlateList = jsonValue;

            // convert to an object
            if (jsonValue != null)
            {
                try
                {
                    _JsonObjectPayByPlateList = jsonValue as JsonObject;
                }
                catch (Exception exp)
                {
                    _JsonObjectPayByPlateList = null;
                    Console.WriteLine("Failed to cast _JsonObjectPayByPlateList Value: " + exp.Message);
                }
            }
            else
            {
                _JsonObjectPayByPlateList = null;
            }

        }


///
/// 
        public static bool JsonValueObjectIsNullOrEmpty(JsonValue jsonValue)
        {
            JsonObject _JsonObjectTester = null;

            // convert to an object
            if (jsonValue != null)
            {
                try
                {
                    _JsonObjectTester = jsonValue as JsonObject;
                }
                catch (Exception exp)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return ((_JsonObjectTester == null) || (_JsonObjectTester.Count == 0));
        }



        /// <summary>
        /// Set the wireless enforcement mode and update the Json object containing the enforcement data
        /// </summary>
        /// <param name="iMode"></param>
        /// <param name="iJsonValueReturnedFromEnforcementService"></param>
        public static void SetWirelessEnforcementMode(TWirelessEnforcementMode iMode, JsonValue iJsonValueReturnedFromEnforcementService)
        {

            // a little kludgy... should be consistenltly passing known value
            bool loNoEnforcementDataPassed = JsonValueObjectIsNullOrEmpty(iJsonValueReturnedFromEnforcementService);

            // override - the web service 
            if (loNoEnforcementDataPassed == true)
            {
                iMode = TWirelessEnforcementMode.wefNone;
            }


            // set the mode 
            gWirelessEnforcementMode = iMode;


            // TODO - maybe put together a collection of Json objects by the enum types


            // update the associated object
            switch (gWirelessEnforcementMode)
            {
                case TWirelessEnforcementMode.wefPayBySpaceMap:
                    {
                        SetJsonValueObjectPaySpaceMap(iJsonValueReturnedFromEnforcementService);
                        break;
                    }

                case TWirelessEnforcementMode.wefPayBySpaceList:
                    {
                        SetJsonValueObjectPaySpaceList(iJsonValueReturnedFromEnforcementService);
                        break;
                    }

                case TWirelessEnforcementMode.wefPayByPlateMap:
                    {
                        SetJsonValueObjectPayByPlateMap(iJsonValueReturnedFromEnforcementService);
                        break;
                    }

                case TWirelessEnforcementMode.wefPayByPlateList:
                    {
                        SetJsonValueObjectPayByPlateList(iJsonValueReturnedFromEnforcementService);
                        break;
                    }


                default:
                    {
                        // do nothing
                        break;
                    }
            }


        }

        /// <summary>
        /// Extract various wireless enforcement options from the TTRegistry.registry and update the global references
        /// </summary>
        public static void EvaluateWirelessEnforcementOptions()
        {


            // check for the legacy parameter that determined Reino PAM vs DPT system
            bool loLegacyStallRangeMode = (TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regGPRS_METER_STALL_RANGE_MODE, TTRegistry.regGPRS_METER_STALL_RANGE_MODE_DEFAULT) == 1);
            if (loLegacyStallRangeMode == true)
            {
                // DPT client
                WirelessEnforcementOptions.fMeterCustomerID = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regDPT_CUSTID, TTRegistry.regDPT_CUSTID_DEFAULT);
                // force DPT
                WirelessEnforcementOptions.fMeterStatusProvider = WirelessEnforcementOptions.TMeterStatusProvider.mspDPT;
                WirelessEnforcementOptions.fMeterStatusProviderName = TTRegistry.regGPRS_METER_PROVIDER_NAME_DPT;
            }
            else
            {
                // not legacy stall mode, check for the newer parameters
                WirelessEnforcementOptions.fMeterStatusProviderName = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regGPRS_METER_PROVIDER_NAME, TTRegistry.regGPRS_METER_PROVIDER_NAME_DEFAULT).ToUpper();

                // see if there is a device-specific override to use (Introduced for Seattle demo)
                //if ( strlen( glMeterProviderNameOverride ) != 0 )
                //{
                //    strcpy( fMeterStatusProviderName, glMeterProviderNameOverride);
                //}


                // extract and translate to enum
                switch (WirelessEnforcementOptions.fMeterStatusProviderName)
                {

                    case TTRegistry.regGPRS_METER_PROVIDER_NAME_DPT:
                        {
                            // DPT client
                            WirelessEnforcementOptions.fMeterStatusProvider = WirelessEnforcementOptions.TMeterStatusProvider.mspDPT;
                            WirelessEnforcementOptions.fMeterCustomerID = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regDPT_CUSTID, TTRegistry.regDPT_CUSTID_DEFAULT);
                            break;
                        }

                    case TTRegistry.regGPRS_METER_PROVIDER_NAME_PARKEON:
                        {
                            // Parkeon client
                            WirelessEnforcementOptions.fMeterStatusProvider = WirelessEnforcementOptions.TMeterStatusProvider.mspParkeon;
                            WirelessEnforcementOptions.fMeterCustomerID = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regPARKEON_CUSTID, TTRegistry.regPARKEON_CUSTID_DEFAULT);
                            break;
                        }

                    case TTRegistry.regGPRS_METER_PROVIDER_NAME_PARKNOW:
                        {
                            // ParkNOW client
                            WirelessEnforcementOptions.fMeterStatusProvider = WirelessEnforcementOptions.TMeterStatusProvider.mspParkNOW;
                            WirelessEnforcementOptions.fMeterCustomerID = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regPARKNOW_CUSTID, TTRegistry.regPARKNOW_CUSTID_DEFAULT);
                            break;
                        }

                    case TTRegistry.regGPRS_METER_PROVIDER_NAME_MULTIVENDOR:
                        {
                            // MultiVendor client
                            WirelessEnforcementOptions.fMeterStatusProvider = WirelessEnforcementOptions.TMeterStatusProvider.mspMultiVendor;
                            WirelessEnforcementOptions.fMeterCustomerID = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regREINO_CUSTID, TTRegistry.regREINO_CUSTID_DEFAULT);
                            break;
                        }


                    case TTRegistry.regGPRS_METER_PROVIDER_NAME_VERRUS:
                        {
                            // Verrus client
                            WirelessEnforcementOptions.fMeterStatusProvider = WirelessEnforcementOptions.TMeterStatusProvider.mspVerrus;
                            break;
                        }

                    case TTRegistry.regGPRS_METER_PROVIDER_NAME_DUNCAN_PAYBYPLATE:
                        {
                            // Verrus client
                            WirelessEnforcementOptions.fMeterStatusProvider = WirelessEnforcementOptions.TMeterStatusProvider.mspDuncanPayByPlate;
                            break;
                        }

                    case TTRegistry.regGPRS_METER_PROVIDER_NAME_PANGO:
                        {
                            // Pango client
                            WirelessEnforcementOptions.fMeterStatusProvider = WirelessEnforcementOptions.TMeterStatusProvider.mspPango;
                            break;
                        }

                    case TTRegistry.regGPRS_METER_PROVIDER_NAME_CALE_PAYBYPLATE:
                        {
                            // Cale PayByPlate client
                            WirelessEnforcementOptions.fMeterStatusProvider = WirelessEnforcementOptions.TMeterStatusProvider.mspCalePayByPlate;
                            break;
                        }

                    default:
                        {
                            // Reino client is the deafult
                            WirelessEnforcementOptions.fMeterStatusProvider = WirelessEnforcementOptions.TMeterStatusProvider.mspDuncan;
                            WirelessEnforcementOptions.fMeterCustomerID = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regREINO_CUSTID, TTRegistry.regREINO_CUSTID_DEFAULT);
                            break;
                        }
                }


                // extract and translate to enum
                string loMeterWarningLevelDetail = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regGPRS_METER_WARNING_LEVEL_DETAIL, TTRegistry.regGPRS_METER_WARNING_LEVEL_DETAIL_DEFAULT);
                switch (loMeterWarningLevelDetail)
                {
                    case TTRegistry.regGPRS_METER_WARNING_LEVEL_DETAIL_METER_COUNTS:
                        {
                            WirelessEnforcementOptions.fMeterWarningLevelDetail = WirelessEnforcementOptions.TMeterWarningLevelDetail.wldMeterCounts;
                            break;
                        }

                    case TTRegistry.regGPRS_METER_WARNING_LEVEL_DETAIL_METER_NAMES:
                        {
                            WirelessEnforcementOptions.fMeterWarningLevelDetail = WirelessEnforcementOptions.TMeterWarningLevelDetail.wldMeterNames;
                            break;
                        }

                    case TTRegistry.regGPRS_METER_WARNING_LEVEL_DETAIL_METER_COUNTS_AND_NAMES:
                        {
                            WirelessEnforcementOptions.fMeterWarningLevelDetail = WirelessEnforcementOptions.TMeterWarningLevelDetail.wldMeterCountsAndNames;
                            break;
                        }

                    default:
                        {
                            // none or unknown is the default
                            WirelessEnforcementOptions.fMeterWarningLevelDetail = WirelessEnforcementOptions.TMeterWarningLevelDetail.wldNone;
                            break;
                        }

                }

            }  // end of not legacy stall mode


            // android web view pages should be shown?


            // PayBySpace web views
            string loOptionEnabledPBSMap = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regPAYBYSPACE_WEBVIEW_MAP_ENABLED, TTRegistry.regPAYBYSPACE_WEBVIEW_MAP_ENABLED_DEFAULT);
            WirelessEnforcementOptions.fPayBySpaceMapEnforcementActive = loOptionEnabledPBSMap.Equals(TTRegistry.regOPTION_ENABLED);

            string loOptionEnabledPBSList = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regPAYBYSPACE_WEBVIEW_LIST_ENABLED, TTRegistry.regPAYBYSPACE_WEBVIEW_LIST_ENABLED_DEFAULT);
            WirelessEnforcementOptions.fPayBySpaceListEnforcementActive = loOptionEnabledPBSList.Equals(TTRegistry.regOPTION_ENABLED);


            // PayByPlate web views
            string loOptionEnabledPBPMap = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regPAYBYPLATE_WEBVIEW_MAP_ENABLED, TTRegistry.regPAYBYPLATE_WEBVIEW_MAP_ENABLED_DEFAULT);
            WirelessEnforcementOptions.fPayByPlateMapEnforcementActive = loOptionEnabledPBPMap.Equals(TTRegistry.regOPTION_ENABLED);

            string loOptionEnabledPBPList = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regPAYBYPLATE_WEBVIEW_LIST_ENABLED, TTRegistry.regPAYBYPLATE_WEBVIEW_LIST_ENABLED_DEFAULT);
            WirelessEnforcementOptions.fPayByPlateListEnforcementActive = loOptionEnabledPBPList.Equals(TTRegistry.regOPTION_ENABLED);


            // override to disable until determined to remove them completely
            //WirelessEnforcementOptions.fPayBySpaceListEnforcementActive = false;
            //WirelessEnforcementOptions.fPayByPlateListEnforcementActive = false;



            // how many minutes of payment age will we display? 
            WirelessEnforcementOptions.fMaxPaymentAgeMinutes = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regGPRS_METER_MAX_PAYMENT_AGE_MINUTES, TTRegistry.regGPRS_METER_MAX_PAYMENT_AGE_MINUTES_DEFAULT);


            // how long before we show "No Recent Contact with Server" ?
            WirelessEnforcementOptions.fMaxServerResponseTimeoutSeconds = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regGPRS_METER_SERVER_RESPONSE_TIMEOUT_SECONDS, TTRegistry.regGPRS_METER_SERVER_RESPONSE_TIMEOUT_SECONDS_DEFAULT);



            // extract the literal string to display when when max the payment age is exceeded
            WirelessEnforcementOptions.fMaxPaymentAgeDisplayText = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regGPRS_METER_MAX_PAYMENT_AGE_DISPLAY_TEXT, TTRegistry.regGPRS_METER_MAX_PAYMENT_AGE_DISPLAY_TEXT_DEFAULT);

            // Pay-by-Cell enforcement options
            WirelessEnforcementOptions.fPBC_SearchZoneID_MappedField = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regPBC_SEARCHZONEID_MAPPEDFIELD, TTRegistry.regPBC_SEARCHZONEID_MAPPEDFIELD_DEFAULT);
            WirelessEnforcementOptions.fPBC_SearchZoneDesc_MappedField = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regPBC_SEARCHZONEDESC_MAPPEDFIELD, TTRegistry.regPBC_SEARCHZONEDESC_MAPPEDFIELD_DEFAULT);



            WirelessEnforcementOptions.fPBCRefreshIntervalSec = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regPBC_GPRS_REFRESH_INTERVAL_SEC, TTRegistry.regPBC_GPRS_REFRESH_INTERVAL_SEC_DEFAULT);

            // AJW - TODO - review the handling of this data type
            WirelessEnforcementOptions.fPBCMaxDataAge100ns = TTRegistry.glRegistry.GetRegistryValueAsUInt64(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regPBC_GPRS_MAX_AGE_SEC, TTRegistry.regPBC_GPRS_MAX_AGE_SEC_DEFAULT);
            //WirelessEnforcementOptions.fPBCMaxDataAge100ns.QuadPart *= FileTimeTicksPerSec;

            WirelessEnforcementOptions.fPBCMaxConfirmationDataAgeSec = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regPBC_GPRS_CONFIRMATION_SEC, TTRegistry.regPBC_GPRS_CONFIRMATION_SEC_DEFAULT);

            //// Special requirement for Ottawa.  This is truely bad coding, but time is of the essence...
            //fMustConfirmBeforePrint = false;
            //if ( strcmp( glIssueAp->fClientName, "Ottawa" ) == 0 )
            //    fMustConfirmBeforePrint = true;


        }



        /// <summary>
        /// Reset after each ticket submission
        /// </summary>
        public static void ClearGISMeterJsonValueObject()
        {
            gWirelessEnforcementMode = TWirelessEnforcementMode.wefNone;
            // DEMO KLUDGE lets keep it 
            //_gisMeterJsonValue = null;
        }



        /// <summary>
        /// A safe-wrapper call to return those values as string - allows other types return " and don't cause exceptions
        /// This is the key comparison case insensitve version
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        /// 

        private static string GetJsonPropertyValueAsStringIgnoreCase(JsonObject iJsonObject, string targetKeyName)
        {
            if (iJsonObject != null)
            {
                foreach (string oneKeyName in iJsonObject.Keys)
                {
                    if (oneKeyName.Equals(targetKeyName, StringComparison.InvariantCultureIgnoreCase) == true)
                    {
                        switch (iJsonObject[oneKeyName].JsonType)
                        {
                            case (JsonType.Array):
                                {
                                    //return iJsonObject[keyName].ToString();
                                    break;
                                }
                            case JsonType.Boolean:
                                {
                                    //return iJsonObject[keyName].ToString();
                                    //if ( *(JsonType.Boolean)iJsonObject[keyName]) == JsonType.Boolean True
                                    break;
                                }
                            case JsonType.Number:
                                {
                                    return iJsonObject[oneKeyName].ToString();
                                }
                            case JsonType.Object:
                                {
                                    //return iJsonObject[keyName].ToString();
                                    break;
                                }
                            case JsonType.String:
                                {
                                    return iJsonObject[oneKeyName];
                                }
                            default:
                                {
                                    break;
                                }
                        }


                    }
                }
            }

            return string.Empty;
        }


/*
        /// <summary>
        /// A safe-wrapper call to return those values as string allows, other types return " and don't cause exceptions
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        private static string GetJsonPropertyValueAsString(JsonValue iJsonValue, string keyName)
        {
            // TODO - move this so the JSON casting is done once when assigned
            if (iJsonValue != null)
            {
                try
                {
                    JsonObject loJsonObject = iJsonValue as JsonObject;
                    return GetJsonPropertyValueAsStringIgnoreCase(loJsonObject, keyName);
                }
                catch ( Exception exp )
                {
                    Console.WriteLine("Failed to cast Json Value: " + exp.Message);
                }
            }


            if (iJsonValue != null && iJsonValue.ContainsKey(keyName))
            {
                switch (iJsonValue[keyName].JsonType)
                {
                    case (JsonType.Array):
                        {
                            //return iJsonObject[keyName].ToString();
                            break;
                        }
                    case JsonType.Boolean:
                        {
                            //return iJsonObject[keyName].ToString();
                            //if ( *(JsonType.Boolean)iJsonObject[keyName]) == JsonType.Boolean True
                            break;
                        }
                    case JsonType.Number:
                        {
                            return iJsonValue[keyName].ToString();
                        }
                    case JsonType.Object:
                        {
                            //return iJsonObject[keyName].ToString();
                            break;
                        }
                    case JsonType.String:
                        {
                            return iJsonValue[keyName];
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            return string.Empty;
        }

        */


        /// <summary>
        /// A safe-wrapper call to return those values as string allows, other types return " and don't cause exceptions
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string GetWirelessEnforcementPropertyValueAsString(string keyName)
        {
            // pull the data from the Json object returned from the enforcement web page
            switch ( gWirelessEnforcementMode )
            {

                case TWirelessEnforcementMode.wefPayBySpaceMap :
                    {
                        //return GetJsonPropertyValueAsString( _JsonValuePayBySpaceMap, keyName );
                        return GetJsonPropertyValueAsStringIgnoreCase( _JsonObjectPayBySpaceMap, keyName);
                    }

                case TWirelessEnforcementMode.wefPayBySpaceList:
                    {
                        //return GetJsonPropertyValueAsString( _JsonValuePayBySpaceList, keyName );
                        return GetJsonPropertyValueAsStringIgnoreCase( _JsonObjectPayBySpaceList, keyName);
                    }

                case TWirelessEnforcementMode.wefPayByPlateMap:
                    { 
                        //return GetJsonPropertyValueAsString( _JsonValuePayByPlateMap, keyName );
                        return GetJsonPropertyValueAsStringIgnoreCase(_JsonObjectPayByPlateMap, keyName);
                    }

                case TWirelessEnforcementMode.wefPayByPlateList:
                    {
                        //return GetJsonPropertyValueAsString( _JsonValuePayByPlateList, keyName );
                        return GetJsonPropertyValueAsStringIgnoreCase(_JsonObjectPayByPlateList, keyName);
                    }


                case TWirelessEnforcementMode.wefNone:
                default :
                    {
                        return string.Empty;
                    }
            }
        }




        private static int _LastGISParentIndex = -1;
        private static string _LastGISParentValue = string.Empty;

        private static void SetListItemForParentIndex( string iParentItem )
        {
            _LastGISParentValue = iParentItem;
        }

        private static string GetAssociatedChildListRowData(EditControlBehavior iParentControl, string columnToReturn)
        {
            if (iParentControl.PanelField.OptionsList != null && !string.IsNullOrEmpty(iParentControl.PanelField.OptionsList.saveColumn))
            {
                //get the parent controls value 
                var parentValue = _LastGISParentValue;
                //also, set some parent control table name and column name stuff here
                var parentTableName = iParentControl.PanelField.OptionsList.ListName;
                var parentColumnName = iParentControl.PanelField.OptionsList.saveColumn;
                //update the lsit item text and value here if needed
                // go get the value from the DB
                var value = (new ListSupport()).GetDataFromTableWithColumnValue(columnToReturn, parentTableName, parentColumnName, parentValue);
                return value;
            }
            return string.Empty;
        }


        private static EditControlBehavior GetUIComponentBehavior(View uiComponent)
        {
            EditControlBehavior uiComponentBehavior = null;

            // extract and deference the behavior component - rolled into a function
            if (uiComponent != null)
            {
                if (uiComponent is CustomEditText)
                {
                    uiComponentBehavior = ((CustomEditText)uiComponent).BehaviorAndroid;
                }
                else if (uiComponent is CustomAutoTextView)
                {
                    uiComponentBehavior = ((CustomAutoTextView)uiComponent).BehaviorAndroid;
                }
                else if (uiComponent is CustomSpinner)
                {
                    uiComponentBehavior = ((CustomSpinner)uiComponent).BehaviorAndroid;
                }
                //else if (uiComponent is SignaturePadView)
                //{
                //    // uiComponentBehavior = ((SignaturePadView)uiComponent).Behavior;
                //}
            }

            return uiComponentBehavior;

        }


        private static EditRestriction GetChildListEditRestriction(View uiComponent, PanelField iPanelField)
        {
            // find the TER_ChildList restriction (if defined) for passed control
            EditControlBehavior uiComponentBehavior = null;

            // extract and deference the behavior component
            if (uiComponent != null)
            {
                uiComponentBehavior = GetUIComponentBehavior(uiComponent);

                if (uiComponentBehavior != null)
                {
                    // Loop through each edit restriction
                    int loLoopMax = uiComponentBehavior.EditRestrictions.Count;
                    for (int loNdx = 0; loNdx < loLoopMax; loNdx++)
                    {
                        Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestriction loRestrict = uiComponentBehavior.EditRestrictions[loNdx];
                        if (loRestrict is Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_ChildList)
                        {
                            return loRestrict;
                        }
                    }
                }
            }

            return null;
        }

        private static string GetChildListItemForParentIndex(View uiComponent, PanelField iPanelField)
        {
            string loResult = string.Empty;

            Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestriction loChildListRestriction = GetChildListEditRestriction(uiComponent, iPanelField);

            if (loChildListRestriction != null)
            {
                loResult = GetAssociatedChildListRowData(loChildListRestriction.ControlEdit1Obj, loChildListRestriction.CharParam);
            }

            return loResult;
        }



        private static bool UIComponentHasBeenProcessed(View uiComponent)
        {
            if (uiComponent != null)
            {
                if (uiComponent is CustomEditText)
                {
                    return (((CustomEditText)uiComponent).FormStatus != null);
                }
                else if (uiComponent is CustomAutoTextView)
                {
                    return (((CustomAutoTextView)uiComponent).FormStatus != null);
                }
                else if (uiComponent is CustomSpinner)
                {
                    return (((CustomSpinner)uiComponent).FormStatus != null);
                }

            }

            return false;
        }



        /// <summary>
        /// Specialize pull against multiple field names
        /// </summary>
        /// <returns></returns>
        private static string GetLicensePlateValueFromJSON()
        {
            // try version 0 
            string loVehLicNoStr = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameVehLicNoStr_Rev0);


            // next version if needed
            if (string.IsNullOrEmpty(loVehLicNoStr) == true)
            {
                loVehLicNoStr = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameVehLicNoStr_Rev1);
            }

            // next version if needed
            if (string.IsNullOrEmpty(loVehLicNoStr) == true)
            {
                loVehLicNoStr = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameVehLicNoStr_Rev2);
            }


            return loVehLicNoStr;
        }

        /// <summary>
        /// Specialize pull against multiple field names
        /// </summary>
        /// <returns></returns>
        private static string GetLicenseStateValueFromJSON()
        {
            // try version 0 
            string loVehLicStateStr = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameVehLicStateStr_Rev0);


            // next version if needed
            if (string.IsNullOrEmpty(loVehLicStateStr) == true)
            {
                loVehLicStateStr = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameVehLicStateStr_Rev1);
            }

            // next version if needed
            if (string.IsNullOrEmpty(loVehLicStateStr) == true)
            {
                loVehLicStateStr = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameVehLicStateStr_Rev2);
            }


            return loVehLicStateStr;
        }


        /// <summary>
        /// Specialize pull against multiple field names
        /// </summary>
        /// <returns></returns>
        private static string GetVehicleMakeValueFromJSON()
        {
            // try version 0 
            string loVehMakeStr = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameVehMakeStr_Rev0);


            // next version if needed
            if (string.IsNullOrEmpty(loVehMakeStr) == true)
            {
                loVehMakeStr = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameVehMakeStr_Rev1);
            }

            // next version if needed
            if (string.IsNullOrEmpty(loVehMakeStr) == true)
            {
                loVehMakeStr = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameVehMakeStr_Rev2);
            }


            return loVehMakeStr;
        }


        /// <summary>
        /// Get current external enforcement values 
        /// </summary>
        /// <param name="iFieldName"></param>
        /// <returns></returns>
        public static bool GetGISEnforcementValue(IssStruct iIssStruct,  View uiComponent, PanelField iPanelField, ref string ioGISValue)
        {
            bool loGISValuePopulated = false;

            string iFieldName = iPanelField.Name;



            try
            {
                if (uiComponent == null)
                {
                    return false;
                }

                // only once per form
                if (UIComponentHasBeenProcessed(uiComponent) == true)
                {
                    return false;
                }


                // only when the mode indicates it
                if (gWirelessEnforcementMode == TWirelessEnforcementMode.wefNone)
                {
                    return loGISValuePopulated;
                }

               // // populate here from the JSON object
               //System.Json.JsonValue loCurrentGISValue = DroidContext.XmlCfg.GettJsonValueObjectPaySpaceMap();

               // if (loCurrentGISValue == null)
               // {
               //     return loGISValuePopulated;
               // }


                iFieldName = iFieldName.ToUpper();

                switch (iFieldName)
                {
                    //case AutoISSUE.DBConstants.sqlLocMeterNumberStr:
                    //case AutoISSUE.DBConstants.sqlLocationMeterNumberStr:
                    ////case AutoISSUE.DBConstants.sqlMeterNumberStr:
                    //case AutoISSUE.DBConstants.sqlEnforcedMeterBayNoFieldName:



                    case AutoISSUE.DBConstants.sqlPBC_ENF_ZONEIDFieldName:
                        {
                            break;
                        }


                    case AutoISSUE.DBConstants.sqlVehLicNoStr:
                    case AutoISSUE.DBConstants.sqlPBC_ENF_LICPLATEFieldName:
                        {
                            // try - these are not always valid values
                            ioGISValue = GetLicensePlateValueFromJSON();
                            loGISValuePopulated = (string.IsNullOrEmpty( ioGISValue ) == false );
                            break;
                        }

                    case AutoISSUE.DBConstants.sqlVehLicStateStr:
                    case AutoISSUE.DBConstants.sqlPBC_ENF_LICSTATEFieldName:
                        {
                            // special case - only usable when licplate is also present
                            if (string.IsNullOrEmpty( GetLicensePlateValueFromJSON() ) == false)
                            {
                                // try - these are not always valid values
                                ioGISValue = GetLicenseStateValueFromJSON();
                                loGISValuePopulated = (string.IsNullOrEmpty(ioGISValue) == false);

                            }
                            break;
                        }

                    case AutoISSUE.DBConstants.sqlVehMakeStr:
                        {
                            // special case - only usable when licplate is also present
                            if (string.IsNullOrEmpty(GetLicensePlateValueFromJSON()) == false)
                            {
                                // try - these are not always valid values
                                ioGISValue = GetVehicleMakeValueFromJSON();
                                loGISValuePopulated = (string.IsNullOrEmpty(ioGISValue) == false);

                            }
                            break;
                        }




                        // this is the meter number used on the location lookup 
                    case AutoISSUE.DBConstants.sqlLocMeterNumberStr:
                        {
                            string loEnfValue = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameMeterName);


                            // TODO - only if this matches existing list entry, set it?

                            //ioGISValue = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameMeterId);
                            // // meter no is the master/parent field for meetr location childen
                            //SetListItemForParentIndex( ioGISValue );

                            ioGISValue = loEnfValue;
                            loGISValuePopulated = true;
                            break;
                        }


                        // this is the meter number captured as part of the violation record
                    case AutoISSUE.DBConstants.sqlMeterNumberStr:  // populating this causes edit restriction issue with VEH LIC NO ???
                        {
                            string loEnfValue = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameMeterName);

                            ioGISValue = loEnfValue;
                            loGISValuePopulated = true;
                            break;
                        }



                    //case AutoISSUE.DBConstants.sqlEnforcedMeterBayExpiredTimeFieldName:
                    //    {
                    //        string loEnfValue = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameMeterExpiredTime);

                    //        ioGISValue = loEnfValue;
                    //        loGISValuePopulated = true;
                    //        break;
                    //    }

                    case AutoISSUE.DBConstants.sqlEnforcedMeterBayNoFieldName:
                    case AutoISSUE.DBConstants.sqlEnforcedMeterBayNoInternalFieldName:
                        {
                            string loEnfValue = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameMeterSpaceNumber);

                            ioGISValue = loEnfValue;
                            loGISValuePopulated = true;
                            break;
                        }

                    case AutoISSUE.DBConstants.sqlEnforcedMeterBayExpiredMinutesFieldName:
                        {
                            string loEnfValue = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameMeterExpiredMinutes);

                            ioGISValue = loEnfValue;
                            loGISValuePopulated = true;
                            break;
                        }





/*
        public const string sqlEnforcedMeterIDFieldName = "ENF_METERID";
        public const string sqlEnforcedMeterIDInternalFieldName = "ENF_METERID_INTERNAL"; // Used for multivendor (ENF_METERID is the name, and ENF_METERID_INTERNAL is the real ID)
        public const string sqlEnforcedMeterBayNoFieldName = "ENF_METERBAYNO";
        public const string sqlEnforcedMeterBayNoInternalFieldName = "ENF_METERBAYNO_INTERNAL"; // Used for multivendor (ENF_METERBAYNO is the name, and ENF_METERBAYNO_INTERNAL is the real ID)
        public const string sqlEnforcedMeterTypeFieldName = "ENF_METERTYPE"; // Used for multivendor mode
        public const string sqlEnforcedMeterScanDateTimeFieldName = "ENF_METERSCANDATETIME";
        public const string sqlEnforcedMeterRTCFieldName = "ENF_METERRTCDATETIME";
        public const string sqlEnforcedMeterLUTFieldName = "ENF_METERLUTDATETIME";
        public const string sqlEnforcedMeterModeFieldName = "ENF_METERMODE";
        public const string sqlEnforcedMeterClusterIDFieldName = "ENF_METERCLUSTERID";
        public const string sqlEnforcedMeterClusterMembersFieldName = "ENF_METERCLUSTERMEMBERS";
        public const string sqlEnforcedMeterBayExpiredMinutesFieldName = "ENF_BAYEXPIREDMINUTES";
        public const string sqlEnforcedMeterBayStatusCodeFieldName = "ENF_BAYSTATUSCODE";
        public const string sqlEnforcedMeterBayStateCodeFieldName = "ENF_BAYSTATECODE";
        public const string sqlEnforcedMeterHandheldModeFieldName = "ENF_HANDHELDMODE";


        // enforcement info captured from PayByCell enforcement, such as Verrus
        public const string sqlPBC_ENF_ZONEIDFieldName = "PBC_ENF_ZONEID";
        public const string sqlPBC_ENF_LICPLATEFieldName = "PBC_ENF_LICPLATE";
        public const string sqlPBC_ENF_LICSTATEFieldName = "PBC_ENF_LICSTATE";
        public const string sqlPBC_ENF_REASONFieldName = "PBC_ENF_REASON";
        public const string sqlPBC_DATA_AGEFieldName = "PBC_DATA_AGE";
        public const string sqlPBC_CONFIRMATION_CODEFieldName = "PBC_CONFIRMATION_CODE";


        // bay status data age (in seconds) relative to the ticket print time. 
        // a negative number indicates the data is from before the print.
        public const string sqlEnforcedMeterREINO_DATA_AGE = "REINO_DATA_AGE";

        // EXPIRED, CONFIRMED, UNCONFIRMED, NOTEXPIRED, METEROOD, UNKNOWN 
        public const string sqlEnforcedMeterREINO_CONFIRMATION_CODE = "REINO_CONFIRMATION_CODE";

        // reino_meter_id which can be different from the displayed meter description.
        public const string sqlEnforcedMeterREINO_METER_ID = "REINO_METER_ID";
*/





                    case AutoISSUE.DBConstants.sqlLocBlockStr:
                        {
                            string loEnfValue = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameMeterBlock);

                            //string loEnfValue = GetChildListItemForParentIndex(uiComponent, iPanelField);


                            ioGISValue = loEnfValue;
                            loGISValuePopulated = true;
                            break;
                        }

                    case AutoISSUE.DBConstants.sqlLocDirectionStr:
                        {
                            string loEnfValue = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameMeterStreetDirection);

                            //string loEnfValue = GetChildListItemForParentIndex(uiComponent, iPanelField);


                            ioGISValue = loEnfValue;
                            loGISValuePopulated = true;
                            break;
                        }


                    case AutoISSUE.DBConstants.sqlLocStreetStr:
                        {
                            string loEnfValue = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameMeterStreet);

                           //string loEnfValue = GetChildListItemForParentIndex(uiComponent, iPanelField);


                            ioGISValue = loEnfValue;
                            loGISValuePopulated = true;
                            break;
                        }


                    case AutoISSUE.DBConstants.sqlLocDescriptorStr:
                    //case AutoISSUE.DBConstants.sqlLocStr
                        {
                            string loEnfValue = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameMeterStreetType);

                            //string loEnfValue = GetChildListItemForParentIndex(uiComponent, iPanelField);

                            ioGISValue = loEnfValue;
                            loGISValuePopulated = true;
                            break;
                        }


                    case AutoISSUE.DBConstants.sqlLocCrossStreet1Str:
                        {
                            string loEnfValue = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameMeterCrossStreet1);

                            //string loEnfValue = GetChildListItemForParentIndex(uiComponent, iPanelField);

                            ioGISValue = loEnfValue;
                            loGISValuePopulated = true;
                            break;
                        }


                    case AutoISSUE.DBConstants.sqlLocCrossStreet2Str:
                        {
                            string loEnfValue = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameMeterCrossStreet2);

                            //string loEnfValue = GetChildListItemForParentIndex(uiComponent, iPanelField);

                            ioGISValue = loEnfValue;
                            loGISValuePopulated = true;
                            break;
                        }

                    /*
                                        //case AutoISSUE.DBConstants.sqlLocDescriptorStr: // LOCSTREETTYPE
                                        case AutoISSUE.DBConstants.sqlLocStreetTypeStr:
                                            {
                                                //string loEnfValue = DroidContext.XmlCfg.GetGISMeterPropertyValueAsString(cnGISPropertyNameMeterStreet);
                                                string loEnfValue = "";  // make it blank
                                                ioGISValue = loEnfValue;
                                                loGISValuePopulated = true;
                                                break;
                                            }
                    */

                    case AutoISSUE.DBConstants.sqlIssueOfficerNameStr:
                        {
                            //ioGISValue = "GIS MAN";
                            //loGISValuePopulated = true;
                            break;
                        }


                    //case AutoISSUE.DBConstants.sqlEnforcedMeterBayNoFieldName:
                    //    {
                    //        break;
                    //    }


                    case AutoISSUE.DBConstants.sqlRemark1Str:
                        {
                            StringBuilder loRemarkField =new StringBuilder();
                            string loEnfValue = null;


                            //loEnfValue = DroidContext.XmlCfg.GetGISMeterPropertyString(cnGISPropertyNameMeterName);
                            //if ( string.IsNullOrEmpty(loEnfValue) == false )
                            //{
                            //    loRemarkField.Append( "METER " + loEnfValue );
                            //}

                            //loEnfValue = DroidContext.XmlCfg.GetJsonPayBySpaceMapPropertyValueAsString(cnGISPropertyNameExpiredTime);

                            loEnfValue = ExternalEnforcementInterfaces.GetWirelessEnforcementPropertyValueAsString(cnGISPropertyNameExpiredTime);


                            if ( string.IsNullOrEmpty(loEnfValue) == false )
                            {
                                // let's see if we can format this in the defined format of the structure
                                if (iIssStruct != null)
                                {
                                    if (iIssStruct.fDisplayFormattingInfo != null)
                                    {
                                        if (string.IsNullOrEmpty(iIssStruct.fDisplayFormattingInfo.fStructDateMask) == false)
                                        {
                                            loEnfValue = CultureDisplayFormatLogic.ConvertDateStringToDisplayFormatLocalTime(
                                                loEnfValue,
                                                cnGISPropertyNameExpiredTimeFormat,
                                                iIssStruct.fDisplayFormattingInfo.fStructDateMask + " " + iIssStruct.fDisplayFormattingInfo.fStructTimeMask);
                                        }
                                    }
                                }

                                loRemarkField.Append( "EXPIRED " + loEnfValue );
                            }


                            ioGISValue = loRemarkField.ToString();
                            loGISValuePopulated = true;
                            break;
                        }

                    default:
                        {
                            loGISValuePopulated = false;
                            break;
                        }
                }

                //                             SetWirelessEnforcementInfo




                // get something?
                if (loGISValuePopulated == true)
                {

                    // kludge against bad JSON data
                    ioGISValue = ioGISValue.ToUpper();



                    EditControlBehavior uiComponentBehavior = GetUIComponentBehavior(uiComponent);

                    // lets see if this text is a match for a list item
                    int loIdx = -1;
                    if (uiComponentBehavior != null)
                    {
                        // see if its a list item
                        loIdx = Helper.GetListItemIndexFromStringList(uiComponentBehavior.GetFilteredListItems(), ioGISValue, Helper.ListItemMatchType.searchNoPartialMatch);

                        // lets get the whole list item, which might include abbrev + text desc
                        if (loIdx != -1)
                        {
                            ioGISValue = uiComponentBehavior.GetFilteredListItems()[loIdx];
                        }
                        else
                        {
                            // doesn't match list item... is it a list only field?
                            if (( string.IsNullOrEmpty( ioGISValue ) == false ) && (uiComponentBehavior.PanelField != null))
                            {
                                if (uiComponentBehavior.PanelField.OptionsList != null)
                                {
                                    if (uiComponentBehavior.PanelField.OptionsList.IsListOnly == true)
                                    {
                                        // this doesn't match any list entry, so we can't use it
                                        ioGISValue = string.Empty;
                                    }
                                }
                            }
                        }
                    }
                }




            }
            catch (Exception exp)
            {
                loGISValuePopulated = false;
                Console.WriteLine( "Failed to populate External Enforcement data:" + exp.Message);
            }


            return loGISValuePopulated;

        }

                

        

    }
}

