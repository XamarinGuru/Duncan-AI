// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 9/11/06 10:53a $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Common/ClientConfig/Lists.cs $
//              Revision: $Revision: 8 $

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Reino.ClientConfig
{
	/// <summary>
	/// Summary description for TListMgr.
	/// </summary>
	
	public class TListMgr : Reino.ClientConfig.TObjBase
	{
		#region Properties and Members
        protected ListObjBase<TAgList> _AgLists;
		/// <summary>
		/// A collection of TAgList objects
		/// </summary>
        public ListObjBase<TAgList> AgLists
		{
			get
			{
				return _AgLists;
			}
 
			set
			{
				_AgLists = value;
			}
		}
		#endregion
			

		public TListMgr()
			: base()
		{
            this._AgLists = new ListObjBase<TAgList>();
		}

        public TAgList this[ String iAgListName ]
        { get { return _AgLists.Find(new TObjBasePredicate(iAgListName).CompareByName); } }

        public TTableDef FindTableByName(string iFindName)
        {
            TObjBasePredicate loPredicate = new TObjBasePredicate(iFindName);
            TTableDef loTableDef;
            foreach (TAgList loAgList in _AgLists)
            {
                if ( (loTableDef = loAgList.FindByName(iFindName)) != null)
                    return loTableDef;
            }
            // fell out w/o finding anything, return null
            return null;
        }

        public TTableDefRev FindTableThatContainsField(string iFieldName)
        {
            // Go thru all the tables that exist and check to see which one 
            // contains the field name that was passed to us.
            TObjBasePredicate predicate = new TObjBasePredicate(iFieldName);
            foreach (TAgList oneList in _AgLists)
            {
                foreach (TTableDef oneTable in oneList.TableDefs)
                {
                    // Return this table if it contains the field.
                    if ((oneTable.HighTableRevision != null) && (oneTable.HighTableRevision.Fields.Find(predicate.CompareByName_CaseInsensitive) != null))
                    {
                        return oneTable.HighTableRevision;
                    }
                }
            }

            // Return null if couldn't find it.
            return null;
        }

        /*
        public TTableDefRev GetTableByName( string tableName )
        {
            // Go thru all lists and try to find table with passed name.
            foreach ( TAgList oneList in AgLists )
            {
                foreach ( TTableDef oneTable in oneList.Tables )
                {
                    // Only care about the highest revision.
                    TTableDefRev highTable = oneTable.HighTableRevision;
                    if ( ( highTable != null ) && ( highTable.Name == tableName ) )
                    {
                        return highTable;
                    }
                }
            }
         
            // Return null if couldn't find table.
            return null;
        }
        */
        public override int PostDeserialize(TObjBase iParent)
        {
            foreach (TAgList loAgList in _AgLists)
            {
                loAgList.PostDeserialize(this);
            }
            // Now call the base method
            return base.PostDeserialize(iParent);
        }

        public override void ResolveRegistryItems(ITTRegistry iRegistry)
        {
            foreach (TAgList loAgList in _AgLists)
            {
                loAgList.ResolveRegistryItems(iRegistry);
            }
            // Now call the base method
            base.ResolveRegistryItems(iRegistry);
        }
    }

	/// <summary>
	/// Summary description for TAgList.
	/// </summary>

    public class TAgList : Reino.ClientConfig.TTableListMgr
	{
		#region Properties and Members
		#endregion

		public TAgList()
			: base()
		{
		}
        public override int PostDeserialize(TObjBase iParent)
        {
            base.PostDeserialize(this);
            return 0;
        }
    }
}
