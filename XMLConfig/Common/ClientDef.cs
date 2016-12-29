// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 12/16/13 10:06a $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Common/ClientConfig/ClientDef.cs $
//              Revision: $Revision: 53 $

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace Reino.ClientConfig
{
    /// <summary>
    /// Summary description for TClientDef.
    /// </summary>

    public class TClientDef : Reino.ClientConfig.TObjBase
    {
        public static Reino.ClientConfig.TClientDef GlobalClientDef = null;
        public static Boolean GuaranteedThreadSafe = false; // Used for PatrolCar 
        public static Boolean SkipHostSideOnlyCode = false; // Used for PatrolCar
        public static Boolean CfgSupportsWireless = false; // Used for .NET Handheld / PatrolCar

        #region Properties and Members
        protected TIssStructMgr _IssStructMgr;
        public TIssStructMgr IssStructMgr
        {
            get { return _IssStructMgr; }
            set { _IssStructMgr = value; }
        }

        protected TListMgr _ListMgr;
        public TListMgr ListMgr
        {
            get { return _ListMgr; }
            set { _ListMgr = value; }
        }

        protected TRegistryMgr _RegistryMgr;
        public TRegistryMgr RegistryMgr
        {
            get { return _RegistryMgr; }
            set { _RegistryMgr = value; }
        }

#if !WindowsCE && !__ANDROID__   

        dont build me on Android!


        [HostSideOnly] // Only used on the host-side
        protected TCustomerCfgMgr _CustomerCfgMgr;

        [HostSideOnly] // Only used on the host-side
        public TCustomerCfgMgr CustomerCfgMgr
        {
            get { return _CustomerCfgMgr; }
            set { _CustomerCfgMgr = value; }
        }
#endif

        // http://jirssb01:8080/browse/AUTOCITE-196
        protected TNSWStatusStruct _NSWStatusStruct;
        public TNSWStatusStruct NSWStatusStruct
        {
            get { return _NSWStatusStruct; }
            set { _NSWStatusStruct = value; }
        }

        protected int _Revision = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int Revision
        {
            get { return _Revision; }
            set { _Revision = value; }
        }

        protected string _Client = "";
        public string Client
        {
            get { return _Client; }
            set { _Client = value; }
        }

        protected bool _FreezeDateInReissueMode = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool FreezeDateInReissueMode
        {
            get { return _FreezeDateInReissueMode; }
            set { _FreezeDateInReissueMode = value; }
        }

        protected string _AgencyDesignator = "";
        public string AgencyDesignator
        {
            get { return _AgencyDesignator; }
            set { _AgencyDesignator = value; }
        }

        protected bool _MeterTraxEnabled = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        /// Flag indicating MeterTrax support        
        public bool MeterTraxEnabled
        {
            get { return _MeterTraxEnabled; }
            set { _MeterTraxEnabled = value; }
        }

        protected bool _IsNSWClient = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool IsNSWClient
        {
            get { return _IsNSWClient; }
            set { _IsNSWClient = value; }
        }

        [XmlIgnore]
        public static string TableRootPath = ""; // Path to data files used by PatrolCar AIR software

        [XmlIgnore]
        public string InstalledRootFilePath = ""; // Easy way to pass installed root file path around to service components
        #endregion

        public TClientDef()
            : base()
        {
            this._ListMgr = new TListMgr();
            this._IssStructMgr = new TIssStructMgr();
            this._RegistryMgr = new TRegistryMgr();
#if !WindowsCE && !__ANDROID__   
            // This property is only defined for the host-side
            this._CustomerCfgMgr = new TCustomerCfgMgr();
#endif
        }

#if !WindowsCE && !__ANDROID__   
        /// <summary>
        /// Constructs a TAgList object around the passed TableRev and adds it to ListMgr
        /// Initially used to construct the lists needed for MeterTrax support
        /// </summary>
        /// <param name="iDynamicListName"></param>
        /// <returns></returns>
      

        private void ConstructDynamicAgListFromTableRev(TTableDefRev iDynamicTableRev)
        {
            // if the list isn't already defined, create it
            if (_ListMgr.AgLists.Find(new TObjBasePredicate(iDynamicTableRev.Name).CompareByName) == null)
            {
                TAgList loDynamicList = new TAgList();
                loDynamicList.Name = iDynamicTableRev.Name;
                TTableDef loDynamicListTableDef = new TTableDef();
                loDynamicListTableDef.Name = iDynamicTableRev.Name;
                loDynamicListTableDef.fTblName = iDynamicTableRev.Name;
                loDynamicListTableDef.Revisions.Add(iDynamicTableRev);
                //loDynamicListTableDef.PostDeserialize(loDynamicList); //?
                loDynamicList.AddTableDef(loDynamicListTableDef);
                //loDynamicList.PostDeserialize(null); //?
                _ListMgr.AgLists.Add(loDynamicList);
            }
        }

        // http://jirssb01:8080/browse/AUTOCITE-196
        private void ConstructNSWStatusStruct()
        {
            // We don't have an elegant way of determining if the customer is NSW (New South Wales),
            // so we will look for a few agency lists that must exist for such a customer
            bool Has_IncludedHandbooksList = false;
            bool Has_IncludedInfringementsList = false;
            bool Has_IncludedOffenceList = false;
            foreach (TAgList loAgencyList in this.ListMgr.AgLists)
            {
                if (loAgencyList.Name == "INCLUDEHANDBOOK")
                    Has_IncludedHandbooksList = true;
                else if (loAgencyList.Name == "INCLUDEINFRINGE")
                    Has_IncludedInfringementsList = true;
                else if (loAgencyList.Name == "INCLUDEDOFFENCE")
                    Has_IncludedOffenceList = true;
            }

            // Now we will consider creating the NSW Status structure only
            // if all three agency lists were found in the configuration
            if ((Has_IncludedHandbooksList) && (Has_IncludedInfringementsList) && (Has_IncludedOffenceList))
            {
                if (_NSWStatusStruct == null)
                {
                    _NSWStatusStruct = new TNSWStatusStruct();
                    _NSWStatusStruct.Name = "NSWSTATUS";
                    _NSWStatusStruct.ObjDisplayName = "NSW Status";
                    _NSWStatusStruct.IsHostSideDefinitionOnly = true;
                    // add it to the strutures list so we know about it - 
                    // also, it will get PostDerialized called to initialize it
                    _IssStructMgr.IssStructs.Add(_NSWStatusStruct);
                }
            }
        }

        /// <summary>
        /// ConstructMeterTraxObjects
        /// 
        /// MeterTrax support is statically defined - if its active, then the 
        /// objects that are required are created here
        /// </summary>
        private void ConstructMeterTraxObjects()
        {
            // create a the tables we'll need to implement MeterTrax
            TMeterTrax_PoleLocationStruct loMTXPoleLocationInventory = new TMeterTrax_PoleLocationStruct();
            loMTXPoleLocationInventory.Name = AutoISSUE.DBConstants.cnMeterTrax_PoleLocationInventoryTableName;
            loMTXPoleLocationInventory.ObjDisplayName = AutoISSUE.DBConstants.cnMeterTrax_PoleLocationInventoryDisplayName;
            // for now, the handheld code is not AutoISSUE, its proprietary
            loMTXPoleLocationInventory.IsHostSideDefinitionOnly = true;
            // add it to the strutures list so we know about it - 
            // also, it will get PostDerialized called to initialize it
            _IssStructMgr.IssStructs.Add(loMTXPoleLocationInventory);

            TMeterTrax_MeterMechanismStruct loMTXMechanismInventory = new TMeterTrax_MeterMechanismStruct();
            loMTXMechanismInventory.Name = AutoISSUE.DBConstants.cnMeterTrax_MeterMechanismInventoryTableName;
            loMTXMechanismInventory.ObjDisplayName = AutoISSUE.DBConstants.cnMeterTrax_MeterMechanismInventoryDisplayName;
            loMTXMechanismInventory.IsHostSideDefinitionOnly = true;
            _IssStructMgr.IssStructs.Add(loMTXMechanismInventory);


            TMeterTrax_MeterInstallationHistory loMTXMechanismHistory = new TMeterTrax_MeterInstallationHistory();
            loMTXMechanismHistory.Name = AutoISSUE.DBConstants.cnMeterTrax_MeterInstallationHistoryTableName;
            loMTXMechanismHistory.ObjDisplayName = AutoISSUE.DBConstants.cnMeterTrax_MeterInstallationHistoryDisplayName;
            loMTXMechanismHistory.IsHostSideDefinitionOnly = true;
            _IssStructMgr.IssStructs.Add(loMTXMechanismHistory);



            // field transactions temp table
            TMeterTrax_FieldTransactionTempStruct loMTXFieldTransactionTemp = new TMeterTrax_FieldTransactionTempStruct();
            loMTXFieldTransactionTemp.Name = AutoISSUE.DBConstants.cnMeterTrax_FieldTransactionTempTableName;
            loMTXFieldTransactionTemp.ObjDisplayName = AutoISSUE.DBConstants.cnMeterTrax_FieldTransactionTempDisplayName;
            loMTXFieldTransactionTemp.IsHostSideDefinitionOnly = true;
            _IssStructMgr.IssStructs.Add(loMTXFieldTransactionTemp);



            // Audits
            TMeterTrax_AuditTransactionStruct loMTXAuditTransactions = new TMeterTrax_AuditTransactionStruct();
            loMTXAuditTransactions.Name = AutoISSUE.DBConstants.cnMeterTrax_AuditTransactionsTableName;
            loMTXAuditTransactions.ObjDisplayName = AutoISSUE.DBConstants.cnMeterTrax_AuditTransactionsDisplayName;
            loMTXAuditTransactions.IsHostSideDefinitionOnly = true;
            _IssStructMgr.IssStructs.Add(loMTXAuditTransactions);


            // OpChecks
            TMeterTrax_OpCheckTransactionStruct loMTXOpCheckTransactions = new TMeterTrax_OpCheckTransactionStruct();
            loMTXOpCheckTransactions.Name = AutoISSUE.DBConstants.cnMeterTrax_OpCheckTransactionsTableName;
            loMTXOpCheckTransactions.ObjDisplayName = AutoISSUE.DBConstants.cnMeterTrax_OpCheckTransactionsDisplayName;
            loMTXOpCheckTransactions.IsHostSideDefinitionOnly = true;
            _IssStructMgr.IssStructs.Add(loMTXOpCheckTransactions);

            // Inventory Transactions
            TMeterTrax_InventoryTransactionStruct loMTXInventoryTransactions = new TMeterTrax_InventoryTransactionStruct();
            loMTXInventoryTransactions.Name = AutoISSUE.DBConstants.cnMeterTrax_InventoryTransactionsTableName;
            loMTXInventoryTransactions.ObjDisplayName = AutoISSUE.DBConstants.cnMeterTrax_InventoryTransactionsDisplayName;
            loMTXInventoryTransactions.IsHostSideDefinitionOnly = true;
            _IssStructMgr.IssStructs.Add(loMTXInventoryTransactions);

            // Outage Transactions
            TMeterTrax_OutageTransactionStruct loMTXOutageTransactions = new TMeterTrax_OutageTransactionStruct();
            loMTXOutageTransactions.Name = AutoISSUE.DBConstants.cnMeterTrax_OutageTransactionsTableName;
            loMTXOutageTransactions.ObjDisplayName = AutoISSUE.DBConstants.cnMeterTrax_OutageTransactionsDisplayName;
            loMTXOutageTransactions.IsHostSideDefinitionOnly = true;
            _IssStructMgr.IssStructs.Add(loMTXOutageTransactions);


            // RateProgram Transactions
            TMeterTrax_RateProgramTransactionStruct loMTXRateProgramTransactions = new TMeterTrax_RateProgramTransactionStruct();
            loMTXRateProgramTransactions.Name = AutoISSUE.DBConstants.cnMeterTrax_RateProgramTransactionsTableName;
            loMTXRateProgramTransactions.ObjDisplayName = AutoISSUE.DBConstants.cnMeterTrax_RateProgramTransactionsDisplayName;
            loMTXRateProgramTransactions.IsHostSideDefinitionOnly = true;
            _IssStructMgr.IssStructs.Add(loMTXRateProgramTransactions);

            // Repair Transactions
            TMeterTrax_RepairTransactionStruct loMTXRepairTransactions = new TMeterTrax_RepairTransactionStruct();
            loMTXRepairTransactions.Name = AutoISSUE.DBConstants.cnMeterTrax_RepairTransactionsTableName;
            loMTXRepairTransactions.ObjDisplayName = AutoISSUE.DBConstants.cnMeterTrax_RepairTransactionsDisplayName;
            loMTXRepairTransactions.IsHostSideDefinitionOnly = true;
            _IssStructMgr.IssStructs.Add(loMTXRepairTransactions);


            // create the lists we'll need to run metertrax

            // Outage codes
            TTableDefRev loOutageCodeListTableDefRev = new TTableDefRev();
            loOutageCodeListTableDefRev.Name = AutoISSUE.DBConstants.cnMeterTrax_OutageCodeListName;
            loOutageCodeListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_OutageCodeColumnNameAbbrev, 3, true));
            loOutageCodeListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_OutageCodeColumnNameDesc, 30, true));
            loOutageCodeListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_OutageCodeColumnMechRelated, 5, true));
            ConstructDynamicAgListFromTableRev(loOutageCodeListTableDefRev);

            // Repair codes
            TTableDefRev loRepairCodeListTableDefRev = new TTableDefRev();
            loRepairCodeListTableDefRev.Name = AutoISSUE.DBConstants.cnMeterTrax_RepairCodeListName;
            loRepairCodeListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_RepairCodeColumnNameAbbrev, 3, true));
            loRepairCodeListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_RepairCodeColumnNameDesc, 30, true));
            ConstructDynamicAgListFromTableRev(loRepairCodeListTableDefRev);

            // street descriptions
            TTableDefRev loStreetDescListTableDefRev = new TTableDefRev();
            loStreetDescListTableDefRev.Name = AutoISSUE.DBConstants.cnMeterTrax_StreetDescListName;
            loStreetDescListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_StreetDescColumnNameDesc, 50, true));
            ConstructDynamicAgListFromTableRev(loStreetDescListTableDefRev);


            // Collection Routes
            TTableDefRev loCollectionRouteListTableDefRev = new TTableDefRev();
            loCollectionRouteListTableDefRev.Name = AutoISSUE.DBConstants.cnMeterTrax_CollectionRouteListName;
            loCollectionRouteListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_CollectionRouteColumnNameAbbrev, 10, true));
            loCollectionRouteListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_CollectionRouteColumnNameDesc, 50, true));
            ConstructDynamicAgListFromTableRev(loCollectionRouteListTableDefRev);

            // Maintenance Routes
            TTableDefRev loMaintenanceRouteListTableDefRev = new TTableDefRev();
            loMaintenanceRouteListTableDefRev.Name = AutoISSUE.DBConstants.cnMeterTrax_MaintenanceRouteListName;
            loMaintenanceRouteListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_MaintenanceRouteColumnNameAbbrev, 10, true));
            loMaintenanceRouteListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_MaintenanceRouteColumnNameDesc, 50, true));
            ConstructDynamicAgListFromTableRev(loMaintenanceRouteListTableDefRev);

            // Parking Zones 
            TTableDefRev loParkingZoneListTableDefRev = new TTableDefRev();
            loParkingZoneListTableDefRev.Name = AutoISSUE.DBConstants.cnMeterTrax_ParkingZoneListName;
            loParkingZoneListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_ParkingZoneColumnNameAbbrev, 10, true));
            loParkingZoneListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_ParkingZoneColumnNameDesc, 50, true));
            TTableRealFldDef loZoneMWR = new TTableRealFldDef();
            loZoneMWR.Name = AutoISSUE.DBConstants.cnMeterTrax_ParkingZoneColumnNameMaxWeeklyRevenue;
            loParkingZoneListTableDefRev.Fields.Add(loZoneMWR);
            ConstructDynamicAgListFromTableRev(loParkingZoneListTableDefRev);


            // special event tags
            TTableDefRev loSpecialEventListTableDefRev = new TTableDefRev();
            loSpecialEventListTableDefRev.Name = AutoISSUE.DBConstants.cnMeterTrax_SpecialEventTagListName;
            loSpecialEventListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_SpecialEventTagColumnNameAbbrev, 20, true));
            loSpecialEventListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_SpecialEventTagColumnNameDesc, 50, true));
            ConstructDynamicAgListFromTableRev(loSpecialEventListTableDefRev);


            // Meter Types
            TTableDefRev loMeterMechTypeTableDefRev = new TTableDefRev();
            loMeterMechTypeTableDefRev.Name = AutoISSUE.DBConstants.cnMeterTrax_MeterMechTypeListName;
            loMeterMechTypeTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_MeterMechTypeColumnNameAbbrev, 10, true));
            loMeterMechTypeTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_MeterMechTypeColumnNameDesc, 50, true));
            loMeterMechTypeTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_MeterMechTypeColumnNameEagle, 5, true));
            loMeterMechTypeTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_MeterMechTypeColumnNameMeterResponse, 3, true));
            loMeterMechTypeTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_MeterMechTypeColumnNameManufacturer, 40, true));
            ConstructDynamicAgListFromTableRev(loMeterMechTypeTableDefRev);

            // Vehicle States
            TTableDefRev loVehicleStateListTableDefRev = new TTableDefRev();
            loVehicleStateListTableDefRev.Name = AutoISSUE.DBConstants.cnMeterTrax_VehicleStateListName;
            loVehicleStateListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_VehicleStateColumnNameAbbrev, 10, true));
            loVehicleStateListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_VehicleStateColumnNameDesc, 50, true));
            ConstructDynamicAgListFromTableRev(loVehicleStateListTableDefRev);

            // Vehicle Plate Types
            TTableDefRev loVehiclePlateTypeListTableDefRev = new TTableDefRev();
            loVehiclePlateTypeListTableDefRev.Name = AutoISSUE.DBConstants.cnMeterTrax_VehiclePlateTypeListName;
            loVehiclePlateTypeListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_VehiclePlateTypeColumnNameAbbrev, 10, true));
            loVehiclePlateTypeListTableDefRev.Fields.Add(new TTableStringFldDef(AutoISSUE.DBConstants.cnMeterTrax_VehiclePlateTypeColumnNameDesc, 50, true));
            ConstructDynamicAgListFromTableRev(loVehiclePlateTypeListTableDefRev);



            // to populate these lists, do this after post-deserialize

            //1. EditDataSet = AutoISSUE.MainMenu.AIServiceManager.BeginListEdit(iListToEdit, AutoISSUE.glCurrentUser.SessionKey, SubConfigKey, ref loErrorMsg);
            //2. add list items
            //3. AutoISSUE.MainMenu.AIServiceManager.EndListEdit(SaveListDataset, ListNameToSave, TableDef.fDBEffectiveDateTime, AutoISSUE.glCurrentUser.SessionKey, SubConfigKey, ref loErrorMsg);
        }

