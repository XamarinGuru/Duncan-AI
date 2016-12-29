using System;
using System.IO;

namespace Duncan.AI
{

    #region VersionHistory
    // Version history
    //
    // See JIRA for detailed revision history
    //
    //
    // 7.12 2016-01-10
    //
    // - Corrected print picture font rendering
    // - Implemented printed text justification

    // - lots more... See JIRA 
    //
    // 7.11 2016-01-05
    //
    // - Corrected print picture font rendering
    // - Implemented printed text justification
    // - Integrated server based space enforcement - map and list webviews
    // - Numerous smaller issues corrected
    //
    // 7.10 2015-12-30
    //
    // - Merged map-based enforcement module
    // - Force all devices to be HW evaluated as Samsung Galaxy S5 
    //      - Successfully tested application on Samgung Galaxy Nexus, Amazon Fire Kindle HD
    //
    // 7.08 2015-12-22
    //  - 
    //
    // 7.07 2015-11-30
    //  - Fixed major bugs that resulted in application crash in modulous check digit calculation functions and sequence manager 
    //  - Implemented proper device serialization assignment for specific Android platforms (Samsung Galaxy S5 to start, others to follow new models)
    //  - Implemented TGenericStructType for MeterStatus form support
    //  - Rebranded to CivicSmart
    //  - Revision Numbering Scheme to 7.x changed to avoid future confusion with Duncan-Aus releases
    //
    // 4.15 2015-08-30
    //  - Initial Version Recieved from Duncan Australia




       

    #endregion


    public class Constants
    {
        public const string DBName = "civicsmart.db";
        public const string username = "username";
        public const string OFFICER_ID = "OFFICERID";
        public const string OFFICER_NAME = "OFFICERNAME";
        public const string AGENCY = "AGENCY";
        public const string SUBAGENCY = "SUBAGENCY";
        //public const string DestFilePath = "/Issue_Ap.xml";
        public const string DestTempFilePath = "/Temp.DAT";

        public const string SPINNER_DEFAULT = "(SELECT)";
        public const string SPINNER_DEFAULT_AS_UPPER = "(SELECT)";


        public const string appsettings_property_name_bootmode = "bootmode";
        public const string appsettings_bootmode_value_logon = "logon";
        public const string appsettings_bootmode_value_resync = "resync";
        public const string appsettings_bootmode_value_restart = "restart";

        public const string cnMenuPopup_UserAccountTitleText = "Profile";
        public const string cnMenuPopup_IssueFormSelectTitleText = "Action";

        public const string cnMenuPopup_ExitText = "EXIT";



        public const string cnUserCommentPopupVoidReason_TitleText = "Void Record";
        public const string cnUserCommentPopupReissueReason_TitleText = "Void and Reissue Record";

        public const string cnUserCommentPopupNotationText_TitleText = "Add Text Note";





        //public const string STRUCT_TYPE_ISSUE_PARKING_TICKET = "TCiteStruct";
        //public const string STRUCT_NAME_ISSUE_PARKING_TICKET = "PARKING";

        public const string STRUCT_TYPE_ACTIVITYLOG = "TActivityLogStruct";
        public const string STRUCT_NAME_ACTIVITYLOG = "ACTIVITYLOG";

        public const string STRUCT_TYPE_CHALKING = "TMarkModeStruct";
        public const string STRUCT_NAME_CHALKING = "MARKMODE";

        public const string STARTDATE_ACTIVITYLOG = "STARTDATE";
        public const string STARTTIME_ACTIVITYLOG = "STARTTIME";
        public const string ENDDATE_ACTIVITYLOG = "ENDDATE";
        public const string ENDTIME_ACTIVITYLOG = "ENDTIME";
        public const string STATUS_COLUMN = "STATUS";
        public const string DEVICEID = "deviceId";
        public const string ID_COLUMN = "_id";
        public const string IS_GPS = "isGPS";

        public const string LATITUDE_ACTIVITYLOG = "START_LOCLATITUDE";
        public const string LONGTITUDE_ACTIVITYLOG = "START_LOCLONGITUDE";
       
        //public const string PARKING_SEQUENCE = "PARKING";
        //public const string PARKING_TABLE = "PARKING";
        public const string SEQUENCE_ID = "SEQUENCEID";
       
