using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Data;
using Reino.ClientConfig;
using Android.Content;
using Android.Preferences;
using Duncan.AI.Droid.Utils.HelperManagers;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using AutoISSUE;

namespace Duncan.AI.Droid.Utils.BarCode
{

    public class BarCodeCommons
    {
        public const int MAX_BAR_BYTES = 600;
        public const int SECONDS_PER_MINUTE = 60;
        public const int MINUTES_PER_HOUR = 60;
        public const int SECONDS_PER_HOUR = (SECONDS_PER_MINUTE * MINUTES_PER_HOUR);
        public const int HOURS_PER_DAY = 24;
        public const int SECONDS_PER_DAY = (SECONDS_PER_HOUR * HOURS_PER_DAY);
        public const int DAYS_IN_WEEK = 7;
        public const int DAYS_IN_YEAR = 365;
        public const int DAYS_IN_LEAPYEAR = 366;
        public const int LEAPYEAR_FEB_DAYS = 29;


        // const for field names used in the magnetic stripe AND InfoCop stuffing
        // field name to hold the spot when there is no RegOwn equivilent of Suspect field
        public const String RONoFieldDefined = "~x";

        // AAMVA Subfile Types we recognize
        public const String AAMVA_SubfileType_DriversLicID = "DL";
        public const String AAMVA_SubfileType_VehicleTitle = "TD";
        public const String AAMVA_SubfileType_VehicleRegistration = "RG";
        public const String AAMVA_SubfileType_MotorCarrierCabCard = "MC";
        public const String AAMVA_SubfileType_MotorCarrierRegistrant = "IR";
        public const String AAMVA_SubfileType_VehicleSafetyInspection = "VS";
        public const String AAMVA_SubfileType_VehicleOwners = "OW";
        public const String AAMVA_SubfileType_VehicleInfo = "VH";

        public const String AAMVA_JurisdictionSubfileType_NY_Vehicle = "ZV";         // TODO - these jurisdiction-specified subfiles are in the context of the issuing authority      
        public const String AAMVA_JurisdictionSubfileType_NY_Registration = "ZR";	  // ie. NY will use subfile types designations that can conflict with subfile designations from 
        public const String AAMVA_JurisdictionSubfileType_NY_VersionControl = "ZZ";  // other issuing authorities

        // AAMVA required constants that designate the fields
        public const String AAMVA_BarDriverLicenseName = "DAA";
        public const String AAMVA_BarDriverMailingStreet1 = "DAG";
        public const String AAMVA_BarDriverMailingCity = "DAI";
        public const String AAMVA_BarDriverMailingJurisidctionCode = "DAJ";
        public const String AAMVA_BarDriverMailingPostalCode = "DAK";
        public const String AAMVA_BarDriverLicenseNumber = "DAQ";
        public const String AAMVA_BarDriverLicenseClassCode = "DAR";
        public const String AAMVA_BarDriverLicenseRestrictionCode = "DAS";
        public const String AAMVA_BarDriverLicenseEndorsementCode = "DAT";
        public const String AAMVA_BarDriverLicenseExpirationDate = "DBA";
        public const String AAMVA_BarDriverLicenseDOB = "DBB";
        public const String AAMVA_BarDriverLicenseSex = "DBC";
        public const String AAMVA_BarDriverLicenseIssueDate = "DBD";

        /*// AAMVA optional constants that designate the fields*/
        public const String AAMVA_BarDriverLicenseHeight = "DAU";
        public const String AAMVA_BarDriverLicenseWeight = "DAW";
        public const String AAMVA_BarDriverLicenseEyeColor = "DAY";
        public const String AAMVA_BarDriverLicenseHairColor = "DAZ";
        public const String AAMVA_BarDriverLicenseSSN = "DBK";
        //#define AAMVA_BarDriverLicenseLastName "DAB"
        public const String AAMVA_BarDriverLicenseLastName = "DCS";
        public const String AAMVA_BarDriverLicenseFirstName = "DAC";
        public const String AAMVA_BarDriverLicenseMiddleName = "DAD";
        public const String AAMVA_BarDriverLicenseNameSuffix = "DAE";
        public const String AAMVA_BarDriverLicenseNamePrefix = "DAF";
        public const String AAMVA_BarDriverLicenseMailingStreet2 = "DAH";
        public const String AAMVA_BarDriverLicenseResidenceStreet1 = "DAL";
        public const String AAMVA_BarDriverLicenseResidenceStreet2 = "DAM";
        public const String AAMVA_BarDriverLicenseResidenceCity = "DAN";
        public const String AAMVA_BarDriverLicenseResidenceJurisdictionCode = "DAO";
        public const String AAMVA_BarDriverLicenseResidencePostalCode = "DAP";
        public const String AAMVA_BarDriverLicenseHeight_CM = "DAV";
        public const String AAMVA_BarDriverLicenseWeight_KG = "DAX";

        /*
        AAMVA Compliant Header

        Field Bytes (Fixed) 	Contents 	 
        1 	1 	Compliance Indicator: A 2D symbol encoded according to the rules of this standard shall include a Compliance Indicator. The Compliance Indicator as defined by this standard is the Commercial At Sign (“@”) (ASCII/ISO 646 Decimal “64”) (ASCII/ISO 646 Hex “40”). The Compliance Indicator is the first character of the symbol. 	 
        2 	1 	Data Element Separator: The Data Element Separator is used in this standard to indicate that a new data element is to follow, and that the current field is terminated. Whenever a Data Element Separator is encountered (within a Subfile type which uses Data Element Separators), the next character(s) shall either be a Segment Terminator or shall define the contents of the next field according to the template of the specific Subfile. The Data Element Separator as defined by this standard is the Line Feed character (“L F” ASCII/ISO 646 Decimal “10”) (ASCII/ISO 646 Hex “0A”). The Data Element Separator is the second character of the symbol. 	 
        3 	1 	Record Separator: The Record Separator as defined by this standard is the Record Separator character (“R S” ASCII/ISO 646 Decimal “30”) (ASCII/ISO 646 Hex “1E”). As this report is presented for ratification, there is no special case defined for when this field will be used. It is embodied within the recommendation for future growth. The Record Separator is the third character of the symbol and shall always be reflected within the header in a compliant symbol. 	 
        4 	1 	Segment Terminator: As used in this standard the Segment Terminator is used to end Subfiles where Field Identifiers are employed. The Segment Terminator as defined by this standard is the Carriage Return character (“C R ” ASCII/ISO 646 Decimal “13”) (ASCII/ISO 646 Hex “0D”). The Segment Terminator is the fourth character of the symbol. 	 
        5 	5 	File Type: This is the designator that identifies the file as an AAMVA compliant format. The designator is defined as the 5 byte upper character String “ANSI “, with a blank space after the fourth character . 	 
        6 	6 	Issuer Identification Number (IIN): This number uniquely identifies the issuing jurisdiction and can be obtained by contacting the ISO Issuing Authority (AAMVA). The full 6-digit IIN should be encoded. 	 
        7 	2 	AAMVA Version Number: This is a decimal value between 00 and 99 that specifies the version level of the PDF417 bar code format. Version “0” and "00" is reserved for bar codes printed to the specification of the American Association of Motor Vehicle Administrators (AAMVA) prior to the adoption of the AAMVA DL/ID-2000 standard. All bar codes compliant with the AAMVA DL/ID-2000 standard are designated Version “01.” All barcodes compliant with AAMVA specification version 1.0, dated 9-25-2003 shall be designated Version “02.” All barcodes compliant with this current AAMVA specification shall be designated Version “03.” Should a need arise requiring major revision to the format, this field provides the means to accommodate additional revision. 	 
        8 	2 	Jurisdiction Version Number: This is a decimal value between 00 and 99 that specifies the jurisdiction version level of the PDF417 bar code format. Notwithstanding iterations of this specification, jurisdictions implement incremental changes to their bar codes, including new jurisdiction-specific data, compression algorithms for digitized images, digital signatures, or new truncation conventions used for names and addresses. Each change to the bar code format within each AAMVA version (above) must be noted, beginning with Jurisdiction Version 00. 	 
        9 	2 	Number of Entries: This is a decimal value between “01 and 99” that specifies the number of different Subfile types that are contained in the bar code. This value defines the number of individual subfile designators that follow. All subfile designators (as defined below) follow one behind the other. The data related to the first subfile designator follows the last Subfile Designator. 	 
        */

        public const int AAMVA_BarHeader_ComplianceOffset = 0;
        public const int AAMVA_BarHeader_ComplianceSize = 1;

        public const int AAMVA_BarHeader_DataElementSeparatorOffset = 1;
        public const int AAMVA_BarHeader_DataElementSeparatorSize = 1;

        public const int AAMVA_BarHeader_RecordSeparatorOffset = 2;
        public const int AAMVA_BarHeader_RecordSeparatorSize = 1;

        public const int AAMVA_BarHeader_SegmentTerminatorOffset = 3;
        public const int AAMVA_BarHeader_SegmentTerminatorSize = 1;

        public const int AAMVA_BarHeader_FileTypeOffset = 4;
        public const int AAMVA_BarHeader_FileTypeSize = 5;

        public const int AAMVA_BarHeader_IssuerIDNumberOffset = 9;
        public const int AAMVA_BarHeader_IssuerIDNumberSize = 6;

        public const int AAMVA_BarHeader_AAMVAVersionNumberOffset = 15;
        public const int AAMVA_BarHeader_AAMVAVersionNumberSize = 2;

        // only present in ID cards
        public const int AAMVA_BarHeader_JurisdictionVersionNumberOffset = 17;
        public const int AAMVA_BarHeader_JurisdictionVersionNumberSize = 2;


        // the subfile count is in different slot for ID cards versus documents. 
        // The ID cards have the Jurisdiction Ver No which pushes the subfile count over
        public const int AAMVA_BarHeader_IDCard_SubfileCountOffset = 19;
        public const int AAMVA_BarHeader_Document_SubfileCountOffset = 17;
        public const int AAMVA_BarHeader_SubfileCountSize = 2;
        public const int AAMVA_BarHeader_SubFileDesignatorSize = 10;

        /*
        AAMVA 2005 DL/ID - subfile header designator - repeated once for each of the subfiles in the barcode

        1 	2 	Subfile Type: This is the designator that identifies what type of data is contained in this portion of the file. The 2-character uppercase character field “DL” is the designator for DL/ID subfile type containing mandatory and optional data elements as defined in tables D.3 and D.4. Jurisdictions may define a subfile to contain jurisdiction-specific information. These subfiles are designated with the first character of “Z” and the second character is the first letter of the jurisdiction's name. For example, "ZC" would be the designator for a California or Colorado jurisdiction-defined subfile; "ZQ" would be the designator for a Quebec jurisdiction-defined subfile. 	 
        2 	4 	Offset: These bytes contain a 4 digit numeric value that specifies the number of bytes from the head or beginning of the file to where the data related to the particular sub-file is located. The first byte in the file is located at offset 0. 	 
        3 	4 	Length: These bytes contain a 4 digit numeric value that specifies the length of the Subfile in bytes. The segment terminator must be included in calculating the length of the subfile. A segment terminator = 1. Each subfile must begin with the two-character Subfile Type and these two characters must also be included in the length. 	 
        */

        // offsets into each subfile header
        public const int AAMVA_BarHeader_SubFileTypeOffset = 0;
        public const int AAMVA_BarHeader_SubFileTypeSize = 2;

        public const int AAMVA_BarHeader_SubFileOffsetOffset = 2;
        public const int AAMVA_BarHeader_SubFileOffsetSize = 4;

        public const int AAMVA_BarHeader_SubFileLengthOffset = 6;
        public const int AAMVA_BarHeader_SubFileLengthSize = 4;

        /*
        AAMVA 2005 DL/ID card spec

        Driver's License (DL Subfile) Data

        AAMVA DL/ID Card Minimum mandatory data elements
        a. 	DCA 	Jurisdiction-specific vehicle class 	Jurisdiction-specific vehicle class / group code, designating the type of vehicle the cardholder has privilege to drive. 	DL 	V4ANS 	 
        b. 	DCB 	Jurisdiction-specific restriction codes 	Jurisdiction-specific codes that represent restrictions to driving privileges (such as airbrakes, automatic transmission, daylight only, etc.). 	DL 	V10ANS 	 
        c. 	DCD 	Jurisdiction-specific endorsement codes 	Jurisdiction-specific codes that represent additional privileges granted to the cardholder beyond the vehicle class (such as transportation of passengers, hazardous materials, operation of motorcycles, etc.). 	DL 	V5ANS 	 
        d. 	DBA 	Document Expiration Date 	Date on which the driving and identification privileges granted by the document are no longer valid. (MMDDCCYY for U.S., CCYYMMDD for Canada) 	Both 	F8N 	 
        e. 	DCS 	Customer Family Name 	Family name of the cardholder. (Family name is sometimes also called “last name” or “surname.”) Collect full name for record, print as many characters as possible on front of DL/ID 	Both 	V40ANS 	 
        f 	DCT 	Customer Given Names 	Given names of the cardholder. (Given names include all names other than the Family Name. This includes all those names sometimes also called “first” and “middle” names.) Collect full name for record, print as many characters as possible on front of DL/ID. 	Both 	V80ANS 	 
						 
        g. 	DBD 	Document Issue Date 	Date on which the document was first issued. (MMDDCCYY for U.S., CCYYMMDD for Canada) 	Both 	F8N 	 
        h. 	DBB 	Date of Birth 	Date on which the cardholder was born. (MMDDCCYY for U.S., CCYYMMDD for Canada) 	Both 	F8N 	 
        i. 	DBC 	Physical Description – Sex 	Gender of the cardholder. 1 = male, 2 = female. 	Both 	F1N 	 
        j. 	DAY 	Physical Description – Eye Color 	Color of cardholder's eyes. (ANSI D-20 codes) 	Both 	F3A 	 
        k. 	DAU 	Physical Description – Height 	Height of cardholder. Inches (in): number of inches followed by " in" ex. 6'1'' = " 73 in" Centimeters (cm): number of centimeters followed by " cm" ex. 181 centimeters="181 cm" 	Both 	F6AN 	 
						 
        l. 	DAG 	Address – Street 1 	Street portion of the cardholder address. 	Both 	V35ANS 	 
        m. 	DAI 	Address – City 	City portion of the cardholder address. 	Both 	V20ANS 	 
        n. 	DAJ 	Address – Jurisdiction Code 	State portion of the cardholder address. 	Both 	F2A 	 
        o. 	DAK 	Address – Postal Code 	Postal code portion of the cardholder address in the U.S. and Canada. If the trailing portion of 	Both 	F11AN 	 
		            Code 	the postal code in the U.S. is not known, zeros will be used to fill the trailing set of numbers. 			 
        p. 	DAQ 	Customer ID Number 	The number assigned or calculated by the issuing authority. 	Both 	V25ANS 	 
        q. 	DCF 	Document Discriminator 	Number must uniquely identify a particular document issued to that customer from others that may have been issued in the past. This number may serve multiple purposes of document discrimination, audit information number, and/or inventory control. 	Both 	V25ANS 	 
        r. 	DCG 	Country Identification 	Country in which DL/ID is issued. U.S. = USA, Canada = CAN. 	Both 	F3A 	 
        s. 	DCH 	Federal Commercial Vehicle Codes 	Federally established codes for vehicle categories, endorsements, and restrictions that are generally applicable to commercial motor vehicles. If the vehicle is not a commercial vehicle, "NONE" is to be entered. 	DL 		 

        D.12.3.2 Optional data elements
        Data 	Elem-	Data Element 	Definiftion 	Card 	Length / 	 
        Ref. 	ent 			type 	type 	 
	    ID 					 
        a. 	DAH 	Address – Street 2 	Second line of street portion of the cardholder 	Both 	V35ANS 	 
			        address. 			 
        b. 	DAZ 	Hair color 	Brown, black, blonde, gray, red/auburn, sandy, 	Both 	V12A 	 
			        white 			 
        c. 	DCI 	Place of birth 	Country and municipality and/or state/province 	Both 	V33A 	 
        d. 	DCJ 	Audit information 	A string of letters and/or numbers that identifies when, where, and by whom a driver license/ID card was made. If audit information is not used 	Both 	V25ANS 	 
			        on the card or the MRT, it must be included in 			 
			        the driver record. 			 
        e. 	DCK 	Inventory control 	A string of letters and/or numbers that is affixed 	Both 	V25ANS 	 
  		            number 	to the raw materials (card stock, laminate, etc.) 			 
			        used in producing driver licenses and ID cards. 			 
        f. 	DBN 	Alias / AKA Family 	Other family name by which cardholder is 	Both 	V10ANS 	 
		            Name 	known. 			 
        g 	DBG 	Alias / AKA Given 	Other given name by which cardholder is 	Both 	V15ANS 	 
		            Name 	known 			 
        h. 	DBS 	Alias / AKA Suffix 	Other suffix by which cardholder is known 	Both 	V5ANS 	 
		            Name 				 
        i. 	DCU 	Name Suffix 	Name Suffix (If jurisdiction participates in systems requiring name suffix (PDPS, CDLIS, 	Both 	V5ANS 	 
			        etc.), the suffix must be collected and displayed 			 
			        on the DL/ID and in the MRT). Collect full 			 
			        name for record, print as many characters as 			 
			        possible on front of DL/ID. 			 
        j. 	DCE 	Physical 	Indicates the approximate weight range of the 	Both 	F1N 	 
		            Description – 	cardholder: 			 
		            Weight Range 				 
			        0 = up to 31 kg (up to 70 lbs) 			 
			        1 = 32 – 45 kg (71 – 100 lbs) 			 
			        2 = 46 - 59 kg (101 – 130 lbs) 			 
			        3 = 60 - 70 kg (131 – 160 lbs) 			 
			        4 = 71 - 86 kg (161 – 190 lbs) 			 
			        5 = 87 - 100 kg (191 – 220 lbs) 	 
			        6 = 101 - 113 kg (221 – 250 lbs) 	 
			        7 = 114 - 127 kg (251 – 280 lbs) 	 
			        8 = 128 – 145 kg (281 – 320 lbs) 	 
			        9 = 146+ kg (321+ lbs) 	 

        k. 	DCL 	Race / ethnicity 	Codes for race or ethnicity of the cardholder, as defined in ANSI D20. 	Both 	F2N 	 
        l. 	DCM 	Standard vehicle classification 	Standard vehicle classification code(s) for cardholder. This data element is a placeholder for future efforts to standardize vehicle classifications. 	DL 	F4AN 	 
        m. 	DCN 	Standard endorsement code 	Standard endorsement code(s) for cardholder. This data element is a placeholder for future efforts to standardize endorsement codes. 	DL 	F5AN 	 
        n. 	DCO 	Standard restriction code 	Standard restriction code(s) for cardholder. This data element is a placeholder for future efforts to standardize restriction codes. 	DL 	F12AN 	 
        o. 	DCP 	Jurisdiction-specific vehicle classification description 	Text that explains the jurisdiction-specific code(s) for types of vehicles cardholder is authorized to drive. 	DL 	V50ANS 	 
        p. 	DCQ 	Jurisdiction-specific endorsement code description 	Text that explains the jurisdiction-specific code(s) that indicates additional driving privileges granted to the cardholder beyond the vehicle class. 	DL 	V50ANS 	 
        q. 	DCR 	Jurisdiction-specific restriction code description 	Text describing the jurisdiction-specific restriction code(s) that curtail driving privileges. 	DL 	V50ANS 	 

        -----------------------------------------------
        AAMVA Bar Code Data Encoding Requirements - AAMVA International Specification - Motor Vehicle Documents - 2006-05-11

        Table D.2 – Subfile designator format
        1 	2 	Subfile Type: This is the designator that identifies what type ofdata is contained in this portion of the file. 
                The 2-character field defined for each standard-compliant MVAdocument type and corresponding subfile is as follows: 

        Title: "TD"
        Registration: "RG"
        Motor carrier cab card: "MC"
        Motor carrier registrant: “IR”
        Vehicle safety inspection: "VS"
        Vehicle owners: “OW”
        Vehicle: “VH”
        2 	4 	Offset: These bytes contain a 4-digit numeric value that specifiesthe number of bytes from the head or beginning of the file to wherethe data related to the particular sub-file is located. The first bytein the file is located at offset 0. 	 
        3 	4 	Length: These bytes contain a 4-digit numeric value that specifiesthe length of the Subfile in bytes. The segment terminator must beincluded in calculating the length of the subfile. The segmentterminator is 1 byte long. 	 


        Vehicle Title (TD Subfile) Data
        Table A.1 – Mandatory PDF417 Data Elements
        a. 	TAC 	Titling jurisdiction 	The code for the jurisdiction (U.S., Canadian, or Mexican) that titled the vehicle. 	F 	2 	AN	 
        b. 	TAA 	Title number 	A unique set of alphanumeric characters assigned by the titling jurisdiction to the certificate of title of a vehicle. 	F 	17 	AN	 
        c. 	TAV 	Title issue date 	The date the jurisdiction’s titling authority issued a title to the owner of the vehicle. The format is CCYYMMDD. 	F 	8 	N	 
        d. 	VAL 	Vehicle model year 	The year that is assigned to a vehicle by the manufacturer. The format is CCYY. 	F 	4 	AN	 
        e. 	VAK 	Vehicle make 	The distinctive (coded) name applied to a group of vehicles by a manufacturer. 	F 	4 	AN	 
        f. 	VAD 	Vehicle identification number (VIN) 	A unique combination of alphanumeric characters that identifies a specific vehicle or component. The VIN is affixed to the vehicle in specific locations and formulated by the manufacturer. State agencies under some controlled instances may assign a VIN to a vehicle. 	F 	17 	AN	 
        g. 	TAF 	Odometer reading—mileage 	This is the odometer reading registered with the DMV either at the time of titling or registration renewal. 	F 	12 	AN	 
        h. 	TAU 	Vehicle purchase date 	The date a vehicle was purchased by the current owner. The format is CCYYMMDD. 	F 	8 	N	 
        i. 	NAA 	Family name 	Family name (commonly called surname or last name) of the owner of the vehicle. 	V 	40 	ANS	 
        j. 	NAE 	Given name 	Given name or names (includes all of what are commonly referred to as first and middle names) of the owner of the vehicle. 	V 	80 	ANS	 
        k. 	NAR 	Address-street 	Street portion of the owner’s address. 	V 	35 	AN 	 
        l. 	NAT 	Address-city 	City portion of the owner’s address. 	V 	20 	AN 	 
        m. 	NAU 	Address-jurisdiction code 	Jurisdiction portion of the owner’s address. 	F 	2 	A	 
        n. 	NAV 	Address-zip code 	The ZIP code or Postal code portion of the owner’s address. 	V 	11 	AN	 
        o. 	TAG 	Odometer disclosure 	This is the federal odometer mileage disclosure. The mandatory information is: (1) Actual vehicle mileage; (2) Mileage exceeds mechanical limitations; (3) Not actual mileage; (4) Mileage disclosure not required. 	F 	1 	AN	 
        NOTE: ANSI D20 contains formatting requirements for names and addresses that must be followed.

        Table A.2 – Optional PDF417 Data Elements
        a. 	TPJ 	Previous titling jurisdiction 	The code for the jurisdiction (U.S., Canadian, or Mexican) that titled the vehicle immediately prior to the current titling jurisdiction. 	F 	2 	AN	 
        b. 	TAZ 	Previous title number 	The title number assigned to the vehicle by the previous titling jurisdiction. 	F 	17 	AN	 
        c. 	TAY 	Title brand 	Code providing information about the brand applied to the title. 	F 	1 	AN	 
        d. 	VAO 	Vehicle body style 	The general configuration or shape of a vehicle distinguished by characteristics such as number of doors, seats, windows, roofline, and type of top. The vehicle body type is 2-character alphanumeric. 	F 	2 	AN	 
        e. 	TAH 	Odometer date 	The date the odometer reading was recorded by the jurisdiction. 	F 	8 	N	 
        f. 	TAW 	New / used indicator 	This code represents whether the vehicle/vessel is new or used. Note: jurisdictions’ definitions of these classifications may vary a little due to state regulations on demo vehicles, slates between dealers, application of state taxes, etc. N = New, U = Used. 	F 	1 	A	 
        g. 	LAA  	First lien holder name 	Name of the first lien holder of the vehicle. 	V 	35 	ANS	 
        h. 	LAF 	First lien holder ID 	A code that uniquely identifies the first holder of a lien. 	F 	16 	AN	 
        i. 	VAM 	Vehicle model 	A code denoting a family of vehicles (within a make), which has a degree of similarity in construction, such as body, chassis, etc. The field does not necessarily contain a standard code; it may contain a value provided by the originator of the field. 	F 	6 	AN	 
        j. 	TAI 	Odometer reading—kilometers 	This is the odometer reading registered with the DMV either at the time of titling or registration renewal in kilometers. 	F 	12 	ANS	 
        k. 	BBC 	Business Name 	The name of business that owns the vehicle. 	V 	35 	AN 	 
        l. 	VBD 	Vehicle color 	Where the vehicle/vessel is one color, this is the appropriate code describing that color. When the vehicle is two colors, this is the code for the top-most or front-most color. 	F 	3 	AN	 
        */
        public const String AAMVA_BarRG_PlateNumber = "RAM";
        public const String AAMVA_BarRG_PlateClass = "RAL";  // referenced in NY doc, not in AAMVA???
        public const String AAMVA_BarRG_ExpiryDate = "RAG";


