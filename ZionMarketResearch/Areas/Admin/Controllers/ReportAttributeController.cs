using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionAdmin.Security;
using ZionMarketResearch.Areas.Admin.Repository;
using ZionMarketResearch.Models;

namespace ZionAdmin.Controllers
{
    [ZionAuthorize(Roles = "CreateReport")]
    public class ReportAttributeController : Controller
    {
        #region Constructor
        private readonly IReportAttributeRepository _reportAttribute;
        public ReportAttributeController()
        {
            _reportAttribute = new ReportAttributeRepository();
        }
        #endregion

        //
        // GET: /Admin/ReportAttribute/
        public ActionResult Index()
        {
            return View(_reportAttribute.GetAll());
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(tblreportattribute attribute)
        {
            if (ModelState.IsValid)
            {
                if (_reportAttribute.Insert(attribute) > 0)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(attribute);
        }

        public ActionResult Edit(int id)
        {
            return View(_reportAttribute.GetById(id));
        }

        [HttpPost]
        public ActionResult Edit(tblreportattribute attribute)
        {
            if (ModelState.IsValid)
            {
                if (_reportAttribute.Update(attribute) > 0)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(attribute);
        }

        public ActionResult Delete(int id)
        {
            _reportAttribute.Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult ShowAttributeCheckbox(int[] selectedAttributes = null)
        {
            var allAttributes = _reportAttribute.GetAll();
            ViewBag.SelectedAttributes = selectedAttributes;
            return PartialView(allAttributes);
        }
    }
}
