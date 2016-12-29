// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 12/04/13 8:15a $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Common/ClientConfig/Structures.cs $
//              Revision: $Revision: 106 $

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using AutoISSUE;

namespace Reino.ClientConfig
{
    /// <summary>
    /// Summary description for TIssStructMgr.
    /// </summary>

    public class TIssStructMgr : TObjBase
    {
        #region Properties and Members
        protected List<TSequenceStruct> _SequenceStructs;
        /// <summary>
        /// A collection of TSequenceStruct objects
        /// </summary>
        public List<TSequenceStruct> SequenceStructs
        {
            get { return _SequenceStructs; }
            set { _SequenceStructs = value; }
        }

        public TUserStruct UserStruct
        {
            get
            {
                // Find the UserStruct.
                foreach (TIssStruct loIssStruct in IssStructs)
                {
                    // check for special case TUserStruct
                    if (loIssStruct is TUserStruct)
                        return loIssStruct as TUserStruct;
                }
                return null;
            }
        }
        protected List<TIssStruct> _IssStructs;
        /// <summary>
        /// A collection of TIssStruct objects
        /// </summary>
        public List<TIssStruct> IssStructs
        {
            get { return _IssStructs; }
            set { _IssStructs = value; }
        }

        protected List<String> _SystemParameters;
        // This originally was a Hashtable of Key/Value pairs, but .NET won't serialize a Hashtable.
        public List<String> SystemParameters
        {
            get { return _SystemParameters; }
            set { _SystemParameters = value; }
        }

        protected List<string> _TransmitFiles;
        /// <summary>
        /// A collection of filenames
        /// </summary>
        public List<string> TransmitFiles
        {
            get { return _TransmitFiles; }
            set { _TransmitFiles = value; }
        }

        /// <summary>
        /// Will return the structure corresponding to passed structName. 
        /// </summary>
        /// <param name="structName"></param>
        /// <returns></returns>
        public TIssStruct this[string structName]
        {
            get
            {
                // Try to find struct in our list.
                foreach (TIssStruct oneStruct in IssStructs)
                {
                    if (oneStruct.Name == structName)
                    {
                        return oneStruct;
                    }
                }

                // If couldn't find then just return null.
                return null;
            }
        }

        #endregion

        public TIssStructMgr()
            : base()
        {
            this._IssStructs = new List<TIssStruct>();
            this._SystemParameters = new List<string>();
            this._TransmitFiles = new List<string>();
            this._SequenceStructs = new List<TSequenceStruct>();
        }

        public override void ResolveRegistryItems(ITTRegistry iRegistry)
        {
            for (int loObjectIndex = 0; loObjectIndex < _IssStructs.Count; loObjectIndex++)
            {
                _IssStructs[loObjectIndex].ResolveRegistryItems(iRegistry);
            }

            for (int loObjectIndex = 0; loObjectIndex < _SequenceStructs.Count; loObjectIndex++)
            {
                _SequenceStructs[loObjectIndex].ResolveRegistryItems(iRegistry);
            }

            base.ResolveRegistryItems(iRegistry);
        }

        public override int PostDeserialize(TObjBase iParent)
        {

            /*
            foreach (TIssStruct loTIssStruct in _IssStructs)
            {
                loTIssStruct.PostDeserialize(this);
            }
             
            */


            // post deserialize can cause items to be added to lists, rendering an enumerator approach invalid
            for (int loObjectIndex = 0; loObjectIndex < _IssStructs.Count; loObjectIndex++)
            {
                _IssStructs[loObjectIndex].PostDeserialize(this);
            }

            /*
            foreach (TSequenceStruct loTSequenceStruct in _SequenceStructs)
            {
                loTSequenceStruct.PostDeserialize(this);
            }
             */


            // post deserialize can cause items to be added to lists, rendering an enumerator approach invalid
            for (int loObjectIndex = 0; loObjectIndex < _SequenceStructs.Count; loObjectIndex++)
            {
                _SequenceStructs[loObjectIndex].PostDeserialize(this);
            }

            // Now call the base method
            return base.PostDeserialize(iParent);
        }
    }

    /// <summary>
    /// Summary description for TSequenceStruct.
    /// </summary>

    public class TSequenceStruct : TObjBase
    {
        public enum CheckDigitType
        {
            cdNone,
            cdMod7,
            cdMod10,
            cdMod10CDW2,
            cdMod10Hobart,
            cdMod10Mississauga,
            cdMod11,
            cdMod11Dade,
            cdHennepinCnty,
            cdPGeorgeMod10,
            cdMod10OddRJ,
            cdSaltLakeCityMod10
        }

        #region Properties and Members
        protected int _MaxLength = 0;
        public int MaxLength
        {
            get { return _MaxLength; }
            set { _MaxLength = value; }
        }

        protected CheckDigitType _CheckDigitCalc = CheckDigitType.cdNone;
        public CheckDigitType CheckDigitCalc
        {
            get { return _CheckDigitCalc; }
            set { _CheckDigitCalc = value; }
        }


        protected bool _IgnorePrefixSuffixInRangeManager = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool IgnorePrefixSuffixInRangeManager
        {
            get { return _IgnorePrefixSuffixInRangeManager; }
            set { _IgnorePrefixSuffixInRangeManager = value; }
        }


        /// <summary>
        /// A collection of TIssStruct objects that use this sequence
        /// </summary>
        protected List<TIssStruct> _IssStructs = new List<TIssStruct>();
        public List<TIssStruct> IssStructs
        {
            get { return _IssStructs; }
            set { _IssStructs = value; }
        }
        #endregion

        public override int PostDeserialize(TObjBase iParent)
        {
            TIssStructMgr loIssStructMgr = (TIssStructMgr)iParent;
            foreach (TIssStruct loIssStruct in loIssStructMgr.IssStructs)
            {
                if (loIssStruct.Sequence == this.Name)
                {
                    //Search to see if this structure is already in the list
                    TObjBasePredicate IssStructPredicate = new TObjBasePredicate(this.Name);
                    TIssStruct FoundIssStruct = this.IssStructs.Find(IssStructPredicate.CompareByName);
                    if (FoundIssStruct == null)
                        this.IssStructs.Add(loIssStruct);
                }
            }
            // Now call the base method
            return base.PostDeserialize(iParent);
        }
        public TSequenceStruct()
            : base()
        {
        }
    }

    /// <summary>
    /// Summary description for TIssStruct.
    /// </summary>

    /* The XmlInclude attribute is used on a base type to indicate that when serializing 
     * instances of that type, they might really be instances of one or more subtypes. 
     * This allows the serialization engine to emit a schema that reflects the possibility 
     * of really getting a Derived when the type signature is Base. For example, we keep
     * data structures in a generic collection of TIssStruct. If an array element is 
     * TUserStruct, the XML serializer gets mad because it was only expecting TIssStruct, 
     * even though TUserStruct is derived from TIssStruct.
     */
    [XmlInclude(typeof(TUserStruct)), XmlInclude(typeof(TActivityLogStruct)),
#if !WindowsCE && !__ANDROID__  
     XmlInclude(typeof(TGenericIssueStruct)), 
     XmlInclude(typeof(TVioStruct)),
     XmlInclude(typeof(TGeoCodeStruct)),
     XmlInclude(typeof(TMeterTrax_PoleLocationStruct)),
     XmlInclude(typeof(TMeterTrax_MeterMechanismStruct)),
     XmlInclude(typeof(TMeterTrax_MeterInstallationHistory)),
     XmlInclude(typeof(TMeterTrax_FieldTransactionTempStruct)),
     XmlInclude(typeof(TMeterTrax_AuditTransactionStruct)),
     XmlInclude(typeof(TMeterTrax_OpCheckTransactionStruct)),
     XmlInclude(typeof(TMeterTrax_InventoryTransactionStruct)),
     XmlInclude(typeof(TMeterTrax_OutageTransactionStruct)),
     XmlInclude(typeof(TMeterTrax_RateProgramTransactionStruct)),
     XmlInclude(typeof(TMeterTrax_RepairTransactionStruct)),
     XmlInclude(typeof(TNSWStatusStruct)),
#endif
     XmlInclude(typeof(TSearchStruct)), XmlInclude(typeof(THotSheetStruct)),
     XmlInclude(typeof(TMarkModeStruct)), XmlInclude(typeof(TCiteStruct)),
     XmlInclude(typeof(TCiteStruct)), XmlInclude(typeof(TPublicContactStruct)),
     XmlInclude(typeof(TCiteDetailStruct)), XmlInclude(typeof(TVoidStruct)),
     XmlInclude(typeof(TNotesStruct)), XmlInclude(typeof(TReissueStruct)),
     XmlInclude(typeof(TCancelStruct)), XmlInclude(typeof(TContinuanceStruct)),
     XmlInclude(typeof(THotDispoStruct)), XmlInclude(typeof(TTowStruct)),
     XmlInclude(typeof(TPermitRFIDSrchLogStruct))]
    public class TIssStruct : TTableListMgr
    {
        public TIssStructMgr ParentStructMgr { get { return (TIssStructMgr)Parent; } }
        public enum TWirelessUploadType
        {
            wuNever,
            wuWhenAvailable
        }

        #region Properties and Members
        protected string _ObjDisplayName = "";
        public string ObjDisplayName
        {
            get { return _ObjDisplayName; }
            set { _ObjDisplayName = value; }
        }

        protected string _Sequence = "";
        public string Sequence
        {
            get { return _Sequence; }
            set { _Sequence = value; }
        }

        protected ListObjBase<TIssMenuItem> _IssMenuItems;
        /// <summary>
        /// A collection of TIssMenuItem objects
        /// </summary>
        public ListObjBase<TIssMenuItem> IssMenuItems
        {
            get { return _IssMenuItems; }
            set { _IssMenuItems = value; }
        }

        protected string _IssueStruct = "";
        public string IssueStruct
        {
            get { return _IssueStruct; }
            set { _IssueStruct = value; }
        }

        protected TWirelessUploadType _WirelessUploadEnabled = TWirelessUploadType.wuNever;
        public TWirelessUploadType WirelessUploadEnabled
        {
            get { return _WirelessUploadEnabled; }
            set 
            { 
                _WirelessUploadEnabled = value;
                // If appropritate, set static member so we know at a glance that wireless is enabled
                if (value == TWirelessUploadType.wuWhenAvailable)
                    TClientDef.CfgSupportsWireless = true;
            }
        }

        
        protected ListObjBase<TTForm> _Forms;
        /// <summary>
        /// A collection of TTForm objects
        /// </summary>
        public ListObjBase<TTForm> Forms
        {
            get { return _Forms; }
            set { _Forms = value; }
        }
        
        /// <summary>
        /// bool IsHandheldCaptured
        /// 
        /// Indicates whether or not the TIssStruct instance contains data captured by the
        /// handheld.  The host uses this info to determine if the file is to be downloaded.
        /// The handheld uses this info to determine if it should open the underlying data file
        /// in "read only" (if is is not handheld captured) or or in "edit" (if it is handheld
        /// captured) mode.
        /// </summary>
        [XmlIgnore]
        public bool IsHandheldCaptured = true; // for most cases, IssStructs are handheld captured

        #region Host Side Only Predefined Standard Field Lists
#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Flag indicating object was created dynamically for the host side only
        /// and does not exist in the handheld client instance
        /// </summary>
        protected bool _IsHostSideDefinitionOnly;
        // This property must be included in the serialization so it is retained through Serialization/Deserialization
        [HostSideOnly] // Only used on the host-side
        public bool IsHostSideDefinitionOnly
        {
            get { return _IsHostSideDefinitionOnly; }
            set { _IsHostSideDefinitionOnly = value; }
        }

        /// <summary>
        /// Add the fields that must be defined for any issuance struct
        /// </summary>
        /// <param name="iFieldList"></param>
        protected void AddStandardIssueFields(List<THostRequiredFieldInfoObj> iFieldList)
        {
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlFormRevisionNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlFormRevisionNameStr, 25, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlTableRevisionNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlTransactionKeyStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlTransactionNameStr, 50, TDataBaseColumnDataType.dbtString, false, "");
        }

