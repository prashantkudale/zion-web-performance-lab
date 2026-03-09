using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.constants;
using ZionMarketResearch.Extension;
using ZionMarketResearch.Log;
using ZionMarketResearch.Models;
using ZionMarketResearch.Models.Factory;
//using Google.Apis.Auth.OAuth2;
//using Google.Cloud.Translation.V2;

namespace ZionMarketResearch.Controllers
{
    [LogException]
    public class ReportController : Controller
    {
        //
        // GET: /Report/

        public ActionResult Index(string url, string reffer, string Lang)
        {
            ViewBag.Lang = Lang != "" ? Lang : "en";

            if (string.IsNullOrEmpty(url) || !(Lang == "" || Lang == "fr"))
                return HttpNotFound();

            if (string.IsNullOrEmpty(url))
                return HttpNotFound("URL for report should not be empty or null.");
            //MainPageReportView report = ReportRepository.GetReportByUrl(url);

            bool showResellerDescription = Request.QueryString["rs"] != null && Convert.ToBoolean(Request.QueryString["rs"]);

            MainPageReportView report = ReportFactory.GetReportByUrl(url, 0);

                  
            if (report == null || string.IsNullOrEmpty(report.ReportTitle))
                return HttpNotFound("Report URL not found. Please contact to https://zionmarketresearch.com for more details.");
            
            while (report != null && string.IsNullOrWhiteSpace(report.MainPageDescription))
            {
                report = ReportFactory.GetReportByUrl(url, 0);
            }
            ViewBag.ReportId = Util.Utility.Encryptstring(report.ReportId.ToString());
            ViewBag.ReportUrl = Util.Utility.Encryptstring(report.ReportUrl);
            ViewBag.ReportUrlOrig = report.ReportUrl;
            ViewBag.DeliveryFormat = report.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : report.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : report.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : report.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail";

            #region Meta Data for page
            string[] source = Request.RawUrl.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            ZionMarketResearch.Models.MetaData m = source[0] != "report" ? ZionMarketResearch.Models.MetaData.GetMetaData(report, source[0] == "market-analysis" ? "freeanalysis" : source[0]) : null;
            ViewBag.Title = m != null && !string.IsNullOrEmpty(m.Title) ? m.Title : (!string.IsNullOrEmpty(report.MetaTitle) ? report.MetaTitle : report.ReportTitle);
            ViewBag.MetaDescription = m != null && !string.IsNullOrEmpty(m.Description) ? m.Description : report.MetaDescription;
            ViewBag.MetaKeywords = m != null && !string.IsNullOrEmpty(m.Keywords) ? m.Keywords : report.MetaKeyword;
            report.MainPageDescription = AddRequestSampleLinkAfterImage(report.MainPageDescription, "<img", "<a href=\"/sample/" + report.ReportUrl + "\" class=\"button-43\">Request Free Sample</a>");
            if (report.MainPageDescription.Contains("<img") && report.MainPageDescription.Contains("src=\"") && report.MainPageDescription.Length > report.MainPageDescription.IndexOf("src=\"") + 6)
            {
                if (report.MainPageDescription.Contains("ogImageClass")) { }
                string _tempOGImagePath = report.MainPageDescription.Substring(report.MainPageDescription.IndexOf("src=\"") + 5);
                _tempOGImagePath = _tempOGImagePath.Substring(0, _tempOGImagePath.IndexOf("\""));
                ViewBag.Firstimage = _tempOGImagePath;
            }

            ViewBag.css = "report_css.css";
            //var reportIDs = ConfigurationManager.AppSettings["FreeAnalysisShowOnReportPage"].Split(',').Select(x => Convert.ToInt32(x)).ToList();
            //ViewBag.ShowFreeAnalysis = reportIDs.Where(x => x == report.ReportId).Count() > 0;


            #endregion

            if (ViewBag.Lang != "en")
            {
                //ViewBag.Title = ViewBag.Title != null ? TranslateText(ViewBag.Title, "en|"+ViewBag.Lang) : ViewBag.Title;
                ViewBag.Title = ViewBag.Title != null ? Translate(ViewBag.Title, ViewBag.Lang, "en") : ViewBag.Title;
                ViewBag.MetaDescription = ViewBag.MetaDescription != null ? Translate(ViewBag.MetaDescription, ViewBag.Lang, "en") : ViewBag.MetaDescription;
                ViewBag.MetaKeywords = ViewBag.MetaKeywords != null ? Translate(ViewBag.MetaKeywords, ViewBag.Lang, "en") : ViewBag.MetaKeywords;
            }

            return View("Index", report);
        }

