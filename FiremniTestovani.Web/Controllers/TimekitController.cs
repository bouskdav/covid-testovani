using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FiremniTestovani.Data.DbContext;
using FiremniTestovani.DataProviders;
using FiremniTestovani.Models;
using FiremniTestovani.Web.Extensions;
using FiremniTestovani.Web.Extensions.Base;
using FiremniTestovani.Web.Models.Timekit.Availability;
using FiremniTestovani.Web.Models.Timekit.Booking;
using FiremniTestovani.Web.Services.Abstraction;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wangkanai.Detection.Services;

namespace FiremniTestovani.Web.Controllers
{
    public class TimekitController : BaseController
    {
        private readonly BookingDataProvider _bookingDataProvider;
        private readonly INotificationService _notificationService;
        private readonly ISourceSettingsService _settingsService;

        public TimekitController(
            IWebHostEnvironment webHostEnvironment, 
            IDetectionService detectionService, 
            ILoggerFactory logger, 
            IConfiguration configuration, 
            ApplicationDbContext db, 
            INotificationService notificationService,
            ISourceSettingsService settingsService) 
            : base(webHostEnvironment, detectionService, logger, configuration, db)
        {
            this._bookingDataProvider = new BookingDataProvider(db, webHostEnvironment, configuration);
            this._notificationService = notificationService;
            this._settingsService = settingsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// POST /api/v2/availability -> get availability
        /// </summary>
        [Route("api/v2/availability")]
        public async Task<IActionResult> GetAvailability([FromBody] TimekitAvailabilityRequest timekitAvailabilityRequest)
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            TimekitAvailabilityResponse timekitAvailabilityResponse = new TimekitAvailabilityResponse();

            var result = await _bookingDataProvider.GetFreeTimeSlotsForDate(source.SourceID, timekitAvailabilityRequest.date);

            foreach (var data in result)
            {
                if (data.To < DateTime.Now)
                    continue;

                timekitAvailabilityResponse.data.Add(new TimekitAvailabilitySlot()
                {
                    start_date = data.From,
                    end_date = data.To,
                    resources = timekitAvailabilityRequest.resources.Select(i => new Models.Timekit.Resources.TimekitResource()
                    {
                        id = data.ID.ToString(),
                        timezone = timekitAvailabilityRequest.output_timezone,
                        name = "Rezervace na testování" // TODO
                    }).ToList()
                });
            }

            return Json(timekitAvailabilityResponse);
        }

        /// <summary>
        /// POST /api/v2/bookings -> create booking
        /// </summary>
        [Route("api/v2/bookings")]
        public async Task<IActionResult> RequestBooking([FromBody] TimekitBookingRequest timekitBookingRequest)
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            TimekitBookingResponse timekitBookingResponse = new TimekitBookingResponse();

            // create new ApplicationBooking from TimekitBookingRequest
            ApplicationBooking applicationBooking = timekitBookingRequest.AsNewApplicationBooking();

            applicationBooking.SecurityCode = StringExtensions.RandomString(6);
            applicationBooking.ValidationCode = StringExtensions.RandomString(6);

            // create in database
            Tuple<ApplicationBooking, bool, string> bookingResult = await _bookingDataProvider.CreateBooking(applicationBooking);

            // result
            timekitBookingResponse.data = new TimekitBookingResult()
            {
                // created now
                created_at = DateTime.Now.ToString("o"),
                updated_at = DateTime.Now.ToString("o"),
                // id of new booking
                id = bookingResult.Item1.TimeSlotBookingID.ToString(),
                // respond with the same graph, disregard in app however
                graph = timekitBookingRequest.graph,
                // logic for appointment status
                state = bookingResult.Item2 ? "accepted" : "declined",
                //state = "tentative",
                // created successfully
                completed = bookingResult.Item2
            };

            // result based operations
            if (bookingResult.Item2)
            {
                // reponse status 201
                Response.StatusCode = (int)HttpStatusCode.Created;

                // send the notifications
                await _notificationService.NotifyBookingCreated(bookingResult.Item1);
            }
            else
            {
                // respond as bad request
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            // serialize response
            string content = JsonConvert.SerializeObject(timekitBookingResponse);

            // return the result
            return Content(content, "application/json");
        }
    }
}