        public const string WS_STATUS_COLUMN = "WSSTATUS";

        //public const string PARKING_SEQUENCE_TABLE = "PARKING_SEQUENCE";
        public const string SRCINUSE_FLAG = "INUSEFLAG";
        public const string SRCINUSE_FLAG_NOTUSED = "0";
        public const string SRCINUSE_FLAG_USED = "1";
        public const string SRCINUSE_FLAG_SUBMITTED = "2";

        public const string STATUS_INPROCESS = "INPROCESS";
        public const string STATUS_READY = "READY";
        public const string STATUS_ISSUED = "ISSUED";
        public const string STATUS_VOIDED = "VOIDED";
        public const string STATUS_REISSUE = "REISSUE";

        public const string WS_STATUS_EMPTY = "EMPTY";
        public const string WS_STATUS_READY = "READY";
        public const string WS_STATUS_SUCCESS = "SUCCESS";
        public const string WS_STATUS_FAILED = "FAILED";

        // datetime fields in the SQLite database will always be written/read back in these formats
        public const string DB_SQLITE_DATEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK = "yyyyMMdd";

        // AJW - TODO clean up these inconsitent references, TIME_HHMMSS consflict 
        //public const string DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK = "HH:mm:ss";
        public const string DB_SQLITE_TIMEONLY_TYPE_FIXED_STORAGE_FORMAT_MASK = "HHmmss";

        public const string DB_SQLITE_DATETIME_TYPE_FIXED_STORAGE_FORMAT_MASK = "yyyyMMdd HH:mm:ss";


        public const string DT_MM_DD_YYYY = "MM/dd/yyyy";
        public const string DT_YYYYMMDD = "yyyyMMdd";
        public const string DT_YYYY_MM_DDT = "yyyy-MM-ddT00:00:00";

        //public const string TIME_HH_MM_SS_TT = "hh:mm:ss tt";
        public const string TIME_HHMMSS = "HHmmss";
        //public const string TIME_DT_HH_MM_SS = "1899-12-30Thh:mm:ss";
        public const string TIME_H_MM_TT = "h:mm tt";

        // the character to use between list items values and their description text
        //public const string LIST_ITEM_DESCRIPTION_SEPARATOR = "|";
        //public const string LIST_ITEM_DESCRIPTION_SEPARATOR = "║";
        //public const string LIST_ITEM_DESCRIPTION_SEPARATOR = "╟";
        //public const string LIST_ITEM_DESCRIPTION_SEPARATOR = "─"; // looks like a dash, but is actually ASCII 196 which won't be in common lists
        //public const string LIST_ITEM_DESCRIPTION_SEPARATOR = "º"; // 
        //public const string LIST_ITEM_DESCRIPTION_SEPARATOR = "∞"; // 
        public const string LIST_ITEM_DESCRIPTION_SEPARATOR = "·"; // looks like dot, but is actually ASCII 250
        //public const string LIST_ITEM_DESCRIPTION_SEPARATOR = "≡"; //  ASCII 240




        // AJW - todo - these duplicated column names to be deprecated and replaced with existing AutoISSUE.DBConstant references
        public const string ISSUENO_COLUMN = "ISSUENO";
        public const string ISSUETIME_COLUMN = "ISSUETIME";
        public const string ISSUEDATE_COLUMN = "ISSUEDATE";

        public const string REVISION_PARKING = "revisionParking";

        public const string WHERE_STATUS_READY = " WHERE " + Constants.STATUS_COLUMN + " = '" + Constants.STATUS_READY + "'";



        //public const string TICKETS_FRAGMENT_TAG = "ticketsFragment";  // deprecated...

        public const string ISSUE_NEW_FRAGMENT_TAG_PREFIX = "issueNew_"; // add struct name to end for tag name
        public const string ISSUE_NEW2_FRAGMENT_TAG_PREFIX = "issueNew2_"; // add struct name to end for tag name
        public const string ISSUE_SELECT_FRAGMENT_TAG_PREFIX = "issueSelect_";  // add struct name to end for tag name
        public const string ISSUE_REVIEW_FRAGMENT_TAG_PREFIX = "issueReview_";  // add struct name to end for tag name

