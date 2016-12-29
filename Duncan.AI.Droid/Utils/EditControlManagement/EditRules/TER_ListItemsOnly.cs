
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    public class TER_ListItemsOnly :  EditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ListItemsOnly()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnDataChanged = true;
            ActiveOnValidate = true;
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            EditEnumerations.ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) == EditEnumerations.ETrueFalseIgnore.tfiIgnore)
                return false;

             //if a edit control has the listitemsonly restriciton, then we are forcing it to be a spinner, no matter the active on events.
             //If parent has "RelaxListOnlyRestriction" enabled, then there is not need to process this restriction.
             //However we will only allow this to happen if the control is not currently user-editable
             //(Note: This is used exclusively for the benefit of creating new note records)
/*
            if (Parent.RelaxListOnlyRestriction == true)
            {
                if ((Parent.CfgCtrl.IsEnabled == false) ||
                    (Parent.CfgCtrl.IsProtected == true) ||
                    (Parent.CfgCtrl.IsHidden == true))
                {
                    return false;
                }
            }
 */

            if ((loEnforce == EditEnumerations.ETrueFalseIgnore.tfiTrue) &&
                (!(Parent.FieldIsBlank())))
            {

                // this isn't always updated before the restriction is enforced
                //string loText = Parent.GetText();
                string loText = Parent.EditBuffer;

                // this is kludgy, two copies of edit buffers out of sync!
                if (string.IsNullOrEmpty(loText) == true)
                {
                    loText = Parent.GetText(); 
                }


                int loListItemIndex = Helper.GetListItemIndexFromStringList(Parent.GetFilteredListItems(), loText, Helper.ListItemMatchType.searchNoPartialMatch);
                if (loListItemIndex < 0)
                {
                    UpdateLabelColorToShowErrorStatus(true);
                    return true;
                }
                else
                {
                    UpdateLabelColorToShowErrorStatus(false);
                    return false;
                }





                // If fails with current index, let's force a resync and try again
                if (Parent.ListItemIndex < 0)
                {
                    // TODO - not implmentd yet on Android 
                    /*
                    Parent.ResynchListNdx();
                     * */

                    //string loText = Parent.GetText();
                    // Parent.ListItemIndex = Helper.GetListItemIndexFromStringList( Parent.GetFilteredListItems(), loText, Helper.ListItemMatchType.searchNoPartialMatch);

                }

                if (Parent.ListItemIndex < 0)
                {

                    // TODO - not implementd yet on Android 
                    UpdateLabelColorToShowErrorStatus(true);
                    return true;

                    /*

                    // If we're not currently pointed to the "normal" table, lets see if the
                    // item is valid in the "normal" table (Could happen from hotsheet match?)
                    if (Parent.ListSourceTable != Parent.NaturalListSourceTable)
                    {
                        if (Parent.GetForcedNaturalNdx() < 0)
                            return true;
                        else
                            return false;
                    }
                    else
                        return true;
                     */



                }
                else
                {
                    UpdateLabelColorToShowErrorStatus(false);
                    return false;
                }
            }
            else
            {
                UpdateLabelColorToShowErrorStatus(false);
                return false;
            }
        }
        #endregion
    }
}