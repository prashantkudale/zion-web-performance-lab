using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionAdmin.Repository;
using ZionAdmin.Security;


namespace ZionAdmin.Controllers
{
    public class ZionInNewsController : Controller
    {
        //
        // GET: /Admin/ZionInNews/

        [ZionAuthorize(Roles = "Admin,CreateZionInNews")]
        public ActionResult Index(int? id)
        {
            return View(ZMRInNewsRepository.List(id));
        }

        [ZionAuthorize(Roles = "Admin,CreateZionInNews")]
        public ActionResult Create()
        {
            return View();
        }

        [ZionAuthorize(Roles = "Admin,CreateZionInNews")]
        [HttpPost]
        public ActionResult Create(ZMRInNewsRepository entity)
        {
            if(ModelState.IsValid && ZMRInNewsRepository.Insert(entity) > 0)
            {
                return RedirectToAction("Index");
            }
            return View(entity);
        }

        [ZionAuthorize(Roles = "Admin,CreateZionInNews")]
        public ActionResult Edit(int id)
        {
            return View(ZMRInNewsRepository.GetById(id));
        }

        [ZionAuthorize(Roles = "Admin,CreateZionInNews")]
        [HttpPost]
        public ActionResult Edit(ZMRInNewsRepository entity)
        {
            if(ModelState.IsValid && ZMRInNewsRepository.Update(entity) > 0)
            {
                return RedirectToAction("Index");
            }
            return View(entity);
        }

        [ZionAuthorize(Roles = "Admin,CreateZionInNews")]
        public ActionResult Delete(int id)
        {
            ZMRInNewsRepository.Delete(id);
            return RedirectToAction("Index");
        }

        [ZionAuthorize(Roles = "Admin,CreateZionInNews")]
        public ActionResult Search(string searchText, int? id)
        {
            return View("Index", ZMRInNewsRepository.Search(searchText, id));
        }

    }
}