        /*
        Vehicle Registration (RG Subfile) Data
        Table B.1 – Mandatory PDF417 Data Elements
        a. 	RBB 	Registration issue date 	The date in which the registration was issued. Format is CCYYMMDD. 	F 	8 	N	 
        b. 	RAG 	Registration expiry date 	The date in which the registration expired. Format is CCYYMMDD. 	F 	8 	N	 
        c. 	RAM 	Registration plate number 	The characters assigned to a registration plate or tag affixed to the vehicle, assigned by the jurisdiction. 	F 	9 	ANS	 
        d. 	RBD 	Registrant family name 	Family name (commonly called surname or last name) of the registered owner of a vehicle. 	V 	40 	ANS	 
        e. 	RBE 	Registrant given name 	Given name or names (includes all of what are commonly referred to as first and middle names) of the registered owner of a vehicle. 	V 	80 	ANS	 
        f. 	RBI 	Address-street 	Street portion of the owner’s address. 	V 	35 	AN	 
        g. 	RBK 	Address-city 	City portion of the owner’s address. 	V 	20 	AN 	 
        h. 	RBL 	Address-jurisdiction code 	Jurisdiction portion of the owner’s address. 	F 	2 	A	 
        i. 	RBM 	Address-zip code 	The Zip code or Postal code of the vehicle owner’s residence address. 	V 	11 	AN	 
        j. 	VAD 	Vehicle identification number (VIN) 	A unique combination of alphanumeric characters that identifies a specific vehicle or component. The VIN is affixed to the vehicle in specific locations and formulated by the manufacturer. State agencies under some controlled instances my assign a VIN to a vehicle. 	F 	17 	AN	 
        k. 	VAK 	Vehicle make 	The distinctive (coded) name applied to a group of vehicles by a manufacturer. 	F 	4 	AN	 
        l. 	VAL 	Vehicle model year 	The year which is assigned to a vehicle by the manufacturer. The format is CCYY. 	F 	4 	N	 
        m. 	VAO 	Vehicle body style 	The general configuration or shape of a vehicle distinguished by characteristics such as number of doors, seats, windows, roofline, and type of top. The vehicle body type is 2-character alphanumeric. 	F 	2 	AN	 
        n. 	RBT 	Registration year 	The year of registration. Format is CCYYMMDD. 	F 	8 	N	 
        o. 	RBU 	Registration window sticker decal 	A unique number printed on the tab/decal and stored as part of the registration record. 	F 	20 	N	 


        Table B.2 – Optional PDF417 Data Elements
        a. 	VPC 	Vehicle use  	Indicates the use of the vehicle. 	F 	4 	AN	 
        b. 	FUL 	Fuel 	The type of fuel used by the vehicle. In most cases, the fuel type would be diesel. 	F 	8 	AN	 
        c. 	VBC 	Axles 	The number of common axles of rotation of one or more wheels of a vehicle, 	F 	2 	AN	 
        d. 	VAT 	Gross vehicle weight 	The unladen weight of the vehicle (e.g., the single-unit truck, truck combination) plus the weight of the load being carried at a specific point in time. 	F 	9 	ANS	 
        e. 	VAM 	Vehicle model 	A code denoting a family of vehicles (within a make), which has a degree of similarity in construction, such as body, chassis, etc. The field does not necessarily contain a standard code; it may contain a value provided by the originator of the field. 	F 	6 	AN	 
        f. 	BBC 	Business Name 	The business name of the first registrant of a vehicle. 	V 	35 	AN	 
        g. 	VBD 	Vehicle color 	Where the vehicle is one color, this is the appropriate code describing that color. When the vehicle is two colors, this is the code for the top-most or front-most color. 	F 	3 	AN	 




        Motor Carrier (MC Subfile) Data 
        Table C.1 – Mandatory PDF417 Data Elements

        a. 	MAN 	USDOT number 	A unique identifier assigned to the carrier responsible for safety issued by the U.S. Department of Transportation – Federal Motor Carrier Safety Administration. 	V 	12 	N	 
        b. 	MAA 	Carrier name 	The name of the carrier responsible for safety. This can be an individual, partnership or corporation responsible for the transportation of persons or property. This is the name that is recognized by law. 	V 	35 	AN	 
        c. 	MAK 	Street address 	This is the mailing address of the individual carrier. This information is utilized by the base jurisdiction to send information to the carrier that purchased the IRP credentials. 	V 	35 	AN 	 
        e. 	MAL 	City 	This is the city for the mailing address of the individual carrier. This information is utilized by the base jurisdiction to send information to the carrier that purchased the IRP credentials. 	V 	20 	AN	 
        f. 	MAI 	 Jurisdiction 	This is the jurisdiction of the residential address of the individual carrier. This information is utilized by the base jurisdiction to send information to the carrier that purchased the IRP credentials. 	F 	2 	AN	 
        g. 	MAO 	Zip 	The ZIP or Postal code of the resident address of the vehicle owner. 	V 	11 	N	 
        */



        /*
        Registrant and Vehicle Data (IR Subfile)
        Table C.2 – Mandatory PDF417 Data Elements

        a. 	RBC 	Carrier name-registrant 	The name of the first registrant of a vehicle. Registrant’s name may be a combined individual name or the name of a business 	V 	35 	AN	 
        b. 	RBI 	Address 	The first line of the registrant’s residence address. 	V 	35 	AN	 
        c. 	RBK 	City 	The registrant’s residence city name. 	V 	20 	AN	 
        d. 	RBL 	Jurisdiction 	The state or province of the registrant’s residence address. 	F 	2 	AN	 
        e. 	RBM 	Zip code 	The ZIP or Postal code of the resident address of the registrant. 	V 	11 	AN	 
        f. 	IEG 	Unit number 	A number, assigned by the registrant of the commercial vehicle or trailer, to identify the vehicle or trailer in the fleet. No two units in a fleet can have the same number. A.K.A vehicle unit number or owner’s equipment number. 	V 	9 	AN	 
        g. 	VAD 	Vehicle identification number (VIN) 	A unique combination of alphanumeric characters that identifies a specific vehicle or component. The VIN is affixed to the vehicle in specific locations and formulated by the manufacturer. State agencies under some controlled instances may assign a VIN to a vehicle. 	F 	17 	AN	 
        h. 	VAL 	Model year 	The year which is assigned to a vehicle by the manufacturer. The format is YY. 	F 	2 	AN	 
        i. 	VAK 	Vehicle make 	The distinctive (coded) name applied to a group of vehicles by a manufacturer. 	V 	4 	AN	 
        j. 	VBB 	Type of vehicle 	The type of vehicle operated for the transportation of persons or property in the furtherance of any commercial or industrial enterprise, for hire or not for hire. Not all states will use all values. 	F 	2 	AN	 
        k. 	RAP/VBC 	Number of seats/axles 	The seat capacity of a commercial vehicle designed for transportation of more than then passengers. The number of common axles of rotation of one or more wheels of a vehicle, whether power design or freely rotating. 	V 	2 	N	 
        l. 	RBT 	Registration year 	This field is the registration year assigned by the jurisdiction. The format is CCYY. 	F 	4 	N	 
        m. 	IFJ 	Registration issue date 	The date in which the registration was issued. CCYYMMDD format. 	F 	8 	N	 
        n. 	RAM 	Registration plate number 	The characters assigned to a registration plate or tag affixed to the vehicle, assigned by the jurisdiction. 	V 	9 	N	 
        o. 	RAD 	Registration decal number 	The number assigned to the registration decal in those jurisdictions that issue decals. 	V 	10 	N	 
        p. 	RAF 	Registration enforcement date 	The registration enforcement date is the date that the current registration was enforced. This may or may not be the original registration date. The date format is CCYYMMDD. 	F 	8 	N	 
        q. 	RAG 	Registration expiration date 	The date in which the registration expired. The date format is CCYYMMDD. 	F 	8 	N	 
        r. 	VAT 	Gross vehicle weight 	The unladen weight of the vehicle (e.g., single-unit truck, truck combination) plus the weight of the maximum load for which vehicle registration fees have been paid within a particular jurisdiction. 	V 	9 	AN	 
        s. 	RAU 	Base jurisdiction registered weight 	The declared base jurisdiction registration weight. 	V 	10 	N	 
        */




        /*
        Vehicle Safety Inspection Document (VS Subfile) Data
        Table D.1 – Mandatory PDF417 Data Elements
        a. 	ISN 	Inspection station number 	Station number performing the inspection. 	F 	4 	AN 	 
        b. 	IIN 	Inspector identification number 	A unique number assigned to each licensed vehicle inspector. 	F 	7 	N	 
        c. 	VAK 	Vehicle make 	The distinctive (coded) name applied to a group of vehicles by a manufacturer. 	V 	4 	AN	 
        d. 	VAL 	Vehicle model year 	The year which is assigned to a vehicle by the manufacturer. The format is CCYY. 	F 	4 	N	 
        e. 	VAO 	Vehicle body type 	The general configuration or shape of a vehicle distinguished by characteristics such as number of doors, seats, windows, roofline, and type of top. The vehicle body type is 2-character alphanumeric. 	F   	2   	AN   	 
        f. 	ORI 	Odometer reading at inspection 	The vehicle’s odometer reading (to the nearest mile or kilometer) at the time of inspection. 	F 	12 	ANS	 
        Table D.2 – Optional PDF417 Data Elements
        a. 	IAN 	Inspection address 	The street name and number, city, state and zip code of the inspection facility. 	V 	108 	ANS	 
        b. 	IPD 	Inspection air pollution device conditions 	Identifies whether the pollution control devices meet the minimum inspection criteria. 	F 	2 	A	 
        c. 	IFI 	Inspection facility identifier 	The unique number assigned to an inspection facility. 	F 	5 	AN	 
        d. 	INC 	Inspection form number, current 	A unique number assigned to a current vehicle inspection form for identification purposes. 	F 	10 	N	 
        e. 	INP 	Inspection form number, previous 	The number of the last inspection form excluding the current inspection. 	F 	10 	N	 
        f. 	ISC 	Inspection smog certificate indicator 	An indicator that specifies whether or not the vehicle has a current smog certificate. 	F 	1 	AN	 
        g. 	INC 	Inspection sticker number, current 	Preprinted unique number on the motor vehicle inspection sticker currently issued to a motor vehicle which has passed inspection. 	F 	9 	N	 
        h. 	INP 	Inspection sticker number, previous 	The certification number of the last inspection sticker, excluding the current inspection. 	F 	9 	N	 




        Vehicle Owner (OW Subfile) Data - 
        Table E.1 – Mandatory PDF417 Data Elements

        ?Item# 	Data element ID 	Data Element 	Description 	Field maximum length/type 	 F/V 	Length 	Type	 
        a. 	NAA 	First owner total name 	Name of the (or one of the) individual(s) who owns the Vehicle as defined in the ANSI D-20 Data Element Dictionary. (Lastname@Firstname@MI@Suffix if any.) 	V 	35 	AN	 
        b. 	NAB 	First owner last name 	Last Name or Surname of the Owner. Hyphenated names acceptable, spaces between names acceptable, but no other use of special symbols. 	V 	35 	AN	 
        c. 	NAC 	First owner name 	First Name or Given Name of the Owner. Hyphenated names acceptable, but no other use of special symbols. 	V 	35 	AN	 
        d. 	NAD 	First owner middle name 	Middle Name(s) or Initial(s) of the Owner. Hyphenated names acceptable, spaces, between names acceptable, but no other use of special symbols. 	V 	35 	AN	 
        e. 	NAE 	Second owner total name 	Name of the (or one of the) individual(s) who owns the Vehicle as defined in the ANSI D-20 Data Element Dictionary. (Lastname@Firstname@MI@Suffix if any.) 	V 	35 	AN	 
        f. 	NAF 	Second owner last name 	Last Name or Surname of the Owner. Hyphenated names acceptable, spaces between names acceptable, but no other use of special symbols. 	V 	35 	AN	 
        g. 	NAG 	Second owner name 	First Name or Given Name of the Owner. Hyphenated names acceptable, but no other use of special symbols. 	V 	35 	AN	 
        h. 	NAH 	Second owner middle name 	Middle Name(s) or Initial(s) of the Owner. Hyphenated names acceptable, spaces between names acceptable, but no other use of special symbols. 	V 	35 	AN	 
        i. 	NAR 	Mailing address 1 	Street address line 1. (Mailing) 	V 	35 	AN 	 
        j. 	NAS 	Mailing address 2 	Street address line 2. (Mailing) 	V 	35 	AN 	 
        k. 	NAT 	Mailing city 	Name of city for mailing address. 	V 	15 	AN 	 
        l. 	NAU 	Mailing jurisdiction code 	Jurisdiction code for mailing address. Conforms to Canadian, Mexican and US jurisdictions as appropriate. Codes for provinces (Canada) and states (US and Mexico). 	F 	2 	AN	 
        m. 	NAV 	Mailing zip code 	The ZIP code or Postal code used for mailing. (As used by Canadian, Mexican and US jurisdictions.) 	V 	11 	N	 
        n. 	NAM 	Residence address 1 	Street address line 1. (Mailing) 	V 	35 	AN 	 
        o. 	NAN 	Residence address 2 	Street address line 2. (Mailing) 	V 	35 	AN 	 
        p. 	NAO 	Residence city 	Name of city for mailing address. 	V 	15 	AN 	 
        q. 	NAP 	Residence jurisdiction code 	Jurisdiction code for mailing address. Conforms to Canadian, Mexican and US jurisdictions as appropriate. Codes for provinces (Canada) and states (US and Mexico). 	F 	2 	AN	 
        r. 	NAQ 	Residence zip code 	The ZIP code or Postal code used for mailing. (As used by Canadian, Mexican and US jurisdictions). 	V 	11 	N	 
        s. 	NAX 	First owner ID number 	The unique customer number/ID of the first vehicle owner. 	V 	35 	AN	 
        t. 	NAY 	Second owner ID number 	The unique customer number/ID of the second vehicle owner. 	V 	35 	AN	 
        v. 	NBA 	First owner legal status 	The legal status of the first vehicle owner. This is only used when a vehicle has multiple owners. A legal status may be (“AND”, “OR”). 	F 	3 	AN	 
        w. 	NBB 	Second owner legal status 	The legal status of the second vehicle owner. This is only used when a vehicle has multiple owners. A legal status may be (“AND”, “OR”). 	F 	3 	AN	 
        */



