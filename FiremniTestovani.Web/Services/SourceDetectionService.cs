using FiremniTestovani.Data.DbContext;
using FiremniTestovani.Models;
using FiremniTestovani.Web.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Services
{
    public class SourceDetectionService : ISourceDetectionService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly ApplicationDbContext _db;

        private ApplicationSource _applicationSource;

        public SourceDetectionService(IHttpContextAccessor httpContext, ApplicationDbContext db)
        {
            this._httpContext = httpContext;
            this._db = db;

            // get source
            var url = _httpContext.HttpContext.Request.GetDisplayUrl();

            var sourcesList = _db.Sources
                    //.Include(i => i.R_DefaultCurrency)
                    .ToList();

            //var dbSource = sourcesList
            //    .Where(i => !String.IsNullOrEmpty(i.URL))
            //    .OrderByDescending(i => i.URL.Length)
            //    .FirstOrDefault(i => url.Contains(i.URL));

            // at first try externalURLs
            var dbSource = sourcesList
                .Where(i => !String.IsNullOrEmpty(i.URL))
                .OrderByDescending(i => i.URL.Length)
                .FirstOrDefault(i => url.Contains(i.URL));

            // then internal ones (only if no match)
            if (dbSource == null)
            {
                dbSource = sourcesList
                    .Where(i => !String.IsNullOrEmpty(i.InternalURL))
                    .OrderByDescending(i => i.InternalURL.Length)
                    .FirstOrDefault(i => url.Contains(i.InternalURL));
            }

            if (dbSource == null)
                _applicationSource = null;
            else
            {
                _applicationSource = new ApplicationSource(dbSource);
            }
        }

        public ApplicationSource ApplicationSource => this._applicationSource;

        public async ValueTask<ApplicationSource> LoadApplicationSource(int? sourceID = null)
        {
            ApplicationSource source;

            if (sourceID.HasValue)
            {
                var dbSource = await _db.Sources
                    //.Include(i => i.R_DefaultCurrency)
                    .SingleOrDefaultAsync(i => i.SourceID == sourceID);

                if (dbSource == null)
                    source = null;
                else
                    source = new ApplicationSource(dbSource);
            }
            else
            {
                var url = _httpContext.HttpContext.Request.GetDisplayUrl();

                var sourcesList = _db.Sources
                        //.Include(i => i.R_DefaultCurrency)
                        .ToList();

                //var dbSource = sourcesList
                //    .Where(i => !String.IsNullOrEmpty(i.URL))
                //    .OrderByDescending(i => i.URL.Length)
                //    .FirstOrDefault(i => url.Contains(i.URL));

                // at first try externalURLs
                var dbSource = sourcesList
                    .Where(i => !String.IsNullOrEmpty(i.URL))
                    .OrderByDescending(i => i.URL.Length)
                    .FirstOrDefault(i => url.Contains(i.URL));

                // then internal ones (only if no match)
                if (dbSource == null)
                {
                    dbSource = sourcesList
                        .Where(i => !String.IsNullOrEmpty(i.InternalURL))
                        .OrderByDescending(i => i.InternalURL.Length)
                        .FirstOrDefault(i => url.Contains(i.InternalURL));
                }

                if (dbSource == null)
                    source = null;
                else
                {
                    sourceID = dbSource.SourceID;
                    source = new ApplicationSource(dbSource);
                }
            }

            return source;
        }
    }
}
