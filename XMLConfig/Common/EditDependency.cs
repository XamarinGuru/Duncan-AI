// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 9/14/09 12:38p $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Common/ClientConfig/EditDependency.cs $
//              Revision: $Revision: 47 $



// DEVELOPER NOTE:
//
// The object defitions in this file are for the XML serializer
// The actual implementation of the object functions are in the Duncan.AI.Droid project
//
// ie The TER_SearchHotSheet object is defined here XML, but has no implementation functions. Those are 
// in TER_SearchHotSheet.cs in the EditRules folder of Duncan.AI.Droid

#define DEBUG

using System;
//using System.Data;
using System.Diagnostics;
//using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using ReinoControls;

namespace Reino.ClientConfig
{
    public enum ETrueFalseIgnore { tfiTrue, tfiFalse, tfiIgnore };

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

    /// <summary>
    /// Summary description for TEditDependency.
    /// </summary>
    /* The XmlInclude attribute is used on a base type to indicate that when serializing 
     * instances of that type, they might really be instances of one or more subtypes. 
     * This allows the serialization engine to emit a schema that reflects the possibility 
     * of really getting a Derived when the type signature is Base. For example, we keep
     * field definitions in a generic collection of TEditDependency. If an array element is 
     * TER_Protected, the XML serializer gets mad because it was only expecting TEditDependency. 
     */
    [XmlInclude(typeof(TEditCondition)), XmlInclude(typeof(TEditRestriction)),
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
    public class TEditDependency : Reino.ClientConfig.TObjBase
    {
        #region Properties and Members
        protected int _IntParam = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int IntParam
        {
            get { return _IntParam; }
            set { _IntParam = value; }
        }

        protected int _IntParam2 = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int IntParam2
        {
            get { return _IntParam2; }
            set { _IntParam2 = value; }
        }

        protected string _CharParam = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string CharParam
        {
            get { return _CharParam; }
            set { _CharParam = value; }
        }

        protected string _ControlEdit1 = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string ControlEdit1
        {
            get { return _ControlEdit1; }
            set { _ControlEdit1 = value.ToUpper(); }
        }

        protected string _ControlEdit2 = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string ControlEdit2
        {
            get { return _ControlEdit2; }
            set { _ControlEdit2 = value.ToUpper(); }
        }

        protected string _ControlEdit3 = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string ControlEdit3
        {
            get { return _ControlEdit3; }
            set { _ControlEdit3 = value.ToUpper(); }
        }
        #endregion

        public TEditDependency()
            : base()
        {
        }

        // For some strange reason, we can't make Parent a public member because it screws up the XML Serializer!
        // (That's really strange since we're not making it a property. But now we have to provide public access 
        // functions, so now its more like property than it was before!)
        protected ReinoControls.TextBoxBehavior _Parent = null;
        public ReinoControls.TextBoxBehavior GetParent()
        {
            return _Parent;
        }
        public void SetParent(ReinoControls.TextBoxBehavior pValue)
        {
            _Parent = pValue;
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION

        // For some strange reason, we can't make public members of type ReinoTextBox because it screws up the XML Serializer!
        // _ControlEdit1Obj will be a reference to the actual edit control
        // (Based on the name from configuration stored in _ControlEdit1)
        protected ReinoControls.TextBoxBehavior _ControlEdit1Obj = null;
        public ReinoControls.TextBoxBehavior GetControlEdit1Obj()
        {
            return _ControlEdit1Obj;
        }
        public void SetControlEdit1Obj(ReinoControls.TextBoxBehavior pValue)
        {
            _ControlEdit1Obj = pValue;
        }

        // For some strange reason, we can't make public members of type ReinoTextBox because it screws up the XML Serializer!
        // _ControlEdit2Obj will be a reference to the actual edit control
        // (Based on the name from configuration stored in _ControlEdit2)
        protected ReinoControls.TextBoxBehavior _ControlEdit2Obj = null;
        public ReinoControls.TextBoxBehavior GetControlEdit2Obj()
        {
            return _ControlEdit2Obj;
        }
        public void SetControlEdit2Obj(ReinoControls.TextBoxBehavior pValue)
        {
            _ControlEdit2Obj = pValue;
        }

        // For some strange reason, we can't make public members of type ReinoTextBox because it screws up the XML Serializer!
        // _ControlEdit3Obj will be a reference to the actual edit control
        // (Based on the name from configuration stored in _ControlEdit3)
        protected ReinoControls.TextBoxBehavior _ControlEdit3Obj = null;
        public ReinoControls.TextBoxBehavior GetControlEdit3Obj()
        {
            return _ControlEdit3Obj;
        }
        public void SetControlEdit3Obj(ReinoControls.TextBoxBehavior pValue)
        {
            _ControlEdit3Obj = pValue;
        }

        protected int CheckForControlEdit1()
        {
            if ((_ControlEdit1 != "") && (_ControlEdit1Obj != null))
                return 0;
            string loLine0 = _Parent._CfgCtrl._Name + "->" + this._Name;
            if (ReinoControls.TextBoxBehavior.OnStandardMessageBox != null)
            {
                ReinoControls.TextBoxBehavior.OnStandardMessageBox(
                    loLine0 + "\r\n" + "Missing ControlEdit1", "");
            }
            else
            {
                MessageBox.Show(loLine0 + "\n" + "Missing ControlEdit1");
            }
            return 1;
        }

        protected int CheckForControlEdit2()
        {
            if ((_ControlEdit2 != "") && (_ControlEdit2Obj != null))
                return 0;
            string loLine0 = _Parent._CfgCtrl._Name + "->" + this._Name;
            if (ReinoControls.TextBoxBehavior.OnStandardMessageBox != null)
            {
                ReinoControls.TextBoxBehavior.OnStandardMessageBox(
                    loLine0 + "\r\n" + "Missing ControlEdit2", "");
            }
            else
            {
                MessageBox.Show(loLine0 + "\n" + "Missing ControlEdit2");
            }
            return 1;
        }

        protected int CheckForControlEdit3()
        {
            if ((_ControlEdit3 != "") && (_ControlEdit3Obj != null))
                return 0;
            string loLine0 = _Parent._CfgCtrl._Name + "->" + this._Name;
            if (ReinoControls.TextBoxBehavior.OnStandardMessageBox != null)
            {
                ReinoControls.TextBoxBehavior.OnStandardMessageBox(
                    loLine0 + "\r\n" + "Missing ControlEdit3", "");
            }
            else
            {
                MessageBox.Show(loLine0 + "\n" + "Missing ControlEdit3");
            }
            return 1;
        }

        public void ResolveObjectReferences()
        {
            // If there is a ControlEdit1 name specified, try to resolve reference to actual object
            if ((_ControlEdit1 != "") && (_ControlEdit1Obj == null))
            {
                // Find ControlEdit1 by name
                TextBoxBehavior loControlEdit = this._Parent.GetTextBoxBehaviorByName(_ControlEdit1);
                if (loControlEdit != null) 
                {
                    // Keep reference to the edit control object
                    _ControlEdit1Obj = loControlEdit;
                    // Now add ourself as a dependent of the ControlEdit if its not ourself
                    if (loControlEdit != this._Parent)
                        loControlEdit.Dependents.Add(this._Parent);
                }
            }

            // If there is a ControlEdit2 name specified, try to resolve reference to actual object
            if ((_ControlEdit2 != "") && (_ControlEdit2Obj == null))
            {
                // Find ControlEdit2 by name
                TextBoxBehavior loControlEdit = this._Parent.GetTextBoxBehaviorByName(_ControlEdit2);
                if (loControlEdit != null)
                {
                    // Keep reference to the edit control object
                    _ControlEdit2Obj = loControlEdit;
                    // Now add ourself as a dependent of the ControlEdit if its not ourself
                    if (loControlEdit != this._Parent)
                        loControlEdit.Dependents.Add(this._Parent);
                }
            }

            // If there is a ControlEdit3 name specified, try to resolve reference to actual object
            if ((_ControlEdit3 != "") && (_ControlEdit3Obj == null))
            {
                // Find ControlEdit3 by name
                TextBoxBehavior loControlEdit = this._Parent.GetTextBoxBehaviorByName(_ControlEdit3);
                if (loControlEdit != null)
                {
                    // Keep reference to the edit control object
                    _ControlEdit3Obj = loControlEdit;
                    // Now add ourself as a dependent of the ControlEdit if its not ourself
                    if (loControlEdit != this._Parent)
                        loControlEdit.Dependents.Add(this._Parent);
                }
            }
        }

        protected ReinoControls.TextBoxBehavior GetControlEdit1()
        {
            // If we don't have a reference to the object yet, try to resolve it first
            if (_ControlEdit1Obj == null)
                ResolveObjectReferences();
            // Now return the ControlEdit object 
            return _ControlEdit1Obj;
        }

        protected ReinoControls.TextBoxBehavior GetControlEdit2()
        {
            // If we don't have a reference to the object yet, try to find by name
            if (_ControlEdit2Obj == null)
                ResolveObjectReferences();
            // Now return the ControlEdit object 
            return _ControlEdit2Obj;
        }

        protected ReinoControls.TextBoxBehavior GetControlEdit3()
        {
            // If we don't have a reference to the object yet, try to find by name
            if (_ControlEdit3Obj == null)
                ResolveObjectReferences();
            // Now return the ControlEdit object 
            return _ControlEdit3Obj;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TEditCondition.
    /// </summary>
    public class TEditCondition : Reino.ClientConfig.TEditDependency
    {
        public enum EditConditionType
        {
            rcIfFieldMatchesValue,
            rcIfFieldContainsValue,
            rcIfFieldContainsFieldName,
            rcIfFieldIsListItem,
            rcIfListIsPopulated,
            rcFieldValueInRange,
            rcFieldValuesMatch,
            rcFieldIsProtected
        }

        #region Properties and Members
        protected EditConditionType _ConditionType = EditConditionType.rcIfFieldMatchesValue;
        [System.ComponentModel.DefaultValue(EditConditionType.rcIfFieldMatchesValue)] // This prevents serialization of default values
        public EditConditionType ConditionType
        {
            get { return _ConditionType; }
            set { _ConditionType = value; }
        }
        #endregion

        public TEditCondition()
            : base()
        {
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        /*
		 * TEditCondition::DateFieldValueInRange()
		 *
		 * Evaluates the FieldValueInRange condition for a date field.
		 * 
		 * Ranges are relative to the current date.
		 * - fIntParam: Number of days from current date to be greater than
		 * - fIntParam2: Number of days from current date to be less than
		 * Example 1: Date Must be in Future
		 * - fIntParam = 1, fIntParam2 = 0 (ignore it)
		 * Example 2: Date Must be in Past
		 * - fIntParam = 0 (ingnore it), fIntParam2 = -1 
		 */
        private bool DateFieldValueInRange(TextBoxBehavior iControlEdit)
        {
            DateTime loCurrentDate = DateTime.Today;
            DateTime loFieldDate = DateTime.Today;
            System.TimeSpan loDateDif;

            ReinoControls.TextBoxBehavior.DateStringToOSDate(iControlEdit.GetEditMask(),
                iControlEdit.EditBuffer, ref loFieldDate);

            // difference between entered date & current date
            loDateDif = loFieldDate - DateTime.Today;

            // 1st check: have a low && number < low;
            if ((_IntParam != 0) && (loDateDif.Days <= IntParam))
                return false; // number is less than lower limit! return 0

            // 2nd check: have a high && number > high;
            if ((_IntParam2 != 0) && (loDateDif.Days >= _IntParam2))
                return false; // number is greater than upper limit! return 0

            // passed both checks, return 1.
            return true;
        }

        /*
         * TEditCondition::EvaluateCondition()
         * 
         * Evaluates conditions based on the condition type.  This should have been implemented as separate classes.  
         * Instead we are faced with a switch statement that will grow as new evaluations are added.
         */
        public bool EvaluateCondition()
        {
            Int64 loTmpNum = 0;
            TextBoxBehavior loControlEdit;

            switch (_ConditionType)
            {
                case TEditCondition.EditConditionType.rcIfFieldMatchesValue:
                    if (CheckForControlEdit1() != 0)
                        return false;
                    if (_CharParam == "" && GetControlEdit1() != null)
                        return GetControlEdit1().FieldIsBlank();
                    return _CharParam.Equals(GetControlEdit1().EditBuffer);

                case TEditCondition.EditConditionType.rcIfFieldContainsValue:
                    if (_ControlEdit1 == "" || _CharParam == "")
                        return false;
                    return GetControlEdit1().EditBuffer.IndexOf(_CharParam) >= 0;

                case TEditCondition.EditConditionType.rcIfFieldContainsFieldName:
                    if (_ControlEdit1 == "" || _Parent == null)
                        return false;
                    // Lets do a case-insensitive comparison for this one
                    return GetControlEdit1().EditBuffer.ToUpper().IndexOf(_Parent._CfgCtrl._Name) >= 0;
                case TEditCondition.EditConditionType.rcIfFieldIsListItem:
                    if (_ControlEdit1 == "")
                        return false;

                    GetControlEdit1().ResynchListNdx();
                    return (GetControlEdit1().GetListNdx() != -1);

                case TEditCondition.EditConditionType.rcIfListIsPopulated:
                    if (_ControlEdit1 == "")   // Is there a controledit, if not use the parent
                        loControlEdit = _Parent;
                    else
                        loControlEdit = GetControlEdit1();
                    // If there is no list, then its not populated
                    if (loControlEdit.ListSourceTable == null)
                        return false;
                    else
                        return loControlEdit.ListSourceTable.GetRecCount() > 0;

                case TEditCondition.EditConditionType.rcFieldValueInRange:
                    if (_ControlEdit1 == "")   // Is there a controledit, if not use the parent
                        loControlEdit = _Parent;
                    else
                        loControlEdit = GetControlEdit1();

                    if (loControlEdit.GetFieldType() == TEditFieldType.efDate)
                        return DateFieldValueInRange(loControlEdit);

                    ReinoControls.TextBoxBehavior.StrTollInt(loControlEdit.EditBuffer, ref loTmpNum);

                    // 1st check: have a low && number < low;
                    if ((_IntParam > 0) && (loTmpNum < _IntParam))
                        return false; // number is less than lower limit! return 0

                    // 2nd check: have a high && number > high;
                    if ((_IntParam2 > 0) && (loTmpNum > _IntParam2))
                        return false; // number is greater than upper limit! return 0

                    // passed both checks, return 1.
                    return true;

                case TEditCondition.EditConditionType.rcFieldValuesMatch:
                    // two fields' values match. Compare controlEdit1 & 2. If one is blank, use parent edit.
                    if (_ControlEdit1 != "" && _ControlEdit2 != "") // both control1 & 2 exist, so compare them.
                        return GetControlEdit1().EditBuffer.Equals(GetControlEdit2().EditBuffer);

                    if (_ControlEdit1 != "") // only control 1 exists (2 doesn't), so compare to parent
                        return GetControlEdit1().EditBuffer.Equals(_Parent.EditBuffer);

                    if (_ControlEdit2 != "") // only control 2 exiss (1 doesn't), so compare to parent
                        return this.GetControlEdit2().EditBuffer.Equals(_Parent.EditBuffer);

                    // control 1 & 2 don't exist, so compare parent to itself
                    return true;

                case TEditCondition.EditConditionType.rcFieldIsProtected:
                    // two fields' values match. Compare controlEdit1 & 2. If one is blank, use parent edit.
                    if (_ControlEdit1 != "")
                        return GetControlEdit1()._CfgCtrl.IsProtected; // !GetControlEdit1().Enabled
                    else
                        return _Parent._CfgCtrl.IsProtected; // !_Parent.Enabled

                default: return true;
            }
        }
#endif
        #endregion
    }

    public class TConditionEvaluation
    {
        protected bool _Evalutation = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool Evaluation
        {
            get { return _Evalutation; }
            set { _Evalutation = value; }
        }

        protected string _ConditionName = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string ConditionName
        {
            get { return _ConditionName; }
            set { _ConditionName = value.ToUpper(); } // Must be uppercase to match the name of an TEditCondition
        }
    }

    /// <summary>
    /// Summary description for TEditRestriction.
    /// </summary>
    public class TEditRestriction : Reino.ClientConfig.TEditDependency
    {
        #region Properties and Members
        protected List<TConditionEvaluation> _Conditions;
        /// <summary>
        /// A collection of TConditionEvaluation objects
        /// </summary>
        public List<TConditionEvaluation> Conditions
        {
            get { return _Conditions; }
            set { _Conditions = value; }
        }

        protected bool _ConditionsANDed = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool ConditionsANDed
        {
            get { return _ConditionsANDed; }
            set { _ConditionsANDed = value; }
        }

        protected bool _ActiveOnFormInit = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool ActiveOnFormInit
        {
            get { return _ActiveOnFormInit; }
            set { _ActiveOnFormInit = value; }
        }

        protected bool _ActiveOnBeforeEdit = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool ActiveOnBeforeEdit
        {
            get { return _ActiveOnBeforeEdit; }
            set { _ActiveOnBeforeEdit = value; }
        }

        protected bool _ActiveOnDataChanged = false;
        // This might have different defaults in descendants, so don't declare default here because 
        // it will screw up serialization/deserialization
        /*[System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values*/
        public bool ActiveOnDataChanged
        {
            get { return _ActiveOnDataChanged; }
            set { _ActiveOnDataChanged = value; }
        }

        protected bool _ActiveOnValidate = false;
        // This might have different defaults in descendants, so don't declare default here because 
        // it will screw up serialization/deserialization
        /*[System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values*/
        public bool ActiveOnValidate
        {
            get { return _ActiveOnValidate; }
            set { _ActiveOnValidate = value; }
        }

        protected bool _ActiveOnParentFieldExit = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool ActiveOnParentFieldExit
        {
            get { return _ActiveOnParentFieldExit; }
            set { _ActiveOnParentFieldExit = value; }
        }

        protected bool _ActiveOnAncestorFieldExit = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool ActiveOnAncestorFieldExit
        {
            get { return _ActiveOnAncestorFieldExit; }
            set { _ActiveOnAncestorFieldExit = value; }
        }

        protected bool _ActiveOnParentDataChanged = false;
        // This might have different defaults in descendants, so don't declare default here because 
        // it will screw up serialization/deserialization
        /*[System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values*/
        public bool ActiveOnParentDataChanged
        {
            get { return _ActiveOnParentDataChanged; }
            set { _ActiveOnParentDataChanged = value; }
        }

        protected bool _ActiveOnFirstEditFocus = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool ActiveOnFirstEditFocus
        {
            get { return _ActiveOnFirstEditFocus; }
            set { _ActiveOnFirstEditFocus = value; }
        }

        protected bool _ActiveOnNewEntry = true;
        [System.ComponentModel.DefaultValue(true)] // This prevents serialization of default values
        public bool ActiveOnNewEntry
        {
            get { return _ActiveOnNewEntry; }
            set { _ActiveOnNewEntry = value; }
        }

        protected bool _ActiveOnCorrection = true;
        // This might have different defaults in descendants, so don't declare default here because 
        // it will screw up serialization/deserialization
        /*[System.ComponentModel.DefaultValue(true)] // This prevents serialization of default values*/
        public bool ActiveOnCorrection
        {
            get { return _ActiveOnCorrection; }
            set { _ActiveOnCorrection = value; }
        }

        protected bool _ActiveOnIssueMore = true;
        // This might have different defaults in descendants, so don't declare default here because 
        // it will screw up serialization/deserialization
        /*[System.ComponentModel.DefaultValue(true)] // This prevents serialization of default values*/
        public bool ActiveOnIssueMore
        {
            get { return _ActiveOnIssueMore; }
            set { _ActiveOnIssueMore = value; }
        }

        protected bool _ActiveOnCancel = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool ActiveOnCancel
        {
            get { return _ActiveOnCancel; }
            set { _ActiveOnCancel = value; }
        }

        protected bool _ActiveOnView = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool ActiveOnView
        {
            get { return _ActiveOnView; }
            set { _ActiveOnView = value; }
        }

        protected bool _ActiveOnReissue = true;
        // This might have different defaults in descendants, so don't declare default here because 
        // it will screw up serialization/deserialization
        /*[System.ComponentModel.DefaultValue(true)] // This prevents serialization of default values*/
        public bool ActiveOnReissue
        {
            get { return _ActiveOnReissue; }
            set { _ActiveOnReissue = value; }
        }

        protected bool _ActiveOnContinuance = true;
        // This might have different defaults in descendants, so don't declare default here because 
        // it will screw up serialization/deserialization
        /*[System.ComponentModel.DefaultValue(true)] // This prevents serialization of default values*/
        public bool ActiveOnContinuance
        {
            get { return _ActiveOnContinuance; }
            set { _ActiveOnContinuance = value; }
        }

        protected bool _DisabledIfNoChange = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool DisabledIfNoChange
        {
            get { return _DisabledIfNoChange; }
            set { _DisabledIfNoChange = value; }
        }

        protected bool _Overrideable = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool Overrideable
        {
            get { return _Overrideable; }
            set { _Overrideable = value; }
        }
        #endregion

        public TEditRestriction()
            : base()
        {
            Conditions = new List<TConditionEvaluation>();
        }

        #region Implementation code at base-level of all TEditRestriction objects
#if USE_DEFN_IMPLEMENTATION
        public static string glUserName = "";  // Global variable for the current user.
        private bool fConditionsChanged = false;
        private int fEnforcementAttributes = 0;

        public virtual bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            return false;
        }

        public virtual bool ConditionsChangedSinceLastCheck()
        {
            return fConditionsChanged;
        }

        public ETrueFalseIgnore RestrictionActiveOnEvent(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            ETrueFalseIgnore loResult = ETrueFalseIgnore.tfiIgnore;
            // if the event is one of "ParentDataChanged, DataChanged, AncestorDataChanged" then we will
            // reset the fConditionsChanged flag 
            if ((iNotifyEvent & (EditRestrictionConsts.dneParentDataChanged | EditRestrictionConsts.dneDataChanged)) > 0)
                fConditionsChanged = true;

            // Build bit-level patterns to represent the active events, modes and attributes
            int fActiveNotifyEvents = 0;
            int fActiveFormEditModes = 0;

            if (ActiveOnAncestorFieldExit == true)
                fActiveNotifyEvents |= EditRestrictionConsts.dneAncestorFieldExit;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneAncestorFieldExit;
            if (ActiveOnBeforeEdit == true)
                fActiveNotifyEvents |= EditRestrictionConsts.dneBeforeEdit;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneBeforeEdit;
            if (ActiveOnCancel == true)
                fActiveFormEditModes |= EditRestrictionConsts.femCancelAttr;
            else
                fActiveFormEditModes &= ~EditRestrictionConsts.femCancelAttr;
            if (ActiveOnContinuance == true)
                fActiveFormEditModes |= EditRestrictionConsts.femContinuance;
            else
                fActiveFormEditModes &= ~EditRestrictionConsts.femContinuance;
            if (ActiveOnCorrection == true)
                fActiveFormEditModes |= EditRestrictionConsts.femCorrectionAttr;
            else
                fActiveFormEditModes &= ~EditRestrictionConsts.femCorrectionAttr;
            if (ActiveOnDataChanged == true)
                fActiveNotifyEvents |= EditRestrictionConsts.dneDataChanged;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneDataChanged;
            if (ActiveOnFirstEditFocus == true)
                fActiveNotifyEvents |= EditRestrictionConsts.dneFirstEditFocus;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneFirstEditFocus;
            if (ActiveOnFormInit == true)
                fActiveNotifyEvents |= EditRestrictionConsts.dneFormInit;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneFormInit;
            if (ActiveOnIssueMore == true)
                fActiveFormEditModes |= EditRestrictionConsts.femIssueMoreAttr;
            else
                fActiveFormEditModes &= ~EditRestrictionConsts.femIssueMoreAttr;
            if (ActiveOnNewEntry == true)
                fActiveFormEditModes |= EditRestrictionConsts.femNewEntry;
            else
                fActiveFormEditModes &= ~EditRestrictionConsts.femNewEntry;
            if (ActiveOnParentDataChanged == true)
                fActiveNotifyEvents |= EditRestrictionConsts.dneParentDataChanged;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneParentDataChanged;
            if (ActiveOnParentFieldExit == true)
                fActiveNotifyEvents |= EditRestrictionConsts.dneParentFieldExit;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneParentFieldExit;
            if (ActiveOnReissue == true)
                fActiveFormEditModes |= EditRestrictionConsts.femReissue;
            else
                fActiveFormEditModes &= ~EditRestrictionConsts.femReissue;
            if (ActiveOnValidate == true)
                fActiveNotifyEvents |= EditRestrictionConsts.dneValidate;
            else
                fActiveNotifyEvents &= ~EditRestrictionConsts.dneValidate;
            if (ActiveOnView == true)
                fActiveFormEditModes |= EditRestrictionConsts.femView;
            else
                fActiveFormEditModes &= ~EditRestrictionConsts.femView;

            if (DisabledIfNoChange == true)
                fEnforcementAttributes |= EditRestrictionConsts.ecDisabledIfNoChange;
            else
                fEnforcementAttributes &= ~EditRestrictionConsts.ecDisabledIfNoChange;
            if (Overrideable == true)
                fEnforcementAttributes |= EditRestrictionConsts.ecOverRideable;
            else
                fEnforcementAttributes &= ~EditRestrictionConsts.ecOverRideable;

            // if restriction is inactive for this event, return FALSE

            // Active on no events means active on ALL events
            if ((fActiveNotifyEvents > 0) && ((iNotifyEvent & fActiveNotifyEvents) > 0) == false)
            {
                if (this is TER_Protected)
                {
                    // JLA 2008.10.09 -- This special condition was messing up IssueDate, Time, IssueNo, etc. when you click "Issue More" and 
                    // current record doesn't validate.  Was this special condition of return false during validations put in for a good reason?
                    if ((iNotifyEvent == EditRestrictionConsts.dneValidate) && (this.ActiveOnValidate == false))
                        return ETrueFalseIgnore.tfiIgnore; //return ETrueFalseIgnore.tfiFalse;
                    else
                        return ETrueFalseIgnore.tfiIgnore;
                }
                else
                {
                    return ETrueFalseIgnore.tfiIgnore;
                }
            }

            if ((iFormEditMode & fActiveFormEditModes) == 0)
                return ETrueFalseIgnore.tfiIgnore;

            if (((fEnforcementAttributes & EditRestrictionConsts.ecDisabledIfNoChange) > 0) && (!ConditionsChangedSinceLastCheck()))
                return ETrueFalseIgnore.tfiIgnore;

            // if no associated conditions, return TRUE 
            fConditionsChanged = false;

            if (Conditions.Count == 0)
            {
                if ((fActiveNotifyEvents == 0) || (((iNotifyEvent & (EditRestrictionConsts.dneParentFieldExit | EditRestrictionConsts.dneParentDataChanged)) == 0)))
                    return ETrueFalseIgnore.tfiTrue;
                if ((iParentBehavior._CfgCtrl._Name != ControlEdit1) && 
                    (iParentBehavior._CfgCtrl._Name != ControlEdit2) && 
                    (iParentBehavior._CfgCtrl._Name != ControlEdit3))
                    return ETrueFalseIgnore.tfiIgnore;
                return ETrueFalseIgnore.tfiTrue;
            }

            // if the event for this restriction is ParentFieldExit or ParentDataChanged, make sure
            // the field causing the event is a parent in ONE of the conditions or the parent of
            // the restriction itself
            if ((fActiveNotifyEvents > 0) && (((iNotifyEvent & (EditRestrictionConsts.dneParentFieldExit | EditRestrictionConsts.dneParentDataChanged)) > 0)))
            {
                bool loOneParentCausedEvent = false;

                // is the the parent of this restriction??
                if ((iParentBehavior._CfgCtrl._Name.Equals(ControlEdit1)) ||
                    (iParentBehavior._CfgCtrl._Name.Equals(ControlEdit2)) ||
                    (iParentBehavior._CfgCtrl._Name.Equals(ControlEdit3)))
                    loOneParentCausedEvent = true;
                else // or, perhaps, a parent of one of our conditions
                {
                    // JLA (3/2/07): This code was looking at parent edit conditions instead of
                    //    edit conditions specified by this TEditRestriction. Problem was exposed
                    //    by configuration for Seattle demo
                    /*
                    foreach (TEditCondition NextExitCondition in _Parent.EditConditions)
                    {
                        if ((iParentBehavior.CfgCtrl.Name.Equals(NextExitCondition.ControlEdit1)) ||
                        (iParentBehavior.CfgCtrl.Equals(NextExitCondition.ControlEdit2)) ||
                        (iParentBehavior.CfgCtrl.Equals(NextExitCondition.ControlEdit3)))
                        {
                            loOneParentCausedEvent = true;
                            break;
                        }
                    }
                    */

                    // JLA (3/2/07): Corrected version of code to fix problem exposed by Seattle demo
                    foreach (TConditionEvaluation NextEvaluation in this.Conditions)
                    {
                        // Find parent's condition that matches name from condition evaluation object
                        TEditCondition AssocCondition = null;
                        foreach (TEditCondition NextExitCondition in _Parent.EditConditions)
                        {
                            if (NextExitCondition.Name == NextEvaluation.ConditionName)
                            {
                                AssocCondition = NextExitCondition;
                                break;
                            }
                        }

                        // Did we find an associated TEditCondition?
                        if (AssocCondition != null)
                        {
                            if ((iParentBehavior._CfgCtrl._Name.Equals(AssocCondition.ControlEdit1)) ||
                            (iParentBehavior._CfgCtrl.Equals(AssocCondition.ControlEdit2)) ||
                            (iParentBehavior._CfgCtrl.Equals(AssocCondition.ControlEdit3)))
                            {
                                loOneParentCausedEvent = true;
                                break;
                            }
                        }
                    }
                }

                // The field causing this event is not a parent, therefore we can ignore this restriction.
                if (!loOneParentCausedEvent)
                    return ETrueFalseIgnore.tfiIgnore;
            }

            foreach (TConditionEvaluation NextCondition in Conditions)
            {
                // Find the implemented version of the edit condition (Match by name -- case insensitive)
                TObjBasePredicate predicate = new TObjBasePredicate(NextCondition.ConditionName);
                Reino.ClientConfig.TEditCondition loEditConditionImp =
                    this._Parent.EditConditions.Find(predicate.CompareByName_CaseInsensitive);

                // there is an associated condition, return TRUE if it evaluates to what this restriction requires. 
                if (loEditConditionImp != null)
                {
                    if (loEditConditionImp.EvaluateCondition() == NextCondition.Evaluation)
                    {
                        // this condition evaluated true, so set our return value to true 
                        loResult = ETrueFalseIgnore.tfiTrue;
                    }
                    else
                    {
                        // this condition evaluated false.  If all the conditions are being AND'd together,
                        // then short-circuit the evaluation and return FALSE now. Otherwise everything is being
                        // OR'd, and we have to trudg through all the conditions 
                        if (ConditionsANDed)
                            return ETrueFalseIgnore.tfiFalse;
                        // if no conditions have evaluated to TRUE, set our return value to FALSE, thus overriding
                        // the default IGNORE result. 
                        if (loResult == ETrueFalseIgnore.tfiIgnore)
                            loResult = ETrueFalseIgnore.tfiFalse;
                    }
                }
            }
            return loResult;
        }

        public void SetDisabledIfNoChange(bool iValue)
        {
            if (iValue == true)
                fEnforcementAttributes |= EditRestrictionConsts.ecDisabledIfNoChange;
            else
                fEnforcementAttributes &= ~EditRestrictionConsts.ecDisabledIfNoChange;
        }
#endif
        #endregion
    }
    public delegate void RestrictionForcesDisplayRebuild();

    /// <summary>
    /// Summary description for TER_Protected.
    /// </summary>
    public class TER_Protected : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        public event RestrictionForcesDisplayRebuild OnRestrictionForcesDisplayRebuild = null;
        #endregion

        public TER_Protected()
            : base()
        {
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) == ETrueFalseIgnore.tfiIgnore)
                return false;

            // Also set "CurrentlyProtected" attribute in behavior object
            // and let application know it should rebuild the display if the state has changed
            bool loCurrentlyProtected = (loEnforce != ETrueFalseIgnore.tfiFalse);
            if (_Parent._CfgCtrl != null)
            {
                if (_Parent._CfgCtrl.IsProtected != loCurrentlyProtected)
                {
                    _Parent._CfgCtrl.IsProtected = loCurrentlyProtected;

                    // JLA 2008.10.08
                    // If there is an associated control, set its enabled state
                    if (_Parent.EditCtrl != null)
                        _Parent.EditCtrl.Enabled = (loEnforce == ETrueFalseIgnore.tfiFalse);

                    if (OnRestrictionForcesDisplayRebuild != null)
                        OnRestrictionForcesDisplayRebuild();
                }
                else
                {
                    // JLA 2008.10.08
                    // If there is an associated control, set its enabled state
                    if (_Parent.EditCtrl != null)
                        _Parent.EditCtrl.Enabled = (loEnforce == ETrueFalseIgnore.tfiFalse);
                }
            }
            else
            {
                // JLA 2008.10.08
                // If there is an associated control, set its enabled state
                if (_Parent.EditCtrl != null)
                    _Parent.EditCtrl.Enabled = (loEnforce == ETrueFalseIgnore.tfiFalse);
            }
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_Hidden.
    /// </summary>
    public class TER_Hidden : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        public event RestrictionForcesDisplayRebuild OnRestrictionForcesDisplayRebuild = null;
        #endregion

        public TER_Hidden()
            : base()
        {
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) == ETrueFalseIgnore.tfiIgnore)
                return false;

            // If there is an associated control, set its visible state
            if (_Parent.EditCtrl != null)
                _Parent.EditCtrl.Visible = (loEnforce == ETrueFalseIgnore.tfiFalse);

            // Also set "CurrentlyHidden" attribute in behavior object
            // and let application know it should rebuild the display if the state has changed
            bool loCurrentlyHidden = (loEnforce != ETrueFalseIgnore.tfiFalse);
            if (_Parent._CfgCtrl.IsHidden != loCurrentlyHidden)
            {
                _Parent._CfgCtrl.IsHidden = loCurrentlyHidden;
                if (OnRestrictionForcesDisplayRebuild != null)
                    OnRestrictionForcesDisplayRebuild();
            }
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_ChildList.
    /// </summary>
    public class TER_ChildList : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        private Reino.ClientConfig.TTTable _ListSourceTable = null;
        public Reino.ClientConfig.TTTable ListSourceTable
        {
            get { return _ListSourceTable; }
            set { _ListSourceTable = value; }
        }

        private string _ListSourceFieldName = "";
        public string ListSourceFieldName
        {
            get { return _ListSourceFieldName; }
            set { _ListSourceFieldName = value; }
        }

        private int _CachedFieldIndex = -1;
        private Reino.ClientConfig.TTTable _CachedListSourceTable = null;
        #endregion

        public TER_ChildList()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnParentDataChanged = true;
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) != ETrueFalseIgnore.tfiTrue)
            {
                // We're not active, so make sure parent is using natural list!
                if ((_Parent.ListSourceTable != _Parent.NaturalListSourceTable) ||
                    (_Parent.ListSourceFieldName != _Parent.NaturalListSourceFieldName))
                {
                    _Parent.ListSourceTable = _Parent.NaturalListSourceTable;
                    _Parent.ListSourceFieldName = _Parent.NaturalListSourceFieldName;

                    // This is already done for us by setting ListSourceTable property
                    /*
                    if (_Parent.ListSourceTable != null)
                        _Parent.ListItemCount = _Parent.ListSourceTable.GetRecCount();
                    else
                        _Parent.ListItemCount = 0;
                    _Parent.ListItemCache.Clear();
                    */
                    
                    // If there is a listbox, make sure it gets updated
                    if (_Parent.ListBox != null)
                        _Parent.ListBox.RefreshItems(true);
                }
                return false;
            }

