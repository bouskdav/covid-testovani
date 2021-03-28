using FiremniTestovani.Data.Tables;
using FiremniTestovani.Data.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Models
{
    public class ApplicationTimeSlot
    {
        public ApplicationTimeSlot()
        {
        }

        public ApplicationTimeSlot(TimeSlot timeSlot)
        {
            this.ID = timeSlot.TimeSlotID;
            this.SourceID = timeSlot.SourceID;

            this.From = timeSlot.From;
            this.To = timeSlot.To;
            this.Capacity = timeSlot.Capacity;

            this.AllowSlotCancelation = timeSlot.AllowSlotCancelation;
            this.RequireSlotConfirmation = timeSlot.RequireSlotConfirmation;
        }

        public ApplicationTimeSlot(TimeSlotOccupancy timeSlotOccupancy)
        {
            this.ID = timeSlotOccupancy.TimeSlotID;
            this.SourceID = timeSlotOccupancy.SourceID;

            this.From = timeSlotOccupancy.From;
            this.To = timeSlotOccupancy.To;
            this.Capacity = timeSlotOccupancy.Capacity;
            this.Occupancy = timeSlotOccupancy.OccupiedSpaceCount;
        }

        public int ID { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int Capacity { get; set; }

        public int? Occupancy { get; set; }

        public int SourceID { get; set; }

        public bool? AllowSlotCancelation { get; set; }

        public bool? RequireSlotConfirmation { get; set; }
    }
}
