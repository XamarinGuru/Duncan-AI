﻿

using System;
using System.Threading;

using Android.App;
using Android.Content.Res;

using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Duncan.AI.Droid;
using Duncan.AI.Droid.Common;
using Duncan.AI.Droid.Fragments;
using Duncan.AI.Droid.Utils;

using System.Collections.Generic;
using Duncan.AI.Droid.Utils.HelperManagers;
using Duncan.AI.Droid.Utils.PrinterSupport;
using Duncan.AI.Droid.Utils.PickerDialogs;

#if !_integrate_n5_support_
using ZXing;
using ZXing.Mobile;
#endif

using Reino.ClientConfig;
using XMLConfig;

using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Widget;


namespace Duncan.AI.Droid
{

    public enum FragmentClassificationType 
    {
        fctActionBarActivity,    // main fragment matching tab heading
        fctIssueItemActivity,    // issue new
        fctSelectItemActivty,    // select/lookup 
        fctSecondaryActivity,     // secondary fragments that may be shown and hidden on return - notes, summary, dialogs
        fctApplicationSettingsActivity   // application settings dialogs
    }
        

    public class FragmentRegistation
    {
        public Android.App.Fragment aFragment;
        public string aFragmentTag;
        public string aTitleText;
        public FragmentClassificationType aFragType;
        public string structName;
        public int GroupPosition;
        public int ChildPosition;
    }



    [Activity( 
               Label = "",
               //Icon = "@drawable/ic_app_title", 
               Icon = null, 
               ScreenOrientation = ScreenOrientation.Portrait,
               Theme = "@android:style/Theme.Holo.Light", 
               LaunchMode = Android.Content.PM.LaunchMode.SingleTask, // may help with camera activity result? test this photo volumne
               WindowSoftInputMode = SoftInput.StateHidden
               )]
    public class MainActivity : Activity, 
                                //ActionBar.ITabListener, 
                                ExpandableListView.IOnChildClickListener, 
                                ExpandableListView.IOnGroupClickListener, 
                                ExpandableListView.IOnGroupCollapseListener, 
                                ExpandableListView.IOnGroupExpandListener
    {
        List<XMLConfig.IssStruct> _structs;



        private DrawerLayout mDrawerLayout;

        //private RecyclerView mDrawerList;
        private ExpandableListView mDrawerList;
        //private ActionBarDrawerToggle mDrawerToggle;
        private MyActionBarDrawerToggle mDrawerToggle;
        private string mDrawerTitle;


        // updated with PARKING or REGISTRY directed startup screen during menu build
        private int gStartItemGroupPosition = 0;
        private int gStartItemChildPosition = 0;

        // updated with PARKING startup screen during menu build
        public string gParkingItemFragmentTag = string.Empty;
        private int gParkingItemGroupPosition = 0;
        private int gParkingtemChildPosition = 0;


        public int gTicketsActionBarIndex = 0;


        // TRUE to hide the sliding drawer menu - TODO - from regsitry?
        private bool gHideSlidingDrawer = true;

        // Single inst of the LocationUpdateListener for all application
        private LocationUpdateListener gLocationUpdateListener;


        // create a our collections of parent/child menu items
        List<TMenuItemParent> gMasterMenuParentItems = new List<TMenuItemParent>();
        List<TMenuItemChild> gMasterMenuChildItems = new List<TMenuItemChild>();

        // IssueAp XML layout client defined items
        List<TMenuItemParent> gClientLayoutParentItems = new List<TMenuItemParent>();
        List<TMenuItemChild> gClientLayoutChildItems = new List<TMenuItemChild>();

        // User Profile menu 
        List<TMenuItemParent> gMenuPopUpUserProfileParentItems = new List<TMenuItemParent>();
        List<TMenuItemChild> gMenuPopUpUserProfileChildItems = new List<TMenuItemChild>();

        // form selection menu 
        //List<TMenuItemParent> gMenuPopUpFormSelectParentItems = new List<TMenuItemParent>();
        //List<TMenuItemChild> gMenuPopUpFormSelectChildItems = new List<TMenuItemChild>();



        // set TRUE to enable favorites
        private bool gFavoriteItemsEnabled = false;

        // favorite items will be added to the very top of the menu for instant access
        List<TMenuItemParent> gFavoriteItems = new List<TMenuItemParent>();


            // TODO - this needs to be handled more elegantly
        private int gLastPrinterSelectedThroughDialogAsIndex = -1;
        private string gLastPrinterSelectedThroughDialogAsString = string.Empty;
        private List<string> gAvailPrinters = new List<string>();


        private static Java.IO.File _pendingPhotoNext = null;

       

        private int IntentRecv_requestCode;
        private Android.App.Result IntentRecv_resultCode;
        private Intent IntentRecv_data;

		//N5 BarCode H/W button
		private N5BarCodeButtonClickReceiver IntentRecv_N5BarCodeButton  = null;
        //N5 Printer Error listner
        private N5PrinterErrorReceiver IntentRecv_N5PrinterErrorReceiver = null;

        private MemoryCleaningUpTimer gMemCleaningUpTimer = null;

        private Intent gIntentActivityLogService = null;
        private Intent gIntentFileCleaaningUpService = null;

        #region FormExitManagment

        // fWaitingForWirelessSearches is set true at the end of record entry
        // and the user decides to wait for pending wireless searches to complete. Once set true, 
        // HandleCursorBlinkTimerEvent checks the status of pending searches, and displays a message
        // indicating wireless searches have completed.
        private bool fWaitingForWirelessSearches = false;


        private bool fPreventESC = false;                           // when TRUE, the form cannot be exited with menu or backing out - must be completed
        private string fPreventESC_Statement = string.Empty;        // to tell them WHY they aren't being allowed to exit this way


        public void SetPreventESC(bool iValue, string iReasonForPrevention)
        {
            fPreventESC = iValue;
            fPreventESC_Statement = iReasonForPrevention;
        }


        private bool fExternalPreventESC = false;
        private string fExternalPreventESC_Statement = string.Empty;

        // Get/Set property functions for GetExternalPreventESC() so we can support
        // configurable override of setting based on contents of OverridePreventEscape field.
        public bool GetExternalPreventESC()
        {
            // TODO - implement at fragment level


            // Don't return true for prevent escape if optional field "OverridePreventEscape" is set to "Y"
            //TTEdit *loEdit;
            //if ( (loEdit = (TTEdit *)(FindControlByName( "OverridePreventEscape", TEditNameStr ))) != 0)
            //{
            //    char loEditValue[20];
            //    loEdit->GetEditBuffer( loEditValue );
            //    if (strcmp( "Y", loEditValue) == 0)
            //        return false;
            //    else
            //        return fExternalPreventESC;
            //}
            //else
            {
                return fExternalPreventESC;
            }
        }

        public void SetExternalPreventESC(bool iValue, string iReasonForPrevention)
        {
            fExternalPreventESC = iValue;
            fExternalPreventESC_Statement = iReasonForPrevention;
        }

        public string GetPreventESC_Statement()
        {
            if (string.IsNullOrEmpty(fPreventESC_Statement) == true)
            {
                // if no specific reason was given, just use the generic one
                return "Form cannot be exited this way.";
            }
            else
            {
                return fPreventESC_Statement;
            }
        }

        public string GetExternalPreventESC_Statement()
        {
            if (GetExternalPreventESC() == true)
            {
                if (string.IsNullOrEmpty(fExternalPreventESC_Statement) == true)
                {
                    // if no specific reason was given, just use the generic one
                    return "Form cannot be exited this way.";
                }
                else
                {
                    return fExternalPreventESC_Statement;
                }
            }
            else
            {
                return string.Empty;
            }
        }



        public void SetPreventESCReset()
        {
            SetPreventESC(false, "");
            SetExternalPreventESC(false, "");
        }





        public bool OkToExitCurrentForm()
        {
            if (fPreventESC || GetExternalPreventESC())
            {
                //if (string.IsNullOrEmpty(fPreventESC_Statement) == true)
                //{
                //    // if no specific reason was given, just use the generic one
                //    oErrMsg = "Form cannot be exited this way.";
                //}
                return false;
            }


            // TODO - wireless search intergration


            //// now we can see if we have any pending wireless searches - and ask them 
            //// if they want to wait for the results
            //if (CleanUpPendingWirelessSearches(this, NULL, false) == -1)
            //{
            //    // they decided to wait...
            //    fWaitingForWirelessSearches = true;

            //    if (oErrMsg != null)
            //    {
            //        oErrMsg = cnPreventExitNoMessage;
            //    }
            //    return false;
            //}
            //else
            {
                // nothing to wait for, or they decided not to
                fWaitingForWirelessSearches = false;

                return true;
            }

        }


        private void DisplayPreventESC()
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Incomplete");
            builder.SetMessage( GetPreventESC_Statement() + "\r\n" + GetExternalPreventESC_Statement() );
            builder.SetPositiveButton("OK", delegate
            {
                // do nothing, just info
            });
            builder.Show();

        }

        #endregion



        // TODO - integrate this into issuestructlogic 
        public void ResetIssueSourceReferences(string iTargetFragmenTag)
        {
            FragmentRegistation oneFragmentRegistration = FindFragmentRegistration(iTargetFragmenTag);

            if (oneFragmentRegistration != null)
            {
                if (oneFragmentRegistration.aFragment != null)
                {
                    if (oneFragmentRegistration.aFragment is CommonFragment)
                    {
                        ((CommonFragment)oneFragmentRegistration.aFragment).fSourceIssueStructLogic = null;
                        ((CommonFragment)oneFragmentRegistration.aFragment).fSourceDataStruct = null;
                        ((CommonFragment)oneFragmentRegistration.aFragment).fSourceDataRawRow = null;
                    }
                }
            }
        }


        public override void OnBackPressed()
        {
            // don't Finish(), just minimize
            MoveTaskToBack(true);

            // and don't call base behaviour
            //base.OnBackPressed();
        }



        protected override void OnStop()
        {
            try
            {
                
                //Remove the drawer listener
                if (mDrawerLayout != null)
                {
                    mDrawerLayout.RemoveDrawerListener(mDrawerToggle);
                }                

                //Stop the location lestener
                if (gLocationUpdateListener != null)
                {
                    gLocationUpdateListener.Stop();
                }

                Log.Debug(GetType().FullName, "MainActivity - OnStop");
                System.Console.WriteLine("MainActivity - OnStop");

                //Start the mem cleaning up timer
                if (gMemCleaningUpTimer != null)
                {
                    //Start the sleep timer so we will restart on next wake up
                    gMemCleaningUpTimer.Start();
                }

                base.OnStop();
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.OnStop", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::OnStop Exception source {0}: {1}", ex.Source, ex.ToString());
            }

        }


        public override void OnLowMemory()
        {
            LoggingManager.LogApplicationError(null, "MainActvity.OnLowMemory", "Recieved");
            System.Console.WriteLine("MainActivity::OnLowMemory Recieved {0}: {1}", "", "" );

            MemoryWatcher.RaiseRestartRequestFlag();

            base.OnLowMemory();
        }


