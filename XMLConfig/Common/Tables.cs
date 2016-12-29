// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 12/04/13 8:15a $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Common/ClientConfig/Tables.cs $
//              Revision: $Revision: 95 $

#define DEBUG

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;
using AutoISSUE;



/*
 Q. That's interesting! Are StringBuilders faster for larger data?

 A. Yes. When you use a normal string and change it, the .net framework will destroy the current string in memory and create a new one somewhere else in the memory. With a stringbuilder the object isn't destroyed each time it's adapted.

 A. As far as reasoning, MS docs suggest it is for use in loops.

 "The String object is immutable. Every time you use one of the methods in the System.String class, you create a new string object in memory, which requires a new allocation of space for that new object. In situations where you need to perform repeated modifications to a string, the overhead associated with creating a new String object can be costly. The System.Text.StringBuilder class can be used when you want to modify a string without creating a new object. For example, using the StringBuilder class can boost performance when concatenating many strings together in a loop."

 So, to answer the question, at what point is it more efficient, I wrote a little tester. The StringBuilder returned Sub-10ms times (DateTime not capable of registering < 10ms) at 1000 or fewer iterations of a loop simply appending the string "string". The += method of concatenation takes 15+ms for the same iterations. At 5000 iterations, the += method was becoming much slower at 218ms. versus the StringBuilder which was still sub 10ms.Even at 10000 iterations, the SB was sub 10, while the concat method had gone over 1.2 seconds. 
 *  	
Subject: Indexing Items in a Generics List

I am using a generics List<MyStructure> object of a structure of information
I need in my application.  Initially I created a predicate to retrieve the
correct struct of data, but I am finding that this approach is very CPU
intensive ~15% for just the look up.  I did a test where I created a list
that contained just the index values I needed so that I could use a
BinarySearch to give me an index value that I could use to pull the item out
of the list of structures.  This is very fast but because the List has to be
sorted to to use the BinarySearch the ordinal positions of my lists no longer
match.  It seems like I need to create an index list that contains the value
I want to lookup and the position of the data in the original list so that I
can pull it out.  It seems like I need to implement a Comparer to be able to
sort and search my index.  The on-line help does not provide any examples of
how to do this.  If I create a Comparer will I still get the efficiency of
the BinarySearch or will it need to look through the entire list to find my
value like the Predicate does?

Does this seem like the correct approach?  It seems like wanting to have an
index on a list would be something that a number of applications would need.  
Any ideas would be appreciated.

Subject: RE: Indexing Items in a Generics List

I found the SortedDictionary object that is doing the job for me.  I use my
first index value as the key and my structure as the value.  I then create
additional SortedDictionaries for each of the other indexes with the value
containing the key in the first SortedDictionary.  Isn't it always the case,
once you post a message for help you find the answer
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * DEVELOPER NOTE:
 * 
 * We are currently using the compiler directive "#if !WindowsCE" to include code
 * specifically for the host side. This may need to be refined when the PatrolCar 
 * version of the issuance software is developed.
 * 
 * 
 */

namespace Reino.ClientConfig
{

    static public class ReinoTablesConst
    {
        // universal pathname, relative to exe folder
#if WindowsCE
        /*public const string cnstClientConfigFolder = @"\AC Flash\"; //@"\";*/
        public static volatile string cnstClientConfigFolder = @"\AC Flash\";
#elif __ANDROID__
        /*public const string cnstClientConfigFolder = @"Storage Card1\App_Data\";*/
        public static volatile string cnstClientConfigFolder = @"Storage Card1\App_Data\";
#else
        /*public const string cnstClientConfigFolder = @"Storage Card1\App_Data\";*/
        public static volatile string cnstClientConfigFolder = @"Storage Card1\App_Data\";
#endif
        // our proprietary field format/conversion routines handle either upper or lower case because
        // we don't mix the date/time fields together. But the .NET formatting routines see M and m
        // as different masks, that is M = MONTH and m = minute because it allows date/time fields to
        // be mixed. As such, the mask for DATEs must be MIXED case to be used in either routine
        public const string DATE_TYPE_DATAMASK = "yyyyMMdd";
        public const int DATE_TYPE_FIELDLENGTH = 8;

        public const int INT_TYPE_FIELDLENGTH = 12;

        public const int REAL_TYPE_FIELDLENGTH = 12;

        /// <summary>
        /// A minimum default size for BLOB types. Actual size is automatically set when
        /// SetFormattedData is called with a byte array parameter
        /// </summary>
        public const int BLOB_TYPE_FIELDLENGTH = 16;

        // our proprietary field format/conversion routines handle either upper or lower case because
        // we don't mix the date/time fields together. But the .NET formatting routines see M and m
        // as different masks, that is M = MONTH and m = minute because it allows date/time fields to
        // be mixed. For the times, we must used uppercase HH so we'll get 24 hour values as the storage
        // type. As such, the mask for TIMESs must be MIXED case to be used in either routine
        public const string TIME_TYPE_DATAMASK = "HHmmss";
        public const int TIME_TYPE_FIELDLENGTH = 6;

        public const int VIRTUAL_COLUMN_FIELDLENGTH = 180;

        // Will know a table needs to be written to RAM if it has this defined in its name.
        public const string WRITE_TABLE_TO_RAM_IDENT = "(TO_RAM)";
    }

    #region Host Side Only Required Field Object Class
#if !WindowsCE && !__ANDROID__  


    /// <summary>
    /// Predicate class for searching field lists by field name
    /// </summary>
    public class THostRequiredFieldPredicate : object
    {
        private string _CompareName;

        public THostRequiredFieldPredicate(string iCompareName)
        {
            _CompareName = iCompareName;
        }

        public bool FindFieldByName(THostRequiredFieldInfoObj pObject)
        {
            return _CompareName.Equals(((THostRequiredFieldInfoObj)pObject).FieldName);
        }
    }


    /// <summary>
    /// Database column types - for holding the types during table requirement checks
    /// </summary>
    public enum TDataBaseColumnDataType
    {
        dbtString = 0,
        dbtInteger,
        dbtReal,
        dbtDateTime,
        dbtBLOB
    }


    /// <summary>
    /// A class for holding information about a column whose existence needs to be verified in a table def
    /// </summary>
    public class THostRequiredFieldInfoObj
    {
        public string FieldName;
        public double FieldSize;
        public TDataBaseColumnDataType FieldType;
        public bool NotNull;
        public string DefaultVal;
        public bool Indexed;
        public bool NoInsertTrigger;
    }

#endif
    #endregion


    #region Android Client Side Only Required Field Object Class
#if __ANDROID__  

    /// <summary>
    /// Predicate class for searching field lists by field name
    /// </summary>
    public class THostRequiredFieldPredicate : object
    {
        private string _CompareName;

        public THostRequiredFieldPredicate(string iCompareName)
        {
            _CompareName = iCompareName;
        }

        public bool FindFieldByName(TAndroidClientRequiredFieldInfoObj pObject)
        {
            return _CompareName.Equals(((TAndroidClientRequiredFieldInfoObj)pObject).FieldName);
        }
    }


    /// <summary>
    /// Database column types - for holding the types during table requirement checks
    /// </summary>
    public enum TDataBaseColumnDataType
    {
        dbtString = 0,
        dbtInteger,
        dbtReal,
        dbtDateTime,
        dbtBLOB
    }


    /// <summary>
    /// A class for holding information about a column whose existence needs to be verified in a table def
    /// </summary>
    public class TAndroidClientRequiredFieldInfoObj
    {
        public string FieldName;
        public double FieldSize;
        public TDataBaseColumnDataType FieldType;
        public bool NotNull;
        public string DefaultVal;
        public bool Indexed;
        public bool NoInsertTrigger;
    }

#endif
    #endregion



    /// <summary>
    /// TTableListMgr 
    /// Base clase for descendents that manage a list of TableDefs. 
    /// TAgList, TIssStruct, and TRegistryMgr all have a list of tables.
    /// </summary>
    [XmlInclude(typeof(TIssStruct)), XmlInclude(typeof(TAgList)), XmlInclude(typeof(TRegistryMgr))]
    public class TTableListMgr : TObjBase
    {
        // This is the globally accessible instance of the TTableListMgr
        public static TTableListMgr glTableMgr = new TTableListMgr();

        #region Properties and Members

        protected ListObjBase<TTableDef> _TableDefs;
        public ListObjBase<TTableDef> TableDefs
        {
            get { return _TableDefs; }
            set { _TableDefs = value; }
        }


        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public TTableDef MainTable
        {
            get
            {
                if (_TableDefs.Count > 0)
                    return _TableDefs[0];
                // Return null if couldn't find a table.
                return null;
            }
        }
        #endregion



        #region Additional Structure PostDeserialize Code for Android Client Side Only
#if __ANDROID__   

        /// <summary>
        /// A collection of fields that are required to exist on the host side
        /// </summary>
        public List<TAndroidClientRequiredFieldInfoObj> fAndroidClientOnlyStandardFields = new List<TAndroidClientRequiredFieldInfoObj>();

        //protected List<TAndroidClientRequiredFieldInfoObj> fHostOnlyStandardFields = new List<TAndroidClientRequiredFieldInfoObj>();

