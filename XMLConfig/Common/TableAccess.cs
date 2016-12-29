#define DEBUG

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;
// KLC using AutoISSUE;

namespace Reino.ClientConfig
{
    //#define MAX_FILTER_FIELD_LEN 60
    public class TFilter   // was a struct, declared as class for effecient usage collection
    {
        public int fFieldNo;
        public String fFilterValue;

        public TFilter()
            : base()
        {
        }
    }

    // a find predicate class for TFilter
    public class TFilterFindPredicate
    {
        private string fCompareValueStr;
        private int fCompareValueInt;

        public TFilterFindPredicate(string iCompareValueStr)
        {
            fCompareValueStr = iCompareValueStr;
        }

        public TFilterFindPredicate(int iCompareValueInt)
        {
            fCompareValueInt = iCompareValueInt;
        }

        public bool CompareByFieldNo(TFilter iFilterObj)
        {
            return iFilterObj.fFieldNo == fCompareValueInt;
        }

        public bool CompareByFiterValue(TFilter iFilterObj)
        {
            return iFilterObj.fFilterValue.Equals(fCompareValueStr);
        }

        public bool CompareByFilterValuePartial(TFilter iFilterObj)
        {
            return iFilterObj.fFilterValue.StartsWith(fCompareValueStr);
        }
    }



    public class TFilterList
    {
        protected List<TFilter> _Filters;
        public List<TFilter> Filters
        {
            get { return _Filters; }
            set { _Filters = value; }
        }

        protected List<int> _FilterRecNos;
        public List<int> fFilterRecNos
        {
            get { return _FilterRecNos; }
            set { _FilterRecNos = value; }
        }

        public int TableRecCount;
        public int fFilterKeyCnt;
        public bool fFiltersActive;
        public bool fFiltersSuspended;
        public bool fUsePrimaryKey;
        public bool fUseSecondaryKey;
        public int fSecondaryKeyNdx;

        public TFilterList()
            : base()
        {
            _Filters = new List<TFilter>();
            _FilterRecNos = new List<int>();
        }

        ~TFilterList()
        {
            //  delete fFilterRecNos;
        }

        public TFilter GetItem(int iItemNo)
        {
            return Filters[iItemNo];
        }

        public TFilter FindItem(int iItemNo)
        {
            TFilterFindPredicate loPredicate = new TFilterFindPredicate(iItemNo);
            return Filters.Find(loPredicate.CompareByFieldNo);
            //(TFilter *)TStructList::GetItem(FindItemNdx((int)&iItemNo)); 
        }

        //public TFilterList( const TFilterList &rList );
        //public TFilterList & operator=(const TFilterList &rList);
        //int operator==(TFilterList &rList);
    }


    public class TTTable : TObjBase, ICloneable
    {
        public enum TSearchType
        {
            SearchType_FirstField = 0,
            SearchType_PrimaryKey,
            SearchType_SecondaryKey,
            SearchType_TableScan
        }

        protected TFilterList fFilterList;  // was *
        protected TTableIndex fCurTableIndex; // was *
        protected int fCurRecordNo;
        protected int fChangeNo;
        protected int fCurFilterRecNo;
        protected int fCurIndexRecNo;

        protected String fTmpFieldBuf;

        protected List<string> _ComparisonFieldValues;
        public List<string> fComparisonFieldValues
        {
            get { return _ComparisonFieldValues; }
            set { _ComparisonFieldValues = value; }
        }

        protected List<string> _FieldValues;
        public List<string> fFieldValues
        {
            get { return _FieldValues; }
            set { _FieldValues = value; }
        }

        public TTableDef fTableDef;

        // JLA 2009.07.09 - Manual entry support (Begin)
//KLC        [XmlIgnore]
//KLC        public IDataSupplier DataSupplier = null;
        // JLA 2009.07.09 - Manual entry support (End)

        /// <summary>
        /// Returns a TTTable clone suitable for use in data formatting, particularly
        /// with SetFormmattedFieldData and GetFormmattedField data. NOTE: this returned
        /// TTTable clone still links to the "original's" TTableDef, so it is NOT suitable
        /// for use for actual record reading or writing to .DAT files, as thread conflicts may
        /// arise when accessing the associated (and uncloned) TTableDef
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            // start with a shallow copy
            TTTable iTableClone = (TTTable)MemberwiseClone();

            // but we'll need some of our own objects
            iTableClone.fFieldValues = new List<string>();
            iTableClone.fFilterList = new TFilterList();
            iTableClone.fComparisonFieldValues = new List<string>();
            iTableClone.fTmpFieldBuf = "";

            // return the new TTTable
            return iTableClone;
        }

        protected void SetCurRecordNo(int iCurRecordNo)
        {
            fCurRecordNo = iCurRecordNo;
            fChangeNo++;
        }

        protected void ClearRecBuf()
        {
            SetCurRecordNo(-1);
            ClearFieldValues();
        }

        /// <summary>
        /// TTTable::AnalyzeFilters
        /// Shuffles filter order so that filter fields are in storage order.  Will also set
        /// fFilterKeyCnt to the number of fields in the filter that are initial fields in the
        /// primary key.
        /// </summary>
        protected void AnalyzeFilters()
        {
            int loFldNdx;
            int loIndexNdx;
            TFilter loFilter;
            TTableIndex loTableIndex;

            // fFilterList is already sorted. We just need to count the primary key fields
            fFilterList.fFilterKeyCnt = 0;
            fFilterList.fUsePrimaryKey = false;
            fFilterList.fUseSecondaryKey = false;
            fFilterList.fSecondaryKeyNdx = -1;

#if PATROL_CAR_AIR
            // in patrol car, keys are not supported. Secondary keys are not constructed, and index points are not constructed (necessary for
            // a primary key search).
            return;
#endif

            for (loFldNdx = 0; (loFldNdx < fTableDef.HighTableRevision.PrimaryKeyFldCnt/*fTableDef.fPrimaryKeyFldCnt*/); loFldNdx++)
            {
                // find this field in the primary key list?
                if (fFilterList.FindItem(loFldNdx) == null)
                    break;
                // ok then, use it 
                fFilterList.fFilterKeyCnt++;
            }

            if (fFilterList.fFilterKeyCnt > 0)
            { // have a primary key
                fFilterList.fUsePrimaryKey = true;
                return;
            }

            // made it here, filters are not on a primary key.  See if they are on a secondary key.
            for (loIndexNdx = 0; loIndexNdx < fTableDef.GetTableIndexCnt(); loIndexNdx++)
            {
                loTableIndex = fTableDef.GetIndex(loIndexNdx);
                for (loFldNdx = 0; (loFldNdx < loTableIndex.GetIndexFieldCnt()); loFldNdx++)
                {
                    /* This used to be "GetItem". However, the filters are not sorted by field number, so
                    use "FindItem"
                    loFilter = fFilterList->GetItem( loFldNdx );
                    if (loFilter->fFieldNo != loFldNdx)
                    break;*/

                    //2005.10.05 ajw -set local var to do secondary check... if (!fFilterList->FindItem( loTableIndex->GetIndexFieldNo( loFldNdx) ) ) break;

                    // 2005.10.05 ajw - is there a filter on this secondary index field?
                    loFilter = fFilterList.FindItem(loTableIndex.GetIndexFieldNo(loFldNdx));
                    // no secondary key, move along
                    if (loFilter == null)
                    {
                        break;
                    }

                    // 2005.10.05 ajw - one more check - if the filter value is empty, we can't use it as a secondary key
                    if (loFilter.fFilterValue.Length == 0)
                    {
                        break;
                    }

                    // ok then, we can use this key
                    fFilterList.fFilterKeyCnt++;
                }

                if (fFilterList.fFilterKeyCnt > 0)
                {    // have a secondary key
                    fFilterList.fUseSecondaryKey = true;
                    fFilterList.fSecondaryKeyNdx = loIndexNdx;
                    return;
                }
            } // for (loIndexNdx...), loop through all defined secondary keys
        }

