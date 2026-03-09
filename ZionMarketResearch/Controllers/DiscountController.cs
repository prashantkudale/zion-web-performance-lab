using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Controllers
{
    public class DiscountController : Controller
    {
        //
        // GET: /Discount/

        public ActionResult Report(string url)
        {
            if (string.IsNullOrEmpty(url))
                return HttpNotFound("URL for report should not be empty or null.");
            MainPageReportView report = ReportRepository.GetReportByUrl(url);
            if (report == null || string.IsNullOrEmpty(report.ReportTitle))
                return HttpNotFound("Report URL not found. Please contact to https://zionmarketresearch.com for more details.");
            report.TOCUrl = string.Format("/discount/report/{0}", report.TOCUrl);
            ViewBag.ReportId = Util.Utility.Encryptstring(report.ReportId.ToString());
            ViewBag.ReportUrl = Util.Utility.Encryptstring(report.ReportUrl);
            ViewBag.DeliveryFormat = report.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : report.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : report.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : report.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail";

            #region Meta Data for page
            string[] source = Request.RawUrl.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            ZionMarketResearch.Models.MetaData m = source[0] != "report" ? ZionMarketResearch.Models.MetaData.GetMetaData(report, source[0] == "market-analysis" ? "freeanalysis" : source[0]) : null;
            ViewBag.Title = m != null && !string.IsNullOrEmpty(m.Title) ? m.Title : (!string.IsNullOrEmpty(report.MetaTitle) ? report.MetaTitle : report.ReportTitle);
            ViewBag.MetaDescription = m != null && !string.IsNullOrEmpty(m.Description) ? m.Description : report.MetaDescription;
            ViewBag.MetaKeywords = m != null && !string.IsNullOrEmpty(m.Keywords) ? m.Keywords : report.MetaKeyword;
            #endregion
            return View("Report", report);
        }

        public ActionResult Newsletter()
        {
            return View();
        }

        public ActionResult YearEndSale()
        {
            //return View();
            //Response.RedirectPermanent("/");
            return Redirect("/");
        }

        public ActionResult BlackFriday()
        {
            return View();
        }

        public ActionResult YearEnd()
        {
            return View();
        }
    }
}
