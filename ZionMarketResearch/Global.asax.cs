using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ZionMarketResearch.App_Start;

namespace ZionMarketResearch
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //enabel only Razor View
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            #region Enable loging Error and Exceptions
            //Enable loging
            var log4NetPath = Server.MapPath("~/log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(log4NetPath));
            #endregion

            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            #region IP block
            string BlockIPs = System.Configuration.ConfigurationManager.AppSettings["BlockedIPs"].ToString();
            string UserIP = GetIPAddress();
            if (Request.Path != "/")
            {
                for (int i = 0; i < BlockIPs.Split(',').Length; i++)
                {
                    if (UserIP == BlockIPs.Split(',')[i].ToString().Trim())
                        Response.Redirect("/");
                }
            }
            #endregion;

            string _domain = !this.Request.IsLocal ? "https://www.zionmarketresearch.com" : "http://localhost:16687";
            int num = this.Request.Url.AbsolutePath.Contains("%20") ? 1 : (this.Request.Url.AbsolutePath.Contains(" ") ? 1 : 0);
            string url = this.Request.Url.AbsolutePath.Replace("%20", "-").Replace(" ", "-");
            num = url.Contains("--") || num == 1 ? 1 : 0;
            url = url.Contains("--") ? url.Replace("--", "-") : url;
            if (num != 0)
                this.Response.RedirectPermanent(url);
            if (url.EndsWith("/news") && !url.Contains("/admin/"))
                this.Response.RedirectPermanent(_domain + url.Substring(0, url.IndexOf("/news")));
            if (url.ToLower().EndsWith("marketutm") && !url.Contains("/admin/"))
            {
                this.Response.RedirectPermanent(_domain + url.ToLower().Replace("marketutm", "market") + this.Request.Url.Query);
            }
            string str = !new HttpRequestWrapper(this.Request).IsAjaxRequest() ? Models.RedirectRepository.Redirect(url) : (string)null;
            //string str = url;
            if (!string.IsNullOrEmpty(str))
            {
                //if (!this.Request.IsLocal)
                    this.Response.RedirectPermanent(_domain + str + this.Request.Url.Query);
                //else
                  //  this.Response.RedirectPermanent(string.Format("http://{0}:{1}", (object)this.Request.Url.Host, (object)this.Request.Url.Port) + str + this.Request.Url.Query);
            }
            // 
            if (this.Request.Url.ToString().Contains("localhost") || !(this.Request.Url.Scheme == "http") && this.Request.Url.ToString().Contains("www.") || !Convert.ToBoolean(ConfigurationManager.AppSettings["SSL"]))
                return;
            
            this.Response.RedirectPermanent(_domain + url);
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["DiscountPercentage"] != null
                && Convert.ToDecimal(ConfigurationManager.AppSettings["DiscountPercentage"]) > 0
                && HttpContext.Current.Session != null)
                HttpContext.Current.Session["Discount"] = "discount";
        }

        public string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}