        public ActionResult AskToAnalyst(string url, string reffer)
        {
            ViewBag.MetaTag = "<meta name=\"robots\" content=\"noindex,nofollow\" />";
            return ReportForm(url, (int)GlobalConstant.FormType.AskToAnalyst);
        }

        [OutputCache(Duration = 3600, VaryByParam = "isSideBar")]
        public ActionResult LatestReport(bool isSideBar = true)
        {
            return !isSideBar ? PartialView(ReportRepository.LatestReport()) : PartialView("~/Views/Report/RightSideBar/_RightSideBarLatestReports.cshtml", ReportRepository.LatestReport(false));
        }
        [OutputCache(Duration = 3600, VaryByParam = "isSideBar")]
        public ActionResult LatestUpcomingReports(bool isSideBar = true)
        {
            return !isSideBar ? PartialView(ReportRepository.LatestUpcomingReports()) : PartialView("~/Views/Report/RightSideBar/_RightSideBarLatestUpcomingReports.cshtml", ReportRepository.LatestUpcomingReports(false));
        }

        public ActionResult RelatedReports(string id)
        {
            return PartialView(ReportRepository.RelatedReports(id));
        }

        public ActionResult AllFeaturedReports(int? page)
        {
            var allfeaturedReports = ReportRepository.AllFeaturedReports(page);

            if (allfeaturedReports == null)
                return RedirectToAction("AllFeaturedReports", new { page = default(int?) });
            return View(allfeaturedReports);
        }

        [OutputCache(Duration = 3600, VaryByParam = "isSideBar")]
        public ActionResult LatestFeaturedReports(bool isSideBar = true)
        {
            return PartialView(ReportRepository.LatestFeaturedReport(!isSideBar));
        }

        public ActionResult AllReports(int? page)
        {
            if (page != null && page <= 0)
                return HttpNotFound("Page number should not be lesss than or eqaul to zero.");
            var allReports = ReportRepository.AllPublishedReports(page);

            if (allReports == null)
                return RedirectToAction("AllReports", new { page = default(int?) });
            ViewBag.css = "reportslist_css.css";
            return View(allReports);
        }

        public ActionResult Upcoming(string url)
        {
            var report = ReportRepository.GetReportByUrl(url);
            if (report == null)
                return HttpNotFound();
            return View(report);
        }

        public ActionResult AllUpcoming(int? page)
        {
            if (page != null && page <= 0)
                return HttpNotFound("Page number should not be lesss than or eqaul to zero.");
            var upcomingReports = ReportRepository.AllUpcomingReports(page);

            if (upcomingReports == null)
                return RedirectToAction("AllUpcoming", new { page = default(int?) });

            return View(upcomingReports);
        }

        public ActionResult TOC(string url, string reffer)
        {
            if (string.IsNullOrEmpty(url))
                return HttpNotFound();

            var v = ReportForm(url, (int)GlobalConstant.FormType.RequestTOC);

            if (v == null)
                return HttpNotFound();

            var model = ((MainPageReportView)((ViewResult)v).ViewData.Model);
            if (model != null && string.IsNullOrEmpty(model.TableOfContent))
            {
                return v;
            }
            return Index(url, reffer, string.Empty);
        }

        public ActionResult DownloadTOC(string url, string reffer)
        {
            return ReportForm(url, (int)GlobalConstant.FormType.RequestTOC);
        }

        public ActionResult Sample(string url, string reffer)
        {
            //if (string.IsNullOrEmpty(url))
            //    return HttpNotFound();
            //return Index(url, reffer);
            return ReportForm(url, (int)GlobalConstant.FormType.RequestSample);
        }

        public ActionResult Methodology(string url, string reffer)
        {
            var res = (ViewResult)ReportForm(url, (int)GlobalConstant.FormType.RequestMethodology);
            if (((MainPageReportView)res.Model).IsUpcoming)
                return HttpNotFound();
            return res;


        }

        private string GetRequestType(int formType)
        {
            var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            var strformType = Enum.GetName(typeof(GlobalConstant.FormType), formType);

            return r.Replace(strformType, " ");
        }

