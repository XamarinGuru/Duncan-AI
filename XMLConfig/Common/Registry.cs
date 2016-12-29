// VSS Keyword information
// File last modified by:   $Author: Alan $
//                    on:     $Date: 03/25/16 1:00p $
//               Archive:  $Archive: /AI_Android/Duncan.AI.Droid/Utils/Registry.cs $
//              Revision: $Revision: 1 $

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.IO;




using Android.Util;


namespace Reino.ClientConfig
{
    public class TRegEntry
    {
        internal string Section = "";
        internal string Item = "";
        internal string Value = "";
    }

    public class TTRegistry : Reino.ClientConfig.ITTRegistry
    {
        public static TTRegistry glRegistry = null;


        public const string RegistryFileNameOnHandheld = "REGISTRY.DAT";

        protected List<TRegEntry> fRegEntries = new List<TRegEntry>();
        private const string SectionFieldName = "SECTION";
        private const string ItemFieldName = "ITEM";
        private const string ValueFieldName = "VALUE";


        // declarations to find entries in REGISTRY.DAT 
        public const string regSECTION_ISSUE_AP = "ISSUE_AP";
        public const string regSECTION_SYSTEM = "SYSTEM";
        public const string regSECTION_INTERNAL = "INTERNAL";

        public const string regOPTION_DISABLED = "0";
        public const string regOPTION_ENABLED = "1";



        // GPRS void/cancel decision points are not left to officer, all defer to public
        public const string regGPRS_METER_STATUS_DEFER_TO_PUBLIC_MODE = "GPRS_METER_STATUS_DEFER_TO_PUBLIC_MODE";
        public const int regGPRS_METER_STATUS_DEFER_TO_PUBLIC_MODE_DEFAULT = 0;


        // Show or hide unenforceable results when warnings are present (Only applicable to Parkeon meter provider)
        public const string regGPRS_METER_SHOW_RESULTS_WHEN_WARNINGS = "GPRS_METER_SHOW_RESULTS_WHEN_WARNINGS";
        public const int regGPRS_METER_SHOW_RESULTS_WHEN_WARNINGS_DEFAULT = 1; // Default value is True (1) for behavioral backward compatibility

        // Show or hide wireless activity on "please wait" screens for wireless meter enforcement (Not applicable to Pay-by-Phone model. --> i.e. Verrus provider)
        public const string regGPRS_METER_SHOW_VERBOSE_ACTIVITY = "GPRS_METER_SHOW_VERBOSE_ACTIVITY";
        public const int regGPRS_METER_SHOW_VERBOSE_ACTIVITY_DEFAULT = 0;

        // how many minutes of payment age will we display? 
        public const string regGPRS_METER_MAX_PAYMENT_AGE_MINUTES = "GPRS_METER_MAX_PAYMENT_AGE_MINUTES";
        public const int regGPRS_METER_MAX_PAYMENT_AGE_MINUTES_DEFAULT = 64800;    // 45 days * 24 hours * 60 mins


        // if we exceed the payment age in minutes, what should be displayed instead>
        public const string regGPRS_METER_MAX_PAYMENT_AGE_DISPLAY_TEXT = "GPRS_METER_MAX_PAYMENT_AGE_DISPLAY_TEXT";
        public const string regGPRS_METER_MAX_PAYMENT_AGE_DISPLAY_TEXT_DEFAULT = "";


        // how many seconds will we wait before displaying "No Recent Contact" message?
        public const string regGPRS_METER_SERVER_RESPONSE_TIMEOUT_SECONDS = "GPRS_METER_SERVER_RESPONSE_TIMEOUT_SECONDS";
        public const int regGPRS_METER_SERVER_RESPONSE_TIMEOUT_SECONDS_DEFAULT = 120;



        // Reino customer ID passed to webservice
        public const string regREINO_CUSTID = "REINO_CUSTID";
        public const string regREINO_CUSTID_DEFAULT = "9999";

        // DPT customer ID passed to webservice
        public const string regDPT_CUSTID = "DPT_CUSTID";
        public const string regDPT_CUSTID_DEFAULT = "9999";

        // Parkeon customer ID passed to webservice
        public const string regPARKEON_CUSTID = "PARKEON_CUSTID";
        public const string regPARKEON_CUSTID_DEFAULT = "9999";

        // ParkNOW customer ID passed to webservice
        public const string regPARKNOW_CUSTID = "PARKNOW_CUSTID";
        public const string regPARKNOW_CUSTID_DEFAULT = "9999";



        // legacy version: set to 1 for set meter enforcement mode to select by range of stalls (DPT style) instead of against a meter/cluster
        public const string regGPRS_METER_STALL_RANGE_MODE = "GPRS_METER_STALL_RANGE_MODE";
        public const int regGPRS_METER_STALL_RANGE_MODE_DEFAULT = 0;

        // new param: set to provider name to determine enforcement mode
        public const string regGPRS_METER_PROVIDER_NAME = "GPRS_METER_PROVIDER_NAME";
        public const string regGPRS_METER_PROVIDER_NAME_DUNCAN = "DUNCAN";
        public const string regGPRS_METER_PROVIDER_NAME_DPT = "DPT";
        public const string regGPRS_METER_PROVIDER_NAME_PARKEON = "PARKEON";
        public const string regGPRS_METER_PROVIDER_NAME_PARKNOW = "PARKNOW";
        public const string regGPRS_METER_PROVIDER_NAME_VERRUS = "VERRUS";
        public const string regGPRS_METER_PROVIDER_NAME_PANGO = "PANGO";
        public const string regGPRS_METER_PROVIDER_NAME_DUNCAN_OVERSTAYVIO = "DUNCAN_OVERSTAYVIO";
        public const string regGPRS_METER_PROVIDER_NAME_MULTIVENDOR = "MULTIVENDOR";
        public const string regGPRS_METER_PROVIDER_NAME_DUNCAN_PAYBYPLATE = "DUNCAN_PAYBYPLATE";
        public const string regGPRS_METER_PROVIDER_NAME_CALE_PAYBYPLATE = "CALE_PAYBYPLATE";
        public const string regGPRS_METER_PROVIDER_NAME_DEFAULT = regGPRS_METER_PROVIDER_NAME_DUNCAN;



        // Is these two tab required for NOLA?

        // If yes. We will share new URLs

        //MAP : http://106.51.253.214/gisformobile/?cityid=5001
        //SPACE : http://106.51.253.214/MobileSpaceStatus/SpaceStatus/SpcSummary/5001





        // for android web views
        public const string regPAYBYSPACE_WEBVIEW_MAP_ENABLED = "PAYBYSPACE_WEBVIEW_MAP_ENABLED";
        public const string regPAYBYSPACE_WEBVIEW_MAP_ENABLED_DEFAULT = regOPTION_DISABLED;

        public const string regPAYBYSPACE_WEBVIEW_MAP_URL_BASE = "PAYBYSPACE_WEBVIEW_MAP_URL_BASE";
        public const string regPAYBYSPACE_WEBVIEW_MAP_URL_BASE_DEFAULT = "http://106.51.253.214/GISForMobile/";

        //MAP : http://106.51.253.214/gisformobile/?cityid=5001



        public const string regPAYBYSPACE_WEBVIEW_MAP_CUSTID = "PAYBYSPACE_WEBVIEW_MAP_CUSTID";
        public const string regPAYBYSPACE_WEBVIEW_MAP_CUSTID_DEFAULT = "";


        // //

        public const string regPAYBYSPACE_WEBVIEW_LIST_ENABLED = "PAYBYSPACE_WEBVIEW_LIST_ENABLED";
        public const string regPAYBYSPACE_WEBVIEW_LIST_ENABLED_DEFAULT =  regOPTION_DISABLED;

        public const string regPAYBYSPACE_WEBVIEW_LIST_URL_BASE = "PAYBYSPACE_WEBVIEW_LIST_URL_BASE";
        public const string regPAYBYSPACE_WEBVIEW_LIST_URL_BASE_DEFAULT = "http://106.51.253.214/MobileSpaceStatus/";
        //SPACE : http://106.51.253.214/MobileSpaceStatus/SpaceStatus/SpcSummary/5001



        public const string regPAYBYSPACE_WEBVIEW_LIST_CUSTID = "PAYBYSPACE_WEBVIEW_LIST_CUSTID";
        public const string regPAYBYSPACE_WEBVIEW_LIST_CUSTID_DEFAULT = "";



        // // 

        public const string regPAYBYPLATE_WEBVIEW_MAP_ENABLED = "PAYBYPLATE_WEBVIEW_MAP_ENABLED";  // not yet available
        public const string regPAYBYPLATE_WEBVIEW_MAP_ENABLED_DEFAULT = regOPTION_DISABLED;  