            // Restriction is active, so attach control to our list instead of the control's natural list
            if ((_Parent.ListSourceTable != this.ListSourceTable) ||
                (_Parent.ListSourceFieldName != this.ListSourceFieldName))
            {
                _Parent.ListSourceTable = this.ListSourceTable;
                _Parent.ListSourceFieldName = this.ListSourceFieldName;

                // This is already done for us by setting ListSourceTable property
                /*
                if (_Parent.ListSourceTable != null)
                    _Parent.ListItemCount = _Parent.ListSourceTable.GetRecCount();
                else
                    _Parent.ListItemCount = 0;
                _Parent.ListItemCache.Clear();
                 * */

                // If there is a listbox, make sure it gets updated
                if (_Parent.ListBox != null)
                    _Parent.ListBox.RefreshItems(true);
            }

            // DEBUG: MFC Code
            //  fParent->SetEditBufferAndPaint(  ((TEditField *)fControlEdit1)->GetListTable()->GetFormattedFieldData(fCharParam, fParent->GetEditMask()) );


            // Avoid field lookup by name if we have a field index and the source table is the same as previous
            if ((_CachedListSourceTable != null) && 
                (_CachedListSourceTable == GetControlEdit1().NaturalListSourceTable/*ListSourceTable*/) &&
                (_CachedFieldIndex != -1))
            {
                // set the edit buffer to the value of the current list item contained in the control fields table 
                _Parent.SetEditBufferAndPaint(ReinoControls.TextBoxBehavior.GetFieldDataFromTable(
                    _CachedListSourceTable, _CachedFieldIndex, GetControlEdit1().GetForcedNaturalNdx()/*GetListNdx()*/,
                    _Parent.GetEditMask()), true/*false*/);
                //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            }
            else
            {
                // Somethings different or not initialized, so we need to resolved field index first
                _CachedListSourceTable = GetControlEdit1().NaturalListSourceTable/*ListSourceTable*/;
                _CachedFieldIndex = ReinoControls.TextBoxBehavior.GetFieldIndexForTable(_CachedListSourceTable, CharParam);
                // set the edit buffer to the value of the current list item contained in the control fields table 
                _Parent.SetEditBufferAndPaint(ReinoControls.TextBoxBehavior.GetFieldDataFromTable(
                    _CachedListSourceTable, _CachedFieldIndex, GetControlEdit1().GetForcedNaturalNdx()/*GetListNdx()*/, 
                    _Parent.GetEditMask()), true/*false*/);
                //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            }
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_ListItemsOnly.
    /// </summary>
    public class TER_ListItemsOnly : Reino.ClientConfig.TEditRestriction
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
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) == ETrueFalseIgnore.tfiIgnore)
                return false;