        ActionResult ReportForm(string url, int formType)
        {
            var report = ReportFactory.GetReportByUrl(url, 0);

            if (report == null)
                return HttpNotFound();

            ViewBag.ReportId = report.ReportId.ToString();
            ViewBag.ReportUrl = report.ReportUrl;
            ViewBag.FormType = formType;
            ViewBag.RequestType = GetRequestType(formType);

            ViewBag.FormTitle = formType == (int)GlobalConstant.FormType.RequestSample ? "Request a Free Sample of" : formType == (int)GlobalConstant.FormType.BuyingEnquiry ? "Inquiry Before Buying for"
                : formType == (int)GlobalConstant.FormType.AskToAnalyst ? "Request for Ask An Analyst"
                : formType == (int)GlobalConstant.FormType.RequestTOC ? "Request TOC of " : formType == (int)GlobalConstant.FormType.RequestBrochure ? "Request Free Brochure of"
                : formType == (int)GlobalConstant.FormType.RequestDiscount ? "Request For Discount for" : formType == (int)GlobalConstant.FormType.RequestMethodology ? "Request methodology of" : "";


            #region Meta Data for page
            var source = Request.RawUrl.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var m = ZionMarketResearch.Models.MetaData.GetMetaData(report,
                formType == (int)GlobalConstant.FormType.RequestSample ? "sample" : formType == (int)GlobalConstant.FormType.BuyingEnquiry ? "inquiry"
                : formType == (int)GlobalConstant.FormType.RequestTOC ? "toc" : "");
            ViewBag.Title = m != null && !string.IsNullOrEmpty(m.Title) ? m.Title : (!string.IsNullOrEmpty(report.MetaTitle) ? report.MetaTitle : report.ReportTitle);
            ViewBag.MetaDescription = m != null && !string.IsNullOrEmpty(m.Description) ? m.Description : report.MetaDescription;
            ViewBag.MetaKeywords = m != null && !string.IsNullOrEmpty(m.Keywords) ? m.Keywords : report.MetaKeyword;
            #endregion

            return View("ReportFormNew", report);
        }

        ActionResult CorporateForm(string url)
        {
            CorporateEmailRepository cer = TempData["cer"] as CorporateEmailRepository;
            if (Session["CrmLeadID"] == null || cer == null)
            {
                return HttpNotFound("URL for report should not be empty or null.");
            }
            TempData.Remove("cer");
            Session["CrmLeadID"] = null;

            ViewBag.CrmLeadID = cer.CrmLeadID;
            ViewBag.Id = cer.Id;
            return View("ShowCorporateEmail", cer);
        }
        public ActionResult Inquiry(string url, string reffer)
        {
            //if (string.IsNullOrEmpty(url))
            //    return HttpNotFound();
            //return Index(url, reffer);
            return ReportForm(url, (int)GlobalConstant.FormType.BuyingEnquiry);
        }

        public ActionResult Analysis(string url, string reffer)
        {
            if (string.IsNullOrEmpty(url))
                return HttpNotFound();
            ViewResult v = (ViewResult)Index(url, reffer, string.Empty);
            if (((MainPageReportView)v.ViewData.Model).FreeAnalysis == null)
                return RedirectToActionPermanent("index", new { url = url, reffer = reffer });
            return v;
        }

        public ActionResult RequestBrochure(string url, string reffer)
        {
            return ReportForm(url, (int)GlobalConstant.FormType.RequestBrochure);
        }

        public ActionResult RequestDiscount(string url, string reffer)
        {
            return ReportForm(url, (int)GlobalConstant.FormType.RequestDiscount);
        }

