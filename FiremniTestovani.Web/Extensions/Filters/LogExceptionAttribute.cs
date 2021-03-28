using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Extensions.Filters
{
    public class LogExceptionsAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            ILoggerFactory loggerFactory = (ILoggerFactory)context.HttpContext.RequestServices.GetService(typeof(ILoggerFactory));
            ILogger logger = loggerFactory.CreateLogger("Application");

            //set breakpoing on the following line to see what the requested path and query is
            string pathAndQuery = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;

            logger.LogError(context.Exception.ToString());

            // Code 
            base.OnException(context);
        }
    }
}
