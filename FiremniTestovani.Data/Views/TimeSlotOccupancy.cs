using System;
using System.Collections.Generic;
using System.Text;

namespace FiremniTestovani.Data.Views
{
    public class TimeSlotOccupancy
    {
        public int SourceID { get; set; }

        public int TimeSlotID { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int Capacity { get; set; }

        public int OccupiedSpaceCount { get; set; }
    }
}
