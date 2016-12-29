using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Android.Content;
using Android.Graphics;
using Duncan.AI.Droid.Utils.EditControlManagement;
//using Duncan.AI.Droid.Utils.EditControlManagement.EditRules;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Reino.ClientConfig;
using XMLConfig;
using TER_CurrentUser = Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_CurrentUser;
using TER_Hidden = Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_Hidden;
using TER_Required = Duncan.AI.Droid.Utils.EditControlManagement.EditRules.TER_Required;

namespace Duncan.AI.Droid.Utils.HelperManagers
{
   public class AndroidConfigData :ConfigData
   {
       private readonly Context _ctx;
       public AndroidConfigData(Context ctx)
       {
           _ctx = ctx;
       }

       public List<EditControlBehavior> BehaviorCollection = new List<EditControlBehavior>();
       public List<CustomTTEditControl> EntryOrder = new List<CustomTTEditControl>();

       public List<CustomEditText> CustomEditTexts = new List<CustomEditText>();
       public List<CustomAutoTextView> CustomAutoTextViews = new List<CustomAutoTextView>();
       public List<CustomSpinner> CustomSpinners = new List<CustomSpinner>();

       readonly Hashtable _serializers = new Hashtable();
       readonly Hashtable _deSerializers = new Hashtable();
       public override void BuildControlsFromCfgTabSheet( IList<TSheet> Container, string structName)
       {
           // Increment the list capacity for the number of objects we know about
           BehaviorCollection.Capacity = BehaviorCollection.Capacity + Container.Count;
            EntryOrder.Capacity = EntryOrder.Capacity + Container.Count;
            base.BuildControlsFromCfgTabSheet(Container, structName);
       }

        public override void BuildControlsFromCfgContainer(IList<TWinClass> Container, string structName)
       {
           // Increment the list capacity for the number of objects we know about
           this.BehaviorCollection.Capacity = this.BehaviorCollection.Capacity + Container.Count;
           this.EntryOrder.Capacity = this.EntryOrder.Capacity + Container.Count;
           base.BuildControlsFromCfgContainer(Container, structName);
       }