        public const string regPAYBYPLATE_WEBVIEW_MAP_URL_BASE = "PAYBYPLATE_WEBVIEW_MAP_URL_BASE";
        public const string regPAYBYPLATE_WEBVIEW_MAP_URL_BASE_DEFAULT = "";

        public const string regPAYBYPLATE_WEBVIEW_MAP_CUSTID = "PAYBYPLATE_WEBVIEW_MAP_CUSTID";
        public const string regPAYBYPLATE_WEBVIEW_MAP_CUSTID_DEFAULT = "";

        //public const string regPAYBYSPACE_WEBVIEW_MAP_CUSTID = "PAYBYSPACE_WEBVIEW_MAP_CUSTID";
        //public const string regPAYBYSPACE_WEBVIEW_MAP_CUSTID_DEFAULT = "";


        // // 

        public const string regPAYBYPLATE_WEBVIEW_LIST_ENABLED = "PAYBYPLATE_WEBVIEW_LIST_ENABLED";
        public const string regPAYBYPLATE_WEBVIEW_LIST_ENABLED_DEFAULT = regOPTION_DISABLED;
        
        public const string regPAYBYPLATE_WEBVIEW_LIST_URL_BASE = "PAYBYPLATE_WEBVIEW_LIST_URL_BASE";
        public const string regPAYBYPLATE_WEBVIEW_LIST_URL_BASE_DEFAULT = "http://106.51.253.214/PaybyPlate/Pbp/PbpSummary/";

        public const string regPAYBYPLATE_WEBVIEW_LIST_CUSTID = "PAYBYPLATE_WEBVIEW_LIST_CUSTID";
        public const string regPAYBYPLATE_WEBVIEW_LIST_CUSTID_DEFAULT = "4176";  // New Orleans            // default to New Orleans if not defined

         // //


        // new generic web view definitions, constructed using index 1..N based on registry settings
        public const string regWEBVIEW_MENU_ITEM_N_PREFIX = "WEBVIEW_MENU_ITEM_";


        public const string regWEBVIEW_MENU_ITEM_N_ENABLED_SUFFIX = "_ENABLED";
        public const string regWEBVIEW_MENU_ITEM_N_ENABLED_DEFAULT = regOPTION_DISABLED;

        public const string regWEBVIEW_MENU_ITEM_N_TEXT_SUFFIX = "_TEXT";
        public const string regWEBVIEW_MENU_ITEM_N_TEXT_DEFAULT = "WEBVIEW MENU ITEM ";

        public const string regWEBVIEW_MENU_ITEM_N_URL_BASE_SUFFIX = "_URL_BASE";
        public const string regWEBVIEW_MENU_ITEM_N_URL_BASE_DEFAULT = "";

        public const string regWEBVIEW_MENU_ITEM_N_PARAM_NAME_1_SUFFIX = "_PARAM_NAME_1";
        public const string regWEBVIEW_MENU_ITEM_N_PARAM_NAME_1_DEFAULT = "";

        public const string regWEBVIEW_MENU_ITEM_N_PARAM_VALUE_1_SUFFIX = "_PARAM_VALUE_1";
        public const string regWEBVIEW_MENU_ITEM_N_PARAM_VALUE_1_DEFAULT = "";

        public const string regWEBVIEW_MENU_ITEM_N_PARAM_NAME_2_SUFFIX = "_PARAM_NAME_2";
        public const string regWEBVIEW_MENU_ITEM_N_PARAM_NAME_2_DEFAULT = "";

        public const string regWEBVIEW_MENU_ITEM_N_PARAM_VALUE_2_SUFFIX = "_PARAM_VALUE_2";
        public const string regWEBVIEW_MENU_ITEM_N_PARAM_VALUE_2_DEFAULT = "";


        // web view parameters that start with this get an attempt at substituion
        public const string cnWebViewParameterSystemPrefix = "sys_";
        public const string cnWebViewParameterCityId = "sys_" + "cityid";
        public const string cnWebViewParameterOfficerId = "sys_" + "officerid";















        // activate this to only consider the plate when looking for PBC payments
        public const string regPBC_EXCLUDE_STATE_IN_VEHICLE_FILTERS = "PBC_EXCLUDE_STATE_IN_VEHICLE_FILTERS";
        // default is active for backward compatibility
        public const int regPBC_EXCLUDE_STATE_IN_VEHICLE_FILTERS_DEFAULT = 1;

        // activate this to supplement GetZoneStatus requests with a GetVehcileStatus request for vendors (such as Pango)
        // can implement their reconciliation reporting
        public const string regPBC_SEND_VEHICLE_STATUS_REQUESTS = "PBC_SEND_VEHICLE_STATUS_REQUESTS";
        // default is inactive for backward compatibility (Verrus doesn't need and/or support)
        public const int regPBC_SEND_VEHICLE_STATUS_REQUESTS_DEFAULT = 0;


        public const string regPBC_SEARCHZONEID_MAPPEDFIELD = "PBC_SEARCHZONEID_MAPPEDFIELD";
        public const string regPBC_SEARCHZONEID_MAPPEDFIELD_DEFAULT = "";

        public const string regPBC_SEARCHZONEDESC_MAPPEDFIELD = "PBC_SEARCHZONEDESC_MAPPEDFIELD";
        public const string regPBC_SEARCHZONEDESC_MAPPEDFIELD_DEFAULT = "";


        public const string regPBC_ANYZONE_ENABLED = "PBC_ANYZONE_ENABLED";
        public const int regPBC_ANYZONE_ENABLED_DEFAULT = 0;

        public const string regPBC_ANYZONE_ZONEDESC = "PBC_ANYZONE_ZONEDESC";
        public const string regPBC_ANYZONE_ZONEDESC_DEFAULT = "ANY ZONE";



        public const string regPBC_GPRS_REFRESH_INTERVAL_SEC = "PBC_GPRS_REFRESH_INTERVAL_SEC";
        public const int regPBC_GPRS_REFRESH_INTERVAL_SEC_DEFAULT = 90;

        public const string regPBC_GPRS_MAX_AGE_SEC = "PBC_GPRS_MAX_AGE_SEC";
        public const int regPBC_GPRS_MAX_AGE_SEC_DEFAULT = 180;

        public const string regPBC_GPRS_CONFIRMATION_SEC = "PBC_GPRS_CONFIRMATION_SEC";
        public const int regPBC_GPRS_CONFIRMATION_SEC_DEFAULT = 20;



        // level details to display for meter warning lists
        public const string regGPRS_METER_WARNING_LEVEL_DETAIL = "GPRS_METER_WARNING_LEVEL_DETAIL";
        public const string regGPRS_METER_WARNING_LEVEL_DETAIL_NONE = "GPRS_METER_WARNING_LEVEL_DETAIL_NONE";
        public const string regGPRS_METER_WARNING_LEVEL_DETAIL_METER_COUNTS = "METER_COUNTS";
        public const string regGPRS_METER_WARNING_LEVEL_DETAIL_METER_NAMES = "METER_NAMES";
        public const string regGPRS_METER_WARNING_LEVEL_DETAIL_METER_COUNTS_AND_NAMES = "METER_COUNTS_AND_NAMES";
        public const string regGPRS_METER_WARNING_LEVEL_DETAIL_DEFAULT = regGPRS_METER_WARNING_LEVEL_DETAIL_NONE;


        // set to 1 to prevent records from being reissued or continued if they were originally issued with time-sensitive MSM status data
        public const string regPREVENT_REISSUE_OF_MSM_BASED_ENFORCEMENT = "PREVENT_REISSUE_OF_MSM_BASED_ENFORCEMENT";
        public const int regPREVENT_REISSUE_OF_MSM_BASED_ENFORCEMENT_DEFAULT = 0;


        // set to 1 to prevent records from being voided the day after the issue date (used for wireless systems with long times between USB sync)
        public const string regVOID_ALLOWED_ON_DATE_OF_ISSUE_ONLY = "VOID_ALLOWED_ON_DATE_OF_ISSUE_ONLY";
        public const int regVOID_ALLOWED_ON_DATE_OF_ISSUE_ONLY_DEFAULT = 0;


        // set to non-zero value to prevent records from being voided beyond a limited time window
        public const string regVOID_ALLOWED_WITHIN_X_MINUTES_ONLY = "VOID_ALLOWED_WITHIN_X_MINUTES_ONLY";
        public const int regVOID_ALLOWED_WITHIN_X_MINUTES_ONLY_DEFAULT = 0;

        // set to non-zero value to prevent records from being reissued beyond a limited time window
        public const string regREISSUE_ALLOWED_WITHIN_X_MINUTES_ONLY = "REISSUE_ALLOWED_WITHIN_X_MINUTES_ONLY";
        public const int regREISSUE_ALLOWED_WITHIN_X_MINUTES_ONLY_DEFAULT = 0;


