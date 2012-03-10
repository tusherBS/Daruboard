using System;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Daruyanagi
{
    public static class StringExtention
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsRegexMatch(this string value, string pattern)
        {
            return Regex.IsMatch(value, pattern);
        }

        public static string UrlEncode(this string value)
        {
            return Uri.EscapeDataString(value);
        }

        public static string UrlDecode(this string value)
        {
            return Uri.UnescapeDataString(value);
        }

        private static readonly string[] RFC2822_FORMAT = new[]
        {
            /* Thu Apr 07 06:10:17 +0000 2011 */
            "ddd MMM dd HH:mm:ss zzzz yyyy",
            /* Thu Apr 07 06:10:17 UTC 2011 */
            "ddd MMM dd HH:mm:ss UTC yyyy"
        };

        public static DateTime ParseAsRfc2822(this string input)
        {
            foreach (var format in RFC2822_FORMAT)
            {
                DateTime output;
                if (DateTime.TryParseExact(
                    input, format, DateTimeFormatInfo.InvariantInfo,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal,
                    out output)
                ) { return output; }
            }

            throw new ArgumentException(
                string.Format("'{0}' is not RFC2822 format", input));
        }
    }
}
