using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.AI
{
	public class CommonDTO
    {
        public string rowId = string.Empty;
        public string masterKey = string.Empty;

        public string seqId = string.Empty;
        public string inUseFlag = string.Empty;
        public string structName;
        public string parkingStatus = string.Empty;
        public Byte[] datFile;


        public DataRow RawDetailRowVoid = null;
        public DataRow RawDetailRowReissue = null;
        public DataRow RawDetailRowContinuance = null;

        public string sqlIssueDateStr = string.Empty;
        public string sqlIssueTimeStr = string.Empty;
        public string sqlIssueNumberPrefixStr = string.Empty;
        public string sqlIssueNumberStr = string.Empty;
        public string sqlIssueNumberSuffixStr = string.Empty;
        public string ISSUENO_DISPLAY = string.Empty;

        public Byte[] attachment;
        public string bookNumber = string.Empty;

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
