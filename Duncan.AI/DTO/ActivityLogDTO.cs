using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.AI
{
	public struct ActivityLogDTO
    {
		public string startDate;
		public string startTime;
		public string endDate;
		public string endTime;
		//public string rowId;
		public string officerId;
		public string officername;
		public string startLatitude;
		public string startLongitude;
        public string endLatitude;
        public string endLongitude;
        public string primaryActivityName;
        public string primaryActivityCount;
        public string secondaryActivityName;
        public string secondaryActivityCount;
    }
}
