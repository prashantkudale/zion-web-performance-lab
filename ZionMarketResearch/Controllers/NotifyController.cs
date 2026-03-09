using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Controllers
{
    public class NotifyController : Controller
    {

        [HttpPost]
        public ActionResult Notify(NotifyMeRepository notifyMeRepo)
        {
            //cache object should be null OR cache object contains diffrent value than unique id
            if (System.Web.HttpContext.Current.Cache["UniqueId"] == null || string.IsNullOrEmpty(System.Web.HttpContext.Current.Cache["UniqueId"].ToString()))
            {
                System.Web.HttpContext.Current.Cache.Add("UniqueId", "should be null", null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
                bool n = NotifyMeRepository.NotifyMe(notifyMeRepo);

                

                return Json(new { isSuccess = n, message = "Success" });
            }
            return Json(new { isSuccess = false, message = "Multiple request are not allowed." });
        }

        public ActionResult ShowNotify()
        {
            string guid = Guid.NewGuid().ToString();
            if (System.Web.HttpContext.Current.Cache["UniqueId"] != null && string.IsNullOrEmpty(System.Web.HttpContext.Current.Cache["UniqueId"].ToString()))
                System.Web.HttpContext.Current.Cache["UniqueId"] = string.Empty;
            return View("~/Views/Report/_NotifyModal.cshtml", new NotifyMeRepository());
        }

        public ActionResult Unsubscribe(int? uniqueid)
        {
            if (uniqueid == null || !NotifyMeRepository.Unsubscribe((int)uniqueid))
                return HttpNotFound();
            return View();
        }
    }
}
