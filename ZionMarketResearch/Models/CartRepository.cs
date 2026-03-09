using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZionMarketResearch.Models
{
    public class CartRepository
    {
        public int Id { get; set; }
        public DateTime CartCreated { get; set; }
        public decimal? TotalAmount { get; set; }
        //public List<OrderSummary> Items { get; set; }

        //public static void Add(CartRepository cr)
        //{
        //    using (ZionDbEntities db = new ZionDbEntities())
        //    {
        //        int cId = 0;
        //        if (cr.Id == 0)
        //        {
        //            tblcart cart = new tblcart();
        //            cart.CreatedDate = DateTime.Now;
        //            db.tblcarts.Add(cart);
        //            db.SaveChanges();
        //            cId = cart.Id;
        //        }
        //        else
        //        {
        //            //TODO: implement add item into cart if cart id already exist
        //            cId = cr.Id;
        //        }

        //        foreach (OrderSummary item in cr.Items)
        //        {
        //            tblcartitem cartItem = new tblcartitem();
        //            cartItem.CartId = cId;
        //            cartItem.ReportId = item.ReportId;
        //            cartItem.Quantity = item.Quantity;
        //            cartItem.SelectedType = item.SelectedReportType;
        //            db.tblcartitems.Add(cartItem);
        //        }
        //        db.SaveChanges();
        //    }
        //}

        //public static CartRepository GetCartById(int id)
        //{
        //    using (ZionDbEntities db = new ZionDbEntities())
        //    {
        //        var cart = (from c in db.tblcarts
        //                    where c.Id == id && c.IsEnable == true
        //                    select new CartRepository
        //                    {
        //                        Id = c.Id,
        //                        CartCreated = (DateTime)c.CreatedDate,
        //                        Items = (from i in db.tblcartitems
        //                                 where i.CartId == c.Id && i.IsEnable == true
        //                                 select new OrderSummary
        //                                 {
        //                                     Id = i.Id,
        //                                     Quantity = i.Id,
        //                                     ReportId = (int)i.ReportId,
        //                                     TotalAmount = i.ItemAmount,
        //                                     SelectedReportType = (int)i.SelectedType,
        //                                     Report = (from r in db.tblreportinformations
        //                                               where r.ReportID == i.ReportId && r.IsDeleted == false
        //                                               select new CartReport
        //                                               {
        //                                                   ReportId = r.ReportID,
        //                                                   ReportTitle = r.ReportTitle,
        //                                                   ReportUrl = r.ReportUrl,
        //                                                   SingleUser = r.PriceSingleUser,
        //                                                   MultiUser = r.PriceEnterpriseLicense,
        //                                                   CorporateUser = r.PriceCUL
        //                                               }).FirstOrDefault()
        //                                 }).ToList()
        //                    }).FirstOrDefault();
        //        return cart;
        //    }
        //}

        public static void RemoveItem(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var item = db.tblcartitems.Where(i => i.Id == id).FirstOrDefault();
                item.IsEnable = false;
                db.SaveChanges();
            }
        }

        public static void RemoveCart(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var cart = db.tblcarts.Where(c => c.Id == id).FirstOrDefault();
                cart.IsEnable = false;
                db.SaveChanges();
            }
        }

        //public static CartRepository GetCart()
        //{
        //    if (HttpContext.Current.Request.Cookies["zCart"] != null)
        //    {
        //        int cartId = Convert.ToInt32(Util.Utility.Decryptstring(HttpContext.Current.Request.Cookies["zCart"].Value));
        //        return GetCartById(cartId);
        //    }
        //    return null;
        //}
    }

    public class CartReport
    {
        public int ReportId { get; set; }
        public string ReportTitle { get; set; }
        public int SelectedType { get; set; }
        public string ReportUrl { get; set; }
        public decimal? SingleUser { get; set; }
        public decimal? MultiUser { get; set; }
        public decimal? CorporateUser { get; set; }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ReportId { get; set; }
        public int Quantity { get; set; }
        public decimal? ItemAmount { get; set; }
        public int SelectedType { get; set; }
        public CartReport Report { get; set; }
    }
}