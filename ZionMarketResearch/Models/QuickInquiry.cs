using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ZionMarketResearch.constants;

namespace ZionMarketResearch.Models
{
    public class QuickInquiry
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Invalid E-mail.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Message is required")]

        [RegularExpression(@"^[a-zA-Z0-9,.?' ']+$", ErrorMessage = "No special characters allowed.")]
        public string Message { get; set; }
        [Required(ErrorMessage = "Captcha is required")]
        public string Captcha { get; set; }
        public string ReportId { get; set; }
        public int FormType { get; set; }

        public static int SubmitQuickForm(QuickInquiry inquiry)
        {
            return FormRepository.Submit(new FormRepository
            {
                ReportId = inquiry.ReportId,
                FormType = inquiry.FormType,
                Name = inquiry.Name,
                Email = inquiry.Email,
                Phone = inquiry.Phone,
                Designation = string.Empty,
                Comment = inquiry.Message
            });
        }
    }

    public class AvailPDF
    {
        [Required(ErrorMessage = "Name is required"), MaxLength(200)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required"), MaxLength(200)]
        [RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Invalid E-mail.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required"), MaxLength(20)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Message is required"), MaxLength(500)]
        public string Message { get; set; }

        [Required(ErrorMessage = "Designation is required"), MaxLength(200)]
        public string Designation { get; set; }

        [Required(ErrorMessage = "ReportId is required")]
        public int ReportId { get; set; }

        [Required(ErrorMessage = "Country is required"), MaxLength(200)]
        public string Country { get; set; }

        [Required(ErrorMessage = "Company is required"), MaxLength(200)]
        public string Company { get; set; }

        public string ReportTitle { get; set; }

        public int CrmLeadID { get; set; }

        public static string SubmitAvailPDF(AvailPDF inquiry)
        {
            var res = FormRepository.Submit(new FormRepository
            {
                ReportId = inquiry.ReportId.ToString(),
                Name = inquiry.Name,
                Email = inquiry.Email,
                Phone = inquiry.Phone,
                Designation = inquiry.Designation,
                Comment = inquiry.Message,
                FormType = (int)GlobalConstant.FormType.PopupSample,
                Company = inquiry.Company,
                Country = inquiry.Country
            });
            if (res > 0)
            {
                var tokens = new Dictionary<string, string>();
                tokens.Add("[MRS:CustomerName]", inquiry.Name);
                tokens.Add("[MRS:ReportTitle]", inquiry.ReportTitle);
                return Util.Utility.GetHtml("RequestSample", tokens);
            }
            return null;
        }
    }
}