#endif

        /// <summary>
        /// Called after the object has been created by the deserializer.  Gives the object the
        /// opportunity to resolve and object instances. For example, can set its _Parent
        /// property.
        /// </summary>
        /// 
        public override int PostDeserialize(TObjBase iParent)
        {

#if !WindowsCE && !__ANDROID__   
            if ((TClientDef.SkipHostSideOnlyCode == false) && (_MeterTraxEnabled == true))
            {
             //KLC    ConstructMeterTraxObjects();
            }

            // http://jirssb01:8080/browse/AUTOCITE-196
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                ConstructNSWStatusStruct();
            }

            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                ConstructMeterEnforcementHostLists();
            }
#endif

            _ListMgr.PostDeserialize(this);
            _IssStructMgr.PostDeserialize(this);
            _RegistryMgr.PostDeserialize(this);
#if !WindowsCE && !__ANDROID__   
            // This property is only defined for the host-side
            _CustomerCfgMgr.PostDeserialize(this);
#endif
            return 0;
        }

        public override void ResolveRegistryItems(ITTRegistry iRegistry)
        {
            base.ResolveRegistryItems(iRegistry);

            _ListMgr.ResolveRegistryItems(iRegistry);
            _IssStructMgr.ResolveRegistryItems(iRegistry);
            _RegistryMgr.ResolveRegistryItems(iRegistry);
#if !WindowsCE && !__ANDROID__   
            // This property is only defined for the host-side
            _CustomerCfgMgr.ResolveRegistryItems(iRegistry);
#endif
        }
