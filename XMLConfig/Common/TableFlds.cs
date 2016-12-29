#define DEBUG

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;
using AutoISSUE;

namespace Reino.ClientConfig
{
    /// <summary>
    /// Summary description for TTableFldDef.
    /// </summary>

    /* The XmlInclude attribute is used on a base type to indicate that when serializing 
	 * instances of that type, they might really be instances of one or more subtypes. 
     * This allows the serialization engine to emit a schema that reflects the possibility 
     * of really getting a Derived when the type signature is Base. For example, we keep
     * field definitions in a generic collection of TTableFldDef. If an array element is 
     * TTableStringFldDef, the XML serializer gets mad because it was only expecting TTableFldDef. 
     */
    [XmlInclude(typeof(TTableStringFldDef)), XmlInclude(typeof(TTableIntFldDef)),
#if !WindowsCE && !__ANDROID__  
  XmlInclude(typeof(TTableBLOBFldDef)),
#endif
 XmlInclude(typeof(TTableRealFldDef)), XmlInclude(typeof(TTableTimeFldDef)),
 XmlInclude(typeof(TTableDateFldDef)), XmlInclude(typeof(TTableVirtualFldDef)),
 XmlInclude(typeof(TTableVTableLinkFldDef)), XmlInclude(typeof(TTableVLinkedFldDef)),
 XmlInclude(typeof(TTableElpsTimeVirtualFldDef)), XmlInclude(typeof(TTableMod97VirtualFldDef)),
 XmlInclude(typeof(TTableMod10VirtualFldDef)), XmlInclude(typeof(TTableMod10FirstCharVirtualFldDef)),
 XmlInclude(typeof(TTableOttawaMod10VirtualFldDef)), XmlInclude(typeof(TTableAlphaPosMod10VirtualFldDef)),
 XmlInclude(typeof(TTableLongBeachMod10VirtualFldDef)), XmlInclude(typeof(TTableMod10AustPostVirtualFldDef)),
 XmlInclude(typeof(TTableAFPMod11VirtualFldDef)), XmlInclude(typeof(TTableMod10CDOnlyVirtualFldDef)),
 XmlInclude(typeof(TTableArlingtonMod11VirtualFldDef)), XmlInclude(typeof(TTableNillumbikMod10VirtualFldDef)),
 XmlInclude(typeof(TTablePOSTbillpayFormatVirtualFldDef)), XmlInclude(typeof(TTableMorelandMod97VirtualFldDef)),
 XmlInclude(typeof(TTableBanyuleMod97VirtualFldDef))]
    public class TTableFldDef : TObjBase
    {
        /// <summary>
        /// Returns the storage mask used by ConvertFrom/ToStorageFormat().
        /// Implemented for PATROL_CAR_AIR data validation.
        /// </summary>
        public virtual string DefaultStorageMask
        {
            get { return ""; }
        }

        /// <summary>
        /// Sometimes a column is edited as a different type than its underlying storage type. 
        /// In particular, the REGISTRY.DAT table's VALUE column is a string, but may contain
        /// numbers and dates.
        /// </summary>
        public enum TEditDataType
        {
            tftString,
            tftDate,
            tftTime,
            tftReal,
            tftInteger
        }

        public enum TSpecialListType
        {
            sltNormal = 0,
            sltCourtName,
            sltCourtTime,
            sltCourtAddress,
            sltCourtCode,
            sltMultiLine
        }

        #region Properties and Members

        /// <summary>
        /// Indicates if the column in the database table should have the "not null" constraint.
        /// </summary>
        private bool _dbNotNull = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public virtual bool dbNotNull
        {
            get { return (_dbNotNull) || (Name == DBConstants.sqlUniqueKeyStr) || (Name == DBConstants.sqlMasterKeyStr); }
            set { _dbNotNull = value; }
        }

#if !WindowsCE && !__ANDROID__  
        /// <summary>
        /// DefaultCurrentTime is an attribute required by the host side to determine if the
        /// underlying table column should have a default.
        /// </summary>
        protected bool _DefaultCurrentTime;
        [XmlIgnore]
        [HostSideOnly] // Only used on the host-side
        public virtual bool DefaultCurrentTime
        {
            get
            {
                return _DefaultCurrentTime || (Name == DBConstants.sqlRecCreationDateStr);
            }
            set { _DefaultCurrentTime = value; }
        }

        /// <summary>
        /// Host only. Returns whether the column is an sql server "identity"; a generated, primary key field.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsIdentityColumn()
        {
            return Name == DBConstants.sqlUniqueKeyStr;
        }
#endif

#if __ANDROID__  

        protected TEditDataType _EditDataType = TEditDataType.tftString;
        [System.ComponentModel.DefaultValue(TEditDataType.tftString)] // This prevents serialization of default values
        [HostSideOnly] // Only used on the host-side
        public TEditDataType EditDataType
        {
            get { return _EditDataType; }
            set { _EditDataType = value; }
        }
#endif

#if !WindowsCE && !__ANDROID__  

        // This enum goes along with the SystemType property and is also only needed for ACW files.
        public enum TSystemType
        {
            stAutoISSUE,
            stACW,
            stAutoISSUEandACW
        }

        // These are only used when generating files for ACW. All of these values will have the DefaultValue
        // attribute to prevent them being written to XML file when their value is same as default value (which
        // most of the time it will be since will hardly ever be used).
        [System.ComponentModel.DefaultValue(-1)]
        [HostSideOnly] // Only used on the host-side
        public int AcwFieldOrder = -1;

        [System.ComponentModel.DefaultValue(false)]
        [HostSideOnly] // Only used on the host-side
        public bool AcwQueryField = false;

        [System.ComponentModel.DefaultValue("")]
        [HostSideOnly] // Only used on the host-side
        public string AcwName = "";

        [System.ComponentModel.DefaultValue(TSystemType.stAutoISSUE)]
        [HostSideOnly] // Only used on the host-side
        public TSystemType SystemType = TSystemType.stAutoISSUE;
#endif // #if !WindowsCE

        protected int _Size = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public virtual int Size
        {
            get { return _Size; }
            set { _Size = value; }
        }

        protected int _TableNdx = 0;
        /// <summary>
        /// Field belongs in main table if = 0, or detail table if > 0. For example, Violation fields
        /// should be TableNdx = 1.
        /// </summary>
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int TableNdx
        {
            get { return _TableNdx; }
            set { _TableNdx = value; }
        }

        protected string _DisplayName = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }

        protected bool _DBIncludeInMatch = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool DBIncludeInMatch
        {
            get { return _DBIncludeInMatch; }
            set { _DBIncludeInMatch = value; }
        }

        protected TSpecialListType _SpecialListType = TSpecialListType.sltNormal;
        [System.ComponentModel.DefaultValue(TSpecialListType.sltNormal)] // This prevents serialization of default values
        public TSpecialListType SpecialListType
        {
            get { return _SpecialListType; }
            set { _SpecialListType = value; }
        }

        protected string _Mask = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string Mask
        {
            get { return _Mask; }
            set { _Mask = value; }
        }

        protected string _MaskForHH = "";
        /// <summary>
        /// MaskForHH was added because Alan likes to translate the mask for his host-side needs, but
        /// that screws up the .NET Handheld and PatrolCar/Emulator. His conversions cannot be reversed,
        /// so in order to play nice with his needs and also save us many hours of debugging, it is 
        /// necessary to add extra bloat and keep a virgin copy of the mask that (hopefully) won't 
        /// get altered by anybody.
        /// </summary>
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string MaskForHH
        {
            get { return _MaskForHH; }
            set { _MaskForHH = value; }
        }

        protected string _ListName = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string ListName
        {
            get { return _ListName; }
            set { _ListName = value; }
        }

        protected string _RegistrySection = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string RegistrySection
        {
            get { return _RegistrySection; }
            set { _RegistrySection = value; }
        }

        protected string _DefaultValue = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string DefaultValue
        {
            get { return _DefaultValue; }
            set { _DefaultValue = value; }
        }

        protected bool _Required = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool Required
        {
            get { return _Required; }
            set { _Required = value; }
        }

        protected bool _ListOnly = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool ListOnly
        {
            get { return _ListOnly; }
            set { _ListOnly = value; }
        }

        protected int _MinValue = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int MinValue
        {
            get { return _MinValue; }
            set { _MinValue = value; }
        }

        protected int _MaxValue = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int MaxValue
        {
            get { return _MaxValue; }
            set { _MaxValue = value; }
        }

        //#if !WindowsCE
        // This property MUST be included in the serialization, otherwise all fields will be marked false
        // the next time the ClientDef is reserialized
        protected bool _IsHostSideDefinitionOnly = false;
        /// <summary>
        /// Flag indicating object was created dynamically for the host side only
        /// and does not exist in the handheld client instance
        /// </summary>
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        [HostSideOnly] // Only used on the host-side
        public bool IsHostSideDefinitionOnly
        {
            get { return _IsHostSideDefinitionOnly; }
            set { _IsHostSideDefinitionOnly = value; }
        }

        // This property MUST be included in the serialization, otherwise all fields will be marked false
        // the next time the ClientDef is reserialized
        protected bool _IsRedefinedInDetailTable = false;
        /// <summary>
        ///  Flag indicating this field has been moved to a detail table. It has 
        /// to exist in the main table for DAT file compatibility, but wont be populated
        /// during database record retrieval - because its data is in a detail table
        /// </summary>
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool IsRedefinedInDetailTable
        {
            get { return _IsRedefinedInDetailTable; }
            set { _IsRedefinedInDetailTable = value; }
        }

        //#endif
        #endregion

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public bool IsVirtual = false;

        protected bool fNotAddedToTableDef;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public TTableDef fParentTable;