        public ActionResult AutoComplete(string query)
        {
            return Json(new { suggestions = ReportRepository.AutoComplete(query) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMethodology(string url)
        {
            return Json(new { Methodology = ReportRepository.GetMethodology(url) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetpaymentLinkReport(string url)
        {
            if (string.IsNullOrEmpty(url))
                return HttpNotFound("URL for report should not be empty or null.");
            MainPageReportView report = ReportRepository.GetReportByPaymentLink(url);
            if (report == null || string.IsNullOrEmpty(report.ReportTitle))
                return HttpNotFound("Report URL not found. Please contact to https://zionmarketresearch.com for more details.");
            ViewBag.ReportId = Util.Utility.Encryptstring(report.ReportId.ToString());
            ViewBag.ReportUrl = Util.Utility.Encryptstring(report.ReportUrl);
            ViewBag.DeliveryFormat = report.DeliveryFormat == 0 ? "fa-file-pdf-o iconsize pdf" : report.DeliveryFormat == 1 ? "fa-file-word-o iconsize doc" : report.DeliveryFormat == 2 ? "fa-file-excel-o iconsize xl" : report.DeliveryFormat == 3 ? "fa-file-powerpoint-o iconsize ppt" : "fa-envelope-o iconsize mail";

            #region Meta Data for page
            string[] source = Request.RawUrl.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            ZionMarketResearch.Models.MetaData m = source[0] != "report" ? ZionMarketResearch.Models.MetaData.GetMetaData(report, source[0] == "market-analysis" ? "freeanalysis" : source[0]) : null;
            ViewBag.Title = m != null && !string.IsNullOrEmpty(m.Title) ? m.Title : (!string.IsNullOrEmpty(report.MetaTitle) ? report.MetaTitle : report.ReportTitle);
            ViewBag.MetaDescription = m != null && !string.IsNullOrEmpty(m.Description) ? m.Description : report.MetaDescription;
            ViewBag.MetaKeywords = m != null && !string.IsNullOrEmpty(m.Keywords) ? m.Keywords : report.MetaKeyword;
            #endregion

            return View("Index", report);

        }

        [Utility.AllowCrossSite]
        public ActionResult GetAllReports(int? page)
        {
            return Json(ReportRepository.AllPublishedReports(page).Select(r =>
            {
                r.Description = Util.Utility.StripTagsRegexCompiled(r.Description).ZSubstring(0, 300);
                return r;
            }), JsonRequestBehavior.AllowGet);
        }

        [Utility.AllowCrossSite]
        public ActionResult GetAllUpcomingReports(int? page)
        {
            return Json(ReportRepository.AllUpcomingReports(page).Select(r =>
            {
                r.Description = Util.Utility.StripTagsRegexCompiled(r.Description).ZSubstring(0, 300);
                return r;
            }), JsonRequestBehavior.AllowGet);
        }

        public string AddRequestSampleLinkAfterImage(string source, string toFind, string toInsert)
        {

            //toFind = "<img";
            //source = "<h2>Fragrance Ingredients Market Size: Industry Perspective</h2><p>The <strong>global fragrance ingredients market size</strong> was evaluated at <strong>$15 Billion in 2022 </strong>and is slated to hit <strong>$25 Billion by the end of 2030</strong> with a <strong>CAGR of nearly 7.6%</strong> between 2023 and 2030. The market report is an indispensable guide on growth factors, challenges, restraints, and opportunities in the global marketplace. The report covers the geographical market along with a comprehensive competitive landscape analysis. Additionally, the report explores the investor and stakeholder space to help companies make data-driven decisions.</p><p><img alt=\"Global Fragrance Ingredients Market Size\" src=\"https://www.zionmarketresearch.com/content/uploadedimages/global-fragrance-ingredients-market-size.png\" /></p><p><img alt=\"Global Fragrance Ingredients Market Size\" src=\"https://www.zionmarketresearch.com/content/uploadedimages/global-fragrance-ingredients-market-size2.png\" /></p><h3>Global Fragrance Ingredients Market: Synopsis</h3><p>Fragrances are unique blends of synthetic and natural ingredients that are added to any product to impart a distinct aroma. Moreover, they are used in perfumes as well as in different consumer goods such as personal care, home cleaning chemicals, soaps, detergents, sanitizers, cosmetics, disinfectants, toiletries, and others. Reportedly, fragrance ingredients are a combination of various chemicals which give a fragrance that coincides with an aroma. The ingredients of fragrances are derived from natural as well as petroleum raw materials.</p><p><img alt=\"Global Fragrance Ingredients Market Size\" src=\"https://www.zionmarketresearch.com/content/uploadedimages/global-fragrance-ingredients-market-size.png3\" /></p><h3>Key Insights</h3><ul><li>As per the analysis shared by our research analyst, the global fragrance ingredients market is projected to expand at the compound annual growth rate of around 7.6% over the forecast timespan (2023-2030)</li></ul><p><img alt=\"Global Fragrance Ingredients Market Size\" src=\"https://www.zionmarketresearch.com/content/uploadedimages/global-fragrance-ingredients-market-size.png4\" /></p><p><strong>To know more about this report,&nbsp;<a href=\"https://www.zionmarketresearch.com/sample/fragrance-ingredients-market\" target=\"_blank\">Request a sample copy</a></strong></p>";
            //toInsert = "<a href=\"#\" class=\"button-43\">Click Here</a>";

            int count = 0;
            int n = 0;

            while ((n = source.IndexOf(toFind, n) + 1) != 0)
            {
                source = source.Insert(source.IndexOf(">", n) + 1, toInsert);
                n++;
                count++;
            }
            return source;
        }

        public String Translate(String word, string toLanguage, string fromLanguage)
        {
            string translatedText = "";
            //try
            //{
            //    TranslationClient client1 = TranslationClient.Create(GoogleCredential.FromFile(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "/Content/language-translation-421612-f62c54a4c9aa.json")), 0);
            //    TranslationResult result = client1.TranslateText(word, toLanguage, fromLanguage, null);
            //    translatedText = result.TranslatedText;
            //}
            //catch (Exception exception)
            //{
            //    exception.ToString();
            //    translatedText = word;
            //}
            return translatedText!="" ? translatedText : word;
        }
    }
}
