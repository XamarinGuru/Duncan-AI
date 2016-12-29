using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.AI
{
    interface ITicket
    {
		NewDataSet GetParkingSequence (string iUnitSerialNumber, ref string errorMsg, string serialNumber);

		Task UpdateBookLastIssued (string bookNumber, string seqId);
    }
}
