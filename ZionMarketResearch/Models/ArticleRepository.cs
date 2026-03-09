using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZionMarketResearch.Models
{
    public class ArticleRepository
    {
        public static IPagedList<Article> GetArticles(int? page)
        {
            ZionDbEntities db = new ZionDbEntities();
            return (from a in db.tblnews
                    where a.IsArticle == true && a.IsDeleted == false
                    orderby a.NewsID descending
                    select new Article
                    {
                        Id = a.NewsID,
                        Url = a.NewsUrl,
                        PublishedBy = a.PublishedBy,
                        PublishedDate = a.PublishedDate,
                        Title = a.NewsTitle,
                        MetaTitle = a.MetaTitle,
                        MetaDescription = a.MetaDescription,
                        MetaKeywords = a.MetaKeywords,
                        Description = a.News
                    }).ToPagedList(page ?? 1, 10);
        }

        public static Article GetArticleByUrl(string url)
        {
            ZionDbEntities db = new ZionDbEntities();
            return (from a in db.tblnews
                    where a.IsArticle == true && a.IsDeleted == false && a.NewsUrl == url
                    select new Article
                    {
                        Id = a.NewsID,
                        Url = a.NewsUrl,
                        PublishedBy = a.PublishedBy,
                        PublishedDate = a.PublishedDate,
                        Title = a.NewsTitle,
                        MetaTitle = a.MetaTitle,
                        MetaDescription = a.MetaDescription,
                        MetaKeywords = a.MetaKeywords,
                        Description = a.News
                    }).FirstOrDefault();
        }

        public static List<Article> GetAllArticles()
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from a in db.tblnews
                        where a.IsArticle == true && a.IsDeleted == false
                        orderby a.NewsID descending
                        select new Article
                        {
                            Url = a.NewsUrl,
                            Description = a.News,
                            PublishedDate = a.PublishedDate,
                            Title = a.NewsTitle
                        }).ToList();
            }
        }

        public static NewsDeatils MapNewsArticle(Article article)
        {
            NewsDeatils n = new NewsDeatils();
            n.NewsId = article.Id;
            n.NewsTitle = article.Title;
            n.Description = article.Description;
            n.PublishedDate = article.PublishedDate;
            n.PublishedBy = article.PublishedBy;
            n.MetaDescription = article.MetaDescription;
            n.MetaTile = article.MetaTitle;
            n.MetaKeywords = article.MetaKeywords;
            n.NewsUrl = article.Url;
            n.IsArticle = true;
            return n;
        }
    }
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [System.ComponentModel.DataAnnotations.UIHint("SearchResultDescription")]
        public string Description { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string PublishedBy { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string Url { get; set; }
    }
}