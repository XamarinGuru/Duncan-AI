// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 1/06/14 10:57a $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Host/AutoISSUE/DBConstants.cs $
//              Revision: $Revision: 529 $ 

#region Recent Revision History
/****************************************
 * 
 *     |---------------------------------------------------------------------------------------------------------------
 *     | SUPPORT NOTE: If Oracle client is installed manually, the ORACLE_HOME folder may not have                    |
 *     |     the correct permission settings to allow the ASP.NET process to utilize the Oracle client DLLs.          |
 *     |     This will manifest itself as an exception stating                                                        |
 *     |     "System.Data.OracleClient requires Oracle client software version 8.1.7 or greater."                     |
 *     |     It can be resolved by setting the permissions manually. See http://www.ureader.com/message/1268508.aspx  |
 *     |     or Google "csharp System.Data.OracleClient requires Oracle client software version 8.1.7"                |
 *     |---------------------------------------------------------------------------------------------------------------
 * 
 * RECENT REVISION HISTORY:
 *  
 *      - TODO - all AutoTRAX reports need to be updated to display the credit card data
 *  
 *  v2.45.0 (In Development)
 *  
 *  v2.44.1 (In QA 2014-01-06)
 *      - Bug Fix: Updated support for newer MC75 PDAs with Windows Mobile 6.5
 *  
 *  v2.44.0 (Seattle Demo)
 *      - New Feature: Added device-specific "Meter Provider Name Override" in the Range Manager so individual devices can have a different
 *        wireless meter enforcement provider than specified in the the platform-wide handheld registry files (Implemented to support the Seattle Demo)
 *  
 *  v2.43.1 (In QA 2013-12-16)
 *      - Change: Custom XML export for Stanislaus County was updated to include new and/or changed fields to match their current layout
 *  
 *  v2.43.0 (In QA 2013-12-05)
 *      - New Feature: Wireless hotsheet searches will automatically perform a "make composite list" process for the hotsheet structure that is about to be searched
 *        (Also added in branch version 2.42.1)
 *      - Enhancement: Updated Damaged Sign and Meter Status reports to handle larger status descriptions
 *        (Also added in branch version 2.42.2)
 *      - New Feature: Added support for multivendor meter enforcement (Initial release limited to support for Atlanta's mixed system: Duncan, Parkeon and ParkMobile)
 *      - New Feature: Configurations used by the AI.NET Public Webservice (ACDI) can be reloaded by browsing to a link, such as this example:
 *        http://LOCALHOST/REINOWEBSERVICES/AUTOISSUEPUBLIC/ReloadConfiguration.aspx
 *        (This reduces the need to explicitly restart IIS when ACDI configs or meter list files are changed)
 *  
 *  v2.42.2 (In QA 2013-10-29)
 *      - Enhancement: Updated Damaged Sign and Meter Status reports to handle larger status descriptions
 *  
 *  v2.42.1 (In QA 2013-10-21)
 *      - New Feature: Wireless hotsheet searches will automatically perform a "make composite list" process for the hotsheet structure that is about to be searched
 *  
 *  v2.42.0 (In QA 2013-09-27)
 *      - New Feature: Custom XML court export for Stanislaus County (only applicable to Stanislaus County)
 *      - Bug Fix: "Object reference not set to an instance of an object" error during login (Introduced in v2.41 with the Abandoned Vehicle reports)
 *  
 *  v2.41.0 (In QA 2013-09-13)
 *      - New Feature: Abandoned Vehicle Reports (Custom work -- only applicable to City of Renton)
 *  
 *  v2.40.0 (In QA 2013-08-08)
 *      - Enhancement: [AUTOCITE-407] Configurable filenaming convention for multimedia exports (For Green Bay's exports to Cardinal)
 *      - New Feature: Added Watermark applet in synchronization process for Windows Mobile 6 platforms
 *      - New Feature: Added host-side synchronization support for Casio IT-9000 (Platform files for this platform not released yet)
 * 
 *  v2.39.0 (Skipped - to align with HH files released as platform files for v2.39.0)
 *  
 *  v2.38.0 (In QA 2013-06-17)
 *       - New Feature: Added support for "DUNCAN_OVERSTAYVIO" provider in the "GPRS_METER_PROVIDER_NAME" registry item for handheld devices.
 *         This provider is used for Overstay Violation enforcement based on vehicle sensors (without any parking meter), and is only applicable
 *         to PDA devices, in conjunction with the MeterReportMVC website. (Not applicable for AutoCITE X3)
 *       - Bug Fix: [AUTOCITE-398] FTP (not SFTP) transfers to subfolders didn't work, and resulted in a filename consisting of the subfolders plus the destination filename 
 *         (Discovered in Brookline)
 *     
 * 
 *  v2.37.0 (In QA 2013-06-03)
 *       - Enhancement: Extended Pay-by-Phone enforcement module to handle both Verrus and Pango.  Pango will utilize GetVehicleStatus + GetZoneStatus,
 *         and Verrus will continue to only use GetZoneStatus
 * 
 *  v2.36.0 (In QA 2013-05-15)
 *       - No Changes. Companion release for handheld release 3.76
 *  
 *  v2.35.0 (In QA 2013-04-30)
 *       - Enhancement - added support for Intermec CK70/CK71 PDAs
 *       - Change: [AUTOCITE-372] More updates to SDRO export and file transfer support. For backward compatibility, reinstated 
 *         automatic SDRO transfer invocation after exports, but also introduced a "Disable Auto-Transfer to SDRO" option 
 *         which will be used when ZIP files will be uploaded to the SDRO instead of XML files.  Also added a "SDROSupportsZIPFileUploads" 
 *         option to SDRO_Connection.xml configuration file. This new option defaults to "false" for backward compatibility.
 *         When "SDROSupportsZIPFileUploads" is false, the system will upload XML files, and when "SDROSupportsZIPFileUploads" 
 *         is "true", the system will create ZIP files for upload, comprised of XML and JPGs.  Also fixed bug in the creation
 *         of ZIP files for SDRO, where JPGs were omitted from the ZIP if infringement numbers were exported to XML with leading
 *         zeroes due to utilizing smaller infringement number ranges that had less characters than allowed by the field definition. 
 *  
 *  v2.34.0 (In QA 2013-04-09)
 *      - Change: [AUTOCITE-372] Updated SDRO export and file transfer support with ability to create ZIP files comprised of 
 *        infringement XML and associated photos. Removed automatic upload to SDRO after exports and added task/schedule
 *        ability for invoking SDRO uploads (Only applicable to customers in New South Wales)
 *  
 *  v2.33.0 (In QA 2013-03-15)
 *      - Change: [AUTOCITE-364] Extended handheld device communication timeouts to support changes in an experimental 
 *        variant of the AutoCITE X3 operating system that automatically reclaims Flash memory after files are deleted
 *      - Change: [AUTOCITE-378] Added device-specific configurability of DPT StallInfo tokens in the Range Manager. 
 *        Digital Payment Technologies (DPT) advised that they can only handle one simultaneous query per token, so 
 *        the shared token is now deprecated and each handheld should be assigned a unique token. 
 *        (Only effects customers using DPT for meter enforcement provider)
 *        Note: To obtain new StallInfo tokens, contact:
 *              Robert Choquette  |  Senior Technical Product Manager  |  Digital Payment Technologies
 *              robert.choquette@digitalpaytech.com
 *              T. 604.630.8213  |  C. 604.790.0841  |  TF. 888.687.6822 x.244  |  F. 604.629.1867  |  digitalpaytech.com
 *              330-4260 Still Creek Drive  |  Burnaby  |  BC  |  V5C 6C6
 *      - Bug Fix: [AUTOCITE-375] Entering an Issue Time between 12:00 am - 12:59 am is not allowed in the Manual Entry form 
 *        (Discovered in Albuquerque)
 *      - Bug Fix: [AUTOCITE-349] CRLF allowed in Account name field caused corrupt Userstruct.dat file in handhelds, 
 *        which prevented login of accounts to handhelds. (Discovered in Los Angeles)
 * 
 * 
 *  v2.32.0 (In QA 2013-02-28)
 *      - Bug Fix: [AUTOCITE-341] "There is no row at position 0 error" when synching handhelds or recovering data from comm sessions. 
 *        This was caused by an out-of-date unique index of the STATUSHISTORY table. This is a rare situation that happens when 2 officers
 *        void tickets within the same minute of the day.  This issue was originally addressed in AI.NET v1.11 (Jan 2007), however the fix 
 *        only worked properly for Oracle and Firebird database platforms. The only customers that may have this issue are ones that use 
 *        SQL Server and were initially installed with AI.NET v1.10 or below. 
 *        Based on our records:
 *            Customers most likely to be affected: Dunedin, Moreland, Nillumbik
 *            Other customers possibly affected:    Greater Geelong, North Sydney Health (a.k.a Royal North Shore)
 *      - Bug Fix: CommCmdIP for PDA devices could encounter an out-of-memory condition when synching large files. New methods were introduced
 *        in CommCmdIP and AI.NET webservices to correct this problem on PDA devices.
 *  
 * v2.31.0 (In QA 2013-02-22)
 *      - Bug Fix: [AUTOCITE-368] Record inquiries could result in an SQL error message about missing SRCMASTERKEY when the results include
 *        a "Continuance" record. (Discovered in Minneapolis)
 *      - Bug Fix: [AUTOCITE-369] Range Manager freezes when initialising a PDA
 *      - Bug Fix: When a customer is misconfigured with a HotSheet structure declared with a PrimaryKeyFldCnt greater than the actual number of
 *        fields, the list editor would encounter an unhandled exception with text "Index was out of range. Must be non-negative and less than 
 *        the size of the collection".  The code has been updated to gracefully handle these situations.
 *      - Bug Fix: [AUTOCITE-371] Reported as "Concatenate To Next does not appear to work correctly".  The Concatenate To Next functionality in
 *        exports intentionally differs from the one in our legacy product, because that version was considered by many to be faulty.  To avoid
 *        causing problems by reverting to legacy behavior, a new "Concatenate_to_Next_or_Delim" option has been added that functions in the 
 *        legacy manner without adversely affecting customers that currently use "Concatenate_to_Next"
 *  
 * v2.30.1 (In QA 2013-02-05)
 *      - Bug Fix: [AUTOCITE-368] Imports of tickets (via Import definition) attached violation information to the incorrect tickets
 *        (This is rarely used functionality, which is different from imports via device synchronization or recovery from session folders)
 *        (Discovered in Miami Beach, trying to restore historical tickets into a new database)
 *  
 * v2.30.0 (In QA 2013-01-30)
 *      - Bug Fix: [AUTOCITE-362] Adding court definition using AutoISSUE menu will allocate wrong due dates
 *      - Bug Fix: [AUTOCITE-340] Save Parking tickets not being imported into database. (i.e. SAVE_PARKING.DAT that is
 *        temporarily created when a citation is printed on the handheld, but prior to final saving of the ticket)
 *      - Enhancement: Added “Seconds to delay after Start Session” and “Retry at server when there are Non-Comm Meters” 
 *        options to the system configuration screen in the Parkeon meter service provider settings used by AI.NET Public
 *        webservice (ACDI).  Only applicable to customers using Parkeon meter provider for wireless meter enforcement 
 *        (Atlanta and Menlo Park)
 *      - Bug Fix: Added exception handling to the Parkeon-related webservices that log additional information to an SQLite 
 *        database. Only applicable to customers using Parkeon meter provider for wireless meter enforcement 
 *        (Atlanta and Menlo Park)
 *      - Enhancement: Updated the handheld registry editor with more registry item choices and descriptions.
 *      - Bug Fix: Disabled the SQLite database logging support for Parkeon-related meter enforcement activities. The database
 *        logging has an intermittent concurrency deadlock issue that precludes it from being production-ready. Activity
 *        is still logged to the text log files -- it is only the DB logging that is disabled. 
 *        This is only applicable to customers using Parkeon meter provider for wireless meter enforcement (Atlanta and Menlo Park)  
 *      - Bug Fix/Enhancement: Fixed bug in DPT meter enforcement logging where activity for a handheld was logged to a device-specific
 *        log file of a different serial number.  Also added server-side retries when error are encountered when requesting
 *        meter information from DPT's server.  This will minimize the impact to our end-users when DPT servers are experiencing 
 *        intermittent errors. (Only applicable to customers configured with DPT as their wireless meter provider; i.e. Milwaukee)
 *       
 *      Handheld Support Files: (Handheld platform files coordinated with AI.NET host-side release)
 *        - Enhancement: Added additional exception handling, logging, and recovery to the ModemMgr and AIWebProxy applets
 *          used by the AutoCITE X3 and PDAs for wireless interaction with AI.NET Public webservice
 *        - Enhancement: Updated AIWEBPROXY.LOG log file format to include additional useful information such as device 
 *          serial number, meter status request parameters, and information about enforcement session and non-communicating
 *          meters when using the Parkeon meter provider.
 *        - Bug Fix: Added deadlock detection and avoidance to UDP-based modem status queries in the ModemMgr applet used by 
 *          the AutoCITE X3. This is intended to reduce the occasional "crashes" of modem manager applet.
 *        - Bug Fix: Updated ModemMgr and AIWebProxy applet startup procedures to reduce/eliminate the occurrence of the applets
 *          remaining in the foreground upon startup of AutoISSUE and/or wireless capabilities being disabled due to non-responsiveness
 *          of AIWebProxy during startup.
 *  
 * v2.29.1 (In QA 2013-01-11)
 *      - Bug Fix: CommCmdIP (device synch via TCP/IP) for PDA devices didn't support revision changes to the UserStruct
 *        (Discovered in Atlanta config for Motorola MC95xx PDA)
 *  
 * v2.29.0 (In QA 2012-12-17)
 *      - Enhancement: Added a new webmethod to ACDI (AI.NET Public Webservice) to support wireless meter status requests
 *        that conserve on over-the-air bandwidth by significantly reducing the size of the XML payload.
 *  
 * v2.28.0 (In QA 2012-12-10)
 *      - New Feature: Supports ticket reproduction exports that included photo selected by the issuing officer
 *      - Enhancement: Added host synchronization support for Motorola MC95xx PDA (via CommCmdIP)
 *  
 * v2.27.0 (In QA 2012-11-29)
 *      - Bug Fix: Updated the custom DLLs for Somerville's EscalatingFines hotsheet to always include 2-day hit records when
 *        also including 3-day hit records in the Guest Permit Abuse tracking system for Somerville (Customer-specific functionality)
 *  
 * v2.26.0 (In QA 2012-11-02)
 *      - Enhancement: [AUTOCITE-352] Need for larger (scalable) windows for Reports and Citation Range Manager, so more than
 *        4 citation structures can be displayed 
 *      - Bug Fix: [AUTOCITE-355]  Print picture reproductions don't reproduce dates correctly when using non-standard mask 
 *        inside virtual field definitions (Discovered in Brampton)
 *      - Bug Fix: [AUTOCITE-342] Popular list items do not work on tables with version higher then 0
 *      - Bug Fix: [AUTOCITE-356] Mark Mode and Activity Log reports sometimes had improper sorting for date and times
 *      - New Feature: Custom DLLs to generate EscalatingFines hotsheet to support the 2-day and 3-day Guest Permit Abuse tracking 
 *        system for Somerville (Customer-specific functionality)
 * 
 * v2.25.0 (In QA 2012-10-30)
 *      - Enhancement: Added a new "Disposition Code by Officer" report which groups HotSheet dispositions by officer,
 *        and shows counts for hotcodes and disposition statuses at grouping and overall report levels.
 *        (Introduced for Milwaukee)
 *      - Enhancement: Modified AI.NET Public Webservice (ACDI) to suppress sending Parking tickets to real-time AutoPROCESS interface
 *        if the record contains a "SKIP_PRINTING" field with a "Y" value.  This was introduced to prevent sending mark records
 *        used for the Guest Pass Abuse tracking system in Somerville, but it is generic enough in nature to be useful for any client.
 *  
 * v2.24.0 (In Q.A. 2012-09-10)
 *      - Enhancement: Optimized PAM UDP access in ACDI (Public Webservice) so response times are significantly faster
 *        in Minneapolis (Cale implementation), which has large PAM clusters resulting in multi-packet responses from
 *        PAM UDP server.
 *  
 * v2.23.0 (In Q.A. 2012-08-24)
 *      - Bug fix: [AUTOCITE-350] Exports to network folders fail in Windows 7 even when Network Access Credentials are supplied
 *  
 * v2.22.0 (In Q.A. 2012-07-27)
 *      - Change: Changed the XML schema for Pay-by-Phone enforcement API to be a string instead of integer, because 
 *        Verrus enforcement in Miami was failing when the xml contained records for "hotel" spaces.
 *  
 * v2.21.0 (In Q.A. 2012-07-10)
 *      - Enhancement: Extended ACDI logging for Parkeon meter enforcement. Parkeon-related meter enforcement activity
 *        will be logged to SQLite databases in addition to the current text-based log files.  This is for support of
 *        reporting and analysis tools, intended for Operations to monitor the Atlanta system.
 *      - Enhancement: Updated installer to be more compatbile with Windows 7 and Vista so users are less likely to see
 *        the "Program Might Not Have Installed Correctly" message at the end of installation process
 *  
 * v2.20.0 (In Q.A. 2012-06-07)
 *      - Enhancement: Emergency patch for AutoISSUE Public Webservice (ACDI) to mark a list of known bad meters with 
 *        high idle times during meter enforcement to prevent issuance of tickets to spaces/meters that are known to 
 *        have problems.  (Emergency patch for Los Angeles)
 *      - Enhancement: Extended ACDI logging for Parkeon meter enforcement. Parkeon-related meter enforcement activity
 *        will be logged to SQLite databases in addition to the current text-based log files.  This is for support of
 *        future reporting and analysis tools (DB logging is a work in progress; not ready for deployment in Atlanta yet)
 *      - Enhancement: Enhanced exception handling and error logging for UDP-based PAM enforcement. 
 *        (To help pinpoint cause of intermittent problems seen in Los Angeles, such as "Object reference not set to an instance of an object"
 *        messages being logged in the ACDI log files)
 *      - Change: Removed the Cale enforcement logic that applied the oldest meter heartbeat of the entire cluster to all
 *        meters in the same cluster (Minneapolis wants to still enforce on primary spaces if proxy meters are down, so this
 *        logic needed to be removed)
 *      - Bug fix.  Corrected a concurrency issue in PAM meter enforcement (concurrent meter status requests were improperly sharing
 *        resources such as UDP socket and wireless log files)
 *  
 * v2.19.0 (In Q.A. 2012-05-03)
 *      - Enhancement: Introduced "Filter Responses to Space Range" option in PAM enforcement configuration to support special 
 *        enforcement configuration for Cale integration (needed for Minneapolis)
 *      - Bug Fix: MeterStatusRequests to UDP-based PAM were failing if the PAM response was over 8KB
 * 
 * v2.18.0 (In Q.A. 2012-04-03)
 *      - Companion build for new handheld release
 *      - Updated copyright year to 2012
 * 
 * v2.17.0 (In Q.A. 2012-03-13)
 *      - Enhancement: Added configurable grace period to Pay-By-Phone enforcement options (Requested by Ottawa)
 *      - Enhancement: Extended Pay-by-Phone logging in Public webservice (ACDI) to include raw response data from 
 *        PBC vendor, and includes information about any grace period that is applied. (Verrus support for Ottawa)
 *      - Enhancement: PBC_ZONES generation for Pay-By-Phone supports the grave accent ( ` ) used by French zone descriptions
 *        (Verrus support for Ottawa)
 *      - Enhancement: When PBC_ZONES are to be sorted by description instead of numeric Zone ID, duplicate Zone descriptions
 *        will be removed from the final list (Verrus support for Ottawa)
 * 
 * v2.16.0 (In Q.A. 2012-02-14)
 *      - Bug Fix: SQL errors on Public Contact report when using a date range report parameter. (Discovered in Alvin)
 * 
 * v2.15.0 (In Q.A. 2012-02-10)
 *      - Bug Fix: SQL errors on Public Contact report when using an Agency or Beat report parameter. (Discovered in Alvin)
 *      - Bug Fix: Ticket reproduction prints truncated the bottom portion of tickets written of S4 handhelds for Alvin, TX.
 *  
 * v2.14.0 (In Q.A. 2012-02-03)
 *      - Bug Fix: [AUTOCITE-333] Court Calendar screen crashes with error about invalid date
 * 
 * v2.13.0 (In Q.A. 2012-01-31)
 *      - Bug Fix: [AUTOCITE-315] Deleting Task from Scheduler creates an error
 *      - Bug Fix: [AUTOCITE-316] Restore for Tasks & Schedules does not work
 *      - Enhancement: SendFile timeouts for Serial communications have been increased, along with a forced reclaim
 *        when SendFile fails for "*WARN*.DAT" in S3/S4 devices to help alleviate the sporadic comms problems that
 *        are noticed in Arlington, TX
 *      - Enhancement: AI.NET public webservice will limit the length of error message responses sent back to the
 *        handhelds for Pay-by-Space wireless enforcement
 * 
 * v2.12.0 (In Q.A. 2012-01-17)
 *      - Bug Fix: [AUTOCITE-310] Windows 7 doesnt not allow the handhelds to reset on synchronization.
 *      - Bug Fix: [AUTOCITE-313] Date format inconsistency for Officer Log Report
 *      - Bug Fix: [AUTOCITE-314] ACDI logs for Chicago are very large
 * 
 * v2.11.0 (In Q.A. 2012-01-05)
 *      - No changes. Just new revision number to keep in sync with required handheld changes in MFC issue_app v3.54 
 *        (Verrus integration for Ottawa)
 * 
 * v2.10.0 (In Q.A. 2011-12-07)
 *      - Bug Fix: Changed the Verrus Pay-by-Phone support functions to accomodate the latest published Verrus API.
 * 
 * v2.9.0 (In Q.A. 2011-11-23)
 *      - Bug Fix: Los Angeles "Abandoned Vehicle" module (PARKWRKORD) exceeded the 32-bit limit of UniqueKey values.
 *        Changes have been made so less gaps are created in the table's Generator/Sequence, and support has also been
 *        added to handle 64-bit values for UniqueKey and MasterKey fields, when supported in the DB.  In Los Angeles
 *        production DB, these columns are already setup for 64-bit, where they normally are 32-bit.
 * 
 * v2.8.0 (In Q.A. 2011-11-21)
 *      - New Feature: Implemented support for Pay-by-Phone enforcement via Verrus as 3rd party vendor (Developed for Ottawa)
 *      - Bug Fix: Chicago was encountering an occasional OutOfMemory condition when performing a wireless HotSheet search against
 *        their large "PLATELISTSTATEREG" list.  The code has been restructured to consume much less memory for this operation, and
 *        additional checks have been put into place to ensure that log file handles are not left open in the event that an error
 *        does occur in the future.
 *      - Bug Fix: If a record somehow was marked with a table revision higher than what exists in configuration, the record formatting 
 *        table for inquiry result couldn't be created, and resulted in errors that prevented the viewing of any data, and/or crashing the
 *        application.  A fix has been put into place to default the revision to the latest one in configuration if the underlying data
 *        had an invalid revision number associated with it. (Discovered in Alvin)
 *      - Bug Fix: [AUTOCITE-304] During failed downloads, AutoTRAX should delete the MTX file off the handheld to prevent use
 * 
 * v2.7.0 (In Q.A. 2011-11-15)
 *      - No changes. This is a companion release for new handheld versions.
 * 
 * v2.6.1 (In Q.A. 2011-10-14)
 *      - Bug Fix: Fixed ticket reproduction issues for Banyule: BanyuleMod97 was omitting leading zero; Virtual field prefix was 
 *        disregarded if the virtual field used substitution values
 * 
 * v2.6.0 (In Q.A. 2011-10-07)
 *      - Bug Fix: [AUTOCITE-301] HotSheet/XREF List containing virtual fields creates error during list maintenance
 *      - Bug Fix: [AUTOCITE-300] When using the advanced search in AI.NET, the user is unable to type in lower case letters for "Remarks" fields
 *      - Enhancement: Implemented TTableBanyuleMod97VirtualFldDef for Banyule's variation of the Mod97 algorithm
 * 
 * v2.5.0 (In Q.A. 2011-09-23)
 *      - Enhancement: Added support for Liberty Credit Card data in handheld audit records
 *      - Bug Fix: [AUTOCITE-298] SQL Error When Running Meter Repair Analysis Report (AutoTRAX customers only)
 * 
 * v2.4.0 (In Q.A. 2011-09-09)
 *      - Bug Fix: [AUTOCITE-296] Problems with SFTP transfer --> "Error: SFTP component not connected."
 *        Note: Geelong sends files to SFTP server hosted by Tenix, which reports itself as "OpenSSH_5.1p1 Debian-5".
 *              This SFTP server apparently doesn't support requesting attributes for a file (such as the file size)
 *              via the "RequestAttributes" command.  A new SendFile verification was introduced that obtains the file
 *              size via the DirectoryListing command (ls).  The FTP configuration in AI.NET now has an option to specify
 *              the SendFile verificaiton methodology that is used. The choices are: RequestAttributes, DirectoryListing,
 *              and None. The default value is RequestAttributes.  Customers using the Tenix server, such as Geelong, should
 *              be configured with the DirectoryListing option instead of RequestAttributes. Also, the code has been updated
 *              to detect and recover from dropped connections, such as experienced with the Texix SFTP server when calling
 *              the RequestAttributes command.
 *      - Bug Fix: [AUTOCITE-294] Too many hotsheets causes database constraint error in LISTLOG table
 *      - Bug Fix: [AUTOCITE-297] AutoTRAX: Numeric and Date conversion errors with non-English regional settings.
 *        AutoTRAX module fails in various places with messages such as "DataXRefField.ConvertStringDataToObject. Cannot parse 
 *        Number '000000000.00'" or "String was not recognized as a valid DateTime" when the regional settings (in ASP.NET 
 *        and/or Windows Control Panel) is set to a regional language such as French-Canadian or Norwegian-Bokmal. The problem 
 *        is that these languages use commas instead of periods as a decimal place in numeric or currency formats. Also, problems 
 *        can arise from date formats with periods instead of slashes for date separators, or different order of year, month and 
 *        day in the date formats. (Discovered in City of London and Bergen)
 * 
 * v2.3.0 (In Q.A. 2011-08-15)
 *      - New Feature: [AUTOCITE-148] Wireless-Only search property.  Added option in AI.NET global configuration screen to
 *        designate which HotSheet structures should be treated as "Wireless-only" search structures.  Any HotSheets in the 
 *        configured list will NOT be uploaded to the handheld via USB synchronization.  Thus, the only way to get a hit on
 *        that hotsheet is if the handheld has wireless capabilities.
 *      - New Feature: [ENGTASKS-13] Added proxy server support for AI.NET.  
 *        Refer to documentation at: http://jirssb01:8080/jira/secure/attachment/12789/AI.NET+Proxy+Server+Configuration.doc
 *      - Bug Fix: [AUTOCITE-291] Unique Constraint violation occured in "MTX_TRANSTMP" table during import of 
 *        AutoTRAX "FieldTrans.txt" file.   (Only affects AutoTRAX customers)
 *      - Bug Fix: [AUTOCITE-292] L.A. PARKWRKORD_001.DAT file causing import constraint issues (Discovered in Los Angeles)
 *      - Enhancement: Device synchronization screen will check for AutoCITE X3s that were already enumerated on the USB hubs.
 *        This is so devices can be detected for communication if the USB hub reset service is not properly toggling the hub status.
 *      - Enhancement: Implemented a per device (or USB port) counter of communication attempts during one synchronization session.
 *        If the system has unsuccessfully completed communication with a device after 5 tries, the code will ignore that unit for
 *        the rest of the communication session. This will prevent infinite communication cycles when a problematic device is 
 *        attached to the system.
 *      - Bug Fix: Fixed a problem with the "Duncan USB Service" where it was not sucessfully toggling USB hub states on 64-bit machines.
 *      - Bug Fix: Updated installer to not attempt modification of configuration for deprecated webservices.  This will remove error messages
 *        in the Install.log file such as "Node not found in config: /configuration/applicationSettings/AutoISSUE.Properties.Settings/setting[@name='AutoISSUE_Reino_AutoIssueService_AutoIssueService']/value"
 * 
 * v2.2.0 (In Q.A. 2011-07-20)
 *      - Bug Fix: [AUTOCITE-272] Mark Mode Module field in record inquiry is formatted as TIME field (Discovered in Newcastle/Cairns)
 *        Variances in handheld table and entry form configurations can effect the datatype being used for TireStems, as well as
 *        if TireStem info is stored in the Hours, Minutes, or Seconds portion of a DateTime.   The code has been updated to 
 *        account for known variations and display the proper data in the inquiry result and MarkMode reports.
 *      - Bug Fix: The "AutoRun.exe" included in AI.NET installation disks couldn't detect if IIS was installed on Windows 7 64-bit
 *
 * 
 * v2.1.0 (In Q.A. 2011-07-13)
 *      - Bug Fix: The generation of ParkNOW meter cluster lists for the handheld omitted the last record of "METER_CLUSTER.DAT" 
 *        and "METER_IN_CLUSTER.DAT" list files. (Only effects Montogomery County)
 *      - Bug Fix: [AUTOCITE-282] Ticket books get the lower limit set to a number higher than their upper limit when 
 *        freezing/unfreezing ranges or deleting units.
 *      - Enhancement: The AI-to-AP interface now supports the real-time sending void records to AutoPROCESS when they 
 *        arrive wirelessly from handheld devices. As an example on how to configure for use, the applicable record type(s)
 *        for upload to AutoPROCESS would be configured as:  PARKING, PARKVOID
 *        (This functionality was added per CM's request initially to target the needs of San Diego)
 *        NOTE: Test URL for AI-to-AP real time interface is: http://aiapprivatewebservices/APPrivateWebServices/mvB/QA/AutoPROCESSPrivateService.asmx
 *      - Bug Fix: [AUTOCITE-265] AI.NET subconfiguration mode not loading the correct registry
 *      - Enhancement: Added AutoTRAX support for new Duncan Liberty meter
 *      - Bug Fix: [AUTOCITE-286] Password Expiration Policy - Update message when password expires instead of showing an error
 *      - Bug Fix: [AUTOCITE-283] Unable to leave the field blank in VoidStatus for valid tickets in the export definition
 *      - Enhancement: [AUTOCITE-287] A single PDF file should be created when using a PDF printer to print all records in 
 *        a record inquiry result set.  This request was handled by adding a "Print all pages in single job" check-box to 
 *        the print parameter screen, with a default of being checked. 
 * 
 * v2.0.0 (2011-06-24)
 *      - Bug Fix: [AUTOCITE-280] PlateList Hotlist does not sort alphabetically inside the list editor
 *      - Bug Fix: [AUTOCITE-208] Problem with Date/Time comparison when processing master SDRO violation lists can 
 *        cause offences to fall out of the generated handheld list 1 day before they expire. (Only effects clients in NSW)
 *      - New Feature: [AUTOCITE-281] Added ability to launch external processes (such as a Perl script) from a Task Group or Schedule
 *        (Only command-line style processes are launched; Executables with a GUI are not supported, due to a limitation of IIS/ASP.NET)
 *      - Enhancement: Per CM's request, added a "Secondary" destination for the AutoISSUE-to-AutoPROCESS webservice integration that is
 *        used in conjunction with wireless handhelds to deliver ticket data to AutoPROCESS in real-time.  The purpose of this 
 *        "Secondary" destination is to send the data to both the production and test systems, so that CM's test system always is 
 *        up-to-date with a good sample size. The "Secondary" destination has reduced logging, and doesn't include the retry logic
 *        and database updates that is applicable to the "Primary" (Production) destination.
 *      - Bug Fix: Import definitions intended to import Park Notes with image files failed. Support for this functionality has been added,
 *        but there are a few requirements about how the Import Definition is structured. The following fields are required in the definition:
 *          Master Key  --> Requires the "Source is an Issue Number instead of a MasterKey" option to be checked. Data file for import should contain IssueNo in this field
 *          MULTIMEDIANOTEDATA --> Must exist in the Import Definition AND appear prior to MULTIMEDIANOTEFILENAME field. Data file for import can contain dummy data for this field
 *          MULTIMEDIANOTEDATATYPE --> Must exist in the Import Definition AND appear prior to MULTIMEDIANOTEFILENAME field. Data file for import can contain dummy data for this field
 *          MULTIMEDIANOTEFILENAME --> Expects data file for import to contain the filename of associated ".JPG" in this field
 *        
 * 
 * v1.99.0 (2011-06-03)
 *      - Bug Fix: [AUTOCITE-272] Mark Mode Module field in record inquiry is formatted as TIME field (Discovered in Newcastle)
 *      - Bug Fix: [AUTOCITE-278] AutoTRAX Imports of FieldTrans.txt would sometimes encounter an error of "Update failed. 
 *        Row must have been changed by another user" and rollback a transaction, resulting in only a portion of the source 
 *        file to be committed to the database. 
 *      - Bug Fix: [AUTOCITE-276] Traffic Court Dates not being saved in the court calendar (Discovered in Millbrae)
 * 
 * v1.98.0 (2011-05-17)
 *      - Bug Fix: AutoTRAX device synchronization was failing with error messages "String was not recognized as a valid DateTime" 
 *        if the desktop had regional settings with "-" as date separators instead of "/". This was because the routine used to 
 *        retrieve the timestamp that tracks when the last inventory update / modification was executed was affected by the current
 *        Culture info, but was expecting a fixed date format. This routine has now been updated to use an InvariantCulture which
 *        handles various regional settings.
 *      - Bug Fix: [AUTOCITE-273] Error updating ParkNOW meter cluster list (Only effects Montogomery County)
 *      - Bug Fix: Parkeon enforcement was encountering frequent errors such as "session already started for agent, please close it 
 *        before open a new one" and "The request was aborted: The request was canceled"  (Only effects Atlanta)
 *      - Bug Fix: MarkMode report was showing "12" for all front and rear tire stems
 * 
 * v1.97.0 (2011-05-03)
 *      - Bug Fix: Exports configured to only inlcude "Non-warning" records did not find any data if the "ISWARNING" column contained
 *        NULL values instead of "N".  (Discovered in South Miami)
 *      - Bug Fix: Void records were failing to import wirelessly from the AutoCITE X3s when they were configured to do so.
 *      - Enhancement: Added support for 64-bit Windows 7 (x64 is supported, not ia64)
 *      - Enhancement: Fixed various issues with the AI.NET installer on Windows 7 (Doesn't require IIS 6 compatibility module to be installed,
 *        and fixed issues with creating shortcut icons in the program group and desktop for all users)
 *      - Enhancement: Added support for generating meter-related lists based on information downloaded from ParkNOW!.  This option is 
 *        available from the System Options screen, and can also be created as a Task or Schedule. Also, the METER_CLUSTER.DAT and 
 *        METER_IN_CLUSTER.DAT list files that are generated will be sorted by Street name first, then by Space number. The Meter Description
 *        in METER_IN_CLUSTER.DAT list file will also have the street name prior to the space number. (Functionality is for Montgomery County)
 * 
 * 
 * v1.96.0 (2011-04-13)
 *      - Bug Fix: [AUTOCITE-243] Unable to import user roles in AI.NET
 *      - Bug Fix: Restore users from system backup failed without any error.  This primarily happened with Firebird databases when the 
 *        UsersDataset.xml file contained elements declared as "decimal" type instead of "int". The code has been updated to handle the
 *        "decimal" datatype. 
 *      - Bug Fix: OMS "990" exports for Los Angeles were failing when 6-character Beats were introduced into the customer's configuration.
 *        The resolution was to truncate beat data to 5-characters to conform to the enforced max lengths in ACS's file format.
 *        (Only effects Los Angeles)
 *      - Bug Fix: [AUTOCITE-246] The handheld Registry is getting duplicate values after an upgrade is installed
 *      - Bug Fix: [AUTOCITE-261] When trying to import ssm rate programs into the AutoTRAX host software information entered into a single 
 *        record's field is creating multiple records instead of being entered into the original record as it should be. (Only Affects AutoTRAX customers)
 *      - Bug Fix: [AUTOCITE-232] User Lists / Registry Editor Has Navigation / Screen Paint / Event Handler Issues
 *      - Bug Fix: OMS "990" exports encountered an "Object reference not set to an instance of an object" error (Only affects Los Angeles)
 *      - Bug Fix: Oracle error "ORA-00923: FROM keyword not found where expected" was encountered in an export for Los Angeles that was
 *        configured with the "Export to DAT file" configuration option
 * 
 * 
 * v1.95.0 (2011-03-17)
 *      - Enhancement: IPI 2011 Release w/ AutoVu support
 * 
 * v1.94.0 (2011-03-11)
 *     - Bug Fix: [DPCAMIII3-4] Date Time stamp was not added to ticket data recovered manually
 *     - Enhancement: Recompression quality of manually exported image files set to 90% quality (previously was 50%)
 *     - Enhancement: AutoVU support improved; AutoVU code restructured into distinct support classes
 *     - New feature: Added configuration option to force resetting X3 handhelds at each communication session, which is recommended
 *       for clients that regularly use large memory footprints, such as Chicago
 *     - Bug Fix: [AUTOCITE-228] AI.NET scheduler will automatically retry a logon if previous session is expired (which could result from 
 *       Oracle or network being offline for extended time, or by manually ending an active session via the locks manager).  The automatic 
 *       logon retry will only be performed if the scheduler is running in the context of the master user.  Also, during application startup
 *       in the context of "ScheduleAutoRun", startup errors such as unable to communicate with IIS or Database, the system will automatically
 *       retry to establish connection every 10 seconds for 2 minutes.  If it still fails after 2 minutes, the program will completely exit 
 *       rather than leaving an error message on the screen.  This will enable the system monitors in the server environment to detect that
 *       AutoISSUE.NET scheduler is not running.
 *     - Bug Fix: [AUTOCITE-259] The meter outage found activity log report isn't sorting in chronological order
 *     - Bug Fix: [AUTOCITE-78] The "Meter Status Report" and "Meter Status Report by Officer" reports were not available when customer is 
 *       configured for AutoTRAX. These needed to be enabled because some customers use the Broken Meter module in AI.NET in conjuction with
 *       AutoTRAX functionality (Discovered in Atlanta)
 *     - Bug Fix: [AUTOCITE-227] Translation for REISSUED field does not work
 *     - Bug Fix: [AUTOCITE-257] The clients aren't copying over the correct files from the server. The solution was to remove *.ZLIB files from
 *       handheld platform folder during install/upgrade. This forces new ones to be generated with newer timestamps.
 *     - Enhancement: [AUTOCITE-238] "Install.log" should not be overwritten each time. Now the filename will include timestamp. Also the contents
 *       of the file have been extended to include timestamps and installation context information, such as timestamp and file size of the source 
 *       archives.
 *     - Enhancement: [AUTOCITE-237] Added Computer Name column to "Session Activity Report"
 * 
 * 
 * v1.93.0 (2011-02-15)
 *     - Bug Fix: [DPCAMIII3-3] Record Viewer Images not disposed properly, resulting in memory/resource leak
 * 
 * 
 * v1.92.1 HOT FIX for Chicago
 *    - Bug Fix: AutoCITE-245 Undo of AI-247 they need the original filename to know what the rate is. Only AutoISSUE.EXE replaced
 * 
 * 
 * v1.92.0 (2011-02-15)
 *     - Bug Fix: [AUTOCITE-245] Disabled AutoTRAX "holdover" correction logic, no longer needed as source of issue was corrected in handheld
 *     - Bug Fix: [AUTOCITE-250] Violation information was missing from wirelessly imported tickets. This bug was introduced in v1.91.0
 *     - Bug Fix: [AUTOCITE-251] Time-related parameters were showing a popup calendar instead of a UI that is appropriate for changing times
 * 
 * v1.90.1 (2011-02-08)
 *     - Enhancement: Work-around bad 3MP camera files by substituting lo-res thumbnail as attachment data.
 *                    This is a temporary solution until robust JPG validation / re-construction tools can be implemented
 * 
 * v1.91.0 2011-02-02
 *     - Enhancements: Support for ParkNOW wireless space enforcement
 *     - Modification: Realtime exports to AutoPROCESS will send the citation's Issue Number as a formatted string (left-padded with zeros)
 *       instead of as an integer, because AutoPROCESS treats ticket numbers as strings. Also, the AI-to-AP interface now uses CM's "internal"
 *       webservices instead of public webservices, so the configured URL needs to change.
 *       For Milwaukee development/testing, the new URL is: http://www0110207/ReinoWebServices/AutoPROCESSPrivate/AutoPROCESSPrivateService.asmx
 *
 * v1.90.0 2011-01-19
 *      - Bug Fix: [AUTOCITE-121] Inventory updates should be committed immediately to support multiple changes to a location during a single session
 *      - Bug Fix: [AUTOCITE-235] List editor didn't show additonal fields in a HotSheet list that are not in the first table revision of the structure
 *      - Enhancements: Support for larger photo sizes as new 3MP camera comes online for X3, also useful for hi-res pictures from PDA devices
 *          - Hi-res photos detected during X3 unload and timestamp watermark added 
 *          - New functions added to Record Viewer: a “fit-to-screen” function that re-sizes the photo for quick review (default), 
 *              and an “Actual Pixels” tool that displays the photo at original resolution (with scroll bars as needed)
 * 
 * v1.89.0 2010-12-13
 *      - Bug Fix: [AutoCITE-230] Recovery of PDA files does not recover with the serial number. Updated recovery code to look 
 *          for all defined hardware types. Previous code only looked for X3 or Symbol devices for serial number extraction
 *      - Enhancement: [AUTOCITE-32] Automate Import of multiple Eagle Toolbox rate databases into AutoISSUE.NET
 *      - Enhancement: [AUTOCITE-178] More detail in Locks Manager to better determine who/what/where the table/system lock is in effect
 *      - Bug Fix: [AUTOCITE-47] Converted Mechanism Rate Program Filenames can exceed X3 limits
 *      - Enhancement: [AUTOCITE-30] Added AutoTRAX feature to allow import of Lat / Long data to update mechanism location tables.
 *        The import of Lat/Long for mechanisms is invoked via the "GeoCode Inventory" button on the "AutoTRAX" tab of System Options screen.
 *        Clicking the "GeoCode Inventory" button will cause the system to import the Lat/Long information from the "Config\InventoryGeoCode.txt"
 *        file, which is expected to be tab-delimited and following this format:
 *           LocationID<TAB>Latitude<TAB>Longitude<CRLF>
 *      - Enhancement: Added reverse-geocoding functionality to AutoVU-related methods in AI.NET Public web service. When an AutoVU hit 
 *        is received from Genetec, we will reverse-geocode the coordinates to obtain full address information if the address provided
 *        by Genetec is missing the block/street number.
 *      - Enhancement: Added the ability to define export definitions for THotDispoStruct data strutures
 *      - New Feature: Ability to export citations to AutoPROCESS in realtime when issued on a wireless handheld. Also can query AutoPROCESS 
 *        service in real-time when a wireless request is received for a HotSheet/Scofflaw search. The ability to use this functionality 
 *        must be enabled via configurations in the System Options menu. The functionality requires that the URL and credentials for the
 *        AutoPROCESS webservice are configured, as well as the applicable AgencyDesignator that is used by AutoPROCESS to identify the
 *        customer. The configuration also needs to be updated to specify which ticket structures are applicable to be uploaded, as well
 *        as which HotSheet structures are applicable for searches via AutoPROCESS. For development/testing purposes, the following 
 *        configuration options can be used (for Milwaukee test environment):
 *           AutoPROC URL: https://apnet.duncansolutions.com/ReinoWebServices/AutoPROCESSMVBDHost/AutoPROCESSHostService.asmx
 *           The credentials for the above service:
 *               Username: james
 *               Password: AutoISSUE
 *           The agency designator for development/testing is:  MILWA
 *           The applicable record type(s) for upload to AutoPROCESS is:  PARKING
 *           The applicable record type(s) for HotSheet search queries to AutoPROCESS is:  PLATELIST
 *        
 * 
 * v1.88.0 2010-11-23
 *      - Enhancement: AutoTRAX Audit transactions retrieved with X3 handhelds now record the individual coin counts broken out by 
 *          coin type, the number of valid card transactions, and the number of rejected tokens. These additional audit details can 
 *          be viewed by double-clicking on an audit transaction row in the AutoTRAX Inventory screens.
 *      - Bug Fix: Transaction Exception Editor not always disposed properly, would sometimes display an error message when invoking
 * 
 * v1.87.0 2010-11-18
 *      - Bug Fix: (JIRA AUTOCITE-55) Scheduled task duplication.  Added two layers of detection and prevention of concurrent
 *        execution of schedules. First layer is a static collection of Schedule IDs that are currently being run. This collection 
 *        of keys is thread-safe with access controlled via critical sections, and the keys will remain constant even if the 
 *        underlying schedule object is refreshed/reloaded.  The second layer of protection is the usage of an machine-wide
 *        global Semaphore which can be detected/shared from multiple instances of the application on the same machine (even
 *        when run in Citrix sessions). The semaphore takes into account the client name to avoid conflict that may be introduced
 *        in service bureau environments that host more than one client on the same physical machine. Also, the log maintenance
 *        has been isolated into a single-instance separate thread to enhance performance and remove delays that may adversely
 *        effect the thread timings in schedule launcher code.
 *      - Enhancement: (JIRA AUTOCITE-173) Multi-instance detection and prompting for entrance into scheduler. In most circumstances,
 *        only one instance of the scheduler is desired to be running (per client). Before displaying the scheduler form, a machine-wide
 *        global semaphore will be checked to see if the scheduler is already running for the current client on the same machine 
 *        (possibly via another instance of the application).  If such condition is detected, user is prompted and given the choice to
 *        continue and use a 2nd instance of the scheduler form. The semaphore takes into account the client name to avoid conflict 
 *        that may be introduced in service bureau environments that host more than one client on the same physical machine.
 *      - Bug Fix: (JIRA AUTOCITE-61) AI.NET runs out of memory when exporting large amounts of photos or ticket images. Found and
 *        fixed a few GDI resource leaks that occur during the export of ticket reproductions.  This particular issue has never been
 *        reproducible, but resource leaks were identified that theoretically could lead to the "Parameter is not valid" message that
 *        occasionally is observed in Chicago's export summary for ticket reproductions.
 *      - Enhancement: (JIRA AUTOCITE-40) Added grouping/sorting by outage code/description to the AutoTRAX "Meter Outage Report"
 *      - Enhancement: (JIRA AUTOCITE-41) Added a Meter Location ID range parameter for the majority of AutoTRAX reports
 *      - Bug Fix: Corrected web service reference in CommCmdIP to support non-compressed composite files for PDA synchronization
 *      - Bug Fix: The user name, officer name, officer ID, and reino tech key fields in the password editor were defined as "multiline" edit boxes
 *         This could inadvertently lead to embedded CR/LF in values keyed into these fields
 *      - Enhancement: Added AutoTRAX options in system config: Set auto DST for mechanisms that support it; Set RTC adjustment 
 *         for mechs that support it. These and a testing option to force a mech's RTC to a specific date/time were added to
 *         satisfy functional requirements for NY SMM trial
 *              
 * 
 *     
 *
 * v1.86.0 2010-10-28
 *      - Enhancement: Synchronization support for 64MB X3s with multiple AC Flash volumes 
 * 
 * v1.85.0 2010-10-14
 *      - Bug Fix: (JIRA AUTOCITE-213, AUTOCITE-203, AUTOCITE-24) Constraint errors on records preventing download of handhelds
 *        (Discovered in Los Angeles, St Louis, Borough of Indiana, Anchorage, Urbana, University of Delaware) 
 *        Implemented fixes include catching record-level exceptions, logging errors to text file, logging exception text to
 *        temp transaction table, and truncating data when necessary to prevent database errors when attempting to import data
 *        that exceeds the max length of the column.
 * 
 * v1.84.1 2010-10-06
 *      - Bug Fix: (JIRA AUTOCITE-219) Can't enter item values for certain registry items in the list editor.
 *      - Bug Fix: (JIRA AUTOCITE-220) The drop down for registry itemvalues remains on top of everything when switching applications
 *      - Bug Fix: (JIRA AUTOCITE-221)  Unhandled exception while editing registry items in the list editor.
 *     
 * v1.84.0 2010-09-13
 *      - Bug Fix: (JIRA AUTOCITE-190) Delete Allocation Icon not readily visible in Windows 7. In Vista/Win7, the form will be displayed
 *        larger so the button is not truncated from the ribbon bar
 *      - Bug Fix: (JIRA AUTOCITE-154) Display the File Upload ID from an SDRO file Transfer to the backend and write it to the log.
 *        The SDRO file upload was being launched prior to the export finishing, due to asynchronous multi-threaded functionality implemented
 *        in a feature of AI.NET v1.79
 *      - Bug Fix: (JIRA AUTOCITE-208 & 35) Changes to XML files for NSW. Added support for "to date" and "from date" parameters for offences
 *        published by SDRO for NSW clients. The system will check/rebuild offence lists daily if any offences or their fine schedules
 *        expire or have a new date that is now effective. Also, the software will rebuild the ADDITIONALQUERY list based on info
 *        published by the SDRO
 *      - Bug Fix: (JIRA AUTOCITE-202) Audio recorded from PDA cannot be heard through the application. In Windows Vista and Windows 7, 
 *        the audio APIs were reworked by Microsoft, which included stricter adherence to WAVE format.  The .WAV files created by the CN50 
 *        mobile device contain a "corrupt" wave header.  When this corruption is detected, AI.NET will now perform a conversion process which 
 *        reads sections of the wave file and produces a new wave file with correct headers.
 *      - New Feature: (JIRA AUTOCITE-196) Database tables for NSW clients on AI HOST no longer in system for AI.NET.  The legacy (Delphi) 
 *        AutoISSUE application had the ability to import payment information for infringements from the SDRO, and display the history in 
 *        the inquiry result screen.  This functionality has now been added to AI.NET also.
 *      - Enhancement: (JIRA AUTOCITE-137) ADDITIONALQUERY table not was not generated automatically.  NSW customers will now have the
 *        ADDITIONALQUERY table populated/maintained automatically when XML files from the SDRO are processed.
 *      - Bug Fix: (AUTOCITE-186) Unable to attach more than one photo through AutoISSUE.Net notes. One bug related to this was fixed in v1.82, 
 *        but another related bug was found:  If you inquire on tickets, select one which doesn't have any notes, and then try to add 2 notes
 *        to it in succession (without viewing a different record in between), errors would be encountered. This has now been corrected also.
 *      - Bug Fix: (AUTOCITE-189) Court Calendar for 1.81 only shows one month in Windows 7. In Windows Vista and Windows 7, the month calendar 
 *        control has a larger size, so the control truncated itself to only display one month because there wasn't enough room on the form to 
 *        display 4 months.  This was rectified by increasing the size of the form, and then centering the calendar it is parent container
 *        The minimum date of a calendar was set to current date, and in Vista/Win 7, the calendar would not display any dates prior to the 
 *        minimum date, which was different behavior than in Windows XP. The minimum date is now set to the 1st of current month instead of 
 *        current date.
 *      - Bug Fix: (JIRA AUTOCITE-193) Courtesy Notices in record Inquiry freeze the application. The Courtesy Notice (Warning) structure used 
 *        by some NSW clients was of a type not specifically handled by the record viewer in AI.NET, which resulted in the error message being 
 *        displayed.  In addition, this error message sometimes ended up behind another form, and it would appear to the user that the 
 *        application was non-responsive.  This has been fixed by preventing the error message from appearing, by specifically allowing the 
 *        Warning structure to not have violation rows in the result dataset.
 *      - Enhancement: (JIRA AUTOCITE-33) Update Handheld Registry Editor (within List Editor) to assist in setting options.
 *        The list editor now has improvements in editing registry items that include drop-down selection for common category and item 
 *        names, edit attributes, descriptions, and validations for common registry items, etc.
 * 
 * v1.83.0 2010-08-20
 *      - Bug Fix: On some exports, a delimeter could sometimes fall into a logic flaw that causes an error with text:
 *       "Cannot find column -1".  (Discovered in Tyler, TX)
 *      - Bug Fix: In Windows 7 the inquiry result screen does not display all of the matches when sorted by location or plate.
 * 
 * v1.82.0 2010-07-28
 *      - Bug Fix: The client side software should check for updated issue_ap.xml files on synchronization. If out-of-date configuration
 *        is loaded, device synchronization won't be allowed, and the application will restart to load the current configurations.
 *      - Bug Fix: Concurrent hotsheet searches from multiple handhelds sometimes resulted in log file access errors ("file in use by another process")
 *      - Enhancement: Hot sheet search logs recorded against individual handhelds / divided by date
 *      - Bug Fix: (JIRA AutoCITE-186) Unable to attach more than one photo through AutoISSUE.Net notes
 *      - Bug Fix: (JIRA AutoCITE-181) Sort by Time in Schedule Manager is Incorrect
 *      - Enhancement: (JIRA AutoCITE-182) Multimedia file export needs an option to append or prepend the ticket number to the filename of the 
 *        multimdia files being created on export 
 *      - New Feature: Database encryption support for Personally Identifiable Information (PII).  Activated by configuring the "Encrypted" property
 *        of a TTableStringFldDef object to "true" (String fields in a table defintion are the only applicable fields)
 * 
 * v1.81.0 2010-06-15 
 *      - Enhancement: AutoTRAX: Added support for inventory / outage / repair tracking of assets other than Duncan SSM.
 *      - Bug Fix: Processing exceptions by date range filter would only process exceptions for a single day at the start of the filtered range 
 *       -Bug Fix: Mech location history was not updated when a mechanism was moved from an active location to spare. 
 *                 This is the cause of the exceptions processing issue in Los Angeles where some locations would not
 *                 accept transactions because the mechanism history was incomplete. After the upgrade is installed, 
 *                 a validation tool will clean up the missing entries, the system will begin tracking these correctly,
 *                 and going forward these histories will be filled in accurately. 
 *      - Bug Fix: "Add New Transaction" did not allow spare mechanisms to be added to inventory
 *      - Bug Fix: "Eagle Error Code" SQL evaluations were not formatted correctly to support all DB options, resulting in 
 *                 an error during processing "ProcessAudit: Cannot perform '=' operation on System.String and System.Int32"
 * 
 * v1.80.0 2010-06-07
 *      - Enhancement: AutoTRAX outage processing first looks to match SN from transaction; is SN is not found in database, a second attempt
 *                 is made to locate consider the value as a Location ID and execute the search that way. This feature allows outage reporting
 *                 to contain either SN *or* Location ID
 * 
 * v1.79.0 2010-05-22
 *      - Bug Fix: Previously was unable to export RecCreationTime for Citation records. Time is stored in RecCreationDate field
 *                 but the export only handles exporting as date.  A "virtual" field named RecCreationTime has been added to the available
 *                 fields for export definitions which allows a time mask to be set for the exported data.
 *      - Enhancement: Implemented logging and display of the File Upload ID for uploads to the SDRO (Only applicable to customers
 *                 in New South Wales)
 *      - Enhancement: Default settings for exports will now prevent the creation of data and summary files when no records qualified
 *                 for the export. The default setting can be overridden for data files via the "Export Empty File when No Data" option,
 *                 and the default setting can be overridden for summary files via the "Create Summary when No Data" option for the export
 *                 definition.
 *      - Enhancement: Extended the timeouts in AI.NET launcher application to 10 minutes (was 100 seconds) to help avoid timeouts
 *                 when upgrading workstation files on a slow network.
 *      - Enhancement: Added Help and About buttons to the toolbar on the Scheduler screen
 *      - Bug Fix: Prevented the import of orphaned child records (i.e. Voids, Notes, Reissuance, etc).  Previously, a bug existed where
 *                 child records could become either orhpaned or associated with the wrong parent record when there is an inconsistency between
 *                 parent-child files downloaded from handheld or manually imported from communication history folders.
 *      - New Feature: Added support to automatically generate work orders for damaged meters when METERSTATUS records are imported into 
 *                 the system from wireless X3 devices. The module was tested with development link at http://fss.duncan-usa.com/iws_WennSoftIntegrationQA/service.asmx
 *                 and requires the METERSTATUS data structure to contain fields such WIRELESSEXPORTDATETIME and OUTAGECODE. Note that scheduled
 *                 exports can omit records that have a non-null value in WIRELESSEXPORTDATETIME field because they would have already been
 *                 submitted to AutoFIELD to generate a work order. The AutoFIELD webservice URL, and WennSoft/GP customer name and ID must be 
 *                 configured in the "3rd Party Webservices" section of AI.NET System Options screen. (Developed for Atlanta)
 *      - Enhancement: New configuration option named "Allow Void/Reinstate after export" has been added to the "Global Settings". The
 *                 default value is False, so users will be prevented from performing a Void or Reinstate operation on a record that has already
 *                 been exported.  If the option is configured as True, then the export timestamps will be cleared so that the record will
 *                 once again qualify for the "All New" criteria for export definitions.
 *      - Enhancement: Payment grace periods will be obtained from PAM service (when available) and added to the true expiration times
 *                 for wireless meter enforcement.  Supports both the HTTP and UDP/TCP variants of PAM server.
 *      - Bug Fix: The Citation Audit Trail report was showing export status for exports that are not related to the data type for which 
 *                 the export was run. (Discovered in Menlo Park)
 *      - Bug Fix: Export list translation does not work for multiple instances of the same field in the same export definition
 *                 (Discovered in Sunshine Coast)
 *      - Enhancement: Manually invoked Imports and Exports show a progess dialog, and will also show the user summary information when complete.
 *      - New Feature: Implemeted support for real-time wireless communications for PDAs via the common wireless gateway project.
 *                 Multispace meter enforcement, real-time hot sheet searching, real-time citation upload, etc is now
 *                 available for PDA devices with wireless capability
 *      - Enhancement: Device synchronization file time stamp comparisons converted to UTC to support wireless synchronizations 
 *                 via CommCmdIP client between handhelds and servers
 *                 located in different timezones
 *      - New Feature: Implemented support for indivudal Parkeon MSM enforcement logins to be assigned to specific handhelds
 *      - New Feature: Implemented support for private APNs to be assigned to specfic handhelds. This is configured in the add/edit unit property
 *                 page in the Unit Range Manager. If a value is entered into the Private APN field here, it will overide any value set in the 
 *                 global registry file. The APN user name and password can also be supplied, but these are optional
 * 
 * v 1.78 2010-05-07 
 *      - Interim release for IPI 
 * 
 * v 1.77.2 2010-04-14
 *      - Enhancement: Updated Parkeon Enforcement to get meter keys in real-time to eliminate usage of stale keys (Parkeon likes to replace keys at will)
 * 
 * v 1.77.1 2010-04-13
 *      - New Feature: Enforcement grace period implemented for wireless enforcement. Phase 1
 *        inludes support for Parkeon meters; future phases will support Duncan and DPT meters
 * 
 * v1.77.0  2010-04-05
 *      - New Feature: Support for PAM-style enforcement of Parkeon meters. This includes:
 *          - Web service support for status of Parkeon meters
 *          - Master-Password only tool that connects to Parkeon web service and populates the METER_CLUSTER and METERS_IN_CLUSTER lists
 *      - Enhancement: Webservice logging filename and internal timestamps now based on UTC (no longer local server time)
 * 
* 
*****************************************/
#endregion

