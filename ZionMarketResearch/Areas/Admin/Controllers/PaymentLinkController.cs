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
    public class PaymentLinkController : Controller
    {
        //
        // GET: /PaymentLink/
        [ZionAuthorize(Roles = "ListPaymentLink")]
        public ActionResult Index(int? id)
        {
            return View(PaymentLinkRepository.List(id));
        }
        [ZionAuthorize(Roles = "CreatePaymentLink")]
        public ActionResult Create()
        {
            return View();
        }
        [ZionAuthorize(Roles = "CreatePaymentLink")]
        [HttpPost]
        public ActionResult Create(PaymentLinkRepository payment)
        {
            if (ModelState.IsValid)
            {
                PaymentLinkRepository.Create(payment);
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = "true";
            return View(payment);
        }
        [ZionAuthorize(Roles = "EditPaymentLink")]
        public ActionResult Edit(int id)
        {
            return View(PaymentLinkRepository.Get(id));
        }
        [ZionAuthorize(Roles = "EditPaymentLink")]
        [HttpPost]
        public ActionResult Edit(PaymentLinkRepository payment)
        {
            if (ModelState.IsValid)
            {
                PaymentLinkRepository.Update(payment);
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = "true";
            return View(payment);
        }
        [ZionAuthorize(Roles = "DeletePaymentLink")]
        public ActionResult Delete(int id)
        {
            PaymentLinkRepository.Delete(id);
            return RedirectToAction("Index");
        }
        [ZionAuthorize(Roles = "SearchPaymentLink")]
        public ActionResult Search(string id, int? page)
        {
            ViewBag.SearchText = id;
            return View("Index", PaymentLinkRepository.Search(id, page));
        }

    }
}