        /// <summary>
        /// Add the fields that must be defined for any ticket struct
        /// </summary>
        /// <param name="iFieldList"></param>
        protected void AddStandardTicketFields(List<THostRequiredFieldInfoObj> iFieldList)
        {
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlFormAndStrRevisionNumberStr, 9.2, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlUnitSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueNumberPrefixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueNumberSuffixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueNumberCheckDigitStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlDueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlRemark1Str, 80, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlRemark2Str, 80, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlVoidStatusStr, 2, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlVoidStatusDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlVoidReasonStr, 80, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlVoidedInFieldStr, 1, TDataBaseColumnDataType.dbtString, false, "N");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlReissuedStr, 1, TDataBaseColumnDataType.dbtString, false, "N");
        }

        /// <summary>
        /// All fields required for the export library.
        /// </summary>
        /// <param name="iFieldList"></param>
        protected void AddStandardExportFields(List<THostRequiredFieldInfoObj> iFieldList)
        {
            // Add all fields needed to export a record.
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlTransactionKeyStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlTransactionNameStr, 50, TDataBaseColumnDataType.dbtString, false, "");
            // User Defined Export Fields will be added dynamically (not sure how many of em we'll have).
            string[] userExportDateFields = DBConstants.GetAllUserDefinedExportDateFieldNames();
            foreach (string oneName in userExportDateFields)
            {
                AddHostSideRequiredFieldInfo(iFieldList, oneName, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            }
            // Custom Export Fields will be added dynamically (not sure how many of em we'll have).
            // (Custom Export Fields are used by multi-datatype exports managed via plug-in DLLs)
            string[] customExportDateFields = DBConstants.GetAllCustomExportDateFieldNames();
            foreach (string oneName in customExportDateFields)
            {
                AddHostSideRequiredFieldInfo(iFieldList, oneName, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            }

            // Add field for tracking real-time exports to AutoPROCESS
            AddHostSideRequiredFieldInfo(iFieldList, AutoISSUE.DBConstants.sqlWirelessAutoPROCExportDate, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, AutoISSUE.DBConstants.sqlAutoProcUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");
        }


        /// <summary>
        /// Add standard officer information fields
        /// </summary>
        /// <param name="iFieldList"></param>
        protected void AddStandardOfficerFields(List<THostRequiredFieldInfoObj> iFieldList)
        {
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");
        }

        /// <summary>
        /// Add Standard location fields
        /// </summary>
        /// <param name="iFieldList"></param>
        protected void AddStandardLocationFields(List<THostRequiredFieldInfoObj> iFieldList)
        {
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueAgencyStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueBeatStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlLocLotStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlLocBlockStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlLocStreetStr, 60, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlLocDescriptorStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlLocCrossStreet1Str, 60, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlLocCrossStreet2Str, 60, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlLatitudeDegreesStr, DBConstants.cnLatitudeOrLongitudeColumnPrecision, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlLongitudeDegreesStr, DBConstants.cnLatitudeOrLongitudeColumnPrecision, TDataBaseColumnDataType.dbtReal, false, "");

        }

        /// <summary>
        /// Add standard vehicle fields
        /// </summary>
        /// <param name="iFieldList"></param>
        protected void AddStandardVehicleFields(List<THostRequiredFieldInfoObj> iFieldList)
        {
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehLicNoStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehLicStateStr, 7, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehVINStr, 18, TDataBaseColumnDataType.dbtString, false, "");
        }

        /// <summary>
        /// Add "enhanced" vehicle fields... color, make, model, etc
        /// </summary>
        /// <param name="iFieldList"></param>
        protected void AddStandardEnhancedVehicleFields(List<THostRequiredFieldInfoObj> iFieldList)
        {
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehColor1Str, 8, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehColor2Str, 8, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehMakeStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehModelStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehBodyTypeStr, 15, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehLicExpDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehVIN8Str, 20, TDataBaseColumnDataType.dbtString, false, "");
        }


        /// <summary>
        /// If the records in this structure has can be geocoded, add the geocode subtable
        /// </summary>
        protected void AddGeoCodeSubTableIfRequired()
        {
            // if this struct has location info, add the geocode detail table
            bool loHasLocationInfo = (this.MainTable.GetFldNo(DBConstants.sqlLatitudeDegreesStr) > 0);
            if ((loHasLocationInfo == true) && (ParentStructMgr[this.Name + DBConstants.cnGeoCodeTableNameSuffix] == null))
            {
                // create a geocode structure for ourselves
                TGeoCodeStruct loGeoCodeStruct = new TGeoCodeStruct();
                loGeoCodeStruct.Name = this.Name + DBConstants.cnGeoCodeTableNameSuffix;
                loGeoCodeStruct.Parent = this.Parent;
                loGeoCodeStruct.ParentStruct = this.Name;
                loGeoCodeStruct.IsHostSideDefinitionOnly = true;
                // add it to the strutures list so we know about it - 
                // also, it will get PostDerialized called to initialize it
                ((TIssStructMgr)(this.Parent)).IssStructs.Add(loGeoCodeStruct);
            }
        }


#endif
        #endregion


        #region Android Client Side Only Predefined Standard Field Lists
#if __ANDROID__  
        ///// <summary>
        ///// Flag indicating object was created dynamically for the host side only
        ///// and does not exist in the handheld client instance
        ///// </summary>
        //protected bool _IsHostSideDefinitionOnly;
        //// This property must be included in the serialization so it is retained through Serialization/Deserialization
        //[HostSideOnly] // Only used on the host-side
        //public bool IsHostSideDefinitionOnly
        //{
        //    get { return _IsHostSideDefinitionOnly; }
        //    set { _IsHostSideDefinitionOnly = value; }
        //}

        /// <summary>
        /// Add the fields that must be defined for any issuance struct
        /// </summary>
        /// <param name="iFieldList"></param>
        protected void AddStandardIssueFields(List<TAndroidClientRequiredFieldInfoObj> iFieldList)
        {
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlFormRevisionNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlFormRevisionNameStr, 25, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlTableRevisionNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlTransactionKeyStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlTransactionNameStr, 50, TDataBaseColumnDataType.dbtString, false, "");
        }

        /// <summary>
        /// Add the fields that must be defined for any ticket struct
        /// </summary>
        /// <param name="iFieldList"></param>
        protected void AddStandardTicketFields(List<TAndroidClientRequiredFieldInfoObj> iFieldList)
        {
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlFormAndStrRevisionNumberStr, 9.2, TDataBaseColumnDataType.dbtReal, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlUnitSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueNumberPrefixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueNumberSuffixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueNumberCheckDigitStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlDueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlRemark1Str, 80, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlRemark2Str, 80, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlVoidStatusStr, 2, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlVoidStatusDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlVoidReasonStr, 80, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlVoidedInFieldStr, 1, TDataBaseColumnDataType.dbtString, false, "N");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlReissuedStr, 1, TDataBaseColumnDataType.dbtString, false, "N");
        }

        /// <summary>
        /// All fields required for the export library.
        /// </summary>
        /// <param name="iFieldList"></param>
        protected void AddStandardExportFields(List<TAndroidClientRequiredFieldInfoObj> iFieldList)
        {
            //// Add all fields needed to export a record.
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlTransactionKeyStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlTransactionNameStr, 50, TDataBaseColumnDataType.dbtString, false, "");
            //// User Defined Export Fields will be added dynamically (not sure how many of em we'll have).
            //string[] userExportDateFields = DBConstants.GetAllUserDefinedExportDateFieldNames();
            //foreach (string oneName in userExportDateFields)
            //{
            //    AddAndroidClientSideRequiredFieldInfo(iFieldList, oneName, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            //}
            //// Custom Export Fields will be added dynamically (not sure how many of em we'll have).
            //// (Custom Export Fields are used by multi-datatype exports managed via plug-in DLLs)
            //string[] customExportDateFields = DBConstants.GetAllCustomExportDateFieldNames();
            //foreach (string oneName in customExportDateFields)
            //{
            //    AddAndroidClientSideRequiredFieldInfo(iFieldList, oneName, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            //}

            //// Add field for tracking real-time exports to AutoPROCESS
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, AutoISSUE.DBConstants.sqlWirelessAutoPROCExportDate, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, AutoISSUE.DBConstants.sqlAutoProcUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");
        }


        /// <summary>
        /// Add standard officer information fields
        /// </summary>
        /// <param name="iFieldList"></param>
        protected void AddStandardOfficerFields(List<TAndroidClientRequiredFieldInfoObj> iFieldList)
        {
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");
        }

        /// <summary>
        /// Add Standard location fields
        /// </summary>
        /// <param name="iFieldList"></param>
        protected void AddStandardLocationFields(List<TAndroidClientRequiredFieldInfoObj> iFieldList)
        {
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueAgencyStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlIssueBeatStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlLocLotStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlLocBlockStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlLocStreetStr, 60, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlLocDescriptorStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlLocCrossStreet1Str, 60, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlLocCrossStreet2Str, 60, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlLatitudeDegreesStr, DBConstants.cnLatitudeOrLongitudeColumnPrecision, TDataBaseColumnDataType.dbtReal, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlLongitudeDegreesStr, DBConstants.cnLatitudeOrLongitudeColumnPrecision, TDataBaseColumnDataType.dbtReal, false, "");

        }

        /// <summary>
        /// Add standard vehicle fields
        /// </summary>
        /// <param name="iFieldList"></param>
        protected void AddStandardVehicleFields(List<TAndroidClientRequiredFieldInfoObj> iFieldList)
        {
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehLicNoStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehLicStateStr, 7, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehVINStr, 18, TDataBaseColumnDataType.dbtString, false, "");
        }

        /// <summary>
        /// Add "enhanced" vehicle fields... color, make, model, etc
        /// </summary>
        /// <param name="iFieldList"></param>
        protected void AddStandardEnhancedVehicleFields(List<TAndroidClientRequiredFieldInfoObj> iFieldList)
        {
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehColor1Str, 8, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehColor2Str, 8, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehMakeStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehModelStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehBodyTypeStr, 15, TDataBaseColumnDataType.dbtString, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehLicExpDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            //AddAndroidClientSideRequiredFieldInfo(iFieldList, DBConstants.sqlVehVIN8Str, 20, TDataBaseColumnDataType.dbtString, false, "");
        }


        /// <summary>
        /// If the records in this structure has can be geocoded, add the geocode subtable
        /// </summary>
        protected void AddGeoCodeSubTableIfRequired()
        {
            // if this struct has location info, add the geocode detail table
            bool loHasLocationInfo = (this.MainTable.GetFldNo(DBConstants.sqlLatitudeDegreesStr) > 0);
            if ((loHasLocationInfo == true) && (ParentStructMgr[this.Name + DBConstants.cnGeoCodeTableNameSuffix] == null))
            {
                // create a geocode structure for ourselves
                TGeoCodeStruct loGeoCodeStruct = new TGeoCodeStruct();
                loGeoCodeStruct.Name = this.Name + DBConstants.cnGeoCodeTableNameSuffix;
                loGeoCodeStruct.Parent = this.Parent;
                loGeoCodeStruct.ParentStruct = this.Name;
                //loGeoCodeStruct.IsHostSideDefinitionOnly = true;
                // add it to the strutures list so we know about it - 
                // also, it will get PostDerialized called to initialize it
                ((TIssStructMgr)(this.Parent)).IssStructs.Add(loGeoCodeStruct);
            }
        }


#endif
        #endregion


        // needed for Android, but only a few items, not the compiler directive
// #if USE_DEFN_IMPLEMENTATION
        [XmlIgnoreAttribute]
        public Object StructLogicObj = null;
// #endif
        #endregion


        public TIssStruct()
            : base()
        {
            this._Forms = new ListObjBase<TTForm>();
            this._IssMenuItems = new ListObjBase<TIssMenuItem>();

            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            // Add standard fields that are common for all issue structs - this is the ancestor of them all
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlRecCreationDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlRecoveredByHHStr, 1, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlFormRevisionNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlFormRevisionNameStr, 25, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlTableRevisionNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlStandardExportDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlReinoRowVersionStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSubConfigKeyStr, DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "0");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlTblEncryptVersionStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlEncryptionKeyID, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            }
#endif
            #endregion

            #region Code to add Android Client Side-Only Standard Fields
#if __ANDROID__

            AddAndroidClientSideRequiredFieldInfo(fAndroidClientOnlyStandardFields, DBConstants.ID_COLUMN, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");
            //AddAndroidClientSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");

            AddAndroidClientSideRequiredFieldInfo(fAndroidClientOnlyStandardFields, DBConstants.STATUS_COLUMN, 1, TDataBaseColumnDataType.dbtString, false, "");
            AddAndroidClientSideRequiredFieldInfo(fAndroidClientOnlyStandardFields, DBConstants.WS_STATUS_COLUMN, 1, TDataBaseColumnDataType.dbtString, false, "");

            //AddAndroidClientSideRequiredFieldInfo(fAndroidClientOnlyStandardFields, DBConstants.sqlVoidStatusDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            //AddAndroidClientSideRequiredFieldInfo(fAndroidClientOnlyStandardFields, DBConstants.sqlReissuedStr, 1, TDataBaseColumnDataType.dbtString, false, "N");

#endif

            #endregion

        }

        public override int PostDeserialize(TObjBase iParent)
        {
#if USE_DEFN_IMPLEMENTATION
            // defense: some tables aren't defined just yet (ie violations)
            if (this.MainTable != null)
            {
                // Before doing PostDeserialize, we must set flag to indicate if the 
                // associated .DAT file will be opened in read-only or Read/Write mode.
                // These are the only types of structures that support read/write access to
                // their database files. All other structure types will use read-only access
                // to the underlying database file:

                // mcb - 1/10/06: Avoid references to descendant classes when an easy oop alternative
                // exists.  In this case, Allow the descendant classes to indicate whether or not
                // they are "capture" type structures. If they are, then must open the table editing.
                this.MainTable.fOpenEdit = this.IsHandheldCaptured;

            }
#endif
            // Call inherited PostDeserialize which will eventually open the physical database file
            base.PostDeserialize(iParent);

#if USE_DEFN_IMPLEMENTATION
            // Need to create a TTTable object for this structure because its
            // not an object that get deserialized from the XML.
            // Get table definition and table revision associated with structure
            Reino.ClientConfig.TTableDef tableDef = this.MainTable;
            Reino.ClientConfig.TTableDefRev tableRev = tableDef.HighTableRevision;
            // If the TableDefRev doesn't have a TTTable, create one
            if (tableRev.Tables.Count == 0)
            {
                Reino.ClientConfig.TTTable newTable = new Reino.ClientConfig.TTTable();
                newTable.SetTableName(tableDef.Name); // this will also add table to the list of tables
            }
#endif


            _Forms.PostDeserializeListItems(this);
            _IssMenuItems.PostDeserializeListItems(this);
            return 0;
        }

        public override void ResolveRegistryItems(ITTRegistry iRegistry)
        {
            base.ResolveRegistryItems(iRegistry);
            _Forms.ResolveRegistryItemsForListItems(iRegistry);
            _IssMenuItems.ResolveRegistryItemsForListItems(iRegistry);
        }

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Virtual routine that allows descendant classes to add whatever fields they need
        /// for the host side.
        /// </summary>
        protected virtual void VerifyHostTable()
        {
        }

        /// <summary>
        /// Called after postdeserialize on the host side only.
        /// Makes alterations to metadata to reflect the additional columns and tables required on the host
        /// side not defined on the handheld side.
        /// </summary>
        protected override void FinalizeHostSideClientConfig()
        {

            // do inherited version first.
            base.FinalizeHostSideClientConfig();

            // developer note: the order here been reversed, previously the code called VerifyStatusTable
            // before VerifyHostTable - but in some cases this resulted in the STATUS table getting
            // assigned as the MainTable because it was the table created first in the cases of
            // specialty tables, such as those in the MeterTrax implementation which have no 
            // ISSUE_AP.XML based configuration and instead are created during VerifyHostTable

            // give each descendant an opportunity to to any class specific verification
            VerifyHostTable();

            // Create a status table for almost all structs. Those that don't want one can
            // override VerifyStatusTable to do nothing.
            VerifyStatusTable();


        }
