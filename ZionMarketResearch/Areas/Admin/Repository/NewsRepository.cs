using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionAdmin.Security;
using ZionMarketResearch.Models;

namespace ZionAdmin.Repository
{
    public class NewsRepository
    {
        #region Properties
        public int NewsId { get; set; }
        
        [Required(ErrorMessage = "News is required.")]
        [AllowHtml]
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public int? ReportId { get; set; }
        public string NewsUrl { get; set; }
        public int UserId { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string NewsTitle { get; set; }
        [System.ComponentModel.DataAnnotations.MaxLength(500, ErrorMessage = "Search Page Description should not exceed 500 characters.")]
        [AllowHtml]
        public string SearchPageDescription { get; set; }

        public string MainPageImagePath { get; set; }
        public string SearchPageImagePath { get; set; }
        public string MainPageImageAltAttribute { get; set; }
        public string SearchPageImageAltAttribute { get; set; }
        [System.ComponentModel.DataAnnotations.MaxLength(200, ErrorMessage = "Meta Title length should not exceed.")]
        public string MetaTitle { get; set; }
        [System.ComponentModel.DataAnnotations.MaxLength(300, ErrorMessage = "Meta Description length should not exceed.")]
        public string MetaDescription { get; set; }
        [System.ComponentModel.DataAnnotations.MaxLength(300, ErrorMessage = "Meta Keywords length should not exceed.")]
        public string MetaKeywords { get; set; }
        public string ReportUrl { get; set; }
        public DateTime PublishedDate { get; set; }
        public string PublishedBy { get; set; }
        public bool AppendId { get; set; }
        public bool IsArticle { get; set; }
        public bool ShowNews { get; set; }
        public bool ShowArticle { get; set; }
        public bool IsActive { get; set; }
        #endregion

        #region Methods
        public static string Create(NewsRepository news, HttpFileCollectionBase files)
        {
            if (!string.IsNullOrEmpty(news.ReportUrl))
            {
                string[] reportUrl = news.ReportUrl.Split(new char[] { '\\','/' }, StringSplitOptions.RemoveEmptyEntries);
                news.ReportUrl = reportUrl[reportUrl.Length - 1];
            }

            log4net.LogManager.GetLogger("Error").Error("Report Url Split");

            Dictionary<string,string> fileName = Utility.UploadFiles(files);
            if (fileName.Keys.Count() > 0 && fileName.ContainsKey("File1"))
                news.MainPageImagePath = fileName["File1"];

            if (fileName.Keys.Count() > 0 && fileName.ContainsKey("File2"))
                news.SearchPageImagePath = fileName["File2"];
            log4net.LogManager.GetLogger("Error").Error("File Uploaded");
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var message = new System.Data.Entity.Core.Objects.ObjectParameter("p_Message", string.Empty);
                //string[] reportUrl = news.ReportUrl.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                db.Zion_AddNews(
                    news.CategoryId,
                    news.Description,
                    ((CustomPrincipal)HttpContext.Current.User).UserId,
                    news.NewsTitle,
                    news.NewsUrl,
                    news.ReportUrl,
                    news.MetaTitle,
                    news.MetaDescription,
                    news.MetaKeywords,
                    news.SearchPageDescription,
                    news.MainPageImagePath,
                    news.SearchPageImagePath,
                    news.MainPageImageAltAttribute,
                    news.SearchPageImageAltAttribute,
                    news.PublishedBy,
                    news.PublishedDate,
                    message,
                    (sbyte)(news.AppendId ? 1 : 0), (sbyte)(news.IsArticle ? 1 : 0), (sbyte)(news.IsActive? 1 : 0));
                log4net.LogManager.GetLogger("Error").Error("News Saved");
                return message.Value.ToString();
            }
        }

        public static int Update(NewsRepository news, HttpFileCollectionBase files)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                NewsRepository n = Get(news.NewsId);
                if (n.MainPageImagePath != news.MainPageImagePath && !string.IsNullOrEmpty(n.MainPageImagePath))
                    Utility.DeleteFile(n.MainPageImagePath);

                if (n.SearchPageImagePath != news.SearchPageImagePath && !string.IsNullOrEmpty(n.SearchPageImagePath))
                    Utility.DeleteFile(n.SearchPageImagePath);

                Dictionary<string, string> fileName = Utility.UploadFiles(files);
                if (fileName.ContainsKey("File1"))
                    news.MainPageImagePath = fileName["File1"];

                if (fileName.ContainsKey("File2"))
                    news.SearchPageImagePath = fileName["File2"];

