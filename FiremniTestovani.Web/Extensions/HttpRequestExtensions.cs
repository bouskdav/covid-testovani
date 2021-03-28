using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return !String.IsNullOrEmpty(request.Headers["X-Requested-With"]) &&
                    String.Equals(
                        request.Headers["X-Requested-With"],
                        "XmlHttpRequest",
                        StringComparison.OrdinalIgnoreCase);

            return false;
        }
    }
}
