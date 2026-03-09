using Newtonsoft.Json;
//using PaymentLibrary.Common;
//using PaymentLibrary.HDFC;
//using PaymentLibrary.PayPal;
//using PaymentLibrary.TwoCheckout;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ZionAdmin.Models.LuceneSearch;
using ZionMarketResearch.Extension;
using ZionMarketResearch.Models.Factory;

namespace ZionMarketResearch.Models
{
    public class AddToHTMLTableAttribute : Attribute
    {
        public string Display { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class BuynowRepository
    {
        #region properties
        public int Id { get; set; }

        [AddToHTMLTable(Display = "Name", DisplayOrder = 1)]
        [Required(ErrorMessage = "Name is required."), MaxLength(100, ErrorMessage = "Name should not be more than 100 characters.")]
        public string Name { get; set; }

        [AddToHTMLTable(Display = "Email", DisplayOrder = 2)]
        [Required(ErrorMessage = "Email id is required."), MaxLength(50, ErrorMessage = "Email id should not be more than 50 characters."),
            RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [AddToHTMLTable(Display = "Phone", DisplayOrder = 3)]
        [Required(ErrorMessage = "Phone # is required."), MaxLength(12, ErrorMessage = "Phone # should not be more than 12 characters.")]
        public string ContactNumber { get; set; }

        [AddToHTMLTable(Display = "Company", DisplayOrder = 4)]
        [Required(ErrorMessage = "Company is required."), MaxLength(50, ErrorMessage = "Company name should not be more than 50 characters.")]
        public string Company { get; set; }

        [AddToHTMLTable(Display = "Designation", DisplayOrder = 5)]
        [Required(ErrorMessage = "Designation/Title is required."), MaxLength(20, ErrorMessage = "Designation/Title should not be more than 20 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Address is required."), MaxLength(150, ErrorMessage = "Address should not be more than 150 characters.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "City is required."), MaxLength(20, ErrorMessage = "City should not be more than 20 characters.")]
        public string City { get; set; }
        [Required(ErrorMessage = "State is required."), MaxLength(20, ErrorMessage = "State should not be more than 20 characters.")]
        public string State { get; set; }
        [Required(ErrorMessage = "ZIP is required."), MaxLength(10, ErrorMessage = "ZIP should not be more than 10 characters.")]
        public string ZipCode { get; set; }

        [AddToHTMLTable(Display = "Country", DisplayOrder = 5)]
        [Required(ErrorMessage = "Please select country.")]
        [ZionMarketResearch.Utility.OptionAttribute("Invalid country")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Please select payment type")]
        public int? PaymentType { get; set; }
        [Required(ErrorMessage = "Captcha is required."), MaxLength(5, ErrorMessage = "Captcha should not be more than 5 characters.")]
        public string Captcha { get; set; }

        [AddToHTMLTable(Display = "IP", DisplayOrder = 9)]
        public string IPAddress { get; set; }
        public IList<OrderSummary> OrderSummary { get; set; }
        public string GuId { get; set; }
        public string r { get; set; }//report URL
        public string l { get; set; }//selected report type
        public string TransactionId { get; set; }
        public string ReportTitle { get; set; }

        [AddToHTMLTable(Display = "Price", DisplayOrder = 8)]
        public decimal? Price { get; set; }
        public string PaymentThrough { get; set; }
        public int ReportId { get; set; }
        public string ErrorReason { get; set; }
        public string razorpay_payment_id { get; set; }
        public string razorpay_order_id { get; set; }

        [AddToHTMLTable(Display = "Type", DisplayOrder = 7)]
        public string LabelSelectedType
        {
            get
            {
                var selectedType = Util.Utility.Decryptstring(l);
                return selectedType == "0" ? "Single User" : selectedType == "1" ? "Multi User" : selectedType == "2" ? "Corporate User" : string.Empty;
            }
        }
        #endregion


        #region public methods
        public static BuynowRepository GetOrderSummary(string url, string rawurl = "")
        {
            string[] u = rawurl.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            bool isPrebook = (!string.IsNullOrEmpty(rawurl) ? (u.Count() > 1 && u[0] == "prebook" ? true : false) : false);
            ZionDbEntities db = new ZionDbEntities();
            var report = LuceneSearch.SearchDefault(url.ToLower(), "ReportUrl").FirstOrDefault();
            if (report == null)
            {
                report = (from r in db.tblreportinformations
                       where r.ReportUrl == url
                       select new LuceneReport
                       {
                           ReportId = r.ReportID,
                           ReportUrl = r.ReportUrl,
                           ReportTitle = r.ReportTitle,
                           Category = r.CategoryBreadCrumb
                       }).FirstOrDefault();

                if (report != null)
                {
                    LuceneSearch.AddUpdateLuceneIndex(report);
                }
            }
            if (report == null)
            {
                return null;
            }
            var orderSummary = (from os in db.tblreportinformations
                                where os.ReportID == report.ReportId && os.IsDeleted == false && (!string.IsNullOrEmpty(rawurl) ? (isPrebook ? os.IsUpComing == true : os.IsUpComing == false) : 1 == 1)
                                select new OrderSummary
                                {
                                    ReportId = os.ReportID,
                                    ReportTitle = os.ReportTitle,
                                    ReportUrl = os.ReportUrl,
                                    SingleUser = os.PriceSingleUser,
                                    MultiUser = os.PriceEnterpriseLicense,
                                    CorporateUser = os.PriceCUL,
                                    IsUpcoming = (bool)os.IsUpComing
                                }).SingleOrDefault();
            BuynowRepository br = new BuynowRepository();
            if (orderSummary != null)
            {
                br.OrderSummary = new List<Models.OrderSummary>();
                br.OrderSummary.Add(orderSummary);
                //br.r = Util.Utility.Encryptstring(br.OrderSummary[0].ReportUrl);
                //br.l = Util.Utility.Encryptstring(br.OrderSummary[0].SelectedReportType.ToString());
            }
            return br;
        }

        public static BuynowRepository GetPaymentOrderSummary(string url, string reportType, string rawurl = "")
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var os = (from r in db.tblreportinformations
                          join p in db.tblpaymentlinks on r.ReportID equals p.ReportId
                          where p.PaymentLink == url && p.ValidTo >= DateTime.Now
                          select new OrderSummary
                          {
                              ReportId = r.ReportID,
                              ReportTitle = r.ReportTitle,
                              ReportUrl = "/payment/" + p.PaymentLink,
                              SingleUser = p.SingleUserPrice,
                              MultiUser = p.MultiUserPrice,
                              CorporateUser = p.CorporateUserPrice,
                              IsUpcoming = (bool)r.IsUpComing
                          }).SingleOrDefault();

                BuynowRepository br = new BuynowRepository();
                if (os != null)
                {
                    br.OrderSummary = new List<Models.OrderSummary>();
                    br.OrderSummary.Add(os);
                    br.OrderSummary[0].SelectedReportType = reportType == "su" || reportType == "0" ? 0 : reportType == "mu" || reportType == "1" ? 1 : reportType == "cu" || reportType == "2" ? 2 : 0;
                    br.r = Util.Utility.Encryptstring(br.OrderSummary[0].ReportUrl);
                    br.l = Util.Utility.Encryptstring(br.OrderSummary[0].SelectedReportType.ToString());
                }
                return br;
            }
        }

        public static BuynowRepository GetOrderSummary(BuynowRepository buynow)
        {
            string ru = Util.Utility.Decryptstring(buynow.r);
            string rs = Util.Utility.Decryptstring(buynow.l);
            BuynowRepository bn = null;
            if (!ru.Contains("/payment/"))
                bn = GetOrderSummary(ru);
            else
                bn = GetPaymentOrderSummary(ru.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1], rs);

            if (bn.OrderSummary != null && bn.OrderSummary[0] != null)
                bn.OrderSummary[0].SelectedReportType = Convert.ToInt32(rs);
            return bn;
        }

        public static BuynowRepository GetOrderSummary(string url, string reportyType, string rawurl)
        {
            var os = OrderSummaryFactory.GetOrderSummary(url, HttpContext.Current.Session["Discount"] != null ? 1 : 0);
            if (os != null && os.OrderSummary != null && os.OrderSummary[0] != null)
            {
                os.OrderSummary[0].SelectedReportType = reportyType == "su" ? 0 : reportyType == "mu" ? 1 : reportyType == "cu" ? 2 : 0;
                os.r = Util.Utility.Encryptstring(url);
                os.l = Util.Utility.Encryptstring(os.OrderSummary[0].SelectedReportType.ToString());
            }
            return os;
        }

        //public static Utility.Message SaveBuynowDetails(BuynowRepository buynow)
        //{
        //    //TODO: this is temprory solution if cart functionality going to implement
        //    string ru = Util.Utility.Decryptstring(buynow.r);
        //    string rs = buynow.l;
        //    BuynowRepository br = !ru.Contains("/payment/") ? OrderSummaryFactory.GetOrderSummary(ru, HttpContext.Current.Session["Discount"] != null ? 1 : 0) : GetPaymentOrderSummary(ru.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1], rs);
        //    if (br == null)
        //        return new Utility.Message { MessageText = "Error while adding buyer." };
        //    int selectedType = 0;
        //    br.OrderSummary[0].SelectedReportType = Int32.TryParse(rs, out selectedType) ? (selectedType <= 2 ? selectedType : 0) : 0;
        //    buynow.OrderSummary = br.OrderSummary;
        //    buynow.OrderSummary[0].Quantity = 1;
        //    buynow.ReportTitle = br.OrderSummary[0].ReportTitle;
        //    buynow.ReportId = br.OrderSummary[0].ReportId;
        //    buynow.IPAddress = Util.Utility.ClientIPAddress;
        //    buynow.Price = br.OrderSummary[0].Price;

        //    string guid = Guid.NewGuid().ToString().Substring(0, 20);
        //    buynow.GuId = string.IsNullOrEmpty(buynow.GuId) ? guid : buynow.GuId;
        //    buynow.TransactionId = PaymentLibrary.Common.Security.GetUniqueKey();
        //    if (_SaveBuyer(buynow))//save buyer
        //    {
        //        Utility.Message message = new Utility.Message();

        //        #region Send Mail To User
        //        Dictionary<string, string> tokens = new Dictionary<string, string>();
        //        tokens.Add("[MRS:CustomerName]", buynow.Name);
        //        tokens.Add("[MRS:ReportTitle]", br.OrderSummary[0].ReportTitle);
        //        SendMail.Send(buynow.Email, "Payment Initiated - " + br.OrderSummary[0].ReportTitle, Util.Utility.GetHtml("PaymentInitiate", tokens));
        //        #endregion

        //        var sb = new StringBuilder();
        //        //sb.Append(string.Format("Dear Admin, <br /> Payment for report <b> {0} </b> <br /> " ,
        //        //    br.OrderSummary[0].ReportTitle,
        //        //    buynow.Name,
        //        //    buynow.Email,
        //        //    buynow.ContactNumber,
        //        //    buynow.Company,
        //        //    buynow.Address,
        //        //    buynow.State,
        //        //    buynow.Country,
        //        //    buynow.ZipCode,
        //        //    buynow.IPAddress
        //        // ));

        //        sb.Append(string.Format("Dear Admin, <br /> Payment for report <b> {0} </b> <br /> ",
        //            br.OrderSummary[0].ReportTitle) + GetTable(buynow));

        //        var crmResponse = FormRepository.SubmitCheckoutInfoToPortal(new FormRepository() { ReportUrl = $"/report/{br.OrderSummary[0].ReportUrl}", Address = buynow.Address, City = buynow.City, Country = buynow.Country, Company = buynow.Company, GuId = buynow.GuId, ReportTitle = buynow.ReportTitle, State = buynow.State, Zip = buynow.ZipCode, Phone = buynow.ContactNumber, Name = buynow.Name, Designation = buynow.Title, Email = buynow.Email, ReportId = buynow.ReportId.ToString() }).ConfigureAwait(true);
        //        #region Payment Switch
        //        switch (buynow.PaymentType)
        //        {
        //            case 0:
        //                SendMail.SendMailToZion("Zion Market Research - Payment Initiated (PayPal)", sb.ToString(), "payment");
        //                message = _PayPal(buynow);
        //                break;
        //            case 1:
        //                SendMail.SendMailToZion("Zion Market Research - Payment Initiated (2CCheckout)", sb.ToString(), "payment");
        //                message = _TwoCheckout(buynow);
        //                break;
        //            case 2:
        //                SendMail.SendMailToZion("Zion Market Research - Payment Initiated (WireTransfer)", sb.ToString(), "payment");
        //                WireTransferProcess(buynow);
        //                message = new Utility.Message { MessageText = "Payment Initiated for Wire Transfer" };
        //                //TODO: implement wire transfer
        //                break;
        //            case 3:
        //                SendMail.SendMailToZion("Zion Market Research - Payment Initiated (HDFC)", sb.ToString(), "payment");
        //                message = _HDFC(buynow);
        //                break;
        //            case 4:
        //                SendMail.SendMailToZion("Zion Market Research - Payment Initiated (CCAvenue)", sb.ToString(), "payment");
        //                message = _CCAvenue(buynow);
        //                break;
        //            case 5:
        //                SendMail.SendMailToZion("Zion Market Research - Payment Initiated (RazorPay)", sb.ToString(), "payment");
        //                break;
        //        }
        //        #endregion
        //        return !string.IsNullOrEmpty(message.MessageText) ? message : new Utility.Message { MessageText = "Payment Initiated" };
        //    }
        //    log4net.LogManager.GetLogger("Error").Error("Error while adding buyer.\nFull Data - " + Newtonsoft.Json.JsonConvert.SerializeObject(buynow));
        //    return new Utility.Message { MessageText = "Error while adding buyer." };
        //}

        public static bool PayPalProcess(PayPalResponse paypalResponse)
        {
            if (paypalResponse != null && (!string.IsNullOrEmpty(paypalResponse.PAYERID) || !string.IsNullOrEmpty(paypalResponse.guid)))
            {
                if (string.IsNullOrEmpty(paypalResponse.PAYERID))
                    log4net.LogManager.GetLogger("Error").Error("PayerID not found OR Response is null OR guid is not found.\nData - " + Newtonsoft.Json.JsonConvert.SerializeObject(paypalResponse));

                BuynowRepository buyer = GetBuyerByGuId(paypalResponse.guid);

                if (buyer == null)
                {
                    log4net.LogManager.GetLogger("Error").Error("Buyer not found.\nData - " + Newtonsoft.Json.JsonConvert.SerializeObject(paypalResponse));
                    return false;
                }
                buyer.PaymentThrough = "PayPal";
                ValidResponse vResponse = PayPal.IsPaymentValid(paypalResponse, PayPalConfig.GetConfiguration(HttpContext.Current.Server.MapPath("/config/paypal.config"), false), false);
                if (vResponse.IsValid)
                {
                    _payment(true, buyer);
                    _saveStatus(buyer.Id, 's', string.Empty);
                    return true;
                }
                JsonSerializer serializer = new JsonSerializer();
                Stream s = new MemoryStream();
                TextWriter t = new StreamWriter(s);

                serializer.Serialize(t, paypalResponse);
                TextReader r = new StreamReader(s);

                _saveStatus(buyer.Id, 'f', "Reason -" + (vResponse.Reason != null ? vResponse.Reason : string.Empty) + "|ErrorCode - " + (vResponse.ErrorCode != null ? vResponse.ErrorCode : string.Empty) + "|PaypalResponse - " + (r != null ? r.ReadToEnd() : string.Empty));
                buyer.ErrorReason = "Reason -" + (vResponse.Reason != null ? vResponse.Reason : string.Empty) + "|ErrorCode - " + (vResponse.ErrorCode != null ? vResponse.ErrorCode : string.Empty);
                _payment(false, buyer);
                return false;
            }
            return false;
        }


        public static bool TwoCheckoutProcess(TwoCheckoutResponse twoCheckoutResponse)
        {
            ValidResponse vResponse = TwoCheckout.IsPaymentValid(twoCheckoutResponse, TwoCheckoutConfig.GetConfiguration(HttpContext.Current.Server.MapPath("/config/twocheckout.config"), false), false);
            BuynowRepository br = GetBuyerByGuId(twoCheckoutResponse.guid);
            if (br == null)
                return false;
            if (vResponse.IsValid)
            {
                _payment(true, br);
                _saveStatus(br.Id, 's', string.Empty);
                return true;
            }
            _saveStatus(br.Id, 'f', "Reason -" + (vResponse.Reason != null ? vResponse.Reason : string.Empty) + "|ErrorCode - " + (vResponse.ErrorCode != null ? vResponse.ErrorCode : string.Empty));
            br.ErrorReason = "Reason -" + (vResponse.Reason != null ? vResponse.Reason : string.Empty) + "|ErrorCode - " + (vResponse.ErrorCode != null ? vResponse.ErrorCode : string.Empty);
            _payment(false, br);
            return vResponse.IsValid;
        }

        //public static async System.Threading.Tasks.Task<bool> HDFCProcess(Response hdfcResponse)
        //{
        //    BuynowRepository br = GetBuyerByGuId(hdfcResponse.udf1);
        //    if (br == null)
        //        return false;

        //    string currentCountry = PaymentLibrary.Common.Util.GetCurrentCountry(br.IPAddress);

        //    ValidResponse vResponse = currentCountry == "IN" ?
        //        hdfc.IsValidPayment(hdfcResponse, HttpContext.Current.Server.MapPath("/config/hdfc_inr.config"), false) :
        //        hdfc.IsValidPayment(hdfcResponse, HttpContext.Current.Server.MapPath("/config/hdfc_usd.config"), false);

        //    if (vResponse.IsValid)
        //    {
        //        var res = await PaymentLibrary.HDFC.hdfc.SaveFraud(new BuyerInfo
        //        {
        //            CustomerName = br.Name,
        //            Address = br.Address,
        //            City = br.City,
        //            Company = br.Company,
        //            Country = currentCountry,
        //            Email = br.Email,
        //            IPAddress = br.IPAddress,
        //            Phone = br.ContactNumber,
        //            ReportTitle = br.ReportTitle,
        //            //ReportUrl = br.OrderSummary[0].ReportUrl,
        //            ReportPrice = (decimal)br.Price,
        //            ZipCode = br.ZipCode,
        //            State = br.State,
        //            Domain = "ZionMarketResearch",
        //            TransactionId = br.GuId
        //        });

        //        br.ErrorReason = (!string.IsNullOrEmpty(vResponse.ErrorCode) ? vResponse.ErrorCode : string.Empty) + " | " + (!string.IsNullOrEmpty(vResponse.Reason) ? vResponse.Reason : string.Empty);
        //        _payment(true, br);
        //        _saveStatus(br.Id, 's', string.Empty);
        //        return true;
        //    }
        //    _saveStatus(br.Id, 'f', vResponse.Reason + "|Error Code - " + vResponse.ErrorCode);
        //    _payment(false, br);
        //    return false;
        //}
        //TODO: Test this
        public static void WireTransferProcess(BuynowRepository br)
        {
            var buyer = GetBuyerByGuId(br.GuId);
            _payment(true, buyer, true);
            _saveStatus(buyer.Id, 's', string.Empty);
            HttpContext.Current.Response.Redirect("/process");
        }

        public static BuynowRepository GetBuyerByGuId(string guId)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var buynow = (from b in db.tblbuyerinformations
                              where b.GuId == guId
                              select new BuynowRepository
                              {
                                  Id = b.Id,
                                  Name = b.BuyerName,
                                  Email = b.Email,
                                  ReportTitle = b.ReportTitle,
                                  PaymentThrough = b.PaymentThrough,
                                  Price = b.Price,
                                  ContactNumber = b.ContactNumber,
                                  Address = b.BuyerAddress,
                                  IPAddress = b.IPAddress,
                                  City = b.City,
                                  Company = b.CompanyName,
                                  Country = b.Country,
                                  State = b.State,
                                  ZipCode = b.ZipCode,
                                  GuId = b.GuId
                              }).SingleOrDefault();
                return buynow;
            }
        }

        public static bool CCAvenueProcess(string ccAvenueResponse)
        {
            var vResponse = PaymentLibrary.CCAvenue.CCAvenue.IsValidPayment(ccAvenueResponse, HttpContext.Current.Server.MapPath("/config/ccavenue.config"));
            var _ccAvenueResponse = PaymentLibrary.CCAvenue.CCAvenue.GetCCAvenueResponse(ccAvenueResponse, HttpContext.Current.Server.MapPath("/config/ccavenue.config"));
            BuynowRepository br = GetBuyerByGuId(_ccAvenueResponse.merchant_param1);
            if (br == null)
                return false;
            br.PaymentThrough = "CCAvenue";
            if (vResponse.IsValid)
            {
                _payment(true, br);
                _saveStatus(br.Id, 's', _ccAvenueResponse.order_id);
                return true;
            }

            _saveStatus(br.Id, 'f', vResponse.Reason + "|Error Code - " + vResponse.ErrorCode);
            _payment(false, br);
            return vResponse.IsValid;
        }
        #endregion



        #region Private Members
        static Utility.Message _PayPal(BuynowRepository buynow)
        {
            #region Paypal
            PayPalConfig config = PayPalConfig.GetConfiguration(HttpContext.Current.Server.MapPath("/config/paypal.config"));
            config.guid = buynow.GuId;
            List<PaymentLibrary.PayPal.Item> items = new List<PaymentLibrary.PayPal.Item>();
            foreach (OrderSummary orderSummary in buynow.OrderSummary)
            {
                //if price is not set or null then do not add to order summary
                if (orderSummary.Price == null)
                    continue;

                PaymentLibrary.PayPal.Item itm = new PaymentLibrary.PayPal.Item();
                itm.Name = orderSummary.ReportTitle.ZSubstring(0, 20);
                itm.Quantity = orderSummary.Quantity;
                itm.Price = Math.Round(orderSummary.Price.Value, 2);
                items.Add(itm);
            }
            PaymentLibrary.PayPal.Token tkn = PayPal.GetToken(config, items);

            if (!string.IsNullOrEmpty(tkn.L_ERRORCODE0))
            {
                log4net.LogManager.GetLogger("Error").Error("Error at SaveBuynowDetails. PayPal \nErrorCode - " + tkn.L_ERRORCODE0 + "\nError Message - " + tkn.L_SHORTMESSAGE0 + "\n" + tkn.L_LONGMESSAGE0 + "\nFull Data - " + Newtonsoft.Json.JsonConvert.SerializeObject(buynow));
                return new Utility.Message { MessageText = tkn.L_LONGMESSAGE0 };
            }

            PayPal.RedirectUser(tkn);

            return new Utility.Message { MessageText = "Payment Initiated" };
            #endregion
        }




        static Utility.Message _TwoCheckout(BuynowRepository buynow)
        {

            #region 2Checkout
            TwoCheckoutRequest twoCheckout = new TwoCheckoutRequest();
            twoCheckout.card_holder_name = buynow.Name;
            twoCheckout.city = buynow.City;
            twoCheckout.state = buynow.State;
            twoCheckout.zip = buynow.ZipCode;
            twoCheckout.phone = buynow.ContactNumber;
            twoCheckout.street_address = buynow.Address;
            twoCheckout.Items = new List<PaymentLibrary.TwoCheckout.Item>();
            twoCheckout.email = buynow.Email;
            twoCheckout.country = buynow.Country.Contains("|") ? buynow.Country.Split('|')[0] : buynow.Country;
            twoCheckout.guid = buynow.GuId;
            twoCheckout.currency_code = "USD";
            //twoCheckout.x_receipt_link_url = "https://www.zionmarketresearch.com/redirect?url=https://www.zionmarketresearch.com/TwoCheckoutProcess";

            twoCheckout.x_receipt_link_url = "https://www.zionmarketresearch.com/TwoCheckoutProcess";

            foreach (OrderSummary os in buynow.OrderSummary)
            {
                PaymentLibrary.TwoCheckout.Item item = new PaymentLibrary.TwoCheckout.Item();
                item.ProductName = os.ReportTitle;
                item.Description = os.ReportTitle;
                item.Quantity = os.Quantity;
                item.Price = os.Price;
                item.Tangible = "N";
                item.ProductId = "ZMR-" + os.ReportId;
                item.Type = "product";
                twoCheckout.Items.Add(item);
            }
            TwoCheckout.RedirectUser(twoCheckout, TwoCheckoutConfig.GetConfiguration(HttpContext.Current.Server.MapPath("/config/twocheckout.config"), false), false);
            return new Utility.Message { MessageText = "Payment Initiated" };
            #endregion
        }

        static Utility.Message _HDFC(BuynowRepository buynow)
        {
            string[] name = buynow.Name.Split(' ');
            Request req = new Request();
            req.firstname = name.Length > 0 ? name[0] : buynow.Name;
            req.lastname = name.Length > 1 ? name[1] : string.Empty;
            req.address1 = PaymentLibrary.Common.Security.RemoveSpecialCharacters(buynow.Address.ZSubstring(0, 100));
            req.city = buynow.City;
            req.country = buynow.Country;
            req.email = buynow.Email;
            req.phone = buynow.ContactNumber;
            req.productinfo = PaymentLibrary.Common.Security.RemoveSpecialCharacters(buynow.OrderSummary[0].ReportTitle.ZSubstring(0, 20));
            req.state = buynow.State;
            req.txnid = buynow.TransactionId;
            req.zipcode = buynow.ZipCode;
            req.udf1 = buynow.GuId;

            #region Currency Conversion
            string currentCountry = Util.Utility.GetCurrentCountry(buynow.IPAddress);
            if (currentCountry == "IN")
            {
                req.amount = (float)buynow.OrderSummary[0].Price * Util.Utility.CurrentRate();
            }
            else
            {
                req.amount = (float)buynow.OrderSummary[0].Price;
            }
            #endregion

            hdfc.RedirectUser(req, hdfcConfig.GetConfiguration(HttpContext.Current.Server.MapPath(currentCountry == "IN" ? "/config/hdfc_inr.config" : "/config/hdfc_usd.config"), false), false);
            return new Utility.Message { MessageText = "Error in HDFC payment." };
        }

        public static string PayUCalculateHash(string data)
        {
            return GetHash(data);
        }


        private static string GetHash(string data)
        {
            byte[] hash;
            var datab = Encoding.UTF8.GetBytes(data);
            using (SHA512 shaM = new SHA512Managed())
            {
                hash = shaM.ComputeHash(datab);
            }
            return GetStringFromHash(hash);
        }


        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2").ToLower());
            }
            return result.ToString();
        }