#region Older Revision History:

/****************************************
* 
*  OLDER REVISION HISTORY:
* 
 * v1.76.0 2010-03-12
 *      - Bug Fix: Corrected issue with AutoTRAX FK relations not being properly updated when an inventory transaction was 
 *         posted with both a brand new location and a brand new mechanism. 
 *      - Bug Fix: Outage could not be closed when repair is performed on a spare meter. This resulted from same invalid FK relation
 *  	- Enhancement: Logic implemented to detect and resolve “phantom” exceptions. These were exceptions resulting from transactions 
 *        in the field being constructed with data obtained from two consecutive locations due to the speed with which collectors moved 
 *        between mechanisms, combined with the generous retry features of the handheld when contacting mechanisms via IR.  
 *      - Enhancement: Added master-pwd only utility on AutoTRAX Inventory | Tools | Database Maintenance | Validate Generated Key Values
 *        This utility should be run following upgrade to v1.76.0, this will clean up the invalid FK and allow related transactions to be processed
 *      - Enhancement: Range Manager "Add Unit" function is no longer master password only. Users need access via standard password roles to 
 *        add Symbol 8146 devices used for AutoTRAX, as well as for AutoISSUE PDAs that need to be added to the range manager 
 *        before TCP/IP sync via CommCmdIP is even attempted
 *      - Enhancement: New filter options added to AutoTRAX exceptions processing functions. These allow reprocessing of exceptions 
 *        grouped by mechanism or by location, making it easier and faster to clean up multiple exceptions centered around specific meter asset types.
 *      - Enhancement: AutoTRAX exceptions form is now a free-floating window, such that it can be viewed alongside open inventory records 
 *        to aid in investigation and resolution of transaction exceptions.
 * 
 * v1.75.0 2010-02-17
 *      - Enhancement: Reworked Scheduler/TaskGroup execution logic to reduce thread counts, simplify execution logic, 
 *        and improve thread synchronization to help alleviate the rare problem of tasks running twice.
 *      - Enhancement: Introduced text-based log files to help monitor and diagnose problems with scheduled jobs. The logs
 *        will be retained for 2 weeks and are located at "[Executable path]\Logging\Scheduler\"
 *      - Enhancements to AutoTRAX inventory / Exception interfaces to facilitate faster resolution of transaction exceptions
 *      - Bug Fix: Unhandled exception occurred in the Range Manager if user presses OK when the lower or upper range has blank
 *        or otherwise invalid entries.
 *      - Bug Fix: Column names on status history tab of inquiry result were sometimes incorrect prior to voiding a ticket.
 *      - Enhancement: Updated Public webservice to support PAM meter cluster satus requests via TCP/UDP for wireless meter enforcement.
 *        For TCP/UDP PAM support, the public webservice's "Web.config" needs to be configured with "PAMServerTCPAddress" and "PAMServerTCPPort"
 *        keys in the "appSettings" section. For initial implementation (Raleigh), it should be configured as port 7878 on IP address 64.198.189.248
 *      - Bug Fix: If non-numeric Squad parameter was entered for report parameters, validation error would properly be displayed, but 
 *        then the report would still be generated after user acknowledged the error. (Discovered in Los Angeles)
 *      - Bug Fix: SQL errors could be encountered when inquiry searches found more than 1,000 records. The SQL statements used 
 *        to gather related detail records (voids, notes, etc) could exceed the SQL implementation limits of the database platform.
 *        (i.e. Oracle has a 1000 item limit, and Firebird has 1500 item limit for an 'IN' statement)
 *      - Enhancement: Inquiry result will now limit the result set to around 5,000 primary records to reduce resource utilization.
 *        Inquiry searches that result in excessively large results will warn user that refining the search criteria is preferred,
 *        but still gives the option to display a partial result set (truncated near 5,000 records)
 *      - Enhancement: Tab order of Parking, Hot Disposition and Mark Mode tabsheets on inquiry result screen was updated to 
 *        support smoother keyboard navigation.
 *      - Bug Fix: General SQL errors and unhandled exceptions could occur during inquiry searches when customer configuration and
 *        database metadata was out of synch with each other. This situation should not occur in production environments, but
 *        in the interest of system integrity, fixes were put in place to handle this rare situation gracefully.
 *      - Enhancement: Added support for USB communication on Window 7 and updated installation packages for Windows 7 compatibility.
 *      - Enhancement: Certain USB communication errors will now advise that the problem may be caused by the "Allow USB Connections"
 *        option in MS ActiveSync (for XP), or Mobile Device Center (For Vista and Windows 7)
 * 
 * v1.74.0 2010-01-13
 *      - Enhancement: When recovering by date range, recovery files processed in order of original synchronization, 
 *        from oldest to newest. This facilitates rebuilding of databases from session folders. 
 *      - Enhancement: Support for 2 inch printer paper added (Pidion 1300)
 *      - Enhancement: Added option to force X3 disk reclaim as part of synchronization process (requested by Chicago)
 * 
 * 
 * v1.73.0 2009-12-16
 *      - Enhancement: Time Zone information now being sent to X3 clients as part of synchronization process. Requires X3 OS 4.50 or newer.
 *         Older OS will ignore the Time Zone information.
 *      - Enhancement: Added host synchronization support for Intermec CN50 PDA (via CommCmdIP)
 *      - Enhancement: Added host synchronization support for Pidion 1300 PDA (via CommCmdIP)
 *      - Bug Fix: Officer Log report sorted groups incorrectly. (Dates were sorted in an alphabetic manner rather than chronologically)
 *      - Enhancement: Firebird Database restore is now only available when logged in as Master user to prevent accidental DB restores
 *        which could lead to problems such as missing data or duplicate citations.
 * 
 * v1.72.0 2009-10-23 
 *      - Bug Fix: Fixed bug that prevented a ticket from being voided when using Oracle and user is logged on with daily master password.
 *        (Discovered in CPS)
 *      - Bug Fix: Fixed bug in mixed AutoISSUE.NET/AutoTRAX configs that resulted in error during make composite task: "Symbol8146 Handheld
 *        does not support AutoIssue" (Discovered in Atlanta)
 * 
 * v1.71.0 2009-10-16
 *      - Enhancement: AutoTRAX transactions now processed utilizing the "as-of" date to support out-of-order transaction processing
 *        Previously transactions could only be posted against the "current" state of meter / location inventory, now they can
 *        be posted out of date order even when the location inventory has changed since they were generated. 
 *      - Enhancement: New AutoTRAX Reports: Meter Audits by Operator (detail and summary) count audit transaction based
 *        on unique locations audited, filtering out retries / repeats. Used by Los Angeles to compensate collectors accurately
 *      - Bug Fix: Mech Serial numbers did not get updated when a manually entered repair / swap transaction was posted
 *      - Bug Fix: Outages for Low Battery were not generated for all mech types that report battery levels
 *      
 * 
 * v1.70.0 2009-10-07
 *      - Enhancement: Integrated support for Symbol MC9090 PDA USB connection over ActiveSync (CommCmdIP)
 *      - Bug Fix: Corrected CEDeviceSync handling of platform files designated as "optional"
 *      - Enhancement: When using subconfigurations, now have ability to have seperate REGISTRY.DAT files for each subconfiguration.
 *        This was needed for Central Parking Services (CPS) so each subconfig could have different default agencies in parking citation.
 *      - Bug Fix: When using subconfigurations, handhelds will only receive officer list for officers that have not only handheld usage rights, 
 *        but also have rights to the subconfiguration assigned to the handheld. This is so a user from subconfig "X" can't login to a handheld
 *        assigned to subconfig "Y". (Discovered in CPS)
 *      - Bug Fix: When using subconfigurations, users can only inquire on data for subconfigurations that the user has access to. There was a 
 *        bug where a user with granted access to only subconfiguration was able to view data from all subconfigurations. (Discovered in CPS)
 *      - Bug Fix: When using subconfigurations, the device synchronization screen showed an error for the range allocations although every sub-config 
 *        and the MAIN config all had a valid range assigned. (Discovered in CPS)
 *      - Bug Fix: When using subconfigurations, subfolders within agency lists and hardware platforms will be created when the subconfigurations are
 *        edited.  This will prevent an error message about invalid path when using the List Editor for a newly introduced subconfiguration.
 *        (Discovered in CPS)
 *      - Bug Fix: Engineer Toolbox didn't include AgencyList subfolders in the installation disk package.  The subfolders need to be included because they
 *        contain lists for subconfigurations. (Discovered in CPS)
 * 
 * v1.69 2009-09-15
 *     - Bug Fix: When running the IPB/SDRO Export file, the filename is supposed to be saved with the first client code number 
 *       used and the first ticket number in the xml file generated. This functionality was broken by the export library restructuring
 *       in v1.62.  (Only effects Australian customers in New South Wales)
 *     - Bug Fix: Sometimes the .SYS file wasn't overwritten during a data sync session when a list change was made. Ensured that error messages
 *       are produced in the event that there is a file access error. Also explicitly delete the .SYS and .SYS.ZLIB files for an agency list
 *       after list changes are saved. This will force the composite file to be regenerated even if the system normally would not have created
 *       a composite file due to list effective date and/or timezone differences between client and server.
 *     - Bug Fix: AI.NET did not use the "AccessLevel" property that is available in the LayoutTool for a customer's definition. For Service Bureau
 *       customers, there are certain lists that should only be editable by supervisors or master user. The list editor now respects the "AccessLevel"
 *       setting. For "lalSupervisor", the user must be in a role of "ADMIN" or "ADMINISTRATOR". For "lalSystem", the user must be logged in with
 *       master password to perform edits.  Users with insufficient rights to edit the list will still be able to view them in a read-only manner.
 *     - New Feature: Support for manual entry and update screens. Manual entry is optional and controlled by designing Manual Entry and update screens
 *       in the LayoutTool. Configuration for Manual Entry in AI.NET is identical to that of the the legacy AutoISSUE Host (Delphi) product.
 *     - Enhancement: The Schedule Editor screen now allows for the schedules to be sorted by Schedule Name, Start Time, or Target Machine Name.
 *     - Enhancement: The Task Editor screen now sorts the task groups. User can toggle sort order between ascending and descending.
 *     - Bug Fix: Certain changes to a schedule could get lost if data was entered via keyboard and then click on the "Add" or "Save" buttons
 *       on the Schedule Editor's tool strip. The problem was caused by "ValueChanged" events of numeric entry fields that don't get fired before the
 *       button click event is processed.
 *     - Bug Fix: When overriding the last issued number for a book during final deactivation of unit, there was an unhandheld exception:
 *       "System.NullReferenceException: Object reference not set to an instance of an object. at ReinoControls.TextBoxBehavior.RefreshState()".
 *       The ReinoTextBox edit control was expecting additional properties that are used for C# issuance application on handheld; it has now been 
 *       updated to work properly for the AI.NET host.
 *     - Enhancement: When unfreezing books in the range manager, the user will now be prompted to override the last issued number assigned to each book.
 *     - Enhancement: The range manager will now only show open books for units. (If book has less than 10 available numbers left, it will not be displayed
 *       in the range manager so the tree view doesn't become excessively long)
 *     - Enhancement: FTP/SFTP functionality has been updated with file-based logging.  Log files will be stored in "[Installed Path]\Logging\FTP Logs", and 
 *       will be kept for 2 weeks. 
 *     - Enhancement: FTP/SFTP functionality has been updated to use ZLIB compression (when supported) to reduce transmission times during uploads to server.
 *       This is expected to help reduce the occurances of truncated OMS XML files in Los Angeles because the amount of data to transfer will be smaller.  
 *       A 20MB XML file for L.A.'s OMS export will compress to about 1.5MB using ZLIB compression. It has been determined that both the Tarrytown SFTP server
 *       for Los Angeles (138.69.165.138) and Duncan's SFTP for host customers (sftp.citationservices.com) support ZLIB's maximum compression level.
 *     - Bug Fix: Exports that included multimedia picutres or ticket reproductions did not report errors that occur during creation on multimedia files.
 *       Now, if multimedia errors occur, they will not cause the entire export to rollback, but the errors will be logged in the export summary, and an error
 *       indicator will also be present on the task/schedule status window when applicable.
 *     - Enhancement: Logging for PAM/DPT meter status requests in the Public webservice has been updated. PAM requests are now logged to files suffixed with
 *       "PAM_Query.log".  The log files will be prefixed with a timestamp. Log files will rollover to a new filename when either the date changes, or the log
 *       file reaches 20MB.  Also, logs older than 14 days will be removed from the system to save on disk space and easy maintenance.
 *     - Enhancement: Compatibility with MS-SQL Server 2008 (32-bit and 64-bit)
 *     - Enhancement: Added ability to manually invoke handheld synchronization from the scheduler screen. The availability of this option is controlled by
 *       configuration in "System Options | Global Settings | Handheld Communications | Allow Manual Device Synchronization from Scheduler". This feature was
 *       added by request to minimize the need to run a second AutoISSUE.EXE to manual perform a download with the a Scheduler window already running, there 
 *       should be a "Synchronize Devices" button in the Scheduler. This will minimize extra load on the local machine, minimize unnecessary connections to 
 *       the web service, and prevent scheduled synchronization failures due to already open synchronization forms. 
 *     - Enhancement: Added support for TWinPrnImage, which adds the ability to incorporate bitmap images into the ticket layout.
 *     
 * 
 * v1.68 2009-08-21
 *     - Bug Fix: AutoTRAX Inventory Reports corrected to display Last Audit/Outage/Inventory etc 
 *                of the Location - instead of said data from the current meter at that location
 *     - Bug Fix: Logic error that allowed duplicate field repair audit transactions to be posted (SUBTRACTION audits)
 *                When this occurred, subseqent processing of parent location resulted in CONSTRAINT violation error
 *                Constructed a tool to crawl through database and remove extraneous audit records created during repairs.
 *                For use on updated systems. This is a master password only option on the MeterTrax Tools menu
 *     - Enhancement: Implemented configuration option to enable/disable use of the location DB in handheld. Disabling
 *                for clients with large inventories speeds up synchronization times
 *     - Enhancement: DataTools library error messages now list the specific constraint violation data to assist 
 *                in diagnosis and resolution of issues
 *     - Bug Fix: SPECIAL EVENT TAG list items now appear in associated combo box drop down edit fields
 *     - Bug Fix: Location ID from the previous transaction is being used for the transaction following it,
 *                dubbed "phantom exceptions" because the transaction was being erroneously rejected 
 * 
 * v1.67 2009-08-13
 *     - Enhancement: Production release for Digital Payment Technologies meter enforcment status webservices
 *     - Enhancement: PAM webserice timedate stamps converted to UTC instead of local server times
 * 
 * 
* v1.66 2009-08-06
*      - Bug Fix: Recovery Data imports could fail because recovery folder names based on timestamp
*        didn't have enough resolution to be unique. End result is the system could copy the contents of
*        two session folders into a recovery folder, which means some .DAT would be overwritten, and
*        child record files such as PARKVOID.DAT and PARKNOTE.DAT would not longer point to the correct
*        master records. (Discovered in Manhattan Beach)
*      - Bug Fix: Inquiry screen sometimes showed void details for the incorrect ticket. Problem was caused
*        by multiple void records sharing the same transaction key (likely from importing records in a batch 
*        via PARKVOID.DAT file). To remain backward compatible, the inquiry screen now gets void information
*        from PARKVOID table if there is a transaction key conflict with the PARKING_TRANSLINK and STATUSHISTORY
*        tables. Cannot rely exclusively on the PARKVOID tables because voids created on the host, as well as 
*        "reinstate" status is only stored in the STATUSHISTORY table. (Discovered in Chicago)
*      - Enhancement: Per request, the Activity Log report will now show start and end Latitude and Longitude
*        values for records where GPS data was captured. (Most customers will see no change)
*      - Bug Fix: The location ID was truncated on some AutoTRAX reports when a 10-character location ID was being used.
*        The following reports have been updated to handle the 10-character location ID: Inventory Verification Report,
*        Transaction Exceptions, Daily Meter Management, Past Due Audit Report, and Inventory Location Manifest.
*        (Discovered in Montgomery County)
*      - Enhancement: Added void code and reason to issuance nodes and additional remarks to duty status nodes for
*        XML-based OMS export.  (Only effects Los Angeles)
*      - Enhancement: Added the ability to Group/Sort records in the inquiry screen by Date and Time. This option will
*        group records by date, and then sorted by time within the group. (Doesn't sub-group by vehicle or structure type)
*      - Enhancment: Implemented full support for Digital Payment Technologies meter enforcment status webservices.
* 
* v1.65 2009-06-05
*      - Bug Fix: The changes to the export library in v1.62 resulted in errors for MS-SQL 2005 that said 
*        "There is already an open DataReader associated with this Command which must be closed first". The 
*        actual error was regarding simultaneous active data readers on the same connection (rather than command).
*        To resolve this issue for MS-SQL 2005, the "MultipleActiveResultSets" property of the SQL connection 
*        string needed to be set to true. This is unsupported in MS-SQL 2000, so that platform will function in
*        the old-style that is not memory efficient for multimedia exports.
*      - Bug Fix: Schedule can be saved with a value that crashes the schedule editor. 
*      - Bug Fix: Fixed bug that could occur in MS SQL 2000 during updates to note tables when there was a 
*        multimedia attachement. The error message was "The query processor could not produce a query plan from the 
*        optimizer because a query cannot update a text, ntext, or image column and a clustering key at the same time."
*        The problem was that the table had two columns in the primary key (UniqueKey + MasterKey), and the SQL update
*        did not add a conditional clause for the MasterKey column. (Discovered in Manhattan Beach)
*      - Enhancement: Added record viewer support for "Tow" module
*      - Enhancement: Added hardware support for Symbol MC35 and Symbol MC75 mobile devices
*      - Bug Fix: Sequence files were getting book numbers written to file in random or backwards order which could
*        cause excessive book allocations and some books would not get filled. (Discovered in Ottawa)
* 
* v1.64 -  2009-04-20
*      - Bug Fix: Wireless record import failed silently when structure revision > 0 (Discovered in Chicago)
* 
* v1.63 - 2009-04-13
*      - Bug Fix: LA OMS 990 export was broken by changes in v1.62. (Only effects Los Angeles)
* 
* v1.62 - 2009-04-03
*      - Bug Fix: Large multimedia exports were causing out of memory errors. The code has been restructured in the 
*        export library to be more efficient with memory utilization.  Two main techniques were used: 1) Data is 
*        is read record-by-record from the database instead of gathering all data at once, and 2) Multimedia files 
*        are flushed to disk immediately after creation instead of caching in memory and performing bulk write at the
*        end of export job (Discovered in Chicago)
*      - Bug Fix: When adding a new unit or attempting to change the platform of a current unit, the software did not 
*        allow you add the serial number or change the platform. A message is displayed stating that the 
*        "Serial number can not be 0".
*      - Bug Fix: Added safety checks to prevent scheduled tasks from being run twice in the same job (To address
*        doubled up export runs that seldomly occur on new server in Los Angeles)
*      - Bug Fix: Was not able to define an export definition to operate against Public Contact structures. 
*        (Discovered in ACT-RTA)
*      - Bug Fix: String to numeric conversion error is displayed when generating a Public Contact Statistics report
*        when the suspect age field contains a null value. (Discovered in ACT RTA)
*      - Enhancement: Added the ability to include void time in exports. Void date was always there, but not the time.
*        The void time can be exported by choosing either the "STATUSTIME" field of a void structure, or the 
*        "VOIDSTATUSTIME" field of a ticket structure. 
*      - Enhancement: Added the ability to include "START DATE" and "END DATE" fields in header or trailer records for
*        for exports. The start and end dates in header/trailer records will correspond to the lowest and highest dates
*        of the records included in the export. For most data types, it is based on ISSUEDATE. ActivityLog will be based
*        on STARTDATE, and Void data structures will be based on STATUSDATE.
* 
* v1.61 - 2009-02-24
*      - Bug Fix: Fixed bug that caused "Cannot find column 1" error to be displayed in List Editor and Make Composite
*        function under certain circumstances. This condition can occur if a future "effective date" is set for a list
*        that has more than 1 revision in its table definition. (Discovered in Los Angeles)
*      - Enhancement: Restructured code modules to support future implementation of TCP/IP-based data synchronization 
*        models for devices.
*      - Bug Fix: "Make Composite" task group and/or scheduled task could result in error message of "Error getting 
*        HHPlatformFileInfo inside DoMakeCompositeListTask(). _GetHHPlatformFileInfoForXSeries: invalid CustomerCfg name".
*        This bug resulted in the local file cache not getting updated for device platform files during the scheduled
*        "Make Composite" task. It would however be updated correctly during normal device communications. (Discovered in
*        San Diego and Los Angeles, which have more than one customer configuration, i.e. X3_Parking and X3_WirelessParking)
*      - Bug Fix: TTableAlphaPosMod10VirtualFldDef doesn't produce correct string; All numeric characters are excluded. 
*        (Discovered in Barcode used for Ottawa tickets)
*      - Bug Fix: Barcode reproduction did not include the start and end characters when FontBC39 font was used. It did work
 *       correctly if the alternative font name of FontBC3of9 was used instead. (Discovered in Ottawa)
* 
* v1.60 2009-02-17
*      - Bug Fix: Fixed bug in TTableOttawaMod10VirtualFldDef calculations (Only affects Ottawa)
*      - Enhancement: Support for PDA registry editing implemented. Generic "CE_REGISTRY" table def is used as a source to create 
*                     clones for each of the defined PDA types (Intermec CN3, Symbol 9090, etc)
* 
* v1.59  - 2009-01-30 
*      - Enhancement: Added USB support for Intermec CN3 PDA
*      - Enhancement: Integrated support for Symbol MC9090 PDA (serial connection only, pending development of USB client driver)
*      - Enhancement: Intermec / Symbol devices utilize last 5 digits of OEM asssigned serial numbers (Symbol 8146 remain user-assignable)
*      - Enhancement: Moved Symbol 8146 support from "MeterTrax" platform folder to accurate "SY" platform folder
*      - Enhancement: Added ActiveSync based device initialization for Intermec CN3 and Symbol 9090 to go with Symbol 8146 
*                     The "Initialize Handheld" button was moved from the Main Menu into the Range Manager
*      - Enhancement: Added "Scramble" support to Authentication library for use in PDA devices
*      - Enhancement: Application's support for 10-digit citation numbers higher than 2147483647 was flakey because under certain cicumstances,
*        they might be treated as 32-bit integers instead of 64-bit. The application does handle 10-character citation numbers, but the Parking 
*        table in the DB must originally be created based on a 10-character citation number length in its configuration. (You can't go from 
*        standard 9-character to 10-character because a different datatype in the database is required)
* 
* v1.58  - 2009-01-15
*      - Bug Fix: Fixed check digit calculation bug in TTableMod10CDOnlyVirtualFldDef. Can affect print pictures and exports.
*        Known to affect: Darebin (Billpay, Keying line, Barcode), Chicago (OCR), and Salt Lake (OCR), Monash (Keying line, Barcode)
*      - Bug Fix: The Advanced Search option in Record Inquiry displayed a predefined list of fields that does not correspond to the 
*        customer configuration. The advanced search fields were the same for any customer and a lot of field were Traffic-related,
*        which is not applicable to most customers. The advanced search now ties directly to the customer specific configuration to allow 
*        querying of "custom" fields that do not appear in the standard search.
*      - Enhancement: A lock will be considered abandoned/orphaned after 8 hours to reduce occurances of items being locked indefinately until
*        someone explicitly unlocks it.
*      - Bug Fix: List changes did not take effect until server time in network/hosted installations. Code now only looks at date instead of 
*        timestamp when checking for composite file effective eligibility. 
*      - Bug Fix: Exports with zero records does not create an empty export file.
*      - Bug Fix: When installing new product with Oracle as the database platform, user could be prompted to enter credentials for an Admin-level
*        Oracle user in order to create the database. The password for the DBA user was not obscured during entry, which poses a security risk. 
*        (Discovered in Ottawa)
*      - Bug Fix: Fixed bug that caused some abandoned vehicle records to be exported against the incorrect session for XML OMS (Only affects Los Angeles)
* 
* v1.57  - 2008-12-05
*      - Bug Fix: Fixed bug caused some XML OMS session records to be exported without start/end timestamps. (Only affects Los Angeles)
*      - Bug Fix: Corrected formatting error in TTablePOSTbillpayFormatVirtualFldDef
*      - Enhancement: Implemented TTableMorelandMod97VirtualFldDef for Moreland's variation of the Australia Post Check Digit
*      - Enhancement: Implemented cdSaltLakeCityMod10 check digit type for sequence structures
*
* v1.56  - 2008-11-20
*      - Bug Fix: Fixed bug that caused "Index was outside the bounds of the array" error on some configurations with OCR
*        (Discovered in Ottawa)
*      - Bug Fix: Change FTP logic to support uploads to a subfolder. Requires using a change directory command followed
*        by a send file that has "\" as the first character of the destination filename. (Discovered in CSULB)
*
*  v1.55  - 2008.09.11
*      - Bug Fix: Removed obsolete AutoTRAX report "Auto-Generated Inventory"
*      - Enhancement: Added host side text-based event logging, stored in the EventLogs folder under main executable locations.
*        This is to record issues when webservices / database access isn't available
*      - Enhancement: Added USERINPT.LOG to the AutoTRAX synchronization file list
*      - Changes to the projects, webservices, and common files to support the PatrolCar AIR prodcut line.
* 
* 
*  v1.54 - 2008-09-05
*      - Bug Fix: Fixed XML token replacement bug in Los Angeles OMS XML export that caused malformed XML when
*        an element contained the "&" symbol. (Only affects Los Angeles)
*      - Bug Fix: Changed the deserialization routine for XRef tables used by Los Angeles OMS XML Export to work
*        around a problem caused by versioning conflicts. (Only affects Los Angeles)
*      - Bug Fix: TGenericIssueStruct structures were missing the fields necessary to generate exports. This was 
*        preventing Broken Meter and Damaged Sign exports to function. (Discovered in Chicago)
*      - Bug Fix: Report Parameters screen dropdowns would show no list items for any lists when it referenced
*        any single non-existent list.
*      - Enhancement: WebService validation code implemented to try to refresh stale connections
*      - Bug Fix: S3/S4 files included in the file cache does not retain their compression property setting,
*        resulting in compressed files getting sent to the handheld without the compression flag (Arlington, TX)
*
*  v1.53 - 2008-08-19
*      - Bug Fix: Errors that occurred inside a CrystalReport were being suppressed, and ended up yielding an 
*        incorrect message of "No data for report".
*      - Bug Fix: Restructured and optimized Violation Summary, Violation Summary by Officer, Violation Summary
*        by Area, and Officer Log reports to be more efficient and resource friendly. Miami Beach was unable to
*        run a Violation Summary report for a 1-year period with 1GB RAM. New technique should help significantly
*        to allow these reports to run on big date ranges on a lower-end machine. (Discovered in Miami Beach)
*      - Bug Fix: When loading a different customers to a handheld, sometimes an error would occur containing the
*        text "Error occurred during Meter data import obtaining current configuration data:", because under some
*        conditions, the system mistakingly treated the handheld as a MeterTRAX configuration. (Discovered in 
*        Arlington, TX)
*      - Enhancement: Added a database index on session key, which improves performance syste-wide for authentication
*        and validation checks performed for all web service calls. (Discovered in Miami Beach)
*      - Bug Fix: Fixed Oracle Blob/LongRaw problem that was exposed when moving/editing Task Groups that have
*        lengthy Task definitions, most likely due to an embedded FTP session script. (Discovered in Los Angeles)
*      - Enhancement: Added logging for the PAMClusterStatusRequest method in Public WebService to assist in 
*        debugging/verifying wireless PAM enforcement.
*      - Enhancement: Implemented Year - Month - Week subfolder scheme for handheld Session backup folders
*      
*    
*  v1.52 - 2008-07-29
*      - Bug Fix: Officer Log Report didn't include group sub-totals when officer's issued tickets in more than one
*        area in the same day. The grouping logic has now been corrected to subtotal and change grouping when there
*        is a change in IssueDate, Agency, Beat, or SquadNo. (Discovered in Long Beach)
*      - Bug Fix: TTableVirtualFldDef virtual field class was forcing a size of 180 instead of simply defaulting to
*        180 characters, which can result in truncation of data in exports or ticket reproductions. (Discovered in 
*        Arlington, TX)
*      - Bug fix: TTableVirtualFldDef virtual field trunacted the 1st character if data exceeded maximum size of field.
*        In combination with above bug, could result in truncation of both the left and right sides of the data.
*        (Discovered in Arlington, TX)
*      - Bug Fix: Adjusted size of 24x24 fonts to be a little bit smaller for a better fit in the print picture.
*        (Discovered in Arlington, TX)
*      - Bug Fix: Adjusted logic so memos show multiple lines of data with less truncation in case height of print 
*        picture object is too short by only a few pixels. (Discovered in Arlington, TX)
*      - Bug Fix: Fixed bug in record view printing that overlapped header information ontop of screen representation.
*        Also added "Page X of N" to the header. (Discovered in Arlington, TX)
*      - Bug Fix: Prevented printing record view pages in 2-page wide formats, which could happen if user expands 
*        size of the record viewer form in AutoISSUE.NET (Resulted in every other page being blank except for header
*        and occassionally a few pixels truncated from previous page, but not enough to warrant a page of their own).
*        (Discovered in Arlington, TX)
*      - Bug Fix: Fixed bug that truncated lengthy data from "non printed"/"other" fields in both the record view and
*        ticket reproduction views. Such data now will print in a mult-line format as necessary so no data is truncated.
*        Also enhanced the cosmetic output of the "non printed" field information in ticket reproduction view.
*        (Discovered in Arlington, TX)
*      - Bug Fix: Fixed several GDI resource leaks when printing record views from inquiry result screen. Also improved 
*        performance and resource utilization by eliminating nested redraws and calculations of form controls and 
*        prevented unnecessary drawing of objects outside of the clipping region. These fixes should resolve printing
*        problems that occassionaly occur on some newer HP LaserJet printers, such as 4250 and 4000 (Printing works
*        on our 4250, but we don't have a 4000 to test against)
*    
* 
*  v1.51 - 2008-06-25
*      - Bug Fix: DatFileManager swallowed list edit/hotsheet file access errors/exceptions without notification
*      - Bug Fix: AutoTRAX occupancy percentage calculation to correctly handle time spans (days vs weeks) 
*      - Bug Fix: AutoTRAX handling of debit card audit values was incorrect
*      - Enhancement: Added user configurable filters to exception transactions re-processing (AutoTRAX)
*      - New Feature: Added legacy conversion code to bring in Integrator 2000 data from Sybase SQL Anywhere DB systems
* 
*  v1.50 - 2008-06-16
*     - Bug Fix: Ticket Reproduction view didn't handle the Font12x12Bold font used by some S4 configurations.
*       (Discovered in Arlington, TX)
*     - Enhancement: Code now suppresses delimiter if previous field was "Concat to next" style and didn't contain data.
*       This was implemented to suppress extraneous spaces in between concatenated data. (Implemented for Arlington, TX)
* 
*  v1.49A - beta version for Milwaukee Demos / Digital PayTech interface
* 
*  v1.48 - 2008-06-06
*     - Bug Fix: "Violator" hotlink in Traffic inquiry was redirected to "Violation" panel instead of "Violator" panel.
*       (Discovered in Arlington, TX)
*     - Bug Fix: Hotlinks in Traffic inquiry didn't exhibit consistent behavior (sometimes scrolled to correct location,
*       othertimes the scrollbar got of sync with what was displayed in the main panel) (Discovered in Arlington, TX)
*     - Enhancement: Range Manager becomes very slow when there are a lot of units and/or allocation books.
*       The book infomration will now only be queried and built when applicable nodes in the treeview are expanded,
*       which drastically speeds up normal usage of the Range Manager screen in a large client (i.e. Los Angeles)
*     - Bug Fix: List Editor allowed a user with "View-Only" rights to modify lists by double-clicking in the grid or
*       tree view, and also by using the "Move Up"/"Move Down" buttons for non-sorted lists, or by changing the
*       effective date of a list. (Discovered in Arlington, TX)
*     - Enhancement: The scheduler now has a 30-second "grace period" in the logic for executing schedules to address
*       the problem of schedules getting skipped if the server is under heavy CPU usage or slow for some other reason.
*       Also, some thread-safety checks were put in place for the scheduler's timer to avoid thread contention problems.
*       (Implemented for Los Angeles)
* 
*  v1.47 - 2008-05-21
*     - Bug Fix: Reports that encompass a lot of data can become lengthy operations that exceed the 10-minute 
*       timeout conditions for WebService calls, ending in the job failing or being aborted. Code has been 
*       restructured so reports are run asynchronously on both the server and client sides so the operation
*       can complete successfully without timing out on either end. (Discovered in AutoTRAX reports after conversion)
* 
*  v1.46 - 2008-05-09
*     - Removed deprecated "Device Unload Report" from AutoTRAX report xml
*     - Bug fix: corrected AutoTRAX reports broken by changes to report engine made for AutoISSUE reports
* 
*  v1.45 - 2008-05-06 
 *     - Bug Fix: "Object Reference" error can occasionally appear during communications if a device is switched
 *       from one sub-configuration to another (within the same customer name). This most likely only happens
 *       when files are deleted manually from the device rather than letting the device synchronization handle 
 *       the file deletes normally.
 *     - New Feature: Added configuration option allowing users to change the title bar text
 *     - Enhancement: Expanded About Box to display URL information in a more readable format
 *     - Bug Fix: Recent changes made to list editor resulted in exceptions when editing lists with unspecified (-1) maximum lengths
 *     - Bug Fix: When users select Zone / Collection Route / Maintenance Route filters, some AutoTRAX reports would fail
 *     - New Feature: AutoTRAX X3 handhelds now recieve a "mini" location db during synchronization. Used for info display during audits
 *       to help auditors confirm / note inventory discrepancies
 * 
 *  v1.44 - 2008-04-11
 *     - Bug Fix: Font property default for serialization introduced at TWinBase level was interfering with
 *       proper serialization of TWinPrnPrompt classes. For example, if a TWinPrnPrompt was supposed to be
 *       using Font8x8, it was explicitly written to ISSUE_AP.XML because Font8x8 is serialization default
 *       introduced at TWinBase level, but the internal font default for TWinPrnPrompt is Font16x16. End result
 *       is that if the label was intended to use 8x8 font, it ended up using 16x16 instead. 
 *       (Discovered in GO Transit, CAN)
 *     - Enhancement: New fields added to Import/Export definitions will now get a default padding of spaces
 *       instead of nulls. Spaces are the most commonly desired padding, so it makes sense to use it as the 
 *       default.
 *     - Bug Fix: Under certain circumstances, a multimedia export could fail due to generating duplicate target 
 *       filenames which were based on the timestamp (i.e. very fast CPUs and/or small .WAV/.JPG data could 
 *       cause this condition). Now, the multimedia filenames will default to the original filename as given
 *       by the handheld at time of creation. If there is a conflict of filenames within current export session
 *       (most likely due to 2 handhelds generating the same filename), then the filename will be changed to 
 *       current timestamp until there is no conflict.
 *     - Bug Fix: "Object reference not set to an instance of an object" error can sometimes occur during
 *       a login attempt of the software. It has been demonstrated that this condition could be caused by 
 *       improper file persmissions assigned to the ASP worker process that runs the WebService under IIS.
 *       The code has been updated to identify this condition and provide a more meaningful error message
 *       to help diagnose problems that occur during instantiation of the WebService.
 *     - Bug Fix: Importing recovery data didn't perform updates for multimedia fields if the target record 
 *       already existed. This was a problem because occasionally a multimedia file may not be present at the
 *       time of importing the original note detail record. Manually recovering the record now will perform
 *       updates so the image/audio attachment gets stored in the database.
 *     - New Feature: Added the ability to export individual photos, diagrams and audio attachments from the
 *       inquiry result screen to aid in the adjudication process. After clicking on a thumbnail image in
 *       the notes grid, the multimedia preview screens have a button that let the user export the multimedia
 *       data to a file.
 *     - Bug Fix: Schedules for exporting data were occasionally being set to contradictory export parameters
 *       (i.e. "All New" date type, but with "Exported and Not Exported" option. This resulted in an "All New"
 *       export defintiion that picks up all previously exported records. This only effected simple schedules,
 *       as the bug was not present in schedules that utilized a task group.
 *     - Bug Fix: Engineer Toolbox was creating installation disks that were overwriting the handheld's 
 *       registry file on upgrades. Since that is a configuration file subject to change by the end-user,
 *       we shouldn't overwrite existing REGISTRY.DAT files.
 *     - Enhancement: AutoIssue.NET Installation disks will now default Oracle installations to use "ETECISSUE"
 *       as the DB username instead of "SYSDBA". This is to help prevent users from accidentally creating
 *       database objects under the standard DBA user, which usually is not preferred.
 *     - Enhancement: User Security Editor now supports dynamic fields defined in the customer's UserStruct.
 *       This ability was present in the older Delphi-based product, but hadn't been implemented in AI.NET.
 *       (Implemented to support Ottawa's upgrade, as they use OfficerAgency as a field tied to the login process)
 *     - Bug Fix: User list was always being transmitted to the handhelds with a static filename (USERSTRUCT.DAT).
 *       That filename is incorrect if there are structural changes to the UserStruct definition that causes a 
 *       revision change, and will result in the inability to login to a handheld device. The code has been updated
 *       to include the correct revision suffix when applicable. (Discovered in Ottawa)
 *     - Bug Fix: Stack Overflow exception was occuring in "Citation Audit Trail Report" if run over a date range
 *       that produces many records. Problem was a function recursively calling itself instead of executing in
 *       a "while loop". Exception occurred in the WebService causing it to get recycled by IIS and returning 
 *       an HTML error about server being unavailable. (Discovered in Subiaco)
 *     - Bug Fix: No data was being populated in the XML-Based OMS export files for Meter ID and Query Info fields
 *       under the Issuance and Query Hit nodes. Meter data was being extracted from the METERNO field, but should
 *       have been extracted from the LOCMETER field instead. (Only effects Los Angeles)
 *     - Bug Fix: Exports that encompass a lot of data can become lengthy operations that exceed the 10-minute 
 *       timeout conditions for WebService calls, ending in the job failing or being aborted. Code has been 
 *       restructured so Imports/Exports are run asynchronously on both the server and client sides so the operation
 *       can complete successfully without timing out on either end. (Discovered in Los Angeles multimedia exports)
 *     - Bug Fix: Handheld registry items have a max length of 200, but the host-side list editor only allowed 
 *       80-characters (Discovered in Los Angeles)
 *     - New Feature: WirelessEnabled flag is now supported in customer configuration which in turn allows a 
 *       different handheld executable and/or HH operating system to be used for customers that need wireless 
 *       capabilities on some devices (Initially implemented for Los Angeles)
 *       Wireless X3 Platform files are named "CE_ISSUE_AP_MFCWireless.EXE" and "NKWireless.NB0"
 *     - New Feature: Added support for specifying device-specific DUN login and password for wireless devices.
 *       Implemented for the purpose of supporting ATT/Cingular authentication requirements when establishing a
 *       GPRS connection to a private APN. (Initially implemented for Los Angeles)
 *     - New Feature: Added Pay-at-Any-Meter support to the AutoISSUE Public WebService, so wireless handhelds
 *       can query a RIPNET server and perform a Cluster Status Request. (Initially implemented for Los Angeles)
 *     - Bug Fix: Make Composite list sorting did not sort dates using the correct formatting, which can cause
 *       records in a court date list to be in the wrong place. This generally only effects Traffic customers.
 *       This bug was introduced in v1.41. (Discovered in Arlington, TX)
 *     - Bug Fix: AutoTRAX Inventory Reports did not utilize "ACTIVATED" parameter when executing
 *     - Bug Fix: AutoTRAX Transaction Exception Report allows users to filter by exception type, but only the "All" option worked.
 * 
 *  v1.43 - In Q.A. (2008-03-18)
 *     - Bug Fix: Fixed bug that could cause some child lists to get the incorrect MasterKey relationship if
 *       it's master table was changed during make composites (usually because of sorting, or the removal of
 *       empty or duplicate rows). Examples of child lists are VehModel (filtered by VehMake), and CourtDate
 *       (filtered by CourtName). (Discovered in Alvin, TX)
 *     - Bug Fix: Fixed various bugs in making composite lists that was introduced in v1.41.  The changes in
 *       that version worked for it's intent, but inadvertently broke related functionality in other styles of
 *       lists. (Discovered in Alvin, TX)
 *     - New Feature: Implemented "cdMod10OddRJ" check digit for citation sequences (Fort Lauderdale)
 * 
 *  v1.42 - In Q.A. (2008-03-12)
 *     - Bug Fix: List editor didn't work for registry files of S3 or S4 handhelds. There was inconsistencies in 
 *       the system for the filenaming convention when dealing with S3/S4 handheld platform.
 *     - New Feature: After successful GetFile/SendFile via FTP, the source file can be renamed, with support for 
 *       wildcards in the filename. Implemented for Los Angeles so a *.DAT file can be renamed to *.DAT.BAK on the 
 *       FTP server after it has been downloaded successfully.
 *     - Bug Fix: Large Import/Export summaries were slow to be displayed and/or printed. Optimized the string 
 *       handling techniques involved to reduce CPU/Memory resource usage and speed up this operation.
 *     - Enhancement: Restructured XML-based OMS export to use more efficient DBDataReader instead of DataSet.
 *       This is an attempt to reduce resource usage during this complex export. (Only effects Los Angeles)
 *     - Enhancement: Multimedia export will now export image files captured by the X3 AutoCites at 50% image
 *       compression quality. This will result in smaller image files with very little noticeable image degradation.
 *     - New Feature: Certain USB-related errors that occur during communication sessions will be logged to a text
 *       file in the server's "Bin" folder. Initially implemented for Los Angeles to track reliablity of USB 1.1
 *       root controllers vs USB 2.0 root controllers.
 *     - Enhancement: Restructured XML-based OMS export to use specialized routines for persisting data to XML file.
 *       The new routines should be faster and consume less resources than the generic XMLSerializer that is built
 *       into the MS .NET Framework (Only effect Los Angeles)s4
 *
 *  v1.41 - 2008-02-04
 *     - New Feature: Can now inquire on "Control Number". This is used for the Abandoned Vehicle module in
 *       Los Angeles. Also, if Control Number exists, it will be shown on the inquiry result screen instead 
 *       of Issue Number.
 *     - Bug Fix: Source files are now deleted after a successful import (via Import definition)
 *     - Bug Fix: Trailing whitespace on fields are trimmed during import and export to solve the problem
 *       of padded data caused by importing fixed length file formats.
 *     - New Feature: An "Order By" clause can now be added to an export definition so it can create a sorted
 *       file. This was needed for the Abandoned Vehicle module in Los Angeles.
 *     - New Feature: Maintenance Detail Report added for AutoTRAX (Was intended to be in v1.40, but was omitted
 *       from the build)
 *     - Bug Fix: "Make Composites" process used a sorting method that was not true ASCII sorting. Resulted in 
 *       files that were not sorted the same way the handheld expected. For example, .NET sorted "-" before "(",
 *       which is different than if sorted based on ASCII character set values. This incompatiblity caused list
 *       only validation checks to fail in Los Angeles when changes were made to their "Activity" list.
 * 
 * v1.40 - 2008-01-28
 *     - New Feature: Maintenance Detail Report added for AutoTRAX 
 *     - Enhancement: Set TopMost property on RangeUnit form so it doesn't stay hidden behind 
 *       the communications form when it is called on from a communications session
 *     - Bug Fix: exports of child structures (notes, voids) were getting left blank in Miami-Dade exports
 *     - Enhancement: Added parameter validation methods to report parameter objects/screens
 *     - Bug Fix: XML-Based OMS export for L.A. sometimes ran into an OutOfMemory Exception when very large
 *       datasets are involved. The technique for generating the XML file has been modified in an attempt to
 *       reduce/optimize some of the resource usage. (Only effects Los Angeles)
 *     - New Feature: Added "UploadToHandheld" property to TSearchStruct objects so Mark Mode data can be sent 
 *       back to the handhelds to resume marks from previous day. (Implemented for Guelph)
 *     - Bug Fix: "Prompt before Overwrite" feature of Export library didn't work if the target directory was 
 *       a network drive. (Discovered in Baltimore)
 *
 * v1.39 - 2007-12-17
 *     - Bug Fix: Import definitions associated with HotSheets were only being used for automatic file conversion
 *       during the "Make Composites" process of communication cycles. The imports failed if they were explicitly 
 *       executed (both manual or scheduled execution modes)
 *     - Bug Fix: Fixed communication support for S3/S4 when sending compressed list (.DAT) files
 *     - Enhancement: Changed the XML file format of the Import/Export summaries to be more efficient.
 *       In L.A., importing a 515KB hotsheet was resulting in a 3MB summary file. New technique cut the summary
 *       downto just 2KB. This can also greatly reduce the size of export summaries.
 *     - Bug Fix: Unhandled exceptions could occur when using the "Recover Data" form. (Manually typing a 
 *       path could result in errors, as well as attempting to import corrupt or incorrect data files)
 *     - Enhancement: Failed imports produced ugly and hard-to-read error dialogs because the error message
 *       was being put into the dialog's title bar.
 *     - Enhancement: Added "Parse before Padding field" option to export library to handle being able to extract
 *       a substring from raw data. This was needed in Daly City to remove hyphen character from suspect height
 *       field during an export.
 *     - Enhancement: Added dropdown lists for Officer Name and Officer ID in applicable report parameter screens.
 *     - Enhancement: Added auto-complete and popup suggestions for dropdown lists in parameter screens. Also
 *       forced uppercase report parameter entries with dropdown lists.
 *
 * v1.38 - 2007-12-10
 *     - Bug Fix: Schedule and Task editors overwrote the ExportedType to "Exported and Not Exported" for LA's
 *       OMS exports since there is no single data structure explicitly associated with these exports.
 *     - Enhancement: Updated SDRO file transfer functionality to support SSL. (New South Wales customers only)
 *     - Enhancement: Added "cdPGeorgeMod10" as a known check-digit type for sequence structures.
 *
 * v1.37 - 2007-12-03
 *     - Bug Fix: Group totals were incorrect on the Violation Summary by Area Report
 *     - Bug Fix: Violation Summary by Area Report would time-out if tried to run on a large dataset.
 *       The SQL statement that was used to gather data was not well optimized. Moved calculation of major 
 *       group totals to a post-query process instead of using nested sub-select statements. (Discovered 
 *       in Miami when they tried to run with a 1 month date range) 
 *     - Bug Fix: Violation Summary by Officer Report was effected by similar problem as above.
 *     - Enhancement: Reports now run in a non-modal fashion that lets the user do other activities in the
 *       application while the report is being created. The user interface also now has a "Cancel" button
 *       that can be used if the user no longer wants to wait for a report to finish.
 *     - Enhancement: Exports now run in a non-modal fashion that lets the user do other activities in the
 *       application while the export is executing. After clicking "OK" on the export parameter screen,
 *       the parameter screen is closed and replaced with a non-modal progress dialog.
 *     - Enhancement: TER_ForceCincinnatiMod7CheckDigit implemented
 *     - Bug Fix: Inquiries could fail if issuance structures used different field sizes. For example, 
 *       if IssueNo was length 6 in Parking, but 7 in Traffic, searching for a 7-digit traffic citation
 *       would fail because input got truncated to 6-digits during record search. (Discovered in Livermore)
 *     - New Feature: Implemented automatic file transfers with State Debt Recovery Office (SDRO) for
 *       clients in New South Wales. This feature relies on specific configuration elements including
 *       export definitions and agency list files meeting the requirements.
 *     - New Feature: Implemented button to manually invoke file transfers with State Debt Recovery Office
 *       (SDRO) for clients in New South Wales. (Complement to the automatic feature above intended to
 *       allow user to resolve error conditions encountered during automatic transfers)
 *     - Bug Fix: Missing data in SDRO exports. Dynamic date/time fields did not have a data mask or length,
 *       so they were being exported as blank strings.
 *     - Bug Fix: Corrected bug that expected HANDBOOK_DATA_FILE_ID field to exist in INCLUDEDOFFENCE list
 *       table. System now treats this field as optional. (New South Wales customers only)
 *     - Bug Fix: Exports did not properly format dollar amounts that contained non-zero cents. For example,
 *       $75.50 might get exported as 75.5 instead of 75.50 if the data mask was set to something like 9990.00.
 *       Problem was caused by a second decimal point being added to the data in the formatting table when
 *       the data mask of the field was assumed to be implied decimal for currency values.
 *     - Bug Fix: Range Allocation grid disappeared from the communication screen; it was hidden behind
 *       other components on the screen. (Bug was introduced in v1.35?)
 *     - Enhancement: Added a new configuration option that controls when a low allocation warning is 
 *       displayed in the Range Manager and Communication screens. (Requested by Miami because default
 *       setting of 10 free books didn't allow them enough time to create new allocations). Configuration
 *       item is named "Books free threshold for low allocation warning:" and appears on the Global Settings
 *       tab of the System Options screen.
 * 
 * * v1.36 - 2007-11-13
 *     - Bug Fix: Entire dataset describing lists attached to report parameter screens would fail if one list
 *       failed validation. Fixed so only the lists that fail are not attached to parameter screen.
 *     - Bug Fix: Report parameter screen could error out and prevent a report from running if a list associated
 *       with a parameter is invalid or has a null entry
 *     - Added "Squad", "Agency", "Beat" filter parameters to Officer Log and Activity Log reports. Only visible 
 *       if field exists in the customer's configuration
 *     - Added "Agency" and "Beat" filter parameters to Overall Device Stats Report (Los Angeles)
 *     - Bug Fix: Fixed report grouping problem in Officer Log and Activity Log reports that occurs if
 *       customer has more than 1 officer with the same name
 *     - Bug Fix: Officer counts and averages were incorrect in the Overall Device Stats Report if customer
 *       has more than 1 officer with the same name (Los Angeles)
 *     - Bug Fix: TTableLongBeachMod10VirtualFldDef was returning checkdigit only instead of appending the 
 *       checkdigit to the source string (Long Beach)
 *     - Bug Fix: Ticket reproduction items got resized incorrectly if they exceeded bounds of their parent object.
 *       Discovered in Long Beach, noticeable as boxes that were not wide enough.
 *     - Bug Fix: Ticket reproduction treated printable objects as transparent instead of opaque. Discovered
 *       in Los Angeles, noticeable as misplaced grid lines. (Opaque treatment essentially erases unwanted
 *       lines drawn from a previous drawing operation)
 *     - Bug Fix: Reproduction view did not properly handle certain date and time formats. Usually noticeable
 *       when a lowercase "am/pm" was displayed when it should be uppercase "AM/PM" instead.
 *     - Bug Fix: Date/Time formats in "Record View" and "Other Data" did not always match those used by the 
 *       handheld.
 *     - Bug Fix and Enhancements (Import/Export Definition View):
 *        1. The delimiters when used with concatenation such as between block and street add to the 
 *           start position of the next field and therefore adds to the total bytes when it should not.
 *        2. The view does not show the characters used in delimiters(maybe not enough room for long strings).
 *        3. The pad char and data mask do not line up with titles. The Pad style names vary and throw off the 
 *           next column.
 *     - Enhancment - "LastIssued" values for books are updated as soon as the sequence files unloaded.
 *         This helps defend against duplicated issue numbers in the case of missing or corrupt structure.DAT files
 *         When the structure files is actually posted, it still updates the range books as well, to cover the
 *         case when the sequence file is missing, incomplete or corrupt
 *     - Enhancement - If the ISSUE_AP.CFG is missing or corrupt, data files will be unloaded from the handheld
 *         prior to formatting the device. The downloaded files will not be automatically imported, but the user
 *         will be notified and given instructions on how to import them if desired. If a sequence file was 
 *         downloaded, an attempt will be made to update the "LastIssued" (as described above) in the range books
 *         to prevent possible duplicates. If the sequence file is invalid, it will not be reported as an error
 *         in this condition since its unknown which customer configuration was used in the handheld.
 *     - Bug Fix: Printing "Record View" caused a BSOD/Reboot when using HP LaserJET 4250. Used a different
 *         technique for generating the printable data to work-around this problem.
 *
 * v1.35 - 2007-11-07  Maroochy Special
 *     - Added code to unload data recovery "SAVE_*" files when detected on the handheld
 *     - Enhanced communication thread code to try and defend against sending 
 *       out units when communication have failed to complete. 
 *       "Object Reference Not Set..." is no longer an "acceptable" exception
 *       TODO: determine which are "acceptable" and only allow those through, 
 *             terminating communication on all others
 *     - Fixed AutoTRAX Symbol handheld unloads broken by X3 integration
 * 
 * v1.34
 *      - Added enhancement to RangeManager to suppress prefix/suffix assignment by user when specified in sequence object
 *      - Added code to suppress prefix/suffix checking during citation import
 *      - Cleaned up RangeManager interface
 * 
 * v1.33
 *    ° Keith added ability to export child vios from detail export. (i.e. Export parking violations
 *        in ParkNote export.
 *    ° Alan modified code related to AutoTRAX cache location.
 *    ° Keith added ability to manually launch a scheduled item.
 *    ° Alan added more fields to the advanced inquiry search parameters. Fields are in categories such as
 *        Suspect Info, DL Info, and Location Info fields)
*****************************************/

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
//KLC using System.Data;
using System.IO;
using System.Xml.Serialization;

