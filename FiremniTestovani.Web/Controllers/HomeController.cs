using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FiremniTestovani.Web.Models;
using FiremniTestovani.Web.Extensions.Base;
using Microsoft.AspNetCore.Hosting;
using Wangkanai.Detection.Services;
using Microsoft.Extensions.Configuration;
using FiremniTestovani.Data.DbContext;
using FiremniTestovani.DataProviders;
using FiremniTestovani.Web.Models.General;
using FiremniTestovani.Web.Models.Home;
using System.Net;
using System.Threading;
using FiremniTestovani.Models;
using FiremniTestovani.Web.Extensions;
using Newtonsoft.Json;
using FiremniTestovani.Data.Enums;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using FiremniTestovani.Web.Services.Abstraction;

namespace FiremniTestovani.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IStringLocalizer<HomeController> _stringLocalizer;
        private readonly ISourceSettingsService _settingsService;
        //private readonly Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine _viewEngine;

        private BookingDataProvider _bookingDataProvider;
        private EnumsDataProvider _enumsDataProvider;

        public HomeController(
            IWebHostEnvironment webHostEnvironment, 
            IDetectionService detectionService, 
            ILoggerFactory logger, 
            IConfiguration configuration, 
            ApplicationDbContext db, 
            IStringLocalizer<HomeController> stringLocalizer,
            ISourceSettingsService settingsService) 
            : base(webHostEnvironment, detectionService, logger, configuration, db)
        {
            this._stringLocalizer = stringLocalizer;

            this._bookingDataProvider = new BookingDataProvider(db, webHostEnvironment, configuration);
            this._enumsDataProvider = new EnumsDataProvider(db, webHostEnvironment, configuration);

            this._settingsService = settingsService;
        }

        public IActionResult Index()
        {
            //string test = _stringLocalizer["HelloMessage"].Value;

            return View();
        }

        [HttpGet]
        public IActionResult PersonalData()
        {
            PersonalDataViewModel model = new PersonalDataViewModel();

            model.DisplayCardImage = _settingsService.GetSettingsForCurrentSource<bool>("DisplayCardImage", false, false);
            model.DisplayCardNotification = _settingsService.GetSettingsForCurrentSource<bool>("DisplayCardNotification", false, false);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PersonalData(PersonalDataViewModel personalDataViewModel)
        {
            // Check if employee exists, if not, prevent access
            // TODO
            bool employeeHasAccess = await UserHasAccessToBooking(
                source: 1, 
                personalNumber: personalDataViewModel.PersonalNumber, 
                firstName: personalDataViewModel.FirstName, 
                lastName: personalDataViewModel.LastName);

            if (!employeeHasAccess)
            {
                return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }

            // give value to tempdata that user has passed the check
            TempData["BookingPersonalCheckPassed"] = true;

            // UX fix - sleep one second
            Thread.Sleep(1000);

            // create response
            PersonalDataResponse response = new PersonalDataResponse();

            response.URL = Url.Action("Booking", "Home", new { 
                personalNumber = personalDataViewModel.PersonalNumber, 
                firstName = personalDataViewModel.FirstName,
                lastName = personalDataViewModel.LastName
            }, Request.Scheme);

            // return data
            return Json(response);
        }

        /// <summary>
        /// Booking page
        /// </summary>
        /// <param name="personalNumber"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Booking(string personalNumber, string firstName, string lastName)
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            // try tempdata if user passed the check
            bool? employeeHasAccess = (bool?)TempData["BookingPersonalCheckPassed"];

            // if nothing present, check again
            if (!employeeHasAccess.HasValue)
            {
                employeeHasAccess = await UserHasAccessToBooking(
                    source: 1,
                    personalNumber: personalNumber,
                    firstName: firstName,
                    lastName: lastName);
            }

            // if check failed, return forbidden
            if (employeeHasAccess == false)
            {
                return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }

            // if personal number null, return back to personal data
            if (String.IsNullOrEmpty(personalNumber))
            {
                return RedirectToAction("PersonalData");
            }

            // view model
            BookingViewModel model = new BookingViewModel();

            // fill data from query
            model.PersonalNumber = personalNumber;
            model.FirstName = firstName;
            model.LastName = lastName;

            model.DisplayTestHistory = _settingsService.GetSettingsForCurrentSource<bool>("DisplayTestHistory", false, false);
            model.DisplayTestResults = _settingsService.GetSettingsForCurrentSource<bool>("DisplayTestResults", false, false);

            // get last test entity
            Data.Tables.TimeSlotBooking lastTest;

            if (_settingsService.GetSettingsForCurrentSource<bool>("SearchEmployeeBasedOnSurname", false, false))
                lastTest = await _bookingDataProvider.GetLastTestForEmployee(personalNumber, source.SourceID, lastName);
            else
                lastTest = await _bookingDataProvider.GetLastTestForEmployee(personalNumber, source.SourceID);

            model.LastTest = lastTest != null ?
                new ApplicationBooking(lastTest) :
                null;

            // add earliest possible date
            //DateTime? lastTestDate = await _bookingDataProvider.GetLastTestDateForEmployee(personalNumber, source.SourceID);
            DateTime? lastTestDate = lastTest?.FromExpected;

            // if there is last test
            if (lastTestDate.HasValue)
            {
                // set date according to logic
                switch (source.EarliestNextPossibleTest)
                {
                    case Data.Enums.EarliestNextPossibleTest.Unlimited:
                        model.EarliestDateForTest = DateTime.Now;

                        break;
                    case Data.Enums.EarliestNextPossibleTest.FixedDays:
                        model.EarliestDateForTest = lastTestDate.Value
                            .AddDays(source.EarliestNextPossibleTestNumericValue ?? 1);

                        break;
                    case Data.Enums.EarliestNextPossibleTest.FloatingWeeks:
                        model.EarliestDateForTest = lastTestDate.Value
                            .StartOfWeek(DayOfWeek.Monday)
                            .AddDays(7 * (source.EarliestNextPossibleTestNumericValue ?? 1));

                        break;
                    case Data.Enums.EarliestNextPossibleTest.FloatingWeeksAndThreeDays:
                        DateTime lastTestPlusThreeDays = lastTestDate.Value.AddDays(3);
                        DateTime startOfNextWeek = lastTestDate.Value
                            .StartOfWeek(DayOfWeek.Monday)
                            .AddDays(7 * (source.EarliestNextPossibleTestNumericValue ?? 1));

                        model.EarliestDateForTest = DateTimeExtensions.GetHighest(lastTestPlusThreeDays, startOfNextWeek);

                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                model.EarliestDateForTest = DateTime.Now;
            }

            // Raise event that employee started registration
            await NotifyStartedRegistration(
                    source: 1,
                    personalNumber: personalNumber,
                    firstName: firstName,
                    lastName: lastName
                );

            // fill additional data
            model = await FillBookingModel(model);

            // data for selectlists
            List<ApplicationCountry> enumCountries = await _enumsDataProvider.GetCountries();

            enumCountries = enumCountries
                .OrderByDescending(i => i.IsImportant)
                .ToList();

            ViewData["ApplicationCountries"] = new SelectList(enumCountries, "Value", "Text");

            // get history
            List<ApplicationBooking> lastBookings;

            if (_settingsService.GetSettingsForCurrentSource<bool>("SearchEmployeeBasedOnSurname", false, false))
                lastBookings = await _bookingDataProvider.GetLastTestsForEmployee(personalNumber, source.SourceID, 10, lastName);
            else
                lastBookings = await _bookingDataProvider.GetLastTestsForEmployee(personalNumber, source.SourceID, 10);

            ViewData["LastBookings"] = lastBookings;

            // return view
            return View(model);
        }

        public async Task<IActionResult> _GetAvailableDates()
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            TimeSlotsViewModel model = new TimeSlotsViewModel();

            model.DatesWithSlots = (await _bookingDataProvider.GetDatesWithSlots(source.SourceID, futureOnly: true))
                .Select(i => new DateAvailabilityOverview(i))
                .ToList();

            return Json(model);
        }

        public async Task<IActionResult> Summary(int id)
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            ApplicationBooking booking = await _bookingDataProvider.GetBooking(source.SourceID, id);

            if (booking == null)
                return new NotFoundResult();

            return View(booking);
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Check if user has access to booking
        /// </summary>
        /// <param name="source"></param>
        /// <param name="personalNumber"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        private async ValueTask<bool> UserHasAccessToBooking(int source, string personalNumber, string firstName, string lastName)
        {
            // for now dont validate employees, always return true
            return await new ValueTask<bool>(true);
        }

        private Task NotifyStartedRegistration(int source, string personalNumber, string firstName, string lastName)
        {
            // intentionally left blank
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get additional data for booking model
        /// </summary>
        /// <param name="bookingViewModel"></param>
        /// <returns></returns>
        private async ValueTask<BookingViewModel> FillBookingModel(BookingViewModel bookingViewModel)
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            // get employee data from last test
            //var lastBooking = await _bookingDataProvider.GetLastTestForEmployee(bookingViewModel.PersonalNumber, source.SourceID);

            // get last test entity
            Data.Tables.TimeSlotBooking lastBooking;

            if (_settingsService.GetSettingsForCurrentSource<bool>("SearchEmployeeBasedOnSurname", false, false))
                lastBooking = await _bookingDataProvider.GetLastTestForEmployee(bookingViewModel.PersonalNumber, source.SourceID, bookingViewModel.LastName);
            else
                lastBooking = await _bookingDataProvider.GetLastTestForEmployee(bookingViewModel.PersonalNumber, source.SourceID);

            // if any data found
            if (lastBooking != null && !String.IsNullOrEmpty(lastBooking.SysAdditionalData))
            {
                Dictionary<string, object> additionalData = JsonConvert.DeserializeObject<Dictionary<string, object>>(lastBooking.SysAdditionalData);

                bookingViewModel.City = (string)additionalData.TryReturnValue("city", bookingViewModel.City);
                bookingViewModel.ZIPCode = (string)additionalData.TryReturnValue("zip", bookingViewModel.ZIPCode);
                bookingViewModel.Nationality = (string)additionalData.TryReturnValue("nationality", bookingViewModel.Nationality);
                //bookingViewModel.Gender = Enum.Parse<Gender>((string)additionalData.TryReturnValue("gender", bookingViewModel.Gender));
                //bookingViewModel.Insurance = Enum.Parse<InsuranceCompany>((string)additionalData.TryReturnValue("insurance", bookingViewModel.Insurance));

                if (Enum.TryParse<Gender>((string)additionalData.TryReturnValue("gender", bookingViewModel.Gender), out Gender tempGender))
                    bookingViewModel.Gender = tempGender;

                if (Enum.TryParse<InsuranceCompany>((string)additionalData.TryReturnValue("insurance", bookingViewModel.Insurance), out InsuranceCompany tempInsurance))
                    bookingViewModel.Insurance = tempInsurance;
            }

            // return bookingviewmodel back
            return await new ValueTask<BookingViewModel>(bookingViewModel);
        }
    }
}
