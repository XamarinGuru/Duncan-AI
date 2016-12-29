using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Text;

using Reino.ClientConfig;

using Android.Graphics;
using Android.Views;

namespace XMLConfig
{
    public class IssStruct
    {
        public String Type;
        public String Name;
        public List<Panel> Panels;
        public TIssPrnFormRev PrintPicture = null;
        public Int32 PanelPageCurrentDisplayedIndexSavedForResume;


        // db identity column 
        public String _rowID;
        public String _MasterKey; // for detail records, the foreign/parent key



        //sequenceId and prefix only for display issue ticket screen nothing to do with XML
        public String sequenceId;    // soon to be deprecated...
        public String prefix;        // soon to be deprecated...
        public String suffix;        // soon to be deprecated


        //chalkRowId only for display tire chalking screen nothing to do with XML
        public String chalkRowId;           // soon to be deprecated...



        public String ParentStruct;
        public string SequenceName;


        // references to XML object defs
        public TIssStruct _TIssStruct = null;
        public TIssForm _TIssForm = null;

        // display formatting
        public CultureDisplayFormattingTableInfo fDisplayFormattingInfo = null;


        public IssStruct()
            : base()
        {
            Panels = new List<Panel>();
        }
    }

    public class Panel
    {
        public List<PanelField> PanelFields;

        public object FocusedViewCurrent = null;
        public object FocusedViewPrevious = null;


        public Panel()
            : base()
        {
            PanelFields = new List<PanelField>();
        }
    }

    public class EditMaskMapEntry
    {
        public string name;
        public string editmask;
        public int type;
    }

    public class PanelField
    {
        public String Name;
        public String Label;
        public String Value;
        public Single Width;
        public Int32 MaxLength;
        public Boolean IsRequired;

        //   public Boolean IsReadOnly;
        public Boolean IsHidden;
        public String ParentControl;
        public String DataParam;
        public String EditMask;
        public String FieldType;

        //     public int IntParamForceList;
        public int IntParamForceCurrDtTime;
        public DBList OptionsList;
        public PanelField ParentPanelField;

        // AJW - todo - eventually would like to simplify and remove all of the duplicated properties and just use the original objects where optimal
        public TTEdit fEditFieldDef;
        public String fParentStructName; // this should be the object reference, not just the label

        public volatile View uiComponent; // associated View; volatile because views are subject to display fragment lifecyles; 
                                          // must test/type reference custom decsenndant class object
    }

    public class DBList
    {
        public String ListName;
        public String[] Columns;
        public Boolean IsListOnly;
        public String saveColumn;

        public DBList()
            : base()
        {
            IsListOnly = false;
        }
    }

    public class TableDef
    {
        public String Name;
        public int Revision;
        public List<TableDefField> TableDefFields;

        // AJW - todo - eventually would like to simplify and remove all of the duplicated properties and just use the original objects where optimal
        public TTableDef fTableDef;
        public TTableDefRev fTableDefRev;
        public TIssStruct fIssueStructTableOwner;  // associated struct, when applicable (null for list tables, etc)

        public TableDef()
            : base()
        {
            TableDefFields = new List<TableDefField>();
        }
    }

    public class TableDefField
    {
        public String Name;
        public Int32 Size;
        public String EditDataType;
        public Boolean Required;
        public String LayoutFileFieldTypeName;

        // AJW - todo - eventually would like to simplify and remove all of the duplicated properties and just use the original objects where optimal
        public TTableFldDef fTableFieldDef;

        public TableDefField()
            : base()
        {
            Required = false;
        }
    }




    public class ConfigData
    {
        public List<IssStruct> IssStructs;
        public List<TableDef> TableDefs;

        //public Int32 CurrentPanel = 0;
        //protected static Reino.ClientConfig.TClientDef clientDef = null;

        //public static Reino.ClientConfig.TClientDef clientDef = null;
        public TClientDef clientDef = null;

       

        
        public TListMgr ListMgr = null;
        public List<string> ListTableNames = null;
        public List<TAgList> AgencyListTableDefsManager = null;

        public static List<EditMaskMapEntry> EditMaskingData = null;

        public ConfigData()
            : base()
        {
            IssStructs = new List<IssStruct>();
            TableDefs = new List<TableDef>();
            EditMaskingData = new List<EditMaskMapEntry>();
        }


        public virtual void GetConfig(byte[] iXMLFileData)
        {
            CreateObjFromXml(iXMLFileData);

#if __ANDROID__  
           // init this static global that is referenced throughout
            TClientDef.GlobalClientDef = clientDef;
#endif

            // call  ResolveRegistry and PostDeserialize
            if (clientDef != null)
            {
                // Before doing the PostDeserialize, we must resolve all registry substitutions
                clientDef.ResolveRegistryItems(TTRegistry.glRegistry);

                // call post deserialize to let the objects finalize themselves
                clientDef.PostDeserialize(clientDef);
            }



            ProcessCfgStructures();
            ProcessAgencyList();
            ProcessTagsList();
            ProcessTableDefs();
        }

