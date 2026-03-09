using MySql.Data.MySqlClient;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using ZionAdmin.Models.LuceneSearch;
using ZionMarketResearch.Areas.Admin.Models;

namespace ZionMarketResearch.Models
{
    /// <summary>
    /// Class responsible for all functionality related to Report
    /// </summary>
    public class ReportRepository
    {

        public int ReportId { get; set; }
        public string ReportTitle { get; set; }
        public string ReportUrl { get; set; }
        public string MainPageDescription { get; set; }
        public string MainPageImagePath { get; set; }
        public string MainPageImageAltAttribute { get; set; }
        public string SearchPageDescription { get; set; }
        public string SearchPageImagePath { get; set; }
        public string SearchPageImageAltAttribute { get; set; }
        public decimal SingleUser { get; set; }
        public decimal? MultiUser { get; set; }
        public decimal? CorporateUser { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeyword { get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool IsUpcoming { get; set; }
        public string[] Category { get; set; }

        #region Methods
        /// <summary>
        /// Get latest report orderb by report id decending
        /// </summary>
        /// <param name="includeDescription">The result need the Description or not</param>
        /// <returns>IEnumerable<LatestReportView></returns>
        public static IEnumerable<LatestReportView> LatestReport(bool includeDescription = true)
        {
            //TODO: find a good way to get categories from category breadcrumb.
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var featuredReportID = db.relreportattributes.Select(x => x.ReportID).Distinct().ToList();

                var reports = includeDescription ? (from r in db.tblreportinformations
                                                    orderby r.PublishDate descending, r.ReportID descending
                                                    where r.IsUpComing == false && r.IsDeleted == false && !featuredReportID.Contains(r.ReportID) && r.IsActive == true
                                                    select new LatestReportView
                                                    {
                                                        ReportId = r.ReportID,
                                                        ReportTitle = r.ReportTitle,
                                                        ReportUrl = r.ReportUrl,
                                                        PublishedDate = r.PublishDate,
                                                        Category = r.CategoryBreadCrumb,
                                                        Description = r.MainPageDescription,
                                                        IsUpcoming = r.IsUpComing,
                                                        NumberOfPages = r.NumberOfPage
                                                    }).Take(6).ToList() :
                                            (from r in db.tblreportinformations
                                             orderby r.PublishDate descending, r.ReportID descending
                                             where r.IsUpComing == false && r.IsDeleted == false && !featuredReportID.Contains(r.ReportID) && r.IsActive == true
                                             select new LatestReportView
                                             {
                                                 ReportId = r.ReportID,
                                                 ReportTitle = r.ReportTitle,
                                                 ReportUrl = r.ReportUrl,
                                                 PublishedDate = r.PublishDate,
                                                 Category = r.CategoryBreadCrumb,
                                                 IsUpcoming = r.IsUpComing,
                                                 NumberOfPages = r.NumberOfPage
                                             }).Take(5).ToList();

                return includeDescription ? (from r in reports
                                             orderby r.PublishedDate descending, r.ReportId descending
                                             select new LatestReportView
                                             {
                                                 ReportId = r.ReportId,
                                                 ReportTitle = r.ReportTitle,
                                                 ReportUrl = r.ReportUrl,
                                                 PublishedDate = r.PublishedDate,
                                                 Categories = _getCategory(r.Category),
                                                 Description = Util.Utility.StripHTML(r.Description),
                                                 IsUpcoming = r.IsUpcoming,
                                                 NumberOfPages = r.NumberOfPages
                                             }) : (from r in reports
                                                   orderby r.PublishedDate descending, r.ReportId descending
                                                   select new LatestReportView
                                                   {
                                                       ReportId = r.ReportId,
                                                       ReportTitle = r.ReportTitle,
                                                       ReportUrl = r.ReportUrl,
                                                       PublishedDate = r.PublishedDate,
                                                       Categories = _getCategory(r.Category),
                                                       IsUpcoming = r.IsUpcoming,
                                                       NumberOfPages = r.NumberOfPages
                                                   });
            }
        }

        /// <summary>
        /// Get latest featured reports
        /// </summary>
        /// <param name="includeDescription"></param>
        /// <returns></returns>
        public static IEnumerable<LatestReportView> LatestFeaturedReport(bool includeDescription = true)
        {
            var result = new List<LatestReportView>();
            //TODO: Select 5 top featured reports
            DatabaseContext.PerformAction(context =>
            {
                var query = from r in context.tblreportinformations
                            join a in context.relreportattributes on r.ReportID equals a.ReportID
                            join attr in context.tblreportattributes on a.AttributeID equals attr.ID
                            where r.IsDeleted == false && attr.AttributeName == "Featured"
                            orderby r.PublishDate, r.ReportID
                            select r;

                result = includeDescription ? query.Select(x => new LatestReportView
                {
                    ReportId = x.ReportID,
                    ReportTitle = x.ReportTitle,
                    PublishedDate = x.PublishDate,
                    ReportUrl = x.ReportUrl,
                    Description = x.MainPageDescription,
                    Category = x.CategoryBreadCrumb
                }).Take(5).ToList()
                : query.Select(x => new LatestReportView
                {
                    ReportId = x.ReportID,
                    ReportTitle = x.ReportTitle,
                    PublishedDate = x.PublishDate,
                    ReportUrl = x.ReportUrl,
                    Category = x.CategoryBreadCrumb
                }).Take(5).ToList();


                foreach (var r in result)
                {
                    r.Description = includeDescription ? Util.Utility.StripHTML(r.Description) : default(string);
                    r.Categories = _getCategory(r.Category);
                }

            });
            return result;
        }

        /// <summary>
        /// Return a report details by URL. URL formatted here to convinent format
        /// </summary>
        /// <param name="url">Formatted URL</param>
        /// <returns>MainPageReportView object for Report Detail View</returns>
        public static MainPageReportView GetReportByUrl(string url, bool showSellerDescription = false)
        {
            MainPageReportView report = default;
            //string[] surl = url.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            //todo: implement lucene
            DatabaseContext.PerformAction(db =>
            {
                #region GetReport
                var rep = LuceneSearch.SearchDefault(url.ToLower(), "ReportUrl").FirstOrDefault();

                if (rep == null)
                {
                    rep = (from r in db.tblreportinformations
                           where r.ReportUrl == url
                           select new LuceneReport
                           {
                               ReportId = r.ReportID,
                               ReportUrl = r.ReportUrl,
                               ReportTitle = r.ReportTitle,
                               Category = r.CategoryBreadCrumb
                           }).FirstOrDefault();

                    if (rep != null)
                    {
                        LuceneSearch.AddUpdateLuceneIndex(rep);
                    }
                }

                if (rep != null)
                {

                    report = (from r in db.Zion_GetReportById(rep.ReportId)
                              select new MainPageReportView
                              {
                                  ReportId = r.ReportId,
                                  ReportTitle = r.ReportTitle,
                                  ReportUrl = r.ReportUrl,
                                  MainPageDescription = r.MainPageDescription,
                                  MainPageImagePath = r.MainPageImagePath,
                                  SearchPageDescription = r.SearchPageDescription,
                                  MainPageImageAltAttribute = r.MainPageImageAltAttribute,
                                  SingleUser = (decimal)r.SingleUser,
                                  MultiUser = r.MultiUser,
                                  CorporateUser = r.CorporateUser,
                                  MetaTitle = r.MetaTitle,
                                  MetaDescription = r.MetaDescription,
                                  MetaKeyword = r.MetaKeywords,
                                  PublishedDate = r.PublishDate,
                                  Category = r.Category,
                                  Categories = r.Category?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                                  TableOfContent = r.TableOfContents,
                                  FreeAnalysis = r.FreeAnalysis,
                                  Methodology = r.Methodology,
                                  ListOfFigures = r.ListOfFigure,
                                  ListOfTabels = r.ListOfTable,
                                  NumberOfPages = r.NumberOfPage,
                                  DeliveryFormat = r.DeliveryFormat,
                                  IsUpcoming = r.IsUpcoming == 1 ? true : false,
                                  TOCUrl = "/toc/" + r.ReportUrl,
                                  RequestSampleUrl = "/sample/" + r.ReportUrl,
                                  SingleUserLink = (r.IsUpcoming == 0 ? "/buynow/su/" : "/prebook/su/") + r.ReportUrl,
                                  MultiUserLink = (r.IsUpcoming == 0 ? "/buynow/mu/" : "/prebook/mu/") + r.ReportUrl,
                                  CorporateUserLink = (r.IsUpcoming == 0 ? "/buynow/cu/" : "/prebook/cu/") + r.ReportUrl,
                                  //RelatedNews = !string.IsNullOrEmpty(r.RelatedNews) ? r.RelatedNews.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : null,
                                  RelatedNews = !string.IsNullOrEmpty(r.RelatedNews) ? r.RelatedNews.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries) : null,
                                  BuyingInquiryLink = "/inquiry/" + r.ReportUrl,
                                  FreeAnalysisLink = r.FreeAnalysis != null && !string.IsNullOrEmpty(r.FreeAnalysis) ? "/market-analysis/" + r.ReportUrl : "/ask-to-analyst/" + r.ReportUrl,
                                  MetaTitleSample = r.MetaTitleSample,
                                  MetaDescriptionSample = r.MetaDescriptionSample,
                                  MetaKeywordsSample = r.MetaKeywordsSample,
                                  MetaTitleTOC = r.MetaTitleTOC,
                                  MetaDescriptionTOC = r.MetaDescriptionTOC,
                                  MetaKeywordsTOC = r.MetaKeywordsTOC,
                                  MetaTitleFreeAnalysis = r.MetaTitleFreeAnalysis,
                                  MetaDescriptionFreeAnalysis = r.MetaDescriptionFreeAnalysis,
                                  MetaKeywordsFreeAnalysis = r.MetaKeywordsFreeAnalysis,
                                  MetaTitleInquiry = r.MetaTitleInquiry,
                                  MetaDescriptionInquiry = r.MetaDescriptionInquiry,
                                  MetaKeywordsInquiry = r.MetaKeywordsInquiry,
                                  URL = r.ReportUrl,
                                  Tags = r.Tags
                              }).FirstOrDefault();
                }


                #endregion

                #region MetaTitle
                if (report != null && !string.IsNullOrEmpty(report.ReportTitle))
                    report.BreadCrumbTitle = !string.IsNullOrEmpty(report.MetaTitle) && report.MetaTitle.ToLower().Contains("market")
                        ? report.MetaTitle.Substring(0, report.MetaTitle.ToLower().IndexOf("market") + 6)
                        : !string.IsNullOrEmpty(report.ReportTitle) && report.ReportTitle.ToLower().Contains("market") ? report.ReportTitle.Substring(0, report.ReportTitle.ToLower().IndexOf("market") + 6) : string.Empty;
                #endregion


                if (report != null)
                {
                    report.FAQ = (from faq in db.tblreportfaqs
                                  where faq.ReportID == report.ReportId
                                  select new ReportFAQ
                                  {
                                      Question = faq.Question,
                                      Answer = faq.Answer,
                                      RowNumber = (int)faq.RowNumber
                                  }).ToList();
                    if (showSellerDescription)
                    {
                        report.MainPageDescription = (from reseller in db.tblreseller_reportinformation
                                                      where reseller.ReportID == report.ReportId
                                                      select reseller.MainPageDescription).FirstOrDefault();
                    }

                    // Commented by Mahesh : for removing Report Scope: from upcomming reports
                    //report.MainPageDescription = ReplaceTableImage(report.MainPageDescription, !report.IsUpcoming);
                }
            });
            if(report!=null)
                report = getAllHeadingTags(report);
            return report;
        }