                string[] reportUrl = news.ReportUrl != null ? news.ReportUrl.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries) : null;
                return db.Zion_UpdateNews(
                    news.CategoryId,
                    news.Description,
                    ((CustomPrincipal)HttpContext.Current.User).UserId,
                    news.NewsTitle,
                    news.NewsId,
                    reportUrl != null && reportUrl.Length > 0 ? reportUrl[reportUrl.Length - 1] : "",
                    news.NewsUrl,
                    news.SearchPageDescription,
                    news.MainPageImagePath,
                    news.SearchPageImagePath,
                    news.MainPageImageAltAttribute,
                    news.SearchPageImageAltAttribute,
                    news.MetaTitle,
                    news.MetaDescription,
                    news.MetaKeywords,
                    news.PublishedDate,
                    news.PublishedBy,
                    (sbyte)(news.IsArticle ? 1 : 0), (sbyte)(news.IsActive ? 1 : 0));
            }
        }

        public static int Delete(int id, int userId)
        {
            ZionDbEntities db = new ZionDbEntities();
            var news = db.tblnews.Where(x => x.NewsID == id).SingleOrDefault();
            news.IsDeleted = true;
            news.DeletedBy = ((CustomPrincipal)HttpContext.Current.User).UserId;
            //news.DeletdDate = DateTime.Now;
            return db.SaveChanges();
        }

        public static NewsRepository Get(int newsId)
        {
            ZionDbEntities db = new ZionDbEntities();
            return (from n in db.tblnews
                    join r in db.tblreportinformations on n.ReportId equals r.ReportID
                    into nx from rt in nx.DefaultIfEmpty()
                    where n.NewsID == newsId
                    select new NewsRepository
                    {
                        NewsId = n.NewsID,
                        CategoryId = n.CategoryId,
                        ReportId = n.ReportId,
                        NewsTitle = n.NewsTitle,
                        MainPageImagePath = n.MainPageImagePath,
                        Description = n.News,
                        SearchPageDescription = n.SearchPageDescription,
                        SearchPageImagePath = n.SearchPageImagePath,
                        MainPageImageAltAttribute = n.MainPageImageAltAttribute,
                        SearchPageImageAltAttribute = n.SearchPageImageAltAttribute,
                        MetaTitle = n.MetaTitle,
                        MetaDescription = n.MetaDescription,
                        MetaKeywords = n.MetaKeywords,
                        NewsUrl = n.NewsUrl,
                        PublishedBy = n.PublishedBy,
                        PublishedDate = (DateTime)n.PublishedDate,
                        ReportUrl = "http://www.zionmarketresearch.com/report/" + rt.ReportUrl,
                        IsArticle = (bool)n.IsArticle,
                        IsActive = (bool)n.IsActive
                    }).SingleOrDefault();
        }

        public static IEnumerable<NewsRepository> List()
        {
            ZionDbEntities db = new ZionDbEntities();
            return (from n in db.tblnews
                    where n.IsDeleted == false && n.IsArticle == false
                    orderby n.NewsID descending
                    select new NewsRepository
                    {
                        NewsId = n.NewsID,
                        CategoryId = n.CategoryId,
                        ReportId = n.ReportId,
                        NewsTitle = n.NewsTitle,
                        MainPageImagePath = n.MainPageImagePath,
                        Description = n.News,
                        SearchPageDescription = n.SearchPageDescription,
                        SearchPageImagePath = n.SearchPageImagePath,
                        MainPageImageAltAttribute = n.MainPageImageAltAttribute,
                        SearchPageImageAltAttribute = n.SearchPageImageAltAttribute,
                        MetaTitle = n.MetaTitle,
                        MetaDescription = n.MetaDescription,
                        MetaKeywords = n.MetaKeywords,
                        IsArticle = (bool)n.IsArticle
                    });
        }

        public static IPagedList<NewsRepository> List(bool? showNews, bool? showArticle, int? page)
        {
            showNews = showNews ?? false;
            showArticle = showArticle ?? false;
            ZionDbEntities db = new ZionDbEntities();
            return (from n in db.tblnews
                    where n.IsDeleted == false && ((bool)showNews && !(bool)showArticle ? n.IsArticle == false : !(bool)showNews && (bool)showArticle ? n.IsArticle == true : (n.IsArticle == true || n.IsArticle == false))
                    orderby n.PublishedDate descending
                    select new NewsRepository
                    {
                        NewsId = n.NewsID,//
                        //CategoryId = n.CategoryId,
                        //ReportId = n.ReportId,
                        NewsTitle = n.NewsTitle,//
                        //MainPageImagePath = n.MainPageImagePath,
                        //Description = n.News,
                        //SearchPageDescription = n.SearchPageDescription,
                        //SearchPageImagePath = n.SearchPageImagePath,
                        //MainPageImageAltAttribute = n.MainPageImageAltAttribute,
                        //SearchPageImageAltAttribute = n.SearchPageImageAltAttribute,
                        //MetaTitle = n.MetaTitle,
                        //MetaDescription = n.MetaDescription,
                        //MetaKeywords = n.MetaKeywords,
                        NewsUrl = n.NewsUrl,//
                        IsArticle = (bool)(n.IsArticle ?? false), //
                        IsActive = (bool)(n.IsActive ?? false), //
                        PublishedDate = (DateTime)n.PublishedDate//
                    }).ToPagedList(page ?? 1, 100);
        }

        public static IPagedList<NewsRepository> Search(string searchText, int? page, bool? showNews, bool? showArticle)
        {
            bool isNews = showNews ?? false;
            bool isArticle = showArticle ?? false;

            ZionDbEntities db = new ZionDbEntities();
            return (from n in db.tblnews
                    where n.NewsTitle.Contains(searchText) && n.IsDeleted == false && (isNews && !isArticle ? n.IsArticle == false : !isNews && isArticle ? n.IsArticle == true : (n.IsArticle == null || n.IsArticle == true || n.IsArticle == false))
                    orderby n.NewsID descending
                    select new NewsRepository
                    {
                        NewsId = n.NewsID,
                        CategoryId = n.CategoryId,
                        ReportId = n.ReportId,
                        NewsTitle = n.NewsTitle,
                        MainPageImagePath = n.MainPageImagePath,
                        Description = n.News,
                        SearchPageDescription = n.SearchPageDescription,
                        SearchPageImagePath = n.SearchPageImagePath,
                        MainPageImageAltAttribute = n.MainPageImageAltAttribute,
                        SearchPageImageAltAttribute = n.SearchPageImageAltAttribute,
                        MetaTitle = n.MetaTitle,
                        MetaDescription = n.MetaDescription,
                        MetaKeywords = n.MetaKeywords,
                        NewsUrl = n.NewsUrl,
                        IsArticle = (bool)(n.IsArticle ?? false),
                        IsActive = (bool)(n.IsActive ?? false),
                        PublishedDate = (DateTime)n.PublishedDate
                    }).ToPagedList(page ?? 1, 10);
        }
        #endregion

    }
}