namespace AutoISSUE
{
    /// <summary>
    /// File contains DB field name constants. This file should be added to
    /// Projects by clicking 'Add Existing Item' and clicking 'Add as link'.
    /// </summary>
    public class DBConstants
    {
        // application name... for message box captions, etc
        public const string cnApplicationName = "AutoISSUE .NET";
        public const string cnApplicationDesc = "Automated Citation Issuance System";

        // version number
        public const string cnSystemRevisionStr = "2.45.0";

        ///////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Default value for WebService call timeouts, in MS
        /// </summary>
        public const int cnStandardWebServiceTimeoutMS = 600000;


        // standard table name extensions, created by taking the the main table name and adding the suffix
        // 2/16/06, mcb: _STATUS table to be replaced by transaction link table in the near future.
        public const string cnStatusTableNameSuffix = "_STATUS"; // only exists on the handheld side
        public const string cnTransactionLinkTableNameSuffix = "_TRANSLINK";

        public const string cnStatusChangeTransactionName = "STATUSCHANGE";
        public const string cnExportTransactionName = "EXPORT";

        public const string cnViolationTableNameSuffix = "_VIOS";
        public const string cnCancelTableNameSuffix = "CANCEL";
        public const string cnReIssueTableNameSuffix = "REISSUE";
        public const string cnContinuanceTableNameSuffix = "CONTINUE";
        public const string cnNotesTableNameSuffix = "NOTE";
        public const string cnGeoCodeTableNameSuffix = "_GC"; //"_GEOCODE"; // GEOCODE makes tablenames too long for Oracle