        //Void is possible without printing if GetExternalPreventESC() is true unless overridden by this registry item
        public const string regPRINT_REQD_FOR_VOID = "PRINT_REQD_FOR_VOID";
        public const int regPRINT_REQD_FOR_VOID_DEFAULT = 1;

        // introduced to hide the void button so the users cant initiate the void manually, but it can be invoked programtically
        public const string regPREVENT_MANUAL_FIELD_VOID = "PREVENT_MANUAL_FIELD_VOID";
        public const int regPREVENT_MANUAL_FIELD_VOID_DEFAULT = 0;


        // when active, a background thread records a GPS location in tha activity log. Will also be uploaded wirelessly if that is enabled
        public const string regLOCATION_UPDATE_MIN = "LOCATION_UPDATE_MIN";   // to activate location tracking thread 
        public const int regLOCATION_UPDATE_MIN_DEFAULT = -1;              // <= 0 means no location tracking


        public const string regAUTOATTACHFILECOUNT = "AUTOATTACHFILECOUNT";
        public const int regAUTOATTACHFILECOUNT_DEFAULT = 0;

        public const string regAUTOATTACH_IMAGE = "AUTOATTACH_IMAGE";
        public const int regAUTOATTACH_IMAGE_DEFAULT = 0;


        public const string regIMAGE_PRINT_SELECTION_AUTO_COUNT = "IMAGE_PRINT_SELECTION_AUTO_COUNT";
        public const int regIMAGE_PRINT_SELECTION_AUTO_COUNT_DEFAULT = 1;

        //Registy key to enable/disable adding watermark to the captured images
        public const string regADDWATERMARK_TO_PICTURE = "ADD_WATERMARK_TO_PICTURE";
        public const int regADDWATERMARK_TO_PICTURE_DEFAULT = 1;

        public const string regWATERMARK_DATE_FORMAT = "reg_WATERMARK_DATE_FORMAT";
        public const string regWATERMARK_DATE_FORMAT_DEFAULT = "MM/dd/yyyy";
        public const string regWATERMARK_TIME_FORMAT = "reg_WATERMARK_DATE_FORMAT";
        public const string regWATERMARK_TIME_FORMAT_DEFAULT = "hh:mm:ss";

        public const string regWATERMARK_LOCATION = "WATERMARK_LOCATION";
        public const int regWATERMARK_LOCATION_DEFAULT = WATERMARK_LOCATION_LOWER_LEFT;
        public const int WATERMARK_LOCATION_LOWER_LEFT = 0;
        public const int WATERMARK_LOCATION_LOWER_RIGHT = 1;
        public const int WATERMARK_LOCATION_UPPER_LEFT = 2;
        public const int WATERMARK_LOCATION_UPPER_RIGHT = 3;


        public const string regFORM_ENTRY_FIRST_FOCUS_PARKING = "FORM_ENTRY_FIRST_FOCUS_PARKING"; // if not "", will try to use it for for parking issue new
        public const string regFORM_ENTRY_FIRST_FOCUS_PARKING_DEFAULT = "";

        public const string regFORM_ENTRY_FIRST_FOCUS_TRAFFIC = "FORM_ENTRY_FIRST_FOCUS_TRAFFIC"; // if not "", will try to use it for for TRAFFIC issue new
        public const string regFORM_ENTRY_FIRST_FOCUS_TRAFFIC_DEFAULT = "";

        public const string regFORM_ENTRY_FIRST_FOCUS_MARKMODE = "FORM_ENTRY_FIRST_FOCUS_MARKMODE"; // if not "", will try to use it for for markmode issue new
        public const string regFORM_ENTRY_FIRST_FOCUS_MARKMODE_DEFAULT = ""; 


        public const string regFORM_ENTRY_FOCUS_FIELD_PLACEMENT_BOTTOM = "FORM_ENTRY_FOCUS_FIELD_PLACEMENT_BOTTOM"; //Value ( 0 or 1 ) - default = 0 "field placed at top of scroll view"
        public const int regFORM_ENTRY_FOCUS_FIELD_PLACEMENT_BOTTOM_DEFAULT = 0; 



        public const string regEXTRA_OCR_DOT_WIDTH = "EXTRA_OCR_DOT_WIDTH";
        public const int regEXTRA_OCR_DOT_WIDTH_DEFAULT = 1;

        #region Printer Settings and Options

        // if set, PDA printing won't send / utilize the sensor mark (used for unmarked paper)
        public const string regIGNORE_PAPER_SENSOR_MARK_PDA = "IGNORE_PAPER_SENSOR_MARK_PDA";
        public const int regIGNORE_PAPER_SENSOR_MARK_PDA_DEFAULT = 0;


        // if set to a print picture revision named in XML, i.e. "ISSUE_4IN", layout will be treated/displayed as 3IN 
        public const string regPRINTER_ALTERNATE_3IN_TEXT_PICTURE = "PRINTER_ALTERNATE_3IN_TEXT_PICTURE";
        public const string regPRINTER_ALTERNATE_3IN_TEXT_PICTURE_DEFAULT = "NONE";



		//2T-N5 specific printer settings
        public const string regPRINTER_GRAPHIC_MODE_T5 = "PRINTER_GRAPHIC_MODE_T5"; //Value ( 0 or 1 ) - default = 0 "text mode"        
        public const int regPRINTER_GRAPHIC_MODE_T5_DEFAULT = 0;

        //2T-N5 specific printer - Text mode parameters
        public const string regPRINTER_CONTRAST_LEVEL_TEXT_T5 = "PRINTER_CONTRAST_LEVEL_TEXT_T5";
        public const int regPRINTER_CONTRAST_LEVEL_TEXT_T5_DEFAULT = 8; //values from 0 to 9 (darkest).

        public const string regPRINTER_SETFF_MAXFEED_T5 = "PRINTER_SETFF_MAXFEED_T5";
        public const int regPRINTER_SETFF_MAXFEED_T5_DEFAULT = 1275;

        public const string regPRINTER_SETFF_SKIPLENGTH_T5 = "PRINTER_SETFF_SKIPLENGTH_T5";
        public const int regPRINTER_SETFF_SKIPLENGTH_T5_DEFAULT = 0;

        public const string regEXTRA_DOT_LINES_T5_PREFEED = "EXTRA_DOT_LINES_T5_PREFEED";
        public const int regEXTRA_DOT_LINES_T5_PREFEED_DEFAULT = 100;

        public const string regEXTRA_DOT_LINES_T5_POSTFEED = "EXTRA_DOT_LINES_T5_POSTFEED";
        public const int regEXTRA_DOT_LINES_T5_POSTFEED_DEFAULT = 0;

        public const string regPRINTER_VERTICAL_SPACE_SCALE_T5 = "PRINTER_VERTICAL_SPACE_SCALE_T5";
        public const int regPRINTER_VERTICAL_SPACE_SCALE_T5_DEFAULT = 70;   // % value 

        //2T-N5 specific printer - Graphic mode parameters 
        public const string regPRINTER_CONTRAST_LEVEL_GRAPHIC_T5 = "PRINTER_CONTRAST_LEVEL_GRAPHIC_T5";
        public const int regPRINTER_CONTRAST_LEVEL_GRAPHIC_T5_DEFAULT = 9; //values from 0 to 9 (darkest).

        public const string regPRINTER_SETFF_MAXFEED_GRAPHIC_T5 = "PRINTER_SETFF_MAXFEED_GRAPHIC_T5"; //( Value 0 – 255 )
        public const int regPRINTER_SETFF_MAXFEED_GRAPHIC_T5_DEFAULT = 1275;

        public const string regPRINTER_SETFF_SKIPLENGTH_GRAPHIC_T5 = "PRINTER_SETFF_SKIPLENGTH_GRAPHIC_T5";
        public const int regPRINTER_SETFF_SKIPLENGTH_GRAPHIC_T5_DEFAULT = 0;

        public const string regEXTRA_DOT_LINES_GRAPHIC_T5_PREFEED = "EXTRA_DOT_LINES_GRAPHIC_T5_PREFEED";
        public const int regEXTRA_DOT_LINES_GRAPHIC_T5_PREFEED_DEFAULT = 100;

        public const string regEXTRA_DOT_LINES_GRAPHIC_T5_POSTFEED = "EXTRA_DOT_LINES_GRAPHIC_T5_POSTFEED";
        public const int regEXTRA_DOT_LINES_GRAPHIC_T5_POSTFEED_DEFAULT = 0;
        
        /////////////////////////////////////////

        public const string regEXTRA_DOT_LINES_QL320_PREFEED = "EXTRA_DOT_LINES_QL320_PREFEED";
        public const int regEXTRA_DOT_LINES_QL320_PREFEED_DEFAULT = 162;

