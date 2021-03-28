using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FiremniTestovani.Data.Tables
{
    public class TimeSlotBooking
    {
        [Key]
        public int TimeSlotBookingID { get; set; }

        public int TimeSlotID { get; set; }

        public int SourceID { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public string EmployeeID { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string SysAdditionalData { get; set; }

        public DateTime? FromExpected { get; set; }

        public DateTime? ToExpected { get; set; }

        public DateTime? FromActual { get; set; }

        public DateTime? ToActual { get; set; }

        public bool AttendanceCanceled { get; set; }

        public bool AttendanceConfirmed { get; set; }

        public bool TestCompleted { get; set; }

        public bool? TestResult { get; set; }

        public string SecurityCode { get; set; }

        public string ValidationCode { get; set; }

        public virtual Source R_Source { get; set; }

        public virtual TimeSlot R_TimeSlot { get; set; }
    }
}
