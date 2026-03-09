using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionAdmin.Models;
using ZionAdmin.Security;
using ZionMarketResearch.Models;

namespace ZionAdmin.Repository
{
    public class CategoryRepository
    {
        #region properties
        [Key]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category is required.")]
        [System.ComponentModel.DataAnnotations.MaxLength(300, ErrorMessage = "Category should not exceed 300 characters.")]
        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
        public string MainPageImagePath { get; set; }
        public string MainPageImageAltAttribute { get; set; }
        public string HomePageImagePath { get; set; }
        public string HomePageImageAltAttribute { get; set; }
        [AllowHtml]
        public string TopDescription { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public string PageHeading { get; set; }
        [MaxLength(200, ErrorMessage = "Tile should not exceed 200 characters.")]
        public string Title { get; set; }
        public string CategoryUrl { get; set; }
        [AllowHtml]
        [MaxLength(500, ErrorMessage = "Home Page Description should not exceed 500.")]
        public string HomePageDescription { get; set; }
        public bool AppendId { get; set; }
        #endregion

        #region methods
        public static string Create(CategoryRepository category, HttpFileCollectionBase files = null)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                Dictionary<string, string> fileNames = Utility.UploadFiles(files);
                if (fileNames.ContainsKey("file1"))
                    category.MainPageImagePath = fileNames["file1"];

                if (fileNames.ContainsKey("file2"))
                    category.HomePageImagePath = fileNames["file2"];

                var message = new ObjectParameter("p_Message", string.Empty);
                db.Zion_AddCategory(category.CategoryName,
                    category.CategoryUrl,
                    category.ParentCategoryId,
                    category.PageHeading,
                    category.Title,
                    category.TopDescription,
                    category.MainPageImagePath,
                    category.MainPageImageAltAttribute,
                    category.HomePageDescription,
                    category.HomePageImagePath,
                    category.HomePageImageAltAttribute,
                    category.MetaKeyword,
                    category.MetaDescription, message,
                    (sbyte)(category.AppendId ? 1 : 0), 
                    ((CustomPrincipal)HttpContext.Current.User).UserId);
                return message.Value.ToString();
            }
        }

        public static int Update(CategoryRepository category, HttpFileCollectionBase files = null)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var _oldData = Get(category.CategoryId);

                Dictionary<string, string> fileNames = Utility.UploadFiles(files);

                if (_oldData.MainPageImagePath != category.MainPageImagePath && !string.IsNullOrEmpty(_oldData.MainPageImagePath))
                    Utility.DeleteFile(_oldData.MainPageImagePath);

                if (_oldData.HomePageImagePath != category.HomePageImagePath && !string.IsNullOrEmpty(_oldData.HomePageImagePath))
                    Utility.DeleteFile(_oldData.HomePageImagePath);

                if (fileNames.ContainsKey("file1"))
                    category.MainPageImagePath = fileNames["file1"];


                if (fileNames.ContainsKey("file2"))
                    category.HomePageImagePath = fileNames["file2"];

                //category.MainPageImagePath = fPath.ContainsKey("MainPageImagePath") && fPath["MainPageImagePath"] != null && fPath["MainPageImagePath"] != string.Empty ? fPath["MainPageImagePath"] : category.MainPageImagePath;
                //category.HomePageImagePath = fPath.ContainsKey("HomePageImagePath") && fPath["HomePageImagePath"] != null && fPath["HomePageImagePath"] != string.Empty ? fPath["HomePageImagePath"] : category.HomePageImagePath;

