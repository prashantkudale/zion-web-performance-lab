using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ZionAdmin.Models.LuceneSearch;
using ZionAdmin.Repository;
using ZionAdmin.Security;
using ZionMarketResearch.Areas.Admin.Repository;
using ZionMarketResearch.Log;
using static ZionAdmin.Repository.ReportRepository;

namespace ZionAdmin.Controllers
{
    [LogException]
    public class ReportController : Controller
    {
        #region Constructor
        private readonly IReportAttributeRepository _reportAttribute;
        public ReportController()
        {
            _reportAttribute = new ReportAttributeRepository();
        }
        #endregion

        //
        // GET: /Report/
        [ZionAuthorize(Roles = "ListReport")]
        public ActionResult Index(bool? showUpcoming, bool? showPublished, bool? showIsDeleted, int? id, int? year, int? monthyear)
        {
            ViewBag.ShowUpcoming = showUpcoming ?? false;
            ViewBag.ShowPublished = showPublished ?? false;
            ViewBag.IsDeleted = showIsDeleted ?? false;

            return View(ReportRepository.List(showUpcoming, showPublished, showIsDeleted, id, year, monthyear));
        }
        [ZionAuthorize(Roles = "CreateReport")]
        public ActionResult Create()
        {
            ViewBag.ReportAttributes = _reportAttribute.GetAll();
            return View();
        }
        [ZionAuthorize(Roles = "CreateReport")]
        [HttpPost]
        public ActionResult Create(ReportRepository report)
        {
            if (ModelState.IsValid)
            {
                ReportRepository.Create(report, Request.Files);
                return RedirectToAction("Index");
            }
            #region ShowError
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (ModelState ms in ModelState.Values)
            {
                foreach (ModelError me in ms.Errors)
                {
                    sb.Append("<li>");
                    sb.Append(me.ErrorMessage);
                    sb.Append("</li>");
                }
            }
            sb.Append("</ul>");
            ViewBag.ErrorMessage = sb.ToString();
            #endregion
            return View("Create", report);
        }
        [ZionAuthorize(Roles = "EditReport")]
        public ActionResult Edit(int id)
        {
            var report = ReportRepository.Get(id);
            var attributes = _reportAttribute.GetAll();

            return View(report);
        }
        [ZionAuthorize(Roles = "EditReport")]
        [HttpPost]
        public ActionResult Edit(ReportRepository report)
        {
            if (ModelState.IsValid)
            {
                ReportRepository.Update(report, Request.Files);
            }
            #region ShowError
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (ModelState ms in ModelState.Values)
            {
                foreach (ModelError me in ms.Errors)
                {
                    sb.Append("<li>");
                    sb.Append(me.ErrorMessage);
                    sb.Append("</li>");
                }
            }
            sb.Append("</ul>");
            ViewBag.ErrorMessage = sb.ToString();
            #endregion
            return RedirectToAction("Index");
        }
        [ZionAuthorize(Roles = "SearchReport")]
        public ActionResult Search(string id, bool? showUpcoming, bool? showPublished, int? page)
        {
            ViewBag.SearchText = id;
            ViewBag.ShowUpcoming = showUpcoming ?? false;
            ViewBag.ShowPublished = showPublished ?? false;

            return View("Index", ReportRepository.Search(id, page, showPublished, showUpcoming));
        }
        [ZionAuthorize(Roles = "DeleteReport")]
        public ActionResult Delete(int id)
        {
            ReportRepository.Delete(id);
            return RedirectToAction("Index");
        }

        [ZionAuthorize(Roles = "CreateReport")]
        public string WriteAllReportsInLucene()
        {
            ReportRepository.WriteAllReportsInLucene();
            return "Success...";
        }
        public ActionResult ShowAllUrl()
        {

            return View();
        }

        [ZionAuthorize(Roles = "DownloadReport")]
        [HttpGet]
        public ActionResult DownloadReports()
        {
            return View();
        }