#endif // !WindowsCE


#if !_ANDROID__  
        /// <summary>
        /// Virtual routine that allows descendant classes to add whatever fields they need
        /// for the host side.
        /// </summary>
        protected virtual void VerifyHostTable()
        {

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Host only post-post deserialization. Adds metadata info that exists on host sideonly
        /// (table columns & indexes, primarily)
        /// </summary>
        protected override void FinalizeHostSideClientConfig()
        {

            // do inherited version first.
            base.FinalizeHostSideClientConfig();

            VerifyFieldsInTableDef(fHostOnlyStandardFields, this.MainTable);
        }
#endif // !WindowsCE

        }

        /// <summary>
        /// Called after postdeserialize on the host side only.
        /// Makes alterations to metadata to reflect the additional columns and tables required on the host
        /// side not defined on the handheld side.
        /// </summary>
        protected override void FinalizeHostSideClientConfig()
        {

            // do inherited version first.
            base.FinalizeHostSideClientConfig();

            // developer note: the order here been reversed, previously the code called VerifyStatusTable
            // before VerifyHostTable - but in some cases this resulted in the STATUS table getting
            // assigned as the MainTable because it was the table created first in the cases of
            // specialty tables, such as those in the MeterTrax implementation which have no 
            // ISSUE_AP.XML based configuration and instead are created during VerifyHostTable

            // give each descendant an opportunity to to any class specific verification
            VerifyHostTable();

            // Create a status table for almost all structs. Those that don't want one can
            // override VerifyStatusTable to do nothing.
            //VerifyStatusTable();



            // Android - doesn't need to override every decendant, no poly-variances like the host side 
            VerifyFieldsInTableDef(fAndroidClientOnlyStandardFields, this.MainTable);


        }
#endif 
    }

    /// <summary>
    /// Summary description for TUserStruct.
    /// </summary>

    public class TUserStruct : TIssStruct
    {
        #region Properties and Members
        #endregion

        public TUserStruct()
            : base()
        {
            IsHandheldCaptured = false;
#if !WindowsCE && !__ANDROID__  
            // The USER struct is a specialty struct and has no host side database table 
            // so we'll clear those fields added in the base code
            if (TClientDef.SkipHostSideOnlyCode == false)
                fHostOnlyStandardFields.Clear();
#endif

        }

        /// <summary>
        /// Instead of storing passwords, we store a salted hash of the password. Because of this,
        /// the "Password" field of the User Structure needs to be replaces with "Salt" and 
        /// "SaltedHash" fields that can be used for validating a user. We will just make the 
        /// switcheroo right here in code so layout tool and/or service group doesn't need to change.
        /// </summary>
        public override int PostDeserialize(TObjBase iParent)
        {
            // Find index of "LoginPwd" field
            Reino.ClientConfig.TObjBasePredicate predicate = new TObjBasePredicate("LoginPwd");
            int loPwdFldIdx = this.MainTable.HighTableRevision.Fields.FindIndex(predicate.CompareByName_CaseInsensitive);

            /// For now we won't modify it for handheld code to retain backward compatibility...
#if !WindowsCE && !__ANDROID__  
            // If the "LoginPwd" field was found, we need to replace it with Salt and SaltedHash
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                if (loPwdFldIdx > -1)
                {
                    // First we'll delete the "LoginPwd" field
                    this.MainTable.HighTableRevision.Fields.RemoveAt(loPwdFldIdx);

                    // Now create the "SaltedHash" field and insert it where password was
                    TTableStringFldDef loSaltedHashFld = new TTableStringFldDef();
                    loSaltedHashFld.DisplayName = "Salted Hash";
                    loSaltedHashFld.EditDataType = TTableFldDef.TEditDataType.tftString;
                    loSaltedHashFld.Name = "SaltedHash";
                    loSaltedHashFld.TableNdx = 0;
                    loSaltedHashFld.Mask = "!";
                    loSaltedHashFld.MaskForHH = "!";
                    loSaltedHashFld.Size = 50;
                    if (this.MainTable.HighTableRevision.Fields.Count > loPwdFldIdx)
                        this.MainTable.HighTableRevision.Fields.Insert(loPwdFldIdx, loSaltedHashFld);
                    else
                        this.MainTable.HighTableRevision.Fields.Add(loSaltedHashFld);

                    // Now create the "Salt" field and insert in front of "SaltedHash";
                    TTableStringFldDef loSaltFld = new TTableStringFldDef();
                    loSaltFld.DisplayName = "Salt";
                    loSaltFld.EditDataType = TTableFldDef.TEditDataType.tftString;
                    loSaltFld.Name = "Salt";
                    loSaltFld.TableNdx = 0;
                    loSaltFld.Mask = "!";
                    loSaltFld.MaskForHH = "!";
                    loSaltFld.Size = 12;
                    this.MainTable.HighTableRevision.Fields.Insert(loPwdFldIdx, loSaltFld);
                }
            }
#endif

            // Now we can proceed with the inherited PostDeserialize routine
            return base.PostDeserialize(iParent);
        }
#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Override the base implementation in order to suppress the creation of a status table.
        /// </summary>
        protected override void VerifyStatusTable()
        {
            // Don't want a status table on void structs. Suppress it by having an empty routine.
        }
#endif
    } // TUserStruct

    /// <summary>
    /// Summary description for TActivityLogStruct.
    /// </summary>

    public class TActivityLogStruct : TIssStruct
    {
        #region Properties and Members
        #endregion

        public TActivityLogStruct()
            : base()
        {
            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            // Add standard collections of fields 
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddStandardOfficerFields(fHostOnlyStandardFields);
                AddStandardExportFields(fHostOnlyStandardFields);

                // Add additional fields needed for activity log structs
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlStartDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlStartTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlEndDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlEndTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlPrimaryActivityNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlPrimaryActivityCountStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSecondaryActivityNameStr, 20, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSecondaryActivityCountStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlExtraPrompt1Str, 80, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlExtraData1Str, 80, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlExtraPrompt2Str, 80, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlExtraData2Str, 80, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlExtraPrompt3Str, 80, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlExtraData3Str, 80, TDataBaseColumnDataType.dbtString, false, "");
                //  add lat/long for GPS enabled plaforms
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLOGStartLatitudeFieldName, DBConstants.cnLatitudeOrLongitudeColumnPrecision, TDataBaseColumnDataType.dbtReal, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLOGStartLongituteFieldName, DBConstants.cnLatitudeOrLongitudeColumnPrecision, TDataBaseColumnDataType.dbtReal, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLOGEndLatitudeFieldName, DBConstants.cnLatitudeOrLongitudeColumnPrecision, TDataBaseColumnDataType.dbtReal, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLOGEndLongitudeFieldName, DBConstants.cnLatitudeOrLongitudeColumnPrecision, TDataBaseColumnDataType.dbtReal, false, "");
                // Also add unit serial number to support asset tracking based on GPS coordinates
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUnitSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            }
#endif
            #endregion
        } // end constructor


#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Host only post-post deserialization. Adds metadata info that exists on host sideonly
        /// (table columns & indexes, primarily)
        /// </summary>
        protected override void FinalizeHostSideClientConfig()
        {

            // do inherited version first.
            base.FinalizeHostSideClientConfig();

            VerifyFieldsInTableDef(fHostOnlyStandardFields, this.MainTable);
        }
#endif // !WindowsCE

    }


    /// <summary>
    /// Summary description for TSearchStruct.
    /// </summary>

    public class TSearchStruct : TIssStruct
    {
        public enum TWirelessEnabledType
        {
            wlNever,
            wlWhenAvailable
        }

        #region Properties and Members
        protected string _MatchFields = "";
        public string MatchFields
        {
            get { return _MatchFields; }
            set { _MatchFields = value; }
        }

        protected string _IssueStructFirstFocus = "";
        public string IssueStructFirstFocus
        {
            get { return _IssueStructFirstFocus; }
            set { _IssueStructFirstFocus = value; }
        }

        protected List<string> _AddlMatchFields = new List<string>();
        public List<string> AddlMatchFields
        {
            get { return _AddlMatchFields; }
            set { _AddlMatchFields = value; }
        }

        protected TWirelessEnabledType _WirelessSearchEnabled = TWirelessEnabledType.wlNever;
        public TWirelessEnabledType WirelessSearchEnabled
        {
            get { return _WirelessSearchEnabled; }
            set 
            { 
                _WirelessSearchEnabled = value;
                // If appropritate, set static member so we know at a glance that wireless is enabled
                if (value == TWirelessEnabledType.wlWhenAvailable)
                    TClientDef.CfgSupportsWireless = true;
            }
        }

        // When this is set to true the cite struct can export its data
        // to the handheld (using a DAT file).
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool UploadToHandheld = false;
        #endregion

        public TSearchStruct()
            : base()
        {
            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
#endif
            #endregion
        }
    }

    /// <summary>
    /// Summary description for THotSheetStruct.
    /// </summary>

    public class THotSheetStruct : TSearchStruct
    {
        public enum THotSheetType
        {
            htNormal,
            htWarning
        }

        #region Properties and Members
        protected string _MatchFieldsName = "";
        public string MatchFieldsName
        {
            get { return _MatchFieldsName; }
            set { _MatchFieldsName = value; }
        }

        protected string _AssocStructure = "";
        public string AssocStructure
        {
            get { return _AssocStructure; }
            set { _AssocStructure = value; }
        }

        protected THotSheetType _HotSheetType = THotSheetType.htNormal;
        public THotSheetType HotSheetType
        {
            get { return _HotSheetType; }
            set { _HotSheetType = value; }
        }
        #endregion

        public THotSheetStruct()
            : base()
        {
            IsHandheldCaptured = false; // not handheld captured data.

            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
#endif
            #endregion


        }
#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Override the base implementation in order to suppress the creation of a status table.
        /// </summary>
        protected override void VerifyStatusTable()
        {
            // Don't want a status table on HotSheet structs. Suppress it by having an empty routine.
        }
#endif
    }


    #region Code for Host Side-Only Generic Structure Fields

    /// <summary>
    /// Generic structure without any specific meaning to the host 
    /// </summary>
    [HostSideOnly] // Only applicable to the host-side
    public class TGenericIssueStruct : TIssStruct
    {
        public TGenericIssueStruct()
            : base()
        {

    #region Code to add Host Side-Only Standard Fields
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization

#if !WindowsCE && !__ANDROID__  
            // Add standard collections of fields 
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddStandardExportFields(fHostOnlyStandardFields);
            }
#endif
    #endregion
        }
#if !WindowsCE && !__ANDROID__  
    /// <summary>
        /// Host only post-post deserialization. Adds metadata info that exists on host sideonly
        /// (table columns & indexes, primarily)
        /// </summary>
        protected override void FinalizeHostSideClientConfig()
        {

            // do inherited version first.
            base.FinalizeHostSideClientConfig();

            VerifyFieldsInTableDef(fHostOnlyStandardFields, this.MainTable);

            // if this struct has location info, add the geocode detail table
            AddGeoCodeSubTableIfRequired();
        }
#endif // !WindowsCE
    }
    #endregion


    /// <summary>
    /// Summary description for TCiteStruct.
    /// </summary>
    public class TCiteStruct : TIssStruct
    {
        #region Properties and Members
        [XmlIgnoreAttribute]
        public TVoidStruct VoidStruct = null;
        [XmlIgnoreAttribute]
        public TCancelStruct CancelStruct = null;
        [XmlIgnoreAttribute]
        public TNotesStruct NotesStruct = null;
        [XmlIgnoreAttribute]
        public TReissueStruct ReissueStruct = null;
        [XmlIgnoreAttribute]
        public TContinuanceStruct ContinuanceStruct = null;

        // http://jirssb01:8080/browse/AUTOCITE-196
        [XmlIgnoreAttribute]
        public bool EligibleForNSWStatus = false;

        // When this is set to true the cite struct can export its data
        // to the handheld (using a DAT file).
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool UploadToHandheld = false;

#if !WindowsCE 
//#if !WindowsCE && !__ANDROID__  
        [HostSideOnly] // Only used on the host-side
        public int ViolationCount = 1; // Must default to 1 in order to match behavior of layout tool

        // These are the "reason" list names that are used by inquiry screen. Only need on host side.
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        [HostSideOnly] // Only used on the host-side
        public string VoidReasonList = "";

        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        [HostSideOnly] // Only used on the host-side
        public string ReinstateReasonList = "";
#endif

        #endregion

        public TCiteStruct()
            : base()
        {

            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            // Add standard collections of fields 
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddStandardIssueFields(fHostOnlyStandardFields);
                AddStandardTicketFields(fHostOnlyStandardFields);
                AddStandardExportFields(fHostOnlyStandardFields);
                AddStandardOfficerFields(fHostOnlyStandardFields);
                AddStandardLocationFields(fHostOnlyStandardFields);
                AddStandardVehicleFields(fHostOnlyStandardFields);
                AddStandardEnhancedVehicleFields(fHostOnlyStandardFields);

                // some addtional fields for all citation issuance structures 
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVehPermitNumberStr, 14, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterNumberStr, 10, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIsWarningStr, 1, TDataBaseColumnDataType.dbtString, false, "N");
            }
