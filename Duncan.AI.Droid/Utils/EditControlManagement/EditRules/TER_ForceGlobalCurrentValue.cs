using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Android.Content;
using Android.Preferences;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_ForceGlobalCurrentValue.
    /// </summary>
    public class TER_ForceGlobalCurrentValue :  EditRestriction
    {

        public TER_ForceGlobalCurrentValue()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnCorrection = false;
            ActiveOnReissue = false;
            ActiveOnContinuance = false;
            ActiveOnFormInit = true;
            ActiveOnFirstEditFocus = true;
        }

        #region Implementation code
        //public delegate string GetCurrentGlobalFieldValue(string FieldName);
        //public event GetCurrentGlobalFieldValue OnGetCurrentGlobalFieldValue = null;

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;


            // here we are working with the global value, not just one from this struct
            //string loGlobalStoreKeyName = Helper.BuildGlobalPreferenceKeyName(Parent.PanelField.fParentStructName, Parent.PanelField.Name);
            string loGlobalStoreKeyName = Helper.BuildGlobalPreferenceKeyName(AutoISSUE.DBConstants.cnDefaultSharedPreferencesGlobalPrefix, Parent.PanelField.Name);


            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(DroidContext.ApplicationContext);

            //string globalValue = prefs.GetString(Parent._CfgCtrl.Name, null);
            string globalValue = prefs.GetString(loGlobalStoreKeyName, null);

            UpdateParentControlWithNewValue((globalValue ?? string.Empty), EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue);

            return false;
        }
        #endregion
    }
}