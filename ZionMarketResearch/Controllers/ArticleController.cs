using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Controllers
{
    public class ArticleController : Controller
    {
        //
        // GET: /Article/

        public ActionResult Index(string url)
        {
            var article = ArticleRepository.GetArticleByUrl(url);
            if (article == null)
                return HttpNotFound("Article not found.");
            ViewBag.css = "news_css.css";
            return View("../News/Index", ArticleRepository.MapNewsArticle(article));
        }

        public ActionResult AllArticles(int? page)
        {
            return View(ArticleRepository.GetArticles(page));
        }
    }
}