#endif
            #endregion
        }

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Host only post-post deserialization. Adds metadata info that exists on host sideonly
        /// (table columns & indexes, primarily)
        /// </summary>
        protected override void FinalizeHostSideClientConfig()
        {

            // do inherited version first.
            base.FinalizeHostSideClientConfig();

            VerifyFieldsInTableDef(fHostOnlyStandardFields, this.MainTable);

            // if this struct has location info, add the geocode detail table
            AddGeoCodeSubTableIfRequired();
        }

        protected override void VerifyHostTable()
        {

            /// We should verify the Violation table if there is supposed to be at least one violation 
            if ((ViolationCount > 0) && (ParentStructMgr[this.Name + DBConstants.cnViolationTableNameSuffix] == null))
            {
                // create a violation structure for ourselves
                TVioStruct loVioStruct = new TVioStruct();
                loVioStruct.Name = this.Name + DBConstants.cnViolationTableNameSuffix;
                loVioStruct.Parent = this.Parent;
                loVioStruct.ParentStruct = this.Name;
                loVioStruct.IsHostSideDefinitionOnly = true;
                // add it to the strutures list so we know about it - 
                // also, it will get PostDerialized called to initialize it
                ((TIssStructMgr)(this.Parent)).IssStructs.Add(loVioStruct);
            }


        }

#endif // !WindowsCE

        // http://jirssb01:8080/browse/AUTOCITE-196
        public override int PostDeserialize(TObjBase iParent)
        {
            // We will consider this as a structure that is applicable to NSW Status if it contains a couple "magic" fields
            // that we would only expect to encounter in an NSW configuration (New South Wales)
            TObjBasePredicate predicate = new TObjBasePredicate("INFRINGEMENTTYPE");
            int loFldIdx = this.MainTable.HighTableRevision.Fields.FindIndex(predicate.CompareByName_CaseInsensitive);
            if (loFldIdx >= 0)
            {
                predicate = new TObjBasePredicate("OFFENCEREQQUERY");
                loFldIdx = this.MainTable.HighTableRevision.Fields.FindIndex(predicate.CompareByName_CaseInsensitive);
                if (loFldIdx >= 0)
                {
                    this.EligibleForNSWStatus = true;
                }
            }

            // Now call the base method
            return base.PostDeserialize(iParent);
        }
    } // TCiteStruct



    /// <summary>
    /// Summary description for TTowStruct.
    /// </summary>
    public class TTowStruct : TCiteStruct
    {
        #region Properties and Members

#if !WindowsCE && !__ANDROID__  
        // These are the "reason" list names that are used by inquiry screen. Only need on host side.
        //[System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        //[HostSideOnly] // Only used on the host-side
        //public string VoidReasonList = "";
#endif

        #endregion

        public TTowStruct()
            : base()
        {

            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            // Add standard collections of fields 
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                //AddStandardTowFields(fHostOnlyStandardFields);

                // some addtional fields for all tow structures 
                //AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVehPermitNumberStr, 14, TDataBaseColumnDataType.dbtString, false, "");
                //AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterNumberStr, 10, TDataBaseColumnDataType.dbtString, false, "");
                //AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIsWarningStr, 1, TDataBaseColumnDataType.dbtString, false, "N");
            }
#endif
            #endregion
        }

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Host only post-post deserialization. Adds metadata info that exists on host sideonly
        /// (table columns & indexes, primarily)
        /// </summary>
        protected override void FinalizeHostSideClientConfig()
        {

            // do inherited version first.
            base.FinalizeHostSideClientConfig();

            //VerifyFieldsInTableDef(fHostOnlyStandardFields, this.MainTable);

            // if this struct has location info, add the geocode detail table
           // AddGeoCodeSubTableIfRequired();
        }

        protected override void VerifyHostTable()
        {
            // call inherited version first
            base.VerifyHostTable();
        }

#endif // !WindowsCE

    } // TTowStruct


    /// <summary>
    /// Summary description for TCiteDetailStruct.
    /// </summary>
    public class TCiteDetailStruct : TIssStruct
    {
        #region Properties and Members
        protected string _ParentStruct = "";
        public string ParentStruct
        {
            get { return _ParentStruct; }
            set { _ParentStruct = value; }
        }

        [XmlIgnoreAttribute]
        public TIssStruct ParentStructObj = null;
        #endregion

        public TCiteDetailStruct()
            : base()
        {
            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            // Add standard collections of fields 
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddStandardOfficerFields(fHostOnlyStandardFields);

                // Add additional fields needed for detail structs
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMasterKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");
            }
#endif

            #region Code to add Android Client-Only Standard Fields
#if __ANDROID__
            // Add standard collections of fields 
            AddAndroidClientSideRequiredFieldInfo(fAndroidClientOnlyStandardFields, DBConstants.SEQUENCE_ID, 1, TDataBaseColumnDataType.dbtString, false, "");
            AddAndroidClientSideRequiredFieldInfo(fAndroidClientOnlyStandardFields, DBConstants.sqlMasterKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");
#endif


            #endregion
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            if (TClientDef.GlobalClientDef != null)
            {
                // Try to get static reference to parent structure
                TObjBasePredicate predicate = new TObjBasePredicate(_ParentStruct);
                ParentStructObj = TClientDef.GlobalClientDef.IssStructMgr.IssStructs.Find(predicate.CompareByName_CaseInsensitive);

                // Also set applicable reference to ourself in the parent structure
                if (ParentStructObj != null)
                {
                    if ((this is TVoidStruct) && (ParentStructObj is TCiteStruct))
                    {
                        ((TCiteStruct)(ParentStructObj)).VoidStruct = this as TVoidStruct;

#if !WindowsCE 
//#if !WindowsCE && !__ANDROID__  


                        // AJW - TODO - these give us host side suggestions, instead we needs to examine/iterate the void form and find the correct field

                        /*
                        //If the Void Reason List is blank try populate it here
                        if (((TCiteStruct)(ParentStructObj)).VoidReasonList.Length == 0)
                        {
                            // give it the name based on the struct + the standard suffix
                            ((TCiteStruct)(ParentStructObj)).VoidReasonList = ((TCiteStruct)(ParentStructObj)).Name.ToUpper() + DBConstants.cnVoidReasonsListNameSuffix;
                        }

                        //If the Reinstate  Reason List is blank try populate it here
                        if (((TCiteStruct)(ParentStructObj)).ReinstateReasonList.Length == 0)
                        {
                            // give it the name based on the struct + the standard suffix
                            ((TCiteStruct)(ParentStructObj)).ReinstateReasonList = ((TCiteStruct)(ParentStructObj)).Name.ToUpper() + DBConstants.cnReinstateReasonsListNameSuffix;
                        }
                         */
#endif

                    }
                    if ((this is TCancelStruct) && (ParentStructObj is TCiteStruct))
                        ((TCiteStruct)(ParentStructObj)).CancelStruct = this as TCancelStruct;
                    if ((this is TNotesStruct) && (ParentStructObj is TCiteStruct))
                        ((TCiteStruct)(ParentStructObj)).NotesStruct = this as TNotesStruct;
                    if ((this is TReissueStruct) && (ParentStructObj is TCiteStruct))
                        ((TCiteStruct)(ParentStructObj)).ReissueStruct = this as TReissueStruct;
                    if ((this is TContinuanceStruct) && (ParentStructObj is TCiteStruct))
                        ((TCiteStruct)(ParentStructObj)).ContinuanceStruct = this as TContinuanceStruct;
                }
            }

            // Now call the base method
            return base.PostDeserialize(iParent);
        }
    }


    #region TVioStruct is a Host Side-Only Standard Fields
    /// <summary>
    /// The TVioStruct is a cite detail struct defined for the host side only.
    /// It correlates to the violation subtable which is created only on the host side
    /// The class must be exposed to both side for XML serialiation on the host to function
    /// </summary>
    public class TVioStruct : TCiteDetailStruct
    {

        public TVioStruct()
            : base()
        {

            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            if (TClientDef.SkipHostSideOnlyCode == false)
                _IsHostSideDefinitionOnly = true;
            // the HostSideRequired fields for the TVioStruct are added in the VerifyViolationTable method
#endif
            #endregion
        }

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Override the base implementation in order to suppress the creation of a status table.
        /// </summary>
        protected override void VerifyStatusTable()
        {
            // Don't want a status table on void structs. Suppress it by having an empty routine.
        }
        /// <summary>
        /// Specialty validation routine for Violation tables
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loViolationTableDef = GetOrCreateSpecialtyTableDef("" /* the name is derived from the struct name*/ );

            // Create list of standard fields we need for this table 
            List<THostRequiredFieldInfoObj> loFieldList = new List<THostRequiredFieldInfoObj>();

            // determine our parent struct
            TIssStruct loParentStruct = ((TCiteDetailStruct)this).ParentStructObj;
            if (loParentStruct == null)
            {
                TObjBasePredicate loFindStuct = new TObjBasePredicate(((TCiteDetailStruct)this).ParentStruct);
                loParentStruct = ((TIssStructMgr)(this.Parent)).IssStructs.Find(loFindStuct.CompareByName_CaseInsensitive);
            }

            // now look through our parent and take all of the fields designated for us
            for (int loFldIdx = 0; loFldIdx < loParentStruct.MainTable.GetFieldCnt(); loFldIdx++)
            {
                // get the next field
                TTableFldDef loField = loParentStruct.MainTable.GetField(loFldIdx);

                // is this a detail field for us?
                if (loField.TableNdx > 0)
                {
                    // it is - we want to make sure its in the subtable
                    AddHostSideRequiredFieldInfo(loFieldList, loField.Name, loField.Size, GetDataBaseColumnTypeForFieldDefType(loField.GetType()), false, loField.DefaultValue);
                    // and, we want to flag it as having been moved into the detail table
                    loField.IsRedefinedInDetailTable = true;
                }
            }

            // now add the collection of minumum standard fields that we expect for the Violation Sub-Table
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlIssueUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlMasterKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlOccurenceNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlVioCodeStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlVioXferCodeStr, 8, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlVioDescription1Str, 80, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlVioDescription2Str, 80, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlVioFineStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlVioLateFee1Str, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlVioLateFee2Str, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlVioLateFee3Str, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlVioQueryTypeStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlVioSelectStr, 80, TDataBaseColumnDataType.dbtString, false, "");

            // make sure the table has the fields we need
            VerifyFieldsInTableDef(loFieldList, loViolationTableDef);

            //{ Verify that we have the desired indexes }
            //{ We gotta make smaller index names cuz oracle has a small limit }
            //VerifyIndex( loViolationTableName, 'NDX' + pTableName + 'ST' + 'MK', 'MASTERKEY' );

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes...
            loViolationTableDef.PostDeserialize(this);
        }
#endif // !WindowsCE

    } // TVioStruct

    #endregion


    /// <summary>
    /// Summary description for TVoidStruct.
    /// </summary>
    public class TVoidStruct : TCiteDetailStruct
    {
        #region Properties and Members
        #endregion

        public TVoidStruct()
            : base()
        {
            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  

            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
#endif
            #endregion
        }
#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Override the base implementation in order to suppress the creation of a status table.
        /// </summary>
        protected override void VerifyStatusTable()
        {
            // Don't want a status table on void structs. Suppress it by having an empty routine.
        }
#endif

    }

    /// <summary>
    /// Summary description for TNotesStruct.
    /// </summary>

    public class TNotesStruct : TCiteDetailStruct
    {
        #region Properties and Members
        #endregion

        public TNotesStruct()
            : base()
        {
            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlNoteDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlNoteTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlNotesMemoStr, 500, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlDetailRecNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");

                // Add fields for binary data support (ie. Wave files, JPG, etc.)
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMultimediaNoteDataTypeStr, 30, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMultimediaNoteFileNameStr, 266, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMultimediaNoteDataStr, -1, TDataBaseColumnDataType.dbtBLOB, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMultimediaNoteFileDateStampStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMultumediaNoteFileTimeStampStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                //Ayman S. Oct/2012: Added support for SDSU project
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlPrintedImageOrderNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
                 
                // We also need ability to export.
                AddStandardExportFields(fHostOnlyStandardFields);//krh-add
            }
#endif
            #endregion
        }

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Host only post-post deserialization. Adds metadata info that exists on host sideonly
        /// (table columns & indexes, primarily)
        /// </summary>
        protected override void FinalizeHostSideClientConfig()
        {

            // do inherited version first.
            base.FinalizeHostSideClientConfig();

            VerifyFieldsInTableDef(fHostOnlyStandardFields, this.MainTable);
        }
#endif // !WindowsCE
    }

    /// <summary>
    /// Summary description for TReissueStruct.
    /// </summary>

    public class TReissueStruct : TCiteDetailStruct
    {
        #region Properties and Members
        #endregion

        public TReissueStruct()
            : base()
        {
            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddStandardExportFields(fHostOnlyStandardFields);
            }
#endif
            #endregion

        }

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Host only post-post deserialization. Adds metadata info that exists on host sideonly
        /// (table columns & indexes, primarily)
        /// </summary>
        protected override void FinalizeHostSideClientConfig()
        {

            // do inherited version first.
            base.FinalizeHostSideClientConfig();

            VerifyFieldsInTableDef(fHostOnlyStandardFields, this.MainTable);
        }
#endif // !WindowsCE

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Specialty validation routine for ReIssue tables
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loReIssueTableDef = GetOrCreateSpecialtyTableDef("" /* this name is included in the actual struct name DBConstants.cnReIssueTableNameSuffix*/);

            // Create list of standard fields we need for this table 
            List<THostRequiredFieldInfoObj> loFieldList = new List<THostRequiredFieldInfoObj>();
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlMasterKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueMasterKeyStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueNumberPrefixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueNumberSuffixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlIssueOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlIssueOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");

            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlNotesMemoStr, 500, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecCreationDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecCreationTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlTableRevisionNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");


            // make sure the table has the fields we need
            VerifyFieldsInTableDef(loFieldList, loReIssueTableDef);

            //{ Verify that we have the desired indexes }
            //VerifyIndex( loReissueTableName, 'NDX_' + loReissueTableName + '_MK', 'MASTERKEY' );
            //VerifyIndex( loReissueTableName, 'NDX_' + loReissueTableName + '_SRCISSNO', 'SRCISSUENO' );

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes for UniqueKey, MaskerKey, etc...
            loReIssueTableDef.PostDeserialize(this);
        }
