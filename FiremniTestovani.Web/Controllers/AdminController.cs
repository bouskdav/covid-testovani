using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ClosedXML.Excel;
using FiremniTestovani.Data.DbContext;
using FiremniTestovani.DataProviders;
using FiremniTestovani.Models;
using FiremniTestovani.Web.Extensions;
using FiremniTestovani.Web.Extensions.Base;
using FiremniTestovani.Web.Models.Admin;
using FiremniTestovani.Web.Models.General;
using FiremniTestovani.Web.Models.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wangkanai.Detection.Services;

namespace FiremniTestovani.Web.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        private BookingDataProvider _bookingDataProvider;
        private EnumsDataProvider _enumsDataProvider;

        public AdminController(
            IWebHostEnvironment webHostEnvironment, 
            IDetectionService detectionService, 
            ILoggerFactory logger, 
            IConfiguration configuration, 
            ApplicationDbContext db) 
            : base(webHostEnvironment, detectionService, logger, configuration, db)
        {
            this._bookingDataProvider = new BookingDataProvider(db, webHostEnvironment, configuration);
            this._enumsDataProvider = new EnumsDataProvider(db, webHostEnvironment, configuration);
        }

        public async Task<IActionResult> Index()
        {
            DateTime statsDate = DateTime.Today;

            DateStatsViewModel model = await GetStatsForDate(statsDate);

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> TimeSlots(bool? includePast)
        {
            bool futureOnly = true;
            if (includePast == true)
                futureOnly = false;

            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            TimeSlotsViewModel model = new TimeSlotsViewModel();

            model.DatesWithSlots = (await _bookingDataProvider.GetDatesWithSlots(source.SourceID, futureOnly: futureOnly))
                .Select(i => new DateAvailabilityOverview(i))
                .ToList();

            if (Request.IsAjaxRequest())
                return Json(model);

            return View(model);
        }

        public async Task<IActionResult> _TimeSlotsPartial(DateTime date)
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            TimeSlotsPartialViewModel model = new TimeSlotsPartialViewModel();

            model.Date = date;
            model.TimeSlots = await _bookingDataProvider.GetTimeSlotsWithOccupanciesForDate(source.SourceID, new DateTime(date.Year, date.Month, date.Day));
            model.DefaultTestDuration = source.TestDuration;

            return View(model);
        }

        public async Task<IActionResult> GenerateSlots(GenerateSlotsViewModel model)
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            // set result variable
            bool result = false;

            // capacity calculation from test duration
            double testDuration = source.TestDuration;
            int capacity = (int)Math.Floor(model.Duration / testDuration);

            // if capacity overrided, set it
            if (model.Capacity.HasValue)
                capacity = model.Capacity.Value;

            if (model.GenerationMethod == SlotsGenerationMethod.EndTime)
            {
                if (!model.To.HasValue)
                {
                    throw new NullReferenceException("Koncové datum není zadáno!");
                }

                model.From = model.From.SetDateTime(
                    year: model.Date.Year,
                    month: model.Date.Month,
                    day: model.Date.Day);

                model.To = model.To.SetDateTime(
                    year: model.Date.Year,
                    month: model.Date.Month,
                    day: model.Date.Day);
            }
            else if (model.GenerationMethod == SlotsGenerationMethod.Count)
            {
                model.From = model.From.SetDateTime(
                    year: model.Date.Year,
                    month: model.Date.Month,
                    day: model.Date.Day);

                model.To = model.From.AddMinutes(model.Duration * model.Count.Value);
            }
            else if (model.GenerationMethod == SlotsGenerationMethod.EmployeeCount)
            {
                model.From = model.From.SetDateTime(
                    year: model.Date.Year,
                    month: model.Date.Month,
                    day: model.Date.Day);

                // required number of slots
                double requiredNumberOfSlots = model.EmployeeCount.Value / (double)capacity;

                // set count
                model.Count = (int)Math.Ceiling(requiredNumberOfSlots);

                model.To = model.From.AddMinutes(model.Duration * model.Count.Value);
            }
            else
            {
                throw new NotImplementedException();
            }

            List<DateTime> starts = DateTimeExtensions
                .GetDateRange(
                    model.From,
                    model.To.Value,
                    TimeSpan.FromMinutes(model.Duration),
                    includeEndDate: false)
                .ToList();

            // create timeslots
            List<ApplicationTimeSlot> timeSlots = starts.Select(i => new ApplicationTimeSlot()
            {
                From = i,
                To = i.AddMinutes(model.Duration),
                SourceID = source.SourceID,
                Capacity = capacity,
                AllowSlotCancelation = model.AllowSlotCancelation,
                RequireSlotConfirmation = model.RequireSlotConfirmation
            }).ToList();

            result = await _bookingDataProvider.InsertTimeSlots(timeSlots);

            if (Request.IsAjaxRequest())
            {
                if (result)
                    return Json(model);
                else
                    return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }  

            return RedirectToAction("TimeSlots");
        }

        public async Task<IActionResult> Bookings()
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            TimeSlotsViewModel model = new TimeSlotsViewModel();

            model.DatesWithSlots = (await _bookingDataProvider.GetDatesWithSlots(source.SourceID, futureOnly: false))
                .Select(i => new DateAvailabilityOverview(i))
                .ToList();

            if (Request.IsAjaxRequest())
                return Json(model);

            return View(model);
        }

        public async Task<IActionResult> _BookingsPartial(DateTime date)
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            BookingsPartialViewModel model = new BookingsPartialViewModel();

            model.Date = date;
            model.Bookings = await _bookingDataProvider.GetBookingsForDate(source.SourceID, date);

            return View(model);
        }

        public async Task<IActionResult> _EditBookingPartial(int id)
        {
            if (id == 0)
                return new StatusCodeResult(400);

            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            ApplicationBooking booking = await _bookingDataProvider.GetBooking(source.SourceID, id);

            // view model
            BookingViewModel model = BookingViewModel.FromApplicationBooking(booking);

            model.AcceptTerms = true;

            // data for selectlists
            List<ApplicationCountry> enumCountries = await _enumsDataProvider.GetCountries();

            enumCountries = enumCountries
                .OrderByDescending(i => i.IsImportant)
                .ToList();

            ViewData["ApplicationCountries"] = new SelectList(enumCountries, "Value", "Text");

            // return view
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditBooking(BookingViewModel model)
        {
            if (!model.BookingID.HasValue)
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);

            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            ApplicationBooking booking = await _bookingDataProvider.GetBooking(source.SourceID, model.BookingID.Value);

            // update booking
            await UpdateBookingFromModel(booking, model);

            ApplicationBooking result = await _bookingDataProvider.UpdateBooking(booking);

            return Json(result);
        }

        private async ValueTask<DateStatsViewModel> GetStatsForDate(DateTime date)
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            DateStatsViewModel model = new DateStatsViewModel(date);

            // get data for date
            model.TimeSlots = await _bookingDataProvider.GetTimeSlotsForDate(source.SourceID, date);
            model.Bookings = await _bookingDataProvider.GetBookingsForDate(source.SourceID, date);

            return model;
        }

        private Task UpdateBookingFromModel(ApplicationBooking applicationBooking, BookingViewModel model)
        {
            applicationBooking.FirstName = model.FirstName;
            applicationBooking.LastName = model.LastName;
            applicationBooking.Name = model.FullName;

            applicationBooking.Email = model.Email;
            applicationBooking.Phone = model.Phone;

            applicationBooking.EmployeeID = model.PersonalNumber;

            Dictionary<string, object> additionalData = applicationBooking.GetAdditionalData();

            additionalData.AddOrUpdateValue("data_version", "v1");
            additionalData.AddOrUpdateValue("name", model.FirstName);
            additionalData.AddOrUpdateValue("surname", model.LastName);
            additionalData.AddOrUpdateValue("phone", model.Phone);
            additionalData.AddOrUpdateValue("date_of_birth", model.DateOfBirth.ToString("o"));
            additionalData.AddOrUpdateValue("personal_identification_number", model.PersonalIdentificationNumber);
            additionalData.AddOrUpdateValue("gender", model.Gender);
            additionalData.AddOrUpdateValue("nationality", model.Nationality);
            additionalData.AddOrUpdateValue("insurance", model.Insurance);
            additionalData.AddOrUpdateValue("personal_number", model.PersonalNumber);
            additionalData.AddOrUpdateValue("city", model.City);
            additionalData.AddOrUpdateValue("zip", model.ZIPCode);

            applicationBooking.SysAdditionalData = JsonConvert.SerializeObject(additionalData);

            return Task.CompletedTask;
        }
    }
}
