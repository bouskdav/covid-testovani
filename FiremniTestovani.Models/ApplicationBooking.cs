using FiremniTestovani.Data.Tables;
using FiremniTestovani.Models.Abstraction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static FiremniTestovani.Models.Enums;

namespace FiremniTestovani.Models
{
    public class ApplicationBooking
    {
        public ApplicationBooking() { }

        public ApplicationBooking(TimeSlotBooking timeSlotBooking)
        {
            this.TimeSlotBookingID = timeSlotBooking.TimeSlotBookingID;
            this.TimeSlotID = timeSlotBooking.TimeSlotID;
            this.SourceID = timeSlotBooking.SourceID;

            this.Name = timeSlotBooking.Name;
            this.FirstName = timeSlotBooking.FirstName;
            this.LastName = timeSlotBooking.LastName;
            this.EmployeeID = timeSlotBooking.EmployeeID;
            this.Email = timeSlotBooking.Email;
            this.Phone = timeSlotBooking.Phone;
            this.SysAdditionalData = timeSlotBooking.SysAdditionalData;

            this.FromExpected = timeSlotBooking.FromExpected;
            this.ToExpected = timeSlotBooking.ToExpected;
            this.FromActual = timeSlotBooking.FromActual;
            this.ToActual = timeSlotBooking.ToActual;

            this.AttendanceCanceled = timeSlotBooking.AttendanceCanceled;
            this.AttendanceConfirmed = timeSlotBooking.AttendanceConfirmed;
            this.TestCompleted = timeSlotBooking.TestCompleted;
            this.TestResult = timeSlotBooking.TestResult;

            this.SecurityCode = timeSlotBooking.SecurityCode;
            this.ValidationCode = timeSlotBooking.ValidationCode;
        }

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

        public Dictionary<string, object> GetAdditionalData() => 
            !String.IsNullOrEmpty(this.SysAdditionalData) ? 
            JsonConvert.DeserializeObject<Dictionary<string, object>>(this.SysAdditionalData) : 
            null;

        public ApplicationBookingState BookingState
        {
            get
            {
                if (this.AttendanceCanceled)
                    return ApplicationBookingState.Canceled;

                if (this.TestCompleted)
                    return ApplicationBookingState.Completed;

                if (this.FromActual.HasValue)
                {
                    if (this.ToActual.HasValue)
                        return ApplicationBookingState.WaitingForResults;

                    return ApplicationBookingState.Arrived;
                }

                return ApplicationBookingState.Booked;
            }
        }

        /// <summary>
        /// Create new timeslot booking
        /// </summary>
        /// <returns></returns>
        public TimeSlotBooking AsNewTimeSlotBooking()
        {
            TimeSlotBooking timeSlotBooking = new TimeSlotBooking()
            {
                TimeSlotID = this.TimeSlotID,
                SourceID = this.SourceID,

                Name = this.Name,
                FirstName = this.FirstName,
                LastName = this.LastName,
                EmployeeID = this.EmployeeID,
                Email = this.Email,
                Phone = this.Phone,
                SysAdditionalData = this.SysAdditionalData,

                FromExpected = this.FromExpected,
                ToExpected = this.ToExpected,
                FromActual = this.FromActual,
                ToActual = this.ToActual,

                AttendanceCanceled = this.AttendanceCanceled,
                AttendanceConfirmed = this.AttendanceConfirmed,
                TestCompleted = this.TestCompleted,
                TestResult = this.TestResult,

                SecurityCode = this.SecurityCode,
                ValidationCode = this.ValidationCode
            };

            return timeSlotBooking;
        }

        public TimeSlotBooking AsFullTimeSlotBooking()
        {
            TimeSlotBooking timeSlotBooking = this.AsNewTimeSlotBooking();

            timeSlotBooking.TimeSlotBookingID = this.TimeSlotBookingID;

            return timeSlotBooking;
        }
    }
}
