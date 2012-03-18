using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Web.Caching;

namespace Daruyanagi
{
    public static class FeedHelper
    {
        public static string GetHtml(string url, string count = "10")
        {
            return GetHtml(url, int.Parse(count));
        }

        public static string GetHtml(string url, int count)
        {
            var cache_key = string.Format("{0}_{1}_{2}", "feed", url, count);

            var cache = (string) HttpRuntime.Cache.Get(cache_key);
            if (cache == null)
            {
                cache = BuildCache(url, count);
                HttpRuntime.Cache.Insert(
                    cache_key, cache, null,
                    DateTime.UtcNow.AddHours(1),
                    Cache.NoSlidingExpiration
                );
            }

            return cache;
        }

        private static string BuildCache(string url, int count)
        {
            var div = new TagBuilder("div");
            div.Attributes.Add("class", "feed");

            try
            {
                using (var reader = XmlReader.Create(url))
                {
                    var feed = SyndicationFeed.Load(reader);
                    /*
                    var h2 = new TagBuilder("h2");
                    h2.InnerHtml = feed.Title.Text;
                    div.InnerHtml += h2.ToString();
                    */
                    var ul = new TagBuilder("ul");
                    foreach (var i in feed.Items)
                    {
                        var a = new TagBuilder("a");
                        a.Attributes.Add("href", i.Links.Count > 0
                            ? i.Links[0].Uri.ToString() : "");
                        a.Attributes.Add("title", i.Title.Text);
                        a.InnerHtml = i.Title.Text;

                        var li = new TagBuilder("li");
                        if (i.LastUpdatedTime.AddDays(1) > DateTime.Now)
                            li.InnerHtml += "<span class='label label-success'>New</span>";
                        li.InnerHtml += a.ToString();
                        ul.InnerHtml += li.ToString();
                        if (--count < 1) break;
                    }
                    div.InnerHtml += ul.ToString();
                }
            }
            catch (Exception e)
            {
                var p = new TagBuilder("p");
                p.Attributes.Add("class", "error");
                p.InnerHtml = e.Message;

                div.InnerHtml += p.ToString();
            }

            return div.ToString();
        }
    }
}