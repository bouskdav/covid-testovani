using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.General
{
    public class TimeSlotsViewModel
    {
        public TimeSlotsViewModel()
        {
            this.DatesWithSlots = new List<DateAvailabilityOverview>();
        }

        public List<DateAvailabilityOverview> DatesWithSlots { get; set; }

        public List<string> Calc_StringDatesWithSlots => this.DatesWithSlots
            .Select(i => i.Date.ToString("yyyy-MM-dd"))
            .ToList();

        public List<string> Calc_StringDatesWithFreeSlots => this.DatesWithSlots
            .Where(i => i.FreeSpaceAvailable)
            .Select(i => i.Date.ToString("yyyy-MM-dd"))
            .ToList();
    }
}
