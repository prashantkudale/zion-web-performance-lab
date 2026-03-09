using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ZionAdmin.Models;
using ZionAdmin.Security;
using ZionMarketResearch.Models;

namespace ZionAdmin.Repository
{
    public class RedirectRepository
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "From Url is required.")]
        [MaxLength(500, ErrorMessage = "From Url should not exceed length 500 characters.")]
        public string FromUrl { get; set; }
        [Required(ErrorMessage = "To Url is required.")]
        [MaxLength(ErrorMessage = "To Url should not exceed length 500 characters.")]
        public string ToUrl { get; set; }
        public int UserId { get; set; }


        public static int Create(RedirectRepository redirect)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return db.Zion_AddRedirectUrl(redirect.FromUrl, redirect.ToUrl, ((CustomPrincipal)HttpContext.Current.User).UserId);
            }
        }

        public static int Update(RedirectRepository redirect)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return db.Zion_UpdateRedrectUrl(redirect.Id, redirect.FromUrl, redirect.ToUrl, ((CustomPrincipal)HttpContext.Current.User).UserId);
            }
        }

        public static IPagedList<RedirectRepository> List(int? page)
        {
            ZionDbEntities db = new ZionDbEntities();
            return (from r in db.tblredirections
                    where r.IsActive == true
                    orderby r.Id
                    select new RedirectRepository
                    {
                        Id = r.Id,
                        FromUrl = r.OldUrl,
                        ToUrl = r.NewUrl
                    }).ToPagedList(page ?? 1, 100);
        }

        public static int Delete(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var redirect = db.tblredirections.Where(x => x.Id == id).SingleOrDefault();
                //redirect.IsActive = false;
                //redirect.DeletedBy = ((CustomPrincipal)HttpContext.Current.User).UserId;
                //redirect.DeletedDate = DateTime.Now;
                db.tblredirections.Remove(redirect);
                return db.SaveChanges();
            }
        }

        public static IPagedList<RedirectRepository> Search(string search, int? page)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from r in db.tblredirections
                        where r.OldUrl.Contains(search) || r.NewUrl.Contains(search) && r.IsActive == true
                        orderby r.Id
                        select new RedirectRepository
                        {
                            Id = r.Id,
                            FromUrl = r.OldUrl,
                            ToUrl = r.NewUrl
                        }).ToPagedList(page ?? 1, 10);
            }
        }

        public static RedirectRepository Get(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from r in db.tblredirections
                        where r.Id == id
                        select new RedirectRepository
                        {
                            Id = r.Id,
                            FromUrl = r.OldUrl,
                            ToUrl = r.NewUrl
                        }).FirstOrDefault();
            }
        }
    }
}