        public static string ReplaceTableImage(string description, bool isPublished)
        {
            if (description.Contains("<table>"))
                return description;
            string str = File.ReadAllText(HttpContext.Current.Server.MapPath("/ReportTable/ziontable.html"));
            if (isPublished)
            {
                List<string> list = ((IEnumerable<string>)description.Replace("\r\n", string.Empty).Split(new string[2]
                {
          "<p>",
          "</p>"
                }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
                int index = list.IndexOf(list.FirstOrDefault<string>((Func<string, bool>)(x => x.Contains("Report Scope"))));
                if (index < list.Count-1 && index > -1 && !list[index + 1].Contains("<table") && !list[index].Contains("<table"))
                {
                    if (list[index + 1] != "&nbsp;" && !list[index + 1].StartsWith("<img"))
                        list.Insert(index + 1, str);
                    else
                        list[index + 1] = str;
                    return string.Join("", list.Select<string, string>((Func<string, string>)(x => "<p>" + x + "</p>")));
                }
            }
            else if (!description.Contains("<p><strong>Report Scope:</strong></p>"))
            {
                List<string> list = ((IEnumerable<string>)description.Replace("\r\n", string.Empty).Split(new string[2]
                {
          "<p>",
          "</p>"
                }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
                if (!list[2].Contains("<table"))
                {
                    list.Insert(2, "<strong>Report Scope:</strong>" + str);
                    return string.Join("", list.Select<string, string>((Func<string, string>)(x => "<p>" + x + "</p>")));
                }
            }
            return description;
        }


        public static MainPageReportView GetReportById(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                #region GetReport
                var report = (from r in db.Zion_GetReportById(id)
                              select new MainPageReportView
                              {
                                  ReportId = r.ReportId,
                                  ReportTitle = r.ReportTitle,
                                  ReportUrl = r.ReportUrl,
                                  MainPageDescription = r.MainPageDescription,
                                  SearchPageDescription = r.SearchPageDescription,
                                  MainPageImagePath = r.MainPageImagePath,
                                  MainPageImageAltAttribute = r.MainPageImageAltAttribute,
                                  SingleUser = (decimal)r.SingleUser,
                                  MultiUser = r.MultiUser,
                                  CorporateUser = r.CorporateUser,
                                  MetaTitle = r.MetaTitle,
                                  MetaDescription = r.MetaDescription,
                                  MetaKeyword = r.MetaKeywords,
                                  PublishedDate = r.PublishDate,
                                  Category = r.Category,
                                  Categories = r.Category != null ? r.Category.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null,
                                  TableOfContent = r.TableOfContents,
                                  FreeAnalysis = r.FreeAnalysis,
                                  Methodology = r.Methodology,
                                  ListOfFigures = r.ListOfFigure,
                                  ListOfTabels = r.ListOfTable,
                                  NumberOfPages = r.NumberOfPage,
                                  DeliveryFormat = r.DeliveryFormat,
                                  IsUpcoming = r.IsUpcoming == 1 ? true : false,
                                  TOCUrl = "/toc/" + r.ReportUrl,
                                  RequestSampleUrl = "/sample/" + r.ReportUrl,
                                  SingleUserLink = (r.IsUpcoming == 0 ? "/buynow/su/" : "/prebook/su/") + r.ReportUrl,
                                  MultiUserLink = (r.IsUpcoming == 0 ? "/buynow/mu/" : "/prebook/mu/") + r.ReportUrl,
                                  CorporateUserLink = (r.IsUpcoming == 0 ? "/buynow/cu/" : "/prebook/cu/") + r.ReportUrl,
                                  //RelatedNews = !string.IsNullOrEmpty(r.RelatedNews) ? r.RelatedNews.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : null,
                                  RelatedNews = !string.IsNullOrEmpty(r.RelatedNews) ? r.RelatedNews.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries) : null,
                                  BuyingInquiryLink = "/inquiry/" + r.ReportUrl,
                                  FreeAnalysisLink = r.FreeAnalysis != null && !string.IsNullOrEmpty(r.FreeAnalysis) ? "/market-analysis/" + r.ReportUrl : "/ask-to-analyst/" + r.ReportUrl,
                                  MetaTitleSample = r.MetaTitleSample,
                                  MetaDescriptionSample = r.MetaDescriptionSample,
                                  MetaKeywordsSample = r.MetaKeywordsSample,
                                  MetaTitleTOC = r.MetaTitleTOC,
                                  MetaDescriptionTOC = r.MetaDescriptionTOC,
                                  MetaKeywordsTOC = r.MetaKeywordsTOC,
                                  MetaTitleFreeAnalysis = r.MetaTitleFreeAnalysis,
                                  MetaDescriptionFreeAnalysis = r.MetaDescriptionFreeAnalysis,
                                  MetaKeywordsFreeAnalysis = r.MetaKeywordsFreeAnalysis,
                                  MetaTitleInquiry = r.MetaTitleInquiry,
                                  MetaDescriptionInquiry = r.MetaDescriptionInquiry,
                                  MetaKeywordsInquiry = r.MetaKeywordsInquiry,
                                  URL = r.ReportUrl,
                                  CRMCategory = r.CRMCategory
                              }).FirstOrDefault();
                #endregion

                #region MetaTitle
                if (report != null && !string.IsNullOrEmpty(report.ReportTitle))
                    report.BreadCrumbTitle = !string.IsNullOrEmpty(report.MetaTitle) && report.MetaTitle.ToLower().Contains("market")
                        ? report.MetaTitle.Substring(0, report.MetaTitle.ToLower().IndexOf("market") + 6)
                        : !string.IsNullOrEmpty(report.ReportTitle) && report.ReportTitle.ToLower().Contains("market") ? report.ReportTitle.Substring(0, report.ReportTitle.ToLower().IndexOf("market") + 6) : string.Empty;
                #endregion
                if (report != null)
                    report = getAllHeadingTags(report);
                return report;
            }

        }

        public static MainPageReportView GetReportByPaymentLink(string url)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var pl = db.tblpaymentlinks.Where(x => x.PaymentLink == url && x.ValidTo >= DateTime.Now).FirstOrDefault();
                MainPageReportView report = null;
                if (pl != null)
                {
                    report = (from r in db.Zion_GetReportById(pl.ReportId)
                              select new MainPageReportView
                              {
                                  ReportId = r.ReportId,
                                  ReportTitle = r.ReportTitle,
                                  ReportUrl = r.ReportUrl,
                                  MainPageDescription = r.MainPageDescription,
                                  SearchPageDescription = r.SearchPageDescription,
                                  MainPageImagePath = r.MainPageImagePath,
                                  MainPageImageAltAttribute = r.MainPageImageAltAttribute,
                                  SingleUser = (decimal)r.SingleUser,
                                  MultiUser = r.MultiUser,
                                  CorporateUser = r.CorporateUser,
                                  MetaTitle = r.MetaTitle,
                                  MetaDescription = r.MetaDescription,
                                  MetaKeyword = r.MetaKeywords,
                                  PublishedDate = r.PublishDate,
                                  Category = r.Category,
                                  Categories = r.Category != null ? r.Category.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null,
                                  TableOfContent = r.TableOfContents,
                                  FreeAnalysis = r.FreeAnalysis,
                                  Methodology = r.Methodology,
                                  ListOfFigures = r.ListOfFigure,
                                  ListOfTabels = r.ListOfTable,
                                  NumberOfPages = r.NumberOfPage,
                                  DeliveryFormat = r.DeliveryFormat,
                                  IsUpcoming = r.IsUpcoming == 1 ? true : false,
                                  TOCUrl = "/toc/" + r.ReportUrl,
                                  RequestSampleUrl = "/sample/" + r.ReportUrl,
                                  SingleUserLink = (r.IsUpcoming == 0 ? "/payment/buynow/su/" : "/prebook/su/") + url,
                                  MultiUserLink = (r.IsUpcoming == 0 ? "/payment/buynow/mu/" : "/prebook/mu/") + url,
                                  CorporateUserLink = (r.IsUpcoming == 0 ? "/payment/buynow/cu/" : "/prebook/cu/") + url,
                                  //RelatedNews = !string.IsNullOrEmpty(r.RelatedNews) ? r.RelatedNews.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : null,
                                  RelatedNews = !string.IsNullOrEmpty(r.RelatedNews) ? r.RelatedNews.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries) : null,
                                  BuyingInquiryLink = "/inquiry/" + r.ReportUrl,
                                  FreeAnalysisLink = "/market-analysis/" + r.ReportUrl,
                                  MetaTitleSample = r.MetaTitleSample,
                                  MetaDescriptionSample = r.MetaDescriptionSample,
                                  MetaKeywordsSample = r.MetaKeywordsSample,
                                  MetaTitleTOC = r.MetaTitleTOC,
                                  MetaDescriptionTOC = r.MetaDescriptionTOC,
                                  MetaKeywordsTOC = r.MetaKeywordsTOC,
                                  MetaTitleFreeAnalysis = r.MetaTitleFreeAnalysis,
                                  MetaDescriptionFreeAnalysis = r.MetaDescriptionFreeAnalysis,
                                  MetaKeywordsFreeAnalysis = r.MetaKeywordsFreeAnalysis,
                                  MetaTitleInquiry = r.MetaTitleInquiry,
                                  MetaDescriptionInquiry = r.MetaDescriptionInquiry,
                                  MetaKeywordsInquiry = r.MetaKeywordsInquiry
                              }).FirstOrDefault();
                    report.SingleUser = (decimal)pl.SingleUserPrice;
                    report.MultiUser = pl.MultiUserPrice;
                    report.CorporateUser = pl.CorporateUserPrice;
                }
                return report;
            }
        }


