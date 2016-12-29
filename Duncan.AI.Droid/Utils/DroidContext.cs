
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Util;
using Android.Gestures;
using Android.OS;
using Duncan.AI.Droid.Utils.EditControlManagement;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Java.IO;

using Duncan.AI.Droid.Utils;
using Duncan.AI.Droid.Utils.DataAccess;
//using System.Duncan.Drawing;
using Android.Graphics;
using Duncan.AI.Droid.Utils.HelperManagers;

using Android.App;
using Android.Views;

using Reino.ClientConfig;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils.PrinterSupport;


namespace Duncan.AI.Droid
{
    public static class DroidContext
    {

        //
        // Only use getApplicationContext() when you know you need a Context for something that may live longer than any other 
        // likely Context you have at your disposal. Scenarios include:
        // 
        //  - Use getApplicationContext() if you need something tied to a Context that itself will have global scope.
        //
        //  - Use getApplicationContext() when you bind to a Service from an Activity, if you wish 
        //    to pass the ServiceConnection (i.e., the handle to the binding) between Activity instances via onRetainNonConfigurationInstance()
        //
        // http://stackoverflow.com/questions/7298731/when-to-call-activity-context-or-application-context
        //
        //
        //  In most cases, use the Context directly available to you from the enclosing component you’re working within. You can safely hold 
        //  a reference to it as long as that reference does not extend beyond the lifecycle of that component. As soon as you need to save 
        //  a reference to a Context from an object that lives beyond your Activity or Service, even temporarily, switch that reference you save 
        //  over to the application context.
        // 
        private static Context _gGlobalScopeApplicationContext;
        public static Context ApplicationContext
        {
            get
            {
                return _gGlobalScopeApplicationContext;
            }
        }

        public static void SetGlobalApplicationContext(Context iGlobalApplicationContext)
        {
            if (_gGlobalScopeApplicationContext == null)
            {
                _gGlobalScopeApplicationContext = iGlobalApplicationContext;
            }
            else
            {
                // not an error: this is updated from SplashActivity, which can be invoked more than once
                //System.Console.WriteLine("Error: Attempting to update global application scope");
            }
        }


        public static MainActivity mainActivity { get; set; }

        public static bool XmlHasBeenLoaded { get; set; }

        // these few bits are first updated by login manager, prior to full XML load
        public static string XmlClientName { get; set; }
        public static string XmlAgencyDesignator { get; set; }
        public static int XmlLayoutRevision { get; set; }

        public static string SyncLastResultText { get; set; }


        public static int CitationsSinceRestart = 0;

