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
    public class RoleRepository
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Role is required.")]
        [MaxLength(200, ErrorMessage = "Role name should not exceed ")]
        public string RoleName { get; set; }
        [Required(ErrorMessage = "Atleast one action should be selected.")]
        public string[] Actions { get; set; }

        public static IEnumerable<RoleRepository> List()
        {
            ZionDbEntities db = new ZionDbEntities();
            return (from r in db.tblroles
                    where r.IsActive == true
                    select new RoleRepository
                    {
                        Id = r.Roleid,
                        RoleName = r.RoleName
                    });
        }

        public static IPagedList<RoleRepository> List(int? page)
        {
            ZionDbEntities db = new ZionDbEntities();
            return (from r in db.tblroles
                    where r.IsActive == true
                    orderby r.Roleid
                    select new RoleRepository
                    {
                        Id = r.Roleid,
                        RoleName = r.RoleName
                    }).ToPagedList(page ?? 1, 10);
        }

        public static int Create(RoleRepository role)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return db.Zion_AddRole(role.RoleName, string.Join(",", role.Actions.Where(x => !string.IsNullOrEmpty(x))), ((CustomPrincipal)HttpContext.Current.User).UserId);
            }
        }

        public static RoleRepository Get(int roleid)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var role = (from r in db.tblroles
                            where r.IsActive == true && r.Roleid == roleid
                            select new RoleRepository
                            {
                                Id = r.Roleid,
                                RoleName = r.RoleName
                            }).SingleOrDefault();
                role.Actions = db.relactionroles.Where(x => x.RoleId == role.Id).ToList().Select(x => x.ActionId.ToString()).ToArray<string>();
                return role;
            }
        }

        public static int Update(RoleRepository role)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter out_RowCount = new System.Data.Entity.Core.Objects.ObjectParameter("p_RowCount", 0);
                string actions = string.Join(",", role.Actions.Where(x => !string.IsNullOrEmpty(x)));
                db.Zion_UpdateRole(role.Id, role.RoleName, actions, out_RowCount, ((CustomPrincipal)HttpContext.Current.User).UserId);
                return Convert.ToInt32(out_RowCount.Value);
            }
        }

        public static int Delete(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var role = db.tblroles.Where(x => x.Roleid == id).SingleOrDefault();
                role.IsActive = false;
                role.DeletedDate = DateTime.Now;
                role.DeletedBy = ((CustomPrincipal)HttpContext.Current.User).UserId;
                return db.SaveChanges();
            }
        }

        public static IPagedList<RoleRepository> Search(string roleName, int? page)
        {
            ZionDbEntities db = new ZionDbEntities();
            var roles = (from r in db.tblroles
                         orderby r.Roleid
                         where r.RoleName.Contains(roleName) && r.IsActive == true
                         select new RoleRepository
                         {
                             Id = r.Roleid,
                             RoleName = r.RoleName
                         }).ToPagedList(page ?? 1, 10);
            return roles;
        }
    }
}