#if USE_DEFN_IMPLEMENTATION
#endif

        public void CloseAllTables()
        {
            foreach (TTableDef loTableDef in TTableListMgr.glTableMgr.TableDefs)
            {
                try
                {
                    loTableDef.CloseTable();
                    continue;
                }
                catch
                {
                    continue;
                }
            }
        }

#if !WindowsCE && !__ANDROID__   
        private void ConstructMeterEnforcementHostLists()
        {
            // We will only create these specialized lists if we have a "METER_CLUSTER" or "PBC_ZONES" list defined in our configuration
            // (METER_CLUSTER is used by wireless meter enforcement, and PBC_ZONES is used for wireless pay-by-plate enforcement)
            if (
                (_ListMgr.AgLists.Find(new TObjBasePredicate("METER_CLUSTER").CompareByName_CaseInsensitive) == null) &&
                (_ListMgr.AgLists.Find(new TObjBasePredicate("PBC_ZONES").CompareByName_CaseInsensitive) == null)
               )
            {
                return;
            }

            // Static host-side list definition for meter zone cross-reference for Duncan assets
            TTableDefRev loMeterZoneXRefTableDefRev = new TTableDefRev();
            loMeterZoneXRefTableDefRev.Name = "METER_CLUSTER_DUNCAN";
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("SELECTION_ID", 40, true)); // Should match a value in METER_CLUSTER.CLUSTER_ID
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableIntFldDef("PAM_CLUSTER_ID", AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("PAM_CLUSTER_NAME", 40, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableIntFldDef("METER_ID", AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, true));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("METER_NAME", 40, true));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableIntFldDef("AREA_ID", AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("AREA_NAME", 40, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableIntFldDef("COLL_ROUTE_ID", AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("COLL_ROUTE_NAME", 40, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableIntFldDef("ENF_ROUTE_ID", AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("ENF_ROUTE_NAME", 40, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableIntFldDef("MAINT_ROUTE_ID", AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("MAINT_ROUTE_NAME", 40, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableIntFldDef("CUSTOMGROUP1_ID", AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("CUSTOMGROUP1_NAME", 40, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableIntFldDef("CUSTOMGROUP2_ID", AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("CUSTOMGROUP2_NAME", 40, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableIntFldDef("CUSTOMGROUP3_ID", AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("CUSTOMGROUP3_NAME", 40, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableIntFldDef("METER_TYPE_ID", AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, false)); // 0 = , 1 =
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("METER_TYPE_NAME", 40, false)); // Example: SSM or MSM
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("LOCLOT", 80, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("LOCBLOCK", 80, false));
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("LOCSTREET", 80, false));
            ConstructDynamicAgListFromTableRev(loMeterZoneXRefTableDefRev);

            // Static host-side list definition for meter zone cross-reference for Parkeon assets
            loMeterZoneXRefTableDefRev = new TTableDefRev();
            loMeterZoneXRefTableDefRev.Name = "METER_CLUSTER_PARKEON";
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("SELECTION_ID", 40, true)); // Should match a value in METER_CLUSTER.CLUSTER_ID
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("PARK_AND_AREA", 40, false));  // Example:  Atlanta|101-033
            ConstructDynamicAgListFromTableRev(loMeterZoneXRefTableDefRev);

            // Static host-side list definition for meter zone cross-reference for DPT assets
            loMeterZoneXRefTableDefRev = new TTableDefRev();
            loMeterZoneXRefTableDefRev.Name = "METER_CLUSTER_DPT";
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("SELECTION_ID", 40, true)); // Should match a value in METER_CLUSTER.CLUSTER_ID
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("STALL_RANGE", 40, false));  // Example:  0001-0008
            ConstructDynamicAgListFromTableRev(loMeterZoneXRefTableDefRev);

            // Static host-side list definition for meter zone cross-reference for PARKNOW assets
            loMeterZoneXRefTableDefRev = new TTableDefRev();
            loMeterZoneXRefTableDefRev.Name = "METER_CLUSTER_PARKNOW";
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("SELECTION_ID", 40, true)); // Should match a value in METER_CLUSTER.CLUSTER_ID
            loMeterZoneXRefTableDefRev.Fields.Add(new TTableStringFldDef("STALL_RANGE", 40, false));  // Example:  1208401-1208436
            ConstructDynamicAgListFromTableRev(loMeterZoneXRefTableDefRev);

          }
#endif
    }

    /// <summary>
    /// Summary description for TCustomerCfgMgr.
    /// </summary>
    public class TCustomerCfgMgr : Reino.ClientConfig.TObjBase
    {
        #region Properties and Members
        protected ListObjBase<TCustomerCfg> _CustomerCfgs;
        /// <summary>
        /// A collection of TCustomerCfg objects
        /// </summary>
        public ListObjBase<TCustomerCfg> CustomerCfgs
        {
            get { return _CustomerCfgs; }
            set { _CustomerCfgs = value; }
        }

        public TCustomerCfg GetCustomerCfg(String iName)
        {
            return _CustomerCfgs.Find(new TObjBasePredicate(iName).CompareByName);
        }
        #endregion

        public TCustomerCfgMgr()
            : base()
        {
            this._CustomerCfgs = new ListObjBase<TCustomerCfg>();
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            TClientDef loClientDef = (TClientDef)iParent;

            //is this AutoTRAX enabled?
            if ((loClientDef != null) && (loClientDef.MeterTraxEnabled))
            {
                //If the Customer Cfgs do not contain support for the hardware, 
                // then we will add them
                TCustomerCfg AutoTRAXCfgSymbol = this.GetCustomerCfg(AutoISSUE.DBConstants.cnAutoTRAXClientConfigSymbol);
                if (AutoTRAXCfgSymbol == null)
                {
                    AutoTRAXCfgSymbol = new TCustomerCfg();
                    AutoTRAXCfgSymbol.Name = AutoISSUE.DBConstants.cnAutoTRAXClientConfigSymbol;
                    AutoTRAXCfgSymbol.HWPlatform = TCustomerCfg.HardwareType.SY;
                    this.CustomerCfgs.Add(AutoTRAXCfgSymbol);
                }

                // new for AutoTRAX 2.0  - support for X3
                TCustomerCfg AutoTRAXCfgX3 = this.GetCustomerCfg(AutoISSUE.DBConstants.cnAutoTRAXClientConfigX3);
                if (AutoTRAXCfgX3 == null)
                {
                    AutoTRAXCfgX3 = new TCustomerCfg();
                    AutoTRAXCfgX3.Name = AutoISSUE.DBConstants.cnAutoTRAXClientConfigX3;
                    AutoTRAXCfgX3.HWPlatform = TCustomerCfg.HardwareType.X3;
                    this.CustomerCfgs.Add(AutoTRAXCfgX3);
                }
            }

            CustomerCfgs.PostDeserializeListItems(this);
            return 0;
        }

        public override void ResolveRegistryItems(ITTRegistry iRegistry)
        {
            base.ResolveRegistryItems(iRegistry);
            CustomerCfgs.ResolveRegistryItemsForListItems(iRegistry);
        }
    }

    /// <summary>
    /// Summary description for TCustomerCfg.
    /// </summary>
    public class TCustomerCfg : Reino.ClientConfig.TObjBase
    {
        public enum HardwareType
        {
            S3 = 0,
            S4,
            CE,                 // generic PDA, used in the layout file but translated to one of the supported PDA types
            PC,
            C3,
            C4,
            X3,
            C88,
            SY,                      // Symbol 8146
            Y2,                      // Symbol MC-9090
            I3,                      // Intermec CN3
            Y3,                      // Symbol MC-35
            Y4,                      // Symbol MC-75            
            I5,                      // Intermec CN50
            P1,                      // Pidion 1300
            Y5,                      // Motorola MC9500
            I7,                      // Intermec CK71
            C9,                      // Casio IT-9000
            UNK //Unknown
        }

        public enum CommunicationMedia
        {
            cmUnknown,
            cmSerial,
            cmFileorBlueTooth,
            cmActiveSync
        }

        public enum UserStructEncryptionType
        {
            encSaltedHash = 0,
            encZLIBCompression,
            encScrambled
        }


        public static UserStructEncryptionType GetUserStructEncryptionTypeForHardwareType(TCustomerCfg.HardwareType iHardwareType)
        {
            switch (iHardwareType)
            {
                case HardwareType.PC:
                    {
                        return UserStructEncryptionType.encSaltedHash;
                    }
                case HardwareType.CE:
                case HardwareType.SY:
                case HardwareType.Y2:
                case HardwareType.Y3:
                case HardwareType.Y4:
                case HardwareType.Y5:
                case HardwareType.I3:
                case HardwareType.I5:
                case HardwareType.I7:
                case HardwareType.P1:
                    {
                        return UserStructEncryptionType.encScrambled;
                    }
                default:
                    {
                        return UserStructEncryptionType.encZLIBCompression;
                    }
            }
        }


        //Translate the Hardware Code to a description
        public static String HardwarePlatformTypeToText(TCustomerCfg.HardwareType iHWType)
        {
            string loresult = "";
            switch (iHWType)
            {
                case TCustomerCfg.HardwareType.S3: loresult = "Series 3";
                    break;
                case TCustomerCfg.HardwareType.S4: loresult = "Series 4";
                    break;
                case TCustomerCfg.HardwareType.CE: loresult = "Pocket PC";       // generic name
                    break;
                case TCustomerCfg.HardwareType.PC: loresult = "Personal Computer";
                    break;
                case TCustomerCfg.HardwareType.C3: loresult = "Series 3 (WinCE)";
                    break;
                case TCustomerCfg.HardwareType.C4: loresult = "Series 4 (WinCE)";
                    break;
                case TCustomerCfg.HardwareType.X3: loresult = "Series 3 (WinCE v5)";
                    break;
                case TCustomerCfg.HardwareType.C88: loresult = "Series D/T";
                    break;
                case TCustomerCfg.HardwareType.SY: loresult = "Symbol";
                    break;
                case TCustomerCfg.HardwareType.Y2: loresult = "Symbol MC-9090";
                    break;
                case TCustomerCfg.HardwareType.I3: loresult = "Intermec CN3";
                    break;
                case TCustomerCfg.HardwareType.Y3: loresult = "Symbol MC-35";
                    break;
                case TCustomerCfg.HardwareType.Y4: loresult = "Symbol MC-75";
                    break;
                case TCustomerCfg.HardwareType.I5: loresult = "Intermec CN50";
                    break;
                case TCustomerCfg.HardwareType.I7: loresult = "Intermec CK71";
                    break;
                case TCustomerCfg.HardwareType.P1: loresult = "Pidion 1300";
                    break;
                case TCustomerCfg.HardwareType.Y5: loresult = "Motorola MC-9500";
                    break;
                case TCustomerCfg.HardwareType.UNK: loresult = "UNKNOWN";
                    break;
            }

            return loresult;
        }


        public static HardwareType HardwareTypeStringToHardwareType(String iHardwareTypeString)
        {
            for (HardwareType loHWType = 0; loHWType <= HardwareType.UNK; loHWType++)
            {
                if (iHardwareTypeString.ToUpper().StartsWith(loHWType.ToString().ToUpper()))
                    return loHWType;
            }
            return HardwareType.UNK;
        }


        public static HardwareType HardwareTypeFromSerialNo(String iSerialNo)
        {
            // try to use the dash, but if its not there, assume the 1st 2 chars
            int loDashIndex = iSerialNo.IndexOf('-');
            if (loDashIndex == -1)
            {
                loDashIndex = 2;
            }

            if (iSerialNo.Length >= loDashIndex)
            {
                return HardwareTypeStringToHardwareType(iSerialNo.Substring(0, loDashIndex));
            }
            else
            {
                return HardwareType.UNK;
            }
        }

        public static bool HardwareTypeIsPDA(HardwareType iHardwareType)
        {
            return
                (
                (iHardwareType == HardwareType.I3) ||        // if we get a few more it will probably be worthwhile to move this to a list
                (iHardwareType == HardwareType.I5) ||
                (iHardwareType == HardwareType.I7) ||
                (iHardwareType == HardwareType.Y2) ||
                (iHardwareType == HardwareType.Y3) ||
                (iHardwareType == HardwareType.Y4) ||
                (iHardwareType == HardwareType.Y5) ||
                (iHardwareType == HardwareType.P1) ||
                (iHardwareType == HardwareType.CE)
         );
        }


        public static HardwareType GetLayoutClassificationTypeFromSerialNo(String iSerialNo)
        {
            // first get the specific hardware type
            HardwareType loDeviceHardware = HardwareTypeStringToHardwareType(iSerialNo.Substring(0, iSerialNo.IndexOf('-') + 1));
            // now translate the specific PDAs into the generic CE class
            if (
                (loDeviceHardware == HardwareType.I3) ||        // if we get a few more it will probably be worthwhile to move this to a list
                (loDeviceHardware == HardwareType.I5) ||
                (loDeviceHardware == HardwareType.I7) ||
                (loDeviceHardware == HardwareType.Y2) ||
                (loDeviceHardware == HardwareType.Y3) ||
                (loDeviceHardware == HardwareType.Y4) ||
                (loDeviceHardware == HardwareType.Y5) ||
                (loDeviceHardware == HardwareType.P1)

                //(loDeviceHardware == HardwareType.CE) implied...
                )
            {
                // substitute the generic CE class referenced in the layout tool
                loDeviceHardware = HardwareType.CE;
            }

            return loDeviceHardware;
        }


        #region Properties and Members
        protected HardwareType _HWPlatform = HardwareType.S3;
        public HardwareType HWPlatform
        {
            get { return _HWPlatform; }
            set { _HWPlatform = value; }
        }

        protected string _DisplayName = "";
        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }

        protected List<string> _Structures;
        /// <summary>
        /// A Collection of structure names
        /// </summary>
        public List<string> Structures
        {
            get { return _Structures; }
            set { _Structures = value; }
        }

        protected List<string> _AgencyLists;
        /// <summary>
        /// A collection of agency list names
        /// </summary>
        public List<string> AgencyLists
        {
            get { return _AgencyLists; }
            set { _AgencyLists = value; }
        }

        protected List<string> _Sequences;
        /// <summary>
        /// A collection of sequence names
        /// </summary>
        public List<string> Sequences
        {
            get { return _Sequences; }
            set { _Sequences = value; }
        }

        /// <summary>
        /// WirelessEnabled added 3.19.2008 - The CommServiceManager library uses this attribute of a configuration to determine
        /// if wireless support files need to be uploaded to a device. Presently, that consists of AIWebProxy.exe, WebSupport.dll,
        /// ModemTest.exe, and CE_ISSUE_AP_MFCWireless.exe (which gets transmitted as CE_ISSUE_AP_MFC.EXE).
        /// </summary>
        protected bool _WirelessEnabled = false;
        public bool WirelessEnabled
        {
            get { return _WirelessEnabled; }
            set { _WirelessEnabled = value; }
        }
        #endregion

        public TCustomerCfg()
            : base()
        {
            this._Structures = new List<string>();
            this._AgencyLists = new List<string>();
            this._Sequences = new List<string>();
        }
    }

    /// <summary>
    /// Summary description for TRegistryMgr.
    /// </summary>
    public class TRegistryMgr : Reino.ClientConfig.TTableListMgr
    {
        #region Properties and Members
        #endregion

        public TRegistryMgr()
            : base()
        {
        }

#if !WindowsCE && !__ANDROID__   
        private void AddRegsistryTableClone(TTableDef iSrcTableDef, string iNewTableName)
        {


            // create the table def for it - pass the name without the extension to the constructor
            TTableDef loCloneTableDef = new TTableDef();
            loCloneTableDef.Name = iNewTableName;
            loCloneTableDef.fOpenEdit = false; // open in read-only mode
            loCloneTableDef.fTblNameExt = iNewTableName;

            if (loCloneTableDef.Revisions.Count == 0)
            {
                TTableDefRev loDefRev = new TTableDefRev();
                loDefRev.Name = iNewTableName;
                loCloneTableDef.Revisions.Add(loDefRev);
            }

            // this isn't read from the CFG file, but we have to make sure the call is made to get things initialized
            loCloneTableDef.PostDeserialize(Reino.ClientConfig.TTableListMgr.glTableMgr);
            // we're just a duplicate of the "real" table
            loCloneTableDef.CopyTableStructure(ref iSrcTableDef, false /* include virtual fields */ );


            // and add it to list of tables in this struct
            this.TableDefs.Add(loCloneTableDef);
        }
#endif


        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(this);

#if !WindowsCE && !__ANDROID__   
            // We only need to execute this code the very first 
            // time we're deserialized from to the initial load
            if (TClientDef.SkipHostSideOnlyCode == false)
            {

                // this is entering kludge territiory.... but otherwise we would have to define the every possible PDA type in the layout tool

                // do we have a "CE" registry def?
                // build a predicate to search for the tablename
                TObjBasePredicate loFindPredicate = new TObjBasePredicate(TCustomerCfg.HardwareType.CE.ToString() + "_");
                // try to find this table
                TTableDef loCERegistryTableDef = this.TableDefs.Find(loFindPredicate.CompareByName_StartsWith);
                // have one?
                if (loCERegistryTableDef != null)
                {
                    // CE is a placeholder type that represents one or more PDA types. 
                    string[] loParts = loCERegistryTableDef.Name.Split(new char[] { '_' });
                    // this is a "CE_REGISTRY", right?
                    if (loParts.GetLength(0) > 1)
                    {
                        foreach (TCustomerCfg.HardwareType oneHardwareType in Enum.GetValues(typeof(TCustomerCfg.HardwareType)))
                        {
                            // skip the generic class
                            if (oneHardwareType == TCustomerCfg.HardwareType.CE)
                            {
                                continue;
                            }

                            // is this a PDA?
                            if (TCustomerCfg.HardwareTypeIsPDA(oneHardwareType) == true)
                            {
                                // clone it to represent each of the other possible types
                                AddRegsistryTableClone(loCERegistryTableDef, oneHardwareType.ToString() + "_REGISTRY");
                            }
                        }


                        // finally, remove the source of the cloned table, the "generic" CE_REGISTRY
                        this.RemoveTableDef(loCERegistryTableDef);



                    }


                }
            }

#endif

            return 0;
        }
    }
}