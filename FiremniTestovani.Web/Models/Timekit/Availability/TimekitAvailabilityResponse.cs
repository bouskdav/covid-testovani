using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.Timekit.Availability
{
    public class TimekitAvailabilityResponse
    {
        public TimekitAvailabilityResponse()
        {
            this.data = new List<TimekitAvailabilitySlot>();
        }

        public List<TimekitAvailabilitySlot> data { get; set; }
    }
}
