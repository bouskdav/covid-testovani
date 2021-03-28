using ClosedXML.Excel;
using FiremniTestovani.Data.DbContext;
using FiremniTestovani.DataProviders;
using FiremniTestovani.Models;
using FiremniTestovani.Web.Extensions.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wangkanai.Detection.Services;

namespace FiremniTestovani.Web.Controllers
{
    public class ExportController : BaseController
    {
        private BookingDataProvider _bookingDataProvider;
        private EnumsDataProvider _enumsDataProvider;

        public ExportController(IWebHostEnvironment webHostEnvironment, IDetectionService detectionService, ILoggerFactory logger, IConfiguration configuration, ApplicationDbContext db) 
            : base(webHostEnvironment, detectionService, logger, configuration, db)
        {
            this._bookingDataProvider = new BookingDataProvider(db, webHostEnvironment, configuration);
            this._enumsDataProvider = new EnumsDataProvider(db, webHostEnvironment, configuration);
        }

        [HttpGet]
        public IActionResult DownloadDayExcel()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> DownloadDayExcel(DateTime date)
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = $"CovidTesty_{date.ToString("yyyy-MM-dd")}.xlsx";

            // Taken List of data from json file which we want to export to excel.
            List<ApplicationBooking> bookings = await _bookingDataProvider.GetBookingsForDate(source.SourceID, date.Date,
                excludeCanceled: true);

            List<ApplicationCountry> countries = await _enumsDataProvider.GetCountries();

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add($"COVID {date.ToString("d. M. yyyy")}");
                worksheet.Range("A1").Value = "Čas začátku";
                worksheet.Range("B1").Value = "Jméno";
                worksheet.Range("C1").Value = "Příjmení";
                worksheet.Range("D1").Value = "Pojišťovna";
                worksheet.Range("E1").Value = "Pohlaví";
                worksheet.Range("F1").Value = "R.Č.";
                worksheet.Range("G1").Value = "Osobní číslo";
                worksheet.Range("H1").Value = "Datum narození";
                worksheet.Range("I1").Value = "Státní příslušnost";
                worksheet.Range("J1").Value = "Město";
                worksheet.Range("K1").Value = "PSČ";
                worksheet.Range("L1").Value = "Telefon";
                worksheet.Range("M1").Value = "Email";
                worksheet.Range("N1").Value = "Výsledek";
                worksheet.Range("O1").Value = "Poznámka";

                worksheet.Column(1).Width = 20;
                worksheet.Column(2).Width = 15;
                worksheet.Column(3).Width = 15;
                worksheet.Column(4).Width = 12.5;
                worksheet.Column(6).Width = 20;
                worksheet.Column(7).Width = 15;
                worksheet.Column(8).Width = 15;
                worksheet.Column(10).Width = 15;
                worksheet.Column(12).Width = 15;
                worksheet.Column(13).Width = 20;
                worksheet.Column(14).Width = 15;
                worksheet.Column(15).Width = 20;

                worksheet.Column(14).Style.Fill.SetBackgroundColor(XLColor.Yellow);
                worksheet.Column(15).Style.Fill.SetBackgroundColor(XLColor.Orange);

                for (int i = 1; i <= bookings.Count; i++)
                {
                    Dictionary<string, object> additionalData = bookings[i - 1].GetAdditionalData();

                    // nationality
                    var nationality = additionalData.GetValueOrDefault("nationality");
                    string nationalityString = nationality.ToString();

                    if (nationality != null && Int32.TryParse(nationality.ToString(), out int nationalityID))
                        nationalityString = countries.SingleOrDefault(i => i.ID == nationalityID).ISOCode;

                    worksheet.Cell(i + 1, 1).SetValue(bookings[i - 1].FromExpected);
                    worksheet.Cell(i + 1, 2).SetValue(bookings[i - 1].FirstName);
                    worksheet.Cell(i + 1, 3).SetValue(bookings[i - 1].LastName);

                    worksheet.Cell(i + 1, 4).SetValue(additionalData.GetValueOrDefault("insurance"));
                    worksheet.Cell(i + 1, 5).SetValue(additionalData.GetValueOrDefault("gender"));
                    worksheet.Cell(i + 1, 6).SetValue(additionalData.GetValueOrDefault("personal_identification_number"));

                    worksheet.Cell(i + 1, 7).SetValue(bookings[i - 1].EmployeeID);

                    worksheet.Cell(i + 1, 8).SetValue(additionalData.GetValueOrDefault("date_of_birth"));
                    worksheet.Cell(i + 1, 9).SetValue(additionalData.GetValueOrDefault("nationality"));
                    worksheet.Cell(i + 1, 10).SetValue(additionalData.GetValueOrDefault("city"));
                    worksheet.Cell(i + 1, 11).SetValue(additionalData.GetValueOrDefault("zip"));

                    worksheet.Cell(i + 1, 12).SetValue(bookings[i - 1].Phone);

                    worksheet.Cell(i + 1, 13).SetValue(bookings[i - 1].Email);
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
        }