#endif // !WindowsCE

    } // TReissueStruct

    /// <summary>
    /// Summary description for TCancelStruct.
    /// </summary>

    public class TCancelStruct : TCiteDetailStruct
    {
        #region Properties and Members
        #endregion

        public TCancelStruct()
            : base()
        {
            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            // Add standard collections of fields 
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddStandardExportFields(fHostOnlyStandardFields);
                AddStandardOfficerFields(fHostOnlyStandardFields);

                // Add additional fields needed for cancellations
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSourceIssueNumberPrefixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSourceIssueNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSourceIssueNumberSuffixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSourceIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSourceIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");

                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlTableRevisionNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlRecCreationDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");

                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlCancelReasonStr, 80, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlTableRevisionNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlRecoveredByHHStr, 1, TDataBaseColumnDataType.dbtString, false, "");
            }
#endif
            #endregion
        } // constuctor
        /// <summary>
        /// Specialty validation routine for Cancellation tables
        /// </summary>
#if !WindowsCE && !__ANDROID__  
        protected override void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loCancelTableDef = GetOrCreateSpecialtyTableDef("" /* this name is included in the struct name DBConstants.cnCancelTableNameSuffix*/ );

            // Create list of standard fields we need for this table 
            List<THostRequiredFieldInfoObj> loFieldList = new List<THostRequiredFieldInfoObj>();
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlMasterKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueNumberPrefixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueNumberSuffixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlIssueOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlIssueOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlCancelReasonStr, 80, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecCreationDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlTableRevisionNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");

            // make sure the table has the fields we need
            VerifyFieldsInTableDef(loFieldList, loCancelTableDef);

            //{ Verify that we have the desired indexes }
            //VerifyIndex( loCancelTableName, 'NDX_' + loCancelTableName + '_SRCISSNO', 'SRCISSUENO' );

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes for UniqueKey, MaskerKey, etc...
            loCancelTableDef.PostDeserialize(this);
        } // VerifyHostTable
#endif
    }//TCancelStruct

    /// <summary>
    /// Summary description for TContinuanceStruct.
    /// </summary>

    public class TContinuanceStruct : TCiteDetailStruct
    {
        #region Properties and Members
        #endregion

        public TContinuanceStruct()
            : base()
        {
            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            // Add standard collections of fields 
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddStandardExportFields(fHostOnlyStandardFields);
                AddStandardOfficerFields(fHostOnlyStandardFields);

                // Add additional fields needed for continuances
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSourceIssueNumberPrefixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSourceIssueNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSourceIssueNumberSuffixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSourceIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSourceIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlStandardExportDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlTableRevisionNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlRecCreationTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlNotesMemoStr, 500, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMasterKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");
            }
#endif
            #endregion
        }
#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Specialty validation routine for Continuance tables
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loContinuanceTableDef = GetOrCreateSpecialtyTableDef("" /* this name is included in the struct type DBConstants.cnContinuanceTableNameSuffix */);

            // Create list of standard fields we need for this table 
            List<THostRequiredFieldInfoObj> loFieldList = new List<THostRequiredFieldInfoObj>();
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlMasterKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueNumberPrefixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueNumberSuffixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlSourceIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlIssueOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlIssueOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");

            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlNotesMemoStr, 500, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecCreationDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecCreationTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlTableRevisionNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");

            // make sure the table has the fields we need
            VerifyFieldsInTableDef(loFieldList, loContinuanceTableDef);

            //{ Verify that we have the desired indexes }
            // VerifyIndex( loContinuanceTableName, 'NDX_' + loContinuanceTableName + '_MK', 'MASTERKEY' );
            // VerifyIndex( loContinuanceTableName, 'NDX_' + loContinuanceTableName + '_SRCISSNO', 'SRCISSUENO' );

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes for UniqueKey, MaskerKey, etc...
            loContinuanceTableDef.PostDeserialize(this);
        }
#endif
    } // TContinuanceStruct


    /// <summary>
    /// Summary description for TGeoCodeStruct.
    /// </summary>

    public class TGeoCodeStruct : TCiteDetailStruct
    {
        #region Properties and Members
        #endregion

        public TGeoCodeStruct()
            : base()
        {
            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            // Add standard collections of fields 
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                // Add additional fields needed for GeoCodes
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlGeoCodeSourceLayerNameStr, 50, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlGeoCodeLabelStr, 50, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlGeoCodeValueStr, 50, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlGeoCodeResultMessageStr, 250, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlGeoCodeDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlGeoCodeTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");

                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlReinoRowVersionStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlStandardExportDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlTableRevisionNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlRecCreationTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMasterKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");

                AddStandardExportFields(fHostOnlyStandardFields);
            }
#endif
            #endregion
        }
#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Specialty validation routine for GeoCode tables
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loGeoCodeTableDef = GetOrCreateSpecialtyTableDef("" /* this name is included in the struct type DBConstants.cnGeoCodeTableNameSuffix */);

            // Create list of standard fields we need for this table 
            List<THostRequiredFieldInfoObj> loFieldList = new List<THostRequiredFieldInfoObj>();
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlMasterKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");

            // Add additional fields needed for GeoCodes
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlGeoCodeSourceLayerNameStr, 50, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlGeoCodeLabelStr, 50, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlGeoCodeValueStr, 50, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlGeoCodeResultMessageStr, 250, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlGeoCodeDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlGeoCodeTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");

            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlReinoRowVersionStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlStandardExportDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlTableRevisionNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecCreationTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecCreationDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");

            // make sure the table has the fields we need
            VerifyFieldsInTableDef(loFieldList, loGeoCodeTableDef);


            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes for UniqueKey, MaskerKey, etc...
            loGeoCodeTableDef.PostDeserialize(this);

            /*
             * 
             * if we add these constraints, then we can't add new records and merge the results back in
             * what we really need is for the edit to re-query the whole detail table and replace it
             * but for now we will drop the constraints so we can do the demo
             * 
            // we can't use the default constraint list - so we have to tell them what they are
            loGeoCodeTableDef.HighTableRevision.UniqueContraintFields.Clear();
            loGeoCodeTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMasterKeyStr);
            loGeoCodeTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlGeoCodeSourceLayerNameStr);
            loGeoCodeTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlGeoCodeLabelStr);
            loGeoCodeTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlGeoCodeValueStr);
             * */

            /*
            // create indexes
            TTableDef loTableDef = loGeoCodeTableDef;
            List<String> loTempIndex = new List<string>();
            string loIdxPrefix = "GEOCODE_";

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechanismForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechanismForeignKey, loTempIndex.ToArray(), false, false);
             */

        }
#endif
    } // TGeoCodeStruct

    // http://jirssb01:8080/browse/AUTOCITE-196
    public class TNSWStatusStruct : TIssStruct
    {
        #region Properties and Members
        #endregion

        public TNSWStatusStruct()
            : base()
        {
            // Not captured by handheld
            IsHandheldCaptured = false;

            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            // Add standard collections of fields 
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                // Add fields needed for NSWStatus
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueNumberPrefixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueNumberSuffixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, "TRANSACTIONSTATUS", 30, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, "TRANSACTIONSTATUSDATE", -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, "TRANSACTIONSTATUSTIME", -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, "OFFENCECODE", 20, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, "OFFENCEDATE", -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, "FINALSTATUS", 20, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, "PENALTYAMOUNT", -1, TDataBaseColumnDataType.dbtReal, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, "PROCESSINGFEE", -1, TDataBaseColumnDataType.dbtReal, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, "PAYMENTSRECEIVED", -1, TDataBaseColumnDataType.dbtReal, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, "ADJUSTMENTREASON", 20, TDataBaseColumnDataType.dbtString, false, "");
                AddStandardExportFields(fHostOnlyStandardFields);
            }
#endif
            #endregion
        }

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Specialty validation routine for NSW Status tables
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loNSWStatusTableDef = GetOrCreateSpecialtyTableDef("");

            // Create list of standard fields we need for this table 
            List<THostRequiredFieldInfoObj> loFieldList = new List<THostRequiredFieldInfoObj>();
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlIssueNumberPrefixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlIssueNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlIssueNumberSuffixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, "TRANSACTIONSTATUS", 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, "TRANSACTIONSTATUSDATE", -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, "TRANSACTIONSTATUSTIME", -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, "OFFENCECODE", 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, "OFFENCEDATE", -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, "FINALSTATUS", 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, "PENALTYAMOUNT", -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, "PROCESSINGFEE", -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, "PAYMENTSRECEIVED", -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, "ADJUSTMENTREASON", 20, TDataBaseColumnDataType.dbtString, false, "");
            AddStandardExportFields(loFieldList);

            // make sure the table has the fields we need
            VerifyFieldsInTableDef(loFieldList, loNSWStatusTableDef);

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes for UniqueKey, MaskerKey, etc...
            loNSWStatusTableDef.PostDeserialize(this);

            // create indexes
            List<String> loTempIndex = new List<string>();
            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueNumberStr);
            loTempIndex.Add("TRANSACTIONSTATUSDATE");
            loTempIndex.Add("TRANSACTIONSTATUSTIME");
            new TTableIndex(loNSWStatusTableDef, "IDX_NSWSTATUS_INFRINGEMENT", loTempIndex.ToArray(), false, false);
        }
#endif
    } // TNSWStatusStruct

// //////////////////

    /// <summary>
    /// Summary description for TMarkModeStruct.
    /// </summary>

    public class TMarkModeStruct : TSearchStruct
    {
        #region Properties and Members
        #endregion

        public TMarkModeStruct()
            : base()
        {
            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            // Add standard collections of fields 
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddStandardIssueFields(fHostOnlyStandardFields);
                AddStandardExportFields(fHostOnlyStandardFields);
                AddStandardOfficerFields(fHostOnlyStandardFields);
                AddStandardLocationFields(fHostOnlyStandardFields);
                AddStandardVehicleFields(fHostOnlyStandardFields);

                // Add additional fields needed for markmode
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlTireStemsRearTimeStr, 5, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlTireStemsFrontTimeStr, 5, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLocMeterNumberStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            }
#endif
            #endregion

        }
#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Host only post-post deserialization. Adds metadata info that exists on host sideonly
        /// (table columns & indexes, primarily)
        /// </summary>
        protected override void FinalizeHostSideClientConfig()
        {

            // do inherited version first.
            base.FinalizeHostSideClientConfig();

            VerifyFieldsInTableDef(fHostOnlyStandardFields, this.MainTable);

            // if this struct has location info, add the geocode detail table
            AddGeoCodeSubTableIfRequired();
        }
#endif // !WindowsCE
    }

    /// <summary>
    /// Summary description for THotDispoStruct.
    /// </summary>

    public class THotDispoStruct : TIssStruct
    {
        #region Properties and Members
        #endregion

        public THotDispoStruct()
            : base()
        {
            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            // Add standard collections of fields 
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddStandardIssueFields(fHostOnlyStandardFields);
                AddStandardOfficerFields(fHostOnlyStandardFields);
                AddStandardLocationFields(fHostOnlyStandardFields);
                AddStandardVehicleFields(fHostOnlyStandardFields);

                // Add additional fields needed for Hot Dispositions
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlHotCodeStr, 10, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlHotDispositionStr, 10, TDataBaseColumnDataType.dbtString, false, "");

                // HotSheetDispo needs to be exportable too (Los Angeles OMS export uses it, and other customers
                // may have a need in the future also)
                AddStandardExportFields(fHostOnlyStandardFields);
            }
#endif
            #endregion

        }
#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Host only post-post deserialization. Adds metadata info that exists on host sideonly
        /// (table columns & indexes, primarily)
        /// </summary>
        protected override void FinalizeHostSideClientConfig()
        {

            // do inherited version first.
            base.FinalizeHostSideClientConfig();

            VerifyFieldsInTableDef(fHostOnlyStandardFields, this.MainTable);

            // if this struct has location info, add the geocode detail table
            AddGeoCodeSubTableIfRequired();
        }
#endif // !WindowsCE
    } // THotDispoStruct

    //Sharan 2007.09.12 begin
    //Added PermitRFIDSearchLog Struct to Support the Symbol MC9090-G RFID 
    /// <summary>
    /// Summary description for TPermitRFIDSrchLogStruct .
    /// </summary>
    public class TPermitRFIDSrchLogStruct : TIssStruct
    {
        #region Properties and Members
        [XmlIgnoreAttribute]
        public TVoidStruct VoidStruct = null;
        [XmlIgnoreAttribute]
        public TCancelStruct CancelStruct = null;
        [XmlIgnoreAttribute]
        public TNotesStruct NotesStruct = null;
        [XmlIgnoreAttribute]
        public TReissueStruct ReissueStruct = null;
        [XmlIgnoreAttribute]
        public TContinuanceStruct ContinuanceStruct = null;

        // When this is set to true the TPermitRFIDSrchLogStruct can export its data
        // to the handheld (using a DAT file).
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool UploadToHandheld = false;

#if !WindowsCE && !__ANDROID__  
        [HostSideOnly] // Only used on the host-side
        public int ViolationCount = 1; // Must default to 1 in order to match behavior of layout tool

        // These are the "reason" list names that are used by inquiry screen. Only need on host side.
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        [HostSideOnly] // Only used on the host-side
        public string VoidReasonList = "";

        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        [HostSideOnly] // Only used on the host-side
        public string ReinstateReasonList = "";
#endif

        #endregion

        public TPermitRFIDSrchLogStruct()
            : base()
        {
            IsHandheldCaptured = false;
            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            // Add standard collections of fields 
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddStandardIssueFields(fHostOnlyStandardFields);
                AddStandardTicketFields(fHostOnlyStandardFields);
                AddStandardExportFields(fHostOnlyStandardFields);
                AddStandardOfficerFields(fHostOnlyStandardFields);
                AddStandardLocationFields(fHostOnlyStandardFields);
                AddStandardVehicleFields(fHostOnlyStandardFields);
                AddStandardEnhancedVehicleFields(fHostOnlyStandardFields);

                // some addtional fields for all citation issuance structures 
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVehPermitNumberStr, 14, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterNumberStr, 10, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIsWarningStr, 1, TDataBaseColumnDataType.dbtString, false, "N");
            }