        [ZionAuthorize(Roles = "DownloadReport")]
        [HttpPost]
        public ActionResult DownloadReports(DateTime FromDate, DateTime ToDate, int ReportStatus)
        {
            if (FromDate == null || ToDate == null)
                return View();

            var directoryPath = HttpContext.Server.MapPath("~/DownlodReportFiles");

            if (!System.IO.Directory.Exists(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }

            var reports = ReportRepository.GetReportsByDate(FromDate, ToDate, ReportStatus);
            var fileName = Guid.NewGuid().ToString() + ".csv";

            //System.IO.File.AppendAllText(directoryPath + "/" + fileName, "ReportID, ReportTitle, ReportUrl, TOCUrl, SampleURL, BuyingInquiry, CategoryName, NumberOfPages, PriceSingleUser, PriceMultiUser, PriceCorporateUser, PublishDate, Description, TOC, DescriptionWithoutHTML\n");

            var host = "https://www.zionmarketresearch.com";

            //foreach (var r in reports)
            //{
            //    System.IO.File.AppendAllText(directoryPath + "/" + fileName, $"\"{r.ReportId}\",\"{r.ReportTitle.Replace("\"", "\"\"")}\",\"{host + "/report/" + r.ReportUrl}\",\"{host + "/toc/" + r.ReportUrl}\", \"{host + "/sample/" + r.ReportUrl}\",\"{host + "/inquiry/" + r.ReportUrl}\",\"{r.CategoryBreadcrumb}\",{r.NumberOfPages},{r.SingleUser},{r.MultiUser}, {r.Corporate}, {r.PublishedDate},\"{r.MainPageDescription.Replace("\"", string.Empty)}\", \"{r.TableOfContent.Replace("\"", string.Empty)}\",\"{StripHTML(r.MainPageDescription).Replace("\"", string.Empty)}\"\n");
            //}

            using (var writer = new System.IO.StreamWriter(directoryPath + "/" + fileName, true))
            {
                using (var csv = new CsvHelper.CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(reports.Select(x => new
                    {
                        ReportID = x.ReportId,
                        Title = x.ReportTitle,
                        ReportUrl = host + "/report/" + x.ReportUrl,
                        TOCUrl = host + "/toc/" + x.ReportUrl,
                        SampleURL = host + "/sample/" + x.ReportUrl,
                        BuyingIquiry = host + "/inquiry/" + x.ReportUrl,
                        Category = x.CategoryBreadcrumb,
                        Pages = x.NumberOfPages,
                        SingleUser = x.SingleUser,
                        MultiUser = x.MultiUser,
                        CorporateUser = x.Corporate,
                        PublishDate = x.PublishedDate,
                        Description = x.MainPageDescription,
                        TOC = x.TableOfContent,
                        DescriptionWithoutHTML = StripHTML(x.MainPageDescription),
                        IsUpcoming = x.IsUpcoming
                    }));
                }
            }

            HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");

            return new FileStreamResult(new System.IO.FileStream(directoryPath + "/" + fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None, 4096, System.IO.FileOptions.DeleteOnClose), "text/csv");

        }


        [ZionAuthorize(Roles = "ListReport")]
        public ActionResult GenerateTOC(int id) => View();


        [ZionAuthorize(Roles = "ListReport")]
        public ActionResult ArchiveReport() => View(GetArchiveReports());


        public static string StripHTML(string source, bool replaceNewLineTabWithEmpty = true, bool replaceBRWithEmpty = false)
        {
            try
            {
                string result = source;

                if (replaceNewLineTabWithEmpty)
                {
                    // Remove HTML Development formatting
                    // Replace line breaks with space
                    // because browsers inserts space
                    result = source.Replace("\r", " ");
                    // Replace line breaks with space
                    // because browsers inserts space
                    result = result.Replace("\n", " ");
                    // Remove step-formatting
                    result = result.Replace("\t", string.Empty);
                }
                // Remove repeating spaces because browsers ignore them
                result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                      @"( )+", " ");

                // Remove the header (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*head([^>])*>", "<head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*head( )*>)", "</head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<head>).*(</head>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all scripts (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*script([^>])*>", "<script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*script( )*>)", "</script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
                //         string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<script>).*(</script>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all styles (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*style([^>])*>", "<style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*style( )*>)", "</style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<style>).*(</style>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*td([^>])*>", replaceBRWithEmpty ? string.Empty : "\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*br( )*>", replaceBRWithEmpty ? string.Empty : "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*li( )*>", replaceBRWithEmpty ? string.Empty : "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line paragraphs (double line breaks) in place
                // if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*div([^>])*>", replaceBRWithEmpty ? string.Empty : "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*tr([^>])*>", replaceBRWithEmpty ? string.Empty : "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*p([^>])*>", replaceBRWithEmpty ? string.Empty : "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Remove remaining tags like <a>, links, images,
                // comments etc - anything that's enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<[^>]*>", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @" ", " ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&bull;", " * ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lsaquo;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&rsaquo;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&trade;", "(tm)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&frasl;", "/",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lt;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&gt;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&copy;", "(c)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&reg;", "(r)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove all others. More can be added, see
                // http://hotwired.lycos.com/webmonkey/reference/special_characters/
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&(.{2,6});", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // for testing
                //System.Text.RegularExpressions.Regex.Replace(result,
                //       this.txtRegex.Text,string.Empty,
                //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // make line breaking consistent
                result = result.Replace("\n", "\r");

                // Remove extra line breaks and tabs:
                // replace over 2 breaks with 2 and over 4 tabs with 4.
                // Prepare first to remove any whitespaces in between
                // the escaped characters and remove redundant tabs in between line breaks
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\t)", "\t\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\r)", "\t\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\t)", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove multiple tabs following a line break with just one tab
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Initial replacement target string for line breaks
                string breaks = "\r\r\r";
                // Initial replacement target string for tabs
                string tabs = "\t\t\t\t\t";
                for (int index = 0; index < result.Length; index++)
                {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                // That's it.
                return result;
            }
            catch
            {
                //MessageBox.Show("Error");
                return source;
            }
        }

        [ZionAuthorize(Roles = "ListReport")]
        public ActionResult InsertOrUpdateResellerDescription(int id) => View(GetResellerDescriptionByReportID(id));

        [ZionAuthorize(Roles = "ListReport")]
        [HttpPost]
        public ActionResult InsertOrUpdateResellerDescription(ResellerDescription resellerDescription)
        {
            ReportRepository.InsertOrUpdateResellerDescription(resellerDescription);
            return RedirectToAction("Index");
        }

        [ZionAuthorize(Roles = "ListReport")]
        [HttpPost]
        public ActionResult SaveTableOfContent(string tableOfContent, string listOfFigures, string listOfTables, int reportID)
        {
            return Json(new
            {
                Success = (ReportRepository.UpdateTableOfContent(tableOfContent, listOfFigures, listOfTables, reportID) > 0)
            });
        }

        [ZionAuthorize(Roles = "ListReport")]
        [HttpPost]
        public ActionResult InsertOrUpdateReportData(
          int reportID,
          List<ReportDataColumn> reportData)
        {
            return Json(new
            {
                Success = ReportRepository.CreateOrUpdateReportData(reportID, reportData)
            });
        }

        [ZionAuthorize(Roles = "ListReport")]
        public ActionResult GetReportData(int reportID) => Json(ReportRepository.GetReportData(reportID), JsonRequestBehavior.AllowGet);
    }
}
