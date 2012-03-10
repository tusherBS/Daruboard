using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using AppLimit.CloudComputing.SharpBox;

namespace Daruyanagi.Models
{
    public class PageRepository
    {
        public Page Find(string title)
        {
            return Find(title, HttpContext.Current.User.Identity.IsAuthenticated);
        }

        public Page Find(string title, bool include_draft = false)
        {
            var pages = List(include_draft)
                .Where(p => p.Title == title)
                .Where(p => p.PageType != PageType.Unknown)
                .OrderBy(p => p.Name);

            if (pages.Count() < 1)
                throw new FileNotFoundException();
            else
                return pages.First();
        }

        private static List<Page> list_cache = null;
        private static DateTime list_cached_at = DateTime.Now;

        public List<Page> List()
        {
            return List(
                HttpContext.Current.User.Identity.IsAuthenticated,
                !HttpContext.Current.User.Identity.IsAuthenticated);
        }

        public List<Page> List(bool include_draft = false, bool cache_enabled = true)
        {
            if (cache_enabled == false || 
                list_cache == null || 
                list_cached_at.AddMinutes(5) < DateTime.Now)
            {
                using (var dropbox = new DropBox())
                {
                    var archives = dropbox["/Archives"];

                    list_cache = archives
                        .Where(_ => _ is ICloudFileSystemEntry)
                        .OrderByDescending(_ => _.Modified)
                        .Select(_ => new Page(_))
                        .ToList();
                    list_cached_at = DateTime.Now;
                }
            }

            if (include_draft)
                return list_cache;
            else
                return list_cache.Where(p => !p.IsDraft).ToList();
        }
    }
}