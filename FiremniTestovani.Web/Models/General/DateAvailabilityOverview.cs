using FiremniTestovani.Data.Views;
using FiremniTestovani.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.General
{
    public class DateAvailabilityOverview
    {
        public DateAvailabilityOverview(DateWithCapacity dateWithCapacity)
        {
            this.Date = dateWithCapacity.Date;
            this.TotalCapacityCount = dateWithCapacity.TotalCapacityCount;
            this.UsedCapacityCount = dateWithCapacity.UsedCapacityCount;
            this.FreeCapacityCount = this.TotalCapacityCount - this.UsedCapacityCount;
        }

        public DateAvailabilityOverview(DateOverview dateOverview)
        {
            this.Date = dateOverview.Date;
            this.TotalCapacityCount = dateOverview.Capacity;
            this.UsedCapacityCount = dateOverview.OccupiedSpaceCount;
            this.FreeCapacityCount = this.TotalCapacityCount - this.UsedCapacityCount;
        }

        public DateTime Date { get; set; }

        public int TotalCapacityCount { get; set; }

        public int UsedCapacityCount { get; set; }

        public int FreeCapacityCount { get; set; }

        public bool FreeSpaceAvailable => this.FreeCapacityCount > 0;
    }
}