#endif
            #endregion
        }

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Host only post-post deserialization. Adds metadata info that exists on host sideonly
        /// (table columns & indexes, primarily)
        /// </summary>
        protected override void FinalizeHostSideClientConfig()
        {

            // do inherited version first.
            base.FinalizeHostSideClientConfig();

            VerifyFieldsInTableDef(fHostOnlyStandardFields, this.MainTable);


            // if this struct has location info, add the geocode detail table
            bool loHasLocationInfo = (this.MainTable.GetFldNo(DBConstants.sqlLatitudeDegreesStr) > 0);
            if ((loHasLocationInfo == true) && (ParentStructMgr[this.Name + DBConstants.cnGeoCodeTableNameSuffix] == null))
            {
                // create a geocode structure for ourselves
                TGeoCodeStruct loGeoCodeStruct = new TGeoCodeStruct();
                loGeoCodeStruct.Name = this.Name + DBConstants.cnGeoCodeTableNameSuffix;
                loGeoCodeStruct.Parent = this.Parent;
                loGeoCodeStruct.ParentStruct = this.Name;
                loGeoCodeStruct.IsHostSideDefinitionOnly = true;
                // add it to the strutures list so we know about it - 
                // also, it will get PostDerialized called to initialize it
                ((TIssStructMgr)(this.Parent)).IssStructs.Add(loGeoCodeStruct);
            }

        }

        protected override void VerifyHostTable()
        {

            /// We should verify the Violation table if there is supposed to be at least one violation 
            if ((ViolationCount > 0) && (ParentStructMgr[this.Name + DBConstants.cnViolationTableNameSuffix] == null))
            {
                // create a violation structure for ourselves
                TVioStruct loVioStruct = new TVioStruct();
                loVioStruct.Name = this.Name + DBConstants.cnViolationTableNameSuffix;
                loVioStruct.Parent = this.Parent;
                loVioStruct.ParentStruct = this.Name;
                loVioStruct.IsHostSideDefinitionOnly = true;
                // add it to the strutures list so we know about it - 
                // also, it will get PostDerialized called to initialize it
                ((TIssStructMgr)(this.Parent)).IssStructs.Add(loVioStruct);
            }


        }

#endif // !WindowsCE

    } // TPermitRFIDSrchLogStruct
    //Sharan 2007.09.12 end

    /// <summary>
    /// Summary description for TPublicContactStruct.
    /// </summary>

    public class TPublicContactStruct : TSearchStruct
    {
        #region Properties and Members
        #endregion

        public TPublicContactStruct()
            : base()
        {
            #region Code to add Host Side-Only Standard Fields
#if !WindowsCE && !__ANDROID__  
            // we only want to do this on the initial construction, not on re-serialization
            // the fStructFinalized flag is set during PostDeserialization
            // Add standard collections of fields 
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddStandardIssueFields(fHostOnlyStandardFields);
                AddStandardExportFields(fHostOnlyStandardFields);
                AddStandardOfficerFields(fHostOnlyStandardFields);
                AddStandardLocationFields(fHostOnlyStandardFields);

                // Add additional fields needed for public contact
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlActionTakenStr, 80, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlActionTakenStructStr, 20, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlActionTakenFormStr, 20, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSearchedStr, 5, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSearchTypeStr, 20, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlConsentToSearchStr, 30, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlResidentStr, 20, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlStopReasonStr, 60, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlStopTypeStr, 30, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlContrabandStr, 5, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlContrabandTypeStr, 20, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSuspAgeStr, 20, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlDLBirthDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSuspRaceStr, 20, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSuspGenderStr, 20, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUnitSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSourceIssueNumberPrefixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSourceIssueNumberStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlSourceIssueNumberSuffixStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            }
#endif
            #endregion

        }
#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Host only post-post deserialization. Adds metadata info that exists on host sideonly
        /// (table columns & indexes, primarily)
        /// </summary>
        protected override void FinalizeHostSideClientConfig()
        {

            // do inherited version first.
            base.FinalizeHostSideClientConfig();

            VerifyFieldsInTableDef(fHostOnlyStandardFields, this.MainTable);

            // if this struct has location info, add the geocode detail table
            AddGeoCodeSubTableIfRequired();
        }
#endif // !WindowsCE
    }

    /// <summary>
    /// Summary description for TIssMenuItem.
    /// </summary>

    public class TIssMenuItem : TObjBase
    {
        #region Properties and Members
        protected string _MenuText = "";
        public string MenuText
        {
            get { return _MenuText; }
            set { _MenuText = value; }
        }

        protected string _HotKey = "";
        public string HotKey
        {
            get { return _HotKey; }
            set { _HotKey = value; }
        }

        protected string _ParentMenuName = "";
        public string ParentMenuName
        {
            get { return _ParentMenuName; }
            set { _ParentMenuName = value; }
        }

        #endregion

        public TIssMenuItem()
            : base()
        {
        }
    }

    /// <summary>
    /// TMeterTrax_InventoryStruct
    /// Abstruct struct class for MeterTrax inventory struct types to descend from
    /// </summary>
    public class TMeterTrax_InventoryStruct : TIssStruct
    {
        public TMeterTrax_InventoryStruct()
            : base()
        {
            #region Host-Side Only Code Section
#if !WindowsCE && !__ANDROID__  
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddStandardExportFields(fHostOnlyStandardFields);
            }
#endif
            #endregion
        }
    }



    /// <summary>
    /// TMeterTrax_TransactionStruct
    /// Abstruct struct class for MeterTrax transaction struct types to descend from
    /// </summary>
    public class TMeterTrax_TransactionStruct : TIssStruct
    {
        public TMeterTrax_TransactionStruct()
            : base()
        {
            #region Host-Side Only Code Section
#if !WindowsCE && !__ANDROID__  
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                AddStandardExportFields(fHostOnlyStandardFields);
                /*
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVoidStatusStr, 2, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVoidStatusDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVoidReasonStr, 80, TDataBaseColumnDataType.dbtString, false, "");
                AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVoidedInFieldStr, 1, TDataBaseColumnDataType.dbtString, false, "N");
                 * */
            }
#endif
            #endregion
        }
    }



    /// <summary>
    /// MeterTrax field transactions table
    /// A temporary holding table where field transactions are stored before they 
    /// are sorted and moved into their appropriate category table
    /// </summary>
    public class TMeterTrax_FieldTransactionTempStruct : TMeterTrax_TransactionStruct
    {

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Override the base implementation in order to suppress the creation of a status table.
        /// </summary>
        protected override void VerifyStatusTable()
        {
            // Don't want a status table on the field transaction temp struct. Suppress it by having an empty routine.
        }
#endif

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Specialty validation routine for metertrax tables 
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loMeterTrax_FieldTransactionTempStructTableDef = GetOrCreateSpecialtyTableDef("" /* the name is derived from the struct name*/ );


            // this is a special column defined to allow us to work-around the limitations of the uniqueconstraints
            // we're not allowed to have a time only field in the uniqueconstraint list (it gets ignored in the datamanager)
            // but we need it to satisfy the uniqueconstraint requirements in the temp table
            // so we define this column that contains the combo issuedate and issuetime in one column
            // that we can add as a unique constraint and allow us to function with the existing restrictions
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_TransactionDateTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");


            // the MeterTrax tables are statically defined in totallity - if they exist, this will be their struct
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUnitSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_DeviceModeStr, 16, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechTypeIDStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ZoneIDStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ZoneDescStr, 60, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LocationIDStr, 50, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RateProgramIDStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_DoorLockIDStr, 10, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechLockIDStr, 10, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ActiveInventoryStr, 1, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OperableStr, 1, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RateProgramSuccessStr, 1, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LocationGeneratedStr, 1, TDataBaseColumnDataType.dbtString, false, "");




            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_TransactionTypeStr, 15, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CollectionRouteIDStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CollectionRouteSequenceStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MaintenanceRouteIDStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MaintenanceRouteSequenceStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");


            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_AuditAmountCashStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_AuditAmountDebitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RegisterReadCashStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RegisterReadDebitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_AuditDeviceResetStr, 5, TDataBaseColumnDataType.dbtString, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OutageCodeIDStr, 3, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OutageCodeDescriptionStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OutageFixedStr, 1, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OutagePriorityStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RepairCodeIDStr, 3, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RepairCodeDescriptionStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechReplacedStr, 1, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechSerialNewStr, 12, TDataBaseColumnDataType.dbtString, false, "");


            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_EagleErrorCodeStr, 8, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_BatteryVoltageStr, 8, TDataBaseColumnDataType.dbtString, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlRemark1Str, 250, TDataBaseColumnDataType.dbtString, false, "");

            // added for vehicle tracking 
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVehLicNoStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVehLicStateStr, 7, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVehPlateTypeStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            
            // not in the standard field transactions, but used for legacy inventory conversion
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RevenueCategoryIDStr, 8, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ActivatedStr, 1, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLocLotStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLocBlockStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLocStreetStr, 60, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLocSideOfStreetStr, 10, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLatitudeDegreesStr, DBConstants.cnLatitudeOrLongitudeColumnPrecision, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLongitudeDegreesStr, DBConstants.cnLatitudeOrLongitudeColumnPrecision, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastAuditDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastCollectionDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastOpCheckDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastOutageDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastRepairDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastProgramDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastInventoryDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RevenueCurrentCashStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RevenuePeriodToDateCashStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RevenueCurrentDebitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RevenuePeriodToDateDebitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OccupancyPercentCurrentStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OccupancyPercentPeriodToDateStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_AverageAuditCashStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_AverageAuditDebitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RevenueAverageStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RevenueMaxWeeklyStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            // audit transaction coin data
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CoinsRejectedStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CashKeyCardTransactionCountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ExternalDeviceTransactionCountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ExternalDeviceTransactionAmountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CoinValueScaleStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CashKeyCard01TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin01TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin02TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin03TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin04TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin05TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin06TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin07TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin08TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin09TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin10TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin11TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin12TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin13TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin14TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin15TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin16TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin17TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin18TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin19TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin20TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CashKeyCard01ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin01ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin02ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin03ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin04ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin05ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin06ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin07ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin08ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin09ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin10ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin11ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin12ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin13ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin14ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin15ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin16ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin17ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin18ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin19ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin20ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin01CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin02CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin03CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin04CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin05CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin06CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin07CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin08CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin09CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin10CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin11CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin12CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin13CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin14CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin15CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin16CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin17CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin18CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin19CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin20CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");


            // a few bits for when a transaction is rejected 
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ExceptionCorrectedStr, 1, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ExceptionReasonTextStr, 80, TDataBaseColumnDataType.dbtString, false, "");

            // make sure the table has the fields we need
            VerifyFieldsInTableDef(fHostOnlyStandardFields, loMeterTrax_FieldTransactionTempStructTableDef);


            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes...
            loMeterTrax_FieldTransactionTempStructTableDef.PostDeserialize(this);

            // we can't use the default constraint list, as multiple exception transactions can be generated 
            // from imports and re-tries that have the same date/time stamp - so we have to tell them what they are
            loMeterTrax_FieldTransactionTempStructTableDef.HighTableRevision.UniqueContraintFields.Clear();
            loMeterTrax_FieldTransactionTempStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_TransactionTypeStr);
            loMeterTrax_FieldTransactionTempStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            loMeterTrax_FieldTransactionTempStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueDateStr);
            loMeterTrax_FieldTransactionTempStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueTimeStr);
            loMeterTrax_FieldTransactionTempStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueOfficerIDStr);
            loMeterTrax_FieldTransactionTempStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_MechSerialStr);
            loMeterTrax_FieldTransactionTempStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_LocationIDStr);

            // create indexes
            TTableDef loTableDef = loMeterTrax_FieldTransactionTempStructTableDef;
            List<String> loTempIndex = new List<string>();
            string loIdxPrefix = "MTX_TRANSTMP_";

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_LocationIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_LocationIDStr, loTempIndex.ToArray(), false, false);


            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechSerialStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechSerialStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlUnitSerialStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlUnitSerialStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueOfficerIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueOfficerIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_TransactionDateTimeStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueDateStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueDateStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueTimeStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_RateProgramIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_RateProgramIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_CollectionRouteIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_CollectionRouteIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_ZoneIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_ZoneIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MaintenanceRouteIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MaintenanceRouteIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlLocStreetStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlLocStreetStr, loTempIndex.ToArray(), false, false);
        }