        public const string regEXTRA_DOT_LINES_QL320_POSTFEED = "EXTRA_DOT_LINES_QL320_POSTFEED";
        public const int regEXTRA_DOT_LINES_QL320_POSTFEED_DEFAULT = 30;

        public const string regPRINT_TONE_QL320 = "PRINT_TONE_QL320";
        public const int regPRINT_TONE_QL320_DEFAULT = 75;

        public const string regPAGE_HEIGHT_DOT_LINES_QL320 = "PAGE_HEIGHT_DOT_LINES_QL320";
        public const int regPAGE_HEIGHT_DOT_LINES_QL320_DEFAULT = 880;


        public const string regPRINTER_BAR_SENSE_SENSITIVITY_QL320 = "PRINTER_BAR_SENSE_SENSITIVITY_QL320";
        public const int regPRINTER_BAR_SENSE_SENSITIVITY_QL320_DEFAULT = -1;

        public const string regPRINTER_BAR_SENSE_IGNORE_QL320 = "PRINTER_BAR_SENSE_IGNORE_QL320";
        public const int regPRINTER_BAR_SENSE_IGNORE_QL320_DEFAULT = 0;

        public const string regPRINTER_JOURNAL_MODE_QL320 = "PRINTER_JOURNAL_MODE_QL320";
        public const int regPRINTER_JOURNAL_MODE_QL320_DEFAULT = -1;

        public const string regPRINTER_SETFF_MAXFEED_QL320 = "PRINTER_SETFF_MAXFEED_QL320";
        public const int regPRINTER_SETFF_MAXFEED_QL320_DEFAULT = -1;

        public const string regPRINTER_SETFF_SKIPLENGTH_QL320 = "PRINTER_SETFF_SKIPLENGTH_QL320";
        public const int regPRINTER_SETFF_SKIPLENGTH_QL320_DEFAULT = -1;

        public const string regPRINTER_SPEED_QL320 = "PRINTER_SPEED_QL320";
        public const int regPRINTER_SPEED_QL320_DEFAULT = 4;


        /////////////////////////////////////////



        public const string regEXTRA_DOT_LINES_MZ320_PREFEED = "EXTRA_DOT_LINES_MZ320_PREFEED";
        public const int regEXTRA_DOT_LINES_MZ320_PREFEED_DEFAULT = 330;

        public const string regEXTRA_DOT_LINES_MZ320_POSTFEED = "EXTRA_DOT_LINES_MZ320_POSTFEED";
        public const int regEXTRA_DOT_LINES_MZ320_POSTFEED_DEFAULT = 30;

        public const string regPRINT_TONE_MZ320 = "PRINT_TONE_MZ320";
        public const int regPRINT_TONE_MZ320_DEFAULT = 75;

        public const string regPAGE_HEIGHT_DOT_LINES_MZ320 = "PAGE_HEIGHT_DOT_LINES_MZ320";
        public const int regPAGE_HEIGHT_DOT_LINES_MZ320_DEFAULT = 880;


        // The Zebra technician sez:
        // The adjustment doesn't actually change the sensitivity of the sensor itself, 
        // but actually adjusts the threshold of when the firmware makes the decision 
        // that a mark or bar has been seen. So, from that standpoint, 
        // the higher the number, the less sensitive the printer is to data seen by the 
        // bar sensor. The DEFAULT = is 70, but sometimes it may vary very slightly. 
        // The sensor doesn't detect color. It is a reflective sensor and measures how much 
        // light is reflected back off the media. Red reflects back less light than black
        //
        public const string regPRINTER_BAR_SENSE_SENSITIVITY_MZ320 = "PRINTER_BAR_SENSE_SENSITIVITY_MZ320";
        public const int regPRINTER_BAR_SENSE_SENSITIVITY_MZ320_DEFAULT = -1;

        public const string regPRINTER_BAR_SENSE_IGNORE_MZ320 = "PRINTER_BAR_SENSE_IGNORE_MZ320";
        public const int regPRINTER_BAR_SENSE_IGNORE_MZ320_DEFAULT = 0;


        public const string regPRINTER_JOURNAL_MODE_MZ320 = "PRINTER_JOURNAL_MODE_MZ320";
        public const int regPRINTER_JOURNAL_MODE_MZ320_DEFAULT = -1;

        public const string regPRINTER_SETFF_MAXFEED_MZ320 = "PRINTER_SETFF_MAXFEED_MZ320";
        public const int regPRINTER_SETFF_MAXFEED_MZ320_DEFAULT = -1;

        public const string regPRINTER_SETFF_SKIPLENGTH_MZ320 = "PRINTER_SETFF_SKIPLENGTH_MZ320";
        public const int regPRINTER_SETFF_SKIPLENGTH_MZ320_DEFAULT = -1;

        public const string regPRINTER_SPEED_MZ320 = "PRINTER_SPEED_MZ320";
        public const int regPRINTER_SPEED_MZ320_DEFAULT = 4;


        /////////////////////////////////////////



        public const string regEXTRA_DOT_LINES_IMZ320_PREFEED = "EXTRA_DOT_LINES_IMZ320_PREFEED";
        public const int regEXTRA_DOT_LINES_IMZ320_PREFEED_DEFAULT = 330;

        public const string regEXTRA_DOT_LINES_IMZ320_POSTFEED = "EXTRA_DOT_LINES_IMZ320_POSTFEED";
        public const int regEXTRA_DOT_LINES_IMZ320_POSTFEED_DEFAULT = 30;

        public const string regPRINT_TONE_IMZ320 = "PRINT_TONE_IMZ320";
        public const int regPRINT_TONE_IMZ320_DEFAULT = 75;

        public const string regPAGE_HEIGHT_DOT_LINES_IMZ320 = "PAGE_HEIGHT_DOT_LINES_IMZ320";
        public const int regPAGE_HEIGHT_DOT_LINES_IMZ320_DEFAULT = 880;


        //
        public const string regPRINTER_BAR_SENSE_SENSITIVITY_IMZ320 = "PRINTER_BAR_SENSE_SENSITIVITY_IMZ320";
        public const int regPRINTER_BAR_SENSE_SENSITIVITY_IMZ320_DEFAULT = -1;

        public const string regPRINTER_BAR_SENSE_IGNORE_IMZ320 = "PRINTER_BAR_SENSE_IGNORE_IMZ320";
        public const int regPRINTER_BAR_SENSE_IGNORE_IMZ320_DEFAULT = 0;

        public const string regPRINTER_JOURNAL_MODE_IMZ320 = "PRINTER_JOURNAL_MODE_IMZ320";
        public const int regPRINTER_JOURNAL_MODE_IMZ320_DEFAULT = -1;

        public const string regPRINTER_SETFF_MAXFEED_IMZ320 = "PRINTER_SETFF_MAXFEED_IMZ320";
        public const int regPRINTER_SETFF_MAXFEED_IMZ320_DEFAULT = -1;

        public const string regPRINTER_SETFF_SKIPLENGTH_IMZ320 = "PRINTER_SETFF_SKIPLENGTH_IMZ320";
        public const int regPRINTER_SETFF_SKIPLENGTH_IMZ320_DEFAULT = -1;

        public const string regPRINTER_SPEED_IMZ320 = "PRINTER_SPEED_IMZ320";
        public const int regPRINTER_SPEED_IMZ320_DEFAULT = 4;
        //

        //////////////////////////////////////////



        public const string regEXTRA_DOT_LINES_RW420_PREFEED = "EXTRA_DOT_LINES_RW420_PREFEED";
        public const int regEXTRA_DOT_LINES_RW420_PREFEED_DEFAULT = 162;

        public const string regEXTRA_DOT_LINES_RW420_POSTFEED = "EXTRA_DOT_LINES_RW420_POSTFEED";
        public const int regEXTRA_DOT_LINES_RW420_POSTFEED_DEFAULT = 30;

        public const string regPRINT_TONE_RW420 = "PRINT_TONE_RW420";
        public const int regPRINT_TONE_RW420_DEFAULT = 75;

        public const string regPRINTER_BAR_SENSE_SENSITIVITY_RW420 = "PRINTER_BAR_SENSE_SENSITIVITY_RW420";
        public const int regPRINTER_BAR_SENSE_SENSITIVITY_RW420_DEFAULT = -1;

        public const string regPRINTER_BAR_SENSE_IGNORE_RW420 = "PRINTER_BAR_SENSE_IGNORE_RW420";
        public const int regPRINTER_BAR_SENSE_IGNORE_RW420_DEFAULT = 0;

        public const string regPRINTER_JOURNAL_MODE_RW420 = "PRINTER_JOURNAL_MODE_RW420";
        public const int regPRINTER_JOURNAL_MODE_RW420_DEFAULT = -1;

