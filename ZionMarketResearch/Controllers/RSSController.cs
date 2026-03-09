using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Controllers
{
    public class RSSController : Controller
    {
        public void UpcomingReports()
        {
            RSSRepository.UpcomingReports();
        }

        public void PublishedReports()
        {
            RSSRepository.PublishedReports();
        }

        public void News()
        {
            RSSRepository.News();
        }

        public void Articles()
        {
            RSSRepository.Articles();
        }
    }
}