            // If parent has "RelaxListOnlyRestriction" enabled, then there is not need to process this restriction.
            // However we will only allow this to happen if the control is not currently user-editable
            // (Note: This is used exclusively for the benefit of creating new note records)
            if (_Parent.RelaxListOnlyRestriction == true)
            {
                if ((_Parent._CfgCtrl.IsEnabled == false) ||
                    (_Parent._CfgCtrl.IsProtected == true) ||
                    (_Parent._CfgCtrl.IsHidden == true))
                {
                    return false;
                }
            }

            if ((loEnforce == ETrueFalseIgnore.tfiTrue) &&
                (!(_Parent.FieldIsBlank())))
            {
                // If fails with current index, let's force a resync and try again
                if (_Parent.GetListNdx() < 0)
                    _Parent.ResynchListNdx();

                if (_Parent.GetListNdx() < 0)
                {
                    // If we're not currently pointed to the "normal" table, lets see if the
                    // item is valid in the "normal" table (Could happen from hotsheet match?)
                    if (_Parent.ListSourceTable != _Parent.NaturalListSourceTable)
                    {
                        if (_Parent.GetForcedNaturalNdx() < 0)
                            return true;
                        else
                            return false;
                    }
                    else
                        return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_ForceLiteral.
    /// </summary>
    public class TER_ForceLiteral : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ForceLiteral()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnCorrection = false;
            ActiveOnReissue = false;
            ActiveOnContinuance = false;
            ActiveOnIssueMore = false;
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) != ETrueFalseIgnore.tfiTrue)
                return false;