        // This suffix will be used with DateTime columns in a DataTable where we want the
        // DateTime to be untouched by the XML serializer. Normally, when a DataTable is
        // deserialized, its time portion is adjusted based on the time zone where it
        // was deserialized. These RAW columns will not be adjusted.
        public const string cnRawColumnSuffix = "_RAW";

        // When importing data usually give comm session the serial number of the unit that is being
        // downloaded. Since we are recovering data, we may not have the serial number available.
        // We will try to extract the serial number from the folder name, 
        // but if we can't, we'll use this as the default serial number.
        public const string cnRecoverDefaultSerialNumber = "RECOVERY";

        /// <summary>w
        /// A virtual table generated by the query library to consolidate status detail data for each record type
        /// </summary>
        public const string cnStatusHistoryVirtualTableNameSuffix = "_" + tblNameStatusHistory + "_VIEW";

        /// <summary>
        /// A virtual table generated by the query library to consolidate export detail data for each record type
        /// </summary>
        public const string cnExportHistoryVirtualTableNameSuffix = "_" + tblNameExportHistory + "_VIEW";

        public const String cnNSWStatusVirtualTableNameSuffix = "_" + tblNameNSWStatus + "_VIEW";

        ///////////////////////////////////////////////////////////////////////////////////////////////

        // add this to the end of GetInstalledRootFilePath() to locate the help file
        public const string cnAutoISSUEHelpFileName = "AutoISSUE_NET_Help.CHM";

        #region HelpKeywordConstants
        //
        public const string cnHelpKeyword_Add_a_New_Schedule = "Add_a_New_Schedule.htm";
        public const string cnHelpKeyword_Add_a_Task_Group_and_Task = "Add_a_Task_Group_and_Task.htm";
        public const string cnHelpKeyword_Add_a_Task_Group_and_Task_exportparameters = "Add_a_Task_Group_and_Task_exportparameters.htm";
        public const string cnHelpKeyword_Adding_Editing_and_Deleting_Roles = "Adding,_Editing_and_Deleting_Roles.htm";
        public const string cnHelpKeyword_Adding_Editing_and_Deleting_Users = "Adding,_Editing_and_Deleting_Users.htm";
        public const string cnHelpKeyword_Adding_Items_to_a_List = "Adding_Items_to_a_List.htm";
        public const string cnHelpKeyword_Assigning_Ranges_to_Handhelds = "Assigning_Ranges_to_Handhelds.htm";
        public const string cnHelpKeyword_AutoPROCESS = "AutoPROCESS.htm";
        public const string cnHelpKeyword_Backing_Up_and_Restoring_User_Rights_and_Roles = "Backing_Up_and_Restoring_User_Rights_and_Roles.htm";
        public const string cnHelpKeyword_Changing_the_Order_of_Tasks = "Changing_the_Order_of_Tasks.htm";
        public const string cnHelpKeyword_Changing_Your_Password = "Changing_Your_Password.htm";
        public const string cnHelpKeyword_Citation_Range_Manager = "Citation_Range_Manager.htm";
        public const string cnHelpKeyword_Comm_Log_Viewer = "Comm_Log_Viewer.htm";
        public const string cnHelpKeyword_Configuration_for_an_Export_File = "Configuration_for_an_Export_File.htm";
        public const string cnHelpKeyword_Court_Calendar = "Court_Calendar.htm";
        public const string cnHelpKeyword_Create_an_Allocation = "Create_an_Allocation.htm";
        public const string cnHelpKeyword_Creating_and_Editing_Import_or_Export_Definitions = "Creating_and_Editing_Import_or_Export_Definitions.htm";
        public const string cnHelpKeyword_Delimiters = "Delimiters.htm";
        public const string cnHelpKeyword_Edit_or_Delete_a_Schedule = "Edit_or_Delete_a_Schedule.htm";
        public const string cnHelpKeyword_Editing_Definition_Window_Layout = "Editing_Definition_Window_Layout.htm";
        public const string cnHelpKeyword_Editing_Fields = "Editing_Fields.htm";
        public const string cnHelpKeyword_Editing_Lists = "Editing_Lists.htm";
        public const string cnHelpKeyword_Editing_Lists_viewexternally = "Editing_Lists_viewexternally.htm";
        public const string cnHelpKeyword_Editing_the_Configuration_of_an_AutoCITE_PDA_Unit = "Editing_the_Configuration_of_an_AutoCITE_PDA_Unit.htm";
        public const string cnHelpKeyword_Exiting_AutoISSUE_NET = "Exiting_AutoISSUE_NET.htm";
        public const string cnHelpKeyword_Favorites = "Favorites.htm";
        public const string cnHelpKeyword_Getting_Started = "Getting_Started.htm";
        public const string cnHelpKeyword_Help = "Help.htm";
        public const string cnHelpKeyword_Import_Allocation = "Import_Allocation.htm";
        public const string cnHelpKeyword_Import_and_Export_Definitions = "Import_and_Export_Definitions.htm";
        public const string cnHelpKeyword_Import_Export_Activities = "Import_Export_Activities.htm";
        public const string cnHelpKeyword_Imports_and_Exports_from_the_Import_Export_tab = "Imports_and_Exports_from_the_Import_Export_tab.htm";
        public const string cnHelpKeyword_Inquiry_Result = "Inquiry_Result.htm";
        public const string cnHelpKeyword_List_Effective_Date = "List_Effective_Date.htm";
        public const string cnHelpKeyword_List_Maintenance = "List_Maintenance.htm";
        public const string cnHelpKeyword_List_Properties = "List_Properties.htm";
        public const string cnHelpKeyword_Logging_On_to_AutoISSUE_NET = "Logging_On_to_AutoISSUE_NET.htm";
        public const string cnHelpKeyword_Main_Activity_Icons = "Main_Activity_Icons.htm";
        public const string cnHelpKeyword_Notes = "Notes.htm";
        public const string cnHelpKeyword_Officer_Log_Report = "Officer_Log_Report.htm";
        public const string cnHelpKeyword_Print_Save_and_Email_a_Report = "Print_Save_and_Email_a_Report.htm";
        public const string cnHelpKeyword_Printing_from_the_Inquiry_Result_Window = "Printing_from_the_Inquiry_Result_Window.htm";
        public const string cnHelpKeyword_Record_History = "Record_History.htm";
        public const string cnHelpKeyword_Record_Inquiry = "Record_Inquiry.htm";
        public const string cnHelpKeyword_Reports = "Reports.htm";
        public const string cnHelpKeyword_Reproduction_View = "Reproduction_View.htm";
        public const string cnHelpKeyword_Result_Set_and_Parameters_Used_Tabs = "Result_Set_and_Parameters_Used_Tabs.htm";
        public const string cnHelpKeyword_Revert_to_Current_List = "Revert_to_Current_List.htm";
        public const string cnHelpKeyword_Revoking_and_Assigning_Roles = "Revoking_and_Assigning_Roles.htm";
        public const string cnHelpKeyword_Schedule_Manager = "Schedule_Manager.htm";
        public const string cnHelpKeyword_Scheduler = "Scheduler.htm";
        public const string cnHelpKeyword_Synchronize_Devices = "Synchronize_Devices.htm";
        public const string cnHelpKeyword_System_Options = "System_Options.htm";
        public const string cnHelpKeyword_System_Policy = "System_Policy.htm";
        public const string cnHelpKeyword_Task_Groups = "Task_Groups.htm";
        public const string cnHelpKeyword_Task_Manager = "Task_Manager.htm";
        public const string cnHelpKeyword_Tool_Tips = "Tool_Tips.htm";
        public const string cnHelpKeyword_User_Profiles_and_Security = "User_Profiles_and_Security.htm";
        public const string cnHelpKeyword_Using_Calendars_to_Set_Dates = "Using_Calendars_to_Set_Dates.htm";
        public const string cnHelpKeyword_View_History = "View_History.htm";
        public const string cnHelpKeyword_Viewing_an_Export_File_Definition = "Viewing_an_Export_File_Definition.htm";
        public const string cnHelpKeyword_Viewing_Inquiry_Results = "Viewing_Inquiry_Results.htm";
        public const string cnHelpKeyword_Void_Citation = "Void_Citation.htm";