        /// <summary>
        /// Returns the Status table that belongs to the main table.
        /// </summary>
        [XmlIgnore]
        public TTableDef StatusTable
        {
            get
            {
                foreach (TTableDef loTableDef in _TableDefs)
                {
                    if (loTableDef.Name.EndsWith(DBConstants.cnStatusTableNameSuffix))
                        return loTableDef;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the transaction link table that belongs to the main table.
        /// </summary>
        [XmlIgnore]
        public TTableDef TransLinkTable
        {
            get
            {
                foreach (TTableDef loTableDef in _TableDefs)
                {
                    if (loTableDef.Name.EndsWith(DBConstants.cnTransactionLinkTableNameSuffix))
                        return loTableDef;
                }
                return null;
            }
        }
        /// <summary>
        /// A collection of violation sub-table fields that are required to exist on the host
        /// </summary>
        //protected List<THostRequiredFieldInfoObj> fHostOnlyViolationStandardFields = new List<THostRequiredFieldInfoObj>();


        /// <summary>
        /// Helper function to return the internal database column enum for the passed fielddef
        /// </summary>
        /// <param name="iFieldDef"></param>
        /// <returns></returns>
        protected TDataBaseColumnDataType GetDataBaseColumnTypeForFieldDefType(Type iFieldDefType)
        {
            if (iFieldDefType == typeof(TTableIntFldDef))
            {
                return TDataBaseColumnDataType.dbtInteger;
            }
            else if (iFieldDefType == typeof(TTableRealFldDef))
            {
                return TDataBaseColumnDataType.dbtReal;
            }
            else if (iFieldDefType == typeof(TTableDateFldDef))
            {
                return TDataBaseColumnDataType.dbtDateTime;
            }
            //else if (iFieldDefType == typeof(TTableBLOBFldDef))
            //{
            //    return TDataBaseColumnDataType.dbtBLOB;
            //}
            else
            {
                // all other types are treated as strings
                return TDataBaseColumnDataType.dbtString;
            }

        }

        /// <summary>
        /// Adds field to passed list
        /// </summary>
        /// <param name="?"></param>
        protected void AddAndroidClientSideRequiredFieldInfo(List<TAndroidClientRequiredFieldInfoObj> iFieldList,
                                                   string pName, double pSize,
                                                   TDataBaseColumnDataType pFieldType,
                                                   bool pNotNull, string pDefaultVal)
        {
            // If the field already exists in the list, then exit
            THostRequiredFieldPredicate loPredicate = new THostRequiredFieldPredicate(pName);
            if (iFieldList.Find(loPredicate.FindFieldByName) != null) { return; }

            // make a new field 
            TAndroidClientRequiredFieldInfoObj loNewField = new TAndroidClientRequiredFieldInfoObj();
            loNewField.FieldName = pName;
            loNewField.FieldSize = pSize;
            loNewField.FieldType = pFieldType;
            loNewField.NotNull = pNotNull;
            loNewField.DefaultVal = pDefaultVal;
            loNewField.Indexed = false; // pIndexed;
            loNewField.NoInsertTrigger = false; // pNoInsertTrigger;

            //Force the not null condition if its a UniqueKey field
            if (pName.ToUpper().Equals(DBConstants.sqlUniqueKeyStr))
            {
                loNewField.NotNull = true;
            }

            // add it to the specified list
            iFieldList.Add(loNewField);
        }

        ///// <summary>
        ///// Adds field to passed list
        ///// </summary>
        ///// <param name="?"></param>
        //protected void AddHostSideRequiredFieldInfo(List<TAndroidClientRequiredFieldInfoObj> iFieldList,
        //                                           string pName, double pSize,
        //                                           TDataBaseColumnDataType pFieldType,
        //                                           bool pNotNull, string pDefaultVal)
        //{
        //    // If the field already exists in the list, then exit
        //    THostRequiredFieldPredicate loPredicate = new THostRequiredFieldPredicate(pName);
        //    if (iFieldList.Find(loPredicate.FindFieldByName) != null) { return; }

        //    // make a new field 
        //    TAndroidClientRequiredFieldInfoObj loNewField = new TAndroidClientRequiredFieldInfoObj();
        //    loNewField.FieldName = pName;
        //    loNewField.FieldSize = pSize;
        //    loNewField.FieldType = pFieldType;
        //    loNewField.NotNull = pNotNull;
        //    loNewField.DefaultVal = pDefaultVal;
        //    loNewField.Indexed = false; // pIndexed;
        //    loNewField.NoInsertTrigger = false; // pNoInsertTrigger;

        //    //Force the not null condition if its a UniqueKey field
        //    if (pName.ToUpper().Equals(DBConstants.sqlUniqueKeyStr))
        //    {
        //        loNewField.NotNull = true;


        //        // DEBUG - since GetSchema adds the uniquekey, we wont put it in the def right now
        //        //return;
        //    }

        //    // add it to the specified list
        //    iFieldList.Add(loNewField);
        //}


        /// <summary>
        /// Does some self-examination to make sure that the minimum column requirements are met
        /// </summary>
        /// <param name="pTableName"></param>
        public void VerifyFieldsInTableDef(List<TAndroidClientRequiredFieldInfoObj> iVerifyFieldList, TTableDef iTableDef)
        {

            // work through the verify fields list and make sure the required fields are defined
            foreach (TAndroidClientRequiredFieldInfoObj loFieldInfo in iVerifyFieldList)
            {
                // watch out for tables that have no defintion to search
                if ((iTableDef.HighTableRevision != null) && (iTableDef.GetFldNo(loFieldInfo.FieldName) != -1))
                    continue; // field already exists. let's move on.

                TTableFldDef loTableFldDef;
                // not present - add it by type
                switch (loFieldInfo.FieldType)
                {
                    case (TDataBaseColumnDataType.dbtInteger):
                        loTableFldDef = new TTableIntFldDef();
                        break;
                    case (TDataBaseColumnDataType.dbtReal):
                        loTableFldDef = new TTableRealFldDef();
                        break;
                    case (TDataBaseColumnDataType.dbtDateTime):
                        if (loFieldInfo.FieldName.Contains("TIME"))
                            loTableFldDef = new TTableTimeFldDef();
                        else
                            loTableFldDef = new TTableDateFldDef();
                        break;
                    //case (TDataBaseColumnDataType.dbtBLOB):
                    //    loTableFldDef = new TTableBLOBFldDef();
                    //    break;
                    default:
                        loTableFldDef = new TTableStringFldDef();
                        break;
                }

                // regardless of the final class, these properties are defined in the base class.

                loTableFldDef.Name = loFieldInfo.FieldName;
                loTableFldDef.DefaultValue = loFieldInfo.DefaultVal;
                loTableFldDef.Size = (int)loFieldInfo.FieldSize;
                loTableFldDef.IsHostSideDefinitionOnly = true;
                iTableDef.AddFieldToAllRevisions(loTableFldDef);
            }
        }

        /// <summary>
        /// Return specialty table definition, locating existing definition or creating one as needed
        /// </summary>
        protected TTableDef GetOrCreateSpecialtyTableDef(string iSpecialtyTableNameSuffix)
        {
            // build the specialy table's name
            string loSpecialtyTableName = this.Name + iSpecialtyTableNameSuffix;

            // build a predicate to search for the tablename
            TObjBasePredicate loFindPredicate = new TObjBasePredicate(loSpecialtyTableName);
            // try to find this table
            TTableDef loSpecialtyTableDef = null;
            loSpecialtyTableDef = this.TableDefs.Find(loFindPredicate.CompareByName_CaseInsensitive);
            // not already there?
            if (loSpecialtyTableDef == null)
            {
                // we'll need to create it
                loSpecialtyTableDef = new TTableDef();
                loSpecialtyTableDef.Name = loSpecialtyTableName;
                // and add it to list of tables in this struct
                this.TableDefs.Add(loSpecialtyTableDef);
                // this isn't read from the CFG file, but we have to make sure the call is made to get things initialized
                loSpecialtyTableDef.PostDeserialize(this);
            }

            // return it
            return loSpecialtyTableDef;
        }


        /// <summary>
        /// Host side only.
        /// Called after FinalizeHostSideClientConfig.
        /// Opportunity to create any indexes required by the host side.
        /// </summary>
        protected virtual void CreateHostSideIndexes()
        {
            foreach (TTableDef loTableDef in TableDefs)
            {
                // the TTableIndex constructor adds itself to loTableDef.Indexes IFF at least one of the fields in iColumns exist.
                List<string> loIndexFields = new List<string>();
                // start w/ primary key on UNIQUEKEY &  MASTERKEY.
                loIndexFields.Add(DBConstants.sqlMasterKeyStr);
                loIndexFields.Add(DBConstants.sqlUniqueKeyStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlUniqueKeyStr, loIndexFields.ToArray(), true, true);
                loIndexFields.Clear();

                // add a secondary key for issueno
                loIndexFields.Add(DBConstants.sqlIssueNumberStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlIssueNumberStr, loIndexFields.ToArray(), false, false);
                loIndexFields.Clear();

                // one for issue date
                loIndexFields.Add(DBConstants.sqlIssueDateStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlIssueDateStr, loIndexFields.ToArray(), false, false);
                loIndexFields.Clear();

                // one for VehLicNo
                loIndexFields.Add(DBConstants.sqlVehLicNoStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlVehLicNoStr, loIndexFields.ToArray(), false, false);
                loIndexFields.Clear();

                // one for VehVIN
                loIndexFields.Add(DBConstants.sqlVehVINStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlVehVINStr, loIndexFields.ToArray(), false, false);
                loIndexFields.Clear();

                // one for reccreationdate
                loIndexFields.Add(DBConstants.sqlRecCreationDateStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlRecCreationDateStr, loIndexFields.ToArray(), false, false);
                loIndexFields.Clear();

                // one for ExportUniqueID
                loIndexFields.Add(DBConstants.sqlExportUniqueIDStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlExportUniqueIDStr, loIndexFields.ToArray(), false, false);
                loIndexFields.Clear();

                // one for TransactionKey
                loIndexFields.Add(DBConstants.sqlTransactionKeyStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlTransactionKeyStr, loIndexFields.ToArray(), false, false);
                loIndexFields.Clear();
            }

            // add the uniqueconstraint index
            if (MainTable != null)
            {
                new TTableIndex(MainTable, MainTable.Name + "_UNIQUECONSTRAINTS", MainTable.HighTableRevision.UniqueContraintFields.ToArray(), false, true);
            }
        }

#endif
        #endregion



        #region Additional Structure PostDeserialize Code for Host Side Only
#if !WindowsCE && !__ANDROID__   


        /// <summary>
        /// A collection of fields that are required to exist on the host side
        /// </summary>
        protected List<THostRequiredFieldInfoObj> fHostOnlyStandardFields = new List<THostRequiredFieldInfoObj>();

        /// <summary>
        /// Returns the Status table that belongs to the main table.
        /// </summary>
        [XmlIgnore]
        public TTableDef StatusTable
        {
            get
            {
                foreach (TTableDef loTableDef in _TableDefs)
                {
                    if (loTableDef.Name.EndsWith(DBConstants.cnStatusTableNameSuffix))
                        return loTableDef;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the transaction link table that belongs to the main table.
        /// </summary>
        [XmlIgnore]
        public TTableDef TransLinkTable
        {
            get
            {
                foreach (TTableDef loTableDef in _TableDefs)
                {
                    if (loTableDef.Name.EndsWith(DBConstants.cnTransactionLinkTableNameSuffix))
                        return loTableDef;
                }
                return null;
            }
        }
        /// <summary>
        /// A collection of violation sub-table fields that are required to exist on the host
        /// </summary>
        //protected List<THostRequiredFieldInfoObj> fHostOnlyViolationStandardFields = new List<THostRequiredFieldInfoObj>();


        /// <summary>
        /// Helper function to return the internal database column enum for the passed fielddef
        /// </summary>
        /// <param name="iFieldDef"></param>
        /// <returns></returns>
        protected TDataBaseColumnDataType GetDataBaseColumnTypeForFieldDefType(Type iFieldDefType)
        {
            if (iFieldDefType == typeof(TTableIntFldDef))
            {
                return TDataBaseColumnDataType.dbtInteger;
            }
            else if (iFieldDefType == typeof(TTableRealFldDef))
            {
                return TDataBaseColumnDataType.dbtReal;
            }
            else if (iFieldDefType == typeof(TTableDateFldDef))
            {
                return TDataBaseColumnDataType.dbtDateTime;
            }
            else if (iFieldDefType == typeof(TTableBLOBFldDef))
            {
                return TDataBaseColumnDataType.dbtBLOB;
            }
            else
            {
                // all other types are treated as strings
                return TDataBaseColumnDataType.dbtString;
            }

        }

        /// <summary>
        /// Adds field to passed list
        /// </summary>
        /// <param name="?"></param>
        protected void AddHostSideRequiredFieldInfo(List<THostRequiredFieldInfoObj> iFieldList,
                                                   string pName, double pSize,
                                                   TDataBaseColumnDataType pFieldType,
                                                   bool pNotNull, string pDefaultVal)
        {
            // If the field already exists in the list, then exit
            THostRequiredFieldPredicate loPredicate = new THostRequiredFieldPredicate(pName);
            if (iFieldList.Find(loPredicate.FindFieldByName) != null) { return; }

            // make a new field 
            THostRequiredFieldInfoObj loNewField = new THostRequiredFieldInfoObj();
            loNewField.FieldName = pName;
            loNewField.FieldSize = pSize;
            loNewField.FieldType = pFieldType;
            loNewField.NotNull = pNotNull;
            loNewField.DefaultVal = pDefaultVal;
            loNewField.Indexed = false; // pIndexed;
            loNewField.NoInsertTrigger = false; // pNoInsertTrigger;

            //Force the not null condition if its a UniqueKey field
            if (pName.ToUpper().Equals(DBConstants.sqlUniqueKeyStr))
            {
                loNewField.NotNull = true;


                // DEBUG - since GetSchema adds the uniquekey, we wont put it in the def right now
                //return;
            }

            // add it to the specified list
            iFieldList.Add(loNewField);
        }


        /// <summary>
        /// Does some self-examination to make sure that the minimum column requirements are met
        /// </summary>
        /// <param name="pTableName"></param>
        protected void VerifyFieldsInTableDef(List<THostRequiredFieldInfoObj> iVerifyFieldList, TTableDef iTableDef)
        {

            // work through the verify fields list and make sure the required fields are defined
            foreach (THostRequiredFieldInfoObj loFieldInfo in iVerifyFieldList)
            {
                // watch out for tables that have no defintion to search
                if ((iTableDef.HighTableRevision != null) && (iTableDef.GetFldNo(loFieldInfo.FieldName) != -1))
                    continue; // field already exists. let's move on.

                TTableFldDef loTableFldDef;
                // not present - add it by type
                switch (loFieldInfo.FieldType)
                {
                    case (TDataBaseColumnDataType.dbtInteger):
                        loTableFldDef = new TTableIntFldDef();
                        break;
                    case (TDataBaseColumnDataType.dbtReal):
                        loTableFldDef = new TTableRealFldDef();
                        break;
                    case (TDataBaseColumnDataType.dbtDateTime):
                        if (loFieldInfo.FieldName.Contains("TIME"))
                            loTableFldDef = new TTableTimeFldDef();
                        else
                            loTableFldDef = new TTableDateFldDef();
                        break;
                    case (TDataBaseColumnDataType.dbtBLOB):
                        loTableFldDef = new TTableBLOBFldDef();
                        break;
                    default:
                        loTableFldDef = new TTableStringFldDef();
                        break;
                }

                // regardless of the final class, these properties are defined in the base class.

                loTableFldDef.Name = loFieldInfo.FieldName;
                loTableFldDef.DefaultValue = loFieldInfo.DefaultVal;
                loTableFldDef.Size = (int)loFieldInfo.FieldSize;
                loTableFldDef.IsHostSideDefinitionOnly = true;
                iTableDef.AddFieldToAllRevisions(loTableFldDef);
            }
        }

        /// <summary>
        /// Return specialty table definition, locating existing definition or creating one as needed
        /// </summary>
        protected TTableDef GetOrCreateSpecialtyTableDef(string iSpecialtyTableNameSuffix)
        {
            // build the specialy table's name
            string loSpecialtyTableName = this.Name + iSpecialtyTableNameSuffix;

            // build a predicate to search for the tablename
            TObjBasePredicate loFindPredicate = new TObjBasePredicate(loSpecialtyTableName);
            // try to find this table
            TTableDef loSpecialtyTableDef = null;
            loSpecialtyTableDef = this.TableDefs.Find(loFindPredicate.CompareByName_CaseInsensitive);
            // not already there?
            if (loSpecialtyTableDef == null)
            {
                // we'll need to create it
                loSpecialtyTableDef = new TTableDef();
                loSpecialtyTableDef.Name = loSpecialtyTableName;
                // and add it to list of tables in this struct
                this.TableDefs.Add(loSpecialtyTableDef);
                // this isn't read from the CFG file, but we have to make sure the call is made to get things initialized
                loSpecialtyTableDef.PostDeserialize(this);
            }

            // return it
            return loSpecialtyTableDef;
        }



        /// <summary>
        /// The TransactionLinkTable replaces the StatusTable.  It serves as a linke table to allow
        /// a many to many relationship between a transaction table row and an issue table row.
        /// See Transaction.Doc for an explanation.
        /// </summary>
        protected virtual void VerifyTransactionLinkTable()
        {
            // retrieve the table definition
            TTableDef loStatusTableDef = GetOrCreateSpecialtyTableDef(DBConstants.cnTransactionLinkTableNameSuffix);

            // Create list of standard fields we need for this table 
            List<THostRequiredFieldInfoObj> loFieldList = new List<THostRequiredFieldInfoObj>();
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, ""); // unique key for this row
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlMasterKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, ""); // unique key into parent table

            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlTransactionKeyStr, -1, TDataBaseColumnDataType.dbtInteger, false, ""); // unique key into transaction table
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlTransactionNameStr, 50, TDataBaseColumnDataType.dbtString, false, ""); // Name of the transaction table

            // Each transaction link table can be exported. The export will tie in the parent 
            // and status history table. Add all fields needed to export a record.
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlExportUniqueIDStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            // User Defined Export Fields will be added dynamically (not sure how many of em we'll have).
            string[] userExportDateFields = DBConstants.GetAllUserDefinedExportDateFieldNames();
            foreach (string oneName in userExportDateFields)
            {
                AddHostSideRequiredFieldInfo(loFieldList, oneName, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            }
            // Custom Export Fields will be added dynamically (not sure how many of em we'll have).
            // (Custom Export Fields are used by multi-datatype exports managed via plug-in DLLs)
            string[] customExportDateFields = DBConstants.GetAllCustomExportDateFieldNames();
            foreach (string oneName in customExportDateFields)
            {
                AddHostSideRequiredFieldInfo(loFieldList, oneName, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            }

            // may or may not use this in the future.
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecordStatusValueStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecordStatusDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecordStatusTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecordStatusOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecordStatusOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecordStatusReasonStr, 80, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecCreationDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");

            // make sure the table has the fields we need
            VerifyFieldsInTableDef(loFieldList, loStatusTableDef);

            //{ Verify that we have the desired indexes }
            //{ We gotta make smaller index names cuz oracle has a small limit }
            //VerifyIndex( loStatusTableName, 'NDX' + pTableName + 'ST' + 'MK', 'MASTERKEY' );
            //VerifyIndex( loStatusTableName, 'NDX' + pTableName + 'ST' + 'SD', 'STATUSDATE' );
            //VerifyIndex( loStatusTableName, 'NDX' + pTableName + 'ST' + 'SN', 'STATUSNAME' );

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes for UniqueKey, MaskerKey, etc...
            loStatusTableDef.PostDeserialize(this);
        }
        /// <summary>
        /// Specialty validation routine for Status tables
        /// </summary>
        protected virtual void VerifyStatusTable()
        {
            // retrieve the table definition
            TTableDef loStatusTableDef = GetOrCreateSpecialtyTableDef(DBConstants.cnStatusTableNameSuffix);

            // Create list of standard fields we need for this table 
            List<THostRequiredFieldInfoObj> loFieldList = new List<THostRequiredFieldInfoObj>();
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlUniqueKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, true, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecordStatusNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecordStatusValueStr, 5, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecordStatusDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecordStatusTimeStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecordStatusOfficerNameStr, 30, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecordStatusOfficerIDStr, 20, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecordStatusReasonStr, 80, TDataBaseColumnDataType.dbtString, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlMasterKeyStr, AutoISSUE.DBConstants.cnPrimaryKeyUniformSize, TDataBaseColumnDataType.dbtInteger, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlStandardExportDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlRecCreationDateStr, -1, TDataBaseColumnDataType.dbtDateTime, false, "NOW");
            AddHostSideRequiredFieldInfo(loFieldList, DBConstants.sqlStatusEventIDStr, -1, TDataBaseColumnDataType.dbtInteger, false, "");
            // User Defined Export Fields will be added dynamically (not sure how many of em we'll have).
            string[] userExportDateFields = DBConstants.GetAllUserDefinedExportDateFieldNames();
            foreach (string oneName in userExportDateFields)
            {
                AddHostSideRequiredFieldInfo(loFieldList, oneName, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            }
            // Custom Export Fields will be added dynamically (not sure how many of em we'll have).
            // (Custom Export Fields are used by multi-datatype exports managed via plug-in DLLs)
            string[] customExportDateFields = DBConstants.GetAllCustomExportDateFieldNames();
            foreach (string oneName in customExportDateFields)
            {
                AddHostSideRequiredFieldInfo(loFieldList, oneName, -1, TDataBaseColumnDataType.dbtDateTime, false, "");
            }

            // make sure the table has the fields we need
            VerifyFieldsInTableDef(loFieldList, loStatusTableDef);

            // We must do the PostDeserialize ourself or it will get skipped and cause errors about
            // mismatched datatypes for UniqueKey, MaskerKey, etc...
            loStatusTableDef.PostDeserialize(this);

            // mcb 2/16/06: anyone who had a Status table gets a transaction link table.
            VerifyTransactionLinkTable();
            //{ Verify that we have the desired indexes }
            //{ We gotta make smaller index names cuz oracle has a small limit }
            //VerifyIndex( loStatusTableName, 'NDX' + pTableName + 'ST' + 'MK', 'MASTERKEY' );
            //VerifyIndex( loStatusTableName, 'NDX' + pTableName + 'ST' + 'SD', 'STATUSDATE' );
            //VerifyIndex( loStatusTableName, 'NDX' + pTableName + 'ST' + 'SN', 'STATUSNAME' );
        }


        /// <summary>
        /// Host side only.
        /// Called after FinalizeHostSideClientConfig.
        /// Opportunity to create any indexes required by the host side.
        /// </summary>
        protected virtual void CreateHostSideIndexes()
        {
            foreach (TTableDef loTableDef in TableDefs)
            {
                // the TTableIndex constructor adds itself to loTableDef.Indexes IFF at least one of the fields in iColumns exist.
                List<string> loIndexFields = new List<string>();
                // start w/ primary key on UNIQUEKEY &  MASTERKEY.
                loIndexFields.Add(DBConstants.sqlMasterKeyStr);
                loIndexFields.Add(DBConstants.sqlUniqueKeyStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlUniqueKeyStr, loIndexFields.ToArray(), true, true);
                loIndexFields.Clear();

                // add a secondary key for issueno
                loIndexFields.Add(DBConstants.sqlIssueNumberStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlIssueNumberStr, loIndexFields.ToArray(), false, false);
                loIndexFields.Clear();

                // one for issue date
                loIndexFields.Add(DBConstants.sqlIssueDateStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlIssueDateStr, loIndexFields.ToArray(), false, false);
                loIndexFields.Clear();

                // one for VehLicNo
                loIndexFields.Add(DBConstants.sqlVehLicNoStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlVehLicNoStr, loIndexFields.ToArray(), false, false);
                loIndexFields.Clear();

                // one for VehVIN
                loIndexFields.Add(DBConstants.sqlVehVINStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlVehVINStr, loIndexFields.ToArray(), false, false);
                loIndexFields.Clear();

                // one for reccreationdate
                loIndexFields.Add(DBConstants.sqlRecCreationDateStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlRecCreationDateStr, loIndexFields.ToArray(), false, false);
                loIndexFields.Clear();

                // one for ExportUniqueID
                loIndexFields.Add(DBConstants.sqlExportUniqueIDStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlExportUniqueIDStr, loIndexFields.ToArray(), false, false);
                loIndexFields.Clear();

                // one for TransactionKey
                loIndexFields.Add(DBConstants.sqlTransactionKeyStr);
                new TTableIndex(loTableDef, loTableDef.Name + "_" + DBConstants.sqlTransactionKeyStr, loIndexFields.ToArray(), false, false);
                loIndexFields.Clear();
            }

            // add the uniqueconstraint index
            if (MainTable != null)
            {
                new TTableIndex(MainTable, MainTable.Name + "_UNIQUECONSTRAINTS", MainTable.HighTableRevision.UniqueContraintFields.ToArray(), false, true);
            }
        }


        /// <summary>
        /// Specialty code for host side only. Adds required additional tables and columns
        /// Adapted from AutoISSUE Host pascal code
        /// </summary>
        protected virtual void FinalizeHostSideClientConfig()
        {
            // mcb 1/10/05: Avoid references to descendant classes when oop alternative is available.
            // Code formerly checked to see if this is a TIssStruct instance. Why not just move
            // the code to TIssStruct? That's what was done.
        }
#endif


#if __ANDROID__
        /// <summary>
        /// Specialty code for host side only. Adds required additional tables and columns
        /// Adapted from AutoISSUE Host pascal code
        /// </summary>
        protected virtual void FinalizeHostSideClientConfig()
        {
            // override to make something happen 
        }
#endif



        #endregion

        public TTableListMgr()
            : base()
        {
            this._TableDefs = new ListObjBase<TTableDef>();
        }

        /// <summary>
        /// PostDeserialize
        /// Called after object has been created via deserialization.  Gives chance to resolve any
        /// internal references. As a container object, must call each contained object's PostDeserialization.
        /// </summary>
        /// <param name="iParent"></param>
        /// <returns></returns>
        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            _TableDefs.PostDeserializeListItems(this);

#if !WindowsCE && !__ANDROID__   
            // We only need to execute this code the very first 
            // time we're deserialized from to the initial load
            if (TClientDef.SkipHostSideOnlyCode == false)
            {
                FinalizeHostSideClientConfig();
                CreateHostSideIndexes();
            }
#endif

#if __ANDROID__
            FinalizeHostSideClientConfig();
            CreateHostSideIndexes();
#endif


            return 0;
        }

        /// <summary>
        /// As a container object, must call each contained object's ResolveRegistryItems
        /// </summary>
        public override void ResolveRegistryItems(ITTRegistry iRegistry)
        {
            base.ResolveRegistryItems(iRegistry);
            _TableDefs.ResolveRegistryItemsForListItems(iRegistry);
        }

        /// <summary>
        /// FindTableCollectionElementNoByName
        /// 
        /// Renamed from "Find.Index.By.Name" to clarify code, 
        /// differentiating the collection index from our "table.index" objects
        /// </summary>
        /// <param name="iFindName"></param>
        /// <returns></returns>
        public int FindTableCollectionElementNoByName(string iFindName)
        {
            TObjBasePredicate loPredicate = new TObjBasePredicate(iFindName);
            return _TableDefs.FindIndex(loPredicate.CompareByName);
        }

        /// <summary>
        /// TTableDef FindByName(string iFindName)
        /// 
        /// returns tabledef matching iFindName
        /// modern substutite for GetTblDef( iName )
        /// 
        /// </summary>
        /// <param name="iFindName"></param>
        /// <returns></returns>
        public TTableDef FindByName(string iFindName)
        {
            int loIndex = FindTableCollectionElementNoByName(iFindName);
            if (loIndex < 0) return null;
            return _TableDefs[loIndex];
        }

        public void AddTableDef(TTableDef iTableDef)
        {
            _TableDefs.Add(iTableDef);
        }

        public void RemoveTableDef(TTableDef iTableDef)
        {
            int loNdx;

            // DEBUG -- Can't do index of object? We'll have to look by name...
            if ((loNdx = _TableDefs.IndexOf(iTableDef)) >= 0)
            {
                _TableDefs.RemoveAt(loNdx);
            }
            /*
            TObjBasePredicate loPredicate = new TObjBasePredicate(iTableDef.Name);
            loNdx = _TableDefs.FindIndex(loPredicate.CompareByName);
            if (loNdx >= 0)
            {
                _TableDefs.RemoveAt(loNdx);
            }
            */
            else
            {
                Debug.Write("Unable to remove table def %p\n", iTableDef.Name);
            }
        }

        public TTableDef GetTableDef(String iTblName)
        {
            return FindByName(iTblName);
        }

        public int GetTableDefCnt()
        {
            return _TableDefs.Count;
        }

        public void PrintStats()
        {
            short loNdx = 0;
            Console.WriteLine("%s->TableDef Count:%d\n", this.Name, this.GetTableDefCnt());
            foreach (TTableDef loTableDef in _TableDefs)
            {
                Console.WriteLine("%d: %s %d", loNdx, loTableDef.Name, loTableDef.GetRecCount());
                loNdx++;
            }
            Console.ReadLine();
        }
    }

    /// <summary>
    /// TTableIndex
    /// Simple structure for holding info about a table index
    /// Declared as a class (instead of a struct) to enable efficient use in collections
    /// </summary>
    public class TIndexRecord
    {
        public int RecordNo;
        public String IndexData;

        public TIndexRecord()
            : base()
        {
        }
    }

    // a find predicate class for TTableIndex
    public class TTableIndexFindPredicate
    {
        private string fCompareValueStr;
        private int fCompareValueInt;

        public TTableIndexFindPredicate(string iCompareValueStr)
        {
            fCompareValueStr = iCompareValueStr;
        }

        public TTableIndexFindPredicate(int iCompareValueInt)
        {
            fCompareValueInt = iCompareValueInt;
        }

        public bool CompareByRecordNo(TIndexRecord iIndexObj)
        {
            return iIndexObj.RecordNo == fCompareValueInt;
        }

        public bool CompareByIndexValue(TIndexRecord iIndexObj)
        {
            return iIndexObj.IndexData.Equals(fCompareValueStr);
        }

        public bool CompareByIndexValuePartial(TIndexRecord iIndexObj)
        {
            return iIndexObj.IndexData.StartsWith(fCompareValueStr);
        }
    }

    public struct TAPIIndexPnt
    {
        public int FirstRecNdx;
        public int LastRecNdx;
        public String FirstRec;
    }

    public class TSubstituteValues : TObjBase
    {
        public string fSubstituteFrom;
        public string fSubstituteTo;

        public TSubstituteValues()
            :
            base()
        {
        }

        /// <summary>
        /// Will return exact copy of ourself.
        /// </summary>
        /// <returns></returns>
        public TSubstituteValues Clone()
        {
            return (TSubstituteValues)MemberwiseClone();
        }
    }

    public class TSubstituteValueFindPredicate
    {
        private string fCompareValue;

        public TSubstituteValueFindPredicate(string iCompareValue)
        {
            fCompareValue = iCompareValue;
        }

        public bool CompareByFromValues(TSubstituteValues iSubstituteObj)
        {
            return iSubstituteObj.fSubstituteFrom.Equals(fCompareValue);
        }

        public bool CompareByToValues(TSubstituteValues iSubstituteObj)
        {
            return iSubstituteObj.fSubstituteTo.Equals(fCompareValue);
        }
    }

    public class TTableIndex : TObjBase
    {
        #region Properties and Members
        protected string _Field = "";
        // mcb - wtf???
        //       public string Field       {get { return _Field; }set { _Field = value; } }

        protected List<TIndexRecord> _Order = new List<TIndexRecord>();
        public List<TIndexRecord> fOrder
        {
            get { return _Order; }
            set { _Order = value; }
        }

        protected List<int> _IndexFieldNos = new List<int>();
        public List<int> fIndexFieldNos
        {
            get { return _IndexFieldNos; }
            set { _IndexFieldNos = value; }
        }

        public TTableDef ParentTableDef { get { return (TTableDef)Parent; } }
        #region Host-Only Properties and Members
#if !WindowsCE && !__ANDROID__   
        /// <summary>
        /// Host side only: indicates if this is the primary key in a table
        /// </summary>
        [HostSideOnly] // Only used on the host-side
        public bool IsPrimaryKey = false;

        /// <summary>
        /// Host side only: indicates if this is THE unique constraint (basis for determining
        /// uniqueness when add a record).
        /// </summary>
        [HostSideOnly] // Only used on the host-side
        public bool IsUniqueConstraint = false;
#endif
        #endregion //Host-Only Properties and Members


        #region Android Client-Only Properties and Members
#if __ANDROID__   
        /// <summary>
        /// Host side only: indicates if this is the primary key in a table
        /// </summary>
        [HostSideOnly] // Only used on the host-side
        public bool IsPrimaryKey = false;

        /// <summary>
        /// Host side only: indicates if this is THE unique constraint (basis for determining
        /// uniqueness when add a record).
        /// </summary>
        [HostSideOnly] // Only used on the host-side
        public bool IsUniqueConstraint = false;
#endif
        #endregion 


        #endregion

        /// <summary>
        /// Set our parent.
        /// </summary>
        /// <param name="iParent"></param>
        /// <returns></returns>
#if __ANDROID__   
        /// <summary>
        /// Host side constructor to create an index and add it to the parent TTableDefRev in one
        /// step.
        /// Will only add
        /// </summary>
        /// <param name="iOwner"></param>
        /// <param name="iIndexName"></param>
        /// <param name="iColumns"></param>
        /// <param name="iPrimary"></param>
        /// <param name="iUnique"></param>
        public TTableIndex(TTableDef iOwner, String iIndexName, String[] iColumns, bool iPrimary, bool iUnique)
            : base()
        {
            if (iOwner.HighTableRevision.FindIndexByName(iIndexName) != null)
                return; // this index already exists. Don't add it again.

            Parent = iOwner;
            //Moved above because needed this obj to exist even if used parameterless constructor.            _IndexFieldNos = new List<int>();
            Name = iIndexName;
            int loFldNdx;
            // add the fields defined
            foreach (String loColumnName in iColumns)
            {
                // is this field defined in the table? Can't be indexed if it isn't.
                if ((loFldNdx = ParentTableDef.GetFldNo(loColumnName)) < 0)
                    continue;
                // it is, so add it index.
                _IndexFieldNos.Add(loFldNdx);
            }

            // don't add ourself to our parent if we are empty. 
            if (_IndexFieldNos.Count == 0)
                return; //Rely on garbage collection to sweep us away...
            IsPrimaryKey = iPrimary;
            IsUniqueConstraint = iUnique;
            // finally, add ourself to our parent.
            ParentTableDef.HighTableRevision.Indexes.Add(this);

        }

        public String ColumnNames(int iNdx)
        {
            return ParentTableDef.HighTableRevision.Fields[_IndexFieldNos[iNdx]].Name;
        }

#endif

        //private int fIndexDataOffset;
        //private int fIndexDataSize;
        //private String fIndexData;
        //private TIndexRecord fIndexRec;

        public void InitIndexSize(int iRecCount)
        {
            //collection approach negates the need
            return;

            //int loFldNdx;
            //int loFieldNo;
            //int loMaxIndexRecSize = 0;

            // get memory for both the order numbers as well as raw data
            // order uses one word per item.
            //fOrder->SetSorted( 0 );
            //fOrder->GrowList( iRecCount + 100 );  

            // fIndexData stores the actual data items. Allocate mem for it.
            //for (loFldNdx = 0; loFldNdx < fIndexFieldNos->fItemCnt; loFldNdx++)
            //{
            //   // extract this field's data from the record buffer
            //  loFieldNo = fIndexFieldNos->GetItem(loFldNdx);

            //    loMaxIndexRecSize += fTableDef->GetField( loFieldNo )->GetSize() + 1;
            //}

            //fIndexData = (char*)realloc_IfNecessary(fIndexData, iRecCount * loMaxIndexRecSize);
        }


        /// <summary>
        /// TableRecToIndexRec
        /// Populates fIndexRec the index field data extracted from iRecBuf which contains the entire
        /// table row of data.  iRecBuf is a tab delimited string of field values that is the "raw"
        /// data read from/written to the table file.
        /// </summary>
        /// <param name="iRecBuf"></param>
        /// <param name="iRecSize"></param>
        /// <param name="iRecordNo"></param>
        /// <returns></returns>
        public int TableRecToIndexRec(String iRecBuf, int iRecSize, int iRecordNo)
        {
            int loFldNdx;

            // init
            //fIndexRec.RecordNo = iRecordNo;
            //fIndexRec.IndexData = "";
            TIndexRecord loIndexRec = new TIndexRecord();
            loIndexRec.RecordNo = iRecordNo;
            loIndexRec.IndexData = "";


            // split the record buffer into individual columns
            string[] loColumnVals = iRecBuf.Split((char)0x09);

            // loop through all fields in the index def'n
            for (loFldNdx = 0; loFldNdx < fIndexFieldNos.Count; loFldNdx++)
            {
                // terminate previous field w/ a TAB
                if (loFldNdx > 0)
                {
                    loIndexRec.IndexData += "\t";
                }

                // copy each column value into the index 
                loIndexRec.IndexData += loColumnVals[fIndexFieldNos[loFldNdx]];

            } // for (loFldNdx... loop through all fields in index def'n


            /*    probably not going to need to maintain the indexes this way 
             * 
             // let's write this index record
            if ( (fIndexDataOffset + fIndexRec.IndexData.Length + sizeof( int )) > fIndexDataSize)
            {
                fIndexDataSize += 0x10000;
                //fIndexData = (char *)realloc_IfNecessary( fIndexData, fIndexDataSize );
            }
  
            // add the raw data 
            memcpy( &fIndexData[ fIndexDataOffset ], &fIndexRec, loIndexRecSize + sizeof( int ) );
            fIndexDataOffset += sizeof( int );

            // add the string
            fOrder->AddString( -1, &fIndexData[ fIndexDataOffset ] );
  
            // Debug.Write( "Add <%s><%s>\n", fIndexRec.IndexData, &fIndexData[ fIndexDataOffset ] ); Tfgetc(stdin);
            fIndexDataOffset += loIndexRecSize;
            fOrder.Add(loIndexData);

            //fIndexDataOffet += fIndexRec.IndexData.Length + sizeof( int );
            */

            fOrder.Add(loIndexRec);

            return 0;
        }

        public int GetIndexFieldCnt()
        {
            return fIndexFieldNos.Count;
        }

        public int GetIndexFieldNo(int iFieldNdx)
        {
            if (fIndexFieldNos.Count == 0)
                return -1;
            return fIndexFieldNos[iFieldNdx];
        }

        /// <summary>
        /// Passed an item number in the index, returns the table row number for it.
        /// </summary>
        /// <param name="iIndexRowNo"></param>
        /// <returns></returns>
        /// 
        public int GetTableRecNo(int iIndexRowNo)
        {
            // Avoid invalid indexes
            if ((iIndexRowNo < 0) || (iIndexRowNo > fOrder.Count - 1))
                return -1;
            TIndexRecord loResult = fOrder[iIndexRowNo];
            if (loResult != null)
            {
                return loResult.RecordNo;
            }
            else
            {
                return -1;
            }
        }


        public TTableIndex()
            : base()
        {
            //All of the serialized classes must have a parameterless constructor - even if they do nothing
        }

        public int SearchForRecord(String iMatchBuffer, int iBufferLen, bool iPartialMatch)
        {
            if (iPartialMatch)
            {
                TTableIndexFindPredicate loPredicate = new TTableIndexFindPredicate(iMatchBuffer.Substring(0, iBufferLen - 1));
                return fOrder.FindIndex(loPredicate.CompareByIndexValuePartial);
            }
            else
            {
                TTableIndexFindPredicate loPredicate = new TTableIndexFindPredicate(iMatchBuffer);
                return fOrder.FindIndex(loPredicate.CompareByIndexValue);
            }
        }
    }



    /// <summary>
    /// /// Summary description for TTableDef.
    /// </summary>

    /* The XmlInclude attribute is used on a base type to indicate that when serializing 
	 * instances of that type, they might really be instances of one or more subtypes. 
     * This allows the serialization engine to emit a schema that reflects the possibility 
     * of really getting a Derived when the type signature is Base. For example, we keep
     * field definitions in a generic collection of TTableDef. If an array element is 
     * TMasterTableDef, the XML serializer gets mad because it was only expecting TTableDef. 
     */
    [XmlInclude(typeof(TMasterTableDef)), XmlInclude(typeof(TReadTableDef))]
    public class TTableDef : TObjBase
    {
        public enum ListAccessLevel
        {
            lalUser,
            lalSupervisor,
            lalSystem
        }

        public enum ListPopularListType
        {
            None,
            HostMaintained,
            DeviceMaintained
        }

        #region Properties and Members

        // KLC [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        // KLC public TReinoFileObj fFileHandle;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public String fTblName;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public String fTblNameExt;             // defaults to .DAT if not specified in layout 

        protected String fTblCompletePathAndFileName;
        protected int fRevision;
        protected int fBiggestFieldSize;

        // protected int fUncompSize;
        protected int _MaxRecSize;
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public int fMaxRecSize          // max size of a record. 
        {
            get { return _MaxRecSize; }
            set
            {
                _MaxRecSize = value;
#if WindowsCE // thread-unsafe code only allowed in issuance ap.
                fRecBuffer.EnsureCapacity(_MaxRecSize);
#else
                if (TClientDef.SkipHostSideOnlyCode == true)
                    fRecBuffer.EnsureCapacity(_MaxRecSize); // For PatrolCar
#endif
            }
        }

        protected int fCurRecNo;			// number of record currently in recbuffer
        public int GetCurRecNo()
        {
            return fCurRecNo;
        }

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public int fPrimaryKeyFldCnt;	    // number of fields in the primary key, i.e., depth of sorting 

        protected ListAccessLevel _AccessLevel = ListAccessLevel.lalUser;
        [System.ComponentModel.DefaultValue(ListAccessLevel.lalUser)] // This prevents serialization of default values
        public ListAccessLevel AccessLevel
        {
            get { return _AccessLevel; }
            set { _AccessLevel = value; }
        }

        /// <summary>
        /// A collection of TTableDefRev objects
        /// </summary>
        protected ListObjBase<TTableDefRev> _Revisions;
        public ListObjBase<TTableDefRev> Revisions
        {
            get { return _Revisions; }
            set { _Revisions = value; }
        }

        protected ListPopularListType _PopularListType;
        public ListPopularListType PopularListType
        {
            get { return _PopularListType; }
            set { _PopularListType = value; }
        }

        protected TTableDefRev _HighTableRevision = null;
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public TTableDefRev HighTableRevision
        {
            get
            {
                // Profiling the app showed this to be expensive for some reason, so we'll try to 
                // speed it up by keeping a reference to the high revision instead of always searching.
                if (_HighTableRevision != null)
                    return _HighTableRevision;

                // Cant just return the last object in list, because this is not guaranteed to
                // be the highest revision. So we will loop thru the list and manually find it.
                // (will never be more than a few revisions anyways, so time taken is minimal).
                TTableDefRev highRevision = null;
                foreach (TTableDefRev oneRevision in Revisions)
                {
                    if ((highRevision == null) || (oneRevision.Revision > highRevision.Revision))
                    {
                        highRevision = oneRevision;
                    }
                }

                // Ok, found highest. Return it after retaining it in protected member.
                _HighTableRevision = highRevision;
                return highRevision;
            }
        }

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public bool fOpenEdit;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public DateTime fDBCreationDateTime;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public String fMasterTable;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public String fMasterColumn;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public String fGroup;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public String fDetailTable;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public DateTime fDBEffectiveDateTime;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public String fListType;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public String fFileName;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public ListPopularListType fPopListType;

        private StringBuilder _RecBuffer;		// buffer area to store a record. 

        /// <summary>
        /// RecBuffer holds a single row of a table.  It is not thread-safe in that multiple TTTable instances
        /// link back to a single TTableDef instance and can access this member simultaneously.
        /// </summary>
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public StringBuilder fRecBuffer
        {
            get
            {
#if !WindowsCE && !__ANDROID__   
                if (TClientDef.GuaranteedThreadSafe == false)
                    throw new Exception("Attempted to access TTableDef::fRecBuffer in  instance " + this.Name + ". This member is not thread-safe.");
#endif
                return _RecBuffer;
            }
        }


        #endregion
        /// <summary>
        /// Returns the TTableDefRev that applies to the passed iCfgRev.
        /// Each new TTableDefRev is stamped with the CfgRev it was created with.  Not every CfgRev
        /// causes a new TTableDef rev, so there can be gaps.
        /// </summary>
        /// <param name="iCfgRev"></param>
        /// <returns></returns>
        public TTableDefRev TableRevForCfgRev(int iCfgRev)
        {
            TTableDefRev loResult = null;
            foreach (TTableDefRev loTableDefRev in _Revisions)
            {
                if (loTableDefRev.CurrentCFGRev <= iCfgRev)
                {
                    if ((loResult == null) || (loTableDefRev.CurrentCFGRev > loResult.CurrentCFGRev))
                    {
                        loResult = loTableDefRev;
                    }
                }
            }

            return loResult;
        }


        public void ClearRecBuf()
        {
            fCurRecNo = -1;
            fRecBuffer.Length = 0;
        }

        public int FindIndexCollectionElementNoByName(string iFindName)
        {
            TObjBasePredicate loPredicate = new TObjBasePredicate(iFindName);
            return this.HighTableRevision.Indexes.FindIndex(loPredicate.CompareByName);
        }

        public TTableIndex GetIndex(int iItemNdx)
        {
            if (this.HighTableRevision.Indexes == null) return null;
            return this.HighTableRevision.Indexes[iItemNdx];
        }

        public TTableIndex GetIndex(String iIndexName)
        {
            int loIndex = FindIndexCollectionElementNoByName(iIndexName);
            if (loIndex < 0) return null;
            return this.HighTableRevision.Indexes[loIndex];
        }

        public int GetTableIndexCnt()
        {
            if (this.HighTableRevision.Indexes == null) return 0;
            return this.HighTableRevision.Indexes.Count;
        }

        public int WriteRecordBuf()
        {
            int loStatus = 0;
            /*  KLC
            if (fFileHandle == null) return -1;// fFileHandle;

            if ((loStatus = fFileHandle.WriteFileRec(fRecBuffer.ToString())) < 0)
            {
                /*
                // write failed. If it was due to insufficient disk space, do a reclaim then try again
                if (loStatus != FILE_DISKFULL) return loStatus;

                ReclaimFlash2();
                if ((loStatus = OSWriteFileRec(fFileHandle, fRecBuffer, fRecBuffer.Length)) < 0)
                    return loStatus;
                 * /

                return loStatus;
            }

            // adding a record may have caused a new index point to have been created. 
            //fIndexPntCnt = 1; // OSFileGetIndexPntCnt(fFileHandle);
            // fRecCount++;

            // add this to all the indexes
            foreach (TTableIndex loNdx in this.HighTableRevision.Indexes)
            {
                loNdx.TableRecToIndexRec(fRecBuffer.ToString(), fRecBuffer.Length, fFileHandle.RecCount - 1);
            }
            */
            return loStatus;
        }

        public void DeleteLastRecord()
        {
        /* KLC
            if (fFileHandle == null) return;

            // First we want to remove last record from all indexes
            foreach (TTableIndex loNdx in this.HighTableRevision.Indexes)
            {
                // Get the last index object. If its for the last record (it should be), remove it
                TIndexRecord LastIndex = loNdx.fOrder[loNdx.fOrder.Count - 1];
                if (LastIndex.RecordNo == fFileHandle.RecCount - 1)
                    loNdx.fOrder.RemoveAt(loNdx.fOrder.Count - 1);
            }

            // Now lets remove the record from the physical file
            fFileHandle.DeleteLastRecord();
         */ 
        }

        protected void SetTableName()
        {
            String loFmtRevisionStr = "";

#if PATROL_CAR_AIR
            // JLA 2009.05.18 -- Revision wasn't begin set, so the filename could be wrong
            fRevision = this.HighTableRevision.Revision;
#endif
            if (this.HighTableRevision != null)
                fRevision = this.HighTableRevision.Revision;
            if (fRevision > 0)
            {
                // the file name will have the revision number 
                // included only if the revision is greater than 0
                loFmtRevisionStr = Convert.ToString(fRevision);
                loFmtRevisionStr = loFmtRevisionStr.PadLeft(3, '0');
                loFmtRevisionStr = "_" + loFmtRevisionStr;
            }

            // build the table name, start with the object name
            fTblName = this.Name;

            // if an extension hasn't been specified, default to .DAT
            if (fTblNameExt.Length == 0)
            {
                fTblNameExt = ".DAT";
            }
            else
            {
                // the object name already includes the extension to preserve its uniqueness
                // we need to delete it before we add the revision string and custom extension
                fTblName = Path.ChangeExtension(fTblName, null); // fTblName.Substring(1, fTblName.Length - fTblNameExt.Length);
            }


            // now add the revision string (if any)
            if (loFmtRevisionStr.Length > 0)
            {
                fTblName += loFmtRevisionStr;
            }

            // and finally the file extension
            fTblName += fTblNameExt;


            fTblCompletePathAndFileName = "";

            /*
            #if (HHTarget==HHTarget_PocketPC) || (HHTarget == HHTarget_Series3CE)
            // in WinCE, we need to specify the folder too
            strcat( fTblCompletePathAndFileName, "\\AutoISSUE\\" );
            #endif
             */

            fTblCompletePathAndFileName += Reino.ClientConfig.ReinoTablesConst.cnstClientConfigFolder + fTblName;
        }

#if !WindowsCE && !__ANDROID__   
        /// <summary>
        /// Forces the certain datacolumns to be uniform in their size and type
        /// (UNIQUEKEY, MASTERKEY, SRCMASTERKEY, etc )
        /// </summary>
        private void EnforceHostSideColumnAttributeRequirements()
        {

            if (this.HighTableRevision == null)
            {
                return;
            }


            // enforce the uniform size/datatypes for key fields
            for (int loKeyFieldArrayIdx = 0; loKeyFieldArrayIdx < DBConstants.cnUniformKeyFieldsArray.Length; loKeyFieldArrayIdx++)
            {
                // look for this field 
                int loKeyFieldTableIdx = this.GetFldNo(DBConstants.cnUniformKeyFieldsArray[loKeyFieldArrayIdx]);
                // if its defined in this table, enforce its standard size
                if (loKeyFieldTableIdx != -1)
                {
                    TTableFldDef loKeyFieldTableDef = this.HighTableRevision.Fields[loKeyFieldTableIdx];
                    loKeyFieldTableDef.Size = DBConstants.cnPrimaryKeyUniformSize;
                }
            }

        }
#endif




#if _needed_   
        /// <summary>
        /// helper routine Does some self-examination to make sure that the minimum column requirements are met
        /// </summary>
        /// <param name="pTableName"></param>
        protected void VerifyFieldsInTableDef(List<TAndroidClientRequiredFieldInfoObj> iVerifyFieldList, TTableDef iTableDef)
        {

            // work through the verify fields list and make sure the required fields are defined
            foreach (TAndroidClientRequiredFieldInfoObj loFieldInfo in iVerifyFieldList)
            {
                // watch out for tables that have no defintion to search
                if ((iTableDef.HighTableRevision != null) && (iTableDef.GetFldNo(loFieldInfo.FieldName) != -1))
                    continue; // field already exists. let's move on.

                TTableFldDef loTableFldDef;
                // not present - add it by type
                switch (loFieldInfo.FieldType)
                {
                    case (TDataBaseColumnDataType.dbtInteger):
                        loTableFldDef = new TTableIntFldDef();
                        break;
                    case (TDataBaseColumnDataType.dbtReal):
                        loTableFldDef = new TTableRealFldDef();
                        break;
                    case (TDataBaseColumnDataType.dbtDateTime):
                        if (loFieldInfo.FieldName.Contains("TIME"))
                            loTableFldDef = new TTableTimeFldDef();
                        else
                            loTableFldDef = new TTableDateFldDef();
                        break;
                    //case (TDataBaseColumnDataType.dbtBLOB):
                    //    loTableFldDef = new TTableBLOBFldDef();
                    //    break;
                    default:
                        loTableFldDef = new TTableStringFldDef();
                        break;
                }

                // regardless of the final class, these properties are defined in the base class.

                loTableFldDef.Name = loFieldInfo.FieldName;
                loTableFldDef.DefaultValue = loFieldInfo.DefaultVal;
                loTableFldDef.Size = (int)loFieldInfo.FieldSize;
                loTableFldDef.IsHostSideDefinitionOnly = true;
                iTableDef.AddFieldToAllRevisions(loTableFldDef);
            }
        }
#endif


#if __ANDROID__   
        /// <summary>
        /// Forces minimum datacolumns to be defined for Android SQLite
        /// </summary>
        private void EnforceAndroidClientSideColumnDefinitionRequirements()
        {

            if (this.HighTableRevision == null)
            {
                return;
            }

            // all tables in Android SQLite need the identity column added
            List<TAndroidClientRequiredFieldInfoObj> loVerifyFieldList = new List<TAndroidClientRequiredFieldInfoObj>();

            // got one yet? shouldn't ever, but the layout tool is powerful...
            int loKeyFieldTableIdx = this.GetFldNo(DBConstants.ID_COLUMN);

            // if its missing, add it
            if (loKeyFieldTableIdx == -1)
            {
                // make a new field 
                TAndroidClientRequiredFieldInfoObj loNewField = new TAndroidClientRequiredFieldInfoObj();
                loNewField.FieldName = DBConstants.ID_COLUMN;
                loNewField.FieldSize = AutoISSUE.DBConstants.cnPrimaryKeyUniformSize;
                loNewField.FieldType = TDataBaseColumnDataType.dbtInteger;
                loNewField.NotNull = true;
                loNewField.DefaultVal = "";
                loNewField.Indexed = false; // pIndexed;
                loNewField.NoInsertTrigger = false; // pNoInsertTrigger;

                // add it 
                loVerifyFieldList.Add(loNewField);
            }

            if (loVerifyFieldList.Count > 0)
            {
                Reino.ClientConfig.TTableListMgr.glTableMgr.VerifyFieldsInTableDef(loVerifyFieldList, this);
            }

        }
#endif


        /// <summary>
        /// As a container object, must call each contained object's ResolveRegistryItems
        /// </summary>
        public override void ResolveRegistryItems(ITTRegistry iRegistry)
        {
            base.ResolveRegistryItems(iRegistry);
            _Revisions.ResolveRegistryItemsForListItems(iRegistry);
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            int loRecNdx;

            // Call inherited PostDeserialize w/ our parent.
            base.PostDeserialize(iParent);

            // call children's postdeserialize w/ ourself as parent.
            _Revisions.PostDeserializeListItems(this);

#if __ANDROID__   
            // all tables in Android SQLite need the identity column added
            EnforceAndroidClientSideColumnDefinitionRequirements();
#endif


#if !WindowsCE && !__ANDROID__   
            // make sure that special columns are uniform in their size and type
            if (TClientDef.SkipHostSideOnlyCode == false)
                EnforceHostSideColumnAttributeRequirements();
#endif


            // add this table def to the list of tables 
            Reino.ClientConfig.TTableListMgr.glTableMgr.AddTableDef(this);

            SetTableName();

            // open the table
#if PATROL_CAR_AIR
            // mcb : OpenEditTable will throw an exception on failure. We can't handle it here in the constructor,
            // so we'll suppress it for now. When the file that caused the exception is accessed in the future, the exception will
            // be handled properly then.
            try
            {
#endif
                if (fOpenEdit)
                    OpenEditTable(500); // set to 5 for testing purposes
                else
                    OpenReadTable(500);
#if PATROL_CAR_AIR
            }
            catch
            {
            }
#endif
/* KLC
            // get the date of the file
            if (fFileHandle.FileInformation.Exists == true)
            {
                fDBCreationDateTime = fFileHandle.FileInformation.CreationTime;
#if PATROL_CAR_AIR
                fDBEffectiveDateTime = fFileHandle.FileInformation.LastWriteTime;
#endif
            }
            */
            // build all the indexes
            if (
                (this.HighTableRevision != null) &&         // can be null if no tables exist (?)
                (this.HighTableRevision.Indexes.Count > 0)
                )
            {
                /*
                String loLine2;
                TMsgForm* loProgressForm;

                loProgressForm = ShowMessageNotModal("Building Indices for", Name );
                loProgressForm->UpdateMsgAndPaint(0, 0, "", "Reading...", "");
                 */

                // first step is to size the indices  
                foreach (TTableIndex loIndex in this.HighTableRevision.Indexes)
                {
                    loIndex.InitIndexSize(GetRecCount());
                    loIndex.fOrder.Clear();
                }

                for (loRecNdx = 0; loRecNdx < GetRecCount(); loRecNdx++)
                {
                    if ((loRecNdx & 0x3FF) == 0) // show progress every 128 records
                    {
                        //uHALr_FormatStr("%d / %d", loLine2, 40, loRecNdx, GetRecCount());
                        //loProgressForm->UpdateMsgAndPaint(0, 0, 0, 0, loLine2);
                    }
                    ReadRecordToBuf(loRecNdx);
                    foreach (TTableIndex loTableNdx in this.HighTableRevision.Indexes)
                    {
                        loTableNdx.TableRecToIndexRec(fRecBuffer.ToString(), fRecBuffer.Length, loRecNdx);
                    }
                }

                //loProgressForm->UpdateMsgAndPaint(0, 0, 0, "Sorting...", "0000000 / 9999999");
                foreach (TTableIndex loIndex in this.HighTableRevision.Indexes)
                {
                    //loIndex.fOrder.fSortProgressWin = loProgressForm->fMsgWin4;
                    //loIndex.fOrder.SetSorted(1);
                    //loIndex.fOrder.fSortProgressWin = 0;
                }

                //delete loProgressForm;
            }

            return 0;
        }

        public TTableDef()
            : base()
        {
            fCurRecNo = -1;
            fOpenEdit = false;

            //KLC fFileHandle = new TReinoFileObj();

            //if (fTblNameExt.Length = 0)
            // {
            //    fTblNameExt = ".DAT";
            // }
            fTblNameExt = "";

            // set an initial size for the record buffer - this is expanded as needed
            _RecBuffer = new StringBuilder(255);

            // Create list of Table Definition Revisions
            this._Revisions = new ListObjBase<TTableDefRev>();
        }

        public int GetRecCount()
        {
        /* KLC
            if (fFileHandle != null)
            {
                return fFileHandle.RecCount;
            }
            else
            {
                return 0;
            }
         */

         return 0; // KLC
        }


        ~TTableDef()
        {
            //short loNdx;
            CloseTable();
            //FREE( fRecBuffer );
            // deleting fFields will in-turn delete each of the TTableFlds 
            //delete fFields;
            //delete fVirtualFields;
            // remove self from TableMgr 
            Reino.ClientConfig.TTableListMgr.glTableMgr.RemoveTableDef(this);

            //if (fIndexes) delete fIndexes;
        }

        public short CloseTable()
        {
        /* KLC
            if (fFileHandle != null)
            {
                fFileHandle.CloseFile();
#if PATROL_CAR_AIR
                fFileHandle = null;
#endif
            }
            */
            return 0;
        }

        /// <summary>
        /// SeekToRecord( int iRecordNo )
        /// </summary>
        public void SeekToRecord(int iRecordNo)
        {
           //KLC  fFileHandle.FileSeekRecord(iRecordNo);
        }

        /// <summary>
        ///  Opens table for writing/editing.  Fails if table already open.
        /// </summary>
        /// <param name="iRecsBetweenIndexPnts"></param>
        /// <returns></returns>
        public short OpenEditTable(int iRecsBetweenIndexPnts)
        {

            //short loRecDelimiter = 0x0A0D;
#if PATROL_CAR_AIR
            if (fFileHandle == null)
                fFileHandle = new TReinoFileObj();
#endif

            // open the source file 
     //KLC        fFileHandle.OpenFileForReadWrite(fTblName, iRecsBetweenIndexPnts);

            // must implement exception handling!!!

            /*
            if (fFileHandle < 0)
            {
    
                // openwrite failed. why?
                switch ( fFileHandle )
                {

                    case FILE_DISKFULL :
                    {
                        // If out of disk space, do a reclaim then try again
                        // even with the check about, the FAT could be full and
                        // return an error when opening reclaim and try again
                        ReclaimFlash2();
                        fFileHandle = OSOpenFile( fTblName, FT_SHORTDELIMITTED_RECORD, OPENMODE_ReadWriteAppend, 0, fFragmentSize, &loRecDelimiter, FILEATTR_FRAGMENTABLE );
                        if (fFileHandle < 0 )
                        {
	                        // didn't work even after a reclaim.. not enough room to write the records
                            ShowMessage( "Insufficient Flash. Unable to write data", "Unload handheld at host." );
                            // should this be a fatal application error?
                            return fFileHandle;
                        }
                        // got a good open after reclaim, break from error recovery and continue
                        break;
                    }

                    // all other errors
                    default :
                    {  
                        char loLine1[60];
                        uHALr_FormatStr( "Failed file open: %d", loLine1, 60, fFileHandle );
 	  			        ShowMessageLong( GetName(), loLine1, TranslateFileErrorCode2( fFileHandle ), 0, 0  );
                        return fFileHandle;
                    }
                }

            }  // of fFileHandle < 0 on initial open
             */

            // set up index points to make searching faster.  For now, make it every 1000 records.
            //OSFileStats( fFileHandle, 0, 0, 0, &fRecCount );
            //fIndexPntCnt = OSFileGetIndexPntCnt( fFileHandle );
            return 0;
        }

        /// <summary>
        ///  Opens table for reading.  Fails if table already open.
        /// </summary>
        /// <param name="iRecsBetweenIndexPnts"></param>
        /// <returns></returns>
        public short OpenReadTable(int iRecsBetweenIndexPnts)
        {
        /* KLC
            // reconstitute the file
            //DoReconstitute( fTblName, 0, 0 );

            // open the source file 
            //fFileHandle = OSOpenFile( fTblName, FT_SHORTDELIMITTED_RECORD, OPENMODE_Read, 0, 0, &loRecDelimiter, 0 );
#if PATROL_CAR_AIR
            if (fFileHandle == null)
                fFileHandle = new TReinoFileObj();
#endif

            // open the source file. The first record will be returned to us in a string
            string loFirstRec = fFileHandle.OpenFileForRead(fTblName, iRecsBetweenIndexPnts);

            // mcb 4/27/2006: AI.NET forces masterkey to a fixed length.  This screws up masterkey
            // filter searches.  If this table has a masterkey column, read in the 1st row.  If it's
            // length is different from the ISSUE_AP.CFG setting, update it accordingly.
            // This is only applicable to X3 handhelds and patrolcar/emulator (GuaranteedThreadSafe == true)
#if !WindowsCE && !__ANDROID__   
            if (TClientDef.GuaranteedThreadSafe == true)
            {
#else
            {
#endif
                int loMasterKeyFldNo = this.GetFldNo(DBConstants.sqlMasterKeyStr);
                if (loMasterKeyFldNo >= 0)
                {
                    int loFldLength = loFirstRec.IndexOf("\t");
                    if (loFldLength != this.GetField(loMasterKeyFldNo).Size)
                        this.GetField(loMasterKeyFldNo).Size = loFldLength;
                }
            }

            /* need to implement exception handling!
            if (fFileHandle < 0)
            {
                if (fFileHandle != FILE_NOT_FOUND)
                {
                    char loLine1[60];
                    uHALr_FormatStr( "Failed file open: %d", loLine1, 60, fFileHandle );
                    //ShowMessage( GetName(), loLine1 );
				    ShowMessageLong( GetName(), loLine1, TranslateFileErrorCode2( fFileHandle ), 0, 0  );
                }   
                return fFileHandle;
            }
             */

            // set up index points to make searching faster.  For now, make it every 1000 records.
            //OSFileStats( fFileHandle, 0, 0, 0, &fRecCount );
            //fIndexPntCnt = OSFileGetIndexPntCnt( fFileHandle );
            return 0;
        }

        /// <summary>
        /// Routine reads the record into fRecBuffer
        /// </summary>
        /// <param name="iRecordNo"></param>
        /// <returns></returns>
        public int ReadRecordToBuf(int iRecordNo)
        {
            int loStatus = 0;
            /* KLC
            // initialize the record buffer 
            fCurRecNo = -1;
            fRecBuffer.Length = 0;

            if ((fFileHandle == null))
            {
                return -1;
            }

            if (iRecordNo < 0)
            {
                Debug.WriteLine("ReadRecordToBuf(): Record to low: " + iRecordNo.ToString());
            }

            //if ( (loStatus = OSFileSeekRec( fFileHandle, iRecordNo ) ) < 0)  return loStatus;
            //if ( (loStatus = OSReadFileRec( fFileHandle, fRecBuffer, fMaxRecSize, &fRecSize ) ) < 0) return loStatus;
            SeekToRecord(iRecordNo);

            fRecBuffer.Length = 0;
#if PATROL_CAR_AIR
            // there are a lot of assumptions here that aren't working, specifically when at the end of a file.
            // ReadCurrentFileRec() returns null on failure, so we'll use that as our end of file indicator.
            string loRecord = fFileHandle.ReadCurrentFileRec();
            if (loRecord == null)
                return -2;
            fRecBuffer.Append(loRecord);
            return 0; // success
#else
            fRecBuffer.Append(fFileHandle.ReadCurrentFileRec());

            // DEBUG: Added this to help with TER_ListFilter problem....
            // Now update the record position
            if (fCurRecNo > this.GetRecCount())
                fCurRecNo = this.GetRecCount();
            else
                fCurRecNo = iRecordNo;
               
            return loStatus;
#endif
            */
            return loStatus;  // KLC
        }

        /// <summary>
        /// Returns a substring starting at iStart position and upto iMaxLength characters.
        /// If iStart is invalid, and empty string is returned. If there are not enough characters
        /// to satisfy iMaxLength, then the returned substring will contain as many characters as
        /// possible.
        /// </summary>
        private static string SafeSubString(string iString, int iStart, int iMaxLength)
        {
            if (iStart > iString.Length - 1)
                return "";
            return iString.Substring(iStart, Math.Min(iMaxLength, (iString.Length - iStart)));
        }

        /// <summary>
        /// Searches for the record that matches iRecordValue.
        /// </summary>
        /// <param name="iRecordValue"></param>
        /// <param name="iMatchChars"></param>
        /// <param name="iClosestNdx"></param>
        /// <returns></returns>
        public int FindRecord(String iRecordValue, int iMatchChars, ref int iClosestNdx)
        {
        /* KLC
            int loLowNdx;
            int loHighNdx;
            int loMidNdx;
            int loIndexPntNdx = -1;
            int loLastMatchNdx = -1;
            TAPIIndexPnt loIndexPnt = new TAPIIndexPnt();
            int loComparison;

            if (iMatchChars <= 0)
                iMatchChars = iRecordValue.Length + 1;

            /*
            if (iClosestNdx != null)
            {
                iClosestNdx = -1;
            }
             * /

            if (fFileHandle.RecCount == 0)
            {
                ReadRecordToBuf(-1); // make sure we are pointing at nothing
                return -1; //won't find anything in an empty list 
            }

            // first off, search the index points for the file chunk that this record resides in. * /
            loLowNdx = 0;
            loHighNdx = fFileHandle.GetIndexPntCnt() - 1;
            loMidNdx = loHighNdx >> 1;

            for (; ; )
            {
                // read the record at the current index point 
                fFileHandle.GetRecAtIndexPnt(loMidNdx, ref loIndexPnt);

                //Debug.Write( "Idx Pnt:%d(%d,%d); 1st:%d; Last:%d\n Rec:<%s>...\n", loMidNdx, loLowNdx, loHighNdx, loIndexPnt.FirstRecNdx,
                //    loIndexPnt.LastRecNdx, loIndexPnt.FirstRec );
                //Debug.ReadLine();


                if ((iRecordValue == null) || (iRecordValue == "") || (loIndexPnt.FirstRec == null))
                {
                    ReadRecordToBuf(-1); // make sure we are pointing at nothing
                    return -1;  // should this really be happening?
                }

                loComparison = iRecordValue.CompareTo(SafeSubString(loIndexPnt.FirstRec, 0, iMatchChars));
                if (loComparison <= 0)
                {
                    if ((loComparison == 0) && ((loIndexPntNdx == -1) || (loMidNdx < loIndexPntNdx)))
                    {
                        // Debug.Write( "%s == to %s\n", iRecordValue,  loIndexPnt.FirstRec ); 
                        loIndexPntNdx = loMidNdx;
                    }

                    // value we seek is before loMidNdx 
                    if ((loMidNdx == loLowNdx) || (loMidNdx == 0))
                        break; /* done searching index points * /
                    loHighNdx = loMidNdx - 1;
                }
                else // (loComparison > 0) 
                {
                    // value we seek is after loMidNdx 
                    loLowNdx = loMidNdx;
                    loIndexPntNdx = loMidNdx;
                    if (loMidNdx == loHighNdx)
                        break;
                }

                // commented out in original code loMidNdx = (loHighNdx + loLowNdx) >> 1;

                // if low and high are only one apart, and we just compared the low, adjust by 1 
                if (((loLowNdx + 1) == loHighNdx) && (loMidNdx == loLowNdx))
                    loLowNdx = loHighNdx; // next time through will be the last 
                // calculate new middle 
                loMidNdx = (loHighNdx + loLowNdx) >> 1;
            } // for (;;), find an appropriate index point from which to begin searching  


            // Debug.Write( "Start Search Index Pnt: %dj\n", loIndexPntNdx ); Tfgetc(stdin);

            // at this point, loIndexPntNdx points to the first chunk that potentially contains this record.
            if (loIndexPntNdx < 0)
            {
                ReadRecordToBuf(-1); // make sure we are pointing at nothing
                return -1;
            }

            // loIndexPntNdx points to the first chunk that conceivably could contain the record.
            // However, index points after it might also contain the record.  Each index point
            // starting from loLstEarlyNdx will be searched until an index point that is greater
            // than the sought value is found or the record itself if found.

            for (; loIndexPntNdx < fFileHandle.GetIndexPntCnt(); loIndexPntNdx++)
            {   // outer loop for searching index chunks 
                fFileHandle.GetRecAtIndexPnt(loIndexPntNdx, ref loIndexPnt);

                //if (strncmp(iRecordValue, loIndexPnt.FirstRec, iMatchChars) < 0)
                if (iRecordValue.CompareTo(SafeSubString(loIndexPnt.FirstRec, 0, iMatchChars)) < 0)
                {
                    ReadRecordToBuf(-1); // make sure we are pointing at nothing
                    return -1; // sought record is before this index, so we are done. 
                }

                //Debug.Write( "Search in Idx Pnt:%d; 1st:%d; Last:%d; Rec:<%s>...\n", loIndexPntNdx, loIndexPnt.FirstRecNdx,
                //    loIndexPnt.LastRecNdx, loIndexPnt.FirstRec );

                loHighNdx = loIndexPnt.LastRecNdx;
                loLowNdx = loIndexPnt.FirstRecNdx;
                loMidNdx = (loHighNdx + loLowNdx) >> 1;

                for (; ; )
                {   // inner loop for performing a search within the chunk 
                    if (iClosestNdx != null)
                    {
                        iClosestNdx = loMidNdx;
                    }

                    // read the record at the current position 
                    ReadRecordToBuf(loMidNdx);

                    //loComparison = strncmp(iRecordValue, fRecBuffer, iMatchChars);
                    loComparison = iRecordValue.CompareTo(SafeSubString(fRecBuffer.ToString(), 0, iMatchChars));
                    if (loComparison == 0)
                    {
                        // Found it! This value matches. However, there could be multiple matches, and
                        // we want to return the 1st match. We will work backwards until we find it. 
                        loLastMatchNdx = loMidNdx;
                        if (loMidNdx == loLowNdx)
                            break;  /* anything we search will be after this one * /
                        loHighNdx = loMidNdx - 1;
                    } // if (loComparison == 0 
                    else if (loComparison < 0)
                    {
                        // before this one 
                        if (loMidNdx == loLowNdx)
                            break;
                        loHighNdx = loMidNdx - 1;
                    }
                    else // (loComparison > 0)
                    {   // after this one 
                        loLowNdx = loMidNdx + 1;
                        if (loLowNdx > loHighNdx)
                            break;
                    }

                    // if low and high are only one apart, and we just compared the low, adjust by 1 
                    if (((loLowNdx + 1) == loHighNdx) && (loMidNdx == loLowNdx))
                        loLowNdx = loHighNdx; // next time through will be the last 
                    // calculate new middle 
                    loMidNdx = (loHighNdx + loLowNdx) >> 1;
                } // for (;;), search within a chunk 

                // if a match was found, return it. 
                if (loLastMatchNdx >= 0)
                {
                    ReadRecordToBuf(loLastMatchNdx);
                    return loLastMatchNdx;
                }
            } // for (;;), search between chunks 

            // fell thru w/o finding anything 
            ReadRecordToBuf(-1); // make sure we are pointing at nothing
         */ 
            return -1;
        }

        /// <summary>
        /// public routine that allows an external call to set the internal record buffer
        /// </summary>
        /// <param name="iNewRecBufferData"></param>
        public void SetRecBuffer(String iNewRecBufferData)
        {
            fRecBuffer.Length = 0;
            fRecBuffer.Append(iNewRecBufferData);
        }


        public int GetRecSize()
        {
            return fRecBuffer.Length;
        }

        //public FileInfo GetFileInfoPtr()
        // {
        //     return fFileInfo;
        // }

        public void GetTblName(ref String oTableName)
        {
            oTableName = fTblName;
        }

        public void GetTblNameExt(ref String oTblNameExt)
        {
            oTblNameExt = fTblNameExt;
        }

        // returns the complete path and file name of the table - required for
        // WinCE systems which place files in subfolders
        public void GetTblCompletePathAndFileName(ref String oTblCompletePathandFileName)
        {
            oTblCompletePathandFileName = fTblCompletePathAndFileName;
        }

        // returns just the pathname where the table is stored
        public void GetTblPathName(ref string oTblPathName)
        {
            oTblPathName = Reino.ClientConfig.ReinoTablesConst.cnstClientConfigFolder;
        }

        public void AddField(TTableFldDef iField)
        {
            // if this is the first field being added, we may need to create a field revision first
            if (HighTableRevision == null)
            {
                Revisions.Add(new TTableDefRev());
            }

            // add it
            HighTableRevision.Fields.Add(iField);

            fMaxRecSize = (fMaxRecSize + iField.Size + 1);

            if (iField.Size > fBiggestFieldSize)
            {
                fBiggestFieldSize = iField.Size;
                //   foreach ( TTTable loTable in Tables )
                //   {
                //       loTable.ReAllocateTmpFieldBuf(fBiggestFieldSize);
                //   }
            }
        }

        public void AddFieldToAllRevisions(TTableFldDef iField)
        {
            // First, add the field to the highest revision.
            AddField(iField);

            // Then, add it to all other revisions.
            foreach (TTableDefRev oneTableRevision in Revisions)
            {
                // Already did the high revision.
                if (oneTableRevision == HighTableRevision) { continue; }
                // Cant just add the field directly or all revisions would have
                // the exact same instance in them. Instead will create a clone
                // of this field and add this.
                TTableFldDef copyOfField = iField.Clone();
                oneTableRevision.Fields.Add(copyOfField);
            }
        }

        public TTableFldDef GetField(int iFieldNdx)
        {
            if (iFieldNdx < 0)
                return null;
            if (iFieldNdx < this.HighTableRevision.Fields.Count)
                return this.HighTableRevision.Fields[iFieldNdx];
            else
                return this.HighTableRevision.VirtualFields[iFieldNdx - this.HighTableRevision.Fields.Count];
        }

        /// <summary>
        ///  Gets the field number for a field name.  Virtual field numbers begin after the
        ///  real field numbers end.
        /// </summary>
        /// <param name="iName"></param>
        /// <returns></returns>
        public int GetFldNo(String iName)
        {
            // Don't bother searching for an invalid name
            if ((iName == null) || (iName.Equals("")))
                return -1;

            TObjBasePredicate loPredicate = new TObjBasePredicate(iName);
            int loResult = this.HighTableRevision.Fields.FindIndex(loPredicate.CompareByName_CaseInsensitive);
            if (loResult >= 0)
                return loResult;

            loResult = this.HighTableRevision.VirtualFields.FindIndex(loPredicate.CompareByName_CaseInsensitive);
            if (loResult >= 0)
                return loResult + this.HighTableRevision.Fields.Count;
            else
                return -1;
        }

        public TTableFldDef GetField(String iName)
        {
            // construct this in a way such that invalid field names 
            // don't cause an internal exception - let the caller deal 
            // with missing or invalid field names
            int loFldNo = GetFldNo(iName);
            if (loFldNo != -1)
            {
                return GetField(loFldNo);
            }
            else
            {
                return null;
            }
        }

        public int GetFieldCnt()
        {
            return this.HighTableRevision.Fields.Count;
        }

        public void AddVirtualField(TTableFldDef iField)
        {
            // if this is the first field being added, we may need to create a field revision first
            if (HighTableRevision == null)
            {
                Revisions.Add(new TTableDefRev());
            }

            fMaxRecSize = (fMaxRecSize + iField.Size + 1);
            if (iField.Size > fBiggestFieldSize)
                fBiggestFieldSize = iField.Size;
        }

        public TTableFldDef GetVirtualField(int iFieldNdx)
        {
            return this.HighTableRevision.VirtualFields[iFieldNdx];
        }

        public TTableFldDef GetVirtualField(String iName)
        {
            TObjBasePredicate loPredicate = new TObjBasePredicate(iName);
            return this.HighTableRevision.VirtualFields.Find(loPredicate.CompareByName_CaseInsensitive);
        }

        public TTableFldDef CloneField(TTableFldDef SrcField)
        {
            // Clone the field. First we have to create a new field of the correct class type!
            // Note that the conditional statement needs to do inherited types before base types.
            TTableFldDef NewFldDef = null;
            if (SrcField is TTableStringFldDef)
                NewFldDef = new TTableStringFldDef();
#if !WindowsCE && !__ANDROID__   
            if (SrcField is TTableBLOBFldDef)
                NewFldDef = new TTableBLOBFldDef();
#endif
            else if (SrcField is TTableIntFldDef)
                NewFldDef = new TTableIntFldDef();
            else if (SrcField is TTableRealFldDef)
                NewFldDef = new TTableRealFldDef();
            else if (SrcField is TTableTimeFldDef)
                NewFldDef = new TTableTimeFldDef();
            else if (SrcField is TTableDateFldDef)
                NewFldDef = new TTableDateFldDef();
            else if (SrcField is TTableVTableLinkFldDef)
                NewFldDef = new TTableVTableLinkFldDef();
            else if (SrcField is TTableVLinkedFldDef)
                NewFldDef = new TTableVLinkedFldDef();
            else if (SrcField is TTableElpsTimeVirtualFldDef)
                NewFldDef = new TTableElpsTimeVirtualFldDef();
            else if (SrcField is TTableMod97VirtualFldDef)
                NewFldDef = new TTableMod97VirtualFldDef();
            else if (SrcField is TTableMod10CDOnlyVirtualFldDef) // Must check for TTableMod10CDOnlyVirtualFldDef before TTableMod10VirtualFldDef
                NewFldDef = new TTableMod10CDOnlyVirtualFldDef();
            else if (SrcField is TTableMod10VirtualFldDef)
                NewFldDef = new TTableMod10VirtualFldDef();
            else if (SrcField is TTableMod10FirstCharVirtualFldDef)
                NewFldDef = new TTableMod10FirstCharVirtualFldDef();
            else if (SrcField is TTableOttawaMod10VirtualFldDef)
                NewFldDef = new TTableOttawaMod10VirtualFldDef();
            else if (SrcField is TTableAlphaPosMod10VirtualFldDef)
                NewFldDef = new TTableAlphaPosMod10VirtualFldDef();
            else if (SrcField is TTableLongBeachMod10VirtualFldDef)
                NewFldDef = new TTableLongBeachMod10VirtualFldDef();
            else if (SrcField is TTableAFPMod11VirtualFldDef)
                NewFldDef = new TTableAFPMod11VirtualFldDef();
            else if (SrcField is TTableMod10AustPostVirtualFldDef)
                NewFldDef = new TTableMod10AustPostVirtualFldDef();
            else if (SrcField is TTableVirtualFldDef) // TTableVirtualFldDef should always be the last check
                NewFldDef = new TTableVirtualFldDef();
            else
                NewFldDef = new TTableFldDef();

            // If we created a field, we need to copy the properties
            if (NewFldDef != null)
            {
                NewFldDef.DBIncludeInMatch = SrcField.DBIncludeInMatch;
                NewFldDef.DefaultValue = SrcField.DefaultValue;
                NewFldDef.DisplayName = SrcField.DisplayName;
#if !WindowsCE && !__ANDROID__   
                NewFldDef.EditDataType = SrcField.EditDataType;
#endif
                NewFldDef.ListName = SrcField.ListName;
                NewFldDef.ListOnly = SrcField.ListOnly;
                NewFldDef.Mask = SrcField.Mask;
                NewFldDef.MaskForHH = SrcField.MaskForHH;
                NewFldDef.MaxValue = SrcField.MaxValue;
                NewFldDef.MinValue = SrcField.MinValue;
                NewFldDef.Name = SrcField.Name;
                NewFldDef.RegistrySection = SrcField.RegistrySection;
                NewFldDef.Required = SrcField.Required;
                NewFldDef.Size = SrcField.Size;
                NewFldDef.SpecialListType = SrcField.SpecialListType;
                NewFldDef.TableNdx = SrcField.TableNdx;

#if !WindowsCE && !__ANDROID__   
                NewFldDef.IsHostSideDefinitionOnly = SrcField.IsHostSideDefinitionOnly;
#endif

            }
            return NewFldDef;
        }

        /// <summary>
        ///  Copy the structure (field and virtual field definitions if specified) from another table
        /// </summary>
        /// <param name="iSourceTableDef"></param>
        /// <param name="iExcludeVirtualFields"></param>
        public void CopyTableStructure(ref TTableDef iSourceTableDef, bool iExcludeVirtualFields)
        {
            // do we need to check for and wipe out any existing defn??
            // to be totally robust, yes. to be efficient, no

            // copy the fields 
            foreach (TTableFldDef loFldDef in iSourceTableDef.HighTableRevision.Fields)
            {
                /*
                // Clone the field
                TTableFldDef NewFldDef = CloneField(loFldDef);
                // Add cloned field
                this.HighTableRevision.Fields.Add(NewFldDef);
                AddField(NewFldDef);
                */

                // just copy the reference, don't create another instance
                AddField(loFldDef);
            }

            // if they dont want to exclude them, copy the virtual fields 
            if (!iExcludeVirtualFields)
            {
                foreach (TTableFldDef loVirtualFldDef in iSourceTableDef.HighTableRevision.VirtualFields)
                {
                    /*
                    // Clone the field
                    TTableFldDef NewFldDef = CloneField(loVirtualFldDef);
                    // Add cloned field
                    this.HighTableRevision.Fields.Add(NewFldDef);
                    AddField(NewFldDef);
                     */
                    // just copy the reference, don't create another instance
                    AddVirtualField(loVirtualFldDef);
                }
            }
        }

        /// <summary>
        /// Copy the structure (field AND virtual field definitions) from another table
        /// </summary>
        /// <param name="iSourceTableDef"></param>
        public void CopyTableStructure(ref TTableDef iSourceTableDef)
        {
            // call the primary method and don't exclude the virtual fields
            CopyTableStructure(ref iSourceTableDef, false /*bool iExcludeVirtualFields*/ );
        }

        /// <summary>
        /// Debug routine that displays field name & value to screen.
        /// </summary>
        public void PrintStoreFields()
        {
            //   short loNdx;
            //   for (loNdx = 0; loNdx < fFields->fItemCnt; loNdx++)
            //   {
            //       Debug.Write( "Field %s,Size <%d>..", GetField( loNdx )->GetName(), GetField( loNdx )->GetSize() );
            //   }
        }

        public int LongestFieldLen()
        {
            int loResult = 0;
            foreach (TTableFldDef loField in this.HighTableRevision.Fields)
            {
                if (loField.Size > loResult)
                {
                    loResult = loField.Size;
                }
            }
            return loResult;
        }

        public int MakeFileSpaceForRecord()
        {
            int loStatus = 0;

            if (!fOpenEdit) return 0;  // nothing to do for read-only tables

     //KLC       if (fFileHandle == null) return -1; // fFileHandle;

            // see how much of file is unused
            //if ((loStatus = OSFileStats(fFileHandle, ref loUsedSize, ref loAllocSize, ref loUncompressedSize, ref loRecCount)) < 0)
            //    return loStatus;

            //if ((loAllocSize - loUsedSize) > fMaxRecSize) return 0;

            loStatus = 0; // loStatus = OSExtendOpenFile(fFileHandle, fMaxRecSize);
            return loStatus;
        }

    }

    /// <summary>
    /// Summary description for TTableDefRev. (This is a revision inside TTableDef)
    /// </summary>
    public class TTableDefRev : TObjBase
    {
        public enum TSpecialGroupType
        {
            sgtNormal = 0,
            sgtCourtList,
            sgtCourtDetailList
        }

        #region Properties and Members

        public String DATFileName
        {
            get
            {
                if (_Revision == 0) return Name + ".DAT";
                return Name + "_" + _Revision.ToString("d3") + ".DAT";
            }
        }
        protected int _PrimaryKeyFldCnt = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int PrimaryKeyFldCnt
        {
            get { return _PrimaryKeyFldCnt; }
            set { _PrimaryKeyFldCnt = value; }
        }

        protected TTableDef.ListAccessLevel _AccessLevel = TTableDef.ListAccessLevel.lalUser;
        [System.ComponentModel.DefaultValue(TTableDef.ListAccessLevel.lalUser)] // This prevents serialization of default values
        public TTableDef.ListAccessLevel AccessLevel
        {
            get { return _AccessLevel; }
            set { _AccessLevel = value; }
        }

        protected int _Revision = 0;
        //Always write this to XML so easy to read/know what revision it is.         [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int Revision
        {
            get { return _Revision; }
            set { _Revision = value; }
        }

        protected int _CurrentCFGRev = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int CurrentCFGRev
        {
            get { return _CurrentCFGRev; }
            set { _CurrentCFGRev = value; }
        }

        /// <summary>
        /// Returns the index that is designated the primary key. Returns null if none found.
        /// </summary>
        [XmlIgnore]
        public TTableIndex PrimaryKey
        {
            get
            {
#if !WindowsCE && !__ANDROID__   
                foreach (TTableIndex loTableIndex in Indexes)
                {
                    if (loTableIndex.IsPrimaryKey) return loTableIndex;
                }
#endif
#if __ANDROID__   
                foreach (TTableIndex loTableIndex in Indexes)
                {
                    if (loTableIndex.IsPrimaryKey) return loTableIndex;
                }
#endif

                return null;
            }
        }
        /// <summary>
        /// _UniqueContraintFields
        /// A host-only list of fields the define a unique constraint.
        /// </summary>
        private List<string> _UniqueContraintFields;
        // [XmlIgnore]
        public List<string> UniqueContraintFields
        {
            get { return _UniqueContraintFields; }
            set { _UniqueContraintFields = value; }
        }

        protected ListObjBase<TTableFldDef> _Fields;
        /// <summary>
        /// A collection of TTableFldDef objects
        /// </summary>
        public ListObjBase<TTableFldDef> Fields
        {
            get { return _Fields; }
            set { _Fields = value; }
        }

        /// <summary>
        /// A collection of TTableIndex objects.
        /// </summary>
        protected ListObjBase<TTableIndex> _Indexes;

        // Indexes need to be serialized or wont get them from LYT file. This should be ok with this property,
        // since the layout converter does not call PostDeserialize for client so only indexes converted from
        // LYT file will included in the XML file.
        //        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public ListObjBase<TTableIndex> Indexes
        {
            get { return _Indexes; }
            set { _Indexes = value; }
        }

        public TTableIndex FindIndexByName(String iIndexName)
        {
            return _Indexes.Find(new TObjBasePredicate(iIndexName).CompareByName);
        }
        /// <summary>
        /// A collection of TTableFldDef objects.
        /// </summary>
        protected ListObjBase<TTableFldDef> _VirtualFields;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public ListObjBase<TTableFldDef> VirtualFields
        {
            get { return _VirtualFields; }
            set { _VirtualFields = value; }
        }

        /// <summary>
        /// A collection of TTTable objects
        /// </summary>
        protected ListObjBase<TTTable> _Tables;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public ListObjBase<TTTable> Tables
        {
            get { return _Tables; }
            set { _Tables = value; }
        }

        protected TSpecialGroupType _SpecialGroupType = TSpecialGroupType.sgtNormal;
        [System.ComponentModel.DefaultValue(TSpecialGroupType.sgtNormal)] // This prevents serialization of default values
        public TSpecialGroupType SpecialGroupType
        {
            get { return _SpecialGroupType; }
            set { _SpecialGroupType = value; }
        }

        // These only used to create RPL for Acw. May be another way to do this, but cant see it right now. These
        // dont take up much memory and are not written to xml file unless different than default, so doesn't seem
        // like a big deal to have them here.
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool MasterAccessOnly = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool SubAgencyAccessOnly = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool AllowDuplicates = false;
        #endregion


        public TTableDefRev()
            : base()
        {
            this._Fields = new ListObjBase<TTableFldDef>();
            this._Indexes = new ListObjBase<TTableIndex>();

            this._VirtualFields = new ListObjBase<TTableFldDef>();
            this._Tables = new ListObjBase<TTTable>();
            _UniqueContraintFields = new List<string>();
        }

        /// <summary>
        /// FillUniqueContraintList
        /// Called by PostDeserialize to fill _UniqueConstraintList. 
        /// _UniqueContraintList is used by the host to place a Unique index on a group of fields
        /// that can define a unique element separate from the primary key.  It is used to prevent
        /// duplicates upon initially adding records.
        /// Only does something if the unique constraint list is empty.
        /// 
        /// </summary>
        private void FillUniqueConstraintList()
        {
            // 
            if (_UniqueContraintFields.Count > 0)
                return; // already populated (perhaps by deserialization?), nothing to do.
            // IssueDate and IssueTime are always part of the unique constraint.


            // If MasterKey and OccurNo exist, this is a child table with order dependencies.
            if ((Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlMasterKeyStr).CompareByName) >= 0) &&
                 (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlOccurNoStr).CompareByName) >= 0))
            {
                _UniqueContraintFields.Add(DBConstants.sqlMasterKeyStr);
                _UniqueContraintFields.Add(DBConstants.sqlOccurNoStr);
                return; // MasterKey and OccurNo are sufficient.
            }

