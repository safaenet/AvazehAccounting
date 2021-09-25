using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehApiClient.Models
{
    public class ChequeEventModel
    {
        public int ChequeId { get; set; }
        public string EventDate { get; set; }
        public ChequeEventTypes EventType { get; set; }
        public string EventText { get; set; }
        public string EventTypeString
        {
            get => EventType.ToString();
            set { if (Enum.IsDefined(typeof(ChequeEventTypes), value)) EventType = Enum.Parse<ChequeEventTypes>(value); }
        }
        public int EventTypeValue
        {
            get => (int)EventType;
            set { if (Enum.IsDefined(typeof(ChequeEventTypes), value)) EventType = (ChequeEventTypes)value; }
        }
    }
}