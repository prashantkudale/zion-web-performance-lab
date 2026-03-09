using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionAdmin.Repository;
using ZionMarketResearch.Log;

namespace ZionAdmin.Controllers
{
    [LogException]
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserRepository user)
        {
            return View();
        }
    }
}
