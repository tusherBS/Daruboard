using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using AppLimit.CloudComputing.SharpBox;
using System.Timers;

namespace Daruyanagi.Models
{
    public static class PageRepository
    {
        private static Timer mTimer = new Timer(5 * 60 * 1000);
        private static List<Page> pages = GetPageList();
        private static DateTime UpdatedAt = DateTime.Now;

        static PageRepository()
        {
            mTimer.Elapsed += new ElapsedEventHandler(mTimer_Elapsed);
            mTimer.Start();
        }

        static void mTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            pages = GetPageList();
            UpdatedAt = DateTime.Now;
        }

        static List<Page> GetPageList()
        {
            using (var dropbox = new DropBox())
            {
                return dropbox["/Archives"]
                    .Where(_ => _ is ICloudFileSystemEntry)
                    .OrderByDescending(_ => _.Modified)
                    .Select(_ => new Page(_))
                    .ToList();
            }
        }

        public static Page Find(string title)
        {
            return Find(title, HttpContext.Current.User.Identity.IsAuthenticated);
        }

        public static Page Find(string title, bool include_draft = false)
        {
            var pages = List(include_draft)
                .Where(p => p.Title == title)
                .Where(p => p.PageType != PageType.Unknown)
                .OrderBy(p => p.Name);

            if (pages.Count() < 1)
                throw new HttpException(404, "Page is not found.");
            else
                return pages.First();
        }

        public static List<Page> List()
        {
            return List(HttpContext.Current.User.Identity.IsAuthenticated);
        }

        public static List<Page> List(bool include_draft = false)
        {
            if (include_draft)
                return pages;
            else
                return pages.Where(p => !p.IsDraft).ToList();
        }
    }
}