        public const string ISSUE_NOTES_FRAGMENT_TAG_PREFIX = "issueNotes_";     // older, to be removed
        public const string ISSUE_NOTES_DETAIL_FRAGMENT_TAG_PREFIX = "issueNotesDetail_";

        public const string NOTES_REVIEW_SELECT_FRAGMENT_TAG_PREFIX = "issueNotesReviewSelect_";
        public const string NOTES_REVIEW_DETAIL_FRAGMENT_TAG_PREFIX = "issueNotesReviewDetail_";


        public const string ISSUE_VOID_FRAGMENT_TAG_PREFIX = "issueVoid_";
        public const string ISSUE_REISSUE_FRAGMENT_TAG_PREFIX = "issueReissue_";
        public const string SEARCH_MATCH_FRAGMENT_TAG_PREFIX = "searchMatch_";


        public const string GIS_MAP_FRAGMENT_TAG = "GISMapFragment";
        public const string GIS_PAYBYSPACE_LIST_FRAGMENT_TAG = "GISPayBySpaceListFragment";
        public const string GIS_PAYBYPLATE_LIST_FRAGMENT_TAG = "GISPayByPlateListFragment";

        public const string WEBVIEW_CLIENT_DEFINABLE_FRAGMENT_TAG_PREFIX = "webview_client_defn_"; // add index number to end for tag name

        public const string WebView_Client_Definable_Index_Parameter_Name = "webview_client_index"; // bundle parameters

        public const string ANPR_CONFIRMATION_FRAGMENT_TAG = "ANPRConfirmationFragment";
        public const string ANPR_DETAIL_FRAGMENT_TAG = "ANPRDetailFragment";


        public const string GEOCODE_LOCATION_CONFIRMATION_FRAGMENT_TAG = "GeoCodeLocationConfirmationFragment";

        public const string EXTERNAL_ENFORCEMENT_CONFIRMATION_FRAGMENT_TAG = "ExternalEnforcementConfirmationFragment";



        // AJW these are not on tabs, they are displayed as needed - so they need to be handled differently than the fragments that have tabs for selection
        //public const string ISSUE_REVIEW_SUMMARY_FRAGMENT_TAG = "issSumDetailFragment";  // being deprectated
        //public const string ISSUE_REVIEW_DETAIL_FRAGMENT_TAG = "issRevDetailFragment";  // removing
        
        //public const string VOID_FRAGMENT_TAG = "voidFragment";
        //public const string NOTES_FRAGMENT_TAG = "notesFragment";
        //public const string NOTE_DETAIL_FRAGMENT_TAG = "notesDetailFragment";




        public const int ACTIVITY_REQUEST_CODE_LPR_PROCESSING = 11;
        public const int ACTIVITY_REQUEST_CODE_CAPTURE_PHOTO_FOR_LPR = 12;
        public const int ACTIVITY_REQUEST_CODE_CAPTURE_PHOTO_PENDING_ATTACHMENT = 14;
        public const int ACTIVITY_REQUEST_CODE_HOTSHEET_RESULT = 15;
        public const int ACTIVITY_REQUEST_CODE_MENU_POPUP_NAVIGATION_RESULT = 16;
        public const int ACTIVITY_REQUEST_CODE_GEOCODE_ADDRESS = 17;
        public const int ACTIVITY_REQUEST_CODE_EXTERNAL_ENFORCMENT_CONFIRMATION = 18;





        public const string ACTIVITY_INTENT_DISPLAY_SYNC_PROGRESS_NAME = "Duncan.AI.Droid.DisplaySyncProgress";

        public const string ActivityIntentExtraInt_ProgressValue = "Progress";
        public const string ActivityIntentExtraInt_ProgressDesc = "ProgressDesc";


        public const string ACTIVITY_INTENT_HIDE_SPLASH_SCREEN_NAME = "Duncan.AI.Droid.HideSplashScreen";

        public const string ACTIVITY_INTENT_CONFIRM_BARCODE_FIELDSNAMES_KEY = "BarCodeConfirmResultFragment.FieldsNamesList";
        public const string ACTIVITY_INTENT_CONFIRM_BARCODE_FIELDSVALUES_KEY = "BarCodeConfirmResultFragment.FieldsValuesList";
        public const string ACTIVITY_INTENT_CONFIRM_BARCODE_PREVIEWLIST_KEY = "BarCodeConfirmResultFragment.BarCodeElementList";


