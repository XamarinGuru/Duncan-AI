

using System.Collections.Generic;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Reino.ClientConfig;
using System.Data;
using Duncan.AI.Droid.Common;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    public class TER_HotsheetFilter : TER_BaseHotSheet
    {
        #region Properties and Members
        public event DoHotSheetFilter OnDoHotSheetFilter = null;

        public bool fPreemptedByWireless = false;
        #endregion

        public TER_HotsheetFilter()
            : base()
        {
        }

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;

            if (OnDoHotSheetFilter != null)
            {
                OnDoHotSheetFilter(this);
            }
            return false;
        }

        /// <summary>
        /// Called when a wireless search result is returned.
        /// iParam is a TTTable holding the result set.
        /// </summary>
        public void FinishEnforceRestriction(SearchParameterPacket iSearchResult)
        //public void FinishEnforceRestriction(TTTable iParam)
        {
            // Nothing to do if no result set returned.
            if (iSearchResult == null)
            {
                return;
            }

            if (iSearchResult.fSearchResultDTOList.Count == 0)
            {
                return;
            }



#if _original_
            TTTable loTable = iParam;
            TTTable loParentTable;

            // Nothing to do if no result set returned.
            if (loTable == null)
                return;
            //todo - finish
            //// Make sure we have an associated table and it is the same as the search table
            //loParentTable = _Parent.ListSourceTable;
            //if (loParentTable == null)
            //    return;

            //// Just in case the wireless result returned while the local result
            //// was being processed we need to tell EnforceRestriction to not use the local result,
            //// our wireless results supercede them.
            //fPreemptedByWireless = true;

            //// When we receive a wireless result, we can't just use it as a filter because
            //// the result set is a separate table w/o filters.  Instead, we must swap tables
            //_Parent.ListSourceTable = loTable;
            //// This is already done for us by setting ListSourceTable property
            ///*
            //_Parent.ListItemCount = loTable.GetRecCount();
            //_Parent.ListItemCache.Clear();
            //*/

            //// If there is a listbox, make sure it gets updated
            //if (_Parent.ListBox != null)
            //    _Parent.ListBox.RefreshItems(true);
            //_Parent.SetEditBufferAndPaint(ReinoControls.TextBoxBehavior.GetFieldDataFromTable(
            //    loTable, _Parent._CfgCtrl._Name,
            //    loTable.GetCurRecNo(), _Parent.GetEditMask()), true); // Must trigger change events!
            //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            return;
#endif
        }

    }
}