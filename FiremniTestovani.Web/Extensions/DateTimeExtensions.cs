using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToString(this DateTime? date, string format)
        {
            if (!date.HasValue)
                return String.Empty;

            return date.Value.ToString(format);
        }

        public static DateTime SetDateTime(this DateTime date, int? year = null, int? month = null, int? day = null, int? hour = null, int? minute = null, int? second = null)
        {
            DateTime tempDate = new DateTime(
                    year ?? date.Year,
                    month ?? date.Month,
                    day ?? date.Day,
                    hour ?? date.Hour,
                    minute ?? date.Minute,
                    second ?? date.Second
                );

            return tempDate;
        }

        public static DateTime? SetDateTime(this DateTime? date, int? year = null, int? month = null, int? day = null, int? hour = null, int? minute = null, int? second = null)
        {
            if (!date.HasValue)
                return null;

            DateTime tempDate = new DateTime(
                    year ?? date.Value.Year,
                    month ?? date.Value.Month,
                    day ?? date.Value.Day,
                    hour ?? date.Value.Hour,
                    minute ?? date.Value.Minute,
                    second ?? date.Value.Second
                );

            return tempDate;
        }

        public static IEnumerable<DateTime> GetDateRange(DateTime startDate, DateTime endDate, TimeSpan step, bool includeEndDate = true)
        {
            if (endDate < startDate)
                throw new ArgumentException("endDate must be greater than or equal to startDate");

            if (!includeEndDate)
            {
                while (startDate < endDate)
                {
                    yield return startDate;
                    startDate = startDate.Add(step);
                }
            }
            else
            {
                while (startDate <= endDate)
                {
                    yield return startDate;
                    startDate = startDate.Add(step);
                }
            }
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime GetHighest(params DateTime[] dateTimes)
        {
            return dateTimes.Max();
        }
    }
}