        public const string regPRINTER_SETFF_MAXFEED_RW420 = "PRINTER_SETFF_MAXFEED_RW420";
        public const int regPRINTER_SETFF_MAXFEED_RW420_DEFAULT = -1;

        public const string regPRINTER_SETFF_SKIPLENGTH_RW420 = "PRINTER_SETFF_SKIPLENGTH_RW420";
        public const int regPRINTER_SETFF_SKIPLENGTH_RW420_DEFAULT = -1;

        public const string regPRINTER_SPEED_RW420 = "PRINTER_SPEED_RW420";
        public const int regPRINTER_SPEED_RW420_DEFAULT = 4;



        ///////////////////////////////////////////

        public const string regEXTRA_DOT_LINES_RW220_PREFEED = "EXTRA_DOT_LINES_RW220_PREFEED";
        public const int regEXTRA_DOT_LINES_RW220_PREFEED_DEFAULT = 162;

        public const string regEXTRA_DOT_LINES_RW220_POSTFEED = "EXTRA_DOT_LINES_RW220_POSTFEED";
        public const int regEXTRA_DOT_LINES_RW220_POSTFEED_DEFAULT = 30;

        public const string regPRINT_TONE_RW220 = "PRINT_TONE_RW220";
        public const int regPRINT_TONE_RW220_DEFAULT = 75;


        public const string regPRINTER_BAR_SENSE_SENSITIVITY_RW220 = "PRINTER_BAR_SENSE_SENSITIVITY_RW220";
        public const int regPRINTER_BAR_SENSE_SENSITIVITY_RW220_DEFAULT = -1;

        public const string regPRINTER_BAR_SENSE_IGNORE_RW220 = "PRINTER_BAR_SENSE_IGNORE_RW220";
        public const int regPRINTER_BAR_SENSE_IGNORE_RW220_DEFAULT = 0;

        public const string regPRINTER_JOURNAL_MODE_RW220 = "PRINTER_JOURNAL_MODE_RW220";
        public const int regPRINTER_JOURNAL_MODE_RW220_DEFAULT = -1;

        public const string regPRINTER_SETFF_MAXFEED_RW220 = "PRINTER_SETFF_MAXFEED_RW220";
        public const int regPRINTER_SETFF_MAXFEED_RW220_DEFAULT = -1;

        public const string regPRINTER_SETFF_SKIPLENGTH_RW220 = "PRINTER_SETFF_SKIPLENGTH_RW220";
        public const int regPRINTER_SETFF_SKIPLENGTH_RW220_DEFAULT = -1;

        public const string regPRINTER_SPEED_RW220 = "PRINTER_SPEED_RW220";
        public const int regPRINTER_SPEED_RW220_DEFAULT = 4;


        ///////////////////////////////////////////
        public const string regEXTRA_DOT_LINES_ZQ510_PREFEED = "EXTRA_DOT_LINES_ZQ510_PREFEED";
        public const int regEXTRA_DOT_LINES_ZQ510_PREFEED_DEFAULT = 162;

        public const string regEXTRA_DOT_LINES_ZQ510_POSTFEED = "EXTRA_DOT_LINES_ZQ510_POSTFEED";
        public const int regEXTRA_DOT_LINES_ZQ510_POSTFEED_DEFAULT = 30;

        public const string regPRINT_TONE_ZQ510 = "PRINT_TONE_ZQ510";
        public const int regPRINT_TONE_ZQ510_DEFAULT = 75;

        public const string regPRINTER_BAR_SENSE_SENSITIVITY_ZQ510 = "PRINTER_BAR_SENSE_SENSITIVITY_ZQ510";
        public const int regPRINTER_BAR_SENSE_SENSITIVITY_ZQ510_DEFAULT = -1;

        public const string regPRINTER_BAR_SENSE_IGNORE_ZQ510 = "PRINTER_BAR_SENSE_IGNORE_ZQ510";
        public const int regPRINTER_BAR_SENSE_IGNORE_ZQ510_DEFAULT = 0;

        public const string regPRINTER_JOURNAL_MODE_ZQ510 = "PRINTER_JOURNAL_MODE_ZQ510";
        public const int regPRINTER_JOURNAL_MODE_ZQ510_DEFAULT = -1;

        public const string regPRINTER_SETFF_MAXFEED_ZQ510 = "PRINTER_SETFF_MAXFEED_ZQ510";
        public const int regPRINTER_SETFF_MAXFEED_ZQ510_DEFAULT = -1;

        public const string regPRINTER_SETFF_SKIPLENGTH_ZQ510 = "PRINTER_SETFF_SKIPLENGTH_ZQ510";
        public const int regPRINTER_SETFF_SKIPLENGTH_ZQ510_DEFAULT = -1;

        public const string regPRINTER_SPEED_ZQ510 = "PRINTER_SPEED_ZQ510";
        public const int regPRINTER_SPEED_ZQ510_DEFAULT = 4;
        //

        //////////////////////////////////////////

        public const string regEXTRA_DOT_LINES_ZQ520_PREFEED = "EXTRA_DOT_LINES_ZQ520_PREFEED";
        public const int regEXTRA_DOT_LINES_ZQ520_PREFEED_DEFAULT = 162;

        public const string regEXTRA_DOT_LINES_ZQ520_POSTFEED = "EXTRA_DOT_LINES_ZQ520_POSTFEED";
        public const int regEXTRA_DOT_LINES_ZQ520_POSTFEED_DEFAULT = 30;

        public const string regPRINT_TONE_ZQ520 = "PRINT_TONE_ZQ520";
        public const int regPRINT_TONE_ZQ520_DEFAULT = 75;

        public const string regPRINTER_BAR_SENSE_SENSITIVITY_ZQ520 = "PRINTER_BAR_SENSE_SENSITIVITY_ZQ520";
        public const int regPRINTER_BAR_SENSE_SENSITIVITY_ZQ520_DEFAULT = -1;

        public const string regPRINTER_BAR_SENSE_IGNORE_ZQ520 = "PRINTER_BAR_SENSE_IGNORE_ZQ520";
        public const int regPRINTER_BAR_SENSE_IGNORE_ZQ520_DEFAULT = 0;

        public const string regPRINTER_JOURNAL_MODE_ZQ520 = "PRINTER_JOURNAL_MODE_ZQ520";
        public const int regPRINTER_JOURNAL_MODE_ZQ520_DEFAULT = -1;

        public const string regPRINTER_SETFF_MAXFEED_ZQ520 = "PRINTER_SETFF_MAXFEED_ZQ520";
        public const int regPRINTER_SETFF_MAXFEED_ZQ520_DEFAULT = -1;

        public const string regPRINTER_SETFF_SKIPLENGTH_ZQ520 = "PRINTER_SETFF_SKIPLENGTH_ZQ520";
        public const int regPRINTER_SETFF_SKIPLENGTH_ZQ520_DEFAULT = -1;

        public const string regPRINTER_SPEED_ZQ520 = "PRINTER_SPEED_ZQ520";
        public const int regPRINTER_SPEED_ZQ520_DEFAULT = 4;


        //////////////////////////////////////////

        public const string regEXTRA_DOT_LINES_FIELDPRO530_PREFEED = "EXTRA_DOT_LINES_FIELDPRO530_PREFEED";
        public const int regEXTRA_DOT_LINES_FIELDPRO530_PREFEED_DEFAULT = 162;

        public const string regEXTRA_DOT_LINES_FIELDPRO530_POSTFEED = "EXTRA_DOT_LINES_FIELDPRO530_POSTFEED";
        public const int regEXTRA_DOT_LINES_FIELDPRO530_POSTFEED_DEFAULT = 30;

        public const string regPRINT_TONE_FIELDPRO530 = "PRINT_TONE_FIELDPRO530";
        public const int regPRINT_TONE_FIELDPRO530_DEFAULT = 75;

        public const string regPRINTER_BAR_SENSE_SENSITIVITY_FIELDPRO530 = "PRINTER_BAR_SENSE_SENSITIVITY_FIELDPRO530";
        public const int regPRINTER_BAR_SENSE_SENSITIVITY_FIELDPRO530_DEFAULT = -1;

        public const string regPRINTER_BAR_SENSE_IGNORE_FIELDPRO530 = "PRINTER_BAR_SENSE_IGNORE_FIELDPRO530";
        public const int regPRINTER_BAR_SENSE_IGNORE_FIELDPRO530_DEFAULT = 0;

        public const string regPRINTER_JOURNAL_MODE_FIELDPRO530 = "PRINTER_JOURNAL_MODE_FIELDPRO530";
        public const int regPRINTER_JOURNAL_MODE_FIELDPRO530_DEFAULT = -1;

