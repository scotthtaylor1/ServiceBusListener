using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusListener
{
    public class MonitorDayMessage
    {
		public Guid ClientId { get; set; }
		public int MonitoringTypeId { get; set; }
		public int? CustomActivityId { get; set; }
		public string EventId { get; set; }

		public override string ToString()
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(this);
		}

	}
}
