using FiremniTestovani.Models;
using FiremniTestovani.Web.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.DateStats
{
    public class DateStatsViewModel
    {
        public DateStatsViewModel()
        {
            this.Bookings = new List<ApplicationBooking>();
            this.TimeSlots = new List<ApplicationTimeSlot>();

            this._initialized = true;

            this.RecalculateData();
        }

        public DateStatsViewModel(DateTime date)
            : this()
        {
            this.Date = date;
        }

        private bool _initialized;
        private List<ApplicationTimeSlot> _timeSlots;
        private List<ApplicationBooking> _bookings;

        public DateTime Date { get; set; }

        public List<ApplicationTimeSlot> TimeSlots
        {
            get => this._timeSlots;
            set
            {
                this._timeSlots = value;

                if (_initialized)
                    this.RecalculateData();
            }
        }

        public List<ApplicationBooking> Bookings
        {
            get => this._bookings;
            set
            {
                this._bookings = value;

                if (_initialized)
                    this.RecalculateData();
            }
        }

        #region business functions

        /// <summary>
        /// Total number of slots
        /// </summary>
        public int TotalSlotsCount { get; private set; }

        /// <summary>
        /// Total capacity of slots
        /// </summary>
        public int TotalSlotCapacity { get; private set; }

        /// <summary>
        /// Total number of bookings
        /// </summary>
        public int TotalUncanceledBookingCount { get; private set; }

        public double TotalOccupancyInPercent { get; private set; }

        public DateTime FirstSlotTime { get; private set; }

        public DateTime LastSlotTime { get; private set; }

        public List<ChartDateIntItem> SlotOverviewCapacity { get; set; }

        public string SlotOverviewCapacityString { get; set; }

        public List<ChartDateIntItem> SlotOverviewOccupied { get; set; }

        public string SlotOverviewOccupiedString { get; set; }

        #endregion

        public void RecalculateData()
        {
            this.TotalSlotsCount = this.TimeSlots?.Count() ?? 0;
            this.TotalSlotCapacity = this.TimeSlots?.Sum(i => i.Capacity) ?? 0;
            this.TotalUncanceledBookingCount = this.Bookings.Where(i => !i.AttendanceCanceled).Count();

            this.TotalOccupancyInPercent = this.TotalSlotCapacity != 0 ?
                this.TotalUncanceledBookingCount / (double)this.TotalSlotCapacity : 
                0;

            this.FirstSlotTime = this.TotalSlotsCount != 0 ?
                this.TimeSlots.OrderBy(i => i.From).FirstOrDefault().From :
                this.Date.SetDateTime(hour: 6, minute: 0);

            this.LastSlotTime = this.TotalSlotsCount != 0 ?
                this.TimeSlots.OrderBy(i => i.From).LastOrDefault().From :
                this.Date.SetDateTime(hour: 18, minute: 0);

            TimeSpan shortestSlotDuration = this.TotalSlotsCount != 0 ?
                this.TimeSlots.Min(i => i.To - i.From) :
                TimeSpan.FromHours(1);

            // reinitiate graph data
            this.SlotOverviewCapacity = new List<ChartDateIntItem>();
            this.SlotOverviewOccupied = new List<ChartDateIntItem>();

            // create timeslots overview chart
            if (this.TotalSlotsCount > 0)
            {
                foreach (DateTime time in DateTimeExtensions.GetDateRange(this.FirstSlotTime, this.LastSlotTime, shortestSlotDuration))
                {
                    var timeSlots = this.TimeSlots.Where(i => i.From <= time && i.To > time);
                    var timeSlotIDs = timeSlots.Select(i => i.ID).ToArray();

                    this.SlotOverviewCapacity.Add(new ChartDateIntItem()
                    {
                        Date = time,
                        Value = timeSlots.Sum(i => i.Capacity)
                    });

                    this.SlotOverviewOccupied.Add(new ChartDateIntItem()
                    { 
                        Date = time,
                        Value = this.Bookings.Where(i => timeSlotIDs.Contains(i.TimeSlotID) && !i.AttendanceCanceled).Count()
                    });
                }
            }

            this.SlotOverviewCapacityString = JsonConvert.SerializeObject(this.SlotOverviewCapacity);
            this.SlotOverviewOccupiedString = JsonConvert.SerializeObject(this.SlotOverviewOccupied);
        }
    }

    public class ChartDateIntItem
    {
        public DateTime Date { get; set; }

        public double Value { get; set; }

        public double X => this.Date.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

        public double Y => this.Value;
    }
}