        public int OccurNo
        {
            get
            {
                // If this field is for a sub-table, then must look at its
                // suffix and determine what occurence number this field is.
                int occurNo = 0;
                if (TableNdx > 0)
                {
                    // Suffix will always end with underscore then number. So
                    // split out all the text by the underscore and see if the
                    // last text is a number, if it is then its the occur number.
                    try
                    {
                        string[] nameParts = Name.Split('_');
                        if (nameParts.Length > 1)
                        {
                            occurNo = Convert.ToInt32(nameParts[nameParts.Length - 1]);
                        }
                    }
                    catch
                    {
                        // Didn't work, make sure occurence = 0;
                        occurNo = 0;
                    }
                }

                // Ok, got the occurence number, return it.
                return occurNo;
            }
        }


        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);

            //fParentTable = iParentTable;
            //Name = iName;
            //Size = 80;

            fParentTable = (iParent as TTableDef);

            /*  fields are added in the xml conversion process, they don't need to be added again
            if (!fNotAddedToTableDef)
            {
                if (fVirtual)
                    fParentTable.AddVirtualField(this);
                else
                    fParentTable.AddField(this);
            }
            */
            // Virtual fields have children, so they need to also be deserialized
            if (this is TTableVirtualFldDef)
            {
                foreach (TTableFldDef ChildFld in ((TTableVirtualFldDef)(this)).Fields)
                    ChildFld.PostDeserialize(iParent);
            }

            return 0;
        }

        public override void ResolveRegistryItems(ITTRegistry iRegistry)
        {
            base.ResolveRegistryItems(iRegistry);

            // Virtual fields have children, so they need to also be resolved
            if (this is TTableVirtualFldDef)
            {
                foreach (TTableFldDef ChildFld in ((TTableVirtualFldDef)(this)).Fields)
                    ChildFld.ResolveRegistryItems(iRegistry);
            }
        }

        public TTableFldDef(String iName, int iSize, bool idbNotNull)
            : base()
        {
            Name = iName;
            Size = iSize;
            dbNotNull = idbNotNull;
        }

        public TTableFldDef()
            : base()
        {
            //All of the serialized classes must have a parameterless constructor
        }


        public virtual int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            if (oResult == null) return -1;

            oResult = "";

            // non-virtual fields are very simple: just copy the raw data over.
            if (!IsVirtual)
            {
                oResult = GetRawFieldData(iSrcTable, iFieldNo);
                return 0;
            }

            return -1;
        }

        public virtual int GetComponentFieldCnt()
        {
            return 1;
        }

        public virtual TTableFldDef GetComponentField(int iIndexNo)
        {
            if (iIndexNo == 0)
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        public String GetRawFieldData(TTTable iSrcTable, int iFieldNo)
        {
            if (iFieldNo < 0)
            {
                TObjBasePredicate loPredicate = new TObjBasePredicate(this.Name);
                iFieldNo = iSrcTable.fTableDef.HighTableRevision.Fields.FindIndex(loPredicate.CompareByName_CaseInsensitive);
            }
            // Protect against accessing an invalid index.
            // (This might happen if were trying to read from an empty table)
            if ((iFieldNo < 0) || (iFieldNo >= iSrcTable.fFieldValues.Count))
                return "";
            else
                return iSrcTable.fFieldValues[iFieldNo];
        }


        public String GetRawComparisonFieldData(TTTable iSrcTable, int iFieldNo)
        {
            if (iSrcTable.fComparisonFieldValues == null)
                return null;

            if (iFieldNo < 0)
            {
                TObjBasePredicate loPredicate = new TObjBasePredicate(this.Name);
                iFieldNo = iSrcTable.fTableDef.HighTableRevision.Fields.FindIndex(loPredicate.CompareByName_CaseInsensitive);
            }

            // Protect against accessing an invalid index.
            // (This might happen if were trying to read from an empty table)
            if ((iFieldNo < 0) || (iFieldNo >= iSrcTable.fComparisonFieldValues.Count))
                return "";
            else
                return iSrcTable.fComparisonFieldValues[iFieldNo];
        }


        ~TTableFldDef()
        {
            // choose the destructor :)
            // nothing to do here
        }

        public virtual int ConvertToStoreFormat(String iSrcData, String iDataMask, ref String oStoreData)
        {
            oStoreData = iSrcData;
            return 0;
        }

        /// <summary>
        /// This will return an exact copy of ourself.
        /// </summary>
        /// <returns></returns>
        public virtual TTableFldDef Clone()
        {
            // First, start out with copy of our object with all the basic properties set.
            TTableFldDef fieldCopy = (TTableFldDef)MemberwiseClone();
            // We dont have any complex objects, just simple properties (the one actual pointer
            // object we have should point to same exact object so its safe), so just return
            // the simple cloned object we made.
            return fieldCopy;
        }
    }

    /// <summary>
    /// Summary description for TTableIntFldDef.
    /// </summary>

    public class TTableIntFldDef : TTableFldDef
    {
        #region Properties and Members
        #endregion


        public override int Size
        {
            get { return _Size; }
            set { _Size = value; }
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            if (_Size <= 0)
                _Size = ReinoTablesConst.INT_TYPE_FIELDLENGTH;

            return 0;
        }


        public TTableIntFldDef()
            : base()
        {
            // must have a parameterless constructor
        }
        public TTableIntFldDef(String iName, int iSize, bool idbNotNull)
            : base(iName, iSize, idbNotNull)
        {
        }

        /// <summary>
        /// Converts a passed integer to a stored integer (left padded w/ 0's to field size)
        /// </summary>
        /// <param name="iSrcTable"></param>
        /// <param name="iFieldNo"></param>
        /// <param name="iDataMask"></param>
        /// <param name="oResult"></param>
        /// <returns></returns>
        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            if ((iDataMask == "") || (iDataMask == null))
                return base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);

            String loRawData;
            oResult = "";
            if ((loRawData = GetRawFieldData(iSrcTable, iFieldNo)) == null)
            {
                return 0;
            }

            return ReinoControls.TextBoxBehavior.FormatNumberStr(loRawData, iDataMask, ref oResult);
        }

        /// <summary>
        /// Converts a passed integer to a stored integer (left padded w/ 0's to field size)
        /// </summary>
        /// <param name="iSrcData"></param>
        /// <param name="iDataMask"></param>
        /// <param name="oStoreData"></param>
        /// <returns></returns>
        public override int ConvertToStoreFormat(String iSrcData, String iDataMask, ref String oStoreData)
        {
            short loCharNdx;
            char loChar;
            StringBuilder loConversionResult = new StringBuilder();

            //if (!oStoreData) return 0;

            //oStoreData = "";
            //if (!iSrcData || !(*iSrcData))
            //  return 0;

            // If the passed data is empty/null, return empty string instead of padding with regular mask
            if ((iSrcData == null) || (iSrcData.Length == 0))
            {
                oStoreData = "";
                return 0;
            }

            // strip out everything by digits 
            for (loCharNdx = 0; loCharNdx < iSrcData.Length; loCharNdx++)
            {
                loChar = iSrcData[loCharNdx];

                if (!((loChar >= '0') && (loChar <= '9')) && (loChar != '-'))
                    continue;

                loConversionResult.Append(loChar);
            }

            // First lets make sure we don't have an invalid length
            if (_Size <= 0)
                _Size = ReinoTablesConst.INT_TYPE_FIELDLENGTH;

            // copy to result, left pad w/ 0's to full length
            oStoreData = loConversionResult.ToString().PadLeft(Size, '0');

            return 0;
        }


    }


#if !WindowsCE && !__ANDROID__  
    /// <summary>
    /// BLOB field type definition. Implemented on host-side only for database
    /// access of pictures, audio files, etc. 
    /// </summary>
    [HostSideOnly] // Only applicable to the host-side
    public class TTableBLOBFldDef : TTableFldDef
    {
    #region Properties and Members
        #endregion

        public byte[] BinaryRawData = null;


        public override int Size
        {
            get { return _Size; }
            set
            {
                if (value == -1)
                {
                    value = ReinoTablesConst.BLOB_TYPE_FIELDLENGTH;
                }

                _Size = value;
                BinaryRawData = new byte[_Size];
            }
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);

            return 0;
        }
        public TTableBLOBFldDef()
            : base()
        {
            // must have a parameterless constructor
        }



        /// <summary>
        /// Converts a passed integer to a stored integer (left padded w/ 0's to field size)
        /// </summary>
        /// <param name="iSrcTable"></param>
        /// <param name="iFieldNo"></param>
        /// <param name="iDataMask"></param>
        /// <param name="oResult"></param>
        /// <returns></returns>
        public int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, byte[] oResult)
        {

            throw new Exception("TTableBLOBFldDef " + this.Name + " invoked ConvertFromStoreFormat which is not implemented in a thread safe manner.");
            /*
            if (BinaryRawData == null)
            {
                throw new Exception(this.Name + " is a BLOB field which has not been assigned a size.");
            }
            else
            {
                BinaryRawData.CopyTo(oResult, 0);
            }
            return 0;
            */

            /*
            String loRawData;
            oResult = "";
            if ((loRawData = GetRawFieldData(iSrcTable, iFieldNo)) == null)
            {
                return 0;
            }

            return ReinoControls.TextBoxBehavior.FormatNumberStr(loRawData, iDataMask, ref oResult);
            */
        }

        /// <summary>
        /// Converts a passed integer to a stored integer (left padded w/ 0's to field size)
        /// </summary>
        /// <param name="iSrcData"></param>
        /// <param name="iDataMask"></param>
        /// <param name="oStoreData"></param>
        /// <returns></returns>
        public int ConvertToStoreFormat(byte[] oStoreData)
        {
            throw new Exception("TTableBLOBFldDef " + this.Name + " invoked ConvertToStoreFormat which is not implemented in a thread safe manner.");

            /*
            // always resize the BLOB to hold the new data
            this.Size = oStoreData.Length;

            oStoreData.CopyTo(BinaryRawData, 0);
            return 0;
            */

            /*
            short loCharNdx;
            char loChar;
            StringBuilder loConversionResult = new StringBuilder();

            //if (!oStoreData) return 0;

            //oStoreData = "";
            //if (!iSrcData || !(*iSrcData))
            //  return 0;

            // strip out everything by digits 
            for (loCharNdx = 0; loCharNdx < iSrcData.Length; loCharNdx++)
            {
                loChar = iSrcData[loCharNdx];

                if (!((loChar >= '0') && (loChar <= '9')) && (loChar != '-'))
                    continue;

                loConversionResult.Append(loChar);
            }

            // copy to result, left pad w/ 0's to full length
            oStoreData = loConversionResult.ToString().PadLeft(Size, '0');

            return 0;
             */
        }

        public override TTableFldDef Clone()
        {
            // Use parent to clone our basic and parent properties.
            TTableFldDef fieldCopy = base.Clone();
            // Also clone our raw data.
            (fieldCopy as TTableBLOBFldDef).BinaryRawData = (byte[])BinaryRawData.Clone();
            // Return cloned object.
            return fieldCopy;
        }

    }
