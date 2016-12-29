using System.Xml.Serialization;
using Duncan.AI.Droid.Utils.EditControlManagement.EditRules;

namespace Duncan.AI.Droid.Utils.EditControlManagement.Entities
{
    public static class EditRestrictionConsts
    {
        // TDependentNotifyEvents 
        public const int dneFormInit = 0x00000001;
        public const int dneBeforeEdit = 0x00000002;
        public const int dneDataChanged = 0x00000004;
        public const int dneValidate = 0x00000008;
        public const int dneParentFieldExit = 0x00000010;
        public const int dneParentDataChanged = 0x00000020;
        public const int dneFirstEditFocus = 0x00000040;
        public const int dneAncestorFieldExit = 0x00000080;
        public const int dneValidatePendingRestrictionsPriorToStorage = 0x00000100;

        // Enforcement conditions
        public const int ecDisabledIfNoChange = 0x00000001;
        public const int ecOverRideable = 0x00000002;

        // Form edit mode attributes
        public const int femNewRecordAttr = 0x0001;
        public const int femCorrectionAttr = 0x0002;
        public const int femIssueMoreAttr = 0x0004;
        public const int femCancelAttr = 0x0008;
        public const int femSingleRecordAttr = 0x0010;
        public const int femViewAttr = 0x0020;
        public const int femReissueAttr = 0x0040;
        public const int femContinuanceAttr = 0x0080;
        public const int femIssueMultipleAttr = 0x0100;

        // Compound form edit mode attributes
        public const int femNewEntry = femNewRecordAttr;
        public const int femIssueMore = (femIssueMoreAttr);
        public const int femView = femViewAttr;
        public const int femSingleEntry = (femNewRecordAttr | femSingleRecordAttr);
        public const int femReissue = (femSingleRecordAttr | femReissueAttr);
        public const int femContinuance = (femSingleRecordAttr | femContinuanceAttr);
        public const int femIssueMultiple = (femIssueMultipleAttr | femIssueMore);

        // form edit attributes 
        public const int feaPrinted = 0x00000001;
        public const int feaSaved = 0x00000002;
        public const int feaEditedFirstField = 0x00000004;
        public const int feaIssueNoLogged = 0x00000008;
        public const int feaTempFileSaved = 0x00000010;

        // edit state attributes.  Dynamic attributes that last the duration of a single form edit.
        public const int esaPreInitialized = 0x0001;
        public const int esaEdited = 0x0002;
        public const int esaPrinted = 0x0004;
    }


     [XmlInclude(typeof(EditCondition)), XmlInclude(typeof(EditRestriction)),
     XmlInclude(typeof(TER_Protected)), XmlInclude(typeof(TER_Hidden)),
     XmlInclude(typeof(TER_ChildList)), XmlInclude(typeof(TER_ListItemsOnly)),
     XmlInclude(typeof(TER_ForceLiteral)), XmlInclude(typeof(TER_ForceParentValue)),
     XmlInclude(typeof(TER_ForceListItem)), XmlInclude(typeof(TER_Required)),
     XmlInclude(typeof(TER_Conditions)), XmlInclude(typeof(TER_ListFilter)),
     XmlInclude(typeof(TER_ForceCurrentDateTime)), XmlInclude(typeof(TER_ValuesMustMatch)),
     XmlInclude(typeof(TER_Increment)), XmlInclude(typeof(TER_Multiply)),
     XmlInclude(typeof(TER_CalcForceAge)), XmlInclude(typeof(TER_ValidateInches)),
     XmlInclude(typeof(TER_ForceCleared)), XmlInclude(typeof(TER_CurrentUser)),
     XmlInclude(typeof(TER_ForceGlobalCurrentValue)), XmlInclude(typeof(TER_SetGlobalCurrentValue)),
     XmlInclude(typeof(TER_Mod10CheckDigit)), XmlInclude(typeof(TER_ForceCalcJuvenile)),
    XmlInclude(typeof(TER_VicRoadsCheckDigit)), XmlInclude(typeof(TER_ForceStLouisCheckDigit)),
     XmlInclude(typeof(TER_BaseHotSheet)), XmlInclude(typeof(TER_SearchHotSheet)),
     XmlInclude(typeof(TER_HotsheetFilter)), XmlInclude(typeof(TER_ForceSequence)),
     XmlInclude(typeof(TER_ForceCincinnatiMod7CheckDigit))]
    public class EditDependency : Reino.ClientConfig.TObjBase
    {
        #region Properties and Members
        public int IntParam { get; set; }
        public int IntParam2 { get; set; }
        public string CharParam { get; set; }
        public string ControlEdit1 { get; set; }
        public string ControlEdit2 { get; set; }
        public string ControlEdit3 { get; set; }
        [XmlIgnoreAttribute]
        public new EditControlBehavior Parent { get; set; }
               [XmlIgnoreAttribute]
        public EditControlBehavior ControlEdit1Obj { get; set; }
        [XmlIgnoreAttribute]
        public EditControlBehavior ControlEdit2Obj { get; set; }
        [XmlIgnoreAttribute]
        public EditControlBehavior ControlEdit3Obj { get; set; }
        #endregion

