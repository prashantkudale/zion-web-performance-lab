using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ZionMarketResearch.Models.Factory
{
    public class ReportFactory
    {
        public static MainPageReportView GetReportByUrl(string url, int state = 0)
        {
            MainPageReportView result = new MainPageReportView();
            switch (state)
            {
                case 0:
                    result = ReportRepository.GetReportByUrl(url);
                    break;
                case 1:
                    result = DiscountReportRepository.GetReportByUrl(url);
                    break;
            }
            return result;
        }
    }

    //public class OrderSummaryFactory
    //{
    //    public static BuynowRepository GetOrderSummary(string url, int state)
    //    {
    //        BuynowRepository buyNow = new BuynowRepository();
    //        switch (state)
    //        {
    //            case 0:
    //                buyNow = BuynowRepository.GetOrderSummary(url);
    //                break;
    //            case 1:
    //                buyNow = DiscountOrderSummaryRepository.GetOrderSummary(url);
    //                break;
    //        }
    //        return buyNow;
    //    }
    //}
}