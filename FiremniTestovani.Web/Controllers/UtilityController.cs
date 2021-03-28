using ClosedXML.Excel;
using FiremniTestovani.Data.DbContext;
using FiremniTestovani.Data.Tables;
using FiremniTestovani.Models;
using FiremniTestovani.Web.Extensions.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wangkanai.Detection.Services;

namespace FiremniTestovani.Web.Controllers
{
    public class UtilityController : BaseController
    {
        private readonly IWebHostEnvironment _whe;

        public UtilityController(IWebHostEnvironment webHostEnvironment, IDetectionService detectionService, ILoggerFactory logger, IConfiguration configuration, ApplicationDbContext db) 
            : base(webHostEnvironment, detectionService, logger, configuration, db)
        {
            this._whe = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        public IActionResult Import()
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            string filePath = String.Empty;

            // scan for files
            var files = new DirectoryInfo(Path.Combine(_whe.WebRootPath, "upload"))
                .EnumerateFiles("*.xlsx");

            var latestFile = files
                .OrderByDescending(i => i.CreationTime)
                .FirstOrDefault();

            if (latestFile == null)
                return new NotFoundResult();

            var wb = new XLWorkbook(latestFile.FullName);
            var ws = wb.Worksheets.FirstOrDefault();

            // First possible address of table:
            var firstPossibleAddress = ws.FirstCellUsed().Address;
            // Last possible address of table:
            var lastPossibleAddress = ws.LastCellUsed().Address;

            // Get a range with the remainder of the worksheet data (the range used)
            var bookingsRange = ws.Range(firstPossibleAddress, lastPossibleAddress).RangeUsed();

            // Treat the range as a table (to be able to use the column names)
            var bookingsTable = bookingsRange.AsTable();

            List<ApplicationTimeSlot> timeSlots = new List<ApplicationTimeSlot>();
            List<ApplicationBooking> bookings = new List<ApplicationBooking>();

            // Get the list bookings
            bookings = bookingsTable.DataRange.Rows()
              .Select(row => new ApplicationBooking()
              {
                  FromExpected = row.Field("Čas začátku").GetDateTime(),
                  FromActual = row.Field("Čas začátku").GetDateTime(),
                  ToExpected = row.Field("Čas konce").GetDateTime(),
                  ToActual = row.Field("Čas konce").GetDateTime(),
                  AttendanceCanceled = false,
                  AttendanceConfirmed = true,
                  SourceID = source.SourceID,
                  TestCompleted = false,
                  FirstName = row.Field("Jméno / First name").GetString(),
                  LastName = row.Field("Příjmení / Surname").GetString(),
                  Phone = row.Field("Tel. kontakt  / Tel. number").GetString(),
                  Email = null,
                  EmployeeID = row.Field("Osobní číslo zaměstnance / Personal number of the employee").GetString(),
                  SysAdditionalData = JsonConvert.SerializeObject(new {
                      name = row.Field("Jméno / First name").GetString(),
                      surname = row.Field("Příjmení / Surname").GetString(),
                      timezone = "UTC",
                      phone = row.Field("Tel. kontakt  / Tel. number").GetString(),
                      date_of_birth = DateTime.TryParse(row.Field("Datum narození / Birthdate (format: den/měsíc/rok)").GetString(), out _) ?
                          DateTime.Parse(row.Field("Datum narození / Birthdate (format: den/měsíc/rok)").GetString()) : 
                          (DateTime?)null,
                      personal_identification_number = row.Field("Rodné číslo / Personal identification number").GetString(),
                      gender = "",
                      nationality = row.Field("Státní příslušnost / Nationality").GetString(),
                      insurance = row.Field("Zdravotní pojištovna / Health insurance").GetString(),
                      personal_number = row.Field("Osobní číslo zaměstnance / Personal number of the employee").GetString(),
                      city = row.Field("Město / City").GetString(),
                      zip = row.Field("PSČ / ZIP Code").GetString(),
                      terms = true
                  })
              })
              .ToList();

            // now get all timeranges in import
            var bookingTimeSlots = bookings
                .GroupBy(i => new
                {
                    From = i.FromExpected.Value,
                    To = i.ToExpected.Value
                });

            // min and max date
            DateTime minDate = bookingTimeSlots
                .Min(i => i.Key.From);
            DateTime maxDate = bookingTimeSlots
                .Max(i => i.Key.From);

            // select existing records from database
            var dbTimeSlots = _db.TimeSlots
                .Where(i => 
                    i.SourceID == source.SourceID &&
                    i.From >= minDate && 
                    i.From <= maxDate)
                .ToList();

            List<TimeSlot> timeslotsToAdd = new List<TimeSlot>();

            // loop through slots and add missing
            foreach (var item in bookingTimeSlots)
            {
                if (dbTimeSlots.Any(i => i.From == item.Key.From))
                    continue;

                timeslotsToAdd.Add(new TimeSlot() { 
                    SourceID = source.SourceID,
                    Date = item.Key.From.Date,
                    Capacity = item.Count(),
                    From = item.Key.From,
                    To = item.Key.To
                });
            }

            // if any timeslots to add, bulkinsert
            if (timeslotsToAdd.Count > 0)
            {
                _db.TimeSlots.BulkInsert(timeslotsToAdd);

                // select existing records from database again
                dbTimeSlots = _db.TimeSlots
                    .Where(i =>
                        i.SourceID == source.SourceID &&
                        i.From >= minDate &&
                        i.From <= maxDate)
                    .ToList();
            }

            // do job on the data
            foreach (var item in bookings)
            {
                // full name
                item.Name = $"{item.FirstName} {item.LastName}";

                // assign timeslot id
                item.TimeSlotID = dbTimeSlots.First(i => i.From == item.FromExpected).TimeSlotID;
            }

            // bulk insert bookings
            List<TimeSlotBooking> bookingsToInsert = bookings
                .Select(i => i.AsNewTimeSlotBooking())
                .ToList();

            _db.TimeSlotBookings.BulkInsert(bookingsToInsert);

            return View();
        }
    }
}