        // new definitions for AutoTRAX 
        public const string cnHelpKeyword_Accessing_AutoTRAX_Reports = "Accessing_AutoTRAX_Reports.htm";
        public const string cnHelpKeyword_ACW_Tab = "ACW_Tab.htm";
        public const string cnHelpKeyword_Adding_a_Handheld = "Adding_a_Handheld.htm";
        public const string cnHelpKeyword_Auditing = "Auditing.htm";
        public const string cnHelpKeyword_Auditing_Meters = "Auditing_Meters.htm";
        public const string cnHelpKeyword_AutoTRAX_Inventory_and_Transactions = "AutoTRAX_Inventory_and_Transactions.htm";
        public const string cnHelpKeyword_AutoTRAX_Overview = "AutoTRAX_Overview.htm";
        public const string cnHelpKeyword_AutoTRAX_PC_Software = "AutoTRAX_PC_Software.htm";
        public const string cnHelpKeyword_AutoTRAX_Tab = "AutoTRAX_Tab.htm";
        public const string cnHelpKeyword_Backup_and_Restore = "Backup_and_Restore.htm";
        public const string cnHelpKeyword_Computer_Tab = "Computer_Tab.htm";
        public const string cnHelpKeyword_Editing_a_Handheld_Unit = "Editing_a_Handheld_Unit.htm";
        public const string cnHelpKeyword_Editing_and_Adding_Transactions = "Editing_and_Adding_Transactions.htm";
        public const string cnHelpKeyword_Fix_a_Meter_Problem_Immediately = "Fix_a_Meter_Problem_Immediately.htm";
        public const string cnHelpKeyword_Global_Tab = "Global_Tab.htm";
        public const string cnHelpKeyword_Handheld_Guide_Introduction = "Handheld_Guide_Introduction.htm";
        public const string cnHelpKeyword_Implementing_AutoTRAX = "Implementing_AutoTRAX.htm";
        public const string cnHelpKeyword_Inventory = "Inventory.htm";
        public const string cnHelpKeyword_Inventory_Active = "Inventory_–_Active.htm";
        public const string cnHelpKeyword_Inventory_Spares = "Inventory_–_Spares.htm";
        public const string cnHelpKeyword_Locating_Information_in_Inquiry = "Locating_Information_in_Inquiry.htm";
        public const string cnHelpKeyword_Location_Mechanism_Summary = "Location_Mechanism_Summary.htm";
        public const string cnHelpKeyword_Main_Menu = "Main_Menu.htm";
        public const string cnHelpKeyword_Maintenance = "Maintenance.htm";
        public const string cnHelpKeyword_Maintenance_Collection_Routes = "Maintenance_&_Collection_Routes.htm";
        public const string cnHelpKeyword_Meter_Maintenance = "Meter_Maintenance.htm";
        public const string cnHelpKeyword_Ongoing_Operation = "Ongoing_Operation.htm";
        public const string cnHelpKeyword_Operational_Check = "Operational_Check.htm";
        public const string cnHelpKeyword_Parking_Meters = "Parking_Meters.htm";
        public const string cnHelpKeyword_Pre_planning = "Pre_planning.htm";
        public const string cnHelpKeyword_Preparing_a_Handheld = "Preparing_a_Handheld.htm";
        public const string cnHelpKeyword_Programming = "Programming.htm";
        public const string cnHelpKeyword_Programming_AutoTRAX_Handhelds = "Programming_AutoTRAX_Handhelds.htm";
        public const string cnHelpKeyword_Replace_Meter = "Replace_Meter.htm";
        public const string cnHelpKeyword_Report_a_Meter_Problem = "Report_a_Meter_Problem.htm";
        public const string cnHelpKeyword_Restoring_a_Dead_Handheld = "Restoring_a_‘Dead’_Handheld.htm";
        public const string cnHelpKeyword_Saving_AutoTRAX_Reports = "Saving_AutoTRAX_Reports.htm";
        public const string cnHelpKeyword_Symbol_8146_Handheld_Computer = "Symbol_8146_Handheld_Computer.htm";
        public const string cnHelpKeyword_Tools = "Tools.htm";
        public const string cnHelpKeyword_Transaction_Exceptions = "Transaction_Exceptions.htm";

        // end help constant names
        ///////////////////////////////////////////////////////////////////////////////////////////////
        #endregion


        public const string sqlDetailRecNumberStr = "DETAILRECNO";

        public const string sqlRecoveredByHHStr = "RECOVEREDBYHH";

        // Implemented as a "timestamp" in sql server (not actually a time field, just a number),
        // and emulated via triggers in other providers.
        public const string sqlReinoRowVersionStr = "REINOROWVERSION";


        // standard revision columns
        public const string sqlTblEncryptVersionStr = "TBLENCRYPTVERSION";
        public const string sqlEncryptionKeyID = "ENCRYPTIONKEYID";
        public const string sqlFormRevisionNumberStr = "FORMREV";
        public const string sqlFormRevisionNameStr = "FORMNAME";
        public const string sqlTableRevisionNumberStr = "TABLEREV";
        public const string sqlFormAndStrRevisionNumberStr = "FRMANDSTRREV";  // still used?

        public const string sqlRecCreationDateStr = "RECCREATIONDATE";
        public const string sqlRecCreationTimeStr = "RECCREATIONTIME"; // primarily for handheld populated-rows

        public const string sqlStandardExportDateStr = "STANDARDEXPORTDATE";

        //  2/16/06 - ExportUniqueID will be deprecated shortly.  TransactionKey replaces it (see Transactions.doc for explanation).
        public const string sqlExportUniqueIDStr = "EXPORTUNIQUEID";
        public const string sqlTransactionKeyStr = "TRANSACTIONKEY";
        public const string sqlTransactionNameStr = "TRANSACTIONNAME";


        /// <summary>
        /// Primary key column for all datatables, lists, and many system tables
        /// </summary>
        public const string sqlIssueUniqueKeyStr = "UNIQUEKEY";
        public const string sqlUniqueKeyStr = "UNIQUEKEY";
        public const string sqlMasterKeyStr = "MASTERKEY";
        public const string sqlOccurenceNumberStr = "OCCURNO";
        public const string sqlOccurNoStr = "OCCURNO";
        /// <summary>
        /// This is a host-side only column that is added to some tables (REISSUED, PUBLICCONTACT)
        /// to reference the associated record
        /// </summary>
        public const string sqlSourceIssueMasterKeyStr = "SRCMASTERKEY";



#if __ANDROID__
        // some key columns in in the SQLite database
        public const string ID_COLUMN = "_id";                  // has to have this name for some adapter classes to work. case sensitive!
        public const string STATUS_COLUMN = "STATUS";
        public const string SEQUENCE_ID = "SEQUENCEID";
        public const string WS_STATUS_COLUMN = "WSSTATUS";
        public const string SRCINUSE_FLAG = "INUSEFLAG";
#endif


        public const string gPanelDividerNameSuffix = "PANEL Divider";



        // all key fields UNIQUEKEY, MASTERKEY, etc
        // uniform terms on the host side so that dataset relations, merges and similar functions will work
        // once this was set at 12 so we could force the datatype to be a double
        // however, as of .NET 2.0 only INTs can be set to AutoIncrement, and so
        // we're forced to live with this size until MS allows DOUBLES to be AutoIncrement
        public const int cnPrimaryKeyUniformSize = 9;
        /// <summary>
        /// Key fields which must be forced to the cnPrimaryKeyUniformSize 
        /// </summary>
        public static readonly string[] cnUniformKeyFieldsArray = 
            {
                sqlUniqueKeyStr, 
                sqlMasterKeyStr, 
                sqlSourceIssueMasterKeyStr,
                sqlTransactionKeyStr,
                sqlOccurenceNumberStr
            };

        public const string sqlIssueNumberPrefixStr = "ISSUENOPFX";
        public const string sqlIssueNumberStr = "ISSUENO";
        public const string sqlIssueNumberSuffixStr = "ISSUENOSFX";
        public const string sqlIssueNumberCheckDigitStr = "ISSUENOCHKDGT";
        public const string sqlControlNumberStr = "CONTROLNO"; // Control # used by L.A. Abandoned Veh module
        public const string sqlOrderTypeCodeStr = "ORDERTYPECODE"; // Work Order Type used by L.A. Abandoned Veh module

        public const string sqlIssueDateStr = "ISSUEDATE";
        public const string sqlIssueTimeStr = "ISSUETIME";
        public const string sqlIssueAgencyStr = "AGENCY";
        public const string sqlIssueBeatStr = "BEAT";

        public const string sqlIssueOfficerNameStr = "OFFICERNAME";
        public const string sqlIssueOfficerIDStr = "OFFICERID";
        public const string sqlOfficerDaysOffStr = "OFFICERDAYSOFF";
        public const string sqlOfficerVacationStr = "OFFICERVACATION";

        public const string sqlOfficerPayrollNoStr = "OFFICERPAYROLLNO";

        public const string sqlArrestOfficerNameStr = "ARRESTOFFICERNAME";
        public const string sqlArrestOfficerIDStr = "ARRESTOFFICERID";
        public const string sqlArrestOfficerDaysOffStr = "ARROFFICERDAYSOFF";
        public const string sqlArrestOfficerVacationStr = "ARROFFICERVACATION";

        public const string sqlVehLicNoStr = "VEHLICNO";
        public const string sqlVehVINStr = "VEHVIN";
        public const string sqlVehVIN8Str = "VEHVIN4"; // we sometime capture up to 8 characters, but the traditional fieldname is VEHNIN4
        public const string sqlVehLicStateStr = "VEHLICSTATE";
        public const string sqlVehLicExpDateStr = "VEHLICEXPDATE";
        public const string sqlVehPlateTypeStr = "VEHLICTYPE";
        public const string sqlVehMakeStr = "VEHMAKE";
        public const string sqlVehModelStr = "VEHMODEL";
        public const string sqlVehBodyTypeStr = "VEHBODYSTYLE";
        public const string sqlVehColor1Str = "VEHCOLOR1";
        public const string sqlVehColor2Str = "VEHCOLOR2";
        public const string sqlVehPermitNumberStr = "PERMITNO";
        public const string sqlVehLabelNumberStr = "VEHLABELNO"; // aussie 
        public const string sqlVehDecalNumberStr = "VEHDECALNO"; // us traffic
        public const string sqlVehDecalNumberSuffixStr = "VEHDECALNOSUFFIX"; // for states that have it
        public const string sqlVehCheckDigitStr = "VEHCHECKDIGIT";
        public const string sqlVehYearDateStr = "VEHYEARDATE";
        public const string sqlVehInfoStr = "VEHINFO";
        public const string sqlVehInspectionStickerTypeStr = "VEHINSPECTIONSTICKERTYPE";
        public const string sqlVehInspectionStickerNumberStr = "VEHINSPECTIONSTICKERNUMBER";
        public const string sqlVehInspectionStickerExpDateStr = "VEHINSPECTIONSTICKEREXPDATE";

        public const string sqlLocLotStr = "LOCLOT";
        public const string sqlLocBlockStr = "LOCBLOCK";
        public const string sqlLocStreetStr = "LOCSTREET";
        public const string sqlLocDescriptorStr = "LOCDESCRIPTOR";

        // public const string sqlLocStreetTypeStr = "LOCSTREETTYPE";  // TODO - copy this back to AI host? No, it shold be LOCDESCRIPTOR


        public const string sqlLocDirectionStr = "LOCDIRECTION";
        public const string sqlLocCrossStreet1Str = "LOCCROSSSTREET1";
        public const string sqlLocCrossStreet2Str = "LOCCROSSSTREET2";
        public const string sqlLocSideOfStreetStr = "LOCSIDEOFSTREET";
        public const string sqlLocSuburbStr = "LOCSUBURB";
        public const string sqlLocCityStr = "LOCCITY";
        public const string sqlLocCountyStr = "LOCCOUNTY";
        public const string sqlLocCountyCodeStr = "LOCCOUNTYCODE";
        public const string sqlLocStateStr = "LOCSTATE";
        public const string sqlLocZipStr = "LOCZIP";
        //public const string sqlMeterNumberStr = "LOCMETER";    //are these two different?
        public const string sqlLocMeterNumberStr = "LOCMETER";    //are these two different?

        public const string sqlMeterNumberStr = "METERNO";
       // public const string sqlLocMeterNumberStr = "METERNO";
        public const string sqlLocMeterBayNumberStr = "METERBAYNO";
        public const string sqlLocTravelDirStr = "LOCTRAVELDIR";
        public const string sqlLocTurnDirStr = "LOCTURNDIR";


        public const string sqlSuspLastNameStr = "SUSPLASTNAME";
        public const string sqlSuspFirstNameStr = "SUSPFIRSTNAME";
        public const string sqlSuspMiddleNameStr = "SUSPMIDNAME";
        public const string sqlSuspSuffixNameStr = "SUSPSFXNAME";
        public const string sqlSuspAddrStreetNoStr = "SUSPADDRSTREETNO";
        public const string sqlSuspAddrStreetStr = "SUSPADDRSTREET";
        public const string sqlSuspAddrAptNoStr = "SUSPADDRAPTNO";
        public const string sqlSuspAddrCityStr = "SUSPADDRCITY";
        public const string sqlSuspAddrStateStr = "SUSPADDRSTATE";
        public const string sqlSuspAddrZipStr = "SUSPADDRZIP";
        public const string sqlSuspPhoneStr = "SUSPPHONE";
        public const string sqlSuspAgeStr = "SUSPAGE";
        public const string sqlSuspIsJuvenileStr = "SUSPJUVENILE";
        public const string sqlSuspRaceStr = "SUSPRACE";
        public const string sqlSuspGenderStr = "SUSPGENDER";
        public const string sqlSuspHeightStr = "SUSPHEIGHT";
        public const string sqlSuspWeightStr = "SUSPWEIGHT";
        public const string sqlSuspHairColorStr = "SUSPHAIRCOLOR";
        public const string sqlSuspEyeColorStr = "SUSPEYECOLOR";
        public const string sqlSuspOtherIDStr = "SUSPOTHERID";
        public const string sqlSuspSignatureStr = "SUSPSIGNATURE";
        public const string sqlSuspSSNStr = "SUSPSSN";


        // for the OTHER/ANIMAL  citation types (aussie standard)
        public const string sqlOffenderLastNameStr = "OFFENDERLASTNAME";
        public const string sqlOffenderFirstNameStr = "OFFENDERFIRSTNAME";
        public const string sqlOffenderMiddleNameStr = "OFFENDERMIDNAME";
        public const string sqlOffenderSuffixNameStr = "OFFENDERSFXNAME";
        public const string sqlOffenderAddrStreetNoStr = "OFFENDERBLOCK";
        public const string sqlOffenderAddrStreetStr = "OFFENDERSTREET";
        public const string sqlOffenderAddrAptNoStr = "OFFENDERADDRAPTNO";
        public const string sqlOffenderAddrCityStr = "OFFENDERSUBURB";
        public const string sqlOffenderAddrStateStr = "OFFENDERADDRSTATE";
        public const string sqlOffenderAddrZipStr = "OFFENDERPOSTCODE";
        public const string sqlOffenderPhoneStr = "OFFENDERPHONE";
        public const string sqlOffenderAgeStr = "OFFENDERAGE";
        public const string sqlOffenderIsJuvenileStr = "OFFENDERJUVENILE";
        public const string sqlOffenderRaceStr = "OFFENDERRACE";
        public const string sqlOffenderGenderStr = "OFFENDERGENDER";
        public const string sqlOffenderHeightStr = "OFFENDERHEIGHT";
        public const string sqlOffenderWeightStr = "OFFENDERWEIGHT";
        public const string sqlOffenderHairColorStr = "OFFENDERHAIRCOLOR";
        public const string sqlOffenderEyeColorStr = "OFFENDEREYECOLOR";
        public const string sqlOffenderOtherIDStr = "OFFENDEROTHERID";
        public const string sqlOffenderSignatureStr = "OFFENDERSIGNATURE";
        public const string sqlOffenderSSNStr = "OFFENDERSSN";


        public const string sqlAnimalRegStr = "ANIMALREG";
        public const string sqlAnimalTypeStr = "ANIMALTYPE";
        public const string sqlAnimalNameStr = "ANIMALNAME";
        public const string sqlAnimalBreedStr = "ANIMALBREED";
        public const string sqlAnimalGenderStr = "ANIMALGENDER";
        public const string sqlAnimalColorStr = "ANIMALCOLOUR";  // this should be US spelling??


        public const string sqlDLNumberStr = "DLNUM";
        public const string sqlDLStateStr = "DLSTATE";
        public const string sqlDLClassStr = "DLCLASS";
        public const string sqlDLRestrictionsStr = "DLRESTRICTIONS";
        public const string sqlDLEndorsementsStr = "DLENDORSEMENTS";
        public const string sqlDLExpDateStr = "DLEXPDATE";
        public const string sqlDLBirthDateStr = "DLBIRTHDATE";
        public const string sqlDLSwipedStr = "DLSWIPED";

        public const string sqlTrafficDivisionStr = "DIVISION";
        public const string sqlAgencyCodeStr = "AGENCYCODE";
        public const string sqlTrafficWatchStr = "WATCH";
        public const string sqlTrafficDetailStr = "DETAIL";
        public const string sqlTicketTypeStr = "TICKETTYPE";
        public const string sqlTicketTypeCodeStr = "TICKETTYPECODE";
        public const string sqlTrafficCaseNoStr = "CASENO";


        public const string sqlApproxSpeedStr = "APPROXSPEED";
        public const string sqlPFMaxSpeedStr = "PFMAXSPEED";
        public const string sqlVehSpeedLimitStr = "VEHSPEEDLIMIT";
        public const string sqlSafeSpeedStr = "SAFESPEED";
        public const string sqlRadarUsedStr = "RADARUSED";
        public const string sqlOverweightStr = "OVERWEIGHT";
        public const string sqlAccidentStr = "ACCIDENT";
        public const string sqlCommVehStr = "COMMVEH";
        public const string sqlHazMatStr = "HAZMAT";
        public const string sqlConstructionZoneStr = "CONSTRUCTIONZONE";
        public const string sqlWorkersPresentStr = "WORKERSPRESENT";
        public const string sqlSchoolZoneStr = "SCHOOLZONE";
        public const string sqlSchoolZoneTimeStr = "SCHOOLZONETIMES";
        public const string sqlFinRespStr = "FINRESP";
        public const string sqlPolicyStr = "POLICY";
        public const string sqlSignedStr = "SIGNED";
        public const string sqlStepStr = "STEP";
        public const string sqlCommLicStr = "COMMLIC";
        public const string sqlPlacardedStr = "PLACARDED";
        public const string sqlArrestedStr = "ARRESTED";
        public const string sqlReportNoStr = "REPORTNO";
        public const string sqlRadarSerianNoStr = "RADARSN";
        public const string sqlRadarCalibDateStr = "RADARCALIBDATE";
        public const string sqlSpeedoCalibDateStr = "SPEEDOCALIBDATE";
        public const string sqlOfficerPresentStr = "OFFICERPRESENT";
        public const string sqlOfficerSignatureFieldNameStr = "OFFICERSIGNATURE";
        public const string sqlPassengersStr = "PASSENGERS";
        public const string sqlWeatherCondStr = "WEATHERCOND";
        public const string sqlLightingCondStr = "LIGHTINGCOND";
        public const string sqlStreetCondStr = "STREETCOND";
        public const string sqlTrafficCondStr = "TRAFFICCOND";
        public const string sqlMisdemeandorStr = "MISDEMEANOR";
        public const string sqlOwnerResponsibleStr = "OWNERRESPONSIBLE";
        public const string sqlSignatureRequiredStr = "SIGNATUREREQD";
        public const string sqlFingerprintRequiredStr = "FINGERPRINTREQD";
        public const string sqlFingerPrintedStr = "FINGERPRINTED";

        public const string sqlTrafBusinessNameStr = "BUSINESSNAME";
        public const string sqlTrafBusinessAddrStreetNoStr = "BUSINESSADDRSTREETNO";
        public const string sqlTrafBusinessAddrStreetStr = "BUSINESSADDRSTREET";
        public const string sqlTrafBusinessAddrAptNoStr = "BUSINESSADDRAPTNO";
        public const string sqlTrafBusinessAddrCityStr = "BUSINESSADDRCITY";
        public const string sqlTrafBusinessAddrStateStr = "BUSINESSADDRSTATE";
        public const string sqlTrafBusinessAddrZipStr = "BUSINESSADDRZIP";
        public const string sqlTrafBusinessPhoneStr = "BUSINESSPHONE";

        public const string sqlTrafRONameLastStr = "ROLASTNAME";
        public const string sqlTrafRONameFirstStr = "ROFIRSTNAME";
        public const string sqlTrafRONameMiddleStr = "ROMIDNAME";
        public const string sqlTrafRONameSuffixStr = "ROSFXNAME";
        public const string sqlTrafROAddrStreetNoStr = "ROADDRSTREETNO";
        public const string sqlTrafROAddrStreetStr = "ROADDRSTREET";
        public const string sqlTrafROAddrAptNoStr = "ROADDRAPTNO";
        public const string sqlTrafROAddrCityStr = "ROADDRCITY";
        public const string sqlTrafROAddrStateStr = "ROADDRSTATE";
        public const string sqlTrafROAddrZipStr = "ROADDRZIP";
        public const string sqlTrafROPhoneStr = "ROPHONE";
        public const string sqlDLPresentedStrStr = "DLPRESENTED";



        //Courts
        public const string tblNameCourtName = "COURTNAME";
        public const string sqlCourtNameStr = "COURTNAME";
        public const string sqlCourtDateStr = "COURTDATE";
        public const string sqlCourtTimeStr = "COURTTIME";
        public const string sqlCourtAddrStr = "COURTADDR";
        public const string sqlCourtAddr2Str = "COURTADDR2";
        public const string sqlCourtPhoneStr = "COURTPHONE";
        public const string sqlCourtCodeStr = "COURTCODE";
        public const string sqlCourtLevelStr = "COURTLEVEL";
        public const string sqlCourtDatePromptStr = "COURTDATEPROMPT";
        public const string sqlWillBeNotifiesStr = "WILLBENOTIFIED";
        public const string sqlNightCourtOKStr = "NIGHTCOURTOK";


        // public contact specialties
        public const string sqlActionTakenStr = "ACTIONTAKEN";
        public const string sqlActionDetailStr = "ACTIONDETAIL";
        public const string sqlActionTakenStructStr = "ACTIONTAKENSTRUCT";
        public const string sqlActionTakenFormStr = "ACTIONTAKENFORM";
        public const string sqlSearchedStr = "SEARCHED";
        public const string sqlSearchTypeStr = "SEARCHTYPE";
        public const string sqlConsentToSearchStr = "CONSENTTOSEARCH";
        public const string sqlResidentStr = "RESIDENT";
        public const string sqlStopReasonStr = "STOPREASON";  // This is also known as "ProbableCause"
        public const string sqlStopTypeStr = "STOPTYPE";
        public const string sqlContrabandStr = "CONTRABAND";
        public const string sqlContrabandTypeStr = "CONTRABANDTYPE";
        public const string sqlNote1Str = "NOTE1";
        public const string sqlNote2Str = "NOTE2";


        // meter / damaged sign status
        public const string sqlMeterStatusStr = "METERSTATUS";
        public const string sqlDamagedSignNameStr = "SIGN";
        public const string sqlDamagedSignStatusStr = "SIGNSTATUS";



        // a placeholder field for multimedia attachment info
        public const string sqlPendingAttachmentsStr = "PENDINGATTACHMENTS";


        public const string sqlPersonAccountKeyStr = "VEHLABELNO";//= "PERSONACOUNTKEY"; use a dummy column until this is defined

        public const string sqlDueDateStr = "DUEDATE";
        public const string sqlRemark1Str = "REMARK1";
        public const string sqlRemark2Str = "REMARK2";



        // tow specialties
        //public const string sqlTOWNOStr = "";
        //public const string sqlTOWNOPFXStr = "";

        public const string sqlTowColumnStr = "TOWCOLUMN";
        public const string sqlTOTALBURNStr = "TOTALBURN";
        public const string sqlELECTLOCKSStr = "ELECTLOCKS";
        public const string sqlFLATTIRESAMTStr = "FLATTIRESAMT";
        public const string sqlTIREMISSINGAMTStr = "TIREMISSINGAMT";
        public const string sqlHUBCAPSMISSINGAMTStr = "HUBCAPSMISSINGAMT";
        public const string sqlSURFACEDAMAGEStr = "SURFACEDAMAGE";
        public const string sqlDENTEDStr = "DENTED";
        public const string sqlCRUSHEDStr = "CRUSHED";
        public const string sqlRADIOStr = "RADIO";
        public const string sqlTAPEPLAYERStr = "TAPEPLAYER";
        public const string sqlSPEAKERSStr = "SPEAKERS";
        public const string sqlDISTRICTStr = "DISTRICT";
        public const string sqlILLEGALLYPARKEDStr = "ILLEGALLYPARKED";
        public const string sqlSAFEKEEPINGStr = "SAFEKEEPING";
        public const string sqlSTOLENStr = "STOLEN";
        public const string sqlABANDONEDStr = "ABANDONED";
        public const string sqlTowCaseNoStr = "CASENO";
        public const string sqlTRUNKStr = "TRUNK";
        public const string sqlGLOVECOMPStr = "GLOVECOMP";
        public const string sqlSTOLENCHECKStr = "STOLENCHECK";
        public const string sqlDOORSStr = "DOORS";




        // The User Defined Export Date fields will be dynamically created based on the number of
        // these fields we want. So, in future, if want to expand our tables to handle more than
        // 5 of these fields, will just have to change the cnNumberOfUserDefinedExportDateFields value.
        // Each user defined field will be its prefix USERDEF, plus the field number, plus the
        // suffix EXPORTDATE (this is to keep it consistent with previous naming convention).

        public const int cnNumberOfUserDefinedExportDateFields = 5;
        public const string cnFieldUserDefinedExportDatePrefix = "USERDEF";
        public const string cnFieldUserDefinedExportDateSuffix = "EXPORTDATE";
        /* The above replaces these static field names. If want to get all names, call 
                public const string sqlUserDefined1ExportDateStr = "USERDEF1EXPORTDATE";
                public const string sqlUserDefined2ExportDateStr = "USERDEF2EXPORTDATE";
                public const string sqlUserDefined3ExportDateStr = "USERDEF3EXPORTDATE";
                public const string sqlUserDefined4ExportDateStr = "USERDEF4EXPORTDATE";
                public const string sqlUserDefined5ExportDateStr = "USERDEF5EXPORTDATE";
         */

        // The Custom Export Date fields will by dynamically created based on the number of
        // these fields we want. These fields are like UserDefined export date fields, but
        // are used for multi-datatype exports managed vi a plug-in DLLs (i.e. OMS export for Los Angeles)
        public const int cnNumberOfCustomExportDateFields = 3;
        public const string cnFieldCustomExportDatePrefix = "CUSTOM";
        public const string cnFieldCustomExportDateSuffix = "EXPORTDATE";

        public const string sqlWirelessAutoPROCExportDate = "WIRELESSEXPORTDATE_AUTOPROC";
        public const string sqlAutoProcUniqueKeyStr = "AUTOPROC_UNIQUEKEY";

