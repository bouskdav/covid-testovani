using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Extensions
{
    public static class JsonExtensions
    {
        public static object GetValueAsObject(this JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.Number:
                    return element.GetDouble();
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.Undefined:
                    return null;
                case JsonValueKind.String:
                    return element.GetString();
                default:
                    return null;
            }
        }
    }
}
