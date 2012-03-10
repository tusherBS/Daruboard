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
        private PageRepository repository = new PageRepository();

        private static readonly String AppName = ConfigurationManager.AppSettings["App.Name"];
        private static readonly String AppDescription = ConfigurationManager.AppSettings["App.Description"];

        //
        // GET: /Wiki/

        public ActionResult Details(string title)
        {
            var page = repository.Find(title);

            return View(page);
        }

        public ActionResult Feed(string title)
        {
            var pages = repository.List(false);

            var feed = new SyndicationFeed(
                AppName, AppDescription, Request.Url, AppName, DateTime.Now,
                pages.Select(p => {
                    var url = Request.Url.GetLeftPart(UriPartial.Authority)
                            + "/" + p.Title;
                    return new SyndicationItem(
                        p.Title,
                        p.Content.ToString(),
                        new Uri(url),
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
