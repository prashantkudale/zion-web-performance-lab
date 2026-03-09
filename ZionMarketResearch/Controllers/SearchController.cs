using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowSearch()
        {
            return PartialView();
        }

        public ActionResult SimpleSearch(string q, int? page)
        {
            ViewBag.SearchText = q;//to preserve search parameters
            return View("SearchResult", ReportRepository.SearchReport(q, page));
        }
        
        public ActionResult AdvanceSearch(SearchParam searchParam)
        {
            ViewBag.SearchParameters = searchParam;//to preserve search parameters
            return View("SearchResult", ReportRepository.AdvanceSearch(searchParam));
        }
    }
}
