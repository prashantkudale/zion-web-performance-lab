using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace ZionMarketResearch.Models
{
    public class SitemapRepository
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? PublishedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Frequency { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }

        public static void MainSitemap()
        {
            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write(GenerateXml(new List<SitemapRepository>
            {
                new SitemapRepository { Title = "All Published Reports", Action = "PublishedReports", Controller = "Sitemap" },
                new SitemapRepository { Title = "All Published Reports", Controller = "Sitemap", Url = "fr-published-reports.xml"},
                new SitemapRepository { Title = "All Published Reports", Controller = "Sitemap", Url = "de-published-reports.xml"},

                new SitemapRepository { Title = "All Upcoming Reports", Action = "UpcomingReports", Controller = "Sitemap" },
                new SitemapRepository { Title = "All News", Action = "News", Controller = "Sitemap" },
                new SitemapRepository { Title = "All Articles", Action="Articles", Controller = "Sitemap" },
                new SitemapRepository { Title = "All Featured Reports", Action="FeaturedReports", Controller = "Sitemap" }
            }));
            HttpContext.Current.Response.End();
        }

        public static void AllReports()
        {

            var sp = (from s in ReportRepository.AllReports()
                      select new SitemapRepository
                      {
                          Description = s.Description,
                          Title = s.ReportTitle,
                          PublishedDate = s.PublishedDate,
                          Url = s.ReportUrl,
                          Action = "Index",
                          Controller = "Report"
                      }).ToList();
            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write(GenerateXml(sp));
            HttpContext.Current.Response.End();
        }

        public static void News()
        {
            var r = (from x in NewsRepository.GetAllNews()
                     select new SitemapRepository
                     {
                         Title = x.NewsTitle,
                         Description = x.Description,
                         Url = x.NewsUrl,
                         PublishedDate = x.PublishedDate,
                         Action = "Index",
                         Controller = "News"
                     }).OrderByDescending(x => x.UpdatedDate).ToList();

            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write(GenerateXml(r));
            HttpContext.Current.Response.End();

        }

        public static void Articles()
        {
            var r = (from x in ArticleRepository.GetAllArticles()
                     select new SitemapRepository
                     {
                         Title = x.Title,
                         Description = x.Description,
                         Url = x.Url,
                         PublishedDate = x.PublishedDate,
                         Action = "Index",
                         Controller = "Article"
                     }).ToList();

            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write(GenerateXml(r));
            HttpContext.Current.Response.End();

        }

        public static void PublishedReports(string Lang)
        {
            var sp = (from s in ReportRepository.AllPublishedReports()
                      select new SitemapRepository
                      {
                          //Description = s.Description,
                          Title = s.ReportTitle,
                          PublishedDate = s.PublishedDate,
                          Url = s.ReportUrl,
                          Action = "Index",
                          Controller = "Report"
                      }).ToList();
            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write(GenerateXml(sp, Lang));
            HttpContext.Current.Response.End();
        }
        public static void FrPublishedReports()
        {
            var sp = (from s in ReportRepository.AllPublishedReports()
                      select new SitemapRepository
                      {
                          //Description = s.Description,
                          Title = s.ReportTitle,
                          PublishedDate = s.PublishedDate,
                          Url = s.ReportUrl
                      }).ToList();
            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write(GenerateXml(sp));
            HttpContext.Current.Response.End();
        }
        public static void DePublishedReports()
        {
            var sp = (from s in ReportRepository.AllPublishedReports()
                      select new SitemapRepository
                      {
                          //Description = s.Description,
                          Title = s.ReportTitle,
                          PublishedDate = s.PublishedDate,
                          Url = s.ReportUrl
                      }).ToList();
            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write(GenerateXml(sp));
            HttpContext.Current.Response.End();
        }

        public static void UpcomingReports()
        {
            var sp = (from s in ReportRepository.AllUpcomingReports()
                      select new SitemapRepository
                      {
                          Description = s.Description,
                          Title = s.ReportTitle,
                          PublishedDate = s.PublishedDate,
                          Url = s.ReportUrl,
                          Action = "Index",
                          Controller = "Report"
                      }).ToList();
            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write(GenerateXml(sp));
            HttpContext.Current.Response.End();
        }

        public static void FeaturedReports()
        {
            var reports = (from r in ReportRepository.FeaturedReports()
                           select new SitemapRepository
                           {
                               Title = r.ReportTitle,
                               Url = r.ReportUrl,
                               Action = "Index",
                               Controller = "Report"
                           }).ToList();
            HttpContext.Current.Response.ContentType = "text/xml; charset=utf-8";
            HttpContext.Current.Response.Write(GenerateXml(reports));
            HttpContext.Current.Response.End();
        }

        private static string GenerateXml(List<SitemapRepository> xml, string Lang="")
        {
            var urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream);
            writer.WriteStartDocument();
            writer.WriteProcessingInstruction("xml-stylesheet", "type='text/xsl' href='gss.xsl'");
            writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            //writer.WriteAttributeString("xmlns", "mrs", null, "http://www.sitemaps.org/schemas/sitemap/0.9");
            writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xsi", "schemaLocation", null, "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");
            writer.WriteStartElement("url");
            writer.WriteStartElement("loc");
            writer.WriteString("https://www.zionmarketresearch.com");
            writer.WriteEndElement();
            writer.WriteStartElement("changefreq");
            writer.WriteString("daily");
            writer.WriteEndElement();
            writer.WriteEndElement();
            //writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            //writer.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");
            foreach (var u in xml)
            {
                writer.WriteStartElement("url");
                writer.WriteStartElement("loc");
                writer.WriteString(HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                   (Lang!="" ? "/"+Lang :"") + (string.IsNullOrEmpty(u.Action) ? "/"+u.Url : (urlHelper.Action(u.Action, u.Controller, !string.IsNullOrEmpty(u.Url) ? new { url = u.Url } : null)))
                 );
                writer.WriteEndElement();
                writer.WriteStartElement("changefreq");
                writer.WriteString("daily");
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            stream.Position = 0;
            StreamReader read = new StreamReader(stream);
            return read.ReadToEnd();
        }
    }
}