using System;
using System.Collections.Generic;
using System.Text;

namespace FiremniTestovani.Models
{
    public class DateWithCapacity
    {
        public DateTime Date { get; set; }

        public int TotalCapacityCount { get; set; }

        public int UsedCapacityCount { get; set; }
    }
}