        public const string regPRINTER_SETFF_MAXFEED_FIELDPRO530 = "PRINTER_SETFF_MAXFEED_FIELDPRO530";
        public const int regPRINTER_SETFF_MAXFEED_FIELDPRO530_DEFAULT = -1;

        public const string regPRINTER_SETFF_SKIPLENGTH_FIELDPRO530 = "PRINTER_SETFF_SKIPLENGTH_FIELDPRO530";
        public const int regPRINTER_SETFF_SKIPLENGTH_FIELDPRO530_DEFAULT = -1;

        public const string regPRINTER_SPEED_FIELDPRO530 = "PRINTER_SPEED_FIELDPRO530";
        public const int regPRINTER_SPEED_FIELDPRO530_DEFAULT = 4;



        ///////////////////////////////////////////



        public const string regEXTRA_DOT_LINES_PB31_PREFEED = "EXTRA_DOT_LINES_PB31_PREFEED";
        public const int regEXTRA_DOT_LINES_PB31_PREFEED_DEFAULT = 162;

        public const string regEXTRA_DOT_LINES_PB31_POSTFEED = "EXTRA_DOT_LINES_PB31_POSTFEED";
        public const int regEXTRA_DOT_LINES_PB31_POSTFEED_DEFAULT = 30;

        public const string regPRINT_TONE_PB31 = "PRINT_TONE_PB31";
        public const int regPRINT_TONE_PB31_DEFAULT = 1;


        // used to decide when a multi-page document is being printed 
        // and sensor marks have to be skipped over
        public const string regPAGE_HEIGHT_DOT_LINES_PB31 = "PAGE_HEIGHT_DOT_LINES_PB31";
        public const int regPAGE_HEIGHT_DOT_LINES_PB31_DEFAULT = 880;



        public const string regEXTRA_DOT_LINES_PB3_PREFEED = "EXTRA_DOT_LINES_PB3_PREFEED";
        public const int regEXTRA_DOT_LINES_PB3_PREFEED_DEFAULT = 162;

        public const string regEXTRA_DOT_LINES_PB3_POSTFEED = "EXTRA_DOT_LINES_PB3_POSTFEED";
        public const int regEXTRA_DOT_LINES_PB3_POSTFEED_DEFAULT = 30;

        public const string regPRINT_TONE_PB3 = "PRINT_TONE_PB3";
        public const int regPRINT_TONE_PB3_DEFAULT = 1;

        public const string regPAGE_HEIGHT_DOT_LINES_PB3 = "PAGE_HEIGHT_DOT_LINES_PB3";
        public const int regPAGE_HEIGHT_DOT_LINES_PB3_DEFAULT = 880;




        public const string regEXTRA_DOT_LINES_SPPR200_PREFEED = "EXTRA_DOT_LINES_SPPR200_PREFEED";
        public const int regEXTRA_DOT_LINES_SPPR200_PREFEED_DEFAULT = 300; //400;

        public const string regEXTRA_DOT_LINES_SPPR200_POSTFEED = "EXTRA_DOT_LINES_SPPR200_POSTFEED";
        public const int regEXTRA_DOT_LINES_SPPR200_POSTFEED_DEFAULT = 220;

        public const string regPRINT_TONE_SPPR200 = "PRINT_TONE_SPPR200";
        public const int regPRINT_TONE_SPPR200_DEFAULT = 1;

        public const string regPAGE_HEIGHT_DOT_LINES_SPPR200 = "PAGE_HEIGHT_DOT_LINES_SPPR200";
        public const int regPAGE_HEIGHT_DOT_LINES_SPPR200_DEFAULT = 880;



        public const string regEXTRA_DOT_LINES_SPPR300_PREFEED = "EXTRA_DOT_LINES_SPPR300_PREFEED";
        public const int regEXTRA_DOT_LINES_SPPR300_PREFEED_DEFAULT = 225;

        public const string regEXTRA_DOT_LINES_SPPR300_POSTFEED = "EXTRA_DOT_LINES_SPPR300_POSTFEED";
        public const int regEXTRA_DOT_LINES_SPPR300_POSTFEED_DEFAULT = 0;

        public const string regPRINT_TONE_SPPR300 = "PRINT_TONE_SPPR300";
        public const int regPRINT_TONE_SPPR300_DEFAULT = 1;

        public const string regPAGE_HEIGHT_DOT_LINES_SPPR300 = "PAGE_HEIGHT_DOT_LINES_SPPR300";
        public const int regPAGE_HEIGHT_DOT_LINES_SPPR300_DEFAULT = 880;

        #endregion


        public const string regINACTIVITY_TIMEOUT_MIN = "INACTIVITY_TIMEOUT_MIN";   // this is for the password dialog requiring re-login
        public const int regINACTIVITY_TIMEOUT_MIN_DEFAULT = 1;    // <= = 0; means no timeout

        public const string regDISABLE_CAMERA = "DISABLE_CAMERA";                    //  check to see if the camera application should be shut down
        public const int regDISABLE_CAMERA_DEFAULT = 0;						   // once disabled, unit must be reset (or camera app launched manually) to re-enable

        public const string regDISABLE_CAMERA_AUDIO = "DISABLE_CAMERA_AUDIO";        //  check to see if the camera sounds should be suppressed	
        public const int regDISABLE_CAMERA_AUDIO_DEFAULT = 1;					   // once disabled, unit must be reset (after registry update) to re-enable

        public const string regCONFIRM_CAMERA_PHOTOS = "CONFIRM_CAMERA_PHOTOS";		// check to see if snapshots should be confirmed before they are saved
        public const int regCONFIRM_CAMERA_PHOTOS_DEFAULT = 0;					    // once enabled, unit must be reset (after registry update) to re-enable

        public const string regMULTI_CAMERA_IMAGES_MODE = "MULTI_CAMERA_IMAGES_MODE";	 // Enable multi images camera mode for windows 6.x devices. 
        public const int regMULTI_CAMERA_IMAGES_MODE_DEFAULT = 0;					 // Default is disabling the multi images mode which means single image mode is active.


        public const string regCAMERA_3MP_IMAGE_SIZE = "CAMERA_3MP_IMAGE_SIZE";				// initial size for camera snapshot images

        public const string regCAMERA_IMAGE_SIZE_640x480 = "CAMERA_IMAGE_SIZE_640x480";  		// size options for camera
        public const string regCAMERA_IMAGE_SIZE_800x600 = "CAMERA_IMAGE_SIZE_800x600";
        public const string regCAMERA_IMAGE_SIZE_1024x768 = "CAMERA_IMAGE_SIZE_1024x768";		// Only 3MP supports images other than 640x480
        public const string regCAMERA_IMAGE_SIZE_2048x1536 = "CAMERA_IMAGE_SIZE_2048x1536";

        public const string regCAMERA_3MP_IMAGE_SIZE_DEFAULT = regCAMERA_IMAGE_SIZE_1024x768;		// default image size - only 3MP camera supports


        public const string regCAMERA_JPG_QUALITY_640x480 = "CAMERA_JPG_QUALITY_640x480";  		// JPG quality settings for camera snapshot images
        public const string regCAMERA_JPG_QUALITY_800x600 = "CAMERA_JPG_QUALITY_800x600";
        public const string regCAMERA_JPG_QUALITY_1024x768 = "CAMERA_JPG_QUALITY_1024x768";
        public const string regCAMERA_JPG_QUALITY_2048x1536 = "CAMERA_JPG_QUALITY_2048x1536";

        public const string regCAMERA_3MP_ENABLE_ERROR_MSG_KEY = "CAMERA_3MP_ENABLE_ERROR_MESSAGE";  //Enable or disable the display of the error msg box.
        public const string regCAMERA_3MP_CAPTURE_TRIAL_NUMBER_KEY = "CAMERA_3MP_CAPTURE_TRIAL_NUMBER";  //Number of trying to capture an image when it fails
        public const string regCAMERA_3MP_CAPTURE_DRIVER_TRIAL_NUMBER_KEY = "CAMERA_3MP_DRIVER_TRIAL_NUMBER";  //Number of trying to capture an image by the driver


        public const int regCAMERA_JPG_QUALITY_640x480_DEFAULT = 95;						// range from 1; to 1;00, higher number = higher quality
        public const int regCAMERA_JPG_QUALITY_800x600_DEFAULT = 95;
        public const int regCAMERA_JPG_QUALITY_1024x768_DEFAULT = 93;
        public const int regCAMERA_JPG_QUALITY_2048x1536_DEFAULT = 90;


        public const string regCAMERA_3MP_DISABLE_USER_SIZE_SELECTION = "CAMERA_3MP_DISABLE_USER_SIZE_SELECTION";  // determines if camera imaging sizing options are available to user
        public const int regCAMERA_3MP_DISABLE_USER_SIZE_SELECTION_DEFAULT = 0;				//


