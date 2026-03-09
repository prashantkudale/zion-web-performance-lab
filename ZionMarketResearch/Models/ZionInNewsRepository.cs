using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace ZionMarketResearch.Models
{
    public class ZionInNewsRepository
    {
        public int ID { get; set; }
        public string LinkText { get; set; }
        public string URL { get; set; }

        public static IPagedList<ZionInNewsRepository> AllLinks(int? page)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                ObjectParameter out_Param = new ObjectParameter("p_totalRecord", 0);
                var links = (from l in db.Zion_GetZionInNewsLinks(10, ((page ?? 1) - 1) * 10, out_Param)
                             select new ZionInNewsRepository
                             {
                                 ID = l.ID,
                                 LinkText = l.LinkText,
                                 URL = l.Url
                             }).ToList();
                return new StaticPagedList<ZionInNewsRepository>(links, page ?? 1, 10, (int)out_Param.Value);

            }
        }

    }
}