#endif // !WindowsCE

    }



    /// <summary>
    /// MeterTrax Inventory Active table
    /// </summary>
    public class TMeterTrax_PoleLocationStruct : TMeterTrax_InventoryStruct
    {

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Specialty validation routine for metertrax tables 
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loMeterTrax_PoleLocationStructTableDef = GetOrCreateSpecialtyTableDef("" /* the name is derived from the struct name*/ );

            // the MeterTrax tables are statically defined in totallity - if they exist, this will be their struct
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LocationIDStr, 50, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_InstallationDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechLockIDStr, 10, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_DoorLockIDStr, 10, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RevenueCategoryIDStr, 8, TDataBaseColumnDataType.dbtString, false, "");             // unsure of this one's use... can think of a few though
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ActivatedStr, 1, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LocationGeneratedStr, 1, TDataBaseColumnDataType.dbtString, false, "");


            // earlier mtx versions had less verbose location descriptions... we've expanded them to match the issuance fields
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ZoneIDStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ZoneDescStr, 60, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLocLotStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLocBlockStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLocStreetStr, 60, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLocDescriptorStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLocSideOfStreetStr, 10, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLatitudeDegreesStr, DBConstants.cnLatitudeOrLongitudeColumnPrecision, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLongitudeDegreesStr, DBConstants.cnLatitudeOrLongitudeColumnPrecision, TDataBaseColumnDataType.dbtReal, false, "");

            // special event tags for customized detail reporting
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_SpecialEventTag1Str, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_SpecialEventTag2Str, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_SpecialEventTag3Str, 20, TDataBaseColumnDataType.dbtString, false, "");

            // collection routes/maintenance routes
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CollectionRouteIDStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CollectionRouteSequenceStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MaintenanceRouteIDStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MaintenanceRouteSequenceStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastAuditDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastCollectionDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastOpCheckDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastOutageDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastRepairDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastProgramDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastInventoryDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");


            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RevenueCurrentCashStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RevenuePeriodToDateCashStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RevenueCurrentDebitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RevenuePeriodToDateDebitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OccupancyPercentCurrentStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OccupancyPercentPeriodToDateStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_AverageAuditCashStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_AverageAuditDebitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RevenueAverageStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RevenueMaxWeeklyStr, -1, TDataBaseColumnDataType.dbtReal, false, "");


            // make sure the table has the fields we need
            VerifyFieldsInTableDef(fHostOnlyStandardFields, loMeterTrax_PoleLocationStructTableDef);

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes...
            loMeterTrax_PoleLocationStructTableDef.PostDeserialize(this);

            if (loMeterTrax_PoleLocationStructTableDef.HighTableRevision.UniqueContraintFields.Count == 0)
            {
                // our unique constraint list won't get filled by the defaults, we have to tell them what they are
                loMeterTrax_PoleLocationStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_LocationIDStr);
            }

            // create indexes
            TTableDef loTableDef = loMeterTrax_PoleLocationStructTableDef;
            List<String> loTempIndex = new List<string>();
            string loIdxPrefix = "MTX_LOC_";

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_CollectionRouteIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_CollectionRouteIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_ZoneIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_ZoneIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MaintenanceRouteIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MaintenanceRouteIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlLocStreetStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlLocStreetStr, loTempIndex.ToArray(), false, false);

        }
#endif // !WindowsCE
    }

    /// <summary>
    /// MeterTrax Meter Mechanism table
    /// </summary>
    public class TMeterTrax_MeterMechanismStruct : TMeterTrax_InventoryStruct
    {

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Specialty validation routine for metertrax tables 
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loMeterTrax_MechTableDef = GetOrCreateSpecialtyTableDef("" /* the name is derived from the struct name*/ );


            // the MeterTrax tables are statically defined in totallity - if they exist, this will be their struct
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");

            // reference to the meter pole location record, set when mech is when installed, null when a defined as a spare mech
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_PoleLocationForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechTypeIDStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RateProgramIDStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OperableStr, 1, TDataBaseColumnDataType.dbtString, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RegisterReadCashStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RegisterReadDebitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastAuditDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastCollectionDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastOpCheckDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastOutageDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastRepairDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastProgramDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LastInventoryDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");

            // some extra info about the mechs to help us in debugging hardware/pgm conflicts
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechPCBBoardNumberStr, 8, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechPCBAssemblyRevStr, 8, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechPCBManufVendorIDStr, 8, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechPCBManufWeekStr, 8, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechPCBManufYearStr, 8, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechPCBSoftwareRevStr, 50, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechPCBPaperClipValStr, 8, TDataBaseColumnDataType.dbtString, false, "");

            // make sure the table has the fields we need
            VerifyFieldsInTableDef(fHostOnlyStandardFields, loMeterTrax_MechTableDef);

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes...
            loMeterTrax_MechTableDef.PostDeserialize(this);

            // our unique constraint list won't get filled by the defaults, we have to tell them what they are
            loMeterTrax_MechTableDef.HighTableRevision.UniqueContraintFields.Clear();
            loMeterTrax_MechTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_MechSerialStr);
            loMeterTrax_MechTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_MechTypeIDStr);


            // create indexes
            TTableDef loTableDef = loMeterTrax_MechTableDef;
            List<String> loTempIndex = new List<string>();
            string loIdxPrefix = "MTX_MECH_";

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechTypeIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechTypeIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_RateProgramIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_RateProgramIDStr, loTempIndex.ToArray(), false, false);

        }
#endif // !WindowsCE

    }


    /// <summary>
    /// MeterTrax Installed Location History  table
    /// </summary>
    public class TMeterTrax_MeterInstallationHistory : TMeterTrax_InventoryStruct
    {

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Override the base implementation in order to suppress the creation of a status table.
        /// </summary>
        protected override void VerifyStatusTable()
        {
            // Don't want a status table on mech installation history. Suppress it by having an empty routine.
        }

        /// <summary>
        /// Specialty validation routine for metertrax tables 
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loMeterTrax_MeterInstallationHistoryTableDef = GetOrCreateSpecialtyTableDef("" /* the name is derived from the struct name*/ );

            // the MeterTrax tables are statically defined in totallity - if they exist, this will be their struct
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");

            // install date
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_TransactionDateTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");

            // reference to the pole location structure 
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_PoleLocationForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");
            // the installed meter 
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechanismForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");

            // flag for system generated entries
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_SystemGeneratedStr, 1, TDataBaseColumnDataType.dbtString, false, "");


            // make sure the table has the fields we need
            VerifyFieldsInTableDef(fHostOnlyStandardFields, loMeterTrax_MeterInstallationHistoryTableDef);

            // We must do the PostDeserialize ourself 
            loMeterTrax_MeterInstallationHistoryTableDef.PostDeserialize(this);

            // we can't use the default constraint list - so we have to tell them what they are
            loMeterTrax_MeterInstallationHistoryTableDef.HighTableRevision.UniqueContraintFields.Clear();
            loMeterTrax_MeterInstallationHistoryTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            loMeterTrax_MeterInstallationHistoryTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_MechanismForeignKey);


            // create indexes
            TTableDef loTableDef = loMeterTrax_MeterInstallationHistoryTableDef;
            List<String> loTempIndex = new List<string>();
            string loIdxPrefix = "MTX_LOCHIST_";

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_PoleLocationForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_PoleLocationForeignKey, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechanismForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechanismForeignKey, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_TransactionDateTimeStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueDateStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueDateStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueTimeStr, loTempIndex.ToArray(), false, false);

        }
#endif // !WindowsCE

    }

    /// <summary>
    /// MeterTrax audit transactions table
    /// </summary>
    public class TMeterTrax_AuditTransactionStruct : TMeterTrax_TransactionStruct
    {

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Specialty validation routine for metertrax tables 
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loMeterTrax_AuditTransactionStructTableDef = GetOrCreateSpecialtyTableDef("" /* the name is derived from the struct name*/ );


            // this is a special column defined to allow us to work-around the limitations of the uniqueconstraints
            // we're not allowed to have a time only field in the uniqueconstraint list (it gets ignored in the datamanager)
            // but we need it to satisfy the uniqueconstraint requirements in the temp table
            // so we define this column that contains the combo issuedate and issuetime in one column
            // that we can add as a unique constraint and allow us to function with the existing restrictions
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_TransactionDateTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");

            // the MeterTrax tables are statically defined in totallity - if they exist, this will be their struct

            // reference to the meter mechanism structure - but may not be available until an exception is resolved
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechanismForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");
            // reference to the meter pole location record, if any, that was associate with the mech at the time the transaction was generated
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_PoleLocationForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUnitSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_DeviceModeStr, 16, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_AuditAmountCashStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_AuditAmountDebitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RegisterReadCashStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RegisterReadDebitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_AuditDeviceResetStr, 5, TDataBaseColumnDataType.dbtString, false, "");

            // added to track battery level trends over time
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_EagleErrorCodeStr, 8, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_BatteryVoltageStr, 8, TDataBaseColumnDataType.dbtString, false, "");

            // added to help detect out-of-whack inventories
              //   * removed collectionrouteid from posted audtrans table because of report conflicts... 
               //  * do we need to actually STORE it to utilize the collected data?
            //AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CollectionRouteIDStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            //AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CollectionRouteSequenceStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");


            // added to hold the 'original' audit information for non-bonafide audits 
            // that get adjusted when the next bonafide audit shows gets posted
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_FieldRepairOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_FieldRepairAuditDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_FieldRepairAuditTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");

            // added to indicate when transactions are processed retroactively, after inventory has already been updated and transaction is not against current inventory state
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ProcessedOutofOrderStr, 1, TDataBaseColumnDataType.dbtString, false, "");


            // audit coin transaction data
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CoinsRejectedStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CashKeyCardTransactionCountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ExternalDeviceTransactionCountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ExternalDeviceTransactionAmountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CoinValueScaleStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CashKeyCard01TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin01TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin02TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin03TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin04TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin05TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin06TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin07TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin08TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin09TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin10TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin11TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin12TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin13TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin14TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin15TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin16TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin17TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin18TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin19TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin20TimeUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CashKeyCard01ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin01ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin02ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin03ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin04ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin05ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin06ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin07ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin08ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin09ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin10ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin11ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin12ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin13ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin14ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin15ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin16ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin17ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin18ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin19ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin20ValueUnitStr, -1, TDataBaseColumnDataType.dbtReal, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin01CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin02CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin03CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin04CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin05CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin06CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin07CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin08CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin09CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin10CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin11CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin12CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin13CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin14CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin15CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin16CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin17CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin18CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin19CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_Coin20CountStr, -1, TDataBaseColumnDataType.dbtReal, false, "");


            // not stored: use the pole key to find this: AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LocationIDStr, 50, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlRemark1Str, 250, TDataBaseColumnDataType.dbtString, false, "");

            // make sure the table has the fields we need
            VerifyFieldsInTableDef(fHostOnlyStandardFields, loMeterTrax_AuditTransactionStructTableDef);

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes...
            loMeterTrax_AuditTransactionStructTableDef.PostDeserialize(this);


            // we can't use the default constraint list - so we have to tell them what they are
            loMeterTrax_AuditTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Clear();
            loMeterTrax_AuditTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            loMeterTrax_AuditTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueDateStr);
            loMeterTrax_AuditTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueTimeStr);
            loMeterTrax_AuditTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueOfficerIDStr);
            loMeterTrax_AuditTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_MechSerialStr);
            loMeterTrax_AuditTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_DeviceModeStr);


            // create indexes
            TTableDef loTableDef = loMeterTrax_AuditTransactionStructTableDef;
            List<String> loTempIndex = new List<string>();
            string loIdxPrefix = "MTX_AUD_";

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechanismForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechanismForeignKey, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_PoleLocationForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_PoleLocationForeignKey, loTempIndex.ToArray(), false, false);


            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechSerialStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechSerialStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlUnitSerialStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlUnitSerialStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueOfficerIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueOfficerIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_TransactionDateTimeStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueDateStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueDateStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueTimeStr, loTempIndex.ToArray(), false, false);

        }
#endif // !WindowsCE

    }


    /// <summary>
    /// MeterTrax OpCheck Transactions table 
    /// </summary>
    public class TMeterTrax_OpCheckTransactionStruct : TMeterTrax_TransactionStruct
    {

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Specialty validation routine for metertrax tables 
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loMeterTrax_OpCheckTransactionTableDef = GetOrCreateSpecialtyTableDef("" /* the name is derived from the struct name*/ );


            // this is a special column defined to allow us to work-around the limitations of the uniqueconstraints
            // we're not allowed to have a time only field in the uniqueconstraint list (it gets ignored in the datamanager)
            // but we need it to satisfy the uniqueconstraint requirements in the temp table
            // so we define this column that contains the combo issuedate and issuetime in one column
            // that we can add as a unique constraint and allow us to function with the existing restrictions
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_TransactionDateTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");

            // the MeterTrax tables are statically defined in totallity - if they exist, this will be their struct
            // reference to the meter mechanism structure - but may not be available until an exception is resolved
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechanismForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");
            // reference to the meter pole location record, if any, that was associate with the mech at the time the transaction was generated
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_PoleLocationForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");


            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUnitSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            // not stored: use the pole key to find this: AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LocationIDStr, 50, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlRemark1Str, 250, TDataBaseColumnDataType.dbtString, false, "");
            ///
            /// a few extra bits of info about the Transaction
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_EagleErrorCodeStr, 8, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_BatteryVoltageStr, 8, TDataBaseColumnDataType.dbtString, false, "");
            /// 

            // added to indicate when transactions are processed retroactively, after inventory has already been updated and transaction is not against current inventory state
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ProcessedOutofOrderStr, 1, TDataBaseColumnDataType.dbtString, false, "");


            // make sure the table has the fields we need
            VerifyFieldsInTableDef(fHostOnlyStandardFields, loMeterTrax_OpCheckTransactionTableDef);

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes...
            loMeterTrax_OpCheckTransactionTableDef.PostDeserialize(this);

            // we can't use the default constraint list - so we have to tell them what they are
            loMeterTrax_OpCheckTransactionTableDef.HighTableRevision.UniqueContraintFields.Clear();
            loMeterTrax_OpCheckTransactionTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            loMeterTrax_OpCheckTransactionTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueDateStr);
            loMeterTrax_OpCheckTransactionTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueTimeStr);
            loMeterTrax_OpCheckTransactionTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueOfficerIDStr);
            loMeterTrax_OpCheckTransactionTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_MechSerialStr);

            // create indexes
            TTableDef loTableDef = loMeterTrax_OpCheckTransactionTableDef;
            List<String> loTempIndex = new List<string>();
            string loIdxPrefix = "MTX_OPC_";

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechanismForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechanismForeignKey, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_PoleLocationForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_PoleLocationForeignKey, loTempIndex.ToArray(), false, false);


            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechSerialStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechSerialStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlUnitSerialStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlUnitSerialStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueOfficerIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueOfficerIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_TransactionDateTimeStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueDateStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueDateStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueTimeStr, loTempIndex.ToArray(), false, false);

        }