        public static void PayUResponse(PayUResponse response)
        {

        }

        static bool _SaveBuyer(BuynowRepository buynow)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var buyer = new tblbuyerinformation
                {
                    BuyerName = buynow.Name,
                    Email = buynow.Email,
                    Designation = buynow.Title,
                    ContactNumber = buynow.ContactNumber,
                    CompanyName = buynow.Company,
                    BuyerAddress = buynow.Address,
                    Country = buynow.Country,
                    State = buynow.State,
                    City = buynow.City,
                    ZipCode = buynow.ZipCode,
                    IPAddress = Util.Utility.ClientIPAddress,
                    GuId = buynow.GuId,
                    ReportTitle = buynow.OrderSummary[0].ReportTitle,
                    PaymentThrough = buynow.PaymentType == 0 ? "paypal" : buynow.PaymentType == 1 ? "2Checkout" : buynow.PaymentType == 3 ? "HDFC" : buynow.PaymentType == 5 ? "RazorPay" : "WireTransfer",
                    Price = buynow.OrderSummary[0].Price,
                    TransactionId = buynow.TransactionId,

                    razorpay_order_id = buynow.razorpay_order_id,
                    razorpay_payment_id = buynow.razorpay_payment_id
                };

                db.tblbuyerinformations.Add(buyer);
               int i= db.SaveChanges();
               