            // If MasterKey and DetailRecNo exist, this is a child table like the NOTES table
            // that increment each child record (very similar to masterkey + occurno).
            if ((Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlMasterKeyStr).CompareByName) >= 0) &&
                 (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlDetailRecNumberStr).CompareByName) >= 0))
            {
                _UniqueContraintFields.Add(DBConstants.sqlMasterKeyStr);
                _UniqueContraintFields.Add(DBConstants.sqlDetailRecNumberStr);
                return; // MasterKey and DetailRecNo are sufficient.
            }

            // If MasterKey and SrcMasterKey exist, then this is a Reissue type table and we will
            // use these 2 fields as the only fields in the unique constraint.
            if ((Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlMasterKeyStr).CompareByName) >= 0) &&
                 (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlSourceIssueMasterKeyStr).CompareByName) >= 0))
            {
                _UniqueContraintFields.Add(DBConstants.sqlMasterKeyStr);
                _UniqueContraintFields.Add(DBConstants.sqlSourceIssueMasterKeyStr);
                return; // MasterKey and SrcMasterKey are sufficient.
            }

            // L.A. abandoned module is a special case where CONTROLNO and ORDERTYPECODE field indicate 
            // uniqueness, so we will NOT include IssueDate, IssueTime and IssueNo in the unique constraint.
            // This allows us to "reissue" workorders in such a way that the original record is 
            // updated instead of actually importing a new record.
            if ((Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlControlNumberStr).CompareByName) >= 0) &&
                (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlOrderTypeCodeStr).CompareByName) >= 0) &&
                (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlIssueNumberStr).CompareByName) >= 0))
            {
                _UniqueContraintFields.Add(DBConstants.sqlControlNumberStr);
                _UniqueContraintFields.Add(DBConstants.sqlOrderTypeCodeStr);
                _UniqueContraintFields.Add(DBConstants.sqlIssueNumberStr);
                return;
            }

            // If either of these fields doesn't exist then we will abandon them.
            if (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlIssueDateStr).CompareByName) >= 0)
                _UniqueContraintFields.Add(DBConstants.sqlIssueDateStr);
            if (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlIssueTimeStr).CompareByName) >= 0)
                _UniqueContraintFields.Add(DBConstants.sqlIssueTimeStr);

            if (_UniqueContraintFields.Count < 2)
            {
                _UniqueContraintFields.Clear();
                return;
            }

            // if there is an issue number, it and issue date + time define uniqueness
            if (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlIssueNumberStr).CompareByName) >= 0)
            {
                _UniqueContraintFields.Add(DBConstants.sqlIssueNumberStr);

                // Special case is L.A. where Control number is more important than Issue number.
                // For now, this should never happen because of the sqlControlNumberStr check above.
                // This code has been left incase we decide to use more fields for uniqueness in
                // Abandoned Vehicles.
                if ((Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlControlNumberStr).CompareByName) >= 0) &&
                    (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlOrderTypeCodeStr).CompareByName) >= 0))
                {
                    _UniqueContraintFields.Add(DBConstants.sqlControlNumberStr);
                    _UniqueContraintFields.Add(DBConstants.sqlOrderTypeCodeStr);
                }

                // Normally we would have three columns for index, but L.A. will have 5 columns 
                // because ControlNo is used for Abandoned Vehicle structure.
                if (_UniqueContraintFields.Count >= 4) return; // that's good enough.
            }

            // Next, look for plate & VIN (we'll risk the same plate w/ different state at the 
            // same date & time
            if (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlVehLicNoStr).CompareByName) >= 0)
            {
                _UniqueContraintFields.Add(DBConstants.sqlVehLicNoStr);
            }

            if (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlVehVINStr).CompareByName) >= 0)
            {
                _UniqueContraintFields.Add(DBConstants.sqlVehVINStr);
            }

            // Is there a name in here somewhere? date/time/first & last name should be good.
            if (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlSuspLastNameStr).CompareByName) >= 0)
            {
                _UniqueContraintFields.Add(DBConstants.sqlSuspLastNameStr);
            }
            if (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlSuspFirstNameStr).CompareByName) >= 0)
            {
                _UniqueContraintFields.Add(DBConstants.sqlSuspFirstNameStr);
            }

            // if we have at least 3 fields (date & time being two of them), that should be sufficient
            if (_UniqueContraintFields.Count >= 3) return;

            // Next, look for officer ID. Date/Time/Officer ID should cover activity records.
            if (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlIssueOfficerIDStr).CompareByName) >= 0)
            {
                _UniqueContraintFields.Add(DBConstants.sqlIssueOfficerIDStr);
            }
            if (Fields.FindIndex(new TObjBasePredicate(DBConstants.sqlIssueOfficerNameStr).CompareByName) >= 0)
            {
                _UniqueContraintFields.Add(DBConstants.sqlIssueOfficerNameStr);
            }

            // if we have at least 3 fields (date & time being two of them), that should be sufficient
            if (_UniqueContraintFields.Count >= 3) return;

            // well, at this point we only have date and time. They are insufficient. So we either allow
            // duplicates, or define the entire row as unique. We'll do the latter.
            _UniqueContraintFields.Clear(); // clear it out, then add them all

            foreach (TTableFldDef loFldDef in _Fields)
            {
                _UniqueContraintFields.Add(loFldDef.Name);
            }
        } // FillUniqueConstraintList

        /// <summary>
        /// PostDeserialize
        /// Opportunity to perform any intialization required immediately after the object
        /// has been deserialized.
        /// Also responsible for invoking PostDeserialize on any child TObjBase instances.
        /// </summary>
        /// <param name="iParent"></param>
        /// <returns></returns>
        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            if (((TTableDef)Parent).Name == "")
                ((TTableDef)Parent).Name = Name; // parent should have same name as us.

            _Fields.PostDeserializeListItems(iParent);
            _Indexes.PostDeserializeListItems(iParent);


#if !WindowsCE && !__ANDROID__   
            if (TClientDef.SkipHostSideOnlyCode == false)
                FillUniqueConstraintList();
#endif

            // add some host-only indexes

            return 0;
        }

        /// <summary>
        /// As a container object, must call each contained object's ResolveRegistryItems
        /// </summary>
        public override void ResolveRegistryItems(ITTRegistry iRegistry)
        {
            base.ResolveRegistryItems(iRegistry);
            _Fields.ResolveRegistryItemsForListItems(iRegistry);
            _Indexes.ResolveRegistryItemsForListItems(iRegistry);
        }

        /// <summary>
        /// Used to have easy way to get the structures main (any only) table.
        /// </summary>
    }


    /// <summary>
    /// Summary description for TMasterTableDef.
    /// </summary>
    public class TMasterTableDef : TTableDef
    {
        #region Properties and Members
        #endregion

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TMasterTableDef()
            : base()
        {
        }
    }

    /// <summary>
    /// TReadTableDef: a read-only table
    /// </summary>
    public class TReadTableDef : TTableDef
    {
        #region Properties and Members
        #endregion

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);
            return 0;
        }

        public TReadTableDef()
            : base()
        {
        }
    }
}

