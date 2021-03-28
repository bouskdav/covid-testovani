using FiremniTestovani.Data.DbContext;
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
    public class EnumsDataProvider
    {
        public EnumsDataProvider(ApplicationDbContext db, IWebHostEnvironment environment, IConfiguration configuration)
        {
            this._db = db;
            this._environment = environment;
            this._configuration = configuration;
        }

        private ApplicationDbContext _db;
        private IWebHostEnvironment _environment;
        private IConfiguration _configuration;

        public async ValueTask<List<ApplicationCountry>> GetCountries()
        {
            var dbCountries = await _db.Countries
                .OrderBy(i => i.ISOCode)
                .ToListAsync();

            return dbCountries
                .Select(i => new ApplicationCountry(i))
                .ToList();
        }
    }
}
