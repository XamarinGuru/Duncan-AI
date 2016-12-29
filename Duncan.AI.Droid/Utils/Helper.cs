

// AJW - TODO if we use this, then the anticipation gets broken, plus the list is always displayed from item 0
//  we need a way for anticipation to work and scroll to closest item on the full list
#define _use_custom_adapter_
//#define _adapter_1_ 
#define _adapter_2_ 


using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Duncan.AI.Droid.Utils.DataAccess;
using Duncan.AI.Droid.Utils.HelperManagers;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;

using Android.App;
using Android.Content;
using Android.Widget;
using Android.Util;
using Android.Views;
using Android.Runtime;
using Android.OS;
using Android.Views.InputMethods;

using System.Collections.Generic;
using System.Text;
using Android.Graphics;
using System.Text.RegularExpressions;
using System.Globalization;

using Reino.ClientConfig;
using XMLConfig;
using Duncan.AI.Droid.Utils.EditControlManagement;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using EditRestrictionConsts = Duncan.AI.Droid.Utils.EditControlManagement.Entities.EditRestrictionConsts;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils;



using Duncan.AI.Droid.Utils.PrinterSupport;


namespace Duncan.AI.Droid
{
    public class Helper
    {
        public Helper()
        {
        }

        /// <summary>
        /// Shows the soft input keyboard
        /// 
        ///  make sure you set the Request Field before calling the ShowKeyboard
        /// </summary>
        /// <param name="pView"></param>
        public static void ShowKeyboard(View pView)
        {
            if (pView != null)
            {

                if (pView.IsFocused == true)
                {
                    // sends it off the deep end.... why ?   pView.ClearFocus();
                }

                pView.RequestFocus();

                InputMethodManager inputMethodManager = pView.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;

                if (inputMethodManager != null)
                {
                    inputMethodManager.ShowSoftInput(pView, ShowFlags.Forced);
                    inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
                }
            }
        }

        /// <summary>
        /// And to hide the keyboard
        /// </summary>
        /// <param name="pView"></param>
        public static void HideKeyboard(View pView)
        {
            if (pView != null)
            {
                try
                {
                    InputMethodManager inputMethodManager = pView.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
                    if (inputMethodManager != null)
                    {
                        // In some cases you will want to pass in InputMethodManager.HIDE_IMPLICIT_ONLY as the second parameter
                        // to ensure you only hide the keyboard when the user didn't explicitly force it to appear (by holding down menu).
                        inputMethodManager.HideSoftInputFromWindow(pView.WindowToken, HideSoftInputFlags.None);
                    }
                }
                catch (Exception exp)
                {

                }
            }
        }


        /// <summary>
        /// Hides soft input keyboard from an Activity - NOT from a DialogFragment. If called from a DialogFragment,
        /// 
        /// HideKeyboard(getActivity()); //won't work
        /// 
        /// This won't work because you'll be passing a reference to the Fragment's host Activity, which will have 
        /// no focused control while the Fragment is shown! Wow! So, for hiding the keyboard from fragments, 
        /// must use the common HideKeyboard( View pView ) above
        /// </summary>
        /// <param name="activity"></param>
        public static void HideKeyboardFromActivity(Activity activity)
        {
            try
            {
                InputMethodManager imm = (InputMethodManager)activity.GetSystemService(Activity.InputMethodService);

                //Find the currently focused view, so we can grab the correct window token from it.
                View view = activity.CurrentFocus;

                //If no view currently has focus, create a new one, just so we can grab a window token from it
                if (view == null)
                {
                    view = new View(activity);
                }
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Failed to Hide Keyboard From Activity:" + exp.Message);
            }
        }


        public static void HideKeyboardFromFragment(Context ctx)
        {
            try
            {
                InputMethodManager inputManager = (InputMethodManager)ctx.GetSystemService(Context.InputMethodService);

                // check if no view has focus:
                View v = ((Activity)ctx).CurrentFocus;
                if (v == null)
                    return;

                inputManager.HideSoftInputFromWindow(v.WindowToken, 0);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Failed to Hide Keyboard From Fragment:" + exp.Message);
            }
        }

        public static string BuildAutoISSUEPublicServiceHostURL( string iURLRoot, string iClientName )
        {
            // we'll append this static suffix to the machine URL 
            const string AIPublicWebServiceAddrSuffix = @"AutoISSUEPublic/AutoISSUEPublicService.asmx";


            if (string.IsNullOrEmpty(iURLRoot) == true)
            {
                return "";
            }

            if (string.IsNullOrEmpty(iClientName) == true)
            {
                return "";
            }


            string loHostURL = iURLRoot +
                                @"/ReinoWebServices/" +
                                Uri.EscapeDataString(iClientName) +
                                AIPublicWebServiceAddrSuffix;


            // help out - if they neglected to prefix with http OR HTTPS, add it for them
            if (
                (loHostURL.ToUpper().StartsWith(@"HTTP://") == false) &&
                // AUTOCITE-260  - need to allow secure connections too
                (loHostURL.ToUpper().StartsWith(@"HTTPS://") == false)
               )
            {
                // avoid extra slashes when the ServiceULR is empty
                if (iURLRoot.Equals("") == true)
                {
                    loHostURL = @"http:/" + loHostURL;
                }
                else
                {
                    loHostURL = @"http://" + loHostURL;
                }
            }

            return loHostURL;

        }



        /// <summary>
        /// Helper function to capitalize first letter of each word
        /// </summary>
        /// <param name="iTitleText"></param>
        /// <returns></returns>
        public static string FormatTitleText(string iTitleText)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            // have to force to lower case so that it is not confused by multiple captial letters
            string loFormattedTitle = textInfo.ToTitleCase(iTitleText.ToLower());


            // look for a few exceptions
            if (loFormattedTitle.Contains("Vin ") == true)
            {
                loFormattedTitle = loFormattedTitle.Replace("Vin ", "VIN ");
            }

            return loFormattedTitle;
        }