        [HttpGet]
        public IActionResult DownloadDayResultExcel()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> DownloadDayResultExcel(DateTime date)
        {
            // get source from URL
            ApplicationSource source = (ApplicationSource)HttpContext.Items["source"];

            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = $"CovidTesty_{date.ToString("yyyy-MM-dd")}.xlsx";

            // Taken List of data from json file which we want to export to excel.
            List<ApplicationBooking> bookings = await _bookingDataProvider.GetBookingsForDate(source.SourceID, date.Date,
                excludeCanceled: true);

            List<ApplicationCountry> countries = await _enumsDataProvider.GetCountries();

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add($"COVID {date.ToString("d. M. yyyy")}");
                worksheet.Range("A1").Value = "Čas začátku";
                worksheet.Range("B1").Value = "Jméno";
                worksheet.Range("C1").Value = "Příjmení";
                worksheet.Range("D1").Value = "Pojišťovna";
                worksheet.Range("E1").Value = "Pohlaví";
                worksheet.Range("F1").Value = "R.Č.";
                worksheet.Range("G1").Value = "Osobní číslo";
                worksheet.Range("H1").Value = "Datum narození";
                worksheet.Range("I1").Value = "Státní příslušnost";
                worksheet.Range("J1").Value = "Město";
                worksheet.Range("K1").Value = "PSČ";
                worksheet.Range("L1").Value = "Telefon";
                worksheet.Range("M1").Value = "Email";
                worksheet.Range("N1").Value = "Výsledek";
                worksheet.Range("O1").Value = "Poznámka";

                worksheet.Column(1).Width = 20;
                worksheet.Column(2).Width = 15;
                worksheet.Column(3).Width = 15;
                worksheet.Column(4).Width = 12.5;
                worksheet.Column(6).Width = 20;
                worksheet.Column(7).Width = 15;
                worksheet.Column(8).Width = 15;
                worksheet.Column(10).Width = 15;
                worksheet.Column(12).Width = 15;
                worksheet.Column(13).Width = 20;
                worksheet.Column(14).Width = 15;
                worksheet.Column(15).Width = 20;

                //worksheet.Column(14).Style.Fill.SetBackgroundColor(XLColor.Yellow);
                //worksheet.Column(15).Style.Fill.SetBackgroundColor(XLColor.Orange);

                for (int i = 1; i <= bookings.Count; i++)
                {
                    Dictionary<string, object> additionalData = bookings[i - 1].GetAdditionalData();

                    // nationality
                    var nationality = additionalData.GetValueOrDefault("nationality");
                    string nationalityString = nationality.ToString();

                    if (nationality != null && Int32.TryParse(nationality.ToString(), out int nationalityID))
                        nationalityString = countries.SingleOrDefault(i => i.ID == nationalityID).ISOCode;

                    worksheet.Cell(i + 1, 1).SetValue(bookings[i - 1].FromExpected);
                    worksheet.Cell(i + 1, 2).SetValue(bookings[i - 1].FirstName);
                    worksheet.Cell(i + 1, 3).SetValue(bookings[i - 1].LastName);

                    worksheet.Cell(i + 1, 4).SetValue(additionalData.GetValueOrDefault("insurance"));
                    worksheet.Cell(i + 1, 5).SetValue(additionalData.GetValueOrDefault("gender"));
                    worksheet.Cell(i + 1, 6).SetValue(additionalData.GetValueOrDefault("personal_identification_number"));

                    worksheet.Cell(i + 1, 7).SetValue(bookings[i - 1].EmployeeID);

                    worksheet.Cell(i + 1, 8).SetValue(additionalData.GetValueOrDefault("date_of_birth"));
                    worksheet.Cell(i + 1, 9).SetValue(additionalData.GetValueOrDefault("nationality"));
                    worksheet.Cell(i + 1, 10).SetValue(additionalData.GetValueOrDefault("city"));
                    worksheet.Cell(i + 1, 11).SetValue(additionalData.GetValueOrDefault("zip"));

                    worksheet.Cell(i + 1, 12).SetValue(bookings[i - 1].Phone);

                    worksheet.Cell(i + 1, 13).SetValue(bookings[i - 1].Email);

                    if (bookings[i - 1].BookingState == Enums.ApplicationBookingState.Completed)
                    {
                        if (bookings[i - 1].TestResult == true)
                        {
                            worksheet.Cell(i + 1, 14).SetValue("POZITIVNÍ");
                            worksheet.Cell(i + 1, 14).Style.Fill.SetBackgroundColor(XLColor.Red);
                        }
                        else
                            worksheet.Cell(i + 1, 14).SetValue("neg");
                    }

                    worksheet.Cell(i + 1, 15).SetValue(bookings[i - 1].BookingState);
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
        }
    }
}
