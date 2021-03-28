using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FiremniTestovani.Data.Tables
{
    public class TimeSlot
    {
        [Key]
        public int TimeSlotID { get; set; }

        public int SourceID { get; set; }

        public DateTime Date { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int Capacity { get; set; }

        public string Note { get; set; }

        public bool Disabled { get; set; }

        public int? TestDuration { get; set; }

        public bool? AllowSlotCancelation { get; set; }
        
        public bool? RequireSlotConfirmation { get; set; }

        [ForeignKey("SourceID")]
        public virtual Source R_Source { get; set; }

        public virtual ICollection<TimeSlotBooking> C_Bookings { get; set; }
    }
}
