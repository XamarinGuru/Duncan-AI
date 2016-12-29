using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Common
{
    public class SearchParameterPacket
    {
        public bool fCancelledByUser = false;

        public Reino.ClientConfig.TSearchStruct fSearchStruct;    // the structure that initiated the search 
        public bool fCalledFromSearchAndIssue;                    // differentiates between two types of evaluations
        public short fMinMatchCount;		      
        public Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditDependency fInitiatingEditRestriction; // editrestriction that initiated the wireless search.

        private HashSet<ListFilter> fFiltersForInitiatingRestriction;
        public HashSet<ListFilter> FiltersForInitiatingRestriction
        {
            get { return fFiltersForInitiatingRestriction; }
            set { fFiltersForInitiatingRestriction = value; }
        }


        public AlertDialog _parentSelectionDialog = null;


        public List<SearchMatchDTO> fSearchResultDTOList;

        public DataRow SearchResultSelectedRow;
        public int SearchResultSelectedRowIndex;

        public SearchParameterPacket()
        {
            fSearchResultDTOList = new List<SearchMatchDTO>();
            fFiltersForInitiatingRestriction = new HashSet<ListFilter>();
        }

    }

    public static class SearchStructWirelessQueueAndroid
    {
        private static List<SearchParameterPacket> glReceiveQue = new List<SearchParameterPacket>();


        public static bool CE_APDI_GetReplyFromReceiveQue(ref SearchParameterPacket oWirelessSearchResult)
        {
            // returns the next packet from the que of Received packets
            oWirelessSearchResult = null;
            if ((glReceiveQue == null) || (glReceiveQue.Count <= 0))
            {
                return false;
            }

            lock (glReceiveQue)
            {
                oWirelessSearchResult = glReceiveQue[0];
                glReceiveQue.RemoveAt(0);
            }
            return true;
        }

        public static void CE_APDI_AddReplyToReceiveQue(SearchParameterPacket iWirelessSearchResult)
        {
            lock (glReceiveQue)
            {
                glReceiveQue.Add(iWirelessSearchResult);
            }

        }


        private static void CleanUpInternalGenericQue(List<SearchParameterPacket> iQue)
        {
            // clean up the passed generic que list
            // go backwards for max efficiency, since each delete causes the indexes to 
            // be shuffled down, we don't need to waste time waiting for hat
            for (int loWirelessQueIdx = iQue.Count - 1; loWirelessQueIdx >= 0; loWirelessQueIdx--)
            {
                // derefernce the ptr
                if (iQue[loWirelessQueIdx] != null)
                    iQue.RemoveAt(loWirelessQueIdx);
            }
        }


        public static void CleanUpCE_APDI_Client()
        {
            if (glReceiveQue == null)
            {
                return;
            }

            lock (glReceiveQue)
            {
                CleanUpInternalGenericQue(glReceiveQue);
                glReceiveQue = null;
            }

        }




    }
}