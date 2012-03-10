using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using Codeplex.Data;

namespace Daruyanagi
{
    public static class FlickrHelper
    {
        private static readonly string SERVICE_ENDPOINT = @"http://www.flickr.com/services/oembed";
        private static readonly string FORMAT_URL = @"{0}?url={1}&maxwidth={2}&maxheight={3}&format={4}";

        public static string FORMAT_HTML_IMAGE_TAG = @"
<blockquote class='flickr flickr-photo'>
  <p><img src='{0}' alt='{1}' /><p>
  <p><small>{1} by <a href='{3}'>{2}</a></small><p>
</blockquote>
";
        public static string FORMAT_HTML_VIDEO_TAG = @"
<blockquote class='flickr flickr-video'>
  <p>{0}<p>
  <p><small>{1} by <a href='{3}'>{2}</a></small><p>
</blockquote>
";
        public static string FORMAT_ERROR = @"<p class='error'>{0}</p>";

        public static string GetHtml(
            string url, string max_width = "500", string max_height = "500")
        {
            try
            {
                return GetHtml(url, int.Parse(max_width), int.Parse(max_height));
            }
            catch (Exception e)
            {
                return string.Format(FORMAT_ERROR, e.Message);
            }
        }

        public static string GetHtml(
            string url, int max_width, int max_height)
        {
            try
            {
                if (url.StartsWith("http://flic.kr/p/"))
                    url = new Uri(url).GetResponseUri().ToString();

                var format = "json";

                var address = string.Format(FORMAT_URL,
                    SERVICE_ENDPOINT, url, max_width, max_height, format);

                using (var client = new WebClient())
                {
                    var response = client.DownloadString(address);
                    var info = DynamicJson.Parse(response);

                    switch (info.type as string)
                    {
                        case "photo":
                            return string.Format(FORMAT_HTML_IMAGE_TAG,
                                info.url, info.title,
                                info.author_name, info.author_url);

                        case "video":
                            return string.Format(FORMAT_HTML_VIDEO_TAG,
                                info.html, info.title,
                                info.author_name, info.author_url);

                        default:
                            throw new Exception("Unknown media type.");
                    }
                }
            }
            catch (Exception e)
            {
                return string.Format(FORMAT_ERROR, e.Message);
            }
        }
    }
}