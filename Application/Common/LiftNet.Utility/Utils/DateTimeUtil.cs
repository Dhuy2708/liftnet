﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Utils
{
    public static class DateTimeUtil
    {
        public static DateTimeOffset ToOffSet(this DateTime dateTime, TimeSpan? timeSpan = null)
        {
            if (timeSpan == null)
            {
                timeSpan = TimeSpan.Zero;
            }
            return new DateTimeOffset(dateTime, timeSpan.Value);
        }

        public static DateTimeOffset? ToOffSet(this DateTime? dateTime, TimeSpan? timeSpan = null)
        {
            if (dateTime == null)
            {
                return null;
            }
            if (timeSpan == null)
            {
                timeSpan = TimeSpan.Zero;
            }
            return new DateTimeOffset(dateTime.Value, timeSpan.Value);
        }
    }
}
