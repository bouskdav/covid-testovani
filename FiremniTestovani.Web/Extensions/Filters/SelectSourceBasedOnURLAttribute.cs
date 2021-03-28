using FiremniTestovani.Data.DbContext;
using FiremniTestovani.Data.Tables;
using FiremniTestovani.Models;
using FiremniTestovani.Web.Models;
using FiremniTestovani.Web.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Extensions.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SelectSourceBasedOnURLAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            int? sourceID;
            ApplicationSource source;

            ApplicationDbContext db = filterContext.HttpContext.RequestServices.GetService<ApplicationDbContext>();

            if (filterContext.ActionArguments.ContainsKey("source") && filterContext.ActionArguments["source"] != null)
            {
                sourceID = (int)filterContext.ActionArguments["source"];

                var dbSource = db.Sources
                    //.Include(i => i.R_DefaultCurrency)
                    .SingleOrDefault(i => i.SourceID == sourceID);

                if (dbSource == null)
                {
                    filterContext.Result = new ContentResult() { Content = $"Této doméně neodpovídá žádná firemní stránka nastavená v tabulce zdrojů!" };

                    base.OnActionExecuting(filterContext);
                    return;
                }

                source = new ApplicationSource(dbSource);
            }
            else
            {
                var url = filterContext.HttpContext.Request.GetDisplayUrl();

                var sourcesList = db.Sources
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
                {
                    filterContext.Result = new ContentResult() { Content = "Této doméně neodpovídá žádná firemní stránka nastavená v tabulce zdrojů!" };

                    base.OnActionExecuting(filterContext);
                    return;
                }

                sourceID = dbSource.SourceID;
                source = new ApplicationSource(dbSource);

                if (filterContext.ActionArguments.ContainsKey("source"))
                    filterContext.ActionArguments["source"] = sourceID;
                else
                    filterContext.ActionArguments.Add("source", sourceID);
            }

            //zdroj.UICulture.NumberFormat.CurrencySymbol = zdroj.Currency.CurrencySymbol;
            //zdroj.UICulture.NumberFormat.CurrencyPositivePattern = 3;
            //zdroj.UICulture.NumberFormat.CurrencyNegativePattern = 8;

            filterContext.HttpContext.Items["source"] = source;
            //new LanguageManager().SetLanguage(zdroj.UICulture);

            base.OnActionExecuting(filterContext);
        }
    }
}