        //Registry items related to LPR (License Plate Recognition)
        public const string regLPR_SERVER_URL = "LPR_SERVER_URL";
        public const string regLPR_SERVER_URL_DEFAULT = @"http://64.132.70.3";
        public const string regLPR_SERVER_PORT = "LPR_SERVER_PORT";
        public const int regLPR_SERVER_PORT_DEFAULT = 8000;
        public const string regLPR_WEBSERVICE_CUSTID = "LPR_WEBSERVICE_CUSTID";
        public const string regLPR_WEBSERVICE_CUSTID_DEFAULT = "4176";  // New Orleans     // default to New Orleans if not defined

        public const string regLPR_CROPPPING_MARGIN_WIDTH = "LPR_CROPPPING_MARGIN_WIDTH";
        public const int regLPR_CROPPPING_MARGIN_WIDTH_DEFAULT = 0; //20;
        public const string regLPR_CROPPPING_MARGIN_HEIGHT = "LPR_CROPPPING_MARGIN_HEIGHT";
        public const int regLPR_CROPPPING_MARGIN_HEIGHT_DEFAULT = 0; //10;

        //Registry items for test LPR
        public const string regLPR_TEST_ALTERNATE_URL = "LPR_TEST_ALTERNATE_URL";
        public const int regLPR_TEST_ALTERNATE_URL_DEFAULT = 0; //value = 0 (original URL is used), value > 0 (Test URL will be used)
        public const string regLPR_TEST_SERVER_URL = "LPR_TEST_SERVER_URL";
        public const string regLPR_TEST_SERVER_URL_DEFAULT = @"http://106.51.39.214";
        public const string regLPR_TEST_SERVER_PORT = "LPR_TEST_SERVER_PORT";
        public const int regLPR_TEST_SERVER_PORT_DEFAULT = 8000;
        public const string regLPR_TEST_USE_BUILTIN_CAMERA = "LPR_TEST_USE_BUILTIN_CAMERA";
        public const int regLPR_TEST_USE_BUILTIN_CAMERA_DEFAULT = 0; //value = 0 (LPR module is used), value > 0 (Built-in camera is used)
        public const string regLPR_TEST_RESIZE_IMAGE = "LPR_TEST_RESIZE_IMAGE";
        public const int regLPR_TEST_RESIZE_IMAGE_DEFAULT = 0; //value = 0 (disable resizing), value > 0 (enable resizing)
        public const string regLPR_TEST_RESIZE_IMAGE_WIDTH = "LPR_TEST_RESIZE_IMAGE_WIDTH";
        public const int regLPR_TEST_RESIZE_IMAGE_WIDTH_DEFAULT = 320;
        public const string regLPR_TEST_RESIZE_IMAGE_HEIGHT = "LPR_TEST_RESIZE_IMAGE_HEIGHT";
        public const int regLPR_TEST_RESIZE_IMAGE_HEIGHT_DEFAULT = 240;

        //OCR LIB to use
        public const string regLPR_IMAGING_LIBRARY = "LPR_IMAGING_LIBRARY";
        public const string regLPR_IMAGING_LIBRARY_NONE = "LPR_IMAGING_LIBRARY_NONE";
        public const string regLPR_IMAGING_LIBRARY_3M = "LPR_IMAGING_LIBRARY_3M";
        public const string regLPR_IMAGING_LIBRARY_ALPR = "LPR_IMAGING_LIBRARY_ALPR";
        public const string regLPR_IMAGING_LIBRARY_DEFAULT = regLPR_IMAGING_LIBRARY_NONE;

        //Action after detecting the lic plate number
        public const string regLPR_IMAGING_ACTION = "LPR_IMAGING_ACTION";
        public const string regLPR_IMAGING_ACTION_NONE = "LPR_IMAGING_ACTION_NONE";
        public const string regLPR_IMAGING_ACTION_KEYBOARD_STUFF = "LPR_IMAGING_ACTION_KEYBOARD_STUFF";     // output from LPR will be stuffed into keyboard buffer, regardless of input focus
        public const string regLPR_IMAGING_ACTION_LICPLATE_STUFF = "LPR_IMAGING_ACTION_LICPLATE_STUFF";     // output from LPR will be placed into the LICPLATE field
        public const string regLPR_IMAGING_ACTION_DIALOG_COMPARE = "LPR_IMAGING_ACTION_DIALOG_COMPARE";     // output from LPR will be displayed in a dialog alongside the value from the LICPLATE field.
        // if different, dialog will display "Values Differ", and display two buttons with the          																		  // different values for user to select to replace value in LICPLATE field. ESC will result in no action
        public const string regLPR_IMAGING_ACTION_DEFAULT = regLPR_IMAGING_ACTION_NONE;

        public const string regLPR_IMAGING_ACTION_DIALOG_COMPARE_CONFIRM_ALWAYS = "LPR_IMAGING_ACTION_DIALOG_COMPARE_CONFIRM_ALWAYS";
        public const int regLPR_IMAGING_ACTION_DIALOG_COMPARE_CONFIRM_ALWAYS_DEFAULT = 1;

        // use this to print QRBarCode instead of BarCode128.... only until layout tool is updated to support QRBarCode explicitly
        public const string regFONTBC128_USE_ALTERNATE_FONTQRCODE = "FONTBC128_USE_ALTERNATE_FONTQRCODE";
        public const int regFONTBC128_USE_ALTERNATE_FONTQRCODE_DEFAULT = 0;


        // for barcodes that don't have identifying characteristics, if the data meets or exceeds this count then it will be interpreted as 2D
        public const string regBARCODE_2D_MIN_CHARACTER_COUNT_THRESHHOLD = "BARCODE_2D_MIN_CHARACTER_COUNT_THRESHHOLD";
        public const int regBARCODE_2D_MIN_CHARACTER_COUNT_THRESHHOLD_DEFAULT = 30;


        // for 18-character barcodes that include the leading "I" import character, we can strip them if they want
        // we know that "I" in the first index of a 17-digit barcode indicates "China"
        // but if it is 18-characters than this is a variant of the VIN standard.
        // as this affects all US customers, we will set default behaviour to look for and remove from 18 character vins
        public const string regBARCODE_1D_VIN_REMOVE_LEADING_IMPORT_CHARACTER = "BARCODE_1D_VIN_REMOVE_LEADING_IMPORT_CHARACTER";
        public const int VIN_STRIP_LEADING_I = 1;
        public const int regBARCODE_1D_VIN_REMOVE_LEADING_IMPORT_CHARACTER_DEFAULT = VIN_STRIP_LEADING_I; //strip leading "I" on 18 character VINs

		//Error handling registry
        public const string regERROR_HANDLING_ENABLE = "ERROR_HANDLING_ENABLE";
        public const int regERROR_HANDLING_ENABLE_DEFAULT = 1;  //1 = Enable, 0 = Disable

        public const string regERROR_HANDLING_LEVEL = "ERROR_HANDLING_LEVEL";
        public const int regERROR_HANDLING_LEVEL_DEFAULT = 2;   //0 = all levels, 1 = Minors and up, 2 = Majors and up, 3 = Criticals only.

        public const string regERROR_HANDLING_EMAIL = "ERROR_HANDLING_EMAIL";
        public const string regERROR_HANDLING_EMAIL_DEFAULT = "ayman.s@emqos.com;AWright@civicsmart.com;TZimmerman@civicsmart.com;SSomanchi@civicsmart.com;JGarcia@civicsmart.com;mrgprasad@civicsmart.com";

        //File cleaning up service.
        public const string regCLEANINGUP_SERVICE_FILETYPES = "CLEANINGUP_SERVICE_FILETYPES";
        public const string regCLEANINGUP_SERVICE_FILETYPES_DEFAULT = "log;jpg;bmp;pcl";   //list of file's types to be removed
        public const string regCLEANINGUP_SERVICE_DIRECTORIES = "CLEANINGUP_SERVICE_DIRECTORIES";
        public const string regCLEANINGUP_SERVICE_DIRECTORIES_DEFAULT = "Download;Pictures"; // the value can include more than one standard Android directory e.g. "Download;Pictures;Documents;Audible;data;DCIM;Music;Alarms;Movies;"
        public const string regCLEANINGUP_SERVICE_INTERVAL_HOURS = "CLEANINGUP_SERVICE_INTERVAL_HOURS";
        public const int regCLEANINGUP_SERVICE_INTERVAL_HOURS_DEFAULT = 2;   //hours

        //public const string regCLEANINGUP_SERVICE_FILE_OLD_DAYS = "CLEANINGUP_SERVICE_FILE_OLD_DAYS";
        //public const int regCLEANINGUP_SERVICE_FILE_OLD_DAYS_DEFAULT = 1;  //7 days