        public enum SyncStepEnummeration
        {
            SyncStep_Idle = 0,
            SyncStep_Authenticate,
            SyncStep_StartSync,
            SyncStep_UploadFiles,
            SyncStep_RemoveLocalFiles,
            SyncStep_GetPlatformfiles,
            SyncStep_GetConfiguration,
            SyncStep_LoadPreferences,
            SyncStep_ProcessListFiles,
            SyncStep_ProcessSequenceFiles,
            SyncStep_ProcessUserStruct,
            SyncStep_Complete

        };


        public const string ISSUE_AP_XML_FILENAME = "ISSUE_AP.XML";

        public const string PREVIOUS_FRAGMENT = "PREVIOUSFRAGMENT";

        //todo caleb remove void
        public const string VOID_AITICKETID = "VoidAITicketID";

        public const string PARKVOID_TABLE = "PARKVOID";
        public const string MASTERKEY_COLUMN = "MASTERKEY";
        public const string RECCREATIONDATE_COLUMN = "RECCREATIONDATE";
        public const string RECCREATIONTIME_COLUMN = "RECCREATIONTIME";
        public const string CANCELREASON_COLUMN = "CANCELREASON";

        public const string PARKVOIDREASON = "PARKVOIDREASON";


        public const string UNIQUEKEY = "UNIQUEKEY";

        public const string PARKREISSUE_TABLE = "PARKREISSUE";
        public const string SRCISSUENO_COLUMN = "SRCISSUENO";
        public const string SRCISSUENOPFX_COLUMN = "SRCISSUENOPFX";
        public const string SRCISSUENOSFX_COLUMN = "SRCISSUENOSFX";

        public const string ISSUENOPFX_COLUMN = "ISSUENOPFX";
        public const string ISSUENOSFX_COLUMN = "ISSUENOSFX";

        public const string PARKNOTE_TABLE = "PARKNOTE";
        public const string DETAILRECNO_COLUMN = "DETAILRECNO";
        public const string NOTEDATE_COLUMN = "NOTEDATE";
        public const string NOTETIME_COLUMN = "NOTETIME";
        public const string NOTESMEMO_COLUMN = "NOTESMEMO";
        public const string DIAGRAM_COLUMN = "DIAGRAM";
        public const string MULTIMEDIANOTEDATATYPE_COLUMN = "MULTIMEDIANOTEDATATYPE";
        public const string MULTIMEDIANOTEFILENAME_COLUMN = "MULTIMEDIANOTEFILENAME";
        public const string MULTIMEDIANOTEFILEDATESTAMP_COLUMN = "MULTIMEDIANOTEFILEDATESTAMP";
        public const string MULTIMEDIANOTEFILETIMESTAMP_COLUMN = "MULTIMEDIANOTEFILETIMESTAMP";


        public const string AIWEBPROXY_DEBUG_FOLDERNAME = "AIWEBPROXY_DEBUG";

        public const string DEBUG_FILENAME_TIMESTAMP_FORMAT = "yyyyMMdd_HHmmss";

        //public const string MULTIMEDIA_FOLDERNAME = "duncan";
        public const string MULTIMEDIA_FOLDERNAME = "civicsmart";


        public const string REVISION_PARKVOID = "revisionParkVoid";
        public const string REVISION_PARKREISSUE = "revisionParkReIssue";
        public const string REVISION_PARKNOTE = "revisionParkNote";

        public const string OFFICER_SIGNATURE = "OFFICERSIGNATURE";
        public const string SIG_FILE_SUFFIX = ".bmp";
        public const string TICKET_REPRODUCTION_FILE_SUFFIX = ".bmp";
        public const string PHOTO_FILE_SUFFIX = ".jpg";
        public const string VIDEO_FILE_SUFFIX = ".mp4";
		public const string PCLCMD_FILE_SUFFIX= ".pcl";

