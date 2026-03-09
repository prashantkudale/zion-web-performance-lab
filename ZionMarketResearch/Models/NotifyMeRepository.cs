using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZionMarketResearch.Models
{
    public class NotifyMeRepository
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(20, ErrorMessage = "Name should not greater than 20 characters.")]
        
        
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required."), StringLength(50, ErrorMessage = "Email should not greater than 50 characters."), RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Invalid Email Id.")]
        public string Email { get; set; }
        public string Phone { get; set; }

        [Display(AutoGenerateField = false)]
        public int CategoryId { get; set; }

        [Display(AutoGenerateField = false)]
        public string ru { get; set; }

        [Display(AutoGenerateField = false)]
        public bool NewReport { get; set; }

        [Display(AutoGenerateField = false)]
        public string UniqueId { get; set; }

        public static bool NotifyMe(NotifyMeRepository notifyMeRepo)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                //save to register
                tblregister reg = new tblregister();
                string[] name = notifyMeRepo.Name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                reg.FirstName = name.Count() > 1 ? name[0] : notifyMeRepo.Name;
                reg.LastName = name.Count() > 1 ? name[1] : string.Empty;
                reg.EmailId = notifyMeRepo.Email;
                reg.Phone = notifyMeRepo.Phone;
                db.tblregisters.Add(reg);
                if (db.SaveChanges() > 0)
                {
                    var urlSegment = notifyMeRepo.ru.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    var url = Util.Utility.Decryptstring(urlSegment);
                    MainPageReportView report = url.Count() > 0 ? ReportRepository.GetReportByUrl(url) : null;

                    //save to subscription with category and report id
                    tblsubscription subscription = new tblsubscription();
                    subscription.RegisterId = reg.Id;
                    subscription.SubscriptionDate = DateTime.Now;
                    subscription.IsEnabled = true;
                    subscription.CategoryId = 0;//notifyMeRepo.NewReport ? report.Categories[0] : 0;
                    subscription.ReportId = report != null ? report.ReportId : 0;
                    subscription.UniqueId = new Random().Next(999999999);
                    db.tblsubscriptions.Add(subscription);

                    //send mail to user
                    Dictionary<string, string> token = new Dictionary<string, string>();
                    token.Add("[MRS:CustomerName]", notifyMeRepo.Name);
                    token.Add("[MRS:MailBody]", "We got your subscription request for <b>&quot;" + report.ReportTitle + "&quot;</b>.<br />We will notify you by email when the report is published.");
                    token.Add("[MRS:UniqueId]", ((int)subscription.UniqueId).ToString());
                    string html = Util.Utility.GetHtml("Subscription", token);

                    SendMail.Send(notifyMeRepo.Email, "Zion Market Research : Subscription", html);

                    return db.SaveChanges() > 0;
                }
                return false;
            }
        }

        public static void NotifyUser(int reportId, string reportTitle, string reportUrl)
        {
            ZionDbEntities db = new ZionDbEntities();
            var users = from r in db.tblregisters join s in db.tblsubscriptions on r.Id equals s.RegisterId where s.ReportId == reportId && (bool)s.IsEnabled select r;
            foreach (var u in users)
            {
                Dictionary<string, string> token = new Dictionary<string, string>();
                token.Add("[MRS:CustomerName]", u.FirstName);
                token.Add("[MRS:MailBody]", "Report <b>&quot;" + reportTitle + "&quot;</b> has been published.<br/><a href='https://www.zionmarketresearch.com/contact-us/'>Conatct Us</a> for more details OR Click <a href='https://www.zionmarketresearch.com/report/" + reportUrl + "'>View Report</a>.");
                string html = Util.Utility.GetHtml("Subscription", token);

                SendMail.Send(u.EmailId, "Zion Market Research : Subscription", html);
            }
        }

        public static bool Unsubscribe(int uniqueId)
        {
            bool res = false;
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var z = db.tblsubscriptions.Where(x => x.UniqueId == uniqueId && x.IsEnabled == true).FirstOrDefault();
                if (z != null)
                {
                    z.IsEnabled = false;
                    db.SaveChanges();
                    res = true;
                }
            }
            return res;
        }
    }
}