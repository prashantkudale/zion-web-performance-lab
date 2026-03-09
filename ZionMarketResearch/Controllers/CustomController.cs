using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Controllers
{
    public class CustomController : Controller
    {
        //
        // GET: /Custom/

        public ActionResult Index(int? id, bool? covid19)
        {
            if (id == null)
                return HttpNotFound();
            var report = ReportRepository.GetReportById(id ?? 0);
            if (report == null)
                return HttpNotFound();
            ViewBag.FormType = (int)constants.GlobalConstant.FormType.CustomRequest;
            ViewBag.ReportId = report.ReportId;
            ViewBag.ReportTitle = report.ReportTitle;
            ViewBag.RequestType = "Custom Request";
            ViewBag.Title = "Request for customization: " + report.ReportTitle.Substring(0, report.ReportTitle.ToLower().IndexOf("market") + 6);
            //ViewBag.ReportId = Util.Utility.Encryptstring(report.ReportId.ToString());
            return View("~/Views/Report/ReportFormNew.cshtml", report);
        }

    }
}
