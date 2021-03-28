using FiremniTestovani.Data.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiremniTestovani.Models
{
    public class ApplicationTimeSlotOverview
    {
        public ApplicationTimeSlotOverview(TimeSlotOccupancy timeSlotOccupancy)
        {
            this.SourceID = timeSlotOccupancy.SourceID;
            this.TimeSlotID = timeSlotOccupancy.TimeSlotID;
            this.From = timeSlotOccupancy.From;
            this.To = timeSlotOccupancy.To;
            this.Capacity = timeSlotOccupancy.Capacity;
            this.OccupiedSpaceCount = timeSlotOccupancy.OccupiedSpaceCount;
        }

        public int SourceID { get; set; }

        public int TimeSlotID { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int Capacity { get; set; }

        public int OccupiedSpaceCount { get; set; }
    }
}
