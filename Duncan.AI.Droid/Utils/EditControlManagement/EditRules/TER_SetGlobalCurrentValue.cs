using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Android.Content;
using Android.Preferences;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_SetGlobalCurrentValue.
    /// </summary>
    public class TER_SetGlobalCurrentValue :  EditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_SetGlobalCurrentValue()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnReissue = false;
            ActiveOnContinuance = false;
            ActiveOnValidate = true;
        }

        #region Implementation code
        //public delegate void SetCurrentGlobalFieldValue(string FieldName, string FieldValue);
        //public event SetCurrentGlobalFieldValue OnSetCurrentGlobalFieldValue = null;

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                return false;


            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(DroidContext.ApplicationContext);
            ISharedPreferencesEditor editor = prefs.Edit();

            // here we are working with the global value, not just one from this struct
            //string loGlobalStoreKeyName = Helper.BuildGlobalPreferenceKeyName(Parent.PanelField.fParentStructName, Parent.PanelField.Name);
            string loGlobalStoreKeyName = Helper.BuildGlobalPreferenceKeyName(AutoISSUE.DBConstants.cnDefaultSharedPreferencesGlobalPrefix, Parent.PanelField.Name);



            string loValue = Parent.GetValue();
            //editor.PutString(Parent._CfgCtrl.Name, loValue);
            editor.PutString(loGlobalStoreKeyName, loValue);
            editor.Apply();


            //string loValue = Parent.GetValue();
            //DroidContext.SetCurrentGlobalFieldValue(Parent._CfgCtrl.Name, loValue);


            return false;
        }
        #endregion
    }
}