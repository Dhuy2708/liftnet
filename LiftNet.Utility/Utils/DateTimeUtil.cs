using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Utils
{
    public static class DateTimeUtil
    {
        public static DateTimeOffset ToOffSet(this DateTime dateTime, string timeZoneId = "UTC")
        {
            if (string.IsNullOrEmpty(timeZoneId))
            {
                timeZoneId = "UTC";
            }
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTime(dateTime, timeZone);
        }

        public static DateTimeOffset? ToOffSet(this DateTime? dateTime, string timeZoneId = "UTC")
        {
            if (dateTime == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(timeZoneId))
            {
                timeZoneId = "UTC";
            }
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTime(dateTime.Value, timeZone);
        }
    }
}
