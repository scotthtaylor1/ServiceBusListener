using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusListener
{
    public class ScramRemoveReplaceMessage
    {
        public Guid ClientId { get; set; }
        public Guid BaseAccountId { get; set; }
        public DateTime MonitorDate { get; set; }
        public int ServiceTypeLevel { get; set; }
        public int ServiceOptionLevel { get; set; }
        public bool Added { get; set; }
        public bool Removed { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string OldServiceTypeLevel { get; set; }
        public string NewServiceTypeLevel { get; set; }
        public string CurrentServiceTypeLevel { get; set; }
        public string BillableStateLevel { get; set; }
        public string Status { get; set; }
    }
}
