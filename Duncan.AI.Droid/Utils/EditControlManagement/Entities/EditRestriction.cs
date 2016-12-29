using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils.EditControlManagement.EditRules;
using Duncan.AI.Droid.Utils.HelperManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Duncan.AI.Droid.Utils.EditControlManagement.Entities
{
    public class EditRestriction : EditDependency
    {
        #region Properties and Members

        protected bool _ActiveOnAncestorFieldExit = false;
        protected bool _ActiveOnBeforeEdit = false;
        protected bool _ActiveOnCancel = false;
        protected bool _ActiveOnContinuance = true;
        protected bool _ActiveOnCorrection = true;
        protected bool _ActiveOnDataChanged = false;
        protected bool _ActiveOnFirstEditFocus = false;
        protected bool _ActiveOnFormInit = false;
        protected bool _ActiveOnIssueMore = true;
        protected bool _ActiveOnNewEntry = true;
        protected bool _ActiveOnParentDataChanged = false;
        protected bool _ActiveOnParentFieldExit = false;
        protected bool _ActiveOnReissue = true;
        protected bool _ActiveOnValidate = false;
        protected bool _ActiveOnView = false;
        protected List<ConditionEvaluation> _Conditions;

        protected bool _ConditionsANDed = false;
        protected bool _DisabledIfNoChange = false;
        protected bool _Overrideable = false;

        /// <summary>
        ///     A collection of ConditionEvaluation objects
        /// </summary>
        public List<ConditionEvaluation> Conditions { get; set; }
        public string OriginalRestrictionType { get; set; }
           
           [DefaultValue(false)] // This prevents serialization of default values
        public bool ConditionsANDed
        { get; set; }

        [DefaultValue(false)] // This prevents serialization of default values
        public bool ActiveOnFormInit
        { get; set; }

        [DefaultValue(false)] // This prevents serialization of default values
        public bool ActiveOnBeforeEdit
        {
            get { return _ActiveOnBeforeEdit; }
            set { _ActiveOnBeforeEdit = value; }
        }

        // This might have different defaults in descendants, so don't declare default here because 
        // it will screw up serialization/deserialization
        /*[System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values*/

        public bool ActiveOnDataChanged
        {
            get { return _ActiveOnDataChanged; }
            set { _ActiveOnDataChanged = value; }
        }

        // This might have different defaults in descendants, so don't declare default here because 
        // it will screw up serialization/deserialization
        /*[System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values*/

        public bool ActiveOnValidate
        {
            get { return _ActiveOnValidate; }
            set { _ActiveOnValidate = value; }
        }

        [DefaultValue(false)] // This prevents serialization of default values
        public bool ActiveOnParentFieldExit
        {
            get { return _ActiveOnParentFieldExit; }
            set { _ActiveOnParentFieldExit = value; }
        }

        [DefaultValue(false)] // This prevents serialization of default values
        public bool ActiveOnAncestorFieldExit
        {
            get { return _ActiveOnAncestorFieldExit; }
            set { _ActiveOnAncestorFieldExit = value; }
        }

        // This might have different defaults in descendants, so don't declare default here because 
        // it will screw up serialization/deserialization
        /*[System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values*/

        public bool ActiveOnParentDataChanged
        {
            get { return _ActiveOnParentDataChanged; }
            set { _ActiveOnParentDataChanged = value; }
        }

        [DefaultValue(false)] // This prevents serialization of default values
        public bool ActiveOnFirstEditFocus
        {
            get { return _ActiveOnFirstEditFocus; }
            set { _ActiveOnFirstEditFocus = value; }
        }

        [DefaultValue(true)] // This prevents serialization of default values
        public bool ActiveOnNewEntry
        {
            get { return _ActiveOnNewEntry; }
            set { _ActiveOnNewEntry = value; }
        }

        // This might have different defaults in descendants, so don't declare default here because 
        // it will screw up serialization/deserialization
        /*[System.ComponentModel.DefaultValue(true)] // This prevents serialization of default values*/

        public bool ActiveOnCorrection
        {
            get { return _ActiveOnCorrection; }
            set { _ActiveOnCorrection = value; }
        }

        // This might have different defaults in descendants, so don't declare default here because 
        // it will screw up serialization/deserialization
        /*[System.ComponentModel.DefaultValue(true)] // This prevents serialization of default values*/

        public bool ActiveOnIssueMore
        {
            get { return _ActiveOnIssueMore; }
            set { _ActiveOnIssueMore = value; }
        }

        [DefaultValue(false)] // This prevents serialization of default values
        public bool ActiveOnCancel
        {
            get { return _ActiveOnCancel; }
            set { _ActiveOnCancel = value; }
        }

        [DefaultValue(false)] // This prevents serialization of default values
        public bool ActiveOnView
        {
            get { return _ActiveOnView; }
            set { _ActiveOnView = value; }
        }

        // This might have different defaults in descendants, so don't declare default here because 
        // it will screw up serialization/deserialization
        /*[System.ComponentModel.DefaultValue(true)] // This prevents serialization of default values*/

        public bool ActiveOnReissue
        {
            get { return _ActiveOnReissue; }
            set { _ActiveOnReissue = value; }
        }

        // This might have different defaults in descendants, so don't declare default here because 
        // it will screw up serialization/deserialization
        /*[System.ComponentModel.DefaultValue(true)] // This prevents serialization of default values*/

        public bool ActiveOnContinuance
        {
            get { return _ActiveOnContinuance; }
            set { _ActiveOnContinuance = value; }
        }

        [DefaultValue(false)] // This prevents serialization of default values
        public bool DisabledIfNoChange
        {
            get { return _DisabledIfNoChange; }
            set { _DisabledIfNoChange = value; }
        }

        [DefaultValue(false)] // This prevents serialization of default values
        public bool Overrideable
        {
            get { return _Overrideable; }
            set { _Overrideable = value; }
        }

        public Context Ctx { get; set; }

        #endregion
        public delegate void RestrictionForcesDisplayRebuild();
        public delegate void ListContentsChangedByRestriction(EditControlBehavior iBehavior);
        public delegate void DoHotSheetFilter(TER_HotsheetFilter EditRestrict);
        public delegate bool SetIssueNoFields();

        public EditRestriction()
        {
            Conditions = new List<ConditionEvaluation>();
        }

        #region Implementation code at base-level of all TEditRestriction objects

        public static string glUserName = ""; // Global variable for the current user.
        private bool fConditionsChanged;
        private int fEnforcementAttributes;

        public virtual bool EnforceRestriction(int iNotifyEvent, int iFormEditMode,
                                               ref EditControlBehavior iParentBehavior)
        {
            return false;
        }

        public virtual bool ConditionsChangedSinceLastCheck()
        {
            return fConditionsChanged;
        }


       

        /// <summary>
        /// Enhanced field updating with Android specific handling of updating View controls
        /// </summary>
        /// <param name="iNewValue"></param>
        public virtual void UpdateParentControlWithNewValue(string iNewValue, EditEnumerations.IgnoreEventsType iIgnoreEvents, EditEnumerations.SetHasBeenFocusedType iSetHasBeenFocused)
        {

            Helper.UpdateControlWithNewValuePrim(Parent, iNewValue, iIgnoreEvents, iSetHasBeenFocused);

#if _previous_implementation_
            // translate NULLs into empty strings
            if (string.IsNullOrEmpty(iNewValue))
            {
                //set this to empty string since once this changes, we need to clear the controls that might have already had values.
                iNewValue = string.Empty;
            }

            // call the appropriate object type
            if (Parent.ControlType == EditEnumerations.CustomEditControlType.EditText)
            {
                var customControl = (CustomEditText)Parent.EditCtrl;
                if (customControl != null)
                {
                    if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                    {
                        customControl.IgnoreEvents = true;
                    }

                    try
                    {
                        customControl.Text = iNewValue;

                        if (iSetHasBeenFocused == EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue)
                        {
                            // TODO - there should be a PrepareForEdit to take care of this kind of stuff
                            customControl.HasBeenFocused = true;
                            //customControl.FormStatus = "Processed";
                        }
                    }
                    catch (Exception exp)
                    {
                        LoggingManager.LogApplicationError(exp, "FieldName: " + customControl.CustomId, "UpdateParentControlWithNewValue");
                        Console.WriteLine("UpdateParentControlWithNewValue: {0} {1}", exp, customControl.CustomId);
                    }
                    finally
                    {
                        if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                        {
                            customControl.IgnoreEvents = false;
                        }
                    }
                }
            }


            if (Parent.ControlType == EditEnumerations.CustomEditControlType.AutoCompleteText)
            {
                var customControl = (CustomAutoTextView)Parent.EditCtrl;
                if (customControl != null)
                {
                    if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                    {
                        customControl.IgnoreEvents = true;
                    }

                    try
                    {

                        // lets see if this text is a match for a list item
                        int loIdx = -1;
                        if (customControl.BehaviorAndroid != null)
                        {
                            // see if its a list item
                            loIdx = Helper.GetListItemIndexFromStringList(customControl.BehaviorAndroid.GetFilteredListItems(), iNewValue, Helper.ListItemMatchType.searchNoPartialMatch);

                            // lets get the list item, which might include abbrev + text desc
                            if (loIdx != -1)
                            {
                                iNewValue = customControl.BehaviorAndroid.GetFilteredListItems()[loIdx];
                            }
                        }


                        // now update the control with the value
                        customControl.SetListItemDataByValue(iNewValue);


                        if (iSetHasBeenFocused == EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue)
                        {
                            // TODO - there should be a InitForEntry to take care of this kind of stuff
                            customControl.HasBeenFocused = true;
                            //customControl.FormStatus = "Processed";
                        }


                        if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                        {

                            customControl.IgnoreEvents = false;
                        }
                    }
                    catch (Exception exp)
                    {
                        LoggingManager.LogApplicationError(exp, "FieldName: " + customControl.CustomId, "UpdateParentControlWithNewValue");
                        Console.WriteLine("UpdateParentControlWithNewValue: {0} {1}", exp, customControl.CustomId);
                    }
                    finally
                    {
                        if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                        {
                            customControl.IgnoreEvents = false;
                        }
                    }

                }
            }


            if (Parent.ControlType == EditEnumerations.CustomEditControlType.Spinner)
            {
                var customControl = (CustomSpinner)Parent.EditCtrl;
                if (customControl != null)
                {

                    if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                    {
                        customControl.IgnoreEvents = true;
                    }

                    try
                    {

                        if (string.IsNullOrEmpty(iNewValue))
                        {
                            customControl.SetListIndex(-1);
                            //customControl.SetListIndex(0);
                        }
                        else
                        {
                            customControl.SetListItemDataByValue(iNewValue);
                        }


                        if (iSetHasBeenFocused == EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue)
                        {
                            // TODO - there should be a InitForEntry to take care of this kind of stuff
                            customControl.HasBeenFocused = true;
                            //customControl.FormStatus = "Processed";
                        }


                        if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                        {
                            customControl.IgnoreEvents = false;
                        }
                    }
                    catch (Exception exp)
                    {
                        LoggingManager.LogApplicationError(exp, "FieldName: " + customControl.CustomId, "UpdateParentControlWithNewValue");
                        Console.WriteLine("UpdateParentControlWithNewValue: {0} {1}", exp, customControl.CustomId);
                    }
                    finally
                    {
                        if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                        {
                            customControl.IgnoreEvents = false;
                        }
                    }

                }
            }

#endif

        }


        int labelColorRedAsInt = Color.Red;
        int labelColorBlackAsInt = Color.Black;

        public virtual void UpdateLabelColorToShowErrorStatus(bool isInErrorCondition)
        {
            LinearLayout currentLayout = null;
            if (Parent.ControlType == EditEnumerations.CustomEditControlType.EditText)
                currentLayout = ((CustomEditText)(Parent.EditCtrl)).ParentLayout;
            if (Parent.ControlType == EditEnumerations.CustomEditControlType.AutoCompleteText)
                currentLayout = ((CustomAutoTextView)(Parent.EditCtrl)).ParentLayout;
            if (Parent.ControlType == EditEnumerations.CustomEditControlType.Spinner)
                currentLayout = ((CustomSpinner)(Parent.EditCtrl)).ParentLayout;

            if (currentLayout != null)
            {
                //var labelTag = StructName + (Parent.PanelField.Label ?? Parent.PanelField.Name) + "_LABEL";
                var labelTag = Helper.GetLabelFieldTag(Parent.PanelField);


                View vw = currentLayout.FindViewWithTag(labelTag);
                if (vw != null)
                {
                    var labelView = vw as TextView;
                    if (labelView != null)
                    {


                        // update the color
                        if (isInErrorCondition == true)
                        {
                            labelView.SetTextColor(Color.Red);
                        }
                        else
                        {
                            labelView.SetTextColor(Color.Black);
                        }


                        // AJW - TODO - below doesn't evaluate because the color INT not same as Color.Red
                        //            - will be good to fix to avoid un-needed repaints
                        return;

                       

                        // check the label and see if if needs update and repaint
                        int loLabelCurrentColor = labelView.CurrentTextColor;


                        // decide the current visible status
                        bool loShowsErrorCondition = (loLabelCurrentColor == labelColorRedAsInt);

                        // what do we want to show?
                        if (isInErrorCondition == true)
                        {
                            if (loShowsErrorCondition == true)
                            {
                                // we already have what we want, no need to repaint
                                return;
                            }

                            // update the color
                            labelView.SetTextColor(Color.Red);
                        }
                        else
                        {
                            if (loShowsErrorCondition == false)
                            {
                                // we already have what we want, no need to repaint
                                return;
                            }

                            // update the color
                            labelView.SetTextColor(Color.Black);
                        }

                    }
                }

            }
        }


        public virtual void UpdateLabelToShowRequiredStatus(bool isRequired)
        {
            LinearLayout currentLayout = null;
            if (Parent.ControlType == EditEnumerations.CustomEditControlType.EditText)
                currentLayout = ((CustomEditText)(Parent.EditCtrl)).ParentLayout;
            if (Parent.ControlType == EditEnumerations.CustomEditControlType.AutoCompleteText)
                currentLayout = ((CustomAutoTextView)(Parent.EditCtrl)).ParentLayout;
            if (Parent.ControlType == EditEnumerations.CustomEditControlType.Spinner)
                currentLayout = ((CustomSpinner)(Parent.EditCtrl)).ParentLayout;

            if (currentLayout != null)
            {
                var labelTag = Helper.GetLabelFieldTag(Parent.PanelField);


                View vw = currentLayout.FindViewWithTag(labelTag);
                if (vw != null)
                {
                    var labelView = vw as TextView;
                    if (labelView != null)
                    {
                        // check the label and see if if needs update and repaint
                        string loNewLabel = labelView.Text;

                        // decide the current visible status
                        bool loShowsRequired = (loNewLabel.IndexOf('*') != -1);

                        // what do we want to show?
                        if (isRequired == true)
                        {
                            if (loShowsRequired == true)
                            {
                                // we already have what we want, no need to repaint
                                return;
                            }

                            // set the color, and add asterisk 
                            labelView.SetTextColor(Color.Red);
                            labelView.Text = loNewLabel + " *";
                        }
                        else
                        {
                            if (loShowsRequired == false)
                            {
                                // we already have what we want, no need to repaint
                                return;
                            }

                            // set the color, and remove the asterisk
                            labelView.SetTextColor(Color.Black);
                            labelView.Text = loNewLabel.Replace(" *", "");
                        }

                    }
                }

            }
        }


        public EditEnumerations.ETrueFalseIgnore RestrictionActiveOnEvent(int iNotifyEvent, int iFormEditMode,
                                                                          ref EditControlBehavior iParentBehavior)
        {
            var loResult = EditEnumerations.ETrueFalseIgnore.tfiIgnore;
            // if the event is one of "ParentDataChanged, DataChanged, AncestorDataChanged" then we will
            // reset the fConditionsChanged flag 
            if ((iNotifyEvent & (EditRestrictionConsts.dneParentDataChanged | EditRestrictionConsts.dneDataChanged)) > 0)
                fConditionsChanged = true;

            // Build bit-level patterns to represent the active events, modes and attributes
            int fActiveNotifyEvents = 0;
            int fActiveFormEditModes = 0;

            if (ActiveOnAncestorFieldExit)
                fActiveNotifyEvents |= EditRestrictionConsts.dneAncestorFieldExit;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneAncestorFieldExit;
            if (ActiveOnBeforeEdit)
                fActiveNotifyEvents |= EditRestrictionConsts.dneBeforeEdit;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneBeforeEdit;
            if (ActiveOnCancel)
                fActiveFormEditModes |= EditRestrictionConsts.femCancelAttr;
            else
                fActiveFormEditModes &= ~EditRestrictionConsts.femCancelAttr;
            if (ActiveOnContinuance)
                fActiveFormEditModes |= EditRestrictionConsts.femContinuance;
            else
                fActiveFormEditModes &= ~EditRestrictionConsts.femContinuance;
            if (ActiveOnCorrection)
                fActiveFormEditModes |= EditRestrictionConsts.femCorrectionAttr;
            else
                fActiveFormEditModes &= ~EditRestrictionConsts.femCorrectionAttr;
            if (ActiveOnDataChanged)
                fActiveNotifyEvents |= EditRestrictionConsts.dneDataChanged;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneDataChanged;
            if (ActiveOnFirstEditFocus)
                fActiveNotifyEvents |= EditRestrictionConsts.dneFirstEditFocus;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneFirstEditFocus;
            if (ActiveOnFormInit)
                fActiveNotifyEvents |= EditRestrictionConsts.dneFormInit;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneFormInit;
            if (ActiveOnIssueMore)
                fActiveFormEditModes |= EditRestrictionConsts.femIssueMoreAttr;
            else
                fActiveFormEditModes &= ~EditRestrictionConsts.femIssueMoreAttr;
            if (ActiveOnNewEntry)
                fActiveFormEditModes |= EditRestrictionConsts.femNewEntry;
            else
                fActiveFormEditModes &= ~EditRestrictionConsts.femNewEntry;
            if (ActiveOnParentDataChanged)
                fActiveNotifyEvents |= EditRestrictionConsts.dneParentDataChanged;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneParentDataChanged;
            if (ActiveOnParentFieldExit)
                fActiveNotifyEvents |= EditRestrictionConsts.dneParentFieldExit;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneParentFieldExit;
            if (ActiveOnReissue)
                fActiveFormEditModes |= EditRestrictionConsts.femReissue;
            else
                fActiveFormEditModes &= ~EditRestrictionConsts.femReissue;
            if (ActiveOnValidate)
                fActiveNotifyEvents |= EditRestrictionConsts.dneValidate;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneValidate;
            if (ActiveOnView)
                fActiveFormEditModes |= EditRestrictionConsts.femView;
            else
                fActiveFormEditModes &= ~EditRestrictionConsts.femView;

            if (DisabledIfNoChange)
                fEnforcementAttributes |= EditRestrictionConsts.ecDisabledIfNoChange;
            else
                fEnforcementAttributes &= ~EditRestrictionConsts.ecDisabledIfNoChange;
            if (Overrideable)
                fEnforcementAttributes |= EditRestrictionConsts.ecOverRideable;
            else
                fEnforcementAttributes &= ~EditRestrictionConsts.ecOverRideable;

            // if restriction is inactive for this event, return FALSE

            // Active on no events means active on ALL events
            if ((fActiveNotifyEvents > 0) && ((iNotifyEvent & fActiveNotifyEvents) > 0) == false)
                return EditEnumerations.ETrueFalseIgnore.tfiIgnore;

            if ((iFormEditMode & fActiveFormEditModes) == 0)
                return EditEnumerations.ETrueFalseIgnore.tfiIgnore;

            if (((fEnforcementAttributes & EditRestrictionConsts.ecDisabledIfNoChange) > 0) &&
                (!ConditionsChangedSinceLastCheck()))
                return EditEnumerations.ETrueFalseIgnore.tfiIgnore;

            // if no associated conditions, return TRUE 
            fConditionsChanged = false;

            if (Conditions.Count == 0)
            {
                if ((fActiveNotifyEvents == 0) ||
                    (((iNotifyEvent &
                       (EditRestrictionConsts.dneParentFieldExit | EditRestrictionConsts.dneParentDataChanged)) == 0)))
                    return EditEnumerations.ETrueFalseIgnore.tfiTrue;
                if ((iParentBehavior.CfgCtrl.Name != ControlEdit1) &&
                    (iParentBehavior.CfgCtrl.Name != ControlEdit2) &&
                    (iParentBehavior.CfgCtrl.Name != ControlEdit3))
                    return EditEnumerations.ETrueFalseIgnore.tfiIgnore;
                return EditEnumerations.ETrueFalseIgnore.tfiTrue;
            }

            // if the event for this restriction is ParentFieldExit or ParentDataChanged, make sure
            // the field causing the event is a parent in ONE of the conditions or the parent of
            // the restriction itself
            if ((fActiveNotifyEvents > 0) &&
                (((iNotifyEvent &
                   (EditRestrictionConsts.dneParentFieldExit | EditRestrictionConsts.dneParentDataChanged)) > 0)))
            {
                bool loOneParentCausedEvent = false;

                // is the the parent of this restriction??
                if ((iParentBehavior.CfgCtrl.Name.Equals(ControlEdit1)) ||
                    (iParentBehavior.CfgCtrl.Name.Equals(ControlEdit2)) ||
                    (iParentBehavior.CfgCtrl.Name.Equals(ControlEdit3)))
                    loOneParentCausedEvent = true;
                else // or, perhaps, a parent of one of our conditions
                {
                    // JLA (3/2/07): This code was looking at parent edit conditions instead of
                    //    edit conditions specified by this TEditRestriction. Problem was exposed
                    //    by configuration for Seattle demo
                    /* AJW james really did want this removed... in 2007, not 2013!
                    foreach (EditCondition NextExitCondition in Parent.EditConditions)
                    {
                        if ((iParentBehavior.CfgCtrl.Name.Equals(NextExitCondition.ControlEdit1)) ||
                        (iParentBehavior.CfgCtrl.Equals(NextExitCondition.ControlEdit2)) ||
                        (iParentBehavior.CfgCtrl.Equals(NextExitCondition.ControlEdit3)))
                        {
                            loOneParentCausedEvent = true;
                            break;
                        }
                    }
                     * */

                    // JLA (3/2/07): Corrected version of code to fix problem exposed by Seattle demo
                    foreach (ConditionEvaluation nextEvaluation in Conditions)
                    {
                        // Find parent's condition that matches name from condition evaluation object
                        EditCondition assocCondition =
                            Parent.EditConditions.FirstOrDefault(
                                nextExitCondition => nextExitCondition.Name == nextEvaluation.ConditionName);

                        // Did we find an associated TEditCondition?
                        if (assocCondition == null) continue;
                        if ((!iParentBehavior.CfgCtrl.Name.Equals(assocCondition.ControlEdit1)) &&
                            (!iParentBehavior.CfgCtrl.Equals(assocCondition.ControlEdit2)) &&
                            (!iParentBehavior.CfgCtrl.Equals(assocCondition.ControlEdit3))) continue;
                        loOneParentCausedEvent = true;
                        break;
                    }
                }

                // The field causing this event is not a parent, therefore we can ignore this restriction.
                if (!loOneParentCausedEvent)
                    return EditEnumerations.ETrueFalseIgnore.tfiIgnore;
            }

            foreach (ConditionEvaluation nextCondition in Conditions)
            {
                // Find the implemented version of the edit condition (Match by name -- case insensitive)
                var predicate = new Reino.ClientConfig.TObjBasePredicate(nextCondition.ConditionName);
                EditCondition loEditConditionImp = Parent.EditConditions.Find(predicate.CompareByName_CaseInsensitive);

                // there is an associated condition, return TRUE if it evaluates to what this restriction requires. 
                if (loEditConditionImp != null)
                {
                    if (loEditConditionImp.EvaluateCondition() == nextCondition.Evaluation)
                    {
                        // this condition evaluated true, so set our return value to true 
                        loResult = EditEnumerations.ETrueFalseIgnore.tfiTrue;
                    }
                    else
                    {
                        // this condition evaluated false.  If all the conditions are being AND'd together,
                        // then short-circuit the evaluation and return FALSE now. Otherwise everything is being
                        // OR'd, and we have to trudg through all the conditions 
                        if (ConditionsANDed)
                            return EditEnumerations.ETrueFalseIgnore.tfiFalse;
                        // if no conditions have evaluated to TRUE, set our return value to FALSE, thus overriding
                        // the default IGNORE result. 
                        if (loResult == EditEnumerations.ETrueFalseIgnore.tfiIgnore)
                            loResult = EditEnumerations.ETrueFalseIgnore.tfiFalse;
                    }
                }
            }
            return loResult;
        }

        public void SetDisabledIfNoChange(bool iValue)
        {
            if (iValue)
                fEnforcementAttributes |= EditRestrictionConsts.ecDisabledIfNoChange;
            else
                fEnforcementAttributes &= ~EditRestrictionConsts.ecDisabledIfNoChange;
        }

        #endregion
    }
}