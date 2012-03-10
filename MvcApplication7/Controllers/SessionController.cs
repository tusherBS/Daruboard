using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Security;
using System.Configuration;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;

namespace Daruyanagi.Controllers
{
    public class SessionController : Controller
    {
        string app_key = ConfigurationManager.AppSettings["Dropbox.AppKey"];
        string app_secret = ConfigurationManager.AppSettings["Dropbox.AppSecret"];

        //
        // GET: /Session/Create -> map route /Logon

        public ActionResult Create()
        {
            // 0. load the config
            var config = DropBoxConfiguration.GetStandardConfiguration();
            config.AuthorizationCallBack = 
                new Uri(Request.Url, "AuthorizationCallBack");

            // 1. get the request token from dropbox
            DropBoxRequestToken requestToken = DropBoxStorageProviderTools
                .GetDropBoxRequestToken(config, app_key, app_secret);

            // 2. build the authorization url based on request token                        
            string url = DropBoxStorageProviderTools
                .GetDropBoxAuthorizationUrl(config, requestToken);

            // 3. Redirect to the authorization page on dropbox
            return Redirect(url);

        }

        public ActionResult AuthorizationCallBack()
        {
            // 4. Get oauth token and uid from Request.Form[]
            var oauth_token = Request["oauth_token"];
            var uid = Request["uid"];

            // 5. Set auth cookie
            if (oauth_token != null && uid != null)
            {
                using (var dropbox = new DropBox())
                {
                    var info = DropBoxStorageProviderTools
                        .GetAccountInformation(dropbox.AccessToken);

                    if (info.UserId == int.Parse(uid))
                        FormsAuthentication.SetAuthCookie(info.DisplayName, true);
                }
            }

            return Redirect("/");
        }

        //
        // GET: /Session/Delete -> map route /Logoff

        public ActionResult Delete()
        {
            FormsAuthentication.SignOut();

            return Redirect("/");
        }
    }
}