        public virtual void GetConfig(String filePath)
        {
            CreateObjFromXml(filePath);

#if __ANDROID__
            // init this static global that is referenced throughout
            TClientDef.GlobalClientDef = clientDef;
#endif

            // call  ResolveRegistry and PostDeserialize
            if (clientDef != null)
            {
                // Before doing the PostDeserialize, we must resolve all registry substitutions
                clientDef.ResolveRegistryItems(TTRegistry.glRegistry);

                // call post deserialize to let the objects finalize themselves
                clientDef.PostDeserialize(clientDef);
            }



            ProcessCfgStructures();
            ProcessAgencyList();
            ProcessTagsList();
            ProcessTableDefs();
        }

        public IssStruct GetStruct(String Type, String Name)
        {
            return this.IssStructs.Find(x => x != null && x.Type == Type && x.Name == Name);
        }

        public IssStruct GetStructByName(String Name)
        {
            return this.IssStructs.Find(x => x != null && x.Name == Name);
        }

        public IssStruct GetStructByNameHavingIssue2(String Name)
        {
            foreach ( IssStruct oneIssStruct in IssStructs )
            {
                if ( oneIssStruct.Name.Equals( Name, StringComparison.CurrentCultureIgnoreCase ) == true )
                {
                    if ( oneIssStruct._TIssForm != null )
                    {
                        if ( oneIssStruct._TIssForm._Name.StartsWith( "ISSUE2" ) == true )
                        {
                            return oneIssStruct;
                        }
                    }
                }
            }

            return null;
        }




        public TableDef GetTbleDef(String Name)
        {
            return this.TableDefs.Find(x => x != null && x.Name == Name);
        }

        public TClientDef GetClientDef()
        {
            return this.clientDef;
        }


        protected virtual void CreateObjFromXml(byte[] iXMLFileData)
        {
            // Create an XMLSerialier that can deserialize ISSUE_AP.XML into a TClientDef object
            XmlSerializer serializer = new XmlSerializer(typeof(Reino.ClientConfig.TClientDef));

             // not in there? 
            if (iXMLFileData == null)
            {
                // nothing to do
                return;
            }

            // todo - may need to specify encoding....
            //MemoryStream loXMConfigMemoryStream = new MemoryStream(iXMLFileData, 0, iXMLFileData.Length);
            //XmlReader reader = new XmlTextReader(loXMConfigMemoryStream);

            string loXMLConfigString = System.Text.UTF8Encoding.UTF8.GetString(iXMLFileData);
            XmlReader reader = new XmlTextReader(new StringReader( loXMLConfigString ));



            // Use the Deserialize method to restore the object's state from XML.
            clientDef = (Reino.ClientConfig.TClientDef)serializer.Deserialize(reader);

            // this is always true for Android client
            TClientDef.SkipHostSideOnlyCode = true;

            // Close the reader
            reader.Close();
        }

        protected virtual void CreateObjFromXml(String filePath)
        {
            // Create an XMLSerialier that can deserialize ISSUE_AP.XML into a TClientDef object
            XmlSerializer serializer = new XmlSerializer(typeof(Reino.ClientConfig.TClientDef));

            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            XmlReader reader = new XmlTextReader(fs);

            // Use the Deserialize method to restore the object's state from XML.
            clientDef = (Reino.ClientConfig.TClientDef)serializer.Deserialize(reader);

            // this is always true for Android client
            TClientDef.SkipHostSideOnlyCode = true;

            // Close both the reader and the filestream
            reader.Close();
            fs.Close();
        }

        private void ProcessAgencyList()
        {

            if ((clientDef != null)  && (clientDef.ListMgr != null))
            {
                // resolve to the sub-property - AJW TODO probably not ideal to have dual references, but for demo leave as is;
                ListTableNames = new List<string>();

                // through all of the AgLists
                foreach ( TAgList oneAgList in clientDef.ListMgr.AgLists )
                {
                    // add each defined tabledef therein
                    foreach (TTableDef oneTableDef in oneAgList.TableDefs)
                    {
                        ListTableNames.Add(oneTableDef.Name);
                    }
                }
            }



            /*
            if (clientDef != null
                && clientDef.CustomerCfgMgr != null
                && clientDef.CustomerCfgMgr.CustomerCfgs != null)
            {
                ListObjBase<TCustomerCfg> customerCfgs = clientDef.CustomerCfgMgr.CustomerCfgs;
                if (customerCfgs != null)
                {
                    foreach (TCustomerCfg tCustomerCfg in customerCfgs)
                    {
                        if (tCustomerCfg.HWPlatform == TCustomerCfg.HardwareType.CE)
                        {
                            agencyLists = tCustomerCfg.AgencyLists;
                            break;
                        }
                    }
                }
            }
             * */

 
        }