        /*
        Vehicle (VH Subfile) Data - Table F.1 – Mandatory PDF417 Data Elements
        a. 	VAA 	Major code 	State to provide definition. 	F 	1 	AN	 
        b. 	VAB 	Minor code 	State to provide definition. 	F 	1 	AN	 
        c. 	VAC 	Transmission code 	Type of transmission the vehicle carries. 	F 	1 	AN	 
        d. 	VAD 	Vehicle identification number 	A unique combination of alphanumeric characters that identifies a specific vehicle or component. The VIN is affixed to the vehicle in specific locations and formulated by the manufacturer. State agencies under some controlled instances may assign a VIN to a vehicle. 	F 	17 	AN	 
        e. 	VAE 	MSRP/FLP 	Manufacturer’s Suggested Retail Price. No decimal places. Right Justified Zero or space fill. 	F 	6 	N	 
        f. 	VAF 	Junked indicator 	Vehicle has been junked. 	F 	1 	N 	 
        g. 	VAG 	Date junked 	CCYYMMDD; Date vehicle reported junked. 	F 	8 	N 	 
        h. 	VAH 	Stolen indicator 	Indicates stolen vehicle. 	F 	1 	AN	 
        i. 	VAI 	Date stolen 	CCYYMMDD; Date vehicle reported stolen. 	F 	8 	N 	 
        j. 	VAJ 	Date recovered 	CCYYMMDD; Date vehicle reported recovered. 	F 	8 	N	 
        k. 	VAK 	Vehicle make 	The distinctive (coded) name applied to a group of vehicles by a manufacturer. 	F 	4 	AN	 
        l. 	VAL 	Make Year 	Vehicle manufacture year. 	F 	4 	N 	 
        m. 	VAM 	Vehicle model 	Vehicle manufacture model. 	F 	3 	A 	 
        n. 	VAN 	Fuel type 	Type of fuel the vehicle utilizes. 	F 	1 	A 	 
        o. 	VAO 	Body style 	Vehicle manufacture body style. 	F 	2 	AN 	 
        p. 	VAP 	Number of doors 	Number of doors on the vehicle. 	F 	1 	N 	 
        q. 	VAQ 	Number of cylinders 	Number of cylinders the vehicle has. 	F 	3 	N 	 
        r. 	VAR 	Engine size 	The size of a vehicle’s engine. 	F 	3 	AN 	 
        s. 	VAS 	Vehicle status code 	This is the status of the vehicle (e.g., active, suspend, etc.) 	F 	3 	AN 	 
        t. 	VAT 	Manufacture gross weight 	Manufacturer’s gross vehicle weight rating. 	F 	9 	ANS	 
        u. 	VAU 	Horsepower 	Manufacturer’s rated horsepower. 	F 	4 	N 	 
        v. 	VAV 	Unladen weight 	Gross weight of the vehicle unloaded. 	F 	9 	ANS	 
        w. 	VAW 	Engine displacement 	Manufacturer’s rated engine displacement. 	F 	4 	N 	 
        x. 	VAX 	IRP indicator 	International registration plan indicator. 	F 	1 	AN 	 
        y. 	VAY 	IFTA indicator 	International fuel tax indicator 	F 	! 	AN 	 
        z. 	VAZ 	VLT clac from date 	Vehicle license tax calculation from date of purchase. 	F 	10 	N	 
        aa. 	VBA 	Vehicle ID number 	Unique number to identify the vehicle record. 	F 	15 	AN 	 
        bb. 	VBB 	Vehicle type code 	EPA vehicle classification. 	F 	8 	AN 	 
        cc. 	VBC 	Number of Axles 	Number of axles the vehicle has. 	F 	1 	N 	 
        */


        public const String AAMVA_BarVH_VIN = "VAD";
        public const String AAMVA_BarVH_VehicleMake = "VAK";
        public const String AAMVA_BarVH_VehicleModel = "VAM";
        public const String AAMVA_BarVH_VehicleYear = "VAL";
        public const String AAMVA_BarVH_VehicleCylinders = "VAQ";
        public const String AAMVA_BarVH_VehicleGrossWeight = "VAT";
        public const String AAMVA_BarVH_VehicleUnladenWeight = "VAV";



        /*
        Vehicle Safety Inspection Document (VS Subfile) Data - Table D.1 – Mandatory PDF417 Data Elements
        a. 	ISN 	Inspection station number 	Station number performing the inspection. 	F 	4 	AN 	 
        b. 	IIN 	Inspector identification number 	A unique number assigned to each licensed vehicle inspector. 	F 	7 	N	 
        c. 	VAK 	Vehicle make 	The distinctive (coded) name applied to a group of vehicles by a manufacturer. 	V 	4 	AN	 
        d. 	VAL 	Vehicle model year 	The year which is assigned to a vehicle by the manufacturer. The format is CCYY. 	F 	4 	N	 
        e. 	VAO 	Vehicle body type 	The general configuration or shape of a vehicle distinguished by characteristics such as number of doors, seats, windows, roofline, and type of top. The vehicle body type is 2-character alphanumeric. 	F   	2   	AN   	 
        f. 	ORI 	Odometer reading at inspection 	The vehicle’s odometer reading (to the nearest mile or kilometer) at the time of inspection. 	F 	12 	ANS	 

        Table D.2 – Optional PDF417 Data Elements
        a. 	IAN 	Inspection address 	The street name and number, city, state and zip code of the inspection facility. 	V 	108 	ANS	 
        b. 	IPD 	Inspection air pollution device conditions 	Identifies whether the pollution control devices meet the minimum inspection criteria. 	F 	2 	A	 
        c. 	IFI 	Inspection facility identifier 	The unique number assigned to an inspection facility. 	F 	5 	AN	 
        d. 	INC 	Inspection form number, current 	A unique number assigned to a current vehicle inspection form for identification purposes. 	F 	10 	N	 
        e. 	INP 	Inspection form number, previous 	The number of the last inspection form excluding the current inspection. 	F 	10 	N	 
        f. 	ISC 	Inspection smog certificate indicator 	An indicator that specifies whether or not the vehicle has a current smog certificate. 	F 	1 	AN	 
        g. 	INC 	Inspection sticker number, current 	Preprinted unique number on the motor vehicle inspection sticker currently issued to a motor vehicle which has passed inspection. 	F 	9 	N	 
        h. 	INP 	Inspection sticker number, previous 	The certification number of the last inspection sticker, excluding the current inspection. 	F 	9 	N	 
        */



        // New York Subfile Specific Element IDs
        public const String AAMVA_BarVH_NY_FourDigitYear = "ZVA";
        public const String AAMVA_BarVH_NY_VehicleMake = "ZVB";
        public const String AAMVA_BarVH_NY_VehicleBody = "ZVC";
        public const String AAMVA_BarVH_NY_VehicleFuel = "ZVD";
        public const String AAMVA_BarVH_NY_VehicleColor = "ZVE";

        public const String AAMVA_BarRG_NY_NYMAIndicator = "ZRA";
        public const String AAMVA_BarRG_NY_ThreeOfName = "ZRB";

        public const String AAMVA_BarVC_NY_BarcodeVersion = "ZZZ";

        public const int CATrack31FldSep = 0x20;
        public const int CATrack31SubFldSep = 0x20;

        public const int NoFldSep = 0x00;
        
        //const Byte[] CharToVK = new Byte[256]
    	//{
        ////     0    1    2    3    4    5    6    7    8    9    A    B    C    D    E    F
        ///*0*/ 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x08,0x09,0x00,0x00,0x00,0x0D,0x00,0x00,
        ///*1*/ 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x1B,0x00,0x00,0x00,0x00,
        ///*2*/ 0x20,0x31,0xDE,0x33,0x34,0x35,0x37,0xDE,0x39,0x30,0x38,0xBB,0xBC,0xBD,0xBE,0xBF,
        ///*3*/ 0x30,0x31,0x32,0x33,0x34,0x35,0x36,0x37,0x38,0x39,0xBA,0xBA,0xBC,0xBB,0xBE,0xBF,
        ///*4*/ 0x32,0x41,0x42,0x43,0x44,0x45,0x46,0x47,0x48,0x49,0x4A,0x4B,0x4C,0x4D,0x4E,0x4F,
        ///*5*/ 0x50,0x51,0x52,0x53,0x54,0x55,0x56,0x57,0x58,0x59,0x5A,0xDB,0x5C,0xDD,0x36,0xBD,
        ///*6*/ 0x60,0x41,0x42,0x43,0x44,0x45,0x46,0x47,0x48,0x49,0x4A,0x4B,0x4C,0x4D,0x4E,0x4F,
        ///*7*/ 0x50,0x51,0x52,0x53,0x54,0x55,0x56,0x57,0x58,0x59,0x5A,0xDB,0x5C,0xDD,0x60,0x60,
        ///*8*/ 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
        ///*9*/ 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
        ///*A*/ 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
        ///*B*/ 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
        ///*C*/ 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
        ///*D*/ 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
        ///*E*/ 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
        ///*F*/ 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
    	//};
        //#define Track1FldSep 0x0D
        // { + $20 = '^' }{ true AAMVA }
        /*#define AAMVATrack31FldSep 0x3E
                  // { + $30 = '=' }{ true AAMVA }
        #define AAMVATrack2FldSep  0x0D*/
        //  + $20 = '$' }{ true AAMVA }
        //#define AAMVATrack31SubFldSep 0x24
        // { space}   { AAMVA MD style }
        //#define AAMVA_MDTrack31SubFldSep 0x40

        //-----------------------
        // revised & validated -- these should replace the above completely when all cards are tested

        //#define AAMVA_DataElementSeparator  0x0A
        //#define AAMVA_SegmentTerminator  0x0D
        public const int AAMVA_ComplianceIndicator = 0x40;

        public const char AAMVA_FieldDelimeter = (char)0x20;
        public const char AAMVA_SubFieldDelim = (char)0x2C;
        public const char AAMVA_MDSubFieldDelim = (char)0x20;
        public const char CommaSubFldSep = (char)0x2C;   // some oddballs, like ontario, put commas in there

        // this number should be a chunk larger than he number of subfile types above 
        // these are the ones we know how to parse, and we will expect to encounter custom 
        // jurisdiction subfiles which we wont know how to handle. Still we must have room
        // to parse what we know, while we skip over those we dont
        public const int MAX_SUBFILE_TYPES = 32;

        public enum TNameStyle
        {
            name_LFM,
            name_FML
        } ;

        public enum TMonthsOfYear
        {
            January = 0,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December,
            MonthsPerYear
        } ;

        public enum TBarCodeType
        {
            Bar_None = 0,   // as new states are added, update the string array below
            Bar_2D,         // 2D bar code
            Bar_1D          // 1D Bar Coide
        }

        public enum TParseDataResult
        {
            PARSE_DATA_GOOD = 0,
            PARSE_DATA_BAD = -1,
            PARSE_DATA_PARTIAL = -2,
            PARSE_DATA_UNKNOWN = -3
        }

        public enum TSubfileType
        {
            sbfUnknown = 0,
            sbfDriversLicID,
            sbfVehicleTitle,
            sbfVehicleRegistration,
            sbfMotorCarrierCabCard,
            sbfMotorCarrierRegistrant,
            sbfVehicleSafetyInspection,
            sbfVehicleOwners,
            sbfVehicleInfo,
            // jurisdiction specific subfile types
            sbfNYVehicleInfo,
            sbfNYRegistration,
            sbfNYVersionControl
        }

        public enum TPDF417DocumentClass                                // general classification
        {
            pdfUnknown = 0,
            pdfDriverLicOrIDCard,
            pdfVehicleDocument
        }


        public enum TDisplayConfirmationMode
        {
            dispUnknown = 0,
            dispVehicleDataOnly,
            dispPersonDataOnly,
            dispVehicleAndPersonData
        }

        public struct TSubFileDesignator
        {
            public TSubfileType fSubfileType;
            public int fSubfileOffset;
            public int fSubfileLength;
        }

        public struct TStateCode
        {
            public String StateCode;
            public String StateValue;
            public TStateCode(String iStateCode, String iStateValue)
            {
                StateCode = iStateCode;
                StateValue = iStateValue;
            }
        }

        public struct TBarCodeResultData
        {
            public List<string> FieldsNamesList;
            public List<string> FieldsValuesList;
        }
    }
    
    public class TBarCodeParser : BarCodeCommons
    {
        //Class fields
        public List<String> fParsedFieldNamesAsDriver;
        public List<String> fParsedFieldNamesAsRegOwn;
        public List<String> fParsedFieldValues_PersonName;
        public List<String> fParsedFieldNames_VehicleInfo;
        public List<String> fParsedFieldValues_VehicleInfo;

        //public List<String> fStateCode;
        //public List<String> fStateValue;
        public List<TStateCode> fSupportedStates;
        
        //TBarCodeData fBarCodeData;
        int fByteCount;
        String fBarCodeString;
        TBarCodeType fLastBarTypeRead;
        TPDF417DocumentClass fDocumentClass; // general classification

        TDisplayConfirmationMode fDisplaySummaryMode;     // how will we show the confirmation?



        bool fVehicleDataPresent;		// flags set when data actually pulled, to determine how to format confirmation 
        bool fPersonDataPresent;

        // info extracted from the AAMVA header
        private char fAAMVA_BarHeader_DataElementSeparatorChar;
        private char fAAMVA_BarHeader_RecordSeparatorChar;
        private char fAAMVA_BarHeader_SegmentTerminatorChar;
        private String fAAMVA_BarHeader_FileType = ""; //new char[AAMVA_BarHeader_FileTypeSize + 1];
        private String fAAMVA_BarHeader_IssuerIDNumber = "";
        private String fAAMVA_BarHeader_AAMVAVersionNumber = ""; //new char[AAMVA_BarHeader_AAMVAVersionNumberSize + 1];
        private String fAAMVA_BarHeader_JurisdictionVersionNumber = ""; //new char[AAMVA_BarHeader_JurisdictionVersionNumberSize + 1];
        private int fAAMVA_BarHeader_SubfileCount;
        private TSubFileDesignator[] fSubfileHeaders = new TSubFileDesignator[MAX_SUBFILE_TYPES];
        private TSubFileDesignator fCurrentParseSubfileInfo;  // ptr to the subfile being parsed.. to save having to pass this around repeatedly
        private List<string> fDataElementList = new List<string>();

        public TBarCodeParser()
        {
            fParsedFieldNamesAsDriver = new List<String>();
            fParsedFieldNamesAsRegOwn = new List<String>();  // need to delete the strings inside??
            fParsedFieldValues_PersonName = new List<String>();
            fParsedFieldNames_VehicleInfo = new List<String>();
            fParsedFieldValues_VehicleInfo = new List<String>();
            //fStateCode.Clear(); 
            //fStateValue.Clear(); 
            fSupportedStates = new List<TStateCode>();
            FillJurisdictionCodes();
        }

        /*
         ~TBarCodeParser()
        {
            fParsedFieldNamesAsDriver.Clear();  // need to delete the strings inside??
            fParsedFieldNamesAsRegOwn.Clear();  // need to delete the strings inside??
            fParsedFieldValues_PersonName.Clear();
            fParsedFieldNames_VehicleInfo.Clear();
            fParsedFieldValues_VehicleInfo.Clear();
            //fStateCode.Clear(); 
            //fStateValue.Clear(); 
            fSupportedStates.Clear();
        }
         */ 

        public TDisplayConfirmationMode GetDisplayConfirmationMode()
        {
            return this.fDisplaySummaryMode;
        }

        // Put all the state jurisdictions in TStringlists, easy to find
        private void FillJurisdictionCodes()
        {
            // these need to be kept in parallel slots for lookup
            fSupportedStates.Add(new TStateCode("AL","636033"));
            fSupportedStates.Add(new TStateCode("LA","636007"));
            fSupportedStates.Add(new TStateCode("AR","636026"));
            fSupportedStates.Add(new TStateCode("ME","636041"));
            fSupportedStates.Add(new TStateCode("OH","636023"));
            fSupportedStates.Add(new TStateCode("AR","636021"));
            fSupportedStates.Add(new TStateCode("MD","636003"));
            fSupportedStates.Add(new TStateCode("OK","636058"));
            fSupportedStates.Add(new TStateCode("BC","636028"));
            fSupportedStates.Add(new TStateCode("MA","636002"));
            fSupportedStates.Add(new TStateCode("ON","636012"));
            fSupportedStates.Add(new TStateCode("CA","636014"));
            fSupportedStates.Add(new TStateCode("MI","636032"));
            fSupportedStates.Add(new TStateCode("OR","636029"));
            fSupportedStates.Add(new TStateCode("CO","636020"));
            fSupportedStates.Add(new TStateCode("MN","636038"));
            fSupportedStates.Add(new TStateCode("PA","636025"));
            fSupportedStates.Add(new TStateCode("CT","636006"));
            fSupportedStates.Add(new TStateCode("MS","636051"));
            fSupportedStates.Add(new TStateCode("RI","636052"));
            fSupportedStates.Add(new TStateCode("DC","636043"));
            fSupportedStates.Add(new TStateCode("MO","636030"));
            fSupportedStates.Add(new TStateCode("SA","636044"));
            fSupportedStates.Add(new TStateCode("DE","636011"));
            fSupportedStates.Add(new TStateCode("MT","636008"));
            fSupportedStates.Add(new TStateCode("SC","636005"));
            fSupportedStates.Add(new TStateCode("FL","636010"));
            fSupportedStates.Add(new TStateCode("NE","636054"));
            fSupportedStates.Add(new TStateCode("SD","636042"));
            fSupportedStates.Add(new TStateCode("GA","636055"));
            fSupportedStates.Add(new TStateCode("NV","636049"));
            fSupportedStates.Add(new TStateCode("TN","636053"));
            fSupportedStates.Add(new TStateCode("GM","636019"));
            fSupportedStates.Add(new TStateCode("NB","636017"));
            fSupportedStates.Add(new TStateCode("HI","636047"));
            fSupportedStates.Add(new TStateCode("NH","636039"));
            fSupportedStates.Add(new TStateCode("TX","636015"));
            fSupportedStates.Add(new TStateCode("ID","636050"));
            fSupportedStates.Add(new TStateCode("NJ","636036"));
            fSupportedStates.Add(new TStateCode("UT","636040"));
            fSupportedStates.Add(new TStateCode("IL","636035"));
            fSupportedStates.Add(new TStateCode("NM","636009"));
            fSupportedStates.Add(new TStateCode("VT","636024"));
            fSupportedStates.Add(new TStateCode("IN","636037"));
            fSupportedStates.Add(new TStateCode("NY","636001"));  // NY is messed up, but this is what it should be, they may fix it someday
            fSupportedStates.Add(new TStateCode("VA","636000"));
            fSupportedStates.Add(new TStateCode("IA","636018"));
            fSupportedStates.Add(new TStateCode("IA","A36018")); // Iowa is messed up
            fSupportedStates.Add(new TStateCode("NF","636016"));
            fSupportedStates.Add(new TStateCode("WA","636045"));
            fSupportedStates.Add(new TStateCode("KS","636022"));
            fSupportedStates.Add(new TStateCode("NC","636004"));
            fSupportedStates.Add(new TStateCode("WI","636031"));
            fSupportedStates.Add(new TStateCode("KY","636046"));
            fSupportedStates.Add(new TStateCode("ND","636034"));
        }

        public int FindStateByValue(String iStateValueStr)
        {
            for( int i=0; i<fSupportedStates.Count;i++)
            {
                if(fSupportedStates[i].StateValue.Contains(iStateValueStr))
                {
                    return i;
                }
            }
            return -1;
        }

        public int FindStateByCode(String iStateCodeStr)
        {
            for( int i=0; i<fSupportedStates.Count;i++)
            {
                if(fSupportedStates[i].StateCode.Contains(iStateCodeStr))
                {
                    return i;
                }
            }
            return -1;
        }