        #region Implementation code
       
        protected EditControlBehavior _Parent = null;
        public EditControlBehavior GetParent()
        {
            return _Parent;
        }
        public void SetParent(EditControlBehavior pValue)
        {
            Parent = pValue;
            _Parent = pValue;
        }

        protected int CheckForControlEdit1()
        {
            if ((ControlEdit1 != "") && (ControlEdit1Obj != null))
                return 0;
            return 1;
        }

        protected int CheckForControlEdit2()
        {
            if ((ControlEdit2 != "") && (ControlEdit2Obj != null))
                return 0;
            return 1;
        }

        protected int CheckForControlEdit3()
        {
            if ((ControlEdit3 != "") && (ControlEdit3Obj != null))
                return 0;
            return 1;
        }

        public void ResolveObjectReferences()
        {

            // some Android defense.. not all code paths properly check for null
            if (ControlEdit1 == null)
            {
                ControlEdit1 = string.Empty;
            }
            if (ControlEdit2 == null)
            {
                ControlEdit2 = string.Empty;
            }
            if (ControlEdit3 == null)
            {
                ControlEdit3 = string.Empty;
            }
            if (CharParam == null)
            {
                CharParam = string.Empty;
            }




            // If there is a ControlEdit1 name specified, try to resolve reference to actual object
            if ((!string.IsNullOrEmpty(ControlEdit1)) && (ControlEdit1Obj == null))
            {
                // Find ControlEdit1 by name
                EditControlBehavior loControlEdit = Parent.GetEditControlBehaviorByName(ControlEdit1);
                if (loControlEdit != null)
                {
                    // Keep reference to the edit control object
                    ControlEdit1Obj = loControlEdit;
                    // Now add ourself as a dependent of the ControlEdit if its not ourself
                    if (loControlEdit != Parent)
                        loControlEdit.Dependents.Add(Parent);
                }
            }

            // If there is a ControlEdit2 name specified, try to resolve reference to actual object
            if ((!string.IsNullOrEmpty(ControlEdit2)) && (ControlEdit2Obj == null))
            {
                // Find ControlEdit2 by name
                EditControlBehavior loControlEdit = Parent.GetEditControlBehaviorByName(ControlEdit2);
                if (loControlEdit != null)
                {
                    // Keep reference to the edit control object
                    ControlEdit2Obj = loControlEdit;
                    // Now add ourself as a dependent of the ControlEdit if its not ourself
                    if (loControlEdit != Parent)
                        loControlEdit.Dependents.Add(Parent);
                }
            }

            // If there is a ControlEdit3 name specified, try to resolve reference to actual object
            if ((!string.IsNullOrEmpty(ControlEdit3)) && (ControlEdit3Obj == null))
            {
                // Find ControlEdit3 by name
                EditControlBehavior loControlEdit = Parent.GetEditControlBehaviorByName(ControlEdit3);
                if (loControlEdit != null)
                {
                    // Keep reference to the edit control object
                    ControlEdit3Obj = loControlEdit;
                    // Now add ourself as a dependent of the ControlEdit if its not ourself
                    if (loControlEdit != Parent)
                        loControlEdit.Dependents.Add(Parent);
                }
            }
        }

        protected EditControlBehavior GetControlEdit1()
        {
            // If we don't have a reference to the object yet, try to resolve it first
            if (ControlEdit1Obj == null)
                ResolveObjectReferences();
            // Now return the ControlEdit object 
            return ControlEdit1Obj;
        }

        protected EditControlBehavior GetControlEdit2()
        {
            // If we don't have a reference to the object yet, try to find by name
            if (ControlEdit2Obj == null)
                ResolveObjectReferences();
            // Now return the ControlEdit object 
            return ControlEdit2Obj;
        }

        protected EditControlBehavior GetControlEdit3()
        {
            // If we don't have a reference to the object yet, try to find by name
            if (ControlEdit3Obj == null)
                ResolveObjectReferences();
            // Now return the ControlEdit object 
            return ControlEdit3Obj;
        }

        #endregion
    }
}