#endif

    /// <summary>
    /// Summary description for TTableStringFldDef.
    /// </summary>

    public class TTableStringFldDef : TTableFldDef
    {
        #region Properties and Members
        //Added new Property by Jeetendra Prasad Date 12/Feb/2010
        protected bool _Encrypted = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool Encrypted
        {
            get { return _Encrypted; }
            set { _Encrypted = value; }
        }
        #endregion

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }


        public TTableStringFldDef()
            : base()
        {
        }
        public TTableStringFldDef(String iName, int iSize, bool idbNotNull)
            : base(iName, iSize, idbNotNull)
        {
        }


    }

    /// <summary>
    /// Summary description for TTableRealFldDef.
    /// </summary>

    public class TTableRealFldDef : TTableFldDef
    {
        #region Properties and Members
        #endregion

        public override int Size
        {
            get { return _Size; }
            set { _Size = value; }
        }

        /// <summary>
        /// Converts a passed integer to a stored integer (left padded w/ 0's to field size)
        /// </summary>
        /// <param name="iSrcTable"></param>
        /// <param name="iFieldNo"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            //if ((!iDataMask) || (!*iDataMask))
            if (iDataMask.Equals(""))
            {
                return base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);
            }

            String loRawData;
            oResult = "";

            if ((loRawData = GetRawFieldData(iSrcTable, iFieldNo)) == null)
            {
                return 0;
            }

            return ReinoControls.TextBoxBehavior.FormatNumberStr(loRawData, iDataMask, ref oResult);
        }


        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            if (_Size <= 0)
                _Size = ReinoTablesConst.REAL_TYPE_FIELDLENGTH;

            return 0;
        }

        public TTableRealFldDef()
            : base()
        {
            //XML Serialization requires a public parameterless constructor
        }


        public override int ConvertToStoreFormat(String iSrcData, String iDataMask, ref String oStoreData)
        {
            short loCharNdx;
            char loChar;

            if (oStoreData == null) return 0;
            oStoreData = "";

            if ((iSrcData == null) || (iSrcData.Length == 0))
                return 0;

            // strip out everything that's not numeric related
            for (loCharNdx = 0; loCharNdx < iSrcData.Length; loCharNdx++)
            {
                loChar = iSrcData[loCharNdx];

                if (!((loChar >= '0') && (loChar <= '9')) && (loChar != '-') && (loChar != '.'))
                    continue;

                oStoreData += loChar;
            }

            // if mask is "8"s, insert the implied decimal point.
            if ((iDataMask != null) && (iDataMask.IndexOf('8') >= 0))
            {
                // First we need to see if a decimal point already exists. If so, we don't want to
                // add a second decimal point, but we do want to make sure there are 2 digits following
                // the decimal point.
                int loDecimalIdx = oStoreData.IndexOf('.');
                if (loDecimalIdx >= 0)
                {
                    // Right-pad with zeros until there are at least 2 digit places behind the decimal point
                    while (loDecimalIdx >= oStoreData.Length - 2)
                    {
                        oStoreData = oStoreData + "0";
                        loDecimalIdx = oStoreData.IndexOf('.');
                    }
                }
                else
                {
                    // No decimal point exists, so insert one in front of the last two digits
                    oStoreData = oStoreData.Insert(oStoreData.Length - 2, ".");
                }
            }

            // left pad w/ 0's to full length
            oStoreData = oStoreData.PadLeft(Size, '0');

            return 0;
        }

    }

    /// <summary>
    /// Summary description for TTableTimeFldDef.
    /// </summary>

    public class TTableTimeFldDef : TTableFldDef
    {
        #region Properties and Members
        public override string DefaultStorageMask
        {
            get
            {
                return ReinoTablesConst.TIME_TYPE_DATAMASK;
            }
        }
        #endregion

        public override int Size
        {
            get { return _Size; }
            set { _Size = ReinoTablesConst.TIME_TYPE_FIELDLENGTH; }
        }

        /// <summary>
        /// Converts a stored Time (in hhmmss format) to a string in the format of the passed mask.
        /// </summary>
        /// <param name="iSrcTable"></param>
        /// <param name="iFieldNo"></param>
        /// <param name="iDataMask"></param>
        /// <param name="oResult"></param>
        /// <returns></returns>
        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            DateTime loOSTime = new DateTime();
            int loStatus;
            String loRawData;

            oResult = "";

            if ((loRawData = GetRawFieldData(iSrcTable, iFieldNo)) == null) return 0;

            if (loRawData.Equals("")) return 0;

            if (iDataMask.Equals(""))
                return base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);

            if ((loStatus = ReinoControls.TextBoxBehavior.TimeStringToOSTime(ReinoTablesConst.TIME_TYPE_DATAMASK, loRawData, ref loOSTime)) < 0)
                return loStatus;

            if ((loStatus = ReinoControls.TextBoxBehavior.OSTimeToTimeString(loOSTime, iDataMask, ref oResult)) < 0)
                return loStatus;

            return 0;
        }


        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            //if (iSize <= 0) there really isn't a choice on size for this, is there?
            {
                Size = ReinoTablesConst.TIME_TYPE_FIELDLENGTH;
            }

            return 0;
        }

        public TTableTimeFldDef()
            : base()
        {
            //XML Serialization requires a public parameterless constructor
        }


        /// <summary>
        /// Converts a passed Time in the passed format to a stored Time (in hhmmss format)
        /// </summary>
        /// <param name="iSrcData"></param>
        /// <param name="iDataMask"></param>
        /// <param name="oStoreData"></param>
        /// <returns></returns>
        public override int ConvertToStoreFormat(String iSrcData, String iDataMask, ref String oStoreData)
        {
            DateTime loOSTime = new DateTime();
            int loStatus;

            if (oStoreData == null) return 0;
            oStoreData = "";

            //if (!iSrcData || !(*iSrcData) ||  !iDataMask || !(*iDataMask))
            if ((iSrcData == null) || (iSrcData.Length == 0) || (iDataMask == null) || (iDataMask.Length == 0))
                return 0;

            if ((loStatus = ReinoControls.TextBoxBehavior.TimeStringToOSTime(iDataMask, iSrcData, ref loOSTime)) < 0)
                return loStatus;

            if ((loStatus = ReinoControls.TextBoxBehavior.OSTimeToTimeString(loOSTime, ReinoTablesConst.TIME_TYPE_DATAMASK, ref oStoreData)) < 0)
                return loStatus;

            return 0;
        }

    }

    /// <summary>
    /// Summary description for TTableDateFldDef.
    /// </summary>

    public class TTableDateFldDef : TTableFldDef
    {
        #region Properties and Members
        public override string DefaultStorageMask
        {
            get
            {
                return ReinoTablesConst.DATE_TYPE_DATAMASK;
            }
        }
        #endregion

        public override int Size
        {
            get { return _Size; }
            set { _Size = ReinoTablesConst.DATE_TYPE_FIELDLENGTH; }
        }

        /// <summary>
        /// Converts a stored date (in yyyymmdd format) to a string in the format of the passed mask.
        /// </summary>
        /// <param name="iSrcTable"></param>
        /// <param name="iFieldNo"></param>
        /// <param name="iDataMask"></param>
        /// <param name="oResult"></param>
        /// <returns></returns>
        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            DateTime loOSDate = new DateTime();
            int loStatus;
            oResult = "";

            String loRawData;
            if ((loRawData = GetRawFieldData(iSrcTable, iFieldNo)) == null) return 0;

            if (loRawData.Equals("")) return 0;

            if (iDataMask.Equals(""))
                return base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);

            if ((loStatus = ReinoControls.TextBoxBehavior.DateStringToOSDate(ReinoTablesConst.DATE_TYPE_DATAMASK, loRawData, ref loOSDate)) < 0)
                return loStatus;

            if ((loStatus = ReinoControls.TextBoxBehavior.OSDateToDateString(loOSDate, iDataMask, ref oResult)) < 0)
                return loStatus;

            return 0;
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            //if (iSize <= 0) there really isn't a choice on size for this, is there?
            {
                Size = ReinoTablesConst.DATE_TYPE_FIELDLENGTH;
            }

            return 0;
        }

        public TTableDateFldDef()
            : base()
        {
            //XML Serialization requires a public parameterless constructor
        }

        public TTableDateFldDef(String iName, int iSize, bool idbNotNull, bool iDefaultCurrentTime)
            : base(iName, iSize, idbNotNull)
        {
#if !WindowsCE && !__ANDROID__  
            DefaultCurrentTime = iDefaultCurrentTime;
#endif
        }


        /// <summary>
        /// Converts a passed date in the passed format to a stored date (in yyyymmdd format)
        /// </summary>
        /// <param name="iSrcData"></param>
        /// <param name="iDataMask"></param>
        /// <param name="oStoreData"></param>
        /// <returns></returns>
        public override int ConvertToStoreFormat(String iSrcData, String iDataMask, ref String oStoreData)
        {
            DateTime loOSDate = new DateTime();
            int loStatus;

            if (oStoreData == null) return 0;

            oStoreData = "";

            //if (!iSrcData || !(*iSrcData) ||  !iDataMask || !(*iDataMask))
            if ((iSrcData.Length == 0) || (iDataMask.Length == 0))
                return 0;

            if ((loStatus = ReinoControls.TextBoxBehavior.DateStringToOSDate(iDataMask, iSrcData, ref loOSDate)) < 0)
                return loStatus;

            if ((loStatus = ReinoControls.TextBoxBehavior.OSDateToDateString(loOSDate, ReinoTablesConst.DATE_TYPE_DATAMASK, ref oStoreData)) < 0)
                return loStatus;

            return 0;
        }

    }

    /* The XmlInclude attribute is used on a base type to indicate that when serializing 
	 * instances of that type, they might really be instances of one or more subtypes. 
     * This allows the serialization engine to emit a schema that reflects the possibility 
     * of really getting a Derived when the type signature is Base. For example, we keep
     * field definitions in a generic collection of TTableFldDef. If an array element is 
     * TTableStringFldDef, the XML serializer gets mad because it was only expecting TTableFldDef. 
     */
    [XmlInclude(typeof(TTableVTableLinkFldDef)), XmlInclude(typeof(TTableVLinkedFldDef)),
     XmlInclude(typeof(TTableElpsTimeVirtualFldDef)), XmlInclude(typeof(TTableMod97VirtualFldDef)),
     XmlInclude(typeof(TTableMod10VirtualFldDef)), XmlInclude(typeof(TTableMod10FirstCharVirtualFldDef)),
     XmlInclude(typeof(TTableOttawaMod10VirtualFldDef)), XmlInclude(typeof(TTableAlphaPosMod10VirtualFldDef)),
     XmlInclude(typeof(TTableLongBeachMod10VirtualFldDef)), XmlInclude(typeof(TTableMod10AustPostVirtualFldDef)),
     XmlInclude(typeof(TTableAFPMod11VirtualFldDef)), XmlInclude(typeof(TTableMod10CDOnlyVirtualFldDef)),
     XmlInclude(typeof(TTableArlingtonMod11VirtualFldDef)), XmlInclude(typeof(TTableNillumbikMod10VirtualFldDef)),
     XmlInclude(typeof(TTablePOSTbillpayFormatVirtualFldDef)), XmlInclude(typeof(TTableMorelandMod97VirtualFldDef)),
     XmlInclude(typeof(TTableBanyuleMod97VirtualFldDef))]
    /// <summary>
    /// Summary description for TTableVirtualFldDef.
    /// </summary>
    public class TTableVirtualFldDef : TTableFldDef
    {
        #region Properties and Members
        protected string _ValueIfBlank = "";
        public string ValueIfBlank
        {
            get { return _ValueIfBlank; }
            set { _ValueIfBlank = value; }
        }

        protected string _Prefix = "";
        public string Prefix
        {
            get { return _Prefix; }
            set { _Prefix = value; }
        }

        protected string _Suffix = "";
        public string Suffix
        {
            get { return _Suffix; }
            set { _Suffix = value; }
        }

        // public StringBuilder VirtualData;


        /// <summary>
        /// A collection of substitute value records
        /// </summary>
        protected List<TSubstituteValues> _SubstituteValues;
        public List<TSubstituteValues> SubstituteValues
        {
            get { return _SubstituteValues; }
            set { _SubstituteValues = value; }
        }

        protected ListObjBase<TTableVirtualFldDef> _Fields;
        public ListObjBase<TTableVirtualFldDef> Fields
        {
            get { return _Fields; }
            set { _Fields = value; }
        }

        protected ListObjBase<TTableFldDef> _OwnedFields;
        public ListObjBase<TTableFldDef> OwnedFields
        {
            get { return _OwnedFields; }
            set { _OwnedFields = value; }
        }
        #endregion

        protected TTableVirtualFldDef fOwnerFld;

        public TTableVirtualFldDef fParentVirtualFld;

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);

            if (_Size <= 0)
                Size = ReinoTablesConst.VIRTUAL_COLUMN_FIELDLENGTH;

            fNotAddedToTableDef = true;
            if (fParentVirtualFld != null)
            {
                fParentVirtualFld.AddDataField(this);
                fParentVirtualFld.AddOwnedField(this);
            }

            // not required since this are referencing pre-existing fields??
            //_Fields.PostDeserializeListItems(iParent);
            //_OwnedFields.PostDeserializeListItems(iParent);

            return 0;
        }

        // constructors
        public TTableVirtualFldDef()
            : base()
        {
            IsVirtual = true;
            // this.VirtualData = new StringBuilder();
            this._Fields = new ListObjBase<TTableVirtualFldDef>();
            this._OwnedFields = new ListObjBase<TTableFldDef>();
            this._SubstituteValues = new List<TSubstituteValues>();
        }


        protected String GetSubstituteValue(String iSubstituteFrom)
        {
            TSubstituteValueFindPredicate loPredicate = new TSubstituteValueFindPredicate(iSubstituteFrom);
            TSubstituteValues loSubVal = SubstituteValues.Find(loPredicate.CompareByFromValues);

            if (loSubVal == null)
            {
                return null;
            }
            else
            {
                // If the substitute value is blank in the layout tool, then it ends up as null
                // when client config is deserialized. In this case, we want to use a blank string.
                if (loSubVal.fSubstituteTo == null)
                    return "";
                else
                    return loSubVal.fSubstituteTo;
            }

        }

        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            int loTableFldNo;
            TTableFldDef loAliasedFld;
            String loSubToValue;
            string loTempDataValue;
            oResult = "";
            StringBuilder VirtualData = new StringBuilder();

            /* a virtual field consists of a prefix, one or more real or virtual data fields, a suffix,
              and a value to print if the data is blank.  The 1st step is to collect all of our data */

            /* whoops, when converting from statically sized fields (80 bytes) to dynamically sized, I failed to consider
              existing set-ups where a field had a declared size that was too small. For backward compatibility, let's
              make the minimum size to be 80 bytes*/
            if (Size < 128)
            {
                Size = 128;
            }
            //fData = (char *)realloc_IfNecessary( fData, GetSize()+1 );
            VirtualData.Length = 0;
            loTempDataValue = "";

            /* A virtual field declared at the level as other table fields is a real field (almost).
              Virtual fields declared under a parent virtual field are only aliases to the actual
              field declaration... */
            loTableFldNo = iSrcTable.fTableDef.GetFldNo(this.Name);
            loAliasedFld = iSrcTable.fTableDef.GetField(loTableFldNo);

            if ((loAliasedFld != null) && (loAliasedFld != this))
            {
                // virtual field needs to be big enough to accomodate its physical self
                if (loAliasedFld.Size > Size)
                {
                    Size = loAliasedFld.Size;
                    //fData = (char *)realloc_IfNecessary( fData, GetSize()+1 );
                    VirtualData.Length = 0;
                }

                // JIRA: [AUTOCITE-355] Need to use the passed mask instead of the internal field mask
                loAliasedFld.ConvertFromStoreFormat(iSrcTable, loTableFldNo, iDataMask/*Mask*/, ref loTempDataValue);
                VirtualData.Length = 0;
                VirtualData.Append(loTempDataValue.ToString());
            }


            // go through each of the fields in the list
            foreach (TTableVirtualFldDef loFld in this.Fields)
            {
                loTempDataValue = "";
                loFld.ConvertFromStoreFormat(iSrcTable, -1, loFld.MaskForHH, ref loTempDataValue);
                VirtualData.Append(loTempDataValue);
                //GetDataField(loNdx).ConvertFromStoreFormat(iSrcTable, -1, GetDataField(loNdx).GetMask(), &fData[strlen(fData)]);
            }

            if (VirtualData.Length == 0)
            {
                // default value if blank?
                if (this.ValueIfBlank.Length > 0)
                {
                    oResult = this.ValueIfBlank;
                }
            }
            else
            {
                ///... not blank, build the result ...

                // Is there a prefix to start? 
                // Note that we had to enclose prefix and suffix values with double quotes 
                // in the XML file, so we need to remove them before using the value 
                string ConvertedPrefix = Prefix;
                if ((ConvertedPrefix.StartsWith("\"")) && (ConvertedPrefix.EndsWith("\"")))
                {
                    ConvertedPrefix = ConvertedPrefix.Remove(0, 1);
                    ConvertedPrefix = ConvertedPrefix.Remove(ConvertedPrefix.Length - 1, 1);
                }
                if (ConvertedPrefix != "")
                {
                    oResult = ConvertedPrefix.Substring(0, Math.Min(ConvertedPrefix.Length, Size));
                }


                // do we want to replace this value?
                loSubToValue = GetSubstituteValue(VirtualData.ToString());
                if ((loSubToValue != null) /*&& (loSubToValue.Length > 0)*/) // Sometimes the substitute value is supposed to be blank
                {
                    // why trash the prefix too?  (This was dropping the Prefix used in Banyule's keying line, resulting in different value than produced by the X3
                    /*oResult = loSubToValue;*/
                    oResult += loSubToValue;
                }
                else
                {
                    oResult += VirtualData.ToString();
                }

                // a suffix to add? 
                // Note that we had to enclose prefix and suffix values with double quotes 
                // in the XML file, so we need to remove them before using the value 
                string ConvertedSuffix = Suffix;
                if ((ConvertedSuffix.StartsWith("\"")) && (ConvertedSuffix.EndsWith("\"")))
                {
                    ConvertedSuffix = ConvertedSuffix.Remove(0, 1);
                    ConvertedSuffix = ConvertedSuffix.Remove(ConvertedSuffix.Length - 1, 1);
                }
                if (ConvertedSuffix.Length > 0)
                {
                    oResult += ConvertedSuffix;
                }
            }

            // ultimately, we can't exceed our size limit
            if (oResult.Length > Size)
            {
                oResult = oResult.Substring(0, Size);
            }

            return 0;
        }

        protected TTableVirtualFldDef GetDataField(short iItemNo)
        {
            //return (TTableVirtualFldDef*)fDataFields.GetObj(iItemNo);
            return (TTableVirtualFldDef)Fields[iItemNo];
        }

        //public methods
        public override int GetComponentFieldCnt()
        {
            return OwnedFields.Count;
        }

        public override TTableFldDef GetComponentField(int iIndexNo)
        {
            return OwnedFields[iIndexNo];
        }

        public void AddDataField(TTableVirtualFldDef iTableFldDef)
        {
            Fields.Add(iTableFldDef);
        }

        public void AddOwnedField(TTableVirtualFldDef iTableFldDef)
        {
            if (fParentVirtualFld != null)
                fParentVirtualFld.AddOwnedField(iTableFldDef);
            else
                OwnedFields.Add(iTableFldDef);
        }

        public override TTableFldDef Clone()
        {
            // Use parent to clone our basic and parent properties.
            TTableFldDef fieldCopy = base.Clone();
            // Also clone our substitute list. Have to create new object since parent set pointer
            // to this list equal to pointer of our list.
            (fieldCopy as TTableVirtualFldDef).SubstituteValues = new List<TSubstituteValues>();
            foreach (TSubstituteValues oneSubstitute in SubstituteValues)
            {
                (fieldCopy as TTableVirtualFldDef).SubstituteValues.Add(oneSubstitute.Clone());
            }

            // And our field list.
            (fieldCopy as TTableVirtualFldDef).Fields = new ListObjBase<TTableVirtualFldDef>();
            foreach (TTableVirtualFldDef oneField in Fields)
            {
                (fieldCopy as TTableVirtualFldDef).Fields.Add((TTableVirtualFldDef)oneField.Clone());
            }

            // Not sure what to do about the pointer objects that point to owner or parent.
            // For now will just leave them and just return the cloned object we made.
            return fieldCopy;
        }
    }

    /// <summary>
    /// Summary description for TTableVTableLinkFldDef.
    /// 
    ///  TTableVTableLinkFldDef is an intermediate field used to link two tables together.
    /// 
    /// </summary>

    public class TTableVTableLinkFldDef : TTableVirtualFldDef
    {
        #region Properties and Members
        protected string _LinkTableName = "";
        public string LinkTableName
        {
            get { return _LinkTableName; }
            set { _LinkTableName = value; }
        }

        protected string _MasterColName = "";
        public string MasterColName
        {
            get { return _MasterColName; }
            set { _MasterColName = value; }
        }

        protected string _LinkColName = "";
        public string LinkColName
        {
            get { return _LinkColName; }
            set { _LinkColName = value; }
        }
        #endregion

        protected TTTable fLinkTable;
        protected int fMasterPrimaryKey;
        protected int fMasterChangeNo;
        protected string fLinkValue;

        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            // our job is to find the row in the linked table that aligns with the master table
#if !WindowsCE && !__ANDROID__  
            if (TClientDef.GuaranteedThreadSafe == false)
                throw new Exception("TTableVTableLinkFldDef instance " + this.Name + " has invoked ConvertFromStoreFormat which is not implemented in a thread-safe manner.");
#endif

            if (fLinkTable == null)
            {
                fLinkTable = new TTTable();
                fLinkTable.Name = this._LinkTableName;
                fLinkTable.SetTableName(_LinkTableName);
                fLinkTable.PostDeserialize(null);
            }

            // avoid unnecessary searches
            if ((fMasterChangeNo == iSrcTable.GetChangeNo()) &&
                (fMasterPrimaryKey == iSrcTable.GetPrimaryKey())) return 0;

            // get the filter field value
            fMasterPrimaryKey = iSrcTable.GetPrimaryKey();
            fMasterChangeNo = iSrcTable.GetChangeNo();
            if (string.Compare(_MasterColName, DBConstants.sqlMasterKeyStr, true) == 0)
            {
                fLinkValue = fMasterPrimaryKey.ToString();
                fLinkTable.AddFilter(_LinkColName, fLinkValue, "-9999999");
            }
            else
            {
                iSrcTable.GetFormattedFieldData(_MasterColName, null, ref fLinkValue);
                fLinkTable.AddFilter(_LinkColName, fLinkValue, null);
            }
            fLinkTable.ReadRecord(0);

            return 0;
        }

        public TTTable GetLinkTable()
        {
            return fLinkTable;
        }


        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TTableVTableLinkFldDef()
            : base()
        {
            fMasterPrimaryKey = -1;
            fMasterChangeNo = -1;
        }


        ~TTableVTableLinkFldDef()
        {
            // no longer required under managed code?
            //if (fLinkTable) delete fLinkTable; 
        }
    }


    /// <summary>
    /// Summary description for TTableVLinkedFldDef.
    /// </summary>  
    public class TTableVLinkedFldDef : TTableVirtualFldDef
    {
        #region Properties and Members
        protected string _LinkFldName = "";
        public string LinkFldName
        {
            get { return _LinkFldName; }
            set
            {
                _LinkFldName = value;
                _LinkFld = null;
            }
        }

        protected TTableVTableLinkFldDef _LinkFld = null;
        #endregion

        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            // a linked field gets its data from another table.  
            // That table is (and the row in the table)
            // are accessed via the "LinkFld".  Our first step will be to 
            // find the link field and make it align rows. 
#if !WindowsCE && !__ANDROID__  
            if (TClientDef.GuaranteedThreadSafe == false)
                throw new Exception("TTableVLinkedFldDef instance " + this.Name + " has invoked ConvertFromStoreFormat which is not implemented in a thread-safe manner.");
#endif

            String loDummyParm = "";
            oResult = "";

            // Try to resolve field by name if we don't have it yet
            if (_LinkFld == null)
                _LinkFld = (TTableVTableLinkFldDef)fParentTable.GetField(_LinkFldName);
            // Exit if link field still doesn't exist
            if (_LinkFld == null)
                return 0;

            // tell the link field to find the proper row based on the connection criteria stored in the link field.
            _LinkFld.ConvertFromStoreFormat(iSrcTable, -1, null, ref loDummyParm);

            // now get our data using the link table
            return base.ConvertFromStoreFormat(_LinkFld.GetLinkTable(), -1, null, ref oResult);
        }


        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TTableVLinkedFldDef()
            : base()
        {
        }

    }

    /// <summary>
    /// Summary description for TTableElpsTimeVirtualFldDef.
    /// </summary>
    public class TTableElpsTimeVirtualFldDef : TTableVirtualFldDef
    {
        #region Properties and Members
        protected string _DateFld = "";
        public string DateFld
        {
            get { return _DateFld; }
            set
            {
                _DateFld = value;
                fDateFld = new TTableVirtualFldDef();
                fDateFld.Name = _DateFld;
            }


        }

        protected string _TimeFld = "";
        public string TimeFld
        {
            get { return _TimeFld; }
            set
            {
                _TimeFld = value;
                fTimeFld = new TTableVirtualFldDef();
                fTimeFld.Name = _TimeFld;
            }

        }
        #endregion

        public override int Size
        {
            get { return _Size; }
            set { _Size = value; }
        }

        protected TTableVirtualFldDef fDateFld;
        protected TTableVirtualFldDef fTimeFld;


        // move this to ReinoTextBoxBehaviour?
        /// <summary>
        /// Converts a date and time value from two different strings into a single DateTime value
        /// </summary>
        /// <param name="iDateMask"></param>
        /// <param name="iDateStr"></param>
        /// <param name="iTimeMask"></param>
        /// <param name="iTimeStr"></param>
        /// <returns></returns>
        public DateTime DateAndTimeStringsToSingleDateTime(String iDateMask, String iDateStr, String iTimeMask, String iTimeStr)
        {
            DateTime loOSDate = new DateTime();
            DateTime loOSTime = new DateTime();
            DateTime loCombinedDateTime = new DateTime();
            int loResult = 0;

            // defense
            if ((iDateMask == null) || (iDateStr == null) || (iTimeMask == null) || (iTimeStr == null))
            {
                return loCombinedDateTime;
            }

            // convert the datestring to an OSDate 
            if ((loResult = ReinoControls.TextBoxBehavior.DateStringToOSDate(iDateMask, iDateStr, ref loOSDate)) < 0)
            {
                Console.WriteLine("Date Convert failed: %s %s, code:%X\n", iDateMask, iDateStr, loResult);
                return loCombinedDateTime; //  date conversion failed 
            }

            // convert the timestring to an OSTime 
            if ((loResult = ReinoControls.TextBoxBehavior.TimeStringToOSTime(iTimeMask, iTimeStr, ref loOSTime)) < 0)
            {
                Console.WriteLine("Time Convert failed: %s %s, code:%X\n", iTimeMask, iTimeStr, loResult);
                return loCombinedDateTime; // time conversion failed 
            }

            // combine OSDate and OSTime into single DateTime 
            // Remember: These Date/Time math functions return values rather than modifying an instance variable!
            loCombinedDateTime = loOSDate;
            loCombinedDateTime = loCombinedDateTime.AddHours(loOSTime.Hour);
            loCombinedDateTime = loCombinedDateTime.AddMinutes(loOSTime.Minute);
            loCombinedDateTime = loCombinedDateTime.AddSeconds(loOSTime.Second);
            loCombinedDateTime = loCombinedDateTime.AddMilliseconds(loOSTime.Millisecond);

            return loCombinedDateTime;
        }

        /// <summary>
        /// Places a formated string according to iMask into oResult for the time elapsed stored in 
        /// iSrcStr.  iElapsedTime should be an TimeSpan representing the elapsed time span.
        ///
        /// Mask options are:
        /// dd -> number of days that have elapsed.
        /// hh -> number of hours that have elapsed.  Can be over 24 if "dd" is not included in the mask.
        /// mm -> number of minutes that have elapsed
        /// ss -> number of seconds that have elapsed.
        /// </summary>
        private void FormatTimeSpan(TimeSpan iElapsedTime, string iPictureMask, ref string oResult)
        {
            int loSeconds = iElapsedTime.Seconds;
            int loPos;
            string loNumStr = "";

            oResult = iPictureMask;
            if (loSeconds < 0)
            {
                oResult = "-" + oResult;
                loSeconds = Math.Abs(loSeconds);
            }

            // Are days part of mask?
            if (((loPos = oResult.IndexOf("dd")) >= 0) ||
                 ((loPos = oResult.IndexOf("DD")) >= 0))
            {
                oResult = oResult.Remove(loPos, 2);
                if (iElapsedTime.Days > 0)
                {
                    loNumStr = iElapsedTime.Days.ToString().PadLeft(2, '0') + " days";
                    oResult = oResult.Insert(loPos, loNumStr);
                }
            }

            // Are hours part of mask?
            if (((loPos = oResult.IndexOf("hh")) >= 0) ||
                 ((loPos = oResult.IndexOf("HH")) >= 0))
            {
                oResult = oResult.Remove(loPos, 2);
                loNumStr = iElapsedTime.Hours.ToString().PadLeft(2, '0');
                oResult = oResult.Insert(loPos, loNumStr);
            }

            // Are minutes part of mask?
            if (((loPos = oResult.IndexOf("mm")) >= 0) ||
                 ((loPos = oResult.IndexOf("MM")) >= 0))
            {
                oResult = oResult.Remove(loPos, 2);
                loNumStr = iElapsedTime.Minutes.ToString().PadLeft(2, '0');
                oResult = oResult.Insert(loPos, loNumStr);
            }

            // Are seconds part of mask?
            if (((loPos = oResult.IndexOf("ss")) >= 0) ||
                 ((loPos = oResult.IndexOf("SS")) >= 0))
            {
                oResult = oResult.Remove(loPos, 2);
                loNumStr = iElapsedTime.Seconds.ToString().PadLeft(2, '0');
                oResult = oResult.Insert(loPos, loNumStr);
            }
            return;
        }

        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            short loStatus = 0;
            oResult = "";

            // make sure we have a date & time to compare.
            if ((fDateFld == null) || (fTimeFld == null)) return 0;

            // elapsed time will be stored as the number of seconds between the current row's 
            //  issue date/time and the comparison row's issue date/time.

            DateTime loFromTime;
            DateTime loToTime;
            TimeSpan loTimeDiff;

            String loDateRawData = fDateFld.GetRawFieldData(iSrcTable, -1);
            String loTimeRawData = fTimeFld.GetRawFieldData(iSrcTable, -1);

            if (loDateRawData.Equals("") || loTimeRawData.Equals(""))
                return 0;

            //Debug.Write( "\n%s %s %s %s", fDateFld->GetRawComparisonFieldData(iSrcTable, -1), fTimeFld->GetRawComparisonFieldData(iSrcTable, -1),
            //        fDateFld->GetRawFieldData(iSrcTable, -1), fTimeFld->GetRawFieldData(iSrcTable, -1) );


            //loToTime = DateTimeStringToRTCDateTime(ReinoTablesConst.DATE_TYPE_DATAMASK, fDateFld.GetRawComparisonFieldData(iSrcTable, -1),
            //                                        ReinoTablesConst.TIME_TYPE_DATAMASK, fTimeFld.GetRawComparisonFieldData(iSrcTable, -1));



#if __ANDROID__
//#if PATROL_CAR_AIR
            loToTime = DateTime.Now;
#else
            loToTime = DateAndTimeStringsToSingleDateTime(
                            ReinoTablesConst.DATE_TYPE_DATAMASK, fDateFld.GetRawComparisonFieldData(iSrcTable, -1),
                            ReinoTablesConst.TIME_TYPE_DATAMASK, fTimeFld.GetRawComparisonFieldData(iSrcTable, -1)
                            );
#endif


            //loFromTime = DateTimeStringToRTCDateTime(ReinoTablesConst.DATE_TYPE_DATAMASK, loDateRawData,
            //                                      ReinoTablesConst.TIME_TYPE_DATAMASK, loTimeRawData);
            loFromTime = DateAndTimeStringsToSingleDateTime(
                            ReinoTablesConst.DATE_TYPE_DATAMASK, loDateRawData,
                            ReinoTablesConst.TIME_TYPE_DATAMASK, loTimeRawData
                            );
            // Figure out the difference then format to desired mask
            loTimeDiff = loToTime - loFromTime;
            FormatTimeSpan(loTimeDiff, iDataMask, ref oResult);
            return loStatus;
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            if (_Size <= 0)
                _Size = ReinoTablesConst.INT_TYPE_FIELDLENGTH;
            return 0;
        }

        public TTableElpsTimeVirtualFldDef()
            : base()
        {
        }

        ~TTableElpsTimeVirtualFldDef()
        {
            // nothing to do
        }
    }


    /// <summary>
    /// Summary description for TTableMod97VirtualFldDef.
    /// </summary>
    public class TTableMod97VirtualFldDef : TTableVirtualFldDef
    {
        #region Properties and Members
        #endregion

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TTableMod97VirtualFldDef()
            : base()
        {
        }

        public static string ThreeDigitIdentifier = "272";

        public static int[] PrimeNumbers = new int[] 
                    { 3,   5,  7,  11,  13,  17,  19,  23,  29,  
                     31,  37,  41, 43,  47,  53,  59,  61,  67,  71 };

        int PrimeNumberCnt = PrimeNumbers.Length;

        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            int loNdx;
            int loDigitCnt;
            int loSumSoFar;
            int loDigitChar;

            // inherited version will build the string that we will then place a check digit on.
            base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);

            // no check digits on a blank string
            if (oResult.Length == 0) return 0;

            // The number that will be used in the check digit calculation will be the TID (three digit identifier)
            // plus the ticket number. The ticket number has to have its last digit use the weight that is the 
            // last prime number in the prime number list, so we have to left pad the ticket number with zeros
            // to take up space between the TID and ticket number.
            string calculationString = ThreeDigitIdentifier + oResult.PadLeft(PrimeNumberCnt - ThreeDigitIdentifier.Length, '0');

            // loop through the data, summing the product of each digit and its corresponding weight
            for (loSumSoFar = 0, loNdx = 0, loDigitCnt = 0; loNdx < calculationString.Length; loNdx++)
            {
                // get the next char
                loDigitChar = calculationString[loNdx];

                // non-digits are a problem
                if ((loDigitChar > '9') || (loDigitChar < '0'))
                {
                    // Even though this non-digit or space is ignored, we will still go onto the next prime number.
                    loDigitCnt++;
                    continue; // mcb 2/26/07: ignore spaces and such.
                    /*
                    //String loLine1;
                    //uHALr_FormatStr( "%s contains non-numerics!", loLine1, 80, oResult );
                    //ShowMessage( "Failed Mod97 Check Digit", loLine1 );
                    oResult = "";
                    return -1;
                     */
                }


                // can't exceed PrimeNumberCnt digits
                if (loDigitCnt == PrimeNumberCnt)
                { // whoops!
                    //String loLine1;
                    //uHALr_FormatStr( "%s exceeds %d characters!", loLine1, 80, oResult, PrimeNumberCnt );
                    //ShowMessage( "Failed Mod97 Check Digit", loLine1 );
                    oResult = "";
                    return -1;
                }

                // passed our checks, multiply the position's weight and 
                // add to running total and increment digit cnt
                // Just for you Alan, it's done in a single line.
                loSumSoFar += (loDigitChar - '0') * PrimeNumbers[loDigitCnt++];
            }

            // check digit is sum of weighted digits mod 97, complemented.
            loSumSoFar %= 97;

            /// mcb 12/15/03: Apparently no adjustment for 00 should be made; i.e, 97 is a valid value.  
            /// This change was prompted by Banyule in Series D software. 
            ///  if (loSumSoFar) loSumSoFar = 97 - loSumSoFar;
            loSumSoFar = 97 - loSumSoFar;

            // convert it to a string and append it to result
            // Will now just return the check digit part instead of entire ticket number plus check digit.
            //            oResult += Convert.ToString(loSumSoFar).PadLeft(2, '0');
            oResult = Convert.ToString(loSumSoFar).PadLeft(2, '0');

            return 0;
        }
    }


    /// <summary>
    /// Summary description for TTableMod10VirtualFldDef.
    /// </summary>
    public class TTableMod10VirtualFldDef : TTableVirtualFldDef
    {
        #region Properties and Members
        #endregion


        /// <summary>
        /// Fixed v1.07, 11/6/03.
        /// Prior version did not sum the digits of the intermediate weighted sums. 
        /// For example, digit "7" with weight "2" should be 5 (2 * 7 = 14, 1 + 4 =5), not 14 (just 2 * 7)
        /// </summary>
        protected void CalcMod10CheckDigitStr(ref string ioSrcStr, bool iCDW2)
        {
            int loLen;
            int loSum = 0;
            int loNdx;
            int loWeightIsTwo;
            int quot = 0;
            int rem = 0;
            int loCheckDigit = 0;

            loLen = ioSrcStr.Length;
            if (iCDW2 == true)
                loWeightIsTwo = 0;
            else
                loWeightIsTwo = 1;

            for (loNdx = loLen - 2; loNdx >= 0; loNdx--)
            {
                // ignore spaces
                if (ioSrcStr[loNdx] == ' ')
                    continue;
                quot = ((ioSrcStr[loNdx] - 0x30) * (1 + loWeightIsTwo)) / 10;
                rem = ((ioSrcStr[loNdx] - 0x30) * (1 + loWeightIsTwo)) % 10;
                loSum += quot + rem;
                if (loWeightIsTwo == 1)
                    loWeightIsTwo = 0;
                else
                    loWeightIsTwo = 1;
            }

            loCheckDigit = 10 - (loSum % 10);
            if (loCheckDigit == 10) loCheckDigit = 0;
            ioSrcStr = ioSrcStr.Substring(0, ioSrcStr.Length - 1) + loCheckDigit.ToString();
        }

        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            // inherited version will build the string that we will then place a check digit on.
            base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);

            // no check digits on a blank string
            if (oResult.Length == 0) return 0;

            // Add a character at the end for the check digit. 
            // CalcMod10CheckDigitStr expects the check digit position to be occupied
            oResult = oResult + "x";

            // calc it!
            CalcMod10CheckDigitStr(ref oResult, false);
            return 0;
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TTableMod10VirtualFldDef()
            : base()
        {
        }
    }

    /// <summary>
    /// Summary description for TTableMod10CDOnlyVirtualFldDef.
    /// </summary>
    public class TTableMod10CDOnlyVirtualFldDef : TTableMod10VirtualFldDef
    {
        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            // inherited version will build the string with checkdigit on the end. All we need to do is get the last character.
            base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);

            // no check digits on a blank string
            if (oResult.Length == 0) return 0;

            // now, grab the check digit and move it to the 1st position.
            if (oResult.Length > 0)
                oResult = oResult.Substring(oResult.Length - 1, 1);
            return 0;
        }

        public TTableMod10CDOnlyVirtualFldDef()
            : base()
        {
        }
    }

    /// <summary>
    /// TTableMod10AustPostVirtualFldDef is just another name for TTableMod10VirtualFldDef
    /// </summary>
    public class TTableMod10AustPostVirtualFldDef : TTableMod10VirtualFldDef
    {
        public TTableMod10AustPostVirtualFldDef()
            : base()
        {
        }
    }

    /// <summary>
    /// TTableMod10FirstCharVirtualFldDef.
    /// Identical to TTableMod10VirtualFldDef except that 
    /// check digit is placed in first character position, not last.
    /// </summary>
    public class TTableMod10FirstCharVirtualFldDef : TTableVirtualFldDef
    {
        #region Properties and Members
        #endregion

        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            // initialize to blank
            oResult = "";

            // inherited version will build the string that we will then place a check digit on.
            base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);

            // no check digits on a blank string
            if (oResult.Length == 0) return 0;

            // Add a character at the end for the check digit. 
            // CalcMod10CheckDigitStr expects the check digit position to be occupied 
            oResult = oResult + "x";

            // calc it!
            // need common call CalcMod10CheckDigitStr(ref oResult, 0);

            // move the check digit from the last position to the first
            oResult = oResult.Substring(oResult.Length - 1, 1) + oResult.Substring(0, oResult.Length - 2);

            return 0;
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TTableMod10FirstCharVirtualFldDef()
            : base()
        {
        }

    }

    /// <summary>
    /// TTableOttawaMod10VirtualFldDef - OttawaMod10
    /// Takes a string, converts all alphas to their position in the alphabet mod 10, 
    /// then adds a mod 10 check digit calculation. The mod10 differs from the 
    /// regular mod 10 in that the first digit is always weight 2 regardless of
    /// overall string length. Also, the individual digits of the intermediate sums 
    /// are not added together. For example, digit 7 in an odd position has a 
    /// weight of 2 and a weighted sum of 14. This sum is added directly to 
    /// the running total, rather than add the 1 and 4 together to get 5.
    /// </summary>
    public class TTableOttawaMod10VirtualFldDef : TTableVirtualFldDef
    {
        #region Properties and Members
        #endregion

        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            int loNdx;
            int loSum = 0;
            String loTempStr;
            char loChar;

            // inherited version will build the string that we will then place a check digit on.
            base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);

            // no check digits on a blank string
            if (oResult.Length == 0) return 0;

            // work through each digit and build a new string as we go
            loTempStr = oResult;
            oResult = "";
            for (loNdx = 0; loNdx < loTempStr.Length; loNdx++)
            {
                loChar = loTempStr[loNdx];
                // is this an numeric? convert to position
                if ((loChar >= '0') && (loChar <= '9'))
                    loChar = (char)((int)(loChar - '0'));
                else if ((loChar >= 'A') && (loChar <= 'Z'))
                    loChar = (char)((int)((loChar - 'A') + 1) % 10); // 'A' is in 1st position, not 0th
                else if ((loChar >= 'a') && (loChar <= 'z'))
                    loChar = (char)((int)((loChar - 'a') + 1) % 10); // 'a' is in 1st position, not 0th
                else
                    continue; // All other chars are treated as 0's and don't affect the sum.

                // convert letters to numbers.
                oResult += (char)((int)(loChar + '0'));

                // gotta love '? :' statements. If loNdx is odd, add char to sum. If it is even, add 2 x char to sum.
                loSum += (loNdx % 2 == 1) ? ((int)(loChar)) : ((int)(loChar)) << 1;
                //loSum += loNdx;
            }

            // convert sum to a check digit.
            loSum %= 10;  // 1) Mod 10.
            if (loSum != 0) loSum = 10 - loSum; // 2) If not 0, 10 - (sum mod 10)
            oResult += (char)((int)(loSum + '0'));  // convert it back to a char and append to result string

            return 0;
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TTableOttawaMod10VirtualFldDef()
            : base()
        {
        }
    }

    /// <summary>
    /// TTableAlphaPosMod10VirtualFldDef.
    /// Converts all chars to their position in the alphabet mod 10, with A & a = 1.
    /// </summary>
    public class TTableAlphaPosMod10VirtualFldDef : TTableVirtualFldDef
    {

        #region Properties and Members
        #endregion

        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            int loNdx;
            char loChar;
            String loTempStr;

            // inherited version will build the string that we will then place a check digit on.
            base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);


            // build the new string char by char
            loTempStr = oResult;
            oResult = "";
            for (loNdx = 0; loNdx < loTempStr.Length; loNdx++)
            {
                // get the next char
                loChar = loTempStr[loNdx];

                // is this an alpha? convert to ordinal
                if ((loChar >= 'A') && (loChar <= 'Z'))
                    loChar -= 'A'; // 'A' is in 1st position, not 0th
                else if ((loChar >= 'a') && (loChar <= 'z'))
                    loChar -= 'a'; // 'a' is in 1st position, not 0th
                else
                {
                    oResult += loChar;
                    continue; // Not an alpha so no convertion necessary
                }

                // convert letters to numbers.
                oResult += Convert.ToString((loChar + 1) % 10);
            }

            return 0;
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TTableAlphaPosMod10VirtualFldDef()
            : base()
        {
        }
    }


    public class TTableLongBeachMod10VirtualFldDef : TTableVirtualFldDef
    {

        #region Properties and Members
        #endregion

        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            // Inherited version will build the string that we will use to generate the check digit.
            base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);

            // Long Beach generates the check digit by multiplying each digit in the source string (probably
            // an issue number) by either a 1 or a 2. Going from right to left, will first multiply by 2, then
            // by 1, then by 2, then 1, etc. etc. After multiplying each add up these values and finally mod by
            // 10 to get the check digit. For example, if had ticket number 3412, would do:
            // (3 * 1) + (4 * 2) + (1 * 1) + (2 * 2) = 3 + 8 + 1 + 4 = 16 mod 10 = 6.
            // Also, if any of the numbers multiplied are > 10, then use the following conversion:
            // if multiplied number = 10, then use 1.
            // if number = 12, then use 3.
            // if 14, use 5.
            // if 16, use 7.
            // if 18, use 9.
            // The easy calculation for this of course is, if number > 10, then use number - 9.
            int sum = 0;
            int weight = 2;
            for (int charIdx = oResult.Length; charIdx > 0; charIdx--)
            {
                try
                {
                    // Convert this character to a number. If the character is not a number then it will
                    // go to the catch part of code and we will effectively skip this char.
                    int charNumber = Convert.ToInt32(oResult[charIdx].ToString());
                    // Multiply this character by the weight. If its > 10 then subtract 9 from it.
                    int multipliedProduct = charNumber * weight;
                    if (multipliedProduct > 9) { multipliedProduct -= 9; }
                    // Add this to the running total.
                    sum += multipliedProduct;

                    // Alternate the weight between 1 and 2.
                    if (weight == 1) { weight = 2; }
                    else { weight = 1; }
                }
                catch
                {
                }
            }

            // The check digit is the last char in the sum (which we get by doing a mod 10).
            int checkDigit = sum % 10;
            // We will return the checkdigit appended to the end of the original source string
            oResult = oResult + checkDigit.ToString();

            return 0;
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TTableLongBeachMod10VirtualFldDef()
            : base()
        {
        }
    }


    public class TTableAFPMod11VirtualFldDef : TTableVirtualFldDef
    {

        #region Properties and Members
        #endregion

        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            // Inherited version will build the string that we will use to generate the check digit.
            base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);

            // This code was added to make the .NET software match the results of the HH MFC app. As it turns out,
            // the HH MFC app was wrong, so we don't need to do this anymore
            /*
            // On the handheld, the source is suffixed with a lowercase "x", so we need to do it here also.
            oResult += "x";
            */

            // The AFP Mod 11 check digit uses starting weight of 2 and goes up from there (from least to most
            // significant digit). We will multiply each digit in source string to its weight and then mod
            // the sum by 11. If the mod = 10 then will just use 0 for check digit, otherwise will use the
            // mod result as the check digit. For example, if had ticket 3412, would do:
            // (3 * 5) + (4 * 4) + (1 * 3) + (2 * 2) = 15 + 16 + 3 + 4 = 38 mod 11 = 5.
            int sum = 0;
            int weight = 2;
            for (int charIdx = oResult.Length - 1; charIdx >= 0; charIdx--)
            {
                try
                {
                    // Convert this character to a number. If the character is not a number then it will
                    // go to the catch part of code and we will effectively skip this char.
                    int charNumber = (int)oResult[charIdx] - 48; // Cant do convert string to int because we have an alpha
                    // Multiply this character by the weight. If its > 10 then subtract 9 from it.
                    int multipliedProduct = charNumber * weight;
                    // Add this to the running total.
                    sum += multipliedProduct;

                    // Always increase the weight after each digit.
                    weight++;
                }
                catch
                {
                }
            }

            // Get the check digit by using mod 11.
            int checkDigit = sum % 11;
            // If its too big (equals 10), then set check digit to 0.
            if (checkDigit == 10) { checkDigit = 0; }
            // We will only return the check digit portion - not the full issue number + check digit.
            oResult = checkDigit.ToString();

            return 0;
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TTableAFPMod11VirtualFldDef()
            : base()
        {
        }
    }

    public class TTableArlingtonMod11VirtualFldDef : TTableVirtualFldDef
    {
        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            // Inherited version will build the string that we will use to generate the check digit.
            base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);

            // The Arlington Mod 11 check digit uses starting weight of 2 and goes up to 7. After 7 the weights
            // go back down to 2 again. So, for example, if had ticket: 634002187 then would add weights like:
            // Ticket: 634002187
            // Weight: 432765432
            // Equals: (6 * 4) + (3 * 3) + (4 * 2) + (0 * 7) + (0 * 6) + (2 * 5) + (1 * 4) + (8 * 3) + (7 * 2)
            //         24 + 9 + 8 + 0 + 0 + 10 + 4 + 24 + 14 = 93
            // Once get all weights added in take the sum and mod by 11.
            // 93 MOD 11 = 5
            // Then subtract from 11 to get the actual check digit.
            // 11 - 5 = 6.
            // If the check digit is > 9 then use A for 10 and H for 11.
            // Another example:
            // Ticket: 170017
            // Weight: 765432
            // Equals: (1 * 7) + (7 * 6) + (0 * 5) + (0 * 4) + (1 * 3) + (7 * 2) = 7 + 42 + 0 + 0 + 3 + 14 = 66.
            // 66 MOD 11 = 0. 11 - 0 = 11. So check digit would be H.
            int sum = 0;
            int weight = 2;
            for (int charIdx = oResult.Length - 1; charIdx >= 0; charIdx--)
            {
                try
                {
                    // Once weight past 7 then goes back down to 2.
                    if (weight > 7) { weight = 2; }
                    // Convert this character to a number. If the character is not a number then it will
                    // go to the catch part of code and we will effectively skip this char.
                    int charNumber = Convert.ToInt32(oResult[charIdx].ToString());
                    // Multiply this character by the weight. 
                    int multipliedProduct = charNumber * weight;
                    // Add this to the running total.
                    sum += multipliedProduct;

                    // Always increase the weight after each digit.
                    weight++;
                }
                catch
                {
                }
            }

            // Get the check digit by using mod 11 and then subtracting mod result from 11.
            int modNumber = sum % 11;
            int checkDigit = 11 - modNumber;
            // If its too big (equals 10 or 11), then set check digit to A or H.
            string digitString = checkDigit.ToString();
            if (checkDigit == 10) { digitString = "A"; }
            else if (checkDigit == 11) { digitString = "H"; }
            // We will only return the check digit portion - not the full issue number + check digit.
            oResult = digitString;

            return 0;
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TTableArlingtonMod11VirtualFldDef()
            : base()
        {
        }
    }

    public class TTableNillumbikMod10VirtualFldDef : TTableVirtualFldDef
    {
        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            // Inherited version will build the string that we will use to generate the check digit.
            base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);

            // The Nillumbik oResult string should now be concatenation of Ticket#, Due Date, and Amount (pay amount?).
            // So, should look something like:
            // D000065626200807006000
            // Where Ticket Number = D000065626, Due Date = 200807, and Amount = 006000
            // The Nillumbik Mod 10 check digit is calculated by multiplying each digit in this source string by 
            // either a 1 or a 2. Going from right to left, will first multiply by 2, then by 1, then by 2, 
            // then 1, etc. etc. All alpha chars will use their ASCII value - 48. So, A=17, B=18, C=19, etc.
            // So, in above example would have:
            // Source String: D000065626200807006000
            // Weight:        1212121212121212121212
            // Equals: (20*1) + (0*2) + (0*1) + (0*2) + (0*1) + (6*2) + (5*1) + (6*2) + (2*1) + (6*2) + (2*1) + 
            //         (0*2) + (0*1) + (8*2) + (0*1) + (7*2) + (0*1) + (0*2) + (6*1) + (0*2) + (0*1) + (0*2) =
            //         20 + 0 + 0 + 0 + 0 + 12 + 5 + 12 + 2 + 12 + 2 + 0 + 0 + 16 + 0 + 14 + 0 + 0 + 6 + 0 + 0 + 0 = 101
            // Once get all weights added in take the sum and mod by 10.
            // 101 MOD 10 = 1
            // Then subtract from 10 to get the actual check digit.
            // 10 - 1 = 9.
            // If the check digit = 10 then use check digit of 0.
            int sum = 0;
            int weight = 2;
            for (int charIdx = oResult.Length - 1; charIdx >= 0; charIdx--)
            {
                try
                {
                    // Make sure all lower case characters are converted to uppercase.
                    string nextCharString = Convert.ToString(oResult[charIdx]).ToUpper();
                    char nextChar = Convert.ToChar(nextCharString);
                    // Also make sure this is an alpha-numeric char. Since we will just
                    // be using the ASCII value of this char would include ANY chars
                    // if didn't do this check.
                    if (((nextChar >= 'A') && (nextChar <= 'Z')) ||
                        ((nextChar >= '0') && (nextChar <= '9')))
                    {
                        // Convert this character to a number. Will use the characters
                        // ASCII value - 48 to get the actual value of char. This is same
                        // for numbers (ASCII of 0 is 48, so subtracting 48 would give you 0)
                        // but need to do it this way because could have alpha characters.
                        int charNumber = Convert.ToInt32(nextChar) - 48;
                        // Multiply this character by the weight. 
                        int multipliedProduct = charNumber * weight;
                        // Add this to the running total.
                        sum += multipliedProduct;

                        // Alternate the weight between 1 and 2.
                        if (weight == 1) { weight = 2; }
                        else { weight = 1; }
                    }
                }
                catch
                {
                }
            }

            // Get the check digit by using mod 10 and then subtracting mod result from 10.
            int modNumber = sum % 10;
            int checkDigit = 10 - modNumber;
            // If its too big (equals 10), then set check digit to 0.
            if (checkDigit == 10) { checkDigit = 0; }
            // We will only return the check digit portion - not the full issue number + check digit.
            oResult = checkDigit.ToString();

            return 0;
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TTableNillumbikMod10VirtualFldDef()
            : base()
        {
        }
    }

    public class TTablePOSTbillpayFormatVirtualFldDef : TTableVirtualFldDef
    {
        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);

            // Copy field value to result, but formatted with a space every 4 characters. 
            // Exclude currency formatting characters ("$" and ".") from the output.
            StringBuilder sb = new StringBuilder();
            int segmentLength = 0;
            for (int charIdx = 0; charIdx < oResult.Length; charIdx++)
            {
                // Is it a source character that we won't ignore?
                if ((oResult[charIdx] != '$') && (oResult[charIdx] != '.') && (oResult[charIdx] != ' '))
                {
                    // If previous segment is full, append a space and reset segment length. 
                    // Otherwise, increment the segment length counter
                    if (segmentLength >= 4)
                    {
                        sb.Append(" ");
                        segmentLength = 1;  // the new segment starts with 1st char appended below
                    }
                    else
                    {
                        segmentLength++;
                    }

                    // Append next source character
                    sb.Append(oResult[charIdx]);
                }
            }

            // Return final contents of string builder
            oResult = sb.ToString();
            return 0;
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TTablePOSTbillpayFormatVirtualFldDef()
            : base()
        {
        }
    }

    public class TTableMorelandMod97VirtualFldDef : TTableVirtualFldDef
    {


        public static int[] MorelandPrimeNumbers = new int[] { 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89 };

        int MorelandPrimeNumberCnt = MorelandPrimeNumbers.Length;



        private int CalcMorelandMod97(string iDataMask, string iSource)
        {


            int loNdx;
            int loWeightNdx;
            int loSumSoFar;
            int loDigitCnt;


            // no check digits on a blank string
            if (iSource.Length == 0) return 0;



            // loop through the data, summing the product of each digit and its corresponding weight
            // this implementation works right to left such that varying issue no lengths can be handled
            for (loSumSoFar = 0, loNdx = iSource.Length - 1, loWeightNdx = MorelandPrimeNumberCnt - 1, loDigitCnt = 0; loNdx >= 0; loNdx--)
            {
                // ignore spaces
                if (iSource[loNdx] == ' ') continue;

                // ignore non-digits 
                if ((iSource[loNdx] > '9') || (iSource[loNdx] < '0'))
                {
                    continue; // ignore non-digits
                    /*
                    char loLine1[80];
                    uHALr_FormatStr( "%s contains non-numerics!", loLine1, 80, iSource );
                    ShowMessage( "Failed Moreland Mod97 Check Digit", loLine1 );
                    *iSource = 0;
                    return -1;
                    */
                }


                // can't exceed MorelandPrimeNumberCnt digits
                if (loDigitCnt == MorelandPrimeNumberCnt)
                { // whoops!
                    //string loLine1;
                    //uHALr_FormatStr( "Exceeds %d characters!", loLine1, 80, MorelandPrimeNumberCnt );
                    //ShowMessage( "Failed Moreland Mod97 ChkDig", loLine1 );
                    //*iSource = 0;
                    //return -1;
                    return -1;

                }


                // passed our checks, multiply be position's weight and add to running total and increment digit cnt
                loSumSoFar += (iSource[loNdx] - '0') * MorelandPrimeNumbers[loWeightNdx--];

                loDigitCnt++;
            }

            // check digit is sum of weigthed digits mod 97, complemented.
            loSumSoFar %= 97;

            return (97 - loSumSoFar);
        }


        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            int loCheckDigitVal;
            string loRawData;

            // inherited version will build the string that we will then place a check digit on.
            base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);



            /*
            //debug / test values 
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211426 060707 10000" ); // chkdig = 74
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211223 120707 10000" ); // chkdig = 16
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211245 080707 08000" ); // chkdig = 78
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211271 040707 06000" ); // chkdig = 77
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211293 070707 14000" ); // chkdig = 49

            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211315 110707 12000" ); // chkdig = 35
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211330 030707 10000" ); // chkdig = 67
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211352 070707 08000" ); // chkdig = 83
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211374 110707 06000" ); // chkdig = 21
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211396 100707 15000" ); // chkdig = 11

            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211426 060707 10000" ); // chkdig = 74
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211234 100707 09000" ); // chkdig = 28
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211256 060707 07000" ); // chkdig = 70
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211282 050707 15000" ); // chkdig = 51
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211304 090707 13000" ); // chkdig = 95

            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211326 010707 11000" ); // chkdig = 73
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211341 050707 09000" ); // chkdig = 85
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211363 090707 07000" ); // chkdig = 81
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211385 120707 05000" ); // chkdig = 66
            loCheckDigitVal = CalcMorelandMod97( iDataMask, "211400 080707 12000" ); // chkdig = 57
            */


            // calculate 
            loCheckDigitVal = CalcMorelandMod97(iDataMask, oResult);

            // convert it to a string and return the value.
            oResult = loCheckDigitVal.ToString();
            return 0;
        }


        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TTableMorelandMod97VirtualFldDef()
            : base()
        {
        }
    }

    public class TTableBanyuleMod97VirtualFldDef : TTableVirtualFldDef
    {
        public static int[] BanyulePrimeNumbers = new int[] { 1, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89 };

        int BanyulePrimeNumberCnt = BanyulePrimeNumbers.Length;

        private int CalcBanyuleMod97(string iDataMask, string iSource)
        {
            int loNdx;
            int loWeightNdx;
            int loSumSoFar;
            int loDigitCnt;

            // no check digits on a blank string
            if (iSource.Length == 0) return 0;

            // loop through the data, summing the product of each digit and its corresponding weight
            // this implementation works right to left such that varying issue no lengths can be handled
            for (loSumSoFar = 0, loNdx = iSource.Length - 1, loWeightNdx = BanyulePrimeNumberCnt - 1, loDigitCnt = 0; loNdx >= 0; loNdx--)
            {
                // ignore spaces
                if (iSource[loNdx] == ' ') continue;

                // ignore non-digits 
                if ((iSource[loNdx] > '9') || (iSource[loNdx] < '0'))
                {
                    continue; // ignore non-digits
                    /*
                    char loLine1[80];
                    uHALr_FormatStr( "%s contains non-numerics!", loLine1, 80, iSource );
                    ShowMessage( "Failed Banyule Mod97 Check Digit", loLine1 );
                    *iSource = 0;
                    return -1;
                    */
                }

                // can't exceed BanyulePrimeNumberCnt digits
                if (loDigitCnt == BanyulePrimeNumberCnt)
                { // whoops!
                    //string loLine1;
                    //uHALr_FormatStr( "Exceeds %d characters!", loLine1, 80, BanyulePrimeNumberCnt );
                    //ShowMessage( "Failed Banyule Mod97 ChkDig", loLine1 );
                    //*iSource = 0;
                    //return -1;
                    return -1;
                }

                // passed our checks, multiply be position's weight and add to running total and increment digit cnt
                loSumSoFar += (iSource[loNdx] - '0') * BanyulePrimeNumbers[loWeightNdx--];

                loDigitCnt++;
            }

            // check digit is sum of weigthed digits mod 97, complemented.
            loSumSoFar %= 97;
            return (97 - loSumSoFar);
        }

        public override int ConvertFromStoreFormat(TTTable iSrcTable, int iFieldNo, String iDataMask, ref String oResult)
        {
            int loCheckDigitVal;
            string loRawData;

            // inherited version will build the string that we will then place a check digit on.
            base.ConvertFromStoreFormat(iSrcTable, iFieldNo, iDataMask, ref oResult);

            /* * /
            //debug / test values 
            loCheckDigitVal = CalcBanyuleMod97( iDataMask, "01 240811 00000290726 12200" ); // chkdig = 35
             
            loCheckDigitVal = CalcBanyuleMod97( iDataMask, "01 101011 00000100002 12200" ); // chkdig = 72
            loCheckDigitVal = CalcBanyuleMod97( iDataMask, "01 101011 00000100003 12200" ); // chkdig = 05
            loCheckDigitVal = CalcBanyuleMod97( iDataMask, "01 101011 00000100004 12200" ); // chkdig = 35
              
             
            / * */

            // calculate 
            loCheckDigitVal = CalcBanyuleMod97(iDataMask, oResult);

            // convert it to a string and return the value.
            /*oResult = loCheckDigitVal.ToString();*/ // Banyule needs it to be 2-characters always
            oResult = Convert.ToString(loCheckDigitVal).PadLeft(2, '0');

            return 0;
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TTableBanyuleMod97VirtualFldDef()
            : base()
        {
        }
    }

}
