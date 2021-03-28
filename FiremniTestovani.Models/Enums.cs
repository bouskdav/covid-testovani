using System;
using System.Collections.Generic;
using System.Text;

namespace FiremniTestovani.Models
{
    public class Enums
    {
        public enum ApplicationBookingState
        {
            Booked = 0,
            Confirmed = 1,
            Arrived = 2,
            WaitingForResults = 3,
            Completed = 4,
            Canceled = 5
        }
    }
}
