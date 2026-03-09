using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionAdmin.Models;
using ZionAdmin.Models.LuceneSearch;
using ZionMarketResearch.Areas.Admin.Models;
using ZionMarketResearch.Models;

namespace ZionAdmin.Repository
{
    public class ReportRepository
    {
        #region properties
        public int ReportId { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string ReportTitle { get; set; }
        public string ReportUrl { get; set; }
        [Required(ErrorMessage = "Main Page Description is required.")]
        [AllowHtml]
        public string MainPageDescription { get; set; }
        [AllowHtml]
        public string SearchPageDescription { get; set; }
        [Required(ErrorMessage = "Catgory is required")]
        public int CategoryId { get; set; }
        public string MainPageImagePath { get; set; }
        public string MainPageImageAltAtrribute { get; set; }
        public string SearchPageImagePath { get; set; }
        public string SearchPageImageAltAttribute { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeywords { get; set; }
        [AllowHtml]
        public string TableOfContent { get; set; }
        [AllowHtml]
        public string ListOfFigure { get; set; }
        public DateTime? PublishedDate { get; set; }
        [AllowHtml]
        public string Methodology { get; set; }
        [AllowHtml]
        public string FreeAnalysis { get; set; }
        [AllowHtml]
        public string ListOfTable { get; set; }
        public decimal SingleUser { get; set; }
        public decimal? MultiUser { get; set; }
        public decimal? Corporate { get; set; }
        public decimal? Discount { get; set; }
        public bool IsUpcoming { get; set; }
        [AllowHtml]
        public string ListOfChart { get; set; }
        public string Tags { get; set; }
        public string CategoryBreadcrumb { get; set; }
        public int? NumberOfPages { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int DeletedBy { get; set; }
        public DateTime DeletedDate { get; set; }
        public int Location { get; set; }
        public bool IsActive { get; set; }
        public int[] CategoryIdBreadCrumb { get { CategoryBreadcrumb.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).CopyTo(CategoryIdBreadCrumb, 0); return CategoryIdBreadCrumb; } }
        public string JsonCategoryBreadcrumb { get; set; }
        public bool AppendId { get; set; }
        public int DeliveryFormat { get; set; }
        public enum enDeliveryFormat
        {
            PDF = 0,
            DOC = 1,
            EXL = 2,
            PPT = 3,
            EMAIL = 4
        }
        public string MetaTitleSample { get; set; }
        public string MetaDescriptionSample { get; set; }
        public string MetaKeywordsSample { get; set; }
        public string MetaTitleTOC { get; set; }
        [AllowHtml]
        public string MetaDescriptionTOC { get; set; }
        public string MetaKeywordsTOC { get; set; }
        public string MetaTitleFreeAnalysis { get; set; }
        public string MetaDescriptionFreeAnalysis { get; set; }
        public string MetaKeywordsFreeAnalysis { get; set; }
        public string MetaTitleInquiry { get; set; }
        public string MetaDescriptionInquiry { get; set; }
        public string MetaKeywordsInquiry { get; set; }
        public int[] ReportAttribute { get; set; }
        public List<ReportFAQ> FAQ { get; set; }
        public string ResellerMainPageDescription { get; set; }

        public bool IsResellerDescriptionAdded { get; set; }

        #endregion

        #region methods
        public static int Create(ReportRepository report, HttpFileCollectionBase files)
        {
            Dictionary<string, string> fileNames = Utility.UploadFiles(files);
            if (fileNames != null && fileNames.Keys.Count() > 0)
            {
                if (fileNames.ContainsKey("File1") && fileNames["File1"] != string.Empty)
                    report.MainPageImagePath = fileNames["File1"];
                if (fileNames.ContainsKey("File2") && fileNames["File2"] != string.Empty)
                    report.SearchPageImagePath = fileNames["File2"];
            }

            #region Object Parameter
            ObjectParameter out_ReportTitle = new ObjectParameter("p_NewReportTitle", string.Empty);
            ObjectParameter out_ReportId = new ObjectParameter("p_NewReportId", 0);
            ObjectParameter out_Category = new ObjectParameter("p_ReportCategoryName", string.Empty);
            ObjectParameter out_ReportUrl = new ObjectParameter("p_NewReportUrl", string.Empty);
            #endregion

            var res = 0;

            DatabaseContext.PerformAction(db =>
            {
                res = (int)db.Zion_AddReportinformation(
                    report.ReportTitle,
                    report.TableOfContent,
                    report.ListOfTable,
                    report.ListOfFigure,
                    report.MainPageDescription,
                    report.SearchPageDescription,
                    report.PublishedDate,
                    report.SingleUser,
                    report.MultiUser,
                    report.Corporate,
                    report.Location,
                    report.Discount,
                    report.NumberOfPages,
                    report.CategoryId,
                    report.MetaTitle,
                    report.MetaKeywords,
                    report.MetaDescription,
                    report.ListOfChart,
                    !string.IsNullOrEmpty(report.ReportUrl) ? System.Text.RegularExpressions.Regex.Replace(report.ReportUrl, @"[^A-Za-z0-9,^-]", "") : report.ReportUrl,
                    (sbyte)(report.IsActive ? 1 : 0),
                    (sbyte)(report.IsUpcoming ? 1 : 0),
                    ((Security.CustomPrincipal)HttpContext.Current.User).UserId,
                    report.Methodology,
                    report.FreeAnalysis,
                    report.MainPageImagePath,
                    report.MainPageImageAltAtrribute,
                    report.SearchPageImagePath,
                    report.SearchPageImageAltAttribute,
                    report.Tags,
                    out_ReportId,
                    out_Category,
                    out_ReportUrl,
                    out_ReportTitle,
                    (sbyte)(report.AppendId ? 1 : 0),
                    report.DeliveryFormat,
                    report.MetaTitleSample,
                    report.MetaDescriptionSample,
                    report.MetaKeywordsSample,
                    report.MetaTitleTOC,
                    report.MetaDescriptionTOC,
                    report.MetaKeywordsTOC,
                    report.MetaTitleFreeAnalysis,
                    report.MetaDescriptionFreeAnalysis,
                    report.MetaKeywordsFreeAnalysis,
                    report.MetaTitleInquiry,
                    report.MetaDescriptionInquiry,
                    report.MetaKeywordsInquiry,
                    report.ResellerMainPageDescription
                    ).FirstOrDefault();


                #region Add Report to Lucene Search Index
                if (out_ReportId != null && out_ReportId.Value != null)
                {
                    _addLuceneIndex(new LuceneReport
                    {
                        ReportId = Convert.ToInt32(out_ReportId.Value),
                        ReportTitle = out_ReportTitle.Value.ToString(),
                        Category = out_Category.Value.ToString(),
                        ReportUrl = out_ReportUrl.Value.ToString()
                    });

                    if (report.ReportAttribute != null)
                    {
                        foreach (var a in report.ReportAttribute)
                        {
                            db.relreportattributes.Add(new relreportattribute
                            {
                                ReportID = (int)out_ReportId.Value,
                                AttributeID = a
                            });
                        }
                        db.SaveChanges();
                    }
                }
                #endregion


                #region Add Report FAQ
                if (report.FAQ != null && report.FAQ.Count > 0)
                {
                    foreach (var faq in report.FAQ)
                    {
                        db.tblreportfaqs.Add(new tblreportfaq
                        {
                            Question = faq.Question,
                            Answer = faq.Answer,
                            RowNumber = faq.RowIndex,
                            ReportID = Convert.ToInt32(out_ReportId.Value)
                        });
                    }
                    db.SaveChanges();
                }
                #endregion
            });

            return res;
        }

        public static IPagedList<ReportRepository> List(bool? showUpcoming, bool? showPublished, bool? showIsDeleted, int? page, int? year, int? month)
        {
            showPublished = showPublished ?? false;
            showUpcoming = showUpcoming ?? false;
            showIsDeleted = showIsDeleted ?? false;
            IQueryable<ReportAndReseller> res = default;
            IPagedList<ReportRepository> result = default;
            DatabaseContext.PerformAction(db =>
            {

                res = from r in db.tblreportinformations
                      //where r.IsDeleted == false
                      join reseller in db.tblreseller_reportinformation on r.ReportID equals reseller.ReportID
                            into resellerJoin
                      from reseller in resellerJoin.DefaultIfEmpty()
                      orderby r.ReportID descending
                      select new ReportAndReseller { Report = r, Reseller = reseller };

                if ((bool)showUpcoming && !(bool)showPublished && !(bool)showIsDeleted)
                {
                    res = res.Where(x => x.Report.IsDeleted == false && x.Report.IsUpComing == true);
                }
                else if (!(bool)showUpcoming && (bool)showPublished && !(bool)showIsDeleted)
                {
                    res = res.Where(x => x.Report.IsDeleted == false && x.Report.IsUpComing == false);
                }
                else if (!(bool)showUpcoming && !(bool)showPublished && (bool)showIsDeleted)
                {
                    res = res.Where(x => x.Report.IsDeleted == true);
                }
                if (year.HasValue)
                {
                    res = res.Where(x => x.Report.IsDeleted == false && x.Report.PublishDate != null && x.Report.PublishDate.Value.Year == year);
                }

                if (month.HasValue)
                {
                    res = res.Where(x => x.Report.IsDeleted == false && x.Report.PublishDate != null && x.Report.PublishDate.Value.Month == month);
                }

                result = res.Select(r => new ReportRepository
                {
                    ReportId = r.Report.ReportID,
                    ReportTitle = r.Report.ReportTitle,
                    ReportUrl = r.Report.ReportUrl,
                    IsUpcoming = (bool)r.Report.IsUpComing,
                    PublishedDate = r.Report.PublishDate
                    //,
                    //IsResellerDescriptionAdded = r.Reseller != null && r.Reseller.MainPageDescription != null,
                    //ResellerMainPageDescription = r.Reseller != null ? r.Reseller.MainPageDescription : null
                }).ToPagedList<ReportRepository>(page ?? 1, 100);
            });
            return result;
        }

        public static ReportRepository Get(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var report = (from r in db.tblreportinformations
                              where r.ReportID == id
                              select new ReportRepository
                              {
                                  ReportId = r.ReportID,
                                  ReportTitle = r.ReportTitle,
                                  MainPageDescription = r.MainPageDescription,
                                  SearchPageDescription = r.SearchPageDescription,
                                  CategoryBreadcrumb = r.CategoryBreadCrumb,
                                  MainPageImageAltAtrribute = r.MainPageImageAltAttribute,
                                  SearchPageImageAltAttribute = r.SearchPageImageAltAttribute,
                                  MainPageImagePath = r.MainPageImagePath,
                                  SearchPageImagePath = r.SearchPageImagePath,
                                  MetaTitle = r.MetaTitle,
                                  MetaDescription = r.MetaDescription,
                                  MetaKeywords = r.MetaKeywords,
                                  TableOfContent = r.TableofContents,
                                  ListOfChart = r.ListOfCharts,
                                  ListOfFigure = r.ListOfFigure,
                                  ListOfTable = r.ListOfTable,
                                  Location = (int)r.Location,
                                  PublishedDate = r.PublishDate,
                                  IsUpcoming = (bool)r.IsUpComing,
                                  ReportUrl = r.ReportUrl,
                                  FreeAnalysis = r.FreeAnalysis,
                                  Methodology = r.Methodology,
                                  SingleUser = (decimal)r.PriceSingleUser,
                                  MultiUser = r.PriceEnterpriseLicense,
                                  Corporate = r.PriceCUL,
                                  Discount = r.DiscountPercentage,
                                  NumberOfPages = r.NumberOfPage,
                                  Tags = r.Tags,
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
                                  IsActive = (bool)r.IsActive
                              }).FirstOrDefault();

                report.ReportAttribute = db.relreportattributes.Where(x => x.ReportID == id).Select(x => x.AttributeID).ToArray();

                report.JsonCategoryBreadcrumb = CategoryRepository.GetCategoryJsonString(report.CategoryBreadcrumb);

                report.FAQ = (from faq in db.tblreportfaqs
                              where faq.ReportID == id
                              orderby faq.RowNumber
                              select new ReportFAQ
                              {
                                  ReportID = (int)faq.ReportID,
                                  Question = faq.Question,
                                  Answer = faq.Answer,
                                  RowIndex = (int)faq.RowNumber
                              }).ToList();
                //report.CategoryId = Convert.ToInt32(report.CategoryBreadcrumb);
                return report;
            }
        }

        public static int Update(ReportRepository report, HttpFileCollectionBase files)
        {
            var res = 0;
            DatabaseContext.PerformAction(db =>
            {
                //get report originial image path to delete old images
                ReportRepository rp = (from r in db.tblreportinformations
                                       where r.ReportID == report.ReportId
                                       select new ReportRepository
                                       {
                                           ReportId = r.ReportID,
                                           ReportTitle = r.ReportTitle,
                                           MainPageImagePath = r.MainPageImagePath,
                                           SearchPageImagePath = r.SearchPageImagePath,
                                           IsUpcoming = (bool)r.IsUpComing
                                       }).SingleOrDefault();

                #region Upload Image

                if (rp.MainPageImagePath != report.MainPageImagePath && !string.IsNullOrEmpty(rp.MainPageImagePath))
                {
                    Utility.DeleteFile(rp.MainPageImagePath);
                }

                if (rp.SearchPageImagePath != report.SearchPageImagePath && !string.IsNullOrEmpty(rp.SearchPageImagePath))
                {
                    Utility.DeleteFile(rp.SearchPageImagePath);
                }

                Dictionary<string, string> fileNames = Utility.UploadFiles(files);
                if (fileNames != null && fileNames.Keys.Count() > 0)
                {
                    if (fileNames.ContainsKey("File1") && fileNames["File1"] != string.Empty)
                    {
                        report.MainPageImagePath = fileNames["File1"];
                    }

                    if (fileNames.ContainsKey("File2") && fileNames["File2"] != string.Empty)
                    {
                        report.SearchPageImagePath = fileNames["File2"];
                    }
                }
                #endregion

                #region ObjectParameter
                ObjectParameter out_ReportId = new ObjectParameter("p_UpdateReportId", 0);
                ObjectParameter out_Category = new ObjectParameter("p_UpdateCategory", string.Empty);
                ObjectParameter out_ReportTitle = new ObjectParameter("p_UpdateReportTitle", string.Empty);
                ObjectParameter out_ReportUrl = new ObjectParameter("p_UpdateReportUrl", string.Empty);
                ObjectParameter out_UpdatedRows = new ObjectParameter("p_RowCount", 0);
                #endregion

                res = (int)(db.Zion_UpdateReportInformation(
                    report.ReportId,
                    report.ReportTitle,
                    report.TableOfContent,
                    report.ListOfTable,
                    report.ListOfFigure,
                    report.MainPageDescription,
                    report.SearchPageDescription,
                    report.PublishedDate,
                    report.SingleUser,
                    report.MultiUser,
                    report.Corporate,
                    report.Location.ToString(),
                    report.Discount,
                    report.NumberOfPages,
                    report.MetaTitle,
                    report.MetaDescription,
                    report.MetaKeywords,
                    report.ListOfChart,
                    report.ReportUrl,
                    report.CategoryId,
                    (sbyte)(report.IsActive ? 1 : 0),
                    ((Security.CustomPrincipal)HttpContext.Current.User).UserId,
                    report.Methodology,
                    (sbyte)(report.IsUpcoming ? 1 : 0),
                    report.FreeAnalysis,
                    report.MainPageImagePath,
                    report.SearchPageImagePath,
                    report.MainPageImageAltAtrribute,
                    report.SearchPageImageAltAttribute,
                    out_ReportId,
                    out_ReportTitle,
                    out_Category,
                    out_ReportUrl,
                    out_UpdatedRows,
                    report.DeliveryFormat,
                    report.MetaTitleSample,
                    report.MetaDescriptionSample,
                    report.MetaKeywordsSample,
                    report.MetaTitleTOC,
                    report.MetaDescriptionTOC,
                    report.MetaKeywordsTOC,
                    report.MetaTitleFreeAnalysis,
                    report.MetaDescriptionFreeAnalysis,
                    report.MetaKeywordsFreeAnalysis,
                    report.MetaTitleInquiry,
                    report.MetaDescriptionInquiry,
                    report.MetaKeywordsInquiry,
                    report.ResellerMainPageDescription)).FirstOrDefault();


                #region Update Report FAQ
                var reportFaq = db.tblreportfaqs.Where(x => x.ReportID == report.ReportId).ToList();
                if (reportFaq != null && reportFaq.Count > 0)
                {
                    foreach (var faq in reportFaq)
                    {
                        db.tblreportfaqs.Remove(faq);
                    }
                    db.SaveChanges();
                }
                if (report.FAQ != null && report.FAQ.Count > 0)
                {
                    foreach (var faq in report.FAQ)
                    {
                        db.tblreportfaqs.Add(new tblreportfaq
                        {
                            ReportID = report.ReportId,
                            Question = faq.Question,
                            Answer = faq.Answer,
                            RowNumber = faq.RowIndex
                        });
                    }
                    db.SaveChanges();
                }

                #endregion

                #region Update Lucene Index
                if (Convert.ToInt32(out_UpdatedRows.Value) > 0)
                {
                    _addLuceneIndex(new LuceneReport
                    {
                        ReportId = Convert.ToInt32(out_ReportId.Value),
                        ReportTitle = out_ReportTitle.Value.ToString(),
                        Category = out_Category.Value.ToString(),
                        ReportUrl = out_ReportUrl.Value.ToString()
                    });
                }
                #endregion

                var executeSaveChanges = false;

                #region RemoveOld Attributes
                var deleteAttributes = db.relreportattributes.Where(x => x.ReportID == report.ReportId).ToList();
                if (deleteAttributes != null && deleteAttributes.Count > 0)
                {
                    for (int l = 0; l < deleteAttributes.Count(); l++)
                    {
                        db.relreportattributes.Remove(deleteAttributes[l]);
                    }
                    executeSaveChanges = true;
                }
                #endregion

                #region Add New Attributes
                if (report.ReportAttribute != null && report.ReportAttribute.Length > 0)
                {
                    foreach (var rr in report.ReportAttribute)
                    {
                        db.relreportattributes.Add(new relreportattribute
                        {
                            ReportID = report.ReportId,
                            AttributeID = rr
                        });
                    }
                    executeSaveChanges = true;
                }
                #endregion

                if (executeSaveChanges)
                    db.SaveChanges();

                #region Notify User
                //if report successfully updated and status changed to Published i.e. Upcoming = false
                if (rp.IsUpcoming == true && report.IsUpcoming == false)
                {
                    NotifyMeRepository.NotifyUser(rp.ReportId, rp.ReportTitle, rp.ReportUrl);
                }
                #endregion
            });

            return res;

        }

        public static IPagedList<ReportRepository> Search(string text, int? page, bool? showPublished, bool? showUpcoming)
        {
            bool isUpcoming = showUpcoming ?? false;
            bool isPublished = showPublished ?? false;
            IQueryable<ReportAndReseller> res = default;
            IPagedList<ReportRepository> result = default;
            DatabaseContext.PerformAction(db =>
            {
                res = from r in db.tblreportinformations
                      join rs in db.tblreseller_reportinformation on r.ReportID equals rs.ReportID
                      into resellerJoin
                      from rs in resellerJoin.DefaultIfEmpty()
                      where r.IsDeleted == false && r.ReportTitle.Contains(text)
                      orderby r.ReportID descending
                      select new ReportAndReseller { Report = r, Reseller = rs };

                if (isUpcoming && !isPublished)
                {
                    res = res.Where(r => r.Report.IsUpComing == true);
                }
                else if (!isUpcoming && isPublished)
                {
                    res = res.Where(r => r.Report.IsUpComing == false);
                }

                result = res.Select(r => new ReportRepository
                {
                    ReportId = r.Report.ReportID,
                    ReportTitle = r.Report.ReportTitle,
                    ReportUrl = r.Report.ReportUrl,
                    IsUpcoming = (bool)r.Report.IsUpComing,
                    ResellerMainPageDescription = r.Reseller != null ? r.Reseller.MainPageDescription : null,
                    IsResellerDescriptionAdded = r.Reseller != null && r.Reseller.MainPageDescription != null
                }).ToPagedList(page ?? 1, 10);
            });
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="reportStatus">1 - Upcoming, 2 - Published, 3 - Both</param>
        /// <returns></returns>
        public static IList<ReportRepository> GetReportsByDate(DateTime fromDate, DateTime toDate, int reportStatus)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var reportQuery = (from x in db.tblreportinformations
                                   join rel in db.relreportcategories on x.ReportID equals rel.ReportId
                                   join c in db.tblcategories on rel.CategoryId equals c.pkCategoryID
                                   where x.PublishDate >= fromDate && x.PublishDate <= toDate
                                   select new ReportRepository
                                   {
                                       ReportId = x.ReportID,
                                       ReportTitle = x.ReportTitle,
                                       MainPageDescription = x.MainPageDescription,
                                       PublishedDate = x.PublishDate,
                                       ReportUrl = x.ReportUrl,
                                       SingleUser = (decimal)x.PriceSingleUser,
                                       MultiUser = x.PriceEnterpriseLicense,
                                       Corporate = x.PriceCUL,
                                       TableOfContent = x.TableofContents,
                                       CategoryBreadcrumb = c.CategoryName,
                                       NumberOfPages = x.NumberOfPage,
                                       IsUpcoming = (bool)x.IsUpComing
                                   });

                reportQuery = reportStatus == 1 ? reportQuery.Where(x => x.IsUpcoming) : reportStatus == 2 ? reportQuery.Where(x => !x.IsUpcoming) : reportQuery;

                return reportQuery.ToList();
            }
        }

