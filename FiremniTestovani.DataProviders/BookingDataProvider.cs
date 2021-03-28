using FiremniTestovani.Data.DbContext;
using FiremniTestovani.Data.Tables;
using FiremniTestovani.Data.Views;
using FiremniTestovani.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FiremniTestovani.DataProviders
{
    public class BookingDataProvider
    {
        public BookingDataProvider(ApplicationDbContext db, IWebHostEnvironment environment, IConfiguration configuration)
        {
            this._db = db;
            this._environment = environment;
            this._configuration = configuration;
        }

        private ApplicationDbContext _db;
        private IWebHostEnvironment _environment;
        private IConfiguration _configuration;

        public async ValueTask<ApplicationBooking> GetBooking(int sourceID, int id)
        {
            var dbBooking = await _db.TimeSlotBookings
                .SingleOrDefaultAsync(i => i.SourceID == sourceID && i.TimeSlotBookingID == id);

            if (dbBooking == null)
                return null;

            return new ApplicationBooking(dbBooking);
        }

        public async ValueTask<ApplicationBooking> GetBookingAsNoTracking(int sourceID, int id)
        {
            var dbBooking = await _db.TimeSlotBookings
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.SourceID == sourceID && i.TimeSlotBookingID == id);

            if (dbBooking == null)
                return null;

            return new ApplicationBooking(dbBooking);
        }

        public async ValueTask<List<DateOverview>> GetDatesWithSlots(int sourceID, bool futureOnly)
        {
            var dbDateOverviews = _db.DateOverviews
                .Where(i => i.SourceID == sourceID);

            if (futureOnly)
            {
                dbDateOverviews = dbDateOverviews.Where(i => i.Date >= DateTime.Today);
            }

            return await dbDateOverviews
                .OrderBy(i => i.Date)
                .ToListAsync();
        }

        public async ValueTask<List<ApplicationTimeSlot>> GetTimeSlotsForDate(int sourceID, DateTime date)
        {
            var dbSlots = await _db.TimeSlots
                .Where(i => 
                    i.SourceID == sourceID &&
                    i.From.Date == date.Date)
                .ToListAsync();

            return dbSlots
                .Select(i => new ApplicationTimeSlot(i))
                .ToList();
        }

        public async ValueTask<List<ApplicationTimeSlot>> GetTimeSlotsWithOccupanciesForDate(int sourceID, DateTime date)
        {
            var dbSlots = await _db.TimeSlotOccupancies
                .Where(i =>
                    i.SourceID == sourceID &&
                    i.From.Date == date.Date)
                .ToListAsync();

            return dbSlots
                .Select(i => new ApplicationTimeSlot(i))
                .ToList();
        }

        public async ValueTask<List<ApplicationTimeSlot>> GetFreeTimeSlotsForDate(int sourceID, DateTime date)
        {
            var dbSlots = await _db.TimeSlotOccupancies
                .Where(i => 
                    i.SourceID == sourceID &&
                    i.From.Date == date.Date &&
                    (i.Capacity - i.OccupiedSpaceCount) > 0)
                .ToListAsync();

            return dbSlots
                .Select(i => new ApplicationTimeSlot(i))
                .ToList();
        }

        public async ValueTask<bool> InsertTimeSlots(IEnumerable<ApplicationTimeSlot> applicationTimeSlots)
        {
            List<TimeSlot> timeSlots = applicationTimeSlots.Select(i => new TimeSlot()
            {
                From = i.From,
                To = i.To,
                Date = i.From.Date,
                Capacity = i.Capacity,
                SourceID = i.SourceID,
                AllowSlotCancelation = i.AllowSlotCancelation,
                RequireSlotConfirmation = i.RequireSlotConfirmation
            }).ToList();

            try
            {
                await _db.BulkInsertAsync<TimeSlot>(timeSlots);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ApplicationBooking> BookingArrived(int bookingID)
        {
            var dbBooking = await _db.TimeSlotBookings
                .SingleOrDefaultAsync(i => i.TimeSlotBookingID == bookingID);

            DateTime now = DateTime.Now;

            if (dbBooking == null)
                return null;

            dbBooking.TestCompleted = false;
            dbBooking.AttendanceCanceled = false;
            dbBooking.TestResult = null;
            dbBooking.AttendanceConfirmed = true;
            dbBooking.FromActual = now;

            _db.Entry(dbBooking).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return new ApplicationBooking(dbBooking);
        }

        public async Task<ApplicationBooking> CancelBooking(int bookingID)
        {
            var dbBooking = await _db.TimeSlotBookings
                .SingleOrDefaultAsync(i => i.TimeSlotBookingID == bookingID);

            DateTime now = DateTime.Now;

            if (dbBooking == null)
                return null;

            dbBooking.TestCompleted = false;
            dbBooking.AttendanceCanceled = true;
            dbBooking.TestResult = null;

            _db.Entry(dbBooking).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return new ApplicationBooking(dbBooking);
        }

        public async ValueTask<Tuple<ApplicationBooking, bool, string>> CreateBooking(ApplicationBooking applicationBooking)
        {
            // - do check if can be done
            var availability = _db.TimeSlotOccupancies.SingleOrDefault(i => i.TimeSlotID == applicationBooking.TimeSlotID);

            // - if slot not available available
            if ((availability.Capacity - availability.OccupiedSpaceCount) <= 0)
            {
                return new Tuple<ApplicationBooking, bool, string>(
                    applicationBooking,
                    false,
                    "Na daný termín již nejsou volná místa."
                );
            }

            TimeSlotBooking booking = applicationBooking.AsNewTimeSlotBooking();

            // add missing data from timeslot
            var timeSlot = await _db.TimeSlots.FindAsync(booking.TimeSlotID);

            booking.SourceID = timeSlot.SourceID;

            // add to database
            _db.TimeSlotBookings.Add(booking);
            await _db.SaveChangesAsync();

            // detach entity to reload it
            _db.Entry(booking).State = EntityState.Detached;

            // query for booking again
            booking = await _db.TimeSlotBookings.FindAsync(booking.TimeSlotBookingID);

            return new Tuple<ApplicationBooking, bool, string>(
                new ApplicationBooking(booking),
                true,
                "OK"
            );
        }

        public async ValueTask<ApplicationBooking> UpdateBooking(ApplicationBooking applicationBooking)
        {
            // find booking in database
            TimeSlotBooking booking = await _db.TimeSlotBookings
                .SingleOrDefaultAsync(i => i.TimeSlotBookingID == applicationBooking.TimeSlotBookingID);

            var tempBooking = applicationBooking.AsFullTimeSlotBooking();

            foreach (PropertyInfo property in typeof(TimeSlotBooking).GetProperties().Where(p => p.CanWrite))
            {
                property.SetValue(booking, property.GetValue(tempBooking, null), null);
            }

            // add to database
            _db.Entry(booking).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return new ApplicationBooking(booking);
        }

        public async ValueTask<DateTime?> GetLastTestDateForEmployee(string personalNumber, int sourceID)
        {
            // get all employee bookings
            var dbEmployeeBookings = _db.TimeSlotBookings
                .Where(i =>
                    i.SourceID == sourceID &&
                    i.EmployeeID == personalNumber &&
                    !i.AttendanceCanceled);

            // get latest
            var lastBooking = await dbEmployeeBookings
                .OrderByDescending(i => i.FromExpected)
                .FirstOrDefaultAsync();

            // return latest from or null
            return lastBooking?.FromExpected;
        }

        public async ValueTask<TimeSlotBooking> GetLastTestForEmployee(string personalNumber, int sourceID, string lastName = null)
        {
            // get all employee bookings
            var dbEmployeeBookings = _db.TimeSlotBookings
                .Where(i =>
                    i.SourceID == sourceID &&
                    i.EmployeeID == personalNumber &&
                    !i.AttendanceCanceled);

            // if lastName is present, select according to last name
            if (!String.IsNullOrEmpty(lastName))
            {
                dbEmployeeBookings = dbEmployeeBookings
                    .Where(i => i.LastName == lastName);
            }

            // get latest
            var lastBooking = await dbEmployeeBookings
                .OrderByDescending(i => i.FromExpected)
                .FirstOrDefaultAsync();

            // return latest from or null
            return lastBooking;
        }

        public async ValueTask<List<ApplicationBooking>> GetLastTestsForEmployee(string personalNumber, int sourceID, int count, string lastName = null)
        {
            // get all employee bookings
            var dbEmployeeBookings = _db.TimeSlotBookings
                .Where(i =>
                    i.SourceID == sourceID &&
                    i.EmployeeID == personalNumber);

            // if lastName is present, select according to last name
            if (!String.IsNullOrEmpty(lastName))
            {
                dbEmployeeBookings = dbEmployeeBookings
                    .Where(i => i.LastName == lastName);
            }

            // get latest
            var lastBookings = await dbEmployeeBookings
                .OrderByDescending(i => i.FromExpected)
                .Take(count)
                .ToListAsync();

            // return latest from or null
            return lastBookings
                .Select(i => new ApplicationBooking(i))
                .ToList();
        }

        public async ValueTask<List<ApplicationBooking>> GetBookingsForDate(int sourceID, DateTime date, bool excludeCanceled = false, bool excludeCompleted = false)
        {
            var dbBookings = _db.TimeSlotBookings
                .Where(i =>
                    i.SourceID == sourceID &&
                    i.R_TimeSlot.Date == date.Date);

            if (excludeCanceled)
            {
                dbBookings = dbBookings
                    .Where(i => !i.AttendanceCanceled);
            }

            if (excludeCompleted)
            {
                dbBookings = dbBookings
                    .Where(i => !i.TestCompleted);
            }

            var bookings = await dbBookings
                .OrderBy(i => i.FromExpected)
                .ToListAsync();

            return bookings
                .Select(i => new ApplicationBooking(i))
                .ToList();
        }
    }
}