#endif // !WindowsCE

    }


    /// <summary>
    /// MeterTrax Inventory Transaction table
    /// </summary>
    public class TMeterTrax_InventoryTransactionStruct : TMeterTrax_TransactionStruct
    {

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Specialty validation routine for metertrax tables 
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loMeterTrax_InventoryTransactionStructTableDef = GetOrCreateSpecialtyTableDef("" /* the name is derived from the struct name*/ );

            // the MeterTrax tables are statically defined in totallity - if they exist, this will be their struct

            // this is a special column defined to allow us to work-around the limitations of the uniqueconstraints
            // we're not allowed to have a time only field in the uniqueconstraint list (it gets ignored in the datamanager)
            // but we need it to satisfy the uniqueconstraint requirements in the temp table
            // so we define this column that contains the combo issuedate and issuetime in one column
            // that we can add as a unique constraint and allow us to function with the existing restrictions
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_TransactionDateTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");

            // reference to the meter mechanism structure - but may not be available until an exception is resolved
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechanismForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");

            // reference to the meter pole location record, if any, that was associate with the mech at the time the transaction was generated
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_PoleLocationForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUnitSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ZoneIDStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ZoneDescStr, 60, TDataBaseColumnDataType.dbtString, false, "");
            // not stored: use the pole key to find this: AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LocationIDStr, 50, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechTypeIDStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RateProgramIDStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_DoorLockIDStr, 10, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechLockIDStr, 10, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ActiveInventoryStr, 1, TDataBaseColumnDataType.dbtString, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLocLotStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLocBlockStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLocStreetStr, 60, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLocSideOfStreetStr, 10, TDataBaseColumnDataType.dbtString, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLatitudeDegreesStr, DBConstants.cnLatitudeOrLongitudeColumnPrecision, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlLongitudeDegreesStr, DBConstants.cnLatitudeOrLongitudeColumnPrecision, TDataBaseColumnDataType.dbtReal, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CollectionRouteIDStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_CollectionRouteSequenceStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MaintenanceRouteIDStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MaintenanceRouteSequenceStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");

            // added to indicate when transactions are processed retroactively, after inventory has already been updated and transaction is not against current inventory state
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ProcessedOutofOrderStr, 1, TDataBaseColumnDataType.dbtString, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlRemark1Str, 250, TDataBaseColumnDataType.dbtString, false, "");


            // make sure the table has the fields we need
            VerifyFieldsInTableDef(fHostOnlyStandardFields, loMeterTrax_InventoryTransactionStructTableDef);

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes...
            loMeterTrax_InventoryTransactionStructTableDef.PostDeserialize(this);

            // we can't use the default constraint list - so we have to tell them what they are
            loMeterTrax_InventoryTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Clear();
            loMeterTrax_InventoryTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            loMeterTrax_InventoryTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueDateStr);
            loMeterTrax_InventoryTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueTimeStr);
            loMeterTrax_InventoryTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueOfficerIDStr);
            loMeterTrax_InventoryTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_MechSerialStr);

            // create indexes
            TTableDef loTableDef = loMeterTrax_InventoryTransactionStructTableDef;
            List<String> loTempIndex = new List<string>();
            string loIdxPrefix = "MTX_INV_";

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechanismForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechanismForeignKey, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_PoleLocationForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_PoleLocationForeignKey, loTempIndex.ToArray(), false, false);


            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechSerialStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechSerialStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlUnitSerialStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlUnitSerialStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueOfficerIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueOfficerIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_TransactionDateTimeStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueDateStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueDateStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueTimeStr, loTempIndex.ToArray(), false, false);


        }
#endif // !WindowsCE
    }




    /// <summary>
    /// MeterTrax Outage Transaction table
    /// </summary>
    public class TMeterTrax_OutageTransactionStruct : TMeterTrax_TransactionStruct
    {

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Specialty validation routine for metertrax tables 
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loMeterTrax_OutageTransactionStructTableDef = GetOrCreateSpecialtyTableDef("" /* the name is derived from the struct name*/ );

            // the MeterTrax tables are statically defined in totallity - if they exist, this will be their struct

            // this is a special column defined to allow us to work-around the limitations of the uniqueconstraints
            // we're not allowed to have a time only field in the uniqueconstraint list (it gets ignored in the datamanager)
            // but we need it to satisfy the uniqueconstraint requirements in the temp table
            // so we define this column that contains the combo issuedate and issuetime in one column
            // that we can add as a unique constraint and allow us to function with the existing restrictions
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_TransactionDateTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");

            // reference to the meter mechanism structure - but may not be available until an exception is resolved
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechanismForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");

            // reference to the meter pole location record, if any, that was associate with the mech at the time the transaction was generated
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_PoleLocationForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUnitSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            // not stored: use the pole key to find this: AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LocationIDStr, 50, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OutageCodeIDStr, 3, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OutageCodeDescriptionStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OutageFixedStr, 1, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OutagePriorityStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlRemark1Str, 250, TDataBaseColumnDataType.dbtString, false, "");

            // added to help detect out-of-whack inventories
            //AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MaintenanceRouteIDStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            //AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MaintenanceRouteSequenceStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");

            // added for vehicle tracking 
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVehLicNoStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVehLicStateStr, 7, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVehPlateTypeStr, 12, TDataBaseColumnDataType.dbtString, false, "");

            // added to indicate when transactions are processed retroactively, after inventory has already been updated and transaction is not against current inventory state
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ProcessedOutofOrderStr, 1, TDataBaseColumnDataType.dbtString, false, "");

            // make sure the table has the fields we need
            VerifyFieldsInTableDef(fHostOnlyStandardFields, loMeterTrax_OutageTransactionStructTableDef);

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes...
            loMeterTrax_OutageTransactionStructTableDef.PostDeserialize(this);

            // we can't use the default constraint list, as multiple outages can be generated from mech error conditions
            // that have the same date/time stamp - so we have to tell them what they are
            loMeterTrax_OutageTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Clear();
            loMeterTrax_OutageTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            loMeterTrax_OutageTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueDateStr);
            loMeterTrax_OutageTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueTimeStr);
            loMeterTrax_OutageTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueOfficerIDStr);
            loMeterTrax_OutageTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_MechSerialStr);
            loMeterTrax_OutageTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_OutageCodeIDStr);


            // create indexes
            TTableDef loTableDef = loMeterTrax_OutageTransactionStructTableDef;
            List<String> loTempIndex = new List<string>();
            string loIdxPrefix = "MTX_OUT_";

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechanismForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechanismForeignKey, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_PoleLocationForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_PoleLocationForeignKey, loTempIndex.ToArray(), false, false);


            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechSerialStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechSerialStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlUnitSerialStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlUnitSerialStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueOfficerIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueOfficerIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_TransactionDateTimeStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueDateStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueDateStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueTimeStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_OutageCodeIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_OutageCodeIDStr, loTempIndex.ToArray(), false, false);


        }
#endif // !WindowsCE
    }




    /// <summary>
    /// MeterTrax RateProgram Transaction table
    /// </summary>
    public class TMeterTrax_RateProgramTransactionStruct : TMeterTrax_TransactionStruct
    {

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Specialty validation routine for metertrax tables 
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loMeterTrax_RateProgramTransactionStructTableDef = GetOrCreateSpecialtyTableDef("" /* the name is derived from the struct name*/ );


            // this is a special column defined to allow us to work-around the limitations of the uniqueconstraints
            // we're not allowed to have a time only field in the uniqueconstraint list (it gets ignored in the datamanager)
            // but we need it to satisfy the uniqueconstraint requirements in the temp table
            // so we define this column that contains the combo issuedate and issuetime in one column
            // that we can add as a unique constraint and allow us to function with the existing restrictions
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_TransactionDateTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");

            // the MeterTrax tables are statically defined in totallity - if they exist, this will be their struct
            // reference to the meter mechanism structure - but may not be available until an exception is resolved
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechanismForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");

            // reference to the meter pole location record, if any, that was associate with the mech at the time the transaction was generated
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_PoleLocationForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUnitSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            // not stored: use the pole key to find this: AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LocationIDStr, 50, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RateProgramIDStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RateProgramSuccessStr, 1, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlRemark1Str, 250, TDataBaseColumnDataType.dbtString, false, "");

            // added to indicate when transactions are processed retroactively, after inventory has already been updated and transaction is not against current inventory state
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ProcessedOutofOrderStr, 1, TDataBaseColumnDataType.dbtString, false, "");

            // make sure the table has the fields we need
            VerifyFieldsInTableDef(fHostOnlyStandardFields, loMeterTrax_RateProgramTransactionStructTableDef);

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes...
            loMeterTrax_RateProgramTransactionStructTableDef.PostDeserialize(this);

            // we can't use the default constraint list - so we have to tell them what they are
            loMeterTrax_RateProgramTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Clear();
            loMeterTrax_RateProgramTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            loMeterTrax_RateProgramTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueDateStr);
            loMeterTrax_RateProgramTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueTimeStr);
            loMeterTrax_RateProgramTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueOfficerIDStr);
            loMeterTrax_RateProgramTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_MechSerialStr);

            // create indexes
            TTableDef loTableDef = loMeterTrax_RateProgramTransactionStructTableDef;
            List<String> loTempIndex = new List<string>();
            string loIdxPrefix = "MTX_PRG_";

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechanismForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechanismForeignKey, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_PoleLocationForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_PoleLocationForeignKey, loTempIndex.ToArray(), false, false);


            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechSerialStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechSerialStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlUnitSerialStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlUnitSerialStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueOfficerIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueOfficerIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_TransactionDateTimeStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueDateStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueDateStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueTimeStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_RateProgramIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_RateProgramIDStr, loTempIndex.ToArray(), false, false);


        }
#endif // !WindowsCE
    }



    /// <summary>
    /// MeterTrax Repair Transaction table
    /// </summary>
    public class TMeterTrax_RepairTransactionStruct : TMeterTrax_TransactionStruct
    {

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// Specialty validation routine for metertrax tables 
        /// </summary>
        override protected void VerifyHostTable()
        {
            // retrieve the table definition
            TTableDef loMeterTrax_RepairTransactionStructTableDef = GetOrCreateSpecialtyTableDef("" /* the name is derived from the struct name*/ );

            // the MeterTrax tables are statically defined in totallity - if they exist, this will be their struct


            // this is a special column defined to allow us to work-around the limitations of the uniqueconstraints
            // we're not allowed to have a time only field in the uniqueconstraint list (it gets ignored in the datamanager)
            // but we need it to satisfy the uniqueconstraint requirements in the temp table
            // so we define this column that contains the combo issuedate and issuetime in one column
            // that we can add as a unique constraint and allow us to function with the existing restrictions
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_TransactionDateTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");

            // reference to the meter mechanism structure - but may not be available until an exception is resolved
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechanismForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");

            // reference to the meter pole location record, if any, that was associate with the mech at the time the transaction was generated
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_PoleLocationForeignKey, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlIssueTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlUnitSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            // not stored: use the pole key to find this: AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_LocationIDStr, 50, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechSerialStr, 12, TDataBaseColumnDataType.dbtString, false, "");

            // added to help detect out-of-whack inventories
            //AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MaintenanceRouteIDStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            //AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MaintenanceRouteSequenceStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");

            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RepairCodeIDStr, 3, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_RepairCodeDescriptionStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OutageCodeIDStr, 3, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_OutageCodeDescriptionStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechReplacedStr, 1, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_MechSerialNewStr, 12, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlRemark1Str, 250, TDataBaseColumnDataType.dbtString, false, "");

            // added for vehicle tracking 
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVehLicNoStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVehLicStateStr, 7, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlVehPlateTypeStr, 12, TDataBaseColumnDataType.dbtString, false, "");

            // added to indicate when transactions are processed retroactively, after inventory has already been updated and transaction is not against current inventory state
            AddHostSideRequiredFieldInfo(fHostOnlyStandardFields, DBConstants.sqlMeterTrax_ProcessedOutofOrderStr, 1, TDataBaseColumnDataType.dbtString, false, "");


            // make sure the table has the fields we need
            VerifyFieldsInTableDef(fHostOnlyStandardFields, loMeterTrax_RepairTransactionStructTableDef);

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes...
            loMeterTrax_RepairTransactionStructTableDef.PostDeserialize(this);

            // we can't use the default constraint list - so we have to tell them what they are
            loMeterTrax_RepairTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Clear();
            loMeterTrax_RepairTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            loMeterTrax_RepairTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueDateStr);
            loMeterTrax_RepairTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueTimeStr);
            loMeterTrax_RepairTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlIssueOfficerIDStr);
            loMeterTrax_RepairTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_MechSerialStr);
            loMeterTrax_RepairTransactionStructTableDef.HighTableRevision.UniqueContraintFields.Add(DBConstants.sqlMeterTrax_OutageCodeIDStr);

            // create indexes
            TTableDef loTableDef = loMeterTrax_RepairTransactionStructTableDef;
            List<String> loTempIndex = new List<string>();
            string loIdxPrefix = "MTX_REP_";

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechanismForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechanismForeignKey, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_PoleLocationForeignKey);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_PoleLocationForeignKey, loTempIndex.ToArray(), false, false);


            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_MechSerialStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_MechSerialStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlUnitSerialStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlUnitSerialStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueOfficerIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueOfficerIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_TransactionDateTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_TransactionDateTimeStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueDateStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueDateStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlIssueTimeStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlIssueTimeStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_RepairCodeIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_RepairCodeIDStr, loTempIndex.ToArray(), false, false);

            loTempIndex.Clear();
            loTempIndex.Add(DBConstants.sqlMeterTrax_OutageCodeIDStr);
            new TTableIndex(loTableDef, loIdxPrefix + DBConstants.sqlMeterTrax_OutageCodeIDStr, loTempIndex.ToArray(), false, false);

        }
#endif // !WindowsCE
    }



}
            #endregion