        //--------------------------------
        private int FindDataElementID( String iDataElementID )
        {
            // specialized routine to find a char in an array- we cant use built in string
            // functions becuause encoded data contains null chars

            // subfile ptr must be set
            if ( fCurrentParseSubfileInfo.fSubfileLength <= 0) 
            {
                return -1;
            }

            /*
            int loByteCount;
            char loSourceString[4];
            char loDestinationString[4];
            memset( loDestinationString, 0, 4 );
            memcpy( loDestinationString, iDataElementID, 3 );

            loByteCount = fByteCount-3;

	        int fSubfileOffset;
	        int fSubfileLength;
            */
            
            //for ( int loSourcePos = fCurrentParseSubfileInfo.fSubfileOffset; loSourcePos < fCurrentParseSubfileInfo.fSubfileOffset + fCurrentParseSubfileInfo.fSubfileLength; loSourcePos++ )
            int loSourcePos = fCurrentParseSubfileInfo.fSubfileOffset;
            loSourcePos += fBarCodeString.Substring(loSourcePos).IndexOf(iDataElementID);
            if (loSourcePos >= fCurrentParseSubfileInfo.fSubfileOffset)
            {
	            return loSourcePos;
            }

            // nada 
            return -1;
        }


        // Get the value for a field postion, searches for the field name then gets the data up
        // to the delimiter
         private String ExtractValueForField(String iFieldName)
        {

            // reset
            String loDataStr = "";
            int ioLength = 0;
            int ioPos = 0;

            ioLength = FindDataElementID( iFieldName);

            // didn't find the required field, just get out
            if ( ioLength < 1 ) 
            {
                return loDataStr;
            }
  
            // add the length of the field
            ioLength +=3;

            //static wchar_t loWMagString[ MAX_TRK_BYTES ];
            

            // pull chars until...
            while ( ioLength < fBarCodeString.Length)
            {
                if (( fBarCodeString[ ioLength ] == fAAMVA_BarHeader_DataElementSeparatorChar ) || ( fBarCodeString[ ioLength ] == fAAMVA_BarHeader_SegmentTerminatorChar ))
                //if (( fBarCodeString[ ioLength ] == AAMVA_DataElementSeparator ) || ( fBarCodeString[ ioLength ] == AAMVA_SegmentTerminator ))
	            {
                    // move past the delimeter that stopped the pull
                    ioPos++;
                    break;
                }

                loDataStr += fBarCodeString[ ioLength ];
                ioLength++;
                ioPos++;
            }
            return loDataStr;

        }



        //---------------------------------------------------------------------
        //#if defined( DEBUG) && defined( _M_IX86 )
        private static int cnSampleIndex = 2;
        private const int cnSampleMax = 4;
        public String GetSampleData( )
        {
	        // all the samples we have
            String loSampleStr = "";
	        String []cnNewJerseyVehInspectionSample = new String[cnSampleMax]
            {
                "@~.AAMVA6360360203VH00490046IN00950007ZJ01020020VHVAD4T1BG22K3XU510999~VAKTOYO~VAL1999~VAMCAMINEARPZJZJCSI~ZJD59796477~",
                "@~.AAMVA6360360203VH00490048IN00970007ZJ01040020VHVAD3N1CB51D74L854680  ~VAKNISS~VAL2004~VAMSENINEARPZJZJCSI~ZJD36868292",
                "@~	AAMVA36001005VH00670058RG01250037ZV01620042ZR02040015ZZ02190013VH~VADWBAUN13548VH80504~VAKBMW ~VAL08~VAQ006~VAT003571 LBRG~RAG20101223  ~RALPAS ~RAMEMD5118 ZV~ZVA2008~ZVBBMW ~ZVCCONV~ZVDG~ZVEBL   ZR~ZRA1~ZRBPACZZ~ZZZ199706",
                "@\n\t\rAAMVA36001005VH00670058RG01250037ZV01620042ZR02040015ZZ02190013VH\nVADIJM1HD4610N0104431\nVAKMAZD\nVAL92\nVAQ006\nVAT003400 LB\rRG\nRAG19980404  \nRALSRF \nRAMVINITSKY\rZV\nZVA1992\nZVBMAZDA\nZVC4DSD\nZVDG\nZVEBL   \rZR\nZRA1\nZRBMOT\rZZ\nZZZ199706\r" // sample with leading 'I' in VIN
            };

	        

	        //cnSampleIndex++;
	        if ( cnSampleIndex >= cnSampleMax )
	        {
		        cnSampleIndex = 0;
	        }


            // this is the one we're using now
	        String loSelectedSample = cnNewJerseyVehInspectionSample[cnSampleIndex];
	        
            // copy the data  and length
            //int loSampleSize = loSelectedSample.Length;
            
            char [] loMagData = loSelectedSample.ToCharArray();


            // undo the substitutions  
            for (int loSubIdx = 0; loSubIdx < loSelectedSample.Length; loSubIdx++)
	        {
		        // put back 00,
                if (loSelectedSample[loSubIdx] == 0xF8)
		        {
			        loMagData[loSubIdx] = (char)0x00;
			        continue;
		        }

		        // put back 0A
                if (loSelectedSample[loSubIdx] == 0x7E)
		        {
			        loMagData[ loSubIdx ] = (char)0x0A;  
			        continue;
		        }

		        // put back 0D
                if (loSelectedSample[loSubIdx] == 0x7F)
		        {
			        loMagData[ loSubIdx ] = (char)0x0D;
			        continue;
		        }


		        // put back FF
                if (loSelectedSample[loSubIdx] == 0xF1)
		        {
			        loMagData[ loSubIdx ] = (char)0xFF;
			        continue;
		        }
	        }

            for(int i=0; i<loMagData.Length;i++)
                loSampleStr += loMagData[i].ToString();

            return loSampleStr;
        }
//#endif

        //---------------------------------------------------------------------
        /*
        * copy the raw mag stripe data into a buffer for parsing
        */
        private void CopyRawBarDataToParsingBuffer( String iRawMagData )
        {
            // emulator doesn't pass data, only a dummy pointer when no file is found
            // only try to copy if we were passed something
            if ( iRawMagData != null && iRawMagData.Length > 0)
            {
                // clean up any previous data
                fBarCodeString = "";
                char[] loTempArraySrc = iRawMagData.ToCharArray();
                for(int i = 0; i<loTempArraySrc.Length; i++)
                {
                    if(loTempArraySrc[i] != '{' && loTempArraySrc[i] != '}')
                    {
                        fBarCodeString += loTempArraySrc[i].ToString();
                    }
                }
                
	            // and the length
                fByteCount = fBarCodeString.Length;

	            //#ifdef _LOG_RAW_BARCODE_DATA_
                // save the data for debugging
	            //LogRawBarcodeData( "RAW BARCODE DATA", &iRawMagData->fBarCodeString[0], iRawMagData->fByteCount );
	            //#endif



	            //#if (HHTarget!=HHTarget_CBuilderEmulator) || (HHTarget == HHTarget_PocketPCEmulator)
                // wipe out the source so we won't see it again
                //iRawMagData = "";
	            //#endif
            }
        }


        /*
         * 
         */
         public int FindString(List<String> inStringList, String inTarget)
         {
             int loIdx;
             for(loIdx = 0; loIdx < inStringList.Count; loIdx++)
             {
                 if(inStringList[loIdx].CompareTo(inTarget) == 0)
                 {
                     return loIdx;
                 }

             }
             return -1;
         }
         
        /* ----------------------------------------------------------------------
        *
        * return the parsed field value for a column name
        * used for destination selection. just a wrapper around 2 functions for
        * cleaner code
        *
        * searches based on the DriverName column list (vs the regown column names)
        *
        */
        public String GetParsedFieldDataByDriverName(  String iFieldName )
        {
            //fParsedFieldValues_PersonName->GetString( fParsedFieldNamesAsDriver->FindString( iFieldName ) );
            int loIdx = FindString(fParsedFieldNamesAsDriver, iFieldName);
            if(loIdx >= 0)
            {
                return fParsedFieldValues_PersonName[loIdx];
            }
            return ""; 
        }

        public String GetParsedFieldDataByVehicle( String iFieldName )
        {
            //fParsedFieldValues_VehicleInfo->GetString( fParsedFieldNames_VehicleInfo->FindString( iFieldName ) );
            int loIdx = FindString(fParsedFieldNames_VehicleInfo, iFieldName);
            if(loIdx >= 0)
            {
                return fParsedFieldValues_VehicleInfo[loIdx];
            }
            return "";
        }


        /* ----------------------------------------------------------------------
        *
        * dedicated to trimming excess spaces or commas from the end of parsed mag data
        *
        */
        String TrimMagData(String iTrackData)
        {
            String loTrackDataStr = "";
            // dont start on null or empty strings
            if ( ( iTrackData == "" ) || ( iTrackData.Length <= 0 ) ) { return ""; };
            
            // init to length, adjusting for zero based array
            int loPos = iTrackData.Length-1; //(strlen( ioTrackData ) - 1);
            char []loTrackData =  iTrackData.ToCharArray();         
            // work from the end, replacing any spaces or commas or equal signs with 0x00
            while (
                ( loTrackData[ loPos ] == 0x20 ) ||
                ( loTrackData[ loPos ] == 0x2C ) ||
                ( loTrackData[ loPos ] == 0x3D )
                )
            {
                loTrackData[ loPos ] = (char)0x00;
                loPos--;
                // reached the start of the string?
                if ( loPos < 0 ) { break; };
            }
            loTrackDataStr = "";
            for(int i=0; i<loTrackData.Length;i++)
                loTrackDataStr += loTrackData[i].ToString();

            return loTrackDataStr;
        }


        /* ----------------------------------------------------------------------
        *
        * replace an already existing field value with a new value
        * used for post-parsing formatting
        *
        * searches based on the DriverName column list (vs the Witness column names)
        *
        */
        public void UpdateParsedFieldDataByDriverNames( String iFieldName, String iNewFieldValue )
        {
            int loIdx;

            // locate the field name by index
            loIdx = FindString(fParsedFieldNamesAsDriver, iFieldName );
            // if we can find it, the we can update it
            if ( loIdx != -1 )
            {
                // trim it
                String loFieldValue = TrimMagData( iNewFieldValue );
                // update it
                fParsedFieldValues_PersonName[loIdx] = loFieldValue;
            }
        }


        /* ----------------------------------------------------------------------
        *
        * Replace an already existing field value with a new value, or add it if its already in the list
        *
        *
        */
        void AddOrUpdateParsedFieldData( String iFieldName, String iNewFieldValue, List<String>iFieldNames, List<String>iFieldValues )
        {
            int loIdx;

            // trim it
            String loFieldValue =TrimMagData( iNewFieldValue );

            // is this a placeholder?
            if ( iFieldName.Contains(RONoFieldDefined))
            {
                // does this field already exist?
                loIdx = FindString(iFieldNames,iFieldName );
            }else{
	            // its a place holder, these cannot be updated (they would overwrite each other)
                loIdx = -1;
            }

            // if we can find it, the we can update it
            if ( loIdx != -1 )
            {
                // update it
                iFieldValues[loIdx] = loFieldValue;
            }else{
	            // add it
                iFieldNames.Add(iFieldName );
                iFieldValues.Add(loFieldValue);
            }
        }




        /* ----------------------------------------------------------------------
        *
        * add extracted field name and value to the global lists
        */
        void AddParsedFieldNameAndDataToList_PersonName( String iAddFieldNameAsDriver, String iAddFieldNameAsRegOwn, String iAddFieldValue )
        {
            // trim the data string
            String loFieldValue = TrimMagData(iAddFieldValue);

            // add this field and it value to the lists
            fParsedFieldNamesAsDriver.Add(iAddFieldNameAsDriver);
            fParsedFieldNamesAsRegOwn.Add(iAddFieldNameAsRegOwn);
            fParsedFieldValues_PersonName.Add(loFieldValue);
        }

        /////////////////////////////////////////////
        void AddParsedFieldNameAndDataToList_VehicleInfo( String iAddFieldName, String iAddFieldValue )
        {
            // trim the data string
            String loFieldValue = TrimMagData( iAddFieldValue );
            //Is it VIN field?
            if (iAddFieldName.Contains(DBConstants.sqlVehVINStr))
            {
                if (loFieldValue.Length == 18)
                {
                    //VIN has leading 'I', see what we should do now based on the registry settings
                    if (TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regBARCODE_1D_VIN_REMOVE_LEADING_IMPORT_CHARACTER, TTRegistry.regBARCODE_1D_VIN_REMOVE_LEADING_IMPORT_CHARACTER_DEFAULT) == TTRegistry.VIN_STRIP_LEADING_I)
                    {
                        //We should strip the leading 'I'
                        String loTempStr = loFieldValue.Substring(1, 17);
                        loFieldValue = "";
                        loFieldValue = loTempStr;
                    }
                }
            }
            // add this field and it value to the lists
            fParsedFieldNames_VehicleInfo.Add(iAddFieldName);
            fParsedFieldValues_VehicleInfo.Add(loFieldValue);
        }

        /*
        * DaysInYear
        *
        * Passed a year, returns the number of days in it.
        *
        */
        int DaysInYear( int iYear )
        {
            /* if year isn't divisible by 4, definitely not a leap year */
            if ((iYear & 0x03) != 0)
                return DAYS_IN_YEAR;
  
            /* This year is divisible by 4. If it not divisible by 100, definiteley IS a leap year */
            if ((iYear % 100) != 0)
                return DAYS_IN_LEAPYEAR;
  
            /* This year is divisible by 100. If it is also divisible by 400, then it IS a leap year */
            if ((iYear % 400) != 0)
                return DAYS_IN_YEAR;
            //Ok, leap year
            return DAYS_IN_LEAPYEAR;
        }

        int DaysInMonth( int iYear, TMonthsOfYear iMonth )
        {
            int[] unDaysInMonths= new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            if ((iMonth < TMonthsOfYear.January) || (iMonth > TMonthsOfYear.December)) return -1;
            if (iMonth != TMonthsOfYear.February) return unDaysInMonths[ (int)iMonth ];
            if (DaysInYear( iYear) == DAYS_IN_LEAPYEAR) return 29;
            return 28; 
        }


        /* ----------------------------------------------------------------------
        *
        * maniplulate the DLExp and DLBirthdate field values for states that
        * bend the AAMVA rules, namely EVERYBODY??? maybe the published rules are wrong!
        *
        */
        void ModifyBirthAndExpDates_AlmostAAMVAStyle()
        {
            String loExpStr;
            String loBirthDateStr;
            String loBirthCCYYStr;
            String loBirthMMStr;
            String loBirthDDStr;
            String loExpCCYYStr;
            String loExpMMStr;
            String loExpDDStr;
            String loNewBirthStr;
            String loNewExpStr;
            int loYearInt = 0;
            int loMonthInt = 0;
            int loDayInt = 0;


            // is this a 2D barcode?
            if ( fLastBarTypeRead == TBarCodeType.Bar_2D )
            {
                // leave it all alone  - TBD - exceptions?
 	            return;
            }


            // get the BIRTHDATE and the EXPDATE
            //strcpy( loBirthDateStr, fParsedFieldValues_PersonName->GetString( fParsedFieldNamesAsDriver->FindString( DLDATEOFBIRTHFieldName ) ) );
            loBirthDateStr = fParsedFieldValues_PersonName[FindString(fParsedFieldNamesAsDriver, DBConstants.sqlDLBirthDateStr)];
            //strcpy( loExpStr, fParsedFieldValues_PersonName->GetString( fParsedFieldNamesAsDriver->FindString( DLEXPDATEFieldName ) ) );
            loExpStr = fParsedFieldValues_PersonName[FindString(fParsedFieldNamesAsDriver, DBConstants.sqlDLExpDateStr)];

            // reset
            loNewBirthStr = "";
            loNewExpStr = "";

            // get the CCYY, MM and DD from the DLBirthdate into a seperate string so we can compare
            //memcpy( loBirthCCYYStr, &loBirthDateStr[0], 4 );
            loBirthCCYYStr = loBirthDateStr.Substring(0,4);
            //memcpy( loBirthMMStr, &loBirthDateStr[4], 2 );
            loBirthMMStr = loBirthDateStr.Substring(4,2);
            //memcpy( loBirthDDStr, &loBirthDateStr[6], 2 );
            loBirthDDStr = loBirthDateStr.Substring(6, 2);
            // get the MM and YY from DLExp so we can build new values
            //memcpy( loExpCCYYStr, &loExpStr[0], 4 );
            loExpCCYYStr = loExpStr.Substring(0, 4);
            //memcpy( loExpMMStr, &loExpStr[4], 2 );
            loExpMMStr = loExpStr.Substring(4, 2);
            //memcpy( loExpDDStr, &loExpStr[6], 2 );
            loExpDDStr = loExpStr.Substring(6, 2);

            // the DLEXP is in YYMM format... check for some special cases in the BIRTHDATE
            // to flesh out the rest of the info between the two
            //
            // these are CA and FL manipulations that are variants on the standard AAMVA
            if ( loBirthMMStr.Contains("77"))
            {
                // this license is "non-expiring"... what should we do?
                // for now, lets wipe out the expiration info so there won't be invalid
                // data sent to the edit field
                loNewBirthStr = loBirthDateStr;
            }else if (loBirthMMStr.Contains("88")){
                // the expiration date is at the end of the month of the birthdate
                // and the year of the expiration date
                // convert from ascii to int, continuing if there's no error code
                loYearInt = Convert.ToInt32(loExpCCYYStr);
                loMonthInt = Convert.ToInt32(loExpMMStr);
                if (
                    ( loYearInt > -1 ) &&
                    ( loMonthInt> -1 )
                   )
                {
                    // get the last day of the month
                    loDayInt = DaysInMonth( loYearInt, (TMonthsOfYear)(loMonthInt-1) );
                }

                // convert it back to string
                loExpDDStr = loDayInt.ToString();
                // put it all back together
                loNewExpStr = loExpCCYYStr + loBirthMMStr + loExpDDStr;

                // this is just a copy
                loNewBirthStr = loBirthDateStr;

            }else if ( loExpMMStr.Contains("99")) {// IA cards were encoded improperly and follow THIS logic
                // the expiration date is on the year of the DLExp and
                // the month and day of the birthdate
                loNewExpStr = loExpCCYYStr + loBirthMMStr + loBirthDDStr;
                // this is just a copy
                loNewBirthStr = loBirthDateStr;
            }else{// all others will follow this format
                // the expiration date is on the month and year of the DLExp and
                // the day of the birthdate
                loNewExpStr= loExpCCYYStr + loExpMMStr + loBirthDDStr;

                // and the birthday will find its correct month within the EXP field
                loNewBirthStr = loBirthCCYYStr + loBirthMMStr + loBirthDDStr;
            }

            // put back our newly formatted dl exp string
            UpdateParsedFieldDataByDriverNames(DBConstants.sqlDLExpDateStr, loNewExpStr);
            // and our formatted birthdate
            UpdateParsedFieldDataByDriverNames( DBConstants.sqlDLBirthDateStr, loNewBirthStr );
        }