/// <summary>
        /// Returns appropriate title text for fragment menu type
        /// 
        /// TODO - get these from registry?
        /// </summary>
        /// <param name="iStructName"></param>
        /// <param name="iFragmentType"></param>
        /// <returns></returns>
        private string GetTitleTextForMenuFragment( string iStructName, FragmentClassificationType iFragmentType )
        {
            switch (iFragmentType)
            {
                case FragmentClassificationType.fctActionBarActivity:
                    {
                        return Helper.FormatTitleText(iStructName);
                        break;
                    }
                case FragmentClassificationType.fctIssueItemActivity:
                    {
                        return Helper.FormatTitleText(iStructName) + " Issue New";
                        break;
                    }
                case FragmentClassificationType.fctSelectItemActivty:
                    {
                        return Helper.FormatTitleText(iStructName) + " Look Up";
                        break;
                    }
                default:
                    {
                        return Helper.FormatTitleText(iStructName);
                        break;
                    }
            }
        }



        public Android.App.Fragment gFragmentLastShown = null;

        //private Fragment[] _fragments;
        public List<FragmentRegistation> _myFragmentColection = new List<FragmentRegistation>();

        public FragmentRegistation RegisterFragment(Android.App.Fragment iFragment, string iFragmentTag, string iStructName, string iMenuItemText, FragmentClassificationType iFragmentType, int iGroupPosition, int iChildPosition)
        {
            if (string.IsNullOrEmpty(iFragmentTag) == true)
            {
                throw new Exception("RegisterFragment call is missing tag value");
            }


            // first see if this one has been around already
            FragmentRegistation fr = FindFragmentRegistration(iFragmentTag);
            if (fr == null)
            {
                // create a new one
                fr = new FragmentRegistation();

                fr.aFragment = iFragment;

                //fr.aFragmentTag = iFragmentTag.ToUpper().Trim();
                fr.aFragmentTag = iFragmentTag;  // leave it exactly as passed
                fr.aFragType = iFragmentType;
                fr.structName = iStructName;

                if (string.IsNullOrEmpty(iMenuItemText) == true)
                {
                    // didn't get one, try to make one
                    fr.aTitleText = GetTitleTextForMenuFragment(iStructName, iFragmentType);
                }
                else
                {
                    // use what was passed
                    fr.aTitleText = iMenuItemText;
                }


                fr.GroupPosition = iGroupPosition;
                fr.ChildPosition = iChildPosition;

                // only need to add the new ones
                _myFragmentColection.Add(fr);
            }
            else
            {
                // jsut update the fragment reference (?)
                fr.aFragment = iFragment;
            }


            return fr;
        }

        public Android.App.Fragment FindRegisteredFragment(string iFragmentTag)
        {
            // force standardized formatting
            // leave as is! iFragmentTag = iFragmentTag.ToUpper().Trim();

            foreach (FragmentRegistation oneFragmentRegistration in _myFragmentColection)
            {
                if (oneFragmentRegistration.aFragmentTag.Equals(iFragmentTag) == true)
                {
                    return oneFragmentRegistration.aFragment;
                }
            }

            // not found
            return null;
        }

        public bool FindRegisteredFragmentMenuPositions(string iFragmentTag, out int ioGroupPosition, out int ioChildPosition)
        {
            ioGroupPosition = 0;
            ioChildPosition = 0;

            foreach (FragmentRegistation oneFragmentRegistration in _myFragmentColection)
            {
                if (oneFragmentRegistration.aFragmentTag.Equals(iFragmentTag) == true)
                {
                    ioGroupPosition = oneFragmentRegistration.GroupPosition;
                    ioChildPosition = oneFragmentRegistration.ChildPosition;
                    return true;
                }
            }

            // not found
            return false;

        }

        public void AdjustRegisteredFragmentForFavorites(string iFragmentTag, int iFavoritesCount)
        {
            // force standardized formatting
            // leave as is!  iFragmentTag = iFragmentTag.ToUpper().Trim();

            foreach (FragmentRegistation oneFragmentRegistration in _myFragmentColection)
            {
                if (oneFragmentRegistration.aFragmentTag.Equals(iFragmentTag) == true)
                {
                    oneFragmentRegistration.GroupPosition += iFavoritesCount;
                    break;
                }
            }
        }


        public FragmentRegistation FindFragmentRegistration(int iGroupPosition, int iChildPosition)
        {
            // look through and find the matching item by group and position
            foreach (FragmentRegistation oneFragmentRegistration in _myFragmentColection)
            {
                // is is this group?
                if (oneFragmentRegistration.GroupPosition.Equals(iGroupPosition) == true)
                {
                    // is it this child?
                    if (oneFragmentRegistration.ChildPosition.Equals(iChildPosition) == true)
                    {
                        return oneFragmentRegistration;
                    }

                    // OR are we looking to invoke a single parent item?
                    if (iChildPosition.Equals(-1) == true)
                    {
                        return oneFragmentRegistration;
                    }

                }
            }

            // not found
            return null;
        }


        public FragmentRegistation FindFragmentRegistration(string iFragmentTag)
        {
            // look through and find the matching item by tag
            foreach (FragmentRegistation oneFragmentRegistration in _myFragmentColection)
            {
                if (oneFragmentRegistration.aFragmentTag.Equals(iFragmentTag) == true)
                {
                    return oneFragmentRegistration;
                }
            }

            // not found
            return null;
        }


        public FragmentRegistation FindFragmentRegistrationByStructName(string iStructName)
        {
            // look through and find the matching item by structure
            foreach (FragmentRegistation oneFragmentRegistration in _myFragmentColection)
            {
                if (oneFragmentRegistration.structName.Equals(iStructName) == true)
                {
                    return oneFragmentRegistration;
                }
            }

            // not found
            return null;
        }


        /// <summary>
        /// Register the secondary fragment tags that can be created so they will be hidden after they are shown
        /// </summary>
        private void RegisterSecondaryFragments()
        {

            string[] cnSecondaryFragments = new string[]
            {
                //Constants.TICKETS_FRAGMENT_TAG,
                //Constants.ISSUE_REVIEW_DETAIL_FRAGMENT_TAG,
                //Constants.VOID_FRAGMENT_TAG,
                //Constants.NOTES_FRAGMENT_TAG,
                //Constants.NOTE_DETAIL_FRAGMENT_TAG,
                //Constants.ISSUE_REVIEW_SUMMARY_FRAGMENT_TAG
                Constants.CHALK_DETAIL_FRAGMENT_TAG
            };

            foreach (string oneSecondaryFragmentTag in cnSecondaryFragments)
            {
                int loGroupPosition = -1;
                int loChildPosition = -1;

                RegisterFragment(null, oneSecondaryFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, loGroupPosition, loChildPosition);
            }


        }



        public void HideAllRegisteredFragments()
        {

            var ft = this.FragmentManager.BeginTransaction();


            // initialize all views to be hidden
            foreach (FragmentRegistation oneFragmentRegistration in _myFragmentColection)
            {


                switch (oneFragmentRegistration.aFragType)
                {
                       
                    case FragmentClassificationType.fctActionBarActivity:
                    case FragmentClassificationType.fctIssueItemActivity:
                    case FragmentClassificationType.fctSelectItemActivty:
                        {

                            if (oneFragmentRegistration.aFragment.View != null)
                            {
                                oneFragmentRegistration.aFragment.View.Visibility = ViewStates.Gone;
                            }
                            break;
                        }

                    case FragmentClassificationType.fctApplicationSettingsActivity:
                        {
                            // these are dialogs, not dynamic XML fragments
                            // do nothing
                            break;
                        }

                    default:  // incl  case FragmentClassificationType.fctSecondaryActivity:
                        {
                            Android.App.Fragment secondaryFragment = FragmentManager.FindFragmentByTag(oneFragmentRegistration.aFragmentTag);
                            if (secondaryFragment != null)
                            {

                                ft.Hide(secondaryFragment);

                                //if (secondaryFragment.View != null)
                                //{
                                //    secondaryFragment.View.Visibility = ViewStates.Gone;
                                //}
                            }


                            break;
                        }
                }


            }

            ft.Commit();

        }

        //public void InitializeAllRegisteredIssueApFragments()
        //{
        //    // initialize all issue ap related fragments
        //    foreach (FragmentRegistation oneFragmentRegistration in _myFragmentColection)
        //    {
        //        if (oneFragmentRegistration.aFragment.View != null)
        //        {
        //            if (oneFragmentRegistration.aFragment is CommonFragment)
        //            {

        //                //DroidContext.ResetControlStatusByStructName(oneFragmentRegistration.structName);
        //                ((CommonFragment)oneFragmentRegistration.aFragment).StartTicketLayout();
        //            }
        //        }
        //    }

        //}


        private bool gOneClickFirstRunOnly = false;

        protected override void OnStart()
        {
            base.OnStart();

            try
            {
                if (gMemCleaningUpTimer != null)
                {                    
                    //Stop memory cleaning up timer until we go into sleep again
                    gMemCleaningUpTimer.Stop();
                }

                if (mDrawerLayout != null && mDrawerToggle != null)
                {
                    mDrawerLayout.SetDrawerListener(mDrawerToggle);
                }
                
                if (gLocationUpdateListener != null)
                {
                    // Start the location update listener 
                    gLocationUpdateListener.Start();
                }


                // only once on the very first load
                if (gOneClickFirstRunOnly == false)
                {
                    gOneClickFirstRunOnly = true;

                    RunOnUiThread(() =>
                    {
                        //InitializeAllRegisteredIssueApFragments();

                        // done in OnClick HideAllRegisteredFragments();

                        // activate the specified default item
                        OnChildClick(null, null, gStartItemGroupPosition, gStartItemChildPosition, 0);

                        // this is needed to init/hide all fragments

                    });
                }

            }
            catch (Exception exp)
            {
                //   WriteLine(exp.Message);

            }


        }



        IExpandableListAdapter mAdapter;
        const string Name = "NAME";
        const string IsEven = "IS_EVEN";



        public class TMenuItemParent
        {
            public string MenuItemText;
            public string ItemTag;
            public int GroupPosition;
            public int ChildCount;

        }

        public class TMenuItemChild
        {
            public string MenuItemText;
            public int IssueFormIdx = 1;
            public string ItemTag;
            public int GroupPosition;
            public int ChildPosition;
            public TMenuItemParent ParentMenu;
        }

        private void AddWebViewEnforcementMenuItems(List<TMenuItemParent> iParentItems, List<TMenuItemChild> iChildItems)
        {


            // add additional 3rd party enforcment tabs first, if they are defined
            TMenuItemParent loWebViewEnforcementParentMenu = null;
            if (
                (WirelessEnforcementOptions.fPayBySpaceMapEnforcementActive == true) ||
                (WirelessEnforcementOptions.fPayBySpaceListEnforcementActive == true) ||
                (WirelessEnforcementOptions.fPayByPlateMapEnforcementActive == true) ||
                (WirelessEnforcementOptions.fPayByPlateListEnforcementActive == true)
                )
            {
                // we'll have at least one web view - create a parent item and add it to collection
                loWebViewEnforcementParentMenu = new TMenuItemParent();
                loWebViewEnforcementParentMenu.MenuItemText = Constants.LABEL_DIRECTED_ENFORCEMENT_TAB;
                loWebViewEnforcementParentMenu.GroupPosition = iParentItems.Count;
                iParentItems.Add(loWebViewEnforcementParentMenu);
            }




            ///////
            if (WirelessEnforcementOptions.fPayBySpaceMapEnforcementActive == true)
            {

                // first to show on landing pages is the GIS map fragment
                Android.App.Fragment gisFrag = new WebViewGISMapFragment { Arguments = new Bundle() };

                //_fragments[loFragmentIdx] = gisFrag;
                //editor.PutString(Constants.LABEL_GIS_TAB + Constants.LABEL_TAB_POSITION, loFragmentIdx.ToString());
                //editor.Apply();

                TMenuItemChild loNewChildMenuItem = new TMenuItemChild();

                loNewChildMenuItem.MenuItemText = Constants.LABEL_GIS_TAB;
                loNewChildMenuItem.ParentMenu = loWebViewEnforcementParentMenu;
                loNewChildMenuItem.ItemTag = Constants.GIS_MAP_FRAGMENT_TAG;

                loNewChildMenuItem.GroupPosition = loWebViewEnforcementParentMenu.GroupPosition;
                loNewChildMenuItem.ChildPosition = loWebViewEnforcementParentMenu.ChildCount;


                iChildItems.Add(loNewChildMenuItem);
                loWebViewEnforcementParentMenu.ChildCount++;

                //AddTabToActionBar(Constants.LABEL_GIS_TAB, Resource.Drawable.ic_enforce_by_map_32, Constants.GIS_MAP_FRAGMENT_TAG, gisFrag);

                TMenuItemParent loNewFavoriteItem = null;
                if (gFavoriteItemsEnabled == true)
                {
                    // make GIS Maps a Favorite
                    loNewFavoriteItem = new TMenuItemParent();
                    loNewFavoriteItem.MenuItemText = loNewChildMenuItem.MenuItemText;
                    loNewFavoriteItem.ItemTag = loNewChildMenuItem.ItemTag;
                    loNewFavoriteItem.GroupPosition = gFavoriteItems.Count;
                    gFavoriteItems.Add(loNewFavoriteItem);
                }



                RegisterFragment(gisFrag, loNewChildMenuItem.ItemTag, "", "", FragmentClassificationType.fctActionBarActivity, loNewChildMenuItem.GroupPosition, loNewChildMenuItem.ChildPosition);

                // is this a favorite?
                if (loNewFavoriteItem != null)
                {
                    // register a second time using the favorite's menu info
                    RegisterFragment(gisFrag, loNewFavoriteItem.ItemTag, "", "", FragmentClassificationType.fctActionBarActivity, loNewFavoriteItem.GroupPosition, -1);
                }

                var ft = this.FragmentManager.BeginTransaction();
                ft.Add(Resource.Id.frameLayout1, gisFrag, loNewChildMenuItem.ItemTag);
                ft.Commit();

            }



            ////
            if (WirelessEnforcementOptions.fPayBySpaceListEnforcementActive == true)
            {
                // second is the GIS List fragment
                Android.App.Fragment gisListFrag = new WebViewPayBySpaceListFragment { Arguments = new Bundle() };

                //_fragments[loFragmentIdx] = gisListFrag;
                //editor.PutString(Constants.LABEL_GISLIST_TAB + Constants.LABEL_TAB_POSITION, loFragmentIdx.ToString());
                //editor.Apply();

                TMenuItemChild loNewChildMenuItem = new TMenuItemChild();

                loNewChildMenuItem.MenuItemText = Constants.LABEL_GISLIST_TAB;
                loNewChildMenuItem.ParentMenu = loWebViewEnforcementParentMenu;
                loNewChildMenuItem.ItemTag = Constants.GIS_PAYBYSPACE_LIST_FRAGMENT_TAG;

                loNewChildMenuItem.GroupPosition = loWebViewEnforcementParentMenu.GroupPosition;
                loNewChildMenuItem.ChildPosition = loWebViewEnforcementParentMenu.ChildCount;

                iChildItems.Add(loNewChildMenuItem);
                loWebViewEnforcementParentMenu.ChildCount++;


                //AddTabToActionBar(Constants.LABEL_GISLIST_TAB, Resource.Drawable.ic_enforce_by_list_32, Constants.GIS_PAYBYSPACE_LIST_FRAGMENT_TAG, gisListFrag);


                RegisterFragment(gisListFrag, loNewChildMenuItem.ItemTag, "", "", FragmentClassificationType.fctActionBarActivity, loNewChildMenuItem.GroupPosition, loNewChildMenuItem.ChildPosition);

                var ft = this.FragmentManager.BeginTransaction();
                ft.Add(Resource.Id.frameLayout1, gisListFrag, loNewChildMenuItem.ItemTag);
                ft.Commit();

            }


            ////
            if (WirelessEnforcementOptions.fPayByPlateListEnforcementActive == true)
            {

                // next is the PayByPlate list fragment
                Android.App.Fragment gisPayByPlateListFrag = new WebViewPayByPlateListFragment { Arguments = new Bundle() };
                //_fragments[loFragmentIdx] = gisListFrag;
                //editor.PutString(Constants.LABEL_PAYBYPLATELIST_TAB + Constants.LABEL_TAB_POSITION, loFragmentIdx.ToString());
                //editor.Apply();

                TMenuItemChild loNewChildMenuItem = new TMenuItemChild();

                loNewChildMenuItem.MenuItemText = Constants.LABEL_PAYBYPLATELIST_TAB;
                loNewChildMenuItem.ParentMenu = loWebViewEnforcementParentMenu;
                loNewChildMenuItem.ItemTag = Constants.GIS_PAYBYPLATE_LIST_FRAGMENT_TAG;

                loNewChildMenuItem.GroupPosition = loWebViewEnforcementParentMenu.GroupPosition;
                loNewChildMenuItem.ChildPosition = loWebViewEnforcementParentMenu.ChildCount;

                iChildItems.Add(loNewChildMenuItem);
                loWebViewEnforcementParentMenu.ChildCount++;

                //AddTabToActionBar(Constants.LABEL_PAYBYPLATELIST_TAB, Resource.Drawable.ic_enforce_by_list_32, Constants.GIS_PAYBYPLATE_LIST_FRAGMENT_TAG, gisPayByPlateListFrag);


                RegisterFragment(gisPayByPlateListFrag, loNewChildMenuItem.ItemTag, "", "", FragmentClassificationType.fctActionBarActivity, loNewChildMenuItem.GroupPosition, loNewChildMenuItem.ChildPosition);


                var ft = this.FragmentManager.BeginTransaction();
                ft.Add(Resource.Id.frameLayout1, gisPayByPlateListFrag, loNewChildMenuItem.ItemTag);
                ft.Commit();

            }


        }

        private void AddSystemEnforcementMenuItems(List<TMenuItemParent> iParentItems, List<TMenuItemChild> iChildItems)
        {
            // TODO - these fixed menus should be defined in an XML menu resouce



            TMenuItemParent loSettingsAndExitParentMenu;
            TMenuItemChild loNewChildMenuItem;


            //  settings parent menu group
            loSettingsAndExitParentMenu = new TMenuItemParent();
            loSettingsAndExitParentMenu.MenuItemText = Constants.LABEL_APPLICATION_SETTINGS_MENU_GROUP;
            loSettingsAndExitParentMenu.GroupPosition = iParentItems.Count;
            iParentItems.Add(loSettingsAndExitParentMenu);



            // TODO issuance info


            // TODO allocation sequences





            // printers
            loNewChildMenuItem = new TMenuItemChild();

            loNewChildMenuItem.MenuItemText = Constants.LABEL_APPLICATION_SETTINGS_PRINTERS_OPTION;
            loNewChildMenuItem.ParentMenu = loSettingsAndExitParentMenu;
            loNewChildMenuItem.ItemTag = Constants.LABEL_APPLICATION_SETTINGS_PRINTERS_OPTION;

            loNewChildMenuItem.GroupPosition = loSettingsAndExitParentMenu.GroupPosition;
            loNewChildMenuItem.ChildPosition = loSettingsAndExitParentMenu.ChildCount;

            RegisterFragment(null, loNewChildMenuItem.ItemTag, "", loNewChildMenuItem.MenuItemText, FragmentClassificationType.fctApplicationSettingsActivity, loNewChildMenuItem.GroupPosition, loNewChildMenuItem.ChildPosition);

            iChildItems.Add(loNewChildMenuItem);
            loSettingsAndExitParentMenu.ChildCount++;


            // bluetooth
            loNewChildMenuItem = new TMenuItemChild();

            loNewChildMenuItem.MenuItemText = Constants.LABEL_APPLICATION_SETTINGS_BLUETOOTH_OPTION;
            loNewChildMenuItem.ParentMenu = loSettingsAndExitParentMenu;
            loNewChildMenuItem.ItemTag = Constants.LABEL_APPLICATION_SETTINGS_BLUETOOTH_OPTION;

            loNewChildMenuItem.GroupPosition = loSettingsAndExitParentMenu.GroupPosition;
            loNewChildMenuItem.ChildPosition = loSettingsAndExitParentMenu.ChildCount;

            RegisterFragment(null, loNewChildMenuItem.ItemTag, "", loNewChildMenuItem.MenuItemText, FragmentClassificationType.fctApplicationSettingsActivity, loNewChildMenuItem.GroupPosition, loNewChildMenuItem.ChildPosition);

            iChildItems.Add(loNewChildMenuItem);
            loSettingsAndExitParentMenu.ChildCount++;



            // logout
            loNewChildMenuItem = new TMenuItemChild();

            loNewChildMenuItem.MenuItemText = Constants.LABEL_APPLICATION_SETTINGS_LOGOUT_OPTION;
            loNewChildMenuItem.ParentMenu = loSettingsAndExitParentMenu;
            loNewChildMenuItem.ItemTag = Constants.LABEL_APPLICATION_SETTINGS_LOGOUT_OPTION;

            loNewChildMenuItem.GroupPosition = loSettingsAndExitParentMenu.GroupPosition;
            loNewChildMenuItem.ChildPosition = loSettingsAndExitParentMenu.ChildCount;

            RegisterFragment(null, loNewChildMenuItem.ItemTag, "", loNewChildMenuItem.MenuItemText, FragmentClassificationType.fctApplicationSettingsActivity, loNewChildMenuItem.GroupPosition, loNewChildMenuItem.ChildPosition);

            iChildItems.Add(loNewChildMenuItem);
            loSettingsAndExitParentMenu.ChildCount++;

        }


        private void BuildLogicForStructure(TIssStruct CfgIssStruct)
        {
            // Get existing structure logic if it exits
            IssueStructLogicAndroid StructLogic = null;
            if ((CfgIssStruct.StructLogicObj != null) && (CfgIssStruct.StructLogicObj is IssueStructLogicAndroid))
            {
                StructLogic = CfgIssStruct.StructLogicObj as IssueStructLogicAndroid;
            }

            // Create new structure logic if it doesn't already exist
            if (StructLogic == null)
            {

                // Is it a Reissue structure?
                if (CfgIssStruct is TReissueStruct)
                {
                    // Create ReissueStructLogic object and set local base-class reference
                    ReissueStructLogicAndroid ReissueLogic = new ReissueStructLogicAndroid();
                    StructLogic = ReissueLogic;
                }

                // Is it a Void structure?
                else if (CfgIssStruct is TVoidStruct)
                {
                    // Create VoidStructLogic object and set local base-class reference
                    VoidStructLogicAndroid VoidLogic = new VoidStructLogicAndroid();
                    StructLogic = VoidLogic;
                }

                // Is it a Continuance structure?
                else if (CfgIssStruct is TContinuanceStruct)
                {
                    // Create ContinuanceStructLogic object and set local base-class reference
                    ContinuanceStructLogicAndroid ContinuanceLogic = new ContinuanceStructLogicAndroid();
                    StructLogic = ContinuanceLogic;
                }

                // Is it a general Cite Detail structure?
                else if (CfgIssStruct is TCiteDetailStruct)
                {
                    // Create DetailStructLogic object and set local base-class reference
                    DetailStructLogicAndroid DetailLogic = new DetailStructLogicAndroid();
                    StructLogic = DetailLogic;
                }

                // Is it a MarkMode structure?
                else if (CfgIssStruct is TMarkModeStruct)
                {
                    // Create MarkModeStructLogic object and set local base-class reference
                    MarkModeStructLogicAndroid MarkModeLogic = new MarkModeStructLogicAndroid();
                    StructLogic = MarkModeLogic;
                }

                // Is it a PublicContact structure?
                else if (CfgIssStruct is TPublicContactStruct)
                {
                    // Create PublicContactStructLogic object and set local base-class reference
                    PublicContactStructLogic PublicContactLogic = new PublicContactStructLogic();
                    StructLogic = PublicContactLogic;
                }

                // Is it a generic Search structure?
                else if (CfgIssStruct is TSearchStruct)
                {
                    // Create SearchStructLogic object and set local base-class reference
                    SearchStructLogicAndroid SearchLogic = new SearchStructLogicAndroid();
                    StructLogic = SearchLogic;
                    // HotSheet structures need to be set to "CopyDataFromResult"
                    if (CfgIssStruct is THotSheetStruct)
                        SearchLogic.fCopyDataFromResult = true;
                }

                // Is it a regular Citation structure?
                else
                {
                    // Create CiteStructLogic object and set local base-class reference
                    CiteStructLogicAndroid CiteLogic = new CiteStructLogicAndroid();
                    StructLogic = CiteLogic;
                }

                // Set cross references between configuration object and logic object
                StructLogic.IssueStruct = CfgIssStruct;
                CfgIssStruct.StructLogicObj = StructLogic;

                // Search structure has special needs after being constructed AND the IssueStruct is set
                if (CfgIssStruct is TSearchStruct)
                {
                    (StructLogic as SearchStructLogicAndroid).PostConstruction();
                }
            }

            // Create 1st ISSUE form logic (and ISSUE form) if necessary
            // Must set ActiveIssueFormIdx so correct issuance form is referenced by application in current context
            StructLogic.ActiveIssueFormIdx = 1;
            if (StructLogic != null)
            {
                if (StructLogic.HasNthIssueForm(1) == false)
                    StructLogic.BuildIssueForm1Logic();
            }

            // Create 2nd ISSUE form logic (and ISSUE form) if necessary 
            // (2nd ISSUE form is NOT common)
            // Must set ActiveIssueFormIdx so correct issuance form is referenced by application in current context
            StructLogic.ActiveIssueFormIdx = 2;
            if (StructLogic != null)
            {
                if (StructLogic.HasNthIssueForm(2) == false)
                    StructLogic.BuildIssueForm2Logic();
            }

            // Now we'll reset the ActiveIssueFormIdx so the primary issuance form is referenced by the application
            StructLogic.ActiveIssueFormIdx = 1;

            // Create SELECT form logic (and SELECT form) if necessary
            if (StructLogic != null)
            {
                if (StructLogic.SelectFormLogic == null)
                    StructLogic.BuildSelectFormLogic();
            }

            // Create IssueReviewDetail form logic  if necessary
            if (StructLogic != null)
            {
               StructLogic.BuildIssueReviewDetailLogic();
            }

        }



        private void PrepareAutoISSUEMenus( List<TMenuItemParent> iParentItems, List<TMenuItemChild> iChildItems )
        {


            //// add review tab at the very end
            //if (ticketsFragment == null)
            ////if (
            ////    (
            ////      Constants.STRUCT_TYPE_CITE.Equals(structI.Type) ||
            ////      Constants.STRUCT_TYPE_CHALKING.Equals(structI.Type) ||
            ////      Constants.STRUCT_TYPE_GENERIC_ISSUE.Equals(structI.Type)
            ////      )
            ////    && ticketsFragment == null)
            //{
            //    ticketsFragment = new TicketsFragment();

            //    editor.PutString(Constants.LABEL_TICKETS_TAB + Constants.LABEL_TAB_POSITION, loFragmentIdx.ToString());
            //    editor.Apply();

            //    AddTabToActionBar(Constants.LABEL_TICKETS_TAB, Resource.Drawable.ticket_image_multiple_rev_1, Constants.TICKETS_FRAGMENT_TAG, ticketsFragment);
            //    gTicketsActionBarIndex = loFragmentIdx;

            //    loFragmentIdx++;
            //}





            // make two passes through the client def - 1st Pass (Main Menus)
            foreach (TIssStruct NextIssStruct in DroidContext.XmlCfg.clientDef.IssStructMgr.IssStructs)
            {

                // First lets create the necessary logic/forms for the structure
                BuildLogicForStructure(NextIssStruct);



                foreach (TIssMenuItem NextCfgMenu in NextIssStruct.IssMenuItems)
                {
                    // Is it a main menu which we will turn into a new tab for the tab control?
                    if ((NextCfgMenu.Name == "MAIN") || (NextCfgMenu.ParentMenuName == ""))
                    {
                        // create a parent item and add it to collection
                        TMenuItemParent loNewParent = new TMenuItemParent();
                        loNewParent.MenuItemText = Helper.FormatTitleText(NextCfgMenu.MenuText);
                        loNewParent.GroupPosition = iParentItems.Count;
                        iParentItems.Add(loNewParent);
                    }
                }
            }

            // 2nd Pass (Child Menus)
            foreach (TIssStruct NextIssStruct in DroidContext.XmlCfg.clientDef.IssStructMgr.IssStructs)
            {
                foreach (TIssMenuItem NextCfgMenu in NextIssStruct.IssMenuItems)
                {

                    // track favorites
                    TMenuItemParent loNewFavoriteItem = null;

                    bool loIsSelectMenu = false;

                    // Is it not a main menu?
                    if ((NextCfgMenu.Name != "MAIN") && (NextCfgMenu.ParentMenuName != ""))
                    {
                        // Find the parent item
                        TMenuItemParent ParentMenuItem = null;
                        for (int loIdx = 0; loIdx < iParentItems.Count; loIdx++)
                        {
                            TMenuItemParent tabAndText = iParentItems[loIdx];
                            if (tabAndText != null)
                            {
                                if (string.Compare(tabAndText.MenuItemText, NextCfgMenu.ParentMenuName, true) == 0)
                                {
                                    ParentMenuItem = tabAndText;
                                    break;
                                }
                            }
                        }

                        // If we found the parent, create a new button for the action
                        if (ParentMenuItem != null)
                        {
                            TMenuItemChild loNewChildMenuItem = null;

                            // Prepare and attach an action to be performed when button is pressed
                            //SetMenuAction(BtnMenuAction, NextCfgMenu, NextIssStruct);
                            // Is this an ISSUE button?
                            if ((string.Compare(NextCfgMenu.Name, "ISSUE", true) == 0))
                            {
                                loNewChildMenuItem = new TMenuItemChild();
                                loNewChildMenuItem.MenuItemText = Helper.FormatTitleText(NextCfgMenu.MenuText);
                                loNewChildMenuItem.ParentMenu = ParentMenuItem;

                                //loNewChildMenuItem.ItemTag = NextIssStruct.Name;
                                loNewChildMenuItem.ItemTag = Helper.BuildIssueNewFragmentTag(NextIssStruct.Name);


                                loNewChildMenuItem.GroupPosition = ParentMenuItem.GroupPosition;
                                loNewChildMenuItem.ChildPosition = ParentMenuItem.ChildCount;

                                iChildItems.Add(loNewChildMenuItem);

                                ParentMenuItem.ChildCount++;


                                // is this the matching startup structure from the registry?
                                string loStartupStruct = "PARKING"; // TODO define in registry
                                if (NextIssStruct.Name.Equals(loStartupStruct) == true)
                                {
                                    // until we figure how to wait until parking form is ready, we go back to the maps
                                   //gStartItemGroupPosition = loNewChildMenuItem.GroupPosition;
                                   //gStartItemChildPosition = loNewChildMenuItem.ChildPosition;
                                }


                                string loParkingStruct = "PARKING"; // TODO define in registry
                                if (NextIssStruct.Name.Equals(loParkingStruct) == true)
                                {
                                    gParkingItemGroupPosition = loNewChildMenuItem.GroupPosition;
                                    gParkingtemChildPosition = loNewChildMenuItem.ChildPosition;
                                    gParkingItemFragmentTag = loNewChildMenuItem.ItemTag;
                                }


                                if (gFavoriteItemsEnabled == true)
                                {
                                    //  create a parent level click able item
                                    if (NextIssStruct.Name.Equals(loParkingStruct) == true)
                                    {
                                        // create an item and add it to collection
                                        loNewFavoriteItem = new TMenuItemParent();
                                        loNewFavoriteItem.MenuItemText = Helper.FormatTitleText(NextCfgMenu.MenuText);
                                        loNewFavoriteItem.ItemTag = NextIssStruct.Name;

                                        loNewFavoriteItem.GroupPosition = gFavoriteItems.Count;

                                        gFavoriteItems.Add(loNewFavoriteItem);
                                    }
                                }


                            }

                            // Is this a 2nd ISSUE button?
                            if ((string.Compare(NextCfgMenu.Name, "ISSUE2", true) == 0))
                            {
                                loNewChildMenuItem = new TMenuItemChild();
                                loNewChildMenuItem.MenuItemText = Helper.FormatTitleText(NextCfgMenu.MenuText);
                                loNewChildMenuItem.ParentMenu = ParentMenuItem;
                                
                                // update the index from default
                                loNewChildMenuItem.IssueFormIdx = 2;



                                //loNewChildMenuItem.ItemTag = NextIssStruct.Name + "2";
                                loNewChildMenuItem.ItemTag = Helper.BuildIssueNew2FragmentTag(NextIssStruct.Name);

                                loNewChildMenuItem.GroupPosition = ParentMenuItem.GroupPosition;
                                loNewChildMenuItem.ChildPosition = ParentMenuItem.ChildCount;

                                iChildItems.Add(loNewChildMenuItem);

                                ParentMenuItem.ChildCount++;
                            }

                            // Is this a SELECT/VIEW button?
                            if ((NextCfgMenu.Name.StartsWith("SELECT", true, System.Globalization.CultureInfo.CurrentCulture) == true))
                            //if ((string.Compare(NextCfgMenu.Name, "SELECT", true) == 0))
                                {
                                loNewChildMenuItem = new TMenuItemChild();
                                loNewChildMenuItem.MenuItemText = Helper.FormatTitleText(NextCfgMenu.MenuText);
                                loNewChildMenuItem.ParentMenu = ParentMenuItem;
                                loNewChildMenuItem.ItemTag = Helper.BuildIssueSelectFragmentTag(NextIssStruct.Name);

                                loNewChildMenuItem.GroupPosition = ParentMenuItem.GroupPosition;
                                loNewChildMenuItem.ChildPosition = ParentMenuItem.ChildCount;

                                iChildItems.Add(loNewChildMenuItem);

                                ParentMenuItem.ChildCount++;

                                loIsSelectMenu = true;
                            }
                           


                            // did we get something defined
                            if (loNewChildMenuItem == null)
                            {
                                // no usable item, move along
                                continue;
                            }






                            string structName = NextIssStruct.Name;



                            //// AJW - DEMO KLUDGE - hide markmode until the chalking is cleaned up
                            //if (Constants.STRUCT_TYPE_CHALKING.Equals(structType) == true)
                            //{
                            //    continue;
                            //}


                            //// AJW - DEMO KLUDGE - hide activity until cleaned up
                            //if (structName.ToUpper().Equals(Constants.STRUCT_NAME_ACTIVITYLOG) == true)
                            //{
                            //    continue;
                            //}

                            //// AJW - DEMO KLUDGE - hide meter status until it cleaned up
                            //if (structName.ToUpper().Equals("METERSTATUS") == true)
                            //{
                            //    continue;
                            //}

                            //// AJW - DEMO KLUDGE - hide damaged sign until it cleaned up  - the ticket end displays placeholder bitmap
                            //if (structName.ToUpper().Equals("DAMAGEDSIGN") == true)
                            //{
                            //    continue;
                            //}

                            // AJW - DEMO KLUDGE - hide activity log until it cleaned up  - the ticket end displays placeholder bitmap
                            //if (structName.ToUpper().Equals("ACTIVITYLOG") == true)
                            //{
                            //    continue;
                            //}




                            // icon id -1 = no icon drawn
                            int iResourceIconIdToUse = -1;
                            if (NextIssStruct is TActivityLogStruct)
                            {

                            }
                            else if (NextIssStruct is TCiteStruct)
                            {
                                iResourceIconIdToUse = Resource.Drawable.ticket_image_single_32_rev_2;

                                // AJW - DEMO KLUGE we need to take these from the menu names
                                if (structName.ToUpper().Equals("METERSTATUS") == true)
                                {
                                    //structMenuTabText = "METER STATUS";
                                    iResourceIconIdToUse = Resource.Drawable.parking_meter_24px;
                                }
                            }
                            else if (NextIssStruct is TMarkModeStruct)
                            {
                            }
                            else if (NextIssStruct is TPublicContactStruct)
                            {
                            }
                            else if (NextIssStruct is TGenericIssueStruct)
                            {
                               iResourceIconIdToUse = Resource.Drawable.ticket_image_single_32_rev_2;
                            }
                            else if (NextIssStruct is THotSheetStruct)
                            {
                            }


                            // is this a SELECT item?
                            if (loIsSelectMenu == true)
                            {
                                 // create a selection fragment
                                Android.App.Fragment fragment = new IssueSelectFragment { Arguments = new Bundle() };
                                fragment.Arguments.PutString("structName", structName);
                                fragment.Arguments.PutString("menuItem", loNewChildMenuItem.MenuItemText);

                                RegisterFragment(fragment, loNewChildMenuItem.ItemTag, structName, loNewChildMenuItem.MenuItemText, FragmentClassificationType.fctSelectItemActivty, loNewChildMenuItem.GroupPosition, loNewChildMenuItem.ChildPosition);


                                var ft = this.FragmentManager.BeginTransaction();
                                ft.Add(Resource.Id.frameLayout1, fragment, loNewChildMenuItem.ItemTag);
                                ft.Commit();

                            }
                            else
                            {

                                // is this a an issuance struct?
                                if (
                                    (NextIssStruct is TActivityLogStruct) ||
                                    (NextIssStruct is TCiteStruct) ||
                                    (NextIssStruct is TMarkModeStruct) ||
                                    (NextIssStruct is TGenericIssueStruct) ||
                                    (NextIssStruct is THotSheetStruct)
                                    )
                                {

                                    Android.App.Fragment fragment = new CommonFragment { Arguments = new Bundle() };
                                    fragment.Arguments.PutString("structName", structName);
                                    fragment.Arguments.PutInt("issueFormNdx", loNewChildMenuItem.IssueFormIdx);
                                    fragment.Arguments.PutString("menuItem", loNewChildMenuItem.MenuItemText);

                                    //AddTabToActionBar(structMenuTabText, iResourceIconIdToUse, structI.Name, fragment);


                                    RegisterFragment(fragment, loNewChildMenuItem.ItemTag, structName, loNewChildMenuItem.MenuItemText, FragmentClassificationType.fctIssueItemActivity, loNewChildMenuItem.GroupPosition, loNewChildMenuItem.ChildPosition);

                                    // is this a favorite?
                                    if (loNewFavoriteItem != null)
                                    {
                                        // register a second time using the favorite's menu info
                                        RegisterFragment(fragment, loNewFavoriteItem.ItemTag, structName, loNewChildMenuItem.MenuItemText, FragmentClassificationType.fctIssueItemActivity, loNewFavoriteItem.GroupPosition, -1);
                                    }



#if _oooo_
                                    // assume that there will be notes for each - TODO - let XML dictacte which get created

                                    // this one gets creates from menu... TODO just cue off XML for accuracy
                                    string loIssueSelectFragmentTag = Helper.BuildIssueSelectFragmentTag(structName);
                                    // this is already a main menu option, if we register again it will get shown/hidden multiple times which will cause display sync issues
                                    //RegisterFragment(null, loIssueSelectFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

                                    string loIssueReviewFragmentTag = Helper.BuildIssueReviewFragmentTag(structName);
                                    RegisterFragment(null, loIssueReviewFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

                                    string loIssueNotesFragmentTag = Helper.BuildIssueNotesFragmentTag(structName);
                                    RegisterFragment(null, loIssueNotesFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

                                    string loIssueNoteDetailFragmentTag = Helper.BuildIssueNoteDetailFragmentTag(structName);
                                    RegisterFragment(null, loIssueNoteDetailFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

                                    string loIssueVoidFragmentTag = Helper.BuildIssueVoidFragmentTag(structName);
                                    RegisterFragment(null, loIssueVoidFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);
#endif
                                    // this one left over from before?
                                    var prevInstance = this.FragmentManager.FindFragmentByTag(loNewChildMenuItem.ItemTag);
                                    if (prevInstance != null)
                                    {
                                        Console.WriteLine("");
                                    }


                                    var ft = this.FragmentManager.BeginTransaction();
                                    ft.Add(Resource.Id.frameLayout1, fragment, loNewChildMenuItem.ItemTag);
                                    ft.Commit();

                                    //// RunOnUiThread(() =>
                                    //// {
                                    //     // initialize the first time through
                                    //     DroidContext.ResetControlStatusByStructName(structName);
                                    //     ((CommonFragment)fragment).StartTicketLayout();
                                    //// });


                                }
                                else
                                {
                                    // unknown structure type, nothing to do fo rnow
                                }
                            }








                            ////

                        }
                    }
                }
            }

            ///////


            // now cycle through and register fragments created for those forms that aren't directly referenced on the menu system
            foreach (TIssStruct NextIssStruct in DroidContext.XmlCfg.clientDef.IssStructMgr.IssStructs)
            {
                FragmentRegistation loFragmentRegistration;

                // Get existing structure logic if it exists
                IssueStructLogicAndroid loIssueStructLogic = null;
                if ((NextIssStruct.StructLogicObj != null) && (NextIssStruct.StructLogicObj is IssueStructLogicAndroid))
                {
                    loIssueStructLogic = NextIssStruct.StructLogicObj as IssueStructLogicAndroid;
                }

                // create/register/update associated secondary select/issuance forms as needed
                if (loIssueStructLogic != null)
                {

                    // is one defined?
                    if (string.IsNullOrEmpty(loIssueStructLogic.IssueForm1LogicFragmentTag) == false)
                    {
                        // is it already registered?
                        loFragmentRegistration = FindFragmentRegistration(loIssueStructLogic.IssueForm1LogicFragmentTag);
                        if (loFragmentRegistration == null)
                        {
                            // register it - as secondary activity (not on a direct menu)
                            loFragmentRegistration = RegisterFragment(loIssueStructLogic.IssueForm1Logic, loIssueStructLogic.IssueForm1LogicFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

                            var ft = this.FragmentManager.BeginTransaction();
                            ft.Add(Resource.Id.frameLayout1, loIssueStructLogic.IssueForm1Logic, loIssueStructLogic.IssueForm1LogicFragmentTag);
                            ft.Commit();
                        }

                        // update the associated logic
                        loIssueStructLogic.IssueForm1Logic = loFragmentRegistration.aFragment;

                    }


                    // is one defined?
                    if (string.IsNullOrEmpty(loIssueStructLogic.IssueForm2LogicFragmentTag) == false)
                    {
                        // is it already registered?
                        loFragmentRegistration = FindFragmentRegistration(loIssueStructLogic.IssueForm2LogicFragmentTag);
                        if (loFragmentRegistration == null)
                        {
                            // register it - as secondary activity (not on a direct menu)
                            loFragmentRegistration = RegisterFragment(loIssueStructLogic.IssueForm2Logic, loIssueStructLogic.IssueForm2LogicFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

                            var ft = this.FragmentManager.BeginTransaction();
                            ft.Add(Resource.Id.frameLayout1, loIssueStructLogic.IssueForm2Logic, loIssueStructLogic.IssueForm2LogicFragmentTag);
                            ft.Commit();
                        }

                        // update the associated logic
                        loIssueStructLogic.IssueForm2Logic = loFragmentRegistration.aFragment;

                    }


                    // is one  defined?
                    if (string.IsNullOrEmpty(loIssueStructLogic.SelectFormLogicFragmentTag) == false)
                    {
                        // is it already registered?
                        loFragmentRegistration = FindFragmentRegistration(loIssueStructLogic.SelectFormLogicFragmentTag);
                        if (loFragmentRegistration == null)
                        {
                            // register it - as secondary activity (not on a direct menu)
                            loFragmentRegistration = RegisterFragment(loIssueStructLogic.SelectFormLogic, loIssueStructLogic.SelectFormLogicFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

                            var ft = this.FragmentManager.BeginTransaction();
                            ft.Add(Resource.Id.frameLayout1, loIssueStructLogic.SelectFormLogic, loIssueStructLogic.SelectFormLogicFragmentTag);
                            ft.Commit();
                        }

                        // update the associated logic
                        loIssueStructLogic.SelectFormLogic = loFragmentRegistration.aFragment;

                    }

                    // is one defined?
                    if (string.IsNullOrEmpty(loIssueStructLogic.IssueReviewDetailLogicFragmentTag) == false)
                    {
                        // is it already registered?
                        loFragmentRegistration = FindFragmentRegistration(loIssueStructLogic.IssueReviewDetailLogicFragmentTag);
                        if (loFragmentRegistration == null)
                        {
                            // register it - as secondary activity (not on a direct menu)
                            loFragmentRegistration = RegisterFragment(loIssueStructLogic.IssueReviewDetailLogic, loIssueStructLogic.IssueReviewDetailLogicFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

                            var ft = this.FragmentManager.BeginTransaction();
                            ft.Add(Resource.Id.frameLayout1, loIssueStructLogic.IssueReviewDetailLogic, loIssueStructLogic.IssueReviewDetailLogicFragmentTag);
                            ft.Commit();
                        }

                        // update the associated logic
                        loIssueStructLogic.IssueReviewDetailLogic = loFragmentRegistration.aFragment;
                    }


                    // is one defined?
                    if (string.IsNullOrEmpty(loIssueStructLogic.NotesFormLogicFragmentTag) == false)
                    {
                        // is it already registered?
                        loFragmentRegistration = FindFragmentRegistration(loIssueStructLogic.NotesFormLogicFragmentTag);
                        if (loFragmentRegistration == null)
                        {
                            // register it - as secondary activity (not on a direct menu)
                            loFragmentRegistration = RegisterFragment(loIssueStructLogic.NotesFormLogic, loIssueStructLogic.NotesFormLogicFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

                            var ft = this.FragmentManager.BeginTransaction();
                            ft.Add(Resource.Id.frameLayout1, loIssueStructLogic.NotesFormLogic, loIssueStructLogic.NotesFormLogicFragmentTag);
                            ft.Commit();
                        }

                        // update the associated logic
                        loIssueStructLogic.NotesFormLogic = loFragmentRegistration.aFragment;
                    }


                    // is one defined?
                    if (string.IsNullOrEmpty(loIssueStructLogic.NotesDetailFormLogicFragmentTag) == false)
                    {
                        // is it already registered?
                        loFragmentRegistration = FindFragmentRegistration(loIssueStructLogic.NotesDetailFormLogicFragmentTag);
                        if (loFragmentRegistration == null)
                        {
                            // register it - as secondary activity (not on a direct menu)
                            loFragmentRegistration = RegisterFragment(loIssueStructLogic.NotesDetailFormLogic, loIssueStructLogic.NotesDetailFormLogicFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

                            var ft = this.FragmentManager.BeginTransaction();
                            ft.Add(Resource.Id.frameLayout1, loIssueStructLogic.NotesDetailFormLogic, loIssueStructLogic.NotesDetailFormLogicFragmentTag);
                            ft.Commit();
                        }

                        // update the associated logic
                        loIssueStructLogic.NotesDetailFormLogic = loFragmentRegistration.aFragment;
                    }



#if _ooo_
                // Is it a Reissue structure?
                if (CfgIssStruct is TReissueStruct)
                {



                // this one gets creates from menu... TODO just cue off XML for accuracy
                string loIssueSelectFragmentTag = Helper.BuildIssueSelectFragmentTag(structName);
                // this is already a main menu option, if we register again it will get shown/hidden multiple times which will cause display sync issues
                //RegisterFragment(null, loIssueSelectFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

                string loIssueReviewFragmentTag = Helper.BuildIssueReviewFragmentTag(structName);
                RegisterFragment(null, loIssueReviewFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

                string loIssueNotesFragmentTag = Helper.BuildIssueNotesFragmentTag(structName);
                RegisterFragment(null, loIssueNotesFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

                string loIssueNoteDetailFragmentTag = Helper.BuildIssueNoteDetailFragmentTag(structName);
                RegisterFragment(null, loIssueNoteDetailFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);

                string loIssueVoidFragmentTag = Helper.BuildIssueVoidFragmentTag(structName);
                RegisterFragment(null, loIssueVoidFragmentTag, "", "", FragmentClassificationType.fctSecondaryActivity, -1, -1);


                var ft = this.FragmentManager.BeginTransaction();
                ft.Add(Resource.Id.frameLayout1, fragment, loNewChildMenuItem.ItemTag);
                ft.Commit();
#endif

                }
            }
            ////////




            ///////////////////////////


            // adjust group position indexes to account for favorites being inserted at the top
            int loFavoritesOffset = gFavoriteItems.Count;

            if (loFavoritesOffset > 0)
            {
                foreach (TMenuItemChild oneChild in iChildItems)
                {
                    oneChild.GroupPosition += loFavoritesOffset;
                    AdjustRegisteredFragmentForFavorites(oneChild.ItemTag, loFavoritesOffset);
                }

                foreach (TMenuItemParent oneParent in iParentItems)
                {
                    oneParent.GroupPosition += loFavoritesOffset;
                }


                gStartItemGroupPosition += loFavoritesOffset;
                gParkingItemGroupPosition += loFavoritesOffset;


                // now insert the favorites at the top
                iParentItems.InsertRange(0, gFavoriteItems);
            }
            
            /////////////////////////////




            // add secondard fragment types
            RegisterSecondaryFragments();



       }


        private void BuildSlidingDrawerMenu( List<TMenuItemParent> iParentItems, List<TMenuItemChild> iChildItems )
        {

            using (var groupData = new JavaList<IDictionary<string, object>>())
            using (var childData = new JavaList<IList<IDictionary<string, object>>>())
            {
                for (int i = 0; i < iParentItems.Count; i++)  
                {
                    TMenuItemParent loParentItem = iParentItems[i];

                    using (var curGroupMap = new JavaDictionary<string, object>())
                    {

                        groupData.Add(curGroupMap);
                        curGroupMap.Add(Name, Helper.FormatTitleText(loParentItem.MenuItemText));
                        //curGroupMap.Add(IsEven, (i % 2 == 0) ? "This group is even" : "This group is odd");


                        using (var children = new JavaList<IDictionary<string, object>>())
                        {
                            for (int j = 0; j < iChildItems.Count; j++)  
                            {
                                TMenuItemChild loChildItem = iChildItems[j];

                                // is this our parent?
                                if (loChildItem.ParentMenu == loParentItem)
                                {
                                    // add to our parent
                                    using (var curChildMap = new JavaDictionary<string, object>())
                                    {
                                        children.Add(curChildMap);
                                        curChildMap.Add(Name, "   " + Helper.FormatTitleText(loChildItem.MenuItemText));
                                        //curChildMap.Add(IsEven, (j % 2 == 0) ? "   This child is even" : "   This child is odd");
                                    }
                                }
                            }
                            childData.Add(children);
                        }
                    }
                }
                // Set up our adapter
                mAdapter = new SimpleExpandableListAdapter(
                        this,
                        groupData,
                        Android.Resource.Layout.SimpleExpandableListItem1,
                        new string[] { Name, IsEven },
                        new int[] { Android.Resource.Id.Text1, Android.Resource.Id.Text2, Android.Resource.Drawable.IcLockIdleAlarm },
                        childData,
                        Android.Resource.Layout.SimpleExpandableListItem2,
                        new string[] { Name, IsEven },
                        new int[] { Android.Resource.Id.Text1, Android.Resource.Id.Text2, Android.Resource.Drawable.IcMediaPlay }
                        );
            }


        }


        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);


                // Init our global error handling class
                ErrorHandling.InitErrorHandling(this);

                AndroidEnvironment.UnhandledExceptionRaiser -= HandleAndroidException; // actually needed to prevent multiple callbacks?
                AndroidEnvironment.UnhandledExceptionRaiser += HandleAndroidException;

                // done 1x at boot only
                //// Init our global DroidContext class
                //DroidContext.ApplicationContext = this.ApplicationContext;
                DroidContext.mainActivity = this;


                SetContentView(Resource.Layout.Main);


                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // DEVELOPER NOTES: dont make any calls that reference XML or REGISTRY settings BEFORE the config is loaded /////
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

                // refresh new every time we start the main activity 
                // if we don't do this, in a logout/login sequence we can be left with a 
                // mixture of old and new objects.
                Duncan.AI.Droid.DroidContext.XmlCfgLoadFromInternalFilesystem();
                
                // reference the XML config 
                Constants.gClientName = DroidContext.XmlCfg.clientDef.Client;


                //Init our single LocationUpdateListenr now so it will be ready for all dependant services
                gLocationUpdateListener = new LocationUpdateListener(this);
                gLocationUpdateListener.Start();



                if (gHideSlidingDrawer == true)
                {
                    // do nothing

                }
                else
                {

                    mDrawerTitle = this.Title;
                }

                mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                mDrawerList = FindViewById<ExpandableListView>(Resource.Id.left_drawer);


                // set a custom shadow that overlays the main content when the drawer opens
                if (mDrawerLayout != null)
                {
                    mDrawerLayout.SetDrawerShadow(Resource.Drawable.drawer_shadow, GravityCompat.Start);
                }

                // improve performance by indicating the list if fixed size.
                //mDrawerList.HasFixedSize = true;
                //mDrawerList.SetLayoutManager(new LinearLayoutManager(this));


                MemoryWatcher.InitMemoryWatcher();


                //// here to add BEFORE configuration based items
                AddWebViewEnforcementMenuItems(gMasterMenuParentItems, gMasterMenuChildItems);

                // get current positions
                TMenuItemParent loLastParentPreLayoutMenuItems = null;
                if (gMasterMenuParentItems.Count > 0)
                {
                    loLastParentPreLayoutMenuItems = gMasterMenuParentItems[gMasterMenuParentItems.Count - 1];
                }

                TMenuItemChild loLastChildPreLayouMenuItems = null;
                if (gMasterMenuChildItems.Count > 0)
                {
                    loLastChildPreLayouMenuItems = gMasterMenuChildItems[gMasterMenuChildItems.Count - 1];
                }

                // extract the XML layout based menu items defined for this client
                PrepareAutoISSUEMenus(gMasterMenuParentItems, gMasterMenuChildItems);

                // copy the XML items for use in the popup selection menu
                int loParentIdx = 0;
                if (loLastParentPreLayoutMenuItems != null)
                {
                    loParentIdx = gMasterMenuParentItems.IndexOf(loLastParentPreLayoutMenuItems);
                }
                // taking care for empty lead in items
                if ( loParentIdx > -1 )
                {
                    for (int loParentCopyIdx = loParentIdx + 1; loParentCopyIdx < gMasterMenuParentItems.Count; loParentCopyIdx++)
                    {
                        gClientLayoutParentItems.Add( gMasterMenuParentItems[ loParentCopyIdx ] );
                    }
                }


                int loChildIdx = 0;
                if (loLastChildPreLayouMenuItems != null)
                {
                    loChildIdx = gMasterMenuChildItems.IndexOf(loLastChildPreLayouMenuItems);
                }

                if (loChildIdx > -1)
                {
                    for (int loChildCopyIdx = loChildIdx + 1; loChildCopyIdx < gMasterMenuChildItems.Count; loChildCopyIdx++)
                    {
                        gClientLayoutChildItems.Add(gMasterMenuChildItems[loChildCopyIdx]);
                    }
                }


                // put the application options AFTER configuration based items
                AddSystemEnforcementMenuItems(gMasterMenuParentItems, gMasterMenuChildItems);


                // initialize the menu adapter
                BuildSlidingDrawerMenu(gMasterMenuParentItems, gMasterMenuChildItems);





                mDrawerList.SetAdapter(mAdapter);

                mDrawerList.SetOnChildClickListener(this);
                mDrawerList.SetOnGroupClickListener(this);
                mDrawerList.SetOnGroupCollapseListener(this);
                mDrawerList.SetOnGroupExpandListener(this);

                // until this is fleshed out a little more
                //mDrawerList.SetGroupIndicator( Resources.GetDrawable( Resource.Drawable.actionbar_drawer_selector_style));
                //mDrawerList.SetGroupIndicator(null);


                if (gHideSlidingDrawer == true)
                {
                    // now turn off the old sliding drawer....
                    this.ActionBar.SetHomeButtonEnabled(false);
                    this.ActionBar.SetDisplayHomeAsUpEnabled(false);
                    this.ActionBar.SetDisplayShowHomeEnabled(false);

                    // customize our action bar
                    LayoutInflater loInflater = LayoutInflater.From(this);
                    View mCustomActionBarView = loInflater.Inflate(Resource.Layout.action_bar, null);
                    ActionBar.LayoutParams ab_lp = new ActionBar.LayoutParams(ActionBar.LayoutParams.WrapContent, ActionBar.LayoutParams.WrapContent, GravityFlags.CenterVertical);
                    this.ActionBar.SetCustomView(mCustomActionBarView, ab_lp);
                    this.ActionBar.SetDisplayShowCustomEnabled(true);

                    // define the click events
                    ImageButton btnUserAccount = mCustomActionBarView.FindViewById<ImageButton>(Resource.Id.btnAccountMenuItem);
                    if (btnUserAccount != null)
                    {
                        btnUserAccount.Click += btnUserAccount_Click;
                    }

                    ImageButton btnMaps = mCustomActionBarView.FindViewById<ImageButton>(Resource.Id.btnMapMenuItem);
                    if (btnMaps != null)
                    {
                        btnMaps.Click += btnMaps_Click;
                    }

                    ImageButton btnCamera = mCustomActionBarView.FindViewById<ImageButton>(Resource.Id.btnCameraMenuItem);
                    if ( btnCamera != null )
                    {
                        btnCamera.Click += btnCamera_Click;
                    }

                }
                else
                {
                    // enable ActionBar app icon to behave as action to toggle nav drawer
                    this.ActionBar.SetDisplayHomeAsUpEnabled(true);
                    this.ActionBar.SetHomeButtonEnabled(true);
                    this.ActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_black_48dp);
                }





                // ActionBarDrawerToggle ties together the the proper interactions
                // between the sliding drawer and the action bar app icon

                mDrawerToggle = new MyActionBarDrawerToggle(this, mDrawerLayout,
                    Resource.Drawable.ic_drawer,
                    Resource.String.drawer_open,
                    Resource.String.drawer_close);

                mDrawerLayout.SetDrawerListener(mDrawerToggle);


                // Defer code dependent on restoration of previous instance state.
                mDrawerLayout.Post(new RunnableAnonymousInnerClassHelper(mDrawerToggle));


                PrepareUserAccountMenus();
                PepareFormSelectionMenus();

                //Set the default selected printer to 2T-N5Class if we are running on N5Printer or N5Scan and also launch PM app.
                if (Helper.IsCurrentDeviceIsN5Class())
                {
                    PrinterSupport_TwoTechN5Printer.SetTwoTechN5PrinterAsDefaultPrinter((Context)(this));
                }

                //Init services
                gIntentActivityLogService = new Intent(this, typeof(GPSService));
                gIntentFileCleaaningUpService = new Intent(this, typeof(FileCleaningUpService));

                //We done with inits, let us init the restart timer
                //This timer will start when we go in sleep and stop when we resume from sleep. 
                if (gMemCleaningUpTimer == null)
                {
                    //Get the timer Interval from Registry
                    int loTimerInterval = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                      TTRegistry.regAPPLICATION_CLEAR_CACHE_INTERVAL_MIN,
                                                                                      TTRegistry.regAPPLICATION_CLEAR_CACHE_INTERVAL_MIN_DEFAULT);
                    if(loTimerInterval > 0)
                    {
                        gMemCleaningUpTimer = new MemoryCleaningUpTimer(loTimerInterval);
                    }
                }
                
            }

            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.OnCreate", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::OnCreate Exception source {0}: {1}", ex.Source, ex.ToString());
            }

            try
            {

                // TODO - need a better mechanism to know when to read
                // force a read every time, we may have skipped past login already if (string.IsNullOrEmpty(Constants.PRINTER_TYPE) == true)
                {
                    ConfigurationLocalOptionsHelper.ReadConfigData();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("MainActivity::OnCreate Exception source {0}: {1}", ex.Source, ex.ToString());
            }



#if _integrate_n5_support_
/* Ayman: This code is borken when _integrate_n5_support_ is defined. The plan to move all N5 LIB related code to the combine app
            try
            {

                if (DroidContext.gPrinterSelected == PrintersSupported.Printer_TwoTechnologiesN5Class )
                {

                    DroidContext.N5Interface = new PrinterSupport_TwoTechN5Printer(DroidContext.ApplicationContext);
                    // wait until later... ?
                    DroidContext.N5Interface.InitalizeN5PlatformInterface( false );

                }
            }
            catch (Exception exp)
            {
              Console.WriteLine("Exception source: {0}", exp.Source);
            }
 */ 
#endif

            //Send Broadcast sync data and GPS tracking
            Intent i = new Intent("Duncan.AI.Droid.AlarmBroadcastRec");
            SendBroadcast(i);
            StartGPSTracking();
            StartFileCleaningUpService();

#if !_integrate_n5_support_
            //Init barcode scanner
            MobileBarcodeScanner.Initialize(Application);
#endif

            //ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            ActionBar.NavigationMode = ActionBarNavigationMode.Standard;

            _structs = DroidContext.XmlCfg.IssStructs;


            if (gHideSlidingDrawer == true)
            {
                // new school
                ActionBar.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.actionbar_style_web));
            }
            else
            {
                // old school
                // set the Action bar background color
                //ActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor(Constants.ACTIONBAR_BACKGROUND_COLOR)));
                ActionBar.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.actionbar_style));
                //Resources.GetDrawable(Resource.Drawable.button_secondary)
            }




            //To change the tab bar color under the actionbar:
            //ActionBar.SetStackedBackgroundDrawable(new ColorDrawable(getResources().getColor(R.color.color_brown_dark)));
            //ActionBar.SetStackedBackgroundDrawable(new ColorDrawable(GetResources().GetColor(R.color.color_brown_dark)));

                //ActionBar.SetStackedBackgroundDrawable(new ColorDrawable(Color.ParseColor(Constants.ACTIONBAR_STACKEDITEM_BACKGROUND_COLOR)));

                ActionBar.SetStackedBackgroundDrawable(new ColorDrawable((Resources.GetColor(Resource.Color.civicsmart_white))));

            //To change tab bar background:
            //ActionBar.SetStackedBackgroundDrawable(getResources().getDrawable(R.drawable.coupon_header));
            //ActionBar.SetStackedBackgroundDrawable(getResources().getDrawable(R.drawable.coupon_header));
            //ActionBar.SetStackedBackgroundDrawable(new ColorDrawable(Color.ParseColor(Constants.ACTIONBAR_BACKGROUND_COLOR)));

            //ActionBar.SetSelectableItemBackground


            // if no internally tracked focus, try to grab the global one
            Helper.HideKeyboardFromActivity(this);
        }



        /// <summary>
        /// A public version than can be called for system restart
        /// </summary>
        public void CleanUpResourcesForShutdown()
        {
            //Stop all listeners
            // clean up our defined receivers 
            try
            {

                if (IntentRecv_N5BarCodeButton != null)
                {
                    UnregisterReceiver(IntentRecv_N5BarCodeButton);
                    IntentRecv_N5BarCodeButton.Dispose();
                    IntentRecv_N5BarCodeButton = null;
                }

                if (IntentRecv_N5PrinterErrorReceiver != null)
                {
                    UnregisterReceiver(IntentRecv_N5PrinterErrorReceiver);
                    IntentRecv_N5PrinterErrorReceiver.Dispose();
                    IntentRecv_N5PrinterErrorReceiver = null;
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.CleanUpResourcesForShutdown", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::CleanUpResourcesForShutdown Exception source {0}: {1}", ex.Source, ex.ToString());
            }

            // clean up
            try
            {
                if (gIntentActivityLogService != null)
                {
                    StopGPSTracking();
                    gIntentActivityLogService.Dispose();
                    gIntentActivityLogService = null;
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.CleanUpResourcesForShutdown", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::CleanUpResourcesForShutdown Exception source {0}: {1}", ex.Source, ex.ToString());
            }


            try
            {
                if (gIntentFileCleaaningUpService != null)
                {
                    StopFileCleaningUpService();
                    gIntentFileCleaaningUpService.Dispose();
                    gIntentFileCleaaningUpService = null;
                }
            }

            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.CleanUpResourcesForShutdown", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::CleanUpResourcesForShutdown Exception source {0}: {1}", ex.Source, ex.ToString());
            }


            try
            {
                if (gLocationUpdateListener != null)
                {
                    gLocationUpdateListener.Stop();
                    gLocationUpdateListener.Dispose();
                    gLocationUpdateListener = null;
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.CleanUpResourcesForShutdown", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::CleanUpResourcesForShutdown Exception source {0}: {1}", ex.Source, ex.ToString());
            }


            try
            {
                if (gMemCleaningUpTimer != null)
                {
                    gMemCleaningUpTimer.Stop();
                    gMemCleaningUpTimer.Dispose();
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.CleanUpResourcesForShutdown", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::CleanUpResourcesForShutdown Exception source {0}: {1}", ex.Source, ex.ToString());
            }

        }


        protected override void OnDestroy()
        {
            Log.Debug("*********************************", "MainActivity.OnDestroy");

            // clean up
            CleanUpResourcesForShutdown();

            AndroidEnvironment.UnhandledExceptionRaiser -= HandleAndroidException; // actually needed to prevent multiple callbacks?

            base.OnDestroy();
        }


        void btnCamera_Click(object sender, EventArgs e)
        {
            MenuPopUp_LaunchCamera();

            //try
            //{
            //    Android.App.FragmentManager loFm = this.FragmentManager;
            //    CameraModeSelectionDialog loModeSelectionDlg = new CameraModeSelectionDialog(this);
            //    loModeSelectionDlg.Show(loFm, "Camera Selection Mode");
            //}
            //catch (Exception ex)
            //{
            //    LoggingManager.LogApplicationError(ex, "MainActvity.btnCamera_Click", ex.TargetSite.Name);
            //    System.Console.WriteLine("MainActivity::btnCamera_Click Exception source {0}: {1}", ex.Source, ex.ToString());
            //}



            //LaunchCameraForPendingAttachments();


        }


        void btnMaps_Click(object sender, EventArgs e)
        {
            try
            {
                // find it  our inventory
                FragmentRegistation loSelectedFragment = FindFragmentRegistration(Constants.GIS_MAP_FRAGMENT_TAG);
                // uh oh
                if (loSelectedFragment != null)
                {
                    OnDrawerMenuItemSelected(null, null, loSelectedFragment.GroupPosition, loSelectedFragment.ChildPosition, -1);
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.btnMaps_Click", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::btnMaps_Click Exception source {0}: {1}", ex.Source, ex.ToString());
            }

        }


        void btnUserAccount_Click(object sender, EventArgs e)
        {

            try
            {
                MenuPopUp_UserAccount();
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.btnUserAccount_Click", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::btnUserAccount_Click Exception source {0}: {1}", ex.Source, ex.ToString());
            }

        }
        
        
        
        private void HandleAndroidException(object sender, RaiseThrowableEventArgs e)
        {
            System.Console.WriteLine("MainActivity::Global Exception source {0}: {1}", e.Exception.TargetSite.Name, e.Exception.Source);
            LoggingManager.LogApplicationError(e.Exception, "Global Exception", e.Exception.TargetSite.Name);
            ErrorHandling.ReportException(e.Exception.Message);
        }

        
        //N5 listner for the BarCode H/W button (KEY_B)
        [BroadcastReceiver]
        [IntentFilter(new String[] { Constants.TWOTECH_N5PRINTER_PM_BARCODEBUTTON_CLICKED_LISTNER })]
        public class N5BarCodeButtonClickReceiver : BroadcastReceiver
        {
            private static MainActivity fMainActivity = null;
            private static bool fScanningIsInProgress = false;
            public N5BarCodeButtonClickReceiver()
            {
            }

            public N5BarCodeButtonClickReceiver(MainActivity iMainActivity)
            {
                fMainActivity = iMainActivity;
            }

            public async override void OnReceive(Context context, Intent intent)
            {
#if _enable_ZXing_
                // if we have a valid destination, we can redirect
                if (fMainActivity == null) return;
                if (fScanningIsInProgress) return;
                fScanningIsInProgress = true;
                if (fMainActivity.gFragmentLastShown != null)
                {
                    //if (fMainActivity.gFragmentLastShown.IsVisible == true)
                    {
                        if (fMainActivity.gFragmentLastShown is CommonFragment)
                        {
                            var loCommonFragment = (CommonFragment)fMainActivity.gFragmentLastShown;
                            if (loCommonFragment._formPanel != null)
                            {
                                //Make sure that the keyboard is hidden before we start the camera                                
                                loCommonFragment._formPanel.HideKeyBoardFromCurrentView();

                                var loScanner = new ZXing.Mobile.MobileBarcodeScanner();
                                loScanner.BottomText = "Touch screen to re-focus";
                                var loResult = await loScanner.Scan();
                                if (loResult == null)
                                {
                                    fScanningIsInProgress = false;
                                    return;
                                }
                                loCommonFragment._formPanel.ParseScannedBarCode(loResult.Text);
                                fScanningIsInProgress = false;
                            }
                        }
                    }
                }
#endif
            }

        }


        //N5 listner for the Printer errors
        [BroadcastReceiver]
        [IntentFilter(new String[] { Constants.TWOTECH_N5PRINTER_PM_ERRORS_LISTNER })]
        public class N5PrinterErrorReceiver : BroadcastReceiver
        {
            private static MainActivity fMainActivity = null;
            private static bool fErrorIsInProgress = false;
            public N5PrinterErrorReceiver()
            {
            }

            public N5PrinterErrorReceiver(MainActivity iMainActivity)
            {
                fMainActivity = iMainActivity;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                // if we have a valid destination, we can redirect
                if (fMainActivity == null) return;
                if (fErrorIsInProgress) return;
                fErrorIsInProgress = true;
                int loErrorCode = intent.GetIntExtra(Constants.TWOTECH_N5PRINTER_PM_ERRORS_ERRORCODE, (int)ErrorHandling.ErrorCode.Error_Unknown);                
                if (loErrorCode < (int)ErrorHandling.ErrorCode.Error_Unknown)
                {
                    ErrorHandling.ThrowError(fMainActivity, (ErrorHandling.ErrorCode)loErrorCode, "");
                    fErrorIsInProgress = false;
                }
                intent.Dispose();
            }
        }

        private class RunnableAnonymousInnerClassHelper : Java.Lang.Object, Java.Lang.IRunnable
        {
            private MyActionBarDrawerToggle nDrawerToggle;

            public RunnableAnonymousInnerClassHelper(MyActionBarDrawerToggle iDrawerToggle)
            {
                this.nDrawerToggle = iDrawerToggle;
                //Console.WriteLine("*** Runnable created");
            }

            public void Run()
            {
                //Console.WriteLine("*** Runnable starting");     // <-- does not show on Output panel
                nDrawerToggle.SyncState();
            }
        }


        internal class MyActionBarDrawerToggle : ActionBarDrawerToggle
        {
            MainActivity owner;

            public MyActionBarDrawerToggle(MainActivity activity, DrawerLayout layout, int imgRes, int openRes, int closeRes)
                : base(activity, layout, imgRes, openRes, closeRes)
            {
                owner = activity;
            }


            private void SetOwnerActionBarTitleForDrawer(string iDrawerTitle)
            {
                // defense - this is sometimes getting invoked even when hidden....
                if (iDrawerTitle != null)
                {
                    if (owner.ActionBar != null)
                    {
                        SpannableString text = new SpannableString(iDrawerTitle);
                        text.SetSpan(new ForegroundColorSpan(Color.Blue), 0, text.Length(), SpanTypes.InclusiveInclusive);
                        owner.ActionBar.Title = text.ToString();
                    }
                }
            }



            public override void OnDrawerClosed(View drawerView)
            {
                //owner.ActionBar.Title = owner.Title;

                //string titleText = "<font color=\"red\">" + owner.Title + "</font>";
                //owner.ActionBar.SetTitle(Title = Html.FromHtml(titleText);


                //
                //owner.ActionBar.SetText((Html.FromHtml(titleText));

                //owner.ActionBar.SetTitle((Html.fromHtml("<font color=\"red\">" + owner.Title + "</font>"));


                SetOwnerActionBarTitleForDrawer(owner.Title);

                owner.InvalidateOptionsMenu();
            }

            public override void OnDrawerOpened(View drawerView)
            {
                //owner.ActionBar.Title = owner.mDrawerTitle;

                SetOwnerActionBarTitleForDrawer(owner.mDrawerTitle);




                owner.InvalidateOptionsMenu();
            }

            //public override bool OnOptionsItemSelected(IMenuItem item)
            //{
            //    return owner.OnOptionsItemSelected(item);
            //}
        }


        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                
                // clean up our defined receivers 
                if (IntentRecv_N5BarCodeButton != null)
                {
                    UnregisterReceiver(IntentRecv_N5BarCodeButton);
                }

                if (IntentRecv_N5PrinterErrorReceiver != null)
                {
                    UnregisterReceiver(IntentRecv_N5PrinterErrorReceiver);
                }

                if (gLocationUpdateListener != null)
                {
                    // Stop the location update listener 
                    gLocationUpdateListener.Stop();
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.OnPause", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::OnPause Exception source {0}: {1}", ex.Source, ex.ToString());
            }

        }

        //OnResume life cycle method
        protected override void OnResume()
        {
            try
            {
                // Always call the superclass first.
                base.OnResume();

                if (gLocationUpdateListener != null)
                {
                    // Start the location update listener 
                    gLocationUpdateListener.Start();
                }

                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                ISharedPreferencesEditor editor = prefs.Edit();
                string username = prefs.GetString(Constants.username, null);

                //Setup our N5 Barcode H/W button receiver
                if (IntentRecv_N5BarCodeButton == null)
                {
                    IntentRecv_N5BarCodeButton = new N5BarCodeButtonClickReceiver(this);
                }
                RegisterReceiver(IntentRecv_N5BarCodeButton, new IntentFilter("com.civicsmart.BarCodeButtonClicked"));


                //Setup our N5 PrinterError receiver
                if (IntentRecv_N5PrinterErrorReceiver == null)
                {
                    IntentRecv_N5PrinterErrorReceiver = new N5PrinterErrorReceiver(this);
                }
                RegisterReceiver(IntentRecv_N5PrinterErrorReceiver, new IntentFilter("com.civicsmart.N5PrinterErrors"));


                if (username == null)
                {
                    StartActivity(typeof(LoginActivity));
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.OnResume", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::OnResume Exception source {0}: {1}", ex.Source, ex.ToString());
            }
        }








        //Add actions  to action bar
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (gHideSlidingDrawer == false)
            {
                MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
            }
            return base.OnPrepareOptionsMenu(menu);
        }

        /* Called whenever we call invalidateOptionsMenu() */
        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            // If the nav drawer is open, hide action items related to the content view
            //bool drawerOpen = mDrawerLayout.IsDrawerOpen(mDrawerList);
            //menu.FindItem(Resource.Id.action_websearch).SetVisible(!drawerOpen);
            return base.OnPrepareOptionsMenu(menu);
        }




        //Handle actions on action bar
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // The action bar home/up action should open or close the drawer.
            // ActionBarDrawerToggle will take care of this.
            if (mDrawerToggle.OnOptionsItemSelected(item))
            {
                return true;
            }

            // Handle action buttons
            switch (item.ItemId)
            {
                case Resource.Id.LogoutMenuItem:
                    {
                        Logout();
                        return true;
                    }

                case Resource.Id.ReportMenuItem:
                    {
                        ThreadPool.QueueUserWorkItem(o => LogCollector.WriteLogData(this, this.ApplicationContext));
                        return true;
                    }


                //case Resource.Id.SelectPrinterMenuItem:
                //    {
                //        SelectPrinter();
                //        return true;
                //    }

                //case Resource.Id.BlueToothMenuItem:
                //    {
                //        LaunchBlueToothAdmin();
                //        return true;
                //    }


                case Resource.Id.AccountMenuItem:
                    {
                        MenuPopUp_UserAccount();
                        return true;
                    }


                    // we will only show this menu item when there is no wireless connectivity
                case Resource.Id.ExitToAppMenuItem:
                    {
                        MenuPopUp_IssueFormSelection();
                        return true;
                    }

                case Resource.Id.MapMenuItem:
                    {

                        // find it  our inventory
                        FragmentRegistation loSelectedFragment = FindFragmentRegistration( Constants.GIS_MAP_FRAGMENT_TAG );
                        // uh oh
                        if (loSelectedFragment == null)
                        {
                            return false;
                        }

                        OnDrawerMenuItemSelected( null, null, loSelectedFragment.GroupPosition, loSelectedFragment.ChildPosition, -1 );


                        return true;
                    }

                case Resource.Id.CameraMenuItem:
                    {                        
                        //Make sure that the parking form is started
                        /*
                        if (gFragmentLastShown == null || !(gFragmentLastShown is CommonFragment))
                        {
                            // if we got here, we couldn't launch - display some kind of message?
                            var builder = new AlertDialog.Builder(this);
                            builder.SetTitle("Camera");
                            builder.SetMessage("Please select a form before taking picture.");
                            builder.SetPositiveButton("OK", delegate
                            {
                                // do nothing, just info
                            });
                            builder.Show();
                            return false;
                        }
                        */


                        //Android.App.FragmentManager loFm = this.FragmentManager;
                        //CameraModeSelectionDialog loModeSelectionDlg = new CameraModeSelectionDialog(this);
                        //loModeSelectionDlg.Show(loFm, "Camera Selection Mode");


                        MenuPopUp_LaunchCamera();
                        return true;
                    }


                //case Resource.Id.BarcodeScanMenuItem:
                //    {

                //        // if we have a valid destination, we can redirect
                //        if (gFragmentLastShown != null)
                //        {
                //            //if (gFragmentLastShown.IsVisible == true)
                //            {
                //                if (gFragmentLastShown is CommonFragment)
                //                {
                //                    var commonFragment = (CommonFragment)gFragmentLastShown;
                //                    if (commonFragment._formPanel != null)
                //                    {
                //                        commonFragment._formPanel.btnScanClick(null, null);
                //                        return true;
                //                    }
                //                }
                //            }
                //        }


                //        // if we got here, we couldn't launch - display some kind of message?
                //        var builder = new AlertDialog.Builder(this);
                //        builder.SetTitle("Barcode Scanner");
                //        builder.SetMessage("Please select a form before scanning.");
                //        builder.SetPositiveButton("OK", delegate
                //        {
                //            // do nothing, just info
                //        });
                //        builder.Show();

                //        return true;
                //    }







                //case Resource.Id.EnforceByListMenuItem:
                //    {
                //        ChangeToTargetTab(Constants.LABEL_GISLIST_TAB);
                //        return true;
                //    }

                //case Resource.Id.EnforceByMapMenuItem:
                //    {
                //        ChangeToTargetTab(Constants.LABEL_GIS_TAB);
                //        return true;
                //    }


                default:
                    {
                        return base.OnOptionsItemSelected(item);
                    }
            }
        }



        private void PepareFormSelectionMenus()
        {

        }

        private void PrepareUserAccountMenus()
        {
            
            // look to define some static stuff

            List<TMenuItemParent> iParentItems = gMenuPopUpUserProfileParentItems;
            List<TMenuItemChild> iChildItems = gMenuPopUpUserProfileChildItems;
                
            TMenuItemParent loUserAccountActivityParentMenu = null;
            TMenuItemChild loNewChildMenuItem;

                    


                // found 1, loop and find all defined up to #9
            for (int loIndex = 1; loIndex < 10; loIndex++)
            {

                string loIdxStr = loIndex.ToString();

                // client definable web views - look for them  by index
                string loClientDefinableWebView = TTRegistry.glRegistry.GetRegistryValue(
                                                            TTRegistry.regSECTION_ISSUE_AP,
                                                            TTRegistry.regWEBVIEW_MENU_ITEM_N_PREFIX + loIdxStr +
                                                            TTRegistry.regWEBVIEW_MENU_ITEM_N_ENABLED_SUFFIX, TTRegistry.regWEBVIEW_MENU_ITEM_N_ENABLED_DEFAULT);


                if (loClientDefinableWebView.Equals(TTRegistry.regOPTION_ENABLED) == true)
                {
                    // at least one is defined
                    WirelessEnforcementOptions.fWebViewClientDefinableEnabled = true;

                    // parent group defined yet?
                    if (loUserAccountActivityParentMenu == null)
                    {
                        // defined on demand - settings parent menu group
                        loUserAccountActivityParentMenu = new TMenuItemParent();
                        loUserAccountActivityParentMenu.MenuItemText = Constants.LABEL_MY_ACTIVITY_MENU_GROUP;
                        loUserAccountActivityParentMenu.GroupPosition = iParentItems.Count;
                        iParentItems.Add(loUserAccountActivityParentMenu);
                    }



                    // client definable 
                    Android.App.Fragment webViewClientDefinable = new WebViewClientDefinableFragment { Arguments = new Bundle() };
                    webViewClientDefinable.Arguments.PutInt(Constants.WebView_Client_Definable_Index_Parameter_Name, loIndex);

                    loNewChildMenuItem = new TMenuItemChild();


                    string loMenuText = TTRegistry.glRegistry.GetRegistryValue(
                                                            TTRegistry.regSECTION_ISSUE_AP,
                                                            TTRegistry.regWEBVIEW_MENU_ITEM_N_PREFIX + loIdxStr +
                                                            TTRegistry.regWEBVIEW_MENU_ITEM_N_TEXT_SUFFIX, 
                                                            TTRegistry.regWEBVIEW_MENU_ITEM_N_TEXT_DEFAULT + loIdxStr);


                    loNewChildMenuItem.MenuItemText = loMenuText;
                    loNewChildMenuItem.ParentMenu = loUserAccountActivityParentMenu;
                    loNewChildMenuItem.ItemTag = Helper.BuildWebViewClientDefinableTag(loIndex);


                    loNewChildMenuItem.GroupPosition = loUserAccountActivityParentMenu.GroupPosition;
                    loNewChildMenuItem.ChildPosition = loUserAccountActivityParentMenu.ChildCount;

                    iChildItems.Add(loNewChildMenuItem);


                    RegisterFragment(webViewClientDefinable, loNewChildMenuItem.ItemTag, "", loNewChildMenuItem.MenuItemText, FragmentClassificationType.fctActionBarActivity, loNewChildMenuItem.GroupPosition, loNewChildMenuItem.ChildPosition);


                    var ft = this.FragmentManager.BeginTransaction();
                    ft.Add(Resource.Id.frameLayout1, webViewClientDefinable, loNewChildMenuItem.ItemTag);
                    ft.Commit();
                }
            }



            // now add the standard application options 
            AddSystemEnforcementMenuItems(gMenuPopUpUserProfileParentItems, gMenuPopUpUserProfileChildItems);

        }


        public void MenuPopUp_LaunchCamera()
        {
            try
            {
                Android.App.FragmentManager loFm = this.FragmentManager;
                CameraModeSelectionDialog loModeSelectionDlg = new CameraModeSelectionDialog(this);
                loModeSelectionDlg.Show(loFm, "Camera Selection Mode");
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.MenuPopUp_LaunchCamera", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::MenuPopUp_LaunchCamera Exception source {0}: {1}", ex.Source, ex.ToString());
            }

            //LaunchCameraForPendingAttachments();
        }


        public void MenuPopUp_UserAccount()
        {


            //FragmentManager fm = ParentFragment.Activity.FragmentManager;
            //FragmentTransaction fragmentTransaction = fm.BeginTransaction();

            var ft = this.FragmentManager.BeginTransaction();

            string loTargetFragmentTag = "menu_popup";

            // always new dialog created
            MenuPopUpDialogFragment oneMenuPopUpDialogFragment = new MenuPopUpDialogFragment { Arguments = new Bundle() };

            //oneMenuPopUpDialogFragment.SetMenuItems( Constants.cnMenuPopup_UserAccountTitleText, gParentItems, gChildItems );
            oneMenuPopUpDialogFragment.SetMenuItems(Constants.cnMenuPopup_UserAccountTitleText, gMenuPopUpUserProfileParentItems, gMenuPopUpUserProfileChildItems);


            oneMenuPopUpDialogFragment.SetCallbackActivity(this);
            //oneMenuPopUpDialogFragment.SetTargetFragment(this, Constants.ACTIVITY_REQUEST_CODE_MENU_POPUP_NAVIGATION_RESULT);

            oneMenuPopUpDialogFragment.Show(this.FragmentManager, loTargetFragmentTag);

            //fragmentTransaction.Commit();
            ft.Commit();

            // we need to see this dialog immediately
            //fm.ExecutePendingTransactions();
            this.FragmentManager.ExecutePendingTransactions();
        }

        public void MenuPopUp_IssueFormSelection()
        {


            //FragmentManager fm = ParentFragment.Activity.FragmentManager;
            //FragmentTransaction fragmentTransaction = fm.BeginTransaction();

            var ft = this.FragmentManager.BeginTransaction();

            string loTargetFragmentTag = "menu_popup_issue";

            // always new dialog created
            MenuPopUpDialogFragment oneMenuPopUpDialogFragment = new MenuPopUpDialogFragment { Arguments = new Bundle() };

            oneMenuPopUpDialogFragment.SetMenuItems(Constants.cnMenuPopup_IssueFormSelectTitleText, gClientLayoutParentItems, gClientLayoutChildItems);


            oneMenuPopUpDialogFragment.SetCallbackActivity(this);
            //oneMenuPopUpDialogFragment.SetTargetFragment(this, Constants.ACTIVITY_REQUEST_CODE_MENU_POPUP_NAVIGATION_RESULT);

            oneMenuPopUpDialogFragment.Show(this.FragmentManager, loTargetFragmentTag);

            //fragmentTransaction.Commit();
            ft.Commit();

            // we need to see this dialog immediately
            //fm.ExecutePendingTransactions();
            this.FragmentManager.ExecutePendingTransactions();
        }


        public void MenuPopUp_ItemSelectCallback(string iSelectedFragment)
        {

            // ANY TIME we use the pop-up menu to change forms, we must make sure we clear old modes.
            // direct luanch from web view will initialize this
            ExternalEnforcementInterfaces.ClearGISMeterJsonValueObject();

            // TODO - more centralized control - 
            // needs to be called from here because they can jump in and out of forms without finishing,
            // where this is usually cleaned up
            ResetIssueSourceReferences(iSelectedFragment);

            ChangeToTargetFragmentTag(iSelectedFragment);


            //FragmentRegistation loSelectedFragment = FindFragmentRegistration(iSelectedFragment);

            //if (loSelectedFragment != null)
            //{
            //    OnDrawerMenuItemSelected(null, null, loSelectedFragment.GroupPosition, loSelectedFragment.ChildPosition, -1);
            //}

        }



        public bool ShowFragmentForIssueanceFromAnotherIssueForm(FragmentRegistation iTargetFragment )
        {
            string loDebugFragName = "Untiltled";

            try
            {
                loDebugFragName = iTargetFragment.aFragmentTag;


                // ANY TIME we use change forms, we must make sure we clear old modes.
                // direct luanch from web view will initialize this
                ExternalEnforcementInterfaces.ClearGISMeterJsonValueObject();  // OK here because this is not called from map

                // TODO - more centralized control - 
                // needs to be called from here because they can jump in and out of forms without finishing,
                // where this is usually cleaned up
                // NOT HERE - this is where it is passed ResetIssueSourceReferences(iTargetFragment.aFragmentTag);


                // reset each main menu item launch
                // not for one-offs DroidContext.MyFragManager.ClearInternalBackstack();



                // clean up any straglers - can be left if they jump out of nested screens with drawer menu
                HideAllRegisteredFragments();


                // hide the keyboard to start for each tab change
                Helper.HideKeyboard(CurrentFocus);


                // update the main content by hiding/showing fragments
                Android.App.Fragment fragment = iTargetFragment.aFragment;
                Android.App.Fragment old_fragment = gFragmentLastShown;


                // once they catch up they are good, but not ready very first time
                if (old_fragment != null)
                {
                    if (old_fragment.View != null)
                    {
                        // avoid piling on - may hav already been updated HideAllFragments
                        if (old_fragment.View.Visibility != ViewStates.Gone)
                        {
                            old_fragment.View.Visibility = ViewStates.Gone;
                        }
                    }
                }

                if (fragment != null)
                {
                    if (fragment.View != null)
                    {
                        if (fragment.View.Visibility != ViewStates.Visible)
                        {
                            fragment.View.Visibility = ViewStates.Visible;
                        }
                    }
                }


                gFragmentLastShown = fragment;

                if (gHideSlidingDrawer == true)
                {
                    // do nothing

                }
                else
                {
                    //Title = loSelectedFragment.aFragmentTag;
                    Title = iTargetFragment.aTitleText;
                }



                if (fragment is CommonFragment)
                {
                    RunOnUiThread(() =>
                    {
                        if (IntentRecv_requestCode != 0)
                        {
                            int lo_requestCode = IntentRecv_requestCode;
                            Android.App.Result lo_resultCode = IntentRecv_resultCode;
                            Intent lo_data = IntentRecv_data;

                            var commonFragment = (CommonFragment)fragment;
                            commonFragment._formPanel.OnActivityResultForCommonFragment(lo_requestCode, lo_resultCode, lo_data);

                            IntentRecv_requestCode = 0;
                        }
                        else
                        {
                            //DroidContext.ResetControlStatusByStructName(formNameToUse);
                            // here comes the new ticket
                            ((CommonFragment)fragment).StartTicketLayout();
                        }


                    });
                }
                else if (fragment is IssueSelectFragment)
                {
                    RunOnUiThread(() =>
                    {
                        // show me the records created so far
                        ((IssueSelectFragment)fragment).DisplayMatchingRecordsForDefaultStruct();
                    });


                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "ShowFragmentForIssueance", "OnTabSelected = " + loDebugFragName);
                //Toast.MakeText(_context, "Error loading ticket data.", ToastLength.Long).Show();
            }


            return true;


        }


        public bool OnDrawerMenuItemSelected(ExpandableListView parent, View v, int groupPosition, int childPosition, long id)
        {

            string loDebugFragName = "Untiltled";
            try
            {



                // find in our inventory
                FragmentRegistation loSelectedFragment = FindFragmentRegistration(groupPosition, childPosition);
                // uh oh
                if (loSelectedFragment == null)
                {
                    return false;
                }

                loDebugFragName = loSelectedFragment.aFragmentTag;

            }
            catch
            {
            }

            return SetActiveFragmentByTag(loDebugFragName);
        }


        public bool SetActiveFragmentByTag(string iNewFragmentTag)
        {


            string loDebugFragName = "Untiltled";
            try
            {

                // find in our inventory
                FragmentRegistation loSelectedFragment = FindFragmentRegistration(iNewFragmentTag);
                // uh oh
                if (loSelectedFragment == null)
                {
                    return false;
                }

                loDebugFragName = loSelectedFragment.aFragmentTag;


                // are they re-selecting the same item?
                if (loSelectedFragment.aFragment == gFragmentLastShown)
                {
                    // no need to reselect the same form
                    // this will only work when ALL fragments report to the "last shown" return false;

                    // no good to do, sometimes same fragment re-show when returning from Intent service call
                }




                // FIRST we have to process the application settings items, these have precendence over the XML based IssStructs
                // this first group of items are not form switching, they can be invoked on top of the issue form
                switch (loSelectedFragment.aFragmentTag)
                {

                    case Constants.LABEL_APPLICATION_SETTINGS_PRINTERS_OPTION:
                        {
                            // OK to fix the printer even if preventESC is acti
                            SelectPrinter();
                            return true;
                        }

                    case Constants.LABEL_APPLICATION_SETTINGS_BLUETOOTH_OPTION:
                        {
                            LaunchBlueToothAdmin();
                            return true;
                        }

                    default:
                        {
                            // fall through to check for prevent ESC before proceeding
                            break;
                        }
                }



                // the choices below would switch pages - first, see if its OK to start something else
                // TODO - can we put this on draweropen?
                if (OkToExitCurrentForm() == false)
                {
                    DisplayPreventESC();
                    return true;
                }


                // NOW we have cleared the way to change pages, so lets see it what they want to do
                switch (loSelectedFragment.aFragmentTag)
                {

                    case Constants.LABEL_APPLICATION_SETTINGS_LOGOUT_OPTION:
                        {
                            Logout();
                            return true;
                        }

                    default:
                        {
                            // fall through to fragments dynamically created from XML config
                            break;
                        }
                }




                // reset each main menu item launch
                DroidContext.MyFragManager.ClearInternalBackstack();


                // clean up any straglers - can be left if they jump out of nested screens with drawer menu
                HideAllRegisteredFragments();


                // hide the keyboard to start for each tab change
                Helper.HideKeyboard(CurrentFocus);


                // update the main content by hiding/showing fragments
                Android.App.Fragment fragment = loSelectedFragment.aFragment;
                Android.App.Fragment old_fragment = gFragmentLastShown;


                // once they catch up they are good, but not ready very first time
                if (old_fragment != null)
                {
                    if (old_fragment.View != null)
                    {
                        // avoid piling on - may hav already been updated HideAllFragments
                        if (old_fragment.View.Visibility != ViewStates.Gone)
                        {
                            old_fragment.View.Visibility = ViewStates.Gone;
                        }
                    }
                }

                if (fragment != null)
                {
                    if (fragment.View != null)
                    {
                        if (fragment.View.Visibility != ViewStates.Visible)
                        {
                            fragment.View.Visibility = ViewStates.Visible;
                        }
                    }
                }


                gFragmentLastShown = fragment;


                if (gHideSlidingDrawer == true)
                {
                    // do nothing

                }
                else
                {

                    //Title = loSelectedFragment.aFragmentTag;
                    Title = loSelectedFragment.aTitleText;
                }



                if (fragment is CommonFragment)
                {
                    RunOnUiThread(() =>
                     {
                         if (IntentRecv_requestCode != 0)
                         {
                             int lo_requestCode = IntentRecv_requestCode;
                             Android.App.Result lo_resultCode = IntentRecv_resultCode;
                             Intent lo_data = IntentRecv_data;

                             var commonFragment = (CommonFragment)fragment;
                             commonFragment._formPanel.OnActivityResultForCommonFragment(lo_requestCode, lo_resultCode, lo_data);

                             IntentRecv_requestCode = 0;
                         }
                         else
                         {
                             //DroidContext.ResetControlStatusByStructName(formNameToUse);
                             // here comes the new ticket
                             ((CommonFragment)fragment).StartTicketLayout();
                         }


                     });
                }
                else if (fragment is IssueSelectFragment)
                {
                    RunOnUiThread(() =>
                     {
                         // show me the records created so far
                         ((IssueSelectFragment)fragment).DisplayMatchingRecordsForDefaultStruct();
                     });


                }


                //// AJW - need to know if this is being selected, TODO restructure and simplify all fragment management into central class
                //bool loTicketsReviewTabIsSelected = (loSelectedFragment.aFragmentTag.Equals(Constants.TICKETS_FRAGMENT_TAG) == true);

                //Log.Debug("OnTabSelected fragment != null", loSelectedFragment.aFragmentTag);
                //if (fragment is TicketsFragment)
                //{
                //    //loTicketsReviewTabIsSelected = true;
                //    var ticketsFragment = (TicketsFragment)fragment;
                //    ticketsFragment.GetTickets(0);
                //}




                //// hide the ones we aren't using right now
                //foreach (FragmentRegistation oneFragmentRegistration in _myFragmentColection)
                //{
                //    if (oneFragmentRegistration.aFragmentTag.Equals(loSelectedFragment.aFragmentTag) == false)
                //    {
                //        Android.App.Fragment fragToHide = FragmentManager.FindFragmentByTag(oneFragmentRegistration.aFragmentTag);
                //        if (fragToHide != null)
                //        {
                //            ft.Hide(fragToHide);
                //        }
                //    }

                //}


#if _use_tabs_
                // hide the review fragment unless it is the one we are moving to
                if (loTicketsReviewTabIsSelected == false)
                {
                    Android.App.Fragment tksFragment = FragmentManager.FindFragmentByTag(Constants.TICKETS_FRAGMENT_TAG);
                    if (tksFragment != null)
                        ft.Hide(tksFragment);
                }


                // Hide the detail type/child fragments that might be around
                Android.App.Fragment detailFragment = FragmentManager.FindFragmentByTag(Constants.DETAIL_FRAGMENT_TAG);
                if (detailFragment != null)
                    ft.Hide(detailFragment);

                Android.App.Fragment voidFragment = FragmentManager.FindFragmentByTag(Constants.VOID_FRAGMENT_TAG);
                if (voidFragment != null)
                    ft.Hide(voidFragment);

                Android.App.Fragment notesFragment = FragmentManager.FindFragmentByTag(Constants.NOTES_FRAGMENT_TAG);
                if (notesFragment != null)
                    ft.Hide(notesFragment);

                Android.App.Fragment noteDetailFragment = FragmentManager.FindFragmentByTag(Constants.NOTE_DETAIL_FRAGMENT_TAG);
                if (noteDetailFragment != null)
                    ft.Hide(noteDetailFragment);

                Android.App.Fragment chalkDetailFragment = FragmentManager.FindFragmentByTag(Constants.CHALK_DETAIL_FRAGMENT_TAG);
                if (chalkDetailFragment != null)
                    ft.Hide(chalkDetailFragment);

                Android.App.Fragment ticketSummaryFragment = FragmentManager.FindFragmentByTag(Constants.SUMMARY_FRAGMENT_TAG);
                if (ticketSummaryFragment != null)
                    ft.Hide(ticketSummaryFragment);
#endif


            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActivity", "OnTabSelected = " + loDebugFragName);
                //Toast.MakeText(_context, "Error loading ticket data.", ToastLength.Long).Show();
            }


            return true;

        }




        public void OnGroupExpand(int iGroupPosition )
        {
            //OnDrawerMenuItemSelected(parent, v, groupPosition, childPosition, id);
            return;
        }

        public void OnGroupCollapse(int iGroupPosition)
        {
            return;
        }

        public bool OnGroupClick(ExpandableListView parent, View v, int groupPosition, long id)
        {
            // any favorites?
            int loFavoritesCount = gFavoriteItems.Count;
            if (loFavoritesCount > 0)
            {
                // it one the top of menu favorite items?
                if (groupPosition < loFavoritesCount)
                {
                    // invoke item
                    OnDrawerMenuItemSelected(parent, v, groupPosition, -1, id);

                    // update selected item title, then close the drawer
                    //Title = mTitles[childPosition];
                    mDrawerLayout.CloseDrawer(mDrawerList);


                    // this click has been handled
                    return true;
                }
            }

            // not a clickable menu item
            return false;
        }
        
        public bool OnChildClick(ExpandableListView parent, View v, int groupPosition, int childPosition, long id)
        {
            // action button clicked - lets do it 
            OnDrawerMenuItemSelected(parent, v, groupPosition, childPosition, id);

            if (parent != null)
            {
                // define as checked/not check for the color schemes to have effect
                int index = parent.GetFlatListPosition(ExpandableListView.GetPackedPositionForChild(groupPosition, childPosition));
                parent.SetItemChecked(index, true);
            }

            // update selected item title, then close the drawer
            //Title = mTitles[childPosition];
            mDrawerLayout.CloseDrawer(mDrawerList);


            return true;
        }

        /* The click listener for RecyclerView in the navigation drawer */
        //public void OnClick(View view, int position)
        //{
        //    selectItem(position);
        //}




        protected override void OnTitleChanged(Java.Lang.ICharSequence title, Android.Graphics.Color color)
        {
            if (gHideSlidingDrawer == true)
            {
                // do nothing - leave action bar alone
            }
            else
            {
                // updadte the action bar

                //base.OnTitleChanged (title, color);
                this.ActionBar.Title = title.ToString();
            }
        }

        /**
         * When using the ActionBarDrawerToggle, you must call it during
         * onPostCreate() and onConfigurationChanged()...
         */

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            // Sync the toggle state after onRestoreInstanceState has occurred.
            if (mDrawerToggle != null)
            {
                mDrawerToggle.SyncState();
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            // Pass any configuration change to the drawer toggls
            if (mDrawerToggle != null)
            {
                mDrawerToggle.OnConfigurationChanged(newConfig);
            }
        }



        public void ChangeToParkingFragment()
        {
            //OnChildClick(null, null, gParkingItemGroupPosition, gParkingtemChildPosition, 0);
            SetActiveFragmentByTag(gParkingItemFragmentTag);
        }


        public void ChangeToTargetFragmentTag(string iTargetFragmentTag)
        {
            RunOnUiThread(() =>
                {

                    SetActiveFragmentByTag(iTargetFragmentTag);

                    //int loGroupPosition = 0;
                    //int loChildPosition = 0;
                    //if (FindRegisteredFragmentMenuPositions(iTargetFragmentTag, out loGroupPosition, out loChildPosition) == true)
                    //{
                    //    OnChildClick(null, null, loGroupPosition, loChildPosition, 0);
                    //}
                });

        }  

        
        private void ListClicked(object sender, DialogClickEventArgs e)
        {

            // TODO - this needs to be handled more elegantly
            gLastPrinterSelectedThroughDialogAsIndex = (int)e.Which;
            gLastPrinterSelectedThroughDialogAsString = gAvailPrinters[(int)e.Which];


            //var items = Resources.GetStringArray(Resource.Array.PrintersList);

            //var builder = new AlertDialog.Builder(this);
            //builder.SetMessage(string.Format("You selected: {0} , {1}", (int)e.Which, items[(int)e.Which]));
            //builder.Show();
        }

        public void SelectPrinter()
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetIconAttribute(Android.Resource.Attribute.AlertDialogIcon);
            builder.SetTitle("Select Printer");
            builder.SetIcon(Resource.Drawable.ic_printer_black_48);


            gAvailPrinters = PrinterSupport_BaseClass.GetListOfSupportedPrinters();
            
            // determine which index the current choice is
            int loDefaultIdx = gAvailPrinters.IndexOf(Constants.PRINTER_TYPE);
            if ( loDefaultIdx == -1 ) 
            {               
                loDefaultIdx = 0;
            }
        
            //builder.SetSingleChoiceItems(Resource.Array.PrintersList, 0, ListClicked);
            builder.SetSingleChoiceItems(gAvailPrinters.ToArray(), loDefaultIdx, ListClicked);


            builder.SetPositiveButton("OK", delegate(object sender, DialogClickEventArgs e)
            {
                //printer selected, get choice and set global
                try
                {

                    string loPrinterSelected = gAvailPrinters[gLastPrinterSelectedThroughDialogAsIndex];
                    Constants.PRINTER_TYPE = loPrinterSelected;


                    ConfigurationLocalOptionsHelper.WriteConfigData();
                    //SaveAppProperties(propertiesDAO);                
                }
                catch (Exception exp)
                {
                    //exp.PrintStackTrace();
                }
            });

            builder.SetNegativeButton("CANCEL", delegate
            {
                // nevermind
            });
            
            builder.Show();

        }

        public string LaunchBlueToothAdmin()
        {
            try
            {
                BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

                if (mBluetoothAdapter == null)
                {
                    return "No Bluetooth Adapter Available";
                }

                Intent settingsBluetooth = new Intent();
                settingsBluetooth.SetAction(Android.Provider.Settings.ActionBluetoothSettings);
                StartActivity(settingsBluetooth);

            }
            catch (Java.Lang.Exception e)
            {

                e.PrintStackTrace();
                return e.StackTrace.ToString();
            }

            return "";
        }

        public async void LaunchCameraForPendingAttachments()
        {
            try
            {
                Console.WriteLine("Start LaunchCameraForPendingAttachments()" );


                try
                {
                    // defense against crashes - if we don't make it back, at least the current partial record gets saved and will be availale when they restart
                    if (gFragmentLastShown != null)
                    {
                        {
                            if (gFragmentLastShown is CommonFragment)
                            {
                                var commonFragment = (CommonFragment)gFragmentLastShown;
                                if (commonFragment._formPanel != null)
                                {
                                    var success = await commonFragment._formPanel.SaveCurrentDisplayPageNoValidation();
                                }
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    LoggingManager.LogApplicationError(exp, "LaunchCameraForPendingAttachments", "SaveCurrentDisplayPage");
                    Console.WriteLine("Exception caught in process: {0} {1}", exp, "LaunchCameraForPendingAttachments calling SaveCurrentDisplayPage");
                }



                string loNewPhotoFileName = System.IO.Path.Combine( Helper.GetMultimediaFolder(), Constants.SERIAL_NUMBER + "_" + DateTimeOffset.Now.ToString("yyyy_MM_dd_HH_mm_ss") + Constants.PHOTO_FILE_SUFFIX);
                // clear it out
                if (_pendingPhotoNext != null)
                {
                    _pendingPhotoNext.Dispose();
                    _pendingPhotoNext = null;
                }
                _pendingPhotoNext = new Java.IO.File(loNewPhotoFileName);
                Android.Net.Uri contentUri = Android.Net.Uri.FromFile(_pendingPhotoNext);

                var cameraIntent = new Intent(MediaStore.ActionImageCapture);
                cameraIntent.AddFlags( 
                                        ActivityFlags.ClearWhenTaskReset | 
                                        ActivityFlags.GrantWriteUriPermission | 
                                        ActivityFlags.GrantReadUriPermission | 
                                        ActivityFlags.NoHistory | 
                                        ActivityFlags.ReceiverRegisteredOnly | 
                                        ActivityFlags.ResetTaskIfNeeded 
                                     );


                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Lollipop)
                {
                    // already done - cameraIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);
                }
                else
                {
                    // for older OS
                    IList<ResolveInfo> resInfoList = this.PackageManager.QueryIntentActivities(cameraIntent, PackageInfoFlags.MatchDefaultOnly);
                    foreach (ResolveInfo resolveInfo in resInfoList)
                    {
                        String packageName = resolveInfo.ActivityInfo.PackageName;
                        this.GrantUriPermission(packageName, contentUri, ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantReadUriPermission);
                    }
                }



                cameraIntent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_pendingPhotoNext));
    
                StartActivityForResult(cameraIntent, Constants.ACTIVITY_REQUEST_CODE_CAPTURE_PHOTO_PENDING_ATTACHMENT);
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActivity::LaunchCameraForPendingAttachments", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::LaunchCameraForPendingAttachments Exception source {0}: {1}", ex.Source, ex.ToString());
            }
        }
        





        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);


            Console.WriteLine("MainActivity.OnActivityResult: requestCode = " + requestCode.ToString() + " resultCode = " + resultCode.ToString());



            switch (requestCode)
            {

                case Constants.ACTIVITY_REQUEST_CODE_MENU_POPUP_NAVIGATION_RESULT:
                    {

                        try
                        {
                            if (resultCode == Android.App.Result.Ok)
                            {
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggingManager.LogApplicationError(ex, "MainActivity::OnActivityResult", ex.TargetSite.Name);
                            System.Console.WriteLine("MainActivity::OnActivityResult Exception source {0}: {1}", ex.Source, ex.ToString());
                        } 

                        break;
                    }


                case Constants.ACTIVITY_REQUEST_CODE_CAPTURE_PHOTO_PENDING_ATTACHMENT:
                    {

                        try
                        {
                            //if (resultCode == Android.App.Result.Ok) //FixMe: Discard and OK button will return cancel code.  
                            {                                
                                if (_pendingPhotoNext != null)
                                {
                                    //If user pressed discard, then the file will not be exist
                                    if (_pendingPhotoNext.Exists() == true)
                                    {
                                        //throw new ArgumentNullException("Attach Photo - Null File name"); //Exception test 
                                        var loRes = DroidContext.AddPhotoToPendingAttachmentList(_pendingPhotoNext.AbsolutePath, 90);
                                    }
                                }
                                else
                                {
                                    throw new ArgumentNullException("Attach Photo - Null File name");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggingManager.LogApplicationError(ex, "MainActivity::OnActivityResult", ex.TargetSite.Name);
                            System.Console.WriteLine("MainActivity::OnActivityResult Exception source {0}: {1}", ex.Source, ex.ToString());
                        } 


                        //// make it available in the gallery
                        //var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                        //Android.Net.Uri contentUri = Android.Net.Uri.FromFile(_photoFile);

                        //mediaScanIntent.SetData(contentUri);
                        ////            SendBroadcast(mediaScanIntent);
                        //ShowImage();
                        
                        break;
                    }

                case Constants.ACTIVITY_REQUEST_CODE_CAPTURE_PHOTO_FOR_LPR:
                    {

                        /*
                        string loFilename = Helper.GetLPRImageFilenameFixed(Constants.PHOTO_FILE_SUFFIX);

                        // second time, rename the file and process it
                        string loFilenameForLPRProcessing = Helper.GetLPRImageFilenameWithTimeStamp(Constants.PHOTO_FILE_SUFFIX);

                        try
                        {
                            System.IO.File.Move(loFilename, loFilenameForLPRProcessing);
                        }
                        catch (Exception exp)
                        {
                            //Toast.MakeText(context, "Error occurred renaming LPR file", ToastLength.Long).Show();
                            Console.WriteLine("Failed to rename file  ANPR:" + exp.Message);

                        }
                        */

                        
                        // go back to the fragment is use when they launched the camera
                        string loFragmentTag = DroidContext.MyFragManager.PeekInternalBackstack();
                        IntentRecv_requestCode = requestCode;
                        IntentRecv_resultCode = resultCode;
                        IntentRecv_data = data;
                        ChangeToTargetFragmentTag( loFragmentTag );


/*
                        // go back to the fragment is use when they launched the camera
                        string tag = DroidContext.MyFragManager.PeekInternalBackstack();
                        Android.App.Fragment fragment = FragmentManager.FindFragmentByTag(tag);



                        if (fragment != null)
                        {

                            if (fragment is CommonFragment)
                            {
                                var commonFragment = (CommonFragment)fragment;
                                commonFragment._formPanel.ShowANPRConfirmationFragmentFormPanel(loFilenameForLPRProcessing);
                            }


                        }
*/
                        break;
                    }


                default:
                    {
                        // unhandled
                        break;
                    }
            }
           



        }


        //Handle Logout
        public void Logout()
        {
            Helper.HideKeyboardFromActivity(this);

            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Confirm Logout");
            builder.SetMessage("Are you sure you want to log out?");
            builder.SetPositiveButton("YES", delegate
                {
                   // //ParkingSequenceADO.ResetAllPendingTickets();
                   // Finish();
                   //// StartService(new Intent(this, typeof (RemoveUserDataService)));
                   // ClearPreferences();


                    ClearPreferences();
                    StartService(new Intent(this, typeof(LoginActivity)));
                    Finish();

                });

            //builder.SetNeutralButton("NO", delegate
            //    {
            //    //    ParkingSequenceADO.ResetAllPendingTickets();
            //        ClearPreferences();
            //        Finish();

            //    });
            builder.SetNegativeButton("CANCEL", delegate
            {
               //jsut a cancel button

            });
            builder.Show();



            //var builder = new AlertDialog.Builder(this);
            //builder.SetTitle("NOTE");
            //builder.SetMessage("Do you want to clear current user data?");
            //builder.SetPositiveButton("YES", delegate
            //    {
            //        ParkingSequenceADO.ResetAllPendingTickets();
            //        Finish();
            //        StartService(new Intent(this, typeof (RemoveUserDataService)));
            //        ClearPreferences();
            //    });

            //builder.SetNeutralButton("NO", delegate
            //    {
            //    //    ParkingSequenceADO.ResetAllPendingTickets();
            //        ClearPreferences();
            //        Finish();

            //    });
            //builder.SetNegativeButton("CANCEL", delegate
            //{
            //   //jsut a cancel button

            //});
            //builder.Show();
        }

        private void ClearPreferences()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            //editor.Clear();
            editor.Remove(Constants.username);
            editor.Commit();
        }

        private void StartGPSTracking()
        {
            try
            {
                if ((DroidContext.gLocationUpdateDuration > 0) && (gIntentActivityLogService != null))
                {
                    var alarmGPS = (AlarmManager)GetSystemService(AlarmService);
                    PendingIntent pendingServiceIntentGPS = PendingIntent.GetService(this, 0, gIntentActivityLogService, PendingIntentFlags.CancelCurrent);
                    long now = SystemClock.CurrentThreadTimeMillis();
                    alarmGPS.SetRepeating(AlarmType.Rtc, now + 60000, 1000 * 60 * DroidContext.gLocationUpdateDuration, pendingServiceIntentGPS);
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.StartGPSTracking", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::StartGPSTracking Exception source {0}: {1}", ex.Source, ex.ToString());
            }
        }

        private void StopGPSTracking()
        {
            try
            {
                if (gIntentActivityLogService != null)
                {
                    var alarmGPS = (AlarmManager)GetSystemService(AlarmService);
                    PendingIntent pendingServiceIntentGPS = PendingIntent.GetService(this, 0, gIntentActivityLogService, PendingIntentFlags.CancelCurrent);
                    alarmGPS.Cancel(pendingServiceIntentGPS);                                        
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.StopGPSTracking", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::StopGPSTracking Exception source {0}: {1}", ex.Source, ex.ToString());
            }
        }

        private void StartFileCleaningUpService()
        {
            try
            {                
                //If the registry value of files types we will remove is empity, then no need to start the service.
                string loFileTypes = TTRegistry.glRegistry.GetRegistryValue(TTRegistry.regSECTION_ISSUE_AP,
                                                                           TTRegistry.regCLEANINGUP_SERVICE_FILETYPES,
                                                                           TTRegistry.regCLEANINGUP_SERVICE_FILETYPES_DEFAULT);
                if( gIntentFileCleaaningUpService == null)
                {
                    return; //No service, return now.
                }


                // we will always try to fire up the filecleaning service, even if the registry doesn't have any info
                // other db maintenance functions are hooked in to this service
                var alarmFileCleaaningUpService = (AlarmManager)GetSystemService(AlarmService);
                
                PendingIntent pendingServiceIntentGPS = PendingIntent.GetService(this, 0, gIntentFileCleaaningUpService, PendingIntentFlags.CancelCurrent);
                long now = SystemClock.CurrentThreadTimeMillis();
                //Get the repeating interval from the registry
                long loFreq = (long)TTRegistry.glRegistry.GetRegistryValueAsInt(   TTRegistry.regSECTION_ISSUE_AP,
                                                                            TTRegistry.regCLEANINGUP_SERVICE_INTERVAL_HOURS,
                                                                            TTRegistry.regCLEANINGUP_SERVICE_INTERVAL_HOURS_DEFAULT);
                loFreq = loFreq * 60 * 60 * 1000;  //convert hours value to msec                

#if _production_
                long loStartTime = now + (1 * 60 * 60 * 1000); //start after 1 hr
#else
                long loStartTime = now + (3 * 60 * 1000); //start after 3 min
#endif



                alarmFileCleaaningUpService.SetRepeating(AlarmType.Rtc, loStartTime, loFreq, pendingServiceIntentGPS);                
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.StartFileCleaningUpService", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::StartFileCleaningUpService Exception source {0}: {1}", ex.Source, ex.ToString());
            }
        }

        private void StopFileCleaningUpService()
        {
            try
            {
                var alarmFileCleaaningUpService = (AlarmManager)GetSystemService(AlarmService);                
                PendingIntent pendingServiceIntentGPS = PendingIntent.GetService(this, 0, gIntentFileCleaaningUpService, PendingIntentFlags.CancelCurrent);
                alarmFileCleaaningUpService.Cancel(pendingServiceIntentGPS);
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MainActvity.StopFileCleaningUpService", ex.TargetSite.Name);
                System.Console.WriteLine("MainActivity::StopFileCleaningUpService Exception source {0}: {1}", ex.Source, ex.ToString());
            }
        }

    }
}
