using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using ZionMarketResearch.Models;

namespace ZionAdmin.Repository
{
    public class ZMRInNewsRepository
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string LinkText { get; set; }
        [Required(ErrorMessage = "Required")]
        public string URL { get; set; }

        public int NewsId { get; set; }

        public static int Insert(ZMRInNewsRepository entity)
        {
            using (var db = new ZionDbEntities())
            {
                db.tblzioninnews.Add(new tblzioninnew
                {
                    LinkText = entity.LinkText,
                    Url = entity.URL
                });
                return db.SaveChanges();
            }
        }

        public static ZMRInNewsRepository GetById(int id)
        {
            using (var db = new ZionDbEntities())
            {
                var n = db.tblzioninnews.Where(x => x.ID == id).FirstOrDefault();
                if (n != null)
                    return new ZMRInNewsRepository
                    {
                        ID = id,
                        LinkText = n.LinkText,
                        URL = n.Url
                    };
                return null;
            }
        }

        public static int Update(ZMRInNewsRepository entity)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var n = db.tblzioninnews.Where(x => x.ID == entity.ID).FirstOrDefault();
                if (n != null)
                {
                    n.LinkText = entity.LinkText;
                    n.Url = entity.URL;
                    return db.SaveChanges();
                }
            }
            return 0;
        }

        public static int Delete(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var n = db.tblzioninnews.Where(x => x.ID == id).FirstOrDefault();
                db.tblzioninnews.Remove(n);
                return db.SaveChanges();
            }
        }

        public static IPagedList<ZMRInNewsRepository> List(int? page)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                ObjectParameter out_Param = new ObjectParameter("p_totalRecord", 0);
                var links = (from l in db.Zion_GetZionInNewsLinks(100, ((page ?? 1) - 1) * 10, out_Param)
                             select new ZMRInNewsRepository
                             {
                                 ID = l.ID,
                                 LinkText = l.LinkText,
                                 URL = l.Url
                             }).ToList();
                return new StaticPagedList<ZMRInNewsRepository>(links, page ?? 1, 10, (int)out_Param.Value);

            }
        }

        public static IPagedList<ZMRInNewsRepository> Search(string searchText, int? page)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from l in db.tblzioninnews
                        where l.LinkText.Contains(searchText)
                        orderby l.ID descending
                        select new ZMRInNewsRepository
                        {
                            ID = l.ID,
                            LinkText = l.LinkText,
                            URL = l.Url
                        }).ToPagedList(page ?? 1, 10);
            }
        }
    }
}