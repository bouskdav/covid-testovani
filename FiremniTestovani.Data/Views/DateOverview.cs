using System;
using System.Collections.Generic;
using System.Text;

namespace FiremniTestovani.Data.Views
{
    public class DateOverview
    {
        public int SourceID { get; set; }

        public DateTime Date { get; set; }

        public int Capacity { get; set; }

        public int OccupiedSpaceCount { get; set; }
    }
}
