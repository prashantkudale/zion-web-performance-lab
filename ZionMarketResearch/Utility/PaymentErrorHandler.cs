using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Utility
{
    public class PaymentErrorHandler : HandleErrorAttribute
    {
        public Type ModelType { get; set; }
        public override void OnException(ExceptionContext filterContext)
        {
            filterContext.HttpContext.Request.InputStream.Position = 0;
            var sb = new StringBuilder();
            sb.Append("<table>");
            using (var sr = new StreamReader(filterContext.HttpContext.Request.InputStream))
            {
                var body = sr.ReadToEnd();

                #region Just Get Model
                if (ModelType != null)
                {
                    var output = JsonConvert.DeserializeObject(body, ModelType);

                    foreach (var p in output.GetType().GetProperties())
                    {
                        sb.Append($"<tr><td>{p.Name}</td><td>{p.GetValue(output)}</td></tr>");
                    }
                }
                else
                {
                    var output = JsonConvert.DeserializeObject<dynamic>(body);
                    foreach (Newtonsoft.Json.Linq.JProperty p in output.Properties())
                    {
                        sb.Append($"<tr><td>{p.Name}</td><td>{p.Value}</td></tr>");
                    }
                }
                #endregion
            }
            sb.Append("</table>");

            var controller = filterContext.RouteData.Values["controller"];
            var action = filterContext.RouteData.Values["action"];
            var stackTrace = filterContext.Exception.StackTrace;
            var exceptionMessage = filterContext.Exception.Message;
            var innerExceptionMessage = filterContext.Exception.InnerException != null && !string.IsNullOrEmpty(filterContext.Exception.InnerException.Message) ? filterContext.Exception.InnerException.Message : null;
            var innserExceptionStackTrace = filterContext.Exception.InnerException != null && !string.IsNullOrEmpty(filterContext.Exception.InnerException.StackTrace) ? filterContext.Exception.InnerException.StackTrace : null;

            var url = filterContext.HttpContext.Request.Url;
            SendMail.Send("ms202301182200@gmail.com", "Zion Market Research | Error On BuyNow", $"<b>Controller</b>{controller.ToString()} <b>Action</b>{action.ToString()}<br/><br/><b>URL</b>{url}<br/><br/><b>Posted Params</b> {sb.ToString()}", true);
            SendMail.SendMailToZion("Zion Market Research - Payment Initiated But Has Error", sb.ToString(), "payment");
            base.OnException(filterContext);
        }
    }
}