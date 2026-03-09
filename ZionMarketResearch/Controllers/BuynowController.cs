//using Newtonsoft.Json;
////using PaymentLibrary.PayPal;
////using PaymentLibrary.TwoCheckout;
//using Stripe;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Net;
//using System.Security.Cryptography;
//using System.Text;
//using System.Web;
//using System.Web.Mvc;
//using ZionMarketResearch.Extension;
//using ZionMarketResearch.Models;
//using ZionMarketResearch.Utility;

//namespace ZionMarketResearch.Controllers
//{
//    public class BuynowController : Controller
//    {
//        //
//        // GET: /Buynow/
//        //TODO: implement cart functionality
//        //public ActionResult Index(string reporttype, string url)
//        //{
//        //    var os = BuynowRepository.GetOrderSummary(url, reporttype, Request.RawUrl);
//        //    //var countryInfo = Util.Utility.GetCountryInfo(Util.Utility.ClientIPAddress);
//        //    ViewBag.Countries = ZionMarketResearch.Models.CountryRepository.GetCountry();
//        //    ViewBag.ReportType = reporttype;
//        //    if (os != null && os.OrderSummary != null && os.OrderSummary[0] != null)
//        //    {
//        //        //os.City = countryInfo.region.city;
//        //        //os.State = countryInfo.region.state;
//        //        //os.ZipCode = countryInfo.region.postal;
//        //        return View("NewBuyNow", os);
//        //    }

//        //    return HttpNotFound();
//        //}

//        //[HttpPost]
//        //public ActionResult StripePayment(StripePayment token)
//        //{
//        //    var report = ReportRepository.GetReportById(token.reportId);
//        //    var price = token.type == "su" ? report.SingleUser : token.type == "mu" ? report.MultiUser : token.type == "cu" ? report.CorporateUser : 0;

//        //    if (price == 0)
//        //        return Json(new { IsSuccess = false, ErrorMessage = "Selected Type not found" });

//        //    try
//        //    {
//        //        var config = PaymentLibrary.Stripe.StripeConfig.GetConfig(HttpContext.Server.MapPath("/config/stripe.config"));
//        //        PaymentLibrary.Stripe.Stripe.IsValidPayment(new StripeChargeCreateOptions
//        //        {
//        //            SourceTokenOrExistingSourceId = token.id,
//        //            Amount = (int)price * 100,
//        //            //Currency = token.card.country == "IN" ? "INR" : "USD",
//        //            Currency = "inr"
//        //        }, config);
//        //    }
//        //    catch (StripeException stripeException)
//        //    {
//        //        return Json(new
//        //        {
//        //            IsSuccess = false,
//        //            ErrorMessage = stripeException.Message
//        //        });
//        //    }
//        //    return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
//        //}

//        //public ActionResult Payment(string reporttype, string url)
//        //{
//        //    var os = BuynowRepository.GetPaymentOrderSummary(url, reporttype, Request.RawUrl);
//        //    if (os.OrderSummary != null && os.OrderSummary[0] != null)
//        //        return View("RazorPayTest", os);
//        //    return HttpNotFound();
//        //}

//        [PaymentErrorHandler(ModelType = typeof(BuynowRepository))]
//        [HttpPost]
//        public ActionResult Index(BuynowRepository buynow)
//        {
//            bool captchaValid = CaptchaRepository.IsCaptchaValid(buynow.Captcha);
//            if (ModelState.IsValid && captchaValid)
//            {
//               // BuynowRepository.SaveBuynowDetails(buynow);
//            }
//            if (!captchaValid)
//                ModelState.AddModelError("Captcha", "Invalid Captcha");

//            ViewBag.Countries = ZionMarketResearch.Models.CountryRepository.GetCountry();
//            return View("Index", BuynowRepository.GetOrderSummary(buynow));
//        }

//        public ActionResult Prebook(string reporttype, string url)
//        {
//            var os = BuynowRepository.GetOrderSummary(url, reporttype, Request.RawUrl);
//            //var countryInfo = Util.Utility.GetCountryInfo(Util.Utility.ClientIPAddress);
//            ViewBag.Countries = ZionMarketResearch.Models.CountryRepository.GetCountry();
//            ViewBag.ReportType = reporttype;
//            if (os != null && os.OrderSummary != null && os.OrderSummary[0] != null)
//                return View("NewBuyNow", os);
//            return HttpNotFound();
//        }