        //---------------------------------------------------
        /*
        * converts the already extracted Height, inserting the passed char
        * (usually a slash or a dash)
        *
        * used in several states
        *
        */
        void FormatHeight( char iSeperator )
        {
            String loHeightStr;
            String loHeightValueStr, loHeightInchStr, loHeightUpperStr;  
            int loHeightInt, loHeightIntFeet, loHeightIntInches;
            char[] loSepStr = new char[2];


            // get the DLHEIGHT
            //strcpy( loHeightStr, fParsedFieldValues_PersonName->GetString( fParsedFieldNamesAsDriver->FindString( SuspHeightFieldName ) ) );
            loHeightStr = fParsedFieldValues_PersonName[FindString(fParsedFieldNamesAsDriver, DBConstants.sqlSuspHeightStr)];
            // is this a 2D barcode?
            if ( fLastBarTypeRead == TBarCodeType.Bar_2D )
            {
                // try to process as AAMVA standard

                // convert the street to upper case for universal comparisons
                loHeightUpperStr = loHeightStr.ToUpper();

	            // aamva - look for IN or CM
	            int loInchPos = loHeightUpperStr.IndexOf(" IN");
	            if ( loInchPos != -1 )
	            {
		            loHeightValueStr = loHeightUpperStr.Substring(0, loInchPos);


		            // convert from ascii to int, continuing if there's no error code
                    loHeightInt = Convert.ToInt32(loHeightValueStr);
		            if ( loHeightInt  > -1 )
		            {
		                // convert to ft and inches
			            loHeightIntFeet = loHeightInt / 12;
			            loHeightIntInches = loHeightInt - ( loHeightIntFeet * 12 );

			            // convert it back to string
			            loHeightValueStr = loHeightIntFeet.ToString();

			            loHeightInchStr = loHeightIntInches.ToString();
			            // leading zero if needed
			            if (loHeightInchStr.Length < 2 )
			            {
				            //uHALr_charinsert( '0', loHeightInchStr, 0 );
	                        loHeightInchStr = "0" + loHeightInchStr;
			            }


			            // convert seperator into string
			            loSepStr[0] = iSeperator;
			            loSepStr[1] = (char)0;
			

			            // rebuild str
			            //loHeightStr[0] = (char)0;
			            loHeightStr= loHeightValueStr + loSepStr + loHeightInchStr;

			            // update it
                        UpdateParsedFieldDataByDriverNames(DBConstants.sqlSuspHeightStr, loHeightStr);

			            // done
			            return;
		            }
	            }

	            // if it didn't have IN in the text, then its probably CM, and we will leave it as is, ie "181 CM"
                UpdateParsedFieldDataByDriverNames(DBConstants.sqlSuspHeightStr, loHeightUpperStr);
	            return;
            }


            // fall back to default formatting
            // insert the slash or the dash
            loHeightStr.Insert(1, iSeperator.ToString());
            // update it
            UpdateParsedFieldDataByDriverNames(DBConstants.sqlSuspHeightStr, loHeightStr);
        }



        //---------------------------------------------------
        /*
        * converts the already extracted gender from a numeric digit
        * or 1 or 2 to an alpha char M or F
        *
        * used in FL
        *
        */
        void ConvertGenderFromNumericToAlpha()
        {
            String loGenderStr;
            char loGenderChar;

            // get the gender already stored as a numeric string
            loGenderStr = fParsedFieldValues_PersonName[FindString(fParsedFieldNamesAsDriver, DBConstants.sqlSuspGenderStr)];
            // extract the numeric
            loGenderChar = loGenderStr[0];
            // reset
            loGenderStr = "";

            // convert it
            switch ( loGenderChar )
            {
                case '1' :
                    loGenderStr.Insert(0, "M");
                    break;
                case '2' :
                    loGenderStr.Insert(0, "F");
                    break;
                default  :
                    return;  // unknown conversion data, leave the empty string
            }

            // put it back
            UpdateParsedFieldDataByDriverNames(DBConstants.sqlSuspGenderStr, loGenderStr);
        }

        //---------------------------------------------------
        /*
        * checks the already extracted names...
        * - if LASTNAME is NULL and MIDDLENAME isn't, these get swapped
        *
        */
        void CleanFirstMiddleLastNames()
        {
            String loLastNameStr = "";
            String loMiddleNameStr = "";

            // get the LASTNAME
            loLastNameStr = fParsedFieldValues_PersonName[FindString(fParsedFieldNamesAsDriver, DBConstants.sqlSuspLastNameStr)];

            // is the LASTNAME empty?
            if (loLastNameStr.Length == 0 )
            {   // get the middle name
                loMiddleNameStr = fParsedFieldValues_PersonName[FindString(fParsedFieldNamesAsDriver, DBConstants.sqlSuspMiddleNameStr)];
                // if its NOT empty, we're gonna swap em
                if (loMiddleNameStr.Length !=0 )
                {
                    // put the MIDDLENAME in the LASTNAME slot
                    UpdateParsedFieldDataByDriverNames(DBConstants.sqlSuspLastNameStr, loMiddleNameStr);
                    // put the (empty) LASTNAME in the MIDDLENAME slot
                    UpdateParsedFieldDataByDriverNames( DBConstants.sqlSuspMiddleNameStr, loLastNameStr );
                }
            }

        }

        //---------------------------------------------------
        /*
        * checks the already extracted names...
        * - if FIRSTNAME has a COMMA in it, EXTRACT the FIRST and MIDDLE names from it
        *
        */
        void LookForMiddleNameAttatchedToFirstName()
        {
            String loOldFirstNameStr = "";
            String loNewFirstNameStr = "";
            String loMiddleNameStr = "";
            String loNewDataStr = "";
            int loPos = 0;
            //  int loLength;

            // get the FIRSTNAME
            loOldFirstNameStr = fParsedFieldValues_PersonName[FindString(fParsedFieldNamesAsDriver, DBConstants.sqlSuspFirstNameStr)];

            // is the FIRSTNAME not empty?
            if (loOldFirstNameStr.Length != 0 )
            {   // get the middle name
                loMiddleNameStr = fParsedFieldValues_PersonName[FindString(fParsedFieldNamesAsDriver,DBConstants.sqlSuspMiddleNameStr)];
                // if the middlename is empty, we're gonna try to extract two names out of the first
                if (loMiddleNameStr.Length == 0 )
                {   
                    // put em back in there
                    UpdateParsedFieldDataByDriverNames(DBConstants.sqlSuspFirstNameStr, loNewFirstNameStr);
                    UpdateParsedFieldDataByDriverNames(DBConstants.sqlSuspMiddleNameStr, loMiddleNameStr);
                }
            }

        }

        //---------------------------------------------------
        /*
        * checks the already extracted block - street combonation and looks for
        * PO BOX entries, which are sometimes encoded with spaces, ie "P O BOX"
        * and corrects the extraction
        *
        * used in most states
        *
        */
        void CleanUpPOBoxAddresses()
        {
            String loBlockStr;
            String loStreetStr;
            String loStreetUpperStr;

            // get the BLOCK and the STREET
            loBlockStr = fParsedFieldValues_PersonName[FindString(fParsedFieldNamesAsDriver, DBConstants.sqlSuspAddrStreetNoStr)];
            loStreetStr = fParsedFieldValues_PersonName[FindString(fParsedFieldNamesAsDriver, DBConstants.sqlSuspAddrStreetStr)];

            // convert the street to upper case for universal comparisons
            loStreetUpperStr = loStreetStr.ToUpper();

            if (
                    // is STREET start with part of "PO BOX" phrase?
                    (loStreetUpperStr.IndexOf("O BOX" ) == 0 )  &&
                    // and does the BLOCK simply contain "P"?
                    ( loBlockStr[0] == 'P' || loBlockStr[0] == 'p' )
                )
            {
                // using loStreetUpperStr as a workspace, concatenate the values together
                loStreetUpperStr = loBlockStr + " " + loStreetStr;

                // clear the BLOCK
                loBlockStr = "";
                // replace them
                UpdateParsedFieldDataByDriverNames(DBConstants.sqlSuspAddrStreetNoStr, loBlockStr);
                UpdateParsedFieldDataByDriverNames(DBConstants.sqlSuspAddrStreetStr, loStreetUpperStr);
            }

        }

        //---------------------------------------------------
        /*
        * checks the already extracted zip code and looks for
        * ZIP + 4 entries, which are sometimes padded with extra zeros ie "920560000"
        * and removes the extraneous 0000
        *
        * used in most states
        *
        */
        void CleanUpZipCodes()
        {
            String loZIPStr;
            String loZIPPlus4Str;
            String loNewZIPStr;

            // get the ZIP
            loZIPStr = fParsedFieldValues_PersonName[FindString(fParsedFieldNamesAsDriver, DBConstants.sqlSuspAddrZipStr)];

            // is it EXACTLY 9 digits?
            if ( loZIPStr.Length != 9 )
            {
                // if its not EXACTLY 9 digits, we're not gonna mess with it
                return;
            }

            // copy the last four digits
            loZIPPlus4Str =  loZIPStr.Substring(5, 4);

            // are the last three digits 0000?
            if ( loZIPPlus4Str.Contains("0000"))
            {
                // they're not 0000, we're outta here
                return;
            }

            // we don't want to see 0000, so just delete the digits
            //uHALr_strdelete( loZIPStr, 5, 4 );
            loZIPStr.Remove(5,4);

            // replace it in the list
            UpdateParsedFieldDataByDriverNames(DBConstants.sqlSuspAddrZipStr, loZIPStr);
        }

        //-------------------------------------------------
        /*
        * Extract field starting at ioPos, pulling until one of the
        * delimeters or iMaxChar is reached. if any iDelimX is 0x00 it is not
        * considered as a delimeter
        */
        void ExtractDataField( String   iDataStr, 
							   int ioPos,
                               char iDelim1,
                               char iDelim2,
                               char iDelim3,
                               int iMaxChar,
                               String oDataStr,
                               int ioLength )

        {
            // reset
            oDataStr = "";
            ioLength = 0;
            char[] loOutStr = new char[iDataStr.Length];
            // pull chars until...
            while (   // iMaxChar is reached
                    ( ioLength < iMaxChar )  &&
                    // nor the maximum size of any one data item (bad data defense)
                    ( ioLength < iDataStr.Length ) &&
                    // and cannot go past the end of the track data either
                    ( ioPos < MAX_BAR_BYTES )
                   )
            {
                // check for a special case of the non-delimeter
                if ( iDelim1 != 0x00 )
                {
                    if (( iDataStr[ ioPos ] == iDelim1 ))
                    {
                        // move past the delimeter that stopped the pull
                        ioPos++;
			            // first char space, skip it
			            if ( ioLength == 0 )
 			                continue;
                        break;
                    }
                }
                // check for a special case of the non-delimeter
                if ( iDelim2 != 0x00 )
                {
                    if (( iDataStr[ ioPos ] == iDelim2 ))
                    {
                        // move past the delimeter that stopped the pull
                        ioPos++;
			            // first char space, skip it
			            if ( ioLength == 0 )
 			                continue;
                        break;
                    }

                }
                // check for a special case of the non-delimeter
                if ( iDelim3 != 0x00 )
                {
                    if (( iDataStr[ ioPos ] == iDelim3 ))
                    {
                        // move past the delimeter that stopped the pull
                        ioPos++;
			            // first char space, skip it
			            if ( ioLength == 0 )
 			                continue;
                        break;
                    }
                }

                // this is good data, get the char
                loOutStr[ioLength] = iDataStr[ ioPos ];
                ioLength++;
                ioPos++;
            }

            for (int i = 0; i < loOutStr.Length; i++)
                oDataStr += loOutStr[i].ToString();

            // did we reach the max size of the field data
            if ( ioLength ==  iDataStr.Length - 1 )
            {
                // make certain that the last byte is the string term - bad data defense
                oDataStr.Insert(oDataStr.Length - 1, "/0");
            }
        }

        //-------------------------------------------------
        /*
        * Extract the iSubFieldNo subfield starting at ioPos, pulling until one of the
        * delimeters, the subfield seperator or iMaxChar is reached. if either iDelimX is 0x00 it is not
        * considered as a delimeter, set it to 0x00 when just one or no delimiter is used
        */
        void ExtractDataSubField( String iDataStr, int ioPos,
                                  char iDelim1, char iDelim2, int iMaxChar,
                                  char iSubFieldDelim, int iSubFieldNo,
                                  String oDataStr, int ioLength )
        {
            int loSubFieldIdx = 0;
            //  char loResultStr[10];

            while ( loSubFieldIdx < iSubFieldNo )
            {
                // just extract the next field
                ExtractDataField( iDataStr, ioPos, iDelim1, iDelim2, iSubFieldDelim, iMaxChar, oDataStr, ioLength );
	            //OSShowMessage( oDataStr );
                // bump the count, we'll leave when we've got the field we want
                loSubFieldIdx++;
                // did we cross a field delimiter?
                if ( ioPos <= 0 )
                    continue;
                if (
                    ( iDataStr[ ioPos-1 ] != iDelim1 ) &&
                    ( iDataStr[ ioPos-1 ] != iDelim2 ) &&
                    ( iDataStr[ ioPos-1 ] != iSubFieldDelim )		
                   )
                {
                    continue;
                }
                // should we be returning an empty field?
                if ( loSubFieldIdx >= iSubFieldNo  )
                    break;

                // they're asking for a subfield that doesn't exist, make
                // sure we don't return the last one pulled while looking
                oDataStr = "";
                ioLength = 0;
            }
        }

        /*
        * Parse a single address field into street no and street.
        */
        void ExtractAAMVA_Address( String iDataStr)
        {
            String loData1Str = "";  // local str used to extract the data
            int loLength = 0;
            int loPos = 0;
            // 1st is street number
            ExtractDataSubField( iDataStr, loPos, CommaSubFldSep, AAMVA_FieldDelimeter, (char)37,(char)0x00, 1, loData1Str, loLength);
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspAddrStreetNoStr, DBConstants.sqlTrafROAddrStreetNoStr/*ROAddrStreetNoFieldName*/, loData1Str);
 
