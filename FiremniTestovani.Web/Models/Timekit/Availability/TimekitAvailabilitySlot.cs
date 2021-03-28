using FiremniTestovani.Web.Models.Timekit.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.Timekit.Availability
{
    public class TimekitAvailabilitySlot
    {
        public TimekitAvailabilitySlot()
        {
            this.resources = new List<TimekitResource>();
        }

        public DateTime start_date { get; set; }

        public string start
        {
            get => this.start_date.ToString("o");
            set => this.start_date = DateTime.Parse(value);
        }

        public DateTime end_date { get; set; }

        public string end
        {
            get => this.end_date.ToString("o");
            set => this.end_date = DateTime.Parse(value);
        }

        public List<TimekitResource> resources { get; set; }
    }
}
