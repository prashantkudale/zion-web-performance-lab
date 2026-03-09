using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZionMarketResearch.Areas.Admin.Models;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Models
{
    public class CategoryRepository
    {
        /// <summary>
        /// This method returns List of parent category with report count
        /// </summary>
        /// <returns></returns>
        public static List<CategoryReportView> GetCategoryWithReportCount()
        {
            //using (ZionDbEntities db = new ZionDbEntities())
            //{

            //    var result = db.Zion_GetCategoryWithReportCount();
            //    var categories = (from c in result
            //                      select new CategoryReportView
            //                      {
            //                          CategoryId = c.CategoryId,
            //                          CategoryName = c.CategoryName,
            //                          //ReportCount = c.ReportCount,
            //                          CategoryUrl = "/category/" + c.CategoryUrl
            //                      });
            //    return categories.ToList();
            //}
            return null;
        }

        public static List<Category> GetParentCategory()
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var categories = db.Database.SqlQuery<Category>("SELECT pkCategoryId CategoryId, CategoryName, CONCAT('/category/', CategoryUrl) CategoryUrl FROM tblcategory c WHERE (c.ParentCategoryId is null OR c.ParentCategoryId = 0) AND c.IsDeleted = 0").ToList();
                return categories;
            }
        }

        public static List<Category> GetAllCategories()
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var categories = (from c in db.tblcategories
                                  where c.IsDeleted == false
                                  select new Category
                                  {
                                      CategoryId = c.pkCategoryID,
                                      CategoryName = c.CategoryName,
                                      CategoryUrl = "/category/" + c.CategoryUrl
                                  }).ToList();
                return categories;
            }
        }

        public static Category GetCategory(string categoryUrl)
        {
            Category cat = new Category();
            return cat;
        }

        public static CategoryReport GetCategoryWithReports(string categoryUrl, int? page, int maxRows = 20)
        {
            ZionDbEntities db = new ZionDbEntities();
            CategoryReport cr = new CategoryReport();

            var categorywithreport = (from c in db.tblcategories
                                      where c.CategoryUrl == categoryUrl && c.IsDeleted == false
                                      select c).SingleOrDefault();
            if (categorywithreport != null)
            {
                string str_categoryId = categorywithreport.pkCategoryID.ToString();

                //TODO: better way to get All/Published/Upcoming reports by category??? Please find it

                var reports = (from r in db.tblreportinformations
                               where r.CategoryBreadCrumb.Contains(str_categoryId) && r.IsDeleted == false
                               orderby r.ReportID descending
                               select new SearchResultView
                               {
                                   ReportId = r.ReportID,
                                   ReportTitle = r.ReportTitle,
                                   ReportUrl = r.ReportUrl,
                                   SingleUser = r.PriceSingleUser,
                                   PublishedDate = r.PublishDate,
                                   ReportFormat = r.DeliveryFormat,
                                   Description = r.MainPageDescription,
                                   DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail",
                                   IsUpcoming = (bool)r.IsUpComing,
                                   NumberOfPages = r.NumberOfPage
                               }).ToPagedList(page ?? 1, maxRows);

                //var publishedReports = (from r in db.tblreportinformations
                //                        where r.CategoryBreadCrumb.Contains(str_categoryId) && r.IsDeleted == false && r.IsUpComing == false
                //                        orderby r.ReportID descending
                //                        select new SearchResultView
                //                        {
                //                            ReportId = r.ReportID,
                //                            ReportTitle = r.ReportTitle,
                //                            ReportUrl = r.ReportUrl,
                //                            SingleUser = r.PriceSingleUser,
                //                            PublishedDate = r.PublishDate,
                //                            ReportFormat = r.DeliveryFormat,
                //                            Description = r.MainPageDescription,
                //                            DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail",
                //                            IsUpcoming = (bool)r.IsUpComing,
                //                            NumberOfPages = r.NumberOfPage
                //                        }).ToPagedList(page ?? 1, maxRows);

                //var upcomingReports = (from r in db.tblreportinformations
                //                       where r.CategoryBreadCrumb.Contains(str_categoryId) && r.IsDeleted == false && r.IsUpComing == true
                //                       orderby r.ReportID descending
                //                       select new SearchResultView
                //                       {
                //                           ReportId = r.ReportID,
                //                           ReportTitle = r.ReportTitle,
                //                           ReportUrl = r.ReportUrl,
                //                           SingleUser = r.PriceSingleUser,
                //                           PublishedDate = r.PublishDate,
                //                           ReportFormat = r.DeliveryFormat,
                //                           Description = r.MainPageDescription,
                //                           DeliveryFormatClass = r.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : r.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : r.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : r.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail",
                //                           IsUpcoming = (bool)r.IsUpComing,
                //                           NumberOfPages = r.NumberOfPage
                //                       }).ToPagedList(page ?? 1, maxRows);

                cr.Category = categorywithreport.CategoryName;
                cr.CategryId = categorywithreport.pkCategoryID;
                cr.Description = categorywithreport.TopDescription;//.Length > 500 ? categorywithreport.TopDescription.Substring(0, 500) + " <a onclick=\"readMore(this)\" style=\"cursor: pointer; \">Read More</a>" + categorywithreport.TopDescription.Substring(500) : categorywithreport.TopDescription;
                cr.MetaTitle = categorywithreport.Title;
                cr.MetaDescription = categorywithreport.MetaDescription;
                cr.MetaKeywords = categorywithreport.Keywords;
                cr.CategoryUrl = categorywithreport.CategoryUrl;
                cr.Reports = reports;
                cr.PublishedReports = null; // publishedReports;
                cr.UpcomingReports = null; // upcomingReports;

                return cr;
            }
            return null;
        }

        public static IPagedList<SearchResultView> GetReportsByCategory(string categoryUrl, int? page, int maxRows = 20)
        {
            IPagedList<SearchResultView> reports = (IPagedList<SearchResultView>)null;
            DatabaseContext.PerformAction(cx =>
            {

                string str_categoryId = cx.tblcategories.Where(c => c.CategoryUrl == categoryUrl).FirstOrDefault().pkCategoryID.ToString();

                reports = cx.tblreportinformations.Where(x => x.CategoryBreadCrumb.Contains(str_categoryId) && x.IsDeleted == false).Select(x => new SearchResultView
                {
                    ReportId = x.ReportID,
                    ReportTitle = x.ReportTitle,
                    ReportUrl = x.ReportUrl,
                    SingleUser = x.PriceSingleUser
                }).OrderByDescending(x => x.ReportId).ToPagedList(page ?? 1, maxRows);
            });
            return reports;
        }

        public static IPagedList<SearchResultView> GetPublishedReportsByCategory(string categoryUrl, int? page, int maxRows = 20)
        {
            IPagedList<SearchResultView> reports = (IPagedList<SearchResultView>)null;
            DatabaseContext.PerformAction(cx =>
            {

                string str_categoryId = cx.tblcategories.Where(c => c.CategoryUrl == categoryUrl).FirstOrDefault().pkCategoryID.ToString();

                reports = cx.tblreportinformations.Where(x => x.CategoryBreadCrumb.Contains(str_categoryId) && x.IsUpComing == false && x.IsDeleted == false).Select(x => new SearchResultView
                {
                    ReportId = x.ReportID,
                    ReportTitle = x.ReportTitle,
                    ReportUrl = x.ReportUrl,
                    SingleUser = x.PriceSingleUser
                }).OrderByDescending(x => x.ReportId).ToPagedList(page ?? 1, maxRows);
            });
            return reports;
        }

        public static IPagedList<SearchResultView> GetUpcomingReportsByCategory(string categoryUrl, int? page, int maxRows = 20)
        {
            IPagedList<SearchResultView> reports = (IPagedList<SearchResultView>)null;
            DatabaseContext.PerformAction(cx =>
            {

                string str_categoryId = cx.tblcategories.Where(c => c.CategoryUrl == categoryUrl).FirstOrDefault().pkCategoryID.ToString();

                reports = cx.tblreportinformations.Where(x => x.CategoryBreadCrumb.Contains(str_categoryId)
                && x.IsUpComing == true
                && x.IsDeleted == false).Select(x => new SearchResultView
                {
                    ReportId = x.ReportID,
                    ReportTitle = x.ReportTitle,
                    ReportUrl = x.ReportUrl,
                    SingleUser = x.PriceSingleUser
                }).OrderByDescending(x => x.ReportId).ToPagedList(page ?? 1, maxRows);
            });
            return reports;
        }

    }

}

public class CategoryReportView
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int ReportCount { get; set; }
    public string CategoryUrl { get; set; }
}

public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryUrl { get; set; }
}

public class CategoryReport
{
    public int CategryId { get; set; }
    public string Category { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public string MetaKeywords { get; set; }
    public string Description { get; set; }
    public string CategoryUrl { get; set; }
    public IPagedList<SearchResultView> Reports { get; set; }
}
