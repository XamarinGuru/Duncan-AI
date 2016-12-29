using System.Data;

namespace Duncan.AI.Droid.Common
{
	public class ANPRMatchDTO
    {
        public string sqlVehLicNoStr = string.Empty;
        public string sqlVehLicStateStr = string.Empty;

        public string sqlConfidenceStr = string.Empty;

        public double confidenceAsDouble;

        public int matches_template;
        public int plate_index;

        public string region = string.Empty;
        public int region_confidence;

        public double processing_time_ms;
        public int requested_topn;
    }


}