        private void ProcessTagsList()
        {
            AgencyListTableDefsManager = clientDef.ListMgr.AgLists;
        }

        protected void ProcessTableDefs()
        {
            foreach (TIssStruct IssStruct in clientDef.IssStructMgr.IssStructs)
            {
                BuildTableDef(IssStruct);
            }
        }

        private void GatherAssociatedFieldNames(ref List<string> iFldNamesForCurrPrnData, TableDefField iFldDef)
        {
            // skip virtual fields for now
            iFldNamesForCurrPrnData.Add(iFldDef.Name);
            // If we get this far, the passed field is fully processed, so exit
            return;
        }

        int ValidatePrintedFields(String Type, String Name)
        {
            int loResult;
            int loFieldNo = -1;
            string loErrMsg = "";
            List<string> FldNamesForCurrPrnData = new List<string>();
            IssStruct issStruct = GetStruct(Type, Name);

            // Loop through all print elements in the print picture
            int loLoopMax = issStruct.PrintPicture.AllPrnDataElements.Count;
            for (int loNdx = 0; loNdx < loLoopMax; loNdx++)
            {
                // Get next print element
                TWinBasePrnData loPrnData = issStruct.PrintPicture.AllPrnDataElements[loNdx];
                if (loPrnData == null)
                    continue;

                TableDef tDef = GetTbleDef(loPrnData.Name);

                TableDefField loDBFld = null;
                if (TableDefs != null)
                {
                    foreach (XMLConfig.TableDef tableDef in TableDefs)
                    {
                        foreach (XMLConfig.TableDefField tableDefField in tableDef.TableDefFields)
                        {
                            if (loPrnData.Name == tableDefField.Name)
                            {
                                loDBFld = tableDefField;
                                break;
                            }
                        }
                    }
                }

                // Find main field associated with current PrnData element
                if (loDBFld == null)
                    continue;

                // Get list of all field names associated with current PrnData element
                // (There can be more than 1 associated field if we're dealing with a virtual field)
                FldNamesForCurrPrnData.Clear();
                GatherAssociatedFieldNames(ref FldNamesForCurrPrnData, loDBFld);


                // AJW - for review - is this functionally complete?


                // Now loop for every fieldname we have in the list (Normally there will only be 1)
                foreach (string loFieldName in FldNamesForCurrPrnData)
                {
                }
            }


            return 0;
        }

        private void ProcessPrintDefs(TBaseIssForm form)
        {
            if ((form is TIssForm) && ((form as TIssForm).PrintPictureList.Count > 0))
            {
                if ((form as TIssForm).PrintPictureList[0] != null)
                {
                    foreach (Reino.ClientConfig.TIssPrnFormRev loPrintPicRev in ((form as TIssForm).PrintPictureList[0].Revisions))
                    {
                        loPrintPicRev.ResolveObjectReferences(null, loPrintPicRev);
                        loPrintPicRev.AddDataElementsToList(loPrintPicRev.AllPrnDataElements);

                        // TODO  AJW review - PrintPicture lists is created by form type... but it only has a print print picture if elements are defined for it
                        if (loPrintPicRev.AllPrnDataElements.Count > 0)
                        {
                            IssStructs[IssStructs.Count - 1].PrintPicture = loPrintPicRev;
                        }
                    }
                }
            }
        }



        /*
         * 
         *  AJW - implemented correctly in PrintManager - remove this code 
         *  
        public int WriteFieldValuesToPrintPicture(String Type, String Name)
        {
            TWinBasePrnData loPrnData;
            IssStruct issStruct = GetStruct(Type, Name);
            short loNdx;

            int loLoopMax = issStruct.PrintPictureList.AllPrnDataElements.Count;
            for (loNdx = 0; loNdx < loLoopMax; loNdx++)
            {
                loPrnData = issStruct.PrintPictureList.AllPrnDataElements[loNdx];
                if (loPrnData == null)
                    continue;


                // AJW - review - looping through all tabledefs and stopping at first matching field name doesn't target specific struct ??

                TableDefField loDBFld = null;
                if (TableDefs != null)
                {
                    foreach (XMLConfig.TableDef tableDef in TableDefs)
                    {
                        foreach (XMLConfig.TableDefField tableDefField in tableDef.TableDefFields)
                        {
                            if (loPrnData.Name == tableDefField.Name)
                            {
                                loDBFld = tableDefField;
                                break;
                            }
                        }
                    }
                }

                if (loDBFld == null)
                    continue;
                loPrnData.TextBuf = loDBFld.Name;
            }

            return 0;
        }
         * */

        //        public int ReadFieldValuesFromForm(TBaseIssForm iForm, TTTable iDestTable)
        //        {
        //            TTControl loNextCtrl;
        //            /*ReinoTextBox loEditCtrl = null;*/
        //            TTableFldDef loField;
        //            int loNdx;

