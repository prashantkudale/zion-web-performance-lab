using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Controllers
{
    public class FormController : Controller
    {
        //
        // GET: /Form/

        public ActionResult ShowForm(int FormType, string ReportId, string SearchText = null)
        {
            ViewBag.ButtonText = FormType == 0 ? "Request Sample" : "Let us know";
            ViewBag.FormType = FormType;
            ViewBag.ReportId = ReportId;
            ViewBag.Search = SearchText;


            var report = default(MainPageReportView);

            if (report != null && ReportId != null && Int32.TryParse(Util.Utility.Decryptstring(ReportId), out int reportID))
            {
                report = ReportRepository.GetReportById(reportID);

                ViewBag.ReportTitle = report.ReportTitle;
                ViewBag.ReportUrl = Request.Url.Host + new UrlHelper(Request.RequestContext).Action("Index", "Report", new { url = report.ReportUrl });
            }
            var p = FormRepository.GetFromCookie();
            if (p == null)
            {
                p = new FormRepository();
            }
            ViewBag.Countries = ZionMarketResearch.Models.CountryRepository.GetCountry();

            if (TempData["Error"] != null)
            {
                foreach (var err in (List<KeyValuePair<string, ModelState>>)TempData["Error"])
                {
                    ModelState.AddModelError(err.Key, err.Value.Errors.FirstOrDefault().Exception);
                }
            }

            return PartialView("NewForm", TempData["model"] != null ? TempData["model"] : p);
        }

        //[HttpPost]
        //public ActionResult Submit(FormRepository fr, int? fromNews)
        //{
        //    var captchaValid = CaptchaRepository.IsCaptchaValid(fr.Captcha);
        //    if (captchaValid && ModelState.IsValid)
        //    {
        //        captchaValid = true;
        //        int id = FormRepository.Submit(fr);

        //        //// Corporate Email ID code
        //        //string _emailDomain = fr.Email.Substring(fr.Email.IndexOf('@') + 1);
        //        //string CorporateEmailDomains = System.Configuration.ConfigurationManager.AppSettings["CorporateEmailDomains"];
        //        //List<string> CorporateEmailDomainsList = CorporateEmailDomains.Split(',').ToList();
        //        //for (int i=0; i < CorporateEmailDomainsList.Count; i++)
        //        //{
        //        //    if (CorporateEmailDomainsList[i] == _emailDomain && Session["CrmLeadID"]!=null)
        //        //    {
        //        //        CorporateEmailRepository cer = new CorporateEmailRepository { Id = id, CrmLeadID = (int)Session["CrmLeadID"], Email = fr.Email };
        //        //        TempData["cer"] = cer;
        //        //        fr.CrmLeadID = cer.CrmLeadID;
        //        //        TempData["fr"] = fr;
        //        //        return RedirectToRoute("CorporateEmailIDRoute", new { id = fr.ReportId });
        //        //        //return RedirectToAction("ShowCorporateEmail", "Form", new { url = fr.ReportUrl });
        //        //    }
        //        //}
                
        //        return RedirectToRoute("ThankYouRoute", new { zn = id });

        //        //return RedirectToRoute("ThankYouRoute", new { zn = 1 });
        //    }
        //    ViewBag.Countries = ZionMarketResearch.Models.CountryRepository.GetCountry();
        //    //return PartialView("ShowForm", fr);

        //    TempData["model"] = fr;

        //    var modelError = ModelState.Where(x => x.Value.Errors != null && x.Value.Errors.Count > 0).ToList();

        //    if (!captchaValid)
        //    {
        //        var captchaError = ModelState.FirstOrDefault(x => x.Key == "Captcha").Value;
        //        captchaError.Errors.Add(new Exception("Invalid Captcha"));
        //        modelError.Add(new KeyValuePair<string, System.Web.Mvc.ModelState>("Captcha", captchaError));
        //    }


        //    TempData["Error"] = modelError;
        //    //
        //    return Redirect(Request.UrlReferrer.ToString());
        //}

        public ActionResult ShowAskToAnalyst(int FormType, string ReportId)
        {
            ViewBag.FormType = FormType;
            ViewBag.ReportId = ReportId;
            return PartialView();
        }

        //[HttpPost]
        //public ActionResult SubmitAskToAnalyst(AskForAnalystRepository askToAnalyst)
        //{
        //    if (ModelState.IsValid && CaptchaRepository.IsCaptchaValid(askToAnalyst.Captcha))
        //    {
        //        int id = AskForAnalystRepository.Submit(askToAnalyst);
        //        return RedirectToRoute("ThankYouRoute", new { zn = id });
        //    }
        //    return PartialView("ShowAskToAnalyst", askToAnalyst);
        //}

        public ActionResult ShowQuickInquiry(int FormType, string ReportId)
        {
            ViewBag.FormType = FormType;
            ViewBag.ReportId = ReportId;
            return PartialView();
        }

        //public ActionResult SubmitQuickInquiry(QuickInquiry inquiry)
        //{
        //    if (ModelState.IsValid && CaptchaRepository.IsCaptchaValid(inquiry.Captcha) && ModelState.IsValid)
        //    {
        //        int id = QuickInquiry.SubmitQuickForm(inquiry);
        //        return RedirectToRoute("ThankYouRoute", new { zn = id });
        //    }
        //    return PartialView(inquiry);
        //}

        [HttpPost]
        public ActionResult SubmitSampleRequest(AvailPDF inquiry)
        {
            if (ModelState.IsValid)
            {
                var html = AvailPDF.SubmitAvailPDF(inquiry);

                //string _emailDomain = inquiry.Email.Substring(inquiry.Email.IndexOf('@') + 1);
                //string CorporateEmailDomains = System.Configuration.ConfigurationManager.AppSettings["CorporateEmailDomains"];
                //List<string> CorporateEmailDomainsList = CorporateEmailDomains.Split(',').ToList();
                //for (int i = 0; i < CorporateEmailDomainsList.Count; i++)
                //{
                //    if (CorporateEmailDomainsList[i] == _emailDomain && Session["CrmLeadID"] != null)
                //    {
                //        CorporateEmailRepository cer = new CorporateEmailRepository { Id = inquiry.ReportId, CrmLeadID = (int)Session["CrmLeadID"], Email = inquiry.Email };
                //        TempData["cer"] = cer;
                //        inquiry.CrmLeadID = cer.CrmLeadID;
                //        TempData["fr"] = inquiry;
                //        return RedirectToRoute("CorporateEmailIDRoute", new { id = inquiry.ReportId });
                //        //return RedirectToAction("ShowCorporateEmail", "Form", new { url = inquiry.ReportId});
                //    }
                //}

                return Json(new { Success = !string.IsNullOrEmpty(html), html = html });
            }
            return Json(new { Success = false });
        }

        [HttpPost]
        public ActionResult AskInfo(AskInfo askInfo)
        {
            if (ModelState.IsValid)
            {
                var html = askInfo.Submit();
                return Json(new { Success = !string.IsNullOrEmpty(html), html = "<h2>Thanks for contacting us.</h2><h3>Your form has been submitted. We will contact you.</h3>" });
            }
            return Json(new { Success = false });
        }

        
        public ActionResult ShowCorporateEmail()
        {
            if (Session["ShowErr"] != null)
            { }
            CorporateEmailRepository cer = TempData["cer"] as CorporateEmailRepository;
            if (Session["CrmLeadID"] == null || cer == null)
            {
                return HttpNotFound("URL for report should not be empty or null.");
            }
            TempData.Remove("cer");
            Session["CrmLeadID"] = null;
            ViewBag.CrmLeadID = cer.CrmLeadID;
            ViewBag.Id = cer.Id;
            ViewBag.Email = cer.Email;
            return PartialView();
        }
        [HttpPost]
        public ActionResult CorporateEmailSubmit(CorporateEmailRepository cer) 
        {
            FormRepository fr = TempData["fr"] as FormRepository;
            fr.Email = cer.Email;
            string _emailDomain = cer.Email.Substring(cer.Email.IndexOf('@') + 1);
            string CorporateEmailDomains = System.Configuration.ConfigurationManager.AppSettings["CorporateEmailDomains"];
            List<string> CorporateEmailDomainsList = CorporateEmailDomains.Split(',').ToList();
            for (int i = 0; i < CorporateEmailDomainsList.Count; i++)
            {
                if (CorporateEmailDomainsList[i] == _emailDomain)
                {
                    ModelState.AddModelError("Email", "Alert! Generic email address not acceptable. Please, try with a corporate email address.");
                }
            }
            if (ModelState.IsValid)
            {
                Session.Remove("ShowErr");
                cer.Submit(cer, fr);
                FormRepository.SubmitToPortal(fr,true);
                return RedirectToRoute("ThankYouRoute", new { zn = cer.Id });
            }
            else
            {
                TempData["cer"] = cer;
                Session["CrmLeadID"] = cer.CrmLeadID;
                Session["ShowErr"] = true;
                return RedirectToRoute("CorporateEmailIDRoute", new { id = cer.Id });
                //return HttpNotFound("URL for report should not be empty or null.");
            }
        }
        public ActionResult GetCountry()
        {
            CountryResponse country = null;
            using (var client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var res = client.DownloadString("http://ip-api.com/json/" + Util.Utility.ClientIPAddress);

                if (res != null)
                {
                    country = Newtonsoft.Json.JsonConvert.DeserializeObject<CountryResponse>(res);
                }
            }
            return Json(new { Response = country }, JsonRequestBehavior.AllowGet);
        }
    }

    public class CountryResponse
    {
        public string status { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string region { get; set; }
        public string regionName { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string timezone { get; set; }
        public string MyProperty { get; set; }
        public string query { get; set; }
    }
}
