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
    public class CategoryController : Controller
    {
        //
        // GET: /Category/

        [ZionAuthorize(Roles = "ListCategory")]
        public ActionResult Index(int? id)
        {
            return View(CategoryRepository.List(id));
        }
        [ZionAuthorize(Roles = "CreateCategory")]
        public ActionResult Create()
        {
            return View();
        }

        [ZionAuthorize(Roles = "CreateCategory")]
        [HttpPost]
        public ActionResult Create(CategoryRepository category)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ErrorMessage = CategoryRepository.Create(category, Request.Files);
            }
            else
            {
                ViewBag.ErrorMessage = "Model Error.|true";
                return View(category);
            }
            return RedirectToAction("index");
        }

        [ZionAuthorize(Roles = "EditCategory")]
        public ActionResult Edit(int id)
        {
            return View(CategoryRepository.Get(id));
        }

        [ZionAuthorize(Roles = "EditCategory")]
        [HttpPost]
        public ActionResult Edit(CategoryRepository category)
        {
            if (ModelState.IsValid)
            {
                CategoryRepository.Update(category, Request.Files);
            }
            else {
                ViewBag.ErrorMessage = "true";
                return View(category);
            }
            return RedirectToAction("Index");
        }

        [ZionAuthorize(Roles = "DeleteCategory")]
        public ActionResult Delete(int id)
        {
            CategoryRepository.Delete(id);
            return RedirectToAction("Index");
        }

        [ZionAuthorize(Roles = "SearchCategory")]
        public ActionResult Search(string id, int? page)
        {
            ViewBag.SearchText = id;
            return View("Index", CategoryRepository.Search(id, page));
        }

        public ActionResult GetChildCategory(int id)
        {
            return Json(CategoryRepository.GetChildCategory(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetParentCategories()
        {
            return Json(CategoryRepository.GetParentCategoryJson(), JsonRequestBehavior.AllowGet);
        }
    }
}
