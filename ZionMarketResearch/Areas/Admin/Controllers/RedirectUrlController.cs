using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionAdmin.Repository;
using ZionAdmin.Security;
using ZionMarketResearch.Log;

namespace ZionAdmin.Controllers
{
    [LogException]
    public class RedirectUrlController : Controller
    {
        //
        // GET: /RedirectUrl/
        [ZionAuthorize(Roles = "ListRedirectUrl")]
        public ActionResult Index(int? id)
        {
            return View(RedirectRepository.List(id));
        }
        [ZionAuthorize(Roles = "CreateRedirectUrl")]
        public ActionResult Create()
        {
            return View();
        }
        [ZionAuthorize(Roles = "CreateRedirectUrl")]
        [HttpPost]
        public ActionResult Create(RedirectRepository redirect)
        {
            if (ModelState.IsValid)
            {
                int x = RedirectRepository.Create(redirect);
                if (x > 0)
                    return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = "true";
            return View(redirect);
        }
        [ZionAuthorize(Roles = "EditRedirectUrl")]
        public ActionResult Edit(int id)
        {
            return View(RedirectRepository.Get(id));
        }
        [ZionAuthorize(Roles = "EditRedirectUrl")]
        [HttpPost]
        public ActionResult Edit(RedirectRepository redirect)
        {
            if (ModelState.IsValid)
            {
                int x = RedirectRepository.Update(redirect);
                if (x > 0)
                    return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = "true";
            return View(redirect);
        }
        [ZionAuthorize(Roles = "DeleteRedirectUrl")]
        public ActionResult Delete(int id)
        {
            RedirectRepository.Delete(id);
            return RedirectToAction("index");
        }
        [ZionAuthorize(Roles = "SearchRedirectUrl")]
        public ActionResult Search(string id, int? page)
        {
            ViewBag.SearchText = id;
            return View("Index", RedirectRepository.Search(id, page));
        }

    }
}
