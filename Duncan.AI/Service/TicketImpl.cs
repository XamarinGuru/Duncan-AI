using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuncanWebServicesClient;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Duncan.AI
{
    class TicketImpl : ITicket
    {
		public NewDataSet GetParkingSequence (string iUnitSerialNumber, ref string errorMsg, string serialNumber)
		{
			NewDataSet newDataSet = null;
			TicketService ticketService = new TicketService();
			DataSet dataSet = ticketService.GetSeqFile (iUnitSerialNumber,ref errorMsg, serialNumber);
			if (dataSet != null) {
				newDataSet = DeSerializeObject(dataSet.GetXml());
			}
			return newDataSet;
		}

		public Task UpdateBookLastIssued (string bookNumber, string seqId)
		{
			return Task.Factory.StartNew (() => { 
				try
				{
					string errorMsg = null;
					TicketService ticketService = new TicketService();
					ticketService.UpdateBookLastIssued (bookNumber, seqId, ref errorMsg);
				}
				catch(Exception e)
				{
					//TODO Need to come up with better exception handling, something like saving to SQLite or Toast msg
					Console.WriteLine("Exception source: {0}", e.Source);
				}
			});
		}

		//Helper method to parse xml response
		public static NewDataSet DeSerializeObject(string xml)
		{
			var serializer = new XmlSerializer(typeof(NewDataSet));
			using (var reader = new StringReader(xml))
			{
				var t = (NewDataSet)serializer.Deserialize(reader);
				reader.Close();
				return t;
			}
		}
    }
}