            _Parent.SetEditBufferAndPaint(CharParam, false);
            //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_ForceParentValue.
    /// </summary>
    public class TER_ForceParentValue : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ForceParentValue()
            : base()
        {
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            ETrueFalseIgnore loEnforce;

            if (ControlEdit1 == "")
                return false;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) == ETrueFalseIgnore.tfiIgnore)
                return false;

            if (loEnforce == ETrueFalseIgnore.tfiTrue)
            {
                // try to handle number format differences
                if (_Parent.GetFieldType() == TEditFieldType.efNumeric)
                {
                    string loSrcStr = "";
                    string loDestStr = "";
                    // is source a numeric field?
                    TextBoxBehavior CtrlEdit1 = GetControlEdit1();
                    if (CtrlEdit1.GetFieldType() == TEditFieldType.efNumeric)
                        ReinoControls.TextBoxBehavior.FormatNumberStr(CtrlEdit1.GetText(),
                            CtrlEdit1.GetEditMask(), ref loSrcStr);
                    else
                        loSrcStr = CtrlEdit1.GetText();
                    ReinoControls.TextBoxBehavior.FormatNumberStr(loSrcStr,
                        _Parent.GetEditMask(), ref loDestStr);
                    _Parent.SetEditBufferAndPaint(loDestStr, false);
                    //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
                }
                else
                {
                    _Parent.SetEditBufferAndPaint(GetControlEdit1().GetText(), false);
                    //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
                }
            }
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_ForceListItem.
    /// </summary>
    public class TER_ForceListItem : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ForceListItem()
            : base()
        {
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) == ETrueFalseIgnore.tfiIgnore)
                return false;

            if (loEnforce == ETrueFalseIgnore.tfiTrue)
                _Parent.SetListNdxAndPaint(IntParam);
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_Required.
    /// </summary>
    public class TER_Required : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_Required()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnValidate = true;
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) == ETrueFalseIgnore.tfiIgnore)
                return false;

            if ((loEnforce == ETrueFalseIgnore.tfiTrue) && ((_Parent.FieldIsBlank()) || (_Parent.GetText().Length < IntParam)))
            {
                //Tprintf( "Length of <%s> shorter than required %d.\n", fParent->GetName(), fIntParam );
                return true;
            }
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_Conditions.
    /// </summary>
    public class TER_Conditions : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_Conditions()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnValidate = true;
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            return RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) == ETrueFalseIgnore.tfiTrue;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_ForceCurrentDateTime.
    /// </summary>
    public class TER_ForceCurrentDateTime : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ForceCurrentDateTime()
            : base()
        {
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            string loTmpBuf = "";
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                return false;

            if (_Parent.GetFieldType() == TEditFieldType.efDate)
                ReinoControls.TextBoxBehavior.OSDateToDateString(DateTime.Today.AddDays(IntParam),
                    _Parent.GetEditMask(), ref loTmpBuf);

            if (_Parent.GetFieldType() == TEditFieldType.efTime)
                ReinoControls.TextBoxBehavior.OSTimeToTimeString(DateTime.Now.AddSeconds(IntParam),
                    _Parent.GetEditMask(), ref loTmpBuf);

            _Parent.SetEditBufferAndPaint(loTmpBuf, false);
            //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            return false;
        }