        //            iDestTable.ClearFieldValues();
        //            int loLoopMax = iDestTable.fTableDef.GetFieldCnt();
        //            for (loNdx = 0; loNdx < loLoopMax; loNdx++)
        //            {
        //                // Get next field and try to find an associated edit control
        //                loField = iDestTable.fTableDef.GetField(loNdx);
        //                loNextCtrl = FindCfgControlByName(loField.Name);
        //                TextBoxBehavior loBehavior = null;
        //                if ((loNextCtrl != null) && (loNextCtrl.Behavior != null))
        //                    loBehavior = loNextCtrl.Behavior;

        //                // If there's a non-blank edit control, grab its value
        //                if ((loBehavior != null) && (!loBehavior.FieldIsBlank()))
        //                {
        //                    iDestTable.SetFormattedFieldData(loNdx, loBehavior.GetEditMask(), loBehavior.EditBuffer);
        //                }
        //                // Is it the Form Revision field?
        //                else if ((loField.Name.Equals(FieldNames.FORMREVFieldName)) && (iForm is TIssForm))
        //                {
        //                    if (this.PrintPicture != null)
        //                    {
        //                        string loRevisionStr = this.PrintPicture.Revision.ToString();
        //                        iDestTable.SetFormattedFieldData(loNdx, "999", loRevisionStr);
        //                    }
        //                    else
        //                        iDestTable.SetFormattedFieldData(loNdx, "999", "0");
        //                }
        //                // Is it the Serial Number field?
        //                else if ((loField.Name.Equals(FieldNames.HHSerialNoFieldName)))
        //                {
        //                    iDestTable.SetFormattedFieldData(loNdx, "", IssueAppImp.GlobalIssueApp.GetHHSerialNumber());
        //                }
        //                // Is it the Form Name field?
        //                else if (loField.Name.Equals(FieldNames.FORMNAMEFieldName))
        //                {
        //                    if (iForm is TIssForm)
        //                    {
        //                        if (this.PrintPicture != null)
        //                            iDestTable.SetFormattedFieldData(loNdx, "", this.PrintPicture.Name);
        //                        else
        //                            iDestTable.SetFormattedFieldData(loNdx, "", iForm.Name);
        //                    }
        //                    else
        //                        iDestTable.SetFormattedFieldData(loNdx, "", iForm.Name);
        //                }
        //                else
        //                    iDestTable.SetFormattedFieldData(loNdx, "", "");
        //            }
        //            return 0;
        //        }



        /*
         * 
         *  AJW - implemented correctly in PrintManager - remove this code 
         *  
        public Bitmap PrintPicture(String Type, String Name)
        {
            // validate
            // read and write values.
            TIssPrnFormRev loPrintPic = null;
            IssStruct issStruct = GetStruct(Type, Name);
            WriteFieldValuesToPrintPicture(Type, Name);
            loPrintPic = issStruct.PrintPictureList;

            loPrintPic.PrepareForPrint();

            loPrintPic.Series3CE_ClearPrintCanvas(loPrintPic.Height, loPrintPic.Width);

            loPrintPic.PaintDescendants();

            return (loPrintPic.WriteBitmapData());
        }
         * */

        private void BuildTableDef(TIssStruct IssueStruct)
        {
            //TIssStruct IssueStruct = clientDef.IssStructMgr.IssStructs.Find(x => x != null && x.GetType().Name == IssStruct);

            TTableDef tableDef = IssueStruct.TableDefs[0];

            // test, should be foreach?
            if (IssueStruct.TableDefs.Count > 1)
            {
                tableDef = IssueStruct.TableDefs[0];  // debug break
            }


            List<TTableDefRev> TableDefRevs = tableDef.Revisions;

            //foreach (TTableDefRev tableDefRev in TableDefRevs)
            //{

            // get the highest revision 
            TTableDefRev tableDefRev = TableDefRevs[TableDefRevs.Count - 1];

            List<TableDefField> tableDefFields = new List<TableDefField>();

            foreach (TTableFldDef oneTableDefField in tableDefRev.Fields)
            {

                // AJW - TODO - do we want virtual fields in the database?? Need them from print pictures?

                
                // skip virtual fields
                if (oneTableDefField.GetType().Name.Contains("Virtual"))
                    continue;
                 

                
                string loFieldTypeNameUpper = oneTableDefField.GetType().Name.ToUpper();

                 /*
                // skip virtual fields
                if (loFieldTypeNameUpper.Contains("VIRTUAL"))
                {
                    continue;
                }

                // AJW TODO - we certainly don't want Virtual Link Fields, these are host side defnitions
                if (loFieldTypeNameUpper.Contains("VTABLELINK"))
                {
                    continue;
                }

                if (loFieldTypeNameUpper.Contains("VLINKED"))
                {
                    continue;
                }
                */



                tableDefFields.Add(new TableDefField
                {
                    Name = oneTableDefField.Name
                  ,
                    EditDataType = oneTableDefField.EditDataType.ToString()
                  ,
                    Size = oneTableDefField.Size
                  ,
                    Required = oneTableDefField.Required
                  ,
                    LayoutFileFieldTypeName = loFieldTypeNameUpper,

                    fTableFieldDef = oneTableDefField
                }
                                      );
            }

            TableDefs.Add(new TableDef
            {
                Name = tableDef.Name
             ,
                Revision = tableDefRev.Revision
             ,
                TableDefFields = tableDefFields,

                fTableDef = tableDef,
                fTableDefRev = tableDefRev,
                fIssueStructTableOwner = IssueStruct
            }
                             );
            //}
        }