        /// <summary>
        /// This is private method to get Categories from category breadcrumb string
        /// </summary>
        /// <param name="categoryBreadcrumb">String with Category Id and seprated with comma (,)</param>
        /// <returns>string[], Returns string array with Category Names</returns>
        static string[] _getCategory(string categoryBreadcrumb, bool withUrl = false)
        {
            int[] categoryId = categoryBreadcrumb.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)).ToArray();
            string[] categories;
            using (ZionDbEntities db = new ZionDbEntities())
            {
                if (withUrl)
                    categories = (from c in db.tblcategories where categoryId.Contains(c.pkCategoryID) orderby c.pkCategoryID select "<a href='/category/" + c.CategoryUrl + "'>" + c.CategoryName + "</a>").ToArray();
                else
                    categories = (from c in db.tblcategories where categoryId.Contains(c.pkCategoryID) orderby c.pkCategoryID select c.CategoryName).ToArray();
            }
            return categories;
        }

        public static IEnumerable<LatestUpComingReportView> LatestUpcomingReports(bool includeDescription = true)
        {
            ZionDbEntities db = new ZionDbEntities();
            var reports = includeDescription ? (from r in db.tblreportinformations
                                                orderby r.ReportID descending
                                                where r.IsUpComing == true && r.IsDeleted == false && r.IsActive == true
                                                select new LatestUpComingReportView
                                                {
                                                    ReportId = r.ReportID,
                                                    ReportTitle = r.ReportTitle,
                                                    ReportUrl = r.ReportUrl,
                                                    PublishedDate = r.PublishDate,
                                                    Category = r.CategoryBreadCrumb,
                                                    Description = r.MainPageDescription
                                                }).Take(5).ToList() : (from r in db.tblreportinformations
                                                                       orderby r.ReportID descending
                                                                       where r.IsUpComing == true && r.IsDeleted == false && r.IsActive == true
                                                                       select new LatestUpComingReportView
                                                                       {
                                                                           ReportId = r.ReportID,
                                                                           ReportTitle = r.ReportTitle,
                                                                           ReportUrl = r.ReportUrl,
                                                                           PublishedDate = r.PublishDate,
                                                                           Category = r.CategoryBreadCrumb
                                                                       }).Take(5).ToList();

            return from r in reports
                   select new LatestUpComingReportView
                   {
                       ReportId = r.ReportId,
                       ReportTitle = r.ReportTitle,
                       ReportUrl = r.ReportUrl,
                       PublishedDate = r.PublishedDate,
                       Categories = _getCategory(r.Category),
                       Description = Util.Utility.StripHTML(r.Description)
                   };
        }

        public static string GetReportSiteMap()
        {
            MemoryStream stream = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(stream);
            writer.WriteStartDocument();
            writer.WriteProcessingInstruction("xml-stylesheet", "type='text/xsl' href='gss.xsl'");
            writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            //writer.WriteAttributeString("xmlns", "mrs", null, "http://www.sitemaps.org/schemas/sitemap/0.9");
            writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xsi", "schemaLocation", null, "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");
            writer.WriteStartElement("url");
            writer.WriteStartElement("loc");
            writer.WriteString("http://www.zionmarketresearch.com");
            writer.WriteEndElement();
            writer.WriteStartElement("changefreq");
            writer.WriteString("daily");
            writer.WriteEndElement();
            writer.WriteEndElement();
            return "";
        }

        public static CustomView GetReportForCustomView(int reportId)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from r in db.tblreportinformations
                        where r.ReportID == reportId
                        select new CustomView
                        {
                            ReportId = r.ReportID,
                            ReportTitle = r.ReportTitle,
                            ReportUrl = r.ReportUrl
                        }).SingleOrDefault();
            }
        }

        ///// <summary>
        ///// Original code
        ///// </summary>
        ///// <param name="searchParam"></param>
        ///// <returns></returns>
        /////// <summary>
        /////// Original code
        /////// </summary>
        /////// <param name="searchParam"></param>
        /////// <returns></returns>
        //public static IPagedList<SearchResultView> SearchReport(string searchtext, int? page, int maxRows = 20)
        //{
        //    var sw = Stopwatch.StartNew();
        //    ZionDbEntities db = new ZionDbEntities();

        //    var pagedList = (from r in db.tblreportinformations
        //                     where (r.ReportTitle.Contains(searchtext) || r.MainPageDescription.Contains(searchtext)) && r.IsDeleted == false && r.IsActive == true
        //                     orderby r.ReportID descending
        //                     select new SearchResultView
        //                     {
        //                         ReportId = r.ReportID,
        //                         ReportTitle = r.ReportTitle,
        //                         ReportUrl = r.ReportUrl,
        //                         Description = r.MainPageDescription,
        //                         PublishedDate = r.PublishDate,
        //                         NumberOfPages = r.NumberOfPage,
        //                         ReportFormat = (int)r.DeliveryFormat,
        //                         SingleUser = (decimal)r.PriceSingleUser,
        //                         IsUpcoming = r.IsUpComing ?? false,
        //                         DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail",
        //                     }).ToPagedList(page ?? 1, maxRows);

        //    sw.Stop();
        //    Debug.WriteLine("DB Time: " + sw.ElapsedMilliseconds + " ms");

        //    return page == null || page <= pagedList.PageCount ? pagedList : null;
        //}

        /// <summary>
        /// New code
        /// </summary>
        /// <param name="searchParam"></param>
        /// <returns></returns>
        /// 
        public static IPagedList<SearchResultView> SearchReport(string searchtext, int? page, int maxRows = 20)
        {
            // Start a stopwatch to measure database execution time
            //var sw = Stopwatch.StartNew();

            // Determine the current page number (default = 1)

            int pageNumber = page ?? 1;

            // Calculate how many rows should be skipped for pagination 
            // Example: page 2 with 20 rows per page → skip first 20 rows
            int offset = (pageNumber - 1) * maxRows;

            // Create DB context
            using (var db = new ZionDbEntities())
            {
                /* Convert search text into MySQL BOOLEAN MODE syntax. 
                 * Example: User input: "online poker market" Converted to: +online +poker +market '+' means every word must exist in the record. 
                 * This allows MySQL FULLTEXT index to quickly shortlist candidate rows. */
                string booleanSearch = "+" + searchtext.Replace(" ", " +");

                /* Execute raw SQL query because Entity Framework LINQ cannot use MySQL FULLTEXT MATCH...AGAINST efficiently. 
                 * Strategy used here: 1. FULLTEXT MATCH → quickly find rows containing required words (very fast due to index) 
                 * 2. LIKE filter → enforce exact substring behavior like the original Contains() 
                 * 3. LIMIT → perform pagination directly in SQL to avoid loading unnecessary rows */

                var results = db.Database.SqlQuery<SearchResultView>(@"
            SELECT 
                ReportID AS ReportId,
                ReportTitle,
                ReportUrl,
                MainPageDescription AS Description,
                PublishDate AS PublishedDate,
                NumberOfPage AS NumberOfPages,
                DeliveryFormat AS ReportFormat,
                PriceSingleUser AS SingleUser,
                IFNULL(IsUpComing,0) AS IsUpcoming
            FROM tblreportinformation
            WHERE
                MATCH(ReportTitle, MainPageDescription)
                AGAINST (@p0 IN BOOLEAN MODE)
                AND CONCAT_WS(' ', ReportTitle, MainPageDescription)
                    LIKE CONCAT('%', @p1, '%')
                AND IsDeleted = 0
                AND IsActive = 1
            ORDER BY ReportID DESC
            LIMIT @p2, @p3
        ", booleanSearch, searchtext, offset, maxRows).ToList();

                /* Map DeliveryFormat to CSS icon classes. 
                 * This logic was originally inside the LINQ projection. We keep it here to preserve exact UI behavior. */
                foreach (var r in results)
                {
                    r.DeliveryFormatClass =
                        r.ReportFormat == 0 ? "fa-file-pdf-o iconsize pdf" :
                        r.ReportFormat == 1 ? "fa-file-word-o iconsize doc" :
                        r.ReportFormat == 2 ? "fa-file-excel-o iconsize xl" :
                        r.ReportFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" :
                        "fa-envelope-o iconsize mail";
                }

                // Stop performance timer
                //sw.Stop();

                // Write execution time to debug output
                //Debug.WriteLine("DB Time: " + sw.ElapsedMilliseconds + " ms");

                /* Create paged result object. 
                 * StaticPagedList is used because pagination was already done at SQL level using LIMIT. */
                return new StaticPagedList<SearchResultView>(results, pageNumber, maxRows, results.Count);
            }
        }

        public static IPagedList<SearchResultView> AdvanceSearch(SearchParam searchParam)
        {
            return AdvanceSearch(searchParam.z, searchParam.i, searchParam.n, searchParam.p);
        }

        public static IPagedList<SearchResultView> AdvanceSearch(string searchText, int category, int typeOfReport, int? page)
        {
            //TODO: implement lucene search
            ZionDbEntities db = new ZionDbEntities();
            IPagedList<SearchResultView> result = null;
            switch (typeOfReport)
            {
                case 0://All
                    if (category > 0)
                    {
                        string cat = category.ToString();
                        result = (from r in db.tblreportinformations
                                  where r.ReportTitle.Contains(searchText) && r.CategoryBreadCrumb.Contains(cat) && r.IsDeleted == false
                                  orderby r.ReportID descending
                                  select new SearchResultView
                                  {
                                      ReportId = r.ReportID,
                                      ReportTitle = r.ReportTitle,
                                      ReportUrl = r.ReportUrl,
                                      Description = r.MainPageDescription,
                                      PublishedDate = r.PublishDate,
                                      NumberOfPages = r.NumberOfPage,
                                      ReportFormat = (int)r.DeliveryFormat,
                                      SingleUser = r.PriceSingleUser,
                                      DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail",
                                      IsUpcoming = r.IsUpComing ?? false
                                  }).ToPagedList(page ?? 1, 10);
                    }
                    else
                    {
                        result = (from r in db.tblreportinformations
                                  where r.ReportTitle.Contains(searchText) && r.IsDeleted == false
                                  orderby r.ReportID descending
                                  select new SearchResultView
                                  {
                                      ReportId = r.ReportID,
                                      ReportTitle = r.ReportTitle,
                                      ReportUrl = r.ReportUrl,
                                      Description = r.MainPageDescription,
                                      PublishedDate = r.PublishDate,
                                      NumberOfPages = (int)r.NumberOfPage,
                                      Category = r.CategoryBreadCrumb,
                                      ReportFormat = (int)r.DeliveryFormat,
                                      SingleUser = r.PriceSingleUser,
                                      DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail",
                                      IsUpcoming = r.IsUpComing ?? false
                                  }).ToPagedList(page ?? 1, 10);
                    }
                    break;
                case 1://Upcoming
                    if (category > 0)
                    {
                        string cat = category.ToString();
                        result = (from r in db.tblreportinformations
                                  where r.ReportTitle.Contains(searchText) && r.CategoryBreadCrumb.Contains(cat) && r.IsUpComing == true && r.IsDeleted == false
                                  orderby r.ReportID descending
                                  select new SearchResultView
                                  {
                                      ReportId = r.ReportID,
                                      ReportTitle = r.ReportTitle,
                                      ReportUrl = r.ReportUrl,
                                      Description = r.MainPageDescription,
                                      PublishedDate = r.PublishDate,
                                      NumberOfPages = (int)r.NumberOfPage,
                                      Category = r.CategoryBreadCrumb,
                                      ReportFormat = (int)r.DeliveryFormat,
                                      SingleUser = r.PriceSingleUser,
                                      DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail",
                                      IsUpcoming = r.IsUpComing ?? false
                                  }).ToPagedList(page ?? 1, 10);
                    }
                    else
                    {
                        result = (from r in db.tblreportinformations
                                  where r.ReportTitle.Contains(searchText) && r.IsUpComing == true && r.IsDeleted == false
                                  orderby r.ReportID descending
                                  select new SearchResultView
                                  {
                                      ReportId = r.ReportID,
                                      ReportTitle = r.ReportTitle,
                                      ReportUrl = r.ReportUrl,
                                      Description = r.MainPageDescription,
                                      PublishedDate = r.PublishDate,
                                      NumberOfPages = (int)r.NumberOfPage,
                                      Category = r.CategoryBreadCrumb,
                                      ReportFormat = (int)r.DeliveryFormat,
                                      SingleUser = r.PriceSingleUser,
                                      DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail",
                                      IsUpcoming = r.IsUpComing ?? false
                                  }).ToPagedList(page ?? 1, 10);
                    }
                    break;
                case 2://Published
                    if (category > 0)
                    {
                        string cat = category.ToString();
                        result = (from r in db.tblreportinformations
                                  where r.ReportTitle.Contains(searchText) && r.CategoryBreadCrumb.Contains(cat) && r.IsUpComing == false && r.IsDeleted == false
                                  orderby r.ReportID descending
                                  select new SearchResultView
                                  {
                                      ReportId = r.ReportID,
                                      ReportTitle = r.ReportTitle,
                                      ReportUrl = r.ReportUrl,
                                      Description = r.MainPageDescription,
                                      PublishedDate = r.PublishDate,
                                      NumberOfPages = (int)r.NumberOfPage,
                                      Category = r.CategoryBreadCrumb,
                                      ReportFormat = (int)r.DeliveryFormat,
                                      SingleUser = r.PriceSingleUser,
                                      DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail",
                                      IsUpcoming = r.IsUpComing ?? false
                                  }).ToPagedList(page ?? 1, 10);
                    }
                    else
                    {
                        result = (from r in db.tblreportinformations
                                  where r.ReportTitle.Contains(searchText) && r.IsUpComing == false && r.IsDeleted == false
                                  orderby r.ReportID descending
                                  select new SearchResultView
                                  {
                                      ReportId = r.ReportID,
                                      ReportTitle = r.ReportTitle,
                                      ReportUrl = r.ReportUrl,
                                      Description = r.MainPageDescription,
                                      PublishedDate = r.PublishDate,
                                      NumberOfPages = (int)r.NumberOfPage,
                                      Category = r.CategoryBreadCrumb,
                                      ReportFormat = (int)r.DeliveryFormat,
                                      SingleUser = r.PriceSingleUser,
                                      DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail",
                                      IsUpcoming = r.IsUpComing ?? false
                                  }).ToPagedList(page ?? 1, 10);
                    }
                    break;
            }
            return page == null || page <= result.PageCount ? result : null;
        }

        public static IPagedList<SearchResultView> AllPublishedReports(int? page, int maxRows = 10)
        {
            ZionDbEntities db = new ZionDbEntities();
            System.Data.Entity.Core.Objects.ObjectParameter totalRow = new System.Data.Entity.Core.Objects.ObjectParameter("p_TotalRecords", 0);
            var reports = (from r in db.Zion_GetAllPublishedReports(page ?? 1, totalRow)
                           select new SearchResultView
                           {
                               ReportId = r.ReportId,
                               ReportUrl = r.ReportUrl,
                               Description = r.MainPageDescription,
                               SingleUser = r.SingleUser,
                               NumberOfPages = r.NumberOfPage,
                               PublishedDate = r.PublishDate,
                               ReportTitle = r.ReportTitle,
                               Category = r.Category,
                               Categories = r.Category.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                               ReportFormat = r.DeliveryFormat,
                               DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail"
                           }).ToList();

            var totalPages = Convert.ToInt32(totalRow.Value) / maxRows;

            totalPages = Convert.ToInt32(totalRow.Value) % maxRows == 0 ? totalPages : totalPages + 1;

            return page == null || page <= totalPages ? new StaticPagedList<SearchResultView>(reports, page ?? 1, maxRows, Convert.ToInt32(totalRow.Value)) : null;
        }

        /// <summary>
        /// Get all featured reports in PagedList
        /// </summary>
        /// <param name="page">Page number</param>
        /// <returns></returns>
        public static IPagedList<SearchResultView> AllFeaturedReports(int? page, int maxRows = 20)
        {
            //TODO: implement get all featured reports

            var result = new List<SearchResultView>();
            var p_totalRecords = new System.Data.Entity.Core.Objects.ObjectParameter("p_TotalRecords", 0);
            DatabaseContext.PerformAction(context =>
            {
                result = (from r in context.Zion_GetAllFeaturedReports(page, "Featured", p_totalRecords)
                          select new SearchResultView
                          {
                              ReportId = r.ReportId,
                              ReportUrl = r.ReportUrl,
                              Description = r.MainPageDescription,
                              SingleUser = r.SingleUser,
                              NumberOfPages = r.NumberOfPage,
                              PublishedDate = r.PublishDate,
                              ReportTitle = r.ReportTitle,
                              Category = r.Category,
                              Categories = r.Category.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                              ReportFormat = r.DeliveryFormat,
                              DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail"
                          }).ToList();
            });

            var totalPages = Convert.ToInt32(p_totalRecords.Value) / maxRows;

            return page == null || page <= totalPages ? new StaticPagedList<SearchResultView>(result, page ?? 1, maxRows, Convert.ToInt32(p_totalRecords.Value)) : null;
        }

        public static IPagedList<SearchResultView> AllUpcomingReports(int? page, int maxRows = 20)
        {
            ZionDbEntities db = new ZionDbEntities();
            System.Data.Entity.Core.Objects.ObjectParameter totalRow = new System.Data.Entity.Core.Objects.ObjectParameter("p_TotalRecords", 0);
            var reports = (from r in db.Zion_GetAllUpcomingReports(page ?? 1, totalRow)
                           select new SearchResultView
                           {
                               ReportId = r.ReportId,
                               ReportUrl = r.ReportUrl,
                               Description = r.MainPageDescription,
                               SingleUser = r.SingleUser,
                               NumberOfPages = r.NumberOfPage,
                               PublishedDate = r.PublishDate,
                               ReportTitle = r.ReportTitle,
                               Category = r.Category,
                               ReportFormat = r.DeliveryFormat,
                               DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail",
                               Categories = r.Category.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                               IsUpcoming = r.IsUpcoming == 1 ? true : false
                           }).ToList();

            var totalNumberOfPages = Convert.ToInt32(totalRow.Value) / maxRows;

            totalNumberOfPages = Convert.ToInt32(totalRow.Value) % maxRows == 0 ? totalNumberOfPages : totalNumberOfPages + 1;

            return page == null || page <= totalNumberOfPages ? new StaticPagedList<SearchResultView>(reports, page ?? 1, maxRows, Convert.ToInt32(totalRow.Value) < 10 ? 10 : Convert.ToInt32(totalRow.Value)) : null;
        }

        public static IEnumerable<SearchResultView> RelatedReports(string id)
        {
            IEnumerable<SearchResultView> res = default;
            if (int.TryParse(Util.Utility.Decryptstring(id), out int reportId))
            {
                DatabaseContext.PerformAction(db =>
                {
                    var reportids = (from r in db.tblreportinformations
                                     join cr in db.relreportcategories on r.ReportID equals cr.ReportId
                                     where r.ReportID == reportId
                                     select r.CategoryBreadCrumb).FirstOrDefault();
                    if (reportids != null)
                    {

                        var rr = reportids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x));
                        //string cat = relatedCategory.ToString();
                        res = (from r in db.tblreportinformations
                               join rc in db.relreportcategories on r.ReportID equals rc.ReportId
                               where rr.Contains((int)rc.CategoryId) && r.IsDeleted == false && r.ReportID != reportId
                               orderby r.ReportID descending
                               select new SearchResultView
                               {
                                   ReportId = r.ReportID,
                                   ReportUrl = r.ReportUrl,
                                   Description = r.MainPageDescription,
                                   SingleUser = r.PriceSingleUser,
                                   NumberOfPages = r.NumberOfPage,
                                   PublishedDate = r.PublishDate,
                                   ReportTitle = r.ReportTitle,
                                   Category = r.CategoryBreadCrumb,
                                   ReportFormat = r.DeliveryFormat,
                                   DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail"
                               }).Take(5);
                    }

                });
            }
            return res;
        }

        public static List<SearchResultView> AllUpcomingReports()
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from r in db.tblreportinformations
                        where r.IsDeleted == false && r.IsUpComing == true orderby r.UpdatedDate descending
                        select new SearchResultView
                        {
                            ReportTitle = r.ReportTitle,
                            Description = r.MainPageDescription,
                            ReportUrl = r.ReportUrl
                        }).ToList();
            }
        }

        public static List<SearchResultView> AllPublishedReports()
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var featuredReports = db.relreportattributes.Select(x => x.ReportID).Distinct().ToList();
                return (from r in db.tblreportinformations
                        where r.IsDeleted == false && r.IsUpComing == false && !featuredReports.Contains(r.ReportID)
                        orderby r.PublishDate descending
                        select new SearchResultView
                        {
                            ReportTitle = r.ReportTitle,
                            //Description = r.MainPageDescription,
                            ReportUrl = r.ReportUrl,
                            PublishedDate = r.PublishDate
                        }).ToList();
            }
        }

        public static List<SearchResultView> AllFrPublishedReports()
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var featuredReports = db.relreportattributes.Select(x => x.ReportID).Distinct().ToList();
                return (from r in db.tblreportinformations
                        where r.IsDeleted == false && r.IsUpComing == false && !featuredReports.Contains(r.ReportID)
                        orderby r.PublishDate descending
                        select new SearchResultView
                        {
                            ReportTitle = r.ReportTitle,
                            Description = r.MainPageDescription,
                            ReportUrl = "/fr/"+r.ReportUrl,
                            PublishedDate = r.PublishDate
                        }).ToList();
            }
        }

        public static List<SearchResultView> AllDePublishedReports()
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var featuredReports = db.relreportattributes.Select(x => x.ReportID).Distinct().ToList();
                return (from r in db.tblreportinformations
                        where r.IsDeleted == false && r.IsUpComing == false && !featuredReports.Contains(r.ReportID)
                        orderby r.PublishDate descending
                        select new SearchResultView
                        {
                            ReportTitle = r.ReportTitle,
                            Description = r.MainPageDescription,
                            ReportUrl = "/de/" + r.ReportUrl,
                            PublishedDate = r.PublishDate
                        }).ToList();
            }
        }
        public static List<SearchResultView> AllReports()
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from r in db.tblreportinformations
                        where r.IsDeleted == false && r.IsActive == true
                        select new SearchResultView
                        {
                            ReportTitle = r.ReportTitle,
                            Description = r.MainPageDescription,
                            ReportUrl = r.ReportUrl,
                            PublishedDate = r.PublishDate,
                            IsUpcoming = r.IsUpComing ?? false,
                            DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail",
                        }).ToList();
            }
        }

        public static List<AutoComplete> AutoComplete(string query)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from r in db.tblreportinformations
                        where (r.ReportTitle.Contains(query) || r.MainPageDescription.Contains(query)) && r.IsDeleted == false && r.IsActive == true
                        orderby r.ReportID descending
                        select new AutoComplete
                        {
                            value = r.ReportTitle,
                            data = r.ReportUrl
                        }).Take(50).ToList();
            }
        }

        public static List<SearchResultView> FeaturedReports()
        {
            var reports = new List<SearchResultView>();
            DatabaseContext.PerformAction(context =>
            {
                reports = (from r in context.tblreportinformations
                           join r_Attr in context.relreportattributes on r.ReportID equals r_Attr.ReportID
                           join attr in context.tblreportattributes on r_Attr.AttributeID equals attr.ID
                           where attr.AttributeName == "Featured" && r.IsDeleted == false
                           select new SearchResultView
                           {
                               ReportId = r.ReportID,
                               ReportTitle = r.ReportTitle,
                               ReportUrl = r.ReportUrl
                           }).ToList();
            });
            return reports;
        }

        public static string GetMethodology(string url)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return db.tblreportinformations.Where(x => x.ReportUrl == url && x.IsUpComing == false).FirstOrDefault().Methodology;
            }
        }

        public static MainPageReportView getAllHeadingTags(MainPageReportView val)
        {
            MainPageReportView result = val;
            string output = null;
            if (!string.IsNullOrEmpty(val.MainPageDescription))
            {
                //MatchCollection matches = Regex.Matches(val.MainPageDescription, "(<h2)|(<h3)");
                MatchCollection tocmatches = Regex.Matches(val.MainPageDescription, "(<h2)(.*?)</h2>|(<h3)(.*?)</h3>");
                for (int i = tocmatches.Count; i > 0; i--)
                {
                    result.MainPageDescription = result.MainPageDescription.Insert(tocmatches[i - 1].Index + 3, " id='toc" + (i).ToString() + "'");
                    output = "<a class='smooth-anchor' href='#toc" + (i).ToString() + "'>" + Util.Utility.StripHTML(tocmatches[i - 1].Value.Substring(tocmatches[i - 1].Value.IndexOf('>') + 1, tocmatches[i - 1].Value.LastIndexOf('<') - tocmatches[i - 1].Value.IndexOf('>') - 1)) + "</a>" + output;

                }
                if (output != null)
                {

                    output = "<h4 class=\"list-group-item-heading\">List of Contents</h4>" + output.Replace(GetKeywordFromReportTitle(result.ReportTitle,result.ReportUrl), "").Replace("Global Market: ", "").Replace("Market : ", "").Replace("Market: ", "");
                    result.MetaKeywordsTOC = output;

                }

            }
            return result;
        }

        public static string GetKeywordFromReportTitle(string ReportTitle, string ReportUrl)
        {
            string res = "";
            int marketIndex;
                                  
            if (ReportTitle.IndexOf("Market ") > 0)
                marketIndex = ReportTitle.IndexOf("Market ");
            else if (ReportTitle.IndexOf("Market:") > 0)
                marketIndex = ReportTitle.IndexOf("Market:");
            else if(ReportTitle.IndexOf("Market :") > 0)
                marketIndex = ReportTitle.IndexOf("Market :");
            else if(ReportTitle.IndexOf("Market-") > 0)
                marketIndex = ReportTitle.IndexOf("Market-");
            else if (ReportTitle.IndexOf("Market,") > 0)
                marketIndex = ReportTitle.IndexOf("Market,");
            else
                marketIndex = -1;
            string smallTitle = marketIndex > -1 ? 
                ReportTitle.Substring(0, marketIndex).Replace("Global", string.Empty) :
                System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(ReportUrl.Replace("-", " ")).Replace("Global", string.Empty).Replace(" Market", string.Empty);
            string h1Title1 = smallTitle.Length > 50 ? smallTitle.Substring(0, smallTitle.Substring(0, 49).LastIndexOf(" ")) : smallTitle;
            res = h1Title1;
            return res;
        }
        #endregion
    }

    #region ModelView
    /// <summary>
    /// This class used for View LatestReport
    /// </summary>
    public class LatestReportView
    {
        public int ReportId { get; set; }
        public string ReportTitle { get; set; }
        public string ReportUrl { get; set; }
        public string Category { get; set; }
        public DateTime? PublishedDate { get; set; }
        [UIHint("LatestReportDescriptionTemplate")]
        public string Description { get; set; }
        public string[] Categories { get; set; }
        public IEnumerable<string> ReportAttribute { get; set; }
        public bool? IsUpcoming { get; set; }
        public int? NumberOfPages { get; set; }
    }

    /// <summary>
    /// This class used for View ReportDetail
    /// </summary>
    public class MainPageReportView
    {
        public int ReportId { get; set; }
        public string ReportTitle { get; set; }
        public string ReportUrl { get; set; }
        public string MainPageDescription { get; set; }
        public string MainPageImagePath { get; set; }
        public string SearchPageDescription { get; set; }
        public string MainPageImageAltAttribute { get; set; }
        public decimal SingleUser { get; set; }
        public decimal? MultiUser { get; set; }
        public decimal? CorporateUser { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeyword { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string Category { get; set; }
        public string[] Categories { get; set; }
        public int? NumberOfPages { get; set; }
        public string TableOfContent { get; set; }
        public string ListOfTabels { get; set; }
        public string ListOfFigures { get; set; }
        public string FreeAnalysis { get; set; }
        public string Methodology { get; set; }
        public int DeliveryFormat { get; set; }
        public bool IsUpcoming { get; set; }
        public string TOCUrl { get; set; }
        public string RequestSampleUrl { get; set; }
        public string BreadCrumbTitle { get; set; }
        public string SingleUserLink { get; set; }
        public string MultiUserLink { get; set; }
        public string CorporateUserLink { get; set; }
        public string BuyingInquiryLink { get; set; }
        public string FreeAnalysisLink { get; set; }
        public string[] RelatedNews { get; set; }
        public string Tags { get; set; }
        public string MetaTitleSample { get; set; }
        public string MetaDescriptionSample { get; set; }
        public string MetaKeywordsSample { get; set; }
        public string MetaTitleTOC { get; set; }
        public string MetaDescriptionTOC { get; set; }
        public string MetaKeywordsTOC { get; set; }
        public string MetaTitleFreeAnalysis { get; set; }
        public string MetaDescriptionFreeAnalysis { get; set; }
        public string MetaKeywordsFreeAnalysis { get; set; }
        public string MetaTitleInquiry { get; set; }
        public string MetaDescriptionInquiry { get; set; }
        public string MetaKeywordsInquiry { get; set; }
        public bool IsFeatured { get; set; }
        public string URL { get; set; }

        public string ReSellerMainPageDescription { get; set; }
        /// <summary>
        /// Property is used for sending categories to CRM
        /// </summary>
        public string CRMCategory { get; set; }
        decimal _discountSingleUser;
        public decimal DiscountSingleUser
        {
            get { return Math.Round(_discountSingleUser, 2); }

            set { _discountSingleUser = value; }
        }
        decimal? _discountMultiUser;
        public decimal? DiscountMultiUser
        {
            get
            {
                if (_discountMultiUser != null)
                {
                    return Math.Round((decimal)_discountMultiUser, 2);
                }
                else
                {
                    return _discountMultiUser;
                }
            }
            set { _discountMultiUser = value; }
        }
        decimal? _discountCorporateUser;
        public decimal? DiscountCorporateUser
        {
            get
            {
                if (_discountCorporateUser != null)
                {
                    return Math.Round((decimal)_discountCorporateUser, 2);
                }
                else
                {
                    return _discountCorporateUser;
                }
            }
            set { _discountCorporateUser = value; }
        }

        public List<ReportFAQ> FAQ { get; set; }
    }

    public class ReportFAQ
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int RowNumber { get; set; }
    }

    public class LatestUpComingReportView
    {
        //
        public int ReportId { get; set; }
        public string ReportTitle { get; set; }
        public string ReportUrl { get; set; }
        public string Category { get; set; }
        public DateTime? PublishedDate { get; set; }
        [UIHint("LatestReportDescriptionTemplate")]
        public string Description { get; set; }
        public string[] Categories { get; set; }
    }

    public class CustomView
    {
        public int ReportId { get; set; }
        public string ReportTitle { get; set; }
        public string ReportUrl { get; set; }
    }

    public class SearchParam
    {
        public int o { get; set; }//SearchType
        public string z { get; set; }//SearchText
        public int i { get; set; }//Category
        public int n { get; set; }//ReportType
        public int? p { get; set; }//Page

    }
    /// <summary>
    /// Class is responsible for showing search results
    /// </summary>
    public class SearchResultView
    {
        public int ReportId { get; set; }
        public string ReportTitle { get; set; }
        [UIHint("SearchResultDescription")]
        public string Description { get; set; }
        public string ReportUrl { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string Category { get; set; }
        public string[] Categories { get; set; }
        public decimal? SingleUser { get; set; }
        public int? NumberOfPages { get; set; }
        public int? ReportFormat { get; set; }
        public string DeliveryFormatClass { get; set; }
        public bool IsUpcoming { get; set; }
        public string ReportAttribute { get; set; }
    }

    public class AutoComplete
    {
        public string value { get; set; }
        public string data { get; set; }
        public string ImageUrl { get; set; }
    }
    #endregion

    public class MetaData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }

        public static MetaData GetMetaData(object o, string endwith)
        {
            var metadata = o.GetType().GetProperties().Where(x => x.Name.ToLower().Contains("meta"));
            MetaData m = new MetaData();
            var title = metadata.Where(x => x.Name.ToLower().Contains("title" + endwith) || x.Name.ToLower().Contains("metatitle" + endwith)).FirstOrDefault();
            var description = metadata.Where(x => x.Name.ToLower().Contains("metadescription" + endwith)).FirstOrDefault();
            var keywords = metadata.Where(x => x.Name.ToLower().Contains("metakeywords" + endwith)).FirstOrDefault();
            m.Title = title != null && title.GetValue(o) != null ? title.GetValue(o).ToString() : string.Empty;
            m.Description = description != null && description.GetValue(o) != null ? description.GetValue(o).ToString() : string.Empty;
            m.Keywords = keywords != null && keywords.GetValue(o) != null ? keywords.GetValue(o).ToString() : string.Empty;
            return m;
        }
    }

}