        public const string regCLEANINGUP_SERVICE_FILE_OLD_HOURS = "CLEANINGUP_SERVICE_FILE_OLD_HOURS";
        public const int regCLEANINGUP_SERVICE_FILE_OLD_HOURS_DEFAULT = 24;  //24 hours days


        // mem watcher restart service
        public const string regAPP_REFRESH_TICKET_INTERVAL = "APP_REFRESH_TICKET_INTERVAL";
        public const int regAPP_REFRESH_TICKET_INTERVAL_DEFAULT = 15;  // 0 = disabled

        public const string regAPP_REFRESH_REASON_TITLE = "APP_REFRESH_REASON_TITLE";
        public const string regAPP_REFRESH_REASON_TITLE_DEFAULT = "Refresh Needed";

        public const string regAPP_REFRESH_REASON_TEXT = "APP_REFRESH_REASON_TEXT";
        public const string regAPP_REFRESH_REASON_TEXT_DEFAULT = "Press OK to sync with server. This will only take a minute.";

        public const string regAPPLICATION_CLEAR_CACHE_INTERVAL_MIN = "APPLICATION_CLEAR_CACHE_INTERVAL_MIN";
        public const int regAPPLICATION_CLEAR_CACHE_INTERVAL_MIN_DEFAULT = 5; //minutes


        public const string regENFORCEMENT_SERVICE_CONFIRMATION_URL = "ENFORCEMENT_SERVICE_CONFIRMATION_URL";
        public const string regENFORCEMENT_SERVICE_CONFIRMATION_URL_DEFAULT = @"http://http://64.132.70.129/CitServ/citationsmiamiService.svc";





        protected int FindEntry(string iSection, string iItem)
        {
            // find the item
            int loNdx;
            TRegEntry loRegEntry;
            for (loNdx = 0; loNdx < fRegEntries.Count; loNdx++)
            {
                loRegEntry = fRegEntries[loNdx];

                // this needs to be case-insensitive comparison
                //if ((loRegEntry.Section.Equals(iSection)) && (loRegEntry.Item.Equals(iItem)))
                //    return loNdx;
                if (
                    (String.Equals(loRegEntry.Section, iSection, StringComparison.OrdinalIgnoreCase))
                    &&
                    (String.Equals(loRegEntry.Item, iItem, StringComparison.OrdinalIgnoreCase))
                    )
                {
                    return loNdx;
                }



            }
            return -1;
        }

        public int ReadInRegistry(string iRegistryFileNamePath)
        {
            TRegEntry loRegEntry;

            try
            {

                // clear out the current entries.
                fRegEntries.Clear();

                if (File.Exists(iRegistryFileNamePath) == false)
                {
                    // nothing to read
                    return 0;
                }


                // read in the registry
                string[] loRegistryFileLines = File.ReadAllLines(iRegistryFileNamePath);

                // parse them into registry objects
                foreach (string oneLine in loRegistryFileLines)
                {
                    // only non-blank lines 
                    if (oneLine.Trim().Length > 0)
                    {
                        string[] oneLineSplit = oneLine.Split('\t');

                        // we only process specific file formats
                        if (oneLineSplit.GetLongLength(0) == 3)
                        {

                            loRegEntry = new TRegEntry();
                            loRegEntry.Section = oneLineSplit[0];
                            loRegEntry.Item = oneLineSplit[1];
                            loRegEntry.Value = oneLineSplit[2];

                            fRegEntries.Add(loRegEntry);
                        }
                    }
                }


            }
            catch (Exception exp)
            {
                Log.Debug("Exception in ReadInRegistry", exp.Message);
                return -1;
            }


            return 0;
        }


        public int ReadInRegistry(byte[] iRegistryFileData)
        {
            TRegEntry loRegEntry;

            try
            {
                // clear out the current entries.
                fRegEntries.Clear();

                if (iRegistryFileData == null)
                {
                    // nothing to read
                    return 0;
                }

        

                // read in the registry
                List<string> loRegistryFileLinesCollection = new List<string>();
                StreamReader loRegistryFileStream = new StreamReader(new MemoryStream(iRegistryFileData));
                while (loRegistryFileStream.EndOfStream == false)
                {
                    loRegistryFileLinesCollection.Add(loRegistryFileStream.ReadLine());
                }
                loRegistryFileStream.Close();


                // parse them into registry objects
                foreach (string oneLine in loRegistryFileLinesCollection)
                {
                    // only non-blank lines 
                    if (oneLine.Trim().Length > 0)
                    {
                        string[] oneLineSplit = oneLine.Split('\t');

                        // we only process specific file formats
                        if (oneLineSplit.GetLongLength(0) == 3)
                        {

                            loRegEntry = new TRegEntry();
                            loRegEntry.Section = oneLineSplit[0];
                            loRegEntry.Item = oneLineSplit[1];
                            loRegEntry.Value = oneLineSplit[2];

                            fRegEntries.Add(loRegEntry);
                        }
                    }
                }


            }
            catch (Exception exp)
            {
                Log.Debug("Exception in ReadInRegistry", exp.Message);
                return -1;
            }


            return 0;
        }

        public string GetValue(string iSection, string iItem)
        {
            // find the item
            int loNdx;
            TRegEntry loRegEntry;

            if ((loNdx = FindEntry(iSection, iItem)) < 0)
            {
                // send expected misses to debug out
                if (iSection.Equals(regSECTION_ISSUE_AP))
                {
                    Debug.WriteLine("Expected registry item not found: " + iSection + " " + iItem);
                }
                return string.Empty;
            }

            loRegEntry = fRegEntries[loNdx];
            return loRegEntry.Value;
        }


        public string GetRegistryValue(string iSection, string iItem, string iDefaultValue)
        {
            // find the item
            int loNdx;
            TRegEntry loRegEntry;
            string loStringValue;

            // if its not found, return the default
            if ((loNdx = FindEntry(iSection, iItem)) < 0)
            {
                return iDefaultValue;
            }

            loRegEntry = fRegEntries[loNdx];
            return loRegEntry.Value;
        }


        public int GetRegistryValueAsInt(string iSection, string iItem, int iDefaultValue)
        {
            string loStringValue;

            loStringValue = GetValue(iSection, iItem);
            if (loStringValue == "")
                return iDefaultValue;
            try
            {
                int loInt = Convert.ToInt32(loStringValue);
                return loInt;
            }
            catch
            {
                return iDefaultValue;
            }
        }

        public UInt64 GetRegistryValueAsUInt64(string iSection, string iItem, UInt64 iDefaultValue)
        {
            string loStringValue;

            loStringValue = GetValue(iSection, iItem);
            if (loStringValue == "")
                return iDefaultValue;
            try
            {
                UInt64 loInt = Convert.ToUInt64(loStringValue);
                return loInt;
            }
            catch
            {
                return iDefaultValue;
            }
        }


        public string ParseAndGetValue(string iSrcString)
        {
            // needs to be case insensitive
            if (iSrcString.ToUpper().StartsWith("*REG*"))
            //if (iSrcString.StartsWith("*REG*"))
            {
                string loRegSectionName = iSrcString.Remove(0, 5);
                string loRegItemName = loRegSectionName;
                int loIdx = loRegSectionName.IndexOf(" ");
                loRegSectionName = loRegSectionName.Substring(0, loIdx);
                loRegItemName = loRegItemName.Remove(0, loIdx + 1);
                return GetValue(loRegSectionName, loRegItemName);
            }
            else
                return "";
        }

        //public static void InitRegistry(string iRegistryFileNamePath)
        //{
        //    if (glRegistry == null)
        //    {
        //        glRegistry = new TTRegistry();
        //    }

        //    glRegistry.ReadInRegistry(iRegistryFileNamePath);


        //    // Set the static variable used for OCRExtraDotsWidth in the print picture routines
        //    Reino.ClientConfig.TWinPrnBase.OCRExtraDotsWidth = glRegistry.GetRegistryValueAsInt(regSECTION_ISSUE_AP, regEXTRA_OCR_DOT_WIDTH, regEXTRA_OCR_DOT_WIDTH_DEFAULT );
        //}

        public static void InitRegistry(byte[] iRegistryFileData)
        {
            if (glRegistry == null)
            {
                glRegistry = new TTRegistry();
            }

            glRegistry.ReadInRegistry(iRegistryFileData);


            // Set the static variable used for OCRExtraDotsWidth in the print picture routines
            Reino.ClientConfig.TWinPrnBase.OCRExtraDotsWidth = glRegistry.GetRegistryValueAsInt(regSECTION_ISSUE_AP, regEXTRA_OCR_DOT_WIDTH, regEXTRA_OCR_DOT_WIDTH_DEFAULT);
        }

    }
}