        public const string sqlVoidStatusStr = "VOIDSTATUS";
        public const string sqlVoidStatusDateStr = "VOIDSTATUSDATE";
        // This will be an aliased field. The time will actually be stored in the VOIDSTATUSDATE field
        public const string sqlVoidStatusTimeStr = "VOIDSTATUSTIME";
        public const string sqlVoidReasonStr = "VOIDREASON";
        public const string sqlVoidedInFieldStr = "VOIDEDINFIELD";
        public const string sqlReissuedStr = "REISSUED";
        public const string sqlIsWarningStr = "ISWARNING";

        public const string sqlMultipleIssuanceStr = "MULTIPLEISSUANCE";
        public const string sqlMultipleIssueRefNoStr = "MULTIISSUEREFNO";


        // database values to represent affirmitave or negative - fixed regardless of language
        public const string cnDBYes = "Y";
        public const string cnDBNo = "N";

        // mark mode specific
        public const string sqlTireStemsFrontTimeStr = "TIRESTEMSFRONTTIME";
        public const string sqlTireStemsRearTimeStr = "TIRESTEMSREARTIME";
        public const string sqlTireStem1DescriptionFieldName = "TIRESTEM1";
        public const string sqlTireStem2DescriptionFieldName = "TIRESTEM2";
        public const string sqlChalkTimeStr = "CHALKTIME";
        public const string sqlChalkDateStr = "CHALKDATE";


        // hot disposition
        public const string sqlHotCodeStr = "HOTCODE";
        public const string sqlHotDispositionStr = "HOTDISPO";

        // relationship specialties - public contact, reissued
        public const string sqlSourceIssueNumberPrefixStr = "SRCISSUENOPFX";
        public const string sqlSourceIssueNumberStr = "SRCISSUENO";
        public const string sqlSourceIssueNumberSuffixStr = "SRCISSUENOSFX";
        public const string sqlSourceIssueDateStr = "SRCISSUEDATE";
        public const string sqlSourceIssueTimeStr = "SRCISSUETIME";


        // cancellations
        public const string sqlCancelReasonStr = "CANCELREASON";


        // status table
        public const string tblNameStatusHistory = "STATUSHISTORY";
        public const string sqlRecordStatusNameStr = "STATUSNAME";
        public const string sqlRecordStatusDateStr = "STATUSDATE";
        public const string sqlStatusStructureStr = "STATUSSTRUCTURE";
        public const string sqlRecordStatusOfficerIDStr = "STATUSOFFICERID";
        public const string sqlRecordStatusOfficerNameStr = "STATUSOFFICERNAME";
        public const string sqlRecordStatusReasonStr = "STATUSREASON";
        // These are from old _Status table. Will not be included in new StatusHistory table.
        public const string sqlRecordStatusStr = "STATUS";
        public const string sqlRecordStatusTimeStr = "STATUSTIME";
        public const string sqlRecordStatusValueStr = "STATUSVALUE";
        public const string sqlRecordEntryStatusStr = "ENTRYSTATUS";
        public const string sqlStatusEventIDStr = "STATUSEVENTID";

        // Some statuses used throughout system.
        public const string cnStatusVoid = "VO";
        public const string cnStatusValid = "XX";

        //activity log
        public const string sqlStartDateStr = "STARTDATE";
        public const string sqlStartTimeStr = "STARTTIME";
        public const string sqlEndDateStr = "ENDDATE";
        public const string sqlEndTimeStr = "ENDTIME";
        public const string sqlPrimaryActivityNameStr = "PRIMARYACTIVITYNAME";
        public const string sqlPrimaryActivityCountStr = "PRIMARYACTIVITYCOUNT";
        public const string sqlSecondaryActivityNameStr = "SECONDARYACTIVITYNAME";
        public const string sqlSecondaryActivityCountStr = "SECONDARYACTIVITYCOUNT";
        public const string sqlExtraPrompt1Str = "EXTRAPROMPT1";
        public const string sqlExtraData1Str = "EXTRADATA1";
        public const string sqlExtraPrompt2Str = "EXTRAPROMPT2";
        public const string sqlExtraData2Str = "EXTRADATA2";
        public const string sqlExtraPrompt3Str = "EXTRAPROMPT3";
        public const string sqlExtraData3Str = "EXTRADATA3";
        public const string sqlStartLatitudeStr = "START_LOCLATITUDE";
        public const string sqlStartLongituteStr = "START_LOCLONGITUDE";
        public const string sqlEndLatitudeStr = "END_LOCLATITUDE";
        public const string sqlEndLongitudeStr = "END_LOCLONGITUDE";


        // violations... sub table
        public const string sqlVioCodeStr = "VIOCODE";
        public const string sqlVioXferCodeStr = "VIOXFERCODE";
        public const string sqlVioDescription1Str = "VIODESCRIPTION1";
        public const string sqlVioDescription2Str = "VIODESCRIPTION2";
        public const string sqlVioTransDescStr = "VIOTRANDESCRIPTION";
        public const string sqlVioFineStr = "VIOFINE";
        public const string sqlVioLateFee1Str = "VIOLATEFEE1";
        public const string sqlVioLateFee2Str = "VIOLATEFEE2";
        public const string sqlVioLateFee3Str = "VIOLATEFEE3";
        public const string sqlVioQueryTypeStr = "VIOQUERYTYPE";
        public const string sqlVioSelectStr = "VIOSELECT";
        public const string sqlVioSectionStr = "VIOSECTION";
        public const string sqlVioCourtStr = "VIOCOURT";
        public const string sqlVioActStr = "VIOACT";
        public const string sqlVioActLine1Str = "ACTLINE1";
        public const string sqlVioActLine2Str = "ACTLINE2";
        public const string sqlVioIsCorrectableStr = "VIOISCORRECTABLE";
        public const string sqlVioIsInfractionStr = "VIOISINFRACTION";
        public const string sqlVioTypeStr = "VIOTYPE";


        // notes sub table
        public const string sqlNoteDateStr = "NOTEDATE";
        public const string sqlNoteTimeStr = "NOTETIME";
        public const string sqlNotesMemoStr = "NOTESMEMO";
        public const string sqlMultimediaNoteDataTypeStr = "MULTIMEDIANOTEDATATYPE";
        public const string sqlMultimediaNoteFileNameStr = "MULTIMEDIANOTEFILENAME";
        public const string sqlMultimediaNoteDataStr = "MULTIMEDIANOTEDATA";
        public const string sqlMultimediaNoteFileDateStampStr = "MULTIMEDIANOTEFILEDATESTAMP";
        public const string sqlMultumediaNoteFileTimeStampStr = "MULTIMEDIANOTEFILETIMESTAMP";
        //Ayman S. Oct/2012: Added support for SDSU project
        public const string sqlPrintedImageOrderNameStr = "PRINTEDIMAGEORDER";
        public const string sqlDiagramStr = "DIAGRAM";


        // geocode sub table
        public const string sqlGeoCodeSourceLayerNameStr = "GEOCODE_SOURCELAYER";
        public const string sqlGeoCodeLabelStr = "GEOCODE_LABEL";
        public const string sqlGeoCodeValueStr = "GEOCODE_VALUE";
        public const string sqlGeoCodeResultMessageStr = "GEOCODE_RESULTMESSAGE";
        public const string sqlGeoCodeDateStr = "GEOCODE_DATE";
        public const string sqlGeoCodeTimeStr = "GEOCODE_TIME";



        #region Android_Added_Constants


        // AutoISSUE client file system management table
        public const string sqlAutoISSUEFileSystemTableName = "AI_FILESYSTEM";
        public const string sqlAutoISSUEFileSystemFileName = "FILENAME";
        public const string sqlAutoISSUEFileSystemFileSize = "FILESIZE";
        public const string sqlAutoISSUEFileSystemFileCreationTimeStamp = "FILECREATIONDATE";
        public const string sqlAutoISSUEFileSystemFileModifiedTimeStamp = "FILEMODIFIEDDATE";
        public const string sqlAutoISSUEFileSystemFileData = "FILEDATA";

        // for preferences that are universal across forms
        public const string cnDefaultSharedPreferencesGlobalPrefix = "GLOBAL";


        // enforcement info captured from the meter used for enforcement
        public const string sqlEnforcedMeterIDFieldName = "ENF_METERID";
        public const string sqlEnforcedMeterIDInternalFieldName = "ENF_METERID_INTERNAL"; // Used for multivendor (ENF_METERID is the name, and ENF_METERID_INTERNAL is the real ID)
        public const string sqlEnforcedMeterBayNoFieldName = "ENF_METERBAYNO";
        public const string sqlEnforcedMeterBayNoInternalFieldName = "ENF_METERBAYNO_INTERNAL"; // Used for multivendor (ENF_METERBAYNO is the name, and ENF_METERBAYNO_INTERNAL is the real ID)
        public const string sqlEnforcedMeterTypeFieldName = "ENF_METERTYPE"; // Used for multivendor mode
        public const string sqlEnforcedMeterScanDateTimeFieldName = "ENF_METERSCANDATETIME";
        public const string sqlEnforcedMeterRTCFieldName = "ENF_METERRTCDATETIME";
        public const string sqlEnforcedMeterLUTFieldName = "ENF_METERLUTDATETIME";
        public const string sqlEnforcedMeterModeFieldName = "ENF_METERMODE";
        public const string sqlEnforcedMeterClusterIDFieldName = "ENF_METERCLUSTERID";
        public const string sqlEnforcedMeterClusterMembersFieldName = "ENF_METERCLUSTERMEMBERS";
        public const string sqlEnforcedMeterBayExpiredMinutesFieldName = "ENF_BAYEXPIREDMINUTES";
        public const string sqlEnforcedMeterBayStatusCodeFieldName = "ENF_BAYSTATUSCODE";
        public const string sqlEnforcedMeterBayStateCodeFieldName = "ENF_BAYSTATECODE";
        public const string sqlEnforcedMeterHandheldModeFieldName = "ENF_HANDHELDMODE";


        // enforcement info captured from PayByCell enforcement, such as Verrus
        public const string sqlPBC_ENF_ZONEIDFieldName = "PBC_ENF_ZONEID";
        public const string sqlPBC_ENF_LICPLATEFieldName = "PBC_ENF_LICPLATE";
        public const string sqlPBC_ENF_LICSTATEFieldName = "PBC_ENF_LICSTATE";
        public const string sqlPBC_ENF_REASONFieldName = "PBC_ENF_REASON";
        public const string sqlPBC_DATA_AGEFieldName = "PBC_DATA_AGE";
        public const string sqlPBC_CONFIRMATION_CODEFieldName = "PBC_CONFIRMATION_CODE";


        // bay status data age (in seconds) relative to the ticket print time. 
        // a negative number indicates the data is from before the print.
        public const string sqlEnforcedMeterREINO_DATA_AGE = "REINO_DATA_AGE";

        // EXPIRED, CONFIRMED, UNCONFIRMED, NOTEXPIRED, METEROOD, UNKNOWN 
        public const string sqlEnforcedMeterREINO_CONFIRMATION_CODE = "REINO_CONFIRMATION_CODE";

        // reino_meter_id which can be different from the displayed meter description.
        public const string sqlEnforcedMeterREINO_METER_ID = "REINO_METER_ID";


        public const string cnHandheldMeterEnforcementMode_Infrared = "IR";
        public const string cnHandheldMeterEnforcementMode_GPRS = "GPRS";

        public const string cnMeterModePAM = "PAM";
        public const string cnMeterModeSneakerNet = "SNEAKER";



        // Button Name constants
        public const string BtnDoneName = "BTNDONE";
        public const string BtnPrintName = "BTNPRINT";
        public const string BtnNotesName = "BTNNOTES";
        public const string BtnCorrectionName = "BTNCORRECTION";
        public const string BtnIssueMoreName = "BTNISSUEMORE";
        public const string BtnIssueMultipleName = "BTNISSUEMULTIPLE";
        public const string BtnEndIssueMultipleName = "BTNENDISSUEMULTIPLE";
        public const string BtnCancelName = "BTNCANCEL";
        public const string BtnVoidName = "BTNVOID";
        public const string BtnReissueName = "BTNREISSUE";
        public const string BtnSuspSignatureName = "BTNSUSPSIGNATURE";
        public const string BtnOfficerSignatureName = "BTNOFFICERSIGNATURE";
        public const string BtnContinuanceName = "BTNCONTINUANCE";
        public const string BtnNewName = "BTNNEW";
        public const string BtnIssueChildName = "BTNISSUECHILD";
        public const string BtnReadReinoName = "BTNREADREINO";
        public const string BtnDiagram = "BtnDiagram";
        public const string BitBtnDiagram = "BITBTNDIAGRAM";
        public const string BtnVoiceNote = "BITBTNVOICENOTE";
        public const string BtnPictureNote = "BITBTNPICTURENOTE";
        public const string BtnVoiceNotePlayPause = "BITBTNVOICENOTEPLAYPAUSE";
        public const string BtnVoiceNoteRecord = "BITBTNVOICENOTERECORD";
        public const string BtnVoiceNoteStop = "BITBTNVOICENOTESTOP";
        public const string BtnVoiceNoteFastForward = "BITBTNVOICENOTEFASTFORWARD";
        public const string BtnVoiceNoteRewind = "BITBTNVOICENOTEREWIND";
        public const string BtnVoiceNoteVolume = "BITBTNVOICENOTEVOLUME";
        public const string BitBtnAttach = "BITBTNATTACH";
        public const string BmpThumbnailPreview = "BMPTHUMBNAILPREVIEW";


        #endregion



        public const string cnSecurityUserTableName = "SECURITY_USER";

        // metertrax specific
        public const string sqlMeterTrax_DeviceModeStr = "DEVICEMODE";
        public const string sqlMeterTrax_MechSerialStr = "MECHSERIALNUMBER";
        public const string sqlMeterTrax_ZoneIDStr = "ZONEID";
        public const string sqlMeterTrax_ZoneDescStr = "ZONEDESC";
        public const string sqlMeterTrax_LocationIDStr = "LOCATIONID";

        public const string sqlMeterTrax_SpecialEventTag1Str = "SPECIALEVENTTAG1";
        public const string sqlMeterTrax_SpecialEventTag2Str = "SPECIALEVENTTAG2";
        public const string sqlMeterTrax_SpecialEventTag3Str = "SPECIALEVENTTAG3";

        public const string sqlMeterTrax_LocationGeneratedStr = "LOCATIONGENERATED";
        public const string sqlMeterTrax_AuditAmountCashStr = "AUDITAMOUNTCASH";
        public const string sqlMeterTrax_AuditAmountDebitStr = "AUDITAMOUNTDEBIT";
        public const string sqlMeterTrax_RegisterReadCashStr = "REGREADCASH";
        public const string sqlMeterTrax_RegisterReadDebitStr = "REGREADDEBIT";
        public const string sqlMeterTrax_AuditDeviceResetStr = "AUDITDEVICERESET";
        public const string sqlMeterTrax_EagleErrorCodeStr = "EAGLEERRORCODE";
        public const string sqlMeterTrax_BatteryVoltageStr = "BATTERYVOLTAGE";
        public const string sqlMeterTrax_ExceptionReasonTextStr = "EXCEPTIONREASONTEXT";
        public const string sqlMeterTrax_ExceptionCorrectedStr = "EXCEPTIONCORRECTED";
        public const string sqlMeterTrax_TransactionTypeStr = "TRANSACTIONTYPE";
        public const string sqlMeterTrax_FieldRepairOfficerIDStr = "FIELDREPAIROFFICERID";
        public const string sqlMeterTrax_FieldRepairAuditDateStr = "FIELDREPAIRAUDITDATE";
        public const string sqlMeterTrax_FieldRepairAuditTimeStr = "FIELDREPAIRAUDITTIME";

        public const string sqlMeterTrax_CoinPrefixStr = "COIN";
        public const string sqlMeterTrax_CashKeyCardPrefixStr = "CASHKEYCARD";
        public const string sqlMeterTrax_TimeUnitSuffixStr = "_TIMEUNIT";
        public const string sqlMeterTrax_ValueUnitSuffixStr = "_VALUEUNIT";
        public const string sqlMeterTrax_CountSuffixStr = "_COUNT";

        public const string sqlMeterTrax_CashKeyCard01TimeUnitStr = sqlMeterTrax_CashKeyCardPrefixStr + "01" + sqlMeterTrax_TimeUnitSuffixStr;

        public const string sqlMeterTrax_Coin01TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "01" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin02TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "02" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin03TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "03" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin04TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "04" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin05TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "05" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin06TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "06" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin07TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "07" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin08TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "08" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin09TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "09" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin10TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "10" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin11TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "11" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin12TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "12" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin13TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "13" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin14TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "14" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin15TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "15" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin16TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "16" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin17TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "17" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin18TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "18" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin19TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "19" + sqlMeterTrax_TimeUnitSuffixStr;
        public const string sqlMeterTrax_Coin20TimeUnitStr = sqlMeterTrax_CoinPrefixStr + "20" + sqlMeterTrax_TimeUnitSuffixStr;

        public const string sqlMeterTrax_CashKeyCard01ValueUnitStr = sqlMeterTrax_CashKeyCardPrefixStr + "01" + sqlMeterTrax_ValueUnitSuffixStr;

        public const string sqlMeterTrax_Coin01ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "01" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin02ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "02" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin03ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "03" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin04ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "04" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin05ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "05" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin06ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "06" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin07ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "07" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin08ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "08" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin09ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "09" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin10ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "10" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin11ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "11" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin12ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "12" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin13ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "13" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin14ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "14" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin15ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "15" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin16ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "16" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin17ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "17" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin18ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "18" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin19ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "19" + sqlMeterTrax_ValueUnitSuffixStr;
        public const string sqlMeterTrax_Coin20ValueUnitStr = sqlMeterTrax_CoinPrefixStr + "20" + sqlMeterTrax_ValueUnitSuffixStr;

        public const string sqlMeterTrax_Coin01CountStr = sqlMeterTrax_CoinPrefixStr + "01" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin02CountStr = sqlMeterTrax_CoinPrefixStr + "02" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin03CountStr = sqlMeterTrax_CoinPrefixStr + "03" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin04CountStr = sqlMeterTrax_CoinPrefixStr + "04" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin05CountStr = sqlMeterTrax_CoinPrefixStr + "05" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin06CountStr = sqlMeterTrax_CoinPrefixStr + "06" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin07CountStr = sqlMeterTrax_CoinPrefixStr + "07" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin08CountStr = sqlMeterTrax_CoinPrefixStr + "08" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin09CountStr = sqlMeterTrax_CoinPrefixStr + "09" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin10CountStr = sqlMeterTrax_CoinPrefixStr + "10" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin11CountStr = sqlMeterTrax_CoinPrefixStr + "11" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin12CountStr = sqlMeterTrax_CoinPrefixStr + "12" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin13CountStr = sqlMeterTrax_CoinPrefixStr + "13" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin14CountStr = sqlMeterTrax_CoinPrefixStr + "14" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin15CountStr = sqlMeterTrax_CoinPrefixStr + "15" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin16CountStr = sqlMeterTrax_CoinPrefixStr + "16" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin17CountStr = sqlMeterTrax_CoinPrefixStr + "17" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin18CountStr = sqlMeterTrax_CoinPrefixStr + "18" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin19CountStr = sqlMeterTrax_CoinPrefixStr + "19" + sqlMeterTrax_CountSuffixStr;
        public const string sqlMeterTrax_Coin20CountStr = sqlMeterTrax_CoinPrefixStr + "20" + sqlMeterTrax_CountSuffixStr;

        public const string sqlMeterTrax_CoinValueScaleStr = "COIN_VALUESCALE";
        public const string sqlMeterTrax_CoinsRejectedStr = "COINS_REJECTED";

        public const string sqlMeterTrax_CashKeyCardTransactionCountStr = "CASHKEYCARD_TRANS_COUNT";

        public const string sqlMeterTrax_ExternalDeviceTransactionCountStr = "EXT_DEVICE_TRANS_COUNT";
        public const string sqlMeterTrax_ExternalDeviceTransactionAmountStr = "EXT_DEVICE_TRANS_AMOUNT";



        // this is a special date/time combo column defined in the metertrax tables to guarantee uniqueness
        public const string sqlMeterTrax_TransactionDateTimeStr = "TRANSACTIONDATETIME";

        // associates mechanism OR transaction with a physical location
        public const string sqlMeterTrax_PoleLocationForeignKey = "POLELOCATION_FK";

        // associates transacion with mechanism
        public const string sqlMeterTrax_MechanismForeignKey = "MECHANISM_FK";

        // flag for records that were created by the system (vs a user)
        public const string sqlMeterTrax_SystemGeneratedStr = "SYSTEMGENERATED";

        public const string sqlMeterTrax_InstallationDateStr = "INSTALLATIONDATE";
        public const string sqlMeterTrax_MechTypeIDStr = "MECHTYPEID";
        public const string sqlMeterTrax_RateProgramIDStr = "RATEPROGRAMID";
        public const string sqlMeterTrax_DoorLockIDStr = "DOORLOCKID";
        public const string sqlMeterTrax_MechLockIDStr = "MECHLOCKID";
        public const string sqlMeterTrax_RevenueCategoryIDStr = "REVENUECATEGORYID";
        public const string sqlMeterTrax_ActivatedStr = "ACTIVATED";
        public const string sqlMeterTrax_OperableStr = "OPERABLE";

        // added to indicate when transactions are processed retroactively, after inventory has already
        // been updated and transaction is not against current inventory state
        public const string sqlMeterTrax_ProcessedOutofOrderStr = "PROCESSED_OUTOFORDER";



        public const string sqlMeterTrax_RepairCodeIDStr = "REPAIRCODE";
        public const string sqlMeterTrax_RepairCodeDescriptionStr = "REPAIRDESC";
        public const string sqlMeterTrax_OutageCodeIDStr = "OUTAGECODE";
        public const string sqlMeterTrax_OutageCodeDescriptionStr = "OUTAGEDESC";
        public const string sqlMeterTrax_OutageFixedStr = "OUTAGEFIXED";
        public const string sqlMeterTrax_OutagePriorityStr = "OUTAGEPRIORITY";
        public const string sqlMeterTrax_MechReplacedStr = "MECHREPLACED";
        public const string sqlMeterTrax_MechSerialNewStr = "MECHSERIALNUMBERNEW";

        public const string sqlMeterTrax_RateProgramSuccessStr = "RATEPROGRAMSUCCESS";


        public const string sqlMeterTrax_CollectionRouteIDStr = "COLLECTIONROUTEID";
        public const string sqlMeterTrax_CollectionRouteSequenceStr = "COLLECTIONROUTESEQ";
        public const string sqlMeterTrax_MaintenanceRouteIDStr = "MAINTENANCEROUTEID";
        public const string sqlMeterTrax_MaintenanceRouteSequenceStr = "MAINTENANCEROUTESEQ";

        public const string sqlMeterTrax_LastAuditDateStr = "LASTAUDITDATE";
        public const string sqlMeterTrax_LastCollectionDateStr = "LASTCOLLECTIONDATE";
        public const string sqlMeterTrax_LastOpCheckDateStr = "LASTOPCHECKDATE";
        public const string sqlMeterTrax_LastOutageDateStr = "LASTOUTAGEDATE";
        public const string sqlMeterTrax_LastRepairDateStr = "LASTREPAIRDATE";
        public const string sqlMeterTrax_LastProgramDateStr = "LASTPROGRAMDATE";
        public const string sqlMeterTrax_LastInventoryDateStr = "LASTINVENTORYDATE";

        public const string sqlMeterTrax_RevenueCurrentCashStr = "REVENUECURRENTCASH";
        public const string sqlMeterTrax_RevenuePeriodToDateCashStr = "REVENUEPTDCASH";
        public const string sqlMeterTrax_RevenueCurrentDebitStr = "REVENUECURRENTDEBIT";
        public const string sqlMeterTrax_RevenuePeriodToDateDebitStr = "REVENUEPTDDEBIT";

        public const string sqlMeterTrax_OccupancyPercentCurrentStr = "OCCUPANCYPERCENTCURRENT";
        public const string sqlMeterTrax_OccupancyPercentPeriodToDateStr = "OCCUPANCYPERCENTPTD";

        public const string sqlMeterTrax_AverageAuditCashStr = "AVERAGEAUDITCASH";
        public const string sqlMeterTrax_AverageAuditDebitStr = "AVERAGEAUDITDEBIT";

        public const string sqlMeterTrax_RevenueAverageStr = "REVENUEAVERAGE";
        public const string sqlMeterTrax_RevenueMaxWeeklyStr = "REVENUEMAXWEEKLY";

        public const string sqlMeterTrax_ActiveInventoryStr = "ACTIVEINVENTORY";

        //Meter Trax EagleProgram fields
        public const string sqlMeterTrax_RateProgramReferenceNumberStr = "REFERENCENUMBER";
        public const string sqlMeterTrax_RateProgramFileNameStr = "PROGRAMFILENAME";
        public const string sqlMeterTrax_RateProgramFileDateStr = "PROGRAMFILEDATE";
        public const string sqlMeterTrax_RateProgramFileDataStr = "PROGRAMFILEDATA";
        public const string sqlMeterTrax_RateProgramDescStr = "PROGRAMDESC";


        // additional info for tracking manufacturing problems
        public const string sqlMeterTrax_MechPCBBoardNumberStr = "PCB_BOARDNUMBER";
        public const string sqlMeterTrax_MechPCBAssemblyRevStr = "PCB_ASSEMBLYREV";
        public const string sqlMeterTrax_MechPCBManufVendorIDStr = "PCB_VENDORID";
        public const string sqlMeterTrax_MechPCBManufWeekStr = "PCB_MANUFACTUREWEEK";
        public const string sqlMeterTrax_MechPCBManufYearStr = "PCB_MANUFACTUREYEAR";
        public const string sqlMeterTrax_MechPCBSoftwareRevStr = "PCB_SOFTWAREREV";
        public const string sqlMeterTrax_MechPCBPaperClipValStr = "PCB_PAPERCLIPVAL";


        // for the favorites menu
        public const string cnMeterTrax_ReportAuditSummaryName = "Collection Route Revenue Report";

        public const string cnMeterTrax_FieldTransactionTempTableName = "MTX_TRANSTMP";
        public const string cnMeterTrax_PoleLocationInventoryTableName = "MTX_LOCATION";
        public const string cnMeterTrax_MeterMechanismInventoryTableName = "MTX_MECHINFO";
        public const string cnMeterTrax_MeterInstallationHistoryTableName = "MTX_LOCHIST";
        public const string cnMeterTrax_EagleProgramTableName = "MTX_EAGLEPROGRAM";

        // english names for autotrax objects
        public const string cnMeterTrax_FieldTransactionTempDisplayName = "Transaction Exceptions";
        public const string cnMeterTrax_PoleLocationInventoryDisplayName = "Meter Locations";
        public const string cnMeterTrax_MeterMechanismInventoryDisplayName = "Meter Mechanisms";
        public const string cnMeterTrax_MeterInstallationHistoryDisplayName = "Installed Mechanism History";
        public const string cnMeterTrax_EagleProgramDisplayName = "Meter Rate Programs";
        public const string cnMeterTrax_AuditTransactionsDisplayName = "Audit Transactions";
        public const string cnMeterTrax_OpCheckTransactionsDisplayName = "OpCheck Transactions";
        public const string cnMeterTrax_InventoryTransactionsDisplayName = "Inventory Transactions";
        public const string cnMeterTrax_OutageTransactionsDisplayName = "Outage Transactions";
        public const string cnMeterTrax_RateProgramTransactionsDisplayName = "Rate Program Transactions";
        public const string cnMeterTrax_RepairTransactionsDisplayName = "Repair Transactions";


        // transaction tables MUST end with the TRANS suffix - code depends on identifying them by this suffix
        public const string cnMeterTrax_TransactionTableNameSuffix = "TRANS";

        public const string cnMeterTrax_AuditTransactionsTableName = "MTX_AUDTRANS";
        public const string cnMeterTrax_OpCheckTransactionsTableName = "MTX_OPCTRANS";
        public const string cnMeterTrax_InventoryTransactionsTableName = "MTX_INVTRANS";
        public const string cnMeterTrax_OutageTransactionsTableName = "MTX_OUTTRANS";
        public const string cnMeterTrax_RateProgramTransactionsTableName = "MTX_PRGTRANS";
        public const string cnMeterTrax_RepairTransactionsTableName = "MTX_REPTRANS";

        // not a physical table, but dataset-only one created during a mech inventory
        // inquiry in mixed AutoISSUE/AutoTRAX systems
        public const string cnMeterTrax_VirtualParkCiteDetailTableName = "MTX_PRKTRANS";

        // another dataset-only one created during a inqury - contains the mechanism installation
        // history extracted from the physical tables and formatted for display
        public const string cnMeterTrax_VirtualLocationHistoryDetailTableName = "MTX_HSTTRANS";

        // metertrax static list defns
        public const string cnMeterTrax_OutageCodeListName = "MTX_OUTAGE_CODES";
        public const string cnMeterTrax_OutageCodeColumnNameAbbrev = "OUTAGECODE";
        public const string cnMeterTrax_OutageCodeColumnNameDesc = "OUTAGECODE_TXT";
        public const string cnMeterTrax_OutageCodeColumnMechRelated = "OUTAGECODE_MECHRELATED";

        public const string cnMeterTrax_StreetDescListName = "MTX_STREET_DESCRIPTIONS";
        public const string cnMeterTrax_StreetDescColumnNameDesc = "STREETDESC";

        public const string cnMeterTrax_RepairCodeListName = "MTX_REPAIR_CODES";
        public const string cnMeterTrax_RepairCodeColumnNameAbbrev = "REPAIRCODE";
        public const string cnMeterTrax_RepairCodeColumnNameDesc = "REPAIRCODE_TXT";

        public const string cnMeterTrax_CollectionRouteListName = "MTX_COLLECTION_ROUTES";
        public const string cnMeterTrax_CollectionRouteColumnNameAbbrev = "COLLECTIONROUTE";
        public const string cnMeterTrax_CollectionRouteColumnNameDesc = "COLLECTIONROUTE_TXT";

