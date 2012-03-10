using System;
using System.Collections.Generic;
using System.Web;

using System.Text.RegularExpressions;
using System.Globalization;
using System.Net;
using System.Linq;

/// <summary>
/// Extends String Class
/// </summary>
public static class StringSupport
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

    public static string Linkfy(this string input, bool expand_url = false)
    {
        var r1 = new Regex(@"(https?|ftp)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?");
        var r2 = new Regex(@"mailto:(\w[\w\+\.\-_]+@\w[\w\.]+$)");
        var result = input;

        result = r1.Replace(result, (MatchEvaluator)( m => {
            var href = m.Value;
            if (expand_url) href = href.ExpandUrl();
            return string.Format("<a href='{0}'>{1}</a>", href, HttpUtility.UrlDecode(href));
        }));

        result = r2.Replace(result, (MatchEvaluator)( m => {
            return string.Format("<a href='{0}'>{1}</a>", m.Value, m.Groups[1].Value);
        }));

        return result;
    }

    public static string ExpandUrl(this string input)
    {   
        try
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(input);
            WebResponse res = req.GetResponse();
            return res.ResponseUri.ToString();
        }
        catch
        {
            return input;
        }
    }

    public static long BaseDecode(string input, string alphabet)
    {
        long decoded = 0, multi = 1;

        foreach (var c in input.Reverse())
        {
            decoded += multi * alphabet.IndexOf(c);
            multi *= alphabet.Length;
        }

        return decoded;
    }

    const string BASE58 = "123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";

    public static long Base58Decode(this string input)
    {
        return BaseDecode(input, BASE58);
    }

    private static readonly string[] TWITTER_FORMAT = new[]
    {
        /* Thu Apr 07 06:10:17 +0000 2011 */
        "ddd MMM dd HH:mm:ss zzzz yyyy",
        /* Thu Apr 07 06:10:17 UTC 2011 */
        "ddd MMM dd HH:mm:ss UTC yyyy",
    };

    public static DateTime ParseAsTwitterFormat(this string input)
    {
        foreach (var format in TWITTER_FORMAT)
        {
            DateTime output;
            if (DateTime.TryParseExact(
                input, format, DateTimeFormatInfo.InvariantInfo,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal,
                out output)
            ) { return output; }
        }

        throw new ArgumentException(
            string.Format("'{0}' is not Twitter's DateTime format", input));
    }

    private static readonly string[] RFC2822_FORMAT = new[]
    {
        /* Thu, 29 Sep 2011 22:53:00 GMT */
        "ddd, dd MMM yyyy HH:mm:ss GMT",
        /* Thu, 29 Sep 2011 22:53:00 +9000 */
        "ddd, dd MMM yyyy HH:mm:ss zzzz",
        /* Thu, 29 Sep 2011 22:53 GMT */
        "ddd, dd MMM yyyy HH:mm GMT",
        /* Thu, 29 Sep 2011 22:53 +9000 */
        "ddd, dd MMM yyyy HH:mm zzzz",
        /* 29 Sep 2011 22:53:00 GMT */
        "dd MMM yyyy HH:mm:ss GMT",
        /* 29 Sep 2011 22:53:00 +9000 */
        "dd MMM yyyy HH:mm:ss zzzz",
        /* 29 Sep 2011 22:53 GMT */
        "dd MMM yyyy HH:mm GMT",
        /* 29 Sep 2011 22:53 +9000 */
        "dd MMM yyyy HH:mm zzzz",
    };

    public static DateTime ParseAsRfc2822Format(this string input)
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