            // rest is street name.
            loData1Str = iDataStr.Substring(loPos);
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspAddrStreetStr, DBConstants.sqlTrafROAddrStreetStr /*ROAddrStreetFieldName*/, loData1Str);
        }


        /* ----------------------------------------------------------------------
        * common extraction routine that pulls name, stored in AAMVA format
        *
        * these are seperated into two methods because some cards store the additional
        * info in a slightly different format
        *
        */
        void ExtractAAMVA_Name( String iDataStr, TNameStyle iNameStyle )
        {
            String loData1Str = "";  // local str used to extract the data
            String loData2Str = "";
            String loData3Str = "";
            int loLength = 0;
            int loAddrBytes = 0;
            int loSubFieldStart = 0;
            int loLastNameIndex, loFirstNameIndex, loMiddleNameIndex;

            int loPos = 0;

            // set the name indexes
            if ( iNameStyle == TNameStyle.name_LFM )
            {
                loLastNameIndex = 1;
                loFirstNameIndex = 2;
                loMiddleNameIndex = 3;
            }else{
                // for name_FML
                loLastNameIndex = 3;
                loFirstNameIndex = 1;
                loMiddleNameIndex = 2;
            }

            // save the start of the sub field so we can re-reference it
            loSubFieldStart = loPos;
            // the last, first, and middle names are subfields
            ExtractDataSubField( iDataStr, loPos, CommaSubFldSep, AAMVA_FieldDelimeter, 37,(char)0x00, loLastNameIndex, loData1Str, loLength );
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspLastNameStr, DBConstants.sqlTrafRONameLastStr /*ROLastNameFieldName*/, loData1Str);

            // go back to the start of the subfield
            loPos = loSubFieldStart;
            ExtractDataSubField( iDataStr, loPos, CommaSubFldSep, AAMVA_FieldDelimeter, 37, (char)0x00, loFirstNameIndex,loData1Str, loLength );
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspFirstNameStr, DBConstants.sqlTrafRONameFirstStr /*ROFirstNameFieldName*/, loData1Str);

            // go back to the start of the subfield
            loPos = loSubFieldStart;
            ExtractDataSubField( iDataStr, loPos, CommaSubFldSep, AAMVA_FieldDelimeter, 37,(char)0x00, loMiddleNameIndex,loData1Str, loLength );
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspMiddleNameStr, DBConstants.sqlTrafRONameMiddleStr /*ROMidNameFieldName*/, loData1Str); // suffix?

            // fix up middle/last name problems
            CleanFirstMiddleLastNames();
        }


        //---------------------------------------------------
        /*
        *    2D
        */
        //---------------------------------------------------
        int Parse_2DBarCode_Header_DetermineSubfileCountOffset( )
        {

            // there are specs that describe where the subfile offset count is to be found
            // however, we have found some variance in how the states actually encode the data
            // so we will employ a brute force method to examine the raw data and figure
            // out where the sub file count actually is...

        	// start here, beyond the issuing ID
            int loIdx = AAMVA_BarHeader_IssuerIDNumberOffset + AAMVA_BarHeader_IssuerIDNumberSize;

            // we know that the values after the issuer ID are numeric, and the first subfile type will be an alpha
            for ( ; loIdx < fByteCount; loIdx++ )
            {
	            // is this alpha?
	            if (( fBarCodeString[ loIdx ] >= 'A') && ( fBarCodeString[ loIdx ] <= 'Z'))
	            {
		            // this is the first alpha of the subfile header record. the count is just before this
		            return loIdx - AAMVA_BarHeader_SubfileCountSize;
	            }
            }


            // if we fall through then we didn't find the alpha... so we'll return the the value from the spec
            return AAMVA_BarHeader_Document_SubfileCountOffset;
        }


        //////////////////////////////////////////////////////////////////////
        void Parse_2DBarCode_Header_FixUpSpecialCases()
        {
	        // fixups for special cases we've encountered

            String OREGON_IDCard_Signature = "AAMVA00000001DL";
            int OREGON_IDCard_SubfileCountOffset = 15;

            // NOTE: OREGON ID sample has "AAMVA", but doesn't follow older document standard. Nor does it follow 2005 DL/ID standard
            //if ( memcmp( &fBarCodeString[ AAMVA_BarHeader_FileTypeOffset ], OREGON_IDCard_Signature, strlen( OREGON_IDCard_Signature )) == 0 )
            String loSignStr = fBarCodeString.Substring(AAMVA_BarHeader_FileTypeOffset, OREGON_IDCard_Signature.Length);
            if (loSignStr.Contains(OREGON_IDCard_Signature))
            {
	            fAAMVA_BarHeader_JurisdictionVersionNumber = "";
                 
 	            //fAAMVA_BarHeader_JurisdictionVersionNumber[ sizeof(fAAMVA_BarHeader_JurisdictionVersionNumber)-1 ] = 0;
	            // person info
	            fDocumentClass = TPDF417DocumentClass.pdfDriverLicOrIDCard;
                return;
            }

            String NEWYORK_VehReg_SignatureBad = "AAMVA36001";
            String NEWYORK_VehReg_IssuerIDGood = "636001";

            // NEW YORK vehicle registrations are shifted over a couple chars
            //if ( memcmp( &fBarCodeString[ AAMVA_BarHeader_FileTypeOffset ], NEWYORK_VehReg_SignatureBad, strlen(NEWYORK_VehReg_SignatureBad )) == 0 )
            loSignStr = fBarCodeString.Substring(AAMVA_BarHeader_FileTypeOffset,NEWYORK_VehReg_SignatureBad.Length);
            if(loSignStr.Contains(NEWYORK_VehReg_SignatureBad))
            {
	            // fix up the issuing ID 
	            //memcpy( &fAAMVA_BarHeader_IssuerIDNumber, NEWYORK_VehReg_IssuerIDGood , strlen(NEWYORK_VehReg_IssuerIDGood) );
                fAAMVA_BarHeader_IssuerIDNumber = NEWYORK_VehReg_IssuerIDGood;
                return;
            }
        }

        /////////////////////////////////////////////////
        int Parse_2DBarCode_Header_CheckForMassachusettsVehInspection()
        {
            String MASSACHUSETTS_VehInspectionSticker_Signature1  = "S2";
            int MASSACHUSETTS_VehInspectionSticker_Signature1Offset = 0;

            String MASSACHUSETTS_VehInspectionSticker_Signature2 = "MA";
            int MASSACHUSETTS_VehInspectionSticker_Signature2Offset = 21;


            // NOTE: Massachusetts Vehicle inspection stickers don't have AAMVA subfiles, they just cram everything together
            //if ( memcmp( &fBarCodeString[ MASSACHUSETTS_VehInspectionSticker_Signature1Offset ], MASSACHUSETTS_VehInspectionSticker_Signature1, strlen( MASSACHUSETTS_VehInspectionSticker_Signature1 )) == 0 )
            String loSignStr = fBarCodeString.Substring(MASSACHUSETTS_VehInspectionSticker_Signature1Offset, MASSACHUSETTS_VehInspectionSticker_Signature1.Length);
            if(loSignStr.Contains(MASSACHUSETTS_VehInspectionSticker_Signature1))
            {
	            // if both signatures check out, we're going to read this as a Massachusetts inspection sticker
	            //if ( memcmp( &fBarCodeString[ MASSACHUSETTS_VehInspectionSticker_Signature2Offset ], MASSACHUSETTS_VehInspectionSticker_Signature2, strlen( MASSACHUSETTS_VehInspectionSticker_Signature2 )) == 0 )
                loSignStr = fBarCodeString.Substring(MASSACHUSETTS_VehInspectionSticker_Signature2Offset, MASSACHUSETTS_VehInspectionSticker_Signature2.Length);
                if(loSignStr.Contains(MASSACHUSETTS_VehInspectionSticker_Signature2))
	            {
	                return 1;
	            }
            }
            return 0;
        }



        //////////////////////////////////////////////////
        String FormatYYYYMMtoLastDayOfMonth( String iDateYYYYMMStr )
        {
            String loYYYYStr;  
            String loMMStr;  
            String loDDStr;  
            int loYearInt, loMonthInt, loDayInt;

            // remove excess
            String loDateYYYYMMStr = TrimMagData(iDateYYYYMMStr);
            // not long enough? do nothing more
            if ( loDateYYYYMMStr.Length < 6 )
            {
	            return "";
            }

            loYYYYStr = "";
            loMMStr = "";
            loDDStr = "";

            loYYYYStr = loDateYYYYMMStr.Substring(0, 4);
            loMMStr = loDateYYYYMMStr.Substring(4, 2);

            // determine the last day of month 
            // convert from ascii to int, continuing if there's no error code
            loYearInt = Convert.ToInt32(loYYYYStr);
            loMonthInt = Convert.ToInt32(loMMStr);
            if (
                    (loYearInt > -1 ) &&
                    (loMonthInt > -1 )
                )
            {
                // get the last day of the month
                loDayInt = DaysInMonth( loYearInt, (TMonthsOfYear)(loMonthInt-1) );
  
		        // convert it back to string
                loDDStr = loDayInt.ToString();

                // reset, and put it all back together
                loDateYYYYMMStr = loYYYYStr +   // YYYY
                                  loMMStr +    // MM
                                  loDDStr;    // DD
            }
            return loDateYYYYMMStr;
        }

        //////////////////////////////////////////////////
        void ExtractAndAdd_VehicleInfoField( String iFieldName, int iOffset, int iLen )
        {
            String loData1Str;  // local str used to extract the data

            loData1Str = fBarCodeString.Substring(iOffset, iLen );
            
            // look for certain field types that need special handling
            if (iFieldName.Contains(DBConstants.sqlVehInspectionStickerExpDateStr))
            {
	            // convert the field into a list compatible string
	            FormatDateFieldData( loData1Str, "MMYYYY", "YYYYMMDD" );
	            // and set to last day of month
	            FormatYYYYMMtoLastDayOfMonth( loData1Str );
            } else if (iFieldName.Contains(DBConstants.sqlVehLicExpDateStr)){
	            // convert the field into a list compatible string
	            FormatDateFieldData( loData1Str, "MMYYYY", "YYYYMMDD" );
	            // and set to last day of month
	            FormatYYYYMMtoLastDayOfMonth( loData1Str );
            }
            else if (iFieldName.Contains(DBConstants.sqlVehInspectionStickerTypeStr))
            {
	            // look up list would be better.... what other types are there?
	            if (loData1Str.Contains("S2"))
	            {
		            loData1Str = "NON COMMERCIAL";
	            }
            }
            AddOrUpdateParsedFieldData( iFieldName, loData1Str, fParsedFieldNames_VehicleInfo, fParsedFieldValues_VehicleInfo );
        }

        int CheckVINNumberForLeading_I(int iOffset)
        {
            if (fBarCodeString[iOffset] == 'I')
            {
                return 1;
            }
            return 0;
        }

        //////////////////////////////////////////////////
        int Parse_2DBarCode_MA_VehicleInspectionSticker()  
        {
            // this one is just crammed together, copy the data out
            int MA_VehInspectionSticker_DataIdentifier_Offset = 0;
            int MA_VehInspectionSticker_DataIdentifier_Len = 2;

            ExtractAndAdd_VehicleInfoField(DBConstants.sqlVehInspectionStickerTypeStr, MA_VehInspectionSticker_DataIdentifier_Offset, MA_VehInspectionSticker_DataIdentifier_Len);

            int MA_VehInspectionSticker_BarCodeVersion_Offset = 2;
            int MA_VehInspectionSticker_BarCodeVersion_Len = 2;
            //ExtractAndAdd_VehicleInfoField( no barcode version field, MA_VehInspectionSticker_BarCodeVersion_Offset, MA_VehInspectionSticker_BarCodeVersion_Len );

            int MA_VehInspectionSticker_VIN_Offset = 4;
            int MA_VehInspectionSticker_VIN_Len = 17;
            int MA_VIN_LenOffset = CheckVINNumberForLeading_I(MA_VehInspectionSticker_VIN_Offset);
            if (MA_VIN_LenOffset == 1)
            {
                //The VIN has the leading 'I' letter, we need now to decide what we should do
                if (TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP, TTRegistry.regBARCODE_1D_VIN_REMOVE_LEADING_IMPORT_CHARACTER, TTRegistry.regBARCODE_1D_VIN_REMOVE_LEADING_IMPORT_CHARACTER_DEFAULT) == TTRegistry.VIN_STRIP_LEADING_I)
                {
                    MA_VehInspectionSticker_VIN_Offset = 5;  //We need to strip the leading 'I'
                }
                else
                {
                    MA_VehInspectionSticker_VIN_Len = 18;    //This is the case of VIN = 18 chars and leading 'I' is needed
                }
            }
            ExtractAndAdd_VehicleInfoField(DBConstants.sqlVehVINStr, MA_VehInspectionSticker_VIN_Offset, MA_VehInspectionSticker_VIN_Len);

            int MA_VehInspectionSticker_VehLicSt_Offset = 21 + MA_VIN_LenOffset;
            int MA_VehInspectionSticker_VehLicSt_Len = 2;
            ExtractAndAdd_VehicleInfoField(DBConstants.sqlVehLicStateStr, MA_VehInspectionSticker_VehLicSt_Offset, MA_VehInspectionSticker_VehLicSt_Len);

            int MA_VehInspectionSticker_VehLicType_Offset = 23 + MA_VIN_LenOffset;
            int MA_VehInspectionSticker_VehLicType_Len = 3;
            ExtractAndAdd_VehicleInfoField(DBConstants.sqlVehPlateTypeStr, MA_VehInspectionSticker_VehLicType_Offset, MA_VehInspectionSticker_VehLicType_Len);

            int MA_VehInspectionSticker_VehLicNo_Offset = 26 + MA_VIN_LenOffset;
            int MA_VehInspectionSticker_VehLicNo_Len = 10;
            ExtractAndAdd_VehicleInfoField(DBConstants.sqlVehLicNoStr, MA_VehInspectionSticker_VehLicNo_Offset, MA_VehInspectionSticker_VehLicNo_Len);

            int MA_VehInspectionSticker_VehDecalNo_Offset = 36 + MA_VIN_LenOffset;
            int MA_VehInspectionSticker_VehDecalNo_Len = 4;
            ExtractAndAdd_VehicleInfoField(DBConstants.sqlVehDecalNumberStr, MA_VehInspectionSticker_VehDecalNo_Offset, MA_VehInspectionSticker_VehDecalNo_Len);

            int MA_VehInspectionSticker_VehDecalNoSuffix_Offset = 40 + MA_VIN_LenOffset;
            int MA_VehInspectionSticker_VehDecalNoSuffix_Len = 2;
            ExtractAndAdd_VehicleInfoField(DBConstants.sqlVehDecalNumberSuffixStr, MA_VehInspectionSticker_VehDecalNoSuffix_Offset, MA_VehInspectionSticker_VehDecalNoSuffix_Len);

            int MA_VehInspectionSticker_VehLicExp_Offset = 42 + MA_VIN_LenOffset;
            int MA_VehInspectionSticker_VehLicExp_Len = 6;					// MMYYYY 
            ExtractAndAdd_VehicleInfoField(DBConstants.sqlVehLicExpDateStr, MA_VehInspectionSticker_VehLicExp_Offset, MA_VehInspectionSticker_VehLicExp_Len);

            int MA_VehInspectionSticker_VehYear_Offset = 48 + MA_VIN_LenOffset;
            int MA_VehInspectionSticker_VehYear_Len = 4;
            ExtractAndAdd_VehicleInfoField(DBConstants.sqlVehYearDateStr, MA_VehInspectionSticker_VehYear_Offset, MA_VehInspectionSticker_VehYear_Len);

            int MA_VehInspectionSticker_VehMake_Offset = 52 + MA_VIN_LenOffset;
            int MA_VehInspectionSticker_VehMake_Len = 17;
            ExtractAndAdd_VehicleInfoField(DBConstants.sqlVehMakeStr, MA_VehInspectionSticker_VehMake_Offset, MA_VehInspectionSticker_VehMake_Len);

            int MA_VehInspectionSticker_VehInspectionStickerExpDate_Offset = 69 + MA_VIN_LenOffset;
            int MA_VehInspectionSticker_VehInspectionStickerExpDate_Len = 6;				// MMYYYY  
            ExtractAndAdd_VehicleInfoField(DBConstants.sqlVehInspectionStickerExpDateStr, MA_VehInspectionSticker_VehInspectionStickerExpDate_Offset, MA_VehInspectionSticker_VehInspectionStickerExpDate_Len);

            int MA_VehInspectionSticker_VehInspectionStickerNumber_Offset = 75 + MA_VIN_LenOffset;
            int MA_VehInspectionSticker_VehInspectionStickerNumber_Len = 9;
            ExtractAndAdd_VehicleInfoField(DBConstants.sqlVehInspectionStickerNumberStr, MA_VehInspectionSticker_VehInspectionStickerNumber_Offset, MA_VehInspectionSticker_VehInspectionStickerNumber_Len);


            fDocumentClass = TPDF417DocumentClass.pdfVehicleDocument;
            fDisplaySummaryMode = TDisplayConfirmationMode.dispVehicleDataOnly;  

            fVehicleDataPresent = true; // only toggle ON, dont override values from other subfiles
            return (int)TParseDataResult.PARSE_DATA_GOOD;
        }


        /////////////////////////////////////////////////
        int Parse_2DBarCode_Header()
        {
            // ........extract get the AAMVA header info..............

            // these are single chars
            fAAMVA_BarHeader_DataElementSeparatorChar = fBarCodeString[ AAMVA_BarHeader_DataElementSeparatorOffset ];
            fAAMVA_BarHeader_RecordSeparatorChar = fBarCodeString[ AAMVA_BarHeader_RecordSeparatorOffset ];
            fAAMVA_BarHeader_SegmentTerminatorChar    = fBarCodeString[ AAMVA_BarHeader_SegmentTerminatorOffset ];

            // get the next group as null term strings
            // specs say: this will be "ANSI " for DL/ID, and "AAMVA" for documents
            fAAMVA_BarHeader_FileType = fBarCodeString.Substring(AAMVA_BarHeader_FileTypeOffset, AAMVA_BarHeader_FileTypeSize);

            fAAMVA_BarHeader_IssuerIDNumber = fBarCodeString.Substring(AAMVA_BarHeader_IssuerIDNumberOffset, AAMVA_BarHeader_IssuerIDNumberSize );

            fAAMVA_BarHeader_AAMVAVersionNumber = fBarCodeString.Substring(AAMVA_BarHeader_AAMVAVersionNumberOffset, AAMVA_BarHeader_AAMVAVersionNumberSize);

            // is this a DL/ID ?
            if ( fAAMVA_BarHeader_FileType.Contains("ANSI "))
            {
	            // we can also get the issuing jurisdiction
	            fAAMVA_BarHeader_JurisdictionVersionNumber = fBarCodeString.Substring(AAMVA_BarHeader_JurisdictionVersionNumberOffset, AAMVA_BarHeader_JurisdictionVersionNumberSize);
	            
	            // ID Card
	            fDocumentClass = TPDF417DocumentClass.pdfDriverLicOrIDCard;
            }else if( fAAMVA_BarHeader_FileType.Contains("AAMVA")){  // is it a veh document?
	            // in the spec, veh documents dont have issuing jurisdiction (why not??)
	            fAAMVA_BarHeader_JurisdictionVersionNumber = "";
	            // veh ID Card
	            fDocumentClass = TPDF417DocumentClass.pdfVehicleDocument;
            }else{
	            // don't know what it is, don't know how to parse it
	            // in the spec, veh documents dont have issuing jurisdiction (why not??)
	            fAAMVA_BarHeader_JurisdictionVersionNumber = "";
	            // veh ID Card
	            fDocumentClass = TPDF417DocumentClass.pdfUnknown;
            }

            // figure out where the subfile count is... this varies in ID versus VEH, plus states do not always follow the specs
            int loSubFileCountOffset = Parse_2DBarCode_Header_DetermineSubfileCountOffset();
            // look to fixup for special cases - might get a different document class from this
            Parse_2DBarCode_Header_FixUpSpecialCases();

            // extract the ASCII value
            String loSubfileCountStr = "";
            loSubfileCountStr = fBarCodeString.Substring(loSubFileCountOffset, AAMVA_BarHeader_SubfileCountSize);
            // convert to int
            fAAMVA_BarHeader_SubfileCount = Convert.ToInt32(loSubfileCountStr);

            // the first subfile header offset is immediately after the header
            int loFirstSubFileHeaderOffset = loSubFileCountOffset + AAMVA_BarHeader_SubfileCountSize;
            // calculated for each header
            int loOneSubFileHeaderOffset;

            // defense - bad counts or unknown document types are tossed
            if ((( fAAMVA_BarHeader_SubfileCount < 1 ) || ( fAAMVA_BarHeader_SubfileCount > MAX_SUBFILE_TYPES )) || ( fDocumentClass == TPDF417DocumentClass.pdfUnknown ))
            {
	            // should tell them why
	            return (int) TParseDataResult.PARSE_DATA_BAD;
            }

            // now extract each of the subfile headers that have been defined
            int loSubfileUnknownCount = 0;
            String loSubfileDataStr = "";
            for ( int loOneSubFileIdx = 0; loOneSubFileIdx < fAAMVA_BarHeader_SubfileCount; loOneSubFileIdx++ )
            {
                // calculate the offset for this header
	            loOneSubFileHeaderOffset = loFirstSubFileHeaderOffset + (loOneSubFileIdx * AAMVA_BarHeader_SubFileDesignatorSize);

	            // extract the subfile type
	            loSubfileDataStr = fBarCodeString.Substring((loOneSubFileHeaderOffset + AAMVA_BarHeader_SubFileTypeOffset), AAMVA_BarHeader_SubFileTypeSize);
	            
                // figure out what it is
	            if ( loSubfileDataStr.Contains(AAMVA_SubfileType_DriversLicID ))
	            {
		            fSubfileHeaders[ loOneSubFileIdx ].fSubfileType = TSubfileType.sbfDriversLicID;
	            }else if ( loSubfileDataStr.Contains(AAMVA_SubfileType_VehicleTitle )){
		            fSubfileHeaders[ loOneSubFileIdx ].fSubfileType = TSubfileType.sbfVehicleTitle;
	            }else if ( loSubfileDataStr.Contains(AAMVA_SubfileType_VehicleRegistration )){
		            fSubfileHeaders[ loOneSubFileIdx ].fSubfileType = TSubfileType.sbfVehicleRegistration;
	            }else if ( loSubfileDataStr.Contains(AAMVA_SubfileType_MotorCarrierCabCard )){
		            fSubfileHeaders[ loOneSubFileIdx ].fSubfileType = TSubfileType.sbfMotorCarrierCabCard;
	            }else if ( loSubfileDataStr.Contains(AAMVA_SubfileType_MotorCarrierRegistrant )){
		            fSubfileHeaders[ loOneSubFileIdx ].fSubfileType = TSubfileType.sbfMotorCarrierRegistrant;
	            }else if ( loSubfileDataStr.Contains(AAMVA_SubfileType_VehicleSafetyInspection )){
		            fSubfileHeaders[ loOneSubFileIdx ].fSubfileType = TSubfileType.sbfVehicleSafetyInspection;
	            }else if ( loSubfileDataStr.Contains(AAMVA_SubfileType_VehicleOwners )){
		            fSubfileHeaders[ loOneSubFileIdx ].fSubfileType = TSubfileType.sbfVehicleOwners;
	            }else if ( loSubfileDataStr.Contains(AAMVA_SubfileType_VehicleInfo )){
		            fSubfileHeaders[ loOneSubFileIdx ].fSubfileType = TSubfileType.sbfVehicleInfo;
	            }else if ( loSubfileDataStr.Contains(AAMVA_JurisdictionSubfileType_NY_Vehicle )){
	                ////////////// jurisdiction specific subfiles //////////////////////
		            fSubfileHeaders[ loOneSubFileIdx ].fSubfileType = TSubfileType.sbfNYVehicleInfo;
	            }else if ( loSubfileDataStr.Contains(AAMVA_JurisdictionSubfileType_NY_Registration )){
		            fSubfileHeaders[ loOneSubFileIdx ].fSubfileType = TSubfileType.sbfNYRegistration;
	            }else if ( loSubfileDataStr.Contains(AAMVA_JurisdictionSubfileType_NY_VersionControl )){
		            fSubfileHeaders[ loOneSubFileIdx ].fSubfileType = TSubfileType.sbfNYRegistration;
	            }else{
		            fSubfileHeaders[ loOneSubFileIdx ].fSubfileType = TSubfileType.sbfUnknown;
		            loSubfileUnknownCount++;
	            }

	            // get the sizes and offsets, translate into numeric
                loSubfileDataStr = fBarCodeString.Substring((loOneSubFileHeaderOffset + AAMVA_BarHeader_SubFileOffsetOffset) , AAMVA_BarHeader_SubFileOffsetSize );
	            
	            // convert to int
                fSubfileHeaders[ loOneSubFileIdx ].fSubfileOffset = Convert.ToInt32(loSubfileDataStr);

                loSubfileDataStr = fBarCodeString.Substring((loOneSubFileHeaderOffset + AAMVA_BarHeader_SubFileLengthOffset) , AAMVA_BarHeader_SubFileLengthSize );
	            
	            // convert to int
                fSubfileHeaders[ loOneSubFileIdx ].fSubfileLength = Convert.ToInt32( loSubfileDataStr);

	            // make adjustments to skip the subfile designator; this makes parsing simpler
	            fSubfileHeaders[ loOneSubFileIdx ].fSubfileOffset += 2;
	            fSubfileHeaders[ loOneSubFileIdx ].fSubfileLength -= 2;
            }

            // did we go through the subfiles and not find a single one we recognize?
            if ( loSubfileUnknownCount == fAAMVA_BarHeader_SubfileCount )
            {
	            // this is not a 2D barcode
	            return (int) TParseDataResult.PARSE_DATA_BAD;
            }

            return (int) TParseDataResult.PARSE_DATA_GOOD;
        }



        //////////////////////////////////////////////////
        int Parse_2DBarCode_Subfile_TD_Title()
        {
            //fVehicleDataPresent = true; only toggle ON, dont override values from other subfiles
            //fPersonDataPresent = true;
            return (int) TParseDataResult.PARSE_DATA_GOOD;
        }

        //////////////////////////////////////////////////
        int Parse_2DBarCode_Subfile_RG_Registration()
        {
            String loData1Str = "";  // local str used to extract the data
            String loStateStr;
            int loDataPos;

            loData1Str = ExtractValueForField(AAMVA_BarRG_ExpiryDate);   // CCYYMMDD
            AddParsedFieldNameAndDataToList_VehicleInfo(DBConstants.sqlVehLicExpDateStr, loData1Str);

            loData1Str = ExtractValueForField(AAMVA_BarRG_PlateNumber);
            AddParsedFieldNameAndDataToList_VehicleInfo(DBConstants.sqlVehLicNoStr, loData1Str);

            loData1Str = ExtractValueForField(AAMVA_BarRG_PlateClass);
            AddParsedFieldNameAndDataToList_VehicleInfo(DBConstants.sqlVehPlateTypeStr, loData1Str);


            // what state?
            loStateStr = fAAMVA_BarHeader_IssuerIDNumber;
            loDataPos = FindStateByValue(loStateStr );

            if ( loDataPos != -1 )
            {
	            // set the state
	            loStateStr = fSupportedStates[loDataPos].StateCode;
                AddOrUpdateParsedFieldData(DBConstants.sqlVehLicStateStr, loStateStr, fParsedFieldNames_VehicleInfo, fParsedFieldValues_VehicleInfo);
            }
  
            fVehicleDataPresent = true; // only toggle ON, dont override values from other subfiles
            //fPersonDataPresent = true;
            return  (int)TParseDataResult.PARSE_DATA_GOOD;
        }


        //////////////////////////////////////////////////
        int Parse_2DBarCode_Subfile_MC_MotorCabCarrierCard()
        {
            //fVehicleDataPresent = true; // only toggle ON, dont override values from other subfiles
            //fPersonDataPresent = true;
            return (int)TParseDataResult.PARSE_DATA_GOOD;
        }


        //////////////////////////////////////////////////
        int Parse_2DBarCode_Subfile_IR_MotorCarrierRegistrant()
        {
            //fVehicleDataPresent = true; // only toggle ON, dont override values from other subfiles
            //fPersonDataPresent = true;
            return (int)TParseDataResult.PARSE_DATA_GOOD;
        }

        //////////////////////////////////////////////////
        int Parse_2DBarCode_Subfile_VS_VehicleSafetyInspection()
        {
            //fVehicleDataPresent = true; // only toggle ON, dont override values from other subfiles
            //fPersonDataPresent = true;
            return (int)TParseDataResult.PARSE_DATA_GOOD;
        }

        //////////////////////////////////////////////////
        int Parse_2DBarCode_Subfile_OW_VehicleOwners()
        {
            //fVehicleDataPresent = true; // only toggle ON, dont override values from other subfiles
            //fPersonDataPresent = true;
            return (int)TParseDataResult.PARSE_DATA_GOOD;
        }


        //////////////////////////////////////////////////
        int Parse_2DBarCode_Subfile_VH_Vehicle()
        {
            String loData1Str = "";  // local str used to extract the data

            loData1Str =ExtractValueForField(AAMVA_BarVH_VIN);
            AddParsedFieldNameAndDataToList_VehicleInfo(DBConstants.sqlVehVINStr, loData1Str);

            loData1Str = ExtractValueForField(AAMVA_BarVH_VehicleMake);
            AddParsedFieldNameAndDataToList_VehicleInfo(DBConstants.sqlVehMakeStr, loData1Str);

            loData1Str = ExtractValueForField(AAMVA_BarVH_VehicleModel);
            AddParsedFieldNameAndDataToList_VehicleInfo(DBConstants.sqlVehModelStr, loData1Str);


            loData1Str = ExtractValueForField(AAMVA_BarVH_VehicleYear);             // CCYY
            AddParsedFieldNameAndDataToList_VehicleInfo(DBConstants.sqlVehYearDateStr, loData1Str);

            loData1Str =ExtractValueForField(AAMVA_BarVH_VehicleCylinders);
            AddParsedFieldNameAndDataToList_VehicleInfo( RONoFieldDefined, loData1Str );

            loData1Str = ExtractValueForField(AAMVA_BarVH_VehicleGrossWeight);
            AddParsedFieldNameAndDataToList_VehicleInfo( RONoFieldDefined, loData1Str );

            loData1Str = ExtractValueForField(AAMVA_BarVH_VehicleUnladenWeight);
            AddParsedFieldNameAndDataToList_VehicleInfo( RONoFieldDefined, loData1Str );

            fVehicleDataPresent = true; // only toggle ON, dont override values from other subfiles
            return (int)TParseDataResult.PARSE_DATA_GOOD;
        }

        //////////////////////////////////////////////////
        int Parse_2DBarCode_Subfile_NY_Vehicle()  
        {
            // we will replace standard values w/ NY specific ones
            // if the encoding doesn't place the jurisdiction specific ones last, 

            String loData1Str = "";  // local str used to extract the data

            loData1Str = ExtractValueForField(AAMVA_BarVH_NY_FourDigitYear);      // CCYY
            AddOrUpdateParsedFieldData( DBConstants.sqlVehYearDateStr, loData1Str, fParsedFieldNames_VehicleInfo, fParsedFieldValues_VehicleInfo );

            loData1Str = ExtractValueForField(AAMVA_BarVH_NY_VehicleMake);      
            AddOrUpdateParsedFieldData(DBConstants.sqlVehMakeStr, loData1Str, fParsedFieldNames_VehicleInfo, fParsedFieldValues_VehicleInfo );

            loData1Str = ExtractValueForField(AAMVA_BarVH_NY_VehicleBody);
            AddOrUpdateParsedFieldData(DBConstants.sqlVehBodyTypeStr, loData1Str, fParsedFieldNames_VehicleInfo, fParsedFieldValues_VehicleInfo);

            loData1Str = ExtractValueForField(AAMVA_BarVH_NY_VehicleFuel);      
            AddOrUpdateParsedFieldData( RONoFieldDefined, loData1Str, fParsedFieldNames_VehicleInfo, fParsedFieldValues_VehicleInfo );

            loData1Str = ExtractValueForField(AAMVA_BarVH_NY_VehicleColor);
            AddOrUpdateParsedFieldData(DBConstants.sqlVehColor1Str, loData1Str, fParsedFieldNames_VehicleInfo, fParsedFieldValues_VehicleInfo);

            fVehicleDataPresent = true; // only toggle ON, dont override values from other subfiles
            return (int)TParseDataResult.PARSE_DATA_GOOD;
        }


        //////////////////////////////////////////////////
        int Parse_2DBarCode_Subfile_NY_Registration()
        {
            //NYMA indicator
            //Three of name
            //#define AAMVA_BarRG_NY_NYMAIndicator "ZRA"
            //#define AAMVA_BarRG_NY_ThreeOfName "ZRB"

            //fVehicleDataPresent = true; // only toggle ON, dont override values from other subfiles
            //fPersonDataPresent = true;
            return (int)TParseDataResult.PARSE_DATA_GOOD;
        }

        //////////////////////////////////////////////////
        int Parse_2DBarCode_Subfile_NY_VersionControl()
        {
	        /*
            Start NY Defined version control	2
            Data element separator-Barcode version	4
            NY defined barcode version (year and month)	9
            Data element separator-End	1

            #define AAMVA_BarVC_NY_BarcodeVersion "ZZZ"
            */

            //fVehicleDataPresent = true; // only toggle ON, dont override values from other subfiles
            //fPersonDataPresent = true;
            return (int)TParseDataResult.PARSE_DATA_GOOD;
        }

        //////////////////////////////////////////////////
        /*
        *    DL sub file parsing
        */
        /////////////////////////////////////////////////
        int Parse_2DBarCode_Subfile_DL_DriversLicense( )   
        {
            String loData1Str = "";  // local str used to extract the data
            String loStateStr;
            int loPos = 0;
            int loLength = 0;
            int loAddrBytes = 0;  
            int loDataPos;

            // what state?
            loStateStr = fAAMVA_BarHeader_IssuerIDNumber;
            loDataPos = FindStateByValue(loStateStr);
 
            if ( loDataPos < 0 )
            {
                // not a code we recognize, try to get the state from the mailing address
                loData1Str = ExtractValueForField(AAMVA_BarDriverMailingJurisidctionCode);
	            if ( loStateStr == "" )
	            {
                    loData1Str = ExtractValueForField(AAMVA_BarDriverLicenseResidenceJurisdictionCode);
	            }
            }else{
	            // set the state
	            loStateStr = fSupportedStates[loDataPos].StateCode;
            }
  
            /*
            wchar_t loWMagString[ MAX_TRK_BYTES ];
            memset( loWMagString, 0, sizeof( loWMagString ) );
            mbstowcs( loWMagString, loStateStr, MAX_TRK_BYTES );
            MessageBox(glCETopWindow, (LPCTSTR)&loWMagString, TEXT("state"), MB_OK);
            */

            // set the state
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlDLStateStr, RONoFieldDefined, loStateStr);
            // and the susp address state isn't in the data, so we'll set it too
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspAddrStateStr, DBConstants.sqlTrafROAddrStateStr /*ROAddrStateFieldName*/, loStateStr);

            // get and save each of the field elements individually
            loData1Str = ExtractValueForField(AAMVA_BarDriverLicenseNumber);
            /*
            memset( loWMagString, 0, sizeof( loWMagString ) );
            mbstowcs( loWMagString, loData1Str, MAX_TRK_BYTES );
            MessageBox(glCETopWindow, (LPCTSTR)&loWMagString, TEXT("dl no"), MB_OK);
            */
            //concatenate with data at pos 37
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlDLNumberStr, RONoFieldDefined, loData1Str);

            // get the dl class
            loData1Str = ExtractValueForField(AAMVA_BarDriverLicenseClassCode);

            /*
            memset( loWMagString, 0, sizeof( loWMagString ) );
            mbstowcs( loWMagString, loData1Str, MAX_TRK_BYTES );
            MessageBox(glCETopWindow, (LPCTSTR)&loWMagString, TEXT("dl class"), MB_OK);
            MessageBox(glCETopWindow, TEXT("before add dlclass"), TEXT("city"), MB_OK);
            */
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlDLClassStr, RONoFieldDefined, loData1Str);

            //  MessageBox(glCETopWindow, TEXT("After add dlclass"), TEXT("city"), MB_OK);
            //get address
            loData1Str = ExtractValueForField(AAMVA_BarDriverMailingCity);
            //MessageBox(glCETopWindow, TEXT("After extract mailingcity"), TEXT("city"), MB_OK);
            if ( loData1Str == "" )
            {
                //MessageBox(glCETopWindow, TEXT("before res city"), TEXT("city"), MB_OK);
	            loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseResidenceCity);
            }
            /*
            MessageBox(glCETopWindow, TEXT("before add city"), TEXT("city"), MB_OK);
            memset( loWMagString, 0, sizeof( loWMagString ) );
            mbstowcs( loWMagString, loData1Str, MAX_TRK_BYTES );
            MessageBox(glCETopWindow, (LPCTSTR)&loWMagString, TEXT("city"), MB_OK);
            */
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspAddrCityStr, DBConstants.sqlTrafROAddrCityStr /*ROAddrCityFieldName*/, loData1Str);
  
            //get Name
            loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseName);
            // MessageBox(glCETopWindow, TEXT("got name"), TEXT("city"), MB_OK);
            // if the name is blank then it is stored in seperate fields
            if ( loData1Str == "" )
            { 
                loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseFirstName);
                AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspFirstNameStr, DBConstants.sqlTrafRONameFirstStr /*ROFirstNameFieldName*/, loData1Str);
                loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseMiddleName);

    	        // only include middle name if its not "NONE"
	            if ( loData1Str.Contains("NONE"))
	            {
                    AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspMiddleNameStr, DBConstants.sqlTrafRONameMiddleStr /*ROMidNameFieldName*/, loData1Str);
	            }

                loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseLastName);
                AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspLastNameStr, DBConstants.sqlTrafRONameLastStr /*ROLastNameFieldName*/, loData1Str); // suffix?
            }else{
                if (loStateStr.Contains("MN") || loStateStr.Contains("MD"))
	            {
                    ExtractAAMVA_Name( loData1Str, TNameStyle.name_FML );
	            }else{
                    ExtractAAMVA_Name( loData1Str, TNameStyle.name_LFM );
	            }
            }
  
            // MessageBox(glCETopWindow, TEXT("got to mailing"), TEXT("city"), MB_OK);
            loData1Str = ExtractValueForField( AAMVA_BarDriverMailingStreet1);
            // MessageBox(glCETopWindow, TEXT("got mailing"), TEXT("city"), MB_OK);

            if ( loData1Str == "" )
            {
                // MessageBox(glCETopWindow, TEXT("get res1"), TEXT("city"), MB_OK);
                loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseResidenceStreet1);
            }
  
            /*
            MessageBox(glCETopWindow, TEXT("got res1"), TEXT("city"), MB_OK);
            memset( loWMagString, 0, sizeof( loWMagString ) );
            mbstowcs( loWMagString, loData1Str, MAX_TRK_BYTES );
            MessageBox(glCETopWindow, (LPCTSTR)&loWMagString, TEXT("street"), MB_OK);
            */
            ExtractAAMVA_Address( loData1Str );

            /*ExtractValueForField( AAMVA_BarDriverLicenseMailingStreet2, loData1Str );
            if ( loData1Str[0] == 0 )
            {
	            ExtractValueForField( AAMVA_BarDriverLicenseResidenceStreet2, loData1Str );
            }

            AddParsedFieldNameAndDataToList_PersonName( SuspAddrStreetFieldName, ROAddrStreetFieldName, loData1Str );
            */

            loData1Str = ExtractValueForField( AAMVA_BarDriverMailingJurisidctionCode);
            if ( loData1Str == "" )
            {
	            loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseResidenceJurisdictionCode);
            }

            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspAddrStateStr, DBConstants.sqlTrafROAddrStateStr /*ROAddrStateFieldName*/, loData1Str);

            loData1Str = ExtractValueForField( AAMVA_BarDriverMailingPostalCode);
            if ( loData1Str == "" )
            {
	            loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseResidencePostalCode);
            }
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspAddrZipStr, DBConstants.sqlTrafROAddrZipStr /*ROAddrZipFieldName*/, loData1Str);

            loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseSex);
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspGenderStr, RONoFieldDefined, loData1Str);

            loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseHairColor);
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspHairColorStr, RONoFieldDefined, loData1Str);

            loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseEyeColor);
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspEyeColorStr, RONoFieldDefined, loData1Str);

            loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseHeight);
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspHeightStr, RONoFieldDefined, loData1Str);

            loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseWeight);
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlSuspWeightStr, RONoFieldDefined, loData1Str);

            loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseDOB);
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlDLBirthDateStr, RONoFieldDefined, loData1Str);

            loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseExpirationDate);
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlDLExpDateStr, RONoFieldDefined, loData1Str);

            loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseRestrictionCode);
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlDLRestrictionsStr, RONoFieldDefined, loData1Str);

            loData1Str = ExtractValueForField( AAMVA_BarDriverLicenseEndorsementCode);
            AddParsedFieldNameAndDataToList_PersonName(DBConstants.sqlDLEndorsementsStr, RONoFieldDefined, loData1Str);

            //MessageBox(glCETopWindow, TEXT( "CLEAN UP ZIPPS"), TEXT("city"), MB_OK);
            // strip extra digits from the zip as needed
            CleanUpZipCodes();

            // insert a dash into the height
            FormatHeight( '-' );

            //MessageBox(glCETopWindow, TEXT( "MODIFY DATES"), TEXT("city"), MB_OK);
            // fix up the birthdate and expiration dates
            ModifyBirthAndExpDates_AlmostAAMVAStyle();

            //MessageBox(glCETopWindow, TEXT( "CONVET GENDER"), TEXT("city"), MB_OK);
            // convert the gender if it's a number
            ConvertGenderFromNumericToAlpha();

            fPersonDataPresent = true; // only toggle ON, dont override values from other subfiles

            return (int) TParseDataResult.PARSE_DATA_GOOD;
        }


        //////////////////////////////////////////////////
        /*
        *    2D parsing. Primary Method
        */
        /////////////////////////////////////////////////
        int Parse_2DBarCode()
        {
            int loParseResult;
            // FIRST we have to handle some "special exceptions"
            if ( Parse_2DBarCode_Header_CheckForMassachusettsVehInspection() == 1 )
            {
	            // its a vehicle inspection sticker
	            return Parse_2DBarCode_MA_VehicleInspectionSticker();
            }

            // first we have to parse the header to determine the type and number of subfiles
            loParseResult = Parse_2DBarCode_Header();
            if ( loParseResult != (int) TParseDataResult.PARSE_DATA_GOOD ) 
            {
	            return loParseResult;
            }

            // now process each of the subfiles 
            for ( int loOneSubFileIdx = 0; loOneSubFileIdx < fAAMVA_BarHeader_SubfileCount; loOneSubFileIdx++ )
            {
	            // assume we'll be able to parse this one, set the global ptr to it
	            fCurrentParseSubfileInfo = fSubfileHeaders[ loOneSubFileIdx ];

	            // which one is it?
	            switch ( fSubfileHeaders[ loOneSubFileIdx ].fSubfileType )
	            {
		            case TSubfileType.sbfDriversLicID :
			        {
			            loParseResult = Parse_2DBarCode_Subfile_DL_DriversLicense();
			            break;
			        }

		            case TSubfileType.sbfVehicleTitle :
			        {
				        loParseResult = Parse_2DBarCode_Subfile_TD_Title();
				        break;
			        }
	
		            case TSubfileType.sbfVehicleRegistration :
			        {
				        loParseResult = Parse_2DBarCode_Subfile_RG_Registration();
				        break;
			        }
	
		            case TSubfileType.sbfMotorCarrierCabCard :
			        {
				        loParseResult = Parse_2DBarCode_Subfile_MC_MotorCabCarrierCard();
				        break;
			        }
	
		            case TSubfileType.sbfMotorCarrierRegistrant :
			        {
				        loParseResult = Parse_2DBarCode_Subfile_IR_MotorCarrierRegistrant();
				        break;
			        }
	
		            case TSubfileType.sbfVehicleSafetyInspection :
			        {
				        loParseResult = Parse_2DBarCode_Subfile_VS_VehicleSafetyInspection();
				        break;
			        }

		            case TSubfileType.sbfVehicleOwners :
			        {
				        loParseResult = Parse_2DBarCode_Subfile_OW_VehicleOwners();
				        break;
			        }
	
		            case TSubfileType.sbfVehicleInfo :
			        {
				        loParseResult = Parse_2DBarCode_Subfile_VH_Vehicle();
				        break;
			        }

	                /////// jurisdiction specific subfiles
		            case TSubfileType.sbfNYVehicleInfo :
			        {
				        loParseResult = Parse_2DBarCode_Subfile_NY_Vehicle();
				        break;
			        }
		            
                    case TSubfileType.sbfNYRegistration :
			        {
				        loParseResult = Parse_2DBarCode_Subfile_NY_Registration();
				        break;
			        }

		            case TSubfileType.sbfNYVersionControl :
			        {
				        loParseResult = Parse_2DBarCode_Subfile_NY_VersionControl();
				        break;
			        }

		            default :
		            {
			            // unknown or unhandled
			            // this doesn't return an error, because we want to use as much as we can, even if we encounter unknown subfile types
			            break;
		            }
	            }

	            // done parsing this one
	            //fCurrentParseSubfileInfo =  null;

	            // if we have a problem, exit immediately
	            if ( loParseResult != (int) TParseDataResult.PARSE_DATA_GOOD ) 
	            {
	                return loParseResult;
	            }
            }

            // OK, we've parsed all the subfiles, how will we display it?
            fDisplaySummaryMode = TDisplayConfirmationMode.dispVehicleDataOnly;  // assume veh mode

            // override cases
            if (( fVehicleDataPresent == false ) && (fPersonDataPresent == true ))
            {
	            // only person info
	            fDisplaySummaryMode = TDisplayConfirmationMode.dispPersonDataOnly;
            }else if (( fVehicleDataPresent == true ) && (fPersonDataPresent == true )){
	            // veh AND person info
	            fDisplaySummaryMode = TDisplayConfirmationMode.dispVehicleAndPersonData;
            }

            /////////////////////////////////////////////////////////////////////
            // debug - put some sample data in
            /*
            TBarCodeString loData1Str; 
            strcpy( loData1Str, "20090517" );   // CCYYMMDD
            AddParsedFieldNameAndDataToList_VehicleInfo( VehLicExp, loData1Str );
            strcpy( loData1Str, "ABC123" );  
            AddParsedFieldNameAndDataToList_VehicleInfo( VehLicNo, loData1Str );

            strcpy( loData1Str, "PAS" );   
            AddParsedFieldNameAndDataToList_VehicleInfo( VehLicType, loData1Str );
            strcpy( loData1Str, "NY" );   
            AddParsedFieldNameAndDataToList_VehicleInfo( VehLicSt, loData1Str );
  
            strcpy( loData1Str, "123ABC456DEF789GHI01" );   
            AddParsedFieldNameAndDataToList_VehicleInfo( VehVIN, loData1Str );

            strcpy( loData1Str, "GMC" );   
            AddParsedFieldNameAndDataToList_VehicleInfo( VehMake, loData1Str );

            strcpy( loData1Str, "1979" );   // CCYY
            AddParsedFieldNameAndDataToList_VehicleInfo( VehYear, loData1Str );

            strcpy( loData1Str, "8" );   
            AddParsedFieldNameAndDataToList_VehicleInfo( RONoFieldDefined, loData1Str );

            strcpy( loData1Str, "2400" );   
            AddParsedFieldNameAndDataToList_VehicleInfo( RONoFieldDefined, loData1Str );

            strcpy( loData1Str, "2950" );   
            AddParsedFieldNameAndDataToList_VehicleInfo( RONoFieldDefined, loData1Str );
            */

            // end sample data
            /////////////////////////////////////////////////////////////////////

            return (int)TParseDataResult.PARSE_DATA_GOOD;
        }

        //---------------------------------------------------
        //
        //  read the card id string
        //
        void DetermineBarType()
        {
            //  int loIdx;
            // reset
            fLastBarTypeRead = TBarCodeType.Bar_None;
            fDocumentClass = TPDF417DocumentClass.pdfUnknown;
            fDisplaySummaryMode = TDisplayConfirmationMode.dispUnknown;
            fVehicleDataPresent = false;
            fPersonDataPresent = false;

            // debug messages for the platforms that support it    
            Console.WriteLine("App: fBarCodeString: {0} {1} {2} {3} {4} {5} {6} {7}",
                (int)fBarCodeString[0],
                (int)fBarCodeString[1],
                (int)fBarCodeString[2],
                (int)fBarCodeString[3],
                (int)fBarCodeString[4],
                (int)fBarCodeString[5],
                (int)fBarCodeString[6],
                (int)fBarCodeString[7]
                );

            if ( (int)fBarCodeString[0] == AAMVA_ComplianceIndicator )		// magic char, easy determination
            {
                fLastBarTypeRead = TBarCodeType.Bar_2D;
                // debug messages for the platforms that support it
                Console.WriteLine("App: BarCodeType: 2D\n");
            }
            // 2012-03-30 ajw - not AAMVA compliance indicator, but look through the barcode data and check for characteristics of 2D data
            else 
            {
		        for ( int loBufIdx = 0; loBufIdx < fByteCount; loBufIdx++ )
		        {
			        // if this is a CR or an LF, then this must be a 2D barcode.
			        if ((fBarCodeString[loBufIdx] == 0xA) || (fBarCodeString[loBufIdx] == 0xD))
			        {
				        fLastBarTypeRead = TBarCodeType.Bar_2D;
				        // debug messages for the platforms that support it
				        Console.WriteLine("App: BarCodeType: 2D\n");
				        break;
			        }
		        }

		        // still not sure?
		        if ( fLastBarTypeRead == TBarCodeType.Bar_None ) 
		        {
			        // default to 1D
			        fLastBarTypeRead = TBarCodeType.Bar_1D;
			        // debug messages for the platforms that support it
			        Console.WriteLine("App: BarCodeType: 1D\n");
		        }
            }
            // 2012-03-30 ajw end
            return;
        }

        
        //---------------------------------------------------
        /*
        *    1D
        */
        int Parse_1DBarCode()
        {
            /*
            String loData1Str = "";  // local str used to extract the data
            String loData2Str;
            int loPos = 0;
            int loLength = 0;
            int loAddrBytes = 0;

            String loWMagString = "";
            loWMagString = fBarCodeString;

            String loKeyBoardString = "";
            int loKeyBoardStrLen;

            
            loKeyBoardStrLen = fBarCodeString.Length + 1;
            // convert into a widechar string
            loKeyBoardString = fBarCodeString;
            */
            return (int)TParseDataResult.PARSE_DATA_GOOD;
        }

        //--------------------------------------------------------
        TParseDataResult ParseAndValidateBarData( /*TTForm iForm*/ )
        {
            TParseDataResult loValidationResult = TParseDataResult.PARSE_DATA_BAD;

            // for now its good
            loValidationResult = TParseDataResult.PARSE_DATA_GOOD;

            //MessageBox(glCETopWindow, TEXT("before determinebar" ), TEXT("in while"), MB_OK);
            // determine which card type we have
            DetermineBarType();

            // make sure the field list is empty
            fParsedFieldNamesAsDriver.Clear();
            fParsedFieldNamesAsRegOwn.Clear();
            fParsedFieldValues_PersonName.Clear();

            fParsedFieldNames_VehicleInfo.Clear();
            fParsedFieldValues_VehicleInfo.Clear();


            // TEMP: for IPI demo, we are going to do a special stuffing method
            if (( loValidationResult == TParseDataResult.PARSE_DATA_GOOD ) && ( fLastBarTypeRead == TBarCodeType.Bar_1D ) )
            {
	            //FixMe: if ( uHALr_strpos( glIssueAp->fClientName, "DEMO" ) != -1)
	            {
	                String IPI_DEMO_TARGET_FIELD = "IPI_BADGE";
	                //#define IPI_DEMO_TARGET_FIELD "REMARK1"
		            char[] loIPIStr = fBarCodeString.ToArray();

		            // copy the data, performing a substution of ^ along the way
		            for ( int loPos = 0; loPos < fBarCodeString.Length; loPos++ )
		            {
			            // get source
			            char loIPIChar = fBarCodeString[ loPos ];

			            // substitute
			            if ( loIPIChar == '^')
			            {
				            loIPIChar = (char)0x0A;  //chLF; // memo field wants cfLF, not chCR
			            }
			            // place dest
			            loIPIStr[ loPos ] = loIPIChar;
		            }

		            // prepare and stuff it
                    AddParsedFieldNameAndDataToList_PersonName( IPI_DEMO_TARGET_FIELD, IPI_DEMO_TARGET_FIELD, loIPIStr.ToString() );
		            StuffBarCodeData_Prim( /*iForm,*/ fParsedFieldNamesAsDriver, fParsedFieldValues_PersonName );
		            return loValidationResult;
	            }
            }

            // call the appropriate parsing routine
            switch ( fLastBarTypeRead )
            {
                case TBarCodeType.Bar_1D :      // 1D
                {
                    loValidationResult = (TParseDataResult) Parse_1DBarCode();
                    return loValidationResult;
                }
                case TBarCodeType.Bar_2D :      // 2D
                {
		            loValidationResult = (TParseDataResult)Parse_2DBarCode();
		            // sometimes 1D gets mis-interpreted as 2D
		            if ( loValidationResult != TParseDataResult.PARSE_DATA_GOOD ) 
		            {
			            // try it as a 1D
			            fLastBarTypeRead = TBarCodeType.Bar_1D;
			            // debug messages for the platforms that support it
			            Console.WriteLine("App: BarCode 2D parse failed, trying as 1D");
			            
			            // give it another go as 1D
			            loValidationResult = (TParseDataResult)Parse_1DBarCode();
		            }

                    return loValidationResult;
                }
                default : // including card_None
                {
                    return TParseDataResult.PARSE_DATA_UNKNOWN;  // unknown or unsupported state
                }
            }

        }

        //--------------------------------------------------------
        /*
        *  convert mag stripe date string into a date string compatible with field
        */
        void FormatDateFieldData( String ioDateStr, String iDateMask, String iFieldMask )
        {
            String loDateStr = "";
            DateTime loDate = new DateTime();

            // convert from string to date
            if ( DateTimeManager.DateStringToOSDate( iDateMask, ioDateStr,  ref loDate ) == 0 )
            {
                // back to date string in the format of the field
                DateTimeManager.OsDateToDateString( loDate, iFieldMask, ref loDateStr );
            }else{
                // bad date, zap the string
                loDateStr = "";
            }

            // set the value for stuffing
            ioDateStr = loDateStr;
        }

        /*
        * Copy the field values from the barcode into the calling form fields
        */
        void StuffBarCodeData_Prim( List<String> iFieldNames, List<String> iFieldValues )
        {
            String loFieldValueStr;
            String loFieldName;
            
            // do em all TWICE - this breaks the hold of parent-child initial default values
            for ( int idxLoopTwice = 0; idxLoopTwice < 2; idxLoopTwice++ )
            {
                // loop through all fields pulled from magstripe and stuff em into dest form
                for ( int idx = 0; idx < iFieldNames.Count; idx++ )
                {
                    // get the field name
                    loFieldName = iFieldNames[idx];
                    // if we can find this field on the form
                    //FixME: if ( (loEdit = (TTEdit *)iForm->FindControlByName( loFieldName ) ) != 0 )
                    {
                        // get the value
                        //memset( loFieldValueStr, 0, sizeof( TTrackString ) );
                        loFieldValueStr = iFieldValues[idx];

                        // look for certain field types that need special handling
                        if (loFieldName.Contains(DBConstants.sqlDLExpDateStr))
                        {
                            // convert the field into a field compatible string
                            //FixME: FormatDateFieldData( loFieldValueStr, "YYYYMMDD", loEdit->GetEditMask() );
                        }
                        else if (loFieldName.Contains(DBConstants.sqlDLBirthDateStr))
                        {
                            // convert the field into a field compatible string
                            //FixME: FormatDateFieldData( loFieldValueStr, "YYYYMMDD", loEdit->GetEditMask() );
                        //FixME: }else if ( strcmp( loFieldName, VehLicExp ) == 0 ){
                           // convert the field into a field compatible string
                        //FixME:    FormatDateFieldData( loFieldValueStr, "YYYYMMDD", loEdit->GetEditMask() );
                        }



                        // make a copy of the field before we stuff it
                        //FixME String loSavedData = loEdit.TextBuf; //loEdit->GetEditBufferPtr();

                        // ok now  stuff the value in as a string
                        //FixME loEdit.TextBuf = loFieldValueStr; //SetEditBuffer( loFieldValueStr );

                        //FixME: loNotifyEvent = dneParentFieldExit;
                        // did the data change as a result the stuff?
                        //FixME: if ( loSavedData.Contains(loEdit->GetEditBufferPtr()))
                        {
                            // note that the data itself has changed
                            //FixME: loEdit->ProcessRestrictions( dneDataChanged, 0 );
                            //FixME: loNotifyEvent |= dneParentDataChanged;
                        }

                        // release the mem
                        //FREE( loSavedData );

                        // let the field do whatever initialization it needs to
                        //FixME:loEdit->NotifyDependents( loNotifyEvent );

                        // and suppress the "FirstFocus" event..
                        //FixME:loEdit->SetEditStatePreInitialized(1);
                    }
                }
            }

            // ok, repaint it and lets see
            //iForm->SetObscured( 0 );
            //FixME:iForm->PaintDescendants();
        }


        /////////////////
        void StuffBarCodeData_Person( /*TTForm iForm,*/ List<String>iFieldNames )
        {
	        StuffBarCodeData_Prim( /*iForm,*/ iFieldNames, fParsedFieldValues_PersonName );
        }
        
        /////////////////
        void StuffBarCodeData_Vehicle( /*TTForm iForm,*/ List<String>iFieldNames )
        {
	        StuffBarCodeData_Prim( /*iForm,*/ iFieldNames, fParsedFieldValues_VehicleInfo );
        }

        // constructor
        void TBarCodeParse()
        {
            fParsedFieldNamesAsDriver = new List<String>();
            fParsedFieldNamesAsRegOwn = new List<String>();
            fParsedFieldValues_PersonName = new List<String>();

            fParsedFieldNames_VehicleInfo = new List<String>();
            fParsedFieldValues_VehicleInfo = new List<String>();
        }

        
        //Returns current barsed data for user confirmation dialog
        public List<string> GetBarCodePreviewList()
        {
            return fDataElementList;
        }

        public TBarCodeResultData GetBarCodeResultDataFields()
        {
            TBarCodeResultData loResultData = new TBarCodeResultData();
            switch (this.fDisplaySummaryMode)
            {
                case TDisplayConfirmationMode.dispVehicleAndPersonData:
                case TDisplayConfirmationMode.dispPersonDataOnly:
                    loResultData.FieldsNamesList = this.fParsedFieldNamesAsDriver;
                    loResultData.FieldsValuesList = this.fParsedFieldValues_PersonName;
                    break;
                case TDisplayConfirmationMode.dispVehicleDataOnly:
                    loResultData.FieldsNamesList = this.fParsedFieldNames_VehicleInfo;
                    loResultData.FieldsValuesList = this.fParsedFieldValues_VehicleInfo;
                    break;
                default:
                    break;
            }
            return loResultData;
        }

        //Main entry point to the barser
        public TBarCodeType QueryUserChooseBarCodeDataDestination(String iRawBarCodeData)
        {
            string loTempStr;
            bool loTempFlag = false;
            
            if (fDataElementList != null) fDataElementList.Clear();
            CopyRawBarDataToParsingBuffer(iRawBarCodeData);
            TParseDataResult loValidateResult = ParseAndValidateBarData();

            // All done 1D stuffs the buffer
		    if (( loValidateResult == TParseDataResult.PARSE_DATA_GOOD ) && ( fLastBarTypeRead == TBarCodeType.Bar_1D ) )
		    {
                return TBarCodeType.Bar_1D;
		    }
	  
            switch (fDisplaySummaryMode)
            {
                case TDisplayConfirmationMode.dispVehicleAndPersonData:  // use person selection mode for both
                case TDisplayConfirmationMode.dispPersonDataOnly:
                    {
                        // build a display for confirmation. Line 1: Last Name, First Name, Middle Name
                        loTempStr = GetParsedFieldDataByDriverName(DBConstants.sqlSuspLastNameStr) + ", " +
                                          GetParsedFieldDataByDriverName(DBConstants.sqlSuspFirstNameStr) + " " +
                                          GetParsedFieldDataByDriverName(DBConstants.sqlSuspMiddleNameStr);
                        fDataElementList.Add(loTempStr);
                        // build a display for confirmation. Line 2: Street No. and Street                        
                        loTempStr = GetParsedFieldDataByDriverName(DBConstants.sqlSuspAddrStreetNoStr) + " " +
                                          GetParsedFieldDataByDriverName(DBConstants.sqlSuspAddrStreetStr);
                        fDataElementList.Add(loTempStr);

                        // Line 3: City, State ZIP                        
                        loTempStr = GetParsedFieldDataByDriverName(DBConstants.sqlSuspAddrCityStr) + ", " +
                                          GetParsedFieldDataByDriverName(DBConstants.sqlSuspAddrStateStr) + " " +
                                          GetParsedFieldDataByDriverName(DBConstants.sqlSuspAddrZipStr);
                        fDataElementList.Add(loTempStr);
                        // Line 4: DL Number, Exp
                        loTempStr = "DL#: " + GetParsedFieldDataByDriverName(DBConstants.sqlDLNumberStr) + ", Exp: " +
                                          GetParsedFieldDataByDriverName(DBConstants.sqlDLExpDateStr);
                        fDataElementList.Add(loTempStr);
                        // Line 5: DOB, Ht, Wt
                        loTempStr = "DOB#: " + GetParsedFieldDataByDriverName(DBConstants.sqlDLBirthDateStr) + ", Ht: " +
                                          GetParsedFieldDataByDriverName(DBConstants.sqlSuspHeightStr) + ", Wt: " +
                                          GetParsedFieldDataByDriverName(DBConstants.sqlSuspWeightStr);
                        fDataElementList.Add(loTempStr);                        
                        break;
                    }

                case TDisplayConfirmationMode.dispVehicleDataOnly:
                    {
                        loTempFlag = false;
                        // skip empty lines
                        if (GetParsedFieldDataByVehicle(DBConstants.sqlVehLicNoStr) != "")
                        {                            
                            loTempStr = "Plate: " + GetParsedFieldDataByVehicle(DBConstants.sqlVehLicStateStr) + " " +
                                              GetParsedFieldDataByVehicle(DBConstants.sqlVehLicNoStr);
                            fDataElementList.Add(loTempStr);
                        }

                        if (GetParsedFieldDataByVehicle(DBConstants.sqlVehYearDateStr) != "")
                        {
                            loTempStr = "Year: " + GetParsedFieldDataByVehicle(DBConstants.sqlVehYearDateStr);
                            fDataElementList.Add(loTempStr);
                        }

                        if (GetParsedFieldDataByVehicle(DBConstants.sqlVehMakeStr) != "")
                        {                            
                            loTempStr = "Make: " + GetParsedFieldDataByVehicle(DBConstants.sqlVehMakeStr);
                            fDataElementList.Add(loTempStr);
                        }

                        // color and body, or one of them: TO - build a more sophisticated display system,that shows ONLY what they have_
                        if ((GetParsedFieldDataByVehicle(DBConstants.sqlVehBodyTypeStr) != "") && (GetParsedFieldDataByVehicle(DBConstants.sqlVehColor1Str) != ""))
                        {
                            loTempStr = "Body: " + GetParsedFieldDataByVehicle(DBConstants.sqlVehBodyTypeStr);
                            fDataElementList.Add(loTempStr);
                            loTempStr = "Color: " + GetParsedFieldDataByVehicle(DBConstants.sqlVehColor1Str);
                            fDataElementList.Add(loTempStr);
                            loTempFlag = true;
                        }
                        else if (GetParsedFieldDataByVehicle(DBConstants.sqlVehBodyTypeStr) != "")
                        {                            
                            loTempStr = "Body: " + GetParsedFieldDataByVehicle(DBConstants.sqlVehBodyTypeStr);
                            fDataElementList.Add(loTempStr);
                            loTempFlag = true;
                        }
                        else if (GetParsedFieldDataByVehicle(DBConstants.sqlVehColor1Str) != "")
                        {                            
                            loTempStr = "Color: " + GetParsedFieldDataByVehicle(DBConstants.sqlVehColor1Str);
                            fDataElementList.Add(loTempStr);
                            loTempFlag = true;
                        }else{
                            loTempStr = "";
                        }
                        // no color/body?
                        if (!loTempFlag)
                        {
                            // maybe MODEL then
                            if (GetParsedFieldDataByVehicle(DBConstants.sqlVehModelStr) != "")
                            {                               
                                loTempStr = "Model: " + GetParsedFieldDataByVehicle(DBConstants.sqlVehModelStr);
                                fDataElementList.Add(loTempStr);
                            }
                        }

                        if (GetParsedFieldDataByVehicle(DBConstants.sqlVehVINStr) != "")
                        {                         
                            loTempStr = "VIN: " + GetParsedFieldDataByVehicle(DBConstants.sqlVehVINStr);
                            fDataElementList.Add(loTempStr);
                        }

                        break;
                    }

                default:
                    {                      
                        loTempStr = "Please Rescan";
                        fDataElementList.Add(loTempStr);
                        break;
                    }
            }
            return fLastBarTypeRead;
        }
    }
}
