using FiremniTestovani.Data.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wangkanai.Detection.Models;
using Wangkanai.Detection.Services;

namespace FiremniTestovani.Web.Extensions.Base
{
    public class BaseController : Controller
    {
        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly IDetectionService _detectionService;
        protected readonly ILogger _logger;
        protected readonly IConfiguration _configuration;
        protected readonly ApplicationDbContext _db;

        public BaseController(
            IWebHostEnvironment webHostEnvironment,
            IDetectionService detectionService,
            ILoggerFactory logger,
            IConfiguration configuration,
            ApplicationDbContext db)
        {
            _detectionService = detectionService;
            _logger = logger.CreateLogger("Application");
            _configuration = configuration;
            _db = db;
        }

        protected bool RequestIsMobile()
        {
            Device[] mobileDevices = new[]
            {
                Device.Mobile,
                Device.Car,
                Device.Tablet,
                Device.Watch
            };

            return mobileDevices.Contains(_detectionService.Device.Type);
        }

        protected class BoolSelectListItem
        {
            public bool value { get; set; }

            public string text { get; set; }
        }
    }
}
