using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.Extension;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Controllers
{
    public class CategoryController : Controller
    {
        //
        // GET: /Category/

        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(Duration = 3600)]
        public ActionResult GetCategoryWithReportCount()
        {
            ViewBag.css = "reportslist_css.css";
            return PartialView(CategoryRepository.GetCategoryWithReportCount());
        }

        public ActionResult CategoryWithReports(string url, int? page)
        {
            if (page != null && page <= 0)
                return HttpNotFound("Page should be greater than zero.");
            var categoryWithReports = CategoryRepository.GetCategoryWithReports(url, page);
            if (categoryWithReports != null)
            {
                ViewBag.css = "reportslist_css.css";
                return View(categoryWithReports);
            }
            return HttpNotFound();
        }

        public ActionResult CategoryReports(string url, int? page, int? maxRows) => (ActionResult)this.Json((object)CategoryRepository.GetReportsByCategory(url, page, maxRows ?? 20).Select<SearchResultView, SearchResultView>((Func<SearchResultView, SearchResultView>)(x =>
        {
            x.Description = Util.Utility.StripHTML(x.Description).ZSubstring(0, 299);
            return x;
        })).ToList<SearchResultView>(), JsonRequestBehavior.AllowGet);

        public ActionResult CategoryPublishedReports(string url, int? page, int? maxRows) => (ActionResult)this.Json((object)CategoryRepository.GetPublishedReportsByCategory(url, page, maxRows ?? 20).Select<SearchResultView, SearchResultView>((Func<SearchResultView, SearchResultView>)(x =>
        {
            x.Description = Util.Utility.StripHTML(x.Description).ZSubstring(0, 299);
            return x;
        })).ToList<SearchResultView>(), JsonRequestBehavior.AllowGet);

        public ActionResult CategoryUpcomingReports(string url, int? page, int? maxRows) => (ActionResult)this.Json((object)CategoryRepository.GetUpcomingReportsByCategory(url, page, maxRows ?? 20).Select<SearchResultView, SearchResultView>((Func<SearchResultView, SearchResultView>)(x =>
        {
            x.Description = Util.Utility.StripHTML(x.Description).ZSubstring(0, 299);
            return x;
        })).ToList<SearchResultView>(), JsonRequestBehavior.AllowGet);

        public ActionResult GetAllParentCategories() => (ActionResult)this.Json((object)CategoryRepository.GetParentCategory(), JsonRequestBehavior.AllowGet);

    }
}
