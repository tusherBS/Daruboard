using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Text.RegularExpressions;
using MarkdownSharp;
using AppLimit.CloudComputing.SharpBox;

namespace Daruyanagi.Models
{
    public class Page
    {
        private Page()
        {

        }

        public Page(ICloudFileSystemEntry entry)
        {
            Name = entry.Name;
            Length = entry.Length;
            Modified = entry.Modified;
        }

        public string Name { get; set; }

        public string Title
        {
            get
            {
                return Path.GetFileNameWithoutExtension(Name);
            }
        }

        public PageType PageType
        {
            get
            {
                switch (Path.GetExtension(Name).ToLower())
                {
                    case ".md":
                    case ".markdown":
                        return PageType.Markdown;

                    case ".htm":
                    case ".html":
                        return PageType.Html;

                    default:
                        return PageType.Unknown;
                }
            }
        }

        private static Markdown markdown = new Markdown();
        private HtmlString content_cache = null;
        private DateTime content_cache_at = DateTime.Now;

        public HtmlString Content
        {
            get
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated ||
                    content_cache == null ||
                    content_cache_at.AddMinutes(5) < DateTime.Now)
                {
                    using (var dropbox = new DropBox())
                    {
                        var remote_file = string.Format("/Archives/{0}", Name);

                        var local_dir = Path.GetTempPath();
                        dropbox.Storage.DownloadFile(remote_file, local_dir);

                        var local_file = Path.Combine(local_dir, Name);
                        var text = File.ReadAllText(local_file);

                        switch (PageType)
                        {
                            case PageType.Markdown:
                                text = ProcessFootnotes(text);
                                text = ProcessBlocks(text);
                                text = markdown.Transform(text);
                                
                                content_cache = new HtmlString(text);
                                content_cache_at = DateTime.Now;
                                break;

                            case PageType.Html:
                                text = ProcessBlocks(text);

                                content_cache = new HtmlString(text);
                                content_cache_at = DateTime.Now;
                                break;

                            default:
                                content_cache = null;
                                throw new Exception("コンテンツタイプが不明です。");
                        }
                    }
                }
                return content_cache;
            }
        }

        public DateTime Modified { get; set; }

        public long Length { get; set; }

        public bool IsDraft
        {
            get { return Title.StartsWith("_"); }
        }

        //---

        private static readonly Regex R_BLOCKS = new Regex(@"
                \[\[
                    (?<params>[^\[\]]+) # params  = $2; can't contain [ or ]
                \]\]
            ",
            RegexOptions.Singleline |
            RegexOptions.IgnorePatternWhitespace |
            RegexOptions.Compiled
        );

        private const string E_PROCESS_BLOCKS = "<p class='error'>{0}</p>";

        public static string ProcessBlocks(string text)
        {
            return string.IsNullOrEmpty(text)
                ? text
                : R_BLOCKS.Replace(text, new MatchEvaluator(ProcessBlocksEvaluator));
        }

        private static string ProcessBlocksEvaluator(Match match)
        {
            string[] p = match.Groups["params"].Value
                .Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                switch (p[0].ToLower())
                {
                    case "twitter":
                        return TwitterHelper.GetHtml(p[1]);

                    case "fotolife":
                        return HatenaFotolifeHelper.GetHtml(p[1]);

                    case "flickr":
                        switch (p.Length)
                        {
                            case 2:
                                return FlickrHelper.GetHtml(p[1]);
                            case 3:
                                return FlickrHelper.GetHtml(p[1], p[2]);
                            case 4:
                                return FlickrHelper.GetHtml(p[1], p[2], p[3]);
                            default:
                                throw new ArgumentException("flickr: params are too long/short.");
                        }

                    case "youtube":
                        switch (p.Length)
                        {
                            case 2:
                                return YouTubeHelper.GetHtml(p[1]);
                            case 3:
                                return YouTubeHelper.GetHtml(p[1], p[2]);
                            case 4:
                                return YouTubeHelper.GetHtml(p[1], p[2], p[3]);
                            default:
                                throw new ArgumentException("Youtube: params are too long/short.");
                        }

                    case "amazon":
                        return AmazonHelper.GetHtml(p[1]);

                    case "feed":
                        switch (p.Length)
                        {
                            case 2:
                                return FeedHelper.GetHtml(p[1]);
                            case 3:
                                return FeedHelper.GetHtml(p[1], p[2]);
                            default:
                                throw new ArgumentException("feed: params are too long/short.");
                        }

                    case "updates":
                        var pages = new PageRepository().List(false)
                            .OrderByDescending(_ => _.Modified)
                            .Take((p.Count() < 3) ? 10 : int.Parse(p[2]));

                        var tag = string.Join("\r\n", pages.Select(_ =>
                            string.Format(
                                "<li>{0}<a href='/{1}'>{1}</a> ({2})</li>",
                                _.Modified.AddDays(1) > DateTime.Now
                                    ? "<span class='label label-success'>New</span>"
                                    : string.Empty,
                                _.Title, _.Modified
                            )
                        ));
                        return string.Format("<ul class='updates'>{0}</ul>", tag);

                    case "label":
                        var label_text = (p.Count() < 2) ? "Default" : p[1];
                        var label_type = (p.Count() < 3) ? "normal" : p[2];
                        return string.Format(
                            "<span class='label label-{1}'>{0}</span>",
                            label_text, label_type);

                    case "new":
                        return "<span class='label label-success'>New</span>";

                    case "warning":
                        return "<span class='label label-warning'>Warning</span>";

                    case "important":
                        return "<span class='label label-important'>Important</span>";

                    case "notice":
                        return "<span class='label label-notice'>Notice</span>";

                    default:
                        string url = p[0];
                        string title = p.Count() > 1 ? p[1] : url;
                        return string.Format("[{0}]({1})", title, url);
                }
            }
            catch (Exception e)
            {
                return string.Format(E_PROCESS_BLOCKS, e.Message);
            }
        }

        private static readonly Regex R_FOOTNOTES = new Regex(@"
                \(\(
                    (?<value>[^\(\)]+)  # params  = $2; can't contain ( or )
                \)\)
            ",
            RegexOptions.Singleline |
            RegexOptions.IgnorePatternWhitespace |
            RegexOptions.Compiled
        );

        public static string ProcessFootnotes(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var matches = R_FOOTNOTES.Matches(text);
            var footnotes = new List<string>();

            foreach (Match m in matches)
            {
                footnotes.Add(string.Format(
                    "<li id='footnote-{0}'>{1} (<a href='#note-{0}'>*</a>)</li>",
                    m.Index, m.Groups["value"].ToString()));
            }

            var footnote = "<ol class='post-footnote'>\r\n" + string.Join("\r\n", footnotes) + "</ol>";

            var index = 0;

            text = R_FOOTNOTES.Replace(text, delegate(Match match)
            {
                return string.Format(
                    "<sup id='note-{0}' class='note'><a href='#footnote-{0}' title='{1}'>[{2}]</a></sup>",
                    match.Index, match.Groups["value"].ToString(), ++index);
            });

            text += "\r\n";
            text += "\r\n";
            text += footnote;
            text += "\r\n";

            return text;
        }
    }

    public enum PageType
    {
        Unknown,
        Html,
        Markdown,
    }
}