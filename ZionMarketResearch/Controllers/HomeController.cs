using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        #region Constructor
        private readonly IJobVacancyRepository _vacancy;
        public HomeController()
        {
            _vacancy = new JobVacancyRepository();
        }
        #endregion

        [OutputCache(Duration = 600, VaryByParam = "none")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ThankYou(int? zn)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated && zn != null)
            {
                var report = ReportRepository.GetReportById((int)zn);
                Console.WriteLine("ID = " + zn);
                if (report != null)
                    ViewBag.Message = "<div style='margin-top:20px;margin-bottom:20px;margin-left:10px;'><b>" + report.ReportTitle + "</b></div>";
            }
            else
            {
                if (Session["formMessage"] == null || zn == null)
                    return HttpNotFound();
                ViewBag.Message = Session["formMessage"];
                Session["formMessage"] = null;
            }
            return View();
        }

        public ActionResult AboutUs()
        {
            return View("About-Us");
        }

        public ActionResult ContactUs()
        {
            return View("Contact-Us");
        }

        public ActionResult PrivacyPolicy()
        {
            return View("Privacy-Statement");
        }
        public ActionResult TermsNCondition()
        {
            return View("Terms-And-Conditions");
        }
        public ActionResult HowToOrder()
        {
            return View("How-To-Order");
        }

        public ActionResult Disclaimer()
        {
            return View("Disclaimer");
        }
        public ActionResult FAQ()
        {
            return View("FAQs");
        }
        public ActionResult ReturnPolicy()
        {
            return View("Return-Policy");
        }
        public ActionResult WhyUs()
        {
            return View();
        }
        public ActionResult Testimonials()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            //Special Redirection
            if (Request.Url.PathAndQuery.EndsWith("."))
            {
                var lastDot = Request.Url.PathAndQuery.LastIndexOf(".");
                return RedirectPermanent("https://www.zionmarketresearch.com" + Request.Url.PathAndQuery.Substring(0, lastDot).Replace("/notfound?404;https://www.zionmarketresearch.com:443", string.Empty));
            }
            Response.StatusCode = 404;
            Response.StatusDescription = "Page Not Found";
            return View();
        }

        public ActionResult ServerError()
        {
            Response.StatusCode = 500;
            Response.StatusDescription = "Server Error";
            return View();
        }

        public ActionResult ZMRInNews(int? page)
        {
            return View(ZionInNewsRepository.AllLinks(page));
        }

        public ActionResult Career()
        {
            return View(_vacancy.GetVacancies());
        }

        public ActionResult CareerDetail(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            return View(_vacancy.GetVacancy((int)id));
        }
        string[] allowedExtension = { ".doc", ".docx", ".pdf" };
        //public ActionResult ApplyJob(JobApplication jobApplication)
        //{
        //    if (!CaptchaRepository.IsCaptchaValid(jobApplication.Captcha))
        //        return Json(new { Success = false, Message = "Invalid Captcha." });

        //    if (Request.Files.Count == 0)
        //        return Json(new { Success = false, Message = "Please Select File." });

        //    if (Request.Files[0].ContentLength > 1048576)
        //        return Json(new { Success = false, Message = "File is too large." });

        //    if (!allowedExtension.Contains(System.IO.Path.GetExtension(Request.Files[0].FileName)))
        //        return Json(new { Success = false, Message = "File format is not valid." });

        //    if (ModelState.IsValid && CaptchaRepository.IsCaptchaValid(jobApplication.Captcha) && Request.Files.Count > 0 && Request.Files[0].ContentLength <= 1048576)
        //    {
        //        jobApplication.File = Request.Files[0];
        //        return Json(_vacancy.ApplyToJob(jobApplication));
        //    }
        //    return Json(new { Success = false, Message = "Your application has not submitted. Please correct errors if any." });
        //}

        //10HRS
        //[OutputCache(Duration = 36000)]
        public PartialViewResult NavigationMenu()
        {
            return PartialView("_NavigationMenu");
        }

        [OutputCache(Duration = 36000)]
        public ActionResult Footer()
        {
            //return PartialView("_footer", NewsRepository.LatestNews().OrderByDescending(x => x.PublishedDate).Take(4).ToList());
            return PartialView("_footer", null);
        }

        [OutputCache(Duration = 36000)]
        public ActionResult FooterMS()
        {
            return PartialView("_footer", null);
        }
    }
}
