using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ZionMarketResearch.Models
{
    public class DiscountOrderSummaryRepository
    {
        //public static BuynowRepository GetOrderSummary(string url)
        //{
        //    var buyNow = BuynowRepository.GetOrderSummary(url);
        //    if (buyNow == null)
        //        return null;
        //    buyNow.OrderSummary[0].SingleUser = Math.Round((decimal)(buyNow.OrderSummary[0].SingleUser - buyNow.OrderSummary[0].SingleUser * (Convert.ToDecimal(ConfigurationManager.AppSettings["DiscountPercentage"]) / 100)), 2);
        //    buyNow.OrderSummary[0].MultiUser = buyNow.OrderSummary[0].MultiUser != null ? Math.Round((decimal)(buyNow.OrderSummary[0].MultiUser - buyNow.OrderSummary[0].MultiUser * (Convert.ToDecimal(ConfigurationManager.AppSettings["DiscountPercentage"]) / 100)), 2) : 0;
        //    buyNow.OrderSummary[0].CorporateUser = buyNow.OrderSummary[0].CorporateUser != null ? Math.Round((decimal)(buyNow.OrderSummary[0].CorporateUser - buyNow.OrderSummary[0].CorporateUser * (Convert.ToDecimal(ConfigurationManager.AppSettings["DiscountPercentage"]) / 100)), 2) : 0;
        //    return buyNow;
        //}
    }
}