        /// <summary>
        /// Load the XML configuration from the internal filesystem
        /// </summary>
        public static void XmlCfgLoadFromInternalFilesystem()
        {
            try
            {
                // load our registry file first - settings defined at the host system for us to follow
                AIFileSystemDAO loRegistryFile = ClientFileManager.GetDataForFileSystemFile(TTRegistry.RegistryFileNameOnHandheld);
                if (loRegistryFile != null)
                {
                    // read it in
                    TTRegistry.InitRegistry(loRegistryFile.FILEDATA);

                    // extract and set globals for wireless enforcement
                    ExternalEnforcementInterfaces.EvaluateWirelessEnforcementOptions();
                }
                else
                {
                    // just init empty registry
                    TTRegistry.InitRegistry(null);
                }


                CitationsSinceRestart = 0;


                _xmlCfg = new AndroidConfigData(ApplicationContext);
                

                AIFileSystemDAO loIssueApXMLConfig = ClientFileManager.GetDataForFileSystemFile(Constants.ISSUE_AP_XML_FILENAME);

                // not in there? 
                if (loIssueApXMLConfig == null)
                {
                    // nothing to do
                    return;
                }

                // load it up
                _xmlCfg.GetConfig(loIssueApXMLConfig.FILEDATA);


                // update these for global reference
                XmlClientName = _xmlCfg.clientDef.Client;
                XmlAgencyDesignator = _xmlCfg.clientDef.AgencyDesignator;
                XmlLayoutRevision = _xmlCfg.clientDef.Revision;


                //now lets resolve the object references
                foreach (EditControlBehavior nextBehavior in _xmlCfg.BehaviorCollection)
                {
                    foreach (EditRestriction loEditRestrict in nextBehavior.EditRestrictions)
                        loEditRestrict.ResolveObjectReferences();
                    foreach (EditCondition loEditCondition in nextBehavior.EditConditions)
                        loEditCondition.ResolveObjectReferences();
                }

                foreach (XMLConfig.IssStruct structType in _xmlCfg.IssStructs)
                {
                    for (int i = 0; i < structType.Panels.Count; i++)
                    {
                        foreach (XMLConfig.PanelField panelField in structType.Panels[i].PanelFields)
                        {
                            if (panelField.ParentControl != null)
                            {
                                foreach (XMLConfig.IssStruct structTypeSub in _xmlCfg.IssStructs)
                                {
                                    for (int j = 0; j < structTypeSub.Panels.Count; j++)
                                    {
                                        foreach (XMLConfig.PanelField panelFieldSub in structTypeSub.Panels[j].PanelFields)
                                        {
                                            if (panelFieldSub.Name == panelField.ParentControl)
                                            {
                                                panelField.ParentPanelField = panelFieldSub;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //// 2nd pass will attach child list after the object references have been created
                ////todo - when we do list items
                //foreach (EditControlBehavior nextBehavior in _XmlCfg.BehaviorCollection)
                //{
                //    // Look through restrictions of current edit control
                //    foreach (EditRestriction loEditRestrict in nextBehavior.EditRestrictions)
                //    {
                //        // If this is a ChildList restriction, we need to tie the appropriate list to the ReinoTextBox
                //        if (loEditRestrict is TER_ChildList)
                //        {
                //            var controller = loEditRestrict.ControlEdit1Obj;
                //            if (controller != null)
                //            {
                //                var loChildListRestrict = loEditRestrict as  TER_ChildList;
                //                // Set the list source referenced by the child restriction
                //                loChildListRestrict.ListSourceTable = controller.NaturalListSourceTable;
                //                loChildListRestrict.ListSourceFieldName = loEditRestrict.CharParam;
                //            }
                //        }
                //    }
                //}
                XmlHasBeenLoaded = true;

            }
            catch (Exception e)
            {
                System.Console.WriteLine("Exception source: {0}", e.Source);
                
                // re-throw exactly as is
                throw;
            }

        }

        private static AndroidConfigData _xmlCfg;
        public static AndroidConfigData XmlCfg
        {
            set
            {
                _xmlCfg = null;
            }

            get
            {
                try
                {
                    if (_xmlCfg == null)
                    {
                        Log.Info("Context", "XmlCfg is Null.... loading...");
                        XmlCfgLoadFromInternalFilesystem();
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Exception source: {0}", e.Source);

                    // re-throw exactly as is
                    throw;
                }

                return _xmlCfg;
            }
        }




        // AJW TODO - move all of these kind of properties into Registry.cs object for auto-update when synced 
        private static int _gLocationUpdateDuration = -1;
        public static int gLocationUpdateDuration
        {
            get
            {
                // we only need to pull this from the registry once - until the reg is reset, need a notification for that
                if (_gLocationUpdateDuration != -1)
                {
                    return _gLocationUpdateDuration;
                }

                if (_xmlCfg != null)
                {
                    if (TTRegistry.glRegistry != null)
                    {
                        {
                            _gLocationUpdateDuration = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regLOCATION_UPDATE_MIN, TTRegistry.regLOCATION_UPDATE_MIN_DEFAULT);
                            return _gLocationUpdateDuration;
                        }
                    }
                }
                return TTRegistry.regLOCATION_UPDATE_MIN_DEFAULT;
            }
        }

        // AJW TODO - move all of these kind of properties into Registry.cs object for auto-update when synced 
        private static int _gFormEntryFocusFieldPlaceAtBottom = -1;
        public static int gFormEntryFocusFieldPlaceAtBottom
        {
            get
            {
                // we only need to pull this from the registry once - until the reg is reset, need a notification for that
                if (_gFormEntryFocusFieldPlaceAtBottom != -1)
                {
                    return _gFormEntryFocusFieldPlaceAtBottom;
                }
                

                if (_xmlCfg != null)
                {
                    if (TTRegistry.glRegistry != null)
                    {
                        {
                            _gFormEntryFocusFieldPlaceAtBottom = TTRegistry.glRegistry.GetRegistryValueAsInt( TTRegistry.regSECTION_ISSUE_AP, 
                                                                                                      TTRegistry.regFORM_ENTRY_FOCUS_FIELD_PLACEMENT_BOTTOM,
                                                                                                      TTRegistry.regFORM_ENTRY_FOCUS_FIELD_PLACEMENT_BOTTOM_DEFAULT);
                            return _gFormEntryFocusFieldPlaceAtBottom;
                        }
                    }
                }
                return TTRegistry.regFORM_ENTRY_FOCUS_FIELD_PLACEMENT_BOTTOM_DEFAULT;
            }
        }



        // AJW TODO - move all of these kind of properties into Registry.cs object for auto-update when synced 
        private static string _gFormEntryFirstFocusParking = null;
        public static string gFormEntryFirstFocusParking
        {
            get
            {
                // we only need to pull this from the registry once - until the reg is reset, need a notification for that
                if (string.IsNullOrEmpty(_gFormEntryFirstFocusParking ) == false)
                {
                    return _gFormEntryFirstFocusParking;
                }


                if (_xmlCfg != null)
                {
                    if (TTRegistry.glRegistry != null)
                    {
                        {
                            _gFormEntryFirstFocusParking = TTRegistry.glRegistry.GetRegistryValue( TTRegistry.regSECTION_ISSUE_AP,
                                                                                                   TTRegistry.regFORM_ENTRY_FIRST_FOCUS_PARKING,
                                                                                                   TTRegistry.regFORM_ENTRY_FIRST_FOCUS_PARKING_DEFAULT);
                            return _gFormEntryFirstFocusParking;
                        }
                    }
                }
                return TTRegistry.regFORM_ENTRY_FIRST_FOCUS_PARKING_DEFAULT;
            }
        }


        // AJW TODO - move all of these kind of properties into Registry.cs object for auto-update when synced 
        private static string _gFormEntryFirstFocusTraffic = null;
        public static string gFormEntryFirstFocusTraffic
        {
            get
            {
                // we only need to pull this from the registry once - until the reg is reset, need a notification for that
                if (string.IsNullOrEmpty(_gFormEntryFirstFocusTraffic) == false)
                {
                    return _gFormEntryFirstFocusTraffic;
                }


                if (_xmlCfg != null)
                {
                    if (TTRegistry.glRegistry != null)
                    {
                        {
                            _gFormEntryFirstFocusTraffic = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP,
                                                                                                   TTRegistry.regFORM_ENTRY_FIRST_FOCUS_TRAFFIC,
                                                                                                   TTRegistry.regFORM_ENTRY_FIRST_FOCUS_TRAFFIC_DEFAULT);
                            return _gFormEntryFirstFocusTraffic;
                        }
                    }
                }
                return TTRegistry.regFORM_ENTRY_FIRST_FOCUS_TRAFFIC_DEFAULT;
            }
        }


        // AJW TODO - move all of these kind of properties into Registry.cs object for auto-update when synced 
        private static string _gFormEntryFirstFocusMarkMode = null;
        public static string gFormEntryFirstFocusMarkMode
        {
            get
            {
                // we only need to pull this from the registry once - until the reg is reset, need a notification for that
                if (string.IsNullOrEmpty(_gFormEntryFirstFocusMarkMode) == false)
                {
                    return _gFormEntryFirstFocusMarkMode;
                }


                if (_xmlCfg != null)
                {
                    if (TTRegistry.glRegistry != null)
                    {
                        {
                            _gFormEntryFirstFocusMarkMode = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP,
                                                                                                   TTRegistry.regFORM_ENTRY_FIRST_FOCUS_MARKMODE,
                                                                                                   TTRegistry.regFORM_ENTRY_FIRST_FOCUS_MARKMODE_DEFAULT);
                            return _gFormEntryFirstFocusMarkMode;
                        }
                    }
                }
                return TTRegistry.regFORM_ENTRY_FIRST_FOCUS_MARKMODE_DEFAULT;
            }
        }



        public static class MyFragManager
        {

            // internal fragment stack for custom navigation
            private static List<string> gInternalBackStack = new List<string>();

            /// <summary>
            /// Internal backstack for (limited) custom navigation, for citation review / notes, etc that can be accessed from different flow points
            /// </summary>
            /// <param name="iFragmentTag"></param>
            public static void AddToInternalBackstack(string iFragmentTag)
            {
                // defeind against double-click 
                if (PeekInternalBackstack().Equals(iFragmentTag) == false)
                {
                    gInternalBackStack.Add(iFragmentTag);
                }
                else
                {
                    // debug break point
                    iFragmentTag = iFragmentTag.Trim();
                }
            }

            public static int GetInternalBackstackCount()
            {
                return gInternalBackStack.Count;
            }

            public static string PeekInternalBackstack()
            {
                string loPeekResultTag = string.Empty;

                int loPeekIndex = gInternalBackStack.Count;

                if (loPeekIndex > 0)
                {
                    loPeekIndex--;
                    loPeekResultTag = gInternalBackStack[loPeekIndex];
                }

                return loPeekResultTag;
            }

            public static string PopInternalBackstack()
            {
                string loPopResultTag = string.Empty;

                int loPopIndex = gInternalBackStack.Count;


                if (loPopIndex > 0)
                {
                    loPopIndex--;
                    loPopResultTag = gInternalBackStack[loPopIndex];
                    gInternalBackStack.RemoveAt(loPopIndex);
                }

                return loPopResultTag;
            }

            public static void ClearInternalBackstack()
            {
                gInternalBackStack.Clear();
            }


            public static void HideThisAndShowThat(string iFragToHideTag, string iFragToShowTag)
            {

                /*

                        FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();

                        Fragment detailFragment = FragmentManager.FindFragmentByTag(Constants.ISSUE_REVIEW_DETAIL_FRAGMENT_TAG);
                        if (detailFragment != null)
                        {
                            fragmentTransaction.Hide(detailFragment);
                        }

                        var notesFragment = (NotesFragment)FragmentManager.FindFragmentByTag(Constants.NOTES_FRAGMENT_TAG);
                        if (notesFragment != null)
                        {
                            fragmentTransaction.Show(notesFragment);
                            notesFragment.GetNotesByTicket();
                        }
                        else
                        {

                            //fragmentTransaction.Replace(Resource.Id.frameLayout1, new NotesFragment(), Constants.NOTES_FRAGMENT_TAG);

                            Android.App.Fragment fragment = new NotesFragment { Arguments = new Bundle() };
                            fragment.Arguments.PutString("structName", _structName);

                            string loTag = Constants.NOTES_FRAGMENT_TAG;
                            //MainActivity.RegisterFragment(fragment, loTag, _structName, "NoMenu " + loTag, FragmentClassificationType.fctSecondaryActivity, -1, -1);
                            fragmentTransaction.Add(Resource.Id.frameLayout1, fragment, loTag);
                        }


                        //// add this to the backstack so we can come back here when done
                        //// TODO - need to account for skipping around with drawer menus
                        //fragmentTransaction.AddToBackStack(null);

                        fragmentTransaction.Commit();
                 */
            }

        }


        #region CurrentGlobalValues

        //private static Hashtable _CurrentValues = new Hashtable();

        //public static string GetCurrentGlobalFieldValue(string iFieldName)
        //{
        //    // return if no current global value for this field
        //    if (_CurrentValues.ContainsKey(iFieldName) == false)
        //        return "";
        //    else
        //        return _CurrentValues[iFieldName].ToString();
        //}

        //public static void SetCurrentGlobalFieldValue(string iFieldName, string iFieldValue)
        //{
        //    // Add or update the value for the passed field
        //    if (_CurrentValues.ContainsKey(iFieldName) == false)
        //        _CurrentValues.Add(iFieldName, iFieldValue);
        //    else
        //        _CurrentValues[iFieldName] = iFieldValue;
        //}

        #endregion


        public static PrintersSupported gPrinterSelected 
        {
            get
            {
                return PrinterSupport_BaseClass.GetPrinterEnumForPrinterNameString(Constants.PRINTER_TYPE);
            }
        }

#if _integrate_n5_support_
        public static PrinterSupport_TwoTechN5Printer N5Interface = null;
#endif


        #region PendingAttachments

        public static List<string> gPhotosTakenForPendingAttatchment = new List<string>();

        /// <summary>
        /// Add watermark to a taken photo and add it to the attachment pending list.
        /// </summary>
        /// <param name="structName"></param>        
        public static bool AddPhotoToPendingAttachmentList(string iFileName, int rotationAngle)
        {

            // try clearing after every photo. does is help?
            MemoryWatcher.DeleteApplicationCache(ApplicationContext);


            if (iFileName == String.Empty) return false;
            Bitmap loImageBitmap = null;
            System.Duncan.Drawing.Graphics loImageGraphics = null;
            Java.IO.FileOutputStream loOutStream = null;

            try
            {
                if (TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                               TTRegistry.regADDWATERMARK_TO_PICTURE,
                                                               TTRegistry.regADDWATERMARK_TO_PICTURE_DEFAULT) > 0)
                {
                    //Check the photo orintation first
                    //Before we add the photo to the list add the timestamp                    
                    Android.Graphics.BitmapFactory.Options loOptions = new Android.Graphics.BitmapFactory.Options();
                    loOptions.InMutable = true;

                    //FixMe: Image rotation causes memory leakage 
                    //if (rotationAngle == 0)
                    //{
                        loImageBitmap = BitmapFactory.DecodeFile(iFileName, loOptions);
                    //}
                    //else                    
                    //{
                    //    Bitmap loOrgImageBitmap = BitmapFactory.DecodeFile(iFileName, loOptions);
                    //    if (loOrgImageBitmap == null) return false;
                    //    loImageBitmap = BitmapHelpers.RotateBitmap(loOrgImageBitmap, rotationAngle);                        
                    //    loOrgImageBitmap.Recycle();                    
                    //}

                    if (loImageBitmap == null) return false;

                    loImageGraphics = new System.Duncan.Drawing.Graphics(loImageBitmap);
                    
                    //Build the watermark string
                    File loImageFile = new File(iFileName);
                    string loWaterMarkStr = String.Empty;
                    string loDateFormat = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP,
                                                                                 TTRegistry.regWATERMARK_DATE_FORMAT,
                                                                                 TTRegistry.regWATERMARK_DATE_FORMAT_DEFAULT);
                    string loTimeFormat = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP,
                                                                                 TTRegistry.regWATERMARK_TIME_FORMAT,
                                                                                 TTRegistry.regWATERMARK_TIME_FORMAT_DEFAULT);
                    int loLocation = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                 TTRegistry.regWATERMARK_LOCATION,
                                                                                 TTRegistry.regWATERMARK_LOCATION_DEFAULT);
                    var dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(loImageFile.LastModified());
                    if (string.IsNullOrEmpty(loDateFormat)) loDateFormat = "MM/dd/yyyy";
                    if (string.IsNullOrEmpty(loTimeFormat)) loTimeFormat = "hh:mm:ss";
                    var loDateStr = dt.ToString(loDateFormat);
                    var loTimeStr = dt.ToString(loTimeFormat);
                    loWaterMarkStr = loDateStr + " " + loTimeStr;

                    System.Duncan.Drawing.RectangleF loRect = new System.Duncan.Drawing.RectangleF();

                    // place timestamp text according to the size of the image  	                 
                    int loTopOffset = (int)((80.00 * loImageBitmap.Height) / 800.00);
                    int loXOffset = (int)((20.00 * loImageBitmap.Height) / 800.00);
                    int loFontHeight = 8 + (int)((32.00 * loImageBitmap.Height) / 800.00);

                    loRect.Height = loImageBitmap.Height - loRect.Top;
                    loRect.Width = loImageBitmap.Width - loRect.Left;

                    if (loLocation == TTRegistry.WATERMARK_LOCATION_LOWER_LEFT || loLocation == TTRegistry.WATERMARK_LOCATION_UPPER_LEFT)
                    {
                        loRect.Left = 5;
                    }
                    else
                    {
                        loRect.Left = loImageBitmap.Width - loRect.Width - 5;
                    }

                    if (loLocation == TTRegistry.WATERMARK_LOCATION_UPPER_LEFT || loLocation == TTRegistry.WATERMARK_LOCATION_UPPER_RIGHT)
                    {
                        loRect.Top = loTopOffset;
                    }
                    else
                    {
                        loRect.Top = loImageBitmap.Height - loTopOffset;
                    }

                    System.Duncan.Drawing.Font loFont = new System.Duncan.Drawing.Font("sansserif", loFontHeight, System.Duncan.Drawing.FontStyle.Regular, System.Duncan.Drawing.Justification_Android.Center,System.Duncan.Drawing.Rotation_Android.Rotate0);
                    System.Duncan.Drawing.Brush loBrush = new System.Duncan.Drawing.Brush(Android.Graphics.Color.Black);
                    loImageGraphics.DrawString(loWaterMarkStr, loFont, loBrush, loRect);

                    byte[] loBitmapData;
                    using (var stream = new System.IO.MemoryStream())
                    {
                        loImageBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, stream);
                        loBitmapData = stream.ToArray();
                    }
                    
                    
                    loOutStream = new Java.IO.FileOutputStream(iFileName);
                    loOutStream.Write(loBitmapData);
                    loOutStream.Close();
                    loOutStream.Dispose();

                    loImageBitmap.Recycle();
                    loImageGraphics.Dispose();
                    loBitmapData = null;
                }

                // does this help the N5 device?
                GC.Collect();

                //Now add the photo to the pending list
                gPhotosTakenForPendingAttatchment.Add(iFileName);
                return true;
            }
            catch (Exception ex)
            {
                if (loOutStream != null)
                {
                    loOutStream.Close();
                    loOutStream.Dispose();
                }
                if (loImageBitmap != null)
                {                    
                    loImageBitmap.Recycle();
                }
                if (loImageGraphics != null)
                {
                    loImageGraphics.Dispose();
                }
                LoggingManager.LogApplicationError(ex, "AddPhotoToPendingAttachmentList", ex.TargetSite.Name);                
                //System.Console.WriteLine("AddPhotoToPendingAttachmentList Exception source {0}: {1}", ex.Source, ex.ToString());                
                HandleExceptions(ex);
                return false;
            } 
        }

        #endregion

        private static void HandleExceptions(Exception e)
        {
            LoggingManager.LogApplicationError(e, "DriodContext Exception", e.TargetSite.Name);
            ErrorHandling.ReportExceptionWithConfirmationDlg(e.Message);
        }

        public static object gWebViewCacheAccessLock = new object();
        public static bool gWebViewCacheHasBeenCleared = false;


        public static object gApplicationCacheAccessLock = new object();
        public static bool gApplicationCacheHasBeenCleared = false;

        /// <summary>
        /// returns TRUE if java pop-up dialogs should be allowed
        /// </summary>
        /// <returns></returns>
        public static bool UseChromeWebViewClient()
        {

            // TODO - read this from registry
            return true;
        }




        /// <summary>
        /// Resets the form status and focus attributes for all behaviors for this collection.
        /// </summary>
        /// <param name="structName"></param>
        public static void ResetControlStatusByStructName(string structName)
        {

            // reset any GIS data from last enforcement
            ExternalEnforcementInterfaces.ClearGISMeterJsonValueObject();   


            // 
            mainActivity.SetPreventESCReset();

                        

            // init for edit
            XMLConfig.IssStruct nextIssueStruct = _xmlCfg.GetStructByName(structName);
            if (nextIssueStruct != null)
            {
                nextIssueStruct._rowID = null;
                nextIssueStruct._MasterKey = null;

                nextIssueStruct.chalkRowId = null;
                nextIssueStruct.sequenceId = null;
                nextIssueStruct.prefix = null;
                nextIssueStruct.suffix = null;
            }



            // TODO - should we delete these files? not here, they should be deleted if not attached
            // these are deleted when attached gPhotosTakenForPendingAttatchment.Clear();



            // DOING THIS IN PREPAREFOREDIT now
            // find the target struct and reset it
            //foreach (var nextBehavior in _xmlCfg.BehaviorCollection.Where(x => x.StructName == structName))
            //{
            //    nextBehavior.ResetFormState(nextBehavior);
            //}


            // why 2x?
            //// find the target struct and reset it
            //foreach (var nextBehavior in _xmlCfg.BehaviorCollection.Where(x => x.StructName == structName))
            //{
            //    nextBehavior.ResetFormState(nextBehavior);
            //}




            //// AJW - TODO - not enough, this needs to call PrepareForEdit to get visual only components
            //// init for edit
            //XMLConfig.IssStruct nextIssueStruct = _xmlCfg.GetStructByName(structName);
            //if (nextIssueStruct != null)
            //{
            //    int noOfColms = 0;
            //    for (int i = 0; i < nextIssueStruct.Panels.Count; i++)
            //    {
            //        foreach (XMLConfig.PanelField panelField in nextIssueStruct.Panels[i].PanelFields)
            //        {
            //            var oneBehavior = _xmlCfg.BehaviorCollection.FirstOrDefault(x => x.PanelField.Name == panelField.Name);

            //            if (oneBehavior != null)
            //            {
            //                oneBehavior.InvokeFormInitEventRestriction(oneBehavior.EditCtrl);
            //            }
            //        }
            //    }
            //}

        }

    }
}
