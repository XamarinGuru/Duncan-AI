using System;
using Android.Content;
using Android.Preferences;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Duncan.AI.Droid.Utils.HelperManagers;

namespace Duncan.AI.Droid.Utils.EditControlManagement.EditRules
{
    /// <summary>
    /// Summary description for TER_CurrentUser.
    /// </summary>
    public class TER_CurrentUser :  EditRestriction
    {
        #region Properties and Members

        public string StructName { get; set; }

        #endregion

        public TER_CurrentUser()
            : base()
        {
        }

        #region Implementation code

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref EditControlBehavior iParentBehavior)
        {
            try
            {
                if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != EditEnumerations.ETrueFalseIgnore.tfiTrue)
                    return false;


                // AJW TODO - the legacy code looped through the columns on the USERSTRUCT table and populated every matching fieldname on form
                //           - get USERSTRUCT definition meta-data at run time and populate here



                //crtTblStrBldr.Append("CREATE TABLE ");
                //crtTblStrBldr.Append(tableName);
                //crtTblStrBldr.Append(" (_id INTEGER PRIMARY KEY AUTOINCREMENT");
                ////Fields
                //ListObjBase<TTableFldDef> tTableFldDefList = tTableDef.HighTableRevision.Fields;
                //int noOfColms = 0;
                //foreach (TTableFldDef tTableFldDef in tTableFldDefList)
                //{
                //    crtTblStrBldr.Append(", ");
                //    crtTblStrBldr.Append("[" + tTableFldDef.Name + "]");

                //    if (noOfColms != 0)
                //        colNamesStrBldr.Append(", ");
                //    noOfColms++;
                //    colNamesStrBldr.Append("[" + tTableFldDef.Name + "]");
                //    //string dataType = convertDataType (tTableFldDef.EditDataType);
                //    crtTblStrBldr.Append(" ");
                //    crtTblStrBldr.Append(" ntext");
                //}
                //crtTblStrBldr.Append(");");
                //queryStringList.Add(crtTblStrBldr.ToString());


                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Ctx);
                if (Parent.CustomId == StructName + Constants.OFFICER_NAME)
                {
                    string username = prefs.GetString(Constants.OFFICER_NAME, null);
                    Parent.SetEditBufferAndPaint(username, false);
                }
                else if (Parent.CustomId == StructName + Constants.OFFICER_ID)
                {
                    string officerId = prefs.GetString(Constants.OFFICER_ID, null);
                    Parent.SetEditBufferAndPaint(officerId, false);
                }
                else if (Parent.CustomId == StructName + Constants.AGENCY)
                {
                    string agency = prefs.GetString(Constants.AGENCY, null);
                    Parent.SetEditBufferAndPaint(agency, false);
                }
                else if (Parent.CustomId == StructName + Constants.SUBAGENCY)
                {
                    string subagency = prefs.GetString(Constants.SUBAGENCY, null);
                    Parent.SetEditBufferAndPaint(subagency, false);
                }    
                return false;
            }
            catch (Exception Ex)
            {
                LoggingManager.LogApplicationError(Ex, null, "TER_CurrentUser");
                return false;
            }
        }

        #endregion
    }
}