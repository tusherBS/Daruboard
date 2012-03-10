using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daruyanagi
{
    public static class DateTimeExtension
    {
        public static string ToRfc822String(this DateTime input)
        {
            return input.ToString(
               "ddd, dd MMM yyyy HH_mm_ss zzz",
               System.Globalization.CultureInfo.InvariantCulture
            ).Replace(":", "").Replace("_", ":");
        }
    }
}
