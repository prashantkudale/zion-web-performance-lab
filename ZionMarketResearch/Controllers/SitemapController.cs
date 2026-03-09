using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Controllers
{
    public class SitemapController : Controller
    {
        public void GenerateSitemap()
        {
            SitemapRepository.AllReports();
        }

        public void MainSitemap()
        {
            SitemapRepository.MainSitemap();
        }

        public void PublishedReports(string Lang)
        {
            SitemapRepository.PublishedReports(Lang);
        }
        public void FrPublishedReports()
        {
            SitemapRepository.PublishedReports("fr");            
        }

        public void DePublishedReports()
        {
            SitemapRepository.PublishedReports("de");
        }
        public void UpcomingReports()
        {
            SitemapRepository.UpcomingReports();
        }

        public void News()
        {
            SitemapRepository.News();
        }

        public void Articles()
        {
            SitemapRepository.Articles();
        }

        public void FeaturedReports()
        {
            SitemapRepository.FeaturedReports();
        }
    }
}