        /// <summary>
        /// Counterpart to RawFilterSearch that searches on a secondary index.
        /// </summary>
        /// <param name="iMatchFldName"></param>
        /// <param name="iMatchValue"></param>
        /// <param name="iMatchMask"></param>
        /// <param name="iPartialMatch"></param>
        /// <returns></returns>
        protected int RawIndexSearch(String iMatchFldName, String iMatchValue, String iMatchMask, bool iPartialMatch)
        {
            return -1; // never implemented in C++
        }


        /// <summary>
        ///  TTTable::RawFilterSearch
        ///  Performs a search on the entire table for the 1st record that passes the existing 
        ///  filters (if any) as well as the additional criterion field.
        ///  
        /// Does not use fFilterFieldNos, the list of record numbers in the 
        /// table that satisfy the filter.  As such, it is suitable for 
        /// building fFilterFieldNos.
        /// </summary>
        /// <param name="iMatchFldName"></param>
        /// <param name="iMatchValue"></param>
        /// <param name="iMatchMask"></param>
        /// <param name="iPartialMatch"></param>
        /// <returns></returns>
/* KLC
        protected int RawFilterSearch(String iMatchFldName, String iMatchValue, String iMatchMask, bool iPartialMatch)
        {
            StringBuilder loMatchBuf = new StringBuilder();
            StringBuilder loMatchFieldNameList = new StringBuilder();
            int loMatchFldNdx = 0;
            int loResult = 0;
            int loClosestNdx = 0;
            int loRecNo = -1;
            bool loMatchFldIsKey = false;
            TSearchType loSearchType;
            bool loMatchesSoFar;

            if (fTableDef == null)
            {
                //ShowMessage( GetName(), "FilterSearch accessed NULL fTableDef" );
                return -1;
            }

            // Sometimes we might not have a filehandle (This happens on read-only tables where the .DAT file doesn't exist)
            if (fTableDef.fFileHandle == null)
                return -1;

            switch (loSearchType = BuildSearchParams(iMatchFldName, iMatchValue, iMatchMask, iPartialMatch,
                                 ref loMatchBuf, ref loMatchFldIsKey, ref loMatchFldNdx, ref loMatchFieldNameList))
            {
                case TSearchType.SearchType_FirstField:
                    {
                        return FirstFieldSearch(ref iMatchValue, iPartialMatch);
                    }

                case TSearchType.SearchType_PrimaryKey:
                    {
                        if ((loRecNo = fTableDef.FindRecord(loMatchBuf.ToString(), loMatchBuf.Length, ref loClosestNdx)) < 0)
                        {
                            //FREE( loMatchBuf );
                            return loRecNo; // didn't find a matching record 
                        }
                        CopyRecBufToFieldValues();
                        //FREE( loMatchBuf );
                        break;
                    }

                case TSearchType.SearchType_SecondaryKey:
                    {
                        fCurIndexRecNo = fCurTableIndex.SearchForRecord(loMatchBuf.ToString(), loMatchBuf.Length, iPartialMatch);
                        //FREE( loMatchBuf );
                        if (fCurIndexRecNo < 0) return fCurIndexRecNo;
                        // translate the index row number to a table row number
                        loRecNo = fCurTableIndex.GetTableRecNo(fCurIndexRecNo);
                        RawReadRecord(loRecNo);
                        break;
                    }

                case TSearchType.SearchType_TableScan:
                    {
                        if ((loResult = RawReadRecord(++loRecNo)) < 0)
                        {
                            return loResult; // didn't find a matching record 
                        }
                        break;
                    }
            }


            // We now have a starting point. The following loop will check to see if the current record
            // matches both the filters and the match value.  If not, the next record will be read in 
            // until either the end of file is found, or the primary key no longer matches
            for (; ; )
            {
#if PATROL_CAR_AIR
                // again, for PATROL_CAR_AIR the table rec count only reflects as far as we've advanced through the file.
                if (loRecNo >= fTableDef.GetRecCount())
                    break;
#else
                if (loSearchType == TSearchType.SearchType_SecondaryKey)
                {
                    if (fCurIndexRecNo >= fTableDef.GetRecCount()) break;
                }
                else
                {
                    if (loRecNo >= fTableDef.GetRecCount()) break;
                }
#endif

                loMatchesSoFar = true;
                if (loMatchFldNdx >= 0)
                {
                    String loMatchTestStr = GetFormattedFieldData(loMatchFldNdx, iMatchMask);

                    if (iPartialMatch)
                    {
                        //if (uHALr_strStartsWith(GetFormattedFieldData( loMatchFldNdx, iMatchMask), iMatchValue, fTableDef.GetMaxRecSize() ) != 0 )
                        if (loMatchTestStr.StartsWith(iMatchValue) == false)
                        {
                            loMatchesSoFar = false;
                            // not this one.  If MatchFld is the primary key, no point 
                            // in continuing 'cuz remaining records are guaranteed not to match 
                            if (loMatchFldIsKey) return -1;
                        }
                    }
                    //else if ( STRCMP(iMatchValue, GetFormattedFieldData( loMatchFldNdx, iMatchMask)) != 0 ) 
                    else if (loMatchTestStr.Equals(iMatchValue) == false)
                    {
                        loMatchesSoFar = false;
                        // not this one.  If MatchFld is the primary key, 
                        // no point in continuing 'cuz remaining records are guaranteed not to match
                        if (loMatchFldIsKey) return -1;
                    }
                }  // if loMatchFldNdx > 0 (compare the match field) 

                if (loMatchesSoFar)
                {
                    // so far, so good. try the filters now.  RecPassesFilter 
                    // will return the index of the 1st failed field.  If the field is 
                    // a primary key, then no point in continuing 
                    if (RecPassesFilter(ref loResult)) return loRecNo; // got it 
                    if (loResult < fFilterList.fFilterKeyCnt) return -1;
                }

                // not this one, try the next record 
                if (loSearchType == TSearchType.SearchType_SecondaryKey)
                    loRecNo = fCurTableIndex.GetTableRecNo(++fCurIndexRecNo);
                else
                    loRecNo++;

                if ((loResult = RawReadRecord(loRecNo)) < 0)
                {
                    return loResult; // didn't find a matching record 
                }

            } // for (;;) (loop through, finding next records until something passes) 

            // fell through w/o finding anything 
            return -1;
        }
*/
        /// <summary>
        /// Finds the next record after the current one that satisfies the filters
        /// </summary>
        /// <returns></returns>
        protected int RawGetNextRec()
        {
            int loResult;

            // from a secondary index?
            for (; ; )
            {
                if (fCurTableIndex != null)
                {
                    if (fCurIndexRecNo >= (fTableDef.GetRecCount() - 1)) break;
                    fCurRecordNo = fCurTableIndex.GetTableRecNo(++fCurIndexRecNo);
                }
                else
                {
                    if (fCurRecordNo >= (fTableDef.GetRecCount() - 1)) break;
                    fCurRecordNo++;
                }

                if ((loResult = RawReadRecord(fCurRecordNo)) < 0)
                {
                    return loResult; // didn't find a matching record 
                }

                // so far, so good. try the filters now.  RecPassesFilter will return the index of the
                // 1st failed field.  If the field is a primary key, then no point in continuing */
                if (RecPassesFilter(ref loResult))
                    return fCurRecordNo; // got it!! 
                // JLA (3/11/07) If table is not sorted, we can't quit yet
                if (fTableDef.HighTableRevision.PrimaryKeyFldCnt == 0)
                {
                    continue;
                }
                if (loResult < fFilterList.fFilterKeyCnt)
                {
                    ClearRecBuf(); // clear out the buffer 
                    return -1;
                }
            }

            // fell through w/o finding anything 
            ClearRecBuf(); // clear out the buffer 
            return -1;
        }