        public static void SetTypefaceForTextView(TextView iTextView, string iTypefaceName, float iTypefaceSizeSp)
        {
            // initialize our typeface 
            Typeface loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, iTypefaceName);
            if (loCustomTypeface != null)
            {
                iTextView.Typeface = loCustomTypeface;
            }
            iTextView.SetTextSize(Android.Util.ComplexUnitType.Sp, iTypefaceSizeSp);
        }

        public static void SetTypefaceForButton(Button iButton, string iTypefaceName, float iTypefaceSizeSp)
        {
            // initialize our typeface 
            Typeface loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, iTypefaceName);
            if (loCustomTypeface != null)
            {
                iButton.Typeface = loCustomTypeface;
            }
            iButton.SetTextSize(Android.Util.ComplexUnitType.Sp, iTypefaceSizeSp);
        }

        public static void SetTypefaceForEditText(EditText iEditText, string iTypefaceName, float iTypefaceSizeSp)
        {
            // initialize our typeface 
            Typeface loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, iTypefaceName);
            if (loCustomTypeface != null)
            {
                iEditText.Typeface = loCustomTypeface;
            }
            iEditText.SetTextSize(Android.Util.ComplexUnitType.Sp, iTypefaceSizeSp);
        }


        /// <summary>
        /// Helper method to return casted value
        /// </summary>
        /// <param name="uiComponent"></param>
        /// <returns></returns>
        public static EditControlBehavior GetBehaviorAndroidFromUIComponent(View uiComponent)
        {
            EditControlBehavior uiComponentBehavior = null;

            // extract and deference the behavior component - the BehaviorAndroid instance
            if (uiComponent != null)
            {
                if (uiComponent is CustomEditText)
                {
                    uiComponentBehavior = ((CustomEditText)uiComponent).BehaviorAndroid;
                }
                else if (uiComponent is CustomAutoTextView)
                {
                    uiComponentBehavior = ((CustomAutoTextView)uiComponent).BehaviorAndroid;
                }
                else if (uiComponent is CustomSpinner)
                {
                    uiComponentBehavior = ((CustomSpinner)uiComponent).BehaviorAndroid;
                }
                else if (uiComponent is CustomSignatureImageView)
                {
                    uiComponentBehavior = ((CustomSignatureImageView)uiComponent).BehaviorAndroid;
                }
            }

            return uiComponentBehavior;
        }

        #region Path Builder Support - for using URL settings from REGISTRY
        public static string UrlCombine(string url1, string url2)
        {
            if (url1.Length == 0)
            {
                return url2;
            }

            if (url2.Length == 0)
            {
                return url1;
            }

            url1 = url1.TrimEnd('/', '\\');
            url2 = url2.TrimStart('/', '\\');

            return string.Format("{0}/{1}", url1, url2);
        }
        #endregion

        //private static string gVerifiedMultimediaFolder = string.Empty;
        private static Java.IO.File gVerifiedMultimediaFolder = null;

        public static string GetMultimediaFolder()
        {
            if (gVerifiedMultimediaFolder == null)
            {
                gVerifiedMultimediaFolder = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), Constants.MULTIMEDIA_FOLDERNAME);

                if (gVerifiedMultimediaFolder.Exists() == false)
                {
                    gVerifiedMultimediaFolder.Mkdirs();
                }
            }

            if (gVerifiedMultimediaFolder != null)
            {
                return gVerifiedMultimediaFolder.AbsolutePath;
            }
            else
            {
                return "";
            }

        }


        public static string GetLPRImageFilenameWithTimeStamp(String fileSuffix)
        {
            string loLPRFilename = System.IO.Path.Combine(Helper.GetMultimediaFolder(), Constants.SERIAL_NUMBER + "_LPR_CANDIDATE_" + DateTimeOffset.Now.ToString("yyyy_MM_dd_HH_mm_ss") + Constants.PHOTO_FILE_SUFFIX);

            return loLPRFilename;
        }




        public static string GetLPRImageFilenameFixed(String fileSuffix)
        {
            //  return a consistent filenmame to overwrite the last each time
            string loLPRFilename = System.IO.Path.Combine(Helper.GetMultimediaFolder(), Constants.SERIAL_NUMBER + "_LPR_CANDIDATE" + Constants.PHOTO_FILE_SUFFIX);

            return loLPRFilename;
        }



        /// <summary>
        ///  Returns a consistent key name
        /// </summary>
        /// <returns></returns>
        public static string BuildGlobalPreferenceKeyName(string iStructNameOrGlobalValuePrefix, string iFieldName)
        {
            return iStructNameOrGlobalValuePrefix + "-" + iFieldName;
        }


        /// <summary>
        ///  Returns a consistent tag name
        /// </summary>
        /// <returns></returns>
        public static string BuildIssueNewFragmentTag(string iStructName)
        {
            return Constants.ISSUE_NEW_FRAGMENT_TAG_PREFIX + iStructName;
        }


        /// <summary>
        ///  Returns a consistent tag name
        /// </summary>
        /// <returns></returns>
        public static string BuildIssueNew2FragmentTag(string iStructName)
        {
            return Constants.ISSUE_NEW2_FRAGMENT_TAG_PREFIX + iStructName;
        }

        /// <summary>
        ///  Returns a consistent tag name
        /// </summary>
        /// <returns></returns>
        public static string BuildIssueSelectFragmentTag(string iStructName)
        {
            return Constants.ISSUE_SELECT_FRAGMENT_TAG_PREFIX + iStructName;
        }




        /// <summary>
        ///  Returns a consistent tag name
        /// </summary>
        /// <returns></returns>
        public static string BuildWebViewClientDefinableTag( int iIndex )
        {
            return Constants.WEBVIEW_CLIENT_DEFINABLE_FRAGMENT_TAG_PREFIX + iIndex.ToString();
        }

        /// <summary>
        ///  Returns a consistent tag name
        /// </summary>
        /// <returns></returns>
        public static string BuildIssueReviewFragmentTag(string iStructName)
        {
            return Constants.ISSUE_REVIEW_FRAGMENT_TAG_PREFIX + iStructName;
        }

        /// <summary>
        ///  Returns a consistent tag name
        /// </summary>
        /// <returns></returns>
        public static string BuildIssueNotesFragmentTag(string iStructName)
        {
            return Constants.ISSUE_NOTES_FRAGMENT_TAG_PREFIX + iStructName;
        }

        /// <summary>
        ///  Returns a consistent tag name
        /// </summary>
        /// <returns></returns>
        public static string BuildIssueNoteDetailFragmentTag(string iStructName)
        {
            return Constants.ISSUE_NOTES_DETAIL_FRAGMENT_TAG_PREFIX + iStructName;
        }


        /// <summary>
        ///  Returns a consistent tag name
        /// </summary>
        /// <returns></returns>

        public static string BuildNotesReviewSelectFragmentTag( string iStructName )
        {
            return Constants.NOTES_REVIEW_SELECT_FRAGMENT_TAG_PREFIX + iStructName;
        }

        /// <summary>
        ///  Returns a consistent tag name
        /// </summary>
        /// <returns></returns>

        public static string BuildNotesReviewDetailFragmentTag(string iStructName)
        {
            return Constants.NOTES_REVIEW_DETAIL_FRAGMENT_TAG_PREFIX + iStructName;
        }





        /// <summary>
        ///  Returns a consistent tag name
        /// </summary>
        /// <returns></returns>
        public static string BuildIssueVoidFragmentTag(string iStructName)
        {
            return Constants.ISSUE_VOID_FRAGMENT_TAG_PREFIX + iStructName;
        }

        /// <summary>
        ///  Returns a consistent tag name
        /// </summary>
        /// <returns></returns>
        public static string BuildSearchMatchFragmentTag(string iStructName)
        {
            return Constants.SEARCH_MATCH_FRAGMENT_TAG_PREFIX + iStructName;
        }

        

        /// <summary>
        /// Returns a consistent tag value for label field
        /// </summary>
        /// <param name="panelField"></param>
        /// <param name="ctx"></param>
        /// <param name="issStruct"></param>
        /// <returns></returns>
        public static string GetLabelFieldTag(PanelField panelField)
        {
            // build a consistent, valid and unique tag
            string loResultTag = panelField.fParentStructName + panelField.Name + Constants.LABEL_OBJECT_NAME_SUFFIX;
            return loResultTag;
        }

        //Make Label
        public static View MakeLabel(PanelField panelField, Context ctx, IssStruct issStruct)
        {
            var labelText = (panelField.Label ?? panelField.Name);

            var labelTag = GetLabelFieldTag(panelField);

            var textView = new TextView(ctx)
                {
                    // required property can be conditional, its too soon to know 
                    // it would be required for this record - let the TER_Required class update
                    // Text = panelField.IsRequired ? labelText + " *" : labelText,
                    Text = labelText,
                    Tag = labelTag
                };


            // initialize our typeface 
            Typeface loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnEditTextLabelTypeface);
            if (loCustomTypeface != null)
            {
                textView.Typeface = loCustomTypeface;
            }
            textView.SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnEditTextLabelTypefaceSizeSp);


            return textView;
        }


        public enum ListItemMatchType
        {
            searchNoPartialMatch,
            searchAllowPartialMatch
        }

        public static int GetListItemIndexFromArrayAdapter(ArrayAdapter iAdapter, string iListItemText, ListItemMatchType iMatchStyle)
        {
            // assume no match can be made
            int loResultIdx = -1;

            // no adapter?
            if (iAdapter == null)
            {
                return loResultIdx;
            }


            // first use the built in complete matching search
            loResultIdx = iAdapter.GetPosition(iListItemText);

            // if we got something, we're done
            if (loResultIdx != -1)
            {
                return loResultIdx;
            }




            // no full match, lets try partial match, up to the list seperator
            string iListItemTextTrimmed = iListItemText.Trim();
            for (int loItemIdx = 0; loItemIdx < iAdapter.Count; loItemIdx++)
            {
                string oneItem = (string)iAdapter.GetItem(loItemIdx);

                // remove abbrev descriptions when present
                int loPos = oneItem.IndexOf(Constants.LIST_ITEM_DESCRIPTION_SEPARATOR);
                // has to be beyond the first char
                if (loPos > 0)
                {
                    // keep everything up to the space preceeding the seperator char
                    oneItem = oneItem.Substring(0, loPos - 1);
                }

                if (oneItem.Length >= iListItemTextTrimmed.Length)
                {
                    // if the list item is blank, it can only match if the compared entry is blank
                    if (iListItemTextTrimmed.Length == 0)
                    {
                        if (oneItem.Length != 0)
                        {
                            // substring comparison will return false positive for comparing an empty string, skip that
                            continue;
                        }
                    }


                    if (iMatchStyle == ListItemMatchType.searchAllowPartialMatch)
                    {
                        // only needs to match up to the length on the source
                        if (oneItem.Substring(0, iListItemTextTrimmed.Length).Equals(iListItemTextTrimmed) == true)
                        {
                            loResultIdx = loItemIdx;
                            break;
                        }
                    }
                    else
                    {
                        // has to match completely
                        if (oneItem.Trim().Equals(iListItemTextTrimmed) == true)
                        {
                            loResultIdx = loItemIdx;
                            break;
                        }
                    }


                }
            }

            // what we got
            return loResultIdx;
        }


        public static int GetListItemIndexFromStringList(List<string> iStringList, string iListItemText, ListItemMatchType iMatchStyle)
        {
            // assume no match can be made
            int loResultIdx = -1;

            // no list?
            if (iStringList == null)
            {
                return loResultIdx;
            }


            // empty string (not blank spaces, but empty)
            if (string.IsNullOrEmpty(iListItemText) == true)
            {
                return loResultIdx;
            }

            // first use the built in complete matching search
            loResultIdx = iStringList.IndexOf(iListItemText);

            // if we got something, we're done
            if (loResultIdx != -1)
            {
                return loResultIdx;
            }



            // no full match, lets try partial match, up to the list seperator
            string iListItemTextTrimmed = iListItemText.Trim();
            // remove abbrev descriptions when present
            int loPosSep = iListItemTextTrimmed.IndexOf(Constants.LIST_ITEM_DESCRIPTION_SEPARATOR);
            // has to be beyond the first char
            if (loPosSep > 0)
            {
                // keep everything up to the space preceeding the seperator char
                iListItemTextTrimmed = iListItemTextTrimmed.Substring(0, loPosSep - 1);
            }




            for (int loItemIdx = 0; loItemIdx < iStringList.Count; loItemIdx++)
            {
                string oneItem = iStringList[loItemIdx];
                // remove abbrev descriptions when present
                int loPos = oneItem.IndexOf(Constants.LIST_ITEM_DESCRIPTION_SEPARATOR);
                // has to be beyond the first char
                if (loPos > 0)
                {
                    // keep everything up to the space preceeding the seperator char
                    oneItem = oneItem.Substring(0, loPos - 1);
                }


                if (oneItem.Length >= iListItemTextTrimmed.Length)
                {
                    // if the list item is blank, it can only match if the compared entry is blank
                    if (iListItemTextTrimmed.Length == 0)
                    {
                        if (oneItem.Length != 0)
                        {
                            // substring comparison will return false positive for comparing an empty string, skip that
                            continue;
                        }
                    }

                    if (iMatchStyle == ListItemMatchType.searchAllowPartialMatch)
                    {
                        // only needs to match up to the length on the source
                        if (oneItem.Substring(0, iListItemTextTrimmed.Length).Equals(iListItemTextTrimmed) == true)
                        {
                            loResultIdx = loItemIdx;
                            break;
                        }
                    }
                    else
                    {
                        // has to match completely
                        if (oneItem.Trim().Equals(iListItemTextTrimmed) == true)
                        {
                            loResultIdx = loItemIdx;
                            break;
                        }
                    }

                }
            }

            // what we got
            return loResultIdx;
        }


        public static int GetListItemIndexFromStringArray(string[] iStringArray, string iListItemText, ListItemMatchType iMatchStyle)
        {
            if (iStringArray != null)
            {
                return GetListItemIndexFromStringList(new List<string>(iStringArray), iListItemText, iMatchStyle);
            }
            else
            {
                return -1;
            }
        }


