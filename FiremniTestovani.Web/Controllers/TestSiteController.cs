using FiremniTestovani.Data.DbContext;
using FiremniTestovani.DataProviders;
using FiremniTestovani.Models;
using FiremniTestovani.Web.Extensions.Base;
using FiremniTestovani.Web.Models.DateStats;
using FiremniTestovani.Web.Services.Abstraction;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wangkanai.Detection.Services;

namespace FiremniTestovani.Web.Controllers
{
    public class TestSiteController : BaseController
    {
        private BookingDataProvider _bookingDataProvider;
        private TestsDataProvider _testsDataProvider;

        private readonly INotificationService _notificationService;

        public TestSiteController(IWebHostEnvironment webHostEnvironment, IDetectionService detectionService, ILoggerFactory logger, IConfiguration configuration, ApplicationDbContext db, INotificationService notificationService) 
            : base(webHostEnvironment, detectionService, logger, configuration, db)
        {
            this._bookingDataProvider = new BookingDataProvider(db, webHostEnvironment, configuration);
            this._testsDataProvider = new TestsDataProvider(db, webHostEnvironment, configuration);
            this._notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            DateTime statsDate = DateTime.Today;

            DateStatsViewModel model = await GetStatsForDate(statsDate);

            return View(model);
        }

        public IActionResult Tests()
        {
            return View();
        }

        public async Task<IActionResult> _TestsPartial()
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            DateTime date = DateTime.Now;

            List<ApplicationBooking> bookings = await _bookingDataProvider.GetBookingsForDate(source.SourceID, date,
                excludeCanceled: true,
                excludeCompleted: true);

            bookings = bookings
                .Where(i => !i.FromActual.HasValue)
                .ToList();

            var result = new { 
                data = bookings
            };

            string data = JsonConvert.SerializeObject(result);

            return Content(data, "application/json");
        }

        public async Task<IActionResult> _TestsWaitingPartial()
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            DateTime date = DateTime.Now;

            List<ApplicationBooking> bookings = await _bookingDataProvider.GetBookingsForDate(source.SourceID, date,
                excludeCanceled: true,
                excludeCompleted: true);

            bookings = bookings
                .Where(i => i.FromActual.HasValue)
                .ToList();

            var result = new
            {
                data = bookings
            };

            string data = JsonConvert.SerializeObject(result);

            return Content(data, "application/json");
        }

        public async Task<IActionResult> _TestsCompletedPartial()
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            DateTime date = DateTime.Now;

            List<ApplicationBooking> bookings = await _bookingDataProvider.GetBookingsForDate(source.SourceID, date,
                excludeCanceled: true,
                excludeCompleted: false);

            bookings = bookings
                .Where(i => i.TestCompleted || i.AttendanceCanceled)
                .ToList();

            var result = new
            {
                data = bookings
            };

            string data = JsonConvert.SerializeObject(result);

            return Content(data, "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> _SubmitTestArrival(int bookingID)
        {
            ApplicationBooking result = await _bookingDataProvider.BookingArrived(bookingID);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> _SubmitTestResult(int bookingID, bool testResult)
        {
            ApplicationBooking result = await _testsDataProvider.SetTestsResult(bookingID, testResult);

            if (result != null)
            {
                // if positive
                if (testResult)
                {
                    await _notificationService.NotifyTestResultPositive(result, true, true);
                }
                else
                {
                    await _notificationService.NotifyTestResultNegative(result, true, false);
                }
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> _SubmitTestCancel(int bookingID)
        {
            ApplicationBooking result = await _bookingDataProvider.CancelBooking(bookingID);

            //if (result != null)
            //{
            //    // if positive
            //    if (testResult)
            //    {
            //        await _notificationService.NotifyTestResultPositive(result, false, false);
            //    }
            //    else
            //    {
            //        await _notificationService.NotifyTestResultNegative(result, false, false);
            //    }
            //}

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
    }
}
