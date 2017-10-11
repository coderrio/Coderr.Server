using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using codeRR.Server.Infrastructure.Security;

namespace codeRR.Server.Web.Views
{
    /// <summary>
    ///     moves some of the logic from the layout to this class instead.
    /// </summary>
    public abstract class BaseViewPage : WebViewPage
    {
        public string ApiUrl { get; set; }

        public Dictionary<string, string> Applications
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                    return new Dictionary<string, string>();

                var identity = (ClaimsIdentity) User.Identity;
                var dict = new Dictionary<string, string>();
                var ids = identity.FindAll(x => x.Type == CoderrClaims.Application).ToList();
                var names = identity.FindAll(x => x.Type == CoderrClaims.ApplicationName).ToList();
                for (var i = 0; i < ids.Count; i++)
                    dict[ids[i].Value] = names[i].Value;
                return dict;
            }
        }

        public bool CanStartSpa { get; set; }

        public static string LoginUrl { get; set; }

        public void Init()
        {
            var root = Url.Content("~/");
            var currentUrl = Request.Url.AbsolutePath;
            if (!currentUrl.EndsWith("/"))
                currentUrl += "/";
            CanStartSpa = currentUrl.Equals(root, StringComparison.OrdinalIgnoreCase);
            ApiUrl = new Uri(Request.Url, VirtualPathUtility.ToAbsolute("~/")).ToString();
        }

        public override void InitHelpers()
        {
            base.InitHelpers();
            Init();
        }
    }

    public abstract class BaseViewPage<T> : WebViewPage<T>
    {
        public string ApiUrl { get; set; }

        public Dictionary<string, string> Applications
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                    return new Dictionary<string, string>();


                var identity = (ClaimsIdentity) User.Identity;
                var dict = new Dictionary<string, string>();
                var ids = identity.FindAll(x => x.Type == CoderrClaims.Application).ToList();
                var names = identity.FindAll(x => x.Type == CoderrClaims.ApplicationName).ToList();
                var count = Math.Min(ids.Count, names.Count);
                for (var i = 0; i < count; i++)
                    dict[ids[i].Value] = names[i].Value;
                return dict;
            }
        }

        public bool CanStartSpa { get; set; }

        public string LoginUrl
        {
            get { return BaseViewPage.LoginUrl; }
        }

        public void Init()
        {
            var root = Url.Content("~/");
            var currentUrl = Request.Url.AbsolutePath;
            if (!currentUrl.EndsWith("/"))
                currentUrl += "/";
            CanStartSpa = currentUrl.Equals(root, StringComparison.OrdinalIgnoreCase);
            ApiUrl = new Uri(Request.Url, VirtualPathUtility.ToAbsolute("~/")).ToString();
        }

        public override void InitHelpers()
        {
            base.InitHelpers();
            Init();
        }
    }
}