using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Codeplex.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Daruyanagi
{
    public static class TwitterHelper
    {
        private static string API_ENDPOINT = string.Format(
            "{0}{1}{2}?id={{0}}",
            "https://api.twitter.com/", // Twitter Domain
            "1",                        // Twitter API Version
            "/statuses/oembed.json"     // Embedded Tweet Endpoint
            );

        public static string GetHtml(string tweet_url)
        {
            try
            {
                var tweet_id = new Regex(@"\d+")
                    .Matches(tweet_url)
                    .Cast<Match>()
                    .Last();

                return DynamicJson.Parse(
                    new System.Net.WebClient().DownloadString(
                        string.Format(API_ENDPOINT, tweet_id)
                    )
                ).html;
            }
            catch (HttpException http_exception)
            {
                switch (http_exception.ErrorCode)
                {
                    case 403:
                        return "twitter: 指定されたツイートは非公開です";
                    case 404:
                        return "twitter: 指定されたツイートは存在しないか削除されています";
                    default:
                        return "twitter: ネットワークエラーが発生しました";
                }
            }
            catch (Exception exception)
            {
                return "twitter: " + exception.Message;
            }
        }

        public static MvcHtmlString Render(string tweet_url)
        {
            return new MvcHtmlString(GetHtml(tweet_url));
        }
    }
}
