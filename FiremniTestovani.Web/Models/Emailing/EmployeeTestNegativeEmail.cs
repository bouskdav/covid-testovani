using FiremniTestovani.Models;
using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.Emailing
{
    public class EmployeeTestNegativeEmail : Email
    {
        public EmployeeTestNegativeEmail()
        {
        }

        public EmployeeTestNegativeEmail(ApplicationBooking booking)
        {
            this.Booking = booking;
        }

        public ApplicationBooking Booking { get; set; }
    }
}
