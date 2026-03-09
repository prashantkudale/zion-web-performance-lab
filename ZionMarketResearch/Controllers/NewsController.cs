using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Controllers
{
    public class NewsController : Controller
    {
        //
        // GET: /News/

        public ActionResult Index(string url)
        {
            var news = NewsRepository.GetNewsByUrl(url);
            if (news == null)
                return HttpNotFound();

            ViewBag.ReportId = Util.Utility.Encryptstring(news.ReportId.ToString());
            ViewBag.css = "news_css.css";
            if (news.Description.Contains("<img") && news.Description.Contains("src=\"") && news.Description.Length > news.Description.IndexOf("src=\"") + 6)
            {
                if (news.Description.Contains("ogImageClass")) { }
                string _tempOGImagePath = news.Description.Substring(news.Description.IndexOf("src=\"") + 5);
                _tempOGImagePath = _tempOGImagePath.Substring(0, _tempOGImagePath.IndexOf("\""));
                ViewBag.Firstimage = _tempOGImagePath;
            }
            return View(news);
        }
        [OutputCache(Duration = 3600)]
        public ActionResult LatestNews()
        {
            return PartialView(NewsRepository.LatestNews());
        }

        [OutputCache(Duration = 3600)]
        public ActionResult LatestNewsOnHome()
        {
            return PartialView(NewsRepository.LatestNews());
        }

        public ActionResult AllNews(int? page)
        {
            if (page != null && page <= 0)
                return HttpNotFound("Page number should not be less than or eqaul to zero.");
            var allNews = NewsRepository.AllNews(page);
            if (allNews == null)
                return RedirectToAction("AllNews", new { page = default(int?) });
            return View(allNews);
        }

        public ActionResult JsonAllNews(int? page = 1)
        {
            var allNews = NewsRepository.AllNews(page);
            return Json(allNews.Select(x => {
                x.Description = Util.Utility.StripHTML(x.Description).Substring(0, 300);
                return x;
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}
