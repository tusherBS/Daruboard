using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml.Linq;
using System.Web.Mvc;

namespace Daruyanagi
{
    public static class AmazonHelper
    {
        public static string AccessKeyId;
        public static string SecretKey;
        public static string AWS_SERVICE = "AWSECommerceService";
        public static string AWS_VERSION = "2011-08-01";
        public static string Destination = "ecs.amazonaws.jp";
        public static string AssosiateTag = "bestylesnet-22";
        public static string StyleSheetUrl = "/Styles/Amazon2Html.css";

        private static string ExpandUrl(string input)
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

        private static string GetAsinFromUrl(string url)
        {
            try
            {
                url = ExpandUrl(url);

                return new Regex(@"\/(?<id>[A-z0-9]{10})")
                    .Matches(url)[0].Groups["id"].Value;
            }
            catch
            {
                throw new Exception("不正なURLです。ASINが取得できませんでした。");
            }
        }

        private static string BuildRequestUrl(string item_id)
        {
            if (string.IsNullOrWhiteSpace(AccessKeyId) && string.IsNullOrWhiteSpace(SecretKey))
                throw new Exception("AccessKeyId or SecretKey is null or empty.");
        
            var helper = new SignedRequestHelper(
                AccessKeyId, SecretKey, Destination);

            var param = new Dictionary<string, String>();
            param["Service"] = AWS_SERVICE;
            param["Version"] = AWS_VERSION;
            param["Operation"] = "ItemLookup";
            param["ItemId"] = item_id;
            param["ResponseGroup"] = "Medium";
            param["AssociateTag"] = AssosiateTag;

            return helper.Sign(param);
        }

        public static string DownloadItemData(string url)
        {
            var item_id = GetAsinFromUrl(url);
            var uri = BuildRequestUrl(item_id);

            return new WebClient() {
                Encoding = Encoding.GetEncoding("utf-8"),
            }.DownloadString(uri);
        }

        private static dynamic ParseItemData(string data)
        {
            var x = XDocument.Parse(data);

            XNamespace ns = string.Format(
                @"http://webservices.amazon.com/{0}/{1}",
                AWS_SERVICE, AWS_VERSION);

            foreach (var error in x.Descendants(ns + "Error"))
            {
                throw new Exception(string.Format("{0}: {1}",
                    error.Element(ns + "Code").Value,
                    error.Element(ns + "Message").Value));
            }

            var q = from e in x.Descendants(ns + "Item")
                    select new 
            {
                Asin     = e.Element(ns + "ASIN"),
                Url      = e.Element(ns + "DetailPageURL"),
                ImageUrl = e.Element(ns + "MediumImage").Element(ns + "URL"),

                Binding         = e.Element(ns + "ItemAttributes").Element(ns + "Binding"),
                Brand           = e.Element(ns + "ItemAttributes").Element(ns + "Brand"),
                Manufacturer    = e.Element(ns + "ItemAttributes").Element(ns + "Manufacturer"),
                Title           = e.Element(ns + "ItemAttributes").Element(ns + "Title"),
                Author          = e.Element(ns + "ItemAttributes").Element(ns + "Author"),
                PublicationDate = e.Element(ns + "ItemAttributes").Element(ns + "PublicationDate"),
                ReleaseDate     = e.Element(ns + "ItemAttributes").Element(ns + "ReleaseDate"),
                ProductGroup    = e.Element(ns + "ItemAttributes").Element(ns + "ProductGroup"),
                Studio          = e.Element(ns + "ItemAttributes").Element(ns + "Studio"),
                    
                Price = e.Element(ns + "OfferSummary")
                         .Element(ns + "LowestNewPrice")
                         .Element(ns + "FormattedPrice"),
            };

            return q.First();
        }

        public static string IncludeStyleSheet()
        {
            return string.Format(
                "<link href='{0}' rel='stylesheet' type='text/css' />",
                StyleSheetUrl);
        }

        public static string GetHtml(string url)
        {
            var response = DownloadItemData(url);
            var result = ParseItemData(response);

            var date = result.PublicationDate;
            if (date == null) { date = result.ReleaseDate; }

            var dl = new TagBuilder("dl");
            dl.MergeAttribute("class", "amazon-goods-detail");

            if (result.Title != null && result.Url != null)
            {
                var dt = new TagBuilder("dt");
                dt.InnerHtml = "Title";
                dl.InnerHtml += dt.ToString();

                var dd = new TagBuilder("dd");
                dd.InnerHtml = string.Format("<a href='{1}'>{0}</a>",
                    result.Title.Value, result.Url.Value);
                dl.InnerHtml += dd.ToString();
            }

            if (result.Studio != null)
            {
                var dt = new TagBuilder("dt");
                dt.InnerHtml = "Studio";
                dl.InnerHtml += dt.ToString();

                var dd = new TagBuilder("dd");
                dd.InnerHtml = string.Format("{0} ({1})",
                    result.Studio.Value, date != null ? date.ToString() : "");
                dl.InnerHtml += dd.ToString();
            }

            if (result.Author != null)
            {
                var dt = new TagBuilder("dt");
                dt.InnerHtml = "Author";
                dl.InnerHtml += dt.ToString();

                var dd = new TagBuilder("dd");
                dd.InnerHtml = result.Author.Value;
                dl.InnerHtml += dd.ToString();
            }

            if (result.Binding != null)
            {
                var dt = new TagBuilder("dt");
                dt.InnerHtml = "Binding";
                dl.InnerHtml += dt.ToString();

                var dd = new TagBuilder("dd");
                dd.InnerHtml = result.Binding.Value;
                dl.InnerHtml += dd.ToString();
            }

            if (result.Price != null)
            {
                var dt = new TagBuilder("dt");
                dt.InnerHtml = "Price";
                dl.InnerHtml += dt.ToString();

                var dd = new TagBuilder("dd");
                dd.InnerHtml = result.Price.Value;
                dl.InnerHtml += dd.ToString();
            }

            var div = new TagBuilder("div");
            {
                if (result.Title != null && result.ImageUrl != null)
                {
                    var img = new TagBuilder("img");
                    img.MergeAttribute("class", "amazon-goods-image");
                    img.MergeAttribute("src", result.ImageUrl.Value);
                    img.MergeAttribute("alt", result.Title.Value);

                    div.InnerHtml += img.ToString();
                }
                div.InnerHtml += dl.ToString();
                div.MergeAttribute("class", "amazon-goods");
            }

            return div.ToString();
        }
    }
}