       protected override void BuildEditCtrlFromCfg(TTEdit pCtrl, string structName)
       {
           // If its a TEditListBox then we're building a listbox instead of a textbox
           if (pCtrl is TEditListBox)
               return;

           // If its a TDrawField then we're building a ReinoDrawBox instead of a textbox
           if (pCtrl is TDrawField)
           {
               // keep going for signature capture
               int i = 1;
           }

           if (pCtrl.Name.Equals("SELECTION"))
               return;


           // Create TextBoxBehavior object and set default attributes
           var behavior = new EditControlBehavior();
           pCtrl.IsEnabled = true;
           behavior.StructName = structName;
           behavior.CustomId = structName + pCtrl.Name;

           // Set cross-ref links between new behavior and configuration object.
           // Set link so behavior can find other behaviors for this form.
           // Add behavior to list of behaviors for this form.
           var newPControl = MakeCustomTtEditControl(pCtrl);
           // Add this configuration control to the EntryOrder list for easier navigation
           EntryOrder.Add(newPControl);

           behavior.BehaviorCollection = this.BehaviorCollection;
           // A TNumEdit should always be efNumeric
           if (pCtrl is TNumEdit)
               behavior.SetFieldType(EditEnumerations.EditFieldType.efNumeric);
           else
           {
               // We must convert the ClientConfig field type to the ReinoTextBox field type
               if (pCtrl is TEditField)
               {
                   switch ((pCtrl as TEditField).FieldType)
                   {
                       case TEditField.TEditFieldType.efDate:
                           behavior.SetFieldType(EditEnumerations.EditFieldType.efDate);
                           EditMaskingData.Add(new EditMaskMapEntry
                           {
                               name = (pCtrl as TEditField).Name,
                               editmask = (pCtrl as TEditField).EditMask,
                               type = (int)(pCtrl as TEditField).FieldType
                           }
                            );
                           break;

                       case TEditField.TEditFieldType.efTime:
                           behavior.SetFieldType(EditEnumerations.EditFieldType.efTime);
                           EditMaskingData.Add(new EditMaskMapEntry
                           {
                               name = (pCtrl as TEditField).Name,
                               editmask = (pCtrl as TEditField).EditMask,
                               type = (int)(pCtrl as TEditField).FieldType
                           }
                            );
                           break;

                       case TEditField.TEditFieldType.efString:
                           behavior.SetFieldType(EditEnumerations.EditFieldType.efString);
                           break;
                   }
               }
           }

           // Copy the edit mask and maxlength properties
           if ((pCtrl is TEditField) && ((pCtrl as TEditField).EditMask != ""))
               behavior.SetEditMask((pCtrl as TEditField).EditMask);
           behavior.MaxLength = pCtrl.MaxLength;

           int intParamForceCurrDtTime = -1; //iF TER_ForceCurrentDateTime exists then value will be set greater than -1
           String Val = "";
           String parentCtl = "";
           String dataPrm = "";
           String editMsk = "";
           var fieldType = TEditField.TEditFieldType.efString;
           var optionsLst = new DBList();

           if (pCtrl is TEditField)
           {
               var editField = (pCtrl as TEditField);
               editMsk = editField.EditMask;
               fieldType = editField.FieldType;
               optionsLst = new DBList
                   {
                       ListName = ((TEditField) pCtrl).ListName.Split(' ')[0],
                       IsListOnly = (pCtrl.Restrictions.Any(x => x.Name == "LISTONLY")),
                       Columns = editField.DBListGrid.Columns.Select(x => x.Name).ToArray()
                   };

               try
               {
                   // AJW - repeating operations and relying on exceptions slows us down, do it more effectively
                   //behavior.ListTableName = ((TEditField)pCtrl).ListName.Split(' ')[0];
                   //behavior.ListTableColumn = editField.DBListGrid.Columns.Select(x => x.Name).ToArray();
                   //optionsLst.saveColumn =  ((TEditField)pCtrl).ListName.Split(' ')[1];

                   string[] loListNames = ((TEditField)pCtrl).ListName.Split(' ');
                   if (loListNames.Length > 0)
                   {
                       behavior.ListTableName = loListNames[0];
                   }

                   behavior.ListTableColumn = editField.DBListGrid.Columns.Select(x => x.Name).ToArray();

                   if (loListNames.Length > 1)
                   {
                       optionsLst.saveColumn = loListNames[1];
                   }
               }
               catch (Exception ex)
               {
                   //Console.WriteLine("");
                   LoggingManager.LogApplicationError(ex, "Defining List and Table Names.", "BuildEditCtrlFromCfg");
                   Console.WriteLine(ex.Message + ex.StackTrace);

               }        
           }
           try
           {
               // Build edit conditions if the configuration object has some
               if (pCtrl.Conditions.Count > 0)
                   BuildCtrlConditions(ref behavior, newPControl);

               // Build edit restrictions if the configuration object has some
               if (pCtrl.Restrictions.Count > 0)
                   BuildCtrlRestrictions(ref behavior, newPControl);
           }
           catch (Exception ex)
           {
               LoggingManager.LogApplicationError(ex, "Building conditions and restrictions.", "BuildEditCtrlFromCfg");
               Console.WriteLine(ex.Message + ex.StackTrace);
           }


           //hook up the properties if needed.
           var childListRestriction = pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_ChildList");
           if (childListRestriction != null)
           {
               parentCtl = childListRestriction.ControlEdit1;
               dataPrm = childListRestriction.CharParam;
           }

           if (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_ForceCurrentDateTime") != null)
               intParamForceCurrDtTime =
                   (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_ForceCurrentDateTime")).IntParam;


           Single width;

           if (pCtrl.Width < 100)
               width = .25f; // quarter
           else if (pCtrl.Width < 200)
               width = .50f; // half
           else
               width = 1.0f; // full width


          // var requiredRestriction = (pCtrl.Restrictions.Any(x => x.GetType().Name == "TER_Required" && x.ActiveOnFormInit));
           var requiredRestriction = (pCtrl.Restrictions.Any(x => x.GetType().Name == "TER_Required"));
           var newPanelField = new PanelField
               {
                   Name = pCtrl.Name,
                   Label = pCtrl.PromptWin.TextBuf,
                   Width = width,
                   MaxLength = pCtrl.MaxLength,
                   Value = Val,
                   IsRequired = requiredRestriction,
                   IsHidden = (pCtrl.Restrictions.Any(x => x.GetType().Name == "TER_Hidden")),
                   ParentControl = parentCtl,
                   DataParam = dataPrm,
                   EditMask = editMsk,
                   FieldType = fieldType.ToString(),
                   OptionsList = optionsLst,
                   IntParamForceCurrDtTime = intParamForceCurrDtTime,
                   fEditFieldDef = pCtrl,
                   fParentStructName = structName,

               };


           try
           {
               IssStructs[IssStructs.Count - 1].Panels[IssStructs[IssStructs.Count - 1].Panels.Count - 1].PanelFields.Add(newPanelField);
           }
           catch (Exception e)
           {
               // AJW - for review, this is not enough
               LoggingManager.LogApplicationError(e, e.Message, "BuildEditCtrl");
               Console.WriteLine(e.Message + " BuildEditCtrl adding new panel field ");
               int i = 0;
           }
           behavior.PanelField = newPanelField;
         
           BehaviorCollection.Add(behavior);
           behavior.NotifiedDependentsParentDataChanged += Behavior_NotifiedDependentsParentDataChanged;
           behavior.CfgCtrl = newPControl;
           behavior.CfgCtrl.BehaviorAndroid = behavior;

           //now add it ot our main colleciton of objects so we can pull when needed.
           //check to see if it is a auto text view or a edit test

           behavior.ControlType = EditEnumerations.CustomEditControlType.EditText;
           //list type items
           if (optionsLst.Columns != null && optionsLst.Columns.Length > 0)
           {
#if _allow_spinners_
               if (optionsLst.IsListOnly)
               {
                   behavior.ControlType = EditEnumerations.CustomEditControlType.Spinner;
                   var item = new CustomSpinner(_ctx) { Text = Val, Tag = pCtrl.Name, CustomId = structName + pCtrl.Name };
                   item.AssociateControl(behavior);
                   item.SetPopupBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Color.LightGray));


                   // AJW - hook here until method changed to pass sender along
                   item.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);

                   CustomSpinners.Add(item);
               }
               else
#endif
               {
                   //add auto text
                   behavior.ControlType = EditEnumerations.CustomEditControlType.AutoCompleteText;
                   var item = new CustomAutoTextView(_ctx) {
                       Text = Val,
                       Tag = pCtrl.Name,
                       CustomId = structName + pCtrl.Name
                   };
                   item.AssociateControl(behavior);
                   item.SetDropDownBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Color.LightGray));
                   item.SetBackgroundColor(Color.White);
                   item.SetTextColor(Color.Black);


