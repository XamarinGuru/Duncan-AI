namespace Duncan.AI
{
	public class ParkingDTO
    {
		public string DBRowId;
		public string aiTicketId;

        public string Status;

        public string sqlIssueDateStr = string.Empty;
        public string sqlIssueTimeStr = string.Empty;

        public string sqlIssueNumberPrefixStr = string.Empty;
        public string sqlIssueNumberStr = string.Empty;
        public string sqlIssueNumberSuffixStr = string.Empty;
        public string ISSUENO_DISPLAY = string.Empty;


        public string sqlVehLicNoStr = string.Empty;
        public string sqlVehLicStateStr = string.Empty;
        public string sqlVehVINStr = string.Empty;
        public string VEHICLE_DISPLAY = string.Empty;

        public string sqlVehPlateTypeStr;
        public string sqlVehLicExpDateStr;
        public string sqlVehMakeStr;
        public string sqlVehYearDateStr;

        public string sqlLocLotStr = string.Empty;
        public string sqlLocBlockStr = string.Empty;
        public string sqlLocDirectionStr = string.Empty;
        public string sqlLocStreetStr = string.Empty;
        public string sqlLocDescriptorStr = string.Empty;
        public string LOCATION_DISPLAY = string.Empty;

		public string sqlIssueOfficerIDStr;
        public string sqlIssueOfficerNameStr;

		public string LocMeter;

		public string VioSelect;
		public string VioCode;
		public string VioDesc;
		public string VioFee;
		public string VioFine;
    }
}
