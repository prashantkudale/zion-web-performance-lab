using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using ZionAdmin.Models.LuceneSearch;

namespace ZionMarketResearch.Models
{
    public class DiscountReportRepository
    {
        public static MainPageReportView GetReportByUrl(string url)
        {
            HttpContext.Current.Session["Discount"] = "discount";
            //string[] surl = url.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            //todo: implement lucene
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var report = ReportRepository.GetReportByUrl(url);
                if (report == null)
                    return null;

                if (report != null)
                {
                    decimal discountPercentage = (Convert.ToDecimal(ConfigurationManager.AppSettings["DiscountPercentage"]) / 100);
                    report.DiscountSingleUser = report.SingleUser - (report.SingleUser * discountPercentage);
                    report.DiscountMultiUser = report.MultiUser != null ? report.MultiUser - (report.MultiUser * discountPercentage) : null;
                    report.DiscountCorporateUser = report.CorporateUser != null ? report.CorporateUser - (report.CorporateUser * discountPercentage) : null;
                }

                return report;
            }
        }
    }
}