        public const string cnMeterTrax_MaintenanceRouteListName = "MTX_MAINTENANCE_ROUTES";
        public const string cnMeterTrax_MaintenanceRouteColumnNameAbbrev = "MAINTENANCEROUTE";
        public const string cnMeterTrax_MaintenanceRouteColumnNameDesc = "MAINTENANCEROUTE_TXT";

        public const string cnMeterTrax_ParkingZoneListName = "MTX_PARKING_ZONES";
        public const string cnMeterTrax_ParkingZoneColumnNameAbbrev = "PARKINGZONE";
        public const string cnMeterTrax_ParkingZoneColumnNameDesc = "PARKINGZONE_TXT";
        public const string cnMeterTrax_ParkingZoneColumnNameMaxWeeklyRevenue = "PARKINGZONE_MAXWEEKLY_REVENUE";

        public const string cnMeterTrax_SpecialEventTagListName = "MTX_SPECIAL_EVENT_TAGS";
        public const string cnMeterTrax_SpecialEventTagColumnNameAbbrev = "SPECIALEVENTTAG";
        public const string cnMeterTrax_SpecialEventTagColumnNameDesc = "SPECIALEVENTTAG_TXT";


        public const string cnMeterTrax_MeterMechTypeListName = "MTX_METER_MECH_TYPES";
        public const string cnMeterTrax_MeterMechTypeColumnNameAbbrev = "MECHTYPE";
        public const string cnMeterTrax_MeterMechTypeColumnNameDesc = "MECHTYPE_TXT";
        public const string cnMeterTrax_MeterMechTypeColumnNameEagle = "MECHTYPE_EAGLE";
        public const string cnMeterTrax_MeterMechTypeColumnNameMeterResponse = "MECHTYPE_METERRESPONSE";
        public const string cnMeterTrax_MeterMechTypeColumnNameManufacturer = "MECHTYPE_MANUFACTURER";


        public const string cnMeterTrax_VehicleStateListName = "MTX_VEHICLE_STATES";
        public const string cnMeterTrax_VehicleStateColumnNameAbbrev = "STATE";
        public const string cnMeterTrax_VehicleStateColumnNameDesc = "STATE_TXT";

        public const string cnMeterTrax_VehiclePlateTypeListName = "MTX_VEHICLE_PLATETYPES";
        public const string cnMeterTrax_VehiclePlateTypeColumnNameAbbrev = "PLATETYPE";
        public const string cnMeterTrax_VehiclePlateTypeColumnNameDesc = "PLATETYPE_TXT";







        // When working with Latitude/Longitude values, we'll store them as Decimal(9,6).  
        // This allows for 3 digits before the decimal point and 6 digits 
        // after the decimal point.  With 6 digits after the decimal point, 
        // this allows us locate an item within +/- .08 of a meter, or within 
        // about 4 inches - provided that the gov't allows that kind of precision ;-)

        // TODO: Milwaukee's LAT/LONG data has as many as 8 digitd after the decimal point, 
        // what will be the impact if we change the precision after the db is already constructed?
        //public const double cnLatitudeOrLongitudeColumnPrecision = 11.8;


        public const double cnLatitudeOrLongitudeColumnPrecision = 9.6;
        public const string sqlLatitudeDegreesStr = "LOCLATITUDE";
        public const string sqlLongitudeDegreesStr = "LOCLONGITUDE";

        public const string sqlLOGStartLatitudeFieldName = "START_LOCLATITUDE";
        public const string sqlLOGStartLongituteFieldName = "START_LOCLONGITUDE";
        public const string sqlLOGEndLatitudeFieldName = "END_LOCLATITUDE";
        public const string sqlLOGEndLongitudeFieldName = "END_LOCLONGITUDE";


        //Range Manager 
        public const string tblNameActiveUsersStr = "NET_ACTIVEUSERS";
        public const string tblNameAllocationStr = "ALLOCATION";
        public const string tblNameSequenceStr = "SEQUENCE";
        public const string tblNameUnitSerialNo = "UNITSERIALNO";
        public const string sqlSequenceNameStr = "SEQUENCENAME";
        public const string sqlAgencyStr = "AGENCY";
        public const string sqlAllocationNoStr = "ALLOCATIONNO";
        public const string sqlAllocLoStr = "ALLOCLO";
        public const string sqlAllocHiStr = "ALLOCHI";
        public const string sqlCreatedDateStr = "CREATEDDATE";
        public const string sqlPostedDateStr = "POSTEDDATE";
        public const string sqlPrefixStr = "PREFIX";
        public const string sqlSuffixStr = "SUFFIX";
        public const string sqlBookNumberStr = "BOOKNUMBER";
        public const string sqlRngLoStr = "RNGLO";
        public const string sqlRngHiStr = "RNGHI";
        public const string sqlRngLastStr = "RNGLAST";
        public const string sqlUnitSerialStr = "UNITSERIAL";
        public const string sqlLastUnloadDateStr = "LASTUNLOADDATE";
        public const string sqlStatusStr = "STATUS";
        public const string sqlPlatformStr = "PLATFORM";
        public const string sqlCommMediumStr = "COMMMEDIUM";
        public const string sqlStatusTxtStr = "STATUS_TXT";
        public const string sqlCfgInDeviceStr = "CFG_IN_DEVICE";
        public const string sqlPDACommunicationProtocolStr = "PDACOMMUNICATIONPROTOCOL";
        public const string sqlPDAModelStr = "PDAMODEL";
        public const string sqlCfgAssignedStr = "ASSIGNED_CFG";
        public const string sqlSubConfigurationKeyStr = "SUB_CONFIGURATION_KEY";
        public const string sqlDeletedDateStr = "DELETEDDATE";
        public const string sqlAssignedSubCfgKeyStr = "ASSIGNED_SUB_CFG_KEY";
        public const string sqlSubCfgInDeviceKeyStr = "SUB_CFG_IN_DEVICE_KEY";
        public const string sqlCfgAutoTRAXAssignedStr = "ASSIGNED_METERTRAX_CFG";
        public const string sqlCfgAutoTRAXInDeviceStr = "METERTRAX_CFG_IN_DEVICE";
        public const string sqlSerialNumStatus_UnusedStr = "UNUSED";
        public const string sqlSerialNumStatusInUseStr = "IN USE";
        public const string sqlSerialNumStatus_PENDING_DEACTIVATIONStr = "PENDING DEACTIVATION";
        public const string sqlSerialNumStatus_DEACTIVATEDStr = "DEACTIVATED";

        // some specific parameters assigned to units in range manager by serial number
        public const string sqlUnitAPN = "PRIVATE_APN_NAME";         // just because an APN is defined doesn't mean APN name / pwd are required
        public const string sqlUnitAPNLoginName = "LOGIN_N";
        public const string sqlUnitAPNLoginPwd = "LOGIN_P";
        public const string sqlUnitMeterProviderOverride = "MTR_PROVIDER_OVRRD";

        public const string sqlUnitParkeonUserName = "PARKEONUSERNAME";
        public const string sqlUnitParkeonUserPass = "PARKEONPASSWORD";

        public const string sqlUnitParkNOWUserName = "PARKNOWUSERNAME";
        public const string sqlUnitParkNOWUserPass = "PARKNOWPASSWORD";

        public const string sqlUnitDPTStallInfoToken = "DPTSTALLTOKEN";



        //Comm Log
        public const string tblNameCommLogStr = "COMMLOG";
        public const string sqlObjectStr = "OBJECT";
        public const string sqlCommLogDetailStr = "DETAIL";
        public const string sqlCommDateStr = "COMMDATE";
        public const string sqlObjCountStr = "OBJCOUNT";

        //Active Users
        public const string sqlUserSessionKeyStr = "USER_SESSION_KEY";
        public const string sqlUserNameStr = "USERNAME";
        public const string sqlLoggedInStr = "LOGGEDIN";
        public const string sqlLastAccessStr = "LASTACCESS";
        public const string sqlLoggedOutStr = "LOGGEDOUT";

        //Lock Resource
        public const string tblNameNet_LockResource = "NET_LOCKRESOURCE";
        public const string tblNameNet_LockTable = "NET_LOCKTABLE";
        public const string sqlLockHolderStr = "LOCKHOLDER";
        public const string sqlLockDateStr = "LOCKDATE";
        public const string sqlResourceNameStr = "RESOURCENAME";
        public const string sqlLockTypeStr = "LOCKTYPE";

        //Configuration
        public const string tblNameAI_ConfigurationStr = "AI_CONFIGURATION";
        public const string tblNameAI_Computers = "AI_COMPUTERS";
        public const string sqlScopeStr = "SCOPE";
        public const string sqlComputerNameStr = "COMPUTERNAME";
        public const string sqlComputerDisplayNameStr = "COMPUTERDISPLAYNAME";
        public const string sqlItemCategoryStr = "ITEMCATEGORY";
        public const string sqlItemStr = "ITEM";
        public const string sqlItemValueStr = "ITEMVALUE";
        public const string sqlItemDataTypeStr = "ITEMDATATYPE";
        public const string cnfg_CityNameOnTitleBarStr = "cnfg_ClientNameOnTitleBar";   // the title bar value - for display only, can be changed as desired
        public const string cnfg_CityNameStr = "cnfg_CityName";  // this is the value from issue_ap.xml, can't be changed 
        public const string cnfg_SystemFileRootStr = "cnfg_SystemFileRoot";
        public const string cnfg_AlwaysScanUntilEmptyStr = "cnfg_AlwaysScanUntilEmpty";
        public const string cnfg_ComPortStr = "cnfg_ComPort";
        public const string cnfg_PortsToScanStr = "cnfg_PortsToScanStr";
        public const string cnfg_StartChannelStr = "cnfg_StartChannel";
        public const string cnfg_MaxAgeOfWarnings = "cnfg_MaxAgeOfWarnings";
        public const string cnfg_WarningsDelta = "cnfg_WarningsDelta";
        public const string cnfg_GlobalFileCachePolicy = "cnfg_GlobalFileCachePolicy";
        public const string cnfg_LocalFileCacheEnabled = "cnfg_LocalFileCacheEnabled";
        public const string cnfg_SystemMainType = "cnfg_SystemMainType";
        public const string cnfg_AcwPath = "cnfg_AcwPath";
        public const string cnfg_AcwSystemBackupDrive = "cnfg_AcwSystemBackupDrive";
        public const string cnfg_NA = "NA";
        public const string cnfg_HubDisableDuration = "cnfg_HubDisableDuration";
        public const string cnfg_HubEnableDuration = "cnfg_HubEnableDuration";
        public const string cnfg_USBMaxDeviceConnections = "cnfg_USBMaxDeviceConnections";
        public const string cnfg_DisableUSBReset = "cnfg_DisableUSBReset";
        public const string cnfg_CommResetStatusDelta = "cnfg_CommResetStatusDelta";
        public const string cnfg_AllowManualDeviceSyncFromScheduler = "cnfg_AllowManualDeviceSyncFromScheduler";
        public const string cnfg_AllowVoidReinstateAfterExport = "cnfg_AllowVoidReinstateAfterExport";
        // File Maintenance
        public const string cnfg_FileMaintenanceCacheCompositeDays = "cnfg_FileMaintenanceCacheCompositeDays";
        public const string cnfg_FileMaintenanceCacheHandheldDays = "cnfg_FileMaintenanceCacheHandheldDays";
        public const string cnfg_FileMaintenanceCacheReportDays = "cnfg_FileMaintenanceCacheReportDays";
        public const string cnfg_FileMaintenanceCacheImportDays = "cnfg_FileMaintenanceCacheImportDays";
        public const string cnfg_FileMaintenanceCacheExportDays = "cnfg_FileMaintenanceCacheExportDays";
        public const string cnfg_FileMaintenanceCacheListDays = "cnfg_FileMaintenanceCacheListDays";
        public const string cnfg_FileMaintenanceCacheCommLogDays = "cnfg_FileMaintenanceCacheCommLogDays";
        public const string cnfg_FileMaintenanceServiceCommManagerDays = "cnfg_FileMaintenanceServiceCommManagerDays";
        public const string cnfg_FileMaintenanceServiceReportManagerDays = "cnfg_FileMaintenanceServiceReportManagerDays";
        public const string cnfg_FileMaintenanceAutomaticCleanup = "cnfg_FileMaintenanceAutomaticCleanup";
        // Network Access 
        public const string cnfg_NetworkAccessDomain1 = "cnfg_NetworkAccessDomain1";
        public const string cnfg_NetworkAccessUserName1 = "cnfg_NetworkAccessUserName1";
        public const string cnfg_NetworkAccessSalt1 = "cnfg_NetworkAccessSalt1";
        public const string cnfg_NetworkAccessSaltedHash1 = "cnfg_NetworkAccessSaltedHash1";
        public const string cnfg_NetworkAccessHashBrown1 = "cnfg_NetworkAccessHashBrown1";
        //Max tickets in allocation range
        public const string cnfg_MaxTicketsInAllocationRange = "cnfg_MaxTicketsInAllocationRange";
        //Total number of allocated numbers required
        public const string cnfg_AllocatedNumbersRequired = "cnfg_AllocatedNumbersRequired";
        // Threshold when low allocation warning is displayed
        public const string cnfg_AllocationLowThreshold = "cnfg_AllocationLowThreshold";
        //duration that a session can stay inactive
        public const string cnfg_User_Session_Inactivity_TimeOut_Duration = "cnfg_User_Session_Inactivity_TimeOut_Duration";
        // force X3 reclaim on every synchronization
        public const string cnfg_ForceX3ReclaimEverySynchronization = "cnfg_ForceX3ReclaimEverySynchronization";
        public const string cnfg_ResetHHBeforeEndSession = "cnfg_ResetHHBeforeEndSession";


        // Report to Xls 
        public const string cnfg_CreateExcelReport = "cnfg_CreateExcelReport";

        //AutoTrax configuration 
        public const string cnfg_AutoTraxCumulativeAudits = "cnfg_AutoTraxCumulativeAudits";
        public const string cnfg_AutoTraxDownloadLocationDB = "cnfg_AutoTraxDownloadLocationDB";
        public const string cnfg_AutoTraxTrackDebits = "cnfg_AutoTraxTrackDebits";
        public const string cnfg_AutoTraxMaxCash = "cnfg_AutoTraxMaxCash";
        public const string cnfg_AutoTraxMaxCredit = "cnfg_AutoTraxMaxCredit";
        public const string cnfg_AutoTraxCityCode = "cnfg_AutoTraxCityCode";
        public const string cnfg_AutoTraxBattLev = "cnfg_AutoTraxBattLev";
        public const string cnfg_AutoTraxVehicleTracking = "cnfg_AutoTraxVehicleTracking";
        public const string cnfg_AutoTraxVehicleTrackingReportOnly = "cnfg_AutoTraxVehicleTrackingReportOnly";
        public const string cnfg_AutoTraxDefaultState = "cnfg_AutoTraxDefaultState";
        public const string cnfg_AutoTraxDefaultPlateType = "cnfg_AutoTraxDefaultPlateType";
        public const string cnfg_AutoTraxEagleToolboxPath = "cnfg_AutoTraxEagleToolboxPath";
        public const string cnfg_AutoTraxDSTEnabled = "cnfg_AutoTraxDSTEnabled";
        public const string cnfg_AutoTraxSpringDate = "cnfg_AutoTraxSpringDate";
        public const string cnfg_AutoTraxSpringTime = "cnfg_AutoTraxSpringTime";
        public const string cnfg_AutoTraxFallDate = "cnfg_AutoTraxFallDate";
        public const string cnfg_AutoTraxFallTime = "cnfg_AutoTraxFallTime";
        public const string cnfg_AutoTraxAudRouteSelect = "cnfg_AutoTraxAudRouteSelect";
        public const string cnfg_AutoTraxAudTimeSelect = "cnfg_AutoTraxAudTimeSelect";
        public const string cnfg_AutoTraxAudDisplayAudits = "cnfg_AutoTraxAudDisplayAudits";
        public const string cnfg_AutoTraxAudErrorCode = "cnfg_AutoTraxAudErrorCode";
        public const string cnfg_AutoTraxAudBattLevel = "cnfg_AutoTraxAudBattLevel";
        public const string cnfg_AutoTraxAudMaxTime = "cnfg_AutoTraxAudMaxTime";
        public const string cnfg_AutoTraxAudSetClock = "cnfg_AutoTraxAudSetClock";
        public const string cnfg_AutoTraxRepRouteSelect = "cnfg_AutoTraxRepRouteSelect";
        public const string cnfg_AutoTraxRepOutageLists = "cnfg_AutoTraxRepOutageLists";
        public const string cnfg_AutoTraxRepSetClock = "cnfg_AutoTraxRepSetClock";
        public const string cnfg_AutoTraxOpSetClock = "cnfg_AutoTraxOpSetClock";
        public const string cnfg_AutoTraxInvMaintRoutePrompt = "cnfg_AutoTraxInvMaintRoutePrompt";
        public const string cnfg_AutoTraxInvMaintRouteSeqPrompt = "cnfg_AutoTraxInvMaintRouteSeqPrompt";
        public const string cnfg_AutoTraxInvCollRoutePrompt = "cnfg_AutoTraxInvCollRoutePrompt";
        public const string cnfg_AutoTraxInvCollRouteSeqPrompt = "cnfg_AutoTraxInvCollRouteSeqPrompt";
        public const string cnfg_AutoTraxInvDoorLockIDPrompt = "cnfg_AutoTraxInvDoorLockIDPrompt";
        public const string cnfg_AutoTraxInvMechLockIDPrompt = "cnfg_AutoTraxInvMechLockIDPrompt";
        public const string cnfg_AutoTraxInvSetClock = "cnfg_AutoTraxInvSetClock";
        public const string cnfg_AutoTraxInvEnableProgram = "cnfg_AutoTraxInvEnableProgram";
        public const string cnfg_AutoTraxRateSetClock = "cnfg_AutoTraxRateSetClock";
        public const string cnfg_AutoTraxOutRouteSelect = "cnfg_AutoTraxOutRouteSelect";
        public const string cnfg_AutoTraxInvResetAudit = "cnfg_AutoTraxInvResetAudit";
        public const string cnfg_AutoTraxAudMechLogging = "cnfg_AutoTraxAudMechLogging";
        public const string cnfg_AutoTraxInventoryUpdateTimeStamp = "cnfg_AutoTraxInventoryUpdateTimeStamp";
        public const string cnAutoTraxInventoryUpdateTimeStampMask = "MM/dd/yyyy HH:mm";
        public const string cnAutoTraxInventoryUpdateTimeStampDefault = "01/01/2000 01:00";

        public const string cnfg_AutoTraxRTCEnabled = "cnfg_AutoTraxRTCEnabled";
        public const string cnfg_AutoTraxRTCAdjustMinutes = "cnfg_AutoTraxRTCAdjustMinutes";
        public const string cnfg_AutoTraxRTCAdjustSeconds = "cnfg_AutoTraxRTCAdjustSeconds";

        public const string cnfg_AutoTraxClockTestEnabled = "cnfg_AutoTraxClockTestEnabled";
        public const string cnfg_AutoTraxClockTestDate = "cnfg_AutoTraxClockTestDate";
        public const string cnfg_AutoTraxClockTestHour = "cnfg_AutoTraxClockTestHour";
        public const string cnfg_AutoTraxClockTestMinute = "cnfg_AutoTraxClockTestMinute";




        // Scheduler configuration.
        public const string cnfg_SchedulesToExclude = "cnfg_SchedulesToExclude";
        public const string cnfg_ScheduleHideExcluded = "cnfg_ScheduleHideExcluded";
        public const string cnfg_ScheduleLastRunDate = "cnfg_ScheduleLastRunDate";
        public const string cnfg_ScheduleNumberHistoryDays = "cnfg_ScheduleNumberHistoryDays";

        // warnings tracking
        public const string cnfg_WarningsTrackingMaximumTicketAge = "cnfg_WarningsTrackingMaximumTicketAge";


        public const string cnfg_CrystalReportsExecutablePath = "cnfg_CrystalReportsExecutablePath";

        // activate sample map point mappings - for demo purposes only
        public const string cnfg_MapPointSampleMapping = "cnfg_MapPointSampleMapping";

        // Oakland GIS Demo website URL
        public const string cnfg_GISWebsiteDemoURL = "cnfg_GISWebsiteDemoURL";


        // Duncan (PAM) meter interface
        public const string cnfg_DuncanPAMServerBaseURL = "cnfg_DuncanPAMServerBaseURL";
        public const string cnfg_DuncanPAMUserName = "cnfg_DuncanPAMUserName"; // Deprecated / Future Use -- no credentials are currently used for PAM access
        public const string cnfg_DuncanPAMUserPwd = "cnfg_DuncanPAMUserPwd"; // Deprecated / Future Use -- no credentials are currently used for PAM access
        public const string cnfg_DuncanPAMEnforcementGracePeriodMinutes = "cnfg_DuncanPAMEnforcementGracePeriodMinutes";
        public const string cnfg_DuncanPAMEnforcementFilterToSpaceRange = "cnfg_DuncanPAMEnforcementFilterToSpaceRange";

        public const string cnfg_DuncanPamUdpServer = "cnfg_DuncanPamUdpServer";
        public const string cnfg_DuncanPamUdpPort = "cnfg_DuncanPamUdpPort";
        public const string cnfg_DuncanLibertyDataBusServer = "cnfg_DuncanLibertyDataBusServer";
        public const string cnfg_DuncanLibertyDataBusPort = "cnfg_DuncanLibertyDataBusPort";
        public const string cnfg_DuncanProviderInterface = "cnfg_DuncanProviderInterface"; // "HTTP_PAM", "UDP_PAM", "LIBERTY_DATABUS"
        public const string cnfg_DuncanCustomerID = "cnfg_DuncanCustomerID";

        // DigitalPayTechnologies meter interface
        public const string cnfg_DPTServerBaseURL = "cnfg_DPTServerBaseURL";
        public const string cnfg_DPTUserName = "cnfg_DPTUserName";
        public const string cnfg_DPTUserPwd = "cnfg_DPTUserPwd";
        public const string cnfg_DPTStallInfoToken = "cnfg_DPTStallInfoToken";
        public const string cnfg_DPTPayStationInfoToken = "cnfg_DPTPayStationInfoToken";
        public const string cnfg_DPTTransactionInfoToken = "cnfg_DPTTransactionInfoToken";
        public const string cnfg_DPTEnforcementGracePeriodMinutes = "cnfg_DPTEnforcementGracePeriodMinutes";

        // Parkeon meter interface
        public const string cnfg_ParkeonPBSServiceURL = "cnfg_ParkeonPBSServiceURL";
        public const string cnfg_ParkeonPBSServiceUser = "cnfg_ParkeonPBSServiceUser";
        public const string cnfg_ParkeonPBSServicePassword = "cnfg_ParkeonPBSServicePassword";
        public const string cnfg_ParkeonEnforcementGracePeriodMinutes = "cnfg_ParkeonEnforcementGracePeriodMinutes";

        public const string cnfg_ParkeonSecondsToDelayAfterStartSession = "cnfg_ParkeonSecondsToDelayAfterStartSession";
        public const string cnfg_ParkeonRetryAtServerWhenNonCommMeters = "cnfg_ParkeonRetryAtServerWhenNonCommMeters";


        // ParkNOW meter interface
        public const string cnfg_ParkNOWPBSServiceURL = "cnfg_ParkNOWPBSServiceURL";
        public const string cnfg_ParkNOWPBSServiceUser = "cnfg_ParkNOWPBSServiceUser";
        public const string cnfg_ParkNOWPBSServicePassword = "cnfg_ParkNOWPBSServicePassword";
        public const string cnfg_ParkNOWEnforcementGracePeriodMinutes = "cnfg_ParkNOWEnforcementGracePeriodMinutes";

        // Meter Enforcement Model
        public const string cnfg_MeterEnforceModel = "cnfg_MeterEnforceModel"; // "Legacy" or "Multi-vendor"
        public const string cnfg_MultiVendor_IncludeDuncan = "cnfg_MultiVendor_IncludeDuncan";
        public const string cnfg_MultiVendor_IncludeDPT = "cnfg_MultiVendor_IncludeDPT";
        public const string cnfg_MultiVendor_IncludeParkeon = "cnfg_MultiVendor_IncludeParkeon";
        public const string cnfg_MultiVendor_IncludeParkNow = "cnfg_MultiVendor_IncludeParkNow";
        public const string cnfg_MultiVendorPlate_IncludePango = "cnfg_MultiVendorPlate_IncludePango";
        public const string cnfg_MultiVendorPlate_IncludeVerrus = "cnfg_MultiVendorPlate_IncludeVerrus";

        public const string cnfg_CustomerTimezoneDisplay = "cnfg_CustomerTimezoneDisplay";
        public const string cnfg_ACDIServerName = "cnfg_ACDIServerName";

        // Pay-by-Phone interface
        public const string cnfg_PayByPhone_Vendor = "cnfg_PayByPhone_Vendor";
        public const string cnfg_PayByPhone_DatabusURL = "cnfg_PayByPhone_DatabusURL";
        public const string cnfg_PayByPhone_CustomerID = "cnfg_PayByPhone_CustomerID";
        public const string cnfg_PayByPhone_GracePeriodMinutes = "cnfg_PayByPhone_GracePeriodMinutes";

        // AutoFIELD (Duncan/WennSoft) meter work order interface
        public const string cnfg_AutoFIELDServiceURL = "cnfg_AutoFIELDServiceURL";
        public const string cnfg_AutoFIELDCustomerName = "cnfg_AutoFIELDCustomerName";
        public const string cnfg_AutoFIELDCustomerID = "cnfg_AutoFIELDCustomerID";

        // AutoPROCESS webservice interface
        public const string cnfg_AutoPROCESSServiceURL = "cnfg_AutoPROCESSServiceURL";
        public const string cnfg_AutoPROCESSServiceUser = "cnfg_AutoPROCESSServiceUser";
        public const string cnfg_AutoPROCESSServicePassword = "cnfg_AutoPROCESSServicePassword";
        public const string cnfg_AutoPROCESSServiceURL_Secondary = "cnfg_AutoPROCESSServiceURL_Secondary";
        public const string cnfg_AutoPROCESSServiceUser_Secondary = "cnfg_AutoPROCESSServiceUser_Secondary";
        public const string cnfg_AutoPROCESSServicePassword_Secondary = "cnfg_AutoPROCESSServicePassword_Secondary";
        public const string cnfg_AutoPROCESSAgencyDesignator = "cnfg_AutoPROCESSAgencyDesignator";
        public const string cnfg_AutoPROCESSStructuresForUpload = "cnfg_AutoPROCESSStructuresForUpload";
        public const string cnfg_AutoPROCESSStructuresForHotSheet = "cnfg_AutoPROCESSStructuresForHotSheet";

        // Wireless-Only Hotsheet
        public const string cnfg_WirelessOnlyHotSheetsCSV = "cnfg_WirelessOnlyHotSheetsCSV";

        //Export
        public const string tblNameExportHistory = "EXPORTHISTORY";
        public const string sqlExportNumberStr = "EXPORTNUMBER";
        public const string sqlExportStructureStr = "EXPORTSTRUCTURE";
        public const string sqlExportDateStr = "EXPORTDATE";
        public const string sqlExportTypeStr = "EXPORTTYPE";
        public const string sqlExportRecordCountStr = "RECORDCOUNT";

        // NSW Status
        public const string tblNameNSWStatus = "NSWSTATUS";

        //Event Log 
        public const string tblNameEventLogStr = "EVENTLOG";


        // Composite File Info (used so know what current file info is for all files
        // that have made up a composite file - for example, need to know the date
        // and size of the latest AGENCY.DAT file, and if this ever changes, then 
        // know we gotta make a new composite file for it).
        public const string tblNameCompositeFileInfo = "COMPOSITEFILEINFO";
        public const string sqlTableNameStr = "TABLENAME";
        public const string sqlFileNameStr = "FILENAME";
        public const string sqlFileDateStr = "FILEDATE";
        public const string sqlFileSizeStr = "FILESIZE";

        /// <summary>
        /// This column will be added to all list tables that have popular items. It
        /// will not appear in any database, just the DataTable of list. This allows
        /// user to indicate if each list item is a popular list item.
        /// </summary>
        public const string sqlPopularItemStr = "PopularItem";
        public const string cnPopularItemDesc = "Is Popular Item";
        public const int MaxPopularListItems = 5;

        // Task Group
        public const string tblNameTaskGroup = "TASKGROUP";
        public const string sqlGroupNameStr = "GROUPNAME";
        public const string sqlGroupDescriptionStr = "GROUPDESCRIPTION";

        // Task
        public const string tblNameTask = "TASK";
        public const string sqlTaskNameStr = "TASKNAME";
        public const string sqlTaskOrderStr = "TASKORDER";
        public const string sqlTaskToPerformStr = "TASKTOPERFORM";
        public const string sqlTaskGroupKeyStr = "TASKGROUPKEY";
        public const string sqlReportNameStr = "REPORTNAME";
        public const string sqlReportStructureStr = "REPORTSTRUCTURE";
        public const string sqlReportGroupStr = "REPORTGROUP";
        public const string sqlReportParamIDStr = "REPORTPARAMETERID";
        public const string sqlExportDateTypeStr = "EXPORTDATETYPE";
        public const string sqlExportFromDateStr = "EXPORTFROMDATE";
        public const string sqlExportToDateStr = "EXPORTTODATE";
        public const string sqlExportFromDateDeltaStr = "EXPORTFROMDATEDELTA";
        public const string sqlExportToDateDeltaStr = "EXPORTTODATEDELTA";
        public const string sqlExportIncludeTypeStr = "EXPORTINCLUDETYPE";
        public const string sqlExportWarningTypeStr = "EXPORTWARNINGTYPE";
        public const string sqlExportVoidTypeStr = "EXPORTVOIDTYPE";
        public const string sqlExportDateFieldNameStr = "EXPORTDATEFIELDNAME";
        public const string sqlExportOverwriteType = "EXPORTOVERWRITETYPE";
        public const string sqlFTPSessionStr = "FTPSESSION";
        public const string sqlExecutableName = "EXECUTABLENAME";
        public const string sqlExecutableParams = "EXECUTABLEPARAMS";

