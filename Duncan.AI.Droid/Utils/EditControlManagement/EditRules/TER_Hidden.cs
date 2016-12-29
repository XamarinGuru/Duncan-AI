using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    public class TER_Hidden : EditRestriction
    {
        #region Properties and Members
        public event RestrictionForcesDisplayRebuild OnRestrictionForcesDisplayRebuild = null;
        #endregion

        public TER_Hidden()
            : base()
        {
        }

        public string StructName { get; set; }

        #region Implementation code
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            EditEnumerations.ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) ==
                EditEnumerations.ETrueFalseIgnore.tfiIgnore)
            {
                //since the labels are directly tied to the edit control, we must have it match the visibility fo the control on every load and not jsut active ones.
                //we render them by default, so if the current control is hidden, then hide the associated label.
                if (Parent.CfgCtrl.IsHidden)
                {
                    SetLabelVisibility(ViewStates.Gone);
                }

                return false;
            }

            // If there is an associated control, set its visible state 
            if (Parent.EditCtrl != null)
            {
                var visible = (loEnforce == EditEnumerations.ETrueFalseIgnore.tfiFalse) ? ViewStates.Visible : ViewStates.Gone;
                SetLabelVisibility(visible);
                Parent.EditCtrl.Visibility = visible;
                Parent.EditCtrl.LayoutParameters = _layoutParameters;
                //GO FIND THE LABEL AND UPDATE THAT AS WELL

                //////uncomment this for testing so you can see the hidden fields
                //if (visible != ViewStates.Visible)
                //{
                //    Parent.EditCtrl.SetBackgroundColor(Color.Pink);
                //    Parent.EditCtrl.Visibility = ViewStates.Visible;
                //    Parent.EditCtrl.Enabled = true;
                //}

            }

            // Also set "CurrentlyHidden" attribute in behavior object
            // and let application know it should rebuild the display if the state has changed
            bool loCurrentlyHidden = (loEnforce != EditEnumerations.ETrueFalseIgnore.tfiFalse);
            if (Parent.CfgCtrl.IsHidden != loCurrentlyHidden)
            {
                Parent.CfgCtrl.IsHidden = loCurrentlyHidden;
                if (OnRestrictionForcesDisplayRebuild != null)
                {
                    OnRestrictionForcesDisplayRebuild();
                }
            }
            return false;
        }

        readonly LinearLayout.LayoutParams _layoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
        private void SetLabelVisibility(ViewStates visibilityState)
        {

            if (visibilityState == ViewStates.Gone)
                _layoutParameters.SetMargins(0, 0, 0, 0);
            else
                _layoutParameters.SetMargins(10, 20, 0, 0);

            LinearLayout currentLayout = null;
            if (Parent.ControlType == EditEnumerations.CustomEditControlType.EditText)
                currentLayout = ((CustomEditText) (Parent.EditCtrl)).ParentLayout;
            if (Parent.ControlType == EditEnumerations.CustomEditControlType.AutoCompleteText)
                currentLayout = ((CustomAutoTextView) (Parent.EditCtrl)).ParentLayout;
            if (Parent.ControlType == EditEnumerations.CustomEditControlType.Spinner)
                currentLayout = ((CustomSpinner) (Parent.EditCtrl)).ParentLayout;

            if (currentLayout != null)
            {

                //var labelTag = StructName + (Parent.PanelField.Label ?? Parent.PanelField.Name) + "_LABEL";

                string labelTag = Helper.GetLabelFieldTag(Parent.PanelField);



                View vw = currentLayout.FindViewWithTag(labelTag);
                if (vw != null)
                {
                    vw.Visibility = visibilityState;
                    currentLayout.LayoutParameters = _layoutParameters;
                }
            }
        }

        #endregion
    }
}