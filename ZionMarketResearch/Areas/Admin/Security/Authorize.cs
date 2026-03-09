using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ZionAdmin.Repository;
using ZionMarketResearch.Models;

namespace ZionAdmin.Security
{
    public class ZionAuthorize : AuthorizeAttribute
    {
        protected virtual CustomPrincipal CurrentUser
        {
            get
            {
                CustomPrincipal cp = HttpContext.Current.User as CustomPrincipal;
                if (cp == null && FormsAuthentication.CookiesSupported == true)
                {
                    if (HttpContext.Current.Request.Cookies["ZionAuth"] != null)
                    {
                        HttpCookie authCookie = HttpContext.Current.Request.Cookies["ZionAuth"];
                        if (authCookie != null)
                        {
                            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                            SerializeUser serializeModel = JsonConvert.DeserializeObject<SerializeUser>(authTicket.UserData);
                            CustomPrincipal newUser = new CustomPrincipal(authTicket.Name);
                            newUser.UserId = serializeModel.UserId;
                            newUser.Roles = serializeModel.Roles;
                            HttpContext.Current.User = newUser;
                            cp = newUser;
                        }
                    }
                }
                return cp;
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                if (CurrentUser != null)
                {
                    if (!CurrentUser.IsInRole(Roles))
                    {
                        if (filterContext.ActionDescriptor.ActionName != "Index" || filterContext.ActionDescriptor.ControllerDescriptor.ControllerName != "Home")
                        {
                            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                        }
                    }
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Home", action = "Login" }));
                }
            }
        }
    }


    public class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role)
        {
            string[] actions = role.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return Roles.Any(r => actions.Contains(r));
        }

        public CustomPrincipal(string username)
        {
            this.Identity = new GenericIdentity(username);
        }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] Roles { get; set; }
        public string IPAddress { get; set; }
        public bool IsRemote { get; set; }
    }
}