        public const string LOCMETER_COLUMN = "LOCMETER";
        public const string LOCBLOCK_COLUMN = "LOCBLOCK";
        public const string LOCDIRECTION_COLUMN = "LOCDIRECTION";
        public const string LOCSTREET_COLUMN = "LOCSTREET";
        public const string VEHLICNO_COLUMN = "VEHLICNO";
        public const string VEHLICSTATE_COLUMN = "VEHLICSTATE";
        public const string VEHLICEXPDATE_COLUMN = "VEHLICEXPDATE";

        public const string VEHPLATETYPE_COLUMN = "VEHLICTYPE";

        // AJW TODO - this is not a valid column name
        public const string VEHYEAR_COLUMN = "VEHYEAR";

        public const string VEHVIN_COLUMN = "VEHVIN";
        public const string VEHMAKE_COLUMN = "VEHMAKE";
        public const string VIOSELECT_COLUMN = "VIOSELECT";
        public const string VIOCODE_COLUMN = "VIOCODE";
        public const string VIODESCRIPTION1_COLUMN = "VIODESCRIPTION1";
        public const string VIOFINE_COLUMN = "VIOFINE";

        public const string CHALKING_LIST_FRAGMENT_TAG = "chalkingListFragment";
        public const string MARKMODE_TABLE = "MARKMODE";
        public const string REVISION_MARKMODE = "revisionMarkMode";

        public const string MARKMODE_ROWID = "markModeRowId";
        public const string CHALK_DETAIL_FRAGMENT_TAG = "chalkDetailFragment";

        public const string BOOKNUMBER_COLUMN = "BOOKNUMBER";

        public const string INVOKE_LOGIN = "invokeLoginFile";

        public const string LOCATION_UPDATE_ACTIVITY_NAME = "LOCATION UPDATE";

        public const string TIRESTEMSFRONTTIME_COLUMN = "TIRESTEMSFRONTTIME";
        public const string TIRESTEMSREARTIME_COLUMN = "TIRESTEMSREARTIME";

        public const string CHALK_TIRE_FRAGMENT_TAG = "chalkTireFragment";

        public const string PRINTER_TYPE_NAME_BIXOLON_SPP200 = "SPP-R200";
        public const string PRINTER_TYPE_NAME_ZEBRA_iMZ320 = "Zebra_iMZ320";
        public const string PRINTER_TYPE_NAME_ZEBRA_MZ320 = "Zebra_MZ320";
        public const string PRINTER_TYPE_NAME_ZEBRA_ZQ510 = "Zebra_ZQ510";
        public const string PRINTER_TYPE_NAME_N5CLASS_PRINTER_TEXT = "TwoTech_N5_Class_Text"; //"TwoTechnologies_N5_Class";
        public const string PRINTER_TYPE_NAME_N5CLASS_PRINTER_GRAPHIC = "TwoTech_N5_Class_Graphic";


        public const string CONFIG_FILE_NAME = "appconfig.txt";

        public const string CONFIG_FILE_PARAMETER_VALUE_DELIMETER = "@";
        public const string CONFIG_FILE_PARAMETER_PUBLIC_URL = "URL";
        public const string CONFIG_FILE_PARAMETER_PRIVATE_URL = "PRIVATEURL";
        public const string CONFIG_FILE_PARAMETER_UNIT_SERIAL_NUMBER = "SERIAL";
        public const string CONFIG_FILE_PARAMETER_CLIENT_ID = "CLIENT";
        public const string CONFIG_FILE_PARAMETER_PRINTER_TYPE = "PRINTER_TYPE";
        

        public const string SESSION_KEY = "sessionKey";
        public const string ENCRYPTED_SESSION_KEY = "encryptedSessionKey";
        public const string ParkNotesWhereClause = " WHERE " + WS_STATUS_COLUMN + " = '" + WS_STATUS_READY + "'";
        public const string MasterKeyValueTemplate = "ISSUENO={0}\tISSUEDATE={1}\tISSUETIME={2}";
        //this is an error on ai host and will always be throw for re-issues and park notes. Ignoring this specific error in those two instances.
        public const string AIHostKnownAutoProcError ="A column named 'AUTOPROC_UNIQUEKEY' already belongs to this DataTable.";

        // this error can occur when a detail record is already uploaded (PARKVOID, PARKNOTE) but the handheld attempts to do it again
        // this happens when the device crashes before a DB update is committed. We need to resolve and prevent these scenarios
        public const string AIHostKnownDetailRecordError = "There is no row at position 0.";

