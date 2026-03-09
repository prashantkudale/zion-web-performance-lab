using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ZionMarketResearch.Areas.Admin.Models;

namespace ZionMarketResearch.Models
{
    public class NewsRepository
    {
        public static List<LatestNewsView> LatestNews()
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from n in db.tblnews
                        where n.IsDeleted == false && n.IsArticle == false && n.IsActive == true
                        orderby n.PublishedDate descending
                        select new LatestNewsView
                        {
                            NewsId = n.NewsID,
                            NewsTitle = n.NewsTitle,
                            Description = n.News,
                            NewsUrl = n.NewsUrl.ToLower(),
                            PublishedDate = n.PublishedDate.Value
                        }).Take(8).ToList();
            }
        }

        public static NewsDeatils GetNewsByUrl(string url)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var news = (from n in db.tblnews
                            where n.NewsUrl == url && n.IsDeleted == false && n.IsActive == true && n.IsArticle == false
                            join r in db.tblreportinformations on n.ReportId equals r.ReportID into leftjoin
                            from subjoin in leftjoin.DefaultIfEmpty()
                            select new NewsDeatils
                            {
                                NewsId = n.NewsID,
                                NewsTitle = n.NewsTitle,
                                Description = n.News,
                                MetaTile = n.MetaTitle,
                                MetaDescription = n.MetaDescription,
                                MetaKeywords = n.MetaKeywords,
                                NewsUrl = n.NewsUrl,
                                PublishedBy = n.PublishedBy,
                                PublishedDate = n.PublishedDate,
                                ReportId = n.ReportId,
                                ReportTitle = subjoin.ReportTitle,
                                ReportUrl = subjoin.ReportUrl
                            }).FirstOrDefault();
                if (news != null && news.ReportId != null && news.ReportId > 0)
                {
                    news.Description = EmbedHTML(news.Description, "/custom/" + news.ReportId ,"/buynow/su/" + news.ReportUrl, "/sample/" + news.ReportUrl);
                }

                if (news != null && news.ReportId != null && news.ReportId > 0)
                {
                    var report = ReportRepository.GetReportById((int)news.ReportId);
                    var keyword = report.ReportTitle.Substring(0, report.ReportTitle.IndexOf("Market") + 6).Replace("Global", string.Empty).Trim();

                    news.Description = EmbedUrl(news.Description, keyword, report.ReportUrl);
                }
                return news;
            }
        }

        public static IPagedList<LatestNewsView> AllNews(int? page, int maxRows = 20)
        {
            ZionDbEntities db = new ZionDbEntities();
            var allnews = (from n in db.tblnews
                           where n.IsDeleted == false && n.IsArticle == false && n.IsActive == true
                           orderby n.PublishedDate descending
                           select new LatestNewsView
                           {
                               NewsTitle = n.NewsTitle,
                               Description = n.News,
                               NewsId = n.NewsID,
                               NewsUrl = n.NewsUrl.ToLower(),
                               PublishedDate = n.PublishedDate,
                               PublishedBy = n.PublishedBy
                           }).ToPagedList(page ?? 1, maxRows);

            return page == null || allnews.PageCount >= page ? allnews : null;
        }

        public static List<LatestNewsView> GetAllNews()
        {
            List<LatestNewsView> allnews = default;
            DatabaseContext.PerformAction(db =>
            {
                DateTime preDate = DateTime.Now.AddHours(-24);
                allnews = (from n in db.tblnews
                               where n.IsDeleted == false && n.IsArticle == false && n.IsActive == true
                               //&& n.PublishedDate <= preDate
                               orderby n.PublishedDate descending
                               select new LatestNewsView
                               {
                                   NewsTitle = n.NewsTitle,
                                   Description = n.News,
                                   PublishedDate = n.PublishedDate,
                                   NewsUrl = n.NewsUrl.ToLower()                                   
                               }).ToList();
            });
            return allnews;
        }

        static string EmbedHTML(string newsContent, string customUrl, string buyNowUrl, string requestSampleUrl)
        {
            string html = Util.Utility.ReadTemplate("NewsButton");
            html = html.Replace("[ZMR:BuyNow]", buyNowUrl).
                Replace("[ZMR:RequestSample]", requestSampleUrl).
                Replace("[ZMR:CustomRequest]", customUrl);
            int secondPara = newsContent.IndexOf("</p>", newsContent.IndexOf("</p>") + 5) + 5;
            newsContent = newsContent.Insert(secondPara, "<div class='imgButton'>" + html + "</div>");
            return newsContent;
        }

        static string EmbedUrl(string newsDescription, string keyword, string reportUrl)
        {
            var firstParaStarting = newsDescription.ToLower().IndexOf("<p>");
            var firstParaEnding = newsDescription.ToLower().IndexOf("</p>");

            var firstPara = newsDescription.Substring(firstParaStarting + 3, firstParaEnding - 3 - firstParaStarting);

            var keywordLinks = Regex.Matches(firstPara, @"<a [^>]*>(.*?)</a>", RegexOptions.IgnoreCase);

            var isKeywordLinked = keywordLinks.Cast<Match>().Select(x => x.Groups[1]).Count(x => x.Value.ToLower().Contains(keyword.ToLower())) > 0;

            if (!isKeywordLinked)
            {
                var regex = new Regex(Regex.Escape(keyword), RegexOptions.IgnoreCase);

                firstPara = regex.Replace(firstPara, delegate (Match m)
                {
                    return $"<a href=\"/report/{reportUrl}\">{m.Value}</a>";
                }, 1);

                newsDescription = newsDescription.Remove(firstParaStarting + 3, (firstParaEnding - 3) - firstParaStarting);
                newsDescription = newsDescription.Insert(newsDescription.IndexOf("<p>") + 3, firstPara);
            }
            return newsDescription;
        }


    }

    public class LatestNewsView
    {
        public int NewsId { get; set; }
        public string NewsTitle { get; set; }
        [UIHint("LatestNewsView")]
        public string Description { get; set; }
        public string NewsUrl { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string PublishedBy { get; set; }
    }

    public class NewsDeatils
    {
        public int NewsId { get; set; }
        public string NewsTitle { get; set; }
        public string NewsUrl { get; set; }
        public string Description { get; set; }
        public string MetaTile { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string PublishedBy { get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool IsArticle { get; set; }
        public int? ReportId { get; set; }
        public string ReportUrl { get; set; }
        public string ReportTitle { get; set; }
    }
}