                return db.Zion_UpdateCategory(
                    category.CategoryId,
                    category.CategoryUrl,
                    category.CategoryName,
                    category.ParentCategoryId,
                    category.Title,
                    category.TopDescription,
                    category.PageHeading,
                    category.MainPageImagePath,
                    category.MainPageImageAltAttribute,
                    category.HomePageDescription,
                    category.HomePageImagePath,
                    category.HomePageImageAltAttribute,
                    category.MetaDescription,
                    category.MetaKeyword,
                    ((CustomPrincipal)HttpContext.Current.User).UserId);
            }
        }

        public static int Delete(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var cat = db.tblcategories.Where(x => x.pkCategoryID == id).FirstOrDefault();
                cat.IsDeleted = true;
                cat.DeletedBy = ((CustomPrincipal)HttpContext.Current.User).UserId;
                cat.DeletedDate = DateTime.Now;
                return db.SaveChanges();
            }
        }

        public static IPagedList<CategoryRepository> List(int? page)
        {
            ZionDbEntities db = new ZionDbEntities();
            return (from c in db.tblcategories
                    orderby c.pkCategoryID
                    where c.IsDeleted == false
                    select new CategoryRepository
                    {
                        CategoryId = c.pkCategoryID,
                        CategoryName = c.CategoryName,
                        CategoryUrl = c.CategoryUrl,
                        ParentCategoryId = c.ParentCategoryId
                    }).ToPagedList(page ?? 1, 10);
        }

        public static IEnumerable<CategoryRepository> List()
        {
            ZionDbEntities db = new ZionDbEntities();
            return (from c in db.tblcategories
                    orderby c.pkCategoryID
                    where c.IsDeleted == false
                    select new CategoryRepository
                    {
                        CategoryId = c.pkCategoryID,
                        CategoryName = c.CategoryName,
                        CategoryUrl = c.CategoryUrl,
                        ParentCategoryId = c.ParentCategoryId
                    });
        }

        public static CategoryRepository Get(int id)
        {
            ZionDbEntities db = new ZionDbEntities();
            return (from c in db.tblcategories
                    where c.pkCategoryID == id
                    select new CategoryRepository
                    {
                        CategoryName = c.CategoryName,
                        CategoryId = c.pkCategoryID,
                        ParentCategoryId = c.ParentCategoryId,
                        CategoryUrl = c.CategoryUrl,
                        TopDescription = c.TopDescription,
                        MainPageImagePath = c.MainPageImagePath,
                        MainPageImageAltAttribute = c.MainPageImageAltAttribute,
                        HomePageImagePath = c.HomePageImagePath,
                        HomePageDescription = c.HomePageDescription,
                        HomePageImageAltAttribute = c.HomePageImageAltAttribute,
                        MetaDescription = c.MetaDescription,
                        MetaKeyword = c.Keywords,
                        PageHeading = c.PageHeading,
                        Title = c.Title
                    }).FirstOrDefault();
        }

        public static CategoryRepository Get(string categoryName)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from c in db.tblcategories
                        where c.CategoryName == categoryName
                        select new CategoryRepository
                        {
                            CategoryName = c.CategoryName,
                            CategoryId = c.pkCategoryID,
                            ParentCategoryId = c.ParentCategoryId,
                            CategoryUrl = c.CategoryUrl,
                            TopDescription = c.TopDescription,
                            MainPageImagePath = c.MainPageImagePath,
                            MainPageImageAltAttribute = c.MainPageImageAltAttribute,
                            HomePageImagePath = c.HomePageImagePath,
                            HomePageDescription = c.HomePageDescription,
                            HomePageImageAltAttribute = c.HomePageImageAltAttribute,
                            MetaDescription = c.MetaDescription,
                            MetaKeyword = c.Keywords,
                            PageHeading = c.PageHeading,
                            Title = c.Title
                        }).FirstOrDefault();
            }
        }

        private static Dictionary<string, string> UploadFiles(HttpFileCollectionBase files, Dictionary<string, string> img = null)
        {
            Dictionary<string, string> fPath = new Dictionary<string, string>();
            if (files != null && files.Count > 0)
            {
                string mainPageImage = Guid.NewGuid().ToString();
                string homePageImage = Guid.NewGuid().ToString();
                if (files["file1"] != null && files["file1"].ContentLength > 0)
                {
                    files["file1"].SaveAs(HttpContext.Current.Server.MapPath("/UploadFiles/" + mainPageImage + "." + files["file1"].FileName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[1]));
                    fPath.Add("MainPageImagePath", mainPageImage + "." + files["file1"].FileName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[1]);


                }

                if (files["file2"] != null && files["file2"].ContentLength > 0)
                {
                    files["file2"].SaveAs(HttpContext.Current.Server.MapPath("/UploadFiles/" + homePageImage + "." + files["file2"].FileName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[1]));
                    fPath.Add("HomePageImagePath", homePageImage + "." + files["file2"].FileName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[1]);

                }
                if (img != null)
                {
                    if (img.ContainsKey("MainPageImagePath") && img["MainPageImagePath"] != null && img["MainPageImagePath"] != string.Empty)
                        DeleteFile(img["MainPageImagePath"]);

                    if (img.ContainsKey("HomePageImagePath") && img["HomePageImagePath"] != null && img["HomePageImagePath"] != string.Empty)
                        DeleteFile(img["HomePageImagePath"]);
                }
            }
            return fPath;
        }
        private static void DeleteFile(string path)
        {
            System.IO.File.Delete(HttpContext.Current.Server.MapPath("/UploadFiles/" + path));
        }

        public static IPagedList<CategoryRepository> Search(string searchText, int? page)
        {
            ZionDbEntities db = new ZionDbEntities();
            return (from c in db.tblcategories
                    orderby c.pkCategoryID
                    where c.CategoryName.Contains(searchText) && c.IsDeleted == false
                    select new CategoryRepository
                    {
                        CategoryId = c.pkCategoryID,
                        CategoryName = c.CategoryName,
                        CategoryUrl = c.CategoryUrl,
                        ParentCategoryId = c.ParentCategoryId
                    }).ToPagedList(page ?? 1, 10);
        }

        public static IEnumerable<CategoryRepository> GetChildCategory(int parentCategory)
        {
            ZionDbEntities db = new ZionDbEntities();
            return from c in db.tblcategories
                   where c.IsDeleted == false && c.ParentCategoryId == parentCategory
                   select new CategoryRepository
                   {
                       CategoryId = c.pkCategoryID,
                       CategoryName = c.CategoryName
                   };
        }

        public static List<SelectListItem> GetParentCategories()
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var pcategory = (from c in db.tblcategories
                                 where (c.ParentCategoryId == null || c.ParentCategoryId == 0) && c.IsDeleted == false
                                 select c).ToList();
                return (from c in pcategory
                        select new SelectListItem
                        {
                            Text = c.CategoryName,
                            Value = c.pkCategoryID.ToString()
                        }).ToList();
            }
        }

        public static List<CategoryRepository> GetParentCategoryJson()
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from c in db.tblcategories
                        where (c.ParentCategoryId == null || c.ParentCategoryId == 0) && c.IsDeleted == false
                        select new CategoryRepository
                        {
                            CategoryId = c.pkCategoryID,
                            CategoryName = c.CategoryName
                        }).ToList();
            }
        }

        public static string GetCategoryJsonString(string categoryBreadcrumb)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var splitString = categoryBreadcrumb.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)).ToList();

                var res = (from c in db.tblcategories
                           where splitString.Contains(c.pkCategoryID)
                           orderby c.pkCategoryID
                           select new
                           {
                               Text = c.CategoryName,
                               Value = c.pkCategoryID
                           }).ToList();
                System.Web.Script.Serialization.JavaScriptSerializer serialize = new System.Web.Script.Serialization.JavaScriptSerializer();
                return serialize.Serialize(res);
            }
        }
        #endregion
    }
}