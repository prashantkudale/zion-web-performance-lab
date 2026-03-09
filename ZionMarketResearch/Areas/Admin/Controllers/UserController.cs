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
    public class UserController : Controller
    {
        //
        // GET: /User/
        [ZionAuthorize(Roles = "ListUser")]
        public ActionResult Index(int? id)
        {
            return View(UserRepository.List(id));
        }
        [ZionAuthorize(Roles = "CreateUser")]
        public ActionResult Create()
        {
            return View();
        }
        [ZionAuthorize(Roles = "CreateUser")]
        [HttpPost]
        public ActionResult Create(UserRepository user)
        {
            if (ModelState.IsValid)
            {
                UserRepository.Create(user);
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = "true";
            return View(user);
        }
        [ZionAuthorize(Roles = "DeleteUser")]
        public ActionResult Delete(int id)
        {
            UserRepository.Delete(id);
            return RedirectToAction("Index");
        }
        [ZionAuthorize(Roles = "EditUser")]
        public ActionResult Edit(int id)
        {
            return View(UserRepository.Get(id));
        }
        [ZionAuthorize(Roles = "EditUser")]
        [HttpPost]
        public ActionResult Edit(UserRepository user)
        {
            if (ModelState.IsValid)
            {
                UserRepository.Update(user);
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = "true";
            return View(user);
        }

        [ZionAuthorize(Roles = "ListUser")]
        public ActionResult Search(string searchText, int? page)
        {
            return View("Index", UserRepository.Search(searchText, page));
        }

    }
}