                   // AJW - hook here until method changed to pass sender along
                   item.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);

                   CustomAutoTextViews.Add(item);
               }
           }
           else
           {
               //add all out edit texts here
               behavior.ControlType = EditEnumerations.CustomEditControlType.EditText;
               var oneNewCustomEditText = new CustomEditText(_ctx) { Text = Val, Tag = pCtrl.Name, CustomId = structName + pCtrl.Name };
               oneNewCustomEditText.AssociateControl(behavior);
               oneNewCustomEditText.SetBackgroundColor(Color.White);
               oneNewCustomEditText.SetTextColor(Color.Black);


               // AJW - hook here until method changed to pass sender along
               oneNewCustomEditText.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);


               CustomEditTexts.Add(oneNewCustomEditText);
           }
       }

       public void Behavior_NotifiedDependentsParentDataChanged(object sender, EventArgs e)
       {

           // AJW - TODO - this appears to be incorrectly implemented, this is not a field exit call
           return;

           ////this occurs when a field looses focus, it calls all its dependants on the parentfieldexit. 
           ////In that iteration, each one of  those call this to process the Ancestor Field Exit. 
           //if (sender is EditControlBehavior == false)
           //    return;
           //var loBehavior = ((EditControlBehavior)(sender));
           //loBehavior.HandleAncestorFieldExit();

       }



       #region Make Methods

       private CustomTTEditControl MakeCustomTtEditControl(TTEdit pCtrl)
       {
           var customEditControl = new CustomTTEditControl
               {
                   PromptWin = pCtrl.PromptWin,
                   Conditions = MakeEditConditions(pCtrl),
                   Restrictions = MakeEditRestrictions(pCtrl),
                   MaxLength = pCtrl.MaxLength,
                   DontDisableWhenCopiedFromForm = pCtrl.DontDisableWhenCopiedFromForm,
                   PreventPrompt = pCtrl.PreventPrompt,
                   IsVioField = pCtrl.IsVioField,
                   IsNotesMemo = pCtrl.IsNotesMemo,
                   IsColorField = pCtrl.IsColorField,
                   Name = pCtrl.Name
               };

           return customEditControl;
       }

       private List<EditRestriction> MakeEditRestrictions(TTEdit pCtrl)
       {
           var customEditRestriction2 = new List<EditRestriction>();
           try
           {
               //SERIALIZE
               //first, serialize it into somehting. for speed purposes, we will keep a runnign list of the serializers
               var originalType2 = pCtrl.Restrictions.GetType();

               var newType2 = (new List<EditRestriction>()).GetType();

               // Check the local cache for a matching serializer.
               var ser = (XmlSerializer)_serializers["MakeEditRestrictions"];
               if (ser == null)
               {
                   ser = new XmlSerializer(originalType2);
                   // Cache the serializer.
                   _serializers["MakeEditRestrictions"] = ser;
               }

               var myXml2 = new XmlDocument();
               XPathNavigator xNav2 = myXml2.CreateNavigator();
               using (var xs2 = xNav2.AppendChild())
               {
                   ser.Serialize(xs2, pCtrl.Restrictions);
               }

               var serializedObject2 = myXml2.OuterXml;

               //perform a but of replaements to fx up the class names
               serializedObject2 = serializedObject2.Replace("TEditRestriction", "EditRestriction");
               serializedObject2 = serializedObject2.Replace("TConditionEvaluation", "ConditionEvaluation");

               //DESERIALIZE
               // Check the local cache for a matching serializer.

               var deser2 = (XmlSerializer)_deSerializers[originalType2.Name];
               if (deser2 == null)
               {
                   deser2 = new XmlSerializer(newType2);
                   _deSerializers[newType2.Name] = deser2;
               }

                customEditRestriction2 = (List<EditRestriction>)deser2.Deserialize(new StringReader(serializedObject2));
            
           }
           catch (Exception ex)
           {
            Console.WriteLine(ex.Message + ex.StackTrace);
           }
           return customEditRestriction2;
       }

       private List<EditCondition> MakeEditConditions(TTEdit pCtrl)
       {
           var customEditConditions = new List<EditCondition>();
           if (pCtrl.Conditions.Count > 0)
           {
               try
               {
                   //SERIALIZE
                   //first, serialize it into somehting. for speed purposes, we will keep a runnign list of the serializers
                   var originalType2 = pCtrl.Conditions.GetType();

                   var newType2 = (new List<EditCondition>()).GetType();

                   // Check the local cache for a matching serializer.
                   var ser = (XmlSerializer) _serializers["MakeEditConditions"];
                   if (ser == null)
                   {
                       ser = new XmlSerializer(originalType2);
                       // Cache the serializer.
                       _serializers["MakeEditConditions"] = ser;
                   }

                   var myXml2 = new XmlDocument();
                   XPathNavigator xNav2 = myXml2.CreateNavigator();
                   using (var xs2 = xNav2.AppendChild())
                   {
                       ser.Serialize(xs2, pCtrl.Conditions);
                   }

                   var serializedObject2 = myXml2.OuterXml;

                   //perform a but of replaements to fx up the class names
                   serializedObject2 = serializedObject2.Replace("TEditCondition", "EditCondition");
                   serializedObject2 = serializedObject2.Replace("TEditRestriction", "EditRestriction");
                   //DESERIALIZE
                   // Check the local cache for a matching serializer.

                   var deser2 = (XmlSerializer) _deSerializers["MakeEditConditions"];
                   if (deser2 == null)
                   {
                       deser2 = new XmlSerializer(newType2);
                       _deSerializers["MakeEditConditions"] = deser2;
                   }

                   customEditConditions = (List<EditCondition>) deser2.Deserialize(new StringReader(serializedObject2));
               }
               catch (Exception ex)
               {
                   Console.WriteLine(ex.Message + ex.StackTrace);
               }
           }

           return customEditConditions;
       }
       #endregion

       private void BuildCtrlConditions(ref EditControlBehavior pBehavior, CustomTTEditControl pCtrl)
       {
           foreach (EditCondition ConfigCondition in pCtrl.Conditions)
           {
               // The edit condition must have a parent property that points to the TextBoxBehavior
               // that it is associated with
               ConfigCondition.SetParent(pBehavior);
               // Add the new edit condition to the EditConditions list of the TextBoxBehavior
               pBehavior.EditConditions.Add(ConfigCondition);
           }
       }

       private void BuildCtrlRestrictions(ref EditControlBehavior pBehavior, CustomTTEditControl pCtrl)
       {
           // Make sure we have the event OnRestrictionForcesRebuildDisplay event for the passed behavior object
           // moved to FormPanel.cs 
           //pBehavior.OnRestrictionForcesDisplayRebuild += this.OnRestrictionForcesDisplayRebuild;
           
           foreach ( var configRestriction  in pCtrl.Restrictions )
           {
               if (configRestriction == null)
               {
                   // debug breakpoint - should not be happening
                   continue;
               }

               configRestriction.Ctx = _ctx;


               // EditRestictions didn't get their registry hooks resolved yet, so do it now
               configRestriction.ResolveRegistryItems(TTRegistry.glRegistry);



               if (configRestriction is TER_CurrentUser)
               {
                   // This edit restriction needs to know where to find datasets to get current user info
                   ((TER_CurrentUser)(configRestriction)).StructName = pBehavior.StructName;
               }

               if (configRestriction is TER_Hidden)
               {
                   // This edit restriction needs to know where to find datasets to get current user info
                   ((TER_Hidden)(configRestriction)).StructName = pBehavior.StructName;
               }

               if (configRestriction is TER_Required)
               {
                   // This edit restriction needs to know where to find datasets to get current user info
                   ((TER_Required)(configRestriction)).StructName = pBehavior.StructName;
               }

                //if (configRestriction is TER_Protected)
                //   ((TER_Protected)(configRestriction)).OnRestrictionForcesDisplayRebuild +=OnRestrictionForcesDisplayRebuild;
              
               // The edit restiction must have a parent property that points to the TextBoxBehavior
               // that it is associated with
               configRestriction.SetParent(pBehavior);

               // Add the new edit restriction to the EditRestrictions list of the TextBoxBehavior
               pBehavior.EditRestrictions.Add(configRestriction);
           }
       }

      


   }



}