        public static int Delete(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var r = db.tblreportinformations.Where(x => x.ReportID == id).SingleOrDefault();
                r.IsDeleted = true;
                r.DeletedBy = ((Security.CustomPrincipal)HttpContext.Current.User).UserId;
                r.DeletedDate = DateTime.Now;
                int res = db.SaveChanges();
                if (res > 0)
                {
                    LuceneSearch.DeleteLuceneIndex(new LuceneReport { ReportId = id });
                }
                return res;
            }
        }

        //this is method for advance search
        static void _addLuceneIndex(LuceneReport luceneReport)
        {
            LuceneSearch.AddUpdateLuceneIndex(luceneReport);
        }

        public static void WriteAllReportsInLucene()
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var reports = (from r in db.tblreportinformations
                               where r.IsDeleted == false
                               orderby r.ReportID
                               select new LuceneReport
                               {
                                   ReportId = r.ReportID,
                                   ReportTitle = r.ReportTitle,
                                   ReportUrl = r.ReportUrl,
                                   Category = r.CategoryBreadCrumb
                               }).ToList();
                int cid = 0;
                string[] x;
                for (int i = 0; i < reports.Count(); i++)
                {
                    x = reports[i].Category.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    cid = Convert.ToInt32(x[x.Length - 1]);
                    reports[i].Category = (from c in db.tblcategories where c.pkCategoryID == cid select c.CategoryName).FirstOrDefault();
                }
                LuceneSearch.AddUpdateLuceneIndex(reports);
            }

        }

        public static int GetReportCount(string type, DateTime? fromDate, DateTime? toDate)
        {
            var count = 0;
            using (ZionDbEntities db = new ZionDbEntities())
            {
                switch (type)
                {
                    case "published":
                        count = db.tblreportinformations.Count(x => x.IsUpComing == false && x.IsActive == true && x.IsDeleted == false && x.CreatedDate >= fromDate && x.CreatedDate < toDate);
                        break;
                    case "upcoming":
                        count = db.tblreportinformations.Count(x => x.IsUpComing == true && x.IsActive == true && x.IsDeleted == false && x.CreatedDate >= fromDate && x.CreatedDate < toDate);
                        break;
                    default:
                        count = 0;
                        break;
                }
            }
            return count;
        }

        public static System.Collections.Generic.List<ArchiveReportYear> GetArchiveReports()
        {
            List<ArchiveReportYear> archiveReportYearList = default;
            DatabaseContext.PerformAction(db =>
            {
                //List<ArchiveRepository> ListYearMonth = db.Database.SqlQuery<ArchiveRepository>("SELECT MONTHNAME(r.PublishDate) as Month, MONTH(r.PublishDate) MonthNumber, YEAR(r.PublishDate) as Year, count(*) as Counts FROM tblreportinformation r WHERE r.IsDeleted = 0 GROUP BY YEAR(r.PublishDate), MONTH(r.PublishDate);").ToList();
                List<ArchiveRepository> ListYearMonth = db.Database.SqlQuery<ArchiveRepository>("SELECT MONTHNAME(r.PublishDate) as Month, MONTH(r.PublishDate) MonthNumber, YEAR(r.PublishDate) as Year, count(*) as Counts FROM tblreportinformation r WHERE r.IsDeleted = 0 GROUP BY YEAR(r.PublishDate), Month, MonthNumber, Year ;").ToList();
                archiveReportYearList = db.Database.SqlQuery<ArchiveRepository>("SELECT YEAR(r.PublishDate) as Year, count(*) as Counts FROM tblreportinformation r WHERE r.IsDeleted = 0 GROUP BY YEAR(r.PublishDate) order by Year desc")
                .ToList().Select(y => new ArchiveReportYear
                {
                    Year = y.Year,
                    YearCount = y.Counts,
                    Month = (from m in ListYearMonth
                             where m.Year == y.Year
                             orderby m.MonthNumber
                             select new ArchiveReportMonth
                             {
                                 Month = m.Month,
                                 MonthCount = m.Counts,
                                 MonthNumber = m.MonthNumber
                             }).ToList()
                }).ToList();
            });

            return archiveReportYearList;

        }

        public static int InsertOrUpdateResellerDescription(ResellerDescription resellerDescription)
        {
            var res = 0;
            DatabaseContext.PerformAction(db =>
            {
                tblreseller_reportinformation reportinformation = db.tblreseller_reportinformation.FirstOrDefault(x => x.ReportID == resellerDescription.ReportID);
                if (reportinformation == null)
                    db.tblreseller_reportinformation.Add(new tblreseller_reportinformation()
                    {
                        ReportID = resellerDescription.ReportID,
                        MainPageDescription = resellerDescription.Description
                    });
                else
                    reportinformation.MainPageDescription = resellerDescription.Description;
                res = db.SaveChanges();
            });
            return res;
        }

        public static int UpdateTableOfContent(string toc, string listOfFigures, string listOfTables, int reportID)
        {
            int res = 0;
            if (!string.IsNullOrEmpty(toc))
                DatabaseContext.PerformAction(ctx =>
                {
                    var tblreportinformation = ctx.tblreportinformations.FirstOrDefault(x => x.ReportID == reportID);
                    tblreportinformation.TableofContents = toc;
                    tblreportinformation.ListOfFigure = listOfFigures;
                    tblreportinformation.ListOfTable = listOfTables;
                    res = ctx.SaveChanges();
                });
            return res;
        }

        public static bool CreateOrUpdateReportData(int reportID, List<ReportDataColumn> reportData)
        {
            bool res = false;
            if (reportID > 0)
                DatabaseContext.PerformAction(ctx =>
                {
                    
                    var entities = ctx.tblcolumnvalues.Where(x => x.ReportID == reportID);
                    ctx.tblcolumnvalues.RemoveRange((IEnumerable<tblcolumnvalue>)entities);
                    List<tblcolumn> list = ctx.tblcolumns.ToList<tblcolumn>();
                    foreach (ReportDataColumn reportDataColumn in reportData)
                    {
                        ReportDataColumn rd = reportDataColumn;
                        if (rd.ColumnID == 0)
                        {
                            tblcolumn tblcolumn1 = list.FirstOrDefault(x => x.ColumnName.ToLower() == rd.ColumnName.ToLower().Trim());
                            if (tblcolumn1 != null)
                            {
                                tblcolumnvalue tblcolumnvalue = ctx.tblcolumnvalues.Add(new tblcolumnvalue()
                                {
                                    ColumnID = new int?(tblcolumn1.ID),
                                    ColumnValue = rd.ColumnValue,
                                    ReportID = new int?(reportID)
                                });
                                ctx.SaveChanges();
                                if (rd.Child != null && rd.Child.Count > 0)
                                {
                                    tblcolumn tblcolumn2 = list.FirstOrDefault(x => x.ColumnName.ToLower() == rd.Child[0].ColumnName.ToLower().Trim());
                                    ctx.tblcolumnvalues.Add(new tblcolumnvalue
                                    {
                                        ColumnID = new int?(tblcolumn2.ID),
                                        ColumnValue = string.Join(",", rd.Child.Select(x => x.ColumnValue.Trim()).ToList()),
                                        ReportID = reportID,
                                        ParentID = tblcolumnvalue.ID
                                    });
                                }
                            }
                        }
                    }
                    res = ctx.SaveChanges() > 0;
                });
            return res;
        }

        public static List<ReportDataColumn> GetReportData(int reportID)
        {
            List<ReportDataColumn> res = new List<ReportDataColumn>();
            DatabaseContext.PerformAction(ctx =>
            {
                res = (from cv in ctx.tblcolumnvalues
                       join col in ctx.tblcolumns on cv.ColumnID equals col.ID
                       where cv.ReportID == reportID
                       select new ReportDataColumn
                       {
                           ColumnID = (int)cv.ColumnID,
                           ColumnName = col.ColumnName,
                           ColumnValue = cv.ColumnValue
                       }).ToList();
            });
            return res;
        }

        public static ResellerDescription GetResellerDescriptionByReportID(int reportID)
        {
            ResellerDescription res = default;
            DatabaseContext.PerformAction(db =>
            {
                res = (from re in db.tblreseller_reportinformation
                       where re.ReportID == reportID
                       select new ResellerDescription
                       {
                           ReportID = re.ReportID,
                           ReportTitle = re.ReportTitle,
                           Description = re.MainPageDescription,
                       }).FirstOrDefault();
            });
            return res;
        }
        #endregion
        public class ReportFAQ
        {
            public string Question { get; set; }
            [AllowHtml]
            public string Answer { get; set; }
            public int RowIndex { get; set; }
            public int ReportID { get; set; }
        }



        public class ArchiveReportYear
        {
            public int? Year { get; set; }

            public int YearCount { get; set; }

            public System.Collections.Generic.List<ArchiveReportMonth> Month { get; set; }
        }

        public class ArchiveReportMonth
        {
            public string Month { get; set; }

            public int MonthCount { get; set; }

            public int? MonthNumber { get; set; }
        }

        public class ResellerDescription
        {
            public int ReportID { get; set; }

            [AllowHtml]
            public string Description { get; set; }

            public string ReportTitle { get; set; }
        }

        public class ReportDataColumn
        {
            public int ID { get; set; }

            public int ColumnID { get; set; }

            public string ColumnName { get; set; }

            public string ColumnValue { get; set; }

            public int ReportID { get; set; }

            public int? ParentID { get; set; }

            public int? Order { get; set; }

            public System.Collections.Generic.List<ReportDataColumn> Child { get; set; }
        }
    }

    public enum enDeliveryFormat
    {
        PDF,
        DOC,
        EXL,
        PPT,
        EMAIL,
    }

    public class ReportAndReseller
    {
        public tblreportinformation Report { get; set; }
        public tblreseller_reportinformation Reseller { get; set; }
    }
}