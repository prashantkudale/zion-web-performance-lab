using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace ZionMarketResearch.Models
{
    public class RSSRepository
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public DateTime? PublishedDate { get; set; }

        /// <summary>
        /// Return RSS for upcoming reports.
        /// </summary>
        public static void UpcomingReports()
        {
            var r = (from x in ReportRepository.AllReports()
                     select new RSSRepository
                     {
                         Title = x.ReportTitle,
                         Description = x.Description,
                         PublishedDate = x.PublishedDate,
                         Url = x.ReportUrl
                     }).ToList();
            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write(CreateReportRss(r, "Upcoming Reports"));
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// returns RSS for Published Reports
        /// </summary>
        public static void PublishedReports()
        {
            var r = (from x in ReportRepository.AllPublishedReports()
                     select new RSSRepository
                     {
                         Title = x.ReportTitle,
                         Description = x.Description,
                         PublishedDate = x.PublishedDate,
                         Url = x.ReportUrl
                     }).ToList();
            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write(CreateReportRss(r, "Published Reports"));
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// return RSS of Articles
        /// </summary>
        public static void Articles()
        {
            var r = (from x in ArticleRepository.GetAllArticles()
                     select new RSSRepository
                     {
                         Title = x.Title,
                         Url = x.Url,
                         Description = x.Description,
                         PublishedDate = x.PublishedDate
                     }).ToList();
            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write(CreateReportRss(r, "Articles"));
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// return RSS of News
        /// </summary>
        public static void News()
        {
            var r = (from x in NewsRepository.GetAllNews()
                     select new RSSRepository
                     {
                         Title = x.NewsTitle,
                         Url = x.NewsUrl,
                         Description = x.Description,
                         PublishedDate = x.PublishedDate
                     }).ToList();
            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write(CreateReportRss(r, "Articles"));
            HttpContext.Current.Response.End();
        }

        #region CreateReportRss
        private static string CreateReportRss(List<RSSRepository> rinfo, string title)
        {
            MemoryStream stream = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(stream);
            writer.WriteStartDocument();

            writer.WriteStartElement("rss");
            writer.WriteAttributeString("version", "2.0");
            writer.WriteAttributeString("xmlns", "content", null, "http://purl.org/rss/1.0/modules/content/");
            writer.WriteAttributeString("xmlns", "wfw", null, "http://wellformedweb.org/CommentAPI/");
            writer.WriteAttributeString("xmlns", "dc", null, "http://purl.org/dc/elements/1.1/");
            writer.WriteAttributeString("xmlns", "atom", null, "http://www.w3.org/2005/Atom");
            writer.WriteAttributeString("xmlns", "sy", null, "http://purl.org/rss/1.0/modules/syndication/");
            writer.WriteAttributeString("xmlns", "slash", null, "http://purl.org/rss/1.0/modules/slash/");
            writer.WriteStartElement("channel");
            writer.WriteStartElement("title");
            writer.WriteString(title);
            writer.WriteEndElement();
            writer.WriteStartElement("link");
            writer.WriteString(HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host);
            writer.WriteEndElement();
            writer.WriteStartElement("description");
            writer.WriteString(title);
            writer.WriteEndElement();


            foreach (RSSRepository r in rinfo)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("title");
                writer.WriteString(r.Title);
                writer.WriteEndElement();
                writer.WriteStartElement("link");
                writer.WriteString(HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + r.Url);
                writer.WriteEndElement();
                if (r.PublishedDate != null)
                {
                    writer.WriteStartElement("pubDate");
                    writer.WriteString(r.PublishedDate.Value.Date.ToShortDateString());
                    writer.WriteEndElement();
                }
                writer.WriteStartElement("description");
                writer.WriteString(r.Description);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Flush();
            stream.Position = 0;
            StreamReader read = new StreamReader(stream);
            return read.ReadToEnd();
        }
        #endregion
    }
}