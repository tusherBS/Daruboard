using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Configuration;

namespace Daruyanagi
{
    // メモ: IIS6 または IIS7 のクラシック モードの詳細については、
    // http://go.microsoft.com/?LinkId=9394801 を参照してください

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute(
                "Archives",
                "Archives",
                new { controller = "Page", action = "Index", }
            );

            routes.MapRoute(
                "Feed",
                "Feed",
                new { controller = "Wiki", action = "Feed", }
            );

            routes.MapRoute(
                "Wiki",
                "{title}",
                new { controller = "Wiki", action = "Details", title = "Home" }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Page", action = "Index", id = UrlParameter.Optional }
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            HatenaFotolifeHelper.User = ConfigurationManager.AppSettings["Hatena.Id"];
            HatenaFotolifeHelper.Pass = ConfigurationManager.AppSettings["Hatena.Password"];

            AmazonHelper.AccessKeyId = ConfigurationManager.AppSettings["Amazon.AccessKeyId"];
            AmazonHelper.SecretKey = ConfigurationManager.AppSettings["Amazon.SecretKey"];
        }
    }
}