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
    public class RoleController : Controller
    {
        //
        // GET: /Role/
        [ZionAuthorize(Roles = "ListRole")]
        public ActionResult Index(int? id)
        {
            return View(RoleRepository.List(id));
        }
        [ZionAuthorize(Roles = "CreateRole")]
        public ActionResult Create()
        {
            return View();
        }
        [ZionAuthorize(Roles = "CreateRole")]
        [HttpPost]
        public ActionResult Create(RoleRepository role)
        {
            if (ModelState.IsValid)
            {
                RoleRepository.Create(role);
                return RedirectToAction("index");
            }
            ViewBag.ErrorMessage = "true";
            return View(role);
        }
        [ZionAuthorize(Roles = "EditRole")]
        public ActionResult Edit(int id)
        {
            return View(RoleRepository.Get(id));
        }
        [ZionAuthorize(Roles = "EditRole")]
        [HttpPost]
        public ActionResult Edit(RoleRepository role)
        {
            if (ModelState.IsValid)
            {
                RoleRepository.Update(role);
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = "true";
            return View(role);
        }

        [ZionAuthorize(Roles = "SearchRole")]
        public ActionResult Search(string id, int? page)
        {
            ViewBag.SearchText = id;
            return View("Index", RoleRepository.Search(id, page));
        }
        [ZionAuthorize(Roles = "DeleteRole")]
        public ActionResult Delete(int id)
        {
            RoleRepository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