        public const string USERSTRUCT_TABLE = "USERSTRUCT";
        public const string LOGINNAME_COLUMN = "LOGINNAME";
        public const string LOGINPWD_COLUMN = "LOGINPWD";




        public const int deviceSerialNumberDigitsLength = 5;

        public const string deviceModelName_UnknownSerialNumber = "00000";

        public const string deviceModelPrefix_UnknownDevice = "UN";
        public const string deviceModelCategorized_UnknownDevice = "UNKNOWN";

#region resource_references_icon_size

        //    ldpi : 36x36px   (120 dpi / 47 dpcm)
        //    mdpi : 48x48px   (160 dpi / 62 dpcm)
        //    hdpi : 72x72px   (240 dpi / 94 dpcm)
        //   xhdpi : 96x96px   (320 dpi / 126 dpcm)
        //  xxhdpi : 144x144px (480 dpi / 189 dpcm)
        // xxxhdpi : 192x192px (640 dpi / 252 dpcm)

        //dip : 48x48dip (dip = dp)
            
        //mm : 76 mm (depends on the real device dpi)
#endregion





        // Note 3 - 1080 x 1920 px   386ppi   480DPI   xxhdpi(?)
        public const string deviceModelNameStartsWith_TwoTechnologiesN5Class = "SM-N900"; // SM-900V   Verizon Samsung Note3 inside of sled
        public const string deviceModelNameContains_TwoTechnologiesN5Class = "SM-N900"; // SAMSUNG-SM-N900A   At&T Samsung Note3 inside of sled
        public const string deviceModelPrefix_TwoTechnologiesN5Class = "T5";
        public const string deviceModelCategorized_TwoTechnologiesN5Class = "Two Technologies N5Class";




        public const string deviceModelNameStartsWith_SamsungGalaxyNote5 = "SM-N920"; // Samsung 
        public const string deviceModelPrefix_SamsumgGalaxyNote5 = "N5";
        public const string deviceModelCategorized_SamsumgGalaxyNote5 = "Samsung Galaxy Note 5";

        // Note 4 - 1440 x 2560 px   - density 3.0 xxhdpi


        public const string deviceModelNameStartsWith_SamsungGalaxyS5 = "SM-G900"; // Samsung SM-G900V - Verizon, SM-G900R4 - US Cellular, SM-G900P - Sprint
        public const string deviceModelPrefix_SamsumgGalaxyS5 = "S5";
        public const string deviceModelCategorized_SamsumgGalaxyS5 = "Samsung Galaxy S5";
        // Galaxy S5 - 1080 x 1920 px   - density 3.0 xxhdpi

        public const string deviceModelNameStartsWith_SamsungGalaxyS6 = "SM-G928"; //SM-G928V (Verizon); SM-G928P (Sprint); SM-G928R (US Cellular)
        public const string deviceModelPrefix_SamsumgGalaxyS6 = "S5";  //"S6"; AJW TODO - treat this as S5 until we flesh out host support
        public const string deviceModelCategorized_SamsumgGalaxyS6 = "Samsung Galaxy S6";
        // Galaxy S6 - 1440 x 2560 px   - density 4.0 xxxhdpi

        // AJW TODO - values that change should be delcared elsewhere? at the very least these should be protected properties
        public static string deviceModelAsCategorized = string.Empty;
        public static string deviceModelBuildInfoFromOS = string.Empty;
        public static string SERIAL_NUMBER = string.Empty;
        public static string CLIENT_NAME = string.Empty;
        public static string PRINTER_TYPE = string.Empty;  // TODO - need a better mechanism to know when to read - for now, if this is EMPTY we will read from file on resume


        public static string gClientName = "UNKNOWN";



        //CUSTOM LABELS
        //public const string LABEL_TAG_REQUIRED = "_REQUIRED";
        public const string LABEL_OBJECT_NAME_SUFFIX = "_LABEL";


        public const string LABEL_SEQUENCE = "_SEQUENCE";
        public const string LABEL_REVISION = "_REVISION";

