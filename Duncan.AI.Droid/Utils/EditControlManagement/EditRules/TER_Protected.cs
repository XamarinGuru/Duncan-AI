using Android.Content;
using Android.Graphics;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    public class TER_Protected : EditRestriction
    {
        #region Properties and Members
        public event RestrictionForcesDisplayRebuild OnRestrictionForcesDisplayRebuild = null;
        #endregion

        public TER_Protected() : base()
        {
        }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            EditEnumerations.ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) == EditEnumerations.ETrueFalseIgnore.tfiIgnore)
                return false;

            // Also set "CurrentlyProtected" attribute in behavior object
            // and let application know it should rebuild the display if the state has changed
            bool loCurrentlyProtected = (loEnforce != EditEnumerations.ETrueFalseIgnore.tfiFalse);
            if (Parent.CfgCtrl != null)
            {
                if (Parent.CfgCtrl.IsProtected != loCurrentlyProtected)
                {
                    Parent.CfgCtrl.IsProtected = loCurrentlyProtected;
                    if (Parent.EditCtrl != null)
                    {
                        Parent.EditCtrl.Enabled = (loEnforce == EditEnumerations.ETrueFalseIgnore.tfiFalse);

                        if (Parent.EditCtrl.Enabled == true)
                        {
                            Parent.EditCtrl.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);
                        }
                        else
                        {
                            Parent.EditCtrl.SetBackgroundResource(Resource.Drawable.EditTextDisabled);
                        }

                    }

                    if (OnRestrictionForcesDisplayRebuild != null)
                    {
                        OnRestrictionForcesDisplayRebuild();
                    }
                }
                else
                {
                    // If there is an associated control, set its enabled state
                    if (Parent.EditCtrl != null)
                    {
                        Parent.EditCtrl.Enabled = (loEnforce == EditEnumerations.ETrueFalseIgnore.tfiFalse);
                    }
                }
            }

            else
            {
                // JLA 2008.10.08
                // If there is an associated control, set its enabled state
                if (Parent.EditCtrl != null)
                    Parent.EditCtrl.Enabled = (loEnforce == EditEnumerations.ETrueFalseIgnore.tfiFalse);
            }

            //now set the colors based on enabled or not.
            if (Parent.EditCtrl != null)
            {


                if (Parent.ControlType == EditEnumerations.CustomEditControlType.EditText)
                {
                    var customEditText = (CustomEditText)Parent.EditCtrl;
                    if (customEditText != null)
                    {
                        if (customEditText.Enabled == true)
                        {
                            if (customEditText.IsFocused == true)
                            {
                                customEditText.SetTextColor(Color.Black);
                                customEditText.SetBackgroundResource(Resource.Drawable.EditTextFocused);
                            }
                            else
                            {
                                customEditText.SetTextColor(Color.Black);
                                customEditText.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);
                            }
                        }
                        else
                        {
                            // clear any error when a field goes into protected mode - it will be rechecked if it returns to editable mode
                            UpdateLabelColorToShowErrorStatus(false);
                            UpdateLabelToShowRequiredStatus(false);


                            // set protected status colors
                            customEditText.SetTextColor(Color.Gray);
                            customEditText.SetBackgroundResource(Resource.Drawable.EditTextDisabled);
                        }
                    }
                }

                if (Parent.ControlType == EditEnumerations.CustomEditControlType.AutoCompleteText)
                {
                    var customAutoTextView = (CustomAutoTextView)Parent.EditCtrl;
                    if (customAutoTextView != null)
                    {
                        if (customAutoTextView.Enabled == true)
                        {
                            if (customAutoTextView.IsFocused == true)
                            {
                                customAutoTextView.SetTextColor(Color.Black);
                                customAutoTextView.SetBackgroundResource(Resource.Drawable.EditTextDropDownListFocused);
                            }
                            else
                            {
                                customAutoTextView.SetTextColor(Color.Black);
                                customAutoTextView.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);
                            }
                        }
                        else
                        {
                            // clear any error when a field goes into protected mode - it will be rechecked if it returns to editable mode
                            UpdateLabelColorToShowErrorStatus(false);
                            UpdateLabelToShowRequiredStatus(false);

                            // set protected status colors
                            customAutoTextView.SetTextColor(Color.Gray);
                            customAutoTextView.SetBackgroundResource(Resource.Drawable.EditTextDisabled);
                        }
                    }
                }


            }

            return false;
        }
        #endregion
    }
}