using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZionMarketResearch.Models.Factory;

namespace ZionMarketResearch.Models
{
    public class RedirectRepository
    {
        private static List<string> routes = new List<string>()
            {
              "custom",
              "requestbrochure",
              "sample",
              "buynow",
              "inquiry",
              "toc",
              "prebook",
              "market-analysis",
              "methodology",
              "requestdiscount",
              "ask-to-analyst",
              "download-toc"
            };

        private static List<string> donotredirectFiles = new List<string> { "jpg", "png", "webp", "js", "css", "scss", "sass", "xml" };

        private static List<string> donotredirectRoute = new List<string> { "bundle" };

        public static string Redirect(string url)
        {
            var segments = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            //Ignore redirection if url is for file
            if (donotredirectFiles.Contains(System.IO.Path.GetExtension(url)) || (segments.Length > 0 ? donotredirectRoute.Contains(segments[0]) : false))
            {
                return null;
            }

            using (ZionDbEntities db = new ZionDbEntities())
            {
                bool InDB = false;
                string[] strArray1 = url.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                string[] strArray2 = new string[] { };
                if (strArray1.Length == 0)
                    return (string)null;
                string str;
                if (url.EndsWith(".css") || url.EndsWith(".js") || url.EndsWith(".png") || url.EndsWith(".jpg") || url.EndsWith(".webp"))
                    return null;
                if (!RedirectRepository.routes.Contains(strArray1[0].ToLower()))
                {
                    str = db.tblredirections.Where(z => z.OldUrl == url && z.IsActive == true).Select(x => x.NewUrl).FirstOrDefault();
                    InDB = str != null ? true : false;
                }
                else
                {
                    string endingUrl = "/" + strArray1[strArray1.Length - 1];
                    
                    //Origial code
                    //str = db.tblredirections.Where(z => (z.OldUrl == url || z.OldUrl.EndsWith(endingUrl) && !z.OldUrl.Contains("/news/") && z.IsActive == true)).Select(x => x.NewUrl).FirstOrDefault();
                    str = db.tblredirections.Where(z => (z.OldUrl == url 
                                                    || (z.OldUrl.StartsWith("/report/") && z.OldUrl.EndsWith(endingUrl)) 
                                                    && !z.OldUrl.Contains("/news/") 
                                                    && z.IsActive == true)
                                                    ).Select(x => x.NewUrl).FirstOrDefault();
                    InDB = false;
                }

                if (!InDB && !string.IsNullOrEmpty(str))
                {
                    strArray2 = str.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    str = strArray1[0].ToLower() == "buynow" || strArray1[0].ToLower() == "prebook" ? "/" + strArray1[0] + "/su/" + strArray2[strArray2.Length - 1] : str;
                    if (str == url)
                        return (string)null;
                }

                if (!InDB && string.IsNullOrEmpty(str))
                {
                    string redirectUrl = null;
                    if (strArray1[0] != "report" && strArray1[0] != "news" && strArray1[0] != "article" && !RedirectRepository.routes.Contains(strArray1[0].ToLower()))
                    {
                        MainPageReportView report = ReportFactory.GetReportByUrl(strArray1[strArray1.Length - 1].ToLower());
                        if (report != null)
                            return "/report/" + strArray1[strArray1.Length - 1].ToLower();
                    }
                    redirectUrl = db.tblredirections.Where(z => z.OldUrl == url && z.IsActive == true).Select(z => z.NewUrl).FirstOrDefault();
                    if (redirectUrl != null)
                        return redirectUrl;
                }
                if (!InDB && str!= null && str.StartsWith("/report/") && !url.StartsWith("/report/") && strArray2.Length > 1)
                {
                    str = "/" + strArray1[0] + "/" + strArray2[1];
                }
                return str;
            }
        }
    }
}