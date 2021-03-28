using FiremniTestovani.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.Admin
{
    public class TimeSlotsPartialViewModel
    {
        public TimeSlotsPartialViewModel()
        {
            this.TimeSlots = new List<ApplicationTimeSlot>();
        }

        public List<ApplicationTimeSlot> TimeSlots { get; set; }

        public DateTime Date { get; set; }

        public double DefaultTestDuration { get; set; }
    }
}
