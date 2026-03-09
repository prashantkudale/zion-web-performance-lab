using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using ZionMarketResearch.constants;

namespace ZionMarketResearch.Models
{
    public class FormRepository
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Name is required."), StringLength(30, ErrorMessage = "Name should not be greater than 30 characters.")]
        [LeadTable(DisplayText = "Name", Order = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required."), RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Invalid E-mail."), StringLength(100, ErrorMessage = "Email should not greater than 100 characters.")]
        [LeadTable(DisplayText = "Email", Order = 3)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone # is required."), StringLength(15, ErrorMessage = "Phone # should not be greater than 15 characters.")]
        [LeadTable(DisplayText = "Phone", Order = 4)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Company is required."), StringLength(200, ErrorMessage = "Company name should not be greater than 200 characters.")]
        [LeadTable(DisplayText = "Company", Order = 5)]
        public string Company { get; set; }

        [Required(ErrorMessage = "Designation/Title is required."), StringLength(100, ErrorMessage = "Designation/Title should not be greater than 100 characters.")]
        [LeadTable(DisplayText = "Designation", Order = 6)]
        public string Designation { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [ZionMarketResearch.Utility.OptionAttribute("Invalid country")]
        [LeadTable(DisplayText = "Country", Order = 7)]
        public string Country { get; set; }

        [Required(ErrorMessage = "CAPTCHA is required."), StringLength(4, ErrorMessage = "Captcha should not be greater than 4 characters.")]
        public string Captcha { get; set; }

        [Required(ErrorMessage = "Comment is required."), StringLength(300, ErrorMessage = "Too large comment.")]
        [RegularExpression(@"^[a-zA-Z0-9,.?' ']+$", ErrorMessage = "No special characters allowed.")]
        [LeadTable(DisplayText = "Comment", Order = 8)]
        public string Comment { get; set; }

        [LeadTable(DisplayText = "Report Title", Order = 1)]
        public string ReportTitle { get; set; }

        public int FormType { get; set; }
        public string ReportId { get; set; }
        public bool AutoFill { get; set; }

        [LeadTable(DisplayText = "Address", Order = 9)]
        public string Address { get; set; }

        [LeadTable(DisplayText = "City", Order = 10)]
        public string City { get; set; }

        [LeadTable(DisplayText = "State", Order = 11)]
        public string State { get; set; }

        [LeadTable(DisplayText = "ZIP", Order = 12)]
        public string Zip { get; set; }

        public string ReportUrl { get; set; }

        public string GuId { get; set; }

        [LeadTable(DisplayText = "Search Query", Order = 13)]
        public string SearchText { get; set; }

        [LeadTable(DisplayText = "IP Address", Order = 14)]
        public string IP { get; set; }

        string _encryptstring = string.Empty;
        bool _isDescrypted = false;
        public string ExtraData { get; set; }
        /// <summary>
        /// Property used for sending Category to CRM
        /// </summary>
        public string CRMCategory { get; set; }
        public int CrmLeadID { get; set; }

        [LeadTable(DisplayText = "Is COVID 19 Analysis Required", Order = 14)]
        public bool COVID19Analysis { get; set; }

        public string ru
        {
            get
            {
                if (!_isDescrypted)
                {
                    _isDescrypted = true;
                    _encryptstring = Util.Utility.Decryptstring(ru);
                }
                return _encryptstring;
            }
            set
            {
                _encryptstring = value;
            }
        }
        /// <summary>
        /// Save submitted data to table
        /// </summary>
        /// <param name="fr">FormRepository object</param>
        /// <returns>int number of rows affected</returns>

        #region Submit old logic
        //public static int Submit(FormRepository fr)
        //{
        //    //fr.FormType <= 2 for ContactUs Form
        //    Int32.TryParse(fr.ReportId, out int ri);
        //    if (ri == 0)
        //    {
        //        if (fr.FormType != (int)GlobalConstant.FormType.ContactUs
        //            && fr.FormType != (int)GlobalConstant.FormType.Search
        //            && fr.FormType != (int)GlobalConstant.FormType.NotFound
        //            && !Int32.TryParse(Util.Utility.Decryptstring(fr.ReportId), out ri))
        //            return 0;
        //    }

        //    //var countryInfo = Util.Utility.ClientIPAddress != null && !string.IsNullOrEmpty(Util.Utility.ClientIPAddress) ? Util.Utility.GetCountryInfo(Util.Utility.ClientIPAddress) : null;

        //    using (ZionDbEntities db = new ZionDbEntities())
        //    {
        //        //var report = db.tblreportinformations.Where(r => r.ReportID == ri).SingleOrDefault();
        //        var report = ri > 0 ? ReportRepository.GetReportById(ri) : null;

        //        //fr.FormType <= 2 for ContactUs Form
        //        if (report == null && fr.FormType != 5
        //            && fr.FormType != 6
        //            && fr.FormType != (int)GlobalConstant.FormType.YearEndSale
        //            && fr.FormType != (int)GlobalConstant.FormType.NotFound
        //            && fr.FormType != (int)GlobalConstant.FormType.BlackFriday)
        //            return 0;

        //        if (report != null)
        //        {
        //            fr.CRMCategory = report != null ? report.CRMCategory : string.Empty;
        //            fr.ReportTitle = report.ReportTitle;
        //            fr.ReportUrl = report.ReportUrl;
        //        }
        //        fr.IP = Util.Utility.ClientIPAddress;

        //        var isFreeAnalysisNull = report != null && string.IsNullOrEmpty(report.FreeAnalysis) && !(bool)report.IsUpcoming;

        //        fr.Comment = fr.Comment.Replace("http://", "").Replace("https://", "");

        //        #region SAve Inquiry to DB
        //        var inquiry = new tblinquiry
        //        {
        //            Name = fr.Name,
        //            CompanyName = fr.Company,
        //            EmailId = fr.Email,
        //            PhoneNumber = fr.Phone,
        //            DateOfInquiry = DateTime.Now,
        //            Type = Enum.GetName(typeof(GlobalConstant.FormType), fr.FormType),
        //            IpAddress = Util.Utility.ClientIPAddress,
        //            Message = fr.Comment,
        //            ReportId = ri.ToString(),
        //            Designation = fr.Designation,
        //            Country = fr.Country //countryInfo != null && countryInfo.country != null ? countryInfo.country.name : fr.Country;
        //        };

        //        //[MS]: 20230126 : comment this code : from saving lead info into fnf db
        //        db.tblinquiries.Add(inquiry);
        //        db.SaveChanges();
        //        #endregion

        //        if (fr.AutoFill)
        //        {
        //            //save form details in cookies for next time//
        //            var cookie_personDetails = new HttpCookie("PersonDetails")
        //            {
        //                Expires = DateTime.Now.Date.AddDays(30),
        //                Value = Newtonsoft.Json.JsonConvert.SerializeObject(fr),
        //                Secure = true,
        //                HttpOnly = true
        //            };
        //            HttpContext.Current.Response.Cookies.Add(cookie_personDetails);
        //        }

        //        var tokens = new Dictionary<string, string>();
        //        var html = string.Empty;
        //        switch (fr.FormType)
        //        {
        //            case (int)GlobalConstant.FormType.PopupSample:
        //            case (int)GlobalConstant.FormType.RequestSample:
        //                {
        //                    tokens.Add("[MRS:CustomerName]", fr.Name);
        //                    tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
        //                    html = Util.Utility.GetHtml("RequestSample", tokens);
        //                    SendMail.Send(fr.Email, "Zion Market Research : " + report.ReportTitle, html);
        //                    SendMail.SendMailToZion($"Zion Market Research : Request Sample {(fr.FormType == (int)GlobalConstant.FormType.PopupSample ? "(PopUp)" : string.Empty)}", string.Format("Dear Admin, </br> sample request/enquiry for report<br/><br/> " + "<table> " +
        //                        "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
        //                        "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
        //                        "<tr><td><b> Inquiry Type - </td>             <td>"+ inquiry.Type.ToLower() + "</td></tr>" +
        //                        "<tr><td><b> Report Title - </b></td>   <td>{0}</td></tr>" +
        //                        "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +
        //                        "<tr><td><b> Customer Name - </b></td>  <td>{1}</td></tr>" +
        //                        "<tr><td><b> Email Id -</b> </td>       <td>{2}</td></tr>" +
        //                        "<tr><td><b> Phone -</b></td>           <td>{3}</td></tr>" +
        //                        "<tr><td><b> Name of company -</b></td> <td>{4}</td></tr>" +
        //                        "<tr><td><b> Designation -</b></td>      <td>{5}</td></tr>" +
        //                        "<tr><td><b> Country Name -</b></td>    <td>{6}</td></tr>" +
        //                        "<tr><td><b> IP Address -</b> </td>     <td>{7}</td></tr>" +
        //                        "<tr><td><b> Message - </td>            <td>{8}</td></tr>" +
        //                        "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
        //                    break;
        //                }
        //            case (int)GlobalConstant.FormType.BuyingEnquiry:
        //                {
        //                    tokens.Add("[MRS:CustomerName]", fr.Name);
        //                    tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
        //                    html = Util.Utility.GetHtml("BuyingEnquiry", tokens);
        //                    SendMail.Send(fr.Email, "Zion Market Research : " + report.ReportTitle, html);
        //                    SendMail.SendMailToZion("Zion Market Research : Buying Inquiry ", string.Format("Dear Admin, </br> Inquiry before buying for report <br/> <table> " +
        //                        "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
        //                        "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
        //                        "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
        //                        "<tr><td> <b> Report Title - </b> </td> <td> {0} </td></tr>" +
        //                        "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +
        //                        "<tr><td> <b> Customer Name -</b></td>  <td> {1} </td></tr>" +
        //                        "<tr><td><b> Email Id -</b> </td>       <td>{2}</td></tr>" +
        //                        "<tr><td><b> Phone -</b></td>           <td> {3} </td></tr>" +
        //                        "<tr><td><b> Name of company -</b></td> <td> {4} </td></tr>" +
        //                        "<tr><td><b> Designation -</b></td>      <td>{5}</td></tr>" +
        //                        "<tr><td><b> Country Name -</b></td>    <td>{6} </td></tr>" +
        //                        "<tr><td><b> IP Address -</b> </td>     <td>{7}</td></tr>" +
        //                        "<tr><td><b> Message </b></td>          <td>{8}</td>" +
        //                        "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
        //                    break;
        //                }
        //            case (int)GlobalConstant.FormType.CustomRequest:
        //                {
        //                    tokens.Add("[MRS:CustomerName]", fr.Name);
        //                    tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
        //                    html = Util.Utility.GetHtml("RequestSample", tokens);
        //                    var emailSubject = fr.COVID19Analysis ? "COVID-19 Impact Analysis" : "Request Customization";
        //                    SendMail.Send(fr.Email, $"Zion Market Research : {emailSubject}", html);
        //                    SendMail.SendMailToZion($"Zion Market Research : {emailSubject}", string.Format("Dear Admin, </br> Request customization for report <br/> <table> " +
        //                        "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
        //                        "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
        //                        "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
        //                        "<tr><td> <b> Report Title - </b> </td><td> {0} </td></tr>" +
        //                        "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +
        //                        "<tr><td> <b> Customer Name - </b> </td><td> {1} </td></tr>" +
        //                        "<tr><td><b> Email Id -</b> </td><td>{2}</td></tr>" +
        //                        "<tr><td><b> Phone -</b></td><td> {3} </td></tr>" +
        //                        "<tr><td><b> Name of company -</b></td><td> {4} </td></tr>" +
        //                        "<tr><td><b>Designation -</b></td><td>{5}</td></tr>" +
        //                        "<tr><td><b> Country Name -</b></td><td>{6} </td></tr>" +
        //                        "<tr><td><b> IP Address -</b> </td><td>{7}</td></tr>" +
        //                        "<tr><td><b>Message -</b></td><td>{8}</td></tr>"
        //                        + (fr.COVID19Analysis ? "<tr><td><b>Covid 19 Impact Analysis</b></td><td>Yes</td></tr>" : string.Empty)
        //                        + "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
        //                    break;
        //                }
        //            case (int)GlobalConstant.FormType.RequestTOC:
        //                {
        //                    tokens.Add("[MRS:CustomerName]", fr.Name);
        //                    tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
        //                    html = Util.Utility.GetHtml("RequestSample", tokens);
        //                    SendMail.Send(fr.Email, "Zion Market Research : Request TOC", html);
        //                    SendMail.SendMailToZion("Zion Market Research : Request TOC", string.Format("Dear Admin, </br> Request TOC for report <br/> <table> " +
        //                        "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
        //                        "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
        //                        "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
        //                        "<tr><td> <b> Report Title - </b> </td><td> {0} </td></tr>" +
        //                        "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +
        //                        "<tr><td> <b> Customer Name - </b> </td><td> {1} </td></tr>" +
        //                        "<tr><td><b> Email Id -</b> </td><td>{2}</td></tr>" +
        //                        "<tr><td><b> Phone -</b></td><td> {3} </td></tr>" +
        //                        "<tr><td><b> Name of company -</b></td><td> {4} </td></tr>" +
        //                        "<tr><td><b>Designation -</b></td><td>{5}</td></tr>" +
        //                        "<tr><td><b> Country Name -</b></td><td>{6} </td></tr>" +
        //                        "<tr><td><b> IP Address -</b> </td><td>{7}</td></tr>" +
        //                        "<tr><td><b> Message -</b></td><td>{8}</td></tr>" +
        //                        "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
        //                    break;
        //                }
        //            case (int)GlobalConstant.FormType.BlackFriday:
        //            case (int)GlobalConstant.FormType.YearEndSale:
        //            case (int)GlobalConstant.FormType.ContactUs:
        //                {
        //                    tokens.Add("[MRS:CustomerName]", fr.Name);
        //                    html = Util.Utility.GetHtml("ContactUs", tokens);
        //                    SendMail.Send(fr.Email, "Zion Market Research : Contact Us", html);
        //                    SendMail.SendMailToZion("Zion Market Research : Contact Us", string.Format("Dear Admin, </br> User Contact us<br/> <table> " +
        //                        "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
        //                        "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
        //                        "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
        //                        "<tr><td> <b> Report Title - </b> </td><td> Contact Us </td></tr>" +
        //                        "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/contact-us</td></tr>" +

        //                        "<tr><td> <b> Customer Name - </b> </td><td> {0} </td></tr>" +
        //                        "<tr><td><b> Email Id -</b> </td><td>{1}</td></tr>" +
        //                        "<tr><td><b> Phone -</b></td><td> {2} </td></tr>" +
        //                        "<tr><td><b> Name of company -</b></td><td> {3} </td></tr>" +
        //                        "<tr><td><b>Designation -</b></td><td>{4}</td></tr>" +
        //                        "<tr><td><b> Country Name -</b></td><td>{5} </td></tr>" +
        //                        "<tr><td><b> IP Address -</b> </td><td>{6}</td></tr>" +
        //                        "<tr><td><b> Message -</b></td><td>{7}</td></tr>" +
        //                        "</table>", fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
        //                    break;
        //                }
        //            case (int)GlobalConstant.FormType.NotFound:
        //                {
        //                    tokens.Add("[MRS:CustomerName]", fr.Name);
        //                    html = Util.Utility.GetHtml("ContactUs", tokens);
        //                    fr.Comment = fr.Comment + " " + HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
        //                    SendMail.Send(fr.Email, "Zion Market Research : Not Found", html);
        //                    SendMail.SendMailToZion("Zion Market Research : Not Found", string.Format("Dear Admin, </br> User Contact us<br/> <table> <tr><td> <b> Customer Name - </b> </td><td> {0} </td></tr><tr><td><b> Email Id -</b> </td><td>{1}</td></tr><tr><td><b> Phone -</b></td><td> {2} </td></tr><tr><td><b> Name of company -</b></td><td> {3} </td></tr><tr><td><b>Designation -</b></td><td>{4}</td></tr><tr><td><b> Country Name -</b></td><td>{5} </td></tr><tr><td><b> IP Address -</b> </td><td>{6}</td></tr><td>Message</td><td>{7}</td></table>", fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
        //                    break;
        //                }
        //            case (int)GlobalConstant.FormType.Search:
        //                {
        //                    tokens.Add("[MRS:CustomerName]", fr.Name);
        //                    html = Util.Utility.GetHtml("ContactUs", tokens);
        //                    fr.Comment = "Search Query - " + fr.SearchText;
        //                    SendMail.Send(fr.Email, "Zion Market Research", html);
        //                    SendMail.SendMailToZion("Zion Market Research : Search", string.Format("Dear Admin, </br> User Contact us<br/> <table> " +
        //                        "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
        //                        "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
        //                        "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
        //                        "<tr><td> <b> Report Title - </b> </td><td> Search Page </td></tr>" +
        //                        "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/simplesearch</td></tr>" +

        //                        "<tr><td> <b> Customer Name - </b> </td><td> {0} </td></tr>" +
        //                        "<tr><td><b> Email Id -</b> </td><td>{1}</td></tr>" +
        //                        "<tr><td><b> Phone -</b></td><td> {2} </td></tr>" +
        //                        "<tr><td><b> Name of company -</b></td><td> {3} </td></tr>" +
        //                        "<tr><td><b>Designation -</b></td><td>{4}</td></tr>" +
        //                        "<tr><td><b> Country Name -</b></td><td>{5} </td></tr>" +
        //                        "<tr><td><b> IP Address -</b> </td><td>{6}</td></tr>" +
        //                        "<tr><td><b> Message -</b></td><td>{7} | <b>Search Query :: </b>{8}</td></tr>" +
        //                        "</table>", fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment, fr.SearchText), inquiry.Type.ToLower());
        //                    break;
        //                }
        //            case (int)GlobalConstant.FormType.AskToAnalyst:
        //                {
        //                    tokens.Add("[MRS:CustomerName]", fr.Name);
        //                    tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
        //                    html = Util.Utility.GetHtml("AskToAnalyst", tokens);
        //                    SendMail.Send(fr.Email, "Zion Market Research : Ask To Analyst", html);
        //                    SendMail.SendMailToZion("Zion Market Research : Ask To Analyst", string.Format("Dear Admin, </br> Request TOC for report <br/> <table> " +
        //                        "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
        //                        "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
        //                        "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
        //                        "<tr><td> <b> Report Title - </b> </td><td> {0} </td></tr>" +
        //                        "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/simplesearch</td></tr>" +

        //                        "<tr><td> <b> Customer Name - </b> </td><td> {1} </td></tr>" +
        //                        "<tr><td><b> Email Id -</b> </td><td>{2}</td></tr>" +
        //                        "<tr><td><b> Phone -</b></td><td> {3} </td></tr>" +
        //                        "<tr><td><b> Name of company -</b></td><td> {4} </td></tr>" +
        //                        "<tr><td><b>Designation -</b></td><td>{5}</td></tr>" +
        //                        "<tr><td><b> Country Name -</b></td><td>{6} </td></tr>" +
        //                        "<tr><td><b> IP Address -</b> </td><td>{7}</td></tr>" +
        //                        "<tr><td>Message</td><td>{8} | <b>Call :: </b> {9}</td></tr>" +
        //                        "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment, fr.ExtraData), inquiry.Type.ToLower());
        //                    //this just email should not have the comment with call timing - Milind Khangan
        //                    fr.Comment += "</br>" + fr.ExtraData;
        //                    break;
        //                }
        //            case (int)GlobalConstant.FormType.RequestBrochure:
        //                {
        //                    tokens.Add("[MRS:CustomerName]", fr.Name);
        //                    tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
        //                    html = Util.Utility.GetHtml("RequestSample", tokens);
        //                    SendMail.Send(fr.Email, "Zion Market Research : " + report.ReportTitle, html);
        //                    SendMail.SendMailToZion("Zion Market Research : Request Brochure", string.Format("Dear Admin, </br> quick inquiry for report <br/> <table> " +
        //                        "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
        //                        "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
        //                        "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
        //                        "<tr><td> <b> Report Title - </b> </td><td> {0} </td></tr>" +
        //                        "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +

        //                        "<tr><td> <b> Customer Name - </b> </td><td> {1} </td></tr>" +
        //                        "<tr><td><b> Email Id -</b> </td><td>{2}</td></tr>" +
        //                        "<tr><td><b> Phone -</b></td><td> {3} </td></tr>" +
        //                        "<tr><td><b> Name of company -</b></td><td> {7} </td></tr>" +
        //                        "<tr><td><b>Designation -</b></td><td>{8}</td></tr>" +
        //                        "<tr><td><b> Country Name -</b></td><td>{4} </td></tr>" +
        //                        "<tr><td><b> IP Address -</b> </td><td>{5}</td></tr>" +
        //                        "<tr><td>Message</td><td>{6}</td></tr>" +
        //                        "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Country, Util.Utility.ClientIPAddress, fr.Comment, fr.Company, fr.Designation), inquiry.Type.ToLower());
        //                    break;
        //                }
        //            case 11:
        //                {
        //                    tokens.Add("[MRS:CustomerName]", fr.Name);
        //                    tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
        //                    html = Util.Utility.GetHtml("RequestSample", tokens);
        //                    SendMail.Send(fr.Email, "Zion Market Research : " + report.ReportTitle, html);
        //                    SendMail.SendMailToZion("Zion Market Research : Request Brochure", string.Format("Dear Admin, </br> sample request/enquiry for report <br/> <table> " +
        //                        "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
        //                        "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
        //                        "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
        //                        "<tr><td> <b> Report Title - </b> </td><td> {0} </td></tr>" +
        //                        "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +

        //                        "<tr><td> <b> Customer Name - </b> </td><td> {1} </td></tr>" +
        //                        "<tr><td><b> Email Id -</b> </td><td>{2}</td></tr>" +
        //                        "<tr><td><b> Phone -</b></td><td> {3} </td></tr>" +
        //                        "<tr><td><b> Name of company -</b></td><td> {4} </td></tr>" +
        //                        "<tr><td><b>Designation -</b></td><td>{5}</td></tr>" +
        //                        "<tr><td><b> Country Name -</b></td><td>{6} </td></tr>" +
        //                        "<tr><td><b> IP Address -</b> </td><td>{7}</td></tr>" +
        //                        "<tr><td><b> Message -</b></td><td>{8}</td></tr>" +
        //                        "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
        //                    break;
        //                }
        //            case (int)GlobalConstant.FormType.RequestDiscount:
        //                {
        //                    tokens.Add("[MRS:CustomerName]", fr.Name);
        //                    tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
        //                    html = Util.Utility.GetHtml("RequestSample", tokens);
        //                    SendMail.Send(fr.Email, "Zion Market Research : " + report.ReportTitle, html);
        //                    SendMail.SendMailToZion("Zion Market Research : Request Discount", string.Format("Dear Admin, </br> sample request/enquiry for report <br/> <table> " +
        //                        "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
        //                        "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
        //                        "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
        //                        "<tr><td> <b> Report Title - </b> </td><td> {0} </td></tr>" +
        //                        "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +

        //                        "<tr><td> <b> Customer Name - </b> </td><td> {1} </td></tr>" +
        //                        "<tr><td><b> Email Id -</b> </td><td>{2}</td></tr>" +
        //                        "<tr><td><b> Phone -</b></td><td> {3} </td></tr>" +
        //                        "<tr><td><b> Name of company -</b></td><td> {4} </td></tr>" +
        //                        "<tr><td><b>Designation -</b></td><td>{5}</td></tr>" +
        //                        "<tr><td><b> Country Name -</b></td><td>{6} </td></tr>" +
        //                        "<tr><td><b> IP Address -</b> </td><td>{7}</td></tr>" +
        //                        "<tr><td><b> Message -</b></td><td>{8}</td></tr>" +
        //                        "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
        //                    break;
        //                }
        //            case (int)GlobalConstant.FormType.RequestMethodology:
        //                {
        //                    tokens.Add("[MRS:CustomerName]", fr.Name);
        //                    tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
        //                    html = Util.Utility.GetHtml("RequestSample", tokens);
        //                    SendMail.Send(fr.Email, "Zion Market Research : " + report.ReportTitle, html);
        //                    SendMail.SendMailToZion("Zion Market Research : Request Methodology", string.Format("Dear Admin, </br> sample request/enquiry for report <br/> <table> " +
        //                        "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
        //                        "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
        //                        "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
        //                        "<tr><td> <b> Report Title - </b> </td><td> {0} </td></tr>" +
        //                        "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +

        //                        "<tr><td> <b> Customer Name - </b> </td><td> {1} </td></tr>" +
        //                        "<tr><td><b> Email Id -</b> </td><td>{2}</td></tr>" +
        //                        "<tr><td><b> Phone -</b></td><td> {3} </td></tr>" +
        //                        "<tr><td><b> Name of company -</b></td><td> {4} </td></tr>" +
        //                        "<tr><td><b>Designation -</b></td><td>{5}</td></tr>" +
        //                        "<tr><td><b> Country Name -</b></td><td>{6} </td></tr>" +
        //                        "<tr><td><b> IP Address -</b> </td><td>{7}</td></tr>" +
        //                        "<tr><td><b> Message -</b></td><td>{8}</td></tr>" +
        //                        "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
        //                    break;
        //                }

        //            default:
        //                throw new Exception("Unexpected Case");
        //        }
        //        HttpContext.Current.Session["formMessage"] = html;

        //        #region Save To CRM
        //        if (fr.FormType != 5 && fr.FormType != (int)GlobalConstant.FormType.NotFound && fr.FormType != 6 && fr.FormType != (int)GlobalConstant.FormType.YearEndSale && fr.FormType != (int)GlobalConstant.FormType.BlackFriday)
        //        {
        //            fr.ReportTitle = report.ReportTitle;
        //            fr.ReportId = report.ReportId.ToString();
        //            fr.ReportUrl = report.ReportUrl;
        //        }

        //        fr.Comment = (!string.IsNullOrEmpty(fr.Comment) ? fr.Comment : string.Empty);

        //        SubmitToPortal(fr, isFreeAnalysisNull ? 42 : 21, isFreeAnalysisNull ? 55 : 21);
        //        #endregion

        //        return fr.FormType != (int)GlobalConstant.FormType.ContactUs
        //            && fr.FormType != (int)GlobalConstant.FormType.Search
        //            && fr.FormType != (int)GlobalConstant.FormType.YearEndSale
        //            && fr.FormType != (int)GlobalConstant.FormType.BlackFriday
        //            && fr.FormType != (int)GlobalConstant.FormType.NotFound
        //            ? report.ReportId : inquiry.Id;
        //    }
        //}
        #endregion Submit old logic


        public static int Submit(FormRepository fr)
        {
            string lang = "";
            string url = HttpContext.Current.Request.RawUrl;
            Console.WriteLine("HttpContext.Current.Request.RawUrl : " + url);
            Regex regexURL = new Regex(@"^([A-Za-z]{2})[\/]");
            if (regexURL.IsMatch(HttpContext.Current.Request.RawUrl))
            {
                lang = regexURL.Match(url).Value + "/";
                Console.WriteLine(lang);
            }

            var tasks = new List<Task>();
            //fr.FormType <= 2 for ContactUs Form
            Int32.TryParse(fr.ReportId, out int ri);
            if (ri == 0)
            {
                if (fr.FormType != (int)GlobalConstant.FormType.ContactUs
                    && fr.FormType != (int)GlobalConstant.FormType.Search
                    && fr.FormType != (int)GlobalConstant.FormType.NotFound
                    && !Int32.TryParse(Util.Utility.Decryptstring(fr.ReportId), out ri))
                    return 0;
            }

            //var countryInfo = Util.Utility.ClientIPAddress != null && !string.IsNullOrEmpty(Util.Utility.ClientIPAddress) ? Util.Utility.GetCountryInfo(Util.Utility.ClientIPAddress) : null;


            //var report = db.tblreportinformations.Where(r => r.ReportID == ri).SingleOrDefault();
            var report = ri > 0 ? ReportRepository.GetReportById(ri) : null;

            //fr.FormType <= 2 for ContactUs Form
            if (report == null && fr.FormType != 5
                && fr.FormType != 6
                && fr.FormType != (int)GlobalConstant.FormType.YearEndSale
                && fr.FormType != (int)GlobalConstant.FormType.NotFound
                && fr.FormType != (int)GlobalConstant.FormType.BlackFriday)
                return 0;

            if (report != null)
            {
                fr.CRMCategory = report != null ? report.CRMCategory : string.Empty;
                fr.ReportTitle = report.ReportTitle;
                fr.ReportUrl = report.ReportUrl;
            }
            fr.IP = Util.Utility.ClientIPAddress;

            var isFreeAnalysisNull = report != null && string.IsNullOrEmpty(report.FreeAnalysis) && !(bool)report.IsUpcoming;

            fr.Comment = fr.Comment.Replace("http://", "").Replace("https://", "");

            #region SAve Inquiry to DB
            var inquiry = new tblinquiry
            {
                Name = fr.Name,
                CompanyName = fr.Company,
                EmailId = fr.Email,
                PhoneNumber = fr.Phone,
                DateOfInquiry = DateTime.Now,
                Type = Enum.GetName(typeof(GlobalConstant.FormType), fr.FormType),
                IpAddress = Util.Utility.ClientIPAddress,
                Message = fr.Comment,
                ReportId = ri.ToString(),
                Designation = fr.Designation,
                Country = fr.Country //countryInfo != null && countryInfo.country != null ? countryInfo.country.name : fr.Country;
            };

            //[MS]: 20230126 : comment this code : from saving lead info into fnf db
            var t0 = Task.Run(() =>
            {
                using (ZionDbEntities db = new ZionDbEntities())
                {
                    db.tblinquiries.Add(inquiry);
                    db.SaveChanges();
                }
            });
            t0.ConfigureAwait(false);
            #endregion

            if (fr.AutoFill)
            {
                //save form details in cookies for next time//
                var cookie_personDetails = new HttpCookie("PersonDetails")
                {
                    Expires = DateTime.Now.Date.AddDays(30),
                    Value = Newtonsoft.Json.JsonConvert.SerializeObject(fr),
                    Secure = true,
                    HttpOnly = true
                };
                HttpContext.Current.Response.Cookies.Add(cookie_personDetails);
            }

            var tokens = new Dictionary<string, string>();
            var html = string.Empty;
            switch (fr.FormType)
            {
                case (int)GlobalConstant.FormType.PopupSample:
                case (int)GlobalConstant.FormType.RequestSample:
                    {
                        tokens.Add("[MRS:CustomerName]", fr.Name);
                        tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
                        html = Util.Utility.GetHtml("RequestSample", tokens);
                        SendMail.Send(fr.Email, "Zion Market Research : " + report.ReportTitle, html);
                        SendMail.SendMailToZion($"Zion Market Research : Request Sample {(fr.FormType == (int)GlobalConstant.FormType.PopupSample ? "(PopUp)" : string.Empty)}", string.Format("Dear Admin, </br> sample request/enquiry for report<br/><br/> " + "<table> " +
                            "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
                            "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
                            "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type.ToLower() + "</td></tr>" +
                            "<tr><td><b> Report Title - </b></td>   <td>{0}</td></tr>" +
                            "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +
                            "<tr><td><b> Customer Name - </b></td>  <td>{1}</td></tr>" +
                            "<tr><td><b> Email Id -</b> </td>       <td>{2}</td></tr>" +
                            "<tr><td><b> Phone -</b></td>           <td>{3}</td></tr>" +
                            "<tr><td><b> Name of company -</b></td> <td>{4}</td></tr>" +
                            "<tr><td><b> Designation -</b></td>      <td>{5}</td></tr>" +
                            "<tr><td><b> Country Name -</b></td>    <td>{6}</td></tr>" +
                            "<tr><td><b> IP Address -</b> </td>     <td>{7}</td></tr>" +
                            "<tr><td><b> Message - </td>            <td>{8}</td></tr>" +
                            "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
                        break;
                    }
                case (int)GlobalConstant.FormType.BuyingEnquiry:
                    {
                        tokens.Add("[MRS:CustomerName]", fr.Name);
                        tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
                        html = Util.Utility.GetHtml("BuyingEnquiry", tokens);
                        SendMail.Send(fr.Email, "Zion Market Research : " + report.ReportTitle, html);
                        SendMail.SendMailToZion("Zion Market Research : Buying Inquiry ", string.Format("Dear Admin, </br> Inquiry before buying for report <br/> <table> " +
                            "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
                            "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
                            "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
                            "<tr><td> <b> Report Title - </b> </td> <td> {0} </td></tr>" +
                            "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +
                            "<tr><td> <b> Customer Name -</b></td>  <td> {1} </td></tr>" +
                            "<tr><td><b> Email Id -</b> </td>       <td>{2}</td></tr>" +
                            "<tr><td><b> Phone -</b></td>           <td> {3} </td></tr>" +
                            "<tr><td><b> Name of company -</b></td> <td> {4} </td></tr>" +
                            "<tr><td><b> Designation -</b></td>      <td>{5}</td></tr>" +
                            "<tr><td><b> Country Name -</b></td>    <td>{6} </td></tr>" +
                            "<tr><td><b> IP Address -</b> </td>     <td>{7}</td></tr>" +
                            "<tr><td><b> Message </b></td>          <td>{8}</td>" +
                            "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
                        break;
                    }
                case (int)GlobalConstant.FormType.CustomRequest:
                    {
                        tokens.Add("[MRS:CustomerName]", fr.Name);
                        tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
                        html = Util.Utility.GetHtml("RequestSample", tokens);
                        var emailSubject = fr.COVID19Analysis ? "COVID-19 Impact Analysis" : "Request Customization";
                        SendMail.Send(fr.Email, $"Zion Market Research : {emailSubject}", html);
                        SendMail.SendMailToZion($"Zion Market Research : {emailSubject}", string.Format("Dear Admin, </br> Request customization for report <br/> <table> " +
                            "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
                            "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
                            "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
                            "<tr><td> <b> Report Title - </b> </td><td> {0} </td></tr>" +
                            "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +
                            "<tr><td> <b> Customer Name - </b> </td><td> {1} </td></tr>" +
                            "<tr><td><b> Email Id -</b> </td><td>{2}</td></tr>" +
                            "<tr><td><b> Phone -</b></td><td> {3} </td></tr>" +
                            "<tr><td><b> Name of company -</b></td><td> {4} </td></tr>" +
                            "<tr><td><b>Designation -</b></td><td>{5}</td></tr>" +
                            "<tr><td><b> Country Name -</b></td><td>{6} </td></tr>" +
                            "<tr><td><b> IP Address -</b> </td><td>{7}</td></tr>" +
                            "<tr><td><b>Message -</b></td><td>{8}</td></tr>"
                            + (fr.COVID19Analysis ? "<tr><td><b>Covid 19 Impact Analysis</b></td><td>Yes</td></tr>" : string.Empty)
                            + "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
                        break;
                    }
                case (int)GlobalConstant.FormType.RequestTOC:
                    {
                        tokens.Add("[MRS:CustomerName]", fr.Name);
                        tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
                        html = Util.Utility.GetHtml("RequestSample", tokens);
                        SendMail.Send(fr.Email, "Zion Market Research : Request TOC", html);
                        SendMail.SendMailToZion("Zion Market Research : Request TOC", string.Format("Dear Admin, </br> Request TOC for report <br/> <table> " +
                            "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
                            "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
                            "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
                            "<tr><td> <b> Report Title - </b> </td><td> {0} </td></tr>" +
                            "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +
                            "<tr><td> <b> Customer Name - </b> </td><td> {1} </td></tr>" +
                            "<tr><td><b> Email Id -</b> </td><td>{2}</td></tr>" +
                            "<tr><td><b> Phone -</b></td><td> {3} </td></tr>" +
                            "<tr><td><b> Name of company -</b></td><td> {4} </td></tr>" +
                            "<tr><td><b>Designation -</b></td><td>{5}</td></tr>" +
                            "<tr><td><b> Country Name -</b></td><td>{6} </td></tr>" +
                            "<tr><td><b> IP Address -</b> </td><td>{7}</td></tr>" +
                            "<tr><td><b> Message -</b></td><td>{8}</td></tr>" +
                            "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
                        break;
                    }
                case (int)GlobalConstant.FormType.BlackFriday:
                case (int)GlobalConstant.FormType.YearEndSale:
                case (int)GlobalConstant.FormType.ContactUs:
                    {
                        tokens.Add("[MRS:CustomerName]", fr.Name);
                        html = Util.Utility.GetHtml("ContactUs", tokens);
                        SendMail.Send(fr.Email, "Zion Market Research : Contact Us", html);
                        SendMail.SendMailToZion("Zion Market Research : Contact Us", string.Format("Dear Admin, </br> User Contact us<br/> <table> " +
                            "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
                            "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
                            "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
                            "<tr><td> <b> Report Title - </b> </td><td> Contact Us </td></tr>" +
                            "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/contact-us</td></tr>" +

                            "<tr><td> <b> Customer Name - </b> </td><td> {0} </td></tr>" +
                            "<tr><td><b> Email Id -</b> </td><td>{1}</td></tr>" +
                            "<tr><td><b> Phone -</b></td><td> {2} </td></tr>" +
                            "<tr><td><b> Name of company -</b></td><td> {3} </td></tr>" +
                            "<tr><td><b>Designation -</b></td><td>{4}</td></tr>" +
                            "<tr><td><b> Country Name -</b></td><td>{5} </td></tr>" +
                            "<tr><td><b> IP Address -</b> </td><td>{6}</td></tr>" +
                            "<tr><td><b> Message -</b></td><td>{7}</td></tr>" +
                            "</table>", fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
                        break;
                    }
                case (int)GlobalConstant.FormType.NotFound:
                    {
                        tokens.Add("[MRS:CustomerName]", fr.Name);
                        html = Util.Utility.GetHtml("ContactUs", tokens);
                        fr.Comment = fr.Comment + " " + HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
                        SendMail.Send(fr.Email, "Zion Market Research : Not Found", html);
                        SendMail.SendMailToZion("Zion Market Research : Not Found", string.Format("Dear Admin, </br> User Contact us<br/> <table> <tr><td> <b> Customer Name - </b> </td><td> {0} </td></tr><tr><td><b> Email Id -</b> </td><td>{1}</td></tr><tr><td><b> Phone -</b></td><td> {2} </td></tr><tr><td><b> Name of company -</b></td><td> {3} </td></tr><tr><td><b>Designation -</b></td><td>{4}</td></tr><tr><td><b> Country Name -</b></td><td>{5} </td></tr><tr><td><b> IP Address -</b> </td><td>{6}</td></tr><td>Message</td><td>{7}</td></table>", fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
                        break;
                    }
                case (int)GlobalConstant.FormType.Search:
                    {
                        tokens.Add("[MRS:CustomerName]", fr.Name);
                        html = Util.Utility.GetHtml("ContactUs", tokens);
                        fr.Comment = "Search Query - " + fr.SearchText;
                        SendMail.Send(fr.Email, "Zion Market Research", html);
                        SendMail.SendMailToZion("Zion Market Research : Search", string.Format("Dear Admin, </br> User Contact us<br/> <table> " +
                            "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
                            "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
                            "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
                            "<tr><td> <b> Report Title - </b> </td><td> Search Page </td></tr>" +
                            "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/simplesearch</td></tr>" +

                            "<tr><td> <b> Customer Name - </b> </td><td> {0} </td></tr>" +
                            "<tr><td><b> Email Id -</b> </td><td>{1}</td></tr>" +
                            "<tr><td><b> Phone -</b></td><td> {2} </td></tr>" +
                            "<tr><td><b> Name of company -</b></td><td> {3} </td></tr>" +
                            "<tr><td><b>Designation -</b></td><td>{4}</td></tr>" +
                            "<tr><td><b> Country Name -</b></td><td>{5} </td></tr>" +
                            "<tr><td><b> IP Address -</b> </td><td>{6}</td></tr>" +
                            "<tr><td><b> Message -</b></td><td>{7} | <b>Search Query :: </b>{8}</td></tr>" +
                            "</table>", fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment, fr.SearchText), inquiry.Type.ToLower());
                        break;
                    }
                case (int)GlobalConstant.FormType.AskToAnalyst:
                    {
                        tokens.Add("[MRS:CustomerName]", fr.Name);
                        tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
                        html = Util.Utility.GetHtml("AskToAnalyst", tokens);
                        SendMail.Send(fr.Email, "Zion Market Research : Ask To Analyst", html);
                        SendMail.SendMailToZion("Zion Market Research : Ask To Analyst", string.Format("Dear Admin, </br> Request TOC for report <br/> <table> " +
                            "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
                            "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
                            "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
                            "<tr><td> <b> Report Title - </b> </td><td> {0} </td></tr>" +
                            "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/simplesearch</td></tr>" +

                            "<tr><td> <b> Customer Name - </b> </td><td> {1} </td></tr>" +
                            "<tr><td><b> Email Id -</b> </td><td>{2}</td></tr>" +
                            "<tr><td><b> Phone -</b></td><td> {3} </td></tr>" +
                            "<tr><td><b> Name of company -</b></td><td> {4} </td></tr>" +
                            "<tr><td><b>Designation -</b></td><td>{5}</td></tr>" +
                            "<tr><td><b> Country Name -</b></td><td>{6} </td></tr>" +
                            "<tr><td><b> IP Address -</b> </td><td>{7}</td></tr>" +
                            "<tr><td>Message</td><td>{8} | <b>Call :: </b> {9}</td></tr>" +
                            "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment, fr.ExtraData), inquiry.Type.ToLower());
                        //this just email should not have the comment with call timing - Milind Khangan
                        fr.Comment += "</br>" + fr.ExtraData;
                        break;
                    }
                case (int)GlobalConstant.FormType.RequestBrochure:
                    {
                        tokens.Add("[MRS:CustomerName]", fr.Name);
                        tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
                        html = Util.Utility.GetHtml("RequestSample", tokens);
                        SendMail.Send(fr.Email, "Zion Market Research : " + report.ReportTitle, html);
                        SendMail.SendMailToZion("Zion Market Research : Request Brochure", string.Format("Dear Admin, </br> quick inquiry for report <br/> <table> " +
                            "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
                            "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
                            "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
                            "<tr><td> <b> Report Title - </b> </td><td> {0} </td></tr>" +
                            "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +

                            "<tr><td> <b> Customer Name - </b> </td><td> {1} </td></tr>" +
                            "<tr><td><b> Email Id -</b> </td><td>{2}</td></tr>" +
                            "<tr><td><b> Phone -</b></td><td> {3} </td></tr>" +
                            "<tr><td><b> Name of company -</b></td><td> {7} </td></tr>" +
                            "<tr><td><b>Designation -</b></td><td>{8}</td></tr>" +
                            "<tr><td><b> Country Name -</b></td><td>{4} </td></tr>" +
                            "<tr><td><b> IP Address -</b> </td><td>{5}</td></tr>" +
                            "<tr><td>Message</td><td>{6}</td></tr>" +
                            "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Country, Util.Utility.ClientIPAddress, fr.Comment, fr.Company, fr.Designation), inquiry.Type.ToLower());
                        break;
                    }
                case 11:
                    {
                        tokens.Add("[MRS:CustomerName]", fr.Name);
                        tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
                        html = Util.Utility.GetHtml("RequestSample", tokens);
                        SendMail.Send(fr.Email, "Zion Market Research : " + report.ReportTitle, html);
                        SendMail.SendMailToZion("Zion Market Research : Request Brochure", string.Format("Dear Admin, </br> sample request/enquiry for report <br/> <table> " +
                            "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
                            "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
                            "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
                            "<tr><td> <b> Report Title - </b> </td><td> {0} </td></tr>" +
                            "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +

                            "<tr><td> <b> Customer Name - </b> </td><td> {1} </td></tr>" +
                            "<tr><td><b> Email Id -</b> </td><td>{2}</td></tr>" +
                            "<tr><td><b> Phone -</b></td><td> {3} </td></tr>" +
                            "<tr><td><b> Name of company -</b></td><td> {4} </td></tr>" +
                            "<tr><td><b>Designation -</b></td><td>{5}</td></tr>" +
                            "<tr><td><b> Country Name -</b></td><td>{6} </td></tr>" +
                            "<tr><td><b> IP Address -</b> </td><td>{7}</td></tr>" +
                            "<tr><td><b> Message -</b></td><td>{8}</td></tr>" +
                            "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
                        break;
                    }
                case (int)GlobalConstant.FormType.RequestDiscount:
                    {
                        tokens.Add("[MRS:CustomerName]", fr.Name);
                        tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
                        html = Util.Utility.GetHtml("RequestSample", tokens);
                        SendMail.Send(fr.Email, "Zion Market Research : " + report.ReportTitle, html);
                        SendMail.SendMailToZion("Zion Market Research : Request Discount", string.Format("Dear Admin, </br> sample request/enquiry for report <br/> <table> " +
                            "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
                            "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
                            "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
                            "<tr><td> <b> Report Title - </b> </td><td> {0} </td></tr>" +
                            "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +

                            "<tr><td> <b> Customer Name - </b> </td><td> {1} </td></tr>" +
                            "<tr><td><b> Email Id -</b> </td><td>{2}</td></tr>" +
                            "<tr><td><b> Phone -</b></td><td> {3} </td></tr>" +
                            "<tr><td><b> Name of company -</b></td><td> {4} </td></tr>" +
                            "<tr><td><b>Designation -</b></td><td>{5}</td></tr>" +
                            "<tr><td><b> Country Name -</b></td><td>{6} </td></tr>" +
                            "<tr><td><b> IP Address -</b> </td><td>{7}</td></tr>" +
                            "<tr><td><b> Message -</b></td><td>{8}</td></tr>" +
                            "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
                        break;
                    }
                case (int)GlobalConstant.FormType.RequestMethodology:
                    {
                        tokens.Add("[MRS:CustomerName]", fr.Name);
                        tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
                        html = Util.Utility.GetHtml("RequestSample", tokens);
                        SendMail.Send(fr.Email, "Zion Market Research : " + report.ReportTitle, html);
                        SendMail.SendMailToZion("Zion Market Research : Request Methodology", string.Format("Dear Admin, </br> sample request/enquiry for report <br/> <table> " +
                            "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
                            "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
                            "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type + "</td></tr>" +
                            "<tr><td> <b> Report Title - </b> </td><td> {0} </td></tr>" +
                            "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +

                            "<tr><td> <b> Customer Name - </b> </td><td> {1} </td></tr>" +
                            "<tr><td><b> Email Id -</b> </td><td>{2}</td></tr>" +
                            "<tr><td><b> Phone -</b></td><td> {3} </td></tr>" +
                            "<tr><td><b> Name of company -</b></td><td> {4} </td></tr>" +
                            "<tr><td><b>Designation -</b></td><td>{5}</td></tr>" +
                            "<tr><td><b> Country Name -</b></td><td>{6} </td></tr>" +
                            "<tr><td><b> IP Address -</b> </td><td>{7}</td></tr>" +
                            "<tr><td><b> Message -</b></td><td>{8}</td></tr>" +
                            "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
                        break;
                    }
                case (int)GlobalConstant.FormType.CorporateEmail:
                    {
                        tokens.Add("[MRS:CustomerName]", fr.Name);
                        tokens.Add("[MRS:ReportTitle]", report.ReportTitle);
                        html = Util.Utility.GetHtml("RequestSample", tokens);
                        SendMail.Send(fr.Email, "Zion Market Research : " + report.ReportTitle, html);
                        SendMail.SendMailToZion($"Zion Market Research : Request Sample {(fr.FormType == (int)GlobalConstant.FormType.PopupSample ? "(PopUp)" : string.Empty)}", string.Format("Dear Admin, </br> sample request/enquiry for report<br/><br/> " + "<table> " +
                            "<tr><td width='30%'><b> Date - </td>               <td width='70%'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr>" +
                            "<tr><td><b> Domain - </td>             <td>ZMR</td></tr>" +
                            "<tr><td><b> Inquiry Type - </td>             <td>" + inquiry.Type.ToLower() + "</td></tr>" +
                            "<tr><td><b> Report Title - </b></td>   <td>{0}</td></tr>" +
                            "<tr><td><b> Report URL - </td>         <td>https://www.zionmarketresearch.com/report/" + report.ReportUrl + "</td></tr>" +
                            "<tr><td><b> Customer Name - </b></td>  <td>{1}</td></tr>" +
                            "<tr><td><b> Email Id -</b> </td>       <td>{2}</td></tr>" +
                            "<tr><td><b> Phone -</b></td>           <td>{3}</td></tr>" +
                            "<tr><td><b> Name of company -</b></td> <td>{4}</td></tr>" +
                            "<tr><td><b> Designation -</b></td>      <td>{5}</td></tr>" +
                            "<tr><td><b> Country Name -</b></td>    <td>{6}</td></tr>" +
                            "<tr><td><b> IP Address -</b> </td>     <td>{7}</td></tr>" +
                            "<tr><td><b> Message - </td>            <td>{8}</td></tr>" +
                            "</table>", report.ReportTitle, fr.Name, fr.Email, fr.Phone, fr.Company, fr.Designation, fr.Country, Util.Utility.ClientIPAddress, fr.Comment), inquiry.Type.ToLower());
                        break;
                    }

                default:
                    throw new Exception("Unexpected Case");
            }
            HttpContext.Current.Session["formMessage"] = html;
            Task.WaitAll(tasks.ToArray());

            #region Save To CRM
            if (fr.FormType != 5 && fr.FormType != (int)GlobalConstant.FormType.NotFound && fr.FormType != 6 && fr.FormType != (int)GlobalConstant.FormType.YearEndSale && fr.FormType != (int)GlobalConstant.FormType.BlackFriday)
            {
                fr.ReportTitle = report.ReportTitle;
                fr.ReportId = report.ReportId.ToString();
                fr.ReportUrl = lang + report.ReportUrl;
            }

            fr.Comment = (!string.IsNullOrEmpty(fr.Comment) ? fr.Comment : string.Empty);

            SubmitToPortal(fr, true, isFreeAnalysisNull ? 42 : 21, isFreeAnalysisNull ? 55 : 21);
            
            //int result = SubmitToPortal(fr, true, isFreeAnalysisNull ? 42 : 21, isFreeAnalysisNull ? 55 : 21);

            //string _emailDomain = fr.Email.Substring(fr.Email.IndexOf('@') + 1);
            //string CorporateEmailDomains = System.Configuration.ConfigurationManager.AppSettings["CorporateEmailDomains"];
            //List<string> CorporateEmailDomainsList = CorporateEmailDomains.Split(',').ToList();
            //for (int i = 0; i < CorporateEmailDomainsList.Count; i++)
            //{
            //    if (CorporateEmailDomainsList[i] == _emailDomain)
            //    {
            //        HttpContext.Current.Session["CrmLeadID"] = result;
            //        break;
            //    }
            //}

            #endregion

            return fr.FormType != (int)GlobalConstant.FormType.ContactUs
                && fr.FormType != (int)GlobalConstant.FormType.Search
                && fr.FormType != (int)GlobalConstant.FormType.YearEndSale
                && fr.FormType != (int)GlobalConstant.FormType.BlackFriday
                && fr.FormType != (int)GlobalConstant.FormType.NotFound
                ? report.ReportId : inquiry.Id;

        }

        public static string GetHtmlTemplateName(int formType)
        {
            var templateName = string.Empty;
            switch (formType)
            {
                case (int)GlobalConstant.FormType.PopupSample:
                case (int)GlobalConstant.FormType.RequestSample:
                case (int)GlobalConstant.FormType.RequestMethodology:
                case (int)GlobalConstant.FormType.RequestDiscount:
                case 11:
                case (int)GlobalConstant.FormType.RequestBrochure:
                case (int)GlobalConstant.FormType.RequestTOC:
                case (int)GlobalConstant.FormType.CustomRequest:
                    templateName = "RequestSample";
                    break;
                case (int)GlobalConstant.FormType.AskToAnalyst:
                    templateName = "AskToAnalyst";
                    break;
                case (int)GlobalConstant.FormType.BlackFriday:
                case (int)GlobalConstant.FormType.YearEndSale:
                case (int)GlobalConstant.FormType.ContactUs:
                case (int)GlobalConstant.FormType.Search:
                    templateName = "ContactUs";
                    break;
                case (int)GlobalConstant.FormType.BuyingEnquiry:
                    templateName = "BuyingEnquiry";
                    break;
                default:
                    templateName = "RequestSample";
                    break;
            }
            return templateName;
        }

        public static FormRepository GetFromCookie()
        {
            var p = HttpContext.Current.Request.Cookies["PersonDetails"];
            if (p != null && !string.IsNullOrEmpty(p.Value))
            {
                var fr = Newtonsoft.Json.JsonConvert.DeserializeObject<FormRepository>(p.Value);
                fr.Captcha = string.Empty;
                fr.Comment = string.Empty;
                fr.AutoFill = false;
                return fr;
            }
            return null;
        }

        public static async Task<int> SubmitToPortal(FormRepository fr, bool isAsync, int domainId = 21, int publisherId = 21)
        {
            try
            {
                using (var client = new CRM.WebServiceSoapClient("WebServiceSoap"))
                {
                    int result = 0;
                    //[MS]: 20230729 : comment this code : from saving lead info into CRM db
                    if (isAsync)
                    {
                        await client.InsertUpdateKey_CatNameAsync(fr.CrmLeadID, Convert.ToInt32(fr.ReportId), fr.ReportTitle, !fr.COVID19Analysis ? GetFormType(fr.FormType) : 14, domainId, 1, 1, "report/" + fr.ReportUrl, Util.Utility.ClientIPAddress, fr.Name.Trim(), fr.Email.Trim(), fr.Phone.Trim(),
                        fr.Company != null ? fr.Company.Trim() : "!", fr.Designation != null ? fr.Designation.Trim() : "!", fr.Address == null ? "!" : fr.Address.Trim(), fr.State == null ? "!" : fr.State.Trim(),
                        fr.Country != null ? fr.Country.Trim() : "!", fr.Zip == null ? "!" : fr.Zip.Trim(), fr.Comment.Trim(), 1, "!", "!", "!", domainId == 21 ? "Zion Market Research" : "Zion New", publisherId, "BW&Zk^HfZ44P339nEzqrrawY4HL_VXw-5f+%8b4Hdw?$?m$G*!+kCGLK%3JjDn-74NY*LyhdJr6RAte&8MBWy6F2j82+qn7ap&DB@z-*q3sdH*#D-kwACucyaM7vzet4pSa?m^xnP@3zN5K9=*L6WLpDurTSuVTR3Hd&3XLHJnCcR!h*dL#fQhp^*#25LEFrMTt@z&8RWdf^CQcj!QrQU^WkdC5$Ub$8qnu!g7?*$$4%%M9?8spAugyCzZg5@dLGBNS_^7?x3VczR75J&=+9yFDVg*Qpd@R^_Jz-GtWgHxv4Kf$=2pxT@bqhx%aqgzZAN6RzZZ%rNX7km3fu$h?Z=+V3b_MQPLAxJBVT!=Ta+7Xd?CF3#4w44L@HU%nf4m#y-d2vgn6Gp2t7w!qFY%kN#y6DNAy#TbrZnqnjMtgeAd%BHSm9H29z4G_?qnBHE5J2EyutZ2RSh?P2fUE-sF8bNFdre@G^qQ??JzJuDCT3hby2py#+yfg*jC%&YBkrutHs", fr.CRMCategory);
                        return 1;
                    }
                    else
                    {
                        result = client.InsertUpdateKey_CatName(fr.CrmLeadID, Convert.ToInt32(fr.ReportId), fr.ReportTitle, !fr.COVID19Analysis ? GetFormType(fr.FormType) : 14, domainId, 1, 1, "report/" + fr.ReportUrl, Util.Utility.ClientIPAddress, fr.Name.Trim(), fr.Email.Trim(), fr.Phone.Trim(),
                        fr.Company != null ? fr.Company.Trim() : "!", fr.Designation != null ? fr.Designation.Trim() : "!", fr.Address == null ? "!" : fr.Address.Trim(), fr.State == null ? "!" : fr.State.Trim(),
                        fr.Country != null ? fr.Country.Trim() : "!", fr.Zip == null ? "!" : fr.Zip.Trim(), fr.Comment.Trim(), 1, "!", "!", "!", domainId == 21 ? "Zion Market Research" : "Zion New", publisherId, "BW&Zk^HfZ44P339nEzqrrawY4HL_VXw-5f+%8b4Hdw?$?m$G*!+kCGLK%3JjDn-74NY*LyhdJr6RAte&8MBWy6F2j82+qn7ap&DB@z-*q3sdH*#D-kwACucyaM7vzet4pSa?m^xnP@3zN5K9=*L6WLpDurTSuVTR3Hd&3XLHJnCcR!h*dL#fQhp^*#25LEFrMTt@z&8RWdf^CQcj!QrQU^WkdC5$Ub$8qnu!g7?*$$4%%M9?8spAugyCzZg5@dLGBNS_^7?x3VczR75J&=+9yFDVg*Qpd@R^_Jz-GtWgHxv4Kf$=2pxT@bqhx%aqgzZAN6RzZZ%rNX7km3fu$h?Z=+V3b_MQPLAxJBVT!=Ta+7Xd?CF3#4w44L@HU%nf4m#y-d2vgn6Gp2t7w!qFY%kN#y6DNAy#TbrZnqnjMtgeAd%BHSm9H29z4G_?qnBHE5J2EyutZ2RSh?P2fUE-sF8bNFdre@G^qQ??JzJuDCT3hby2py#+yfg*jC%&YBkrutHs", fr.CRMCategory);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                //HttpContext.Current.Response.Write(ex.Message);
            }
            return 0;
        }

        public static async Task<int> SubmitCheckoutInfoToPortal(FormRepository fr)
        {
            try
            {
                using (CRM.WebServiceSoapClient client = new CRM.WebServiceSoapClient("WebServiceSoap"))
                {
                    ////[MS]: 20230729 : comment this code : from saving lead info into CRM db
                    //await client.InsertUpdateKey_CatNameAsync(0, Convert.ToInt32(fr.ReportId), fr.ReportTitle, 3, 21, 1, 1, fr.ReportUrl, Util.Utility.ClientIPAddress, fr.Name.Trim(), fr.Email.Trim(), fr.Phone.Trim(), fr.Company.Trim(), fr.Designation.Trim(), fr.Address == null ? "!" : fr.Address.Trim(), fr.State == null ? "!" : fr.State.Trim(), fr.Country.Trim(), fr.Zip == null ? "!" : fr.Zip.Trim(), "!", 1, fr.GuId, "!", "!", "Zion Market Research", 21, "BW&Zk^HfZ44P339nEzqrrawY4HL_VXw-5f+%8b4Hdw?$?m$G*!+kCGLK%3JjDn-74NY*LyhdJr6RAte&8MBWy6F2j82+qn7ap&DB@z-*q3sdH*#D-kwACucyaM7vzet4pSa?m^xnP@3zN5K9=*L6WLpDurTSuVTR3Hd&3XLHJnCcR!h*dL#fQhp^*#25LEFrMTt@z&8RWdf^CQcj!QrQU^WkdC5$Ub$8qnu!g7?*$$4%%M9?8spAugyCzZg5@dLGBNS_^7?x3VczR75J&=+9yFDVg*Qpd@R^_Jz-GtWgHxv4Kf$=2pxT@bqhx%aqgzZAN6RzZZ%rNX7km3fu$h?Z=+V3b_MQPLAxJBVT!=Ta+7Xd?CF3#4w44L@HU%nf4m#y-d2vgn6Gp2t7w!qFY%kN#y6DNAy#TbrZnqnjMtgeAd%BHSm9H29z4G_?qnBHE5J2EyutZ2RSh?P2fUE-sF8bNFdre@G^qQ??JzJuDCT3hby2py#+yfg*jC%&YBkrutHs", fr.CRMCategory);
                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Use to get the appropriate value for form in CRM
        /// 0 - Sample Request
        /// 1 - Inquiry
        /// 2 - 
        /// 3 - Request for customization
        /// 4 - Request TOC
        /// 5 - Contact Us
        /// 6 - Search
        /// </summary>
        /// <param name="formType"></param>
        /// <returns></returns>
        static int GetFormType(int formType)
        {
            int res = -1;
            switch (formType)
            {
                case (int)GlobalConstant.FormType.RequestSample:
                    res = 2;
                    break;
                case (int)GlobalConstant.FormType.BuyingEnquiry:
                    res = 1;
                    break;
                case (int)GlobalConstant.FormType.CustomRequest:
                    res = 8;
                    break;
                case (int)GlobalConstant.FormType.RequestTOC:
                    res = 5;
                    break;
                case (int)GlobalConstant.FormType.YearEndSale:
                    res = 16;
                    break;
                case (int)GlobalConstant.FormType.Search:
                case (int)GlobalConstant.FormType.ContactUs:
                    res = 4;
                    break;
                case (int)GlobalConstant.FormType.AskToAnalyst:
                    res = 9;
                    break;
                case (int)GlobalConstant.FormType.QuickInquiry:
                    res = 10;
                    break;
                case (int)GlobalConstant.FormType.RequestDiscount:
                    res = 11;
                    break;
                case (int)GlobalConstant.FormType.RequestBrochure:
                    res = 12;
                    break;
                case (int)GlobalConstant.FormType.RequestMethodology:
                    res = 13;
                    break;
                case (int)GlobalConstant.FormType.PopupSample:
                    res = 15;
                    break;
                default:
                    res = formType;
                    break;
            }
            return res;
        }

        public static string BuildTable<T>(T obj)
        {
            var properties = obj.GetType().GetProperties();
            var attributes = new List<LeadTable>();
            var sb = new StringBuilder();
            sb.Append("<table>");
            foreach (var p in properties.Where(x => Attribute.GetCustomAttribute(x, typeof(LeadTable)) != null).ToList().OrderBy(x => (Attribute.GetCustomAttribute(x, typeof(LeadTable)) as LeadTable).Order))
            {
                var displayText = Attribute.GetCustomAttribute(p, typeof(LeadTable));

                if (displayText == null || !((LeadTable)displayText).Include || p.GetValue(obj) == null)
                    continue;
                attributes.Add(displayText as LeadTable);

                sb.Append($"<tr><td>{(displayText != null ? (displayText as LeadTable).DisplayText : p.Name)}</td><td>{p.GetValue(obj).ToString()}</td></tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }
    }

    public sealed class LeadTable : Attribute
    {
        private bool _include = true;
        public bool Include { get { return _include; } set { _include = value; } }
        public string DisplayText { get; set; }
        public int Order { get; set; }
    }

    public class AskInfo
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; }
        [Required, MaxLength(100)]
        public string LastName { get; set; }
        [Required, MaxLength(200)]
        [RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Invalid E-mail.")]
        public string Email { get; set; }
        [Required, MaxLength(20)]
        public string Phone { get; set; }
        [Required, MaxLength(300)]
        public string Comment { get; set; }
    }

    public static class AskInfoExtension
    {
        public static string Submit(this AskInfo askInfo)
        {
            var res = FormRepository.Submit(new FormRepository
            {

                Name = askInfo.FirstName + " " + askInfo.LastName,
                Email = askInfo.Email,
                Phone = askInfo.Phone,
                Comment = askInfo.Comment,
                FormType = (int)GlobalConstant.FormType.ContactUs
            });
            if (res > 0)
            {
                var tokens = new Dictionary<string, string>();
                tokens.Add("[MRS:CustomerName]", askInfo.FirstName + " " + askInfo.LastName);
                return Util.Utility.GetHtml("ContactUs", tokens);
            }
            return null;
        }
    }
}