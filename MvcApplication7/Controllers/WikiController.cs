using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Daruyanagi.Models;
using System.Xml;
using System.Configuration;
using System.ServiceModel.Syndication;

namespace Daruyanagi.Controllers
{
    public class WikiController : Controller
    {
        private static readonly String AppName = ConfigurationManager.AppSettings["App.Name"];
        private static readonly String AppDescription = ConfigurationManager.AppSettings["App.Description"];

        //
        // GET: /Wiki/

        public ActionResult Details(string title)
        {
            var page = PageRepository.Find(title);

            return View(page);
        }

        public ActionResult Feed(string title, int count = 10)
        {
            var pages = PageRepository.List().Take(count);

            var feed = new SyndicationFeed(
                AppName, AppDescription, Request.Url, AppName, DateTime.Now,
                pages.Select(p => {
                    var url = MvcApplication.Domain + "/" + p.Title;
                    return new SyndicationItem(
                        p.Title,
                        p.Content.Body.ToString(),
                        new Uri(MvcApplication.Domain + "/" + p.Title),
                        url,
                        p.Modified
                    );
                })
            );

            return new RssActionResult(feed);
        }
    }

    public class RssActionResult : ActionResult
    {
        public SyndicationFeed Feed { get; private set; }

        private RssActionResult()
        {

        }

        public RssActionResult(SyndicationFeed feed)
        {
            Feed = feed;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/rss+xml";

            using (var writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                new Rss20FeedFormatter(Feed).WriteTo(writer);
            }
        }
    }
}