//        [PaymentErrorHandler(ModelType = typeof(BuynowRepository))]
//        [HttpPost]
//        public ActionResult Prebook(BuynowRepository buynow)
//        {
//            if (ModelState.IsValid)
//            {
//               // BuynowRepository.SaveBuynowDetails(buynow);
//            }
//            return View("NewBuyNow", BuynowRepository.GetOrderSummary(buynow));
//        }



//        //[HttpPost]
//        //public ActionResult Submit(BuynowRepository buynow)
//        //{
//        //    if (ModelState.IsValid)
//        //    {
//        //        BuynowRepository.SaveBuynowDetails(buynow);
//        //    }
//        //    return View("Index", BuynowRepository.GetOrderSummary(buynow));
//        //}

//        //public ActionResult PayPalProcess(PayPalResponse paypalResponse)
//        //{
//        //    BuynowRepository.PayPalProcess(paypalResponse);
//        //    return new RedirectResult("/process");
//        //}

//        public ActionResult CCAvenueProcess()
//        {
//            BuynowRepository.CCAvenueProcess(Request.Form["encResp"]);
//            return new RedirectResult("/process");
//        }

//        //public ActionResult TwoCheckoutProcess(TwoCheckoutResponse twoCheckoutResponse)
//        //{
//        //    //TODO: implement two checkout process
//        //    BuynowRepository.TwoCheckoutProcess(twoCheckoutResponse);
//        //    return RedirectToAction("process");
//        //}

//        //public async System.Threading.Tasks.Task<ActionResult> HDFCProcess(PaymentLibrary.HDFC.Response hdfcResponse)
//        //{
//        //    await BuynowRepository.HDFCProcess(hdfcResponse);
//        //    return RedirectToAction("process");
//        //}

//        public ActionResult Process()
//        {
//            if (Session["process"] != null)
//            {
//                ViewBag.Message = Session["process"];
//                Session["process"] = null;
//                return View("process");
//            }
//            return HttpNotFound();
//        }

//        [HttpPost]
//        public ActionResult CalculateHash1(PayUHashRequest data)
//        {
//            var report = ReportRepository.GetReportByUrl(Util.Utility.Decryptstring(data.ReportUrl));
//            var selectedType = ZionMarketResearch.Util.Utility.Decryptstring(data.SelectedType);
//            var price = selectedType == "0" ? report.SingleUser : selectedType == "1" ? report.MultiUser : selectedType == "2" ? report.CorporateUser : 0;
//            var udf5 = "ZionMarketResearch";
//            var transactionID = Guid.NewGuid().ToString();

//            var d = ConfigurationManager.AppSettings["PayUKey"] + "|" + transactionID + "|" + price + "|" + report.ReportTitle + "|" + data.fname + "|" + data.email + "|||||" + udf5 + "||||||" + ConfigurationManager.AppSettings["PayUSalt"];

//            return Json(new { Hash = BuynowRepository.PayUCalculateHash(d), TransactionId = transactionID, Price = price.ToString(), ReportTitle = report.ReportTitle, FirstName = data.fname, Email = data.email, UDF5 = udf5, Phone = data.Phone });
//        }

//        [HttpPost]
//        public string PayuResponse(PayUResponse response)
//        {
//            if (response.status.ToLower() == "success")
//            {
//                return "Sucess";
//            }
//            return "Error " + JsonConvert.SerializeObject(response);
//        }

//        public ActionResult TestPayment(string reporttype, string url)
//        {
//            var os = BuynowRepository.GetOrderSummary(url, reporttype, Request.RawUrl);
//            //var countryInfo = Util.Utility.GetCountryInfo(Util.Utility.ClientIPAddress);
//            ViewBag.Countries = ZionMarketResearch.Models.CountryRepository.GetCountry();
//            if (os != null && os.OrderSummary != null && os.OrderSummary[0] != null)
//            {
//                //os.City = countryInfo.region.city;
//                //os.State = countryInfo.region.state;
//                //os.ZipCode = countryInfo.region.postal;
//                return View("RazorPayTest", os);
//            }

//            return HttpNotFound();
//        }

//        [PaymentErrorHandler(ModelType = typeof(BuynowRepository))]
//        [HttpPost]
//        public ActionResult CreateOrder(BuynowRepository req)
//        {
//            if (ModelState.IsValid)
//            {
//                if (Session["Captcha"].ToString() != req.Captcha)
//                    return Json(new RazorPayCreateOrderResponse { Message = "Wrong Captcha" });