#endif
        #endregion
    }

    public delegate void ListContentsChangedByRestriction(TextBoxBehavior iBehavior);

    /// <summary>
    /// Summary description for TER_ListFilter.
    /// </summary>
    public class TER_ListFilter : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        public event ListContentsChangedByRestriction OnListContentsChangedByRestriction = null;
        #endregion

        public TER_ListFilter()
            : base()
        {
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                return false;

            if (CheckForControlEdit1() != 0)
                return false;

            // Assume the list filtering makes some sort of change to the list
            bool ListFilterMadeChanges = true;
            int FilterResult = 0;

            // add the filter to our field. Remember, "bool StrictlyEnforced" is mapped to fIntParam 
            if (GetControlEdit1().FieldIsBlank() && IntParam == 0)
            {
                // if the field is blank and the filter is not strictly enforced, clear out the filter
                _Parent.ListSourceTable.RemoveFilter(CharParam);
            }
            else
            {
                if (string.Compare(CharParam, "MasterKey", true) == 0)
                {
                    string loListNdxStr = GetControlEdit1().GetListPrimaryKey().ToString();
                    FilterResult = _Parent.ListSourceTable.AddFilter(CharParam, loListNdxStr, /*"-9999"*/"-9999999");
                    // If AddFilter returned -1, then we know nothing has actually changed
                    if (FilterResult == -1)
                        ListFilterMadeChanges = false;
                }
                else
                {
                    if (GetControlEdit1().FieldIsBlank())
                        FilterResult = _Parent.ListSourceTable.AddFilter(CharParam, "", GetControlEdit1().GetEditMask());
                    else
                        FilterResult = _Parent.ListSourceTable.AddFilter(CharParam, GetControlEdit1().EditBuffer, GetControlEdit1().GetEditMask());

                    // If AddFilter returned -1, then we know nothing has actually changed
                    if (FilterResult == -1)
                        ListFilterMadeChanges = false;
                }
            }

            // Don't bother doing anything time-consuming if nothing has actually changed
            if (ListFilterMadeChanges == true)
            {
                // Underlying list items may have changed, so clear any existing cache and update the
                // virtual count of items.
                _Parent.ListItemCache.Clear();
                if (_Parent.GridDisplayCache != null)
                    _Parent.GridDisplayCache.Clear();
                _Parent.ListItemCount = _Parent.ListSourceTable.GetRecCount();

                // Underlying list items may have changed, so refresh the list indexes.
                _Parent.ResynchListNdx();
                // Update margin, dropdown button, and create listbox if necessary
                _Parent.RefreshState();
            }

            // if the current field value doesn't pass the filter, clear List Item no. 
            // if the filter is strictly enforced, clear the field 
            if ((IntParam > 0) && (_Parent.GetListNdx() < 0))
            {
                _Parent.SetEditBufferAndPaint("", false);
                //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            }

            // Do callback function so application knows to respond accordingly
            if ((OnListContentsChangedByRestriction != null) && (ListFilterMadeChanges == true))
                OnListContentsChangedByRestriction(_Parent);
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_ValuesMustMatch.
    /// </summary>
    public class TER_ValuesMustMatch : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ValuesMustMatch()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnValidate = true;
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            ETrueFalseIgnore loEnforce;
            if (((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) != ETrueFalseIgnore.tfiTrue)
                 || ControlEdit1 == "")
                return false;
            if (CheckForControlEdit1() != 0)
                return false;
            return _Parent.GetText().CompareTo(GetControlEdit1().GetText()) != 0;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_Increment.
    /// </summary>
    public class TER_Increment : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        private int _CachedFieldIndex = -1;
        private Reino.ClientConfig.TTTable _CachedListSourceTable = null;
        #endregion

        public TER_Increment()
            : base()
        {
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            ETrueFalseIgnore loEnforce;
            double loIncValue = 0;
            double loCurrValue = 0;
            DateTime loCurrValueDateTime = DateTime.Now;
            string loCurrValueStr = "";

            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) == ETrueFalseIgnore.tfiIgnore)
                return false;

            if (loEnforce == ETrueFalseIgnore.tfiTrue)
            {  // go get the increment value
                // Is there a value in a table column?
                if (ControlEdit2 != "" && (this.GetControlEdit2().ListSourceTable != null))
                {
                    // Avoid field lookup by name if we have a field index and the source table is the same as previous
                    if ((_CachedListSourceTable != null) && (_CachedListSourceTable == GetControlEdit2().ListSourceTable) &&
                        (_CachedFieldIndex != -1))
                    {
                        loCurrValue = Convert.ToDouble(ReinoControls.TextBoxBehavior.GetFieldDataFromTable(
                            _CachedListSourceTable, _CachedFieldIndex, _Parent.GetListNdx(), null));
                    }
                    else
                    {
                        // Somethings different or not initialized, so we need to resolved field index first
                        _CachedListSourceTable = this.GetControlEdit2().ListSourceTable;
                        _CachedFieldIndex = ReinoControls.TextBoxBehavior.GetFieldIndexForTable(this.GetControlEdit2().ListSourceTable,
                            CharParam);

                        loCurrValue = Convert.ToDouble(ReinoControls.TextBoxBehavior.GetFieldDataFromTable(
                            _CachedListSourceTable, _CachedFieldIndex, _Parent.GetListNdx(), null));
                    }
                }

                // is there a value in another field
                if (ControlEdit1 != "")
                    ReinoControls.TextBoxBehavior.MaskStrToDouble(
                        GetControlEdit1().GetText(),
                        GetControlEdit1().GetEditMask(),
                        ref loIncValue);

                // get the total amt, include constant value in fIntParam2
                loIncValue += loCurrValue + IntParam2;
            }

            if (loIncValue == 0) // adding/subtracting by 0 does nothing
                return false;

            // Get current field value
            switch (_Parent.GetFieldType())
            {
                case ReinoControls.TEditFieldType.efDate:
                    {
                        ReinoControls.TextBoxBehavior.DateStringToOSDate(_Parent.GetEditMask(), _Parent.GetText(), ref loCurrValueDateTime);
                        if (IntParam > 0)
                            loCurrValueDateTime.AddDays(loIncValue * -1);
                        else
                            loCurrValueDateTime.AddDays(loIncValue);
                        break;
                    }
                case ReinoControls.TEditFieldType.efTime:
                    {
                        ReinoControls.TextBoxBehavior.TimeStringToOSTime(_Parent.GetEditMask(), _Parent.GetText(), ref loCurrValueDateTime);
                        if (IntParam > 0)
                            loCurrValueDateTime.AddSeconds(loIncValue * -1);
                        else
                            loCurrValueDateTime.AddSeconds(loIncValue);
                        break;
                    }
                default:
                    ReinoControls.TextBoxBehavior.MaskStrToDouble(_Parent.GetText(), _Parent.GetEditMask(), ref loCurrValue);
                    if (IntParam > 0)
                        loCurrValue -= loIncValue;
                    else
                        loCurrValue += loIncValue;
                    break;
            }

            // Get current field value
            switch (_Parent.GetFieldType())
            {
                case ReinoControls.TEditFieldType.efDate:
                    {
                        ReinoControls.TextBoxBehavior.OSDateToDateString(loCurrValueDateTime, _Parent.GetEditMask(), ref loCurrValueStr);
                        break;
                    }
                case ReinoControls.TEditFieldType.efTime:
                    {
                        ReinoControls.TextBoxBehavior.OSTimeToTimeString(loCurrValueDateTime, _Parent.GetEditMask(), ref loCurrValueStr);
                        break;
                    }
                default:
                    loCurrValueStr = string.Format("{0:N}", loCurrValue); //Convert.ToString(loCurrValue);
                    break;
            }
            _Parent.SetEditBufferAndPaint(loCurrValueStr, false);
            //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_Multiply.
    /// </summary>
    public class TER_Multiply : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        private int _CachedFieldIndex = -1;
        private Reino.ClientConfig.TTTable _CachedListSourceTable = null;
        #endregion

        public TER_Multiply()
            : base()
        {
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            ETrueFalseIgnore loEnforce;
            Int64 loMulValue = 1;
            Int64 loCurrValue = 0;
            string loCurrValueStr = "";

            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) == ETrueFalseIgnore.tfiIgnore)
                return false;

            if (loEnforce == ETrueFalseIgnore.tfiTrue)
            {  // go get the increment value

                // Is there a value in a table column?
                if (ControlEdit2 != "" && (this.GetControlEdit2().ListSourceTable != null))
                {
                    // Avoid field lookup by name if we have a field index and the source table is the same as previous
                    if ((_CachedListSourceTable != null) && (_CachedListSourceTable == GetControlEdit2().ListSourceTable) &&
                        (_CachedFieldIndex != -1))
                    {
                        ReinoControls.TextBoxBehavior.StrTollInt(ReinoControls.TextBoxBehavior.GetFieldDataFromTable(
                            _CachedListSourceTable, _CachedFieldIndex, _Parent.GetListNdx(), null), 
                            ref loCurrValue);
                    }
                    else
                    {
                        // Somethings different or not initialized, so we need to resolved field index first
                        _CachedListSourceTable = this.GetControlEdit2().ListSourceTable;
                        _CachedFieldIndex = ReinoControls.TextBoxBehavior.GetFieldIndexForTable(this.GetControlEdit2().ListSourceTable,
                            CharParam);

                        ReinoControls.TextBoxBehavior.StrTollInt(ReinoControls.TextBoxBehavior.GetFieldDataFromTable(
                            _CachedListSourceTable, _CachedFieldIndex, _Parent.GetListNdx(), null), 
                            ref loCurrValue);
                    }
                }

                // is there a value in another field
                if (ControlEdit1 != "")
                    loMulValue = Convert.ToInt64(GetControlEdit1().GetText());
                // get the total amt, include constant value in fIntParam2
                loMulValue = loCurrValue + IntParam2;
            }

            if (loMulValue == 1) // multplying/dividing by 1 does nothing
                return false;
            // Get current field value. If string contains a decimal, then we first have to treat it as a double/floating value
            if (_Parent.GetText().IndexOf('.') >= 0 )
                loCurrValue = Convert.ToInt64(Convert.ToDouble(_Parent.GetText()));
            else
                loCurrValue = Convert.ToInt64(_Parent.GetText());

            if (IntParam > 0)
                loCurrValue /= loMulValue;
            else
                loCurrValue *= loMulValue;

            loCurrValueStr = Convert.ToString(loCurrValue);
            _Parent.SetEditBufferAndPaint(loCurrValueStr, false);
            //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_CalcForceAge.
    /// </summary>
    public class TER_CalcForceAge : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_CalcForceAge()
            : base()
        {
            // Set our defaults that differ from parent.
            ControlEdit1 = "IssueDate";
            ControlEdit2 = "DLBirthDate";
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        protected int CalcAge()
        {
            int loIssueYear = 0;
            int loIssueMo = 0;
            int loIssueDay = 0;
            int loDOBYear = 0;
            int loDOBMo = 0;
            int loDOBDay = 0;
            int loAge = 0;

            if (CheckForControlEdit1() != 0) return 0;
            if (CheckForControlEdit2() != 0) return 0;

            // get the issue date in fControlEdit1
            if (ReinoControls.TextBoxBehavior.DateStringToDMY(GetControlEdit1().GetEditMask(),
                GetControlEdit1().GetText(), ref loIssueDay, ref loIssueMo, ref loIssueYear) < 0)
                return -1;

            // get the birth date in fControlEdit2
            if (ReinoControls.TextBoxBehavior.DateStringToDMY(this.GetControlEdit2().GetEditMask(),
                this.GetControlEdit2().GetText(), ref loDOBDay, ref loDOBMo, ref loDOBYear) < 0)
                return -1;

            // calculate the age in years
            loAge = loIssueYear - loDOBYear;
            if ((loDOBMo > loIssueMo) || ((loDOBMo == loIssueMo) && (loDOBDay > loIssueDay)))
                loAge--;

            return loAge;
        }

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            int loPrevAge;
            int loAge;
            string loAgeStr;

            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                return false;

            if (ControlEdit1 == "" || ControlEdit2 == "")
                return false;

            // is DOB blank? blank out age
            if (this.GetControlEdit2().FieldIsBlank())
            {
                _Parent.SetEditBufferAndPaint("", false);
                //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
                return false;
            }

            // save the previous value of the field so we can determine if we changed it
            try
            {
                loPrevAge = Convert.ToInt32(_Parent.EditBuffer/*.GetText()*/);
            }
            catch
            {
                loPrevAge = 0;
            }

            if ((loAge = CalcAge()) < 0)
                return false;

            if (loAge == loPrevAge)
                return false;

            loAgeStr = Convert.ToString(loAge);
            _Parent.SetEditBufferAndPaint(loAgeStr, false);
            //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();

            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_ValidateInches.
    /// </summary>
    public class TER_ValidateInches : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ValidateInches()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnValidate = true;
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            int loNdx;
            int loInchesStartPos = -1;
            bool lo2InchesChars = false;
            string loDataStr = "";

            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                return false;

            if (_Parent.FieldIsBlank())
                return false;

            loDataStr = _Parent.GetText();

            // find the inches portion. It is either the 1st two digits from the end OR the first digit before a literal mask character.
            for (loNdx = loDataStr.Length - 1; loNdx >= 0; loNdx--)
            {
                if ((loDataStr[loNdx] < '0') || (loDataStr[loNdx] > '9'))
                    break; // found a delimiter

                // have a digit. Is this the 1st or 2nd?
                if (loInchesStartPos == -1) // 1st inches digit we've encountered
                {
                    loInchesStartPos = loNdx;
                }
                else
                { // 2nd inches character
                    loInchesStartPos = loNdx;
                    lo2InchesChars = true;
                    break;
                }
            }

            // did we encounter any inches?
            if (loInchesStartPos == -1)
                return true; // no inches, return FAILURE

            // make sure there are feet as well
            for (loNdx = loInchesStartPos - 1; loNdx >= 0; loNdx--)
            {
                if ((loDataStr[loNdx] >= '0') && (loDataStr[loNdx] <= '9'))
                    break; // found a DIGIT for feet
            }

            if (loNdx < 0)
                return true; // no feet, so return FAILURE!

            if (!lo2InchesChars)
            { // only 1 inch digit, so merely insert a 0 and we're done.
                loDataStr = loDataStr.Insert(loInchesStartPos, "0");
                _Parent.SetEditBufferAndPaint(loDataStr, false);
                //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
                return false;
            }

            // there are two inches digits. Make sure they don't exceed 11.
            if ((loDataStr[loInchesStartPos] > '1') ||
                 ((loDataStr[loInchesStartPos] == '1') && (loDataStr[loInchesStartPos + 1] > '1')))
            {
                return true;
            }
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_ForceCleared.
    /// </summary>
    public class TER_ForceCleared : TER_ForceLiteral /*Reino.ClientConfig.TEditRestriction*/
    {
        #region Properties and Members
        #endregion

        public TER_ForceCleared()
            : base()
        {
            // Put default settings back in place
            ActiveOnCorrection = true;
            ActiveOnReissue = true;
            ActiveOnContinuance = true;
            ActiveOnIssueMore = true;
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            ETrueFalseIgnore loEnforce;
            if ((loEnforce = RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior)) != ETrueFalseIgnore.tfiTrue)
                return false;

            _Parent.SetEditBufferAndPaint(CharParam, false);
            //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_CurrentUser.
    /// </summary>
    public class TER_CurrentUser : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_CurrentUser()
            : base()
        {
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public Reino.ClientConfig.TClientDef clientDef = null;

        private Reino.ClientConfig.TTTable GetUserStructTable()
        {
            try
            {
                // Use predicate to find the UserStruct (There should only be one!)
                TObjBasePredicate predicate = new TObjBasePredicate(typeof(TUserStruct));
                TIssStruct issStruct = clientDef.IssStructMgr.IssStructs.Find(predicate.CompareByClassType);
                if (issStruct != null)
                {
                    // Get Table definition and table revision associated with UserStruct
                    Reino.ClientConfig.TTableDef tableDef = issStruct.MainTable;
                    Reino.ClientConfig.TTableDefRev tableRev = tableDef.HighTableRevision;
                    // If the TableDefRev doesn't have a TTTable, create one
                    if (tableRev.Tables.Count == 0)
                    {
                        Reino.ClientConfig.TTTable newTable = new Reino.ClientConfig.TTTable();
                        newTable.SetTableName(tableDef.Name);
                        //tableRev.Tables.Add(newTable); // SetTableName adds the table to the list
                    }
                    // Return the first TTTable object
                    return tableRev.Tables[0];
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            try
            {
                if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                    return false;

                // Get the table associated with the user structure
                Reino.ClientConfig.TTTable UsersTable = GetUserStructTable();

                // Create predicate we can use for comparison
                TObjBasePredicate predicate = new TObjBasePredicate(_Parent._CfgCtrl._Name);

                // Find the appropriate field index within the users table
                int loFldIdx = UsersTable.fTableDef.HighTableRevision.Fields.FindIndex(predicate.CompareByName_CaseInsensitive);

                // Set the parent control's text from the database field if applicable
                if ((loFldIdx > -1) && (UsersTable.fFieldValues.Count > 0))
                {
                    _Parent.SetEditBufferAndPaint(UsersTable.fFieldValues[loFldIdx], false);
                    //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
                }
                return false;
            }
            catch (Exception Ex)
            {
                Debug.WriteLine("TER_CurrentUser.EnforceRestriction Error:\r\n" + Ex.Message);
                Debug.WriteLine("Type: " + Ex.GetType().ToString());
                if (Ex.InnerException != null)
                    Debug.WriteLine("Inner: " + Ex.InnerException.Message);
                Debug.WriteLine("StackTrace: " + Ex.StackTrace);
                return false;
            }
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_ForceGlobalCurrentValue.
    /// </summary>
    public class TER_ForceGlobalCurrentValue : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

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
#if USE_DEFN_IMPLEMENTATION
        public delegate string GetCurrentGlobalFieldValue(string FieldName);
        public event GetCurrentGlobalFieldValue OnGetCurrentGlobalFieldValue = null;

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                return false;
            // Find this field's current global value.
            // Do the OnGetCurrentGlobalFieldValue event if its assigned 
            if (OnGetCurrentGlobalFieldValue != null)
            {
                _Parent.SetEditBufferAndPaint(OnGetCurrentGlobalFieldValue(_Parent._CfgCtrl._Name), false);
                //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            }
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_SetGlobalCurrentValue.
    /// </summary>
    public class TER_SetGlobalCurrentValue : Reino.ClientConfig.TEditRestriction
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
#if USE_DEFN_IMPLEMENTATION
        public delegate void SetCurrentGlobalFieldValue(string FieldName, string FieldValue);
        public event SetCurrentGlobalFieldValue OnSetCurrentGlobalFieldValue = null;

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                return false;

            // Set this field's current global value.
            // Do the OnSetCurrentGlobalFieldValue event if its assigned 
            if (OnSetCurrentGlobalFieldValue != null)
                OnSetCurrentGlobalFieldValue(_Parent._CfgCtrl.Name, _Parent.GetText());
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_Mod10CheckDigit.
    /// </summary>
    public class TER_Mod10CheckDigit : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_Mod10CheckDigit()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnCorrection = false;
            ActiveOnReissue = false;
            ActiveOnContinuance = false;
            ActiveOnFormInit = true;
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                return false;
            if (ReinoControls.TextBoxBehavior.OnStandardMessageBox != null)
            {
                ReinoControls.TextBoxBehavior.OnStandardMessageBox(
                    "TER_Mod10CheckDigit Not Implemented", "Error");
            }
            else
            {
                MessageBox.Show("TER_Mod10CheckDigit Not Implemented", "Error");
            }
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_ForceCalcJuvenile.
    /// </summary>
    public class TER_ForceCalcJuvenile : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ForceCalcJuvenile()
            : base()
        {
            // Set our defaults that differ from parent.
            CharParam = "Y";
            IntParam = 17;
            ControlEdit1 = "IssueDate";
            ControlEdit2 = "DLBirthDate";
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        protected int CalcAge()
        {
            int loIssueYear = 0;
            int loIssueMo = 0;
            int loIssueDay = 0;
            int loDOBYear = 0;
            int loDOBMo = 0;
            int loDOBDay = 0;
            int loAge = 0;

            if (CheckForControlEdit1() != 0) return 0;
            if (CheckForControlEdit2() != 0) return 0;

            // get the issue date in fControlEdit1
            if (ReinoControls.TextBoxBehavior.DateStringToDMY(GetControlEdit1().GetEditMask(),
                GetControlEdit1().GetText(), ref loIssueDay, ref loIssueMo, ref loIssueYear) < 0)
                return -1;

            // get the birth date in fControlEdit2
            if (ReinoControls.TextBoxBehavior.DateStringToDMY(this.GetControlEdit2().GetEditMask(),
                this.GetControlEdit2().GetText(), ref loDOBDay, ref loDOBMo, ref loDOBYear) < 0)
                return -1;

            // calculate the age in years
            loAge = loIssueYear - loDOBYear;
            if ((loDOBMo > loIssueMo) || ((loDOBMo == loIssueMo) && (loDOBDay > loIssueDay)))
                loAge--;

            return loAge;
        }

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            string loPrevValue;
            int loAge;

            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                return false;

            if (ControlEdit1 == "" || ControlEdit2 == "")
                return false;

            // is DOB blank? blank out juvenile
            if (this.GetControlEdit2().FieldIsBlank())
            {
                _Parent.SetEditBufferAndPaint("", false);
                //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
                return false;
            }

            // save the previous value of the field so we can determine if we changed it
            loPrevValue = _Parent.GetText();

            if ((loAge = CalcAge()) < 0)
                return false;

            // is it less than the limit?
            if (loAge < IntParam)
            {
                // We have a juvenile, is the value changing?
                if (loPrevValue.CompareTo("Y") != 0)
                {
                    string loLine1 = String.Format("{0:d} yrs old (Juv is under {1:d}).", loAge, IntParam);
                    _Parent.SetEditBufferAndPaint("Y", false);
                    //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
                    // should we alert the user?
                    if (CharParam == "Y")
                    {
                        if (ReinoControls.TextBoxBehavior.OnStandardMessageBox != null)
                            ReinoControls.TextBoxBehavior.OnStandardMessageBox(loLine1, "Juvenile Offender!");
                        else
                            MessageBox.Show(loLine1, "Juvenile Offender!");
                    }
                }
            }
            else
            {
                _Parent.SetEditBufferAndPaint("N", false);
                //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            }
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_VicRoadsCheckDigit.
    /// </summary>
    public class TER_VicRoadsCheckDigit : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_VicRoadsCheckDigit()
            : base()
        {
            // Set our defaults that differ from parent.
            ActiveOnNewEntry = false;
            ActiveOnCorrection = false;
            ActiveOnReissue = false;
            ActiveOnContinuance = false;
            ActiveOnValidate = true;
            Overrideable = true;
            ControlEdit1 = "VehLicNo";
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                return false;

            if (ControlEdit1 == "")
                return false;

            /* Lifted from the C88 code.  The algorithm is as follows:
               - make sure the plate is <= 6 characters.  Return an error if not. If the plate is less than
                 6, left pad w/ spaces to 6 characters.
               - Convert each digit to a numeric value; '0'-'9' = 0-9; 'A'-'Z' are 10 + position in 
                 alphabet.  'A' is 11, 'B' is 12.. 'Z' is 37.
               - Sum the product of each converted value and its corresponding position's weight.
               - Take the remainder of the sum divided by 37.
               - The check digit is the character found in the remainder position in the string
                 '+123456789.ABCDEFGHIJKLMN-PQRSTUVWXYZ'; */

            const int FixedVicRoadsLength = 6;
            const int VicRoadsCheckDigitCnt = 37;
            const string CheckDigits = "+123456789.ABCDEFGHIJKLMN-PQRSTUVWXYZ";
            // the weight multipliers 
            int[] DigitWeights = new int[FixedVicRoadsLength] { 1, 3, 7, 1, 3, 7 };
            // the digit lookup table 
            string loPlateStr = "";
            int loCharNdx = 0;
            int loRunningTotal = 0;
            int loDigitVal = 0;
            char loCalcCheckDigit;

            loPlateStr = GetControlEdit1().GetText();
            // make sure plate isn't too long or blank
            if (loPlateStr == "" || (loPlateStr.Length > FixedVicRoadsLength))
                return true;

            // left pad it to 6
            loPlateStr = loPlateStr.PadLeft(FixedVicRoadsLength, ' ');

            for (loCharNdx = 0; loCharNdx < FixedVicRoadsLength; loCharNdx++)
            {
                // assign a numeric value to the character between 0 and 36
                if ((loPlateStr[loCharNdx] >= '0') && (loPlateStr[loCharNdx] <= '9'))
                    loDigitVal = loPlateStr[loCharNdx] - '0';
                else if (loPlateStr[loCharNdx] == ' ')
                    loDigitVal = 10;
                else if ((loPlateStr[loCharNdx] >= 'A') && (loPlateStr[loCharNdx] <= 'Z'))
                    loDigitVal = loPlateStr[loCharNdx] - 'A' + 11;
                else
                    return true; // illegal character

                // add product of number and weight to running total
                loRunningTotal += loDigitVal * DigitWeights[loCharNdx];
            }

            // mod Running total w/ 37
            loCalcCheckDigit = CheckDigits[loRunningTotal % VicRoadsCheckDigitCnt];

            // compare the calced check digit w/ the entered one
            if (_Parent.GetText() == loCalcCheckDigit.ToString())
                return false;

            // under no circumstances will the check digit be left wrong.  Clear is preferred.
            _Parent.SetEditBufferAndPaint("", false);
            //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();

            return true;
        }
#endif
        #endregion
    }



    /// <summary>
    /// Summary description for TER_ForceStLouisCheckDigit.
    /// </summary>
    public class TER_ForceStLouisCheckDigit : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ForceStLouisCheckDigit()
            : base()
        {
            // Set our defaults that differ from parent.
            ControlEdit1 = "IssueNo";
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {

            int loNdx = 0;
            int loSourceStrLen = 0;
            int loSum = 0;
            string loResult;

            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                return false;

            // make sure we have a source field
            if (CheckForControlEdit1() != 0)
                return false;

            if (GetControlEdit1().GetText() == "")
                return false; // source is blank.

            loSourceStrLen = GetControlEdit1().GetText().Length;

            for (loNdx = loSourceStrLen - 1; loNdx >= 0; loNdx--)
            {
                // make sure the character is a digit.
                if ((GetControlEdit1().GetText()[loNdx] < '0') ||
                    (GetControlEdit1().GetText()[loNdx] > '9'))
                    continue;

                // least significant digit is at right-most position in string.
                loSum += (GetControlEdit1().GetText()[loNdx] - '0') * (loSourceStrLen - loNdx + 1);
            }

            // calculate the sum Mod 11
            loSum = 11 - (loSum % 11);
            // convert to a character.
            if (loSum < 10)
                loResult = ((char)(loSum + (int)'0')).ToString();
            else
                loResult = ((char)(loSum + (int)'A' - 10)).ToString();
            // force the parent field to this value.
            _Parent.SetEditBufferAndPaint(loResult, false);
            //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            return false;
        }
#endif
        #endregion
    }

    /// <summary>
    /// The hot sheet restrictions also use a match fields name property. This
    /// class is just used as a base for these.
    /// </summary>
    public class TER_BaseHotSheet : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        protected string _MatchFieldsName = "";
        public string MatchFieldsName
        {
            get { return _MatchFieldsName; }
            set { _MatchFieldsName = value; }
        }

        #endregion

        public TER_BaseHotSheet()
            : base()
        {
        }
    }

    public delegate bool DoSearch(TER_SearchHotSheet EditRestrict);

    /// <summary>
    /// Summary description for TER_SearchHotSheet.
    /// </summary>
    public class TER_SearchHotSheet : Reino.ClientConfig.TER_BaseHotSheet
    {
        #region Properties and Members
        public event DoSearch OnDoSearch = null;
        #endregion

        public TER_SearchHotSheet()
            : base()
        {
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                return false;

            if (OnDoSearch != null)
            {
                return OnDoSearch(this);
            }
            return false;
        }
#endif
        #endregion
    }

    public delegate void DoHotSheetFilter(TER_HotsheetFilter EditRestrict);

    /// <summary>
    /// Summary description for TER_HotsheetFilter.
    /// </summary>
    public class TER_HotsheetFilter : Reino.ClientConfig.TER_BaseHotSheet
    {
        #region Properties and Members
        public event DoHotSheetFilter OnDoHotSheetFilter = null;
        public bool fPreemptedByWireless = false;
        #endregion

        public TER_HotsheetFilter()
            : base()
        {
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                return false;

            if (OnDoHotSheetFilter != null)
                OnDoHotSheetFilter(this);
            return false;
        }

        /// <summary>
        /// Called when a wireless search result is returned.
        /// iParam is a TTTable holding the result set.
        /// </summary>
        public void FinishEnforceRestriction(TTTable iParam)
        {
            TTTable loTable = iParam;
            TTTable loParentTable;

            // Nothing to do if no result set returned.
            if (loTable == null)
                return;

            // Make sure we have an associated table and it is the same as the search table
            loParentTable = _Parent.ListSourceTable;
            if (loParentTable == null)
                return;

            // Just in case the wireless result returned while the local result
            // was being processed we need to tell EnforceRestriction to not use the local result,
            // our wireless results supercede them.
            fPreemptedByWireless = true;

            // When we receive a wireless result, we can't just use it as a filter because
            // the result set is a separate table w/o filters.  Instead, we must swap tables
            _Parent.ListSourceTable = loTable;
            // This is already done for us by setting ListSourceTable property
            /*
            _Parent.ListItemCount = loTable.GetRecCount();
            _Parent.ListItemCache.Clear();
            */

            // If there is a listbox, make sure it gets updated
            if (_Parent.ListBox != null)
                _Parent.ListBox.RefreshItems(true);
            _Parent.SetEditBufferAndPaint(ReinoControls.TextBoxBehavior.GetFieldDataFromTable(
                loTable, _Parent._CfgCtrl._Name,
                loTable.GetCurRecNo(), _Parent.GetEditMask()), true); // Must trigger change events!
            //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            return;
        }

#endif
        #endregion
    }

    /// <summary>
    /// Summary description for TER_ForceSequence.
    /// </summary>
    public class TER_ForceSequence : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ForceSequence()
            : base()
        {
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION
        public delegate bool SetIssueNoFields();
        public event SetIssueNoFields OnSetIssueNoFields = null;

        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {
            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                return false;

            // Do the OnSetIssueNoFields event if its assigned 
            if (OnSetIssueNoFields != null)
                OnSetIssueNoFields();
            return false;
        }
#endif
        #endregion
    }


    /// <summary>
    /// Summary description for TER_ForceCincinnatiMod7CheckDigit.
    /// </summary>
    public class TER_ForceCincinnatiMod7CheckDigit : Reino.ClientConfig.TEditRestriction
    {
        #region Properties and Members
        #endregion

        public TER_ForceCincinnatiMod7CheckDigit()
            : base()
        {
            // Set our defaults that differ from parent.
            ControlEdit1 = "IssueNo";
        }

        #region Implementation code
#if USE_DEFN_IMPLEMENTATION

        protected Int64 CalcMod7CheckDigit(Int64 iSourceNumber)
        {
            Int64 loCheckDigit = 0;

            // chop off the last digit
            iSourceNumber /= 10;

            // check digit number mod 7
            loCheckDigit = (iSourceNumber % 7);
            // add a digit back to source.
            iSourceNumber *= 10;
            iSourceNumber += loCheckDigit;
            return iSourceNumber;
        }


        public override bool EnforceRestriction(int iNotifyEvent, int iFormEditMode, ref TextBoxBehavior iParentBehavior)
        {

            int loNdx = 0;
            int loSum = 0;
            string loResult;
            string loSource;

            if (RestrictionActiveOnEvent(iNotifyEvent, iFormEditMode, ref iParentBehavior) != ETrueFalseIgnore.tfiTrue)
                return false;

            // make sure we have a source field
            if (CheckForControlEdit1() != 0)
                return false;

            if (GetControlEdit1().GetText() == "")
                return false; // source is blank.

            // start with the first
            loSource = GetControlEdit1().GetText();


            // look for a second source
            if ((_ControlEdit2 != "") && (_ControlEdit2Obj != null))
            {
                if (GetControlEdit2().GetText() != "")
                {
                    // append it to the first
                    loSource = loSource + GetControlEdit2().GetText();
                }
            }

            // while we're at it, look for a third
            if ((_ControlEdit3 != "") && (_ControlEdit3Obj != null))
            {
                if (GetControlEdit3().GetText() != "")
                {
                    // append it to the first
                    loSource = loSource + GetControlEdit3().GetText();
                }
            }

            // keep only the numeric digits
            string loNumericOnly = "";
            foreach (char oneChar in loSource)
            {
                if (( oneChar >= '0') && (oneChar <= '9' ))
                {
                    loNumericOnly += oneChar;
                }
            }


            // convert to numeric
            Int64 loHugeInt;
            loHugeInt = Convert.ToInt64(loNumericOnly);

            // bump it by ten to make a place holder for the check digit
            loHugeInt = (loHugeInt * 10);

            // calculate the check digit
            Int64 loValueWithCheckDigit = CalcMod7CheckDigit(loHugeInt);

            // isolate the check digit
            Int64 loCheckDigitOnly = loValueWithCheckDigit - ((loValueWithCheckDigit / 10) * 10);

            // Cincinnati special - add 1
            loCheckDigitOnly++;

            loResult = Convert.ToString(loCheckDigitOnly);

            // force the parent field to this value.
            _Parent.SetEditBufferAndPaint(loResult, false);
            //DEBUG -- Not needed and makes stuff too slow?// _Parent.ResynchListNdx();
            return false;
        }
#endif
        #endregion
    }

}
