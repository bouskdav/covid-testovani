using FiremniTestovani.Web.Models.Timekit.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.Timekit.Booking
{
    public class TimekitBookingResult
    {
        public bool completed { get; set; }

        public string created_at { get; set; }

        public string graph { get; set; }

        public string id { get; set; }

        public string state { get; set; }

        public string updated_at { get; set; }

        public TimekitResource user { get; set; }

        public TimekitBookingAttributes attributes { get; set; }
    }
}
