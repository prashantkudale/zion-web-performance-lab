using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZionMarketResearch.Models;

namespace ZionAdmin.Repository
{
    public class ActionRepository
    {
        public int Id { get; set; }
        public string Action { get; set; }

        public static IEnumerable<ActionRepository> List()
        {
            ZionDbEntities db = new ZionDbEntities();
            return from a in db.tblactions
                   where a.IsActive == true
                   select new ActionRepository
                   {
                       Id = a.Id,
                       Action = a.Action
                   };
        }

        public static IEnumerable<ActionRepository> List(int roleId)
        {
            ZionDbEntities db = new ZionDbEntities();
            return from a in db.tblactions
                   join ar in db.relactionroles on a.Id equals ar.ActionId
                   where ar.RoleId == roleId
                   select new ActionRepository
                   {
                       Action = a.Action,
                       Id = a.Id
                   };
        }

        public static ActionRepository Get(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from a in db.tblactions
                        where a.Id == id
                        select new ActionRepository
                        {
                            Action = a.Action,
                            Id = a.Id
                        }).SingleOrDefault();
            }
        }
    }
}