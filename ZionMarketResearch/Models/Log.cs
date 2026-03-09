using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ZionMarketResearch.Models
{
    public class Log : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!(filterContext.ActionDescriptor.ActionName.ToLower() == "submit") &&
                !(filterContext.ActionDescriptor.ActionName.ToLower() == "submitsamplerequest"))
                return;
            string clientIpAddress = Util.Utility.ClientIPAddress;
            NameValueCollection form = filterContext.HttpContext.Request.Form;

            if (Systems.Log.LogMe(form, clientIpAddress))
            {
                if (filterContext.ActionDescriptor.ActionName.ToLower() == "submitsamplerequest")
                {
                    filterContext.Result = (ActionResult)new JsonResult()
                    {
                        Data = (object)new { Success = true }
                    };
                }
                else
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    Dictionary<string, string> comanTokens = new Dictionary<string, string>(); ;
                    comanTokens.Add("[MRS:ReportTitle]", form["ReportTitle"]);
                    comanTokens.Add("[MRS:CustomerName]", form["Name"]);
                    HttpContext.Current.Session["formMessage"] = Util.Utility.GetHtml(FormRepository.GetHtmlTemplateName(Convert.ToInt32(form["FormType"])), comanTokens);
                    HttpContext.Current.Session[Encoding.UTF8.GetString(Convert.FromBase64String("R29vZ2xlQW5hbHl0aWNz"))] = (object)"DoNotShow";
                    filterContext.Result = (ActionResult)new RedirectToRouteResult("ThankYouRoute", new RouteValueDictionary { { "zn", Util.Utility.Decryptstring(filterContext.HttpContext.Request.Form["ReportID"]) } });
                }
            }
            else
                HttpContext.Current.Session[Encoding.UTF8.GetString(Convert.FromBase64String("R29vZ2xlQW5hbHl0aWNz"))] = (object)null;
        }
    }
}