                return  i > 0?true : false;
            }
        }

        public static void _payment(bool isConfirm, BuynowRepository br, bool isWireTransfer = false)
        {
            Dictionary<string, string> tokens = new Dictionary<string, string>();
            tokens.Add("[MRS:CustomerName]", br.Name);
            tokens.Add("[MRS:ReportTitle]", br.ReportTitle);
            if (isConfirm && !isWireTransfer)
            {
                var html = Util.Utility.GetHtml("PaymentConfirm", tokens);
                HttpContext.Current.Session["process"] = html;
                SendMail.Send(br.Email, "Zion Market Research Payment Confirm (" + br.PaymentThrough + ") - " + br.ReportTitle, html);
                SendMail.SendMailToZion("Zion Market Research Payment Confirm (" + br.PaymentThrough + ") - " + br.ReportTitle, "Dear Admin, Payment made for report <b>" + br.ReportTitle + "</b> <table><tr><td><b>Client Name</b></td><td>" + br.Name + "</td></tr><tr><td><b>Email</b></td><td>" + br.Email + "</td></tr><tr><td><b>Phone</b></td><td>" + br.ContactNumber + "</td></tr><tr><td><b>Address</b></td><td>" + br.Address + "</td></tr><tr><td><b>Ip Address</b></td><td>" + br.IPAddress + "</td><td>Price</td><td>" + br.Price + "</td></tr></table>", "payment");
            }
            else if (isConfirm && isWireTransfer)
            {
                var html = Util.Utility.GetHtml("PaymentInitiate", tokens);
                HttpContext.Current.Session["process"] = html;
            }
            else
            {
                var html = Util.Utility.GetHtml("PaymentFailed", tokens);
                HttpContext.Current.Session["process"] = html;
                SendMail.Send(br.Email, "Zion Market Research Payment Failed (" + br.PaymentThrough + ") - " + br.ReportTitle, html);
                SendMail.SendMailToZion("Zion Market Research Payment Failed (" + br.PaymentThrough + ") - " + br.ReportTitle, "Dear Admin,<br/>Payment Failed for <b>" + br.ReportTitle + "</b> <table><tr><td><b>Client Name</b></td><td>" + br.Name + "</td></tr><tr><td><b>Email</b></td><td>" + br.Email + "</td></tr><tr><td><b>Phone</b></td><td>" + br.ContactNumber + "</td></tr><tr><td><b>Address</b></td><td>" + br.Address + "</td></tr><tr><td><b>Ip Address</b></td><td>" + br.IPAddress + "</td><td>Price</td><td>" + br.Price + "</td></tr><tr><td>Error-</td><td>" + br.ErrorReason + "</td></tr></table>", "payment");
            }
        }

        public static void _saveStatus(int id, char transactionStatus, string reason)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var x = db.tblbuyerinformations.Where(z => z.Id == id).SingleOrDefault();
                x.TransactionStatus = transactionStatus.ToString();
                x.Reason = reason;
                db.SaveChanges();
            }
        }

        public static void SaveStatus(BuynowRepository buyer, char transactionStatus, string reason)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var x = db.tblbuyerinformations.FirstOrDefault(z => z.Id == buyer.Id);
                x.TransactionStatus = transactionStatus.ToString();

                x.razorpay_order_id = buyer.razorpay_order_id;
                x.razorpay_payment_id = buyer.razorpay_payment_id;

                x.Reason = reason;
                db.SaveChanges();
            }
        }

        static Utility.Message _CCAvenue(BuynowRepository buynow)
        {
            PaymentLibrary.CCAvenue.CCAvenueRequest ccAvenueReq = new PaymentLibrary.CCAvenue.CCAvenueRequest();
            string guid = DateTime.Now.Ticks.ToString().ZSubstring(0, 10);//CCAvenue has different GUID
            //Currenly CCAvenue is India based thats why just convert USD to INR
            #region Convert Currency
            float rate = Util.Utility.CurrentRate();
            ccAvenueReq.amount = (decimal)((float)buynow.OrderSummary[0].Price * rate);
            #endregion

            ccAvenueReq.billing_address = buynow.Address.ZSubstring(0, 149);
            ccAvenueReq.billing_city = buynow.City.ZSubstring(0, 29);
            ccAvenueReq.billing_country = buynow.Country;
            ccAvenueReq.billing_email = buynow.Email.ZSubstring(0, 69);
            ccAvenueReq.billing_name = buynow.Name.ZSubstring(0, 59);
            ccAvenueReq.billing_state = buynow.State.ZSubstring(0, 29);
            ccAvenueReq.billing_tel = buynow.ContactNumber.ZSubstring(0, 19);
            ccAvenueReq.billing_zip = buynow.ZipCode.ZSubstring(0, 14);
            ccAvenueReq.merchant_param1 = buynow.GuId;
            ccAvenueReq.order_id = guid;
            ccAvenueReq.tid = guid;
            PaymentLibrary.CCAvenue.CCAvenue.RedirectUser(ccAvenueReq, HttpContext.Current.Server.MapPath("/config/ccavenue.config"));
            return new Utility.Message { MessageText = "Payment Initiated" };
        }
        #endregion

        public static string GetTable(BuynowRepository buyerInfo)
        {
            var properties = buyerInfo.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(AddToHTMLTableAttribute), true).Count() > 0);

            properties = properties.OrderBy(x => ((AddToHTMLTableAttribute)x.GetCustomAttributes(typeof(AddToHTMLTableAttribute), true).FirstOrDefault()).DisplayOrder);

            var sb = new StringBuilder();
            sb.Append("<table>");
            foreach (var prop in properties)
            {
                var value = prop.GetValue(buyerInfo);
                if (value == null)
                    continue;
                var displayText = ((AddToHTMLTableAttribute)prop.GetCustomAttributes(typeof(AddToHTMLTableAttribute), true).FirstOrDefault()).Display;
                sb.Append($"<tr><td>{displayText}</td><td>{value}</td></tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }
    }

    public class OrderSummary
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string ReportTitle { get; set; }
        public string ReportUrl { get; set; }
        public decimal? SingleUser { get; set; }
        public decimal? MultiUser { get; set; }
        public decimal? CorporateUser { get; set; }
        public int SelectedReportType { get; set; }
        public int Quantity { get; set; }
        public decimal? Price
        {
            get
            {
                return SelectedReportType == 0 ? SingleUser : SelectedReportType == 1 ? MultiUser : SelectedReportType == 2 ? CorporateUser : null;
            }
        }
        public decimal? TotalAmount { get; set; }
        public CartReport Report { get; set; }
        public bool IsUpcoming { get; set; }

    }

    public class PayUHashRequest
    {

        public string fname { get; set; }
        public string email { get; set; }
        public string Phone { get; set; }
        public string ReportUrl { get; set; }
        public string SelectedType { get; set; }
    }

    public class PayUResponse
    {
        public string txnid { get; set; }
        public string amount { get; set; }
        public string productinfo { get; set; }
        public string firstname { get; set; }
        public string email { get; set; }
        public string udf5 { get; set; }
        public string mihpayid { get; set; }
        public string status { get; set; }
        public string hash { get; set; }
    }
}