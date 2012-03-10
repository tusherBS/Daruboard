using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.Text;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Daruyanagi
{
    public static class HatenaFotolifeHelper
    {
        public static string User { get; set; }
        public static string Pass { get; set; }

        private static readonly Regex REGEX_FOTOLIFE_URL = new Regex(
            @"http://cdn-ak.f.st-hatena.com/images/fotolife/[A-z0-9_\.\-\/]+"
        );

        public static string GetHtml(string url)
        {
            if (url.IsRegexMatch(@"^\d+$"))
                url = string.Format("http://f.hatena.ne.jp/{0}/{1}", User, url);

            using (WebClient wc = new WebClient { Encoding = Encoding.UTF8 })
            {
                try
                {
                    var s = wc.DownloadString(url);

                    var fotolife_id = new Regex(@"(?<id>\d{14})")
                        .Match(url).Groups["id"].ToString();

                    var image = REGEX_FOTOLIFE_URL
                        .Matches(s).Cast<Match>()
                        .First(m => m.Value.Contains(fotolife_id))
                        .Value;

                    return string.Format("<img src='{0}' />", image);
                }
                catch
                {
                    return string.Format(
                        "<p class='error'>fotolife: {0}</p>",
                        "指定された画像は存在しないかプライベートみたいです。");
                }
            }
        }
    }

    public class HatenaAtomPub
    {
        public static readonly XNamespace NameSpace = "http://purl.org/atom/ns#";

        private string endpoint;
        private string wsse;

        public HatenaAtomPub(string uri, string id, string pass)
        {
            this.endpoint = uri;
            this.wsse = CreateWsseHeader(id, pass);
        }

        private string CreateWsseHeader(string username, string password)
        {
            var nonce = new byte[8];
            new Random().NextBytes(nonce);
            var created = DateTime.Now.ToUniversalTime().ToString("o");

            var digest = nonce
                .Concat(Encoding.UTF8.GetBytes(created))
                .Concat(Encoding.UTF8.GetBytes(password))
                .ToArray();
            var sh1 = new SHA1Managed();
            sh1.Initialize();
            var hashedDigest = sh1.ComputeHash(digest);

            return string.Format(
                "UsernameToken Username=\"{0}\", PasswordDigest=\"{1}\", Nonce=\"{2}\", Created=\"{3}\"",
                username, Convert.ToBase64String(hashedDigest), Convert.ToBase64String(nonce), created);
        }

        public string Get()
        {
            using (WebClient wc = new WebClient { Encoding = Encoding.UTF8 })
            {
                wc.Headers.Add("X-WSSE", this.wsse);
                return wc.DownloadString(this.endpoint);
            }
        }

        public string Post(XDocument xml)
        {
            using (WebClient wc = new WebClient { Encoding = Encoding.UTF8 })
            {
                wc.Headers.Add("X-WSSE", this.wsse);
                wc.Headers.Add("Content-Type", "application/x.atom+xml");
                return wc.UploadString(this.endpoint, xml.ToString());
            }
        }
    }
}