        /// <summary>
        /// TTTable::RawGetPrevRec
        ///
        /// Finds the next record before the current one that satisfies the filters. Returns the record
        /// number (-1 if none found) and fills the buffer with the record (cleared if no record found).
        /// 
        /// </summary>
        /// <returns></returns>
        protected int RawGetPrevRec()
        {
            int loResult;
            for (; fCurRecordNo > 0; )
            {
                // not this one, try the next record 
                if ((loResult = RawReadRecord(fCurRecordNo - 1)) < 0)
                {
                    return loResult; // didn't find a matching record 
                }

                //  so far, so good. try the filters now.  RecPassesFilter will return the index of the
                // 1st failed field.  If the field is a primary key, then no point in continuing 
                if (RecPassesFilter(ref loResult)) return fCurRecordNo; // got it!! 

                if (loResult < fFilterList.fFilterKeyCnt)
                {
                    ClearRecBuf(); // clear out the buffer 
                    return -1;
                }
            }

            // fell through w/o finding anything 
            ClearRecBuf(); // clear out the buffer 
            return -1;
        }

        protected int RawReadRecord(int iRecordNo)
        {
            if (fTableDef == null)
            {
                //Debug.Write( "%s->ReadRecord accessed NULL fTableDef\n", GetName() );
                //Tfgetc(stdin);
                return -1;
            }

            // Sometimes we might not have a filehandle (This happens on read-only tables where the .DAT file doesn't exist)
    /* KLC        if (fTableDef.fFileHandle == null)
                return -1;
                */
            if (fFilterList.fFiltersActive)
            {
                if ((iRecordNo < 0) || (iRecordNo >= fFilterList.fFilterRecNos.Count))
                {
                    ClearRecBuf();
                    fCurFilterRecNo = -1;
                    SetCurRecordNo(-1);
                    return -1;
                }

                // Lets avoid re-reading and stuffing record buffer if we currently have the
                // data for the desired record. (This should speed up Look Up screens such as 
                // the "In Box" for Abandoned Vehicle module used by L.A.)
                int TrueRecordNo = fFilterList.fFilterRecNos[iRecordNo];
                if (TrueRecordNo != fCurRecordNo)
                {
                    fTableDef.ReadRecordToBuf(fFilterList.fFilterRecNos[iRecordNo]);
                    CopyRecBufToFieldValues();
                }
                fCurFilterRecNo = iRecordNo;
                fCurRecordNo = fFilterList.fFilterRecNos[iRecordNo];
                return fCurFilterRecNo;
            }
            else
            {
#if !PATROL_CAR_AIR
                // patrol car air, GetRecCount() will only reflect as far as we've gone in the file.
                if ((iRecordNo < 0) || (iRecordNo >= fTableDef.GetRecCount()))
                {
                    ClearRecBuf();
                    SetCurRecordNo(-1);
                    return -1;
                }
#endif

                // DEBUG -- For better response, lets not re-read record if there is no change
                // DEBUG -- Something is wrong with the logic because the recordbuf and fieldvalues are not always up-to-date
                /*
                if (fCurRecordNo == iRecordNo)
                    return iRecordNo;
                */

                fTableDef.ReadRecordToBuf(iRecordNo);
#if PATROL_CAR_AIR
                // patrol car air, fTableDef.fFileHandle.fCurRecord should be one past the targeted record because
                // reading in the targeted record advanced the cursor to the next record. 
                if (fTableDef.fFileHandle.CurrentRecNo != iRecordNo + 1)
                {
                    ClearRecBuf();
                    SetCurRecordNo(-1);
                    return -1;
                }
#endif
                CopyRecBufToFieldValues();
                fCurRecordNo = iRecordNo;
                return iRecordNo;
            }
        }

        public void SetTableName(String iTableName)
        {
            // find the associated table definition in global list of table defs. 
            fTableDef = Reino.ClientConfig.TTableListMgr.glTableMgr.GetTableDef(iTableName);

            if (fTableDef == null)
            {
                Debug.WriteLine("Table has no associated definition: %s", iTableName);
                return;
            }

            // add self to associated TableDef's list of tables 
            fTableDef.HighTableRevision.Tables.Add(this);
        }

        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(iParent);

