using FiremniTestovani.Web.Models.Timekit.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.Timekit.Booking
{
    public class TimekitBookingAttributes
    {
        public TimekitBookingAttributes()
        {
            this.customer = new Dictionary<string, object>();
        }

        public Dictionary<string, object> customer { get; set; }

        public TimekitEvent @event { get; set; }

        public TimekitEventInfo event_info { get; set; }

        public TimekitBookingSettings settings { get; set; }
    }
}
