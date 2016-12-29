using System.Data;

namespace Duncan.AI.Droid.Common
{
	public class SearchMatchDTO
    {
        public string rowId = string.Empty;
        public string seqId = string.Empty;
        public string structName;

        // the tab delimited matching row
        public string RawResultRow = string.Empty;


        public DataRow rawRow = null;
        public SearchStructLogicAndroid fSearchStructLogic;


        public string parkingStatus = string.Empty;




        public string sqlIssueDateStr = string.Empty;
        public string sqlIssueTimeStr = string.Empty;
        public string sqlIssueNumberPrefixStr = string.Empty;
        public string sqlIssueNumberStr = string.Empty;
        public string sqlIssueNumberSuffixStr = string.Empty;
        public string ISSUENO_DISPLAY = string.Empty;

        public string sqlVehLicNoStr = string.Empty;
        public string sqlVehLicStateStr = string.Empty;
        public string sqlVehVINStr = string.Empty;
        public string sqlVehYearDateStr;
        public string VEHICLE_DISPLAY = string.Empty;

        public string sqlLocLotStr = string.Empty;
        public string sqlLocBlockStr = string.Empty;
        public string sqlLocDirectionStr = string.Empty;
        public string sqlLocStreetStr = string.Empty;
        public string sqlLocDescriptorStr = string.Empty;
        public string LOCATION_DISPLAY = string.Empty;
    }
}