        // AJW TODO - move this into appropriate source file
        private string GetDividerViewSubstitutionText(string iTSheetLabelText)
        {
            switch (iTSheetLabelText)
            {
                case "Loc":
                    {
                        return "Location";
                    }

                case "Rem":
                    {
                        return "Remarks";
                    }

                case "Veh":
                    {
                        return "Vehicle";
                    }

                case "Vio":
                    {
                        return "Violation";
                    }
                default:
                    {
                        // no subs, just return original
                        return iTSheetLabelText;
                    }
            }

        }



        protected void ProcessCfgStructures()
        {
            foreach (TIssStruct IssStruct in clientDef.IssStructMgr.IssStructs)
            {
                BuildIssStruct(IssStruct);
            }
        }


        /// <summary>
        /// Return the issueance form for passed TIssStruct
        /// </summary>
        /// <param name="iIssStruct"></param>
        /// <returns></returns>
        private TIssForm GetIssueFormForStruct(TIssStruct iIssStruct, int iIssueFormIdx )
        {

            string loStartsWith;
            if (iIssueFormIdx < 2)
            {
                loStartsWith = "ISSUE";
            }
            else
            {
                loStartsWith = "ISSUE" + iIssueFormIdx.ToString();
            }


            // Look through the forms defined in the passed TCiteStruct
            foreach (TTForm loForm in iIssStruct.Forms)
            {
                if (loForm.Name.ToUpper().StartsWith(loStartsWith) == true)
                {
                    return loForm as TIssForm;
                }

                //// Is this the issuance form?
                //if (loForm is TIssForm)
                //{
                //    return loForm as TIssForm;
                //}

            }
            // If we get this far, we couldn't find the target 
            return null;
        }

