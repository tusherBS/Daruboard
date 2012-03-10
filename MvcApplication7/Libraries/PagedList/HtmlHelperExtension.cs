using System.Web.Mvc;
using System.Web.UI;

namespace Daruyanagi
{
    public static class HtmlHelperExtention
    {
        /// <summary>
        /// Shows a pager control - Creates a list of links that jump to each page
        /// </summary>
        /// <param name="helper">The ViewPage instance this method executes on.</param>
        /// <param name="pagedList">A PagedList instance containing the data for the paged control</param>
        /// <param name="controller">Name of the controller.</param>
        /// <param name="action">Name of the action on the controller.</param>

        public static MvcHtmlString RenderPagenation(this HtmlHelper helper, IPagedList paged_list,
            string controller, string action)
        {
            return RenderPagenation(helper, paged_list, controller, action, new { query = "" });
        }

        public static MvcHtmlString RenderPagenation(this HtmlHelper helper, IPagedList paged_list,
            string controller, string action, object routeValues)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            var url = urlHelper.Action(action, controller, routeValues);

            var div = new TagBuilder("div");
            div.MergeAttribute("class", "pagination");
            {
                var ul = new TagBuilder("ul");
                {
                    var prev = new TagBuilder("li");
                    {
                        if (paged_list.CurrentPage <= 1)
                        {
                            prev.MergeAttribute("class", "prev disabled");

                            var a = new TagBuilder("a");
                            {
                                a.MergeAttribute("href", "#");
                                a.InnerHtml = "&larr; Previous";
                            }

                            prev.InnerHtml = a.ToString();
                        }
                        else
                        {
                            prev.MergeAttribute("class", "prev");

                            var a = new TagBuilder("a");
                            {
                                var format = url.Contains("?")
                                    ? "{0}&current={1}&items_per_page={2}"
                                    : "{0}?current={1}&items_per_page={2}";

                                a.MergeAttribute("href", string.Format(
                                    format, url,
                                    paged_list.CurrentPage - 1, paged_list.ItemsPerPage));
                                a.InnerHtml = "&larr; Previous";
                            }
                            prev.InnerHtml = a.ToString();
                        }
                    }
                    ul.InnerHtml += prev.ToString();
                }

                for (int i = 1; i <= paged_list.TotalPages; i++)
                {
                    var li = new TagBuilder("li");
                    {
                        var a = new TagBuilder("a");
                        {
                            var format = url.Contains("?")
                                ? "{0}&current={1}&items_per_page={2}"
                                : "{0}?current={1}&items_per_page={2}";

                            a.MergeAttribute("href", string.Format(
                                format, url, i, paged_list.ItemsPerPage));
                            a.InnerHtml = i.ToString();
                        }
                        li.InnerHtml = a.ToString();
                    }
                    ul.InnerHtml += li.ToString();
                }

                {
                    var next = new TagBuilder("li");
                    {
                        if (paged_list.CurrentPage >= paged_list.TotalPages)
                        {
                            next.MergeAttribute("class", "next disabled");

                            var a = new TagBuilder("a");
                            {
                                a.MergeAttribute("href", "#");
                                a.InnerHtml = "Next &rarr;";
                            }
                            next.InnerHtml = a.ToString();
                        }
                        else
                        {
                            next.MergeAttribute("class", "next");

                            var a = new TagBuilder("a");
                            {
                                var format = url.Contains("?")
                                    ? "{0}&current={1}&items_per_page={2}"
                                    : "{0}?current={1}&items_per_page={2}";

                                a.MergeAttribute("href", string.Format(
                                    format, url,
                                    paged_list.CurrentPage + 1, paged_list.ItemsPerPage));
                                a.InnerHtml = "Next &rarr;";
                            }
                            next.InnerHtml = a.ToString();
                        }
                    }
                    ul.InnerHtml += next.ToString();
                }
                div.InnerHtml = ul.ToString();
            }

            return new MvcHtmlString(div.ToString());
        }
    }
}