        // Task Report Parameters
        public const string tblNameTaskReportParameter = "TASKREPORTPARAMETER";
        public const string sqlParameterColumnStr = "PARAMETERCOLUMN";
        public const string sqlParameterOperationStr = "PARAMETEROPERATION";
        public const string sqlParameterDateValueStr = "PARAMETERDATEVALUE";
        public const string sqlParameterStringValueStr = "PARAMETERSTRINGVALUE";
        public const string sqlParameterCategoryStr = "PARAMETERCATEGORY";
        public const string sqlParameterFrmTextStr = "PARAMETERFRMTEXT";

        // Schedule (for scheduling tasks or task groups).
        public const string tblNameSchedule = "SCHEDULE";
        public const string sqlScheduleNameStr = "SCHEDULENAME";
        public const string sqlTaskTypeStr = "TASKTYPE";
        public const string sqlTaskKeyStr = "TASKKEY";
        public const string sqlTimeIntervalStr = "TIMEINTERVAL";
        public const string sqlTimeIntervalStepStr = "TIMEINTERVALSTEP";
        public const string sqlLastRunDateStr = "LASTRUNDATE";
        public const string sqlLastRunStatusStr = "LASTRUNSTATUS";
        public const string sqlLastRunByStr = "LASTRUNBY";
        public const string sqlDayTypeStr = "DAYTYPE";
        public const string sqlDayOfWeekStr = "DAYOFWEEK";
        public const string sqlDayOfWeekListStr = "DAYOFWEEKLIST";
        public const string sqlMonthDayStr = "MONTHDAY";
        public const string sqlMonthDayNumberStr = "MONTHDAYNUMBER";
        public const string sqlMonthWeekStr = "MONTHWEEK";
        public const string sqlAllowMultipleRunsStr = "ALLOWMULTIPLERUNS";
        public const string sqlMinutesBeforeAutoAbortStr = "MINUTESBEFOREAUTOABORT";

        // Schedule log table.
        public const string tblNameScheduleLog = "SCHEDULELOG";
        public const string sqlScheduleKeyStr = "SCHEDULEKEY";
        public const string sqlLogDateStr = "LOGDATE";
        public const string sqlScheduleStatusStr = "SCHEDULESTATUS";
        public const string sqlScheduleDetailsStr = "SCHEDULEDETAILS";
        public const string sqlScheduleRunByStr = "SCHEDULERUNBY";

        // This table is used to store chunks of data that will be
        // converted from AutoIssue to AI.NET.
        public const string tblNameConvertData = "CONVERTDATA";
        public const string sqlMainTableStr = "MAINTABLE";
        public const string sqlLowKeyStr = "LOWKEY";
        public const string sqlHighKeyStr = "HIGHKEY";
        public const string sqlLowDateStr = "LOWDATE";
        public const string sqlHighDateStr = "HIGHDATE";
        public const string sqlRunDateStr = "RUNDATE";
        public const string sqlConvertStatusStr = "CONVERTSTATUS";
        public const string sqlRunByStr = "RUNBY";
        public const string sqlConvertErrorStr = "CONVERTERROR";
        // This table used in AutoIssue to AI.NET conversion so can XRef 
        // AutoIssue export definition number with new AI.NET export
        // definition number (may not be same between 2).
        public const string tblNameConvertExport = "CONVERTEXPORT";
        public const string sqlAutoIssueExportNumber = "AUTOISSUEEXPORTNUMBER";
        // This table is used when converting range allocations from AutoIssue to AI.NET.
        public const string tblNameConvertRange = "CONVERTRANGE";
        public const string sqlRangeAgeStr = "RANGEAGE";


        // This table will store all the sub-configurations. It will only be used in 
        // Miami-Dade type clients that have a master and sub-agencies.
        public const string tblNameSubConfiguration = "SUBCONFIGURATION";
        public const string sqlSubConfigNameStr = "SUBCONFIGNAME";
        public const string sqlSubConfigCodeStr = "SUBCONFIGCODE";
        // This table holds the lists each sub-config will use.
        public const string tblNameSubConfigurationList = "SUBCONFIGURATIONLIST";
        public const string sqlSubConfigKeyStr = "SUBCONFIGKEY";
        public const string sqlSubConfigListNameStr = "LISTNAME";

        // General reports
        public const string sqlGroupValidCount = "GROUPVALIDCOUNT";
        public const string sqlGroupVoidCount = "GROUPVOIDCOUNT";
        public const string sqlGroupVioSum = "GROUPVIOSUM";
        public const string sqlGroup2ValidCount = "GROUP2VALIDCOUNT";
        public const string sqlGroup2VoidCount = "GROUP2VOIDCOUNT";
        public const string sqlGroup2VioSum = "GROUP2VIOSUM";
        public const string sqlGroup1Expr = "GROUP1EXPR";
        public const string sqlGroup2Expr = "GROUP2EXPR";
        public const string sqlSquadStr = "SQUADNO";
        public const string tblNameParking = "PARKING";

        // generated client configuration names for the Symbol and X3 handhelds
        public const string cnAutoTRAXClientConfigSymbol = "SY_AUTOTRAX";
        public const string cnAutoTRAXClientConfigX3 = "X3_AUTOTRAX";


        ///////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// printing/paper values - used for citation print out
        /// </summary>
        /// 
        public const string cnPaperClassSuffixTwoInch = "2IN";
        public const string cnPaperClassSuffixThreeInch = "3IN";
        public const string cnPaperClassSuffixFourInch = "4IN";

        // these dot widths/heights are based on ACTUAL DOTS IN PRINT AREA, not DPI
        public const int LTP3445_PaperAvailDotWd = 832;   /* LTP3445 is 832 Dots wide */
        public const int LTP3445_PaperAvailDotHt = 1080;   /* LTP3445 is 1080 Dots high */
        //public const int LTP3445_PaperAvailDotHt = 1250;   /* Corinth is 1250 - must match print picture configured for patrol car*/
        public const int LTP3345_PaperAvailDotWd = 576;   // ~3 in paper - LTP3345 is 576 dots wide
        public const int LTP3345_PaperAvailDotHt = 1248;  // ~3 in paper

        // 2 inch paper on Intermec PB2 or Pidion 1300
        public const int LTP_2IN_PaperAvailDotWd = 384;   // ~2 in paper - 384 dots wide
        public const int LTP_2IN_PaperAvailDotHt = 1248;  // ~2 in paper is rolled, but pre-printed area is this


        // these widths/heights are based on DPI for scaling, *not* available print area
        // the conversion from MM to IN doesn't usually result  whole numbers
        public const double LTP3445_PaperInchWd = 4.09448819;  // 3445 width = 832 dots = 104 millimeters = 4.09448819 inches
        public const double LTP3445_PaperInchHt = 6.14173228;  // ~4in paper
        public const double LTP3345_PaperInchWd = 2.83464567;  // 3345 width = 576 dots = 72 millimeters = 2.83464567 inches
        public const double LTP3345_PaperInchHt = 6.14173228;  // ~3in paper
        // 2 inch paper
        public const double LTP_2IN_PaperInchWd = 1.88976;  // 384 dots  = 48 mm = 1.88976 inches
        public const double LTP_2IN_PaperInchHt = 6.14173228;  // 2in paper


        // these widths/heights are based on physical paper size, *not* available print area
        public const double LTP3445_PhysicalPaperInchWd = 4.375;  // ~4in paper
        public const double LTP3445_PhysicalPaperInchHt = 6.375;
        public const double LTP3345_PhysicalPaperInchWd = 3.15625;  // ~3in paper 3 5/32"
        public const double LTP3345_PhysicalPaperInchHt = 6.14173228;
        // 2 inch paper
        public const double LTP_2IN_PhysicalPaperInchWd = 2.24409;  // ~2in paper 57 mm
        public const double LTP_2IN_PhysicalPaperInchHt = 6.14173228;


        // horiz and vertial DPI are both 8 dots per mm = 203.2 dpi
        public const double LTP3445_DPIHorz = 203.2;
        public const double LTP3445_DPIVert = 203.2;
        public const double LTP3345_DPIHorz = 203.2;
        public const double LTP3345_DPIVert = 203.2;
        // 2 inch 
        public const double LTP_2IN_DPIHorz = 203.2;
        public const double LTP_2IN_DPIVert = 203.2;


        // As X3 specific features are added in newer OS builds, these comparisons to determine when they can be utilized
        public const string MinOEMVersion_AdditionalACFlashVolumes = "4.60";
        public const string MinOEMVersion_ChangeFolderPathSupport = "4.60";

        // current 64MB X3 only has 1 extra flash volume, but should the 1GB NAND flash get worked out, we could have many more
        public const int MAX_ACFLASH_VOLUMES = 33;


        ///////////////////////////////////////////////////////////////////////////////////////////////



        public const string cnWIFIStr = "WIFI";
        public const string cnBlueToothStr = "BlueTooth";
        public const string cnActiveSyncStr = "ActiveSync";
        public const string CommCmdStr = "CommCmd2";

        //public const string cnPDA5550Str    = "iPaq 5550"; // iPaq 5550    // iPAQs are not supported in AI.NET, they were for AI Delphi
        //public const string cnPDA6365Str    = "iPaq 6365"; // iPaq 6365
        //public const string cnPDA5455Str    = "iPaq 5455"; // iPaq 5455 (BlueTooth & WiFi 8011)
        //public const string cnPDA3970Str    = "iPaq 3970"; // iPaq 3970 (BlueTooth)
        //public const string cnPDA3850Str    = "iPaq 3850"; // iPaq 3850 (IrDA)
        public const string cnPDASym8146PPC2002MonoStr = "Symbol 8146 PPC2002 Mono";  // Symbol 8146 PPC2002 Mono
        public const string cnPDASym8146PPC2002ColorStr = "Symbol 8146 PPC2002 Color";//Symbol 8146 PPC2002 Color
        public const string cnPDASym8146PPC2003MonoStr = "Symbol 8146 PPC2003 Mono";//Symbol 8146 PPC2003 Mono
        public const string cnPDASym8146PPC2003ColorStr = "Symbol 8146 PPC2003 Color";//Symbol 8146 PPC2003 Color

        public const string cnPDASymbolMC9090Str = "Symbol MC-9090 RFID";
        public const string cnPDASymbolMC35Str = "Symbol MC-35";
        public const string cnPDASymbolMC75Str = "Symbol MC-75";
        public const string cnPDAIntermecCN3Str = "Intermec CN3";
        public const string cnPDAIntermecCN50Str = "Intermec CN50";
        public const string cnPDAIntermecCK71Str = "Intermec CK71";
        public const string cnPDAPidion1300Str = "Pidion 1300";
        public const string cnPDAMotorolaMC9500Str = "Motorola MC-9500";
        public const string cnPDACasioIT9000Str = "Casio IT-9000";



        public const string AutoCITE_Hub_VID_PID = "VID_04CC&PID_1521";    // Hub
        public const string AutoCITE_X3_VID_PID = "VID_045E&PID_00CE";    // X3
        public const string Intermec_CN3_VID_PID = "VID_067E&PID_1001";   // Intermec CN3


        // the following PDAs have synchronization support through CommCmdIP, initiated by the client. These USB IDs are unlikely to ever be used, they are just here as placeholders in case USB drivers are written
        //public  const string Symbol_MC9090_RFID_VID_PID = "VID_05E0&PID_200D"; // Symbol 9090 handheld, but is RNDIS driver on client side, we can't use until we deploy a USB client driver
        public const string Symbol_MC9090_RFID_VID_PID = "VID_0BAD&PID_F00D";    // Symbol 9090 handheld placeholder until USB client driver is deployed
        public const string Symbol_MC35_VID_PID = "VID_0BAD&PID_F00D";    // Symbol MC35 handheld placeholder until USB client driver is deployed
        public const string Symbol_MC75_VID_PID = "VID_0BAD&PID_F00D";    // Symbol MC75 handheld placeholder until USB client driver is deployed
        public const string Intermec_CN50_VID_PID = "VID_0BAD&PID_F00D";    // Placeholder until USB client driver is deployed
        public const string Intermec_CK71_VID_PID = "VID_0BAD&PID_F00D";    // Placeholder until USB client driver is deployed
        public const string Pidion_1300_VID_PID = "VID_0BAD&PID_F00D";    // Placeholder until USB client driver is deployed
        public const string Motorola_MC9500_VID_PID = "VID_0BAD&PID_F00D";    // Motorola MC9500 handheld placeholder until USB client driver is deployed


        // name of the host side file the does the initial setup of the PDA devices handheld via ActiveSync
        public const string cnAutoISSUE_Symbol9090ActiveSyncInitializerApp = "AutoISSUE_Symbol9090_Setup.exe";
        public const string cnAutoISSUE_IntermecCN3ActiveSyncInitializerApp = "AutoISSUE_IntermecCN3_Setup.exe";
        public const string cnAutoISSUE_SymbolMC35ActiveSyncInitializerApp = "AutoISSUE_SymbolMC35_Setup.exe";
        public const string cnAutoISSUE_SymbolMC75ActiveSyncInitializerApp = "AutoISSUE_SymbolMC75_Setup.exe";
        public const string cnAutoISSUE_MotorolaMC9500ActiveSyncInitializerApp = "AutoISSUE_MotorolaMC9500_Setup.exe";
        public const string cnAutoISSUE_IntermecCN50ActiveSyncInitializerApp = "AutoISSUE_IntermecCN50_Setup.exe";
        public const string cnAutoISSUE_IntermecCK71ActiveSyncInitializerApp = "AutoISSUE_IntermecCK71_Setup.exe";
        public const string cnAutoISSUE_Pidion1300ActiveSyncInitializerApp = "AutoISSUE_Pidion1300_Setup.exe";
        public const string cnAutoISSUE_CasioIT9000ActiveSyncInitializerApp = "AutoISSUE_CasioIT9000_Setup.exe";


        // the make composite lists a full 5 minutes before timing out
        public const int cnMakeCompositeListsTimeoutMS = 300000;


        ////////////////////////////////////////////////////////////////////////////////////////////////


        /// we store the dates and times as seperate fields in the database. Since most
        /// database "date" columnn are a composite of date AND time, we store these "known"
        /// values in the unused portion of the column. this allows us to to date or time range comparisons
        public const int cnFixedDateYear = 1899;
        public const int cnFixedDateMonth = 12;
        public const int cnFixedDateDay = 30;
        public const int cnFixedTimeHour = 0;
        public const int cnFixedTimeMinute = 0;
        public const int cnFixedTimeSecond = 0;


        // starndardized LIST SUFFIX names - these are lists that the host side is looking for so
        // certain combo boxes can have pre-populated choices
        // SOMEDAY these lists names and their existence should be enforced by the layout tool
        // the "STANDARD" is the OBJECTNAME + "VOIDREASON", so that the layout tool
        // can ensure that a VOIDREASON list exists for any object that has a VOID capability
        public const string cnVoidReasonsListNameSuffix = "VOIDREASON";
        public const string cnReinstateReasonsListNameSuffix = "REINSTATEREASON";


        // our proprietary field format/conversion routines handle either upper or lower case because
        // we don't mix the date/time fields together. But the .NET formatting routines see M and m
        // as different masks, that is M = MONTH and m = minute because it allows date/time fields to
        // be mixed. As such, the mask for DATEs must be MIXED case to be used in either routine
        //public const string cnDefaultDateDisplayMask = "MM/dd/yyyy";
        //public const string cnDefaultTimeDisplayMask = "hh:mm";


        ////////////////////////////////////////////////////////////////////////////////////////////////
        // Filenames used for persistant data used for Installation and Backup/Restore
        public const string PersistedSecurityTablesFilename = "UsersDataset.xml";
        public const string PersistedConfigTablesFilename = "SysConfig.xml";
        public const string PersistedTaskGroupTablesFilename = "TaskGroupsDataset.xml";
        public const string PersistedSystemFileBackupFilename = "SystemFiles.lib";
        ////////////////////////////////////////////////////////////////////////////////////////////////


        public const string cnApplicationEventLogFolder = "EventLogs";
        public const string cnAutoISSUEHostEventLogFilename = "AutoISSUEHostEventLogFile.log";


        /// <summary>
        /// import definition mask that results in a decimal point getting inserted into the 
        /// imported data before posting to the database. Example, "3591" becomes "35.91"
        /// </summary>
        public const string mskImpliedPennies = "8";

        //Ayman S. Oct/2012: Added support for SDSU project
        public enum TPrintedImageOrder
        {
            piNone = 0,
            piPrintedImage,
            piSelectedImage
        };

        /// <summary>
        /// Date-Centric - Returns a dotNET style mask in return for the passed AutoISSUE style mask
        /// Pass in an empty mask "" to get the culture-specific system default
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static string GetDotNetMaskForAutoISSUEMask_Date(string iAutoISSUEDateMask)
        {
            // is it empty?
            if (iAutoISSUEDateMask.Length == 0)
            {
                // just return a default according to current system settings - should work for CompactFramework as well
                return DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            }

            // rebuild the mask, translating the characters
            StringBuilder loDotNetMask = new StringBuilder(iAutoISSUEDateMask);

            // First, we need to translate month and day of week names and/or abbreviations
            // NOTE: The full-name versions are replaced first, to be compatible with abbreviated versions as well
            loDotNetMask.Replace("WWWW", "dddd"); // day of week, full
            loDotNetMask.Replace("wwww", "dddd");
            loDotNetMask.Replace("WWW", "ddd");  // day of week, abbreviated
            loDotNetMask.Replace("www", "ddd");
            loDotNetMask.Replace("MONTH", "MMMM"); // month name, full
            loDotNetMask.Replace("month", "MMMM");
            loDotNetMask.Replace("MON", "MMM"); // month name, abbreviated
            loDotNetMask.Replace("mon", "MMM");

            // Next, make sure all DAY number designators are lowercase
            loDotNetMask.Replace('D', 'd');
            // Next, make sure all MONTH number designators are uppercase
            loDotNetMask.Replace('m', 'M');
            // Next, make sure all YEAR designators are lowercase
            loDotNetMask.Replace('Y', 'y');

            // return the dot net format mask
            return loDotNetMask.ToString();
        }

        /// <summary>
        /// Time-Centrix - Returns a dotNET style mask in return for the passed AutoISSUE style mask
        /// </summary>
        /// <param name="iAutoISSUETimeMask"></param>
        /// <returns></returns>
        public static string GetDotNetMaskForAutoISSUEMask_Time(string iAutoISSUETimeMask)
        {
            // is it empty?
            if (iAutoISSUETimeMask.Length == 0)
            {
                // return the default accoring to the current system settings
                return DateTimeFormatInfo.CurrentInfo.ShortTimePattern;
            }

            // 1st we need to examine the mask to determine if if is using 12-Hour or 24-Hour format
            bool Is24HourFormat;
            if ((iAutoISSUETimeMask.IndexOf('T') >= 0) || (iAutoISSUETimeMask.IndexOf('t') >= 0))
                Is24HourFormat = false;
            else
                Is24HourFormat = true;

            // rebuild the mask, translating the characters
            StringBuilder loAutoISSUEMask = new StringBuilder(iAutoISSUETimeMask);

            // If using 24-Hour format, all hour designators need to be uppercase, otherwise they
            // should be lowercase for 12-Hour format
            if (Is24HourFormat == true)
                loAutoISSUEMask.Replace('h', 'H');
            else
                loAutoISSUEMask.Replace('H', 'h');

            // All MINUTE designators must be lowercase
            loAutoISSUEMask.Replace('M', 'm');
            // All SECONDS designators must be lowercase
            loAutoISSUEMask.Replace('S', 's');
            // All AM/PM designators must be lowercase
            loAutoISSUEMask.Replace('T', 't');

            // return the dot net format mask
            return loAutoISSUEMask.ToString();
        }


        /// <summary>
        /// Date-Centric - Returns an AutoISSUE style mask in return for the passed dotNet style mask
        /// Pass in an empty mask "" to get the culture-specific system default
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static string GetAutoISSUEMaskForDotNetMask_Date(string iDotNetDateMask)
        {
            // is it empty?
            if (iDotNetDateMask.Length == 0)
            {
                // get the default accoring to the current system settings, then translate it
                iDotNetDateMask = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            }

            // rebuild the mask, translating the characters
            StringBuilder loAutoISSUEMask = new StringBuilder();

            // take the whole thing to start
            loAutoISSUEMask.Append(iDotNetDateMask);

            // look for some specialty combos 
            // NOTE: that the full-name versions are replaced first, 
            //       to be compatible with abbreviated versions as well

            loAutoISSUEMask.Replace("dddd", "WWWW"); // day of week, full
            loAutoISSUEMask.Replace("ddd", "WWW");  // day of week, abbreviated
            loAutoISSUEMask.Replace("MMMM", "MONTH"); // month name, full
            loAutoISSUEMask.Replace("MMM", "MON"); // month name, abbreviated

            // If the dotNet mask has zero-padded days, then convert to AutoISSUE zero-padded days,
            // otherwise, just convert remaining "d" characters to uppercase
            if (loAutoISSUEMask.ToString().IndexOf("dd") >= 0)
                loAutoISSUEMask.Replace("dd", "DD");
            else
                loAutoISSUEMask.Replace("d", "D");

            // Month and year should already be suitable as-is

            // return the dot net format mask
            return loAutoISSUEMask.ToString();
        }

        /// <summary>
        /// Time-Centrix - Returns an AutoISSUE style mask in return for the passed DotNet style mask
        /// Pass in an empty mask "" to get the culture-specific system default
        /// </summary>
        /// <param name="iDotNetTimeMask"></param>
        /// <returns></returns>
        public static string GetAutoISSUEMaskForDotNetMask_Time(string iDotNetTimeMask)
        {
            // is it empty?
            if (iDotNetTimeMask.Length == 0)
            {
                // get the default accoring to the current system settings, then translate it
                iDotNetTimeMask = DateTimeFormatInfo.CurrentInfo.ShortTimePattern;
            }

            // rebuild the mask, translating the characters
            StringBuilder loAutoISSUEMask = new StringBuilder(iDotNetTimeMask);

            // Make all HOUR designators uppercase for AutoISSUE
            loAutoISSUEMask.Replace('h', 'H');
            // Make all MINUTE designators uppercase for AutoISSUE
            loAutoISSUEMask.Replace('m', 'M');
            // Make all SECONDS designators uppercase for AutoISSUE
            loAutoISSUEMask.Replace('s', 'S');
            // Make all AM/PM designators uppercase for AutoISSUE
            loAutoISSUEMask.Replace('t', 'T');

            // return the dot net format mask
            return loAutoISSUEMask.ToString();
        }

        public static string GetUserDefinedExportDateFieldName(int iExportNumber)
        {
            // Each field named something like USERDEF1EXPORTDATE.
            return cnFieldUserDefinedExportDatePrefix + iExportNumber.ToString() + cnFieldUserDefinedExportDateSuffix;
        }

        public static string[] GetAllUserDefinedExportDateFieldNames()
        {
            // Will return one name for export date number (from 1 to cnNumberOfUserDefinedExportDateFields).
            List<string> nameList = new List<string>();
            for (int exportNumber = 1; exportNumber <= cnNumberOfUserDefinedExportDateFields; exportNumber++)
            {
                nameList.Add(GetUserDefinedExportDateFieldName(exportNumber));
            }

            // Return all names in form of an array.
            return nameList.ToArray();
        }

        public static string GetCustomExportDateFieldName(int iExportNumber)
        {
            // Each field named something like CUSTOM1EXPORTDATE.
            return cnFieldCustomExportDatePrefix + iExportNumber.ToString() + cnFieldCustomExportDateSuffix;
        }

        public static string[] GetAllCustomExportDateFieldNames()
        {
            // Will return one name for export date number (from 1 to cnNumberOfCustomExportDateFields).
            List<string> nameList = new List<string>();
            for (int exportNumber = 1; exportNumber <= cnNumberOfCustomExportDateFields; exportNumber++)
            {
                nameList.Add(GetCustomExportDateFieldName(exportNumber));
            }

            // Return all names in form of an array.
            return nameList.ToArray();
        }

        /// <summary>
        /// Return a standardized name for a DataRelation. This centralized method should be called
        /// to create a name whenever a DataRelation is defined to ensure that all DataRelations follow
        /// a standard naming scheme which is repeatable and predictable when attempting to locate a relation
        /// The naming scheme is "ParentTableName_ParentColumnName__ChildTableName_ChildColumnName"
        /// </summary>
        /// <param name="iDataTable"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        /* KLC
        public static string GetStandardizedDataRelationName(DataColumn iParentDataColumn, DataColumn iChildDataColumn)
        {
            try
            {
                return iParentDataColumn.Table.TableName + "_" + iParentDataColumn.ColumnName + "__" + iChildDataColumn.Table.TableName + "_" + iChildDataColumn.ColumnName;
            }
            catch
            {
                return "INVALID COLUMNS PASSED";
            }
        }
        */
        /// <summary>
        /// Will return a serialized string version of object passed to it.
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string SerializeObject(object objectToSerialize)
        {
            // Will return blank string if serialize fails.
            string objectAsString = "";

            try
            {
                // Serialize the object into a string.
                if (objectToSerialize != null)
                {
                    StringWriter writer = new StringWriter();
                    XmlSerializer serializer = new XmlSerializer(objectToSerialize.GetType());
                    serializer.Serialize(writer, objectToSerialize);
                    objectAsString = writer.ToString();
                    writer.Close();
                }
            }
            catch
            {
            }

            // Return serialized string.
            return objectAsString;
        }

        /// <summary>
        /// Will create an object (by deserializing it) from passed string based on passed type.
        /// </summary>
        /// <param name="objectAsString"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static object DeSerializeObject(string objectAsString, Type objectType)
        {
            // This is the object we will return (null if fail).
            object objectToReturn = null;

            try
            {
                // Deserialize the object.
                StringReader reader = new StringReader(objectAsString);
                XmlSerializer serializer = new XmlSerializer(objectType);
                objectToReturn = serializer.Deserialize(reader);
                reader.Close();
            }
            catch
            {
            }

            // Return object we created.
            return objectToReturn;
        }


        // only available on the host side
#if !WindowsCE && !__ANDROID__   
        /// <summary>
        /// Go thru all tables/columns in dataset and set each DateTime column to
        /// "Unspecified". This will tell the XML Serializer that we do NOT want
        /// the time zone (UTC) included with the time part so the serializer will
        /// NOT adjust the time to match the time zone (for example, if server in
        /// New York extracts a DateTime record that has time=6pm and then sends this
        /// DataSet to California, the client in California will convert this 6pm to
        /// 3pm since the DateTime portion contains time zone info and it knows how to
        /// adjust the time based on the time zone).
        /// </summary>
        /// <param name="oDataSet"></param>
        /// 
        public static void ConvertDateTimeColumnsInDataSetToUnspecified(ref DataSet oDataSet)
        {
            foreach (DataTable oneTable in oDataSet.Tables)
            {
                foreach (DataColumn oneColumn in oneTable.Columns)
                {
                    if (oneColumn.DataType == typeof(DateTime))
                    {
                        oneColumn.DateTimeMode = DataSetDateTime.Unspecified;
                    }
                }
            }
        }
#endif



        /// <summary>
        /// Multimedia types we can attach. These values stored as string their representations in NOTES tables
        /// </summary>
        public enum TMultimediaType
        {
            mmNone = 0,
            mmUnknown,
            mmPicture,
            mmWaveAudio,
            mmDiagram
        }


        // only available on the host side
#if !WindowsCE && !__ANDROID__   
        /// <summary>
        /// Attempts to parse the byte stream to determine what kind of multimedia object it is
        /// </summary>
        /// <param name="iMultimediaBuffer"></param>
        /// <returns></returns>
        public static TMultimediaType DetermineMultimediaType(Byte[] iMultimediaBuffer)
        {
            // null is no attachment
            if (iMultimediaBuffer == null)
            {
                return TMultimediaType.mmNone;
            }


            if (iMultimediaBuffer.LongLength > 4)
            {
                // look at first 4 bytes to see if it's probably wave file data 
                StringBuilder loWavHeader = new StringBuilder();
                for (int loIdx = 0; loIdx < 4; loIdx++)
                {
                    loWavHeader.Append((char)iMultimediaBuffer[loIdx]);
                }
                if (loWavHeader.ToString().Equals("RIFF"))
                {
                    return TMultimediaType.mmWaveAudio;
                }


                // for now, if its not a WAV, then its some kind of image
                return TMultimediaType.mmPicture;
            }
            else
            {
                return TMultimediaType.mmUnknown;
            }
        }
#endif


        // only available on the host side
#if !WindowsCE //&& !__ANDROID__   
        /// <summary>
        /// Returns TMultimediaType for passed string - useful for retreiving from the database
        /// </summary>
        /// <param name="iMultimediaType"></param>
        /// <returns></returns>
        public static TMultimediaType GetMultimediaTypeForDisplayName(string iMultimediaDisplayName)
        {

            // null strings means nothing
            if (iMultimediaDisplayName == null)
            {
                return TMultimediaType.mmNone;
            }

            foreach (TMultimediaType loMultimediaType in Enum.GetValues(typeof(TMultimediaType)))
            {
                // this it?
                if (iMultimediaDisplayName.ToUpper().Equals(loMultimediaType.ToString().ToUpper()))
                {
                    return loMultimediaType;
                }
            }

            // or we don't know
            return TMultimediaType.mmUnknown;
        }
#endif

    }

    // (Moved here to a light-weight shared file)
    public enum CiteRangeActionType
    {
        craDeleteUnit,
        craFreezeUnit,
        craThawUnit
    }


}

namespace AutoISSUE.Data
{
    // These are the different type of files that the GetListFiles method can return.
    // (Moved here to a light-weight shared file)
    public enum ListFileType
    {
        DataFile,
        EditFile,
        CompositeFile,
        PopularFile
    }
}
