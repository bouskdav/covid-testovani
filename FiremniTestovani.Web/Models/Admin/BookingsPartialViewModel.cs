using FiremniTestovani.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.Admin
{
    public class BookingsPartialViewModel
    {
        public BookingsPartialViewModel()
        {
            this.Bookings = new List<ApplicationBooking>();
        }

        public DateTime Date { get; set; }

        public List<ApplicationBooking> Bookings { get; set; }
    }
}
