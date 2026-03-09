using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;
using ZionAdmin.Models;
using ZionAdmin.Security;
using ZionMarketResearch.Areas.Admin.Models;
using ZionMarketResearch.Models;

namespace ZionAdmin.Repository
{
    public class UserRepository
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "User Name is required")]
        [MaxLength(100, ErrorMessage = "User name should not exceed 100 characters.")]
        public string UserName { get; set; }
        [MaxLength(100, ErrorMessage = "First Name should not exceed 100 characters.")]
        public string FirstName { get; set; }
        [MaxLength(100, ErrorMessage = "Last Name should not exceed 100 characters.")]
        public string LastName { get; set; }
        public string Password { get; set; }
        public DateTime? LastLogin { get; set; }
        public string IPAddress { get; set; }
        public bool IsRemote { get; set; }
        public bool IsAuthenticated { get; set; }
        [MaxLength(100, ErrorMessage = "Email Id should not exceed 100 characters.")]
        [Required(ErrorMessage = "Email Id is required.")]
        public string EmailId { get; set; }
        [Required(ErrorMessage = "Atleast one role should be selected.")]
        public string[] UserRoles { get; set; }
        [Required(ErrorMessage = "Registered IP is required.")]
        public string RegisteredIP { get; set; }
        public string TempPermitIP { get; set; }
        public static UserRepository Get(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var user = (from u in db.tblusers
                            where u.Id == id
                            select new UserRepository
                            {
                                Id = u.Id,
                                UserName = u.UserName,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                EmailId = u.EmailId,
                                IsRemote = (bool)u.IsRemote,
                                RegisteredIP = u.RegisterIP
                            }).SingleOrDefault();

                user.UserRoles = db.tbluserroles.Where(x => x.UserId == id).ToList().Select(x => x.RoleId.ToString()).ToArray<string>();
                return user;
            }
        }

        public static int Create(UserRepository user)
        {
            var res = 0;
            DatabaseContext.PerformAction(db =>
            {
                //TODO: Implement send mail functionality, for sending user id and password to user
                user.Password = Utility.GenerateUID();
                System.Data.Entity.Core.Objects.ObjectParameter out_RowCount = new System.Data.Entity.Core.Objects.ObjectParameter("p_RowCount", 0);
                res = (int)db.Zion_AddUser(user.UserName.ToLower(), user.FirstName, user.LastName, user.Password, user.EmailId, string.Join(",", user.UserRoles), out_RowCount, ((CustomPrincipal)HttpContext.Current.User).UserId, user.RegisteredIP, (sbyte)(user.IsRemote ? 1 : 0)).FirstOrDefault();
                //return 0;
            });
            return res;
        }

        public static int Update(UserRepository user)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter out_RowCount = new System.Data.Entity.Core.Objects.ObjectParameter("p_RowCount", 0);
                db.Zion_UpdateUser(user.Id, user.UserName.ToLower(), string.Join(",", user.UserRoles), out_RowCount, ((CustomPrincipal)HttpContext.Current.User).UserId, string.Join(",", user.UserRoles), user.RegisteredIP, (sbyte)(user.IsRemote ? 1 : 0));
                return Convert.ToInt32(out_RowCount.Value);
            }
        }

        public static IPagedList<UserRepository> List(int? page)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from u in db.tblusers
                        where u.IsActive == true
                        orderby u.Id
                        select new UserRepository
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            LastLogin = u.LastLogin,
                            UserName = u.UserName
                        }).ToPagedList(page ?? 1, 10);
            }
        }

        public static int Delete(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var user = db.tblusers.SingleOrDefault(x => x.Id == id);
                user.IsActive = false;
                user.DeletedBy = ((CustomPrincipal)HttpContext.Current.User).UserId;
                user.DeletedDate = DateTime.Now;
                return db.SaveChanges();
            }
        }

        public static UserRepository IsValid(string username, string password)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from u in db.tblusers
                        where u.UserName == username && u.Password == password
                        select new UserRepository
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName
                        }).FirstOrDefault();
            }
        }

        public static bool Login(string userName, string password)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var user = db.tblusers.Where(x => x.UserName == userName && x.Password == password).SingleOrDefault();
                if (user == null)
                    return false;


                if (!(bool)user.IsRemote && user.RegisterIP != HttpContext.Current.Request.UserHostAddress &&
                    user.TempPermitIP != HttpContext.Current.Request.UserHostAddress && !HttpContext.Current.Request.IsLocal)
                {
                    //TODO: implement admin mail functionality
                    Utility.SendMail("ms202301182200@gmail.com", "Trying Login on ZMR", string.Format("Dear Admin,<br/>User {0} (User Name - {1}) trying to login from IP Address - {2}", user.FirstName, user.UserName, HttpContext.Current.Request.UserHostAddress));
                    return false;
                }
                //If user is login using temp IP address then check that admin approved for temp IP address
                //TODO: this logic should be changed. Check KeyCreatedDate and KeyApprovedDate for more accuracy
                //CUrrent condition may lead to unexpected behaviour.
                if (user.TempPermitIP == HttpContext.Current.Request.UserHostAddress && !HttpContext.Current.Request.IsLocal)
                {
                    var key = db.tblkeys.Where(x => x.UserId == user.Id && x.TempIPAddress == HttpContext.Current.Request.UserHostAddress && x.IsUsed == true).FirstOrDefault();
                    if (key == null)
                        return false;
                }
                var actions = (from ur in db.tbluserroles join ar in db.relactionroles on ur.RoleId equals ar.RoleId join a in db.tblactions on ar.ActionId equals a.Id where ur.UserId == user.Id select a.Action).ToArray<string>();
                string userData = JsonConvert.SerializeObject(new SerializeUser { UserId = user.Id, UserName = user.UserName, FirstName = user.FirstName, LastName = user.LastName, Roles = actions });
                FormsAuthenticationTicket authticket = new FormsAuthenticationTicket(1, user.UserName, DateTime.Now, DateTime.Now.AddDays(15), false, userData);

                string encTicket = FormsAuthentication.Encrypt(authticket);
                HttpCookie faCookie = new HttpCookie("ZionAuth", encTicket);
                //It temp login. it will work for one day only.
                faCookie.Expires = user.TempPermitIP == HttpContext.Current.Request.UserHostAddress ? DateTime.Now.AddDays(1) : DateTime.Now.AddDays(15);
                HttpContext.Current.Response.Cookies.Add(faCookie);
                FormsAuthentication.SetAuthCookie(user.UserName, false);
                user.LastLogin = DateTime.Now;
                user.IPAddress = HttpContext.Current.Request.UserHostAddress;
                db.SaveChanges();
                return user != null;
            }
        }

        public static void Logout()
        {
            FormsAuthentication.SignOut();
            HttpContext.Current.Response.Cookies["ZionAuth"].Expires = DateTime.Now.AddDays(-1);
        }

        public static bool AskPermission(string username)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var user = db.tblusers.Where(u => u.UserName == username).FirstOrDefault();
                if (user != null)
                {
                    string strkey = Utility.GenerateUID();
                    tblkey key = new tblkey();
                    key.Key = strkey;
                    key.GeneratedOn = DateTime.Now;
                    key.UserId = user.Id;
                    key.IsActive = true;
                    key.TempIPAddress = HttpContext.Current.Request.UserHostAddress;
                    db.tblkeys.Add(key);
                    db.SaveChanges();
                    //Utility.SendMail("ms202301182200@gmail.com,sandip@marketresearchstore.com", "ZMR Permission Request", string.Format("Dear Admin,<br/> User {0} {1} has requested a key.<br/>Click on {2} for approve the request.", user.FirstName, user.LastName, System.Configuration.ConfigurationManager.AppSettings["WebsiteUrl"] + "admin/home/approve/" + strkey));
                    SendMail.Send("danny@marketresearchstore.com,ms202301182200@gmail.com", "ZMR Permission Request", string.Format("Dear Admin,<br/> User {0} {1} has requested a key.<br/>Click on {2} for approve the request.", user.FirstName, user.LastName, System.Configuration.ConfigurationManager.AppSettings["WebsiteUrl"] + "admin/home/approve/" + strkey));
                    return true;
                }
            }
            return false;
        }

        public static bool Approve(string strKey)
        {
            string v = HttpContext.Current.Request.Cookies["ApprovedIP"] != null ? HttpContext.Current.Request.Cookies["ApprovedIP"].Value : string.Empty;
            if (!string.IsNullOrEmpty(v) && DateTime.Now < Convert.ToDateTime(v.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[1]).AddMinutes(30))
            {
                return false;
            }
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var key = db.tblkeys.Where(x => x.Key == strKey && x.IsUsed == false && x.IsActive == true).FirstOrDefault();
                if (key == null)
                    return false;
                var user = db.tblusers.Where(u => u.Id == key.UserId && u.IsActive == true).FirstOrDefault();
                if (key != null && user != null)
                {
                    HttpCookie cookie = new HttpCookie("ApprovedIP", key.TempIPAddress + "|" + DateTime.Now);//just for not bothering to system
                    key.IsUsed = true;
                    user.TempPermitIP = key.TempIPAddress;
                    db.SaveChanges();
                    Utility.SendMail(user.EmailId, "Request Approved", string.Format("Dear {0},<br/>Your request has been approved. Now you can login with your userid and password.", user.FirstName));
                    return true;
                }
                return false;
            }
        }

        public static IPagedList<UserRepository> Search(string searchText, int? page)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from u in db.tblusers
                        where u.IsActive == true && u.UserName.Contains(searchText)
                        orderby u.Id
                        select new UserRepository
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            LastLogin = u.LastLogin,
                            UserName = u.UserName
                        }).ToPagedList(page ?? 1, 10);
            }
        }
    }

    public class SerializeUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] Roles { get; set; }
    }
}