//                var url = Util.Utility.Decryptstring(req.r);
//                MainPageReportView report = null;
//                try
//                {
//                    report = Models.Factory.ReportFactory.GetReportByUrl(url, 0);
//                }
//                catch (Exception ex)
//                {
//                    return Json(new { Message = "Why this is not working " + url });
//                }

//                if (report == null)
//                    return Json(new { Message = "Report Not Found" });

//                var outSelectedType = 1;

//                int selectedType = !Int32.TryParse(req.l, out outSelectedType) ? Convert.ToInt32(Util.Utility.Decryptstring(req.l)) : outSelectedType;
//                decimal reportPrice = (decimal)(selectedType == 0 ? report.SingleUser : selectedType == 1 ? report.MultiUser : selectedType == 2 ? report.CorporateUser : report.SingleUser) * 100;

//                RazorPayCreateOrderResponse res = null;
//                try
//                {
//                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
//                    var client = new Razorpay.Api.RazorpayClient(ConfigurationManager.AppSettings["RazorPayKey"], ConfigurationManager.AppSettings["RazorPaySecret"]);
//                    var values = new Dictionary<string, object>();
//                    var receipt = Guid.NewGuid().ToString().ZSubstring(0, 20);

//                    values.Add("amount", reportPrice);
//                    values.Add("currency", "USD");
//                    values.Add("payment_capture", 1);
//                    values.Add("receipt", receipt);
//                    var order = client.Order.Create(values);
//                    req.PaymentType = 5;
//                    req.PaymentThrough = "RazorPay";
//                    //BuynowRepository.SaveBuynowDetails(req);

//                    return Json(new RazorPayCreateOrderResponse
//                    {
//                        id = order.Attributes.id,
//                        ReportTitle = report.ReportTitle.ZSubstring(0, 200),
//                        Amount = (double)reportPrice,
//                        ClientKey = ConfigurationManager.AppSettings["RazorPayKey"]
//                    });
//                }
//                catch (Exception ex)
//                {
//                    return Json(new { Message = JsonConvert.SerializeObject(ex), Success = false });
//                }
//            }
//            return Json(new { Message = "Something is wrong" });
//        }

//        [HttpPost]
//        public ActionResult RazorPaymentSuccess(string razorpay_order_id, string razorpay_payment_id, string razorpay_signature)
//        {
//            var client = new Razorpay.Api.RazorpayClient(ConfigurationManager.AppSettings["RazorPayKey"], ConfigurationManager.AppSettings["RazorPaySecret"]);
//            var values = new Dictionary<string, string>();

//            values.Add("razorpay_order_id", razorpay_order_id);
//            values.Add("razorpay_payment_id", razorpay_payment_id);
//            values.Add("razorpay_signature", razorpay_signature);

//            Razorpay.Api.Utils.verifyPaymentSignature(values);

//            var o = client.Order.Fetch(razorpay_order_id);

//            if (o.Attributes.status == "paid")
//            {
//                BuynowRepository buyer = BuynowRepository.GetBuyerByGuId(o.Attributes.receipt.ToString());

//                buyer.razorpay_order_id = razorpay_order_id;
//                buyer.razorpay_payment_id = razorpay_payment_id;

//                BuynowRepository._payment(true, buyer);
//                BuynowRepository.SaveStatus(buyer, 's', string.Empty);
//                //TODO: Send Email to User Success
//            }

//            return Json(new { Success = o.Attributes.status == "paid" });
//        }
//    }



//    public class StripePayment
//    {
//        public string id { get; set; }
//        public Card card { get; set; }
//        public int reportId { get; set; }
//        public string type { get; set; }
//    }

//    public class Card
//    {
//        public string address_city { get; set; }
//        public string address_country { get; set; }
//        public string address_line1 { get; set; }
//        public string address_line1_check { get; set; }
//        public string address_line2 { get; set; }
//        public string address_state { get; set; }
//        public string address_zip { get; set; }
//        public string address_zip_check { get; set; }
//        public string brand { get; set; }
//        public string country { get; set; }
//    }

//    public class RazorPayCreateOrderRequest
//    {
//        public string r { get; set; }
//        public string l { get; set; }
//        public string Name { get; set; }
//        public string Email { get; set; }
//        public string Phone { get; set; }
//        public string Captcha { get; set; }
//        public string Company { get; set; }
//    }

//    public class RazorPayCreateOrderResponse
//    {
//        public string id { get; set; }
//        public string ReportTitle { get; set; }
//        public double Amount { get; set; }
//        public string ClientKey { get; set; }
//        public string Message { get; set; }
//    }
//}