        protected void BuildIssStruct(TIssStruct IssueStruct)   //String StructType, String StructName)
        {
            //TIssStruct IssueStruct = clientDef.IssStructMgr.IssStructs.Find(x => x != null && x.GetType().Name == StructType && x.Name == StructName);


            // AJW - review - optimize and clean up
            // why do we want to create and maintain two copies of this data map? Only enough to adapt to Android env

            if (IssueStruct == null)
                return;
            else
                IssStructs.Add(new IssStruct());

            IssStructs[IssStructs.Count - 1].Name = IssueStruct.Name;
            IssStructs[IssStructs.Count - 1].SequenceName = IssueStruct.Sequence;
            IssStructs[IssStructs.Count - 1].Type = IssueStruct.GetType().Name;


            IssStructs[IssStructs.Count - 1]._TIssStruct = IssueStruct;

            //TODO: wire parent struct
            if (IssueStruct is TNotesStruct)
            {
                TNotesStruct tNotesStruct = (TNotesStruct)IssueStruct;
                IssStructs[IssStructs.Count - 1].ParentStruct = tNotesStruct.ParentStruct;
            }
            else if (IssueStruct is TVoidStruct)
            {
                TVoidStruct tVoidStruct = (TVoidStruct)IssueStruct;
                IssStructs[IssStructs.Count - 1].ParentStruct = tVoidStruct.ParentStruct;
            }
            else if (IssueStruct is TReissueStruct)
            {
                TReissueStruct tReissueStruct = (TReissueStruct)IssueStruct;
                IssStructs[IssStructs.Count - 1].ParentStruct = tReissueStruct.ParentStruct;
            }

            else if (IssueStruct is THotDispoStruct)
            {
                //
            }

            else if (IssueStruct is THotSheetStruct)
            {
                //
            }

            else if (IssueStruct is TSearchStruct)
            {
                //
            }

            // layout supports up to 2 ISSUE forms for the same structure
            for (int loIssueFormIdx = 1; loIssueFormIdx < 3; loIssueFormIdx++)
            {

                // find the issue form for this struct, if there is one
                TIssForm loThisStructIssueForm = GetIssueFormForStruct(IssueStruct, loIssueFormIdx);

                // defined for this index?
                if (loThisStructIssueForm == null)
                {
                    continue;
                }

                // is this beyond the first issue form index?
                if (loIssueFormIdx > 1)
                {
                    // we'll need to make a holder for each extra
                    IssStructs.Add(new IssStruct());

                    IssStructs[IssStructs.Count - 1].Name = IssueStruct.Name;
                    IssStructs[IssStructs.Count - 1].SequenceName = IssueStruct.Sequence;
                    IssStructs[IssStructs.Count - 1].Type = IssueStruct.GetType().Name;

                    IssStructs[IssStructs.Count - 1]._TIssStruct = IssueStruct;
                }

                // save for easy reference later
                IssStructs[IssStructs.Count - 1]._TIssForm = loThisStructIssueForm;


                // did we find an issue form?
                TBaseIssForm loThisStructBaseIssueForm = null;
                if (loThisStructIssueForm == null)
                {
                    // create an empty shell
                    loThisStructBaseIssueForm = new TBaseIssForm();
                }
                else
                {
                    // cast the base class for local use
                    loThisStructBaseIssueForm = (loThisStructIssueForm as TBaseIssForm);
                }



                //TBaseIssForm IssForm = new TBaseIssForm();

                ////TBaseIssForm loResult = null;
                ////TObjBasePredicate predicate = null;

                //// Find the first form in the structure with the desired name
                //TObjBasePredicate predicate = new TObjBasePredicate("ISSUE");   // "SELECT");
                //TBaseIssForm loResult = IssueStruct.Forms.Find(predicate.CompareByName_CaseInsensitive) as TBaseIssForm;
                //if (loResult != null)
                //{
                //    IssForm = loResult;
                //}
                //else
                //{
                //    // If we didn't find it yet, lets try again with a different name because the
                //    // layout tool converter switched names on us
                //    foreach (TTForm NextForm in IssueStruct.Forms)
                //    {
                //        //if (NextForm.Name.StartsWith("SELECT_", StringComparison.CurrentCultureIgnoreCase))
                //        if (NextForm.Name.StartsWith("ISSUE_", StringComparison.CurrentCultureIgnoreCase))
                //            IssForm = NextForm as TBaseIssForm;
                //    }
                //}

                ProcessPrintDefs(loThisStructBaseIssueForm);



                foreach (TWinClass NextCtrl in loThisStructBaseIssueForm.Controls)
                {


                    // first for look parent conntainers
                    // Is it a "TabSheet" container?
                    if (NextCtrl is TTTabSheet)
                    {
                        BuildControlsFromCfgTabSheet((NextCtrl as Reino.ClientConfig.TTTabSheet).Sheets, IssueStruct.Name); // Recursive
                    }
                    else
                    {
                        // else if because TTabsheet is also a TTPanel, but we don't want to generate controls for it 2x, only 1x from this outermost level, when defined

                        // Is it a TTPanel or descendant? (TTTabSheet, TPanel, TSheet)
                        if (NextCtrl is Reino.ClientConfig.TTPanel)
                        {
                            // do we have our one panel yet? TODO - move this per the efDivider strategy in BuildControlsFromCfgTabSheet
                            if (IssStructs[IssStructs.Count - 1].Panels.Count == 0)
                            {
                                IssStructs[IssStructs.Count - 1].Panels.Add(new Panel());
                            }

                            BuildControlsFromCfgContainer((NextCtrl as Reino.ClientConfig.TTPanel).Controls, IssueStruct.Name); // Recursive
                        }
                    }

                    // Is it a TTEdit or descendant? (TNumEdit, TIssEdit, TTMemo, TEditListBox)
                    if (NextCtrl is Reino.ClientConfig.TTEdit)
                    {
                        BuildEditCtrlFromCfg(NextCtrl as Reino.ClientConfig.TTEdit, IssueStruct.Name); // Specific
                    }


                    /*
                // Is it a TTButton?
                if (NextCtrl is Reino.ClientConfig.TTButton)
                    BuildActionBtnFromCfg(NextCtrl as Reino.ClientConfig.TTButton); // Specific

                // Is it a TTBitmap?
                if (NextCtrl is Reino.ClientConfig.TTBitmap)
                    BuildBitmapFromCfg(NextCtrl as Reino.ClientConfig.TTBitmap); //Specific
                     */
                }


                // does this issue form have a print picture?
                if (IssStructs[IssStructs.Count - 1].PrintPicture != null)
                {
                    // add a final page for printout review
                    IssStructs[IssStructs.Count - 1].Panels.Add(new Panel());
                }


                // determine what our date/time masks will be
                CultureDisplayFormatLogic oneFormatter = new CultureDisplayFormatLogic();
                IssStructs[IssStructs.Count - 1].fDisplayFormattingInfo = oneFormatter.ConstructFormattingTableInfoForIssueStruct(IssStructs[IssStructs.Count - 1]._TIssStruct, IssStructs[IssStructs.Count - 1]._TIssForm);


            }  // of 1..2 ISSUE form index


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iOneTSheet"></param>
        /// <param name="structName"></param>
        private void BuildOneTSheetContainer(Reino.ClientConfig.TSheet iOneTSheet, string structName)
        {
        }

        public virtual void BuildControlsFromCfgTabSheet(System.Collections.Generic.IList<TSheet> Container, string structName)
        {
            // Increment the list capacity for the number of objects we know about
            //this.BehaviorCollection.Capacity = this.BehaviorCollection.Capacity + Container.Count;
            // this.EntryOrder.Capacity = this.EntryOrder.Capacity + Container.Count;

            // lets force everything onto ONE panel list view
            bool loOnePanelAdded = false;
            int loPanelCounter = 0;
            string loLastPanelName = String.Empty;

            foreach (Reino.ClientConfig.TSheet NextCtrl in Container)
            {
                // Is it a TTPanel or descendant? (It certainly should be since TSheet inherits from TTPanel)
                if (NextCtrl is Reino.ClientConfig.TTPanel)
                {
                    // do we have our one panel yet?
                    if (loOnePanelAdded == false)
                    {
                        IssStructs[IssStructs.Count - 1].Panels.Add(new Panel());
                        loOnePanelAdded = true;
                    }

                    // AJW TODO - move this into isolated method focueds on TPanel so it can be called for any cnfg, incl forms that only have TPanels but no TTabsheet containers

                    // at the top of each panel grouping
                    // add text view divider groupings to enhance navigation
                    try
                    {
                        loPanelCounter++;

                        // dereference for simplified access
                       TTPanel onePanel = (Reino.ClientConfig.TTPanel)NextCtrl;


                        // keep track, if we have duplicate panel names we dont want the second one
                       if (onePanel.Name.Equals(loLastPanelName) == false)
                       {
                           PanelField onePanelDivider = new PanelField();
                           onePanelDivider.Name = structName + AutoISSUE.DBConstants.gPanelDividerNameSuffix + " " + loPanelCounter.ToString();
                           loLastPanelName = onePanel.Name;

                           //onePanelDivider.Label = GetDividerViewSubstitutionText(onePanel.Name);
                           // TODO - this should be using layout margins to place the text
                           //onePanelDivider.Label = " " + GetDividerViewSubstitutionText(onePanel.Name).ToUpper();  // all upper case for UX consistency?
                           onePanelDivider.Label = " " + GetDividerViewSubstitutionText(onePanel.Name);  
                           
                           onePanelDivider.fParentStructName = structName;
                           onePanelDivider.FieldType = TEditField.TEditFieldType.efDivider.ToString();

                           // not used, but referenced unsafely so we have to define it
                           var optionsLst = new DBList();
                           onePanelDivider.OptionsList = optionsLst;

                           IssStructs[IssStructs.Count - 1].Panels[IssStructs[IssStructs.Count - 1].Panels.Count - 1].PanelFields.Add(onePanelDivider);
                       }
                    }
                    catch (Exception e)
                    {
                        int i = 0;
                    }
     
                    var controls = (NextCtrl as Reino.ClientConfig.TTPanel).Controls;
                    BuildControlsFromCfgContainer(controls, structName); // recursive
                }
            }
        }


        public virtual void BuildControlsFromCfgContainer(System.Collections.Generic.IList<TWinClass> Container, string structName)
        {
            // Increment the list capacity for the number of objects we know about
            // this.BehaviorCollection.Capacity = this.BehaviorCollection.Capacity + Container.Count;
            // this.EntryOrder.Capacity = this.EntryOrder.Capacity + Container.Count;

            foreach (TWinClass NextCtrl in Container)
            {
                // Is it a "TabSheet" container?
                //  if (NextCtrl is Reino.ClientConfig.TTTabSheet)
                //    BuildControlsFromCfgTabSheet((NextCtrl as Reino.ClientConfig.TTTabSheet).Sheets); // Recursive

                // Is it a TTPanel or descendant? (TTTabSheet, TPanel, TSheet)
                //if (NextCtrl is Reino.ClientConfig.TTPanel)
                //BuildControlsFromCfgContainer((NextCtrl as Reino.ClientConfig.TTPanel).Controls); // Recursive

                // Is it a TTEdit or descendant? (TNumEdit, TIssEdit, TTMemo, TEditListBox)
                if (NextCtrl is Reino.ClientConfig.TTEdit)
                    BuildEditCtrlFromCfg(NextCtrl as Reino.ClientConfig.TTEdit, structName); // Specific
                /*
            // Is it a TTButton?
            if (NextCtrl is Reino.ClientConfig.TTButton)
                BuildActionBtnFromCfg(NextCtrl as Reino.ClientConfig.TTButton); // Specific

            // Is it a TTBitmap?
            if (NextCtrl is Reino.ClientConfig.TTBitmap)
                BuildBitmapFromCfg(NextCtrl as Reino.ClientConfig.TTBitmap); //Specific
                 */
            }
        }


        protected virtual void BuildEditCtrlFromCfg(Reino.ClientConfig.TTEdit pCtrl, string structName)
        {
            // Add this configuration control to the EntryOrder list for easier navigation
            //            EntryOrder.Add(pCtrl);

            // If its a TEditListBox then we're building a listbox instead of a textbox
            if (pCtrl is TEditListBox)
            {
                //BuildListBoxFromCfg(pCtrl as TEditListBox);
                return;
            }

            // If its a TDrawField then we're building a ReinoDrawBox instead of a textbox
            if (pCtrl is TDrawField)
            {
                //              BuildDrawBoxFromCfg(pCtrl as TDrawField);
                // return;
                // keep going for signature capture

                int i = 1;


            }

            if (pCtrl.Name.Equals("SELECTION"))
            {
                return;
            }






            String charParamForceLiteral = "";
            int intParamForceList = 0;
            int intParamForceCurrDtTime = -1; //iF TER_ForceCurrentDateTime exists then value will be set greater than -1
            String setGlobalCurrentValue = "";
            String forceGlobalCurrentValue = "";
            String Val = "";
            String parentCtl = "";
            String dataPrm = "";
            String editMsk = "";
            var fieldType = TEditField.TEditFieldType.efString;
            DBList optionsLst = new DBList();

            if (pCtrl is TEditField)
            {
                editMsk = (pCtrl as TEditField).EditMask;
                fieldType = (pCtrl as TEditField).FieldType;
                optionsLst = new DBList
                {
                    ListName = ((TEditField)pCtrl).ListName.Split(' ')[0]
                    ,
                    IsListOnly = (pCtrl.Restrictions.Any(x => x.Name == "LISTONLY"))
                    ,
                    Columns = (pCtrl as TEditField).DBListGrid.Columns.Select(x => x.Name).ToArray()
                };
            }

            // there's possible a bug, so set this string here to allocate memory before we fill the object below.
            if (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_ChildList") != null)
            {
                parentCtl = (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_ChildList")).ControlEdit1;
                dataPrm = (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_ChildList")).CharParam;
            }

            if (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_ForceLiteral") != null)
            {
                charParamForceLiteral = (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_ForceLiteral")).CharParam;
            }

            if (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_ForceListItem") != null)
            {
                intParamForceList = (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_ForceListItem")).IntParam;
            }

            if (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_ForceCurrentDateTime") != null)
            {
                intParamForceCurrDtTime = (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_ForceCurrentDateTime")).IntParam;
            }

            if (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_SetGlobalCurrentValue") != null)
            {
                setGlobalCurrentValue = (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_SetGlobalCurrentValue")).Name;
            }

            if (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_ForceGlobalCurrentValue") != null)
            {
                forceGlobalCurrentValue = (pCtrl.Restrictions.FirstOrDefault(x => x.GetType().Name == "TER_ForceGlobalCurrentValue")).Name;
            }


            Single width;

            if (pCtrl.Width < 100)
                width = .25f;   // quarter
            else if (pCtrl.Width < 200)
                width = .50f;  // half
            else
                width = 1.0f;  // full width

            try
            {
                IssStructs[IssStructs.Count - 1].Panels[IssStructs[IssStructs.Count - 1].Panels.Count - 1].PanelFields.Add(new PanelField
                {
                    Name = pCtrl.Name
,
                    Label = pCtrl.PromptWin.TextBuf
,
                    Width = width
,
                    MaxLength = pCtrl.MaxLength
,
                    Value = Val
,
                    IsRequired = (pCtrl.Restrictions.Any(x => x.Name == "REQUIRED"))
,
                    // IsReadOnly = (pCtrl.Restrictions.Any(x => x.Name == "PROTECTED"))
                    //  ,
                    IsHidden = (pCtrl.Restrictions.Any(x => x.Name == "HIDDEN"))
,
                    ParentControl = parentCtl
,
                    DataParam = dataPrm
,
                    EditMask = editMsk
,
                    FieldType = fieldType.ToString()
,
                    OptionsList = optionsLst
,
                    // CharParamForceLiteral = charParamForceLiteral
                    // ,
                    // IntParamForceList = intParamForceList
                    //  ,
                    IntParamForceCurrDtTime = intParamForceCurrDtTime
,
                    // IsValidateInches = (pCtrl.Restrictions.Any(x => x.Name == "ValidateInches"))
                    //  ,
                    //  IsForceCleared = (pCtrl.Restrictions.Any(x => x.Name == "ForceCleared"))
                    //  ,
                    //  SetGlobalCurrentValue = setGlobalCurrentValue
                    //  ,
                    //  ForceGlobalCurrentValue = forceGlobalCurrentValue

                    fEditFieldDef = pCtrl,
                    fParentStructName = structName,
                });
            }
            catch (Exception e)
            {
                int i = 0;
            }

        }


    } // end class 


}   // end namespace
