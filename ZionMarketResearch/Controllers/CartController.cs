using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Controllers
{
    public class CartController : Controller
    {
        //
        // GET: /Cart/

        public ActionResult AddToCart(CartRepository cr)
        {
            return View();
        }

        public ActionResult RemoveItem()
        {
            return View();
        }

        public ActionResult GetCart()
        {
            return View();
        }
    }
}
