using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionAdmin.Repository;
using ZionAdmin.Security;
using ZionMarketResearch.Log;

namespace ZionAdmin.Controllers
{
    [LogException]
    public class NewsController : Controller
    {
        //
        // GET: /News/
        [ZionAuthorize(Roles = "ListNews")]
        public ActionResult Index(bool? showNews, bool? showArticle, int? id)
        {
            ViewBag.IsNews = showNews ?? false;
            ViewBag.IsArticle = showArticle ?? false;
            return View(NewsRepository.List(showNews, showArticle, id));
        }
        [ZionAuthorize(Roles = "CreateNews")]
        public ActionResult Create()
        {
            return View();
        }

        [ZionAuthorize(Roles = "CreateNews")]
        [HttpPost]
        public ActionResult Create(NewsRepository news)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ErrorMessage = NewsRepository.Create(news, Request.Files);
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = "true";
            return View(news);
        }
        [ZionAuthorize(Roles = "EditNews")]
        public ActionResult Edit(int id)
        {
            return View(NewsRepository.Get(id));
        }
        [ZionAuthorize(Roles = "EditNews")]
        [HttpPost]
        public ActionResult Edit(NewsRepository news)
        {
            if (ModelState.IsValid)
            {
                NewsRepository.Update(news, Request.Files);
            }
            ViewBag.ErrorMessage = "true";
            return RedirectToAction("Index");
        }
        [ZionAuthorize(Roles = "DeleteNews")]
        public ActionResult Delete(int id)
        {
            NewsRepository.Delete(id, 0);
            return RedirectToAction("Index");
        }
        [ZionAuthorize(Roles = "SearchNews")]
        public ActionResult Search(string id, int? page, bool? showNews, bool? showArticle)
        {
            ViewBag.SearchText = id;
            ViewBag.IsNews = showNews ?? false;
            ViewBag.IsArticle = showArticle ?? false;
            return View("Index", NewsRepository.Search(id, page, showNews, showArticle));
        }
    }
}
