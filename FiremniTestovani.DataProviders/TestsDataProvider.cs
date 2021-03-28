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
using System.Text;
using System.Threading.Tasks;

namespace FiremniTestovani.DataProviders
{
    public class TestsDataProvider
    {
        public TestsDataProvider(ApplicationDbContext db, IWebHostEnvironment environment, IConfiguration configuration)
        {
            this._db = db;
            this._environment = environment;
            this._configuration = configuration;
        }

        private ApplicationDbContext _db;
        private IWebHostEnvironment _environment;
        private IConfiguration _configuration;

        public async ValueTask<ApplicationBooking> SetTestsResult(int bookingID, bool result)
        {
            var dbBooking = await _db.TimeSlotBookings
                .SingleOrDefaultAsync(i => i.TimeSlotBookingID == bookingID);

            DateTime now = DateTime.Now;

            if (dbBooking == null)
                return null;

            if (!dbBooking.FromActual.HasValue)
                dbBooking.FromActual = now.AddMinutes(-5);

            dbBooking.AttendanceCanceled = false;
            dbBooking.AttendanceConfirmed = true;

            dbBooking.ToActual = now;

            dbBooking.TestCompleted = true;
            dbBooking.TestResult = result;

            _db.Entry(dbBooking).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return new ApplicationBooking(dbBooking);
        }
    }
}
