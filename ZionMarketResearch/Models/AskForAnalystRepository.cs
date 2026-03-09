using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZionMarketResearch.Models
{
    public class AskForAnalystRepository
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Message is required"), StringLength(300, ErrorMessage = "No big message please.")]
        [RegularExpression(@"^[a-zA-Z0-9,.?' ']+$", ErrorMessage = "No special characters allowed.")]
        public string Message { get; set; }
        [Required(ErrorMessage = "Date is required")]
        public DateTime SelectedDate { get; set; }
        public DateTime UTCDate { get; set; }
        public string IPAddress { get; set; }
        public string Captcha { get; set; }
        public string ReportId { get; set; }

        public static int Submit(AskForAnalystRepository analyst)
        {
            //var countryInfo = Util.Utility.GetCountryInfo(Util.Utility.ClientIPAddress);
            DateTime expectingCallingDate = new DateTime(analyst.SelectedDate.Year, analyst.SelectedDate.Month, analyst.SelectedDate.Day, 9, 0, 0);
            return FormRepository.Submit(new FormRepository
            {
                Name = analyst.Name,
                Email = analyst.Email,
                Phone = analyst.Phone,
                FormType = 9,
                Country = string.Empty, //countryInfo != null ? countryInfo.country.name : string.Empty,
                Comment = analyst.Message,
                ExtraData = "<b>Call me on</b> - Actual Date - " + analyst.SelectedDate + "<br/> IST - " + analyst.SelectedDate,
                ReportId = analyst.ReportId,
                Designation = string.Empty
            });
        }
    }
}