            return 0;
        }

        public TTTable()
            : base()
        {
            // all serializable objects must have an parameterless constructor 

            // create a collection of strings to hold a record 
            this._FieldValues = new List<string>();

            // filters will be sorted ???
            fFilterList = new TFilterList();
            // fFilterList->SetSorted( 1 );

            // create a buffer large enough to hold the largest field 
            //fTmpFieldBuf = (char *) MALLOC( fTableDef->LongestFieldLen() + 1);
            fTmpFieldBuf = "";

            SetCurRecordNo(-1);
            fCurFilterRecNo = -1;
        }

        ~TTTable()
        {
            // remove self from associated TableDef's list of tables 
            if (fTableDef != null)
            {
                if (fTableDef.HighTableRevision.Tables.Contains(this) == true)
                {
                    fTableDef.HighTableRevision.Tables.Remove(this);
                }
            }

            //delete fFieldValues;
            //delete fFilterList;
            //FREE( fTmpFieldBuf );
        }

        /// <summary>
        /// BuildSearchParams
        /// Determines the optimal search type (PrimaryKey, SecondaryKey, full table scan) for the given
        /// search field (iMatchFldName) and the current filters.
        ///
        /// populates oMatchBuffer t with the search values
        /// 
        /// Returns: First field search (1st column only on a primary key table)
        ///          PrimaryKey search
        ///          SecondaryKey search
        ///          Full table scan
        ///         
        /// if ioSearchParamFieldNameList is not NULL, it is populated with 
        /// the field names in the search params
        /// </summary>
        /// <param name="iMatchFldName"></param>
        /// <param name="iMatchValue"></param>
        /// <param name="iMatchMask"></param>
        /// <param name="iPartialMatch"></param>
        /// <param name="oMatchBuf"></param>
        /// <param name="oMatchBufLen"></param>
        /// <param name="oMatchFldIsKey"></param>
        /// <param name="oMatchFldNdx"></param>
        /// <param name="ioSearchParamFieldNameList"></param>
        /// <returns></returns>
        public TSearchType BuildSearchParams(String iMatchFldName, String iMatchValue, String iMatchMask,
                                      bool iPartialMatch, ref StringBuilder oMatchBuf,
                                      ref bool oMatchFldIsKey, ref int oMatchFldNdx,
                                      ref StringBuilder ioMatchFieldNameList)
        {
            int loFldNdx;
            int loTrackNdx; // used to preserve the value of loFldNdx from the for loop
            TTableFldDef loFld;
            int loStrLen;
            TFilter loFilter;

            if (ioMatchFieldNameList != null)
            {
                ioMatchFieldNameList.Length = 0;
            }

            fCurTableIndex = null;
            oMatchFldNdx = fTableDef.GetFldNo(iMatchFldName);
#if PATROL_CAR_AIR
            // mcb PatrolCAR Air app is pc based and also opens and closes files for each access.  Index points are not currently
            // implemented.  Besides, they require recalculation after each file open, so instead we'll just perform a table
            // scan each time.
            return TSearchType.SearchType_TableScan;
#endif

            // short cut.  Do a "FirstFieldSearch" if iMatchFldName is 1st fld, 
            // there are no filters,  and primary key is > 0 
            oMatchFldIsKey = ((oMatchFldNdx == 0) && (fTableDef.HighTableRevision.PrimaryKeyFldCnt/*fTableDef.fPrimaryKeyFldCnt*/ > 0));
            if (oMatchFldIsKey && (!fFilterList.fFiltersActive))
            {
                // keep the field name list up to date, if it was passed
                if (ioMatchFieldNameList != null)
                {
                    ioMatchFieldNameList.Append(iMatchFldName);
                }
                return TSearchType.SearchType_FirstField;
            }

            //  Try to find an index to search on.  1st see if we have any initial, consecutive, primary
            // key fields in the filter to search on.
            if (fFilterList.fUsePrimaryKey)
            {
                // The filter has the primary key, do a search on it 
                //*oMatchBuf = (char *)MALLOC(fTableDef->fMaxRecSize);
                oMatchBuf = new StringBuilder(fTableDef.fMaxRecSize);

                for (loFldNdx = 0; loFldNdx < fFilterList.fFilterKeyCnt; loFldNdx++)
                {
                    // c# wont let us use the loop var after the loop, so we need to capture it
                    loTrackNdx = loFldNdx;
                    loFld = fTableDef.GetField(loFldNdx);
                    loFilter = fFilterList.FindItem(loFldNdx);

                    // this field is in the filter field list, add it to match buffer
                    loStrLen = loFilter.fFilterValue.Length;


                    // keep the field name list up to date, if it was passed
                    if (ioMatchFieldNameList != null)
                    {
                        ioMatchFieldNameList.Append(loFld.Name);
                    }

                    oMatchBuf.Append(loFilter.fFilterValue);

                    // don't place a tab after the last field 
                    if (loFldNdx < (fTableDef.HighTableRevision.Fields.Count - 1))
                    {
                        oMatchBuf.Append("\t");
                        if (ioMatchFieldNameList != null)
                        {
                            ioMatchFieldNameList.Append("\t");
                        }
                    }

                    // c# wont let us use the loop var after the loop, so we need to capture it
                    loTrackNdx = loFldNdx;
                } // for loop to find primary key fields  

                return TSearchType.SearchType_PrimaryKey;
            } // if filters are on the primary key


            if (oMatchFldIsKey == true)
            {
                // the match field is the primary key, do a search on it
                oMatchBuf = new StringBuilder(255);
                oMatchBuf.Append(iMatchValue);

                // don't place a tab after the last field or if a partial match 
                //if (loTrackNdx < (fTableDef.Fields.Count-1) && !iPartialMatch)
                //if (loFldNdx < (fTableDef.Fields.Count-1) && !iPartialMatch)
                {
                    oMatchBuf.Append("\t");
                }

                return TSearchType.SearchType_PrimaryKey;
            }


            // not a primary key, how about a secondary key? 

            // try the filters first
            if (fFilterList.fUseSecondaryKey)
            {
                fCurTableIndex = fTableDef.GetIndex(fFilterList.fSecondaryKeyNdx);

                // The filter has the primary key, do a search on it 
                oMatchBuf = new StringBuilder(fTableDef.fMaxRecSize);

                for (loFldNdx = 0; loFldNdx < fFilterList.fFilterKeyCnt; loFldNdx++)
                {
                    loFld = fTableDef.GetField(fCurTableIndex.GetIndexFieldNo(loFldNdx));
                    loFilter = fFilterList.FindItem(fCurTableIndex.GetIndexFieldNo(loFldNdx));

                    // this field is in the filter field list, add it to match buffer 
                    oMatchBuf.Append(loFilter.fFilterValue);

                    // keep the field name list up to date, if it was passed
                    if (ioMatchFieldNameList != null)
                    {
                        ioMatchFieldNameList.Append(loFld.Name);
                    };


                    // don't place a tab after the last field 
                    if (loFldNdx < (fCurTableIndex.GetIndexFieldCnt() - 1))
                    {
                        oMatchBuf.Append("\t");
                        if (ioMatchFieldNameList != null)
                        {
                            ioMatchFieldNameList.Append("\t");
                        }
                    }
                } // for loop to find primary key fields  

                return TSearchType.SearchType_SecondaryKey;
            } // if filters are on a secondary key

            fCurTableIndex = null;
            // see if this field is the 1st field in any secondary keys
            for (loFldNdx = 0; loFldNdx < fTableDef.GetTableIndexCnt(); loFldNdx++)
            {
                if (fTableDef.GetIndex(loFldNdx).GetIndexFieldNo(0) != oMatchFldNdx)
                {
                    continue;
                }

                fCurTableIndex = fTableDef.GetIndex(loFldNdx);

                // found a secondary key
                oMatchFldIsKey = true;
                oMatchBuf = new StringBuilder(fTableDef.fMaxRecSize);

                oMatchBuf.Append(iMatchValue);

                // keep the field name list up to date, if it was passed
                loFld = fTableDef.GetField(fCurTableIndex.GetIndexFieldNo(loFldNdx));
                if (ioMatchFieldNameList != null)
                {
                    ioMatchFieldNameList.Append(loFld.Name);
                }

                // don't place a tab after the last field or if a partial match 
                if (loFldNdx < (fTableDef.HighTableRevision.Fields.Count - 1) && !iPartialMatch)
                {
                    oMatchBuf.Append("\t");
                    if (ioMatchFieldNameList != null)
                    {
                        ioMatchFieldNameList.Append("\t");
                    };
                }


                return TSearchType.SearchType_SecondaryKey;
            }

            return TSearchType.SearchType_TableScan;
        }

        /// <summary>
        /// Search performed on first field in table only.
        ///
        /// Returns the record number found, -1 if no match.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public int FirstFieldSearch(ref String iMatchValue, bool iPartialMatch)
        {
            StringBuilder loMatchBuf;
            int loResult;
            int loMatchChars = iMatchValue.Length;

            if (fTableDef == null)
            {
                //Debug.Write( "%s->FirstFieldSearch accessed NULL fTableDef\n", GetName());
                //Tfgetc(stdin);
                return -1;
            }

            // construct the match buffer 
            loMatchBuf = new StringBuilder(fTableDef.fMaxRecSize);
            loMatchBuf.Append(iMatchValue);

            // JLA 2008.10.08
            /*
            // need a tab after fields, but not on the last (only) field of a partial match
            if ((fTableDef.HighTableRevision.Fields.Count > 1) && (!iPartialMatch))
            {
                loMatchBuf.Append("\t");
            }
            */

            // do the search
            int loDummyParm = 0;
            loResult = fTableDef.FindRecord(loMatchBuf.ToString(), loMatchBuf.Length, ref loDummyParm);

            CopyRecBufToFieldValues();
            //FREE( loMatchBuf );

            return loResult;
        }

        public int CopyFieldValuesToRecBuf()
        {
            short loNdx;
            //short loFieldLen;
            TTableFldDef loField;

            if (fTableDef == null)
            {
                Debug.WriteLine(this.Name + "->CopyFieldValuesToRecBuf accessed NULL fTableDef");
                return -1;
            }

            // initialize the record buffer 
            fTableDef.ClearRecBuf();

            // copy the field values to the read buffer (but NOT Virtual Fields!)
            bool lo1stFldWritten = false;
            for (loNdx = 0; loNdx < fTableDef.HighTableRevision.Fields.Count; loNdx++)
            {
                // Get next field
                loField = fTableDef.HighTableRevision.Fields[loNdx];
                // Skip this field if its Virtual
                if (loField is TTableVirtualFldDef)
                    continue;
                // First we need to append a tab character if its not the first WRITTEN field
                if (lo1stFldWritten == true)
                    fTableDef.fRecBuffer.Append("\t");
                // Now append the field value. If the field index is invalid, its
                // because there are no more fields with data.
                if ((loNdx >= 0) && (loNdx < fFieldValues.Count))
                    fTableDef.fRecBuffer.Append(fFieldValues[loNdx]);
                // Set flag so we know the first field has been written
                lo1stFldWritten = true;
            }
            return fTableDef.GetRecSize();
        }

        /// <summary>
        /// copies the current value in fTableDef->RecBuffer into our fFieldValues string list.
        /// </summary>
        public void CopyRecBufToFieldValues()
        {
            fFieldValues.Clear();

            // Copy the the read buffer to the field values. Note that this loop will initialize fields
            // to null even if the end of data is encountered prior to reading in all the fields.
            if (fTableDef == null)
            {
                Debug.WriteLine(this.Name + "->CopyRecBufToFieldValues accessed NULL fTableDef");
                return;
            }

            // poof!
            string[] loFieldValuesArray = fTableDef.fRecBuffer.ToString().Split((char)0x09);

            // Get a local variable holding the field count. Should be faster than looking it up in each loop iteration
            int loHiRevFldCnt = fTableDef.HighTableRevision.Fields.Count;
            fFieldValues.Capacity = loHiRevFldCnt;

            // put them into the list
            int loFldIdx = 0;
            ListObjBase<TTableFldDef> HiTblFlds = fTableDef.HighTableRevision.Fields;
            foreach (string loFieldValue in loFieldValuesArray)
            {
                // For each sequential virtual field in the field list, we need to just 
                // add a place holder in the field values
                while ((loFldIdx < loHiRevFldCnt) && (HiTblFlds[loFldIdx].IsVirtual))
                {
                    // Add place holder and increment the field index
                    fFieldValues.Add("");
                    loFldIdx++;
                }

                // Only add the field value if the index is valid
                if ((loFldIdx >= 0) && (loFldIdx < loHiRevFldCnt))
                    fFieldValues.Add(loFieldValue);

                // Increment the field index
                loFldIdx++;
            }

            // Now lets fill place-holders for unaccounted fields
            while (fFieldValues.Count < loHiRevFldCnt)
                fFieldValues.Add("");

            SetCurRecordNo(fTableDef.GetCurRecNo());

            // clear out the rec buffer so it is doesn't get used mistakenly 
            fTableDef.ClearRecBuf();
        }

        public void ForceLinkedFieldRefresh()
        {
            fChangeNo++;
        }

        public void ReAllocateTmpFieldBuf(int iBiggestFieldSize)
        {
            //fTmpFieldBuf = (char *)realloc_IfNecessary( fTmpFieldBuf, iBiggestFieldSize + 1 ); 
        }

        /// <summary>
        ///  Wrapper around RawReadRecord that calls FillVirtualFieldValues in order to populate 
        ///  the virtual table columns.
        /// </summary>
        /// <param name="iRecordNo"></param>
        /// <returns></returns>
        public int ReadRecord(int iRecordNo)
        {
        /*KLC
            // JLA 2009.07.09  - Manual entry support (Begin)
            if (this.DataSupplier != null)
            {
                return this.DataSupplier.ReadRecord(iRecordNo);
            }
            // JLA 2009.07.09 - Manual entry support (End)
            int loStatus;

            if ((loStatus = RawReadRecord(iRecordNo)) < 0)
                return loStatus;
            return loStatus;

         */
         return 0; // KLC
        }


        public int GetPrimaryKey()
        {
        /* KLC
            // JLA 2009.07.09  - Manual entry support (Begin)
            if (this.DataSupplier != null)
            {
                return this.DataSupplier.GetPrimaryKey();
            }
            // JLA 2009.07.09  - Manual entry support (End)
            */
            return fCurRecordNo;
        }


        public int GetChangeNo()
        {
            return fChangeNo;
        }

        public int WriteRecord()
        {
            CopyFieldValuesToRecBuf();
            return fTableDef.WriteRecordBuf();
        }

        public void DeleteLastRecord()
        {
            fTableDef.DeleteLastRecord();
        }

        public void PrintRecord()
        {
            short loNdx;
            TTableFldDef loField;

            if (fTableDef == null)
            {
                Debug.WriteLine(this.Name + "->PrintRecord accessed NULL fTableDef");
                return;
            }

            for (loNdx = 0; loNdx < fTableDef.HighTableRevision.Fields.Count; loNdx++)
            {
                loField = fTableDef.HighTableRevision.Fields[loNdx];
                Console.Write("%d %s: <%s>\n", loNdx, loField.Name, fFieldValues[loNdx]);
            }

        }

        /// <summary>
        /// Checks to see if record currently in fFieldValues matches all the values in
        /// iFilterFldValues.
        ///
        /// Returns: filter field index that caused record to fail (-1 if it passes)
        /// </summary>
        /// <param name="fFailedFilterNdx"></param>
        /// <returns></returns>
        public bool RecPassesFilter(ref int pFailedFilterNdx)
        {
            short loFilterFldNdx;
            TFilter loFilter;
            TTableFldDef loFilterField;
            String loFilterFieldValue = "";

            //if (fFailedFilterNdx != null) 
            {
                pFailedFilterNdx = -1;
            }

            // loop through all filter fields 
            for (loFilterFldNdx = 0; loFilterFldNdx < fFilterList.Filters.Count /* fFilterKeyCnt*/; loFilterFldNdx++) // DEBUG
            {
                // get the index of this filter field in the fields list 
                loFilter = fFilterList.GetItem(loFilterFldNdx);
                if ((loFilterField = fTableDef.GetField(loFilter.fFieldNo)) == null)
                {
                    //if (fFailedFilterNdx != null) 
                    {
                        pFailedFilterNdx = loFilterFldNdx;
                    }
                    return false;
                }

                /*
                Debug.Write( "Filter:%d <%s> to %d <%s>\n", loFilterFldNdx, fFilterFldValues->GetString(loFilterFldNdx),
                    loFldNdx, fFieldValues->GetString( loFldNdx) );
                */

                loFilterField.ConvertFromStoreFormat(this, loFilter.fFieldNo, "", ref loFilterFieldValue);
                if (loFilter.fFilterValue.Equals(loFilterFieldValue) == false)
                {
                    //if (fFailedFilterNdx != null) 
                    {
                        pFailedFilterNdx = loFilterFldNdx;
                    }
                    return false;
                }
            }

            return true; /* found it! */
        }


        public void RemoveFilter(String iFilterFieldName)
        {
        /*
            // JLA 2009.07.09  - Manual entry support (Begin)
            if (this.DataSupplier != null)
            {
                this.DataSupplier.RemoveFilter(iFilterFieldName);
                return;
            }
            // JLA 2009.07.09  - Manual entry support (eND)

            int loFldNdx;

            TObjBasePredicate loFieldPredicate = new TObjBasePredicate(iFilterFieldName);
            int loFieldNo = fTableDef.HighTableRevision.Fields.FindIndex(loFieldPredicate.CompareByName_CaseInsensitive);

            //loFldNdx = fFilterList.FindItemNdx( (int)&loFieldNo );
            TFilterFindPredicate loFilterPredicate = new TFilterFindPredicate(loFieldNo);
            loFldNdx = fFilterList.Filters.FindIndex(loFilterPredicate.CompareByFieldNo);
            if (loFldNdx >= 0)
            {
                fFilterList.Filters.RemoveAt(loFldNdx);
                AnalyzeFilters();
                if (!fFilterList.fFiltersSuspended)
                {
                    ActivateFilters();
                }
            }
            // Debug.Write( "\nRemove %s from %s,cnt=%d", iFilterFieldName, this.Name, fFilterList.fItemCnt );
         */ 
        }

        /// <summary>
        ///  TTable::AddFilter
        ///  Adds a single column filter for FilterSearch and GetNext/PrevRec.  Passed a column name
        ///   and a column value to filter by.
        /// </summary>
        /// <param name="iFilterFldName"></param>
        /// <param name="?"></param>
        /// <returns>Returns -1 if no search was necessary due to unchanged parameters</returns>
        public int AddFilter(String iFilterFldName, String iFilterFldValue, String iFilterFldMask)
        {
        /*
            // JLA 2009.07.09  - Manual entry support (Begin)
            if (this.DataSupplier != null)
            {
                return this.DataSupplier.AddFilter(iFilterFldName, iFilterFldValue, iFilterFldMask);
            }
            // JLA 2009.07.09  - Manual entry support (eND)

            int loFldNdx;
            int loFilterFldNdx;
            TTableFldDef loFld;
            TFilter loFilter;

            if (
                    (iFilterFldName == null) ||
                    (iFilterFldValue == null) ||
                    ((loFldNdx = fTableDef.GetFldNo(iFilterFldName)) < 0)
                )
            {
                return 0;
            }

            loFld = fTableDef.GetField(loFldNdx);

            // is filter already defined? 
            //if ( (loFilterFldNdx = fFilterList->FindItemNdx( (int) &loFldNdx )) >= 0)
            TFilterFindPredicate loFilterPredicate = new TFilterFindPredicate(loFldNdx);
            if ((loFilterFldNdx = fFilterList.Filters.FindIndex(loFilterPredicate.CompareByFieldNo)) >= 0)
            {
                String loNewFilterValue = "";

                //yep, only have to change the filter field value 
                TFilter loPFilter = fFilterList.Filters[loFilterFldNdx];
                loFld.ConvertToStoreFormat(iFilterFldValue, iFilterFldMask, ref loNewFilterValue);

                // is the new filter same as the old and is the table size the same as when the filter was first created?
                if (
                        (loNewFilterValue.Equals(loPFilter.fFilterValue) == true) &&
                        (fTableDef.GetRecCount() == fFilterList.TableRecCount)
                    )
                {
                    // Return -1 so caller knows we didn't have to spend time searching because nothing has changed
                    return -1;
                }

                // Update the filter field value
                loPFilter.fFilterValue = loNewFilterValue;

                // capture the record count
                fFilterList.TableRecCount = fTableDef.GetRecCount();

                // re-activate the filter 
                if (!fFilterList.fFiltersSuspended)
                {
                    ActivateFilters();
                }

                return 0; // all done. 
            }

            // filter doesn't already exist, so we have to add it. 
            loFilter = new TFilter();
            loFilter.fFieldNo = loFldNdx;
            loFilter.fFilterValue = "";
            loFld.ConvertToStoreFormat(iFilterFldValue, iFilterFldMask, ref loFilter.fFilterValue);
            fFilterList.Filters.Add(loFilter);

            AnalyzeFilters();

            fFilterList.TableRecCount = fTableDef.GetRecCount();
            if (!fFilterList.fFiltersSuspended)
            {
                ActivateFilters();
            }
            */
            return 0;
        }


        /// <summary>
        ///  TTTable::RemoveAllFilters
        /// 
        /// Removes all filters from table.
        /// </summary>
        public void RemoveAllFilters()
        {
            // clear out the filters 
            fFilterList.Filters.Clear();
            // re-do filter stats based on empty filter list 
            AnalyzeFilters();
            //ActivateFilters will short-circuit out, leaving FilterRecNos blank and filters de-activated. 
            ActivateFilters();
        }

        /// <summary>
        /// TTTable::FilterSearch
        /// Perform a search on the passed field name and value. If iPartialMatch is true, will
        /// allow a substring match.  Uses the filters set previously by SetFilter.
        /// 
        /// If iMatchFldName is null, performs the search exclusively on the filter.
        /// 
        /// Returns the record number found.  The buffer will contain this 
        /// record (or will be empty if no record was found).
        /// </summary>
        /// <param name="iMatchFldName"></param>
        /// <param name="iMatchValue"></param>
        /// <param name="iMatchMask"></param>
        /// <param name="iPartialMatch"></param>
        /// <returns></returns>
        /// 
        public int FilterSearch(String iMatchFldName, String iMatchValue, String iMatchMask, bool iPartialMatch)
        {
        /*
            // JLA 2009.07.09  - Manual entry support (Begin)
            if (this.DataSupplier != null)
            {
                return this.DataSupplier.FilterSearch(iMatchFldName, iMatchValue, iMatchMask, iPartialMatch);
            }
            // JLA 2009.07.09  - Manual entry support (End)

            int loMatchFldNdx;
            int loStatus;
            int loMatchFldLen = 0;
            String loMaskMatchValue = "";
            String loFieldData = "";
            TTableFldDef loMatchFld;

            if (fFilterList.fFilterRecNos == null)
            {
                return -1;
            }

            loMatchFldNdx = fTableDef.GetFldNo(iMatchFldName);

            if ((loMatchFld = fTableDef.GetField(loMatchFldNdx)) != null)
                loMatchFld.ConvertToStoreFormat(iMatchValue, iMatchMask, ref loMaskMatchValue);
            else
                loMaskMatchValue = iMatchValue;


            // if no filters, do a straight up search 
            if (!fFilterList.fFiltersActive)
            {
                // DEBUG -- Lets try to speed things up for most common search
                if ((iPartialMatch == false) && (this.GetCurRecNo() >= 0))
                {
                    // Get current field value and see if it matches the desired value
                    string CurrRecFldData = GetFormattedFieldData(loMatchFldNdx, iMatchMask);
                    if (CurrRecFldData == loMaskMatchValue)
                        return this.GetCurRecNo();
                }
                loStatus = RawFilterSearch(iMatchFldName, loMaskMatchValue, iMatchMask, iPartialMatch);
                return loStatus;
            }

            // there are filters, which means we can perform our search using the 
            // reduced data set in FilterFieldNos 

            // if no match fields, return 1st record in filtered data set 
            if ((iMatchFldName == null) || (iMatchFldName.Length == 0))
            {
                ReadRecord(0);
                return fCurFilterRecNo;
            }

            if (loMatchFldNdx < 0)
            {
                ReadRecord(-1);
                return fCurFilterRecNo;
            }

            // The order of the records in the filter will be the order 
            // they appear in the table unless the filter used a 
            // secondary key.  IF it is in the order of the table AND the table 
            // is primary keyed AND the match field is part of
            // the primary key AND the match field and the filter fields
            // contain all the primary key fields up to the match field
            // THEN we can perform a binary search.
            //
            // For example: 
            //  - PrimaryKeyFldCnt = 1, MatchFieldNdx = 0, Filter not on secondary key BINARY SEARCH OK.
            //  - PrimaryKeyFldCnt = 3, filter on fields 0 & 1, MatchFieldNdx = 2, BINARY SEARCH OK 

            if (
                (!fFilterList.fUseSecondaryKey) &&
                (fTableDef.HighTableRevision.PrimaryKeyFldCnt/*fTableDef.fPrimaryKeyFldCnt* / > loMatchFldNdx) &&
                (
                  (loMatchFldNdx == 0) || (loMatchFldNdx == fFilterList.fFilterKeyCnt)
                )
               )
            {   // perform binary search
                // Reset initial filter record number 
                fCurFilterRecNo = -1;

                int loLowNdx = 0;
                int loHighNdx = fFilterList.fFilterRecNos.Count;
                int loMidNdx = loHighNdx >> 1;
                int loComparison = 1;
                if (!iPartialMatch) loMatchFldLen++; // include the terminating 0 if not a partial match
                for (; ; )
                {
                    ReadRecord(loMidNdx);  // sets fCurFilterRecNo
                    if (fCurFilterRecNo < 0) break;

                    // Do the comparison.
                    // We're matching on formatted data, so use passed value instead of 
                    // data converted to store format.
                    loMaskMatchValue = iMatchValue;
                    string loCompareValue = GetFormattedFieldData(loMatchFldNdx, iMatchMask);
                    if (iPartialMatch)
                    {
                        loCompareValue = ReinoControls.TextBoxBehavior.SafeSubString(loCompareValue, 0, loMaskMatchValue.Length);
                    }
                    loComparison = loMaskMatchValue.CompareTo(loCompareValue);

                    // What's the result?
                    // If this was a match, continue looking for matches before this one 
                    // so that we end up w/ the 1st match in order 

                    // No match, and no-where else to search.
                    if (loLowNdx == loHighNdx) break;

                    if (loComparison > 0) // record value is too low, look higher
                    {
                        if (loMidNdx == loHighNdx) break; // can't go any higher
                        loLowNdx = loMidNdx;
                        loMidNdx = (loLowNdx + loHighNdx) >> 1;
                        if (loMidNdx == fCurFilterRecNo)
                        {
                            loMidNdx++;
                            loLowNdx++;
                        }
                        continue;
                    }

                    // what's left is record value too high or matches, look 
                    // lower until we find the lowest possible match
                    if (loLowNdx == loMidNdx) break;  // can't go any lower
                    loHighNdx = loMidNdx;
                    loMidNdx = (loLowNdx + loHighNdx) >> 1;
                    if (loMidNdx == fCurFilterRecNo) loMidNdx++;
                    continue;
                } // for... (binary search part of loop)

                if (loComparison != 0)
                { // didn't find a record!
                    ReadRecord(-1);
                    return fCurFilterRecNo;
                }

                return fCurFilterRecNo;
            } // perform binary search
            else
            { // couldn't do a binary search, so do a linear search

                // JLA (5/22/07) Speed Optimization: Before doing a linear search from the first record,
                //   we will first check the current record. If its already a match, then no need to look
                //   any further. This should significantly speed up Lookup screens or Abandonded "In-Box" screens.
                if ((fCurFilterRecNo != -1) && (fCurRecordNo != -1))
                {
                    // get the data from the field
                    GetFormattedFieldData(loMatchFldNdx, iMatchMask, ref loFieldData);
                    // We're matching on formatted data, so use passed value instead of 
                    // data converted to store format.
                    loMaskMatchValue = iMatchValue;

                    if (iPartialMatch)
                    {
                        if (loMaskMatchValue.Equals(loFieldData.Substring(0, loMatchFldLen)))
                            return fCurFilterRecNo; // found it 
                    }
                    else
                    {
                        if (loMaskMatchValue.Equals(loFieldData))
                            return fCurFilterRecNo;
                    }
                }

                // Reset initial filter record number, then do linear search
                fCurFilterRecNo = -1;
                while (GetNextRec() >= 0)
                {
                    // get the data from the field
                    GetFormattedFieldData(loMatchFldNdx, iMatchMask, ref loFieldData);
                    // We're matching on formatted data, so use passed value instead of 
                    // data converted to store format.
                    loMaskMatchValue = iMatchValue;

                    if (iPartialMatch)
                    {
                        if (loMaskMatchValue.Equals(loFieldData.Substring(0, loMatchFldLen)))
                            return fCurFilterRecNo; // found it 
                    }
                    else
                    {
                        if (loMaskMatchValue.Equals(loFieldData))
                            return fCurFilterRecNo;
                    }
                } // while GetNextRec 
            } // else perform linear search
            */
            return fCurFilterRecNo;
        }

        /// <summary>
        /// * TTTable::GetNextRec
        ///
        /// Finds the next record after the current one that satisfies the filters. Returns the record
        /// number (-1 if none found) and fills the buffer with the record (cleared if no record found).
        /// </summary>
        /// <returns></returns>
        public int GetNextRec()
        {
            if (fFilterList.fFiltersActive)
            {   // pull a filtered record 
                if (fCurFilterRecNo < fFilterList.fFilterRecNos.Count)
                {
                    ReadRecord(fCurFilterRecNo + 1);
                }
                else
                {
                    fCurFilterRecNo = -1;
                    SetCurRecordNo(-1);
                    ClearRecBuf();
                }
                return fCurFilterRecNo;
            }
            else // pull the prev record 
            {
                if (fCurRecordNo < fTableDef.GetRecCount())
                {
                    ReadRecord(fCurRecordNo + 1);
                }
                else
                {
                    ClearRecBuf();
                    SetCurRecordNo(-1);
                }
                return fCurRecordNo;
            }
        }

        /// <summary>
        /// TTTable::GetPrevRec
        /// 
        /// Finds the next record before the current one that satisfies the filters. Returns the record
        /// number (-1 if none found) and fills the buffer with the record (cleared if no record found).
        /// </summary>
        /// <returns></returns>
        public int GetPrevRec()
        {
            if (fFilterList.fFiltersActive)
            {
                // pull a filtered record 
                if ((fCurFilterRecNo > 0) && (fFilterList.fFilterRecNos.Count > 0))
                {
                    ReadRecord(fCurFilterRecNo - 1);
                }
                else
                {
                    fCurFilterRecNo = -1;
                    SetCurRecordNo(-1);
                    ClearRecBuf();
                }
                return fCurFilterRecNo;
            }
            else // pull the prev record 
            {
                if (fCurRecordNo > 0)
                {
                    ReadRecord(fCurRecordNo - 1);
                }
                else
                {
                    ClearRecBuf();
                    SetCurRecordNo(-1);
                }
                return fCurRecordNo;
            }
        }

        public void PrintFilter()
        {
            TTableFldDef loField;
            Debug.WriteLine(this.Name + " filters; Key Count:" + fFilterList.fFilterKeyCnt);
            foreach (TFilter loFilter in fFilterList.Filters)
            {
                loField = fTableDef.GetField(loFilter.fFieldNo);
                Console.Write("%s = <%s>\n", loField.Name, loFilter.fFilterValue);
            }
        }

        public void GetFormattedFieldData(int iFieldNo, String iDataMask, ref String oResult)
        {
     
            //// JLA 2009.07.09  - Manual entry support (Begin)
            //if (this.DataSupplier != null)
            //{
            //    this.DataSupplier.GetFormattedFieldData(iFieldNo, iDataMask, ref oResult);
            //    return;
            //}
            //// JLA 2009.07.09  - Manual entry support (End)

            TTableFldDef loField;
            oResult = "";

            // If the field number is invalid, then we will simply return with empty string
            if (iFieldNo == -1)
                return;

            if ((loField = fTableDef.GetField(iFieldNo)) != null)
                loField.ConvertFromStoreFormat(this, iFieldNo, iDataMask, ref oResult);
         
        }

        /// <summary>
        ///  GetFormattedFieldData
        /// 
        /// Overloaded version.  This one uses fTmpFieldBuf as the data buffer so that the caller
        /// doesn't have to provide a buffer.
        /// </summary>
        public String GetFormattedFieldData(int iFieldNo, String iDataMask)
        {
        /*KLC
            // JLA 2009.07.09  - Manual entry support (Begin)
            if (this.DataSupplier != null)
            {
                return this.DataSupplier.GetFormattedFieldData(iFieldNo, iDataMask);
            }
            // JLA 2009.07.09  - Manual entry support (End)
*/
            GetFormattedFieldData(iFieldNo, iDataMask, ref fTmpFieldBuf);
         
            return fTmpFieldBuf;
        }


        public void GetFormattedFieldData(String iFieldName, String iDataMask, ref String oResult)
        {
        /*KLC
            // JLA 2009.07.09  - Manual entry support (Begin)
            if (this.DataSupplier != null)
            {
                this.DataSupplier.GetFormattedFieldData(iFieldName, iDataMask, ref oResult);
                return;
            }
            // JLA 2009.07.09  - Manual entry support (End)
           */
            int loFieldNo = fTableDef.GetFldNo(iFieldName);
            GetFormattedFieldData(loFieldNo, iDataMask, ref oResult);
        
        }

        public String GetFormattedFieldData(String iFieldName, String iDataMask)
        {
        /* KLC
            // JLA 2009.07.09  - Manual entry support (Begin)
            if (this.DataSupplier != null)
            {
                return this.DataSupplier.GetFormattedFieldData(iFieldName, iDataMask);
            }
            // JLA 2009.07.09  - Manual entry support (End)
        */
            GetFormattedFieldData(iFieldName, iDataMask, ref fTmpFieldBuf);
        
            return fTmpFieldBuf;
        }

        public void SetFormattedFieldData(int iFieldNo, String iDataMask, String iData)
        {
            TTableFldDef loFld = fTableDef.GetField(iFieldNo);

            //  make sure we have enough items in fFieldValues 
            while (fFieldValues.Count <= (iFieldNo))
                fFieldValues.Add("");

            if (loFld != null)
                loFld.ConvertToStoreFormat(iData, iDataMask, ref fTmpFieldBuf);
            else
                fTmpFieldBuf = "";

            fFieldValues[iFieldNo] = fTmpFieldBuf;
        }

        public void SetFormattedFieldData(String iFieldName, String iDataMask, String iData)
        {
            int loFieldNo;

            TObjBasePredicate loPredicate = new TObjBasePredicate(iFieldName);
            if ((loFieldNo = fTableDef.HighTableRevision.Fields.FindIndex(loPredicate.CompareByName_CaseInsensitive)) < 0)
            {
                return;
            }

            SetFormattedFieldData(loFieldNo, iDataMask, iData);
        }

        public void ClearFieldValues()
        {
            fFieldValues.Clear();
        }

        /// <summary>
        ///  TTTable::ActivateFilters()
        ///  
        /// Routine will gather stats (1st, last, count) of records in table that satisfy the
        /// current filters.  
        /// </summary>
        public void ActivateFilters()
        {
            int loSavedRecNo = fCurRecordNo;
            int loNextMatchRecNo;

            // disable the filter 
            fFilterList.fFiltersActive = false;
            fFilterList.fFiltersSuspended = false;

            fCurFilterRecNo = -1;

            fFilterList.fFilterRecNos.Clear();

            if (fFilterList.Filters.Count == 0)
            {
                fFilterList.fFiltersActive = true;
                fCurRecordNo = -1;
                fCurFilterRecNo = -1;
                RawReadRecord(fCurFilterRecNo);
                return; // real easy.  No filters. 
            }

            // a little more difficult.  Have to perform searches.
/*KLC
           for (
                    loNextMatchRecNo = RawFilterSearch(null, null, null, false);
                    loNextMatchRecNo >= 0;
                    loNextMatchRecNo = RawGetNextRec()
                )
            {
                if (loNextMatchRecNo == loSavedRecNo)
                {
                    fCurFilterRecNo = fFilterList.fFilterRecNos.Count;
                }
                fFilterList.fFilterRecNos.Add(loNextMatchRecNo);
            }
   */
 
            // enable the filter 
            fFilterList.fFiltersActive = true;
            if (loSavedRecNo >= 0)
            {
                RawReadRecord(fCurFilterRecNo);
            }

        }

        public void SuspendFilters()
        {
            fFilterList.fFiltersSuspended = true;
            fFilterList.fFiltersActive = false;
        }

        public int GetRecCount()
        {
        /* KLC
            // JLA 2009.07.09  - Manual entry support (Begin)
            if (this.DataSupplier != null)
            {
                return this.DataSupplier.GetRecCount();
            }
            else
            {
                if (fFilterList.fFiltersActive)
                {
                    return fFilterList.fFilterRecNos.Count;
                }
                else
                {
                    return fTableDef.GetRecCount();
                }
            }
            // JLA 2009.07.09  - Manual entry support (End)
    */

            if (fFilterList.fFiltersActive)
            {
                return fFilterList.fFilterRecNos.Count;
            }
            else
            {
                return fTableDef.GetRecCount();
            }

        }

        public int GetCurRecNo()
        {
            if (fFilterList.fFiltersActive)
            {
                return fCurFilterRecNo;
            }
            else
            {
                return fCurRecordNo;
            }
        }

        public void CopyFilters(TFilterList iFromFilters)
        {
            fFilterList = iFromFilters;

            if (fFilterList.fFilterKeyCnt != iFromFilters.fFilterKeyCnt)
            {
                Debug.WriteLine("Failed copy filters");
                return;
            }

            if ((fFilterList.GetItem(0) != null) && (fFilterList.GetItem(0) == iFromFilters.GetItem(0)))
            {
                Debug.WriteLine("Filter copy did not allocate new lists!");
                return;
            }
        }

        public TFilterList GetFilter()
        {
            return fFilterList;
        }

        //public TTStringList *GetFieldValuesPtr(){ return fFieldValues; };
        public List<string> GetFieldValuesPtr()
        {
            return fFieldValues;
        }


    }
}
