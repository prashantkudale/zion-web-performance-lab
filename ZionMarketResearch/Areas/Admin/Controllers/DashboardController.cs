using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionAdmin.Repository;
using ZionAdmin.Security;

namespace ZionAdmin.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Admin/Dashboard/
        [ZionAuthorize(Roles = "ListNews")]
        public ActionResult Index()
        {
            return View();
        }

        [ZionAuthorize(Roles = "ListNews")]
        [HttpPost]
        public ActionResult GetPublishedReportCount(DateTime? fromDate, DateTime? toDate)
        {
            return Json(ReportRepository.GetReportCount("published", fromDate, toDate));
        }

        [ZionAuthorize(Roles = "ListNews")]
        [HttpPost]
        public ActionResult GetUpcomingReportCount(DateTime? fromDate, DateTime? toDate)
        {
            return Json(ReportRepository.GetReportCount("upcoming", fromDate, toDate));
        }
    }
}
