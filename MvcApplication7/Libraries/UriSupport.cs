using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;

namespace Daruyanagi
{
    public static class UriSupport
    {
        public static Uri GetResponseUri(this Uri input)
        {
            var req = WebRequest.Create(input) as HttpWebRequest;
            WebResponse res = req.GetResponse();
            return res.ResponseUri;
        }
    }
}