        public const string STRUCT_TYPE_CITE = "TCiteStruct";
        public const string STRUCT_TYPE_NOTES = "TNotesStruct";
        public const string STRUCT_TYPE_VOID = "TVoidStruct";
        public const string STRUCT_TYPE_REISSUE = "TReissueStruct";
        public const string STRUCT_TYPE_GENERIC_ISSUE = "TGenericIssueStruct";

        public const string STRUCT_NAME_TKT_DTL = "structNameTktDtl";
        public const string LABEL_TAB_POSITION = "_TabPosition";

        //public const string LABEL_TICKETS_TAB = "TICKETS";
        public const string LABEL_TICKETS_TAB = "REVIEW";

        public const string LABEL_CHALKS_TAB = "CHALKS";
        public const string PARKNOTE_ROWID = "ParkNoteRowId";

        public const string LABEL_DIRECTED_ENFORCEMENT_TAB = "Directed Enforcement";
        //public const string LABEL_GIS_TAB = "GIS Map";
        //public const string LABEL_GIS_TAB = "MAP VIEW";
        //public const string LABEL_GIS_TAB = "MAP";
        //public const string LABEL_GIS_TAB = "Map Based Enforcement";
        public const string LABEL_GIS_TAB = "Maps";



        //public const string LABEL_GISLIST_TAB = "GIS List";
        //public const string LABEL_GISLIST_TAB = "GIS LIST";
        //public const string LABEL_GISLIST_TAB = "SPACE LIST";
        public const string LABEL_GISLIST_TAB = "SPACES";


        public const string LABEL_PAYBYPLATELIST_TAB = "VEHICLES";


        public const string LABEL_MY_ACTIVITY_MENU_GROUP = "MY ACTIVITY";
        public const string LABEL_MY_ACTIVITY_MY_LOG = "Log";
        public const string LABEL_MY_ACTIVITY_MY_ROUTE = "Route";
        public const string LABEL_MY_ACTIVITY_MY_JOURNAL = "My Journal";


        public const string LABEL_APPLICATION_SETTINGS_MENU_GROUP = "SETTINGS";
        public const string LABEL_APPLICATION_SETTINGS_PRINTERS_OPTION = "Printers";
        public const string LABEL_APPLICATION_SETTINGS_BLUETOOTH_OPTION = "BlueTooth";
        public const string LABEL_APPLICATION_SETTINGS_LOGOUT_OPTION = "Logout";
        



        public const string TICKETID = "TICKETID";

        public const string FORMREV = "FORMREV";
        public const string FORMNAME = "FORMNAME";


        public const string cnStatusUpdateActionMenuTextVoid = "Void";
        public const string cnStatusUpdateActionMenuTextReIssue = "Void and Re-Issue";
        public const string cnStatusUpdateActionMenuTextIssueMore = "Issue More";
        public const string cnStatusUpdateActionMenuTextIssueMultiple = "Issue Multiple";
        public const string cnStatusUpdateActionMenuTextCancel = "Cancel Citation";


        public const string cnNotesActionMenuTextAddPhoto = "Add Photo";
        public const string cnNotesActionMenuTextAddNotation = "Add Text Note";
        public const string cnNotesActionMenuTextReviewNotes = "Review All Notes";

        //N5Printer constants
        public const string TWOTECH_N5PRINTER_PRINTJOB_INTENT = "com.civicsmart.N5printjob";
        public const string TWOTECH_N5PRINTER_SEND_PM_TO_BACKGROUND_INTENT = "com.civicsmart.SendPMToBackground";
        public const string TWOTECH_N5PRINTER_PM_APP_NAME_TOKEN = "n5simpleprint"; //"com.twotechnologies.n5simpleprint"
        public const string TWOTECH_N5PRINTER_PM_BARCODEBUTTON_CLICKED_LISTNER = "com.civicsmart.BarCodeButtonClicked";
        public const string TWOTECH_N5PRINTER_PM_ERRORS_LISTNER = "com.civicsmart.N5PrinterErrors";
        public const string TWOTECH_N5PRINTER_PM_ERRORS_ERRORCODE = "com.civicsmart.N5PrinterErrors.ErrorCode";
        public const int TWOTECH_N5PRINTER_MODE_T5_TEXT = 0;
        public const int TWOTECH_N5PRINTER_MODE_T5_GRAPHIC = 1;
  
    }
}