/// <summary>
/// Return EditControlBehavoir for generic UI object
/// </summary>
/// <param name="uiComponent"></param>
/// <returns></returns>
        public static EditControlBehavior GetSafeEditControlBehaviorForCustomView(object uiComponent)
        {
            if (uiComponent != null)
            {
                if (uiComponent is CustomEditText)
                {
                    return ((CustomEditText)uiComponent).BehaviorAndroid;
                }


                if (uiComponent is CustomAutoTextView)
                {
                    return ((CustomAutoTextView)uiComponent).BehaviorAndroid;
                }


                if (uiComponent is CustomSpinner)
                {
                    return ((CustomSpinner)uiComponent).BehaviorAndroid;
                }

            }

            return null;
        }

        /// <summary>
        /// Enhanced field updating with Android specific handling of updating View controls
        /// </summary>
        /// <param name="iNewValue"></param>
        public static void UpdateControlWithNewValuePrim(EditControlBehavior iParent, string iNewValue, EditEnumerations.IgnoreEventsType iIgnoreEvents, EditEnumerations.SetHasBeenFocusedType iSetHasBeenFocused)
        {
            // translate NULLs into empty strings
            if (string.IsNullOrEmpty(iNewValue))
            {
                //set this to empty string since once this changes, we need to clear the controls that might have already had values.
                iNewValue = string.Empty;
            }

            // may need to convert dates...
            if ( string.IsNullOrEmpty(iNewValue) == false )
            {
                // if this is non-null after evaluation, it will be used in place of db raw string value
                string loConvertedFromDBFormatOverrideString = null;

                // safe extraction from datarow
                string loOneValueAsString = iNewValue;

                // format the date/time types
                switch (iParent.PanelField.FieldType)
                {
                    case "efTime":          // this is really lame type evaluation :-(
                        {
                            if ((loOneValueAsString != null) && (iParent.PanelField.EditMask != null))
                            {
                                if ((loOneValueAsString.Length > 0) && (iParent.PanelField.EditMask.Length > 0))
                                {
                                    // convert from fixed DB format to panel format
                                    loConvertedFromDBFormatOverrideString = DateTimeHelper.ConvertDBTimeColumnValueToString(loOneValueAsString, iParent.PanelField.EditMask);
                                }
                            }
                            break;
                        }

                    case "efDate":
                        {
                            if ((loOneValueAsString != null) && (iParent.PanelField.EditMask != null))
                            {
                                if ((loOneValueAsString.Length > 0) && (iParent.PanelField.EditMask.Length > 0))
                                {
                                    // convert from fixed DB format to panel format
                                    loConvertedFromDBFormatOverrideString = DateTimeHelper.ConvertDBDateColumnValueToString(loOneValueAsString, iParent.PanelField.EditMask);
                                }
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                // use the override if available
                if (loConvertedFromDBFormatOverrideString != null)
                {
                    iNewValue = loConvertedFromDBFormatOverrideString;
                }

            }







            // call the appropriate object type
            if (iParent.ControlType == EditEnumerations.CustomEditControlType.EditText)
            {
                var customControl = (CustomEditText)iParent.EditCtrl;
                if (customControl != null)
                {

                    bool loSavedIgnoreEventsState = customControl.IgnoreEvents;
                    if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                    {
                        customControl.IgnoreEvents = true;
                    }

                    try
                    {
                        customControl.Text = iNewValue;

                        if (iSetHasBeenFocused == EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue)
                        {
                            customControl.HasBeenFocused = true;
                            //customControl.FormStatus = "Processed";
                        }
                    }
                    catch (Exception exp)
                    {
                        LoggingManager.LogApplicationError(exp, "FieldName: " + customControl.CustomId, "UpdateParentControlWithNewValue");
                        Console.WriteLine("UpdateParentControlWithNewValue: {0} {1}", exp, customControl.CustomId);
                    }
                    finally
                    {
                        if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                        {
                            customControl.IgnoreEvents = loSavedIgnoreEventsState;
                        }
                    }
                }
            }


            if (iParent.ControlType == EditEnumerations.CustomEditControlType.AutoCompleteText)
            {
                var customControl = (CustomAutoTextView)iParent.EditCtrl;
                if (customControl != null)
                {
                    bool loSavedIgnoreEventsState = customControl.IgnoreEvents;
                    if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                    {
                        customControl.IgnoreEvents = true;
                    }

                    try
                    {

                        // lets see if this text is a match for a list item
                        int loIdx = -1;
                        if (customControl.BehaviorAndroid != null)
                        {
                            // see if its a list item
                            loIdx = Helper.GetListItemIndexFromStringList(customControl.BehaviorAndroid.GetFilteredListItems(), iNewValue, Helper.ListItemMatchType.searchNoPartialMatch);

                            // lets get the list item, which might include abbrev + text desc
                            if (loIdx != -1)
                            {
                                iNewValue = customControl.BehaviorAndroid.GetFilteredListItems()[loIdx];
                            }
                        }


                        /* this causes other bad side effects    - ew need to have CLEAR button override
                         * 
                        // kludge - buffers do not stay in sync!
                        customControl.BehaviorAndroid.SetText(iNewValue);
                         */

                        // now update the control with the value
                        customControl.SetListItemDataByValue(iNewValue);


                        if (iSetHasBeenFocused == EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue)
                        {
                            // TODO - there should be a InitForEntry to take care of this kind of stuff
                            customControl.HasBeenFocused = true;
                            //customControl.FormStatus = "Processed";
                        }

                    }
                    catch (Exception exp)
                    {
                        LoggingManager.LogApplicationError(exp, "FieldName: " + customControl.CustomId, "UpdateParentControlWithNewValue");
                        Console.WriteLine("UpdateParentControlWithNewValue: {0} {1}", exp, customControl.CustomId);
                    }
                    finally
                    {
                        if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                        {
                            customControl.IgnoreEvents = loSavedIgnoreEventsState;
                        }
                    }

                }
            }


            if (iParent.ControlType == EditEnumerations.CustomEditControlType.Spinner)
            {
                var customControl = (CustomSpinner)iParent.EditCtrl;
                if (customControl != null)
                {

                    bool loSavedIgnoreEventsState = customControl.IgnoreEvents;
                    if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                    {
                        customControl.IgnoreEvents = true;
                    }

                    try
                    {

                        if (string.IsNullOrEmpty(iNewValue))
                        {
                            customControl.SetListIndex(-1);
                            //customControl.SetListIndex(0);
                        }
                        else
                        {
                            customControl.SetListItemDataByValue(iNewValue);
                        }


                        if (iSetHasBeenFocused == EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue)
                        {
                            // TODO - there should be a InitForEntry to take care of this kind of stuff
                            customControl.HasBeenFocused = true;
                            //customControl.FormStatus = "Processed";
                        }


                    }
                    catch (Exception exp)
                    {
                        LoggingManager.LogApplicationError(exp, "FieldName: " + customControl.CustomId, "UpdateParentControlWithNewValue");
                        Console.WriteLine("UpdateParentControlWithNewValue: {0} {1}", exp, customControl.CustomId);
                    }
                    finally
                    {
                        if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                        {
                            customControl.IgnoreEvents = loSavedIgnoreEventsState;
                        }
                    }

                }
            }

        }






        /// <summary>
        /// Change the list on an existing AutoCompleteTextView
        /// </summary>
        /// <param name="iNewValue"></param>
        public static void UpdateControlWithNewListPrim(CustomAutoTextView iParent, string[] iNewList, EditEnumerations.IgnoreEventsType iIgnoreEvents, EditEnumerations.SetHasBeenFocusedType iSetHasBeenFocused)
        {

#if _use_custom_adapter_

#if _adapter_2_

                        //// here's another implmentation -

                        var autoCompleteAdapter = new CustomAutoCompleteTextViewAdapter2(iParent.Context);
                        autoCompleteAdapter.OriginalItems = iNewList;

                        //// tried using a small lists instead of agency lists, still crashes
                        //string[] dummyitems = { "ALFA", "BETA", "CAPTAIN", "DAVID", "EDWARD", "FRANK", "GEORGE", "HARRY" };
                        //autoCompleteAdapter.OriginalItems = dummyitems;

                        iParent.Adapter = autoCompleteAdapter;


            

                        //var dispWidth = metrics.WidthPixels;
                        //autoTextView.DropDownWidth = dispWidth;
#endif

#else
            // create or update?
            //if (iParent.Adapter != null)
            //{
            //    if (iParent.Adapter is ArrayAdapter)
            //    {
            //        ArrayAdapter oneAdapter = (ArrayAdapter)iParent.Adapter;
            //        oneAdapter.Clear();
            //        oneAdapter.Add(iNewList);
            //        oneAdapter.NotifyDataSetChanged();
            //    }

            //}
            //else
            {
                // using the standard adapter works without crashing - but we don't like the default filtering
                //var autoCompleteAdapter = new ArrayAdapter(iParent.Context, Android.Resource.Layout.SimpleSpinnerItem, iNewList);   
                //ArrayAdapter<String> adapter = new ArrayAdapter(iParent.Context, Android.Resource.Layout.SimpleSpinnerDropDownItem, iNewList);
                //var autoCompleteAdapter = new ArrayAdapter(iParent.Context, Android.Resource.Layout.SimpleSpinnerItem, iNewList);   // deal with the white text bug
                var autoCompleteAdapter = new ArrayAdapter(iParent.Context, Android.Resource.Layout.SimpleDropDownItem1Line, iNewList);   // deal with the white text bug
                iParent.Adapter = autoCompleteAdapter;
            }
#endif
        }




        public static int gAdapterCount = 0;

        //Make AutoTextView
        public static CustomAutoTextView MakeAutoTextView(PanelField panelField, Context ctx, DisplayMetrics metrics, LinearLayout layout, Panel panel, IssStruct issStruct)
        {
            try
            {
                //go find the custom auto text view that has a behavior assigned to it.
                var autoTextView = DroidContext.XmlCfg.CustomAutoTextViews.FirstOrDefault(x => x.CustomId == issStruct.Name + panelField.Name);
                if (autoTextView == null)
                {
                    return null;
                }


                // autoTextView.SetDataSource();
                //autoTextView.ImeOptions = Android.Views.InputMethods.ImeAction.Next; AJW TODO - the NEXT can cause crash when the next field is covered by the toolbar or otherwise not visible
                autoTextView.ImeOptions = Android.Views.InputMethods.ImeAction.None;

                //autoTextView.ImeOptions = Android.Views.InputMethods.ImeAction.Next;




                // we only need to set the datasource if we didn't already do it - re-assigning it on an existing field resets the current values
                // this is also true of other event handlers (?)
                if (autoTextView.Adapter == null)
                {

                    if (metrics != null)
                    {
                        autoTextView.SetWidth(Convert.ToInt32(metrics.WidthPixels * panelField.Width));
                    }

                    string[] response;
                    //wire up list items


                    // AJW - AutoComplete should be displaying the list for the actual field, not the parent's list
                    /*
                               if (panelField.ParentPanelField != null)
                                   response = (new ListSupport()).GetListDataByTableColumnName(panelField.ParentPanelField.OptionsList.ListName, panelField.ParentPanelField.OptionsList.saveColumn);
                               else
                                   response = (new ListSupport()).GetListDataByTableColumnName(panelField.OptionsList.ListName, ConcatColumns(panelField.OptionsList.Columns));
                               var autoCompleteOptions = response;
                   */
                    response = (new ListSupport()).GetListDataByTableColumnName(panelField.OptionsList.ListName, ConcatColumns(panelField.OptionsList.Columns));
                    var autoCompleteOptions = response;






#if _old__use_custom_adapter_


                //a lot of this code was to help narrow down which field makes it choke, but in 
                // the debugging the crash is inconsistent.
                // in fact in often runs without crashing if you step through a but an then let it fly
                // as if some background thread is finished up and the this component can be used.... ?



                if (autoCompleteOptions != null)
                {
                    gAdapterCount++;

                    bool loUseCustomAdapter = false;

                    int loTargetIdx = 12;    // _adapter_1_ = usually fails around 10
                    //loUseCustomAdapter = (gAdapterCount <= loTargetIdx);

                    switch ( panelField.Name )
                    {
                        case "LOCDESCRIPTOR":
                        case "LOCCROSSSTREET1":
                            {
                                // this doesn't fix it
                                loUseCustomAdapter = false;
                                break;
                            }
                        default:
                            {
                                loUseCustomAdapter = (gAdapterCount <= loTargetIdx);
                                break;
                            }
                    }

                    
                    if (panelField.OptionsList.ListName.StartsWith("STREETTYPE") == true)
                    {
                        // this doesn't fix it
                        loUseCustomAdapter = false;
                    }
                    

                    //if ( panelField.IsHidden == false )
                    //{
                    //    // this doesn't fix it
                    //    loUseCustomAdapter = false;
                    //}




                    if (loUseCustomAdapter == true)
                    {

                        if (gAdapterCount == loTargetIdx-1)
                        {
                            //Console.WriteLine("breakpoint");
                        }

                        if (gAdapterCount == loTargetIdx)
                        {
                            //Console.WriteLine("breakpoint");
                        }


                        Console.WriteLine("_use_custom_adapter_ " + panelField.Name + " [" + panelField.OptionsList.ListName + "] " + autoCompleteOptions.Length.ToString());


#if _adapter_1_

                        // using this crashes the app

                        //// this is our own custom adapter filter that shows the entire list on drop downs
                        JavaList objects = new JavaList(autoCompleteOptions);
                        var autoCompleteAdapter = new CustomAutoCompleteTextViewAdapter(ctx, Android.Resource.Layout.SimpleSpinnerItem, objects);
                        autoTextView.Adapter = autoCompleteAdapter;
#endif
#if _adapter_2_

                        //// here's another implmentation - using this one crashes too

                        var autoCompleteAdapter = new CustomAutoCompleteTextViewAdapter2(ctx);
                        autoCompleteAdapter.OriginalItems = autoCompleteOptions;

                        //// tried using a small lists instead of agency lists, still crashes
                        //string[] dummyitems = { "ALFA", "BETA", "CAPTAIN", "DAVID", "EDWARD", "FRANK", "GEORGE", "HARRY" };
                        //autoCompleteAdapter.OriginalItems = dummyitems;

                        autoTextView.Adapter = autoCompleteAdapter;

                        //var dispWidth = metrics.WidthPixels;
                        //autoTextView.DropDownWidth = dispWidth;
#endif
#if _adapter_3_

                        //// here's another implmentation - using this one crashes too


                        //var autoCompleteAdapter = new CustomAutoCompleteTextViewAdapter3(ctx, Android.Resource.Layout.SimpleSpinnerItem, autoCompleteOptions);
                        //autoTextView.Adapter = autoCompleteAdapter;
#endif

                    }
                    else
                    {
                        // using the standard adapter works without crashing - but we don't like the default filtering

                        Console.WriteLine("_use_standard_adapter_ " + panelField.Name + " [" + panelField.OptionsList.ListName + "] " + autoCompleteOptions.Length.ToString());

                        
                        var autoCompleteAdapter = new ArrayAdapter(ctx, Android.Resource.Layout.SimpleSpinnerItem, autoCompleteOptions);
                        autoTextView.Adapter = autoCompleteAdapter;
                    }


                }

#else
                    // using the standard adapter works without crashing - but we don't like the default filtering
                    //var autoCompleteAdapter = new ArrayAdapter(ctx, Android.Resource.Layout.SimpleSpinnerItem, autoCompleteOptions);
                    // autoTextView.Adapter = autoCompleteAdapter;

                    // standard function to (re)create Adapter
                    UpdateControlWithNewListPrim(autoTextView, autoCompleteOptions, EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue, EditEnumerations.SetHasBeenFocusedType.bfSetHasBeenFocusedFalse);
#endif


                    //now set the input filters, text watchers, etc based on the edit mask.
                    autoTextView.InputType = (new EditMaskManager()).DetermineEditMask(panelField.EditMask, ref autoTextView, panelField.MaxLength,
                                                                                        panelField.IntParamForceCurrDtTime, panelField.FieldType,
                                                                                        ctx);
                }


                //now hook up the events for this edit text
                autoTextView.HookupEvents(layout, panel);


                //try
                //{
                //    // AJW - get GIS info now
                //    string loGISValue = string.Empty;

                //    if (ExternalEnforcementInterfaces.GetGISEnforcementValue(autoTextView, panelField, ref loGISValue) == true)
                //    {
                //        // there should be a PrepareForEdit to do this once and not when values are being pulled from DB
                //        if (autoTextView.FormStatus == null)
                //        {
                //            autoTextView.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
                //            autoTextView.FormStatus = "Processed";
                //        }

                //        panelField.Value = loGISValue;

                //        // TODO these should be implementaed as an edit restriction ala TER_ChildList
                //        autoTextView.IgnoreEvents = true;
                //        autoTextView.SetListItemDataByValue(loGISValue);
                //        autoTextView.IgnoreEvents = false;
                //        autoTextView.HasBeenFocused = true;
                //    }
                //}
                //catch (Exception exp)
                //{
                //    LoggingManager.LogApplicationError(exp, "FieldName: " + panelField.Name, "MakeAutoTextView");
                //    Console.WriteLine("Exception caught in process: {0} {1}", exp, panelField.Name);
                //}





                // AJW - we do this for standard EditText, why not for AutoText?
                if (autoTextView.BehaviorAndroid.PanelField.Value != panelField.Value)
                {
                    autoTextView.Text = panelField.Value;
                }

                // AJW - for review - determine what the intention was here.... why are we juggling multiple fields carrying the same value??
                //todo - caleb, you were testing this to make sure seq ids displayed correctly.
                if (autoTextView.BehaviorAndroid.PanelField.Value == panelField.Value && autoTextView.Text != panelField.Value)
                {
                    autoTextView.Text = panelField.Value;
                }


                // this is done in PrepareForEditAndroid now
                //if (autoTextView.FormStatus == null)
                //{
                //    autoTextView.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
                //    autoTextView.FormStatus = "Processed";
                //}

                // self reference
                panelField.uiComponent = autoTextView;

                return autoTextView;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception source: {0}", e.Source);
                LoggingManager.LogApplicationError(e, "MakeAutoTextView Exception", e.TargetSite.Name);
                ErrorHandling.ReportExceptionWithConfirmationDlg(e.Message);
                return null;
            }
        }



        public static CustomSpinner MakeSpinnerSelector(string panelFieldName, PanelField panelField, Context context, DisplayMetrics metrics, LinearLayout layout, Panel panel, IssStruct issStruct)
        {

            //go find the custom auto text view that has a behavior assigned to it.
            var spinner = DroidContext.XmlCfg.CustomSpinners.FirstOrDefault(x => x.CustomId == issStruct.Name + panelFieldName);
            if (spinner == null)
            {
                return null;
            }

            if (panelFieldName.Equals(AutoISSUE.DBConstants.sqlVehLicStateStr) == true)
            {
                if (spinner.Adapter == null)
                {
                    spinner.Adapter = null;             // debug breakpoint
                }

            }


            // we only need to set the datasource if we didn't already do it - re-assigning it on an existing field resets the current values
            // this is also true of other event handlers (?)
            if (spinner.Adapter == null)
            {
                spinner.SetDataSource(context);
            }


            // Width as a percentage of the screen
            if (metrics != null)
            {
                spinner.SetMinimumWidth(Convert.ToInt32(metrics.WidthPixels * 1));
            }


            spinner.HookupEvents(layout, panel);


            //try
            //{
            //    // AJW - get external enforcement info info now
            //    string loGISValue = string.Empty;

            //    if (ExternalEnforcementInterfaces.GetGISEnforcementValue(spinner, panelField, ref loGISValue) == true)
            //    {
            //        // TODO there should be a PrepareForEdit to do this once and not when values are being pulled from DB
            //        if (spinner.FormStatus == null)
            //        {
            //            spinner.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
            //            spinner.FormStatus = "Processed";
            //        }

            //        panelField.Value = loGISValue;

            //        // TODO these should be implementaed as an edit restriction ala TER_ChildList
            //        if (string.IsNullOrEmpty(loGISValue))
            //        {
            //            spinner.SetListIndex(-1);
            //        }
            //        else
            //        {
            //            spinner.SetListItemDataByValue(loGISValue);
            //        }

            //        spinner.HasBeenFocused = true;
            //    }

            //}
            //catch (Exception exp)
            //{
            //    LoggingManager.LogApplicationError(exp, "FieldName: " + panelField.Name, "MakeSpinnerSelector");
            //    Console.WriteLine("Exception caught in process: {0} {1}", exp, panelField.Name);
            //}


            // AJW - we do this for standard EditText, why not for spinners?
            if (spinner.BehaviorAndroid.PanelField.Value != panelField.Value)
            {
                spinner.Text = panelField.Value;
            }
            // AJW - for review - determine what the intention was here
            //todo - caleb, you were testing this to make sure seq ids displayed correctly.
            if (spinner.BehaviorAndroid.PanelField.Value == panelField.Value && spinner.Text != panelField.Value)
            {
                // AJW - this is reqiured because the spinner text could be "list item : list item description", but we only want the "list item" (?)
                spinner.Text = panelField.Value;
            }



            // this is done in PrepareForEditAndroid 
            //if (spinner.FormStatus == null)
            //{
            //    spinner.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
            //    spinner.FormStatus = "Processed";
            //}


            // self reference
            panelField.uiComponent = spinner;

            return spinner;
        }

        public static String ConcatColumns(IEnumerable<string> cols)
        {
            //const string sep = "||' : '||";
           const string sep = "||' " + Constants.LIST_ITEM_DESCRIPTION_SEPARATOR + " '||";

            var columns = new System.Text.StringBuilder();

            foreach (var col in cols)
            {
                columns.Append("[" + col + "]");
                columns.Append(sep);
            }

            // defend against empty list definitions
            if (columns.Length > 0)
            {
                // Remove the last separator
                columns.Remove(columns.Length - sep.Length, sep.Length);
            }
            else
            {
                // do nothing, return the empty string
              
            }

            return columns.ToString();
        }

        //Make EditTextBox
        public static CustomEditText MakeEditTextBox(PanelField panelField, Context ctx, DisplayMetrics metrics, LinearLayout layout, Panel panel, IssStruct issStruct)
        //public static View MakeEditTextBox(PanelField panelField, Context ctx, DisplayMetrics metrics, LinearLayout layout, Panel panel, IssStruct issStruct)
        {
            //var editText = DroidContext.XmlCfg.CustomEditTexts.FirstOrDefault(x => x.CustomId == issStruct.Name + panelField.Name);
            CustomEditText editText = DroidContext.XmlCfg.CustomEditTexts.FirstOrDefault(x => x.CustomId == issStruct.Name + panelField.Name);

            if (editText.BehaviorAndroid.PanelField.Value != panelField.Value)
            {
                editText.Text = panelField.Value;
            }

            editText.SetHighlightColor(Android.Graphics.Color.BlueViolet);


            //try
            //{
            //    // AJW - get GIS info now
            //    string loGISValue = string.Empty;


            //    if (ExternalEnforcementInterfaces.GetGISEnforcementValue(editText, panelField, ref loGISValue) == true)
            //    {
            //        // TODO there should be a PrepareForEdit to do this once and not when values are being pulled from DB
            //        if (editText.FormStatus == null)
            //        {
            //            editText.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
            //            editText.FormStatus = "Processed";
            //        }

            //        panelField.Value = loGISValue;

            //        // TODO these should be implementaed as an edit restriction ala TER_ChildList
            //        editText.Text = loGISValue;
            //        editText.HasBeenFocused = true;
            //    }
            //}
            //catch (Exception exp)
            //{
            //    LoggingManager.LogApplicationError(exp, "FieldName: " + panelField.Name, "MakeEditTextBox");
            //    Console.WriteLine("Exception caught in process: {0} {1}", exp, panelField.Name);
            //}

            // AJW - for review - determine what the intention was here
            //todo - caleb, you were testing this to make sure seq ids displayed correctly.
            if (editText.BehaviorAndroid.PanelField.Value == panelField.Value && editText.Text != panelField.Value)
            {
                editText.Text = panelField.Value;
            }


            // editText.SetBackgroundColor(Color.White);
            // editText.SetTextColor(Color.Black);
            //if (metrics != null)
            //{
            //    editText.SetWidth(Convert.ToInt32(metrics.WidthPixels * panelField.Width));

            //}

            //now set the input filters, text watchers, etc based on the edit mask.
            editText.InputType = (new EditMaskManager()).DetermineEditMask( panelField.EditMask, ref editText, panelField.MaxLength, 
                                                                            panelField.IntParamForceCurrDtTime, panelField.FieldType, 
                                                                            ctx );

            // always want the next on the keyboard
            //editText.ImeOptions = Android.Views.InputMethods.ImeAction.Next;
            editText.ImeOptions = Android.Views.InputMethods.ImeAction.None;


            // AJW - the start of a much more sophisticated 

            // these visual property updates have to come AFTER the inputyype is set, otherwise they don't stick
            if (metrics != null)
            {
                editText.SetWidth(Convert.ToInt32(metrics.WidthPixels * panelField.Width));

                if (panelField.fEditFieldDef is Reino.ClientConfig.TTMemo)
                {
                    // just to test the effectiveness
                    //editText.SetWidth(Convert.ToInt32(metrics.WidthPixels * panelField.Width) / 2 );


                    // AJW - todo - move to centralized function and calculate using display DPI
                    float loScaler = (metrics.HeightPixels / 320);
                    Int32 loScaledHeight = Convert.ToInt32(panelField.fEditFieldDef.Height * loScaler);


                    // this works 
                    //editText.SetHeight(loScaledHeight); 

                    // this also works 
                    Rect loBounds = editText.Background.Bounds;
                    //editText.Background.SetBounds(loBounds.Left, loBounds.Top, loBounds.Right, (loBounds.Top + loScaledHeight));


                    //var myparams = new LinearLayout.LayoutParams(LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT, Gravity.CENTER);


                    // this works
                    // this needs to take the defined font size into account. ie 17 pixels per line for Font8x8, N pixles per line for Font12x12
                    Int32 loMinTextLines = Convert.ToInt32(panelField.fEditFieldDef.Height / 17);
                    editText.SetMinLines(loMinTextLines);
                    editText.SetLines(loMinTextLines);

                    //editText.SetTypeface(Typeface.Serif, TypefaceStyle.BoldItalic);

                    //editText.Gravity = GravityFlags.CenterHorizontal;
                    editText.Gravity = GravityFlags.Left | GravityFlags.Top;


                    //var layoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent)
                    //{
                    //    Width = 900,
                    //    Height = 300
                    //};
                    //editText.LayoutParameters = layoutParams;


                    editText.SetHorizontallyScrolling(false);
                    editText.SetMaxLines(int.MaxValue);

                }
            }




            //now hook up the events for this edit text
            editText.HookupEvents(layout, panel);


            // this is done in PrepareForEditAndroid 
            //if (editText.FormStatus == null)
            //{
            //    editText.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
            //    editText.FormStatus = "Processed";
            //}



            // self reference
            panelField.uiComponent = editText;


            return editText;
        }



        public static CustomSignatureImageView MakeCustomSignatureImageView(PanelField panelField, Context ctx, DisplayMetrics metrics, LinearLayout layout, Panel panel, IssStruct issStruct)
        {
            
            CustomSignatureImageView sigView = new CustomSignatureImageView(ctx);
            sigView.Tag = panelField.Name;
            sigView.CustomId = (issStruct.Name + panelField.Name);



            // a signature field is represented by an edit text - find the definition to get the behavior
            //var editText = DroidContext.XmlCfg.CustomEditTexts.FirstOrDefault(x => x.CustomId == issStruct.Name + panelField.Name);
            var editText = DroidContext.XmlCfg.CustomEditTexts.FirstOrDefault(x => x.CustomId == sigView.CustomId);

            if (editText != null)
            {
                sigView.BehaviorAndroid = editText.BehaviorAndroid;
            }


            // AJW - the start of a much more sophisticated 

            // these visual property updates have to come AFTER the inputyype is set, otherwise they don't stick
            if (metrics != null)
            {
                // nothing to do for sigimageview
            }

            //now hook up the events for this edit text
            sigView.HookupEvents(layout, panel);



            if (sigView.FormStatus == null)
            {
                // this is done in PrepareForEditAndroid 
                //if (sigView.BehaviorAndroid != null)
                //{
                //    sigView.ProcessRestrictions(EditRestrictionConsts.dneFormInit);
                //    sigView.FormStatus = "Processed";
                //}
            }


            // self reference
            panelField.uiComponent = sigView;

            // display only, always un-focused
            sigView.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);


            return sigView;
        }


        public static string GetPrefix(string spinnerValue)
        {
            /* if (spinnerValue.Contains (Constants.LIST_ITEM_DESCRIPTION_SEPARATOR)) {
                int index =  spinnerValue.IndexOf(Constants.LIST_ITEM_DESCRIPTION_SEPARATOR[0]);
                string newValue = spinnerValue.Substring (0, index);
                return newValue;
            }*/
            return spinnerValue;
        }


        public static byte[] ImageToByteArray(string fileName, int reqWidth, int reqHeight)
        {
            try
            {
                Bitmap bitmap = BitmapHelpers.LoadAndResizeBitmap(fileName, reqWidth, reqHeight);
                //Bitmap bitmap = BitmapHelpers.LoadAndResizeBitmap(fileName, 240, 240);

                using (var ms = new MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                    //  WriteBitmapData(bitmap);
                    byte[] bitmapData = ms.ToArray();

                    bitmap.Recycle();

                    // added in 7.25.06 - does it actually help? need a mem profile before/after
                    bitmap.Dispose();
                    bitmap = null;
                    GC.Collect();


                    return bitmapData;
                }
            }
            catch (Exception exception)
            {
                LoggingManager.LogApplicationError(exception, "FileName: " + fileName, "ImageToByteArray");
                Console.WriteLine("Exception caught in process: {0}", exception);
            }

            return null;

        }

        public static byte[] FileToByteArray(string fileName)
        {
            byte[] buffer = null;
            try
            {
                // Open file for reading
                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    // attach filestream to binary reader
                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        // get total byte length of the file
                        long totalBytes = new FileInfo(fileName).Length;

                        // read entire file into buffer
                        buffer = binaryReader.ReadBytes((Int32)totalBytes);

                        // close file reader
                        fileStream.Close();
                        fileStream.Dispose();
                        binaryReader.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                LoggingManager.LogApplicationError(exception, "FileName: " + fileName, "FileToByteArray");
                Console.WriteLine("Exception caught in process: {0}", exception);
            }
            return buffer;
        }


        // TODO - add an user facing dialog for failures
        public static void LoadBitmapToCustomSignatureImageView(CustomSignatureImageView sigImg, String fileName)
        {
            try
            {
                var mediaDir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), Constants.MULTIMEDIA_FOLDERNAME);
                Bitmap sigBmp = BitmapFactory.DecodeFile(mediaDir + "/" + fileName + Constants.SIG_FILE_SUFFIX);
                sigImg.SetImageBitmap(sigBmp);
                sigImg.SetBackgroundColor(Color.DarkGray);
            }
            catch (Exception exception)
            {
                LoggingManager.LogApplicationError(exception, "FileName: " + fileName, "LoadBitmapToCustomSignatureImageView");
                Console.WriteLine("Exception caught in process: {0}", exception);
            }

        }

        public static Bitmap BuildCustomSignatureBitmap(String fileName)
        {
            Bitmap sigBmp = null;

            try
            {
                var mediaDir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), Constants.MULTIMEDIA_FOLDERNAME);
                sigBmp = BitmapFactory.DecodeFile(mediaDir + "/" + fileName + Constants.SIG_FILE_SUFFIX);
            }
            catch (Exception exception)
            {
                LoggingManager.LogApplicationError(exception, "FileName: " + fileName, "BuildCustomSignatureBitmap");
                Console.WriteLine("Exception caught in process: {0}", exception);
            }

            return sigBmp;
        }


        public static ImageView GetTIssueFormBitmapImageFromStorage(ImageView sigImg, String fileName) // ticket image name YYYY MM DD iSSUE_NO_DISPLAY
        {
            try
            {
                var mediaDir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), Constants.MULTIMEDIA_FOLDERNAME);
                //Bitmap sigBmp = BitmapFactory.DecodeFile(mediaDir + "/" + fileName + Constants.TICKET_REPRODUCTION_FILE_SUFFIX);
                Bitmap sigBmp = BitmapFactory.DecodeFile(mediaDir + "/" + fileName);
                sigImg.SetImageBitmap(sigBmp);
                sigImg.SetBackgroundColor(Color.DarkGray);
            }
            catch (Exception exception)
            {
                LoggingManager.LogApplicationError(exception, "FileName: " + fileName, "GetTIssueFormBitmapImageFromStorage");
                Console.WriteLine("Exception caught in process: {0}", exception);
            }



            return sigImg;
        }

        /// <summary>
        /// Return a standardized filename for ticket image file
        /// TODO should utilize strucname YYYY MM DD iSSUE_NO_DISPLAY
        /// </summary>
        /// <param name="iStructName"></param>
        /// <param name="issueNum"></param>
        /// <param name="issueDate"></param>
        /// <returns></returns>
        public static string GetTIssueFormBitmapImageFileNameOnly(string iStructName, string issueNum, DateTime issueDate)
        {
            return iStructName + " " + issueNum + Constants.TICKET_REPRODUCTION_FILE_SUFFIX; // should utilize strucname YYYY MM DD iSSUE_NO_DISPLAY
        }

        public static string GetTIssueFormPCLPrintJobFileNameOnly(string iStructName, string issueNum, DateTime issueDate)
        {
            return iStructName + " " + issueNum + Constants.PCLCMD_FILE_SUFFIX; // should utilize strucname YYYY MM DD iSSUE_NO_DISPLAY
        }


        public static void SaveTIssueFormPrintJobDataStorage(Bitmap iTicketImage, List<PCLPrintingClass.PCLStringRow> ticketPCLCommands, string iStructName, string issueNum, DateTime issueDate)
        {
            string loFileName = GetTIssueFormBitmapImageFileNameOnly(iStructName, issueNum, issueDate);
            try
            {
                var photoDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures) + "/" + Constants.MULTIMEDIA_FOLDERNAME;
                if (!Directory.Exists(photoDirectory))
                {
                    Directory.CreateDirectory(photoDirectory);
                }


                Java.IO.File MediaDir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), Constants.MULTIMEDIA_FOLDERNAME);
                Java.IO.File ticketImageFile = new Java.IO.File(MediaDir, loFileName);


                // save the image to the device
                using (FileStream fileStream = System.IO.File.OpenWrite(ticketImageFile.ToString()))
                {
                    iTicketImage.Compress(Bitmap.CompressFormat.Png, 0, fileStream);
                }


                // are there PCL commands also?
                if (ticketPCLCommands != null)
                {
                    string loPCLPrintJobFileName = GetTIssueFormPCLPrintJobFileNameOnly(iStructName, issueNum, issueDate);

                    SaveTIssueFormPrintJobPCLCommandFile(loPCLPrintJobFileName, ticketPCLCommands);
                }
            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "FileName: " + loFileName, "SaveTIssueFormPrintJobDataStorage");
                Console.WriteLine("SaveTIssueFormPrintJobDataStorage Exception: {0}", exp);
            }


        }


        public static void SaveTIssueFormPrintJobPCLCommandFile(string iPCLPrintJobFileName, List<PCLPrintingClass.PCLStringRow> ticketPCLCommands)
        {
            try
            {
                var photoDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures) + "/" + Constants.MULTIMEDIA_FOLDERNAME;
                if (!Directory.Exists(photoDirectory))
                {
                    Directory.CreateDirectory(photoDirectory);
                }

                Java.IO.File MediaDir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), Constants.MULTIMEDIA_FOLDERNAME);
                Java.IO.File loPCLPrintJobFileName_java = new Java.IO.File(MediaDir, iPCLPrintJobFileName);
                string loPCLPrintJobAbsolutePath = loPCLPrintJobFileName_java.AbsolutePath;

                using (var os = new StreamWriter(loPCLPrintJobAbsolutePath, false))  // will overwrite every time, or do we need to delete old first?
                {
                    foreach (PCLPrintingClass.PCLStringRow loPCLObj in ticketPCLCommands)
                    {
                        os.WriteLine(loPCLObj.font.ToString());
                        os.WriteLine(loPCLObj.fontStyleFlags.ToString());
                        os.WriteLine(loPCLObj.strText);
                        os.WriteLine(loPCLObj.forwardFeedVal.ToString());
                    }
                    os.Close();
                }

            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "FileName: " + iPCLPrintJobFileName, "SaveTIssueFormPrintJobPCLCommandFile");
                Console.WriteLine("SaveTIssueFormPrintJobPCLCommandFile Exception: {0}", exp);
            }



        }


        /// <summary>
        /// Safe method to retrieve value from datarow
        /// </summary>
        /// <param name="iColumnName"></param>
        /// <returns></returns>
        public static string GetSafeColumnStringValueFromDataRow(DataRow iSourceRow, string iColumnName)
        {
            string loResultStr = string.Empty;
            try
            {
                if (iSourceRow.Table.Columns.IndexOf(iColumnName) != -1)
                {
                    if (iSourceRow[iColumnName] != null)
                    {
                        loResultStr = iSourceRow[iColumnName].ToString();
                    }
                }
            }
            catch (Exception exp)
            {
                //string loErrMsg = "Error: GetValueFromDataRow " + "tableName: " + iSourceRow.Table.TableName;
                LoggingManager.LogApplicationError(exp, "tableName: " + iSourceRow.Table.TableName, "GetValueFromDataRow");
                Console.WriteLine("Exception source: {0}", exp.Source);
            }

            // suppress spinner values - ultimately these shoud NOT be written to the DB
            //if (loResultStr.Equals(Constants.SPINNER_DEFAULT) == true)
            //{
            //    loResultStr = "";
            //}


            return loResultStr;
        }


        public static string SafeGetAssociatePromptWinLabel(View iSourceView)
        {
            string loResultStr = string.Empty;

            if (iSourceView is CustomEditText)
            {
                CustomEditText loCustomEditText = (CustomEditText)iSourceView;

                //  let us see if there is a label we can use
                if (loCustomEditText.BehaviorAndroid != null)
                {
                    if (loCustomEditText.BehaviorAndroid.CfgCtrl != null)
                    {
                        if (loCustomEditText.BehaviorAndroid.CfgCtrl.PromptWin != null)
                        {
                            if (loCustomEditText.BehaviorAndroid.CfgCtrl.PromptWin.TextBuf != null)
                            {
                                loResultStr = loCustomEditText.BehaviorAndroid.CfgCtrl.PromptWin.TextBuf;
                            }
                        }
                    }
                }

                return loResultStr;
            }


            if (iSourceView is CustomAutoTextView)
            {
                CustomAutoTextView loCustomAutoTextView = (CustomAutoTextView)iSourceView;

                //  let us see if there is a label we can use
                if (loCustomAutoTextView.BehaviorAndroid != null)
                {
                    if (loCustomAutoTextView.BehaviorAndroid.CfgCtrl != null)
                    {
                        if (loCustomAutoTextView.BehaviorAndroid.CfgCtrl.PromptWin != null)
                        {
                            if (loCustomAutoTextView.BehaviorAndroid.CfgCtrl.PromptWin.TextBuf != null)
                            {
                                loResultStr = loCustomAutoTextView.BehaviorAndroid.CfgCtrl.PromptWin.TextBuf;
                            }
                        }
                    }
                }

                return loResultStr;
            }


            if (iSourceView is CustomSpinner)
            {
                CustomSpinner loCustomSpinner = (CustomSpinner)iSourceView;

                //  let us see if there is a label we can use
                if (loCustomSpinner.BehaviorAndroid != null)
                {
                    if (loCustomSpinner.BehaviorAndroid.CfgCtrl != null)
                    {
                        if (loCustomSpinner.BehaviorAndroid.CfgCtrl.PromptWin != null)
                        {
                            if (loCustomSpinner.BehaviorAndroid.CfgCtrl.PromptWin.TextBuf != null)
                            {
                                loResultStr = loCustomSpinner.BehaviorAndroid.CfgCtrl.PromptWin.TextBuf;
                            }
                        }
                    }
                }

                return loResultStr;
            }



            return loResultStr;
        }




        public static string GetSelectedPrinterType()
        {
            return Constants.PRINTER_TYPE;
        }

        // return a AI compatible serial number for this device
        // AJW TODO - create a seperate class for device detection and format initialization
        public static string GetDeviceUniqueSerialNumber()
        {

            // only once
            if (Constants.SERIAL_NUMBER.Length > 0)
            {
                return Constants.SERIAL_NUMBER;
            }

            // if we can't get the build info, we'll have nothing
            if (Build.Model != null)
            {
                // get and keep the OS info
                Constants.deviceModelBuildInfoFromOS = Build.Model.Trim().ToUpper();
            }

            string deviceSerial = Constants.deviceModelName_UnknownSerialNumber;
            if (Build.Serial != null)
            {
                deviceSerial = Build.Serial.Trim().ToUpper();
            }


            // TODO - research an utilize the Android hased SN info
            StringBuilder deviceFormattedSerialNumber = new StringBuilder(deviceSerial);

            // make sure its at least N digits
            while (deviceFormattedSerialNumber.Length < Constants.deviceSerialNumberDigitsLength)
            {
                deviceFormattedSerialNumber.Append("0");
            }


            // and make sure its not longer than N
            if (deviceFormattedSerialNumber.Length > Constants.deviceSerialNumberDigitsLength)
            {
                deviceFormattedSerialNumber = deviceFormattedSerialNumber.Remove(0, (deviceFormattedSerialNumber.Length - Constants.deviceSerialNumberDigitsLength));
            }


            // determine the device category, starting with the OS info
            Constants.deviceModelAsCategorized = Constants.deviceModelCategorized_UnknownDevice;

            // select an appropriate prefix
            string loDeviceModelPrefix = Constants.deviceModelPrefix_UnknownDevice;

//#if _force_S5_
            if (Constants.deviceModelBuildInfoFromOS.StartsWith(Constants.deviceModelNameStartsWith_SamsungGalaxyS5) == true)
            {
                Constants.deviceModelAsCategorized = Constants.deviceModelCategorized_SamsumgGalaxyS5;
                loDeviceModelPrefix = Constants.deviceModelPrefix_SamsumgGalaxyS5;
            }

                // Samsung Galaxy Note 5
            else if (Constants.deviceModelBuildInfoFromOS.StartsWith(Constants.deviceModelNameStartsWith_SamsungGalaxyNote5) == true)
            {
                Constants.deviceModelAsCategorized = Constants.deviceModelPrefix_SamsumgGalaxyNote5;
                loDeviceModelPrefix = Constants.deviceModelPrefix_SamsumgGalaxyNote5;
            }

                // 2T Class is a Galaxy Note 3 in a sled - need better identfiers of this platform
            else if (Constants.deviceModelBuildInfoFromOS.StartsWith(Constants.deviceModelNameStartsWith_TwoTechnologiesN5Class) == true)
            {
                Constants.deviceModelAsCategorized = Constants.deviceModelPrefix_SamsumgGalaxyNote5;
                loDeviceModelPrefix = Constants.deviceModelPrefix_TwoTechnologiesN5Class;
            }

                // 2T Class is a Galaxy Note 3 from AT&T comes back as "SAMSUNG-SM-900A", so we will use "contains"
            else if (Constants.deviceModelBuildInfoFromOS.Contains(Constants.deviceModelNameContains_TwoTechnologiesN5Class) == true)
            {
                Constants.deviceModelAsCategorized = Constants.deviceModelPrefix_SamsumgGalaxyNote5;
                loDeviceModelPrefix = Constants.deviceModelPrefix_TwoTechnologiesN5Class;
            }


                // AJW - for later review - force unknown devices to be treated as Galaxy S5
            else
//#endif
            {
                Constants.deviceModelAsCategorized = Constants.deviceModelCategorized_SamsumgGalaxyS5;
                loDeviceModelPrefix = Constants.deviceModelPrefix_SamsumgGalaxyS5;
            }


            deviceFormattedSerialNumber.Insert(0, loDeviceModelPrefix + "-");

            Constants.SERIAL_NUMBER = deviceFormattedSerialNumber.ToString();

            return Constants.SERIAL_NUMBER;
        }


       
        public static string GetSerialNumber(string serialNumber)
        {
            return GetDeviceUniqueSerialNumber();
        }



        public static string SerializeObject(object objectToSerialize)
        {
            // Will return blank string if serialize fails.
            string objectAsString = "";

            try
            {
                // Serialize the object into a string.
                var writer = new StringWriter();
                var serializer = new XmlSerializer(objectToSerialize.GetType());
                serializer.Serialize(writer, objectToSerialize);
                objectAsString = writer.ToString();
                writer.Close();
            }
            catch (Exception ex)
            {
                // Re-Throw the error
                throw (ex);
            }

            // Return serialized string.
            return objectAsString;
        }

        public static object DeSerializeObject(string objectAsString, Type objectType)
        {
            // This is the object we will return (null if fail).
            object objectToReturn = null;

            try
            {
                // Deserialize the object.
                var reader = new StringReader(objectAsString);
                var serializer = new XmlSerializer(objectType);
                objectToReturn = serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
                // Re-Throw the error
                throw (ex);
            }

            // Return object we created.
            return objectToReturn;
        }

        public static int SafeGetFileInfoDataSize(FileInformation iFileInfo)
        {
            try
            {
                return iFileInfo.Data.Length;
            }
            catch
            {
                return 0;
            }
        }

        public static bool IsCurrentDeviceIsN5Class()
        {
            string deviceId = Build.Serial;
            string loDeviceId = Helper.GetSerialNumber(deviceId);
            if (loDeviceId.Contains(Duncan.AI.Constants.deviceModelPrefix_TwoTechnologiesN5Class))
            {
                return true;
            }
            return false;
        }

        public static bool UseBuiltInCameraWithLPR()
        {

            int loRegValue = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                         TTRegistry.regLPR_TEST_USE_BUILTIN_CAMERA,
                                                                         TTRegistry.regLPR_TEST_USE_BUILTIN_CAMERA_DEFAULT);
            if (loRegValue > 0)
            {
                return true;
            }
            return false;
        }
    }

    public static class BitmapHelpers
    {
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            var options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
                inSampleSize = outWidth > outHeight ? outHeight / height : outWidth / width;

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }

        public static Bitmap LoadAndResizeBitmapFixed(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            var options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
                inSampleSize = outWidth > outHeight ? outHeight / height : outWidth / width;

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;

            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }

        public static Bitmap RotateBitmap(Android.Graphics.Bitmap source, int angle)
        {            
            Bitmap rotatedBitmap = null;
            Matrix matrix = null;
            try
            {
                matrix = new Matrix();
                if (matrix == null) return null;
                matrix.PostRotate(angle);
                rotatedBitmap = Bitmap.CreateBitmap(source, 0, 0, source.Width, source.Height, matrix, false);
                matrix.Dispose();
                return rotatedBitmap;
            }catch (Exception ex)
            {
                if (rotatedBitmap != null)
                {
                    rotatedBitmap.Recycle();
                    rotatedBitmap.Dispose();
                    rotatedBitmap = null;
                }
                if (matrix != null) matrix.Dispose();
                System.Console.WriteLine("Exception source {0}: {1}", ex.Source, ex.ToString());
                return null;
            }             
        }

        public static Bitmap RotateBitmapFromFile(string iFileName, int angle)
        {
            try
            {
                Android.Graphics.BitmapFactory.Options loOptions = new Android.Graphics.BitmapFactory.Options();
                loOptions.InMutable = true;
                Bitmap loSource = Android.Graphics.BitmapFactory.DecodeFile(iFileName, loOptions);
                return RotateBitmap(loSource, angle);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception source {0}: {1}", ex.Source, ex.ToString());
                return null;
            }
        }

        public static Bitmap decodeSampledBitmapFromFile(String path, int reqWidth, int reqHeight)
        { // BEST QUALITY MATCH

            // First decode with inJustDecodeBounds=true to check dimensions
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            BitmapFactory.DecodeFile(path, options);

            // Calculate inSampleSize
            // Raw height and width of image
            int height = options.OutHeight;
            int width = options.OutWidth;
            options.InPreferredConfig = Bitmap.Config.Rgb565;
            int inSampleSize = 1;

            if (height > reqHeight)
            {
                inSampleSize = (int)Math.Round((float)height / (float)reqHeight);
            }

            int expectedWidth = width / inSampleSize;

            if (expectedWidth > reqWidth)
            {
                //if(Math.round((float)width / (float)reqWidth) > inSampleSize) // If bigger SampSize..
                inSampleSize = (int)Math.Round((float)width / (float)reqWidth);
            }


            options.InSampleSize = inSampleSize;

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;

            return BitmapFactory.DecodeFile(path, options);
        }

        public static byte[] GetBitmapData(Bitmap bitmap)
        {
            int _compressRatio = 100;
            try
            {
                byte[] bitmapData;
                using (var stream = new System.IO.MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, _compressRatio, stream);
                    bitmapData = stream.ToArray();
                }
                return bitmapData;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception source {0}: {1}", ex.Source, ex.ToString());
                return null;
            }
        }

    }
}

